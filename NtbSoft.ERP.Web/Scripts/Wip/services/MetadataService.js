(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class MetadataService {
        constructor(api) {
            this.api = api;
        }

        loadColumnDynamicMeta() {
            return this.api.getColumnDynamicMeta({}, { cache: false });
        }

        loadSaveDeltaFieldMeta() {
            return this.api.getSaveDeltaFields({}, { cache: false });
        }

        loadStepStatusMeta() {
            return this.api.getStepStatusMeta({}, { cache: false });
        }
    }

    WIP.Services.MetadataService = MetadataService;
})(window);
