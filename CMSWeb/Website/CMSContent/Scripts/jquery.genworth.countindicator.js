(function ($) {
	$.widget("ui.countindicator", {
		options: {
			imageSet: "orange"
		},
		_create: function () {
			var self = this;
			var options = self.options;
			var element = self.element;
			element.addClass("countIndicator");
			if (element.text().length > 1) {
				$("<span class=\"countIndicatorCap\"></span>")
					.css({
						backgroundImage: "url(CMSContent/Images/countIndicator_" + options.imageSet + "_left.png)",
						backgroundRepeat: "no-repeat",
						backgroundPosition: "right"
					})
					.insertBefore(element);
				element.css({
				    backgroundImage: "url(CMSContent/Images/countIndicator_" + options.imageSet + "_center.png)",
					backgroundRepeat: "repeat-x",
					minWidth: "0"
				});
				$("<span class=\"countIndicatorCap\"></span>")
					.css({
					    backgroundImage: "url(CMSContent/Images/countIndicator_" + options.imageSet + "_right.png)",
						backgroundRepeat: "no-repeat",
						backgroundPosition: "left"
					})
					.insertAfter(element);
			} else {
				element.css({
				    backgroundImage: "url(CMSContent/Images/countIndicator_" + options.imageSet + ".png)",
					backgroundRepeat: "no-repeat"
				});
			}
		}
	});
})(jQuery);
