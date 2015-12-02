// Main Binder

function GridInvestmentsInitialize() {

    GetSessionObjectJsonMethod = "GetResearch/";

    $("div.InvestmentFilterBar").each(function () {
        var oFilterDiv;

        oFilterDiv = $(this);
        oFilterDiv.find(".fromdate,.todate").keypress(function (event) {
            if (event.which == 13) {
                event.preventDefault();
                researchPagerSetDateRange(oFilterDiv.find(".fromdate").val(), oFilterDiv.find(".todate").val());
                return false;
            }
        });
        oFilterDiv.find(".page").change(function (event) {
            researchPagerSetDateRange(oFilterDiv.find(".fromdate").val(), oFilterDiv.find(".todate").val());
        });
        oFilterDiv.find(".reset").click(function (event) {
            oFilterDiv.find(".fromdate").val("");
            oFilterDiv.find(".todate").val("");
            researchReset();
        });
    });
}

// Main Refresh
function GridInvestmentsRefresh(oResearch) {
    researchRefreshOptions(oResearch);
    researchRefreshDates(oResearch);
    researchRefreshResults();
}


// Option json calls and Binders
function researchReset() {
    $.getJSON(GridJsonServiceUrl + "Reset/" + ServiceCode, function (oResearch, status) {
        GridRefresh(oResearch)
    });
}

function researchPagerSetDateRange(sDateFrom, sDateTo) {
    if (sDateFrom != null && sDateFrom != "" && sDateTo != null && sDateTo != "")
    $.getJSON(GridJsonServiceUrl + "SetDateRange/" + ServiceCode + "/" + sDateFrom.replace(/\//gi, "-") + "/" + sDateTo.replace(/\//gi, "-"), function (oResearch, status) {
        GridRefresh(oResearch)
    });
}

function researchFilterBarCheckedChanged(cCheckBox, sFilterCode, sOptionCode) {
    $.getJSON(GridJsonServiceUrl + "SetFilterOption/" + ServiceCode + "/" + sFilterCode + "/" + sOptionCode + "/" + (cCheckBox.checked ? "1" : "0"), function (oResearch, status) {
        GridRefresh(oResearch)
    });
}

function researchPagerSetMonths(iMonths) {
    //iMonths = iMonths > 0 ? iMonths : -1;

    var url = GridJsonServiceUrl + "SetDateLookback/" + ServiceCode + "/" + iMonths;

    $.getJSON(url, function (oResearch, status) {
        GridRefresh(oResearch)
    });
}

// Refresh methods
function researchRefreshOptions(oResearch) {
    var f, c, oFilter, o, oOption, a, ac, an, r;
    for (f = 0; f < oResearch.Filters.length; f++) {
        oFilter = oResearch.Filters[f];
        ac = c = 0;
        for (o = 0; o < oFilter.Options.length; o++) {
            oOption = oFilter.Options[o];
            r = oOption.Filtered && oOption.Available;
            c += r ? 1 : 0;

            an = $("#" + oFilter.Code + "-" + oOption.Code);
            if (oOption.Available) {
                ac += 1;
                an.parent().removeClass("closed");
            }
            else
                an.parent().addClass("closed");



            an.attr('disabled', !oOption.Available).attr('checked', r);

        }
        if (ac == 0)
            $("#" + oFilter.Code + "-All").attr('disabled', 1).parent().addClass("closed");
        else
        //		if (ac == oFilter.Options.length)
        //				$("#" + oFilter.Code + "-All").attr('disabled', 0).parent().removeClass("closed");
        //		else 
        {
            a = ac == c ? 1 : 0;
            $("#" + oFilter.Code + "-All").attr('checked', a).attr('disabled', 0).parent().removeClass("closed");
        }


    }
}

