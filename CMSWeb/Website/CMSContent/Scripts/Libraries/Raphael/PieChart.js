function createPieChart(elementID, centerX, centerY, radius, labels, values) {
    
    if (!($('#' + elementID).hasClass('hidden')) || $('#' + elementID).css('display') != 'block') {
    
        $('#' + elementID).css('display', 'block');

        var r = Raphael(elementID);
        r.g.txtattr.font = "11px 'Arial', Helvetica, sans-serif";
        var pie = r.g.piechart(centerX, centerY, radius, values, { legend: labels });        
                  
        pie.hover(function () {
            this.sector.stop();
            this.sector.scale(1.1, 1.1, this.cx, this.cy);
            if (this.label) {
                this.label[0].stop();
                this.label[0].scale(1.5);
                this.label[1].attr({ "font-weight": 800 });
            }
        },
        function () {
            this.sector.animate({ scale: [1, 1, this.cx, this.cy] }, 500, "bounce");
            if (this.label) {
                this.label[0].animate({ scale: 1 }, 500, "bounce");
                this.label[1].attr({ "font-weight": 400 });
            }
        });

    }
}