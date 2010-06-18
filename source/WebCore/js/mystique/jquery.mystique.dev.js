

if (isIE == 'undefined') var isIE = false;
if (isIE6 == 'undefined') var isIE6 = false;
if (lightbox == 'undefined') var lightbox = 0;
if (ajaxComments == 'undefined') var ajaxComments = 0;
if (redirectReadMore == 'undefined') var redirectReadMore = 0;

/* easing */
// t: current time, b: begInnIng value, c: change In value, d: duration
jQuery.easing['jswing'] = jQuery.easing['swing'];
jQuery.extend(jQuery.easing, {
  def: 'easeOutQuad',
  swing: function (x, t, b, c, d) { //alert(jQuery.easing.default);
    return jQuery.easing[jQuery.easing.def](x, t, b, c, d);
  },
  easeInQuad: function (x, t, b, c, d) {
    return c * (t /= d) * t + b;
  },
  easeOutQuad: function (x, t, b, c, d) {
    return -c * (t /= d) * (t - 2) + b;
  },
  easeInOutQuad: function (x, t, b, c, d) {
    if ((t /= d / 2) < 1) return c / 2 * t * t + b;
    return -c / 2 * ((--t) * (t - 2) - 1) + b;
  },
  easeInCubic: function (x, t, b, c, d) {
    return c * (t /= d) * t * t + b;
  },
  easeOutCubic: function (x, t, b, c, d) {
    return c * ((t = t / d - 1) * t * t + 1) + b;
  },
  easeInOutCubic: function (x, t, b, c, d) {
    if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
    return c / 2 * ((t -= 2) * t * t + 2) + b;
  },
  easeInQuart: function (x, t, b, c, d) {
    return c * (t /= d) * t * t * t + b;
  },
  easeOutQuart: function (x, t, b, c, d) {
    return -c * ((t = t / d - 1) * t * t * t - 1) + b;
  },
  easeInOutQuart: function (x, t, b, c, d) {
    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
    return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
  },
  easeInQuint: function (x, t, b, c, d) {
    return c * (t /= d) * t * t * t * t + b;
  },
  easeOutQuint: function (x, t, b, c, d) {
    return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
  },
  easeInOutQuint: function (x, t, b, c, d) {
    if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
    return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
  },
  easeInSine: function (x, t, b, c, d) {
    return -c * Math.cos(t / d * (Math.PI / 2)) + c + b;
  },
  easeOutSine: function (x, t, b, c, d) {
    return c * Math.sin(t / d * (Math.PI / 2)) + b;
  },
  easeInOutSine: function (x, t, b, c, d) {
    return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b;
  },
  easeInExpo: function (x, t, b, c, d) {
    return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b;
  },
  easeOutExpo: function (x, t, b, c, d) {
    return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;
  },
  easeInOutExpo: function (x, t, b, c, d) {
    if (t == 0) return b;
    if (t == d) return b + c;
    if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
    return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
  },
  easeInCirc: function (x, t, b, c, d) {
    return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b;
  },
  easeOutCirc: function (x, t, b, c, d) {
    return c * Math.sqrt(1 - (t = t / d - 1) * t) + b;
  },
  easeInOutCirc: function (x, t, b, c, d) {
    if ((t /= d / 2) < 1) return -c / 2 * (Math.sqrt(1 - t * t) - 1) + b;
    return c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b;
  },
  easeInElastic: function (x, t, b, c, d) {
    var s = 1.70158;
    var p = 0;
    var a = c;
    if (t == 0) return b;
    if ((t /= d) == 1) return b + c;
    if (!p) p = d * .3;
    if (a < Math.abs(c)) {
      a = c;
      var s = p / 4;
    } else var s = p / (2 * Math.PI) * Math.asin(c / a);
    return - (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
  },
  easeOutElastic: function (x, t, b, c, d) {
    var s = 1.70158;
    var p = 0;
    var a = c;
    if (t == 0) return b;
    if ((t /= d) == 1) return b + c;
    if (!p) p = d * .3;
    if (a < Math.abs(c)) {
      a = c;
      var s = p / 4;
    } else var s = p / (2 * Math.PI) * Math.asin(c / a);
    return a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b;
  },
  easeInOutElastic: function (x, t, b, c, d) {
    var s = 1.70158;
    var p = 0;
    var a = c;
    if (t == 0) return b;
    if ((t /= d / 2) == 2) return b + c;
    if (!p) p = d * (.3 * 1.5);
    if (a < Math.abs(c)) {
      a = c;
      var s = p / 4;
    } else var s = p / (2 * Math.PI) * Math.asin(c / a);
    if (t < 1) return -.5 * (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
    return a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
  },
  easeInBack: function (x, t, b, c, d, s) {
    if (s == undefined) s = 1.70158;
    return c * (t /= d) * t * ((s + 1) * t - s) + b;
  },
  easeOutBack: function (x, t, b, c, d, s) {
    if (s == undefined) s = 1.70158;
    return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
  },
  easeInOutBack: function (x, t, b, c, d, s) {
    if (s == undefined) s = 1.70158;
    if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
    return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
  },
  easeInBounce: function (x, t, b, c, d) {
    return c - jQuery.easing.easeOutBounce(x, d - t, 0, c, d) + b;
  },
  easeOutBounce: function (x, t, b, c, d) {
    if ((t /= d) < (1 / 2.75)) {
      return c * (7.5625 * t * t) + b;
    } else if (t < (2 / 2.75)) {
      return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
    } else if (t < (2.5 / 2.75)) {
      return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
    } else {
      return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
    }
  },
  easeInOutBounce: function (x, t, b, c, d) {
    if (t < d / 2) return jQuery.easing.easeInBounce(x, t * 2, 0, c, d) * .5 + b;
    return jQuery.easing.easeOutBounce(x, t * 2 - d, 0, c, d) * .5 + c * .5 + b;
  }
});

/*
  	loopedSlider 0.5.4 - jQuery plugin
 	written by Nathan Searles
 	http://nathansearles.com/loopedslider/

 	Copyright (c) 2009 Nathan Searles (http://nathansearles.com/)
 	Dual licensed under the MIT (MIT-LICENSE.txt)
 	and GPL (GPL-LICENSE.txt) licenses.

 	Built for jQuery library
 	http://jquery.com

    - MODIFIED FOR MYSTIQUE! BE CAREFUL WHEN UPDATING! */

(function (jQuery) {
  jQuery.fn.loopedSlider = function (options) {
    var defaults = {
      container: '.slide-container',
      slides: '.slides',
      pagination: '.pagination',
      containerClick: false,
      // Click container for next slide
      autoStart: 0,
      // Set to positive number for auto start and interval time
      restart: 0,
      // Set to positive number for restart and restart time
      slidespeed: 333,
      // Speed of slide animation
      fadespeed: 133,
      // Speed of fade animation
      autoHeight: true,
      // Set to positive number for auto height and animation speed
      easing: 'easeOutQuart'
    };
    this.each(function () {
      var obj = jQuery(this);
      var o = jQuery.extend(defaults, options);
      var pagination = jQuery(o.pagination + ' li a', obj);
      var m = 0;
      var t = 1;
      var s = jQuery(o.slides, obj).find('li.slide').size();
      var w = jQuery(o.slides, obj).find('li.slide').outerWidth();
      var p = 0;
      var u = false;
      var n = 0;
      var interval = 0;
      var restart = 0;
      jQuery(o.slides, obj).css({
        width: (s * w)
      });
      jQuery(o.slides, obj).find('li.slide').each(function () {
        jQuery(this).css({
          position: 'absolute',
          left: p,
          display: 'block'
        });
        p = p + w;
      });
      jQuery(pagination, obj).each(function () {
        n = n + 1;
        jQuery(this).attr('rel', n);
        jQuery(pagination.eq(0), obj).parent().addClass('active');
      });
      jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
        position: 'absolute',
        left: -w
      });
      if (s > 3) {
        jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
          position: 'absolute',
          left: -w
        });
      }
      if (o.autoHeight) {
        autoHeight(t);
      }
      jQuery('.next', obj).click(function () {
        if (u === false) {
          animate('next', true);
          if (o.autoStart) {
            if (o.restart) {
              autoStart();
            } else {
              clearInterval(sliderIntervalID);
            }
          }
        }
        return false;
      });
      jQuery('.previous', obj).click(function () {
        if (u === false) {
          animate('prev', true);
          if (o.autoStart) {
            if (o.restart) {
              autoStart();
            } else {
              clearInterval(sliderIntervalID);
            }
          }
        }
        return false;
      });
      if (o.containerClick) {
        jQuery(o.container, obj).click(function () {
          if (u === false) {
            animate('next', true);
            if (o.autoStart) {
              if (o.restart) {
                autoStart();
              } else {
                clearInterval(sliderIntervalID);
              }
            }
          }
          return false;
        });
      }
      jQuery(pagination, obj).click(function () {
        if (jQuery(this).parent().hasClass('active')) {
          return false;
        } else {
          t = jQuery(this).attr('rel');
          jQuery(pagination, obj).parent().siblings().removeClass('active');
          jQuery(this).parent().addClass('active');
          animate('fade', t);
          if (o.autoStart) {
            if (o.restart) {
              autoStart();
            } else {
              clearInterval(sliderIntervalID);
            }
          }
        }
        return false;
      });
      if (o.autoStart) {
        sliderIntervalID = setInterval(function () {
          if (u === false) {
            animate('next', true);
          }
        },
        o.autoStart);

        function autoStart() {
          if (o.restart) {
            clearInterval(sliderIntervalID);
            clearInterval(interval);
            clearTimeout(restart);
            restart = setTimeout(function () {
              interval = setInterval(function () {
                animate('next', true);
              },
              o.autoStart);
            },
            o.restart);
          } else {
            sliderIntervalID = setInterval(function () {
              if (u === false) {
                animate('next', true);
              }
            },
            o.autoStart);
          }
        };
      }

      function current(t) {
        if (t === s + 1) {
          t = 1;
        }
        if (t === 0) {
          t = s;
        }
        jQuery(pagination, obj).parent().siblings().removeClass('active');
        jQuery(pagination + '[rel="' + (t) + '"]', obj).parent().addClass('active');
      };

      function autoHeight(t) {
        if (t === s + 1) {
          t = 1;
        }
        if (t === 0) {
          t = s;
        }
        var getHeight = jQuery(o.slides, obj).find('li.slide:eq(' + (t - 1) + ')', obj).outerHeight();
        jQuery(o.container, obj).animate({
          height: getHeight
        },
        o.autoHeight, o.easing);
      };

      function animate(dir, clicked) {
        u = true;
        switch (dir) {
        case 'next':
          t = t + 1;
          m = (-(t * w - w));
          current(t);
          if (o.autoHeight) {
            autoHeight(t);
          }
          if (s < 3) {
            if (t === 3) {
              jQuery(o.slides, obj).find('li.slide:eq(0)').css({
                left: (s * w)
              });
            }
            if (t === 2) {
              jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
                position: 'absolute',
                left: (w)
              });
            }
          }
          jQuery(o.slides, obj).animate({
            left: m
          },
          o.slidespeed, o.easing, function () {
            if (t === s + 1) {
              t = 1;
              jQuery(o.slides, obj).css({
                left: 0
              },
              function () {
                jQuery(o.slides, obj).animate({
                  left: m
                })
              });
              jQuery(o.slides, obj).find('li.slide:eq(0)').css({
                left: 0
              });
              jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
                position: 'absolute',
                left: -w
              });
            }
            if (t === s) jQuery(o.slides, obj).find('li.slide:eq(0)').css({
              left: (s * w)
            });
            if (t === s - 1) jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
              left: s * w - w
            });
            u = false;
          });
          break;
        case 'prev':
          t = t - 1;
          m = (-(t * w - w));
          current(t);
          if (o.autoHeight) {
            autoHeight(t);
          }
          if (s < 3) {
            if (t === 0) {
              jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
                position: 'absolute',
                left: (-w)
              });
            }
            if (t === 1) {
              jQuery(o.slides, obj).find('li.slide:eq(0)').css({
                position: 'absolute',
                left: 0
              });
            }
          }
          jQuery(o.slides, obj).animate({
            left: m
          },
          o.slidespeed, o.easing, function () {
            if (t === 0) {
              t = s;
              jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
                position: 'absolute',
                left: (s * w - w)
              });
              jQuery(o.slides, obj).css({
                left: -(s * w - w)
              });
              jQuery(o.slides, obj).find('li.slide:eq(0)').css({
                left: (s * w)
              });
            }
            if (t === 2) jQuery(o.slides, obj).find('li.slide:eq(0)').css({
              position: 'absolute',
              left: 0
            });
            if (t === 1) jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
              position: 'absolute',
              left: -w
            });
            u = false;
          });
          break;
        case 'fade':
          t = [t] * 1;
          m = (-(t * w - w));
          current(t);
          if (o.autoHeight) {
            autoHeight(t);
          }
          jQuery(o.slides, obj).find('li:slide').fadeOut(o.fadespeed, function () {
            jQuery(o.slides, obj).css({
              left: m
            });
            jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
              left: s * w - w
            });
            jQuery(o.slides, obj).find('li.slide:eq(0)').css({
              left: 0
            });
            if (t === s) {
              jQuery(o.slides, obj).find('li.slide:eq(0)').css({
                left: (s * w)
              });
            }
            if (t === 1) {
              jQuery(o.slides, obj).find('li.slide:eq(' + (s - 1) + ')').css({
                position: 'absolute',
                left: -w
              });
            }
            jQuery(o.slides, obj).find('li:slide').fadeIn(o.fadespeed);
            u = false;
          });
          break;
        default:
          break;
        }
      };
    });
  };
})(jQuery);

