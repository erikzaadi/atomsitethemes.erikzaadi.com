$(document).ready(function () {
    $('.arjuna-watermark').each(function () {
        var $elem = $(this);
        $elem.watermark({ watermarkText: $elem.attr('title'), watermarkCssClass: $elem.attr('rel') });
    });
}); 