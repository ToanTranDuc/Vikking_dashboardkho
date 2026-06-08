(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    // Compatibility placeholder for the historical script path.
    // The actual legacy runtime now lives in core/WipRuntimeLegacyAdapter.js
    // and is started by index.js through WipDonHangPage.
    if (typeof WIP.Core.runLegacyWipDonHangRuntime !== "function") {
        WIP.Core.runLegacyWipDonHangRuntime = async function () {
            throw new Error("WipRuntimeLegacyAdapter.js is not loaded.");
        };
    }
})(window);
