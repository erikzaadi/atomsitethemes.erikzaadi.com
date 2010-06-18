/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;
    using StructureMap;
    using System.Linq;

  public class EntryModel : EntryModel<AtomEntry> { }

  public class EntryModel<T> : PageModel where T : AtomEntry
  {
    public T Entry { get; set; }

    protected override void OnUpdatePageModel(IContainer container)
    {
      base.OnUpdatePageModel(container);

      //also allow inline widgets to update pagemodel
      var includes = Entry.Xml.Descendants(Include.IncludeXName).Select(i => new Include(i));
        
      //let each widget update page model
      foreach (Include include in includes)
      {
        var w = container.TryGetInstance<IWidget>(include.Name);
        if (w != null) w.UpdatePageModel(this, include);
      }
    }
  }
}
