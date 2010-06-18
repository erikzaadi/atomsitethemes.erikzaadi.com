/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.Dlinq
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.ServiceModel.Syndication;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;
  using AtomSite.Domain;

  /// <summary>
  /// This class can store AtomEntries into a database by converting it into
  /// the common .Net Framework SyndicationItem.
  /// </summary>
  /// <remarks>This class is not finished.</remarks>
  public class DlinqAtomEntryRepository : DlinqBaseRepository, IAtomEntryRepository
  {
    public DlinqAtomEntryRepository()
      : base(new ItemDataContext())
    { }

    #region IAtomEntryRepository Members

    public int GetEntriesCount(EntryCriteria criteria)
    {
      throw new NotImplementedException();
    }

    public int DeleteEntries(EntryCriteria criteria)
    {
      int count;
      foreach (AtomEntry e in GetEntries(criteria, 0, int.MaxValue, out count))
      {
        DeleteEntry(e.Id);
      }
      return count;
    }

    public IEnumerable<AtomEntry> GetEntries(EntryCriteria criteria, int pageIndex, int pageSize, out int totalEntries)
    {
      throw new NotImplementedException();
    }

    public AtomEntry GetEntry(Id entryId)
    {
      ItemDataContext dc = new ItemDataContext();
      Item item = dc.Items.Where(i => i.Id == entryId.ToString()).SingleOrDefault();
      SyndicationItem si = new SyndicationItem()
      {
        Id = item.Id,
        LastUpdatedTime = item.LastUpdatedTime,
        PublishDate = item.PublishDate.Value
      };
      if (!string.IsNullOrEmpty(item.BaseUri)) si.BaseUri = new Uri(item.BaseUri);

      LoadAttributes(si.AttributeExtensions, item.Attributes);
      LoadElements(si.ElementExtensions, item.Elements);
      LoadPersons(si.Authors, item.Persons, PersonTypeAuthor);
      LoadPersons(si.Contributors, item.Persons, PersonTypeContributor);
      si.Content = GetContent(item.Content);
      si.Title = GetTextContent(item.Title);
      si.Summary = GetTextContent(item.Summary);
      si.Copyright = GetTextContent(item.Copyright);
      LoadLinks(si.Links, item.Links);
      LoadCategories(si.Categories, item.Categories);

      using (Stream s = new MemoryStream())
      {
        XmlWriter w = new XmlTextWriter(s, Encoding.UTF8);
        si.GetAtom10Formatter().WriteTo(w);
        w.Flush();
        AtomEntry entry = new AtomEntry();
        s.Position = 0;
        XmlReader r = new XmlTextReader(s);
        entry.Xml = XElement.Load(r);
        //entry.ReadXml(r);
        return entry;
      }
    }

    public string GetEntryEtag(Id entryId)
    {
      throw new NotImplementedException();
    }

    public void DeleteEntry(Id entryId)
    {
      throw new NotImplementedException();
    }

    public AtomEntry CreateEntry(AtomEntry entry)
    {
      //translate from atom entry into syndication item for persisting
      //TODO: make this an extension
      SyndicationItem synItem;
      using (MemoryStream ms = new MemoryStream())
      {
        XmlWriter w = new XmlTextWriter(ms, Encoding.UTF8);
        entry.Xml.Save(w);
        w.Flush();
        ms.Position = 0;
        XmlReader r = new XmlTextReader(ms);
        synItem = SyndicationItem.Load(r);
      }
      
      //TODO: transaction, handle existing Id
      using (ItemDataContext dc = new ItemDataContext())
      {
        Item i = new Item();
        i.BaseUri = synItem.BaseUri != null ? synItem.BaseUri.OriginalString : null;
        i.Id = synItem.Id;
        i.LastUpdatedTime = synItem.LastUpdatedTime.DateTime;
        i.PublishDate = synItem.PublishDate.DateTime;
        i.Title = CreateTextContent(synItem.Title);
        i.Summary = CreateTextContent(synItem.Summary);
        i.Copyright = CreateTextContent(synItem.Copyright);
        //TODO: content
        dc.SubmitChanges(); //save content to get content keys (one to one)
        
        dc.Items.InsertOnSubmit(i);
        dc.SubmitChanges(); //save item to get item key

        //child data (one to many)
        SaveAttributes(i.Attributes, synItem.AttributeExtensions, i.ItemKey);
        SaveElements(i.Elements, synItem.ElementExtensions, i.ItemKey);
        SavePeople(i.Persons, synItem.Authors, PersonTypeAuthor, i.ItemKey);
        SavePeople(i.Persons, synItem.Contributors, PersonTypeContributor, i.ItemKey);
        //TODO:links, categories
      }
      return entry;
    }

    public AtomEntry UpdateEntry(AtomEntry entry)
    {
      throw new NotImplementedException();
    }

    public void ApproveEntry(Id entryId)
    {
      throw new NotImplementedException();
    }

    public int ApproveAll(Id id)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IAtomEntryRepository Members


    public System.Web.Caching.CacheDependency GetCacheDependency(EntryCriteria criteria)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
