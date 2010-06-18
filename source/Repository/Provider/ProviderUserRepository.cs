/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Repository.Provider
{
  using System;
  using System.Collections.Generic;
  using System.Web.Profile;
  using System.Web.Security;
  using AtomSite.Domain;

  /// <summary>
  /// User repository that is tied to ASP.NET membership provider.
  /// NOTE: this class is untested and unfinished
  /// </summary>
  public class ProviderUserRepository : IUserRepository
  {
    public ProviderUserRepository() { }

    #region IUserRepository Members

    public User GetUser(Uri id)
    {
      throw new NotImplementedException();
    }

    public void DeleteUser(string userName)
    {
      throw new NotImplementedException();
    }

    public User GetUser(string userName)
    {
      MembershipUser u = Membership.GetUser(userName);
      ProfileBase pc = ProfileBase.Create(userName, true);
      if (u == null || pc == null) throw new ResourceNotFoundException("user", userName);
      return new User()
      {
        Name = u.UserName,
        Email = u.Email,
        FullName = (string)pc[userName],
        Ids = (IEnumerable<string>)pc["Ids"]
      };
    }

    public void CreateUser(User user)
    {
      throw new NotImplementedException();
    }

    public User UpdateUser(User user)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<User> GetUsers(int pageIndex, int pageSize, out int totalUsers)
    {
      throw new NotImplementedException();
    }
    public IEnumerable<User> GetUsersByName(string name)
    {
      throw new NotImplementedException();
    }
    public IEnumerable<User> GetUsersByEmail(string email)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
