<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AtomSite.Plugins.Rater.RaterModel>" %>

<div id="rating" class="stat<%= Model.CanRate ? " canrate" : string.Empty %>">
    <label for="rating">Rating</label>
    <div class="statVal"><%-- add rate url if it can be rated --%>
    <%if (Model.CanRate) { %><form action="<%= Model.PostHref %>" method="post"><fieldset> <%} %>
		<span class="ui-rater">
			<span class="ui-rater-starsOff" style="width:90px;">
				<span class="ui-rater-starsOn" style="width:<%= Math.Round(18F * Model.Rating) %>px"></span>
			</span>
			<span class="ui-rater-rating"><%= Model.Rating.ToString("0.0") %></span>
			(<span class="ui-rater-rateCount"><%= Model.RatingCount%></span>)
		</span>
        <%if (Model.CanRate) { %></fieldset></form><%} %>
    </div>
</div>