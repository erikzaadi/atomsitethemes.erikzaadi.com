/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.Dlinq
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Data.Linq;
  using System.IO;
  using System.Linq;
  using System.ServiceModel.Syndication;
  using System.Xml;

  public abstract class DlinqBaseRepository
  {
    protected readonly ItemDataContext dc;

    public DlinqBaseRepository(ItemDataContext itemDataContext)
    {
      dc = itemDataContext;
    }

    public static readonly string PersonTypeAuthor = "author";
    public static readonly string PersonTypeContributor = "contributor";
    public static readonly string PersonTypeAdmin = "admin";

    protected DateTimeOffset GetDateTimeOffset(DateTime dateTime)
    {
      //TODO: update, temporarily hard-code offset
      return new DateTimeOffset(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time").BaseUtcOffset);
    }

    protected TextSyndicationContent GetTextContent(Content content)
    {
      if (content == null) return null;
      return new TextSyndicationContent(content.Text,
          (string.IsNullOrEmpty(content.Type) || content.Type == "text") ?
        TextSyndicationContentKind.Plaintext :
        content.Type == "xhtml" ? TextSyndicationContentKind.XHtml :
        TextSyndicationContentKind.Html);
    }

    protected void LoadCategories(Collection<SyndicationCategory> collection, EntitySet<Category> entitySet)
    {
      foreach (Category entity in entitySet)
      {
        SyndicationCategory s = new SyndicationCategory()
        {
          Label = entity.Label,
          Name = entity.Name,
          Scheme = entity.Scheme
        };
        LoadAttributes(s.AttributeExtensions, entity.Attributes);
        LoadElements(s.ElementExtensions, entity.Elements);
      }
    }

    protected void LoadLinks(Collection<SyndicationLink> collection, EntitySet<Link> entitySet)
    {
      foreach (Link entity in entitySet)
      {
        SyndicationLink s = new SyndicationLink()
        {
          BaseUri = !string.IsNullOrEmpty(entity.BaseUri) ? null : new Uri(entity.BaseUri),
          Length = entity.Length.Value,
          MediaType = entity.MediaType,
          RelationshipType = entity.RelationshipType,
          Title = entity.Title,
          Uri = !string.IsNullOrEmpty(entity.Uri) ? null : new Uri(entity.Uri)
        };
        LoadAttributes(s.AttributeExtensions, entity.Attributes);
        LoadElements(s.ElementExtensions, entity.Elements);
      }
    }

    protected SyndicationContent GetContent(Content content)
    {
      SyndicationContent sc = null;
      if (content != null)
      {
        if (!string.IsNullOrEmpty(content.Url))
        {
          sc = new UrlSyndicationContent(
            !string.IsNullOrEmpty(content.Url) ? null : new Uri(content.Url),
            content.Type);
        }
        if (string.IsNullOrEmpty(content.Type) || content.Type == "xhtml" ||
          content.Type == "html" || content.Type == "text")
        {

          sc = new TextSyndicationContent(content.Text,
            (string.IsNullOrEmpty(content.Type) || content.Type == "text") ?
          TextSyndicationContentKind.Plaintext :
          content.Type == "xhtml" ? TextSyndicationContentKind.XHtml :
          TextSyndicationContentKind.Html);
        }
        else
        {
          sc = new XmlSyndicationContent(content.Type,
           new SyndicationElementExtension(new XmlTextReader(new StringReader(content.Text))));
        }
        LoadAttributes(sc.AttributeExtensions, content.Attributes);
      }
      return sc;
    }

    protected void LoadPersons(Collection<SyndicationPerson> persons, EntitySet<Person> entitySet, string personType)
    {
      foreach (Person person in entitySet.Where(p => p.Type == personType))
      {
        SyndicationPerson p = new SyndicationPerson()
        {
          Email = person.Email,
          Name = person.Name,
          Uri = person.Uri //TODO: extensions
        };
        LoadAttributes(p.AttributeExtensions, person.Attributes);
        LoadElements(p.ElementExtensions, person.Elements);
        persons.Add(p);
      }
    }

    protected void LoadElements(SyndicationElementExtensionCollection collection, EntitySet<Element> entitySet)
    {
      foreach (Element element in entitySet)
      {
        collection.Add(new SyndicationElementExtension(new XmlTextReader(new StringReader(element.Xml))));
      }
    }

    protected void LoadAttributes(Dictionary<XmlQualifiedName, string> dictionary, EntitySet<Attribute> entitySet)
    {//TODO: support null 
      //AttributeExtensions = new Dictionary<XmlQualifiedName,string>( item.Attributes.Select(a => new KeyValuePair<XmlQualifiedName, string>
      //( new XmlQualifiedName( a.Name, a.Namespace), a.Value)).ToList()),
      foreach (Attribute a in entitySet)
      {
        dictionary.Add(new XmlQualifiedName(a.Name, a.Namespace), a.Value);
      }
    }

    protected void SavePeople(EntitySet<Person> people, Collection<SyndicationPerson> collection,
      string personType, int itemKey)
    {
      foreach (SyndicationPerson person in collection)
      {
        Person p = new Person()
        {
          ParentKey = itemKey,
          Name = person.Name,
          Uri = person.Uri,
          Email = person.Email,
          Type = personType
        };

        people.Add(p);
        dc.Persons.InsertOnSubmit(p);
        dc.SubmitChanges();

        //add attributes and elements to people
        SaveAttributes(p.Attributes, person.AttributeExtensions, p.PersonKey);
        SaveElements(p.Elements, person.ElementExtensions, p.PersonKey);
      }
    }

    protected void SaveAttributes(EntitySet<Attribute> attributes,
      Dictionary<XmlQualifiedName, string> dictionary, int parentKey)
    {
      var atts = dictionary.Select(ae => new Attribute()
      {
        Name = ae.Key.Name,
        Namespace = ae.Key.Namespace,
        Value = ae.Value,
        ParentKey = parentKey
      });
      foreach (Attribute att in atts)
      {
        attributes.Add(att);
        dc.Attributes.InsertOnSubmit(att);
      }
      dc.SubmitChanges();
    }

    protected void SaveElements(EntitySet<Element> elements,
      SyndicationElementExtensionCollection collection, int parentKey)
    {
      var els = collection.Select(e => new Element()
      {
        OuterNamespace = e.OuterNamespace,
        OuterName = e.OuterName,
        Xml = e.GetReader().ReadOuterXml(),
        ParentKey = parentKey
      });
      foreach (Element e in els)
      {

        elements.Add(e);
        dc.Elements.InsertOnSubmit(e);
      }
      dc.SubmitChanges();
    }

    protected Content CreateTextContent(TextSyndicationContent sc)
    {
      if (sc == null) return null;
      Content content = new Content() { Text = sc.Text, Type = sc.Type };
      content.Attributes.AddRange(sc.AttributeExtensions.Select(ae => new Attribute()
      {
        Name = ae.Key.Name,
        Namespace = ae.Key.Namespace,
        Value = ae.Value
      }));
      dc.Contents.InsertOnSubmit(content);
      return content;
    }
  }
}
