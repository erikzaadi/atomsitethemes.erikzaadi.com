using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomSite.Domain
{

  public enum MeritLevel : int
  {
    Default = 0,
    Low = 1000,
    Medium = 1000 * 10,
    High = 1000 * 100,
    Max = Int32.MaxValue
  }

  public static class Merit
  {
    public static MeritLevel GetNearest(int merit)
    {
      if (merit < 0) return MeritLevel.Default;
      //find nearest Named merit
      var levels = Enum.GetValues(typeof(MeritLevel)).Cast<int>().Reverse();
      int nearest = levels.First();
      foreach (int level in levels)
      {
        int diff = Math.Abs(level - merit);
        if (diff < Math.Abs(nearest - merit)) nearest = level;
        if (diff == 0) break;
      }
      return (MeritLevel)nearest;
    }
    public static string MeritToName(int merit)
    {
      int nearest = (int)GetNearest(merit);
      string name = Enum.GetName(typeof(MeritLevel), nearest);
      int d = merit - nearest;
      if (d > 0) name = name + "+" + d.ToString();
      else if (d < 0) name = name + d.ToString();
      return name;
    }
    public static bool HasNext(int merit)
    {
      var nearest = GetNearest(merit);
      return (nearest != Enum.GetValues(typeof(MeritLevel)).Cast<MeritLevel>().Last());
    }
    public static bool HasPrev(int merit)
    {
      var nearest = GetNearest(merit);
      return (nearest != Enum.GetValues(typeof(MeritLevel)).Cast<MeritLevel>().First());
    }
    public static MeritLevel GetNext(int merit)
    {
      var nearest = GetNearest(merit);
      var levels = Enum.GetValues(typeof(MeritLevel)).Cast<MeritLevel>();
      var next = levels.Where(l => l > nearest).First();
      return next;
    }
    public static MeritLevel GetPrev(int merit)
    {
      var nearest = GetNearest(merit);
      var levels = Enum.GetValues(typeof(MeritLevel)).Cast<MeritLevel>();
      var prev = levels.Where(l => l < nearest).Last();
      return prev;
    }
  }
}
