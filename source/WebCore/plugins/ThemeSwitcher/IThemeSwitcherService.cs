using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AtomSite.Domain;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Linq;

namespace AtomSite.WebCore.plugins.ThemeSwitcher
{
    public interface IThemeSwitcherService
    {
        IThemeService ThemeService { get; set; }
        void ChangeTheme(string ThemeName, string PageTemplate, string PageWidth, string Customizations, HttpContextBase Context);
        ThemeSwitcherViewModel GetModel(HttpContextBase Context, Scope scope);
        string Download(string ThemeName);
        ThemeModel GetThemeModel(string ThemeName);
    }

    public class ThemeSwitcherService : IThemeSwitcherService
    {
        public ThemeSwitcherService(IThemeService themeService)
        {
            ThemeService = themeService;
        }

        #region IThemeSwitcherService Members

        public IThemeService ThemeService { get; set; }

        public void ChangeTheme(string ThemeName, string PageTemplate, string PageWidth, string Customizations, HttpContextBase Context)
        {
            var theme = ThemeService.GetTheme(ThemeName);

            if (!string.IsNullOrEmpty(PageTemplate))
            {
                if (!theme.Templates.Contains(PageTemplate))
                {
                    throw new Exception("PageTemplate Invalid");
                }
                else
                {
                    Context.Session["PageTemplate"] = PageTemplate;
                }
            }
            else
            {
                Context.Session["PageTemplate"] = null;
            }
            if (!string.IsNullOrEmpty(PageWidth))
            {
                if (!theme.Widths.Contains(PageWidth))
                {
                    throw new Exception("PageWidth Invalid");
                }
                else
                {
                    Context.Session["PageWidth"] = PageWidth;
                }
            }
            else
            {
                Context.Session["PageWidth"] = null;
            }

            if (!string.IsNullOrEmpty(Customizations))
            {
                var customizations = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(Customizations));

                Context.Session["Customizations"] = customizations;
            }
            else
            {
                Context.Session["Customizations"] = null;
            }


            Context.Session["theme"] = ThemeName;
        }

        public ThemeSwitcherViewModel GetModel(HttpContextBase Context, Scope scope)
        {
            var themeNames = ThemeService.GetInstalledThemes();

            var currentTheme = (Context.Session != null && Context.Session["theme"] != null) ?
                                                                                 Context.Session["theme"].ToString() : ThemeService.GetInheritedThemeName(scope);

            var currentPageWidth = Context.Session != null && Context.Session["PageWidth"] != null ?
                                                                                       Context.Session["PageWidth"].ToString() : string.Empty;

            var currentPageTemplate = Context.Session != null && Context.Session["PageTemplate"] != null ?
                                                                                             Context.Session["PageTemplate"].ToString() : string.Empty;

            var themes = new List<Theme>(themeNames.Select(p => ThemeService.GetTheme(p)));

            var model = new ThemeSwitcherViewModel
            {
                ThemeNames = new SelectList(themes.Select(p => p.Name), currentTheme),
                Themes = new List<ThemeSwitcherThemeModel>(
                    themes.Select(theme =>
                                  new ThemeSwitcherThemeModel
                                  {
                                      ThemeName = theme.Name,
                                      Selected = theme.Name == currentTheme,
                                      PageTemplates = new SelectList(
                                          theme.Templates,
                                          currentTheme == theme.Name ? currentPageTemplate : string.Empty),
                                      PageWidths = new SelectList(
                                          theme.Widths,
                                          currentTheme == theme.Name ? currentPageWidth : string.Empty),
                                      Description = theme.Text.Text,
                                      Author = string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", theme.Authors.First().Uri, theme.Authors.First().Name),
                                      Downloaded = GetDownloadCount(theme.Name),
                                      Customizations = GetCustomizations(theme)
                                  }))
            };

            return model;
        }

