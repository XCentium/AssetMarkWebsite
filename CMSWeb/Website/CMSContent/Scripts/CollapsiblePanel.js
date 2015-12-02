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
    $('ul.collapsible > li.expanded > div').each(function(){
        $(this).show();
    });
    $('ul.collapsible > li > h6').click(function () {
        var e = this;
        var jq = $(e);

        var jqLi = jq.closest('li');

        var jqDiv = $('div', jqLi[0]);
        if (jqLi.hasClass('expanded')) {
            jqDiv.slideUp();
        }
        else {
            jqDiv.slideDown();
        }

        jqLi.toggleClass('expanded');
    });
});