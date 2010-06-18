namespace AtomSite.Repository.File
{
  using System;
  using System.Collections.Generic;
  using System.Web;
  using System.Web.Caching;
  using System.Xml.Linq;
  using AtomSite.Domain;

  //TODO: use code below 
  /*
  public class SynchronizedCache
  {
    private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
    private Dictionary<int, string> innerCache = new Dictionary<int, string>();

    public string Read(int key)
    {
      cacheLock.EnterReadLock();
      try
      {
        return innerCache[key];
      }
      finally
      {
        cacheLock.ExitReadLock();
      }
    }

    public void Add(int key, string value)
    {
      cacheLock.EnterWriteLock();
      try
      {
        innerCache.Add(key, value);
      }
      finally
      {
        cacheLock.ExitWriteLock();
      }
    }

    public bool AddWithTimeout(int key, string value, int timeout)
    {
      if (cacheLock.TryEnterWriteLock(timeout))
      {
        try
        {
          innerCache.Add(key, value);
        }
        finally
        {
          cacheLock.ExitWriteLock();
        }
        return true;
      }
      else
      {
        return false;
      }
    }

    public AddOrUpdateStatus AddOrUpdate(int key, string value)
    {
      cacheLock.EnterUpgradeableReadLock();
      try
      {
        string result = null;
        if (innerCache.TryGetValue(key, out result))
        {
          if (result == value)
          {
            return AddOrUpdateStatus.Unchanged;
          }
          else
          {
            cacheLock.EnterWriteLock();
            try
            {
              innerCache[key] = value;
            }
            finally
            {
              cacheLock.ExitWriteLock();
            }
            return AddOrUpdateStatus.Updated;
          }
        }
        else
        {
          cacheLock.EnterWriteLock();
          try
          {
            innerCache.Add(key, value);
          }
          finally
          {
            cacheLock.ExitWriteLock();
          }
          return AddOrUpdateStatus.Added;
        }
      }
      finally
      {
        cacheLock.ExitUpgradeableReadLock();
      }
    }

    public void Delete(int key)
    {
      cacheLock.EnterWriteLock();
      try
      {
        innerCache.Remove(key);
      }
      finally
      {
        cacheLock.ExitWriteLock();
      }
    }

    public enum AddOrUpdateStatus
    {
      Added,
      Updated,
      Unchanged
    };
  }*/
  /// <summary>
  /// Class for getting and putting xml based objects to the
  /// file system and cache at the same time.  It creates a
  /// dependency on the file so that changes to the file will
  /// invalidate the cache.
  /// </summary>
  public class XmlCache<T> where T : XmlBase
  {
    private Dictionary<string, T> objs;

    public XmlCache()
    {
      objs = new Dictionary<string, T>();
    }

    public void Clear(string filename)
    {
      lock (objs)
      {
        if (objs.ContainsKey(filename))
          objs[filename] = null;
      }
    }

    public void Put(string filename, T item)
    {
      lock (objs)
      {
        // don't lose formatting, thx Chomper
        // http://blogsvc.codeplex.com/WorkItem/View.aspx?WorkItemId=16279                
        item.Xml.Save(filename, filename.EndsWith(".xml") ? SaveOptions.DisableFormatting : SaveOptions.None);
        CacheItem(filename, item);
      }
    }

    public T Get(string filename)
    {
      lock (objs)  // TODO: does this make it thread safe?  Is this a perf bottleneck?
      {
        if (objs.ContainsKey(filename) && objs[filename] != null)
        {
          // already loaded, just return
          return objs[filename];
        }
        else
        {
          // need to load and cache
          //if (!System.IO.File.Exists(filename)) return default(T);
          T obj = Activator.CreateInstance<T>();
          obj.Xml = XElement.Load(filename, filename.EndsWith(".xml") ? LoadOptions.PreserveWhitespace : LoadOptions.None);
          // cache in dictionary
          CacheItem(filename, obj);
          return obj;
        }
      }
    }

    public void CacheItem(string filename, T item)
    {
      if (!objs.ContainsKey(filename)) objs.Add(filename, item);
      // items aren't actually put in http cache, but an object handle is
      HttpRuntime.Cache.Insert(filename, new object(), new CacheDependency(filename),
          Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30) /*TODO: make configurable*/, CacheItemPriority.Normal,
        // remove from cache
          new CacheItemRemovedCallback((key, value, reason) =>
          {
            lock (objs) //TODO: is this lock needed?
            {
              objs[key] = null; //just let it reload on next get
            }
          })  // objs.Remove(key))
      );
    }
  }
}
