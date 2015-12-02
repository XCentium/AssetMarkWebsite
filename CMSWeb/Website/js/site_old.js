/*** community gallery ***/

var CommGallery = {
	/* Community page thumbnail scroller / lightbox gallery 
		just add new photos to the galleryPhotos array
	*/
	

	// the list of photos
	galleryPhotos : [
		{ photo: 'community4.jpg', thumb: 'tn-community4.jpg', copy: 'Building houses with Habitat for Humanity' },	
		{ photo: 'community5.jpg', thumb: 'tn-community5.jpg', copy: 'Volunteering at a local food bank' },	
		{ photo: 'community2.jpg', thumb: 'tn-community2.jpg', copy: 'Volunteering at a local food bank' },		
		{ photo: 'community6.jpg', thumb: 'tn-community6.jpg', copy: 'Back-to-school donations for the Monument Crisis Center' }
	],
	
	config : {
		photoDir : '../../img/community/',
		thumbsShown: 5
	},
	
	lightboxShown : -1,
	currentThumb: 0,
	
	showGalleryLightbox : function(num) {
	
		if (num<0 || num >= CommGallery.galleryPhotos.length) {
			return;
		}
		
		$('#gallery-lightbox-copy').html( CommGallery.galleryPhotos[num].copy );
		$('#gallery-lightbox-img').html("<img alt='"+ CommGallery.galleryPhotos[num].copy +"' " +
																		"src='"+ CommGallery.config.photoDir +
																		CommGallery.galleryPhotos[num].photo + "'>" );
																	
		$('#gallery-lightbox').addClass('shown');
		
		// IE8 - Image sometimes is letting the background peek through. Reload the image to fix
		var img = $('#gallery-lightbox-img IMG').first();
		img.attr('src', img.attr('src') );
			
		$('#gallery-lightbox-nav-prev').removeClass('nav-disabled');
		$('#gallery-lightbox-nav-next').removeClass('nav-disabled');
		if(num===0) {
			$('#gallery-lightbox-nav-prev').addClass('nav-disabled');
		}
		if(num >= CommGallery.galleryPhotos.length-1) {
			$('#gallery-lightbox-nav-next').addClass('nav-disabled');
		}
		
		CommGallery.lightboxShown = num;
	},
	
	showGalleryLightboxNext : function() {
		CommGallery.showGalleryLightbox(CommGallery.lightboxShown+1);
	},
	showGalleryLightboxPrev : function() {
		CommGallery.showGalleryLightbox(CommGallery.lightboxShown-1);
	},	
				
	hideGalleryLightbox : function () {
		$('#gallery-lightbox').removeClass('shown');
	},

	// thumbnail click handler
	makeGalleryClickHandler : function (){
		return function(evt) {
			var num = parseInt( $(this).attr('id').split('-')[1] , 10);
			CommGallery.showGalleryLightbox(num);
		};
	},	
	
	populateThumbnails : function () {
	
		var thumbsRow = $('#thumbs-row');
		var img, div;
		for (var i=0;i< CommGallery.galleryPhotos.length;i++) {
		
			div = document.createElement('div');
			$(div).addClass('thumbnail')
						.attr('id', 'thumbnail-'+ i)
						.appendTo(thumbsRow)
						.on('click', CommGallery.makeGalleryClickHandler());
		
			img = document.createElement('img');
			$(img).attr('src', CommGallery.config.photoDir + CommGallery.galleryPhotos[i].thumb)
						.attr('alt', '')
						.appendTo($(div));
						
		} 
		CommGallery.setThumbArrows();		
	},

	setFirstThumb : function(num) {

		var numThumbs = $('.thumbnail').length;
		var thumbsShown = CommGallery.config.thumbsShown;
		// don't go past the ends
		if (num < 0) {
				num = 0;
		}
		if (num > numThumbs-thumbsShown) {
			num = numThumbs-thumbsShown;
		}
		
		for (var i=0; i<numThumbs; i++) {
			if (i >= num && i<=num+thumbsShown-1) {
				$('#thumbnail-'+i).removeClass('thumbnail-hidden');
			} else if ( num+thumbsShown > numThumbs-1 && i <= (num+thumbsShown-numThumbs-1) ) {
				// don't wrap
				// $('#thumbnail-'+i).removeClass('thumbnail-hidden');
			} else {
				$('#thumbnail-'+i).addClass('thumbnail-hidden');
			}
		}

		CommGallery.currentThumb = num;
		CommGallery.setThumbArrows();
	},
	
	setThumbArrows : function() {
		$('#thumbs-nav-left').removeClass('thumb-nav-disabled');
		$('#thumbs-nav-right').removeClass('thumb-nav-disabled');
		
		if (CommGallery.currentThumb === 0 ) {
			$('#thumbs-nav-left').addClass('thumb-nav-disabled');		
		}
		if (CommGallery.galleryPhotos.length <= CommGallery.config.thumbsShown) {
			$('#thumbs-nav-right').addClass('thumb-nav-disabled');		
		}
		if (CommGallery.currentThumb >=  CommGallery.galleryPhotos.length) {
			$('#thumbs-nav-right').addClass('thumb-nav-disabled');		
		}				
	},	
	
	setNextThumb : function() {
		CommGallery.setFirstThumb(CommGallery.currentThumb+1);
	},
	setPrevThumb : function() {
		CommGallery.setFirstThumb(CommGallery.currentThumb-1);
	},	

	init : function() {
		CommGallery.populateThumbnails();
	},

	xxx : function() {}

};



