let arrTieuChiDGia = [];
let arrChungLoaiVT = [];
let arrDiemDanhGia = [];
let arrNhaCungCap = [];
let arrNhanVien = [];

let TenPhieu = "";
let LoaiDanhGia = "DanhGia";

let rowSelected_CLVT = null;

const userName = localStorage.getItem("username1");
const ModuleID = 'M.21.00.00'
let IsSoatXet = false;
let allowXacNhan = false;

function CheckPerminsion() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=CheckPerminsion&para1=${userName}&para2=${ModuleID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                const permissions = data[0];          
                setPermission('xac-nhan-soat-xet', permissions.AllowKySoatXet === 1 || permissions.AllowKySoatXet === true);
                setPermission('xac-nhan-giam-doc', permissions.AllowKyGD === 1 || permissions.AllowKyGD === true);
                IsSoatXet = permissions.AllowKySoatXet === 1 || permissions.AllowKySoatXet === true;
                allowXacNhan = permissions.AllowXacNhan === 1 || permissions.AllowXacNhan === true;
            }
            else {
                IsSoatXet = false;
                setPermission('xac-nhan-soat-xet', false);
                setPermission('xac-nhan-giam-doc', false);
            }
        }
    });
}

function setPermission(className, hasPermission) {
    const selector = `.${className}`;

    if (hasPermission) {

        $(selector + " input[type='radio']").prop('disabled', false);
        $(selector + " textarea").prop('readonly', false);
        $(selector + " .btn-clear").prop('disabled', false);
        $(selector + " canvas").css('pointer-events', 'auto');

        $(selector).css({
            'opacity': '1',
            'cursor': 'default'
        });
    } else {

        $(selector + " input[type='radio']").prop('disabled', true);
        $(selector + " textarea").prop('readonly', true);
        $(selector + " .btn-clear").prop('disabled', true);
        $(selector + " canvas").css('pointer-events', 'none');

        $(selector).css({
            'opacity': '0.6',
            'cursor': 'not-allowed'
        });
    }
}

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
    GetTenNhanVien();
    GetDiemDanhGia();
    GetTieuChiDanhGia();

    let today = moment().format('DD-MM-YYYY');

    $('#ngay-danh-gia').daterangepicker({
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

    $('#ngay-danh-gia').val(today);

    $("#txt-nam").val(new Date().getFullYear())


    if (Action == 'edit') {
        GetPhieuDanhGia(MaCLVTID);
        GetKetQuaDanhGia();
        $("#selectChungLoaiVatTu").dxDropDownBox("instance").option("disabled", true);

    } else if (Action == 'add') {
        $("#selectChungLoaiVatTu").dxDropDownBox("instance").option("disabled", false);
    }

}

window.addEventListener('resize', InitComponent);

window.addEventListener('load', InitComponent);

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

function GetNhaCungCap(MaCLVTID) {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetNCC&para1=${MaCLVTID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            ResetVal('ChungLoaiVT');
            if (data.length > 0) {
                arrNhaCungCap = [...data]

            }

        }
    });
}

function GetChungLoaiVatTu() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetChungLoaiVT`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            //ResetVal('ChungLoaiVT');
            if (data.length > 0) {
                arrChungLoaiVT = [...data]
                fetchChungLoaiVatTu(data)

            }

        }
    });
}

function fetchChungLoaiVatTu(data) {
    $("#selectChungLoaiVatTu").dxDropDownBox({
        value: null,
        valueExpr: "MaCLVT",
        displayExpr: "ChungLoaiVatTu",
        placeholder: "Chọn loại hàng hóa",
        dataSource: data.length == 0 ? new Array() : data,
        contentTemplate: function (e) {
            return $("<div class='pl-contents'>").dxDataGrid({
                dataSource: e.component.option("dataSource"),
                height: 600, // hoặc giá trị phù hợp
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
                        rowSelected_CLVT = keys[0];
                        GetNhaCungCap(keys[0]?.MaCLVT);
                        if (Action != 'edit') {
                            Action = "add"
                            GeneratePhieuDanhGia();
                            GetCountPhieuDanhGia(rowSelected_CLVT.MaCLVT, rowSelected_CLVT.ChungLoaiVatTu)
                            GetPhieuDanhGia(rowSelected_CLVT.MaCLVT);
                        }
                    }
                    eRow.component.selectRowsByIndexes([eRow.rowIndex]);
                    $("#selectChungLoaiVatTu").dxDropDownBox("instance").close();
                },
                onContentReady: function () {
                    $(".dx-datagrid-headers td").css({
                        "background-color": "#00477f",
                        "color": "#fff",
                        "text-align": "center",
                        "vertical-align": "middle",
                        "font-weight": "700"
                    });
                }
            });

        },

    });
    if (Action == 'edit') {
        if (MaCLVTID != "NONE") {
            $("#selectChungLoaiVatTu").dxDropDownBox("option", "value", MaCLVTID);
            GetNhaCungCap(MaCLVTID);
        }
    }
}

function GetCountPhieuDanhGia(MaVTID, TenCL) {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetCountPhieuDanhGia&para1=${MaVTID}&para2=${LoaiDanhGia}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            var countPhieu = 1
            if (data.length > 0) {
                countPhieu = data[0].CountPhieu
            }
            TenPhieu = `PDG|${TenCL}|${getDateTimeNow()}|${countPhieu}`
            $("#txt-phieu-dg").val(TenPhieu);
        }
    });
}

function GeneratePhieuDanhGia() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GeneratePhieuDanhGia`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {

                MaPhieu = data[0].SoPhieu
            }

        }
    });
}

