/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AtomSite.Domain;

  public class BlogAppService : AppService
  {
    public static readonly string DefaultBlogPageExt = ".xhtml";

    public BlogAppService(AppService svc) : base(svc.Xml) { }

    /// <summary>
    /// Gets or sets the page ext for blog web pages. The default value when null is ".xhtml"
    /// </summary>
    /// <value>Extension starting with period.</value>
    public string BlogPageExt
    {
      get { return GetProperty<string>(Atom.SvcNs + "blogPageExt") ?? DefaultBlogPageExt; }
      set { SetProperty(Atom.SvcNs + "blogPageExt", value == DefaultBlogPageExt ? null : value); }
    }
  }
}
