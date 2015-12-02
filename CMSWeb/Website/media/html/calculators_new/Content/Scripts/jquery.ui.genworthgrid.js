/*
 * Genworth Grid, jqGrid wrapper.
 */
(function ($, undefined) {

	var _showHideCol = $.fn.jqGrid.showHideCol;
	$.jgrid.extend({
		// grid.base.genworth.js:2809
		showHideCol: function (colname, show) {
			_showHideCol.call(this, colname, show);
			return this.each(function () {
				if (!this.grid) { return; }

				var t = this;

				if ($.isFunction(t.p.onShowHideCol)) { t.p.onShowHideCol.call(this, colname, show); }
			});
		}
	});

	$.widget("ui.genworthgrid", {
		options: {
			url: null,
			subGridUrl: null,
			subGridOptions: null,
			colNames: [],
			colModel: [],
			subGridModel: [{
				name: [],
				width: [],
				align: []
			}],
			subGridOptions : {},
			postData: {},
			totalModel: null,
			pager: null,
			sortableExclude: null,
			gridInitialized: null,
			gridComplete: null,
			loadComplete: null,
			subGridLoadComplete: null,
			rowClassifier: null,
			cellClassifier: null,
			selectable: true,
			hoverrows: true,
			width: null,
			minWidth: null,
			rowList: [50, 150, 250, 'All'],
			onRowClick: null,
			afterInsertRow: null,
			countLabel: 'Records',
			hideCountLabel: false,
			loadonce: false,
			noResultsText: 'There are currently no records found'
		},

		_create: function () {
			var self = this,
				o = this.options,
				el = this.element;

			this._jqGrid = el.get(0);

			el.jqGrid({
				col: { caption: 'DataGridVisibleColoumns', bSubmit: 'Submit', bCancel: 'Cancel' },
				url: o.url,
				datatype: o.datatype,
				jsonReader: o.jsonReader,
				mtype: o.mtype,
				width: o.width,
				height: '100%',
				colNames: o.colNames,
				colModel: o.colModel,
				rowNum: o.rowList && o.rowList.length > 0 ? o.rowList[0] : 0,
				rowList: o.rowList,
				sortname: o.sortname,
				viewrecords: false,
				sortorder: o.sortorder,
				sortable: o.sortable !== false ? {
					exclude: o.sortableExclude,
					update: function (permutation) {
						self._updateGridStyleHints();
						self._updateGridFooter();
					}
				} : false,
				hoverrows: o.hoverrows,
				multiselect: false,
				loadui: 'block',
				loadtext: 'Loading Data',
				noresultsui: 'block',
				noresultstext: o.noResultsText,
				hidegrid: false,
				forceFit: false,
				autowidth: false,
				cellLayout: 0,
				footerrow: o.totalModel && o.totalModel.rows && o.totalModel.rows.length > 0,
				subGrid: o.subGrid,
				shrinkToFit: false,
				subGridUrl: o.subGridUrl,
				subGridModel: o.subGridModel,
				subGridOptions: o.subGridOptions,
				postData: o.postData,
				loadonce: o.loadonce !== null ? o.loadonce : false,
				beforeSelectRow: function (rowid, e) {
					var ts = this;

					if (o.onRowClick) {
						$.each($.isArray(o.onRowClick) ? o.onRowClick : [o.onRowClick], function () {
							if ($.isFunction(this)) {
								this.call(ts, rowid, e);
							}
						});
					}

					return o.selectable == true;
				},
				gridInitialized: function () {
					var ts = this, gview = $('#gview_' + ts.p.id);

					self._updateGridStyleHints();

					// set minWidth on the subgrid expander column
					for (var i = 0; i < ts.p.colModel.length; i++) {
						if (ts.p.colModel[i].name == 'subgrid') {
							if (ts.p.colModel[i].width && !isNaN(ts.p.colModel[i].width))
								ts.p.colModel[i].minWidth = ts.p.colModel[i].maxWidth = ts.p.colModel[i].width;
							break;
						}
					}

					// no results
					$('<div class="ui-widget-overlay jqgrid-overlay" id="nrui_' + ts.p.id + '"></div>').insertBefore(gview);
					$('<div class="noresults ui-state-default ui-state-active" id="noresults_' + ts.p.id + '">' + ts.p.noresultstext + '</div>').insertBefore(gview);

					// footer row labels container
					if (o.totalModel) {
						var footrowLabelsHtml = '<div class="footrow-labels" style="display: none; position: absolute;">';

						if (!o.hideCountLabel)
							footrowLabelsHtml += '<div class="count"><span class="value">' + ts.p.records + '</span>' + (o.countLabel ? ' ' + o.countLabel : '') + '</div>';

						footrowLabelsHtml += '<div class="row-labels">';

						$.each(o.totalModel.rows, function (index, row) {
							footrowLabelsHtml += '<div class="row-label">' + (row.label ? row.label : '') + '</div>';
						});

						footrowLabelsHtml += '</div></div>';

						$(ts.grid.sDiv).append(footrowLabelsHtml);
					}

					if (o.gridInitialized) {
						$.each($.isArray(o.gridInitialized) ? o.gridInitialized : [o.gridInitialized], function () {
							if ($.isFunction(this)) {
								this.call(ts);
							}
						});
					}
				},
				gridComplete: function () {
					var ts = this;

					self._updateGridStyleHints();

					if (o.totalModel && o.totalModel.rows && o.totalModel.cols) {
						// reshape data
						var totals = new Object();
						$.each(ts.p.userData, function (key, value) {
							var keyTokens = key.split('_');

							if (!totals[keyTokens[0]])
								totals[keyTokens[0]] = new Object();

							totals[keyTokens[0]][keyTokens[1]] = value;
						});

						var footerData = new Object();
						$.each(o.totalModel.rows, function (rowIndex, row) {
							$.each(o.totalModel.cols, function (colIndex, col) {
								var value = 0;
								if (row.key && totals[row.key] && totals[row.key][col]) {
									value = totals[row.key][col];
								}

								if (!footerData[col])
									footerData[col] = '';

								footerData[col] += '<div class="' + row.key + ' total rbox" title="' + value + '">' + value + '</div>';
							});
						});

						$(ts).jqGrid(
							'footerData',
							'set',
							footerData,
							false
						);
					}

					self._placeSortIcons();
					self._updateGridFooter();

					if (o.pager)
						$(o.pager).genworthgridpager('updatePager', ts.p.records, ts.p.rowNum, ts.p.rowList, ts.p.page, ts.p.lastpage);

					if (o.gridComplete) {
						$.each($.isArray(o.gridComplete) ? o.gridComplete : [o.gridComplete], function () {
							if ($.isFunction(this)) {
								this.call(ts);
							}
						});
					}

					// ** DON'T REMOVE **  TEMP EXCLUSION OF HEADER WORK.
					/*
					// Wrap the table headers in a div, to enable absolute positioning within
					$('th').each(function () {
					var height = $(this).height();
					$(this).wrapInner('<div style="position: relative; width: 100%; height: ' + height + 'px;" />');
					});
					*/

					// create horizontal scrollbar

					var gridWrapper = el.closest('.ui-jqgrid-view');
					var bdiv = gridWrapper.find('.ui-jqgrid-bdiv');
					bdiv.after('<div class="ui-jqgrid-scroller"></div>');
					var scrollRange = gridWrapper.find('.ui-jqgrid-btable').width() - bdiv.width();
					var scroller = gridWrapper.find('.ui-jqgrid-scroller');

					scroller.genworthscrollbar({
						orientation: "horizontal",
						min: 0,
						max: scrollRange,
						value: 0,
						slide: function (event, ui) {
							bdiv.scrollLeft(ui.value);
						}
					});

					if (scrollRange <= 0)
						scroller.hide();
				},
				beforeRequest: function () {
					var ts = this, gview = $('#gview_' + ts.p.id);

					self._hideNoResults();
					gview.removeClass('ui-jqgrid-noresults').addClass('ui-jqgrid-loading');
				},
				loadComplete: function (data) {
					var ts = this, gview = $('#gview_' + ts.p.id);

					gview.removeClass('ui-jqgrid-loading');

					if (ts.p.records <= 0) {
						gview.addClass('ui-jqgrid-noresults');
						self._showNoResults();
					}

					if (o.loadComplete) {
						$.each($.isArray(o.loadComplete) ? o.loadComplete : [o.loadComplete], function () {
							if ($.isFunction(this)) {
								this.call(ts, data);
							}
						});
					}
				},
				subGridBeforeExpand: function (pID, id) {
					var ts = this, r = $(ts).find('tr#' + id, ts.grid.bDiv), c = r.children('td.ui-sgcollapsed:first');

					if (c.hasClass('sgloading'))
						return false;
					else
						return true;
				},
				subGridLoad: function (sid) {
					var ts = this, subGrid = $('#' + ts.p.id + '_' + sid, ts.grid.bDiv), tr = subGrid.parents('tr.ui-subgrid:first'),
						r = tr.prev('tr'), c = r.children('td.ui-sgcollapsed:first'), subgridData = subGrid.parents('td.subgrid-data:first'),
						subgridCell = subgridData.siblings('td.subgrid-cell:first');

					var colspan = subgridData.attr("colspan");
					if (!colspan || isNaN(colspan) || colspan < 1)
						colspan = 1;
					subgridCell.hide();
					subgridData.attr("colspan", colspan + 1);

					subGrid.hide();
					c.addClass('sgloading');
					r.addClass('ui-sgexpanded');
				},
				subGridLoadComplete: function (sid) {
					var ts = this, subGrid = $('#' + ts.p.id + '_' + sid, ts.grid.bDiv), tr = subGrid.parents('tr.ui-subgrid:first'),
						r = tr.prev('tr'), c = r.children('td.ui-sgcollapsed:first'),
						hCells = subGrid.find('th'), rows = subGrid.find('tr');

					hCells.each(function (index) {
						if (index == 0)
							$(this).addClass('ui-th-first');
						else if (index + 1 >= hCells.length)
							$(this).addClass('ui-th-last');

						if (ts.p.subGridModel[0].align[index])
							$(this).addClass('ui-th-align-' + ts.p.subGridModel[0].align[index]);
					});

					rows.each(function (index) {
						var cells = $(this).find('td');
						cells.each(function (index) {
							if (index == 0)
								$(this).addClass('ui-td-first');
							else if (index + 1 >= cells.length)
								$(this).addClass('ui-td-last');
						});
					});

					if (o.subGridLoadComplete) {
						$.each($.isArray(o.subGridLoadComplete) ? o.subGridLoadComplete : [o.subGridLoadComplete], function () {
							if ($.isFunction(this)) {
								this.call(ts, sid);
							}
						});
					}

					c.removeClass('sgloading');
					subGrid.slideDown('fast');
				},
				loadError: function (xhr, status, error) {
					var ts = this, gview = $('#gview_' + ts.p.id);

					gview.removeClass('ui-jqgrid-loading');

					alert('ERROR: ' + status);
				},
				subGridRowColapsed: function (pID, id) {
					var ts = this, subGrid = $('#' + pID, ts.grid.bDiv), tr = subGrid.parents('tr.ui-subgrid:first'),
						r = tr.prev('tr'), c = r.children('td.ui-sgcollapsed:first');

					if (c.hasClass('sgloading'))
						return false;

					r.removeClass('ui-sgexpanded');

					subGrid.slideUp('fast', function () {
						if (ts.p.subGridOptions.reloadOnExpand === true)
							tr.remove();
					});

					c.html("<a href='javascript:void(0);'><span class='ui-icon " + ts.p.subGridOptions.plusicon + "'></span></a>").removeClass("sgexpanded").addClass("sgcollapsed");

					return false;
				},
				onShowHideCol: function (colname, show) {
					self._updateGridColumnWidths();
					self._updateGridFooter();
					self._saveUserPreferences();
				   },
				afterInsertRow: function (rowId, rowData, rowElement) {
					var ts = this, row = $(ts).find('tr#' + rowId, ts.grid.bDiv), rowClass = null, cellClass = null;

					if ($.isFunction(o.rowClassifier)) {
						rowClass = o.rowClassifier.call(ts, rowId, rowData);
					}

					if (rowClass)
						row.addClass(rowClass);

					if ($.isFunction(o.cellClassifier)) {
						row.children('td').each(function (colNum, cell) {
							cellClass = o.cellClassifier.call(ts, rowId, colNum, ts.p.colModel[colNum], rowData[ts.p.colModel[colNum].name]);
							if (cellClass)
								cell.addClass(cellClass);
						});
					}

					if (o.afterInsertRow) {
						$.each($.isArray(o.afterInsertRow) ? o.afterInsertRow : [o.afterInsertRow], function () {
							if ($.isFunction(this)) {
								this.call(ts, rowId, rowData, rowElement);
							}
						});
					}
				},
				resizeStop: function (nw, idx) {
					self._updateGridColumnWidths(idx);
					self._updateGridFooter();
					self._saveUserPreferences();
				}
			});

			// Override of dragMove, used to enforce a minimum width
			var gridDragMove_orig = this._jqGrid.grid.dragMove;
			this._jqGrid.grid.dragMove = function (x) { // grid.base.genworth.js:706
				if (this.resizing) {
					var diff = x.clientX - this.resizing.startX,
						h = this.headers[this.resizing.idx],
					newWidth = h.width + diff;
					if (newWidth >= self._jqGrid.p.colModel[this.resizing.idx].minWidth) {
						gridDragMove_orig.call(this, x);
					}
				}
			};
		},

		_updateGridColumnWidths: function (offset) {
			var self = this,
				o = this.options,
				el = this.element;

			var lastIndex = -1, totalMinWidth = 0, totalWidth = 0, totalOffsetWidth = 0,
				totalOffsetMinWidth = 0, totalWidthOrg = 0;
			$.each(this._jqGrid.p.colModel, function (index, colModel) {
				if (colModel.hidden)
					return;

				var colWidth = colModel.width;
				if (colWidth.maxWidth && colWidth > colWidth.maxWidth)
					colWidth = colWidth.maxWidth;

				if (colWidth.minWidth && colWidth < colWidth.minWidth)
					colWidth = colWidth.minWidth;

				// TODO: move col min/max constraints up here.
				lastIndex = index;

				if (!isNaN(colModel.width))
					totalWidth += colWidth;

				if (!isNaN(colModel.minWidth))
					totalMinWidth += colModel.minWidth;

				if (index <= offset)
					return;

				if (!isNaN(colModel.width))
					totalOffsetWidth += colWidth;

				if (!isNaN(colModel.minWidth))
					totalOffsetMinWidth += colModel.minWidth;

				if (!isNaN(colModel.widthOrg))
					totalWidthOrg += colModel.widthOrg;
			});

			var newTableWidth = totalWidth, td = o.minWidth - newTableWidth;
			// console.log('newTableWidth', newTableWidth, 'totalWidth', totalWidth, 'td', td, '---------------------');
			$.each(this._jqGrid.p.colModel, function (index, colModel) {
				var header = self._jqGrid.grid.headers[index], col = self._jqGrid.grid.cols[index],
					footer = self._jqGrid.grid.footers[index], newWidth = colModel.width;
				if (colModel.hidden || index <= offset)
					return;

				var colWidth = colModel.width;
				if (colWidth.maxWidth && newWidth > colWidth.maxWidth)
					newWidth = colWidth.maxWidth;

				if (colWidth.minWidth && newWidth < colWidth.minWidth)
					newWidth = colWidth.minWidth;

				// console.log('newColWidth', index, offset, colModel.hidden, newWidth);

				if (td != 0) {
					var d = 0;

					if (td > 0) {
						var w = colModel.widthOrg / totalWidthOrg, wtd = Math.ceil(td * w);

						d = Math.max(1, wtd);

						if (colModel.maxWidth && newWidth + d > colModel.maxWidth) {
							// console.log('maxWidth', newWidth);
							d = colModel.maxWidth - newWidth;
						}
					} else {
						var colSpace = colModel.width - colModel.minWidth,
							offsetSpace = totalOffsetWidth - totalOffsetMinWidth, nw = 0;

						if (colSpace > 0 && offsetSpace > 0)
							nw = colSpace / offsetSpace;

						d = Math.min(-1, Math.ceil(td * nw));

						if (newWidth + d < colModel.minWidth) {
							// console.log('minWidth', newWidth);
							d = colModel.minWidth - newWidth;
						}
					}
					// console.log(' - d', d);

					newWidth += d;
					newTableWidth += d;

					if (index >= lastIndex) {
						if ((newTableWidth < o.minWidth) ||
							(newTableWidth > o.minWidth && newTableWidth - newWidth + colModel.minWidth < o.minWidth)) {
							// console.log('toast', newWidth, newTableWidth, o.minWidth, newTableWidth, (o.minWidth - newTableWidth));
							newWidth += (o.minWidth - newTableWidth);
							newTableWidth += o.minWidth - newTableWidth;
							// console.log('toast', newWidth, newTableWidth);
						}
					}
				}

				// console.log(' + newColWidth', index, newWidth);

				header.width = newWidth;
				colModel.width = newWidth;
				header.el.style.width = newWidth + "px";
				col.style.width = newWidth + "px";
				if (footer) {
					footer.style.width = newWidth + "px";
				}
			});
			// console.log('= newTableWidth', newTableWidth);

			self._jqGrid.p.width = newTableWidth;
			self._jqGrid.p.tblwidth = newTableWidth;
			$('table:first', self._jqGrid.grid.bDiv).css("width", self._jqGrid.p.tblwidth + "px");
			$('table:first', self._jqGrid.grid.hDiv).css("width", self._jqGrid.p.tblwidth + "px");
			self._jqGrid.grid.hDiv.scrollLeft = self._jqGrid.grid.bDiv.scrollLeft;
			if (self._jqGrid.p.footerrow) {
				$('table:first', self._jqGrid.grid.sDiv).css("width", self._jqGrid.p.tblwidth + "px");
				self._jqGrid.grid.sDiv.scrollLeft = self._jqGrid.grid.bDiv.scrollLeft;
			}

			// update horizontal scrollbar

			var gridWrapper = el.closest('.ui-jqgrid-view');
			var scroller = gridWrapper.find('.ui-jqgrid-scroller');
			var scrollRange = gridWrapper.find('.ui-jqgrid-btable').width() - gridWrapper.find('.ui-jqgrid-bdiv').width();

			if (scrollRange > 0) {
				scroller.genworthscrollbar('option', 'max', scrollRange);
				scroller.show();
			}
			else {
				scroller.hide();
			}
		},

		_placeSortIcons: function () {
			var ts = this._jqGrid;

			$('.ui-jqgrid-sortable .s-ico', ts.grid.hDiv).each(function () {
				var sIco = $(this);
				var parent = sIco.parent();
				sIco.detach();
				parent.prepend(sIco);
			});
		},

		_updateGridFooter: function () {
			var self = this, o = this.options, el = this.element, ts = this._jqGrid,
				footrowLabels = $(ts.grid.sDiv).children('.footrow-labels'),
				footrow = $(ts.grid.sDiv).find('.footrow:first'), widthLeft = 0,
				hasTotals = false;

			if (o.totalModel && o.totalModel.rows && o.totalModel.rows.length > 0 &&
					o.totalModel.cols && o.totalModel.cols.length > 0) {
				$.each(ts.p.colModel, function (index, colModel) {
					if (colModel.hidden)
						return;

					if ($.inArray(colModel.name, o.totalModel.cols) > -1) {
						hasTotals = true;
						return false;
					}

					widthLeft += colModel.width;
				});
			}

			if (hasTotals && widthLeft >= 200) {
				footrowLabels.find('.count .value').text(ts.p.records);

				footrowLabels.css({
					left: 0,
					top: 0,
					width: widthLeft,
					height: footrow.height()
				});

				footrowLabels.show();
			} else
				footrowLabels.hide();
		},

		_updateGridStyleHints: function () {
			var self = this,
				o = this.options,
				el = this.element,
				ts = this._jqGrid,
				hcells = $('.ui-jqgrid-htable th', ts.grid.hDiv),
				brows = $('.ui-jqgrid-btable tr.jqgrow', ts.grid.bDiv);

			var sortIndex = -1;
			hcells.each(function (index) {
				var th = $(this);

				if (index == 0)
					th.addClass('ui-th-first');
				else
					th.removeClass('ui-th-first');

				if (index + 1 >= hcells.length)
					th.addClass('ui-th-last');
				else
					th.removeClass('ui-th-last');

				if (ts.p.colModel[index].align)
					th.addClass('ui-th-align-' + ts.p.colModel[index].align);
				else
					th.removeClass('ui-th-align-left ui-th-align-right ui-th-align-center');

				if (th.find('.s-ico:first').is(':visible')) {
					th.addClass('ui-th-sort');
					sortIndex = index;
				} else
					th.removeClass('ui-th-sort');

				th.removeClass('ui-th-beforesort');
			});

			if (sortIndex > 0)
				hcells.eq(sortIndex - 1).addClass('ui-th-beforesort');

			brows.each(function (index) {
				var brcells = $(this).find('td');
				brcells.each(function (index) {
					var td = $(this);

					if (index == 0)
						td.addClass('ui-td-first');
					else
						td.removeClass('ui-td-first');

					if (index + 1 >= brcells.length)
						td.addClass('ui-td-last');
					else
						td.removeClass('ui-td-last');

					if (index == sortIndex)
						td.removeClass('ui-td-notsort').addClass('ui-td-sort');
					else
						td.removeClass('ui-td-sort').addClass('ui-td-notsort');

					td.addClass('ui-td-' + ts.p.colModel[index].name);
				});
			});
		},

		_showNoResults: function () {
			var self = this, o = this.options, el = this.element,
				ts = this._jqGrid;

			switch (ts.p.noresultsui) {
				case "disable":
					break;
				case "enable":
					$("#noresults_" + $.jgrid.jqID(ts.p.id)).show();
					break;
				case "block":
					$("#nrui_" + $.jgrid.jqID(ts.p.id)).show();
					$("#noresults_" + $.jgrid.jqID(ts.p.id)).show();
					break;
			}
		},

		_hideNoResults: function () {
			var self = this, o = this.options, el = this.element,
				ts = this._jqGrid;

			switch (ts.p.loadui) {
				case "disable":
					break;
				case "enable":
					$("#noresults_" + $.jgrid.jqID(ts.p.id)).hide();
					break;
				case "block":
					$("#nrui_" + $.jgrid.jqID(ts.p.id)).hide();
					$("#noresults_" + $.jgrid.jqID(ts.p.id)).hide();
					break;
			}
		},

		destroy: function () {
			var self = this,
				o = this.options,
				el = this.element;

			el.each(function () {
				var id = $(this).attr('id');
				el.jqGrid('GridDestroy', id);
			});

			return this;
		},

		_setOption: function (key, value) {
			var self = this,
				o = this.options,
				el = this.element;

			$.Widget.prototype._setOption.apply(this, arguments);

			switch (key) {
				case "url":
					el.jqGrid('setGridParam', { url: value });
					break;
				case "subGridUrl":
					el.jqGrid('setGridParam', { subGridUrl: value });
					break;
				case "colNames":
					el.jqGrid('setGridParam', { colNames: value });
					break;
				case "colModel":
					el.jqGrid('setGridParam', { colModel: value });
					break;
				case "subGridModel":
					el.jqGrid('setGridParam', { subGridModel: value });
					break;
				case "pager":
					break;
				case "sortableExclude":
					el.jqGrid('setGridParam', {
						sortable: {
							exclude: value
						}
					});
					break;
			}
		},

		reset: function () {
			var self = this, o = this.options, el = this.element;

			if (typeof (o.defaults) != 'undefined') {
				o.colNames = o.defaults.colNames;
				o.colModel = o.defaults.colModel;
			}
			el.GridUnload();
			$('#' + el.attr('id')).genworthgrid(options);

			self._updateGridFooter();
		},

		_saveUserPreferences: function () {
			var self = this, o = this.options, el = this.element,
				ts = this._jqGrid, preferences = {};

			$.each(ts.p.colModel, function (index, colModel) {
				preferences['grid_' + ts.p.id + '_column_' + colModel.name + '_width'] = colModel.width;
				preferences['grid_' + ts.p.id + '_column_' + colModel.name + '_hidden'] = colModel.hidden;
			});

			genworthUserPreferences && genworthUserPreferences.saveUserPreferences(preferences);
		}
	});

} (jQuery));
