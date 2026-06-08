(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Api = WIP.Api || {};

    class AjaxClient {
        request(options) {
            return $.ajax(options);
        }

        get(url, data, options) {
            return this.request($.extend(true, {
                url: url,
                method: "GET",
                dataType: "json",
                data: data || {}
            }, options || {}));
        }

        post(url, data, options) {
            return this.request($.extend(true, {
                url: url,
                method: "POST",
                data: data || {}
            }, options || {}));
        }

        postJson(url, payload, options) {
            return this.request($.extend(true, {
                url: url,
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify(payload || {})
            }, options || {}));
        }
    }

    WIP.Api.AjaxClient = AjaxClient;
})(window, jQuery);
