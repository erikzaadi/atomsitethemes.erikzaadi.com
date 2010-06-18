<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminPluginsModel>" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
  <title>Plugins &rsaquo; AtomSite Manager</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <div class="yui-g">
<div class="yui-u first">
<h2>Plugins</h2>
<form id="uploadplugin" action="<%= Url.Action("UploadPlugin", "Admin") %>" enctype="multipart/form-data" method="post">
<fieldset>
<label for='upplug'>Upload Plugin <small>zip plugin file</small></label>
<input type="file" name="upplug" size="30" />
</fieldset>
</form>
</div>
<div class="yui-u">
<div id="restart" class="ui-widget"> 
  <form action="<%=Url.Action("Restart", "Admin") %>" method="post">
	<div class="notifications"> 
		<button type="submit">Restart</button>
		<p><span class="icon-alert"></span> 
		<strong>Note</strong> You may need to restart the application for changes to take effect</p>
	</div>
	</form>
</div>

</div>
</div>

<div id="target">
<% Html.RenderPartial("AdminPluginListing", Model); %>
</div>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server"></asp:Content>
