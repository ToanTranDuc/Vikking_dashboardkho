/// VARIABLE
//var userNameSave = localStorage.getItem("username1");
let dxDataGridDangKyVatTu;
let dxDataGridDanhSachDangKy;
let dxDataGridEditVatTu;
let dxDataGridThuVien;
let selectedItems = [];
let selectedRowsToDelete = [];
let selectedRowsToSave = [];
let lstDataDangKyVatTu = [];
let selectedItemsChungLoai = [];
let batchData;
let dot;
let selectSoPhieu = '';
let selectSoPhieuDisplay = '';
let rowDelete = {};
let rowEdit = {};
let lstEditDangKyVatTu = [];
let isDeleteAll = false;
let isDeleteDK = false;
let isDeleteAllDK = false;
var isShowNgayCapAndGioCap = false;
let picker1, picker2, picker3, picker4;
let pickerTuNgayFilter, pickerDenNgayFilter;
let phieuDKJoin = [];

let mauSPOptions = [];
let sizeInfoOptions = [];
let tempSelectedValue = [];

let chungLoaiChiTietOptions = [];
let cacheSoLuong = [];
let dataSoLuong = [];
let lenhDangKyVatTuOptions = [];
let chungLoaiThuVienCache = [];
let soLuongLoadedMaLenh = "";

/// UTILS
function toIntFlag(val, fallback) {
    if (val === true) return 1;
    if (val === false) return 0;
    const n = parseInt(val, 10);
    return Number.isFinite(n) ? n : (fallback ?? 0);
}
function ddmmyyyyToYmd(dateStr) {
    if (!dateStr) return "1990-01-01";
    const [d, m, y] = dateStr.split("/");
    return `${y}-${m}-${d}`;
}
function formatThousands(numStr) {
    // numStr là string số nguyên phần (không có dấu thập phân)
    return String(numStr).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function PlayAudio() {
    const beepSound = document.getElementById("beepSound");
    beepSound.currentTime = 0;
    beepSound.play();
}

function PlayAudioError() {
    const beepSoundE = document.getElementById("beepSoundError");
    beepSoundE.currentTime = 0;
    beepSoundE.play();
}

/// API
const API = {
    async Get(action, para = {}, textSuccess = '') {
        const paraConvert = Object.entries(para || {})
            .map(([key, value]) => `${key}=${value}`)
            .join("&");

        const url = `/api/DeNghiCapThemNPL/Get?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error(`Response status: ${response.status}`);

            const data = await response.json();
            if (textSuccess) showToast("success", textSuccess);
            return data;
        } catch (error) {
            console.error(error);
        }
    },

    async Post(router = 'Post', action, arrSave, textSuccess = "Lưu thành công!", para = {}) {
        const paraConvert = Object.entries(para || {})
            .map(([key, value]) => `${key}=${value}`)
            .join("&");

        const url = `/api/DeNghiCapThemNPL/${router}?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(arrSave),
            });

            if (!response.ok) throw new Error(`Response status: ${response.status}`);

            const data = await response.json();
            if (data == "True") showToast('success', textSuccess, 1500);
        } catch (error) {
            console.error(error);
        }
    }
};

/// API CALLS
async function GetMaLenhDangKyVatTu() {
    const maKH = $("#khachhang").val() || "all";
    const $maLenhSelect = $("#malenhdangkyvattu");

    // Destroy select2 trước khi khởi tạo lại
    if ($maLenhSelect.hasClass("select2-hidden-accessible")) {
        $maLenhSelect.select2("destroy");
    }

    $maLenhSelect.empty().append(`<option ></option>`);

    const data = await API.Get("GetLenhDNCapThem", { para1: isNPL, para2: maKH });
    lenhDangKyVatTuOptions = Array.isArray(data) ? data : [];
    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaLenhSanXuat}">${x.Display}</option>`).join('');
        $maLenhSelect.append(html);
    }

    $maLenhSelect.select2({
        dropdownParent: $("#modalAddPhieu")
    });
}

/// check1
async function GetPhieuDangKyXuat() {
    const maLenh = $("#malenhdangkyvattu").val();
    const data = await API.Get("GetPhieuDKTHLenh", { para1: isNPL, para2: maLenh });
    batchData = data;
    renderList(data);
}


async function GetChiTietLenhThuHoi() {
    const maLenh = $("#malenhdangkyvattu").val();

    const data = await API.Get("GetChiTietLenhThuHoi", { para1: isNPL, para2: maLenh, para3: 1 })
    const maDH = Array.isArray(data) && data.length > 0
        ? ((data.find(x => (x.MaDH || "").toString().trim() !== "") || data[0]).MaDH || "")
        : "";
    $("#maDHNhap").val(maDH || "");
    createViewDxDataGridDangKyVatTu(data)
}

async function GetMaLenh2() {
    //const $maLenhSelect = $("#malenh");
    //const data = await API.Get("GetPYCDNTH", { para1: isNPL });
    //$maLenhSelect.empty();

    //if (data.length > 0) {
    //    const html = data.map(x => `<option value="${x.MaLenhSX}">${x.MaLenh}</option>`).join('');
    //    $maLenhSelect.append(html);
    //}

    //$maLenhSelect.select2();
    await GetPhieu();
}

async function GetDSPhieuDNTHVT() {
    const data = await API.Get("GetViewPDKVT", { para1: 'all', para2: isNPL })

    lstDSPhieuDKVT = data
    updateGrid(dxDataPhieuDKVT, data)
}

async function GetLenhChiTiet(maLenhSanXuat) {
    return await API.Get("GetChiTietLenh", { para1: isNPL, para2: maLenhSanXuat });
}

async function GetKhachHang() {
    const data = await API.Get("GetMaKH");
    const $khachHangSelect = $("#khachhang");

    if ($khachHangSelect.data('select2')) $khachHangSelect.select2('destroy');

    $khachHangSelect.empty().append(`<option value="all">Tất cả</option>`);

    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaKH}">${x.TenKH}</option>`).join('');
        $khachHangSelect.append(html);
    }

    $khachHangSelect.select2({ dropdownParent: $('#modalAddPhieu') });
}

async function GetPhieuMax() {
    const data = await API.Get("GetPhieuMaxYCTH", { para1: isNPL });
    dot = data[0].PXH;
    $("#txtPhieu").val(`PDNCP_${dot}`);
}

async function GetPhieu() {
    //const maLenhSanXuat = $("#malenh").val();
    //const data = await API.Get("GetPhieuDNTH", { para1: isNPL, para2: maLenhSanXuat });
    const data = await API.Get("GetPhieuDNTH", { para1: isNPL, para2: 'all' })
    const $soPhieu = $("#maphieu");

    if ($soPhieu.hasClass("select2-hidden-accessible")) {
        $soPhieu.select2("destroy");
    }
    $soPhieu.empty();

    $soPhieu.append('<option value ="all">--Tất cả--</option>');

    //if (data.length > 0) {
    //    const html = data.map(x => `<option data-display="${x.Display}" value="${x.PhieuTH}">${x.Display}</option>`).join('');
    //    selectSoPhieuDisplay = data[0].Display
    //    $soPhieu.append(html);
    //}
    if (data && data.length > 0) {
        const filtered = data.filter(x => x.PhieuTH !== 'all');
        const html = filtered.map(x =>
            `<option data-display="${x.Display}" value="${x.PhieuTH}">${x.Display}</option>`
        ).join('');
        $soPhieu.append(html);

    }

    $soPhieu.select2();

    await GetDSPhieuDNTH();
}

//async function GetDSPhieuDNTH() {
//    const soPhieu = $("#maphieu option:selected").val() || 'all';
//    const MaLenhSX = 'all';
//    const data = await API.Get("GetDSChiTietPhieu", { para1: soPhieu, para2: isNPL, para3: MaLenhSX });
//    lstDataDangKyVatTu = data;
//    updateGrid(dxDataGridDanhSachDangKy, data);
//}
//async function GetDSPhieuDNTH() {
//    showGlobalLoading("Đang tải danh sách phiếu...");
//    try {
//        const soPhieu = $("#maphieu option:selected").val() || 'all';
//        const data = await API.Get("GetDSChiTietPhieu", {
//            para1: soPhieu,
//            para2: isNPL,
//            para3: 'all'
//        });
//        lstDataDangKyVatTu = data;
//        updateGrid(dxDataGridDanhSachDangKy, data);
//        filterByTrangThai();
//    } finally {
//        hideGlobalLoading();
//    }
//}
async function GetDSPhieuDNTH() {
    showGlobalLoading("Đang tải danh sách phiếu...");
    try {
        const soPhieu = $("#maphieu option:selected").val() || 'all';
        const maNPLRows = await API.Get("GetMaNPLByPhieu", {
            para1: soPhieu,
            para3: 'all'
        });
        const maNPLList = (maNPLRows || []).map(x => x.MaNPL).filter(Boolean);
        const fastPromise = API.Get("GetDSChiTietPhieuFast", { para1: soPhieu, para2: isNPL, para3: 'all' });
        const tonKhoPromise = maNPLList.length > 0 ? fetchTonKhoMap(maNPLList) : Promise.resolve({});
        const sizeInfoPromise = fetchSizeInfoMap(soPhieu);
        const data = await fastPromise;
        lstDataDangKyVatTu = data;
        updateGrid(dxDataGridDanhSachDangKy, data);
        filterByTrangThai();
        loadPhongBanOptions();
        applyLazyColumns(data, tonKhoPromise, sizeInfoPromise);

    } finally {
        hideGlobalLoading();
    }
}

async function fetchTonKhoMap(maNPLList) {
    const BATCH_SIZE = 50;
    const batches = [];
    for (let i = 0; i < maNPLList.length; i += BATCH_SIZE) {
        batches.push(maNPLList.slice(i, i + BATCH_SIZE));
    }
    const results = await Promise.all(
        batches.map(b => API.Get("GetTonKhoByNPL", { para1: b.join(';') }))
    );
    const map = {};
    results.flat().forEach(x => { if (x?.MaNPL) map[x.MaNPL] = x; });
    return map;
}

async function fetchSizeInfoMap(soPhieu) {
    const data = await API.Get("GetSizeInfoByPhieu", { para1: soPhieu, para3: 'all' });
    const map = {};
    (data || []).forEach(x => {
        const key = `${x.MaVTID}|${x.MauVTID}|${x.KhoVaiID}|${x.MaNhom}|${x.MaCode}|${x.MaDot}`;
        map[key] = x.SizeInfo;
    });
    return map;
}

async function applyLazyColumns(data, tonKhoPromise, sizeInfoPromise) {
    dxDataGridDanhSachDangKy.columnOption("TonKho", "caption", "Tồn kho ⏳");
    dxDataGridDanhSachDangKy.columnOption("SizeInfo", "caption", "Size SP ⏳");

    try {
        const [tonKhoMap, sizeInfoMap] = await Promise.all([tonKhoPromise, sizeInfoPromise]);

        lstDataDangKyVatTu = lstDataDangKyVatTu.map(item => {
            const sizeKey = `${item.MaVTID}|${item.MauVTID}|${item.KhoVaiID}|${item.MaNhom}|${item.MaCode}|${item.MaDot}`;
            return {
                ...item,
                SizeInfo: item.IsTV == 1
                    ? item.SizeInfo
                    : (sizeInfoMap[sizeKey] ?? ''),
                TonKho: tonKhoMap[item.MaNPL]?.TonKho ?? null,
                TonKhoDK: tonKhoMap[item.MaNPL]?.TonKhoDK ?? null,
                SLDuKien: tonKhoMap[item.MaNPL]?.SLDuKien ?? null,
                SLLoi: tonKhoMap[item.MaNPL]?.SLLoi ?? null,
            };
        });

        filterByTrangThai();

    } finally {
        dxDataGridDanhSachDangKy.columnOption("TonKho", "caption", "SL Tồn kho");
        dxDataGridDanhSachDangKy.columnOption("SizeInfo", "caption", "Size SP");
    }
}

async function SavePhieu(arrSave) {
    await API.Post("PostDNCT", "PostDeNghiCapThem", arrSave, "Lưu phiếu thành công", { para1: isNPL });
    resetModalState();
    $("#modalAddPhieu").modal("hide"); 
    await GetMaLenh2();            
    await GetPhieuMax();
}
async function UpdateKiTenPhieu(arrSave) {
    await API.Post("PostDNCT", "PostDeNghiCapThem", arrSave, "Lưu phiếu thành công", { para1: isNPL });
    await GetDSPhieuDNTH();
}
async function DeleteChiTietPhieu(phieuTH, maNPL) {
    const res = await API.Get("DeleteChiTietPhieu", { para1: phieuTH, para2: maNPL });

    if (res && res[0].SoDongDaXoa > 0) {
        showToast("success", "Xóa thành công");
        await GetDSPhieuDNTH();

        if (lstDataDangKyVatTu.length === 0) {
            await resetToDefault();
        }
    } else {
        showToast("error", res?.[0]?.Message || "Không thể xóa");
    }
}

async function DeletePhieuDNTH() {
    await API.Get("DeletePhieuDNTH", {
        para1: selectSoPhieu
    }, "Xóa thành công");

    await GetDSPhieuDNTH();
    await GetMaLenh2();

    if (lstDataDangKyVatTu.length === 0) {
        await resetToDefault();
    }

    $("#modalComfimrtDeleteVT").modal('hide');
}

async function DeleteAllPhieu() {
    let res = await API.Get("DeletePhieuDNTH", { para1: selectSoPhieu });

    if (res && res[0].SoDongDaXoa > 0) {
        showToast("success", "Xóa phiếu thành công");

        await resetToDefault();
        chiTietLenhCache = {};

        $("#modalComfimrtDeleteVT").modal('hide');
    } else {
        showToast("error", res[0].Message || "Không thể xóa phiếu");
    }
}
async function UpdatePhieu() {
    const dataSource = dxDataGridEditVatTu.option("dataSource");
    const arrSave = [];
    if (dataSource.length === 0) {
        showToast("warning", "Không có dữ liệu để cập nhật");
        PlayAudioError();
        return;
    }

    const ngayThuHoi = ddmmyyyyToYmd($("#ngayCapEdit").val());
    dataSource.map(item => {

        const objectUpdate = {
            PhieuTH: item.PhieuTH,
            MaLenhSX: item.MaLenhSX,
            MaDH: item.MaDH,
            MaLenh: item.MaLenh,
            MaNPL: item.MaNPL,
            SLDK: item.SLDK,
            SLDKSP: item.SLDKSP ?? 0,
            GhiChu: item.GhiChu,
            MaNhom: item.MaNhom,
            MaVTID: item.MaVTID,
            MauVTID: item.MauVTID,
            KhoVaiID: item.KhoVaiID,
            MaVT: item.MaVT,
            MauVT: item.MauVT,
            KhoVai: item.KhoVai,
            MaDVVT: item.MaDVVT,
            NgayDK: item.NgayDK,
            NgayTH: ngayThuHoi,
            NgayTao: new Date(),
            IsNPL: isNPL,
            NguoiTH: userNameSave,
            PhieuDK: "",
            SignNgDK: "",
            NgayKi: "",
            SignTBPNgDK: "",
            NgayKiTBPNgDK: "",
            SignMer: "",
            NgaySignMer: "",
            SignTBPMer: "",
            NgaySignTBPMer: "",
            IsTV: item.IsTV,
            DinhMuc: item.DinhMuc,
            CapPhat: item.CapPhat,
            SoLuong: item.SoLuong,
            MaNhomChiTiet: item.ChungLoaiChiTiet,
            PhatSinhChiPhi: item.PhatSinhChiPhi ? 1 : 0,
            NgoaiDinhMuc: item.NgoaiDinhMuc ? 1 : 0,
            TrongDinhMuc: item.TrongDinhMuc ? 1 : 0,
            MaCode: item.MaCode || '',
        }

        arrSave.push(objectUpdate)
    })
    await API.Post("PostDNCT", "PostDeNghiCapThem", arrSave, "Cập nhật thành công");
    $('#modalEditDKVT').modal('hide');
    await GetDSPhieuDNTH();

    // trả về lại null
    picker4.dates.setValue('');
}

/// HELPER FUNCTIONS
function updateGrid(grid, dataSource) {
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}

function resetModalState() {
    selectedItems = [];
    selectedRowsToSave = [];
    selectedItemsChungLoai = [];
    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#phieuDKXInput').val('');
    $('#maDHNhap').val('');
    $('#dropdownList').removeClass('show');

    updateGrid(dxDataGridDangKyVatTu, []);
}

function resetThuVienModalState() {
    selectedItemsChungLoai = [];
    cacheSoLuong = [];
    chungLoaiChiTietOptions = [];

    if (typeof $floatingTagBox !== "undefined" && $floatingTagBox) {
        $floatingTagBox.hide();
    }
    if (typeof activeCell !== "undefined") {
        activeCell = null;
    }
    tempSelectedValue = [];

    if (dxDataGridThuVien) {
        dxDataGridThuVien.option("dataSource", []);
        dxDataGridThuVien.refresh();
    }

    const $checkAllRows = $("#checkAllRows");
    if ($checkAllRows.length) {
        $checkAllRows.prop("checked", false);
    }

    const $chungLoaiSelect = $("#chungloaiSelect");
    if ($chungLoaiSelect.length) {
        if ($chungLoaiSelect.hasClass("select2-hidden-accessible")) {
            $chungLoaiSelect.val(null).trigger("change.select2");
        } else {
            $chungLoaiSelect.val("");
        }
    }
}

async function resetToDefault() {
    selectSoPhieu = '';
    lstDataDangKyVatTu = [];

    await GetPhieu();
    $("#maphieu").val('all').trigger("change");
    await GetPhieuMax();

    updateGrid(dxDataGridDanhSachDangKy, []);
    isDeleteAll = false;
}

function renderList(data) {
    const itemList = $('#itemList');
    itemList.empty();

    // Kiểm tra data có tồn tại và là array không
    if (!data || !Array.isArray(data)) {
        return;
    }

    const html = data.map(item => {
        const isChecked = selectedItems.some(selected => selected.PhieuDK == item.PhieuDK);
        return `
            <div class="dropdown-item">
                <input type="checkbox" 
                       id="${item.PhieuDK}"
                       data-display="${item.PhieuDK}"
                       ${isChecked ? 'checked' : ''}>
                <label class="mb-0" for="${item.PhieuDK}">${item.PhieuDK}</label>
            </div>
        `;
    }).join('');

    itemList.append(html);
    updateCheckAll();
}

function updateCheckAll() {
    const total = $('#itemList input[type="checkbox"]').length;
    const checked = $('#itemList input[type="checkbox"]:checked').length;
    $('#checkAll').prop('checked', total > 0 && total == checked);
}


function updateInput() {
    const displayText = selectedItems.map(item => item.PhieuDK).join(', ');
    $('#phieuDKXInput').val(displayText);
    phieuDKJoin = selectedItems.map(item => item.PhieuDK);
}

function showLoading() {
    $('#loadingSpinner').fadeIn(200);
    $('#modalAddPhieu .modal-body section').css('opacity', '0.5');
}

function hideLoading() {
    $('#loadingSpinner').fadeOut(200);
    $('#modalAddPhieu .modal-body section').css('opacity', '1');
}

function updateSummary(grid) {
    const dataSource = grid.option("dataSource");

    const total = dataSource.reduce(
        (sum, item) => sum + (Number(item.SLDK) || 0),
        0
    );

    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
    let str = total.toString();
    let intPart = str;
    let decPart = "";

    if (str.includes(".")) {
        [intPart, decPart] = str.split(".");
        decPart = decPart.substring(0, 4);
    }

    /*  const formattedInt = Number(intPart).toLocaleString("en");*/
    const formattedInt = formatThousands(intPart);
    const result = decPart ? `${formattedInt}.${decPart}` : formattedInt;

    // tìm summary cell
    const columns = grid.option("columns");
    const visibleColumns = columns.filter(col => col.visible !== false);
    const checkbox_show = 1
    const sldkColumnIndex = visibleColumns.findIndex(col => col.dataField === "SLDK") + checkbox_show;

    if (sldkColumnIndex !== -1) {
        const $summaryRow = grid.element().find('.dx-datagrid-total-footer .dx-row');
        const $summaryCell = $summaryRow.find('td').eq(sldkColumnIndex);
        const $summaryItem = $summaryCell.find('.dx-datagrid-summary-item');

        if ($summaryItem.length) {
            $summaryItem.text(result);
        }
    }
}

