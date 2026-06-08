// popupSplitLine.js
(function () {
    window.WIP = window.WIP || {};

    // =========================
    // STATE
    // =========================

    //Global state dùng lại trong popup
    const splitState = {
        gopDH: "",
        maGop: "",
        existsCache: new Map()
    };

    function existsKey(maDH, maLenhSanXuat, lineMoiId) {
        return `${String(maDH || "").trim()}|${String(maLenhSanXuat || "").trim()}|${Number(lineMoiId || 0)}`;
    }

    let popup = null;
    let gridTop = null;
    let gridBottom = null;
    let editors = {
        MaDH: null,
        LenhSX: null,
        LineGocText: null,
        LineMoi: null
    };

    let formData = {
        MaDH: "",
        gopDH:"",
        LenhSX: "",
        LineGocId: 0,
        LineGocText: "",
        LineMoi: null,
        MaLenhSanXuat: "",
        Mer: ""
    };
    let ctx = null; // payload from open()
    const APPROVE_NOTIFY_MODULE_ID = "M.43.00.00";

    // Datasets
    let _rowsTop = [];
    let _rowsTopSource = [];
    let _rowsBottom = [];

    let lp = null; // load panel instance
    let _loadingCount = 0;
    function isPhoneDevice() {
        const ua = String(navigator.userAgent || "").toLowerCase();
        const uaMobile = /android|iphone|ipad|ipod|windows phone|opera mini|mobile/i.test(ua);
        let coarsePointer = false;
        try {
            coarsePointer = !!(window.matchMedia && (window.matchMedia("(pointer: coarse)").matches || window.matchMedia("(hover: none)").matches));
        } catch (e) { }
        return !!(uaMobile || coarsePointer);
    }
    function isMobileUiMode() {
        if (isPhoneDevice()) return true;
        return window.innerWidth < 992;
    }
    function getPopupResponsiveOptions() {
        if (!isMobileUiMode()) return {};
        return {
            fullScreen: true,
            width: "100vw",
            height: "100vh",
            maxWidth: "100vw",
            maxHeight: "100vh",
            dragEnabled: false
        };
    }
    function isSizeField(field) {
        // theo Win: "95@SIZE_95"
        const s = String(field || "");
        return s.includes("@") && !["POID", "PO", "MaDH", "MaMau", "TenMau", "DauSize", "DauSizeID", "CodeMau", "Line", "Name", "IsLineGoc", "Amount"].includes(s);
    }

    function rowKey(r) {
        // giống WinForms: POID + MaDH + MaMau + DauSizeID + CodeMau
        return [
            r.MaDH || "",
            r.POID || "",
            r.MaMau || "",
            r.DauSizeID || "",
            r.CodeMau || ""
        ].join("|");
    }

    function getSizeFieldsFromRow(r) {
        return Object.keys(r || {}).filter(k => isSizeField(k));
    }

    function parseSizeFieldKey(k) {
        const parts = String(k).split("@"); // ["95","SIZE_95"]
        return { caption: parts[0] || k, sizeId: parts[1] || k };
    }

    function calcAmount(r) {
        let sum = 0;
        const fs = getSizeFieldsFromRow(r);
        for (const f of fs) sum += (Number(r[f] ?? 0) || 0);
        return sum;
    }

    function notify(msg, type = "info", ms = 2000) {
        // bạn có notify2 ngoài page thì ưu tiên dùng
        if (typeof window.notify2 === "function") return window.notify2(msg, type, ms);
        DevExpress.ui.notify(msg, type, ms);
    }

    function refreshGridLayouts() {
        setTimeout(() => {
            try { gridTop?.updateDimensions(); } catch { }
            try { gridBottom?.updateDimensions(); } catch { }
        }, 0);
    }
    function ensurePopupScrollable() {
        if (!popup) return;
        try {
            const $content = popup.content();
            const $overlay = $content.closest(".dx-overlay-content");
            $overlay.addClass("wip-split-popup-overlay");
            $content.css({
                overflowY: "auto",
                overflowX: "hidden",
                maxHeight: "100%"
            });
        } catch (e) { }
    }

    // =========================
    // INIT UI
    // =========================
    function ensure() {
        if (popup) return;

        popup = $("#popupSplitLine").dxPopup($.extend(true, {
            title: "Chia chuyền",
            width: "92vw",
            height: "92vh",
            maxWidth: 1250,
            showTitle: true,
            dragEnabled: true,
            hideOnOutsideClick: true,
            deferRendering: false,

            toolbarItems: [
                {
                    widget: "dxButton", toolbar: "bottom", location: "after",
                    options: { text: "Lưu", type: "success", onClick: onSaveClick }
                },
                {
                    widget: "dxButton", toolbar: "bottom", location: "after",
                    options: { text: "Đóng", onClick: () => popup.hide() }
                }
            ],
            onShown: function () {
                ensurePopupScrollable();
                try {
                    const phoneMode = isPhoneDevice();
                    gridTop?.option("columnAutoWidth", !phoneMode);
                    gridBottom?.option("columnAutoWidth", !phoneMode);
                    gridTop?.option("columnFixing", { enabled: !phoneMode });
                    gridBottom?.option("columnFixing", { enabled: !phoneMode });
                } catch (e) { }
                try {
                    popup.content().css("overflow", "auto");
                } catch (e) { }
                refreshGridLayouts();
            },
            onHiding: function () {
                resetPopupState();
            }

        }, getPopupResponsiveOptions())).dxPopup("instance");
        lp = $("<div/>").appendTo(popup.content()).dxLoadPanel({
            shading: true,
            showPane: true,
            closeOnOutsideClick: false,
            visible: false,
            message: "Đang tải dữ liệu..."
        }).dxLoadPanel("instance");
        buildForm();
        buildGrids();
    }
    function setLoading(on, message) {
        if (!lp) return;

        if (on) {
            _loadingCount++;
            if (message) lp.option("message", message);
            lp.show();
        } else {
            _loadingCount = Math.max(0, _loadingCount - 1);
            if (_loadingCount === 0) lp.hide();
        }
    }
    function resetPopupState() {
        ctx = null;
        _rowsTop = [];
        _rowsTopSource = [];
        _rowsBottom = [];

        formData = { MaDH: "", gopDH:"", LenhSX: "", LineGocId: 0, LineGocText: "", LineMoi: null, MaLenhSanXuat: "", Mer: "" };

        try { editors.MaDH?.option("value", ""); } catch { }
        try { editors.LenhSX?.option("value", ""); } catch { }
        try { editors.LineGocText?.option("value", ""); } catch { }
        try { editors.LineMoi?.option({ dataSource: [], value: null }); } catch { }

        try { gridTop?.option({ columns: [], dataSource: [] }); } catch { }
        try { gridBottom?.option({ columns: [], dataSource: [] }); } catch { }
        try { _pendingAjax?.abort(); } catch { }
        _loadingCount = 0;
        try { lp?.hide(); } catch { }
    }

    function buildForm() {
        editors.MaDH = $("#sl_MaDH").dxTextBox({ readOnly: true }).dxTextBox("instance");
        editors.gopDH = $("#sl_gopDH").dxTextBox({ readOnly: true }).dxTextBox("instance");
        editors.LenhSX = $("#sl_LenhSX").dxTextBox({ readOnly: true }).dxTextBox("instance");
        editors.LineGocText = $("#sl_LineGocText").dxTextBox({ readOnly: true }).dxTextBox("instance");

        editors.LineMoi = $("#sl_LineMoi").dxSelectBox({
            dataSource: [],
            valueExpr: "id",
            displayExpr: "name",
            searchEnabled: false,
            placeholder: "Chọn chuyền",
            onValueChanged: async (e) => {
                formData.LineMoi = e.value;
                if (!formData.LineMoi) {
                    _rowsBottom = [];
                    try { gridBottom?.option("dataSource", []); } catch { }
                    return;
                }

                await loadBottomGridData(formData.LineMoi);

            }
        }).dxSelectBox("instance");
    }
    function validateForm() {
        if (!formData.LineMoi) {
            notify("Vui lòng chọn chuyền cần chia", "warning", 2200);
            editors.LineMoi.focus();
            return false;
        }
        return true;
    }
    let _suppress = false;
    function buildGrids() {
        // TOP = readonly
        gridTop = $("#gridSplitTop").dxDataGrid({
            dataSource: [],
            showBorders: true,
            width: "100%",
            height: "100%",
            scrolling: { mode: "standard", useNative: true, showScrollbar: "always" },
            editing: { mode: "cell", allowUpdating: false },
            columnAutoWidth: !isMobileUiMode(),
            columnFixing: { enabled: !isMobileUiMode() },
            wordWrapEnabled: false,
            columns: []
        }).dxDataGrid("instance");

        // BOTTOM = editable (size cols only)
        gridBottom = $("#gridSplitBottom").dxDataGrid({
            dataSource: [],
            showBorders: true,
            width: "100%",
            height: "100%",
            scrolling: { mode: "standard", useNative: true, showScrollbar: "always" },
            editing: {
                mode: "cell",
                allowUpdating: true,
                selectTextOnEditStart: true,
                startEditAction: "click",
                enterKeyAction: "moveFocus",
                enterKeyDirection: "row"
            },
            columnAutoWidth: !isMobileUiMode(),
            columnFixing: { enabled: !isMobileUiMode() },
            columns: [],

            onEditorPreparing: function (e) {
                if (e.parentType !== "dataRow") return;

                if (e.row?.data?.IsLineGoc == 1) { e.cancel = true; return; }

                if (!isSizeField(e.dataField)) return;

                e.editorName = "dxNumberBox";
                e.editorOptions = e.editorOptions || {};
                e.editorOptions.min = 0;
                e.editorOptions.step = 1;
                e.editorOptions.showSpinButtons = true;

                const field = e.dataField;
                const row = e.row.data;

                let prevVal = parseInt(e.value ?? 0, 10);
                if (isNaN(prevVal) || prevVal < 0) prevVal = 0;

                e.editorOptions.onValueChanged = function (args) {
                    if (_suppress) return;

                    let newVal = parseInt(args.value ?? 0, 10);
                    if (isNaN(newVal) || newVal < 0) newVal = 0;

                    const delta = newVal - prevVal;

                    const key = rowKey(row);
                    const topIndex = _rowsTop.findIndex(x => rowKey(x) === key);
                    const rTop = topIndex >= 0 ? _rowsTop[topIndex] : null;

                    if (!rTop) {
                        _suppress = true;
                        row[field] = newVal;
                        e.setValue(newVal);
                        _suppress = false;

                        gridBottom.repaintRows([e.row.rowIndex]);
                        prevVal = newVal;
                        return;
                    }

                    const topVal = parseInt(rTop[field] ?? 0, 10) || 0;

                    if (delta > topVal) {
                        notify(`Không được nhập vượt quá số lượng còn lại (${topVal}).`, "warning", 2200);

                        _suppress = true;
                        args.component.option("value", prevVal); 
                        row[field] = prevVal;  
                        e.setValue(prevVal);
                        _suppress = false;

                        gridBottom.repaintRows([e.row.rowIndex]);
                        return;
                    }

                    rTop[field] = topVal - delta;

                    _suppress = true;
                    row[field] = newVal; 
                    e.setValue(newVal);
                    _suppress = false;

                    gridTop.repaintRows([topIndex]);
                    gridBottom.repaintRows([e.row.rowIndex]);

                    prevVal = newVal;
                };
            }
        }).dxDataGrid("instance");
    }

    // =========================
    // DATA LOAD + BUILD COLUMNS
    // =========================
    async function apiGetJson(url) {
        return $.ajax({ url, method: "GET", dataType: "json" });
    }

    function getChiTietLenhUrl(maDH, lineId, maLenhSXStr) {
        const apiBase = (ctx?.apiBase || "/api/").replace(/\/?$/, "/");

        return apiBase + "ERPDonHangTong/Get"
            + "?action=GetChiTietLenh"
            + "&para=" + encodeURIComponent(maDH)
            + "&para2="
            + "&para3="
            + "&para4=" + encodeURIComponent(String(lineId))
            + "&para5=" + encodeURIComponent(maLenhSXStr);
    }
    function deepCloneRows(rows) {
        return (rows || []).map(r => JSON.parse(JSON.stringify(r)));
    }
    function buildColumnsFromRows(sampleRow) {
        const phoneMode = isPhoneDevice();
        const sizeFields = getSizeFieldsFromRow(sampleRow);

        const sizeCols = sizeFields.map(k => {
            const p = parseSizeFieldKey(k);
            return {
                caption: p.caption,
                dataField: k,
                alignment: "center",
                minWidth: 70,
                customizeText(e) {
                    const v = Number(e.value ?? 0) || 0;
                    return v === 0 ? "-" : String(v);
                }
            };
        });

        const baseCols = [
            { caption: "Đơn hàng", dataField: "MaDH", fixed: !phoneMode, width: 90, allowEditing: false },
            { caption: "Màu", dataField: "TenMau", fixed: !phoneMode, width: 120, allowEditing: false },
            { caption: "PO", dataField: "PO", fixed: !phoneMode, width: 110, allowEditing: false },
            { caption: "Nhóm size", dataField: "DauSize", fixed: !phoneMode, width: 90, allowEditing: false },
            { caption: "Chuyền", dataField: "Name", fixed: !phoneMode, width: 90, allowEditing: false }
        ];

        const totalCol = {
            caption: "Tổng",
            dataField: "Amount",
            fixed: !phoneMode,
            fixedPosition: !phoneMode ? "right" : undefined,
            width: 80,
            alignment: "center",
            allowEditing: false,
            calculateCellValue: function (rowData) {
                return calcAmount(rowData);
            }
        };

        _sizeCols = sizeCols;
        _baseCols = baseCols;
        _totalCol = totalCol;
        return [...baseCols, ...sizeCols, totalCol];
    }
    async function loadTopGridData() {
        if (!ctx) return;

        setLoading(true, "Đang tải dữ liệu chuyền gốc...");

        try {
            const fd = formData;
            const maDH = String(fd.MaDH || "").trim();
            const maDHSplit = maDH.split(';');
            const lineGocId = Number(fd.LineGocId || 0) || 0;
            const maLenhSXStr = String(fd.MaLenhSanXuat || ctx?.maLenhSanXuat || "").trim();

            if (!maDH || !lineGocId) {
                notify("Thiếu MaDH hoặc Line gốc.", "warning", 2200);
                return;
            }

            const topData = await apiGetJson(getChiTietLenhUrl(maDHSplit[0], lineGocId, maLenhSXStr));
            if (!Array.isArray(topData) || !topData.length) {
                notify("Không có dữ liệu line gốc để chia (GetChiTietLenh trả rỗng).", "warning", 2500);
                _rowsTop = [];
                _rowsTopSource = [];
                _rowsBottom = [];
                gridTop.option("dataSource", []);
                gridTop.option("columns", []);
                gridBottom.option("columns", []);
                gridBottom.option("dataSource", []);
                return;
            }

            for (const r of topData) r.Amount = calcAmount(r);
            const columns = buildColumnsFromRows(topData[0]);

            _rowsTopSource = deepCloneRows(topData);
            _rowsTop = deepCloneRows(topData);
            _rowsBottom = [];

            gridTop.option("columns", columns);
            gridTop.option("dataSource", _rowsTop);

            gridBottom.option("columns", columns);
            gridBottom.option("dataSource", _rowsBottom);
            refreshGridLayouts();
        }
        catch (err) {
            console.log(err);
            notify("Lỗi tải dữ liệu chuyền gốc. Xem Console.", "error", 2500);
        }
        finally {
            setLoading(false);
        }
    }
    async function loadBottomGridData(lineMoiId) {
        if (!ctx) return;

        const selectedLineMoiId = Number(lineMoiId || 0) || 0;
        if (!selectedLineMoiId) {
            _rowsBottom = [];
            gridBottom.option("dataSource", []);
            return;
        }

        if (!_rowsTopSource.length) {
            await loadTopGridData();
            if (!_rowsTopSource.length) return;
        }

        setLoading(true, "Đang tải dữ liệu chuyền cần chia...");

        try {
            const fd = formData;
            const maDH = String(fd.MaDH || "").trim();
            const maDHSplit = maDH.split(';');
            const maLenhSXStr = String(fd.MaLenhSanXuat || ctx?.maLenhSanXuat || "").trim();

            if (!maDH) {
                notify("Thiếu MaDH.", "warning", 2200);
                return;
            }

            let bottomData = [];
            try {
                bottomData = await apiGetJson(getChiTietLenhUrl(maDHSplit[0], selectedLineMoiId, maLenhSXStr));
            } catch {
                bottomData = [];
            }

            _rowsTop = deepCloneRows(_rowsTopSource);

            if (Array.isArray(bottomData) && bottomData.length) {
                for (const r of bottomData) r.Amount = calcAmount(r);
                _rowsBottom = deepCloneRows(bottomData);
            } else {
                const lineMoiObj = (ctx.lines || []).find(x => Number(x.id) === selectedLineMoiId);
                _rowsBottom = deepCloneRows(_rowsTopSource).map(r => {
                    const rr = { ...r, Line: String(selectedLineMoiId), Name: lineMoiObj?.name || "", IsLineGoc: 0 };
                    for (const f of getSizeFieldsFromRow(rr)) rr[f] = 0;
                    rr.Amount = 0;
                    return rr;
                });
            }

            gridTop.option("dataSource", _rowsTop);
            gridBottom.option("dataSource", _rowsBottom);
            refreshGridLayouts();
        }
        catch (err) {
            console.log(err);
            notify("Lỗi tải dữ liệu chuyền cần chia. Xem Console.", "error", 2500);
        }
        finally {
            setLoading(false);
        }
    }


    // =========================
    // SAVE (placeholder)
    // =========================
    function normText(x) {
        return String(x ?? "").trim().replace(/^['"]+|['"]+$/g, "").toUpperCase();
    }
    function tvpRowTemplate() {
        return {
            ID: null,
            MaLenhSanXuat: null,
            MaLenh: null,
            TenLenh: null,
            MaDH: null,
            MaDVSX: null,
            DotSX: null,
            MaQG: null,
            POID: null,
            PO: null,
            MaMau: null,
            DauSizeID: null,
            DauSize: null,
            SizeID: null,
            Size: null,
            SoLuong: 0,
            TrangThai: null,
            STTLenh: null,
            MaGop: null,
            POID_T: null,
            MaCu: null,
            Line: null,
            GhiChu: null
        };
    }

    function buildSaveList(rows, maLenhSanXuat) {
        const list = [];
        for (const r of (rows || [])) {
            for (const k of Object.keys(r)) {
                if (!isSizeField(k)) continue;

                const parts = String(k).split("@");
                const sizeText = parts[0] || null;
                const sizeId = parts[1] || null;

                const item = tvpRowTemplate();

                item.MaLenhSanXuat = maLenhSanXuat;
                item.MaDH = String(r.MaDH ?? "");
                item.POID = String(r.POID ?? "");
                item.PO = String(r.PO ?? "");
                item.MaMau = String(r.MaMau ?? "");
                item.DauSizeID = String(r.DauSizeID ?? "");
                item.DauSize = String(r.DauSize ?? "");
                item.MaQG = String(r.MaQG ?? "");

                item.Line = String(r.Line ?? r.LineX ?? "");
                item.SizeID = String(sizeId ?? "");
                item.Size = String(sizeText ?? "");

                item.SoLuong = parseInt(r[k] ?? 0, 10) || 0;

                // các cột khác không có thì để null
                list.push(item);
            }
        }
        return list;
    }
    async function checkLineExists(maDH, lineMoiId, maLenhSXStr) {
        const apiBase = (ctx.apiBase || "/api/").replace(/\/?$/, "/");

        const url =
            apiBase + "wip-donhang/checkLineExists"
            + "?maDH=" + encodeURIComponent(maDH)
            + "&maLenhSanXuat=" + encodeURIComponent(maLenhSXStr)
            + "&lineX=" + encodeURIComponent(String(lineMoiId));

        // controller trả JSON: { ok:true, exists:true/false, raw:"TRUE/FALSE", maGop:"..." }
        const res = await fetch(url, { method: "GET", credentials: "include" }).then(r => r.json());

        return {
            exists: !!res?.exists,
            maGop: (res?.maGop ?? "").toString().trim(),
            raw: res?.raw
        };
    }
    async function ensureFrameCreated(maGop, lineGocId, lineMoiId, maLenhSXStr) {
        const apiBase = (ctx.apiBase || "/api/").replace(/\/?$/, "/");
        const userName = getCurrentUserName();
        const urlCreate =
            apiBase + "ERPDonHangTong/GetChuyen"
            + "?action=PostChiaChuyen"
            + "&para=" + encodeURIComponent(String(maGop || "").trim())
            + "&para2=" + encodeURIComponent(String(lineGocId))
            + "&para3=" + encodeURIComponent(String(lineMoiId))
            + "&para4=0"
            + "&para5=" + encodeURIComponent(maLenhSXStr)
            + "&para6=" + encodeURIComponent(userName); 

        const text = await $.ajax({ url: urlCreate, method: "GET", dataType: "text" });
        return normText(text) === "TRUE";
    }
    function getCurrentUserName() {
        return localStorage.getItem("username") || localStorage.getItem("username1") || "";
    }
    function sendApproveNotify(title, detail, sendTo, boPhan = "ALL", status = -1) {
        const userName = getCurrentUserName();
        const url = `/api/SendToNotification/PushNotification?` +
            `UserIDTao=${encodeURIComponent(userName)}&` +
            `ModuleID=${encodeURIComponent(APPROVE_NOTIFY_MODULE_ID)}&` +
            `Title=${encodeURIComponent(title)}&` +
            `Detail=${encodeURIComponent(detail)}&` +
            `SendTo=${encodeURIComponent(sendTo)}&` +
            `BoPhan=${encodeURIComponent(boPhan)}&` +
            `Status=${encodeURIComponent(status)}`;

        return $.ajax({
            url: url,
            type: "POST",
            contentType: false,
            processData: false
        });
    }
    async function safeApproveStep(name, fn, errors) {
        try {
            return await fn();
        } catch (e) {
            console.error(name + " error:", e);
            errors.push({ step: name, error: e });
            return null;
        }
    }
    async function approveAfterSplitSave(data) {
        const errors = [];
        const apiBase = (ctx?.apiBase || "/api/").replace(/\/?$/, "/");
        const maDH = String(data.maDH || "").trim();
        const maLenh = String(data.maLenh || "").trim();
        const maLenhSanXuat = String(data.maLenhSanXuat || "").trim();
        const maKH = String(data.maKH || "").trim();
        const styleId = String(data.styleId || "").trim();
        const userName = getCurrentUserName();
        const maGop = String(data.maGop || "").trim();
        if (!maDH || !maLenh || !maLenhSanXuat || !maKH || !styleId) {
            notify("Thiếu MaDH/MaLenh/MaLenhSanXuat/MaKH/StyleId để duyệt.", "warning", 3000);
            return { ok: false, fatal: true, errors: [{ step: "validate", error: "missing required fields" }] };
        }
        let maDot = "";
        const tblDot = await safeApproveStep("GETMAXDOT", async function () {
            const urlDot = apiBase + "ERPDonHangTong/Get"
                + "?Action=GETMAXDOT"
                + "&para=" + encodeURIComponent(maKH)
                + "&para2=" + encodeURIComponent(styleId);
            return await apiGetJson(urlDot);
        }, errors);
        if (Array.isArray(tblDot) && tblDot.length) {
            maDot = String(tblDot[0].MaDot || "").trim();
        }

        await safeApproveStep("GET_TSNEW", async function () {
            const urlTS = apiBase + "KhoiTaoBOMV1/Get"
                + "?action=GET_TSNEW"
                + "&para1=" + encodeURIComponent(maKH)
                + "&para2=" + encodeURIComponent(styleId)
                + "&para6=" + encodeURIComponent(userName)
                + "&para7=" + encodeURIComponent(maDot);
            return await apiGetJson(urlTS);
        }, errors);

        await safeApproveStep("XacNhanBOM", async function () {
            const urlXN = apiBase + "ERPDonHangTong/Get"
                + "?action=XacNhanBOM"
                + "&para=" + encodeURIComponent(maLenh)
                + "&para2=" + encodeURIComponent(maKH)
                + "&para3=" + encodeURIComponent(styleId);
            return await apiGetJson(urlXN);
        }, errors);

        const dongBoResult = await safeApproveStep("DongBo", async function () {
            const urlDongBo = apiBase + "ERPDongBoQLSX/DongBo"
                + "?action=DongBo"
                + "&para=" + encodeURIComponent(maLenh);
            return await $.ajax({ url: urlDongBo, method: "GET", dataType: "text" });
        }, errors);
        if (String(dongBoResult || "").trim().toUpperCase() === "2") {
            console.error("DongBo returned 2: Duyệt thất bại.");
            errors.push({ step: "DongBo", error: "returned 2" });
        }

        await safeApproveStep("GhiLogLenh", async function () {
            const urlLog = apiBase + "CanDoiDinhMucNPLLog/GhiLogLenh"
                + "?action=GhiLogLenh"
                + "&para=" + encodeURIComponent("Xác nhận lệnh trên WIP")
                + "&para1=" + encodeURIComponent("ChiaChuyenVaDuyetSXWIP")
                + "&para2=" + encodeURIComponent(maDH + "@" + maLenhSanXuat)
                + "&para3=" + encodeURIComponent(userName);
            return await apiGetJson(urlLog);
        }, errors);

        const detail =
            `Đơn Hàng: ${maDH}\n` +
            `Mã Hàng: ${data.tenHang || styleId}\n` +
            `Lệnh: ${maLenh}`;
        await safeApproveStep("sendApproveNotify", async function () {
            return await sendApproveNotify("Lệnh sản xuất đã xác nhận số lượng", detail, "PKH", "ALL", 1);
        }, errors);

        await safeApproveStep("BatDuyetLai", async function () {
            const urlBatDuyetLai = apiBase + "ERPDonHangTong/Get"
                + "?action=BatDuyetLai"
                + "&para=" + encodeURIComponent(maLenhSanXuat)
                + "&para2=" + encodeURIComponent(maGop)
                + "&para3=";
            return await apiGetJson(urlBatDuyetLai);
        }, errors);

        return { ok: errors.length === 0, fatal: false, errors };
    }
    async function onSaveClick() {
        if (!validateForm()) return;

        try { await gridBottom.saveEditData(); } catch { }

        const fd = formData;
        const maDH = String(fd.MaDH || "").trim();
        const lineGocId = Number(fd.LineGocId || 0) || 0;
        const lineMoiId = Number(fd.LineMoi || 0) || 0;
        const maLenhSXStr = String(ctx?.maLenhSanXuat || "").trim();
        const mer = String(fd.Mer || "").trim();
        const skipUpdateMer = !mer;
        if (!maLenhSXStr) {
            notify("Thiếu MaLenhSanXuat (para5).", "warning", 2500);
            return;
        }

        if (!maDH || !lineGocId || !lineMoiId) {
            notify("Thiếu dữ liệu MaDH / LineGoc / LineMoi.", "warning", 2500);
            return;
        }
        const maGop = String(splitState.maGop || fd.MaGop || "").trim();
        if (!maGop) {
            notify("Không xác định được MaGop.", "warning", 2500);
            return;
        }

        let exists = false;
        try {
            const chk = await checkLineExists(maDH, lineMoiId, maLenhSXStr);
            exists = chk.exists;
        } catch (e) {
            console.log("Lỗi kiểm tra tồn tại khung chia chuyền:", e);
        }

        if (!exists) {
            const created = await ensureFrameCreated(maGop, lineGocId, lineMoiId, maLenhSXStr);
            if (!created) {
                notify("Tạo khung chia chuyền thất bại.", "error", 3500);
                return;
            }
        }

        const listSave = [
            ...buildSaveList(_rowsTop, maLenhSXStr),
            ...buildSaveList(_rowsBottom, maLenhSXStr)
        ];

        if (!listSave.length) {
            notify("Không có dữ liệu để lưu.", "warning", 2000);
            return;
        }

        const apiBase = (ctx.apiBase || "/api/").replace(/\/?$/, "/");
        const url = apiBase + "ERPDonHangTong/Post?action=PostSoLuongChuyen&type=@TypeTable";

        const respText = await $.ajax({
            url,
            method: "POST",
            data: JSON.stringify(listSave),
            contentType: "application/json; charset=utf-8",
            dataType: "text"
        });

        const ok = normText(respText) === "TRUE";
        if (!ok) {
            notify("Lưu thất bại: " + respText, "error", 4000);
            return;
        }

        let approveResult = null;
        setLoading(true, "Đang duyệt lệnh...");
        try {
            approveResult = await approveAfterSplitSave({
                maDH,
                maLenh: ctx?.lenhSX,
                maLenhSanXuat: maLenhSXStr,
                maKH: ctx?.maKH,
                styleId: ctx?.styleId,
                tenHang: ctx?.tenHang,
                maGop: maGop
            });
        } catch (e) {
            console.error("approveAfterSplitSave error:", e);
            notify("Lưu thành công nhưng duyệt lệnh thất bại. Xem Console.", "error", 4000);
            return;
        } finally {
            setLoading(false);
        }

        if (!approveResult?.ok) {
            notify("Đã chia chuyền và duyệt số lượng nhưng có bước lỗi. Xem Console.", "warning", 4000);
            if (approveResult?.fatal) return;
        } else {
            notify("Đã lưu số lượng chia chuyền và duyệt lệnh.", "success", 1800);
        }

        //const lineX = listSave[0].Line;
        popup.hide();
        await WIP.loadUnassignedData();
        await WIP.loadWipData({ /*lineX*/ });

        await loadLineSilent(lineMoiId);

        if (!skipUpdateMer) {
            updateMer(maLenhSXStr, maDH, lineMoiId, mer)
                .catch(err => console.error("updateMer error:", err));
        }
    }
    function updateMer(maLenhSanXuat, maDH, newLine, mer) {
        var url = "/api/wip-donhang/update-mer";
        return $.ajax({
            url: url,
            method: "POST",
            dataType: "text",
            data: {
                maLenhSanXuat: maLenhSanXuat,
                maDH: maDH,
                newLine: newLine,
                mer: mer
            }
        });
    }
    function loadLineSilent(lineX) {
        var lx = Number(lineX || 0) || 0;
        if (!lx) {
            return $.Deferred().resolve(null).promise();
        }

        var url = "/api/wip-donhang/loadline";

        return $.ajax({
            url: url,
            method: "GET",
            dataType: "json",
            timeout: 20000,
            data: {
                lineX: lx,
            }
        }).fail(function (xhr) {
            if (xhr && xhr.statusText === "abort") return;
            console.error("[WIP.loadLineSilent] sync fail:", {
                lineX: lx,
                status: xhr && xhr.status,
                message: (xhr && (xhr.responseText || xhr.statusText)) || "unknown error"
            });
        });
    }
    // =========================
    // PUBLIC OPEN
    // =========================
    async function fetchChiTietGop(maGop, maLenhSanXuat) {
        maGop = String(maGop || "").trim();
        if (!maGop) return [];

        const url =
            `/api/ERPDonHangTong/Get?action=GetChiTiet&para=${encodeURIComponent(maGop)}&para5=${encodeURIComponent(maLenhSanXuat)}`;

        const resp = await fetch(url, { method: "GET" });
        if (!resp.ok) throw new Error(`HTTP ${resp.status}`);

        const json = await resp.json();
        let gopDH = "";

        if (Array.isArray(json) && json.length > 0) {
            gopDH = String(json[0].GopDH || "").trim();
        }
        splitState.gopDH = gopDH;
        return Array.isArray(json) ? json : [];
    }
    function showWarnNotGoc(lineGocId, lineGocName) {
        // DevExtreme: bạn có thể dùng DevExpress.ui.notify hoặc dialog
        DevExpress.ui.dialog.alert(
            `Chuyền này không phải chuyền gốc.\nChuyền gốc là: ${lineGocName}`,
            "Cảnh báo"
        );
    }
    async function open(payload) {
        ensure();
        ctx = payload || {};

        const maDH = String(ctx.maDH || "").trim();
        const maLenhSanXuat = String(ctx.maLenhSanXuat || "").trim();
        const selectedLineId = Number(ctx.lineGoc || 0) || 0;
        const mer = String(ctx.mer || "").trim();
        let skipUpdateMer = false;
        if (mer !== null && mer !== undefined && mer !== "") {
            skipUpdateMer = true;
        }
        if (!maDH) {
            DevExpress.ui.dialog.alert("Thiếu MaDH.", "Thông báo");
            return;
        }
        if (!maLenhSanXuat) {
            DevExpress.ui.dialog.alert("Thiếu MaLenhSanXuat (para5).", "Thông báo");
            return;
        }
        if (!selectedLineId) {
            DevExpress.ui.dialog.alert("Thiếu LineGoc (line hiện tại).", "Thông báo");
            return;
        }

        let maGop = "";
        try {
            const chk = await checkLineExists(maDH, selectedLineId, maLenhSanXuat);
            maGop = chk.maGop;
        } catch (e) {
            DevExpress.ui.dialog.alert(`Không gọi được API checkLineExists.\n${e.message}`, "Lỗi");
            console.log(e);
            return;
        }

        if (!maGop) {
            DevExpress.ui.dialog.alert("Không lấy được MaGop từ checkLineExists.", "Thông báo");
            return;
        }

        splitState.maGop = maGop;

        let rows = [];
        try {
            rows = await fetchChiTietGop(maGop, maLenhSanXuat);
        } catch (e) {
            DevExpress.ui.dialog.alert(`Không gọi được API GetChiTiet.\n${e.message}`, "Lỗi");
            console.log(e);
            return;
        }

        if (!rows.length) {
            DevExpress.ui.dialog.alert("Không tìm thấy dữ liệu GetChiTiet để xác định chuyền gốc.", "Thông báo");
            return;
        }

        const first = rows[0];
        const apiLineGocId = Number(first.Line || 0) || 0;
        const apiLineGocName = String(first.Name || "");

        if (selectedLineId !== apiLineGocId) {
            showWarnNotGoc(apiLineGocId, apiLineGocName || `Line ${apiLineGocId}`);
            return;
        }

        const lines = (ctx.lines || []).map(x => ({ id: Number(x.id), name: String(x.name || "") }));
        const lineGocText = apiLineGocName || (lines.find(x => x.id === apiLineGocId)?.name || String(apiLineGocId));

        formData = {
            MaDH: maDH,
            gopDH: splitState.gopDH,
            LenhSX: ctx.lenhSX || "",
            LineGocId: apiLineGocId,
            LineGocText: lineGocText,
            LineMoi: null,
            MaLenhSanXuat: maLenhSanXuat,
            MaGop: maGop,
            Mer: mer,
        };

        editors.MaDH.option("value", formData.MaDH);
        editors.gopDH.option("value", formData.gopDH);
        editors.LenhSX.option("value", formData.LenhSX);
        editors.LineGocText.option("value", formData.LineGocText);

        editors.LineMoi.option("dataSource", lines.filter(x => x.id !== apiLineGocId));
        editors.LineMoi.option("value", null);

        popup.show();
        await loadTopGridData();
    }

    window.WIP.SplitLinePopup = { open };
})();
