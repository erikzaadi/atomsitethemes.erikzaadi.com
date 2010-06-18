<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<div class="widget summary">
    <h3><%= Model.Feed.Title %></h3>
<% if (Model.EntryCount > 0) { %>
    <ol class="entries">
    <% foreach (AtomEntry entry in Model.Feed.Entries) {%>
        <li>
            <h4>
	            <strong><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>"><%= entry.Title.Text %></a></strong> by <% Html.RenderPartial("AtomPubPeople", entry.People); %>
            </h4>
            <p>
	            <%= entry.Text.ToStringPreview(300) %>
            </p>
            <% if (entry.Categories.Count() > 0) { %> 
            <div class="filedUnder">
		        Filed under: <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Feed.Id }); %>
		    </div>
		    <% } %>
            <em>Posted <%= Html.DateTimeAbbreviation(entry.Date) %></em>
        </li>
    <%} %>
    </ol>
<%} else { %>
	<div style="color:Red;">There is no feed to display.</div><br />
<%} %>
</div>