$(document).ready(function () {
	var filters = $('.filterbar .filterbar-genworthdropdownpanel-range');

	filters.each(function () {
		var filter = $(this),
			defaultValue = filter.find('.range-values').eq(1);

		// Set default value
		$('input.textRange', filter).val(defaultValue.text());
		defaultValue.addClass('focus');

		$('.range-values', filter).click(function (event) {
			var value = $(this).text();
			$('input.textRange', filter).val(value);
			$(this).siblings().removeClass('focus');
			$(this).addClass('focus');
			filterValues[filter.attr('id')] = value;
			filter.trigger('filterItemChange');
		});
	});

	// Reset
	$('.filterbar .resetfilters .button').click(function (event) {
		filters.each(function () {
			var filter = $(this),
				defaultValue = filter.find('.range-values').eq(1);

			// Clear last selection
			$('.range-values', filter).removeClass('focus');

			// Set default value
			var value = defaultValue.text();
			$('input.textRange', filter).val(value);
			defaultValue.addClass('focus');

			filter.genworthdropdownpanel('hideDropdown');
			filterValues[filter.attr('id')] = value;
			filter.trigger('filterItemChange');
		});
	});
});
