var genworthGridButtons = {
	// save last customizeConfig for reset defaults
	lastCustomizeConfig: null,
	// customizeConfig { options[] { name, label, checked, changeCallback } }
	customizeConfig: function (config) {
		var customizeOptions = $('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent .customizeOptions');

		customizeOptions.empty();

		$.each(config.options, function (index, option) {
			var customizeOption = $('<div class="customizeCheckboxContainer">' +
                '<input class="customizeCheckbox" type="checkbox" name="' + option.name + '" id="' + option.name + '"' + (option.checked ? ' checked="checked"' : '') + ' /> ' +
                '<label class="customizeCheckboxLabel" for="' + option.name + '">' + option.label + '</label>' +
                '</div>');

			customizeOption.find('.customizeCheckbox')
				.change(option.changeCallback)
				.change(genworthGridButtons._updateAllCheckbox);
			customizeOptions.append(customizeOption);
		});

		$('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent .customizeCheckbox')
        .genworthcheckbox();

		if ($('.customizeOptions .customizeCheckbox').is(':not(:checked)'))
			$('#customizeAllCheckbox').removeAttr('checked').genworthcheckbox('updateState');
		else
			$('#customizeAllCheckbox').attr('checked', 'checked').genworthcheckbox('updateState');

		genworthGridButtons.lastCustomizeConfig = config;
		genworthGridButtons._updateAllCheckbox.call(customizeOptions);
	},
	// Update the checked attribute of the All checkbox, based on the customizeOptions checkboxes
	_updateAllCheckbox: function () {
		var dropdownContent = $(this).closest('.ui-genworthdropdownpanel-dropdownContent');
		if (dropdownContent.find('.customizeOptions .customizeCheckbox:not(:checked)').size() == 0) {
			dropdownContent
				.find('.customizeAllCheckbox')
				.attr('checked', 'checked')
				.genworthcheckbox('updateState');
		} else {
			dropdownContent
				.find('.customizeAllCheckbox')
				.removeAttr('checked')
				.genworthcheckbox('updateState');
		}
	}
};

$(document).ready(function () {
	$('.gridbuttonsBar-genworthdropdownpanel').genworthdropdownpanel({
		dropdownHorizontalAlignment: 'right',
		autoHideOtherDropdownsOnOpenSelector: '.gridbuttonsBar-genworthdropdownpanel'
	});

	$('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent .customizeButton').click(function (event) {
		$('.gridbuttonsBar-genworthdropdownpanel-customize').genworthdropdownpanel('hideDropdown');
	});

	$('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent .customizeCheckbox')
		.genworthcheckbox();

	$('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent .customizeAllReset input.customizeAllCheckbox').change(function (event) {
		if ($(this).is(':checked'))
			$('.customizeOptions .customizeCheckbox:not(:checked)').attr('checked', 'checked').change();
		else
			$('.customizeOptions .customizeCheckbox:checked').removeAttr('checked').change();
	});

	$('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent .customizeAllReset .customizeReset').click(function (event) {
		if (!genworthGridButtons.lastCustomizeConfig)
			return;

		var customizeOptions = $(this).parents('.gridbuttonsBar-genworthdropdownpanel-customize .ui-genworthdropdownpanel-dropdownContent').find('.customizeOptions .customizeCheckbox');
		customizeOptions.each(function (index) {
			var customizeOption = $(this), found = false;
			$.each(genworthGridButtons.lastCustomizeConfig.options, function (index, option) {
				if (option.name == customizeOption.attr('name'))
					found = true;
			});

			if (found)
				customizeOption.attr('checked', 'checked').change();
			else
				customizeOption.removeAttr('checked').change();
		});
	});
});

// TODO: remove temporary preloading when something better is in place
(new Image()).src = "/Shared/Images/searchbarCustomizeButtonBackgroundActive.png";
(new Image()).src = "/Shared/Images/searchbarPrintButtonBackgroundActive.png";
(new Image()).src = "/Shared/Images/searchbarDownloadButtonBackgroundActive.png";
(new Image()).src = "/Shared/Images/searchbarCustomizeDropdownBackground.png";
(new Image()).src = "/Shared/Images/searchbarPrintDropdownBackground.png";
(new Image()).src = "/Shared/Images/searchbarDownloadDropdownBackground.png";
(new Image()).src = "/Shared/Images/searchbarCustomizeHorizontalDiv.png";
(new Image()).src = "/Shared/Images/searchbarCheckbox.png";
(new Image()).src = "/Shared/Images/searchbarCheckboxOver.png";
(new Image()).src = "/Shared/Images/searchbarCheckboxChecked.png";
(new Image()).src = "/Shared/Images/searchbarCheckboxCheckedOver.png";
(new Image()).src = "/Shared/Images/searchbarCustomizeSeparator.png";
(new Image()).src = "/Shared/Images/searchbarCustomizeHorizontalDiv.png";
(new Image()).src = "/Shared/Images/searchbarButton.png";
(new Image()).src = "/Shared/Images/searchbarButtonOver.png";
