
// Script code for the Strategies sublayout.
// Expects the page to generate a StrategyData global variable.
Strategies = function () {

    function sliderControl() {

        var gray1 = "rgb(203,205,199)";
        var gray2 = "rgb(101,101,101)";
        var gray3 = "rgb(180,180,180)";
        var green = "rgb(0,124,56)";
        var white = "white";

        var controlWidth = 464;
        var controlHeight = 62;
        var baselineY = 32;
        var rulerStartY = 25;
        var rulerEndY = 25 + 17;
        var rulerTextY = 59;
        var marginX = 20;

        var minPos = marginX;
        var maxPos = marginX + 5 * 80;

        function createSvgElement(tagName, attributes) {
            var e = document.createElementNS("http://www.w3.org/2000/svg", tagName);
            if (tagName == "svg") {
                e.setAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns:xlink", "http://www.w3.org/1999/xlink");
            }
            if (attributes != undefined) {
                for (var key in attributes) {
                    e.setAttribute(key, attributes[key]);
                }
            }
            return e;
        }

        var svg = createSvgElement("svg", { width: controlWidth + "px", height: controlHeight + "px", viewBox: "0 0 " + controlWidth + " " + controlHeight });
        var feeBars = svg.appendChild(createSvgElement("g"));
        var baseline = svg.appendChild(createSvgElement("rect", { x: 0, y: baselineY, width: controlWidth, height: 2, fill: gray1 }));
        var rangeBar = svg.appendChild(createSvgElement("rect", { x: minPos, y: baselineY, width: maxPos - minPos, height: 2, fill: green }));

        for (var i = 0; i < 6; i++) {
            var x = marginX + i * 80;
            var crossline = svg.appendChild(createSvgElement("rect", { x: x, y: rulerStartY, width: 1, height: rulerEndY - rulerStartY, fill: gray2 }));
            var text = svg.appendChild(createSvgElement("text", { x: x + 1, y: rulerTextY, "font-size": 10, fill: "black", "text-anchor": "middle", style: "cursor: default" }));
            text.appendChild(document.createTextNode((i * 0.25).toFixed(2) + "%"));
        }

        var minCircle = svg.appendChild(createSvgElement("circle", { cx: minPos, cy: baselineY + 1, r: 10, fill: white, stroke: gray3, "stroke-width": 1 }));
        var maxCircle = svg.appendChild(createSvgElement("circle", { cx: maxPos, cy: baselineY + 1, r: 10, fill: white, stroke: gray3, "stroke-width": 1 }));

        var changeCallback = function () { }

        // Block cursor selection if user misclicks
        svg.addEventListener("mousedown", function (e) { e.preventDefault(); });
        svg.addEventListener("mouseup", function (e) { e.preventDefault(); });

        function histogram(barValues) {
            barValues.forEach(function (value, i) {
                var width = 5 * 80 / barValues.length;
                var x = marginX + i * width;
                value *= 20;
                var feeBar = feeBars.appendChild(createSvgElement("rect", { x: x, y: baselineY - Math.max(value, 0), width: width + 1, height: Math.abs(value) + 1, fill: gray1 }));
            });
        }

        function range(newMin, newMax) {
            if (newMin === undefined) {
                return {
                    min: (minPos - marginX) / (5 * 80) * 1.25,
                    max: (maxPos - marginX) / (5 * 80) * 1.25
                };
            }
            else {
                minPos = marginX + newMin / 1.25 * (5 * 80);
                maxPos = marginX + newMax / 1.25 * (5 * 80);
                updateRange();
            }
        }

        function onEvent(name, callback) {
            if (name == "change") changeCallback = callback;
        }

        function handleCircleInput(circle, moveBeginHandler, moveHandler) {
            var startValue = 0;
            var delta = 0;

            circle.addEventListener("mousedown", function (e) {
                function mouseMove(e) {
                    e.stopPropagation();

                    delta += e.movementX;
                    moveHandler(startValue + delta);
                }

                function mouseUp(e) {
                    e.stopPropagation();
                    document.body.removeEventListener("mousemove", mouseMove, true);
                    document.body.removeEventListener("mouseup", mouseUp, true);
                }

                e.stopPropagation();
                e.preventDefault();
                document.body.addEventListener("mousemove", mouseMove, true);
                document.body.addEventListener("mouseup", mouseUp, true);

                delta = 0;
                startValue = moveBeginHandler();
            });
        }

        var trackingTimeout = null;
        function updateRange() {
            minCircle.setAttribute("cx", minPos);
            maxCircle.setAttribute("cx", maxPos);
            rangeBar.setAttribute("x", minPos);
            rangeBar.setAttribute("width", maxPos - minPos);

            if (trackingTimeout !== null) {
                clearTimeout(trackingTimeout);
                trackingTimeout = null;
            }
            trackingTimeout = setTimeout(function () {
                omnitureTrackEvent("Fees");
            }, 2500);
        }

        handleCircleInput(minCircle, function () {
            return minPos;
        }, function (pos) {
            minPos = Math.max(Math.min(pos, maxPos - 20), marginX);
            updateRange();
            if (changeCallback) changeCallback(range());
        });

        handleCircleInput(maxCircle, function () {
            return maxPos;
        }, function (pos) {
            maxPos = Math.min(Math.max(pos, minPos + 20), marginX + 5 * 80);
            updateRange();
            if (changeCallback) changeCallback(range());
        });

        return {
            element: svg,
            histogram: histogram,
            range: range,
            on: onEvent
        };
    }

    function loadSavedFavorites() {
        function loadSuccess(savedFavorites) {
            // Convert list to dictionary for faster lookup:
            var dictionary = {};
            savedFavorites.forEach(function (favorite) {
                dictionary[favorite.ModelSetTypeId + "::" + favorite.StrategistCode] = true;
            });

            // Update favorite status on strategies:
            StrategyData.strategies.forEach(function (strategy) {
                var key = strategy.modelSetTypeId + "::" + strategy.strategistCode;
                strategy.favorite = dictionary[key] ? true : false;
            });

            updateStrategyList();
        }

        $.ajax({
            url: "/api/v1/FavoriteInvestment",
            dataType: "json",
            success: loadSuccess
        });
    }

    function addSavedFavorite(strategy) {
        $.ajax({
            url: "/api/v1/FavoriteInvestment",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                ModelSetTypeId: strategy.modelSetTypeId,
                StrategistCode: strategy.strategistCode,
                Title: strategy.name
            })
        });
    }

    function removeSavedFavorite(strategy) {
        $.ajax({
            url: "/api/v1/FavoriteInvestment/delete",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                ModelSetTypeId: strategy.modelSetTypeId,
                StrategistCode: strategy.strategistCode,
                Title: strategy.name
            })
        });
    }

    function cloneTemplate(query) {
        var copy = $(query).clone();
        copy.removeClass("template");
        return copy;
    }

    function toStringWithThousandSep(v) {
        var text = Math.round(v).toString();
        for (var i = 3; i < text.length; i += 4) {
            text = text.substr(0, text.length - i) + "," + text.substr(text.length - i);
        }
        return text;
    }

    var strategyFilters = [];

    function sortAndUpdateStrategyList() {
        var sortColumn = $(".strategyListHeader .strategyListColumn.sortColumn");
        var reverse = sortColumn.hasClass("reverseOrder");
        var field = sortColumn.attr("data-field");
        if (field == "favorite") {
            if (!reverse) {
                StrategyData.strategies.sort(function (a, b) { var va = a.favorite ? 1 : 0; var vb = b.favorite ? 1 : 0; if (va < vb) return -1; else if (va > vb) return 1; else return 0; });
            }
            else {
                StrategyData.strategies.sort(function (a, b) { var va = a.favorite ? 1 : 0; var vb = b.favorite ? 1 : 0; if (va > vb) return -1; else if (va < vb) return 1; else return 0; });
            }
        }
        else if (field == "custom") {
            var index = parseInt(sortColumn.attr("data-index"));
            var valueType = StrategyData.header[index].type;
            if (valueType == "Percentage" || valueType == "USD") {
                if (!reverse) {
                    StrategyData.strategies.sort(function (a, b) { var va = parseFloat(a.columns[index]); var vb = parseFloat(b.columns[index]); if (va < vb) return -1; else if (va > vb) return 1; else return 0; });
                }
                else {
                    StrategyData.strategies.sort(function (a, b) { var va = parseFloat(a.columns[index]); var vb = parseFloat(b.columns[index]); if (va > vb) return -1; else if (va < vb) return 1; else return 0; });
                }
            }
            else {
                if (!reverse) {
                    StrategyData.strategies.sort(function (a, b) { if (a.columns[index] < b.columns[index]) return -1; else if (a.columns[index] > b.columns[index]) return 1; else return 0; });
                }
                else {
                    StrategyData.strategies.sort(function (a, b) { if (a.columns[index] > b.columns[index]) return -1; else if (a.columns[index] < b.columns[index]) return 1; else return 0; });
                }
            }
        }
        else {
            if (!reverse) {
                StrategyData.strategies.sort(function (a, b) { if (a.name < b.name) return -1; else if (a.name > b.name) return 1; else return 0; });
            }
            else {
                StrategyData.strategies.sort(function (a, b) { if (a.name > b.name) return -1; else if (a.name < b.name) return 1; else return 0; });
            }
        }

        updateStrategyList();
    }

    function updateStrategyHeader() {

        var index = 0;
        StrategyData.header.forEach(function (item) {
            $(".strategyListHeader .strategyListColumnCustom" + index).text(item.name);
            index++;
        });

    }

    function updateStrategyList() {
        var body = $(".strategyListBody");
        body.empty();

        var searchText = $("#strategySearchField").val().toLowerCase();

        var count = 0;
        StrategyData.strategies.forEach(function (strategy) {
            if (searchText.length > 1 && strategy.name.toLowerCase().indexOf(searchText) == -1) return;

            var skip = false;
            strategyFilters.forEach(function (filter) { if (!filter(strategy)) skip = true; });
            if (skip) return;

            count++;

            var item = cloneTemplate(".strategyListRow.template");
            if (strategy.rowing !== undefined) {
                $(".strategyListColumnColor", item).addClass(strategy.rowing ? "rowing" : "sailing");
            }
            $(".strategyDetailLink", item).text(strategy.name);
            $(".strategyDetailLink", item).attr('href', StrategyData.detailUrl + "?Document=" + strategy.id);

            strategy.columns.forEach(function (customValue, customIndex) {
                var valueDiv = $(".strategyListColumnCustom" + customIndex, item);
                var valueType = StrategyData.header[customIndex].type;
                if (valueType == "Percentage") {
                    customValue = parseFloat(customValue);
                    if (!isNaN(customValue)) valueDiv.text(customValue.toFixed(2) + "%");
                }
                else if (valueType == "USD") {
                    customValue = parseFloat(customValue);
                    if (!isNaN(customValue)) valueDiv.html('$<span class="strategyListColumnUSD">' + toStringWithThousandSep(customValue) + '</span>');
                }
                else {
                    valueDiv.text(customValue);
                }
            });

            if (strategy.favorite) {
                $(".strategyFavoriteButton", item).addClass("selected");
            }

            $(".strategyFavoriteButton", item).on('click', function () {
                if (strategy.favorite) {
                    strategy.favorite = false;
                    $(this).removeClass("selected");
                    removeSavedFavorite(strategy);
                }
                else {
                    strategy.favorite = true;
                    $(this).addClass("selected");
                    addSavedFavorite(strategy);
                }
                omnitureTrackEvent("Favorites");
            });

            body.append(item);
        });

        $(".strategyCount").text(count);
    }

    function createInvestmentApproachControl(control) {
        var activeApproachFilters = {};

        var controlItem = cloneTemplate(".filterApproachControl.template");
        var approachGroupItems = $(".approachColumns", controlItem);
        control.groups.forEach(function (approachGroup) {
            var approachGroupItem = cloneTemplate(".approachColumn.template");
            $(".approachColumnTitle", approachGroupItem).text(approachGroup.name);
            approachGroup.approaches.forEach(function (approach) {
                var approachItem = cloneTemplate(".approachCheckbox.template");
                $(".approachCheckboxLabel", approachItem).text(approach.name);
                approachItem.addClass(approach.rowing ? "rowing" : "sailing");
                approachGroupItem.append(approachItem);

                var checkboxInput = $("input[type='checkbox']", approachItem);
                activeApproachFilters[approach.name] = checkboxInput.get(0).checked;

                var activeFilter = {
                    name: approach.name,
                    clear: function () {
                        checkboxInput.get(0).checked = false;
                        activeApproachFilters[approach.name] = false;
                    },
                };

                checkboxInput.on('change', function () {
                    activeApproachFilters[approach.name] = this.checked;
                    if (activeApproachFilters[approach.name]) {
                        addActiveFilter(activeFilter);
                    }
                    else {
                        removeActiveFilter(activeFilter);
                    }
                    updateStrategyList();
                    if (approach.omnitureEvent != "") {
                        omnitureTrackEvent(approach.omnitureEvent);
                    }
                });
            });
            approachGroupItems.append(approachGroupItem);
        });

        strategyFilters.push(function (strategy) {
            var anyActive = false;
            var matchFound = false;
            for (var approachName in activeApproachFilters) {
                if (activeApproachFilters[approachName]) {
                    anyActive = true;
                    matchFound = matchFound || strategy.allocationApproach == approachName;
                }
            }
            return matchFound || !anyActive;
        });

        return controlItem;
    }

    function createRiskProfileControl(control) {
        var controlItem = cloneTemplate(".filterRiskProfileControl.template");
        return controlItem;
    }

    function createFeeControl(control) {
        var numColumns = 50;
        var maxPercentage = 1.25;

        var histogram = [];
        for (var histogramIndex = 0; histogramIndex < numColumns; histogramIndex++) {
            histogram[histogramIndex] = 0;
        }

        StrategyData.strategies.forEach(function (strategy) {
            if (strategy.fee == null) return;
            var histogramIndex = Math.floor(strategy.fee / maxPercentage * histogram.length);
            if (histogram[histogramIndex] != undefined) histogram[histogramIndex]++;
        });

        var maxValue = histogram.reduce(function (a, b) { return Math.max(a, b); }, 1);
        histogram = histogram.map(function (a) { return a / maxValue; });

        var feeControl = sliderControl();
        feeControl.histogram(histogram);

        var feeFilterActive = false;

        var activeFilter = {
            name: "Platform Fee",
            clear: function () {
                feeControl.range(0, 1.25);
                feeFilterActive = false;
                updateStrategyList();
            },
        };

        feeControl.on('change', function (range) {
            var needFilter = range.min > 0 || range.max < 1.25;
            if (!feeFilterActive && needFilter) {
                addActiveFilter(activeFilter);
            }
            else if (feeFilterActive && !needFilter) {
                removeActiveFilter(activeFilter);
            }
            feeFilterActive = needFilter;
            updateStrategyList();
        });

        strategyFilters.push(function (strategy) {
            return !feeFilterActive || (strategy.fee !== null && strategy.fee >= feeControl.range().min && strategy.fee <= feeControl.range().max);
        });

        return feeControl.element;
    }

    function createStrategistListControl(control) {
        var controlItem = $("<div></div>");
        var strategists = {};
        StrategyData.strategies.forEach(function (strategy) {
            if (strategy.strategist == undefined) return;
            strategists[strategy.strategist] = strategy.strategistEvent;
        });
        Object.keys(strategists).forEach(function (strategist) {
            var strategistEvent = strategists[strategist];

            var strategistItem = cloneTemplate(".filterCheckboxControl.template");
            $(".filterCheckboxLabel", strategistItem).text(strategist);
            controlItem.append(strategistItem);

            var checkboxInput = $("input[type='checkbox']", strategistItem);
            var checked = checkboxInput.get(0).checked;

            var activeFilter = {
                name: strategist,
                clear: function () {
                    checkboxInput.get(0).checked = false;
                    checked = false;
                },
            };

            checkboxInput.on('change', function () {
                checked = this.checked;
                if (checked) {
                    addActiveFilter(activeFilter);
                }
                else {
                    removeActiveFilter(activeFilter);
                }
                updateStrategyList();
                omnitureTrackEvent(strategistEvent);
            });

            strategyFilters.push(function (strategy) {
                return !checked || strategy.strategist == strategist;
            });
        });
        return controlItem;
    }

    function createManagerListControl(control) {
        var controlItem = $("<div></div>");
        var managers = {};
        StrategyData.strategies.forEach(function (strategy) {
            if (strategy.manager == undefined) return;
            managers[strategy.manager] = strategy.managerEvent;
        });
        Object.keys(managers).forEach(function (manager) {
            var managerEvent = managers[manager];

            var managerItem = cloneTemplate(".filterCheckboxControl.template");
            $(".filterCheckboxLabel", managerItem).text(manager);
            controlItem.append(managerItem);

            var checkboxInput = $("input[type='checkbox']", managerItem);
            var checked = checkboxInput.get(0).checked;

            var activeFilter = {
                name: manager,
                clear: function () {
                    checkboxInput.get(0).checked = false;
                    checked = false;
                },
            };

            checkboxInput.on('change', function () {
                checked = this.checked;
                if (checked) {
                    addActiveFilter(activeFilter);
                }
                else {
                    removeActiveFilter(activeFilter);
                }
                updateStrategyList();
                omnitureTrackEvent(managerEvent);
            });

            strategyFilters.push(function (strategy) {
                return !checked || strategy.manager == manager;
            });
        });
        return controlItem;
    }

    function createCheckboxControl(control) {
        var controlItem = cloneTemplate(".filterCheckboxControl.template");
        $(".filterCheckboxLabel", controlItem).text(control.name);

        var checkboxInput = $("input[type='checkbox']", controlItem);
        var checked = checkboxInput.get(0).checked;

        var activeFilter = {
            name: control.name,
            clear: function () {
                checkboxInput.get(0).checked = false;
                checked = false;
            },
        };

        checkboxInput.on('change', function () {
            checked = this.checked;
            if (checked) {
                addActiveFilter(activeFilter);
            }
            else {
                removeActiveFilter(activeFilter);
            }
            updateStrategyList();
            if (control.omnitureEvent != "") {
                omnitureTrackEvent(control.omnitureEvent);
            }
        });

        strategyFilters.push(function (strategy) {
            return !checked || strategy.filters[control.field] == "1";
        });
        return controlItem;
    }

    function createFilterList() {
        strategyFilters = [];

        function createFilterGroup(filtersDiv, filterGroup) {
            if (filterGroup.controls) {
                var groupItem = cloneTemplate(".filterGroup.template");
                $(".filterTitleText", groupItem).text(filterGroup.name);
                if (filterGroup.tip && filterGroup.tip.length > 0) {
                    $(".filterTitleTooltip", groupItem).html(filterGroup.tip);
                }
                else {
                    $(".filterTitleInfoIcon", groupItem).hide();
                }

                filterGroup.controls.forEach(function (control) {
                    var controlItem = null;
                    switch (control.type) {
                        case "Investment Approach Control":
                            controlItem = createInvestmentApproachControl(control);
                            break;
                        case "Risk Profile Control":
                            controlItem = createRiskProfileControl(control);
                            break;
                        case "Fee Control":
                            controlItem = createFeeControl(control);
                            break;
                        case "Strategist List Control":
                            controlItem = createStrategistListControl(control);
                            break;
                        case "Manager List Control":
                            controlItem = createManagerListControl(control);
                            break;
                        case "Checkbox Control":
                            controlItem = createCheckboxControl(control);
                            break;
                    }
                    if (controlItem != null) {
                        $(".filterControls", groupItem).append(controlItem);
                    }
                });

                filtersDiv.append(groupItem);
            }
            else {
                var groupRollout = cloneTemplate(".filterRollout.template");
                var rolloutFilters = $(".filterRolloutGroupItems", groupRollout);
                $(".filterRolloutTitleText", groupRollout).text(filterGroup.name);
                $(".filterRolloutTitle", groupRollout).on('click', function () {
                    $(this).parent().toggleClass('open');
                    $(this).parent().find('.filterRolloutGroupItems').slideToggle();

                    var duration = 500;
                    var fromAngle = 90;
                    var toAngle = 180;
                    if (!$(this).parent().hasClass('open')) {
                        fromAngle = 180;
                        toAngle = 90;
                    }

                    var svgElement = $(this).parent().find('.filterRolloutIcon g');

                    var start = null;
                    var svgAnimation = null;
                    svgAnimation = function(timestamp) {
                        if (!start) start = timestamp;
                        var progress = Math.min((timestamp - start) / duration, 1);
                        var angle = fromAngle * (1 - progress) + toAngle * progress;
                        svgElement.attr("transform", "rotate(" + angle + ")");
                        if (progress < 1) {
                            window.requestAnimationFrame(svgAnimation);
                        }
                    };
                    window.requestAnimationFrame(svgAnimation);
                });
                filterGroup.groups.forEach(function (item) { createFilterGroup(rolloutFilters, item); });
                filtersDiv.append(groupRollout);
            }
        }

        var filters = $(".filterGroupItems");
        filters.empty();

        StrategyData.filterGroups.forEach(function (item) { createFilterGroup(filters, item); });
    }

    var activeFilters = [];

    function addActiveFilter(filter) {
        filter.button = cloneTemplate(".filterToolbarButton.template");
        $(".filterToolbarButtonTitle", filter.button).text(filter.name);
        filter.button.on('click', function () { removeActiveFilter(filter); });
        $(".filterList").append(filter.button);
        activeFilters.push(filter);
    }

    function removeActiveFilter(filter) {
        if (filter.button) {
            filter.button.remove();
        }
        var index = activeFilters.indexOf(filter);
        if (index != -1) activeFilters.splice(index, 1);
        filter.clear();
        updateStrategyList();
    }

    function clearActiveFilters() {
        $(".filterList").empty();
        activeFilters.forEach(function (filter) { filter.clear(); });
        activeFilters = [];
        updateStrategyList();
        omnitureTrackEvent("ClearFilters");
    }

    function toggleStrategyOpener() {
        $(".strategySection .filterStrategySplit").toggleClass("open");
        omnitureTrackEvent("MoreDetails");
    }

    function toggleSortOrder() {
        var sortColumn = $(this).hasClass("sortColumn");
        var sortReverse = $(this).hasClass("reverseOrder");
	$(".strategyListHeader .strategyListColumn").removeClass("sortColumn");
	$(".strategyListHeader .strategyListColumn").removeClass("reverseOrder");

        $(this).addClass("sortColumn");
        if (!sortReverse) $(this).addClass("reverseOrder");

        sortAndUpdateStrategyList();

        var sortEvent = $(this).attr("data-omniture");
        if (sortEvent != "" && sortEvent != undefined) {
            omnitureTrackEvent(sortEvent);
        }
    }

    var searchFieldTrackTimeout = null;
    function searchFieldInput() {
        updateStrategyList();

        if (searchFieldTrackTimeout !== null) {
            clearTimeout(searchFieldTrackTimeout);
            searchFieldTrackTimeout = null;
        }

        searchFieldTrackTimeout = setTimeout(function () {
            omnitureTrackEvent("SearchBox");
        }, 2500);
    }

    function omnitureTrackEvent(name) {
        try {
            s.linkTrackEvents = name;
            s.tl(true, 'o');
        }
        catch (e) {
            console.warn("omnitureTrackEvent(" + name + ") failed: " + e);
        }
    }

    $(document).ready(function () {
        if (window.StrategyData == undefined) return;

        $("#strategyClearFilters").on('click', clearActiveFilters);
        $("#strategySearchField").on('input', searchFieldInput);
        $("#strategyOpener").on('click', toggleStrategyOpener);
        $(".strategyListHeader .strategyListColumn").on('click', toggleSortOrder);

        createFilterList();
        updateStrategyHeader();
        sortAndUpdateStrategyList();
        loadSavedFavorites();
    });

    return {};
}();

