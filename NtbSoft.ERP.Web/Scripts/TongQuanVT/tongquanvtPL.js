var userNameSave = localStorage.getItem("username1")
let timeoutId2;
var barcodeLocal;
var selectSoLo;
var checkChange;
let currentSelectedPhieu = null;
let manplCurrent = ""
var allData = []
$(function () {
    $(".select_2").select2();
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    $(".filter").on("click", async function () {
        LoadChiTietVT();
    });
    $("#soloid").on("change", function () {

    })
    $("#cayvai").on("change", function () {
        isFirstLoadKiemKe = true;
        loadPINCC();
    })
    $("#khachhang").on("change", function () {
        loadMH();
    })
    $("#mahang").on("change", function () {
        GetVatTu()
    })
    const picker1 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker'), {
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
    });

    const picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
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
    });

    $(".dateInput").trigger("click")
    picker1.hide()
    picker2.hide()
    $("#tuNgay").on("change", function () {
    })
    $("#denNgay").on("change", function () {
    })
    loadKH();

    $(".enterbarcode").on("click", function () {
        barcodeLocal = $('#QRScan').val().trim()
        barcodeInput();
    })
    $(document).on('keydown', async function (e) {
        if (e.key === 'Enter') {
            if ($('#QRScan:focus').length > 0) {
                barcodeLocal = e.target.value.trim()
                barcodeInput();
            }
            else {
                showToast("warning", "Vui lòng chọn vào ô scan !")
                PlayAudioError()
            }
        }
    });

    $("#btnCancelSwitchVT").on("click", function () {
        // Khôi phục lại vật tư cũ
        if (window.pendingVTSwitch && window.pendingVTSwitch.oldVT) {
            $("#cayvai").val(window.pendingVTSwitch.oldVT);

            // Destroy và reinit select2
            if ($("#cayvai").data('select2')) {
                $("#cayvai").select2('destroy');
            }
            $("#cayvai").select2();
        }

        $("#myModalSwitchVT").modal("hide");
        window.pendingVTSwitch = null;
    });

    $("#btnConfirmSwitchVT").on("click", async function () {
        $("#myModalSwitchVT").modal("hide");
        renderSwitch(window.pendingVTSwitch.newVT)
    });
    // Reset form khi đóng modal
    $('#inventoryModal').on('hidden.bs.modal', function () {
        $('#note').val('');
        $('#inventoryQty').removeClass('is-invalid');
        $('#qtyError').hide();
    });
    $('#inventoryModal').on('shown.bs.modal', function () {
        $('#inventoryQty').focus();
    });
    var lastValidQty = 0;

    $('#inventoryQty').on('input', function () {
        const originalQty = parseFloat($('#quantity').text()) || 0;
        let inventoryQty = parseFloat($(this).val()) || 0;
        lastValidQty = originalQty
        if (inventoryQty > originalQty) {
            // Trả về giá trị hợp lệ trước đó
            $(this).val(lastValidQty);
            $(this).addClass('is-invalid');
            $('#qtyError').show();
        } else {
            // Lưu lại giá trị hợp lệ
            lastValidQty = inventoryQty;
            $(this).removeClass('is-invalid');
            $('#qtyError').hide();
        }
    });
})
async function barcodeInput() {
    if (barcodeLocal.trim() != "") {
        await GetVTBarCode(barcodeLocal)
        $("#QRScan").val("")
    }
    else {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan. !")
        PlayAudioError()
    }
}
async function GetChiTietBarCode(para1, para2) {
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetChiTietVTPL&para1=${para1}&para2=${para2}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        let html = ``
        data.map(item => {
            var trangthai = item.ModuleXH == 1 ? "Xuất hàng" : "Thu hồi"
            var className = item.ModuleXH == 1 ? "green" : "#8d54f7"
            html += `
                <tr data-status="${item.Status}" data-barcode="${item.BarCode}">
                        <td class="ngay">${item.Ngay}</td>
                        <td class="">${item.MaLenh}</td>
                        <td class="">${item.TenHang}</td>
                        <td class="">${item.TenKH}</td>
                        <td class="d-none">${item.PhieuYC}</td>
                        <td class="status" style="color:${className};min-width:80px">${trangthai}</td>
                         <td class="" style="color:${className}">${item.SLNhap || ""}</td>
                </tr>

            `
        })
        $("#tblDetail").html(html)
    } catch (error) {
        console.error(error.message);
    }
}

