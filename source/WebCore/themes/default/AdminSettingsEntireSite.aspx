<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminSettingsEntireSiteModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Settings for Entire Site &rsaquo; AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">

<%--the tabs--%>
<ul class="tabs floatr"> 
    <li><a href="#basicSettings">Basic Settings</a></li>
    <% Html.RenderWidgets("settingsTabs"); %>
</ul>

<h2>Settings for Entire Site</h2>
<div class="panes"> 
  <div id="basicSettings" class="widget settings area basic">
  <h3>Basic Settings</h3>
<form action="<%= Url.Action("UpdateEntireSite", "Admin") %>" method="post">
<fieldset>
<div class="yui-gb">
  <div class="yui-u first">
    <div>
      <label for="siteAddress">Site Address <small>with trailing slash</small></label>
      <input type="text" name="siteAddress" maxlength="100" value="<%= Model.SiteAddress %>" />
    </div>
    <div>
      <label for="serviceType">Service Type <small>workspaces mode</small></label>
      <%= Html.DropDownList("serviceType", Enum.GetNames(typeof(ServiceType)).Select(t => new SelectListItem() { Text = t, Value = t, Selected = t == Model.ServiceType })) %>
    </div>
  </div>
  <div class="yui-u">
    <div>
      <label for="defaultSubdomain">Default Sub-domain <small>leave blank if not using one</small></label>
      <input type="text" name="defaultSubdomain" maxlength="50" value="<%= Model.DefaultSubdomain %>" />
    </div>
    <div>
      <label for="secure">Site supports SSL? <small>secure https</small></label>
      <input type="checkbox" value="true" name="secure" <%= Model.Secure ?? false ? "checked=\"checked\"" : string.Empty %> />
    </div>    
  </div>
  <div class="yui-u">
  <p class="restart">If you change these settings, the application will restart.</p>
    <button type="submit" name="submit" value="Save">Save Settings</button>
  </div>
</div>
</fieldset>
</form>
</div>
<% Html.RenderWidgets("settingsPanes"); %>
</div>

<div class="yui-g">
  <div id="workspaces" class="yui-u first">
    <% Html.RenderWidgets("settingsLeft"); %>
  </div>
  <div class="yui-u">
    <% Html.RenderWidgets("settingsRight"); %>
  </div>
</div>


</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
  $('li a[title="Delete Workspace"]').live("click", function() {
    if (confirm('Deleting this workspace will delete all collections and entries within this workspace. \n' +
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
