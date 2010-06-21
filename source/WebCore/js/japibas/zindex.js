// jQuery IE7 Z-index fix
// http://css-tricks.com/snippets/jquery/fixing-ie-z-index/

$(function() {
	var zIndexNumber = 1000;
	$('#header, #nav li').each(function() {
		$(this).css('zIndex', zIndexNumber);
		zIndexNumber -= 10;
	});
});