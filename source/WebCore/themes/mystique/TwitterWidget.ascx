<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.Plugins.TwitterPlugin.TwitterTimeline>" %>
<%@ Import Namespace="ThemeExtensions.HtmlHelpers" %>

<li class="block">
    <div class="block-twitter clear-block" id="instance-twitterwidget-3">
        <h3 class="title">
            <span>Latest tweets</span></h3>
        <div class="block-div">
        </div>
        <div class="block-div-arrow">
        </div>
        <div class="twitter-content clear-block" id="twitterwidget-3">
            <% if (Model.Statuses.Count() == 0)
               { %>
            <em class="empty">No Tweets.</em>
            <% }
               else
               {%>
            <div class="clearfix">
                <div class="avatar">
                    <img src="<%= Model.Statuses.First().User.ProfileImageUrl %>" alt="<%= Model.Statuses.First().User.ScreenName %> " /></div>
                <div class="info">
                    <a href="http://www.twitter.com/<%= Model.Statuses.First().User.ScreenName %>" id="twitterId">
                        <%= Model.Statuses.First().User.ScreenName %>
                    </a>
                    <br />
                    <span class="followers">
                        <%= Model.Statuses.First().User.Followers %>
                        followers</span></div>
            </div>
            <ul>
                <%
                    foreach (AtomSite.Plugins.TwitterPlugin.TwitterStatus status in Model.Statuses)
                    {%>
                <li><span class="entry">
                    <%= Html.ThemeExtensions().Social.MakeTwitterContentClickable(status.Text)%><a class="date" href="http://twitter.com/<%= Model.Statuses.First().User.ScreenName %>/statuses/<%= status.Id %>"
                        rel="nofollow"><%= Html.DateTimeAgoAbbreviation(status.CreatedAt)%></a></span></li><%
                                                                                                               }%>
            </ul>
            <%
                }%>
        </div>
        <a class="followMe" href="http://www.twitter.com/<%= Model.Statuses.First().User.ScreenName %>">
            <span>Follow me on Twitter!</span></a>
    </div>
</li>
