<%@ Control Language="C#" Inherits="ViewUserControl<AdminModel>" %>
<div class="widget settings area workspaces">
  <h3><% if (Model.Service.ServiceType != ServiceType.Single) { %>
    <a rel="add" href="<%= Url.Action("NewWorkspace", "Admin") %>" title="Add New Workspace">New Workspace</a>
  <%} %>Workspaces</h3>
  <ul>
  <%bool alt = false;  foreach (AppWorkspace workspace in Model.Service.Workspaces)
    { alt = !alt; %>
    <li class="<%= alt ? "alt " : string.Empty %><%= workspace.Default ? "default" : "notdefault" %>" id="<%= string.IsNullOrEmpty(workspace.Name) ? Atom.DefaultWorkspaceName : workspace.Name %>">
    <%if (workspace.Default) { %>
    <img src="<%=Url.ImageSrc("asterik.png") %>" alt="default" width="24" height="24" />
      <%} else { %>
      <a rel="default" href="<%= Url.Action("SetDefaultWorkspace", "Admin", new { workspace = string.IsNullOrEmpty(workspace.Name) ? Atom.DefaultWorkspaceName : workspace.Name }) %>" title="Set workspace as default"><img src="<%=Url.ImageSrc("asterikW.png") %>" alt="not default" width="24" height="24" /></a>
      <%} %>
      <div class="actions"><a title="Edit Workspace" href="<%= Url.Action("Settings", "Admin", new { workspace = string.IsNullOrEmpty(workspace.Name) ? Atom.DefaultWorkspaceName : workspace.Name }) %>">edit</a> <a title="Delete Workspace" href="<%= Url.Action("DeleteWorkspace", "Admin", new { workspace = string.IsNullOrEmpty(workspace.Name) ? Atom.DefaultWorkspaceName : workspace.Name }) %>">delete</a></div>
    <h4><a href="<%= Url.Action("Settings", "Admin", new { workspace = string.IsNullOrEmpty(workspace.Name) ? Atom.DefaultWorkspaceName : workspace.Name }) %>"><%=workspace.Title%></a></h4>
      <div><strong><%= string.IsNullOrEmpty(workspace.Name) ? Atom.DefaultWorkspaceName : workspace.Name %></strong> <%= workspace.Collections.Count() %>&#160;<small>collections</small> <%= workspace.Authors.Count() %>&#160;<small>authors</small> <%= workspace.Contributors.Count() %>&#160;<small>contributors</small></div> 
    </li><%} %>
  </ul>
</div>