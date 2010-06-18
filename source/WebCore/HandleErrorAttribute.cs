/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;

  /// <summary>
  /// This attribute extends the HandleErrorAttribute to use our ErrorModel
  /// that is compatible with the master page.
  /// </summary>
  public class HandleSiteErrorAttribute : System.Web.Mvc.HandleErrorAttribute
  {
    public override void OnException(ExceptionContext filterContext)
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

      Exception exception = filterContext.Exception;

      // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
      // ignore it.
      if (new HttpException(null, exception).GetHttpCode() != 500)
      {
        return;
      }

      if (!ExceptionType.IsInstanceOfType(exception))
      {
        return;
      }

      string controllerName = (string)filterContext.RouteData.Values["controller"];
      string actionName = (string)filterContext.RouteData.Values["action"];
      ErrorModel model = new ErrorModel() { HandleErrorInfo = new HandleErrorInfo(exception, controllerName, actionName) };
      BaseController bc = filterContext.Controller as BaseController;
      if (bc != null) bc.SetModel(model);

      filterContext.Result = new ViewResult
      {
        ViewName = View,
        MasterName = Master,
        ViewData = new ViewDataDictionary<ErrorModel>(model),
        TempData = filterContext.Controller.TempData
      };
      filterContext.ExceptionHandled = true;
      filterContext.HttpContext.Response.Clear();
      filterContext.HttpContext.Response.StatusCode = 500;
      filterContext.HttpContext.Response.Write("asfasdafdssasf sadfljkasdflkjsdalkjsd a jsklkjsa jksda kjasdkjls lkjf kjlasjkldfakjsldfj klasdfjkl asdlkjfaskjldf kjasdjfk asjkldf;sdfa;eoijfoeaw feklameomcvoiamowemw lekmcowim mwoimcvmwoeimalksdm;ailewfmsldkmasldkm ackmvoiasoiae mfkasd mlfkamsoiea;lkdfm oieefmalk emfalimeslakeam iamioa meofmla;sfma;ie mfa;simfea;lskdm;aflwm;l  kfmwe;fmsd;lka;wlefeowifa;slkdfaoeifaemiwqqkkamd falmszpkmdvpkagapsdpfkjdn;kdhgghtujpi[azi;lzkxjckj v;xkzj;iej;zkv z;ijev;ziomvd;aijekaj; afk;jfepqoweqpoudsk;ajsd ksdj;a fpoeipquwr da;kfsj ;alksdj f;aksd");

      // Certain versions of IIS will sometimes use their own error page when
      // they detect a server error. Setting this property indicates that we
      // want it to try to render ASP.NET MVC's error page instead.
      filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
    }
  }
}
