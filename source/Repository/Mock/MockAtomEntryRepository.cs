using System;
using System.Collections.Generic;
using System.Linq;
using AtomSite.Domain;
using AtomEntry=AtomSite.Domain.AtomEntry;

namespace AtomSite.Repository.Mock
{
    /// <summary>
    /// Mock Implementation of IAtomEntryRepository
    ///  - data isn't put anywhere, just stored in memory
    /// </summary>
    public class MockAtomEntryRepository: IAtomEntryRepository
    {
        #region data

        /// <summary>
        /// the entry "data store"
        /// contains all entries that have been saved into the mock repository
        /// </summary>
        private readonly List<Domain.AtomEntry> entriesList = new List<Domain.AtomEntry>();

        /// <summary>
        /// Map of which entries are annotations of other entries
        /// </summary>
        private readonly Dictionary<string, List<Domain.AtomEntry>> annotationMap = new Dictionary<string, List<Domain.AtomEntry>>();

        private readonly Dictionary<string, int> eTags = new Dictionary<string, int>();

        #endregion

        #region constructor


        #endregion

        #region IAtomEntryRepository Members

        public int GetEntriesCount(EntryCriteria criteria)
        {
          int count;
          GetEntries(criteria, 0, int.MaxValue, out count);
          return count;
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

        public IEnumerable<AtomEntry> GetEntries(EntryCriteria criteria, int pageIndex, int pageSize, out int totalEntries)
        {
            IQueryable<Domain.AtomEntry> entries = GetEntries();

            if (criteria.StartDate.HasValue)
            {
                entries = entries.Where(entry => (entry.Published.GetValueOrDefault().DateTime >= criteria.StartDate));
            }

            if (criteria.EndDate.HasValue)
            {
                entries = entries.Where(entry => (entry.Published.GetValueOrDefault().DateTime <= criteria.EndDate));
            }

            if (!string.IsNullOrEmpty(criteria.WorkspaceName))
            {
                entries = entries.FilterByWorkspace(criteria.WorkspaceName);
            }

            if (!string.IsNullOrEmpty(criteria.CollectionName))
            {
                entries = entries.FilterByCollection(criteria.CollectionName);
            }

            if (! criteria.Authorized)
            {
                entries = entries.FilterByVisible();
            }

            if (!string.IsNullOrEmpty(criteria.CategoryTerm))
            {
                entries = entries.FilterByCategories(criteria.CategoryTerm, criteria.CategoryScheme);
            }

            if (!string.IsNullOrEmpty(criteria.PersonName))
            {
                entries = entries.FilterByPersonName(criteria.PersonName, criteria.PersonType);
            }


            if (criteria.EntryId != null)
            {
                // filter by annotations
                entries = FilterResultsByAnnotations(entries, criteria.EntryId, criteria.Deep);
            }

            if (!string.IsNullOrEmpty(criteria.SearchTerm))
            {
                entries = entries.FilterBySearchTerm(criteria.SearchTerm);
            }

            switch (criteria.SortMethod)
            {
                case SortMethod.EditDesc:
                    entries = entries.OrderByDescending(entry => entry.Edited);
                    break;

                case SortMethod.DateAsc:
                    entries = entries.OrderBy(entry => entry.Published);
                    break;

                case SortMethod.DateDesc:
                    entries = entries.OrderByDescending(entry => entry.Published);
                    break;
                case SortMethod.Default:
                    // no sorting
                    break;

                default:
                    // unhandled option
                    throw new ArgumentException("criteria.SortMethod");
            }

            totalEntries = entries.Count();

            entries = entries.Page(pageSize, pageIndex);

            return entries;
        }

        public AtomEntry GetEntry(Id entryId)
        {
            return GetEntryById(entryId);
        }

        public string GetEntryEtag(Id entryId)
        {
            string key = entryId.ToString();
            if (eTags.ContainsKey(key))
            {
                // return the entry Id with a version apended
                return key + "::" + eTags[key];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Delete an entry by id
        /// </summary>
        /// <param name="entryId"></param>
        public void DeleteEntry(Id entryId)
        {
            int index = IndexOfId(entryId);
            if (index >= 0)
            {
                entriesList.RemoveAt(index);
            }
        }

        public AtomEntry CreateEntry(AtomEntry entry)
        {
            if (!entriesList.Contains(entry))
            {
                entriesList.Add(entry);
                eTags.Add(entry.Id.ToString(), 1);

                Domain.AtomEntry parent = GetParentEntry(entry);
                if (parent != null)
                {
                    AttachAnnotation(parent, entry);
                }
            }

            return entry;
        }

        public AtomEntry UpdateEntry(AtomEntry entry)
        {
            AtomEntry existingEntry = GetEntryById(entry.Id);

            if (existingEntry == null)
            {
                entriesList.Add(entry);
                eTags.Add(entry.Id.ToString(), 1);
            }
            else
            {
                int index = IndexOfId(entry.Id);
                entriesList[index] = entry;

                // entry has changed, increment the entry's eTag
                string eTagKey = entry.Id.ToString();
                eTags[eTagKey] = eTags[eTagKey] + 1;
            }

            return entry;
        }

        private int IndexOfId(Id id)
        {
            string idString = id.ToString();

            for (int loopIndex = 0; loopIndex < entriesList.Count(); loopIndex++)
            {
                if (entriesList[loopIndex].Id.ToString() == idString)
                {
                    return loopIndex;
                }
            }

            return -1;
        }

        public void ApproveEntry(Id entryId)
        {
            AtomEntry entry = GetEntryById(entryId);

            entry.Control.Approved = true;
            entry.Updated = DateTime.Now;
        }

        public int ApproveAll(Id id)
        {
            List<AtomEntry> entriesToUpdate = new List<AtomEntry>();
            AtomEntry rootEntry = GetEntryById(id);

            if (! rootEntry.Approved)
            {
                entriesToUpdate.Add(rootEntry);
            }

            entriesToUpdate.AddRange(GetAnnotations(rootEntry, true));

            // update all of the entries that are not yet approved
            int authCount = 0;
            DateTimeOffset dateApproval = DateTimeOffset.Now;

            foreach (AtomEntry entry in entriesToUpdate)
            {
                if (!entry.Approved)
                {
                    authCount++;
                    entry.Control.Approved = true;
                    entry.Edited = dateApproval;
                }
            }

            return authCount;
        }

        #endregion

        #region annotations support 

        private AtomEntry GetParentEntry(AtomEntry entry)
        {
            Id parentId = entry.Id.GetParentId();

            if (parentId != null)
            {
                return GetEntryById(parentId);
            }

            return null;
        }


        private void AttachAnnotation(Domain.AtomEntry parent, Domain.AtomEntry entry)
        {
            string key = parent.Id.ToString();
            if (!annotationMap.ContainsKey(key))
            {
                annotationMap.Add(key, new List<Domain.AtomEntry>());
            }

            List<Domain.AtomEntry> childEntries = annotationMap[key];

            if (!childEntries.Contains(entry))
            {
                childEntries.Add(entry);
            }
        }


        private IQueryable<AtomEntry> FilterResultsByAnnotations(IQueryable<AtomEntry> results, Id parentEntryId, bool deep)
        {
            List<AtomEntry> annotations = GetAnnotations(parentEntryId, deep);
            List<string> annotationIds = new List<string>(from an in annotations select an.Id.ToString());

            return results.Where(ent => annotationIds.Contains(ent.Id.ToString()));
        }

        private List<Domain.AtomEntry> GetAnnotations(Id parentEntryId, bool deep)
        {
            Domain.AtomEntry parent = GetEntryById(parentEntryId);

            if (parent != null)
            {
                return GetAnnotations(parent, deep);
            }
            else
            {
                return new List<Domain.AtomEntry>();
            }
        }

        private List<AtomEntry> GetAnnotations(Domain.AtomEntry parentEntry, bool deep)
        {
            List<Domain.AtomEntry> result = new List<Domain.AtomEntry>();
            string key = parentEntry.Id.ToString();

            if (annotationMap.ContainsKey(key))
            {
                List<Domain.AtomEntry> annotations = annotationMap[key];
                result.AddRange(annotations);

                if (deep)
                {
                    foreach (Domain.AtomEntry childEntry in annotations)
                    {
                        result.AddRange(GetAnnotations(childEntry, true));
                    }
                }
            }

            return result;
        }


        #endregion

        #region private

        private AtomEntry GetEntryById(Id entryId)
        {
            return entriesList.Where(entry => entry.Id.ToString() == entryId.ToString()).SingleOrDefault();
        }

        private IQueryable<AtomEntry> GetEntries()
        {
            return entriesList.AsQueryable();
        }

        #endregion


        #region IAtomEntryRepository Members


        public System.Web.Caching.CacheDependency GetCacheDependency(EntryCriteria criteria)
        {
          throw new NotImplementedException();
        }

        #endregion
    }
}
