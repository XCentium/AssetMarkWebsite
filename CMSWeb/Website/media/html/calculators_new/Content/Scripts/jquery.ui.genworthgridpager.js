/*
* Genworth Grid Pager.
*/
(function ($, undefined) {

	$.widget("ui.genworthgridpager", {
		options: {
			grid: '.ui-jqgrid-btable'
		},

		_create: function () {
			var self = this,
				o = this.options,
				el = this.element;

			el.append(
				'<form action="#">' +
				'<a class="viewall" href="#">View All (<span class="total">0</span>)</a>' +
				'<span class="text">&nbsp;Items per page&nbsp;</span>' +
				'<div class="pagesize-box">' +
				'<select class="pagesize">' +
				'<option>0</option>' +
				'</select>' +
				'</div>' +
				'<span class="text">&nbsp;</span>' +
				'<a href="#" class="prev-arrow" title="Previous page"></a>' +
				'<span class="text">&nbsp;Page&nbsp;</span>' +
				'<input type="text" class="page-input" value="0" />' +
				'<span class="text">&nbsp;of&nbsp;</span>' +
				'<span class="last">0</span>' +
				'<span class="text">&nbsp;</span>' +
				'<a href="#" class="next-arrow" title="Next page"></a>' +
				'</form>'
			).addClass('grid-pager');

			el.find('.viewall').click(function (event) {
				var grid = $(o.grid);

				grid.jqGrid('setGridParam', {
					page: 1,
					lastpage: 1,
					rowNum: 0 // All
				}).trigger('reloadGrid');

				return false;
			});

			el.find('select.pagesize').selectBox()
				.bind('change', function () {
					var grid = $(o.grid), rowNum = parseInt(grid.jqGrid('getGridParam', 'rowNum')), page = parseInt(grid.jqGrid('getGridParam', 'page')),
						records = parseInt(grid.jqGrid('getGridParam', 'records')), newRowNum = parseInt(this.value), newPage = 1, newLastPage = 1;

					if (isNaN(newRowNum))
						newRowNum = 0; // All
					else {
						newPage = Math.round(rowNum * (page - 1) / newRowNum - 0.5) + 1;
						newLastPage = Math.ceil(records / newRowNum);
					}

					grid.jqGrid('setGridParam', {
						page: newPage,
						lastpage: newLastPage,
						rowNum: newRowNum
					}).trigger('reloadGrid');

					return false;
				});

			el.find('.prev-arrow').click(function (event) {
				var grid = $(o.grid), page = parseInt(grid.jqGrid('getGridParam', 'page'));

				if (!isNaN(page) && page > 1) {
					grid.jqGrid('setGridParam', {
						page: page - 1
					}).trigger('reloadGrid');
				}

				return false;
			});

			el.find('.page-input').change(function (event) {
				var grid = $(o.grid), value = parseInt($(this).val()),
					lastpage = parseInt(grid.jqGrid('getGridParam', 'lastpage'));

				if (!isNaN(value) && value > 0 && value <= lastpage) {
					grid.jqGrid('setGridParam', {
						page: value
					}).trigger('reloadGrid');
				}
			}).focus(function (event) {
				$(this).addClass('ui-state-active');
			}).blur(function (event) {
				$(this).removeClass('ui-state-active');
			});

			el.find('.next-arrow').click(function (event) {
				var grid = $(o.grid), page = parseInt(grid.jqGrid('getGridParam', 'page')),
					lastpage = parseInt(grid.jqGrid('getGridParam', 'lastpage'));

				if (!isNaN(page) && page < lastpage) {
					grid.jqGrid('setGridParam', {
						page: page + 1
					}).trigger('reloadGrid');
				}

				return false;
			});
		},

		destroy: function () {
			var self = this,
				o = this.options,
				el = this.element;

			return this;
		},

		_setOption: function (key, value) {
			var self = this,
				o = this.options,
				el = this.element;

			$.Widget.prototype._setOption.apply(this, arguments);

			switch (key) {
				case "grid":
					break;
			}
		},

		updatePager: function (records, rowNum, rowList, page, lastpage) {
			var self = this,
				o = this.options,
				el = this.element;

			if (isNaN(lastpage) || lastpage < 0)
				lastpage = 0;

			if (isNaN(page) || page <= 0)
				page = lastpage = 0;

			if (isNaN(records) || records <= 0)
				records = page = last = 0;

			el.find('.total').text(records);

			var newOptions = new Object();
			for (var index = 0; index < rowList.length; index++)
				newOptions[rowList[index]] = rowList[index];
			el.find('select.pagesize').selectBox('options', newOptions)
				.selectBox('value', rowNum <= 0 ? 'All' : rowNum);

			el.find('.page-input').val(page);
			el.find('.last').text(lastpage);

			if (page <= 1)
				el.find('.prev-arrow').addClass('ui-state-disabled');
			else
				el.find('.prev-arrow').removeClass('ui-state-disabled');

			if (page >= lastpage)
				el.find('.next-arrow').addClass('ui-state-disabled');
			else
				el.find('.next-arrow').removeClass('ui-state-disabled');
		}

	});

} (jQuery));