// Script code for the Strategy Detail sublayout.
// Expects the page to generate a StrategyDetailData global variable.
StrategyDetail = function () {

    function loadSavedFavorites() {
        function loadSuccess(savedFavorites) {
            var saved = savedFavorites.reduce(function (favorite, lastResult) {
                return lastResult || (StrategyDetailData.modelSetTypeId == favorite.ModelSetTypeId && StrategyDetailData.strategistCode == favorite.StrategistCode);
            }, false);

            var div = $(".strategyDetail .strategyFavoriteButton");
            if (saved) {
                div.addClass("selected");
            }
            else {
                div.removeClass("selected");
            }
        }

        $.ajax({
            url: "/api/v1/FavoriteInvestment",
            dataType: "json",
            success: loadSuccess
        });
    }

    function addSavedFavorite() {
        $.ajax({
            url: "/api/v1/FavoriteInvestment",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                ModelSetTypeId: StrategyDetailData.modelSetTypeId,
                StrategistCode: StrategyDetailData.strategistCode,
                Title: StrategyDetailData.title
            })
        });
    }

    function removeSavedFavorite() {
        $.ajax({
            url: "/api/v1/FavoriteInvestment/delete",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                ModelSetTypeId: StrategyDetailData.modelSetTypeId,
                StrategistCode: StrategyDetailData.strategistCode,
                Title: StrategyDetailData.title
            })
        });
    }

    function omnitureTrackEvent(name) {
        try {
            s.linkTrackEvents = name;
            s.tl(true, 'o');
        }
        catch (e) {
            console.warn("omnitureTrackEvent(" + name + ") failed: " + e);
        }
    }

    function sidebarClicked(e) {
        var url = $(this).attr("data-url");
        var ext = $(this).attr("data-extension");
        var omnitureEvent = $(this).attr("data-omniture-event");
        if (omnitureEvent != undefined && omnitureEvent != "") {
            omnitureTrackEvent(omnitureEvent);
        }
        if (ext != undefined && ext.toLowerCase() == "pdf") {
            $(".strategyDetail .sidebarRow").removeClass("selected");
            $(this).addClass("selected");
            var iframe = $(".detailDocument iframe");
            iframe.attr("src", iframe.attr("data-viewer-url-prefix") + url);
        }
        else {
            // Give omniture 500 ms to track the event before navigating
            setTimeout(function () {
                window.location.href = url;
            }, 500);
        }
    }

    function favoriteButtonClicked() {
        var div = $(".strategyDetail .strategyFavoriteButton");
        if (div.hasClass("selected")) {
            div.removeClass("selected");
            removeSavedFavorite();
        }
        else {
            div.addClass("selected");
            addSavedFavorite();
        }
    }

    $(document).ready(function () {
        if (window.StrategyDetailData == undefined) return;

        $(".strategyDetail .sidebarRow").on('click', sidebarClicked);
        $(".strategyDetail .strategyFavoriteButton").on('click', favoriteButtonClicked);
        loadSavedFavorites();
    });

    return {};
}();
