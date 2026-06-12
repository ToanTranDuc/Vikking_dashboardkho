/// VARIABLE
var userNameSave = localStorage.getItem("username1");
let dxDataGridDangKyVatTu;
let dxDataGridDanhSachDangKy;
let dxDataGridEditVatTu;
let selectedItems = [];
let selectedRowsToDelete = [];
let lstDataDangKyVatTu = [];
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
let phieuDKJoin = [];

/// UTILS
function ddmmyyyyToYmd(dateStr) {
    if (!dateStr) return "1990-01-01";
    const [d, m, y] = dateStr.split("/");
    return `${y}-${m}-${d}`;
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

        const url = `/api/DangKyVatTu/Get?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

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

        const url = `/api/DangKyVatTu/${router}?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

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

    const data = await API.Get("GetLenhDNTH", { para1: isNPL, para2: maKH });
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
    $("#txtPhieu").val(`PDNTH_${dot}`);
}

async function GetPhieu() {
    const maLenhSanXuat = $("#malenh").val();
    const data = await API.Get("GetPhieuDNTH", { para1: isNPL, para2: /*maLenhSanXuat */'' });
    const $soPhieu = $("#maphieu");

    if ($soPhieu.hasClass("select2-hidden-accessible")) {
        $soPhieu.select2("destroy");
    }
    $soPhieu.empty();

    if (data.length > 0) {
        const html = data.map(x => `<option data-display="${x.Display}" value="${x.PhieuTH}">${x.Display}</option>`).join('');
        selectSoPhieuDisplay = data[0].Display
        $soPhieu.append(html);
    }

    $soPhieu.select2();

    GetDSPhieuDNTH();
}

async function GetDSPhieuDNTH() {
    const soPhieu = $("#maphieu option:selected").val() || '';
    /*    const MaLenhSX = $("#malenh").val() || '';*/
    if (!soPhieu) return;
    const data = await API.Get("GetDSChiTietPhieu", { para1: soPhieu, para2: isNPL /*, para3: MaLenhSX*/ });
    lstDataDangKyVatTu = data;
    updateGrid(dxDataGridDanhSachDangKy, data);
    loadPhongBanOptions();
    filterByTrangThai();
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
                || String(item.TenUserNgDK_Ten || "").toLowerCase().includes(nguoiLap);
            const okPB = !phongBan || String(item.MaPB || "") === phongBan;
            return okMaHang && okMaLenh && okNguoi && okPB;
        });
    }

    updateGrid(dxDataGridDanhSachDangKy, filtered);
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
async function SavePhieu(arrSave) {
    await API.Post("PostDNTH", "PostDeNghiThuHoi", arrSave, "Lưu phiếu thành công", { para1: isNPL });
    const phieuVuaTao = `PDNTH_${dot}`;

    resetModalState();

    await GetMaLenh2();
    await GetPhieuMax();

    $("#maphieu").val(phieuVuaTao).trigger("change");
    $("#modalAddPhieu").modal("hide")
}
async function UpdateKiTenPhieu(arrSave) {
    await API.Post("PostDNTH", "PostDeNghiThuHoi", arrSave, "Lưu phiếu thành công", { para1: isNPL });
    await GetDSPhieuDNTH();
}
async function DeletePhieuDNTH() {
    await API.Get("DeletePhieuDNTH", {
        para1: selectSoPhieu,
        para2: rowDelete.MaLenhSX,
        para3: rowDelete.MaNPL
    }, "Xóa thành công");

    await GetDSPhieuDNTH();
    await GetMaLenh2();

    if (lstDataDangKyVatTu.length === 0) {
        await resetToDefault();
    }

    $("#modalComfimrtDeleteVT").modal('hide');
}

async function DeleteAllPhieu() {
    await API.Get("Delete", { para1: selectSoPhieu, para2: '', para3: '' }, "Xóa phiếu thành công");
    await resetToDefault();
    chiTietLenhCache = {};

    $("#modalComfimrtDeleteVT").modal('hide');
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
            GhiChu: item.GhiChu,
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
        }

        arrSave.push(objectUpdate)
    })
    await API.Post("PostDNTH", "PostDeNghiThuHoi", arrSave, "Cập nhật thành công");
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

    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#phieuDKXInput').val('');
    $('#dropdownList').removeClass('show');

    updateGrid(dxDataGridDangKyVatTu, []);
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

    const formattedInt = Number(intPart).toLocaleString("en");
    const result = decPart ? `${formattedInt}.${decPart}` : formattedInt;

    // tìm summary cell
    const columns = grid.option("columns");
    const visibleColumns = columns.filter(col => col.visible !== false);
    const sldkColumnIndex = visibleColumns.findIndex(col => col.dataField === "SLDK");

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
    let str = value.toString();
    let intPart = str;
    let decPart = "";

    if (str.includes(".")) {
        [intPart, decPart] = str.split(".");
        decPart = decPart.substring(0, 4); // cắt, không làm tròn
    }

    // format phần nguyên
    let formattedInt = Number(intPart).toLocaleString();

    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
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

    await GetMaLenh2();
    await GetMaLenhDangKyVatTu();
    await GetDSPhieuDNTH();
    selectSoPhieu = $("#maphieu option:selected").val();
});