/* "clearField" by Stijn Van Minnebruggen
   http://www.donotfold.be  */
(function (jQuery) {
  jQuery.fn.clearField = function (settings) {
    settings = jQuery.extend({
      blurClass: 'clearFieldBlurred',
      activeClass: 'clearFieldActive'
    },
    settings);
    jQuery(this).each(function () {
      var el = jQuery(this);
      if (el.hasClass('password') && el.hasClass('label')){
        el.show();
        var $target = jQuery('#'+el.attr('alt'));
        $target.hide();

        $target.blur(function () {
          if (el.hasClass('password') && el.hasClass('label') && (jQuery(this).val() == (''))){
           jQuery(this).hide();
           el.show();
           el.val(el.attr('rel')).removeClass(settings.activeClass).addClass(settings.blurClass);
          }
        });

      }

      if (el.attr('rel') == undefined) {
        el.attr('rel', el.val()).addClass(settings.blurClass);
      }
      el.focus(function () {
        if (el.val() == el.attr('rel')) {
          var v = '';
          if (el.attr('name') == 'url') v = 'http://';

          if (el.hasClass('password') && el.hasClass('label')){
            el.hide();
            $target.show();
            $target.focus();
          }

          el.val(v).removeClass(settings.blurClass).addClass(settings.activeClass);
        }
      });
      el.blur(function () {
        if ((el.val() == ('')) || ((el.attr('name') == 'url') && (el.val() == ('http://')))) {
          el.val(el.attr('rel')).removeClass(settings.activeClass).addClass(settings.blurClass);
        }
      });
    });
    return jQuery;
  };
})(jQuery);

