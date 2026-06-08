(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class RecalcService {
        constructor(api, options) {
            this.api = api;
            this.options = options || {};
        }

        buildRecalcLinePayload(context) {
            var ctx = context || {};
            return {
                lineX: ctx.lineX,
                fromThuTu: ctx.fromThuTu || 1,
                modifyBy: ctx.modifyBy || null,
                isKetThuc: ctx.isKetThuc ? 1 : 0
            };
        }

        recalcLine(payload, options) {
            return this.api.recalcLine(payload, options);
        }

        recalcLineForContext(context, options) {
            return this.recalcLine(this.buildRecalcLinePayload(context), options);
        }
    }

    WIP.Services.RecalcService = RecalcService;
})(window);
