var Genworth = Genworth || {};
Genworth.Planning = Genworth.Planning || {};

Genworth.Planning.BottomSlidePanel = function () {

    this.init = function () {
        var main = $('#BottomSlidePanel'),
            columns = $('.c4', main);

        columns.each(function () {
            var column = $(this),
                body = $('.body', column);

            column.hoverIntent(function () {
                body.slideDown();
            }, function () {
                body.slideUp();
            });
        });
    }

    this.init();
    return this;
}

Genworth.Planning.ClientExperience = function () {

    // Declare global vars
    this.main;
    this.menuItems;
    this.menuGroupItem;
    this.heros;
    this.autoplayString = '?autoplay=1';

    this.init = function () {
        var self = this,
            next = $('#ui-carousel-next'),
            prev = $('#ui-carousel-prev'),
            main = $('.carousel_horizontal_menu_scroll_bottom').first(),
            menuItems = $('.carouselMenuItem', main),
            menuGroupItem = $('.carouselGroupItem', main),
            heros = $('.carouselItemContainer', main);

        this.main = main;
        this.menuItems = menuItems;
        this.menuGroupItem = menuGroupItem;
        this.heros = heros;

        // Set active items
        menuGroupItem.first().add(heros.first()).addClass('active');

        // Set navigation
        next.click(function () {
            self.scrollMenu(true);
            return false;
        });

        prev.click(function () {
            self.scrollMenu(false);
            return false;
        });

        //Set video behavior
        menuItems.each(function (index) {
            var item = $(this),
                hero = self.heros.eq(index),
                videoItem = hero.find('.carouselContentVideoItem'),
                isVideo = videoItem.length > 0,
                imgSlide = hero.find('.imgSlide'),
                videoSlide = hero.find('.videoSlide');


            if (isVideo) {
                imgSlide.click(function () {
                    self.reproduceVideo(hero);
                });

                var iframe = videoSlide.find('iframe'),
                    src = iframe.attr('src').replace(self.autoplayString, '');

                iframe.attr('original-src', src);
                iframe.attr('src', src);
            }

            item.click(function () {
                self.cleanAutoPlay();
                self.heros.removeClass('active');
                hero.addClass('active');
                self.reproduceVideo(hero);
            });
        });

    }

    this.cleanAutoPlay = function () {
        var self = this;

        self.heros.each(function () {
            var hero = $(this),
                imgSlide = hero.find('.imgSlide'),
                videoSlide = hero.find('.videoSlide'),
                iframe = videoSlide.find('iframe'),
                src = iframe.attr('src'),
                originalSrc = iframe.attr('original-src');

            imgSlide.show();
            videoSlide.hide();
            iframe.attr('src', originalSrc);
        });
    }

    this.reproduceVideo = function (hero) {
        var self = this,
            videoSlide = hero.find('.videoSlide'),
            imgSlide = hero.find('.imgSlide'),
            iframe = videoSlide.find('iframe'),
            src = iframe.attr('original-src');

        src += self.autoplayString;
        iframe.attr('src', src);
        imgSlide.hide();
        videoSlide.show();
    }

    this.scrollMenu = function (next) {
        var active = this.menuGroupItem.filter('.active'),
            item = next ? active.next('.carouselGroupItem') : active.prev('.carouselGroupItem');

        if (item.length > 0) {
            active.removeClass('active');
            item.addClass('active');
        }
    }

    this.init();
    return this;
};

Genworth.Planning.Overview = function () {
    this.main;
    this.menuItems;
    this.heros;
    this.autoplayString = '?autoplay=1';
    this.fadeOn = false;

    this.init = function () {
        var self = this,
            main = $('.carousel_bottom_4square').first(),
            menuItems = $('.carousel_menuItems a', main),
            heros = $('.carouselItemContainer', main);

        this.main = main;
        this.menuItems = menuItems;
        this.heros = heros;

        menuItems.first().add(heros.first()).addClass('active');

        menuItems.click(function () {
            $this = $(this),
            index = menuItems.index(this),
            hero = heros.eq(index);
            self.stopStartCycle();
            self.fadeTo($this);
            return false;
        });

        heros.each(function (index) {
            var hero = $(this),
                imgSlide = hero.find('.imgSlide'),
                videoSlide = hero.find('.videoSlide'),
                iframe = videoSlide.find('iframe'),
                src = iframe.attr('src').replace(self.autoplayString, '');

            iframe.attr('original-src', src);
            iframe.attr('src', src);

            imgSlide.click(function () {
                if (self.fadeOn) return false;
                self.cleanAutoPlay();
                self.stopStartCycle();
                self.reproduceVideo(hero);
            });

        });

        $(window).load(function () {
            self.initStartCycle();
        });
    }

    this.start_cycle_duration = 6000;
    this.start_cycle_timer;

    this.initStartCycle = function () {
        var self = this;
        start_cycle_timer = setInterval(function () {

            var active = self.menuItems.filter('.active'),
                next = (active.next().length > 0) ? active.next() : self.menuItems.first();
            self.fadeTo(next);

        }, self.start_cycle_duration);
    }

    this.stopStartCycle = function () {
        clearInterval(start_cycle_timer);
    }

    this.cleanAutoPlay = function () {
        var self = this;

        self.heros.each(function () {
            var hero = $(this),
                imgSlide = hero.find('.imgSlide'),
                videoSlide = hero.find('.videoSlide'),
                iframe = videoSlide.find('iframe'),
                src = iframe.attr('src'),
                originalSrc = iframe.attr('original-src'),
                isVideoVisible = videoSlide.is(':visible');
            
            if (isVideoVisible) {
                imgSlide.show();
                videoSlide.hide();
                iframe.attr('src', originalSrc);
            }
        });
    }

    this.fadeTo = function (menuNext) {
        var self = this;

        if (menuNext.is('.active') || self.fadeOn) {
            return false;
        }

        self.cleanAutoPlay();

        var index = self.menuItems.index(menuNext),
            heroNext = self.heros.eq(index),
            heroActive = self.heros.filter('.active'),
            menuActive = self.menuItems.filter('.active');

        self.fadeOn = true;
        heroNext.addClass('pre');
        heroActive.fadeOut(300, function () {
            heroActive.removeClass('active').show();
            heroNext.addClass('active').removeClass('pre');
            self.fadeOn = false;
        });

        menuActive.removeClass('active');
        menuNext.addClass('active');

    }

    this.reproduceVideo = function (hero) {
        var self = this,
            videoSlide = hero.find('.videoSlide'),
            imgSlide = hero.find('.imgSlide'),
            iframe = videoSlide.find('iframe'),
            src = iframe.attr('original-src');

        src += self.autoplayString;
        iframe.attr('src', src);
        imgSlide.hide();
        videoSlide.show();
    }

    this.init();
    return this;
}

$(function () {

    var planningBody = $("body.planning");
    if (planningBody.length > 0) {

        if (planningBody.hasClass("overview") || planningBody.hasClass("why") || planningBody.hasClass("getting-started")) {

            var loginAnchor = planningBody.find(".component_list .component_item:eq(0) .ImageLinkControl a");
            loginAnchor.click(function () {
                cancelTimeout();
            });
        }
    }

});

