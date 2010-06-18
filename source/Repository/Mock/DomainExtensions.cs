using System;
using System.Collections.Generic;
using System.Linq;
using AtomSite.Domain;

namespace AtomSite.Repository
{
    public static class DomainExtensions
    {
        #region AtomEntry


        public static IQueryable<AtomEntry> FilterByVisible(this IQueryable<AtomEntry> entries)
        {
            return entries.Where(entry => (entry.Control != null) && (!entry.Control.Draft.GetValueOrDefault()) && (entry.Control.Approved.GetValueOrDefault()));
        }

        public static IQueryable<AtomEntry> FilterByPersonName(this IQueryable<AtomEntry> entries, string name, string type)
        {
            //get entries that have a person of the correct type and name
            Func<AtomEntry, bool> personFilter = 
                entry => entry.People.Exists(
                    p => (p.Type == "person" || p.Type == type) &&
                    String.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));

            return entries.Where(entry => personFilter(entry));
        }

        public static IQueryable<AtomEntry> FilterByCategories(this IQueryable<AtomEntry> entries, string term, Uri scheme)
        {
            // entries where any category matches
            Func<AtomEntry, bool> categoriesFilter = 
                entry => entry.Categories.Exists(
                    c => String.Equals(c.Term, term, StringComparison.InvariantCultureIgnoreCase) &&
                    (c.Scheme == scheme || scheme == null));

            return entries.Where(entry => categoriesFilter(entry));
        }

        public static IQueryable<AtomEntry> FilterByWorkspace(this IQueryable<AtomEntry> entries, string workspaceName)
        {
            Func<AtomEntry, bool> workspaceFilter = entry => entry.Id.Workspace == workspaceName;

            return entries.Where(entry => workspaceFilter(entry));
        }

        public static IQueryable<AtomEntry> FilterByCollection(this IQueryable<AtomEntry> entries, string collectionName)
        {
            Func<AtomEntry, bool> collectionFilter = entry => entry.Id.Collection == collectionName;

            return entries.Where(entry => collectionFilter(entry));
        }

        public static IQueryable<AtomEntry> FilterBySearchTerm(this IQueryable<AtomEntry> entries, string searchTerm)
        {
            searchTerm = searchTerm.ToUpperInvariant();

            return entries.Where(e =>
                 e.Title.Text.ToUpperInvariant().Contains(searchTerm) ||
                 e.Summary.Text.ToUpperInvariant().Contains(searchTerm) ||
                 e.Content.Text.ToUpperInvariant().Contains(searchTerm));
        }

        #endregion

        #region other list extensions

        public static IEnumerable<AppCollection> RemoveTitles(this IEnumerable<AppCollection> collections, params string[] titles)
        {
            if (collections == null)
            {
                return null;
            }

            List<AppCollection> result = new List<AppCollection>(collections);

            result.RemoveAll(col => titles.Contains(col.Title.Text)); 

            return result;
        }

        public static IEnumerable<AppWorkspace> RemoveByName(this IEnumerable<AppWorkspace> workspaces, string name)
        {
            if (workspaces == null)
            {
                return null;
            }

            List<AppWorkspace> result = new List<AppWorkspace>(workspaces);

            result.RemoveAll(ws => ((ws.Name == name)));

            return result;
        }

        #endregion
    }
}