function GetTenNhanVien() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetUser`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                arrNhanVien = [...data];
                if (Action != 'edit') {
                    $("#txt-nguoi-danh-gia").val(FindTenNhanVien(userName));
                }
            }

        }
    });
}

function FindTenNhanVien(userName) {
    var objNhanVien = arrNhanVien.find(x =>
        x.UserID.toUpperCase() === userName.toUpperCase()
    );
    return !objNhanVien ? userName : objNhanVien.TenNV;
}

function GetTieuChiDanhGia() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetTieuChiDG`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {

            if (data.length > 0) {
                arrTieuChiDGia = [...data]
                fetchTieuChiDGia(arrTieuChiDGia)

            }

        }
    });
}

function fetchTieuChiDGia(data) {
    const diemFields = Object.keys(data[0]).filter(k => k.startsWith("Điểm"));

    const diemColumns = diemFields.map(field => ({
        dataField: field,
        caption: field,
        minWidth: 180,
        wordWrapEnabled: true
    }));
    const columns = [
        { dataField: "STT", caption: "Stt", width: 50 },
        { dataField: "TieuChiTQ", caption: "Tiêu chí đánh giá nhà cung cấp/nhà thầu phụ", minWidth: 250, wordWrapEnabled: true },
        {
            caption: "Điểm đánh giá",
            alignment: "center",
            columns: diemColumns
        },
        { dataField: "GhiChu", caption: "Ghi chú", minWidth: 300, wordWrapEnabled: true }
    ];

    $("#gridTieuChiDG").dxDataGrid({
        dataSource: data,
        columns: columns,
        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        scrolling: { mode: "standard" },
        paging: { enabled: false },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        onContentReady: function () {
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

function GetPhieuDanhGia(MaCLVTID) {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetPhieuDG_TQ&para1=${MaCLVTID}&para2=${MaPhieu}&para3=${Action}&para4=${LoaiDanhGia}&para5=${userName}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                data = prepareData(data)
                console.table(data)
                fetchPhieuDanhGia(data)
                NguoiDanhGiaPhieu = data[0]["NguoiDanhGiaPhieu"];
                if (Action == "edit") {
                    //$("#txt-phieu-dg").val(data[0].TenPhieu)
                    //$("#txt-nguoi-danh-gia").val(FindTenNhanVien(data[0].NguoiDanhGiaPhieu || userName))
                    $('#ngay-danh-gia').val(formatDateView(data[0].NgayDanhGiaPhieu));
                }
                CheckPerminsion();
            }

        }
    });

}

function kiemTraTatCaDiemTren3(rowData) {
    const diemFields = Object.keys(rowData).filter(k => k.includes("@TieuChi@"));
    if (diemFields.length === 0) return false;

    for (let field of diemFields) {
        const value = rowData[field];
        if (value !== null && value !== undefined && value !== "") {

            if (value === "Diem_1" || value === "Diem_2") {
                return false;
            }
        }
    }
    // Nếu tất cả đều >= Diem_3 thì thỏa
    return true;
}

function kiemTraTatCaDiemRong(rowData) {
    const pivotKeys = Object.keys(rowData).filter(k => k.includes("@TieuChi@"));
    const allEmpty = pivotKeys.every(k => { const val = rowData[k]; return val === null || val === undefined || val === ""; });
    return allEmpty;
}

function capNhatThuTuUuTien(grid) {
    grid.saveEditData();
    const visibleRows = grid.getVisibleRows();
    const data = visibleRows.map(row => row.data).filter(d => d); // Lọc bỏ undefined

    const danhSachThoaDK = data.filter(row => kiemTraTatCaDiemTren3(row));
    const danhSachKhongThoaDK = data.filter(row => !kiemTraTatCaDiemTren3(row));
    const DsDiemRong = data.filter(row => kiemTraTatCaDiemRong(row));

    danhSachThoaDK.sort((b, a) => a.TongDiemUuTien - b.TongDiemUuTien);
    danhSachKhongThoaDK.sort((b, a) => a.TongDiemUuTien - b.TongDiemUuTien);

    let TTUUTien = 1;

    [...danhSachThoaDK, ...danhSachKhongThoaDK].forEach(row => {
        if (!kiemTraTatCaDiemRong(row)) {
            const rowIndex = visibleRows.findIndex(r => r.data.MaNhaCC === row.MaNhaCC);
            if (rowIndex !== -1) {
                grid.cellValue(rowIndex, "ThuTuUuTien", TTUUTien);
                TTUUTien++;
            }
        }
    });

    DsDiemRong.forEach(row => {
        const rowIndex = visibleRows.findIndex(r => r.data.MaNhaCC === row.MaNhaCC);
        if (rowIndex !== -1) {
            grid.cellValue(rowIndex, "ThuTuUuTien", 0);
        }
    });

    grid.refresh();
}

