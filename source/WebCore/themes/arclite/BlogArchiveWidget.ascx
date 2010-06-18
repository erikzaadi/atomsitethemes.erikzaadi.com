<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h3>
    Archive</h3>
<% if (Model.EntryCount > 0)
   { %>
<ul class="menu" >
    <% foreach (int year in Model.GetYears())
       { %>
    <% foreach (int month in Model.GetMonths(year))
       { %>
    <li class="cat-item trigger"><a class="fadeThis" href="<%= Url.RouteIdUrl("BlogDateMonth", Model.Feed.Id, new { year = year, month = month.ToString("00") }) %>">
        <%= string.Format("{0} ({1})", new DateTime(year, month, 1).ToString("MMMM yyyy"), Model.GetEntries(year, month).Count()) %></a><a
            href="<%= Url.RouteIdUrl("BlogDateMonth", Model.Feed.Id, new { year = year, month = month.ToString("00") }) %>"
            class="rss tip"></a> </li>
    <% }
       } %>
</ul>
<% }
   else
   { %>
<div style="color: Red;">
    There is nothing to display.</div>
<br />
<% } %>
<%--<div style="color:Red"><%= DateTime.Now.ToString() %></div>--%>
