var genworthGridPager = {
	// called by grid pager view to initialize pagers
	initialize: function (model) {
		$('#' + model.Id).genworthgridpager({
			grid: '#' + model.GridId
		});
	}
};

// TODO: remove temporary preloading when something better is in place
(new Image()).src = "/Shared/Images/gridPagerInputBackgroundOver.png";

(new Image()).src = "/Shared/Images/selectBackgroundOver.png";
(new Image()).src = "/Shared/Images/selectArrowOver.png";
(new Image()).src = "/Shared/Images/selectDropdownTopRight.png";
(new Image()).src = "/Shared/Images/selectDropdownTopLeft.png";
(new Image()).src = "/Shared/Images/selectDropdownBottomLeft.png";
(new Image()).src = "/Shared/Images/selectDropdownBottomRight.png";
(new Image()).src = "/Shared/Images/selectDropdownTopRightOver.png";
(new Image()).src = "/Shared/Images/selectDropdownTopLeftOver.png";
(new Image()).src = "/Shared/Images/selectDropdownBottomLeftOver.png";
(new Image()).src = "/Shared/Images/selectDropdownBottomRightOver.png";
