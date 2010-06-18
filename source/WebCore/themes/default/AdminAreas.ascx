<%@ Control Language="C#" Inherits="ViewUserControl<AdminWidgetsModel>" %>
<div class="widget widgets area areas">
  <h3><a rel="add" href="<%= Url.Action("AddArea", "Admin", Model.GetRouteData()) %>" title="Add an Area for Includes">Add Area</a>
  Areas</h3>
  <ul>
  <%bool alt = false;  foreach (Area area in Model.Areas) { alt = !alt; %>
    <li class="<%= alt ? "alt" : string.Empty %> <%= area.Inherited ? "inherited" : string.Empty %> <%= area.Name == Model.AreaName ? "selected" : string.Empty %>">
    <div class="actions"><% if (area.RemoveHref != null) { %><a rel="remove" href="<%= area.RemoveHref %>">remove</a><%} %></div>
    
    <%--<span class="ico" title="<%= area.Name %>"><%= area.Name %></span>--%><a href="<%= area.SelectHref %>"><%= area.Name%></a> <%= area.Inherited ? "<small>Inherited</small>" : null%>
    <p><%= area.IncludeCount %>&#160;include<%= area.IncludeCount == 1 ? string.Empty : "s" %></p>
    </li><%} %>
  </ul>
  <div class="areakey"><a href="http://atomsite.net/info/Themes.xhtml"><img src="<%= Url.ImageSrc("areakey.png") %>" alt="Learn about theme areas" /></a>
  </div>
</div>