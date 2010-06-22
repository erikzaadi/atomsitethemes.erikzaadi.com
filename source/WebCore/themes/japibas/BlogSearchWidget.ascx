<%@ Control Language="C#" Inherits="ViewUserControl<BlogSearchModel>" %>
<form action="<%= Model.SearchUrl %>" method="get" class="searchform">
<input type="text" name="term" value="Search..."  onclick="if(this.value=='Search...')this.value='';" onblur="if(this.value=='')this.value='Search...';" />
<input type="submit" value="Search" />
</form>
