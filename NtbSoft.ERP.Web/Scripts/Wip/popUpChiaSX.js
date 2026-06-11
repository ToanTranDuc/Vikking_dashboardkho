const API_BASE = "/api/CanDoiDonHangTong";
//không init lặp
let _wipPopupInited = false;
let _autoConfirmPending = false;
// Hàm auto Click Xác nhận
async function tryAutoConfirm() {
    if (_autoConfirmPending) return;

    const maDH = $("#txt_MaDH").dxTextBox("instance").option("value");
    const poids = $("#sl_PO").dxTagBox("instance").option("value") || [];
    const inseams = $("#sl_InSeam").dxTagBox("instance").option("value") || [];
    const maus = $("#sl_Mau").dxTagBox("instance").option("value") || [];
    const sizes = $("#sl_Size").dxTagBox("instance").option("value") || [];

    // bạn muốn đủ cả size
    const ok = maDH && poids.length && inseams.length && maus.length && sizes.length;
    if (!ok) return;

    _autoConfirmPending = true;
    try {
        await onConfirmClick();  // gọi trực tiếp thay vì trigger click
    } finally {
        _autoConfirmPending = false;
    }
}
// ===== DVSX cache =====
let _dvsxList = []; // [{MaDVSX, TenDVSX, GiaCong, ...}]

// build 1 row công đoạn theo DVSX được chọn
function buildCongDoanRow(maDVSXInit) {
    const dvsx = (_dvsxList || []).find(x => String(x.MaDVSX) === String(maDVSXInit));
    const isGiaCong = !!(dvsx && dvsx.GiaCong);

    return {
        MaDVSXCat: maDVSXInit || null,
        MaDVSXMay: maDVSXInit || null,
        MaDVSXHoanThanh: isGiaCong ? null : (maDVSXInit || null),
        MaDVSXDongThung: isGiaCong ? null : (maDVSXInit || null),
    };
}

