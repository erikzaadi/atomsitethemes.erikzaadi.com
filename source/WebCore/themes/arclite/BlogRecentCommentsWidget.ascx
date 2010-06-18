<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h3>
    Recent Comments</h3>
<% if (Model.EntryCount > 0)
   { %>
<ul style="list-style: none;">
    <% foreach (AtomEntry entry in Model.Feed.Entries)
       {
           if (entry.AnnotationType == null || !entry.AnnotationType.EndsWith("back"))
           {%>
    <li><a class="fadeThis" href="<%=Url.RouteIdUrl("BlogEntry", entry.Id.GetParentId()) + "#" + entry.Id.ToWebId()%>">
        <blockquote>
            <p>
                <%=entry.Text.ToStringPreview(64)%>
                <br />
                <%=entry.Authors.Count() > 0 ? "- " + entry.Authors.First().ToString() : string.Empty%>
                <br />
                <%=Html.DateTimeAbbreviation(entry.Date, (d, tz) => d.ToString("g"))%>
            </p>
        </blockquote>
    </a></li>
    <%
        }
       }%>
</ul>
<% }
   else
   { %>
<div>
    <em id="commentsEmpty">None.</em>
</div>
<% } %>
