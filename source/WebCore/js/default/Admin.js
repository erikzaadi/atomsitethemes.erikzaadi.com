/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function () {
  
  $('.ddmenu').singleDropMenu({ parentMO: 'ddmenu-hover', childMO: 'ddchildhover' });
  
  $('abbr.timeago').timeago();

  $("ul.tabs").tabs("div.panes > div");

  $('li, tr').live('mouseover', function() { $(this).find('.actions').css('visibility', 'visible') });
  $('li, tr').live('mouseout', function() { $(this).find('.actions').css('visibility', 'hidden') });

  if ($('#errors p').length != 0) { $('#errors').show(); }
  if ($('#notifications p').length != 0) { $('#notifications').show(); }

  $('a[rel=approve], a[rel=unapprove]').live('click', function() { approve($(this)); return false; });
  $('a[rel=delete]').live('click', function() { del($(this)); return false; });
});

function showNotification(name, message) {
  $('#notifications>div').append('<p><span class="icon-info"></span> <strong>' + name + '</strong> ' + message + '</p>');
  $('#notifications').show();
}
function showError(error) {
  $('#errors>div').append('<p><span class="icon-alert"></span> <strong>Error</strong> ' + error + '</p>');
  $('#errors').show();
}

function approve($a) {
  var approved = $a.text().toUpperCase() == 'APPROVE';
  var $item = $a.closest('*[id]');
  $item.fadeTo(800, 0.3, function() {
    $.ajax({
      type: 'POST',
      data: "approved=" + approved,
      url: $a.attr('href'),
      complete: function(req) {
        if (req.status == 200) {
          if (!approved) {
            $a.text('approve');
            $item.find('.status').text('[Not Approved]');
            $item.addClass('pending');
          } else {
            $a.text('unapprove');
            $item.find('.status').text('');
            $item.removeClass('pending');
          }
        } else {
          alert('Failed to set approval: ' + req.responseText);
        }
        $item.fadeTo(2000, 1);
      }
    });
  });
}

function del($a) {
  var $item = $a.closest('*[id]');
  $item.fadeTo(500, 0.4, function() {
    if (confirm('Are you sure you want to delete this entry?')) {
      $.ajax({
        type: "DELETE",
        data: "",
        url: $a.attr('href'),
        complete: function(req) {
          if (req.status == 200) {
            $item.fadeOut(function() { $(this).remove(); });
          } else {
            alert('Failed to delete entry: ' + req.responseText);
            $item.fadeTo(1000, 1);
          }
        }
      });
    } else {
      $item.fadeTo(500, 1);
    }
  });
}

var flashit = function($this, duration, cycles) {
  if (cycles < 1) return;
  $this.fadeTo(duration / 2, 0.2, function() {
    $this.fadeTo(duration / 2, 1, function() { 
      flashit($this, duration, cycles - 1); }); });
};

var scrollIntoView = function($this, $parent) {
  var divOffset = $parent.offset().top;
  var pOffset = $this.offset().top;
  var pScroll = pOffset - divOffset;
  $parent.scrollTop(pScroll);
};

function arrayToCsv(arr) {
  var csv = "";
  for (var i = 0; i < arr.length; i++) {
    csv += arr[i]; if (i == arr.length - 1) break; csv += ',';
  }
  return csv;
}