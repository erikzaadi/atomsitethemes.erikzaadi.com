/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using AtomSite.Domain;
    using AtomSite.Repository;

    public class WizardController : BaseController
    {
        protected IAppServiceRepository AppServiceRepository { get; private set; }
        protected IAtomEntryRepository AtomEntryRepository { get; private set; }
        protected IMediaRepository MediaRepository { get; private set; }
        protected IUserRepository UserRepository { get; private set; }
        protected IThemeService ThemeService { get; private set; }

        public WizardController(IAppServiceRepository svcRepo, IAtomEntryRepository entryRepo,
          IMediaRepository mediaRepo,
          IUserRepository userRepo, IThemeService theme, ILogService logger)
        {
            AppServiceRepository = svcRepo;
            MediaRepository = mediaRepo;
            AtomEntryRepository = entryRepo;
            UserRepository = userRepo;
            LogService = logger;
            ThemeService = theme;
        }

        public ActionResult CatchAll()
        {
            return RedirectToAction("TestInstall");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult TestInstall()
        {
            return View("WizardTestInstall", "Wizard", new PageModel());
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SetupChoice()
        {
            return View("WizardSetupChoice", "Wizard", new PageModel());
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult BasicSettings()
        {
            int total;
            string email = string.Empty;
            string name = string.Empty;
            var e = UserRepository.GetUsers(0, int.MaxValue, out total).LastOrDefault();
            if (e != null)
            {
                email = e.Email;
                name = e.Name;
            }
            var m = new WizardBasicSettingsModel()
            {
                Address = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath,
                Owner = AppService.GetWorkspace().GetCollection().Id.Owner,
                Year = DateTime.UtcNow.Year,
                WorkspaceSubtitle = AppService.GetWorkspace().Subtitle.Text,
                BlogTitle = AppService.GetWorkspace().GetCollection().Title.Text,
                Name = name,
                Email = email
            };

            return View("WizardBasicSettings", "Wizard", m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BasicSettings(WizardBasicSettingsModel m)
        {
            try
            {
                //validate
                if (string.IsNullOrEmpty(m.Owner) || m.Owner.Trim().Length == 0)
                    ModelState.AddModelError("owner", "Owner is required.");

                if (m.Year < 1990 || m.Year > 2090)
                    ModelState.AddModelError("year", "Please choose a valid year.");

                if (string.IsNullOrEmpty(m.WorkspaceSubtitle) || m.WorkspaceSubtitle.Trim().Length == 0)
                    ModelState.AddModelError("workspaceSubtitle", "The workspace subtitle is required.");

                if (string.IsNullOrEmpty(m.BlogTitle) || m.BlogTitle.Trim().Length == 0)
                    ModelState.AddModelError("blogTitle", "The blog title is required.");

                if (string.IsNullOrEmpty(m.Name) || m.Name.Trim().Length == 0)
                    ModelState.AddModelError("name", "Your name is required.");

                if (string.IsNullOrEmpty(m.Email) || m.Email.Trim().Length == 0)
                    ModelState.AddModelError("email", "Your email is required."); //TODO: proper email validation

                if (string.IsNullOrEmpty(m.Password) || m.Password.Trim().Length == 0)
                    ModelState.AddModelError("password", "Please choose a password.");

                if (ModelState.IsValid)
                {
                    UpdateBasicSettingsAndAllExistingData(m);
                    return RedirectToAction("ThemeChoice");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
                LogService.Error(ex);
            }
            return View("WizardBasicSettings", "Wizard", m);
        }

        private void UpdateBasicSettingsAndAllExistingData(WizardBasicSettingsModel m)
        {
            AppService appSvc = this.AppService;
            //update service config, user config, and pages
            appSvc.Base = new Uri(Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath);
            if (!appSvc.Base.ToString().EndsWith("/")) appSvc.Base = new Uri(appSvc.Base.ToString() + "/");
            appSvc.Admins = new[] { m.Name };
            appSvc.Workspaces.First().Title = new AtomText() { Text = m.Owner };
            appSvc.Workspaces.First().Subtitle = new AtomText() { Text = m.WorkspaceSubtitle };

            LogService.Info("Removing old user 'Admin'");
            UserRepository.DeleteUser("Admin");
            LogService.Info("Removing old user '{0}'", m.Name);
            UserRepository.DeleteUser(m.Name);
            User user = new User()
            {
                Name = m.Name,
                Email = m.Email,
                Ids = new[] { m.Name },
                Uri = appSvc.Base,
                Password = m.Password,
                PasswordFormat = "clear"
            };
            LogService.Info("Creating new user '{0}'", m.Name);
            UserRepository.CreateUser(user);

            foreach (string c in new[] { "blog", "pages", "media" })
            {
                LogService.Info("Updating collection '{0}' with new id and author information.", c);
                AppCollection coll = appSvc.GetWorkspace().GetCollection(c);

                //update blog title
                if (c == "blog") coll.Title = new AtomText() { Text = m.BlogTitle };

                //update include ids
                foreach (XElement xid in appSvc.Xml.Descendants(Atom.SvcNs + "id"))
                {
                    Id id = new Uri(((string)xid));
                    Id newId = new Id(m.Owner, m.Year.ToString(), c, id.EntryPath);
                    if (id.Collection == c) xid.SetValue(newId);
                    LogService.Info("Updating include id from {0} to {1}", id, newId);
                }

                //get entries to update later
                int total;
                IList<AtomEntry> entries = AtomEntryRepository.GetEntries(new EntryCriteria()
                {
                    WorkspaceName = coll.Id.Workspace,
                    CollectionName = coll.Id.Collection,
                    Authorized = true
                }, 0, int.MaxValue, out total).ToList();

                //get annotations to update later
                IList<AtomEntry> annotations = AtomEntryRepository.GetEntries(new EntryCriteria()
                {
                    WorkspaceName = coll.Id.Workspace,
                    CollectionName = coll.Id.Collection,
                    Annotations = true,
                    Deep = false,
                    Authorized = true
                }, 0, int.MaxValue, out total).ToList();

                //must process annotations before entries
                entries = annotations.Concat(entries).ToList();

                //update collection id
                Id newCollId = new Id(m.Owner, m.Year.ToString(), c);
                LogService.Info("Updating collection id from {0} to {1}", coll.Id, newCollId);
                coll.Id = newCollId;
                bool newWorkspace = coll.Id.Workspace != Atom.DefaultWorkspaceName;

                //save all with new id
                foreach (AtomEntry entry in entries)
                {
                    try
                    {
                        //update author based on email
                        if (entry.Authors.First().Email != null && 
              (entry.Authors.First().Email.ToLower() == user.Email.ToLower() || entry.Authors.First().Email == "admin@example.com"))
                        {
                            entry.Authors = new[] { user.ToAtomAuthor() };
                        }
                        //entry.Published = DateTimeOffset.Now;
                        //entry.Updated = DateTimeOffset.Now;
                        entry.Edited = DateTimeOffset.Now;

                        if (entry.Id.EntryPath == "About")
                        {
                            LogService.Info("Updating name on About page from Admin to {0}", m.Name);
                            entry.Content.Text = entry.Content.Text.Replace(" Admin ", string.Format(" {0} ", m.Name));
                        }

                        if (entry.Id.EntryPath == "Contact")
                        {
                            LogService.Info("Updating email on Contact page from admin@example.com to {0}", m.Name);
                            entry.Content.Text = entry.Content.Text.Replace("admin@example.com", m.Name);
                        }

                        //fix links http://localhost:1333 to new address
                        string xmlstr = entry.Xml.ToString();
                        xmlstr = xmlstr.Replace("http://localhost:1333/", appSvc.Base.ToString());
                        entry.Xml = XElement.Parse(xmlstr);

                        //entry.Content.Xml.DescendantNodesAndSelf().Where(n => n.NodeType == System.Xml.XmlNodeType.Attribute)
                        //  .Cast<XAttribute>()
                        //  .Where(a => a.Value.Contains("http://localhost:1333/")).ToList()
                        //  .ForEach(a => a.Value = a.Value.Replace("http://localhost:1333/", appSvc.Base.ToString()));

                        Id oldId = entry.Id;

                        if (newWorkspace)
                        {
                            LogService.Info("Workspace name is changed to {0}", appSvc.Workspaces.First().Name);
                            appSvc.Workspaces.First().Name = coll.Id.Workspace;
                            if (!entry.Media)
                            {
                                if (coll.Dated)
                                    entry.Id = new Id(m.Owner, entry.Id.Date, c, entry.Id.EntryPath);
                                else
                                    entry.Id = new Id(m.Owner, m.Year.ToString(), c, entry.Id.EntryPath);
                                LogService.Info("Creating entry with id from {0} to {1}", oldId, entry.Id);
                                AtomEntryRepository.CreateEntry(entry);
                                //delte old entry
                                AtomEntryRepository.DeleteEntry(oldId);
                            }
                            else
                            {
                                using (Stream s = MediaRepository.GetMedia(entry))
                                {
                                    if (coll.Dated)
                                        entry.Id = new Id(m.Owner, entry.Id.Date, c, entry.Id.EntryPath);
                                    else
                                        entry.Id = new Id(m.Owner, m.Year.ToString(), c, entry.Id.EntryPath);
                                    LogService.Info("Creating media with id from {0} to {1}", oldId, entry.Id);
                                    MediaRepository.CreateMedia(entry, s);
                                }
                                //delete old media
                                entry.Id = oldId;
                                MediaRepository.DeleteMedia(entry);
                            }
                        }
                        else
                        {
                            //just update
                            if (coll.Dated)
                                entry.Id = new Id(m.Owner, entry.Id.Date, c, entry.Id.EntryPath);
                            else
                                entry.Id = new Id(m.Owner, m.Year.ToString(), c, entry.Id.EntryPath);
                            LogService.Info("Updating entry id from {0} to {1}", oldId, entry.Id);
                            AtomEntryRepository.UpdateEntry(entry);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex);
                    }
                }
            }

            appSvc = AppServiceRepository.UpdateService(appSvc);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ThemeChoice()
        {
            return View("WizardThemeChoice", "Wizard",
              new WizardThemeChoiceModel()
              {
                  Theme = ThemeService.GetInheritedThemeName(Scope.EntireSite),
                  Themes = ThemeService.GetInstalledThemes().Select(t => ThemeService.GetTheme(t))
              });
        }

        /// <summary>
        /// Handle's the wizard choose theme postback
        /// TODO: move the logic to a service?
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThemeChoice(WizardThemeChoiceModel m)
        {
            if (ModelState.IsValid)
            {
                //update service config
                AppService appSvc = AppService;
                if (ThemeService.GetThemeName(Scope.EntireSite) != m.SelectedTheme)
                {
                    appSvc.Theme = m.SelectedTheme;
                }

                //disable the wizard plugin since it is finished
                appSvc.Plugins.Where(p => p.Type == typeof(WizardPlugin).AssemblyQualifiedName)
                  .Single().Status = PluginStatus.Disabled;

                AppServiceRepository.UpdateService(appSvc);

                ServerApp.Restart();

                return RedirectToAction("Complete");
            }
            return View("WizardThemeChoice", "Wizard", m);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Complete()
        {
            return View("WizardComplete", "Wizard", new PageModel());
        }


        /// <summary>
        /// Allows tests for various verbs.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST", "PUT", "DELETE")]
        public ActionResult Test()
        {
            LogService.Info("Successfully tested verb {0}", Request.HttpMethod);
            System.Threading.Thread.Sleep(300); //don't go too fast that user can't see it working
            return success;
        }

        /// <summary>
        /// Tests writing to the App_Data folder
        /// </summary>
        /// <returns></returns>
        public ActionResult WriteTest()
        {
            try
            {
                string path = Server.MapPath("~/App_Data/test.txt");
                System.IO.File.WriteAllText(path, "Test");
                LogService.Info("Successfully created a test file {0}", path);
                System.Threading.Thread.Sleep(300);
                return success;
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
                return failure;
            }
        }

        /// <summary>
        /// Tests deleting from the App_Data folder
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteTest()
        {
            try
            {
                System.IO.File.Delete(Server.MapPath("~/App_Data/test.txt"));
                LogService.Info("Successfully deleted the test file");
                return success;
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
                return failure;
            }
        }

        /// <summary>
        /// Tests if the current connection is secure
        /// </summary>
        /// <returns></returns>
        public ActionResult SecureTest()
        {
            if (Request.IsSecureConnection)
                return success;
            else
                return failure;
        }

        ActionResult success = new HttpResult() { StatusCode = System.Net.HttpStatusCode.OK, Content = "success" };
        ActionResult failure = new HttpResult() { StatusCode = System.Net.HttpStatusCode.Forbidden, Content = "failure" };
    }
}
