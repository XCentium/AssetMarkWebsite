$(document).ready(function () {
	var resultIsFiltered = false;

	$('#client_search_input').autocomplete({
		source: '/AccountsAnalysis/Clients/ClientList/Search',
		minLength: 3,
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
		if (!$('#client_search_input').hasClass('prefill')) {
			var txt = $('#client_search_input').val();
			if (txt != recentsearches[recentsearches.length - 1]) {
				recentsearches.push(txt);
				var length = recentsearches.length;
				$('#recentsearch1').text(recentsearches[length - 1]);
				$('#recentsearch2').text(recentsearches[length - 2]);
				updateRecentSeparator();
			}
			resultIsFiltered = true;
			$(this).closest('.searchbar').trigger('onSearch', txt);
		} else if (resultIsFiltered) {
			resultIsFiltered = false;
			$(this).closest('.searchbar').trigger('onSearch', '');
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

	function updateRecentSeparator() {
		$('#recentSeparator').css('display', ($('#recentsearch2').text() == '' ? 'none' : 'inline'));
	}

	updateRecentSeparator();
});