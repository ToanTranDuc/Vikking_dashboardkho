(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    class WipDonHangPage {
        constructor(dependencies) {
            this.dependencies = dependencies || {};
        }

        async init() {
            if (this.initialized) return;
            this.initialized = true;

            if (WIP.Core && typeof WIP.Core.runLegacyWipDonHangRuntime === "function") {
                await WIP.Core.runLegacyWipDonHangRuntime(this.dependencies);
                return;
            }

            throw new Error("Wip legacy runtime is not registered.");
        }

        destroy() {
            var signalr = this.dependencies.signalr;
            if (signalr && typeof signalr.cleanup === "function") {
                signalr.cleanup();
            }
        }
    }

    WIP.Core.WipDonHangPage = WipDonHangPage;
})(window);
