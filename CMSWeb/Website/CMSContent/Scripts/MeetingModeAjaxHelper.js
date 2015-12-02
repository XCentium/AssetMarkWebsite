$(function () {

    $(".onOffSwitch").click(function () {
        $("#meetingModeMessage").dialog("open");

        return false;
    });

    meetingModeDialog = $("#meetingModeMessage");

    ActionMessage = meetingModeDialog.attr("data-ActionMessage");
    JsonUrl = meetingModeDialog.attr("data-JsonUrl");
    RootPath = meetingModeDialog.attr("data-RootPath");

    meetingModeDialog.dialog({
        resizable: false,
        modal: true,
        dialogClass: "MeetingModeDialog",
        width: 400,
        height: 200,
        zIndex: 10000,
        autoOpen: false,
        buttons:
        [
            {
                text: "Cancel",
                className: "cancelButton",
                click: function () {
                    $(this).dialog("close");
                }
            },
            {
                text: ActionMessage + ' Meeting Mode',
                click: function () {
                    JsonUrl += "?date=" + new Date().getTime().toString();
                    $.getJSON(
                        JsonUrl,
                        function (data) {
                            Action = data["ActionName"];
                            if (Action == "Refresh") {
                                window.location.reload();
                            }
                            else if (Action == "Redirect") {
                                Redirect = RootPath + data["UrlString"];
                                window.location.href = Redirect;
                            }
                            else {
                                alert("Can't recognize this action " + Action);
                            }
                        }
                    );

                    $(this).dialog("close");
                    $("#loadingMessage").dialog("open");
                }
            }
        ]
    });

    $("#loadingMessage").dialog({
        resizable: false,
        height: 160,
        zIndex: 10000,
        modal: true,
        autoOpen: false
    });
});
