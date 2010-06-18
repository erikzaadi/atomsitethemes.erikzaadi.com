<%@ Page Language="C#" MasterPageFile="Admin.Master" AutoEventWireup="true" Inherits="ViewPage<AdminProgressModel>" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<title>Progress &rsaquo; AtomSite Manager</title>
<style type="text/css">#msgs {border:1px solid #888; margin:0.4em;height:400px; overflow:auto} #msgs li {list-style-type:none; margin:0.1em; padding:0.2em}</style>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
<h2><%= Model.ProcessName %></h2>
<ul id="msgs">
<% bool alt = true; foreach (string msg in Model.Messages)  {alt = !alt; %>
<li style="<%= alt ? "background:#fcfcfc" : string.Empty %>"><%= msg %></li>
<%} %>
</ul>
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
<script type="text/javascript">
$(document).ready(function() {
  window.setTimeout(function() { updateProgress();  }, 300);
});

function updateProgress() {
  $.getJSON('<%= Model.ProgressUrl %>', function(data) {
    $.each(data.Messages, function(i, msg) {
      var $li = $('<li>' + msg + '</li>');
      $('#msgs').append($li);
      $('#msgs>li:odd').css('background-color', '#fff').css('color', '#444');
    });

    if (data.PercentComplete < 100) {
      window.setTimeout(function() { updateProgress(); }, 300);
    }
    
    scrollIntoView($('#msgs>li:last'), $('#msgs'));
  });
}
</script>
</asp:Content>
