<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<EntryModel>" %>

<li class="block">
    <div class="block clearfix">
        <%if (Model.Entry != null)
          { %>
        <h3 class="title">
            <span><%= Model.Entry.Title.Text%></span></h3>
        <div class="block-div">
        </div>
        <div class="block-div-arrow">
        </div>
        <div>
            <%-- Only show first half of entry --%>
            <%= Model.Entry.ContentBeforeSplit.ToString()%>
            <% if (Model.Entry.Content.IsExtended)
               { %>
            <a class="more" href="<%= Url.RouteIdUrl("BlogEntry", Model.Entry.Id) %>">Read More</a>
            <%} %>
        </div>
        <%}
          else
          { %>
        <h3>
            Not Found</h3>
        <div class="content">
            <em>The entry is missing. Please check the log for more information.</em>
        </div>
        <%} %>
    </div>
</li>
