let arrTieuChiDGia = [];
let arrChungLoaiVT = [];
let arrDiemDanhGia = [];
let arrNhaCungCap = [];

let TenPhieu = "";
let LoaiDanhGia = "DanhGia";

let rowSelected_CLVT = null;

const userName = localStorage.getItem("username1");

function formatDateView(isoDate) {
    if (!isoDate) return "";

    let timestamp = Date.parse(isoDate);
    if (isNaN(timestamp)) return isoDate;

    let date = new Date(timestamp);
    let day = String(date.getDate()).padStart(2, '0');
    let month = String(date.getMonth() + 1).padStart(2, '0');
    let year = date.getFullYear();

    return `${day}-${month}-${year}`;
}

function formatTime(isoTimeString) {
    if (!isoTimeString) return "";

    let date = new Date(isoTimeString);
    if (isNaN(date.getTime())) return ""; // Kiểm tra nếu không phải ngày hợp lệ

    // Lấy giờ, phút, giây, đảm bảo luôn có 2 chữ số
    let hours = String(date.getHours()).padStart(2, '0');
    let minutes = String(date.getMinutes()).padStart(2, '0');
    let seconds = String(date.getSeconds()).padStart(2, '0');

    return `${hours}:${minutes}:${seconds}`;
}

function formatDateSQL(dateStr) {
    if (!dateStr) return "";

    if (dateStr instanceof Date) {
        return dateStr.toISOString().split("T")[0];
    }

    if (typeof dateStr !== "string") {
        console.error("Invalid dateStr:", dateStr);
        return "";
    }

    let parts = dateStr.split("-");
    if (parts.length !== 3) return "";

    let day = parts[0].padStart(2, "0");
    let month = parts[1].padStart(2, "0");
    let year = parts[2];

    return `${year}-${month}-${day}`;
}

function formatCurrencyValue(value) {
    if (value === null || value === undefined || value === '') return 0;

    const absVal = Math.abs(value);
    const isInteger = Number.isInteger(absVal);

    if (value < 0) {
        return isInteger
            ? `${absVal}`
            : `${absVal.toFixed(2)}`;
    }

    if (isInteger) return absVal.toString();


    return absVal.toFixed(2);
}

function getDateTimeNow() {
    const now = new Date();

    const dd = String(now.getDate()).padStart(2, '0');        // ngày
    const MM = String(now.getMonth() + 1).padStart(2, '0');   // tháng (0-11 nên +1)
    const yyyy = now.getFullYear();                           // năm
    const hh = String(now.getHours()).padStart(2, '0');       // giờ
    const ss = String(now.getSeconds()).padStart(2, '0');     // giây

    return dd + MM + yyyy + hh + ss;
}

function InitComponent() {

    GetChungLoaiVatTu();

    GetDiemDanhGia();

    GetNhaCungCap();

    let today = moment().format('DD-MM-YYYY');

    $("#selectYear").empty();

    $("#selectNhaCungCap").select2();

    const currentYear = new Date().getFullYear();
    for (let y = currentYear - 5; y <= currentYear + 1; y++) {
        $("#selectYear, #selectYearCLVT").append(`<option value="${y}">${y}</option>`);
    }
    $("#selectYear, #selectYearCLVT").val(currentYear);

    $("#selectFilterType").on("change", function () {
        const type = $(this).val();

        $(".filter-date, .filter-quarter, .filter-chungloai, .filter-nhacungcap").addClass("d-none");

        if (type === "date") {
            $(".filter-date").removeClass("d-none");
        } else if (type === "quarter") {
            $(".filter-quarter").removeClass("d-none");
        } else if (type === "chungloai") {
            $(".filter-chungloai").removeClass("d-none");
            const dropDown = $("#selectChungLoaiVatTu").dxDropDownBox("instance");
            const ds = dropDown.option("dataSource");
            if (ds && ds.length > 0) {
                dropDown.option("value", ds[0].MaCLVT);
            }
        } else if (type === "nhacungcap") {
            $(".filter-nhacungcap").removeClass("d-none");
            //GetNhaCungCap();
        }

        triggerGetPhieuDanhGia(); // gọi lại khi đổi kiểu lọc
    });


    $('#fromDate').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        autoApply: true,
        autoUpdateInput: true,
        locale: {
            format: 'DD-MM-YYYY',
            separator: ' - ',
            applyLabel: 'Chọn',
            cancelLabel: 'Hủy',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            customRangeLabel: 'Tùy chỉnh',
            daysOfWeek: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
            firstDay: 1
        }
    });

    $('#fromDate').val(today);

    $('#toDate').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        autoApply: true,
        autoUpdateInput: true,
        locale: {
            format: 'DD-MM-YYYY',
            separator: ' - ',
            applyLabel: 'Chọn',
            cancelLabel: 'Hủy',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            customRangeLabel: 'Tùy chỉnh',
            daysOfWeek: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
            monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
                'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
            firstDay: 1
        }
    });

    $('#toDate').val(today);


    triggerGetPhieuDanhGia();
}

