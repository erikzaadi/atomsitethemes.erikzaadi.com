<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<MenuModel>" %>
            <ul id="nav" class="alignright">
<% foreach (AtomSite.WebCore.MenuItem item in Model.MenuItems) { %>
  <li class="<%= item.Selected ? "current_" : string.Empty %>page_item">
    <a href="<%= item.Href %>" title="<%= item.Title %>"><%= item.Text %></a>
  </li><% } %>
</ul>