jQuery(function ($) {

	$('.big-list').each(function () {
		var eBigList = this;
		var jqBigList = $(eBigList);
		var jqList = $('ul', eBigList);
		var eList = jqList[0];
		var jqToggler = $('.big-list-toggler', eBigList);
		var jqListItems = $('li', eList);
		var jqListValues = $('.big-list-value', eList);
		var fClickToggler = function () {
			jqBigList.toggleClass('open');
		};
		var fClickValue = function () {
			if (jqBigList.hasClass('open')) {
				var e = this;
				var jq = $(e);
				var jqLi = jq.closest('li');
				jqListItems.removeClass('selected');
				jqLi.addClass('selected');
				jqBigList.removeClass('open');
			}
		};
		jqToggler.click(fClickToggler);
		jqListValues.click(fClickValue);
	});


	$('.dynamic-content-display').each(function () {
		var eContainer = this;
		var jqContainer = $(eContainer);
		var jqNavItems = $('.dynamic-navigation > li');
		var jqContentItems = $('.dynamic-content > li');

		jqNavItems.each(function (index) {
			$(this).attr('index', index);
		});
		jqContentItems.each(function (index) {
			$(this).attr('index', index);
		});

		var fClickItem = function () {
			var e = this;
			var jq = $(e);
			var index = parseInt(jq.attr('index'));
			jqNavItems.removeClass('selected');
			jqContentItems.removeClass('selected');
			jq.addClass('selected');
			$(jqContentItems[index]).addClass('selected');
		};

		jqNavItems.click(fClickItem);
	});

	$('.collapsible-table > thead > tr.collapsible-table-toggler > th.first').each(function () {
		var e = this;
		var jq = $(e);
		var jqTable = jq.closest('.collapsible-table');
		var fClick = function () {
			if (!jqTable.hasClass('collapsible-table-disabled')) {
				if (jqTable.hasClass('collapsible-table-open')) {
					$('.options-table-wrapper table td', jqTable[0]).removeClass('open');
				}
				jqTable.toggleClass('collapsible-table-open');
				jqTable.toggleClass('collapsible-table-closed');

			}
		};
		jq.click(fClick);
	});

});