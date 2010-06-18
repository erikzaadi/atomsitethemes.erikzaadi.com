<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<MenuModel>" %>
<!-- main navigation -->
<div id="nav-wrap1">
    <div id="nav-wrap2">
        <ul id="nav">
            <%
                foreach (AtomSite.WebCore.MenuItem item in Model.MenuItems)
                {%>
            <li><a class="fadeThis" href="<%=item.Href%>" title="<%=item.Title%>"><span>
                <%=item.Text%></span></a> </li>
            <%
                }%>
            <%
                if (Model.AuthorizeService.GetScopes(Model.User).Count() > 0)
                {%>
            <li><a class="fadeThis" href="<%=Url.Action("Dashboard", "Admin")%>"><span>Dashboard</span></a></li>
            <%
                }%>
            <%
                if (Model != null && Model.Collection != null && Model.Collection.Id != null)
                {%>
            <li><a href="#" class="fadeThis"><span>Subscribe</span></a>
                <ul>
                    <li><a class="fadeThis" href="<%=Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id)%>"
                        title="<%=Model.Collection.Title%>"><span>Entries Feed</span></a></li>
                    <li><a class="fadeThis" href="<%=Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id)%>"
                        title="<%=Model.Collection.Title%>"><span>Comments Feed</span></a></li>
                </ul>
            </li>
            <%
                }
            %>
        </ul>
    </div>
</div>
<!-- /main navigation -->
