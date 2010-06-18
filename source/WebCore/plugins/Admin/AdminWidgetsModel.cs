/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using AtomSite.Domain;

  public class AdminWidgetsModel : AdminModel
  {
    public string TargetType { get; set; }
    public string TargetName { get; set; }
    public string AreaName { get; set; }
    public string IncludeName { get; set; }

    public IEnumerable<Target> Targets { get; set; }
    public IEnumerable<Area> Areas { get; set; }
    public IEnumerable<WidgetInclude> Includes { get; set; }

    protected override void OnUpdatePageModel(StructureMap.IContainer container)
    {
      base.OnUpdatePageModel(container);

      // special, update page model with config includes
      foreach (Include include in Includes.Select(i => i.ConfigInclude).Where(i => i != null))
      {
        var w = container.TryGetInstance<IWidget>(include.Name);
        if (w != null) w.UpdatePageModel(this, include);
      }
    }

    public object GetRouteData()
    {
      return new
      {
        workspace = Scope.Workspace,
        collection = Scope.Collection,
        targetType = TargetType,
        targetName = TargetName,
        areaName = AreaName
      };
    }
  }

  public class Target
  {
    public Target() { Areas = new List<Area>(); }

    public string Name { get; set; }
    public string Parent { get; set; }
    public string Desc { get; set; }
    public IEnumerable<Area> Areas { get; set; }
    public bool Inherited { get; set; }
    public int AreaCount { get { return Areas.Count(); } }
    public int TotalAreaCount { get; set; }
    public int IncludeCount { get { return Areas.Where(a => a != null).SelectMany(a => a.Includes).Count(); } }
    public int ScopeFlags { get; set; }
    public bool IsPage { get; set; }
    public bool IsMasterPage { get { return IsPage && Parent == null; } }
    public string Icon { get { return IsPage ? "page" : "widget"; } }
    public string SelectHref { get; set; }
    public string RemoveHref { get; set; }

    // override object.Equals
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      return this.Name == ((Target)obj).Name && this.IsPage == ((Target)obj).IsPage;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
      return this.IsPage.GetHashCode() ^ this.Name.GetHashCode();
    }
  }

  public class Area
  {
    public Area() { Includes = new List<WidgetInclude>(); }

    public string Name { get; set; }
    public int IncludeCount { get { return Includes.Count(); } }
    public bool Inherited { get; set; }
    public IEnumerable<WidgetInclude> Includes { get; set; }
    public string SelectHref { get; set; }
    public string RemoveHref { get; set; }

    // override object.Equals
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }
      return (Name.Equals(((Area)obj).Name));
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }
  }

  public class WidgetInclude
  {
    public string IncludePath { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int ScopeFlags { get; set; }
    public ConfigInclude ConfigInclude { get; set; }
    public string SelectHref { get; set; }
    public string RemoveHref { get; set; }
    public string MoveHref { get; set; }
    public bool Inherited { get; set; }
    public bool Valid { get; set; }
    // override object.Equals
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }
      return (IncludePath == ((WidgetInclude)obj).IncludePath && Name.Equals(((WidgetInclude)obj).Name));
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
      return IncludePath.GetHashCode() ^ Name.GetHashCode();
    }
  }

}
