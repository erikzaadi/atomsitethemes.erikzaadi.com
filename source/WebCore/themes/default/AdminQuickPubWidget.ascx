<%@ Control Language="C#" Inherits="ViewUserControl<AdminModel>" %>
<div class="widget" id="quickpub">
<h3>QuickPub</h3>

	<div class="notifications">
	  <p><span class="ui-icon ui-icon-info"></span><span class="message"></span></p>
  </div>

<% if (Model.TargetCollection != null && Model.AuthorizeService.IsAuthorized(Model.User, Model.Scope, AuthAction.CreateEntryOrMedia)) { %>
<div class="location">

Workspace: <strong><%= Model.TargetCollection.Id.Workspace %></strong>
Collection: <strong><%= Model.TargetCollection.Id.Collection%></strong>
</div>

<% using (Html.BeginForm("QuickPub", "Admin"))
   { %>
<fieldset>
<%= Html.Hidden("id", Model.TargetCollection.Id)%>
<label for="title">Title</label>
<%= Html.TextBox("title")%>
<label for="content">Content</label>
<%= Html.TextArea("content")%>
<div class="publish">
<button class="wymupdate" type="submit" name="submit" value="Save Draft">Save Draft</button>
<button class="wymupdate default" type="submit" name="submit" value="Publish">Publish</button>
</div>
</fieldset>
<%} %>
<%}else { %>
  <p class="warning">We're sorry. You do not have access to publish new entries or there is no default collection that accepts entries.  Please contact an author or administrator.</p>
<%} %>
</div>