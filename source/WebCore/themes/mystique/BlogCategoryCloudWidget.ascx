<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<%@ Import Namespace="ThemeExtensions.HtmlHelpers" %>

<%
    var showAdvancedMenu = Html.ThemeExtensions().Theme.GetThemeBooleanProperty("showadvancedmenu", false);

    if (showAdvancedMenu)
    {
%><div id="instance-sidebartabswidget-section-category-cloud" class="box section"
    style="display: none;">
    <div class="box-top-left">
        <div class="box-top-right">
        </div>
    </div>
    <div class="box-main">
        <div class="box-content">
            <div class="tag-cloud">
                <% if (Model.GetCategories().Count() > 0)
                   { %>
                <p class="cloud">
                    <% foreach (var cat in Model.GetCategories())
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
            </div>
        </div>
    </div>
</div>
<%}
    else
    { %>
<li class="block">
    <div class="block clearfix">
        <h3 class="title">
            <span>Category Cloud</span></h3>
        <div class="block-div">
        </div>
        <div class="block-div-arrow">
        </div>
        <div class="tag-cloud">
            <% if (Model.GetCategories().Count() > 0)
               { %>
            <p class="cloud">
                <% foreach (var cat in Model.GetCategories())
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
        </div>
        <%} %>
