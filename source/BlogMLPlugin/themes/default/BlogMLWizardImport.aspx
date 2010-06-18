<%@ Page Language="C#" MasterPageFile="Wizard.Master" AutoEventWireup="true" Inherits="ViewPage<AtomSite.Plugins.BlogMLPlugin.BlogMLWizardImportModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title>BlogML Import - Wizard</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <h1>BlogML Import</h1>
  <%= Html.ValidationSummary() %>

 <% using (Html.BeginForm("WizardImport", "BlogML", FormMethod.Post, new { enctype = "multipart/form-data" })) { %> 
	 <fieldset>
	   <div class="field">
	   <small>Target collection for blog posts</small>
	     <label for="blogName">Blog Name</label>
	     <strong><%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath%></strong><%= Html.TextBox("blogName", Model.BlogName, new { @class = "required", style = "width:7em" }) %>
	   </div>
	   <div class="field">
	   <small>Target collection for articles</small>
	     <label for="pagesName">Pages Name</label>
	     <strong><%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath%></strong><%= Html.TextBox("pagesName", Model.PagesName, new { style = "width:7em" })%>
	   </div>
	   
	   <div class="field">
	   <small>Target collection for attachments</small>
	     <label for="mediaName">Media Name</label>
	     <strong><%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath%></strong><%= Html.TextBox("mediaName", Model.MediaName, new { style = "width:7em" })%>
	   </div>
	   
	   <div class="field">
	     <small>Enter your domain or email to uniquely identify your posts <a style="vertical-align:super; font-weight:bold;" href="http://atomsite.net/info/URIsAndURLs.xhtml">?</a></small>
	     <label for="owner">Id Name</label>
       <%= Html.TextBox("owner", Model.Owner, new { @class = "required", style = "width:12em" })%>
	   </div>
  	 
	   <div class="field">
	     <small>Enter the first year you obtained this domain or email</small>
	     <label for="year">Id Year</label>
	     <%= Html.DropDownList("year", new SelectList(Enumerable.Range(1990, (DateTime.Today.Year + 2) - 1990)))%>
	   </div>
  	 
    <div class="field">
      <label for="blogmlfile">BlogML File</label>
      <input class="required" type="file" name="blogmlfile" />
    </div>
  
  <div class="taskOption">
    <input type="submit" value="Import" /> 
  </div>
 </fieldset>	 
<% } %>
   
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
  <%-- set the year to this year --%>
    <script type="text/javascript">
      function isValid() {
        var valid = true;
        $("input[type=text].required").each(function() {
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
        $('form').submit(function () { if (!isValid()) return false; });
        $('select[name=year]').val('<%= Model.Year %>');
        $('input[name=blogmlfile]').MultiFile({ max: 1, accept: 'xml' });
      });
    </script>
</asp:Content>
