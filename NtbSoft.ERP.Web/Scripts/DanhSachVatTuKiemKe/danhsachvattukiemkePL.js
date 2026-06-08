/// VARIABLE
let dxDanhSachVatTuKiemKe;
let dxPhieuDanhSachVatTuKiemKe;
let dxDanhSachItemCode;
let lstDanhSachItemCode = [];
var userNameSave = localStorage.getItem("username1");
var selectedRow = [];
var selectedRowDelete = [];
let batchData;
let dot;
let selectedItems = [];
let lastSelectedMaCLVT;

$(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })

    $(".select_2").select2()
    $(".select_2").select2({
        minimumResultsForSearch: Infinity
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.select-container').length) {
            $('#dropdownList').removeClass('show');
        }
    });

    // thay đổi từ ngày
    $("#tuNgay").on("change", function () {
        GetKH()
    })
    $("#denNgay").on("change", function () {
        GetKH()
    })

    $("#tuNgayTab2").on("change", function () {
        GetPhieuDanhSachVatTuKiemKe()
    })
    $("#denNgayTab2").on("change", function () {
        GetPhieuDanhSachVatTuKiemKe()
    })

    $("#phieuvattukiemke").on("change", function () {
        GetDataPhieuDanhSachVatTuKiemKe()
    })

    // thay đổi khách hàng 
    $("#khachhang").on("change", async function (e) {
        selectedItems = []
        updateGrid(dxDanhSachVatTuKiemKe, [])
        $("#chungLoaiInput").val("")
        await GetChungLoai()


        //// Nếu có items đã chọn, reload lại data
        //if (selectedItems.length > 0) {
        //    const selectedMaCLVT = selectedItems.map(item => item.MaCLVT).join(';');
        //    await GetDanhSachVatTuKiemKe(selectedMaCLVT);
        //}
    });

    // Nút Lưu Danh Sách
    $("#btnSave").on("click", async function (e) {
        showConfirmModal(async function () {
            let data = selectedRow;
            const tuNgay = ddmmyyyyToYmd($('#tuNgay').val());
            const denNgay = ddmmyyyyToYmd($('#denNgay').val());
            const arrSave = []
            if (data.length === 0) {
                showToast("warning", "Không có thông tin danh sách để lưu")
                return;
            }

            data.forEach(item => {
                const object = {
                    PhieuVatTuKK: '',
                    MaNPL: item.MaNPL,
                    MaVT: item.MaVT,
                    MauVT: item.MauVT,
                    KhoVai: item.KhoVai,
                    MaDVVT: item.TenDVVT,
                    NguoiTao: userNameSave,
                    IsLog: 1,
                    IsNPL: isNPL,
                    GhiChu: item.GhiChu ?? '',
                    Dot: dot,
                    TonDKy: item.TonDKy,
                    TonCKy: item.TonCKy,
                    TuNgay: tuNgay,
                    DenNgay: denNgay,
                    SLNhapKho: item.SLNK,
                    ChiTiet: item.ChiTiet
                };
                arrSave.push(object);
            });

            // Lưu danh sách
            await SaveDanhSachVatTuKiemKe(arrSave)
            const module = 'M.38.00.00'
            const title = 'Xác nhận vật tư kiểm kê'
            const detail = `Phiếu: ${$("#txtSoPhieu").val()} đã có danh vật tư cần xác nhận kiểm kê !`
            const sendTo = 'KHo'
            const BoPhan = 'ALL'
            const Status = 1

            sendNotify(module, title, detail, sendTo, BoPhan, Status)
        })

    })
    // Nút show modal xác nhận hủy
    $("#btnHuyKiemKe").on("click", function () {
        if (selectedRowDelete.length === 0) {
            showToast("warning", "Không có thông tin để hủy kiểm kê !")
            return;
        }

        showConfirmModalDelete(async function () {
            const maPhieu = $("#phieuvattukiemke").val();
            const url = `/api/DangKyVatTu/PostKiemKe?action=DeleteVTKK&para1=${maPhieu}`
            const arrDelete = [];
            try {

                selectedRowDelete.forEach(item => {
                    const object = {
                        PhieuVatTuKK: item.PhieuVatTuKK,
                        MaNPL: item.MaNPL,
                        MaVT: item.MaVT,
                        MauVT: item.MauVT,
                        KhoVai: item.KhoVai,
                        MaDVVT: item.TenDVVT,
                        NguoiTao: item.NguoiTao,
                        IsLog: item.IsLog,
                        IsNPL: isNPL,
                        GhiChu: item.GhiChu,
                        Dot: item.Dot,
                        TonDKy: item.TonDKy,
                        TonCKy: item.TonCKy,
                        TuNgay: item.TuNgay,
                        DenNgay: item.DenNgay,
                        SLNhapKho: item.SLNK,
                        ChiTiet: item.ChiTiet
                    };
                    arrDelete.push(object)
                })
                const response = await fetch(url, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(arrDelete)
                })

                const result = await response.json()

                if (result == "True") {
                    showToast("success", `Xóa Thành Công ${arrDelete.length} Vật Tư!`)
                    GetDataPhieuDanhSachVatTuKiemKe();
                    selectedRowDelete = [];
                    $("#txtSLVTDelete").empty();
                    GetPhieuDanhSachVatTuKiemKe();
                }

            } catch (error) {
                console.error(error)
            }
        })
    })
    // Nút xác nhận xóa
    $("#btnConfirmtDeletePhieu").on("click", async function () {

    })
    // Xử lý khi thay đổi lọc
    $("#loc").on("change", function () {
        const value = $(this).val()
        if (value == "2") {
            $(".filterThoiGian").removeClass("d-none")
            $(".btnHuyKiemKe").removeClass("col-4 col-md-6 col-lg-8 col-xl-8")
            $(".btnHuyKiemKe").addClass("col-8 col-md-12 col-lg-4 col-xl-4")
        } else {
            $(".filterThoiGian").addClass("d-none")
            $(".btnHuyKiemKe").removeClass("col-8 col-md-12 col-lg-4 col-xl-4")
            $(".btnHuyKiemKe").addClass("col-4 col-md-6 col-lg-8 col-xl-8")

        }
    })

    // Nút bật modal tìm nhanh
    $("#btnTimNhanh").on("click", function () {
        GetDSItemCode();

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
                updateGrid(dxDanhSachItemCode, lstDanhSachItemCode);
            } else {
                // Lọc dữ liệu
                const dataFiltered = lstDanhSachItemCode.filter(item =>
                    String(item.MaVT).trim().toLowerCase().includes(search) ||
                    String(item.MauVT).trim().toLowerCase().includes(search)
                );
                updateGrid(dxDanhSachItemCode, dataFiltered);
            }
        }, 100); // Delay 300ms sau khi ngừng gõ
    });


    $("#modalTimNhanh").on("shown.bs.modal", function () {
        setTimeout(function () {
            $("#inputTimKiem").focus();
        }, 100);
    });

    $("#modalTimNhanh").on("hide.bs.modal", function () {
        $("#inputTimKiem").val("").trigger("input")
    });

    $('#checkAll').change(async function () {
        const isChecked = $(this).is(':checked');

        dxDanhSachVatTuKiemKe.beginCustomLoading("Đang tải dữ liệu...");

        try {
            if (isChecked) {
                selectedItems = [...batchData];
                $('#itemList input[type="checkbox"]').prop('checked', true);

                const allMaCLVT = batchData.map(item => item.MaCLVT).join(';');
                lastSelectedMaCLVT = allMaCLVT; // LƯU LẠI

                const data = await GetDanhSachVatTuKiemKe(allMaCLVT);

            } else {
                selectedItems = [];
                lastSelectedMaCLVT = ""; // XÓA LƯU TRỮ
                $('#itemList input[type="checkbox"]').prop('checked', false);

                dxDanhSachVatTuKiemKe.option("dataSource", []);
                selectedRow = [];
            }

            updateInput();

        } catch (error) {
            console.error('Error in checkAll:', error);
        } finally {
            dxDanhSachVatTuKiemKe.endCustomLoading();
        }
    });

    $(document).on('change', '#itemList input[type="checkbox"]', async function () {
        const itemId = $(this).attr('id');
        const isChecked = $(this).is(':checked');

        dxDanhSachVatTuKiemKe.beginCustomLoading("Đang tải dữ liệu...");

        try {
            if (isChecked) {
                // Thêm item vào selectedItems
                const item = batchData.find(x => x.MaCLVT == itemId);
                if (item && !selectedItems.some(x => x.MaCLVT == itemId)) {
                    selectedItems.push(item);
                }
            } else {
                // Xóa item khỏi selectedItems
                const index = selectedItems.findIndex(x => x.MaCLVT == itemId);
                if (index > -1) {
                    selectedItems.splice(index, 1);
                }
            }

            // Join tất cả MaCLVT đã chọn bằng dấu ";"
            const selectedMaCLVT = selectedItems.map(item => item.MaCLVT).join(';');
            lastSelectedMaCLVT = selectedMaCLVT; // LƯU LẠI

            // Nếu có items được chọn, gọi API
            if (selectedMaCLVT) {
                await GetDanhSachVatTuKiemKe(selectedMaCLVT);
            } else {
                // Nếu không có gì được chọn, clear grid
                dxDanhSachVatTuKiemKe.option("dataSource", []);
                selectedRow = [];
            }

            updateCheckAll();
            updateInput();

        } catch (error) {
            console.error('Error in checkbox change:', error);
        } finally {
            dxDanhSachVatTuKiemKe.endCustomLoading();
        }
    });

    $('#chungLoaiInput').click(function (e) {
        e.stopPropagation();
        $('#dropdownList').toggleClass('show');
        $('#searchInput').val('').focus();
    });

    $('#searchInput').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const filtered = batchData.filter(item => item.ChungLoaiVatTu.toLowerCase().includes(searchTerm));
        renderList(filtered);
    });


})


