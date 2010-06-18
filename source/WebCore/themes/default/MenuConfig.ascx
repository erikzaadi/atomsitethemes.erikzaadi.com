<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.WebCore.MenuConfigModel>" %>
<h3>Configure Custom Menu Widget</h3>
<% using (Html.BeginForm("MenuConfig", "Menu", FormMethod.Post, new { id = "configureForm" }))
   { %>
<fieldset>
  <label for="html">Menu Items <small>required, one item per line, use format <em style="color:#000">text;href;title</em> (title is optional)</small></label>
  <strong style="display:block; margin:1em 0 0 1em">example</strong>
  <pre style="background:#ddd;color:#444;border:1px dotted #888; margin:0 1em 1em 1em; padding:0.4em">Home;/
Blog;/blog.xhtml;View My Super Blog
Pages;/pages
Code;http://github.com/jvance;View my source on GitHub</pre>
  <%= Html.ValidationMessage("menu")%>
  <%= Html.TextArea("menu", Model.Menu, new { style = "width:100%", rows = 10 })%>
  <%= Html.Hidden("includePath", Model.IncludePath)%>

  <div class="buttons">
  <button type="button" name="close">Cancel</button>
  <input type="submit" value="Save" />
  </div>
</fieldset>
<%} %>
