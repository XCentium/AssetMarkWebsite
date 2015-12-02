(function ($) {
	"use strict";
	$.extend($.wijmo.wijgrid, {
		loadColumnList: function (targetContainer) {
			alert("check");
			/*
			//if a string is passed process it as a selector
			if (typeof (targetContainer) === "string") {
				targetContainer = $(targetContainer).get(0);
			}

			var grid = $(this);
			var columns = this.option["columns"];
			var checkBox;
			var isChecked;

			$.each(columns, function (index, col) {
				isChecked = (col.visible) ? "checked = 'checked'" : "";
				//checkBox = $("<label><input type='checkbox' " + isChecked + " />" + col.headerText + "</label>" + delimiter);
				checkBox = $("<div class=\"customizeCheckboxContainer\"><label><input type=\"checkbox\" class=\"customizeCheckbox\" " + isChecked + "/> " + col.headerText + "</label></div>");
				targetContainer.append(checkBox);
				checkBox.click(function (e) {
					columns[index].visible = $(this).children("input")[0].checked;
					this.doRefresh();
				})
			})
			*/
		}
	});
})(jQuery);

$(function () {
	$(".advancedGrid").bind("wijgridcolumndropping", columnDroppingHandler);
	$(".advancedGrid").bind("wijgridcolumnresizing", columnResizingHandler);
	$(".advancedGrid").bind("wijgridsorted", sortedHandler);
});

/*
function loadColumnList() {
	var grid = $(".advancedGrid");
	var columns = grid.wijgrid("option", "columns");
	var listContainer = $(".advancedGridContainer .columnsList");
	var checkBox;
	var isChecked;

	$.each(columns, function (index, col) {
		isChecked = (col.visible) ? "checked = 'checked'" : "";
		checkBox = $("<label><input type='checkbox' " + isChecked + " />" + col.headerText + "</label>");
		listContainer.append(checkBox);
		checkBox.click(function (e) {
			columns[index].visible = $(this).children("input")[0].checked;
			grid.wijgrid("doRefresh");
		})
	})
}
*/

// Function to check column drop location, since wijmo only checks the column grabbed.
function columnDroppingHandler(e, args) {
	var dropAllowed = true;
	var dropIndex = args.drop.owner.options.columns.indexOf(args.drop);
	switch (args.at.toLowerCase()) {
		case "left":
			if (dropIndex == 0) {
				dropAllowed = args.drop.allowMoving;
			} else {
				dropAllowed = args.drop.allowMoving || args.drop.owner.options.columns[dropIndex].allowMoving;
			}
			break;
		case "right":
			if (dropIndex == args.drop.owner.options.columns.length - 1) {
				dropAllowed = args.drop.allowMoving;
			} else {
				dropAllowed = args.drop.allowMoving || args.drop.owner.options.columns[dropIndex + 1].allowMoving;
			}
			break;
		case "center":
		default:
			dropAllowed = false;
			break;
	}
	if (!dropAllowed) {
		e.preventDefault();
		alert("Columns cannot be dropped at this position.");
	}
}

// Function to enforce a minimum column size.
function columnResizingHandler(e, args) {
	if (args.newWidth <= 75) {
		e.preventDefault();
		alert("Column size is too small.");
	}
}

// Function to change the selected column background
function sortedHandler(e, args) {
	// TODO: Implement sort handler
}
