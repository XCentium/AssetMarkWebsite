(function ($) {

$.ajaxSelector = {
    version : "1.0",
    status : {
        NoData : "NoData",
        DataAvalible : "DataAvalible",
        GettingData : "GettingData"
    }
};
    
$.widget( "genworth.ajaxSelector", {
	version: "1.0",
    options: {
		serviceStringFormat: $.noop,
        onSuccess: $.noop,
        onError: $.noop,
        onGettingData: $.noop 
    },

	_onChange: function() 
    {
        this._getAjaxData();
    },

    _clearData : function()
    {
        this._status = $.ajaxSelector.status.NoData;
        this._jsonData = $.noop;
    },

	_getAjaxData: function() 
    {
        var that = this;
        serviceString = this.GetServiceString();

        if(!serviceString)
        {
            that._clearData();
            return;
        }

        this.options.onGettingData();
        this._status = $.ajaxSelector.status.GettingData;

        $.getJSON(serviceString, function(data) {
            that._jsonData = data;
            that._status = $.ajaxSelector.status.DataAvalible;
            that.options.onSuccess({
                data : that.GetData(),
                url : that.GetServiceString()
            });
        })
        .fail(function( jqxhr, textStatus, error ) 
        {
            that._clearData();
            that.options.onError(jqxhr, textStatus, error); 
        });

    },

    GetStatus : function()
    {
        return this._status;
    },

    GetData: function()
    {
        return this._jsonData;
    },

    GetServiceString: function()
    {
        this._serviceString = this.options.serviceStringFormat(
            this.element.val()
        );
        
        return this._serviceString;
    },

	_init: function() {

        this._status = $.ajaxSelector.status.NoData;
        var that = this;
        that.element.bind('change', function () {
            that._onChange()
        });
    },

    widget: function () {
        return this.ajaxSelector;
    }

    
});

})(jQuery);