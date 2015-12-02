$(document).ready(function () {

    $('.vertical-tabs .tabs li').each(function (i) {

        $(this).click(function () {

            $('.vertical-tabs .tabs li').removeClass('active');
            $('.vertical-tabs .tab-content').hide();

            $(this).addClass('active');

            $('.vertical-tabs .tab-content').eq(i).show();

            // needed to initialize the chart due to an IE bug when rendering it in a hidden element            
            var chart = $('.vertical-tabs .tab-content').eq(i).find('.pie-chart');

            // call the dynamically named function that creates the chart
            if (chart.length > 0 && chart.hasClass('hidden'))
                window['show' + chart.attr('id')]();
        });
    });

    // select first tab by default
    $('.vertical-tabs .tabs li').first().addClass('active');
    $('.vertical-tabs .tab-content').first().show();

});