// fill grid công đoạn (luôn 1 row)
function fillGridCongDoanByDVSX(maDVSXInit) {
    const grid = $("#gridProcessContainer").dxDataGrid("instance");
    if (!grid) return;

    const ds = initDataCongDoan(maDVSXInit);
    _congDoanDs = ds;              // sync state
    grid.option("dataSource", ds);
    grid.option("focusedRowIndex", 0);
}
function ensurePopupInited() {
    if (_wipPopupInited) return;
    initDevExpressControls();
    _wipPopupInited = true;
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
function apiGetJson(url) {
    return $.ajax({ url, method: "GET", dataType: "json" });
}
async function loadPOByMaDH(maDH) {
    ensurePopupInited();

    const poTag = $("#sl_PO").dxTagBox("instance");
    const inseamTag = $("#sl_InSeam").dxTagBox("instance");

    poTag.option({ dataSource: [], value: [] });
    inseamTag.option({ dataSource: [], value: [] });

    if (!maDH) return;

    const url = `${API_BASE}/GetPO?madh=${encodeURIComponent(maDH)}`;
    const dtPO = await apiGetJson(url);

    poTag.option("dataSource", dtPO || []);

    // default chọn hết
    const allPOIDs = (dtPO || []).map(x => String(x.POID));
    poTag.option("value", allPOIDs);

    await loadInSeamByMaDHAndPOs(maDH, allPOIDs);
}
let _isEdit = false;
let _defaultLineX = null;

async function onLayDuLieuClick() {
    try {
        ensurePopupInited();
        DevExpress.ui.notify("Đang lấy dữ liệu...", "info", 800);

        // ✅ tạo mã SX + đợt nếu không phải edit
        if (!_isEdit) {
            await buildNewLenhSX();
        } else {
            // edit mode: bạn set value từ biến _maSX/_dotSX/_maDVSX từ ngoài
            // (nếu bạn có)
        }

        // ✅ load DVSX
        await loadDonViSanXuat();

        // ✅ load line list và set default lineX truyền vào
        // (chỉ khi bạn chuyển txt_LineX thành dxSelectBox như trên)
        await loadLinesAndDefault(_defaultLineX);

        DevExpress.ui.notify("Xong.", "success", 900);
    } catch (e) {
        console.error(e);
        DevExpress.ui.notify("Lỗi lấy dữ liệu.", "error", 1500);
    }
}
async function loadInSeamByMaDHAndPOs(maDH, poids) {
    const inseamTag = $("#sl_InSeam").dxTagBox("instance");
    inseamTag.option({ dataSource: [], value: [] });

    if (!maDH || !poids || !poids.length) return;

    const poidStr = poids.join(";");
    const url = `${API_BASE}/GetDauSize?madh=${encodeURIComponent(maDH)}&poid=${encodeURIComponent(poidStr)}`;
    const dtDauSize = await apiGetJson(url); // [{MaDH, DauSizeID, DauSize}, ...]

    inseamTag.option({
        dataSource: dtDauSize,
        valueExpr: "DauSizeID",
        displayExpr: "DauSize"
    });

    // ✅ default chọn hết InSeam (nếu bạn muốn “chọn hết” luôn)
    inseamTag.option("value", (dtDauSize || []).map(x => String(x.DauSizeID)));
    const allInSeams = (dtDauSize || []).map(x => String(x.DauSizeID));
    inseamTag.option("value", allInSeams);

    // ✅ auto load Màu + Size
    await loadMauByMaDHAndInSeams(maDH, allInSeams);
}
async function loadMauByMaDHAndInSeams(maDH, dauSizeIds) {
    const mauTag = $("#sl_Mau").dxTagBox("instance");
    const sizeTag = $("#sl_Size").dxTagBox("instance");

    // reset downstream
    mauTag.option({ dataSource: [], value: [] });
    sizeTag.option({ dataSource: [], value: [] });

    if (!maDH || !dauSizeIds || !dauSizeIds.length) return;

    const dauSizeStr = dauSizeIds.join(";");
    const url = `${API_BASE}/GetMau?madh=${encodeURIComponent(maDH)}&dausizeID=${encodeURIComponent(dauSizeStr)}`;
    const dtMau = await apiGetJson(url); // [{MaMau, TenMau}, ...]

    mauTag.option({
        dataSource: dtMau || [],
        valueExpr: "MaMau",
        displayExpr: "TenMau"
    });

    // ✅ default chọn hết màu
    const allMau = (dtMau || []).map(x => String(x.MaMau));
    mauTag.option("value", allMau);

    // ✅ kéo tiếp Size theo tất cả màu
    await loadSizeByMaDH_InSeam_Mau(maDH, dauSizeIds, allMau);
}
async function loadSizeByMaDH_InSeam_Mau(maDH, dauSizeIds, mauIds) {
    const sizeTag = $("#sl_Size").dxTagBox("instance");
    sizeTag.option({ dataSource: [], value: [] });

    if (!maDH || !dauSizeIds?.length || !mauIds?.length) return;

    const dauSizeStr = dauSizeIds.join(";");
    const mauStr = mauIds.join(";");

    const url = `${API_BASE}/GetSizeID?madh=${encodeURIComponent(maDH)}&dausizeID=${encodeURIComponent(dauSizeStr)}&mamau=${encodeURIComponent(mauStr)}`;
    const dtSize = await apiGetJson(url); // [{SizeID, Size}, ...]

    sizeTag.option({
        dataSource: dtSize || [],
        valueExpr: "SizeID",
        displayExpr: "Size"
    });

    // ✅ default chọn hết size
    sizeTag.option("value", (dtSize || []).map(x => String(x.SizeID)));
    await tryAutoConfirm();
}
async function onConfirmClick() {
    const maDH = $("#txt_MaDH").dxTextBox("instance").option("value");

    const isAll = $("#chk_AllData").is(":checked");

    const poids = $("#sl_PO").dxTagBox("instance").option("value") || [];
    const dauSizeIds = $("#sl_InSeam").dxTagBox("instance").option("value") || [];
    const mauIds = $("#sl_Mau").dxTagBox("instance").option("value") || [];
    const sizeIdsSelected = $("#sl_Size").dxTagBox("instance").option("value") || [];

    // ===== Validate giống Win =====
    if (!isAll) {
        if (!maDH) return DevExpress.ui.notify("Vui lòng chọn đơn hàng!", "warning", 1800);
        if (!poids.length) return DevExpress.ui.notify("Vui lòng chọn PO!", "warning", 1800);
        if (!dauSizeIds.length) return DevExpress.ui.notify("Vui lòng chọn InSeam!", "warning", 1800);
        if (!mauIds.length) return DevExpress.ui.notify("Vui lòng chọn màu!", "warning", 1800);
    }

    DevExpress.ui.notify("Đang tải dữ liệu...", "info", 1000);

    // multi => join ;
    const poidStr = poids.map(String).join(";");
    const dauSizeStr = dauSizeIds.map(String).join(";");
    const mauStr = mauIds.map(String).join(";");

    // ===== 1) GetListSize (để vẽ cột) =====
    const urlListSize = `${API_BASE}/GetListSize?madh=${encodeURIComponent(maDH)}`;
    let dtSizeID = await apiGetJson(urlListSize); // [{SizeID, Size}, ...]

    // ===== 2) GetSoLuongDH =====
    const urlSLDH =
        `${API_BASE}/GetSoLuongDH?madh=${encodeURIComponent(maDH)}&dausizeid=${encodeURIComponent(dauSizeStr)}&mamau=${encodeURIComponent(mauStr)}&poid=${encodeURIComponent(poidStr)}`;
    let dtData = await apiGetJson(urlSLDH);

    // ===== 3) Filter theo size nếu user có chọn size (giống Win) =====
    if (!isAll && sizeIdsSelected.length) {
        const setSize = new Set(sizeIdsSelected.map(String));
        dtSizeID = (dtSizeID || []).filter(x => setSize.has(String(x.SizeID)));
        dtData = (dtData || []).filter(x => setSize.has(String(x.SizeID)));
    }

    // ===== 4) Build columns động từ dtSizeID (giống TaoCot) =====
    const SIZE_COL_W = 60;

    const sizeCols = (dtSizeID || []).map(s => ({
        caption: s.Size,
        dataField: `${s.SizeID}@Size@${s.Size}`,
        alignment: "center",
        width: SIZE_COL_W,
        minWidth: SIZE_COL_W,
        maxWidth: SIZE_COL_W,
        customizeText: function (e) {
            const v = Number(e.value ?? 0) || 0;
            return v === 0 ? "-" : String(v);
        }
    }));


    const baseCols = [
        { caption: "Đơn hàng", dataField: "MaDH", fixed: true, width: 90 },
        { caption: "Màu", dataField: "TenMau", fixed: true, width: 120 },
        { caption: "PO", dataField: "PO", fixed: true, width: 90 },
        { caption: "InSeam", dataField: "DauSize", fixed: true, width: 80 },
    ];

    const totalCol = {
        caption: "Tổng",
        dataField: "Amount",
        fixed: true,
        fixedPosition: "right",
        width: 70,
        alignment: "center"
    };

    // ===== 5) Pivot giống getDataTemp() =====
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

        // 👉 dùng SoLuong (nếu muốn SLTH thì thay r.SoLuong -> r.SLTH)
        const val = r.SoLuong == null || r.SoLuong === "" ? 0 : Number(r.SoLuong);

        const colKey = `${r.SizeID}@Size@${r.Size}`;
        row[colKey] = val;
        row.Amount += val;
    }


    const pivotRows = Array.from(map.values());

    // ===== 6) Apply lên grid =====
    const gridTop = $("#gridDonHangContainer").dxDataGrid("instance");
    gridTop.option({
        columnAutoWidth: false,
        wordWrapEnabled: false
    });
    gridTop.option("columns", [...baseCols, ...sizeCols, totalCol]);
    gridTop.option("dataSource", pivotRows);

    // footer totals
    const totalTop = pivotRows.reduce((s, x) => s + (x.Amount || 0), 0);
    $("#lbl_TotalTop").text(totalTop);
    // ===== 7) Grid dưới: lấy dữ liệu từ grid trên (giống btn_LayDuLieu + LoadDataGrd_1) =====

    const gridBottom = $("#gridSanXuatContainer").dxDataGrid("instance");

    // Nếu gridBottom bạn muốn edit các cột size thì bật editing và set allowUpdating theo cột size
    gridBottom.option({
        columnAutoWidth: false,
        wordWrapEnabled: false,
        columns: [...baseCols, ...sizeCols, totalCol],
        dataSource: pivotRows
    });
}

