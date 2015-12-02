$(document).ready(function () {
    /* Adding some validation method specifc for genworth calendar filter */
    $.validator.addMethod("HistoricDate", function (value, element) {
        var calendarFilter = $(this), dateInput = calendarFilter.find('.dateInput:first');
        var currentDate = new Date($(".dateInput").val());
        var todaysDate = new Date();
        var minDate = new Date(2011, 2, 4);
        if (currentDate.getTime() < todaysDate.getTime()) {
            $(".dateInput").removeClass("error");
            $("label.error").remove();
            return true;
        } else {
            $(".dateInput").addClass("error");
            return false;
        }
    },
        "Please enter a valid date. "
    );

    $.validator.addMethod("ValidDate", function (value, element) {
        var calendarFilter = $(this), dateInput = calendarFilter.find('.dateInput:first');
        var dateEntered = $(".dateInput").val();
        var enteredDate = new Date(dateEntered);
        var month = String(enteredDate.getMonth() + 1);
        var day = String(enteredDate.getDate());
        if (month.length < 2) {
            month = '0' + month;
        }
        if (day.length < 2) {
            day = '0' + day;
        }
        var newdate_1 = month + '/' + day + '/' + enteredDate.getFullYear();
        var dateParts = dateEntered.match(/(\d{1,2})\/(\d{1,2})\/(\d{4}|\d{2})/);
        var day = dateParts[2]
        if (day.length < 2) {
            day = "0" + day;
        }
        var month = dateParts[1]
        if (month.length < 2) {
            month = "0" + month;
        }
        var year = dateParts[3];
        var newdate_1 = month + '/' + day + '/';
        if (dateParts[3].length == 4) {
            newdate_1 += enteredDate.getFullYear();
        } else {
            newdate_1 += enteredDate.getYear();
        }
        var newdate_2 = month + '/' + day + '/' + year;
        if (newdate_1 == newdate_2) {
            $(".dateInput").removeClass("error");
            $("label.error").remove();
            return true;
        } else {
            $(".dateInput").addClass("error");
            return false;
        }
    },
        "Please enter a valid date. "
   );

    $('.genworthcalendarfilter').validate({
        rules: {
            genworthdate: {
                required: true,
                HistoricDate: true,
                ValidDate: true,
                onfocusout: false,
                onsubmit: false,
                onkeyup: false,
                onclick: false,
                focusCleanup: true
            }
        }
    });

});