/*
 * FancyBox - simple and fancy jQuery plugin
 * Examples and documentation at: http://fancy.klade.lv/
 * Version: 1.2.1 (13/03/2009)
 * Copyright (c) 2009 Janis Skarnelis
 * Licensed under the MIT License: http://en.wikipedia.org/wiki/MIT_License
 * Requires: jQuery v1.3+

 http://fancybox.net

 - MODIFIED FOR MYSTIQUE! BE CAREFUL WHEN UPDATING!

*/
(function (jQuery) {
  var elem, opts, busy = false,
    imagePreloader = new Image,
    loadingTimer, loadingFrame = 1,
    imageRegExp = /\.(jpg|gif|png|bmp|jpeg|media)(.*)?$/i;
  jQuery.fn.fancyboxlite = function (settings) {
    settings = jQuery.extend({},
    jQuery.fn.fancyboxlite.defaults, settings);
    var matchedGroup = this;

    function _initialize() {
      elem = this;
      opts = settings;
      _start();
      return false;
    };

    function _start() {
      if (busy) return;
      if (jQuery.isFunction(opts.callbackOnStart)) {
        opts.callbackOnStart();
      }
      opts.itemArray = [];
      opts.itemCurrent = 0;
      if (settings.itemArray.length > 0) {
        opts.itemArray = settings.itemArray;
      } else {
        var item = {};
        if (!elem.rel || elem.rel == '') {
          var item = {
            href: elem.href,
            title: elem.title
          };
          if (jQuery(elem).children("img:first").length) {
            item.orig = jQuery(elem).children("img:first");
          }
          opts.itemArray.push(item);
        } else {
          var subGroup = jQuery(matchedGroup).filter("a[rel=" + elem.rel + "]");
          var item = {};
          for (var i = 0; i < subGroup.length; i++) {
            item = {
              href: subGroup[i].href,
              title: subGroup[i].title
            };
            if (jQuery(subGroup[i]).children("img:first").length) {
              item.orig = jQuery(subGroup[i]).children("img:first");
            }
            opts.itemArray.push(item);
          }
          while (opts.itemArray[opts.itemCurrent].href != elem.href) {
            opts.itemCurrent++;
          }
        }
      }
      if (opts.overlayShow) {
        if (isIE6) {
          jQuery('embed, object, select').css('visibility', 'hidden');
        }
        jQuery("#fancyoverlay").css('opacity', 0).show().animate({
          opacity: opts.overlayOpacity
        },
        166);
      }
      _change_item();
    };

    function _change_item() {
      jQuery("#fancyright, #fancyleft, #fancytitle").fadeOut(333);
      var href = opts.itemArray[opts.itemCurrent].href;
      if (href.match(/#/)) {
        var target = window.location.href.split('#')[0];
        target = href.replace(target, '');
        target = target.substr(target.indexOf('#'));
        _set_content('<div id="fancydiv">' + jQuery(target).html() + '</div>', opts.frameWidth, opts.frameHeight);
      } else if (href.match(imageRegExp)) {
        imagePreloader = new Image;
        imagePreloader.src = href;
        if (imagePreloader.complete) {
          _proceed_image();
        } else {
          jQuery.fn.fancyboxlite.showLoading();
          jQuery(imagePreloader).unbind().bind('load', function () {
            jQuery(".fancyloading").hide();
            _proceed_image();
          });
        }
      } else if (href.match("iframe") || elem.className.indexOf("iframe") >= 0) {
        _set_content('<iframe id="fancyframe" onload="jQuery.fn.fancyboxlite.showIframe()" name="fancyiframe' + Math.round(Math.random() * 1000) + '" frameborder="0" hspace="0" src="' + href + '"></iframe>', opts.frameWidth, opts.frameHeight);
      } else {
        jQuery.get(href, function (data) {
          _set_content('<div id="fancyajax">' + data + '</div>', opts.frameWidth, opts.frameHeight);
        });
      }
    };

    function _proceed_image() {
      if (opts.imageScale) {
        var w = jQuery.fn.fancyboxlite.getViewport();
        var r = Math.min(Math.min(w[0] - 36, imagePreloader.width) / imagePreloader.width, Math.min(w[1] - 60, imagePreloader.height) / imagePreloader.height);
        var width = Math.round(r * imagePreloader.width);
        var height = Math.round(r * imagePreloader.height);
      } else {
        var width = imagePreloader.width;
        var height = imagePreloader.height;
      }
      _set_content('<img alt="" id="fancyimg" src="' + imagePreloader.src + '" />', width, height);
    };

    function _preload_neighbor_images() {
      if ((opts.itemArray.length - 1) > opts.itemCurrent) {
        var href = opts.itemArray[opts.itemCurrent + 1].href;
        if (href.match(imageRegExp)) {
          objNext = new Image();
          objNext.src = href;
        }
      }
      if (opts.itemCurrent > 0) {
        var href = opts.itemArray[opts.itemCurrent - 1].href;
        if (href.match(imageRegExp)) {
          objNext = new Image();
          objNext.src = href;
        }
      }
    };

    function _set_content(value, width, height) {
      busy = true;
      var pad = opts.padding;
      if (isIE6) {
        jQuery("#fancycontent")[0].style.removeExpression("height");
        jQuery("#fancycontent")[0].style.removeExpression("width");
      }
      if (pad > 0) {
        width += pad * 2;
        height += pad * 2;
        jQuery("#fancycontent").css({
          'top': pad + 'px',
          'right': pad + 'px',
          'bottom': pad + 'px',
          'left': pad + 'px',
          'width': 'auto',
          'height': 'auto'
        });
        if (isIE6) {
          jQuery("#fancycontent")[0].style.setExpression('height', '(this.parentNode.clientHeight - 20)');
          jQuery("#fancycontent")[0].style.setExpression('width', '(this.parentNode.clientWidth - 20)');
        }
      } else {
        jQuery("#fancycontent").css({
          'top': 0,
          'right': 0,
          'bottom': 0,
          'left': 0,
          'width': '100%',
          'height': '100%'
        });
      }
      if (jQuery("#fancyouter").is(":visible") && width == jQuery("#fancyouter").width() && height == jQuery("#fancyouter").height()) {
        jQuery("#fancycontent").fadeOut(99, function () {
          jQuery("#fancycontent").empty().append(jQuery(value)).fadeIn(99, function () {
            _finish();
          });
        });
        return;
      }
      var w = jQuery.fn.fancyboxlite.getViewport();
      var itemLeft = (width + 36) > w[0] ? w[2] : (w[2] + Math.round((w[0] - width - 36) / 2));
      var itemTop = (height + 50) > w[1] ? w[3] : (w[3] + Math.round((w[1] - height - 50) / 2));
      var itemOpts = {
        'left': itemLeft,
        'top': itemTop,
        'width': width + 'px',
        'height': height + 'px'
      };
      if (jQuery("#fancyouter").is(":visible")) {
        jQuery("#fancycontent").fadeOut(99, function () {
          jQuery("#fancycontent").empty();
          jQuery("#fancyouter").animate(itemOpts, opts.zoomSpeedChange, opts.easingChange, function () {
            jQuery("#fancycontent").append(jQuery(value)).fadeIn(99, function () {
              _finish();
            });
          });
        });
      } else {
        if (opts.zoomSpeedIn > 0 && opts.itemArray[opts.itemCurrent].orig !== undefined) {
          jQuery("#fancycontent").empty().append(jQuery(value));
          var orig_item = opts.itemArray[opts.itemCurrent].orig;
          var orig_pos = jQuery.fn.fancyboxlite.getPosition(orig_item);
          jQuery("#fancyouter").css({
            'left': (orig_pos.left - 18) + 'px',
            'top': (orig_pos.top - 18) + 'px',
            'width': jQuery(orig_item).width(),
            'height': jQuery(orig_item).height()
          });
          if (opts.zoomOpacity) {
            itemOpts.opacity = 'show';
          }
          jQuery("#fancyouter").animate(itemOpts, opts.zoomSpeedIn, opts.easingIn, function () {
            _finish();
          });
        } else {
          jQuery("#fancycontent").hide().empty().append(jQuery(value)).show();
          jQuery("#fancyouter").css(itemOpts).fadeIn(99, function () {
            _finish();
          });
        }
      }
    };

    function _set_navigation() {
      if (opts.itemCurrent != 0) {
        jQuery("#fancyleft, #fancyleftico").unbind().bind("click", function (e) {
          e.stopPropagation();
          opts.itemCurrent--;
          _change_item();
          return false;
        });
        jQuery("#fancyleft").show();
      }
      if (opts.itemCurrent != (opts.itemArray.length - 1)) {
        jQuery("#fancyright, #fancyrightico").unbind().bind("click", function (e) {
          e.stopPropagation();
          opts.itemCurrent++;
          _change_item();
          return false;
        });
        jQuery("#fancyright").show();
      }
    };

    function _finish() {
      _set_navigation();
      _preload_neighbor_images();
      jQuery(document).keydown(function (e) {
        if (e.keyCode == 27) {
          jQuery.fn.fancyboxlite.close();
          jQuery(document).unbind("keydown");
        } else if (e.keyCode == 37 && opts.itemCurrent != 0) {
          opts.itemCurrent--;
          _change_item();
          jQuery(document).unbind("keydown");
        } else if (e.keyCode == 39 && opts.itemCurrent != (opts.itemArray.length - 1)) {
          opts.itemCurrent++;
          _change_item();
          jQuery(document).unbind("keydown");
        }
      });
      if (opts.centerOnScroll) {
        jQuery(window).bind("resize scroll", jQuery.fn.fancyboxlite.scrollBox);
      } else {
        jQuery("div#fancyouter").css("position", "absolute");
      }
      if (opts.hideOnContentClick) {
        jQuery("#fancywrap").click(jQuery.fn.fancyboxlite.close);
      }
      jQuery("#fancyoverlay").bind("click", jQuery.fn.fancyboxlite.close);
      if (opts.itemArray[opts.itemCurrent].title !== undefined && opts.itemArray[opts.itemCurrent].title.length > 0) {
        jQuery('#fancytitle').html(opts.itemArray[opts.itemCurrent].title);
        jQuery('#fancytitle').fadeIn(133);
      }
      if (opts.overlayShow && isIE6) {
        jQuery('embed, object, select', jQuery('#fancycontent')).css('visibility', 'visible');
      }
      if (jQuery.isFunction(opts.callbackOnShow)) {
        opts.callbackOnShow();
      }
      busy = false;
    };
    return this.unbind('click').click(_initialize);
  };
  jQuery.fn.fancyboxlite.scrollBox = function () {
    var pos = jQuery.fn.fancyboxlite.getViewport();
    jQuery("#fancyouter").css('left', ((jQuery("#fancyouter").width() + 36) > pos[0] ? pos[2] : pos[2] + Math.round((pos[0] - jQuery("#fancyouter").width() - 36) / 2)));
    jQuery("#fancyouter").css('top', ((jQuery("#fancyouter").height() + 50) > pos[1] ? pos[3] : pos[3] + Math.round((pos[1] - jQuery("#fancyouter").height() - 50) / 2)));
  };
  jQuery.fn.fancyboxlite.getNumeric = function (el, prop) {
    return parseInt(jQuery.curCSS(el.jquery ? el[0] : el, prop, true)) || 0;
  };
  jQuery.fn.fancyboxlite.getPosition = function (el) {
    var pos = el.offset();
    pos.top += jQuery.fn.fancyboxlite.getNumeric(el, 'paddingTop');
    pos.top += jQuery.fn.fancyboxlite.getNumeric(el, 'borderTopWidth');
    pos.left += jQuery.fn.fancyboxlite.getNumeric(el, 'paddingLeft');
    pos.left += jQuery.fn.fancyboxlite.getNumeric(el, 'borderLeftWidth');
    return pos;
  };
  jQuery.fn.fancyboxlite.showIframe = function () {
    jQuery(".fancyloading").hide();
    jQuery("#fancyframe").show();
  };
  jQuery.fn.fancyboxlite.getViewport = function () {
    return [jQuery(window).width(), jQuery(window).height(), jQuery(document).scrollLeft(), jQuery(document).scrollTop()];
  };
  jQuery.fn.fancyboxlite.animateLoading = function () {
    if (!jQuery("#fancyloading").is(':visible')) {
      clearInterval(loadingTimer);
      return;
    }
    loadingFrame = (loadingFrame + 1) % 12;
  };
  jQuery.fn.fancyboxlite.showLoading = function () {
    clearInterval(loadingTimer);
    var pos = jQuery.fn.fancyboxlite.getViewport();
    jQuery("#fancyloading").css({
      'left': ((pos[0] - 40) / 2 + pos[2]),
      'top': ((pos[1] - 40) / 2 + pos[3])
    }).show();
    jQuery("#fancyloading").bind('click', jQuery.fn.fancyboxlite.close);
    loadingTimer = setInterval(jQuery.fn.fancyboxlite.animateLoading, 66);
  };
  jQuery.fn.fancyboxlite.close = function () {
    busy = true;
    jQuery(imagePreloader).unbind();
    jQuery("#fancyoverlay").unbind();
    if (opts.hideOnContentClick) {
      jQuery("#fancywrap").unbind();
    }
    jQuery(".fancyloading, #fancyleft, #fancyright, #fancytitle").fadeOut(133);
    if (opts.centerOnScroll) {
      jQuery(window).unbind("resize scroll");
    }
    __cleanup = function () {
      jQuery("#fancyouter").hide();
      jQuery("#fancyoverlay").fadeOut(133);
      if (opts.centerOnScroll) {
        jQuery(window).unbind("resize scroll");
      }
      if (isIE6) {
        jQuery('embed, object, select').css('visibility', 'visible');
      }
      if (jQuery.isFunction(opts.callbackOnClose)) {
        opts.callbackOnClose();
      }
      busy = false;
    };
    if (jQuery("#fancyouter").is(":visible") !== false) {
      if (opts.zoomSpeedOut > 0 && opts.itemArray[opts.itemCurrent].orig !== undefined) {
        var orig_item = opts.itemArray[opts.itemCurrent].orig;
        var orig_pos = jQuery.fn.fancyboxlite.getPosition(orig_item);
        var itemOpts = {
          'left': (orig_pos.left - 18) + 'px',
          'top': (orig_pos.top - 18) + 'px',
          'width': jQuery(orig_item).width(),
          'height': jQuery(orig_item).height()
        };
        if (opts.zoomOpacity) {
          itemOpts.opacity = 'hide';
        }
        jQuery("#fancyouter").stop(false, true).animate(itemOpts, opts.zoomSpeedOut, opts.easingOut, __cleanup);
      } else {
        jQuery("#fancyouter").stop(false, true).fadeOut(99, __cleanup);
      }
    } else {
      __cleanup();
    }
    return false;
  };
  jQuery.fn.fancyboxlite.build = function () {
    var html = '';
    html += '<div id="fancyoverlay"></div>';
    html += '<div id="fancywrap">';
    html += '<div class="fancyloading" id="fancyloading"><div></div></div>';
    html += '<div id="fancyouter">';
    html += '<div id="fancyinner">';
    html += '<a href="javascript:;" id="fancyleft"><span class="fancyico" id="fancyleftico"></span></a><a href="javascript:;" id="fancyright"><span class="fancyico" id="fancyrightico"></span></a>';
    html += '<div id="fancycontent"></div>';
    html += '</div>';
    html += '<div id="fancytitle"></div>';
    html += '</div>';
    html += '</div>';
    jQuery(html).appendTo("body");
    if (isIE6) {
      jQuery("#fancyinner").prepend('<iframe class="fancybigIframe" scrolling="no" frameborder="0"></iframe>');
    }
  };
  jQuery.fn.fancyboxlite.defaults = {
    padding: 10,
    imageScale: true,
    zoomOpacity: true,
    zoomSpeedIn: 0,
    zoomSpeedOut: 0,
    zoomSpeedChange: 300,
    easingIn: 'swing',
    easingOut: 'swing',
    easingChange: 'swing',
    frameWidth: 425,
    frameHeight: 355,
    overlayShow: true,
    overlayOpacity: 0.3,
    hideOnContentClick: true,
    centerOnScroll: true,
    itemArray: [],
    callbackOnStart: null,
    callbackOnShow: null,
    callbackOnClose: null
  };
  jQuery(document).ready(function () {
    jQuery.fn.fancyboxlite.build();
  });
})(jQuery);

function liteboxCallback() {
  jQuery('.flickrGallery li a').fancyboxlite({
    'zoomSpeedIn': 333,
    'zoomSpeedOut': 333,
    'zoomSpeedChange': 133,
    'easingIn': 'easeOutQuart',
    'easingOut': 'easeInQuart',
    'overlayShow': true,
    'overlayOpacity': 0.75
  });
}

/*
 * jQuery Flickr - jQuery plug-in
 * Version 1.0, Released 2008.04.17
 *
 * Copyright (c) 2008 Daniel MacDonald (www.projectatomic.com)
 * Dual licensed GPL http://www.gnu.org/licenses/gpl.html
 * and MIT http://www.opensource.org/licenses/mit-license.php
 */
(function (jQuery) {
  jQuery.fn.flickr = function (o) {
    var s = {
      api_key: null,
      // [string]    required, see http://www.flickr.com/services/api/misc.api_keys.html
      type: null,
      // [string]    allowed values: 'photoset', 'search', default: 'flickr.photos.getRecent'
      photoset_id: null,
      // [string]    required, for type=='photoset'
      text: null,
      // [string]    for type=='search' free text search
      user_id: null,
      // [string]    for type=='search' search by user id
      group_id: null,
      // [string]    for type=='search' search by group id
      tags: null,
      // [string]    for type=='search' comma separated list
      tag_mode: 'any',
      // [string]    for type=='search' allowed values: 'any' (OR), 'all' (AND)
      sort: 'relevance',
      // [string]    for type=='search' allowed values: 'date-posted-asc', 'date-posted-desc', 'date-taken-asc', 'date-taken-desc', 'interestingness-desc', 'interestingness-asc', 'relevance'
      thumb_size: 's',
      // [string]    allowed values: 's' (75x75), 't' (100x?), 'm' (240x?)
      size: null,
      // [string]    allowed values: 'm' (240x?), 'b' (1024x?), 'o' (original), default: (500x?)
      per_page: 100,
      // [integer]   allowed values: max of 500
      page: 1,
      // [integer]   see paging notes
      attr: '',
      // [string]    optional, attributes applied to thumbnail <a> tag
      api_url: null,
      // [string]    optional, custom url that returns flickr JSON or JSON-P 'photos' or 'photoset'
      params: '',
      // [string]    optional, custom arguments, see http://www.flickr.com/services/api/flickr.photos.search.html
      api_callback: '?',
      // [string]    optional, custom callback in flickr JSON-P response
      callback: null // [function]  optional, callback function applied to entire <ul>
      // PAGING NOTES: jQuery Flickr plug-in does not provide paging functionality, but does provide hooks for a custom paging routine
      // within the <ul> created by the plug-in, there are two hidden <input> tags,
      // input:eq(0): current page, input:eq(1): total number of pages, input:eq(2): images per page, input:eq(3): total number of images
      // SEARCH NOTES: when setting type to 'search' at least one search parameter  must also be passed text, user_id, group_id, or tags
      // SIZE NOTES: photos must allow viewing original size for size 'o' to function, if not, default size is shown
    };
    if (o) jQuery.extend(s, o);
    return this.each(function () { // create unordered list to contain flickr images
      var list = jQuery('<ul class="clearfix">').appendTo(this);
      var url = jQuery.flickr.format(s);
      jQuery.getJSON(url, function (r) {
        if (r.stat != "ok") {
          for (i in r) {
            jQuery('<li>').text(i + ': ' + r[i]).appendTo(list);
          };
        } else {
          if (s.type == 'photoset') r.photos = r.photoset; // add hooks to access paging data
          list.append('<input type="hidden" value="' + r.photos.page + '" />');
          list.append('<input type="hidden" value="' + r.photos.pages + '" />');
          list.append('<input type="hidden" value="' + r.photos.perpage + '" />');
          list.append('<input type="hidden" value="' + r.photos.total + '" />');
          for (var i = 0; i < r.photos.photo.length; i++) {
            var photo = r.photos.photo[i]; // format thumbnail url
            var t = 'http://farm' + photo['farm'] + '.static.flickr.com/' + photo['server'] + '/' + photo['id'] + '_' + photo['secret'] + '_' + s.thumb_size + '.jpg'; //format image url
            var h = 'http://farm' + photo['farm'] + '.static.flickr.com/' + photo['server'] + '/' + photo['id'] + '_';
            switch (s.size) {
            case 'm':
              h += photo['secret'] + '_m.jpg';
              break;
            case 'b':
              h += photo['secret'] + '_b.jpg';
              break;
            case 'o':
              if (photo['originalsecret'] && photo['originalformat']) {
                h += photo['originalsecret'] + '_o.' + photo['originalformat'];
              } else {
                h += photo['secret'] + '_b.jpg';
              };
              break;
            default:
              h += photo['secret'] + '.jpg';
            };
            list.append('<li><a class="thumb" rel="flickr" href="' + h + '" ' + s.attr + ' title="' + photo['title'] + '"><img src="' + t + '" alt="' + photo['title'] + '" /></a></li>'); //galleryPreview("#flickrGallery li a.thumb","tooltip");
          };
          if (s.callback) s.callback(list);
        };
      });
    });
  }; // static function to format the flickr API url according to the plug-in settings
  jQuery.flickr = {
    format: function (s) {
      if (s.url) return s.url;
      var url = 'http://api.flickr.com/services/rest/?format=json&jsoncallback=' + s.api_callback + '&api_key=' + s.api_key;
      switch (s.type) {
      case 'photoset':
        url += '&method=flickr.photosets.getPhotos&photoset_id=' + s.photoset_id;
        break;
      case 'search':
        url += '&method=flickr.photos.search&sort=' + s.sort;
        if (s.user_id) url += '&user_id=' + s.user_id;
        if (s.group_id) url += '&group_id=' + s.group_id;
        if (s.tags) url += '&tags=' + s.tags;
        if (s.tag_mode) url += '&tag_mode=' + s.tag_mode;
        if (s.text) url += '&text=' + s.text;
        break;
      default:
        url += '&method=flickr.photos.getRecent';
      };
      if (s.size == 'o') url += '&extras=original_format';
      url += '&per_page=' + s.per_page + '&page=' + s.page + s.params;
      return url;
    }
  };
})(jQuery);

///* twitter functions  */
///* http://jquery-howto.blogspot.com/2009/04/jquery-twitter-api-plugin.html */
//(function (jQuery) {
//  jQuery.extend({
//    jTwitter: function (username, fnk) {
//      var url = "http://twitter.com/status/user_timeline/" + username + ".json?count=1&callback=?";
//      var info = {};
//      jQuery.getJSON(url, function (data) {
//        if (typeof fnk == 'function') fnk.call(this, data[0].user);
//      });
//    }
//  });
//})(jQuery);
//
//
//
///* http://tweet.seaofclouds.com/ */
//(function(jQuery) {
//
//  jQuery.fn.getTwitter = function(o){
//    var s = {
//      username: ["wordpress"],                // [string]   required, unless you want to display our tweets. :) it can be an array, just do ["username1","username2","etc"]
//      avatar_size: null,                      // [integer]  height and width of avatar if displayed (48px max)
//      count: 6,                               // [integer]  how many tweets to display?
//      loading_text: null,                     // [string]   optional loading text, displayed while tweets load
//      query: null                             // [string]   optional search query
//    };
//
//    jQuery.fn.extend({
//      linkUrl: function() {
//        var returning = [];
//        var regexp = /((ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?)/gi;
//        this.each(function() {
//          returning.push(this.replace(regexp,"<a href=\"$1\">$1</a>"))
//        });
//        return jQuery(returning);
//      },
//      linkUser: function() {
//        var returning = [];
//        var regexp = /[\@]+([A-Za-z0-9-_]+)/gi;
//        this.each(function() {
//          returning.push(this.replace(regexp,"<a href=\"http://twitter.com/$1\">@$1</a>"))
//        });
//        return jQuery(returning);
//      },
//      linkHash: function() {
//        var returning = [];
//        var regexp = / [\#]+([A-Za-z0-9-_]+)/gi;
//        this.each(function() {
//          returning.push(this.replace(regexp, ' <a href="http://search.twitter.com/search?q=&tag=$1&lang=all&from='+s.username.join("%2BOR%2B")+'">#$1</a>'))
//        });
//        return jQuery(returning);
//      },
//      capAwesome: function() {
//        var returning = [];
//        this.each(function() {
//          returning.push(this.replace(/(a|A)wesome/gi, 'AWESOME'))
//        });
//        return jQuery(returning);
//      },
//      capEpic: function() {
//        var returning = [];
//        this.each(function() {
//          returning.push(this.replace(/(e|E)pic/gi, 'EPIC'))
//        });
//        return jQuery(returning);
//      },
//      makeHeart: function() {
//        var returning = [];
//        this.each(function() {
//          returning.push(this.replace(/[&lt;]+[3]/gi, "<tt class='heart'>&#x2665;</tt>"))
//        });
//        return jQuery(returning);
//      }
//    });
//
//    function relative_time(time_value) {
//      var parsed_date = Date.parse(time_value);
//      var relative_to = (arguments.length > 1) ? arguments[1] : new Date();
//      var delta = parseInt((relative_to.getTime() - parsed_date) / 1000);
//      if(delta < 60) {
//      return 'less than a minute ago';
//      } else if(delta < 120) {
//      return 'about a minute ago';
//      } else if(delta < (45*60)) {
//      return (parseInt(delta / 60)).toString() + ' minutes ago';
//      } else if(delta < (90*60)) {
//      return 'about an hour ago';
//      } else if(delta < (24*60*60)) {
//      return 'about ' + (parseInt(delta / 3600)).toString() + ' hours ago';
//      } else if(delta < (48*60*60)) {
//      return '1 day ago';
//      } else {
//      return (parseInt(delta / 86400)).toString() + ' days ago';
//      }
//    }
//
//    if(o) jQuery.extend(s, o);
//    return this.each(function(){
//      var list = jQuery('<ul class="tweet_list">').appendTo(this);
//      list.hide();
//      var loading = jQuery('<p class="preLoader">'+s.loading_text+'</p>');
//      if(typeof(s.username) == "string"){
//        s.username = [s.username];
//      }
//      var query = '';
//      if(s.query) {
//        query += 'q='+s.query;
//      }
//      query += '&q=from:'+s.username.join('%20OR%20from:');
//      var url = 'http://search.twitter.com/search.json?&'+query+'&rpp='+s.count+'&callback=?';
//      if (s.loading_text) jQuery(this).append(loading);
//      jQuery.getJSON(url, function(data){
//        if (s.loading_text) loading.remove();
//        jQuery.each(data.results, function(i,item){
//
//          var avatar_template = '<a class="tweet_avatar" href="http://twitter.com/'+ item.from_user+'"><img src="'+item.profile_image_url+'" height="'+s.avatar_size+'" width="'+s.avatar_size+'" alt="'+item.from_user+'\'s avatar" border="0"/></a>';
//          var avatar = (s.avatar_size ? avatar_template : '')
//          var date = '<a class="date" href="http://twitter.com/'+item.from_user+'/statuses/'+item.id+'" title="view tweet on twitter">'+relative_time(item.created_at)+'</a>';
//          var text = '<span class="entry">' +jQuery([item.text]).linkUrl().linkUser().linkHash().makeHeart().capAwesome().capEpic()[0]+ date + '</span>';
//
//          // until we create a template option, arrange the items below to alter a tweet's display.
//          list.append('<li>' + text + '</li>');
//
//          list.children('li:first').addClass('firstTweet');
//          list.children('li:odd').addClass('even');
//          list.children('li:even').addClass('lastTweet');
//
//
//        });
//
//        list.animate({opacity: 'toggle', height: 'toggle'}, 500, 'easeOutQuart');
//
//      });
//
//
//    });
//  };
//})(jQuery);


// cookie functions
jQuery.cookie = function (name, value, options) {
  if (typeof value != 'undefined') { // name and value given, set cookie
    options = options || {};
    if (value === null) {
      value = '';
      options = jQuery.extend({},
      options); // clone object since it's unexpected behavior if the expired property were changed
      options.expires = -1;
    }
    var expires = '';
    if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
      var date;
      if (typeof options.expires == 'number') {
        date = new Date();
        date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
      } else {
        date = options.expires;
      }
      expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
    } // NOTE Needed to parenthesize options.path and options.domain
    // in the following expressions, otherwise they evaluate to undefined
    // in the packed version for some reason...
    var path = options.path ? '; path=' + (options.path) : '';
    var domain = options.domain ? '; domain=' + (options.domain) : '';
    var secure = options.secure ? '; secure' : '';
    document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
  } else { // only name given, get cookie
    var cookieValue = null;
    if (document.cookie && document.cookie != '') {
      var cookies = document.cookie.split(';');
      for (var i = 0; i < cookies.length; i++) {
        var cookie = jQuery.trim(cookies[i]); // Does this cookie string begin with the name we want?
        if (cookie.substring(0, name.length + 1) == (name + '=')) {
          cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
          break;
        }
      }
    }
    return cookieValue;
  }
};

