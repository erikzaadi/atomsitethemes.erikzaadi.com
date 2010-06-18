<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<FeedModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <title>
        <%= Model.Title %></title>
    <link rel="service" type="application/atomsvc+xml" href="<%= Url.RouteUrlEx("AtomPubService", AbsoluteMode.Force) %>" />
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id) %>"
        title="<%= Model.Collection.Title %>" />
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id) %>"
        title="<%= Model.Collection.Title %> Comments Feed" />
    <link rel="wlwmanifest" type="application/wlwmanifest+xml" href="<%= Url.RouteIdUrl("BlogWriterManifest", Model.Feed.Id, AbsoluteMode.Force) %>" />
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
    <h3>
        <%= Model.Feed.Title %>
        <%if (Model.Feed.Subtitle != null)
          { %>
        -
        <%= Model.Feed.Subtitle %>
        <%} %></h3>
    <ol class="listing">
        <% foreach (AtomEntry entry in Model.Feed.Entries)
           { %>
        <li class="entry">
            <!-- post -->
            <div class="post">
                <div class="post-header">
                    <h3 class="post-title">
                        <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">
                            <%= entry.Title.Text%></a></h3>
                    <p class="post-date">
                        <span class="month">
                            <%= entry.Date.ToString("MMM")%></span> <span class="day">
                                <%= entry.Date.ToString("dd")%></span>
                    </p>
                    <p class="post-author">
                        <span class="info">posted by
                            <% Html.RenderPartial("AtomPubPeople", entry.People, new ViewDataDictionary() { { "id", entry.Id } }); %>
                            <% if (entry.Categories.Count() > 0)
                               { %>
                            In :
                            <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                               } %>
                            | <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>#comments" class="comments">Comments
                                (<%= entry.Total%>)</a> </span>
                    </p>
                </div>
                <div class="post-content clearfix">
                    <p class="alignleft">
                        <%= entry.IsExtended ? entry.ContentBeforeSplit.ToString() : entry.Text.Text %>
                        <% if (entry.IsExtended)
                           { %>
                        <a class="more" href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">Read More</a>
                        <%} %>
                    </p>
                    <p class="tags">
                        <% if (entry.Categories.Count() > 0)
                           { %>
                        Tags:
                        <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                           } %>
                    </p>
                </div>
            </div>
            <!-- /post -->
            <br />
        </li>
        <%} %>
    </ol>
    <div class="navigation">
        <% if (Url.GetPrevPage(Model.Feed.Links) != null)
           {%>
        <div class="alignleft">
            « <a rel="prev" href="<%= Url.GetPrevPage(Model.Feed.Links) %>">Newer Entries</a></div>
        <%} %>
        <% if (Url.GetNextPage(Model.Feed.Links) != null)
           {%>
        <div class="alignright">
            <a rel="next" href="<%= Url.GetNextPage(Model.Feed.Links) %>">Older Entries</a>
            »</div>
        <%} %>
        <div class="clear">
        </div>
    </div>
</asp:Content>
