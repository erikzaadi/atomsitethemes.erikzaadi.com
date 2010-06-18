/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$().ready(function() {
  $('.accepts li input[type=checkbox]').click(acceptClick);
  $('.accepts select').change(templateChanged);
});

var templateChanged = function() {
  toggleAccepts($(this).val().split(','), function() {
    scrollIntoView($('.accepts input[type=checkbox]:checked'), $('.accepts ul'));
  });
  return false;
};

var acceptClick = function () {
  var accepts = [];
  $('.accepts ul').find('input[type=checkbox]:checked').each(function (i, val) { accepts[i] = $(val).val() });
  toggleAccepts(accepts);
  return false;
};

function toggleAccepts(accepts, func) {
  $.post($('.accepts form').attr('action'), { accepts: arrayToCsv(accepts) },
      function(data) {
        if (data.error == undefined) {
          var accepts = data.accepts.split(',');
          updateAccepts(accepts);
          if (func != undefined) func();
        } else { alert(data.error); }
      }, "json");
  return false;
}

function updateAccepts(accepts) {
  $('.accepts li').each(function() {
    var $li = $(this).removeClass('checked');
    var $cb = $li.find('input[type=checkbox]').removeAttr('checked');
    for (var i = 0; i < accepts.length; i++) {
      if (accepts[i] == $cb.val()) {
        $cb.attr('checked', 'checked');
        $li.addClass('checked');
        break;
      }
    }
  });
  var csv = arrayToCsv(accepts);
  $('select[name=template]').val(csv);
}