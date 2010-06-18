/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.IO;
  using AtomSite.Domain;
  using System.Collections;
  using System.Collections.Generic;
  using AtomSite.Repository;

  /// <summary>
  /// The AtomPubService provides support for working with the Atom object model.  This is where
  /// the business logic for working with domain is implemented.
  /// </summary>
  public interface IAtomPubService
  {
    AppService GetService();
    AppService UpdateService(AppService service);

    AtomFeed GetCollectionFeed(Id collectionId, int pageIndex);

    //support all scopes
    AtomFeed GetFeed(int pageIndex, int pageSize);
    AtomFeed GetFeed(string workspace, int pageIndex, int pageSize);
    AtomFeed GetFeed(Id collectionId, int pageIndex, int pageSize);

    IPagedList<AtomEntry> GetEntries(EntryCriteria criteria, int pageIndex, int pageSize);

    AtomFeed GetFeedByAuthor(Id collectionId, string authorName, int pageIndex, int pageSize);
    AtomFeed GetFeedByContributor(Id collectionId, string contributorName, int pageIndex, int pageSize);
    AtomFeed GetFeedByPerson(Id collectionId, string personName, int pageIndex, int pageSize);
    AtomFeed GetFeedByDate(Id collectionId, DateTime startDate, DateTime endDate, int pageIndex, int pageSize);
    AtomFeed GetFeedByCategory(Id collectionId, string term, Uri scheme, int pageIndex, int pageSize);

    //support all scopes
    AtomFeed GetFeedBySearch(string term, int pageIndex, int pageSize);
    AtomFeed GetFeedBySearch(Id collectionId, string term, int pageIndex, int pageSize);
    AtomFeed GetFeedBySearch(string workspace, string term, int pageIndex, int pageSize);

    AtomEntry GetEntry(Id entryId);
    Stream GetMedia(Id entryId, out string contentType);

    AtomEntry CreateEntry(Id collectionId, AtomEntry entry, string slug);
    AtomEntry CreateMedia(Id collectionId, Stream stream, string slug, string contentType);
    event Action<Id, AtomEntry, string> CreatingEntry;
    event Action<AtomEntry> EntryCreated;

    AtomEntry UpdateEntry(Id entryId, AtomEntry entry, string slug);
    AtomEntry UpdateMedia(Id entryId, Stream stream, string contentType);
    event Action<Id, AtomEntry, string> UpdatingEntry;
    event Action<AtomEntry> EntryUpdated;

    void DeleteEntry(Id entryId);
    void DeleteMedia(Id entryId);
    event Action<Id> DeletingEntry;
    event Action<Id> EntryDeleted;

    void DeleteWorkspace(string workspaceName, bool updateService);
    void DeleteCollection(Id collectionId, bool updateService);

    string GetEntryEtag(Id entryId);
    string GetMediaEtag(Id entryId, out string contentType);

    AtomCategory AddCategory(Id collectionId, string category, Uri scheme);
    void RemoveCategory(Id collectionId, string category, Uri scheme);

    int ApproveAll(Id id);
    void ApproveEntry(Id entryId, bool approved);

    event Action<AtomEntry> SettingEntryLinks;
    event Action<AtomFeed> SettingFeedLinks;
  }
}
