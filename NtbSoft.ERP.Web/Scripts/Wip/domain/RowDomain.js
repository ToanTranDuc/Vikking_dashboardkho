(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Domain = WIP.Domain || {};

    function mapRowId(row) {
        if (!row) return row;

        if (row.id == null && row.WIPId != null) {
            row.id = row.WIPId;
        }
        if (row.WIPId == null && row.id != null) {
            row.WIPId = row.id;
        }

        return row;
    }

    function hydrateWipRow(row, nextIdProvider) {
        if (!row) return row;

        row.id = (row.WIPId && row.WIPId > 0)
            ? row.WIPId
            : (row.id || (typeof nextIdProvider === "function" ? nextIdProvider() : row.id));

        row.WIPId = row.WIPId || row.id;
        return row;
    }

    function hydrateWipRows(items, nextIdProvider) {
        return (items || []).map(function (row) {
            return hydrateWipRow(row, nextIdProvider);
        });
    }

    function mapRows(rows) {
        return (Array.isArray(rows) ? rows : []).map(function (row) {
            return mapRowId($.extend(true, {}, row));
        });
    }

    function getWipIdKey(row) {
        return row && (row.WIPId || row.id) ? String(row.WIPId || row.id) : "";
    }

    function getWipIdObject(row) {
        return { WIPId: row && row.WIPId };
    }

    function normalizeResponse(response, mode) {
        mode = mode || "single";

        if (mode === "assign" && response && response.lineItems) {
            return {
                lineItems: response.lineItems,
                poolItems: response.poolItems || null
            };
        }

        if (response && response.items) return response.items;
        return Array.isArray(response) ? response : (response ? [response] : []);
    }

    function upsertRows(targetArray, items) {
        var target = Array.isArray(targetArray) ? targetArray : [];
        var rows = Array.isArray(items) ? items : [];

        rows.forEach(function (item) {
            var key = getWipIdKey(item);
            if (!key) return;

            var index = target.findIndex(function (row) {
                return getWipIdKey(row) === key;
            });

            if (index >= 0) target[index] = item;
            else target.push(item);
        });

        return target;
    }

    WIP.Domain.Row = {
        mapRowId: mapRowId,
        hydrateWipRow: hydrateWipRow,
        hydrateWipRows: hydrateWipRows,
        mapRows: mapRows,
        getWipIdKey: getWipIdKey,
        getWipIdObject: getWipIdObject,
        normalizeResponse: normalizeResponse,
        upsertRows: upsertRows
    };
})(window, jQuery);
