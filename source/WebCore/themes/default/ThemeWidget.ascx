<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AtomSite.WebCore.plugins.ThemeSwitcher.ThemeModel>" %>
<div>
    <h3>
        Description:</h3>
    <div>
        <%=Model.Theme.Text.Text%></div>
    <h3>
        Compatibility</h3>
    <div>
        <%=string.Join(", ", Model.Theme.CompatibleVersions.ToArray())%></div>
    <div>
        <a href="<%=Model.Theme.Screenshot%>" target="_blank">
            <img src="<%=Model.Theme.Thumbnail%>" alt="Thumbnail" />
        </a>
    </div>
    <div>
        &nbsp;</div>
    <p>
        <%
            if (Model.Theme.Homepage != Request.Url)
            {%>
        <a target="_blank" href="<%=Model.Theme.Homepage%>">Visit
            <%=Model.Theme.Name%>
            Homepage </a>|
        <%
            }
        %><a href="<%= Url.Action("Download", "ThemeSwitcher", new { ThemeName = Model.Theme.Name }) %>">
            Download </a>| <em>Downloaded
                <%= Model.Downloaded %>
                times </em>
    </p>
    <div>
        &nbsp;</div>
    <div id="LivePreview">
        <h3>
            Live Preview</h3>
        <noscript>
            <div>
                <a href="<%= Model.LivePreviewUrl %>" target="_blank">Show Preview</a>
            </div>
            <div id="errors">
                <span class="error">If you visit this page with javascript enabled, you'd be able to
                    switch between the different PageWidths and PageTemplates...</span></div>
        </noscript>
        <div class="JSLivePreview">
            <%= Html.Hidden("ThemeName", Model.Theme.Name)%>
            <%= Html.Hidden("LivePreviewBaseUrl", Model.LivePreviewBaseUrl)%>
            <div>
                &nbsp;</div>
            <div>
                <label for="PageWidth">
                    Width</label>
                <%= Html.DropDownList("PageWidth", Model.Theme.Widths.Select(p=>new SelectListItem { Text = p}))%>
            </div>
            <div>
                <label for="PageTemplate">
                    Template (Columns )</label>
                <%= Html.DropDownList("PageTemplate", Model.Theme.Templates.Select(p=>new SelectListItem { Text = p}))%>
            </div>
            <div>
                <label>
                    &nbsp;</label>
                <a id="LiveLink" href="<%= Model.LivePreviewUrl %>" target="_blank">Show Preview</a>
            </div>
        </div>
    </div>
</div>
