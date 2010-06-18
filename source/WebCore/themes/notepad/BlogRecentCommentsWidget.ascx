<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
    <h4 class="widgettitle">Recent Comments</h4>
	<% if (Model.EntryCount > 0) { %>
    <ul class="recentcomments">
        <% foreach (AtomEntry entry in Model.Feed.Entries) { %>
        <li class="recentcomments">
	        <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id.GetParentId()) + "#" + entry.Id.ToWebId() %>"><%= entry.Title.ToString() %></a>
	        <p><%= entry.Text.ToStringPreview(64) %></p>
	        <em><%= entry.Authors.Count() > 0 ? entry.Authors.First().ToString() + " - " : string.Empty %><%= Html.DateTimeAbbreviation(entry.Date, (d, tz) => d.ToString("g")) %></em>
        </li>
        <%} %>
    </ul>
<% } else { %>
    <div style="color:Red;">There is nothing to display.<br /></div>
<% } %>
