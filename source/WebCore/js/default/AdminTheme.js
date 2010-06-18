/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function() {

  $('.choose li').click(function() {
    var $this = $(this);
    document.location = "#" + $this.attr('id');
    $.get($this.find('a').attr('href'), { theme: $this.attr('id') },
       function(data) {
         $("#target").html(data); $('abbr.timeago').timeago();
         $('.choose li.selected').removeClass('selected');
         $this.addClass('selected');
         $('#themeForm input[name=theme]').val($this.attr('id'));
       });
    return false;
  });
  $('.choose a.tab').click(function() {
    alert('Not yet implemented.');
  });

  //auto load based on hash
  var selTheme = document.location.hash;
  if (selTheme.length > 1) {
    if ('#' + $('#theme').val() != selTheme) {
      $(selTheme).click();
    }
  }
});