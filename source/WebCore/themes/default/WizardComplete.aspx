<%@ Page Language="C#" MasterPageFile="Wizard.Master" AutoEventWireup="true" Inherits="ViewPage<PageModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title>Complete - Wizard</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <h1>Setup Complete</h1>
   <p>AtomSite is setup and ready to use.  Please stay up to date by visiting <a href="http://atomsite.net">http://atomsite.net</a>.</p>
   <p>You are being redirected to your new blog in <span id="timeSecs">6</span> seconds.</p>
   <h3><a href="<%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath %>"><%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath%></a></h3>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">

    <script type="text/javascript">      $(document).ready(function() {
        tick = setInterval(onTick, 1000);
      });
      var tick;
      var time = 6;
      function onTick() {
        time = time - 1;
        $('#timeSecs').text(time.toString());
        if (time <= 2) {
          window.location = '<%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath%>';
          clearInterval(tick);
        }
      }
    </script>
</asp:Content>
