(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Domain = WIP.Domain || {};

    function normalizeCaption(value) {
        return String(value || "").replace(/\s+/g, " ").trim();
    }

    function normalizeDateOnlyForRange(value) {
        return WIP.Utils.Date.normalizeDateOnly(value);
    }

    function isActive(dateFilter) {
        return !!(dateFilter && dateFilter.field && (dateFilter.range[0] || dateFilter.range[1]));
    }

    function rowMatches(row, dateFilter) {
        if (!isActive(dateFilter)) return true;
        if (!row) return false;

        var field = dateFilter.field;
        var rowDate = normalizeDateOnlyForRange(row[field]);
        if (!rowDate) return false;

        var fromDate = normalizeDateOnlyForRange(dateFilter.range[0]);
        var toDate = normalizeDateOnlyForRange(dateFilter.range[1]);

        if (fromDate && rowDate < fromDate) return false;
        if (toDate && rowDate > toDate) return false;
        return true;
    }

    function apply(dataSource, dateFilter) {
        var list = Array.isArray(dataSource) ? dataSource : [];
        if (!isActive(dateFilter)) return list;
        return list.filter(function (row) {
            return rowMatches(row, dateFilter);
        });
    }

    WIP.Domain.DateFilter = {
        normalizeCaption: normalizeCaption,
        normalizeDateOnlyForRange: normalizeDateOnlyForRange,
        isActive: isActive,
        rowMatches: rowMatches,
        apply: apply
    };
})(window);
