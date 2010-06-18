/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function () {

    $('.selection li a').live("click", function () {
        $('.selection').expose({ api: true }).close();
        var userId = $(this).closest('li').attr('id');
        $.post($(this).attr('href'), { userId: userId }, function (data) { $('.selection').replaceWith(data); });
        return false;
    });

    $('.selection a[rel=cancel]').live("click", function () {
        $('.selection').expose({ api: true }).close();
        $.get($(this).attr('href'), function (data) { $('.selection').replaceWith(data); });
        return false;
    });

    $('.users a[rel=add]').live("click", function () {
        $.get($(this).attr('href'), function (data) {
          $('.users').replaceWith(data);
          $('.selection').expose({ api: true, closeOnClick: false, closeOnEsc: false }).load();
        });
        return false;
    });

    $('.users a[rel=remove]').live("click", function () {
        $.post($(this).attr('href'), function (data) {
            if (data.error != undefined) alert(data.error);
            else $('.users').replaceWith(data);
        });
        return false;
    });

    $('.users a[rel=delete-user]').live("click", function () {
      var $this = $(this);
      if (confirm('Deleting a user will not delete their posts. Are you sure you want to delete the user?')) {
          $.post($this.attr('href'), {}, function (data) {
              if (data.success) $this.closest('tr').fadeOut();
              else alert(data.error);
          }, "json");
      }
      return false;
  });
});