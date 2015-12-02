$(document).ready(function () {
	var dateFormat = 'mm/dd/yy';
	var newMaxDate = new Date();
	var nextDay = new Date(new Date().getTime() - (1000 * 60 * 60 * 24));
	var minDate = new Date(2011, 2, 4);

	$('.filterbar .filterbar-genworthdropdownpanel-calendar').each(function () {
		var calendarFilter = $(this),
			filter = calendarFilter.closest('.filter'),
			filterId = filter.attr('id'),
			dateInput = calendarFilter.find('.dateInput:first'),
			datePicker = calendarFilter.find('.filterbar-datepicker:first'),
			prevDate,
			value = "";

		dateInput.focus(function () {
			dateInput.addClass("datefocus");
			prevDate = dateInput.val();
		});

		dateInput.focusout(function () {
			dateInput.removeClass("datefocus");
			if (dateInput.val() != prevDate) {
				value = dateInput.val();
				filterValues[filterId] = value;
				filter.trigger('filterItemChange');
			}
		});

		datePicker.datepicker({
			format: dateFormat,
			minDate: minDate,
			maxDate: newMaxDate,
			onSelect: function (dateText, inst) {
				if (dateInput.val() != dateText) {
					value = dateText;
					filterValues[filterId] = value;
					dateInput.val(value);
					$("label.error").css({ display: "none" });
					$(".dateInput").removeClass("error");
					filter.trigger('filterItemChange');
				}
			}
		});

		dateInput.change(function (event) {
			value = $(this).val();
			filterValues[filterId] = value;
			datePicker.datepicker('setDate', value);
		}).mousedown(function (event) {
			if (calendarFilter.hasClass('ui-genworthdropdownpanel-dropdownShowing'))
				event.stopPropagation();
		});

		// set initial date on input and fire change event so datepicker updates
		value = $.datepicker.formatDate(dateFormat, newMaxDate);
		filterValues[filterId] = value;
		dateInput.val(value).change();
	});

	$('.filterbar .resetfilters .button').click(function (event) {
		$('.filterbar .filterbar-genworthdropdownpanel-calendar').each(function () {
			var calendarFilter = $(this),
				filter = calendarFilter.closest('.filter'),
				filterId = filter.attr('id'),
				dateInput = calendarFilter.find('.dateInput:first');
			value = $.datepicker.formatDate(dateFormat, newMaxDate);
			filterValues[filterId] = value;
			dateInput.val(value).change();
			calendarFilter.genworthdropdownpanel('hideDropdown');
			filter.trigger('filterItemChange');
		});
	});
});
