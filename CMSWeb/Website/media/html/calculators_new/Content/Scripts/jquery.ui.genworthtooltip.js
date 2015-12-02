/*
 * Genworth tooltip, extends jQuery UI 1.9 tooltip.
 */
(function ($, undefined) {

$.widget("ui.genworthtooltip", $.ui.tooltip, {
	options: {
		position: {
			my: "left bottom",
			at: "center top",
			offset: "-97 0"
		}
	},

	_create: function () {
		var el = this.element;

		$.ui.tooltip.prototype._create.call(this);

		el.hoverIntent($.proxy(this._openIntent, this), $.proxy(this._closeIntent, this));
	},

	open: function (event) {
		if (!event)
			this._openIntent();
	},

	_openIntent: function (event) {
		$.ui.tooltip.prototype.open.call(this, event);
	},

	close: function (event) {
		if (!event)
			this._closeIntent();
	},

	_closeIntent: function (event) {
		$.ui.tooltip.prototype.close.call(this, event);
	},

	_tooltip: function () {
		var tooltip = $.ui.tooltip.prototype._tooltip.call(this);

		// tag tooltip and add skin
		tooltip
			.addClass('ui-genworthtooltip')
			.skinnable({
				name: 'tooltip',
				areas: 4
			});

		return tooltip;
	}
});

} (jQuery));
