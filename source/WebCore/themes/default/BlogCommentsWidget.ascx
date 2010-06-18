<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<div id="comments">
	<h3>Comments</h3>
	<% if (Model.Feed.Entries.Count() == 0) { %>
	<em id="commentsEmpty">None.</em>
  <% }
  foreach (AtomEntry comment in Model.Feed.Entries) {
	  Html.RenderPartial("BlogComment", new CommentModel() { Comment = comment });
	} %>
</div>
