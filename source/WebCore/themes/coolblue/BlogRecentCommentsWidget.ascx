<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<div>
    <h3>
        Recent Comments</h3>
    <% if (Model.EntryCount > 0)
       { %>
    <div class="recent-comments">
        <ul>
            <% foreach (AtomEntry entry in Model.Feed.Entries)
               { %>
            <li><a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id.GetParentId()) + "#" + entry.Id.ToWebId() %>">
                <%=  entry.AnnotationType != null && entry.AnnotationType.EndsWith("back") ? entry.Title.ToString() :  entry.Text.ToStringPreview(48) %>
            </a>
                <br />
                <cite>
                    <%= entry.Authors.Count() > 0 && entry.Authors.First().ToString().Trim().Length > 0 ? "- " + entry.Authors.First().ToString() : string.Empty%></cite>
            </li>
            <%} %>
        </ul>
    </div>
    <% }
       else
       { %>
    <div style="color: Red;">
        There is nothing to display.<br />
    </div>
    <% } %>
</div>
