<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<FeedModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title><%= Model.Title %></title>
  <link rel="service" type="application/atomsvc+xml" href="<%= Url.RouteUrlEx("AtomPubService", AbsoluteMode.Force) %>" />
	<link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id) %>" title="<%= Model.Collection.Title %>" />
  <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id) %>" title="<%= Model.Collection.Title %> Comments Feed" />
  <link rel="wlwmanifest" type="application/wlwmanifest+xml" href="<%= Url.RouteIdUrl("BlogWriterManifest", Model.Feed.Id, AbsoluteMode.Force) %>" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
    <h3><%= Model.Feed.Title %> <%if (Model.Feed.Subtitle != null) { %> - <%= Model.Feed.Subtitle %> <%} %></h3>    
    <ol class="listing">
        <% foreach (AtomEntry entry in Model.Feed.Entries) { %>
        <li class="entry">
	        <h4><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>"><%= entry.Title.Text %></a></h4>
	        <div class="content">
	        <%= entry.IsExtended ? entry.ContentBeforeSplit.ToString() : entry.Text.Text %>
	        <% if (entry.IsExtended) Response.Write("<a class=\"more\" href=\"" + Url.RouteIdUrl("BlogEntry", entry.Id) + "\">Read More</a>"); %>
	        </div>
	        <div>
		        <address>Posted by <% Html.RenderPartial("AtomPubPeople", entry.People); %> on <span class="date"><%= Html.DateTimeAbbreviation(entry.Date, (d, tz) => d.ToString("m") + " at " + d.ToString("t"))%></span></address>
		        
		        <% if (Model.Collection.AnnotationsOn) { %>
		        <div class="commentslink"><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>#comments">Comments</a> (<%= entry.Total%>)</div>
		        <%} %>
	        </div><br />
        </li>
        <%} %>
    </ol>
    <div id="pagenav">
    <% if (Url.GetNextPage(Model.Feed.Links) != null) {%>
        <a class="next" href="<%= Url.GetNextPage(Model.Feed.Links) %>">Older Entries</a>
    <%} %>
    <% if (Url.GetPrevPage(Model.Feed.Links) != null) {%>
        <a class="prev" href="<%= Url.GetPrevPage(Model.Feed.Links) %>">Newer Entries</a>
    <%} %>
    </div>
</asp:Content>