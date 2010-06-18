<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<% 
    var showAdvancedMenu = false;

    if (showAdvancedMenu)
    {
%>
<div id="instance-sidebartabswidget-section-recentcomments" class="box section" style="display: none;">
    <div class="box-top-left">
        <div class="box-top-right">
        </div>
    </div>
    <div class="box-main">
        <div class="box-content">
            <% if (Model.EntryCount > 0)
               { %>
            <ul class="menuList recentcomm">
                <% foreach (AtomEntry entry in Model.Feed.Entries)
                   {
                       if (entry.AnnotationType == null || !entry.AnnotationType.EndsWith("back"))
                       {%>
                <!-- <%=Html.DateTimeAbbreviation(entry.Date, (d, tz) => d.ToString("g"))%> -->
                <li class="clearfix"><a href="<%=Url.RouteIdUrl("BlogEntry", entry.Id.GetParentId()) + "#" + entry.Id.ToWebId()%>">
                    <span class="avatar">
                        <img height="32" width="32" class="avatar" src="<%=Url.GetGravatarHref(entry.Authors.First().Email, 32)
                          + Request.Url.GetLeftPart(UriPartial.Authority)
                          + Url.ImageSrc("noav.png")%>" alt="avatar"></span> <span class="entry">
                              <%=entry.Authors.Count() > 0 ? "- " + entry.Authors.First().ToString() : string.Empty%>:
                              <span class="details">
                                  <%=entry.Text.ToStringPreview(64)%></span></span></a></li>
                <%
                    }
                   }%>
            </ul>
            <%
                }
               else
               { %>
            <div>
                <em id="commentsEmpty">None.</em>
            </div>
            <% } %>
        </div>
    </div>
</div>
<%}
    else
    { %>
<li class="block">
    <div class="block clearfix">
        <h3 class="title">
            <span>Recent Comments</span></h3>
        <% if (Model.EntryCount > 0)
           { %>
        <ul class="recentcomm">
            <% foreach (AtomEntry entry in Model.Feed.Entries)
               {
                   if (entry.AnnotationType == null || !entry.AnnotationType.EndsWith("back"))
                   {%>
            <!-- <%=Html.DateTimeAbbreviation(entry.Date, (d, tz) => d.ToString("g"))%> -->
            <li class="clearfix"><a href="<%=Url.RouteIdUrl("BlogEntry", entry.Id.GetParentId()) + "#" + entry.Id.ToWebId()%>">
                <span class="avatar">
                    <img height="32" width="32" class="avatar" src="<%=Url.GetGravatarHref(entry.Authors.First().Email, 32)
                          + Request.Url.GetLeftPart(UriPartial.Authority)
                          + Url.ImageSrc("noav.png")%>" alt="avatar"></span> <span class="entry">
                              <%=entry.Authors.Count() > 0 ? "- " + entry.Authors.First().ToString() : string.Empty%>:
                              <span class="details">
                                  <%=entry.Text.ToStringPreview(64)%></span></span></a></li>
            <%
                }
               }%>
        </ul>
        <%
            }
           else
           { %>
        <div>
            <em id="commentsEmpty">None.</em>
        </div>
        <% } %>
    </div>
</li>
<%} %>