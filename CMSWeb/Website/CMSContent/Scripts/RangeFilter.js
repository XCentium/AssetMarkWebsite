$(document).ready(function () {
	var filters = $('.filterbar .filterbar-genworthdropdownpanel-range');

	filters.each(function () {
		var filter = $(this), defaultValue = filter.find('.range-values').eq(1);

		// Set default value
		$('input.textRange', filter).val(defaultValue.text());
		defaultValue.addClass('focus');

		$('.range-values', filter).click(function (event) {
			$('input.textRange', filter).val($(this).text());
			$(this).siblings().removeClass('focus');
			$(this).addClass('focus');
		});
	});

	// Reset
	$('.filterbar .resetfilters .button').click(function (event) {
		filters.each(function () {
			var filter = $(this), defaultValue = filter.find('.range-values').eq(1);

			// Clear last selection
			$('.range-values', filter).removeClass('focus');

			// Set default value
			$('input.textRange', filter).val(defaultValue.text());
			defaultValue.addClass('focus');

			filter.genworthdropdownpanel('hideDropdown');
		});
	});
});
