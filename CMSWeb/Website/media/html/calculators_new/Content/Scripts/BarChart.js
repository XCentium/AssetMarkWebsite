
function createBarChart(centerX, centerY, width, height,values,labels) {
	data1 = [[ 6.3, 6.2, 5.35, 2.7, 45, 34], [ 10.3, 11.15, 10.14, 36, 50, 45], [ 32.4, 30.6, 30.9, 30, 65, 34], [ 24.3, 22.2, 18.0, 56, 45, 56], [ 4.2, 3.34, -1.30, 44, 56, 16]];
	colors = [ "#9E9EA2", "#009FD7", "#024B89", "#00243F"];
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


