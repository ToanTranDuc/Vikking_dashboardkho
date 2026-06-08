(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Api = WIP.Api || {};

    class WipDonHangApi {
        constructor(ajaxClient, endpoints) {
            this.ajax = ajaxClient;
            this.endpoints = endpoints || {};
        }

        getWipData(params, options) {
            return this.ajax.get(this.endpoints.GET, params, options);
        }

        save(payload, options) {
            return this.ajax.postJson(this.endpoints.SAVE, payload, options);
        }

        saveDelta(payload, options) {
            return this.ajax.postJson(this.endpoints.SAVE_DELTA, payload, options);
        }

        saveDeltaFields(payload, options) {
            return this.ajax.postJson(this.endpoints.SAVE_DELTA_FIELDS, payload, options);
        }

        getSaveDeltaFields(params, options) {
            return this.ajax.get(this.endpoints.SAVE_DELTA_FIELDS, params, options);
        }

        saveByRequest(saveRequest, options) {
            var request = saveRequest || {};
            return this.ajax.postJson(request.url, request.payload, options);
        }

        reorder(payload, options) {
            return this.ajax.postJson(this.endpoints.REORDER, payload, options);
        }

        recalcLine(payload, options) {
            return this.ajax.postJson(this.endpoints.RECALC, payload, options);
        }

        getLineXList(params, options) {
            return this.ajax.get(this.endpoints.GET_LINEX, params, options);
        }

        getMdList(params, options) {
            return this.ajax.get(this.endpoints.GET_MD, params, options);
        }

        loadLine(params, options) {
            return this.ajax.get(this.endpoints.LOADLINE, params, options);
        }

        getUnassigned(params, options) {
            return this.ajax.get(this.endpoints.GET, $.extend({}, params || {}, {
                unassigned: true
            }), options);
        }

        assignToLine(payload, options) {
            return this.ajax.postJson(this.endpoints.ASSIGN, payload, options);
        }

        assignOutsource(payload, options) {
            return this.ajax.postJson(this.endpoints.ASSIGN_OUTSOURCE, payload, options);
        }

        unassignFromLine(payload, options) {
            return this.ajax.postJson(this.endpoints.UNASSIGN, payload, options);
        }

        syncByMaDH(params, options) {
            return this.ajax.get(this.endpoints.SYNC_BY_MADH, params, options);
        }

        syncLib(params, options) {
            return this.ajax.post(this.endpoints.SYNC_LIB, params, options);
        }

        getAudit(params, options) {
            return this.ajax.get(this.endpoints.AUDIT, params, options);
        }

        getHeaderColors(params, options) {
            return this.ajax.get(this.endpoints.HEADER_COLORS_GET, params, options);
        }

        updateHeaderColors(payload, options) {
            return this.ajax.postJson(this.endpoints.HEADER_COLORS_UPDATE, payload, options);
        }

        getStepStatusMeta(params, options) {
            return this.ajax.get(this.endpoints.STEP_STATUS_META, params, options);
        }

        getColumnDynamicMeta(params, options) {
            return this.ajax.get(this.endpoints.COLUMN_DYNAMIC, params, options);
        }

        getFieldPermissions(params, options) {
            return this.ajax.get(this.endpoints.FIELD_PERMISSION, params, options);
        }

        getFeaturePermissions(params, options) {
            return this.ajax.get(this.endpoints.FEATURE_PERMISSION, params, options);
        }
    }

    WIP.Api.WipDonHangApi = WipDonHangApi;
})(window);
