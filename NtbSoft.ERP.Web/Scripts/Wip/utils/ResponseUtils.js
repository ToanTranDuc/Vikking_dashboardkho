(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Utils = WIP.Utils || {};

    function normalizeList(response) {
        if (Array.isArray(response)) return response;
        if (!response || typeof response !== "object") return [];

        if (Array.isArray(response.data)) return response.data;
        if (Array.isArray(response.Data)) return response.Data;
        if (Array.isArray(response.items)) return response.items;
        if (Array.isArray(response.Items)) return response.Items;
        if (Array.isArray(response.rows)) return response.rows;
        if (Array.isArray(response.Rows)) return response.Rows;

        return [];
    }

    function isSuccessResponse(response) {
        if (response === true) return true;
        if (!response || typeof response !== "object") return false;
        if (response.success === true || response.Success === true) return true;
        if (response.ok === true || response.Ok === true) return true;
        return false;
    }

    function getMessage(response, fallback) {
        if (!response || typeof response !== "object") return fallback || "";
        return response.message || response.Message || response.error || response.Error || fallback || "";
    }

    WIP.Utils.Response = {
        normalizeList: normalizeList,
        isSuccessResponse: isSuccessResponse,
        getMessage: getMessage
    };
})(window);