function formatNumber(value) {
    if (value === null || value === undefined || value === "") return "0";
    let str = value.toString();
    let intPart = str;
    let decPart = "";

    if (str.includes(".")) {
        [intPart, decPart] = str.split(".");
        decPart = decPart.substring(0, 4); // cắt, không làm tròn
    }

    // format phần nguyên
    /*  let formattedInt = Number(intPart).toLocaleString();*/
    const formattedInt = formatThousands(intPart);
    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
}

function getLoaiVatTuValue(item) {
    return item.LoaiVT || item.LoaiVatTu || item.TenLoaiVatTu || item.Loai || "";
}

function formatLoaiVatTuText(value) {
    const normalized = normalizeText(value);
    if (normalized.includes("nguyen lieu") || normalized.includes("nguyenlieu") || normalized === "npl" || normalized === "nl") {
        return "Nguyên liệu";
    }
    if (normalized.includes("phu lieu") || normalized.includes("phulieu") || normalized === "pl") {
        return "Phụ liệu";
    }
    return value || "Khác";
}

function getDangKyRowBaseKey(item) {
    const parts = [
        item.MaLenhSanXuat,
        item.MaNPL,
        item.MaVTID,
        item.MauVTID,
        item.KhoVaiID,
        item.MaVT,
        item.MauVT,
        item.KhoVai,
        item.ChungLoaiVatTu,
        getLoaiVatTuValue(item)
    ];
    return parts.map(v => String(v ?? "")).join("|");
}

function getDangKyRowKey(item) {
    return item?.__rowKey || getDangKyRowBaseKey(item);
}

function isSameDangKyRow(a, b) {
    if (!a || !b) return false;
    const rowKeyA = a.__rowKey || "";
    const rowKeyB = b.__rowKey || "";
    if (rowKeyA && rowKeyB) return rowKeyA === rowKeyB;
    return getDangKyRowBaseKey(a) === getDangKyRowBaseKey(b);
}

function isDangKyRowSelected(item) {
    return selectedRowsToSave.some(x => isSameDangKyRow(x, item));
}

function isThuVienRowSelected(item) {
    return selectedItemsChungLoai.some(x =>
        x.MaLenhSanXuat === item.MaLenhSanXuat &&
        x.MaNPL === item.MaNPL
    );
}

function setThuVienRowHighlight(rowElement, isHighlighted) {
    const $row = $(rowElement);
    if (!$row || $row.length === 0) return;

    $row.toggleClass("thuvien-row-selected", !!isHighlighted);

    if (isHighlighted) {
        $row.children("td").css("background-color", "#bdd5f0");
    } else {
        $row.children("td").css("background-color", "");
    }
}

function ensureDangKyRowKeys(dataSource) {
    const rows = Array.isArray(dataSource) ? dataSource : [];
    const counters = {};
    return rows.map(item => {
        if (item.__rowKey) return item;
        const base = getDangKyRowBaseKey(item);
        counters[base] = (counters[base] || 0) + 1;
        return {
            ...item,
            __rowKey: `${base}#${counters[base]}`
        };
    });
}

function normalizeDangKyRows(dataSource) {
    if (!Array.isArray(dataSource)) return [];
    return ensureDangKyRowKeys(dataSource).map(item => ({
        ...item,
        LoaiVT: getLoaiVatTuValue(item)
    }));
}

function normalizeText(value) {
    return String(value || "")
        .toLowerCase()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .trim();
}

function getLoaiVatTuPriority(loaiVT) {
    const normalized = normalizeText(loaiVT);

    if (normalized.includes("nguyen lieu") || normalized.includes("nguyenlieu") || normalized === "npl" || normalized === "nl") {
        return 0;
    }

    if (normalized.includes("phu lieu") || normalized.includes("phulieu") || normalized === "pl") {
        return 1;
    }

    return 2;
}

function sortDangKyDataSource(dataSource) {
    const normalizedRows = normalizeDangKyRows(dataSource);
    return [...normalizedRows].sort((a, b) => {
        const selectedA = isDangKyRowSelected(a) ? 1 : 0;
        const selectedB = isDangKyRowSelected(b) ? 1 : 0;
        if (selectedA !== selectedB) {
            return selectedB - selectedA;
        }

        const loaiDiff = getLoaiVatTuPriority(getLoaiVatTuValue(a)) - getLoaiVatTuPriority(getLoaiVatTuValue(b));
        if (loaiDiff !== 0) {
            return loaiDiff;
        }

        const maVTDiff = String(a.MaVT || "").localeCompare(String(b.MaVT || ""), "vi");
        if (maVTDiff !== 0) return maVTDiff;

        return String(a.MaNPL || "").localeCompare(String(b.MaNPL || ""), "vi");
    });
}

async function setAutoMaDHByMaLenh() {
    const maLenh = $("#malenhdangkyvattu").val();
    if (!maLenh) {
        $("#maDHNhap").val("");
        return;
    }

    let maDH = "";

    const selectedLenh = lenhDangKyVatTuOptions.find(x => x.MaLenhSanXuat == maLenh);
    if (selectedLenh) {
        maDH = selectedLenh.MaDH || selectedLenh.MDH || selectedLenh.MaDonHang || "";
    }

    if (!maDH) {
        try {
            const response = await fetch(`/api/DeNghiCapThemNPL/GetMaDHByLenh?maLenhSX=${encodeURIComponent(maLenh)}&isNPL=${isNPL}`);
            if (response.ok) {
                const payload = await response.json();
                maDH = payload?.MaDH || "";
            }
        } catch (error) {
            console.error(error);
        }
    }

    $("#maDHNhap").val(maDH || "");
}

function getManualMaDH() {
    return ($("#maDHNhap").val() || "").trim();
}

/// EVENTS
$(document).ready(async function () {
    $(".select_2").select2();
    $("#malenhdangkyvattu").select2({
        dropdownParent: $("#modalAddPhieu")
    });
    createViewDxDataGridDanhSachDangKy();
    createViewDxDataGridDangKyVatTu();
    createViewDxDataGridEditVatTu();
    createViewDxGridDanhSachPhieuDKVT();

    await Promise.all([
        GetMaLenh2(),
        GetMaLenhDangKyVatTu()
    ]);
    selectSoPhieu = $("#maphieu option:selected").val();
});

$(function () {
    $("#khachhang").val("all").select2('destroy').select2();

    $("#home").on("click", () => window.location.href = '/Home/Dashboard');

    $("#khachhang").on("change", function () {
        GetMaLenh()
    });

    $('#phieuDKXInput').click(function (e) {
        e.stopPropagation();
        $('#dropdownList').toggleClass('show');
        $('#searchInput').val('').focus();
    });

    $("#trangThaiDuyet").select2();

    $("#trangThaiDuyet").on("change", function () {
        filterByTrangThai();
    });

    pickerTuNgayFilter = new tempusDominus.TempusDominus(
        document.getElementById('dtpTuNgayFilter'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    pickerDenNgayFilter = new tempusDominus.TempusDominus(
        document.getElementById('dtpDenNgayFilter'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    document.getElementById('dtpTuNgayFilter')
        .addEventListener(tempusDominus.Namespace.events.change, function () {
            filterByTrangThai();
        });

    document.getElementById('dtpDenNgayFilter')
        .addEventListener(tempusDominus.Namespace.events.change, function () {
            filterByTrangThai();
        });


    $(document).on('click', function (e) {
        if (!$(e.target).closest('.select-container').length) {
            $('#dropdownList').removeClass('show');
        }
    });

    //picker1 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker'), {
    //    display: {
    //        components: {
    //            calendar: true, date: true, month: true, year: true,
    //            clock: false, hours: false, minutes: false, seconds: false
    //        }
    //    },
    //    localization: { format: 'dd/MM/yyyy' },
    //    restrictions: {
    //        minDate: new tempusDominus.DateTime()
    //    }
    //});
    picker1 = new tempusDominus.TempusDominus(
        document.getElementById('datetimepicker'),
        {
            defaultDate: new tempusDominus.DateTime(),
            display: {
                components: {
                    calendar: true,
                    date: true,
                    month: true,
                    year: true,
                    clock: false,
                    hours: false,
                    minutes: false,
                    seconds: false
                }
            },
            localization: {
                format: 'dd/MM/yyyy'
            }
        }
    );

    // khóa không cho chọn
    document.getElementById('datetimepicker').disabled = true;

    picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' },
        restrictions: {
            minDate: new tempusDominus.DateTime()
        }
    });
    picker4 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker3'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    $("#tuNgay").trigger("click");
    $("#ngayCap").trigger("click");
    picker1.hide();
    picker2.hide();

    $('#searchInput').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const filtered = batchData.filter(item => item.Display.toLowerCase().includes(searchTerm));
        renderList(filtered);
    });

    //$('#btnSavePhieu').on("click", async function () {
    //    SaveDeNghiThuHoi()

    //});
    $('#btnSavePhieu').on("click", async function () {
        $(this).prop("disabled", true);
        try {
            await SaveDeNghiThuHoi();
        } finally {
            $(this).prop("disabled", false);
        }
    });
    $('#btnChonTuThuVien').on("click", async function () {
        const maLenh = $("#malenhdangkyvattu").val();
        if (maLenh == '') {
            showToast("warning", "Vui lòng chọn lệnh");
            return
        }
        showModalThuVien()

    });
    $('#checkAll').change(async function () {
        const isChecked = $(this).is(':checked');

        dxDataGridDangKyVatTu.beginCustomLoading("Đang tải dữ liệu...");

        try {
            if (isChecked) {
                selectedItems = [...batchData];

            } else {
                phieuDKJoin = [];
                selectedItems = [];
            }

            $('#itemList input[type="checkbox"]').prop('checked', isChecked);
            updateInput();
            GetChiTietLenhThuHoi();
        } finally {
            dxDataGridDangKyVatTu.endCustomLoading();
        }
    });

    $(document).on('change', '#itemList input[type="checkbox"]', async function () {
        const itemId = $(this).attr('id');
        const isChecked = $(this).is(':checked');

        if (isChecked) {
            const item = batchData.find(x => x.PhieuDK == itemId);
            if (item && !selectedItems.some(x => x.PhieuDK == itemId)) {
                selectedItems.push(item);
            }
        } else {
            selectedItems = selectedItems.filter(x => x.PhieuDK != itemId);
        }

        updateCheckAll();
        updateInput();
        GetChiTietLenhThuHoi();
    });

    $("#maphieu").on("change", function () {
        const $selectedOption = $(this).find("option:selected");

        selectSoPhieu = $selectedOption.val();
        selectSoPhieuDisplay = $selectedOption.data("display") || '';
        GetDSPhieuDNTH();
    });

    $("#btnShowModal").on("click", function () {
        // Reset các picker về ngày giờ hiện tại
        const now = new tempusDominus.DateTime();
        picker1.dates.setValue(now);
        picker2.dates.setValue(now);
        //picker3.dates.setValue(now);

        $("#modalAddPhieu").modal("show");
    });

    $("#btnDeleteAllPhieu").on("click", function () {
        if (selectSoPhieu == '' || !selectSoPhieu || selectSoPhieu === "all") {
            showToast("warning", "Vui lòng chọn phiếu để xóa");
            return;
        }
        isDeleteAll = true;
        $("#modalComfimrtDeleteVT").modal("show");
    });

    $("#btnEditAllPhieu").on("click", function () {
        if (selectSoPhieu == '' || !selectSoPhieu || selectSoPhieu === "all") {
            showToast("warning", "Vui lòng chọn phiếu để chỉnh sửa");
            return;
        }

        lstEditDangKyVatTu = dxDataGridDanhSachDangKy.option("dataSource")

        updateGrid(dxDataGridEditVatTu, lstEditDangKyVatTu)
        // Cập nhật ngày cấp và giờ cấp theo phiếu
        if (lstEditDangKyVatTu.length > 0) {
            const ngayCap = moment(lstEditDangKyVatTu[0].NgayCap).toDate();

            picker4.dates.setValue(tempusDominus.DateTime.convert(ngayCap));
        }
        isShowNgayCapAndGioCap = true
        $("#modalEditDKVT").modal("show");
        $("#txtNgayChinhSua").text(selectSoPhieuDisplay)
    });

    $("#btnConfirmtDeletePhieu").on("click", function () {
        if (rowDelete.__deleteType === "chiTiet") {
            DeleteChiTietPhieu(rowDelete.PhieuTH, rowDelete.MaNPL);
            rowDelete = {};
            $("#modalComfimrtDeleteVT").modal("hide");
            return;
        }
        if (isDeleteAllDK) {
            let dataSource = dxDataGridDangKyVatTu.option("dataSource");
            const deleteCount = selectedRowsToDelete.length;

            // Lọc bỏ tất cả các item đã chọn
            const updatedDataSource = dataSource.filter(item => {
                return !selectedRowsToDelete.some(selected =>
                    selected.MaLenhSanXuat === item.MaLenhSanXuat &&
                    selected.MaVT === item.MaVT
                );
            });

            updateGrid(dxDataGridDangKyVatTu, updatedDataSource);

            selectedRowsToDelete = [];
            $('#checkAllRows').prop('checked', false);
            $('.row-checkbox').prop('checked', false);

            showToast("success", `Đã xóa ${deleteCount} vật tư`);

            $("#txtSLVTDelete").empty();
            isDeleteAllDK = false;
            $("#modalComfimrtDeleteVT").modal("hide");
            return;
        }

        if (isDeleteDK) {
            let dataSource = dxDataGridDangKyVatTu.option("dataSource");
            const index = dataSource.findIndex(item =>
                item.MaLenhSanXuat === rowDelete.MaLenhSanXuat &&
                item.MaVT === rowDelete.MaVT &&
                item.KhoVai === rowDelete.KhoVai
            );

            if (index !== -1) {
                dataSource.splice(index, 1);
                updateGrid(dxDataGridDangKyVatTu, dataSource);
                selectedRowsToDelete = [];
                showToast("success", "Xóa thành công");
            }

            isDeleteDK = false;
            $("#modalComfimrtDeleteVT").modal("hide");

            return;
        }

        if (isDeleteAll) {
            DeleteAllPhieu();
        } else {
            DeletePhieuDNTH();
        }
    });
    $("#btnNapLaiDanhSach").on("click", async function (e) {
        e.preventDefault();
        const $btn = $(this);
        const $icon = $btn.find("i");
        if ($btn.prop("disabled")) return;

        $btn.prop("disabled", true);
        $icon.addClass("fa-spin");

        try {
            pickerTuNgayFilter.dates.clear();
            pickerDenNgayFilter.dates.clear();
            $("#filterTuNgay").val("");
            $("#filterDenNgay").val("");
            $("#trangThaiDuyet").val("all").trigger("change");

            await GetMaLenh2();
            showToast("success", "Nạp lại thành công");
        } catch (err) {
            console.error("Lỗi khi nạp lại:", err);
            alert("Có lỗi xảy ra khi nạp dữ liệu!");
        } finally {
            $btn.prop("disabled", false);
            $icon.removeClass("fa-spin");
        }
    });
    // Khi đóng modal - reset tất cả checkbox và state
    $('#modalAddPhieu').on('hidden.bs.modal', function () {
        // Reset checkbox
        $('#itemList input[type="checkbox"]').prop('checked', false);
        $('#checkAll').prop('checked', false);
        $('#phieuDKXInput').val('');
        $('#maDHNhap').val('');
        $('#dropdownList').removeClass('show');
        $("#malenhdangkyvattu").empty();

        // Reset state
        selectedItems = [];
        selectedRowsToSave = [];
        $("#txtSLVTDelete").empty();
        // Clear grid
        if (dxDataGridDangKyVatTu) {
            updateGrid(dxDataGridDangKyVatTu, []);
        }
    });

    // Khi mở modal - khởi tạo lại
    $('#modalAddPhieu').on('show.bs.modal', function () {
        GetPhieuMax();
        GetMaLenhDangKyVatTu();
        renderList([])
        selectedItems = []
        selectedRowsToSave = [];
        phieuDKJoin = [];
        $('#maDHNhap').val('');
    });

    // Nút xóa tất cả vật tư
    $("#btnDeleteAllDKVT").on("click", function () {
        if (selectedRowsToDelete.length === 0) {
            showToast("warning", "Vui lòng chọn vật tư để xóa")
            return;
        }
        isDeleteAllDK = true;
        $("#txtSLVTDelete").text(` (${selectedRowsToDelete.length})`);
        $("#modalComfimrtDeleteVT").modal("show")
    })

    $("#modalComfimrtDeleteVT").on("hide.bs.modal", function () {
        $("#txtSLVTDelete").empty();

        $("#txtConfirmDeleteMsg").html(
            `Bạn có chắc chắn muốn xóa<span id="txtSLVTDelete"></span> phiếu này khỏi danh sách?`
        );

        rowDelete = {};
    });

    $("#modalThuVien").on("hidden.bs.modal", function () {
        resetThuVienModalState();
    });

    $("#modalEditDKVT").on("show.bs.modal", function () {
        if (isShowNgayCapAndGioCap) {
            $("#colNgayCap").removeClass("d-none")
            $("#colGioCap").removeClass("d-none")

        } else {
            $("#colNgayCap").addClass("d-none")
            $("#colGioCap").addClass("d-none")
        }
    })
    $("#modalEditDKVT").on("hide.bs.modal", function () {
        isShowNgayCapAndGioCap = false
    })

    $("#malenh").on("change", async function () {
        await GetPhieu();
        //await loadOptionsMauSize();
    })

    $("#modalTimNhanh").on("shown.bs.modal", function () {
        setTimeout(function () {
            $("#inputTimKiem").focus();
        }, 100);
    });

    $("#modalTimNhanh").on("hide.bs.modal", function () {
        $("#inputTimKiem").val("").trigger("input")
    });

    $("#btnTimNhanh").on("click", function () {
        GetDSPhieuDNTHVT();

        $("#modalTimNhanh").modal('show')
    })

    // Nút tìm kiếm
    let searchTimeout;
    $("#inputTimKiem").on("input", function () {
        clearTimeout(searchTimeout);
        const search = $(this).val().trim().toLowerCase();

        searchTimeout = setTimeout(() => {
            if (search === "") {
                // Nếu rỗng thì hiện tất cả
                updateGrid(dxDataPhieuDKVT, lstDSPhieuDKVT);
            } else {
                // Lọc dữ liệu
                const dataFiltered = lstDSPhieuDKVT.filter(item =>
                    String(item.MaVT).trim().toLowerCase().includes(search) ||
                    String(item.MauVT).trim().toLowerCase().includes(search)
                );
                updateGrid(dxDataPhieuDKVT, dataFiltered);
            }
        }, 100); // Delay 300ms sau khi ngừng gõ
    });

    // check1.1
    $("#malenhdangkyvattu").on("change", async function () {
        selectedItems = []
        selectedRowsToSave = [];
        phieuDKJoin = []
        $('#phieuDKXInput').val('');
        await setAutoMaDHByMaLenh();
        createViewDxDataGridDangKyVatTu([]);
        await GetChiTietLenhThuHoi()
        await loadOptionsMauSize();
        await loadSoLuong(true);
        //    GetPhieuDangKyXuat();
    })


    $("#btnNapLai").on("click", async function () {
        const $btn = $(this);
        $btn.prop("disabled", true);
        try {
            await Promise.all([GetPhieuMax(), GetMaLenhDangKyVatTu()]);
            $("#phieuDKXInput").val("");
            $("#maDHNhap").val("");
            renderList([]);
            selectedItems = [];
            selectedRowsToSave = [];
            phieuDKJoin = [];
            updateGrid(dxDataGridDangKyVatTu, []); 
        } finally {
            $btn.prop("disabled", false);
        }
    });
    $("#chungloaiSelect").on("change", function () {
        const maCLVT = $(this).val();
        selectedItemsChungLoai = [];

        if (maCLVT) {
            chungLoaiChiTietOptions = GetChungLoaiChiTiet(maCLVT);
        }

        loadChungLoai()

    })

    // Toggle panel tìm kiếm
    $("#btnDropSearch").on("click", function (e) {
        e.stopPropagation();
        const isOpen = $("#panelTimKiem").is(":visible");
        $("#panelTimKiem").toggle(!isOpen);
        $("#arrowDropSearch").text(isOpen ? "▼" : "▲");
    });

    $(document).on("click", function (e) {
        if (!$(e.target).closest("#btnDropSearch, #panelTimKiem").length) {
            $("#panelTimKiem").hide();
            $("#arrowDropSearch").text("▼");
        }
    });

    // Xóa filter
    $("#btnXoaTimKiem").on("click", function () {
        $("#tkMaHang, #tkMaLenh, #tkNguoiLap").val("");
        $("#tkPhongBan").val("");
        $("#dotTimKiem").hide();
        filterByTrangThai();
    });

    // Apply filter
    $("#btnApplyTimKiem").on("click", function () {
        $("#panelTimKiem").hide();
        $("#arrowDropSearch").text("▼");
        const hasFilter = $("#tkMaHang").val() || $("#tkMaLenh").val() || $("#tkNguoiLap").val() || $("#tkPhongBan").val();
        $("#dotTimKiem").css("display", hasFilter ? "inline-block" : "none");
        filterByTrangThai();
    });

});