function prepareData(data) {
    data.forEach(row => {
        row.TongDiemUuTien = tinhTong(row);
        if (Action == "add") {
            row["IsEdit"] = true
        } else {
            row["IsEdit"] = false
        }
    });
    return data;
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
var checkInput = true
function fetchPhieuDanhGia(data) {
    const dynamicColumns = Object.keys(data[0])
        .filter(k => k.includes("@TieuChi@"))
        .reduce((acc, key) => {
            const parts = key.split("@TieuChi@");
            const tenTieuChi = parts[1];
            const soThuTu = parts[2];

            let group = acc.find(g => g.caption === `(${soThuTu})`);
            if (!group) {
                group = { caption: `(${soThuTu})`, columns: [] };
                acc.push(group);
            }

            group.columns.push({
                dataField: key,
                caption: tenTieuChi,
                cssClass: "col-diem-dg",
                lookup: {
                    dataSource: arrDiemDanhGia,
                    valueExpr: "MaDiemDanhGia",
                    displayExpr: "DiemDanhGia"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    showClearButton: false,
                    searchEnabled: false,
                    acceptCustomValue: false,
                    placeholder: "Chọn",
                    showDropDownButton: false,
                   
                    onFocusIn: function (e) {
                        e.component.open();
                    }
                }
            });

            return acc;
        }, []);

    $("#gridPhieuDanhGia").dxDataGrid({
        dataSource: data,
        editing: {
            mode: "cell",
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: true,
            confirmDelete: false,
            startEditAction: "click",
            selectTextOnEditStart: true
        },
        columns: [
            {
                dataField: "IsEdit",
                caption: "IsEdit",
                visible: false
            },
            {
                dataField: "MaNhaCC",
                caption: "Nhà cung cấp",
              
                lookup: {
                    dataSource: arrNhaCungCap,
                    valueExpr: "MaNhaCC",
                    displayExpr: "TenNCC"
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    showClearButton: false,
                    showClearButton: false,
                    searchEnabled: false,
                    acceptCustomValue: false,
                    placeholder: "Chọn NCC",
                    showDropDownButton: false,
                    onFocusIn: function (e) {
                        e.component.open();

                    }
                },
                
            },
            {
                caption: "Thông tin",
                columns: dynamicColumns
            },
            {
                dataField: "TongDiemUuTien",
                caption: "Tổng điểm",
                minWidth: 60,
                width: 60,
                allowEditing: false
            },
            {
                dataField: "ThuTuUuTien",
                caption: "TT ưu tiên",
                minWidth: 60,
                width: 60,
                allowEditing: false,
                

            },
            {
                dataField: "GhiChu",
                caption: "Kết luận",
                minWidth: 100,
                width: 100,
               
            },
            {
                type: "buttons",
                caption: "Xóa",
                minWidth: 60,
                width: 60,
                buttons: [{
                    hint: "Xoá",
                    icon: "trash",
                    onClick: function (e) {
                        const rowData = e.row.data;
                        const grid = e.component;

                        const removeRowAndRecalcAndRedirect = () => {
                            const rowIndex = e.row.rowIndex;
                            if (rowIndex < 0) return;

                            grid.cancelEditData();
                            grid.deleteRow(rowIndex);

                            const rows = grid.getVisibleRows().map(r => r.data);

                            const danhSachThoaDK = rows.filter(r => kiemTraTatCaDiemTren3(r));
                            const danhSachKhongThoaDK = rows.filter(r => !kiemTraTatCaDiemTren3(r));
                            const DsDiemRong = rows.filter(r => kiemTraTatCaDiemRong(r));

                            danhSachThoaDK.sort((a, b) => b.TongDiemUuTien - a.TongDiemUuTien);
                            danhSachKhongThoaDK.sort((a, b) => b.TongDiemUuTien - a.TongDiemUuTien);

                            let TTUUTien = 1;

                            [...danhSachThoaDK, ...danhSachKhongThoaDK].forEach(row => {
                                if (!kiemTraTatCaDiemRong(row)) {
                                    const idx = grid.getVisibleRows()
                                        .findIndex(r => r.data.MaNhaCC === row.MaNhaCC);
                                    if (idx !== -1) {
                                        grid.cellValue(idx, "ThuTuUuTien", TTUUTien++);
                                    }
                                }
                            });

                            DsDiemRong.forEach(row => {
                                const idx = grid.getVisibleRows()
                                    .findIndex(r => r.data.MaNhaCC === row.MaNhaCC);
                                if (idx !== -1) {
                                    grid.cellValue(idx, "ThuTuUuTien", 0);
                                }
                            });

                            grid.saveEditData();

                            if (grid.getVisibleRows().length === 1) {
                                window.location.assign("/POMuaHang/TongQuanDanhGia");
                            }
                        };

                        if (rowData.IsEdit == true) {
                            removeRowAndRecalcAndRedirect();
                            return;
                        }

                        DevExpress.ui.dialog.confirm(
                            'Bạn có chắc muốn xoá dòng này?',
                            'Xác nhận xoá'
                        ).done(function (res) {
                            if (!res) return;

                            removeRowAndRecalcAndRedirect();

                            fetch(`/api/PhieuDanhGiaNhaCC/Delete?action=DeleteNhaCCDG&Para1=${MaPhieu}&Para2=${rowData.MaNhaCC}`, {
                                method: 'DELETE'
                            })
                                .then(r => {
                                    if (!r.ok) throw new Error();
                                    DevExpress.ui.notify('Đã xoá thành công.', 'success', 1500);
                                })
                                .catch(() => {
                                    DevExpress.ui.dialog.alert('Xoá thất bại. Dữ liệu sẽ được tải lại.', 'Lỗi');
                                    grid.refresh();
                                });
                        });
                    }
                }]
            }
        ],
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        columnAutoWidth: true,
        scrolling: {
            mode: 'standard',
            useNative: true,
            showScrollbar: 'always',
        },
        paging: { enabled: false },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        onCellPrepared: function (e) {    
            if (e.rowType === "data" && e.column.dataField && e.column.dataField.includes("@TieuChi@")) {
                const cellValue = e.value;

                if (cellValue === "Diem_1" || cellValue === "Diem_2") {
                    e.cellElement.css("background-color", "#ffcccc");
                    e.cellElement.css("color", "#cc0000");
                    e.cellElement.css("font-weight", "bold");
                }
            } else if (e.rowType === "data" && e.column.dataField && e.column.dataField == "TongDiemUuTien")
            {
                const cellValue = e.value;
                var TongDiem = Number(cellValue)
                if (!isNaN(TongDiem) && TongDiem < 18) {
                    e.cellElement.css("background-color", "#ffcccc");
                    e.cellElement.css("color", "#cc0000");
                    e.cellElement.css("font-weight", "bold");
                }
            }
        },
        onEditorPreparing: function (e) {

            if (e.parentType === "dataRow" && e.dataField === "MaNhaCC") {
                let originalOnValueChanged = e.editorOptions.onValueChanged;


                if (e.row.data.IsEdit !== true) {
                    e.editorOptions.disabled = true;
                    e.editorOptions.readOnly = true;
                }

                e.editorOptions.onValueChanged = function (args) {
                    if (originalOnValueChanged) {
                        originalOnValueChanged(args);
                    }

                    const rowData = e.row.data;
                    rowData.MaNhaCC = args.value;

                    const grid = e.component;
                    const dataSource = grid.option("dataSource");
                    const currentRowIndex = e.row.rowIndex;
                    const selectedValue = args.value;


                    const duplicateRows = dataSource
                        .map((row, idx) => ({ row, idx }))
                        .filter(item =>
                            item.row.MaNhaCC === selectedValue &&
                            item.idx !== currentRowIndex
                        );

                    if (duplicateRows.length > 0) {
                        const duplicateLineNumbers = duplicateRows
                            .map(d => d.idx + 1)
                            .join(", ");

                        DevExpress.ui.notify(
                            `Nhà cung cấp đã bị trùng ở các dòng: ${duplicateLineNumbers}`,
                            "error",
                            4000
                        );
                    }

                    grid.refresh();
                };
            }

            else if (e.parentType === "dataRow" && e.dataField && e.dataField.includes("@TieuChi@")) {
                let originalOnValueChanged = e.editorOptions.onValueChanged;
                e.editorOptions.onValueChanged = function (args) {
                    if (originalOnValueChanged) {
                        originalOnValueChanged(args);
                    }

                    const rowData = e.row.data;
                    rowData[e.dataField] = args.value;
                    const grid = e.component;


                    const sum = tinhTong(rowData);
                    e.component.cellValue(e.row.rowIndex, "TongDiemUuTien", sum);
                    if (sum >= 18) {
                        e.component.cellValue(e.row.rowIndex, "GhiChu", "Pass");
                    } else {
                        e.component.cellValue(e.row.rowIndex, "GhiChu", "Fail");
                    }
                    capNhatThuTuUuTien(grid);
                };
            }
            else if (e.parentType === "dataRow" && e.dataField === "GhiChu")
            {
                if (Action == "edit" &&
                    (allowXacNhan == true || NguoiDanhGiaPhieu == FindTenNhanVien(userName || userName))) {
                    e.editorOptions.disabled = false;
                    e.editorOptions.readOnly = false;
                } else {
                    e.editorOptions.disabled = true;
                    e.editorOptions.readOnly = true;
                }
                
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

            
            $(".dx-dropdowneditor-button").hide();
        }
    });
}

/*function tinhTong(rowData) {
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
}*/

function GetKetQuaDanhGia() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetKetQuaDG&para1=${MaPhieu}&para2=${LoaiDanhGia}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {

                fetchKetQuaDanhGia(data[0]);
            }
        }
    });
}
let NguoiDanhGiaPhieu = "";
function fetchKetQuaDanhGia(item) {

    $("#txt-phieu-dg").val(item.TenPhieu)
    $("#txt-nguoi-danh-gia").val(FindTenNhanVien(item.NguoiDanhGiaPhieu || userName))
    $("#txt-nam").val(item.Nam || new Date().getFullYear())

    if (item.KQ_DG_NgDeXuat == "true") {
        $("#KQPass_NgDeXuat").prop("checked", true);
    } else if (item.KQ_DG_NgDeXuat == "false") {
        $("#KQFail_NgDeXuat").prop("checked", true);
    }
    $("#txtDG_QC").val(item.YKien_NgDeXuat || "");

    // Soát xét
    if (item.KQ_DG_SoatXet == "true") {
        $("#KQPass_SoatXet").prop("checked", true);
    } else if (item.KQ_DG_SoatXet == "false") {
        $("#KQFail_SoatXet").prop("checked", true);
    }
    $("#txtDG_Mer").val(item.YKien_SoatXet || "");

    // Tổng giám đốc
    if (item.KQ_DG_TongGiamDoc == "true") {
        $("#KQPass_TongGiamDoc").prop("checked", true);
    } else if (item.KQ_DG_TongGiamDoc == "false") {
        $("#KQFail_TongGiamDoc").prop("checked", true);
    }
    $("#txtDG_KQ").val(item.YKien_TongGiamDoc || "");


    if (item.SignDG_NgDeXuat) {
        drawSignature("signQC", item.SignDG_NgDeXuat);
    }
    if (item.SignDG_SoatXet) {
        drawSignature("signMer", item.SignDG_SoatXet);
    }
    if (item.SignDG_TongGiamDoc) {
        drawSignature("signXN_KQ", item.SignDG_TongGiamDoc);
    }
}

