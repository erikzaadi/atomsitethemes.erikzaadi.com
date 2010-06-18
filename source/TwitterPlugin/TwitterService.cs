/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.TwitterPlugin
{
  using System.Xml.Linq;

  public class TwitterService
  {
    public TwitterTimeline GetTimeline(string twitterName)
    {
      return GetTimeline(twitterName, 5);
    }

    public TwitterTimeline GetTimeline(string twitterName, int limit)
    {
      //TODO: cache data
      string url = string.Format("http://twitter.com/statuses/user_timeline/{0}.xml?count={1}", twitterName, limit);
      return new TwitterTimeline() { Xml = XElement.Load(url) };
    }
  }
}
