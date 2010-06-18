/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;
  using AtomSite.Domain;

  public interface IWidget
  {
    string Name { get; }
    string Description { get; }
    IEnumerable<Asset> Assets { get; }
    bool IsEnabled(BaseModel baseModel, Include include);
    void Render(ViewContext ctx, Include include);
    SupportedScopes SupportedScopes { get; }
    IEnumerable<string> Areas { get; }
    IEnumerable<string> AreaHints { get; }
    ConfigInclude GetConfigInclude(string includePath);
    void UpdatePageModel(PageModel pageModel, Include include);
    bool IsValid(Include include);
  }

  public abstract class BaseWidget : IWidget
  {
    public virtual string Name { get; protected set; }
    public virtual string Description { get; set; }
    public virtual IEnumerable<Asset> Assets { get; set; }
    public SupportedScopes SupportedScopes { get; set; }
    public IEnumerable<string> Areas { get; set; }
    public IEnumerable<string> AreaHints { get; set; }
    public virtual string TailScript { get; set; }
    public Func<string, ConfigInclude> OnGetConfigInclude { get; set; }
    public Func<Include, bool> OnValidate { get; set; }
    public Func<BaseModel, Include, bool> OnIsEnabled { get; set; }
    public BaseWidget(string name)
    {
      this.Name = name;
      this.Assets = new Asset[] { };
      this.Areas = new string[] { };
      this.AreaHints = new string[] { };
      this.TailScript = string.Empty;
      this.OnGetConfigInclude = (s) => null;
      this.OnValidate = (i) => true;
      this.OnIsEnabled = (m, i) => true;
    }

    protected virtual Asset AddAsset(string assetName)
    {
      var asset = new Asset(assetName) { AssetScope = AssetScope.Widget };
      Assets = Assets.Concat(new[] { asset });
      return asset;
    }
    protected virtual Asset AddAsset(string assetName, string assetGroup)
    {
      var asset = new Asset(assetName, assetGroup, AssetScope.Widget);
      Assets = Assets.Concat(new[] { asset });
      return asset;
    }

    public virtual bool IsEnabled(BaseModel baseModel, Include include)
    {
      return OnIsEnabled(baseModel, include);
    }

    public abstract void Render(ViewContext ctx, Include include);

    public virtual void UpdatePageModel(PageModel pageModel, Include include)
    {
      foreach (Asset asset in Assets) pageModel.Assets.Add(asset);
      if (TailScript != null) pageModel.AddToTailScript(TailScript);
    }

    public virtual ConfigInclude GetConfigInclude(string includePath)
    {
      return OnGetConfigInclude(includePath);
    }

    public virtual bool IsValid(Include include)
    {
      return OnValidate(include);
    }
  }

  public class ViewWidget : BaseWidget
  {
    public ViewWidget(string name) : base(name) { }

    public override void Render(ViewContext ctx, Include include)
    {
      HtmlHelper helper = new HtmlHelper(ctx, new ViewDataContainer() { ViewData = ctx.ViewData });
      System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(helper, Name);
    }

    class ViewDataContainer : IViewDataContainer
    {
      public ViewDataDictionary ViewData { get; set; }
    }
  }

  public class CompositeWidget : BaseWidget
  {
    protected string Controller { get; private set; }
    protected string Action { get; private set; }

    public CompositeWidget(string name, string controller, string action)
      : base(name)
    {
      this.Controller = controller;
      this.Action = action;
    }

    public override void Render(ViewContext ctx, Include include)
    {
      //TODO: do we need to clone routedata?
      ctx.RouteData.Values["controller"] = Controller;
      ctx.RouteData.Values["action"] = Action;
      ctx.RouteData.Values["include"] = include;
      ctx.RouteData.Values["widgetName"] = this.Name;
      IHttpHandler handler = new MvcHandler(ctx.RequestContext);
      handler.ProcessRequest(System.Web.HttpContext.Current);
    }
  }
}