function drawSignature(canvasId, base64Image) {
    const canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext("2d");

    function resizeCanvasAndDraw() {
        const ratio = Math.max(window.devicePixelRatio || 1, 1);
        canvas.width = canvas.offsetWidth * ratio;
        canvas.height = canvas.offsetHeight * ratio;
        ctx.scale(ratio, ratio);

        const img = new Image();
        img.onload = function () {
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            // Vẽ ảnh đúng kích thước gốc, căn giữa nếu muốn
            const x = (canvas.width / ratio - img.width) / 2;
            const y = (canvas.height / ratio - img.height) / 2;
            ctx.drawImage(img, x, y);
        };
        img.src = base64Image;
    }

    window.addEventListener('resize', resizeCanvasAndDraw);
    resizeCanvasAndDraw();
}

$("#btn-them-nhacc").on("click", function () {
    const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
    let ds = grid.option("dataSource");
    let today = new Date();

    let newRow = {
        NgayDanhGia: formatDateView(today),
        MaNhaCC: null,
        TongDiemUuTien: 0,
        ThuTuUuTien: null,
        GhiChu: "",
        NguoiDanhGia: FindTenNhanVien(userName),
        IsEdit: true
    };

    grid.getVisibleColumns().forEach(col => {
        if (col.dataField && col.dataField.includes("@TieuChi@")) {
            newRow[col.dataField] = null;
        }
    });


    ds.push(newRow);


    grid.option("dataSource", ds);

    let rowIndex = ds.length - 1;
    grid.editRow(rowIndex);
});

