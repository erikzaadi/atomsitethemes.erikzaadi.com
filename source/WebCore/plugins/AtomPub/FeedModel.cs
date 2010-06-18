/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using AtomSite.Domain;

  public class FeedModel<T> : PageModel where T : AtomEntry
  {

  }

  public class FeedModel : PageModel
  {
    /// <summary>
    /// Allow simple override of Feed Title
    /// </summary>
    public string Title
    {
      get { return Feed != null ? Feed.Title.Text : string.Empty; }
      set { Feed.Title.Text = value; }
    }

    public AtomFeed Feed { get; set; }

    public int EntryCount
    {
      get { return Feed != null && Feed.Entries != null ? Feed.Entries.Count() : 0; }
    }

    public IEnumerable<int> GetYears()
    {
      if (Feed.Entries.Count() == 0) return Enumerable.Empty<int>();
      return Feed.Entries.Select(e => e.Date.Year).Distinct()
          .OrderByDescending(y => y);
    }

    public IEnumerable<int> GetMonths(int year)
    {
      if (Feed.Entries.Count() == 0) return Enumerable.Empty<int>();
      return Feed.Entries.Where(e => e.Date.Year == year).Select(e => e.Date.Month).Distinct()
          .OrderByDescending(m => m);
    }

    public IEnumerable<AtomEntry> GetEntries(int year, int month)
    {
      return Feed.Entries.Where(e => e.Date.Year == year && e.Date.Month == month)
          .OrderByDescending(e => e.Date.Day);
    }

    public double GetAveragePerDay()
    {
      if (!Feed.TotalResults.HasValue || Feed.TotalResults.Value == 0) return 0;
      double days = Feed.Entries.First().Date.Subtract(Feed.Entries.Last().Date).TotalDays;
      if (days < 1) days = 1;
      return (double)Feed.TotalResults / days;
    }

    public IEnumerable<AtomCategory> GetCategories()
    {
      if (Feed.Entries.Count() == 0 ||
          Feed.Entries.SelectMany(e => e.Categories).Count() == 0)
        return Enumerable.Empty<AtomCategory>();
      return Feed.Entries.SelectMany(e => e.Categories).Distinct()
          .OrderBy(c => c.ToString());
    }

    public IEnumerable<AtomEntry> GetEntries(AtomCategory cat)
    {
      return Feed.Entries.Where(e => e.Categories.Contains(cat))
          .OrderByDescending(e => e.Date);
    }

    public IEnumerable<AtomPerson> GetAuthors()
    {
      if (Feed.Entries.Count() == 0 ||
          Feed.Entries.SelectMany(e => e.Authors).Count() == 0)
        return Enumerable.Empty<AtomPerson>();
      return Feed.Entries.SelectMany(e => e.Authors).Distinct();
    }

    public IEnumerable<AtomPerson> GetContributors()
    {
      //does selectmany return empty enum?
      if (Feed.Entries.Count() == 0) return Enumerable.Empty<AtomPerson>();
      return Feed.Entries.SelectMany(e => e.Contributors).Distinct();
    }

    public IEnumerable<AtomEntry> GetEntries(AtomPerson p)
    {
      if (p.Type == "author")
      {
        return Feed.Entries.Where(e => e.Authors.Contains(p))
            .OrderByDescending(e => e.Date);
      }
      return Feed.Entries.Where(e => e.Contributors.Contains(p))
          .OrderByDescending(e => e.Date);
    }

    public float GetCategorySize(AtomCategory cat, float minSize, float maxSize)
    {
      int min = GetCategories().Select(c => GetEntries(c).Count()).Min();
      int max = GetCategories().Select(c => GetEntries(c).Count()).Max();
      int spread = max - min == 0 ? 1 : max - min;

      float step = (maxSize - minSize) / (float)spread;

      return minSize + ((float)(GetEntries(cat).Count() - min) * step);
    }
  }
}
