/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Linq;

  /// <summary>
  /// This class represents a unique identifier which is also a Uri in 
  /// the tag scheme.  This class is immutable.
  /// </summary>
  public class Id
  {
    public Id(string owner, DateTime date, string collection, string entryPath) :
      this(owner, date.Year.ToString("0000") + "-" + date.Month.ToString("00") + "-"
            + date.Day.ToString("00"), collection, entryPath) { }

    public Id(string owner, string collection) :
      this(owner, DateTime.Now.Year.ToString(), collection) { }

    public Id(string owner, string date, string collection) :
      this(owner, date, collection, null) { }

    public Id(string owner, string date, string collection, string entryPath)
    {
      Owner = owner;
      //TODO: verify date is valid, when no entry path, must only be year
      //format must be "YYYY-MM-DD"
      Date = date;
      Collection = collection;
      EntryPath = entryPath;
    }

    public string Owner { get; private set; }
    public string Date { get; private set; }
    public string Collection { get; private set; }
    public string EntryPath { get; private set; }
    public string Workspace
    {
      get
      {
        //when email
        if (Owner.Contains('@'))
        {
          return Owner.Substring(0, Owner.IndexOf('@'));
        }
        //when domain w/ subdomain
        else if (Owner.Split('.').Length > 2)
        {
          return Owner.Substring(0, Owner.LastIndexOf(".", Owner.LastIndexOf(".") - 1));
        }
        else
        {
          return Atom.DefaultWorkspaceName;
        }
      }
    }

    public bool IsAnnotation
    {
      get { return EntryPath != null && EntryPath.Contains(','); }
    }

    public Id GetParentId()
    {
      return
          EntryPath.LastIndexOf(',') > -1 ?
                new Id(Owner, Date, Collection, EntryPath.Substring(0, EntryPath.LastIndexOf(','))) :
                null;
    }

    public override bool Equals(object obj)
    {
      if (obj != null && obj.GetType().Equals(this.GetType()))
      {
        return this.ToString().Equals(obj.ToString());
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.ToString().GetHashCode();
    }

    public override string ToString()
    {
      return ((Uri)this).OriginalString;
    }

    public string ToWebId()
    {
      Id parentId = this.GetParentId();
      return parentId != null ?
            this.EntryPath.Substring(this.GetParentId().EntryPath.Length + 1) :
            this.EntryPath;
    }
    public string ToFullWebId()
    {
      string date = !string.IsNullOrEmpty(Date) ? Date.Replace('-','_') + "_" : string.Empty;
      return string.Format("{0}_{1}_{2}{3}",this.Workspace, this.Collection, date,  this.EntryPath.Replace(',', '_'));
    }
    public Scope ToScope()
    {
      return new Scope() { Workspace = Workspace, Collection = Collection };
    }

    public Id AddPath(string entryPath)
    {
      //TODO: detect dated entry
      return new Id(this.Owner, this.Date, this.Collection, entryPath);
    }

    public Id AddPath(string date, string entryPath)
    {
      if (string.IsNullOrEmpty(date)) return AddPath(entryPath);
      return new Id(this.Owner, date, this.Collection, entryPath);
    }

    public Id AddPath(int? year, int? month, int? day, string entryPath)
    {
      if (year.HasValue && month.HasValue && day.HasValue)
        return AddPath(year.Value.ToString("0000") + "-" + month.Value.ToString("00") + 
          "-" + day.Value.ToString("00"), entryPath);
      else
        return AddPath(entryPath);
    }

    public static implicit operator Uri(Id id)
    {
      if (id == null) return null;
      return new Uri("tag:" + id.Owner + "," + id.Date + ":" + id.Collection +
                (!string.IsNullOrEmpty(id.EntryPath) ? ("," + id.EntryPath) : string.Empty));
    }

    public static implicit operator Id(Uri uri)
    {
      if (uri == null) return null;
      if (uri.Scheme != "tag") throw new ArgumentException("Uri is not a tag.", "uri");
      try
      {
        string owner = uri.OriginalString.Split(':')[1].Split(',')[0];
        string date = uri.OriginalString.Split(':')[1].Split(',')[1];
        string collection = uri.OriginalString.Split(':')[2].Split(',')[0];
        string entryPath = uri.OriginalString.Split(',').Length > 2 ?
                    string.Join(",", uri.OriginalString.Split(',').Skip(2).ToArray()) :
                    null;
        return new Id(owner, date, collection, entryPath);
      }
      catch (Exception)
      {
        throw new InvalidCastException("Uri is not a valid tag scheme.");
      }
    }
    public static implicit operator Id(string tag)
    {
      if (tag == null) return null;
      return (Id)new Uri(tag);
    }
  }
}
