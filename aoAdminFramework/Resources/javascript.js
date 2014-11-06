//
// adminFramework javascript
//  reference to google jsapi moved to admin framework resources addon
//  necessary to prevent duplicate reference error
//
// document.write('<scr' + 'ipt type="text/javascript" src="https://www.google.com/jsapi"></scr' + 'ipt>')
//
if (typeof cjFrameSubmitForm != 'function') {
alert( 'This feature is dependent on the Admin Framework Resources addon from the Addon Framework Collection. It does not appear to be installed correctly.' )
}
function cjFrameSubmitForm(remoteMethodName, frameHtmlId, formHtmlId) {
    jQuery('#' + frameHtmlId).block();
    cj.ajax.addon(
			remoteMethodName
			, 'frameName=' + frameHtmlId
			, formHtmlId
			, frameHtmlId
			)
}
function cjFrameUpdate(remoteMethodName, frameHtmlId, qs) {
    jQuery('#' + frameHtmlId).block();
    cj.ajax.addon(
			remoteMethodName
			, qs + '&frameName=' + frameHtmlId
			, ''
			, frameHtmlId
			)
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
