<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true"
	Inherits="ViewPage<FeedModel>" EnableSessionState="False" EnableViewState="False"
	EnableViewStateMac="False" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title><%= Model.Title %></title>
  <link rel="service" type="application/atomsvc+xml" href="<%= Url.RouteUrlEx("AtomPubService", AbsoluteMode.Force) %>" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
	<h3><%= Model.Title %></h3>
<% foreach (AtomEntry entry in Model.Feed.Entries) { %>
	<h4><a href="<%= Url.RouteIdUrl("AtomPubResource", entry.Id) %>"><%= entry.Title %></a></h4>
  <p><%= entry.Text.ToStringPreview(90) %></p>
<% } %>
</asp:Content>

<asp:Content ID="Side" ContentPlaceHolderID="sidemid" runat="server">
	<div id="total" class="stat">
		<label for="total">
			Total Entries</label>
		<div class="statVal">
			<%= Model.Feed.TotalResults %></div>
	</div>
	<div id="avg" class="stat">
		<label for="avg">
			Average per Day</label>
		<div class="statVal">
			<%= Model.GetAveragePerDay().ToString("0.#") %></div>
	</div>
</asp:Content>
