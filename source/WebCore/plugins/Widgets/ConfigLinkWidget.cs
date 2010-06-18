/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using AtomSite.Domain;

  public class ConfigLinkWidget : BaseWidget
  {
    public ConfigLinkWidget() : base("ConfigLinkWidget") { }
    public ConfigLinkWidget(string name) : base(name) { }
    public ConfigLinkWidget(string name, Asset[] assets)
      : base(name)
    {
      this.Assets = assets;
    }

    public override void Render(ViewContext ctx, Include include)
    {
      TagBuilder b = new TagBuilder("a");
      b.Attributes.Add("href",  GetLinkHref(ctx, include));
      b.Attributes.Add("rel", "config");
      b.SetInnerText("configure");
      ctx.HttpContext.Response.Write(b.ToString() + " | ");
    }

    protected virtual string GetLinkHref(ViewContext ctx, Include include)
    {
      var i = new ConfigLinkInclude(include);
      return new UrlHelper(ctx.RequestContext).Action(i.Action, i.Controller, new { includePath = i.IncludePath });
    }
  }

  public class FeedConfigLinkWidget : ConfigLinkWidget
  {
    public FeedConfigLinkWidget() : base("FeedConfigLinkWidget") { }
    public FeedConfigLinkWidget(string name, Asset[] assets)
      : base(name, assets) { }

    protected override string GetLinkHref(ViewContext ctx, Include include)
    {
      var i = new FeedConfigLinkInclude(include);
      return new UrlHelper(ctx.RequestContext).Action(i.Action, i.Controller, new { includePath = i.IncludePath, hasTitle = i.HasTitle, hasCount = i.HasCount });
    }

  }

  public class ConfigLinkInclude : ConfigInclude
  {
    public ConfigLinkInclude(string widgetName)
      : base()
    {
      Name = widgetName;
    }

    public ConfigLinkInclude()
      : base()
    {
      Name = "ConfigLinkWidget";
    }

    public ConfigLinkInclude(string includePath, string controller, string action)
      : this()
    {
      IncludePath = includePath;
      Controller = controller;
      Action = action;
    }

    public ConfigLinkInclude(string widgetName, string includePath, string controller, string action)
      : this(widgetName)
    {
      IncludePath = includePath;
      Controller = controller;
      Action = action;
    }

    public ConfigLinkInclude(Include include) : base(include.Xml) { }

    /// <summary>
    /// Gets or sets the name, which is typically the name of the widget to display.
    /// </summary>
    public string Action
    {
      get { return GetProperty<string>("action"); }
      set { SetProperty<string>("action", value); }
    }

    /// <summary>
    /// Gets or sets the name, which is typically the name of the widget to display.
    /// </summary>
    public string Controller
    {
      get { return GetProperty<string>("controller"); }
      set { SetProperty<string>("controller", value); }
    }

  }

  public class FeedConfigLinkInclude : ConfigLinkInclude
  {
    public FeedConfigLinkInclude(Include include) : base(include) { }

    public FeedConfigLinkInclude() : base("FeedConfigLinkWidget") { }

    public bool HasCount
    {
      get { return GetBoolean(Atom.SvcNs + "hasCount") ?? false; }
      set { SetBoolean(Atom.SvcNs + "hasCount", value == false ? (bool?)null : (bool?)value); }
    }
    public bool HasTitle
    {
      get { return GetBoolean(Atom.SvcNs + "hasCount") ?? false; }
      set { SetBoolean(Atom.SvcNs + "hasCount", value == false ? (bool?)null : (bool?)value); }
    }
  }
}