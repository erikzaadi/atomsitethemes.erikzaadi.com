<%@ Control Language="C#" Inherits="ViewUserControl<AdminEntriesModel>" %>
<div id="pendent" class="widget items">
  <h3>Pending Entries</h3>
  
  <% if (Model.Entries.Count > 0) { %>
  <ol>
  <% bool alt = true; foreach (AtomEntry e in Model.Entries) { alt = !alt;%>
    <li id="<%= e.Id.ToFullWebId() %>" class="entry pending <%= e.Approved ? " approved"  : string.Empty %><%= alt ? " alt" : string.Empty %>">
	    <h4><a href="<%= Url.Action("EditEntry", "Admin", new { id = e.Id.ToString() }) %>"><%= e.Title %></a> <%= Html.DateTimeAbbreviation(e.Date, (dto, tzi) => dto.ToString("g")) %></h4>
	    <blockquote><%= e.Text.ToStringPreview(300) %></blockquote>
	    <div>
	      <span class="draft"><%= e.Draft ? "[Draft]" : string.Empty%></span> <span class='status'><%= !e.Approved ? "[Not Approved]" : string.Empty %></span>
	    </div>
    </li>
  <% } //end foreach %>
  </ol>

    <form class="more" method="get" action="<%= Url.Action("Entries", "Admin", new { workspace=Model.Scope.Workspace, collection = Model.Scope.Collection}) %>">
      <button class="white" type="submit" name="filter" value="pending">View All</button>
    </form>

<%} else { %>
  <p class="warning">There are no draft entries or entries pending approval.</p>
<%} %>

</div>