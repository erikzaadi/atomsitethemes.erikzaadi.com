<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtomSite.Plugins.TwitterPlugin.TwitterTimeline>" %>
<div id="twitter">
	
	<% if (Model.Statuses.Count() == 0) { %>

  <h3>Tweets</h3>
	<em class="empty">No Tweets.</em>

  <% } else { %>

  <h3>Tweets by <a href="http://www.twitter.com/<%= Model.Statuses.First().User.ScreenName %>" title="Follow <%= Model.Statuses.First().User.ScreenName %> on Twitter">
  <strong><%= Model.Statuses.First().User.ScreenName %></strong></a> <small><%= Model.Statuses.First().User.Followers %> followers</small></h3>
  
  <%-- <img src="<%= Model.Statuses.First().User.ProfileImageUrl %>" alt="icon" border="0" /> --%>
    
  <ol>
    <% foreach (AtomSite.Plugins.TwitterPlugin.TwitterStatus status in Model.Statuses) { %>
    <li class="tweet" id="twitter_<%= status.Id %>"><%= status.Text%> <em><%= Html.DateTimeAgoAbbreviation(status.CreatedAt)%></em></li>
    <% } %>
  </ol>

  <div class="follow">
    <a href="http://www.twitter.com/<%= Model.Statuses.First().User.ScreenName %>" title="Follow <%= Model.Statuses.First().User.ScreenName %> on Twitter">
      <img src="http://twitter-badges.s3.amazonaws.com/follow_me-c.png" alt="Follow <%= Model.Statuses.First().User.ScreenName %> on Twitter"/></a>
  </div>
  <%} %>
</div>
