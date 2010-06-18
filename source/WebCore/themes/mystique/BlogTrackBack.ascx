<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<li class="ping" id="<%= Model.Comment.Id.ToWebId() %>"><a class="websnapr" href="<%= Model.Comment.Content.Src %>"
    rel="nofollow">
    <%= Model.Comment.Title%></a> </li>
