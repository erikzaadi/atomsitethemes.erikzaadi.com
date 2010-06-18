<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if (!Page.User.Identity.IsAuthenticated)
		 { %>
		 <div id="openid_login">
		 <a title="Login" rel="#openid_overlay" href="<%= Url.Action("OpenIdLogin", "OpenId") %>?ReturnUrl=<%= !string.IsNullOrEmpty(Page.Request.QueryString["ReturnUrl"]) ? Page.Request.QueryString["ReturnUrl"] : Server.UrlEncode(Page.Request.Url.PathAndQuery) %>#addcomment">Login</a> using <span class="openid_logo">OpenID</span>
		 <div>
		 <img src="<%= Url.ImageSrc("googleW.png") %>" alt="Google" />
		 <img src="<%= Url.ImageSrc("yahooW.png") %>" alt="Yahoo" />
		 <img src="<%= Url.ImageSrc("flickrW.png") %>" alt="flickr" />
		 <img src="<%= Url.ImageSrc("aolW.png") %>" alt="AOL" /></div><span>and more</span>
		 </div>
<%-- only if anonymous allowed show OR clause --%>

<div id="orLogin"><strong>Or</strong> provide your details</div>
<%} %>
