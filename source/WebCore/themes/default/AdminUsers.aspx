<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminUsersModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Users &rsaquo; AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">

<h2>Users <%= !string.IsNullOrEmpty(Model.Filter) ? "(" + Model.Filter + ")" : null %></h2>
<h3 class="action"><a rel="add" href="<%= Url.Action("EditUser", "Admin") %>">Add New User</a></h3>
<div class="filter">
<a class="all" href="<%= Url.Action("Users", "Admin") %>">All (<%= Model.AllCount %>)</a> | 
<a class="admins" href="<%= Url.Action("Users", "Admin", new { filter="admins" }) %>">Administrators (<%= Model.AdminsCount %>)</a> | 
<a class="authors" href="<%= Url.Action("Users", "Admin", new { filter="authors" }) %>">Authors (<%= Model.AuthorsCount %>)</a> | 
<a class="contribs" href="<%= Url.Action("Users", "Admin", new { filter="contribs" }) %>">Contributors (<%= Model.ContribsCount %>)</a><%-- | 
<a class="users" href="<%= Url.Action("Users", "Admin", new { filter="users" }) %>">Users</a>--%>
</div>
<table class="users"><thead>
<tr><th style="min-width:12em">Name</th><th>Ids</th><th>Email</th><th>Uri</th><th style="width:12em">Roles</th></tr></thead>
<tfoot>
<tr><td colspan="3">Pages: <%
foreach (int i in Enumerable.Range(1, Model.Users.PageCount))
{
    if (Model.Users.PageNumber == i) Response.Write("<strong>");
  Response.Write("<a href='" + Url.Action("Users", "Admin", new { filter=Model.Filter, page = i }) + "'>" + i + "</a> ");
  if (Model.Users.PageNumber == i) Response.Write("</strong>");
}

%></td><td colspan="2">Count: <strong><%=Model.Users.TotalItemCount %></strong></td></tr></tfoot>

<tbody>
<% bool alt = true; foreach (User u in Model.Users) { alt = !alt;%>
<tr id="<%= u.Ids.FirstOrDefault() %>" class="entry<%= alt? " alt" : string.Empty %>">
<td class="name"><strong><%= u.Name %></strong>
<div class="actions">
          <a rel="edit" href="<%= Url.Action("EditUser", "Admin", new { userId = u.Ids.First() }) %>">edit</a> | 
          <a rel="delete-user" href="<%= Url.Action("DeleteUser", "Admin", new { userId = u.Ids.First() }) %>">delete</a></div>
</td>
<td class="ids"><%= string.Join("<br />", u.Ids.ToArray()) %></td>
<td class="email"><%= u.Email %></td>
<td class="uri"><%= u.Uri %></td>
<td class="roles"><%= Model.GetRoles(u) %></td>
</tr>
<%} %>
</tbody>

</table>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
});
</script>
</asp:Content>
