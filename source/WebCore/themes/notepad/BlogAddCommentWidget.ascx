﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AddCommentModel>" %>
<br />
<div id="addcomment">
    <% if (Model.AnnotationState == AnnotationState.On)
       { %>
    <h4>
        Add Comment</h4>
    <form id="addcommentform" method="post" action="<%= Url.RouteIdUrl("BlogPostComment", Model.EntryId) %>">
    <%----%>
    <% if (Model.User.IsAuthenticated)
       { %>
    <fieldset id="userDetails">
        <%= Html.GravatarImg(Model.User.Email, 20)%>
        <strong>
            <%= Model.User.Name%></strong>
        <input type="hidden" id="authenticated" value="" />
    </fieldset>
    <%}
       else
       { //TODO: check if anon comments are on %>
    <% Html.RenderSubWidgets("BlogAddCommentWidget", "commentator"); %>
    <fieldset id="anonDetails">
        <label for="txtName">
            Name</label><span id="nameError" class="error">Please enter your name.</span>
        <input type="text" id="txtName" name="txtName" value="<%= Model.AnonAuthor.Name %>" />
        <label for="txtEmail">
            Email <small>(never displayed)</small></label><span id="emailError" class="error">Please
                enter a valid email.</span>
        <input type="text" id="txtEmail" name="txtEmail" value="<%= Model.AnonAuthor.Email %>" />
        <label for="txtWebsite">
            Website</label><span id="websiteError" class="error">Please enter a valid website.</span>
        <input type="text" id="txtWebsite" name="txtWebsite" value="<%= Model.AnonAuthor.Uri != null ? Model.AnonAuthor.Uri.ToString() : "http://" %>" />
    </fieldset>
    <%} %>
    <p>
        <textarea tabindex="4" rows="10" cols="58" id="txtComment" name="txtComment"></textarea></p>
    <%--<br class="clear" />
		<label for="chkNotify" class="notify">Notify me when new comments are added</label><input id="chkNotify" type="checkbox" name="chkNotify" />
		<br class="clear" />  --%>
    <input type="submit" value="Submit Comment" id="submit" name="submit" />
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
