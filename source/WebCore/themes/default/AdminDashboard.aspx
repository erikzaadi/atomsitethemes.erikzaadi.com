<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Dashboard - AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2>Dashboard</h2>
<div class="yui-g">
  <div class="yui-u first"><% Html.RenderWidgets("dashboardLeft"); %></div>
  <div class="yui-u"><% Html.RenderWidgets("dashboardRight"); %></div>
</div>
</asp:Content>
