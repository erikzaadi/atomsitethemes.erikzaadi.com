<%@ Control Language="C#" Inherits="ViewUserControl<FeedModel>" %>
<% 
    var showAdvancedMenu = false;

    if (showAdvancedMenu)
    {
%>
<div id="instance-sidebartabswidget-section-categories" class="box section" style="display: block;">
    <div class="box-top-left">
        <div class="box-top-right">
        </div>
    </div>
    <div class="box-main">
        <div class="box-content">
            <% if (Model.GetCategories().Count() == 0)
               { %>
            <div style="color: Red;">
                There is nothing to display.</div>
            <br />
            <% }
               else
               { %>
            <!-- categories -->
            <ul class="menuList categories">
                <% foreach (AtomCategory cat in Model.GetCategories())
                   { %>
                <li class="cat-item"><a class="fadeThis" href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>"
                    style="padding-left: 0px;"><span class="entry">
                        <%= cat%>
                        <span class="details inline">(<%= Model.GetEntries(cat).Count()%>)</span></span><span
                            class="hover" style="opacity: 0;"></span></a><a title="XML" href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>"
                                class="rss bubble"></a></li>
                <%} %>
            </ul>
            <!-- /categories -->
            <% } %>
        </div>
    </div>
</div>
<%}
    else
    { %>
<li class="block">
    <div class="block clearfix">
        <h3 class="title">
            <span>Categories</span></h3>
        <div class="block-div">
        </div>
        <div class="block-div-arrow">
        </div>
        <% if (Model.GetCategories().Count() == 0)
           { %>
        <div style="color: Red;">
            There is nothing to display.</div>
        <br />
        <% }
           else
           { %>
        <!-- categories -->
        <ul>
            <% foreach (AtomCategory cat in Model.GetCategories())
               { %>
            <li><a href="<%= Url.RouteIdUrl("BlogCategory", Model.Feed.Id, new {term = cat.Term}) %>"
                style="padding-left: 0px;">
                <%= cat%>
                (<%= Model.GetEntries(cat).Count()%>)</a></li>
            <%} %>
        </ul>
        <!-- /categories -->
        <% } %>
    </div>
</li>
<%} %>