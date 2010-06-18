/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml.Linq;
  using AtomSite.Domain;

  internal class PathResolver
  {
    internal static readonly XNamespace FileNs = "http://atomsite.net/info/FileRepository";
    internal static readonly string DefaultServiceDocFileName = "Service.config";
    internal static readonly string DefaultCategoriesFileName = "Categories.config";
    internal static readonly StoreDepth DefaultStoreDepth = StoreDepth.Flat;
    internal static readonly string DefaultEntryExt = ".xml";
    internal static readonly string DefaultMediaLinkEntryExt = ".xml";

    public readonly string StorePath;
    public readonly AppService AppService;
    public readonly string ServiceDocPath;

    public PathResolver(string storePath) : this(storePath, null) { }

    public PathResolver(string storePath, AppService appService)
    {
      StorePath = storePath;
      AppService = appService;
      ServiceDocPath = GetServiceDocPath();
    }

    public string GetCategoriesPath(Id collectionId, AppCategories externalCats)
    {
      string filename = externalCats.GetProperty<string>(FileNs + "name");
      if (string.IsNullOrEmpty(filename)) filename = DefaultCategoriesFileName;
      string path = GetCollectionPath(collectionId);
      path = Path.Combine(path, filename);
      return path;
    }

    public string GetCollectionPath(Id id)
    {
      return GetCollectionPath(id.Workspace, id.Collection);
    }

    public string GetCollectionPath(string workspaceName, string collectionName)
    {
      string path = AppService.GetCollection(workspaceName, collectionName)
        .GetProperty<string>(FileNs + "path");
      if (!string.IsNullOrEmpty(path)) path = Path.Combine(StorePath, path);
      else
      {
        path = Path.Combine(StorePath, "App_Data");
        path = Path.Combine(path, workspaceName);
        path = Path.Combine(path, collectionName);
      }
      return path;
    }

    public string GetEntryPath(Id id)
    {
      //TODO: get entry ext from configuration
      return GetResourcePath(id) + DefaultEntryExt;
    }

    public string GetMediaLinkEntryPath(Id id)
    {
      //TODO: get entry ext from configuration
      return GetResourcePath(id) + DefaultMediaLinkEntryExt;
    }

    public string GetMediaPath(Id id, string contentType)
    {
      //TODO: use extension found in config
      string fileExt = "." + contentType.Substring(contentType.LastIndexOf('/') + 1);
      return GetResourcePath(id) + fileExt;
    }

    string GetServiceDocPath()
    {
      //TODO: get filename from config
      return Path.Combine(StorePath, DefaultServiceDocFileName);
    }

    StoreDepth GetStoreDepth(Id id)
    {
      AppCollection coll = AppService.GetCollection(id);
      StoreDepth depth = DefaultStoreDepth; //TODO: get from config
      string storeDepth = coll.GetProperty<string>(FileNs + "depth");
      if (!string.IsNullOrEmpty(storeDepth) &&
                Enum.GetNames(typeof(StoreDepth)).Contains(storeDepth))
      {
        depth = (StoreDepth)Enum.Parse(typeof(StoreDepth), storeDepth, true);
      }

      //only dated collections can use a storedepth other than flat
      if (!coll.Dated && depth != StoreDepth.Flat)
        throw new StoreDepthException(depth.ToString(), coll.Id.Collection);

      return depth;
    }

    public string[] GetEntryFilePaths(Id entryId, bool annotations, bool deep)
    {
      return GetEntryFilePaths(GetPaths(entryId, annotations, deep));
    }

    public string[] GetEntryFilePaths(string workspaceName, string collectionName, bool annotations, bool deep)
    {
      return GetEntryFilePaths(GetPaths(workspaceName, collectionName, annotations, deep));
    }

    string[] GetPaths(Id entryId, bool annotations, bool deep)
    {
      string path = Path.Combine(Path.GetDirectoryName(GetResourcePath(entryId)),
          Path.GetFileNameWithoutExtension(GetResourcePath(entryId)) + Path.DirectorySeparatorChar);
      //if (annotations) return GetDirectories(path, deep ? int.MaxValue : 1);
      return GetDirectories(path, deep ? int.MaxValue : 1);
    }

    string[] GetPaths(string workspaceName, string collectionName, bool annotations, bool deep)
    {
      List<string> paths = new List<string>();
      IEnumerable<AppWorkspace> workspaces = workspaceName == null ?
        AppService.Workspaces : Enumerable.Repeat(AppService.GetWorkspace(workspaceName), 1);
      foreach (AppWorkspace workspace in workspaces)
      {
        IEnumerable<AppCollection> collections = collectionName == null ?
          workspace.Collections : Enumerable.Repeat(workspace.GetCollection(collectionName), 1);
        foreach (AppCollection collection in collections)
        {
          List<string> collPaths = GetDirectories(GetCollectionPath(collection.Id),
            (int)GetStoreDepth(collection.Id)).ToList();

          if (annotations) paths.AddRange(collPaths.SelectMany(p => Directory.GetDirectories(p)
                        .SelectMany(p1 => GetDirectories(p1, deep ? int.MaxValue : 1))));
          else paths.AddRange(collPaths);
        }
      }

      return paths.ToArray();
    }

    string[] GetDirectories(string path, int depth)
    {
      List<string> dirs = new List<string>();
      if (!Directory.Exists(path)) return dirs.ToArray();
      dirs.Add(path);
      if (depth > 0)
      {
        foreach (string dir in Directory.GetDirectories(path))
        {
          dirs.AddRange(GetDirectories(dir, depth - 1));
        }
      }
      return dirs.ToArray();
    }

    string[] GetEntryFilePaths(string[] paths)
    {
      return paths.Where(p => Directory.Exists(p))
          .SelectMany(p => Directory.GetFiles(p, "*" + PathResolver.DefaultEntryExt))
          .ToArray();
    }

    string GetResourcePath(Id id)
    {
      string path = GetCollectionPath(id);
      switch (GetStoreDepth(id))
      {
        //TODO: unit test
        case StoreDepth.Year:
          path = Path.Combine(path, id.Date.Substring(0, 4));
          break;
        case StoreDepth.Month:
          path = Path.Combine(path, id.Date.Substring(0, 4));
          path = Path.Combine(path, id.Date.Substring(5, 2));
          break;
        case StoreDepth.Day:
          path = Path.Combine(path, id.Date.Substring(0, 4));
          path = Path.Combine(path, id.Date.Substring(5, 2));
          path = Path.Combine(path, id.Date.Substring(8, 2));
          break;
      }

      foreach (string name in id.EntryPath.Split(','))
      {
        path = Path.Combine(path, name);
      }
      return path;
    }
  }
}
