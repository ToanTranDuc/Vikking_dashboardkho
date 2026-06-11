// ======================================================================
// 0) CONSTANTS + STATE
// ======================================================================
const API_BASE = "/api/CanDoiDonHangTong";

let _wipPopupInited = false;
let _autoConfirmPending = false;

let _isEdit = false;
let _defaultLineX = null;
let _mobileSizeQtyPopup = null;
let _mobileSizeQtyNumberBox = null;
let _mobileSizeQtyContext = null;

// DVSX cache
let _dvsxList = []; // [{MaDVSX, TenDVSX, GiaCong, ...}]
let _merList = [];
// CongDoan datasource (luôn 1 row)
let _congDoanDs = [{
    ID: 0,
    MaDH: "",
    MaHang: "",
    MaLenhSX: "",
    MaDVSXCat: null,
    MaDVSXMay: null,
    MaDVSXHoanThanh: null,
    MaDVSXDongThung: null,
    NguoiTao: window.CURRENT_USER || "",
    NguoiSua: ""
}];

// ======================================================================
// 1) SAFE INSTANCE HELPERS
// ======================================================================
function safeInstance(sel, widgetName) {
    try { return $(sel)[widgetName]("instance"); } catch { return null; }
}
function isMobileTabletProductionPopup() {
    try {
        return !!(window.matchMedia && window.matchMedia("(max-width: 1199.98px)").matches);
    } catch {
        return window.innerWidth < 1200;
    }
}
function productionGridOptions() {
    const mobileTablet = isMobileTabletProductionPopup();
    return {
        columnAutoWidth: false,
        wordWrapEnabled: false,
        columnFixing: { enabled: !mobileTablet },
        scrolling: { mode: "standard", showScrollbar: "always", useNative: false }
    };
}
function refreshProductionGridLayout() {
    setTimeout(function () {
        ["#gridDonHangContainer", "#gridSanXuatContainer"].forEach(function (sel) {
            const grid = safeInstance(sel, "dxDataGrid");
            if (!grid) return;

            try {
                grid.option(productionGridOptions());
                if (sel === "#gridSanXuatContainer") {
                    grid.option("editing.startEditAction", isMobileTabletProductionPopup() ? "dblClick" : "click");
                }
                grid.updateDimensions();
            } catch { }
        });
    }, 0);
}
(function () {
    window.Popup = window.Popup || {};

    window.Popup.safeInstance = function (sel, widgetName) {
        try { return $(sel)[widgetName]("instance"); }
        catch { return null; }
    };
})();
let _lastNormalLineX = null;

function getDvsxByMa(maDVSX) {
    return (_dvsxList || []).find(x => String(x.MaDVSX) === String(maDVSX));
}

function isGiaCongByMaDVSX(maDVSX) {
    return !!getDvsxByMa(maDVSX)?.GiaCong;
}

// Rule chuẩn theo Win: bám vào công đoạn Cắt
function getLineRuleMaDVSX() {
    const cdRow = getGridData("#gridProcessContainer")[0] || {};
    return String(
        cdRow.MaDVSXCat ||
        safeInstance("#sl_DonViSX", "dxSelectBox")?.option("value") ||
        ""
    ).trim();
}

function applyLineRuleByMaDVSX(maDVSX) {
    const lineSb = safeInstance("#txt_LineX", "dxSelectBox");
    if (!lineSb) return;

    const isGiaCong = isGiaCongByMaDVSX(maDVSX);
    const currentVal = lineSb.option("value");

    if (isGiaCong) {
        if (currentVal) _lastNormalLineX = currentVal;

        lineSb.option({
            value: null,
            disabled: true,
            placeholder: "Gia công - không chọn chuyền"
        });
        return;
    }

    lineSb.option({
        disabled: false,
        placeholder: "Chọn chuyền"
    });

    const restoreVal = _lastNormalLineX || _defaultLineX || null;
    if (!lineSb.option("value") && restoreVal) {
        lineSb.option("value", restoreVal);
    }
}
// ======================================================================
// 2) NOTIFY (Toast inside popup) + fallback to DevExpress.ui.notify
// ======================================================================
let _popupToastInited = false;

function ensurePopupToast() {
    if (!$("#toastPopup").length) {
        $("body").append('<div id="toastPopup"></div>');
    }

    // ✅ init 1 lần dựa theo data("dxToast") cho chắc
    if ($("#toastPopup").data("dxToast")) return;

    $("#toastPopup").dxToast({
        visible: false,
        displayTime: 2000,
        closeOnClick: true,
        container: "body",
        hideOnOutsideClick: false,

        // ✅ neo vào dialog của popup
        position: {
            my: "top center",
            at: "top center",
            of: "#modalProduction .wip-dialog",
            offset: "0 52"   // ✅ xuống dưới header (header cao ~50px)
        },

        animation: {
            show: { type: "fade", duration: 150 },
            hide: { type: "fade", duration: 150 }
        },

        onShowing: function (e) {
            try {
                const $wrap = e.component.$element().closest(".dx-overlay-wrapper");
                // ✅ phải cao hơn modal (modal 99999)
                $wrap.css("z-index", 100200);
            } catch { }
        }
    });
}
function isPopupOpen() {
    return $("#modalProduction").hasClass("is-open");
}

function notify2(message, type = "info", ms = 2000) {
    if (isPopupOpen()) {
        ensurePopupToast();
        const t = $("#toastPopup").dxToast("instance");
        if (!t) return DevExpress.ui.notify(message, type, ms);

        t.option({ message, type, displayTime: ms });
        t.show();
        return;
    }
    DevExpress.ui.notify(message, type, ms);
}
// ======================================================================
// 3) LOADING (dxLoadPanel) on popup
// ======================================================================
let _lpInited = false;

function ensureLoadPanel() {
    if (_lpInited) return;
    _lpInited = true;

    if (!$("#lpModal").length) {
        $("#modalProduction").append('<div id="lpModal"></div>');
    }

    $("#lpModal").dxLoadPanel({
        container: "#modalProduction",      
        position: { of: "#modalProduction" },    
        shading: true,
        showIndicator: true,
        showPane: true,
        hideOnOutsideClick: false,
        visible: false,
        message: "Đang xử lý...",
        // ✅ ép zIndex cao (phòng trường hợp wrapper bị tính thấp)
        onShowing: function (e) {
            try {
                const $wrap = e.component.$element().closest(".dx-overlay-wrapper");
                $wrap.css("z-index", 20050);
            } catch { }
        }
    });
}

function setLoading(visible, message) {
    ensureLoadPanel();
    const lp = $("#lpModal").dxLoadPanel("instance");
    if (!lp) return;

    if (message != null) lp.option("message", message);
    lp.option("visible", !!visible);
}

async function withLoading(message, fn) {
    setLoading(true, message || "Đang xử lý...");
    try {
        return await fn();
    } finally {
        setLoading(false);
    }
}

// ======================================================================
// 4) API HELPERS
// ======================================================================
function apiGetJson(url) {
    return $.ajax({ url, method: "GET", dataType: "json" });
}

async function apiPostJson(url, body) {
    return $.ajax({
        url,
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(body),
        dataType: "text" // backend đang trả string "true"/"false"/message
    });
}

function isTrueResponse(x) {
    if (x === true) return true;
    if (x == null) return false;

    let s = String(x).trim();
    s = s.replace(/^"+|"+$/g, "").trim(); // bỏ quote ngoài nếu có
    return s.toLowerCase() === "true";
}

// ======================================================================
// 5) POPUP INIT + OPEN/CLOSE
// ======================================================================
function ensurePopupInited() {
    if (_wipPopupInited) return;
    initDevExpressControls(); // init widgets + bind events (1 lần)
    _wipPopupInited = true;
}

function setPopupOrderInfo(maDH, maHang) {
    // Keep popup order info readonly and in sync with WIP.
    const maDhInst = safeInstance("#txt_MaDH", "dxTextBox");
    const maHangInst = safeInstance("#txt_MaHang", "dxTextBox");

    if (maDhInst) maDhInst.option("value", maDH || "");
    if (maHangInst) maHangInst.option("value", maHang || "");
}

async function openProductionPopup(maDH, lineX, maHang) {
    const $m = $("#modalProduction");
    if (!$m.length) return;

    ensurePopupInited();
    _defaultLineX = lineX;

    $("html,body").addClass("modal-open");
    $m.addClass("is-open").attr("aria-hidden", "false");
    refreshProductionGridLayout();

    setPopupOrderInfo(maDH, maHang);

    // 1) Load toolbar: MaSX/Dot + DVSX + Lines
    await onLayDuLieuClick();

    // 2) Load PO -> InSeam -> Mau -> Size (+ auto confirm nếu đủ)
    await loadPOByMaDH(maDH || "");
}

