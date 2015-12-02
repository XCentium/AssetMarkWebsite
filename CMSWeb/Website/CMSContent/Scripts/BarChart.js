
function createBarChart(centerX, centerY, width, height,values,labels) {
	data1 = [[55, 20, 13, 32, 5, 45, 34], [55, 45, 33, 32, 36, 50, 45], [45, 40, 30, 45, 30, 65, 34], [56, 43, 43, 43, 56, 45, 56], [47, 42, 42, 23, 44, 56, 16]];
	colors = ["#F7BA00", "#9E9EA2", "#009FD7", "#024B89", "#00243F"];
	var r = Raphael(centerX,centerY, width, height);
	r.g.barchart(0, 0, 1603, 220, data1, { colors: colors, gutter: "160%" });
	axis = r.g.axis(46, 220, 1100, null, null, 6, 2, ["Year to date", "1 Quater", "1 Year", "2 Years", "3 Years", "5 Years", "7 Years"], "|", 0);
	axis.text.attr({ font: "13px 'Oswald',Arial,Sans-Serif", "font-weight": "regular", "fill": "#333" });
	// show y-axis by setting orientation to 1
	// axis2 = r.g.axis(170, 230, 300, 0, 400, 10, 1);
	$("svg").appendTo($("#barChart"));
	$("svg").appendTo($("#barChart2"));
	$("#barChart svg").css({ position: "relative", top: "", left: "" });
	$("#barChart2 svg").css({ position: "relative", top: "", left: "" });
	$("#barChartWrapper").easySlider({
	});
}


