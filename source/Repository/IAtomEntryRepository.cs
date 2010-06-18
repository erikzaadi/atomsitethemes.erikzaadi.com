/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository
{
  using System.Collections.Generic;
  using AtomSite.Domain;
  using System.Web.Caching;

  public interface IAtomEntryRepository
  {
    IEnumerable<AtomEntry> GetEntries(EntryCriteria criteria, int pageIndex, int pageSize, out int totalEntries);
    int GetEntriesCount(EntryCriteria criteria);
    CacheDependency GetCacheDependency(EntryCriteria criteria);
    int DeleteEntries(EntryCriteria criteria);
    AtomEntry GetEntry(Id entryId);
    string GetEntryEtag(Id entryId);
    void DeleteEntry(Id entryId);
    AtomEntry CreateEntry(AtomEntry entry);
    AtomEntry UpdateEntry(AtomEntry entry);
    int ApproveAll(Id id);
  }
}
