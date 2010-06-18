<%@ Control Language="C#" Inherits="ViewUserControl<AdminCollCountModel>" %>
<div class="widget settings area collections">
  <h3><a rel="add" href="<%= Url.Action("NewCollection", "Admin", new { workspace = Model.Scope.Workspace }) %>" title="Add New Collection">New Collection</a>
 Collections</h3>
  <ul>
  <%bool alt = false;  foreach (AppCollection collection in Model.Workspace.Collections)
    { alt = !alt; %>
    <li class="<%= alt ? "alt " : string.Empty %><%= collection.Default ? "default" : "notdefault" %>" id="<%= collection.Id.Collection %>">
    <%if (collection.Default) { %>
    <img src="<%=Url.ImageSrc("asterik.png") %>" alt="default" width="24" height="24" />
      <%} else { %>
      <a rel="default" href="<%= Url.Action("SetDefaultCollection", "Admin", new { workspace = Model.Scope.Workspace, collection = collection.Id.Collection }) %>" title="Set collection as default"><img src="<%=Url.ImageSrc("asterikW.png") %>" alt="not default" width="24" height="24" /></a>
      <%} %>
      <div class="actions"><a title="Edit Collection" href="<%= Url.Action("Settings", "Admin", new { workspace = Model.Scope.Workspace, collection = collection.Id.Collection }) %>">edit</a> <a title="Delete Collection" href="<%= Url.Action("DeleteCollection", "Admin", new { workspace = Model.Scope.Workspace, collection = collection.Id.Collection }) %>">delete</a></div>
    <h4><a href="<%= Url.Action("Settings", "Admin", new { workspace = Model.Scope.Workspace, collection = collection.Id.Collection }) %>"><%=collection.Title%></a></h4>
      <div><strong><%= collection.Id.Collection %></strong> <%= collection.GetInt32Property("count") %>&#160;<small>items</small> <%= collection.Authors.Count() %>&#160;<small>authors</small> <%= collection.Contributors.Count() %>&#160;<small>contributors</small></div> 
    </li><%} %>
  </ul>
</div>