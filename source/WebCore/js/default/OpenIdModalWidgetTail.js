$().ready(function() {
  //add the target overlay area
  $('body').append('<div id="openid_overlay" class="overlay"><div class="wrap"/></div>');
  //plugin the overlay to the trigger button
  $('#openid_login a').overlay({ expose: '#444'
    , onBeforeLoad: function() { var wrap = this.getContent().find('div.wrap'); if (wrap.is(':empty')) { wrap.load(this.getTrigger().attr('href'), function() { $('form.openid').openid() }); } }
  });
});