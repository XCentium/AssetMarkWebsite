/*globals window jQuery */
/*
* genworth grid extensions
*
* Depends:
*	jquery-1.4.2.js
*   wijmo 1.1.6 wijgrid
*/
(function ($) {
    "use strict";

    $.extend($.wijmo.wijgrid.prototype, {
        externalFilters: [],
        linkExternalFilter: function (element, filterType) {
            //if a string is passed process it as a selector
            //TODO add some error checking
            if (typeof (element) === "string") {
                element = $(element).get(0);
            }

            var filter = {
                name: "",
                element: element,
                type: filterType,
                value: ""
            };

            switch (filterType.toLowerCase()) {
                case "checkbox":
                    filter.name = getNameFromContainer(element);
                    break;
                case "text":
                    filter.name = getNameFromInput(element);
                    break;
                default:
                    throw "Unknown filter type, please verify that the type is a valid html input type";
                    break;
            }

            //add filter to external filters


            function getNameFromInput(element) {
                return ($(element).attr("name")) ? $(element).attr("name") : $(element).attr("id");
            }

            function getNameFromContainer(element) {
                //get all inputs
                var inputs = $("input", element),
                inputsLength = inputs.length,
                inputNames = [],
                inputName = "",
                input = null;

                //check for a common name
                for (var i = 0; i < inputsLength; i++) {
                    input = inputs[i];
                    inputName = $(input).attr("name");
                    inputName = inputName.replace(/\[\]/i, ""); //remove [] from the name

                    if (!inputNames[inputName]) {
                        inputNames[inputName] = 0;
                    }

                    inputNames[inputName]++;
                }

                inputNames = inputNames.sort(function (a, b) { return b - a });

                //return name with most matches
            }
        },

        //
        // Column management dialog
        //
        enableColumnManagement: function () {
            // make sure genworthCustomizeConfig() is available - it is defined in the search bar view...
            if (typeof genworthCustomizeConfig != 'function')
                return;

            var el = this.element, gridId = el.attr("id"), columns = this.options.columns,
                customizeConfigOptions = new Array(columns.length);

            $.each(columns, function (index, column) {
                customizeConfigOptions[index] = {
                    name: gridId + '_column_' + column.dataKey.toLowerCase(),
                    label: column.headerText,
                    checked: column.visible,
                    changeCallback: function (event) {
                        column.visible = $(this).is(":checked");
                        el.wijgrid("doRefresh");
                    }
                };
            });

            genworthCustomizeConfig({
                options: customizeConfigOptions
            });
        }
    });
})(jQuery);