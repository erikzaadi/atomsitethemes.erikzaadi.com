<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminThemeModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Theme &rsaquo; AtomSite Manager</title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">

<form action="<%= Url.Action("ChangeTheme", "Admin") %>" id="themeForm">
<%= Model.Scope.Workspace != null ? Html.Hidden("workspace",  Model.Scope.Workspace) : null %>
<%= Model.Scope.Collection != null ? Html.Hidden("collection", Model.Scope.Collection) : null %>
<%= Html.Hidden("theme", Model.Theme.Name) %>
<div class="yui-gf">
<div class="yui-u first">
<h2>Theme</h2>
<h3>Choose Theme</h3>
<div class="widget choose">
<div class="tabs">
<strong>Installed</strong> | <a href="http://atomsite.net/themes.xhtml">Online</a> | <a href="<%= Url.Action("ThemeUpload", "Admin") %>">Upload</a>
</div>
<ul>
<%if (Model.CanInherit) {%>
<li class="<%= Model.Inherited ? "selected current" : string.Empty %>" id="(inherit)">
<h4><a href="<%= Url.Action("ThemeInfo", "Admin", new { workspace=Model.Scope.Workspace, collection=Model.Scope.Collection, theme = "(inherit)"}) %>">Inherit</a></h4>
<small>use parent's theme</small>
</li>
<%} %>
<%bool alt = false;  foreach (Theme theme in Model.InstalledThemes)
  { alt = !alt; %>
  <li class="<%= alt ? "alt" : string.Empty %> <%= Model.Theme.Equals(theme) && (Model.Scope.IsEntireSite || !Model.Inherited) ? "selected current" : string.Empty %>" id="<%= theme.Name %>">
  <h4><a href="<%= Url.Action("ThemeInfo", "Admin", new { workspace=Model.Scope.Workspace, collection=Model.Scope.Collection, theme = theme.Name}) %>"><%=theme.Title%></a>
  <span class="version"><%= theme.Version%></span></h4>
    <div class="rating">
			<span class="ui-rater-starsOff" style="width:90px;">
				<span class="ui-rater-starsOn" style="width:<%= Math.Round(18F * theme.Rating) %>px"></span>
			</span>
		</div>
  </li>
<%} %></ul></div>
</div>
<div class="yui-u" id="target">

<% Html.RenderPartial("AdminThemeInfo", Model); %>
        
  </div>
</div>
</form>

</asp:Content>

