/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$().ready(function() {

  $('.settings.categories a[rel=remove]').live("click", function() {
    var $this = $(this);
    $.post($this.attr('href'), {}, function(data) {
      if (data.success) $this.closest('li').fadeOut(function() { $(this).remove(); }); else alert(data.error);
    });
    return false;
  });
  
  $('.settings.categories select').live('change', function() {
    $.get($(this).val(), function(data) { $('.settings.categories').replaceWith(data); });
  });

});
