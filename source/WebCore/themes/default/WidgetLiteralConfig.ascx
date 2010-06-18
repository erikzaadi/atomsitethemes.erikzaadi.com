<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.WebCore.LiteralConfigModel>" %>
<h3>Configure Literal Widget</h3>
<% using (Html.BeginForm("LiteralConfig", "Widget", FormMethod.Post, new { id = "configureForm" }))
   { %>
<fieldset>
  <label for="html">HTML <small>required</small></label>
  <%= Html.ValidationMessage("html")%>
  <%= Html.TextArea("html", Model.Html, new { style = "width:100%", rows = 10 })%>
  <%= Html.Hidden("includePath", Model.IncludePath)%>

  <div class="buttons">
  <button type="button" name="close">Cancel</button>
  <input type="submit" value="Save" />
  </div>
</fieldset>
<%} %>
