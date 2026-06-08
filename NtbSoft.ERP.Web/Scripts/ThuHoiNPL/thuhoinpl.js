
var userNameSave = localStorage.getItem("username1")
var sortPhieu = 0;
var $currentRow;
var tungaytc = ""
var denngaytc = ""
var htmltbodyCurrent;
var htmltbodyCurrentReport;
var checkChangeTable;
var checkChangeTableReport;
var valueSelectReport;
var timeoutId2;
var checkPalet;
var selectedPhieuDNTH;
var tempRowsToDelete = [];
var sort;
$(function () {
    $(document).on('keydown', async function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();

            // Kiểm tra xem đang focus vào input nào
            if ($('#QRScanSoLuong:focus').length > 0 || $('#QRScanVatTuLoi:focus').length > 0) {
                barcodeLocal = e.target.value.trim();

                // Xóa giá trị
                var $input = $(e.target);
                var value = $input.val().trim();
                $input.val("");

                // Gọi hàm với giá trị đã lưu
                await GetCheckBarCode(value);
            } else {
                showToast("warning", "Vui lòng chọn vào ô QR scan.")
            }
        }
    });


    $(".select_2").select2();

    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })

    $("#maphieukiem").on("change", async function () {
        selectedPhieuDNTH = $(this).val();
        GetDanhSachItemCode()

        //DataPhieu();
    });
    $(".enterbarcode").on("click", function () {
        if ($('#QRScanSoLuong').val().trim() != "") {
            GetCheckBarCode($('#QRScanSoLuong').val().trim())
            $("#QRScanSoLuong").val("")
        }
        else {
            showToast("warning", `Vui lòng nhập barcode vào ô QR scan.`);
        }
    });
    $("#cbmModal").on('hidden.bs.modal', function () {
        $("#checkAllCBMTH").prop("checked", false);
        $("#dxGridThuHoiSoLuong").find(".checkCBM").prop("checked", false)
    });
})




async function PhieuScan() {
    const isActiveTab = tabActive === "thu-hoi-so-luong" ? 1 : 2;
    var url = `/api/PhieuXuatHangNPL/GetTH?action=GetPhieu&para1=1990-01-01&para2=1990-01-01&para3=${isNPL}&para4=${isActiveTab}`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        let html = `<option data-sort="0" value="">Phiếu mới</option>`;

        // Lấy giá trị đã chọn trước đó
        const savedValue = isActiveTab === 1 ? selectedPhieuThuHoiSoLuong : selectedPhieuThuHoiVatTuLoi;

        data.map(x => {
            // So sánh với giá trị đã lưu thay vì PTH_sortPhieu
            var selected = savedValue === x.MaPhieu ? "selected" : "";
            html += `
                <option data-sort="${x.Sort}" ${selected} value="${x.MaPhieu}">${x.Display}</option>
            `;
        });

        $("#maphieukiemvattuloi").html(html);
    } catch (error) {
        console.error(error.message);
    }
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

function showConfirmModalDeleteLichSu(onConfirm) {
    $('#btnConfirmtDeleteVT').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#modalComfimrtDeleteVTLichSu").modal("hide");
    });

    $("#modalComfimrtDeleteVTLichSu").modal("show");
}