// fixes for IE-7/8 cleartype bug on fade in/out
jQuery.fn.fadeIn = function (speed, callback) {
  return this.animate({
    opacity: 'show'
  },
  speed, function () {
    if (jQuery.browser.msie) this.style.removeAttribute('filter');
    if (jQuery.isFunction(callback)) callback();
  });
};
jQuery.fn.fadeOut = function (speed, callback) {
  return this.animate({
    opacity: 'hide'
  },
  speed, function () {
    if (jQuery.browser.msie) this.style.removeAttribute('filter');
    if (jQuery.isFunction(callback)) callback();
  });
};
jQuery.fn.fadeTo = function (speed, to, callback) {
  return this.animate({
    opacity: to
  },
  speed, function () {
    if (to == 1 && jQuery.browser.msie) this.style.removeAttribute('filter');
    if (jQuery.isFunction(callback)) callback();
  });
};

// nundge effect
jQuery.fn.nudge = function (params) { //set default parameters
  params = jQuery.extend({
    amount: 20,
    //amount of pixels to pad / marginize
    duration: 300,
    //amount of milliseconds to take
    property: 'padding',
    //the property to animate (could also use margin)
    direction: 'left',
    //direction to animate (could also use right)
    toCallback: function () {},
    //function to execute when MO animation completes
    fromCallback: function () {} //function to execute when MOut animation completes
  },
  params); //For every element meant to nudge...
  this.each(function () { //variables
    var $t = jQuery(this);
    var $p = params;
    var dir = $p.direction;
    var prop = $p.property + dir.substring(0, 1).toUpperCase() + dir.substring(1, dir.length);
    var initialValue = $t.css(prop);
    /* fx */
    var go = {};
    go[prop] = parseInt($p.amount) + parseInt(initialValue);
    var bk = {};
    bk[prop] = initialValue; //Proceed to nudge on hover
    $t.hover(function () {
      $t.stop().animate(go, $p.duration, '', $p.toCallback);
    },
    function () {
      $t.stop().animate(bk, $p.duration, '', $p.fromCallback);
    });
  });
  return this;
};

