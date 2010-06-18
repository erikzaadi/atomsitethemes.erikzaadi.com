/*

    Arclite theme : http://digitalnature.ro/projects/arclite/

    Slighty modified to fit AtomSite by Erik Zaadi (http://erikzaadi.com)

*/

var isIE = jQuery.browser.msie;
var isIE6 = parseInt(jQuery.browser.version) == 6;
var lightbox = true;


// ie 6 png fix
(function ($) {
    $.fn.pngfix = function (options) {

        // Review the Microsoft IE developer library for AlphaImageLoader reference
        // http://msdn2.microsoft.com/en-us/library/ms532969(VS.85).aspx

        // ECMA scope fix
        var elements = this;
        var settings = $.extend({
            imageFixSrc: false,
            sizingMethod: false
        }, options);

        if (!$.browser.msie || ($.browser.msie && $.browser.version >= 7)) {
            return (elements);
        }

        function setFilter(el, path, mode) {
            var fs = el.attr("filters");
            var alpha = "DXImageTransform.Microsoft.AlphaImageLoader";
            if (fs[alpha]) {
                fs[alpha].enabled = true;
                fs[alpha].src = path;
                fs[alpha].sizingMethod = mode;
            } else {
                el.css("filter", 'progid:' + alpha + '(enabled="true", sizingMethod="' + mode + '", src="' + path + '")');
            }
        }

        function setDOMElementWidth(el) {
            if (el.css("width") == "auto" & el.css("height") == "auto") {
                el.css("width", el.attr("offsetWidth") + "px");
            }
        }

        return (
			elements.each(function () {

			    // Scope
			    var el = $(this);

			    if (el.attr("tagName").toUpperCase() == "IMG" && (/\.png/i).test(el.attr("src"))) {
			        if (!settings.imageFixSrc) {

			            // Wrap the <img> in a <span> then apply style/filters,
			            // removing the <img> tag from the final render
			            el.wrap("<span></span>");
			            var par = el.parent();
			            par.css({
			                height: el.height(),
			                width: el.width(),
			                display: "inline-block"
			            });
			            setFilter(par, el.attr("src"), "scale");
			            el.remove();
			        } else if ((/\.gif/i).test(settings.imageFixSrc)) {

			            // Replace the current image with a transparent GIF
			            // and apply the filter to the background of the
			            // <img> tag (not the preferred route)
			            setDOMElementWidth(el);
			            setFilter(el, el.attr("src"), "image");
			            el.attr("src", settings.imageFixSrc);
			        }

			    } else {
			        var bg = new String(el.css("backgroundImage"));
			        var matches = bg.match(/^url\("(.*)"\)$/);
			        if (matches && matches.length) {

			            // Elements with a PNG as a backgroundImage have the
			            // filter applied with a sizing method relevant to the
			            // background repeat type
			            setDOMElementWidth(el);
			            el.css("backgroundImage", "none");

			            // Restrict scaling methods to valid MSDN defintions (or one custom)
			            var sc = "crop";
			            if (settings.sizingMethod) {
			                sc = settings.sizingMethod;
			            }
			            setFilter(el, matches[1], sc);

			            // Fix IE peek-a-boo bug for internal links
			            // within that DOM element
			            el.find("a").each(function () {
			                $(this).css("position", "relative");
			            });
			        }
			    }

			})
		);
    }

})(jQuery)


