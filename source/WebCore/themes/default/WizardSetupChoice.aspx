<%@ Page Language="C#" MasterPageFile="Wizard.Master" AutoEventWireup="true" Inherits="ViewPage<PageModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title>Setup AtomSite - Wizard</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <h1>Setup AtomSite</h1>
  <div class="taskOption">
    <form action="<%= Url.Action("BasicSettings") %>" method="get"><fieldset>
      <input type="submit" name="default" value="Use Default Setup" />
      Setup AtomSite using the default settings. This includes three collections for your
      blog, pages, and media.
    </fieldset></form>
  </div>
  <% Html.RenderWidgets("setupOptions"); %>
  <div class="taskOption notSupported">
    <form action="<%= Url.Action("AdvancedSettings") %>" method="get"><fieldset>
      <input type="submit" name="advanced" value="Advanced Setup" />
      <em>(Advanced)</em> Setup AtomSite using custom settings. Choose your collection
      names and setup the administrators, authors, and contributors.
    </fieldset></form>
  </div>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
  <script type="text/javascript">    $(document).ready(function() {
      $('.notSupported').fadeTo(1000, 0.4);
      $('.notSupported input[type=submit]').click(function() { alert('This option is not yet supported.'); return false; });
    });
  </script>
</asp:Content>
