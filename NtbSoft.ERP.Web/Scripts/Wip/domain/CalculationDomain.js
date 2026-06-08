(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Domain = WIP.Domain || {};

    function normalizeCompareValue(value, toYMD) {
        if (value === undefined || value === "" || value === null) return null;

        if (value instanceof Date) {
            return typeof toYMD === "function" ? toYMD(value) : value.toISOString().slice(0, 10);
        }

        if (typeof value === "string") {
            var text = value.trim();
            if (text === "") return null;

            var numberValue = Number(text.replaceAll(",", ""));
            if (Number.isFinite(numberValue) && String(numberValue) !== "NaN") return numberValue;

            return text;
        }

        if (typeof value === "number" || typeof value === "boolean") return value;

        return String(value);
    }

    function isFieldValueChanged(oldRow, newRow, field, toYMD) {
        if (!field) return false;

        var normalizedField = String(field).replace("__libId_", "");
        var oldValue = normalizeCompareValue(oldRow ? oldRow[normalizedField] : null, toYMD);
        var newValue = normalizeCompareValue(newRow ? newRow[normalizedField] : null, toYMD);

        return String(oldValue == null ? "" : oldValue) !== String(newValue == null ? "" : newValue);
    }

    function getActuallyChangedFields(oldRow, newRow, changedFieldsObj, toYMD) {
        var actual = {};

        Object.keys(changedFieldsObj || {}).forEach(function (field) {
            var normalizedField = String(field || "").replace("__libId_", "");
            if (!normalizedField) return;

            if (isFieldValueChanged(oldRow, newRow, normalizedField, toYMD)) {
                actual[normalizedField] = true;
            }
        });

        return actual;
    }

    function hasAnyRecalcChange(oldRow, newRow, recalcFields, toYMD) {
        oldRow = oldRow || {};
        newRow = newRow || {};

        return (recalcFields || []).some(function (field) {
            return isFieldValueChanged(oldRow, newRow, field, toYMD);
        });
    }

    function attachRecalcFlag(oldRow, newRow, recalcFields, toYMD) {
        var needRecalc = hasAnyRecalcChange(oldRow, newRow, recalcFields, toYMD);
        newRow.SkipCalc = needRecalc ? 0 : 1;
        return newRow;
    }

    WIP.Domain.Calculation = {
        normalizeCompareValue: normalizeCompareValue,
        isFieldValueChanged: isFieldValueChanged,
        getActuallyChangedFields: getActuallyChangedFields,
        hasAnyRecalcChange: hasAnyRecalcChange,
        attachRecalcFlag: attachRecalcFlag
    };
})(window);
