<%@ Control Language="C#" Inherits="ViewUserControl<BlogSearchModel>" %>
<div id="searchbox">
			<form action="<%= Model.SearchUrl %>" method="get">
				<div class="content">
					<input type="text" class="textfield" name="term" size="24" value="" />
					<input type="submit" class="button" value="" />
				</div>
			</form>
	</div>