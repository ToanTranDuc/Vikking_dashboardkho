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
let phieuDKJoin = [];

let sizeInfoOptions = [];
let tempSelectedValue = [];

let cacheSoLuong = [];
let dataSoLuong = [];
let lenhDangKyVatTuOptions = [];
let chungLoaiThuVienCache = [];
let soLuongLoadedMaLenh = "";
let pickerTuNgayFilter, pickerDenNgayFilter;
let lstDSPhieuDKVT = [];
function resetModalState() {
    selectedItems = [];
    selectedRowsToSave = [];
    selectedItemsChungLoai = [];
    $('#itemList input[type="checkbox"]').prop('checked', false);
    $('#checkAll').prop('checked', false);
    $('#phieuDKXInput').val('');
    $('#maDHNhap').val('');
    $('#dropdownList').removeClass('show');

    /* GetPhieu();*/
    GetPhieu(false);

    updateGrid(dxDataGridDangKyVatTu, []);
}

function resetThuVienModalState() {
    selectedItemsChungLoai = [];
    cacheSoLuong = [];

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
    await GetPhieu(false);
/*    await GetPhieu();*/
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

function updateSummary(grid, checkbox_show) {
    checkgroup = checkbox_show || 0;
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
        const $summaryCell = $summaryRow.find('td').eq(sldkColumnIndex + checkgroup);
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
    let formattedInt = Number(intPart).toLocaleString();

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
    await GetDSPhieuDNTH();
});

$(function () {

    //bổ sung TP
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
        const hasFilter = $("#tkMaHang").val() || $("#tkMaLenh").val()
            || $("#tkNguoiLap").val() || $("#tkPhongBan").val();
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
            $("#trangThaiDuyet").val("all").trigger("change");

            await GetMaLenh2();
            showToast("success", "Nạp lại thành công");
        } catch (err) {
            console.error("Lỗi khi nạp lại:", err);
        } finally {
            $btn.prop("disabled", false);
            $icon.removeClass("fa-spin");
        }
    });
    //bổ sung TP
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

    $('#btnSavePhieu').on("click", async function () {
        SaveDeNghiThuHoi()

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
        const now = new tempusDominus.DateTime();
        picker1.dates.setValue(now);
        picker2.dates.setValue(now);

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
            if (rowDelete && rowDelete.PhieuCT_NgoaiDH && rowDelete.MaNPL) {
                DeleteItemInPhieu(rowDelete.PhieuCT_NgoaiDH, rowDelete.MaNPL);
            } else {
                showToast("error", "Dữ liệu không hợp lệ để xóa");
            }
        }

        $("#modalComfimrtDeleteVT").modal("hide");
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
    })

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
        /*await GetPhieu();*/
        await GetPhieu(true);
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
            
                updateGrid(dxDataPhieuDKVT, lstDSPhieuDKVT);
            } else {
               
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
        await loadSoLuong(true);
        //    GetPhieuDangKyXuat();
    })


    $("#btnNapLai").on("click", function () {
        GetPhieuMax();
        GetMaLenhDangKyVatTu();
        $("#phieuDKXInput").val("")
        $("#maDHNhap").val("");
        renderList([])
        selectedItems = []
        selectedRowsToSave = [];
        phieuDKJoin = [];
        createViewDxDataGridDangKyVatTu([])
    })
    $("#chungloaiSelect").on("change", function () {
        const maCLVT = $(this).val();
        selectedItemsChungLoai = [];
        loadChungLoai()

    })
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
        PhieuCT_NgoaiDH: '',
        MaLenhSX: item.MaLenhSanXuat,
        MaDH: item.MaDH,
        MaLenh: item.MaLenh,
        MaNPL: item.MaNPL,
        SLDK: item.SLDK,
        GhiChu: item.GhiChu,
        MaNhom: item.MaNhom,
        MaVTID: item.MaVTID,
        MaMauVT: item.MaMauVT,
        MauVTID: item.MauVTID,
        KhoVaiID: item.KhoVaiID,
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
        IsTV: item.IsTV,
        MaNhomChiTiet: getMaNhom(item.ChungLoaiChiTiet),
        PhatSinhChiPhi: item.PhatSinhChiPhi == 1 ? 1 : 0,
        LiDo: item.LiDo
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
    //show chữ ký
    showConfirmModalSign(async function () {
        if (signatureHistory.length === 0) {
            alert('Vui lòng ký tên trước khi lưu!');
            return;
        }
        // Chuyển canvas thành hình ảnh
        const signatureImage = canvas.toDataURL('image/png');
        arrSave.forEach(x => {
            x.SignNgDK = signatureImage

        })
        await SavePhieu(arrSave);

        const arrTV = dataToSave.filter(item => item.IsTV === 1);
        if (arrTV.length > 0) {
            await SavePhieuThuVien(arrTV, signatureImage);
        }
        /*  alert('' + userNameSave)*/
        var maPhieu = $("#maphieu").val();

        sendNotify(userNameSave, "M.53.00.00", "Duyệt cấp thêm",
            "Yêu cầu duyệt phiếu " + maPhieu,
            "TBP", "ALL", 1);
        return true; // ✅ cho đóng
    })

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

