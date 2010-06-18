/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using AtomSite.Domain;

  public interface IRaterService
  {
    RaterModel GetRaterModel(Id entryId, User user, string ip);
    RaterModel Rate(Id entryId, float rating, User user, string ip);
  }
}
