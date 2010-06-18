<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<!-- comment entry -->
<li class="comment withAvatars" id="comment-<%= Model.Comment.Id.ToWebId() %>">
    <div class="comment-head comment withAvatars <%= Model.IsOwner() ? " comment-author-admin" : "" %>">
        <div class="avatar-box">
            <img height="48" width="48" src="<%= Url.GetGravatarHref(Model.Comment.Authors.First().Email, 48)
                             + Request.Url.GetLeftPart(UriPartial.Authority) 
                             + Url.ImageSrc("noav.png") %>" alt="Gravatar" class="avatar avatar-48 photo" /></div>
        <div class="author">
            <span class="by">written by
                <% Html.RenderPartial("AtomPubPeople", Model.Comment.People);%>
            </span>
            <br />
            <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date)%>
        </div>
        <div class="controls bubble">
            <a class="reply" id="reply-to-<%= Model.Comment.Id.ToWebId() %>" href="#respond">Reply</a>
            <a class="quote" title="Quote" href="#respond">Quote</a>
            <% if (Model.User.IsAuthenticated)
               { %>
            <% if (!Model.Comment.Approved && Model.CanApprove())
               { %>
            <a href="javascript:approve('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveEntry", Model.Comment.Id)%>', this);return false;">
                Approve</a>
            <%} %>
            <% if (Model.CanDelete())
               { %>
            <a href="javascript:del('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id) %>')">
                Delete</a> <a href="mailto:<%= Model.Comment.Authors.First().Email %>">Email</a>
            <%} %>
            <%} %>
        </div>
    </div>
    <div class="comment-body clearfix" id="comment-body-<%= Model.Comment.Id.ToWebId() %>">
        <%if (!Model.Comment.Visible && !Model.Comment.Approved)
          { %><p class="error">
              Your comment is awaiting moderation.</p>
        <%} %>
        <div class="comment-text">
            <%= Model.Comment.Text.Text%></div>
        <a id="comment-reply-<%= Model.Comment.Id.ToWebId() %>"></a>
    </div>
</li>