$("#btn-save").on("click", function () {
    saveDanhGiaNCC();
});

function getYearAndQuarter(dateStr) {
    if (!dateStr) return { year: "", quarter: "" };

    // Parse đúng định dạng DD-MM-YYYY
    const m = moment(dateStr, "DD-MM-YYYY", true);
    if (!m.isValid()) return { year: "", quarter: "" };

    const year = m.year();
    const month = m.month() + 1; // month() trả 0-11

    let quarter = "";
    if (month >= 1 && month <= 3) {
        quarter = "I";
    } else if (month >= 4 && month <= 6) {
        quarter = "II";
    } else if (month >= 7 && month <= 9) {
        quarter = "III";
    } else if (month >= 10 && month <= 12) {
        quarter = "IV";
    }

    return { year, quarter };
}

//function saveDanhGiaNCC() {    
//    const grid = $("#gridPhieuDanhGia").dxDataGrid('instance');
//    if (!grid) {
//        DevExpress.ui.notify('Không tìm thấy dữ liệu đánh giá!', 'warning', 2000);
//        return;
//    }
//    const ngayDG = $("#ngay-danh-gia").val();
//    const { year, quarter } = getYearAndQuarter(ngayDG);

//    const visibleRows = grid.getVisibleRows();
//    if (visibleRows.length === 0) {
//        DevExpress.ui.notify('Không có dữ liệu để lưu!', 'warning', 2000);
//        return;
//    }

