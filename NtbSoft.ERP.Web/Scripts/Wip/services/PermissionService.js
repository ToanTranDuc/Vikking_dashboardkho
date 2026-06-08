(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class PermissionService {
        constructor(api) {
            this.api = api;
        }

        loadFieldPermissions(userName) {
            return this.api.getFieldPermissions({
                userID: userName
            });
        }

        loadFeaturePermissions(userName) {
            return this.api.getFeaturePermissions({
                action: "GET_ALLOWED_FEATURE_KEYS",
                userID: userName
            });
        }
    }

    WIP.Services.PermissionService = PermissionService;
})(window);
