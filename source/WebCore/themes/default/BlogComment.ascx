<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<div id="<%= Model.Comment.Id.ToWebId() %>" class="comment<%= Model.IsOwner() ? " commentOwner" : "" %><%= !Model.Comment.Approved ? " commentNotApproved" : "" %>">
	
	<% if (Model.User.IsAuthenticated) { %>
	<div class="admin">
		<% if (!Model.Comment.Approved && Model.CanApprove()) { %>
		<button onclick="approve('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveEntry", Model.Comment.Id)%>', this)">Approve</button>
		<%} %>
		<% if (Model.CanDelete()) { %>
		<button onclick="del('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id) %>')">Delete</button>
		<button onclick="location.href = 'mailto:<%= Model.Comment.Authors.First().Email %>'">Email</button>
		<%} %>
	</div>
	<%} %>
	
	<%-- Trackback & Pingback View --%>
	<% if (Model.Comment.AnnotationType != null && Model.Comment.AnnotationType.EndsWith("back")) { %>
		<h5>Trackback <% if (!string.IsNullOrEmpty(Model.Comment.Authors.First().Name))
                   { %>from <%= Model.Comment.Authors.First().Name%><%} %>&#160;<%= Html.DateTimeAgoAbbreviation(Model.Comment.Date) %> </h5>
		<div>
	        <h4><a href="<%= Model.Comment.Content.Src %>"><%= Model.Comment.Title %></a></h4>
	        <%= Model.Comment.Text%>
	    </div>
	<%-- Comment View --%>
	<%} else { %>
	    <%= Html.GravatarImg(Model.Comment.Authors.First().Email, 80)%>
	    <h5>Posted by <% Html.RenderPartial("AtomPubPeople", Model.Comment.People);%> <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date)%></h5>
	    <div><%= Model.Comment.Text.Text%></div>
	<%} %>	
	
	<%if (!Model.Comment.Visible && !Model.Comment.Approved)
   { %>
		<em class="warning">This comment will not appear until it is approved.</em>
	<%} %>
</div>
