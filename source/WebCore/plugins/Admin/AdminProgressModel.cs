/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;

  public class AdminProgressModel : AdminModel
  {
    public AdminProgressModel()
    {
      Messages = new List<string>();
    }

    public string ProcessName { get; set; }
    public IList<string> Messages { get; set; }
    public string ProgressUrl { get; set; }
  }

  public class Progress
  {
    public Progress()
    {
      Messages = new List<string>();
    }
    public List<string> Messages { get; set; }
    public byte PercentComplete { get; set; }
  }
}
