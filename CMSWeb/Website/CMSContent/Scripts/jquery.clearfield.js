/*** FUNTION FOR PREFILLING TEXT INPUTS ***/
(function ($) {
    $.fn.prefillText = function (options) {
        var defaults = {
            prefillClass: 'prefill',
            callback: function () {
            }
        };
        var opts = $.extend(defaults, options || {});

        return this.each(function () {
        	$(this).attr('value', $(this).attr('prefill'));
            insertPrefillText($(this));
        });

        function insertPrefillText($elem) {

        	var elemVal = $elem.val(), prefillValue = $elem.attr('prefill');

            if (elemVal === '' || elemVal === prefillValue) {
                $elem.val(prefillValue).addClass(opts.prefillClass);
            }

            /* Binding the element for various interactions */
            $elem.focus(function () {
                var $this = $(this);
                if ($this.val() === prefillValue) {
                    $this.val('').removeClass(opts.prefillClass);
                };
                /* trigger click since focus prevents bubbling up of the original click event */
                $this.trigger("click");
            }).blur(function () {
                var $this = $(this);
                if ($this.val() === '' || $this.val() == prefillValue) {
                    $this.val(prefillValue).addClass(opts.prefillClass);
                }
                else {
                    $this.removeClass(opts.prefillClass);
                }
            }).change(function () {
                $(this).removeClass(opts.prefillClass);
            });
        }
    }
})(jQuery);