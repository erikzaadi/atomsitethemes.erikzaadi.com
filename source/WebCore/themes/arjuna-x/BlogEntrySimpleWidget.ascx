<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<EntryModel>" %>
<%if (Model.Entry != null)
  { %>
<h4>
    <span>
        <%= Model.Entry.Title.Text%></span></h4>
<div class="content">
    <%-- Only show first half of entry --%>
    <%= Model.Entry.ContentBeforeSplit.ToString()%>
    <% if (Model.Entry.Content.IsExtended)
       { %>
    <a class="more" href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>">Read More</a>
    <%} %>
</div>
<%}
  else
  { %>
<h3>
    Not Found</h3>
<div class="content">
    <em>The entry is missing. Please check the log for more information.</em>
</div>
<%} %>
