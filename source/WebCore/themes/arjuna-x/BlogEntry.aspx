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
    <div id="<%= Model.Entry.Id.ToWebId() %>" class="post">
        <div class="postHeader">
            <h1 class="postTitle">
                <span><a href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>" title="<%= Model.Entry.Title.Text %>">
                    <%= Model.Entry.Title.Text %></a></span></h1>
            <div class="bottom">
                <div>
                    <span class="postDate"><span class="date">
                        <%= Html.DateTimeAbbreviation(Model.Entry.Date) %></span></span> <span class="postAuthor">
                            <% Html.RenderPartial("AtomPubPeople", Model.Entry.People, new ViewDataDictionary() { { "id", Model.Entry.Id } }); %></span>
                    <% if (Model.Collection.AnnotationsOn)
                       { %>
                    <a class="postCommentLabel" href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>#comments"
                        title="Comment on <%= Model.Entry.Title.Text %>"><span>Comments (<%= Model.Entry.Total%>)</span></a>
                    <%}%>
                </div>
            </div>
        </div>
        <div class="postContent">
            <% Html.RenderContent(Model.Entry.Content); %>
        </div>
        <div class="postLinkPages">
        </div>
        <div class="postFooter">
            <div class="r">
            </div>
            <div class="left">
                <span class="postCategories">
                    <% if (Model.Entry.Categories.Count() > 0)
                       { %>
                    <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id });
                       } %></span>
            </div>
            <% if (User.Identity.IsAuthenticated)
               {
                   if (Model.CanEdit())
                   { %>
            <a class="postEdit" href="<%= Url.Action(Model.Entry.Media?"EditMedia":"EditEntry", "Admin", new { id = Model.Entry.Id.ToString() }) %>">
                <span>Edit</span></a>
            <%}
               }%>
        </div>
    </div>
</asp:Content>
<asp:Content ID="SideTop" ContentPlaceHolderID="sidetop" runat="server">
    Last Updated
    <%= Html.DateTimeAgoAbbreviation(Model.Entry.Updated) %>
</asp:Content>
<asp:Content ID="SideMid" ContentPlaceHolderID="sidemid" runat="server">
    <% if (User.Identity.IsAuthenticated)
       {%>
    <h4>
        <span>Admin</span></h4>
    <% if (Model.CanEdit())
       { %>
    <button onclick="window.location.href = '<%= Url.Action(Model.Entry.Media?"EditMedia":"EditEntry", "Admin", new { id = Model.Entry.Id.ToString() }) %>';">
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
    <%} %>
</asp:Content>
