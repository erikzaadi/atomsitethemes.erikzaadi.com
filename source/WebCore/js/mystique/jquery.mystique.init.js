
var $lang = new Array();
$lang[0] = "Posting. Please wait...";
$lang[1] = "Your comment was added.";
$lang[2] = "Post another comment";

// init
jQuery(document).ready(function ($) {
    if (isIE6) {
        jQuery('#page').append("<div class='crap-browser-warning'>You're using a old and buggy browser. Switch to a <a href='http://www.mozilla.com/firefox/'>normal browser</a> or consider <a href='http://www.microsoft.com/windows/internet-explorer'>upgrading your Internet Explorer</a> to the latest version</div>");
    }
    jQuery('#navigation').superfish({ autoArrows: true });

    webshot("a.websnapr", "webshot");

    jQuery("a[rel='lightbox'], a[href$='.jpg'], a[href$='.jpeg'], a[href$='.gif'], a[href$='.png'], a[href$='.JPG'], a[href$='.JPEG'], a[href$='.GIF'], a[href$='.PNG'], a[href$='.media'], a[href$='.MEDIA']").fancyboxlite({
        'zoomSpeedIn': 333,
        'zoomSpeedOut': 333,
        'zoomSpeedChange': 133,
        'easingIn': 'easeOutQuart',
        'easingOut': 'easeInQuart',
        'overlayShow': true,
        'overlayOpacity': 0.75
    });


    // layout controls
    fontControl("#pageControls", "body", 10, 18);
    pageWidthControl("#pageControls", ".page-content", '100%', '940px', '1200px');
    webshot("a.websnapr", "webshot");
    jQuery(".post-tabs").minitabs({
        content: '.sections',
        nav: '.tabs',
        effect: 'top',
        speed: 333,
        cookies: false
    });

    jQuery(".sidebar-tabs").minitabs({
        content: '.sections',
        nav: '.box-tabs',
        effect: 'slide',
        speed: 150
    });

    jQuery("ul.menuList .cat-item").bubble({
        timeout: 6000
    });
    jQuery(".shareThis, .bubble-trigger").bubble({
        offset: 16,
        timeout: 0
    });

    jQuery("#pageControls").bubble({
        offset: 30
    });
    jQuery('ul.menuList li a').nudge({
        property: 'padding',
        direction: 'left',
        amount: 6,
        duration: 166
    });
    jQuery('a.nav-extra').nudge({
        property: 'marginTop',
        direction: '',
        amount: -18,
        duration: 166
    });

    // fade effect
    if (!isIE) {
        jQuery('.fadeThis').append('<span class="hover"></span>').each(function () {
            var jQueryspan = jQuery('> span.hover', this).css('opacity', 0);
            jQuery(this).hover(function () {
                jQueryspan.stop().fadeTo(333, 1);
            },
      function () {
          jQueryspan.stop().fadeTo(333, 0);
      });
        });
    }
    jQuery("#footer-blocks.withSlider").loopedSlider();
    jQuery("#featured-content.withSlider").loopedSlider({
        autoStart: 10000,
        autoHeight: false
    }); // scroll to top
    jQuery("a#goTop").click(function () {
        if ($.browser.safari) {
            bodyelem = $("body")
        } else {
            bodyelem = $("html,body")
        }
        bodyelem.animate({
            scrollTop: 0
        },
    'slow');
    });
    jQuery('.clearField').clearField({
        blurClass: 'clearFieldBlurred',
        activeClass: 'clearFieldActive'
    });

    jQuery("a#show-author-info").livequery("click", function () {
        jQuery("#author-info").slideFade('toggle', 333, 'easeOutQuart');
    });

    setup_comment_controls();
    //setup_comment_ajax();
    jQuery('a.print').click(function () {
        jQuery('.post.single').printElement({ printMode: 'popup' });

        return false;
    });

    //$(".scrollable").scrollable({ speed: 333, circular: true, mousewheel: true }).autoscroll({ autoplay: true, interval: 1000 }); ;

    // set accessibility roles on some elements trough js (to not break the xhtml markup)
    jQuery("#navigation").attr("role", "navigation");
    jQuery("#primary-content").attr("role", "main");
    jQuery("#sidebar").attr("role", "complementary");
    jQuery("#searchform").attr("role", "search");

    //Enable Twitter
    var $twitter = jQuery("#twitterId");
    if ($twitter.length) {
        jQuery("#header a.twitter")
        .attr('href', $twitter.attr('href'))
        .css('display', 'block');
    }

});