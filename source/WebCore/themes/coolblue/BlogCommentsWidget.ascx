<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<div class="post-bottom-section">
    <div id="comments">
        <h4>
            <%=Model.Feed.Entries.Count()%>
            comments</h4>
        <%
            if (Model.Feed.Entries.Count() == 0)
            {%>
        <em id="commentsEmpty">None.</em>
        <%
            }
            else {%>
        <div class="right">
            <ol class="commentlist">
                <%
                foreach (AtomEntry comment in Model.Feed.Entries)
                {
                    Html.RenderPartial("BlogComment", new CommentModel() {Comment = comment});
                } %>
            </ol>
        </div>
        <%
            }%>
    </div>
</div>
