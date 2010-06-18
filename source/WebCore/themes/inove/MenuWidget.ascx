<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<MenuModel>" %>
<ul id="menus">
    <% foreach (AtomSite.WebCore.MenuItem item in Model.MenuItems)
       {
           var isHome = item.Href == Url.Content("~/");
           %>
    <li class="<%= item.Selected ? "current_page_item" : "page_item" %>"><a<%=isHome ? " class=\"home\"" : string.Empty %> href="<%= item.Href %>" title="<%= item.Title %>">
        <%= isHome ? string.Empty : item.Text %></a> </li>
    <% } %>
    <li><a class="lastmenu" href="javascript:void(0);"></a></li>
</ul>
