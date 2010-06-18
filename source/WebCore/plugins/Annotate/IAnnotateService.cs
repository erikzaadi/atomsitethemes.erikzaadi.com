/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.IO;
  using AtomSite.Domain;

  public interface IAnnotateService
  {
    //support all scopes
    AtomFeed GetAnnotations(bool deep, int pageIndex, int pageSize);
    AtomFeed GetAnnotations(string workspace, bool deep, int pageIndex, int pageSize);
    AtomFeed GetAnnotations(Id id, bool deep, int pageIndex, int pageSize);

    AtomEntry Annotate(Id entryId, AtomEntry entry, string slug);
    AtomEntry MediaAnnotate(Id entryId, Stream stream, string slug, string contentType);
    AnnotationState GetAnnotationState(AppCollection coll, Id entryId);
    event Action<Id, AtomEntry, string> AnnotatingEntry;
    event Action<AtomEntry> EntryAnnotated;
  }
}
