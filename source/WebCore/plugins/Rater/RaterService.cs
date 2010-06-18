/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.Rater
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AtomSite.Domain;
  using AtomSite.Repository;
  using AtomSite.WebCore;

  public class RaterService : IRaterService
  {
    protected IAuthorizeService AuthorizeService { get; private set; }
    protected IAtomEntryRepository AtomEntryRepository { get; private set; }
    protected IRouteService RouteService { get; private set; }
    protected ILogService LogService { get; private set; }

    public RaterService(IAuthorizeService auth, IAtomEntryRepository repo, ILogService logger,
      IRouteService router)
    {
      AuthorizeService = auth;
      AtomEntryRepository = repo;
      LogService = logger;
      RouteService = router;
    }

    public RaterModel Rate(Id entryId, float rating, User user, string ip)
    {
      LogService.Info("Rater.Rate entryId={0} rating={1} ip={2}", entryId, rating, ip);

      if (!AuthorizeService.IsAuthorized(user, entryId.ToScope(), AuthAction.RateEntryOrMedia))
        throw new UserNotAuthorizedException(user.Name, AuthAction.RateEntryOrMedia.ToString());

      if (rating < 1 || rating > 5) throw new ArgumentOutOfRangeException("Rating value must be 1 thru 5.");

      AtomEntry entry = AtomEntryRepository.GetEntry(entryId);
      if (entry.Raters.Contains(ip)) throw new UserAlreadyRatedEntryException(ip, entry.Id.ToString());

      entry.RatingCount++;
      entry.RatingSum += (int)Math.Round(rating);
      entry.Edited = DateTimeOffset.UtcNow;
      List<string> raters = entry.Raters.ToList();
      raters.Add(ip);
      entry.Raters = raters;
      entry = AtomEntryRepository.UpdateEntry(entry);
      return new RaterModel()
      {
        PostHref = RouteService.RouteUrl("RaterRateEntry", entryId),
        Rating = entry.Rating,
        CanRate = false,
        RatingCount = entry.RatingCount
      };
    }

    public RaterModel GetRaterModel(Id entryId, User user, string ip)
    {
      bool auth = AuthorizeService.IsAuthorized(user, entryId.ToScope(), AuthAction.RateEntryOrMedia);
      AtomEntry entry = AtomEntryRepository.GetEntry(entryId);
      return new RaterModel()
      {
        PostHref = RouteService.RouteUrl("RaterRateEntry", entryId),
        Rating = entry.Rating,
        CanRate = auth && !entry.Raters.Contains(ip),
        RatingCount = entry.RatingCount
      };
    }
  }

  public class UserAlreadyRatedEntryException : BaseException
  {
    public UserAlreadyRatedEntryException(string ip, string entry)
      : base("The user with ip \"{0}\" has already rated \"{1}\".", ip, entry)
    { }
  }

}
