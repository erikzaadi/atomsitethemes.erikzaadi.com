<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<MenuModel>" %>
<ul id="headerMenu2">
    <%
        foreach (AtomSite.WebCore.MenuItem item in Model.MenuItems)
        {%>
    <li><a class="cat-item  page_item<%= item.Selected ? "homeIcon" : "cat-item" %>"
        href="<%=item.Href%>" title="<%=item.Title%>">
        <%=item.Text%></a> </li>
    <%
        }%>
</ul>