function closeProductionPopup() {
    $("#modalProduction").removeClass("is-open").attr("aria-hidden", "true");
    $("html,body").removeClass("modal-open");
}

// ======================================================================
// 6) CONG DOAN (1 row)
// ======================================================================
function initDataCongDoan(maDVSXInit) {
    const dvsx = (_dvsxList || []).find(x => String(x.MaDVSX) === String(maDVSXInit));
    const isGiaCong = !!dvsx?.GiaCong;

    return [{
        ID: 0,
        MaDH: "",
        MaHang: "",
        MaLenhSX: "",
        MaDVSXCat: maDVSXInit || null,
        MaDVSXMay: maDVSXInit || null,
        MaDVSXHoanThanh: isGiaCong ? null : (maDVSXInit || null),
        MaDVSXDongThung: isGiaCong ? null : (maDVSXInit || null),
        NguoiTao: window.CURRENT_USER || "",
        NguoiSua: ""
    }];
}

function fillGridCongDoanByDVSX(maDVSXInit) {
    const grid = safeInstance("#gridProcessContainer", "dxDataGrid");
    if (!grid) return;

    const ds = initDataCongDoan(maDVSXInit);
    _congDoanDs = ds;
    grid.option("dataSource", ds);
    grid.option("focusedRowIndex", 0);
}

// ======================================================================
// 7) LOAD CHAIN: PO -> InSeam -> Mau -> Size (+ auto confirm)
// ======================================================================
async function loadPOByMaDH(maDH) {
    ensurePopupInited();

    const poTag = safeInstance("#sl_PO", "dxTagBox");
    const inseamTag = safeInstance("#sl_InSeam", "dxTagBox");
    if (!poTag || !inseamTag) return;

    poTag.option({ dataSource: [], value: [] });
    inseamTag.option({ dataSource: [], value: [] });

    if (!maDH) return;

    const url = `${API_BASE}/GetPO?madh=${encodeURIComponent(maDH)}`;
    const dtPO = await apiGetJson(url);

    poTag.option("dataSource", dtPO || []);

    // default chọn hết
    const allPOIDs = (dtPO || []).map(x => String(x.POID));
    setValueSilent(poTag, allPOIDs, "po");
    await loadInSeamByMaDHAndPOs(maDH, allPOIDs); 
}

async function loadInSeamByMaDHAndPOs(maDH, poids) {
    const inseamTag = safeInstance("#sl_InSeam", "dxTagBox");
    if (!inseamTag) return;

    inseamTag.option({ dataSource: [], value: [] });

    if (!maDH || !poids?.length) return;

    const poidStr = poids.join(";");
    const url = `${API_BASE}/GetDauSize?madh=${encodeURIComponent(maDH)}&poid=${encodeURIComponent(poidStr)}`;
    const dtDauSize = await apiGetJson(url);

    inseamTag.option({
        dataSource: dtDauSize || [],
        valueExpr: "DauSizeID",
        displayExpr: "DauSize"
    });

    // default chọn hết (✅ giữ 1 lần - bỏ duplicate)
    const allInSeams = (dtDauSize || []).map(x => String(x.DauSizeID));
    inseamTag.option("value", allInSeams);

    await loadMauByMaDHAndInSeams(maDH, allInSeams);
}

async function loadMauByMaDHAndInSeams(maDH, dauSizeIds) {
    const mauTag = safeInstance("#sl_Mau", "dxTagBox");
    const sizeTag = safeInstance("#sl_Size", "dxTagBox");
    if (!mauTag || !sizeTag) return;

    mauTag.option({ dataSource: [], value: [] });
    sizeTag.option({ dataSource: [], value: [] });

    if (!maDH || !dauSizeIds?.length) return;

    const dauSizeStr = dauSizeIds.join(";");
    const url = `${API_BASE}/GetMau?madh=${encodeURIComponent(maDH)}&dausizeID=${encodeURIComponent(dauSizeStr)}`;
    const dtMau = await apiGetJson(url);

    mauTag.option({
        dataSource: dtMau || [],
        valueExpr: "MaMau",
        displayExpr: "TenMau"
    });

    // default chọn hết màu
    const allMau = (dtMau || []).map(x => String(x.MaMau));
    mauTag.option("value", allMau);

    await loadSizeByMaDH_InSeam_Mau(maDH, dauSizeIds, allMau);
}

async function loadSizeByMaDH_InSeam_Mau(maDH, dauSizeIds, mauIds) {
    const sizeTag = safeInstance("#sl_Size", "dxTagBox");
    if (!sizeTag) return;

    sizeTag.option({ dataSource: [], value: [] });

    if (!maDH || !dauSizeIds?.length || !mauIds?.length) return;

    const dauSizeStr = dauSizeIds.join(";");
    const mauStr = mauIds.join(";");

    const url = `${API_BASE}/GetSizeID?madh=${encodeURIComponent(maDH)}&dausizeID=${encodeURIComponent(dauSizeStr)}&mamau=${encodeURIComponent(mauStr)}`;
    const dtSize = await apiGetJson(url);

    sizeTag.option({
        dataSource: dtSize || [],
        valueExpr: "SizeID",
        displayExpr: "Size"
    });

    // default chọn hết size
    sizeTag.option("value", (dtSize || []).map(x => String(x.SizeID)));

    await tryAutoConfirm();
}

// Auto Confirm chỉ chạy khi đủ MaDH + PO + InSeam + Mau + Size
async function tryAutoConfirm() {
    if (_autoConfirmPending) return;

    const maInst = safeInstance("#txt_MaDH", "dxTextBox");
    const poInst = safeInstance("#sl_PO", "dxTagBox");
    const inInst = safeInstance("#sl_InSeam", "dxTagBox");
    const mauInst = safeInstance("#sl_Mau", "dxTagBox");
    const szInst = safeInstance("#sl_Size", "dxTagBox");
    if (!maInst || !poInst || !inInst || !mauInst || !szInst) return;

    const maDH = maInst.option("value");
    const poids = poInst.option("value") || [];
    const inseams = inInst.option("value") || [];
    const maus = mauInst.option("value") || [];
    const sizes = szInst.option("value") || [];

    const ok = maDH && poids.length && inseams.length && maus.length && sizes.length;
    if (!ok) return;

    _autoConfirmPending = true;
    try {
        await onConfirmClick();
    } finally {
        _autoConfirmPending = false;
    }
}

// ======================================================================
// 8) TOOLBAR LOAD: build lenhSX + load DVSX + load Lines
// ======================================================================
async function getMaxLenhSX() {
    const url = `/api/ERPDonHangTong/Get?action=GetMaxLenh`;
    const dt = await apiGetJson(url);
    return dt?.[0]?.LenhSX ?? "";
}

async function buildNewLenhSX() {
    const max = await apiGetJson(`${API_BASE}/GetMaxDH`);
    const maxNum = Number(max || 0) || 0;

    const lenhSX = maxNum <= 0 ? "SX_1" : `SX_${maxNum + 1}`;
    const lenhSXnewVal = await getMaxLenhSX();
    const lenhSXnew = String(lenhSXnewVal);

    const dot = `${lenhSX}-DOT-${new Date().getDate()}`;

    $("#txt_MaSX").val(lenhSXnew);
    $("#txt_DotSX").val(dot);

    return { lenhSXnew, dot };
}

async function LoadMer() {
    const merSb = safeInstance("#sl_Mer", "dxSelectBox");
    if (!merSb) return;

    merSb.option({ dataSource: [], value: null });

    const url = "/api/wip-donhang/get-md";
    const dt = await apiGetJson(url);

    _merList = (dt || [])
        .map(x => ({
            UserID: String(x?.UserID ?? "").trim(),
            Ten: String(x?.Ten ?? "").trim()
        }))
        .filter(x => x.UserID && x.Ten);

    merSb.option("dataSource", _merList);
}

async function loadDonViSanXuat() {
    const dvsxSb = safeInstance("#sl_DonViSX", "dxSelectBox");
    if (!dvsxSb) return;

    dvsxSb.option({ dataSource: [], value: null });

    const url = `/api/DonViSanXuat/GetDonViSanXuat`;
    const dt = await apiGetJson(url);

    _dvsxList = (dt || []).slice().sort((a, b) => (a.sort ?? 0) - (b.sort ?? 0));
    dvsxSb.option("dataSource", _dvsxList);

    if (_dvsxList.length) {
        const defaultVal = String(_dvsxList[0].MaDVSX);
        dvsxSb.option("value", defaultVal);
        fillGridCongDoanByDVSX(defaultVal);
    }
}

