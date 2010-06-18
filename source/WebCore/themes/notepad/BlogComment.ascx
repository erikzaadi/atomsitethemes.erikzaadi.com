<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<% var isTrackBack = Model.Comment.AnnotationType != null && Model.Comment.AnnotationType.EndsWith("back"); %>
<li>
    <p class="comment-author">
        <img height="42" width="42" src="<%= Url.GetGravatarHref(Model.Comment.Authors.First().Email, 42)
                             + Request.Url.GetLeftPart(UriPartial.Authority) 
                             + Url.ImageSrc("noav.png") %>" alt="Gravatar" class="avatar avatar-42 photo<%= Model.IsOwner() ? " avatar-owner" : "" %>" />
        <% if (isTrackBack)
           { %>
        Trackback
        <%}
           else
           {
               if (Model.IsOwner())
               {%>
        <span class="postauthor">&nbsp;&nbsp;&nbsp;&nbsp;<em>
            <%} %>
            <% Html.RenderPartial("AtomPubPeople", Model.Comment.People);%>
            <%  if (Model.IsOwner())
                {%>
        </em></span>
        <%} 
        %>
        <%} %><br />
        <small>
            <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date)%></small>
    </p>
    <div class="commententry" id="commententry-<%= Model.Comment.Id.ToWebId() %>">
        <p>
            <%if (!Model.Comment.Visible && !Model.Comment.Approved)
              { %>
            <em>Your comment is awaiting moderation.</em>
            <%} %>
        </p>
        <% if (isTrackBack)
           { %>
        <a href="<%= Model.Comment.Content.Src %>">
            <%= Model.Comment.Title%></a>
        <%= Model.Comment.Text%>
        <%}
           else
           { %>
        <%= Model.Comment.Text.Text%>
        <%} %>
    </div>
    <% if (Model.User.IsAuthenticated)
       {%>
    <div class="admin">
        <%
            if (!Model.Comment.Approved && Model.CanApprove())
            {%>
        <button onclick="approve('<%=Model.Comment.Id.ToWebId()%>', '<%=Url.RouteIdUrl("AtomPubApproveEntry", Model.Comment.Id)%>', this)">
            Approve</button>
        <%
            }%>
        <%
            if (Model.CanDelete())
            {%>
        <button onclick="del('<%=Model.Comment.Id.ToWebId()%>', '<%=Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id)%>')">
            Delete</button>
        <button onclick="location.href = 'mailto:<%=Model.Comment.Authors.First().Email%>'">
            Email</button>
        <%
            }%>
    </div>
    <%
        }%>
</li>
