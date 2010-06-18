/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Diagnostics;
  using System.Reflection;
  using System.Security.Principal;
  using System.Threading;
  using System.Web;
  using System.Web.Hosting;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using StructureMap;
  using System.Xml.Linq;
  using System.IO;

  public class ServerApp : HttpApplication
  {
    public static readonly Version CurrentVersion;
    public static readonly string CurrentRelease;
    public static readonly Version Version09 = new Version(0, 9, 0, 0);
    public static readonly Version Version10 = new Version(1, 0, 0, 0);
    public static readonly Version Version11 = new Version(1, 1, 0, 0);
    public static readonly Version Version12 = new Version(1, 2, 0, 0);
    public static readonly Version Version13 = new Version(1, 3, 0, 0);
    public static readonly Version Version131 = new Version(1, 3, 1, 0);
    public static readonly Version Version14 = new Version(1, 4, 0, 0);
    public static readonly Version Version15 = new Version(1, 5, 0, 0);
    public static readonly AspNetHostingPermissionLevel CurrentTrustLevel;

    static ServerApp()
    {
      CurrentRelease = "AtomSite Widgets Release"; // FileVersionInfo.GetVersionInfo(typeof(ServerApp).Assembly.Location).ProductName;
      CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
      CurrentTrustLevel = GetCurrentTrustLevel();
    }

    public IContainer Container
    {
      get { return Application["Container"] as Container; }
      protected set { Application["Container"] = value; }
    }
    public ILogService LogService { get { return this.Container.GetInstance<ILogService>(); } }

    protected void Application_Start()
    {
      Container = new Container(new FileRegistry(HostingEnvironment.ApplicationPhysicalPath));

      LogService.Info("Application is starting");

      ViewEngines.Engines.Clear();
      //default view engine
      ViewEngines.Engines.Add(new ThemeViewEngine());

      //use IoC controller factory
      ControllerBuilder.Current.SetControllerFactory(typeof(DynamicControllerFactory));

      //modularity
      PluginEngine.LoadPlugins(Container, RouteTable.Routes, ViewEngines.Engines, ModelBinders.Binders, Server.MapPath("~/"), Server.MapPath("~/bin"));

      //default route
      RouteTable.Routes.MapRoute("Default", @"{controller}/{action}");
      Container.Inject<RouteCollection>(RouteTable.Routes);
      LogService.Debug(Container.WhatDoIHave());

      LogService.Info("Cache size bytes={0} percent used={1}", HttpRuntime.Cache.EffectivePrivateBytesLimit, HttpRuntime.Cache.EffectivePercentagePhysicalMemoryLimit);
    }

    protected void Application_End()
    {
      LogService.Info("Application is shutting down");
    }

    protected void Application_Error()
    {
      Exception lastException = Server.GetLastError();
      LogService.Fatal(lastException);
    }

    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
      foreach (var auth in Container.GetAllInstances<IAuthenticateService>())
      {
        auth.Authenticate(this);
      }
    }

    protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
    {
      foreach (var auth in Container.GetAllInstances<IAuthenticateService>())
      {
        auth.PostAuthenticate(this);
      }

      User user = Context.User.Identity as User;
      if (user == null)
      {
        if (Context.User.Identity.IsAuthenticated)
        {
          //fix FormsAuth Principal
          IUserRepository userRepo = Container.GetInstance<IUserRepository>();
          user = userRepo.GetUser(Context.User.Identity.Name);
          if (user != null) Context.User = new GenericPrincipal(user, new string[0]);
        }
        //anon
        if (user == null) Context.User = new GenericPrincipal(
           new User() { Name = string.Empty }, new string[0]);
        System.Threading.Thread.CurrentPrincipal = Context.User;
      }
    }

    public static void Restart()
    {
      //restart app
      ThreadPool.QueueUserWorkItem((state) =>
      {
        System.Threading.Thread.Sleep(1500);
        //if (CurrentTrustLevel > AspNetHostingPermissionLevel.Medium)
        //  System.Web.HttpRuntime.UnloadAppDomain();
        //else
        //{
          // support medium trust by saving file in bin folder (above method causes security exception)
          string path = Path.Combine(System.Web.HttpRuntime.BinDirectory, "restart.txt");
          File.WriteAllText(path, "This file used to force restart of application.");
        //}
      });
    }


    protected static readonly AspNetHostingPermissionLevel[] TrustLevels = {
        AspNetHostingPermissionLevel.Unrestricted,
        AspNetHostingPermissionLevel.High,
        AspNetHostingPermissionLevel.Medium,
        AspNetHostingPermissionLevel.Low,
        AspNetHostingPermissionLevel.Minimal 
    };

    protected static AspNetHostingPermissionLevel GetCurrentTrustLevel()
    {
      foreach (var trustLevel in TrustLevels)
      {
        try
        {
          new AspNetHostingPermission(trustLevel).Demand();
        }
        catch (System.Security.SecurityException)
        {
          continue;
        }
        return trustLevel;
      }
      return AspNetHostingPermissionLevel.None;
    }

  }
}
