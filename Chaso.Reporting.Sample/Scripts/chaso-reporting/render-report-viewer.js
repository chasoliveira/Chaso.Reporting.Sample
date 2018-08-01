var ReportViewerForMvc = (new function () {

    var _iframeId = null;
    var _iframe = null;

    var resizeIframe = function (msg) {
        var height = msg.source.document.body.scrollHeight;
        //var width = msg.source.document.body.scrollWidth;

        var iframe = ReportViewerForMvc.getIframe();
        $(iframe).removeAttr("style")
        $(ReportViewerForMvc.getIframeId()).height(height);
        $(ReportViewerForMvc.getIframeId()).width("100%");
    }

    var addEvent = function (element, eventName, eventHandler) {
        if (element.addEventListener) {
            element.addEventListener(eventName, eventHandler);
        } else if (element.attachEvent) {
            element.attachEvent('on' + eventName, eventHandler);
        }
    }

    this.setIframeId = function (value) {

    };

    this.getIframeId = function () {
        return _iframeId || (_iframeId = "#" + $("iframe").attr("id"));
    };
    this.getIframe = function () {
        return _iframe || (_iframe = $("iframe"));
    };
    this.setAutoSize = function () {
        addEvent(window, 'message', resizeIframe);
    }

}());

ReportViewerForMvc.setAutoSize();