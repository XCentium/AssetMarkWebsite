var Genworth = Genworth || {};
Genworth.PracticeManagement = Genworth.PracticeManagement || {};

Genworth.PracticeManagement.Overview = function () {
	Genworth.Carousel.VerticalMenuCarousel();
}

Genworth.PracticeManagement.MenuExpand = function () {
	var verticalTabsMenu = $('#verticalTabsMenu');

	this.init = function () {
		verticalTabsMenu.find('li.hasChildren > a span.icon')
            .each(function () {
            	var arrow = $(this),
                    parent = arrow.closest('.hasChildren'),
                    parentId = parent.attr('id'),
                    hasValue = $.cookie(parentId) === "true";

            	if (hasValue) {
            		parent.addClass('collapsed');
            	}
            })
            .click(function () {
            	var arrow = $(this),
                    parent = arrow.closest('.hasChildren');

            	parent.toggleClass('collapsed');
            	$.cookie(parent.attr('id'), parent.hasClass('collapsed'));
            	return false;
            });
	};

	this.init();
	return this;
};

Genworth.PracticeManagement.PremierConsultant_LevelStatus = function () {
	var Rows = $(".htmlTable-ILSAdvisor > table > tbody > tr");
	var container = $(".PracticeManagement > .body-container");
	Rows.on("click.swapImage", function () {
		$this = $(this);
		url = $this.data("imageurl");
		Rows.removeClass("select");
		$this.addClass("select");
		$("#ILSAdvisorImageSwap").remove();
		container.hide();
		if (url != undefined) {
			container.append($('<img id="ILSAdvisorImageSwap" />').attr("src", url));
			container.show();
		}
	})
	if (Rows.size() == 1) {
		Rows.eq(0).trigger("click.swapImage");
	}
};