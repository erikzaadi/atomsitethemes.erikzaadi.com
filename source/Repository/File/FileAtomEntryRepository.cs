/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Web.Hosting;
  using AtomSite.Domain;
  using AtomSite.Utils;
  using System.Xml.Linq;
  using System.Web.Caching;
  using System.Web;

  /// <summary>
  /// This class supports storing atom entries to the file system.  
  /// Dated collections can be stored in a hierarchy depending on the StoreDepth.
  /// 
  /// Dated StoreDepth = Day: collectionPath\YYYY\MM\DD\EntryName.xml
  /// Dated StoreDepth = Month: collectionPath\YYYY\MM\EntryName.xml
  /// Dated StoreDepth = Year: collectionPath\YYYY\EntryName.xml
  /// StoreDepth = Flat: collectionPath\EntryName.xml
  /// </summary>
  public class FileAtomEntryRepository : IAtomEntryRepository
  {
    static readonly XmlCache<AtomEntry> entries = new XmlCache<AtomEntry>();

    readonly PathResolver pathResolver;

    public FileAtomEntryRepository(IAppServiceRepository svcRepo) :
      this(HostingEnvironment.ApplicationPhysicalPath, svcRepo) { }

    public FileAtomEntryRepository(string storePath, IAppServiceRepository svcRepo)
    {
      pathResolver = new PathResolver(storePath, svcRepo.GetService());
    }

    static readonly Func<AtomEntry, EntryCriteria, bool> whereAuth =
      (entry, criteria) => (entry.Visible || criteria.Authorized);

    static readonly Func<AtomEntry, EntryCriteria, bool> whereDate =
      (entry, criteria) => entry.Date >= criteria.StartDate && entry.Date <= criteria.EndDate;

    static readonly Func<AtomEntry, EntryCriteria, bool> whereCategory =
      (entry, criteria) => entry.Categories
          .Where(c => c.Term.ToUpperInvariant() == criteria.CategoryTerm.ToUpperInvariant() &&
                        (c.Scheme == criteria.CategoryScheme || criteria.CategoryScheme == null))
          .Count() > 0;

    static readonly Func<AtomEntry, EntryCriteria, bool> wherePerson =
      (entry, criteria) => entry.People
                    .Where(p => (p.Type == "person" || p.Type == criteria.PersonType) &&
                        p.Name.ToUpperInvariant() == criteria.PersonName.ToUpperInvariant())
                    .Count() > 0;

    static readonly Func<AtomEntry, EntryCriteria, bool> whereSearch =
        (entry, criteria) => entry.Xml.Value.ToUpperInvariant()
            .Contains(criteria.SearchTerm.ToUpperInvariant());

    static readonly Func<AtomEntry, EntryCriteria, bool> whereApproved =
        (entry, criteria) => entry.Approved == criteria.Approved.Value;

    static readonly Func<AtomEntry, EntryCriteria, bool> whereDraft =
        (entry, criteria) => entry.Draft == criteria.Draft.Value;

    static readonly Func<AtomEntry, EntryCriteria, bool> wherePending =
        (entry, criteria) => entry.Draft || !entry.Approved;

    static readonly Func<AtomEntry, EntryCriteria, bool> wherePublished =
        (entry, criteria) => entry.Visible;

    //TODO: add spam flag
    static readonly Func<AtomEntry, EntryCriteria, bool> whereSpam =
        (entry, criteria) => false;

    public IEnumerable<AtomEntry> GetEntries(EntryCriteria criteria, int pageIndex, int pageSize, out int totalRecords)
    {
      string[] paths = criteria.EntryId != null ?
                pathResolver.GetEntryFilePaths(criteria.EntryId, criteria.Annotations, criteria.Deep) :
        pathResolver.GetEntryFilePaths(criteria.WorkspaceName, criteria.CollectionName, criteria.Annotations, criteria.Deep);

      Func<AtomEntry, bool> where = (entry) =>
        whereAuth(entry, criteria) &&
      (criteria.StartDate.HasValue && criteria.EndDate.HasValue ? whereDate(entry, criteria) : true) &&
      (criteria.CategoryTerm != null ? whereCategory(entry, criteria) : true) &&
      (criteria.PersonName != null ? wherePerson(entry, criteria) : true) &&
      (criteria.SearchTerm != null ? whereSearch(entry, criteria) : true) &&
      (criteria.Approved != null ? whereApproved(entry, criteria) : true) &&
      (criteria.Draft != null ? whereDraft(entry, criteria) : true) &&
      (criteria.Pending != null ? wherePending(entry, criteria) : true) &&
      (criteria.Published != null ? wherePublished(entry, criteria) : true) &&
      (criteria.Spam != null ? whereSpam(entry, criteria) : true);

      IEnumerable<AtomEntry> entries = GetEntries(paths, where);
      totalRecords = entries.Count();
      if (criteria.SortMethod == SortMethod.EditDesc) entries = entries.OrderByDescending(e => e.Edited);
      else if (criteria.SortMethod == SortMethod.DateAsc) entries = entries.OrderBy(e => e.Date);
      else if (criteria.SortMethod == SortMethod.DateDesc) entries = entries.OrderByDescending(e => e.Date);
      else if (criteria.SortMethod == SortMethod.Default && criteria.Annotations) entries = entries.OrderBy(e => e.Date);
      else if (criteria.SortMethod == SortMethod.Default && criteria.SearchTerm != null) entries = entries.OrderByDescending(e =>
          Regex.Matches(e.Xml.Value, criteria.SearchTerm, RegexOptions.IgnoreCase).Count);

      return entries.Skip(pageIndex * pageSize).Take(pageSize);
    }

    public int GetEntriesCount(EntryCriteria criteria)
    {
      var key = criteria.ToString();
      if (HttpRuntime.Cache.Get(key) != null) return (int)HttpRuntime.Cache.Get(key);
      
      int count;
      GetEntries(criteria, 0, int.MaxValue, out count);

      //cache it
      HttpRuntime.Cache.Insert(key, count, GetCacheDependency(criteria),
          Cache.NoAbsoluteExpiration, TimeSpan.FromDays(30), CacheItemPriority.High, null);
      return count;
    }

    public CacheDependency GetCacheDependency(EntryCriteria criteria)
    {
      //TODO: only get a dependency on specific files in entry criteria
      string[] paths = criteria.EntryId != null ?
                pathResolver.GetEntryFilePaths(criteria.EntryId, criteria.Annotations, criteria.Deep) :
        pathResolver.GetEntryFilePaths(criteria.WorkspaceName, criteria.CollectionName, criteria.Annotations, criteria.Deep);
      return new CacheDependency(paths);
    }

    public int DeleteEntries(EntryCriteria criteria)
    {
      int count;
      foreach (AtomEntry e in GetEntries(criteria, 0, int.MaxValue, out count))
      {
        DeleteEntry(e.Id);
      }
      return count;
    }

    IEnumerable<AtomEntry> GetEntries(string[] paths, Func<AtomEntry, bool> whereTrue)
    {
      List<AtomEntry> e = new List<AtomEntry>();
      foreach (string path in paths)
      {
        try
        {
          //TODO: why the cast needed?
          AtomEntry entry = (AtomEntry)entries.Get(path);
          if (whereTrue == null || whereTrue(entry))
          {
            Compute(entry);
            e.Add(entry);
          }
        }
        catch (Exception ex)
        {
          //TODO: use logger
          Trace.WriteLine(ex);
        }
      }
      e.Sort();
      e.Reverse();
      return e;
    }

    public AtomEntry GetEntry(Id entryId)
    {
      string path = pathResolver.GetEntryPath(entryId);
      if (!File.Exists(path)) throw new ResourceNotFoundException("entry", entryId.ToString());

      //TODO: why the cast needed?
      AtomEntry e = (AtomEntry)entries.Get(path);
      Compute(e); //determine computed data
      return e;
    }

    void Uncompute(AtomEntry e)
    {
      //remove the total number of annotations
      e.Total = null;

      //TODO: remove other computations

      //remove links
      var rels = new [] { "edit", "self", "edit-media", "replies", "alternate", "delete", "unapprove", "approve", "admin-edit", "related", "reply" };
      e.Links = e.Links.Where(l => !rels.Contains(l.Rel));
    }

    void Compute(AtomEntry e)
    {
      if (e == null) return;
      //compute the total number of annotations
      string[] paths = pathResolver.GetEntryFilePaths(e.Id, true, true);
      e.Total = paths.Length;

      //TODO: other computations
    }

    public string GetEntryEtag(Id entryId)
    {
      string path = pathResolver.GetEntryPath(entryId);
      try
      {
        //contentLength = (int)(new FileInfo(path).Length);
        return FileHelper.ComputeMD5Sum(path);
      }
      catch (FileNotFoundException)
      {
        throw new ResourceNotFoundException("entry", entryId.ToString());
      }
    }
    public void DeleteEntry(Id entryId)
    {
      string path = pathResolver.GetEntryPath(entryId);
      if (!System.IO.File.Exists(path))
        throw new ResourceNotFoundException("entry", entryId.ToString());
      System.IO.File.Delete(path);

      //delete annotations
      string dir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path),
        System.IO.Path.GetFileNameWithoutExtension(path));
      
      if (System.IO.Directory.Exists(dir))
      {
        try
        {

          foreach (string file in Directory.GetFiles(dir, "*" + PathResolver.DefaultEntryExt, SearchOption.AllDirectories))
            System.IO.File.Delete(file);
          foreach (string file in Directory.GetFiles(dir, "*" + PathResolver.DefaultMediaLinkEntryExt, SearchOption.AllDirectories))
            System.IO.File.Delete(file);

      //try and clean up old directories
          //don't delete directory because it causes application restart

          //System.IO.Directory.Delete(dir, true);
          //AtomSite.Utils.FileHelper.RemoveEmptyPaths(pathResolver.GetCollectionPath(entryId), true);
        }
        catch (Exception ex)
        {
          System.Diagnostics.Trace.TraceError(ex.Message);
        }
      }

      //clear parent from cache
      if (entryId.IsAnnotation)
      {
        string parent = pathResolver.GetEntryPath(entryId.GetParentId());
        entries.Clear(parent);
      }

    }
    public AtomEntry CreateEntry(AtomEntry entry)
    {
      entry.Xml = CleanNamespaces(entry.Xml);
      Uncompute(entry);

      string path = entry.Media ? pathResolver.GetMediaLinkEntryPath(entry.Id)
          : pathResolver.GetEntryPath(entry.Id);


      //ensure id is unused
      Id id = entry.Id;
      int i = 1;
      while (System.IO.File.Exists(path))
      {
        id = new Id(id.Owner, id.Date, id.Collection, entry.Id.EntryPath + i++);
        path = pathResolver.GetEntryPath(id);
      }
      entry.Id = id;

      //write to disk
      FileHelper.EnsurePathExists(path);
      entries.Put(path, entry);
      return entry;
    }

    public AtomEntry UpdateEntry(AtomEntry entry)
    {
      entry.Xml = CleanNamespaces(entry.Xml);
      Uncompute(entry);

      //file must already exist
      string path = entry.Media ? pathResolver.GetMediaLinkEntryPath(entry.Id)
          : pathResolver.GetEntryPath(entry.Id);
      if (!System.IO.File.Exists(path)) throw new ResourceNotFoundException("entry", entry.Id.ToString());

      //write to disk
      entries.Put(path, entry);
      return entry;
    }

    /// <summary>
    /// Approves an entry... 
    /// TODO: should we remove this whole method and just keep in service?
    /// It is more efficient here, but is it worth the cost of duplicate logic?
    /// </summary>
    /// <param name="entryId"></param>
    public void ApproveEntry(Id entryId)
    {
      string path = pathResolver.GetEntryPath(entryId);
      try
      {
        AtomEntry entry = AtomEntry.Load(path);
        if (!entry.Approved)
        {
          Uncompute(entry);
          entry.Control.Approved = true;
          entry.Edited = DateTimeOffset.UtcNow;
          entries.Put(path, entry);
        }
      }
      catch (FileNotFoundException)
      {
        throw new ResourceNotFoundException("entry", entryId.ToString());
      }
    }

    public int ApproveAll(Id id)
    {
      int total;
      EntryCriteria criteria = id.EntryPath != null ? new EntryCriteria()
      {
        EntryId = id,
        Annotations = true,
        Deep = true,
        Authorized = true
      } : new EntryCriteria()
      {
        WorkspaceName = id.Workspace,
        CollectionName = id.Collection,
        Annotations = true,
        Deep = true,
        Authorized = true
      };
      IEnumerable<AtomEntry> anns = GetEntries(criteria, 0, int.MaxValue, out total);

      int count = 0;
      foreach (AtomEntry entry in anns.Where(e => !e.Approved))
      {
        Uncompute(entry);
        string path = pathResolver.GetEntryPath(entry.Id);
        entry.Control.Approved = true;
        entry.Edited = DateTimeOffset.UtcNow;
        entries.Put(path, entry);
        count++;
      }
      return count;
    }


    protected XElement CleanNamespaces(XElement xml)
    {
      Dictionary<string, XNamespace> nss = new Dictionary<string, XNamespace>();
      nss.Add("atom", Atom.AtomNs);
      nss.Add("svc", Atom.SvcNs);
      nss.Add("app", Atom.AppNs);
      nss.Add("thread", Atom.ThreadNs);

      //update old namespaces
      var old = new string[] {
      //"=\"http://atomsite.net/info/Schema.xhtml\"", 
      "=\"http://blogsvc.net/2008/Service\"" };
      string xmlstr = xml.ToString();
      foreach (string o in old)
      {
        if (xmlstr.Contains(o))
        {
          //Console.WriteLine(" Old namespace replaced {0}", o);
          xmlstr = xmlstr.Replace(o, "=\"" + Atom.SvcNs.NamespaceName + "\"");
        }
      }
      xml = XElement.Parse(xmlstr);

      //set standard prefixes
      foreach (string prefix in nss.Keys)
      {
        xml.SetAttributeValue(XNamespace.Xmlns + prefix, nss[prefix]);
      }

      var namespaces = nss.Values.Select(ns => ns.NamespaceName);

      //remove unneccesary namespace declarations
      foreach (XElement el in xml.Descendants().Where(x => x.Attribute("xmlns") != null
          && x.Name.Namespace != Atom.XhtmlNs))
      {
        if (namespaces.Contains((string)el.Attribute("xmlns")))
        {
          //Console.WriteLine(" Extra namespace removed on {0}: {1}", el.Name.LocalName, (string)el.Attribute("xmlns"));
          el.Attribute("xmlns").Remove();
        }
      }
      foreach (XAttribute at in xml.Descendants().SelectMany(
        x => x.Attributes().Where(a => a.Name.Namespace == XNamespace.Xmlns)))
      {
        if (namespaces.Contains((string)at))
        {
          //Console.WriteLine(" Extra attr namespace removed {0}: {1}", at.Name.LocalName, (string)at);
          at.Remove();
        }
      }
      return xml;
    }
  }
}
