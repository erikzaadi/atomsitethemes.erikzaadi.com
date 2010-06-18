$(document).ready(function () {
  $('#contact form').live('submit', function () {
    $.post($(this).attr('action'), $(this).serialize(), function (data) {
      $("#contact").replaceWith($(data));
    });
    return false;
  });
});