function managerSearchInitialize() {

    $("div.manager-results-block table.filter-results-table tbody tr").each(function () {
        var oTr;
        oTr = $(this);
        oTr.click(function (event) {
            managerSearchLoadStrategy(oTr.attr("href"));
            $('div.manager-results-block table.filter-results-table tbody tr').removeClass('hi-lite');
            $(this).addClass('hi-lite');
        });
    });
    $("div.manager-results-block table.filter-results-table tbody tr:first").click();
}

function managerSearchLoadStrategy(strategy) {
    $.getJSON("/services/ManagerSearch.svc/GetStrategy/" + strategy, function (oResult, status) {
        var oTable;
        var sContent;
        var i;
        var j;
        $(".manager-detail .manager-name").text(oResult.Manager);
        $(".manager-detail .strategy-name").text(oResult.Strategy);
        $(".manager-detail div.strategy-information").html(oResult.Information);

        oTable = $(".manager-detail table.investment-style-guide");
        oTable.find("thead, tbody").remove();

        if (oResult.Dimensions.length == 1) {
            sContent = "<tbody>";
            for (i = 0; i < oResult.Dimensions[0].Values.length; i++) {
                sContent += "<tr>";
                sContent += "<td style=\"min-width: 35px;\"";
                if (oResult.Dimensions[0].Values[i].Selected) {
                    sContent += " class=\"selected\"";
                }
                sContent += ">&nbsp;</td>";
                sContent += "<td width=\"*\" class=\"row-label\">" + oResult.Dimensions[0].Values[i].Value + "</td>"
                sContent += "</tr>";
            }
            sContent += "</tbody>";
            oTable.append(sContent);
        } else if (oResult.Dimensions.length > 1) {
            sContent = "<thead><tr>";
            for (i = 0; i < oResult.Dimensions[1].Values.length; i++) {
                sContent += "<th style=\"min-width: 35px;\">" + oResult.Dimensions[1].Values[i].Value + "</th>";
            }
            sContent += "<th width=\"*\"></th></tr></thead>";
            sContent += "<tbody>";
            for (i = 0; i < oResult.Dimensions[0].Values.length; i++) {
                sContent += "<tr>";
                for (j = 0; j < oResult.Dimensions[1].Values.length; j++) {
                    sContent += "<td style=\"min-width: 35px;\"";
                    if (oResult.Dimensions[0].Values[i].Selected && oResult.Dimensions[1].Values[j].Selected) {
                        sContent += " class=\"selected\"";
                    }
                    sContent += ">&nbsp;</td>";
                }
                sContent += "<td width=\"*\" class=\"row-label\">" + oResult.Dimensions[0].Values[i].Value + "</td>"
                sContent += "</tr>";
            }
            sContent += "</tbody>";
            oTable.append(sContent);
        }
    });
}