// Check1
async function Save() {
    const dataSource = dataThuHoi
    const itemChuaNhapCBM = dataSource.find(item => !item.CBM)
    if (itemChuaNhapCBM) {
        showToast("warning", `Vui lòng nhập CBM cho MaVT: ${itemChuaNhapCBM.MaVT}`)
        return;
    }

    showConfirmModalSign(async function () {
        var dataUser = !window.CefSharp ? userNameSave : userName
        var ArrSaveTH = []
        var ArrSaveTHSign = []
        var ArrSaveONPL = [];

        if (signatureHistory.length === 0) {
            alert('Vui lòng ký tên trước khi lưu!');
            return;
        }
        // Chuyển canvas thành hình ảnh
        const signatureImage = canvas.toDataURL('image/png');
        const dataSource = dataThuHoi
        const phieuTH = $("#maPhieuThuHoi").val();
        dataSource.map(item => {
            const thuHoiObject = {
                MaPhieu: isExist == 0 ? '' : phieuTH,
                MaDVSX: selectedRowPhieuDNTH,
                TenDVSX: item.MaNPL,
                MaLenh: item.MaLenh,
                MaPhieuYC: 1,
                Sort: sort,
                Dot: item.DotXH ?? item.Dot,
                NgayTH: "",
                NgayCN: "",
                NguoiTao: item.isNew ? dataUser : "",
                NguoiCN: !item.isNew ? dataUser : "",
                BarCode: item.BarCode,
                ThuHoi: item.SLXuat,
                Palet: item.Palet ?? "",
            }

            const thuHoiSignObject = {
                PhieuDNTH: selectedRowPhieuDNTH,
                PhieuTH: phieuTH,
                MaNPL: item.MaNPL,
                BarCode: item.BarCode,
                ChuKy: signatureImage,
            }

            const oNPLObject =
            {
                ID: 0,
                SoLoID: item.SoLoID,
                MaNPL: item.MaNPL,
                MaVTGhep: item.MaVTGhep,
                BarCode: item.BarCode,
                Dai: 0,
                Rong: 0,
                Cao: 0,
                DK: 0,
                CBM: 0,
                IsNPL: isNPL,
                IsCBM: 0,
                MaONPL: item.ViTriO !== '1' && item.ViTriO ? item.ViTriO : null,
            }

            ArrSaveTH.push(thuHoiObject)
            ArrSaveTHSign.push(thuHoiSignObject)
            ArrSaveONPL.push(oNPLObject)
        })
        console.log("ArrSaveTH: ", ArrSaveTH)
        console.log("ArrSaveTHSign: ", ArrSaveTHSign)
        console.log("ArrSaveONPL: ", ArrSaveONPL)

        const isSuccess = await ApiSavePhieu(ArrSaveTH)
        if (isSuccess) {
            await ApiSavePhieuKyTen(ArrSaveTHSign)
            await SaveONPL(ArrSaveONPL)
        }

        return true;
    })
}
async function ApiSavePhieu(arrSave) {
    console.log("ApiSavePhieu: ", arrSave)
    try {
        const request = new Request(`/api/PhieuXuatHangNPL/PostTH?action=PostTH&para1=${isNPL}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(arrSave),
        });
        await fetch(request)
        // Refresh grid để ẩn nút xóa
        dataThuHoi = []
        createViewDxGridThuHoiSoLuong([])
        GetPhieuThuHoiSoLuong()
        showToast("success", "Lưu thành công")
        return true;

    } catch (err) {
        console.error(err)
        return false;
    }

}
async function ApiSavePhieuKyTen(arrSave) {

    try {
        const request = new Request(`/api/PhieuXuatHangNPL/PostTHNPL?action=PostThuHoiKiTen`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(arrSave),
        });
        let response = await fetch(request)
        let result = await response.json()

        if (result == "True") {

        }

    } catch (err) {
        console.error(err)
    }
}
async function ApiDeleteLichSu(PhieuTH, BarCode) {
    try {
        const url = `/api/PhieuXuatHangNPL/GetTH?Action=DeleteLSThuHoi&para1=${PhieuTH}&para2=${BarCode}`
        await fetch(url);

        // Cập nhật dxPhieuCap2
        const dataSourcePC2 = dxPhieuCap2.option("dataSource");
        const deletedItem = dataSourcePC2.find(item => item.PhieuTH == PhieuTH && item.BarCode == BarCode);
        const newDataPC2 = dataSourcePC2.filter(item => !(item.PhieuTH == PhieuTH && item.BarCode == BarCode));
        dxPhieuCap2.option({ dataSource: newDataPC2 });
        dxPhieuCap2.refresh();

        // Cập nhật lại SLTH bên dxPhieuCap1
        if (deletedItem) {
            const dataSourcePC1 = dxPhieuCap1.option("dataSource");
            const rowPC1 = dataSourcePC1.find(item => item.MaNPL == deletedItem.MaNPL);

            if (rowPC1) {
                // Tính lại SLTH từ data còn lại trong PC2
                const tongSLTH = newDataPC2
                    .filter(item => item.MaNPL == deletedItem.MaNPL)
                    .reduce((sum, item) => sum + (parseFloat(item.ThuHoi) || 0), 0);

                if (tongSLTH == 0) {
                    const newDataPC1 = dataSourcePC1.filter(item => item.MaNPL !== deletedItem.MaNPL);
                    dxPhieuCap1.option({ dataSource: newDataPC1 });
                    dxPhieuCap1.refresh();
                    GetDanhSachItemCode()
                    showToast("success", "Xóa thành công");
                    return;
                }
                rowPC1.SLTH = tongSLTH;
                dxPhieuCap1.option({ dataSource: [...dataSourcePC1] });
                dxPhieuCap1.refresh();
            }
        }

        GetDanhSachItemCode()
        showToast("success", "Xóa thành công");

    } catch (err) {
        console.error(err);
        showToast("error", "Xóa thất bại");
    }
}
async function PhieuScanReport(para1, para2) {
    var url = `/api/PhieuXuatHangNPL/GetTH?action=GetPhieu&para1=${para1}&para2=${para2}&para3=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        let html = ""
        data.map(x => {
            html += `
                <option  value="${x.MaPhieu}">${x.Display}</option>
            `
        })
        $("#maphieukiemreport").html(html)
        valueSelectReport = data[0].MaPhieu
        //DataReport()

    } catch (error) {
        console.error(error.message);
    }
}
function formatNumberUS(num) {
    const [integer, decimal] = num.toString().split(".");
    const withCommas = integer.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return decimal ? `${withCommas}.${decimal}` : withCommas;
}
function formatDateVN(date) {
    const d = String(date.getDate()).padStart(2, '0');
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const y = date.getFullYear();
    return `${d}/${m}/${y}`;
}



/// Kiệt
let dxGridThuHoiSoLuong;
let dxGridThuHoiVatTuLoi;
let dxLichSuQuetTHSoLuong;
let dxLichSuQuetTHVatTuLoi;
let lstKienQuetSL = [];
let lstKienQuetVT = [];
let lstLichSuQuetTHSoLuong = [];
let lstLichSuQuetTHVatTuLoi = [];
let rowEditSLTH = {};
let rowDeleteData = null;
let rowDeleteIndex = null;
let isScannedFromModal = false;
var tabActive = 'thu-hoi-vat-tu-loi';
var barcodeLocal;
var selectedPhieuThuHoiSoLuong = "";
var selectedPhieuThuHoiVatTuLoi = "";

// Sound
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

/// Event
$(document).ready(function () {
    //PhieuScan();
    GetPhieuDNTH();
    GetViTriO();

    createViewDxGridThuHoiSoLuong([]);
});

function formatDate(date) {
    if (!date) return "";

    const d = date instanceof Date ? date : new Date(date);
    if (isNaN(d.getTime())) return "";

    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();

    return `${day}/${month}/${year}`;
}
async function barcodeInput(value) {
    if (barcodeLocal.trim() != "") {
        await GetCheckBarCode(barcodeLocal, value);
        $("#QRScanSoLuong").val("");
        $("#QRScanVatTuLoi").val("");
    } else {
        showToast("warning", "Vui lòng nhập barcode vào ô QR scan.");
        PlayAudioError();
    }
}

// DxDataGrid
function createViewDxGridThuHoiSoLuong(data) {
    if (data.length > 0)
        data.sort((a, b) => (b.sort || 0) - (a.sort || 0));
    $("#dxGridThuHoiSoLuong").dxDataGrid({
        dataSource: data,
        columnAutoWidth: true,
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
            { dataField: "MaVT", caption: "Item Code", alignment: "center", minWidth: 120 },
            { dataField: "MauVT", caption: "Màu", alignment: "center", minWidth: 100 },
            { dataField: "LOTBATCH", caption: "LOT/BATCH", alignment: "center", minWidth: 120 },
            { dataField: "KhoSize", caption: "Width/Size", alignment: "center", minWidth: 90 },
            { dataField: "DonVi", caption: "Đơn vị", alignment: "center", width: 90 },
            { dataField: "SoKienHienThi", caption: "Số kiện/Roll", alignment: "center", minWidth: 100 },
            {
                dataField: "ViTriO",
                caption: "Vị Trí Ô",
                alignment: "center",
                minWidth: 180,
                cellTemplate: function (container, options) {
                    const $select = $('<select class="form-select form-select-sm"></select>');
                    let html = `<option value="1">Ngoài Ô</option>`;
                    if (dataViTriO.length > 0) {
                        dataViTriO.map(item => {
                            html += `<option data-a="${item.A}" value="${item.TenO}">${item.TenO}</option>`
                        })
                    }
                    $select.html(html);

                    container.append($select);

                    $select.select2({
                        dropdownParent: $("body")
                    });

                    setTimeout(function () {
                        if (options.data.ViTriO) {
                            $select.val(options.data.ViTriO).trigger('change');
                        }
                    }, 0);

                    $select.on('change', function () {
                        options.data.ViTriO = $(this).val();
                    });
                }
            },
            { dataField: "SLXuatVuot", caption: "SL xuất", alignment: "center", minWidth: 100 },
            {
                dataField: "SLXuat",
                caption: "SL T.Hồi",
                alignment: "center",
                minWidth: 140,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control" style="font-size:13px"
                               value="${options.value || ''}" />
                    `);
                    $input.on("input", function () {
                        let value = this.value;

                        // Không cho âm
                        if (Number(value) < 0) {
                            showToast("warning", "Số lượng thu hồi không được âm");
                            this.value = "";
                            options.data.SLXuat = 0;

                        }
                        else if (value.includes(".")) {
                            const [intPart, decPart] = value.split(".");
                            this.value = intPart + "." + decPart.substring(0, 4);
                            value = this.value;
                        }
                        else if (Number(value) > options.data.SLXuatVuot) {
                            showToast("warning", "Số lượng thu hồi không được lớn hơn số lượng xuất hàng");

                            const truncated = Math.trunc(options.data.SLXuatVuot * 10000) / 10000;

                            this.value = truncated;
                            options.data.SLXuat = truncated;
                        }
                        else
                            options.data.SLXuat = Number(value) || 0;

                        sumSLMaNPL()

                    });

                    container.append($input);
                }

            },
            {
                dataField: "Palet",
                caption: "Ghi chú",
                minWidth: 200,
                cellTemplate: function (container, options) {
                    const $input = $(`
                        <input type="text" class="form-control" style="font-size:13px"
                               value="${options.value || ''}" />
                    `);

                    $input.on("input", function () {
                        options.data.Palet = this.value
                    });

                    container.append($input);
                }
            },
            {
                dataField: "CBM",
                caption: "CBM",
                width: 130,
                cssClass: "cbm col-header",
                cellTemplate: function (container, options) {
                    const cbmValue = options.data.CBM != null && !isNaN(options.data.CBM)
                        ? options.data.CBM.toFixed(4) : "";
                    $("<div>")
                        .css({
                            width: "100%",
                            position: "relative",
                            paddingRight: "15px"
                        })
                        .data("rowdata", options.data)
                        .append(
                            $("<input>", {
                                type: "number",
                                readonly: true,
                                value: parseFloat(cbmValue) || "",
                                class: "inputSLN",
                                style: "border:none;outline:none;text-align:center;background:transparent;font-weight:700;width:100%;"
                            })
                        )
                        .append(`<i class="fa-solid fa-pen-to-square editcbm"></i>`)
                        .appendTo(container);
                }
            },
            {
                caption: "",
                width: 40,
                cssClass: "col-header",
                allowFiltering: false,
                allowSorting: false,
                headerCellTemplate: function (container, options) {
                    let $checkAll = $("<input>", {
                        type: "checkbox",
                        id: "checkAllCBMTH",
                        style: "width:18px;height:18px;"
                    });
                    $("<div>")
                        .css({ display: "flex", justifyContent: "center", alignItems: "center" })
                        .append($checkAll)
                        .appendTo(container);
                },
                cellTemplate: function (container, options) {
                    var $checkbox = $("<input>", {
                        type: "checkbox",
                        class: "checkCBM",
                        style: "width:18px;height:18px;",
                        checked: false
                    });
                    $("<div>")
                        .css({ display: "flex", justifyContent: "center", alignItems: "center" })
                        .append($checkbox)
                        .appendTo(container);
                }
            },
            {
                caption: "Xóa",
                alignment: "center",
                minWidth: 60,
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    const item = options.data;
                    const $deleteBtn = $("<i></i>")
                        .addClass("fa-solid fa-trash text-danger")
                        .css({
                            cursor: "pointer",
                            fontSize: "16px"
                        })
                        .attr("title", "Xóa")
                        .on("click", function (e) {
                            e.stopPropagation();


                            showConfirmModalDelete(async function () {
                                dataThuHoi = dataThuHoi.filter(x => x.BarCode != item.BarCode)
                                createViewDxGridThuHoiSoLuong(dataThuHoi)

                                return true; // ✅ cho đóng
                            })

                        });
                    container.append($deleteBtn);

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
                        options.totalValue = { totalKien: 0, totalXH: 0, totalSLTH: 0 };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalKien += 1;
                        options.totalValue.totalXH += options.value.SLXuat || 0;
                        options.totalValue.totalSLTH += parseFloat(options.value.ThuHoi) || 0;
                    }
                    if (options.summaryProcess === "finalize") {
                        const { totalKien, totalXH, totalSLTH } = options.totalValue;
                        options.totalValue = `Tổng Kiện: ${totalKien.toLocaleString()} -- SL Xuất: ${totalXH.toLocaleString()} -- SL Thu Hồi: ${totalSLTH.toLocaleString()}`;
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
        onContentReady: function () {
            sumSLMaNPL()
        }
    })

    $("#Layer_1").click()
}
function showConfirmModalDelete(onConfirm) {
    $('#btnHuyKien').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#modalComfimrtDeleteVT").modal("hide");
    });

    $("#modalComfimrtDeleteVT").modal("show");
}
/// 11032026
let dataViTriO = [];
var checkNhap;
var isExist;
/// API
async function GetViTriO() {
    try {
        const url = `/api/PhieuXuatHangNPL/GetTH?action=GetDanhSachViTriO&para1=${isNPL}`;
        const response = await fetch(url)
        const data = await response.json();

        dataViTriO = data
    } catch (err) {
        console.error(err)
    }
}

async function GetPhieuThuHoiSoLuong() {
    try {
        const phieuDNTH = $("#maphieukiem").val();
        const url = `/api/PhieuXuatHangNPL/GetTH?action=GetPhieuThuHoi&para=&para2=&para3=${isNPL}&para4=${phieuDNTH}`;


        const response = await fetch(url)
        const data = await response.json();
        sort = data[0].PTH
        isExist = data[0].IsExists
        const displayPhieu = `PTH_${data[0].PTH}`;
        $("#maPhieuThuHoi").val(displayPhieu)
        selectedPhieuThuHoiSoLuong = displayPhieu

    } catch (err) {
        console.error(err)
    }
}

async function GetPhieuDNTH() {
    try {
        const url = `/api/PhieuXuatHangNPL/GetTH?action=GetPhieuDeNghiTH`;
        const selectPhieuDNTH = $('#maphieukiem')
        selectPhieuDNTH.empty();

        const response = await fetch(url)
        const data = await response.json();

        renderTablePieuDNTH(data)

        renderTableItemCode([])
    } catch (err) {
        console.error(err)
    }
}
/// EVENT
$(function () {
    $(".btnNhapCBM").on("click", function () {
        const $checkedBoxes = $("#dxGridThuHoiSoLuong").find(".checkCBM:checked");
        if ($checkedBoxes.length === 0) {
            showToast("warning", "Vui lòng chọn ít nhất một ô cần nhập CBM !");
            return;
        }
        $('#cbmResult').text(0.00);
        $(".cell-rect .cbm-input").val("")
        $(".cell-cylinder .cbm-input").val("")
        $("#cbmModal").modal("show");
    })
})


let currentCBMRowData = null;

function calculateCBM() {
    const type = $('input[name="typeCBM"]:checked').val();
    let cbm = 0;

    if (type === 'rect') {
        const l = parseFloat($('.input-length').val()) || 0;
        const w = parseFloat($('.input-width').val()) || 0;
        const h = parseFloat($('.input-height').val()) || 0;
        cbm = l * w * h;
    } else {
        const length = parseFloat($('.input-cylinder-length').val()) || 0;
        const diameter = parseFloat($('.input-cylinder-diameter').val()) || 0;
        const radius = diameter / 2;
        cbm = Math.PI * Math.pow(radius, 2) * length;
    }

    $('#cbmResult').text(cbm.toFixed(4));
    return cbm;
}


$(function () {
    $('input[name="typeCBM"]').on('change', function () {
        if (this.value === 'rect') {
            $('.cell-rect').removeClass('d-none');
            $('#headerRect').removeClass('d-none');
            $('.cell-cylinder').addClass('d-none');
            $('#headerCylinder').addClass('d-none');
        } else {
            $('.cell-rect').addClass('d-none');
            $('#headerRect').addClass('d-none');
            $('.cell-cylinder').removeClass('d-none');
            $('#headerCylinder').removeClass('d-none');
        }
        $('#cbmResult').text(0.0);
        $(".cell-rect .cbm-input").val("")
        $(".cell-cylinder .cbm-input").val("")
    });

    $(document).on('input', '.cbm-input', calculateCBM);

    $(document).on("click", ".editcbm", function () {
        $(".cbm-input").val("");
        $('#cbmResult').text('0.00');

        currentCBMRowData = $(this).closest("div").data("rowdata");

        if (currentCBMRowData) {
            const isCBM = currentCBMRowData.IsCBM;
            const dai = currentCBMRowData.Dai;
            const rong = currentCBMRowData.Rong;
            const cao = currentCBMRowData.Cao;
            const dk = currentCBMRowData.DK;
            const cbm = currentCBMRowData.CBM;

            if (isCBM == 2) {
                $(`input[name="typeCBM"][value="cylinder"]`).trigger("click");
                $(".input-cylinder-length").val(dai);
                $(".input-cylinder-diameter").val(dk);
            } else {
                $(`input[name="typeCBM"][value="rect"]`).trigger("click");
                $(".input-length").val(dai);
                $(".input-width").val(rong);
                $(".input-height").val(cao);
            }

            if (cbm) {
                $('#cbmResult').text(parseFloat(cbm).toFixed(4));
            }
        }
        $(this).closest("tr").find(".checkCBM").prop("checked", true);
        $("#cbmModal").modal("show");
    });

    $("#btnSaveCBMTH").on("click", async function () {

        const cbmValue = calculateCBM();
        const type = $('input[name="typeCBM"]:checked').val();

        const $checkedBoxes = $("#dxGridThuHoiSoLuong").find(".checkCBM:checked");

        // Nếu không có checkbox nào được check VÀ không có currentCBMRowData thì return
        if ($checkedBoxes.length === 0 && !currentCBMRowData) return;


        let cbm = 0, dai, rong, cao, dk;

        if (type === 'rect') {
            dai = parseFloat($('.input-length').val()) || 0;
            rong = parseFloat($('.input-width').val()) || 0;
            cao = parseFloat($('.input-height').val()) || 0;
            dk = 0;
            cbm = dai * rong * cao;
        } else {
            dai = parseFloat($('.input-cylinder-length').val()) || 0;
            dk = parseFloat($('.input-cylinder-diameter').val()) || 0;
            rong = 0; cao = 0;
            cbm = Math.PI * Math.pow(dk / 2, 2) * dai;
        }

        const arrSave = [];

        $checkedBoxes.each(function () {
            const $row = $(this).closest("tr");
            const rowData = $row.find(".editcbm").closest("div").data("rowdata");

            if (rowData) {
                rowData.CBM = cbm;
                rowData.Dai = dai;
                rowData.Rong = rong;
                rowData.Cao = cao;
                rowData.DK = dk;
                rowData.IsCBM = type === 'rect' ? 1 : 2;

                arrSave.push({
                    ID: 0,
                    SoLoID: rowData.SoLoID ?? 0,
                    MaNPL: rowData.MaNPL ?? 0,
                    MaVTGhep: rowData.MaVTGhep ?? 0,
                    BarCode: rowData.BarCode,
                    Dai: dai,
                    Rong: rong,
                    Cao: cao,
                    DK: dk,
                    CBM: cbm,
                    IsNPL: isNPL,
                    IsCBM: type === 'rect' ? 1 : 2,
                    MaONPL: "",
                });
            }
        });
        await SaveCBM(arrSave)
    });
});

async function SaveCBM(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    const request = new Request(`/api/ERPVatTuCBM/Post?action=PostCBM&para1=${dataUser}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(arrSave),
    });

    let response = await fetch(request);
    let data = await response.json();

    if (data == "True") {
        arrSave.forEach(x => {
            const objectFind = dataThuHoi.find(t => t.BarCode == x.BarCode)
            if (objectFind) return
            objectFind.MaVTGhep = x.MaVTGhep
            objectFind.SoLoID = x.SoLoID
            objectFind.Dai = x.Dai
            objectFind.Rong = x.Rong
            objectFind.Cao = x.Cao
            objectFind.DK = x.DK
            objectFind.CBM = x.CBM
            objectFind.IsCBM = x.IsCBM
        })
        showToast('success', `Lưu CBM thành công!`, 1500);
        $('#cbmModal').modal('hide');
        $('#cbmResult').text(0.00);
        $(".cell-rect .cbm-input").val("");
        $(".cell-cylinder .cbm-input").val("");
        createViewDxGridThuHoiSoLuong(dataThuHoi)

    } else {
        showToast('error', `Lưu CBM thất bại!`, 2000);
    }
}

async function SaveONPL(arrSave) {
    console.log("arrSave SaveONPL: ", arrSave)
    var dataUser = !window.CefSharp ? userNameSave : userName
    try {
        const request = new Request(`/api/ERPVatTuCBM/Post?action=PostMaONPL&para1=${dataUser}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(arrSave),
        });
        await fetch(request)
    } catch (err) {
        console.error(err)
    }
}

$(document).on("click", "#checkAllCBMTH", function () {
    const checker = $(this).is(":checked");
    $("#dxGridThuHoiSoLuong").find(".checkCBM").prop("checked", checker);
});

function renderTableItemCode(data) {
    dataItemCode = data
    $("#tbodyXHItemCode").dxDataGrid({
        dataSource: data,
        columnAutoWidth: true,
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

            { dataField: "MaVT", caption: "ItemCode", cssClass: "col-header", minWidth: 100 },
            { dataField: "Status", caption: `Trạng thái`, cssClass: "col-header", width: 80 },
            {
                dataField: "SLDK", caption: "SL đăng ký", cssClass: "col-header", width: 80,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.SLDK || 0).toFixed(2));
                },
            },
            {
                dataField: "SLTH",
                caption: "SL T.hồi",
                cssClass: "col-header slthuhoi",
                width: 80,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.SLTH || 0).toFixed(2));
                }
            },
            { dataField: "MauVT", caption: "Màu", cssClass: "col-header", minWidth: 100 },
            { dataField: "KhoVai", caption: "Width/size", cssClass: "col-header", minWidth: 100 },
            {
                dataField: "TenDVVT", caption: "Đơn vị", cssClass: "col-header", minWidth: 100,
            }

        ],
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-manpl", e.data.MaNPL);
                e.rowElement.attr("data-thuhoi", e.data.SLTH);
            }
        }

    });
}
async function GetDanhSachItemCode() {
    var url = `/api/DangKyVatTu/Get?Action=GetDanhSachItemThuHoi&para1=${selectedRowPhieuDNTH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        var data = await response.json();
        dataThuHoi = []
        renderTableItemCode(data)
        await GetPhieuThuHoiSoLuong();
    } catch (error) {
        console.error(error.message);
    }
}
function createViewDxGridDetailsContainer(data) {

    $("#dxLichSuQuet").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
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
                dataField: "POMua",
                caption: "PO Mua",
                width: 100,
            },
            {
                caption: "LOT/Batch",
                dataField: "SoLotBatch",
                width: 120,
                calculateCellValue: function (row) {
                    return `${row.SoLot ?? ""}/${row.Batch ?? ""}`;
                },
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                width: 90,
            },
            {
                dataField: "SoKienHienThi",
                caption: "Vật tư",
                minWidth: 80,
                width: 140,
            },

            {
                dataField: "SLXuat",
                caption: "SL xuất",
                width: 80,
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
    });

}
function getBarCodeScan(result, itembarcode) {
    const itemScan = result.find(x => x.BarCode == itembarcode)
    setTextAndTitle("#txtPoMua", itemScan.POMua);
    setTextAndTitle("#txtLotBatch", `${itemScan.LOTBATCH}`);
    setTextAndTitle("#txtItemCode", itemScan.MaVT);
    setTextAndTitle("#txtRoll", itemScan.SoKienHienThi);
    setTextAndTitle("#txtSLThuHoi", itemScan.SLXuat);

    createViewDxGridDetailsContainer(result)
}

function setTextAndTitle(selector, value) {
    $(selector).text(value ?? "").attr("title", value ?? "");
}
var dataThuHoi = []
var dataItemCode = []
var historyScanBarcode = [];
async function GetCheckBarCode(barcode, value) {
    const urlCheckBarcode = `/api/PhieuXuatHangNPL/GetTH?action=CheckBarCodeExists&para1=${barcode}&para2=${isNPL}&para3=${barcode}`;
    const urlGetDanhSachTH = `/api/PhieuXuatHangNPL/GetTH?action=GetDanhSachTH&para1=${barcode}&para2=${isNPL}`;
    // Lấy grid tương ứng với tab hiện tại
    try {
        const responseDSTH = await fetch(urlGetDanhSachTH);
        const data = await responseDSTH.json();

        if (data.length === 0) {
            const response = await fetch(urlCheckBarcode);
            const result = await response.json();

            if (result.length === 0) {
                showToast("warning", "Barcode không tồn tại");
            } else {
                showToast("warning", "Vật tư chưa được xuất hàng");
            }
            PlayAudioError();
            return;
        } else if (data[0].PhieuTH != "") {
            showToast("warning", `Barcode đã được thu hồi tại ${data[0].PhieuTH}!`);
            PlayAudioError();
            return
        }
        var d = data[0];

        //dataThuHoi = resultDSTH[0];

        if (d.CheckKiemKe == 1) {
            showToast("warning", `Vật tư đang trong quá trình kiểm kê không được xuất hàng!`);
            PlayAudioError();
            return
        }
        const existingItem = dataThuHoi.find(item => item.BarCode == d.BarCode);

        if (existingItem) {
            showToast("warning", "BarCode đã tồn tại trong danh sách thu hồi!");
            PlayAudioError();
            return;
        }
        let $tr = $(`#tbodyXHItemCode tr[data-manpl="${d.MaNPL}"]`);
        const total = parseFloat($tr.find("td.slthuhoi").text().trim() || 0)
        const itemPCBarCode = dataItemCode.find(x => x.MaNPL == d.MaNPL)
        if (!itemPCBarCode) {
            showToast("warning", "BarCode không nằm trong danh sách vật tư phiếu cấp!");
            PlayAudioError();
            return
        }
        let html = `Thu hồi vật tư <span style="color:Red" class="">${itemPCBarCode.MaVT} / ${d.ChiTiet}</span> đã vượt số lượng thu hồi trong phiếu đề nghị.Bạn có muốn cho phép vượt không! `
        if (value == 1) {
            historyScanBarcode.push(barcode)
        }
        if (total + d.SLXuat > itemPCBarCode.SLDK && itemPCBarCode.isCheckVuot == 0) {
            $(".textVuot").html(html)
            showConfirmModal(async function () {
                dataThuHoi.forEach(item => item.sort = 1);

                var d = data[0];
                d.sort = 2;
                d.isCheckVuot = 1

                dataThuHoi.push(d);
                PlayAudio();

                createViewDxGridThuHoiSoLuong(dataThuHoi);
                itemPCBarCode.isCheckVuot = 1
                if (value == 1) {
                    const result = dataThuHoi.filter(item => historyScanBarcode.includes(item.BarCode));
                    getBarCodeScan(result, barcode)
                }
            });
        } else {
            dataThuHoi.forEach(item => item.sort = 1);

            var d = data[0];
            d.sort = 2;
            d.isCheckVuot = itemPCBarCode.isCheckVuot

            dataThuHoi.push(d);
            PlayAudio();
            $(".kienquet").val(d.SoKienHienThi);
            $(".thucnhap").val(d.SLNhap);

            createViewDxGridThuHoiSoLuong(dataThuHoi);
            if (value == 1) {
                const result = dataThuHoi.filter(item => historyScanBarcode.includes(item.BarCode));
                getBarCodeScan(result, barcode)
            }
        }
    } catch (error) {
        console.error(error.message);
    }
}

function showConfirmModal(onConfirm) {

    $('#btnComfirm').off('click');
    // Khi nhấn Đồng ý
    $('#btnComfirm').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        const modalEl = document.getElementById('myModalV');
        const modal = bootstrap.Modal.getInstance(modalEl);
        modal.hide();
    });

    // Hiện modal
    const modal = new bootstrap.Modal(document.getElementById('myModalV'));
    modal.show();
}