window.addEventListener('resize', InitComponent);
window.addEventListener('load', InitComponent);
function GetDiemDanhGia() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetDiemDG`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ResetVal('ChungLoaiVT');
            if (data.length > 0) {
                arrDiemDanhGia = [...data]
                arrDiemDanhGia = arrDiemDanhGia.sort((a, b) => b.ID - a.ID);

            }

        }
    });
}
function GetNhaCungCap() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetNCC&para1=ALL`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {

            if (data.length > 0) {
                arrNhaCungCap = [...data]
                fetchNhaCC(data)
            }

        }
    });
}
function fetchNhaCC(data) {
    $('#selectNhaCungCap').empty();
    $.each(data, function (index, item) {
        $('#selectNhaCungCap').append(
            `<option value="${item.MaNhaCC}">${item.TenNCC}</option>`
        );
    });
    $('#selectNhaCungCap').val(data[0].MaNhaCC || "");
}
$('#selectNhaCungCap').on("change", function () {
    triggerGetPhieuDanhGia();
})
function ResetVal(selector) {
    //currentStream = null;
    //currentContainer = null;
    //currentFacingMode = 'environment'; // 'environment' (sau) hoặc 'user' (trước)
    //imageData = {};
    //fieldDelete = null;
    //arrSoRoll = [];
    //$("#selectSoRoll").empty();
    //if (selector == 'SoLo') {
    //    arrPhuLieu = new Array();
    //    arrSoLo = new Array();
    //    rowSelected_CLVT = null;
    //} else if (selector == 'PL') {
    //    rowSelected_CLVT = null;

    //}


}


// Sự kiện change cho select kiểu lọc
$("#selectFilterType").on("change", function () {
    triggerGetPhieuDanhGia();
});

// Sự kiện change cho các input liên quan
$("#fromDate, #toDate, #selectQuarter, #selectYear").on("change", function () {
    triggerGetPhieuDanhGia();
});

function triggerGetPhieuDanhGia() {
    const filterMode = $("#selectFilterType").val();
    let para1 = "", para2 = "";

    if (filterMode === "date") {
        para1 = formatDateSQL($("#fromDate").val());   // từ ngày
        para2 = formatDateSQL($("#toDate").val());     // đến ngày
    } else if (filterMode === "quarter") {
        para1 = $("#selectQuarter").val(); // quý I,II,III,IV
        para2 = $("#selectYear").val();    // năm
    } else if (filterMode === "chungloai") {
        para1 = $("#selectChungLoaiVatTu").val(); // mã chủng loại vật tư
        para2 = $("#selectYearCLVT").val();       // năm
    } else if (filterMode === "nhacungcap") {
        para1 = $("#selectNhaCungCap").val(); // mã nhà cung cấp

    }

    GetPhieuDanhGia(para1, para2);
}

function GetPhieuDanhGia(Para1, Para2) {
    let url = '';
    const filterMode = $("#selectFilterType").val();

    if (filterMode === "date") {
        url = `/api/PhieuDanhGiaNhaCC/Get?action=GetTheoDoiKhoangNgay&para1=${Para1}&para2=${Para2}&para3=${LoaiDanhGia}`;
    } else if (filterMode === "quarter") {
        url = `/api/PhieuDanhGiaNhaCC/Get?action=GetTDTheoQuy&para1=${Para1}&para2=${Para2}&para3=${LoaiDanhGia}`;
    } else if (filterMode === "chungloai") {
        const maCLVT = $("#selectChungLoaiVatTu").dxDropDownBox("instance").option("value");
        const nam = $("#selectYearCLVT").val();
        url = `/api/PhieuDanhGiaNhaCC/Get?action=GetTheoDoiChungLoai&para1=${maCLVT}&para2=${nam}&para3=${LoaiDanhGia}`;
    }
    else if (filterMode === "nhacungcap") {

        url = `/api/PhieuDanhGiaNhaCC/Get?action=GetTheoDoiNCC&para1=${$("#selectNhaCungCap").val()}&para2=NONE&para3=${LoaiDanhGia}`;
    }
    $.ajax({
        async: false,
        url: url,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                data = prepareData(data);

            }
            fetchPhieuDanhGia(data);
        }
    });
}

