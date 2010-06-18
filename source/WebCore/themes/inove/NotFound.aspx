<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true"
    Inherits="ViewPage<PageModel>" %>

<asp:Content ID="Body" ContentPlaceHolderID="content" runat="server">
    <div id="container">
        <div id="talker">
            <a href="http://www.neoease.com/">
                <img src="<%= Url.ImageSrc("lovelace.gif") %>" alt="Talker" /></a>
        </div>
        <div id="notice">
            <h1>
                Welcome to 404 error page!</h1>
            <p>
                Welcome to this customized error page. You've reached this page because you've clicked
                on a link that does not exist. This is probably our fault... but instead of showing
                you the basic '404 Error' page that is confusing and doesn't really explain anything,
                we've created this page to explain what went wrong.</p>
            <p>
                You can either (a) click on the 'back' button in your browser and try to navigate
                through our site in a different direction, or (b) click on the following link to
                go to homepage.</p>
            <div class="back">
                <a href="<%= Url.Content("~/") %>">Back to homepage &raquo;</a>
            </div>
            <div class="fixed">
            </div>
        </div>
        <div class="fixed">
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="head" ID="Head">
    <link rel="stylesheet" href="<%= Url.Content("~/css/inove/404.css") %>" type="text/css"
        media="screen" />
</asp:Content>
