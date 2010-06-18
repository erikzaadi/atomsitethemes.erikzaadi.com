/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository
{
  using System;
  using AtomSite.Domain;
  /// <summary>
  /// Critera for retrieving entries from the entry repository
  /// </summary>
  public struct EntryCriteria
  {
    public static EntryCriteria Empty = new EntryCriteria();

    public string WorkspaceName { get; set; }
    public string CollectionName { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool Authorized { get; set; }
    public SortMethod SortMethod { get; set; }

    public string CategoryTerm { get; set; }
    public Uri CategoryScheme { get; set; }

    public string PersonName { get; set; }
    public string PersonType { get; set; }

    public string SearchTerm { get; set; }

    public Id EntryId { get; set; }
    public bool Annotations { get; set; }
    public bool Deep { get; set; }
    public bool? Approved { get; set; }
    public bool? Draft { get; set; }
    public bool? Pending { get; set; }
    public bool? Published { get; set; }
    public bool? Spam { get; set; }

    public bool SummaryView { get; set; }

    // override object.Equals
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      return this.ToString().Equals(obj.ToString());
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
      return this.ToString().GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("{0},{1},{2}-{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
        WorkspaceName, CollectionName, StartDate, EndDate, Authorized, SortMethod, CategoryTerm, CategoryScheme,
        PersonName, PersonType, SearchTerm, EntryId, Annotations, Deep, Approved, Draft, Pending, Published, Spam,
        SummaryView);
    }
  }

  public enum SortMethod
  {
    Default=0,
    EditDesc,
    DateAsc,
    DateDesc
  }
}
