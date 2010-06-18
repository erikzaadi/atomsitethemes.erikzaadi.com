/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Linq;
  using System.Net;
  using System.Security;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;
  using System.Collections.Generic;
  using System.Web;
  using System.Web.Configuration;
  using System.Configuration;

  /// <summary>
  /// This is the base controller to be used by all other controllers.  It provides
  /// a way to populate the model with common services and objects.
  /// </summary>
  public abstract class BaseController : Controller
  {
    protected const int SEC = 1;
    protected const int MIN = 60 * SEC;

    private AppService appService = null;
    public AppService AppService { get { return appService; } }

    public ILogService LogService { get; protected set; }
    public IAuthorizeService AuthorizeService { get; protected set; }

    public AppWorkspace Workspace { get { return !Scope.IsEntireSite ? appService.GetWorkspace(Scope.Workspace) : null; } }
    public AppCollection Collection { get { return Scope.IsCollection ? appService.GetCollection(Scope.Workspace, Scope.Collection) : null; } }
    public Id EntryId { get; protected set; }
    public Scope Scope { get; protected set; }

    /// <summary>
    /// Determines dynamically the collection, workspace, entry, and theme
    /// for the current request.
    /// </summary>
    /// <param name="ctx"></param>
    protected override void Execute(RequestContext ctx)
    {
      IContainer container = (IContainer)ctx.HttpContext.Application["Container"];
      appService = container.GetInstance<IAppServiceRepository>().GetService();
      LogService = container.GetInstance<ILogService>();
      AuthorizeService = container.GetInstance<IAuthorizeService>();
      //determine resource context based on routing data
      IRouteService r = RouteService.GetCurrent(ctx);
      if (EntryId == null) EntryId = r.GetEntryId();
      Scope = r.GetScope();
      base.Execute(ctx);

    }

    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext == null)
      {
        throw new ArgumentNullException("filterContext");
      }

      // If custom errors are disabled, we need to let the normal ASP.NET exception handler
      // execute so that the user can see useful debugging information.
      if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
      {
        return;
      }

      LogService.Error(filterContext.Exception);
      var status = GetStatusForError(filterContext.Exception);

      filterContext.ExceptionHandled = true;
      filterContext.HttpContext.Response.Clear();
      filterContext.HttpContext.Response.StatusCode = (int)status.Key;
      filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
      
      // Get the section.
      CustomErrorsSection customErrorsSection = (CustomErrorsSection)filterContext.HttpContext.GetSection("system.web/customErrors");
      
      string url = customErrorsSection.DefaultRedirect;
      // Get the collection
      foreach (CustomError e in customErrorsSection.Errors)
      {
        if (filterContext.HttpContext.Response.StatusCode == e.StatusCode)
          url = e.Redirect ?? url;
      }
      filterContext.HttpContext.Server.Transfer(url);

      //string controllerName = (string)filterContext.RouteData.Values["controller"];
      //string actionName = (string)filterContext.RouteData.Values["action"];
      //if (status.Key == HttpStatusCode.NotFound) NotFound().ExecuteResult(this.ControllerContext);
      //else
      //{
      //  var model = new ErrorModel(new HandleErrorInfo(exception, controllerName, actionName));
      //  Error(model).ExecuteResult(this.ControllerContext);
      //}
    }

    public ViewResult Error(ErrorModel model)
    {
      return View("Error", model ?? UnknownError);
    }

    private readonly ErrorModel UnknownError = new ErrorModel(
      new HandleErrorInfo(new Exception("Unknown exception."), "unknown", "unknown"));

    public ViewResult NotFound()
    {
      return View("NotFound", new PageModel());
    }

    protected virtual ViewResult View(BaseModel model)
    {
      return View(null, model);
    }

    protected virtual ViewResult View(string viewName, string masterViewName, BaseModel model)
    {
      SetModel(model);
      return base.View(viewName, masterViewName, model);
    }

    protected virtual ViewResult View(string viewName, BaseModel model)
    {
      SetModel(model);
      if (viewName != null)
        return base.View(viewName, model);
      else
        return base.View(model);
    }

    protected virtual PartialViewResult PartialView(BaseModel model)
    {
      return PartialView(null, model);
    }

    protected virtual PartialViewResult PartialView(string viewName, BaseModel model)
    {
      SetModel(model);
      if (viewName != null)
        return base.PartialView(viewName, model);
      else
        return base.PartialView(model);
    }

    public virtual void SetModel(BaseModel model)
    {
      model.Logger = LogService;
      model.User = User.Identity as User;
      model.AuthorizeService = AuthorizeService;
      if (model.Service == null) model.Service = AppService;
      if (model.Workspace == null) model.Workspace = Workspace;
      if (model.Collection == null) model.Collection = Collection;
      if (model.EntryId == null) model.EntryId = EntryId;
      model.Scope = Scope;
    }

    /// <summary>
    /// Gets the slug set in the header.
    /// </summary>
    /// <returns></returns>
    protected string GetSlug()
    {
      return HttpContext.Request.Headers["Slug"];
    }

    /// <summary>
    /// Determines if the etag does not match in the header
    /// </summary>
    /// <param name="etag"></param>
    /// <returns></returns>
    protected bool NoneMatch(string etag)
    {
      string match = HttpContext.Request.Headers["If-None-Match"];
      return match != etag;
    }

    /// <summary>
    /// Determines if the etag matches in the header
    /// </summary>
    /// <param name="etag"></param>
    /// <returns></returns>
    protected bool Match(string etag)
    {
      string match = HttpContext.Request.Headers["If-Match"];
      return !(!string.IsNullOrEmpty(match) && match != "*" && match != etag);
    }
    
    /// <summary>
    /// Gets a feed of the given type (rss or atom) and remove's emails
    /// to prevent spam
    /// </summary>
    /// <param name="feed"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected ActionResult GetFeedResult(AtomFeed feed, string type)
    {
      //remove email's to prevent spam
      foreach (AtomPerson person in feed.People.Concat(feed.Entries.SelectMany(e => e.People)))
      {
        person.Email = null; //string.Empty;
      }

      if (type == "atom")
      {
        return new XmlWriterResult((w) => feed.Xml.WriteTo(w))
        {
          ContentType = Atom.ContentTypeFeed
        };
      }
      else if (type == "rss")
      {
        return new XmlWriterResult((w) => feed.WriteRssTo(w))
        {
          ContentType = "text/xml"
        };
      }
      else
      {
        throw new ResourceNotFoundException(type + " feed", "requested");
      }
    }

    protected virtual KeyValuePair<HttpStatusCode, string> GetStatusForError(Exception ex)
    {
      if (ex is ResourceNotFoundException)
      {
        return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotFound, ex.Message);
      }
      else if (ex is UserNotAuthenticatedException || ex is UserNotAuthorizedException)
      {
        //let the forms auth bypass module know to remove the redirect to login form
        HttpContext.Items["FormsAuthBypass"] = true;
        return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Unauthorized, ex.Message);
      }
      else if (ex is SecurityException)
      {
        return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, ex.Message);
      }
      else if (ex is InvalidContentTypeException)
      {
        return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.NotAcceptable, ex.Message);
      }
      else if (ex is BaseException)
      {
        return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, ex.Message);
      }
      else
      {
          return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.InternalServerError,
            (HttpContext.IsDebuggingEnabled) ? ex.ToString() :
            "An error occurred. Please contact the administrator if the problem persists.");       
      }
    }  
  }
}
