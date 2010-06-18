<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<% var isTrackBack = Model.Comment.AnnotationType != null && Model.Comment.AnnotationType.EndsWith("back"); %>
<li class="depth-1<%= Model.IsOwner() ? " thread-alt" : "" %>">
    <div class="comment-info">
        <% if (!isTrackBack)
           {%>
        <%=Html.GravatarImg(Model.Comment.Authors.First().Email, 40)%>
        <%
            } %>
        <cite>
            <% if (isTrackBack)
               {%>
            Trackback
            <% if (!string.IsNullOrEmpty(Model.Comment.Authors.First().Name))
               { %>from
            <%= Model.Comment.Authors.First().Name%><%} %>
            <%
                }
               else
               {%>
            <% Html.RenderPartial("AtomPubPeople", Model.Comment.People);%>
            Says:
            <br />
            <%
                }%>
            <span class="comment-data"><a href="#comment-<%= Model.Comment.Id.ToWebId() %>" title="">
                <%= Html.DateTimeAgoAbbreviation(Model.Comment.Date) %></a></span> </cite>
    </div>
    <div class="comment-text">
        <% if (!isTrackBack)
           {%>
        <p>
            <%= Model.Comment.Text.Text%>
        </p>
        <div class="reply">
            <a rel="nofollow" class="comment-reply-link" href="#addcomment">Reply</a>
        </div>
        <%
            }
           else
           {%>
            <a href="<%= Model.Comment.Content.Src %>">
                <%= Model.Comment.Title %></a>
        <%= Model.Comment.Text%>
        <%
            }%>
              <div>
                        <%if (!Model.Comment.Visible && !Model.Comment.Approved)
                          { %>
                        <em class="warning">This comment will not appear until it is approved.</em>
                        <%} %>
                    </div>
    </div>
</li>