/// Kiệt - 20260320
let selectedRowMaLenh;
let selectedRowPhieuDNTH;
let selectedRowIC;
/// Event
$(document).ready(function () {

    const firstDayOfMonth = moment().startOf('month').toDate();
    const today = moment().toDate();


    $(".select_loc").select2({
        minimumResultsForSearch: Infinity
    });
    $(".selectView").select2({
        minimumResultsForSearch: Infinity
    });

    $("#locPhieuCap").select2({
        dropdownParent: "#modalPhieuCap",
        minimumResultsForSearch: Infinity
    })
    createViewDxDataGridPhieuCap1([]);
    createViewDxDataGridPhieuCap2([]);
    createViewDxDataGridPhieuCap3([]);
    createViewDxDataGridPhieuCap4([]);
    renderTablePieuDNTH([]);

    const picker3 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker3'), {
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

    const picker4 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker4'), {
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

    // set giá trị
    picker3.dates.setValue(new tempusDominus.DateTime(firstDayOfMonth));
    picker4.dates.setValue(new tempusDominus.DateTime(today));

    $(".dateInput").trigger("click");
    picker3.hide();
    picker4.hide();

})

$(function () {
    $("#locPhieuCap").on("change", async function () {
        const valueLoc = $(this).val();
        $("#itemCodeInput").val('');
        batchDataMalenh = [];
        itemCodeJoin = [];
        $("#itemList").empty();
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])

        if (valueLoc == 1) {
            $(".containerGrid1").removeClass("d-none")
            $(".containerGrid2").addClass("d-none")

            $(".colTuNgayPC").addClass("d-none")
            $(".colDenNgayPC").addClass("d-none")

            $(".colCBItemCode").addClass("d-none")
            GetMaLenhChuaCap();
        } else {
            $(".containerGrid1").addClass("d-none")
            $(".containerGrid2").removeClass("d-none")

            $(".colTuNgayPC").removeClass("d-none")
            $(".colDenNgayPC").removeClass("d-none")

            $(".colCBItemCode").removeClass("d-none")

            GetICDaCap();
        }
    })

    $("#modalPhieuCap").on("show.bs.modal", function () {
        GetICDaCap();
        GetMaLenhChuaCap();
    })



    $("#tuNgayPhieuYC").on("change", async function () {
        $("#ItemCodeInput").val('');
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])
        await GetICDaCap();

    })

    $("#denNgayPhieuYC").on("change", async function () {
        $("#ItemCodeInput").val('');
        createViewDxDataGridPhieuCap1([])
        createViewDxDataGridPhieuCap2([])
        await GetICDaCap();
    })

})

