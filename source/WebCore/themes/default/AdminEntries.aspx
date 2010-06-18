<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminEntriesModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Entries &rsaquo; AtomSite Manager </title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2>Entries</h2>
<div class="filter">
<a class="all" href="<%= Url.Action("Entries", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection }) %>">All (<%=Model.AllCount %>)</a> | 
<a class="published" href="<%= Url.Action("Entries", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter="published" }) %>">Published (<%= Model.PublishedCount %>)</a> | 
<a class="draft" href="<%= Url.Action("Entries", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter="draft" }) %>">Draft (<%= Model.DraftCount %>)</a> | 
<a class="unapproved" href="<%= Url.Action("Entries", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter="unapproved" }) %>">Unapproved (<%= Model.UnapprovedCount %>)</a>

<div class="category">Category <%= Html.DropDownList("category", Model.GetCategorySelectList(), "Choose a category") %></div>
<div class="search">
<form action="<%= Url.Action("Entries", "Admin")  %>" method="get">
<fieldset>
Search <%= Html.TextBox("search", Model.Search) %> <input type="submit" value="Go" />
</fieldset>
</form>
</div></div>
<table class="entries"><thead>
<tr><th>Collection</th><th>Entry</th><th>Authors</th><th>Categories</th><th>Anns</th><th>Date</th></tr></thead>
<tfoot>
<tr><td colspan="5">Pages: <%
foreach (int i in Enumerable.Range(1, Model.Entries.PageCount))
{
  if (Model.Entries.PageNumber == i) Response.Write("<strong>");
  Response.Write("<a href='" + Url.Action("Entries", "Admin", new { workspace= Model.Scope.Workspace, collection=Model.Scope.Collection, filter=Model.Filter, search=Model.Search, category=Model.Category, page = i }) + "'>" + i + "</a> ");
  if (Model.Entries.PageNumber == i) Response.Write("</strong>");
}

%></td><td>Total Entries: <strong><%=Model.Entries.TotalItemCount %></strong></td></tfoot>
<tbody>
<% bool alt = true; foreach (AtomEntry e in Model.Entries) { alt = !alt;%>
<tr id="<%= e.Id.ToFullWebId() %>" class="entry<%= alt? " alt" : string.Empty %><%= !e.Visible? " pending" : string.Empty %>">
<td class="coll"><%= e.Id.Collection %></td>
<td class="title"><strong><a href="<%= Url.Action(e.Media?"EditMedia":"EditEntry", "Admin", new { id = e.Id.ToString() }) %>"><%= e.Title %></a></strong> <span class="draft"><%= e.Draft ? "[Draft]" : string.Empty%></span> <span class='status'><%= !e.Approved ? "[Not Approved]" : string.Empty %></span>
<div class="actions">
<% foreach (AtomLink link in e.Links) { %>
<a rel="<%= link.Rel %>" href="<%= link.Href %>"><%= link.Title %></a><% if (!e.Links.LastOrDefault().Equals(link)) {%> | <%} %>
<%} %>
<td class="authors"><%Html.RenderPartial("AtomPubPeople", e.Authors); %></td>
<td class="cats"><% foreach (AtomCategory cat in e.Categories) { Response.Write(cat); if (e.Categories.Last().Term != cat.Term) Response.Write(", "); }%></td>
<td class="anns"><%= e.Total %></td>
<td class="date"><%= Html.DateTimeAbbreviation(e.Date, (dto, tzi) => dto.ToString("g")) %><br />
<%= e.Published.HasValue ? e.Published.Value < DateTimeOffset.Now ? "Published" : "Scheduled" : "Updated" %>

</td>
</tr>
<%} %>
</tbody>

</table>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('select[name=category]').change(function () {
            if ($(this).val.length > 0) location.search = '?category=' + $(this).val();
        });
    });

</script>
</asp:Content>