async function GetVatTu() {
    const soloid = $("#filterType").val() != "khachhang" ? "all" : $("#soloid").val();
    const makh = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    const mahang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();
    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetVTMK&para1=${isNPL}&para2=${makh}&para3=${mahang}&para4=${tuNgayFormat}&para5=${denNgayFormat}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value=""></option><option value="all">Tất cả</option>`;

        if (data.length == 0) {
            $("#cayvai").html(html);
            /*$("#soloid").html(`<option value="all">Tất cả</option>`);*/
            $("#tbody").empty();
            $("#tfoot").empty();
            return;
        }

        data.map(x => {
            html += `<option value="${x.MaNPL}">${x.Display}</option>`;
        })

        $("#cayvai").html(html);

        $("#cayvai").select2();
    } catch (error) {
        console.error(error.message);
    }
}
//end
function ChangeFilter() {
    const val = $("#filterType").val();

    if (val === "khachhang") {
        $(".filterKH").show();
        $(".filterDate").hide();

    }
    else {
        $(".filterDate").show();
        $(".filterKH").hide();
    }
    GetVatTu()
}
async function loadKH() {
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GETKHTQVT&para1=1`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        const $khachHangSelect = $("#khachhang");
        if ($khachHangSelect.data('select2')) { $khachHangSelect.select2('destroy'); }
        $khachHangSelect.empty();

        $khachHangSelect.append(`<option value="all">Tất cả</option>`);

        if (data.length > 0) {
            let html = '';
            data.map(x => {
                html += `<option value="${x.MaKH}">${x.TenKH}</option>`;
            });
            $khachHangSelect.append(html);
        }
        $khachHangSelect.select2();
        loadMH();
    } catch (error) {
        console.error("Lỗi khi tải danh sách khách hàng:", error.message);
    }
}
async function loadMH() {
    const maKH = $("#khachhang").val();
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GETMHTQVT&para1=1&para2=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;
        data.map(x => {
            html += `
                <option value="${x.MaHang}">${x.MaHang == "" ? "Mã hàng rỗng" : x.MaHang}</option>
            `
        })
        $("#mahang").html(html)
        GetVatTu()
    } catch (error) {
        console.error("Lỗi khi tải danh sách mã hàng:", error.message);
    }
}
async function loadPINCC(value) {
    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    const maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GETPINCCNL&para1=${maKH}&para2=${maHang}&para3=${isNPL}&para4=${tuNgayFormat}&para5=${denNgayFormat}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status}`);
        const data = await response.json();

        const $soLoSelect = $("#soloid");
        if ($soLoSelect.data('select2')) $soLoSelect.select2('destroy');

        $soLoSelect.empty();
        $soLoSelect.append(`<option value="all">Tất cả</option>`);

        if (data.length > 0) {
            let html = data.map(x => `<option value="${x.SoLoID}">${x.SoLo}</option>`).join('');
            $soLoSelect.append(html);
        }
        $soLoSelect.select2();

        /* await LoadChiTietVT();
         await LoadChiTietVTKiemKe(value)*/

        await LoadChiTietVTDx();
        await LoadChiTietVTKiemKeDx(value);
    } catch (error) {
        console.error("Lỗi khi tải danh sách PINCC:", error.message);
    }
}
async function LoadChiTietVTKiemKe(value) {
    var maVT = manplCurrent;
    var soloid = $("#soloid").val();
    var maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    var maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();

    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";


    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetPhieuKiemKe&para1=${maVT}&para2=${soloid}&para3=${maHang}&para4=${maKH}&para5=${tuNgayFormat}&para6=${denNgayFormat}&para7=0`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        if (data.length == 0) {
            $("#tblInventoryLeft").html(`
              <tr>
                    <td colspan="10" class="text-center py-4">
                      <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                       <p class="text-muted">Chưa có dữ liệu</p>
                        </td>
                        </tr>
         `)
            updateFooterStats('left');
            return;
        }
        let html = ``;
        data.map(item => {

            html += `
                <tr data-barcode="${item.BarCode}">
                         <td class="solo">${item.SoLo}</td>
                         <td class="pomua">${item.PoMua}</td>
                         <td class="itemcode">${item.MaVT}</td>
                         <td style="min-width:300px;max-width: 300px;" class="chitiet">${item.ChiTiet}</td>
                         <td style="min-width:150px;max-width: 150px;" class="mau" >${item.MauVT}</td>
                         <td class="solot">${item.SoLoT}</td>
                         <td class="khosize">${item.KhoVai}</td>
                         <td class="donvi">${item.TenDVVT ?? ""}</td>
                         <td class="sokien">${item.SoKien}</td>
                         <td class="barcode">${item.SLNK}</td>
                        </td>
                </tr>

            `
        })

        $("#tblInventoryLeft").html(html)
        updateFooterStats('left');

        const activeTab = $('#mainTabs .nav-link.active').attr('id');

        if (activeTab === 'inventory-tab' && value == 1)
            handleInventoryScan(barcodeLocal)


    } catch (error) {
        console.error(error.message);
    }
}
async function handleInventoryScan(barcode) {
    if (!barcode || barcode.trim() === "") {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan!");
        PlayAudioError();
        return;
    }

    const $leftTable = $("#tblInventoryLeft");
    const $rightTable = $("#tblInventoryRight");

    // Tìm row có barcode trùng khớp ở bảng trái
    const $targetRow = $leftTable.find("tr").filter(function () {
        return $(this).data("barcode") === barcode;
    });

    if ($targetRow.length > 0) {
        // Nếu tìm thấy: xóa row khỏi bảng trái và thêm vào bảng phải
        const rowHtml = $targetRow[0].outerHTML;

        // Xóa row khỏi bảng trái
        $targetRow.remove();

        // Thêm row vào đầu bảng phải
        $rightTable.prepend(rowHtml);

        // Highlight row vừa thêm
        $rightTable.find("tr").removeClass("highlight");
        $rightTable.find("tr:first").addClass("highlight");

        // Cập nhật footer cho cả 2 bảng
        updateFooterStats('left');
        updateFooterStats('right');

        // Phát âm thanh thành công
        PlayAudio();
        //showToast("success", `Đã chuyển barcode ${barcode} sang bảng kiểm kê`);

        // Xóa input
        $("#QRScan").val("");
    } else {
        // Kiểm tra xem barcode đã có ở bảng phải chưa
        const $existInRight = $rightTable.find("tr").filter(function () {
            return $(this).data("barcode") === barcode;
        });

        if ($existInRight.length > 0) {
            showToast("warning", `Barcode ${barcode} đã được kiểm kê rồi!`);
            PlayAudioError();

            // Highlight row đã tồn tại
            $rightTable.find("tr").removeClass("highlight");
            $existInRight.addClass("highlight");
        } else {
            showToast("error", `Không tìm thấy barcode ${barcode} trong danh sách!`);
            PlayAudioError();
        }

        $("#QRScan").val("");
    }
}
// Hàm cập nhật thống kê footer
function updateFooterStats(side) {
    const tableId = side === 'left' ? '#tblInventoryLeft' : '#tblInventoryRight';
    const footerId = side === 'left' ? '#tfootLeft' : '#tfootRight';

    const $table = $(tableId);
    const $rows = $table.find('tr').filter(function () {
        return $(this).data('barcode') !== undefined;
    });

    const rowCount = $rows.length;
    let totalQuantity = 0;

    $rows.each(function () {
        const quantity = parseFloat($(this).find('.barcode').text()) || 0;
        totalQuantity += quantity;
    });

    const footerHtml = `
        <tr style="background-color: #f8f9fa; font-weight: bold;">
            <td colspan="2" class="text-right"></td>
            <td class="text-center">${rowCount} Vật tư</td>
            <td class="text-center">Số lượng : ${totalQuantity.toFixed(2)}</td>
  <td colspan="6" class="text-right"></td>
        </tr>
    `;

    $(footerId).html(footerHtml);
}


/// Kiet
let dxDataPLTongQuan;
let dxKiemKePhuLieuLeft;
let dxKiemKePhuLieuRight;
let dxGridLichSuQuet;
let dxDataGhiNhanKiemKe;
let isScannedFromModal = false;
let isFirstLoadKiemKe = true;
let isPostSwitchScan = false;
let lstLichSuQuet = []; // Data Lich su quet
let lstDataTongQuanKiemKe = [];
let rowEditSLKK = {}