/// Function
function showLSTH() {
    $("#modalPhieuCap").modal("show");
}

/// Helper Function
function formatNumber(value, decimals = -1) {
    if (value === undefined || value === null || value === "") return "";

    let num = parseFloat(value.toString().replace(/,/g, ''));
    if (isNaN(num)) return "";

    if (decimals >= 0) {
        num = parseFloat(num.toFixed(decimals));
        return num.toLocaleString(undefined, {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        });
    }

    let str = num.toString();
    let intPart = str;
    let decPart = "";
    if (str.includes(".")) {
        [intPart, decPart] = str.split(".");
        decPart = decPart.substring(0, 4);
    }
    let formattedInt = Number(intPart).toLocaleString();
    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
}

/// API
async function GetMaLenhChuaCap() {
    try {
        const url = `/api/PhieuXuatHangNPL/GetTH?action=GetChuaThuHoi&para1=${isNPL}`;
        const response = await fetch(url)
        const data = await response.json();

        createViewDxDataGridPhieuCap4(data)
    } catch (err) {
        console.error(err)
    }
}


async function GetChiTietLenhChuaCap() {

    try {
        var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetChiTietLenhChuaThuHoi&para1=${selectedRowMaLenh}&para2=${isNPL}`;
        const response = await fetch(url);
        const data = await response.json();

        createViewDxDataGridPhieuCap3(data)
    } catch (err) {
        console.error(err)
    }
}

async function GetICDaCap() {
    try {
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetICDaThuHoi&para1=${isNPL}&para2=${tuNgay}&para3=${denNgay}`;
        const response = await fetch(url);
        const data = await response.json();

        createViewDxDataGridPhieuCap1(data)
    } catch (err) {
        console.error(err)
    }
}


