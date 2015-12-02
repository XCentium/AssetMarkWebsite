$(document).ready(function () {
	var filterbarMultiselectUpdateLabel = function (filter) {
		var _filter = $(filter),
			options = _filter.find('.checkboxOptionContainer .checkbox'), 
			checkall = _filter.find('.checkboxAllContainer  .checkboxAll'),
			optionsChecked = options.filter(':checked'),
			checkAll = checkall.filter(':checked'), 
			lengthSpan = _filter.find('.ui-genworthdropdownpanel-buttonContent .length');
		if(checkAll.length==1)	
			lengthSpan.text('All');
		else
			lengthSpan.text(optionsChecked.length + ' of ' + options.length);
	};

	var updateAllCheckbox = function (filter) {
		var options = filter.find('.checkboxOptionContainer .checkbox'),
			optionsChecked = options.filter(':checked'), checkboxAll = filter.find('.checkboxAllContainer .checkboxAll');

		if (options.length > 0 && options.length == optionsChecked.length)
			checkboxAll.attr('checked', 'checked').genworthcheckbox('updateState');
		else
			checkboxAll.removeAttr('checked').genworthcheckbox('updateState');
	};

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

	$('.filterbar .filterbar-genworthdropdownpanel-multiselect').each(function () {
		var filter = $(this), count = 0;

		filter.find('.checkboxContainer .checkbox').each(function () {
			var checkbox = $(this);

			checkbox.genworthcheckbox();

			checkbox.change(function (event) {
				if ($(this).hasClass('checkboxAll')) {
					if ($(this).is(':checked'))
						filter.find('.checkboxOptionContainer .checkbox:not(:checked)').attr('checked', 'checked').change();
					else
						filter.find('.checkboxOptionContainer .checkbox:checked').removeAttr('checked').change();
				} else {
					filterbarMultiselectUpdateLabel(filter);
					updateAllCheckbox(filter);
					showOverlay(filter);
					// TODO: replace this with ajax call
					setTimeout(function () {
						hideOverlay(filter);
					}, 1500);
				}
			});
		});

		filterbarMultiselectUpdateLabel(filter);
		updateAllCheckbox(filter);
	});

	$('.filterbar .resetfilters .button').click(function (event) {
		$('.filterbar .filterbar-genworthdropdownpanel-multiselect').each(function () {
			var filter = $(this);
			filter.find('.checkboxContainer .checkbox:checked')
				.removeAttr('checked').change();

			filter.genworthdropdownpanel('hideDropdown');
			$("label.error").css({ display: "none" });
			$(".dateInput").removeClass("error");
		});
	});
});
