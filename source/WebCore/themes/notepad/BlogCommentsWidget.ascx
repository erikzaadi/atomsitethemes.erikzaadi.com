<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h4 id="comments">
    <%
        var commentString = string.Empty;
        var count = Model.Feed.Entries.Count();
        switch (count)
        {
            case 0:
                commentString = "No Comments";
                break;
            case 1:
                commentString = "1 Comment";
                break;
            default:
                commentString = string.Format("{0} Comments", count);
                break;
        }
	                    
    %>
    <%= commentString %><em> (<a href="#addcomment">+add yours?</a>)</em></h4>
<ol class="commentlist">
    <% 
        foreach (AtomEntry comment in Model.Feed.Entries)
        {
            Html.RenderPartial("BlogComment", new CommentModel() { Comment = comment });
        } %>
</ol>
