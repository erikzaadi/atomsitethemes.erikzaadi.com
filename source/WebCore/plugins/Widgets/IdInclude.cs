/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using AtomSite.Domain;

  public class IdInclude : Include
  {
    public IdInclude() : base() { }
    public IdInclude(Include include) : base(include) { }

    public Id Id
    {
      get { return GetUri(Atom.SvcNs + "id"); }
      set { SetUri(Atom.SvcNs + "id", value); }
    }
  }

  public class FeedInclude : IdInclude
  {
    public FeedInclude() : base() { }
    public FeedInclude(Include include) : base(include) { }
    
    public int? Count
    {
      get { return GetInt32(Atom.SvcNs + "count"); }
      set { SetInt32(Atom.SvcNs + "count", value); }
    }

    public string Title
    {
      get { return GetValue<string>(Atom.SvcNs + "title"); }
      set { SetValue<string>(Atom.SvcNs + "title", value); }
    }
  }
}
