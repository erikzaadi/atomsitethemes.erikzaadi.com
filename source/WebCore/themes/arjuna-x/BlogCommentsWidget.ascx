<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<div class="postComments" id="comments">
    <div class="commentHeader">
        <h4>
            Comments</h4>
        <a href="#comments" class="btnReply btn"><span>Leave a comment</span></a>
    </div>
    <% if (Model.Feed.Entries.Count() == 0)
       { %>
    <em id="commentsEmpty">None.</em>
    <% }
       else
       {%>
    <ul class="commentList<%= Model.PageWidth.Contains("left") ? " commentListLeft" : " commentListAlt"%>">
        <%
            foreach (AtomEntry comment in Model.Feed.Entries)
            {
                Html.RenderPartial("BlogComment", new CommentModel() { Comment = comment });
            } %>
    </ul>
    <%} %>
</div>
