$(function () {
    $('.toggler').click(function () {
        var e = this;
        var jq = $(e);
        var sibs = jq.siblings();
        var sHtml = jq.html();

        jq.toggleClass('showing');
        if (jq.hasClass('showing')) {
            sibs.fadeIn();
            sHtml = jq.attr('hideText') == undefined ? sHtml : jq.attr('hideText');
        }
        else {
            sibs.fadeOut();
            sHtml = jq.attr('showText') == undefined ? sHtml : jq.attr('showText');
        }
        jq.html(sHtml);
    }).siblings().hide();

    $('.show-all').click(function () {
        var e = this;
        var jq = $(e);
        var sHtml = jq.html();
        jq.toggleClass('showing');
        var jqTogglers = $('.toggler');

        if (!jq.hasClass('showing')) {
            sHtml = jq.attr('showText') == undefined ? sHtml : jq.attr('showText');
            jqTogglers.each(function () {
                var jq = $(this);
                if (jq.hasClass('showing')) { jq.click(); }
            });
        }
        else {
            sHtml = jq.attr('hideText') == undefined ? sHtml : jq.attr('hideText');
            jqTogglers.each(function () {
                var jq = $(this);
                if (!jq.hasClass('showing')) { jq.click(); }
            });
        }
        jq.html(sHtml);


    });
});