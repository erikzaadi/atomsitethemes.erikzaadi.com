<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<% 
    var showAdvancedMenu = false;

    if (showAdvancedMenu)
    {
%>
<div id="instance-sidebartabswidget-section-popular" class="box section" style="display: block;">
    <div class="box-top-left">
        <div class="box-top-right">
        </div>
    </div>
    <div class="box-main">
        <div class="box-content">
            <% if (Model.EntryCount > 0)
               { %>
            <ul class="menuList">
                <% foreach (AtomEntry entry in Model.Feed.Entries)
                   { %>
                <li><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>" class="fadeThis" style="padding-left: 0px;">
                    <span class="entry">
                        <%= entry.Title.ToString() %>
                        <span class="details inline"><em>updated
                            <%= Html.DateTimeAgoAbbreviation(entry.Updated) %></em></span></span><span class="hover"
                                style="opacity: 0;"></span></a></li>
                <% } %>
            </ul>
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
<%}
    else
    { %>
<li class="block">
    <div class="block clearfix">
        <h3 class="title">
            <span>
                <%= Model.Feed.Title.Text %></span></h3>
        <div class="block-div">
        </div>
        <div class="block-div-arrow">
        </div>
        <% if (Model.EntryCount > 0)
           { %>
        <ul>
            <% foreach (AtomEntry entry in Model.Feed.Entries)
               { %>
            <li><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">
                <%= entry.Title.ToString() %></a> <em>updated
                    <%= Html.DateTimeAgoAbbreviation(entry.Updated) %></em></li>
            <% } %>
        </ul>
        <% }
           else
           { %>
        <div style="color: Red;">
            There is nothing to display.</div>
        <br />
        <% } %>
    </div>
</li>
<%} %>