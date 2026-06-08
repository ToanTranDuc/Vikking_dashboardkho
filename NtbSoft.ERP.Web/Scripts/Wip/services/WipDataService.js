(function (window, $) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Services = WIP.Services || {};

    class WipDataService {
        constructor(options) {
            this.options = options || {};
        }

        normalizeKey(value) {
            if (typeof this.options.normalizeRegistryKey === "function") {
                return this.options.normalizeRegistryKey(value);
            }
            return String(value == null ? "" : value).trim().toLowerCase();
        }

        getDeltaParentFieldMap() {
            return this.options.getDeltaParentFieldMap ? (this.options.getDeltaParentFieldMap() || {}) : {};
        }

        normalizeDateOnly(row) {
            Object.keys(row || {}).forEach(function (key) {
                var value = row[key];
                if (value instanceof Date) {
                    var yyyy = value.getFullYear();
                    var mm = String(value.getMonth() + 1).padStart(2, "0");
                    var dd = String(value.getDate()).padStart(2, "0");
                    row[key] = yyyy + "-" + mm + "-" + dd;
                }
            });
            return row;
        }

        buildChangedFieldMap(changedFieldsObj) {
            var map = Object.create(null);
            var self = this;
            Object.keys(changedFieldsObj || {}).forEach(function (field) {
                var normalized = String(field || "").replace("__libId_", "").trim();
                if (normalized) map[self.normalizeKey(normalized)] = true;
            });
            return map;
        }

        normalizeSaveRow(newRow, oldRow, changedFieldsObj) {
            var row = $.extend(true, {}, newRow || {});
            var baseRow = oldRow || {};

            Object.keys(row).forEach(function (key) {
                if (typeof row[key] === "string") {
                    var trimmed = row[key].trim();
                    row[key] = trimmed === "" ? null : trimmed;
                }
            });

            (this.options.fixedFields || []).forEach(function (key) {
                row[key] = baseRow[key];
            });

            this.normalizeDateOnly(row);
            return row;
        }

        parseManualFlags(row) {
            if (typeof this.options.parseManualFlags === "function") {
                return this.options.parseManualFlags(row);
            }

            if (!row || !row.ManualJson) return {};
            try { return JSON.parse(row.ManualJson) || {}; } catch (e) { return {}; }
        }

        isEqualValue(a, b) {
            if (a === "") a = null;
            if (b === "") b = null;
            return String(a == null ? "" : a) === String(b == null ? "" : b);
        }

        buildManualFlagsPatch(oldRow, newRow) {
            var patch = $.extend(true, {}, this.parseManualFlags(oldRow));
            (this.options.manualFields || []).forEach((field) => {
                if (!this.isEqualValue(oldRow && oldRow[field], newRow && newRow[field])) {
                    patch[field] = true;
                }
            });
            return patch;
        }

        attachKeyJson(oldRow, newRow) {
            newRow.ManualJsonPatch = JSON.stringify(this.buildManualFlagsPatch(oldRow, newRow));
            return newRow;
        }

        resolveNoDataStepMeta(field) {
            var raw = String(field || "").trim();
            if (!raw || raw.toLowerCase().slice(-7) !== "_nodata") return null;

            var ttField = raw.slice(0, -7);
            var meta = this.options.getStepMetaByTtField ? this.options.getStepMetaByTtField(ttField) : null;
            if (!meta || meta.AllowNoDataConfirm !== true) return null;

            var expected = String(meta.NoDataField || (String(meta.TTField || ttField).trim() + "_NoData")).trim();
            return this.normalizeKey(expected) === this.normalizeKey(raw) ? meta : null;
        }

        isDeltaSaveSupportedField(field) {
            var key = this.normalizeKey(field);
            if (!key) return false;
            if (this.getDeltaParentFieldMap()[key]) return true;
            if (this.options.getStepMetaByKhField && this.options.getStepMetaByKhField(field)) return true;
            if (this.options.getStepMetaByTtField && this.options.getStepMetaByTtField(field)) return true;
            return !!this.resolveNoDataStepMeta(field);
        }

        shouldSendDeltaManualPatch(field) {
            var khMeta = this.options.getStepMetaByKhField ? this.options.getStepMetaByKhField(field) : null;
            if (khMeta) return khMeta.AllowManualKH !== false;

            var ttMeta = this.options.getStepMetaByTtField ? this.options.getStepMetaByTtField(field) : null;
            if (ttMeta) return true;

            var parentMap = this.getDeltaParentFieldMap();
            var supportsManual = this.options.isManualSupportedField
                ? this.options.isManualSupportedField(field)
                : false;
            if (parentMap[this.normalizeKey(field)] && supportsManual) return true;

            return false;
        }

        buildDeltaChangesFromRow(row, changedFieldsObj) {
            var changes = [];
            Object.keys(changedFieldsObj || {}).forEach(function (field) {
                var normalized = String(field || "").replace("__libId_", "").trim();
                if (!normalized) return;
                if (field.indexOf("__libId_") === 0) return;

                changes.push({
                    field: normalized,
                    value: row[normalized] === undefined ? null : row[normalized]
                });
            });
            return changes;
        }

        buildDeltaManualPatch(oldRow, newRow, changedFieldsObj) {
            var patch = {};
            Object.keys(changedFieldsObj || {}).forEach((field) => {
                var normalized = String(field || "").replace("__libId_", "").trim();
                if (!normalized || field.indexOf("__libId_") === 0) return;
                if (!this.shouldSendDeltaManualPatch(normalized)) return;
                if (!this.options.isFieldValueChanged(oldRow, newRow, normalized)) return;

                patch[normalized] = true;
            });
            return patch;
        }

        canUseSaveDelta(changes) {
            if (!changes || !changes.length) return false;
            return changes.every((change) => {
                return change && this.isDeltaSaveSupportedField(change.field);
            });
        }

        getUnsupportedSaveDeltaFields(changes) {
            return (changes || []).filter((change) => {
                return !change || !this.isDeltaSaveSupportedField(change.field);
            }).map(function (change) {
                return change && change.field ? change.field : "";
            }).filter(Boolean);
        }

        buildSavePayload(newRow, oldRow, changedFieldsObj) {
            var baseRow = oldRow || {};
            var row = this.normalizeSaveRow(newRow, baseRow, changedFieldsObj);

            this.attachKeyJson(baseRow, row);
            if (typeof this.options.attachRecalcFlag === "function") {
                this.options.attachRecalcFlag(baseRow, row);
            }

            return {
                row: row,
                changes: this.options.buildChangesFromRow
                    ? this.options.buildChangesFromRow(row, changedFieldsObj)
                    : this.buildDeltaChangesFromRow(row, changedFieldsObj),
                modifyBy: this.options.getUserName ? this.options.getUserName() : null
            };
        }

        buildSaveDeltaPayload(newRow, oldRow, changedFieldsObj) {
            var baseRow = oldRow || {};
            var row = this.normalizeSaveRow(newRow, baseRow, changedFieldsObj);
            var changes = this.buildDeltaChangesFromRow(row, changedFieldsObj);

            return {
                wipId: Number(row.WIPId || baseRow.WIPId || row.id || baseRow.id || 0),
                lineX: row.LineX || baseRow.LineX || null,
                changes: changes,
                manualPatch: this.buildDeltaManualPatch(baseRow, row, changedFieldsObj),
                modifyBy: this.options.getUserName ? this.options.getUserName() : null,
                isKetThuc: this.options.getShowEnded && this.options.getShowEnded() ? 1 : 0
            };
        }

        buildSaveRequest(newRow, oldRow, changedFieldsObj) {
            var endpoints = this.options.endpoints || {};
            var deltaPayload = this.buildSaveDeltaPayload(newRow, oldRow, changedFieldsObj);
            var canSaveDelta = this.canUseSaveDelta(deltaPayload.changes);
            var unsupportedDeltaFields = canSaveDelta ? [] : this.getUnsupportedSaveDeltaFields(deltaPayload.changes);

            if (canSaveDelta) {
                return {
                    url: endpoints.SAVE_DELTA,
                    payload: deltaPayload,
                    mode: "delta"
                };
            }

            return {
                url: endpoints.SAVE,
                payload: this.buildSavePayload(newRow, oldRow, changedFieldsObj),
                mode: "legacy",
                unsupportedDeltaFields: unsupportedDeltaFields
            };
        }

        buildAutoSaveRequest(rowData, oldRow, field) {
            var endpoints = this.options.endpoints || {};
            var payload = $.extend(true, {}, rowData || {});
            var baseRow = oldRow || {};

            payload.ManualJsonPatch = JSON.stringify({ [field]: false });
            payload.SkipCalc = 0;

            (this.options.fixedFields || []).forEach(function (key) {
                payload[key] = baseRow[key];
            });
            this.normalizeDateOnly(payload);

            var canUseDeltaAuto = this.isDeltaSaveSupportedField(field) && this.shouldSendDeltaManualPatch(field);
            var savePayload = canUseDeltaAuto
                ? {
                    wipId: Number(payload.WIPId || baseRow.WIPId || payload.id || baseRow.id || 0),
                    lineX: payload.LineX || baseRow.LineX || null,
                    changes: [{ field: field, value: payload[field] === undefined ? null : payload[field] }],
                    manualPatch: { [field]: false },
                    modifyBy: this.options.getUserName ? this.options.getUserName() : null,
                    isKetThuc: this.options.getShowEnded && this.options.getShowEnded() ? 1 : 0
                }
                : {
                    row: payload,
                    changes: [{ field: field, newValue: payload[field] }],
                    modifyBy: this.options.getUserName ? this.options.getUserName() : null
                };

            return {
                url: canUseDeltaAuto ? endpoints.SAVE_DELTA : endpoints.SAVE,
                payload: savePayload,
                mode: canUseDeltaAuto ? "delta" : "legacy"
            };
        }
    }

    WIP.Services.WipDataService = WipDataService;
})(window, jQuery);
