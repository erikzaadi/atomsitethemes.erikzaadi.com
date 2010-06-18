/* Copyright 2008-10 Jarrett Vance
* see http://www.codeplex.com/blogsvc/license */
$(document).ready(function () {

  $('.widgets.selection li a').live("click", function () {
    $.post($(this).attr('href'), function (data) {
      $('.selection').expose({ api: true }).close();
      var $new = $(data);
      if ($new.find('.area').length > 2) {
        $('div.area:eq(0)').replaceWith($new.find('.targets'));
        $('div.area:eq(1)').replaceWith($new.find('.areas'));
        $('div.area:eq(2)').replaceWith($new.find('.includes'));
        $('.selected').each(function () { scrollIntoView($(this), $(this).parent()) });
        wireConfig();
      } else {
        $('.selection').replaceWith($new).remove();
        $new.find('.error').delay(800).slideUp('slow');
        $new.expose({ api: true, closeOnClick: false, closeOnEsc: false }).load();
      }
    });
    return false;
  });

  $('.widgets.selection a[rel=cancel]').live("click", function () {
    $('.selection').expose({ api: true }).close();
    $.get($(this).attr('href'), function (data) {
      $('.selection').replaceWith(data);
      wireConfig();
    });
    return false;
  });

  $('.widgets a[rel=add]').live("click", function () {
    var $this = $(this);
    $.get($this.attr('href'), function (data) {
      var $new = $(data);
      $this.closest('.widgets').replaceWith($new);
      $('.selection').expose({ api: true, closeOnClick: false, closeOnEsc: false }).load();
    });
    return false;
  });

  $('.widgets a[rel=remove]').live("click", function () {
    if (confirm("Removing pages, areas, or includes can alter the appearance or functionality of your website.  Are you sure you want to remove it?")) {
      var $this = $(this);
      $.post($(this).attr('href'), function (data) {
        var $new = $(data);
        if ($new.find('.area').length > 2) {
          $this.closest('li').fadeTo(400, 0.3, function () {
            $('div.area:eq(0)').replaceWith($new.find('.targets'));
            $('div.area:eq(1)').replaceWith($new.find('.areas'));
            $('div.area:eq(2)').replaceWith($new.find('.includes'));
            $('.selected').each(function () { scrollIntoView($(this), $(this).parent()) });
            wireConfig();
          });
        } else alert('Removal failed. Please contact administrator.');
      });
    }
    return false;
  });

  $('.widgets.includes a[rel=up], .widgets.includes a[rel=down]').live("click", function () {
    var $this = $(this);
    $.post($this.attr('href'), { down: $this.attr('rel') == "down" }, function (data) {
      if (data.error != undefined) alert(data.error);
      else {
        $this.closest('li').fadeTo(400, 0.3, function () {
          var $list = $(this).closest('.area').replaceWith($(data));
          wireConfig();
        });
      }
    });
    return false;
  });  
});

function wireConfig() {
  $('a[rel=config]').overlay({ target: '#configure', expose: '#444', closeOnClick: false, close: 'button[name=close]', onBeforeLoad: function () {
    var ovr = this;
    var wrap = ovr.getContent().find('div.wrap');
    wrap.load(this.getTrigger().attr('href'), function () { configureForm(); $(this).find('button[name=close]').live('click', function () { ovr.close(); }); });
  }
  });
}

function configureForm() {
  $('#configureForm').live('submit', function () {
    var $this = $(this);
    $.post($this.attr('action'), $this.serialize(), function (data) { onConfigResp(data); });
    return false;
  });
  // following line is a workaround for IE
  $('#configureForm *[type=submit]').live('click', function () { $('#configureForm').trigger('submit'); return false; });
}

function onConfigResp(data) {
  if (data.success != undefined) {
    $('#configure').find('button[name=close]').trigger('click');
    $('#' + data.includePath).find('.ico[title=invalid]').remove();
    $('#' + data.includePath).find('.ico[title=configurable]').after('<span class="ico" title="saved">saved</span>');
    $('#' + data.includePath).toggleClass('selected').css('opacity', 0.3).delay(200).fadeTo(1200, 1, function () {
      $(this).toggleClass('selected');
      $(this).find('.ico[title=saved]').delay(2000).fadeOut();
    });
  } else {
    $('#configure').find('.wrap').empty().append(data);
  }
}