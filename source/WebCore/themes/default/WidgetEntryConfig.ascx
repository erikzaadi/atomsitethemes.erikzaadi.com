<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.WebCore.EntryConfigModel>" %>

<% using (Html.BeginForm("EntryConfig", "Widget", FormMethod.Post, new { id = "configureForm", @class = "choose" })) { %>
<fieldset>

<%= Html.Hidden("includePath", Model.IncludePath)%>

<% if (Model.SelectedId != null)
   { %>
<h4><small>Currently Selected Entry</small> <strong><%= Model.SelectedTitle%></strong> (<%= Model.GetSelectedId().Workspace ?? Atom.DefaultWorkspaceName%> <%= Model.GetSelectedId().Collection%>)</h4>
<%} %>
<h3>Choose an Entry</h3>

  <%= Html.ValidationSummary() %>

  <label>Entry <small>required, only visible entries you can access are shown</small></label>  
  <ul class="choices">
  <% bool alt = false; foreach (AtomEntry e in Model.Entries)
     {
       alt = !alt; %>

  <li class="<%= alt ? "alt" : null %> <%= e.Id.Equals(Model.GetSelectedId()) ? "selected" : null %>">
  <label><input type="radio" name="selectedId" value="<%= e.Id %>" <%= e.Id.Equals(Model.GetSelectedId()) ? " checked=\"checked\"" : null %> /> <strong><%= e.Title%></strong> (<%= e.Id.Workspace ?? Atom.DefaultWorkspaceName%> <%= e.Id.Collection%>)</label>
  </li>
  <%} %>
  </ul>

  <div class="buttons">
  <button type="button" name="close">Cancel</button>
  <input type="submit" value="Save" />
  </div>

  </fieldset>
<%} %>