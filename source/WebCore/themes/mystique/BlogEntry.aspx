<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<BlogEntryModel>" %>

<%@ Import Namespace="ThemeExtensions.UrlHelpers" %>
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
    <div id="post-<%=Model.Entry.Id.ToWebId() %>" class="post single">
        <h2 class="title">
            <a href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>" rel="bookmark" title="<%= Model.Entry.Title.Text %>">
                <%= Model.Entry.Title.Text%></a></h2>
        <div class="post-content clearfix">
            <% Html.RenderContent(Model.Entry.Content); %>
        </div>
        <div class="post-tags">
            <% if (Model.Entry.Categories.Count() > 0)
               { %>
            <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id });
               } %>
        </div>
        <table class="post-meta">
            <tr>
                <td>
                    <!-- socialize -->
                    <div class="shareThis clearfix">
                        <a href="#" class="control share">Share this post!</a>
                        <ul class="bubble">
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.Twitter) %>"
                                class="twitter" title="Tweet This!"><span>Twitter</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.Digg) %>"
                                class="digg" title="Digg this!"><span>Digg</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.Facebook) %>"
                                class="facebook" title="Share this on Facebook"><span>Facebook</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.Delicious) %>"
                                class="delicious" title="Share this on del.icio.us"><span>Delicious</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.StumbleUpon) %>"
                                class="stumbleupon" title="Stumbled upon something good? Share it on StumbleUpon">
                                <span>StumbleUpon</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.GoogleBookmarks) %>"
                                class="google" title="Add this to Google Bookmarks"><span>Google Bookmarks</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.LinkedIn) %>"
                                class="linkedin" title="Share this on Linkedin"><span>LinkedIn</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.YahooBuzz) %>"
                                class="yahoo" title="Buzz up!"><span>Yahoo Bookmarks</span></a></li>
                            <li><a href="<%= Url.ThemeExtensions().Social.ShareEntryUrl(Model.Entry, Social.SocialNetwork.Techorati) %>"
                                class="technorati" title="Share this on Technorati"><span>Technorati Favorites</span></a></li>
                        </ul>
                    </div>
                    <!-- /socialize -->
                </td>
                <td>
                    <a class="control print">Print article</a>
                </td>
                <td class="details">
                    This entry was posted by
                    <% Html.RenderPartial("AtomPubPeople", Model.Entry.People, new ViewDataDictionary() { { "id", Model.Entry.Id } }); %>
                    on <span class="date">
                        <%= Model.Entry.Date.ToString("MMMM dd, yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)%>
                        at
                        <%= Model.Entry.Date.ToString("hh:mm tt")%></span>
                    <% if (Model.Entry.Categories.Count() > 0)
                       { %>
                    <br />
                    Filed under
                    <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id });
                       } %>.<br />
                    Follow any responses to this post through <a title="RSS 2.0" href="<%= Url.RouteIdUrl("AnnotateEntryAnnotationsFeed", Model.EntryId) %>">
                        RSS 2.0</a>.
                    <br />
                    You can <a href="#respond">leave a response</a>
                </td>
            </tr>
        </table>
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
