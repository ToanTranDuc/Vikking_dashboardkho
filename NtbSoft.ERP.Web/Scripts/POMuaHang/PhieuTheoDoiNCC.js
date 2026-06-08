let arrTieuChiDGia = [];
let arrChungLoaiVT = [];
let arrDiemDanhGia = [];
let arrNhaCungCap = [];
let arrNhanVien = [];

let TenPhieu = "";
let LoaiDanhGia = "TheoDoi";

let rowSelected_CLVT = null;

const userName = localStorage.getItem("username1");
const ModuleID = 'M.23.00.00'
let IsSoatXet = false;
let allowXacNhan = true;

function CheckPerminsion() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=CheckPerminsion&para1=${userName}&para2=${ModuleID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {
                const permissions = data[0];
                allowXacNhan = permissions.AllowXacNhan === 1 || permissions.AllowXacNhan === true;

            }
            else {
                allowXacNhan = false;


            }
        }
    });
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
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetCountPhieuDanhGia&para1=${MaVTID}&para2={LoaiDanhGia}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            var countPhieu = 1
            if (data.length > 0) {
                countPhieu = data[0].CountPhieu
            }
            TenPhieu = `PTD|${TenCL}|${getDateTimeNow()}|${countPhieu}`
            $("#txt-phieu-dg").val(TenPhieu);
        }
    });
}

