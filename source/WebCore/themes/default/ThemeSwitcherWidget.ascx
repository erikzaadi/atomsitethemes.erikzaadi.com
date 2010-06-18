<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AtomSite.WebCore.plugins.ThemeSwitcher.ThemeSwitcherViewModel>" %>
<div id="ThemeSwitcher" class="dragcontainer">
    <button type="button" id="Drag">
        move</button>
    <button type="button" id="ShowHide">
        show</button>
    <div id="content">
        <h2>
            Theme Switcher
        </h2>
        <%if (Model.HasMessage)
          {%>
        <div class="<%= Model.IsMessageError ? "error" : "notice" %>">
            <%= Model.Message %></div>
        <%
            }
          using (Html.BeginForm("ChangeTheme", "ThemeSwitcher", HttpVerbs.Post))
          {%>
        <div>
            <fieldset>
                <legend>
                    <h3>
                        Theme</h3>
                </legend>
                <%=Html.DropDownList("ThemeName", Model.ThemeNames)%>
            </fieldset>
        </div>
        <div>
            <%
                foreach (var theme in Model.Themes)
                {%>
            <div class="ThemeInfo<%= theme.Selected ? " ThemeInfoSelected" : "" %>" id="ThemeInfo<%= theme.ThemeName %>">
                <fieldset>
                    <legend>
                        <h3>
                            Description</h3>
                    </legend>
                    <h4>
                        <%= theme.Description %>
                        <h4>
                            Author :
                            <%= theme.Author %>
                            |
                            <%= Html.ActionLink("Download", "Download", "ThemeSwitcher", new { ThemeName = theme.ThemeName }, null)%>
                            |
                            <%= theme.Downloaded %>
                            Downloads</h4>
                    </h4>
                </fieldset>
                <div>
                    <fieldset>
                        <legend>
                            <h3>
                                Customizations</h3>
                        </legend>
                        <div>
                            <label for="<%= "PageWidth" + theme.ThemeName %>">
                                Page Width</label><%=Html.DropDownList("PageWidth" + theme.ThemeName, theme.PageWidths)%>
                        </div>
                        <div>
                            <label for="<%= "PageTemplate" + theme.ThemeName %>">
                                Page Template</label><%=Html.DropDownList("PageTemplate" + theme.ThemeName, theme.PageTemplates)%>
                        </div>
                        <% if (theme.Customizations.Any())
                           { %>
                        <h3>
                            Additional Customizations</h3>
                        <%
                            foreach (var customization in theme.Customizations)
                            {
                                var customizationIdentifier = string.Format("customization_{0}_{1}", theme.ThemeName, customization.Name);%>
                        <div>
                            <label for="<%=customizationIdentifier%>" title="<%=customization.Description %>">
                                <%=customization.Name%></label><%=Html.DropDownList(customizationIdentifier, customization.Options)%>
                        </div>
                        <%
                            }
                           }%>
                    </fieldset>
                </div>
            </div>
            <%
                }%>
            <div>
                &nbsp;</div>
            <label>
                &nbsp;</label>
            <button type="submit" id="submit">
                Apply Theme</button>
        </div>
        <%
            }%>
    </div>
</div>
