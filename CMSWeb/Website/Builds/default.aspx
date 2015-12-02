<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta charset="utf-8" />
        <title>Advisor App Build Site</title>
        <script src="/builds/jquery203min.js"></script>
        <script src="/builds/index.js"></script>
        <link rel="stylesheet" type="text/css" href="/builds/fonts.css" />
        <link rel="stylesheet" type="text/css" href="/builds/index.css" />
    </head>
    <body>
        <form>
            <div id="uploadBuildDialog" class="modalPopup">
                <div class="modalBackdrop"></div>
                <div class="modalDialogFrame">
                    <div class="modalNavigationBar">
                        <div class="modalButton left modalCancel">Cancel</div>
                        <div class="modalButton right" id="uploadBuildSave">Save</div>
                        <div class="modalTitle">Upload Build</div>
                    </div>
                    <div class="modalContent">
                        <div class="uploadBuildLine">
                            <div class="uploadBuildLabel">Build name</div>
                            <input id="uploadBuildName" class="uploadBuildEdit" type="text" />
                        </div>
                        <div class="uploadBuildLine">
                            <div class="uploadBuildLabel">IPA file</div>
                            <input id="uploadBuildFile" class="uploadBuildFile" type="file" />
                        </div>
                    </div>
                </div>
            </div>
            <div id="editBuildDialog" class="modalPopup">
                <div class="modalBackdrop"></div>
                <div class="modalDialogFrame">
                    <div class="modalNavigationBar">
                        <div class="modalButton left modalCancel">Cancel</div>
                        <div class="modalButton right" id="editBuildSave">Save</div>
                        <div class="modalTitle">Edit Build</div>
                    </div>
                    <div class="modalContent">
                        <div class="uploadBuildLine">
                            <div class="uploadBuildLabel">Build name</div>
                            <input id="editBuildName" class="uploadBuildEdit" type="text" />
                        </div>
                        <div id="editDelete" class="uploadDelete">Delete Build</div>
                    </div>
                </div>
            </div>
            <div class="banner">
                <div class="bannerInner">
                    <div class="employeeOnly">Employee Only</div>
                </div>
            </div>
            <div class="sections">
                <div class="sectionHeader">Advisor App Builds</div>
                <div class="sectionGroup">
                    <div class="sectionPanel">
                        <div id="ipaItems" class="ipaItems"></div>
                        <div id="ipaItemTemplate" class="ipaItem template">
                            <div class="ipaButton ipaInstall">Install</div>
                            <div class="ipaName"></div>
                        </div>
                        <div id="ipaUpload" class="ipaUpload">Upload Build</div>
                    </div>
                </div>
            </div>
        </form>
    </body>
</html>
