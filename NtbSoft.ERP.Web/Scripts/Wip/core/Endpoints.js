(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    var configured = window.WIP_API || {};

    WIP.Core.endpoints = {
        URL: configured.URL || "/api/",
        GET: configured.GET || "/api/wip-donhang/get",
        SAVE: configured.SAVE || "/api/wip-donhang/save",
        GET_LINEX: configured.GET_LINEX || "/api/wip-donhang/get-list-linex",
        GET_MD: configured.GET_MD || "/api/wip-donhang/get-md",
        REORDER: configured.REORDER || "/api/wip-donhang/reorder",
        LOADLINE: configured.LOADLINE || "/api/wip-donhang/loadline",
        ASSIGN: configured.ASSIGN || "/api/wip-donhang/assign-to-line",
        ASSIGN_OUTSOURCE: configured.ASSIGN_OUTSOURCE || "/api/wip-donhang/assign-outsource",
        UNASSIGN: configured.UNASSIGN || "/api/wip-donhang/unassign-from-line",
        SYNC_BY_MADH: configured.SYNC_BY_MADH || "/api/wip-donhang/sync-by-madh",
        SYNC_LIB: configured.SYNC_LIB || "/api/wip-donhang/syncLib",
        AUDIT: configured.AUDIT || "/api/wip-donhang/audit",
        SAVE_DELTA: configured.SAVE_DELTA || "/api/wip-donhang/save-delta",
        SAVE_DELTA_FIELDS: configured.SAVE_DELTA_FIELDS || "/api/wip-donhang/save-delta-fields",
        RECALC: configured.RECALC || "/api/wip-donhang/recalc-line",
        STEP_STATUS_META: configured.STEP_STATUS_META || "/api/wip-donhang/step-status-meta",
        COLUMN_DYNAMIC: configured.COLUMN_DYNAMIC || "/api/wip-donhang/column-dynamic",
        HEADER_COLORS_GET: configured.HEADER_COLORS_GET || "/api/wip-donhang/header-colors",
        HEADER_COLORS_UPDATE: configured.HEADER_COLORS_UPDATE || "/api/wip-donhang/header-colors/update",
        FEATURE_PERMISSION: configured.FEATURE_PERMISSION || "/api/PhanQuyenWIP/Feature",
        FIELD_PERMISSION: configured.FIELD_PERMISSION || "/api/PhanQuyenWIP/GetFieldPermissions"
    };
})(window);
