// Main Binder
var IsFromSetKeyword = false;

function GridCommonInitialize() {
    $("div.InvestmentResearchPager").each(function () {
        var oPagerDiv;

        oPagerDiv = $(this);

        oPagerDiv.find(".per-page").change(function () {
            researchPagerSetResultsPerPage($(this).val())
        });

        oPagerDiv.find(".page").keypress(function (event) {
            var iVal;
            if (event.which == 13) {
                event.preventDefault();
                iVal = $(this).val();
                if (!isNaN(iVal)) {
                    researchPagerSetPage(iVal - 1);
                }
                return false;
            }
        });
        oPagerDiv.find(".page").change(function (event) {
            var iVal;
            iVal = $(this).val();
            if (!isNaN(iVal)) {
                researchPagerSetPage(iVal - 1);
            }
        });
    });
    
    $("div#KeywordSearchBar").each(function () {
        var oSearchDiv;

        oSearchDiv = $(this);

        oSearchDiv.find(".keywords").keypress(function (event) {
            if (event.which == 13) {
                event.preventDefault();
                var sKeyword = $(this).val();
                sKeyword = $(this).hasClass("editing") ? "" : sKeyword;
                IsFromSetKeyword = true;
                researchPagerSetKeywords(sKeyword);
                return false;
            }
        });
        oSearchDiv.find(".submit").click(function (event) {
            event.preventDefault();
            IsFromSetKeyword = true;
            researchPagerSetKeywords(oSearchDiv.find(".keywords").val())
        });
    });
}

//Main Refresh
function GridCommonRefresh(oResearch) {
    researchRefreshPagers(oResearch);
    researchRefreshSearches(oResearch);
}

// Option json calls and Binders
function researchPagerSetResultsPerPage(iResultsPerPage) {

    switch (ServiceCode.toLowerCase()) {
        case "researchclientview":
        case "research":
            $(".filter-bar-wrapper")[0].researchFilter.SetResultsPerPage(iResultsPerPage);
            break;
        default:
            $.getJSON(GridJsonServiceUrl + "SetResultsPerPage/" + ServiceCode + "/" + iResultsPerPage, function (oResearch, status) {
                GridRefresh(oResearch)
            });
    }
}

function researchPagerSetPage(iPage) {

    switch (ServiceCode.toLowerCase()) {
        case "researchclientview":
        case "research":
            $(".filter-bar-wrapper")[0].researchFilter.SetPage(iPage);
            break;
        default:
            $.getJSON(GridJsonServiceUrl + "SetPage/" + ServiceCode + "/" + iPage, function (oResearch, status) {
                GridRefresh(oResearch)
            });
    }
}

function researchSetSort(sField) {
    switch (ServiceCode.toLowerCase()) {
        case "researchclientview":
        case "research":
            $(".filter-bar-wrapper")[0].researchFilter.SetSort(sField);
            break;
        default:
            $.getJSON(GridJsonServiceUrl + "SetSort/" + ServiceCode + "/" + sField, function (oResearch, status) {
                GridRefresh(oResearch)
            });
    }
}

function researchPagerSetKeywords(sKeywords) {

    $.getJSON(GridJsonServiceUrl + "SetKeywords?Keywords=" + escape(sKeywords) + "&Type=" + ServiceCode, function (oResearch, status) {
		GridRefresh(oResearch)
	});
}

// Refresh Methods
function researchRefreshPagers(oResearch) {
    $("div.InvestmentResearchPager").each(function () {
        var oPagerDiv;
        var iPages;
        var oElement;

        //how many pages are there?
        iPages = Math.max(1, Math.floor(oResearch.ResultCount / oResearch.ResultsPerPage) + ((oResearch.ResultCount % oResearch.ResultsPerPage) == 0 ? 0 : 1));

        oPagerDiv = $(this);
        oPagerDiv.find(" .research-result-count").text(oResearch.ResultCount);
        oPagerDiv.find(".page").val(oResearch.Page + 1);
        oPagerDiv.find(".pages").text(iPages);

        if (oResearch.ResultsPerPage == -1) {
            oElement = oPagerDiv.find(".all,.pager");
            oElement.hide();
        } else {
            oElement = oPagerDiv.find(".all,.pager");
            oElement.show();
            oElement = oPagerDiv.find(".all");
            oElement.unbind();
            oElement.click(function () {
                researchPagerSetResultsPerPage(-1);
                return false;
            });
        }

        oElement = $(".perpage", this);
        oElement.selectBox('value', oResearch.ResultsPerPage);
        //alert(oElement.val());
        //alert(oResearch.ResultsPerPage);
        //$('.InvestmentResearchPager select.perpage option').attr('selected', false);
        //$('.InvestmentResearchPager select.perpage option[value="' + oResearch.ResultsPerPage + '"]').attr('selected', 'selected');

        oElement = oPagerDiv.find(".previous");
        if (oResearch.Page == 0) {
            oElement.hide();
        } else {
            oElement.show();
            oElement.unbind();
            oElement.click(function () {
                researchPagerSetPage(oResearch.Page - 1);
                return false;
            });
        }

        oElement = oPagerDiv.find(".next");
        if (oResearch.Page == (iPages - 1)) {
            oElement.hide();
        } else {
            oElement.show();
            oElement.unbind();
            oElement.click(function () {
                researchPagerSetPage(oResearch.Page + 1);
                return false;
            });
        }
    });
}

function clickRecentSearch(sSearchString) {
    var jqTxt = $('div#KeywordSearchBar input[type="text"]');
    var jqA = $('div#KeywordSearchBar a.submit');
	jqTxt.val(sSearchString).keyup();
	jqA.click();
}

var IsX = false;
function researchRefreshSearches(oResearch) {
    $("div#KeywordSearchBar").each(function () {
        var oSearchDiv;
        var oRecent;
        var i;
        var j;
        var sSearch;
        var c;
        oSearchDiv = $(this);
        oRecent = oSearchDiv.find(".recent-searches");
        oSearchDiv.find('input[type="text"]').keyup();
        oRecent.empty();
        c = oResearch.KeywordSearches.length;
        if ((c > 1 && IsFromSetKeyword) || (c > 0 && !IsFromSetKeyword)) {
            oRecent.append("<label>Recent Searches:&nbsp;</label>");
            if (IsFromSetKeyword && !IsX)
                oSearchDiv.find(".keywords").val(oResearch.KeywordSearches[0]);
            i = IsFromSetKeyword && !IsX ? 1 : 0;
            for (j = 0; i < c && j < 5; i++) {
                sSearch = $.trim(oResearch.KeywordSearches[i]);
                if (sSearch != '') {
                    j++;
                    sSearch = "<a href=\"JavaScript: clickRecentSearch('" + sSearch + "');\">" + sSearch + "</a>";
                    if (j > 1) {
                        sSearch = ", " + sSearch;
                    }
                    oRecent.append(sSearch);
                }
            }
        }
        IsX = IsFromSetKeyword = false;
    });
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}