/// HELPER FUNCTIONS
function getFirstDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
}

function getLastDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 1, 0);
}

function ddmmyyyyToYmd(dateStr) {
    if (!dateStr) return "1990-01-01";
    const [d, m, y] = dateStr.split("/");
    return `${y}-${m}-${d}`;
}

function updateGrid(grid, dataSource) {
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}

function updateCheckAll() {
    const total = $('#itemList input[type="checkbox"]').length;
    const checked = $('#itemList input[type="checkbox"]:checked').length;
    $('#checkAll').prop('checked', total > 0 && total == checked);
}

function updateInput() {
    const displayText = selectedItems.map(item => item.ChungLoaiVatTu).join(', ');
    $('#chungLoaiInput').val(displayText);
}

// API
async function GetKH() {
    const tuNgay = ddmmyyyyToYmd($("#tuNgay").val())
    const denNgay = ddmmyyyyToYmd($("#denNgay").val())
    const url = `/api/DangKyVatTu/Get?action=GetKhachHang&para1=${tuNgay}&para2=${denNgay}&para3=''&para4=''&para5=${isNPL}`
    const $khachHangSelect = $("#khachhang");
    $khachHangSelect.empty().append(`<option value="all">Tất cả</option>`);
    try {
        const response = await fetch(url)
        const data = await response.json()

        if (data.length > 0) {
            let html = ''
            data.map(kh => {
                html += `<option value="${kh.MaKH}">${kh.TenKH}</option>`
            })
            $khachHangSelect.append(html);
        }
        $khachHangSelect.select2()
        GetChungLoai();
    } catch (err) {
        console.error(err)
    }
}
async function GetChungLoai() {
    const tuNgay = ddmmyyyyToYmd($("#tuNgay").val())
    const denNgay = ddmmyyyyToYmd($("#denNgay").val())
    const maKH = $("#khachhang option:selected").val()
    const url = `/api/DangKyVatTu/Get?action=GetChungLoaiVT&para1=${tuNgay}&para2=${denNgay}&para3=${maKH}&para4=''&para5=${isNPL}`
    try {
        const response = await fetch(url)
        const data = await response.json()
        batchData = data;
        renderList(batchData)
    } catch (err) {
        console.error(err)
    }
}

