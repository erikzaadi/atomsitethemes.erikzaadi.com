<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.Plugins.TwitterPlugin.TwitterConfigModel>" %>
<% using (Html.BeginForm("Config", "Twitter", FormMethod.Post, new { id = "configureForm" })) { %>
<fieldset>

<%= Html.Hidden("includePath", Model.IncludePath) %>
<h3>Twitter Widget Configuration</h3>

<%= Html.ValidationSummary() %>

<div>
    <label for="username">Twitter Username <small>required, the user must have a public feed</small></label>
    <%= Html.TextBox("username", Model.Username, new { style = "width:10em", maxlength = 100 })%></div>

<div>
    <label for="count">Count <small>required, maximum tweets to show</small></label>
    <%= Html.TextBox("count", Model.Count, new { style="width:5em", maxlength = 8} ) %></div>

  <div class="buttons">
  <button type="button" name="close">Cancel</button>
  <input type="submit" value="Save" />
  </div>

</fieldset>
<%}%>