function handleBack() {
    // Reset checkbox
    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#phieuDKXInput').val('');
    $('#maDHNhap').val('');
    $('#dropdownList').removeClass('show');

    // Reset state
    selectedItems = [];
    selectedRowsToSave = [];
    $("#txtSLVTDelete").empty();
    // Clear grid
    if (dxDataGridDangKyVatTu) {
        updateGrid(dxDataGridDangKyVatTu, []);
    }

    $("#modalAddPhieu").modal("hide")
}
async function SaveDeNghiThuHoi() {
    const dataSource = dxDataGridDangKyVatTu.option("dataSource");

    if (dataSource == null || dataSource.length === 0) {
        showToast("warning", "Chưa có dữ liệu");
        PlayAudioError();
        return;
    }
    const dataToSave = selectedRowsToSave.length > 0
        ? dataSource.filter(item =>
            selectedRowsToSave.some(s =>
                isSameDangKyRow(s, item)
            )
        )
        : dataSource; // Nếu không chọn dòng nào thì save tất cả (tùy bạn)

    if (dataToSave.length === 0) {
        showToast("warning", "Vui lòng chọn ít nhất một dòng để lưu");
        PlayAudioError();
        return;
    }
    const maDHNhap = getManualMaDH();
    if (maDHNhap) {
        dataToSave.forEach(item => {
            item.MaDH = maDHNhap;
        });
    }

    const refItemGoc = dataToSave.find(item => item.MaLenhSanXuat && item.MaLenh && item.MaDH && item.MaDH !== "Thư viện");
    if (refItemGoc) {
        dataToSave.forEach(item => {
            if (!item.MaLenhSanXuat) item.MaLenhSanXuat = refItemGoc.MaLenhSanXuat;
            if (!item.MaLenh) item.MaLenh = refItemGoc.MaLenh;
            if (!item.MaDH || item.MaDH === 'Thư viện') item.MaDH = maDHNhap || refItemGoc.MaDH;
        });
    }
    const ngayDK = ddmmyyyyToYmd($("#tuNgay").val());
    const ngayCap = ddmmyyyyToYmd($("#ngayCap").val());

    const arrSave = dataToSave.map(item => ({
        PhieuTH: '',
        MaLenhSX: item.MaLenhSanXuat,
        MaDH: item.MaDH,
        MaLenh: item.MaLenh,
        MaNPL: item.MaNPL,
        SLDK: item.SLDK,
        SLDKSP: item.SLDKSP == null ? null : item.SLDKSP,
        GhiChu: item.GhiChu,
        MaNhom: item.MaNhom,
        MaVTID: item.MaVTID,
        MauVTID: item.MauVTID,
        KhoVaiID: item.KhoVaiID,
        MaVT: item.MaVT,
        MauVT: item.MauVT,
        KhoVai: item.KhoVai,
        MaDVVT: item.MaDVVT,
        NgayDK: ngayDK,
        NgayTH: ngayCap,
        NgayTao: new Date(),
        IsNPL: toIntFlag(item.IsNPL, item.NPL ? 1 : 0),
        NguoiTH: userNameSave,
        PhieuDK: "",
        SignNgDK: "",
        NgayKi: "",
        SignTBPNgDK: "",
        NgayKiTBPNgDK: "",
        SignMer: "",
        NgaySignMer: "",
        SignTBPMer: "",
        NgaySignTBPMer: "",
        IsTV: item.IsTV,
        DinhMuc: item.DinhMuc,
        CapPhat: item.CapPhat,
        SoLuong: item.SoLuong,
        MaNhomChiTiet: getMaNhom(item.ChungLoaiChiTiet),
        PhatSinhChiPhi: item.PhatSinhChiPhi == 1 ? 1 : 0,
        TrongDinhMuc: item.TrongDinhMuc == 1 ? 1 : 0,
        MaCode: item.MaCode || '',
        NgoaiDinhMuc: item.NgoaiDinhMuc == 1 ? 1 : 0,
    }));
    const refItem = arrSave.find(item => item.MaLenhSX && item.MaLenh && item.MaDH);
    if (refItem) {
        arrSave.forEach(item => {
            if (!item.MaLenhSX) item.MaLenhSX = refItem.MaLenhSX;
            if (!item.MaLenh) item.MaLenh = refItem.MaLenh;
            if (item.MaDH == 'Thư viện') item.MaDH = refItem.MaDH;
        });
    }
    const phieuChuaNhapSLDK = arrSave.find(item => item.SLDK <= 0 || !item.SLDK);
    if (phieuChuaNhapSLDK) {
        // Tìm row index trong grid
        const rowIndex = dataSource.findIndex(item =>
            item.MaVT === phieuChuaNhapSLDK.MaVT &&
            item.MaLenhSanXuat === phieuChuaNhapSLDK.MaLenhSX &&
            item.KhoVai === phieuChuaNhapSLDK.KhoVai
        );

        if (rowIndex !== -1) {
            // Scroll đến row
            dxDataGridDangKyVatTu.navigateToRow(dataSource[rowIndex]);

            // Thêm class highlight vào row
            setTimeout(() => {
                const $row = $(`#dxDataGridDangKyVatTu .dx-data-row`).eq(rowIndex);
                $row.addClass('highlight-row');

                // Focus vào input SLDK
                const $input = $row.find('.inputSLDK');
                if ($input.length) {
                    $input.focus().select();

                    // Xóa highlight khi người dùng thay đổi số lượng
                    $input.one('input', function () {
                        $row.removeClass('highlight-row');
                    });
                }
            }, 100);
        }

        showToast("warning", `Item Code: ${phieuChuaNhapSLDK.MaVT} chưa nhập số lượng đăng ký`);
        PlayAudioError();
        return;
    }
    const phieuChuaChonDinhMuc = dataToSave.find(item =>
        !item.TrongDinhMuc && !item.NgoaiDinhMuc
    );
    if (phieuChuaChonDinhMuc) {
        const rowIndex = dataSource.findIndex(item =>
            item.MaVT === phieuChuaChonDinhMuc.MaVT &&
            item.MaNPL === phieuChuaChonDinhMuc.MaNPL &&
            item.KhoVai === phieuChuaChonDinhMuc.KhoVai
        );

        if (rowIndex !== -1) {
            dxDataGridDangKyVatTu.navigateToRow(dataSource[rowIndex]);
            setTimeout(() => {
                const $row = $(`#dxDataGridDangKyVatTu .dx-data-row`).eq(rowIndex);
                $row.addClass('highlight-row');
                setTimeout(() => $row.removeClass('highlight-row'), 2000);
            }, 100);
        }

        showToast("warning", `Item Code: ${phieuChuaChonDinhMuc.MaVT} - Vui lòng chọn "Trong định mức" hoặc "Ngoài định mức"!`);
        PlayAudioError();
        return;
    }
    //show chữ ký
    showConfirmModalSign(async function () {
        if (signatureHistory.length === 0) {
            alert('Vui lòng ký tên trước khi lưu!');
            return false;
        }

        try {
            const signatureImage = canvas.toDataURL('image/png');
            arrSave.forEach(x => {
                x.SignNgDK = signatureImage;
            });

            // ✅ Đóng signatureModal TRƯỚC
            $("#signatureModal").modal("hide");

            // Sau đó mới gọi SavePhieu (sẽ đóng modalAddPhieu)
            await SavePhieu(arrSave);

            const arrTV = dataToSave.filter(item => item.IsTV === 1);
            if (arrTV.length > 0) {
                await SavePhieuThuVien(arrTV, signatureImage);
            }

            var maPhieu = $("#maphieu").val();
            sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                "Yêu cầu duyệt phiếu " + maPhieu,
                "TBP", "ALL", 1);

            return true;

        } catch (err) {
            console.error("Lỗi khi lưu phiếu:", err);
            showToast("error", "Lưu phiếu thất bại, vui lòng thử lại");
            return false;
        }
    });

}
function getMaNhom(chungLoaiChiTiet) {
    try {

        console.log(chungLoaiChiTiet)
        // Nếu là array
        if (Array.isArray(chungLoaiChiTiet) && chungLoaiChiTiet.length > 0) {
            return JSON.parse(chungLoaiChiTiet[0].MaNhom)?.MaCLCT ?? '';
        }
        // Nếu là object trực tiếp
        if (typeof chungLoaiChiTiet === 'object' && chungLoaiChiTiet !== null) {
            return JSON.parse(chungLoaiChiTiet.MaNhom)?.MaCLCT ?? '';
        }
        // Nếu là string
        if (typeof chungLoaiChiTiet === 'string') {
            return JSON.parse(chungLoaiChiTiet)?.MaCLCT ?? '';
        }
        return '';
    } catch (e) {
        console.error('getMaNhom error:', e, chungLoaiChiTiet);
        return '';
    }
}
async function SavePhieuThuVien(arrTV, signatureImage) {

    const payload = arrTV.flatMap(item => {
        const mauList = Array.isArray(item.MauSP) && item.MauSP.length > 0
            ? item.MauSP.map(v => {

                if (typeof v === "string") {
                    return {
                        MaMauSP: v,
                        TenMauSP: null
                    };
                }
                // Nếu v là object có MaMau/TenMau
                if (typeof v === "object" && v !== null) {
                    const inner = parseJsonSafe(v.MaMau);
                    return {
                        MaMauSP: inner?.MaMau ?? v.MaMau ?? null,
                        TenMauSP: inner?.TenMau ?? v.TenMau ?? null
                    };
                }
                return { MaMauSP: null, TenMauSP: null };
            })
            : (() => {
                const raw = (typeof item.MauSP === "object" && item.MauSP?.value !== undefined)
                    ? item.MauSP.value
                    : item.MauSP;
                const obj = parseJsonSafe(raw);
                return [{
                    MaMauSP: obj?.MaMau ?? (typeof raw === "string" ? raw : null),
                    TenMauSP: obj?.TenMau ?? null
                }];
            })();
        const sizeList = Array.isArray(item.SizeInfo) && item.SizeInfo.length > 0
            ? item.SizeInfo.map(v => {
                const obj = typeof v === "object" ? v : (() => { try { return JSON.parse(v); } catch { return {}; } })();
                return {
                    MaNhomSize: obj.MaNhomSize ?? null,
                    NhomSize: obj.NhomSize ?? obj.TenNhomSize ?? null,
                    MaSize: obj.MaSize ?? null,
                    TenSize: obj.TenSize ?? null
                };
            })
            : [{
                MaNhomSize: item.MaNhomSize ?? null,
                NhomSize: item.NhomSize ?? null,
                MaSize: item.MaSize ?? null,
                TenSize: item.TenSize ?? null
            }];

        return mauList.flatMap(mau =>
            sizeList.map(size => ({
                PhieuTH: item.PhieuTH ?? "",
                Dot: item.Dot ?? null,
                MaLenhSX: item.MaLenhSX ?? "",
                MaDH: item.MaDH ?? "",
                MaLenh: item.MaLenh ?? "",
                MaNPL: item.MaNPL ?? "",
                MaNhom: item.MaNhom ?? "",
                MaVTID: item.MaVTID ?? "",
                MauVTID: item.MauVTID ?? "",
                KhoVaiID: item.KhoVaiID ?? "",
                IsTV: item.IsTV ?? 0,
                MaMauSP: mau.MaMauSP,
                TenMauSP: mau.TenMauSP,
                MaNhomSize: size.MaNhomSize,
                NhomSize: size.NhomSize,
                MaSize: size.MaSize,
                TenSize: size.TenSize,
                SoLuong: item.SoLuong
            }))
        );
    });
    console.log()
    await API.Post("PostChiTiet", "POSTCHITIET", payload, "", { para1: isNPL });

}
function parseJsonSafe(v) {
    if (!v) return null;
    if (typeof v === "object") return v; // đã là object rồi, không cần parse
    try {
        return JSON.parse(v);
    } catch {
        return null;
    }
}
/// DX DataGrid
//đăng ký
function createViewDxDataGridDangKyVatTu(data) {
    dxDataGridDangKyVatTu = $("#dxDataGridDangKyVatTu").dxDataGrid({
        dataSource: sortDangKyDataSource(data),
        width: '100%',
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                caption: "",
                width: 50,
                alignment: "center",
                cellTemplate: function (container, options) {
                    const isChecked = isDangKyRowSelected(options.data);

                    const $checkbox = $(`
                        <input type="checkbox" 
                               class="row-checkbox" 
                               data-row-index="${options.rowIndex}"
                               ${isChecked ? 'checked' : ''}
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);
                
                    $checkbox.on("change", function () {
                        const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];

                        if (this.checked) {
                            if (!isDangKyRowSelected(options.data)) {
                                // Logic kế thừa như cũ
                                if (!options.data.SLDKSP || options.data.SLDKSP <= 0) {
                                    const lastSelectedWithValue = selectedRowsToSave
                                        .slice()
                                        .reverse()
                                        .find(r => r.SLDKSP && r.SLDKSP > 0);

                                    if (lastSelectedWithValue) {
                                        options.data.SLDKSP = lastSelectedWithValue.SLDKSP;
                                        const dinhMuc = parseFloat(options.data.DinhMuc) || 0;
                                        if (dinhMuc > 0) {
                                            options.data.SLDK = Math.round(options.data.SLDKSP * dinhMuc * 10000) / 10000;
                                        }
                                    }
                                }
                                selectedRowsToSave.push(options.data);
                            }
                        } else {
                         
                            options.data.SLDKSP = null;
                            options.data.SLDK = null;
                   

                            selectedRowsToSave = selectedRowsToSave.filter(item =>
                                !isSameDangKyRow(item, options.data)
                            );
                        }

                        dxDataGridDangKyVatTu.option("dataSource", sortDangKyDataSource(dataSource));
                        dxDataGridDangKyVatTu.refresh();
                        updateSummary(dxDataGridDangKyVatTu); 
                    });

                    container.append($checkbox);
                },
                headerCellTemplate: function (container) {
                    const $checkAll = $(`
                        <input type="checkbox" 
                               id="checkAllRows" 
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkAll.on("change", function () {
                        const isChecked = this.checked;
                        const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];

                        if (isChecked) {
                            selectedRowsToSave = [...dataSource];
                        } else {
                            selectedRowsToSave = [];
                        }

                        $('.row-checkbox').prop('checked', isChecked);
                        dxDataGridDangKyVatTu.option("dataSource", sortDangKyDataSource(dataSource));
                        dxDataGridDangKyVatTu.refresh();
                    });

                    container.append($checkAll);
                }
            },
            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MaLenh", caption: "Mã Lệnh", alignment: "center", minWidth: 100, visible: false },
            { dataField: "IsTV", caption: "IsTV", alignment: "center", minWidth: 100, visible: false },
            {
                dataField: "LoaiVT",
                caption: "Loại vật tư",
                alignment: "center",
                minWidth: 100,
                groupIndex: 0,
                visible: false,
                calculateGroupValue: function (rowData) {
                    return formatLoaiVatTuText(getLoaiVatTuValue(rowData));
                },
                groupCellTemplate: function (container, options) {
                    container.text(formatLoaiVatTuText(options.value));
                }
            },
            { dataField: "ChungLoaiVatTu", caption: "Chủng loại vật tư", alignment: "center", minWidth: 180 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "ChiTiet", caption: "Mô tả", alignment: "center", minWidth: 180 },
            { dataField: "MaNhomChiTiet", caption: "Item Code", alignment: "center", minWidth: 180, visible: false },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 120 },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "MauSP",
                caption: "Màu SP",
                alignment: "center",
                minWidth: 160,
                cellTemplate: function (container, cellOptions) {
                    const val = cellOptions.data.MauSP;
                    const displayText = Array.isArray(val)
                        ? val.map(v => {
                            const obj = typeof v === "object" ? v : (() => { try { return JSON.parse(v); } catch { return {}; } })();
                            return obj.TenMau || obj.MaMau || v;
                        }).join(", ")
                        : (val || "");
                    $("<span>").text(displayText).appendTo(container);
                }
            },
            {
                dataField: "SizeInfo",
                caption: "Size SP",
                alignment: "center",
                minWidth: 200,
                cellTemplate: function (container, cellOptions) {
                    const val = cellOptions.data.SizeInfo;
                    let displayText = "";
                    if (Array.isArray(val)) {
                        const grouped = {};
                        val.forEach(v => {
                            const obj = typeof v === "object" ? v : (() => { try { return JSON.parse(v); } catch { return {}; } })();
                            const nhom = obj.MaNhomSize || "Khác";
                            const size = obj.TenSize || v;
                            if (!grouped[nhom]) grouped[nhom] = [];
                            grouped[nhom].push(size);
                        });
                        displayText = Object.entries(grouped)
                            .map(([nhom, sizes]) => `${nhom}: ${sizes.join(", ")}`)
                            .join("; ");
                    } else {
                        displayText = val || "";
                    }
                    $("<span>").text(displayText).appendTo(container);
                }
            },

            {
                dataField: "CapPhat",
                caption: "SL Cấp Phát",
                alignment: "center",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.append(formatNumber(options.value))
                }
            },     
            { dataField: "SLNhap",
                caption: "SL Xuất",
                alignment: "center",
                minWidth: 120,
                visible: false,
                allowSorting: false,
                headerCellTemplate: function (header, info) {
                    const $container = $("<div></div>").css({
                        width: "100%",
                        display: "flex",
                        alignItems: "center",
                        gap: "14px",
                        padding: "0 8px"
                    });
                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-circle-arrow-right")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            flexShrink: "0",
                            marginRight: "6px",
                        })
                        .on("click", function (e) {
                            e.stopPropagation();
                            const dataSource = dxDataGridDangKyVatTu.option("dataSource");
                            dataSource.forEach(item => {
                                item.SLDK = Math.trunc((item.SLNhap ?? 0) * 10000) / 10000
                                refresh = true;
                            });

                            if (refresh) {
                                // Refresh grid để hiển thị dữ liệu mới
                                dxDataGridDangKyVatTu.refresh();
                            }
                        });
                    const $text = $("<span></span>")
                        .text(info.column.caption)
                        .css({
                            flexShrink: "0"
                        });
                    $container.append($text, $icon);
                    header.append($container);
                },
                cellTemplate: function (container, options) {
                    let value = options.value;
                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    const $wrapper = $("<div></div>").css({
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        gap: "6px"
                    });

                    const $text = $("<span></span>").text(formatNumber(value));

                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-circle-arrow-right")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            color: "#2196F3"
                        })
                        .on("click", function () {
                            options.data.SLDK = Math.trunc((value ?? 0) * 10000) / 10000;
                            dxDataGridDangKyVatTu.refresh();
                        });

                    $wrapper.append($text, $icon);
                    container.append($wrapper);
                }
            },
            { dataField: "DinhMuc", caption: "Định mức cấp phát", minWidth: 80 },
            { dataField: "SLDKSP",
                caption: "SL SP cấp thêm",
                minWidth: 110,
                allowSorting: false,
                cellTemplate: function (container, options) {

                    const initVal = options.data.SLDKSP != null
                        ? (() => {
                            const str = options.data.SLDKSP.toString();
                            const [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                            const formatted = formatThousands(intPart);
                            return decPart ? `${formatted}.${decPart.substring(0, 4)}` : formatted;
                        })()
                        : "";

                    const $input = $(`
                        <input type="text"
                               class="form-control inputSLSP"
                               value="${initVal}"
                               style="text-align:right;" />
                    `);

                    $input.on("input", function () {
                        const el = this;
                        let raw = el.value.replace(/,/g, "").replace(/[^0-9.]/g, "");

                        const dotIndex = raw.indexOf(".");
                        if (dotIndex !== -1) {
                            raw = raw.substring(0, dotIndex + 1) + raw.substring(dotIndex + 1).replace(/\./g, "");
                        }

                        if (raw.includes(".")) {
                            const [intPart, decPart] = raw.split(".");
                            raw = intPart + "." + decPart.substring(0, 4);
                        }

                        const slsp = raw === "" ? null : parseFloat(raw);

                        options.data.SLDKSP = slsp;

                        if (slsp < 0) {
                            showToast("warning", "Số lượng không được âm");
                            el.value = "";
                            options.data.SLDKSP = null;
                            return;
                        }

                        let formatted = "";
                        if (raw !== "" && raw !== ".") {
                            const parts = raw.split(".");
                            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            formatted = parts.join(".");
                        } else {
                            formatted = raw;
                        }

                        el.value = formatted;

                        const dinhMuc = parseFloat(options.data.DinhMuc) || 0;

                        if (slsp !== null && slsp >= 0 && dinhMuc > 0) {
                            let slvt = slsp * dinhMuc;
                            slvt = Math.round(slvt * 10000) / 10000;
                            options.data.SLDK = slvt;
                            if (slsp != null) {
                                if (!isDangKyRowSelected(options.data)) {
                                    selectedRowsToSave.push(options.data);
                                }
                            } else {
                                selectedRowsToSave = selectedRowsToSave.filter(item =>
                                    !isSameDangKyRow(item, options.data)
                                );
                            }
                            const $row = $(dxDataGridDangKyVatTu.getRowElement(options.rowIndex));
                            const $cb = $row.find('input.row-checkbox').first();
                            if ($cb.length) {
                                $cb.prop('checked', slvt > 0);
                            }

                            const $inputSLDK = $row.find('input.inputSLDK').first();
                            if ($inputSLDK.length) {
                                const str = slvt.toString();
                                const [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                                const formatted = formatThousands(intPart);
                                $inputSLDK.val(decPart ? `${formatted}.${decPart.substring(0, 4)}` : formatted);
                            }
                        }

                        updateSummary(dxDataGridDangKyVatTu);
                    });

                    $input.on("focus", function () {
                        this.select();
                    });

                    container.append($input);
                }
            },
            {
                dataField: "SLDK",
                caption: "SL VT cấp thêm",
                alignment: "center",
                minWidth: 120,
                allowSorting: false,
                headerCellTemplate: function (header, info) {
                    const $container = $("<div></div>").css({
                        width: "100%",
                        display: "flex",
                        alignItems: "center",
                        gap: "14px",
                        padding: "0 8px"
                    });
                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-rotate")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            flexShrink: "0",
                            marginRight: "6px",
                        })
                        .on("click", function (e) {
                            e.stopPropagation();
                            const dataSource = dxDataGridDangKyVatTu.option("dataSource");
                            dataSource.forEach(item => {
                                item.SLDK = null
                                refresh = true;
                            });

                            if (refresh) {
                                dxDataGridDangKyVatTu.refresh();
                            }
                        });
                    const $text = $("<span></span>")
                        .text(info.column.caption)
                        .css({
                            flexShrink: "0"
                        });
                    $container.append($text, $icon);
                    header.append($container);
                },
                //cellTemplate: function (container, options) {
                //    const $input = $(`
                //        <input type="number"
                //                class="form-control inputSLDK"
                //                value="${options.data.SLDK ?? ''}" />
                //    `);

                //    $input.on("input", function () {
                //        let value = this.value;

                //        // Không cho số âm
                //        if (Number(value) < 0) {
                //            showToast("warning", "Số lượng đăng ký không được âm");
                //            this.value = "";
                //            options.data.SLDK = null;
                //            updateSummary(dxDataGridDangKyVatTu);
                //            return;
                //        }

                //        // Chỉ cho tối đa 4 số thập phân 
                //        if (value.includes(".")) {
                //            const [intPart, decPart] = value.split(".");
                //            this.value = intPart + "." + decPart.substring(0, 4);
                //            value = this.value;
                //        }

                //        //if (Number(value) > options.data.SLNhap) {
                //        //    showToast("warning", "Số lượng đăng ký không được lớn hơn số lượng cấp phát");

                //        //    const truncated = Math.trunc(options.data.SLNhap * 10000) / 10000;

                //        //    this.value = truncated;
                //        //    options.data.SLDK = truncated;
                //        //    updateSummary(dxDataGridDangKyVatTu);
                //        //    return;
                //        //}

                //        options.data.SLDK = value === "" ? null : Number(value);
                //        updateSummary(dxDataGridDangKyVatTu);
                //        const numValue = options.data.SLDK;

                //        if (numValue > 0) {
                //            if (!isDangKyRowSelected(options.data)) {
                //                selectedRowsToSave.push(options.data);
                //            }
                //        } else {
                //            selectedRowsToSave = selectedRowsToSave.filter(item =>
                //                !isSameDangKyRow(item, options.data)
                //            );
                //        }

                //        const $row = $(dxDataGridDangKyVatTu.getRowElement(options.rowIndex));
                //        const $cb = $row.find('input.row-checkbox').first();
                //        if ($cb.length) {
                //            $cb.prop('checked', numValue > 0);
                //        }

                //        const capPhat = parseFloat(options.data.CapPhat) || 0;
                //        const sldk = options.data.SLDK || 0;
                //        const phanTram = capPhat > 0
                //            ? parseFloat(((sldk / capPhat) * 100).toFixed(2))
                //            : 0;
                //        const $rowEl = $(dxDataGridDangKyVatTu.getRowElement(options.rowIndex));
                //        $rowEl.find(".phan-tram-cell").text(phanTram + "%");
                //    });

                //    container.append($input);
                //}
                cellTemplate: function (container, options) {
                    const initVal = options.data.SLDK != null
                        ? (() => {
                            const str = options.data.SLDK.toString();
                            const [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                            const formatted = formatThousands(intPart);
                            return decPart ? `${formatted}.${decPart.substring(0, 4)}` : formatted;
                        })()
                        : '';
                    const $input = $(`
                    <input type="text"
                           class="form-control inputSLDK"
                           value="${initVal}"
                           style="text-align:right;" />
                `);

                    $input.on("input", function () {
                        const el = this;
                        const oldVal = el.value;
                        const cursorPos = el.selectionStart;
                        const commasBefore = (oldVal.substring(0, cursorPos).match(/,/g) || []).length;

                        let raw = oldVal.replace(/,/g, "").replace(/[^0-9.]/g, "");

                        const dotIndex = raw.indexOf(".");
                        if (dotIndex !== -1) {
                            raw = raw.substring(0, dotIndex + 1) + raw.substring(dotIndex + 1).replace(/\./g, "");
                        }
                        if (raw.includes(".")) {
                            const [intPart, decPart] = raw.split(".");
                            raw = intPart + "." + decPart.substring(0, 4);
                        }

                        if (Number(raw) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            el.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridDangKyVatTu);
                            return;
                        }

                        let formatted = "";
                        if (raw !== "" && raw !== ".") {
                            const parts = raw.split(".");
                            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            formatted = parts.join(".");
                        } else {
                            formatted = raw;
                        }

                        el.value = formatted;

                        const commasAfter = (formatted.substring(0, cursorPos).match(/,/g) || []).length;
                        const newCursor = cursorPos + (commasAfter - commasBefore);
                        el.setSelectionRange(newCursor, newCursor);

                        const numVal = raw === "" ? null : parseFloat(raw);
                        options.data.SLDK = numVal;
                        updateSummary(dxDataGridDangKyVatTu);

                        const numValue = options.data.SLDK;
                        if (numValue > 0) {
                            if (!isDangKyRowSelected(options.data)) {
                                selectedRowsToSave.push(options.data);
                            }
                        } else {
                            selectedRowsToSave = selectedRowsToSave.filter(item =>
                                !isSameDangKyRow(item, options.data)
                            );
                        }

                        const $row = $(dxDataGridDangKyVatTu.getRowElement(options.rowIndex));
                        const $cb = $row.find('input.row-checkbox').first();
                        if ($cb.length) {
                            $cb.prop('checked', numValue > 0);
                        }

                        const capPhat = parseFloat(options.data.CapPhat) || 0;
                        const sldk = options.data.SLDK || 0;
                        const phanTram = capPhat > 0
                            ? parseFloat(((sldk / capPhat) * 100).toFixed(2))
                            : 0;
                        const $rowEl = $(dxDataGridDangKyVatTu.getRowElement(options.rowIndex));
                        $rowEl.find(".phan-tram-cell").text(phanTram + "%");
                    });

                    $input.on("focus", function () {
                        this.select();
                    });

                    container.append($input);
                }
            },
            {
                dataField: "PhanTramCapThem",
                caption: "% Cấp thêm/Cấp phát",
                alignment: "center",
                minWidth: 130,
                allowEditing: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    const slCapPhat = parseFloat(options.data.CapPhat) || 0;
                    const slCapThem = parseFloat(options.data.SLDK) || 0;
                    const value = slCapPhat > 0
                        ? parseFloat(((slCapThem / slCapPhat) * 100).toFixed(2))
                        : 0;

                    const $span = $("<span>")
                        .addClass("phan-tram-cell")
                        .text(value + "%");

                    container.append($span);
                }
            },
            {
                dataField: "SLDaXuat",
                caption: "SL đã xuất",
                alignment: "center",
                minWidth: 120,
                allowEditing: false
            },

            {
                dataField: "TonKho",
                caption: "SL Tồn kho",
                alignment: "center",
                minWidth: 110,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    const value = options.value;

                    if (value === null || value === undefined) {
                        $("<span>")
                            .text("...")
                            .css({
                                color: "var(--color-text-tertiary)",
                                fontSize: "12px"
                            })
                            .appendTo(container);
                        return;
                    }

                    const num = parseFloat(value);
                    $("<span>")
                        .text(formatNumber(value))
                        .css({ color: num <= 0 ? "red" : "inherit" })
                        .appendTo(container);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Lý do",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control"
                               value="${options.data.GhiChu || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.GhiChu = this.value;
                    });

                    container.append($input);
                }
            },
            {
                dataField: "PhatSinhChiPhi",
                caption: "Phát sinh chi phí",
                alignment: "center",
                width: 80,
                visible: false,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    const $checkbox = $(`
            <input type="checkbox"
                   style="width: 18px; height: 18px; cursor: default;"
                   disabled />
        `);
                    $checkbox.prop("checked", options.data.PhatSinhChiPhi == 1 || options.data.PhatSinhChiPhi === true);
                    container.css({ justifyContent: "center", alignItems: "center" });
                    container.append($checkbox);
                }
            },
            {
                dataField: "TrongDinhMuc",
                caption: "Trong định mức",
                alignment: "center",   
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const $wrap = $("<div>").css({
                        display: "flex", justifyContent: "center",
                        alignItems: "center", width: "100%", height: "100%", minHeight: "30px"
                    });
                    const $checkbox = $(`<input type="checkbox"
            style="width: 18px; height: 18px; cursor: pointer; flex-shrink: 0;" />`);
                    $checkbox.prop("checked", options.data.TrongDinhMuc == 1 || options.data.TrongDinhMuc === true);
                    $checkbox.on("change", function () {
                        if (this.checked) {
                            options.data.TrongDinhMuc = 1;
                            options.data.NgoaiDinhMuc = 0;
                            const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];
                            dataSource.forEach(item => {
                                if (!item.TrongDinhMuc && !item.NgoaiDinhMuc) {
                                    item.TrongDinhMuc = 1;
                                    item.NgoaiDinhMuc = 0;
                                }
                            });
                        } else {
                            options.data.TrongDinhMuc = 0;
                        }
                        dxDataGridDangKyVatTu.refresh();
                    });
                    $wrap.append($checkbox);
                    container.empty().append($wrap);
                }
            },
            {
                dataField: "NgoaiDinhMuc",
                caption: "Ngoài định mức",
                alignment: "center",  
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const $wrap = $("<div>").css({
                        display: "flex", justifyContent: "center",
                        alignItems: "center", width: "100%", height: "100%", minHeight: "30px"
                    });
                    const $checkbox = $(`<input type="checkbox"
            style="width: 18px; height: 18px; cursor: pointer; flex-shrink: 0;" />`);
                    $checkbox.prop("checked", options.data.NgoaiDinhMuc == 1 || options.data.NgoaiDinhMuc === true);
                    $checkbox.on("change", function () {
                        if (this.checked) {
                            options.data.NgoaiDinhMuc = 1;
                            options.data.TrongDinhMuc = 0;

                            // ✅ Tự động tick tất cả các dòng chưa chọn gì
                            const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];
                            dataSource.forEach(item => {
                                if (!item.TrongDinhMuc && !item.NgoaiDinhMuc) {
                                    item.NgoaiDinhMuc = 1;
                                    item.TrongDinhMuc = 0;
                                }
                            });
                        } else {
                            options.data.NgoaiDinhMuc = 0;
                        }
                        dxDataGridDangKyVatTu.refresh();
                    });
                    $wrap.append($checkbox);
                    container.empty().append($wrap);
                }
            },
            // THÊM CỘT CHECKBOX
            {
                caption: "",
                width: 50,
                alignment: "center",
                visible: false,
                cellTemplate: function (container, options) {
                    const uniqueKey = `${options.data.MaLenhSanXuat}_${options.data.MaVT}`;
                    const isChecked = selectedRowsToDelete.some(item =>
                        item.MaLenhSanXuat === options.data.MaLenhSanXuat &&
                        item.MaVT === options.data.MaVT
                    );

                    const $checkbox = $(`
                        <input type="checkbox" 
                               class="row-checkbox" 
                               data-row-index="${options.rowIndex}"
                               ${isChecked ? 'checked' : ''}
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkbox.on("change", function () {
                        if (this.checked) {
                            // Thêm vào danh sách đã chọn
                            if (!selectedRowsToDelete.some(item =>
                                item.MaLenhSanXuat === options.data.MaLenhSanXuat &&
                                item.MaVT === options.data.MaVT)) {
                                selectedRowsToDelete.push(options.data);
                            }
                        } else {
                            // Xóa khỏi danh sách đã chọn
                            selectedRowsToDelete = selectedRowsToDelete.filter(item =>
                                !(item.MaLenhSanXuat === options.data.MaLenhSanXuat &&
                                    item.MaVT === options.data.MaVT)
                            );
                        }
                    });

                    container.append($checkbox);
                },
                headerCellTemplate: function (container) {
                    const $checkAll = $(`
                        <input type="checkbox" 
                               id="checkAllRows" 
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkAll.on("change", function () {
                        const isChecked = this.checked;
                        const dataSource = dxDataGridDangKyVatTu.option("dataSource");

                        if (isChecked) {
                            // Chọn tất cả
                            selectedRowsToDelete = [...dataSource];
                        } else {
                            // Bỏ chọn tất cả
                            selectedRowsToDelete = [];
                        }

                        // Update tất cả checkbox trong grid
                        $('.row-checkbox').prop('checked', isChecked);
                    });

                    container.append($checkAll);
                }
            },
            {
                caption: "Xóa",
                minWidth: 50,
                alignment: "center",
                visible: false,
                cellTemplate: function (container, options) {
                    // Thêm nút
                    $("<div>")
                        .css({
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center",
                            height: "25px"
                        })
                        .append(
                            $("<i>")
                                .addClass("fa-solid fa-trash-can")
                                .css({ fontSize: "15px", color: "red", cursor: "pointer" })
                                .on("click", function () {
                                    rowDelete = options.data;
                                    isDeleteDK = true;
                                    $("#modalComfimrtDeleteVT").modal("show");
                                })
                        )
                        .appendTo(container);
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "CapPhat",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                },
                {
                    column: "SLDK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                }
            ]
        },
        onRowPrepared: function (e) {
            if (e.rowType !== "data") return;
            const isSelected = isDangKyRowSelected(e.data);
            $(e.rowElement).toggleClass("row-selected-soft", isSelected);
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

                if (e.column.dataField === "IsNPL") {
                    const visibleRows = e.component.getVisibleRows();
                    const currentRowIndex = e.rowIndex;
                    const currentRow = visibleRows[currentRowIndex];
                    if (!currentRow) return;

                    const currentVal = currentRow.data?.IsNPL;
                    const currentPhieu = currentRow.data?.PhieuTH;

                    let rowSpan = 1;
                    for (let i = currentRowIndex + 1; i < visibleRows.length; i++) {
                        const nextRow = visibleRows[i];
                        if (
                            nextRow.rowType === "data" &&
                            nextRow.data.IsNPL === currentVal &&
                            nextRow.data.PhieuTH === currentPhieu
                        ) {
                            rowSpan++;
                        } else {
                            break;
                        }
                    }

                    const prevRow = visibleRows[currentRowIndex - 1];
                    if (
                        prevRow &&
                        prevRow.rowType === "data" &&
                        prevRow.data.IsNPL === currentVal &&
                        prevRow.data.PhieuTH === currentPhieu
                    ) {
                        $(e.cellElement).css("display", "none");
                    } else {
                        e.cellElement.rowSpan = rowSpan;
                        const text = currentVal == 1 ? "Nguyên liệu" : "Phụ liệu";
                        const color = currentVal == 1 ? "#1565c0" : "#e65100";
                        $(e.cellElement).css("vertical-align", "middle");
                        $(e.cellElement).html(
                            `<span style="font-size:12px;font-weight:bold;color:${color}">${text}</span>`
                        );
                    }
                }
            }
        },
        onContentReady: function (e) {
            const grid = e.component;
            if (grid.columnOption("MaDH", "visible") !== false) {
                grid.columnOption("MaDH", "visible", false);
            }
            if (grid.columnOption("LoaiVT", "groupIndex") !== 0) {
                grid.columnOption("LoaiVT", "groupIndex", 0);
            }
        }
    }).dxDataGrid("instance");
}
function renderSignCell(container, options, fieldName) {
    if (!options.data) return;

    const containerDiv = $("<div>").css({
        width: "100%",
        height: "40px",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        gap: "6px",
        position: "relative"
    });

    if (options.data[fieldName] && options.data[fieldName] !== "checked") {
        const tenOnly = options.data[{ SignNgDK: "TenUserNgDK_Ten", SignTBPNgDK: "TenUserTBPNDK_Ten", SignMer: "TenUserMerSoatSet_Ten", SignTBPMer: "TenUserTBPMerSoatSet_Ten" }[fieldName]] ?? "";
        const $wrapper = $("<div>").css({ display: "flex", flexDirection: "column", alignItems: "center", gap: "2px" });
        $("<img>").attr("src", `/Images/SignDeNghiCapThemNPL/${options.data[fieldName]}?${Date.now()}`).css({ width: "70px", height: "35px" }).appendTo($wrapper);
        if (tenOnly) $("<span>").text(tenOnly).css({ fontSize: "11px", color: "#333", fontWeight: "bold" }).appendTo($wrapper);
        $wrapper.appendTo(containerDiv);
    }
    else if (options.data[fieldName] === "checked") {
        const userField = { SignNgDK: "HoTenNgDK", SignTBPNgDK: "HoTenTBPNDK", SignMer: "HoTenMerSoatSet", SignTBPMer: "HoTenTBPMerSoatSet" }[fieldName] ?? "";
        const hinhAnhField = { SignNgDK: "HinhAnhNgDK", SignTBPNgDK: "HinhAnhTBPNDK", SignMer: "HinhAnhMerSoatSet", SignTBPMer: "HinhAnhTBPMerSoatSet" }[fieldName] ?? "";
        const tenOnly = options.data[{ SignNgDK: "TenUserNgDK_Ten", SignTBPNgDK: "TenUserTBPNDK_Ten", SignMer: "TenUserMerSoatSet_Ten", SignTBPMer: "TenUserTBPMerSoatSet_Ten" }[fieldName]] ?? "";
        const userName = userField ? (options.data[userField] ?? "") : "";
        const hinhAnh = hinhAnhField ? (options.data[hinhAnhField] ?? "") : "";
        if (hinhAnh) {
            const $wrapper = $("<div>").css({ display: "flex", flexDirection: "column", alignItems: "center", gap: "2px" });
            $("<img>").attr("src", `/Images/NhanVien/${hinhAnh}`).css({ width: "70px", height: "35px", objectFit: "cover", borderRadius: "3px" }).on("error", function () { $(this).hide(); }).appendTo($wrapper);
            $("<span>").text(tenOnly || userName).css({ fontSize: "11px", color: "#333", fontWeight: "bold" }).appendTo($wrapper);
            $wrapper.appendTo(containerDiv);
        } else {
            $("<span>").text(userName).css({ fontSize: "12px", color: "#333", fontWeight: "bold" }).appendTo(containerDiv);
        }
    }

    let checkSign = "";
    const signNgDK = options.data.SignNgDK ?? "";
    const signTBPNgDK = options.data.SignTBPNgDK ?? "";
    const signMer = options.data.SignMer ?? "";
    const signTBPMer = options.data.SignTBPMer ?? "";

    if (fieldName === "SignNgDK") {
        checkSign = signNgDK ? "d-none" : "";
    } else if (fieldName === "SignTBPNgDK") {
        if (!signNgDK) checkSign = "d-none";
        else checkSign = signTBPNgDK ? "d-none" : "";
    } else if (fieldName === "SignMer") {
        if (!signTBPNgDK) checkSign = "d-none";
        else checkSign = signMer ? "d-none" : "";
    } else if (fieldName === "SignTBPMer") {
        if (!signMer) checkSign = "d-none";
        else checkSign = signTBPMer ? "d-none" : "";
    }

    $("<input>")
        .attr("type", "checkbox")
        .addClass(checkSign)
        .css({
            cursor: "pointer",
            left: "4px",
            top: "5px",
            position: "absolute",
            width: "14px",
            height: "14px",
            accentColor: "#007bff"
        })
        .on("change", async function () {
            const rowData = options.data;
            const phieuTH = rowData.PhieuTH;
            const allRows = dxDataGridDanhSachDangKy.option("dataSource")
                .filter(r => r.PhieuTH === phieuTH);
            let arrSave = allRows.map(r => ({
                PhieuTH: r.PhieuTH,
                MaLenhSX: r.MaLenhSX,
                MaDH: r.MaDH,
                MaLenh: r.MaLenh,
                MaNPL: r.MaNPL,
                SLDK: r.SLDK,
                GhiChu: r.GhiChu,
                MaNhom: r.MaNhom,
                MaVTID: r.MaVTID,
                MauVTID: r.MauVTID,
                KhoVaiID: r.KhoVaiID,
                MaVT: r.MaVT,
                MauVT: r.MauVT,
                KhoVai: r.KhoVai,
                MaDVVT: r.MaDVVT,
                NgayDK: "", NgayTH: "", NgayTao: "",
                IsNPL: toIntFlag(r.IsNPL),
                NguoiTH: userNameSave,
                PhieuDK: "", SignNgDK: "", NgayKi: "",
                SignTBPNgDK: fieldName === "SignTBPNgDK" ? "checked" : "",
                NgayKiTBPNgDK: "",
                SignMer: fieldName === "SignMer" ? "checked" : "",
                NgaySignMer: "",
                SignTBPMer: fieldName === "SignTBPMer" ? "checked" : "",
                NgaySignTBPMer: "",
                PhatSinhChiPhi: r.PhatSinhChiPhi ? 1 : 0,
                TrongDinhMuc: r.TrongDinhMuc ? 1 : 0,
                NgoaiDinhMuc: r.NgoaiDinhMuc ? 1 : 0,
                MaCode: r.MaCode || '', 
            }));
            await UpdateKiTenPhieu(arrSave);
            if (fieldName === "SignTBPMer") {
                await PostCanDoiDinhMucNPL(phieuTH);
                await PostCanDoiDinhMucNPL();
                sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                    "Yêu cầu duyệt phiếu " + phieuTH, 
                    "PKH", "ALL", 1);
            } else if (fieldName === "SignMer") {
                sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                    "Yêu cầu duyệt phiếu " + phieuTH, 
                    "PKH", "ALL", -1);
            } else if (fieldName === "SignTBPNgDK") {
                sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                    "Yêu cầu duyệt phiếu " + phieuTH,  
                    "PKH", "ALL", -1);
            }
        })
        .appendTo(containerDiv);

    $("<i>")
        .addClass(`fa-solid fa-signature ${checkSign}`)
        .css({
            cursor: "pointer",
            color: "#007bff",
            fontSize: "14px",
            position: "absolute",
            top: "2px",
            right: "4px"
        })
        .on("click", function () {
            const rowData = options.data;
            const phieuTH = rowData.PhieuTH;
            showConfirmModalSign(async function () {
                const signatureImage = canvas.toDataURL('image/png');
                const allRows = dxDataGridDanhSachDangKy.option("dataSource")
                    .filter(r => r.PhieuTH === phieuTH);
                let arrSave = allRows.map(r => ({
                    PhieuTH: r.PhieuTH,
                    MaLenhSX: r.MaLenhSX,
                    MaDH: r.MaDH,
                    MaLenh: r.MaLenh,
                    MaNPL: r.MaNPL,
                    SLDK: r.SLDK,
                    GhiChu: r.GhiChu,
                    MaNhom: r.MaNhom,
                    MaVTID: r.MaVTID,
                    MauVTID: r.MauVTID,
                    KhoVaiID: r.KhoVaiID,
                    MaVT: r.MaVT,
                    MauVT: r.MauVT,
                    KhoVai: r.KhoVai,
                    MaDVVT: r.MaDVVT,
                    NgayDK: "", NgayTH: "", NgayTao: "",
                    IsNPL: r.IsNPL ? 1 : 0,
                    NguoiTH: userNameSave,
                    PhieuDK: "", SignNgDK: "", NgayKi: "",
                    SignTBPNgDK: fieldName !== "SignTBPNgDK" ? "" : signatureImage,
                    NgayKiTBPNgDK: "",
                    SignMer: fieldName !== "SignMer" ? "" : signatureImage,
                    NgaySignMer: "",
                    SignTBPMer: fieldName !== "SignTBPMer" ? "" : signatureImage,
                    NgaySignTBPMer: "",
                    PhatSinhChiPhi: r.PhatSinhChiPhi ? 1 : 0,
                    TrongDinhMuc: r.TrongDinhMuc ? 1 : 0,
                    NgoaiDinhMuc: r.NgoaiDinhMuc ? 1 : 0,
                    MaCode: r.MaCode || '',
                }));
                await UpdateKiTenPhieu(arrSave);
                if (fieldName === "SignTBPMer") {
                    await PostCanDoiDinhMucNPL(phieuTH);
                    await PostCanDoiDinhMucNPL();
                    sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                        "Yêu cầu duyệt phiếu " + phieuTH,  
                        "PKH", "ALL", 1);
                } else if (fieldName === "SignMer") {
                    sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                        "Yêu cầu duyệt phiếu " + phieuTH,  
                        "PKH", "ALL", -1);
                } else if (fieldName === "SignTBPNgDK") {
                    sendNotify(userNameSave, "M.47.00.00", "Duyệt cấp thêm",
                        "Yêu cầu duyệt phiếu " + phieuTH,
                        "KHO", "ALL", -1);
                }
                $("#signatureModal").modal("hide");
                return true;
            });
        })
        .appendTo(containerDiv);

    containerDiv.appendTo(container);
    container.addClass("position-relative");
}
//view ngoài
function createViewDxDataGridDanhSachDangKy(data) {
    dxDataGridDanhSachDangKy = $("#dxDataGridDanhSachDangKy").dxDataGrid({
        dataSource: data,
        width: '100%',
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        onCellClick: function (e) {
            if (e.rowType === "header" && e.column.dataField === "NgayTH") {

                const grid = e.component;

                const currentSort = grid.columnOption("PhieuTH", "sortOrder");
                const newSort = currentSort === "asc" ? "desc" : "asc";

                grid.columnOption("PhieuTH", {
                    sortOrder: newSort,
                    sortByGroupSummaryInfo: [{
                        summaryItem: "maxNgayTH"
                    }]
                });
            }
        },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "PhieuTH",
                caption: "Số Phiếu",
                alignment: "center",
                visible: 0,
                minWidth: 160,
                groupIndex: 0,
                sortOrder: "desc",
                sortByGroupSummaryInfo: [{
                    summaryItem: "maxNgayTH"
                }],
                //calculateGroupValue: function (rowData) {
                //    const ngay = rowData.NgayTH
                //        ? new Date(rowData.NgayTH).toISOString()
                //        : "1970-01-01T00:00:00.000Z";
                //    return ngay + "___" + (rowData.PhieuTH || "");
                //},
                groupCellTemplate: function (container, options) {
                    const items = options.data?.items || options.data?.collapsedItems || [];
                    const firstItem = items[0] || {};
                    const maLenh = firstItem.MaLenh || "";
                    const maDH = firstItem.MaDH || "";
                    const phieuTH = firstItem.PhieuTH || "";

                    $("<div>").css({
                        fontWeight: "400px",
                        color: "#000000",
                        fontSize: "13px",
                        padding: "2px 4px",
                        display: "flex",
                        gap: "16px",
                        alignItems: "center"
                    })
                        .append(
                            $("<span>").html(`Số Phiếu: ${phieuTH}`),
                            maLenh ? $("<span>").html(`| LSX: ${maLenh}`) : null,
                            maDH ? $("<span>").html(`| MH: ${maDH}`) : null
                        )
                        .appendTo(container);
                }
            },
            { dataField: "MaLenh", caption: "LSX", alignment: "center", minWidth: 100, visible: false, },
            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 150, visible: false, },
            { dataField: "IsNPL", caption: "Loại VT", alignment: "center", minWidth: 80 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "ChiTiet", caption: "Mô tả", alignment: "center", minWidth: 180 },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "IsTV", caption: "IsTV", alignment: "center", minWidth: 100, visible: false },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 120 },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
            },
            { dataField: "MauSP", caption: "Màu SP", alignment: "center", minWidth: 120 },
            { dataField: "SizeInfo", caption: "Size SP", alignment: "center", minWidth: 120 },
            {
                dataField: "CapPhat",
                caption: "SL Cấp Phát",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let result = formatNumber(value)
                    container.text(result);
                }
            },
            { dataField: "DinhMuc", caption: "Định mức cấp phát", minWidth: 80 },

            {
                dataField: "SLDKSP",
                caption: "SL SP Cấp thêm",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let result = formatNumber(value);
                    container.text(result);
                }
            },

            {
                dataField: "SLDK",
                caption: "SL VT Cấp thêm",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let result = formatNumber(value)
                    container.text(result);
                }
            },
            {
                dataField: "PhanTramCapThem",
                caption: "% Cấp thêm/Cấp phát",
                alignment: "center",
                minWidth: 130,
                allowEditing: false,
                allowSorting: false,
                calculateCellValue: function (rowData) {
                    const slCapPhat = parseFloat(rowData.CapPhat) || 0;
                    const slCapThem = parseFloat(rowData.SLDK) || 0;
                    if (slCapPhat === 0) return null;
                    return parseFloat(((slCapThem / slCapPhat) * 100).toFixed(2));
                },
                cellTemplate: function (container, options) {
                    const value = options.value;
                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }
                    /*                    const color = value > 100 ? "red" : value > 0 ? "#e67e00" : "inherit";*/
                    $("<span>")
                        .text(value + "%")
                        .css({/* color: color*//* fontWeight: "bold"*/ })
                        .appendTo(container);
                }
            },
            {
                dataField: "SLDaXuat",
                caption: "SL đã xuất",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    const value = options.value;
                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }
                    container.text(formatNumber(value));
                }
            },

            {
                dataField: "TonKho",
                caption: "SL Tồn kho",
                alignment: "center",
                minWidth: 110,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    const value = options.value;
                    if (value === null || value === undefined || value === "") {
                        container.text("");
                        return;
                    }
                    const num = parseFloat(value);
                    $("<span>")
                        .text(formatNumber(value))
                        .css({ color: num <= 0 ? "red" : "inherit" })
                        .appendTo(container);
                }
            },

            {
                dataField: "NgayTH",
                caption: "Ngày YC Cấp thêm",
                alignment: "center",
                minWidth: 150,
                allowSorting: true,         
                sortOrder: "desc",         
                dataType: "date",
                calculateSortValue: function (rowData) {
                    return rowData.NgayTH ? new Date(rowData.NgayTH).getTime() : 0;
                },
                cellTemplate: function (container, options) {
                    const dateConvert = moment(options.value).format("DD/MM/YYYY");
                    container.text(dateConvert);
                }
            },

            {
                dataField: "GhiChu",
                caption: "Lý do",
                minWidth: 200,
            },
            {
                dataField: "PhatSinhChiPhi",
                caption: "Phát sinh chi phí",
                alignment: "center",
                width: 80,
                visible: false,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    const $checkbox = $(`
            <input type="checkbox"
                   style="width: 18px; height: 18px; cursor: default;"
                   disabled />
        `);
                    $checkbox.prop("checked", options.data.PhatSinhChiPhi == 1 || options.data.PhatSinhChiPhi === true);
                    container.css({ justifyContent: "center", alignItems: "center" });
                    container.append($checkbox);
                }
            },
            {
                dataField: "TrongDinhMuc",
                caption: "Trong định mức",
                alignment: "center",
                minWidth: 100,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    const $wrap = $("<div>").css({
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center",
                        width: "100%",
                        height: "100%",
                        minHeight: "30px"
                    });
                    const $checkbox = $(`<input type="checkbox"
               style="width: 18px; height: 18px; cursor: default; flex-shrink: 0;"
               disabled />`);
                    $checkbox.prop("checked", options.data.TrongDinhMuc == 1 || options.data.TrongDinhMuc === true);
                    $wrap.append($checkbox);
                    container.empty().append($wrap);
                }
            },
            {
                dataField: "NgoaiDinhMuc",
                caption: "Ngoài định mức",
                alignment: "center",
                minWidth: 100,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    const $wrap = $("<div>").css({
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center",
                        width: "100%",
                        height: "100%",
                        minHeight: "30px"
                    });
                    const $checkbox = $(`<input type="checkbox"
               style="width: 18px; height: 18px; cursor: default; flex-shrink: 0;"
               disabled />`);
                    $checkbox.prop("checked", options.data.NgoaiDinhMuc == 1 || options.data.NgoaiDinhMuc === true);
                    $wrap.append($checkbox);
                    container.empty().append($wrap);
                }
            },
            {
                dataField: "SignNgDK",
                caption: "Người ĐK",
                minWidth: 120,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignNgDK");
                }
            },
            {
                dataField: "SignTBPNgDK",
                caption: "TBP người ĐK",
                minWidth: 120,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignTBPNgDK");
                }
            },
            {
                dataField: "SignMer",
                caption: "Mer soát sét",
                minWidth: 120,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignMer");
                }
            },
            {
                dataField: "SignTBPMer",
                caption: "TBP Mer soát sét",
                minWidth: 120,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignTBPMer");
                }
            },

            {
                caption: "Thao tác",
                width: 80,
                alignment: "center",
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    $("<button>")
                        .addClass("btn btn-danger btn-sm")
                        .html('<i class="fa-solid fa-trash"></i>')
                        .css({ padding: "3px 8px" })
                        .on("click", function () {
                            const phieuTH = options.data.PhieuTH;
                            const maNPL = options.data.MaNPL;

                            if (!phieuTH || !maNPL) {
                                showToast("warning", "Không xác định được dòng cần xóa");
                                return;
                            }

                            rowDelete = options.data;
                            rowDelete.__deleteType = "chiTiet";
                            $("#txtConfirmDeleteMsg").html(
                                `Bạn có chắc chắn muốn xóa dòng <strong>${options.data.MaVT || ''}</strong> khỏi phiếu?`
                            );

                            $("#modalComfimrtDeleteVT").modal("show");
                        })
                        .appendTo(container);
                }
            },

            //{
            //    caption: "Xóa",
            //    minWidth: 100,
            //    alignment: "center",
            //    cellTemplate: function (container, options) {

            //        if (options.data.CheckPDN != '0') {
            //            return;
            //        }

            //        const $wrapper = $(`<div></div>`).css({ display: "flex", justifyContent: "center", height: "25px", gap: "8px" })
            //        // Thêm nút
            //        const $btnEdit = $("<div>")
            //            .append(
            //                $("<i>")
            //                    .addClass("fa-solid fa-pen-to-square")
            //                    .css({ fontSize: "16px", color: "red", cursor: "pointer" })
            //                    .on("click", function () {
            //                        rowEdit = { ...options.data };
            //                        dxDataGridEditVatTu.option("dataSource", [rowEdit]);
            //                        $("#modalEditDKVT").modal("show");
            //                    })
            //            )
            //            .appendTo(container);

            //        // Thêm nút
            //        const $btnDelete = $("<div>")
            //            .append(
            //                $("<i>")
            //                    .addClass("fa-solid  fa-trash")
            //                    .css({ fontSize: "16px", color: "red", cursor: "pointer" })
            //                    .on("click", function () {
            //                        rowDelete = options.data
            //                        $("#modalComfimrtDeleteVT").modal('show')
            //                    })
            //            )
            //            .appendTo(container);

            //        $wrapper.append($btnDelete, $btnEdit).appendTo(container)
            //    }

            //}
        ],
        //summary: {
        //    totalItems: [
        //        {
        //            column: "CapPhat",
        //            name: "sumCapPhat",
        //            summaryType: "sum",
        //            showInColumn: "CapPhat",
        //            customizeText(e) {
        //                if (e.value == null) return "";

        //                let str = e.value.toString();
        //                let intPart = str;
        //                let decPart = "";

        //                if (str.includes(".")) {
        //                    [intPart, decPart] = str.split(".");
        //                    decPart = decPart.substring(0, 4); // cắt, không làm tròn
        //                }

        //                const formattedInt = Number(intPart).toLocaleString("en");
        //                return decPart ? `${formattedInt}.${decPart}` : formattedInt;
        //            }
        //        },
        //        {
        //        column: "SLDK",
        //            name: "sumSLDK",
        //            showInColumn: "SLDK",
        //        summaryType: "sum",
        //        customizeText(e) {
        //            if (e.value == null) return "";

        //            let str = e.value.toString();
        //            let intPart = str;
        //            let decPart = "";

        //            if (str.includes(".")) {
        //                [intPart, decPart] = str.split(".");
        //                decPart = decPart.substring(0, 4); // cắt, không làm tròn
        //            }

        //            const formattedInt = Number(intPart).toLocaleString("en");
        //            return decPart ? `${formattedInt}.${decPart}` : formattedInt;
        //        }
        //    }

        //    ]
        //},
        summary: {
            totalItems: [
                {
                    column: "CapPhat",
                    summaryType: "sum",
                    showInColumn: "CapPhat",
                    displayFormat: "{0}",
                    customizeText(e) {
                        if (e.value == null) return "";
                        let str = e.value.toString();
                        let [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                        if (decPart) decPart = decPart.substring(0, 4);
                        /*     const formattedInt = Number(intPart).toLocaleString("en");*/
                        const formattedInt = formatThousands(intPart);
                        return decPart ? `${formattedInt}.${decPart}` : formattedInt;
                    }
                },
                {
                    column: "SLDK",
                    summaryType: "sum",
                    showInColumn: "SLDK",
                    displayFormat: "{0}",
                    customizeText(e) {
                        if (e.value == null) return "";
                        let str = e.value.toString();
                        let [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                        if (decPart) decPart = decPart.substring(0, 4);
                        /*       const formattedInt = Number(intPart).toLocaleString("en");*/
                        const formattedInt = formatThousands(intPart);
                        return decPart ? `${formattedInt}.${decPart}` : formattedInt;
                    }
                }
            ],
            groupItems: [
                {
                    column: "NgayTH",
                    summaryType: "max",
                    name: "maxNgayTH"
                }
            ]
        },
        onContentReady: function (e) {
            //setTimeout(() => {
            //    mergeIsNPLCells();
            //}, 50);
        },

        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.dataField === "IsNPL") {
                const val = e.data?.IsNPL;
                e.cellElement.text(val == 1 ? "Nguyên Liệu" : "Phụ liệu");
            }
            if (e.column.command === "expand" && !e.column.dataField) {
                $(e.cellElement).css({
                    width: "0px",
                    minWidth: "0px",
                    maxWidth: "0px",
                    padding: "0px",
                    border: "none",
                    overflow: "hidden"
                });
            }
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        }
    }).dxDataGrid("instance");

    $("#Layer_1").click()
}
function createViewDxDataGridEditVatTu() {
    dxDataGridEditVatTu = $("#dxDataGridEditVatTu").dxDataGrid({
        dataSource: [],
        width: '100%',
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: false },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "MaDH",
                caption: "Mã ĐH",
                alignment: "center",
                minWidth: 150,
                allowEditing: false
            },
            {
                dataField: "MaLenh",
                caption: "Mã Lệnh",
                alignment: "center",
                minWidth: 100,
                allowEditing: false
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 180,
                allowEditing: false
            },
            {
                dataField: "MauVT",
                caption: "Màu VT",
                alignment: "center",
                minWidth: 120,
                allowEditing: false
            },
            {
                dataField: "KhoVai",
                caption: "Khổ/Size",
                alignment: "center",
                minWidth: 100,
                allowEditing: false
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
                allowEditing: false
            },
            {
                dataField: "CapPhat",
                caption: "SL Cấp Phát",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let result = formatNumber(value)
                    container.text(result);
                }
            },
            { dataField: "DinhMuc", caption: "Định mức cấp phát", minWidth: 80 },
            {
                dataField: "SLDKSP",
                caption: "SL SP cấp thêm",
                minWidth: 110,
                allowSorting: false,
                cellTemplate: function (container, options) {

                    const initVal = options.data.SLDKSP != null
                        ? (() => {
                            const str = options.data.SLDKSP.toString();
                            const [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                            const formatted = formatThousands(intPart);
                            return decPart ? `${formatted}.${decPart.substring(0, 4)}` : formatted;
                        })()
                        : "";

                    const $input = $(`
                        <input type="text"
                               class="form-control inputSLSP"
                               value="${initVal}"
                               style="text-align:right;" />
                    `);

                    $input.on("input", function () {
                        const el = this;
                        let raw = el.value.replace(/,/g, "").replace(/[^0-9.]/g, "");

                        const dotIndex = raw.indexOf(".");
                        if (dotIndex !== -1) {
                            raw = raw.substring(0, dotIndex + 1) + raw.substring(dotIndex + 1).replace(/\./g, "");
                        }

                        if (raw.includes(".")) {
                            const [intPart, decPart] = raw.split(".");
                            raw = intPart + "." + decPart.substring(0, 4);
                        }

                        const slsp = raw === "" ? null : parseFloat(raw);

                        options.data.SLDKSP = slsp;

                        if (slsp < 0) {
                            showToast("warning", "Số lượng không được âm");
                            el.value = "";
                            options.data.SLDKSP = null;
                            return;
                        }

                        let formatted = "";
                        if (raw !== "" && raw !== ".") {
                            const parts = raw.split(".");
                            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            formatted = parts.join(".");
                        } else {
                            formatted = raw;
                        }

                        el.value = formatted;

                        const dinhMuc = parseFloat(options.data.DinhMuc) || 0;

                        if (slsp !== null && slsp >= 0 && dinhMuc > 0) {
                            let slvt = slsp * dinhMuc;
                            slvt = Math.round(slvt * 10000) / 10000;
                            options.data.SLDK = slvt;
                            if (slsp != null) {
                                if (!isDangKyRowSelected(options.data)) {
                                    selectedRowsToSave.push(options.data);
                                }
                            } else {
                                selectedRowsToSave = selectedRowsToSave.filter(item =>
                                    !isSameDangKyRow(item, options.data)
                                );
                            }
                            // ✅ Tìm đúng trong dxDataGridEditVatTu
                            const $row = $(dxDataGridEditVatTu.getRowElement(options.rowIndex));

                            const $inputSLDK = $row.find('input.inputSLDK').first();
                            if ($inputSLDK.length) {
                                const str = slvt.toString();
                                const [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                                const formatted = formatThousands(intPart);
                                $inputSLDK.val(decPart ? `${formatted}.${decPart.substring(0, 4)}` : formatted);
                            }
                        }
                        updateSummary(dxDataGridEditVatTu);
                    });

                    $input.on("focus", function () {
                        this.select();
                    });

                    container.append($input);
                }
            },
            {
                dataField: "SLDK",
                caption: "SL Cấp thêm",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    const truncated = Math.trunc(options.data.SLDK * 10000) / 10000;
                    truncated.toString();

                    const $input = $(`
                        <input type="number"
                               class="form-control inputSLDK"
                               value="${truncated ?? ''}" />
                    `);

                    $input.on("input", function () {
                        let value = this.value;

                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng cấp thêm không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridEditVatTu);
                            return;
                        }

              
                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }
                        options.data.SLDK = value === "" ? null : Number(value);
                        updateSummary(dxDataGridEditVatTu);

         
                        const dinhMuc = parseFloat(options.data.DinhMuc) || 0;
                        if (dinhMuc > 0 && options.data.SLDK != null) {
                            let slsp = options.data.SLDK / dinhMuc;
                            slsp = Math.round(slsp * 10000) / 10000;
                            options.data.SLDKSP = slsp;

                            const $row = $(dxDataGridEditVatTu.getRowElement(options.rowIndex));
                            const $inputSLSP = $row.find('input.inputSLSP').first();
                            if ($inputSLSP.length) {
                                const str = slsp.toString();
                                const [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                                const formatted = formatThousands(intPart);
                                $inputSLSP.val(decPart ? `${formatted}.${decPart.substring(0, 4)}` : formatted);
                            }
                        } else {
                            options.data.SLDKSP = null;
                            const $row = $(dxDataGridEditVatTu.getRowElement(options.rowIndex));
                            const $inputSLSP = $row.find('input.inputSLSP').first();
                            if ($inputSLSP.length) $inputSLSP.val("");
                        }

                    });
                    container.append($input);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Lý do",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control"
                               value="${options.data.GhiChu || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.GhiChu = this.value;
                    });

                    container.append($input);
                }
            },
            {
                dataField: "PhatSinhChiPhi",
                caption: "Phát sinh chi phí",
                alignment: "center",
                width: 80,
                visible: false,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {

                    const $checkbox = $('<input type="checkbox" style="width:18px;height:18px;cursor:pointer;" />');

                    $checkbox.prop("checked", options.data.PhatSinhChiPhi == 1 || options.data.PhatSinhChiPhi === true);

                    $checkbox.on("change", function () {
                        options.data.PhatSinhChiPhi = this.checked ? 1 : 0;
                    });

                    container.css({

                        justifyContent: "center",
                        alignItems: "center"
                    });

                    container.append($checkbox);
                }
            },
            {
                dataField: "TrongDinhMuc",
                caption: "Trong định mức",
                alignment: "center",
                minWidth: 100,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    const $wrap = $("<div>").css({
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center",
                        width: "100%",
                        height: "100%",
                        minHeight: "30px"
                    });
                    const $checkbox = $(`<input type="checkbox"
            style="width: 18px; height: 18px; cursor: pointer; flex-shrink: 0;" />`);
                    $checkbox.prop("checked", options.data.TrongDinhMuc == 1 || options.data.TrongDinhMuc === true);
                    $checkbox.on("change", function () {
                        if (this.checked) {
                            options.data.TrongDinhMuc = 1;
                            options.data.NgoaiDinhMuc = 0;
                        } else {
                            options.data.TrongDinhMuc = 0;
                        }
                        dxDataGridEditVatTu.repaintRows([options.rowIndex]);
                    });
                    $wrap.append($checkbox);
                    container.empty().append($wrap);
                }
            },
            {
                dataField: "NgoaiDinhMuc",
                caption: "Ngoài định mức",
                alignment: "center",
                minWidth: 100,
                allowFiltering: false,
                allowHeaderFiltering: false,
                cellTemplate: function (container, options) {
                    const $wrap = $("<div>").css({
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center",
                        width: "100%",
                        height: "100%",
                        minHeight: "30px"
                    });
                    const $checkbox = $(`<input type="checkbox"
            style="width: 18px; height: 18px; cursor: pointer; flex-shrink: 0;" />`);
                    $checkbox.prop("checked", options.data.NgoaiDinhMuc == 1 || options.data.NgoaiDinhMuc === true);
                    $checkbox.on("change", function () {
                        if (this.checked) {
                            options.data.NgoaiDinhMuc = 1;
                            options.data.TrongDinhMuc = 0;
                        } else {
                            options.data.NgoaiDinhMuc = 0;
                        }
                        dxDataGridEditVatTu.repaintRows([options.rowIndex]);
                    });
                    $wrap.append($checkbox);
                    container.empty().append($wrap);
                }
            },
        ],
        summary: {
            totalItems: [{
                column: "SLDK",
                summaryType: "sum",
                customizeText(e) {
                    if (e.value == null) return "";

                    const value = Number(e.value);

                    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
                    const truncated = Math.trunc(value * 10000) / 10000;

                    // Bỏ số 0 dư
                    return truncated.toString();
                }
            },
            {
                column: "CapPhat",
                summaryType: "sum",
                customizeText(e) {
                    if (e.value == null) return "";

                    let str = e.value.toString();
                    let intPart = str;
                    let decPart = "";

                    if (str.includes(".")) {
                        [intPart, decPart] = str.split(".");
                        decPart = decPart.substring(0, 4);
                    }

                    /*    const formattedInt = Number(intPart).toLocaleString("en");*/
                    const formattedInt = formatThousands(intPart);
                    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
                }
            }
            ]
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css({
                    "vertical-align": "middle",
                    "border-bottom": "1px solid #ddd"
                });
            }
        }
    }).dxDataGrid("instance");
}

