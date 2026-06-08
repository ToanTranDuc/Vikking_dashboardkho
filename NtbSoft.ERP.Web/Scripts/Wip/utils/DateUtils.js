(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Utils = WIP.Utils || {};

    function pad2(value) {
        return String(value).padStart(2, "0");
    }

    function toYMD(date) {
        if (!(date instanceof Date) || isNaN(date.getTime())) return "";
        return date.getFullYear() + "-" + pad2(date.getMonth() + 1) + "-" + pad2(date.getDate());
    }

    function normalizeDateOnly(value) {
        if (value == null || value === "") return null;
        if (value instanceof Date) return toYMD(value);

        var text = String(value).trim();
        if (!text) return null;

        var ymdMatch = text.match(/^(\d{4})-(\d{2})-(\d{2})/);
        if (ymdMatch) return ymdMatch[1] + "-" + ymdMatch[2] + "-" + ymdMatch[3];

        var dmyMatch = text.match(/^(\d{1,2})\/(\d{1,2})\/(\d{2,4})$/);
        if (dmyMatch) {
            var year = Number(dmyMatch[3]);
            if (year < 100) year += 2000;
            return year + "-" + pad2(dmyMatch[2]) + "-" + pad2(dmyMatch[1]);
        }

        var parsed = new Date(value);
        return isNaN(parsed.getTime()) ? null : toYMD(parsed);
    }

    function todayYMD() {
        return toYMD(new Date());
    }

    function isEmptyDateValue(value) {
        return value == null || value === "" || value === "0001-01-01T00:00:00" || value === "1900-01-01T00:00:00";
    }

    WIP.Utils.Date = {
        pad2: pad2,
        toYMD: toYMD,
        todayYMD: todayYMD,
        normalizeDateOnly: normalizeDateOnly,
        isEmptyDateValue: isEmptyDateValue
    };
})(window);
