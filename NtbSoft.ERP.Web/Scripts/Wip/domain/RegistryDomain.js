(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Domain = WIP.Domain || {};

    function normalizeRegistryKey(value) {
        return String(value == null ? "" : value).trim().toLowerCase();
    }

    function inferBindingRole(field, stepMeta) {
        if (!stepMeta) return "STEP_EXTRA";

        var f = normalizeRegistryKey(field);
        var kh = normalizeRegistryKey(stepMeta.KHField);
        var tt = normalizeRegistryKey(stepMeta.TTField);

        if (f && kh && f === kh) return "KH";
        if (f && tt && f === tt) return "TT";
        return "STEP_EXTRA";
    }

    function buildStepRegistry(meta) {
        var registry = {
            stepMetaByCode: Object.create(null),
            stepMetaByKhField: Object.create(null),
            stepMetaByTtField: Object.create(null),
            comparableTtFieldMap: Object.create(null)
        };

        var steps = (meta && Array.isArray(meta.steps)) ? meta.steps : [];
        var compareTtColumns = (meta && Array.isArray(meta.compareTtColumns)) ? meta.compareTtColumns : [];

        steps.forEach(function (step) {
            if (!step || typeof step !== "object") return;

            var codeKey = normalizeRegistryKey(step.StepCode);
            var khKey = normalizeRegistryKey(step.KHField);
            var ttKey = normalizeRegistryKey(step.TTField);

            if (codeKey) registry.stepMetaByCode[codeKey] = step;
            if (khKey) registry.stepMetaByKhField[khKey] = step;
            if (ttKey) registry.stepMetaByTtField[ttKey] = step;
        });

        compareTtColumns.forEach(function (field) {
            var key = normalizeRegistryKey(field);
            if (key) registry.comparableTtFieldMap[key] = true;
        });

        return registry;
    }

    WIP.Domain.Registry = {
        normalizeRegistryKey: normalizeRegistryKey,
        inferBindingRole: inferBindingRole,
        buildStepRegistry: buildStepRegistry
    };
})(window);
