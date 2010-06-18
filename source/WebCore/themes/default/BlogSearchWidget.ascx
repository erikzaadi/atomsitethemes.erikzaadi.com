<%@ Control Language="C#" Inherits="ViewUserControl<BlogSearchModel>" %>
<div id="search">
<form action="<%= Model.SearchUrl %>" method="get">
	<fieldset>
		<input type="text" name="term" value="" />
		<input type="submit" value="Go" />
	</fieldset>
</form></div>