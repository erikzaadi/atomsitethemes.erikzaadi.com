<%@ Control Language="C#" Inherits="ViewUserControl<BlogSearchModel>" %>
<div id="search">
    <form action="<%= Model.SearchUrl %>" id="searchform" method="get">
    <input type="text" name="term" id="s" value="" />
    </form>
</div>
