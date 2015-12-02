$(document).ready(function () {

    $('#client_search_input').autocomplete({
        source: '/AccountsAnalysis/Clients/ClientList/Search',
        minLength: 2,
        open: function (event, ui) { $(".ui-autocomplete").css("z-index", 9999); }
    }).focus(function () {
        $(this).closest(".searchContainer").addClass("highlight");
    }).blur(function () {
        $(this).closest(".searchContainer").removeClass("highlight");
    }).keyup(function (event) {

        if ($('#client_search_input').val() != '' && !$('#client_search_input').hasClass('prefill')) {
            $('#cancel').show();
        } else {
            $('#cancel').hide();
        }

        if (event.keyCode == '13') {
            $('#searchbutton').trigger('click');
            $(this).autocomplete('close');
        }
    }).trigger('keyup');

    var recentsearches = [$('#recentsearch1').html(), $('#recentsearch1').html()];
    $('#searchbutton').click(function () {
        if ($('#client_search_input').val() != '' && !$('#client_search_input').hasClass('prefill')) {
            var txt = $('#client_search_input').val();
            if (txt != recentsearches[recentsearches.length - 1]) {
                recentsearches.push(txt);
                var length = recentsearches.length;
                $('#recentsearch1').text(recentsearches[length - 1]);
                $('#recentsearch2').text(recentsearches[length - 2]);
            }
        }
    });

    $('.recentSearch').live('click', function () {
        $('#client_search_input').val($(this).html()).removeClass('prefill').trigger('keyup');
        $('#searchbutton').trigger('click');
        return false;
    });

    $('#cancel').hover(function () {
        $('.client_search').addClass('focus');

    }, function () {
        $('.client_search').removeClass('focus');
    });

    $('#searchbutton').hover(function () {
        $('.client_search').addClass('focus');

        if ($('#client_search_input').val() != $('#client_search_input').attr('prefill') && $('#client_search_input').val() != '') {
            $(this).attr('class', 'searchbutton-hover');            
        }        
    }, function () {
        $('.client_search').removeClass('focus');
        $(this).attr('class', 'searchbutton');
    });

    $('#cancel').click(function () {
        $('#client_search_input').val('');
        $('input[prefill]').prefillText();
        $(this).hide();
    });

    $('.searchbar form.client_search').submit(function (event) {
        return false;
    });

    $('.searchbar .search_container .help').click(function (event) {
        return false;
    });
});

jQuery(function ($) {
	$('input[example]').each(function () {
		var e = this;
		var jq = $(e);
		if (jq.val() == '') {
			jq.val(jq.attr('example'));
		}
		jq.focus(function () {
			$(this).addClass('editing');
			if ($(this).val() == $(this).attr('example')) {
				$(this).val('');
			}
		});
		jq.blur(function () {
			var jq = $(this);
			if (jq.val() == '') {
				jq.removeClass('editing');
				jq.val(jq.attr('example'));
			}
		});
	});
	window.currentInputSpan = null;
	$('.text-input input[type="text"]').each(function () {
		var e = this;
		var jq = $(e);
		var jqSpan = $(this).closest('.text-input');
		jq.focus(function () {
			jqSpan.addClass('focus');
			window.currentInputSpan = jqSpan;
		});
		jq.blur(function () {
			//$(this).closest('.text-input').removeClass('focus');
			var tLatentFocusToggle = setTimeout(function () {
				window.currentInputSpan.removeClass('focus');
			}, 250);
		});
	});

});