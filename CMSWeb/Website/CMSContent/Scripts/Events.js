$(function () {
    $('.event-search-proximity select').selectBox();
    $('.show-event-details').click(function () {
        $(this).hide().siblings('.event-details').slideDown().css('zoom', '1');
    });
    $('.hide-event-details').click(function () {
        $(this).closest('.event-details').slideUp().siblings('.show-event-details').show().css('zoom', '1');
    });

    function getElementFromHref(element) {
        return $($(element).attr("href"));
    }

    $(".events-switch-links").each(function () {
        $(this).find("a").each(function () {
            $(this).bind('click', function () {
                $this = $(this);
                $toDisplay = getElementFromHref(this);

                $this.parent().find("a").each(function () {
                    $(this).removeClass("active");
                    getElementFromHref(this).hide();
                });

                $toDisplay.show();
                $this.addClass("active");

                return false;
            });
        });
        $(this).find("a:first-child").each(function () {
            $(this).click();
        });
    });
});