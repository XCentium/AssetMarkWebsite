/*
 * Genworth Checkbox
 *
 * eric.degroot@cynergysystems.com
 */
(function ($, undefined) {

	$.widget("ui.genworthcheckbox", {
		options: {
			disabled: false
		},

		_create: function () {
			var self = this,
				o = this.options,
				el = this.element;

			this._checkbox = el.get(0);
			if (this._checkbox.tagName.toUpperCase() != 'INPUT') {
				alert("Element is not an input");
				return;
			}

			if (el.attr('type').toLowerCase() != 'checkbox') {
				alert('Input is not a checkbox');
				return;
			}

			if (el.data('genworthcheckbox-control'))
				return;

			var control = $('<a href="javascript:void(0);" class="ui-widget ui-widget-content ui-corner-all ui-state-default ui-genworthcheckbox' +
				(el.is(':checked') ? ' ui-state-checked' : '') + (o.disabled ? ' ui-state-disabled' : '') + '"></a>')
				.mousedown(function (event) {
					if (o.disabled)
						return;

					$(this).addClass('ui-state-active');
				})
				.mouseup(function (event) {
					if (o.disabled)
						return;

					$(this).removeClass('ui-state-active');

					if (el.is(':checked'))
						el.removeAttr('checked').change();
					else
						el.attr('checked', 'checked').change();
				})
				.hover(function (event) {
					$(this).addClass('ui-state-hover');
				}, function (event) {
					$(this).removeClass('ui-state-hover');
				})
				.focus(function (event) {
					$(this).addClass('ui-state-focus');
				})
				.blur(function (event) {
					$(this).removeClass('ui-state-focus');
				})
				.insertAfter(el);

			this._elementChangeHandler = function (event) {
				self.updateState();
			};

			el.data('genworthcheckbox-control', control)
				.change(this._elementChangeHandler)
				.hide();
		},

		updateState: function () {
			var self = this,
				o = this.options,
				el = this.element,
				control = el.data('genworthcheckbox-control');

			if (el.is(':checked'))
				control.addClass('ui-state-checked');
			else
				control.removeClass('ui-state-checked');
		},

		destroy: function () {
			var self = this,
				o = this.options,
				el = this.element,
				control = el.data('genworthcheckbox-control');

			control.remove();
			el.unbind('change', this._elementChangeHandler);
			el.removeData('genworthcheckbox-control').show();

			return this;
		},

		_setOption: function (key, value) {
			var self = this,
				o = this.options,
				el = this.element,
				control = el.data('genworthcheckbox-control');

			$.Widget.prototype._setOption.apply(this, arguments);

			switch (key) {
				case 'disabled':
					if (value)
						control.addClass('ui-state-disabled');
					else
						control.removeClass('ui-state-disabled');
					break;
			}
		}
	});

} (jQuery));
