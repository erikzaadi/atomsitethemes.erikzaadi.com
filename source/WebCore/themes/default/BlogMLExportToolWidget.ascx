<%@ Control Language="C#" Inherits="ViewUserControl<AtomSite.Plugins.BlogMLPlugin.BlogMLModel>" %>
<div class="widget tools blogml">
  <h3>Export to <a href="http://en.wikipedia.org/wiki/BlogML">BlogML</a></h3>
  <% using (Html.BeginForm("Export", "BlogML"))
     { %>  
  <div>
    <label for="sourceEntryCollectionId">Source Entry Collection <small>blog posts</small></label>
    <%= Html.DropDownList("sourceEntryCollectionId", Model.EntrySelections) %>
  </div>
  
  <div>
    <label for="sourceEntryCollectionId">Source Pages Collection <small>articles</small></label>
    <%= Html.DropDownList("sourcePagesCollectionId", Model.PagesSelections) %>
  </div>
  
  <div>
    <label for="sourceMediaCollectionId">Source Media Collection <small>attachments</small></label>
    <%= Html.DropDownList("sourceMediaCollectionId", Model.MediaSelections, new Dictionary<string, object>() { {"readonly", "readonly"} })%>
  </div>
  
  <p>
    <em>Note: media referenced from entries to attachments is not yet supported</em>
  </p>
  
  <button type="submit" name="export">Export</button>  
  <%} %>
</div>