// ======== LOAD THONG TIN VAT TU ========
async function LoadChiTietVTDx() {
    var maVT = manplCurrent;
    var soloid = $("#soloid").val();
    var maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    var maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();


    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetTQVTPL&para1=${maVT}&para2=${soloid}&para3=${maHang}&para4=${maKH}&para5=${tuNgayFormat}&para6=${denNgayFormat}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();

        if (!dxDataPLTongQuan) return;

        dxDataPLTongQuan.beginUpdate();
        dxDataPLTongQuan.clearFilter();
        dxDataPLTongQuan.option({
            dataSource: data,
        });
        dxDataPLTongQuan.endUpdate();
    } catch (error) {
        console.error(error.message);
    }
}
async function LoadChiTietVTKiemKeDx(value) {
    var maVT = manplCurrent;
    var soloid = $("#soloid").val();
    var maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    var maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();

    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetPhieuKiemKe&para1=${maVT}&para2=${soloid}&para3=${maHang}&para4=${maKH}&para5=${tuNgayFormat}&para6=${denNgayFormat}&para7=0`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();

        if (!dxKiemKePhuLieuLeft) return;

        dxKiemKePhuLieuLeft.beginUpdate();
        dxKiemKePhuLieuLeft.clearFilter();
        dxKiemKePhuLieuLeft.option({
            dataSource: data,
            repaintChangesOnly: true
        });
        dxKiemKePhuLieuLeft.endUpdate();

        if (maVT && maVT !== "" && maVT !== "all") {
            await reloadKiemKeRightByMaVT(maVT);
        } else if (!value) {
            dxKiemKePhuLieuRight.beginUpdate();
            dxKiemKePhuLieuRight.option('dataSource', []);
            dxKiemKePhuLieuRight.endUpdate();
        }

        // BƯỚC 3: Xử lý quét barcode nếu cần
        const activeTab = $('#mainTabs .nav-link.active').attr('id');
        if (activeTab === 'inventory-tab' && value == 1) {
            handleInventoryScanDx(barcodeLocal);
        }

    } catch (error) {
        console.error(error.message);
    }
}

// ========== BARCODE HANDLING LOGIC ==========
async function GetVTBarCode(barcode) {
    const url = `/api/PhieuXuatHangNPL/GetDS?action=GetChiTietBarCodeKK&para1=${encodeURIComponent(barcode)}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status}`);

        const data = await response.json();
        if (data.Table.length == 0) {
            PlayAudioError();
            showToast("warning", "Barcode chưa được khai báo");
            return;
        }
        if (data.Table1.length == 0) {
            PlayAudioError();
            showToast("warning", `ItemCode: ${data.Table[0].MaVT} - Màu: ${data.Table[0].MauVT} chưa được tạo phiếu kiểm kê vật tư`);
            return;
        }

        const selectNPL = data.Table1[0].MaNPL;
        const currentVT = manplCurrent;
        const currentPhieu = data.Table1[0].PhieuVatTuKK;;
        const activeTab = $('#mainTabs .nav-link.active').attr('id');

        const phieuSelect = $("#phieuvattukiem").val()
        // Khác mã phiếu 
        if (phieuSelect != currentPhieu) {
            manplCurrent = selectNPL;
            $("#phieuvattukiem").val(currentPhieu).trigger("change")

        }
        //// TRƯỜNG HỢP 1: VẬT TƯ ĐÚNG
        else if (currentVT == selectNPL) {
            await handleCorrectVT(activeTab, barcode);
        }
        // TRƯỜNG HỢP 2: CHƯA CHỌN VẬT TƯ
        else if (currentVT == "" || currentVT == "all") {
            await handleNoVTSelected(activeTab, selectNPL, barcode);
        }
        // TRƯỜNG HỢP 3: VẬT TƯ KHÁC
        else {
            await handleDifferentVT(activeTab, selectNPL, barcode);
        }
    } catch (error) {
        console.error(error.message);
    }
}
function getItemInfoFromTongQuan(barcode) {
    if (!dxDataPLTongQuan || !barcode) {
        console.warn("Không thể lấy thông tin từ Tổng Quan");
        return null;
    }

    const allData = dxDataPLTongQuan.getDataSource().items();
    const item = allData.find(i => i.BarCode === barcode);

    if (item) {
        return item;
    } else {
        console.warn(`Không tìm thấy barcode ${barcode} trong Tổng Quan`);
        return null;
    }
}
function updateThongTinSauKhiScan(barcode) {
    if (!barcode) return;

    // ƯU TIÊN LẤY TỪ TỔNG QUAN (có đầy đủ Status)
    let targetItem = getItemInfoFromTongQuan(barcode);

    // Nếu không tìm thấy trong Tổng Quan, thử tìm trong các bảng khác
    if (!targetItem && dxKiemKePhuLieuLeft) {
        const leftData = dxKiemKePhuLieuLeft.getDataSource().items();
        targetItem = leftData.find(item => item.BarCode === barcode);
    }

    if (!targetItem && dxKiemKePhuLieuRight) {
        const rightData = dxKiemKePhuLieuRight.getDataSource().items();
        targetItem = rightData.find(item => item.BarCode === barcode);
    }

    if (targetItem) {
        updateThongTinQuet(targetItem);
    } else {
        console.warn(`Không tìm thấy thông tin cho barcode ${barcode}`);
    }
}
async function handleCorrectVT(activeTab, barcode) {
    if (activeTab === 'inventory-tab') {
        const leftData = dxKiemKePhuLieuLeft ? dxKiemKePhuLieuLeft.getDataSource().items() : [];

        if (leftData.length === 0) {
            await LoadChiTietVTDx();
            showToast("warning", "Đã kiểm kê hết vật tư.");
            return;


        } else {
            await handleInventoryScanDx(barcode);
        }
    } else if (activeTab === 'overview-tab') {
        const found = CheckBarcodeTableDx(barcode);
        if (found) {
            PlayAudio();
        } else {
            PlayAudioError();
            showToast("warning", `Không tìm thấy barcode ${barcode} trong danh sách`);
        }
    }
}
async function handleNoVTSelected(activeTab, selectNPL, barcode) {
    renderSwitch(selectNPL)
}
async function handleDifferentVT(activeTab, selectNPL, barcode) {
    const selectedItemOld = allData.find(item => item.MaNPL.toUpperCase() === manplCurrent);
    const selectedItemNew = allData.find(item => item.MaNPL.toUpperCase() === selectNPL);
    $("#currentVT").text(selectedItemOld.Display);
    $("#newVT").text(selectedItemNew.Display);
    window.pendingVTSwitch = {
        oldVT: manplCurrent,
        newVT: selectNPL,
        barcode: barcode
    };
    $("#myModalSwitchVT").modal("show");
}
$("#soloid").on("select2:select", function () {
    const value = $(this).val();

    if (value === "all") {
        // Xóa filter nếu chọn "Tất cả"
        dxDataPLTongQuan.clearFilter();
        dxKiemKePhuLieuLeft.clearFilter();
    } else {
        // Apply filter cho SoLoID cụ thể
        dxDataPLTongQuan.filter(["SoLoID", "=", value]);
        dxKiemKePhuLieuLeft.filter(["SoLoID", "=", value]);
    }

    dxDataPLTongQuan.refresh();
});
// ====== CREATE VIEW DX ======
function createViewDxDataTongQuan() {
    dxDataPLTongQuan = $("#dxDataPLTongQuan").dxDataGrid({
        dataSource: [],
        columnAutoWidth: false,
        allowColumnResizing: true,
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
        width: "100%",
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
            { dataField: "SoLo", caption: "PI NCC", alignment: "center", width: 90 },
            { dataField: "PoMua", caption: "PO Mua", alignment: "center", width: 90 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", width: 110 },

            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                alignment: "center",
                width: 300
            },

            { dataField: "MauVT", caption: "Màu", alignment: "center", width: 80 },
            { dataField: "SoLoT", caption: "LOT/BATCH", alignment: "center", width: 120 },
            { dataField: "KhoVai", caption: "Width/Size", alignment: "center", width: 90 },
            { dataField: "SoKien", caption: "Vật tư", alignment: "center", width: 130 },

            {
                dataField: "BarCode",
                caption: "Barcode",
                alignment: "center",
                width: 160,
                visible: false,
                showInColumnChooser: false,
                allowFiltering: true,
                allowSorting: true
            },

            {
                dataField: "Status",
                caption: "Trạng thái",
                alignment: "center",
                width: 120,
                cellTemplate: function (container, options) {
                    const [text, color] =
                        options.value === 0 ? ["Nhập kho", "green"] :
                            options.value === 1 ? ["Xuất hàng", "red"] :
                                options.value === 2 ? ["Thu hồi", "orange"] :
                                    ["Chưa nhập kho", "gray"];

                    container.text(text).css({
                        color: color,
                        fontWeight: "bold"
                    });
                }
            },

            { dataField: "NgayNK", caption: "Ngày nhập kho", alignment: "center", width: 120 },

            {
                dataField: "SLNK",
                caption: "SL N.Kho",
                alignment: "center",
                width: 90,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    const value = options.value;
                    const status = options.data.Status;

                    const color =
                        status === 0 ? "green" :
                            status === 1 ? "red" :
                                status === 2 ? "orange" : "gray";

                    // Nếu value == 0 thì để trống
                    container
                        .text(value === 0 || value == null ? "" : value)
                        .css({
                            color: color,
                            fontWeight: "bold"
                        });
                }
            },


            { dataField: "NgayXH", caption: "Ngày xuất", alignment: "center", width: 120 },

            {
                dataField: "SLXH",
                caption: "SL Xuất",
                alignment: "center",
                width: 90,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    if (!options.value) return;
                    const status = options.data.Status;
                    const color = status === 0 ? "green" : status === 1 ? "red" : status === 2 ? "orange" : "gray";
                    container.text(options.value).css({ color, fontWeight: "bold" });
                }
            },

            { dataField: "NgayTK", caption: "Ngày T.Hồi", alignment: "center", width: 120, },
            {
                dataField: "SLTK", caption: "SL T.Hồi", alignment: "center", width: 90,
                cellTemplate: function (container, options) {
                    if (!options.value) return;
                    const status = options.data.Status;
                    const color = status === 0 ? "green" : status === 1 ? "red" : status === 2 ? "orange" : "gray";
                    container.text(options.value).css({ color, fontWeight: "bold" });
                },
                customizeText: function (e) {
                    return e.value === 0 ? "" : e.value;
                }
            },
            { dataField: "DonGia", caption: "Đơn giá", alignment: "center", width: 100, visible: false },
            { dataField: "ThanhTien", caption: "Thành tiền", alignment: "center", width: 120, visible: false },

            {
                caption: "Chi Tiết",
                alignment: "center",
                width: 70,
                visible: false,
                cellTemplate: function (container, options) {
                    const $icon = $('<i class="fa-solid fa-server" style="cursor:pointer;"></i>');
                    $icon.on("click", async function () {
                        const rowData = options.data;

                        if (rowData.Status === 0 || rowData.Status === 4) {
                            iziToast.warning({
                                message: `Cây vải ${rowData.SoKien} chưa được xuất hàng`,
                                position: 'topRight',
                                timeout: 2500
                            });
                            return;
                        }

                        $("#modalVatTuBC").text(rowData.ChiTiet || '');
                        $("#modalSoLoBC").text(rowData.SoLo || '');
                        $("#modalNgayNK").text(rowData.NgayNK || '');
                        $("#modalSLNK").text(rowData.SLNK || '');

                        await GetChiTietBarCode(rowData.MaNPL || '', rowData.SoLoID || "");
                        $("#myModalDetail").modal("show");
                    });
                    container.append($icon);
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    name: "tongKho",
                    summaryType: "custom",
                    showInColumn: "ChiTiet",
                }
            ],
            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {
                    if (options.summaryProcess === "start") {
                        options.totalValue = { totalNK: 0, totalXH: 0 };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalNK += options.value.SLNK || 0;
                        options.totalValue.totalXH += options.value.SLXH || 0;
                    }
                    if (options.summaryProcess === "finalize") {
                        const { totalNK, totalXH } = options.totalValue;
                        options.totalValue = `Tồn kho: ${totalNK.toLocaleString()}  -- Xuất hàng: ${totalXH.toLocaleString()}`;
                    }
                }
            }
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
function createViewDxDataKiemKe() {
    dxKiemKePhuLieuLeft = $("#dxKiemKePhuLieuLeft").dxDataGrid({
        dataSource: [],
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
            { dataField: "SoLo", caption: "PI NCC", alignment: "center", width: 90 },
            { dataField: "POMua", caption: "PO Mua", alignment: "center", width: 90 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", width: 110 },

            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                alignment: "center",
                width: 300
            },

            { dataField: "MauVT", caption: "Màu", alignment: "center", width: 80 },
            { dataField: "SoLoT", caption: "LOT/BATCH", alignment: "center", width: 120 },
            { dataField: "KhoVai", caption: "Width/Size", alignment: "center", width: 90 },
            { dataField: "SoKien", caption: "Vật tư", alignment: "center", width: 130 },

            {
                dataField: "BarCode",
                caption: "Barcode",
                alignment: "center",
                width: 160,
                visible: false,
                showInColumnChooser: false,
                allowFiltering: true,
                allowSorting: true
            },

            {
                dataField: "SLNK",
                caption: "Số lượng",
                alignment: "center",
                width: 100,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    let value = options.value;

                    if (value === null || value === undefined) {
                        container.text("");
                        return;
                    }

                    let result = formatNumber(value)
                    container.text(result);
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    name: "tongKho",
                    summaryType: "custom",
                    showInColumn: "ChiTiet",
                }
            ],
            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {
                    if (options.summaryProcess === "start") {
                        options.totalValue = { totalRoll: 0, totalNH: 0 };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalRoll += 1
                        options.totalValue.totalNH += options.value.SLNK || 0;
                    }
                    if (options.summaryProcess === "finalize") {
                        const { totalRoll, totalNH } = options.totalValue;
                        options.totalValue = `${totalRoll.toLocaleString()} Vật tư -- Số lượng: ${totalNH.toLocaleString()}`;
                    }
                }
            }
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
    // Bảng đã kiểm kê
    dxKiemKePhuLieuRight = $("#dxKiemKePhuLieuRight").dxDataGrid({
        dataSource: [],
        columnAutoWidth: true,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        noDataText: "Chưa có dữ liệu kiểm kê",
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: { enabled: false },
        renderAsync: false,
        columns: [
            { dataField: "SoLo", caption: "PI NCC", alignment: "center", width: 90 },
            { dataField: "POMua", caption: "PO Mua", alignment: "center", width: 90 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", width: 110 },

            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                alignment: "center",
                width: 300
            },

            { dataField: "MauVT", caption: "Màu", alignment: "center", width: 80 },
            { dataField: "SoLoT", caption: "LOT/BATCH", alignment: "center", width: 120 },
            { dataField: "KhoVai", caption: "Width/Size", alignment: "center", width: 90 },
            { dataField: "SoKien", caption: "Vật tư", alignment: "center", width: 130 },

            {
                dataField: "BarCode",
                caption: "Barcode",
                alignment: "center",
                width: 160,
                visible: false,
                showInColumnChooser: false,
                allowFiltering: true,
                allowSorting: true
            },

            {
                dataField: "SLNK",
                caption: "S.Lượng",
                alignment: "center",
                width: 100,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    const rowData = options.data;
                    const slnk = rowData.SLNK || 0;
                    const slkk = rowData.SLKiemKe || 0;

                    if (slnk !== slkk) {
                        container.css('background-color', '#fff3cd'); // Màu vàng nhạt
                    }

                    let result = formatNumber(slnk)
                    container.text(result);

                }
            },
            {
                dataField: "SLKiemKe",
                caption: "SLKK",
                alignment: "center",
                width: 100,
                cssClass: "position-relative",
                cellTemplate: function (container, options) {
                    const rowData = options.data;
                    const isDisabled = rowData.IsXacNhan === 1;
                    const currentValue = rowData.SLKiemKe || 0;
                    let resultValueDisplay = formatNumber(currentValue)
                    const slnk = rowData.SLNK || 0;

                    // Tô màu nếu khác nhau
                    if (slnk !== currentValue) {
                        container.css('background-color', '#fff3cd'); // Màu vàng nhạt
                    }
                    const $wrapper = $(`<div>`)
                        .css({
                            'display': 'flex',
                            'align-items': 'center',
                            'justify-content': 'center',
                            'gap': '8px',
                            'width': '100%'
                        });

                    // Hiển thị giá trị
                    const $valueSpan = $('<span>')
                        .addClass('slkk-value')
                        .text(resultValueDisplay)
                        .css({
                            'font-size': '13px',
                            'min-width': '50px'
                        });
                    const $editIcon = $('<i>')
                        .addClass('fa fa-edit position-absolute')
                        .css({
                            'cursor': 'pointer',
                            'color': '#007bff',
                            'font-size': '14px',
                            'top': '1px',
                            'right': '4px'
                        })
                        .click(function () {
                            const itemCode = rowData.MaVT
                            const mauVT = rowData.MauVT
                            const khosize = rowData.KhoVai
                            const roll = rowData.SoKien
                            const ghiChuKK = rowData.GhiChuKK
                            const soluong = rowData.SLNK
                            $("#itemCode").text(itemCode)
                            $("#color").text(mauVT)
                            $("#khosizeM").text(khosize)
                            $("#roll").text(roll)
                            $("#quantity").text(soluong)
                            $("#inventoryQty").val(currentValue)
                            $("#note").val(ghiChuKK)
                            showConfirmModal(async function () {
                                const newSLKK = parseFloat($("#inventoryQty").val())
                                const ghichu = $("#note").val()
                                if (parseFloat(slnk) != newSLKK && ghichu == "") {
                                    showToast("warning", "Vui lòng nhập lí do chỉnh sửa số lượng kiểm kê!");
                                    $("#note").focus();
                                    return false; // ❌ không cho đóng
                                }
                                var arrSave = []
                                var user = localStorage.getItem("username1")
                                var dataUser = !window.CefSharp ? user : userName
                                arrSave.push({
                                    SoLoID: 1,
                                    SoLo: 1,
                                    MaNPL: 1,
                                    MaVTID: 1,
                                    MauVTID: 1,
                                    IsNPL: 0,
                                    BarCode: rowData.BarCode,
                                    SLKiemKe: 1,
                                    SLKiemKeEdit: newSLKK,
                                    GhiChu: ghichu,
                                    UserKK: dataUser,
                                    DateKiemKe: '2026-01-01',
                                    IsXacNhan: 1,
                                    PhieuKiemKe: $("#phieuvattukiem").val()
                                })
                                rowData.SLKiemKe = newSLKK;
                                rowData.GhiChuKK = ghichu;

                                dxKiemKePhuLieuRight.refresh()

                                SaveUpdateKiemKe(arrSave)
                                return true; // ✅ cho đóng
                            })
                        })
                        .attr('title', 'Chỉnh sửa')

                    $wrapper.append($valueSpan).append($editIcon);
                    container.append($wrapper);
                }
            },
            {
                dataField: "GhiChuKK",
                caption: "Ghi chú",
                width: 250,

            }

        ],
        summary: {
            totalItems: [{
                name: "tongKho",
                summaryType: "custom",
                showInColumn: "ChiTiet",

            },
            {
                name: "SLNK",
                summaryType: "sum",
                column: "SLNK",
                valueFormat: "#,##0.##",
                customizeText(e) {
                    if (e.value == null) return "";

                    return formatNumber(e.value)
                }
            },
            {
                name: "SLKiemKe",
                summaryType: "sum",
                column: "SLKiemKe",
                valueFormat: "#,##0.##",
                customizeText(e) {
                    if (e.value == null) return "";

                    return formatNumber(e.value)
                }
            },],
            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {
                    if (options.summaryProcess === "start") {
                        options.totalValue = { totalRoll: 0, totalNH: 0 };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalRoll += 1;
                        options.totalValue.totalNH += options.value.SLNK || 0;
                    }
                    if (options.summaryProcess === "finalize") {
                        const { totalRoll, totalNH } = options.totalValue;
                        options.totalValue = `${totalRoll} Vật tư`;
                    }
                }
            }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css({ "vertical-align": "middle", });
            }
        },
    }).dxDataGrid("instance");
    $("#Layer_1").click()
}
async function SaveUpdateKiemKe(updatedList) {
    const url = `/api/PhieuXuatHangNPL/PostChiTietKiemKeNPL?action=PostKiemKeNPL`;
    try {
        // POST tất cả cùng lúc
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updatedList)
        });
        const results = await response.json();

        if (results == "True") {
            showToast("success", `Đã cập nhật số lượng kiểm kê thành công!`);
        } else {
            showToast("warning", `Đã cập nhật số lượng kiểm kê thất bại!`);
        }
    } catch (err) {
        console.error("Lỗi:", err);
        showToast("error", "Có lỗi xảy ra khi lưu dữ liệu!");
    }
}
function showConfirmModal(onConfirm) {
    $('#saveBtn').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#inventoryModal").modal("hide");
    });

    $("#inventoryModal").modal("show");
}
function createViewDxLichSuQuet() {
    dxGridLichSuQuet = $("#dxLichSuQuetPL").dxDataGrid({
        dataSource: lstLichSuQuet,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: { enabled: false },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "poMua",
                caption: "PO Mua",
                dataType: 'string',
                width: 100,
                alignment: 'center'
            },
            {
                dataField: "itemCode",
                caption: "Item Code",
                dataType: 'string',
                width: 90,
                alignment: 'center',
                cellTemplate: function (container, options) {
                    container.append(
                        $("<div>")
                            .css({
                                "white-space": "normal",
                                "word-wrap": "break-word"
                            })
                            .text(options.value)
                    );
                }
            },
            {
                dataField: "moTa",
                caption: "Mô tả",
                dataType: 'string',
                width: 170,
                alignment: 'left'
            },
            {
                dataField: "roll",
                caption: "Vật tư",
                dataType: 'string',
                width: 100,
                alignment: 'center',
            }
        ],
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                //  THÊM STYLE CHO HEADER
                $(e.cellElement).css({
                    "background-color": "#eaf0f1",
                    "color": "black",
                    "text-align": "center",
                    "vertical-align": "middle",
                });
            }
            if (e.rowType === "data") {
                $(e.cellElement).css({
                    "text-align": "center",
                    "vertical-align": "middle",
                });
            }
        },
    }).dxDataGrid("instance");

    $("#Layer_1").click();
}
//  Xử lý highlight và đưa row lên đầu
function CheckBarcodeTableDx(barcode) {
    if (!dxDataPLTongQuan) {
        return false;
    }
    const dataSource = dxDataPLTongQuan.getDataSource();
    const allData = dataSource.items();


    // Tìm row có barcode trùng khớp
    const targetIndex = allData.findIndex(item => item.BarCode === barcode);
    if (targetIndex !== -1) {
        // Lấy item cần highlight
        const targetItem = allData[targetIndex];

        updateThongTinQuet(targetItem);

        // Xóa item khỏi vị trí cũ và thêm vào đầu
        allData.splice(targetIndex, 1);
        allData.unshift(targetItem);

        // Cập nhật lại dataSource
        dxDataPLTongQuan.beginUpdate();
        dxDataPLTongQuan.option('dataSource', allData);
        dxDataPLTongQuan.endUpdate();

        // Scroll to top
        dxDataPLTongQuan.getScrollable().scrollTo(0);

        // Highlight row
        setTimeout(() => {
            const rowElement = dxDataPLTongQuan.getRowElement(0);
            if (rowElement && rowElement.length > 0) {
                // Xóa highlight cũ
                $('.dx-row').removeClass('highlight-row');

                // Thêm highlight mới
                $(rowElement).addClass('highlight-row');
            }
        }, 100);

        return true;
    } else {
        PlayAudioError();
        showToast("warning", `Không tìm thấy barcode ${barcodeLocal} trong danh sách`);
    }

    return false;
}
// ===== HAM HO TRO =====
async function handleInventoryScanDx(barcode) {
    const url = `/api/PhieuXuatHangNPL/PostKiemKeNPL?action=PostKiemKeNPL`;
    if (!barcode || barcode.trim() === "") {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan!");
        PlayAudioError();
        return;
    }

    const leftDataSource = dxKiemKePhuLieuLeft.getDataSource();
    const leftData = leftDataSource.items();

    const targetIndex = leftData.findIndex(item => item.BarCode === barcode);

    if (targetIndex !== -1) {
        const targetItem = leftData[targetIndex];

        // LẤY THÔNG TIN TỪ TỔNG QUAN ĐỂ CÓ STATUS CHÍNH XÁC
        const fullInfo = getItemInfoFromTongQuan(barcode) || targetItem;
        fullInfo.UserKK = userNameSave;
        fullInfo.IsNPL = 0;
        fullInfo.SLKiemKe = fullInfo.SLNK;
        fullInfo.GhiChu = ""
        fullInfo.PhieuKiemKe = $("#phieuvattukiem").val()
        // CẬP NHẬT THÔNG TIN QUÉT
        updateThongTinQuet(fullInfo);

        // THÊM VÀO LỊCH SỬ QUÉT
        if (isScannedFromModal) {
            addToLichSuQuet(fullInfo);
        }


        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(fullInfo)
            });

            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const result = await response.json();

            if (result === "True" || response.ok) {
                PlayAudio();

                //  Load lại dữ liệu bảng phải sau khi POST thành công**
                await reloadKiemKeRightByMaVT(fullInfo.MaVT);

                // Xóa item khỏi bảng trái
                leftData.splice(targetIndex, 1);
                dxKiemKePhuLieuLeft.beginUpdate();
                dxKiemKePhuLieuLeft.option('dataSource', leftData);
                dxKiemKePhuLieuLeft.endUpdate();

                highlightBarcodeInRightGrid(barcode);

                // Scroll to top bảng phải
                dxKiemKePhuLieuRight.getScrollable().scrollTo(0);
            }

        } catch (err) {
            console.error("Lỗi POST:", err);
            PlayAudioError();
        }

        if (dxGridLichSuQuet) {
            dxGridLichSuQuet.refresh();
        }

        $("#QRScan").val("");

    } else {
        const rightDataSource = dxKiemKePhuLieuRight.getDataSource();
        const rightData = rightDataSource.items();
        const existInRight = rightData.findIndex(item => item.BarCode === barcode);

        if (existInRight !== -1) {
            const existingItem = rightData[existInRight];

            // LẤY THÔNG TIN TỪ TỔNG QUAN
            const fullInfo = getItemInfoFromTongQuan(barcode) || existingItem;

            updateThongTinQuet(fullInfo);

            showToast("warning", `Barcode ${barcode} đã được kiểm kê rồi!`);
            PlayAudioError();

            setTimeout(() => {
                const rowElement = dxKiemKePhuLieuRight.getRowElement(existInRight);
                if (rowElement && rowElement.length > 0) {
                    $('.dx-row').removeClass('highlight');
                    $(rowElement).addClass('highlight');
                    dxKiemKePhuLieuRight.getScrollable().scrollToElement($(rowElement));
                }
            }, 100);
        } else {
            if (!isPostSwitchScan) {
                showToast("error", `Không tìm thấy barcode ${barcode} trong danh sách!`);
                PlayAudioError();
            }
        }
        $("#QRScan").val("");
    }
}
async function performVTSwitchDx(selectNPL) {
    isPostSwitchScan = true;

    var $select = $("#cayvai");
    var $matchedOption = $select.find("option").filter(function () {
        return $(this).val() == selectNPL;
    });

    if ($matchedOption.length > 0) {
        $matchedOption.prependTo($select);
        $select.val(selectNPL);
        await loadPINCC(1);
    }
}
function clearInventoryTablesDx() {
    if (dxKiemKePhuLieuLeft) {
        dxKiemKePhuLieuLeft.beginUpdate();
        dxKiemKePhuLieuLeft.option('dataSource', []);
        dxKiemKePhuLieuLeft.endUpdate();
    }

    if (dxKiemKePhuLieuRight) {
        dxKiemKePhuLieuRight.beginUpdate();
        dxKiemKePhuLieuRight.option('dataSource', []);
        dxKiemKePhuLieuRight.endUpdate();
    }

    // RESET FLAG KHI XÓA DATA
    isFirstLoadKiemKe = true;
}
// ===== SCANNER ====
function updateThongTinQuet(itemData) {
    if (!itemData) {
        // Reset về mặc định
        $("#txtPoMua").text("—");
        $("#txtLotBatch").text("—");
        $("#txtItemCode").text("—");
        $("#txtRoll").text("—");
        $("#txtTrangThai").html('<span style="color: gray; font-weight: bold;">—</span>');
        return;
    }

    // Cập nhật thông tin
    $("#txtPoMua").text(itemData.PoMua || "—");
    $("#txtLotBatch").text(itemData.SoLoT || "—");
    $("#txtItemCode").text(itemData.MaVT || "—");
    $("#txtRoll").text(itemData.SoKien || "—");

    // XỬ LÝ TRẠNG THÁI ĐÚNG
    const status = itemData.Status;
    let trangThai = "—";
    let color = "gray";

    if (status === 0) {
        trangThai = "Nhập kho";
        color = "green";
    } else if (status === 1) {
        trangThai = "Xuất hàng";
        color = "red";
    } else if (status === 2) {
        trangThai = "Thu hồi";
        color = "orange";
    } else if (status === 4) {
        trangThai = "Chưa nhập kho";
        color = "#D39E00";
    } else {
        trangThai = "Không xác định";
        color = "gray";
    }

    $("#txtTrangThai").html(`<span style="color: ${color}; font-weight: bold;">${trangThai}</span>`);
}
function addToLichSuQuet(itemData) {
    if (!itemData) {
        console.error("itemData is null/undefined");
        return;
    }
    // Tạo object cho lịch sử quét
    const lichSuItem = {
        poMua: itemData.PoMua || "",
        itemCode: itemData.MaVT || "",
        moTa: itemData.ChiTiet || "",
        roll: itemData.SoKien || "",
        lotBatch: itemData.SoLoT || "",
        donVi: itemData.TenDVVT || "",
        barCode: itemData.BarCode || "",
    };

    // Kiểm tra trùng lặp barcode
    const existIndex = lstLichSuQuet.findIndex(item => item.barCode === lichSuItem.barCode);

    if (existIndex !== -1) {
        lstLichSuQuet.splice(existIndex, 1);
    }
    // Thêm vào đầu mảng
    lstLichSuQuet.unshift(lichSuItem);
    // Cập nhật grid
    updateLichSuQuetGrid();
}
function updateLichSuQuetGrid() {
    if (!dxGridLichSuQuet) {
        console.error("dxGridLichSuQuet chưa được khởi tạo!");
        return;
    }
    try {
        dxGridLichSuQuet.beginUpdate();
        dxGridLichSuQuet.option('dataSource', [...lstLichSuQuet]);
        dxGridLichSuQuet.endUpdate();
        // Force refresh
        setTimeout(() => {
            dxGridLichSuQuet.refresh();
        }, 100);
    } catch (error) {
        console.error("Lỗi khi cập nhật grid:", error);
    }
}
$(document).ready(function () {
    createViewDxDataTongQuan();
    createViewDxDataKiemKe();
    createViewDxLichSuQuet();
    updateThongTinQuet(null);
    loadKHKiemKe();
});
$(function () {
    $("#khachhangkiemke").on("change", function () {
        loadMHKiemKe();
        GetVatTuKiemKe(1);
    });

    $("#mahangkiemke").on("change", function () {
        GetVatTuKiemKe(1);
    });


});

/// Kiểm kê
async function loadKHKiemKe() {
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GETKHTQVT&para1=0`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        const $khachHangSelect = $("#khachhangkiemke");

        if ($khachHangSelect.data('select2')) {
            $khachHangSelect.select2('destroy');
        }
        $khachHangSelect.empty();
        $khachHangSelect.append(`<option value="all">Tất cả</option>`);

        if (data.length > 0) {
            let html = '';
            data.map(x => {
                html += `<option value="${x.MaKH}">${x.TenKH}</option>`;
            });
            $khachHangSelect.append(html);
        }
        $khachHangSelect.select2();
        loadMHKiemKe();
    } catch (error) {
        console.error("Lỗi khi tải danh sách khách hàng kiểm kê:", error.message);
    }
}

