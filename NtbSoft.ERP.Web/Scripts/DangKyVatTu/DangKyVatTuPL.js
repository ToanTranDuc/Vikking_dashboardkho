/// VARIABLE
var userNameSave = localStorage.getItem("username1");
let dxDataGridDangKyVatTu;
let dxDataGridDanhSachDangKy;
let dxDataGridEditVatTu;
let selectedItems = [];
let selectedRowsToDelete = [];
let lstDataChiTietLenh = [];
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
let picker1, picker2, picker3, picker4, picker5;
let currentStatus = 1;
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

    async Post(action, arrSave, textSuccess = "Lưu thành công!", para = {}) {
        const paraConvert = Object.entries(para || {})
            .map(([key, value]) => `${key}=${value}`)
            .join("&");

        const url = `/api/DangKyVatTu/Post?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

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
/*async function GetMaLenh() {
    const maKH = $("#khachhang").val() || "all";
    const data = await API.Get("GetLenh", { para1: isNPL, para2: maKH });
    batchData = data;
    renderList(data);
}*/

async function GetMaLenh() {
    const maKH = $("#khachhang").val() || "all";
    const data = await API.Get("GetLenh", { para1: isNPL, para2: maKH });

    // Lọc để chỉ lấy phần tử đầu tiên khi trùng MaLenhSanXuat
    const uniqueMap = new Map();
    data.forEach(item => {
        if (!uniqueMap.has(item.MaLenhSanXuat)) {
            uniqueMap.set(item.MaLenhSanXuat, item);
        }
    });

    const uniqueData = Array.from(uniqueMap.values());

    batchData = uniqueData;
    renderList(uniqueData);
}

async function GetMaLenh2() {
    const $maLenhSelect = $("#malenh");
    const data = await API.Get("GetPYC", { para1: isNPL });

    $maLenhSelect.empty();
    if (data.length > 0) {
        const html = data.map(x => `<option value="${x.MaLenhSX}">${x.MaLenh}</option>`).join('');
        $maLenhSelect.append(html);
    }

    $maLenhSelect.select2();
    await GetPhieu();
}

async function GetDSPhieuDKVT() {
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
    const data = await API.Get("GetPhieuMax", { para1: isNPL });
    dot = data[0].PXH;
    $("#txtPhieu").val(`PDK_${dot}`);
}

async function GetPhieu() {
    const maLenhSanXuat = $("#malenh").val();
    const data = await API.Get("GetPhieu", { para1: isNPL, para2: maLenhSanXuat });
    const $soPhieu = $("#maphieu");

    if ($soPhieu.hasClass("select2-hidden-accessible")) {
        $soPhieu.select2("destroy");
    }
    $soPhieu.empty();

    /*  if ($soPhieu.data('select2')) $soPhieu.select2('destroy');*/

    if (data.length > 0) {
        const html = data.map(x => `<option data-display="${x.Display}" value="${x.PhieuDK}">${x.Display}</option>`).join('');
        selectSoPhieuDisplay = data[0].Display
        $soPhieu.append(html);
    }

    $soPhieu.select2();
    GetDSPhieuDK();
}

async function GetDSPhieuDK() {
    const soPhieu = $("#maphieu option:selected").val() || '';
    const MaLenhSX = $("#malenh").val() || '';
    const data = await API.Get("GetDSPhieuDK", { para1: soPhieu, para2: isNPL, para3: MaLenhSX });
    lstDataDangKyVatTu = data;
    updateGrid(dxDataGridDanhSachDangKy, data);
}

async function SavePhieu(arrSave) {
    await API.Post("Post", arrSave, "Lưu phiếu thành công", { para1: isNPL });
    const phieuVuaTao = `PDK_${dot}`;

    resetModalState();

    await GetMaLenh2();
    $("#maphieu").val(phieuVuaTao).trigger("change");
    await GetPhieuMax();
    $("#modalAddPhieu").modal("hide")
}

async function DeletePhieu() {
    await API.Get("Delete", {
        para1: selectSoPhieu,
        para2: rowDelete.MaLenhSX,
        para3: rowDelete.MaNPL
    }, "Xóa thành công");

    await GetDSPhieuDK();

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

    const ngayCap = ddmmyyyyToYmd($("#ngayCapEdit").val());
    const gioCap = $("#gioCapEdit").val().trim();

    dataSource.map(item => {
        const objectUpdate = {
            PhieuDK: selectSoPhieu,
            MaLenhSX: item.MaLenhSanXuat || item.MaLenhSX,
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
            NgayCap: ngayCap == "1990-01-01" ? item.NgayCap : ngayCap,
            GioCap: gioCap == "" ? item.GioCap : gioCap,
            IsNPL: item.IsNPL,
            NguoiDK: item.NguoiDK,
            Status: currentStatus,
        };

        arrSave.push(objectUpdate)
    })
    await API.Post("Post", arrSave, "Cập nhật thành công");
    $('#modalEditDKVT').modal('hide');
    await GetDSPhieuDK();

    // trả về lại null
    picker4.dates.setValue('');
    picker5.dates.setValue('');
}

/// HELPER FUNCTIONS
function updateGrid(grid, dataSource) {
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}

function resetModalState() {
    selectedItems = [];
    lstDataChiTietLenh = [];

    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#maLenhInput').val('');
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
        const isChecked = selectedItems.some(selected => selected.MaLenhSanXuat == item.MaLenhSanXuat);
        return `
            <div class="dropdown-item">
                <input type="checkbox" 
                       id="${item.MaLenhSanXuat}"
                       data-display="${item.Display}"
                       ${isChecked ? 'checked' : ''}>
                <label class="mb-0" for="${item.MaLenhSanXuat}">${item.Display}</label>
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
    const displayText = selectedItems.map(item => item.Display).join(', ');
    $('#maLenhInput').val(displayText);
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

/// EVENTS
$(document).ready(async function () {
    $(".select_2").select2();
    createViewDxDataGridDanhSachDangKy();
    createViewDxDataGridDangKyVatTu();
    createViewDxDataGridEditVatTu();
    createViewDxGridDanhSachPhieuDKVT();

    await GetMaLenh2();
    await GetDSPhieuDK();
    selectSoPhieu = $("#maphieu option:selected").val();
});

$(function () {
    $("#khachhang").val("all").select2('destroy').select2();

    $("#home").on("click", () => window.location.href = '/Home/Dashboard');

    $("#khachhang").on("change", function () {
        GetMaLenh()
    });

    $('#maLenhInput').click(function (e) {
        e.stopPropagation();
        $('#dropdownList').toggleClass('show');
        $('#searchInput').val('').focus();
        renderList(batchData);
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.select-container').length) {
            $('#dropdownList').removeClass('show');
        }
    });

    picker1 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' }
    });

    picker3 = new tempusDominus.TempusDominus(document.getElementById('timepicker'), {
        display: {
            components: {
                calendar: false, date: false, month: false, year: false,
                clock: true, hours: true, minutes: true, seconds: true
            }
        },
        localization: { format: 'HH:mm:ss' }
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

    picker5 = new tempusDominus.TempusDominus(document.getElementById('timepicker2'), {
        display: {
            components: {
                calendar: false, date: false, month: false, year: false,
                clock: true, hours: true, minutes: true, seconds: true
            }
        },
        localization: { format: 'HH:mm:ss' }
    });



    $("#tuNgay").trigger("click");
    $("#ngayCap").trigger("click");
    $("#gioCap").trigger("click");
    picker1.hide();
    picker2.hide();
    picker3.hide();

    $('#searchInput').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const filtered = batchData.filter(item => item.Display.toLowerCase().includes(searchTerm));
        renderList(filtered);
    });

    $('#btnSavePhieu').on("click", async function () {

        try {
            const dataSource = dxDataGridDangKyVatTu.option("dataSource");

            if (dataSource.length === 0) {
                showToast("warning", "Chưa có thông tin đăng ký");
                PlayAudioError();
                return;
            }

            const ngayDK = ddmmyyyyToYmd($("#tuNgay").val());
            const ngayCap = ddmmyyyyToYmd($("#ngayCap").val());
            const gioCap = $("#gioCap").val();

            const arrSave = dataSource.map(item => ({
                PhieuDK: '',
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
                NgayCap: ngayCap,
                GioCap: gioCap,
                IsNPL: item.NPL ? 1 : 0,
                NguoiDK: userNameSave,
                Status: currentStatus,
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

            await SavePhieu(arrSave);
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

                // Load trực tiếp không cache
                const allData = [];
                for (const item of batchData) {
                    const data = await GetLenhChiTiet(item.MaLenhSanXuat);
                    allData.push(...(Array.isArray(data) ? data : []));
                }
                lstDataChiTietLenh = allData;
            } else {
                selectedItems = [];
                lstDataChiTietLenh = [];
            }

            $('#itemList input[type="checkbox"]').prop('checked', isChecked);
            dxDataGridDangKyVatTu.option("dataSource", lstDataChiTietLenh);
            updateInput();
        } finally {
            dxDataGridDangKyVatTu.endCustomLoading();
        }
    });

    $(document).on('change', '#itemList input[type="checkbox"]', async function () {
        const itemId = $(this).attr('id');
        const isChecked = $(this).is(':checked');

        if (isChecked) {
            const isDuplicate = lstDataChiTietLenh.some(x => x.MaLenhSanXuat == itemId);

            if (isDuplicate) {
                showToast("warning", `Mã lệnh ${itemId} đã tồn tại trong danh sách!`);
                PlayAudioError();
                $(this).prop('checked', false);
                return;
            }

            const item = batchData.find(x => x.MaLenhSanXuat == itemId);
            if (item && !selectedItems.some(x => x.MaLenhSanXuat == itemId)) {
                selectedItems.push(item);
            }

            // Load trực tiếp không cache
            const data = await GetLenhChiTiet(itemId);
            const newData = Array.isArray(data) ? data : [];
            lstDataChiTietLenh = [...lstDataChiTietLenh, ...newData];
        } else {
            selectedItems = selectedItems.filter(x => x.MaLenhSanXuat != itemId);
            lstDataChiTietLenh = lstDataChiTietLenh.filter(x => x.MaLenhSanXuat != itemId);
        }

        dxDataGridDangKyVatTu.option("dataSource", lstDataChiTietLenh);
        updateCheckAll();
        updateInput();
    });

    $("#maphieu").on("change", function () {
        const $selectedOption = $(this).find("option:selected");

        selectSoPhieu = $selectedOption.val();
        selectSoPhieuDisplay = $selectedOption.data("display") || '';
        GetDSPhieuDK();
    });

    $("#btnShowModal").on("click", function () {
        // Reset các picker về ngày giờ hiện tại
        const now = new tempusDominus.DateTime();
        picker1.dates.setValue(now);
        picker2.dates.setValue(now);
        picker3.dates.setValue(now);

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
            const gioCap = moment(lstEditDangKyVatTu[0].GioCap, "HH:mm:ss").toDate();

            picker4.dates.setValue(tempusDominus.DateTime.convert(ngayCap));
            picker5.dates.setValue(tempusDominus.DateTime.convert(gioCap));
        }
        isShowNgayCapAndGioCap = true
        $("#modalEditDKVT").modal("show");
        $("#txtNgayChinhSua").text(selectSoPhieuDisplay)
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
            lstDataChiTietLenh = updatedDataSource;

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
                lstDataChiTietLenh = dataSource;
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
            DeletePhieu();
        }
    });

    // Khi đóng modal - reset tất cả checkbox và state
    $('#modalAddPhieu').on('hidden.bs.modal', function () {
        // Reset checkbox
        $('#itemList input[type="checkbox"]').prop('checked', false);
        $('#checkAll').prop('checked', false);
        $('#maLenhInput').val('');
        $('#dropdownList').removeClass('show');

        // Reset state
        selectedItems = [];
        lstDataChiTietLenh = [];
        $("#txtSLVTDelete").empty();
        // Clear grid
        if (dxDataGridDangKyVatTu) {
            updateGrid(dxDataGridDangKyVatTu, []);
        }
    });

    // Khi mở modal - khởi tạo lại
    $('#modalAddPhieu').on('shown.bs.modal', function () {
        GetKhachHang();
        GetPhieuMax();
        GetMaLenh();
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
        GetDSPhieuDKVT();

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
    $('#maLenhInput').val('');
    $('#dropdownList').removeClass('show');

    // Reset state
    selectedItems = [];
    lstDataChiTietLenh = [];
    $("#txtSLVTDelete").empty();
    // Clear grid
    if (dxDataGridDangKyVatTu) {
        updateGrid(dxDataGridDangKyVatTu, []);
    }

    $("#modalAddPhieu").modal("hide")
}

/// DX DataGrid
function createViewDxDataGridDangKyVatTu() {
    dxDataGridDangKyVatTu = $("#dxDataGridDangKyVatTu").dxDataGrid({
        dataSource: lstDataChiTietLenh,
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
                                item.SLDK = item.CapPhat
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

                    let result = formatNumber(value)
                    container.text(result);
                }

            },
            {
                dataField: "SLDK",
                caption: "SL Đăng Ký",
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

                        if (Number(value) > options.data.CapPhat) {
                            showToast("warning", "Số lượng đăng ký không được lớn hơn số lượng cấp phát");

                            const truncated = Math.trunc(options.data.CapPhat * 10000) / 10000;

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
            totalItems: [{
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
            }]
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

    $("#Layer_1").click();
}
function createViewDxDataGridDanhSachDangKy() {
    dxDataGridDanhSachDangKy = $("#dxDataGridDanhSachDangKy").dxDataGrid({
        dataSource: lstDataChiTietLenh,
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

            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 150 },
            { dataField: "Display", caption: "Mã Lệnh", alignment: "center", minWidth: 250 },
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
                dataField: "NgayCap",
                caption: "Ngày Cấp",
                alignment: "center",
                minWidth: 150,
                format: "dd/MM/yyyy",
                cellTemplate: function (container, options) {
                    const dateConvert = moment(options.value).format("DD/MM/YYYY")
                    container.text(dateConvert)
                }

            },
            {
                dataField: "GioCap",
                caption: "Giờ Cấp",
                alignment: "center",
                minWidth: 150,
                cellTemplate: function (container, options) {
                    if (!options.value) return;

                    const timeConvert = moment(options.value, "HH:mm:ss").format("HH:mm");
                    container.text(timeConvert);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                minWidth: 200,
            },
            {
                caption: "Xóa",
                minWidth: 100,
                alignment: "center",
                cellTemplate: function (container, options) {
                    if (options.data.CheckPXH != '0') {
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
            totalItems: [{
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

                        if (Number(value) > options.data.CapPhat) {
                            showToast("warning", "Số lượng đăng ký không được lớn hơn số lượng cấp phát");
                            this.value = options.data.CapPhat;
                            options.data.SLDK = options.data.CapPhat;
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