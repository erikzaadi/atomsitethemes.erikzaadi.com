<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h4 class="widgettitle">
    Categories</h4>
<% if (Model.GetCategories().Count() > 0)
   { %>
<p class="cloud">
    <% foreach (AtomCategory cat in Model.GetCategories())
       { %>
    <a style="font-size: <%= Model.GetCategorySize(cat, 1F, 1.9F) %>em" href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>">
        <%= cat %></a>
    <% } %>
</p>
<% }
   else
   { %>
<div style="color: Red;">
    There is nothing to display.</div>
<br />
<% } %>