async function loadMHKiemKe() {
    const maKH = $("#khachhangkiemke").val();
    const url = `/api/PhieuXuatHangNPL/GetTH?action=GETMHTQVT&para1=0&para2=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;
        data.map(x => {
            html += `<option value="${x.MaHang}">${x.MaHang == "" ? "Mã hàng rỗng" : x.MaHang}</option>`;
        });
        $("#mahangkiemke").html(html);
        $("#mahangkiemke").select2();
        //    GetVatTuKiemKe();
    } catch (error) {
        console.error("Lỗi khi tải danh sách mã hàng kiểm kê:", error.message);
    }
}

async function GetVatTuKiemKe() {
    const makh = $("#khachhangkiemke").val();
    const mahang = $("#mahangkiemke").val();
    const tuNgayFormat = "1990-01-01";
    const denNgayFormat = "1990-01-01";

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetVTMKKiemKe&para1=0&para2=${makh}&para3=${mahang}&para4=${tuNgayFormat}&para5=${denNgayFormat}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;

        if (data.length > 0) {
            data.map(x => {
                html += `<option value="${x.MaNPL}">${x.Display}</option>`;
            });
        }

        $("#cayvaikiemke").html(html);
        $("#cayvaikiemke").select2();

    } catch (error) {
        console.error("Lỗi khi tải vật tư kiểm kê:", error.message);
    }
}

//const inventoryTab = document.getElementById("inventory-record-tab");

//inventoryTab.addEventListener("shown.bs.tab", function () {
//    $(".card-body").addClass("d-none");

//    if (!dxDataGhiNhanKiemKe) {
//        console.error("Grid chưa khởi tạo");
//        return;
//    }
//    GetVatTuKiemKe()
//    dxDataGhiNhanKiemKe.beginUpdate();
//    dxDataGhiNhanKiemKe.endUpdate();
//    isInventoryRecordLoaded = true;
//});

//inventoryTab.addEventListener("hidden.bs.tab", function () {
//    $(".card-body").removeClass("d-none");
//});
async function reloadKiemKeRightByMaVT(maVT) {
    const maNPL = manplCurrent;
    const soloid = $("#soloid").val();
    const maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    const maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();

    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";

    const url = `/api/PhieuXuatHangNPL/GetTH?action=GetKiemKe&para1=${maNPL}&para2=${soloid}&para3=${maHang}&para4=${maKH}&para5=${tuNgayFormat}&para6=${denNgayFormat}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();

        if (!dxKiemKePhuLieuRight) return;

        // Cập nhật dữ liệu bảng phải
        dxKiemKePhuLieuRight.beginUpdate();
        dxKiemKePhuLieuRight.option({
            dataSource: data,
            repaintChangesOnly: true
        });
        dxKiemKePhuLieuRight.endUpdate();

    } catch (error) {
        console.error("Lỗi khi reload dữ liệu kiểm kê:", error.message);
    }
}

