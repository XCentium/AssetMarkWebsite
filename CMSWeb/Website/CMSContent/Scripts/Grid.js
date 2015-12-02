var genworthGrid = {
	_allOptionsExtensions: {},

	// call from individual view js to add custom grid options
	extendOptions: function (id, options) {
		var optionsExtensions = this._allOptionsExtensions[id] || [];
		optionsExtensions[optionsExtensions.length] = options;
		this._allOptionsExtensions[id] = optionsExtensions;
	},

	// Called from grid view to initialize grids
	initialize: function (model) {
		var colNames = [], colModel = [], subGridName = [],
			subGridWidth = [], subGridAlign = [], totalRows = [], totalCols = [];

		// reshape view model columns for genworthgrid options
		if (model.Columns) {
			$.each(model.Columns, function (index, column) {
				colNames[index] = column.Label;
				colModel[index] = {
					name: column.Name,
					index: column.Index,
					minWidth: column.MinWidth,
					width: column.Width,
					align: column.Align,
					sortable: column.Sortable,
					customizable: column.Customizable,
					hidden: column.Hidden
				};

				if (column.Total)
					totalCols[totalCols.length] = column.Name;
			});
		}

		// genworthgrid options
		var options = {
			url: model.Url,
			colNames: colNames,
			colModel: colModel,
			defaults: {
				colNames: colNames,
				colModel: colModel
			},
			// !important - all handlers are in arrays so that option extensions push instead of overwrite
			gridInitialized: [function () {
				if (model.RowClickUrl)
					$(this).addClass('ui-jqgrid-clickablerows');

				// make sure genworthGridButtons.customizeConfig() is available - it is defined in the search bar view...
				if (typeof genworthGridButtons !== 'undefined') {
					var ts = this, customizeConfigOptions = new Array(), j = 0;
					$.each(ts.p.colModel, function (index, colModel) {
						if (!colModel.customizable)
							return;

						customizeConfigOptions[j++] = {
							name: ts.p.id + '_column_' + colModel.name,
							label: ts.p.colNames[index],
							checked: !colModel.hidden,
							changeCallback: function (event) {
								if ($(this).is(":checked"))
									$(ts).jqGrid('showCol', colModel.name);
								else
									$(ts).jqGrid('hideCol', colModel.name);
							}
						};
					});

					genworthGridButtons.customizeConfig({
						options: customizeConfigOptions
					});
				}
			} ],
			// !important - all handlers are in arrays so that option extensions push instead of overwrite
			afterInsertRow: [],
			gridComplete: [],
			loadComplete: [],
			subGridLoadComplete: [],
			onRowClick: model.RowClickUrl ? [function (rowid, e) {
				$('<form method="post" action="' + model.RowClickUrl + '"><input type="hidden" name="rowid" value="' + rowid + '" /></form>"')
					.appendTo("body").submit();
			} ] : []
		};

		if (model.Width)
			options.width = model.Width;

		if (model.MinWidth)
			options.minWidth = model.MinWidth;

		if (model.CountLabel)
		    options.countLabel = model.CountLabel;

		if (model.HideCountLabel)
		    options.hideCountLabel = model.HideCountLabel;

		if (model.SortName)
			options.sortname = model.SortName;

		if (model.Subgrid)
			options.subGrid = model.Subgrid;

		if (model.SubgridUrl)
			options.subGridUrl = model.SubgridUrl;

		// reshape view model subgrid columns for genworthgrid options
		if (model.SubgridColumns) {
			$.each(model.SubgridColumns, function (index, column) {
				subGridName[index] = column.Label;
				subGridWidth[index] = column.Width;
				subGridAlign[index] = column.Align;
			});

			options.subGridModel = [{
				name: subGridName,
				width: subGridWidth,
				align: subGridAlign
			}];
		}

		// reshape view model total rows for genworthgrid options
		if (model.TotalRows) {
			$.each(model.TotalRows, function (index, totalRow) {
				totalRows[totalRows.length] = {
					key: totalRow.Name,
					label: totalRow.Label
				};
			});

			options.totalModel = {
				cols: totalCols,
				rows: totalRows
			};
		}

		if (model.Pager)
			options.pager = model.Pager;

		if (model.SortableExclude)
			options.sortableExclude = model.SortableExclude;

		if (model.Selectable)
			options.selectable = model.Selectable;

		// merge option extensions into options
		var optionsExtensions = this._allOptionsExtensions[model.Id];
		if (optionsExtensions) {
			$.each(optionsExtensions, function (index, optionsExtension) {
				$.each(optionsExtension, function (key, optionExtension) {
					if (key == 'colModel' && options.colModel) { // colModel extension special case, attempt to find match by colModel.name
						$.each(optionExtension, function (colModelIndex, colModel) {
							var found = -1;
							for (var i = 0; i < options.colModel.length; i++) {
								if (options.colModel[i].name == colModel.name) {
									found = i;
									break;
								}
							}

							if (found < 0) // match not found, add new column
								options.colModel.push(colModel);
							else // match found, extend
								$.extend(options.colModel[found], colModel);
						});
					} else if ($.isArray(options[key])) {
						if ($.isArray(optionExtension)) // concat arrays
							options[key] = options[key].concat(optionExtension);
						else // add extension to end of option array
							options[key].push(optionExtension);
					} else if ($.isPlainObject(options[key]) && $.isPlainObject(optionExtension)) // extend option
						$.extend(options[key], optionExtension);
					else // overwrite option
						options[key] = optionExtension;
				});
			});
		}

		$('#' + model.Id).genworthgrid(options);
	}
};

// TODO: remove temporary preloading when something better is in place
(new Image()).src = "/CMSContent/Images/gridRowSelectedBackground.png";
(new Image()).src = "/CMSContent/Images/gridRowExpanded.png";
(new Image()).src = "/CMSContent/Images/globals_spinner_yellow.gif";
(new Image()).src = "/CMSContent/Images/gridHeader_arrow_up.png";
(new Image()).src = "/CMSContent/Images/gridHeader_arrow_down.png";
(new Image()).src = "/CMSContent/Images/gridSubgridHeaderBackground.png";
(new Image()).src = "/CMSContent/Images/gridTotalsBackground.png";
(new Image()).src = "/CMSContent/Images/gridTotalsDataBackground.png";
(new Image()).src = "/CMSContent/Images/gridLoader.gif";
