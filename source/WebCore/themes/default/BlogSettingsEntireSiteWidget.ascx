<%@ Control Language="C#" Inherits="ViewUserControl<BlogSettingsEntireSiteModel>" %>
<div id="blogSettings" class="widget settings area blog">
  <h3>Blog Settings</h3>
  <form action="<%= Url.Action("UpdateSettingsEntireSite", "Blog") %>" method="post">
  <fieldset>
  <div>
      <label for="blogPageExt">Blog Page Extension <small>changes dynamic links only</small></label>
        <%= Html.DropDownList("blogPageExt", Model.ExtSelections) %>
  </div>
  <p class="restart">If you change these settings, the application will restart.</p>
    <button type="submit" name="submit" value="Save">Save Settings</button>
 </fieldset>
 </form>
</div>