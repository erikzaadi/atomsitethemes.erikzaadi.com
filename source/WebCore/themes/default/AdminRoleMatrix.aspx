<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminRoleMatrixModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Role Matrix &rsaquo; AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2>Role Matrix</h2>
<p>AtomSite has five roles. Administrators, authors, contributors, and users are authenticated. The fifth role is anonymous. Each role has a set of actions that can be performed depending on their function.</p>
<p>Please see <a href="http://atomsite.net/info/Roles.xhtml">Roles</a> for more information.</p>
<table class="roles"><thead>
<tr><th style="min-width:12em">Action</th><th>Admin</th><th>Author</th><th>Contrib</th><th>User</th><th>Anon</th></tr></thead>

<tbody>
<% bool alt = true; foreach (RoleAction a in Model.RoleMatrix.RoleActions) { alt = !alt;%>
<tr id="<%= a.Name %>" class="action<%= alt? " alt" : string.Empty %>"> 
<td class="name"><strong><%= a.Name %></strong></td>
<td class="admin"><%= a.Admin %></td>
<td class="author"><%= a.Author %></td>
<td class="contrib"><%= a.Contrib %></td>
<td class="user"><%= a.User %></td>
<td class="anon"><%= a.Anon %></td>
</tr>
<%} %>
</tbody>

</table>
<p><strong>Coming Soon</strong> for advanced users. Edit the role matrix at any scope (Entire Site, Workspace, Collection).</p>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
});
</script>
</asp:Content>