function createViewDxGridDanhSachPhieuDKVT() {
    dxDataPhieuDKVT = $("#dxDataPhieuDKVT").dxDataGrid({
        dataSource: [],
        rowAlternationEnabled: true,
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: false },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "PhieuDK",
                caption: "Phiếu",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const $wrapper = $(`<div></div>`).css({
                        display: "flex",
                        justifyContent: "center",
                        height: "25px",
                        gap: "8px"
                    })

                    // Text
                    const $span = $(`<span>${options.data.PhieuDK}</span>`)

                    // Icon
                    const $btnNext = $(`<i class="fa-solid fa-circle-right"></i>`)
                        .css({ fontSize: '16px', color: 'green', cursor: 'pointer' })
                        .on("click", function () {
                            $("#malenh").val(`${options.data.MaLenhSX}`).trigger("change");
                            setTimeout(() => {
                                $("#phieuxuathang").val(`${options.data.PhieuDK}`).trigger("change");
                            }, 300)

                            $("#modalTimNhanh").modal('hide');
                            $("#inputTimKiem").val('').trigger('input');
                            updateGrid(dxDataPhieuDKVT, []);
                        })

                    $wrapper.append($span, $btnNext).appendTo(container)
                }
            },
            {
                dataField: "MaLenh",
                caption: "Mã Lệnh Sản Xuất",
                width: 400,
                cellTemplate: function (container, options) {
                    const $wrapper = $(`<div></div>`).css({ display: "flex", justifyContent: "center", height: "25px", gap: "8px" })

                    // Text
                    const $span = $(`<span>${options.data.MaLenh}</span>`)

                    // Icon
                    const $btnNext = $(`<i class="fa-solid fa-circle-right"></i>`)
                        .css({ fontSize: '16px', color: 'green' })
                        .on("click", function () {
                            $("#malenh").val(`${options.data.MaLenhSX}`).trigger("change");
                            $("#modalTimNhanh").modal('hide');
                            $("#inputTimKiem").val('').trigger('input');
                            updateGrid(dxDataPhieuDKVT, []);
                        })

                    $wrapper.append($span, $btnNext).appendTo(container)

                }
            },
            {
                dataField: "MaVT",
                caption: "ItemCode",
                minWidth: 100,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                minWidth: 100,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                minWidth: 100,
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn Vị",
                minWidth: 100,
                width: 100,
            },
            { dataField: "MauSP", caption: "Màu SP", alignment: "center", minWidth: 120 },
            { dataField: "SizeInfo", caption: "Size SP", alignment: "center", minWidth: 120 },
            { dataField: "IsTV", caption: "IsTV", alignment: "center", minWidth: 100, visible: false },
            {
                dataField: "SLDK",
                caption: "SL Yêu Cầu",
                minWidth: 100,
            },
            {
                dataField: "SLNhap",
                caption: "Thực xuất",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const value = parseFloat(parseFloat(options.value || 0).toFixed(2));
                    $("<div>")
                        .text(value)
                        .addClass("thucnhap")
                        .toggleClass("text-danger", options.data.isCheckVuot === true)
                        .appendTo(container);
                }
            },
        ],

        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

                const searchTerm = $("#inputTimKiem").val().trim().toLowerCase();
                if (searchTerm && (e.column.dataField === "MaVT" || e.column.dataField === "MauVT")) {
                    const cellValue = String(e.value || "");
                    const cellValueLower = cellValue.toLowerCase();

                    if (cellValueLower.includes(searchTerm)) {
                        // Tìm vị trí bắt đầu của text match
                        const startIndex = cellValueLower.indexOf(searchTerm);
                        const endIndex = startIndex + searchTerm.length;

                        // Tạo HTML với phần match được highlight
                        const before = cellValue.substring(0, startIndex);
                        const match = cellValue.substring(startIndex, endIndex);
                        const after = cellValue.substring(endIndex);

                        const highlightedHTML = `${before}<mark style="background-color: #ffeb3b; font-weight: 600; padding: 2px 4px; border-radius: 3px;">${match}</mark>${after}`;

                        $(e.cellElement).html(highlightedHTML);
                    }
                }
            }
        },
    }).dxDataGrid("instance");

    $("#Layer_1").click()
}
function showConfirmModalSign(onConfirm) {
    $('#saveBtn').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();
            if (result === false) {
                return;
            }
        }
        $("#signatureModal").modal("hide");
    });
    $("#signatureModal").modal("show");
}

