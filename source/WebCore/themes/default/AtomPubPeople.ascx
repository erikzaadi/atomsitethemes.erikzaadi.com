<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<IEnumerable<AtomPerson>>" %>
<% foreach (AtomPerson person in ViewData.Model) { %>
    <% if (person.Uri != null) { %>
    <a class="person" href="<% Response.Write(person.Uri); %>"><%= person.Name %></a><%= Model.LastOrDefault().Equals(person) ? string.Empty : ", " %>
    <% } else { %>
    <%= person.Name %><%= Model.LastOrDefault().Equals(person) ? string.Empty : ", " %>
    <% } %>
<% } %>