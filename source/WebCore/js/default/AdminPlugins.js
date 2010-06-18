/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function () {

  $('input[name=upplug]').MultiFile({ max: 1, accept: 'zip|dll', afterFileAppend: function (element, value, master_element) {
    $('.MultiFile-label').append('<button type="submit">Install Plugin</button>');
  }
  });

  var plugins = function () {

    var coreConfirm = function (form) {
      var type = $(form).find('input[name=pluginType]').val();
      var warn = type.indexOf('AtomSite.WebCore') == 0;
      var proceed = true;
      if (warn) proceed = confirm('Warning: you are about to disable a core plugin! There is no guarantee that AtomSite will function properly. Continue?');
      return proceed;
    };

    var changeVal;

    $('button[name=change]').click(function () {
      changeVal = $(this).val();
    });

    $('td.ctrl form, td.merit form').submit(function () {
      var confirmed = true;
      if (changeVal == 'disable') confirmed = coreConfirm(this);
      if (confirmed) {
        $(this).find('button').hide();
        $(this).append('<strong>Processing</strong>, please wait</small>');
        $.post($(this).attr('action'), $(this).serialize() + "&change=" + changeVal, function (data) { redisplayPlugins(data) });
      }
      return false;
    });

    $('button[name=change]').removeAttr('disabled');
    $('button.disabled').attr('disabled', 'disabled');
    flashit($('tr.greenbg'), 800, 1);
  };

  var redisplayPlugins = function (data) {
    if (!data.error) {
      $('.plugins').remove();
      $('#target').html(data);
      plugins();
      flashit($('#restart'), 800, 2);
    } else {
      alert(data.error);
    }
  };

  plugins();

  $('#restart form').submit(function () {
    if (confirm('If you restart, your user sessions may be lost.  Continue with application restart?')) {
      $.post($(this).attr('action'), function (data) {
        alert('The application is restarting. Your pages may load slow until the application is fully loaded.');
      }, "json");
    }
    return false;
  });
});