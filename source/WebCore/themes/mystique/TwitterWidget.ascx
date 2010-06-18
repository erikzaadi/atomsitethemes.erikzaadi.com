<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.Plugins.TwitterPlugin.TwitterTimeline>" %>
<script runat="server" language="C#">
    protected string ReplaceTweetContent(string Content)
    {
        return Regex.Replace(Regex.Replace(make_clickable(Content), @"\@(\w+)", "<a rel=\"nofollow\" href=\"http://twitter.com/$1\">@$1</a>"),
            @"\#(\w+)", "<a rel=\"nofollow\" href=\"http://twitter.com/#search?q=%23$1\">#$1</a>");
    }
    protected string make_clickable(string Content)
    {
        Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

        MatchCollection mactches = regx.Matches(Content);

        foreach (Match match in mactches)
        {
            Content = Content.Replace(match.Value, "<a href='" + match.Value + "' rel=\"nofollow\">" + match.Value + "</a>");
        }

        return Content;
    }
</script>
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
                    <%= ReplaceTweetContent(status.Text)%><a class="date" href="http://twitter.com/<%= Model.Statuses.First().User.ScreenName %>/statuses/<%= status.Id %>"
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
