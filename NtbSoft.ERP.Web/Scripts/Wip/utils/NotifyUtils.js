(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Utils = WIP.Utils || {};

    function notify(message, type, displayTime) {
        if (window.DevExpress && DevExpress.ui && typeof DevExpress.ui.notify === "function") {
            DevExpress.ui.notify(message || "", type || "info", displayTime || 1800);
        }
    }

    WIP.Utils.Notify = {
        show: notify,
        success: function (message, displayTime) {
            notify(message, "success", displayTime || 1200);
        },
        warning: function (message, displayTime) {
            notify(message, "warning", displayTime || 1800);
        },
        error: function (message, displayTime) {
            notify(message, "error", displayTime || 3000);
        },
        fromError: function (prefix, error, displayTime) {
            var message = error && (error.message || error.responseText || error.statusText) || error || "";
            notify((prefix || "Loi") + ": " + message, "error", displayTime || 3000);
        }
    };
})(window);
