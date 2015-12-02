$(init);

function init() {
    $('.genworth-article-list .article-date').css('position', 'absolute');

    var b = $.browser;
    var v = parseFloat($.browser.version);

    var fWindowResize = function () {
    	if ($(window).width() < 1050) {
    		$('#wrapper-inner').addClass('hideShadow');
    		$('img.container_bg_bottom').hide();
    	}
    	else {
    		$('#wrapper-inner').removeClass('hideShadow');
    		$('img.container_bg_bottom').show();
		}
    };
    fWindowResize();
    $(window).resize(fWindowResize);

//    if (b.msie && (v < 8.0)) {
//    	var sAlert = "Attention:\n";
//    	sAlert += "eWealthManager has been optimized for Internet Explorer version 8.0 and above.";
//    	alert(sAlert);
//	}

    $('hr').each(function () {
        var jq = $(this);
        var divCssClass = "hr-wrapper";
        var jqCssClass = jq.attr('class');
        jq.wrap(function () {
            return '<div class="' + divCssClass + '" />';
        });
        //divCssClass += jqCssClass != undefined ? ' ' + jqCssClass: '';
        jq.closest('.hr-wrapper').addClass(jqCssClass);
    });

    var f = function () {
        $('.grid-system>.gc>.sidebar,.grid-system>.gc>.aside-right').each(function () {
            var e = this;
            var jq = $(e);
            var eGC = e.parentNode;
            var jqGC = $(eGC);
            var eGS = eGC.parentNode;
            var jqGS = $(eGS);
            var jqEventCal = $('table.event-calendar');
            if (jqEventCal.size() > 0) {
                var iCalWidth = jqEventCal.outerWidth();
                var iCalCellWidth = Math.floor(iCalWidth / 7);
                //$('table.event-calendar td[align="center"]').css('width', iCalCellWidth);
            }

            var iMaxHeight = jqGS.innerHeight();
            var iContainerOuterHeight = jqGC.outerHeight();
            var iCurrentOuterHeight = jq.outerHeight();
            var sCurrentCssHeight = jq.css('height').toString();
            var iCurrentCssHeight = parseInt(sCurrentCssHeight.replace("px", ""));
            var iDeltaHeight = iMaxHeight - iContainerOuterHeight;
            var iNewCssHeight = iCurrentCssHeight + iDeltaHeight;
            jq.css('height', iNewCssHeight);
        });
    };
    $(window).load(f);

    $('.inline-video').each(function () {
    	var e = this;
    	var jq = $(e);
    	var sHref = jq.attr('href');
    	jq.click(function () {

    		if (jq.hasClass('isDocument')) {
    			window.open(sHref, '_blank');
    		}
    		else {
    			$('.sitecore-content, #sitecore-content').hide();
    			$('.inline-video-theater iframe').attr('src', jq.attr('href'));
    			$('.inline-video-theater').show();
    		}
    		return false;
    	});
    });
    $('.inline-video-theater').click(function () {
        $('.sitecore-content, #sitecore-content').show();
        $('.inline-video-theater iframe').attr('src', '#');
        $('.inline-video-theater').hide();
    });

    $('.content-footer').each(function () {
        var e = this;
        var jq = $(e);
        var iMaxHeight = 0;
        $('.link-list-column', e).each(function () {
            var iHeight = $(this).outerHeight();
            iMaxHeight = iHeight > iMaxHeight ? iHeight : iMaxHeight;
        });
        $('.link-list-column', e).css('height', iMaxHeight);
    });


//    $('#mainMenu li').click(function () {
//        $('#mainMenu li.selected').removeClass('selected');
//        $(this).addClass('selected');
//        $('a', this)[0].click();
//        $('a', this).blur();
//    });
//    $('.secondaryMenu li').click(function () {
//        $('.secondaryMenu li.selected').removeClass('selected');
//        $(this).addClass('selected');
//        $('a', this).blur();
//    });
    $('#mainMenu .iconLink').each(function () {
        $(this).attr('title', $('.iconLinkTitle', this).text());
    });
//    $('.verticalTabs li').click(function () {
//        var jq = $(this);
//        if (!jq.hasClass('summary')) {
//            $('.verticalTabs li.selected').removeClass('selected');
//            $(this).addClass('selected');
//            $('a', this).blur();
//        }
//    });
    
	$(".hasHighlight").hover(showHighlight, hideHighlight);
	$("input[prefill]").prefillText();
	$("#searchInput").focus(function () {
		$(this).closest(".searchContainer").addClass("highlight");
	}).blur(function () {
		$(this).closest(".searchContainer").removeClass("highlight");
	});
	$(".orangeCountIndicator").countindicator({ imageSet: "orange" });
	$(".blueCountIndicator").countindicator({ imageSet: "blue" });
	$(".whiteCountIndicator").countindicator({ imageSet: "white" });
}

// Shows highlight image, assuming the highlight image ends in "hl".
function showHighlight() {
    if($(this).attr("id") === "searchButton" && ($("#searchInput").val() === $("#searchInput").attr("prefill") || $("#searchInput").val() === "")) {
        return;
    }
    else {
        $(this).find("img").each(function () {
            var ext = "." + $(this).attr("src").split('.').pop();
            if ($(this).attr("src").indexOf("_hl") == -1) {
                $(this).attr("src", $(this).attr("src").replace(ext, "_hl" + ext));
            }
        });
    }
}

// Hides highlight image, assuming the highlight image ends in "hl".
function hideHighlight() {    
	$(this).find("img").each(function () {
		var ext = "." + $(this).attr("src").split('.').pop();
		$(this).attr("src", $(this).attr("src").replace("_hl" + ext, ext));
	});
}

function doSearch() {
	var input = $("#headerSearch #searchInput");
	return !input.hasClass("prefill") && input.val() != "";
}
