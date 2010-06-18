/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.BlogMLPlugin
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Security.Principal;
  using System.Xml.Linq;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore;
  using BlogML.Xml;

  public enum ImportMode
  {
    Merge,
    New,
    Overwrite
  }

  public class BlogMLService : IBlogMLService
  {
    protected IAppServiceRepository AppServiceRepository;
    protected IAtomPubService AtomPubService;
    protected IAnnotateService AnnotateService;
    protected IUserRepository UserRepository;
    protected IAuthorizeService AuthorizeService;
    protected ILogService LogService;
    
    //public event Action<string, byte> ProgressUpdate;

    volatile Progress progress;

    public BlogMLService(IAppServiceRepository svcRepo, IAtomPubService atompub, IAnnotateService annotate,
      IUserRepository usersRepo, IAuthorizeService auth, ILogService logger)
    {
      AppServiceRepository = svcRepo;
      AtomPubService = atompub;
      AnnotateService = annotate;
      UserRepository = usersRepo;
      AuthorizeService = auth;
      LogService = logger;
      progress = new Progress();
    }

    protected void LogProgress(string message, params object[] formatParameters)
    {
      LogProgress(string.Format(message, formatParameters));
    }

    protected void LogProgress(string message)
    {
      LogProgress(message, 0);
    }

    protected void LogProgress(string message, byte percentComplete)
    {
      LogService.Info(message);
      progress.Messages.Add(message);
      progress.PercentComplete = percentComplete;
    }

    public Progress GetProgress()
    {
      var p = new Progress();
      while (progress.Messages.Count > 0)
      {
        p.Messages.Add(progress.Messages[0]);
        progress.Messages.RemoveAt(0);
      }
      p.PercentComplete = progress.PercentComplete;
      return p;
    }

    #region Export
    public BlogMLBlog Export(Id entryCollectionId, Id pagesCollectionId, Id mediaCollectionId)
    {
      LogService.Info("Beginning export of collection with Id='{0}'", entryCollectionId);
      BlogMLBlog blog = new BlogMLBlog();
      AppService appSvc = AtomPubService.GetService();

      AppCollection coll = appSvc.GetCollection(entryCollectionId);

      blog.Title = coll.Title.Text;
      if (coll.Subtitle != null) blog.SubTitle = coll.Subtitle.Text;
      blog.RootUrl = coll.Href.ToString();

      //extended properties
      blog.ExtendedProperties.Add(new BlogML.Pair<string, string>("CommentModeration", AuthorizeService.IsAuthorized(AuthRoles.Anonymous, coll.Id.ToScope(),
        AuthAction.ApproveAnnotation) ? "Anonymous" : "Authenticated"));
      blog.ExtendedProperties.Add(new BlogML.Pair<string, string>("SendTrackback", new BlogAppCollection(coll).TrackbacksOn ? "Yes" : "No"));

      foreach (BlogMLCategory cat in coll.AllCategories.Select(c => new BlogMLCategory()
      {
        ID = c.Term,
        Approved = true,
        DateCreated = DateTime.UtcNow,
        DateModified = DateTime.UtcNow,
        Title = c.ToString()
      })) { blog.Categories.Add(cat); }

      IPagedList<AtomEntry> entries = null;
      int page = 0;
      do
      {
        entries = AtomPubService.GetEntries(new EntryCriteria() { WorkspaceName = entryCollectionId.Workspace, CollectionName = entryCollectionId.Collection, Authorized = true },
        page, 100); page++;
        foreach (AtomEntry entry in entries)
        {
          try
          {
            LogService.Info("Processing entry with ID='{0}'", entry.Id);
            AddEntry(entry, blog);
          }
          catch (Exception ex)
          {
            LogService.Error(ex);
          }
        }
      } while (entries.PageIndex < entries.PageCount);

      LogService.Info("Finished export!");
      return blog;
    }

    private void AddEntry(AtomEntry entry, BlogMLBlog blog)
    {
      BlogMLPost post = new BlogMLPost()
      {
        Approved = entry.Visible,
        Content = new BlogMLContent() { Text = entry.Content.ToString() },
        DateCreated = entry.Published.HasValue ? entry.Published.Value.DateTime : entry.Updated.DateTime,
        DateModified = entry.Updated.DateTime,
        HasExcerpt = entry.Summary != null,
        Excerpt = entry.Summary != null ? new BlogMLContent() { Text = entry.Summary.Text } : null,
        ID = entry.Id.ToString(),
        PostName = entry.Id.EntryPath,
        PostType = BlogML.BlogPostTypes.Normal,
        PostUrl = entry.LocationWeb.ToString(),
        Title = entry.Title.ToString()
      };

      foreach (AtomPerson author in entry.Authors)
      {
        BlogMLAuthor a = new BlogMLAuthor()
        {
          Approved = true,
          DateCreated = DateTime.UtcNow,
          DateModified = DateTime.UtcNow,
          Email = author.Email,
          ID = author.Id,
          Title = author.Name
        };

        if (blog.Authors.Where(x => x.ID == a.ID).Count() == 0) blog.Authors.Add(a);
        post.Authors.Add(new BlogMLAuthorReference() { Ref = a.ID });
      }

      foreach (AtomCategory cat in entry.Categories)
      {
        BlogMLCategory c = new BlogMLCategory()
        {
          ID = cat.Term,
          Approved = false,
          DateCreated = DateTime.UtcNow,
          DateModified = DateTime.UtcNow,
          Title = cat.ToString()
        };
        if (blog.Categories.Where(x => x.ID == c.ID).Count() == 0) blog.Categories.Add(c);
        post.Categories.Add(new BlogMLCategoryReference() { Ref = c.ID });
      }


      IPagedList<AtomEntry> anns = null;
      int page = 0;
      do
      {
        anns = AtomPubService.GetEntries(new EntryCriteria() { EntryId = entry.Id, Annotations = true, Authorized = true, Deep = true },
        page, 100); page++;
        foreach (AtomEntry ann in anns)
        {
          try
          {
            LogService.Info("Processing annotation with ID='{0}'", ann.Id);
            AddAnnotation(ann, post, blog);
          }
          catch (Exception ex)
          {
            LogService.Error(ex);
          }
        }
      } while (anns.PageIndex < anns.PageCount);

      //TODO: attachments
      //post.Attachments.Add(new BlogMLAttachment()
      //{

      //});

      blog.Posts.Add(post);
    }
    private void AddAnnotation(AtomEntry ann, BlogMLPost post, BlogMLBlog blog)
    {
      if (ann.AnnotationType.EndsWith("back"))
        AddTrackback(ann, post, blog);
      else
        AddComment(ann, post, blog);
    }
    private void AddComment(AtomEntry ann, BlogMLPost post, BlogMLBlog blog)
    {
      post.Comments.Add(new BlogMLComment()
      {
        Approved = ann.Visible,
        DateCreated = ann.Published.HasValue ? ann.Published.Value.DateTime : ann.Updated.DateTime,
        DateModified = ann.Updated.DateTime,
        ID = ann.Id.ToString(),
        Title = ann.Title.ToString(),
        Content = new BlogMLContent() { Text = ann.Content.ToString() },
        UserEMail = ann.Authors.First().Email,
        UserName = ann.Authors.First().Name,
        UserUrl = ann.Authors.First().Uri != null ? ann.Authors.First().Uri.ToString() : null
      });
    }
    private void AddTrackback(AtomEntry ann, BlogMLPost post, BlogMLBlog blog)
    {
      post.Trackbacks.Add(new BlogMLTrackback()
      {
        Approved = ann.Visible,
        DateCreated = ann.Published.HasValue ? ann.Published.Value.DateTime : ann.Updated.DateTime,
        DateModified = ann.Updated.DateTime,
        ID = ann.Id.ToString(),
        Title = ann.Title.ToString(),
        Url = ann.Content.Src.ToString()
      });
    }
    #endregion Export

    public void Import(Id entryCollectionId, Id pagesCollectionId, Id mediaCollectionId, ImportMode mode, BlogMLBlog blog)
    {
      progress = new Progress();
      LogProgress("Import started, mode={0}", mode);
      LogProgress("Blog found with title of '{0}'", blog.Title);
      try
      {
        AppService appSvc = AppServiceRepository.GetService();

        if (mode == ImportMode.New)
        {
          var ws = appSvc.GetWorkspace();
          //clean out old collections
          CleanOutCollection(ws.GetCollection("blog").Id, appSvc);
          CleanOutCollection(ws.GetCollection("pages").Id, appSvc);
          CleanOutCollection(ws.GetCollection("media").Id, appSvc);

          ws.Title = new AtomTitle() { Text = entryCollectionId.Owner };

          //change old id's to new id's
          ChangeCollectionId(appSvc, ws.GetCollection("blog").Id, entryCollectionId);
          ChangeCollectionId(appSvc, ws.GetCollection("pages").Id, pagesCollectionId);
          ChangeCollectionId(appSvc, ws.GetCollection("media").Id, mediaCollectionId);
        }
        else if (mode == ImportMode.Overwrite)
        {
          CleanOutCollection(entryCollectionId, appSvc);
          CleanOutCollection(pagesCollectionId, appSvc);
          CleanOutCollection(mediaCollectionId, appSvc);
        }

        TurnOffTrackbacks(entryCollectionId, appSvc);
        TurnOffTrackbacks(pagesCollectionId, appSvc);
        TurnOffTrackbacks(mediaCollectionId, appSvc);

        var coll = appSvc.GetCollection(entryCollectionId);

        if (mode != ImportMode.Merge)
        {
          coll.Title = new AtomTitle() { Text = blog.Title };
          //this is workspace subtitle, not collection
          appSvc.GetWorkspace().Subtitle = new AtomSubtitle() { Text = blog.SubTitle };
        }

        var users = ImportUsers(blog, appSvc);
        if (users.FirstOrDefault() != null && mode == ImportMode.New)
        {
          //user is likely not authenticated when new, so auth them
          System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(users[0], null);
        }

        ImportCategories(blog, coll);
        AtomPubService.UpdateService(appSvc);
        ImportPosts(entryCollectionId, pagesCollectionId, mediaCollectionId, mode, blog);

        //make sure there is an About and Blogroll pages
        if (mode == ImportMode.New && pagesCollectionId != null)
        {
          var entries = AtomPubService.GetEntries(new EntryCriteria()
          {
            EntryId = pagesCollectionId.AddPath("About"),
            Approved = true
          }, 0, 1);
          if (entries.Count() == 0)
          {
            AtomPubService.CreateEntry(pagesCollectionId, new AtomEntry()
            {
              Title = new AtomTitle() { Text = "About Me" },
              Content = new AtomContent() { Text = "This is a temporary placeholder until I can update this entry." },
            }, "About");
          }
          entries = AtomPubService.GetEntries(new EntryCriteria()
          {
            EntryId = pagesCollectionId.AddPath("Blogroll"),
            Approved = true
          }, 0, 1);
          if (entries.Count() == 0)
          {
            AtomPubService.CreateEntry(pagesCollectionId, new AtomEntry()
            {
              Title = new AtomTitle() { Text = "Blog Roll" },
              Content = new AtomContent() { Type = "html", Text = "<ul><li><a href='http://atomsite.net'>AtomSite.net</a></li></ul>" },
            }, "Blogroll");
          }
        }

        ResetTrackbacks(entryCollectionId, appSvc);
        ResetTrackbacks(pagesCollectionId, appSvc);
        ResetTrackbacks(mediaCollectionId, appSvc);

        LogProgress("Finished!", 100);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        LogProgress(System.Web.HttpUtility.HtmlEncode(ex.Message), 100);
      }
    }

    private void TurnOffTrackbacks(Id collId, AppService appSvc)
    {
      if (collId == null) return;
      var bcoll = new BlogAppCollection(appSvc.GetCollection(collId));
      //store old value
      bcoll.SetBooleanProperty(Atom.SvcNs + "tracbacksWereOn", bcoll.TrackbacksOn);
      bcoll.TrackbacksOn = false;
    }

    private void ResetTrackbacks(Id collId, AppService appSvc)
    {
      if (collId == null) return;
      var bcoll = new BlogAppCollection(appSvc.GetCollection(collId));
      //get old value
      bool? on = bcoll.GetBooleanProperty(Atom.SvcNs + "tracbacksWereOn");
      bcoll.TrackbacksOn = on.HasValue ? on.Value : true;
      bcoll.SetBooleanProperty(Atom.SvcNs + "tracbacksWereOn", null);
    }

    private void ChangeCollectionId(AppService appSvc, Id oldId, Id newId)
    {
      if (newId == null) return;

      //update old id's in service.config
      //update include ids
      foreach (XElement xid in appSvc.Xml.Descendants(Atom.SvcNs + "id"))
      {
        if (xid.Value.StartsWith(oldId.ToString()))
        {
          string id = newId.ToString() + xid.Value.Substring(oldId.ToString().Length);
          LogProgress("Changing old include id from {0} to {1}", xid, id);
          xid.SetValue(id);
        }
      }
      LogProgress("Changing old collection id from {0} to {1}", oldId, newId);
      appSvc.GetCollection(oldId).Id = newId;
    }

    private void CleanOutCollection(Id collId, AppService appSvc)
    {
      if (collId == null) return;
      LogProgress("Cleaning out old entries and categories in collection with id={0}", collId);
      //just clean out existing entries
      AtomPubService.DeleteCollection(collId, false);
      var coll = appSvc.GetCollection(collId);
      coll.Categories = null;
    }

    void ImportCategories(BlogMLBlog blog, AppCollection coll)
    {      
      var cats = coll.Categories.FirstOrDefault();
      if (cats == null)
      {
        cats = new AppCategories();
        coll.Categories = Enumerable.Repeat(cats, 1);
      }

      foreach (BlogMLCategory cat in blog.Categories)
      {
        LogProgress("Processing blog category with ID={0}", cat.ID);
        if (cats.AddCategory(new AtomCategory() { Term = cat.Title }))
          LogProgress("Adding blog category with ID={0}", cat.ID);
        else
          LogProgress("Blog category with ID={0} already exists", cat.ID);
      }
    }

    IList<User> ImportUsers(BlogMLBlog blog, AppService appSvc)
    {
      var users = new List<User>();
      foreach (BlogMLAuthor author in blog.Authors)
      {
        LogProgress("Processing blog author with ID={0}", author.ID);
        var user = UserRepository.GetUsersByEmail(author.Email).FirstOrDefault();
        if (user == null) user = UserRepository.GetUser(author.ID);
        if (user == null) user = UserRepository.GetUsersByName(author.Title).FirstOrDefault();
        if (user != null)
        {
          LogProgress("User '{0}' already exists in system", author.Title);
          var ids = user.Ids.ToList();
          if (!ids.Contains(author.ID)) ids.Add(author.ID);
          user.Ids = ids;
          UserRepository.UpdateUser(user);
        }
        else
        {
          LogProgress("Existing user not found, creating new user '{0}'", author.Title);
          user = new User();
          user.Name = author.Title;
          user.Email = author.Email;
          user.Ids = new[] { author.ID };
          UserRepository.CreateUser(user);
          LogProgress("Making new user '{0}' an administrator", author.Title);
          appSvc.AddAdmin(user.Ids.First());
        }
        users.Add(user);
      }
      return users;
    }

    void ImportPosts(Id entryCollectionId, Id pagesCollectionId, Id mediaCollectionId, ImportMode mode, BlogMLBlog blog)
    {

      foreach (BlogMLPost post in blog.Posts)
      {
        LogProgress("Loading post with ID={0}", post.ID);

        try
        {
          AtomEntry entry = new AtomEntry();
          entry.Title = new AtomTitle() { Text = post.Title };
          if (post.HasExcerpt) entry.Summary = new AtomSummary() { Text = post.Excerpt.UncodedText };
          entry.Content = new AtomContent() { Type = "html", Text = post.Content.Text };
          entry.Control = new AppControl();
          entry.Control.Approved = post.Approved;

          //categories
          var ecats = new List<AtomCategory>();
          foreach (BlogMLCategoryReference cat in post.Categories)
          {
            var ec = blog.Categories.Where(c => c.ID == cat.Ref).SingleOrDefault();
            if (ec == null) throw new Exception("Invalid category reference in BlogML file.");
            ecats.Add(new AtomCategory() { Term = ec.Title });
          }
          entry.Categories = ecats;

          //authors
          var eauths = new List<AtomPerson>();
          for (int i = 0; i < post.Authors.Count; i++)
          {
            var user = UserRepository.GetUser(post.Authors[i].Ref);
            if (user == null) throw new Exception("Invalid author reference in BlogML file.");
            eauths.Add(user.ToAtomAuthor());
          }
          entry.Authors = eauths;

          entry.Published = post.DateCreated;
          entry.Updated = post.DateModified;
          
          ////fix links http://localhost:1333 to new address
          //string xmlstr = entry.Xml.ToString();
          //xmlstr = xmlstr.Replace("http://localhost:1333/", appSvc.Base.ToString());
          //entry.Xml = XElement.Parse(xmlstr);

          //TODO: add old link
          //entry.Links =

          //post to blog when normal, otherwise pages collection
          if (post.PostType == BlogML.BlogPostTypes.Normal)
            entry = AtomPubService.CreateEntry(entryCollectionId, entry, post.PostName);
          else if (pagesCollectionId != null)
            entry = AtomPubService.CreateEntry(pagesCollectionId, entry, post.PostName);
          else
          {
            LogProgress("Skipping {0} posts as there is no pages collection selected", post.PostType.ToString());
            continue;
          }
          
          LogProgress("Post added as new entry in collection with ID={0}", entry.Id);

          if (mediaCollectionId != null)
          {
            foreach (BlogMLAttachment attachment in post.Attachments)
            {
              try
              {
                string name = null;
                if (!string.IsNullOrEmpty(attachment.Path)) name = Path.GetFileNameWithoutExtension(attachment.Path);
                if (name == null && !string.IsNullOrEmpty(attachment.Url)) name = attachment.Url.IndexOf('/') > 0 ?
                attachment.Url.Substring(attachment.Url.LastIndexOf('/')) : attachment.Url;
                if (name == null) name = "Attachment";
                Stream stream = null;
                if (attachment.Embedded)
                {
                  LogProgress("Processing embedded attachment '{0}'", name);
                  stream = new MemoryStream(attachment.Data);
                }
                else if (!string.IsNullOrEmpty(attachment.Path) && File.Exists(attachment.Path))
                {
                  LogProgress("Processing local attachment '{0}'", name);
                  //first try to get locally
                  stream = File.OpenRead(attachment.Path);
                }
                else
                {
                  LogProgress("Processing remote attachment '{0}'", name);
                  //next try to download
                  stream = new WebClient().OpenRead(attachment.Url);
                }
                var media = AtomPubService.CreateMedia(mediaCollectionId, stream, name, attachment.MimeType);
                LogProgress("New media created from attachment with Id={0}", media.Id);
                //update entry with new media url
                entry.Content.Text = entry.Content.Text.Replace(attachment.Url, media.Content.Src.ToString());
              }
              catch (Exception ex)
              {
                LogService.Error(ex);
                LogProgress(System.Web.HttpUtility.HtmlEncode(ex.Message));
              }
            }
          }

          foreach (BlogMLComment comment in post.Comments)
          {
            try
            {
              LogProgress("Processing comment for post with ID={0}", comment.ID);
              AtomEntry acmt = new AtomEntry();
              acmt.Control = new AppControl();
              acmt.Title = new AtomTitle() { Text = string.IsNullOrEmpty(comment.Title) ? "Comment" : comment.Title };
              acmt.Content = new AtomContent() { Type = "html", Text = comment.Content.Text };
              acmt.Published = comment.DateCreated;
              acmt.Updated = comment.DateModified;
              acmt.Control.Approved = comment.Approved;
              //TODO: detect if user and set ID
              acmt.Authors = Enumerable.Repeat(new AtomPerson(Atom.AtomNs + "author")
              {
                Email = comment.UserEMail,
                Name = comment.UserName,
                Uri = !string.IsNullOrEmpty(comment.UserUrl) ? new Uri(comment.UserUrl) : null
              }, 1);

              acmt = AnnotateService.Annotate(entry.Id, acmt, comment.ID);

              LogProgress("Comment added as new annotation on entry with ID={0}", acmt.Id);
            }
            catch (Exception ex)
            {
              LogService.Error(ex);
              LogProgress(System.Web.HttpUtility.HtmlEncode(ex.Message));
            }
          }

          foreach (BlogMLTrackback trackback in post.Trackbacks)
          {
            try
            {
              LogProgress("Processing trackback for post with ID={0}", trackback.ID);

              AtomEntry atb = new AtomEntry();
              atb.AnnotationType = "trackback";
              atb.Title = new AtomTitle() { Text = trackback.Title };
              atb.Control = new AppControl();
              atb.Control.Approved = trackback.Approved;
              atb.Published = trackback.DateCreated;
              atb.Updated = trackback.DateModified;
              atb.Content = new AtomContent() { Src = new Uri(trackback.Url), Type = "text/html" };
              atb.Authors = new List<AtomPerson>() { new AtomAuthor() 
                  { 
                      Name = "Trackback",// string.Empty,
                      Uri = new Uri(trackback.Url)
                  } };

              AnnotateService.Annotate(entry.Id, atb, trackback.ID);
              LogProgress("Trackback added as new annotation on entry with ID={0}", atb.Id);
            }
            catch (Exception ex)
            {
              LogService.Error(ex);
              LogProgress(System.Web.HttpUtility.HtmlEncode(ex.Message));
            }
          }
        }
        catch (Exception ex)
        {
          LogService.Error(ex);
          LogProgress(System.Web.HttpUtility.HtmlEncode(ex.Message));
        }
      }
    }

  }
}
