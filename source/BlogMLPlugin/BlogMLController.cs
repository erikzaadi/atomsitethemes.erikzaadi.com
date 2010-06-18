/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.BlogMLPlugin
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Web;
  using System.Web.Mvc;
  using System.Xml;
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using BlogML.Xml;
using AtomSite.Repository;

  public class BlogMLController : BaseController
  {
    protected IBlogMLService BlogMLService;
    protected IAppServiceRepository AppServiceRepository;

    public BlogMLController(IBlogMLService svc, IAppServiceRepository svcRepo)
    {
      BlogMLService = svc;
      AppServiceRepository = svcRepo;
    }

    [AcceptVerbs("GET")]
    public ActionResult ImportToolWidget()
    {
      var m = Session["BlogMLModel"] as BlogMLModel;
      if (m == null) m = new BlogMLModel();
      return PartialView("BlogMLImportToolWidget", m);
    }
    [AcceptVerbs("GET")]
    public ActionResult ExportToolWidget()
    {
      return PartialView("BlogMLExportToolWidget", new BlogMLModel());
    }
    [AcceptVerbs("GET")]
    public ActionResult WizardImport()
    {
      BlogMLWizardImportModel m = new BlogMLWizardImportModel()
        {
          BlogName = "blog",
          PagesName = "pages",
          MediaName = "media",
          Owner = "example.com",
          Year = DateTime.Today.Year
        };
      return View("BlogMLWizardImport", "Wizard", m);
    }

    [AcceptVerbs("POST")]
    public ActionResult WizardImport(BlogMLWizardImportModel m)
    {
      try
      {
            //validate
            if (string.IsNullOrEmpty(m.Owner) || m.Owner.Trim().Length == 0)
                ModelState.AddModelError("owner", "Owner is required.");

            if (m.Year < 1990 || m.Year > 2090)
                ModelState.AddModelError("year", "Please choose a valid year.");

            if (string.IsNullOrEmpty(m.BlogName) || m.BlogName.Trim().Length == 0)
                ModelState.AddModelError("blogName", "The blog name is required.");
          
            if (m.BlogMLFile == null && Request.Files.Count > 0) m.BlogMLFile = Request.Files[0];

            if (m.BlogMLFile == null || m.BlogMLFile.ContentLength < 10)
                ModelState.AddModelError("blogmlfile", "Please post a valid BlogML file.");

            if (ModelState.IsValid)
            {
                ProcessBlogMLFile(m);
                return RedirectToAction("ThemeChoice", "Wizard");
            }        
      }
      catch (Exception ex)
      {
          ModelState.AddModelError("error", ex);
            LogService.Error(ex);
      }
      return View("BlogMLWizardImport", "Wizard", m);
    }

    private void ProcessBlogMLFile(BlogMLWizardImportModel m)
    {
        BlogMLBlog blog = null;
        using (XmlReader r = new XmlTextReader(m.BlogMLFile.InputStream))
        {
            blog = BlogMLSerializer.Deserialize(r);
        }

        BlogMLService.Import(new Id(m.Owner, m.Year.ToString(), m.BlogName),
          !string.IsNullOrEmpty(m.PagesName) ? new Id(m.Owner, m.Year.ToString(), m.PagesName) : null,
          !string.IsNullOrEmpty(m.MediaName) ? new Id(m.Owner, m.Year.ToString(), m.MediaName) : null, ImportMode.New, blog);

        var uri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath);
        if (!uri.ToString().EndsWith("/")) uri = new Uri(uri.ToString() + "/");
        AppService appSvc = AppServiceRepository.GetService();
        appSvc.Base = uri;
        AppServiceRepository.UpdateService(appSvc);
    }

    [AcceptVerbs("POST")]
    public ActionResult Import(BlogMLModel m)
    {
      //validate input
      try
      {
        HttpPostedFileBase blogml = null;
        if (blogml == null && Request.Files.Count > 0) blogml = Request.Files[0];
        var coll = AppService.GetCollection(new Uri(m.EntryCollectionId));
        if (string.IsNullOrEmpty(m.PagesCollectionId)) m.PagesCollectionId = null;
        if (string.IsNullOrEmpty(m.MediaCollectionId)) m.MediaCollectionId = null;
        if (string.IsNullOrEmpty(m.ImportMode)) m.ImportMode = "Merge";
        if (blogml == null || blogml.ContentLength < 10) throw new Exception("Please post a valid BlogML file.");

        BlogMLBlog blog = null;
        using (XmlReader r = new XmlTextReader(blogml.InputStream))
        {
          blog = BlogMLSerializer.Deserialize(r);
        }

        AdminProgressModel model = new AdminProgressModel() { ProcessName = "BlogML Import" };
        model.Messages.Add("BlogML file parsed successfully");

        //launch a new thread
        new Thread(() => BlogMLService.Import(m.EntryCollectionId, m.PagesCollectionId, m.MediaCollectionId,
          (ImportMode)Enum.Parse(typeof(ImportMode), m.ImportMode), blog)) { IsBackground = true }.Start();
        //ThreadPool.QueueUserWorkItem((s) => Import(s), state);

        model.ProgressUrl = Url.Action("ImportProgress");
        TempData["BlogML-AdminProgressModel"] = m;
        return RedirectToAction("ImportResult");
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        TempData["error"] = ex.Message;
        Session["BlogMLModel"] = m;
        return RedirectToAction("Tools", "Admin");
      }
    }

    public ViewResult ImportResult()
    {
      var m = TempData["BlogML-AdminProgressModel"] as AdminProgressModel;
      if (m == null) m = new AdminProgressModel() { ProcessName = "BlogML Import", ProgressUrl = Url.Action("ImportProgress") };
      return View("AdminProgress", "Admin", m);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(Duration=0, VaryByParam="None")]
    public JsonResult ImportProgress()
    {
      return Json(BlogMLService.GetProgress());
    }
        
    [AcceptVerbs("POST")]
    public ActionResult Export(string sourceEntryCollectionId, string sourcePagesCollectionId, string sourceMediaCollectionId)
    {
      try
      {
        var coll = AppService.GetCollection(new Uri(sourceEntryCollectionId));
        var mediaColl = AppService.GetCollection(new Uri(sourceMediaCollectionId));
        BlogMLBlog blog = BlogMLService.Export(sourceEntryCollectionId, sourcePagesCollectionId, 
          sourceMediaCollectionId);
        using (MemoryStream ms = new MemoryStream())
        {
          BlogMLSerializer.Serialize(ms, blog);
          ms.Position = 0;
          byte[] bytes = new byte[ms.Length];
          ms.Read(bytes, 0, (int)ms.Length);
          return new FileContentResult(bytes, "text/xml") { FileDownloadName = coll.Id.Collection + ".blogml.xml" };
        }        
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        TempData["error"] = ex.Message;
        return RedirectToAction("Tools", "Admin");
      }
    }
  }
}
