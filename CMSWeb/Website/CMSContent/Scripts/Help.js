var Help = Help || {};
Help.faq = Help.faq || {};

Help.faq.expandAnswer = function (elem) {
    var e = elem;
    var jq = $(e);
    var jqAnswer = jq.closest('.answer');
    jqAnswer.toggleClass('minimized');
};
Help.faq.expandDefinition = function (elem) {
    var e = elem;
    var jq = $(e);
    var jqAnswer = jq.closest('.definition');
    jqAnswer.toggleClass('minimized');
   };

   $(function () {
   	$('.faq .answer .more').each(function () {

   		$(this).click(function () { Help.faq.expandAnswer(this); });
   	});
   	$('.glossary .definition .more').each(function () {

   		$(this).click(function () { Help.faq.expandDefinition(this); });
   	});

   	$('.search-block').each(function () {
   		var jqTxt = $('.text-input input[type="text"]', this);
   		var jqImg = $('.text-input img.magnify', this);
   		var jqBtn = $('.text-input input.magnify', this);

   		var fKeyPress = function (event) {
   			if (event.which == 13) {
   				event.preventDefault();
   				jqBtn.click();
   			}
   		};

   		var fMouseUp = function () {
   			if (!jqTxt.hasClass('editing')) {
   				jqTxt.val('').addClass('editing');
   			}
   		};

   		jqTxt.keypress(fKeyPress);
   		jqImg.mouseup(fMouseUp);
   		jqBtn.mouseup(fMouseUp);
   	});
   });