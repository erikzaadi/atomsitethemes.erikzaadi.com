<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<% var isTrackBack = Model.Comment.AnnotationType != null && Model.Comment.AnnotationType.EndsWith("back"); %>
<li id="<%= Model.Comment.Id.ToWebId() %>" class="comment with-avatar <%= Model.IsOwner() ? " admincomment" : "" %><%= !Model.Comment.Approved ? " commentNotApproved" : "" %><%= isTrackBack ? " trackback" : "" %>">
    <!-- comment -->
    <div class="comment-mask">
        <div class="comment-main">
            <div class="comment-wrap1">
                <div class="comment-wrap2">
                    <div class="comment-head tiptrigger">
                        <a class="commentid" name="<%= Model.Comment.Id.ToWebId() %>"></a>
                        <p>
                            <% if (isTrackBack)
                               { %>
                            Trackback
                            <%}
                               else
                               { %>
                            Posted by <span class="commenter">
                                <% Html.RenderPartial("AtomPubPeople", Model.Comment.People);%></span>
                            <%} %>
                            <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date)%></p>
                        <p class="controls tip">
                            <a class="quote"><span>Quote</span></a><a href="#addcomment" class="reply"><span>Reply</span></a>
                            <% if (Model.User.IsAuthenticated)
                               { %>
                            <% if (!Model.Comment.Approved && Model.CanApprove())
                               { %>
                            <a href="#" class="approve" rel="<%= Model.Comment.Id.ToWebId() %>;<%= Url.RouteIdUrl("BlogApproveEntry", Model.Comment.Id)%>">
                                Approve</a>
                            <%} %>
                            <% if (Model.CanDelete())
                               { %>
                            <a href="#" class="delete" rel="<%= Model.Comment.Id.ToWebId() %>;<%= Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id) %>">
                                Delete</a> <a href="mailto:<%= Model.Comment.Authors.First().Email %>">Email</a>
                            <%} %>
                            <%} %>
                        </p>
                    </div>
                    <div class="comment-body clearfix">
                        <%-- Trackback & Pingback View --%>
                        <% if (isTrackBack)
                           { %>
                        <div>
                            <h4>
                                <a href="<%= Model.Comment.Content.Src %>">
                                    <%= Model.Comment.Title%></a></h4>
                            <%= Model.Comment.Text%>
                        </div>
                        <%}
                           else
                           { %>
                        <div class="avatar">
                            <img src="<%= Url.GetGravatarHref(Model.Comment.Authors.First().Email, 64)
                             + Request.Url.GetLeftPart(UriPartial.Authority) 
                             + Url.ImageSrc("noav.png") %>" alt="Gravatar" />
                        </div>
                        <p>
                            <%= Model.Comment.Text.Text%></p>
                        <%} %>
                    </div>
                    <div>
                        <%if (!Model.Comment.Visible && !Model.Comment.Approved)
                          { %>
                        <em class="warning">This comment will not appear until it is approved.</em>
                        <%} %>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- /comment -->
</li>
