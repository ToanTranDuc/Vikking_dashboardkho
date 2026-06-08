let arrTieuChiDGia = [];
let arrChungLoaiVT = [];
let arrDiemDanhGia = [];
let arrNhaCungCap = [];
let arrNhanVien = [];


let LoaiDanhGia = "DanhGia";

let rowSelected_NhaCC = null;

const userName = localStorage.getItem("username1");
const ModuleID = 'M.22.00.00'
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
                setPermission("group-box", allowXacNhan)
            }
            else {
                allowXacNhan = false;
                setPermission("group-box", allowXacNhan)
               
            }
        }
    });
}
function setPermission(className, hasPermission) {
    const selector = `.${className}`;
    if (hasPermission) {
        $(selector + " input[type='radio']").prop('disabled', false);
        $(selector).css({
            'opacity': '1',
            'cursor': 'default'
        });
    } else {

        $(selector + " input[type='radio']").prop('disabled', true);      
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
    $("#selectNhaCC").select2();
    GetTenNhanVien();
    GetNhaCungCap();
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
    $("#selectNhaCC").prop("disabled", false)
    if (actionType == 'edit') {
        $("#selectNhaCC").prop("disabled", true)
        $("#selectNhaCC").val(MaNhaCC);
        GetPhieuDanhGia(MaNhaCC);
        $("#txt-phieu-dg").val(TenPhieu);
        $("#txt-nguoi-danh-gia").val(FindTenNhanVien(NguoiTao))
    } else if (actionType == "add") {
        $("#txt-nguoi-danh-gia").val(FindTenNhanVien(userName));
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

function GetNhaCungCap() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetNCC&para1=ALL`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            //ResetVal('ChungLoaiVT');
            if (data.length > 0) {
                arrNhaCungCap = [...data]
                fetchNhaCC(data)
            }

        }
    });
}
function fetchNhaCC(data) {
    $('#selectNhaCC').empty();
    $.each(data, function (index, item) {
        $('#selectNhaCC').append(
            `<option value="${item.MaNhaCC}">${item.TenNCC}</option>`
        );
    });
    $('#selectNhaCC').val("");
}
$('#selectNhaCC').on("change", function () {
    const MaNhaCC = $('#selectNhaCC').val();
    if (MaNhaCC) {
        rowSelected_NhaCC = arrNhaCungCap.find(x => x.MaNhaCC == MaNhaCC);
        if (actionType == "add") {
            GetCountPhieuDanhGia(MaNhaCC, rowSelected_NhaCC.TenNCC);
            GeneratePhieuDanhGia();
        }
       
        GetPhieuDanhGia(MaNhaCC);     
    }
})


function GetCountPhieuDanhGia(MaNhaCC, TenNCC) {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetCountPhieuDGLV&para1=${MaNhaCC}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            var countPhieu = 1
            if (data.length > 0) {
                countPhieu = data[0].CountPhieu
            }
            TenPhieu = `PDGLV|${TenNCC}|${getDateTimeNow()}|${countPhieu}`
            $("#txt-phieu-dg").val(TenPhieu);
        }
    });
}

function GeneratePhieuDanhGia() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GeneratePhieuDGLV`,
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
                if (actionType == 'add') {
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
    return objNhanVien == null ? userName : objNhanVien.TenNV;
}

function GetPhieuDanhGia(MaNhaCC) {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetPhieuDGNoiLV&para1=${MaPhieu}&para2=${MaNhaCC}&para3=${actionType}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            if (data.length > 0) {               
               
               
                if (actionType == 'add') {
                    document.getElementById("radio-pass").checked = false;
                    document.getElementById("radio-fail").checked = true;
                }
                else if (actionType == 'edit') {
                    if (data[0].KetQuaDat == true) {
                        document.getElementById("radio-pass").checked = true;
                        document.getElementById("radio-fail").checked = false;
                    } else if (data[0].KetQuaDat == false) {
                        document.getElementById("radio-fail").checked = true;
                        document.getElementById("radio-pass").checked = false;
                    }
                }

                fetchPhieuDanhGia(data);
               
            }
        }
    });
}