async function GetChiTietICDaCap() {
    try {
        console.log(selectedRowIC)
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        var url = `/api/PhieuXuatHangNPL/GetTH?Action=GetChiTietICDaCap&para1=${isNPL}&para2=${tuNgay}&para3=${denNgay}&para4=${selectedRowIC}`;
        const response = await fetch(url);
        const data = await response.json();

        createViewDxDataGridPhieuCap2(data)
    } catch (err) {
        console.error(err)
    }
}

/// DxDataGrid
function createViewDxDataGridPhieuCap1(data) {
    dxPhieuCap1 = $("#dxPhieuCap1").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
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
        selection: {
            mode: 'single',
            showCheckBoxesmode: "onclick",
            allowSelectAll: false,
        },

        columns: [

            {
                caption: "ItemCode",
                minWidth: 140,
                cellTemplate: function (container, options) {
                    container.text(`${options.data.MaVT}-${options.data.MauVT}`)
                }
            },
            {
                dataField: "SLDK",
                caption: "SL Đăng Ký",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            {
                dataField: "SLTH",
                caption: "SL Thu Hồi",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            {
                dataField: "NgayTH",
                caption: "Ngày Thu Hồi",
                minWidth: 120
            },

        ],
        onContentReady: function (e) {
            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === 'data');
                if (firstDataRow) {
                    const rowIndex = firstDataRow.rowIndex;
                    selectedRowIC = firstDataRow.data.MaNPL;
                    GetChiTietICDaCap()
                    // Highlight row đầu tiên
                    setTimeout(() => {
                        const $rowElement = $(e.component.getRowElement(rowIndex));
                        e.component.element().find(".dx-row").removeClass("activeT");
                        $rowElement.addClass("activeT");
                    }, 50);
                }
            }
        },
        onRowClick: function (e) {
            selectedRowIC = e.data.MaNPL;
            GetChiTietICDaCap()
            // highlight row
            $(e.rowElement).closest(".dx-datagrid-rowsview")
                .find(".dx-row")
                .removeClass("activeT");
            $(e.rowElement).addClass("activeT");
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

    }).dxDataGrid('instance');
}
function createViewDxDataGridPhieuCap2(data) {
    dxPhieuCap2 = $("#dxPhieuCap2").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
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
            { dataField: "PhieuTH", groupIndex: 0, caption: "Phiếu Thu Hồi", width: 90, },
            { dataField: "POMua", caption: "PO Mua", width: 100, },
            { dataField: "SoLo", caption: "Số Lô", minWidth: 100, },
            { dataField: "MaVT", caption: "Item Code", width: 120, },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "SoKienHienThi", caption: "Số Roll", minWidth: 100, },
            {
                dataField: "ThuHoi",
                caption: "Thu Hồi",
                minWidth: 140,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            { dataField: "NgayTH", caption: "Ngày Thu hồi", minWidth: 100, },
            {
                caption: "Thao Tác",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const isMaONPL = options.data.MaONPL
                    const $delete =
                        $(`<button class="btn p-0" style="font-size:10px"><i class="fa-solid fa-trash" style="font-size: 12px; color: red"></i></button`)
                            .addClass(isMaONPL ? "d-none" : "")
                            .on("click", function () {
                                showConfirmModalDeleteLichSu(function () {
                                    const phieuTH = options.data.PhieuTH;
                                    const barcode = options.data.BarCode;
                                    ApiDeleteLichSu(phieuTH, barcode)
                                })
                            })

                    container.append($delete)
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
            }

        },
        summary: {
            groupItems: [
                {
                    column: "ThuHoi", summaryType: "sum", showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        if (!e.value) return '';
                        return formatNumber(e.value, 2);

                    }
                },
            ]
        }
    }).dxDataGrid('instance');
}
function createViewDxDataGridPhieuCap3(data) {
    dxPhieuCap3 = $("#dxPhieuCap3").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
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
            { dataField: "PhieuTH", caption: "Phiếu Thu Hồi", groupIndex: 0, visible: false },
            { dataField: "NgayTH", caption: "Ngày ĐK TH", minWidth: 120 },
            { dataField: "MaVT", caption: "Item Code", minWidth: 120, },
            //{
            //    dataField: "CapPhat",
            //    caption: "Cấp Phát",
            //    minWidth: 100,
            //    cellTemplate: function (container, options) {
            //        container.text(formatNumber(options.value))
            //    }
            //},
            {
                dataField: "SLDK",
                caption: "SL Đăng Ký",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
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
    }).dxDataGrid('instance');

}
function createViewDxDataGridPhieuCap4(data) {
    dxPhieuCap4 = $("#dxPhieuCap4").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
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
        selection: {
            mode: 'single',
            showCheckBoxesmode: "onclick",
            allowSelectAll: false,
        },

        columns: [
            { dataField: "MaLenh", caption: "Mã Lệnh", width: 90, },
            { dataField: "MaHang", caption: "Tên Hàng", width: 230, },
        ],
        onContentReady: function (e) {
            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === 'data');
                if (firstDataRow) {
                    const rowIndex = firstDataRow.rowIndex;
                    selectedRowMaLenh = firstDataRow.data.MaLenhSanXuat;
                    GetChiTietLenhChuaCap();
                    // Highlight row đầu tiên
                    setTimeout(() => {
                        const $rowElement = $(e.component.getRowElement(rowIndex));
                        e.component.element().find(".dx-row").removeClass("activeT");
                        $rowElement.addClass("activeT");
                    }, 50);
                }
            }
        },
        onRowClick: function (e) {
            selectedRowMaLenh = e.data.MaLenhSanXuat;
            GetChiTietLenhChuaCap();

            // highlight row
            $(e.rowElement).closest(".dx-datagrid-rowsview")
                .find(".dx-row")
                .removeClass("activeT");
            $(e.rowElement).addClass("activeT");
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
    }).dxDataGrid('instance');

}

