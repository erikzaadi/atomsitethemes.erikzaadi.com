<%@ Control Language="C#" Inherits="ViewUserControl<BaseModel>" %>
<div class="widget" id="addCategoriesWidget">
<h3>Category</h3>
<% using (Html.BeginForm("AddCategory", "Admin"))
   { %>
<label for="categoryName">Category Name</label>
<%= Html.TextBox("categoryName")%>
<button type="submit">Add</button>
<%} %>
</div>