function fetchPhieuDanhGia(data) {
    $("#gridPhieuDanhGia").dxDataGrid({
        dataSource: data,
        keyExpr: "ID",
        showBorders: true,
        showRowLines: true,
        showColumnLines: true,
        columnAutoWidth: true,

        groupPanel: {
            visible: false
        },

        grouping: {
            autoExpandAll: true,
            contextMenuEnabled: true
        },

        paging: {
            enabled: false
        },

        sorting: {
            mode: "multiple"
        },

        editing: {
            mode: "cell",
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: false,
            startEditAction: "click",
            selectTextOnEditStart: true
        },

        summary: {
            groupItems: [
                {
                    column: "DiemToiDa",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    alignByColumn: true,
                    showInGroupFooter: true
                },
                {
                    column: "DiemDatDuoc",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    alignByColumn: true,
                    showInGroupFooter: true
                }
            ],
            totalItems: [
                {
                    column: "TieuChi",
                    summaryType: "count",
                    displayFormat: "Số tiêu chí đánh giá : {0}"
                },
                {
                    column: "DiemToiDa",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "DiemDatDuoc",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                }
            ]
        },

        filterRow: {
            visible: false
        },

        headerFilter: {
            visible: false
        },

        columns: [
            {
                dataField: "TenNhomDanhGia",
                caption: "Nhóm Đánh Giá",
                groupIndex: 0,
                calculateGroupValue: function (rowData) {
                    return rowData.Sort;
                },
                groupCellTemplate: function (cellElement, cellInfo) {
                    const data = cellInfo.data.collapsedItems;
                    if (data) {
                        cellElement.html(`<span style="color: brown; font-weight: bold;"> ${data[0].TenNhomDanhGia}</span>`);
                    }
                    else if (cellInfo.data.items) {
                        const items = cellInfo.data.items;
                        cellElement.html(`<span style="color: brown; font-weight: bold;">${items[0].TenNhomDanhGia}</span>`);
                    }
                },
            },

            {
                dataField: "No",
                caption: "No",
                width: 60,
                allowEditing: false,
                sortOrder: "asc",
                sortingMethod: function (value1, value2) {
                    const parseNo = (str) => {
                        if (!str) return [0];
                        return str.toString().split('.').map(num => parseInt(num) || 0);
                    };

                    const arr1 = parseNo(value1);
                    const arr2 = parseNo(value2);

                    const maxLen = Math.max(arr1.length, arr2.length);
                    for (let i = 0; i < maxLen; i++) {
                        const num1 = arr1[i] || 0;
                        const num2 = arr2[i] || 0;
                        if (num1 !== num2) {
                            return num1 - num2;
                        }
                    }
                    return 0;
                }
            },

            {
                dataField: "TieuChi",
                caption: "Tiêu Chí Đánh Giá",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    $("<div>")
                        .html(options.value)
                        .css("white-space", "normal")
                        .appendTo(container);
                }
            },

            {
                dataField: "ApDung",
                caption: "Có/không/ không áp dụng",
                width: 120,
                alignment: "center",
                allowEditing: true,

                cellTemplate: function (container, options) {
                    let iconHtml = '<i class="fa fa-minus" style="color: gray; font-size: 18px;"></i>';
                    if (options.value == "V") {
                        iconHtml = '<i class="fa fa-check" style="color: green; font-size: 18px;"></i>';
                    } else if (options.value == "X") {
                        iconHtml = '<i class="fa fa-times" style="color: red; font-size: 18px;"></i>';
                    } else {
                        iconHtml = '<i class="fa fa-minus" style="color: gray; font-size: 18px;"></i>';
                    }

                    $(container)
                        .html(iconHtml)
                        .css('cursor', 'pointer');
                },

                editCellTemplate: function (container, options) {
                    const grid = options.component;

                    const dataSource = [
                        { value: "V", icon: "fa-check", color: "green" },
                        { value: "X", icon: "fa-times", color: "red" },
                        { value: null, icon: "fa-minus", color: "gray" }
                    ];

                    const $selectBox = $("<div>").appendTo(container);

                    $selectBox.dxSelectBox({
                        dataSource: dataSource,
                        valueExpr: "value",
                        displayExpr: "value",// ✅ Bắt buộc phải có
                        value: options.value,
                        showClearButton: false,
                        searchEnabled: false,
                        dropDownOptions: {
                            width: 100
                        },

                       
                        itemTemplate: function (itemData, itemIndex, itemElement) {
                            const icon = itemData?.icon || "fa-minus";
                            const color = itemData?.color || "gray";
                            $(itemElement).html(`<i class="fa ${icon}" style="color: ${color}; font-size: 18px;"></i>`);
                        },

                     
                        fieldTemplate: function (selectedItem, fieldElement) {
                            const $container = $('<div>').css({
                                'position': 'relative',
                                'width': '100%',
                                'height': '100%',
                                'display': 'flex',
                                'align-items': 'center',
                                'justify-content': 'center'
                            });

                            // Tạo TextBox ẩn (bắt buộc)
                            const $textBox = $('<div>').dxTextBox({
                                value: null,
                                readOnly: true,
                                stylingMode: 'outlined'
                            });

                            $textBox.css({
                                'opacity': '0',
                                'position': 'absolute',
                                'width': '100%',
                                'pointer-events': 'none'
                            });

                            let icon = 'fa-minus';
                            let color = 'gray';

                            if (selectedItem) {
                                icon = selectedItem.icon;
                                color = selectedItem.color;
                            }
                         
                            const $icon = $(`<i class="fa ${icon}"></i>`).css({
                                'color': color,
                                'font-size': '18px',
                                'position': 'relative',
                                'z-index': '10'
                            });

                            $container.append($icon);
                            $container.append($textBox);
                            fieldElement.append($container);
                        },

                     
                        onValueChanged: function (e) {
                            if (e.event) { 
                                const newValue = e.value;
                                const rowData = options.data;
                                const rowKey = options.key;

                             
                                grid.beginUpdate();

                                try {
                                    
                                    options.setValue(newValue);

                                  
                                    const rowIndex = grid.getRowIndexByKey(rowKey);
                                    if (newValue === "V") {
                                        const diemToiDa = rowData.DiemToiDa || 0;
                                        grid.cellValue(rowIndex, "DiemDatDuoc", diemToiDa);
                                    } else {
                                        grid.cellValue(rowIndex, "DiemDatDuoc", 0);
                                    }
                                } finally {
                                    grid.endUpdate();
                                }

                               
                                setTimeout(() => {
                                    grid.closeEditCell();
                                  /*  grid.saveEditData();*/
                                    checkKetLuanTheoTong(grid);
                                    grid.repaintRows([rowIndex]);
                                }, 0);
                            }
                        },

                      
                        onInitialized: function (e) {
                            setTimeout(() => {
                                e.component.open();
                            }, 50);
                        }
                    });
                }
            },
            {
                dataField: "DiemToiDa",
                caption: "Điểm Tối Đa",
                dataType: "number",
                width: 100,
                allowEditing: false,
                format: {
                    type: "fixedPoint",
                    precision: 2
                }
            },

            {
                dataField: "DiemDatDuoc",
                caption: "Điểm Đạt Được",
                dataType: "number",
                width: 100,
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                
            },

            {
                dataField: "GhiChu",
                caption: "Ghi Chú",
                width: 200,
                visible: true
               
            },

            {
                dataField: "Version",
                caption: "Version",
                width: 80,
                visible: false
            },

            {
                dataField: "Sort",
                caption: "Thứ Tự",
                visible: false
            },

            {
                dataField: "NhomDanhGia",
                caption: "NhomDanhGia",
                visible: false
            },

            {
                dataField: "TieuChiNoiLVID",
                caption: "TieuChiNoiLVID",
                visible: false
            }
        ],
        onEditorPreparing: function (e) {
            const grid = e.component;

            if (e.dataField === "DiemDatDuoc") {
                const originalOnValueChanged = e.editorOptions.onValueChanged;
                const apDungValue = e.row.data.ApDung;
                //if (apDungValue !== "V") {
                   
                //    e.editorOptions.disabled = true;
                //    return;
                //}

                e.editorOptions.onValueChanged = function (args) {
                    let inputValue = args.value;

                    
                    let currentValue = parseFloat(inputValue);

                 
                    if (currentValue < 0) {
                        DevExpress.ui.notify({
                            message: "Điểm đạt được không được là số âm",
                            type: "error",
                            displayTime: 3000
                        });

                        args.component.option("value", 0);
                        return;
                    }

                  
                    const maxDiem = e.row.data.DiemToiDa || 0;

                    if (isNaN(currentValue)) {
                        DevExpress.ui.notify({
                            message: "Điểm đạt được phải là số",
                            type: "error",
                            displayTime: 3000
                        });
                        currentValue = 0;
                    }

                    if (currentValue > maxDiem) {
                        DevExpress.ui.notify({
                            message: `Điểm nhập vào không được vượt quá điểm tối đa (${maxDiem})`,
                            type: "warning",
                            displayTime: 3000
                        });

                       
                        currentValue = maxDiem;
                        args.component.option("value", currentValue);
                    }

                    
                    if (originalOnValueChanged) {
                        originalOnValueChanged({
                            value: currentValue
                        });
                    }

                 
                    setTimeout(() => {
                        checkKetLuanTheoTong(grid);
                        grid.saveEditData();
                    }, 100);
                };

               
                e.editorOptions.min = 0; 
                e.editorOptions.max = e.row.data.DiemToiDa; 
                e.editorOptions.step = 0.5; 
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

function checkKetLuanTheoTong(grid) {
    const visibleRows = grid.getVisibleRows();

    let tongDiemToiDa = 0;
    let tongDiemDatDuoc = 0;

    visibleRows.forEach(row => {      
        if (row.rowType !== "data") return;
        const data = row.data;     
        if (data.ApDung === null) return;
        tongDiemToiDa += (data.DiemToiDa || 0);
        tongDiemDatDuoc += (data.DiemDatDuoc || 0);
    });

    if (tongDiemToiDa > 0) {
        const ratio = tongDiemDatDuoc / tongDiemToiDa;
        if (ratio >= 0.7) {
            document.getElementById("radio-pass").checked = true;
        } else {
            document.getElementById("radio-fail").checked = true;
        }
    } else {
        document.getElementById("radio-fail").checked = true;
    }
}

$("#btn-save").on("click", function () {
    saveDanhGiaNCC();
});

function saveDanhGiaNCC() {
    const grid = $("#gridPhieuDanhGia").dxDataGrid('instance');
    if (!grid) {
        DevExpress.ui.notify('Không tìm thấy dữ liệu đánh giá!', 'warning', 2000);
        return;
    }
    const MaNhaCC = $("#selectNhaCC").val();
    if (!MaNhaCC) {
        DevExpress.ui.notify('Vui lòng chọn nhà cung cấp', 'warning', 2000);
        return;
    }
    const visibleRows = grid.getVisibleRows();
    let arrSaveData = [];

    let ketQuaDat;
    if (document.getElementById("radio-pass").checked)
    {
     
        ketQuaDat = true;
    } else if (document.getElementById("radio-fail").checked) {
     
        ketQuaDat = false;
    } else { 
        ketQuaDat = null;
    }
    visibleRows.forEach(row => {
        
        if (row.rowType !== "data") return;
        const data = row.data;      
        let ApDung = null;
        if (data.ApDung == "V") {
            ApDung = true;

        } else if (data.ApDung == "X") {
            ApDung = false;

        } else {
            ApDung = null;
        }

        arrSaveData.push({
            ID: data.ID,
            MaPhieuDG: MaPhieu,
            TenPhieuDG: $("#txt-phieu-dg").val(),
            KetQuaDat: ketQuaDat,
            MaNCC: MaNhaCC,
            NgayDG: formatDateSQL($("#ngay-danh-gia").val()),
            NguoiDanhGia: userName,
            Version: data.Version,
            NhomDanhGia: data.NhomDanhGia,
            TieuChiNoiLVID: data.TieuChiNoiLVID,
            ApDung: ApDung,
            DiemDatDuoc: data.DiemDatDuoc,
            GhiChu: data.GhiChu
        });
    });

 



    SaveDanhGia(arrSaveData);
}

function SaveDanhGia(data) {
    fetch('/api/PhieuDanhGiaNhaCC/PostPhieuDGNoiLV', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.ok ? res.json() : Promise.reject('Lỗi server'))
        .then(() => {
            DevExpress.ui.notify('Lưu đánh giá thành công!', 'success', 1500);
            window.location.assign("/POMuaHang/TongQuanDanhGiaNoiLamViec");
            $("#gridPhieuDanhGia").dxDataGrid('instance').refresh();
        })
        .catch(error => {
            console.error('Save error:', error);
            DevExpress.ui.dialog.alert('Lưu thất bại: ' + error, 'Lỗi');
        });
}

$("#btn-ls-danh-gia").on("click", function () {

    window.location.assign("/POMuaHang/TongQuanDanhGiaNoiLamViec");

})
$("#btn-refresh").on("click", function () {
    const MaNhaCC = $('#selectNhaCC').val();
    if (MaNhaCC) {
        rowSelected_NhaCC = arrNhaCungCap.find(x => x.MaNhaCC == MaNhaCC);
        if (actionType == "add") {
            GetCountPhieuDanhGia(MaNhaCC, rowSelected_NhaCC.TenNCC);
            GeneratePhieuDanhGia();
        }

        GetPhieuDanhGia(MaNhaCC);
    }
    if (actionType == "edit") {
        CheckPerminsion();
    }
})
$("#btn-Excel").on("click", function () {
    
    let NguoiDG = $("#txt-nguoi-danh-gia").val();
    let obj = arrNhaCungCap.find(x => x["MaNhaCC"] == $('#selectNhaCC').val());
    var url = `/api/PhieuDanhGiaNhaCC/ExportPhieuDGNoiLV?para1=${MaPhieu}&para2=${$('#selectNhaCC').val()}&para3=${obj.TenNCC}&para4=${NguoiDG}`;

    
    const now = new Date();
    const pad = n => n.toString().padStart(2, '0');
    var fileName = `BM13_QT15-${pad(now.getDate())}${pad(now.getMonth() + 1)}${now.getFullYear()}${pad(now.getHours())}${pad(now.getMinutes())}${pad(now.getSeconds())}.xlsx`;

    
    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: null
    })
        .then(response => {
            if (!response.ok) {
                alert("Lỗi: Không thể xuất Excel. Vui lòng thử lại!");
                return null;
            }
            return response.blob();
        })
        .then(blob => {
            if (!blob) return;
            var a = document.createElement("a");
            var objectUrl = window.URL.createObjectURL(blob);
            a.href = objectUrl;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(objectUrl);
        })
        .catch(error => {
            console.error("Error fetching data from the server.", error);
        });
});

$("#toggle-card").on("click", function () {
    const $content = $(".collapse-content");
    $(this).toggleClass("collapsed");
    $content.toggleClass("collapsed");

    if ($(this).hasClass("collapsed")) {
        $(this).html('<i class="fas fa-plus"></i>');
    } else {
        $(this).html('<i class="fas fa-minus"></i>');
    }
});

$(document).ready(function () {
    InitComponent();

    if (actionType == "edit") {
        CheckPerminsion();
    }
});