// bubble
(function (jQuery) {
  jQuery.fn.bubble = function (options) {
    jQuery.fn.bubble.defaults = {
      timeout: 0,
      offset: 22
    };
    var o = jQuery.extend({},
    jQuery.fn.bubble.defaults, options);
    return this.each(function () {
      var showTip = function () {
        var el = jQuery(this).find('.bubble').css('display', 'block')[0];
        var ttHeight = jQuery(el).height();
        var ttOffset = el.offsetHeight;
        var ttTop = ttOffset + ttHeight;
        jQuery(this).find('.bubble').stop().css({
          'opacity': 0,
          'top': 2 - ttOffset
        }).animate({
          'opacity': 1,
          'top': o.offset - ttOffset
        },
        250);
      };
      var hideTip = function () {
        var self = this;
        var el = jQuery('.bubble', this).css('display', 'block')[0];
        var ttHeight = jQuery(el).height();
        var ttOffset = el.offsetHeight;
        var ttTop = ttOffset + ttHeight;
        jQuery(this).find('.bubble').stop().animate({
          'opacity': 0,
          'top': 12 - ttOffset
        },
        250, 'swing', function () {
          el.hiding = false;
          jQuery(this).css('display', 'none');
        })
      }
      jQuery(this).find('.bubble').hover(function () {
        return false;
      },
      function () {
        return true;
      });
      jQuery(this).hover(function () {
        var self = this;
        showTip.apply(this);
        if (o.timeout > 0) this.tttimeout = setTimeout(function () {
          hideTip.apply(self)
        },
        o.timeout);
      },
      function () {
        clearTimeout(this.tttimeout);
        hideTip.apply(this);
      });
    });
  };
})(jQuery);

