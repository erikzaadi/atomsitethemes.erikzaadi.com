/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;

  /// <summary>
  /// This class acts as the base of the Atom object model and provides helper methods for
  /// exposing/updating the data in a strongly typed fashon.
  /// </summary>
  [Serializable]
  public abstract class XmlBase : MarshalByRefObject
  {
    private static readonly Dictionary<string, bool> boolVals = new Dictionary<string, bool>();

    static XmlBase()
    {
      boolVals.Add("yes", true);
      boolVals.Add("no", false);
      boolVals.Add("true", true);
      boolVals.Add("false", false);
      boolVals.Add("1", true);
      boolVals.Add("0", false);
    }

    protected XmlBase() { }

    protected XmlBase(XElement xml)
    {
      Xml = xml;
    }

    public static implicit operator XElement(XmlBase xmlBase)
    {
      return xmlBase.Xml;
    }

    public IEnumerable<T> GetXmlValues<T>(XName name) where T : XmlBase, new()
    {
      return Xml.Elements(name).Select(x => new T() { Xml = x });
    }

    public void SetXmlValues<T>(XName name, IEnumerable<T> t) where T : XmlBase
    {
      //keep object references so they are not actually destroyed on remove
      List<T> list = t != null ? t.Where(x => x != null).ToList() : null;

      //nothing to add, just clear existing
      if (t == null)
      {
        Xml.Elements(name).Remove();
        return;
      }

      list.ForEach(x => x.Xml.Name = name);

      //if existing, replace
      if (Xml.Elements(name).Count() > 0)
      {
        //remove all but first to replace
        Xml.Elements(name).Except(Xml.Elements(name).Take(1)).Remove();
        //replace first
        Xml.Elements(name).First().ReplaceWith(list.Select(x => x.Xml));
      }
      else //just add, nothing to replace
      {
        Xml.Add(list.Select(x => x.Xml));
      }
    }

    public T GetXmlValue<T>(XName name) where T : XmlBase, new()
    {
      return Xml.Element(name) != null ? new T() { Xml = Xml.Element(name) } : default(T);
    }
    public void SetXmlValue<T>(XName name, T t) where T : XmlBase
    {
      //nothing to add, just clear existing
      if (t == null)
      {
        Xml.Elements(name).Remove();
        return;
      }

      t.Xml.Name = name;

      //if existing, replace
      if (Xml.Elements(name).Count() > 0)
      {
        //remove all but first to replace
        Xml.Elements(name).Except(Xml.Elements(name).Take(1)).Remove();
        //replace first
        Xml.Elements(name).First().ReplaceWith(t.Xml);
      }
      else //just add, nothing to replace
      {
        Xml.Add(t.Xml);
      }
    }

    public IEnumerable<T> GetValues<T>(XName name) where T : class
    {
      return Xml.Elements(name).Select(x => x.Value as T);
    }
    public void SetValues<T>(XName name, IEnumerable<T> t) where T : class
    {
      //nothing to add, just clear existing
      if (t == null)
      {
        Xml.Elements(name).Remove();
        return;
      }

      //if existing, replace
      if (Xml.Elements(name).Count() > 0)
      {
        //remove all but first to replace
        Xml.Elements(name).Except(Xml.Elements(name).Take(1)).Remove();
        //replace first
        Xml.Elements(name).First().ReplaceWith(t.Select(x => new XElement(name, x)));
      }
      else //just add, nothing to replace
      {
        Xml.Add(t.Select(x => new XElement(name, x)));
      }
    }

    public virtual T GetValue<T>(XName name) where T : class
    {
      return Xml.Element(name) != null ? Xml.Element(name).Value as T : default(T);
    }
    public void SetValue<T>(XName name, T t) where T : class
    {
      //nothing to add, just clear existing
      if (t == null)
      {
        Xml.Elements(name).Remove();
        return;
      }

      //if existing, replace
      if (Xml.Elements(name).Count() > 0)
      {
        //remove all but first to replace
        Xml.Elements(name).Except(Xml.Elements(name).Take(1)).Remove();
        //replace first
        Xml.Elements(name).First().ReplaceWith(new XElement(name, t));
      }
      else //just add, nothing to replace
      {
        Xml.Add(new XElement(name, t));
      }
    }

    public T GetProperty<T>(XName name) where T : class
    {
      return Xml.Attribute(name) != null ? Xml.Attribute(name).Value as T : default(T);
    }
    public void SetProperty<T>(XName name, T t) where T : class
    {
      Xml.Attributes(name).Remove();
      if (t != null) Xml.Add(new XAttribute(name, t));
    }

    #region bool?
    public bool? GetBoolean(XName name)
    {
      return Xml.Element(name) != null ? boolVals.ContainsKey(Xml.Element(name).Value) ?
                      boolVals[Xml.Element(name).Value] : (bool?)Xml.Element(name) : null;
    }

    public void SetBoolean(XName name, bool? value, string trueVal, string falseVal)
    {
      SetValue(name, !value.HasValue ? null : value.Value ? trueVal : falseVal);
    }

    public void SetYesNoBoolean(XName name, bool? value)
    {
      SetBoolean(name, value, "yes", "no");
    }

    public void SetBoolean(XName name, bool? value)
    {
      SetBoolean(name, value, "true", "false");
    }

    public bool? GetBooleanProperty(XName name)
    {
      return Xml.Attribute(name) != null ? boolVals.ContainsKey(Xml.Attribute(name).Value) ?
                      boolVals[Xml.Attribute(name).Value] : (bool?)Xml.Attribute(name) : null;
    }

    public void SetBooleanProperty(XName name, bool? value, string trueVal, string falseVal)
    {
      Xml.Attributes(name).Remove();
      if (value.HasValue) Xml.Add(new XAttribute(name, value.Value ? trueVal : falseVal));
    }

    public void SetYesNoBooleanProperty(XName name, bool? value)
    {
      SetBooleanProperty(name, value, "yes", "no");
    }

    public void SetBooleanProperty(XName name, bool? value)
    {
      SetBooleanProperty(name, value, "true", "false");
    }
    #endregion

    #region bool
    public bool GetBooleanWithDefault(XName name)
    {
      return GetBooleanWithDefault(name, default(bool));
    }
    public virtual bool GetBooleanWithDefault(XName name, bool defaultVal)
    {
      return Xml.Element(name) != null ? boolVals.ContainsKey(Xml.Element(name).Value) ?
                      boolVals[Xml.Element(name).Value] : (bool)Xml.Element(name) : defaultVal;
    }
    public bool GetBooleanPropertyWithDefault(XName name)
    {
      return GetBooleanPropertyWithDefault(name, default(bool));
    }
    public bool GetBooleanPropertyWithDefault(XName name, bool defaultVal)
    {
      return Xml.Attribute(name) != null ? boolVals.ContainsKey(Xml.Attribute(name).Value) ?
                      boolVals[Xml.Attribute(name).Value] : (bool)Xml.Attribute(name) : defaultVal;
    }

    #endregion

    #region int

    public int GetInt32WithDefault(XName name, int defaultVal)
    {
      return (int?)Xml.Element(name) ?? defaultVal;
    }
    public int? GetInt32(XName name)
    {
      return (int?)Xml.Element(name);
    }
    public void SetInt32(XName name, int? val)
    {
      SetValue<string>(name, val.HasValue ? val.Value.ToString() : null);
    }


    public int GetInt32Property(XName name)
    {
      return Xml.Attribute(name) != null ? (int)Xml.Attribute(name) : 0;
    }
    public int GetInt32PropertyWithDefault(XName name, int defaultVal)
    {
      return Xml.Attribute(name) != null ? (int)Xml.Attribute(name) : defaultVal;
    }

    public void SetInt32Property(XName name, int? value)
    {
      Xml.Attributes(name).Remove();
      if (value.HasValue)
      {
        Xml.Add(new XAttribute(name, value.Value));
      }
    }


    #endregion

    /// <summary>
    /// Gets the URI (either relative or absolute).
    /// </summary>
    /// <param name="name">ELement Name.</param>
    /// <returns>Uri</returns>
    public Uri GetUriRelativeOrAbsolute(XName name)
    {
      // cache XElement
      XElement element = Xml.Element(name);

      if (element != null)
      {
        string escapedUri = Uri.EscapeUriString(element.Value);

        if (Uri.IsWellFormedUriString(escapedUri, UriKind.RelativeOrAbsolute))
        {
          if (Uri.IsWellFormedUriString(escapedUri, UriKind.Absolute))
            return new Uri(element.Value, UriKind.Absolute);
          return new Uri(element.Value, UriKind.Relative);
        }

        if (Uri.IsWellFormedUriString(Base + escapedUri, UriKind.Absolute))
        {
          return new Uri(Base + element.Value, UriKind.Absolute);
        }
      }
      return null;
    }

    public Uri GetUri(XName name)
    {
      // cache XElement
      XElement element = Xml.Element(name);

      if (element != null)
      {
        // BUGFIX: oising 2009/10/18
        // Need to escape Uri text because Uri.IsWellFormedUriString call
        // will not escape before test, yet Uri will do it on construction
        // which yields a valid absolute uri. Uris are escaped further up
        // the AtomSite stack in most cases; the exception is for BlogML import.
        string escapedUri = Uri.EscapeUriString(element.Value);

        if (Uri.IsWellFormedUriString(escapedUri, UriKind.Absolute))
        {
          return new Uri(element.Value);
        }
        if (Uri.IsWellFormedUriString(Base + escapedUri, UriKind.Absolute))
        {
          return new Uri(Base + element.Value);
        }
      }
      return null;
    }

    public void SetUri(XName name, Uri uri)
    {
      SetValue(name, uri);
    }

    public Uri GetUriProperty(XName name)
    {
      // see GetUri above (oising)

      // cache attribute  
      XAttribute attribute = Xml.Attribute(name);

      if (attribute != null)
      {
        string escapedUri = Uri.EscapeUriString(attribute.Value);

        if (Uri.IsWellFormedUriString(escapedUri, UriKind.Absolute))
        {
          return new Uri(attribute.Value);
        }

        if (Uri.IsWellFormedUriString(Base + escapedUri, UriKind.Absolute))
        {
          return new Uri(Base + attribute.Value);
        }

        // BUGFIX: need to test validity of relative uri here too, or else this
        // return becomes the default for this branch, instead of null (oising)
        if (Uri.IsWellFormedUriString(escapedUri, UriKind.Relative))
        {
          return new Uri(attribute.Value, UriKind.Relative);
        }
      }
      return null;
    }
    public void SetUriProperty(XName name, Uri uri)
    {
      Xml.Attributes(name).Remove(); if (uri != null) Xml.Add(new XAttribute(name, uri));
    }

    // Note: This is potentially buggy due to namespaces and prefixes.
    protected void WriteXml(string name, string ns, XmlWriter writer)
    {
      WriteXml(name, ns, null, writer);
    }

    // Note: This is potentially buggy due to namespaces and prefixes.
    protected void WriteXml(string name, string ns, Dictionary<string, string> prefixes, XmlWriter writer)
    {
      //write start if at start
      bool start = false;
      if (writer.WriteState == System.Xml.WriteState.Start)
      {
        start = true;
        writer.WriteStartDocument();
        writer.WriteStartElement(name, ns);
      }

      //TODO: normalize existing prefixes?  What if others already exist?

      //write prefixes if they don't already exist
      if (prefixes != null)
      {
        foreach (KeyValuePair<string, string> pair in prefixes)
        {
          string curPrefix = writer.LookupPrefix(pair.Value);
          if (string.IsNullOrEmpty(curPrefix) || curPrefix != pair.Key)
            writer.WriteAttributeString("xmlns", pair.Key, null, pair.Value);
        }
      }

      //write attributes of the xml
      foreach (XAttribute att in Xml.Attributes())
      {
        if (att.IsNamespaceDeclaration && att.Value == ns) continue;
        writer.WriteAttributeString(att.Name.LocalName, att.Name.NamespaceName, att.Value);
      }

      //write elements of the xml
      foreach (XNode el in Xml.Nodes())
      {
        el.WriteTo(writer);
      }

      //close
      if (start)
      {
        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }

    /// <summary>
    /// Gets or sets the underlying xml of the entry.
    /// </summary>
    public XElement Xml { get; set; }

    /// <summary>
    /// Gets the xml:base of the current element by walking up the tree or sets the xml:base on the current element.
    /// </summary>
    public Uri Base
    {
      get { return GetBase(); }
      set
      {
        if (value != null)
        {
          Xml.Attributes(XNamespace.Xml + "base").Remove();
          Xml.Add(new XAttribute(XNamespace.Xml + "base", value));
        }
      }
    }

    /// <summary>
    /// Gets the xml:lang of the current element by walking up the tree or sets the xml:lang on the current element.
    /// </summary>
    public string Lang
    {
      get { return GetLang(); }
      set { Xml.Attributes(XNamespace.Xml + "lang").Remove(); Xml.Add(new XAttribute(XNamespace.Xml + "lang", value)); }
    }

    public override bool Equals(object obj)
    {
      XmlBase other = obj as XmlBase;
      if (other == null) return false;
      return Xml.Value.Equals(other.Xml.Value);
    }

    public override int GetHashCode()
    {
      return Xml.GetHashCode();
    }

    Uri GetBase()
    {
      XElement curr = Xml;
      while (true)
      {
        if (curr.Attribute(XNamespace.Xml + "base") != null) return new Uri(curr.Attribute(XNamespace.Xml + "base").Value);
        if (curr.Parent == null) break;
        curr = curr.Parent;
      }
      return null;
    }

    string GetLang()
    {
      XElement curr = Xml;
      while (true)
      {
        if (curr.Attribute(XNamespace.Xml + "lang") != null) return (string)curr.Attribute(XNamespace.Xml + "lang");
        if (curr.Parent == null) break;
        curr = curr.Parent;
      }
      return null;
    }
  }
}
