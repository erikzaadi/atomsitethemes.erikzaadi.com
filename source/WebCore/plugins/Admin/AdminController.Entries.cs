/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Utils;

  public partial class AdminController : BaseController
  {
    [ScopeAuthorize]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public ViewResult Entries(string workspace, string collection, string filter, string category, string search, int? page, int? pageSize)
    {
      int pageIndex = page - 1 ?? 0;
      int pSize = pageSize ?? AppService.GetCollection().PageSize;

      var entries = AtomPubService.GetEntries(
        filter == "pending" ? SelectionCriteria.EntriesPending(workspace, collection, search, category) :
        filter == "published" ? SelectionCriteria.EntriesPublished(workspace, collection, search, category) :
        filter == "draft" ? SelectionCriteria.EntriesDraft(workspace, collection, search, category) :
        filter == "unapproved" ? SelectionCriteria.EntriesUnapproved(workspace, collection, search, category) :
        SelectionCriteria.EntriesAll(workspace, collection, search, category), pageIndex, pSize);

      //filter and title links for display
      var rels = new[] { "alternate", "admin-edit", "approve", "unapprove", "delete" };
      foreach (var e in entries)
      {
        e.Links = e.Links.Where(l => rels.Contains(l.Rel));
        var link = e.Links.Where(l => l.Rel == "alternate").FirstOrDefault();
        if (link != null) link.Title = "view";
        link = e.Links.Where(l => l.Rel == "admin-edit").FirstOrDefault();
        if (link != null) link.Title = "edit";
        link = e.Links.Where(l => l.Rel == "delete").FirstOrDefault();
        if (link != null) link.Title = "delete";
        link = e.Links.Where(l => l.Rel == "approve").FirstOrDefault();
        if (link != null) link.Title = "approve";
        link = e.Links.Where(l => l.Rel == "unapprove").FirstOrDefault();
        if (link != null) link.Title = "unapprove";
      }

      var m = new AdminEntriesModel() { Entries = entries };
      m.AllCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.EntriesAll(workspace, collection, null, null));
      m.PublishedCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.EntriesPublished(workspace, collection, null, null));
      m.UnapprovedCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.EntriesUnapproved(workspace, collection, null, null));
      m.DraftCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.EntriesDraft(workspace, collection, null, null));
      m.Filter = filter;


      return View("AdminEntries", "Admin", m);
    }

    [ScopeAuthorize]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public ActionResult EditEntry(string workspace, string collection, string id)
    {
      //TODO: move logic to service
      //TODO: only allow show of create entry page based on authorization
      AtomEntry entry = null;
      if (TempData["entry"] != null) entry = TempData["entry"] as AtomEntry;
      if (entry == null)
      {
        if (id != null) entry = AtomPubService.GetEntry(id);
        else if (Collection != null && Collection.OnlyAcceptsMedia)
        {
          return RedirectToAction("EditMedia", new
          {
            workspace = Collection.Id.Workspace,
            collection = Collection.Id.Collection
          });
        }
        else if (Collection == null)
        {
          //find entry collection in approved scopes
          var scopes = AuthorizeService.GetScopes(User.Identity as User);
          var coll = AppService.GetCollections(scopes).Where(c => c.AcceptsEntries).FirstOrDefault()
            ?? AppService.GetCollection();

          return RedirectToAction("EditEntry", new
          {
            workspace = coll.Id.Workspace,
            collection = coll.Id.Collection
          });
        }
        else
        {
          entry = new AtomEntry()
          {
            Title = new AtomTitle() { Text = "Enter Title" },
            Content = new AtomContent() { Text = string.Empty }
          };
          //TODO: workspace/collection?
        }
      }
      if (entry.Media) throw new Exception("Can't edit media entry");
      if (Collection.OnlyAcceptsMedia) throw new Exception("Selected collection only accepts media.");
      if (!Collection.AcceptsEntries) throw new Exception("Selected collection does not accept new entries.");
      var m = new AdminEntryModel() { Entry = entry };
      if (TempData["error"] != null) m.Errors.Add((string)TempData["error"]);
      if (TempData["created"] != null) m.Notifications.Add("Success", "The new entry '" + entry.Id.EntryPath + "' was successfully created. Please be patient for the changes to appear.");
      if (TempData["updated"] != null) m.Notifications.Add("Success", "The existing entry '" + entry.Id.EntryPath + "' was successfully updated. Please be patient for the changes to appear.");
      return View("AdminEditEntry", "Admin", m);
    }


    [ScopeAuthorize]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public ActionResult EditMedia(string workspace, string collection, string id)
    {
      AtomEntry entry = null;
      if (TempData["entry"] != null) entry = TempData["entry"] as AtomEntry;
      if (entry == null)
      {
        if (id != null) entry = AtomPubService.GetEntry(id);
        else if (Collection != null && Collection.OnlyAcceptsEntries)
        {
          return RedirectToAction("EditEntry", new
          {
            workspace = Collection.Id.Workspace,
            collection = Collection.Id.Collection
          });
        }
        else if (Collection == null || !Collection.AcceptsMedia)
        {
          //find media collection in approved scopes
          var scopes = AuthorizeService.GetScopes(User.Identity as User);
          var coll = AppService.GetCollections(scopes).Where(c => c.AcceptsMedia).FirstOrDefault()
            ?? AppService.GetCollection();

          return RedirectToAction("EditMedia", new
          {
            workspace = coll.Id.Workspace,
            collection = coll.Id.Collection
          });
        }
        else
        {
          entry = new AtomEntry()
          {
            Title = new AtomTitle() { Text = "Enter Title" },
            Content = new AtomContent() { Text = string.Empty },
            Media = true
          };
          //TODO: workspace/collection?
        }
      }
      if (!entry.Media) throw new Exception("Can't edit non-media entry");
      if (Collection.OnlyAcceptsEntries) throw new Exception("Selected collection does not accept media.");
      var m = new AdminMediaModel() { Entry = entry };
      if (TempData["error"] != null) m.Errors.Add((string)TempData["error"]);
      if (TempData["created"] != null) m.Notifications.Add("Success", "The new media '" + entry.Id.EntryPath + "' was successfully created. Please be patient for the changes to appear.");
      if (TempData["updated"] != null) m.Notifications.Add("Success", "The existing media '" + entry.Id.EntryPath + "' was successfully updated. Please be patient for the changes to appear.");
      return View("AdminEditMedia", "Admin", m);
    }

    [ScopeAuthorize]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    [System.Web.Mvc.ValidateInput(false)]
    public ActionResult EditMedia(string workspace, string collection, string id, string title, string slug, string summary, bool? draft, bool? approved, string publishedDate, string publishedTime, string submit, string[] categories, bool? allowAnnotations)
    {

      //TODO: use ModelBinder?
      AtomEntry entry = null;
      if (title == "Enter Title") title = null;
      if (string.IsNullOrEmpty(slug)) slug = title;

      try
      {
        if (Request.Files.Count == 1)
        {
          HttpPostedFileBase hpf = Request.Files[0] as HttpPostedFileBase;
          if (hpf.ContentLength == 0) throw new Exception("Empty file uploaded");
          if (string.IsNullOrEmpty(slug)) slug = title = hpf.FileName;

          if (!string.IsNullOrEmpty(id))
          {
            entry = AtomPubService.UpdateMedia(id, hpf.InputStream, hpf.ContentType);
            TempData["updated"] = true;
          }
          else
          {
            entry = AtomPubService.CreateMedia(AppService.GetCollection(workspace, collection).Id,
            hpf.InputStream, slug, hpf.ContentType);
            TempData["created"] = true;
          }
        }
        if (!string.IsNullOrEmpty(id)) entry = AtomPubService.GetEntry(id);

        if (entry == null) throw new Exception("File not uploaded");

        //TODO: DRY violation with EditEntry
        if (string.IsNullOrEmpty(title)) throw new Exception("Title is required");
        entry.Title = new AtomTitle() { Text = title };

        if (!string.IsNullOrEmpty(summary)) entry.Summary = new AtomSummary() { Text = summary };
        else entry.Summary = null;

        if (entry.Control == null) entry.Control = new AppControl();

        entry.Control.Draft = draft;
        if (submit != "Publish") entry.Control.Draft = true;

        entry.Control.Approved = approved;

        if (publishedDate != null && publishedTime != null)
        {
          try
          {
            entry.Published = DateTimeHelper.GetForTimeZone(publishedDate, publishedTime, Properties.Settings.Default.TimeZoneInfoId);
          }
          catch
          {
            throw new Exception("The publish date or time was invalid.  Please enter a valid date and time.");
          }
        }

        if (allowAnnotations.HasValue) entry.Control.AllowAnnotate = allowAnnotations;
        else entry.Control.AllowAnnotate = null;

        if (categories != null)
        {
          var cats = categories.Select(c => new AtomCategory() { Term = c });
          entry.Categories = cats;
        }

        AtomPubService.UpdateEntry(entry.Id, entry, slug);

        if (submit == "Preview")
          return Redirect(entry.LocationWeb.ToString());

        return RedirectToAction("EditMedia", new { id = entry.Id.ToString() });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        TempData["entry"] = entry;
        TempData["error"] = ex.Message;
        if (id != null)
          return RedirectToAction("EditMedia", new { id = id });
      }
      return RedirectToAction("EditMedia", new { workspace = workspace, collection = collection });
    }

    [ScopeAuthorize]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    [System.Web.Mvc.ValidateInput(false)]
    public ActionResult EditEntry(AdminEditEntryModel m)
    {
      AtomEntry entry = null;
      try
      {
        if (!string.IsNullOrEmpty(m.Id)) entry = AtomPubService.GetEntry(m.Id);
        else entry = new AtomEntry();

        if (string.IsNullOrEmpty(m.Title) || m.Title == "Enter Title") m.Title = m.Slug;
        if (string.IsNullOrEmpty(m.Title)) throw new Exception("Title is required");
        entry.Title = new AtomTitle() { Text = m.Title };

        //TODO: verify contents
        entry.Content = new AtomContent() { Type = "html", Text = m.Content };

        if (!string.IsNullOrEmpty(m.Summary)) entry.Summary = new AtomSummary() { Text = m.Summary };
        else entry.Summary = null;

        if (entry.Control == null) entry.Control = new AppControl();

        entry.Control.Draft = m.Draft;
        if (m.Submit != "Publish") entry.Control.Draft = true;

        entry.Control.Approved = m.Approved;

        if (m.PublishedDate != null && m.PublishedTime != null)
        {
          try
          {
            entry.Published = DateTimeHelper.GetForTimeZone(m.PublishedDate, m.PublishedTime, Properties.Settings.Default.TimeZoneInfoId);
          }
          catch
          {
            throw new Exception("The publish date or time was invalid.  Please enter a valid date and time.");
          }
        }

        if (m.AllowAnnotations.HasValue) entry.Control.AllowAnnotate = m.AllowAnnotations;
        else entry.Control.AllowAnnotate = null;

        if (m.Categories != null)
        {
          var cats = m.Categories.Select(c => new AtomCategory() { Term = c });
          entry.Categories = cats;
        }

        if (string.IsNullOrEmpty(m.Id))
        {
          AtomPubService.CreateEntry(Collection.Id, entry, m.Slug);
          TempData["created"] = true;
        }
        else
        {
          AtomPubService.UpdateEntry(m.Id, entry, m.Slug);
          TempData["updated"] = true;
        }

        if (m.Submit == "Preview")
          return Redirect(entry.LocationWeb.ToString());

        return RedirectToAction("EditEntry", new { id = entry.Id.ToString() });
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        TempData["error"] = ex.Message;
        TempData["entry"] = entry;
        if (m.Id != null)
          return RedirectToAction("EditEntry", new { id = m.Id });
      }
      return RedirectToAction("EditEntry");
    }

  }
}