$("#btnXacNhan").off("click").on("click", onConfirmClick);
function buildPivotAndColumns(dtRows) {
    const fixedCols = [
        { caption: "Đơn hàng", dataField: "MaDH", fixed: true, width: 100 },
        { caption: "Màu", dataField: "TenMau", fixed: true, width: 120 },
        { caption: "PO", dataField: "PO", fixed: true, width: 90 },
        { caption: "InSeam", dataField: "DauSize", fixed: true, width: 70 },
    ];

    // size map: SizeID -> SizeName
    const sizeMap = new Map();
    // group key -> row object
    const groupMap = new Map();

    (dtRows || []).forEach(r => {
        const sizeId = String(r.SizeID ?? "");
        const sizeName = String(r.Size ?? sizeId);
        const qty = Number(r.SoLuong ?? 0) || 0;

        if (sizeId) sizeMap.set(sizeId, sizeName);

        const key = [
            r.MaDH, r.POID, r.MaMau, r.DauSizeID
        ].map(x => String(x ?? "")).join("|");

        if (!groupMap.has(key)) {
            groupMap.set(key, {
                MaDH: r.MaDH ?? "",
                MaHang: r.MaHang ?? "",
                POID: r.POID ?? "",
                PO: r.PO ?? "",
                MaMau: r.MaMau ?? "",
                TenMau: r.TenMau ?? "",
                DauSizeID: r.DauSizeID ?? "",
                DauSize: r.DauSize ?? "",
                Total: 0
            });
        }

        const rowObj = groupMap.get(key);
        const colKey = `SIZE_${sizeId}`;            // dataField an toàn
        rowObj[colKey] = (Number(rowObj[colKey] ?? 0) || 0) + qty;
        rowObj.Total += qty;
    });

    // build dynamic size columns (theo thứ tự label)
    const sizeCols = Array.from(sizeMap.entries())
        .sort((a, b) => a[1].localeCompare(b[1], "vi")) // sort theo tên Size
        .map(([sizeId, sizeName]) => ({
            caption: sizeName,
            dataField: `SIZE_${sizeId}`,
            width: 70,
            alignment: "right"
        }));

    const cols = [
        ...fixedCols,
        ...sizeCols,
        { caption: "Tổng", dataField: "Total", fixed: true, fixedPosition: "right", width: 70, alignment: "right" }
    ];

    // remove rows total=0 (giống WinForm bạn đang remove)
    const data = Array.from(groupMap.values()).filter(x => (Number(x.Total) || 0) > 0);

    return { columns: cols, data };
}
/* ---------------------------------------------------------------------- */
$(document).ready(function () {
    // Khởi tạo các control DevExtreme khi trang load (hoặc khi mở popup)
    ensurePopupInited();
});
async function openProductionPopup(maDH, lineX) {
    const $m = $("#modalProduction");
    if (!$m.length) return;

    ensurePopupInited();
    _defaultLineX = lineX;
    $("html,body").addClass("modal-open");
    $m.addClass("is-open").attr("aria-hidden", "false");

    $("#txt_MaDH").dxTextBox("instance").option("value", maDH || "");

    // 2) Lấy dữ liệu toolbar: Mã SX + Đợt + DVSX
    await onLayDuLieuClick();

    // 3) load PO -> InSeam -> Màu -> Size
    await loadPOByMaDH(maDH || "");
}
async function loadDonViSanXuat() {
    const dvsxSb = $("#sl_DonViSX").dxSelectBox("instance");
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

async function getMaxLenhSX() {
    const url = `/api/ERPDonHangTong/Get?action=GetMaxLenh`;
    const dt = await apiGetJson(url); // [{LenhSX:420}]
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

async function loadLinesAndDefault(defaultInput) {
    const lineSb = $("#txt_LineX").dxSelectBox("instance");
    const dt = await apiGetJson(`/api/ERPDonHangTong/Get?action=GetLine`);

    lineSb.option("dataSource", dt || []);

    if (defaultInput == null || defaultInput === "") return;

    const input = String(defaultInput).trim();

    // ✅ Nếu input là Name ("Daisy") thì tìm Line tương ứng ("101")
    const found = (dt || []).find(x => String(x.Name).trim() === input);

    // Nếu tìm thấy theo Name -> lấy Line
    // Nếu không tìm thấy -> coi input đã là Line
    const valueToSet = found ? String(found.Line).trim() : input;

    lineSb.option("value", valueToSet);

    console.log("[loadLinesAndDefault] input=", input, "=> set value=", valueToSet, "current=", lineSb.option("value"));
}

function closeProductionPopup() {
    $("#modalProduction").removeClass("is-open").attr("aria-hidden", "true");
    $("html,body").removeClass("modal-open");
}

// lấy value khi cần
function getPopupValues() {
    const maDH = $("#txt_MaDH").dxTextBox("instance").option("value");
    const lineX = $("#txt_LineX").dxSelectBox("instance").option("value");
    const dvsx = $("#sl_DonViSX").dxSelectBox("instance").option("value");
    return { maDH, lineX, dvsx };
}
function initDevExpressControls() {
    const dd = { container: "#modalProduction" };

    // Textbox
    $("#txt_MaDH").dxTextBox({ height: 30, readOnly: false });
    $("#txt_LineX").dxSelectBox({
        dataSource: [],
        valueExpr: "Line",
        displayExpr: "Name",
        searchEnabled: true,
        placeholder: "Chọn chuyền",
        height: 30,
        dropDownOptions: dd
    });

    // ✅ PO: init rỗng, dataSource sẽ set sau
    $("#sl_PO").dxTagBox({
        dataSource: [],
        valueExpr: "POID",
        displayExpr: "PO",
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        searchEnabled: true,
        placeholder: "Chọn PO",
        maxDisplayedTags: 2,
        showMultiTagOnly: true,
        multiline: false,
        height: 30,
        dropDownOptions: dd,
        onValueChanged: async function (e) {
            // ✅ mỗi lần đổi PO thì reload InSeam theo PO đã chọn
            const maDH = $("#txt_MaDH").dxTextBox("instance").option("value");
            const poids = (e.value || []).map(x => String(x));
            await loadInSeamByMaDHAndPOs(maDH, poids);
        }
    });

    // ✅ InSeam: init rỗng
    $("#sl_InSeam").dxTagBox({
        dataSource: [],
        valueExpr: "DauSizeID",
        displayExpr: "DauSize",
        placeholder: "Chọn InSeam",
        searchEnabled: true,
        height: 30,
        dropDownOptions: dd,
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        showClearButton: true,
        multiline: false,
        maxDisplayedTags: 1,
        showMultiTagOnly: true
    });

    // ✅ Màu: cũng multi như bạn nói → TagBox
    $("#sl_Mau").dxTagBox({
        dataSource: [],
        valueExpr: "MaMau",
        displayExpr: "TenMau",
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        searchEnabled: true,
        placeholder: "Chọn màu",
        maxDisplayedTags: 2,
        showMultiTagOnly: true,
        multiline: false,
        height: 30,
        dropDownOptions: dd
    });

    // ✅ Size: multi → TagBox
    $("#sl_Size").dxTagBox({
        dataSource: [],
        valueExpr: "SizeID",
        displayExpr: "Size",
        showSelectionControls: true,
        selectAllMode: "allPages",
        applyValueMode: "useButtons",
        searchEnabled: true,
        placeholder: "Chọn size",
        maxDisplayedTags: 2,
        showMultiTagOnly: true,
        multiline: false,
        height: 30,
        dropDownOptions: dd
    });

    $("#sl_DonViSX").dxSelectBox({
        dataSource: [],
        valueExpr: "MaDVSX",
        displayExpr: "TenDVSX",
        placeholder: "[Chọn DVSX]",
        searchEnabled: true,
        height: 30,
        dropDownOptions: dd,
        onValueChanged: function (e) {
            if (!e.value) return;
            fillGridCongDoanByDVSX(e.value);
        }
    });

    $("#btnLayDuLieu").off("click").on("click", onLayDuLieuClick);
    // ✅ tránh bind click nhiều lần nếu init lại (dù đã guard)
    $("#btnXacNhan").off("click").on("click", async function () {
        try {
            DevExpress.ui.notify("Đang tải dữ liệu...", "info", 800);
            $(this).prop("disabled", true);

            await onConfirmClick();

            DevExpress.ui.notify("Đã tải xong.", "success", 900);
        } catch (err) {
            console.error(err);
            DevExpress.ui.notify("Lỗi tải dữ liệu.", "error", 1500);
        } finally {
            $(this).prop("disabled", false);
        }
    });

    // grids giữ nguyên như bạn
    $("#gridDonHangContainer").dxDataGrid({
        dataSource: [],
        showBorders: true,
        columnAutoWidth: false,
        height: "100%",
        width: "100%",
        scrolling: { mode: "standard" },
        columns: []
        //columns: [
        //    { caption: "Đơn hàng", dataField: "MaDH", fixed: true, width: 100 },
        //    { caption: "Màu", dataField: "TenMau", fixed: true, width: 100 },
        //    { caption: "PO", dataField: "PO", fixed: true, width: 80 },
        //    { caption: "InSeam", dataField: "DauSize", fixed: true, width: 60 },
        //    { caption: "Z", dataField: "Size_Z", width: 80 },
        //    { caption: "MTM-6XL", dataField: "Size_MTM", width: 80 },
        //    { caption: "XS", dataField: "Size_XS", width: 80 },
        //    { caption: "S", dataField: "Size_S", width: 80 },
        //    { caption: "Tổng", dataField: "Total", fixed: true, fixedPosition: "right", width: 60 }
        //]
    });

    $("#gridSanXuatContainer").dxDataGrid({
        dataSource: [],
        showBorders: false,
        columnAutoWidth: true,
        height: "100%",
        width: "100%",
        scrolling: { mode: "standard", useNative: true },
        editing: { mode: "cell", allowUpdating: true },
        columns: [
            { caption: "Đơn hàng", dataField: "MaDH", width: 90 },
            { caption: "Màu", dataField: "TenMau" },
            { caption: "PO", dataField: "PO", width: 80 },
            { caption: "InSeam", dataField: "DauSize", width: 60 },
            { caption: "Tổng", dataField: "Total", width: 90 }
        ]
    });

    $("#gridProcessContainer").dxDataGrid({
        dataSource: _congDoanDs,
        keyExpr: "ID",
        showBorders: true,
        columnAutoWidth: false,
        height: "100%",
        editing: { mode: "cell", allowUpdating: true },

        // để cell selectbox mượt
        paging: { enabled: false },
        scrolling: { mode: "standard", useNative: true },

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
                    dropDownOptions: {
                        container: "#modalProduction",
                        width: 260
                    },
                    searchEnabled: true
                }
            },
            {
                caption: "May", dataField: "MaDVSXMay", alignment: "center", cssClass: "cd-wrap",
                lookup: { dataSource: () => _dvsxList, valueExpr: "MaDVSX", displayExpr: "TenDVSX" }, editorOptions: {
                    dropDownOptions: {
                        container: "#modalProduction",
                        width: 260
                    },
                    searchEnabled: true
                }
            },
            {
                caption: "Hoàn thành", dataField: "MaDVSXHoanThanh", alignment: "center", cssClass: "cd-wrap",
                lookup: { dataSource: () => _dvsxList, valueExpr: "MaDVSX", displayExpr: "TenDVSX" }, editorOptions: {
                    dropDownOptions: {
                        container: "#modalProduction",
                        width: 260
                    },
                    searchEnabled: true
                }
            },
            {
                caption: "Nhập kho", dataField: "MaDVSXDongThung", alignment: "center", cssClass: "cd-wrap",
                lookup: { dataSource: () => _dvsxList, valueExpr: "MaDVSX", displayExpr: "TenDVSX" }, editorOptions: {
                    dropDownOptions: {
                        container: "#modalProduction",
                        width: 260
                    },
                    searchEnabled: true
                }
            }
        ]
    });
}


