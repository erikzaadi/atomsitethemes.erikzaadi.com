<%@ Page Language="C#" MasterPageFile="Wizard.Master" AutoEventWireup="true" Inherits="ViewPage<PageModel>" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
  <title>Welcome - Wizard</title>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="content" runat="server">
  <h1>Welcome</h1>
  
  <p>This is the first time you have run AtomSite.  Please wait while we test your installation.</p>
  <table id="tblTests">
    <tr>
      <td>GET</td><td><img id="imgGET" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr>
    <tr>
      <td>POST</td><td><img id="imgPOST" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr>
    <tr>
      <td>PUT</td><td><img id="imgPUT" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr>
    <tr>
      <td>DELETE</td><td><img id="imgDELETE" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr>
    <tr>
      <td>Write Access</td><td><img id="imgWriteFile" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr>
    <tr>
      <td>Delete Access</td><td><img id="imgDeleteFile" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr><%--
    <tr>
      <td>Secure Access</td><td><img id="imgSecure" src="<%= Url.ImageSrc("pending.png") %>" alt="indicator" /></td>
    </tr>--%>
  </table>
  <div class="status">Testing...</div>
  
  <div id="divContinue" class="taskOption" style="display:none;">
    <form action="<%= Url.Action("SetupChoice") %>" method="get">
      <fieldset>
        <input type="submit" name="continue" value="Continue" /> 
        <strong>Congratulations</strong>, your AtomSite installation is fully functional.  Choose continue to setup your new installation.
      </fieldset>
    </form>  
  </div>
  
  <div id="divTrouble" class="taskOption" style="display:none;">
    <input type="button" name="retest" value="Retest" /> 
    <strong>Sorry</strong>, your AtomSite installation may not function correctly.  Your web site provider may be blocking features you need to run AtomSite. <strong>See <a href="http://atomsite.net/info/Troubleshooting.xhtml">Troubleshooting</a> for help.</strong>
  </div>
  
  <div id="divContinueAnyway" class="taskOption">
    <form action="<%= Url.Action("SetupChoice") %>" method="get">
      <fieldset>
        <input type="submit" name="continue" value="Continue Anyway" /> 
        If you want to ignore any error(s) and try to continue setup, click continue anyway.
      </fieldset>
    </form>
  </div>  
</asp:Content>

<asp:Content ID="Tail" ContentPlaceHolderID="tail" runat="server">
    <script type="text/javascript">

      $(document).ready(function() {
        $('divContinueAnyway').hide();
        $('input[name=retest]').click(function() { runTest(0); });
        runTest(0);
      });

    var tests = ['GET', 'POST', 'PUT', 'DELETE', 'WriteFile', 'DeleteFile'];
    var testsUrl = ['<%= Url.Action("Test") %>', '<%= Url.Action("Test") %>',
     '<%= Url.Action("Test") %>', '<%= Url.Action("Test") %>',
     '<%= Url.Action("WriteTest") %>', '<%= Url.Action("DeleteTest") %>'];
    var testResults = [0, 0, 0, 0, 0, 0];
    var timeout;
    function runTest(idx) {
    	if (timeout) { clearTimeout(timeout); }
    	
    	if (idx == 0) {
    		$('#divTrouble').fadeOut();
    		$('#divContinueAnyway').fadeOut();
    	}
    	
    	if (idx > tests.length) {
    	
    	  var sum = 0;
				for (i=0; i<testResults.length; i++) {
					sum = sum + testResults[i];
				}

				if (testResults.length == sum) {
					$('.status').text("Complete").removeClass('failure').addClass('complete');
					$('#divContinue').fadeIn();
				} else {
					$('.status').text("Error").removeClass('complete').addClass('failure');
					$('#divTrouble').fadeIn();
					$('#divContinueAnyway').fadeIn();
				}
    			return;
    	}
    	
    	$('.status').text("Testing " + idx + "...");
    	$('#img' + tests[idx]).replaceWith('<img id="img' + tests[idx] + '" src="<%= Url.ImageSrc("busy.gif") %>" alt="indicator" />');

    	$.ajax({
    		type: idx < 4 ? tests[idx] : "GET",
    		url: testsUrl[idx],
    		data: "",
    		timeout: 4000,
    		success: function(resp) {
    		$('#img' + tests[idx]).replaceWith('<img id="img' + tests[idx] + '" src="<%= Url.ImageSrc("success.png") %>" alt="indicator" />');
    				testResults[idx] = 1;
    				runTest(idx + 1);
    		},
    		error: function(req, errorType, errorThrown) {
    		$('#img' + tests[idx]).replaceWith('<img id="img' + tests[idx] + '" src="<%= Url.ImageSrc("failure.png") %>" alt="indicator" />');
    				testResults[idx] = 0;
    				runTest(idx + 1);
    		}
    	});
    	}
 </script>
</asp:Content>
