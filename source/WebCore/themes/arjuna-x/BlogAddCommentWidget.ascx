<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AddCommentModel>" %>
<br />
<div id="addcomment" class="commentReply">
    <% if (Model.AnnotationState == AnnotationState.On)
       { %>
    <div class="replyHeader">
        <h4>
            Leave a Comment</h4>
    </div>
    <form id="addcommentform" method="post" action="<%= Url.RouteIdUrl("BlogPostComment", Model.EntryId) %>">
    <%----%>
    <% if (Model.User.IsAuthenticated)
       { %>
    <div class="replyLoggedIn">
        <%= Html.GravatarImg(Model.User.Email, 20)%>
        <strong>
            <%= Model.User.Name%></strong>
        <input type="hidden" id="authenticated" value="" />
    </div>
    <%}
       else
       { //TODO: check if anon comments are on %>
    <% Html.RenderSubWidgets("BlogAddCommentWidget", "commentator"); %>
    <div class="alignright ">
        <div class="replyRow">
            <span id="nameError" class="error">Please enter your name.</span>
            <input type="text" class="inputText arjuna-watermark" rel="inputIA" title="Enter name.." id="txtName"
                name="txtName" value="<%= Model.AnonAuthor.Name %>" />
        </div>
        <div class="replyRow">
            <span id="emailError" class="error">Please enter a valid email.</span>
            <input type="text" class="inputText arjuna-watermark" rel="inputIA" id="txtEmail" title="Enter email.."
                name="txtEmail" value="<%= Model.AnonAuthor.Email %>" />
        </div>
        <div class="replyRow">
            <span id="websiteError" class="error">Please enter a valid website.</span>
            <input type="text" class="arjuna-watermark inputText" rel="inputIA" title="Enter Website.." id="txtWebsite"
                name="txtWebsite" value="<%= Model.AnonAuthor.Uri != null ? Model.AnonAuthor.Uri.ToString() : "http://" %>" />
        </div>
    </div>
    <%} %>
    <div class="replyRow">
        <textarea class="arjuna-watermark" title="Enter comment.." rel="inputIA"" name="txtComment" rows="8" cols="20" id="txtComment" style="height: 8em;"></textarea>
        <%--<br class="clear" />
		<label for="chkNotify" class="notify">Notify me when new comments are added</label><input id="chkNotify" type="checkbox" name="chkNotify" />
		<br class="clear" />  --%>
    </div>
    <div class="replySubmitArea">
        <a href="<%= Url.RouteIdUrl("AnnotateEntryAnnotationsFeed", Model.EntryId) %>" class="btnSubscribe btn">
            <span>Subscribe to comments</span></a>
        <input type="submit" class="inputBtn" value="Leave Comment" /><div id="commentError"
            class="error">
            Please supply a comment.</div>
    </div>
    </form>
    <% }
       else if (Model.AnnotationState == AnnotationState.Closed || Model.AnnotationState == AnnotationState.Expired)
       { %>
    Comments are closed.
    <%}
       else if (Model.AnnotationState == AnnotationState.Unauthorized)
       { %>
    You must be <a href="<%= Url.Action("Login", "Account") %>?ReturnUrl=<%= Server.UrlEncode(Page.Request.Url.PathAndQuery) %>">
        logged in</a> to comment.
    <%} %>
</div>
