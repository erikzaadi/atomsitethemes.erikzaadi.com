/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.File
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Web;
  using System.Web.Caching;
  using System.Web.Hosting;
  using System.Xml.Linq;
  using AtomSite.Domain;

  public class FileUserRepository : IUserRepository
  {
    const string cacheKey = "FileUserRepositoryUsers";
    public static readonly string DefaultUsersFileName = "Users.config";

    readonly string path;

    public FileUserRepository()
      : this(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, DefaultUsersFileName))
    { }

    public FileUserRepository(string storePath)
    {
        path = storePath;
    }

    List<User> Users
    {
      get
      {
        if (HttpRuntime.Cache[cacheKey] == null)
        {
          IEnumerable<User> users = System.IO.File.Exists(path) ?
              XDocument.Load(path).Root.Elements(Atom.SvcNs + "user")
              .Select(x => new User(x)).ToList() : new List<User>();
          HttpRuntime.Cache.Add(cacheKey, users, new CacheDependency(path),
              Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30) /*TODO: make configurable*/,
              CacheItemPriority.Normal, null);
        }
        return HttpRuntime.Cache[cacheKey] as List<User>;
      }
    }

    void Save()
    {
      new XDocument(new XElement(Atom.SvcNs + "users",
          Users.Select(u => u.Xml))).Save(path);
    }

    #region IUserRepository Members

    public void DeleteUser(string userId)
    {
      User user = GetUser(userId);
      if (user != null)
      {
        Users.Remove(user);
        Save();
      }
    }

    public User GetUser(string userId)
    {
      //just return null if not found
      return Users.Where(user => user.Ids.Select(id => id.ToUpperInvariant())
        .Contains(userId.ToUpperInvariant())).SingleOrDefault();
    }

    public void CreateUser(User user)
    {
      //verify unique id's
      foreach (string id in user.Ids)
      {
        if (GetUser(id) != null)
          throw new BaseException("User with given ID '{0}' already exists", id);
      }

      Users.Add(user);
      Save();
    }

    public User UpdateUser(User user)
    {
      //Get the existing user to update
      User existing = null;
      foreach (string id in user.Ids)
      {
        existing = GetUser(id);
        if (existing != null) break;
      }
      if (existing == null)
        throw new Exception("User could not be found with existing ID matching new ID.");

      //rather than update, just remove existing and add new
      Users.Remove(existing);
      Users.Add(user);
      Save();
      return user;
    }

    public IEnumerable<User> GetUsers(int pageIndex, int pageSize, out int totalUsers)
    {
      totalUsers = Users.Count;
      return Users.Skip(pageIndex * pageSize).Take(pageSize);
    }
    public IEnumerable<User> GetUsersByName(string name)
    {
      return Users.Where(u => u.Name.ToLowerInvariant() == name.ToLowerInvariant());
    }
    public IEnumerable<User> GetUsersByEmail(string email)
    {
      return Users.Where(u => u.Email.ToLowerInvariant() == email.ToLowerInvariant());
    }

    #endregion
  }
}
