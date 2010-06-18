/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using AtomSite.Domain;
  
  public interface ITrackbackService
  {
    void AutoPing(AtomEntry entry);
    string GetTrackbackMetadata(Id entryId);
    PingbackResult ReceivePingback(Id entryId, Uri sourceUrl, Uri targetUrl, string ip, Uri referrer);
    TrackbackResult ReceiveTrackback(Id entryId, string title, string excerpt, string url, string blogName, string ip, Uri referrer);
    PingbackResult SendPingback(AtomEntry entry, Uri pingUrl, Uri pageUrl);
    TrackbackResult SendTrackback(AtomEntry entry, Uri pingUrl);
  }
}