let canvas, ctx;
let isDrawing = false;
let penColor = '#000000';
let penSize = 2;
let signatureHistory = [];
let currentStroke = [];

$(document).ready(function () {
    canvas = document.getElementById('signatureCanvas');
    ctx = canvas.getContext('2d');

    // Khởi tạo kích thước canvas
    initCanvas();

    // Cấu hình canvas
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    // Sự kiện chuột
    $(canvas).on('mousedown', startDrawing);
    $(canvas).on('mousemove', draw);
    $(canvas).on('mouseup', stopDrawing);
    $(canvas).on('mouseleave', stopDrawing);

    // Sự kiện chạm (mobile)
    canvas.addEventListener('touchstart', handleTouchStart, { passive: false });
    canvas.addEventListener('touchmove', handleTouchMove, { passive: false });
    canvas.addEventListener('touchend', stopDrawing, { passive: false });

    //Ký nhanh
    $(document).on("change", "#chkXacNhanNhanh", async function () {
        if (!this.checked) return;
        const fakeStroke = [{ x: 0, y: 0, color: "#000", size: 2 }];
        signatureHistory.push(fakeStroke); 
        const originalToDataURL = canvas.toDataURL.bind(canvas);
        canvas.toDataURL = () => "checked";

        $("#saveBtn").trigger("click");
        setTimeout(() => {
            canvas.toDataURL = originalToDataURL;
            signatureHistory.pop();
            $("#chkXacNhanNhanh").prop("checked", false);
        }, 100);
    });

    // Reset checkbox khi mở/đóng modal
    $('#signatureModal').on('shown.bs.modal', function () {
        $("#chkXacNhanNhanh").prop("checked", false);
    });




    // Cập nhật màu từ color picker
    $('#penColor').on('change', function () {
        penColor = $(this).val();
    });

    // Reset canvas khi mở modal
    $('#signatureModal').on('shown.bs.modal', function () {
        initCanvas();
        clearSignature();
    });

    // Resize canvas khi thay đổi kích thước màn hình
    $(window).on('resize', function () {
        if ($('#signatureModal').hasClass('show')) {
            initCanvas();
        }
    });
});

