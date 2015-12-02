
function cloneTemplate(query) {
    var clone = $(query).clone();
    clone.prop("id", "");
    clone.removeClass("template");
    return clone;
}

function showDialog(query) {
    $(query).fadeIn();
}

function hideDialog(query) {
    $(query).fadeOut();
}

function addItemView(item) {
    var ipaItem = cloneTemplate("#ipaItemTemplate");
    $(".ipaName", ipaItem).text(item.name);

    $(".ipaName", ipaItem).on('click', function (e) {
        editItemClicked(item, $(this).closest('.ipaItem'));
    });

    $(".ipaInstall", ipaItem).on('click', function (e) {
        window.location.href = "itms-services://?action=download-manifest&url=itms-services://?action=download-manifest&url=https://assetmark.xcentium.net/builds/" + item.plist;
    });

    $("#ipaItems").append(ipaItem);
}

function updateViewItems(items) {
    $("#ipaItems").empty();
    items.forEach(addItemView);
}

function uploadClicked() {
    $(uploadBuildName).val("");
    $(uploadBuildFile).val("");
    showDialog("#uploadBuildDialog");
    $(uploadBuildName).focus();
}

function uploadBuildSave() {
    var name = $(uploadBuildName).val();
    hideDialog("#uploadBuildDialog");
    // To do: upload here
    addItemView({
        name: name,
        plist: encodeURI(name)
    });
}

function editItemClicked(item, view) {
    editItem = { view: view, item: item };
    $("#editBuildName").val(item.name);
    showDialog("#editBuildDialog");
}

function editDelete() {
    editItem.view.remove();
    editItem = null;
    hideDialog("#editBuildDialog");
}

function editBuildSave() {
    editItem = null;
    hideDialog("#editBuildDialog");
}

$(document).ready(function () {

    $("form").on('submit', function (e) { e.preventDefault(); });
    $(".modalBackdrop").on('click', function () { $(this).closest(".modalPopup").fadeOut(); });
    $(".modalCancel").on('click', function () { $(this).closest(".modalPopup").fadeOut(); });

    $("#ipaUpload").on('click', uploadClicked);
    $("#uploadBuildSave").on('click', uploadBuildSave);
    $("#editDelete").on('click', editDelete);

    updateViewItems([
        { name: "AdvisorApp-1.7-prod-1", plist: "AdvisorApp-1.7-prod-1.plist" },
        { name: "AdvisorApp-1.7-staging-1", plist: "AdvisorApp-1.7-staging-1.plist" },
        { name: "AdvisorApp-1.7-dev-1-auth", plist: "AdvisorApp-1.7-dev-1-auth.plist" },
        { name: "AdvisorApp-1.7-dev-1-noauth", plist: "AdvisorApp-1.7-dev-1-noauth.plist" },
    ]);

});
