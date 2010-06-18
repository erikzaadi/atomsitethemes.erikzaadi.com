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
    <% foreach (AtomEntry entry in Model.Feed.Entries)
       { %>
    <div class="post">
        <h2 class="post-title">
            <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>" title="<%= entry.Title.Text %>">
                <%= entry.Title.Text %></a></h2>
        <p class="post-date">
            <%= entry.Date.ToString("MMM d") %></p>
        <p class="post-data">
            <span class="postauthor">
                <% Html.RenderPartial("AtomPubPeople", entry.People); %></span><span class="postcategory">
                    <% if (entry.Categories.Count() > 0)
                       { %>
                    <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                       } %></span> <span class="postcomment">
                           <% if (Model.Collection.AnnotationsOn)
                              { %>
                           <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>#comments" title="Comment on <%= entry.Title.Text %>">
                               Comments</a> (<%= entry.Total%>)
                           <%} %></span></p>
        <p>
            <%= entry.Text.ToStringPreview(250) %>
            <% if (entry.Text.Text.Length > 250)
               {
            %>
            <a class="more-link" href="<%=Url.RouteIdUrl("BlogEntry", entry.Id)%>">Read More</a>
            <%
                }%>
    </div>
    <%} %>
    <p class="post-nav">
        <span class="previous">
            <% if (Url.GetNextPage(Model.Feed.Links) != null)
               {%>
            <a class="next" href="<%= Url.GetNextPage(Model.Feed.Links) %>">Older Entries</a>
            <%} %></span> <span class="next">
                <% if (Url.GetPrevPage(Model.Feed.Links) != null)
                   {%>
                <a class="prev" href="<%= Url.GetPrevPage(Model.Feed.Links) %>">Newer Entries</a>
                <%} %></span>
    </p>
</asp:Content>
