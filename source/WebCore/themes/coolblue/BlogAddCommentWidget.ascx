<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AddCommentModel>" %>
<br />
<div id="addcomment">
    <% if (Model.AnnotationState == AnnotationState.On)
       { %>
    <h4>
        Leave a Reply</h4>
    <div class="right">
        <form id="addcommentform" method="post" action="<%= Url.RouteIdUrl("BlogPostComment", Model.EntryId) %>">
        <%----%>
        <% if (Model.User.IsAuthenticated)
           { %>
        <p>
            <%= Html.GravatarImg(Model.User.Email, 20)%>
            <br />
            Logged in as <strong>
                <%= Model.User.Name%></strong>
            <input type="hidden" id="authenticated" value="" />
        </p>
        <%}
           else
           { //TODO: check if anon comments are on %>
        <div class="clear">
            <% Html.RenderSubWidgets("BlogAddCommentWidget", "commentator"); %>
        </div>
        <p>
            <label for="txtName">
                Name (required)</label><br />
            <span id="nameError" class="error">Please enter your name.</span>
            <input tabindex="1" type="text" id="txtName" name="txtName" value="<%= Model.AnonAuthor.Name %>" />
        </p>
        <p>
            <label for="txtEmail">
                Email Address (required)</label><br />
            <span id="emailError" class="error">Please enter a valid email.</span>
            <input type="text" tabindex="2" id="txtEmail" name="txtEmail" value="<%= Model.AnonAuthor.Email %>" />
        </p>
        <p>
            <label for="txtWebsite">
                Website</label><br />
            <span id="websiteError" class="error">Please enter a valid website.</span>
            <input type="text" tabindex="3" id="txtWebsite" name="txtWebsite" value="<%= Model.AnonAuthor.Uri != null ? Model.AnonAuthor.Uri.ToString() : "http://" %>" />
        </p>
        <%} %>
        <p>
            <label for="txtComment">
                Your Message</label><br />
            <textarea name="txtComment" rows="10" cols="20" tabindex="4" id="txtComment" style="height: 8em;"></textarea>
        </p>
        <%--<br class="clear" />
		<label for="chkNotify" class="notify">Notify me when new comments are added</label><input id="chkNotify" type="checkbox" name="chkNotify" />
		<br class="clear" />  --%>
        <p class="no-border">
            <input class="button" type="submit" value="Submit Comment" tabindex="5" />
        </p>
        <div id="commentError" class="error">
            Please supply a comment.</div>
        </form>
    </div>
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
