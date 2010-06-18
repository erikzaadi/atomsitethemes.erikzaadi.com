<%@ Control Language="C#" Inherits="ViewUserControl<AdminAcceptsModel>" %>
<div class="widget settings area accepts">
  <h3>Accepts <small>choose content types to store</small></h3>
  <form action="<%= Url.Action("ToggleAccepts", "Admin", new { workspace = Model.Scope.Workspace, collection = Model.Scope.Collection }) %>" method="post">
  <div class="tool">Template&#160;<%= Html.DropDownList("template", AdminAcceptsModel.AcceptTemplates().Select(t => new SelectListItem() { Text = t.Key, Value = string.Join(",", t.Value.Select(a => a.Value).ToArray()), Selected = string.Join(",", t.Value.Select(a => a.Value).ToArray()) == Model.CurrentAccepts }), "Custom")%></div>
  <ul>
  <%bool alt = false;  foreach (AcceptCheck accept in Model.AcceptChecks) { alt = !alt; %>
    <li class="<%= alt ? "alt" : string.Empty %> <%= accept.Checked || accept.Default ? "checked" : string.Empty %>">
    <label><input type="checkbox" name="accept" value="<%=accept.Accept %>" <%= accept.Checked || accept.Default ? "checked=\"checked\"" : string.Empty %> />
    <%= accept.Accept %><%= accept.Default ? " <small>Default</small>" : string.Empty %><%= accept.Accept == string.Empty ? "<strong>Read-Only</strong>" : string.Empty %>
    </label>
    </li><%} %>
  </ul>
  <%-- TODO: add button for non-javascript post --%>
  </form>
</div>