function highlightBarcodeInRightGrid(barcode) {
    if (!dxKiemKePhuLieuRight || !barcode) return;

    setTimeout(() => {
        const rightData = dxKiemKePhuLieuRight.getDataSource().items();
        const targetIndex = rightData.findIndex(item => item.BarCode === barcode);

        if (targetIndex !== -1) {
            // Xóa highlight cũ
            $('.dx-row').removeClass('highlight');

            // Scroll đến row
            dxKiemKePhuLieuRight.getScrollable().scrollTo(0);

            // Highlight row mới
            setTimeout(() => {
                const rowElement = dxKiemKePhuLieuRight.getRowElement(targetIndex);
                if (rowElement && rowElement.length > 0) {
                    $(rowElement).addClass('highlight');
                }
            }, 100);

        } else {
            console.warn(`⚠️ Không tìm thấy barcode ${barcode} trong bảng phải sau khi reload`);
        }
    }, 300); // Đợi DevExtreme render xong
}


function formatDate(dateStr) {
    if (!dateStr) return "";
    const d = new Date(dateStr);
    if (isNaN(d)) return dateStr; // phòng khi chuỗi không parse được

    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();

    return `${day}/${month}/${year}`;
}


async function GetPhieuVatTuKK() {
    const url = `/api/PhieuXuatHangNPL/GetDS?action=GetPhieuVatTuKK&para1=0`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        let html = `<option value=""></option>`;
        data.Table.forEach(x => {
            html += `<option value="${x.PhieuVatTuKK}">Phiếu VTKK: ${x.Dot} - ${x.NgayTao}</option>`

        })
        $("#phieuvattukiem").html(html)
    } catch (error) {
        console.error(error.message);
    }
}
async function GetVatTuKK(value) {
    const phieuVTKK = $("#phieuvattukiem").val()
    const url = `/api/PhieuXuatHangNPL/GetDS?action=GetVatTuKiemKe&para1=0&para2=${phieuVTKK}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        allData = await data.Table

        if (value == 1) {
            console.log(manplCurrent)
            await renderSwitch(manplCurrent)
        }
    } catch (error) {
        console.error(error.message);
    }
}

$(document).ready(function () {
    GetPhieuVatTuKK()

    // Khi click vào input hiển thị
    $('#displayInput').on('click', function () {
        displayData(allData);
        $('#dropdownTable').show();
        $('#searchInput').val('').focus();
    });

    // Khi nhập vào input tìm kiếm
    $('#searchInput').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();

        if (searchTerm === '') {
            displayData(allData);
        } else {
            const filtered = allData.filter(item =>
                item.Display.toLowerCase().includes(searchTerm)
            );
            displayData(filtered);
        }

        $('#dropdownTable').show();
    });

    // Khi click vào một row
    $(document).on('click', '#tableBody tr', function () {
        const itemCode = $(this).data('manpl');
        const selectedItem = allData.find(item => item.MaNPL === itemCode);

        if (selectedItem) {
            showSelectedInfo(selectedItem);
        }
    });

    // Đóng dropdown khi click bên ngoài
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.search-container, #displayInput,#searchInput').length) {
            $('#dropdownTable').hide();
        }
    });
    $("#phieuvattukiem").on("change", async function (e) {
        $('#displayInput').val("");
        $('#searchInput').val('');
        if (e.originalEvent) {
            manplCurrent = ""
            await LoadChiTietVTDx();
            await LoadChiTietVTKiemKeDx()
            GetVatTuKK()

        } else {
            GetVatTuKK(1)
        }

    });

})
// Hàm scroll đến phiếu được chọn
function scrollToSelectedRow(phieu) {
    const row = $(`#tableBody tr[data-display="${phieu}"]`);
    if (row.length > 0) {
        const dropdown = $('#dropdownTable');

        // Sử dụng scrollIntoView của browser - reliable hơn
        row[0].scrollIntoView({
            behavior: 'smooth',
            block: 'center',
            inline: 'nearest'
        });
    }
}

