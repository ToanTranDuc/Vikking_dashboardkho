/// VARIABLE
var userNameSave = localStorage.getItem("username1")
let dxDataGridKiemKe;
let dxDataPhieuDKVT;
let checkIndex = 0;
let checkIndexNguoiGhiNhan = 0;
let lstDSPhieuDKVT = [];
// ============= SIGNATURE CANVAS CODE =============

var imageSign, idNguoiKy, scrollPosition;

async function UpdateKT(image) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser
    var ArrPhieuXHKT = []
    const phieKiemKe = $("#phieuKiemKe").val()
    if (phieKiemKe == "" || phieKiemKe == null) {
        showToast("warning", "Vui lòng chọn phiếu đăng ký")
        return
    }
    const phieuXuatHang = {
        PhieuVatTuKK: phieKiemKe,
        MaNPL: '',
        MaVT: '',
        MauVT: '',
        KhoVai: '',
        MaDVVT: '',
        NguoiTao: dataUser,
        IsLog: '',
        IsNPL: isNPL,
        GhiChu: image,
        Dot: '',
        TonDKy: '',
        TonCKy: '',
        TuNgay: '',
        DenNgay: '',
        SLNhapKho: '',
        ChiTiet: ''
    };
    ArrPhieuXHKT.push(phieuXuatHang)
    await SaveKyTenKiemKe(ArrPhieuXHKT)
}
function saveSignature() {
    if (signatureHistory.length === 0) {
        showToast("warning", 'Vui lòng ký tên trước khi lưu!');
        return;
    }
    $('#signatureModal').modal('hide');
    // Chuyển canvas thành hình ảnh
    const signatureImage = canvas.toDataURL('image/png');
    UpdateKT(signatureImage)
}

function CallModal() {
    $('#signatureModal').modal('show');
}
/// HELPER FUNCTIONS
function updateGrid(grid, dataSource) {
    checkIndex = 0;
    checkIndexNguoiGhiNhan = 0;
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}



/// EVENT
$(document).ready(function () {
    createViewDxDataGridKiemKe([]);
})



/// DxDataGrid
function createViewDxDataGridKiemKe(data) {
    dxDataGridKiemKe = $("#dxDataGridKiemKe").dxDataGrid({
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
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "PhieuVTKK",
                caption: "Phiếu Kiểm Kê",
                minWidth: 100,
                calculateCellValue: function (rowData) {
                    if (!rowData) return "";
                    return `${rowData.PhieuVatTuKK}`;
                }
            },
            {
                dataField: "MaVT",
                caption: "ItemCode",
                minWidth: 100,
            },
            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                width: 500,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                minWidth: 80,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                minWidth: 80,
            },
            {
                dataField: "MaDVVT",
                caption: "Đơn Vị",
                minWidth: 80,
            },
            {
                dataField: "SLTonKho",
                caption: "SL Tồn Kho",
                minWidth: 80,
            },
            {
                dataField: "KyTen",
                caption: "Ký tên",
                minWidth: 120,
                cellTemplate: function (container, options) {

                    if (!options.data) return;

                    const grid = options.component;
                    const dataSource = grid.option("dataSource") || [];

                    const valueCheck = options.data.PhieuVatTuKK;
                    const valueCheckKyTen = options.data.KyTen;

                    const rowIndexInData = dataSource.findIndex(
                        d => d.PhieuVatTuKK === valueCheck && d.KyTen == valueCheckKyTen
                    );

                    let rowspan = 0;

                    if (checkIndex === 0) {
                        for (let i = rowIndexInData; i < dataSource.length; i++) {
                            if (dataSource[i].PhieuVatTuKK === valueCheck && dataSource[i].KyTen == valueCheckKyTen) {
                                rowspan++;
                            } else {
                                break;
                            }
                        }
                    }

                    if (checkIndex === 0) {

                        const containerDiv = $("<div>").css({
                            width: "100%",
                            height: "40px",
                            position: "relative",
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        });

                        // ảnh ký tên
                        if (options.data.KyTen) {
                            $("<img>")
                                .attr(
                                    "src",
                                    `/Images/SignDeNghiThuHoi/${options.data.KyTen}?${Date.now()}`
                                )
                                .css({ width: "70px", height: "35px" })
                                .appendTo(containerDiv);
                        }

                        containerDiv.appendTo(container);
                        container.attr("rowspan", rowspan);
                        checkIndex = rowspan;

                    } else {
                        container.addClass("d-none");
                    }

                    checkIndex--;
                }
            },


        ],
        summary: {
            totalItems: [
                {
                    name: "tongKho",
                    summaryType: "custom",
                    showInColumn: "ChiTiet",
                },
                {
                    column: "SLDK",
                    summaryType: "max",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return Math.trunc(Number(e.value) * 10000) / 10000;
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                }
            ],

            // ✅ SUMMARY THEO GROUP
            groupItems: [
                {
                    column: "SLDK",
                    summaryType: "max",
                    showInGroupFooter: true,
                    alignByColumn: true,
                    customizeText(e) {
                        return (
                            Math.trunc(Number(e.value || 0) * 10000) / 10000
                        );
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    showInGroupFooter: true,
                    alignByColumn: true,
                    customizeText(e) {
                        return formatNumber(e.value)
                    }
                }
            ],

            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {
                    if (options.summaryProcess === "start") {
                        options.totalValue = { totalRoll: 0 };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalRoll += 1;
                    }
                    if (options.summaryProcess === "finalize") {
                        options.totalValue =
                            `Tổng số roll/số kiện: ${options.totalValue.totalRoll.toLocaleString()}`;
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
            Object.values(mergeState).forEach(s => s.value = 0);
        }
    }).dxDataGrid("instance");
    $("#Layer_1").click()
}
function mergeCell(options, container, state, renderContent) {
    if (!options.data) return;

    const grid = options.component;
    const dataSource = grid.option("dataSource") || [];

    const key1 = options.data.PhieuXH;
    const key2 = options.data.MaNPL;

    const rowIndex = dataSource.findIndex(
        d => d.PhieuXH === key1 && d.MaNPL == key2
    );

    let rowspan = 0;

    if (state.value === 0) {
        for (let i = rowIndex; i < dataSource.length; i++) {
            if (
                dataSource[i].PhieuXH === key1 &&
                dataSource[i].MaNPL == key2
            ) {
                rowspan++;
            } else {
                break;
            }
        }
    }

    if (state.value === 0) {
        renderContent(container, options);
        container.attr("rowspan", rowspan);
        state.value = rowspan;
    } else {
        container.addClass("d-none");
    }

    state.value--;
}
const mergeState = {
    SLDK: { value: 0 },
    NguoiKT: { value: 0 },
    KyTen: { value: 0 }
};
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


