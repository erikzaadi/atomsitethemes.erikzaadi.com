<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CategoriesModel>" %>
<% foreach (AtomCategory cat in Model.Categories) {%>
    <a class="category" href="<%= Url.RouteIdUrl("BlogCategory", Model.Id, new {term = cat.Term})%>"><%= cat %></a><% if (!cat.Equals(Model.Categories.LastOrDefault())) Response.Write(", "); %>
<%}%>