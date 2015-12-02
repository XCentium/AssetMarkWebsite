
$(document).ready(function () {
	var dateFormat = 'mm/dd/yy';
	var newMaxDate = new Date();
	var nextDay = new Date(new Date().getTime() - (1000 * 60 * 60 * 24));
	var minDate = new Date(2011, 2, 4);
	$('.filterbar .filterbar-genworthdropdownpanel-calendar').each(function () {
		var calendarFilter = $(this), dateInput = calendarFilter.find('.dateInput:first'),
			datePicker = calendarFilter.find('.filterbar-datepicker:first');
		dateInput.focus(function () {
			dateInput.addClass("datefocus");
			});
		dateInput.focusout(function () {
			dateInput.removeClass("datefocus");

		});
		datePicker.datepicker({
			format: dateFormat,
			minDate: minDate,
			maxDate: newMaxDate,
			onSelect: function (dateText, inst) {
				dateInput.val(dateText);
				$("label.error").css({ display: "none" });
				$(".dateInput").removeClass("error");
			}
		});

		dateInput.change(function (event) {
			datePicker.datepicker('setDate', $(this).val());
		}).mousedown(function (event) {
			if (calendarFilter.hasClass('ui-genworthdropdownpanel-dropdownShowing'))
				event.stopPropagation();
		});

		// set initial date on input and fire change event so datepicker updates
		dateInput.val($.datepicker.formatDate(dateFormat, newMaxDate)).change();
	});

	$('.filterbar .resetfilters .button').click(function (event) {
		$('.filterbar .filterbar-genworthdropdownpanel-calendar').each(function () {
			var filter = $(this), dateInput = filter.find('.dateInput:first');
			dateInput.val($.datepicker.formatDate(dateFormat, newMaxDate)).change();
			filter.genworthdropdownpanel('hideDropdown');
		});
	});
});