function initCanvas() {
    const container = canvas.parentElement;
    const containerWidth = container.offsetWidth;

    // Xác định kích thước canvas dựa trên màn hình
    if (window.innerWidth < 768) {
        canvas.width = Math.min(containerWidth - 40, 500);
        canvas.height = 250;
    } else {
        canvas.width = Math.min(containerWidth - 40, 700);
        canvas.height = 300;
    }

    // Cấu hình lại context sau khi thay đổi kích thước
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
}

function getMousePos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
    };
}

function getTouchPos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.touches[0].clientX - rect.left) * scaleX,
        y: (e.touches[0].clientY - rect.top) * scaleY
    };
}

function startDrawing(e) {
    isDrawing = true;
    const pos = getMousePos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!isDrawing) return;

    const pos = getMousePos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function stopDrawing() {
    if (isDrawing && currentStroke.length > 0) {
        signatureHistory.push([...currentStroke]);
        currentStroke = [];
    }
    isDrawing = false;
}

function handleTouchStart(e) {
    e.preventDefault();
    isDrawing = true;
    const pos = getTouchPos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function handleTouchMove(e) {
    e.preventDefault();
    if (!isDrawing) return;

    const pos = getTouchPos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function clearSignature() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    signatureHistory = [];
    currentStroke = [];
}

function undoSignature() {
    if (signatureHistory.length === 0) return;

    signatureHistory.pop();
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Vẽ lại tất cả nét còn lại
    signatureHistory.forEach(stroke => {
        if (stroke.length === 0) return;

        ctx.beginPath();
        ctx.moveTo(stroke[0].x, stroke[0].y);

        stroke.forEach((point, index) => {
            if (index === 0) return;
            ctx.strokeStyle = point.color;
            ctx.lineWidth = point.size;
            ctx.lineTo(point.x, point.y);
            ctx.stroke();
        });
    });
}

function setPenColor(color) {
    penColor = color;
    $('#penColor').val(color);
}

function setPenSize(size) {
    penSize = size;
    $('.pen-size-btn').removeClass('active');
    $(`.pen-size-btn[data-size="${size}"]`).addClass('active');
}
async function GetChungLoaiSelect() {

    const $maLenhSelect = $("#chungloaiSelect");

    // Destroy select2 trước khi khởi tạo lại
    if ($maLenhSelect.hasClass("select2-hidden-accessible")) {
        $maLenhSelect.select2("destroy");
    }

    $maLenhSelect.empty().append(`<option ></option>`);

    if (!Array.isArray(chungLoaiThuVienCache) || chungLoaiThuVienCache.length === 0) {
        const data = await API.Get("GETCHUNGLOAI");
        chungLoaiThuVienCache = Array.isArray(data) ? data : [];
    }
    const data = chungLoaiThuVienCache;
    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaCLVT}">${x.ChungLoaiVatTu}</option>`).join('');
        $maLenhSelect.append(html);
    }

    $maLenhSelect.select2({
        dropdownParent: $("#modalThuVien")
    });
}
async function showModalThuVien() {
    resetThuVienModalState();
    await GetChungLoaiSelect();
    $("#modalThuVien").modal("show");
}


//async function loadChungLoai() {
//    const maLenh = $("#chungloaiSelect").val();

//    const data = await API.Get("GETTHUVIEN", { para1:maLenh});

//    if (data.length > 0) {
//        initFloatingTagBox();
//        createViewdxDataGridThuVien(data);
//        autoSelectSingleMau(data);
//    }
//}