//Private function for setting cookie
function updateCookie(target, data) {
  var cookie = target.replace(/[#. ]/g, '');
  jQuery.cookie(cookie, data, {
    path: '/'
  });
}

function fontControl(container, target, minSize, maxSize) {
  jQuery(container).append('<a href="javascript:void(0);" class="fontSize bubble" title="Increase or decrease text size"></a>');
  var cookie = 'page-font-size';
  var value = jQuery.cookie(cookie);
  if (value != null) {
    jQuery(target).css('fontSize', parseInt(value));
  } //on clicking small font button, font size is decreased by 1px
  jQuery(container + " .fontSize").click(function () {
    curSize = parseInt(jQuery(target).css("fontSize"));
    newSize = curSize + 1;
    if (newSize > maxSize) newSize = minSize;
    if (newSize >= minSize) //jQuery(target).css('fontSize', newSize);
    jQuery(target).animate({
      fontSize: newSize
    },
    333, 'swing');
    updateCookie(cookie, newSize); //sets the cookie
  });
}

function pageWidthControl(container, target, fullWidth, fixedWidth, fluidWidth) {
  jQuery(container).append('<a href="javascript:void(0);" class="pageWidth bubble" title="switch from fixed to fluid page width"></a>');
  var cookie = 'page-max-width';
  var value = jQuery.cookie(cookie);
  if (value != null) {
    jQuery(target).css('maxWidth', value);
  }
  jQuery(container + " .pageWidth").click(function () {
    curMaxWidth = jQuery(target).css('maxWidth');
    newMaxWidth = curMaxWidth;
    switch (curMaxWidth) {
    case fullWidth:
      newMaxWidth = fixedWidth;
      break;
    case fixedWidth:
      newMaxWidth = fluidWidth;
      break;
    case fluidWidth:
      newMaxWidth = fullWidth;
      break;
    default:
      newMaxWidth = fluidWidth;
    }
    jQuery(target).animate({
      maxWidth: newMaxWidth
    },
    333, 'easeOutQuart');
    updateCookie(cookie, newMaxWidth); //sets the cookie
  });
}

/* old menu */
//(function (jQuery) {
//  jQuery.fn.dropDown = function (options) {
//    jQuery.fn.dropDown.defaults = {
//      delay: 0
//    };
//    var o = jQuery.extend({},
//    jQuery.fn.dropDown.defaults, options);
//    return this.each(function () {
//      jQuery(this).find("ul").css({
//        display: "none"
//      });
//      jQuery(this).find("li").hover(function () {
//        jQuery(this).find('ul:first').css({
//          display: "block",
//          opacity: 0,
//          marginLeft: 20
//        }).animate({
//          opacity: 1,
//          marginLeft: 0
//        },
//        150, 'swing');
//      },
//      function () {
//        jQuery(this).find('ul:first').animate({
//          opacity: 0,
//          marginLeft: 20
//        },
//        150, 'swing', function () {
//          jQuery(this).css({
//            display: "none"
//          });
//        });
//      });
//    });
//  };
//})(jQuery);

/*
 * Superfish v1.4.8 - jQuery menu widget
 * Copyright (c) 2008 Joel Birch
 *
 * Dual licensed under the MIT and GPL licenses:
 * 	http://www.opensource.org/licenses/mit-license.php
 * 	http://www.gnu.org/licenses/gpl.html
 *
 * CHANGELOG: http://users.tpg.com.au/j_birch/plugins/superfish/changelog.txt
 
- MODIFIED FOR MYSTIQUE! BE CAREFUL WHEN UPDATING!

 */
;
(function (jQuery) {
  jQuery.fn.superfish = function (op) {
    var sf = jQuery.fn.superfish,
      c = sf.c,
      $arrow = jQuery(['<span class="', c.arrowClass, '"> &#187;</span>'].join('')),
      over = function () {
      var $$ = jQuery(this),
        menu = getMenu($$);
      clearTimeout(menu.sfTimer);
      $$.showSuperfishUl().siblings().hideSuperfishUl();
    },
      out = function () {
      var $$ = jQuery(this),
        menu = getMenu($$),
        o = sf.op;
      clearTimeout(menu.sfTimer);
      menu.sfTimer = setTimeout(function () {
        o.retainPath = (jQuery.inArray($$[0], o.$path) > -1);
        $$.hideSuperfishUl();
        if (o.$path.length && $$.parents(['li.', o.hoverClass].join('')).length < 1) {
          over.call(o.$path);
        }
      },
      o.delay);
    },
      getMenu = function ($menu) {
      var menu = $menu.parents(['ul.', c.menuClass, ':first'].join(''))[0];
      sf.op = sf.o[menu.serial];
      return menu;
    },
      addArrow = function ($a) {
      $a.addClass(c.anchorClass).append($arrow.clone());
    };
    return this.each(function () {
      var s = this.serial = sf.o.length;
      var o = jQuery.extend({},
      sf.defaults, op);
      o.$path = jQuery('li.' + o.pathClass, this).slice(0, o.pathLevels).each(function () {
        jQuery(this).addClass([o.hoverClass, c.bcClass].join(' ')).filter('li:has(ul)').removeClass(o.pathClass);
      });
      sf.o[s] = sf.op = o;
      jQuery('li:has(ul)', this)[(jQuery.fn.hoverIntent && !o.disableHI) ? 'hoverIntent' : 'hover'](over, out).each(function () {
        if (o.autoArrows) addArrow(jQuery('>a:first-child', this));
      }).not('.' + c.bcClass).hideSuperfishUl();
      var $a = jQuery('a', this);
      $a.each(function (i) {
        var $li = $a.eq(i).parents('li');
        $a.eq(i).focus(function () {
          over.call($li);
        }).blur(function () {
          out.call($li);
        });
      });
      o.onInit.call(this);
    }).each(function () {
      var menuClasses = [c.menuClass];
      jQuery(this).addClass(menuClasses.join(' '));
    });
  };
  var sf = jQuery.fn.superfish;
  sf.o = [];
  sf.op = {};
  sf.c = {
    bcClass: 'sf-breadcrumb',
    menuClass: 'sf-js-enabled',
    anchorClass: 'sf-with-ul',
    arrowClass: 'arrow'
  };
  sf.defaults = {
    hoverClass: 'sfHover',
    pathClass: 'overideThisToUse',
    pathLevels: 1,
    delay: 500,
    speed: 'normal',
    autoArrows: true,
    disableHI: false,
    // true disables hoverIntent detection
    onInit: function () {},
    // callback functions
    onBeforeShow: function () {},
    onShow: function () {},
    onHide: function () {}
  };
  jQuery.fn.extend({
    hideSuperfishUl: function () {
      var o = sf.op,
        not = (o.retainPath === true) ? o.$path : '';
      o.retainPath = false;
      if (isIE) {
        css1 = {
          marginLeft: 20
        };
      } else {
        css1 = {
          opacity: 0,
          marginLeft: 20
        };
      }
      var $ul = jQuery(['li.', o.hoverClass].join(''), this).add(this).not(not).removeClass(o.hoverClass).find('>ul').animate(css1, 150, 'swing', function () {
        jQuery(this).css({
          display: "none"
        })
      });
      o.onHide.call($ul);
      return this;
    },
    showSuperfishUl: function () {
      var o = sf.op,
        $ul = this.addClass(o.hoverClass).find('>ul:hidden').css('visibility', 'visible');
      o.onBeforeShow.call($ul);
      if (isIE) {
        css1 = {
          display: "block",
          marginLeft: 20
        };
        css2 = {
          marginLeft: 0
        };
      } else {
        css1 = {
          display: "block",
          opacity: 0,
          marginLeft: 20
        };
        css2 = {
          opacity: 1,
          marginLeft: 0
        };
      }
      $ul.css(css1).animate(css2, 150, 'swing', function () {
        o.onShow.call($ul);
      });
      return this;
    }
  });
})(jQuery);

// simple tooltips
function webshot(target_items, name) {
  jQuery(target_items).each(function (i) {
    jQuery("body").append("<div class='" + name + "' id='" + name + i + "'><img src='http://images.websnapr.com/?size=s&amp;url=" + jQuery(this).attr('href') + "' /></div>");
    var my_tooltip = jQuery("#" + name + i);
    jQuery(this).mouseover(function () {
      my_tooltip.css({
        opacity: 1,
        display: "none"
      }).fadeIn(333);
    }).mousemove(function (kmouse) {
      my_tooltip.css({
        left: kmouse.pageX + 15,
        top: kmouse.pageY + 15
      });
    }).mouseout(function () {
      my_tooltip.fadeOut(333);
    });
  });
}

// optimized minitabs
(function (jQuery) {
  jQuery.fn.minitabs = function (options) {
    jQuery.fn.minitabs.defaults = {
      content: '.sections',
      nav: 'ul:first',
      effect: 'top',
      speed: 333,
      cookies: true
    };
    var o = jQuery.extend({},
    jQuery.fn.minitabs.defaults, options);
    return this.each(function () {
      var $tabs = jQuery(this);
      var $instance = $tabs.attr('id');
      var $nav = jQuery('#' + $instance + ' ' + o.nav);
      if (o.cookies) { // check for the active tab cookie
        var cookieID = $instance;
        var cookieState = jQuery.cookie(cookieID);
      } // hide all sections
      $tabs.find(o.content + " >div:gt(0)").hide();
      if (o.cookies && (cookieState != null)) { // if we have a cookie then show the section according to its value
        $nav.find('li.' + cookieState).addClass("active");
        var link = $nav.find('li.' + cookieState + ' a');
        var section = link.attr('href');
        $tabs.find(o.content + ' div' + section).show();
      } else { // if not, show the 1st section
        $nav.find('li:last').addClass("active");
        $tabs.find(o.content + ' div:first').show();
      }
      $nav.find("li>a").click(function () {
        if (!jQuery(this).parent('li').hasClass("active")) {
          $nav.find('li').removeClass("active");
          if (o.cookies) {
            var cookieValue = jQuery(this).parent('li').attr("class");
            jQuery.cookie(cookieID, cookieValue, {
              path: '/'
            });
          }
          jQuery(this).parent('li').addClass("active");
          jQuery(this).blur();
          var re = /([_\-\w]+$)/i;
          var target = jQuery('#' + $instance + ' #' + re.exec(this.href)[1]);
          if (o.effect == 'slide') $tabs.find(o.content + " >div").slideUp(o.effect);
          else $tabs.find(o.content + " >div").hide();
          switch (o.effect) {
          case 'top':
            if (isIE) target.css({
              top: -300
            }).show().animate({
              top: 0
            },
            o.speed, 'easeOutQuart');
            else target.css({
              opacity: 0,
              top: -300
            }).show().animate({
              opacity: 1,
              top: 0
            },
            o.speed, 'easeOutQuart');
            break;
          case 'slide':
            target.slideDown(o.speed);
            break;
          case 'height':
            originalHeight = target.height();
            target.css({
              opacity: 0,
              height: 0
            }).show().animate({
              opacity: 1,
              height: originalHeight
            },
            o.speed, 'easeOutQuart');
            break;
          }
          return false;
        }
      })
    });
  };
})(jQuery);

// better alternative to slidetoggle
jQuery.fn.slideFade = function(type, speed, easing, callback) {
  if (isIE) return this.animate({height: type}, speed, easing, callback); // no fading on IE because of the text AA bug
  else return this.animate({opacity: type, height: type}, speed, easing, callback);
};


/*
// based on "Read More Right Here" plugin by William King - http://www.wooliet.com
function setup_readmorelink() {

  jQuery('a.more-link').each(function () {

    var url = jQuery(this).attr('href');
    var pos = url.lastIndexOf('-');

    jQuery(this).bind('click', {
      "el": jQuery(this),
      "url": url,
      "postid": url.substr(++pos)
    },
    ajaxClick);
  });


  function ajaxClick(event) {
    var theEl = event.data.el;
    //var loading = jQuery(document.createElement('span')).attr('class','loading');

    // append and display the loading image
    // after the 'more' anchor element
    theEl.html('Loading...');

    // perform the ajax request
    jQuery.ajax({
      type: "POST",
      url: event.data.url,
      dataType: "html",
      cache: false,
      data: {
        'redirect-more-link': '1',
        'postid': event.data.postid
      },
      error: function (request, textStatus, errorThrown) {
        //data = "<p class=\"error\">Sorry! There was an error retrieving content.<br>Click again to be taken to this entry's page.</p>";
        ajaxFinished(theEl, data, true);

      },
      success: function (data, textStatus) {
        ajaxFinished(theEl, data, false);
      }

    });
    // keep anchor 'click' event propagating
    return false;
  }



  function ajaxFinished(theEl, result, bError) {
    var newEl = jQuery("<p>").html(result).hide(),
      tempFunc, funcObjToggle = function () {
      newEl.find('object').each(

      function () {
        jQuery(this).toggle();
      });
    },
      funcArray = new Array(

    function () {
      funcObjToggle();
      newEl.slideFade('toggle',333, 'easeOutCubic', function () {

        theEl.html('More &gt;');
      });
    },

    function () {
      newEl.slideFade('toggle',333, 'easeOutCubic', function () {

        theEl.html('&lt; Less');
        funcObjToggle();

      });
    });

    theEl.unbind('click', ajaxClick);

    // If IE 7 and newer, and the new content has an
    //	embedded object (e.g. flash video), we have
    //	to just re-direct to the single page entry.
    //	The object will NOT display.
    if (isIE) if (hasEmbed(newEl)) {
      window.location = theEl.attr('href');
      return;
    }

    newEl.find('object').each(

    function () {
      jQuery(this).hide();
    });

    // put the new content after the more link
    theEl.after(newEl);

    newEl.slideFade('toggle',333, 'easeOutCubic', function () {

      theEl.html('&lt; Less');
      funcObjToggle();
    });

    // if no error, 'more' link slides the content in and
    // out of view; otherwise future clicks behave normally
    // and take user to the single post page
    if (!bError) {
      theEl.click(function () {
        funcArray[0]();

        // Swap functions to execute. When 'collapse'
        //	want object to hide first. When expand want
        //	object to show last.
        tempFunc = funcArray[0];
        funcArray[0] = funcArray[1];
        funcArray[1] = tempFunc;

        // keep anchor 'click' event propagating
        return false;
      });
    }
  }

  function hasEmbed(el) {
    var result = false;
    el.find('object').each(

    function () {
      result = true;
      console.log(this);
      return;
    });

    return result;
  }

}
*/



function setup_readmorelink() {

  jQuery('a.more-link').click(function () {

    var target_url = jQuery(this).attr('href');
    var pos = target_url.lastIndexOf('-');
    var postid = target_url.substr(++pos);

    var link = jQuery(this);
    var content = jQuery(this).parent();
    var more_content = jQuery("<div></div>")

    link.html('').addClass('loading');

    // perform the ajax request
    jQuery.ajax({
      type: "POST",
      url: target_url,
      dataType: "html",
      cache: false,
      data: {
        'redirect-more-link': 1,
        'postid': postid
      },
      error: function (request, textStatus, errorThrown) {
        window.location = link.attr('href'); // go to link

      },
      success: function (data, textStatus) {
        more_content.html(data).hide();
        content.append(more_content);
        link.remove();
        more_content.slideFade('show',333, 'easeOutCubic', function () {
      });

      }

    });
    return false;
  });
}




     jQuery.fn.extend({
	plainHtml: function(value) {
		if (value == undefined) {
			return (this[0] ? this[0].innerHTML : null);
		}
		else if(this[0]) {
			try {
				this[0].innerHTML = value;
			} catch(e) {}
			return this;
		}
	}
});


// quote comment
(function (jQuery) {
  jQuery.fn.quoteComment = function (options) {
    jQuery.fn.quoteComment.defaults = {
      comment: 'li.comment',
      comment_id: '.comment-id',
      author: '.comment-author',
      source: '.comment-body',
      target: '#comment'
    };
    jQuery.fn.appendVal = function (txt) {
      return this.each(function () {
        this.value += txt;
      });
    };
    var o = jQuery.extend({},
    jQuery.fn.quoteComment.defaults, options);
    return this.each(function () {
      jQuery(this).click(function () {
        $c = jQuery(this).parents(o.comment).find(o.source);
        $author = jQuery(this).parents(o.comment).find(o.author);
        $cid = jQuery(this).parents(o.comment).find(o.comment_id).attr('href');
        jQuery(o.target).appendVal('<blockquote>\n<a href="' + $cid + '">\n<strong><em>' + $author.html() + ':</em></strong>\n</a>\n ' + $c.html() + '</blockquote>');
        jQuery(o.target).focus();
        return false;
      })
    });
  };
})(jQuery);


function setup_comments(){

 jQuery("a#show-author-info").click(function () {
    jQuery("#author-info").slideFade('toggle',333,'easeOutQuart');
  });

  function quote(){
    jQuery.fn.appendVal = function (txt) {
      return this.each(function () {
        this.value += txt;
      });
    };

    comment_class = 'li.comment';
    target_id = '#comment';

    jQuery('a.quote').click(function () {
        $c = jQuery(this).parents(comment_class).find('.comment-text');
        $author = jQuery(this).parents(comment_class).find('.comment-author');
        $cid = jQuery(this).parents(comment_class).find('.comment-id').attr('href');
        jQuery(target_id).appendVal('<blockquote>\n<a href="' + $cid + '">\n<strong><em>' + $author.html() + ':</em></strong>\n</a>\n ' + $c.html() + '</blockquote>');
        jQuery(target_id).focus();
        return false;
      })

  }

  function reply(){

  // replaces wp's comment reply script
  jQuery("a.reply").click(function () {
    linkClass = jQuery(this).attr('id');
    var pos = linkClass.lastIndexOf('-');
    var targetID = linkClass.substr(++pos);

    jQuery("#respond").hide();

    jQuery('#comment_parent').attr('value',targetID);
    // jQuery('#comment_post_ID').attr('value',postID);

    jQuery("#cancel-reply").show();
    jQuery("#respond").appendTo('#comment-body-'+targetID).show(0,function(){

      // move cursor in textarea, at the end of the text
      jQuery('#comment').each(function(){
       if (this.createTextRange) {
        var r = this.createTextRange();
        r.collapse(false);
        r.select();
       }
       jQuery(this).focus();
      });

    });

    return false;
  });

  // cancel-reply
  jQuery("#cancel-reply").click(function (event) {
    jQuery("#respond").hide();
    jQuery('#comment_parent').attr('value','0');
    //  jQuery('#comment_post_ID').attr('value',postID);

    jQuery("#cancel-reply").hide();
    jQuery("#respond").appendTo('#section-comments').show(0,function(){

     // move cursor in textarea, at the end of the text
     jQuery('#comment').each(function(){
      if (this.createTextRange) {
       var r = this.createTextRange();
       r.collapse(false);
       r.select();
      }
      jQuery(this).focus();
     });

    });
    return false;
  });

  }


  function comment_navi(){
  jQuery(".comment-navigation a").click(function () {
    $link = jQuery(this);
    var link_url = $link.attr('href');


//	var commentPage = 1;
//	if (/comment-page-/i.test(link_url)) {
//		commentPage = link_url.split(/^.*comment-page-/)[1];
//        commentPage = commentPage.split(/(\/|#|&).*$/)[0];
//	} else if (/cpage=/i.test(link_url)) {
//		commentPage = link_url.split(/^.*cpage=/)[1];
//        commentPage = commentPage.split(/(\/|#|&).*$/)[0];
//	}

	var ajax_url = link_url.split(/#.*$/)[0];
	//ajax_url += /\?/i.test(link_url) ? '&' : '?';

	jQuery('.comment-navigation').before('<div id="pagination_status"></div>');
	var status = jQuery('#pagination_status');

    jQuery.ajax({
        url: ajax_url,
        type: "GET",
        data: ({action: 'commentnavi'/*, page: commentPage*/}),
		beforeSend: function() {
			status.empty();
            $link.removeAttr('href').addClass('loading').html('&nbsp;');
            jQuery("ul#comments").css('opacity', 1).animate({ opacity: 0.5 }, 333,'easeOutQuart');

		}, // end beforeSubmit

		error: function(request){

		        window.location=target_url;
				return false;
		},

        success: function(data) {
           try {
                var response = jQuery("<div />").plainHtml(data);
                    $comm = response.find('#comments-wrap').plainHtml();

                jQuery("ul#comments #cancel-reply").hide();
                jQuery("ul#comments").find("#respond").appendTo('#section-comments');

			 	jQuery('#comments-wrap').plainHtml($comm);
                jQuery("ul#comments").css('opacity', 0.5).animate({ opacity: 1 }, 333,'easeOutQuart');
				status.empty();

            } catch (e) {

				status.attr("class","error").plainHtml(e);

            }

                if (window.AjaxEditComments) AjaxEditComments.init();
                reply();
                quote();
                comment_navi();
                //comment_submit();
                jQuery(".comment-head").bubble({timeout: 0});

			}

		});

    return false;
  });
  }

//  function comment_submit(){
//	jQuery('#respond form').before('<div id="comment_post_status"></div>');
//	var form = jQuery('#commentform');
//	var status = jQuery('#comment_post_status');
//    var submit_text = jQuery('#submit').val();
//
//
//    form.submit(function(evt) {
//
//    jQuery(this).ajaxSubmit({
//
//		beforeSubmit: function() {
//			status.empty();
//			jQuery('#submit, #comment').attr('disabled','disabled');
//          	jQuery('#submit').val('Posting. Please wait..');
//		}, // end beforeSubmit
//
//		error: function(request){
//				status.empty();
//				if (request.responseText.search(/<title>WordPress &rsaquo; Error<\/title>/) != -1) {
//					var data = request.responseText.match(/<p>(.*)<\/p>/);
//					status.attr("class","error").plainHtml(data[1]);
//				} else {
//					var data = request.responseText;
//					status.attr("class","error").plainHtml(data[1]);
//				}
//				jQuery('#submit').val(submit_text);
//				jQuery('#submit, #comment').removeAttr("disabled");
//				return false;
//		},
//
//        success: function(data) {
//           try {
//                var response = jQuery("<div />").plainHtml(data);
//                    $comm = response.find('#comments-wrap').plainHtml();
//                    // if is a nested comment
//                    jQuery("ul#comments #cancel-reply").hide();
//                    jQuery("ul#comments").find("#respond").appendTo('#section-comments');
//
//			 		jQuery('#comments-wrap').plainHtml($comm);
//				    status.empty();
//                    status.attr("class","success").plainHtml("Comment successfully sent.");
//                    jQuery('#comment').val('');
//                    jQuery('#submit').val("Post another comment");
//                    jQuery('#submit, #comment').removeAttr("disabled");
//
//            } catch (e) {
//				jQuery('#submit, #comment').removeAttr("disabled");
//				status.attr("class","error").plainHtml(e);
//
//            }
//
//
//
//                if (window.AjaxEditComments) AjaxEditComments.init();
//                reply();
//                quote();
//                jQuery(".comment-head").bubble({timeout: 0});
//
//
//			}
//
//		});
//
//        return false;
//
//	});
//  }


  // WP Ajax Edit Comments hook
  if (window.AjaxEditComments) AjaxEditComments.init();
  reply();
  quote();
  jQuery(".comment-head").bubble({timeout: 0});

  if(ajaxComments){
     comment_navi();
     //comment_submit();
  }

}








(function( $ ){
  $.cssRule = function (Selector, Property, Value) {

    // Selector == {}
    if(typeof Selector == "object"){
      jQuery.each(Selector, function(NewSelector, NewProperty){
        jQuery.cssRule(NewSelector, NewProperty);
      });
      return;
    }

    // Selector == "body:background:#F99"
    if((typeof Selector == "string") && (Selector.indexOf(":") > -1)
      && (Property == undefined) && (Value == undefined)){
      Data = Selector.split("{");
      Data[1] = Data[1].replace(/\}/, "");
      jQuery.cssRule(jQuery.trim(Data[0]), jQuery.trim(Data[1]));
      return;
    }

    // Check for multi-selector, [ IE don't accept multi-selector on this way, we need to split ]
    if((typeof Selector == "string") && (Selector.indexOf(",") > -1)){
      Multi = Selector.split(",");
      for(x = 0; x < Multi.length; x++){
        Multi[x] = jQuery.trim(Multi[x]);
        if(Multi[x] != "")
          jQuery.cssRule(Multi[x], Property, Value);
      }

      return;
    }

    // Porperty == {} or []
    if(typeof Property == "object"){

      // Is {}
      if(Property.length == undefined){

        // Selector, {}
        jQuery.each(Property, function(NewProperty, NewValue){
          jQuery.cssRule(Selector + " " + NewProperty, NewValue);
        });

      // Is [Prop, Value]
      }else if((Property.length == 2) && (typeof Property[0] == "string") &&
        (typeof Property[1] == "string")){
        jQuery.cssRule(Selector, Property[0], Property[1]);

      // Is array of settings
      }else{
        for(x1 = 0; x1 < Property.length; x1++){
          jQuery.cssRule(Selector, Property[x1], Value);
        }
      }

      return;
    }

    // Parse for property at CSS Style "{property:value}"
    if((typeof Property == "string") && (Property.indexOf("{") > -1)
       && (Property.indexOf("}") > -1)){
      Property = Property.replace(/\{/, "").replace(/\}/, "");
    }

    // Check for multiple properties
    if((typeof Property == "string") && (Property.indexOf(";") > -1)){
      Multi1 = Property.split(";");
      for(x2 = 0; x2 < Multi1.length; x2++){
        jQuery.cssRule(Selector, Multi1[x2], undefined);
      }
      return;
    }

    // Check for property:value
    if((typeof Property == "string") && (Property.indexOf(":") > -1)){
      Multi3 = Property.split(":");
      jQuery.cssRule(Selector, Multi3[0], Multi3[1]);
      return;
    }

    //********************************************
    // Logical CssRule additions
    // Check for multiple logical properties [ "padding,margin,border:0px" ]
    if((typeof Property == "string") && (Property.indexOf(",") > -1)){
      Multi2 = Property.split(",");
      for(x3 = 0; x3 < Multi2.length; x3++){
        jQuery.cssRule(Selector, Multi2[x3], Value);
      }
      return;
    }


    if((Property == undefined) || (Value == undefined))
      return;

    Selector = jQuery.trim(Selector);
    Property = jQuery.trim(Property);
    Value = jQuery.trim(Value);

    if((Property == "") || (Value == ""))
      return;

    // adjusts on property
    if(jQuery.browser.msie){
      // for IE (@.@)^^^
      switch(Property){
        case "float": Property = "style-float"; break;
      }
    }else{
      // CSS rights
      switch(Property){
        case "float": Property = "css-float"; break;
      }
    }

    CssProperty = (Property || "").replace(/\-(\w)/g, function(m, c){ return (c.toUpperCase()); });



    if(Property && Value){
      for(var i = 0; i < document.styleSheets.length; i++){
        WorkerStyleSheet = document.styleSheets[i];
        if(WorkerStyleSheet.insertRule){
          Rules = (WorkerStyleSheet.cssRules || WorkerStyleSheet.rules);
          WorkerStyleSheet.insertRule(Selector + "{ " + Property + ":" + Value + "; }", Rules.length);
        }else if(WorkerStyleSheet.addRule){
          WorkerStyleSheet.addRule(Selector, Property + ":" + Value + ";", 0);
        }else{
          throw new Error("Add/insert not enabled.");
        }
      }
    }
  };
})( jQuery );





 jQuery.fn.extend({
    highlight: function(search, insensitive, hls_class){
      var regex = new RegExp("(<[^>]*>)|(\\b"+ search.replace(/([-.*+?^${}()|[\]\/\\])/g,"\\$1") +")", insensitive ? "ig" : "g");
      return this.html(this.html().replace(regex, function(a, b, c){
        return (a.charAt(0) == "<") ? a : "<strong class=\""+ hls_class +"\">" + c + "</strong>";
      }));
    }
  });



/*
* Print Element Plugin 1.0
*
* Copyright (c) 2009 Erik Zaadi
*
* Inspired by PrintArea (http://plugins.jquery.com/project/PrintArea) and
* http://stackoverflow.com/questions/472951/how-do-i-print-an-iframe-from-javascript-in-safari-chrome
*
*  jQuery plugin page : http://plugins.jquery.com/project/printElement
*  Wiki : http://wiki.github.com/erikzaadi/jQueryPlugins/jqueryprintelement
*  Home Page : http://erikzaadi.github.com/jQueryPlugins/jQuery.printElement
*
*  Thanks to David B (http://github.com/ungenio) and icgJohn (http://www.blogger.com/profile/11881116857076484100)
*  For their great contributions!
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
*   Note, Iframe Printing is not supported in Opera and Chrome 3.0, a popup window will be shown instead
*/
;
(function($){
    $.fn.printElement = function(options){
        var mainOptions = $.extend({}, $.fn.printElement.defaults, options);
        //iframe mode is not supported for opera and chrome 3.0 (it prints the entire page).
        //http://www.google.com/support/forum/p/Webmasters/thread?tid=2cb0f08dce8821c3&hl=en
        if (mainOptions.printMode == 'iframe') {
            if ($.browser.opera || (/chrome/.test(navigator.userAgent.toLowerCase())))
                mainOptions.printMode = 'popup';
        }
        //Remove previously printed iframe if exists
        $("[id^='printElement_']").remove();

        return this.each(function(){
            //Support Metadata Plug-in if available
            var opts = $.meta ? $.extend({}, mainOptions, $this.data()) : mainOptions;
            _printElement($(this), opts);
        });
    };
    $.fn.printElement.defaults = {
        printMode: 'iframe', //Usage : iframe / popup
        pageTitle: '', //Print Page Title
        overrideElementCSS: null,
        /* Can be one of the following 3 options:
         * 1 : boolean (pass true for stripping all css linked)
         * 2 : array of $.fn.printElement.cssElement (s)
         * 3 : array of strings with paths to alternate css files (optimized for print)
         */
        printBodyOptions: {
            styleToAdd: 'padding:10px;margin:10px;background:#fff;', //style attributes to add to the body of print document
            classNameToAdd: '' //css class to add to the body of print document
        },
        leaveOpen: false, // in case of popup, leave the print page open or not
        iframeElementOptions: {
            styleToAdd: 'border:none;position:absolute;width:0px;height:0px;bottom:0px;left:0px;background:#fff;', //style attributes to add to the iframe element
            classNameToAdd: '' //css class to add to the iframe element
        }
    };
    $.fn.printElement.cssElement = {
        href: '',
        media: ''
    };
    function _printElement(element, opts){
        //Create markup to be printed
        var html = _getMarkup(element, opts);

        var popupOrIframe = null;
        var documentToWriteTo = null;
        if (opts.printMode.toLowerCase() == 'popup') {
            popupOrIframe = window.open('about:blank', 'printElementWindow', 'width=650,height=440,scrollbars=yes');
            documentToWriteTo = popupOrIframe.document;
        }
        else {
            //The random ID is to overcome a safari bug http://www.cjboco.com.sharedcopy.com/post.cfm/442dc92cd1c0ca10a5c35210b8166882.html
            var printElementID = "printElement_" + (Math.round(Math.random() * 99999)).toString();
            //Native creation of the element is faster..
            var iframe = document.createElement('IFRAME');
            $(iframe).attr({
                style: opts.iframeElementOptions.styleToAdd,
                id: printElementID,
                className: opts.iframeElementOptions.classNameToAdd,
                frameBorder: 0,
                scrolling: 'no',
                src: 'about:blank'
            });
            document.body.appendChild(iframe);
            documentToWriteTo = (iframe.contentWindow || iframe.contentDocument);
            if (documentToWriteTo.document)
                documentToWriteTo = documentToWriteTo.document;
            iframe = document.frames ? document.frames[printElementID] : document.getElementById(printElementID);
            popupOrIframe = iframe.contentWindow || iframe;
        }
        focus();
        documentToWriteTo.open();
        documentToWriteTo.write(html);
        documentToWriteTo.close();
        _callPrint(popupOrIframe);
    };

    function _callPrint(element){
        if (element && element.printPage)
            element.printPage();
        else
            setTimeout(function(){
                _callPrint(element);
            }, 50);
    }

    function _getElementHTMLIncludingFormElements(element){
        var $element = $(element);
        //Radiobuttons and checkboxes
        $(":checked", $element).each(function(){
            this.setAttribute('checked', 'checked');
        });
        //simple text inputs
        $("input[type='text']", $element).each(function(){
            this.setAttribute('value', $(this).val());
        });
        $("select", $element).each(function(){
            var $select = $(this);
            $("option", $select).each(function(){
                if ($select.val() == $(this).val())
                    this.setAttribute('selected', 'selected');
            });
        });
        $("textarea", $element).each(function(){
            //Thanks http://blog.ekini.net/2009/02/24/jquery-getting-the-latest-textvalue-inside-a-textarea/
            var value = $(this).attr('value');
            if ($.browser.mozilla) {
               if(this.firstChild) this.firstChild.textContent = value;
            }
            else {
                this.innerHTML = value; }
        });
        var elementHtml = $element.html();
        return elementHtml;
    }

    function _getBaseHref(){
        return window.location.protocol + window.location.hostname + window.location.pathname;
    }

    function _getMarkup(element, opts){
        var $element = $(element);
        var elementHtml = _getElementHTMLIncludingFormElements(element);

        var html = new Array();
        html.push('<html><head><title>' + opts.pageTitle + '</title>');
        if (opts.overrideElementCSS) {
            if (opts.overrideElementCSS.length > 0) {
                for (var x = 0; x < opts.overrideElementCSS.length; x++) {
                    var current = opts.overrideElementCSS[x];
                    if (typeof(current) == 'string')
                        html.push('<link type="text/css" rel="stylesheet" href="' + current + '" >');
                    else
                        html.push('<link type="text/css" rel="stylesheet" href="' + current.href + '" media="' + current.media + '" >');
                }
            }
        }
        else {
            $(document).find("link").filter(function(){
                return $(this).attr("rel").toLowerCase() == "stylesheet";
            }).each(function(){
                html.push('<link type="text/css" rel="stylesheet" href="' + $(this).attr("href") + '" media="' + $(this).attr('media') + '" >');
            });
        }
        //Ensure that relative links work
        html.push('<base href="' + _getBaseHref() + '" />');
        html.push('</head><body style="' + opts.printBodyOptions.styleToAdd + '" class="' + opts.printBodyOptions.classNameToAdd + '">');
        html.push('<div class="' + $element.attr('class') + '">' + elementHtml + '</div>');
        html.push('<script type="text/javascript">function printPage(){focus();print();' + ((!$.browser.opera && !opts.leaveOpen && opts.printMode.toLowerCase() == 'popup') ? 'close();' : '') + '}</script>');
        html.push('</body></html>');

        return html.join('');
    };
    })(jQuery);






(function (jQuery) {
	jQuery.fn.addGrid = function (cols, options) {
		var defaults = {
			default_cols: 12,
			z_index: 999,
			img_path: '/images/',
			opacity:.6
		};

		// Extend our default options with those provided.
		var opts = jQuery.extend(defaults, options);

		var cols = cols != null && (cols === 12 || cols === 16) ? cols : 12;
		var cols = cols === opts.default_cols ? '12_col' : '16_col';

		return this.each(function () {
			var $el = jQuery(this);
			var height = $el.height();

			var wrapper = jQuery('<div id="'+opts.grid_id+'"/>')
				.appendTo($el)
				.css({
					'display':'none',
					'position':'absolute',
					'top':0,
					'z-index':(opts.z_index -1),
					'height':height,
					'opacity':opts.opacity,
					'width':'100%'});

			jQuery('<div/>')
				.addClass('container_12')
				.css({
					'margin':'0 auto',
					'width':'960px',
					'height':height,
					'background-image': 'url('+opts.img_path+cols + '.png)',
					'background-repeat': 'repeat-y'})
				.appendTo(wrapper);

				// add toggle
				jQuery('<div>grid on</div>')
					.appendTo($el)
					.css({
						'position':'absolute',
						'top':0,
						'left':0,
						'z-index':opts.z_index,
						'background': '#ed1e24',
						'font-weight': 'bold',
						'text-transform': 'uppercase',
						'color':'#fff',
						'padding': '3px 6px',
                        'cursor' : 'pointer',
						'text-align':'left'
					})
					.hover( function() {
						jQuery(this).css("cursor", "pointer");
					}, function() {
						jQuery(this).css("cursor", "default");
					})
					.toggle( function () {
						jQuery(this).text("grid off");
						jQuery('#'+opts.grid_id).slideDown();
					},
					function() {
						jQuery(this).text("grid on");
						jQuery('#'+opts.grid_id).slideUp();
					});

		});

	};
})(jQuery);



