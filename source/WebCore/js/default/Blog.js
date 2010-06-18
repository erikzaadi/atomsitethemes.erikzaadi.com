/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
function del(id, url) {
  id = getSelectorId(id);
  $(id).fadeTo(500, 0.4, function() {
    if (confirm('Are you sure you want to delete this entry?')) {
      $.ajax({
        type: "DELETE",
        data: "",
        url: url,
        complete: function(req) {
          if (req.status == 200) {
            $(id).fadeOut(function() { $(this).remove(); });
          } else {
            alert('Failed to delete entry: ' + req.responseText);
            $(id).fadeTo(1000, 1);
          }
        }
      });
    } else {
      $(id).fadeTo(500, 1);
    }
  });
}

function approve(id, url, link) {
  id = getSelectorId(id);
  var approved = $(link).text().toUpperCase() == 'APPROVE';

  $(id).fadeTo(800, 0.3, function() {
    $.ajax({
      type: 'POST',
      data: "approved=" + approved,
      url: url,
      complete: function(req) {
        if (req.status == 200) {
          if (!approved) {
            $(id).removeClass('commentApprove').addClass('commentNotApproved');
            $(link).text('Approve');
          } else {
            $(id).removeClass('commentNotApproved').addClass('commentApprove');
            $(link).text('Unapprove');
          }
          $(id).find('em.warning').remove();
        } else {
          alert('Failed to set approval: ' + req.responseText);
        }
        $(id).fadeTo(2000, 1);
      }
    });
  });
}

function approveAll(id, url, link) {
  $.ajax({
    type: 'POST',
    data: "",
    url: url,
    complete: function(req) {
      if (req.status == 200) {
        alert('Approved: ' + req.responseText);
        $(link).remove();
        $('#comments div.commentNotApproved').fadeTo(800, 0.3, function() {
          $('#comments button:contains("Approve")').remove();
          $('#comments em.warning').remove();
          $('#comments div.commentNotApproved')
								.toggleClass('commentNotApproved').addClass('commentApprove').fadeTo(2000, 1);
        });
      } else {
        alert('Failed to approve: ' + req.responseText);
      }
    }
  });
}

function getSelectorId(id) {
  return "#" + id.toString().replace(/:/g, '\\:').replace(/\./g, '\\.').replace(/,/g, '\\,');
}