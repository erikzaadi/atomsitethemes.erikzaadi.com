<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminAnnotationsModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Annotations &rsaquo; AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2>Annotations</h2>
<div class="filter">
<a class="all" href="<%= Url.Action("Annotations", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection }) %>">All (<%=Model.AllCount %>)</a> | 
<a class="published" href="<%= Url.Action("Annotations", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter="published" }) %>">Published (<%= Model.PublishedCount %>)</a> | 
<a class="unapproved" href="<%= Url.Action("Annotations", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter="unapproved" }) %>">Unapproved (<%= Model.UnapprovedCount %>)</a> | 
<a class="spam" href="<%= Url.Action("Annotations", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter="spam" }) %>">Spam (<%= Model.SpamCount %>)</a>
<div class="search">
<form action="<%= Url.Action("Annotations", "Admin")  %>" method="get">
<fieldset>
Search <%= Html.TextBox("search", Model.Search) %> <input type="submit" value="Go" />
</fieldset>
</form>
</div>

</div>

<table class="annotations">
<thead>
<tr><th>Author</th><th>Content</th><th>In Reply To</th></tr>
</thead>
<tfoot>
<tr><td colspan="2">Pages: <%
                      foreach (int i in Enumerable.Range(1, Model.Annotations.PageCount))
                      {
                        if (Model.Annotations.PageNumber == i) Response.Write("<strong>");
                        Response.Write("<a href='" + Url.Action("Annotations", "Admin", new { workspace = Model.Scope.Workspace, collection = Model.Scope.Collection, filter=Model.Filter, search=Model.Search, page = i }) + "'>" + i + "</a> ");
                        if (Model.Annotations.PageNumber == i) Response.Write("</strong>");
                      }

                      %></td><td>Total Entries: <strong><%=Model.Annotations.TotalItemCount%></strong></td></tfoot>
<tbody>
<% bool alt = true; foreach (AtomEntry e in Model.Annotations) { alt = !alt;%>
<tr id="<%= e.Id.ToFullWebId() %>" class="annotation<%= alt? " alt" : string.Empty %><%= !e.Approved? " pending" : string.Empty %>">
<td class="authors"><%Html.RenderPartial("AtomPubPeople", e.Authors); %></td>
<td class="content">
  Submited on <%= Html.DateTimeAbbreviation(e.Date, (dto, tzi) => dto.ToString("g")) %> <span class='status'><%= !e.Approved? "[Not Approved]" : string.Empty %></span>
  <p><%= e.Text.ToStringPreview(500) %></p>
  <div class="actions">
<% foreach (AtomLink link in e.Links) { %>
<a rel="<%= link.Rel %>" href="<%= link.Href %>"><%= link.Title %></a><% if (!e.Links.LastOrDefault().Equals(link)) {%> | <%} %>
<%} %>
  </div>
</td>
<td class="inreplyto">
<a href="<%= e.InReplyTo.Href %>">
<%= e.GetValue<string>(Atom.SvcNs + "parentTitle") %></a><br />
<strong><%= e.Id.Collection %> entry <%-- TODO: get proper parent type to support nested --%></strong><br />
<%= e.Total %></td>
</tr>
<%} %>
</tbody>

</table>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server"></asp:Content>
