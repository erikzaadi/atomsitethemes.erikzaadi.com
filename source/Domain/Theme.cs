using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Collections.Specialized;

namespace AtomSite.Domain
{
    public class Theme : AtomEntry
    {

        public Theme() : base(new XElement(Atom.PluginNs + "plugin")) { }
        public Theme(XElement xml) : base(xml) { }
        /*
      <theme:homepage>http://atomsite.net</theme:homepage>
      <theme:screenshot>http://atomsite.net/media/BlueTheme.png</theme:screenshot>
      <theme:media>screen</theme:media>
      <theme:layout>2 column</theme:layout>
      <theme:width>doc, doc2, doc3, doc4, doc-custom</theme:width>
      <theme:template>yui-t1, yui-t2, yui-t3, yui-t4, yui-t5, yui-t6</theme:template>
      <theme:version>4</theme:version>
         */
        public string Name { get { return Id.EntryPath; } }

        public Uri Homepage
        {
            get { return GetUri(Atom.ThemeNs + "homepage"); }
            set { SetUri(Atom.ThemeNs + "homepage", value); }
        }
        public Uri Thumbnail
        {
            get { return GetUriRelativeOrAbsolute(Atom.ThemeNs + "thumbnail"); }
            set { SetUri(Atom.ThemeNs + "thumbnail", value); }
        }
        public Uri Screenshot
        {
            get { return GetUriRelativeOrAbsolute(Atom.ThemeNs + "screenshot"); }
            set { SetUri(Atom.ThemeNs + "screenshot", value); }
        }
        public string TargetMedia
        {
            get { return GetValue<string>(Atom.ThemeNs + "media"); }
            set { SetValue<string>(Atom.ThemeNs + "media", value); }
        }
        public string Layout
        {
            get { return GetValue<string>(Atom.ThemeNs + "layout"); }
            set { SetValue<string>(Atom.ThemeNs + "layout", value); }
        }
        public string Version
        {
            get { return GetValue<string>(Atom.ThemeNs + "version"); }
            set { SetValue<string>(Atom.ThemeNs + "version", value); }
        }
        public int Rank
        {
            get { return GetInt32WithDefault(Atom.ThemeNs + "rank", 0); }
            set { SetInt32(Atom.ThemeNs + "rank", value); }
        }
        public int RankTotal
        {
            get { return GetInt32WithDefault(Atom.ThemeNs + "rankTotal", 0); }
            set { SetInt32(Atom.ThemeNs + "rankTotal", value); }
        }

        public int Downloads
        {
            get { return GetInt32WithDefault(Atom.ThemeNs + "downloads", 0); }
            set { SetInt32(Atom.ThemeNs + "downloads", value); }
        }

        public IEnumerable<string> CompatibleVersions
        {
            get { return GetValues<string>(Atom.ThemeNs + "compatibleVersion"); }
            set { SetValues<string>(Atom.ThemeNs + "compatibleVersion", value); }
        }

        public IEnumerable<string> Widths
        {
            get { return GetValue<string>(Atom.ThemeNs + "width").Split(',').Select(s => s.Trim()); }
            set { SetValue<string>(Atom.ThemeNs + "width", string.Join(", ", value.ToArray())); }
        }
        public IEnumerable<string> Templates
        {
            get { return GetValue<string>(Atom.ThemeNs + "template").Split(',').Select(s => s.Trim()); }
            set { SetValue<string>(Atom.ThemeNs + "template", string.Join(", ", value.ToArray())); }
        }

        public override bool GetBooleanWithDefault(XName name, bool defaultVal)
        {
            var custom = GetPossibleCustomizedBooleanValueFromSession(name.LocalName);

            return custom.HasValue ? custom.Value : base.GetBooleanWithDefault(name, defaultVal);
        }

        public override T GetValue<T>(XName name)
        {
            return GetPossibleCustomizedValueFromSession<T>(name.LocalName) ?? base.GetValue<T>(name);
        }

        private T GetPossibleCustomizedValueFromSession<T>(string Property) where T : class
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Session != null
                && HttpContext.Current.Session["Customizations"] != null)
            {
                var customizations = HttpContext.Current.Session["Customizations"] as NameValueCollection;
                return customizations.AllKeys.Contains(Property) ? customizations[Property] as T : null;
            }
            return null;
        }

        private bool? GetPossibleCustomizedBooleanValueFromSession(string Property)
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Session != null
                && HttpContext.Current.Session["Customizations"] != null)
            {
                var customizations = HttpContext.Current.Session["Customizations"] as NameValueCollection;
                return customizations.AllKeys.Contains(Property) ? bool.Parse(customizations[Property]) : null as bool?;
            }
            return null;
        }
    }
}