/// GET
async function GetDanhSachVatTuKiemKe(maChungLoai) {
    const maKH = $("#khachhang option:selected").val()
    const tuNgay = ddmmyyyyToYmd($("#tuNgay").val())
    const denNgay = ddmmyyyyToYmd($("#denNgay").val())
    const url = `/api/DangKyVatTu/Get?action=GetDanhSachVatTu&para1=${tuNgay}&para2=${denNgay}&para3=${maKH}&para4=${maChungLoai}&para5=${isNPL}`

    // Bắt đầu loading
    dxDanhSachVatTuKiemKe.beginCustomLoading('Đang tải dữ liệu...');

    try {
        const response = await fetch(url)
        const data = await response.json()

        data.forEach(item => {
            item.isCheck = 1;
        });

        // update grid
        updateGrid(dxDanhSachVatTuKiemKe, data)
        selectedRow = data

        setTimeout(() => {
            updateSummary(dxDanhSachVatTuKiemKe);
        }, 100);

        return data
    } catch (err) {
        console.error(err)
    } finally {
        dxDanhSachVatTuKiemKe.endCustomLoading();
    }
}
async function GetPhieuDanhSachVatTuKiemKe() {
    const loc = $("#loc").val()
    const tuNgayTab2 = loc == "1" ? '1990-01-01' : ddmmyyyyToYmd($("#tuNgayTab2").val());
    const denNgayTab2 = loc == "1" ? '1990-01-01' : ddmmyyyyToYmd($("#denNgayTab2").val());
    const $phieuSlect = $("#phieuvattukiemke")
    const url = `/api/DangKyVatTu/Get?action=GetPhieuDSVatTuKiemKe&para1=${tuNgayTab2}&para2=${denNgayTab2}&para3=${isNPL}`

    $phieuSlect.empty()
    $phieuSlect.append(`<option value=''></option>`)
    try {
        const response = await fetch(url)
        const data = await response.json()
        let html = ''

        if (data.length > 0) {
            data.map(phieu => {
                html += `<option value="${phieu.PhieuVatTuKK}">${phieu.Display}</option>`
            })
        }
        $phieuSlect.append(html)
        GetDataPhieuDanhSachVatTuKiemKe()
    } catch (err) {
        console.error(err)
    }
}
async function GetDataPhieuDanhSachVatTuKiemKe() {
    const phieuVTKK = $("#phieuvattukiemke").val()
    var url = `/api/DangKyVatTu/Get?action=GetDSPhieuVTKK&para1=${phieuVTKK}&para2=${isNPL}`
    try {
        const response = await fetch(url)
        const data = await response.json()

        // update grid
        updateGrid(dxPhieuDanhSachVatTuKiemKe, data)

    } catch (err) {
        console.error(err)
    }
}
async function GetDot() {
    const url = `/api/DangKyVatTu/Get?action=GetPhieuDK&para1=${isNPL}`

    try {
        const response = await fetch(url)
        const data = await response.json()
        dot = data[0].PKK
        console.log("dot: ", dot)
        $("#txtSoPhieu").val(`PVTKK_${dot}`)
    } catch (err) {
        console.error(err)
    }
}
async function GetDSItemCode() {
    const url = `/api/DangKyVatTu/Get?action=GetDSItemCode&para1=${isNPL}`

    try {
        const response = await fetch(url)
        const data = await response.json()

        // update data
        lstDanhSachItemCode = data
        updateGrid(dxDanhSachItemCode, data)
    } catch (err) {
        console.error(err)
    }
}
function renderList(data) {
    const itemList = $('#itemList');
    itemList.empty();

    // Kiểm tra data có tồn tại và là array không
    if (!data || !Array.isArray(data)) {
        return;
    }

    const html = data.map(item => {
        const isChecked = selectedItems.some(selected => selected.MaCLVT == item.MaCLVT);
        return `
            <div class="dropdown-item">
                <input type="checkbox" 
                       id="${item.MaCLVT}"
                       data-display="${item.ChungLoaiVatTu}"
                       ${isChecked ? 'checked' : ''}>
                <label for="${item.MaCLVT}">${item.ChungLoaiVatTu}</label>
            </div>
        `;
    }).join('');

    itemList.append(html);
    updateCheckAll();
}