// Hiển thị tất cả dữ liệu
function displayData(dataToShow) {
    const tbody = $('#tableBody');
    tbody.empty();

    if (dataToShow.length === 0) {
        tbody.append('<tr><td colspan="4" class="no-results">Không tìm thấy kết quả</td></tr>');
        return;
    }

    const searchTerm = $('#searchInput').val().toLowerCase();

    dataToShow.forEach(item => {
        let itemCode = item.Display;
        if (searchTerm && item.Display.toLowerCase().includes(searchTerm)) {
            const regex = new RegExp(`(${searchTerm})`, 'gi');
            itemCode = item.Display.replace(regex, '<span class="highlight">$1</span>');
        }

        const isSelected = item.Display === currentSelectedPhieu ? 'selected' : '';

        const row = `
                        <tr data-display="${item.Display}" data-manpl="${item.MaNPL}" class="${isSelected}">
                            <td>${itemCode}</td>
                            <td>${item.SLNhapKho.toLocaleString()}</td>
                        </tr>
                    `;
        tbody.append(row);
    });

    // Highlight và scroll đến phiếu đang chọn nếu có
    if (currentSelectedPhieu) {
        setTimeout(() => {
            scrollToSelectedRow(currentSelectedPhieu);
        }, 100);
    }
}
// Hàm hiển thị thông tin phiếu đã chọn
async function showSelectedInfo(selectedItem) {
    currentSelectedPhieu = selectedItem.Display;
    $('#displayInput').val(selectedItem.Display);
    $('#searchInput').val('');
    $('#dropdownTable').hide();
    manplCurrent = selectedItem.MaNPL

    await LoadChiTietVTDx();
    await LoadChiTietVTKiemKeDx()
}
function renderSwitch(manpl) {
    const selectedItem = allData.find(item => item.MaNPL.toUpperCase() === manpl);

    if (selectedItem) {
        showSelectedInfo(selectedItem);
        $('#directInput').val('');
    }
}

