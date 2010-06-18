<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.WebCore.ScopeConfigModel>" %>
<% using (Html.BeginForm("ScopeConfig", "Widget", FormMethod.Post, new { id = "configureForm", @class = "choose" })) { %>
<fieldset>

<%= Html.Hidden("includePath", Model.IncludePath) %>

<h3>Choose a Scope</h3>

  <label><strong>Scope</strong> <small>target data in selected scope based on context</small></label>
  <%= Html.ValidationMessage("selectedScope")%>
<ul class="choices">
  <li class="alt<%= Model.SelectedScope == "site" ? " selected" : null %>">  
  <label><input type="radio" name="selectedScope" value="site" <%= Model.SelectedScope == "site" ? " checked=\"checked\"" : null %> /> <strong>Entire Site</strong> (site)</label>
  </li>
  <li<%= Model.SelectedScope == "workspace" ? " class=\"selected\"" : null %>>  
  <label><input type="radio" name="selectedScope" value="workspace" <%= Model.SelectedScope == "workspace" ? " checked=\"checked\"" : null %> /> <strong>Workspace</strong> (workspace)</label>
  </li>
  <li class="alt<%= Model.SelectedScope == "collection" ? " selected" : null %>">  
  <label><input type="radio" name="selectedScope" value="collection" <%= Model.SelectedScope == "collection" ? " checked=\"checked\"" : null %> /> <strong>Collection</strong> (collection)</label>
  </li>
</ul>

  <div class="buttons">
  <button type="button" name="close">Cancel</button>
  <input type="submit" value="Save" />
  </div>

</fieldset>
<%}%>