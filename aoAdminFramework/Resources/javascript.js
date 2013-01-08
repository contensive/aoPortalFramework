//
// adminFramework javascript
//
function afwUpdateFrame( remoteMethodName, qs, frameHtmlId ) {
		jQuery('#'+frameHtmlId).block();
		cj.ajax.addon(
			remoteMethodName
			,qs
			,''
			,frameHtmlId
			)
}