function GeneratePhieuDanhGia() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GeneratePhieuTheoDoi`,
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
                if (Action == "edit") {
                    $("#txt-phieu-dg").val(data[0].TenPhieu)
                    $("#txt-nguoi-danh-gia").val(FindTenNhanVien(data[0].NguoiDanhGiaPhieu || userName))
                    $('#ngay-danh-gia').val(formatDateView(data[0].NgayDanhGiaPhieu));
                }
            }


        }
    });

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
                    onFocusIn: function (e) {
                        e.component.open();
                    }
                },
               
                
            });

            return acc;
        }, []);

    $("#gridPhieuDanhGia").dxDataGrid({
        dataSource: data,
        editing: {
            mode: "batch",  // Bật batch mode
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: true,
            confirmDelete: false,
            //startEditAction: "click",
            //selectTextOnEditStart: true
        },
        columns: [
            {
                dataField: "NgayDanhGia",
                caption: "Ngày tháng năm",
                dataType: "date",
                format: "dd-MM-yyyy",
                editorType: "dxDateBox",
                editorOptions: {
                    type: "date",
                    onFocusIn: function (e) {
                        e.component.open();

                    }
                },
                minWidth: 150,
                width: 150,
                
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
                    searchEnabled: false,
                    acceptCustomValue: false,
                    placeholder: "Chọn nhà cung cấp",
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
                allowEditing: false,
                //setCellValue: function (newData, value, currentRowData) {
                //    let sum = tinhTong(currentRowData);
                //    newData.TongDiemUuTien = sum;
                //}
            },
         
            {
                dataField: "GhiChu", caption: "Ghi chú về sự không phù hợp", minWidth: 120,
                width: 120, },

            {
                caption: "Kết quả",
                alignment: "center",
                width: 120,
                minWidth: 120,
                columns: [
                    {
                        dataField: "KetQuaPass",
                        caption: "Tiếp tục",
                        alignment: "center",
                        allowFiltering: false,
                        width: 60,
                        minWidth: 60,
                        cellTemplate: function (container, options) {
                            const mainWrapper = $("<div>").addClass("result-checkboxes-container");
                            const passWrapper = $("<div>").addClass("checkbox-item checkbox-pass");

                            $("<div>")
                                .dxCheckBox({
                                    value: options.data.KetQuaPass,
                                    readOnly: Action == "add" ?  false :!allowXacNhan && Action == "edit",
                                    onValueChanged: function (e) {
                                        if (!allowXacNhan && Action == "edit") return;
                                        const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
                                        const rowIndex = options.rowIndex;

                                        // Gán giá trị vào grid
                                        grid.cellValue(rowIndex, "KetQuaPass", e.value);
                                        grid.cellValue(rowIndex, "KetQuaFail", !e.value);


                                        grid.saveEditData();
                                    }
                                })
                                .appendTo(passWrapper);

                            passWrapper.appendTo(mainWrapper);
                            mainWrapper.appendTo(container);
                        }
                    },
                    {
                        dataField: "KetQuaFail",
                        caption: "Đổi NCC",
                        alignment: "center",
                        allowFiltering: false,
                        width: 60,
                        minWidth: 60,
                        cellTemplate: function (container, options) {
                            const mainWrapper = $("<div>").addClass("result-checkboxes-container");
                            const failWrapper = $("<div>").addClass("checkbox-item checkbox-fail");

                            $("<div>")
                                .dxCheckBox({
                                    value: options.data.KetQuaFail,
                                    readOnly: Action == "add" ? false : !allowXacNhan && Action == "edit",
                                    onValueChanged: function (e) {
                                        if (!allowXacNhan && Action == "edit") return;
                                        const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
                                        const rowIndex = options.rowIndex;

                                        grid.cellValue(rowIndex, "KetQuaFail", e.value);
                                        grid.cellValue(rowIndex, "KetQuaPass", !e.value);


                                        grid.saveEditData();
                                    }
                                })
                                .appendTo(failWrapper);

                            failWrapper.appendTo(mainWrapper);
                            mainWrapper.appendTo(container);
                        }
                    }
                ]
            },


            {
                dataField: "NguoiDanhGia", caption: "Người đánh giá" ,minWidth: 150,width: 150 },
            {
                type: "buttons",
                caption: "Xóa",
                minWidth: 60,
                width: 60,
                buttons: [{
                    hint: "Xoá",
                    icon: "trash",
                    onClick: function (e) {
                        const data = e.row.data;
                        const grid = e.component;

                        const removeRowAndCheckRedirect = () => {
                            const rowIndex = e.row.rowIndex;
                            if (rowIndex < 0) return;

                            
                            grid.cancelEditData();

                          
                            grid.deleteRow(rowIndex);

                           
                            grid.saveEditData();

                          
                            if (grid.getVisibleRows().length === 1) {
                                window.location.assign("/POMuaHang/TongQuanPhieuTheoDoi");
                            }
                        };



                       
                        if (data.IsEdit == true) {
                            removeRowAndCheckRedirect();
                            return;
                        }

                        DevExpress.ui.dialog.confirm(
                            'Bạn có chắc muốn xoá dòng này?',
                            'Xác nhận xoá'
                        ).done(function (res) {
                            if (!res) return;

                           
                            removeRowAndCheckRedirect();

                          
                            fetch(`/api/PhieuDanhGiaNhaCC/Delete?action=DeleteNhaCCTheoDoi&Para1=${MaPhieu}&Para2=${data.MaNhaCC}&Para3=${ConvertDateSave(data.NgayDanhGia)}`, {
                                method: 'DELETE'
                            })
                                .then(response => {
                                    if (!response.ok) throw new Error();
                                    DevExpress.ui.notify('Đã xoá thành công.', 'success', 1500);
                                })
                                .catch(() => {
                                    DevExpress.ui.dialog.alert('Xoá thất bại. Dữ liệu sẽ được tải lại.', 'Lỗi');
                                    grid.refresh(); // rollback
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
        scrolling: { mode: "standard" },
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
            } else if (e.rowType === "data" && e.column.dataField && e.column.dataField == "TongDiemUuTien") {
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
           
            if (e.parentType === "dataRow" && e.dataField == "MaNhaCC" || e.dataField == "NgayDanhGia") {
               
                if (e.row.data.IsEdit !== true) {
                    e.editorOptions.disabled = true;
                    e.editorOptions.readOnly = true;
                }

                
                let originalOnValueChanged = e.editorOptions.onValueChanged;

               

                e.editorOptions.onValueChanged = function (args) {
                 
                    if (originalOnValueChanged) {
                        originalOnValueChanged(args);
                    }

                    const rowData = e.row.data;
                    const grid = e.component;
                    const dataSource = grid.getDataSource().items();
                    const currentRowIndex = e.row.rowIndex;
                    //const selectedValue = args.value;

                  
                    const duplicateRows = dataSource
                        .map((row, idx) => ({ row, idx }))
                        .filter(item => {
                            
                            const itemDate = item.row.NgayDanhGia;
                            const currentDate = rowData.NgayDanhGia;

                            if (!itemDate || !currentDate) return false;

                          
                            return item.row.MaNhaCC === rowData.MaNhaCC &&
                                ConvertDateSave(itemDate) === ConvertDateSave(currentDate) &&
                                item.idx !== currentRowIndex;
                        });

                  
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

        
            if (e.parentType === "dataRow" && e.editorOptions && e.dataField.includes("@TieuChi@")) {
                let originalOnValueChanged = e.editorOptions.onValueChanged;
               

                e.editorOptions.onValueChanged = function (args) {
                    if (originalOnValueChanged) {
                        originalOnValueChanged(args);
                    }

                    let rowData = e.row.data;
                    const grid = e.component;
                    const rowIndex = e.row.rowIndex;

                    rowData[e.dataField] = args.value;

                    let sum = tinhTong(rowData);
                    rowData.TongDiemUuTien = sum;

                    let hasBelow3 = Object.keys(rowData).some(field => {
                        if (field.includes("@TieuChi@") && rowData[field]) {
                            let diem = parseInt(rowData[field].split("_")[1]);
                            return diem < 3;
                        }
                        return false;
                    });

                    let isPass = sum >= 18 && !hasBelow3;
                    rowData.KetQuaPass = isPass;
                    rowData.KetQuaFail = !isPass;

                    e.component.cellValue(e.row.rowIndex, "TongDiemUuTien", sum);
                    e.component.cellValue(e.row.rowIndex, "KetQuaPass", isPass);
                    e.component.cellValue(e.row.rowIndex, "KetQuaFail", !isPass);
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

$("#btn-them-nhacc").on("click", function () {
    const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
    let ds = grid.option("dataSource");
    let today = new Date();   // đúng cú pháp

    let newRow = {
        NgayDanhGia: today,   // giả sử bạn có hàm formatDateView để định dạng ngày
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

function saveDanhGiaNCC() {
    const grid = $("#gridPhieuDanhGia").dxDataGrid('instance');
    if (!grid) {
        DevExpress.ui.notify('Không tìm thấy dữ liệu đánh giá!', 'warning', 2000);
        return;
    }
  

    const visibleRows = grid.getVisibleRows();
    if (visibleRows.length === 0) {
        DevExpress.ui.notify('Không có dữ liệu để lưu!', 'warning', 2000);
        return;
    }

    //let dataUpload = [];
    let arrSaveData = [];


    const maPhieuDG = MaPhieu;
 
    const nguoiDanhGia = userName;
    //const ghiChu = null;

    const ngayDGInput = $("#ngay-danh-gia").val();
    if (!ngayDGInput) {
        DevExpress.ui.notify("Ngày đánh giá không được bỏ trống!", "error", 3000);
        return;
    }
    const ngayDG = $("#ngay-danh-gia").val();
    const { year, quarter } = getYearAndQuarter(ngayDG);
    const ngayDanhGia = formatDateSQL(ngayDGInput);
    for (let [rowIndex, row] of visibleRows.entries()) {
        const rowData = row.data;

        if (!rowData.MaNhaCC) {
            DevExpress.ui.notify(`Dòng ${rowIndex + 1}: Nhà cung cấp không được bỏ trống!`, "error", 4000);
            return; // dừng hẳn hàm
        }
       
        for (let key of Object.keys(rowData)) {
            if (/@TieuChi@/.test(key)) {
                const tieuChiID = key.split('@TieuChi@')[0] || "";
                const diemValue = rowData[key] || "";
                let KetQua = null;
                if (rowData.KetQuaFail == true) KetQua = false;
                else if (rowData.KetQuaPass == true) KetQua = true;
                const isDuplicate = arrSaveData.some(item =>
                    item.MaNhaCC == rowData.MaNhaCC &&
                    item.NgayDanhGia == ConvertDateSave(rowData.NgayDanhGia) && item.TieuChiID == tieuChiID
                );
                if (isDuplicate) {
                    DevExpress.ui.notify(
                        `Dữ liệu dòng ${rowIndex + 1} đã bị trùng`,
                        "error",
                        4000
                    );
                    return; // dừng hẳn hàm
                }

                const objSave = {
                    ID: rowData.ID || 0,
                    MaCLVTID: Action == "edit" ? MaCLVTID : rowSelected_CLVT.MaCLVT,
                    Quy: quarter,
                    Nam: year,
                    NgayDanhGia: ConvertDateSave(rowData.NgayDanhGia),
                    NguoiDanhGia: rowData.NguoiDanhGia,
                    GhiChu: rowData.GhiChu,
                    MaPhieuDG: maPhieuDG,
                    LoaiDanhGia: LoaiDanhGia,
                    TieuChiID: tieuChiID,
                    DiemID: diemValue || null,
                    MaNhaCC: rowData.MaNhaCC || "",
                    ThuTuUuTien: 0,
                    KetQua: KetQua,
                    TenPhieu: $("#txt-phieu-dg").val(),
                    NgayDanhGiaPhieu: ngayDanhGia,
                    NguoiDanhGiaPhieu: nguoiDanhGia,
                };

                arrSaveData.push(objSave);
            }
        }
    }


    SaveDanhGia(arrSaveData);
}

function ConvertDateSave(isoString) {
    const date = new Date(isoString);
  
    const year = date.getFullYear(); const month = String(date.getMonth() + 1).padStart(2, '0');
 
    const day = String(date.getDate()).padStart(2, '0');
   
    const formatted = `${year}-${month}-${day}`;
    return formatted;
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
            window.location.assign("/POMuaHang/TongQuanPhieuTheoDoi");
        })
        .catch(error => {
            console.error('Save error:', error);
            DevExpress.ui.dialog.alert('Lưu thất bại: ' + error, 'Lỗi');
        });
}

$("#btn-ls-danh-gia").on("click", function () {

    window.location.assign("/POMuaHang/TongQuanPhieuTheoDoi");

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
    var url = `/api/PhieuDanhGiaNhaCC/ExportPhieuTheoDoi?para1=${MaPhieu}&para2=${MaCLVTID}&para3=${userName}`;
    const fileName = (() => {
        const now = new Date();
        const pad = n => n.toString().padStart(2, '0');
        return `BM04/QT15-${pad(now.getDate())}${pad(now.getMonth() + 1)}${now.getFullYear()}${pad(now.getHours())}${pad(now.getMinutes())}${pad(now.getSeconds())}.xlsx`;
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
    CheckPerminsion();
});
