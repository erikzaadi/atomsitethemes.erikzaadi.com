using System;
using System.Collections.Generic;
using System.Linq;

using AtomSite.Domain;

namespace AtomSite.Repository.Mock
{
    public class MockUserRepository : IUserRepository
    {
        private readonly List<User> users = new List<User>();

        #region IUserRepository Members
      
        public void DeleteUser(string userName)
        {
          User result = users.Where(user =>
                user.Name.ToUpperInvariant() == userName.ToUpperInvariant()).SingleOrDefault();
          if (result != null)
          {
            users.Remove(result);
          }
        }
        public User GetUser(string userName)
        {
          //just return null;
            return GetUserOrNull(userName);
        }

        public void CreateUser(User user)
        {
            User existingUser = GetUserOrNull(user.Ids.First());
            if (existingUser == null)
            {
                users.Add(user);
            }
            else
            {
                throw new Exception("User already exists with name" + user.Name);
            }
        }

        public User UpdateUser(User user)
        {
            User existingUser = GetUser(user.Ids.First());
            if (existingUser != null)
            {
                users.Remove(existingUser);                
            }

            users.Add(user);
            return user;
        }

        public IEnumerable<User> GetUsers(int pageIndex, int pageSize, out int totalUsers)
        {
            totalUsers = users.Count;
            return users.Page(pageSize, pageIndex);
        }
        public IEnumerable<User> GetUsersByName(string name)
        {
          return users.Where(u => u.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }
        public IEnumerable<User> GetUsersByEmail(string email)
        {
          return users.Where(u => u.Email.ToLowerInvariant() == email.ToLowerInvariant());
        }

        #endregion

        #region private

        public User GetUserOrNull(string id)
        {
            return
                users.Where(user => user.Ids
                  .Select(i => i.ToUpperInvariant())
                  .Contains(id.ToUpperInvariant()))
                  .SingleOrDefault();
        }

        #endregion
    }
}
