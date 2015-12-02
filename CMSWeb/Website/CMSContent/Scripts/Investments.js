
jQuery(function ($) {
	// fix has-layout
	$('.message-block .close, .message-block .open').css('position', 'absolute');

	$('.message-block').each(function () {
		var open = $('.open', this);
		var close = $('.close', this);
		if (open.size() > 0) {
			close.addClass('with-open');
		}
	});

	$('.message-block .close').click(function () {
		var jqClose = $(this);
		var jqContainer = $(this).closest('.message-block');
		var jqOthers = $(this).siblings('.banner, h1, p')

		if (jqClose.hasClass('with-open')) {
			jqOthers.slideUp();
			jqContainer.addClass('closed');
		}
		else {
			jqContainer.slideUp();
		}
	});
	$('.message-block .open').click(function () {
		var jqOpen = $(this);
		var jqContainer = $(this).closest('.message-block');
		var jqOthers = $(this).siblings('.banner, h1, p')

		jqOthers.slideDown();
		jqContainer.removeClass('closed');

	});

	$('#selSolutionType select').selectBox().change(function () {
		window.location = $(this).val();
	});

	$('.filter-results-pagination .inline-form-block select').selectBox();

	$('.split-wrapper .split-col-cell a').each(function () {
		var e;
		var jq, jqParent, jqPeers;
		var fOver, fOut;
		var rel;

		e = this;
		jq = $(e);
		jqParent = jq.closest('.split-col-cell');
		jqPeers = $('a[href="' + jq.attr('href') + '"]', jqParent[0]);

		if (jq.hasClass('foot-link')) {
			// Removed per 16984
			//jq.attr('target', '_blank');
		}
		else {
			// is not foot-link, wire shadowbox
			rel = jq.attr('rel');
			rel = (rel === undefined? 'shadowbox;height=600px;width=962px;' : rel);
			jq.attr('rel', rel);
		}

		fOver = function () {
			jq.addClass('hover');
			//alert(jq.attr('href') != '#');
			if (jq.attr('href') != '#') {
				jqPeers.addClass('hover');
			}
		};
		fOut = function () {
			jqPeers.removeClass('hover');
		};

		jq.mouseover(fOver);
		jq.mouseout(fOut);
	});

	$('.split-wrapper li > .split-col-cell, .info-card-list .info-card').each(function () {
		var e;
		var jq, jqParentUl, jqParentLi;
		var fOver, fOut;

		e = this;
		jq = $(e);
		jqParentUl = jq.closest('ul');
		jqParentLi = jq.closest('li');

		fOver = function () {
			jqParentUl.addClass('hover');
		};
		fOut = function () {
			jqParentUl.removeClass('hover');
		};

		jqParentLi.mouseover(fOver);
		jqParentLi.mouseout(fOut);
	});

	$('.filter-bar-wrapper > ul > li > span').each(function () {
		var e = this;
		var jq = $(e);
		var jqLi = jq.closest('li');
		var jqUl = jq.closest('ul');
		var jqWrapper = jq.closest('.filter-bar-wrapper');

		//		var iDeltaWidth = jqLi.innerWidth() - jq.outerWidth();
		//		//alert(parseInt(jq.css('width').replace('px', '')) + iDeltaWidth);
		//		var iNewWidth = parseInt(jq.css('width').replace('px', '')) + iDeltaWidth;
		//		if (!jqLi.hasClass('date') && !jqLi.hasClass('date-range')) {
		//			//alert(jqLi.attr('class'));
		//			jq.css('width', iNewWidth);
		//		}

		jqUl.addClass('wired');
		$('.filter-bar-list-wrapper', jqLi[0]).css({ 'visibility': 'visible' });


		//var jqSiblings
		var fClick = function () {
			var bIsOn = jqLi.hasClass('open');
			$('ul > li', jqWrapper[0]).removeClass('open');
			jqLi.toggleClass('open', !bIsOn);
		};

		jq.click(fClick);
	});

	$('.options-table-wrapper td > span').each(function () {
		var e = this;
		var jq = $(e);
		var jqTd = jq.closest('td');
		var jqTr = jq.closest('tr');
		var jqWrapper = jq.closest('.options-table-wrapper');

		jqWrapper.addClass('wired');
		$('.filter-bar-list-wrapper', jqTd[0]).css({ 'visibility': 'visible' });


		//var jqSiblings
		var fClick = function () {
			var bIsOn = jqTd.hasClass('open');
			$('td', jqWrapper[0]).removeClass('open');
			jqTd.toggleClass('open', !bIsOn);
		};

		jq.click(fClick);
	});

	$('.filter-bar-wrapper ul > li input[type="checkbox"]').each(function () {
		var e = this;
		var jq = $(e);
		var jqLi = jq.closest('li');

		jqLi.toggleClass('closed', jq.attr('disabled'));

		var fClick = function () {
			jqLi.toggleClass('selected', jq.attr('checked'));
		};

		jq.click(fClick);
	});

	window.CURRENT_FILTER_LINK = null;
	$('.filter-results-table tbody tr[href]').each(function () {
		var e, js;
		var sHref;

		e = this;
		js = $(e);

		sHref = js.attr('href');

		js.click(function () {
			if (window.CURRENT_FILTER_LINK != null) {
				window.location.href = sHref;
				window.CURRENT_FILTER_LINK = null;
			}
		});

	});
	$('.filter-results-table tbody tr a').each(function () {
		$(this).click(function () {
			window.CURRENT_LINK = this;
		});
	});

	$('.filter-bar-list-header input.check-all').each(function () {
		var e = this;
		var jq = $(e);
		var jqLi = jq.closest('li');
		var jqWrapper = jq.closest('.filter-bar-list-wrapper');
		var jqCheckboxes = $('.filter-bar-list-body input[type="checkbox"]', jqWrapper[0]);
		var jqCheckboxList = $('.filter-bar-list-body li', jqWrapper[0]);
		var fClick = function () {

			jqLi.toggleClass('selected', jq.attr('checked'));
			jqCheckboxes.each(function (index) {
				var jqCheckbox = $(this);
				if (!jqCheckbox.attr('disabled')) {
					var jqListItem = $(jqCheckboxList[index]);
					jqCheckbox.attr('checked', (jq.attr('checked') ? 'checked' : false));
					jqListItem.toggleClass('selected', jq.attr('checked'));
				}
			});
		};
		jq.click(fClick);
	});


	wireDatePickers();

	$('.results-search').each(function () {
		var txt = $('input[type="text"]', this);
		var x = $('img.search-clear', this);
		var btn = $('a.submit', this);
		var fClickX = function () {
			IsX = true;
			txt.val('').change();
			btn.click();
			x.css('visibility', 'hidden');
			txt.focus();

		};
		var fFocusTxt = function () {
			fKeyPressTxt();
		};
		var fKeyPressTxt = function (e) {
			if (txt.val() != '') {
				x.css('visibility', 'visible');
			}
			else {
				x.css('visibility', 'hidden');
			}
			//			var code = (e.keyCode ? e.keyCode : e.which);
			//			if (code == 13) {
			//				e.preventDefault();
			//				btn.click();
			//				return false;
			//			}

		};
		txt.focus(fFocusTxt);
		txt.keyup(fKeyPressTxt);
		x.click(fClickX);

		fKeyPressTxt();
		setTimeout(fKeyPressTxt, 500);
	});


	// Synchronize all Pager DDLs
	//	var jqPerPageDDLs = $('.InvestmentResearchPager select.perpage');
	//	jqPerPageDDLs.each(function (index) {
	//		var e = this;
	//		var jq = $(e);
	//		jq.attr('index', index);
	//		jq.change(function () {
	//			alert('change');
	//			$('.InvestmentResearchPager select[index!="' + $(this).attr('index') + '"].perpage').val($(this).val());
	//		});
	//	});

	// Wire <body> click to close filter bar.
	$('.filter-bar-wrapper > ul > li').mousedown(function () {
		window.FILTERBAR_CLICKED = true;
	});
	$('body').mouseup(function () {
		if (window.FILTERBAR_CLICKED != true) {
			$('.filter-bar-wrapper > ul > li.open').removeClass('open');
		}
		window.FILTERBAR_CLICKED = false;
	});

	// Enter Trigger Search
	//	$('.results-search .keywords').each(function () {
	//		var jq = $(this);
	//		var fKeyPress = function (e) {
	//			var code = (e.keyCode ? e.keyCode : e.which);
	//			if (code == 13) {
	//				e.preventDefault();
	//				jq.blur();
	//				jq.siblings('.submit').focus().click();
	//				return false;
	//			}
	//		};
	//	});


});


