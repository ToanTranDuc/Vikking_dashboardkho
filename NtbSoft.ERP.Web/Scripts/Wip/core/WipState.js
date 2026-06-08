(function (window) {
    "use strict";

    var WIP = window.WIP = window.WIP || {};
    WIP.Core = WIP.Core || {};

    class WipState {
        constructor() {
            this.userName = localStorage.getItem("username") || localStorage.getItem("username1") || "";

            this.wipData = [];
            this.unassignedData = [];
            this.lineItems = [];
            this.selectedLineX = null;
            this.selectedIsGiaCong = false;
            this.selectedDataField = null;
            this.nextId = 1;
            this.baseGridColumns = null;

            this.grid = null;
            this.gridUnassigned = null;
            this.tabs = null;
            this.loadPanel = null;

            this.unassignedExpanded = false;
            this.showEnded = false;
            this.sortMode = false;
            this.crossMode = true;
            this.popupMode = "add";
            this.isGridSaving = false;
            this.busyCount = 0;

            this.permissionMap = {};
            this.viewableFields = new Set();
            this.editableFields = new Set();
            this.hasLoadedFieldPermissions = false;
            this.featurePermissions = {};
            this.hasLoadedFeaturePermissions = false;

            this.stepStatusMeta = {
                statusCodes: [0, 1, 2, 3],
                steps: [],
                compareTtColumns: [],
                compareTtColumnsSet: new Set(),
                byTtField: {}
            };
            this.columnDynamicMeta = [];
            this.saveDeltaFieldMeta = [];
            this.columnRegistry = {
                columnMetaByField: Object.create(null),
                fieldsByStepCode: Object.create(null),
                nonStepFieldsMap: Object.create(null),
                parentFieldsMap: Object.create(null),
                codeMap: Object.create(null)
            };
            this.stepRegistry = {
                stepMetaByCode: Object.create(null),
                stepMetaByKhField: Object.create(null),
                stepMetaByTtField: Object.create(null),
                comparableTtFieldMap: Object.create(null)
            };
            this.stepBindingByField = Object.create(null);

            this.requests = {
                loadXhr: null,
                loadSeq: 0,
                unassignedXhr: null,
                auditLoadXhr: null,
                syncTimers: {},
                syncInflight: {},
                wipHub: null
            };
        }
    }

    WIP.Core.WipState = WipState;
})(window);
