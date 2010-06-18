<%@ Control Language="C#" Inherits="ViewUserControl<BaseModel>" %>
|<span class="acctstat">
    <% if (Model.User.IsAuthenticated)
       { %>
    <%= Model.User.Name%>
    | <a class="logout" href="<%= Url.Action("Logout", "Account") %>">Logout</a>
    <% }
       else
       { %>
    <a class="login" href="<%= Url.Action("Login", "Account") %>?ReturnUrl=<%= Server.UrlEncode(Page.Request.Url.PathAndQuery) %>">
        Login</a>
    <%} %></span> 