/*------ CongDoan ------------------*/ 
let _congDoanRow = {
    ID: 0,
    MaDH: "",       // sẽ set lúc save (magop)
    MaHang: "",     // sẽ set lúc save
    MaLenhSX: "",   // sẽ set lúc save
    MaDVSXCat: null,
    MaDVSXMay: null,
    MaDVSXHoanThanh: null,
    MaDVSXDongThung: null,
    NguoiTao: "",   // set lúc init
    NguoiSua: ""
};
let _congDoanDs = [_congDoanRow];
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


// =========================
// SAVE FLOW
// =========================

// Lấy datasource hiện tại trong dxDataGrid (ưu tiên option dataSource)
function getGridData(gridId) {
    const grid = $(gridId).dxDataGrid("instance");
    if (!grid) return [];
    const ds = grid.option("dataSource");
    if (Array.isArray(ds)) return ds;
    // fallback nếu ds là DataSource object
    try {
        const items = grid.getDataSource()?.items?.();
        return Array.isArray(items) ? items : [];
    } catch {
        return [];
    }
}
// Tách các cột size theo format bạn đang dùng: `${SizeID}@Size@${Size}`
function getSizeColumnsFromRow(rowObj) {
    const keys = Object.keys(rowObj || {});
    return keys.filter(k => k.includes("@Size@"));
}
function parseSizeColKey(colKey) {
    // "123@Size@XL" -> { SizeID:"123", Size:"XL" }
    const parts = String(colKey).split("@Size@");
    return {
        SizeID: String(parts[0] ?? "").trim(),
        Size: String(parts[1] ?? "").trim()
    };
}
function buildDotSX(lenhSXUiLike, day = new Date().getDate()) {
    return `${lenhSXUiLike}-DOT-${day}`;
}
// ======= VALIDATION giống Win =======
function validateBeforeSave() {
    const dotSX = $("#txt_DotSX").val();
    if (!dotSX) {
        DevExpress.ui.notify("Vui lòng nhập Đợt SX trước khi lưu!", "warning", 2000);
        return false;
    }

    const dvsx = $("#sl_DonViSX").dxSelectBox("instance")?.option("value");
    if (!dvsx) {
        DevExpress.ui.notify("Vui lòng chọn Đơn vị sản xuất (DVSX) trước khi lưu!", "warning", 2000);
        return false;
    }

    const lineX = $("#txt_LineX").dxSelectBox("instance")?.option("value");

    // grid công đoạn: lấy row đầu
    const cdRow = getGridData("#gridProcessContainer")[0] || null;
    if (!cdRow) {
        DevExpress.ui.notify("Không có dữ liệu công đoạn!", "warning", 2000);
        return false;
    }

    // rule: bắt buộc chọn đủ 4 công đoạn (trừ gia công theo logic)
    // GiaCong lấy theo dvsx đang chọn
    const dvsxObj = (_dvsxList || []).find(x => String(x.MaDVSX) === String(dvsx));
    const isGiaCong = !!dvsxObj?.GiaCong;

    // Win: nếu KHÔNG gia công thì bắt buộc chọn Line
    if (!isGiaCong && !lineX) {
        DevExpress.ui.notify("Vui lòng chọn Chuyền (Line) trước khi lưu!", "warning", 2000);
        return false;
    }

    // Win: kiểm tra 4 DVSX công đoạn != null/empty
    const needAll4 = ["MaDVSXCat", "MaDVSXMay", "MaDVSXHoanThanh", "MaDVSXDongThung"];
    for (const f of needAll4) {
        const v = cdRow[f];
        // nếu isGiaCong: HoanThanh/DongThung cho phép null (theo bạn: 2 ô sau vẫn được chọn nếu gia công,
        // nhưng Win đang set null mặc định; user có thể chọn thì vẫn ok)
        if (isGiaCong && (f === "MaDVSXHoanThanh" || f === "MaDVSXDongThung")) continue;
        if (!v) {
            DevExpress.ui.notify("Vui lòng chọn đầy đủ Đơn vị sản xuất cho các công đoạn!", "warning", 2500);
            return false;
        }
    }

    // rule: MaDVSXHoanThanh không được là đơn vị GiaCong
    if (cdRow.MaDVSXHoanThanh) {
        const hoanThanhObj = (_dvsxList || []).find(x => String(x.MaDVSX) === String(cdRow.MaDVSXHoanThanh));
        if (hoanThanhObj?.GiaCong) {
            DevExpress.ui.notify("Công đoạn Hoàn thành không được chọn đơn vị Gia Công. Vui lòng chọn lại!", "warning", 2600);
            return false;
        }
    }

    return true;
}
// ======= SoID giống Win =======
async function resolveSoId(madh, mahangFirst) {
    // 1) GetKiemTra?madh=...
    const kt = await apiGetJson(`${API_BASE}/GetKiemTra?madh=${encodeURIComponent(madh || "")}`);
    const soIdFromKT = kt?.[0]?.SoID;

    if (soIdFromKT != null && String(soIdFromKT) !== "") {
        return Number(soIdFromKT) || 0;
    }

    // 2) GetGopID?madh=...
    const gop = await apiGetJson(`${API_BASE}/GetGopID?madh=${encodeURIComponent(mahangFirst || "")}`);
    const soIdFromGop = gop?.[0]?.SoID;

    if (soIdFromGop == null || String(soIdFromGop) === "") return 1;
    return (Number(soIdFromGop) || 0) + 1;
}

