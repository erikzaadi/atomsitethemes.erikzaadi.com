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
    <h2>
        <%= Model.Feed.Title %>
        <%if (Model.Feed.Subtitle != null)
          { %>
        -
        <%= Model.Feed.Subtitle %>
        <%} %></h2>
    <div class="navigation clear">
        <% if (Url.GetNextPage(Model.Feed.Links) != null)
           {%>
        <div>
            <a class="next" href="<%= Url.GetNextPage(Model.Feed.Links) %>">Older Entries</a></div>
        <%} %>
        <% if (Url.GetPrevPage(Model.Feed.Links) != null)
           {%>
        <div>
            <a class="prev" href="<%= Url.GetPrevPage(Model.Feed.Links) %>">Newer Entries</a></div>
        <%} %>
    </div>
    <ul class="archive">
        <% foreach (AtomEntry entry in Model.Feed.Entries)
           { %>
        <li>
            <div class="post-title">
                <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">
                    <%= entry.Title.Text %></a></div>
            <div class="post-details">
                Posted on <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">
                    <%= Html.DateTimeAbbreviation(entry.Date, (d, tz) => d.ToString("m") + " at " + d.ToString("t"))%></a>
                <% if (entry.Categories.Count() > 0)
                   { %>
                | Filed under
                <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                       } %>
            </div>
        </li>
        <%} %>
    </ul>
    
</asp:Content>
