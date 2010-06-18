<%@ Control Language="C#" Inherits="ViewUserControl<AdminWidgetsModel>" %>
<div class="widget widgets area includes">
  <h3><a rel="add" href="<%= Url.Action("AddInclude", "Admin", Model.GetRouteData()) %>" title="Add a Widget Include">Add Include</a>
  Widget Includes</h3>
  <ul>
  <%bool alt = false; foreach (WidgetInclude include in Model.Includes) { alt = !alt; %>
    <li class="<%= alt ? "alt" : string.Empty %> <%= include.Inherited ? "inherited" : string.Empty %>" id="<%= include.IncludePath %>">
    <div class="actions"><% Html.RenderInclude(include.ConfigInclude); %> <% if (include.RemoveHref != null) { %><a rel="remove" href="<%= include.RemoveHref %>">remove</a><%} %></div>
    <% if (include.MoveHref != null) { %>
    <div class="move">
    <a class="ico" title="moveup" href="<%= include.MoveHref %>" rel="up">up</a>
    <a class="ico" title="movedown" href="<%= include.MoveHref %>" rel="down">down</a>
    </div>
    <%} %>
    <strong><%= include.Name%></strong> <%= include.Inherited ? "<small>Inherited</small>" : null%> <% if (include.ConfigInclude != null) { %><span title="configurable" class="ico tip">configurable</span><%} %> <% if (!include.Valid) { %><span title="invalid" class="ico tip">invalid</span><%} %>
    <p><%= include.Desc %> &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</p>
    <div class="scopes scopes-<%= include.ScopeFlags %>"><%= include.ScopeFlags %></div>
    </li><%} %>
  </ul>
 </div>