function wireDatePickers() 
{
	$('.date-picker-calendar[wired!="true"]').each(function () {
		//alert('this one');
		var e, jq, jqAll, jqDays, jqTarget, r;
		e = this;
		jq = $(e);
		jqAll = $('*', e);
		jqTarget = $('#' + jq.attr('target'));
		r = $("div.filter-bar-wrapper");
		//jq.attr('style', 'border: 1px solid #D2D2D2;');
		jqAll.attr('style', '');
		$('.day a, .selected-day a', e).each(function () {
			var a, jqA, td, jqTd, sTitle, fClick, jqWrapper;
			a = this;
			jqA = $(a);
			jqTd = jqA.closest('td');
			sTitle = jqA.attr('title');

			fClick = function () {
				jqTarget.val(sTitle);
				researchPagerSetDateRange(r.find(".fromdate").val(), r.find(".todate").val());
				$('.selected-day', jq[0]).removeClass('selected-day').addClass('day');
				jqTd.removeClass('day').addClass('selected-day');
			};

			jqA.attr('href', 'javascript: retrun false;');
			jqA.click(fClick);

		});

		jq.attr('wired', 'true');


	});
	$('.filter-bar-wrapper > ul > li .date-options li').each(function () {
		var e = this;
		var jq = $(e);
		var jqSiblings = $('li', this.parentNode);
		//alert(jqSiblings.size());
		var jqLi = $(this.parentNode).closest('li');
		//alert(jqLi.size());
		var jqTxt = $('.date-option-value input', jqLi[0]);

		if (jq.hasClass('selected')) {
			//alert("The selected element is:" + jq.attr('value'));
			jqSiblings.removeClass('selected');
			jq.addClass('selected');
		}

		var fClick = function () {
			if (jq.hasClass('custom-date-range')) {
				jqLi.addClass('date-range');
			}
			else {
				jqSiblings.removeClass('selected');
				jq.addClass('selected');
				jqTxt.val(jq.text());
				var iMonths = parseInt(jq.attr('val'));
				//alert(iMonths);
				researchPagerSetMonths(iMonths);
			}
		};

		jq.click(fClick);
	});
}