function renderTablePieuDNTH(data) {
    $("#tbodyPhieuDNTH").dxDataGrid({
        dataSource: data,
        columnAutoWidth: true,
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
        selection: {
            mode: 'single',
            showCheckBoxesmode: "onclick",
            allowSelectAll: false,
        },
        columns: [

            { dataField: "PhieuTH", caption: "PĐN", cssClass: "col-header", minWidth: 100 },
            { dataField: "MaLenh", caption: `Mã Lệnh`, cssClass: "col-header", minWidth: 120 },
            { dataField: "MaHang", caption: "Tên Hàng", cssClass: "col-header", minWidth: 100 },
        ],
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-manpl", e.data.MaNPL);
                e.rowElement.attr("data-thuhoi", e.data.SLTH);
            }
        },
        onContentReady: function (e) {
            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === 'data');
                if (firstDataRow) {
                    const rowIndex = firstDataRow.rowIndex;
                    selectedRowPhieuDNTH = firstDataRow.data.PhieuTH;
                    GetDanhSachItemCode()
                    // Highlight row đầu tiên
                    setTimeout(() => {
                        const $rowElement = $(e.component.getRowElement(rowIndex));
                        e.component.element().find(".dx-row").removeClass("activeT");
                        $rowElement.addClass("activeT");
                    }, 50);
                }
            }
        },
        onRowClick: function (e) {
            selectedRowPhieuDNTH = e.data.PhieuTH;
            GetDanhSachItemCode();
            dataThuHoi = []
            createViewDxGridThuHoiSoLuong([])
            // highlight row
            $(e.rowElement).closest(".dx-datagrid-rowsview")
                .find(".dx-row")
                .removeClass("activeT");
            $(e.rowElement).addClass("activeT");
        },

    });
}
function sumSLMaNPL() {
    let sumByNPL = {};
    // Tính tổng theo NPL
    dataThuHoi.forEach(x => {
        let npl = x.MaNPL;
        let val = x.SLXuat;
        if (!sumByNPL[npl]) sumByNPL[npl] = 0;
        sumByNPL[npl] += val;
    })
    // Cập nhật lại SL xuất trong dxDataGrid
    $("#tbodyXHItemCode tr").each(function () {
        const trNPL = $(this).data("manpl");
        if (trNPL == null) return
        if (sumByNPL[trNPL] !== undefined) {
            const slGoc = parseFloat($(this).data("thuhoi") || 0);  // SLXuất gốc từ data attr
            const tongMoi = slGoc + sumByNPL[trNPL];
            $(this).find("td.slthuhoi").text(tongMoi);
        }
    });

}