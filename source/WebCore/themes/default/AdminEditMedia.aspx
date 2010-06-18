<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminMediaModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title><%= (Model.IsNew) ? "Add New Media" : "Edit Media"%> &rsaquo; AtomSite Manager</title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<div class="location floatr">
Workspace: <strong><%= Model.Scope.Workspace %></strong>
Collection: <strong><%= Model.Scope.Collection %></strong>
</div>
<h2><%= (Model.IsNew) ? "Add New Media" : "Edit Media" %></h2>
<div class="yui-ge" id="<%= Model.IsNew ? "NewMedia" : Model.Entry.Id.ToWebId() %>">
<form method="post" action="<%= Url.Action("EditMedia", "Admin", new {workspace=Model.Scope.Workspace,collection=Model.Scope.Collection}) %>" enctype="multipart/form-data">
  <div class="yui-u first">
  <input type="hidden" name="id" value="<%= Model.EntryId %>" />
  <div class="entry">
  <input type="text" name="title" title="Enter Title" value="<%= Model.Entry.Title %>" />
  <div class="links">
  <% if (Model.Entry.Content.Src != null)
     {  %><span class="floatr"><a style="opacity:0.5" href="#">Edit Id</a> | <a style="opacity:0.5" href="#">Add Link</a> | <a class="larger" href="<%=Model.Entry.Content.Src %>">View Media</a></span>
  <div><strong>Web Link:</strong> <%= Model.Entry.Content.Src %></div> 
  <%} else { %>
  <strong>Media Path <small>(slug)</small>:</strong> <input type="text" maxlength="200" name="slug" value="<%=Model.Entry.Id != null ? Model.EntryId.EntryPath : string.Empty%>"/> <span class="info">special characters not allowed</span>
  <%} %>
  </div>
  
  <div class="upload">
    <label for='upmedia'>Upload Media <small><%= string.Join(", ", Model.Collection.Accepts.Select(a => a.Ext).ToArray()) %></small></label>
    <input type="file" name="upmedia" size="30" />
  </div>
 

<%if (Model.Entry.Content.Src != null && Model.Entry.Content.Type.ToLowerInvariant().StartsWith("image")) { %>
<img id="mediapreview" src="<%= Model.Entry.Content.Src %>" />
<%} %>
  </div>
  
  <div class="widget summary">
  <h3>Summary</h3>
  <textarea cols="80" rows="3" name="summary"><%= Model.Entry.Summary != null ? Model.Entry.Summary.ToString() : string.Empty%></textarea>
  <small>The summary is an optional hand-crafted description of this entry.</small>
  </div>
  
  <% Html.RenderWidgets("editEntryLeft"); %></div>
  
  <div class="yui-u">
  
  <div class="widget publish">
  <h3>Publish</h3>
  <fieldset title="Publish">
  <div class="state">
  Draft: <strong><input type="radio" id="draftyes" name="draft" value="true"<%= (Model.Entry.Draft) ? " checked=\"checked\"" : string.Empty %> /><label for="draftyes">yes</label><input type="radio" id="draftno" name="draft" value="false"<%= (!Model.Entry.Draft) ? " checked=\"checked\"" : string.Empty %> /><label for="draftno">no</label></strong>
  </div>
  
  <div class="state alt"><%-- TODO: authorized to approve? --%>
  Approved: <strong><input type="radio" id="approvedyes" name="approved" value="true"<%= (Model.Entry.Approved) ? " checked=\"checked\"" : string.Empty %> /><label for="approvedyes">yes</label><input type="radio" id="approvedno" name="approved" value="false"<%= (!Model.Entry.Approved) ? " checked=\"checked\"" : string.Empty %> /><label for="approvedno">no</label></strong>
  </div>
  
  <div class="state">
  <% if (Model.Entry.Published.HasValue)
     { %>
  <%= Model.Entry.Published.Value >= DateTimeOffset.Now ? "Publish on" : "Published on"%>: <strong class="published"><%=Html.DateTimeAbbreviation(Model.Entry.Published.Value, (dto, tsi) => dto.ToString("g"))%></strong>
  
  <%} else { %>
  Publish: <strong class="published">immediately</strong>
  
  <%} %><a id="editpublished" class="edit" href="javascript:return false">Edit</a>

<div id="publishedinput">
  <input class="date" type="text" maxlength="10" name="publishedDate" value="<%=Html.DateTimeFormat(Model.Entry.Published ?? DateTime.Now, "d")%>" /> @ <input type="text" class="time" maxlength="8" name="publishedTime" value="<%=Html.DateTimeFormat(Model.Entry.Published ?? DateTime.Now, "hh:mm tt")%>" />
</div>

  </div>
  
  <%if (Model.Entry.Edited.HasValue)
    {  %>
  <div class="state alt">
  Edited: <%=Html.DateTimeAgoAbbreviation(Model.Entry.Edited.Value)%>
  </div>
  <%} %>
  
  <div class="publish">
    <%if (Model.Entry.Id == null)
      { %>
    <button type="submit" name="submit" value="Save Draft">Save Draft</button>
    <%}
      else
      { %>
    <button type="button" onclick="del('<%= Model.Entry.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntry", Model.Entry.Id) %>');"><span class="btnicon ui-icon ui-icon-trash"></span> Delete</button>
    <%} %>
    <button class="default" type="submit" name="submit" value="Publish">Publish</button>
  </div>
    </fieldset>
  
</div>
  <div class="widget categories">
    <h3>Categories</h3>
    <ul>
    <% foreach (AtomCategory cat in Model.Collection.AllCategories) { %><li>
       <input type="checkbox" id="cat-<%=cat.Term %>" name="categories" value="<%=cat.Term %>" <%= Model.Entry.Categories.Contains(cat) ? "checked=\"checked\"" : string.Empty  %>/>
       <label for="cat-<%=cat.Term %>"><%=cat %></label>
       </li>
    <%} %>
    </ul>
    <div class="add">
    <%--<% using (Html.BeginForm("AddCategory", "Admin"))
       { %>--%><input type="text" name="categoryName" title="Category Name" value="Category Name" />
       <button name="addCategory">Add</button>
    <%--<%} %>--%>
    </div>
  </div>
  
  
  <div class="widget annotations">
  <h3>Annotations</h3>
  <div class="state">
  Comments: <strong><%= Model.Entry.Total ?? 0 %></strong>
  </div>
  <div class="state alt">
  Trackbacks/Pingbacks: <strong>0</strong>
  </div>
  <div class="state last">
  Allow New: <strong><input type="radio" id="annsyes" name="allowAnnotations" value="true" <%= Model.Entry.AllowAnnotate ? "checked=\"checked\"" : string.Empty %> /><label for="annsyes">yes</label><input type="radio" id="annsno" name="allowAnnotations" value="false" <%= !Model.Entry.AllowAnnotate ? "checked=\"checked\"" : string.Empty %> /><label for="annsno">no</label></strong>
  </div>
  </div>
  
  <% Html.RenderWidgets("editEntryRight"); %> </div> </form>
</div>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
$('.entry .upload input[type=file]').MultiFile({ max: 1, accept: '<%= string.Join("|", Model.Collection.Accepts.Select(a => a.Ext).ToArray()) %>' });
});</script>
</asp:Content>
