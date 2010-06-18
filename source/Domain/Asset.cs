using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AtomSite.Domain
{
  public enum AssetType
  {
    Css,
    Js,
    Img,
    Media
  }

  public enum AssetScope
  {
    Global,
    Page,
    Widget
  }

  public class Asset
  {
    public static readonly string DefaultGroup = "site";
    public static readonly string NoGroup = string.Empty;

    public Asset(string assetName)
      : this(assetName, Asset.DefaultGroup, AssetScope.Global) { }

    public Asset(string assetName, string assetGroup)
      : this(assetName, assetGroup, AssetScope.Global) { }

    public Asset(string assetName, string assetGroup, AssetScope assetScope)
    {
      Group = assetGroup;
      Name = assetName;
      AssetScope = assetScope;

      string ext = Path.GetExtension(assetName).ToLower();
      AssetType = ext == ".css" ? AssetType.Css :
        ext == ".js" ? AssetType.Js :
        ext == ".png" || ext == ".jpg" || ext == ".gif" ? AssetType.Img :
        AssetType.Media;
    }

    public AssetScope AssetScope { get; set; }
    public AssetType AssetType { get; set; }
    public string Group { get; set; }
    public string Name { get; set; }
    public string Md5 { get; set; }
    public DateTimeOffset LastModified { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      var other = (Asset)obj;
      return Name == other.Name && Group == other.Group;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
      return Name.GetHashCode() ^ Group.GetHashCode();
    }
  }
}
