<%@ Control Language="C#" Inherits="ViewUserControl<AdminUsersModel>" %>
<div class="widget settings users">
  <h3>
  <% foreach (var link in Model.AddLinks.Reverse()) { %>
  <a rel="add" href="<%= link.Value %>" title="<%= link.Key %>"><%= link.Key %></a><% } %><%= Model.Title %></h3>
  <ul>
    <%bool alt = false; foreach (User user in Model.Users) { alt = !alt; %>
    <li class="<%= alt ? "alt" : string.Empty %>" id="<%= user.Ids.First() %>">
    <%= Html.GravatarImg(user.Email, 40)%>
      <div class="actions"><% if (Model.CanEdit) { %><a rel="edit" href="<%= Url.Action("EditUser", "Admin", new { userId = user.Ids.First() }) %>">edit</a><%} %> <% if (Model.CanRemove) { %><a rel="remove" href="<%= Model.GetRemoveHref(user) %>">remove</a><%} %></div>
      <h4><a rel="edit" href="<%= Url.Action("EditUser", "Admin", new { userId = user.Ids.First() }) %>"><%= user.Name%></a> <small><%= Model.GetRoles(user) %></small></h4>
      <div><%= user.Email%></div>
    </li><%} %>
  </ul>
  <%-- TODO: paging --%>
</div>