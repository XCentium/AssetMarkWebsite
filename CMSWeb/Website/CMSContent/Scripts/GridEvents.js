// Main Binder
function GridEventsInitialize() {

    GetSessionObjectJsonMethod = "GetSessionObject/";

    $("#LocationSearchBar").each(function () {
        var oSearchDiv;
        var txtZipCode;

        oSearchDiv = $(this);

        //alert($("select[id$='ddlProximity']").text());

        oSearchDiv.find("[id$='Zipcode']:text").keypress(function (event) {
            if (event.which == 13) {
                event.preventDefault();
                SearchSetProximity(
                    oSearchDiv.find("select[id$='ddlProximity']").val(),
                    escape($(this).val())
                );
                return false;
            }
        });
        oSearchDiv.find(".submit").click(function (event) {
            event.preventDefault();
            IsFromSetKeyword = true;
            SearchSetProximity(
                oSearchDiv.find("select[id$='ddlProximity']").val(),
                escape(oSearchDiv.find("[id$='Zipcode']:text").val())
            );
        });
    });

    var isAutoRefreshEnable = true;

    var date = getUrlVars()["date"];

    if (date) {
        $.getJSON(StringFormat("{2}SetDate/{0}/{1}", ServiceCode, escape(date), GridJsonServiceUrl), function (oResearch, status) {
            GridRefresh(oResearch);
        });
        isAutoRefreshEnable = false;
    }

    return isAutoRefreshEnable;
}

function SearchSetProximity(sRatio, sZipCode) {
    $.getJSON(StringFormat("{3}SetLocation?type={0}&ratio={1}&zipCode={2}", ServiceCode, sRatio, sZipCode, GridJsonServiceUrl), function (oResearch, status) {
        GridRefresh(oResearch)
    });
}

// Main Refresh
function GridEventsRefresh(sessionObject) {
    EventsRefreshResults(sessionObject);
}

function EventsRefreshResults(sessionObject) {
    $.getJSON(GridJsonServiceUrl + "GetResults/" + ServiceCode, function (eventItems, status) {
        $("table.InvestmentResearchGrid tbody").each(function () {
            $table = $(this);
            $table.empty();

            if (eventItems.length > 0) {

                $.each(eventItems, function (index, item) {
                    $table.append(RenderEventsRow(item));
                });

                window.dialogBox.shadowboxPopUp('a[rel*="eventshadowbox"]');

                $('tr[href]', $table).click(function () {
                    var param = $(this).attr("data-omniture");
                    setOmniture(param);
                    window.location = $(this).attr("href");
                });

                $('a[rel*="newWindowParam"]', $table).click(function () {
                    window.open($(this).attr("href"), '_blank');
                    return false;
                });

                $('a', $table).click(function () {
                    var param = $(this).attr("data-omniture");
                    setOmniture(param);
                    return false;
                });

            } else {
                $table.append('<tr><td colspan="5" align="left">There are no events available.</td></tr>');
            }

        });
    });
}

function RenderEventsRow(item) {

    var RowDeclaration = GetEventRowDeclaration(item);

    var buttonCell = GetEventButton(item);

    switch (ServiceCode.toLowerCase()) {
        case "events-inperson":
            var s = StringFormat('{0}<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>',
                RowDeclaration, item.EventName, item.BeginDate, item.Location, buttonCell);
            break;
        case "events-webinar":
        case "events-archive":
            var s = StringFormat('{0}<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>',
                RowDeclaration, item.EventName, item.BeginDate, item.EventType, buttonCell);
            break;

        default:
            var s = StringFormat('{0}<td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>',
                RowDeclaration, item.EventName, item.BeginDate, item.Location, item.EventType, buttonCell);
            break;
    }

    return s;
}

function GetEventButton(item) {

    var newWindowParams = 'newWindowParam';
    var RegisterNow = "Register&nbsp;now";
    var sOmnitureAttr = GetOmnitureAttribute(item);

    var buttonFormat = '<span class="button view-event-link"><a href="{0}" rel="{1}" {2}>{3}</a>';

    var response = "";

    if (item.IsInvitationOnly && ServiceCode.toLowerCase() != "events-archive") {
        response = item.InvitationOnlyText;
    } else {
        if (item.ActionUrl) {
            if (item.IsPastEvent) {
                if (item.EventType.toLowerCase() != "in-person") {
                    response = StringFormat(buttonFormat, item.ActionUrl, newWindowParams,sOmnitureAttr, 'View&nbsp;Archive');
                }
            } else {
                if (item.EventType.toLowerCase() == "webinar") {
                    response = StringFormat(buttonFormat, item.ActionUrl, newWindowParams, sOmnitureAttr, RegisterNow);
                }
                else if (item.EventType.toLowerCase() == "in-person") {
                    response = StringFormat(buttonFormat, item.ActionUrl, 'eventshadowbox;width=600px;height=300px;', sOmnitureAttr, RegisterNow);
                }
            }
        }
    }

    return response;
}

function GetEventRowDeclaration(item) {

    var response = "";
    var sOmnitureAttr = GetOmnitureAttribute(item);

    var rowUrl = (item.RowUrl) ? StringFormat(' href="{0}" {1}', item.RowUrl, sOmnitureAttr) : ""

    response = StringFormat("<tr{0}{1}>",
        (item.IsGwnHosted) ? '' : ' class="third-party"',
        rowUrl
    );

    return response;
}

function GetOmnitureAttribute(item) {

    var sOmnitureAttr = "";

    //Set omniture attribute
    if (typeof (CurrentContextItem) !== 'undefined' && item.Id != null) {
        sOmnitureAttr = 'data-omniture=\"CMS:: ' + CurrentContextItem + " :: " + item.Id + "\" ";
    }

    return sOmnitureAttr;
}

function StringFormat () {
    var s = arguments[0];

    for (var i = 0; i < arguments.length - 1; i++) {       
        var reg = new RegExp("\\{" + i + "\\}", "gm");             
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;

}
