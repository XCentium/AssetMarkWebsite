var Genworth = Genworth || {};
Genworth.Carousel = Genworth.Carousel || {};
Genworth.Carousel.VerticalMenuCarousel = function () {
	this.init = function () {
		var self = this;

		self.carousel_container = $(".carousel_container");
		self.li_list = $('li', '.carousel_menuItems');
		self.pages = $('>.carouselContentItem', self.carousel_container);

		$(">.carouselContentItem", self.carousel_container).hide();
		$("li:eq(0)", ".carousel_menuItems").addClass("active");
		$(">.carouselContentItem:eq(0)", self.carousel_container).addClass("active").show();

		self.li_list.each(function () {

			var li_ele = $(this);
			var pageIndex = self.li_list.index(this);
			var ftr_item = self.pages.eq(pageIndex);
			self.resetFeatureItem(ftr_item);

			var liClickEvent = function () {

				self.stopStartCycle();

				self.pages.each(function () {
					self.resetFeatureItem($(this));
				});

				if (!$(this).parent().hasClass("active")) {

					self.fadeNext(ftr_item, 300);
					li_ele.siblings().removeClass('active');
					li_ele.addClass('active');
				}
				
			};

			$(this).find('a').click(liClickEvent);

			if (ftr_item.hasClass('carouselContentVideoItem')) {
				var imgSlide = ftr_item.find('.imgSlide');

				var imgSlideClickEvent = function () {

					imgSlide.siblings().removeClass('active');
					self.stopStartCycle();

					var feature_video = ftr_item.find('.videoSlide');
					var vid_src = feature_video.find('iframe').attr('src');

					vid_src = vid_src + '?autoplay=1';
					feature_video.find('iframe').attr('src', vid_src);
					imgSlide.fadeOut(300);
					feature_video.show();
				};

				imgSlide.click(imgSlideClickEvent);

				li_ele.find('.videoClicPlay').click(function () {
					setTimeout(imgSlideClickEvent, 500);
					$.proxy(li_ele[0], liClickEvent)();
					return false;
				});
			}

		});

		this.initStartCycle();
	}

	this.fadeNext = function ($next, fadeTime) {
		var self = this;
		var $active = $(">.active", self.carousel_container);
		$active.siblings().hide();
		$active.removeClass('active');
		$next.addClass('active');
		$next.fadeIn(fadeTime);
		$active.fadeOut(fadeTime);
	}

	this.resetFeatureItem = function (ftr_item) {
		ftr_item.find('.imgSlide').show();

		// handle autoplay
		if (ftr_item.hasClass('carouselContentVideoItem')) {
			var feature_video = ftr_item.find('.videoSlide');
			feature_video.hide();
			var ftr_content = feature_video.html();
			feature_video.html('').html(ftr_content.replace('?autoplay=1', ''));
		}
	}


	/* start cycle timer */
	this.start_cycle_duration = 6000;
	this.start_cycle_fade_duration = 1200;
	this.start_cycle_timer;

	this.initStartCycle = function () {
		var self = this;
		start_cycle_timer = setInterval(function () {

			var $active = $(">.active", self.carousel_container);
			var $next = ($active.next().length > 0) ? $active.next() : $(">:eq(0)", self.carousel_container);

			self.fadeNext($next, 1000);

			var pageIndex = self.pages.index($next);
			var li_ele = $(self.li_list).eq(pageIndex);
			li_ele.siblings().removeClass('active');
			li_ele.addClass('active');

		}, this.start_cycle_duration);
	}

	this.stopStartCycle = function () {
		clearInterval(start_cycle_timer);
	}

	this.reset = function () {

	};


	this.init();
	return this;

};