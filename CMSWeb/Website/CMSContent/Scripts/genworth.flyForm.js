var Genworth = Genworth || {};

Genworth.flyForm = function (data) {

    data = $.extend({
        method: "POST",
        target: "_blank"
    }, data);

    dForm = $('<form action="' + data.ActionUrl + '" style="display:none;" target="' + data.target + '" method="' + data.method + '"></form>');

    $.each(data.Fields, function () {
        $('<input type="text" name="' + this.Key + '" value="' + this.Value + '">').appendTo(dForm);
    });

    dForm.prependTo('body').submit();
};