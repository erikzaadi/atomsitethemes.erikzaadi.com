<%@ Control Language="C#" Inherits="ViewUserControl<ContactSettingsModel>" %>
<div id="contactSettings" class="widget settings area contact">
  <h3>Contact Settings</h3>
  <div style="float:right; margin:0.8em; padding:0.4em; width:28em"> <%-- TODO: move to CSS file --%>
  <p>These settings apply to any contact forms that are added on your site.</p>
  <p>You can add a contact form to any workspace, collection, entry, or even as inline content using the following include:</p>
  <code>&lt;svc:include name="ContactWidget" /&gt;</code>
  </div>
  <form action="<%= Url.Action("UpdateSettings", "Contact") %>" method="post">
  <fieldset>
  <%--mode="email" to="test@gmail.com;test2@gmail.com" host="smtp.gmail.com" port="587" userName="test@gmail.com" password="*****" />--%>
  <%= Html.Hidden("workspace", Model.Scope.Workspace) %>
  <div>
  <label for="mode">Mode</label>
  <%= Html.DropDownList("mode", Model.GetContactModeSelectList()) %>
  </div>
  <div>
    <label for="to">Send Email To <small>for email mode only, separate multiple emails by semicolon</small></label>
    <%= Html.TextBox("to", Model.To, new { style = "width:22em" })%> 
  </div>
  <div>
    <label for="host">SMTP Host <small>for email mode only, ex: smtp.gmail.com</small></label>
    <%= Html.TextBox("host", Model.Host) %> 
  </div>
  <div>
    <label for="port">SMTP Port <small>for email mode only, ex: 587 or leave blank for default</small></label>
    <%= Html.TextBox("port", Model.Port, new { style = "width:3.4em" })%> 
  </div>
  <div>
    <label for="userName">SMTP User Name <small>for email mode only, ex: bob@gmail.com</small></label>
    <%= Html.TextBox("userName", Model.UserName)%>
  </div>
  <div>
    <label for="password">SMTP Password <small>for email mode only</small></label>
    <%= Html.Password("password", Model.Password) %>
  </div> 
 </fieldset>
 
    <button type="submit" name="submit" value="Save">Save Settings</button>
 </form>
</div>