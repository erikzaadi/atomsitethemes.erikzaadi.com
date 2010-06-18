/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */

$(document).ready(function() {
  $('#editpublished').click(function() {
    $('strong.published').hide();
    $(this).hide();
    $('#publishedinput').show();
    return false;
  });
  $('.entry input[name=title]').watermark();
  $('.categories input[name=categoryName]').watermark();
  $('.categories button[name=addCategory]').click(function() {
    var cat = $.trim($('input[name=categoryName]').val());
    if (cat.length < 1) return false;
    var add = true;
    $('.categories li label').each(function() {
      if ($(this).text().toUpperCase().indexOf(cat.toUpperCase()) == 0) {
        add = false;
        $(this).prev('input[type=checkbox]').attr('checked', 'checked');
        scrollIntoView($(this), $(this).closest('ul'));
        flashit($(this).closest('li'), 'greenbg', 600, 3);
      }
    });
    if (add) {
      $('.categories ul').append("<li><input type='checkbox' name='categories' value='" + cat + "' checked='checked' /><label>" + cat + "</label></li>");
      scrollIntoView($('.categories ul li:last'), $('.categories ul'));
      flashit($('.categories ul li:last'), 'greenbg', 900, 2);
    }
    $('.categories input[name=categoryName]').val('');
    return false;
  });

  $('.categories input[name=categoryName]').live('keypress', function(e) {
    if ($(this).next('button').length <= 0) return true;
    if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
      $(this).next('button').click();
      return false;
    } else {
      return true;
    }
  });


  $('a[rel=editid], a[rel=addlink]')
  .live("click", function() {
    alert('Not yet implemented');
    return false;
  });
}); 