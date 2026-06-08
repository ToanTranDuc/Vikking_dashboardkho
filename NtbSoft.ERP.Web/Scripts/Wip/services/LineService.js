(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class LineService {
        constructor(api) {
            this.api = api;
        }

        loadLineXList(params, options) {
            return this.api.getLineXList(params || {}, options);
        }

        loadLine(params, options) {
            return this.api.loadLine(params, options);
        }

        syncLibByLine(lineX, options) {
            if (!lineX) return null;
            return this.api.syncLib({ LineX: lineX }, options);
        }
    }

    WIP.Services.LineService = LineService;
})(window);
