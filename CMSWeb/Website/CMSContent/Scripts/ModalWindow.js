$(function () {
	//$('.dialog-content').wrap('<div class="dialog-content-wrapper" />');

	function resizeModalWindows() {
		var jqModalWindowBody = $('.ModalWindowBody');
		var jqDialogContent = $('.dialog-content');
		//var jqDialogContentWrapper = $('.dialog-content-wrapper');
		var jqH2 = $('.dialog > h2');

		var iMaxHeight = jqModalWindowBody.height();
		var iH2Heght = jqH2.outerHeight();
		var iPadding = parseInt(String(jqDialogContent.css('paddingTop')).replace('px', '')) + parseInt(String(jqDialogContent.css('paddingBottom')).replace('px', ''));
		var iDelta = iMaxHeight - iH2Heght;
		//alert(iMaxHeight + ' - ' + iH2Heght + ' = ' + iDelta);
		//jqDialogContentWrapper.css('height', iDelta);
		iDelta = iDelta - iPadding;
		jqDialogContent.css('height', iDelta);
	}

	resizeModalWindows();
	$(window).resize(resizeModalWindows);

});