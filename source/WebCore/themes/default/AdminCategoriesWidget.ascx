<%@ Control Language="C#" Inherits="ViewUserControl<AdminCategoriesModel>" %>
<div class="widget settings area categories">
  <h3>Categories</h3>
  <div class="tool"><label for="scheme">Scheme</label>&#160;<%--<%= Html.DropDownList("scheme", Model.Schemes.Select(s => new SelectListItem() { Text = s.Key, Value = s.Value, Selected = Model.IsSelected(s.Key) }))%>--%>
  <select name="scheme">
  <% foreach (var s in Model.Schemes) { %>
  <option value="<%= s.Value %>" <%= Model.IsSelected(s.Key) ? "selected=\"selected\"" : string.Empty %>><%= s.Key %></option>
  <%} %></select>
  </div>
  <% if (Model.Categories == null)
     { %><ul><em>No categories</em></ul><%}
     else
     { %>
  <ul>
  <% bool alt = false; foreach (AtomCategory cat in Model.Categories.Categories)
     {
       alt = !alt; %>
  <li class="<%= alt ? "alt" : string.Empty %>" id="cat-<%=cat.Term %>"><% if (!Model.Categories.Fixed ?? false)
                                                                           { %>
    <a rel="remove" href="<%= Url.Action("RemoveCategory", "Admin", new { workspace = Model.Scope.Workspace, collection = Model.Scope.Collection, term = cat.Term, scheme = cat.Scheme }) %>">remove</a>
  <%} %><%= cat.ToString()%>
  </li>
  <%} %>
  </ul><%} %>
  <%--
  <div>
    <div>
      <input type="checkbox" name="fixed" <%=Model.Collection.Categories.Where(c => c.Scheme == null).First().Fixed ?? false ? "checked=\"checked\"" : string.Empty %> /> <label for="fixed">Fixed</label>    
    </div>
    <input type="text" name="categoryName" title="Category Name" value="Category Name" />
    <button name="addCategory">Add</button>
  </div>--%>
</div>