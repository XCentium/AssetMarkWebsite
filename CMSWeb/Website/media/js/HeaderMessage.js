jQuery(function ($) {
    showHeaderMessage(0);
    $('#genworth-message-nav-left').click(function () { traverseHeaderMessages(-1); });
    $('#genworth-message-nav-right').click(function () { traverseHeaderMessages(1); });
});

function traverseHeaderMessages(iDirectionAndQty) {
    var jqMessageWrapper = $('#genworth-message-wrapper');
    var jqMessage = $('#genworth-message');
    var jqMessageText = $('#genworth-message-text');
    var jqLI = $('#genworth-message-data li');
    var iMax = jqLI.size();
    var iCurrentLi = jqMessage.attr('currentMessageId') == undefined ? 0 : parseInt(jqMessage.attr('currentMessageId'));
    //alert(iCurrentLi);
    if (iMax > 0) {
        var iNewLi = iCurrentLi + iDirectionAndQty;
        //alert('Step 1: iNewLi = ' + iNewLi);
        iNewLi = iNewLi < 0 ? iMax + iNewLi : iNewLi;
        //alert('Step 2: iNewLi = ' + iNewLi);
        iNewLi = iNewLi >= iMax ? iNewLi % iMax : iNewLi;
        //alert('Step 3: iNewLi = ' + iNewLi);
        jqMessage.attr('currentMessageId', iNewLi);
        eval("jqMessage.fadeOut('fast', function () { showHeaderMessage(" + iNewLi + ") });");
    }

}
function showHeaderMessage(iIndex) {
    //alert('iIndex = ' + iIndex);
    var jqMessageWrapper = $('#genworth-message-wrapper');
    var jqMessage = $('#genworth-message');
    var jqMessageText = $('#genworth-message-text');
    var jqLIs = $('#genworth-message-data li');
    if (iIndex >= 0 && iIndex < jqLIs.size()) {
        var jqLI = $(jqLIs[iIndex]);
        jqMessageText.html(jqLI.html());
        jqMessage.attr('class', jqLI.attr('class'));
        jqMessage.fadeIn('fast');
    }
}