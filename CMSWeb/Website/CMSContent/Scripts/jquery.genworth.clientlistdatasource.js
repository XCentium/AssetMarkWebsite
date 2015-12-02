/*globals window jQuery */
/*
* client list datasource
*
* Depends:
*	jquery-1.4.2.js
*   wijmo 1.1.6 datasource
*/
(function ($) {
    "use strict";
    var clientlistdatasource;
    var clientlistproxy;

    //duplicated from wijdatasource
    clientlistdatasource = function (options) {
        var self = this;
        /// <summary>
        /// The data to process using the wijdatasource class.
        /// Default: {}.
        /// Type: Object. 
        /// </summary>
        self.data = {};
        /// <summary>
        /// The reader to use with wijdatasource. The wijdatasource class will call the
        /// read method of reader to read from rawdata with an array of fields provided.
        /// The field contains a name, mapping and defaultValue properties which define
        /// the rule of the mapping.
        /// If no reader is configured with wijdatasource it will directly return the
        /// raw data.
        /// Default: null.
        /// Type: Object. 
        /// </summary>
        self.reader = null;
        /// <summary>
        /// The proxy to use with wijdatasource. The wijdatasource class will call
        /// the proxy object's request method.  
        /// In the proxy object, you can send a request to a remote server to
        /// obtain data with the ajaxs options object provided.
        /// Then you can use the wijdatasource reader to process the raw data in the call.
        /// Default: null.
        /// Type: Object. 
        /// </summary>
        self.proxy = null;
        /// <summary>
        /// The processed items from the raw data.  This can be obtained after
        /// datasource is loaded.
        /// Default: [].
        /// Type: Array. 
        /// </summary>
        self.items = [];
        /// <summary>
        /// Function called before loading process starts
        /// Default: null.
        /// Type: Function. 
        /// Code example:
        /// var datasource = new wijdatasource({loading: function(e, data) { }})
        /// </summary>
        /// <param name="datasource" type="wijdatasource">
        /// wijdatasource object that raises this event.
        /// </param>
        /// <param name="data" type="Object">
        /// data passed in by load method.
        /// </param>
        self.loading = null;
        /// <summary>
        /// Function called after loading.
        /// Default: null.
        /// Type: Function. 
        /// Code example:
        /// var datasource = new wijdatasource({loaded: function(e, data) { }})
        /// </summary>
        /// <param name="datasource" type="wijdatasource">
        /// wijdatasource object that raises this event.
        /// </param>
        /// <param name="data" type="Object">
        /// data passed in by load method.
        /// </param>
        self.loaded = null;

        self.mode = null;

        self.grid = null;

        self._constructor(options);
    }

    //copy wijdatasource methods
    $.extend(clientlistdatasource.prototype, window.wijdatasource.prototype);

    window.clientlistdatasource = clientlistdatasource;

    //add our own methods overriding which ever ones we want
    $.extend(window.clientlistdatasource.prototype, {
        _constructor: function (options) {
            $.extend(this, options);
        },

        read: function () {
            var self = this,
			d = self.data;

            // reads using a reader object
            if (d && self.reader) {
                self.reader.read(self);
            }
            else {
                // returns raw data if no reader is configured with datasource.
                self.items = self.data;
            }
        }

    });

    //proxy to handle requests/local parsing
    //client

    clientlistproxy = function (options) {
        var self = this;

        self.options = options;

        self.mode = null;

        self.initialized = null;

        self._constructor(options);
    };

    $.extend(clientlistproxy.prototype, window.wijhttpproxy.prototype);

    window.clientlistproxy = clientlistproxy;

    $.extend(clientlistproxy.prototype, {
        _constructor: function (options) {
            $.extend(this, options);
            //*
            //setup grid mode
            switch (options.mode) {
                case 1:
                    //load initial paging info

                    //load sorting info

                    //load filter info

                    //setting the request handler to avoid an extra grid type check
                    this.options.requestHandler = this._localRequest;
                    break;
                case 2:
                    this.options.requestHandler = this._remoteRequest;
                    break;
                default:
                    throw "Unknown gridMode, try local or remote";
                    break;
            }
            //*/
        },

        request: function (datasource, callBack, userData) {

            this.options.requestHandler(datasource, callBack, userData)
        },

        _localRequest: function (datasource, callBack, userData) {
            //local request
            //load local data from static variable
            var tempData = [],
            totalRows = 0,
            pageStart = 0,
            pageEnd = 0;

            //paging calculation
            if (userData.data.paging && clientlistData) {
                totalRows = clientlistData.length;
                pageStart = userData.data.paging.pageIndex * userData.data.paging.pageSize;
                pageEnd = pageStart + userData.data.paging.pageSize;

                tempData = clientlistData.slice(pageStart, pageEnd);
            }

            //set raw data
            datasource.data = {
                rows: tempData,
                totalRows: totalRows
            };

            datasource.read();

            // fire loaded callback
            if ($.isFunction(callBack)) {
                callBack(datasource, userData);
            }
        },

        _remoteRequest: function (datasource, callBack, userData) {
            //old http proxy request handling all serverside
            var self = this,
			o = $.extend({}, this.options),
			oldSuccess = o.success;

            o.success = function (data) {
                if ($.isFunction(oldSuccess)) {
                    oldSuccess(data);
                }
                self._complete(data, datasource, callBack, o, userData);
            };
            $.ajax(o);
        },

        _complete: function (data, datasource, callback, options, userData) {
            // set raw data
            datasource.data = options.key !== undefined ? data[options.key] : data;
            // read raw data using a data reader in datasource
            datasource.read();
            // fire loaded callback
            if ($.isFunction(callback)) {
                callback(datasource, userData);
            }
        }
    });

} (jQuery));