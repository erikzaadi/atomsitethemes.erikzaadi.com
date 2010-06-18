<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<li class="comment <%= Model.IsOwner() ? "admincomment" : "regularcomment" %>" id="comment-<%= Model.Comment.Id.ToWebId() %>">
    <div class="author">
        <div class="pic">
            <img alt='' class="avatar avatar-32 photo avatar-default" src='<%= Url.GetGravatarHref(Model.Comment.Authors.First().Email, 32)
                             + Request.Url.GetLeftPart(UriPartial.Authority) 
                             + Url.ImageSrc("noav.png") %>' height='32' width='32' />
        </div>
        <div class="name">
            <%
            var author = Model.Comment.People.Where(p => p.Uri != null).Take(1).First(); %>
            <a rel="external nofollow" href="<%= author.Uri.ToString() %>" class="url" id="commentauthor-<%= Model.Comment.Id.ToWebId() %>">
                <%= author.Name %>
            </a>
        </div>
    </div>
    <div class="info">
        <div class="date">
            <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date)%>
            | <a href="#comment-<%= Model.Comment.Id.ToWebId() %>">
                <%= Model.Comment.Id.ToWebId() %></a>
        </div>
        <div class="act">
            <a href="javascript:void(0);" onclick="MGJS_CMT.reply('commentauthor-<%= Model.Comment.Id.ToWebId() %>', 'comment-<%= Model.Comment.Id.ToWebId() %>', 'txtComment');">
                Reply</a> | <a href="javascript:void(0);" onclick="MGJS_CMT.quote('commentauthor-<%= Model.Comment.Id.ToWebId() %>', 'comment-<%= Model.Comment.Id.ToWebId() %>', 'commentbody-<%= Model.Comment.Id.ToWebId() %>', 'txtComment');">
                    Quote</a>
            <% if (Model.User.IsAuthenticated)
               { %>
            <% if (!Model.Comment.Approved && Model.CanApprove())
               { %>
            | <a href="javascript:void(0);" onclick="approve('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveEntry", Model.Comment.Id)%>', this);">
                Approve</a>
            <%} %>
            <% if (Model.CanDelete())
               { %>
            | <a href="javascript:void(0);" onclick="del('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id) %>')">
                Delete</a> | <a href="mailto:<%= Model.Comment.Authors.First().Email %>">Email</a>
            <%} %>
            <%} %>
        </div>
        <div class="fixed">
        </div>
        <div class="content">
            <%if (!Model.Comment.Visible && !Model.Comment.Approved)
              {%>
            <p>
                <small>Your comment is awaiting moderation.</small></p>
            <%
                }
              else
              {%>
            <div id="commentbody-<%= Model.Comment.Id.ToWebId() %>">
                <%= Model.Comment.Text.Text%>
            </div>
            <%
                }%>
        </div>
    </div>
    <div class="fixed">
    </div>
</li>
