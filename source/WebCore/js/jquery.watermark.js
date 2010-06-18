$.fn.watermark = function(options) {
  var defaults = {
    color: '#999'
  };
  var opts = $.extend({}, defaults, options);
  return this.each(function() {
    var obj = $(this);
    obj.attr('baseColor', obj.css('color'));
    if (!obj.attr('title')) obj.attr('title', obj.val());
    if (obj.val() == obj.attr('title')) {
      obj.attr('isWatermark', 1);
      obj.css('color', opts.color);
    }
    obj.bind('focus', function() {
      var o = $(this);
      if (o.val() != o.attr('title') && o.val() != '') {
        o.attr("isWatermark", 0);
      }
      if (o.attr('isWatermark') == 1) {
        o.val('');
        o.attr('isWatermark', 0);
        o.css('color', o.attr('baseColor'));
      }
    }).bind('blur', function() {
      var o = $(this);
      if (o.val() != o.attr('title') && o.val() != '') {
        o.attr('isWatermark', 1);
      }
      if (o.attr('isWatermark') == 0) {
        o.css("color", opts.color);
        if (o.val() == '') {
          o.attr('isWatermark', 1);
          o.val(o.attr('title'));
        }
      }
    });
  });
};