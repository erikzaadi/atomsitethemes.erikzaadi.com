<%@ Control Language="C#" Inherits="ViewUserControl<AdminWidgetSelectModel>" %>
<div class="widget widgets area selection">
  <h3><a rel="cancel" href="<%= Model.CancelHref %>" title="Cancel">Cancel</a><%= Model.SelectionTitle %></h3>
   <%if (!string.IsNullOrEmpty(Model.Error)) { %><div class="error"><%= Model.Error %></div><%} %>
  <ul>
    <%--<li><%= Html.TextBox("custom", "Enter custom name or select below") %> <div class="actions"><a rel="select" title="<%= Model.SelectionTitle %>" href="<%= Model.GetPostHref() %>">select</a></div></li>--%>
    <%bool alt = false; foreach (WidgetSelect option in Model.WidgetSelections) { alt = !alt; %>
    <li class="<%= (option.IsHint ? "hint" : (option.ScopeMatch.HasValue && option.ScopeMatch.Value == 0) ? "mismatch" : string.Empty) + (alt ? " alt" : string.Empty) %>">
      <div class="actions"><a rel="select" title="<%= Model.SelectionTitle %>" href="<%= option.PostHref %>">select</a></div>
      <% if (option.Icon != null) { %><span class="ico" title="<%= option.Icon %>"><%= option.Icon%></span><%} %><strong><%= option.Name%></strong>
      <% if (option.IsHint) { %><span title="hint" class="ico tip">hint</span><%} %>
      <% if (option.ScopeMatch.HasValue && option.ScopeMatch.Value == 0) { %><span title="invalid" class="ico tip">invalid</span><%} %>
      <% if (option.Description != null) {%><p><%= option.Description%> &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</p>
    <%}%>
    <% if (option.ScopeFlags.HasValue) {%>
    <div class="scopes scopes-<%= option.ScopeFlags %>"><%= option.ScopeFlags%></div>
    <%} %>
    </li><%} %>
  </ul>
</div>