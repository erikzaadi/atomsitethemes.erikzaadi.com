<%@ Control Language="C#" Inherits="ViewUserControl<BlogSettingsModel>" %>
<div id="blogSettings" class="widget settings area blog">
  <h3>Blog Settings</h3>
  <form action="<%= Url.Action("UpdateSettings", "Blog") %>" method="post">
  <%= Html.Hidden("workspace", Model.Scope.Workspace) %>
  <%= Html.Hidden("collection", Model.Scope.Collection) %>
  <fieldset>
  <div>
  <label for="bloggingOn">Enable Blogging? <small>turn on all blogging features</small></label>
 <%= Html.CheckBox("bloggingOn", Model.BloggingOn ?? false) %></div>
 <div>
  <label for="trackbacksOn">Enable Trackbacks? <small>allow send and receive of trackbacks and pingbacks</small></label>
 <%= Html.CheckBox("trackbacksOn", Model.TrackbacksOn ?? false) %></div>
 
 </fieldset>
 
  <p class="restart">If you change these settings, the application will restart.</p>
    <button type="submit" name="submit" value="Save">Save Settings</button>
 </form>
</div>