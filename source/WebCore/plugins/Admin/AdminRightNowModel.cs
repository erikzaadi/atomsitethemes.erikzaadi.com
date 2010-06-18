/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using AtomSite.Domain;

  public class AdminRightNowModel : BaseModel
  {
    public AdminRightNowModel()
    {
      CurrentThemes = new Dictionary<Scope, string>();
      Metrics = new List<ScopeMetric>();
    }
    public IDictionary<Scope, string> CurrentThemes { get; set; }
    public string Version { get; set; }
    public string ReleaseName { get; set; }
    public Scope HighestScope { get; set; }
    public IList<ScopeMetric> Metrics { get; set; }
    public bool HasAnnotations()
    {
        return Metrics.SelectMany(m => m.Metrics.Keys).Where(k => k.StartsWith("ann")).Count() > 0;
    }

    public IEnumerable<AppWorkspace> GetMetricWorkspaces(string startsWith)
    {
      return ScopesWithMetricsFor(startsWith)
        .Select(m => m.Scope.Workspace).Distinct().Select(w => Service.GetWorkspace(w));
    }

    public IEnumerable<AppCollection> GetMetricCollections(AppWorkspace w, string startsWith)
    {
      return ScopesWithMetricsFor(startsWith)
        .Where(m => m.Scope.Workspace == (w.Name ?? Atom.DefaultWorkspaceName))
        .Select(m => Service.GetCollection(m.Scope.Workspace, m.Scope.Collection));
    }

    public IEnumerable<ScopeMetric> ScopesWithMetricsFor(string startsWith)
    {
      return Metrics.Where(m => m.Metrics.Keys.Where(k => k.StartsWith(startsWith)).Count() > 0);
    }

    public int GetMetric(AppCollection c, string metricName)
    {
      return Metrics.Where(m => m.Scope.Workspace == c.Id.Workspace && m.Scope.Collection == c.Id.Collection)
        .SelectMany(m => m.Metrics).Where(m => m.Key == metricName).Sum(m => m.Value);
    }
  }

  public class ScopeMetric
  {
    public ScopeMetric()
    {
      Metrics = new Dictionary<string, int>();
    }
    public Scope Scope {get;set;}
    public IDictionary<string, int> Metrics {get;set;}
  }
}
