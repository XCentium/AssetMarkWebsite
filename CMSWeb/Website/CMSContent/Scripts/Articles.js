jQuery(function ($) {
	// Article Search On Enter Keypress.
	$('.article-search .keywords').each(function () {
		var jq = $(this);
		var fKeyPress = function (e) {
			var code = (e.keyCode ? e.keyCode : e.which);
			if (code == 13) {
				e.preventDefault();
				$('#' + jq.attr('target')).click();
				return false;
			}
		};
		jq.keypress(fKeyPress);
	});
});