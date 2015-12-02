/*
 * Genworth Scrollbar, JQuery UI Slider wrapper
 *
 * eric.degroot@cynergysystems.com
 */
(function($, undefined) {

$.widget("ui.genworthscrollbar", {
    widgetEventPrefix: "slide",

    options: {
        max: 100,
        min: 0,
        orientation: "vertical",
        value: 0
    },

    _create: function() {
        var self = this,
            o = this.options,
            el = this.element;
        
        el.addClass("ui-genworthscrollbar" +
            " ui-genworthscrollbar-" + o.orientation +
            " ui-widget" +
            " ui-widget-content" +
            " ui-corner-all" +
            (o.disabled ? " ui-genworthscrollbar-disabled ui-disabled" : ""));

        this._top = el.append(
            '<div class="ui-genworthscrollbar-inner">' +
            '<div class="ui-genworthscrollbar-track-container">' +
            '<div class="ui-genworthscrollbar-track1 ui-genworthscrollbar-track"></div>' +
            '<div class="ui-genworthscrollbar-track2 ui-genworthscrollbar-track"></div>' +
            '<div class="ui-genworthscrollbar-track3 ui-genworthscrollbar-track"></div>' +
            '</div>' +
            '<div class="ui-genworthscrollbar-slider"></div>' +
            '</div>'
        );

        this._slider = el.find('.ui-genworthscrollbar-slider').slider(o);
        this._handle = this._slider.find('.ui-slider-handle');

        // this._updateDimensions();
        
        this._slider.bind("slide", function(event, ui) {
            self._trigger("slide", event, ui);
        });

        this._top.find('.ui-genworthscrollbar-track').click(function(event, ui) {
            event.preventDefault();

            var sliderOffset = self._slider.offset(), value;
            if (o.orientation == "horizontal") {
                if (sliderOffset.left >= event.pageX)
                    value = o.min;
                else
                    value = o.max;
            } else { // vertical
                if (sliderOffset.top >= event.pageY)
                    value = o.max;
                else
                    value = o.min;
            }
            
            self._slider.slider({
                value: value
            });
            
            // trigger a slide event, setting the value above will only trigger a slidechange from the slider
            self._trigger("slide", event, {
                handle: self._handle,
                value: value
            });
        });
    },

    destroy: function() {
        this.element
            .removeClass("ui-genworthscrollbar" +
                " ui-genworthscrollbar-horizontal" +
                " ui-genworthscrollbar-vertical" +
                " ui-genworthscrollbar-disabled" +
                " ui-widget" +
                " ui-widget-content" +
                " ui-corner-all");

        this._slider.unbind();
        this._top.find('.ui-genworthscrollbar-track').unbind();

        return this;
    },

    _setOption: function(key, value) {
        $.Widget.prototype._setOption.apply( this, arguments );

        switch (key) {
            case "orientation":
                this._slider.slider({ orientation: value });
                this.element
                    .removeClass( "ui-genworthscrollbar-horizontal ui-genworthscrollbar-vertical" )
                    .addClass( "ui-genworthscrollbar-" + this.options.orientation );
                // this._updateDimensions();
                break;
            case "min":
                this._slider.slider({ min: value });
                break;
            case "max":
                this._slider.slider({ max: value });
                break;
            case "value":
                this._slider.slider({ value: value });
                break;
        }
    }
    
    /*
    _updateDimensions: function() {
        var self = this,
            o = this.options,
            el = this.element;

        if (o.orientation == "horizontal") {
            this._top.find('.ui-genworthscrollbar-inner').css({ width: el.width(), height: el.height() });
            this._slider.css({ width: el.width() - this._slider.find('.ui-slider-handle').width(), left: this._slider.find('.ui-slider-handle').width() / 2 });
        } else { // vertical
            this._top.find('.ui-genworthscrollbar-inner').css({ width: el.width(), height: el.height() });
            this._slider.css({ height: el.height() - this._slider.find('.ui-slider-handle').height(), top: this._slider.find('.ui-slider-handle').height() / 2 });
        }
    }
    */
});

}(jQuery));