async function Export() {
    let dataEX = dxKiemKePhuLieuRight.option("dataSource")
    if (dataEX.length === 0) {
        showToast("warning", "Không có thông tin dữ liệu kiểm kê");
        return;
    }

    // Tạo workbook và worksheet
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("Kiểm Kê");

    // Định nghĩa các cột
    worksheet.columns = [
        { header: "Code", key: "Code", width: 20 },
        { header: "Số Roll", key: "SoRoll", width: 15 },
        { header: "Số Lượng", key: "SoLuong", width: 15 },
        { header: "Ghi Chú", key: "GhiChu", width: 30 }
    ];

    // Thêm dữ liệu vào worksheet
    dataEX.forEach(item => {
        worksheet.addRow({
            Code: item.MaVT || "",
            SoRoll: item.SoKien || 0,
            SoLuong: item.SLKiemKe || 0,
            GhiChu: item.GhiChuKK || ""
        });
    });

    // Style border cho tất cả các ô
    const borderStyle = {
        top: { style: 'thin' },
        left: { style: 'thin' },
        bottom: { style: 'thin' },
        right: { style: 'thin' }
    };

    // Áp dụng style cho header (row 1)
    worksheet.getRow(1).eachCell((cell) => {
        cell.font = { bold: true };
        cell.alignment = { vertical: 'middle', horizontal: 'center' };
        cell.border = borderStyle;
    });

    // Áp dụng style cho các row dữ liệu
    worksheet.eachRow((row, rowNumber) => {
        if (rowNumber > 1) { // Bỏ qua header
            row.eachCell((cell) => {
                cell.alignment = { vertical: 'middle', horizontal: 'center' };
                cell.border = borderStyle;
            });
        }
    });

    // Tạo tên file với timestamp
    const fileName = `DSKiemKePL_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`;

    // Xuất file
    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);

    showToast("success", "Xuất Excel thành công!");
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



