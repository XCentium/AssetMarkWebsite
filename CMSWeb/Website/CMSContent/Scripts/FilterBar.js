$(document).ready(function () {
	$('.filterbar-genworthdropdownpanel').genworthdropdownpanel({
		dropdownHorizontalAlignment: 'left',
		autoHideDropdownOnClickOut: false,
		autoHideOtherDropdownsOnOpen: false,
		autoHideOtherDropdownsOnOpenSelector: '.filterbar-genworthdropdownpanel',
		autoDropdownWidth: true
	});

	// TODO: remove temporary preloading when something better is in place
(new Image()).src = "/CMSContent/Images/filterbarArrowDown.png";
(new Image()).src = "/CMSContent/Images/filterbarButtonOver.png";
(new Image()).src = "/CMSContent/Images/filterbarDatepickerCellBackground.png";
(new Image()).src = "/CMSContent/Images/filterbarDatepickerCellBackgroundCurrentDay.png";
(new Image()).src = "/CMSContent/Images/filterbarDatepickerCellBackgroundToday.png";
(new Image()).src = "/CMSContent/Images/filterbarDatepickerNext.png";
(new Image()).src = "/CMSContent/Images/filterbarDatepickerPrev.png";
(new Image()).src = "/CMSContent/Images/filterbarDropdownButtonBackgroundActive.png";

(new Image()).src = "/CMSContent/Images/dropdownpanelDropdownBackground.png";
(new Image()).src = "/CMSContent/Images/dropdownpanelDropdownBackgroundTopLeft.png";
(new Image()).src = "/CMSContent/Images/dropdownpanelDropdownBackgroundTopRight.png";
(new Image()).src = "/CMSContent/Images/dropdownpanelDropdownBackgroundBottomLeft.png";
(new Image()).src = "/CMSContent/Images/dropdownpanelDropdownBackgroundBottomRight.png";
});
