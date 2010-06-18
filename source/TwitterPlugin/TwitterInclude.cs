/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.TwitterPlugin
{
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class TwitterInclude : Include
  {
    public TwitterInclude() : base(new XElement(Include.IncludeXName)) { }

    public TwitterInclude(Include include)
      : base(include.Xml) { }

    public TwitterInclude(string username, int count)
      : base(new XElement(Atom.SvcNs + "twitter"))
    {
      this.Username = username;
      this.Count = count;
    }

    /// <summary>
    /// Gets or sets the twitter username of the public twitter timeline to show.
    /// </summary>
    public string Username
    {
      get { return GetProperty<string>("username"); }
      set { SetProperty<string>("username", value); }
    }

    /// <summary>
    /// Gets or sets the count of tweets to show.  The default value is 6.
    /// </summary>
    public int Count
    {
      get { return GetInt32PropertyWithDefault("count", 6); }
      set { SetInt32Property("count", value == 6 ? (int?)null : value); }
    }
  }
}
