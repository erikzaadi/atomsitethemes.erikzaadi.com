/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Plugins.TwitterPlugin
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AtomSite.Domain;
  using System.Xml.Linq;
  public class TwitterTimeline : XmlBase
  {
    public TwitterTimeline() : base(new XElement("statuses")) { }
    public TwitterTimeline(XElement xml) : base(xml) { }
    public IEnumerable<TwitterStatus> Statuses { get { return GetXmlValues<TwitterStatus>("status"); } }
  }

  //<statuses type="array">
  //<status>
  //  <created_at>Mon Dec 07 01:05:10 +0000 2009</created_at>
  //  <id>6416066277</id>
  //  <text>woops, forgot my password could expire when running AppPool under my login in IIS, no wonder the site is returning 503</text>
  //  <source>web</source>
  //  <truncated>false</truncated>
  //  <in_reply_to_status_id></in_reply_to_status_id>
  //  <in_reply_to_user_id></in_reply_to_user_id>
  //  <favorited>false</favorited>
  //  <in_reply_to_screen_name></in_reply_to_screen_name>
  //  <user>...</user>
  //  <geo/>
  //</status>
  public class TwitterStatus : XmlBase
  {
    public TwitterStatus() : base(new XElement("status")) { }
    public TwitterStatus(XElement xml) : base(xml) { }
    public DateTimeOffset CreatedAt
    {
      get
      {
        return DateTimeOffset.ParseExact(
          GetValue<string>("created_at"), "ddd MMM dd HH:mm:ss zzzzz yyyy",
          System.Globalization.CultureInfo.InvariantCulture);
      }
    }
    public string Id { get { return GetValue<string>("id"); } }
    public string Text { get { return GetValue<string>("text"); } }
    public TwitterUser User { get { return GetXmlValue<TwitterUser>("user"); } }
  }

  //  <user>
  //    <id>17393641</id>
  //    <name>jarrettv</name>
  //    <screen_name>jarrettv</screen_name>
  //    <location>Southeast USA</location>
  //    <description>Software Engineer</description>
  //    <profile_image_url>http://a3.twimg.com/profile_images/70031291/icon488_normal.png</profile_image_url>
  //    <url>http://jvance.com</url>
  //    <protected>false</protected>
  //    <followers_count>87</followers_count>
  //    <profile_background_color>1A1B1F</profile_background_color>
  //    <profile_text_color>666666</profile_text_color>
  //    <profile_link_color>5bae13</profile_link_color>
  //    <profile_sidebar_fill_color>252429</profile_sidebar_fill_color>
  //    <profile_sidebar_border_color>181A1E</profile_sidebar_border_color>
  //    <friends_count>98</friends_count>
  //    <created_at>Fri Nov 14 19:27:50 +0000 2008</created_at>
  //    <favourites_count>0</favourites_count>
  //    <utc_offset>-21600</utc_offset>
  //    <time_zone>Central Time (US &amp; Canada)</time_zone>
  //    <profile_background_image_url>http://s.twimg.com/a/1259882278/images/themes/theme9/bg.gif</profile_background_image_url>
  //    <profile_background_tile>false</profile_background_tile>
  //    <statuses_count>206</statuses_count>
  //    <notifications></notifications>
  //    <geo_enabled>false</geo_enabled>
  //    <verified>false</verified>
  //    <following></following>
  //  </user>
  public class TwitterUser : XmlBase
  {
    public TwitterUser() : base(new XElement("user")) { }
    public TwitterUser(XElement xml) : base(xml) { }
    public string Id { get { return GetValue<string>("id"); } }
    public string Name { get { return GetValue<string>("name"); } }
    public string ScreenName { get { return GetValue<string>("screen_name"); } }
    public int Followers { get { return GetInt32WithDefault("followers_count", 0); } }
    public Uri ProfileImageUrl { get { return GetUri("profile_image_url"); } }
  }
}
