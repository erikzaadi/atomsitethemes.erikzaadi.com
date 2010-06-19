<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<%@ Import Namespace="ThemeExtensions.HtmlHelpers" %>

<%
    var comments = Html.ThemeExtensions().Entries.GetCommentsWithoutTrackBacks(Model);
    var trackbacks = Html.ThemeExtensions().Entries.GetTrackBacks(Model);
%>
<!-- tabbed content -->
<div class="tabbed-content post-tabs clearfix" id="post-tabs">
    <!-- tab navigation (items must be in reverse order because of the tab-design) -->
    <div class="tabs-wrap clearfix">
        <ul class="tabs">
            <%  if (trackbacks.Any())
                {%><li class="trackbacks"><a href="#section-trackbacks"><span>Trackbacks (<%= trackbacks.Count() %>)</span></a></li><%} %>
            <% if (comments.Any())
               { %><li class="comments"><a href="#section-comments"><span>Comments (<%= comments.Count()%>)</span></a></li><%} %>
        </ul>
    </div>
    <!-- /tab nav -->
    <!-- tab sections -->
    <div class="sections">
        <div class="section clearfix" id="section-comments">
            <% if (comments.Any())
               { %>
            <div id="comments-wrap">
                <div class="clearfix">
                    <ul id="comments" class="comments">
                        <% var x = 0; foreach (var comment in comments)
                           {
                               Html.RenderPartial("BlogComment", new CommentModel() { Comment = comment });
                           } %>
                    </ul>
                </div>
            </div>
            <%}
               else
               { %>
            <h6 class="title">
                No comments yet.</h6>
            <%} %>
        </div>
        <!-- trackbacks -->
        <div class="section" id="section-trackbacks">
            <%  if (trackbacks.Any())
                {%>
            <ul id="trackbacks">
                <%  foreach (var trackback in trackbacks)
                    {
                        Html.RenderPartial("BlogTrackBack", new CommentModel() { Comment = trackback });
                    } %>
            </ul>
            <%}
                else
                { %>
            <h6 class="title">
                No trackbacks yet</h6>
            <%} %>
        </div>
        <!-- /trackbacks -->
    </div>
</div>
