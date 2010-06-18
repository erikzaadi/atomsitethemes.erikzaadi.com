/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using AtomSite.Domain;
  public class AdminAcceptsModel : AdminModel
  {
    public IEnumerable<AcceptCheck> AcceptChecks { get; set; }
    public string CurrentAccepts { get; set; }

    public static IDictionary<string, IEnumerable<AppAccept>> AcceptTemplates()
    {
      var t = new Dictionary<string, IEnumerable<AppAccept>>();
      t.Add("Entries (default)", Enumerable.Repeat(AppAccept.Entries, 1));
      t.Add("Images", AppAccept.Images);
      t.Add("Audio", AppAccept.Audio);
      t.Add("Video", AppAccept.Video);
      t.Add("Documents & Text", AppAccept.DocsAndText);
      t.Add("Media (images, audio, video)", AppAccept.Media);
      return t;
    }
  }

  public struct AcceptCheck
  {
    public string Accept { get; set; }
    public bool Checked { get; set; }
    public bool Default { get; set; }
  }
}