// time based tooltips
function initTooltips(o) {
    var showTip = function () {
        var el = jQuery('.tip', this).css('display', 'block')[0];
        var ttHeight = jQuery(el).height();
        var ttOffset = el.offsetHeight;
        var ttTop = ttOffset + ttHeight;
        jQuery('.tip', this)
	  .stop()
	  .css({ 'opacity': 0, 'top': 2 - ttOffset })
  	  .animate({ 'opacity': 1, 'top': 18 - ttOffset }, 250);
    };
    var hideTip = function () {
        var self = this;
        var el = jQuery('.tip', this).css('display', 'block')[0];
        var ttHeight = jQuery(el).height();
        var ttOffset = el.offsetHeight;
        var ttTop = ttOffset + ttHeight;
        jQuery('.tip', this)
	  	.stop()
	  	.animate({ 'opacity': 0, 'top': 10 - ttOffset }, 250, function () {
	  	    el.hiding = false;
	  	    jQuery(this).css('display', 'none');
	  	}
      );
    };
    jQuery('.tip').hover(
	  function () { return false; },
	  function () { return true; }
	);
    jQuery('.tiptrigger, .cat-item').hover(
	  function () {
	      var self = this;
	      showTip.apply(this);
	      if (o.timeout) this.tttimeout = setTimeout(function () { hideTip.apply(self) }, o.timeout);
	  },
	  function () {
	      clearTimeout(this.tttimeout);
	      hideTip.apply(this);
	  }
	);
}

// simple tooltips
function webshot(target_items, name) {
    jQuery(target_items).each(function (i) {
        jQuery("body").append("<div class='" + name + "' id='" + name + i + "'><p><img src='http://images.websnapr.com/?size=s&amp;url=" + jQuery(this).attr('href') + "' /></p></div>");
        var my_tooltip = jQuery("#" + name + i);

        jQuery(this).mouseover(function () {
            my_tooltip.css({ opacity: 0.8, display: "none" }).fadeIn(400);
        }).mousemove(function (kmouse) {
            my_tooltip.css({ left: kmouse.pageX + 15, top: kmouse.pageY + 15 });
        }).mouseout(function () {
            my_tooltip.fadeOut(400);
        });
    });
}




function liteboxCallback() {
    jQuery('.flickrGallery li a').fancybox({
        'zoomSpeedIn': 333,
        'zoomSpeedOut': 333,
        'zoomSpeedChange': 133,
        'easingIn': 'easeOutQuart',
        'easingOut': 'easeInQuart',
        'overlayShow': true,
        'overlayOpacity': 0.75
    });
}


/*!
* jQuery Konami code trigger v. 0.1
*
* Copyright (c) 2009 Joe Mastey
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* Usage:
*  // konami code unlocks the tetris
*  $('#tetris').konami(function(){
*     $(this).show();
*  });
* 
*
*  // enable all weapons on 'idkfa'.
*  // note that each weapon must be unlocked by its own code entry
*  $('.weapon').konami(function(){
*     $(this).addClass('enabled');
*  }, {'code':[73, 68, 75, 70, 65]});
*
*
*  // listens on any element that can trigger a keyup event.
*  // unlocks all weapons at once
*  $(document).konami(function(){
*     $('.weapon').addClass('enabled');
*  }, {'code':[73, 68, 75, 70, 65]});
*
*
*/
(function($) {
    $.fn.konami = function(fn, params) {
        params = $.extend({}, $.fn.konami.params, params);
        this.each(function() {
            var tgt = $(this);
            tgt.bind('konami', fn)
               .bind('keyup', function(event) { $.fn.konami.checkCode(event, params, tgt); });
        });
        return this;
    };

    $.fn.konami.params = {
        'code': [38, 38, 40, 40, 37, 39, 37, 39, 66, 65],
        'step': 0
    };

    $.fn.konami.checkCode = function(event, params, tgt) {
        if (event.keyCode == params.code[params.step]) {
            params.step++;
        } else {
            params.step = 0;
        }

        if (params.step == params.code.length) {
            tgt.trigger('konami');
            params.step = 0;
        }
    };
})(jQuery);




/*
* Superfish v1.4.8 - jQuery menu widget
* Copyright (c) 2008 Joel Birch
*
* Dual licensed under the MIT and GPL licenses:
* 	http://www.opensource.org/licenses/mit-license.php
* 	http://www.gnu.org/licenses/gpl.html
*
* CHANGELOG: http://users.tpg.com.au/j_birch/plugins/superfish/changelog.txt
*/

