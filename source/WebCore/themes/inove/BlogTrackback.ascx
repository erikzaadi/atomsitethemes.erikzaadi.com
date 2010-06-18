<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<CommentModel>" %>
<li class="trackback">
                <div class="date">
                    <%=Html.DateTimeAgoAbbreviation(Model.Comment.Date)%>
                    | <a href="#comment-<%=Model.Comment.Id.ToWebId()%>"><%=Model.Comment.Id.ToWebId()%></a>
                </div>
                <div class="act">
                    <% if (Model.User.IsAuthenticated)
                       { %>
                    <% if (!Model.Comment.Approved && Model.CanApprove())
                       { %>
                    <a href="#" onclick="approve('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubApproveEntry", Model.Comment.Id)%>', this)">
                        Approve</a>
                    <%} %>
                    <% if (Model.CanDelete())
                       { %>
                    <a href="#" onclick="del('<%= Model.Comment.Id.ToWebId() %>', '<%= Url.RouteIdUrl("AtomPubEntryEdit", Model.Comment.Id) %>')">
                        Delete</a>
                    <%} %>
                    <%} %>
                </div>
                <div class="fixed">
                </div>
                <div class="title">
                    <a href="<%=Model.Comment.Content.Src%>">
                        <%=Model.Comment.Title%></a>
                </div>
            </li>