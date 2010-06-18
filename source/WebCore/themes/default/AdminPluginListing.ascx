<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AdminPluginsModel>" %>

<table class="plugins"><thead>
<tr><th>Merit</th><th>Plugin</th><th>Assembly</th><%--<th>Categories</th><th>Author</th><th>Widgets</th><th>Pages</th><th>Routes</th>--%><th></th></tr></thead>
<tfoot>
<tr><td colspan="3"><a href="http://atomsite.net/plugins.xhtml">Find More Plugins</a>
</td><td class="total">Total Plugins: <strong><%=Model.Plugins.Count() %></strong></td></tr></tfoot>
<tbody>
<% bool alt = true; foreach (PluginState e in Model.Plugins) { alt = !alt;%>
<tr id="<%= e.Type %>" class="plugin<%= (alt? " alt" : string.Empty) + (!e.Enabled ? " disabled" : string.Empty) + (Model.ChangedType == e.Type ? " greenbg" : string.Empty) %>">
<td class="merit"><div class="meritval"><%= e.Merit %></div><form action="<%= Url.Action("SetPluginMerit", "Admin") %>" method="post"><input type="hidden" name="pluginType" value="<%= e.Type %>" /><button type="submit" name="change" value="next" class="<%= Merit.HasNext(e.Merit) ? string.Empty : "disabled" %>"><img src="<%= Url.ImageSrc("next.png") %>" alt="Move to next merit level" /></button><button type="submit" name="change" value="add" class="<%= e.Merit == int.MaxValue ? "disabled" : string.Empty %>"><img src="<%= Url.ImageSrc("add.png") %>" alt="Add one to merit level" /></button><span><%= Merit.MeritToName(e.Merit)%></span><button type="submit" name="change" value="subtract" class="<%= e.Merit == int.MinValue ? "disabled" : string.Empty %>"><img src="<%= Url.ImageSrc("subtract.png") %>" alt="Subtract one from merit level" /></button><button type="submit" name="change" value="prev" class="<%= Merit.HasPrev(e.Merit) ? string.Empty : "disabled" %>"><img src="<%= Url.ImageSrc("prev.png") %>" alt="Move to previous merit level" /></button></form></td>
<td class="title"><%--<img src='<%= Url.ImageSrc(e.Installed ? e.Enabled ? "green.png" : "red.png" : "gray.png") %>' alt='status' />--%>
<strong>
<% if (Model.HasPluginEntry(e.Type)) { %>
<a href="<%= Url.Action("Plugin", "Admin", new { type = Type.GetType(e.Type).FullName }) %>"><%= e.Name %></a>
<% } else { %>
<%= e.Name %>
<% } %>
</strong> <small><%= e.Version %></small></td>
<td class="assembly"><%= e.Assembly %></td>
<td class="ctrl">
<% if (Model.ChangedType == e.Type && !string.IsNullOrEmpty(Model.MessageForType)) { %>
<%= Model.MessageForType %>
<% } else { %>

<form action="<%= Url.Action("ControlPlugin", "Admin") %>" method="post">
<input type="hidden" name="pluginType" value="<%= e.Type %>" />
<% if (e.Status == PluginStatus.Incompatible) { %>
<small style="color:#a90000">Possibly incompatible, force<button name="change" value="install">Install</button></small>
<% } else if (e.Status == PluginStatus.NeedSetup) {  %>
<strong>Setup</strong>, please restart</small>
<% } else {%>
<% if (e.Enabled) { %><button name="change" value="disable" style="visibility:<%= e.Setup ? "visible" : "hidden" %>">Disable</button>
<% } 
   else {%><button name="change" value="enable" style="visibility:<%= e.Setup ? "visible" : "hidden" %>">Enable</button>
<% } %><button name="change" value="setup" style="visibility:<%= e.Installed ? "visible" : "hidden" %>">Re-setup</button>
<% if (e.Installed) 
   { %><button name="change" value="uninstall" style="visibility:<%= Model.CanUninstall(e.Type) ? "visible" : "hidden" %>">Uninstall</button><% }
   else { %><button name="change" value="install">Install</button><% } } %>
</form>
<% } %>
</td>
</tr>
<% } %>
</tbody>

</table>
