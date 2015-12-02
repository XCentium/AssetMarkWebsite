/*
 * Skinnable effect
 *
 * Adds a given number of absolute divs to the selected elements for skinning purposes.  If the element
 * cannot contain children it is first wrapped with an inline div.
 */
(function ($, undefined) {

$.widget("ui.skinnable", {
	options: {
		areas: 5,
		zIndex: -999, // base zIndex, each area after the first will be raised by 1
		name: null
	},

	_create: function () {
		var el = this.element, o = this.options, areaContainer, areasHtml = '', name = o.name;

		if (el[0].nodeName.match(/canvas|textarea|input|select|button|img/i)) {
			if (!name)
				name = el[0].nodeName.toLowerCase();

			areaContainer = el.wrap('<div class="ui-skinnable ui-skinnable-wrapper ui-skinnable-' +
				name + '"></div>').parent();
		} else {
			areaContainer = el.addClass('ui-skinnable' + (name ? ' ui-skinnable-' + name : ''));
		}

		for (var i = 0; i < o.areas; i++) {
			areasHtml += '<div class="ui-skinnable-area' + (i + 1) + ' ui-skinnable-area';

			if (name)
				areasHtml += ' ui-skinnable-' + name + '-area' + (i + 1) + ' ui-skinnable-' + name + '-area';

			areasHtml += '" style="z-index: ' + (o.zIndex + i) + ';"></div>';
		}

		// IE7 float/hasLayout fix: Triggering hasLayout with floats immediately following the absolute skinnable
		// areas causes the areas to vanish unless a static element is placed inbetween.  This fix may apply to older
		// versions of IE as well, but I haven't tested them.
		if ($.browser.msie && parseInt($.browser.version) == 7)
			areasHtml += '<div class="ui-skinnable-iefloatfix" style="width: 0; height: 0; visibility: hidden;"></div>';

		areaContainer.prepend(areasHtml);
	},

	destroy: function () {
		var el = this.element;

		if (el.hasClass('ui-skinnable')) {
			el.removeClass('ui-skinnable').children('.ui-skinnable-area, .ui-skinnable-iefloatfix').remove();
		} else {
			el.siblings('.ui-skinnable-area, .ui-skinnable-iefloatfix').remove();
			el.unwrap();
		}

		return this;
	}
});

} (jQuery));