async function loadChungLoai() {
    const maCLVT = $("#chungloaiSelect").val();

    if (!maCLVT) {
        if (dxDataGridThuVien) {
            dxDataGridThuVien.option("dataSource", []);
            dxDataGridThuVien.refresh();
        }
        return;
    }

    await loadSoLuong();
    const data = await API.Get("GETTHUVIEN", { para1: maCLVT });
    cacheSoLuong = dataSoLuong || [];

    if (data.length > 0) {
        initFloatingTagBox();
        if (dxDataGridThuVien) {
            dxDataGridThuVien.option("dataSource", data);
            dxDataGridThuVien.refresh();
        } else {
            createViewdxDataGridThuVien(data);
        }
        autoSelectSingleMau(data);
    } else if (dxDataGridThuVien) {
        dxDataGridThuVien.option("dataSource", []);
        dxDataGridThuVien.refresh();
    }
}

function createViewdxDataGridThuVien(data) {
    dxDataGridThuVien = $("#dxDataGridThuVien").dxDataGrid({
        dataSource: data,
        width: '100%',
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "",
        repaintChangesOnly: true,
        scrolling: {
            mode: 'virtual',
            rowRenderingMode: 'virtual'
        },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: true,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                caption: "",
                width: 50,
                alignment: "center",
                cellTemplate: function (container, options) {
                    const uniqueKey = `${options.data.MaLenhSanXuat}_${options.data.MaNPL}`;
                    const isChecked = isThuVienRowSelected(options.data);

                    const $checkbox = $(`
                        <input type="checkbox" 
                               class="row-checkbox" 
                               data-row-index="${options.rowIndex}"
                               ${isChecked ? 'checked' : ''}
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkbox.on("change", function () {
                        if (this.checked) {
                            const existsInSave = selectedRowsToSave.some(item =>

                                item.MaNPL === options.data.MaNPL
                            );

                            if (existsInSave) {
                                showToast("warning", "Vật tư này đã có trong danh sách, không thể chọn thêm!");
                                // Uncheck lại checkbox
                                this.checked = false;
                                return; // Dừng, không xử lý gì thêm
                            }
                        }
                        const dataSource = dxDataGridThuVien.option("dataSource");

                        // Lấy index hiện tại của row này
                        const index = dataSource.findIndex(item =>
                            item.MaLenhSanXuat === options.data.MaLenhSanXuat &&
                            item.MaNPL === options.data.MaNPL
                        );

                        if (this.checked) {
                            // Thêm vào danh sách đã chọn
                            if (!isThuVienRowSelected(options.data)) {
                                selectedItemsChungLoai.push(options.data);
                            }


                        } else {
                            // Xóa khỏi danh sách đã chọn
                            selectedItemsChungLoai = selectedItemsChungLoai.filter(item =>
                                !(item.MaLenhSanXuat === options.data.MaLenhSanXuat &&
                                    item.MaNPL === options.data.MaNPL)
                            );

                            // ❌ Đẩy xuống cuối

                        }

                        //    // Cập nhật lại dataSource để grid re-render đúng thứ tự
                        dxDataGridThuVien.option("dataSource", [...dataSource]);
                        dxDataGridThuVien.refresh();
                        setThuVienRowHighlight($(this).closest("tr.dx-data-row"), this.checked);
                    });

                    container.append($checkbox);

                },
                headerCellTemplate: function (container) {
                    const $checkAll = $(`
                        <input type="checkbox" 
                               id="checkAllRows" 
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkAll.on("change", function () {
                        const isChecked = this.checked;
                        const dataSource = dxDataGridThuVien.option("dataSource");

                        if (isChecked) {
                            // Chọn tất cả
                            selectedRowsToDelete = [...dataSource];
                        } else {
                            // Bỏ chọn tất cả
                            selectedRowsToDelete = [];
                        }

                        // Update tất cả checkbox trong grid
                        $('.row-checkbox').prop('checked', isChecked);
                    });

                    container.append($checkAll);
                }
            },
            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MaLenh", caption: "Mã Lệnh", alignment: "center", minWidth: 100, visible: false },
            { dataField: "LoaiVT", caption: "Loại vật tư", alignment: "center", minWidth: 100, visible: false },
            { dataField: "ChungLoaiVatTu", caption: "Chủng loại vật tư", alignment: "center", minWidth: 180, visible: false },
            {
                dataField: "ChungLoaiChiTiet",
                caption: "Chủng loại chi tiết",
                alignment: "center",
                minWidth: 200,
                cellTemplate: function (container, cellOptions) {
                    return createChungLoaiSelectBoxTemplate(
                        chungLoaiChiTietOptions,
                        "Chọn chi tiết..."
                    )(container, cellOptions);
                }
            },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 120 },
            { dataField: "IsTV", caption: "IsTV", alignment: "center", minWidth: 100, visible: false },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "MauSP",
                caption: "Màu SP",
                alignment: "center",
                minWidth: 160,
                cellTemplate: createLazyTagBoxTemplate("MauSP", mauSPOptions, "Chọn màu SP..."),
                calculateCellValue: rowData => Array.isArray(rowData.MauSP)
                    ? rowData.MauSP.map(v => typeof v === "object" ? v.TenMau : v).join(", ")
                    : ""
            },
            {
                dataField: "SizeInfo",
                caption: "Size SP",
                alignment: "center",
                minWidth: 200,
                cellTemplate: createLazyTagBoxTemplate("SizeInfo", sizeInfoOptions, "Chọn size..."),
                calculateCellValue: rowData => Array.isArray(rowData.SizeInfo)
                    ? rowData.SizeInfo.map(v => typeof v === "object" ? v.TenSize : v).join(", ")
                    : ""
            },
            {
                dataField: "DinhMuc",
                caption: "Định mức",
                alignment: "center",
                width: 70,
                cellTemplate: function (container, options) {
                    const $input = $(`<input type="text" class="form-control input-dinhmuc" 
                            value="${options.data.DinhMuc ?? ''}" />`);
                    $input.on("input", function () {
                        let value = this.value.replace(",", ".").replace(/[^0-9.]/g, "");
                        const firstDot = value.indexOf(".");
                        if (firstDot !== -1) {
                            value = value.slice(0, firstDot + 1) + value.slice(firstDot + 1).replace(/\./g, "");
                        }
                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            value = intPart + "." + decPart.substring(0, 4);
                        }
                        this.value = value;

                        let dm = parseFloat(value);
                        if (Number.isNaN(dm)) dm = 0;
                        options.data.DinhMuc = dm;
                    });

                    $input.on("keydown", function (e) {
                        if (e.key === "Enter") {
                            e.preventDefault();
                            this.blur();
                        }
                    });

                    $input.on("blur", async function () {
                        await reloadSoLuongWhenSelectSize(options.data);
                        dxDataGridThuVien.refresh();
                    });

                    container.append($input);
                }
            },
            {
                dataField: "SLNhap",
                caption: "SL Xuất",
                alignment: "center",
                minWidth: 120,
                visible: false,
                allowSorting: false,
                headerCellTemplate: function (header, info) {
                    const $container = $("<div></div>").css({
                        width: "100%",
                        display: "flex",
                        alignItems: "center",
                        gap: "14px",
                        padding: "0 8px"
                    });
                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-circle-arrow-right")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            flexShrink: "0",
                            marginRight: "6px",
                        })
                        .on("click", function (e) {
                            e.stopPropagation();
                            const dataSource = dxDataGridThuVien.option("dataSource");
                            dataSource.forEach(item => {
                                item.SLDK = Math.trunc((item.SLNhap ?? 0) * 10000) / 10000
                                refresh = true;
                            });

                            if (refresh) {
                                dxDataGridThuVien.refresh();
                            }
                        });
                    const $text = $("<span></span>")
                        .text(info.column.caption)
                        .css({
                            flexShrink: "0"
                        });
                    $container.append($text, $icon);
                    header.append($container);
                },
                cellTemplate: function (container, options) {
                    let value = options.value;
                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    const $wrapper = $("<div></div>").css({
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        gap: "6px"
                    });

                    const $text = $("<span></span>").text(formatNumber(value));

                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-circle-arrow-right")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            color: "#2196F3"
                        })
                        .on("click", function () {
                            options.data.SLDK = Math.trunc((value ?? 0) * 10000) / 10000;
                            dxDataGridThuVien.refresh();
                        });

                    $wrapper.append($text, $icon);
                    container.append($wrapper);
                }
            },
            {
                dataField: "SoLuong",
                caption: "Số lượng",
                alignment: "center",
                width: 70,
                visible: true,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value || 0));
                }
            },
            {
                dataField: "CapPhat",
                caption: "Cấp Phát",
                alignment: "center",
                minWidth: 80,
                visible: true,
                cellTemplate: function (container, options) {
                    container.append(formatNumber(options.value))
                }
            },
            {
                dataField: "SLDK",
                caption: "SL Cấp thêm",
                alignment: "center",
                minWidth: 120,
                allowSorting: false,
                headerCellTemplate: function (header, info) {
                    const $container = $("<div></div>").css({
                        width: "100%",
                        display: "flex",
                        alignItems: "center",
                        gap: "14px",
                        padding: "0 8px"
                    });
                    const $icon = $("<i></i>")
                        .addClass("fa-light fa-rotate")
                        .css({
                            cursor: "pointer",
                            fontSize: "14px",
                            flexShrink: "0",
                            marginRight: "6px",
                        })
                        .on("click", function (e) {
                            e.stopPropagation();
                            const dataSource = dxDataGridThuVien.option("dataSource");
                            dataSource.forEach(item => {
                                item.SLDK = null
                                refresh = true;
                            });

                            if (refresh) {
                                // Refresh grid để hiển thị dữ liệu mới
                                dxDataGridThuVien.refresh();
                            }
                        });
                    const $text = $("<span></span>")
                        .text(info.column.caption)
                        .css({
                            flexShrink: "0"
                        });
                    $container.append($text, $icon);
                    header.append($container);
                },
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="number"
                                class="form-control inputSLDK input-sldk"
                                value="${options.data.SLDK ?? ''}" />
                    `);

                    $input.on("input", function () {
                        let value = this.value;

                        // Không cho số âm
                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridThuVien);
                            return;
                        }

                        // Chỉ cho tối đa 4 số thập phân 
                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }

                        //if (Number(value) > options.data.SLNhap) {
                        //    showToast("warning", "Số lượng đăng ký không được lớn hơn số lượng cấp phát");

                        //    const truncated = Math.trunc(options.data.SLNhap * 10000) / 10000;

                        //    this.value = truncated;
                        //    options.data.SLDK = truncated;
                        //    updateSummary(dxDataGridThuVien);
                        //    return;
                        //}

                        options.data.SLDK = value === "" ? null : Number(value);
                        updateSummary(dxDataGridThuVien); clearTimeout(options.data.__refreshTimer);
                        options.data.__refreshTimer = setTimeout(function () {
                            dxDataGridDangKyVatTu.repaintRows([options.rowIndex]);
                        }, 300);
                    });

                    container.append($input);
                }
            },
            {
                dataField: "SLDaXuat",
                caption: "SL đã xuất",
                alignment: "center",
                minWidth: 120,
                allowEditing: false,
                visible: false
            },
            {
                dataField: "GhiChu",
                caption: "Lý do",
                minWidth: 150
            }

        ],
        summary: {
            totalItems: [
                {
                    column: "CapPhat",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                },
                {
                    column: "SLDK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                }
            ]
        },
        onRowPrepared: function (e) {
            if (e.rowType !== "data") return;
            const isCheckedRow = isThuVienRowSelected(e.data);
            setThuVienRowHighlight(e.rowElement, isCheckedRow);
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
                setThuVienRowHighlight(e.rowElement, isThuVienRowSelected(e.data));
            }
        }
    }).dxDataGrid("instance");
}
//function InsertThuVien() {
//    if (!selectedItemsChungLoai || selectedItemsChungLoai.length === 0) {
//        showToast("warning", "Vui lòng chọn ít nhất một vật tư!");
//        return;
//    }

//    const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];
//    const maDHNhap = getManualMaDH();
//    const maLenhSanXuatCurrent = $("#malenhdangkyvattu").val() || "";
//    const maLenhDisplayCurrent = ($("#malenhdangkyvattu option:selected").text() || "").trim();
//    const previouslySelectedRows = [...selectedRowsToSave];
//    const insertedItems = [];

//    selectedItemsChungLoai.forEach(function (item) {
//        // Kiểm tra trùng theo MaNPL (đã tồn tại trong lưới DangKyVatTu chưa)
//        const alreadyInGrid = dataSource.some(row => row.MaNPL === item.MaNPL);
//        if (alreadyInGrid) {
//            showToast("warning", `Vật tư ${item.MaNPL} đã tồn tại trong danh sách đăng ký!`);
//            return; // bỏ qua item này
//        }

//        const newItem = {
//            ...item,
//            MaLenhSanXuat: item.MaLenhSanXuat || maLenhSanXuatCurrent,
//            MaLenh: item.MaLenh || maLenhDisplayCurrent,
//            MaDH: maDHNhap || item.MaDH,
//            GhiChu: '',
//            __thuVienImportId: `TV_${Date.now()}_${Math.random().toString(16).slice(2)}`
//        };
//        insertedItems.push(newItem);

//        // Chèn lên đầu dataSource (trước các row chưa check)
//        dataSource.unshift(newItem);
//    });

//    // Gán selected trước khi sort để dòng lấy từ thư viện được tích + đẩy lên trên ngay
//    selectedRowsToSave = [...previouslySelectedRows, ...insertedItems];
//    const sortedDataSource = sortDangKyDataSource(dataSource);
//    const selectedBaseKeys = new Set(selectedRowsToSave.map(getDangKyRowBaseKey));
//    selectedRowsToSave = sortedDataSource.filter(row => {
//        return selectedBaseKeys.has(getDangKyRowBaseKey(row));
//    });
//    dxDataGridDangKyVatTu.option("dataSource", sortedDataSource);
//    dxDataGridDangKyVatTu.refresh();

//    // Reset selectedItemsChungLoai và uncheck lưới ThuVien
//    selectedItemsChungLoai = [];
//    const thuVienSource = dxDataGridThuVien.option("dataSource");
//    dxDataGridThuVien.option("dataSource", [...thuVienSource]);
//    $("#modalThuVien").modal("hide")
//}
function InsertThuVien() {
    if (!selectedItemsChungLoai || selectedItemsChungLoai.length === 0) {
        showToast("warning", "Vui lòng chọn ít nhất một vật tư!");
        return;
    }

    // ✅ Validate các trường bắt buộc trước khi insert
    for (const item of selectedItemsChungLoai) {
        const tenVT = item.MaVT || item.MaNPL || "";

        // Kiểm tra Chủng loại chi tiết
        const hasChungLoai = item.ChungLoaiChiTiet &&
            (typeof item.ChungLoaiChiTiet === "object"
                ? item.ChungLoaiChiTiet.MaNhom || item.ChungLoaiChiTiet.TenNhom
                : String(item.ChungLoaiChiTiet).trim() !== "");
        if (!hasChungLoai) {
            showToast("warning", `Vật tư [${tenVT}] chưa chọn Chủng loại chi tiết!`);
            PlayAudioError();
            return;
        }

        // Kiểm tra Màu SP
        const hasMauSP = Array.isArray(item.MauSP)
            ? item.MauSP.length > 0
            : (item.MauSP && String(item.MauSP).trim() !== "");
        if (!hasMauSP) {
            showToast("warning", `Vật tư [${tenVT}] chưa chọn Màu SP!`);
            PlayAudioError();
            return;
        }

        // Kiểm tra Size SP
        const hasSizeInfo = Array.isArray(item.SizeInfo)
            ? item.SizeInfo.length > 0
            : (item.SizeInfo && String(item.SizeInfo).trim() !== "");
        if (!hasSizeInfo) {
            showToast("warning", `Vật tư [${tenVT}] chưa chọn Size SP!`);
            PlayAudioError();
            return;
        }
    }

    const dataSource = dxDataGridDangKyVatTu.option("dataSource") || [];
    const maDHNhap = getManualMaDH();
    const maLenhSanXuatCurrent = $("#malenhdangkyvattu").val() || "";
    const maLenhDisplayCurrent = ($("#malenhdangkyvattu option:selected").text() || "").trim();
    const previouslySelectedRows = [...selectedRowsToSave];
    const insertedItems = [];

    const firstRow = dataSource.length > 0 ? dataSource[0] : null;
    const defaultMaLenhSanXuat = firstRow?.MaLenhSanXuat || maLenhSanXuatCurrent;
    const defaultMaLenh = firstRow?.MaLenh || maLenhDisplayCurrent;
    const defaultMaDH = firstRow?.MaDH || maDHNhap;

    selectedItemsChungLoai.forEach(function (item) {
        const alreadyInGrid = dataSource.some(row => row.MaNPL === item.MaNPL);
        if (alreadyInGrid) {
            showToast("warning", `Vật tư ${item.MaNPL} đã tồn tại trong danh sách đăng ký!`);
            return;
        }
        const newItem = {
            ...item,
            MaLenhSanXuat: defaultMaLenhSanXuat,
            MaLenh: defaultMaLenh,
            MaDH: defaultMaDH,
            GhiChu: '',
            __thuVienImportId: `TV_${Date.now()}_${Math.random().toString(16).slice(2)}`
        };
        insertedItems.push(newItem);
        dataSource.unshift(newItem);
    });

    selectedRowsToSave = [...previouslySelectedRows, ...insertedItems];
    const sortedDataSource = sortDangKyDataSource(dataSource);
    const selectedBaseKeys = new Set(selectedRowsToSave.map(getDangKyRowBaseKey));
    selectedRowsToSave = sortedDataSource.filter(row => {
        return selectedBaseKeys.has(getDangKyRowBaseKey(row));
    });
    dxDataGridDangKyVatTu.option("dataSource", sortedDataSource);
    dxDataGridDangKyVatTu.refresh();

    selectedItemsChungLoai = [];
    const thuVienSource = dxDataGridThuVien.option("dataSource");
    dxDataGridThuVien.option("dataSource", [...thuVienSource]);
    $("#modalThuVien").modal("hide");
}
async function PostCanDoiDinhMucNPL(phieuTH) {
    const dataSource = phieuTH
        ? lstDataDangKyVatTu.filter(item => item.PhieuTH === phieuTH) 
        : lstDataDangKyVatTu;

    if (!dataSource || dataSource.length === 0) {
        showToast("warning", "Không có dữ liệu");
        PlayAudioError();
        return;
    }

    const refItem = dataSource.find(item => item.MaLenhSX && item.MaLenhSX.trim() !== '');
    const maLenh = refItem?.MaLenhSX || '';

    if (!maLenh) {
        showToast("warning", "Không xác định được mã lệnh");
        PlayAudioError();
        return;
    }

    const arrSave = dataSource.map(item => ({
        PhieuTH: item.PhieuTH,
        MaLenhSX: item.MaLenhSX || maLenh,
        MaDH: item.MaDH,
        MaLenh: item.MaLenh,
        MaNPL: item.MaNPL,
        SLDK: item.SLDK,
        GhiChu: item.GhiChu,
        MaNhom: item.MaNhom,
        MaVTID: item.MaVTID,
        MauVTID: item.MauVTID,
        KhoVaiID: item.KhoVaiID,
        MaVT: item.MaVT,
        MauVT: item.MauVT,
        KhoVai: item.KhoVai,
        MaDVVT: item.MaDVVT,
        NgayDK: item.NgayDK,
        NgayTH: item.NgayTH,
        NgayTao: new Date(),
        IsNPL: toIntFlag(item.IsNPL, isNPL),
        NguoiTH: userNameSave,
        PhieuDK: "",
        SignNgDK: "", NgayKi: "",
        SignTBPNgDK: "", NgayKiTBPNgDK: "",
        SignMer: "", NgaySignMer: "",
        SignTBPMer: "", NgaySignTBPMer: "",
        IsTV: item.IsTV
    }));

    await API.Post("PostDNCT", "PostCanDoiDinhMucNPL", arrSave, "Cập nhật thành công", { para1: maLenh });
}

async function loadOptionsMauSize() {
    const maLenh = $("#malenhdangkyvattu").val();

    const data = await API.Get("GETBANGMAU", { para1: maLenh });

    if (data.length > 0) {

        // Sửa lại mauSPOptions — value lưu JSON string của object
        mauSPOptions = data.map(item => ({
            value: item.MaMau,
            text: item.TenMau
        }));

    }
    const dataSize = await API.Get("GETBANGSIZE", { para1: maLenh });
    if (dataSize.length > 0) {
        sizeInfoOptions = dataSize.map(item => ({
            // value lưu cả object
            value: JSON.stringify({
                MaNhomSize: item.MaNhomSize,
                NhomSize: item.NhomSize,
                MaSize: item.MaSize,
                TenSize: item.TenSize
            }),
            text: `${item.NhomSize}: ${item.TenSize} `, // Hiển thị: "2XL (REG)"
            // Giữ lại raw data để dùng nếu cần
            raw: item
        }));

    }
}

// ====== KHỞI TẠO 1 LẦN DUY NHẤT ======
let $floatingTagBox = null;
let floatingTagBoxInstance = null;
let activeCell = null; // { data, fieldName, $displayEl }

function initFloatingTagBox() {
    if ($floatingTagBox && floatingTagBoxInstance) {
        return;
    }

    // Tạo container floating gắn vào body
    $floatingTagBox = $("<div id='floatingTagBoxWrap'>").css({
        position: "fixed",
        zIndex: 99999,
        display: "none",
        background: "white",
        border: "1px solid #ddd",
        borderRadius: "6px",
        //boxShadow: "0 4px 16px rgba(0,0,0,0.18)",
        padding: "6px",
        minWidth: "220px"
    }).appendTo("body");

    floatingTagBoxInstance = $("<div>").appendTo($floatingTagBox).dxTagBox({
        items: [],
        valueExpr: "value",
        displayExpr: "text",
        searchEnabled: true,
        showSelectionControls: true,
        applyValueMode: "instantly",
        maxDisplayedTags: 3,
        width: "100%",
        opened: true,           // Luôn mở sẵn khi hiện
        deferRendering: false,
        onValueChanged: function (e) {
            //if (!activeCell) return;
            //saveActiveCell();
            //$floatingTagBox.hide();
            //activeCell = null;
            tempSelectedValue = e.value;
            if (activeCell && activeCell.fieldName === "SizeInfo") {
                saveActiveCell();
            }
        }
    }).dxTagBox("instance");



    // Click ra ngoài → đóng
    $(document).on("click.floatingTagBox", function (e) {
        if (!$(e.target).closest("#floatingTagBoxWrap, .dx-tagbox-popup-wrapper, .dx-overlay-wrapper").length) {
            $floatingTagBox.hide();
            activeCell = null;
        }
    });
}
$(document).off("click.floatingTagBox").on("click.floatingTagBox", function (e) {
    if (!$floatingTagBox || !$floatingTagBox.is(":visible")) return;

    const clickedInside =
        $(e.target).closest("#floatingTagBoxWrap").length > 0 ||
        $(e.target).closest(".dx-overlay-wrapper").length > 0 ||
        $(e.target).closest(".dx-tagbox-popup-wrapper").length > 0;

    if (!clickedInside) {
        closeFloatingTagBox();
    }
});
function saveActiveCell() {
    if (!activeCell) return;

    activeCell.data[activeCell.fieldName] = tempSelectedValue.map(v => {
        try { return typeof v === "string" ? JSON.parse(v) : v; }
        catch { return v; }
    });

    if (activeCell.fieldName === "SizeInfo") {
        reloadSoLuongWhenSelectSize(activeCell.data);
    } else if (activeCell.fieldName === "MauSP") {
        updateSoLuong(activeCell.data);
    }

    let displayText = "";

    if (activeCell.fieldName === "SizeInfo") {
        // Group theo MaNhomSize
        const grouped = {};
        activeCell.data[activeCell.fieldName].forEach(v => {
            const obj = typeof v === "object" ? v : {};
            const nhom = obj.MaNhomSize || "Khác";
            const size = obj.TenSize || v;
            if (!grouped[nhom]) grouped[nhom] = [];
            grouped[nhom].push(size);
        });
        // Format: "REG: S,M,L; BIG: XL,2XL"
        displayText = Object.entries(grouped)
            .map(([nhom, sizes]) => `${nhom}: ${sizes.join(", ")}`)
            .join("; ");

    } else if (activeCell.fieldName === "MauSP") {
        // ✅ Tra lại text từ mauSPOptions theo value (JSON string)
        displayText = tempSelectedValue.map(v => {
            // v là JSON string {"MaMau":"...","TenMau":"..."}
            const found = mauSPOptions.find(opt => opt.value === v);
            if (found) return found.text;

            // Fallback: parse trực tiếp nếu không tìm thấy trong options
            try {
                const parsed = typeof v === "string" ? JSON.parse(v) : v;
                return parsed.TenMau || v;
            } catch {
                return v;
            }
        }).join(", ");
    }

    activeCell.$displayEl.text(displayText || activeCell.placeholder);
    activeCell.$displayEl.css("color", displayText ? "#333" : "#aaa");
}

function closeFloatingTagBox() {
    saveActiveCell(); // ✅ Luôn lưu trước khi đóng
    $floatingTagBox.hide();
    activeCell = null;
}
function openFloatingTagBox(options, currentValue, cellData, fieldName, $displayEl, placeholder, $anchor) {
    floatingTagBoxInstance.close();
    floatingTagBoxInstance.option("items", options);
    floatingTagBoxInstance.option("value", currentValue.map(v =>
        typeof v === "object" ? JSON.stringify(v) : v
    ));

    // ✅ Sync tempSelectedValue với giá trị hiện tại của cell
    tempSelectedValue = currentValue.map(v =>
        typeof v === "object" ? JSON.stringify(v) : v
    );

    activeCell = { data: cellData, fieldName, $displayEl, placeholder };

    const offset = $anchor.offset();
    const winHeight = $(window).height();
    const dropdownHeight = 320;
    let top = offset.top + $anchor.outerHeight() + 2;
    if (top + dropdownHeight > winHeight) {
        top = offset.top - dropdownHeight - 2;
    }

    $floatingTagBox.css({
        top: top,
        left: offset.left,
        minWidth: Math.max($anchor.outerWidth(), 220)
    }).show();

    setTimeout(() => {
        floatingTagBoxInstance.open();
        floatingTagBoxInstance.focus();
    }, 0);
}
function createLazyTagBoxTemplate(fieldName, optionsList, placeholder) {
    return function (container, cellOptions) {
        const val = cellOptions.data[fieldName];
        const displayText = Array.isArray(val) && val.length
            ? val.map(v => typeof v === "object" ? (v.TenSize || v.TenMau || v) : v).join(", ")
            : "";

        // Chỉ render DIV + text — cực nhẹ, không có JS nặng
        const $cell = $("<div>").css({
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            padding: "4px 8px",
            border: "1px solid #ddd",
            borderRadius: "4px",
            cursor: "pointer",
            minHeight: "32px",
            background: "white",
            userSelect: "none"
        });

        const $text = $("<span>")
            .text(displayText || placeholder)
            .css({
                flex: 1,
                overflow: "hidden",
                textOverflow: "ellipsis",
                whiteSpace: "nowrap",
                fontSize: "13px",
                color: displayText ? "#333" : "#aaa"
            });

        const $icon = $("<i>").addClass("fa-light fa-chevron-down").css({
            fontSize: "11px", color: "#999", marginLeft: "4px", flexShrink: 0
        });

        $cell.append($text, $icon);
        container.append($cell);

        // Click → mở floating TagBox dùng chung
        $cell.on("click", function (e) {
            e.stopPropagation();
            const currentVal = Array.isArray(cellOptions.data[fieldName])
                ? cellOptions.data[fieldName] : [];
            openFloatingTagBox(optionsList, currentVal, cellOptions.data, fieldName, $text, placeholder, $cell);
        });
    };
}
// Sửa lại autoSelectSingleMau — lưu object thay vì string
function autoSelectSingleMau(dataSource) {
    if (mauSPOptions.length !== 1) return;

    // ✅ Lưu object { MaMau, TenMau } thay vì chỉ MaMau
    const autoObj = {
        MaMau: mauSPOptions[0].value,
        TenMau: mauSPOptions[0].text
    };

    dataSource.forEach(item => {
        if (!Array.isArray(item.MauSP) || item.MauSP.length === 0) {
            item.MauSP = [autoObj];
        }
    });

    dxDataGridThuVien.option("dataSource", [...dataSource]);
}

function GetChungLoaiChiTiet(maCLVT) {
    let result = [];

    $.ajax({
        url: `/api/DeNghiCapThemNPL/Get?action=GETCLCT_2&para1=${maCLVT}`,
        method: "GET",
        async: false, // <--- Quan trọng: Chuyển sang đồng bộ
        success: function (data) {
            if (data && data.length > 0) {
                result = data.map(item => ({
                    value: JSON.stringify({ MaCLCT: item.MaNhom, TenCLCT: item.TenNhom }),
                    text: item.TenNhom,
                    original: item
                }));
            }
        },
        error: function (err) {
            console.error("Lỗi API GetChungLoaiChiTiet:", err);
        }
    });

    return result;
}

function createChungLoaiSelectBoxTemplate(optionsList, placeholder) {
    return function (container, cellOptions) {
        const fieldName = "ChungLoaiChiTiet";
        const val = cellOptions.data[fieldName];

        const displayText = val ? (val.TenNhom || val) : "";

        const $cell = $("<div>").css({
            display: "flex", alignItems: "center", justifyContent: "space-between",
            padding: "4px 8px", border: "1px solid #ddd", borderRadius: "4px",
            cursor: "pointer", minHeight: "32px", background: "white"
        });

        const $text = $("<span>").text(displayText || placeholder).css({
            flex: 1, overflow: "hidden", textOverflow: "ellipsis", fontSize: "13px",
            color: displayText ? "#333" : "#aaa"
        });

        const $icon = $("<i>").addClass("fa-light fa-chevron-down").css({ fontSize: "11px", color: "#999" });

        $cell.append($text, $icon).appendTo(container);

        $cell.on("click", function (e) {
            e.stopPropagation();

            const $popup = $("<div>").appendTo("body").css({
                position: "absolute", zIndex: 10000,
                top: $(this).offset().top + $(this).outerHeight(),
                left: $(this).offset().left,
                width: $(this).outerWidth()
            });

            const selectInstance = $("<div>").appendTo($popup).dxSelectBox({
                items: optionsList,
                displayExpr: "text",
                valueExpr: "value",
                searchEnabled: true,
                opened: true,
                onValueChanged: function (args) {
                    const item = optionsList.find(x => x.value === args.value);
                    cellOptions.data[fieldName] = item ? { MaNhom: item.value, TenNhom: item.text } : null;

                    $text.text(item ? item.text : placeholder).css("color", "#333");
                    $popup.remove();
                },
                onClosed: function () { $popup.remove(); }
            }).dxSelectBox("instance");
        });
    };
}

function normalizeKey(rawKey) {
    return String(rawKey || "")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .replace(/[^a-zA-Z0-9]/g, "")
        .toLowerCase();
}

function toNumberSafe(value) {
    if (value === null || value === undefined || value === "") return null;
    const num = parseFloat(String(value).replace(",", "."));
    return Number.isFinite(num) ? num : null;
}

function getSLDaXuatValue(rowData) {
    if (!rowData || typeof rowData !== "object") return 0;

    const aliases = [
        "SLDaXuat",
        "SoLuongDaXuat",
        "SLXuat",
        "SLCapPhat",
        "SoLuongXuat",
        "SLNhap"
    ];
    const normalizedAliases = aliases.map(normalizeKey);
    const keys = Object.keys(rowData);

    for (const key of keys) {
        const normalizedKey = normalizeKey(key);
        if (normalizedAliases.includes(normalizedKey)) {
            const value = toNumberSafe(rowData[key]);
            if (value !== null) return value;
        }
    }

    for (const key of keys) {
        const normalizedKey = normalizeKey(key);
        if (!normalizedKey.startsWith("sl") && !normalizedKey.startsWith("soluong")) continue;
        if (
            normalizedKey.includes("xuat") ||
            normalizedKey.includes("capphat") ||
            normalizedKey.includes("nhap")
        ) {
            const value = toNumberSafe(rowData[key]);
            if (value !== null) return value;
        }
    }

    return 0;
}

function updateSoLuong(item) {

    let totalSoLuong = 0;
    item.MauSP.forEach(mau => {
        item.SizeInfo.forEach(size => {
            const found = dataSoLuong.find(x =>
                x.MaMau === mau.MaMau &&
                x.DauSizeID === size.MaNhomSize &&
                x.SizeID === size.MaSize
            );
            if (found) {
                totalSoLuong += (Number(found.SoLuong) || 0);
            }
        });
    });

    item.SoLuong = totalSoLuong;

    const dinhMuc = parseFloat(String(item.DinhMuc ?? 0).replace(",", ".")) || 0;
    item.CapPhat = Math.round((totalSoLuong * dinhMuc) * 10000) / 10000;
    item.SLDK = item.CapPhat;

    if (dxDataGridThuVien) {
        dxDataGridThuVien.refresh();
    }
}
async function loadSoLuong(forceReload = false) {
    const maLenh = $("#malenhdangkyvattu").val();

    if (!maLenh) {
        dataSoLuong = [];
        soLuongLoadedMaLenh = "";
        return;
    }

    if (!forceReload && soLuongLoadedMaLenh === maLenh && Array.isArray(dataSoLuong) && dataSoLuong.length > 0) {
        return;
    }

    dataSoLuong = await API.Get("GETSOLUONG", { para1: maLenh });
    soLuongLoadedMaLenh = maLenh;
}

async function reloadSoLuongWhenSelectSize(item) {
    await loadSoLuong();
    updateSoLuong(item);
}
// module = M.12.00.01 
//title xác nhận phiếu kiểm kê 
//detail //Maha, vat ""
//sendTo ERP_PhongBan ->  KHo 
//BoPhan ERP_BoPhan ->  KhoPL or K hoNL  or TBP = All
//Status 1 Truong BP  Ha, 2 -> to truong Can
function sendNotify(UserID, ModuleID, title, detail, sendTo, BoPhan = "ALL", Status = -1) {

    const url = `/api/SendToNotification/PushNotification?` +
        `UserIDTao=${encodeURIComponent(UserID)}&` +
        `ModuleID=${encodeURIComponent(ModuleID)}&` +
        `Title=${encodeURIComponent(title)}&` +
        `Detail=${encodeURIComponent(detail)}&` +
        `SendTo=${encodeURIComponent(sendTo)}&` +
        `BoPhan=${encodeURIComponent(BoPhan)}&` +
        `Status=${Status}`
        ;

    $.ajax({
        url: url,
        type: "POST",
        contentType: false,
        processData: false,
        success: function (result) {
            console.log("Gửi thông báo thành công:", result);
        },
        error: function (xhr, status, error) {
            console.error("Lỗi gửi thông báo:", error);
        }
    });
}
function loadPhongBanOptions() {
    const $sel = $("#tkPhongBan");
    const currentVal = $sel.val();

    const pbSet = new Map();
    lstDataDangKyVatTu.forEach(item => {
        if (item.MaPB && item.TenPB) {
            pbSet.set(item.MaPB, item.TenPB);
        }
    });

    $sel.find("option:not(:first)").remove();
    pbSet.forEach((tenPB, maPB) => {
        $sel.append(`<option value="${maPB}">${tenPB}</option>`);
    });

    if (currentVal) $sel.val(currentVal);
}
function filterByTrangThai() {
    const trangThai = $("#trangThaiDuyet").val();
    const tuNgayStr = $("#filterTuNgay").val();
    const denNgayStr = $("#filterDenNgay").val();
    const maHang = $("#tkMaHang").val().trim().toLowerCase();
    const maLenh = $("#tkMaLenh").val().trim().toLowerCase();
    const nguoiLap = $("#tkNguoiLap").val().trim().toLowerCase();
    const phongBan = $("#tkPhongBan").val();

    function parseDate(str) {
        if (!str) return null;
        const [d, m, y] = str.split("/");
        if (!d || !m || !y) return null;
        return new Date(y, m - 1, d);
    }

    const tuNgay = parseDate(tuNgayStr);
    const denNgay = parseDate(denNgayStr);
    if (denNgay) denNgay.setHours(23, 59, 59);

    let filtered = [...lstDataDangKyVatTu];

    if (trangThai === "done") {
        filtered = filtered.filter(item =>
            item.SignTBPMer && item.SignTBPMer.trim() !== ''
        );
    } else if (trangThai === "pending") {
        filtered = filtered.filter(item =>
            !item.SignTBPMer || item.SignTBPMer.trim() === ''
        );
    }

    if (tuNgay || denNgay) {
        filtered = filtered.filter(item => {
            if (!item.NgayTH) return false;
            const ngayTH = new Date(item.NgayTH);
            if (tuNgay && ngayTH < tuNgay) return false;
            if (denNgay && ngayTH > denNgay) return false;
            return true;
        });
    }

    if (maHang || maLenh || nguoiLap || phongBan) {
        filtered = filtered.filter(item => {
            const okMaHang = !maHang || String(item.MaDH || "").toLowerCase().includes(maHang);
            const okMaLenh = !maLenh || String(item.MaLenh || "").toLowerCase().includes(maLenh);
            const okNguoi = !nguoiLap || String(item.NguoiTH || "").toLowerCase().includes(nguoiLap)
                || String(item.HoTenNgDK || "").toLowerCase().includes(nguoiLap);
            const okPB = !phongBan || String(item.MaPB || "") === phongBan;
            return okMaHang && okMaLenh && okNguoi && okPB;
        });
    }

    updateGrid(dxDataGridDanhSachDangKy, filtered);
    setTimeout(() => mergeIsNPLCells(), 100);
}
function mergeIsNPLCells() {
    const grid = dxDataGridDanhSachDangKy;
    if (!grid) return;
    const $rows = grid.element().find(".dx-datagrid-rowsview .dx-data-row");
    if ($rows.length === 0) return;

    $rows.each(function () {
        $(this).find("td").removeAttr("rowspan").show();
    });

    const visibleRows = grid.getVisibleRows().filter(r => r.rowType === "data");
    if (visibleRows.length === 0) return;


    const mergeColumns = [
        { caption: "Loại VT", dataField: "IsNPL" },
        { caption: "Người ĐK", dataField: "SignNgDK" },
        { caption: "TBP người ĐK", dataField: "SignTBPNgDK" },
        { caption: "Mer soát sét", dataField: "SignMer" },
        { caption: "TBP Mer soát sét", dataField: "SignTBPMer" },
    ];

    const headerTds = grid.element().find(".dx-datagrid-headers .dx-header-row td");

    mergeColumns.forEach(function ({ caption, dataField }) {
        let tdIndex = -1;
        headerTds.each(function (idx) {
            if ($(this).text().trim() === caption) {
                tdIndex = idx;
                return false;
            }
        });
        if (tdIndex === -1) return;

        let i = 0;
        while (i < visibleRows.length) {
            const currentData = visibleRows[i].data;
            const currentPhieu = currentData?.PhieuTH;
            let spanCount = 1;
            let j = i + 1;

            while (
                j < visibleRows.length &&
                visibleRows[j].data?.PhieuTH === currentPhieu
            ) {
                spanCount++;
                j++;
            }

            if (spanCount > 1) {
                const $firstRow = $($rows[i]);
                const $firstTd = $firstRow.find("td").eq(tdIndex);

                $firstTd.attr("rowspan", spanCount).css("vertical-align", "middle");

                for (let k = i + 1; k < i + spanCount; k++) {
                    $($rows[k]).find("td").eq(tdIndex).hide();
                }
            }

            i += spanCount;
        }
    });
}
function showGlobalLoading(text = "Đang xử lý...") {
    $("#globalLoadingText").text(text);
    $("#globalLoadingOverlay").css("display", "flex");
}

function hideGlobalLoading() {
    $("#globalLoadingOverlay").css("display", "none");
}