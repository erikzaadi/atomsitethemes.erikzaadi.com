<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<AdminThemeModel>" %>

<h3><%= Model.Theme.Title %><%= Model.Current ? " <em class='active'>Active</em>" : string.Empty %><%= Model.Inherited && !Model.Scope.IsEntireSite ? " <em class='inherited'>Inherited</em>" : string.Empty %></h3>
<table id="meta"><tr>
<td><strong>Version</strong><div><%= Model.Theme.Version%></div></td>
<td style="width:30%"><strong>Author</strong><div><% Html.RenderPartial("AtomPubPeople", Model.Theme.Authors); %></div></td>
<td><div class="stat">
          <label for="updated">Last Updated</label>
          <div id="updated" class="statVal">
              <%= Html.DateTimeAgoAbbreviation(Model.Theme.Updated)%>
          </div>
        </div></td>
<td><strong>Rank</strong><div><span style="font-size:larger; font-weight:bold"><%= Model.Theme.Rank %> </span> out of <%= Model.Theme.RankTotal %></div></td>
<td><label for="rating">Rating</label>
    <div class="statVal">
    <div class="rating">
			<span class="ui-rater-starsOff" style="width:90px;">
				<span class="ui-rater-starsOn" style="width:<%= Math.Round(18F * Model.Theme.Rating) %>px"></span>
			</span>
		</div></div></td>
</tr></table>

<div class="right widget">
<h3>Preview &amp; <%= Model.Installed ? "Apply" : "Download" %></h3>
 <div class="themePreview">
      <a href="<%= Model.Theme.Screenshot %>"><img src="<%= Model.Theme.Thumbnail %>" alt="Theme Screenshot" /></a><br />
      <a href="<%= Model.Theme.Screenshot %>">View Larger</a>
    </div><br />
 <div class="publish">
<% if (Model.Installed) { %>
<% if (Model.CanDelete) { %>
<button type="submit" name="submit" value="Delete Theme">Delete</button>
<%} %>
<button type="submit" name="submit" value="Apply Theme">Apply Theme</button>
<%} else { %>
<button type="submit" name="submit" value="Download Theme">Download</button>
<%} %></div>
</div>
    
    <div id="themeWidths"><strong>Widths</strong>
        <%= Model.GetSpecImage(Url, "doc") %>
        <%= Model.GetSpecImage(Url, "doc2")%>
        <%= Model.GetSpecImage(Url, "doc3")%>
        <%= Model.GetSpecImage(Url, "doc4")%>
        <%= Model.GetSpecImage(Url, "doc-custom")%></div>
      
    <div id="themeTemplates"><strong>Cols</strong>
        <%= Model.GetSpecImage(Url, "yui-t1") %>
        <%= Model.GetSpecImage(Url, "yui-t2")%>
        <%= Model.GetSpecImage(Url, "yui-t3")%>
        <%= Model.GetSpecImage(Url, "yui-t4")%>
        <%= Model.GetSpecImage(Url, "yui-t5")%>
        <%= Model.GetSpecImage(Url, "yui-t6")%></div>
     
     <dl class="info">
  <dt>Target Media</dt><dd><%= Model.Theme.TargetMedia %></dd>
  <dt>Layout Type</dt><dd><%= Model.Theme.Layout%></dd>
  <dt>Compatibility</dt><dd><%= string.Join(", ", Model.Theme.CompatibleVersions.ToArray()) %></dd>
  </dl>
  <div class="content"><%= Model.Theme.Text.Text%></div>
  <br />
  <p>
  <a class="button" href="<%= Model.Theme.Homepage %>">Visit Homepage</a>
  </p>
 