/// POST
async function SaveDanhSachVatTuKiemKe(arrSave) {
    const url = `/api/DangKyVatTu/PostKiemKe?action=PostKiemKe&para1=${isNPL}`
    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(arrSave)
        })
        const data = await response.json();
        if (data == "True") {
            showToast("success", "Lưu thành công !")
        }

        // Load lại danh sách
        /* await GetDanhSachVatTuKiemKe(lastSelectedMaCLVT);*/
        selectedItems = []
        updateGrid(dxDanhSachVatTuKiemKe, [])
        $("#chungLoaiInput").val("")
        await GetPhieuDanhSachVatTuKiemKe();
        await GetDot();
    } catch (err) {
        console.error(error)
    }
}

function initializeDatePickers() {
    $(".select_2").select2();
    $(".select_2").select2({
        minimumResultsForSearch: Infinity
    });

    const picker1 = new tempusDominus.TempusDominus(document.getElementById("datetimepicker"), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            },
        },
        localization: { format: 'dd/MM/yyyy' },
        defaultDate: getFirstDayOfCurrentMonth()
    });

    const picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' },
        defaultDate: getLastDayOfCurrentMonth()
    });

    const picker3 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker3'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' },
        defaultDate: getFirstDayOfCurrentMonth()
    });

    const picker4 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker4'), {
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            }
        },
        localization: { format: 'dd/MM/yyyy' },
        defaultDate: getLastDayOfCurrentMonth()
    });

    // Trigger để set giá trị
    $("#tuNgay").trigger("click");
    $("#denNgay").trigger("click");
    $("#tuNgayTab2").trigger("click");
    $("#denNgayTab2").trigger("click");

    picker1.hide();
    picker2.hide();
    picker3.hide();
    picker4.hide();
}