        private IList<ThemeCustomization> GetCustomizations(Theme theme)
        {
            var toReturn = new List<ThemeCustomization>();

            var customizationNames = theme.GetValue<string>(XName.Get("customizations", Atom.ThemeNs.NamespaceName));
            if (!string.IsNullOrEmpty(customizationNames))
            {

                customizationNames.Split(new char[] { ',' })
                    .ToList()
                    .ForEach(customization =>
                            {
                                var trimmedCustom =
                                    customization.Trim();
                                toReturn.Add(

                                    new ThemeCustomization
                                        {
                                            Name = trimmedCustom,
                                            Description =
                                                theme.GetValue
                                                <string>(XName.Get(trimmedCustom +
                                                    "description", Atom.ThemeNs.NamespaceName)),
                                            Options = new SelectList(theme.GetValue<string>(XName.Get(trimmedCustom +
                                                    "values", Atom.ThemeNs.NamespaceName)).Split(new char[] { ',' }))
                                        }
                                    );

                            });
            }
            return toReturn;
        }

        public class ThemeZipFileInfo
        {
            public string PhysicalPath { get; set; }
            public string ZipPath { get; set; }
        }

        public string Download(string ThemeName)
        {
            var theme = ThemeService.GetTheme(ThemeName);
            List<ThemeZipFileInfo> filenames = new List<ThemeZipFileInfo>();

            foreach (var dir in GetPaths(ThemeName))
            {
                if (Directory.Exists(dir))
                {
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        filenames.Add(new ThemeZipFileInfo
                        {
                            PhysicalPath = file,
                            ZipPath = file.Replace(HostingEnvironment.ApplicationPhysicalPath, "")
                        });
                    }
                }
            }

            filenames.Add(new ThemeZipFileInfo
            {
                PhysicalPath = ThemeService.GetThemePath(ThemeName),
                ZipPath = Path.GetFileName(ThemeService.GetThemePath(ThemeName))
            });

            var toReturn = Path.GetTempFileName();

            byte[] buffer = new byte[4096];
            using (ZipOutputStream s = new ZipOutputStream(File.Create(toReturn)))
            {
                s.SetLevel(9); // 0 - store only to 9 - means best compression

                foreach (var file in filenames)
                {
                    var entry = new ZipEntry(file.ZipPath);
                    s.PutNextEntry(entry);

                    using (FileStream fs = File.OpenRead(file.PhysicalPath))
                    {
                        StreamUtils.Copy(fs, s, buffer);
                    }
                }

                var readme = string.Format("Downloaded from http://atomsitethemes.erikzaadi.com {0}.{1}Enjoy the theme, and do come back for more!",
                    DateTime.Now.ToShortDateString(),
                    Environment.NewLine);

                var readmeEntry = new ZipEntry("ReadMe.txt");
                s.PutNextEntry(readmeEntry);

                StreamUtils.Copy(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(readme)), s, buffer);
            }

            IncreaseDownloadCount(ThemeName);

            return toReturn;
        }

        public ThemeModel GetThemeModel(string ThemeName)
        {
            return new ThemeModel
            {
                Theme = ThemeService.GetTheme(ThemeName),
                Downloaded = GetDownloadCount(ThemeName)
            };
        }

        #endregion

        IEnumerable<string> GetPaths(string themeName)
        {
            yield return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, string.Format(@"css\{0}", themeName));
            yield return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, string.Format(@"img\{0}", themeName));
            yield return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, string.Format(@"js\{0}", themeName));
            yield return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, string.Format(@"themes\{0}", themeName));
        }

        public void IncreaseDownloadCount(string themeName)
        {
            var path = ThemeDownloadCounterPath(themeName);

            try
            {
                var current = GetDownloadCount(themeName);
                File.WriteAllText(path, (++current).ToString());
            }
            catch { }
        }

        public int GetDownloadCount(string themeName)
        {
            var path = ThemeDownloadCounterPath(themeName);
            if (!File.Exists(path))
                return 0;
            string content = File.ReadAllText(path);

            int temp = -1;

            return int.TryParse(content, out temp) && temp > 0 ? temp : 0;
        }

        private string ThemeDownloadCounterPath(string themeName)
        {
            return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, string.Format(@"App_Data\ThemeCounter-{0}.txt", themeName));
        }
    }
}