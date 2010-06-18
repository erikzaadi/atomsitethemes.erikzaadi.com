<%@ Control Language="C#" Inherits="ViewUserControl<AdminUserSelectModel>" %>
<div class="widget settings selection">
  <h3><a rel="cancel" href="<%= Model.CancelHref %>" title="Cancel">Cancel</a><%= Model.SelectionTitle %></h3>
  <ul>
    <%bool alt = false; foreach (User user in Model.Users) { alt = !alt; %>
    <li class="<%= alt ? "alt" : string.Empty %>" id="<%= user.Ids.First() %>">
    <%= Html.GravatarImg(user.Email, 40)%>
      <div class="actions"><a rel="select" title="<%= Model.SelectionTitle %>" href="<%= Model.GetPostHref(user) %>">select</a></div>
      <h4><%= user.Name%></h4>
      <div><%= user.Email%></div>
    </li><%} %>
  </ul>
  <%-- TODO: paging --%>
</div>
