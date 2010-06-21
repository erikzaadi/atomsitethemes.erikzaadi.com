$(document).ready(function(){
						   
	$('<div class="post-top"></div><div class="post-bottom"></div>').prependTo('.post, #comments'); // Add post-top and post-bottom to the post and comment divs
	$('<div class="sidebar-top"></div><div class="sidebar-bottom"></div>').prependTo('#sidebar');
	
	$('.japibas-ads img:odd').css('margin-right', '0'); // Remove the right margin on every 2nd ad
	
	$('.commentlist li').each(function(i) {
		$(this).append('<span class="comment-num">' + i + '</span>'); // Add Comment number to comments
	});


	// Drop down 
	$("#nav li").hover(function(){
		$('ul:first',this).css('visibility', 'visible');
    }, function(){
        $('ul:first',this).css('visibility', 'hidden');
    });
	
	
	/* Slider */
	
	function formatText(index, panel) {
		return index + "";
	}
		
	$('#featured-section').anythingSlider({
		easing: "swing",        // Anything other than "linear" or "swing" requires the easing plugin
		autoPlay: false,                 // This turns off the entire FUNCTIONALY, not just if it starts running or not.
		animationTime: 600,             // How long the slide transition takes
		buildNavigation: false,          // If true, builds and list of anchor links to link to each slide
		navigationFormatter: formatText       // Details at the top of the file on this use (advanced use)
     });
	
});