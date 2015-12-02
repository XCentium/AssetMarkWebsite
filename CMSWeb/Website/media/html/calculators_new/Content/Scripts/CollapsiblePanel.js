$(function () {
	/*
	$(".collapsiblePanel").wijexpander({ expanded: false, beforecollapse: hidePanelButtons, beforeexpand: showPanelButtons })
		.siblings(".headerButtons").hide();
	*/
});

function hidePanelButtons() {
	$(this).siblings(".headerButtons").fadeOut(250);
}

function showPanelButtons() {
	$(this).siblings(".headerButtons").fadeIn(250)
}

jQuery(function ($) {
    $('ul.collapsible > li > h6').click(function () {
        var e = this;
        var jq = $(e);

        var jqLi = jq.closest('li');
        jqLi.toggleClass('expanded');

        var jqDiv = $('div', jqLi[0]);
        jqDiv.slideToggle();
    });
});