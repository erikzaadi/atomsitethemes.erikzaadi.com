<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminPluginModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Plugin &rsaquo; AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2><%= Model.Plugin.Title %></h2>
<div class="content">
<%= Model.Plugin.Text%>
</div>
<h3>Routes</h3><ul>
<% foreach (RoutePart r in Model.Plugin.Routes) { %>
<li><h4><%= r.Name %></h4> <%= r.Url %></li>
<%} %></ul>
<h3>Pages</h3><ul>
<% foreach (PagePart page in Model.Plugin.Pages) { %>
<li><h4><%= page.Name %></h4> <%= page.Description %></li>
<%} %></ul>
<h3>Widgets</h3><ul>
<% foreach (WidgetPart w in Model.Plugin.Widgets) { %>
<li><h4><%= w.Name %></h4> <%= w.Description %></li>
<%} %></ul>
<h3>View Engines</h3><ul>
<% foreach (ViewEnginePart v in Model.Plugin.ViewEngines) { %>
<li><h4><%= v.Name %></h4> <%= v.Description %></li>
<%} %></ul>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript"></script>
</asp:Content>
