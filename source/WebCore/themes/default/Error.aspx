<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<ErrorModel>" %>
<asp:Content ID="Body" ContentPlaceHolderID="content" runat="server">
    <h3>Error</h3>
    <p>
        Sorry, an error occurred while processing your request.  The error was been logged and will be reviewed by the site administrator.
    </p>
    <% if (Model.User.IsAuthenticated && Model.AuthorizeService.GetRoles(Model.User, Scope.EntireSite) > AuthRoles.User) { %>
    <h4>Error Details</h4>
    <div id="errors">
    <%= Model.HandleErrorInfo.Exception != null ? Model.HandleErrorInfo.Exception.Message : "(no exception)"%>
    </div>
    <%} %>
</asp:Content>
