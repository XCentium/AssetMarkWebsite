/*
* Genworth dropdown panel jQuery UI component
*
* eric.degroot@cynergysystems.com
*/
(function ($, undefined) {

	$.widget('ui.genworthdropdownpanel', {
		widgetEventPrefix: 'drop',

		options: {
			dropdownHorizontalAlignment: 'left',
			autoHideDropdownOnClickOut: true,
			autoHideOtherDropdownsOnOpen: true,
			autoHideOtherDropdownsOnOpenSelector: '.ui-genworthdropdownpanel',
			autoDropdownWidth: false,
			highZIndex: 3000,
			lowZIndex: 1000
		},

		_create: function () {
			var self = this,
		o = this.options,
		el = this.element;

			var buttonContent = this._buttonContent = el.find('.ui-genworthdropdownpanel-buttonContent');
			var dropdownContent = this._dropdownContent = el.find('.ui-genworthdropdownpanel-dropdownContent');

			if (!buttonContent || !dropdownContent || buttonContent.length <= 0 || dropdownContent.length <= 0)
				return;

			buttonContent.wrap('<div class="ui-genworthdropdownpanel-button" />');
			var button = this._button = el.find('.ui-genworthdropdownpanel-button')
			.bind('mousedown.genworthdropdownpanel', function (event) {
				if (el.hasClass('ui-genworthdropdownpanel-dropdownShowing')) {
					self.hideDropdown();
				} else {
					event.stopPropagation();
					self.showDropdown();
				}
			});

			dropdownContent.wrap('<div class="ui-genworthdropdownpanel-dropdown ui-state-default ui-widget-content ui-corner-all"></div>');
			var dropdown = this._dropdown = el.find('.ui-genworthdropdownpanel-dropdown')
			.append('<div class="ui-genworthdropdownpanel-dropdownbackground ui-genworthdropdownpanel-dropdownbackground1"></div>' +
				'<div class="ui-genworthdropdownpanel-dropdownbackground ui-genworthdropdownpanel-dropdownbackground2"></div>' +
				'<div class="ui-genworthdropdownpanel-dropdownbackground ui-genworthdropdownpanel-dropdownbackground3"></div>' +
				'<div class="ui-genworthdropdownpanel-dropdownbackground ui-genworthdropdownpanel-dropdownbackground4"></div>' +
				'<div class="ui-genworthdropdownpanel-dropdownbackground ui-genworthdropdownpanel-dropdownbackground5"></div>');

			el.addClass('ui-genworthdropdownpanel ui-widget ui-state-default ui-widget-content ui-corner-all' +
				(o.disabled ? ' ui-genworthdropdownpanel-disabled ui-disabled' : ''))
			.bind('mouseenter.genworthdropdownpanel', function (event) {
				el.addClass('ui-genworthdropdownpanel-state-hover ui-state-hover');
			})
			.bind('mouseleave.genworthdropdownpanel', function (event) {
				el.removeClass('ui-genworthdropdownpanel-state-hover ui-state-hover');
			});

			/* focus behaves wildly in ie...needs workaround */
			/* .bind('focus.genworthdropdownpanel', function (event) {
			if (this !== document.activeElement)
			$(document.activeElement).blur();

			if (!el.hasClass('ui-state-active'))
			el.addClass('ui-state-active');

			if (!dropdown.hasClass('ui-state-active'))
			dropdown.addClass('ui-state-active');
				
			self._highZIndex();
			})
			.bind('blur.genworthdropdownpanel', function (event) {
			if (el.hasClass('ui-state-active'))
			el.removeClass('ui-state-active');

			if (dropdown.hasClass('ui-state-active'))
			dropdown.removeClass('ui-state-active');

			self._lowZIndex();

			if (el.hasClass('ui-genworthdropdownpanel-dropdownShowing'))
			self._hideDropdowns(true);                
			});*/

			this._disableSelection(el);
		},

		destroy: function () {
			var el = this.element, buttonContent = this._buttonContent, dropdownContent = this._dropdownContent,
			button = this._button, dropdown = this._dropdown;

			this._enableSelection(el);

			dropdownContent.unwrap();
			button.unbind('mousedown.genworthdropdownpanel');
			buttonContent.unwrap();

			el.removeClass('ui-genworthdropdownpanel ui-genworthdropdownpanel-disabled' +
			' ui-widget ui-widget-content ui-corner-all')
		.unbind('mouseleave.genworthdropdownpanel')
		.unbind('mouseenter.genworthdropdownpanel');

			return this;
		},

		_setOption: function (key, value) {
			$.Widget.prototype._setOption.apply(this, arguments);

			switch (key) {
				case 'dropdownHorizontalAlignment':
					break;
			}
		},

		showDropdown: function () {
			var self = this,
		o = this.options,
		el = this.element,
		dropdown = this._dropdown,
		dropdownContent = this._dropdownContent;

			if (el.hasClass('ui-genworthdropdownpanel-disabled'))
				return false;

			if (o.autoHideOtherDropdownsOnOpen)
				this._hideDropdowns();

			var elBorderTopWidth = parseInt(el.css('borderTopWidth'));
			if (isNaN(elBorderTopWidth))
				elBorderTopWidth = 0;

			var elBorderBottomWidth = parseInt(el.css('borderBottomWidth'));
			if (isNaN(elBorderBottomWidth))
				elBorderBottomWidth = 0;

			var dropdownPaddingTop = parseInt(dropdown.css('paddingTop'));
			if (isNaN(dropdownPaddingTop))
				dropdownPaddingTop = 0;

			var outerHeight = el.outerHeight();
			var top = outerHeight - elBorderTopWidth - elBorderBottomWidth - dropdownPaddingTop;
			dropdown.css('top', top);

			if ((o.dropdownHorizontalAlignment == 'right' || o.dropdownHorizontalAlignment == 'left') && !o.autoDropdownWidth) {
				var dropdownContentPaddingRight = parseInt(dropdownContent.css('paddingRight'));
				if (isNaN(dropdownContentPaddingRight))
					dropdownContentPaddingRight = 0;

				var dropdownContentPaddingLeft = parseInt(dropdownContent.css('paddingLeft'));
				if (isNaN(dropdownContentPaddingLeft))
					dropdownContentPaddingLeft = 0;

				var dropdownContentBorderRightWidth = parseInt(dropdownContent.css('borderRightWidth'));
				if (isNaN(dropdownContentBorderRightWidth))
					dropdownContentBorderRightWidth = 0;

				var dropdownContentBorderLeftWidth = parseInt(dropdownContent.css('borderLeftWidth'));
				if (isNaN(dropdownContentBorderLeftWidth))
					dropdownContentBorderLeftWidth = 0;

				var dropdownContentMarginRight = parseInt(dropdownContent.css('marginRight'));
				if (isNaN(dropdownContentMarginRight))
					dropdownContentMarginRight = 0;

				var dropdownContentMarginLeft = parseInt(dropdownContent.css('marginLeft'));
				if (isNaN(dropdownContentMarginLeft))
					dropdownContentMarginLeft = 0;

				// jQuery outerWidth() isn't including the padding sometimes...
				var dropdownContentOuterWidth = dropdownContentMarginLeft + dropdownContentBorderLeftWidth +
				dropdownContentPaddingLeft + dropdownContent.width() + dropdownContentPaddingRight +
				dropdownContentBorderRightWidth + dropdownContentMarginRight;

				dropdown.css('width', dropdownContentOuterWidth);
			}

			if (o.dropdownHorizontalAlignment == 'right' || o.autoDropdownWidth) {
				var elBorderRightWidth = parseInt(el.css('borderRightWidth'));
				if (isNaN(elBorderRightWidth))
					elBorderRightWidth = 0;

				var elPaddingRight = parseInt(el.css('paddingRight'));
				if (isNaN(elPaddingRight))
					elPaddingRight = 0;

				var dropdownPaddingRight = parseInt(dropdown.css('paddingRight'));
				if (isNaN(dropdownPaddingRight))
					dropdownPaddingRight = 0;

				var right = -(elBorderRightWidth + elPaddingRight + dropdownPaddingRight);
				dropdown.css('right', right);
			}

			if (o.dropdownHorizontalAlignment == 'left' || o.autoDropdownWidth) { // left
				var elBorderLeftWidth = parseInt(el.css('borderLeftWidth'));
				if (isNaN(elBorderLeftWidth))
					elBorderLeftWidth = 0;

				var elPaddingLeft = parseInt(el.css('paddingLeft'));
				if (isNaN(elPaddingLeft))
					elPaddingLeft = 0;

				var dropdownPaddingLeft = parseInt(dropdown.css('paddingLeft'));
				if (isNaN(dropdownPaddingLeft))
					dropdownPaddingLeft = 0;

				var left = -(elBorderLeftWidth + elPaddingLeft + dropdownPaddingLeft);
				dropdown.css('left', left);
			}

			dropdown.animate({ opacity: 'show', height: 'show' }, 'fast');

			el.addClass('ui-genworthdropdownpanel-dropdownShowing');

			if (o.autoHideDropdownOnClickOut) {
				$(document).bind('mousedown.genworthdropdownpanel', function (event) {
					if ($(event.target).parents().andSelf().hasClass('ui-genworthdropdownpanel-dropdown'))
						return;

					self.hideDropdown();
				});
			}
		},

		hideDropdown: function () {
			this._hideDropdowns(true);
		},

		_hideDropdowns: function (selfOnly) {
			var self = this,
		o = this.options,
		el = this.element;

			$(document).unbind('mousedown.genworthdropdownpanel');

			var dropdowns;
			if (selfOnly)
				dropdowns = el.find('.ui-genworthdropdownpanel-dropdown');
			else
				dropdowns = $(o.autoHideOtherDropdownsOnOpenSelector).find('.ui-genworthdropdownpanel-dropdown');

			if (dropdowns.length <= 0)
				return;

			dropdowns.each(function () {
				var dropdown = $(this), el = dropdown.parents('.ui-genworthdropdownpanel:first');

				if (!el || el.length <= 0)
					return;

				el.removeClass('ui-genworthdropdownpanel-dropdownShowing');

				dropdown.animate({ opacity: 'hide', height: 'hide' }, 'fast');
			});
		},

		_disableSelection: function (selector) {
			$(selector).addClass('ui-genworthdropdownpanel-selectiondisabled')
		.bind('selectstart.genworthdropdownpanel', function (event) {
			event.preventDefault();
		});
		},

		_enableSelection: function (selector) {
			$(selector).removeClass('ui-genworthdropdownpanel-selectiondisabled')
		.unbind('selectstart.genworthdropdownpanel');
		},

		_highZIndex: function () {
			var self = this,
		o = this.options,
		el = this.element;

			el.css('z-index', o.highZIndex);
		},

		_lowZIndex: function () {
			var self = this,
		o = this.options,
		el = this.element;

			el.css('z-index', o.lowZIndex);
		}
	});

} (jQuery));
