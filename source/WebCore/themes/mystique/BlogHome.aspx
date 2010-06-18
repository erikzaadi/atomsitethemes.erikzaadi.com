<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="ViewPage<PageModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title><%= Model.Workspace.Title + "-" + Model.Workspace.Subtitle %></title>
  <link rel="service" type="application/atomsvc+xml" href="<%= Url.RouteUrlEx("AtomPubService", AbsoluteMode.Force) %>" />
  <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AtomPubFeed", Model.Collection.Id) %>" title="<%= Model.Collection.Title %>" />
  <link rel="alternate" type="application/atom+xml" href="<%= Url.RouteIdUrl("AnnotateAnnotationsFeed", Model.Collection.Id) %>" title="<%= Model.Collection.Title %> Comments Feed" />
  <link rel="wlwmanifest" type="application/wlwmanifest+xml" href="<%= Url.RouteIdUrl("BlogWriterManifest", Model.Collection.Id, AbsoluteMode.Force) %>" />
</asp:Content>
