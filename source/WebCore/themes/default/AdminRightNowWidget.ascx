<%@ Control Language="C#" Inherits="ViewUserControl<AdminRightNowModel>" %>
<div class="widget" id="rightnow">
<h3>Right Now</h3>
<table class="entries status">
<thead><tr><th class="main" colspan="2">Entries</th>
<th class="c"><a title="Published Entries" class="published" href="<%= Url.Action("Entries", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection, filter = "published" }) %>">Published</a></th>
<th class="c"><a title="Pending Entries" class="pending" href="<%= Url.Action("Entries", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection, filter = "pending" }) %>">Pending</a></th>
<th class="c"><a title="Draft Entries" class="draft" href="<%= Url.Action("Entries", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection, filter = "draft" }) %>">Draft</a></th>
<th class="c"><a title="All Entries" class="all" href="<%= Url.Action("Entries", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection }) %>">Total</a></th></tr></thead>
<tbody>
<%foreach (AppWorkspace w in Model.GetMetricWorkspaces("entries"))
  { %>

<% bool first = true; foreach (AppCollection c in Model.GetMetricCollections(w, "entries"))
   { %>
<tr>
<%if (first)
  { %>
  <th rowspan="<%= w.Collections.Count() %>"><%= w.Name ?? Atom.DefaultWorkspaceName%></th> <%} %>
  <% if (Model.AuthorizeService.IsInRole(Model.User, Model.Scope, AuthRoles.Author) || Model.AuthorizeService.IsInRole(Model.User, Model.Scope, AuthRoles.Administrator))
     { %>
<th><a href="<%= Url.Action("Settings", new { workspace = c.Id.Workspace, collection = c.Id.Collection }) %>"><%= c.Id.Collection%></a></th> 
<%} else { %>
<th><%= c.Id.Collection%></th>
<%} %>
<td><a class="published" href="<%= Url.Action("Entries", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection, filter = "published" }) %>">
  <%= Model.GetMetric(c, "entries-pub") %></a></td>
<td><a class="pending" href="<%= Url.Action("Entries", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection, filter = "pending" }) %>">
<%= Model.GetMetric(c, "entries-pend") %></a></td>
<td><a class="draft" href="<%= Url.Action("Entries", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection, filter = "draft" }) %>">
<%= Model.GetMetric(c, "entries-draft") %></a></td>
<td class="total"><a class="all" href="<%= Url.Action("Entries", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection }) %>">
<%= Model.GetMetric(c, "entries-total") %></a></td>
</tr>

<%first = false;
   }
  } %>

</tbody>
</table><%
          if (Model.HasAnnotations())
          { %>
<table class="annotations">
<thead><tr><th class="main" colspan="2">Annotations</th>
<th class="c"><a title="Published Annotations" class="published" href="<%= Url.Action("Annotations", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection, filter = "published" }) %>">Published</a></th>
<th class="c"><a title="Pending Annotations" class="pending" href="<%= Url.Action("Annotations", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection, filter = "pending" }) %>">Pending</a></th>
<th class="c"><a title="Spam Annotations" class="spam" href="<%= Url.Action("Annotations", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection, filter = "spam" }) %>">Spam</a></th>
<th class="c"><a title="All Annotations" class="all" href="<%= Url.Action("Annotations", "Admin", new { workspace = Model.HighestScope.Workspace, collection = Model.HighestScope.Collection }) %>">Total</a></th></tr></thead>
<tbody>
<%foreach (AppWorkspace w in Model.GetMetricWorkspaces("ann"))
  { %>

<% bool first = true; foreach (AppCollection c in Model.GetMetricCollections(w, "ann"))
   { %>
<tr>
<%if (first)
  { %>
  <th rowspan="<%= w.Collections.Count() %>"><%= w.Name ?? Atom.DefaultWorkspaceName%></th> <%} %>
  <% if (Model.AuthorizeService.IsInRole(Model.User, Model.Scope, AuthRoles.Author) || Model.AuthorizeService.IsInRole(Model.User, Model.Scope, AuthRoles.Administrator))
     { %>
<th><a href="<%= Url.Action("Settings", new { workspace = c.Id.Workspace, collection = c.Id.Collection }) %>"><%= c.Id.Collection%></a></th> 
<%}
     else
     { %>
<th><%= c.Id.Collection%></th>
<%} %>
<td><a class="published" href="<%= Url.Action("Annotations", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection, filter = "published" }) %>">
  <%= Model.GetMetric(c, "ann-pub")%></a></td>
<td><a class="pending" href="<%= Url.Action("Annotations", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection, filter = "pending" }) %>">
<%= Model.GetMetric(c, "ann-pend")%></a></td>
<td><a class="spam" href="<%= Url.Action("Annotations", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection, filter = "spam" }) %>">
<%= Model.GetMetric(c, "ann-spam")%></a></td>
<td class="total"><a class="all" href="<%= Url.Action("Annotations", "Admin", new { workspace = c.Id.Workspace, collection = c.Id.Collection }) %>">
<%= Model.GetMetric(c, "ann-total")%></a></td>
</tr>

<%first = false;
   }
  } %>

</tbody>
</table>
<%} if (Model.AuthorizeService.IsInRole(Model.User, Model.Scope, AuthRoles.Author) || Model.AuthorizeService.IsInRole(Model.User, Model.Scope, AuthRoles.Administrator))
   { %>
<small>Click collection name to change settings.</small>
<%} %>
<%--
<p>
The theme is <strong><%= ThemeViewEngine.GetCurrentTheme(ViewContext.RequestContext) %></strong> and can be configured differently for each collection.
</p>--%>

<p>
You are using <strong><%= Model.ReleaseName  %></strong> <%= Model.Version %>.
</p>
</div>