$(function() {

	if ($('.community').length > 0) {
		CommGallery.init();
	}

	// events
	$('#thumbs-nav-left').on('click', function(evt) {
			CommGallery.setPrevThumb();
	});
	$('#thumbs-nav-right').on('click', function(evt) {
			CommGallery.setNextThumb();
	});
	$('#gallery-lightbox-close').on('click', function(evt) {
			CommGallery.hideGalleryLightbox();
	});
	$('#gallery-lightbox-nav-prev').on('click', function(evt) {
			evt.preventDefault();
			CommGallery.showGalleryLightboxPrev();
	});
	$('#gallery-lightbox-nav-next').on('click', function(evt) {
			evt.preventDefault();	
			CommGallery.showGalleryLightboxNext();
	}); 
	

});

;/*** 360 Degree Infographic ***/

// ** Note: Circles are numbered clockwise

var Infographic = {
	config : {
		fadeOut: 200,
		fadeIn : 500
	},
	
	boxSettings : {
		"default":{ width: 500 },
		"1-1" :		{ width: 805 }
	},
	
	active : 0,

	// unfortunately need to browser sniff 
	// bring back jquery.browser code as a lib
	//  - IE8 fade() looks awful on transparent pngs
	//  - IE10 - no support for 'pointer-events: none', overlay layer not supported

	browserOverlay : null,
	browserIE8 : null,
	
	isIE8 : function() {
		if (Infographic.browserIE8===null) {
			if ($.browser) {
				Infographic.browserIE8 = ($.browser.msie && (parseInt($.browser.version, 10) <= 8) );
			}
		}
		//console.log('browserIE8:'+ Infographic.browserIE8);
		return Infographic.browserIE8;
	},	
	
	supportsOverlay : function() {
		if (Infographic.browserOverlay===null) {
			if ($.browser) {	
				Infographic.browserOverlay = ($.browser.webkit || $.browser.chrome || $.browser.mozilla );
			}
		}		
		//return false;
		return Infographic.browserOverlay;
	},
		
	showOverlay: function(viewNum) {
		if (Infographic.supportsOverlay()) {
			$('#view-overlay').addClass('display-overlay');
			
			if (viewNum > 0) {
				// show the active circle
				$('.circle-overlay').removeClass('circle-overlay-visible');			
				var overlay = $('#circle'+ viewNum +'-overlay');
				overlay.addClass('circle-overlay-visible');
			}		
		}
	},
	
	clearOverlay : function(viewNum) {
		if (Infographic.supportsOverlay()) {
			if (viewNum===0) {
				$('.circle-overlay').removeClass('circle-overlay-visible');	
			}
		}
	},
	
	view : function (viewNum) {
		return( $('#view'+ viewNum) );
	},
		
	show : function (viewNum) {
		// fade out current and fade in new;
		var current = Infographic.view(Infographic.active);
		var shown = Infographic.view(viewNum);
		
		/*
		current.css('display', 'none');
		shown.fadeIn(Infographic.config.fadeIn);
		*/
		
		Infographic.showOverlay(viewNum);
		
		if (!Infographic.isIE8()) {

			current.fadeOut(Infographic.config.fadeOut, function () {
				shown.fadeIn(Infographic.config.fadeIn, function () {
					Infographic.clearOverlay(viewNum);
				});
			});

		} else {
			// no animations for you
			current.css('display', 'none');
			shown.css('display', 'block');
		}

		Infographic.active = viewNum;
		
		// tracking 
		if (_elq) {
			var fauxpath = window.location.pathname + 'circle-'+viewNum;
			_elq.trackEvent(fauxpath);
		}			
	},
	
	click : function(viewNum) {
		if (viewNum===Infographic.active) {
			// return to the default
			Infographic.show(0);
			$('.infographic-cta').css('visibility', 'visible');
		} else {
			Infographic.show(viewNum);
			$('.infographic-cta').css('visibility', 'hidden');
		}
	
	},
	
	lightboxSetting : function(setting, boxName) {
		var val = Infographic.boxSettings['default'][setting];
		if (Infographic.boxSettings[boxName] && Infographic.boxSettings[boxName][setting]) {
			val = Infographic.boxSettings[boxName][setting];
		} 
		return val;
	},
	
	lightbox : function(boxName) {
		var lightbox = $('#infographic-lightbox');
		var lightboxContent =  $('#infographic-lightbox-content');
		var box = $('#box'+ boxName);
		lightboxContent.html( box.html() );
		
		lightbox.css('width', ''+ Infographic.lightboxSetting('width', boxName) +'px' );
	
		lightbox.lightbox_me({
			'centered' : true,
			'closeSelector' : '.close',
			'overlaySpeed' : 100,
			'onClose' : function () {
				Infographic.clearLightbox();
			}
		});
		//	'overlayCSS' : '{background: black, opacity: 0.25}'
		
		// tracking 
		if (_elq) {
			var fauxpath = window.location.pathname + 'circle-'+boxName;
			_elq.trackEvent(fauxpath);
		}				
	},
	
	clearLightbox : function() {
		var lightboxContent =  $('#infographic-lightbox-content');
		lightboxContent.html("");
	},
	
	xxx : function () {}

};


