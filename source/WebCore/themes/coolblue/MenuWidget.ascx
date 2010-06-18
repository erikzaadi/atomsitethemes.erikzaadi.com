<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<MenuModel>" %>
<ul><% foreach (AtomSite.WebCore.MenuItem item in Model.MenuItems) { %>
  <li<%= item.Selected ? " id='current'" : string.Empty %>>
    <a href="<%= item.Href %>" title="<%= item.Title %>"><%= item.Text %></a>
  </li><% } %>
</ul>