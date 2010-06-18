<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h4>
    <span>Archive</span></h4>
<% if (Model.EntryCount > 0)
   { %>
<ol>
    <% foreach (int year in Model.GetYears())
       { %>
    <% foreach (int month in Model.GetMonths(year))
       { %>
    <li><a href="<%= Url.RouteIdUrl("BlogDateMonth", Model.Feed.Id, new { year = year, month = month.ToString("00") }) %>">
        <%= new DateTime(year, month, 1).ToString("MMMM yyyy")%>
        (<%= Model.GetEntries(year, month).Count() %>)</a></li>
    <% }
           } %>
</ol>
<% }
   else
   { %>
<div style="color: Red;">
    There is nothing to display.</div>
<br />
<% } %>
<%--<div style="color:Red"><%= DateTime.Now.ToString() %></div>--%>
