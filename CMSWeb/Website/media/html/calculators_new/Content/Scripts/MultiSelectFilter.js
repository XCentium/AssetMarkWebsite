$(document).ready(function () {

	// Method to update "Results From" text
	var filterbarMultiselectUpdateLabel = function (filter) {
		var _filter = $(filter),
			options = _filter.find('.checkboxOptionContainer .checkbox'),
			checkall = _filter.find('.checkboxAllContainer  .checkboxAll'),
			optionsChecked = options.filter(':checked'),
			checkAll = checkall.filter(':checked'),
			lengthSpan = _filter.find('.ui-genworthdropdownpanel-buttonContent .length');
		if (checkAll.length == 1)
			lengthSpan.text('All');
		else
			lengthSpan.text(optionsChecked.length + ' of ' + options.length);
	};

	// Method to update All checkbox
	var updateAllCheckbox = function (filter) {
		var options = filter.find('.checkboxOptionContainer .checkbox'),
			optionsChecked = options.filter(':checked'), checkboxAll = filter.find('.checkboxAllContainer .checkboxAll');

		if (options.length > 0 && options.length == optionsChecked.length)
			checkboxAll.attr('checked', 'checked').genworthcheckbox('updateState');
		else
			checkboxAll.removeAttr('checked').genworthcheckbox('updateState');
	};

	// Methods to show/hide disabled overlay
	var showOverlay = function (filter) {
		var overlay = filter.find('.filter-overlay'), progressOverlay = filter.find('.filter-progressoverlay');

		overlay.show();
		progressOverlay.show();
	};

	var hideOverlay = function (filter) {
		var overlay = filter.find('.filter-overlay'), progressOverlay = filter.find('.filter-progressoverlay');

		overlay.hide();
		progressOverlay.hide();
	};

	// Initialize the multi-select filters
	$('.filterbar .filterbar-genworthdropdownpanel-multiselect').each(function () {
		var filter = $(this),
			count = 0,
			value = [];

		filter.find('.checkboxContainer .checkbox').each(function () {
			var checkbox = $(this);

			checkbox.genworthcheckbox();

			if (!checkbox.hasClass('checkboxAll') && checkbox.is(':checked')) {
				value.push(checkbox.val());
			}

			checkbox.change(function (event) {
				var $this = $(this);
				if ($this.hasClass('checkboxAll')) {
					if ($this.is(':checked')) {
						filter.find('.checkboxOptionContainer .checkbox:not(:checked)').attr('checked', 'checked').change();
					} else {
						filter.find('.checkboxOptionContainer .checkbox:checked').removeAttr('checked').change();
					}
				} else {
					// Add/remove filter item from value array
					var valueIndex = $.inArray($this.val(), value);
					if ($this.is(':checked') && valueIndex == -1) {
						value.push($this.val());
					} else if ($this.not(':checked') && valueIndex > -1) {
						value.splice(valueIndex, 1);
					}

					filterbarMultiselectUpdateLabel(filter);
					updateAllCheckbox(filter);
					filter.trigger('filterItemChange');
				}
			});
		});

		filterValues[filter.attr('id')] = value;

		filterbarMultiselectUpdateLabel(filter);
		updateAllCheckbox(filter);
	});

	// Reset button handler
	$('.filterbar .resetfilters .button').click(function (event) {
		$('.filterbar .filterbar-genworthdropdownpanel-multiselect').each(function () {
			var filter = $(this);
			filter.find('.checkboxContainer .checkbox:checked')
				.removeAttr('checked').change();

			filter.genworthdropdownpanel('hideDropdown');
			$("label.error").css({ display: "none" });
			$(".dateInput").removeClass("error");
			filter.trigger('filterItemChange');
		});
	});
});
