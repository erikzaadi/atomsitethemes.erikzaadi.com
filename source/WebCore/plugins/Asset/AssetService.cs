/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Web.Caching;
  using System.Web.Mvc;
  using System.Web.Routing;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore.Properties;
  using StructureMap;
  using System.Web.Hosting;

  public class AssetService : IAssetService
  {
    protected ILogService LogService { get; private set; }
    protected IAppServiceRepository AppServiceRepository { get; private set; }
    protected IThemeService ThemeService { get; private set; }
    public ICollection<Asset> Assets { get; protected set; }
    public AssetGroupMode AssetGroupMode { get { return (AssetGroupMode)(Enum.Parse(typeof(AssetGroupMode), Settings.Default.AssetGroupMode)); } }

    public AssetService(IAppServiceRepository svcRepo, IThemeService themeSvc, ILogService logSvc)
    {
      AppServiceRepository = svcRepo;
      ThemeService = themeSvc;
      LogService = logSvc;
      Assets = new Collection<Asset>();
    }

    public void UpdatePageModel(PageModel pageModel)
    {
      //get any global assets from asset manager either in no group or in a group used on the current page
      foreach (Asset asset in Assets.Where(a => a.Group == Asset.NoGroup)) pageModel.Assets.Add(asset);
      var groups = pageModel.Assets.Select(a => a.Group).Distinct();
      foreach (Asset asset in Assets.Where(a => groups.Contains(a.Group))) pageModel.Assets.Add(asset);
    }

    string GetCdnUrl(string assetName)
    {
      //AppService appSvc = AppServiceRepository.GetService();
      //CdnAssetLocations locs = appSvc.Cdn
      CdnAssetLocations locs = CdnAssetLocations.Default;
      var loc = locs.Assets.Where(a => a.Name == assetName).FirstOrDefault();
      if (loc != null) return loc.Url;
      return null;
    }

    /// <summary>
    /// Gets assets that are either available over a CDN or are not in any group
    /// </summary>
    /// <param name="assets"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    ICollection<Asset> GetNonGroupedAssets(ICollection<Asset> assets, AssetType type)
    {
      var nonGrouped = assets.Where(a => a.AssetType == type && a.Group == Asset.NoGroup);

      if (Settings.Default.UseCDN)
      {
        return nonGrouped.Concat(assets.Where(a => a.AssetType == type && a.Group != Asset.NoGroup &&
          GetCdnUrl(a.Name) != null)).Distinct().ToList();
      }
      return nonGrouped.Distinct().ToList();
    }

    /// <summary>
    /// Gets assets that are in the given groups and not delivered by CDN (when enabled)
    /// </summary>
    /// <param name="assets"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    ICollection<Asset> GetGroupedAssets(ICollection<Asset> assets, string[] groups, AssetType type)
    {
      var grouped = assets.Where(a => a.AssetType == type && groups.Contains(a.Group));
      
      //filter out CDN delivered assets when enabled
      if (Settings.Default.UseCDN)
      {
        return grouped.Where(a => GetCdnUrl(a.Name) == null).Distinct().ToList();
      }
      return grouped.Distinct().ToList();
    }


    public IEnumerable<string> GetAssetSources(AssetType type, PageModel pageModel, UrlHelper helper)
    {
      //load items not in any group or delivered by CDN
      foreach (Asset asset in GetNonGroupedAssets(pageModel.Assets, type))
      {
        if (Settings.Default.UseCDN && GetCdnUrl(asset.Name) != null)
          yield return GetCdnUrl(asset.Name);
        else yield return helper.AssetPath(asset.AssetType, asset.Name);
      }

      var groups = pageModel.Assets.Select(a => a.Group).Distinct().ToArray();
      var grouped = GetGroupedAssets(pageModel.Assets, groups, type);
      if (AssetGroupMode == AssetGroupMode.Disabled)
      {
        foreach (string s in grouped.Select(a => helper.AssetPath(a.AssetType, a.Name)))
          yield return s;
      }
      else //load local via combination
      {
        if (grouped.Count > 0)
        {
          foreach (string group in groups)
          {
            //the following method will load and cache the combined script for later
            var version = GetGroupCombined(type, group, helper).Md5;
            //add versioning querystring for future updates overridding expires
            yield return helper.RouteUrl("AssetGroup" + type.ToString(), new { group = group, v = version });
          }
        }
      }
    }

    public GroupCombinedAsset GetGroupCombined(AssetType type, string group, UrlHelper helper)
    {
      string key = string.Format("{0} - {1}", type, group);

      //first try to get from cache
      GroupCombinedAsset combined = helper.RequestContext.HttpContext.Cache[key] as GroupCombinedAsset;
      if (combined != null) return combined;

      if (type != AssetType.Js && type != AssetType.Css)
        throw new ArgumentException("Can't group for the given type.", "type");

      AggregateCacheDependency cacheDep = new AggregateCacheDependency();

      IContainer container = (IContainer)helper.RequestContext.HttpContext.Application["Container"];
      var assets = GetGroupedAssets(
        //global
        Assets
        //pages
        .Concat(container.GetAllInstances<IPage>().SelectMany(p => p.Assets))
        //widgets
        .Concat(container.GetAllInstances<IWidget>().SelectMany(w => w.Assets)).ToList(), new[] { group }, type);

      StringBuilder sb = new StringBuilder();
      foreach (Asset a in assets)
      {
        //add helpful debugging info when not going to minify
        string path = helper.AssetPath(a.AssetType, a.Name);
        if (AssetGroupMode != AssetGroupMode.CombineAndMinify)
        {
          sb.Append(Environment.NewLine);
          sb.Append(Environment.NewLine);
          sb.AppendFormat("/* asset: {0} */", path);
          sb.Append(Environment.NewLine);
        }
        path = helper.RequestContext.HttpContext.Server.MapPath(path);
        try
        {
          sb.Append(File.ReadAllText(path));
          cacheDep.Add(new CacheDependency(path));
          sb.Append(Environment.NewLine);
        }
        catch (FileNotFoundException ex)
        {
          LogService.Warn(ex.Message);
        }
      }

      string content = string.Empty;
      if (sb.Length > 0)
      {        
          if (type == AssetType.Css)
              content = AssetGroupMode == AssetGroupMode.CombineAndMinify ?
                Minify(type, sb.ToString()) : sb.ToString();
          else
              content = AssetGroupMode == AssetGroupMode.CombineAndMinify ?
                Minify(type, sb.ToString()) : sb.ToString();
      }

      combined = new GroupCombinedAsset()
      {
        Content = content,
        Group = group,
        Md5 = AtomSite.Utils.SecurityHelper.HashIt(content, "MD5"),
        LastModified = cacheDep.UtcLastModified.ToLocalTime()
      };

      helper.RequestContext.HttpContext.Cache.Add(key, combined, cacheDep, Cache.NoAbsoluteExpiration,
        Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
      return combined;
    }

    protected virtual string Minify(AssetType type, string content)
    {
      if (type == AssetType.Css)
        return Yahoo.Yui.Compressor.CssCompressor.Compress(content);
      else
        return Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(content);
    }

    public string GetAssetUrl(AssetType assetType, string theme, string assetName, UrlHelper helper)
    {
      if (Settings.Default.UseCDN)
      {
        string url = GetCdnUrl(assetName);
        if (url != null) return url;
      }

      string type = assetType.ToString().ToLower();
      if (theme != null)
      {
        string path = string.Format("~/{0}/{1}/{2}", type, theme, assetName);
        if (File.Exists(HostingEnvironment.MapPath(path))) return helper.Content(path);
        path = string.Format("~/{0}/default/{1}", type, assetName);
        if (File.Exists(HostingEnvironment.MapPath(path))) return helper.Content(path);
      }
      return helper.Content(string.Format("~/{0}/{1}", type, assetName));  
    }

    public static IAssetService GetCurrent(RequestContext ctx)
    {
      return ((IContainer)ctx.HttpContext.Application["Container"]).GetInstance<IAssetService>();
    }
  }
}
