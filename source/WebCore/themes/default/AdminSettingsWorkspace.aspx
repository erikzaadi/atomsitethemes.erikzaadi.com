<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminSettingsWorkspaceModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<% if (Model.IsNew) { %>
<title>Create New Workspace &rsaquo; AtomSite Manager</title>
<% } else { %>
<title>Settings for Workspace <%= Model.Scope.Workspace%> &rsaquo; AtomSite Manager</title>
<%} %>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">

<%--the tabs--%>
<ul class="tabs floatr"> 
    <li><a href="#basicSettings">Basic Settings</a></li>
    <% if (!Model.IsNew) { Html.RenderWidgets("settingsTabs"); }%>
</ul>

<% if (Model.IsNew) { %>
<h2>Create New Workspace</h2>
<% } else { %>
<h2>Settings for Workspace <%= Model.Scope.Workspace%></h2>
<%} %>
<div class="panes"> 
  <div id="basicSettings" class="widget settings area basic">
  <h3>Basic Settings</h3>
<form action="<%= Url.Action(Model.IsNew ? "NewWorkspace" : "UpdateWorkspace", "Admin", new { workspace = Model.Scope.Workspace }) %>" method="post">
<fieldset>
<div class="yui-gb">
  <div class="yui-u first">
    <div>
      <label for="title">Title <small>ex: MichaelJackson.com</small></label>
      <input type="text" name="title" maxlength="100" value="<%= Model.Title %>" />
    </div>
    <div>
      <label for="subtitle">Subtitle <small>ex: Everything about the king of pop</small></label>
      <input type="text" name="subtitle" size="40" maxlength="200" value="<%= Model.Subtitle %>" />
    </div>
  </div>
  <div class="yui-u">
    <div>
      <label for="name">Name <small>internal/external path</small></label>
      <input type="text" name="name" maxlength="50" <%= Model.IsNew ? string.Empty : "readonly=\"readonly\"" %> value="<%= Model.Name %>" />
    </div> 
  </div>
  <div class="yui-u">
<%--  <% if (Model.IsNew)  {%>
  <p class="restart">If you create a new workspace, the application will restart.</p>
  <% } else { %>
  <p class="restart">If you change these settings, the application will restart.</p>
  <%} %>--%>
    <button type="submit" name="submit" value="Save"><%= Model.IsNew ? "Create Workspace" : "Save Settings"%></button>
  </div>
</div>
</fieldset>
</form>
</div> 

<% if (!Model.IsNew) { Html.RenderWidgets("settingsPanes"); } %>

</div>

<% if (!Model.IsNew) { %>
<div class="yui-g">
  <div class="yui-u first">
    <% Html.RenderWidgets("settingsLeft"); %>
  </div>  
  <div class="yui-u">
    <% Html.RenderWidgets("settingsRight"); %>
  </div>
</div>
<% } %>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
  $('li a[title="Delete Collection"]')
  .live("click", function() {
    if (confirm('Deleting this collection will delete all entries and annotations within this collection. \n' +
          'ALL DATA WILL BE LOST! This action cannot be undone.')) {
      var id = $(this).closest('li').attr('id');
      $.post($(this).attr('href'), function(data) {
      if (data.error == undefined) $('#' + id).fadeOut(); else alert(data.error);
      }, "json");
    }
    return false;
  });
});
</script>
</asp:Content>
