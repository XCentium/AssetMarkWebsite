$(function() {

	/* old IE polyfill */
	if (window.PIE) {
		//console.log('pie loaded');
		$('.home-box').each(function() {
			PIE.attach(this);
		});	
		$('.page-box').each(function() {
			PIE.attach(this);
		});		
	}
	
	//home box links
	$('#home-box-opportunity').click(function(e) {
		window.location = 'opportunity/';
	});
	$('#home-box-connections').click(function(e) {
		window.location = 'connections/';
	});
	$('#home-box-getstarted').click(function(e) {
		window.location = 'getstarted/';
	});		

	//Connections more links
	$(".connections-more-link, .connections-triangle-link").click(function(e) {
		e.preventDefault();
		var link = $(this);
		var hrefLink = link.attr("href");
		var thisClass = link.attr("class");
		if (thisClass === "connections-more-link"){
			$(hrefLink).slideToggle('', function() {
				var htmlStr = link.html();
				if (htmlStr === "Close -"){
					link.html("More +");
				}else if (htmlStr === "More +"){
					link.html("Close -");
				}
			});			
		}else{
			$(hrefLink).slideToggle('', function() {
				var sibling= link.siblings(".connections-more-link");
				var htmlStr = sibling.html();
				if (htmlStr === "Close -"){
					sibling.html("More +");
				}else if (htmlStr === "More +"){
					sibling.html("Close -");
				}
			});				
		}
	});
	
	//conections modal
	$('.connections-modal-link').click(function(e){
		var dataBox = $(this).attr("data-box");
		
		var lightbox = $('#modal-lightbox');
		var lightboxContent =  $('#modal-lightbox-content');
		var box = $('#'+ dataBox);
		//console.log(box.html());
		lightboxContent.html( box.html() );
		
		lightbox.css('width', '725px' );
	
		lightbox.lightbox_me({
			'centered' : true,
			'closeSelector' : '.close',
			'overlaySpeed' : 100
		});		
		e.preventDefault();
		
		//tracking
		var fauxpath = window.location.pathname + dataBox;
		if (_elq) {
			_elq.trackEvent(fauxpath);
		}
		
	});
	$('#try-1').click(function(e) {
		$('#sign_up').lightbox_me({
			centered: true, 
			onLoad: function() { 
				$('#sign_up').find('input:first').focus()
				}
			});
		e.preventDefault();
	});	
});