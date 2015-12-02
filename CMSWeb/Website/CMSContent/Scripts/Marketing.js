var Genworth = Genworth || {};
Genworth.Marketing = Genworth.Marketing || {};

Genworth.Marketing.GetUrlParameter = function () {
    var url = '';
    var params = '';
    var urlFound = false;
    if (window.location.href.indexOf('?')) {
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            if (hashes[i].indexOf('url=') >= 0) {
                var start = hashes[i].indexOf('=') + 1;
                var end = hashes[i].length;
                url = hashes[i].substring(start, end);
                urlFound = true;
            }
            else {
                params = params + '&' + hashes[i]
            }
        }
        if (urlFound) {
            url = url + params;
        }
    }
    return url;
}

Genworth.Marketing.AgentSelectorLoad = function () {

    // Prepare loading bar
    $loadingMessage = $("#loadingMessage");
    $loadingMessage.dialog({
        resizable: false,
        height: 160,
        zIndex: 10000,
        modal: true,
        autoOpen: false
    });


    // Configurate Ajax Selector
    var ajaxSelector = $('select').ajaxSelector({

        serviceStringFormat: function (agentId) {
            if (agentId == "") return;
            var url = "/services/Marketing.svc/GetFormData/" + agentId + "/";

            var urlParam = Genworth.Marketing.GetUrlParameter();
            if (!(urlParam === undefined || urlParam == null || urlParam.length <= 0)) {
                url += "?url=" + urlParam;
            }

            return url;
        },

        onGettingData: function () {
            $loadingMessage.dialog("open");
        },

        onSuccess: function (obj) {
            $loadingMessage.dialog("close");
        },

        onError: function (jqxhr, textStatus, error) {
            $loadingMessage.dialog("close");
            alert("Request Failed: " + error);
        }

    });

    // Config Select Box for good visual impact
    ajaxSelector.selectBox({
        labelFunction: function (label) {
            var elements = label.split("|", 3);
            return '<div class=\"agentSelectOption\"><div class=\"agentSelectLabel\">' + elements[0] + '</div><div class=\"agentSelectLabel\" id="agentSelectOptionName">' + elements[1] + '</div><div class=\"agentSelectLabel\">' + elements[2] + '</div></div>';
        }
    });

    // Configure FlyForm
    $('#aLaunchUnBlockedPopUp').bind("click", function () {
        if (ajaxSelector.ajaxSelector("GetStatus") == $.ajaxSelector.status.DataAvalible) {
            agentSelected();
        } else {
            alert("Please Choose an Advisor ID");
        }
        return false;
    });

}

Genworth.Marketing.MarcomCentralAgentSelectorLoad = function () {

    // Prepare loading bar
    $loadingMessage = $("#loadingMessage");
    $loadingMessage.dialog({
        resizable: false,
        height: 160,
        zIndex: 10000,
        modal: true,
        autoOpen: false
    });


    // Configurate Ajax Selector
    var ajaxSelector = $('select').ajaxSelector({

        onGettingData: function () {
            $loadingMessage.dialog("open");
        },

        onSuccess: function (obj) {
            $loadingMessage.dialog("close");
        },

        onError: function (jqxhr, textStatus, error) {
            $loadingMessage.dialog("close");
            alert("Request Failed: " + error);
        }

    });

    // Config Select Box for good visual impact
    ajaxSelector.selectBox({
        labelFunction: function (label) {
            var elements = label.split("|", 3);
            return '<div class=\"agentSelectOption\"><div class=\"agentSelectLabel\">' + elements[0] + '</div><div class=\"agentSelectLabel\" id="agentSelectOptionName">' + elements[1] + '</div><div class=\"agentSelectLabel\">' + elements[2] + '</div></div>';
        }
    });
}