//    let dataUpload = [];
//    let arrSaveData = [];


//    const maPhieuDG = MaPhieu || "NONE";
//    const ngayDanhGia = formatDateSQL($("#ngay-danh-gia").val());
//    const nguoiDanhGia = userName;
//    const ghiChu = null;


//    let kqNgDeXuat = null;
//    const radioNgDeXuat = $('input[name="result-nguoi-de-xuat"]:checked').val();
//    if (radioNgDeXuat === 'pass') {
//        kqNgDeXuat = true;
//    } else if (radioNgDeXuat === 'fail') {
//        kqNgDeXuat = false;
//    }

//    let kqSoatXet = null;
//    const radioSoatXet = $('input[name="result-soat-xet"]:checked').val();
//    if (radioSoatXet === 'pass') {
//        kqSoatXet = true;
//    } else if (radioSoatXet === 'fail') {
//        kqSoatXet = false;
//    }

//    let kqTongGiamDoc = null;
//    const radioTongGiamDoc = $('input[name="result-tong-giam-doc"]:checked').val();
//    if (radioTongGiamDoc === 'pass') {
//        kqTongGiamDoc = true;
//    } else if (radioTongGiamDoc === 'fail') {
//        kqTongGiamDoc = false;
//    }


//    const yKienQC = $("#txtDG_QC").val() || "";
//    const yKienMer = $("#txtDG_Mer").val() || "";
//    const yKienKQ = $("#txtDG_KQ").val() || "";


//    const signQC = getSignatureBase64('signQC');
//    const signMer = getSignatureBase64('signMer');
//    const signKQ = getSignatureBase64('signXN_KQ');


//    const getFileName = `DG-${maPhieuDG}`;

//    if (signQC) {
//        dataUpload.push({
//            img: signQC,
//            name: 'NgDeXuat'
//        });
//    }

//    if (signMer) {
//        dataUpload.push({
//            img: signMer,
//            name: 'SoatXet'
//        });
//    }

//    if (signKQ) {
//        dataUpload.push({
//            img: signKQ,
//            name: 'TongGiamDoc'
//        });
//    }

//    visibleRows.forEach(row => {
//        const rowData = row.data;       
//        Object.keys(rowData).forEach(key => {
//            if (/@TieuChi@/.test(key)) {

//                const parts = key.split('@TieuChi@');
//                const tieuChiID = parts[0] || ""; // ID
//                //const tieuChiNO = parts[2] || ""; // NO

//                const diemValue = rowData[key] || "";

//                const objSave = {
//                    ID: rowData.ID || 0,
//                    MaCLVTID: Action == "edit" ? MaCLVTID : rowSelected_CLVT.MaCLVT,
//                    Quy: quarter,
//                    Nam: year,
//                    NgayDanhGia: formatDateSQL($("#ngay-danh-gia").val()),
//                    NguoiDanhGia: rowData.NguoiDanhGia,
//                    GhiChu: ghiChu,


//                    KQ_DG_NgDeXuat: kqNgDeXuat,
//                    SignDG_NgDeXuat: signQC ? null : (rowData.SignDG_NgDeXuat || null),
//                    YKien_NgDeXuat: yKienQC,
//                    NgayKy_NgDeXuat: signQC ? formatDateSQL(new Date()) : null,


//                    KQ_DG_SoatXet: kqSoatXet,
//                    SignDG_SoatXet: signMer ? null : (rowData.SignDG_SoatXet || null),
//                    YKien_SoatXet: yKienMer,
//                    NgayKy_SoatXet: signMer ? formatDateSQL(new Date()) : null,


//                    KQ_DG_TongGiamDoc: kqTongGiamDoc,
//                    SignDG_TongGiamDoc: signKQ ? null : (rowData.SignDG_TongGiamDoc || null),
//                    YKien_TongGiamDoc: yKienKQ,
//                    NgayKy_TongGiamDoc: signKQ ? formatDateSQL(new Date()) : null,


//                    MaPhieuDG: maPhieuDG,
//                    LoaiDanhGia: rowData.LoaiDanhGia || "",
//                    TieuChiID: tieuChiID,       
//                    DiemID: diemValue || null,           
//                    MaNhaCC: rowData.MaNhaCC || "",
//                    ThuTuUuTien: rowData.ThuTuUuTien,
//                    KetQua: rowData.KetQua || "",
//                    TenPhieu: $("#txt-phieu-dg").val(),
//                    NgayDanhGiaPhieu: ngayDanhGia,
//                    NguoiDanhGiaPhieu: nguoiDanhGia,
//                };

//                arrSaveData.push(objSave);
//            }
//        });
//    });

//    console.table(arrSaveData); // Debug

//    if (dataUpload.length > 0) {
//        $.ajax({
//            url: `/api/PhieuDanhGiaNhaCC/UploadSignature?getFileName=${encodeURIComponent(getFileName)}`,
//            method: 'POST',
//            contentType: 'application/json',
//            data: JSON.stringify(dataUpload),
//            success: function (imagePaths) {
//                console.log("Signature paths:", imagePaths);