/// EVENT
$(document).ready(function () {
    initializeDatePickers();
    GetKH();
    GetDot();
    GetPhieuDanhSachVatTuKiemKe();
    createViewDxGridDanhSachVatTuKiemKe();
    createViewDxGridPhieudDanhSachVatTuKiemKe();
    createViewDxGridDanhSachItemCode();
})

/// DxDataGrid
function createViewDxGridDanhSachVatTuKiemKe() {
    dxDanhSachVatTuKiemKe = $("#dxDanhSachVatTuKiemKe").dxDataGrid({
        dataSource: [],
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [

            {
                caption: "",
                width: 100,
                alignment: "center",
                dataField: "IsLog",
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    // Khởi tạo IsLog nếu chưa có - MẶC ĐỊNH LÀ 1 (checked)
                    if (options.data.IsLog === undefined) {
                        options.data.IsLog = 1; // Thay đổi từ 0 thành 1
                    }
                    const $checkbox = $(`
                        <input type="checkbox" 
                               class="row-checkbox" 
                               data-row-index="${options.rowIndex}"
                               ${options.data.IsLog === 1 ? 'checked' : ''}
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);
                    $checkbox.on("change", function () {
                        // Cập nhật IsLog
                        if (this.checked) {
                            selectedRow.push(options.data);
                            options.data.isCheck = 1;
                        } else {
                            const index = selectedRow.findIndex(item => item === options.data);
                            if (index > -1) {
                                selectedRow.splice(index, 1);
                            }
                            options.data.isCheck = 0;
                        }
                        options.data.IsLog = this.checked ? 1 : 0;
                        // CẬP NHẬT SUMMARY - THÊM DÒNG NÀY
                        updateSummary(dxDanhSachVatTuKiemKe);
                    });
                    container.append($checkbox);
                },
                headerCellTemplate: function (container) {
                    const $wrapper = $('<div style="display: flex; align-items: center; justify-content: center; gap: 5px;"></div>');

                    const $label = $('<span>Chọn</span>');
                    const $checkAll = $(`
                        <input type="checkbox" 
                                id="checkAllRows" 
                                checked
                                style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkAll.on("change", function () {
                        const isChecked = this.checked;
                        const dataSource = dxDanhSachVatTuKiemKe.option("dataSource");
                        dataSource.forEach(item => {
                            item.IsLog = isChecked ? 1 : 0;
                        });
                        $('.row-checkbox').prop('checked', isChecked);
                        // Xử lý selectedRow
                        if (isChecked) {
                            selectedRow = [...dataSource];
                        } else {
                            // Uncheck all: xóa hết
                            selectedRow = [];
                        }
                    });

                    $wrapper.append($label);
                    $wrapper.append($checkAll);
                    container.append($wrapper);
                }
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "ChiTiet",
                caption: "Mô Tả",
                alignment: "center",
                minWidth: 350,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                alignment: "center",
                minWidth: 60,
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn Vị",
                alignment: "center",
                minWidth: 60,
            },
            {
                dataField: "TonDKy",
                caption: "Tồn Đầu Kỳ",
                alignment: "center",
                minWidth: 90,
                visible: false
            },
            {
                dataField: "SLNK",
                caption: "Tồn kho",
                alignment: "center",
                type: "number",
                width: 130,
                cellTemplate: function (container, options) {
                    const rowData = options.data;
                    const slnk = rowData.SLNK || 0;


                    // Tô màu nếu khác nhau
                    if (slnk === 0) {
                        container.css('background-color', '#fff3cd'); // Màu vàng nhạt
                    }

                    let str = slnk.toString();
                    let intPart = str;
                    let decPart = "";

                    if (str.includes(".")) {
                        [intPart, decPart] = str.split(".");
                        decPart = decPart.substring(0, 2); // cắt, không làm tròn
                    }

                    // format phần nguyên
                    let formattedInt = Number(intPart).toLocaleString();

                    let result = decPart ? `${formattedInt}.${decPart}` : formattedInt;
                    container.text(result);
                }
            },

            {
                dataField: "TonCKy",
                caption: "Tồn Cuối Kỳ",
                alignment: "center",
                minWidth: 90,
                visible: false
            },
            {
                dataField: "GhiChu",
                caption: "Ghi chú",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control"
                               value="${options.data.GhiChu || ''}"
                               style="font-size:13px"
                            />
                    `);

                    $input.on("input", function () {
                        options.data.GhiChu = this.value;
                    });

                    container.append($input);
                }
            }

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
            }
        },
        summary: {
            totalItems: [
                {
                    name: "tongKho",
                    summaryType: "custom",
                    showInColumn: "ChiTiet"
                },
                {
                    column: "TonDKy",
                    summaryType: "sum",
                    showInColumn: "TonDKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "TonCKy",
                    summaryType: "sum",
                    showInColumn: "TonCKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "SLNK",
                    summaryType: "sum",
                    showInColumn: "SLNK",
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
            ],
            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {

                    if (options.summaryProcess === "start") {
                        options.totalValue = {
                            totalSoKien: 0,
                        };
                    }

                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalSoKien += 1;
                    }

                    if (options.summaryProcess === "finalize") {
                        const { totalSoKien } = options.totalValue;
                        options.totalValue =
                            `Số Vật Tư: ${totalSoKien.toLocaleString()}  `;
                    }
                }
            }
        }

    }).dxDataGrid("instance");

    $("#Layer_1").click()
}
function updateSummary(grid) {
    const dataSource = grid.option("dataSource");

    // Tính tổng SLNK
    const totalSLNK = dataSource.reduce(
        (sum, item) => sum + (item.isCheck === 1 ? (Number(item.SLNK) || 0) : 0), // Sửa SLDK -> SLNK
        0
    );

    // Đếm số vật tư được chọn
    const totalSLVT = dataSource.reduce(
        (sum, item) => sum + (item.isCheck === 1 ? 1 : 0),
        0
    );

    // CẮT 4 số thập phân – KHÔNG LÀM TRÒN
    const truncatedSLNK = Math.trunc(totalSLNK * 10000) / 10000;

    // Tìm column index
    const columns = grid.option("columns");
    const visibleColumns = columns.filter(col => col.visible !== false);
    const sldkColumnIndex = visibleColumns.findIndex(col => col.dataField === "SLNK");
    const totalslvtColumnIndex = visibleColumns.findIndex(col => col.dataField === "ChiTiet");

    const $summaryRow = grid.element().find('.dx-datagrid-total-footer .dx-row');

    // Cập nhật summary cho cột SLNK
    if (sldkColumnIndex !== -1) {
        const $summaryCell = $summaryRow.find('td').eq(sldkColumnIndex);
        const $summaryItem = $summaryCell.find('.dx-datagrid-summary-item');
        if ($summaryItem.length) {
            $summaryItem.text(parseFloat(truncatedSLNK).toLocaleString());
        }
    }

    // Cập nhật summary cho cột ChiTiet (Số Vật Tư)
    if (totalslvtColumnIndex !== -1) {
        const $summaryCell = $summaryRow.find('td').eq(totalslvtColumnIndex);
        const $summaryItem = $summaryCell.find('.dx-datagrid-summary-item');
        if ($summaryItem.length) {
            $summaryItem.text(`Vật Tư Đăng Ký Là: ${totalSLVT.toLocaleString()}`);
        }
    }
}
function createViewDxGridPhieudDanhSachVatTuKiemKe() {
    dxPhieuDanhSachVatTuKiemKe = $("#dxPhieuDanhSachVatTuKiemKe").dxDataGrid({
        dataSource: [],
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "IsLog",
                caption: "Trạng Thái",
                alignment: "center",
                cellTemplate: function (container, options) {
                    const $icon =
                        $(`<span style="font-size:14px; font-weight: 500; color: ${options.data.IsLog === 0 ? "green" : "#ffa200"}" class="">
                        <i class="${options.data.IsLog === 0 ? "fa-solid fa-circle-check" : "fa-solid fa-spinner"}"></i>
                        ${options.data.IsLog === 0
                                ? "Hoàn Thành Kiểm Kê"
                                : "Đang Kiểm Kê"}
                        </span>`);
                    container.append($icon);
                },
                minWidth: 170,
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 120,
            },
            {
                dataField: "ChiTiet",
                caption: "Mô Tả",
                alignment: "center",
                minWidth: 360,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                alignment: "center",
                minWidth: 150,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "MaDVVT",
                caption: "Đơn Vị",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "TonDKy",
                caption: "Tồn Đầu Kỳ",
                alignment: "center",
                minWidth: 90,
                visible: false,
            },
            {
                dataField: "SLNhapKho",
                caption: "Tồn Kho",
                alignment: "center",
                minWidth: 90,
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let str = value.toString();
                    let intPart = str;
                    let decPart = "";

                    if (str.includes(".")) {
                        [intPart, decPart] = str.split(".");
                        decPart = decPart.substring(0, 2); // cắt, không làm tròn
                    }

                    // format phần nguyên
                    let formattedInt = Number(intPart).toLocaleString();

                    let result = decPart ? `${formattedInt}.${decPart}` : formattedInt;
                    container.text(result);
                }

            },
            {
                dataField: "TonCKy",
                caption: "Tồn Cuối Kỳ",
                alignment: "center",
                minWidth: 90,
                visible: false
            },
            {
                dataField: "GhiChu",
                caption: "Ghi Chú",
                alignment: "center",
                minWidth:120,
            },
            {
                caption: "",
                width: 100,
                alignment: "center",
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    const $checkbox = $(`
                        <input type="checkbox" 
                               class="row-checkbox" 
                               data-row-index="${options.rowIndex}"
                               ${options.data.IsKiemKe === 1 || options.data.IsLog === 0 ? 'disabled' : ''}
                               style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkbox.on("change", function () {
                        if (options.data.IsKiemKe === 1) {
                            showToast("warning", `itemCode: ${options.data.MaVT} Đã hủy kiểm kê`);
                            return;
                        }
                        if (options.data.IsLog === 0) {
                            showToast("warning", `itemCode: ${options.data.MaVT} Đã hoàn thành kiểm kê`);
                            return;
                        }
                        if (this.checked) {
                            selectedRowDelete.push(options.data);
                        } else {
                            const index = selectedRowDelete.findIndex(item => item === options.data);
                            if (index > -1) {
                                selectedRowDelete.splice(index, 1);
                            }
                        }
                    });
                    container.append($checkbox);
                },
                headerCellTemplate: function (container) {
                    const $wrapper = $('<div style="display: flex; align-items: center; justify-content: center; gap: 5px;"></div>');

                    const $label = $('<span>Chọn</span>');
                    const $checkAll = $(`
                        <input class='d-none' type="checkbox" 
                                id="checkAllRows" 
                                style="width: 18px; height: 18px; cursor: pointer;" />
                    `);

                    $checkAll.on("change", function () {
                        const isChecked = this.checked;
                        const dataSource = dxPhieuDanhSachVatTuKiemKe.option("dataSource");
                        $('.row-checkbox').prop('checked', isChecked);
                        // Xử lý selectedRow
                        if (isChecked) {
                            selectedRowDelete = [...dataSource];
                        } else {
                            // Uncheck all: xóa hết
                            selectedRowDelete = [];
                        }
                    });

                    $wrapper.append($label);
                    $wrapper.append($checkAll);
                    container.append($wrapper);
                }
            },
            {
                dataField: "NgayHuy1",
                caption: "Ngày Hủy",
                alignment: "center",
                minWidth: 150,
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
            }
        },
        summary: {
            totalItems: [
                {
                    column: "TonDKy",
                    summaryType: "sum",
                    showInColumn: "TonDKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "TonCKy",
                    summaryType: "sum",
                    showInColumn: "TonCKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "SLNhapKho",
                    summaryType: "sum",
                    showInColumn: "SLNhapKho",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toLocaleString();
                    }
                },
            ],
        }

    }).dxDataGrid("instance");

    $("#Layer_1").click()
}

