(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Domain = WIP.Domain || {};

    function isCalcField(field, calcFields) {
        return Array.isArray(calcFields) && calcFields.indexOf(field) >= 0;
    }

    function getFieldMaxLength(field, maxLengthMap) {
        var maxLength = maxLengthMap && maxLengthMap[field];
        return Number(maxLength || 0) || null;
    }

    function isInputHintField(field, inputHintFields) {
        return Array.isArray(inputHintFields) && inputHintFields.indexOf(field) >= 0;
    }

    function isNonNegField(field, nonNegFields) {
        return !!(nonNegFields && typeof nonNegFields.has === "function" && nonNegFields.has(field));
    }

    WIP.Domain.Validation = {
        isCalcField: isCalcField,
        getFieldMaxLength: getFieldMaxLength,
        isInputHintField: isInputHintField,
        isNonNegField: isNonNegField
    };
})(window);
