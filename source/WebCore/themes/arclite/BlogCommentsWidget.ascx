<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h3 class="comments">
    Comments</h3>
<ul id="comments">
    <% if (Model.Feed.Entries.Count() == 0)
       { %>
    <em id="commentsEmpty">None.</em>
    <% }
       foreach (AtomEntry comment in Model.Feed.Entries)
       {
           Html.RenderPartial("BlogComment", new CommentModel() { Comment = comment });
       } %>
</ul>