; (function (jQuery) {
    jQuery.fn.superfish = function (op) {

        var sf = jQuery.fn.superfish,
            c = sf.c,
            $arrow = jQuery(['<span class="', c.arrowClass, '"> &#187;</span>'].join('')),
            over = function () {
                var $$ = jQuery(this), menu = getMenu($$);
                clearTimeout(menu.sfTimer);
                $$.showSuperfishUl().siblings().hideSuperfishUl();
            },
            out = function () {
                var $$ = jQuery(this), menu = getMenu($$), o = sf.op;
                clearTimeout(menu.sfTimer);
                menu.sfTimer = setTimeout(function () {
                    o.retainPath = (jQuery.inArray($$[0], o.$path) > -1);
                    $$.hideSuperfishUl();
                    if (o.$path.length && $$.parents(['li.', o.hoverClass].join('')).length < 1) { over.call(o.$path); }
                }, o.delay);
            },
            getMenu = function ($menu) {
                var menu = $menu.parents(['ul.', c.menuClass, ':first'].join(''))[0];
                sf.op = sf.o[menu.serial];
                return menu;
            },
            addArrow = function ($a) { $a.addClass(c.anchorClass).append($arrow.clone()); };

        return this.each(function () {
            var s = this.serial = sf.o.length;
            var o = jQuery.extend({}, sf.defaults, op);
            o.$path = jQuery('li.' + o.pathClass, this).slice(0, o.pathLevels).each(function () {
                jQuery(this).addClass([o.hoverClass, c.bcClass].join(' '))
                    .filter('li:has(ul)').removeClass(o.pathClass);
            });
            sf.o[s] = sf.op = o;

            jQuery('li:has(ul)', this)[(jQuery.fn.hoverIntent && !o.disableHI) ? 'hoverIntent' : 'hover'](over, out).each(function () {
                if (o.autoArrows) addArrow(jQuery('>a:first-child', this));
            })
            .not('.' + c.bcClass)
                .hideSuperfishUl();

            var $a = jQuery('a', this);
            $a.each(function (i) {
                var $li = $a.eq(i).parents('li');
                $a.eq(i).focus(function () { over.call($li); }).blur(function () { out.call($li); });
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
        delay: 333,
        speed: 'normal',
        autoArrows: true,
        disableHI: false, 	// true disables hoverIntent detection
        onInit: function () { }, // callback functions
        onBeforeShow: function () { },
        onShow: function () { },
        onHide: function () { }
    };


    jQuery.fn.extend({
        hideSuperfishUl: function () {
            var o = sf.op,
                not = (o.retainPath === true) ? o.$path : '';
            o.retainPath = false;
            if (isIE) {
                css1 = { marginTop: 20 };

            } else {
                css1 = { opacity: 0, marginTop: 20 };
            }
            var $ul = jQuery(['li.', o.hoverClass].join(''), this).add(this).not(not).removeClass(o.hoverClass).find('>ul').animate(css1, 150, 'swing', function () {
                jQuery(this).css({ display: "none" })
            });
            o.onHide.call($ul);
            return this;
        },
        showSuperfishUl: function () {
            var o = sf.op,
            $ul = this.addClass(o.hoverClass).find('>ul:hidden').css('visibility', 'visible');
            o.onBeforeShow.call($ul);
            if (isIE) {
                css1 = { display: "block", marginTop: 20 };
                css2 = { marginTop: 0 };
            } else {
                css1 = { display: "block", opacity: 0, marginTop: 20 };
                css2 = { opacity: 1, marginTop: 0 };
            }
            $ul.css(css1).animate(css2, 150, 'swing', function () { o.onShow.call($ul); });
            return this;
        }
    });

})(jQuery);


// init.

jQuery(document).ready(function () {
    jQuery(".comment .avatar").pngfix();
    jQuery("h1.logo a img").pngfix();

    // fade span
    jQuery('.fadeThis, ul#footer-widgets li.widget li').append('<span class="hover"></span>').each(function () {
        var jQueryspan = jQuery('> span.hover', this).css('opacity', 0);
        jQuery(this).hover(function () {
            jQueryspan.stop().fadeTo(333, 1);
        }, function () {
            jQueryspan.stop().fadeTo(333, 0);
        });
    });


    jQuery('#sidebar ul.menu li li a').mouseover(function () {
        jQuery(this).animate({ marginLeft: "5px" }, 100);
    });
    jQuery('#sidebar ul.menu li li a').mouseout(function () {
        jQuery(this).animate({ marginLeft: "0px" }, 100);
    });


    jQuery('a.toplink').click(function () {
        jQuery('html').animate({ scrollTop: 0 }, 'slow');
    });

    //navigationeffects();
    jQuery('#nav').superfish({ autoArrows: true });

    if (lightbox) {
        // enable fancyBox for any link with rel="lightbox" and on links which references end in image extensions (jpg, gif, png)
        jQuery("a[rel='lightbox'], a[href$='.jpg'], a[href$='.jpeg'], a[href$='.gif'], a[href$='.png'], a[href$='.JPG'], a[href$='.JPEG'], a[href$='.GIF'], a[href$='.PNG'], a[href$='.png'], a[href$='.media']").fancybox({
            'speedIn': 333,
            'speedOut': 333,
            'speedChange': 133,
            'transitionIn'	: 'elastic',
			'transitionOut'	: 'elastic',
            'overlayShow': true,
            'overlayOpacity': 0.75,
        });
    }

    if (document.all && !window.opera && !window.XMLHttpRequest && jQuery.browser.msie) { var isIE6 = true; }
    else { var isIE6 = false; };
    jQuery.browser.msie6 = isIE6;
    if (!isIE6) {
        initTooltips({
            timeout: 6000
        });
    }

    webshot(".with-tooltip a", "tooltip");

    // widget title adjustments
    jQuery('.widget .titlewrap').each(function () { jQuery(this).prependTo(this.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode); });


    // set roles on some elements (for accessibility)
    jQuery("#nav").attr("role", "navigation");
    jQuery("#main-content").attr("role", "main");
    jQuery("#sidebar").attr("role", "complementary");
    jQuery("#searchform").attr("role", "search");
    
    jQuery(".watermark").watermark();
   
    $(document).konami(function(){
        var $vid = $('<div class="mariovidwrapper"><div id="mariovid"><object width="480" height="385"><param name="movie" value="http://www.youtube.com/v/7nHLan_u2T0&hl=en_US&fs=1&rel=0&autoplay=1"></param><param name="allowFullScreen" value="true"></param><param name="allowscriptaccess" value="always"></param><embed src="http://www.youtube.com/v/7nHLan_u2T0&hl=en_US&fs=1&rel=0&autoplay=1" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" width="480" height="385"></embed></object></div></div>')
            .appendTo("body");
        var $image = $('<img alt="mario" />');
        $image.load(function(){
                var $mario = $(this).wrap('<a class="mario" href="#mariovid" title="Tribute to 8 bit" />')
                    .parents(".mario");
                $mario
                    .fancybox()
                    .appendTo("body");
                $mario.slideToggle('slow');
        });
        $image.attr('src', $("#_root").val() + 'img/arclite/super_mario_mushroom.png');
    });

    $(".quote").click(function () {
        var $wrapper = $(this).parents('.comment-wrap2');
        var toqoute = $wrapper.find('.comment-body p').text();
        var name = $.trim($wrapper.find('.comment-head').find('.commenter').text());
        var commentid = $wrapper.find('.comment-head .commentid').attr('name');
        var text = 'Type your comment here<blockquote><a href="#' + commentid + '"><strong><em>' + name + ' </em></strong></a>' +
    '<p>' + toqoute + '</p></blockquote>';
        $('#txtComment').text(text);
        window.location.hash = 'addcomment';
        return false;
    });

    $(".approve").click(function () {
        $(this).text($.trim($(this).text()));
        var values = $(this).attr('rel').split(';');
        approve(values[0], values[1], this);
        return false;
    });
    $(".delete").click(function () {
        $(this).text($.trim($(this).text()));
        var values = $(this).attr('rel').split(';');
        del(values[0], values[1]);
        return false;
    });
});