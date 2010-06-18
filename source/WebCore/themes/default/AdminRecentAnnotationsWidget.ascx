<%@ Control Language="C#" Inherits="ViewUserControl<AdminAnnotationsModel>" %>
<div id="recanno" class="widget items">
  <h3>Recent Annotations</h3>

  <% if (Model.Annotations.Count > 0) { %>
  <ol>
    <% bool alt = true; foreach (AtomEntry a in Model.Annotations) { alt = !alt;%>
    <li id="<%= a.Id.ToFullWebId() %>" class="annotation <%= !a.Approved ? " pending" : string.Empty %><%= alt ? " alt" : string.Empty %>">
	      <%=Html.GravatarImg(a.Authors.First().Email, 50) %>
		    <h4>From <cite class="author"><%=a.Authors.First().Name %></cite> on <a href='<%= Url.Action("EditEntry", "Admin", new { id = a.Id}) %>'><%= a.GetValue<string>(Atom.SvcNs + "parentTitle") %></a> <a class="link" href="<%= a.LocationWeb %>">#</a> <span class='status'><%= !a.Approved? "[Not Approved]" : string.Empty %></span></h4>
        <blockquote>
        <%= a.Text.ToStringPreview(120) %>
        </blockquote>
        <div class="actions">
<% foreach (AtomLink link in a.Links) { %>
<a rel="<%= link.Rel %>" href="<%= link.Href %>"><%= link.Title %></a><% if (!a.Links.LastOrDefault().Equals(link)) {%> | <%} %>
<%} %>
        </div>
    </li>
  <% } %>
  </ol>

  <form class="more" method="get" action="<%= Url.Action("Annotations", "Admin", new { workspace=Model.Scope.Workspace, collection = Model.Scope.Collection}) %>">
    <button class="white" type="submit">View All</button>
  </form>

<%} else { %>
  <p class="warning">There are no annotations to show.</p>
<%} %>
</div>
