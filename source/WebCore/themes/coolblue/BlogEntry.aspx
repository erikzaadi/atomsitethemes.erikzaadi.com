<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<BlogEntryModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <title>
        <%= Model.Entry.Title %></title>
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id) %>"
        title="<%= Model.Collection.Title %> Feed" />
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id) %>"
        title="<%= Model.Collection.Title %> Comments Feed" />
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateEntryAnnotationsFeed", Model.EntryId) %>"
        title="<%= Model.Entry.Title %> Comments Feed" />
    <% if (Model.Entry.Summary != null)
       { %><meta name="description" content="<%=Model.Entry.Summary %>" />
    <%} %>
    <%--<% if (User.Identity.IsAuthenticated) { %>
  <script src="<%= Url.ScriptSrc("user.js")%>" type="text/javascript"></script>
  <% } %>--%>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
    <div class="post" id="<%= Model.Entry.Id.ToWebId() %>">
        <div class="right">
            <h2>
                <a href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>">
                    <%= Model.Entry.Title.Text%></a></h2>
            <p class="post-info">
                <% if (Model.Entry.Categories.Count() > 0)
                   { %>
                Filed under
                <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id });
                   } %></p>
            <% Html.RenderContent(Model.Entry.Content); %>
        </div>
        <div class="left">
            <p class="dateinfo">
                <%= Model.Entry.Date.ToString("MMM").ToUpper().Trim()%><span><%= Model.Entry.Date.Day %></span></p>
            <div class="post-meta">
                <h4>
                    Post Info</h4>
                <ul>
                    <li class="user">
                        <% Html.RenderPartial("AtomPubPeople", Model.Entry.People, new ViewDataDictionary() { { "id", Model.Entry.Id } }); %></li>
                    <li class="time"><a href="#">
                        <%= Model.Entry.Date.ToString("hh:mm tt")%></a></li>
                    <li class="comment"><a href="#">
                        <%= Model.Entry.Total%>
                        Comments</a></li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="SideTop" ContentPlaceHolderID="sidetop" runat="server">
    <% if (User.Identity.IsAuthenticated)
       {%>
    <div id="userActions">
        <% if (Model.CanEdit())
           { %>
        <button onclick="location.href = '<%= Url.Action(Model.Entry.Media?"EditMedia":"EditEntry", "Admin", new { id = Model.Entry.Id.ToString() }) %>';">
            Edit</button>
        <%} %>
        <% if (Model.CanDelete())
           { %>
        <button type="button" onclick="del('<%= Model.Entry.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntry", Model.Entry.Id) %>');">
            Delete</button>
        <%} %>
        <%if (Model.ShowApproveAll())
          {%>
        <button onclick="approveAll('<%= Model.Entry.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveAll", Model.Entry.Id) %>', this)">
            Approve All</button>
        <%} %>
    </div>
    <%} %>
</asp:Content>
<asp:Content ID="SideMid" ContentPlaceHolderID="sidemid" runat="server">
    <div id="updated" class="stat">
        <label for="updated">
            Last Updated</label>
        <div class="statVal">
            <%= Html.DateTimeAgoAbbreviation(Model.Entry.Updated) %>
        </div>
    </div>
</asp:Content>
