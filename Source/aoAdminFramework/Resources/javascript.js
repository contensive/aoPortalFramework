//
// adminFramework assembly referenced js
//
if (typeof google !== 'object') {
    document.write('<scr' + 'ipt type="text/javascript" src="https://www.google.com/jsapi"></scr' + 'ipt>');
}
function cjFrameSubmitForm(remoteMethodName, frameHtmlId, formHtmlId) {
    jQuery('#' + frameHtmlId).block();
    cj.ajax.addon(
        remoteMethodName
        , 'frameName=' + frameHtmlId
        , formHtmlId
        , frameHtmlId
    );
}
function cjFrameUpdate(remoteMethodName, frameHtmlId, qs) {
    jQuery('#' + frameHtmlId).block();
    cj.ajax.addon(
        remoteMethodName
        , qs + '&frameName=' + frameHtmlId
        , ''
        , frameHtmlId
    );
}
//
// attach this to the contensive cj object
//
function cjFrame() {
    this.submitForm = cjFrameSubmitForm;
    this.update = cjFrameUpdate;
}
cj.frame = new cjFrame();
//
// legacy names
//
function afwPostFrame(remoteMethodName, formHtmlId, frameHtmlId) { cj.frame.submitForm(remoteMethodName, frameHtmlId, formHtmlId) }
function afwUpdateFrame(remoteMethodName, qs, frameHtmlId) { cj.frame.update(remoteMethodName, frameHtmlId, qs) }
