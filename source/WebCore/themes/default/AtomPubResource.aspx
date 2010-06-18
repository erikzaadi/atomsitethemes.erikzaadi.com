<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true"
	Inherits="ViewPage<EntryModel>" EnableSessionState="False" EnableViewState="False"
	EnableViewStateMac="False" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
<title><%= Model.Entry.Title %></title>
    <link rel="service" type="application/atomsvc+xml" href="<%= Url.RouteUrl("AtomPubService") %>" />
</asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="content" runat="server">
			<h3><%= Model.Entry.Title %></h3>
			<%if (Model.Entry.IsExternal)
     { %>
			<a href="<%=Model.Entry.Content.Src%>">View Resource</a>
			<%}
     else
     { %>
			<div class="content">
      <% Html.RenderContent(Model.Entry.Content); %>
			<%--<%= Model.Entry.Content.ToString() %>--%>
			</div><%} %>
			<br />
			<address>-<% Html.RenderPartial("AtomPubPeople", Model.Entry.People, new ViewDataDictionary() { { "id", Model.Entry.Id } }); %></address>
      <br />
</asp:Content>
<asp:Content ID="Side" ContentPlaceHolderID="sidemid" runat="server">
	<div id="stats">
		<div id="updated" class="stat">
			<label for="updated">Last Updated</label>
			<div class="statVal"><%= Html.DateTimeAgoAbbreviation(Model.Entry.Updated) %></div>
		</div>
	</div>
</asp:Content>