function createViewDxGridDanhSachItemCode() {
    dxDanhSachItemCode = $("#dxDanhSachItemCode").dxDataGrid({
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
                dataField: "PhieuVatTuKK",
                caption: "Số Phiếu",
                alignment: "center",
                minWidth: 40,
                cellTemplate: function (container, options) {
                    const $wrapper = $(`<div></div>`).css({ display: "flex", justifyContent: "center", height: "25px", gap: "8px" })

                    // Text
                    const $span = $(`<span>${options.data.PhieuVatTuKK}</span>`)

                    // Icon
                    const $btnNext = $(`<i class="fa-solid fa-circle-right"></i>`)
                        .css({ fontSize: '16px', color: 'green' })
                        .on("click", function () {
                            $("#phieuvattukiemke").val(`${options.data.PhieuVatTuKK}`).trigger("change");
                            $("#modalTimNhanh").modal('hide');
                            $("#inputTimKiem").val('').trigger('input');
                            updateGrid(dxDanhSachItemCode, []);
                        })

                    $wrapper.append($span, $btnNext).appendTo(container)

                }
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                alignment: "center",
                minWidth: 40,
            },
            {
                dataField: "MaDVVT",
                caption: "Đơn Vị",
                alignment: "center",
                minWidth: 40,
            },
            {
                dataField: "DisplayDate",
                caption: "Khoảng Ngày Kiểm Kê",
                alignment: "center",
                minWidth: 150
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
        summary: {
            totalItems: [
                {
                    column: "TonDKy",
                    summaryType: "sum",
                    showInColumn: "TonDKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "TonCKy",
                    summaryType: "sum",
                    showInColumn: "TonCKy",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
                {
                    column: "SLNhapKho",
                    summaryType: "sum",
                    showInColumn: "SLNhapKho",
                    customizeText(e) {
                        if (e.value == null) return "";

                        const value = Number(e.value);
                        const truncated = Math.trunc(value * 10000) / 10000

                        return truncated.toString();
                    }
                },
            ],
        }

    }).dxDataGrid("instance");

    $("#Layer_1").click()
}

function showConfirmModal(onConfirm) {

    $('#confirmBtn').off('click');
    // Khi nhấn Đồng ý
    $('#confirmBtn').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        $("#confirmModal").modal("hide")
    });

    // Hiện modal
    $("#confirmModal").modal("show")
}


function showConfirmModalDelete(onConfirm) {
    $("#txtSLVTDelete").text(` (${selectedRowDelete.length})`);
    $('#btnConfirmtDeletePhieu').off('click');
    // Khi nhấn Đồng ý
    $('#btnConfirmtDeletePhieu').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        $("#modalComfimrtDeleteVT").modal("hide")
    });
    // Hiện modal
    $("#modalComfimrtDeleteVT").modal("show")
}
function sendNotify(ModuleID, title, detail, sendTo, BoPhan = "ALL", Status = -1) {
    const url = `/api/SendToNotification/PushNotification?` +
        `UserIDTao=${encodeURIComponent(userNameSave)}&` +
        `ModuleID=${encodeURIComponent(ModuleID)}&` +
        `Title=${encodeURIComponent(title)}&` +
        `Detail=${encodeURIComponent(detail)}&` +
        `SendTo=${encodeURIComponent(sendTo)}&` +
        `BoPhan=${encodeURIComponent(BoPhan)}&` +
        `Status=${encodeURIComponent(Status)}&`
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