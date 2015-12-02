// Global property to manage filter values
var filterValues = [];

$(document).ready(function () {
	$('.filterbar-genworthdropdownpanel').genworthdropdownpanel({
		dropdownHorizontalAlignment: 'left',
		autoHideDropdownOnClickOut: false,
		autoHideOtherDropdownsOnOpen: false,
		autoHideOtherDropdownsOnOpenSelector: '.filterbar-genworthdropdownpanel',
		autoDropdownWidth: true
	});


	// An interval is used to consolidate multiple change events into a single event
	var filterChangeInterval = 0;

	// Listen for changes to filter items
	$('.filter').bind('filterItemChange', function () {
		var filter = $(this);
		clearInterval(filterChangeInterval);
		filterChangeInterval = setInterval(function () { triggerFilterChange(filter); }, 250);
	});

	// Fire the filterChange event
	function triggerFilterChange(filter) {
		clearInterval(filterChangeInterval);
		filter.trigger('filterChange');
	}

	// TODO: remove temporary preloading when something better is in place
	(new Image()).src = "/Shared/Images/filterbarArrowDown.png";
	(new Image()).src = "/Shared/Images/filterbarButtonOver.png";
	(new Image()).src = "/Shared/Images/filterbarDatepickerCellBackground.png";
	(new Image()).src = "/Shared/Images/filterbarDatepickerCellBackgroundCurrentDay.png";
	(new Image()).src = "/Shared/Images/filterbarDatepickerCellBackgroundToday.png";
	(new Image()).src = "/Shared/Images/filterbarDatepickerNext.png";
	(new Image()).src = "/Shared/Images/filterbarDatepickerPrev.png";
	(new Image()).src = "/Shared/Images/filterbarDropdownButtonBackgroundActive.png";

	(new Image()).src = "/Shared/Images/dropdownpanelDropdownBackground.png";
	(new Image()).src = "/Shared/Images/dropdownpanelDropdownBackgroundTopLeft.png";
	(new Image()).src = "/Shared/Images/dropdownpanelDropdownBackgroundTopRight.png";
	(new Image()).src = "/Shared/Images/dropdownpanelDropdownBackgroundBottomLeft.png";
	(new Image()).src = "/Shared/Images/dropdownpanelDropdownBackgroundBottomRight.png";
});
