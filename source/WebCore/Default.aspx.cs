using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace AtomSite.WebCore
{
  public partial class _Default : System.Web.UI.Page
  {
    public void Page_Load(object sender, System.EventArgs e)
    {
      HttpContext.Current.RewritePath(Request.ApplicationPath, false);
      IHttpHandler httpHandler = new MvcHttpHandler();
      httpHandler.ProcessRequest(HttpContext.Current);
    }
  }
}
