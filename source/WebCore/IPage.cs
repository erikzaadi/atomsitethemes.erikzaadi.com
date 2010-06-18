/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Collections.Generic;
  using System.Linq;
  using AtomSite.Domain;

  public interface IPage
  {
    string Name { get; }
    //string Description { get; }
    string Parent { get; }
    IEnumerable<Asset> Assets { get; }
    SupportedScopes SupportedScopes { get; }
    IEnumerable<string> Areas { get; set; }
    void UpdatePageModel(PageModel pageModel);
  }

  public class Page : IPage
  {
    public Page(string name) : this(name, null) { }
    public Page(string name, string parent) : this(name, parent, new Asset[]{}) { }
    public Page(string name, string parent, IEnumerable<Asset> assets)
    {
      Name = name;
      Parent = parent;
      Assets = assets;
      Areas = new string[] { };
    }

    public string Name { get; protected set; }
    //public string Description { get { return string.Format("{0}"; } }
    public string Parent { get; set; }
    //public bool IsMasterPage { get { return Parent == null; } }
    public IEnumerable<Asset> Assets { get; set; }
    public SupportedScopes SupportedScopes { get; set; }
    public IEnumerable<string> Areas { get; set; }
    public void UpdatePageModel(PageModel pageModel)
    {
      foreach (Asset asset in Assets) pageModel.Assets.Add(asset);
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
  }

}
