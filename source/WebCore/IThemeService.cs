/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Routing;
    using System.Xml.Linq;
    using AtomSite.Domain;
    using AtomSite.Repository;
    using StructureMap;
    using System;
    using System.Linq;
    using ICSharpCode.SharpZipLib.Zip;
    using System.Xml;

    public interface IThemeService
    {
        IEnumerable<string> GetInstalledThemes();
        string GetThemePath(string themeName);
        Theme GetTheme(string themeName);
        string GetThemeName(Scope scope);
        string GetInheritedThemeName(Scope scope);
        void SetTheme(Scope scope, string themeName);
        void DeleteTheme(string themeName);
        void CloneTheme(string themeName, string newThemeName);
        string InstallTheme(Stream themeStream, string filename, string contentType);
    }

    public class ThemeService : IThemeService
    {
        protected IAppServiceRepository AppServiceRepository { get; private set; }
        protected ILogService LogService { get; private set; }
        protected string ThemePath { get; private set; }
        protected string AppPath { get; private set; }

        public ThemeService(IAppServiceRepository svcRepo, string appPath, ILogService logger)
        {
            AppServiceRepository = svcRepo;
            LogService = logger;
            AppPath = appPath;
            ThemePath = Path.Combine(appPath, "themes");
        }

        public IEnumerable<string> GetInstalledThemes()
        {
            foreach (string folder in Directory.GetDirectories(ThemePath))
            {
                if (File.Exists(Path.Combine(folder, Path.GetFileName(folder) + ".xml")))
                    yield return Path.GetFileName(folder);
            }
            yield break;
        }

        public string GetThemePath(string themeName)
        {
            string path = Path.Combine(Path.Combine(ThemePath, themeName), themeName + ".xml");
            return path;
        }

        public Theme GetTheme(string themeName)
        {
            string path = GetThemePath(themeName);
            Theme theme = new Theme() { Xml = XElement.Load(path) };
            return theme;
        }

        /// <summary>
        /// Gets the theme name for the given scope.  Does not return inherited value.
        /// </summary>
        public string GetThemeName(Scope scope)
        {
            AppService svc = AppServiceRepository.GetService();
            if (scope.IsCollection) return svc.GetCollection(scope.Workspace, scope.Collection).Theme;
            else if (scope.IsWorkspace) return svc.GetWorkspace(scope.Workspace).Theme;
            else return svc.Theme;
        }

        /// <summary>
        /// Gets the theme name by using fallback algrorithm of inheritance.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public string GetInheritedThemeName(Scope scope)
        {
            AppService svc = AppServiceRepository.GetService();
            string theme = null;
            if (scope.IsCollection) theme = svc.GetCollection(scope.Workspace, scope.Collection).Theme;
            if (theme == null && (scope.IsCollection || scope.IsWorkspace)) theme = svc.GetWorkspace(scope.Workspace).Theme;
            if (theme == null) theme = svc.Theme;
            if (theme == null) theme = "default";
            return theme;
        }

        public static IThemeService GetCurrent(RequestContext ctx)
        {
            return ((IContainer)ctx.HttpContext.Application["Container"]).GetInstance<IThemeService>();
        }

        public void DeleteTheme(string themeName)
        {
            //TODO: auth?
            LogService.Info("Deleting theme {0}", themeName);

            if (themeName == "default") throw new Exception("You can't delete the default theme.");
            if (IsThemeInUse(themeName)) throw new Exception("You can't delete the theme because it is in use.  Please change to a different theme.");

            //delete css
            LogService.Info("Deleting theme css folder");
            string path = Path.Combine(AppPath, "css\\" + themeName);
            if (System.IO.Directory.Exists(path)) System.IO.Directory.Delete(path, true);
            //delete img
            LogService.Info("Deleting theme img folder");
            path = Path.Combine(AppPath, "img\\" + themeName);
            if (System.IO.Directory.Exists(path)) System.IO.Directory.Delete(path, true);
            //delete js
            LogService.Info("Deleting theme js folder");
            path = Path.Combine(AppPath, "js\\" + themeName);
            if (System.IO.Directory.Exists(path)) System.IO.Directory.Delete(path, true);
            //delete views
            LogService.Info("Deleting theme themes folder");
            path = Path.Combine(ThemePath, themeName); //it should exist
            if (System.IO.Directory.Exists(path)) System.IO.Directory.Delete(path, true);

        }

        public void CloneTheme(string themeName, string newThemeName)
        {
            //TODO: auth?

            if (GetInstalledThemes().Contains(newThemeName)) throw new Exception("Can't clone theme to new theme name because a theme already exists with that name.");

            //copy css
            LogService.Info("Cloning theme css folder");
            string path = Path.Combine(AppPath, "css\\" + themeName);
            if (System.IO.Directory.Exists(path)) CopyFilesRecursively(new DirectoryInfo(path), new DirectoryInfo(Path.Combine(AppPath, "css\\" + themeName)));
            //copy img
            LogService.Info("Cloning theme img folder");
            path = Path.Combine(AppPath, "img\\" + themeName);
            if (System.IO.Directory.Exists(path)) CopyFilesRecursively(new DirectoryInfo(path), new DirectoryInfo(Path.Combine(AppPath, "img\\" + themeName)));
            //copy js
            LogService.Info("Cloning theme js folder");
            path = Path.Combine(AppPath, "js\\" + themeName);
            if (System.IO.Directory.Exists(path)) CopyFilesRecursively(new DirectoryInfo(path), new DirectoryInfo(Path.Combine(AppPath, "js\\" + themeName)));
            //copy views
            LogService.Info("Cloning theme themes folder");
            path = Path.Combine(ThemePath, themeName);
            if (System.IO.Directory.Exists(path)) CopyFilesRecursively(new DirectoryInfo(path), new DirectoryInfo(Path.Combine(AppPath, "themes\\" + themeName)));

        }

        static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        protected bool IsThemeInUse(string theme)
        {
            AppService svc = AppServiceRepository.GetService();

            if (svc.Theme == theme) return true;
            foreach (AppWorkspace w in svc.Workspaces)
            {
                if (w.Theme == theme) return true;
                foreach (AppCollection c in w.Collections) if (c.Theme == theme) return true;
            }
            return false;
        }

        public void SetTheme(Scope scope, string themeName)
        {
            //TODO: auth?
            AppService svc = AppServiceRepository.GetService();

            if (scope.IsEntireSite)
            {
                //for top level, when set to default, set to null since it is default
                if (themeName == "default") themeName = null;
                svc.Theme = themeName;
            }
            else if (scope.IsWorkspace) svc.GetWorkspace(scope.Workspace).Theme = themeName;
            else if (scope.IsCollection) svc.GetCollection(scope.Workspace, scope.Collection).Theme = themeName;

            AppServiceRepository.UpdateService(svc);
        }

        public string InstallTheme(Stream themeStream, string filename, string contentType)
        {
            if (!(contentType == "application/zip" || contentType == "application/x-zip-compressed" || contentType == "application/octet-stream"))
                throw new InvalidContentTypeException(contentType);

            string themeName = null;

            // read theme manifest and make sure compatible
            ZipInputStream s = new ZipInputStream(themeStream);
            ZipEntry e;
            Theme theme = null;

            // first find manifest
            while ((e = s.GetNextEntry()) != null)
            {
                if (e.Name.EndsWith(".xml"))
                {
                    //we found manifest
                    theme = new Theme(XDocument.Load(new XmlTextReader(s)).Root);
                    if (theme.Id != null) themeName = theme.Name;
                    //TODO: check compatability etc.
                    break;
                }
            }

            if (theme == null) throw new Exception("Could not found theme manifest file in zip file.");

            string[] paths = GetPaths(themeName).ToArray();

            //reopen stream for re-reading
            themeStream.Seek(0, SeekOrigin.Begin);
            s = new ZipInputStream(themeStream);

            while ((e = s.GetNextEntry()) != null)
            {
                string path = Path.GetDirectoryName(Path.Combine(AppPath, e.Name));

                // make sure valid dir
                if (!paths.Contains(path))
                {
                    LogService.Warn("The current path is not an approved theme path.");
                    continue;
                }

                if (e.IsDirectory)
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                }
                else
                {
                    //write the data to disk
                    using (FileStream fs = File.Create(Path.Combine(AppPath, e.Name)))
                    {
                        byte[] buffer = new byte[1024];
                        int read = buffer.Length;
                        while (true)
                        {
                            read = s.Read(buffer, 0, buffer.Length);
                            if (read > 0) fs.Write(buffer, 0, read);
                            else break;
                        }
                    }
                }
            }
            return themeName;
        }

        IEnumerable<string> GetPaths(string themeName)
        {
            yield return Path.Combine(AppPath, string.Format(@"css\{0}", themeName));
            yield return Path.Combine(AppPath, string.Format(@"img\{0}", themeName));
            yield return Path.Combine(AppPath, string.Format(@"js\{0}", themeName));
            yield return Path.Combine(AppPath, string.Format(@"themes\{0}", themeName));
        }
    }
}
