/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System.Web.Mvc;
  using AtomSite.Domain;
    using System.Linq;
  public partial class AdminController : BaseController
  {
    [ScopeAuthorize]
    public ViewResult Annotations(string workspace, string collection, string filter, string search, int? page, int? pageSize)
    {
      var annotations = GetAnnotations(workspace, collection, filter, search, page, pageSize);

      var m = new AdminAnnotationsModel() { Annotations = annotations };
      m.AllCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.AnnotationsAll(workspace, collection, null));
      m.PublishedCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.AnnotationsPublished(workspace, collection, null));
      m.UnapprovedCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.AnnotationsUnapproved(workspace, collection, null));
      m.SpamCount = AtomEntryRepository.GetEntriesCount(SelectionCriteria.AnnotationsSpam(workspace, collection, null));
      m.Filter = filter;

      return View("AdminAnnotations", "Admin", m);
    }

    [ScopeAuthorize]
    public PartialViewResult RecentAnnotations(string workspace, string collection)
    {
        return PartialView("AdminRecentAnnotationsWidget", new AdminAnnotationsModel()
        {
            Annotations = GetAnnotations(workspace, collection, null, null, 1, 6)
        });
    }

    protected IPagedList<AtomEntry> GetAnnotations(string workspace, string collection, string filter, string search, int? page, int? pageSize)
    {
      int pageIndex = page - 1 ?? 0;
      int pSize = pageSize ?? AppService.GetCollection().PageSize;

      var annotations = AtomPubService.GetEntries(
        filter == "pending" ? SelectionCriteria.AnnotationsPending(workspace, collection, search) :
        filter == "published" ? SelectionCriteria.AnnotationsPublished(workspace, collection, search) :
        filter == "spam" ? SelectionCriteria.AnnotationsSpam(workspace, collection, search) :
        filter == "unapproved" ? SelectionCriteria.AnnotationsUnapproved(workspace, collection, search) :
        SelectionCriteria.AnnotationsAll(workspace, collection, search), pageIndex, pSize);

      foreach (AtomEntry a in annotations)
      {
        a.SetValue<string>(Atom.SvcNs + "parentTitle", AtomEntryRepository.GetEntry(a.Id.GetParentId()).Title.ToString());
      }

      //filter and title links for display
      var rels = new[] { "alternate", "admin-edit", "approve", "unapprove", "delete", "reply" };
      foreach (var e in annotations)
      {
          e.Links = e.Links.Where(l => rels.Contains(l.Rel));
          var link = e.Links.Where(l => l.Rel == "alternate").FirstOrDefault();
          if (link != null) link.Title = "view";
          link = e.Links.Where(l => l.Rel == "admin-edit").FirstOrDefault();
          if (link != null) link.Title = "edit";
          link = e.Links.Where(l => l.Rel == "delete").FirstOrDefault();
          if (link != null) link.Title = "delete";
          link = e.Links.Where(l => l.Rel == "approve").FirstOrDefault();
          if (link != null) link.Title = "approve";
          link = e.Links.Where(l => l.Rel == "unapprove").FirstOrDefault();
          if (link != null) link.Title = "unapprove";
          link = e.Links.Where(l => l.Rel == "reply").FirstOrDefault();
          if (link != null) link.Title = "reply";
      }
      return annotations;
    }
  }
}