$(function () {
    $("#confirmVT").on("click", function () {
        const phieuVatTuKiem = $("#phieuvattukiem").val();
        if (!phieuVatTuKiem || phieuVatTuKiem == '') {
            showToast("warning", "Vui lòng chọn phiếu !")
            return;
        }

        const maVTInput = $("#displayInput").val().trim();
        if (!maVTInput || maVTInput == '') {
            showToast("warning", "Vui lòng chọn Item Code để xác nhận hoàn thành vật tư !")
            return;
        }

        openConfirmModal(2)
        showConfirmModalXacNhan(async function () {
            const module = 'M.10.17.00'
            const title = ''
            const detail = `Phiếu: ${phieuVatTuKiem} - ${maVTInput} đã hoàn thành kiểm kê !`
            const sendTo = 'Kho'
            const BoPhan = "ALL"
            const Status = 2

            sendNotify(module, title, detail, sendTo, BoPhan, Status)
            return true; // ✅ cho đóng
        })
    })
    $("#confirmPhieu").on("click", function () {
        const phieuVatTuKiem = $("#phieuvattukiem").val();
        if (!phieuVatTuKiem || phieuVatTuKiem == '') {
            showToast("warning", "Vui lòng chọn phiếu để gửi xác nhận hoàn thành phiếu !")
            return;
        }

        openConfirmModal(1)
        showConfirmModalXacNhan(async function () {
            const module = 'M.10.17.00'
            const title = ''
            const detail = `Phiếu: ${phieuVatTuKiem} đã hoàn thành kiểm kê !`
            const sendTo = 'Kho'
            const BoPhan = 'ALL'
            const Status = 1

            sendNotify(module, title, detail, sendTo, BoPhan, Status)
            return true; // ✅ cho đóng
        })
    })
})
function openConfirmModal(loai) {
    _mcLoai = loai;
    const isPhieu = loai === 1;

    $('#modalTieuDe').text(isPhieu ? `Xác nhận kiểm xong Phiếu` : `Xác nhận kiểm xong Vật tư`);
    $('#mcHeader').attr('class', `modal-header mc-modal-header${isPhieu ? '' : ' mc-vattu'}`);
    $('#mcIconWrap').attr('class', `mc-icon-wrap ${isPhieu ? 'mc-phieu' : 'mc-vattu'}`);
    $('#confirmIcon').attr('class', `bi ${isPhieu ? 'bi-file-earmark-check' : 'bi-box-seam'}`);
    $('#confirmMoTa').text(isPhieu
        ? `Bạn xác nhận đã kiểm xong Phiếu và muốn gửi lên quản lý để duyệt?`
        : `Bạn xác nhận đã kiểm xong Vật tư và muốn gửi lên quản lý để duyệt?`);
    $('#mcLoaiTen').text(isPhieu ? `phiếu` : `vật tư`);
    $('#mcBadge').text(isPhieu ? `Phiếu` : `Vật tư`).attr('class', `mc-badge ${isPhieu ? 'mc-phieu' : 'mc-vattu'}`);
    $('#mcBtnSubmit').attr('class', `btn mc-btn-submit${isPhieu ? '' : ' mc-vattu'}`);


}
function showConfirmModalXacNhan(onConfirm) {
    $('#btnSubmitConfirm').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#modalXacNhan").modal("hide");
    });

    $("#modalXacNhan").modal("show");
}

// module = M.19.00.00
//title Hoàn Thành kiểm kê phiếu 
//detail //Maha, vat ""
//sendTo ERP_PhongBan ->  KHo 
//BoPhan ERP_BoPhan ->  KhoPL or KhoNL  or TBP = All
//Status 1 Truong BP  Ha, 2 -> to truong Can

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