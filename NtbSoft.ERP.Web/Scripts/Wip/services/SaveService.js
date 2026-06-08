(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class SaveService {
        constructor(api) {
            this.api = api;
        }

        saveByRequest(saveRequest, options) {
            return this.api.saveByRequest(saveRequest, options);
        }

        reorder(payload, options) {
            return this.api.reorder(payload, options);
        }

        updateHeaderColors(payload, options) {
            return this.api.updateHeaderColors(payload, options);
        }
    }

    WIP.Services.SaveService = SaveService;
})(window);
