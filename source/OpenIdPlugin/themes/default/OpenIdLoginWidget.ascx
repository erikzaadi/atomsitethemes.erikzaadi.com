<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<h3>Login with OpenID</h3>
<form class="openid" method="post" action="<%= Url.Action("Login", "Account") %>?ReturnUrl=<%= !string.IsNullOrEmpty(Page.Request.QueryString["ReturnUrl"]) ? Page.Request.QueryString["ReturnUrl"] : Server.UrlEncode(Page.Request.Url.PathAndQuery) %>">
  <div><ul class="providers">
  <li class="highlight openid" title="OpenID"><img src="<%= Url.ImageSrc("openidW.png") %>" alt="icon" />
  <span><strong>http://{your-openid-url}</strong></span></li>
  <li class="direct" title="Google"><%  %>
		<img src="<%= Url.ImageSrc("googleW.png") %>" alt="icon" /><span>https://www.google.com/accounts/o8/id</span></li>
  <li class="direct" title="Yahoo">
		<img src="<%= Url.ImageSrc("yahooW.png") %>" alt="icon" /><span>http://yahoo.com/</span></li>
  <li class="username" title="AOL screen name">
		<img src="<%= Url.ImageSrc("aolW.png") %>" alt="icon" /><span>http://openid.aol.com/<strong>username</strong></span></li>
  <li class="username" title="MyOpenID user name">
		<img src="<%= Url.ImageSrc("myopenid.png") %>" alt="icon" /><span>http://<strong>username</strong>.myopenid.com/</span></li>
  <li class="username" title="Flickr user name">
		<img src="<%= Url.ImageSrc("flickr.png") %>" alt="icon" /><span>http://flickr.com/<strong>username</strong>/</span></li>
  <li class="username" title="Technorati user name">
		<img src="<%= Url.ImageSrc("technorati.png") %>" alt="icon" /><span>http://technorati.com/people/technorati/<strong>username</strong>/</span></li>
  <li class="username" title="Wordpress blog name">
		<img src="<%= Url.ImageSrc("wordpress.png") %>" alt="icon" /><span>http://<strong>username</strong>.wordpress.com</span></li>
  <li class="username" title="Blogger blog name">
		<img src="<%= Url.ImageSrc("blogger.png") %>" alt="icon" /><span>http://<strong>username</strong>.blogspot.com/</span></li>
  <li class="username" title="LiveJournal blog name">
		<img src="<%= Url.ImageSrc("livejournal.png") %>" alt="icon" /><span>http://<strong>username</strong>.livejournal.com</span></li>
  <li class="username" title="ClaimID user name">
		<img src="<%= Url.ImageSrc("claimid.png") %>" alt="icon" /><span>http://claimid.com/<strong>username</strong></span></li>
  <li class="username" title="Vidoop user name">
		<img src="<%= Url.ImageSrc("vidoop.png") %>" alt="icon" /><span>http://<strong>username</strong>.myvidoop.com/</span></li>
  <li class="username" title="Verisign user name">
		<img src="<%= Url.ImageSrc("verisign.png") %>" alt="icon" /><span>http://<strong>username</strong>.pip.verisignlabs.com/</span></li>
  </ul></div>
  <fieldset>
  <label for="openid_username">Enter your <span>Provider user name</span></label>
  <div><span></span><input type="text" name="openid_username" /><span></span>
  <input type="submit" value="Login" /></div>
  </fieldset>
  <fieldset>
  <label for="openid_identifier">Enter your <a class="openid_logo" href="http://openid.net">OpenID</a></label>
  <div><input type="text" name="openid_identifier" />
  <input type="submit" value="Login" /></div> 
  </fieldset>
</form>
