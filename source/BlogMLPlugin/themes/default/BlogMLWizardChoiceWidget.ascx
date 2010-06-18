<%@ Control Language="C#" Inherits="ViewUserControl<PageModel>" %>

  <div class="taskOption">
    <form action="<%= Url.Action("WizardImport", "BlogML") %>" method="get"><fieldset>
      <input type="submit" name="blogml" value="Import from BlogML" />
      Import from a <a href="http://en.wikipedia.org/wiki/BlogML">BlogML</a> file containing your existing blog posts.
    </fieldset></form>
  </div>
  
<%--
    <div id="BlogMLWizard">
<h1>Import from <a href="http://en.wikipedia.org/wiki/BlogML">BlogML</a></h1>
	 
  <% using (Html.BeginForm("WizardImport", "BlogML", FormMethod.Post, new { enctype = "multipart/form-data" })) { %> 
	 <fieldset>
	 <div class="field">
	 <label for="collectionName">Address</label>
	 <strong><%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath%>/</strong><input class="required" style="width:10em" type="text" name="collectionName" value="blog" />
	 </div>
	 
	 <div class="field">
	 <small>Enter your domain or email to uniquely identify your posts <a style="vertical-align:super; font-weight:bold;" href="http://atomsite.net/info/URIsAndURLs.xhtml">?</a></small>
	 <label for="txtBaseId">Id Name</label>
	 <input class="required" style="width:12em;" type="text" name="txtBaseId" value="<%= Model.Owner %>" />
	 </div>
	 
	 <div class="field">
	 <small>Enter the first year you obtained this domain or email</small>
	 <label for="ddlYear">Id Year</label>
	 <%= Html.DropDownList("ddlYear", new SelectList(Enumerable.Range(1990, (DateTime.Today.Year + 2) - 1990)))%>
	 </div>
	 
  <div class="taskOption">
    <label for="blogml">BlogML File</label>
    <input type="file" name="blogml" />
  </div>
  
  <button type="submit" name="import">Import</button>
   
	 </fieldset>	 
<% } %>

</div>--%>