$(function () {

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

    $('#signatureModal').on('shown.bs.modal', function () {
        $("#chkXacNhanNhanh").prop("checked", false);
    });

    $("#trangThaiDuyet").on("change", function () {
        filterByTrangThai();
    });

    let pickerTuNgayFilter = new tempusDominus.TempusDominus(
        document.getElementById('dtpTuNgayFilter'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    let pickerDenNgayFilter = new tempusDominus.TempusDominus(
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

    $("#btnXoaTimKiem").on("click", function () {
        $("#tkMaHang, #tkMaLenh, #tkNguoiLap").val("");
        $("#tkPhongBan").val("");
        $("#dotTimKiem").hide();
        filterByTrangThai();
    });

    $("#btnApplyTimKiem").on("click", function () {
        $("#panelTimKiem").hide();
        $("#arrowDropSearch").text("▼");
        const hasFilter = $("#tkMaHang").val() || $("#tkMaLenh").val() || $("#tkNguoiLap").val();
        $("#dotTimKiem").css("display", hasFilter ? "inline-block" : "none");
        filterByTrangThai();
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
            $("#trangThaiDuyet").val("pending").trigger("change");

            await GetMaLenh2();
            showToast("success", "Nạp lại thành công");
        } catch (err) {
            console.error(err);
        } finally {
            $btn.prop("disabled", false);
            $icon.removeClass("fa-spin");
        }
    });

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

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.select-container').length) {
            $('#dropdownList').removeClass('show');
        }
    });

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

    $('#btnSavePhieu').on("click", async function () {
        const $btn = $(this);

        // Nếu đang loading thì chặn luôn
        if ($btn.prop("disabled")) return;

        setLoadingButton($btn, true);
        try {
            await SaveDeNghiThuHoi();
        }
        catch (err) {
            console.error(err);
        } finally {
            // Luôn mở lại nút sau khi xong
            setLoadingButton($btn, false);
        }

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

        lstEditDangKyVatTu = dxDataGridDanhSachDangKy.option("dataSource");

        const daDuocKy = lstEditDangKyVatTu.some(item =>
            
            (item.SignTBPNgDK && item.SignTBPNgDK.trim() !== '') ||
            (item.SignMer && item.SignMer.trim() !== '') ||
            (item.SignTBPMer && item.SignTBPMer.trim() !== '')
        );

        if (daDuocKy) {
            showToast("warning", "Phiếu đã được ký, không thể chỉnh sửa!");
            return;
        }

        updateGrid(dxDataGridEditVatTu, lstEditDangKyVatTu);
        if (lstEditDangKyVatTu.length > 0) {
            const ngayCap = moment(lstEditDangKyVatTu[0].NgayCap).toDate();
            picker4.dates.setValue(tempusDominus.DateTime.convert(ngayCap));
        }
        isShowNgayCapAndGioCap = true;
        $("#modalEditDKVT").modal("show");
        $("#txtNgayChinhSua").text(selectSoPhieuDisplay);
    });

    $("#btnConfirmtDeletePhieu").on("click", function () {
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

    // Khi đóng modal - reset tất cả checkbox và state
    $('#modalAddPhieu').on('hidden.bs.modal', function () {
        // Reset checkbox
        $('#itemList input[type="checkbox"]').prop('checked', false);
        $('#checkAll').prop('checked', false);
        $('#phieuDKXInput').val('');
        $('#dropdownList').removeClass('show');
        $("#malenhdangkyvattu").empty();

        // Reset state
        selectedItems = [];
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
        phieuDKJoin = [];
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
    })

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
    $("#malenhdangkyvattu").on("change", function () {
        selectedItems = []
        phieuDKJoin = []
        $('#phieuDKXInput').val('');
        createViewDxDataGridDangKyVatTu([]);
        GetChiTietLenhThuHoi()
        //    GetPhieuDangKyXuat();
    })

    $("#btnNapLai").on("click", function () {
        GetPhieuMax();
        GetMaLenhDangKyVatTu();
        $("#phieuDKXInput").val("")
        renderList([])
        selectedItems = []
        phieuDKJoin = [];
        createViewDxDataGridDangKyVatTu([])
    })
});

//Xử lý người dùng click nhiều lần khi đang submit dữ liệu (tránh gọi API trùng)
function setLoadingButton($btn, isLoading, text = "Đang lưu...") {
    if (isLoading) {
        $btn.data("original-text", $btn.html());
        $btn.prop("disabled", true);
        $btn.css("opacity", "0.6");
        $btn.html(`<i class="fa fa-spinner fa-spin"></i> ${text}`);
    } else {
        $btn.prop("disabled", false);
        $btn.css("opacity", "1");
        $btn.html($btn.data("original-text"));
    }
}

function handleBack() {
    // Reset checkbox
    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#phieuDKXInput').val('');
    $('#dropdownList').removeClass('show');

    // Reset state
    selectedItems = [];
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
        showToast("warning", "Chưa có thông tin đăng ký");
        PlayAudioError();
        return;
    }

    const ngayDK = ddmmyyyyToYmd($("#tuNgay").val());
    const ngayCap = ddmmyyyyToYmd($("#ngayCap").val());

    const arrSave = dataSource.map(item => ({
        PhieuTH: '',
        MaLenhSX: item.MaLenhSanXuat,
        MaDH: item.MaDH,
        MaLenh: item.MaLenh,
        MaNPL: item.MaNPL,
        SLDK: item.SLDK,
        GhiChu: item.GhiChu,
        MaVT: item.MaVT,
        MauVT: item.MauVT,
        KhoVai: item.KhoVai,
        MaDVVT: item.MaDVVT,
        NgayDK: ngayDK,
        NgayTH: ngayCap,
        NgayTao: new Date(),
        IsNPL: item.NPL ? 1 : 0,
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
        Status: item.Status

    }));


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

    showConfirmModalSign(async function () {
        if (signatureHistory.length === 0) {
            alert('Vui lòng ký tên trước khi lưu!');
            return;
        }
        const signatureImage = canvas.toDataURL('image/png');
        arrSave.forEach(x => {
            x.SignNgDK = signatureImage

        })
        await SavePhieu(arrSave);
        const MaPhieu = $("#txtPhieu").val();                              
        const MaLenhDL = $("#malenhdangkyvattu option:selected").text();
        sendNotify(userNameSave, "M.48.00.00", "Đề nghị thu hồi nguyên liệu",
            `Mã phiếu: ${MaPhieu} | Mã lệnh: ${MaLenhDL}`, "TBP", "ALL", 1);
        return true; 
    })

}
/// DX DataGrid
function createViewDxDataGridDangKyVatTu(data) {
    dxDataGridDangKyVatTu = $("#dxDataGridDangKyVatTu").dxDataGrid({
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
                dataField: "Status",
                caption: "",
                width: 120,
                alignment: "center",
                allowSorting: false,
                cellTemplate: function (container, options) {
                    const isChecked = options.value === 2;
                    const $checkbox = $(`
                        <input type="checkbox" 
                               class="row-checkboxVTLoi" 
                               data-row-index="${options.rowIndex}"
                               ${isChecked ? 'checked' : ''}
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);
                    $checkbox.on("change", function () {
                        const checked = $(this).prop("checked");
                        options.data.Status = checked ? 2 : 1;
                    });
                    container.append($checkbox);
                },
                headerCellTemplate: function (container) {
                    const $wrapper = $(`
                        <div class="flex-column" style="display: flex; align-items: center; gap: 4px;">
                            <input type="checkbox" 
                                   id="checkAllRowsVTLoi" 
                                   style="width: 18px; height: 18px; cursor: pointer;" />
                            <span style="font-size: 12px;">Vật tư lỗi</span>
                        </div>
                    `);

                    $wrapper.find("#checkAllRowsVTLoi").on("change", function () {
                        const isChecked = $(this).prop("checked");
                        const grid = dxDataGridDangKyVatTu;
                        const dataSource = grid.option("dataSource");

                        dataSource.forEach((item) => {
                            item.Status = isChecked ? 2 : 1;
                        });

                        $('.row-checkboxVTLoi').prop('checked', isChecked);
                    });

                    container.append($wrapper);
                }
            },
            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 120 },
            { dataField: "MaLenh", caption: "Mã Lệnh", alignment: "center", minWidth: 100 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
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
                dataField: "CapPhat",
                caption: "SL Cấp Phát",
                alignment: "center",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.append(formatNumber(options.value))
                }
            },
            {
                dataField: "SLNhap",
                caption: "SL Xuất",
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
            {
                dataField: "SLDK",
                caption: "SL Thu Hồi",
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
                    const $input = $(`
                        <input type="number"
                                class="form-control inputSLDK"
                                value="${options.data.SLDK ?? ''}" />
                    `);

                    $input.on("input", function () {
                        let value = this.value;

                        // Không cho số âm
                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridDangKyVatTu);
                            return;
                        }

                        // Chỉ cho tối đa 4 số thập phân 
                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }

                        if (Number(value) > options.data.SLNhap) {
                            showToast("warning", "Số lượng đăng ký không được lớn hơn số lượng cấp phát");

                            const truncated = Math.trunc(options.data.SLNhap * 10000) / 10000;

                            this.value = truncated;
                            options.data.SLDK = truncated;
                            updateSummary(dxDataGridDangKyVatTu);
                            return;
                        }

                        options.data.SLDK = value === "" ? null : Number(value);
                        updateSummary(dxDataGridDangKyVatTu);
                    });

                    container.append($input);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
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
            // THÊM CỘT CHECKBOX
            {
                caption: "",
                width: 50,
                alignment: "center",
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
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },
    }).dxDataGrid("instance");
}
//function renderSignCell(container, options, fieldName) {

//    if (!options.data) return;

//    const grid = options.component;
//    const dataSource = grid.option("dataSource") || [];

//    const valueCheck = options.data.MaLenhSX;
//    const rowIndex = options.rowIndex;

//    let rowspan = 0;

//    if (rowIndex === 0 || dataSource[rowIndex - 1].MaLenhSX !== valueCheck) {

//        for (let i = rowIndex; i < dataSource.length; i++) {
//            if (dataSource[i].MaLenhSX === valueCheck) {
//                rowspan++;
//            } else {
//                break;
//            }
//        }

//        const containerDiv = $("<div>").css({
//            width: "100%",
//            height: "40px",
//            display: "flex",
//            justifyContent: "center",
//            alignItems: "center",
//            gap: "6px"
//        });

//        //Bổ sung 
//        if (options.data[fieldName] && options.data[fieldName] !== "checked") {
//            const tenOnly = options.data[{
//                SignNgDK: "TenUserNgDK_Ten", SignTBPNgDK: "TenUserTBPNDK_Ten", SignMer: "TenUserMerSoatSet_Ten", SignTBPMer: "TenUserTBPMerSoatSet_Ten"
//            }[fieldName]] ?? "";

//            const $wrapper = $("<div>").css({ display: "flex", flexDirection: "column", alignItems: "center", gap: "2px" });
//            $("<img>")
//                .attr("src", `/Images/SignDeNghiThuHoi/${options.data[fieldName]}?${Date.now()}`)
//                .css({ width: "70px", height: "35px" })
//                .appendTo($wrapper);
//            if (tenOnly) $("<span>").text(tenOnly).css({ fontSize: "11px", color: "#333", fontWeight: "bold" }).appendTo($wrapper);
//            $wrapper.appendTo(containerDiv);

//        } else if (options.data[fieldName] === "checked") {

//            const userField = { SignNgDK: "HoTenNgDK", SignTBPNgDK: "HoTenTBPNDK", SignMer: "HoTenMerSoatSet", SignTBPMer: "HoTenTBPMerSoatSet" }[fieldName] ?? "";
//            const hinhAnhField = { SignNgDK: "HinhAnhNgDK", SignTBPNgDK: "HinhAnhTBPNDK", SignMer: "HinhAnhMerSoatSet", SignTBPMer: "HinhAnhTBPMerSoatSet" }[fieldName] ?? "";
//            const tenOnly = options.data[{ SignNgDK: "TenUserNgDK_Ten", SignTBPNgDK: "TenUserTBPNDK_Ten", SignMer: "TenUserMerSoatSet_Ten", SignTBPMer: "TenUserTBPMerSoatSet_Ten" }[fieldName]] ?? "";
//            const userName = options.data[userField] ?? "";
//            const hinhAnh = options.data[hinhAnhField] ?? "";
//            const $wrapper = $("<div>").css({ display: "flex", flexDirection: "column", alignItems: "center", gap: "2px" });
//            if (hinhAnh) {
//                $("<img>").attr("src", `/Images/NhanVien/${hinhAnh}`)
//                    .css({ width: "70px", height: "35px", objectFit: "cover", borderRadius: "3px" })
//                    .on("error", function () { $(this).hide(); })
//                    .appendTo($wrapper);
//            }
//            let displayName = "";
//            if (hinhAnh) {
//                displayName = tenOnly || userName;
//            }
//            else {
//                displayName = userName || tenOnly;
//            }
//            $("<span>")
//                .text(displayName)
//                .css({ fontSize: "11px", color: "#333", fontWeight: "bold" })
//                .appendTo($wrapper);
//            $wrapper.appendTo(containerDiv);
//        }

//        let checkSign = "";

//        const signNgDK = options.data.SignNgDK ?? "";
//        const signTBPNgDK = options.data.SignTBPNgDK ?? "";
//        const signMer = options.data.SignMer ?? "";
//        const signTBPMer = options.data.SignTBPMer ?? "";

//        if (fieldName === "SignNgDK") {

//            checkSign = signNgDK ? "d-none" : "";

//        }

//        else if (fieldName === "SignTBPNgDK") {

//            if (!signNgDK) checkSign = "d-none";
//            else checkSign = signTBPNgDK ? "d-none" : "";

//        }

//        else if (fieldName === "SignMer") {

//            if (!signTBPNgDK) checkSign = "d-none";
//            else checkSign = signMer ? "d-none" : "";

//        }

//        else if (fieldName === "SignTBPMer") {

//            if (!signMer) checkSign = "d-none";
//            else checkSign = signTBPMer ? "d-none" : "";

//        }
//        $("<input>")
//            .attr("type", "checkbox")
//            .addClass(checkSign)
//            .css({
//                cursor: "pointer",
//                left: "4px",
//                top: "5px",
//                position: "absolute",
//                width: "14px",
//                height: "14px",
//                accentColor: "#007bff"
//            })
//            .on("change", async function () {
//                const rowData = options.data;
//                let arrSave = [];
//                arrSave.push({
//                    PhieuTH: rowData.PhieuTH,
//                    MaLenhSX: rowData.MaLenhSX,
//                    MaDH: rowData.MaDH,
//                    MaLenh: rowData.MaLenh,
//                    MaNPL: rowData.MaNPL,
//                    SLDK: rowData.SLDK,
//                    GhiChu: rowData.GhiChu,
//                    MaVT: rowData.MaVT,
//                    MauVT: rowData.MauVT,
//                    KhoVai: rowData.KhoVai,
//                    MaDVVT: rowData.MaDVVT,
//                    NgayDK: "",
//                    NgayTH: "",
//                    NgayTao: "",
//                    IsNPL: rowData.IsNPL ? 1 : 0,
//                    NguoiTH: userNameSave,
//                    PhieuDK: "",
//                    SignNgDK: "",
//                    NgayKi: "",
//                    SignTBPNgDK: fieldName === "SignTBPNgDK" ? "checked" : "",
//                    NgayKiTBPNgDK: "",
//                    SignMer: fieldName === "SignMer" ? "checked" : "",
//                    NgaySignMer: "",
//                    SignTBPMer: fieldName === "SignTBPMer" ? "checked" : "",
//                    NgaySignTBPMer: "",
//                });
//                await UpdateKiTenPhieu(arrSave);
//            })
//            .appendTo(containerDiv);

//        $("<i>")
//            .addClass(`fa-solid fa-signature ${checkSign}`)
//            .css({
//                cursor: "pointer",
//                color: "#007bff",
//                fontSize: "14px",
//                position: "absolute",
//                top: "2px",
//                right: "4px"
//            })
//            .on("click", function () {

//                const rowData = options.data;

//                showConfirmModalSign(async function () {
//                    const signatureImage = canvas.toDataURL('image/png');

//                    let arrSave = []
//                    arrSave.push({
//                        PhieuTH: rowData.PhieuTH,
//                        MaLenhSX: rowData.MaLenhSX,
//                        MaDH: rowData.MaDH,
//                        MaLenh: rowData.MaLenh,
//                        MaNPL: rowData.MaNPL,
//                        SLDK: rowData.SLDK,
//                        GhiChu: rowData.GhiChu,
//                        MaVT: rowData.MaVT,
//                        MauVT: rowData.MauVT,
//                        KhoVai: rowData.KhoVai,
//                        MaDVVT: rowData.MaDVVT,
//                        NgayDK: "",
//                        NgayTH: "",
//                        NgayTao: "",
//                        IsNPL: rowData.IsNPL ? 1 : 0,
//                        NguoiTH: userNameSave,
//                        PhieuDK: "",
//                        SignNgDK: "",
//                        NgayKi: "",
//                        SignTBPNgDK: fieldName !== "SignTBPNgDK" ? "" : signatureImage,
//                        NgayKiTBPNgDK: "",
//                        SignMer: fieldName !== "SignMer" ? "" : signatureImage,
//                        NgaySignMer: "",
//                        SignTBPMer: fieldName !== "SignTBPMer" ? "" : signatureImage,
//                        NgaySignTBPMer: "",

//                    });
//                    await UpdateKiTenPhieu(arrSave);
//                    return true;
//                })
//                //openModalSign(rowData, fieldName);

//            })
//            .appendTo(containerDiv);

//        containerDiv.appendTo(container);
//        container.addClass("position-relative")
//        container.attr("rowspan", rowspan);

//    } else {

//        container.addClass("d-none");

//    }
//}
function renderSignCell(container, options, fieldName) {
    if (!options.data) return;

    const grid = options.component;
    const dataSource = grid.option("dataSource") || [];

    const currentPhieu = options.data.PhieuTH;

    // Tìm vị trí thực trong dataSource theo PhieuTH + MaNPL (key duy nhất)
    const dsIndex = dataSource.findIndex(item =>
        item.PhieuTH === options.data.PhieuTH &&
        item.MaNPL === options.data.MaNPL &&
        item.MaVT === options.data.MaVT &&
        item.MauVT === options.data.MauVT &&
        item.KhoVai === options.data.KhoVai
    );

    // isFirst: không có item nào trước nó trong dataSource có cùng PhieuTH
    const isFirst = dsIndex === 0 ||
        dataSource[dsIndex - 1]?.PhieuTH !== currentPhieu;

    if (!isFirst) {
        container.addClass("d-none");
        return;
    }

    // rowspan: đếm bao nhiêu item liên tiếp cùng PhieuTH từ dsIndex
    let rowspan = 0;
    for (let i = dsIndex; i < dataSource.length; i++) {
        if (dataSource[i]?.PhieuTH === currentPhieu) rowspan++;
        else break;
    }

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
        const tenOnly = options.data[{
            SignNgDK: "TenUserNgDK_Ten", SignTBPNgDK: "TenUserTBPNDK_Ten",
            SignMer: "TenUserMerSoatSet_Ten", SignTBPMer: "TenUserTBPMerSoatSet_Ten"
        }[fieldName]] ?? "";

        const $wrapper = $("<div>").css({ display: "flex", flexDirection: "column", alignItems: "center", gap: "2px" });
        $("<img>")
            .attr("src", `/Images/SignDeNghiThuHoi/${options.data[fieldName]}?${Date.now()}`)
            .css({ width: "70px", height: "35px" })
            .appendTo($wrapper);
        if (tenOnly) $("<span>").text(tenOnly)
            .css({ fontSize: "11px", color: "#333", fontWeight: "bold" })
            .appendTo($wrapper);
        $wrapper.appendTo(containerDiv);

    } else if (options.data[fieldName] === "checked") {
        const userField = { SignNgDK: "HoTenNgDK", SignTBPNgDK: "HoTenTBPNDK", SignMer: "HoTenMerSoatSet", SignTBPMer: "HoTenTBPMerSoatSet" }[fieldName] ?? "";
        const hinhAnhField = { SignNgDK: "HinhAnhNgDK", SignTBPNgDK: "HinhAnhTBPNDK", SignMer: "HinhAnhMerSoatSet", SignTBPMer: "HinhAnhTBPMerSoatSet" }[fieldName] ?? "";
        const tenOnly = options.data[{ SignNgDK: "TenUserNgDK_Ten", SignTBPNgDK: "TenUserTBPNDK_Ten", SignMer: "TenUserMerSoatSet_Ten", SignTBPMer: "TenUserTBPMerSoatSet_Ten" }[fieldName]] ?? "";
        const userName = userField ? (options.data[userField] ?? "") : "";
        const hinhAnh = hinhAnhField ? (options.data[hinhAnhField] ?? "") : "";

        if (hinhAnh) {
            const $wrapper = $("<div>").css({ display: "flex", flexDirection: "column", alignItems: "center", gap: "2px" });
            $("<img>").attr("src", `/Images/NhanVien/${hinhAnh}`)
                .css({ width: "70px", height: "35px", objectFit: "cover", borderRadius: "3px" })
                .on("error", function () { $(this).hide(); })
                .appendTo($wrapper);
            $("<span>").text(tenOnly || userName)
                .css({ fontSize: "11px", color: "#333", fontWeight: "bold" })
                .appendTo($wrapper);
            $wrapper.appendTo(containerDiv);
        } else {
            $("<span>").text(userName || tenOnly)
                .css({ fontSize: "12px", color: "#333", fontWeight: "bold" })
                .appendTo(containerDiv);
        }
    }

    let checkSign = "";
    const signNgDK = options.data.SignNgDK ?? "";
    const signTBPNgDK = options.data.SignTBPNgDK ?? "";
    const signMer = options.data.SignMer ?? "";
    const signTBPMer = options.data.SignTBPMer ?? "";

    if (fieldName === "SignNgDK") { checkSign = signNgDK ? "d-none" : ""; }
    else if (fieldName === "SignTBPNgDK") { checkSign = !signNgDK ? "d-none" : signTBPNgDK ? "d-none" : ""; }
    else if (fieldName === "SignMer") { checkSign = !signTBPNgDK ? "d-none" : signMer ? "d-none" : ""; }
    else if (fieldName === "SignTBPMer") { checkSign = !signMer ? "d-none" : signTBPMer ? "d-none" : ""; }

    $("<input>").attr("type", "checkbox").addClass(checkSign)
        .css({
            cursor: "pointer", left: "4px", top: "5px", position: "absolute",
            width: "14px", height: "14px", accentColor: "#007bff"
        })
        .on("change", async function () {
            const rowData = options.data;
            let arrSave = [{
                PhieuTH: rowData.PhieuTH, MaLenhSX: rowData.MaLenhSX,
                MaDH: rowData.MaDH, MaLenh: rowData.MaLenh, MaNPL: rowData.MaNPL,
                SLDK: rowData.SLDK, GhiChu: rowData.GhiChu, MaVT: rowData.MaVT,
                MauVT: rowData.MauVT, KhoVai: rowData.KhoVai, MaDVVT: rowData.MaDVVT,
                NgayDK: "", NgayTH: "", NgayTao: "",
                IsNPL: rowData.IsNPL ? 1 : 0, NguoiTH: userNameSave,
                PhieuDK: "", SignNgDK: "", NgayKi: "",
                SignTBPNgDK: fieldName === "SignTBPNgDK" ? "checked" : "",
                NgayKiTBPNgDK: "",
                SignMer: fieldName === "SignMer" ? "checked" : "",
                NgaySignMer: "",
                SignTBPMer: fieldName === "SignTBPMer" ? "checked" : "",
                NgaySignTBPMer: "",
            }];
            await UpdateKiTenPhieu(arrSave);
            const phieu = rowData.PhieuTH;
            if (fieldName === "SignNgDK") {
                sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                    "Người ĐK đã ký phiếu " + phieu, "TBP", "ALL", 1);
            } else if (fieldName === "SignTBPNgDK") {
                sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                    "TBP Người ĐK đã ký phiếu " + phieu, "PKH", "ALL", -1);
            } else if (fieldName === "SignMer") {
                sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                    "Mer soát sét đã ký phiếu " + phieu, "PKH", "ALL", 1);
            } else if (fieldName === "SignTBPMer") {
                sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                    "TBP Mer soát sét đã ký phiếu " + phieu, "ALL", "ALL", -1);
            }
        }).appendTo(containerDiv);

    $("<i>").addClass(`fa-solid fa-signature ${checkSign}`)
        .css({
            cursor: "pointer", color: "#007bff", fontSize: "14px",
            position: "absolute", top: "2px", right: "4px"
        })
        .on("click", function () {
            const rowData = options.data;
            showConfirmModalSign(async function () {
                const signatureImage = canvas.toDataURL('image/png');
                let arrSave = [{
                    PhieuTH: rowData.PhieuTH, MaLenhSX: rowData.MaLenhSX,
                    MaDH: rowData.MaDH, MaLenh: rowData.MaLenh, MaNPL: rowData.MaNPL,
                    SLDK: rowData.SLDK, GhiChu: rowData.GhiChu, MaVT: rowData.MaVT,
                    MauVT: rowData.MauVT, KhoVai: rowData.KhoVai, MaDVVT: rowData.MaDVVT,
                    NgayDK: "", NgayTH: "", NgayTao: "",
                    IsNPL: rowData.IsNPL ? 1 : 0, NguoiTH: userNameSave,
                    PhieuDK: "", SignNgDK: "", NgayKi: "",
                    SignTBPNgDK: fieldName !== "SignTBPNgDK" ? "" : signatureImage,
                    NgayKiTBPNgDK: "",
                    SignMer: fieldName !== "SignMer" ? "" : signatureImage,
                    NgaySignMer: "",
                    SignTBPMer: fieldName !== "SignTBPMer" ? "" : signatureImage,
                    NgaySignTBPMer: "",
                }];
                await UpdateKiTenPhieu(arrSave);
                const phieu = rowData.PhieuTH;
                if (fieldName === "SignNgDK") {
                    sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                        "Người ĐK đã ký phiếu " + phieu, "TBP", "ALL", 1);
                } else if (fieldName === "SignTBPNgDK") {
                    sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                        "TBP Người ĐK đã ký phiếu " + phieu, "PKH", "ALL", -1);
                } else if (fieldName === "SignMer") {
                    sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                        "Mer soát sét đã ký phiếu " + phieu, "PKH", "ALL", 1);
                } else if (fieldName === "SignTBPMer") {
                    sendNotify(userNameSave, "M.48.00.00", "Phiếu thu hồi nguyên liệu",
                        "TBP Mer soát sét đã ký phiếu " + phieu, "ALL", "ALL", -1);
                }
                return true;
            });
        }).appendTo(containerDiv);

    containerDiv.appendTo(container);
    container.addClass("position-relative");
    container.attr("rowspan", rowspan);
}

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
                visible: false,
                minWidth: 160,
                groupIndex: 0,
                sortOrder: "desc",
                groupCellTemplate: function (container, options) {
                    const items = options.data?.items || options.data?.collapsedItems || [];
                    const firstItem = items[0] || {};
                    const maLenh = firstItem.MaLenh || "";
                    const phieuTH = firstItem.PhieuTH || options.value || "";
                    const maDH = firstItem.MaDH || "";
                    $("<div>").css({
                        fontWeight: "400",
                        color: "#000000",
                        fontSize: "13px",
                        padding: "2px 4px",
                        display: "flex",
                        gap: "16px",
                        alignItems: "center"
                    })
                        .append(
                           
                            $("<span>").html(`Số Phiếu: ${phieuTH}`).css("font-weight", "600"),
                            maLenh ? $("<span>").html(`|  LSX: ${maLenh}`).css("font-weight", "600") : null,
                            maDH ? $("<span>").html(`| MH: ${maDH}`).css("font-weight", "600") : null,
                        )
                        .appendTo(container);
                }
            },
            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 130 },
            { dataField: "StatusVT", caption: "Trạng thái", alignment: "center", minWidth: 100 },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 100 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 100 },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
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

            {
                dataField: "SLNhap",
                caption: "SL Xuất",
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
                dataField: "SLDK",
                caption: "SL ĐN Thu Hồi",
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
                dataField: "NgayTH",
                caption: "Ngày YC Thu Hồi",
                alignment: "center",
                minWidth: 150,
                format: "dd/MM/yyyy",
                cellTemplate: function (container, options) {
                    const dateConvert = moment(options.value).format("DD/MM/YYYY")
                    container.text(dateConvert)
                }

            },

            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                minWidth: 200,
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
                caption: "Xóa",
                minWidth: 100,
                alignment: "center",
                cellTemplate: function (container, options) {

                    if (options.data.CheckPDN != '0') {
                        return;
                    }

                    const $wrapper = $(`<div></div>`).css({ display: "flex", justifyContent: "center", height: "25px", gap: "8px" })
                    // Thêm nút
                    const $btnEdit = $("<div>")
                        .append(
                            $("<i>")
                                .addClass("fa-solid fa-pen-to-square")
                                .css({ fontSize: "16px", color: "red", cursor: "pointer" })
                                .on("click", function () {
                                    rowEdit = { ...options.data };
                                    dxDataGridEditVatTu.option("dataSource", [rowEdit]);
                                    $("#modalEditDKVT").modal("show");
                                })
                        )
                        .appendTo(container);

                    // Thêm nút
                    const $btnDelete = $("<div>")
                        .append(
                            $("<i>")
                                .addClass("fa-solid  fa-trash")
                                .css({ fontSize: "16px", color: "red", cursor: "pointer" })
                                .on("click", function () {
                                    rowDelete = options.data
                                    $("#modalComfimrtDeleteVT").modal('show')
                                })
                        )
                        .appendTo(container);

                    $wrapper.append($btnDelete, $btnEdit).appendTo(container)
                }

            }
        ],
        summary: {
            totalItems: [
                {
                    column: "SLDK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        let str = e.value.toString();
                        let intPart = str;
                        let decPart = "";

                        if (str.includes(".")) {
                            [intPart, decPart] = str.split(".");
                            decPart = decPart.substring(0, 4); // cắt, không làm tròn
                        }

                        const formattedInt = Number(intPart).toLocaleString("en");
                        return decPart ? `${formattedInt}.${decPart}` : formattedInt;
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
                            decPart = decPart.substring(0, 4); // cắt, không làm tròn
                        }

                        const formattedInt = Number(intPart).toLocaleString("en");
                        return decPart ? `${formattedInt}.${decPart}` : formattedInt;
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";

                        let str = e.value.toString();
                        let intPart = str;
                        let decPart = "";

                        if (str.includes(".")) {
                            [intPart, decPart] = str.split(".");
                            decPart = decPart.substring(0, 4); // cắt, không làm tròn
                        }

                        const formattedInt = Number(intPart).toLocaleString("en");
                        return decPart ? `${formattedInt}.${decPart}` : formattedInt;
                    }
                }
            ]
        },
        onCellPrepared: function (e) {
            if (e.column.command == "expand" && !e.column.dataField) {
                $(e.cellElement).css({
                    width: "0px", minWidth: "0px", maxWidth: "0px",
                    padding: "0px", border: "none", overflow: "hidden"
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
        },
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
            {
                dataField: "SLNhap",
                caption: "SL Xuất",
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
                dataField: "SLDK",
                caption: "SL Đăng Ký",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
                    const truncated = Math.trunc(options.data.SLDK * 10000) / 10000;

                    // Bỏ số 0 dư
                    truncated.toString();

                    const $input = $(`
                        <input type="number"
                               class="form-control inputSLDK"
                               value="${truncated ?? ''}" />
                    `);

                    $input.on("input", function () {
                        let value = this.value;

                        // Không cho số âm
                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridEditVatTu);
                            return;
                        }

                        // Chỉ cho tối đa 4 số thập phân 
                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }

                        if (Number(value) > options.data.SLNhap) {
                            showToast("warning", "Số lượng đăng ký không được lớn hơn số lượng xuất");
                            this.value = options.data.SLNhap;
                            options.data.SLDK = options.data.SLNhap;
                            updateSummary(dxDataGridEditVatTu);
                            return;
                        }

                        options.data.SLDK = value === "" ? null : value
                        updateSummary(dxDataGridEditVatTu);
                    });
                    container.append($input);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
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
            }
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
                        decPart = decPart.substring(0, 4); // cắt, không làm tròn
                    }

                    const formattedInt = Number(intPart).toLocaleString("en");
                    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
                }
            }
            ]
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
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#signatureModal").modal("hide");
    });

    $("#signatureModal").modal("show");
}

function sendNotify(UserID, ModuleID, title, detail, sendTo, BoPhan = "ALL", Status = -1) {
    const url = `/api/SendToNotification/PushNotification?` +
        `UserIDTao=${encodeURIComponent(UserID)}&` +
        `ModuleID=${encodeURIComponent(ModuleID)}&` +
        `Title=${encodeURIComponent(title)}&` +
        `Detail=${encodeURIComponent(detail)}&` +
        `SendTo=${encodeURIComponent(sendTo)}&` +
        `BoPhan=${encodeURIComponent(BoPhan)}&` +
        `Status=${Status}`;

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