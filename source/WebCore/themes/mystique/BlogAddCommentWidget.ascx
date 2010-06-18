<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AddCommentModel>" %>
<!-- comment form -->
<div id="addcomment">
    <div class="comment-form clearfix" id="respond">
        <% if (Model.AnnotationState == AnnotationState.On)
           { %>
        <form id="commentform" method="post" action="<%= Url.RouteIdUrl("BlogPostComment", Model.EntryId) %>">
        <% if (Model.User.IsAuthenticated)
           { %>
        <p>
            <%= Html.GravatarImg(Model.User.Email, 20)%>
            Logged in as <strong>
                <%= Model.User.Name%></strong>
            <input type="hidden" id="authenticated" value="" />
        </p>
        <%}
           else
           {%>
        <% Html.RenderSubWidgets("BlogAddCommentWidget", "commentator"); %>
        <div class="clearfix">
        </div>
        <div id="author-info">
            <div class="row">
                <input type="text" name="txtName" id="field-author" class="validate required textfield clearField"
                    value="Name (required)" size="40" />
            </div>
            <div class="row">
                <input type="text" name="txtEmail" id="field-email" class="validate required textfield clearField"
                    value="E-mail (required, will not be published)" size="40" />
            </div>
            <div class="row">
                <input type="text" name="txtWebsite" id="field-url" class="textfield clearField"
                    value="Website" size="40" />
            </div>
        </div>
        <%
            }%>
        <!-- comment input -->
        <div class="row">
            <textarea name="txtComment" id="comment" class="validate required" rows="8" cols="50"></textarea>
        </div>
        <!-- /comment input -->
        <!-- comment submit and rss -->
        <div id="submitbox">
            <input name="submit" type="submit" id="submit" class="button" value="Submit Comment" />
            <input type="hidden" name="formInput" />
        </div>
        </form>
        <%}
           else
           {%>
        <div id="comment_login" class="messagebox">
            <p>
                <%
                    if (Model.AnnotationState == AnnotationState.Closed || Model.AnnotationState == AnnotationState.Expired)
                    {%>
                Comments are closed.
                <%}
                else if (Model.AnnotationState == AnnotationState.Unauthorized)
                {%>
                You must be <a href="<%=Url.Action("Login", "Account")%>?ReturnUrl=<%=Server.UrlEncode(Page.Request.Url.PathAndQuery)%>">
                    logged in</a> to comment.
                <%}%>
            </p>
        </div>
        <%    }%>
    </div>
</div>
