/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Web;
  using System.Web.Routing;
  using AtomSite.Domain;
  using System.Linq;

  /// <summary>
  /// This class provides a way to constrain a route to certian collections.
  /// </summary>
  public class CollectionRouteConstraint : IRouteConstraint
  {
    public IEnumerable<Id> CollectionIds { get; set; }
    public CollectionRouteConstraint(IEnumerable<Id> collectionIds)
    {
      CollectionIds = collectionIds;
    }

    public bool Match(HttpContextBase httpContext, System.Web.Routing.Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      bool noMatch = true;
      if (parameterName.ToLowerInvariant() == "collectionids")
      {
        foreach (Id id in CollectionIds)
        {
          if (values.ContainsKey("workspace"))
          {
            noMatch &= String.Compare(id.Workspace, (string)values["workspace"], true) != 0;
          }
          if (values.ContainsKey("collection"))
          {
            noMatch &= String.Compare(id.Collection, (string)values["collection"], true) != 0;
          }
        }
      }
      return !noMatch;
    }

  }

  public class PathRouteConstraint : IRouteConstraint
  {
    protected static readonly string[] restricted = new []
    { "search"
    }.Concat(Enumerable.Range(1996, 2026).Select(i=>i.ToString())).ToArray();
    public bool Match(HttpContextBase httpContext, System.Web.Routing.Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
      if (parameterName.ToLowerInvariant() == "path")
      {
        return !restricted.Contains((string)values["path"], StringComparer.InvariantCultureIgnoreCase);
      }
      return true;
    }

  }
}
