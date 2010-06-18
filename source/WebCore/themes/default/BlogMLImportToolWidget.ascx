<%@ Control Language="C#" Inherits="ViewUserControl<AtomSite.Plugins.BlogMLPlugin.BlogMLModel>" %>
<div class="widget tools blogml">
  <h3>Import from <a href="http://en.wikipedia.org/wiki/BlogML">BlogML</a></h3>
  <% using (Html.BeginForm("Import", "BlogML", FormMethod.Post, new { enctype = "multipart/form-data" }))
     { %>
  
  <div>
  <label for="entryCollectionId">Target Entry Collection <small>for blog posts</small></label>
  <%= Html.DropDownList("entryCollectionId", Model.EntrySelections) %>
  </div>
  
  <div>
  <label for="pagesCollectionId">Target Pages Collection <small>for articles</small></label>
  <%= Html.DropDownList("pagesCollectionId", Model.PagesSelections) %>
  </div>
  
  <div>
  <label for="mediaCollectionId">Target Media Collection <small>for attachments</small></label>
<%= Html.DropDownList("mediaCollectionId", Model.MediaSelections) %>
  </div>
  
  <div>
    <label for="importMode">Import Mode</label>
    <%= Html.DropDownList("importMode", Model.ImportModeSelections)%>
  </div>
  <div>
    <label for="blogml">BlogML File</label>
    <input type="file" name="blogml" />
  </div>
  
  <p><em style="color:#a90000">Warning: if you choose overwrite import mode, then existing data will be deleted</em></p>
  <button type="submit" name="import">Import</button>
  
  <%} %>
</div>