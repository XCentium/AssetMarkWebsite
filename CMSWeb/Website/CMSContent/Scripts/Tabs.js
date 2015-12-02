jQuery(function ($) {

    var bTabsEnabled;
    var jqTabWrappers;
    var jqFootCards;

    bTabsEnabled = true;
    jqTabWrappers = $('.tabs-wrapper');
    jqFootCards = $('.foot-cards li');

    if ($('body.strategy-performance').length > 0) {
        bTabsEnabled = false;
    }

    // for each tab-wrapper
    jqTabWrappers.each(function () {

        // closures
        var eTabWrapper;
        var jqTabs;
        var jqTabContent;
        var fClickTab;

        // assign closures
        eTabWrapper = this;
        jqTabs = $('ul.tabs li a', this);
        jqTabContent = $('ul.tab-content > li', this);

        // private method(s)
        fClickTab = function (iTabIndex) {

            // get y offset
            alert(window.pageYOffset);

            // de-select currently selected tabs/content
            //$('ul.tabs > li.selected, ul.tab-content > li.selected', eTabWrapper).removeClass('selected');

            // select new tabs/content
            //$(jqTabs[iTabIndex]).addClass('selected');

            // if the tab-content <ul> is used
            //if (jqTabContent.size() == jqTabs.size()) {
            // select the associated tab-content <ul>
            //$(jqTabContent[iTabIndex]).addClass('selected');
            //}

        };
        fClickFootCard = function () {
            var e;
            e = this;
            // is there only one(1) link in the child html?
            if ($('a', e).size() == 1) {
                // click child link
                var a;
                a = $('a:first', this)[0];
                if (a.click)
                    a.click();
                else
                    window.location = $(a).attr('href');
            }
        };

        // unobtrusive interaction
        if (bTabsEnabled) {
            jqTabs.each(function (index) {
                $(this).click(function () {

                    var a = $(this);
                    var s = a.attr('href') + '?y=' + window.pageYOffset + "#scroll";
                    window.location.href = s;
                    return false;

                    //fClickTab(index); 
                });
            });
        }
        jqFootCards.each(function () {
            // is there only one(1) link in the child html?
            if ($('a', this).size() == 1) {
                $(this).addClass('pointer');
                $(this).mouseup(fClickFootCard);
            }
        });


    });

});