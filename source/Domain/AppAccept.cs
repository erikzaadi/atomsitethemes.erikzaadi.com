using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AtomSite.Domain
{
  public class AppAccept : XmlBase
  {
    public AppAccept() : base(new XElement(Atom.AppNs + "accept")) { }
    public AppAccept(string accept) : this(accept, null) { }
    public AppAccept(string accept, string ext)
      : base(new XElement(Atom.AppNs + "accept"))
    {
      Value = accept;
      Ext = ext;
    }

    public string Value { get { return Xml.Value; } set { Xml.Value = value; } }
    public string Ext
    {
      get { return GetProperty<string>(Atom.SvcNs + "ext"); }
      set { SetProperty<string>(Atom.SvcNs + "ext", value); }
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }
      return ((AppAccept)obj).Value == this.Value;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    public static IEnumerable<AppAccept> Images
    {
      get
      {
        yield return new AppAccept("image/gif", "gif");
        yield return new AppAccept("image/jpeg", "jpg");
        yield return new AppAccept("image/png", "png");
        yield return new AppAccept("image/svg+xml", "svg");
        yield return new AppAccept("image/tiff", "tiff");
      }
    }
    
    public static IEnumerable<AppAccept> Audio
    {
      get
      {
        yield return new AppAccept("audio/mpeg", "mp3");
        yield return new AppAccept("audio/ogg", "ogg");
        yield return new AppAccept("audio/vorbis", "ogg");
        yield return new AppAccept("audio/vnd.rn-realaudio", "ra");
        yield return new AppAccept("audio/x-ms-wma", "wma");
        yield return new AppAccept("audio/x-wav", "wav");
      }
    }

    public static IEnumerable<AppAccept> Video
    {
      get
      {
        yield return new AppAccept("video/mp4", "mp4");
        yield return new AppAccept("video/mpeg", "mpg");
        yield return new AppAccept("video/ogg", "ogm");
        yield return new AppAccept("video/quicktime", "mov");
        yield return new AppAccept("video/x-ms-wmv", "wmv");
      }
    }

    public static IEnumerable<AppAccept> DocsAndText
    {
      get
      {
        yield return new AppAccept("application/javascript", "js");
        yield return new AppAccept("application/json", "json");
        yield return new AppAccept("application/msword", "doc");
        yield return new AppAccept("application/pdf", "pdf");
        yield return new AppAccept("application/vnd.ms-excel", "xls");
        yield return new AppAccept("application/vnd.ms-powerpoint", "msp");
        yield return new AppAccept("application/vnd.oasis.opendocument.text", "odt");
        yield return new AppAccept("application/vnd.oasis.opendocument.spreadsheet", "odt");
        yield return new AppAccept("application/vnd.oasis.opendocument.presentation", "odt");
        yield return new AppAccept("application/vnd.oasis.opendocument.graphics", "odt");
        yield return new AppAccept("application/zip", "zip");
        yield return new AppAccept("text/css", "css");
        yield return new AppAccept("text/csv", "csv");
        yield return new AppAccept("text/html", "html");
        yield return new AppAccept("text/plain", "txt");
        yield return new AppAccept("text/xml", "xml");
      }
    }

    public static IEnumerable<AppAccept> Media
    {
      get
      {
        foreach (var a in Images) yield return a;
        foreach (var a in Audio) yield return a;
        foreach (var a in Video) yield return a;
      }
    }

    public static readonly AppAccept ReadOnly = new AppAccept(string.Empty, null);
    public static readonly AppAccept Entries = new AppAccept(Atom.ContentTypeEntry, "atom"); //default entries

    public static IEnumerable<AppAccept> AllAccepts
    {
      get
      {
        yield return ReadOnly;
        yield return Entries;
        foreach (var a in Images) yield return a;
        foreach (var a in Audio) yield return a;
        foreach (var a in Video) yield return a;
        foreach (var a in DocsAndText) yield return a;
      }
    }
  }
}
