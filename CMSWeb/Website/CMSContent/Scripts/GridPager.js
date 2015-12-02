var genworthGridPager = {
	// called by grid pager view to initialize pagers
	initialize: function (model) {
		$('#' + model.Id).genworthgridpager({
			grid: '#' + model.GridId
		});
	}
};

// TODO: remove temporary preloading when something better is in place
(new Image()).src = "/CMSContent/Images/gridPagerInputBackgroundOver.png";

(new Image()).src = "/CMSContent/Images/selectBackgroundOver.png";
(new Image()).src = "/CMSContent/Images/selectArrowOver.png";
(new Image()).src = "/CMSContent/Images/selectDropdownTopRight.png";
(new Image()).src = "/CMSContent/Images/selectDropdownTopLeft.png";
(new Image()).src = "/CMSContent/Images/selectDropdownBottomLeft.png";
(new Image()).src = "/CMSContent/Images/selectDropdownBottomRight.png";
(new Image()).src = "/CMSContent/Images/selectDropdownTopRightOver.png";
(new Image()).src = "/CMSContent/Images/selectDropdownTopLeftOver.png";
(new Image()).src = "/CMSContent/Images/selectDropdownBottomLeftOver.png";
(new Image()).src = "/CMSContent/Images/selectDropdownBottomRightOver.png";
