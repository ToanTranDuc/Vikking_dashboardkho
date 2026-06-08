let arrTieuChiDGia = [];
let arrChungLoaiVT = [];
let arrDiemDanhGia = [];
let arrNhaCungCap = [];

let TenPhieu = "";
let LoaiDanhGia = "DanhGia";

let rowSelected_CLVT = null;

let userName = localStorage.getItem("username1");
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

function GetVersion() {
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetVersion`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            //ResetVal('ChungLoaiVT');
            if (data.length > 0) {     
                fetchVersion(data)
            }

        }
    });
}
function fetchVersion(data) {
    $('#select-version').empty();
    var selectedVer = "";
    $.each(data, function (index, item) {
        if (item.IsActive == true) {
            selectedVer = item.Version
        }
        $('#select-version').append(
            `<option value="${item.Version}">${item.VersionView}</option>`
        );
    });
    $('#select-version').val(selectedVer);
    GetPhieuDanhGia(selectedVer)
}


$('#select-version').on("change", function () {
    const Version = $('#select-version').val();
    if (Version) {   
        GetPhieuDanhGia(Version);
    }
})

function GetPhieuDanhGia(Version) {
    $.ajax({
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetTQDanhGiaNoiLV&para1=${Version}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            fetchPhieuDanhGia(data)

        }
    });
}
function fetchPhieuDanhGia(data) {
    $("#gridPhieuDanhGia").dxDataGrid({
        dataSource: data,
        keyExpr: "ID",
        editing: {
            mode: "cell",
            allowUpdating: false,
            allowAdding: false,
            allowDeleting: false
        },
        focusedRowEnabled: true,
        hoverStateEnabled: true,
        columns: [
            { dataField: "NgayDG", caption: "Ngày đánh giá", minWidth: 120, allowEditing: false },
            { dataField: "NguoiDanhGia", caption: "Người đánh giá", minWidth: 120, allowEditing: false },
            {
                dataField: "TenPhieuDG",
                caption: "Phiếu đánh giá",
                minWidth: 100,
                allowEditing: false
            },
            { dataField: "Version", caption: "Phiên bản đánh giá", allowEditing: false, visible: false },
            { dataField: "MaNCC", caption: "Mã NCC", visible: false, allowEditing: false },
            { dataField: "UserID_DG", caption: "UserID_DG", minWidth: 100, visible: false },
            {
                dataField: "TenNhaCC",
                caption: "Nhà cung cấp",
                minWidth: 200,
                groupIndex: 0,
                groupCellTemplate: function (cellElement, cellInfo) {
                    cellElement.html(`<span style="color: teal; font-weight: bold;">${cellInfo.value}</span>`);
                }
            },
            { dataField: "MaPhieuDG", caption: "Mã phiếu", minWidth: 100, visible: false },
            { dataField: "SoTieuChi", caption: "Tổng tiêu chí đánh giá", minWidth: 90, width: 90, alignment: "center", allowEditing: false },
            { dataField: "SoTCKhongApDung", caption: "Tổng tiêu chí không áp dụng", minWidth: 90, width: 90, alignment: "center", allowEditing: false },
            { dataField: "TongDiemDG", caption: "Tổng điểm tối đa", minWidth: 90, width: 90, alignment: "center", allowEditing: false },
            { dataField: "DiemKhongApDung", caption: "Tổng điểm không áp dụng", minWidth: 90, width: 90, alignment: "center", allowEditing: false },
            { dataField: "DiemDG", caption: "Tổng điểm đạt được", minWidth: 90, width: 90, alignment: "center", allowEditing: false },
            {
                caption: "Kết quả đánh giá",
                alignment: "center",
                columns: [
                    {
                        dataField: "DanhGia_Pass",
                        caption: "Đạt",
                        alignment: "center",
                        width: 100,
                        cellTemplate: function (container, options) {
                            const mainWrapper = $("<div>").addClass("result-checkboxes-container");
                            const passWrapper = $("<div>").addClass("checkbox-item checkbox-pass-danhgia");

                            $("<div>")
                                .dxCheckBox({
                                    value: options.data.DanhGia_Pass,
                                    readOnly: !allowXacNhan,
                                    onValueChanged: function (e) {
                                        if (!allowXacNhan) return;
                                        const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
                                        const rowIndex = options.rowIndex;
                                        const rowData = options.data;

                                        grid.cellValue(rowIndex, "DanhGia_Pass", e.value);
                                        grid.cellValue(rowIndex, "DanhGia_Fail", !e.value);
                                        grid.refresh();

                                     
                                        $.ajax({
                                            type: "POST",
                                            url: `/api/PhieuDanhGiaNhaCC/UpdateKetLuan?action=UpdateKLDanhGiaNoiLV&para1=${rowData.MaPhieuDG}&para2=${rowData.MaNCC}&para3=true&para4=NONE&para5=NONE`,
                                            success: function () {
                                                DevExpress.ui.notify("Cập nhật kết luận thành công", "success", 2000);
                                            },
                                            error: function () {
                                                DevExpress.ui.notify("Có lỗi khi cập nhật kết luận", "error", 2000);
                                            }
                                        });
                                    }
                                })
                                .appendTo(passWrapper);

                            passWrapper.appendTo(mainWrapper);
                            mainWrapper.appendTo(container);
                        }
                    },
                    {
                        dataField: "DanhGia_Fail",
                        caption: "Không Đạt",
                        alignment: "center",
                        width: 100,
                        cellTemplate: function (container, options) {
                            const mainWrapper = $("<div>").addClass("result-checkboxes-container");
                            const failWrapper = $("<div>").addClass("checkbox-item checkbox-fail-danhgia");

                            $("<div>")
                                .dxCheckBox({
                                    value: options.data.DanhGia_Fail,
                                    readOnly: !allowXacNhan,
                                    onValueChanged: function (e) {
                                        if (!allowXacNhan) return;
                                        const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");
                                        const rowIndex = options.rowIndex;
                                        const rowData = options.data;

                                        grid.cellValue(rowIndex, "DanhGia_Fail", e.value);
                                        grid.cellValue(rowIndex, "DanhGia_Pass", !e.value);
                                        grid.refresh();

                                        $.ajax({
                                            type: "POST",
                                            url: `/api/PhieuDanhGiaNhaCC/UpdateKetLuan?action=UpdateKLDanhGiaNoiLV&para1=${rowData.MaPhieuDG}&para2=${rowData.MaNCC}&para3=false&para4=NONE&para5=NONE`,
                                            success: function () {
                                                DevExpress.ui.notify("Cập nhật kết luận thành công", "success", 2000);
                                            },
                                            error: function () {
                                                DevExpress.ui.notify("Có lỗi khi cập nhật kết luận", "error", 2000);
                                            }
                                        });
                                    }
                                })
                                .appendTo(failWrapper);

                            failWrapper.appendTo(mainWrapper);
                            mainWrapper.appendTo(container);
                        }
                    }
                ]
            }
        ],
        noDataText: "Chưa có đánh giá",
        toolbar: {
            items: [
                {
                    location: 'after',
                    widget: 'dxTextBox',
                    options: {
                        width: 200,
                        placeholder: 'Tìm kiếm...',
                        mode: 'text',
                        showClearButton: true,
                        valueChangeEvent: 'keyup input',
                        onValueChanged: function (e) {
                            const value = (e.value || "").toLowerCase();
                            const grid = $("#gridPhieuDanhGia").dxDataGrid("instance");

                            grid.clearFilter();

                            if (!value) return;

                            const columns = grid.getVisibleColumns();
                            const filterExpr = [];

                            filterExpr.push(function (data) {
                                let match = false;

                                for (const col of columns) {
                                    const field = col.dataField;

                                    if (!field) continue;

                                    // Xử lý riêng cột ngày
                                    if (field === "NgayDG") {
                                        const d = new Date(data[field]);
                                        const day = d.getDate().toString().padStart(2, '0');       // "22"
                                        const month = (d.getMonth() + 1).toString().padStart(2, '0'); // "01"
                                        const year = d.getFullYear().toString();                   // "2025"

                                        const fullDateStr = `${day}-${month}-${year}`.toLowerCase();

                                        if (fullDateStr.includes(value)) {
                                            match = true;
                                            break;
                                        }
                                    }   

                                    // Cột thường dạng chuỗi
                                    else if (typeof data[field] === "string" || typeof data[field] === "number") {

                                        if (data[field].toLowerCase().includes(value) && !isSignatureImage(data[field])) {
                                            match = true;
                                            break;
                                        }
                                    }
                                }

                                return match;
                            });

                            grid.filter([filterExpr[0], "=", true]);
                        }


                    }
                }

            ]

        },
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        columnAutoWidth: true,
        scrolling: { mode: "standard" },
        paging: { enabled: false },
        rowAlternationEnabled: true,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: true },
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
    window.location.assign("/POMuaHang/PhieunDanhGiaNoiLamViec?actionType=" + Action + "&MaPhieu=" + MaPhieu + "&MaNhaCC=NONE" + "&TenPhieu=NONE" + "&NguoiTao=NONE");
});

$("#btn-sua-phieu").on("click", function () {
    let Action = "edit";
    var focusedRowData = getFocusedMaPhieu();
    let MaPhieu = focusedRowData ? focusedRowData.MaPhieuDG : "NONE";
    let MaNhaCC = focusedRowData ? focusedRowData.MaNCC : "NONE";
    if (MaPhieu === "NONE") {
        DevExpress.ui.notify("Vui lòng chọn phiếu để sửa", "warning", 2000);
        return;
    }
    window.location.assign("/POMuaHang/PhieunDanhGiaNoiLamViec?actionType=edit"
       
        + "&MaPhieu=" + MaPhieu
        + "&MaNhaCC=" + MaNhaCC
        + "&TenPhieu=" + focusedRowData.TenPhieuDG
        + "&NguoiTao=" + focusedRowData.UserID_DG);

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
                fetch(`/api/PhieuDanhGiaNhaCC/Delete?action=DeleteDGNoiLV&Para1=${MaPhieu}`, { method: 'DELETE' })
                    .then(response => {
                        if (!response.ok) throw new Error("Lỗi khi xoá");
                        DevExpress.ui.notify('Đã xoá thành công.', 'success', 1500);
                        const Version = $('#select-version').val();
                        if (Version) {
                            GetPhieuDanhGia(Version);
                        }

                    })
                    .catch(() => DevExpress.ui.dialog.alert('Xoá thất bại. Vui lòng thử lại.', 'Lỗi'));
            }
        });

})


$("#btn-refresh").on("click", function () {
    const Version = $('#select-version').val();
    if (Version) {
        GetPhieuDanhGia(Version);
    }
})

$("#btn-Excel").on("click", function () {

    var focusedRowData = getFocusedMaPhieu();
    let MaPhieu = focusedRowData ? focusedRowData.MaPhieuDG : "NONE";
    let MaNhaCC = focusedRowData ? focusedRowData.MaNCC : "NONE";
    if (MaPhieu === "NONE") {
        DevExpress.ui.notify("Vui lòng chọn phiếu để sửa", "warning", 2000);
        return;
    }
    var url = `/api/PhieuDanhGiaNhaCC/ExportPhieuDGNoiLV?para1=${MaPhieu}&para2=${MaNhaCC}&para3=${focusedRowData.TenNhaCC}&para4=${focusedRowData.NguoiDanhGia}`;

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

$(document).ready(async function () {
  
    //GetPhieuDanhGia()
    GetVersion();
    CheckPerminsion();
});