//                const findImage = (keyword) => {
//                    const found = imagePaths.find(p => {
//                        if (typeof p === 'string') return p.includes(keyword);
//                        if (typeof p === 'object' && p.name && p.url) return p.name.includes(keyword);
//                        return false;
//                    });
//                    return typeof found === 'string' ? found : found?.url || null;
//                };
//                arrSaveData.forEach(function (item) {
//                    if (item.SignDG_NgDeXuat === null && signQC) {
//                        item.SignDG_NgDeXuat = findImage('NgDeXuat');
//                    }
//                    if (item.SignDG_SoatXet === null && signMer) {
//                        item.SignDG_SoatXet = findImage('SoatXet');
//                    }
//                    if (item.SignDG_TongGiamDoc === null && signKQ) {
//                        item.SignDG_TongGiamDoc = findImage('TongGiamDoc');
//                    }
//                });

//                SaveDanhGia(arrSaveData);
//            },
//            error: function (xhr, status, err) {
//                console.error('Upload error:', err);
//                DevExpress.ui.notify('Không thể lưu chữ ký. Vui lòng thử lại!', 'error', 2000);
//            }
//        });
//    } else {
//        SaveDanhGia(arrSaveData);
//    }
//}

function saveDanhGiaNCC() {
    const grid = $("#gridPhieuDanhGia").dxDataGrid('instance');
    if (!grid) {
        DevExpress.ui.notify('Không tìm thấy dữ liệu đánh giá!', 'warning', 2000);
        return;
    }

    const ngayDG = $("#ngay-danh-gia").val();
    const { year, quarter } = getYearAndQuarter(ngayDG);
    const visibleRows = grid.getVisibleRows();

    if (visibleRows.length === 0) {
        DevExpress.ui.notify('Không có dữ liệu để lưu!', 'warning', 2000);
        return;
    }


    const maNhaCCMap = {};
    for (let i = 0; i < visibleRows.length; i++) {
        const row = visibleRows[i];
        const ma = row.data.MaNhaCC;

        if (!ma) {
            DevExpress.ui.notify(`Vui lòng chọn nhà cung cấp ở dòng ${i + 1}`, "error", 6000);
            return;
        }

        if (!maNhaCCMap[ma]) maNhaCCMap[ma] = [];
        maNhaCCMap[ma].push(i + 1);
    }

    const duplicates = Object.keys(maNhaCCMap)
        .filter(ma => maNhaCCMap[ma].length > 1)
        .map(ma => ({
            maNhaCC: ma,
            lines: maNhaCCMap[ma]
        }));

    if (duplicates.length > 0) {
        let errorMsg = "Không thể lưu vì có nhà cung cấp bị trùng:\n";
        duplicates.forEach(d => {
            errorMsg += `• Nhà cung cấp bị trùng các dòng: ${d.lines.join(", ")}\n`;
        });
        DevExpress.ui.notify(errorMsg, "error", 6000);
        return;
    }


    let dataUpload = [];
    let arrSaveData = [];

    const maPhieuDG = MaPhieu || "NONE";
    const ngayDanhGia = formatDateSQL($("#ngay-danh-gia").val());
    const nguoiDanhGia = userName;
    const ghiChu = null;

    // Kết quả radio
    const kqNgDeXuat = $('input[name="result-nguoi-de-xuat"]:checked').val() === 'pass' ? true :
        $('input[name="result-nguoi-de-xuat"]:checked').val() === 'fail' ? false : null;

    const kqSoatXet = $('input[name="result-soat-xet"]:checked').val() === 'pass' ? true :
        $('input[name="result-soat-xet"]:checked').val() === 'fail' ? false : null;

    const kqTongGiamDoc = $('input[name="result-tong-giam-doc"]:checked').val() === 'pass' ? true :
        $('input[name="result-tong-giam-doc"]:checked').val() === 'fail' ? false : null;

    // Ý kiến
    const yKienQC = $("#txtDG_QC").val() || "";
    const yKienMer = $("#txtDG_Mer").val() || "";
    const yKienKQ = $("#txtDG_KQ").val() || "";

    // Chữ ký
    const signQC = getSignatureBase64('signQC');
    const signMer = getSignatureBase64('signMer');
    const signKQ = getSignatureBase64('signXN_KQ');

    const getFileName = `DG-${maPhieuDG}`;

    if (signQC) dataUpload.push({ img: signQC, name: 'NgDeXuat' });
    if (signMer) dataUpload.push({ img: signMer, name: 'SoatXet' });
    if (signKQ) dataUpload.push({ img: signKQ, name: 'TongGiamDoc' });

    // Duyệt các dòng để tạo arrSaveData
    for (const row of visibleRows) {
        const rowData = row.data;
        Object.keys(rowData).forEach(key => {
            if (/@TieuChi@/.test(key)) {
                const parts = key.split('@TieuChi@');
                const tieuChiID = parts[0] || "";
                const diemValue = rowData[key] || "";

                const objSave = {
                    ID: rowData.ID || 0,
                    MaCLVTID: Action == "edit" ? MaCLVTID : rowSelected_CLVT.MaCLVT,
                    Quy: quarter,
                    Nam: year,
                    NgayDanhGia: ngayDanhGia,
                    NguoiDanhGia: rowData.NguoiDanhGia,
                    GhiChu: rowData.GhiChu,
                    KQ_DG_NgDeXuat: kqNgDeXuat,
                    SignDG_NgDeXuat: signQC ? null : (rowData.SignDG_NgDeXuat || null),
                    YKien_NgDeXuat: yKienQC,
                    NgayKy_NgDeXuat: signQC ? formatDateSQL(new Date()) : null,
                    KQ_DG_SoatXet: kqSoatXet,
                    SignDG_SoatXet: signMer ? null : (rowData.SignDG_SoatXet || null),
                    YKien_SoatXet: yKienMer,
                    NgayKy_SoatXet: signMer ? formatDateSQL(new Date()) : null,
                    KQ_DG_TongGiamDoc: kqTongGiamDoc,
                    SignDG_TongGiamDoc: signKQ ? null : (rowData.SignDG_TongGiamDoc || null),
                    YKien_TongGiamDoc: yKienKQ,
                    NgayKy_TongGiamDoc: signKQ ? formatDateSQL(new Date()) : null,
                    MaPhieuDG: maPhieuDG,
                    LoaiDanhGia: LoaiDanhGia,
                    TieuChiID: tieuChiID,
                    DiemID: diemValue || null,
                    MaNhaCC: rowData.MaNhaCC,
                    ThuTuUuTien: rowData.ThuTuUuTien,
                    KetQua: rowData.KetQua || "",
                    TenPhieu: $("#txt-phieu-dg").val(),
                    NgayDanhGiaPhieu: ngayDanhGia,
                    NguoiDanhGiaPhieu: rowData.NguoiDanhGia,
                };
                arrSaveData.push(objSave);
            }
        });
    }

    console.table(arrSaveData);

    // ================= SAVE =================
    const SaveDanhGiaFinal = (finalData) => {
        SaveDanhGia(finalData);
    };

    if (dataUpload.length > 0) {
        $.ajax({
            url: `/api/PhieuDanhGiaNhaCC/UploadSignature?getFileName=${encodeURIComponent(getFileName)}`,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataUpload),
            success: function (imagePaths) {
                const findImage = (keyword) => {
                    const found = imagePaths.find(p => {
                        if (typeof p === 'string') return p.includes(keyword);
                        if (typeof p === 'object' && p.name && p.url) return p.name.includes(keyword);
                        return false;
                    });
                    return typeof found === 'string' ? found : found?.url || null;
                };

                arrSaveData.forEach(function (item) {
               
                    item.NguoiSoatXet = IsSoatXet == true ? userName : null;
                    if (item.SignDG_NgDeXuat === null && signQC) {
                        item.SignDG_NgDeXuat = findImage('NgDeXuat');
                    }
                    if (item.SignDG_SoatXet === null && signMer) {
                        item.SignDG_SoatXet = findImage('SoatXet');
                    }
                    if (item.SignDG_TongGiamDoc === null && signKQ) {
                        item.SignDG_TongGiamDoc = findImage('TongGiamDoc');
                    }
                });

                SaveDanhGiaFinal(arrSaveData);
            },
            error: function () {
                DevExpress.ui.notify('Không thể lưu chữ ký. Vui lòng thử lại!', 'error', 3000);
            }
        });
    } else {
        SaveDanhGiaFinal(arrSaveData);
    }
}

function SaveDanhGia(data) {
    fetch('/api/PhieuDanhGiaNhaCC/PostPhieuDG', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.ok ? res.json() : Promise.reject('Lỗi server'))
        .then(() => {
            DevExpress.ui.notify('Lưu đánh giá thành công!', 'success', 1500);
            $("#gridPhieuDanhGia").dxDataGrid('instance').refresh();
            window.location.assign("/POMuaHang/TongQuanDanhGia");
        })
        .catch(error => {
            console.error('Save error:', error);
            DevExpress.ui.dialog.alert('Lưu thất bại: ' + error, 'Lỗi');
        });
}

$("#btn-ls-danh-gia").on("click", function () {

    window.location.assign("/POMuaHang/TongQuanDanhGia");

})

$("#btn-refresh").on("click", function () {

    if (Action != 'edit') {
        Action = "add"
        GeneratePhieuDanhGia();
        GetCountPhieuDanhGia(rowSelected_CLVT.MaCLVT, rowSelected_CLVT.ChungLoaiVatTu)
        GetPhieuDanhGia(rowSelected_CLVT.MaCLVT);
    }
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

$("#btn-Excel-export").on("click", function () {
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

$(document).ready(function () {
    GetChungLoaiVatTu();
    InitComponent();


});


