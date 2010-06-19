<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<FeedModel>" %>

<%@ Import Namespace="ThemeExtensions.HtmlHelpers" %>
<%
    var showAdvancedMenu = Html.ThemeExtensions().Theme.GetThemeBooleanProperty("showadvancedmenu", false); 

    if (showAdvancedMenu)
    {
%>
<div id="instance-sidebartabswidget-section-archives" class="box section" style="display: none;">
    <div class="box-top-left">
        <div class="box-top-right">
        </div>
    </div>
    <div class="box-main">
        <div class="box-content">
            <%
                if (Model.EntryCount > 0)
                {%>
            <ul class="menuList">
                <%
                    foreach (int year in Model.GetYears())
                    {%>
                <%
                    foreach (int month in Model.GetMonths(year))
                    {%>
                <li class="cat-item"><a class="fadeThis" href="<%=Url.RouteIdUrl("BlogDateMonth", Model.Feed.Id,
                                                    new {year = year, month = month.ToString("00")})%>">
                    <span class="entry">
                        <%=new DateTime(year, month, 1).ToString("MMMM yyyy")%>
                        <span class="details inline">(<%=Model.GetEntries(year, month).Count()%>)</span></span><span
                            class="hover" style="opacity: 0;"></span></a></li>
                <%
                    }
                    }%>
            </ul>
            <%
                }
                else
                {%>
            <div style="color: Red;">
                There is nothing to display.</div>
            <br />
            <%
                }%>
        </div>
    </div>
</div>
<%
}
    else
    {%>
<li class="block">
    <div class="block clearfix">
        <h3 class="title">
            <span>Archive</span></h3>
        <%
            if (Model.EntryCount > 0)
            {%>
        <div class="block-div">
        </div>
        <div class="block-div-arrow">
        </div>
        <ul>
            <%
                foreach (int year in Model.GetYears())
                {%>
            <%
                foreach (int month in Model.GetMonths(year))
                {%>
            <li><a href="<%=Url.RouteIdUrl("BlogDateMonth", Model.Feed.Id,
                                                        new {year = year, month = month.ToString("00")})%>">
                <span class="entry">
                    <%=new DateTime(year, month, 1).ToString("MMMM yyyy")%>
                    <span class="details inline">(<%=Model.GetEntries(year, month).Count()%>)</span></span><span
                        class="hover" style="opacity: 0;"></span></a></li>
            <%
                }
            }%>
        </ul>
        <%
            }
        else
        {%>
        <div style="color: Red;">
            There is nothing to display.</div>
        <br />
        <%
            }%>
    </div>
</li>
<%
}%>
