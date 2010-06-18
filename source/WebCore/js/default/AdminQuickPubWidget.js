/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function() {
  $('#quickpub .notifications').hide();
  $('#quickpub button').click(function() {
    var $form = $('#quickpub form');
    $.post($form.attr('action'), $form.serialize() + "&submit=" + $(this).val(),
       function(data) {
         if (!data.error) {
           $('#quickpub .notifications').show().find('.message').html(data.message);
           $('#quickpub form').each(function() { this.reset(); });
         } else {
           alert(data.error);
         }
       }, "json");
    return false;
  });
});