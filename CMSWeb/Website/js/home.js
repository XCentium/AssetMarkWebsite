/* 
			BookBlock flipboard animation 
			http://tympanus.net/codrops/2012/09/03/bookblock-a-content-flip-plugin/
			https://github.com/codrops/BookBlock
*/
$(function() {	
	
	//* Config BookBlock
	var homeBB = (function() {
				
		var config = {
				$bookBlock : $( '#home-hero-pages' ),
				$navNext : $( '.home-hero-next' ),
				count: 0
			},
			init = function() {
				config.$bookBlock.bookblock( {
					autoplay : true,
					interval : 4000,
					circular: true,
					speed : 800,
					shadowSides : 0.8,
					shadowFlip: 0.7,
					onEndFlip: function (old, page, isLimit) {
					    if (page == 0) {
					        config.count++;
					        if (config.count > 2) {
					            config.$bookBlock.bookblock("_stopSlideshow");
					        }
					    }
					}
				} );
				initEvents();
			},
			initEvents = function() {
				
				var $slides = config.$bookBlock.children();

				// add navigation events
				config.$navNext.on( 'click touchstart', function() {
					config.$bookBlock.bookblock( 'next' );
					return false;
				} );
				
				$('.home-hero-dots SPAN').on('click', function(evt) {
					var dotClass = $(this).attr('class').split(' ')[0];
					var dotNum = parseInt( dotClass.split('-')[3] , 10);
					config.$bookBlock.bookblock( 'jump', dotNum );
					evt.stopPropagation();
				});	
				
				// stop animation on menu click & show the appropriate hero
				$('.nav-main-item A').on('click', function(evt) {
					config.$bookBlock.bookblock( '_stopSlideshow' );
					var menuNum = $(this).attr('data-navlink');
					showHeroPage(menuNum);
				});
										
			};
		
			return { init : init };

	})();	
	
	var loadBookblockCss = function() {
			var link = document.createElement("link");
			link.setAttribute("rel", "stylesheet");
			link.setAttribute("type", "text/css");
			link.setAttribute("media", "screen");
			link.setAttribute("href", "css/bookblock.css")	;
			document.getElementsByTagName("head")[0].appendChild(link)
	};
	
	// shows the hero page with no animation
	var showHeroPage = function (pageNum) {
		$('.home-hero-page').css('display', 'none');
		$('#home-hero-page-'+ pageNum).css('display', 'list-item');
	};
	
	
	//---------------------
	// init home page hero
	
	if (Modernizr.csstransitions && Modernizr.csstransforms3d && Modernizr.csstransformspreserve3d) {
		// Use Bookblock
		loadBookblockCss();
		homeBB.init();
	}	else {
		// fallback animation
		Hero.init();
		// events 
		$('.home-hero-next').on('click', function(evt) {
			Hero.nextSlide();
			evt.stopPropagation();
		})
		$('.home-hero-dots SPAN').on('click', function(evt) {
			var dotClass = $(this).attr('class').split(' ')[0];
			var dotNum = parseInt( dotClass.split('-')[3] , 10);
			Hero.showSlide(dotNum);
			evt.stopPropagation();
		});
		
		// stop animation on menu click & show the appropriate hero
		$('.nav-main-item A').on('click', function(evt) {
			Hero.animationStop();
			var menuNum = $(this).attr('data-navlink');
			showHeroPage(menuNum);
		});		

	}
	
	//home hero links
	$('#home-hero-page-1').on('click', function(evt) {
		window.location = 'people/important-things/';
		evt.cancelDefault();
	});
	$('#home-hero-page-2').on('click', function() {
		window.location = 'practice/lets-talk/ ';
		evt.cancelDefault();
	});
	$('#home-hero-page-3').on('click', function() {
		window.location = 'together/three-sixty-support/';
		evt.cancelDefault();
	});	

});	
