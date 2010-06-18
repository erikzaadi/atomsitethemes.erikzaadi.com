<%@ Control Language="C#" Inherits="ViewUserControl<FeedModel>" %>
<h3>
    Categories</h3>
<% if (Model.GetCategories().Count() == 0)
   { %>
<div style="color: Red;">
    There is nothing to display.</div>
<br />
<% }
   else
   { %>
<!-- categories -->
<ul class="menu">
    <% foreach (AtomCategory cat in Model.GetCategories())
       { %>
    <li class="cat-item trigger"><a class="fadeThis" href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>">
        <%= string.Format("{0} ({1})", cat,Model.GetEntries(cat).Count())%></a><a href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>"
            class="rss tip"></a> </li>
    <%} %>
</ul>
<!-- /categories -->
<% } %>
