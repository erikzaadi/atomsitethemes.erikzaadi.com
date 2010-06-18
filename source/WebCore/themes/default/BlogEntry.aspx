<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<BlogEntryModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title><%= Model.Entry.Title %></title>
  <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id) %>" title="<%= Model.Collection.Title %> Feed" />
  <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id) %>" title="<%= Model.Collection.Title %> Comments Feed" />
  <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateEntryAnnotationsFeed", Model.EntryId) %>" title="<%= Model.Entry.Title %> Comments Feed" />
  <% if (Model.Entry.Summary != null) { %><meta name="description" content="<%=Model.Entry.Summary %>" /> <%} %>
  <%--<% if (User.Identity.IsAuthenticated) { %>
  <script src="<%= Url.ScriptSrc("user.js")%>" type="text/javascript"></script>
  <% } %>--%>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<div id="<%= Model.Entry.Id.ToWebId() %>" class="entry">
    <h3><%= Model.Entry.Title.Text %></h3>
    <address>
      Posted 
      <% if (Model.Entry.Categories.Count() > 0) { %> 
		in <% Html.RenderPartial("BlogCategories", new CategoriesModel() { Categories = Model.Entry.Categories, Id = Model.Collection.Id}); 
	  } %>
      by <% Html.RenderPartial("AtomPubPeople", Model.Entry.People, new ViewDataDictionary() { { "id", Model.Entry.Id } }); %>
      on <span class="date"><%= Html.DateTimeAbbreviation(Model.Entry.Date) %></span>
    </address>
    
    <div class="content"><%--<%= Model.Entry.Text.Text%>
    --%>
      <% Html.RenderContent(Model.Entry.Content); %>
    </div>
</div>
</asp:Content>
<asp:Content ID="SideTop" ContentPlaceHolderID="sidetop" runat="server">
<% if (User.Identity.IsAuthenticated) {%>
    <div id="userActions">
		<% if (Model.CanEdit()) { %>
		<button onclick="location.href = '<%= Url.Action(Model.Entry.Media?"EditMedia":"EditEntry", "Admin", new { id = Model.Entry.Id.ToString() }) %>';">Edit</button>
		<%} %>
		<% if (Model.CanDelete()) { %>
		<button type="button" onclick="del('<%= Model.Entry.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntry", Model.Entry.Id) %>');">Delete</button>
		<%} %>
		<%if (Model.ShowApproveAll()) {%>
	    <button onclick="approveAll('<%= Model.Entry.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveAll", Model.Entry.Id) %>', this)">Approve All</button>
  <%} %>
    </div>
<%} %>

</asp:Content>
<asp:Content ID="SideMid" ContentPlaceHolderID="sidemid" runat="server">
  <div id="updated" class="stat">
      <label for="updated">Last Updated</label>
      <div class="statVal">
          <%= Html.DateTimeAgoAbbreviation(Model.Entry.Updated) %>
      </div>
  </div>
</asp:Content>