async function loadLinesAndDefault(defaultInput) {
    const lineSb = safeInstance("#txt_LineX", "dxSelectBox");
    if (!lineSb) return;

    const dt = await apiGetJson(`/api/ERPDonHangTong/Get?action=GetLine`);
    const ds = [
        { Line: null, Name: "[Không chọn]" },
        ...(dt || [])
    ];

    lineSb.option("dataSource", ds);

    if (defaultInput == null || defaultInput === "") {
        lineSb.option("value", "");
        return;
    }

    const input = String(defaultInput).trim();
    const found = (dt || []).find(x => String(x.Name).trim() === input);
    const valueToSet = found ? String(found.Line).trim() : input;

    lineSb.option("value", valueToSet);
}

async function onLayDuLieuClick() {
    return await withLoading("Đang lấy dữ liệu...", async () => {
        try {
            ensurePopupInited();

            if (!_isEdit) {
                await buildNewLenhSX();
            }

            await loadDonViSanXuat();
            await loadLinesAndDefault(_defaultLineX);
            await LoadMer();

            applyLineRuleByMaDVSX(getLineRuleMaDVSX());

            const pendingCross = window.WIP?.cross?.pending;
            const ghiChu = pendingCross?.ghiChu ?? "";

            $(".wip-toolbar-note input").val(ghiChu);
            notify2("Xong.", "success", 900);
            return true;
        } catch (e) {
            console.error(e);
            notify2("Lỗi lấy dữ liệu.", "error", 1500);
            return false;
        }
    });
}

// ======================================================================
// 9) CONFIRM FLOW (Build dynamic columns + pivot)
// ======================================================================
async function onConfirmClick() {
    // Loading bọc bên ngoài để tránh notify bị che / spam
    return await withLoading("Đang tải dữ liệu...", async () => {
        const maDH = safeInstance("#txt_MaDH", "dxTextBox")?.option("value") || "";
        const isAll = $("#chk_AllData").is(":checked");

        const poids = safeInstance("#sl_PO", "dxTagBox")?.option("value") || [];
        const dauSizeIds = safeInstance("#sl_InSeam", "dxTagBox")?.option("value") || [];
        const mauIds = safeInstance("#sl_Mau", "dxTagBox")?.option("value") || [];
        const sizeIdsSelected = safeInstance("#sl_Size", "dxTagBox")?.option("value") || [];

        // Validate giống Win
        if (!isAll) {
            if (!maDH) { notify2("Vui lòng chọn đơn hàng!", "warning", 1800); return false; }
            if (!poids.length) { notify2("Vui lòng chọn PO!", "warning", 1800); return false; }
            if (!dauSizeIds.length) { notify2("Vui lòng chọn nhóm size!", "warning", 1800); return false; }
            if (!mauIds.length) { notify2("Vui lòng chọn màu!", "warning", 1800); return false; }
        }

        const poidStr = poids.map(String).join(";");
        const dauSizeStr = dauSizeIds.map(String).join(";");
        const mauStr = mauIds.map(String).join(";");

        // 1) GetListSize
        const urlListSize = `${API_BASE}/GetListSize?madh=${encodeURIComponent(maDH)}`;
        let dtSizeID = await apiGetJson(urlListSize);

        // 2) GetSoLuongDH
        const urlSLDH =
            `${API_BASE}/GetSoLuongDH?madh=${encodeURIComponent(maDH)}&dausizeid=${encodeURIComponent(dauSizeStr)}&mamau=${encodeURIComponent(mauStr)}&poid=${encodeURIComponent(poidStr)}`;
        let dtData = await apiGetJson(urlSLDH);

        // 3) Filter theo size user chọn
        if (!isAll && sizeIdsSelected.length) {
            const setSize = new Set(sizeIdsSelected.map(String));
            dtSizeID = (dtSizeID || []).filter(x => setSize.has(String(x.SizeID)));
            dtData = (dtData || []).filter(x => setSize.has(String(x.SizeID)));
        }

        // 4) Build columns động
        //const SIZE_COL_W = 60;
        //const sizeCols = (dtSizeID || []).map(s => ({
        //    caption: s.Size,
        //    dataField: `${s.SizeID}@Size@${s.Size}`,
        //    alignment: "center",
        //    width: SIZE_COL_W,
        //    minWidth: SIZE_COL_W,
        //    maxWidth: SIZE_COL_W,
        //    customizeText: function (e) {
        //        const v = Number(e.value ?? 0) || 0;
        //        return v === 0 ? "-" : String(v);
        //    }
        //}));
        const mobileTablet = isMobileTabletProductionPopup();
        const fixedOnDesktop = !mobileTablet;
        const sizeCols = (dtSizeID || []).map(s => ({
            caption: s.Size,
            dataField: `${s.SizeID}@Size@${s.Size}`,
            alignment: "center",
            width: mobileTablet ? 60 : undefined,
            customizeText(e) {
                const v = Number(e.value ?? 0) || 0;
                return v === 0 ? "-" : String(v);
            }
        }));

        const baseCols = [
            { caption: "Đơn hàng", dataField: "MaDH", fixed: fixedOnDesktop, fixedPosition: fixedOnDesktop ? "left" : undefined, width: mobileTablet ? 80 : 90 },
            { caption: "Màu", dataField: "TenMau", fixed: fixedOnDesktop, fixedPosition: fixedOnDesktop ? "left" : undefined, width: mobileTablet ? 90 : 120 },
            { caption: "PO", dataField: "PO", fixed: fixedOnDesktop, fixedPosition: fixedOnDesktop ? "left" : undefined, width: mobileTablet ? 75 : 90 },
            { caption: "Nhóm size", dataField: "DauSize", fixed: fixedOnDesktop, fixedPosition: fixedOnDesktop ? "left" : undefined, width: mobileTablet ? 70 : 80 },
        ];

        const totalCol = {
            caption: "Tổng",
            dataField: "Amount",
            fixed: fixedOnDesktop,
            fixedPosition: fixedOnDesktop ? "right" : undefined,
            width: mobileTablet ? 60 : 70,
            alignment: "center",
            allowEditing: false
        };

        const map = new Map();

        for (const r of (dtData || [])) {
            const key = `${r.MaDH}|${r.POID}|${r.MaMau}|${r.DauSizeID}`;

            if (!map.has(key)) {
                map.set(key, {
                    MaDH: r.MaDH,
                    MaHang: r.MaHang,
                    POID: r.POID,
                    PO: r.PO,
                    MaQG: r.MaQG,
                    MaMau: r.MaMau,
                    TenMau: r.TenMau,
                    DauSizeID: r.DauSizeID,
                    DauSize: r.DauSize,
                    Amount: 0
                });
            }

            const row = map.get(key);
            const val = (r.SoLuong == null || r.SoLuong === "") ? 0 : Number(r.SoLuong);
            const colKey = `${r.SizeID}@Size@${r.Size}`;
            row[colKey] = val;
        }

        const pivotRows = Array.from(map.values());
        const sizeKeys = (dtSizeID || []).map(s => `${s.SizeID}@Size@${s.Size}`);

        for (const row of pivotRows) {
            let sum = 0;
            for (const k of sizeKeys) {
                sum += Number(row[k] ?? 0) || 0;
            }
            row.Amount = sum;
        }
        // 6) Apply lên grid top + bottom
        const gridTop = safeInstance("#gridDonHangContainer", "dxDataGrid");
        const gridBottom = safeInstance("#gridSanXuatContainer", "dxDataGrid");
        if (!gridTop || !gridBottom) return false;

        gridTop.option(productionGridOptions());
        gridTop.option("columns", [...baseCols, ...sizeCols, totalCol]);
        gridTop.option("dataSource", pivotRows);

        gridBottom.option($.extend(true, {}, productionGridOptions(), {
            columns: [...baseCols, ...sizeCols, totalCol],
            dataSource: pivotRows
        }));
        refreshProductionGridLayout();
        window._baselineQtyMap = buildBaselineQtyMap(pivotRows);
        // update footer lần đầu
        updateBottomFooter(pivotRows, window._baselineQtyMap);
        const totalTop = pivotRows.reduce((s, x) => s + (Number(x.Amount) || 0), 0);
        $("#lbl_TotalTop").text(totalTop);

        return true;
    });
}

// ======================================================================
// 10) SAVE FLOW HELPERS
// ======================================================================
let _silent = { po: false, inseam: false, mau: false, size: false };

function setValueSilent(inst, value, key) {
    _silent[key] = true;
    inst.option("value", value);
    _silent[key] = false;
}
function parseNumberText(sel) {
    const t = $(sel).text() || "0";
    return Number(String(t).replaceAll(",", "").trim()) || 0;
}
async function flushGridEdits() {
    const gBottom = safeInstance("#gridSanXuatContainer", "dxDataGrid");
    const gProc = safeInstance("#gridProcessContainer", "dxDataGrid");

    // commit cell đang edit của grid bottom
    if (gBottom) {
        try { await gBottom.saveEditData(); } catch { }
        try { gBottom.closeEditCell(); } catch { }
    }

    // commit grid công đoạn (nếu đang edit)
    if (gProc) {
        try { await gProc.saveEditData(); } catch { }
        try { gProc.closeEditCell(); } catch { }
    }

    // sau commit thì footer tính lại
    updateBottomFooter();
}
function rowKey(r) {
    return `${r.MaDH}|${r.POID}|${r.MaMau}|${r.DauSizeID}`;
}

