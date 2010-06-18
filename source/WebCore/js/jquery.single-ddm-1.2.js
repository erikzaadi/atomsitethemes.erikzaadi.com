/*! 
 * Single Drop Down Menu 1.2 
 * September 26, 2009 
 * Corey Hart @ http://www.codenothing.com 
 */ 
;(function($, undefined){ 
    // bgiframe is needed to fix z-index problem for IE6 users. 
    $.fn.bgiframe = $.fn.bgiframe ? $.fn.bgiframe : $.fn.bgIframe ? $.fn.bgIframe : function(){ 
        // For applications that don't have bgiframe plugin installed, create a useless  
        // function that doesn't break the chain 
        return this; 
    }; 
 
     // Drop Menu Plugin 
    $.fn.singleDropMenu = function(options){ 
        return this.each(function(){ 
            // Default Settings 
            var $obj = $(this), timer, menu, 
                settings = $.extend({ 
                    timer: 500, 
                    parentMO: undefined, 
                    childMO: undefined, 
                    show: 'show', 
                    hide: 'hide' 
                }, options||{}, $.metadata ? $obj.metadata() : {}); 
     
            // Run Menu 
            $obj.children('li').bind('mouseover.single-ddm', function(){ 
                // Clear any open menus 
                if (menu && menu.data('single-ddm-i') != $(this).data('single-ddm-i')) 
                    closemenu(); 
                else 
                    menu =false; 
                 
                // Open nested list 
                $(this).children('a').addClass(settings.parentMO).siblings('ul')[settings.show](); 
            }).bind('mouseout.single-ddm', function(){ 
                // Prevent auto close 
                menu = $(this); 
                timer = setTimeout(closemenu, settings.timer); 
            }).each(function(i){ 
                // Attach indexs to each menu 
                $(this).data('single-ddm-i', i); 
            }).children('ul').bgiframe(); 
 
            // Dropped Menu Highlighting 
            $('li > ul > li', $obj).bind('mouseover.single-ddm', function(){ 
                $('a', this).addClass(settings.childMO); 
            }).bind('mouseout.single-ddm', function(){ 
                $('a', this).removeClass(settings.childMO); 
            }); 
     
            // Closes any open menus when mouse click occurs anywhere else on the page 
            $(document).click(closemenu); 
     
            // Function to close set menu 
            function closemenu(){ 
                if (menu && timer){ 
                    menu.children('a').removeClass(settings.parentMO).siblings('ul')[settings.hide](); 
                    clearTimeout(timer); 
                    menu = false; 
                } 
            } 
        }); 
    }; 
})(jQuery); 