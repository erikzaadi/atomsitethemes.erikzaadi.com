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
    <div id="postpath">
        <a title="Go to homepage" href="<%= Url.Content("~/") %>">Home</a> &gt;
        <a title="Go to collection" href="<%= Url.Content("~/" + Model.Collection.Id.ToString().Split(new char[]{':'}).Last())%>"><%=Model.Collection.Title%></a> &gt;
        <% if (Model.Entry.Categories.Count() > 0)
           { %>
        <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id }); %>
        &gt;
        <%} %>
        <%= Model.Entry.Title.Text %>
    </div>
    <div class="post" id="post-<%= Model.Entry.Id.ToWebId() %>">
        <h2>
            <%= Model.Entry.Title.Text %></h2>
        <div class="info">
            <span class="date">
                <%= Html.DateTimeAbbreviation(Model.Entry.Date) %>
            </span><span class="author">
                <% Html.RenderPartial("AtomPubPeople", Model.Entry.People, new ViewDataDictionary() { { "id", Model.Entry.Id } }); %></span>
            <% if (User.Identity.IsAuthenticated && Model.CanEdit())
               {%>
            <span class="editpost"><a href="<%= Url.Action(Model.Entry.Media?"EditMedia":"EditEntry", "Admin", new { id = Model.Entry.Id.ToString() }) %>">
                Edit</a> </span>
            <%
                }%>
            <% if (Model.Collection.AnnotationsOn)
               { %>
            <span class="addcomment"><a href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>#respond">
                Leave a comment</a></span> <span class="comments"><a href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>#comments">
                    Go to comments</a></span>
            <%}%>
            <div class="fixed">
            </div>
        </div>
        <div class="content">
            <% Html.RenderContent(Model.Entry.Content); %>
            <div class="fixed">
            </div>
        </div>
        <div class="under">
            <% if (Model.Entry.Categories.Count() > 0)
               { %>
            <span class="categories">Categories: </span><span>
                <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id }); %>
            </span>
            <%} %>
        </div>
    </div>
</asp:Content>
<asp:Content ID="SideTop" ContentPlaceHolderID="sidetop" runat="server">
    <% if (User.Identity.IsAuthenticated)
       {%>
    <div id="userActions" class="widget">
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
    <div class="widget">
        <div id="updated" class="stat">
            <label for="updated">
                Last Updated</label>
            <div class="statVal">
                <%= Html.DateTimeAgoAbbreviation(Model.Entry.Updated) %>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
    <script type="text/javascript" src="<%= Url.ScriptSrc("comment.js")%>"></script>

</asp:Content>