function buildBaselineQtyMap(rows) {
    const map = new Map();
    for (const r of (rows || [])) {
        const key = rowKey(r);
        const sizeCols = getSizeColumnsFromRow(r);
        const base = { Amount: Number(r.Amount ?? 0) || 0 };
        for (const f of sizeCols) base[f] = Number(r[f] ?? 0) || 0;
        map.set(key, base);
    }
    return map;
}

function calcAmountFromRow(r) {
    const sizeCols = getSizeColumnsFromRow(r);
    let sum = 0;
    for (const f of sizeCols) sum += (Number(r[f] ?? 0) || 0);
    return sum;
}

function getGridBottomRows() {
    return getGridData("#gridSanXuatContainer"); // ✅ reuse hàm của bạn
}

function updateBottomFooter() {
    const rows = getGridBottomRows();

    const curTotal = rows.reduce((s, r) => s + (Number(r.Amount ?? 0) || 0), 0);

    let baseTotal = 0;
    const bm = window._baselineQtyMap;
    if (bm) for (const v of bm.values()) baseTotal += (Number(v.Amount ?? 0) || 0);

    const remain = baseTotal - curTotal;

    $("#lbl_SLTH").text(curTotal);
    $("#lbl_Remain").text(remain < 0 ? 0 : remain);
}

function parseLenhSXInt(v) {
    // hỗ trợ "427", "SX_427"
    const s = String(v ?? "").trim();
    const m = s.match(/(\d+)/);
    return m ? (parseInt(m[1], 10) || 0) : 0;
}
function getGridData(gridId) {
    const grid = safeInstance(gridId, "dxDataGrid");
    if (!grid) return [];
    const ds = grid.option("dataSource");
    if (Array.isArray(ds)) return ds;
    try {
        const items = grid.getDataSource()?.items?.();
        return Array.isArray(items) ? items : [];
    } catch {
        return [];
    }
}

function getSizeColumnsFromRow(rowObj) {
    return Object.keys(rowObj || {}).filter(k => k.includes("@Size@"));
}

function parseSizeColKey(colKey) {
    const parts = String(colKey).split("@Size@");
    return {
        SizeID: String(parts[0] ?? "").trim(),
        Size: String(parts[1] ?? "").trim()
    };
}

function ensureMobileSizeQtyPopup() {
    if (_mobileSizeQtyPopup) return _mobileSizeQtyPopup;

    if (!$("#popupMobileSizeQty").length) {
        $("body").append('<div id="popupMobileSizeQty"></div>');
    }

    _mobileSizeQtyPopup = $("#popupMobileSizeQty").dxPopup({
        title: "Sửa số lượng",
        width: "92vw",
        maxWidth: 360,
        height: "auto",
        showTitle: true,
        dragEnabled: false,
        hideOnOutsideClick: true,
        container: "#modalProduction",
        position: { of: "#modalProduction" },
        contentTemplate: function (content) {
            const $wrap = $("<div>").css({ padding: "8px 4px 2px" }).appendTo(content);
            $("<div id='mobileSizeQtyInfo'></div>")
                .css({ marginBottom: "10px", fontWeight: 600, fontSize: "13px" })
                .appendTo($wrap);
            $("<div id='mobileSizeQtyNumber'></div>").appendTo($wrap);
        },
        onShown: function () {
            if (!_mobileSizeQtyNumberBox) {
                _mobileSizeQtyNumberBox = $("#mobileSizeQtyNumber").dxNumberBox({
                    min: 0,
                    step: 1,
                    showSpinButtons: true,
                    inputAttr: { inputmode: "numeric" }
                }).dxNumberBox("instance");
            }

            const ctx = _mobileSizeQtyContext || {};
            $("#mobileSizeQtyInfo").text(ctx.label || "");
            _mobileSizeQtyNumberBox.option("value", Number(ctx.value ?? 0) || 0);
            setTimeout(function () {
                try { _mobileSizeQtyNumberBox.focus(); } catch { }
            }, 0);
        },
        onShowing: function (e) {
            try {
                const $wrap = e.component.$element().closest(".dx-overlay-wrapper");
                $wrap.css("z-index", 100500);
            } catch { }
        },
        toolbarItems: [
            {
                widget: "dxButton",
                toolbar: "bottom",
                location: "after",
                options: {
                    text: "Lưu",
                    type: "success",
                    onClick: saveMobileSizeQty
                }
            },
            {
                widget: "dxButton",
                toolbar: "bottom",
                location: "after",
                options: {
                    text: "Hủy",
                    onClick: function () { _mobileSizeQtyPopup.hide(); }
                }
            }
        ]
    }).dxPopup("instance");

    return _mobileSizeQtyPopup;
}

function openMobileSizeQtyPopup(e) {
    if (!isMobileTabletProductionPopup()) return false;
    if (!e || e.rowType !== "data" || !e.data) return false;

    const field = String(e.column && e.column.dataField || "");
    if (!field.includes("@Size@")) return false;

    try { e.event && e.event.preventDefault && e.event.preventDefault(); } catch { }
    try { e.event && e.event.stopPropagation && e.event.stopPropagation(); } catch { }

    const grid = e.component || safeInstance("#gridSanXuatContainer", "dxDataGrid");
    try { grid && grid.closeEditCell(); } catch { }

    const sizeInfo = parseSizeColKey(field);
    const base = window._baselineQtyMap ? window._baselineQtyMap.get(rowKey(e.data)) : null;
    const max = base ? (Number(base[field] ?? 0) || 0) : null;
    const maxText = max != null ? ` / SL gốc: ${max}` : "";

    _mobileSizeQtyContext = {
        grid,
        rowIndex: e.rowIndex,
        rowData: e.data,
        field,
        value: Number(e.data[field] ?? 0) || 0,
        max,
        label: `${e.data.MaDH || ""} - Size: ${sizeInfo.Size || field}${maxText}`
    };

    ensureMobileSizeQtyPopup().show();
    return true;
}

function saveMobileSizeQty() {
    const ctx = _mobileSizeQtyContext;
    if (!ctx || !ctx.grid || !ctx.rowData || !ctx.field) return;

    let value = Number(_mobileSizeQtyNumberBox ? _mobileSizeQtyNumberBox.option("value") : 0);
    if (!Number.isFinite(value) || value < 0) value = 0;

    if (ctx.max != null && value > ctx.max) {
        const sizeInfo = parseSizeColKey(ctx.field);
        value = ctx.max;
        notify2(`Size ${sizeInfo.Size}: không được vượt quá số lượng gốc (${ctx.max}).`, "warning", 2200);
    }

    ctx.rowData[ctx.field] = value;
    ctx.rowData.Amount = calcAmountFromRow(ctx.rowData);

    try { ctx.grid.cellValue(ctx.rowIndex, ctx.field, value); } catch { }
    try { ctx.grid.cellValue(ctx.rowIndex, "Amount", ctx.rowData.Amount); } catch { }
    try { ctx.grid.refresh(); } catch { }

    updateBottomFooter();
    _mobileSizeQtyPopup.hide();
}

function validateBeforeSave() {
    const dotSX = $("#txt_DotSX").val();
    if (!dotSX) {
        notify2("Vui lòng nhập Đợt SX trước khi lưu!", "warning", 2000);
        return false;
    }

    const dvsx = safeInstance("#sl_DonViSX", "dxSelectBox")?.option("value");
    if (!dvsx) {
        notify2("Vui lòng chọn Đơn vị sản xuất (DVSX) trước khi lưu!", "warning", 2000);
        return false;
    }

    const lineX = safeInstance("#txt_LineX", "dxSelectBox")?.option("value");
    const cdRow = getGridData("#gridProcessContainer")[0] || null;

    if (!cdRow) {
        notify2("Không có dữ liệu công đoạn!", "warning", 2000);
        return false;
    }

    const maDVSXCat = String(cdRow.MaDVSXCat || dvsx || "").trim();
    const isGiaCongCat = isGiaCongByMaDVSX(maDVSXCat);

    if (isGiaCongCat && lineX) {
        notify2("Đơn vị Cắt là Gia Công nên không được chọn Chuyền (Line).", "warning", 2200);
        return false;
    }

    if (!isGiaCongCat && !lineX) {
        notify2("Vui lòng chọn Chuyền (Line) trước khi lưu!", "warning", 2000);
        return false;
    }

    const needAll4 = ["MaDVSXCat", "MaDVSXMay", "MaDVSXHoanThanh", "MaDVSXDongThung"];
    for (const f of needAll4) {
        const v = cdRow[f];

        if (!v) {
            notify2("Vui lòng chọn đầy đủ Đơn vị sản xuất cho các công đoạn!", "warning", 2500);
            return false;
        }
    }

    if (cdRow.MaDVSXHoanThanh) {
        const hoanThanhObj = (_dvsxList || []).find(x => String(x.MaDVSX) === String(cdRow.MaDVSXHoanThanh));
        if (hoanThanhObj?.GiaCong) {
            notify2("Công đoạn Hoàn thành không được chọn đơn vị Gia Công. Vui lòng chọn lại!", "warning", 2600);
            return false;
        }
    }

    return true;
}