/// DX DataGrid
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
                                selectedRowsToSave.push(options.data);
                            }
                        } else {
                            selectedRowsToSave = selectedRowsToSave.filter(item =>
                                !isSameDangKyRow(item, options.data)
                            );
                        }

                        dxDataGridDangKyVatTu.option("dataSource", sortDangKyDataSource(dataSource));
                        dxDataGridDangKyVatTu.refresh();
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
            { dataField: "ChungLoaiVatTu", caption: "Chủng loại", alignment: "center", minWidth: 180 },
            //{ dataField: "ChungLoaiVatTu", caption: "Chủng loại vật tư", alignment: "center", minWidth: 180, visible: false },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "MaNhomChiTiet", caption: "Item Code", alignment: "center", minWidth: 180, visible: false },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MaMauVT", caption: "Mã Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 120 },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
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
                        const checkbox_show = 1

                        // Không cho số âm
                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridDangKyVatTu, checkbox_show);
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
                        //    updateSummary(dxDataGridDangKyVatTu);
                        //    return;
                        //}

                        options.data.SLDK = value === "" ? null : Number(value);
                        updateSummary(dxDataGridDangKyVatTu, checkbox_show);
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

                        clearTimeout(options.data.__refreshTimer);
                        options.data.__refreshTimer = setTimeout(function () {
                            dxDataGridDangKyVatTu.repaintRows([options.rowIndex]);
                        }, 300);
                    });

                    container.append($input);
                }
            },
            {
                dataField: "TonKho",
                caption: "SL Tồn kho",
                alignment: "center",
                minWidth: 110,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    const value = options.value || 0;
                    if (value === null || value === undefined || value === "") {
                        container.text(0);
                        return;
                    }
                    const num = parseFloat(value);
                    $("<span>")
                        .text(formatThousands(value))
                        .css({ color: num <= 0 ? "red" : "inherit" })
                        .appendTo(container);
                }
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                visible: false,
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
                dataField: "LiDo",
                caption: "Lí do",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control"
                               value="${options.data.LiDo || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.LiDo = this.value;
                    });

                    container.append($input);
                }
            },
            {
                dataField: "PhatSinhChiPhi",
                caption: "Phát sinh chi phí",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowHeaderFiltering: false,
                visible: false,
                cellTemplate: function (container, options) {
                    const $checkbox = $(`
                        <input type="checkbox"
                            style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkbox.prop("checked",
                        options.data.PhatSinhChiPhi == 1 || options.data.PhatSinhChiPhi === true
                    );

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
                dataField: "NgoaiDM",
                caption: "Ngoài định mức",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowHeaderFiltering: false,
                visible: false,
                cellTemplate: function (container, options) {
                    const $checkbox = $(`
                        <input type="checkbox"
                            style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkbox.prop("checked",
                        options.data.NgoaiDM == 1 || options.data.NgoaiDM === true
                    );

                    $checkbox.on("change", function () {
                        options.data.NgoaiDM = this.checked ? 1 : 0;
                    });

                    container.css({
                        justifyContent: "center",
                        alignItems: "center"
                    });

                    container.append($checkbox);
                }
            },
            {
                dataField: "TrongDM",
                caption: "Trong định mức",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowHeaderFiltering: false,
                visible: false,
                cellTemplate: function (container, options) {
                    const $checkbox = $(`
                        <input type="checkbox"
                            style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkbox.prop("checked",
                        options.data.NgoaiDM == 1 || options.data.NgoaiDM === true
                    );

                    $checkbox.on("change", function () {
                        options.data.NgoaiDM = this.checked ? 1 : 0;
                    });

                    container.css({
                        justifyContent: "center",
                        alignItems: "center"
                    });

                    container.append($checkbox);
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
                    column: "TonKho",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatThousands(e.value)
                    }
                },
                {
                    column: "SLDK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatThousands(e.value)
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
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

                const isCheckedRow = isDangKyRowSelected(e.data);
                if (isCheckedRow) {
                    $(e.cellElement).css({
                        "background-color": "#bdd5f0",
                        "border-color": "#8aa8c6",
                        "box-shadow": "inset 0 0 0 1px #41414101"
                    });
                } else {
                    $(e.cellElement).css({
                        "background-color": "",
                        "border-color": "",
                        "box-shadow": ""
                    });
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
        },
    }).dxDataGrid("instance");
}
function renderSignCell(container, options, fieldName) {
    if (!options.data) {
        container.text('');
        return;
    }

    const data = options.data;
    const value = data[fieldName];

    const $wrapper = $("<div>").css({
        width: "100%",
        height: "40px",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        position: "relative"
    });

    // === Hiển thị chữ ký ===
    if (value && value !== "checked") {
        // Chữ ký đã ký (ảnh)
        const tenField = {
            SignNgDK: "TenUserNgDK_Ten",
            SignTBPNgDK: "TenUserTBPNDK_Ten",
            SignMer: "TenUserMerSoatSet_Ten",
            SignTBPMer: "TenUserTBPMerSoatSet_Ten"
        }[fieldName];

        const $signWrapper = $("<div>").css({
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: "2px"
        });

        $("<img>")
            .attr("src", `/Images/SignDeNghiCapThemNPL_NgoaiDH/${value}?${Date.now()}`)
            .css({ width: "70px", height: "35px", objectFit: "contain" })
            .appendTo($signWrapper);

        if (data[tenField]) {
            $("<span>").text(data[tenField]).css({
                fontSize: "11px", color: "#333", fontWeight: "bold"
            }).appendTo($signWrapper);
        }
        $signWrapper.appendTo($wrapper);

    } else if (value === "checked") {
        const tenField = {
            SignNgDK: "TenUserNgDK_Ten",
            SignTBPNgDK: "TenUserTBPNDK_Ten",
            SignMer: "TenUserMerSoatSet_Ten",
            SignTBPMer: "TenUserTBPMerSoatSet_Ten"
        }[fieldName];

        const hoTenField = {
            SignNgDK: "HoTenNgDK",
            SignTBPNgDK: "HoTenTBPNDK",
            SignMer: "HoTenMerSoatSet",
            SignTBPMer: "HoTenTBPMerSoatSet"
        }[fieldName];

        const hinhAnhField = {
            SignNgDK: "HinhAnhNgDK",
            SignTBPNgDK: "HinhAnhTBPNDK",
            SignMer: "HinhAnhMerSoatSet",
            SignTBPMer: "HinhAnhTBPMerSoatSet"
        }[fieldName];

        const ten = data[hoTenField] || data[tenField] || "";
        const hinhAnh = data[hinhAnhField];

        if (hinhAnh) {
            const $signWrapper = $("<div>").css({
                display: "flex", flexDirection: "column", alignItems: "center", gap: "2px"
            });
            $("<img>")
                .attr("src", `/Images/NhanVien/${hinhAnh}`)
                .css({ width: "70px", height: "35px", objectFit: "cover", borderRadius: "3px" })
                .on("error", function () { $(this).hide(); })
                .appendTo($signWrapper);
            if (ten) $("<span>").text(ten).css({ fontSize: "11px", fontWeight: "bold" }).appendTo($signWrapper);
            $signWrapper.appendTo($wrapper);
        } else if (ten) {
            $("<span>").text(ten).css({ fontSize: "12px", fontWeight: "bold", color: "#333" }).appendTo($wrapper);
        }
    }

    // === Nút ký (chỉ hiển thị nếu chưa ký) ===
    let canSign = false;
    const signNgDK = data.SignNgDK || "";
    const signTBPNgDK = data.SignTBPNgDK || "";
    const signMer = data.SignMer || "";
    const signTBPMer = data.SignTBPMer || "";

    if (fieldName === "SignNgDK" && !signNgDK) canSign = true;
    else if (fieldName === "SignTBPNgDK" && signNgDK && !signTBPNgDK) canSign = true;
    else if (fieldName === "SignMer" && signTBPNgDK && !signMer) canSign = true;
    else if (fieldName === "SignTBPMer" && signMer && !signTBPMer) canSign = true;

    if (canSign) {
        // ✅ THÊM CHECKBOX xác nhận nhanh
        $("<input>")
            .attr("type", "checkbox")
            .css({
                cursor: "pointer",
                position: "absolute",
                left: "4px",
                top: "13px",
                width: "14px",
                height: "14px",
                accentColor: "#007bff"
            })
            .on("change", async function () {
                if (!this.checked) return;
                const rowData = options.data;
                const phieu = rowData.PhieuCT_NgoaiDH;
                const allRows = dxDataGridDanhSachDangKy.option("dataSource")
                    .filter(r => r.PhieuCT_NgoaiDH === phieu);

                let arrSave = allRows.map(r => ({
                    ...r,
                    SignNgDK: '',
                    SignTBPNgDK: '',
                    SignMer: '',
                    SignTBPMer: '',
                    [fieldName]: "checked",
                    NguoiTH: userNameSave
                }));

                await UpdateKiTenPhieu(arrSave);

                if (fieldName === "SignTBPMer") {
                    sendNotify(userNameSave, "M.58.00.00", "Duyệt cấp thêm ngoài đơn hàng",
                        "Yêu cầu duyệt phiếu " + phieu, "PKH", "ALL", 1);
                } else if (fieldName === "SignMer") {
                    sendNotify(userNameSave, "M.58.00.00", "Duyệt cấp thêm ngoài đơn hàng",
                        "Yêu cầu duyệt phiếu " + phieu, "PKH", "ALL", -1);
                } else if (fieldName === "SignTBPNgDK") {
                    sendNotify(userNameSave, "M.58.00.00", "Duyệt cấp thêm ngoài đơn hàng",
                        "Yêu cầu duyệt phiếu " + phieu, "ALL", "ALL", -1);
                }
            })
            .appendTo($wrapper);

        // Icon bút ký (giữ nguyên)
        $("<i>")
            .addClass("fa-solid fa-signature")
            .css({
                cursor: "pointer",
                color: "#007bff",
                fontSize: "16px",
                position: "absolute",
                right: "8px",
                top: "10px"
            })
            .on("click", function () {
                const rowData = options.data;
                const phieu = rowData.PhieuCT_NgoaiDH;

                showConfirmModalSign(async function () {
                    const signatureImage = canvas.toDataURL('image/png');
                    const allRows = dxDataGridDanhSachDangKy.option("dataSource")
                        .filter(r => r.PhieuCT_NgoaiDH === phieu);

                    let arrSave = allRows.map(r => ({
                        ...r,
                        SignNgDK: '',
                        SignTBPNgDK: '',
                        SignMer: '',
                        SignTBPMer: '',
                        [fieldName]: signatureImage,
                        NguoiTH: userNameSave
                    }));

                    await UpdateKiTenPhieu(arrSave);
                    if (fieldName === "SignTBPNgDK") {
                        sendNotify(userNameSave, "M.58.00.00", "Duyệt cấp thêm ngoài đơn hàng",
                            "Yêu cầu duyệt phiếu " + phieu,   
                            "PKH", "ALL", -1);
                    } else if (fieldName === "SignMer") {
                        sendNotify(userNameSave, "M.58.00.00", "Duyệt cấp thêm ngoài đơn hàng",
                            "Yêu cầu duyệt phiếu " + phieu,  
                            "PKH", "ALL", 1);
                    } else if (fieldName === "SignTBPMer") {
                        sendNotify(userNameSave, "M.58.00.00", "Duyệt cấp thêm ngoài đơn hàng",
                            "Yêu cầu duyệt phiếu " + phieu,   
                            "ALL", "ALL", -1);
                    }
                    return true;
                });
            })
            .appendTo($wrapper);
    }

    $wrapper.appendTo(container);
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
                dataField: "PhieuCT_NgoaiDH",
                caption: "Số Phiếu",
                alignment: "left",
                minWidth: 200,
                groupIndex: 0,   
                visible: false,
                calculateGroupValue: function (rowData) {
                    return [
                        rowData.PhieuCT_NgoaiDH || '',
                        rowData.MaLenh || '',
                        rowData.MaHang || rowData.MaDH || ''
                    ].join('||');
                },
                groupCellTemplate: function (container, options) {
                    const parts = (options.value || '').split('||');
                    const phieu = parts[0] || '';
                    const lenh = parts[1] || '';
                    const maHang = parts[2] || '';

                    const text = [
                        phieu ? `Số Phiếu: ${phieu}` : '',
                        lenh ? `LSX: ${lenh}` : '',
                        maHang ? `MH: ${maHang}` : ''
                    ].filter(Boolean).join(' | ');

                    $('<span>')
                        .text(text)
                        .css({
                            'font-weight': '500',
                            'font-size': '15px'
                        })
                        .appendTo(container);
                }
            },
            {
                caption: "STT",
                alignment: "center",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    const component = options.component;
                    const currentPhieu = options.data.PhieuCT_NgoaiDH;

                    let stt = 0;
                    const visibleRows = component.getVisibleRows();

                    for (let i = 0; i < visibleRows.length; i++) {
                        const row = visibleRows[i];
                        if (row.rowType !== "data") continue;
                        if (row.data.PhieuCT_NgoaiDH !== currentPhieu) continue;

                        stt++;
                        if (row.data === options.data) break;
                    }

                    container.text(stt || "");
                }
            },
            { dataField: "ChungLoaiVatTu", caption: "Chủng loại", alignment: "center", minWidth: 180 },
            { dataField: "MaDH", caption: "Mã ĐH", alignment: "center", minWidth: 150, visible: false },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "IsTV", caption: "IsTV", alignment: "center", minWidth: 100, visible: false },
            { dataField: "MaMauVT", caption: "Mã Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "MauVT", caption: "Màu VT", alignment: "center", minWidth: 120 },
            { dataField: "KhoVai", caption: "Khổ/Size", alignment: "center", minWidth: 120 },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "SLDK",
                caption: "SL Cấp thêm",
                alignment: "center",
                minWidth: 120,
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let result = formatThousands(value)
                    container.text(result);
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
                        .text(formatThousands(value))
                        .css({ color: num <= 0 ? "red" : "inherit" })
                        .appendTo(container);
                }
            },

            {
                dataField: "NgayTH",
                caption: "Ngày YC Cấp thêm",
                alignment: "center",
                minWidth: 150,
                format: "dd/MM/yyyy",
                cellTemplate: function (container, options) {
                    const dateConvert = moment(options.value).format("DD/MM/YYYY")
                    container.text(dateConvert)
                }

            },

            {
                dataField: "LiDo",
                caption: "Lí do",
                minWidth: 200,
            },
            {
                dataField: "PhatSinhChiPhi",
                caption: "Phát sinh chi phí",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowHeaderFiltering: false,
                visible: false,
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
                dataField: "SignNgDK",
                caption: "Người ĐK",
                visible:true,
                minWidth: 120,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignNgDK");
                }
            },
            {
                dataField: "SignTBPNgDK",
                caption: "TBP người ĐK",
                visible: true,
                minWidth: 150,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignTBPNgDK");
                }
            },
            {
                dataField: "SignMer",
                caption: "Mer soát sét",
                visible: true,
                minWidth: 120,
                cellTemplate(container, options) {
                    renderSignCell(container, options, "SignMer");
                }
            },
            {
                dataField: "SignTBPMer",
                caption: "TBP Mer soát sét",
                visible: true,
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
                                .addClass("fa-solid fa-trash")
                                .css({ fontSize: "16px", color: "red", cursor: "pointer" })
                                .on("click", function () {
                                    rowDelete = options.data;
                                    isDeleteAll = false;

                                    $("#modalComfimrtDeleteVT").modal('show');
                                })
                        );

                    $wrapper.append($btnDelete, $btnEdit).appendTo(container)
                }

            }
        ],
        summary: {
            totalItems: [
                {
                    column: "TonKho",
                    summaryType: "sum",
                    showInColumn: "TonKho",
                    displayFormat: "{0}",
                    customizeText(e) {
                        if (e.value == null) return "";
                        let str = e.value.toString();
                        let [intPart, decPart] = str.includes(".") ? str.split(".") : [str, ""];
                        if (decPart) decPart = decPart.substring(0, 4);
                        const formattedInt = Number(intPart).toLocaleString("en");
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
        onContentReady: function (e) {
            setTimeout(function () {
                mergeSignCells();
            }, 50);
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
                allowEditing: false,
                visible: false
            },
            {
                dataField: "MaLenh",
                caption: "Mã Lệnh",
                alignment: "center",
                minWidth: 100,
                allowEditing: false,
                visible: false
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 180,
                allowEditing: false
            },
            {
                dataField: "MaMauVT",
                caption: "Mã Màu VT",
                alignment: "center",
                minWidth: 120,
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

                    let result = formatThousands(value)
                    container.text(result);
                },
                visible: false
            },
            {
                dataField: "SLDK",
                caption: "SL Cấp thêm",
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
                            showToast("warning", "Số lượng cấp thêm không được âm");
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

                        options.data.SLDK = value === "" ? null : value
                        updateSummary(dxDataGridEditVatTu);

                        const rowIndex = options.rowIndex;
                        const phanTramColIndex = dxDataGridDangKyVatTu.columnCount() - 1;

                    });
                    container.append($input);
                }
            },
            {
                dataField: "LiDo",
                caption: "Lí do",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control"
                               value="${options.data.LiDo || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.LiDo = this.value;
                    });

                    container.append($input);
                }
            },
            {
                dataField: "PhatSinhChiPhi",
                caption: "Phát sinh chi phí",
                alignment: "center",
                width: 80,
                allowFiltering: false,
                allowHeaderFiltering: false,
                visible: false,
                cellTemplate: function (container, options) {

                    const $checkbox = $('<input type="checkbox" style="width:18px;height:18px;cursor:pointer;" />');

                    $checkbox.prop("checked", options.data.PhatSinhChiPhi == 1 || options.data.PhatSinhChiPhi === true);

                    $checkbox.on("change", function () {
                        options.data.PhatSinhChiPhi = this.checked ? 1 : 0;
                    });

                    container.css({
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center"
                    });

                    container.append($checkbox);
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
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
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
            // THÊM CỘT CHECKBOX Cấp thêm cái nào
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

                            // ✅ Đẩy lên sau các row đang được check






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
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 180 },
            { dataField: "MaNPL", caption: "Mã NPL", alignment: "center", minWidth: 120, visible: false },
            { dataField: "MaMauVT", caption: "Mã Màu VT", alignment: "center", minWidth: 120 },
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

                    const $text = $("<span></span>").text(formatThousands(value));

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
                visible: false,
                allowEditing: false,
                cellTemplate: function (container, options) {
                    container.text(formatThousands(options.value || 0));
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

                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng đăng ký không được âm");
                            this.value = "";
                            options.data.SLDK = null;
                            updateSummary(dxDataGridThuVien);
                            return;
                        }

                        if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }

                        options.data.SLDK = value === "" ? null : Number(value);
                        handleAutoCheckThuVien(options.data, options.rowIndex);
                        updateSummary(dxDataGridThuVien); clearTimeout(options.data.__refreshTimer);
                        options.data.__refreshTimer = setTimeout(function () {
                            dxDataGridDangKyVatTu.repaintRows([options.rowIndex]);
                        }, 300);
                    });

                    container.append($input);
                }
            },
            {
                dataField: "TonKho",
                caption: "SL Tồn kho",
                alignment: "center",
                minWidth: 80,
                visible: true,
                cellTemplate: function (container, options) {
                    container.append(formatThousands(options.value))
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
                caption: "Ghi chú",
                minWidth: 150,
                visible: false
            },
            {
                dataField: "LiDo",
                caption: "Lí do",
                minWidth: 150,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control inputSLDK input-sldk"
                               value="${options.data.LiDo || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.LiDo = this.value;
                        handleAutoCheckThuVien(options.data, options.rowIndex);
                    });

                    container.append($input);
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "TonKho",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatThousands(e.value)
                    }
                },
                {
                    column: "SLDK",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatThousands(e.value)
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
        opened: true,           
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
});

// Ký nhanh
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