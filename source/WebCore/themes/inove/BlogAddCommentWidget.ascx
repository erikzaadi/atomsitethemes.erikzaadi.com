<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AddCommentModel>" %>
<div id="respond">
    <div id="addcomment">
        <% if (Model.AnnotationState == AnnotationState.On)
           { %>
        <form id="addcommentform" method="post" action="<%= Url.RouteIdUrl("BlogPostComment", Model.EntryId) %>">
        <%----%>
        <% if (Model.User.IsAuthenticated)
           { %>
        <%= Html.GravatarImg(Model.User.Email, 20)%>
        <strong>
            <%= Model.User.Name%></strong>
        <input type="hidden" id="authenticated" value="" />
        <%}
           else
           { //TODO: check if anon comments are on %>
        <div>
            <% Html.RenderSubWidgets("BlogAddCommentWidget", "commentator"); %>
        </div>
        <div class="fixed">
        </div>
        <div>
            &nbsp;
        </div>
        <div id="author_info">
            <div class="row">
                <span id="nameError" class="error">Please enter your name.</span>
                <input type="text" id="txtName" size="24" class="textfield" name="txtName" value="<%= Model.AnonAuthor.Name %>" />
                <label for="txtName" class="small">
                    Name (required)</label>
            </div>
            <div class="row">
                <span id="emailError" class="error">Please enter a valid email.</span>
                <input type="text" id="txtEmail" name="txtEmail" size="24" class="textfield" value="<%= Model.AnonAuthor.Email %>" />
                <label for="txtEmail" class="small">
                    Email <small>(will not be published)</small></label>
            </div>
            <div class="row">
                <span id="websiteError" class="error">Please enter a valid website.</span>
                <input type="text" id="txtWebsite" name="txtWebsite" size="24" class="textfield"
                    value="<%= Model.AnonAuthor.Uri != null ? Model.AnonAuthor.Uri.ToString() : "http://" %>" /><label
                        for="txtWebsite" class="small">
                        Website</label>
            </div>
        </div>
        <%} %>
        <div class="row">
            <textarea name="txtComment" id="txtComment" tabindex="4" rows="8" cols="50"></textarea>
        </div>
        <%--<br class="clear" />
		<label for="chkNotify" class="notify">Notify me when new comments are added</label><input id="chkNotify" type="checkbox" name="chkNotify" />
		<br class="clear" />  --%>
        <div id="submitbox">
            <a class="feed" href="<%= Url.RouteIdUrl("AnnotateEntryAnnotationsFeed", Model.EntryId) %>">
                Subscribe to comments feed</a>
            <div class="submitbutton">
                <input name="submit" type="submit" id="submit" class="button" tabindex="5" value="Submit Comment" />
            </div>
            <div class="fixed">
            </div>
        </div>
        <div id="commentError" class="error">
            Please supply a comment.</div>
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
</div>
