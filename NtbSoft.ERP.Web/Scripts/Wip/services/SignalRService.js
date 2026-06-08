(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class SignalRService {
        constructor(api) {
            this.api = api;
            this.timers = {};
            this.inflight = {};
            this.hub = null;
            this.poolService = null;
        }

        setPoolService(poolService) {
            this.poolService = poolService || null;
        }

        syncByMaDH(maDH, options) {
            return this.api.syncByMaDH({
                maDH: maDH
            }, options);
        }

        debounceSyncByMaDH(maDH, delayMs, syncHandler) {
            delayMs = delayMs || 250;
            maDH = String(maDH || "").trim();
            if (!maDH) return;

            if (this.timers[maDH]) clearTimeout(this.timers[maDH]);

            var self = this;
            this.timers[maDH] = setTimeout(function () {
                delete self.timers[maDH];
                if (typeof syncHandler === "function") {
                    syncHandler(maDH);
                }
            }, delayMs);
        }

        syncPoolByMaDH(maDH, patchHandler) {
            maDH = String(maDH || "").trim();
            if (!maDH || this.inflight[maDH]) return null;

            var self = this;
            this.inflight[maDH] = this.syncByMaDH(maDH, { cache: false, timeout: 20000 })
                .done(function (res) {
                    var items = (res && Array.isArray(res.items)) ? res.items : [];
                    var handler = patchHandler;
                    if (typeof handler !== "function" && self.poolService && typeof self.poolService.patchPoolByMaDH === "function") {
                        handler = function (key, rows) {
                            self.poolService.patchPoolByMaDH(key, rows);
                        };
                    }
                    if (typeof handler === "function") {
                        handler(maDH, items);
                    }
                })
                .fail(function (xhr) {
                    console.error("sync-by-madh failed:", maDH, xhr && (xhr.responseText || xhr.statusText));
                })
                .always(function () {
                    delete self.inflight[maDH];
                });

            return this.inflight[maDH];
        }

        init(options) {
            options = options || {};

            if (!window.$ || !$.connection || !$.connection.hub) {
                console.warn("SignalR not ready: missing /signalr/hubs or jquery.signalR");
                return false;
            }

            $.connection.hub.url = (options.baseUrl || window.location.origin) + "/signalr";

            var hub = $.connection.wipHub;
            this.hub = hub;

            var self = this;
            hub.client.orderCreated = function (msg) {
                var maDH = msg && (msg.maDH || msg.MaDH)
                    ? String(msg.maDH || msg.MaDH).trim()
                    : "";

                if (!maDH) return;
                self.debounceSyncByMaDH(maDH, options.delayMs || 250, options.onSyncByMaDH);
            };

            $.connection.hub.start({ withCredentials: true })
                .done(function () {
                    console.log("SignalR connected:", $.connection.hub.url);
                })
                .fail(function (err) {
                    console.error("SignalR connect failed", err);
                });

            return true;
        }

        cleanup() {
            var self = this;

            Object.keys(this.timers || {}).forEach(function (key) {
                if (self.timers[key]) {
                    clearTimeout(self.timers[key]);
                }
                delete self.timers[key];
            });

            Object.keys(this.inflight || {}).forEach(function (key) {
                var xhr = self.inflight[key];
                try {
                    if (xhr && xhr.readyState !== 4) xhr.abort();
                } catch (e) { }
                delete self.inflight[key];
            });

            if (window.$ && $.connection && $.connection.hub) {
                if (this.hub && this.hub.client) {
                    this.hub.client.orderCreated = null;
                }

                try {
                    $.connection.hub.stop();
                } catch (e) { }
            }

            this.hub = null;
        }
    }

    WIP.Services.SignalRService = SignalRService;
})(window);
