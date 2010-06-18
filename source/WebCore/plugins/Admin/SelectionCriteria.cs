/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Repository;

  public static class SelectionCriteria
  {

    public static EntryCriteria EntriesAll(string workspace, string collection, string search, string category)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        CategoryTerm = category,
        Authorized = true
      };
    }

    public static EntryCriteria EntriesPublished(string workspace, string collection, string search, string category)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        CategoryTerm = category,
        Authorized = true,
        Draft = false,
        Published = true
      };
    }

    public static EntryCriteria EntriesDraft(string workspace, string collection, string search, string category)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        CategoryTerm = category,
        Authorized = true,
        Draft = true
      };
    }

    public static EntryCriteria EntriesUnapproved(string workspace, string collection, string search, string category)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        CategoryTerm = category,
        Authorized = true,
        Approved = false
      };
    }

    public static EntryCriteria EntriesPending(string workspace, string collection, string search, string category)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        CategoryTerm = category,
        Authorized = true,
        Pending = true
      };
    }
    public static EntryCriteria AnnotationsAll(string workspace, string collection, string search)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        Annotations = true,
        Deep = true,
        Authorized = true,
        SortMethod = SortMethod.EditDesc
      };
    }

    public static EntryCriteria AnnotationsPublished(string workspace, string collection, string search)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        Annotations = true,
        Deep = true,
        Authorized = true,
        Published = true,
        SortMethod = SortMethod.EditDesc
      };
    }

    public static EntryCriteria AnnotationsSpam(string workspace, string collection, string search)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        Annotations = true,
        Deep = true,
        Authorized = true,
        Spam = true,
        SortMethod = SortMethod.EditDesc
      };
    }

    public static EntryCriteria AnnotationsUnapproved(string workspace, string collection, string search)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        Annotations = true,
        Deep = true,
        Authorized = true,
        Approved = false,
        SortMethod = SortMethod.EditDesc
      };
    }

    public static EntryCriteria AnnotationsPending(string workspace, string collection, string search)
    {
      return new EntryCriteria()
      {
        WorkspaceName = workspace,
        CollectionName = collection,
        SearchTerm = search,
        Annotations = true,
        Deep = true,
        Authorized = true,
        Pending = true,
        SortMethod = SortMethod.EditDesc
      };
    }
  }
}
