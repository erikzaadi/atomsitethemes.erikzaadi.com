<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<div id="annotations">
	<h3>Annotations</h3>
	<% if (Model.Feed.Entries.Count() == 0) { %>
	<em id="annotationsEmpty">No annotations.</em>
    <%}
    foreach (AtomEntry entry in Model.Feed.Entries) {%>
    <h4><a href="<%= Url.RouteIdUrl("AtomPubResource", entry.Id) %>"><%= entry.Title %></a></h4>
		<p><%= entry.Text.ToStringPreview(90) %></p>
		<% } %>
</div>