$(function() {

	if ($('#infographic').length > 0) {

		$('.bigcircle').on('click', function(evt) {
			var view = parseInt( $(this).attr('data-view'), 10);
			//console.log(view);
			Infographic.click(view);
		});

		$('.smallcircle').on('click', function(evt) {
			var box = $(this).attr('data-box');
			//console.log(box);
			Infographic.lightbox(box);
		});
		
		$('#suit-overlay').on('click', function(evt) {
			//Infographic.click(0);
		});
		
		$(".infographic-anchor-link").on('click', function(evt) {
				var link = $(this);
				var anchor = link.attr('href');
				if (anchor.substr(0,1)==='#') {
					$('A[name='+ anchor.substr(1)+']').first().ScrollTo();
					evt.preventDefault();
				}					
		});
		
	}

});;/*** menu events ***/
$(function() {

		var menus = ['people', 'together', 'practice', 'click'];
		var selectedMenu = "";		
		
		$('.nav-main-item').each(function() {
			var thisMenu = $(this).prop('id').split('-').pop();
		
			if ($('body').hasClass(thisMenu)) {
				// current section submenu
				selectedMenu = thisMenu;
			}			
			
			$('A', this).on('click', function(evt) {							
				evt.preventDefault(); 
				
				//if ($('.nav-submenu').length > 0) {
					//$('#content-page').addClass('menu-transition');
				//}
								
				clearSubMenus();
				$('#nav-main-'+thisMenu).addClass('nav-main-selected');
				
				//hide current submenu
				var activeSelectedMenu = (selectedMenu!=="" && thisMenu != selectedMenu);
				if (activeSelectedMenu) {
					$('#nav-sub-'+ selectedMenu).addClass('nav-sub-displaynone');
				}
				
				// display the dropdown menu
				if (thisMenu!=selectedMenu) {
					$('#nav-main-'+selectedMenu).addClass('nav-main-selected-inactive');	
	
					$('#nav-sub-'+thisMenu).addClass('nav-submenu');		
					
					if( Modernizr.csstransitions) { 
						// css animation classes
						$('#nav-sub-'+thisMenu).addClass('nav-submenu-animate');
						$('#content-page').addClass('menu-active'); 
					} else {
						// jquery fallback 
						$('#nav-sub-'+thisMenu).animate({height: '150px'}, 
																						{queue:false, ease:'linear', duration:500});
						$('#content-page').css('margin-top', '13px');
						$('#content-page').animate({paddingTop: '0px'}, 
																						{queue:false, ease:'linear', duration:500});
					}
					
				}

			});
		});
		
		$(".nav-sub-close").on('click', function(evt) {
			$('#content-page').removeClass('menu-active');		
			clearSubMenus();
		});
		
		
		/* remove display and animation styles */
		var clearSubMenus = function() {
			$('#content-page').removeClass('menu-active');
			$('#content-page').attr("style", "");
			
			$('.nav-main-item').each(function() {
				$(this).removeClass('nav-main-selected');
				$(this).removeClass('nav-main-selected-inactive');	
			});
			
			$('.nav-sub').each(function() {
				$(this).attr("style", "");
				$(this).removeClass('nav-sub-hidden');
				$(this).removeClass('nav-sub-displaynone');			
				$(this).removeClass('nav-submenu');
				$(this).removeClass('nav-submenu-animate');
			});			
		};

});;/*** news ***/
$(function() {

		$('.news-box-more').on('click', function(evt) {
			var myId = $(this).attr('id');
			var elmName = myId.substr(0, myId.indexOf('-more'));
			showHideTable(elmName, true);
			//IE8 fix
			$('#content-page').addClass('news-content-IE8');
			evt.preventDefault();
		}); 
		
		$('.news-box-close').on('click', function(evt) {
			var myId = $(this).attr('id');
			var elmName = myId.substr(0, myId.indexOf('-close'));
			showHideTable(elmName, false);	
			evt.preventDefault();
		});				
		
		
		var showHideTable = function(elmName, show) {
			var table = $('#'+ elmName + '-table');
			var height = table.outerHeight(true);
			var container = $('#'+ elmName + '-table-container');
			container.css('height', (show ? ''+height+'px' : '0') );
			
			$('#'+ elmName + '-more').css('display', (show ? 'none':'block') );
			$('#'+ elmName + '-close').css('display', (show ? 'block':'none') );

		};
		
		//tracking for news items
		$('.news-table-links A').on('click', function(evt) {
			if(_elq) {
				_elq.trackEvent($(this).attr('href'));
			}
		});

});;/*** our-people ***/
$(function() {

		var featuredExec = 0;
		var totalExecs = 0;
		var execsVisible = false;		
		
		$('#our-people-execs-lead').on('click', function(evt) {
			showHideExecs(!execsVisible);
			evt.preventDefault();
		});
		
		var showHideExecs = function(show) {
			var MAX_HEIGHT = '2000px';
			
			var boxes = $('#our-people-execs-boxes');
			var height = boxes.outerHeight(true);
			//console.log(height);
		
			// using max-height animations because the height is variable. 
			//  - timing difference will likely be offscreen
			$('#our-people-execs-container').css('max-height', (show ? MAX_HEIGHT : '0px') );
			$('#our-people-exec-widget').attr('src', '../../img/'+(show ? 'arrow-blue-down.png':'arrow-blue-up.png') );
			
			execsVisible = show;
		};	
		
		$('#our-people-execs-feature-close').on('click', function(evt) {
			$('#our-people-execs-feature').css('display', 'none');
			evt.preventDefault();					
		});		
		
		$('.our-people-exec').on('click', function(evt) {
		
			if ($(this).attr('id') != "our-people-feature-content") {
				thisExec = parseInt( $(this).attr('id').split('-')[1], 10);	
				featureExec(thisExec, true);	
			}
			evt.preventDefault();			
		});

		$('#our-people-execs-feature-nav-prev').on('click', function(evt) {
			if(featuredExec > 1) {
				featureExec(featuredExec-1, false);
			}
			evt.preventDefault();
		});		
		$('#our-people-execs-feature-nav-next').on('click', function(evt) {
			//console.log(totalExecs);
			if(featuredExec <= totalExecs) {
				featureExec(featuredExec+1, false);
			}
			evt.preventDefault();			
		});
				
		var featureExec = function(execNum, scroll) {
			featuredExec = execNum;
			totalExecs = $('.our-people-exec').length - 1;
			
			$('#our-people-execs-feature-content').html($('#exec-'+ execNum).html());
			$('#our-people-execs-feature').css('display', 'block');
			
			/* nav links */
			$('#our-people-execs-feature-nav-prev').removeClass('feature-nav-disabled');
			$('#our-people-execs-feature-nav-next').removeClass('feature-nav-disabled');				
			if (featuredExec==1) {
				$('#our-people-execs-feature-nav-prev').addClass('feature-nav-disabled');			
			}
			if (featuredExec==totalExecs) {
				$('#our-people-execs-feature-nav-next').addClass('feature-nav-disabled');				
			}
			
			if (scroll) {
				$('#exec-team-anchor').ScrollTo();
			}	
		}	;


});;/*** site functions ***/
$(function() {
		var lightboxEffect = {background: '#000', opacity: "0.85"};

		/* old IE polyfill */
		if (window.PIE) {
				$('.nav-main-item').each(function() {
						PIE.attach(this);
				});
				$('.sidebar-box').each(function() {
						PIE.attach(this);
				});
				/*
				$('.highlight-box').each(function() {
						PIE.attach(this);
				});
				*/				 
		}
		
		// Sidebar box rotator
		var boxCount = $('.sidebar-rotate').length;
		var boxRand = 0;
		if (boxCount > 0) {
			boxRand = Math.floor(Math.random() * boxCount) + 1;
			$('#sidebar-rotate-'+ boxRand).addClass('sidebar-rotate-active');
		}
		boxCount = $('.community-rotate').length;
		if (boxCount > 0) {
			boxRand = Math.floor(Math.random() * boxCount) + 1;
			$('#community-rotate-'+ boxRand).addClass('community-rotate-active');		
		}

		// Videos which appear in a lighbox
		var lightboxVideos = [
			{link: '#home-feature-cta', video: '#home-video'},
			{link: '#home-feature-image', video: '#home-video'},			
			{link: '#how-we-started-hero-image', video: '#how-we-started-video'},								
			{link: '#news-hero-image', video: '#news-video'}						
		];
		//			{link: '#ewealthmanager-hero-image', video: '#ewealthmanager-video'},
		
		var makeVideoClickEvt = function(videoId) {
			return function(evt) {
				evt.preventDefault();	
				var lb = $('#page-lightbox');
				lb.html( $(videoId).html() );
				lb.lightbox_me({
					centered: true,
					overlayCSS: lightboxEffect,
					onClose : closeVideoLightbox
				});	
				// tracking 
				if (_elq) {
					var fauxpath = window.location.pathname + videoId.replace('#', '');
					_elq.trackEvent(fauxpath);
				}	
			};				
		};
		
		var closeVideoLightbox = function() {
			$('#page-lightbox').html('');
		};
		
		for (var i=0;i<lightboxVideos.length;i++) {
			$(lightboxVideos[i].link).on('click', makeVideoClickEvt(lightboxVideos[i].video) );
		}

		
		/*** faq anchor animation ***/

		$(".faq A.anchor-link").each( function() {
			$(this).on('click', function(evt) {
				var link = $(this);
				var anchor = link.attr('href');
				if (anchor.substr(0,1)==='#') {
					$('A[name='+ anchor.substr(1)+']').first().ScrollTo();
					evt.preventDefault();
				}				
			});
		}); 
		
		//*** funds link tracking ***/
		$("#funds-main A").on('click', function(evt) {
			if(_elq) {
				_elq.trackEvent($(this).attr('href'));
			}		
		});
		
		
		/*** lets-talk form placeholder polyfill ***/

		if ($(".lets-talk").length > 0) {

			// test for html5 placeholder		
			var hasPlaceholderSupport = function() {
				var input = document.createElement('input');
				return ('placeholder' in input);
			};		
		
			if (!hasPlaceholderSupport()) {
					var field;
					var field_default;				
			
					$("form input:text").each( function() {
						field = $(this);
						//console.log(field.attr('name'));
						
						field_default = field.attr('placeholder');
						field.val(field_default);
						field.addClass('inactive');
						field.on("focus", function(event) {
							var el = $(this);
							var placeholder = el.attr('placeholder');
							if (el.val()===placeholder) {
								el.val('');
								el.removeClass('inactive');
							}
						});
						field.on("blur", function(event) {
							var el = $(this);
							var placeholder = el.attr('placeholder');
							if (el.val()==='') {
								el.val(placeholder);
								el.addClass('inactive');
							}
						});					
					});	
					
					// clear placeholders on submit
					$("form").submit(function(evt) {
						var fld, valid = true;
						$("input:text", this).each(function() {
							fld = $(this);
							if (fld.attr('placeholder')===fld.val()) {
								fld.val('');
								fld.removeClass('inactive');
								valid = false;
							}
						});
						if (!valid) { evt.preventDefault(); }
						return valid;
					});
							
			}
		
		}

});


