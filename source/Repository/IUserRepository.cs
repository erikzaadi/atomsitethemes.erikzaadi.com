/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository
{
  using System.Collections.Generic;
  using AtomSite.Domain;

  public interface IUserRepository
  {
    User GetUser(string userId);
    void CreateUser(User user);
    User UpdateUser(User user);
    void DeleteUser(string userId);
    IEnumerable<User> GetUsers(int pageIndex, int pageSize, out int totalUsers);
    IEnumerable<User> GetUsersByName(string name);
    IEnumerable<User> GetUsersByEmail(string email);
  }
}
