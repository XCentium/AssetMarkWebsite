/*
 * Genworth Dialog, jQueryUI dialog wrapper.
 */
(function ($, undefined) {

$.widget("ui.genworthdialog", {
	options: {
		width: 450,
		modal: true,
		ok: null,
		save: null,
		cancel: null,
		zIndex: 9999
	},

	_create: function () {
		var self = this,
			o = this.options,
			el = this.element;

		el.dialog({
			autoOpen: false,
			width: o.width,
			modal: o.modal,
			minHeight: 0,
			resizable: false,
			draggable: false,
			zIndex: o.zIndex
		});

		el.dialog('widget').skinnable({
			name: 'dialog'
		});

		this._updateButtons();
	},

	destroy: function () {
		var self = this,
			o = this.options,
			el = this.element;

		el.dialog('widget').skinnable('destroy');
		el.dialog('destroy');

		return this;
	},

	_setOption: function (key, value) {
		var self = this,
			o = this.options,
			el = this.element;

		$.Widget.prototype._setOption.apply(this, arguments);

		switch (key) {
			case 'ok':
			case 'save':
			case 'cancel':
				this._updateButtons();
				break;
		}
	},

	open: function() {
		var self = this,
			o = this.options,
			el = this.element;

		el.dialog('open');
	},

	close: function () {
		var self = this,
			o = this.options,
			el = this.element;

		el.dialog('close');
	},

	isOpen: function () {
		var self = this,
			o = this.options,
			el = this.element;

		return el.dialog('isOpen');
	},

	_updateButtons: function () {
		var self = this,
			o = this.options,
			el = this.element,
			buttons = {};

		if ($.isFunction(o.ok))
			buttons.Ok = o.ok;

		if ($.isFunction(o.save))
			buttons.Save = o.save;

		if ($.isFunction(o.cancel))
			buttons.Cancel = o.cancel;

		el.dialog('option', 'buttons', buttons);
	}
});

} (jQuery));
