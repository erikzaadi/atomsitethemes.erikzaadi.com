/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.Repository;

  public class MenuController : BaseController
  {
    protected IAppServiceRepository AppServiceRepository;

    public MenuController(IAppServiceRepository svcRepo)
    {
      AppServiceRepository = svcRepo;
    }

    //TODO: add caching
    public ActionResult MenuWidget(string workspace, string collection, int? year,
      int? month, int? day, string path)
    {
      MenuModel model = new MenuModel();

      //add home link
      model.MenuItems.Add(new MenuItem()
      {
        Href = Url.Content("~/"),
        Text = "Home",
        Selected = Request.Url.AbsoluteUri.ToLower()
        .StartsWith(Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath)
      });

      //add link for each visible collection in the workspace
      if (Workspace != null)
      {
        foreach (AppCollection coll in Workspace.Collections.Where(c => c.Visible))
        {
          if (!coll.Visible) continue;
          string view = "AtomPubCollectionIndex";
          if (!string.IsNullOrEmpty(coll.DefaultView)) view = coll.DefaultView;
          model.MenuItems.Add(new MenuItem()
          {
            Href = Url.RouteIdUrl(view, coll.Id),
            Title = coll.Title.ToString(),
            Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(coll.Id.Collection),
            Selected = Request.Url.AbsoluteUri.ToLower()
            .StartsWith(Url.RouteIdUrl(view, coll.Id, AbsoluteMode.Force).Split('.').First().ToLower())
          });
        }
      }
      EnforceMaxSelection(model.MenuItems);
      return PartialView("MenuWidget", model);
    }

    //TODO: add caching?
    public ActionResult CustomWidget(Include include)
    {
      MenuModel model = new MenuModel();
      var menuItems = new MenuCustomInclude(include).MenuItems.Select(mi => new MenuItem()
      {
        Href = mi.Href,
        Title = mi.Title,
        Text = mi.Text,
        ExactSelect = mi.ExactSelect
      }).ToList();

      if (menuItems.Count() < 1) throw new ArgumentException("Include is missing custom menu items.");

      // mark selected, first pass do exact
      bool exact = false;
      foreach (MenuItem item in menuItems.Where(mi => mi.ExactSelect))
      {
        bool selected = item.Href.Substring(item.Href.IndexOf('/')) == Request.Url.PathAndQuery;
        if (selected)
        {
          item.Selected = true;
          exact = true;
          break;
        }
      }

      // no exact match, do fuzzy match
      if (!exact)
      {
        foreach (MenuItem item in menuItems.Where(mi => !mi.ExactSelect))
        {
          int lastSlash = item.Href.LastIndexOf('/');
          if (lastSlash < 1) lastSlash = item.Href.Length - 1;
          string areaPath = item.Href.Substring(0, lastSlash).Split('.').First().ToLower();
          item.Selected = Request.Url.PathAndQuery.ToLower().StartsWith(areaPath);
        }
      }

      foreach (MenuItem item in menuItems) model.MenuItems.Add(item);
      EnforceMaxSelection(model.MenuItems);
      return PartialView("MenuWidget", model);
    }

    protected void EnforceMaxSelection(IList<MenuItem> items)
    {
      //unselect if multple are selected
      var selected = items.Where(mi => mi.Selected);
      foreach (var sel in selected.OrderByDescending(s => s.Href.Length).Skip(1))
      {
        sel.Selected = false;
      }
      if (items.Where(mi => mi.Selected).Count() > 1)
        throw new Exception("There are too many items in the menu are selected.");
    }


    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult MenuConfig(MenuConfigModel m)
    {
      var include = AppService.GetInclude<MenuCustomInclude>(m.IncludePath);
      m.Menu = string.Concat(include.MenuItems.Select(mi =>
        string.Format("{0};{1};{2};{3}{4}", mi.Text, mi.Href, mi.Title,
        (mi.ExactSelect) ? "1" : "0", Environment.NewLine)).ToArray());
      return PartialView("MenuConfig", m);
    }

    [ScopeAuthorize(Action = AuthAction.UpdateServiceDoc, Roles = AuthRoles.AuthorOrAdmin)]
    [AcceptVerbs(HttpVerbs.Post)]
    [ActionName("MenuConfig")]
    [ValidateInput(false)]
    public ActionResult PostMenuConfig(MenuConfigModel m)
    {
      if (string.IsNullOrEmpty(m.Menu))
        ModelState.AddModelError("menu", "You must supply at least one menu item.");

      var lines = m.Menu.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      if (lines.Where(l => l.Split(';').Length < 2).Count() > 0)
        ModelState.AddModelError("menu", "Each menu item must have at least some text and href.");

      if (ModelState.IsValid)
      {
        var appSvc = AppServiceRepository.GetService();
        var include = appSvc.GetInclude<MenuCustomInclude>(m.IncludePath);

        IList<CustomMenuItem> items = new List<CustomMenuItem>();
        foreach (string line in lines)
        {
          CustomMenuItem item = new CustomMenuItem();
          var vals = line.Split(';');
          item.Text = vals[0].Trim();
          item.Href = vals[1].Trim();
          if (vals.Length > 2 && vals[2].Trim().Length > 0) item.Title = vals[2];
          if (vals.Length > 3 && vals[3].Trim().Length > 0) item.ExactSelect = vals[3] == "1";
          items.Add(item);
        }
        include.MenuItems = items;
        AppServiceRepository.UpdateService(appSvc);
        return Json(new { success = true, includePath = m.IncludePath });
      }

      return PartialView("MenuConfig", m);
    }
  }
}