function prepareData(data) {
    data.forEach((row, index) => {

        row.ID = index + 1;


    });

    return data;
}

function fetchPhieuDanhGia(data) {
    let dynamicColumns = []
    if (data.length > 0) {
        dynamicColumns = Object.keys(data[0])
            .filter(k => k.includes("@TieuChi@"))
            .reduce((acc, key) => {
                const parts = key.split("@TieuChi@");
                const tenTieuChi = parts[1];
                const soThuTu = parts[2];
                let group = acc.find(g => g.caption === `(${soThuTu})`);
                if (!group) {
                    group = {
                        caption: `(${soThuTu})`,
                        columns: []
                    };
                    acc.push(group);
                }
                group.columns.push({
                    dataField: key,
                    caption: tenTieuChi,
                    allowFiltering: false,
                    cssClass: "col-diem-dg",
                    lookup: {
                        dataSource: arrDiemDanhGia,
                        valueExpr: "MaDiemDanhGia",
                        displayExpr: "DiemDanhGia"
                    },
                    editorOptions: {
                        showClearButton: true,
                        placeholder: "Chọn điểm đánh giá"
                    }
                });

                return acc;
            }, []);
    }


    $("#gridPhieuDanhGia").dxDataGrid({
        dataSource: data,
        editing: {
            mode: "batch",
            allowUpdating: false,
            allowAdding: false,
            allowDeleting: false
        },
        noDataText: "Chưa có đánh giá",
        keyExpr: "ID",
        focusedRowEnabled: true,

        columns: [

            {
                dataField: "ChungLoaiVatTu",
                caption: "ChungLoaiVatTu",
                minWidth: 200,
                groupIndex: 0,
                groupCellTemplate: function (cellElement, cellInfo) {
                    cellElement.html(`<span style="color: teal; font-weight: bold;">${cellInfo.value}</span>`);
                }
            },
            {
                dataField: "TenPhieu",
                caption: "Phiếu đánh giá",
                minWidth: 100,
                groupIndex: 1,
                groupCellTemplate: function (cellElement, cellInfo) {
                    cellElement.html(`<span style="color: teal; font-weight: bold;">${cellInfo.value}</span>`);
                }
            },
            { dataField: "NgayDanhGiaPhieu", caption: "Ngày đánh giá", dataType: "date", format: "dd-MM-yyyy", minWidth: 90, width: 100, },
            { dataField: "Quy", caption: "Quý" },
            { dataField: "Nam", caption: "Năm" },
            {
                dataField: "MaNhaCC",
                caption: "Tên nhà cung cấp",
                lookup: {
                    dataSource: arrNhaCungCap,
                    valueExpr: "MaNhaCC",
                    displayExpr: "TenNCC"
                }
            },
            {
                caption: "Tiêu chí theo dõi",
                columns: dynamicColumns
            },
            {
                dataField: "TongDiemUuTien",
                caption: "Tổng điểm",
                minWidth: 50,
                maxWidth: 60,
                setCellValue: function (newData, value, currentRowData) {
                    let sum = 0;
                    sum = tinhTong(currentRowData);
                    newData.TongDiemUuTien = sum;
                }
            },
            {
                dataField: "ThuTuUuTien",
                caption: "TT ưu tiên",
                minWidth: 60,
                width: 60,
                allowEditing: false
            },
            { dataField: "MaChungLoai", caption: "MaChungLoai", visible: false },
            {
                dataField: "GhiChu", caption: "Kết luận", minWidth: 100,
                width: 100
            },

            { dataField: "NguoiDanhGiaPhieu", caption: "Người tạo phiếu" },

        ],
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        columnAutoWidth: true,
        scrolling: { mode: "standard" },
        paging: { enabled: false },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        filterRow: { visible: true },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.dataField && e.column.dataField.includes("@TieuChi@")) {
                const cellValue = e.value;

                if (cellValue === "Diem_1" || cellValue === "Diem_2") {
                    e.cellElement.css("background-color", "#ffcccc");
                    e.cellElement.css("color", "#cc0000");
                    e.cellElement.css("font-weight", "bold");
                }
            } else if (e.rowType === "data" && e.column.dataField && e.column.dataField == "TongDiemUuTien") {
                
                let TongDiem = tinhTong(e.data)
                if (!isNaN(TongDiem) && TongDiem < 18) {
                    e.cellElement.css("background-color", "#ffcccc");
                    e.cellElement.css("color", "#cc0000");
                    e.cellElement.css("font-weight", "bold");
                }
            }
        },
        onEditorPreparing: function (e) {
            if (e.parentType === "dataRow" && e.editorOptions) {
                let originalOnValueChanged = e.editorOptions.onValueChanged;
                e.editorOptions.onValueChanged = function (args) {
                    if (originalOnValueChanged) originalOnValueChanged(args);
                    let rowData = e.row.data;
                    rowData[e.dataField] = args.value;
                    let sum = tinhTong(rowData);
                    e.component.cellValue(e.row.rowIndex, "TongDiemUuTien", sum);
                };
            }
        },
        onContentReady: function (e) {


            $(".dx-datagrid-headers td").css({
                "background-color": "#00477f",
                "color": "#fff",
                "text-align": "center",
                "vertical-align": "middle",
                "font-weight": "700"
            });
        }


    });
}
function GetChungLoaiVatTu() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetChungLoaiVT`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ResetVal('ChungLoaiVT');
            if (data.length > 0) {
                arrChungLoaiVT = [...data]
                fetchChungLoaiVatTu(data)

            }

        }
    });
}

function fetchChungLoaiVatTu(data) {
    $("#selectChungLoaiVatTu").dxDropDownBox({
        value: data.length > 0 ? data[0].MaCLVT : null,
        valueExpr: "MaCLVT",
        displayExpr: "ChungLoaiVatTu",
        placeholder: "Chọn loại hàng hóa",
        dataSource: data.length == 0 ? new Array() : data,
        contentTemplate: function (e) {
            return $("<div class='pl-contents'>").dxDataGrid({
                dataSource: e.component.option("dataSource"),
                height: 600,
                columns: [
                    { dataField: "MaCLVT", caption: "MaCLVT", visible: false },
                    { dataField: "ChungLoaiVatTu", caption: "Loại hàng hóa", cssClass: "col-chi-tiet" },
                    { dataField: "TenKhac", caption: "Tên gọi khác" },
                    {
                        dataField: "LoaiVT", caption: "Loại vật tư",
                        groupIndex: 0,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            cellElement.html(`<span style="color: teal; font-weight: bold;">${cellInfo.value}</span>`);
                        }
                    },
                ],
                searchPanel: { visible: true },
                columnAutoWidth: false,
                scrolling: {
                    mode: 'standard',
                    showScrollbar: 'always'
                },
                width: '100%',

                noDataText: "Chưa có dữ liệu",
                selection: { mode: "single" },
                hoverStateEnabled: true,
                paging: { enabled: false },
                filterRow: { visible: true },
                showBorders: true,
                showRowLines: true,
                showColumnLines: true,

                onRowClick: function (eRow) {
                    const keys = [eRow.data];
                    e.component.option("value", keys.length ? keys[0].MaCLVT : null);
                    if (keys[0]?.MaCLVT) {
                        /*  ResetVal('PL');*/
                        rowSelected_CLVT = keys[0];
                        GetPhieuDanhGia("NONE", "NONE")

                    }
                    eRow.component.selectRowsByIndexes([eRow.rowIndex]);
                    $("#selectChungLoaiVatTu").dxDropDownBox("instance").close();
                },
                onContentReady: function () {
                    $(".dx-header-row td").css({
                        "background-color": "#00477f",
                        "color": "#fff",
                        "text-align": "center",
                        "vertical-align": "middle",
                        "font-weight": "700"
                    });
                }

            })
        },

    });
}

function tinhTong(rowData) {
    let sum = 0;
    Object.keys(rowData).forEach(field => {
        if (field.includes("@TieuChi@") && rowData[field]) {
            switch (rowData[field]) {
                case "Diem_1":
                    sum += 1;
                    break;
                case "Diem_2":
                    sum += 2;
                    break;
                case "Diem_3":
                    sum += 3;
                    break;
                case "Diem_4":
                    sum += 4;
                    break;
                case "Diem_5":
                    sum += 5;
                    break;
                default:
                    sum += 0;
            }
        }
    });
    return sum;
}

function getFocusedMaPhieu() {
    let grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
    let focusedRowKey = grid.option("focusedRowKey");
    let visibleRows = grid.getVisibleRows();

 
    let focusedRowData = null;
    if (focusedRowKey) {
        let row = visibleRows.find(r => r.key === focusedRowKey && r.rowType === "data");
        if (row) {
            focusedRowData = row.data;
        }
    }


    if (!focusedRowData || !focusedRowData.MaPhieuDG) {
        let groupRowIndex = visibleRows.findIndex(r => r.key === focusedRowKey && r.rowType === "group");
        if (groupRowIndex >= 0) {
            let childRow = visibleRows.find(r => r.rowIndex > groupRowIndex && r.rowType === "data");
            if (childRow) {
                focusedRowData = childRow.data;
            }
        }
    }

    return focusedRowData;
}

$("#btn-them-danh-gia").on("click", function () {
    let Action = 'add';
    let MaPhieu = 'NONE'; 
    window.location.assign("/POMuaHang/PhieuDanhGia?Action=" + Action + "&MaPhieu=" + MaPhieu + "&MaCLVTID=NONE");
});

$("#btn-sua-phieu").on("click", function () {
    let Action = "edit";
    var focusedRowData = getFocusedMaPhieu();
    let MaPhieu = focusedRowData ? focusedRowData.MaPhieuDG : "NONE";
    let MaCLVTID = focusedRowData ? focusedRowData.MaChungLoai : "NONE";
  
    if (MaPhieu === "NONE") {
        DevExpress.ui.notify("Vui lòng chọn phiếu để sửa", "warning", 2000);
        return;
    }
    window.location.assign("/POMuaHang/PhieuDanhGia?ActionDG=edit"  + "&MaPhieu=" + MaPhieu + "&MaCLVTID=" + MaCLVTID);
});

$("#btn-xoa-phieu").on("click", function () {
    var focusedRowData = getFocusedMaPhieu();
    let MaPhieu = focusedRowData ? focusedRowData.MaPhieuDG : "NONE";
    if (MaPhieu === "NONE") {
        DevExpress.ui.notify("Vui lòng chọn phiếu để xóa", "warning", 2000);
        return;
    }
    DevExpress.ui.dialog.confirm('Bạn có chắc muốn xoá phiếu này?', 'Xác nhận xoá')
        .done(function (res) {
            if (res) {
                fetch(`/api/PhieuDanhGiaNhaCC/Delete?action=DeletePhieuDG&Para1=${MaPhieu}&Para2=${LoaiDanhGia}`, { method: 'DELETE' })
                    .then(response => {
                        if (!response.ok) throw new Error("Lỗi khi xoá");
                        DevExpress.ui.notify('Đã xoá thành công.', 'success', 1500);
                        triggerGetPhieuDanhGia();
                        
                    })
                    .catch(() => DevExpress.ui.dialog.alert('Xoá thất bại. Vui lòng thử lại.', 'Lỗi'));
            }
        });

})

$("#btn-refresh").on("click", function () {
    triggerGetPhieuDanhGia();

})
$("#toggle-card").on("click", function () {
    const $content = $(".collapse-content");
    $(this).toggleClass("collapsed");
    $content.toggleClass("collapsed");

    // đổi icon +/-
    if ($(this).hasClass("collapsed")) {
        $(this).html('<i class="fas fa-plus"></i>');
    } else {
        $(this).html('<i class="fas fa-minus"></i>');
    }
});


$("#btn-Excel").on("click", function () {
    var focusedRowData = getFocusedMaPhieu();
    let MaPhieu = focusedRowData ? focusedRowData.MaPhieuDG : "NONE";
    let MaCLVTID = focusedRowData ? focusedRowData.MaChungLoai : "NONE";

    if (MaPhieu === "NONE") {
        DevExpress.ui.notify("Vui lòng chọn phiếu để xuất excel", "warning", 2000);
        return;
    }
    var url = `/api/PhieuDanhGiaNhaCC/ExportPhieuDanhGia?para1=${MaPhieu}&para2=${MaCLVTID}&para3=${userName}`;
    const fileName = (() => {
        const now = new Date();
        const pad = n => n.toString().padStart(2, '0');
        return `BM03/QT15-${pad(now.getDate())}${pad(now.getMonth() + 1)}${now.getFullYear()}${pad(now.getHours())}${pad(now.getMinutes())}${pad(now.getSeconds())}.xlsx`;
    })();

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: null,
    })
        .then(response => {
            if (!response.ok) {
                alert("Lỗi Không thể xuất excel . Vui lòng thử lại!")
                return;
            }

            return response.blob();
        })
        .then(blob => {
            var a = document.createElement("a");
            var url = window.URL.createObjectURL(blob);
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Error fetching data from the server.', error);
        });
})
$(document).ready(async function () {
    InitComponent();
    GetPhieuDanhGia()
    
});