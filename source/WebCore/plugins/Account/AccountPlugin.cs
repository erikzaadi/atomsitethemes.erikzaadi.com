/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using StructureMap;
  using System.Web.Routing;
  using System.Web.Mvc;
  using AtomSite.Repository;
  using StructureMap.Attributes;
  using AtomSite.Domain;

  public class AccountPlugin : BasePlugin
  {
    public AccountPlugin(ILogService logger)
      : base(logger)
    {
      DefaultMerit = (int)MeritLevel.High - 20;
    }

    public override void Register(IContainer container, List<SiteRoute> routes, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders, ICollection<Asset> globalAssets)
    {
      container.Configure(x =>
      {
        x.For<IAuthorizeService>().Add<ServiceAuthorizeService>();
      });

      RegisterController<AccountController>(container);

      RegisterWidget(container, new ViewWidget("AccountStatusWidget")
      {
        Description = "Shows a login link, or if already logged in, the user name and a logout link.",
        SupportedScopes = SupportedScopes.All,
        AreaHints = new[] { "nav", "foot" }
      });

      RegisterPage(container, new Page("AccountLogin", "Site")
      {
        SupportedScopes = SupportedScopes.EntireSite
      });
    }

    public override PluginState Setup(IContainer container, string appPath)
    {
      LogService.Info("Setting up Account plugin");

      base.SetupIncludeInPageArea(container, "Site", "foot", "AccountStatusWidget");

      //when version is 0.9, update namespace and author ids
      if (this.GetType().Assembly.GetName().Version == ServerApp.Version09)
        UpdateAuthorizationAndPersonIds(container);

      return base.Setup(container, appPath);
    }

    protected virtual void UpdateAuthorizationAndPersonIds(IContainer container)
    {
      LogService.Info("Updating authorization and person ids");

      IAtomEntryRepository entryRepo = container.GetInstance<IAtomEntryRepository>();
      AppService appSvc = container.GetInstance<IAppServiceRepository>().GetService();
      int total;
      var users = container.GetInstance<IUserRepository>().GetUsers(0, int.MaxValue, out total);

      if (users.Where(u => u.Ids.Count() == 0).Count() > 0)
        throw new Exception("User(s) found with no Id.  All user's must have at least one Id.");

      try
      {
        //Update Ids for Admins
        var people = appSvc.GetXmlValues<AtomPerson>(Atom.SvcNs + "admin").Where(p => p.Xml.Elements().Count() > 0);
        if (people != null && people.Count() > 0)
        {
          var ids = appSvc.Admins != null ? appSvc.Admins.ToList() : new List<string>();
          appSvc.Admins = GetIdsForPeople(people, users).Concat(appSvc.Admins);
        }
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
      }

      //Update Ids for Authors and Contributors
      foreach (AppWorkspace w in appSvc.Workspaces)
      {
        try
        {
          var people = w.GetXmlValues<AtomPerson>(Atom.AtomNs + "author").Where(p => p.Xml.Elements().Count() > 0);
          if (people != null && people.Count() > 0)
          {
            var ids = w.Authors != null ? w.Authors.ToList() : new List<string>();
            w.Authors = GetIdsForPeople(people, users).Concat(w.Authors);
          }

          people = w.GetXmlValues<AtomPerson>(Atom.AtomNs + "contributor").Where(p => p.Xml.Elements().Count() > 0);
          if (people != null && people.Count() > 0)
          {
            var ids = w.Contributors != null ? w.Contributors.ToList() : new List<string>();
            w.Contributors = GetIdsForPeople(people, users).Concat(w.Contributors);
          }
        }
        catch (Exception ex)
        {
          LogService.Error(ex);
        }

        foreach (AppCollection c in w.Collections)
        {
          try
          {

            var people = c.GetXmlValues<AtomPerson>(Atom.AtomNs + "author").Where(p => p.Xml.Elements().Count() > 0);
            if (people != null && people.Count() > 0)
            {
              var ids = c.Authors != null ? c.Authors.ToList() : new List<string>();
              c.Authors = GetIdsForPeople(people, users).Concat(c.Authors);
            }

            people = c.GetXmlValues<AtomPerson>(Atom.AtomNs + "contributor").Where(p => p.Xml.Elements().Count() > 0);
            if (people != null && people.Count() > 0)
            {
              var ids = c.Contributors != null ? c.Contributors.ToList() : new List<string>();
              c.Contributors = GetIdsForPeople(people, users).Concat(c.Contributors);
            }

            //update author id's
            foreach (AtomEntry e in entryRepo.GetEntries(new EntryCriteria()
              {
                Authorized = true,
                WorkspaceName = c.Id.Workspace,
                CollectionName = c.Id.Collection
              }, 0, int.MaxValue, out total))
            {
              LogService.Info("Updating {0}", e.Id.EntryPath);
              NormalizeEntry(e, users, entryRepo);
            }

          }
          catch (Exception ex)
          {
            LogService.Error(ex);
          }
        }
      }
    }

    protected virtual void NormalizeEntry(AtomEntry entry, IEnumerable<User> users,
      IAtomEntryRepository repo)
    {
      try
      {
        FixAuthors(entry, users);
        entry.Edited = DateTimeOffset.UtcNow;
        repo.UpdateEntry(entry); //this should clean up namespaces as well
        int total;
        foreach (AtomEntry annotation in repo.GetEntries(new EntryCriteria()
        {
          EntryId = entry.Id,
          Annotations = true,
          Authorized = true,
          Deep = false
        }, 0, int.MaxValue, out total))
        {
          LogService.Info("Updating {0}", annotation.Id.EntryPath);
          try
          {
            NormalizeEntry(annotation, users, repo);
          }
          catch (Exception ex)
          {
            LogService.Error(ex);
          }
        }
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
      }
    }

    protected virtual void FixAuthors(AtomEntry entry, IEnumerable<User> users)
    {
      try
      {
        List<AtomPerson> authors = entry.Authors != null ? entry.Authors.ToList() : new List<AtomPerson>();

        if (authors.Count > 1)
          LogService.Warn(" Detected multiple authors: {0}", entry.Authors.Count());

        bool matched = false;
        for (int i = authors.Count - 1; i >= 0; i--)
        {
          if (authors[i].Email == null) continue;

          if (matched && users.Select(u => u.Email.ToLowerInvariant())
            .Contains(authors[i].Email.ToLowerInvariant()))
          {
            LogService.Warn(" Duplicate author removed: {0}", authors[i].Name);
            authors.RemoveAt(i);
            continue;
          }

          if (users.Select(u => u.Email.ToLowerInvariant())
            .Contains(authors[i].Email.ToLowerInvariant()))
          {
            AtomPerson person = users.Where(u => u.Email.ToLowerInvariant() ==
              authors[i].Email.ToLowerInvariant()).First().ToAtomAuthor();

            if (authors[i].Id != person.Id || authors[i].Name != person.Name ||
                authors[i].Uri != person.Uri || authors[i].Email != person.Email)
            {
              authors[i].Xml = person.Xml;
              entry.Edited = DateTimeOffset.UtcNow;
              LogService.Info(" Author updated: {0}", authors[i].Id);
            }
            matched = true;
          }
        }

        if (authors.Count == 0)
        {
          AtomPerson person = users.First().ToAtomAuthor();
          //users.Where(u => u.Ids.Contains(svc.Admins.First())).Single().ToAtomAuthor();
          LogService.Warn(" Missing author added: {0}", person.Id);
          authors.Add(person);
        }

        entry.Authors = authors;
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
      }
    }

    protected List<string> GetIdsForPeople(IEnumerable<AtomPerson> people, IEnumerable<User> users)
    {
      List<string> ids = new List<string>();
      foreach (AtomPerson p in people)
      {
        string id = users.Where(u => u.Email.ToLowerInvariant() == p.Email.ToLowerInvariant())
          .First().Ids.First();
        LogService.Info("Adding person id {0} based on previous email {1}", id, p.Email);
        ids.Add(id);
      }
      return ids;
    }
  }
}
