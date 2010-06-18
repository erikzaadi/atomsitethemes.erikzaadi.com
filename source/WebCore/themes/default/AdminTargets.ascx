<%@ Control Language="C#" Inherits="ViewUserControl<AdminWidgetsModel>" %>
<div class="widget widgets area targets">
  <h3><a rel="add" href="<%= Url.Action("AddTarget", "Admin", Model.GetRouteData()) %>" title="Add Target">Add Target</a>
  Pages &amp; Widgets</h3>
  <ul>
  <%bool alt = false; int idx = -1; foreach (Target target in Model.Targets)
    { alt = !alt; idx++; %>
    <li id="target-<%= idx %>" class="<%= alt ? "alt" : string.Empty %> <%= target.Inherited ? "inherited" : string.Empty %> <%= target.Name == Model.TargetName ? "selected" : string.Empty %> <%= target.IsPage ? "target-p" : "target-w"%>">
    <div class="actions"><% if (target.RemoveHref != null) { %><a rel="remove" href="<%= target.RemoveHref %>">remove</a><%} %></div>

    <span class="ico" title="<%= target.Icon %>"><%= target.Icon %></span><a href="<%= target.SelectHref %>"><%= target.Name %></a> <%= target.Inherited ? "<small>Inherited</small>" : null%>

    <p><%= target.Desc %> &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</p>    
    <div class="scopes scopes-<%= target.ScopeFlags %>"><%= target.ScopeFlags %></div>   
    </li><%} %>
  </ul>
</div>