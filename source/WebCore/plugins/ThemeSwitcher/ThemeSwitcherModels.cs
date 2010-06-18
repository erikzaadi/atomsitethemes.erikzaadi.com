using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Xml.Linq;
using AtomSite.Domain;

namespace AtomSite.WebCore.plugins.ThemeSwitcher
{
    public class ThemeSwitcherViewModel
    {
        public bool HasMessage { get { return !string.IsNullOrEmpty(Message); } }
        public string Message { get; set; }
        public bool IsMessageError { get; set; }
        public SelectList ThemeNames { get; set; }
        public IList<ThemeSwitcherThemeModel> Themes { get; set; }
    }

    public class ThemeSwitcherThemeModel
    {
        public bool Selected { get; set; }
        public SelectList PageWidths { get; set; }
        public SelectList PageTemplates { get; set; }
        public string ThemeName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Downloaded { get; set; }
        public IList<ThemeCustomization> Customizations { get; set; }
    }

    public class ThemeCustomization
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SelectList Options { get; set; }
    }

    public class ThemeSwitcherInputModel
    {
        public string ThemeName { get; set; }
        public string PageWidth { get; set; }
        public string PageTemplate { get; set; }
        public string Customizations { get; set; }
    }

    public class ThemeModel
    {
        public Theme Theme { get; set; }
        public int Downloaded { get; set; }
        public string LivePreviewBaseUrl { get { return "http://atomsitethemes.erikzaadi.com/ThemeSwitcher/ChangeTheme"; } }
        public string LivePreviewUrl
        {
            get
            {
                return string.Format(LivePreviewBaseUrl + "?ThemeName={0}&PageWidth={1}&PageTemplate={2}", Theme.Name,
                                     Theme.Widths.FirstOrDefault(), Theme.Templates.FirstOrDefault());
            }
        }
    }

    public class ThemeInclude : Include
    {
        public ThemeInclude() : base(new XElement(Include.IncludeXName)) { }

        public ThemeInclude(Include include)
            : base(include.Xml) { }

        public ThemeInclude(string themename)
            : base(new XElement(Atom.SvcNs + "theme"))
        {
            this.ThemeName = themename;
        }

        public string ThemeName
        {
            get { return GetProperty<string>("themename"); }
            set { SetProperty<string>("themename", value); }
        }
    }
}