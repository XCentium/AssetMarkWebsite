$("span").hover(function () {
    $(this).addClass("hilite");
}, function () {
    $(this).removeClass("hilite");
});

function closeDialog() {
    $("#emailDialog").fadeOut(3000, function () {
        $("#saveFeedback").unbind();
        $("#loadingDiv").hide();
//        $("#emailDialog").remove();
        $(this).next().remove();
        $(this).remove();
    });
}

function beginProcessing() {
    $("#dialogDiv").hide();
    $("#loadingDiv").show();
    $("#ui-dialog-title-emailDialog").html("Please wait while your request is being processed...");
}

function feedSuccess() {
    $("#emailDialog").remove();
    $('<div id="emailDialog" style="background-color: white;">Your feedback helps us improve eWealthManager to better serve your needs.</div>')
        .dialog({
            title: "Thank you!",         
            resizable: false,
            modal: true,
            width: 430,
            height: 130,
            zIndex: 10000

        }
        );
    closeDialog();
}

function feedFailure() {
    alert("Sorry, some error occured while saving your feedback. Please try again later.");
}

$.ajaxSetup({ cache: false });

$(function () {
    $(".openDialog").unbind().live("click", function (e) {
        e.preventDefault();

        $("<div></div>")
                    .attr("id", $(this)
                    .attr("data-dialog-id"))
                    .dialog({
                        title: "Loading...",
                        width: 500,
                        height: 300,
                        resizable: false,
                        close: closeDialog,
                        modal: true
                    })
                    .load(this.href);
    });

    $(".close").live("click", function (e) {
        e.preventDefault();
        closeDialog();
    });


    $("#saveFeedback").unbind().live("click", function (e) {
        e.preventDefault();
        $("#PageContext").val(window.location.pathname);        
        $("#saveFeedback").closest('form').submit();
    })

});