/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.OpenIdPlugin
{
  using System.Web;
  using System.Web.Mvc;
  using AtomSite.Domain;
  using AtomSite.WebCore;
  using StructureMap;
  using System.IO;
  using System.Web.Hosting;

  public class OpenIdModalWidget : ViewWidget
  {
    public OpenIdModalWidget()
      : base("OpenIdModalWidget")
    {
      Description = "This widget adds a modal popup dialog where users can login using OpenID.";
      AddAsset("OpenId.css");
      AddAsset("jquery.openid-1.1.js");
      AddAsset("jquery.tools-1.1.2.js");
      SupportedScopes = SupportedScopes.All;
      AreaHints = new[] { "commentator" };
    }

    public override void UpdatePageModel(PageModel pageModel, Include include)
    {
      //TODO: don't hardcode path to default theme
      string tailscript = File.ReadAllText(
        HostingEnvironment.MapPath("~/js/default/OpenIdModalWidgetTail.js"));
      pageModel.AddToTailScript(tailscript);

      base.UpdatePageModel(pageModel, include);
    }
  }
}
