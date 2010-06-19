<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<FeedModel>" %>

<%@ Import Namespace="ThemeExtensions.HtmlHelpers" %>
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
    <h1 class="title archive-category">
        <%= Model.Feed.Title %>
        <%if (Model.Feed.Subtitle != null)
          { %>
        -
        <%= Model.Feed.Subtitle %>
        <%} %></h1>
    <div class="divider">
    </div>
    <%
        var x = 0; foreach (AtomEntry entry in Model.Feed.Entries)
        {%>
    <div id="post-<%=entry.Id.ToWebId() %>" class="<%= string.Format("{0} {1}", "clearfix", ++x  % 2 != 0 ? "odd" : "even alt")%>">
        <h2 class="title">
            <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>" rel="bookmark" title="<%= entry.Title.Text %>">
                <%= entry.Title.Text %></a></h2>
    </div>
    <div class="post-date">
        <p class="day">
            <%= Html.ThemeExtensions().Date.GetDateNthFormat(entry.Date)%></p>
    </div>
    <div class="post-info clearfix">
        <p class="author alignleft">
            Posted by
            <% Html.RenderPartial("AtomPubPeople", entry.People); %>
        </p>
        <% if (Model.Collection.AnnotationsOn)
           { %>
        <p class="comments alignright">
            <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>#comments" class="<%= entry.Total == 0 ?"no":"comments"%>">
                <%=Html.ThemeExtensions().Entries.GetNumberOfCommentsString(entry.Total) %>
            </a>
        </p>
        <%}%>
    </div>
    <div class="post-content clearfix">
        <%= entry.IsExtended ? entry.ContentBeforeSplit.ToString() : entry.Text.Text %>
        <% if (entry.IsExtended) Response.Write("<a class=\"more-link\" href=\"" + Url.RouteIdUrl("BlogEntry", entry.Id) + "\">Read More</a>"); %>
    </div>
    <div class="post-tags">
        <% if (entry.Categories.Count() > 0)
           { %>
        <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                       } %>
    </div>
    <%
}%>
    <div class="page-navigation clearfix">
        <% if (Url.GetNextPage(Model.Feed.Links) != null)
           {%>
        <a class="last" href="<%= Url.GetNextPage(Model.Feed.Links) %>">Older Entries</a>
        <span class="extend">...</span>
        <%} %>
        <% if (Url.GetPrevPage(Model.Feed.Links) != null)
           {%>
        <a class="first" href="<%= Url.GetPrevPage(Model.Feed.Links) %>">Newer Entries</a>
        <span class="extend">...</span>
        <%} %>
    </div>
</asp:Content>
