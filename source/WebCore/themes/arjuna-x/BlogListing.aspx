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
    <div class="postHeaderCompact">
        <div class="inner">
            <h1 class="postTitle">
                <%= Model.Feed.Title %>
                <%if (Model.Feed.Subtitle != null)
                  { %>
                -
                <%= Model.Feed.Subtitle %>
                <%} %></h1>
            <div class="bottom">
                <span></span>
            </div>
        </div>
    </div>
    <% foreach (AtomEntry entry in Model.Feed.Entries)
       { %>
    <div id="<%= entry.Id.ToWebId() %>" class="post">
        <div class="postHeader">
            <h1 class="postTitle">
                <span><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>" title="<%= entry.Title.Text %>">
                    <%= entry.Title.Text %></a></span></h1>
            <div class="bottom">
                <div>
                    <span class="postDate"><span class="date">
                        <%= Html.DateTimeAbbreviation(entry.Date) %></span></span> <span class="postAuthor">
                            <% Html.RenderPartial("AtomPubPeople", entry.People, new ViewDataDictionary() { { "id", entry.Id } }); %></span>
                    <% if (Model.Collection.AnnotationsOn)
                       { %>
                    <a class="postCommentLabel" href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>#comments"
                        title="Comment on <%= entry.Title.Text %>"><span>Comments (<%= entry.Total%>)</span></a>
                    <%}%>
                </div>
            </div>
        </div>
        <div class="postContent">
            <%= entry.IsExtended ? entry.ContentBeforeSplit.ToString() : entry.Text.Text %>
            <% if (entry.IsExtended) Response.Write("<a class=\"postReadMore\" href=\"" + Url.RouteIdUrl("BlogEntry", entry.Id) + "\"><span>Read More</span></a>"); %>
        </div>
        <div class="postLinkPages">
        </div>
        <div class="postFooter">
            <div class="r">
            </div>
            <div class="left">
                <span class="postCategories">
                    <% if (entry.Categories.Count() > 0)
                       { %>
                    <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                       } %></span>
            </div>
        </div>
    </div>
    <%} %>
    <div class="pagination">
        <div>
            <% if (Url.GetNextPage(Model.Feed.Links) != null)
               {%>
            <a rel="next" class="older" href="<%= Url.GetNextPage(Model.Feed.Links) %>"><span>Older
                Entries</span></a>
            <%} %>
            <% if (Url.GetPrevPage(Model.Feed.Links) != null)
               {%>
            <a rel="prev" class="newer" href="<%= Url.GetPrevPage(Model.Feed.Links) %>"><span>Newer
                Entries</span></a>
            <%} %>
        </div>
    </div>
</asp:Content>
