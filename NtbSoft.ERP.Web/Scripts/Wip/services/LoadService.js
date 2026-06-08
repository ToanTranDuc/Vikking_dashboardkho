(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class LoadService {
        constructor(api) {
            this.api = api;
        }

        loadWipData(params, options) {
            return this.api.getWipData(params, options);
        }

        loadUnassignedLine(params, options) {
            return this.api.loadLine(params, options);
        }

        loadAudit(params, options) {
            return this.api.getAudit(params, options);
        }

        loadMdList(params, options) {
            return this.api.getMdList(params, options);
        }

        loadHeaderColors(params, options) {
            return this.api.getHeaderColors(params, options);
        }

        fetchLibrary(apiUrl, params, options) {
            return this.api.ajax.get(apiUrl, params || {}, options);
        }
    }

    WIP.Services.LoadService = LoadService;
})(window);
