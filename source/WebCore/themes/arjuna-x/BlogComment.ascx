<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<% var isTrackBack = Model.Comment.AnnotationType != null && Model.Comment.AnnotationType.EndsWith("back"); %>
<li class="comment<%= Model.IsOwner() ? " thread-odd" : "" %>" id="comment-<%= Model.Comment.Id.ToWebId() %>">
    <img alt='' src='<%= Url.GetGravatarHref(Model.Comment.Authors.First().Email, 40)
                             + Request.Url.GetLeftPart(UriPartial.Authority) 
                             + Url.ImageSrc("noav.png") %>' class='avatar avatar-40 photo avatar-default'
        height='40' width='40' />
    <div class="message">
        <div class="t">
            <div>
            </div>
        </div>
        <div class="i">
            <div class="i2">
                <span class="title">
                    <% if (isTrackBack)
                       { %>
                    Trackback
                    <%}
                       else
                       { %>
                    Written by
                    <% Html.RenderPartial("AtomPubPeople", Model.Comment.People);%>
                    <%} %>
                    <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date)%></span> <span class="links">
                </span>
                <div id="commentbody-<%= Model.Comment.Id.ToWebId() %>" class="commentmain">
                    <p>
                        <%if (!Model.Comment.Visible && !Model.Comment.Approved)
                          { %>
                        <em>Your comment is awaiting moderation.</em>
                        <%} %>
                    </p>
                    <p>
                        <% if (isTrackBack)
                           { %>
                        <a href="<%= Model.Comment.Content.Src %>">
                            <%= Model.Comment.Title%></a>
                        <%= Model.Comment.Text%>
                        <%}
                           else
                           { %>
                        <%= Model.Comment.Text.Text%>
                        <%} %></p>
                </div>
            </div>
        </div>
        <div class="b">
            <div>
            </div>
        </div>
    </div>
    <% if (Model.User.IsAuthenticated)
       { %>
    <% if (!Model.Comment.Approved && Model.CanApprove())
       { %>
    <button onclick="approve('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveEntry", Model.Comment.Id)%>', this)">
        Approve</button>
    <%} %>
    <% if (Model.CanDelete())
       { %>
    <button onclick="del('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id) %>')">
        Delete</button>
    <button onclick="location.href = 'mailto:<%= Model.Comment.Authors.First().Email %>'">
        Email</button>
    <%} %>
    <%} %>
</li>
