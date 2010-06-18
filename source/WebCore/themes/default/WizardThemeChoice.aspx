<%@ Page Language="C#" MasterPageFile="Wizard.Master" AutoEventWireup="true" Inherits="ViewPage<WizardThemeChoiceModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title>Choose Theme - Wizard</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <h1>Choose a Theme</h1>
	<form action="<%= Url.Action("ThemeChoice") %>" method="post">
	  <fieldset>
	   <div class="field">
	     <label for="selectedTheme">Theme</label>
	     <%= Html.DropDownList("selectedTheme", new SelectList(Model.Themes, "Name", "Title", Model.Theme)) %>
	   </div>
  	 <div id="divPreview">
	     <img id="imgPreview" src="<%= Url.Content(string.Format("~/themes/{0}/{0}.png", Model.Theme)) %>" alt="Theme Preview" />
	 	 </div>
     <div class="taskOption">
       <input type="submit" value="Finish" /> 
     </div>
	 </fieldset>
  </form>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
  <%-- set the year to this year --%>

    <script type="text/javascript">
      var previewUrl = '<%= Url.Content("~/themes/") %>';
      $(document).ready(function() {
        $('select[name=selectedTheme]').change(function() {
          var theme = $('select[name = selectedTheme]').val();
          $('#imgPreview').fadeOut('fast', function() {
            $(this).replaceWith('<img style="display:none" id="imgPreview" src="' + previewUrl + theme + "/" + theme + '.png" alt="Theme Preview" />');
            $('#imgPreview').fadeIn('slow');
          });
        });
      });
    </script>
</asp:Content>
