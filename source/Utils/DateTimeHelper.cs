namespace AtomSite.Utils
{
  using System;
  using System.Text;
  using System.Security.Cryptography;

  public static class DateTimeHelper
  {
    public static DateTimeOffset GetForTimeZone(string date, string time, string timeZoneId)
    {
      var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
      DateTime dt = DateTime.Parse(string.Format("{0} {1}", date, time));
      return new DateTimeOffset(dt, tz.GetUtcOffset(dt));
    }
  }
}
