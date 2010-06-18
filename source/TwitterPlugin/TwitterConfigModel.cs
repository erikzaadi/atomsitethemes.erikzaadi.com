/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.TwitterPlugin
{
  using AtomSite.WebCore;

  public class TwitterConfigModel : ConfigModel
  {
    public string Username { get; set; }
    public int? Count { get; set; }
  }
}
