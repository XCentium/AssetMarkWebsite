window.dialogBox = {
    close: function () {
        $('#' + window.dialogBox.elementDictionary.wrapperId + ":visible").fadeOut(function () {
            $('#' + window.dialogBox.elementDictionary.iFrameId).attr('src', '');
            //$('#' + this.elementDictionary.iFrameId).attr('src', window.location.href);
        });
    },
    closeModal: function () {
        var isModal = $('#' + window.top.dialogBox.elementDictionary.wrapperId).attr('modal');
        if (isModal.toLowerCase() == 'no') {
            window.dialogBox.close();
        }
    },
    open: function (sUrl) {
        $('#' + window.dialogBox.elementDictionary.iFrameId).css('visibility', 'hidden').attr('src', sUrl);
        $('#' + window.dialogBox.elementDictionary.wrapperId + ":hidden").show();
        window.dialogBox.setTop();
        $('#' + window.dialogBox.elementDictionary.iFrameId).css('visibility', 'visible');
    },
    elementDictionary: {
        wrapperId: 'gw-dialog-wrapper',
        backgroundId: 'gw-dialog-background',
        iFrameWrapperId: 'gw-dialog-iframe-wrapper',
        iFrameId: 'gw-dialog-iframe',
        closeTriggerId: 'gw-dialog-close-trigger'
    },
    setTop: function () {
        var iMax = parseInt($('#' + window.dialogBox.elementDictionary.wrapperId).css('height').replace('px', ''));
        var iSubtrahend = parseInt($('#' + window.dialogBox.elementDictionary.iFrameWrapperId).css('height').replace('px', ''));
        var iDelta = iMax - iSubtrahend;
        var iTop = parseInt(iDelta / 2);
        $('#' + window.dialogBox.elementDictionary.iFrameWrapperId).css('top', iTop);
    },
    shadowboxPopUp: function (elements) {
        $(elements).each(function () {
            //alert('here');
            var e = this;
            var jq = $(e);
            var rel = jq.attr('rel');
            var fClick = function () {
                //alert('here');
                var o = getParameters(rel);
                $('#' + window.top.dialogBox.elementDictionary.iFrameWrapperId).css(o);

                var scroll = o.scroll ? o.scroll : 'yes';
                $('#' + window.top.dialogBox.elementDictionary.iFrameId).attr('scrolling', scroll);

                var modal = o.modal ? o.modal : 'no';
                $('#' + window.top.dialogBox.elementDictionary.wrapperId).attr('modal', modal);

                var hideClose = o.hideClose ? o.hideClose : 'no';
                if (hideClose.toLocaleLowerCase() == 'yes') {
                    $('#' + window.top.dialogBox.elementDictionary.closeTriggerId).css('display', 'none');
                }

                window.top.dialogBox.open(jq.attr('href'));
                return false;
            };
            jq.click(fClick);
        });
    }

};

function getParameters(sRelText) {
    var aMatch = sRelText.match(/([^;=]+=[^;=]+)/g);
    var o = {};
    for (var i = 0; i < aMatch.length; i++) {
        var a = aMatch[i].split("=");
        o[a[0]] = a[1];
    }
    return o;
}

jQuery(function ($) {

    var d = window.dialogBox;

    if ($('#' + d.elementDictionary.wrapperId).size() == 0) {

        var sDialogSystemMarkup = '';

        sDialogSystemMarkup += '<div id="' + d.elementDictionary.wrapperId + '" style="display:none;overflow:hidden;z-index:999;">';
        sDialogSystemMarkup += '<div id="' + d.elementDictionary.backgroundId + '"></div>';
        sDialogSystemMarkup += '<div id="' + d.elementDictionary.iFrameWrapperId + '">';
        sDialogSystemMarkup += '<iframe id="' + d.elementDictionary.iFrameId + '" scrolling="yes" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen src="" style="margin:0px;width:100%;height:100%"></iframe>';
        sDialogSystemMarkup += '<span id="' + d.elementDictionary.closeTriggerId + '"><span>Close</span></span>';
        sDialogSystemMarkup += '</div>';
        sDialogSystemMarkup += '</div>';

        $('body').append(sDialogSystemMarkup);
        $('#' + d.elementDictionary.wrapperId).css({
            height: $(window).height(),
            width: '100%',
            position: 'fixed',
            top: 0,
            left: 0,
            overflow: 'hidden'
        });
        $('#' + d.elementDictionary.backgroundId).css({
            height: '100%',
            width: '100%',
            position: 'absolute',
            top: 0,
            left: 0,
            backgroundColor: '#000',
            opacity: 0.5,
            zIndex: 2001
        });
        $('#' + d.elementDictionary.iFrameWrapperId).css({
            height: '90%',
            width: '968px',
            position: 'relative',
            margin: '0px auto',
            backgroundColor: '#000',
            zIndex: 2002,
            overflow: 'hidden'
        });
        $('#' + d.elementDictionary.iFrameId).css({
            overflow: 'hidden'
        });
        $('#' + d.elementDictionary.iFrameId).load(function () {
            $('#' + d.elementDictionary.iFrameId).css('visibility', 'visible');
        });
        $('#' + d.elementDictionary.closeTriggerId).css({
            position: 'absolute',
            top: '8px',
            right: '-8px',
            zIndex: 2003
        });
    }

    $('#' + d.elementDictionary.backgroundId).click(d.closeModal);
    $('#' + d.elementDictionary.closeTriggerId).click(d.close);

    $(window).resize(function () {
        $('#' + d.elementDictionary.wrapperId).css({
            height: $(window).height()
        });
        d.setTop();
    });

    window.dialogBox.shadowboxPopUp('a[rel*="shadowbox"]');



});