/// ============= Xác Nhận Kiểm Kê NL - Kiệt - 20260330 =============
/// EVENT
$(document).ready(function () {
    $("#home").on("click", () => window.location.href = '/Home/Dashboard');
    $(".select_2").select2();
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
        defaultDate: getDayNow()
    });

    // set giá trị
    picker1.dates.setValue(new tempusDominus.DateTime(getFirstDayOfCurrentMonth()));
    picker2.dates.setValue(new tempusDominus.DateTime(getDayNow()));

    $(".dateInput").trigger("click");
    picker1.hide();
    picker2.hide();

    LoadPhieuKiem()
})

$(function () {

    $("#locTrangThai").on("change", function () {
        LoadPhieuKiem()
    })
    $("#dateInput").on("change", function () {
        LoadPhieuKiem()
    })
    $("#dateInput2").on("change", function () {
        LoadPhieuKiem()
    })
})

/// HELPER FUNCTIONS
function getFirstDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
}

function getDayNow() {
    const now = new Date();
    return new Date(now);
}

function ddmmyyyyToYmd(dateStr) {
    if (!dateStr) return "1990-01-01";
    const [d, m, y] = dateStr.split("/");
    return `${y}-${m}-${d}`;
}

/// API
async function LoadPhieuKiem() {
    try {
        const tuNgay = ddmmyyyyToYmd($("#dateInput").val());
        const denNgay = ddmmyyyyToYmd($("#dateInput2").val());
        const locTrangThai = $("#locTrangThai").val();

        const url = `/api/DangkyVatTu/Get?Action=GetDanhSachPhieuKiemKe&Para1=${tuNgay}&Para2=${denNgay}&Para3=${locTrangThai}&Para4=${isNPL}`

        const response = await fetch(url);

        const data = await response.json();

        let html = ``;
        data.map(x => {
            html += `<option value="${x.PhieuVatTuKK}">${x.DisPlay}</option> `
        })

        $("#phieuKiemKe").html(html)

        LoadDanhSachKK()

    } catch (err) {
        console.error(err)
    }
}

async function LoadDanhSachKK() {
    try {
        const phieuKiemKe = $("#phieuKiemKe").val();
        const url = `/api/DangkyVatTu/Get?Action=GetPhieuKiemKeXN&Para1=${phieuKiemKe}&Para2=${isNPL}`
        const response = await fetch(url);
        const data = await response.json();

        createViewDxDataGridKiemKe(data)

    } catch (err) {
        console.error(err)
    }
}

async function SaveKyTenKiemKe(arrSave) {
    try {
        const url = `/api/DangkyVatTu/PostXNKK?Action=XacNhanKyTenKiemKe`
        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(arrSave),
        });

        const result = await response.json();
        if (result == "True") {
            showToast("success", "Lưu thành công !")
        }
        LoadDanhSachKK()
    } catch (err) {
        console.error(err)
    }
}


// module = M.12.00.01 
//title xác nhận phiếu kiểm kê 
//detail //Maha, vat ""
//sendTo ERP_PhongBan ->  KHo 
//BoPhan ERP_BoPhan ->  KhoPL or KhoNL  or TBP = All
//Status 1 Truong BP  Ha, 2 -> to truong Can 
function sendNotify(ModuleID, title, detail, sendTo, BoPhan = "ALL", Status = -1) {

    const url = `/api/SendToNotification/PushNotification?` +
        `UserIDTao=${encodeURIComponent(UserID)}&` +
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