<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Tools &rsaquo; AtomSite Manager</title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">

<%--the tabs--%>
<ul class="tabs floatr"> 
    <li><a href="#import-export">Import/Export</a></li>
    <% Html.RenderWidgets("toolsTabs"); %>
</ul>

<h2>Tools</h2>
 
<%--tab "panes"--%>
<div class="panes"> 
  
  <div id="import-export" class="yui-g">
    <div class="yui-u first">
      <% Html.RenderWidgets("toolsImport"); %>      
    </div>
    <div class="yui-u">
      <% Html.RenderWidgets("toolsExport"); %>  
    </div>
  </div>

  <% Html.RenderWidgets("toolsPanes"); %>

</div>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
</script>
</asp:Content>
