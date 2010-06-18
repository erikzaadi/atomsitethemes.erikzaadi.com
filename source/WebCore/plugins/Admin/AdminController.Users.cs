/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using AtomSite.Domain;

    public partial class AdminController : BaseController
    {
        const int DEFAULT_PAGE_SIZE = 20;

        [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ViewResult Users(int? page, int? pageSize, string filter)
        {
            AdminUsersModel m = new AdminUsersModel();
            int pageIdx = (page ?? 1) - 1;
            m.Filter = filter;
            m.AdminsCount = AppService.Admins.Count();
            m.AuthorsCount = AppService.Workspaces.SelectMany(a => a.Authors).Count() +
                AppService.Workspaces.SelectMany(c => c.Collections).SelectMany(a => a.Authors).Count();
            m.ContribsCount = AppService.Workspaces.SelectMany(a => a.Contributors).Count() +
                AppService.Workspaces.SelectMany(c => c.Collections).SelectMany(a => a.Contributors).Count();

            //get total TODO: fix inefficient when filter
            int total;
            m.Users = UserRepository.GetUsers(pageIdx, pageSize ?? DEFAULT_PAGE_SIZE, out total)
                .ToPagedList(pageIdx, pageSize ?? DEFAULT_PAGE_SIZE, total);
            m.AllCount = total;

            // if filter, then filter
            if (filter == "admins")
            {
                var users = AppService.Admins.Select(a => UserRepository.GetUser(a));
                m.Users = users.ToPagedList(pageIdx, pageSize ?? DEFAULT_PAGE_SIZE, users.Count());
            }
            else if (filter == "authors")
            {
                var users = AppService.Workspaces.SelectMany(a => a.Authors).Concat(
                AppService.Workspaces.SelectMany(c => c.Collections).SelectMany(a => a.Authors)).Select(a => UserRepository.GetUser(a));
                
                m.Users = users.ToPagedList(pageIdx, pageSize ?? DEFAULT_PAGE_SIZE, users.Count());
            }
            else if (filter == "contribs")
            {
                var users = AppService.Workspaces.SelectMany(a => a.Contributors).Concat(
                AppService.Workspaces.SelectMany(c => c.Collections).SelectMany(a => a.Contributors)).Select(a => UserRepository.GetUser(a));

                m.Users = users.ToPagedList(pageIdx, pageSize ?? DEFAULT_PAGE_SIZE, users.Count());
            }
            else if (filter == "users")
            {
                //TODO: find a way to efficiently filter to only user
            }
            return View("AdminUsers", "Admin", m);
        }

        [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ViewResult EditUser(string userId)
        {
            User u = !string.IsNullOrEmpty(userId) ? UserRepository.GetUser(userId) : new User();
            AdminUserModel m = new AdminUserModel()
            {
                UserId = userId,
                Name = u.Name,
                Email = u.Email,
                FullName = !string.IsNullOrEmpty(u.FullName) ? u.FullName : null,
                Uri = u.Uri != null ? u.Uri.ToString() : null,
                Ids = u.Ids != null ? string.Join(Environment.NewLine, u.Ids.ToArray()) : null,
                Password = u.Password
            };
            if (TempData["success"] != null)
            {
                m.Notifications.Add("Saved!", "The user data was saved successfully.");
            }
            return View("AdminUser", "Admin", m);
        }

        [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteUser(string userId)
        {
            try
            {
              if (AppService.Admins.Contains(userId) || AppService.Workspaces.SelectMany(w => w.People).Contains(userId) ||
                AppService.Workspaces.SelectMany(w => w.Collections).SelectMany(w => w.People).Contains(userId))
                  throw new Exception("User is currently an administrator, author, or contributor.  Please remove user from role(s) before deleting.");
                UserRepository.DeleteUser(userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
                return Json(new { error = ex.Message });
            }
        }

        [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditUser(AdminUserModel m)
        {

            if (string.IsNullOrEmpty(m.Name) || m.Name.Trim().Length == 0)
            {
                this.ModelState.AddModelError("name", "The username is required.");
            }

            if (string.IsNullOrEmpty(m.Email) || m.Email.Trim().Length == 0)
            {
                this.ModelState.AddModelError("email", "The email is required.");
            } //TODO: check valid email format

            if (string.IsNullOrEmpty(m.Ids) || m.Ids.Trim().Length == 0)
            {
                this.ModelState.AddModelError("ids", "You must supply at least one Id.");
            }

            if (!string.IsNullOrEmpty(m.Password) && m.Password != m.ConfirmPassword)
            {
                this.ModelState.AddModelError("confirmPassword", "The passwords don't match.  Please reconfirm.");
            }

            if (!string.IsNullOrEmpty(m.Uri) && !Uri.IsWellFormedUriString(m.Uri, UriKind.Absolute))
            {
                this.ModelState.AddModelError("uri", "The website address you've entered is not correct.");
            }

            if (this.ModelState.IsValid)
            {
                //TODO: support both ajax and full page
                try
                {
                    bool isNew = true;
                    User u = new User();
                    if (!string.IsNullOrEmpty(m.UserId))
                    {
                        u = UserRepository.GetUser(m.UserId);
                        if (u == null) throw new Exception("Can't find user to modify.");
                        else isNew = false;
                    }
                    u.Ids = m.Ids.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(id => id.Trim());
                    u.Name = m.Name;
                    u.FullName = m.FullName;
                    u.Email = m.Email;
                    if (!string.IsNullOrEmpty(m.Uri)) u.Uri = new Uri(m.Uri);
                    if (!string.IsNullOrEmpty(m.Password)) u.Password = m.Password;
                    m.UserId = u.Ids.First();

                    if (isNew) UserRepository.CreateUser(u);
                    else UserRepository.UpdateUser(u);

                    TempData["success"] = true;
                    return RedirectToAction("EditUser", new { userId = m.UserId });
                }
                catch (Exception ex)
                {
                    m.Errors.Add(ex.Message);
                }
            }
            else
            {
                m.Errors.Add("");
            }

            return View("AdminUser", "Admin", m);
        }

        [ScopeAuthorize(Roles = AuthRoles.AuthorOrAdmin)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ViewResult RoleMatrix()
        {
            var m = new AdminRoleMatrixModel();
            m.RoleMatrix = AppService.RoleMatrix ?? AtomSite.Domain.RoleMatrix.Default;
            return View("AdminRoleMatrix", "Admin", m);
        }
    }
}
