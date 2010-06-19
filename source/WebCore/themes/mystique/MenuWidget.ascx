<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<MenuModel>" %>
<ul id="navigation" class="clearfix">
    <% foreach (var item in Model.MenuItems)
       { %>
    <li><a class="fadeThis<%= item.Selected ? " active" : string.Empty %><%= item.Href== Url.Content("~/") ? " home" : string.Empty %>"
        href="<%= item.Href %>" title="<%= item.Title %>"><span class="title">
            <%= item.Text %></span><span class="pointer"></span></a> </li>
    <% } %>
</ul>
