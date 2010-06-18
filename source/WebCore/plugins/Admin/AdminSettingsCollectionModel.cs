/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Web.Mvc;
  using AtomSite.Domain;

  [Bind(Include="Owner,Name,CollectionId,Title,Subtitle,Dated,AnnotationsOn,ExtendedEntriesOn,SyndicationOn,Visible")]
  public class AdminSettingsCollectionModel : AdminSettingsModel
  {
    public Id CollectionId { get; set; }
    public string Owner { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public bool? Dated { get; set; }
    public bool? AnnotationsOn { get; set; }
    public bool? ExtendedEntriesOn { get; set; }
    public bool? SyndicationOn { get; set; }
    public string DefaultView { get; set; }
    public string DefaultEntryView { get; set; }
    public bool? Visible { get; set; }
    public Uri Icon { get; set; }
    public Uri Logo { get; set; }
    public int? PageSize { get; set; }
    public bool IsNew { get { return CollectionId == null; } }
  }
}
