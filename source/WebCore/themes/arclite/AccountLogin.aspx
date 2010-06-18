<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<PageModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Login</title>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="content" runat="server">
<%-- Show Errors --%>
<%
    IList<string> errors = ViewData["errors"] as IList<string>;
    if (errors != null) {
        %><div id="errors">
            <ul>
            <% foreach (string error in errors) { %>
                <li><%= Html.Encode(error) %></li>
            <% } %>
            </ul>
          </div>
        <%
    }
     %>         
    
    <h3>Login</h3>
    <form id="normallogin" method="post" action="<%= Url.Action("Login", "Account") %>?ReturnUrl=<%= HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]) %>">
    <fieldset>
    <div>
        <div>
            <label for="username">Username</label>
			<%= Html.TextBox("username") %>
		</div>
        <div>
            <label for="password">Password</label>
            <%= Html.Password("password") %>
        </div><br style="clear:both" />
        <%= Html.CheckBox("rememberMe") %>&#160;<label for="rememberMe">Remember me?</label>
			</div>
			<div>
			<input type="submit" value="Login" /></div>
        </fieldset>
    </form><br style="clear:both" />
</asp:Content>