async function resolveSoId(madh, mahangFirst) {
    const kt = await apiGetJson(`${API_BASE}/GetKiemTra?madh=${encodeURIComponent(madh || "")}`);
    const soIdFromKT = kt?.[0]?.SoID;
    if (soIdFromKT != null && String(soIdFromKT) !== "") return Number(soIdFromKT) || 0;

    const gop = await apiGetJson(`${API_BASE}/GetGopID?madh=${encodeURIComponent(mahangFirst || "")}`);
    const soIdFromGop = gop?.[0]?.SoID;
    if (soIdFromGop == null || String(soIdFromGop) === "") return 1;
    return (Number(soIdFromGop) || 0) + 1;
}

async function checkDuplicateDotAndDvsx(madh, poidStr, dotSX, maDvsx) {
    const url = `${API_BASE}/GetLenhSX?madh=${encodeURIComponent(madh || "")}&poid=${encodeURIComponent(poidStr || "")}`;
    const dt = await apiGetJson(url);
    if (!Array.isArray(dt) || !dt.length) return false;

    return dt.some(x =>
        String(x.DotSX || "").trim() === String(dotSX || "").trim() &&
        String(x.MaDVSX || "").trim() === String(maDvsx || "").trim()
    );
}

function buildGopDhTableFromGridRows(gridRows, soId, gopDhText) {
    const map = new Map();
    for (const r of (gridRows || [])) {
        const maDH = String(r.MaDH || "").trim();
        const maHang = String(r.MaHang || "").trim();
        if (!maDH || !maHang) continue;

        const key = `${maDH}||${maHang}`;
        if (map.has(key)) continue;

        map.set(key, {
            ID: 0,
            MaGop: `${maHang}|${soId}`,
            GopDH: gopDhText || maDH,
            MaDH: maDH,
            MaHang: maHang,
            SoID: soId
        });
    }
    return Array.from(map.values());
}