function researchRefreshDates(oResearch) {
    $("div.InvestmentFilterBar").each(function () {
        var oFilterDiv;
        var oDefaultDateListItem;
        var jqSiblings;

        oFilterDiv = $(this);

        oFilterDiv.find(".date-options").val(oResearch.Months);
        if (oResearch.Months == -2) {
            oFilterDiv.find(".range").addClass('date-range');
        } else {
            oFilterDiv.find(".range").removeClass('date-range');
        }
        oFilterDiv.find(".fromdate").val(oResearch.FromDate);
        oFilterDiv.find(".todate").val(oResearch.ToDate);

        oDefaultDateListItem = $('li[val=' + oResearch.Months + ']');
        oFilterDiv.find(".months").val(oDefaultDateListItem.text());
        if (oDefaultDateListItem.length != 0) {
            jqSiblings = $('li', oDefaultDateListItem[0].parentNode);
            jqSiblings.removeClass('selected');
            oDefaultDateListItem.addClass("selected");
        }

    });
}

function researchRefreshResults() {
    $.getJSON("/services/InvestmentResearch.svc/GetResults/" + ServiceCode, function (oDocs, status) {
        $("table.InvestmentResearchGrid tbody").each(function () {
            var oTableBody;
            var sRow;
            var sRows = '';
            var i;
            var oDoc;
            var fClick;
            var sPath;
            var sOmnitureAttr = '';

            oTableBody = $(this);
            oTableBody.find("tr").remove();

            fClick = function (s) {
                return function () { window.open(s, "_blank"); };
            };

            for (i = 0; i < oDocs.length; i++) {
                oDoc = oDocs[i];
                sPath = (oDoc.Path.length > 0) ? oDoc.Path : '';

                if (typeof (oDoc.Url) !== 'undefined') {
                    // if item has a URL, use URL instead of document PATH
                    sPath = oDoc.url != '' && oDoc.Url.length > 0 ? oDoc.Url : sPath;
                }

                sPath = (sPath != '') ? ' path="' + sPath + '"' : sPath;

                //Set omniture attribute
                if (typeof (CurrentContextItem) !== 'undefined' && oDoc.OmnitureId != null) {
                    sOmnitureAttr = 'data-omniture=\"CMS:: ' + CurrentContextItem + " :: " + oDoc.OmnitureId + "\" ";
                }
                else if (ServiceCode == 'modelportfolio' && oDoc.OmnitureParam.length > 0)
                {
                    sOmnitureAttr = 'data-omniture=\"' + oDoc.OmnitureParam + "\" ";
                }

                switch (ServiceCode) {
                    case "modelportfolio":
                        sRow = "<tr " + sOmnitureAttr + "href='#' " + sPath + "><td>" + oDoc.Title + "</td><td>" + oDoc.Strategist + "</td><td>" + oDoc.SolutionType + "</td><td>" + oDoc.Custodian + "</td><td>" + oDoc.Date + "</td><td ><a href=\"#\" class=\"icon " + oDoc.Icon + "-icon\">Download</a></td></tr>";
                        break;
                    default:
                        sRow = "<tr " + sOmnitureAttr + "href='#' " + sPath + "><td><img src=\"" + oDoc.Icon + "\"></td><td>" + oDoc.Title + "</td><td>" + oDoc.Category + "</td><td>" + oDoc.Source + "</td><td>" + oDoc.Strategist + "</td><td>" + oDoc.Manager + "</td><td>" + oDoc.AllocationApproach + "</td><td>" + oDoc.Date + "</td></tr>";
                        break;
                }
                sRows += sRow;
            }

            sRows = (oDocs.length == 0) ? '<tr><td colspan="6" align="left">There are no documents available.</td></tr>' : sRows;


            oTableBody.html(sRows);
            $('tr[path]', this).click(function () {
                var param = $(this).attr("data-omniture");
                setOmniture(param);
                window.open($(this).attr("path"), "_blank");
            });

            // Check If Client View.
            try {
                FrameChild.publishHeight("override");
            }
            catch (e) { }
        });
    });
}