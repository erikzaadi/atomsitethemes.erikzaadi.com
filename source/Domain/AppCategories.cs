/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Serialization;

  [XmlRoot("categories", Namespace = "http://www.w3.org/2007/app")]
  public class AppCategories : XmlBase, IXmlSerializable
  {
    public AppCategories() : this(new XElement(Atom.AppNs + "categories")) { }
    public AppCategories(XElement xml) : base(xml) { }

    public Uri Scheme
    {
      get { return GetUriProperty("scheme"); }
      set { SetUriProperty("scheme", value); }
    }

    public bool? Fixed
    {
      get { return GetBooleanProperty("fixed"); }
      set { SetYesNoBooleanProperty("fixed", value); }
    }

    /// <summary>
    /// Gets or sets the link location.
    /// </summary>
    /// <value>The href.</value>
    public Uri Href
    {
      get { return GetUriProperty("href"); }
      set { SetUriProperty("href", value); }
    }

    public bool IsExternal { get { return Href != null; } }

    public IEnumerable<AtomCategory> Categories
    {
      get { return IsExternal ? null : Xml.Elements(Atom.AtomNs + "category").Select(x => new AtomCategory(x)); }
      set
      {
        if (IsExternal) throw new InvalidOperationException("Categories are external.");
        Xml.Elements(Atom.AtomNs + "category").Remove();
        Xml.Add(value.Select(c => c.Xml));
      }
    }

    public static AppCategories Load(string path)
    {
      AppCategories cats = new AppCategories();
      cats.Xml = XElement.Load(path, LoadOptions.None);
      return cats;
    }

    public void Save(string path)
    {
      Xml.Save(path);
    }

    #region IXmlSerializable Members

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
      Xml = XElement.Load(reader, LoadOptions.SetBaseUri);
    }

    public void WriteXml(XmlWriter writer)
    {
      Dictionary<string, string> prefixes = new Dictionary<string, string>();
      prefixes.Add("atom", Atom.AtomNs.NamespaceName);
      WriteXml("categories", Atom.AppNs.NamespaceName, prefixes, writer);
    }


    #endregion

    public bool AddCategory(AtomCategory cat)
    {
      //TODO: support parent scheme according to standard

      //validate category term does not contain special characters,
      // if it does, remove them and use original term as label (when no label)
      if (cat.Term.ToLowerInvariant() != cat.Term.CleanSlug().ToLowerInvariant())
      {
        if (string.IsNullOrEmpty(cat.Label))
        {
          cat.Label = cat.Term;
        }
        cat.Term = cat.Term.CleanSlug();
        
        //no need for a label if same as term
        if (cat.Label == cat.Term) cat.Label = null;
      }

      if (!Categories.Contains(cat))
      {
        //The absence of the "fixed" attribute is equivalent to the presence of 
        //a "fixed" attribute with a value of "no".
        if (Fixed.HasValue && Fixed.Value)
          throw new CategoryAddWhenFixedException(cat.ToString());

        List<AtomCategory> list = Categories.ToList();
        //TODO: add the category, but ignore the scheme if it matches the parent
        //list.Add(new AtomCategory() { Term = cat.Term, Label = cat.Label });
        list.Add(cat);
        Categories = list.AsEnumerable();
        return true;
      }
      else
      {
        //normalize label
        cat.Label = Categories.Where(c => c.Scheme == cat.Scheme && c.Term == cat.Term).Single().Label;
      }
      return false;
    }
  }
}
