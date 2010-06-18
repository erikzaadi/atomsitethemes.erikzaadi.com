<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.WebCore.FeedConfigModel>" %>
<% using (Html.BeginForm("FeedConfig", "Widget", FormMethod.Post, new { id = "configureForm", @class = "choose" })) { %>
<fieldset>

<%= Html.Hidden("includePath", Model.IncludePath) %>
<h3>Choose a Collection</h3>

<%= Html.ValidationSummary() %>

  <label><strong>Collection</strong> <small>required, only visible collections you can access are shown</small></label>
<ul class="choices">
  <% bool alt = false; foreach (AppCollection e in Model.Collections) { alt = !alt; %>

  <li class="<%= alt ? "alt" : null %> <%= e.Id.Equals(Model.GetSelectedId()) ? "selected" : null %>">  
  <label><input type="radio" name="selectedId" value="<%= e.Id %>" <%= e.Id.Equals(Model.GetSelectedId()) ? " checked=\"checked\"" : null %> /> <strong><%= e.Title %></strong> (<%= e.Id.Workspace ?? Atom.DefaultWorkspaceName %>)</label>
  </li>
  <%} %>
</ul>

<%= Html.Hidden("hasCount", Model.HasCount) %>
<% if (Model.HasCount) { %>
<div>
    <label for="count">Count <small>maximum entries to show, leave blank for default</small></label>
    <%= Html.TextBox("count", Model.Count, new { style="width:5em", maxlength = 8} ) %></div>
<% } %>


<%= Html.Hidden("hasTitle", Model.HasTitle) %>
<% if (Model.HasTitle) { %>
<div>
    <label for="title">Title <small>use custom title, leave blank for default</small></label>
    <%= Html.TextBox("title", Model.Title, new { style = "width:25em", maxlength = 200 })%></div>
<% } %>


  <div class="buttons">
  <button type="button" name="close">Cancel</button>
  <input type="submit" value="Save" />
  </div>

</fieldset>
<%}%>