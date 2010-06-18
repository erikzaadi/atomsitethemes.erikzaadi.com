<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<%
    var comments = Model.Feed.Entries != null && Model.Feed.Entries.Any()
                       ? Model.Feed.Entries.Where(x => !(x.AnnotationType ?? string.Empty).EndsWith("back"))
                       : new List<AtomEntry>();
    var trackbacks = Model.Feed.Entries != null && Model.Feed.Entries.Any()
                         ? Model.Feed.Entries.Where(x => (x.AnnotationType ?? string.Empty).EndsWith("back"))
                         : new List<AtomEntry>();
%>
<div id="comments">
    <div id="cmtswitcher">
        <a id="commenttab" class="curtab" href="javascript:void(0);" onclick="MGJS.switchTab('thecomments,commentnavi', 'thetrackbacks', 'commenttab', 'curtab', 'trackbacktab', 'tab');">
            Comments (<%=comments.Count()%>)</a> <a id="trackbacktab" class="tab" href="javascript:void(0);"
                onclick="MGJS.switchTab('thetrackbacks', 'thecomments,commentnavi', 'trackbacktab', 'curtab', 'commenttab', 'tab');">
                Trackbacks (<%=trackbacks.Count()%>)</a>
        <%
            if (Model.Collection.AnnotationsOn)
            {%>
        <span class="addcomment"><a href="#respond">Leave a comment</a></span>
        <%
            }%>
        <div class="fixed">
        </div>
    </div>
    <div id="commentlist">
        <ol id="thecomments">
            <%
                if (comments.Any())
                {
                    foreach (AtomEntry comment in comments)
                    {
                        Html.RenderPartial("BlogComment", new CommentModel() { Comment = comment });
                    }
                }
                else
                {%>
            <li class="messagebox">No comments yet. </li>
            <%
                }%>
        </ol>
        <ol id="thetrackbacks">
            <% if (trackbacks.Any())
               {
                   foreach (CommentModel trackback in trackbacks.Select(p => new CommentModel() { Comment = p }))
                   {
                       Html.RenderPartial("BlogTrackback", trackback);
                   }
               }
               else
               {
            %>
            <li class="messagebox">No trackbacks yet. </li>
            <%
                }
            %>
        </ol>
        <div class="fixed">
        </div>
    </div>
</div>