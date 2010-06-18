<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<FeedModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <title>
        <%= Model.Title %></title>
    <link rel="service" type="application/atomsvc+xml" href="<%= Url.RouteUrlEx("AtomPubService", AbsoluteMode.Force) %>" />
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id) %>"
        title="<%= Model.Collection.Title %>" />
    <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id) %>"
        title="<%= Model.Collection.Title %> Comments Feed" />
    <link rel="wlwmanifest" type="application/wlwmanifest+xml" href="<%= Url.RouteIdUrl("BlogWriterManifest", Model.Feed.Id, AbsoluteMode.Force) %>" />
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
    <%
        foreach (AtomEntry entry in Model.Feed.Entries)
        {%>
    <div class="post" id="post-<%=entry.Id%>">
        <h2>
            <a rel="bookmark" class="title" href="<%= Url.RouteIdUrl("BlogEntry", entry.Id) %>">
                <%=entry.Title.Text%>
        </h2>
        <div class="info">
            <span class="date">
                <%=Html.DateTimeAbbreviation(entry.Date,
                                                        (d, tz) => d.ToString("m") + " at " + d.ToString("t"))%></span>
            <%
                if (Model.Collection.AnnotationsOn)
                {%>
            <span class="addcomment"><a href="<%=Url.RouteIdUrl("BlogEntry", entry.Id)%>#respond">
                Leave a comment</a></span> <span class="comments"><a href="<%=Url.RouteIdUrl("BlogEntry", entry.Id)%>#comments">
                    Go to comments</a></span>
            <%
                }%>
            <div class="fixed">
            </div>
        </div>
        <div class="content">
            <%=entry.IsExtended ? entry.ContentBeforeSplit.ToString() : entry.Text.Text%>
            <%
                if (entry.IsExtended)
                    Response.Write("<a class=\"more\" href=\"" + Url.RouteIdUrl("BlogEntry", entry.Id) + "\">Read More</a>");%>
            <div class="fixed">
            </div>
        </div>
    </div>
    <%
        }%>
    <div id="postnavi">
        <%
            if (Url.GetNextPage(Model.Feed.Links) != null)
            {%>
        <span class="prev"><a href="<%=Url.GetNextPage(Model.Feed.Links)%>">Older Entries</a></span>
        <%
            }
        if (Url.GetPrevPage(Model.Feed.Links) != null)
        {%>
        <span class="next"><a href="<%=Url.GetPrevPage(Model.Feed.Links)%>">Newer Entries</a></span>
        <%
            }
        %>
        <div class="fixed">
        </div>
    </div>
</asp:Content>
