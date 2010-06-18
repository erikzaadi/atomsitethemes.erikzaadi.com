<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminUserModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<% if (Model.IsNew) { %>
<title>Create New User &rsaquo; AtomSite Manager</title>
<% } else { %>
<title>Edit User <%= Model.Name %> &rsaquo; AtomSite Manager</title>
<%} %>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<%--the tabs--%>
<ul class="tabs floatr"> 
    <li><a href="#basic">Basic Information</a></li>
    <% if (!Model.IsNew) { Html.RenderWidgets("userTabs"); }%>
</ul>

<% if (Model.IsNew) { %>
<h2>Create New User</h2>
<% } else { %>
<h2>
Edit User <%= Model.Name %></h2>
<%} %>

 
<%--tab "panes"--%>
<div class="panes"> 
  <div class="widget users area basic" id="basic">
  <h3>Basic User Information</h3>
<form action="<%= Url.Action("EditUser") %>" method="post" id="userForm">
<fieldset>
<%= Html.Hidden("userId", Model.UserId) %>
<div class="yui-gb">
  <div class="yui-u first">
    <div>
      <label for="name"><strong>Username</strong> <small>required</small></label>
      <%= Html.TextBox("name", Model.Name, new { maxlength = "100", style = "width:9em" })%>
    </div>
    <div>
      <label for="fullName">Full Name</label>
      <%= Html.TextBox("fullName", Model.FullName, new { maxlength = "120", style = "width:18em" })%>
    </div>
    <div>
      <label for="email"><strong>Email</strong> <small>required (ex: bob123@gmail.com)</small></label>
      <%= Html.TextBox("email", Model.Email, new { maxlength = "120", style = "width:16em" })%>
    </div>
    <div>
      <label for="uri">Website <small>ex: http://bobsblog.com</small></label>
      <%= Html.TextBox("uri", Model.Uri, new { maxlength = "200", style = "width:20em" })%>
    </div>
  </div>
  <div class="yui-u">
    <div>
      <label for="ids"><strong>Ids</strong> <small>required, one per line, must be unique</small></label>
      <%= Html.TextArea("ids", Model.Ids, new { rows = "4", style = "width:20em" })%>
    </div>
    <div>
        <% if (!string.IsNullOrEmpty(Model.Password)) { %> <p class="warning">Only enter password if you'd like to change your existing password</p><%} %>
      <label for="password">Password <small>not required if using <a href="http://openid.net">OpenId</a></small></label>
      <%= Html.Password("password", null, new { maxlength = "50", style = "width:11em" })%>
    </div>
    <div>
      <label for="confirmPassword">Confirm Password <small>retype your new password</small></label>
      <%= Html.Password("confirmPassword", null, new { maxlength = "50", style = "width:11em" })%>
    </div>
  </div>
  <div class="yui-u">
    <button type="submit" name="submit" value="Save"><%= Model.IsNew ? "Create User" : "Save User"%></button>
  </div>
</div>
</fieldset>
</form>
</div> 

<% if (!Model.IsNew) { Html.RenderWidgets("userPanes"); } %>

</div>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server"></asp:Content>
