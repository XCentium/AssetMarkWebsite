/*

jQuery selectBox (version 1.0.7)

A cosmetic, styleable replacement for SELECT elements.

Homepage:   http://abeautifulsite.net/blog/2011/01/jquery-selectbox-plugin/
Demo page:  http://labs.abeautifulsite.net/projects/js/jquery/selectBox/

Copyright 2011 Cory LaViska for A Beautiful Site, LLC.

*/
/*
Caution!!! - This is a heavily modified version that supports html options and several other
bugfixes/options/tweaks/enhancements needed for the Genworth Financial project.

eric.degroot@cynergysystems.com
*/
if (jQuery) (function ($) {

    $.extend($.fn, {

        selectBox: function (method, data) {
            var typeTimer, typeSearch = '';

            var init = function (select, data) {

                // Disable for iOS devices (their native controls are more suitable for a touch device)
                if (navigator.userAgent.match(/iPad|iPhone|Android/i)) return false;

                // Element must be a select control
                if (!select.is('select')) return false;

                if (select.data('selectBox-control')) return false;

                var wrapper = $('<div class="selectBox-wrapper"><div class="selectBox-background selectBox-background1"></div><div class="selectBox-background selectBox-background2"></div></div>');
                var control = $('<div class="selectBox selectBox-dropdown" />');
                wrapper.append(control);

                var settings = data || {};
                select.data('selectBox-settings', settings);
                if (settings.autoWidth === undefined) settings.autoWidth = true;

                // Inherit class names, style, and title attributes
                control
                    .addClass(select.attr('class'))
                    .attr('style', select.attr('style') || '')
                    .attr('title', select.attr('title') || '')
                    .attr('tabindex', parseInt(select.attr('tabindex')))
                    .bind('focus.selectBox', function () {
                        if (this !== document.activeElement) $(document.activeElement).blur();
                        if (control.hasClass('selectBox-active')) return;
                        control.addClass('selectBox-active');
                        select.trigger('focus');
                    })
                    .bind('blur.selectBox', function () {
                        if (!control.hasClass('selectBox-active')) return;
                        control.removeClass('selectBox-active');
                    });

                if (select.attr('disabled')) control.addClass('selectBox-disabled');

                //
                // Generate Dropdown controls
                //

                var label, arrow = $('<span class="selectBox-arrow" />');

                var optionSelected = $(select).find('OPTION:selected');
                if (optionSelected && optionSelected.length > 0 && settings && settings.labelFunction) {
                    label = $('<span class="selectBox-label"><a rel="' + optionSelected.val() + '">' + settings.labelFunction(optionSelected.text()) + '</a></span>');
                } else {
                    label = $('<span class="selectBox-label" />');
                    if (optionSelected && optionSelected.length > 0) {
                        label.append($('<a rel="' + optionSelected.val() + '">' + (optionSelected.text() || '\u00A0') + '</a>'));
                    }
                }

                // focus fix for IE
                label.focus(function () {
                    $(this).parent().focus();
                });

                // focus fix for IE
                arrow.focus(function () {
                    $(this).parent().focus();
                });

                control
                    .append(label)
                    .append(arrow)
                    .bind('mousedown.selectBox', function (event) {
                        if (control.hasClass('selectBox-menuShowing')) {
                            hideMenus();
                        } else {
                            event.stopPropagation();
                            var options = control.data('selectBox-options');
                            // Webkit fix to prevent premature selection of options
                            options.data({ 'selectBox-down-at-x': event.screenX, 'selectBox-down-at-y': event.screenY });
                            showMenu(select, settings, control, options);
                        }
                    });

                wrapper.hover(function (event) {
                    $(this).addClass('selectBox-hover');
                }, function (event) {
                    $(this).removeClass('selectBox-hover');
                });

                var options = getOptions(select);
                wrapper.append(options);

                control
                    .data('selectBox-options', options)
                    .bind('keydown.selectBox', function (event) {
                        handleKeyDown(select, event);
                    })
                    .bind('keypress.selectBox', function (event) {
                        handleKeyPress(select, event);
                    });

                wrapper.insertAfter(select);

                disableSelection(control);

                updateSlider(select, control);

                // Store data for later use and show the control
                select
                    .addClass('selectBox')
                    .data({ 'selectBox-wrapper': wrapper, 'selectBox-control': control, 'selectBox-settings': settings })
                    .hide();

            };

            var updateSlider = function (select, control) {
                // var control = select.data('selectBox-control');
                var settings = select.data('selectBox-settings');
                var options = control.data('selectBox-options');

                var listContainer = options.find('.selectBox-options-list-container');
                var list = listContainer.find('.selectBox-options-list');
                var scrollBar = options.find('.selectBox-options-scrollbar');

                var listContainerMaxHeight = parseInt(listContainer.css('maxHeight'));
                if (isNaN(listContainerMaxHeight))
                    listContainerMaxHeight = 0;

                if (listContainerMaxHeight > 0 && listContainer.height() < listContainerMaxHeight) {
                    scrollBar.hide();
                    listContainer.css({ width: options.width() });

                    if (options.hasClass("selectBox-options-hasscrollbar"))
                        options.removeClass("selectBox-options-hasscrollbar");

                    return;
                }

                var scrollBarPaddingTop = parseInt(scrollBar.css('paddingTop'));
                if (isNaN(scrollBarPaddingTop))
                    scrollBarPaddingTop = 0;

                var scrollBarPaddingBottom = parseInt(scrollBar.css('paddingBottom'));
                if (isNaN(scrollBarPaddingBottom))
                    scrollBarPaddingBottom = 0;

                // scrollBar.css({ height: listContainer.height() - scrollBarPaddingTop - scrollBarPaddingBottom });

                var scrollRange = list.height() - listContainer.height();

                scrollBar.genworthscrollbar({
                    orientation: "vertical",
                    min: 0,
                    max: scrollRange,
                    value: scrollRange - listContainer.scrollTop(),
                    slide: function (event, ui) {
                        listContainer.scrollTop(scrollRange - ui.value);
                    }
                });

                if (scrollBar.is(":hidden"))
                    scrollBar.show();

                if (!options.hasClass("selectBox-options-hasscrollbar"))
                    options.addClass("selectBox-options-hasscrollbar");

                listContainer.css({ width: options.width() - scrollBar.outerWidth() });
            };

            var getOptions = function (select) {
                var settings = select.data('selectBox-settings');
                var options = $('<div class="selectBox-dropdown-menu selectBox-options"><div class="selectBox-dropdown-menu-background1 selectBox-dropdown-menu-background"></div><div class="selectBox-dropdown-menu-background2 selectBox-dropdown-menu-background"></div><div class="selectBox-dropdown-menu-background3 selectBox-dropdown-menu-background"></div><div class="selectBox-dropdown-menu-background4 selectBox-dropdown-menu-background"></div><div class="selectBox-dropdown-menu-list-container selectBox-options-list-container"><ul class="selectBox-dropdown-menu-list selectBox-options-list" /></div><div class="selectBox-dropdown-menu-scrollbar selectBox-options-scrollbar"></div></div>');
                var optionsListContainer = options.find('.selectBox-options-list-container');
                var optionsScrollBar = options.find('.selectBox-options-scrollbar');
                var optionsList = optionsListContainer.find('.selectBox-options-list');

                var selectOptions = select.children('OPTION');
                if (selectOptions.length > 0) {
                    selectOptions.each(function (index) {
                        var li = $('<li />'), a;
                        li.addClass($(this).attr('class'));

                        if (settings && settings.labelFunction) {
                            a = $('<a>' + settings.labelFunction($(this).text()) + '</a>');
                        } else {
                            a = $('<a />');
                            a.text($(this).text());
                        }

                        a.attr('rel', $(this).val());
                        li.append(a);

                        if ($(this).attr('disabled'))
                            li.addClass('selectBox-disabled');

                        if ($(this).attr('selected'))
                            li.addClass('selectBox-selected');

                        if (index == 0)
                            li.addClass('selectBox-first');

                        if (index + 1 == selectOptions.length)
                            li.addClass('selectBox-last');

                        optionsList.append(li);
                    });
                } else {
                    optionsList.append('<li>\u00A0</li>');
                }

                options
                    .data('selectBox-select', select)
                    .css('display', 'none')
                    .find('A')
                    .bind('mousedown.selectBox', function (event) {
                        event.preventDefault(); // Prevent options from being "dragged"
                        if (event.screenX === optionsList.data('selectBox-down-at-x') && event.screenY === optionsList.data('selectBox-down-at-y')) {
                            optionsList.removeData('selectBox-down-at-x').removeData('selectBox-down-at-y');
                            hideMenus();
                        }
                    })
                    .bind('mouseup.selectBox', function (event) {
                        if (event.screenX === optionsList.data('selectBox-down-at-x') && event.screenY === optionsList.data('selectBox-down-at-y')) {
                            return;
                        } else {
                            optionsList.removeData('selectBox-down-at-x').removeData('selectBox-down-at-y');
                        }
                        selectOption(select, $(this).parent());
                        hideMenus();
                    })
                    .bind('mouseover.selectBox', function (event) {
                        addHover(select, $(this).parent());
                    })
                    .bind('mouseout.selectBox', function (event) {
                        removeHover(select, $(this).parent());
                    });

                disableSelection(optionsList);

                return options;
            };

            var destroy = function (select) {
                alert('destroy');
                var wrapper = select.data('selectBox-wrapper');
                if (!wrapper) return;
                var control = select.data('selectBox-control');
                if (!control) return;
                var options = control.data('selectBox-options');

                options.remove();
                control.remove();
                wrapper.remove();
                select
                    .removeClass('selectBox')
                    .removeData('selectBox-control')
                    .removeData('selectBox-settings')
                    .show();
            };


            var showMenu = function (select, settings, control, options) {
                if (control.hasClass('selectBox-disabled')) return false;

                if (control.hasClass('selectBox-menuShowing'))
                    hideMenus();

                var optionsOffsetLeft = settings.optionsOffsetLeft ? settings.optionsOffsetLeft : 0;
                var optionsOffsetRight = settings.optionsOffsetRight ? settings.optionsOffsetRight : 0;
                var optionsOffsetTop = settings.optionsOffsetTop ? settings.optionsOffsetTop : 0;

                var borderLeftWidth = parseInt(control.css('borderLeftWidth'));
                if (isNaN(borderLeftWidth))
                    borderLeftWidth = 0;

                var borderBottomWidth = parseInt(control.css('borderBottomWidth'));
                if (isNaN(borderBottomWidth))
                    borderBottomWidth = 0;

                var optionsPaddingLeft = parseInt(options.css('paddingLeft'));
                if (isNaN(optionsPaddingLeft))
                    optionsPaddingLeft = 0;

                var optionsPaddingRight = parseInt(options.css('paddingRight'));
                if (isNaN(optionsPaddingRight))
                    optionsPaddingRight = 0;

                // Auto-width // borderLeftWidth + borderLeftWidth + optionsOffsetLeft + optionsOffsetRight + 
                if (settings.autoWidth)
                    options.css('width', control.outerWidth() - (optionsPaddingLeft + optionsPaddingRight));

                // Menu position
                /* options.css({
                top: control.offset().top + control.outerHeight() - borderBottomWidth + optionsOffsetTop,
                left: control.offset().left + optionsOffsetLeft
                }); */

                // Show menu
                switch (settings.menuTransition) {

                    case 'fade':
                        options.fadeIn(settings.menuSpeed);
                        break;

                    case 'slide':
                        options.slideDown(settings.menuSpeed);
                        break;

                    default:
                        options.show(settings.menuSpeed);
                        break;

                }

                // Center on selected option
                var li = options.find('.selectBox-selected:first');
                if (li && li.length > 0) {
                    keepOptionInView(select, li, true);
                    addHover(select, li);
                }

                control.addClass('selectBox-menuShowing');

                $(document).bind('mousedown.selectBox', function (event) {
                    if ($(event.target).parents().andSelf().hasClass('selectBox-options')) return;
                    hideMenus();
                });

                updateSlider(select, control);
            };


            var hideMenus = function () {

                if ($(".selectBox-dropdown-menu").length === 0) return;
                $(document).unbind('mousedown.selectBox');

                $(".selectBox-dropdown-menu").each(function () {

                    var options = $(this),
                        select = options.data('selectBox-select'),
                        control = select.data('selectBox-control'),
                        settings = select.data('selectBox-settings');

                    switch (settings.menuTransition) {

                        case 'fade':
                            options.fadeOut(settings.menuSpeed);
                            break;

                        case 'slide':
                            options.slideUp(settings.menuSpeed);
                            break;

                        default:
                            options.hide(settings.menuSpeed);
                            break;

                    }

                    control.removeClass('selectBox-menuShowing');

                });

            };


            var selectOption = function (select, li, event) {
                li = $(li);
                var control = select.data('selectBox-control'),
                    settings = select.data('selectBox-settings');

                if (control.hasClass('selectBox-disabled')) return false;
                if (li.length === 0 || li.hasClass('selectBox-disabled')) return false;

                li.siblings().removeClass('selectBox-selected');
                li.addClass('selectBox-selected');

                control.find('.selectBox-label').html(li.html());

                // Update original control's value
                var selection = li.find('A').attr('rel');

                // Remember most recently selected item
                control.data('selectBox-last-selected', li);

                // Change callback
                if (select.val() !== selection) {
                    select.val(selection);
                    select.trigger('change');
                }

                updateSlider(select, control);

                return true;

            };


            var addHover = function (select, li) {
                li = $(li);
                var control = select.data('selectBox-control'),
                    options = control.data('selectBox-options');

                options.find('.selectBox-hover').removeClass('selectBox-hover');
                li.addClass('selectBox-hover');
            };


            var removeHover = function (select, li) {
                li = $(li);
                var control = select.data('selectBox-control'),
                    options = control.data('selectBox-options');
                options.find('.selectBox-hover').removeClass('selectBox-hover');
            };


            var keepOptionInView = function (select, li, center) {
                if (!li || li.length === 0) return;

                var control = select.data('selectBox-control'),
                    options = control.data('selectBox-options'),
                    scrollBox = options.find('.selectBox-dropdown-menu-list-container'),
                    top = parseInt(li.offset().top - scrollBox.position().top),
                    bottom = parseInt(top + li.outerHeight());

                if (center) {
                    scrollBox.scrollTop(li.offset().top - scrollBox.offset().top + scrollBox.scrollTop() - (scrollBox.height() / 2));
                } else {
                    if (top < 0) {
                        scrollBox.scrollTop(li.offset().top - scrollBox.offset().top + scrollBox.scrollTop());
                    }
                    if (bottom > scrollBox.height()) {
                        scrollBox.scrollTop((li.offset().top + li.outerHeight()) - scrollBox.offset().top + scrollBox.scrollTop() - scrollBox.height());
                    }
                }
            };


            var handleKeyDown = function (select, event) {
                var settings = select.data('selectBox-settings'),
                    control = select.data('selectBox-control'),
                    options = control.data('selectBox-options'),
                    totalOptions = 0,
                    i = 0;

                if (control.hasClass('selectBox-disabled')) return;

                switch (event.keyCode) {

                    case 8: // backspace
                        event.preventDefault();
                        typeSearch = '';
                        break;

                    case 9: // tab
                    case 27: // esc
                        hideMenus();
                        removeHover(select);
                        break;

                    case 13: // enter
                        if (control.hasClass('selectBox-menuShowing')) {
                            selectOption(select, options.find('LI.selectBox-hover:first'), event);
                            hideMenus();
                        } else {
                            showMenu(select, settings, control, options);
                        }
                        break;

                    case 38: // up
                    case 37: // left

                        event.preventDefault();

                        if (control.hasClass('selectBox-menuShowing')) {

                            var prev = options.find('.selectBox-hover').prev('LI');
                            totalOptions = options.find('LI:not(.selectBox-optgroup)').length;
                            i = 0;

                            while (prev.length === 0 || prev.hasClass('selectBox-disabled') || prev.hasClass('selectBox-optgroup')) {
                                prev = prev.prev('LI');
                                if (prev.length === 0) prev = options.find('LI:last');
                                if (++i >= totalOptions) break;
                            }

                            addHover(select, prev);
                            keepOptionInView(select, prev);
                            updateSlider(select, control);
                        } else {
                            showMenu(select, settings, control, options);
                        }

                        break;

                    case 40: // down
                    case 39: // right

                        event.preventDefault();

                        if (control.hasClass('selectBox-menuShowing')) {

                            var next = options.find('.selectBox-hover').next('LI');
                            totalOptions = options.find('LI:not(.selectBox-optgroup)').length;
                            i = 0;

                            while (next.length === 0 || next.hasClass('selectBox-disabled') || next.hasClass('selectBox-optgroup')) {
                                next = next.next('LI');
                                if (next.length === 0) next = options.find('LI:first');
                                if (++i >= totalOptions) break;
                            }

                            addHover(select, next);
                            keepOptionInView(select, next);
                            updateSlider(select, control);
                        } else {
                            showMenu(select, settings, control, options);
                        }

                        break;

                }

            };


            var handleKeyPress = function (select, event) {
                var settings = select.data('selectBox-settings'),
                    control = select.data('selectBox-control'),
                    options = control.data('selectBox-options');

                if (control.hasClass('selectBox-disabled')) return;

                switch (event.keyCode) {

                    case 9: // tab
                    case 27: // esc
                    case 13: // enter
                    case 38: // up
                    case 37: // left
                    case 40: // down
                    case 39: // right
                        // Don't interfere with the keydown event!
                        break;

                    default: // Type to find

                        if (!control.hasClass('selectBox-menuShowing')) showMenu(select, settings, control, options);

                        event.preventDefault();

                        clearTimeout(typeTimer);
                        typeSearch += String.fromCharCode(event.charCode || event.keyCode);

                        options.find('A').each(function () {
                            if ($(this).text().substr(0, typeSearch.length).toLowerCase() === typeSearch.toLowerCase()) {
                                addHover(select, $(this).parent());
                                keepOptionInView(select, $(this).parent());
                                updateSlider(select, control);
                                return false;
                            }
                        });

                        // Clear after a brief pause
                        typeTimer = setTimeout(function () { typeSearch = ''; }, 1000);

                        break;

                }

            };


            var enable = function (select) {
                select.attr('disabled', false);
                var control = select.data('selectBox-control');
                if (!control) return;
                control.removeClass('selectBox-disabled');
            };


            var disable = function (select) {
                select.attr('disabled', true);
                var control = select.data('selectBox-control');
                if (!control) return;
                control.addClass('selectBox-disabled');
            };

            var updateLabel = function (select) {
                var control = select.data('selectBox-control');
                if (!control) return;
                var settings = select.data('selectBox-settings');

                var newLabel;
                var optionSelected = $(select).find('OPTION:selected');
                if (optionSelected && optionSelected.length > 0 && settings && settings.labelFunction) {
                    newLabel = $('<span class="selectBox-label"><a rel="' + optionSelected.val() + '">' + settings.labelFunction(optionSelected.text()) + '</a></span>');
                } else {
                    newLabel = $('<span class="selectBox-label" />');
                    if (optionSelected && optionSelected.length > 0) {
                        newLabel.append($('<a rel="' + optionSelected.val() + '">' + (optionSelected.text() || '\u00A0') + '</a>'));
                    }
                }

                // focus fix for IE
                newLabel.focus(function () {
                    $(this).parent().focus();
                });

                control.find('.selectBox-label').before(newLabel).remove();
            };

            var setValue = function (select, value) {
                select.val(value);
                value = select.val();
                var control = select.data('selectBox-control');
                if (!control) return;
                var settings = select.data('selectBox-settings'),
                    options = control.data('selectBox-options');

                // Update label
                updateLabel(select);

                // Update control values
                options.find('.selectBox-selected').removeClass('selectBox-selected');
                options.find('A').each(function () {
                    if (typeof (value) === 'object') {
                        for (var i = 0; i < value.length; i++) {
                            if ($(this).attr('rel') == value[i]) {
                                $(this).parent().addClass('selectBox-selected');
                            }
                        }
                    } else {
                        if ($(this).attr('rel') == value) {
                            $(this).parent().addClass('selectBox-selected');
                        }
                    }
                });

                if (settings.change)
                    settings.change.call(select);
            };

            var setOptions = function (select, data) {
                var wrapper = select.data('selectBox-wrapper'),
                    control = select.data('selectBox-control'),
                    settings = select.data('selectBox-settings');

                switch (typeof (data)) {
                    case 'string':
                        select.html(data);
                        break;

                    case 'object':
                        select.html('');
                        for (var i in data) {
                            if (data[i] === null) continue;
                            var option = $('<option value="' + i + '">' + data[i] + '</option>');
                            select.append(option);
                        }
                        break;
                }

                if (!control) return;

                // Remove old options
                control.data('selectBox-options').remove();

                // Generate new options
                var options = getOptions(select);
                control.data('selectBox-options', options);

                wrapper.append(options);

                // Update label
                updateLabel(select);
            };


            var disableSelection = function (selector) {
                $(selector)
                    .css('MozUserSelect', 'none')
                    .bind('selectstart', function (event) {
                        event.preventDefault();
                    });
            };


            //
            // Public methods
            //


            switch (method) {

                case 'control':
                    return $(this).data('selectBox-control');
                    break;

                case 'settings':
                    if (!data) return $(this).data('selectBox-settings');
                    $(this).each(function () {
                        $(this).data('selectBox-settings', $.extend(true, $(this).data('selectBox-settings'), data));
                    });
                    break;

                case 'options':
                    $(this).each(function () {
                        setOptions($(this), data);
                    });
                    break;

                case 'value':
                    if (!data) return $(this).val();
                    $(this).each(function () {
                        setValue($(this), data);
                    });
                    break;

                case 'enable':
                    $(this).each(function () {
                        enable($(this));
                    });
                    break;

                case 'disable':
                    $(this).each(function () {
                        disable($(this));
                    });
                    break;

                case 'destroy':
                    $(this).each(function () {
                        destroy($(this));
                    });
                    break;

                default:
                    $(this).each(function () {
                        init($(this), $.extend(true, {
                            optionsOffsetTop: -5
                        }, method));
                    });
                    break;

            }

            return $(this);

        }

    });

})(jQuery);