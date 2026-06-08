(function (window, $) {
    window.WIP = window.WIP || {};
    window.WIP.Core = window.WIP.Core || {};
    var WIP = window.WIP;

    // SCOPE MAP - WipRuntimeLegacyAdapter
    // 01. Bootstrap state và controller bridge.
    // 02. Toolbar, responsive layout và compact action sheet.
    // 03. Popup đổi/chia chuyền và bridge sang popup ngoài.
    // 04. Service/controller wiring còn phụ thuộc closure legacy.
    // 05. Metadata, registry, validation và field config.
    // 06. Audit, màu header, column filter và style rules.
    // 07. Grid init/events, toolbar buttons và export Excel.
    // 08. Edit row/cell popup, permission, load/save/recalc.
    // 09. Pool, SignalR, public WIP.* adapter và image helpers.

    WIP.Core.runLegacyWipDonHangRuntime = async function () {
        if (WIP.Core.legacyRuntimeStarted) return;
        WIP.Core.legacyRuntimeStarted = true;
        // REGION 01 - Bootstrap state và controller bridge.
        // Permission state is initialized once before bootstrap starts.
        window.WIP.permissionMap = window.WIP.permissionMap || {};
        window.WIP.viewableFields = window.WIP.viewableFields || new Set();
        window.WIP.editableFields = window.WIP.editableFields || new Set();
        window.WIP.hasLoadedFieldPermissions = window.WIP.hasLoadedFieldPermissions === true;
        // Feature permissions are loaded before protected UI becomes visible.
        window.WIP.featurePermissions = window.WIP.featurePermissions || {};
        window.WIP.hasLoadedFeaturePermissions = window.WIP.hasLoadedFeaturePermissions === true;
        window.WIP.cross = window.WIP.cross || {
            pending: null,
            set(p) { this.pending = p || null; },
            clear() { this.pending = null; }
        };
        const userName = localStorage.getItem("username") || localStorage.getItem("username1") || "";
        // TODO DB-CONFIG: danh sách feature key nên đồng bộ từ bảng quyền/chức năng khi BE có metadata đầy đủ.
        const FEATURE_KEYS = {
            HEADER_COLOR_CONFIG: "HEADER_COLOR_CONFIG"
        };
        var unassignedExpanded = false;
        var wipMoreActionSheet = null;
        var responsiveChromeTimer = null;
        var layoutTitleText = null;
        var layoutSubTitleText = null;
        var responsiveController = new WIP.UI.ResponsiveController();
        var toolbarController = new WIP.UI.ToolbarController();
        var dateFilterController = new WIP.UI.DateFilterController({
            isCompactActionMode: isCompactActionMode,
            hasActiveField: function () { return !!DateFilter.field; },
            updateButtonState: updateDateFilterButtonState,
            updatePopoverTitle: updateDateFilterPopoverTitle,
            syncPopoverContent: syncDateFilterPopoverContent,
            setActiveField: setActiveDateFilterField
        });

        function isCompactActionMode() {
            return responsiveController.isCompactActionMode();
        }

        function getUnassignedButtonText() {
            return responsiveController.getUnassignedButtonText(unassignedExpanded);
        }

        function getEndedButtonText() {
            return responsiveController.getEndedButtonText(showEnded);
        }

        function getLoadButtonText() {
            return responsiveController.getLoadButtonText();
        }

        function getButtonInstance(selector) {
            return toolbarController.getButtonInstance(selector);
        }

        function invokeButtonClick(selector) {
            toolbarController.invokeButtonClick(selector);
        }

        function getDateFilterPopoverTarget() {
            return dateFilterController.getPopoverTarget();
        }

        // REGION 02 - Toolbar, responsive layout và compact action sheet.
        // TODO DB-CONFIG: danh sách action toolbar đang hard-code ở runtime; có thể chuyển sang metadata theo user/role.
        function openDateFilterPopover() {
            dateFilterController.togglePopover();
        }

        function prepareDateFilterPopover() {
            return dateFilterController.preparePopover();
        }

        function showDateFilterPopover() {
            dateFilterController.showPopover();
        }

        function openDateFilterForColumn(field, caption) {
            dateFilterController.openForColumn(field, caption);
        }

        function buildWipMoreActionItems() {
            var defs = [
                { selector: "#btnExportWipExcel", text: "Xuất Excel", icon: "xlsxfile" },
                { selector: "#btnSortMode", text: "Sắp xếp", icon: "sort" },
                { selector: "#btnDateFilter", text: "Lọc ngày", icon: "filter" },
                { selector: "#btnLibraryWip", text: "Thư viện", icon: "folder" },
                { selector: "#btnHeaderColorConfig", text: "Tùy chỉnh màu", icon: "color" },
                { selector: "#btnColumnFilter", text: "Lọc cột", icon: "columnchooser" }
            ];

            return toolbarController.buildActionItems(defs);
        }

        function ensureWipMoreActionSheet() {
            wipMoreActionSheet = toolbarController.ensureMoreActionSheet("#wipMoreActionSheet", "More action");
            return wipMoreActionSheet;
        }
        function enableActionSheetOutsideClose(actionSheet) {
            toolbarController.enableActionSheetOutsideClose(actionSheet);
        }

        function syncWipMoreActionButton() {
            toolbarController.syncMoreActionButton({
                buttonSelector: "#btnWipMoreActions",
                definitions: [
                    { selector: "#btnExportWipExcel", text: "Xuất Excel", icon: "xlsxfile" },
                    { selector: "#btnSortMode", text: "Sắp xếp", icon: "sort" },
                    { selector: "#btnDateFilter", text: "Lọc ngày", icon: "filter" },
                    { selector: "#btnLibraryWip", text: "Thư viện", icon: "folder" },
                    { selector: "#btnHeaderColorConfig", text: "Tùy chỉnh màu", icon: "color" },
                    { selector: "#btnColumnFilter", text: "Lọc cột", icon: "columnchooser" }
                ],
                isCompact: isCompactActionMode(),
                text: "More"
            });
            wipMoreActionSheet = toolbarController.moreActionSheet;
        }

        function initWipMoreActions() {
            toolbarController.initMoreActionButton({
                buttonSelector: "#btnWipMoreActions",
                insertAfterSelector: "#btnColumnFilter",
                appendToSelector: ".topbar-right",
                sheetSelector: "#wipMoreActionSheet",
                sheetTitle: "More action",
                definitions: [
                    { selector: "#btnExportWipExcel", text: "Xuất Excel", icon: "xlsxfile" },
                    { selector: "#btnSortMode", text: "Sắp xếp", icon: "sort" },
                    { selector: "#btnDateFilter", text: "Lọc ngày", icon: "filter" },
                    { selector: "#btnLibraryWip", text: "Thư viện", icon: "folder" },
                    { selector: "#btnHeaderColorConfig", text: "Tùy chỉnh màu", icon: "color" },
                    { selector: "#btnColumnFilter", text: "Lọc cột", icon: "columnchooser" }
                ],
                isCompact: isCompactActionMode()
            });
            wipMoreActionSheet = toolbarController.moreActionSheet;
        }

        function applyResponsiveHeaderAndActions() {
            var titleState = responsiveController.applyHeaderAndActions({
                bodyClass: "wip-compact-actions",
                titleSelector: "#tittleBC",
                subTitleSelector: "#Title",
                titleState: {
                    titleText: layoutTitleText,
                    subTitleText: layoutSubTitleText
                },
                updateButtons: function () {
                    var unassignedBtn = getButtonInstance("#btnToggleUnassigned");
                    if (unassignedBtn) unassignedBtn.option("text", getUnassignedButtonText());

                    var endedBtn = getButtonInstance("#btnToggleIsKetThuc");
                    if (endedBtn) endedBtn.option("text", getEndedButtonText());

                    var loadBtn = getButtonInstance("#btnLoad");
                    if (loadBtn) loadBtn.option("text", getLoadButtonText());

                    syncWipMoreActionButton();
                    syncGridInteractionModes();
                }
            });
            layoutTitleText = titleState.titleText;
            layoutSubTitleText = titleState.subTitleText;
        }

        function scheduleResponsiveHeaderAndActions() {
            responsiveChromeTimer = responsiveController.schedule(responsiveChromeTimer, function () {
                applyResponsiveHeaderAndActions();
            }, 80);
        }

        async function setUnassignedUI(expand) {
            unassignedExpanded = !!expand;

            $("#unassignedWrap")
                .toggleClass("is-expanded", unassignedExpanded)
                .toggleClass("is-collapsed", !unassignedExpanded);

            $("body").toggleClass("unassigned-open", unassignedExpanded);

            var btn = $("#btnToggleUnassigned").dxButton("instance");
            if (btn) btn.option("text", getUnassignedButtonText());
            //if (unassignedExpanded) await loadUnassignedData();
            setTimeout(function () {
                try { grid && grid.updateDimensions(); } catch (e) { }
                try { gridUnassigned && gridUnassigned.updateDimensions(); } catch (e) { }
            }, 0);
        }

        toolbarController.initButton("#btnToggleUnassigned", {
            text: getUnassignedButtonText(),
            icon: "chevrondown",
            stylingMode: "contained",
            type: "default",
            onClick: async function () {
                await setUnassignedUI(!unassignedExpanded);
            }
        });

        var showEnded = false;
        function setEndedUI(enable) {
            showEnded = !!enable;

            const btn = $("#btnToggleIsKetThuc").dxButton("instance");
            if (btn) {
                btn.option({
                    text: getEndedButtonText(),
                    type: showEnded ? "success" : "normal",
                    stylingMode: "contained",
                    icon: null                               
                });
            }
            loadWipData({
                lineX: selectedLineX,
                includeEnded: showEnded ? 1 : 0
            });
        }

        toolbarController.initButton("#btnToggleIsKetThuc", {
            text: getEndedButtonText(),
            stylingMode: "contained",
            type: "normal", 
            icon: null,       
            onClick: function () {
                setEndedUI(!showEnded);
            }
        });
        toolbarController.initButton("#btnLibraryWip", {
            text: "Thư viện", // Tên hiển thị trên nút
            icon: "folder",      // Icon thư mục hoặc 'link' tùy bạn chọn
            type: "default",      // Màu xanh chuyên nghiệp đồng bộ với hệ thống
            stylingMode: "contained",
            hint: "Mở thư viện WIP trong tab mới",
            onClick: function () {
                // Thay đường dẫn '/ThuVienWip/Index' bằng link thực tế của bạn
                window.open('/ThuVienWip/Index', '_blank');
            }
        });
        toolbarController.initButton("#btnHeaderColorConfig", {
            text: "Tùy chỉnh màu",
            icon: "color",
            type: "default",
            stylingMode: "contained",
            visible: false,
            elementAttr: {
                style: "background: linear-gradient(90deg, #ef4444 0%, #f97316 16%, #eab308 32%, #22c55e 48%, #06b6d4 64%, #3b82f6 80%, #a855f7 100%); border: 0; color: #fff; font-weight: 700; box-shadow: 0 2px 8px rgba(0,0,0,.18);"
            },
            onClick: function () {
                if (!canUseFeature(FEATURE_KEYS.HEADER_COLOR_CONFIG)) {
                    DevExpress.ui.notify("Bạn không có quyền tùy chỉnh màu header", "warning", 1800);
                    return;
                }
                openHeaderColorPopup();
            }
        });
        // REGION 03 - Popup đổi/chia chuyền và bridge sang popup ngoài.
        // Demo PopUp Chia SX
        var pendingCross = null; // giữ info item vừa kéo

        function openCrossPopup(maDH, lineX, maHang) {
            // mở popup bạn đang có
            openProductionPopup(maDH, lineX, maHang);

        //    console.log("[CROSS->POPUP] MaDH =", maDH, "| LineX =", lineX);
        }
        function fillPopup_MaDH_LineX(maDH, lineX, maHang) {
            // đảm bảo init controls trước khi lấy instance
            ensurePopupInited();

            // MaDH là dxTextBox
            const ma = $("#txt_MaDH").dxTextBox("instance");
            ma.option("value", maDH || "");

            const style = $("#txt_MaHang").dxTextBox("instance");
            if (style) style.option("value", maHang || "");

            // LineX bây giờ là dxSelectBox (không còn dxTextBox nữa)
            const ln = $("#txt_LineX").dxSelectBox("instance");
            ln.option("value", Number(lineX) || null);

            //console.log("[CROSS->POPUP] set popup value => MaDH:", maDH, "| LineX:", lineX);
        }
        // REGION 04 - Service/controller wiring còn phụ thuộc closure legacy.
        // Nhóm này giữ state runtime và các bridge sang module mới; ưu tiên tách dần thay vì đổi behavior hàng loạt.
        var _poolSelBaseline = null;// { MaKH, MaHang, Season, ... }
        var DEFAULT_LINE_X = 101; // Daisy
        var _loadXhr = null;
        var _loadSeq = 0;
        var _tabsInitLock = true;
        var CROSS_GROUP = "wip-cross";
        var wipData = [];
        var unassignedData = [];
        var grid = null;
        var gridUnassigned = null;
        var baseGridColumns = null;
        var nextId = 1;
        var selectedDataField = null;
        var lineItems = [];
        var selectedLineX = null;
        var selectedIsGiaCong = false;
        var lineState = new WIP.Core.LineState();
        var lineTabsController = null;
        //Lọc ngày đến ngày cho audit
        var auditPopupController = new WIP.UI.AuditPopupController({
            getIds: function () { return AUDIT_POPUP_IDS; },
            popupOptions: mobilePopupOptions,
            notify: function (message, type, time) { DevExpress.ui.notify(message, type, time); },
            getLoadService: function () { return wipLoadService; },
            isGiaCongMode: isGiaCongReadOnlyMode,
            getFieldCaptionMap: function () { return AUDIT_FIELD_CAPTION_BY_FIELD; },
            getFieldCaption: getFieldCaption
        });
        var tabs = null;
        var isGridSaving = false;
        var sortMode = false;
        var btnSort;
        var _unassignedXhr = null;
        var crossMode = true;
        var btnCrossMode = null;
        var _busyCount = 0;
        var MOBILE_LONG_PRESS_MS = 500;
        var isMobileDeviceClient = detectMobileDevice();
        var mobileActionSheet = null;
        var _mobileActionSheetLastOpenAt = 0;
        var _mobileHoldTimer = null;
        var _mobileHoldTriggered = false;
        var _mobileTouchStartPoint = null;
        var _mobileTouchStartTarget = null;
       
        var loadPanel = $("#wipLoadPanel").dxLoadPanel({
            shading: true,
            showIndicator: true,
            showPane: true,
            hideOnOutsideClick: false,
            visible: false,
            message: "Đang xử lý..."
        }).dxLoadPanel("instance");

        function showBusy(msg) {
            _busyCount++;
            try {
                loadPanel.option("message", msg || "Đang xử lý...");
                loadPanel.show();
            } catch (e) { }
        }
        function hideBusy() {
            _busyCount = Math.max(0, _busyCount - 1);
            if (_busyCount === 0) {
                try { loadPanel.hide(); } catch (e) { }
            }
        }
        function isGiaCongReadOnlyMode() {
            return selectedIsGiaCong === true;
        }
        function applyModeToGrid() {
            const grid = $("#gridContainer").dxDataGrid("instance");
            if (!grid) return;

            grid.columnOption("LineName", "visible", !selectedIsGiaCong && !isUserColumnHidden("LineName"));
            grid.columnOption("TenDVSX", "visible", selectedIsGiaCong && !isUserColumnHidden("TenDVSX"));
        }
        //function ensureWritableMode() {
        //    if (!isGiaCongReadOnlyMode()) return true;
        //    DevExpress.ui.notify("Tab Gia công chỉ xem, không cho chỉnh sửa.", "warning", 1800);
        //    return false;
        //}
        //function applyGridEditability() {
        //    if (typeof grid === "undefined" || !grid) return;
        //    var allowEdit = !sortMode && !isGiaCongReadOnlyMode();
        //    grid.option("editing.allowUpdating", allowEdit);
        //}
        function detectMobileDevice() {
            var ua = (navigator.userAgent || "").toLowerCase();
            var coarsePointer = false;
            try {
                coarsePointer = !!(window.matchMedia && (window.matchMedia("(pointer: coarse)").matches || window.matchMedia("(hover: none)").matches));
            } catch (e) { }
            var uaMobile = /android|iphone|ipad|ipod|windows phone|opera mini|mobile/i.test(ua);
            return !!(coarsePointer || uaMobile);
        }
        function isMobileViewport() {
            try {
                return !!(window.matchMedia && window.matchMedia("(max-width: 767.98px)").matches);
            } catch (e) {
                return window.innerWidth < 768;
            }
        }
        function isMobileUiMode() {
            return !!(isMobileDeviceClient || isMobileViewport());
        }
        function mobilePopupOptions() {
            if (!isMobileUiMode()) return {};
            return {
                fullScreen: true,
                width: "100vw",
                height: "100vh",
                maxHeight: "100vh",
                dragEnabled: false
            };
        }
        function unfixColumnsForMobile(columns) {
            if (!Array.isArray(columns)) return columns;
            return columns.map(function (col) {
                if (!col || typeof col !== "object") return col;
                var next = $.extend(true, {}, col);
                if (Object.prototype.hasOwnProperty.call(next, "fixed")) next.fixed = false;
                if (Object.prototype.hasOwnProperty.call(next, "fixedPosition")) delete next.fixedPosition;
                if (Array.isArray(next.columns) && next.columns.length) {
                    next.columns = unfixColumnsForMobile(next.columns);
                }
                return next;
            });
        }
        function disableFixedColumnsForMobile(gridInstance) {
            if (!isMobileUiMode() || !gridInstance) return;
            try { gridInstance.option("columnFixing.enabled", false); } catch (e) { }
            try {
                var cols = gridInstance.option("columns");
                if (Array.isArray(cols) && cols.length) {
                    gridInstance.option("columns", unfixColumnsForMobile(cols));
                }
            } catch (e) { }
            try { gridInstance.updateDimensions(); } catch (e) { }
        }
        function syncGridInteractionModes() {
            var mobileMode = isMobileUiMode();
            try {
                if (grid) {
                    grid.option("allowColumnResizing", !mobileMode);
                    grid.updateDimensions();
                }
            } catch (e) { }
            try {
                if (gridUnassigned) {
                    gridUnassigned.option("allowColumnResizing", !mobileMode);
                    gridUnassigned.updateDimensions();
                }
            } catch (e) { }
        }
        function clearMobileHoldTimer() {
            if (_mobileHoldTimer) {
                clearTimeout(_mobileHoldTimer);
                _mobileHoldTimer = null;
            }
        }

        function setCrossModeUI(isOn) {
            crossMode = true;

            if (grid) {
                grid.option("rowDragging.showDragIcons", !!sortMode);
            }

            if (gridUnassigned) {
                gridUnassigned.option("rowDragging.showDragIcons", false);
            }

            btnCrossMode && btnCrossMode.option({
                text: "Gán vào chuyền",
                type: "success"
            });
        }

        // TODO DB-CONFIG: fallback endpoint chỉ dùng khi Razor chưa inject window.WIP_API.
        var API = window.WIP_API || {
            GET: "/api/wip-donhang/get",
            SAVE: "/api/wip-donhang/save",
            REORDER: "/api/wip-donhang/reorder",
            RECALC: "/api/wip-donhang/recalc-line"
        };
        API.SAVE_DELTA = API.SAVE_DELTA || "/api/wip-donhang/save-delta";
        API.SAVE_DELTA_FIELDS = API.SAVE_DELTA_FIELDS || "/api/wip-donhang/save-delta-fields";
        API.RECALC = API.RECALC || "/api/wip-donhang/recalc-line";
        API.GET_MD = API.GET_MD || "/api/wip-donhang/get-md";
        API.STEP_STATUS_META = API.STEP_STATUS_META || "/api/wip-donhang/step-status-meta";
        API.COLUMN_DYNAMIC = API.COLUMN_DYNAMIC || "/api/wip-donhang/column-dynamic";
        API.HEADER_COLORS_GET = API.HEADER_COLORS_GET || "/api/wip-donhang/header-colors";
        API.HEADER_COLORS_UPDATE = API.HEADER_COLORS_UPDATE || "/api/wip-donhang/header-colors/update";
        API.FEATURE_PERMISSION = API.FEATURE_PERMISSION || "/api/PhanQuyenWIP/Feature";
        API.FIELD_PERMISSION = API.FIELD_PERMISSION || "/api/PhanQuyenWIP/GetFieldPermissions";
        API.SYNC_LIB = API.SYNC_LIB || "/api/wip-donhang/syncLib";
        API.AUDIT = API.AUDIT || "/api/wip-donhang/audit";
        var wipAjaxClient = (window.WIP && WIP.Api && WIP.Api.AjaxClient) ? new WIP.Api.AjaxClient() : null;
        var wipApi = (window.WIP && WIP.Api && WIP.Api.WipDonHangApi && wipAjaxClient)
            ? new WIP.Api.WipDonHangApi(wipAjaxClient, API)
            : null;
        var wipPermissionService = new WIP.Services.PermissionService(wipApi);
        var wipMetadataService = new WIP.Services.MetadataService(wipApi);
        var wipLoadService = new WIP.Services.LoadService(wipApi);
        var wipLibraryService = new WIP.Services.WipLibraryService({
            loadService: wipLoadService
        });
        var wipSaveService = new WIP.Services.SaveService(wipApi);
        var wipRecalcService = new WIP.Services.RecalcService(wipApi);
        var wipLineService = new WIP.Services.LineService(wipApi);
        var wipSignalRService = new WIP.Services.SignalRService(wipApi);
        var wipPoolService = new WIP.Services.PoolService({
            getPoolData: function () { return unassignedData; },
            setPoolData: function (items) { unassignedData = items || []; },
            getLineData: function () { return wipData; },
            setLineData: function (items) { wipData = items || []; },
            getPoolGrid: function () { return gridUnassigned; },
            getMainGrid: function () { return grid; },
            getSelectedLineX: function () { return selectedLineX; },
            mapRowId: mapRowId,
            gridViewAdapter: GridViewAdapter
        });
        wipSignalRService.setPoolService(wipPoolService);
        var wipExportExcelService = new WIP.Services.ExportExcelService({
            getGrid: function () { return grid; },
            getSelectedLineX: function () { return selectedLineX; },
            getLineItems: function () { return lineItems; },
            getTabs: function () { return tabs; },
            normalizeHexColor: normalizeHexColor,
            normalizeHeaderColorConfig: normalizeHeaderColorConfig,
            normalizeHeaderColorClassName: normalizeHeaderColorClassName,
            splitClassNames: splitClassNames,
            getHeaderColorClassSet: function () { return HEADER_COLOR_CLASS_SET; },
            getPrefixedHeaderClass: getPrefixedHeaderClass,
            buildHeaderColorMap: function (items) { return headerColorPopupController.buildColorMap(items); },
            getHeaderColorItems: function () { return headerColorItems; },
            getDefaultHeaderTextColorConfig: function () { return DEFAULT_HEADER_TEXT_COLOR_CONFIG; },
            getManualCellColorConfig: function () { return manualCellColorConfig; },
            isManualCell: isManualCell,
            getImageByField: getImageByField,
            showBusy: showBusy,
            hideBusy: hideBusy,
            notify: function (message, type, time) { DevExpress.ui.notify(message, type, time); }
        });
        var formEditorFactory = new WIP.UI.FormEditorFactory({
            $: $,
            DevExpress: DevExpress,
            libraryService: wipLibraryService,
            getPopupMode: function () { return editRowPopupController ? editRowPopupController.getMode() : "add"; },
            getCurrentEditRowData: function () { return editRowPopupController ? editRowPopupController.getFormData() : {}; },
            getFallbackLineX: function () {
                var editorData = editRowPopupController ? editRowPopupController.getFormData() : null;
                return editorData ? editorData.LineX : null;
            },
            isFixedField: function (field) { return FIXED_FIELDS.includes(field); },
            isCalcField: isCalcField,
            isInputHintField: isInputHintField,
            canEditFieldByPermission: canEditFieldByPermission,
            safeFormUpdate: function (ctx, fn) { return safeFormUpdate(ctx, fn); },
            markCtxFieldChanged: function (ctx, field) { return markCtxFieldChanged(ctx, field); },
            getFieldCaption: getFieldCaption,
            getFieldEditorType: getFieldEditorType,
            isEmptyDateValue: isEmptyDateValue,
            getTodayDateOnly: getTodayDateOnly,
            text: text,
            merSelect: merSelect,
            num: num,
            date: date,
            bool: bool
        });
        lineTabsController = new WIP.UI.LineTabsController({
            lineState: lineState,
            lineService: wipLineService,
            defaultLineX: DEFAULT_LINE_X,
            onSelectionChanged: async function (selection) {
                selectedLineX = selection.lineX;
                selectedIsGiaCong = selection.isGiaCong;
                applyModeToGrid();
                rebuildGridColumnsByPermission(selectedLineX);
                applyModeToGrid();
                try { if (gridUnassigned) gridUnassigned.clearSelection(); } catch (ex) { }

                await syncLibByLine(selectedLineX);
                await loadUnassignedData();
                loadWipData({
                    lineX: selectedLineX,
                    includeEnded: showEnded ? 1 : 0
                });
            },
            afterLoad: async function (selection, lines, isFirstLoad) {
                lineItems = lineState.getItems();
                selectedLineX = selection.lineX;
                selectedIsGiaCong = selection.isGiaCong;
                rebuildGridColumnsByPermission(selectedLineX);
                applyModeToGrid();

                if (isFirstLoad) {
                    await syncLibByLine(selectedLineX);
                    loadWipData({
                        lineX: selectedLineX,
                        isGiaCong: selectedIsGiaCong,
                        includeEnded: showEnded ? 1 : 0
                    });
                }

                DevExpress.ui.notify(
                    "Đã tải danh sách chuyền: " + (lines.length || 0),
                    "success",
                    800
                );
            }
        });
        var wipGridController = new WIP.Grid.WipGridController({
            filterData: function (items) {
                return applyClientDateFilter(items);
            }
        });
        var unassignedGridController = new WIP.Grid.UnassignedGridController({
            columnsProvider: function () {
                return poolColumns();
            }
        });
        var headerColorItems = [];
        var headerColorPopupController = new WIP.UI.HeaderColorPopupController({
            canOpen: function () { return canUseFeature(FEATURE_KEYS.HEADER_COLOR_CONFIG); },
            notify: function (message, type, time) { DevExpress.ui.notify(message, type, time); },
            popupOptions: mobilePopupOptions,
            loadService: wipLoadService,
            saveService: wipSaveService,
            getUserName: function () { return userName; },
            getGroupOptions: function () { return HEADER_GROUP_COLOR_OPTIONS; },
            getSeed: function () { return headerColorConfigSeed; },
            getManualDefaults: function () { return DEFAULT_MANUAL_CELL_COLOR_CONFIG; },
            getTtBomDefaults: function () { return DEFAULT_TT_BOM_STATUS_COLOR_CONFIG; },
            normalizeHexColor: normalizeHexColor,
            setItems: function (items) { headerColorItems = items; },
            applyHeaderColors: applyHeaderColors,
            applyManualCellColors: applyManualCellColors,
            applyTtBomStatusColors: applyTtBomStatusColors
        });
        var columnFilterPopupController = new WIP.UI.ColumnFilterPopupController({
            notify: function (message, type, time) { DevExpress.ui.notify(message, type, time); },
            storageKeyPrefix: "wip-donhang-hidden-columns",
            getUserName: function () { return userName; },
            popupOptions: mobilePopupOptions,
            showBusy: showBusy,
            hideBusy: hideBusy,
            hasBaseColumns: function () { return !!baseGridColumns; },
            getPermissionLineX: function () {
                return (selectedLineX === null || selectedLineX === undefined || selectedLineX === "") ? undefined : selectedLineX;
            },
            getSelectedLineX: function () { return selectedLineX; },
            cloneBaseColumns: function () { return $.extend(true, [], baseGridColumns); },
            prepareGridColumns: prepareGridColumns,
            applyLineModeColumnVisibility: applyLineModeColumnVisibility,
            rebuildGridColumns: rebuildGridColumnsByPermission
        });
        // REGION 05 - Metadata, registry, validation và field config.
        var DEFAULT_HEADER_TEXT_COLOR_CONFIG = {
            "grid-header-default-text": "#111827",
            "hdr-donhang-text": "#111827",
            "hdr-dongbo-intheu-text": "#111827",
            "hdr-kh-ngay-text": "#111827",
            "hdr-merchandiser-text": "#111827",
            "hdr-qc-text": "#111827",
            "hdr-ie-text": "#111827",
            "hdr-codien-text": "#111827",
            "hdr-nangsuat-text": "#111827",
            "hdr-tinhtoan-nangsuat-text": "#111827",
            "hdr-tiendo-thucte-text": "#111827",
            "hdr-thoigian-text": "#111827",
            "hdr-intheu-text": "#111827",
            "hdr-kythuat-text": "#111827"
        };
        var DEFAULT_MANUAL_CELL_COLOR_CONFIG = {
            Background: "#FFE8C2",
            Border: "#FF9800",
            Text: "#111827"
        };
        var manualCellColorConfig = {};
        var stepStatusMeta = {
            statusCodes: [0, 1, 2, 3],
            steps: [],
            compareTtColumns: [],
            compareTtColumnsSet: new Set(),
            byTtField: {}
        };
        var columnDynamicMeta = [];
        var columnRegistry = {
            columnMetaByField: Object.create(null),
            fieldsByStepCode: Object.create(null),
            nonStepFieldsMap: Object.create(null),
            parentFieldsMap: Object.create(null),
            codeMap: Object.create(null)
        };

        var stepBindingByField = Object.create(null);
        var stepRegistry = {
            stepMetaByCode: Object.create(null),
            stepMetaByKhField: Object.create(null),
            stepMetaByTtField: Object.create(null),
            comparableTtFieldMap: Object.create(null)
        };
        function inferBindingRole(field, stepMeta) {
            return WIP.Domain.Registry.inferBindingRole(field, stepMeta);
        }

        function buildStepBindingByField() {
            var binding = Object.create(null);
            var fieldMap = columnRegistry.columnMetaByField || Object.create(null);

            Object.keys(fieldMap).forEach(function (fieldKey) {
                var colMeta = fieldMap[fieldKey];
                if (!colMeta || !colMeta.dataField) return;

                var stepCode = String(colMeta.stepCode || "").trim();
                if (!stepCode) {
                    binding[fieldKey] = {
                        field: colMeta.dataField,
                        stepCode: null,
                        role: "PARENT",
                        dataType: colMeta.dataType || "text",
                        allowManualKH: false,
                        isCheckTT: false
                    };
                    return;
                }

                var stepMeta = (typeof getStepMetaByCode === "function")
                    ? getStepMetaByCode(stepCode)
                    : null;

                var role = inferBindingRole(colMeta.dataField, stepMeta);

                binding[fieldKey] = {
                    field: colMeta.dataField,
                    stepCode: stepCode,
                    role: role,
                    dataType: colMeta.dataType || "text",
                    allowManualKH: !!(stepMeta && stepMeta.AllowManualKH),
                    isCheckTT: !!(stepMeta && stepMeta.IsCheckTT)
                };
            });

            return binding;
        }

        function rebuildStepBindingByField() {
            stepBindingByField = buildStepBindingByField();
        }

        function getStepBindingByField(dataField) {
            var key = normalizeRegistryKey(dataField);
            return key ? (stepBindingByField[key] || null) : null;
        }

        function normalizeRegistryKey(value) {
            return WIP.Domain.Registry.normalizeRegistryKey(value);
        }

        function buildStepRegistry(meta) {
            return WIP.Domain.Registry.buildStepRegistry(meta);
        }

        function getStepMetaByCode(stepCode) {
            var key = normalizeRegistryKey(stepCode);
            return key ? (stepRegistry.stepMetaByCode[key] || null) : null;
        }

        function getStepMetaByKhField(khField) {
            var key = normalizeRegistryKey(khField);
            return key ? (stepRegistry.stepMetaByKhField[key] || null) : null;
        }

        function getStepMetaByTtField(ttField) {
            var key = normalizeRegistryKey(ttField);
            return key ? (stepRegistry.stepMetaByTtField[key] || null) : null;
        }

        function isComparableTtField(ttField) {
            var key = normalizeRegistryKey(ttField);
            return !!(key && stepRegistry.comparableTtFieldMap[key]);
        }
        // TODO DB-CONFIG: mã trạng thái TT_BOM hiện fallback 0..3; ưu tiên dùng stepStatusMeta từ DB khi có.
        var TT_BOM_STATUS_KEYS = [0, 1, 2, 3];
        var DEFAULT_TT_BOM_STATUS_COLOR_CONFIG = {
            0: "",
            1: "#22C55E",
            2: "#F97316",
            3: "#EF4444"
        };
        var ttBomStatusColorConfig = {};
        // TODO DB-CONFIG: nhóm field cố định/không cho sửa nên lấy từ metadata cột/quyền thay vì hard-code.
        var FIXED_FIELDS = [
            "WIPId", "id",
            "LineX", "ThuTuChuyen", "LineName",
            "MaDH", "LenhSX", "MaLenhSanXuat",
            "MaKH", "Season", "StyleId",
            "MaHang", "KhachHang", "PO",
            "SLKH", "TT_Cat", "BTP_DK", "RaChuyen", "Packing",
            "MaDVSX", "TenDVSX",
            "TP_Nhan", "TP_KiemDat",
            "AllocatedRegularHours", "AllocatedOTHours"
            //, "Mer"
        ];
        // TODO DB-CONFIG: giới hạn độ dài field nên lấy từ schema/metadata save.
        var FIELD_MAX_LENGTH = {
            GhiChuKho: 500
        };
        // TODO DB-CONFIG: field tính toán nên lấy từ metadata công thức/recalc.
        var CALC_FIELDS = [
            // KPI
            "NSGio", "SoGioSX", "SoNgay",
            //"AllocatedRegularHours", "AllocatedOTHours",

            // Timeline chính
            "KHMay", "ThoatChuyen", "KHLapTrinh", "KHCat", "KH_Top",

            // Timeline phụ thuộc
            "KH_Vai", "KH_PLInEp", "KH_PLMay", "KH_PackingList", "KH_PLDongGoi", "KH_LenhSX"
        ];
        // TODO DB-CONFIG: field hỗ trợ nhập tay nên lấy từ metadata save/manual.
        var MANUAL_FIELDS = [
            "NSGio", "SoGioSX", "SoNgay",
            "NSCost", "NS_SMV", "DoanhThuCM",
            "KHMay", "ThoatChuyen", "KHLapTrinh", "KHCat", "KH_Top",
            "KH_Vai", "KH_PLInEp", "KH_PLMay", "KH_PackingList", "KH_PLDongGoi", "KH_LenhSX", "CostPerM", "InTheu", "DoKim", "InTheuTrong_PKH",
            "KH_NPL_BM_KTX_MayMau",
            "KH_TestKeoLogo_TapeLaser",
            "KH_Rap",
            "KH_TacNghiepCat",
            "KH_SoDo",
            "KH_BangMauVai_InEp",
            "KH_BangMauPL_Full",
            "KH_BangDanhSo",
            "KH_TLieuInEp",
            "KH_TLieuLT",
            "KH_TSCatPL",
            "KH_MauDauChuyen",
            "KH_DuyetMauDauChuyen",
            "NgayNhanTT_BOM",
            "KH_BOM", "HutAm",
            "KH_GuiInTheu", "KH_NhanInTheu"
        ];
        // Logic cu: filter ngay theo 2 mode co dinh CUT / OUT.
        //const DateFilter = {
        //    mode: "CUT",            // "CUT" | "OUT"
        //    range: [null, null]     // [from, to]
        //};
        //const FIELDS = {
        //    CUT: "KHCat",
        //    OUT: "ThoatChuyen"
        //};
        // Logic moi: filter client-side theo dataField cua cot ngay dang duoc chon.
        // DateFilter mới chạy client-side theo dataField của cột ngày đang chọn.
        const DateFilter = {
            field: null,
            caption: "",
            range: [null, null]
        };
        // TODO DB-CONFIG: nhóm field kích hoạt recalc 16 nên lấy từ metadata recalc.
        const RECALC_FIELDS_16 = [
            "NSGio", "SoGioSX", "SoNgay",
            "SLCN", "TGLV", "HieuSuat",
            "SMV_WIP", "CostPerM", "Profit",
            "CM", "HeSoCM",
            "SLKH",
            "KHMay", "ThoatChuyen", "KHCat", "InTheu", "InTheuTrong_PKH"
        ];
        // TODO DB-CONFIG: chỉ giữ fallback khi endpoint save-delta-fields lỗi hoặc chưa trả metadata.
        const FALLBACK_DELTA_SAVE_PARENT_FIELDS = [
            "GhiChuKho", "GhiChu", "Mer",
            "InTheu", "DoKim", "HutAm", "InTheuTrong_PKH",
            "SLCN", "TGLV", "OT", "HieuSuat",
            "SMV_WIP", "CostPerM", "Profit",
            "CM", "HeSoCM",
            "NSGio", "SoGioSX", "SoNgay",
            "NSCost", "NS_SMV", "DoanhThuCM",
            "KH_Top", "SLSizeMau_Top", "TT_MauTop",
            "KH_Ship", "SLSizeMau_Ship", "TT_Ship",
            "NgayNhanTT_BOM", "DateTrenNhanCare",
            "KH_MauDauChuyen", "TT_MauDauChuyen"
        ];
        var saveDeltaFieldMeta = [];
        var DELTA_SAVE_PARENT_FIELD_MAP = buildSaveDeltaParentFieldMap(FALLBACK_DELTA_SAVE_PARENT_FIELDS);
        var wipDataPayloadService = new WIP.Services.WipDataService({
            endpoints: API,
            fixedFields: FIXED_FIELDS,
            manualFields: MANUAL_FIELDS,
            getUserName: function () { return userName; },
            getShowEnded: function () { return showEnded; },
            getDeltaParentFieldMap: function () { return DELTA_SAVE_PARENT_FIELD_MAP; },
            normalizeRegistryKey: normalizeRegistryKey,
            getStepMetaByKhField: getStepMetaByKhField,
            getStepMetaByTtField: getStepMetaByTtField,
            isManualSupportedField: isManualSupportedField,
            isFieldValueChanged: isFieldValueChanged,
            attachRecalcFlag: attachRecalcFlag,
            buildChangesFromRow: buildChangesFromRow,
            parseManualFlags: parseManualFlags
        });

        function buildSaveDeltaParentFieldMap(fields) {
            return (fields || []).reduce(function (map, field) {
                var key = normalizeRegistryKey(field);
                if (key) map[key] = true;
                return map;
            }, Object.create(null));
        }
        function normalizeDateFilterCaption(value) {
            return WIP.Domain.DateFilter.normalizeCaption(value).replace(/^\s*\(\d+\)\s*/, "").trim();
        }
        function getDateFilterCaption(field, fallbackCaption) {
            if (!field) return "";
            var colMeta = getColumnMetaByField(field);
            if (colMeta) {
                var pathText = normalizeDateFilterCaption(colMeta.captionPath || "");
                if (pathText) return pathText;
            }

            var fallback = normalizeDateFilterCaption(fallbackCaption);
            if (fallback) return fallback;
            return normalizeDateFilterCaption(getFieldCaption(field, field));
        }
        function isFilterableDateColumn(field, column) {
            if (!field) return false;
            if (column && column.dataType === "date") return true;

            return getFieldDataType(field) === "date";
        }
        function normalizeDateOnlyForRange(value) {
            return WIP.Domain.DateFilter.normalizeDateOnlyForRange(value);
        }
        function isClientDateFilterActive() {
            return WIP.Domain.DateFilter.isActive(DateFilter);
        }
        function updateDateFilterButtonState() {
            var btn = $("#btnDateFilter").dxButton("instance");
            var hint = "Chọn cột ngày rồi lọc";

            if (DateFilter.field) {
                hint = "Lọc theo: " + getDateFilterCaption(DateFilter.field, DateFilter.caption) + " (" + DateFilter.field + ")";
            }

            $("#btnDateFilter").toggleClass("is-filter-active", isClientDateFilterActive());
            if (btn) btn.option("hint", hint);
        }
        function setActiveDateFilterField(field, caption) {
            if (!field) return;

            DateFilter.field = field;
            DateFilter.caption = getDateFilterCaption(field, caption);
            updateDateFilterButtonState();
        }
        function rowMatchesClientDateFilter(row) {
            return WIP.Domain.DateFilter.rowMatches(row, DateFilter);
        }
        function applyClientDateFilter(dataSource) {
            return WIP.Domain.DateFilter.apply(dataSource, DateFilter);
        }
        function updateDateFilterPopoverTitle() {
            var pop = $("#popDateFilter").dxPopover("instance");
            if (!pop) return;

            var caption = DateFilter.field ? getDateFilterCaption(DateFilter.field, DateFilter.caption) : "";
            pop.option("title", caption ? ("Lọc theo ngày: " + caption) : "Lọc theo ngày");
        }
        function syncDateFilterPopoverContent() {
            var caption = getDateFilterCaption(DateFilter.field, DateFilter.caption);
            $("#dfSelectedFieldLabel").text(caption || "Chưa chọn cột ngày");
            //$("#dfSelectedFieldCode").text(DateFilter.field || "");

            var $from = $("#dfFrom");
            var $to = $("#dfTo");
            var fromInst = $from.length ? $from.dxDateBox("instance") : null;
            var toInst = $to.length ? $to.dxDateBox("instance") : null;

            if (fromInst) fromInst.option("value", DateFilter.range[0] || null);
            if (toInst) toInst.option("value", DateFilter.range[1] || null);
        }
        function refreshMainGridWithDateFilter(notifyMsg, notifyType, notifyTime) {
            if (!grid) return;
            GridViewAdapter.updateGrid(grid, wipData, notifyMsg, notifyType, notifyTime);
        }

        function isCalcField(field) { return WIP.Domain.Validation.isCalcField(field, CALC_FIELDS); }
        function getFieldMaxLength(field) {
            return WIP.Domain.Validation.getFieldMaxLength(field, FIELD_MAX_LENGTH);
        }

        // TODO DB-CONFIG: input hint/read-only gợi ý nên đi theo metadata editor của field.
        var INPUT_HINT_FIELDS = [
            "LineX", "LenhSX", "StyleId",
            "CostPerM",
            "SLCN", "TGLV", "OT", "HieuSuat",
            "SLKH", "SMV_WIP", "Cost/Min",
            "CM", "HeSoCM", "Profit",
            "PO", "MaHang", "KhachHang", "InTheu"
        ];
        function isInputHintField(field) { return WIP.Domain.Validation.isInputHintField(field, INPUT_HINT_FIELDS); }

        function canEditField(field) {
            if (field === "ThuTuChuyen") return false;
            return field && !FIXED_FIELDS.includes(field);
        }
        // TODO DB-CONFIG: rule không âm nên lấy từ validation metadata.
        const NON_NEG_FIELDS = new Set([
            "SLKH", "NSGio", "SoGioSX", "SoNgay",
            "SLCN", "TGLV", "OT", "HieuSuat",
            "AllocatedRegularHours", "AllocatedOTHours",
            "TT_Cat", "BTP_DK", "RaChuyen",
            "SMV_WIP", "NSCost", "NS_SMV", "CostPerM", "Profit",
            "CM", "HeSoCM", "DoanhThuCM",
            "LenhSX", "ThuTuChuyen", "LineX",
            "Packing" // nếu Packing là số
        ]);
        function isNonNegField(field) {
            return WIP.Domain.Validation.isNonNegField(field, NON_NEG_FIELDS);
        }
        // TODO DB-CONFIG: FIELD_META là fallback caption/type; ưu tiên column-dynamic metadata khi BE trả đủ.
        var FIELD_META = {
            ThuTuChuyen: { cap: "STT", group: "1. Đơn hàng" },
            Mer: { cap: "MD" },
            LineX: { cap: "Chuyền", group: "1. Đơn hàng" },
            MaKH: { cap: "Brand" },
            Season: { cap: "Season" },
            MaDH: { cap: "Mã đơn hàng" },
            LenhSX: { cap: "Lệnh SX", group: "1. Đơn hàng" },
            StyleId: { cap: "Style", group: "1. Đơn hàng" },
            MaHang: { cap: "Mã hàng", group: "1. Đơn hàng" },
            KhachHang: { cap: "Khách hàng", group: "1. Đơn hàng" },
            PO: { cap: "PO", group: "1. Đơn hàng" },
            SLKH: { cap: "SLKH", group: "1. Đơn hàng" },
            InTheu: { cap: "In Bên ngoài", group: "1. Đơn hàng" },
            // ===== In/Ép/Thêu/Laser =====
            DoKim: { cap: "Dò kim" },
            InTheuTrong_PKH: { cap: "In Công ty" },
            HutAm: { cap: "Hút ẩm" },
            // ===== Đồng bộ in/thêu =====
            KH_GuiInTheu: { cap: "KH gửi BTP in_thêu" },
            TT_GuiInTheu: { cap: "TT gửi BTP in_thêu" },
            KH_NhanInTheu: { cap: "KH nhận BTP in_thêu" },
            TT_NhanInTheu: { cap: "TT nhận BTP in_thêu" },

            NSGio: { cap: "NS_Giờ", group: "2. NS/Thời gian" },
            SoGioSX: { cap: "Số giờ SX", group: "2. NS/Thời gian" },
            SoNgay: { cap: "Số ngày", group: "2. NS/Thời gian" },
            SLCN: { cap: "SL CN(Người)", group: "2. NS/Thời gian" },
            TGLV: { cap: "Giờ làm việc(Giờ)", group: "2. NS/Thời gian" },
            OT: { cap: "Làm việc ngoài giờ(giờ)", group: "2. NS/Thời gian" },   
            AllocatedRegularHours: { cap: "Giờ làm việc(Giờ)", group: "2. NS/Thời gian" },
            AllocatedOTHours: { cap: "Giờ tăng ca(Giờ)", group: "2. NS/Thời gian" },
            HieuSuat: { cap: "Hiệu suất(%)", group: "2. NS/Thời gian" },

            KHCat: { cap: "KH Cắt", group: "3. Kế hoạch ngày" },
            TTCat: { cap: "TT Cắt", group: "3. Kế hoạch ngày" },
            KHLapTrinh: { cap: "KH Lập trình", group: "3. Kế hoạch ngày" },
            TTLapTrinh: { cap: "TT Lập trình", group: "3. Kế hoạch ngày" },
            KHMay: { cap: "KH May", group: "3. Kế hoạch ngày" },
            TTMay: { cap: "TT May", group: "3. Kế hoạch ngày" },
            ThoatChuyen: { cap: "KH Thoát chuyền", group: "3. Kế hoạch ngày" },
            TTThoatChuyen: { cap: "TT Thoát chuyền", group: "3. Kế hoạch ngày" },

            TT_Cat: { cap: "Cắt", group: "3. Kế hoạch ngày" },
            RaChuyen: { cap: "Output", group: "3. Kế hoạch ngày" },
            TP_KiemDat: { cap: "TP Kiểm đạt "},
            TP_Nhan: { cap: "Nhận TP"},
            KCS_KiemDat: { cap: "KCS Kiểm đạt"},
            BTP_DK: { cap: "Đăng ký nhận BTP", group: "4. WIP/Cost/CM" },

            KH_Top: { cap: "Kế hoạch" },
            SLSizeMau_Top: { cap: "SL_Size_màu" },
            TT_MauTop: { cap: "Thực tế" },

            // --- Shipment sample ---
            KH_Ship: { cap: "Kế hoạch" },
            SLSizeMau_Ship: { cap: "SL_Size_màu" },
            TT_Ship: { cap: "Thực tế" },

            // --- Vải ---
            KH_Vai: { cap: "Kế hoạch" },
            TT_Vai: { cap: "Thực tế" },

            // --- PL In ép-LT ---
            KH_PLInEp: { cap: "Kế hoạch" },
            TT_PLInEp: { cap: "Thực tế" },

            // --- PL may ---
            KH_PLMay: { cap: "Kế hoạch" },
            TT_PLMay: { cap: "Thực tế" },

            // --- Packing list (= input) ---
            KH_PackingList: { cap: "Kế hoạch" },
            TT_PackingList: { cap: "Thực tế" },

            // --- PL đóng gói (+8 kich thùng) ---
            KH_PLDongGoi: { cap: "Kế hoạch" },
            TT_PLDongGoi: { cap: "Thực tế" },

            KH_LenhSX: { cap: "Kế hoạch" },
            TT_LenhSX: { cap: "Thực tế" },

            SMV_WIP: { cap: "SMV(phút)", group: "4. WIP/Cost/CM" },
            NSCost: { cap: "NS theo Cost(pcs)", group: "4. WIP/Cost/CM" },
            NS_SMV: { cap: "NS theo SMV(pcs)", group: "4. WIP/Cost/CM" },
            CostPerM: { cap: "Chi phí phút(usd)", group: "4. WIP/Cost/CM" },
            CM: { cap: "CM(usd)", group: "4. WIP/Cost/CM" },
            HeSoCM: { cap: "Hệ số CM", group: "4. WIP/Cost/CM" },
            DoanhThuCM: { cap: "Doanh thu CM chuyền(usd)", group: "4. WIP/Cost/CM" },
            Profit: { cap: "Hệ số lợi nhuận", group: "4. WIP/Cost/CM" },
            // ===== Phòng kỹ thuật =====
            NgayNhanTT_BOM: { cap: "Ngày nhận thông tin" },
            KH_BOM: { cap: "Kế hoạch" },
            TT_BOM: { cap: "Thực tế" },

            KH_NPL_BM_KTX_MayMau: { cap: "Kế hoạch" },
            TT_NPL_BM_KTX_MayMau: { cap: "Thực tế" },

            KH_TestKeoLogo_TapeLaser: { cap: "Kế hoạch" },
            TT_TestKeoLogo_TapeLaser: { cap: "Thực tế" },

            KH_Rap: { cap: "Kế hoạch" },
            TT_Rap: { cap: "Thực tế" },

            KH_TacNghiepCat: { cap: "Kế hoạch" },
            TT_TacNghiepCat: { cap: "Thực tế" },

            KH_SoDo: { cap: "Kế hoạch" },
            TT_SoDo: { cap: "Thực tế" },

            KH_BangMauVai_InEp: { cap: "Kế hoạch" },
            TT_BangMauVai_InEp: { cap: "Thực tế" },

            KH_BangMauPL_Full: { cap: "Kế hoạch" },
            TT_BangMauPL_Full: { cap: "Thực tế" },

            KH_BangDanhSo: { cap: "Kế hoạch" },
            TT_BangDanhSo: { cap: "Thực tế" },

            KH_TLieuInEp: { cap: "Kế hoạch" },
            TT_TLieuInEp: { cap: "Thực tế" },

            KH_TLieuLT: { cap: "Kế hoạch" },
            TT_TLieuLT: { cap: "Thực tế" },

            KH_TSCatPL: { cap: "Kế hoạch" },
            TT_TSCatPL: { cap: "Thực tế" },

            KH_CoiNut: { cap: "Kế hoạch" },
            TT_CoiNut: { cap: "Thực tế" },

            KH_Layout: { cap: "Kế hoạch" },
            TT_Layout: { cap: "Thực tế" },

            KH_NhanCare: { cap: "Kế hoạch" },
            TT_NhanCare: { cap: "Thực tế" },
            DateTrenNhanCare: { cap: "Date trên nhãn" },

            KH_CbiSXChoCoDien: { cap: "Kế hoạch" },
            TT_CbiSXChoCoDien: { cap: "Thực tế" },

            KH_TaiLieuHoanChinh: { cap: "Kế hoạch" },
            TT_TaiLieuHoanChinh: { cap: "Thực tế" },

            KH_KichThuoc: { cap: "Kế hoạch" },
            TT_KichThuoc: { cap: "Thực tế" },

            KH_QuyCachDongGoi: { cap: "Kế hoạch" },
            TT_QuyCachDongGoi: { cap: "Thực tế" },

            KH_HopTKSX: { cap: "Kế hoạch" },
            TT_HopTKSX: { cap: "Thực tế" },

            KH_NhanBaoThung: { cap: "Kế hoạch" },
            TT_NhanBaoThung: { cap: "Thực tế" },

            KH_MauDauChuyen: { cap: "Kế hoạch" },
            TT_MauDauChuyen: { cap: "Thực tế" },

            KH_DuyetMauDauChuyen: { cap: "Kế hoạch" },
            TT_DuyetMauDauChuyen: { cap: "Thực tế" },

            // ===== IE =====
            KH_NhuCauMayMoc: { cap: "Kế hoạch" },
            TT_NhuCauMayMoc: { cap: "Thực tế" },

            KH_Layout_IE: { cap: "Kế hoạch" },
            TT_Layout_IE: { cap: "Thực tế" },

            KH_QTMay: { cap: "Kế hoạch" },
            TT_QTMay: { cap: "Thực tế" },

            KH_DonGiaCongNhan: { cap: "Kế hoạch" },
            TT_DonGiaCongNhan: { cap: "Thực tế" },

            // ===== Cơ điện =====
            KH_ChuanBiMayMoc_KTX: { cap: "Kế hoạch" },
            TT_ChuanBiMayMoc_KTX: { cap: "Thực tế" },

            KH_ChuanBiMayMoc_Chuyen: { cap: "Kế hoạch" },
            TT_ChuanBiMayMoc_Chuyen: { cap: "Thực tế" },

            // ===== Kho =====
            KH_KhoVai: { cap: "Kế hoạch" },
            TT_KhoVai: { cap: "Thực tế" },

            KH_KhoPhuLieuMay: { cap: "Kế hoạch" },
            TT_KhoPhuLieuMay: { cap: "Thực tế" },

            KH_Thung: { cap: "Kế hoạch" },
            TT_Thung: { cap: "Thực tế" },
            GhiChuKho: { cap: "Ghi chú của kho" },
        };

        var FALLBACK_FIELD_DATA_TYPES = buildFallbackFieldDataTypeMap();

        function buildFallbackFieldDataTypeMap() {
            var map = Object.create(null);

            function setType(fields, dataType) {
                (fields || []).forEach(function (field) {
                    var key = normalizeRegistryKey(field);
                    if (key) map[key] = dataType;
                });
            }

            setType([
                "KHCat", "TTCat", "KHLapTrinh", "TTLapTrinh",
                "KHMay", "TTMay", "ThoatChuyen", "TTThoatChuyen",
                "KH_Top", "TT_MauTop", "KH_Ship", "TT_Ship",
                "KH_Vai", "KH_PLInEp",
                "KH_PLMay", "KH_PackingList",
                "KH_PLDongGoi", "KH_LenhSX",
                "NgayNhanTT_BOM",
                "KH_BOM", "TT_BOM",
                "KH_NPL_BM_KTX_MayMau", "TT_NPL_BM_KTX_MayMau",
                "KH_TestKeoLogo_TapeLaser", "TT_TestKeoLogo_TapeLaser",
                "KH_Rap", "TT_Rap",
                "KH_TacNghiepCat", "TT_TacNghiepCat",
                "KH_SoDo", "TT_SoDo",
                "KH_BangMauVai_InEp", "TT_BangMauVai_InEp",
                "KH_BangMauPL_Full", "TT_BangMauPL_Full",
                "KH_BangDanhSo", "TT_BangDanhSo",
                "KH_TLieuInEp", "TT_TLieuInEp",
                "KH_TLieuLT", "TT_TLieuLT",
                "KH_TSCatPL", "TT_TSCatPL",
                "KH_CoiNut", "TT_CoiNut",
                "KH_Layout", "TT_Layout",
                "KH_NhanCare", "TT_NhanCare", "DateTrenNhanCare",
                "KH_CbiSXChoCoDien", "TT_CbiSXChoCoDien",
                "KH_TaiLieuHoanChinh", "TT_TaiLieuHoanChinh",
                "KH_KichThuoc", "TT_KichThuoc",
                "KH_QuyCachDongGoi", "TT_QuyCachDongGoi",
                "KH_HopTKSX", "TT_HopTKSX",
                "KH_NhanBaoThung", "TT_NhanBaoThung",
                "KH_DuyetMauDauChuyen", "TT_DuyetMauDauChuyen",
                "KH_MauDauChuyen",
                "TT_KhoVai", "TT_KhoPhuLieuMay", "TT_Thung", "KH_KhoVai", "KH_KhoPhuLieuMay", "KH_Thung",
                "KH_GuiInTheu", "TT_GuiInTheu", "KH_NhanInTheu", "TT_NhanInTheu",
                "TT_PackingList", "TT_LenhSX"
            ], "date");

            setType(["InTheu", "DoKim", "HutAm", "InTheuTrong_PKH"], "bool");

            setType([
                "SLKH", "NSGio", "SoGioSX", "SoNgay",
                "SLCN", "TGLV", "OT", "HieuSuat",
                "AllocatedRegularHours", "AllocatedOTHours", 
                "TT_Cat", "BTP_DK", "RaChuyen",
                "SMV_WIP", "NSCost", "NS_SMV", "CostPerM", "Profit",
                "CM", "HeSoCM", "DoanhThuCM",
                "LenhSX", "ThuTuChuyen", "LineX"
            ], "number");

            return map;
        }

        function getFallbackFieldDataType(field) {
            var key = normalizeRegistryKey(field);
            return key ? (FALLBACK_FIELD_DATA_TYPES[key] || "") : "";
        }

        function getFieldConfig(field) {
            var fieldKey = String(field || "").trim();
            if (!fieldKey) return null;

            var staticConfig = FIELD_META[fieldKey] || null;
            var dynamicConfig = getColumnMetaByField(fieldKey);
            var fallbackDataType = getFallbackFieldDataType(fieldKey);
            if (!staticConfig && !dynamicConfig && !fallbackDataType) return null;

            var config = {};
            if (staticConfig) {
                Object.keys(staticConfig).forEach(function (key) {
                    config[key] = staticConfig[key];
                });
            }

            if (dynamicConfig) {
                config.field = dynamicConfig.dataField || fieldKey;
                config.dataField = dynamicConfig.dataField || fieldKey;
                config.dataType = dynamicConfig.hasDataType ? dynamicConfig.dataType : (config.dataType || fallbackDataType);
                config.stepCode = dynamicConfig.stepCode || config.stepCode || null;
                config.captionPath = dynamicConfig.captionPath || config.captionPath || "";
                config.caption = dynamicConfig.caption || config.caption || "";
                config.cap = config.cap || dynamicConfig.caption || fieldKey;
            }

            config.field = config.field || fieldKey;
            config.dataField = config.dataField || fieldKey;
            config.cap = config.cap || config.caption || fieldKey;
            config.dataType = normalizeColumnDataType(config.dataType || fallbackDataType);

            return config;
        }

        function getFieldDataType(field, rowData) {
            var config = getFieldConfig(field);
            var dataType = config && config.dataType ? normalizeColumnDataType(config.dataType) : "";
            if (dataType && dataType !== "text") return dataType;

            var currentValue = rowData && field ? rowData[field] : null;
            if (typeof currentValue === "boolean") return "bool";
            if (typeof currentValue === "number") return "number";

            return dataType || "text";
        }

        function getFieldEditorType(field, rowData) {
            var dataType = getFieldDataType(field, rowData);
            if (dataType === "date") return "date";
            if (dataType === "bool" || dataType === "boolean") return "bool";
            if (dataType === "number") return "number";
            return "text";
        }

        function getFieldCaption(field, fallbackCaption) {
            var config = getFieldConfig(field);
            var caption = config && (config.cap || config.caption);
            if (caption) return String(caption).trim();

            var fallback = String(fallbackCaption || "").trim();
            return fallback || String(field || "").trim();
        }

        window.WIP.getFieldConfig = getFieldConfig;
        window.WIP.getFieldDataType = getFieldDataType;
        window.WIP.getFieldEditorType = getFieldEditorType;
        window.WIP.getFieldCaption = getFieldCaption;

        // REGION 06 - Audit, màu header, column filter và style rules.
        // TODO DB-CONFIG: caption audit theo nhóm field nên lấy từ metadata cột/step để tránh lệch với màn hình chính.
        var AUDIT_FIELD_CAPTION_GROUPS = [
            { context: "Mẫu TOP", fields: { KH_Top: "KH", TT_MauTop: "TT", SLSizeMau_Top: "SL size/màu" } },
            { context: "Shipment sample", fields: { KH_Ship: "KH", TT_Ship: "TT", SLSizeMau_Ship: "SL size/màu" } },
            { context: "Vải", fields: { KH_Vai: "KH", TT_Vai: "TT" } },
            { context: "PL In ép-LT", fields: { KH_PLInEp: "KH", TT_PLInEp: "TT" } },
            { context: "PL may", fields: { KH_PLMay: "KH", TT_PLMay: "TT" } },
            { context: "Packing list", fields: { KH_PackingList: "KH", TT_PackingList: "TT" } },
            { context: "PL đóng gói", fields: { KH_PLDongGoi: "KH", TT_PLDongGoi: "TT" } },
            { context: "Lệnh SX", fields: { KH_LenhSX: "KH", TT_LenhSX: "TT" } },
            { context: "BOM", fields: { KH_BOM: "KH", TT_BOM: "TT", NgayNhanTT_BOM: "Ngày nhận TT" } },
            { context: "NPL BM+KTX may mẫu", fields: { KH_NPL_BM_KTX_MayMau: "KH", TT_NPL_BM_KTX_MayMau: "TT" } },
            { context: "Test keo logo - tape laser", fields: { KH_TestKeoLogo_TapeLaser: "KH", TT_TestKeoLogo_TapeLaser: "TT" } },
            { context: "Rập", fields: { KH_Rap: "KH", TT_Rap: "TT" } },
            { context: "Tác nghiệp cắt", fields: { KH_TacNghiepCat: "KH", TT_TacNghiepCat: "TT" } },
            { context: "Sơ đồ", fields: { KH_SoDo: "KH", TT_SoDo: "TT" } },
            { context: "Bảng màu vải + in ép", fields: { KH_BangMauVai_InEp: "KH", TT_BangMauVai_InEp: "TT" } },
            { context: "Bảng màu PL + full", fields: { KH_BangMauPL_Full: "KH", TT_BangMauPL_Full: "TT" } },
            { context: "Bảng đánh số", fields: { KH_BangDanhSo: "KH", TT_BangDanhSo: "TT" } },
            { context: "TLiệu in ép", fields: { KH_TLieuInEp: "KH", TT_TLieuInEp: "TT" } },
            { context: "TLiệu LT", fields: { KH_TLieuLT: "KH", TT_TLieuLT: "TT" } },
            { context: "TS cắt PL", fields: { KH_TSCatPL: "KH", TT_TSCatPL: "TT" } },
            { context: "Cối nút", fields: { KH_CoiNut: "KH", TT_CoiNut: "TT" } },
            { context: "Layout nhãn", fields: { KH_Layout: "KH", TT_Layout: "TT" } },
            { context: "Nhãn care", fields: { KH_NhanCare: "KH", TT_NhanCare: "TT", DateTrenNhanCare: "Date trên nhãn" } },
            { context: "Cbị SX cho cơ điện", fields: { KH_CbiSXChoCoDien: "KH", TT_CbiSXChoCoDien: "TT" } },
            { context: "Tài liệu hoàn chỉnh", fields: { KH_TaiLieuHoanChinh: "KH", TT_TaiLieuHoanChinh: "TT" } },
            { context: "Kích thước", fields: { KH_KichThuoc: "KH", TT_KichThuoc: "TT" } },
            { context: "Quy cách đóng gói", fields: { KH_QuyCachDongGoi: "KH", TT_QuyCachDongGoi: "TT" } },
            { context: "Họp TKSX", fields: { KH_HopTKSX: "KH", TT_HopTKSX: "TT" } },
            { context: "Nhãn bao thùng", fields: { KH_NhanBaoThung: "KH", TT_NhanBaoThung: "TT" } },
            { context: "Mẫu đầu chuyền", fields: { KH_MauDauChuyen: "KH", TT_MauDauChuyen: "TT" } },
            { context: "Duyệt mẫu đầu chuyền", fields: { KH_DuyetMauDauChuyen: "KH", TT_DuyetMauDauChuyen: "TT" } },
            { context: "Nhu cầu máy móc", fields: { KH_NhuCauMayMoc: "KH", TT_NhuCauMayMoc: "TT" } },
            { context: "Layout IE", fields: { KH_Layout_IE: "KH", TT_Layout_IE: "TT" } },
            { context: "QT may", fields: { KH_QTMay: "KH", TT_QTMay: "TT" } },
            { context: "Đơn giá công nhân", fields: { KH_DonGiaCongNhan: "KH", TT_DonGiaCongNhan: "TT" } },
            { context: "Duyệt mockup công đoạn máy", fields: { KH_ChuanBiMayMoc_KTX: "KH", TT_ChuanBiMayMoc_KTX: "TT" } },
            { context: "Chuẩn bị, bàn giao máy móc cho chuyền", fields: { KH_ChuanBiMayMoc_Chuyen: "KH", TT_ChuanBiMayMoc_Chuyen: "TT" } },
            { context: "Cấp phát vải", fields: { KH_KhoVai: "KH", TT_KhoVai: "TT" } },
            { context: "Cấp phát phụ liệu may", fields: { KH_KhoPhuLieuMay: "KH", TT_KhoPhuLieuMay: "TT" } },
            { context: "Cấp phát thùng", fields: { KH_Thung: "KH", TT_Thung: "TT" } }
        ];

        function buildAuditFieldCaptionByField(groups) {
            var output = {};
            var items = groups || [];

            items.forEach(function (groupItem) {
                var context = String(groupItem && groupItem.context || "").trim();
                var fields = groupItem && groupItem.fields ? groupItem.fields : {};
                Object.keys(fields).forEach(function (field) {
                    var suffix = String(fields[field] || "").trim();
                    output[field] = suffix ? (context + " - " + suffix) : context;
                });
            });

            return output;
        }

        var AUDIT_FIELD_CAPTION_BY_FIELD = buildAuditFieldCaptionByField(AUDIT_FIELD_CAPTION_GROUPS);

        // DOM id của audit popup vẫn giữ cố định vì controller đang bind trực tiếp vào markup động.
        var AUDIT_POPUP_IDS = {
            popup: "wipAuditPopup",
            header: "wipAuditHeader",
            filter: "wipAuditFilter",
            from: "wipAuditFrom",
            to: "wipAuditTo",
            apply: "wipAuditApply",
            clear: "wipAuditClear",
            grid: "wipAuditGrid"
        };

        function formatAuditFieldName(fieldName) {
            return auditPopupController.formatFieldName(fieldName);
        }

        function parseAuditBooleanLike(value) {
            return auditPopupController.parseBooleanLike(value);
        }

        function formatAuditDefaultValue(value) {
            return auditPopupController.formatDefaultValue(value);
        }

        function formatAuditBooleanValue(value) {
            return auditPopupController.formatBooleanValue(value);
        }

        var AUDIT_FIELD_VALUE_MAPPERS = {
            InTheu: formatAuditBooleanValue,
            DoKim: formatAuditBooleanValue,
            HutAm: formatAuditBooleanValue,
            InTheuTrong_PKH: formatAuditBooleanValue
        };

        function formatAuditValueByField(fieldName, value) {
            return auditPopupController.formatValueByField(fieldName, value);
        }

        function mapAuditApiItem(item) {
            return auditPopupController.mapApiItem(item);
        }

        function renderAuditHeader(rowData) {
            auditPopupController.renderHeader(rowData || {});
        }

        function createAuditValueCell(className) {
            return auditPopupController.createValueCell(className);
        }
        function ensureAuditFilterUi() {
            auditPopupController.ensureFilterUi();
        }

        function ensureAuditPopup() {
            return auditPopupController.ensurePopup();
        }

        function loadAuditData(wipId, fromDate, toDate) {
            auditPopupController.loadData(wipId, fromDate, toDate);
        }


        function openAuditPopup(rowData) {
            auditPopupController.open(rowData);
        }

        //Action mở popup từ selection
        function openAssignPopupFromSelection() {
            if (sortMode) return;

            //if (!selectedLineX) {
            //    DevExpress.ui.notify("Hãy chọn 1 chuyền cụ thể trước.", "warning", 1800);
            //    return;
            //}

            let rowsMoved = [];
            try { rowsMoved = gridUnassigned?.getSelectedRowsData?.() || []; } catch { }

            if (!rowsMoved.length) {
                DevExpress.ui.notify("Hãy tick chọn 1 hoặc nhiều đơn hàng (hoặc kéo 1 dòng).", "info", 2200);
                return;
            }

            // baseline để approve lấy MaKH/MaHang
            const base = rowsMoved[0] || {};

            const maDHJoined = rowsMoved
                .map(r => String(r.MaDH || "").trim())
                .filter(Boolean)
                .join(";");

            const wipIds = rowsMoved
                .map(r => Number(r.WIPId ?? r.id))
                .filter(x => Number.isFinite(x) && x > 0);
            const ghiChuGop = buildGhiChuGopFromRows(rowsMoved);
            pendingCross = {
                item: base,         // ✅ baseline row
                items: rowsMoved,   // ✅ full list
                maDH: maDHJoined,   // ✅ "DH_1;DH_2"
                lineX: selectedLineX,
                wipIds,
                rowsMoved,
                ghiChu: ghiChuGop
            };

            window.WIP?.cross?.set?.(pendingCross);

            const maHangJoined = String(base?.MaHang || "").trim();
            openCrossPopup(maDHJoined, selectedLineX, maHangJoined);

            setTimeout(function () {
                fillPopup_MaDH_LineX(maDHJoined, selectedLineX, maHangJoined);
            }, 0);
        }


        // Grid/header style rules.
        // TODO DB-CONFIG: class màu header và mapping caption -> class nên chuyển sang metadata header group.
        var HEADER_COLOR_CLASS_SET = {
            "hdr-donhang": true,
            "hdr-kh-ngay": true,
            "hdr-tiendo-thucte": true,
            "hdr-nangsuat": true,
            "hdr-qc": true,
            "hdr-merchandiser": true,
            "hdr-kythuat": true,
            "hdr-ie": true,
            "hdr-codien": true,
            "hdr-intheu": true,
            "hdr-dongbo-intheu": true,
            "hdr-thoigian": true,
            "hdr-tinhtoan-nangsuat": true
        };

        var TOP_BAND_HEADER_CLASS_BY_CAPTION = {
            "Đơn hàng": "hdr-donhang",
            "IN-ÉP-THÊU-LASER": "hdr-intheu",
            "Đồng bộ in/thêu": "hdr-dongbo-intheu",
            "Năng suất": "hdr-nangsuat",
            "Thời gian": "hdr-thoigian",
            "Tiến độ": "hdr-tiendo-thucte",
            "QC": "hdr-qc",
            "Merchandiser": "hdr-merchandiser",
            "Tính toán năng suất": "hdr-tinhtoan-nangsuat",
            "Phòng kỹ thuật": "hdr-kythuat",
            "IE": "hdr-ie",
            "Cơ điện": "hdr-codien"
        };

        function splitClassNames(value) {
            return String(value || "").split(/\s+/).filter(Boolean);
        }

        function uniqueClassNames(classNames) {
            return classNames.filter(Boolean).filter(function (name, index, items) {
                return items.indexOf(name) === index;
            });
        }

        function addClassName(value, className) {
            var classNames = splitClassNames(value);
            classNames.push(className);
            return uniqueClassNames(classNames).join(" ");
        }

        function removeClassName(value, className) {
            return splitClassNames(value).filter(function (name) {
                return name !== className;
            }).join(" ");
        }

        function normalizeHeaderColorClassName(name) {
            if (!name) return "";
            if (name === "hdr-IE") return "hdr-ie";
            return name;
        }

        function getUtilityClasses(value) {
            return splitClassNames(value)
                .map(normalizeHeaderColorClassName)
                .filter(function (name) { return !HEADER_COLOR_CLASS_SET[name]; });
        }

        function getPrefixedHeaderClass(field) {
            if (!field) return "";

            if (field.indexOf("KH_") === 0 || field.indexOf("KH") === 0 || field === "ThoatChuyen") return "hdr-kh-ngay";
            if (field.indexOf("TT_") === 0 || field.indexOf("TT") === 0) return "hdr-tiendo-thucte";

            return "";
        }

        function resolveTopBandHeaderClass(caption) {
            return TOP_BAND_HEADER_CLASS_BY_CAPTION[String(caption || "").trim()] || "";
        }

        function getDataCellCssClass(cssClass) {
            return uniqueClassNames(getUtilityClasses(cssClass)).join(" ");
        }

        function applyDynamicGroupEnd(columns) {
            function clearLeafGroupEnd(column) {
                if (!column) return;

                column.cssClass = removeClassName(column.cssClass, "group-end");
                column.headerCssClass = removeClassName(column.headerCssClass, "group-end");

                if (!column.cssClass) delete column.cssClass;
                if (!column.headerCssClass) delete column.headerCssClass;
            }

            function setLeafGroupEnd(column) {
                if (!column) return;

                column.cssClass = addClassName(column.cssClass, "group-end");
                column.headerCssClass = addClassName(column.headerCssClass, "group-end");
            }

            function walk(items) {
                (items || []).forEach(function (column) {
                    if (!column) return;

                    var hasChildren = Array.isArray(column.columns) && column.columns.length > 0;
                    if (!hasChildren) return;

                    walk(column.columns);

                    var visibleChildren = column.columns.filter(function (child) {
                        return child && child.visible !== false;
                    });

                    column.columns.forEach(function (child) {
                        var isLeaf = child && (!Array.isArray(child.columns) || child.columns.length === 0);
                        if (isLeaf) clearLeafGroupEnd(child);
                    });

                    if (!visibleChildren.length) return;

                    var lastVisibleChild = visibleChildren[visibleChildren.length - 1];
                    var lastVisibleChildIsLeaf = lastVisibleChild && (!Array.isArray(lastVisibleChild.columns) || lastVisibleChild.columns.length === 0);

                    if (lastVisibleChildIsLeaf) {
                        setLeafGroupEnd(lastVisibleChild);
                    }
                });
            }

            walk(columns);
            return columns;
        }

        function prepareGridColumns(columns, permissionLineX) {
            function walk(items, inheritedHeaderColorClass, depth, topBandCaption) {
                return (items || []).map(function (column) {
                    var current = $.extend(true, {}, column);
                    var hasChildren = Array.isArray(current.columns) && current.columns.length > 0;
                    var currentHeaderColorClass = inheritedHeaderColorClass;
                    var currentTopBandCaption = depth === 0 ? String(current.caption || "").trim() : topBandCaption;
                    var topLevelBandClass = depth === 0 ? resolveTopBandHeaderClass(current.caption) : "";
                    var headerClassNames = getUtilityClasses(current.headerCssClass);

                    if (topLevelBandClass) currentHeaderColorClass = topLevelBandClass;

                    if (hasChildren) {
                        current.columns = walk(current.columns, currentHeaderColorClass, depth + 1, currentTopBandCaption);
                        if (!current.columns.some(function (child) { return child && child.visible !== false; })) {
                            current.visible = false;
                        }
                    } else {
                        var prefixedHeaderClass = getPrefixedHeaderClass(current.dataField);

                        if (prefixedHeaderClass) currentHeaderColorClass = prefixedHeaderClass;

                        current.cssClass = getDataCellCssClass(current.cssClass);

                        if (!current.cssClass) delete current.cssClass;

                        var shouldBypassViewPermission = currentTopBandCaption === "Đơn hàng";
                        if (current.dataField && current.visible !== false && !shouldBypassViewPermission && !canViewFieldByPermission(current.dataField, permissionLineX)) {
                            current.visible = false;
                        }
                    }

                    if (currentHeaderColorClass) {
                        headerClassNames.unshift(currentHeaderColorClass);
                    }

                    current.headerCssClass = uniqueClassNames(headerClassNames).join(" ");

                    if (!current.headerCssClass) delete current.headerCssClass;

                    return current;
                });
            }

            return walk(columns, "", 0, "");
        }
        // ========================= FILTER Hiển thị cột theo user =========================
        function getColumnFilterStorageKey() {
            return columnFilterPopupController.getStorageKey();
        }

        function normalizeHiddenColumnFields(fields) {
            return columnFilterPopupController.normalizeHiddenFields(fields);
        }

        function loadHiddenColumnFields() {
            return columnFilterPopupController.loadHiddenFields();
        }

        function saveHiddenColumnFields(fields) {
            columnFilterPopupController.saveHiddenFields(fields);
        }

        function buildHiddenColumnFieldMap() {
            return columnFilterPopupController.buildHiddenFieldMap();
        }

        function isUserColumnHidden(field) {
            return columnFilterPopupController.isUserColumnHidden(field);
        }

        function normalizeColumnFilterCaption(value) {
            return columnFilterPopupController.normalizeCaption(value);
        }

        function applyUserColumnVisibility(columns) {
            return columnFilterPopupController.applyUserColumnVisibility(columns);
        }

        function applyLineModeColumnVisibility(columns, honorUserHidden) {
            function isVisibleByMode(field) {
                if (field === "LineName") return !selectedIsGiaCong;
                if (field === "TenDVSX") return !!selectedIsGiaCong;
                return null;
            }

            function walk(items) {
                (items || []).forEach(function (column) {
                    if (!column) return;

                    var hasChildren = Array.isArray(column.columns) && column.columns.length > 0;
                    if (hasChildren) {
                        walk(column.columns);

                        if (!column.columns.some(function (child) {
                            return child && child.visible !== false;
                        })) {
                            column.visible = false;
                        } else if (column.visible === false) {
                            delete column.visible;
                        }
                        return;
                    }

                    var visibleByMode = isVisibleByMode(column.dataField);
                    if (visibleByMode === null) return;

                    column.visible = visibleByMode && !(honorUserHidden && isUserColumnHidden(column.dataField));
                });
            }

            walk(columns);
            return columns;
        }

        function buildColumnFilterTreeData(columns) {
            return columnFilterPopupController.buildTreeData(columns);
        }

        function ensureColumnFilterButtonElement() {
            columnFilterPopupController.ensureButtonElement();
        }

        function ensureColumnFilterPopup() {
            columnFilterPopupController.ensurePopup();
        }

        function setColumnFilterSelection(isSelected) {
            columnFilterPopupController.setSelection(isSelected);
        }

        function getSelectedColumnFilterFieldMap() {
            return columnFilterPopupController.getSelectedFieldMap();
        }

        function setColumnFilterSavingState(isSaving) {
            columnFilterPopupController.setSavingState(isSaving);
        }

        function openColumnFilterPopup() {
            columnFilterPopupController.open();
        }

        function saveColumnFilterPopup() {
            columnFilterPopupController.save();
        }
        // ============================================================= //
        function numberGridLeafCaptions(columns) {
            var order = 1;

            function stripLeadingOrder(caption) {
                // Hàm xóa số đã đánh trong grid
                return String(caption || "").replace(/^\s*(?:\(\d+\)|\d+)\s+/, "").trim();
            }

            function walk(items) {
                (items || []).forEach(function (column) {
                    if (column && column.visible === false) return;

                    var hasChildren = Array.isArray(column.columns) && column.columns.length > 0;
                    if (hasChildren) {
                        walk(column.columns);
                        return;
                    }

                    var baseCaption = stripLeadingOrder(column.caption);
                    if (!baseCaption && column.dataField) {
                        baseCaption = stripLeadingOrder(getFieldCaption(column.dataField, column.dataField));
                    }
                    if (!baseCaption) {
                        baseCaption = String(column.dataField || "");
                    }

                    //column.caption = "(" + order +")" + " " + baseCaption;
                    column.caption = `(${order}) ${baseCaption}`;
                    order += 1;
                });
            }

            walk(columns);
            return columns;
        }
        // Grid column rebuild và visibility theo permission/line.
        function rebuildGridColumnsByPermission(lineX) {
            if (!grid || !baseGridColumns) return;
            var permissionLineX = (lineX === null || lineX === undefined || lineX === "") ? undefined : lineX;

            var columns = prepareGridColumns($.extend(true, [], baseGridColumns), permissionLineX);
            columns = applyUserColumnVisibility(columns);
            columns = applyLineModeColumnVisibility(columns, true);
            columns = numberGridLeafCaptions(applyDynamicGroupEnd(columns));

            grid.beginUpdate();
            grid.option("columns", columns);
            grid.endUpdate();

            try { grid.repaint(); } catch (e) { }
            try { grid.updateDimensions(); } catch (e) { }
        }

        function normalizeHeaderColorConfig(config) {
            if (!config) return {};

            if (Array.isArray(config)) {
                return config.reduce(function (acc, item) {
                    if (!item || typeof item !== "object") return acc;

                    var key = item.ColorKey || item.colorKey || item.Key || item.key;
                    var value = item.ColorValue || item.colorValue || item.Value || item.value;

                    if (key && value) acc[key] = value;

                    return acc;
                }, {});
            }

            if (typeof config === "object") return config;

            return {};
        }

        function applyHeaderColors(config) {
            var normalizedConfig = normalizeHeaderColorConfig(config);
            var rootStyle = document.documentElement.style;
            var colorVarMap = {
                "grid-header-default": "--grid-header-default-bg",
                "grid-header-default-text": "--grid-header-default-text",
                "hdr-donhang": "--hdr-donhang-bg",
                "hdr-donhang-text": "--hdr-donhang-text",
                "hdr-kh-ngay": "--hdr-kh-ngay-bg",
                "hdr-kh-ngay-text": "--hdr-kh-ngay-text",
                "hdr-qc": "--hdr-qc-bg",
                "hdr-qc-text": "--hdr-qc-text",
                "hdr-merchandiser": "--hdr-merchandiser-bg",
                "hdr-merchandiser-text": "--hdr-merchandiser-text",
                "hdr-nangsuat": "--hdr-nangsuat-bg",
                "hdr-nangsuat-text": "--hdr-nangsuat-text",
                "hdr-tiendo-thucte": "--hdr-tiendo-thucte-bg",
                "hdr-tiendo-thucte-text": "--hdr-tiendo-thucte-text",
                "hdr-kythuat": "--hdr-kythuat-bg",
                "hdr-kythuat-text": "--hdr-kythuat-text",
                "hdr-ie": "--hdr-ie-bg",
                "hdr-ie-text": "--hdr-ie-text",
                "hdr-IE": "--hdr-ie-bg",
                "hdr-IE-text": "--hdr-ie-text",
                "hdr-codien": "--hdr-codien-bg",
                "hdr-codien-text": "--hdr-codien-text",
                "hdr-intheu": "--hdr-intheu-bg",
                "hdr-intheu-text": "--hdr-intheu-text",
                "hdr-dongbo-intheu": "--hdr-dongbo-intheu-bg",
                "hdr-dongbo-intheu-text": "--hdr-dongbo-intheu-text",
                "hdr-thoigian": "--hdr-thoigian-bg",
                "hdr-thoigian-text": "--hdr-thoigian-text",
                "hdr-tinhtoan-nangsuat": "--hdr-tinhtoan-nangsuat-bg",
                "hdr-tinhtoan-nangsuat-text": "--hdr-tinhtoan-nangsuat-text",
                "manual-cell-bg": "--manual-cell-bg",
                "manual-cell-border": "--manual-cell-border",
                "manual-cell-text": "--manual-cell-text",
                "tt-bom-status-1-bg": "--tt-bom-status-1-bg",
                "tt-bom-status-2-bg": "--tt-bom-status-2-bg",
                "tt-bom-status-3-bg": "--tt-bom-status-3-bg"
            };

            Object.keys(normalizedConfig).forEach(function (key) {
                var normalizedKey = normalizeHeaderColorClassName(String(key).trim());
                var cssVarName = colorVarMap[normalizedKey] || colorVarMap[String(key).trim()];
                var colorValue = normalizedConfig[key];

                if (!cssVarName || !colorValue) return;

                rootStyle.setProperty(cssVarName, String(colorValue).trim());
            });
        }

        window.applyHeaderColors = applyHeaderColors;

        var headerColorConfigSeed = normalizeHeaderColorConfig(window.WIP_HEADER_COLOR_CONFIG);

        applyHeaderColors($.extend(
            {},
            DEFAULT_HEADER_TEXT_COLOR_CONFIG,
            normalizeHeaderColorConfig(window.WIP_HEADER_TEXT_COLOR_CONFIG),
            headerColorConfigSeed
        ));

        function normalizeHexColor(value) {
            var raw = String(value || "").trim().toUpperCase();
            if (!raw) return "";
            if (raw.charAt(0) !== "#") raw = "#" + raw;

            if (/^#[0-9A-F]{3}$/.test(raw)) {
                return "#" + raw.charAt(1) + raw.charAt(1) + raw.charAt(2) + raw.charAt(2) + raw.charAt(3) + raw.charAt(3);
            }

            if (/^#[0-9A-F]{6}$/.test(raw)) return raw;

            return "";
        }

        function normalizeManualCellColorConfig(config) {
            var source = (config && typeof config === "object") ? config : {};
            return {
                Background: normalizeHexColor(source.Background || source.background) || DEFAULT_MANUAL_CELL_COLOR_CONFIG.Background,
                Border: normalizeHexColor(source.Border || source.border) || DEFAULT_MANUAL_CELL_COLOR_CONFIG.Border,
                Text: normalizeHexColor(source.Text || source.text) || DEFAULT_MANUAL_CELL_COLOR_CONFIG.Text
            };
        }

        function applyManualCellColors(config) {
            var normalized = normalizeManualCellColorConfig(config);
            manualCellColorConfig = normalized;
            window.WIP_MANUAL_CELL_COLOR_CONFIG = normalized;
            var rootStyle = document.documentElement.style;
            rootStyle.setProperty("--manual-cell-bg", normalized.Background);
            rootStyle.setProperty("--manual-cell-border", normalized.Border);
            rootStyle.setProperty("--manual-cell-text", normalized.Text);
        }

        window.applyManualCellColors = applyManualCellColors;
        applyManualCellColors({
            Background: headerColorConfigSeed["manual-cell-bg"] || (window.WIP_MANUAL_CELL_COLOR_CONFIG && window.WIP_MANUAL_CELL_COLOR_CONFIG.Background),
            Border: headerColorConfigSeed["manual-cell-border"] || (window.WIP_MANUAL_CELL_COLOR_CONFIG && window.WIP_MANUAL_CELL_COLOR_CONFIG.Border),
            Text: headerColorConfigSeed["manual-cell-text"] || (window.WIP_MANUAL_CELL_COLOR_CONFIG && window.WIP_MANUAL_CELL_COLOR_CONFIG.Text)
        });

        function normalizeStepStatusMetaResponse(res) {
            var source = (res && typeof res === "object") ? res : {};
            var rawStatusCodes = source.StatusCodes || source.statusCodes;
            var statusCodes = Array.isArray(rawStatusCodes)
                ? rawStatusCodes.map(function (x) { return Number(x); }).filter(function (x) { return Number.isFinite(x) && x >= 0; })
                : [];
            if (!statusCodes.length) statusCodes = [0, 1, 2, 3];
            statusCodes = Array.from(new Set(statusCodes)).sort(function (a, b) { return a - b; });

            var rawSteps = Array.isArray(source.Steps) ? source.Steps : (Array.isArray(source.steps) ? source.steps : []);
            var steps = rawSteps.map(function (x) {
                if (!x || typeof x !== "object") return null;
                var ttField = String(x.TTField || x.LegacyTTColumn || x.ttField || "").trim();
                var khField = String(x.KHField || x.LegacyKHColumn || x.khField || "").trim();
                var stepCode = String(x.StepCode || x.stepCode || "").trim();
                if (!ttField || !khField || !stepCode) return null;
                return {
                    StepCode: stepCode,
                    StepName: String(x.StepName || x.stepName || stepCode).trim(),
                    SortOrder: Number(x.SortOrder || x.sortOrder || 0),
                    TTField: ttField,
                    KHField: khField,
                    IsCheckTT: x.IsCheckTT === true || x.IsCheckTT === 1 || String(x.IsCheckTT || "").toLowerCase() === "true",
                    AllowManualKH: x.AllowManualKH === true || x.AllowManualKH === 1 || String(x.AllowManualKH || "").toLowerCase() === "true",
                    AllowNoDataConfirm: normalizeTruthyFlag(x.AllowNoDataConfirm),
                    NoDataField: String(x.NoDataField || x.noDataField || (ttField + "_NoData")).trim(),
                    StatusField: String(x.StatusField || x.statusField || (ttField + "_Status")).trim(),
                    LateLockedField: String(x.LateLockedField || x.lateLockedField || (ttField + "_IsLateLocked")).trim()
                };
            }).filter(Boolean);

            steps.sort(function (a, b) {
                if (a.SortOrder !== b.SortOrder) return a.SortOrder - b.SortOrder;
                return String(a.StepCode).localeCompare(String(b.StepCode));
            });

            var rawCompareTtColumns = Array.isArray(source.CompareTtColumns)
                ? source.CompareTtColumns
                : (Array.isArray(source.compareTtColumns) ? source.compareTtColumns : []);
            var compareTtColumns = rawCompareTtColumns
                .map(function (x) { return String(x || "").trim(); })
                .filter(function (x) { return !!x; });
            if (!compareTtColumns.length) {
                compareTtColumns = steps
                    .filter(function (s) { return s.IsCheckTT; })
                    .map(function (s) { return s.TTField; });
            }

            var byTtField = {};
            steps.forEach(function (s) {
                var key = String(s.TTField || "").trim().toLowerCase();
                if (!key) return;
                byTtField[key] = s;
            });

            return {
                statusCodes: statusCodes,
                steps: steps,
                compareTtColumns: compareTtColumns,
                compareTtColumnsSet: new Set(compareTtColumns.map(function (x) { return String(x || "").trim().toLowerCase(); })),
                byTtField: byTtField
            };
        }

        function normalizeTruthyFlag(v) {
            if (v === true || v === 1) return true;
            var s = String(v == null ? "" : v).trim().toLowerCase();
            return s === "true" || s === "1";
        }

        function isStepStatusComparableField(field) {
            if (!field || !stepStatusMeta || !stepStatusMeta.compareTtColumnsSet) return false;
            return stepStatusMeta.compareTtColumnsSet.has(String(field).trim().toLowerCase());
        }
        function normalizeColumnDataType(rawType) {
            var t = String(rawType || "").trim().toLowerCase();
            if (!t) return "text";
            if (t === "date" || t === "datetime") return "date";
            if (t === "number" || t === "numeric" || t === "int" || t === "float" || t === "decimal") return "number";
            if (t === "bool" || t === "boolean" || t === "bit") return "bool";
            if (t === "group" || t === "band") return "group";
            return t;
        }
        function normalizeColumnDynamicResponse(res) {
            var rows = Array.isArray(res) ? res : [];
            return rows.map(function (x, idx) {
                if (!x || typeof x !== "object") return null;
                var dataField = String(x.DataField || x.dataField || "").trim();
                var stepCode = String(x.StepCode || x.stepCode || "").trim();
                var code = String(x.Code || x.code || "").trim();
                var parentCode = String(x.ParentCode || x.parentCode || "").trim();
                var rawDataType = x.DataType != null ? x.DataType : x.dataType;
                if (!String(rawDataType || "").trim() && x.dataType != null) rawDataType = x.dataType;
                var hasDataType = String(rawDataType || "").trim() !== "";

                return {
                    id: Number(x.Id || x.id || 0),
                    code: code,
                    parentCode: parentCode,
                    module: Number(x.Module || x.module || 0),
                    bandCode: String(x.BandCode || x.bandCode || "").trim(),
                    caption: String(x.Caption || x.caption || "").trim(),
                    dataField: dataField,
                    stepCode: stepCode,
                    dataType: hasDataType ? normalizeColumnDataType(rawDataType) : "",
                    hasDataType: hasDataType,
                    rowSpan: Number(x.RowSpan || x.rowSpan || 1),
                    colSpan: Number(x.ColSpan || x.colSpan || 1),
                    stt: Number(x.STT || x.stt || idx),
                    isCheckTT: x.IsCheckTT === true || x.IsCheckTT === 1 || String(x.IsCheckTT || "").toLowerCase() === "true",
                    allowManualKH: x.AllowManualKH === true || x.AllowManualKH === 1 || String(x.AllowManualKH || "").toLowerCase() === "true"
                };
            }).filter(Boolean);
        }

        function buildColumnRegistry(items) {
            var registry = {
                columnMetaByField: Object.create(null),
                fieldsByStepCode: Object.create(null),
                nonStepFieldsMap: Object.create(null),
                parentFieldsMap: Object.create(null),
                codeMap: Object.create(null)
            };

            var rows = Array.isArray(items) ? items : [];

            rows.forEach(function (r) {
                var codeKey = normalizeRegistryKey(r.code);
                if (codeKey) registry.codeMap[codeKey] = r;
            });

            function resolveNearestParentField(row) {
                var curParentCode = normalizeRegistryKey(row.parentCode);
                var guard = 0;
                while (curParentCode && guard < 20) {
                    guard++;
                    var parentNode = registry.codeMap[curParentCode];
                    if (!parentNode) return "";
                    if (parentNode.dataField) return String(parentNode.dataField).trim();
                    curParentCode = normalizeRegistryKey(parentNode.parentCode);
                }
                return "";
            }
            function resolveCaptionPath(row) {
                if (!row) return "";

                var selfCaption = normalizeDateFilterCaption(row.caption || row.dataField || "");
                var parentNode = registry.codeMap[normalizeRegistryKey(row.parentCode)] || null;
                var parentCaption = parentNode
                    ? normalizeDateFilterCaption(parentNode.caption || parentNode.dataField || "")
                    : "";

                return parentCaption
                    ? (parentCaption + " - " + selfCaption)
                    : selfCaption;
            }

            rows.forEach(function (r) {
                var field = String(r.dataField || "").trim();
                if (!field) return;

                var fieldKey = normalizeRegistryKey(field);
                var stepCode = String(r.stepCode || "").trim();
                var stepKey = normalizeRegistryKey(stepCode);

                registry.columnMetaByField[fieldKey] = {
                    dataField: field,
                    stepCode: stepCode || null,
                    dataType: r.dataType || "text",
                    module: r.module,
                    code: r.code || null,
                    parentCode: r.parentCode || null,
                    bandCode: r.bandCode || null,
                    caption: r.caption || field,
                    captionPath: resolveCaptionPath(r),
                    hasDataType: !!r.hasDataType,
                    stt: r.stt,
                    isCheckTT: !!r.isCheckTT,
                    allowManualKH: !!r.allowManualKH
                };

                if (stepKey) {
                    if (!registry.fieldsByStepCode[stepKey]) registry.fieldsByStepCode[stepKey] = [];
                    if (registry.fieldsByStepCode[stepKey].indexOf(field) < 0) {
                        registry.fieldsByStepCode[stepKey].push(field);
                    }
                } else {
                    registry.nonStepFieldsMap[fieldKey] = true;
                }

                var parentField = resolveNearestParentField(r);
                if (parentField) {
                    registry.parentFieldsMap[fieldKey] = parentField;
                }
            });

            return registry;
        }

        function getColumnMetaByField(dataField) {
            var key = normalizeRegistryKey(dataField);
            return key ? (columnRegistry.columnMetaByField[key] || null) : null;
        }

        function loadColumnDynamicMeta() {
            return wipMetadataService.loadColumnDynamicMeta().then(function (res) {
                columnDynamicMeta = normalizeColumnDynamicResponse(res);
                columnRegistry = buildColumnRegistry(columnDynamicMeta);
                rebuildStepBindingByField();
            }).catch(function () {
                columnDynamicMeta = [];
                columnRegistry = buildColumnRegistry([]);
                rebuildStepBindingByField();
            });
        }
        function normalizeSaveDeltaFieldMetaResponse(res) {
            var rows = Array.isArray(res) ? res : (res && Array.isArray(res.fields) ? res.fields : []);
            return rows.filter(function (x) {
                return x && x.FieldName;
            }).map(function (x) {
                return {
                    FieldName: String(x.FieldName || "").trim(),
                    TargetScope: String(x.TargetScope || "parent").trim(),
                    TargetColumn: String(x.TargetColumn || x.FieldName || "").trim(),
                    DataType: String(x.DataType || "nvarchar").trim().toLowerCase(),
                    RecalcGroup: x.RecalcGroup == null ? null : String(x.RecalcGroup).trim(),
                    ManualGroup: x.ManualGroup == null ? null : String(x.ManualGroup).trim(),
                    IsWritable: x.IsWritable !== false,
                    SortOrder: Number(x.SortOrder || 0)
                };
            });
        }
        function loadSaveDeltaFieldMeta() {
            return wipMetadataService.loadSaveDeltaFieldMeta().then(function (res) {
                saveDeltaFieldMeta = normalizeSaveDeltaFieldMetaResponse(res);
                var parentFields = saveDeltaFieldMeta.filter(function (x) {
                    var scope = normalizeRegistryKey(x.TargetScope);
                    return x.IsWritable
                        && (scope === "parent" || scope === "wip")
                        && x.FieldName;
                }).map(function (x) {
                    return x.FieldName;
                });
                DELTA_SAVE_PARENT_FIELD_MAP = buildSaveDeltaParentFieldMap(parentFields.length ? parentFields : FALLBACK_DELTA_SAVE_PARENT_FIELDS);
            }).catch(function () {
                saveDeltaFieldMeta = [];
                DELTA_SAVE_PARENT_FIELD_MAP = buildSaveDeltaParentFieldMap(FALLBACK_DELTA_SAVE_PARENT_FIELDS);
            });
        }
        function loadStepStatusMeta() {
            return wipMetadataService.loadStepStatusMeta().then(function (res) {
                stepStatusMeta = normalizeStepStatusMetaResponse(res);
                stepRegistry = buildStepRegistry(stepStatusMeta);
                rebuildStepBindingByField();
                TT_BOM_STATUS_KEYS = (stepStatusMeta && Array.isArray(stepStatusMeta.statusCodes) && stepStatusMeta.statusCodes.length)
                    ? stepStatusMeta.statusCodes.slice()
                    : [0, 1, 2, 3];
                applyTtBomStatusColors(ttBomStatusColorConfig);
            }).catch(function () {
                stepStatusMeta = normalizeStepStatusMetaResponse(null);
                stepRegistry = buildStepRegistry(stepStatusMeta);   
                rebuildStepBindingByField();
                TT_BOM_STATUS_KEYS = [0, 1, 2, 3];
                applyTtBomStatusColors(ttBomStatusColorConfig);
            });
        }

        function normalizeTtBomStatusColorConfig(config) {
            var normalized = {};
            var source = (config && typeof config === "object") ? config : {};

            TT_BOM_STATUS_KEYS.forEach(function (status) {
                var fromNumeric = source[status];
                var fromString = source[String(status)];
                var candidate = fromNumeric != null ? fromNumeric : fromString;
                var normalizedColor = normalizeHexColor(candidate);
                normalized[status] = normalizedColor || "";
            });

            return normalized;
        }

        function applyTtBomStatusColors(config) {
            var rootStyle = document.documentElement.style;
            TT_BOM_STATUS_KEYS.forEach(function (status) {
                if (status === 0) return;
                var cssVarName = "--tt-bom-status-" + status + "-bg";
                var color = (config && config[status]) || DEFAULT_TT_BOM_STATUS_COLOR_CONFIG[status] || "";
                rootStyle.setProperty(cssVarName, color);
            });
        }
        function resolveMissingTTWarning(rowData, ttField) {
            if (!rowData || !ttField) return false;

            var meta = getStepMetaByTtField(ttField);
            var canonicalTtField = meta && meta.TTField ? String(meta.TTField).trim() : String(ttField || "").trim();
            var rawTtField = String(ttField || "").trim();
            var warningMap = rowData.StepMissingTTWarningByTtField || rowData.stepMissingTTWarningByTtField || null;

            function readMapValue(mapObj) {
                if (!mapObj) return undefined;
                if (canonicalTtField && mapObj[canonicalTtField] !== undefined) return mapObj[canonicalTtField];
                if (rawTtField && rawTtField !== canonicalTtField && mapObj[rawTtField] !== undefined) return mapObj[rawTtField];
                return undefined;
            }

            return normalizeTruthyFlag(readMapValue(warningMap));
        }

        function resolveStepStatus(rowData, ttField) {
            if (!rowData || !ttField) return 0;

            var meta = getStepMetaByTtField(ttField);
            var canonicalTtField = meta && meta.TTField ? String(meta.TTField).trim() : String(ttField || "").trim();
            var rawTtField = String(ttField || "").trim();
            if (!canonicalTtField) return 0;

            var statusMap = rowData.StepStatusByTtField || rowData.stepStatusByTtField || null;
            //var lateLockedMap = rowData.StepLateLockedByTtField || rowData.stepLateLockedByTtField || null;
            var hasStepStatusPayload =
                rowData.StepStatusByTtField != null ||
                rowData.stepStatusByTtField != null ||
                rowData.StepLateLockedByTtField != null ||
                rowData.stepLateLockedByTtField != null;

            function readMapValue(mapObj) {
                if (!mapObj) return undefined;
                if (canonicalTtField && mapObj[canonicalTtField] !== undefined) return mapObj[canonicalTtField];
                if (rawTtField && rawTtField !== canonicalTtField && mapObj[rawTtField] !== undefined) return mapObj[rawTtField];
                return undefined;
            }

            //if (normalizeTruthyFlag(readMapValue(lateLockedMap))) {
            //    return 3;
            //}

            var mappedStatus = Number(readMapValue(statusMap));
            if (Number.isFinite(mappedStatus) && mappedStatus >= 0 && mappedStatus <= 3) {
                return mappedStatus;
            }

            if (hasStepStatusPayload) {
                return 0;
            }

            if (canonicalTtField.toUpperCase() === "TT_BOM" || rawTtField.toUpperCase() === "TT_BOM") {
                if (normalizeTruthyFlag(rowData.TT_BOM_IsLateLocked)) return 3;
                var bomStatus = Number(rowData.TT_BOM_Status);
                if (Number.isFinite(bomStatus) && bomStatus >= 0 && bomStatus <= 3) return bomStatus;
            }

            return 0;
        }

        function applyStepStatusCellStyle($cell, rowData, ttField) {
            if (!$cell || !$cell.length || !ttField) return false;

            $cell.removeClass("tt-bom-status-1 tt-bom-status-2 tt-bom-status-3").removeAttr("title");

            var status = resolveStepStatus(rowData, ttField);
            if (status === 1 || status === 2 || status === 3) {
                $cell.addClass("tt-bom-status-" + status);
                $cell.attr("title", ttField + " Status = " + status);
                return true;
            }

            var isWarning = resolveMissingTTWarning(rowData, ttField);
            if (status === 0 && isWarning) {
                $cell.addClass("tt-bom-status-3"); // dùng đúng màu status 3
                $cell.attr("title", ttField + " Warning: quá KH, chưa có TT");
                return true;
            }

            return false;
        }

        function hasInlineStepStatus(row) {
            if (!row || typeof row !== "object") return false;
            var statusMap = row.StepStatusByTtField || row.stepStatusByTtField || null;
            if (statusMap && typeof statusMap === "object" && Object.keys(statusMap).length > 0) return true;

            var warningMap = row.StepMissingTTWarningByTtField || row.stepMissingTTWarningByTtField || null;
            if (warningMap && typeof warningMap === "object" && Object.keys(warningMap).length > 0) return true;

            var lateMap = row.StepLateLockedByTtField || row.stepLateLockedByTtField || null;
            return !!(lateMap && typeof lateMap === "object" && Object.keys(lateMap).length > 0);
        }

        function ensureStepStatusForRows(rows) {
            if (!Array.isArray(rows) || !rows.length) return $.Deferred().resolve(rows || []).promise();
            // Status đi kèm row từ BE (AttachStepProgressStatus), FE không tự gọi endpoint status.
            rows.forEach(function (r) {
                if (r && typeof r === "object" && hasInlineStepStatus(r)) {
                    r._stepStatusResolved = true;
                }
            });
            return $.Deferred().resolve(rows).promise();
        }

        ttBomStatusColorConfig = $.extend({}, DEFAULT_TT_BOM_STATUS_COLOR_CONFIG);
        if (headerColorConfigSeed["tt-bom-status-1-bg"]) ttBomStatusColorConfig[1] = normalizeHexColor(headerColorConfigSeed["tt-bom-status-1-bg"]) || ttBomStatusColorConfig[1];
        if (headerColorConfigSeed["tt-bom-status-2-bg"]) ttBomStatusColorConfig[2] = normalizeHexColor(headerColorConfigSeed["tt-bom-status-2-bg"]) || ttBomStatusColorConfig[2];
        if (headerColorConfigSeed["tt-bom-status-3-bg"]) ttBomStatusColorConfig[3] = normalizeHexColor(headerColorConfigSeed["tt-bom-status-3-bg"]) || ttBomStatusColorConfig[3];
        (function applyWindowTtBomConfig() {
            var hasSeedStatusColor =
                !!normalizeHexColor(headerColorConfigSeed["tt-bom-status-1-bg"]) ||
                !!normalizeHexColor(headerColorConfigSeed["tt-bom-status-2-bg"]) ||
                !!normalizeHexColor(headerColorConfigSeed["tt-bom-status-3-bg"]);

            if (hasSeedStatusColor) return;

            var cfg = normalizeTtBomStatusColorConfig(window.WIP_TT_BOM_STATUS_COLOR_CONFIG);
            TT_BOM_STATUS_KEYS.forEach(function (status) {
                if (status === 0) return;
                if (cfg[status]) ttBomStatusColorConfig[status] = cfg[status];
            });
        })();
        window.WIP_TT_BOM_STATUS_COLOR_CONFIG = ttBomStatusColorConfig;
        applyTtBomStatusColors(ttBomStatusColorConfig);

        // TODO DB-CONFIG: danh sách nhóm màu header nên dùng chung với metadata cột/header group từ DB.
        var HEADER_GROUP_COLOR_OPTIONS = [
            { key: "hdr-donhang", name: "Nhóm Đơn hàng" },
            { key: "hdr-intheu", name: "Nhóm IN-ÉP-THÊU-LASER" },
            { key: "hdr-dongbo-intheu", name: "Nhóm Đồng bộ in/thêu" },
            { key: "hdr-nangsuat", name: "Nhóm Năng suất" },
            { key: "hdr-thoigian", name: "Nhóm Thời gian" },
            { key: "hdr-tiendo-thucte", name: "Nhóm Tiến độ thực tế" },
            { key: "hdr-kh-ngay", name: "Nhóm Ngày kế hoạch" },
            { key: "hdr-merchandiser", name: "Nhóm Merchandiser" },
            { key: "hdr-tinhtoan-nangsuat", name: "Nhóm Tính toán năng suất" },
            { key: "hdr-kythuat", name: "Nhóm Phòng kỹ thuật" },
            { key: "hdr-ie", name: "Nhóm IE" },
            { key: "hdr-codien", name: "Nhóm Cơ điện" }
        ];

        function getHeaderColorMapSnapshot() {
            return headerColorPopupController.getMapSnapshot();
        }

        function getColorByCssClass(cssClass, fallback) {
            return headerColorPopupController.getColorByCssClass(cssClass, fallback);
        }

        function setEditorColor(editor, color) {
            headerColorPopupController.setEditorColor(editor, color);
        }

        function upsertHeaderColorItem(cssClass, colorCode) {
            headerColorPopupController.upsertItem(cssClass, colorCode);
        }

        function syncHeaderGroupEditors(groupCssClass) {
            headerColorPopupController.syncGroupEditors(groupCssClass);
        }

        function syncManualEditors() {
            headerColorPopupController.syncManualEditors();
        }

        function syncTtBomEditors() {
            headerColorPopupController.syncTtBomEditors();
        }

        function refreshColorPopupEditors() {
            headerColorPopupController.refreshEditors();
        }

        function buildHeaderColorMap(items) {
            return headerColorPopupController.buildColorMap(items);
        }

        function loadHeaderColors() {
            return headerColorPopupController.load();
        }

        function saveHeaderColor() {
            headerColorPopupController.save();
        }

        function initHeaderColorPopup() {
            return headerColorPopupController.initPopup();
        }

        function openHeaderColorPopup() {
            headerColorPopupController.open();
        }

        function poolColumns() {
            return WIP.Grid.WipGridColumns.poolColumns();
        }
        // Load danh sách Mer
        const mdStore = new DevExpress.data.CustomStore({
            key: "UserID",
            loadMode: "raw",
            load: function () {
                return wipLoadService.loadMdList();
            },
        });
        // Grid context menu và mobile action sheet.
        function buildGridRowMenuItems(rowData, field) {
            var items = [];
            if (!rowData) return items;
            var isReadOnlyTab = isGiaCongReadOnlyMode();
            var rowLineX = rowData.LineX;
            var hasEditablePermissions = hasAnyEditablePermissionForLine(rowLineX);

            //if (!isReadOnlyTab && field && canEditFieldByPermission(field)) {
            if (field && canEditFieldByPermission(field, rowLineX)) {

                items.push({
                    text: "Sửa ô này",
                    icon: "edit",
                    onItemClick: function () {
                        openEditCellPopup(rowData, field);
                    }
                });
            }

            //if (!isReadOnlyTab) {
            if (hasEditablePermissions) {
                items.push({
                    text: "Sửa dòng này",
                    icon: "edit",
                    onItemClick: function () {
                        //selectedDataField = resolvePermissionFocusField("MaHang");
                        openPopupForEdit(rowData);
                            //, selectedDataField);
                    }
                });
            }
            //}

            if (!isReadOnlyTab) {
                items.push({
                    text: "Đổi chuyền",
                    icon: "repeat",
                    onItemClick: function () {
                        var lines = (lineItems || [])
                            .filter(x => x && x.value != null && !x.isGiaCongTab)
                            .map(x => ({ id: x.value, name: x.text }));
                        const displayDH = getGopDHFromMaLenhSX(rowData.MaLenhSanXuat, rowData.MaDH);
                        window.WIP.ChangeLinePopup.open({
                            row: rowData,
                            lines: lines,
                            ThuTuChuyen: rowData.ThuTuChuyen,
                            displayMaDH: displayDH
                        });
                    }
                });
            }

            if (!isReadOnlyTab) {
                items.push({
                    text: "Chia chuyền",
                    icon: "copy",
                    onItemClick: function () {
                        const lines = (lineItems || [])
                            .filter(x => x && x.value != null && !x.isGiaCongTab)
                            .map(x => ({ id: x.value, name: x.text }));
                        const filter = {
                            poids: window.Popup.safeInstance("#sl_PO", "dxTagBox")?.option("value") || [],
                            dauSizeIds: window.Popup.safeInstance("#sl_InSeam", "dxTagBox")?.option("value") || [],
                            mauIds: window.Popup.safeInstance("#sl_Mau", "dxTagBox")?.option("value") || []
                        };

                        WIP.SplitLinePopup.open({
                            apiBase: "/api/",
                            maDH: rowData.MaDH,
                            lenhSX: rowData.LenhSX,
                            maLenhSanXuat: rowData.MaLenhSanXuat,
                            maKH: rowData.MaKH,
                            styleId: rowData.StyleId || rowData.MaHang,
                            tenHang: rowData.MaHang,
                            lineGoc: rowData.LineX,
                            lines: lines,
                            mer: rowData.Mer,
                            filter
                        });
                    }
                });
            }

            //if (!isReadOnlyTab && field && isManualSupportedField(field) && isManualCell(rowData, field)) {
            if (field && isManualSupportedField(field) && isManualCell(rowData, field)) {
                items.push({
                    text: "Tính tự động",
                    icon: "refresh",
                    onItemClick: function () {
                        setAutoForCell(rowData, field);
                    }
                });
            }
            //}

            items.push({
                text: "Xem lịch sử",
                icon: "clock",
                onItemClick: function () {
                    var auditRow = $.extend({}, rowData, {
                        id: rowData && rowData.id != null
                            ? rowData.id
                            : (rowData && rowData.WIPId != null ? rowData.WIPId : null),
                        WIPId: rowData && rowData.WIPId != null
                            ? rowData.WIPId
                            : (rowData && rowData.id != null ? rowData.id : null)
                    });
                    openAuditPopup(auditRow);
                }
            });

            return items;
        }
        function getGridColumnOption(gridInstance, field) {
            if (!gridInstance || !field) return null;
            try {
                return gridInstance.columnOption(field) || null;
            } catch (e) {
                return null;
            }
        }
        function getGridColumnCaption(gridInstance, field) {
            var column = getGridColumnOption(gridInstance, field);
            return (column && column.caption) || getDateFilterCaption(field, "");
        }
        function buildDateColumnMenuItems(field, column) {
            var items = [];
            if (!isFilterableDateColumn(field, column)) return items;

            items.push({
                text: "Lọc ngày theo cột này",
                icon: "filter",
                onItemClick: function () {
                    openDateFilterForColumn(field, column && column.caption);
                }
            });

            return items;
        }
        function ensureMobileActionSheet() {
            if (mobileActionSheet) return mobileActionSheet;

            var $sheet = $("#wipMobileActionSheet");
            if (!$sheet.length) {
                $sheet = $("<div id='wipMobileActionSheet'></div>").appendTo("body");
            }

            mobileActionSheet = $sheet.dxActionSheet({
                usePopover: false,
                showTitle: true,
                title: "Thao tác đơn hàng",
                showCancelButton: false,
                // cancelText: "Đóng",
                onItemClick: function (e) {
                    var action = e && e.itemData ? e.itemData._action : null;
                    if (typeof action === "function") action();
                }
            }).dxActionSheet("instance");

            return mobileActionSheet;
        }
        function openMobileRowMenu(rowData, field) {
            if (!isMobileUiMode() || !rowData) return;
            if (FIXED_FIELDS.includes(field)) return;
            var now = Date.now();
            if (now - _mobileActionSheetLastOpenAt < 250) return;

            var menuItems = buildGridRowMenuItems(rowData, field);
            if (!menuItems.length) return;

            var actionSheetItems = menuItems.map(function (x) {
                return {
                    text: x.text,
                    icon: x.icon,
                    _action: x.onItemClick
                };
            });

            var sheet = ensureMobileActionSheet();
            sheet.option({
                title: "Thao tác đơn hàng",
                items: actionSheetItems
            });
            enableActionSheetOutsideClose(sheet);
            sheet.show();
            _mobileActionSheetLastOpenAt = now;
        }
        function openMobileHeaderMenu(gridInstance, field) {
            if (!isMobileUiMode() || !gridInstance || !field) return;
            var now = Date.now();
            if (now - _mobileActionSheetLastOpenAt < 250) return;

            var column = getGridColumnOption(gridInstance, field);
            if (!column) column = { dataField: field, caption: getGridColumnCaption(gridInstance, field) };

            var menuItems = buildDateColumnMenuItems(field, column);
            if (!menuItems.length) return;

            var actionSheetItems = menuItems.map(function (x) {
                return {
                    text: x.text,
                    icon: x.icon,
                    _action: x.onItemClick
                };
            });

            var sheet = ensureMobileActionSheet();
            sheet.option({
                title: "Thao tác cột",
                items: actionSheetItems
            });
            enableActionSheetOutsideClose(sheet);
            sheet.show();
            _mobileActionSheetLastOpenAt = now;
        }
        function bindMobileLongPressForGrid(gridInstance, rootSelector) {
            if (!isMobileDeviceClient || !gridInstance) return;

            var $root = $(rootSelector);
            if (!$root.length) return;

            $root.off(".wipMobileHold");

            $root.on("touchstart.wipMobileHold", ".dx-datagrid-rowsview .dx-data-row td", function (ev) {
                var oe = ev.originalEvent;
                var touch = oe && oe.touches && oe.touches[0] ? oe.touches[0] : null;
                if (!touch) return;

                _mobileHoldTriggered = false;
                _mobileTouchStartTarget = this;
                _mobileTouchStartPoint = { x: touch.clientX, y: touch.clientY };
                clearMobileHoldTimer();

                _mobileHoldTimer = setTimeout(function () {
                    var $cell = $(_mobileTouchStartTarget);
                    var $row = $cell.closest("tr.dx-data-row");
                    var wipId = Number($row.attr("data-wip-id") || "");
                    var field = $cell.attr("data-field") || selectedDataField || "MaHang";
                    var rowData = null;

                    if (Number.isFinite(wipId) && wipId > 0) {
                        rowData = (wipData || []).find(function (x) {
                            return Number((x && (x.WIPId || x.id)) || 0) === wipId;
                        }) || null;
                    }

                    if (!rowData) {
                        var rowIndex = Number($row.attr("aria-rowindex") || "");
                        if (Number.isFinite(rowIndex) && rowIndex > 0) {
                            var visibleRows = gridInstance.getVisibleRows() || [];
                            var candidate = visibleRows[rowIndex - 1];
                            rowData = candidate && candidate.data ? candidate.data : null;
                        }
                    }

                    selectedDataField = field || selectedDataField;
                    if (FIXED_FIELDS.includes(field)) return;
                    _mobileHoldTriggered = true;
                    openMobileRowMenu(rowData, field);
                }, MOBILE_LONG_PRESS_MS);
            });

            $root.on("touchmove.wipMobileHold", ".dx-datagrid-rowsview .dx-data-row td", function (ev) {
                var oe = ev.originalEvent;
                var touch = oe && oe.touches && oe.touches[0] ? oe.touches[0] : null;
                if (!touch || !_mobileTouchStartPoint) return;

                var movedX = Math.abs(touch.clientX - _mobileTouchStartPoint.x);
                var movedY = Math.abs(touch.clientY - _mobileTouchStartPoint.y);
                if (movedX > 10 || movedY > 10) {
                    clearMobileHoldTimer();
                }
            });

            $root.on("touchend.wipMobileHold touchcancel.wipMobileHold", ".dx-datagrid-rowsview .dx-data-row td", function (ev) {
                clearMobileHoldTimer();
                _mobileTouchStartTarget = null;
                _mobileTouchStartPoint = null;
                if (_mobileHoldTriggered) {
                    ev.preventDefault();
                    setTimeout(function () { _mobileHoldTriggered = false; }, 0);
                }
            });

            $root.on("contextmenu.wipMobileHold", ".dx-datagrid-rowsview .dx-data-row td", function (ev) {
                ev.preventDefault();
            });

            $root.on("click.wipMobileHold", ".dx-datagrid-headers .dx-header-row td", function (ev) {
                if (!isMobileUiMode()) return;

                var field = $(this).attr("data-field") || "";
                var column = getGridColumnOption(gridInstance, field);
                if (!isFilterableDateColumn(field, column)) return;

                ev.preventDefault();
                ev.stopPropagation();
                openMobileHeaderMenu(gridInstance, field);
            });

            $root.on("contextmenu.wipMobileHold", ".dx-datagrid-headers .dx-header-row td", function (ev) {
                if (!isMobileUiMode()) return;

                var field = $(this).attr("data-field") || "";
                var column = getGridColumnOption(gridInstance, field);
                if (!isFilterableDateColumn(field, column)) return;

                ev.preventDefault();
                openMobileHeaderMenu(gridInstance, field);
            });
        }
        function applyMobileContextHint() {
            if (isMobileUiMode()) {
                $("body").addClass("wip-mobile-device");
            } else {
                $("body").removeClass("wip-mobile-device");
            }
        }

        // Grid widget initialization và event wiring.
        function initGridWidgets() {
            if (grid && gridUnassigned) return;

            // Build grids only after permissions are available.
            grid = wipGridController.create("#gridContainer", wipData, [], {
                dataSource: wipData,
                keyExpr: "id",
                showBorders: true,
                allowColumnResizing: true,
                columnAutoWidth: false,
                wordWrapEnabled: true,
                rowAlternationEnabled: true,
                hoverStateEnabled: true,
                paging: { enabled: false },
                scrolling: { mode: "standard" },
                columnResizingMode: "widget",
                focusedRowIndex: -1,
                focusedRowEnabled: false,
                focusStateEnabled: false,
                sorting: {
                    mode: "none"
                },
                editing: {
                    mode: "cell",
                    allowUpdating: true,
                    selectTextOnEditStart: true,
                    startEditAction: "dblClick"
                },
                filterRow: {
                    visible: true,
                    applyFilter: "auto"
                },
            
                onCellPrepared: WIP.Grid.WipGridEvents.createCellPreparedHandler({
                    isStepStatusComparableField: isStepStatusComparableField,
                    applyStepStatusCellStyle: applyStepStatusCellStyle,
                    isManualCell: isManualCell
                }),
                onRowPrepared: WIP.Grid.WipGridEvents.createRowPreparedHandler(),

                rowDragging: WIP.Grid.WipGridEvents.createMainRowDraggingOptions({
                    group: CROSS_GROUP,
                    isSortMode: function () { return sortMode; },
                    getSelectedLineX: function () { return selectedLineX; },
                    onReorder: function (e) {
                        if (!sortMode || !selectedLineX) { e.cancel = true; return; }

                        var rows = e.component.getVisibleRows();
                        var dragged = e.itemData;
                        var targetRow = rows[e.toIndex] && rows[e.toIndex].data ? rows[e.toIndex].data : null;
                        if (!dragged || !targetRow) return;
                        if (dragged.ThuTuChuyen === targetRow.ThuTuChuyen) return;
                   
                        var payload = {
                            lineX: selectedLineX,
                            fromThuTu: dragged.ThuTuChuyen,
                            toThuTu: targetRow.ThuTuChuyen,
                            isKetThuc: showEnded ? 1 : 0
                        };

                        btnSort && btnSort.option({ disabled: true, text: "Đang lưu...", type: "default" });

                        showBusy("Đang lưu sắp xếp...");

                        wipSaveService.reorder(payload, {
                            success: function (res) {

                                btnSort && btnSort.option({
                                    disabled: false,
                                    text: "Đang sắp xếp",
                                    type: "success"
                                });

                                // 2. Đảm bảo biến sortMode vẫn là true (để chặn các hành động khác nếu cần)
                                sortMode = true;

                                // 3. Đảm bảo Grid vẫn cho phép Reorder
                                grid.option("rowDragging.allowReordering", true);
                                var items = Array.isArray(res) ? res : (res && res.items ? res.items : []);
                                items = hydrateWipRows(items || []);
                                wipData = items;
                                GridViewAdapter.updateGrid(grid, wipData, "Đã sắp xếp & tính lại.", "success", 1200);
                            },
                            error: function (xhr) {
                                btnSort && btnSort.option({ disabled: false, text: "⚠ Lỗi sắp xếp", type: "danger" });
                                DevExpress.ui.notify("Reorder lỗi: " + (xhr.responseText || xhr.statusText), "error", 3000);
                            },
                            complete: function () {
                                hideBusy();
                            }
                        });
                    }
                }),

                onEditorPreparing: WIP.Grid.WipGridEvents.createEditorPreparingHandler({
                    canEditFieldByPermission: canEditFieldByPermission,
                    isFixedField: function (field) { return FIXED_FIELDS.includes(field); },
                    getFieldMaxLength: getFieldMaxLength,
                    buildStrictDateEditorOptions: buildStrictDateEditorOptions,
                    isEmptyDateValue: isEmptyDateValue,
                    getTodayDateOnly: getTodayDateOnly,
                    isNonNegField: isNonNegField
                }),
                onCellClick: WIP.Grid.WipGridEvents.createCellClickHandler({
                    setSelectedDataField: function (field) {
                        selectedDataField = field;
                    }
                }),
                onRowClick: WIP.Grid.WipGridEvents.createMobileRowClickHandler({
                    isMobileUiMode: isMobileUiMode,
                    getMobileHoldTriggered: function () { return _mobileHoldTriggered; },
                    setMobileHoldTriggered: function (value) { _mobileHoldTriggered = !!value; },
                    getSelectedDataField: function () { return selectedDataField; },
                    isFixedField: function (field) { return FIXED_FIELDS.includes(field); },
                    openMobileRowMenu: openMobileRowMenu
                }),
                onContextMenuPreparing: WIP.Grid.WipGridEvents.createContextMenuPreparingHandler({
                    isMobileUiMode: isMobileUiMode,
                    buildDateColumnMenuItems: buildDateColumnMenuItems,
                    buildGridRowMenuItems: buildGridRowMenuItems
                }),

                columns: WIP.Grid.WipGridColumns.buildMainColumns(WIP.Grid.WipGridColumns.buildRawMainColumns({
                    $: $,
                    mdStore: mdStore,
                    getFieldCaption: getFieldCaption,
                    getDataCellCssClass: getDataCellCssClass,
                    getSelectedIsGiaCong: function () { return selectedIsGiaCong; }
                }), {
                    $: $,
                    permissionLineX: selectedLineX,
                    prepareGridColumns: prepareGridColumns,
                    applyDynamicGroupEnd: applyDynamicGroupEnd,
                    numberGridLeafCaptions: numberGridLeafCaptions
                }),
                onSaving: WIP.Grid.WipGridEvents.createSavingHandler({
                    getIsGridSaving: function () { return isGridSaving; },
                    setIsGridSaving: function (value) { isGridSaving = !!value; },
                    getRows: function () { return wipData; },
                    buildSaveRequest: function (newRow, oldRow, changedFieldsObj) {
                        return WipDataService.buildSaveRequest(newRow, oldRow, changedFieldsObj);
                    },
                    showBusy: showBusy,
                    hideBusy: hideBusy,
                    saveByRequest: function (saveRequest, handlers) {
                        return wipSaveService.saveByRequest(saveRequest, handlers);
                    },
                    normalizeResponse: normalizeResponse,
                    ensureStepStatusForRows: ensureStepStatusForRows,
                    upsertRows: upsertRows,
                    updateGrid: function (gridInstance, rows, notifyMsg, notifyType, notifyTime) {
                        GridViewAdapter.updateGrid(gridInstance, rows, notifyMsg, notifyType, notifyTime);
                    }
                }),
        });
        applyMobileContextHint();
        bindMobileLongPressForGrid(grid, "#gridContainer");
        disableFixedColumnsForMobile(grid);
        syncGridInteractionModes();
        
        gridUnassigned = unassignedGridController.create("#gridContainerUnassigned", unassignedData, {
            onSelectionChanged: WIP.Grid.WipGridEvents.createUnassignedSelectionChangedHandler({
                getCrossButtonInstance: function () { return btnCrossMode || getButtonInstance("#btnCrossMode"); },
                getBaseline: function () { return _poolSelBaseline; },
                setBaseline: function (value) { _poolSelBaseline = value; },
                setLastGoodKeys: function (keys) { _poolSelLastGoodKeys = keys || []; },
                pickBaseline: pickBaseline,
                matchBaseline: matchBaseline,
                isRowCompatibleWithBaseline: isRowCompatibleWithBaseline
            }),
        });
        if (!baseGridColumns) {
            baseGridColumns = $.extend(true, [], grid.option("columns"));
        rebuildGridColumnsByPermission(selectedLineX);
        }
        disableFixedColumnsForMobile(gridUnassigned);
        syncGridInteractionModes();
        }

        // REGION 07 - Grid init/events, toolbar buttons và export Excel.
        // Export hiện đã nằm trong ExportExcelService; các hàm dưới đây chỉ là wrapper giữ contract legacy.
        function saveExcelBuffer(buffer, fileName) {
            wipExportExcelService.saveExcelBuffer(buffer, fileName);
        }

        function buildWipExportFileName() {
            return wipExportExcelService.buildWipExportFileName();
        }

        function colorToExcelArgb(value) {
            return wipExportExcelService.colorToExcelArgb(value);
        }

        function hasClassName(classListText, className) {
            return splitClassNames(classListText).indexOf(className) >= 0;
        }

        function getHeaderColorCssVarMap() {
            return wipExportExcelService.getHeaderColorCssVarMap();
        }

        function getHeaderTextColorCssVarMap() {
            return wipExportExcelService.getHeaderTextColorCssVarMap();
        }

        function resolveHeaderColorClassForExport(gridCell) {
            return wipExportExcelService.resolveHeaderColorClassForExport(gridCell);
        }

        function resolveHeaderArgbForExport(headerClassKey) {
            return wipExportExcelService.resolveHeaderArgbForExport(headerClassKey);
        }

        function resolveHeaderTextArgbForExport(headerClassKey) {
            return wipExportExcelService.resolveHeaderTextArgbForExport(headerClassKey);
        }

        function resolveManualCellArgbForExport() {
            return wipExportExcelService.resolveManualCellArgbForExport();
        }

        async function exportWipVisibleGridToExcel() {
            return await wipExportExcelService.exportWipVisibleGridToExcel();
        }

        function setSortModeUI(isOn) {
            sortMode = !!isOn;

            // ✅ đang sort thì disable nút gán
            btnCrossMode && btnCrossMode.option({ disabled: !!sortMode });

            grid.beginUpdate();
            grid.option("rowDragging.allowReordering", !!sortMode);
            grid.option("rowDragging.showDragIcons", !!sortMode);
            //grid.option("editing.allowUpdating", !sortMode && !isGiaCongReadOnlyMode());
            grid.endUpdate();

            if (btnSort) {
                btnSort.option({
                    icon: "",
                    text: sortMode ? "Đang sắp xếp" : "Sắp xếp",
                    type: sortMode ? "success" : "normal"
                });
            }
        }


        // init buttons
        toolbarController.initButton("#btnSortMode", {
            text: "Sắp xếp",
            icon: "",
            type: "normal",
            stylingMode: "contained",
            onClick: function () {
                setSortModeUI(!sortMode);
                if (sortMode) {
                    if (!selectedLineX) DevExpress.ui.notify("Sort mode bật. Hãy chọn 1 chuyền cụ thể để kéo thả.", "warning", 2000);
                    else DevExpress.ui.notify("Sort mode bật: Kéo thả dòng để thay đổi thứ tự.", "info", 1500);
                } else {
                    DevExpress.ui.notify("Đã tắt Sort mode.", "success", 800);
                }
            }
        });
        btnSort = $("#btnSortMode").dxButton("instance");
        toolbarController.initButton("#btnCrossMode", {
            text: "Gán vào chuyền",
            icon: "link",
            type: "success",
            visible: false,
            stylingMode: "contained",
            onClick: function () {
                setUnassignedUI(true);

                openAssignPopupFromSelection();
            }
        });
        btnCrossMode = $("#btnCrossMode").dxButton("instance");
        toolbarController.initButton("#btnAddRow", {
            text: "Thêm",
            icon: "plus",
            stylingMode: "contained",
            type: "default",
            visible: false,
            onClick: function () {
                clearGridFocus();
                selectedDataField = null;
                openPopupForAdd(null);
            }
        });

        toolbarController.initButton("#btnLoad", {
            text: getLoadButtonText(),
            icon: "refresh",
            type: "default",
            stylingMode: "contained",
            onClick: async function () {
                await syncLibByLine(selectedLineX);
                await loadUnassignedData();
                await loadWipData({ includeEnded: showEnded ? 1 : 0 });
            }
        });
        toolbarController.initButton("#btnRecalcLine", {
            text: "Tính",
            type: "default",
            stylingMode: "contained",
            hint: "Tính toán lại dữ liệu chuyền hiện tại",
            onClick: function () {
                if (!selectedLineX) {
                    DevExpress.ui.notify("Hãy chọn 1 chuyền trước khi tính toán lại.", "warning", 1800);
                    return;
                }

                showBusy("Đang tính toán lại...");

                recalcLine(selectedLineX)
                    .done(function (res) {
                        var items = normalizeResponse(res);
                        ensureStepStatusForRows(items).always(function () {
                            wipData = hydrateWipRows(items || []);
                            GridViewAdapter.updateGrid(
                                grid,
                                wipData,
                                "Đã tính toán lại chuyền hiện tại",
                                "success",
                                1200
                            );
                        });
                    })
                    .fail(function (xhr) {
                        var message = "Có lỗi xảy ra";

                        if (typeof xhr === "string") {
                            message = xhr;
                        } else if (xhr && xhr.responseText) {
                            message = xhr.responseText;
                        } else if (xhr && xhr.statusText) {
                            message = xhr.statusText;
                        }

                        DevExpress.ui.notify("Tính toán lại lỗi: " + message, "error", 3000);
                    })
                    .always(function () {
                        hideBusy();
                    });
            }
        });
        toolbarController.initButton("#btnExportWipExcel", {
            text: "Xuất Excel",
            icon: "xlsxfile",
            type: "success",
            stylingMode: "contained",
            hint: "Xuất đúng dữ liệu đang hiển thị trên lưới WIP",
            onClick: function () {
                exportWipVisibleGridToExcel();
            }
        });
        toolbarController.initButton("#btnDateFilter", {
            icon: "filter",
            text: "",
            type: "normal",
            stylingMode: "text",
            hint: "Chọn cột ngày rồi lọc",
            onClick: function () {
                openDateFilterPopover();
            }
        });
        updateDateFilterButtonState();
        $("#popDateFilter").dxPopover({
            target: "#btnDateFilter",
            showTitle: true,
            title: "Lọc theo ngày",
            width: 360,
            hideOnOutsideClick: true,
            position: { my: "top", at: "bottom", of: "#btnDateFilter" },
            wrapperAttr: {
                class: "date-filter-popover"
            },
            onShowing: function () {
                updateDateFilterPopoverTitle();
                syncDateFilterPopoverContent();
            },
            contentTemplate: function (content) {
                // Logic cu: radio chon 2 mode co dinh CUT / OUT va goi loadWipData xuong server.
                //$("<div>").dxRadioGroup({
                //    items: [
                //        { text: "Ngày cắt", value: "CUT" },
                //        { text: "Thoát chuyền", value: "OUT" }
                //    ],
                //    valueExpr: "value",
                //    displayExpr: "text",
                //    value: DateFilter.mode,
                //    layout: "horizontal",
                //    onValueChanged: function (e) {
                //        DateFilter.mode = e.value;
                //    }
                //}).appendTo(content);
                //
                //let fromVal = DateFilter.range?.[0] || null;
                //let toVal = DateFilter.range?.[1] || null;

                $("<div>", { id: "dfSelectedFieldLabel", class: "date-filter-selected-field" }).appendTo(content);
                //$("<div>", { id: "dfSelectedFieldCode", class: "date-filter-selected-code", style: "font-size:12px;color:#6b7280;margin-top:4px;" }).appendTo(content);

                const $row = $("<div class='date-range-row'></div>").appendTo(content);
                const $from = $("<div id='dfFrom'></div>").appendTo($row);
                $("<div class='date-sep'>|</div>").appendTo($row);
                const $to = $("<div id='dfTo'></div>").appendTo($row);

                $from.dxDateBox({
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    placeholder: "Từ ngày",
                    value: DateFilter.range[0] || null,
                    onValueChanged: (e) => {
                        DateFilter.range = [e.value || null, DateFilter.range[1] || null];
                    }
                });

                $to.dxDateBox({
                    type: "date",
                    displayFormat: "dd/MM/yyyy",
                    placeholder: "Đến ngày",
                    value: DateFilter.range[1] || null,
                    onValueChanged: (e) => {
                        DateFilter.range = [DateFilter.range[0] || null, e.value || null];
                    }
                });

                // Buttons
                const row = $("<div style='display:flex; gap:8px; justify-content:flex-end; margin-top:12px;'></div>");
                row.appendTo(content);

                $("<div>").dxButton({
                    text: "Xóa",
                    type: "normal",
                    stylingMode: "outlined",
                    onClick: function () {
                        // Logic cu:
                        //DateFilter.range = [null, null];
                        //const fromInst = $("#dfFrom").dxDateBox("instance");
                        //const toInst = $("#dfTo").dxDateBox("instance");
                        //fromInst && fromInst.option("value", null);
                        //toInst && toInst.option("value", null);
                        //$("#btnDateFilter").removeClass("is-filter-active");
                        //loadWipData({ lineX: selectedLineX, mode: DateFilter.mode, range: DateFilter.range, includeEnded: showEnded ? 1 : 0 });
                        //$("#popDateFilter").dxPopover("instance").hide();

                        DateFilter.range = [null, null];
                        const fromInst = $("#dfFrom").dxDateBox("instance");
                        const toInst = $("#dfTo").dxDateBox("instance");
                        fromInst && fromInst.option("value", null);
                        toInst && toInst.option("value", null);
                        updateDateFilterButtonState();
                        refreshMainGridWithDateFilter("Đã xóa lọc ngày cục bộ.", "success", 900);
                        $("#popDateFilter").dxPopover("instance").hide();
                    }
                }).appendTo(row);

                $("<div>").dxButton({
                    text: "Lọc",
                    type: "default",
                    stylingMode: "contained",
                    onClick: function () {
                        // Logic cu:
                        //$("#btnDateFilter").toggleClass("is-filter-active", !!(DateFilter.range?.[0] || DateFilter.range?.[1]));
                        //loadWipData({ lineX: selectedLineX, mode: DateFilter.mode, range: DateFilter.range, includeEnded: showEnded ? 1 : 0 });
                        //$("#popDateFilter").dxPopover("instance").hide();

                        if (!DateFilter.field) {
                            DevExpress.ui.notify("Chưa có cột ngày để lọc.", "warning", 1800);
                            return;
                        }

                        var fromInst = $("#dfFrom").dxDateBox("instance");
                        var toInst = $("#dfTo").dxDateBox("instance");
                        var fromValue = fromInst ? (fromInst.option("value") || null) : null;
                        var toValue = toInst ? (toInst.option("value") || null) : null;

                        var fromDate = normalizeDateOnly(fromValue);
                        var toDate = normalizeDateOnly(toValue);
                        if (fromDate && toDate && fromDate > toDate) {
                            DevExpress.ui.notify("Từ ngày không được lớn hơn đến ngày.", "warning", 2200);
                            return;
                        }

                        DateFilter.range = [fromValue, toValue];
                        updateDateFilterButtonState();
                        refreshMainGridWithDateFilter("Đã lọc theo " + getDateFilterCaption(DateFilter.field, DateFilter.caption) + ".", "success", 900);
                        $("#popDateFilter").dxPopover("instance").hide();
                    }
                }).appendTo(row);
            }
        });
        ensureColumnFilterButtonElement();
        toolbarController.initButton("#btnColumnFilter", {
            text: "Cột",
            icon: "columnchooser",
            type: "normal",
            stylingMode: "contained",
            hint: "Chọn cột hiển thị",
            onClick: function () {
                openColumnFilterPopup();
            }
        });
        initWipMoreActions();
        applyResponsiveHeaderAndActions();
        function clearGridFocus() {
            try {
                grid.clearSelection();
                grid.option("focusedRowIndex", -1);
            } catch (e) { }
        }

        // =========================
        // 4) Popup Editor (Add + Edit dùng chung)
        // =========================
        var editCellContext = {
            row: null,
            field: null
        };
        var editCellPopupController = new WIP.UI.EditCellPopupController({
            getContext: function () { return editCellContext; },
            getRows: function () { return wipData; },
            getPopup: function () { return popupEditCell; },
            isFieldValueChanged: isFieldValueChanged,
            buildSaveRequest: function (row, oldRow, changedObj) {
                return WipDataService.buildSaveRequest(row, oldRow, changedObj);
            },
            showBusy: showBusy,
            hideBusy: hideBusy,
            saveByRequest: function (saveRequest, handlers) {
                return wipSaveService.saveByRequest(saveRequest, handlers);
            },
            normalizeResponse: normalizeResponse,
            ensureStepStatusForRows: ensureStepStatusForRows,
            upsertRows: upsertRows,
            updateGrid: function (rows, notifyMsg, notifyType, notifyTime) {
                GridViewAdapter.updateGrid(grid, rows, notifyMsg, notifyType, notifyTime);
            }
        });
        function buildChangesFromRow(row, changedFieldsObj) {
            const changes = [];
            const keys = Object.keys(changedFieldsObj || {});
            for (let i = 0; i < keys.length; i++) {
                const field = keys[i];
                if (!field) continue;

                // chặn field tạm UI
                if (field.indexOf("__libId_") === 0) continue;

                changes.push({
                    field: field,
                    newValue: row[field] === undefined ? null : row[field]
                });
            }
            return changes;
        }
        function inferBindingFromStepRegistry(field) {
            var stepByKh = (typeof getStepMetaByKhField === "function") ? getStepMetaByKhField(field) : null;
            if (stepByKh) {
                return {
                    stepCode: stepByKh.StepCode,
                    role: "KH",
                    dataType: "date",
                    allowManualKH: !!stepByKh.AllowManualKH,
                    isCheckTT: !!stepByKh.IsCheckTT
                };
            }

            var stepByTt = (typeof getStepMetaByTtField === "function") ? getStepMetaByTtField(field) : null;
            if (stepByTt) {
                var ttDataType = "date";
                var colMeta = (typeof getColumnMetaByField === "function") ? getColumnMetaByField(field) : null;
                if (colMeta && colMeta.dataType) ttDataType = colMeta.dataType;

                return {
                    stepCode: stepByTt.StepCode,
                    role: "TT",
                    dataType: ttDataType,
                    allowManualKH: !!stepByTt.AllowManualKH,
                    isCheckTT: !!stepByTt.IsCheckTT
                };
            }

            return null;
        }

        function buildStructuredChanges(changedFieldsObj, rowData) {
            var legacyChanges = buildChangesFromRow(rowData, changedFieldsObj); // giữ y hệt flow cũ
            var parentChanges = [];
            var stepMap = Object.create(null);

            legacyChanges.forEach(function (ch) {
                var field = String(ch && ch.field || "").trim();
                if (!field) return;

                var binding = (typeof getStepBindingByField === "function")
                    ? getStepBindingByField(field)
                    : null;

                if (!binding) {
                    binding = inferBindingFromStepRegistry(field);
                }

                if (!binding || !binding.stepCode) {
                    var parentMeta = (typeof getColumnMetaByField === "function") ? getColumnMetaByField(field) : null;
                    parentChanges.push({
                        field: field,
                        newValue: ch.newValue,
                        dataType: (parentMeta && parentMeta.dataType) ? parentMeta.dataType : "text"
                    });
                    return;
                }

                var stepKey = String(binding.stepCode || "").trim().toLowerCase();
                if (!stepMap[stepKey]) {
                    stepMap[stepKey] = {
                        stepCode: binding.stepCode,
                        allowManualKH: !!binding.allowManualKH,
                        isCheckTT: !!binding.isCheckTT,
                        khChanges: [],
                        ttChanges: [],
                        stepExtraChanges: []
                    };
                }

                var item = {
                    field: field,
                    newValue: ch.newValue,
                    role: binding.role || "STEP_EXTRA",
                    dataType: binding.dataType || "text"
                };

                if (item.role === "KH") stepMap[stepKey].khChanges.push(item);
                else if (item.role === "TT") stepMap[stepKey].ttChanges.push(item);
                else stepMap[stepKey].stepExtraChanges.push(item);
            });

            return {
                parentChanges: parentChanges,
                stepChanges: Object.keys(stepMap).map(function (k) { return stepMap[k]; }),
                legacyChanges: legacyChanges
            };
        }

        var popupEditCell = $("#popupEditCell").dxPopup($.extend(true, {
            title: "Sửa ô",
            width: 460,
            height: "auto",
            showTitle: true,
            visible: false,
            dragEnabled: true,
            hideOnOutsideClick: true,

            // tạo DOM form khi popup render
            contentTemplate: function (content) {
                $("<div id='editCellForm'></div>").appendTo(content);
            },

            onShowing: function () {
                // clear để tránh reuse form cũ
                $("#editCellForm").empty();
            },

            onShown: function () {
                // ✅ lúc này #editCellForm chắc chắn tồn tại => init dxForm ok
                renderEditCellForm();

                // focus editor
                setTimeout(function () {
                    var form = $("#editCellForm").dxForm("instance");
                    var field = editCellContext.field;
                    var editor = form && field ? form.getEditor(field) : null;
                    if (editor && editor.focus) editor.focus();
                }, 0);
            },

            onHidden: function () {
                editCellContext.row = null;
                editCellContext.field = null;
                $("#editCellForm").empty();
            },

            toolbarItems: [
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: {
                        text: "Lưu",
                        type: "success",
                        onClick: saveEditCell
                    }
                },
                {
                    widget: "dxButton",
                    toolbar: "bottom",
                    location: "after",
                    options: {
                        text: "Hủy",
                        onClick: function () { popupEditCell.hide(); }
                    }
                }
            ]
        }, mobilePopupOptions())).dxPopup("instance");
        var editRowPopupController = new WIP.UI.EditRowPopupController({
            $: $,
            formSelector: "#wipForm",
            popupOptions: mobilePopupOptions(),
            titleAdd: "Nh\u1eadp WIP",
            titleEdit: "S\u1eeda WIP",
            saveText: "L\u01b0u",
            closeText: "\u0110\u00f3ng",
            orderInfoGroupCaption: "1) \u0110\u01a1n h\u00e0ng",
            messages: {
                formNotReady: "Form ch\u01b0a s\u1eb5n s\u00e0ng!",
                saving: "\u0110ang l\u01b0u...",
                saved: function (count) { return "\u0110\u00e3 l\u01b0u (" + count + " d\u00f2ng c\u1eadp nh\u1eadt)"; },
                saveErrorPrefix: "L\u01b0u l\u1ed7i: ",
                noEditPermission: "B\u1ea1n kh\u00f4ng c\u00f3 quy\u1ec1n ch\u1ec9nh s\u1eeda WIP"
            },
            notify: function (message, type, time) { DevExpress.ui.notify(message, type, time); },
            getSelectedDataField: function () { return selectedDataField; },
            setSelectedDataField: function (field) { selectedDataField = field; },
            buildFormItems: buildEditRowFormItems,
            normalizePermissionField: normalizePermissionField,
            canViewFieldByPermission: canViewFieldByPermission,
            hasAnyEditablePermissionForLine: hasAnyEditablePermissionForLine,
            findOldRow: function (row) { return wipData.find(function (x) { return x && x.WIPId === row.WIPId; }) || {}; },
            getActuallyChangedFields: getActuallyChangedFields,
            buildSaveRequest: function (row, oldRow, actualChangedFields) {
                return WipDataService.buildSaveRequest(row, oldRow, actualChangedFields);
            },
            showBusy: showBusy,
            hideBusy: hideBusy,
            saveByRequest: function (saveRequest, handlers) { return wipSaveService.saveByRequest(saveRequest, handlers); },
            normalizeResponse: normalizeResponse,
            ensureStepStatusForRows: ensureStepStatusForRows,
            upsertRows: function (items) { upsertRows(wipData, items); },
            updateGrid: function (items, notifyMsg, notifyType, notifyTime) {
                GridViewAdapter.updateGrid(grid, wipData, notifyMsg, notifyType, notifyTime);
            }
        });
        var popup = editRowPopupController.init("#popupEditor");
        // REGION 08 - Edit row/cell popup, permission, load/save/recalc.
        function renderEditCellForm() {
            var row = editCellContext.row;
            var field = editCellContext.field;
            if (!row || !field) return;

            var cap = getFieldCaption(field, field);

            $("#editCellForm").dxForm({
                formData: row,            // bind trực tiếp row
                labelLocation: "top",
                colCount: 1,
                showColonAfterLabel: false,
                items: [buildEditorForCell(field, cap)]
            });
        }
        function buildEditorForCell(field, caption) {
            const ctxCellEdit = {
                data: editCellContext.row,
                formSelector: "#editCellForm",
                getLineX: () => (editCellContext.row ? editCellContext.row.LineX : null)
            };
            return formEditorFactory.buildEditorForCell(ctxCellEdit, field, caption);
        }

        function getGopDHFromMaLenhSX(maLenhSX, fallbackMaDH) {
            const s = String(maLenhSX || "").trim();
            if (!s) return String(fallbackMaDH || "").trim();

            // lấy phần trước dấu |
            const head = s.split("|")[0] || "";
            // tách theo @, lọc rỗng, unique
            const dhs = head
                .split("@")
                .map(x => String(x || "").trim())
                .filter(Boolean);

            const uniq = [...new Set(dhs)];
            if (!uniq.length) return String(fallbackMaDH || "").trim();

            // join để hiển thị
            return uniq.join(";");
        }
        function initLineTabs() {
            tabs = lineTabsController.init("#lineTabs");
        }

        function loadLineXList(isFirstLoad) {
            return lineTabsController.load(isFirstLoad);
        }

        // Permission scope vẫn mirror state cũ để popup và grid event không đổi behavior.
        function normalizePermissionField(field) {
            return String(field || "").trim();
        }
        function normalizePermissionLineX(lineX) {
            if (lineX === null || lineX === undefined || lineX === "") return 0;

            var normalized = Number(lineX);
            return Number.isFinite(normalized) ? normalized : 0;
        }
        function getPermissionScopeKey(lineX) {
            return String(normalizePermissionLineX(lineX));
        }
        function ensurePermissionScope(lineX) {
            var scopeKey = getPermissionScopeKey(lineX);
            if (!WIP.permissionMap[scopeKey]) {
                WIP.permissionMap[scopeKey] = {};
            }
            return WIP.permissionMap[scopeKey];
        }
        function getPermissionEntry(field, lineX) {
            field = normalizePermissionField(field);
            if (!field || !WIP.hasLoadedFieldPermissions) return null;

            var exactScope = WIP.permissionMap[getPermissionScopeKey(lineX)] || {};
            if (exactScope[field]) return exactScope[field];

            var defaultScope = WIP.permissionMap[getPermissionScopeKey(0)] || {};
            if (defaultScope[field]) return defaultScope[field];

            return null;
        }
        function hasAnyEditablePermissionForLine(lineX) {
            if (!WIP.hasLoadedFieldPermissions) return false;

            var exactScope = WIP.permissionMap[getPermissionScopeKey(lineX)] || {};
            var defaultScope = WIP.permissionMap[getPermissionScopeKey(0)] || {};
            var keys = new Set(Object.keys(defaultScope).concat(Object.keys(exactScope)));

            return Array.from(keys).some(function (key) {
                var entry = getPermissionEntry(key, lineX);
                return entry && entry.isEdit === true;
            });
        }
        function resetWipPermissions() {
            WIP.permissionMap = {};
            WIP.viewableFields = new Set();
            WIP.editableFields = new Set();
            WIP.hasLoadedFieldPermissions = false;
        }
        function resetWipFeaturePermissions() {
            WIP.featurePermissions = {};
            WIP.hasLoadedFeaturePermissions = false;
        }
        function applyWipPermissions(items) {
            resetWipPermissions();

            (Array.isArray(items) ? items : []).forEach(function (item) {
                if (!item || typeof item !== "object") return;

                var colKey = normalizePermissionField(item.ColKey);
                if (!colKey) return;
                var lineX = normalizePermissionLineX(item.LineX);

                var isView = item.IsView === true || item.IsView === 1 || item.IsView === "1";
                var isEdit = item.IsEdit === true || item.IsEdit === 1 || item.IsEdit === "1";

                ensurePermissionScope(lineX)[colKey] = {
                    isView: isView,
                    isEdit: isEdit
                };

                if (isView) WIP.viewableFields.add(colKey);
                if (isEdit) WIP.editableFields.add(colKey);
            });

            WIP.hasLoadedFieldPermissions = true;
        }
        function applyWipFeaturePermissions(items) {
            resetWipFeaturePermissions();

            (Array.isArray(items) ? items : []).forEach(function (item) {
                if (!item || typeof item !== "object") return;

                var featureKey = String(item.FeatureKey || "").trim();
                if (!featureKey) return;

                var isAllow = item.IsAllow === true
                    || item.IsAllow === 1
                    || item.IsAllow === "1"
                    || item.IsAllow === "true"
                    || item.IsAllow === "True";

                // Runtime route may return only allowed keys, so missing IsAllow means true.
                if (!Object.prototype.hasOwnProperty.call(item, "IsAllow")) isAllow = true;

                WIP.featurePermissions[featureKey] = isAllow;
            });

            WIP.hasLoadedFeaturePermissions = true;
            updateHeaderColorButtonState();
        }
        async function loadWipPermissions(userName) {
            if (typeof showBusy === "function") showBusy("Đang tải dữ liệu...");

            try {
                const res = await wipPermissionService.loadFieldPermissions(userName);

                applyWipPermissions(res);
            } catch (error) {
                resetWipPermissions();
                console.error("Lỗi khi tải phân quyền WIP:", error);
                throw error;
            } finally {
                if (typeof hideBusy === "function") hideBusy();
            }
        }
        async function loadWipFeaturePermissions(userName) {
            try {
                const res = await wipPermissionService.loadFeaturePermissions(userName);

                applyWipFeaturePermissions(res);
            } catch (err) {
                resetWipFeaturePermissions();
                updateHeaderColorButtonState();
                throw err;
            }
        }
        function canViewFieldByPermission(field) {
            var lineX = arguments.length > 1 ? arguments[1] : undefined;
            field = normalizePermissionField(field);
            if (!field) return true;
            if (!WIP.hasLoadedFieldPermissions) return false;

            if (lineX !== undefined) {
                var entry = getPermissionEntry(field, lineX);
                return entry ? entry.isView === true : false;
            }

            return WIP.viewableFields.has(field);
        }
        function canUseFeature(featureKey) {
            featureKey = String(featureKey || "").trim();
            if (!featureKey) return false;
            if (!WIP.hasLoadedFeaturePermissions) return false;

            return WIP.featurePermissions[featureKey] === true;
        }
        function updateHeaderColorButtonState() {
            var btn = $("#btnHeaderColorConfig").dxButton("instance");
            if (!btn) return;

            var isAllowed = canUseFeature(FEATURE_KEYS.HEADER_COLOR_CONFIG);
            btn.option({
                visible: isAllowed,
                disabled: !isAllowed
            });
            syncWipMoreActionButton();
        }
        function canEditFieldByPermission(field) {
            var lineX = arguments.length > 1 ? arguments[1] : undefined;
            field = normalizePermissionField(field);
            if (!field) return false;
            if (FIXED_FIELDS.includes(field)) return false;
            if (!WIP.hasLoadedFieldPermissions) return false;

            if (lineX !== undefined) {
                var entry = getPermissionEntry(field, lineX);
                return entry ? entry.isEdit === true : false;
            }

            return WIP.editableFields.has(field);
        }
        function resolvePermissionFocusField(field, lineX) {
            field = normalizePermissionField(field);
            if (field && canViewFieldByPermission(field, lineX)) return field;

            var firstEditable = WIP.editableFields && WIP.editableFields.size > 0
                ? WIP.editableFields.values().next().value
                : null;
            if (firstEditable && canViewFieldByPermission(firstEditable, lineX)) return firstEditable;

            var firstViewable = WIP.viewableFields && WIP.viewableFields.size > 0
                ? WIP.viewableFields.values().next().value
                : null;
            if (firstViewable) return firstViewable;

            return "MaHang";
        }
        function normalizeLines(res) {
            return lineState.normalizeLines(res);
        }

        // =========================
        // 5) API load data -> grid
        // =========================
        function setAutoForCell(rowData, field) {
            //if (!ensureWritableMode()) return;
            if (!rowData || !field) return;

            var oldRow = wipData.find(x => x && (x.WIPId === rowData.WIPId || x.id === rowData.id)) || {};

            showBusy("Đang tính lại...");

            wipSaveService.saveByRequest(wipDataPayloadService.buildAutoSaveRequest(rowData, oldRow, field), {
                success: function (savedRow) {
                    var items = normalizeResponse(savedRow);
                    ensureStepStatusForRows(items).always(function () {
                        upsertRows(wipData, items);
                        GridViewAdapter.updateGrid(grid, wipData, "Đã chuyển về tự động tính", "success", 1200);
                    });

                },
                error: function (xhr) {
                    DevExpress.ui.notify("Lỗi: " + (xhr.responseText || xhr.statusText), "error", 3000);
                },
                complete: function () {
                    hideBusy();
                }
            });
        }
        function isEmptyDateValue(v) {
            return v === null || v === undefined || v === "";
        }
        function getTodayDateOnly() {
            var d = new Date();
            d.setHours(0, 0, 0, 0);
            return d;
        }
        function buildStrictDateEditorOptions(editorOptions) {
            var opts = $.extend(true, {}, editorOptions || {});
            opts.type = "date";
            opts.displayFormat = "dd/MM/yyyy";
            opts.dateSerializationFormat = "yyyy-MM-dd";
            opts.useMaskBehavior = true;
            opts.invalidDateMessage = opts.invalidDateMessage || "Nhập đúng định dạng dd/MM/yyyy";

            var oldKeyDown = opts.onKeyDown;
            opts.onKeyDown = function (args) {
                var ev = args && args.event;
                if (ev && !ev.ctrlKey && !ev.metaKey && !ev.altKey && typeof ev.key === "string" && ev.key.length === 1) {
                    if (!/[0-9/]/.test(ev.key)) {
                        ev.preventDefault();
                        return;
                    }
                }
                if (typeof oldKeyDown === "function") oldKeyDown(args);
            };

            var oldPaste = opts.onPaste;
            opts.onPaste = function (args) {
                var ev = args && args.event;
                var txt = "";
                if (ev && ev.clipboardData && typeof ev.clipboardData.getData === "function") {
                    txt = ev.clipboardData.getData("text") || "";
                } else if (window.clipboardData && typeof window.clipboardData.getData === "function") {
                    txt = window.clipboardData.getData("Text") || "";
                }

                if (txt && /[^0-9/]/.test(txt)) {
                    ev && ev.preventDefault && ev.preventDefault();
                    return;
                }
                if (typeof oldPaste === "function") oldPaste(args);
            };

            return opts;
        }

        // Data loading/recalc/save entrypoints còn giữ wrapper legacy cho public WIP.*.
        function loadWipData(opt) {
            opt = opt || {};
            const lx = (opt.lineX === undefined ? selectedLineX : opt.lineX);
            const gc = (opt.isGiaCong === undefined ? selectedIsGiaCong : opt.isGiaCong);
            // Logic cu: lay mode tu DateFilter.mode (CUT / OUT) de gui xuong backend.
            //const mode = opt.mode || DateFilter.mode || "CUT";
            const mode = opt.mode || "CUT";
            const modeIsKetThuc = opt.includeEnded || 0;
            // Logic cu: neu popup loc ngay su dung server-side thi lay range tu DateFilter.range.
            //const range = opt.range || DateFilter.range || [null, null];
            const range = opt.range || [null, null];
            const fromDate = opt.fromDate !== undefined ? opt.fromDate : toYMD(range[0]);
            const toDate = opt.toDate !== undefined ? opt.toDate : toYMD(range[1]);

            var params = {
                lineX: lx,
                isGiaCong: gc,
                mode: mode,
                fromDate: fromDate,
                toDate: toDate,
                isKetThuc: modeIsKetThuc
            };

            try { if (_loadXhr && _loadXhr.readyState !== 4) _loadXhr.abort(); } catch (e) { }

            var seq = ++_loadSeq;
            _loadXhr = wipLoadService.loadWipData(params)
                .done(function (res) {
                    if (seq !== _loadSeq) return;

                    wipData = hydrateWipRows(res || []);

                    ensureStepStatusForRows(wipData).always(function () {
                        if (seq !== _loadSeq) return;
                        var msg = "Đã tải xong: " + wipData.length + " dòng";
                        GridViewAdapter.updateGrid(grid, wipData, msg, "success", 800);
                    });
                })
                .fail(function (xhr) {
                    if (xhr && xhr.statusText === "abort") return;
                    if (seq !== _loadSeq) return;
                    DevExpress.ui.notify("Tải lỗi: " + (xhr.responseText || xhr.statusText), "error", 3000);
                });

            return _loadXhr;
        }
        function syncLibByLine(lineX) {
            if (!lineX) return;
            return wipLineService.syncLibByLine(lineX);
        }
        function recalcLine(lineX) {
            return wipRecalcService.recalcLineForContext({
                lineX: lineX,
                fromThuTu: 1,
                modifyBy: userName || null,
                isKetThuc: showEnded
            });
        }
        function loadUnassignedData() {
            if (_unassignedXhr && _unassignedXhr.readyState !== 4) {
                try { _unassignedXhr.abort(); } catch (e) { }
            }
            showBusy("Đang tải...");

            _unassignedXhr = wipLoadService.loadUnassignedLine(
                { fromDate: null, toDate: null, lineX: selectedLineX, isGiaCong: selectedIsGiaCong },
                { timeout: 20000 }
            )
                .done(function (res) {
                    unassignedData = (res || [])
                        .filter(function (x) { return !x || !x.LineX || x.LineX === null; })
                        .map(function (x) {
                            return hydrateWipRow(x);
                        });

                    ensureStepStatusForRows(unassignedData).always(function () {
                        GridViewAdapter.updateGrid(
                            gridUnassigned,
                            unassignedData,
                            "Đã tải: " + unassignedData.length + " dòng",
                            "success",
                            1200
                        );
                    });

                })
                .fail(function (xhr) {
                    if (xhr && xhr.statusText === "abort") return;
                    DevExpress.ui.notify("Load grid dưới lỗi: " + (xhr.responseText || xhr.statusText), "error", 2500);
                })
                .always(function () {
                    hideBusy();
                    _unassignedXhr = null;
                });
            return _unassignedXhr;
        }
        // Lưu thay đổi của ô
        function saveEditCell() {
            editCellPopupController.save();
        }

        // =========================
        // 6) Form render + helpers
        // =========================
        //function normalizeInTheu(row, changedField) {
        //    if (!row) return row;

        //    const trong = row.InTheuTrong_PKH === true;
        //    const ngoai = row.InTheuNgoai_PKH === true;

        //    // Nếu user vừa tick Ngoài -> ưu tiên Ngoài
        //    if (changedField === "InTheuNgoai_PKH") {
        //        if (ngoai) row.InTheuTrong_PKH = false;   // giữ Ngoài, tắt Trong
        //        return;
        //    }

        //    // Nếu user vừa tick Trong -> ưu tiên Trong
        //    if (changedField === "InTheuTrong_PKH") {
        //        if (trong) row.InTheuNgoai_PKH = false;   // giữ Trong, tắt Ngoài
        //        return;
        //    }
        //}
        // normalize để so sánh ổn định (null/""/number/date)
        function normCompareValue(v) {
            return WIP.Domain.Calculation.normalizeCompareValue(v, toYMD);
        }
        function isFieldValueChanged(oldRow, newRow, field) {
            return WIP.Domain.Calculation.isFieldValueChanged(oldRow, newRow, field, toYMD);
        }
        function getActuallyChangedFields(oldRow, newRow, changedFieldsObj) {
            return WIP.Domain.Calculation.getActuallyChangedFields(oldRow, newRow, changedFieldsObj, toYMD);
        }
        function hasAnyRecalcChange(oldRow, newRow) {
            return WIP.Domain.Calculation.hasAnyRecalcChange(oldRow, newRow, RECALC_FIELDS_16, toYMD);
        }
        // gắn cờ cho backend
        function attachRecalcFlag(oldRow, newRow) {
            return WIP.Domain.Calculation.attachRecalcFlag(oldRow, newRow, RECALC_FIELDS_16, toYMD);
        }
        function mapRowId(r) {
            if (!r) return r;
            return hydrateWipRow(r);
        }

        function wipIdStrFromRow(r) {
            return wipPoolService.wipIdStrFromRow(r);
        }

        // Pool row/data wrappers qua PoolService.
        function removePoolByWipIds(wipIds) {
            wipPoolService.removePoolByWipIds(wipIds);
        }
        function addRowsToPool(rows) {
            wipPoolService.addRowsToPool(rows);
        }
        function replaceLineData(lineX, rows) {
            return wipPoolService.replaceLineData(lineX, rows);
        }

        // nếu muốn lấy full row đang kéo (để add vào pool) khi UNASSIGN
        function getRowsFromGridOrRow(gridInstance, rowData) {
            var selected = [];
            try { selected = gridInstance.getSelectedRowsData() || []; } catch (e) { selected = []; }
            return selected.length ? selected : (rowData ? [rowData] : []);
        }
        function normalizeResponse(res, mode = 'single') {
            return WIP.Domain.Row.normalizeResponse(res, mode);
        }
        function upsertRows(targetArray, items) {
            var hydratedItems = (items || []).map(function (r) {
                return r ? hydrateWipRow(r) : r;
            }).filter(Boolean);

            WIP.Domain.Row.upsertRows(targetArray, hydratedItems);
        }
        function getWipIdKey(row) {
            return WIP.Domain.Row.getWipIdObject(row);
        }
        function getWipIdsFromGridOrRow(gridInstance, rowData) {
            var selected = [];
            try { selected = gridInstance.getSelectedRowsData() || []; } catch (e) { selected = []; }

            var list = selected.length ? selected : (rowData ? [rowData] : []);
            // chỉ lấy WIPId hợp lệ
            return list
                .map(r => r && r.WIPId ? Number(r.WIPId) : null)
                .filter(x => Number.isFinite(x) && x > 0);
        }
        function optFor(field, extraOpt) {
            return formEditorFactory.optFor(field, extraOpt);
        }
        function prepareFormItems(items, groupCaption) {
            return editRowPopupController.prepareFormItems(items, groupCaption);
        }
        function safeFormUpdate(ctx, fn) {
            return WIP.UI.EditRowPopupController.safeFormUpdate(ctx, fn);
        }
        function markCtxFieldChanged(ctx, field) {
            return editRowPopupController.markCtxFieldChanged(ctx, field);
        }
        function buildEditRowFormItems(ctxRowEdit, isEdit) {
            var isGiaCong = isGiaCongReadOnlyMode();
            var lineDisplayField = isGiaCong ? "TenDVSX" : "LineName";
            var nameDisplayField = isGiaCong ? "\u0110\u01a1n v\u1ecb gia c\u00f4ng" : "Chuy\u1ec1n";
            return [
                    {
                        itemType: "group",
                        caption: "1) Đơn hàng",
                        colSpan: 4,
                        colCount: 4,
                        items: [
                            text("ThuTuChuyen", getFieldCaption("ThuTuChuyen"), optFor("ThuTuChuyen", { readOnly: isEdit })),
                            //text("LineName", "Chuyền", optFor("LineX", { readOnly: isEdit })),
                            text(lineDisplayField, nameDisplayField, optFor(lineDisplayField, { readOnly: isEdit })),
                            //reqNum("LenhSX", getFieldCaption("LenhSX"), optFor("LenhSX", { readOnly: isEdit })),
                            //reqText("StyleId", getFieldCaption("StyleId"), optFor("StyleId", { readOnly: isEdit })),

                            text("MaHang", getFieldCaption("MaHang"), optFor("MaHang")),
                            text("KhachHang", getFieldCaption("KhachHang"), optFor("KhachHang")),
                            //text("PO", getFieldCaption("PO"), optFor("PO")),
                            num("SLKH", getFieldCaption("SLKH"), optFor("SLKH")),
                            merSelect("Mer", getFieldCaption("Mer"), optFor("Mer")),
                            bool("InTheu", getFieldCaption("InTheu"), optFor("InTheu")),
                            bool("HutAm", getFieldCaption("HutAm"), optFor("HutAm")),
                            bool("DoKim", getFieldCaption("DoKim"), optFor("DoKim")),
                            bool("InTheuTrong_PKH", getFieldCaption("InTheuTrong_PKH"), optFor("InTheuTrong_PKH")),
                        //    bool("InTheuNgoai_PKH", getFieldCaption("InTheuNgoai_PKH"), optFor("InTheuNgoai_PKH")),
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "2) Tính toán năng suất",
                        colSpan: 4,
                        colCount: 4,
                        items: [
                            //num("SLCN", getFieldCaption("SLCN"), optFor("SLCN")),
                            wipLibrarySelect(ctxRowEdit, "SLCN", getFieldCaption("SLCN"), "/api/ThuVienWip/nang-luc-chuyen", "SLCN", optFor("SLCN"), { sendMaChuyen: true }),

                            // 2. Thời gian làm việc

                            wipLibrarySelect(ctxRowEdit, "TGLV", getFieldCaption("TGLV"), "/api/ThuVienWip/thoi-gian-lam-viec", "SoGio", optFor("TGLV"), { sendMaChuyen: true }),
                            // 3. Hệ số lợi nhuận
                            //num("TGLV", getFieldCaption("TGLV"), optFor("TGLV")),
                            num("OT", getFieldCaption("OT"), optFor("OT")),
                            num("HieuSuat", getFieldCaption("HieuSuat"), optFor("HieuSuat")),

                            num("CostPerM", getFieldCaption("CostPerM"), optFor("CostPerM")),
                            //num("Profit", getFieldCaption("Profit"), optFor("Profit")),
                            wipLibrarySelect(ctxRowEdit, "Profit", getFieldCaption("Profit"), "/api/ThuVienWip/he-so-loi-nhuan", "HeSo", optFor("Profit"), { sendMaChuyen: false }),

                            //num("SMV_WIP", getFieldCaption("SMV_WIP"), optFor("SMV_WIP")),
                            wipLibraryPickByStyle(
                                ctxRowEdit,
                                "SMV_WIP",
                                getFieldCaption("SMV_WIP"),
                                "/api/ThuVienWip/smv",
                                optFor("SMV_WIP"),
                                { autoPick: true, compareEmpty: true, valueField: "SMV" }
                            ),
                            num("NSCost", getFieldCaption("NSCost"), optFor("NSCost")),
                            num("NS_SMV", getFieldCaption("NS_SMV"), optFor("NS_SMV"))
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "3) Theo dõi doanh thu",
                        colSpan: 4,
                        colCount: 3,
                        items: [
                            wipLibraryPickByStyle(
                                ctxRowEdit,
                                "CM",
                                getFieldCaption("CM"),
                                "/api/ThuVienWip/get-cm",
                                optFor("CM"),
                                { autoPick: true, compareEmpty: true, valueField: "CM", tempIdField: "__libId_CM" }
                            ),
                            num("HeSoCM", getFieldCaption("HeSoCM"), optFor("HeSoCM")),
                            num("DoanhThuCM", getFieldCaption("DoanhThuCM"), optFor("DoanhThuCM"))
                        ]
                    },
                    //{
                    //    itemType: "group",
                    //    caption: "4) Tiến độ ra hàng",
                    //    colSpan: 4,
                    //    colCount: 4,
                    //    items: [
                    //        num("TT_Cat", getFieldCaption("TT_Cat"), optFor("TT_Cat", { readOnly: isEdit })),
                    //        num("BTP_DK", getFieldCaption("BTP_DK"), optFor("BTP_DK", { readOnly: isEdit })),
                    //        num("RaChuyen", getFieldCaption("RaChuyen"), optFor("RaChuyen", { readOnly: isEdit })),
                    //        num("Packing", "Packing", optFor("Packing", { readOnly: isEdit })),
                    //    ]
                    //},
                    {
                        itemType: "group",
                        caption: "4) Năng suất",
                        colSpan: 4,
                        colCount: 3,
                        items: [
                            num("NSGio", getFieldCaption("NSGio"), optFor("NSGio")),
                            num("SoGioSX", getFieldCaption("SoGioSX"), optFor("SoGioSX")),
                            num("SoNgay", getFieldCaption("SoNgay"), optFor("SoNgay"))
                        ]
                    },
                    {
                        itemType: "group",
                        caption: "5) Thời gian",
                        colSpan: 4,
                        colCount: 4,
                        items: [
                            date("KHCat", getFieldCaption("KHCat"), optFor("KHCat")),
                            date("KHLapTrinh", getFieldCaption("KHLapTrinh"), optFor("KHLapTrinh")),
                            date("KHMay", getFieldCaption("KHMay"), optFor("KHMay")),
                            date("ThoatChuyen", getFieldCaption("ThoatChuyen"), optFor("ThoatChuyen"))
                        ]
                    },

            ];
        }

        function renderForm() {
            return editRowPopupController.renderForm();
        }

        function openPopupForAdd(fieldToFocus) {
            return editRowPopupController.openForAdd(fieldToFocus);
        }

        function openPopupForEdit(rowData) {
            return editRowPopupController.openForEdit(rowData);
        }
        function openEditCellPopup(rowData, field) {
            //if (!ensureWritableMode()) return;
            if (!rowData || !field) {
                DevExpress.ui.notify("Không thể mở popup sửa ô", "warning", 1500);
                return;
            }
            if (!canEditFieldByPermission(field, rowData && rowData.LineX)) {
                DevExpress.ui.notify("Bạn không có quyền sửa cột này", "warning", 1500);
                return;
            }

            // Clone row để không ảnh hưởng grid
            editCellContext.row = $.extend(true, {}, rowData);
            editCellContext.field = field;

            // Đổi title theo field
            var caption = getFieldCaption(field, field);

            popupEditCell.option("title", "Sửa: " + caption);
            popupEditCell.show();
        }
        function focusFormField(field) {
            return editRowPopupController.focusFormField(field);
        }

        function getEmptyRow() {
            return WIP.UI.EditRowPopupController.getEmptyRow();
        }

        // ====== helpers form items ======
        function parseManualFlags(row) {
            if (!row) return {};
            var s = row.ManualJson;
            if (!s) return {};
            try { return JSON.parse(s) || {}; } catch { return {}; }
        }
        function isEqualValue(a, b) {
            // normalize null/"" and number string
            if (a === "") a = null;
            if (b === "") b = null;

            // date string "yyyy-MM-dd" compare
            return String(a ?? "") === String(b ?? "");
        }

        function buildManualFlagsPatch(oldRow, newRow) {
            return wipDataPayloadService.buildManualFlagsPatch(oldRow, newRow);
        }

        // attach vào payload (tên field tùy API/SP bạn set)
        function attachKeyJson(oldRow, newRow) {
            return wipDataPayloadService.attachKeyJson(oldRow, newRow);
        }
        function isManualCell(rowData, field) { 
            if (!rowData || !field) return false;
            var khMeta = getStepMetaByKhField(field);
            if (khMeta) {
                var manualMap = rowData.StepManualByField || rowData.stepManualByField || null;
                var canonicalField = String((khMeta && khMeta.KHField) || field || "").trim();
                var rawField = String(field || "").trim();
                if (!manualMap) return false;
                if (canonicalField && manualMap[canonicalField] !== undefined) return normalizeTruthyFlag(manualMap[canonicalField]);
                if (rawField && rawField !== canonicalField && manualMap[rawField] !== undefined) return normalizeTruthyFlag(manualMap[rawField]);
                return false;
            }

            var flags = rowData._manualFlags || parseManualFlags(rowData);
            if (!flags) return false;
            if (flags[field] === true) return true;

            return false;
        }

        // chỉ những field được phép manual/auto
        function isManualSupportedField(field) {
            field = String(field || "").trim();
            if (!field) return false;

            if (MANUAL_FIELDS.includes(field)) return true;

            var khMeta = getStepMetaByKhField(field);
            if (khMeta) return khMeta.AllowManualKH !== false;

            return false;
        }
        function toYMD(d) {
            if (!d) return null;
            const yyyy = d.getFullYear();
            const mm = String(d.getMonth() + 1).padStart(2, "0");
            const dd = String(d.getDate()).padStart(2, "0");
            return `${yyyy}-${mm}-${dd}`;
        }
        function normalizeDateOnly(obj) {
            Object.keys(obj || {}).forEach(function (k) {
                var v = obj[k];
                if (v instanceof Date) {
                    // convert Date -> "yyyy-MM-dd"
                    var yyyy = v.getFullYear();
                    var mm = String(v.getMonth() + 1).padStart(2, "0");
                    var dd = String(v.getDate()).padStart(2, "0");
                    obj[k] = yyyy + "-" + mm + "-" + dd;
                }
            });
            return obj;
        }
        function buildChangedFieldMap(changedFieldsObj) {
            return wipDataPayloadService.buildChangedFieldMap(changedFieldsObj);
        }
        function normalizeSaveRow(newRow, oldRow, changedFieldsObj) {
            return wipDataPayloadService.normalizeSaveRow(newRow, oldRow, changedFieldsObj);
        }
        function resolveNoDataStepMeta(field) {
            return wipDataPayloadService.resolveNoDataStepMeta(field);
        }
        function isDeltaSaveSupportedField(field) {
            return wipDataPayloadService.isDeltaSaveSupportedField(field);
        }
        function shouldSendDeltaManualPatch(field) {
            return wipDataPayloadService.shouldSendDeltaManualPatch(field);
        }
        function buildDeltaChangesFromRow(row, changedFieldsObj) {
            return wipDataPayloadService.buildDeltaChangesFromRow(row, changedFieldsObj);
        }
        function buildDeltaManualPatch(oldRow, newRow, changedFieldsObj) {
            return wipDataPayloadService.buildDeltaManualPatch(oldRow, newRow, changedFieldsObj);
        }
        function canUseSaveDelta(changes) {
            return wipDataPayloadService.canUseSaveDelta(changes);
        }
        function getUnsupportedSaveDeltaFields(changes) {
            return wipDataPayloadService.getUnsupportedSaveDeltaFields(changes);
        }
        var WipDataService = wipDataPayloadService;
        var GridViewAdapter = {
            updateGrid: function (gridInstance, dataSource, notifyMsg, notifyType, notifyTime) {
                wipGridController.updateGrid(gridInstance, dataSource, notifyMsg, notifyType, notifyTime);
            }
        };
        // Data Layer: fetch va xu ly du lieu thu vien, khong phu thuoc UI/form/grid.
        function getGridColumnFactoryDeps() {
            return {
                $: $,
                mdStore: mdStore,
                getDataCellCssClass: getDataCellCssClass,
                getFieldCaption: getFieldCaption,
                getFieldMaxLength: getFieldMaxLength,
                isNonNegField: isNonNegField,
                buildStrictDateEditorOptions: buildStrictDateEditorOptions
            };
        }

        function colDate(field, width, cssClass, opt) {
            return WIP.Grid.WipGridColumns.colDate(field, width, cssClass, opt, getGridColumnFactoryDeps());
        }
        function hydrateWipRow(row) {
            if (!row) return row;
            WIP.Domain.Row.hydrateWipRow(row, function () {
                return nextId++;
            });
            row._manualFlags = parseManualFlags(row);
            return row;
        }
        function hydrateWipRows(items) {
            return WIP.Domain.Row.hydrateWipRows(items, function () {
                return nextId++;
            }).map(function (row) {
                row._manualFlags = parseManualFlags(row);
                return row;
            });
        }
        function text(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.text(field, caption, editorOpt, getGridColumnFactoryDeps());
        }

        function reqText(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.reqText(field, caption, editorOpt, getGridColumnFactoryDeps());
        }

        function merSelect(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.merSelect(field, caption, editorOpt, getGridColumnFactoryDeps());
        }

        function num(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.num(field, caption, editorOpt, getGridColumnFactoryDeps());
        }
        /**
         * Auto pick SMV theo StyleId (WIP.StyleId) từ thư viện SMV
         * - Filter: maHang = styleId, maKH = MaKH (optional)
         * - Nếu field target đang rỗng/<=0 thì auto set
         */
        function wipLibraryPickByStyle(ctx, dataField, caption, apiUrl, opt, extra) {
            return formEditorFactory.wipLibraryPickByStyle(ctx, dataField, caption, apiUrl, opt, extra);
        }
        /**
         * wipLibrarySelect - dùng chung cho nhiều form/context
         *
         * @param {object} ctx
         *   - data: object formData đang bind (editorFormData hoặc editCellContext.row)
         *   - formSelector: string selector của dxForm (#wipForm hoặc #editCellForm)
         *   - getLineX?: function() => lineX (optional, nếu muốn lấy lineX động)
         *
         * @param {string} dataField
         * @param {string} caption
         * @param {string} apiUrl
         * @param {string} valueField  (SLCN / SoGio / HeSo...)
         * @param {object} opt
         * @param {object} extra
         *   - sendMaChuyen?: boolean (default true)
         *   - autoPickLatest?: boolean (default true)
         *   - compareDate?: boolean (default true)  // lọc <= today
         */
        function wipLibrarySelect(ctx, dataField, caption, apiUrl, valueField, opt, extra) {
            return formEditorFactory.wipLibrarySelect(ctx, dataField, caption, apiUrl, valueField, opt, extra);
        }
        function reqNum(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.reqNum(field, caption, editorOpt, getGridColumnFactoryDeps());
        }

        function date(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.date(field, caption, editorOpt, getGridColumnFactoryDeps());
        }

        function bool(field, caption, editorOpt) {
            return WIP.Grid.WipGridColumns.bool(field, caption, editorOpt);
        }

        // Drag/drop baseline và SignalR sync helpers.
        function pickBaseline(r) {
            r = r || {};
            return {
                MaKH: String(r.MaKH || "").trim(),
                MaHang: String(r.MaHang || "").trim(),
                Season: String(r.Season || "").trim(),
                StyleId: String(r.StyleId || "").trim(),
                // nếu bạn cần thêm: KhachHang, PO... thì add
            };
        }

        function matchBaseline(r, b) {
            if (!r || !b) return false;
            return String(r.MaKH || "").trim() === b.MaKH &&
                String(r.MaHang || "").trim() === b.MaHang &&
                String(r.Season || "").trim() === b.Season &&
                String(r.StyleId || "").trim() === b.StyleId;
        }

        function isRowCompatibleWithBaseline(r, b) {
            if (!r || !b) return false;
            if (String(r.MaHang || "").trim() !== b.MaHang) return false;
            return true;
        }
        function buildGhiChuGopFromRows(rows) {
            const list = [];
            (rows || []).forEach(r => {
                const gc = String(r?.GhiChu ?? "").trim();
                if (gc && !list.includes(gc)) list.push(gc);
            });
            return list.length ? list.join(" - ") : null;
        }

        function getGhiChuFromBaselineRow(row) {
            const gc = String(row?.GhiChu ?? "").trim();
            return gc || null;
        }
        function buildMaDHJoinedFromRows(rows) {
            const arr = (rows || [])
                .map(r => String(r?.MaDH || "").trim())
                .filter(Boolean);
            return Array.from(new Set(arr)).join(";");
        }
        // =========================
        // 7) SignalR -> SyncByMaDH -> Patch Pool (grid bottom)
        // =========================
        function debounceSyncByMaDH(maDH, delayMs) {
            wipSignalRService.debounceSyncByMaDH(maDH, delayMs, syncByMaDH);
        }

        function syncByMaDH(maDH) {
            return wipSignalRService.syncPoolByMaDH(maDH);
        }

        function patchPoolByMaDH(maDH, items) {
            wipPoolService.patchPoolByMaDH(maDH, items);
        }

        // =========================
        // 8) Init SignalR
        // =========================
        function cleanupSignalR() {
            wipSignalRService.cleanup();
        }
        function initSignalR() {
            wipSignalRService.init({
                delayMs: 250,
                onSyncByMaDH: syncByMaDH
            });
        }
        //setTimeout(function () {
            $("#Layer_1").trigger("click");
        //}, 1);
        // =========================
        // REGION 09 - Pool, SignalR, public WIP.* adapter và image helpers.
        // Init
        // =========================
        async function initPage() {
            await Promise.all([
                loadWipPermissions(userName),
                loadWipFeaturePermissions(userName)
            ]);
            initGridWidgets();
            initLineTabs();
            await Promise.all([loadStepStatusMeta(), loadColumnDynamicMeta(), loadSaveDeltaFieldMeta()]);
            setCrossModeUI(true);
            await setUnassignedUI(false);
            // Load tabs/data only after the grid structure is ready.
            await loadUnassignedData();
            await loadLineXList(true);
            initSignalR();
        }
        initPage().catch(err => {
            console.error(err);
            DevExpress.ui.notify("Init lỗi: " + (err.message || err), "error", 3000);
        });
        $(window).on("resize.wipResponsiveActions orientationchange.wipResponsiveActions", function () {
            scheduleResponsiveHeaderAndActions();
        });
        $(window).on("beforeunload", function () {
            cleanupSignalR();
        });
        // Public WIP.* contract cho các file popup load sau runtime.
        new WIP.Core.PublicApiAdapter(WIP).register({
            normalizeAssignResponse: normalizeResponse,
            replaceLineData: replaceLineData,
            loadWipData: loadWipData,
            syncLibByLine: syncLibByLine,
            loadUnassignedData: loadUnassignedData,
            addRowsToPool: addRowsToPool,
            mapRowId: mapRowId,
            getGridUnassigned: function () { return gridUnassigned; }
        });

    };
})(window, jQuery);
function imageToBase64(imageSrc) {
    return WIP.Utils.Image.imageToBase64(imageSrc);
}
function isBase64(str) {
    return WIP.Utils.Image.isBase64Image(str);
}
async function getImageByField() {
    return await WIP.Utils.Image.getImageByField("/Content/Image/LOGOVIKING.png");
}
