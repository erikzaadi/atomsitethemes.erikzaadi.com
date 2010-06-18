<%@ Page Language="C#" MasterPageFile="Wizard.Master" AutoEventWireup="true" Inherits="ViewPage<WizardBasicSettingsModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title>Basic Settings - Wizard</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <h1>Basic Settings</h1>

	 <%= Html.ValidationSummary() %>

	 <form action="<%= Url.Action("BasicSettings") %>" method="post">
	 <fieldset>
	 <div class="field">
	 <label>Address</label>
	 <strong><%= Model.Address %></strong>
	 </div>
	 
	 <div class="field">
	 <small>Enter your domain or email to uniquely identify your posts <a style="vertical-align:super; font-weight:bold;" href="http://atomsite.net/info/URIsAndURLs.xhtml">?</a></small>
	 <label for="owner">Id Name</label>
    <%= Html.TextBox("owner", Model.Owner, new { @class = "required", style = "width:12em" }) %>
	 </div>
	 
	 <div class="field">
	 <small>Enter the first year you obtained this domain or email</small>
	 <label for="year">Id Year</label>
	 <%= Html.DropDownList("year", new SelectList(Enumerable.Range(1990, (DateTime.Today.Year + 2) - 1990)))%>
	 </div>
	 
	 <div class="field">
	 <small>Enter a short description of your website.</small>
	 <label for="workspaceSubtitle">Workspace Subtitle</label>
    <%= Html.TextBox("workspaceSubtitle", Model.WorkspaceSubtitle, new { @class = "required" }) %>
	 </div>
	 
	 <div class="field">
	 <label for="blogTitle">Blog Title</label>
    <%= Html.TextBox("blogTitle", Model.BlogTitle, new { @class = "required" }) %>
	 </div>
	 
	 <div class="field">
	 <small>Enter your name (nickname), which is also your username</small>
	 <label for="name">Your Name</label>
    <%= Html.TextBox("name", Model.Name, new { @class = "required", style = "width:12em" }) %>
	 </div>
	 
	 <div class="field">
	 <label for="password">Password</label>
   <%= Html.Password("password", Model.Password, new { @class = "required", style = "width:12em" })  %>
	 </div>
	 
	 <div class="field">
	 <label for="email">Your Email</label>
    <%= Html.TextBox("email", Model.Email, new { @class = "required", style = "width:16em" }) %>
	 </div>
	 
  <div class="taskOption">
    <input type="submit" value="Continue" /> 
  </div>
   
	 </fieldset>	 
	 </form>
	 
   
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
  <%-- set the year to this year --%>
    <script type="text/javascript">
      function isValid() {
        var valid = true;
        $("input.required").each(function() {
          if (jQuery.trim($(this).val()).length < 1) {
            $(this).addClass('failure').fadeTo(200, 0.2, function() {
              $(this).fadeTo(200, 0.9,
  		function() { $(this).fadeTo(200, 0.2, function() { $(this).fadeTo(200, 1); }); });
            });
            valid &= false;
          } else {
            $(this).removeClass('failure');
          }
        });
        return valid;
      }

      $(document).ready(function () {
        $("form").submit(function () { if (!isValid()) return false; });
        $('#year').val('<%= DateTime.Today.Year %>');
      });
    </script>
</asp:Content>
