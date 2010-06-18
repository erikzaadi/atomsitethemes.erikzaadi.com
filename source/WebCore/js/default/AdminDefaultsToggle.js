/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function() {
  $('li.notdefault img')
  .live("mouseover", function() { $(this).attr('src', $(this).attr('src').replace('W.png', 'Y.png')); })
  .live("mouseout", function() { $(this).attr('src', $(this).attr('src').replace('Y.png', 'W.png')); })
  $('li.notdefault a[rel=default]').live("click", function() {
    var $this = $(this);
    $.post($this.attr('href'), function(data) { $this.closest('.area').replaceWith(data); });
    return false;
  });
});