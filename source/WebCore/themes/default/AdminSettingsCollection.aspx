<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminSettingsCollectionModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<% if (Model.IsNew) { %>
<title>Create New Collection &rsaquo; AtomSite Manager</title>
<% } else { %>
<title>Settings for <%= Model.Collection.Title %> &rsaquo; AtomSite Manager</title>
<%} %>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<%--the tabs--%>
<ul class="tabs floatr"> 
    <li><a href="#basic">Basic Settings</a></li>
    <% if (!Model.IsNew) { Html.RenderWidgets("settingsTabs"); }%>
</ul>

<% if (Model.IsNew) { %>
<h2>Create New Collection</h2>
<% } else { %>
<h2>
Settings for <%= Model.Collection.Title %></h2>
<%} %>

 
<%--tab "panes"--%>
<div class="panes"> 
  <div class="widget settings area basic" id="basic">
  <h3>Basic Settings</h3>
<form action="<%= Url.Action(Model.IsNew ? "NewCollection":"UpdateCollection", new { workspace = Model.Scope.Workspace, collection = Model.Scope.Collection }) %>" method="post" id="settingsForm">
<fieldset>
<div class="yui-gb">
  <div class="yui-u first">
    <div>
      <label for="title">Title <small>ex: Awesome Blog</small></label>
      <input type="text" name="title" maxlength="100" value="<%= Model.Title %>" />
    </div>
    <div>
      <label for="subtitle">Subtitle <small>ex: My blog about awesome stuff</small></label>
      <input type="text" name="subtitle" size="40" maxlength="200" value="<%= Model.Subtitle %>" />
    </div>
    <div>
      <label for="secure">Syndication <small>allow syndication rss and atom feeds?</small></label>
      <input type="checkbox" value="true" name="syndicationOn" <%= Model.SyndicationOn ?? false ? "checked=\"checked\"" : string.Empty %> />
    </div>
  </div>
  <div class="yui-u">
    <div>
      <label for="name">Owner <small>your domain or email</small></label>
      <input type="text" name="owner" maxlength="50" size="20" <%= Model.IsNew ? string.Empty : "readonly=\"readonly\"" %> value="<%= Model.Owner %>" />
    </div>
    <div>
      <label for="name"><strong>Name</strong> <small>internal/external path</small></label>
      <input type="text" name="name" maxlength="50" size="14" <%= Model.IsNew ? string.Empty : "readonly=\"readonly\"" %> value="<%= Model.Name %>" />
    </div>
    <div>
      <label for="secure">Visible <small>display collection on site?</small></label>
      <input type="checkbox" value="true" name="visible" <%= Model.Visible ?? false ? "checked=\"checked\"" : string.Empty %> />
    </div>
  </div>
  <div class="yui-u">
  <div>
      <label for="secure">Dated <small>entry identification includes creation date</small></label>
      <input type="checkbox" value="true" name="dated" <%= Model.IsNew ? string.Empty : "disabled=\"disabled\"" %> <%= Model.Dated ?? false ? "checked=\"checked\"" : string.Empty %> />
      </div>
    <div>
      <label for="secure">Annotations <small>allow comments?</small></label>
      <input type="checkbox" value="true" name="annotationsOn" <%= Model.AnnotationsOn ?? false ? "checked=\"checked\"" : string.Empty %> />
    </div>
  <% if (Model.IsNew)  {%>
  <p class="restart">If you create a new collection, the application will restart.</p>
  <% } else { %>
  <p class="restart">If you change these settings, the application will restart.</p>
  <%} %>
    <button type="submit" name="submit" value="Save"><%= Model.IsNew ? "Create Collection" : "Save Settings"%></button>
  </div>
</div>
</fieldset>
</form>
</div> 

<% if (!Model.IsNew) { Html.RenderWidgets("settingsPanes"); } %>

</div>

<% if (!Model.IsNew) { %>
  <div class="yui-gd">
    <div class="yui-u first">
      <% Html.RenderWidgets("settingsLeft"); %>
    </div>
    <div class="yui-u yui-gd">
        <div class="yui-u first">
          <% Html.RenderWidgets("settingsMiddle"); %>
        </div>
        <div class="yui-u">
          <% Html.RenderWidgets("settingsRight"); %>
        </div>
    </div>
  </div>
<% } %>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server"></asp:Content>
