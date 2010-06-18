<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminWidgetsModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Widgets &rsaquo; AtomSite Manager</title>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2>Widgets</h2>
<div class="info">
<p>
<img src="<%= Url.ImageSrc("scopeskey.png") %>" alt="Scopes Key" />
When you include a widget into an area on a page for the entire site, it will show in every workspace and collection.  Widgets included in a master page show on every child page.</p>
</div>
<div class="yui-gd">
    <div class="yui-u first">
    <% Html.RenderPartial("AdminTargets", Model); %>
    </div>
    <div class="yui-u yui-gd">
        <div class="yui-u first">
            <% Html.RenderPartial("AdminAreas", Model); %>
        </div>
        <div class="yui-u">
            <% Html.RenderPartial("AdminIncludes", Model); %>
        </div>
    </div>
  </div>

</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
    $().ready(function () {
        $('.selected').each(function () { scrollIntoView($(this), $(this).parent()) });
        $('body').append('<div id="configure" class="overlay"><div class="wrap"/></div>');
        wireConfig();
    });
</script>
</asp:Content>
