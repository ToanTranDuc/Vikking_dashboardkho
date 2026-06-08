(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    class PublicApiAdapter {
        constructor(target) {
            this.target = target || WIP;
        }

        register(methods) {
            var target = this.target;
            Object.keys(methods || {}).forEach(function (name) {
                if (typeof methods[name] === "function") {
                    target[name] = methods[name];
                }
            });
            return target;
        }
    }

    WIP.Core.PublicApiAdapter = PublicApiAdapter;
})(window);