function buildDvsxSaveList(gridRows, ctx) {
    const list = [];

    for (const r of (gridRows || [])) {
        const maDH = String(r.MaDH || "").trim();
        const maHang = String(r.MaHang || "").trim();

        const poid = String(r.POID || "").trim();
        const po = String(r.PO || "").trim();
        const maQG = String(r.MaQG || "").trim();
        const maMau = String(r.MaMau || "").trim();
        const dauSizeID = String(r.DauSizeID || "").trim();
        const dauSize = String(r.DauSize || "").trim();

        const sizeCols = getSizeColumnsFromRow(r);

        for (const colKey of sizeCols) {
            const qty = Number(r[colKey] ?? 0) || 0;
            if (qty <= 0) continue;

            const sz = parseSizeColKey(colKey);

            list.push({
                ID: 0,
                MaLenhSanXuat: ctx.maLenhSanXuat,
                MaLenh: ctx.maLenh,
                TenLenh: ctx.tenLenh,
                MaDH: maDH,
                MaDVSX: ctx.maDvsx,
                DotSX: ctx.dotSX,
                MaQG: maQG,
                POID: poid,
                PO: po,
                MaMau: maMau,
                DauSizeID: dauSizeID,
                DauSize: dauSize,
                SizeID: sz.SizeID,
                Size: sz.Size,
                SoLuong: qty,
                TrangThai: 1,
                STTLenh: ctx.sttLenh,
                MaGop: ctx.maGop,
                POID_T: String(po).replace(/[\s!@#$%^&*()\-+=\[\]{};:'"\\|,.<>/?]+/g, "_"),
                MaCu: ctx.maCu,
                Line: ctx.line,
                GhiChu: ctx.ghiChu
            });
        }
    }

    return list;
}

function buildChiTietCongDoanRow(gridRowCongDoan, ctx) {
    const userName = localStorage.getItem("username") || localStorage.getItem("username1") || "";
    return {
        ID: 0,
        MaDH: ctx.magop,
        MaHang: ctx.mahangFirst,
        MaLenhSX: ctx.maLenhSanXuat,

        MaDVSXCat: gridRowCongDoan.MaDVSXCat || null,
        MaDVSXMay: gridRowCongDoan.MaDVSXMay || null,
        MaDVSXHoanThanh: gridRowCongDoan.MaDVSXHoanThanh ?? null,

        NguoiTao: userName,
        NguoiSua: "",
        MaDVSXDongThung: gridRowCongDoan.MaDVSXDongThung ?? null,
    };
}
function splitMaDHJoined(maDHJoined) {
    return String(maDHJoined || "")
        .split(";")
        .map(s => s.trim())
        .filter(Boolean);
}

function buildMaDHJoinedFromRows(rows) {
    const arr = (rows || [])
        .map(r => String(r?.MaDH || "").trim())
        .filter(Boolean);
    return Array.from(new Set(arr)).join(";");
}

// ✅ Win: GetLenhSX chỉ check theo từng MaDH => multi thì loop từng dh
async function checkDuplicateDotAndDvsx_multi(maDHJoinedOrList, poidStr, dotSX, maDvsx) {
    const list = Array.isArray(maDHJoinedOrList)
        ? maDHJoinedOrList.map(x => String(x).trim()).filter(Boolean)
        : splitMaDHJoined(maDHJoinedOrList);

    if (!list.length) return false;

    for (const dh of list) {
        const url = `${API_BASE}/GetLenhSX?madh=${encodeURIComponent(dh)}&poid=${encodeURIComponent(poidStr || "")}`;
        const dt = await apiGetJson(url);
        if (!Array.isArray(dt) || !dt.length) continue;

        const dup = dt.some(x =>
            String(x.DotSX || "").trim() === String(dotSX || "").trim() &&
            String(x.MaDVSX || "").trim() === String(maDvsx || "").trim()
        );
        if (dup) return true;
    }
    return false;
}
function escapeHtml(s) {
    return String(s ?? "").replace(/[&<>"']/g, c => ({
        "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
    }[c]));
}

async function confirmInProductionModal(message, title) {
    // ✅ quan trọng: tắt loadpanel trước để khỏi che popup
    try { setLoading(false); } catch { }

    const dlg = DevExpress.ui.dialog.custom({
        title: title || "Cảnh báo",
        messageHtml: `<div style="white-space:pre-wrap; line-height:1.4">${escapeHtml(message)}</div>`,
        buttons: [
            { text: "Không", onClick: () => false },
            { text: "Có", type: "default", onClick: () => true }
        ],
        popupOptions: {
            container: "#modalProduction",                 // ✅ nằm trong modal của bạn
            position: { of: "#modalProduction .wip-dialog" },
            onShowing: function (e) {
                try {
                    const $wrap = e.component.$element().closest(".dx-overlay-wrapper");
                    $wrap.css("z-index", 100500);           // ✅ cao hơn modal 99999
                } catch { }
            }
        }
    });

    return await dlg.show(); // => boolean
}
async function checkSizeBOM_web(maDHJoined) {
    const rows = getGridData("#gridSanXuatContainer");
    if (!rows || !rows.length) return true;

    const urlBOM = `/api/KhoiTaoBOMV1/Get?action=GETCHECKSIZECHIASX&para1=${encodeURIComponent(String(maDHJoined || "").trim())}`;
    const tblSizeBOM = await apiGetJson(urlBOM);

    if (!Array.isArray(tblSizeBOM) || tblSizeBOM.length === 0) return true;

    // ===== 1) Build sizesInDtByNhom từ GRID (theo DauSizeID) =====
    const sizesInDtByNhom = new Map(); // DauSizeID -> Map(SizeID -> SizeName)

    for (const r of rows) {
        const nhom = String(r.DauSizeID || "").trim();
        if (!nhom) continue;

        if (!sizesInDtByNhom.has(nhom)) sizesInDtByNhom.set(nhom, new Map());
        const dtMap = sizesInDtByNhom.get(nhom);

        const sizeCols = getSizeColumnsFromRow(r); // field includes "@Size@" 
        for (const colKey of sizeCols) {
            const qty = Number(r[colKey] ?? 0) || 0;
            if (qty <= 0) continue;

            const sz = parseSizeColKey(colKey); // {SizeID, Size}
            if (!sz.SizeID) continue;

            if (!dtMap.has(sz.SizeID)) dtMap.set(sz.SizeID, sz.Size || sz.SizeID);
        }
    }

    // ===== 2) Build sizesInBOMByNhom từ tblSizeBOM =====
    const sizesInBOMByNhom = new Map(); // MaNhomSize -> Map(MaSize -> TenSize)
    const nhomSizeNames = new Map();    // MaNhomSize -> NhomSize

    for (const row of tblSizeBOM) {
        const maNhom = String(row.MaNhomSize || "").trim();
        const nhomName = String(row.NhomSize || "").trim();
        const maSize = String(row.MaSize || "").trim();
        const tenSize = String(row.TenSize || "").trim();

        if (!maNhom || !maSize) continue;

        if (!nhomSizeNames.has(maNhom)) nhomSizeNames.set(maNhom, nhomName || maNhom);
        if (!sizesInBOMByNhom.has(maNhom)) sizesInBOMByNhom.set(maNhom, new Map());

        const bomMap = sizesInBOMByNhom.get(maNhom);
        if (!bomMap.has(maSize)) bomMap.set(maSize, tenSize || maSize);
    }

    // ===== 3) Compare giống Win =====
    const errors = [];

    // 3.1 _dt có nhom nhưng BOM không có
    for (const [maNhom, dtMap] of sizesInDtByNhom.entries()) {
        if (!sizesInBOMByNhom.has(maNhom)) {
            const sizeList = Array.from(dtMap.values()).join(", ");
            errors.push(`Nhóm size '${maNhom}' không tồn tại trong BOM. Các size bị ảnh hưởng: ${sizeList}`);
            continue;
        }

        const bomMap = sizesInBOMByNhom.get(maNhom);
        const nhomName = nhomSizeNames.get(maNhom) || maNhom;

        const missing = [];
        for (const [sizeId, sizeName] of dtMap.entries()) {
            if (!bomMap.has(sizeId)) missing.push(sizeName);
        }

        const extra = [];
        for (const [sizeId, sizeName] of bomMap.entries()) {
            if (!dtMap.has(sizeId)) extra.push(sizeName);
        }

        if (missing.length) {
            errors.push(`Nhóm size '${nhomName}': Các size trong ${maDHJoined} không có trong BOM: ${missing.join(", ")}`);
        }
        //if (extra.length) {
        //    errors.push(`Inseam '${nhomName}': Các size trong BOM không có trong ${maDHJoined}: ${extra.join(", ")}`);
        //}
    }

    // 3.2 BOM có nhom nhưng _dt không có
    //for (const [maNhom, bomMap] of sizesInBOMByNhom.entries()) {
    //    if (!sizesInDtByNhom.has(maNhom)) {
    //        const nhomName = nhomSizeNames.get(maNhom) || maNhom;
    //        const sizeList = Array.from(bomMap.values()).join(", ");
    //        errors.push(`Inseam '${nhomName}' có trong BOM nhưng không có trong ${maDHJoined}:${sizeList}`);
    //    }
    //}

    if (!errors.length) return true;

    const message =
        "Phát hiện các vấn đề sau:\n\n" +
        errors.join("\n") +
        "\n\nBạn có muốn tiếp tục lưu không?";

    // confirm theo DevExtreme
    const ok = await confirmInProductionModal(message, "Cảnh báo");
    // confirm trả Promise<boolean>
    if (!ok) return false;

    return true;
}
// ======================================================================
// 11) MAIN SAVE (return true/false) + loading wrapper outside
// ======================================================================
async function onSaveClick() {
    try {
        ensurePopupInited();

        if (!validateBeforeSave()) return false;

        // MaDH ở textbox có thể là "DH_1;DH_2" nếu multi
        const maDHText = safeInstance("#txt_MaDH", "dxTextBox")?.option("value") || "";
        const dotSX = $("#txt_DotSX").val() || "";
        const maLenhUi = $("#txt_MaSX").val() || "";
        const lineRaw = safeInstance("#txt_LineX", "dxSelectBox")?.option("value");
        const line = (lineRaw == null || String(lineRaw).trim() === "") ? "" : lineRaw;
        const maDvsx = safeInstance("#sl_DonViSX", "dxSelectBox")?.option("value") || "";
        const ghiChu = $(".wip-toolbar-note input").val() || "";

        const poids = safeInstance("#sl_PO", "dxTagBox")?.option("value") || [];
        const poidStr = (poids || []).map(String).join(";");

        const gridRows = getGridData("#gridSanXuatContainer");
        if (!gridRows.length) {
            notify2("Không có dữ liệu sản xuất để lưu.", "warning", 2000);
            return false;
        }

        // ✅ maDHJoined ưu tiên pendingCross (kéo từ pool lên)
        const pendingCross = window.WIP?.cross?.pending;
        const maDHJoined = String(pendingCross?.maDH || maDHText || "").trim();   // "DH_1;DH_2" hoặc "DH_1"
        const maDHForLenh = maDHJoined.replaceAll(";", "@");                     // "DH_1@DH_2"

        const mahangFirst = String(gridRows[0]?.MaHang || "").trim();
        if (!maDHJoined || !mahangFirst) {
            notify2("Thiếu MaDH/MaHang trong dữ liệu grid.", "warning", 2200);
            return false;
        }

        // ✅ duplicate check theo cụm (joined)
        if (!_isEdit) {
            const dup = await checkDuplicateDotAndDvsx(maDHJoined, poidStr, dotSX, maDvsx);
            if (dup) {
                notify2("Dữ liệu đã bị trùng Đợt SX hoặc Đơn vị sản xuất. Vui lòng kiểm tra lại!", "warning", 2600);
                return false;
            }
        }
        const okBOM = await checkSizeBOM_web(maDHJoined);
        if (!okBOM) return false;
        //setLoading(true, "Đang lưu...");
        // ✅ resolve SoId theo cụm (joined)
        const soId = await resolveSoId(maDHJoined, mahangFirst);
        const magop = `${mahangFirst}|${soId}`;

        let sttLenh = 0;
        const m = String(dotSX).match(/^SX_(\d+)/i);
        if (m?.[1]) sttLenh = Number(m[1]) || 0;
        const maCu = m?.[1] ? `SX_${m[1]}` : "";
        // ✅ key LỆNH dùng DH đã @ hoá
        const maLenhSanXuat = [maDHForLenh, mahangFirst, dotSX, maDvsx, sttLenh || 0].join("|");
        const tenLenh = [maDHForLenh, mahangFirst, dotSX, maDvsx].join("|");

        const gopDhText = maDHJoined;

        const gopDhTable = buildGopDhTableFromGridRows(gridRows, soId, gopDhText);

        const dvsxListToSave = buildDvsxSaveList(gridRows, {
            maLenhSanXuat,
            maLenh: maLenhUi,
            tenLenh,
            dotSX,
            maDvsx,
            sttLenh,
            maGop: magop,
            maCu: maCu,
            line,
            ghiChu
        });

        if (!dvsxListToSave.length) {
            notify2("Tổng số lượng đã chia hết (<=0). Vui lòng kiểm tra lại!", "warning", 2400);
            return false;
        }

        const cdRow = getGridData("#gridProcessContainer")[0] || {};
        const chiTietCongDoanRow = buildChiTietCongDoanRow(cdRow, {
            magop,
            mahangFirst,
            maLenhSanXuat
        });

        const urlPostDvsx = _isEdit ? `${API_BASE}/PostDVSXV2` : `${API_BASE}/PostDVSX`;
        const rsDvsx = await apiPostJson(urlPostDvsx, dvsxListToSave);
        if (!isTrueResponse(rsDvsx)) {
            notify2(`Lỗi lưu DVSX: ${rsDvsx}`, "error", 3500);
            return false;
        }

        if (!_isEdit) {
            const rsGop = await apiPostJson(`${API_BASE}/PostGopDH`, gopDhTable);
            if (!isTrueResponse(rsGop)) {
                notify2(`Lỗi lưu Gộp đơn hàng: ${rsGop}`, "error", 3500);
                return false;
            }
        }

        const rsCD = await apiPostJson(`/api/CongDoan/PostChiTietCongDoan`, [chiTietCongDoanRow]);
        if (!isTrueResponse(rsCD)) {
            notify2(`Lỗi lưu Chi tiết công đoạn: ${rsCD}`, "error", 3500);
            return false;
        }

        return {
            success: true,
            maLenhSanXuat: maLenhSanXuat,
            maLenhUi: maLenhUi
        };
    } catch (err) {
        console.error(err);
        notify2("Đã xảy ra lỗi khi lưu. Mở console để xem chi tiết.", "error", 2500);
        return false;
    }
}

async function approveLenhAfterSplit(ctx) {
    const userName = ctx.userName || "";
    //console.log("approveLenhAfterSplit - user:", userName);
    // 1) GETMAXDOT
    let madot = "";
    try {
        const urlDot = `/api/ERPDonHangTong/Get?action=GETMAXDOT&para=${encodeURIComponent(ctx.maKH)}&para2=${encodeURIComponent(ctx.maHang)}`;
        const dtDot = await apiGetJson(urlDot);
        madot = dtDot?.[0]?.MaDot ? String(dtDot[0].MaDot) : "";
    } catch (e) {
        console.error(e);
        // cho phép madot rỗng nếu backend ok
    }

    // 2) GET_TSNEW
    try {
        const urlTS =
            `/api/KhoiTaoBOMV1/Get?action=GET_TSNEW&para1=${encodeURIComponent(ctx.maKH)}&para2=${encodeURIComponent(ctx.maHang)}&para6=${encodeURIComponent(userName)}&para7=${encodeURIComponent(madot)}`;
        await apiGetJson(urlTS); // Win không check kết quả, cứ gọi
    } catch (e) {
        console.error(e);
        // tuỳ bạn: nếu TSNEW fail thì cho dừng hay vẫn cho đi tiếp
        // mình đề xuất dừng để khỏi duyệt nửa vời:
        notify2("GET_TSNEW lỗi. Không thể duyệt.", "error", 2500);
        return { ok: false };
    }

    // 3) DongBo
    let dongboRes = "";
    try {
        const urlDongBo = `/api/ERPDongBoQLSX/DongBo?action=DongBo&para=${encodeURIComponent(String(ctx.maLenhSX))}`;
        // bên Win đang GetAsync trả string
        dongboRes = await $.ajax({ url: urlDongBo, method: "GET", dataType: "text" });
    } catch (e) {
        console.error(e);
        notify2("DongBo lỗi.", "error", 2500);
        return { ok: false };
    }

    if (String(dongboRes || "").trim().toUpperCase() === "2") {
        notify2("Duyệt thất bại. Vui lòng thử lại!", "error", 2500);
        return { ok: false };
    }

    // 4) PostIsKeoVe
    try {
    const listDH = splitMaDHJoined(ctx.maDH); // ctx.maDH có thể "DH1;DH2"
    if (!listDH.length) throw new Error("MaDH rỗng");

    for (const dh of listDH) {
        const urlKeoVe =
            `/api/ERPDonHangTong/GetChuyen?action=PostIsKeoVe&para=${encodeURIComponent(dh)}&para5=${encodeURIComponent(String(ctx.maLenhSX))}`;
        await $.ajax({ url: urlKeoVe, method: "GET", dataType: "text" });
    }
    } catch (e) {
        console.error(e);
        notify2("PostIsKeoVe lỗi (không chặn duyệt).", "warning", 2200);
    }

    return { ok: true };
}
// ======================================================================
// 12) INIT DEVEXPRESS CONTROLS + BIND EVENTS (only once)
// ======================================================================
function initDevExpressControls() {
    ensurePopupToast();
    ensureLoadPanel();

    const dd = { container: "#modalProduction" };

    // Textbox
    $("#txt_MaDH").dxTextBox({ height: 30, readOnly: true });
    $("#txt_MaHang").dxTextBox({ height: 30, readOnly: true });
    $("#txt_MaSX").prop("readonly", true).attr("readonly", "readonly");

    // Line select
    $("#txt_LineX").dxSelectBox({
        dataSource: [],
        valueExpr: "Line",
        displayExpr: "Name",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        placeholder: "Chọn chuyền",
        showClearButton: true,
        height: 30,
        dropDownOptions: dd
    });

    // PO
    $("#sl_PO").dxTagBox({
        dataSource: [],
        valueExpr: "POID",
        displayExpr: "PO",
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        placeholder: "Chọn PO",
        showMultiTagOnly: false,
        maxDisplayedTags: 5,
        multiline: true,  
        height: 30,
        dropDownOptions: dd,
        onValueChanged: async function (e) {
            if (_silent.po) return; // ✅ chặn lúc set bằng code
            const maDH = safeInstance("#txt_MaDH", "dxTextBox")?.option("value") || "";
            const poids = (e.value || []).map(x => String(x));
            await withLoading("Đang tải nhóm size...", async () => {
                await loadInSeamByMaDHAndPOs(maDH, poids);
            });
        }
    });

    // InSeam
    $("#sl_InSeam").dxTagBox({
        dataSource: [],
        valueExpr: "DauSizeID",
        displayExpr: "DauSize",
        placeholder: "Chọn nhóm size",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        height: 30,
        dropDownOptions: dd,
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        showClearButton: true,
        showMultiTagOnly: false, 
        maxDisplayedTags: 20,    
        multiline: true,  
    });

    // Màu
    $("#sl_Mau").dxTagBox({
        dataSource: [],
        valueExpr: "MaMau",
        displayExpr: "TenMau",
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        placeholder: "Chọn màu",
        maxDisplayedTags: 10,
        showMultiTagOnly: true,
        multiline: false,
        height: 30,
        dropDownOptions: dd
    });

    // Size
    $("#sl_Size").dxTagBox({
        dataSource: [],
        valueExpr: "SizeID",
        displayExpr: "Size",
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        placeholder: "Chọn size",
        maxDisplayedTags: 20,
        showMultiTagOnly: false,
        multiline: true,
        height: 30,
        dropDownOptions: dd
    });

    // DVSX select
    $("#sl_DonViSX").dxSelectBox({
        dataSource: [],
        valueExpr: "MaDVSX",
        displayExpr: "TenDVSX",
        placeholder: "[Chọn DVSX]",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        height: 30,
        dropDownOptions: dd,
        onValueChanged: function (e) {
            if (!e.value) return;
            fillGridCongDoanByDVSX(e.value);

            // top DVSX đổi -> grid công đoạn Cắt cũng đổi theo
            applyLineRuleByMaDVSX(e.value);
        }
    });
    $("#sl_Mer").dxSelectBox({
        dataSource: [],
        valueExpr: "UserID",
        displayExpr: "Ten",
        placeholder: "[Chọn MD]",
        searchEnabled: false,
        acceptCustomValue: false,
        openOnFieldClick: true,
        height: 30,
        dropDownOptions: dd
    });

    // Buttons
    $("#btnLayDuLieu").off("click").on("click", async function () {
        await onLayDuLieuClick();
    });

    $("#btnXacNhan").off("click").on("click", async function () {
        const $btn = $(this);
        try {
            $btn.prop("disabled", true);
            const ok = await onConfirmClick();
            if (ok) notify2("Đã tải xong.", "success", 900);
        } catch (err) {
            console.error(err);
            notify2("Lỗi tải dữ liệu.", "error", 1500);
        } finally {
            $btn.prop("disabled", false);
        }
    });
    // File JS PopUp
    $("#btnLuuTop").off("click").on("click", async function (e) {
        e.preventDefault();
        e.stopPropagation();
        const $btn = $(this);
        const pendingCross = window.WIP?.cross?.pending;
        const slth = parseNumberText("#lbl_SLTH");
        try {
            $btn.prop("disabled", true);
            await flushGridEdits();
            const mer = String(safeInstance("#sl_Mer", "dxSelectBox")?.option("value") || "").trim();
            const maDvsx = safeInstance("#sl_DonViSX", "dxSelectBox")?.option("value") || "";
            if (!mer) {
                notify2("Vui lòng chọn Mer trước khi gán chuyền.", "warning", 1800);
                return;
            }
            const ok = await withLoading("Đang lưu...", async () => {
                return await onSaveClick();
            });
            if (!ok) return;
            // ===== BƯỚC MỚI: DUYỆT =====
            const maLenhSanXuat = ok.maLenhSanXuat;
            const maLenhUi = $("#txt_MaSX").val() || "";
            const maLenhSX = parseLenhSXInt(maLenhUi);

            // lấy MaKH/MaHang ưu tiên từ pendingCross.item (vì kéo từ pool lên)
            const maKH = String(pendingCross?.item?.MaKH || "").trim();
            const maHang = String(pendingCross?.item?.StyleId || "").trim();
            const maDH = String(pendingCross?.maDH || safeInstance("#txt_MaDH", "dxTextBox")?.option("value") || "").trim();

            if (!maKH || !maHang || !maLenhSX) {
                notify2("Thiếu MaKH/MaHang/LenhSX để duyệt.", "warning", 2200);
                return;
            }
            const userName = localStorage.getItem("username") || localStorage.getItem("username1") ||"";
            //console.log(userName);
            const approve = await withLoading("Đang duyệt lệnh...", async () => {
                return await approveLenhAfterSplit({
                    maLenhUi,
                    maLenhSX,
                    maKH,
                    maHang,
                    maDH,
                    userName: userName || ""
                });
            });

            if (!approve?.ok) {
                // user cancel hoặc duyệt fail => stop ở đây, không assign, không lưu WIP
                if (approve?.canceled) notify2("Đã hủy duyệt.", "info", 1200);
                return;
            }

            // ===== SAU KHI DUYỆT OK: mới làm bước tiếp (assign-to-line / assign-outsource) =====
            if (pendingCross?.wipIds?.length) {
                const itemsAssign = (pendingCross.wipIds || []).map(id => ({
                    wipId: id,
                    //slkh: slth
                }));
                const cdRow = getGridData("#gridProcessContainer")[0] || {};
                const maDVSXCat = String(cdRow.MaDVSXCat || "").trim();
                const isGiaCong = isGiaCongByMaDVSX(maDVSXCat);

                const lineX = pendingCross.lineX ?? _defaultLineX ?? null;
                const assignUrl = isGiaCong ? "/api/wip-donhang/assign-outsource" : "/api/wip-donhang/assign-to-line";
                const payload = {
                    lineX: isGiaCong ? null : lineX,
                    isGiaCong: !!isGiaCong,
                    lenhSX: maLenhSX,
                    maLenhSanXuat: maLenhSanXuat,
                    items: itemsAssign,
                    mer: mer,
                    MaDVSX: maDvsx
                };

                if (!isGiaCong && (!lineX || Number(lineX) <= 0)) {
                    notify2("Thiếu chuyền để gán nội bộ.", "warning", 2200);
                    return;
                }

                await withLoading(isGiaCong ? "Đang gán Gia công ngoài..." : "Đang gán chuyền...", async () => {
                    return await $.ajax({
                        url: assignUrl,
                        method: "POST",
                        contentType: "application/json",
                        dataType: "json",
                        data: JSON.stringify(payload),
                        success: async function (res) {
                            //const pack = WIP.normalizeAssignResponse(res);
                            //const okLine = WIP.replaceLineData(lineX, pack.lineItems);
                            //if (!okLine) WIP.loadWipData({ lineX });

                            //pendingCross = null;
                            window.WIP?.cross?.clear?.();
                            notify2(isGiaCong ? "Đã duyệt & gán Gia công ngoài." : "Đã duyệt & gán vào chuyền.", "success", 1300);
                            closeProductionPopup();
                            if (!isGiaCong) await WIP.syncLibByLine(lineX);
                            await WIP.loadUnassignedData();
                            await WIP.loadWipData({ lineX });
                        },
                        error: function (xhr) {
                            try { WIP.addRowsToPool(pendingCross.rowsMoved || []); } catch { }
                            //pendingCross = null;
                            window.WIP?.cross?.clear?.();
                            notify2("Assign lỗi: " + (xhr.responseText || xhr.statusText), "error", 2500);
                        }
                    });
                });
                return;
            }
            closeProductionPopup();
            if (ok) notify2("Đã lưu.", "success", 1200);
            WIP.loadUnassignedData();
            // ok=false => onSaveClick đã notify warning/error rồi
        } catch (e) {
            console.error(e);
            notify2("Lưu thất bại.", "error", 1500);
        } finally {
            $btn.prop("disabled", false);
        }
    });

    // Grid top
    $("#gridDonHangContainer").dxDataGrid({
        dataSource: [],
        showBorders: true,
        height: "100%",
        width: "100%",
        columnAutoWidth: productionGridOptions().columnAutoWidth,
        wordWrapEnabled: productionGridOptions().wordWrapEnabled,
        columnFixing: productionGridOptions().columnFixing,
        scrolling: productionGridOptions().scrolling,
        columns: []
    });

    // Grid bottom
    $("#gridSanXuatContainer").dxDataGrid({
        dataSource: [],
        showBorders: true,
        height: "100%",
        width: "100%",
        columnAutoWidth: productionGridOptions().columnAutoWidth,
        wordWrapEnabled: productionGridOptions().wordWrapEnabled,
        columnFixing: productionGridOptions().columnFixing,
        scrolling: productionGridOptions().scrolling,
        editing: {
            mode: "cell",
            allowUpdating: true,
            selectTextOnEditStart: true,
            startEditAction: isMobileTabletProductionPopup() ? "dblClick" : "click",
            // Enter sẽ commit và move focus (user cảm giác “lưu”)
            enterKeyAction: "moveFocus",
            enterKeyDirection: "row"
        },
        columns: [
            { caption: "Đơn hàng", dataField: "MaDH"},
            { caption: "Màu", dataField: "TenMau" },
            { caption: "PO", dataField: "PO" },
            { caption: "Nhóm size", dataField: "DauSize"},
            { caption: "Tổng", dataField: "Amount" }
        ],
        onCellClick: function (e) {
            openMobileSizeQtyPopup(e);
        },
        onEditorPreparing: function (e) {
            if (e.parentType !== "dataRow") return;

            const field = String(e.dataField || "");
            if (!field.includes("@Size@")) return;

            const rowData = e.row.data;

            const bm = window._baselineQtyMap;
            const base = bm ? bm.get(rowKey(rowData)) : null;
            const max = base ? (Number(base[field] ?? 0) || 0) : null;

            e.editorName = "dxNumberBox";
            e.editorOptions = e.editorOptions || {};
            e.editorOptions.min = 0;
            e.editorOptions.step = 1;
            e.editorOptions.showSpinButtons = true;
            //if (max != null) e.editorOptions.max = max;
        },
        onRowUpdating: function (e) {
            const merged = Object.assign({}, e.oldData, e.newData);

            const bm = window._baselineQtyMap;
            const base = bm ? bm.get(rowKey(merged)) : null;
            for (const f of getSizeColumnsFromRow(merged)) {
                let v = Number(merged[f]);
                if (!Number.isFinite(v) || v < 0) v = 0;

                if (base) {
                    const max = Number(base[f] ?? 0) || 0;
                    if (v > max) {
                        const { Size } = parseSizeColKey(f);
                        v = max;
                        notify2(`Size ${Size}: không được vượt quá số lượng gốc (${max}).`, "warning", 2200);
                    }
                }
                merged[f] = v;
            }

            merged.Amount = calcAmountFromRow(merged);

            e.newData = merged;

            setTimeout(updateBottomFooter, 0);
        },
    });

    // Grid process
    $("#gridProcessContainer").dxDataGrid({
        dataSource: _congDoanDs,
        keyExpr: "ID",
        showBorders: true,
        columnAutoWidth: false,
        height: "100%",
        editing: { mode: "cell", allowUpdating: true },
        paging: { enabled: false },
        scrolling: { mode: "standard"},
        columns: [
            {
                caption: "Cắt",
                dataField: "MaDVSXCat",
                cssClass: "cd-wrap",
                alignment: "center",
                lookup: {
                    dataSource: () => _dvsxList,
                    valueExpr: "MaDVSX",
                    displayExpr: "TenDVSX"
                },
                editorOptions: {
                    dropDownOptions: { container: "#modalProduction", width: 260 },
                    searchEnabled: false,
                    acceptCustomValue: false,
                    openOnFieldClick: true
                }
            },
            {
                caption: "May",
                dataField: "MaDVSXMay",
                alignment: "center",
                cssClass: "cd-wrap",
                lookup: { dataSource: () => _dvsxList, valueExpr: "MaDVSX", displayExpr: "TenDVSX" },
                editorOptions: {
                    dropDownOptions: { container: "#modalProduction", width: 260 },
                    searchEnabled: false,
                    acceptCustomValue: false,
                    openOnFieldClick: true
                }
            },
            {
                caption: "Hoàn thành",
                dataField: "MaDVSXHoanThanh",
                alignment: "center",
                cssClass: "cd-wrap",
                lookup: { dataSource: () => _dvsxList, valueExpr: "MaDVSX", displayExpr: "TenDVSX" },
                editorOptions: {
                    dropDownOptions: { container: "#modalProduction", width: 260 },
                    searchEnabled: false,
                    acceptCustomValue: false,
                    openOnFieldClick: true
                }
            },
            {
                caption: "Nhập kho",
                dataField: "MaDVSXDongThung",
                alignment: "center",
                cssClass: "cd-wrap",
                lookup: { dataSource: () => _dvsxList, valueExpr: "MaDVSX", displayExpr: "TenDVSX" },
                editorOptions: {
                    dropDownOptions: { container: "#modalProduction", width: 260 },
                    searchEnabled: false,
                    acceptCustomValue: false,
                    openOnFieldClick: true
                }
            }
        ]
    });

    $(window)
        .off("resize.productionPopupGrid orientationchange.productionPopupGrid")
        .on("resize.productionPopupGrid orientationchange.productionPopupGrid", refreshProductionGridLayout);
}

// ======================================================================
// 13) PAGE READY
// ======================================================================
$(document).ready(function () {
    ensurePopupInited();
});
