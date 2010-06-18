/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(function() {
  $('#addcommentform').submit(function() {
    $('.error').hide();
    var entry = "<entry xmlns='http://www.w3.org/2005/Atom'><title>Comment</title>";
    if ($('#authenticated').length == 0) {
      entry += "<author>";
      if ($.trim($('#txtName').val()).length == 0) {
        $("#nameError").fadeIn("slow"); return false;
      } else {
        entry += "<name>" + parse($('#txtName').val()) + "</name>";
      }
      if ($.trim($('#txtEmail').val()).length == 0 ||
		    $('#txtEmail').val().search(emailRegxp) == -1) {
        $("#emailError").fadeIn("slow"); return false;
      } else {
        entry += "<email>" + encodeURI($('#txtEmail').val()) + "</email>";
      }
      if ($.trim($('#txtWebsite').val()) != 'http://' &&
            $.trim($('#txtWebsite').val()).length > 0) {
        if ($('#txtWebsite').val().search(urlRegxp) == -1) {
          $("#websiteError").fadeIn("slow"); return false;
        } else {
          entry += "<uri>" + encodeURI($('#txtWebsite').val()) + "</uri>";
        }
      }
      entry += "</author>";
    }
    if ($.trim($('#txtComment').val()).length == 0) {
      $("#commentError").fadeIn("slow"); return false;
    } else {
      entry += "<content type='html'>" + parse($('#txtComment').val()) + "</content></entry>";
    }
    $.ajax({
      type: "POST",
      url: $('#addcommentform').attr('action'),
      data: entry,
      contentType: "application/atom+xml;type=entry",
      dataType: "xml",
      complete: function(req) {
        if (req.status == 200) {
          reset();
          var $a = $(req.responseText);
          $a.addClass("commentSelf").fadeTo(0, 0, function() {
            $a.appendTo("#comments").fadeTo(1000, 1); $('abbr.timeago').timeago();
            $('#commentsEmpty').remove();
          });
        } else {
          alert('Failed to post comment: ' + req.statusText + "\n" + req.responseText);
        }
      }
    });
    return false;
  });
});
function parse(s) {
    if (s)
        return s.replace(/\n/, "<br />").replace(/&/g, "&amp;").replace(/\"/g, "&quot;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    return ""
};
function reset() {
    $('.commentError').hide();
    $('#txtComment')[0].value = "";
};
var emailRegxp = /^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i;
var urlRegxp = /^(?:https?|s?ftp|telnet|ssh|scp):\/\/(?:(?:[\w]+:)?\w+@)?(?:(?:(?:[\w-]+\.)*\w[\w-]{0,66}\.(?:[a-z]{2,6})(?:\.[a-z]{2})?)|(?:(?:25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.)(?:(?:25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(?:25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})))(?:\:\d{1,5})?(?:\/(~[\w-_.])?)?(?:(?:\/[\w-_.]*)*)?\??(?:(?:[\w-_.]+\=[\w-_.]+&?)*)?$/i; 