// ======= Check trùng (Dot + DVSX) giống Win =======
async function checkDuplicateDotAndDvsx(madh, poidStr, dotSX, maDvsx) {
    // Win: GetLenhSX?madh&poid -> trả list => lọc DotSX + MaDVSX
    const url = `${API_BASE}/GetLenhSX?madh=${encodeURIComponent(madh || "")}&poid=${encodeURIComponent(poidStr || "")}`;
    const dt = await apiGetJson(url);

    if (!Array.isArray(dt) || !dt.length) return false; // [] => không trùng

    // Nếu backend trả field khác, chỉnh tại đây
    const found = dt.some(x =>
        String(x.DotSX || "").trim() === String(dotSX || "").trim() &&
        String(x.MaDVSX || "").trim() === String(maDvsx || "").trim()
    );

    return found;
}
// ======= Build payload PostGopDH (DataTable Win) =======
function buildGopDhTableFromGridRows(gridRows, soId, gopDhText) {
    // Win: group MaDH + MaHang, tạo row cho từng MaDH trong danh sách _madh; web: đơn giản hoá:
    // - Mỗi row gridRows đã có MaDH + MaHang
    // - Tạo MaGop = `${MaHang}|${SoID}` (giống Win)
    // - GopDH = gopDhText (chuỗi đơn hàng hiển thị)
    // - SoID = soId

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

// ======= Build list CanDoiDonViSanXuatSaveEntity =======
function buildDvsxSaveList(gridRows, ctx) {
    // ctx: { maLenhSanXuat, maLenh, tenLenh, dotSX, maDvsx, sttLenh, maGop, maCu, line, ghiChu }
    const list = [];

    for (const r of (gridRows || [])) {
        const maDH = String(r.MaDH || "").trim();
        const maHang = String(r.MaHang || "").trim();

        // Các cột “base” bạn đang có sẵn trong pivot row
        const poid = String(r.POID || "").trim();
        const po = String(r.PO || "").trim();
        const maQG = String(r.MaQG || "").trim();
        const maMau = String(r.MaMau || "").trim();
        const dauSizeID = String(r.DauSizeID || "").trim();
        const dauSize = String(r.DauSize || "").trim();

        // size dynamic columns
        const sizeCols = getSizeColumnsFromRow(r);

        for (const colKey of sizeCols) {
            const qty = Number(r[colKey] ?? 0) || 0;

            // Win vẫn add mọi size, nhưng qty=0 thì SoLuong=0; thường ta bỏ 0 cho nhẹ
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
                POID_T: String(po).replace(/[\s!@#$%^&*()\-+=\[\]{};:'"\\|,.<>/?]+/g, "_"), // giống Win ReplaceSpecialCharacters
                MaCu: ctx.maCu,
                Line: ctx.line,
                GhiChu: ctx.ghiChu
            });
        }
    }

    return list;
}

// ======= Build ChiTietCongDoan row (1 dòng) =======
function buildChiTietCongDoanRow(gridRowCongDoan, ctx, isGiaCongDefault) {
    // SQL MERGE key: MaDH + MaHang + MaLenhSX
    // Win: tblSave.Rows[0]["MaDH"] = magop; tblSave.Rows[0]["MaHang"] = _mahang; tblSave.Rows[0]["MaLenhSX"] = MaLenhSanXuat
    // Web: ctx.magop, ctx.mahangFirst

    const row = {
        ID: 0,
        MaDH: ctx.magop,
        MaHang: ctx.mahangFirst,
        MaLenhSX: ctx.maLenhSanXuat,

        MaDVSXCat: gridRowCongDoan.MaDVSXCat || null,
        MaDVSXMay: gridRowCongDoan.MaDVSXMay || null,
        MaDVSXHoanThanh: gridRowCongDoan.MaDVSXHoanThanh ?? null,
        MaDVSXDongThung: gridRowCongDoan.MaDVSXDongThung ?? null,

        NguoiTao: window.CURRENT_USER || "",
        NguoiSua: ""
    };

    // Nếu bạn muốn giống Win “gia công thì mặc định null 2 ô sau”, nhưng vẫn cho user chọn -> không ép.
    // Nếu muốn ép null khi isGiaCongDefault=true thì uncomment:
    // if (isGiaCongDefault) { row.MaDVSXHoanThanh = null; row.MaDVSXDongThung = null; }

    return row;
}

// ======= MAIN SAVE =======
async function onSaveClick() {
    try {
        ensurePopupInited();

        // 0) Validate UI/Rules
        if (!validateBeforeSave()) return;

        // 1) Collect UI values
        const maDH = $("#txt_MaDH").dxTextBox("instance")?.option("value") || "";
        const dotSX = $("#txt_DotSX").val() || "";
        const maLenhUi = $("#txt_MaSX").val() || ""; // bạn đang gán txt_MaSX = lenhSXnew (ví dụ 419/420...)
        const line = $("#txt_LineX").dxSelectBox("instance")?.option("value") || "";
        const maDvsx = $("#sl_DonViSX").dxSelectBox("instance")?.option("value") || "";
        const ghiChu = $(".wip-toolbar-note input").val() || "";

        // POIDs để check trùng / dùng GetLenhSX (Win check theo madh+poid)
        const poids = $("#sl_PO").dxTagBox("instance")?.option("value") || [];
        const poidStr = (poids || []).map(String).join(";");

        // 2) Get grid rows (grid dưới là nơi user edit qty -> dùng gridSanXuatContainer)
        const gridRows = getGridData("#gridSanXuatContainer");
        if (!gridRows.length) {
            DevExpress.ui.notify("Không có dữ liệu sản xuất để lưu.", "warning", 2000);
            return;
        }

        // 3) Derive mahangFirst + magop + soId
        const mahangFirst = String(gridRows[0]?.MaHang || "").trim();
        if (!maDH || !mahangFirst) {
            DevExpress.ui.notify("Thiếu MaDH/MaHang trong dữ liệu grid.", "warning", 2200);
            return;
        }

        // 4) Check duplicate DotSX + DVSX (chỉ khi create mới, nếu edit mode thì tuỳ bạn)
        if (!_isEdit) {
            const dup = await checkDuplicateDotAndDvsx(maDH, poidStr, dotSX, maDvsx);
            if (dup) {
                DevExpress.ui.notify("Dữ liệu đã bị trùng Đợt SX hoặc Đơn vị sản xuất. Vui lòng kiểm tra lại!", "warning", 2600);
                return;
            }
        }

        // 5) BOM size check (bạn đã có endpoint GETCHECKSIZECHIASX ở Win)
        // Nếu bạn đã có API giống Win: KhoiTaoBOMV1/Get?action=GETCHECKSIZECHIASX&para1=...
        // -> ở web làm tương tự: hiện confirm nếu lệch.
        // (Đoạn này giữ đúng tinh thần Win, nhưng cần endpoint đúng URL của bạn)
        // Nếu bạn muốn bật lại, uncomment + sửa URL cho đúng:
        //
        // const bomUrl = `${URL}KhoiTaoBOMV1/Get?action=GETCHECKSIZECHIASX&para1=${encodeURIComponent(maDH)}`;
        // const bom = await apiGetJson(bomUrl);
        // ... build message & confirm ...
        //
        // Hiện tại mình không auto bật để khỏi sai URL.

        // 6) Resolve SoID (GetKiemTra -> GetGopID)
        const soId = await resolveSoId(maDH, mahangFirst);
        const magop = `${mahangFirst}|${soId}`;

        // 7) Build STTLenh (nếu bạn có số thứ tự trong UI txtLenhSXnew như Win "SX_437" -> lấy 437)
        // Web của bạn có lenhSX kiểu "SX_438" -> tách ra:
        let sttLenh = 0;
        const m = String(dotSX).match(/^SX_(\d+)/i);
        if (m?.[1]) sttLenh = Number(m[1]) || 0;

        // 8) Build MaLenhSanXuat / TenLenh (Win concat nhiều phần; web: chốt theo “khóa ổn định”)
        // Quan trọng: dùng cùng 1 string để PostDVSX và PostChiTietCongDoan key.
        const maLenhSanXuat = [
            maDH,
            mahangFirst,
            dotSX,
            maDvsx,
            sttLenh || 0
        ].join("|");

        const tenLenh = [
            maDH,
            mahangFirst,
            dotSX,
            maDvsx
        ].join("|");

        // 9) Build PostGopDH table
        const gopDhText = maDH; // nếu bạn có chuỗi “gộp đơn hàng” riêng thì set ở đây
        const gopDhTable = buildGopDhTableFromGridRows(gridRows, soId, gopDhText);

        // 10) Build DVSX list from grid size columns
        const dvsxListToSave = buildDvsxSaveList(gridRows, {
            maLenhSanXuat,
            maLenh: maLenhUi,     // nếu bạn muốn đúng Win: MaLenh = txtLenhSXnew.Text
            tenLenh,
            dotSX,
            maDvsx,
            sttLenh,
            maGop: magop,
            maCu: maLenhUi,
            line,
            ghiChu
        });

        if (!dvsxListToSave.length) {
            DevExpress.ui.notify("Tổng số lượng đã chia hết (<=0). Vui lòng kiểm tra lại!", "warning", 2400);
            return;
        }

        // 11) Build ChiTietCongDoan row (1 row)
        const cdRow = getGridData("#gridProcessContainer")[0] || {};
        const dvsxObj = (_dvsxList || []).find(x => String(x.MaDVSX) === String(maDvsx));
        const isGiaCongDefault = !!dvsxObj?.GiaCong;

        const chiTietCongDoanRow = buildChiTietCongDoanRow(cdRow, {
            magop,
            mahangFirst,
            maLenhSanXuat
        }, isGiaCongDefault);

        // 12) POST theo đúng thứ tự Win
        // a) PostDVSX / PostDVSXV2
        // Win: create mới dùng PostDVSX, edit dùng PostDVSXV2
        const urlPostDvsx = _isEdit ? `${API_BASE}/PostDVSXV2` : `${API_BASE}/PostDVSX`;
        const rsDvsx = await apiPostJson(urlPostDvsx, dvsxListToSave);
        if (!isTrueResponse(rsDvsx)) {
            DevExpress.ui.notify(`Lỗi lưu DVSX: ${rsDvsx}`, "error", 3500);
            return;
        }

        // b) PostGopDH (Win chỉ gọi khi create; edit tuỳ nghiệp vụ)
        if (!_isEdit) {
            const rsGop = await apiPostJson(`${API_BASE}/PostGopDH`, gopDhTable);
            if (!isTrueResponse(rsGop)) {
                DevExpress.ui.notify(`Lỗi lưu Gộp đơn hàng: ${rsGop}`, "error", 3500);
                return;
            }
        }

        // c) PostChiTietCongDoan (Win gọi khi create mới; edit thì vẫn MERGE được -> cứ gọi luôn)
        const rsCD = await apiPostJson(`/api/CongDoan/PostChiTietCongDoan`, [chiTietCongDoanRow]);
        if (!isTrueResponse(rsCD)) {
            DevExpress.ui.notify(`Lỗi lưu Chi tiết công đoạn: ${rsCD}`, "error", 3500);
            return;
        }
        closeProductionPopup();
    } catch (err) {
        console.error(err);
        DevExpress.ui.notify("Đã xảy ra lỗi khi lưu. Mở console để xem chi tiết.", "error", 2500);
    }
}

$("#btnLuuTop").off("click").on("click", async function () {
    try {
        $(this).prop("disabled", true);
        DevExpress.ui.notify("Đang lưu...", "info", 800);

        await onSaveClick();

        DevExpress.ui.notify("Đã lưu.", "success", 1200);
    } catch (e) {
        console.error(e);
        DevExpress.ui.notify("Lưu thất bại.", "error", 1500);
    } finally {
        $(this).prop("disabled", false);
    }
});

function isTrueResponse(x) {
    if (x === true) return true;
    if (x == null) return false;

    let s = String(x).trim();

    // nếu API trả '"True"' hoặc '"true"' -> bỏ cặp quote ngoài
    s = s.replace(/^"+|"+$/g, "").trim();

    return s.toLowerCase() === "true";
}
