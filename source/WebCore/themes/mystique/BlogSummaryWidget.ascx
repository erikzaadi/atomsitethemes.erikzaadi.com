<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>
<h3>
    <%= Model.Feed.Title %></h3>
<% if (Model.EntryCount > 0)
   { %>
<ol class="entries">
    <% foreach (var entry in Model.Feed.Entries)
       {%>
    <li>
        <!-- post -->
        <div class="post">
            <div class="post-header">
                <h3 class="post-title">
                    <a href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">
                        <%=entry.Title.Text%></a></h3>
                <p class="post-date">
                    <span class="month">
                        <%=entry.Date.ToString("MMM")%></span> <span class="day">
                            <%=entry.Date.ToString("dd")%></span>
                </p>
                <p class="post-author">
                    <span class="info">posted by
                        <%
                            Html.RenderPartial("AtomPubPeople", entry.People, new ViewDataDictionary() { { "id", entry.Id } });%>
                        <%
                            if (entry.Categories.Count() > 0)
                            {%>
                        In :
                        <%
                            Html.RenderPartial("BlogCategories",
                                               new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                            }%>
                        | <a href="<%=Url.RouteIdUrl("BlogEntry", entry.Id)%>#comments" class="comments">Comments
                            (<%=entry.Total%>)</a> </span>
                </p>
            </div>
            <div class="post-content clearfix">
                <p>
                    <%=entry.Text.ToStringPreview(300)%>
                    <%
                        if (entry.Text.ToString().Length > 300)
                        {%>
                    <a class="more" href="<%=Url.RouteIdUrl("BlogEntry", entry.Id)%>">Read More</a>
                    <%
                        }%>
                </p>
                <p class="tags">
                    <%
                        if (entry.Categories.Count() > 0)
                        {%>
                    Tags:
                    <%
                        Html.RenderPartial("BlogCategories",
                                           new CategoriesModel() { Categories = entry.Categories, Id = Model.Collection.Id });
                        }%>
                </p>
            </div>
        </div>
        <!-- /post -->
        <br />
    </li>
    <%
        }%>
</ol>
<%}
   else
   { %>
<div style="color: Red;">
    There is no feed to display.</div>
<br />
<%} %>
