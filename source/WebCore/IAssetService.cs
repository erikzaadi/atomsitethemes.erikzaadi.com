/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Web.Mvc;
  using AtomSite.Domain;

  public interface IAssetService
  {
    ICollection<Asset> Assets { get; }
    void UpdatePageModel(PageModel pageModel);
    IEnumerable<string> GetAssetSources(AssetType type, PageModel pageModel, UrlHelper helper);
    GroupCombinedAsset GetGroupCombined(AssetType type, string group, UrlHelper helper);
    string GetAssetUrl(AssetType type, string themeName, string assetName, UrlHelper helper);
  }

  public enum AssetGroupMode : int
  {
    Disabled = -1,
    Combine = 0,
    CombineAndMinify
  }

  public class GroupCombinedAsset
  {
    public string Md5 { get; set; }
    public string Group { get; set; }
    public string Content { get; set; }
    public DateTime LastModified { get; set; }
  }
}
