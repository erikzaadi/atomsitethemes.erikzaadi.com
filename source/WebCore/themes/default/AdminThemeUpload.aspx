<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Theme Upload &rsaquo; AtomSite Manager</title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">

<h2>Upload and Install a Theme</h2>

<p class="warning">Note: the contents of the zip file will be extracted into the css, img, js, and themes directories.  The theme must have a proper manifest (xml) file for installation to succeed.<br />
<strong>Themes may contain executable code.  You should only install themes from trusted sources.</strong></p>

<form id="uploadtheme" action="<%= Url.Action("ThemeUpload", "Admin") %>" enctype="multipart/form-data" method="post">
<fieldset>
<label for='uptheme'>Upload Theme <small>zip theme file</small></label>
<br />
<input class="multi" type="file" name="uptheme" size="30" />
</fieldset>
</form>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
    $('input[name=uptheme]').MultiFile({ max: 1, accept: 'zip', afterFileAppend: function (element, value, master_element) {
        $('.MultiFile-label').append('<button type="submit">Install Theme</button>');
        }
    });
});</script>
</asp:Content>

