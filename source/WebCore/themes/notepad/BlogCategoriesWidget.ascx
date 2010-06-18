<%@ Control Language="C#" Inherits="ViewUserControl<FeedModel>" %>
    <h4 class="widgettitle">Categories</h4>
    <% if (Model.GetCategories().Count() > 0) { %>
        <ul>
        <% foreach (AtomCategory cat in Model.GetCategories()) { %>
            <li><a href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>"><%= cat%> (<%= Model.GetEntries(cat).Count()%>)</a></li>
        <%} %>
        </ul>
    <% } else { %>
        <div style="color:Red;">There is nothing to display.</div><br />
    <% } %>
