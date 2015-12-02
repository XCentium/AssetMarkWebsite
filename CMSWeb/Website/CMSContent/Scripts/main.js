var ServiceCode = "";
var GridJsonServiceUrl = "";
var GetSessionObjectJsonMethod = "";
var CurrentContextItem = "";

function GridInitialize(ServiceCodeParam, GridJsonServiceUrlParam, CurrentContextItemParam) {

    var isAutoRefreshEnable = true;
    ServiceCode = ServiceCodeParam;
    GridJsonServiceUrl = GridJsonServiceUrlParam;
    CurrentContextItem = CurrentContextItemParam;

    switch (ServiceCode.toLowerCase())
    {
        case "events-all":
        case "events-inperson":
        case "events-webinar":
        case "events-archive":
            isAutoRefreshEnable = GridEventsInitialize();
            break;
        case "research":
        case "researchclientview":
            isAutoRefreshEnable = false;
            GridInvestmentsInitialize();
            GridInvestmentsRefresh();

            break;
        default:
            GridInvestmentsInitialize();
    }

    GridCommonInitialize();

    if (isAutoRefreshEnable) {
        $.getJSON(GridJsonServiceUrl + GetSessionObjectJsonMethod + ServiceCode, function (oResearch, status) {
            GridRefresh(oResearch)
        });
    }
}

function GridRefresh(sessionObject) {

    switch (ServiceCode.toLowerCase()) 
    {
        case "events-all":
        case "events-inperson":
        case "events-webinar":
        case "events-archive":
            GridEventsRefresh(sessionObject);
            break;
        default:
            GridInvestmentsRefresh(sessionObject);
    }

    GridCommonRefresh(sessionObject);
}

//function to set omniture tags
jQuery(function ($) {
    $(".omniture").click(function () {
        var param = $(this).attr("data-omniture");
        setOmniture(param);
    });

});
