/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Web.Mvc;
  using System.Web.Routing;
  using StructureMap;

  public class DynamicControllerFactory : IControllerFactory
  {
    public IController CreateController(RequestContext requestContext, string controllerName)
    {
      IContainer container = (IContainer)requestContext.HttpContext.Application["Container"];
      container.EjectAllInstancesOf<RequestContext>();
      container.Inject<RequestContext>(requestContext);
      IController c = container.TryGetInstance<IController>(controllerName + "Controller");
      if (c == null) c = container.GetInstance<IController>();
      return c;
    }

    public void ReleaseController(IController controller)
    {
      if (controller is IDisposable)
      {
        (controller as IDisposable).Dispose();
      }
      controller = null;
    }
  }
}