/* simple fader used as animation fall back - see home.js */
var Hero = {
	animated : true,
	currentSlide : 1,
	totalSlides : 0,
	timer : null,	
	config : {
		animate: true,
		fade: 500,
		delay: 5000
	},

	displaySlide : function(slide) {
		//console.log("slide:"+slide);
		$('#home-hero-page-'+ Hero.currentSlide).fadeOut(Hero.config.fade, function() {
			$('#home-hero-page-'+ slide).fadeIn(Hero.config.fade);
		});
		Hero.currentSlide = slide;
	},

	displayNextSlide : function() {
	
		var slide = Hero.currentSlide + 1;
		if (slide > Hero.totalSlides) {
			slide = 1;
		}
		Hero.displaySlide(slide);
		Hero.animateNext();
	},

	animateNext: function() {
		if (Hero.animated) {
			Hero.timer = window.setTimeout(function() {
				Hero.displayNextSlide();
			}, Hero.config.delay);
		}
	},

	animationStop : function() {
		window.clearTimeout(Hero.timer);
		Hero.animated = false;
	},

	nextSlide : function() {
		Hero.animationStop();
		Hero.displayNextSlide();
	} ,

	showSlide : function(slide) {
		Hero.animationStop();
		Hero.displaySlide(slide);
	},

	init : function() {
		Hero.totalSlides = $('#home-hero-pages LI').length;
		for (var i=1; i<=Hero.totalSlides; i++) {
			if (i != Hero.currentSlide ) {
				$('#home-hero-page-'+ i).css('display', 'none');
			}
		}
		if (Hero.config.animate) {
			Hero.animateNext();
		}
	},

	xxxx : function() {}
};	
		

