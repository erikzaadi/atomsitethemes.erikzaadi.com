/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using StructureMap;

  public partial class AdminController : BaseController
  {
    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    public ViewResult Widgets(AdminWidgetsModel m)
    {
      LoadWidgetsData(m);
      return View("AdminWidgets", "Admin", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult AddTarget(AdminWidgetsModel model, string error)
    {
      var m = new AdminWidgetSelectModel();
      m.SelectionTitle = "Select New Page or Widget";

      // get list of pages from container            
      var container = (IContainer)HttpContext.Application["container"];
      //SupportedScopes scope = SupportedScopes.EntireSite; //TODO: get correct scope

      m.WidgetSelections = container.GetAllInstances<IPage>()//.Where(w => (w.SupportedScopes & scope) == scope)
          .Select(p => new WidgetSelect()
          {
            Name = p.Name,
            Description = string.Format("{0} with {1} area{2}",
              p.Parent == null ? "<em>Master</em> page" : "Page", p.Areas.Count(), p.Areas.Count() == 1 ? string.Empty : "s"),
            ScopeFlags = (int)p.SupportedScopes,
            Icon = "page",
            PostHref = Url.Action("AddTarget", new
            {
              workspace = Scope.Workspace,
              collection = Scope.Collection,
              targetType = "page",
              targetName = p.Name
            })
          }).Concat(container.GetAllInstances<IWidget>().Where(w => w.Areas.Count() > 0) //.Where(w => (w.SupportedScopes & scope) == scope)
          .Select(w => new WidgetSelect()
          {
            Name = w.Name,
            Description = w.Description,
            ScopeFlags = (int)w.SupportedScopes,
            Icon = "widget",
            PostHref = Url.Action("AddTarget", new
            {
              workspace = Scope.Workspace,
              collection = Scope.Collection,
              targetType = "widget",
              targetName = w.Name
            })
          }));

      m.Error = error;
      m.CancelHref = Url.Action("AdminTargets", new
      {
        workspace = Scope.Workspace,
        collection = Scope.Collection,
        targetType = model.TargetType,
        targetName = model.TargetName,
        pageName = model.PageName
      });
      return PartialView("AdminWidgetSelect", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddTarget(AdminWidgetsModel model)
    {
      try
      {
        var appSvc = AdminService.AddTarget(Scope, model.TargetName, model.TargetType == "page");
        LoadWidgetsData(model, appSvc);
      }
      catch (Exception ex)
      {
        // return select model with error message
        LogService.Error(ex);
        return AddTarget(model, ex.Message);
      }
      // return updated target listings
      return View("AdminWidgets", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    public ActionResult RemoveTarget(AdminWidgetsModel model)
    {
      try
      {
        var appSvc = AdminService.RemoveTarget(Scope, model.TargetName, model.TargetType == "page");
        LoadWidgetsData(model, appSvc);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        model.Errors.Add(ex.Message);
      }
      return View("AdminWidgets", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult AdminTargets(AdminWidgetsModel model)
    {
      // return updated target listings
      LoadWidgetsData(model);
      return PartialView("AdminTargets", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult AddArea(AdminWidgetsModel model, string error)
    {
      var m = new AdminWidgetSelectModel();
      m.SelectionTitle = "Select an Area";

      IEnumerable<string> areas = null;
      var container = (IContainer)HttpContext.Application["container"];
      if (model.TargetType == "page")
      {
        var pages = container.GetAllInstances<IPage>();
        IPage page = pages.Where(p => p.Name == model.TargetName).Single();
        IPage parent = pages.Where(p => p.Name == page.Parent).SingleOrDefault();
        if (parent != null) areas = page.Areas.Concat(parent.Areas);
        else areas = page.Areas;
      }
      else
      {
        var widgets = container.GetAllInstances<IWidget>();
        IWidget widget = widgets.Where(w => w.Name == model.TargetName).Single();
        areas = widget.Areas;
      }

      m.WidgetSelections = areas.Select(a => new WidgetSelect()
      {
        Name = a,
        //Icon = a,
        PostHref = Url.Action("AddArea", new
        {
          workspace = Scope.Workspace,
          collection = Scope.Collection,
          targetType = model.TargetType,
          targetName = model.TargetName,
          areaName = a
        })
      });

      m.Error = error;
      m.CancelHref = Url.Action("AdminAreas", new
      {
        workspace = Scope.Workspace,
        collection = Scope.Collection,
        targetType = model.TargetType,
        targetName = model.TargetName,
        pageName = model.PageName
      });
      return PartialView("AdminWidgetSelect", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddArea(AdminWidgetsModel model)
    {
      TargetBase target = model.TargetType == "page" ? (TargetBase)new ServicePage() { Name = model.TargetName } :
                (TargetBase)new ServiceWidget() { Name = model.TargetName };
      try
      {
        var appSvc = AdminService.AddArea(Scope, model.TargetName, model.TargetType == "page", model.AreaName);
        LoadWidgetsData(model, appSvc);
      }
      catch (Exception ex)
      {
        // return select model with error message
        LogService.Error(ex);
        return AddArea(model, ex.Message);
      }
      // return updated area listings
      return View("AdminWidgets", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    public ActionResult RemoveArea(AdminWidgetsModel model)
    {
      TargetBase target = model.TargetType == "page" ? (TargetBase)new ServicePage() { Name = model.TargetName } :
                (TargetBase)new ServiceWidget() { Name = model.TargetName };
      try
      {
        var appSvc = AdminService.RemoveArea(Scope, model.TargetName, model.TargetType == "page", model.AreaName);
        LoadWidgetsData(model, appSvc);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        model.Errors.Add(ex.Message);
      }
      return View("AdminWidgets", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult AdminAreas(AdminWidgetsModel model)
    {
      // return updated target listings
      LoadWidgetsData(model);
      return PartialView("AdminAreas", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult AddInclude(AdminWidgetsModel model, string error)
    {
      AdminWidgetSelectModel m = new AdminWidgetSelectModel();
      m.SelectionTitle = "Select a Widget Include";

      var container = (IContainer)HttpContext.Application["container"];
      //SupportedScopes scope = SupportedScopes.EntireSite; //TODO: get correct scope

      m.WidgetSelections = container.GetAllInstances<IWidget>()//.Where(w => (w.SupportedScopes & scope) == scope)
          .Select(w => new WidgetSelect()
          {
            Name = w.Name,
            Description = w.Description ?? "This widget has no description.",
            ScopeFlags = (int)w.SupportedScopes,
            Icon = "widget",
            IsHint = w.AreaHints.Contains(model.AreaName),
            ScopeMatch = IsScopeMatch(container, w.SupportedScopes, model.TargetType, model.TargetName),
            PostHref = Url.Action("AddInclude", new
            {
              workspace = Scope.Workspace,
              collection = Scope.Collection,
              targetType = model.TargetType,
              targetName = model.TargetName,
              areaName = model.AreaName,
              includeName = w.Name
            })
          }).OrderByDescending(w => w.IsHint).ThenByDescending(w => w.ScopeMatch);
      m.Error = error;
      m.CancelHref = Url.Action("AdminIncludes", new
      {
        workspace = Scope.Workspace,
        collection = Scope.Collection,
        targetType = model.TargetType,
        targetName = model.TargetName,
        pageName = model.PageName
      });
      return PartialView("AdminWidgetSelect", m);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    public ActionResult AddInclude(AdminWidgetsModel model)
    {
      try
      {
        var appSvc = AdminService.AddInclude(Scope, model.TargetName, model.TargetType == "page", model.AreaName, model.IncludeName);
        LoadWidgetsData(model, appSvc);
      }
      catch (Exception ex)
      {
        // return select model with error message
        LogService.Error(ex);
        return AddInclude(model, ex.Message);
      }

      return View("AdminWidgets", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
    public ActionResult RemoveInclude(AdminWidgetsModel model, int includeIdx)
    {
      try
      {
        var appSvc = AdminService.RemoveInclude(Scope, model.TargetName, model.TargetType == "page", model.AreaName, includeIdx);
        LoadWidgetsData(model, appSvc);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        model.Errors.Add(ex.Message);
      }
      return View("AdminWidgets", "Admin", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
    public PartialViewResult AdminIncludes(AdminWidgetsModel model)
    {
      // return updated target listings
      LoadWidgetsData(model);
      return PartialView("AdminIncludes", model);
    }

    [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
    [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)] //TODO: fix some bug in jquery won't post will only get
    public ActionResult MoveInclude(AdminWidgetsModel model, int includeIdx, bool down)
    {
      try
      {
        var appSvc = AdminService.MoveInclude(Scope, model.TargetName, model.TargetType == "page", model.AreaName, includeIdx, down);
        LoadWidgetsData(model, appSvc);
        return PartialView("AdminIncludes", model);
      }
      catch (Exception ex)
      {
        LogService.Error(ex);
        return Json(new { error = ex.Message });
      }
    }

    private void LoadWidgetsData(AdminWidgetsModel m, AppService appSvc)
    {
      //TODO: move into service
      var targets = new List<Target>();

      var container = (IContainer)HttpContext.Application["container"];
      var instances = container.Model.AllInstances.Where(i => i.PluginType.Equals(typeof(IWidget)));
      var widgets = instances.Select(i => i.Get<IWidget>()).ToList();
      //var widgets = container.GetAllInstances<IWidget>();
      var pages = container.GetAllInstances<IPage>();

      //load data based on scope
      if (Scope.IsWorkspace || Scope.IsCollection)
      {
        var w = appSvc.GetWorkspace(Scope.Workspace);
        if (Scope.IsCollection)
        {
          var c = w.GetCollection(Scope.Collection);
          var cTargets = c.Pages.Cast<TargetBase>().Concat(c.Widgets.Cast<TargetBase>());
          targets.AddRange(cTargets.Select(p => GetTarget(p, !Scope.IsCollection, pages, widgets, m)));
        }
        var wTargets = w.Pages.Cast<TargetBase>().Concat(w.Widgets.Cast<TargetBase>())
          .Select(p => GetTarget(p, !Scope.IsWorkspace, pages, widgets, m));
        MergeTargets(wTargets, targets);
      }
      var sTargets = appSvc.Pages.Cast<TargetBase>().Concat(appSvc.Widgets.Cast<TargetBase>())
        .Select(p => GetTarget(p, !Scope.IsEntireSite, pages, widgets, m));
      MergeTargets(sTargets, targets);

      // select first target
      if (string.IsNullOrEmpty(m.TargetName) && targets.Count > 0)
      {
        m.TargetName = targets[0].Name;
        m.TargetType = targets[0].IsPage ? "page" : "widget";
      }
      m.Targets = targets.OrderBy(t => t.Inherited);

      // get areas for target
      var target = m.Targets.Where(t => t.Name == m.TargetName && t.IsPage == (m.TargetType == "page")).FirstOrDefault();
      m.Areas = (target != null) ? target.Areas.OrderBy(a => a.Inherited).ToArray() : new Area[] { };

      // select first area
      if (m.AreaName == null && m.Areas.Count() > 0) m.AreaName = m.Areas.First().Name;

      var area = m.Areas.Where(a => a.Name == m.AreaName).FirstOrDefault();
      m.Includes = (area != null) ? area.Includes.OrderBy(a => a.Inherited).ToArray() : new WidgetInclude[] { };
    }

    private void MergeTargets(IEnumerable<Target> upLevelTargets, List<Target> targets)
    {
      //TODO: move into service
      foreach (Target target in upLevelTargets)
      {
        Target existing = targets.Find(t => t.Equals(target));
        if (existing == null) targets.Add(target);
        else
        {
          foreach (Area area in target.Areas)
          {
            Area existingArea = existing.Areas.Where(a => a.Name == area.Name).FirstOrDefault();
            if (existingArea == null) existing.Areas = existing.Areas.Concat(new Area[] { area });
            else
            {
              existingArea.Includes = existingArea.Includes.Concat(area.Includes);
            }
          }
        }
      }
    }

    private Target GetTarget(TargetBase t, bool inherited, IList<IPage> pages, IList<IWidget> widgets, AdminWidgetsModel m)
    {
      Target target = new Target()
      {
        Name = t.Name,
        IsPage = t is ServicePage,
        Inherited = inherited,
        SelectHref = Url.Action("Widgets", new
        {
          workspace = Scope.Workspace,
          collection = Scope.Collection,
          targetType = t is ServicePage ? "page" : "widget",
          targetName = t.Name
        }),
        RemoveHref = inherited ? null : Url.Action("RemoveTarget", new
        {
          workspace = Scope.Workspace,
          collection = Scope.Collection,
          targetType = t is ServicePage ? "page" : "widget",
          targetName = t.Name
        }),
        Areas = t.Areas.Select(a => new Area()
        {
          Name = a.Name,
          Inherited = inherited,
          SelectHref = Url.Action("Widgets", new
          {
            workspace = Scope.Workspace,
            collection = Scope.Collection,
            targetType = t is ServicePage ? "page" : "widget",
            targetName = t.Name,
            areaName = a.Name
          }),
          RemoveHref = inherited ? null : Url.Action("RemoveArea", new
          {
            workspace = Scope.Workspace,
            collection = Scope.Collection,
            targetType = t is ServicePage ? "page" : "widget",
            targetName = t.Name,
            areaName = a.Name
          }),
          Includes = a.Includes.Select(i => GetInclude(inherited, t.Name, t is ServicePage, a.Name, a.Includes.ToList().IndexOf(i), i, widgets, m)).ToList()
        }).ToList()
      };

      if (target.IsPage)
      {
        IPage page = pages.Where(p => p.Name == target.Name).FirstOrDefault();
        if (page != null)
        {
          int cnt = page.Areas.Count();
          target.ScopeFlags = (int)page.SupportedScopes;
          target.Parent = page.Parent;
          IPage parent = pages.Where(p => p.Name == page.Parent).FirstOrDefault();
          if (parent != null) cnt += parent.Areas.Count();
          target.TotalAreaCount = cnt;
          //target.Desc = page.Description;
        }
      }
      else
      {
        IWidget widget = widgets.Where(w => w.Name == target.Name).FirstOrDefault();
        if (widget != null)
        {
          target.ScopeFlags = (int)widget.SupportedScopes;
          target.TotalAreaCount = widget.Areas.Count();
          //target.Desc = widget.Description;
        }
      }

      target.Desc = string.Format("{0} with {1} include{2} in {3} of {4} area{5}",
        target.IsMasterPage ? "<em>Master</em>" : target.IsPage ? "Page" : "Customizable widget",
        target.IncludeCount, target.IncludeCount == 1 ? string.Empty : "s",
        target.AreaCount, target.TotalAreaCount, target.TotalAreaCount == 1 ? string.Empty : "s");
      return target;
    }

    private WidgetInclude GetInclude(bool inherited, string targetName, bool isPage, string areaName, int index, Include include, IList<IWidget> widgets, AdminWidgetsModel m)
    {
      var i = new WidgetInclude()
      {
        IncludePath = AppService.GetIncludePath(include),
        Name = include.Name,
        Inherited = inherited,
        RemoveHref = inherited ? null : Url.Action("RemoveInclude", new
        {
          workspace = Scope.Workspace,
          collection = Scope.Collection,
          targetType = isPage ? "page" : "widget",
          targetName = targetName,
          areaName = areaName,
          includeIdx = index
        }),
        MoveHref = inherited ? null : Url.Action("MoveInclude", new
        {
          workspace = Scope.Workspace,
          collection = Scope.Collection,
          targetType = isPage ? "page" : "widget",
          targetName = targetName,
          areaName = areaName,
          includeIdx = index
        }),
      };

      var widget = widgets.Where(w => w.Name == i.Name).FirstOrDefault();
      if (widget != null)
      {
        i.Desc = widget.Description;
        i.ScopeFlags = (int)widget.SupportedScopes;
        i.ConfigInclude = inherited ? null : widget.GetConfigInclude(i.IncludePath);
        i.Valid = widget.IsValid(include);
      }
      if (string.IsNullOrEmpty(i.Desc)) i.Desc = "Widget has no description.";
      return i;
    }

    private void LoadWidgetsData(AdminWidgetsModel m)
    {
      LoadWidgetsData(m, AppService);
    }

    private int IsScopeMatch(IContainer container, SupportedScopes supportedScopes, string targetType, string targetName)
    {
      if (targetType == "page")
      {
        IPage page = container.GetAllInstances<IPage>().Where(p => p.Name == targetName).FirstOrDefault();
        if (page != null) return (int)(page.SupportedScopes & supportedScopes);
      }
      else
      {
        IWidget widget = container.GetAllInstances<IWidget>().Where(p => p.Name == targetName).FirstOrDefault();
        if (widget != null) return (int)(widget.SupportedScopes & supportedScopes);
      }
      return 0;
    }
  }
}
