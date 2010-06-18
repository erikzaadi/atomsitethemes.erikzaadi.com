;(function ($) {
function GetCustomizations(){
var prefix = "customization_" + $('#ThemeName').val() + "_";
    var $selects = $("select[name^='" + prefix + "']");
    var toReturn = new Array();
    $selects.each(function(){
        toReturn.push(this.name.replace(prefix,"") + "=" + $(this).val());
    });

   return encodeURIComponent(toReturn.join("&"));
}

    $(document).ready(function () {
    
    $("#LivePreview #LiveLink").click(function () {
        $(this).attr('href', $('#LivePreviewBaseUrl').val() + '?ThemeName=' + $('#ThemeName').val() + '&PageWidth=' + $('#PageWidth').val() + '&PageTemplate=' + $('#PageTemplate').val() +"&Customizations=" + GetCustomizations());
    });
    $("#LivePreview .JSLivePreview").css('visibility','visible');

        makeDraggable($("#ThemeSwitcher #Drag").get(0));
        var $showhide = $("#ThemeSwitcher #ShowHide").click(function () {
            if ($showhide.attr('disabled') == 'disabled')
                return;
            var show = $("#ThemeSwitcher #content").css('display') == 'none';
            var next = show ? "hide&nbsp;" : "show";
            $(this).html(next);
            $showhide.attr('disabled', 'disabled')
            if (show)
                $("#ThemeSwitcher #content").show("slow",
            function () {
                $showhide.attr('disabled', '')
            });
            else
                $("#ThemeSwitcher #content").hide("slow",
            function () {
                $showhide.attr('disabled', '')
            });
        })
    .focus(function () {
        $(this).blur();
    });
        $("#ThemeName").change(function () {
            var $select = $(this);
            var $targetDiv = $("#ThemeInfo" + $select.val());
            if ($select.attr('disabled') == 'disabled' || $targetDiv.hasClass("ThemeInfoSelected"))
                return;
            $select.attr('disabled', 'disabled');
            $(".ThemeInfoSelected").slideUp('slow', function () {
                $(this).removeClass("ThemeInfoSelected");
                $targetDiv.slideDown('slow', function () {
                    $(this).addClass("ThemeInfoSelected");
                    $select.attr('disabled', '');
                });
            });
        }).keyup(function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode == 38 || keyCode == 40) { // if up or down key is pressed
                $(this).change(); // trigger the change event
            }
        }); ;
        $("#ThemeSwitcher form").submit(function () {
            var $form = $(this);
            var theme = $("#ThemeName").val();
            $.post(
            $form.attr('action'),
            {
                ThemeName: theme,
                PageWidth: $("#PageWidth" + theme).val(),
                PageTemplate: $("#PageTemplate" + theme).val(),
                Customizations : GetCustomizations()
            },
            function (data, err) {
                if (data && data.success) {
                    $("#ThemeSwitcher #content").hide("slow",
                        function () {
                            window.location.reload(true);
                        });
                }
            },
            'json');
            return false;
        });

    });
    //Cheep ass drag, didn't want to include the entire jQuery UI library

    document.onmousemove = mouseMove;
    document.onmouseup = mouseUp;

    var dragObject = null;
    var mouseOffset = null;

    function mouseCoords(ev) {
        if (ev.pageX || ev.pageY) {
            return { x: ev.pageX, y: ev.pageY };
        }
        return {
            x: ev.clientX + document.body.scrollLeft - document.body.clientLeft,
            y: ev.clientY + document.body.scrollTop - document.body.clientTop
        };
    }

    function getMouseOffset(target, ev) {
        ev = ev || window.event;

        var docPos = getPosition(target);
        var mousePos = mouseCoords(ev);
        return { x: mousePos.x - docPos.x, y: mousePos.y - docPos.y };
    }

    function getPosition(e) {
        var left = 0;
        var top = 0;

        while (e.offsetParent) {
            left += e.offsetLeft;
            top += e.offsetTop;
            e = e.offsetParent;
        }

        left += e.offsetLeft;
        top += e.offsetTop;

        return { x: left, y: top };
    }

    function mouseMove(ev) {
        ev = ev || window.event;
        var mousePos = mouseCoords(ev);

        if (dragObject) {
            setTimeout(function () {
                $(dragObject).parents(".dragcontainer").css(
        {
            'top': (mousePos.y - mouseOffset.y - 25) + 'px',
            'left': (mousePos.x - mouseOffset.x - 25) + 'px'
        });
            }, 25);
            return false;
        }
    }
    function mouseUp() {
        $(dragObject).removeClass("active");
        dragObject = null;
    }

    function makeDraggable(item) {
        if (!item) return;
        item.onmousedown = function (ev) {
            $(this).addClass("active");
            dragObject = this;
            mouseOffset = getMouseOffset(this, ev);
            return false;
        }
    }
}) (jQuery);