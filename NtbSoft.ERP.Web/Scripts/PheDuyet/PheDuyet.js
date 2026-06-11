 //UserID = localStorage.getItem("username1");
let arrSave = [];

var arrActionDetail = [
    { ModuleID: "M.12.01.00", Action: "GetPhieuCanDoiDetail", HasChild: false, IsFrm: true },
    { ModuleID: "M.12.02.00", Action: "GetPhieuBaoGiaDetail", HasChild: true, IsFrm: true },
    { ModuleID: "M.12.03.00", Action: "GetPhieuMuaHangDetail", HasChild: true, IsFrm: true },
    { ModuleID: "M.21.00.00", Action: "GetPhieuDanhGiaDetail", HasChild: false, IsFrm: false }
];

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

function initGrid() {
    $("#gridContainer").dxDataGrid({
        dataSource: [],      
        noDataText: "Không có dữ liệu để hiển thị",
        showBorders: true,
        showRowLines: true,
        showColumnLines: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        selection: {
            mode: "single"
        },
        hoverStateEnabled: false,

        headerFilter: {
            visible: true
        },
        filterRow: {
            visible: true
        },
        scrolling: {
            mode: 'standard',
            useNative: true,
            showScrollbar: 'always',
        },
        paging: {
            enabled: false,

        },
        searchPanel: {
            visible: true,
            placeholder: "Tìm kiếm..."
        },
    });

}

function generateColumns(sampleRow, ModuleID)
{ 
    if (!sampleRow) return [];
    console.log(ModuleID);
    let  columns = [
        {
            dataField: "rowIndex",
            caption: "STT",
            width: 50,          
            alignment: "center",
            allowFiltering: false,
            allowHeaderFiltering: false,
            sortIndex: 0,
            visibleIndex: 0,
            sortingMethod: function (a, b) {
                return (Number(a) || 0) - (Number(b) || 0);
            }
        }

    ];
   
    switch (ModuleID) {
        case 'overview':
            columns.push(
                { dataField: "ModuleID", caption: "ModuleID", visible: false },
                {
                    dataField: "Title",
                    caption: "Title",
                    sortIndex: 1,
                    groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {
                        const data = cellInfo.data.collapsedItems;
                        if (data) {
                            cellElement.html(`<span style="color: brown; font-weight: bold;"> ${stripIndex(data[0].Title)}</span>`);
                        }
                        else if (cellInfo.data.items) {
                            const items = cellInfo.data.items;
                            cellElement.html(`<span style="color: brown; font-weight: bold;">${stripIndex(items[0].Title)}</span>`);
                        }
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },
                {
                    dataField: "TenPhieu",
                    caption: "Tên phiếu/Đợt",
                    minWidth: 150, width: 150,
             
                },
                { dataField: "MaDH", caption: "Đơn hàng" },
                { dataField: "TenHang", caption: "Mã hàng" },
                { dataField: "MaNCC", caption: "MaNCC", visible: false },
                { dataField: "TenNhaCungCap", caption: "NCC/Khách hàng", minWidth: 120, width: 120 },
                { dataField: "NguoiTao", caption: "Người tạo" },
                { dataField: "NgayTao", caption: "Ngày tạo", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); }, allowFiltering: false },
                { dataField: "NguoiSua", caption: "Người sửa", visible: false  },
                { dataField: "NgaySua", caption: "Ngày sửa", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); }, visible: false },
                { dataField: "TrangThai", caption: "Trạng thái", minWidth: 100, width: 100, },
                {
                    caption: " ", allowFiltering: false,
                    minWidth: 110, width: 110,
                    cellTemplate: function (container, options) {

                        $("<span>").html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                            .css("cursor", "pointer")
                            .on("click", function () {
                                initDetailGrid();
                                var moduleFocused = options.data.ModuleID;
                                if (moduleFocused == "M.21.00.00") {                                
                                    ShowDanhGiaPopup(options.data);
                                } else if (moduleFocused == "M.04.02.00") {
                                    showDetailBOMPopup(options.data, moduleFocused)
                                }
                                else {
                                    showDetailPopup(options.data, moduleFocused);
                                }
                                                                                                                
                            }).appendTo(container);
                    }
                }
            );
            break;
        case 'M.12.01.00':
            columns.push(
                {
                    dataField: "TenPhieu",
                    caption: "TenPhieu",
                    groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {
                        const data = cellInfo.data.collapsedItems;
                        if (data) {
                            cellElement.html(`<span style="color: brown; font-weight: bold;"> ${data[0].TenPhieu}</span>`);
                        }
                        else if (cellInfo.data.items) {
                            const items = cellInfo.data.items;
                            cellElement.html(`<span style="color: brown; font-weight: bold;">${items[0].TenPhieu}</span>`);
                        }
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },
                { dataField: "MaDH", caption: "Đơn hàng" },
                { dataField: "TenHang", caption: "Mã hàng" },
                { dataField: "NguoiTao", caption: "Người tạo" },
                { dataField: "NgayTao", caption: "Ngày tạo", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); }, allowFiltering: false },
                { dataField: "NguoiSua", caption: "Người sửa"},
                { dataField: "NgaySua", caption: "Ngày sửa", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); }, allowFiltering: false},
                { dataField: "TrangThai", caption: "Trạng thái" },
                {
                    caption: " ", allowFiltering: false,
                    minWidth: 110, width: 110,
                    cellTemplate: function (container, options) {

                        $("<span>").html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                            .css("cursor", "pointer")
                            .on("click", function () {

                                initDetailGrid(); showDetailPopup(options.data, options.component.option("customModuleId"));
                            }).appendTo(container);
                    }
                }
            );
            break;
        case 'M.12.02.00':
            columns = [];
            columns.push(
                {
                    dataField: "TenNhaCC",
                    caption: "TenNhaCC",
                    groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {
                        const data = cellInfo.data.collapsedItems;
                        if (data) {
                            cellElement.html(`<span style="color: brown; font-weight: bold;"> ${data[0].TenNhaCC}</span>`);
                        }
                        else if (cellInfo.data.items) {
                            const items = cellInfo.data.items;
                            cellElement.html(`<span style="color: brown; font-weight: bold;">${items[0].TenNhaCC}</span>`);
                        }
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieuBG", visible: false },
                { dataField: "TenPhieu", caption: "Tên phiếu" },
                { dataField: "NgayBDHieuLuc", caption: "Ngày bắt đầu hiệu lực", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); } },
                { dataField: "NgayHetHieuLuc", caption: "Ngày kết thúc hiệu lực", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); } },
                { dataField: "KhoangTGGHDuKien", caption: "Thời gian dự kiến GH", },
                { dataField: "ThanhToan", caption: "Phương thức thanh toán" },
                {
                    dataField: "DonGia", caption: "Đơn giá cơ bản", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                {
                    dataField: "ChiPhiPhatSinh", caption: "Chi phí phát sinh", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    } },
                {
                    dataField: "DonGiaSauThueCK", caption: "Tổng chí phí sau thuế và chiết khấu", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                { dataField: "MaTienTe", caption: "Đơn vị tiền tệ", },
                { dataField: "GhiChu", caption: "Ghi chú", allowFiltering: false},
                { dataField: "TrangThai", caption: "Trạng thái" },
                {
                    caption: " ", allowFiltering: false,
                    minWidth: 110, width: 110,
                    cellTemplate: function (container, options) {

                        $("<span>").html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                            .css("cursor", "pointer")
                            .on("click", function () {

                                initDetailGrid(); showDetailPopup(options.data, options.component.option("customModuleId"));
                            }).appendTo(container);
                    }
                }
            );
            break;
        case 'M.12.03.00':
            columns.push(
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },
                { dataField: "TenPhieu", caption: "Purchase order" },
                { dataField: "TenKH", caption: "Nhà cung cấp" },
                { dataField: "NgayDuKienHV", caption: "Giao hàng dự kiến" },
                { dataField: "MaHang", caption: "Mã hàng", },
                { dataField: "MaDH", caption: "Mã ĐH", },
                { dataField: "POMua", caption: "PO mua hàng" },
                {
                    dataField: "TongCPVT", caption: "Chi phí vật tư", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                {
                    dataField: "TongCPPS", caption: "Chi phí phát sinh", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                {
                    caption: "Tổng tiền",
                    dataField: "Total",
                    calculateCellValue: function (rowData) {

                        return (rowData.TongCPVT || 0) + (rowData.TongCPPS || 0);
                    }
                    , allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }
                },
               
                { dataField: "MaTienTe", caption: "Đơn vị tiền tệ", },
                { dataField: "GhiChu", caption: "Ghi chú" , allowFiltering: false},
                { dataField: "TrangThai", caption: "Trạng thái" },
                {
                    caption: " ", allowFiltering: false,
                    minWidth: 110, width: 110,
                    cellTemplate: function (container, options) {
                        
                        $("<span>").html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                            .css("cursor", "pointer")
                            .on("click", function () {
                                
                                initDetailGrid();
                                showDetailPopup(options.data, options.component.option("customModuleId"));
                            }).appendTo(container);
                    }
                }
            );
            break;
        case 'M.21.00.00':
            columns.push(
                { dataField: "MaCLVTID", caption: "MaCLVTID", visible: false },
                {
                    dataField: "ChungLoaiVatTu", caption: "ChungLoaiVatTu", groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {
                    
                        cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.data.key}</span>`);
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },
                {
                    dataField: "TenPhieu", caption: "Tên phiếu", groupIndex: 1,
                    groupCellTemplate: function (cellElement, cellInfo) {
                        const items = cellInfo.data.items;
                        cellElement.html(`<span style="color: brown; font-weight: bold;">${items[0].TenPhieu}</span>`);
                    },
                },
                { dataField: "Quy", caption: "Quý" },
                { dataField: "Nam", caption: "Năm" },
                { dataField: "TenNCC", caption: "Nhà cung cấp", minWidth: 200, width: 250},
                /*{ dataField: "GhiChu", caption: "Ghi chú", allowFiltering: false },*/
                { dataField: "NgayDanhGiaPhieu", caption: "Ngày đánh giá" },
                { dataField: "NguoiDanhGiaPhieu", caption: "Người đánh giá", },
                { dataField: "NgayKy_SoatXet", caption: "Ngày ký soát xét" },
                { dataField: "NguoiSoatXet", caption: "Soát xét", },
                { dataField: "TrangThai", caption: "Trạng thái" },
                {
                    caption: " ", allowFiltering: false,
                    minWidth: 110, width: 110,
                    cellTemplate: function (container, options) {

                        $("<span>").html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                            .css("cursor", "pointer")
                            .on("click", function () {

                                //initDetailGrid();
                             
                                ShowDanhGiaPopup(options.data);
                            }).appendTo(container);
                    }
                }
            );
            break;
        case 'M.04.01.00':
            columns.push(
               
                { dataField: "ID", caption: "ID", visible: false },
                { dataField: "MaGop", caption: "MaGop", visible: false },
                {
                    dataField: "MaDH",
                    caption: "Đơn hàng",
                    minWidth: 120, width: 120,
                    
                    allowHeaderFiltering: true
                },              
                {
                    dataField: "TenHang",
                    caption: "Mã hàng",
                    minWidth: 100, width: 100,
                    
                    allowHeaderFiltering: true
                },
                {
                    dataField: "MaLenh",
                    caption: "Mã lệnh",
                    width: 120,
                    cssClass: "col-soluong",
                    allowHeaderFiltering: true
                },
                {
                    dataField: "Dot",
                    caption: "SeaSon",
                    width: 120,
                    
                    allowHeaderFiltering: true
                },
                {
                    dataField: "TenKH",
                    caption: "Khách hàng",
                    width: 150,
                    
                    allowHeaderFiltering: true
                },
                {
                    dataField: "TenCL",
                    caption: "Chủng loại",
                    width: 120,
                    allowHeaderFiltering: true
                },
                {
                    dataField: "SoLuong",
                    caption: "Số lượng",
                    width: 100,
                    alignment: "right",
                    format: "#,##0",
                    dataType: "number",
                    allowFiltering: false,
                    cssClass: "col-soluong"
                },
            
                {
                    dataField: "Status_LSX",
                    caption: "Trạng thái",
                    minWidth: 80, width: 80,
                    allowFiltering: false,
                    cellTemplate: function (container, options) {
                        const status = options.value;
                        let color = "";

                        if (status === "Sản xuất") color = "#28a745";
                        else if (status === "Duyệt lại") color = "#17a2b8";
                        else if (status === "Gia công") color = "#ffc107";

                        $("<div>")
                            .css({
                                "padding": "5px 10px",
                                "background-color": color,
                                "color": "white",
                                "border-radius": "4px",
                                "text-align": "center",
                                "font-weight": "500",
                                "font-size": "12px"
                            })
                            .text(status || "")
                            .appendTo(container);
                    }
                },
                {
                    caption: " ",
                    allowFiltering: false,
                    allowSorting: false,
                    minWidth: 110,
                    width: 110,
                    alignment: "center",
                    cellTemplate: function (container, options) {
                        $("<span>")
                            .html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                            .css("cursor", "pointer")
                            .on("click", function () {
                                ShowDetailDHMaLenh(options.data.MaDH, options.data.MaLenh, options.data.IsDuyetLai);
                            })
                            .appendTo(container);
                    }
                }
               
            );         
            break;
        default:

            Object.keys(sampleRow).forEach(key => {
                columns.push({
                    dataField: key,
                    caption: key,
                    minWidth: 100
                });
            });
            break;
    }

    return columns;
}

function generateToolbar(ModuleID) {
    let toolbarItems = [];
    const isSmallScreen = window.innerWidth <= 500;
    switch (true) {
        case ModuleID == "overview" && selectedTab == 0:
        case ModuleID == "M.12.01.00":
        case ModuleID == "M.12.02.00":
        case ModuleID == "M.04.02.00":
            toolbarItems.push(
                {
                    location: "after",
                    widget: "dxCheckBox",
                    options: {
                        text:  "Đã duyệt" ,
                        value: IsDuyetSelected || false,
                        onValueChanged: function (e) {
                            IsDuyetSelected = e.value;
                            if (currentModuleId === 'overview') {
                                loadOverviewData();
                            }
                            else {
                                loadModuleData(currentModuleId, e.value);

                            }
                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Duyệt",
                        icon: "check",
                        elementAttr: { class: "btn-duyet" },
                        onClick: async function (e) {
                            var focusedRowData = getFocusedRowData();
                            var ModuleID = currentModuleId == "overview" ? focusedRowData.ModuleID : currentModuleId;
                            if (ModuleID == "M.12.03.00") {
                                DuyetMuaHang(true)
                            }
                            else {
                                var title = "";
                                var detail = "";
                                var arrDuyetPhieu = [];
                              
                                let MaPhieu = "NONE";
                                
                                MaPhieu= focusedRowData ? focusedRowData.MaPhieu : "NONE";

                                if (MaPhieu === "NONE") {
                                    DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                                    return;
                                }

                               
                                let obj = {
                                    Action: "",
                                    MaPhieu: MaPhieu ,
                                    IsDuyet: true,
                                    NguoiPheDuyet: UserID,
                                    GhiChu: ""
                                };

                                switch (ModuleID) {
                                    case "M.12.01.00":
                                        obj.Action = "DuyetCanDoiNPL";
                                        title = "Phiếu cân đối đã duyệt";
                                        detail = `Phiếu CD : ${focusedRowData.TenPhieu}\nMã Hàng: ${focusedRowData.TenHang || ''} - ${focusedRowData.MaDH || ''}`;
                                        break;
                                    case "M.12.02.00":
                                        obj.Action = "DuyetBaoGia";
                                        title = "Phiếu báo giá đã duyệt";
                                        detail = `Phiếu BG: ${focusedRowData.TenPhieu}\nNhà cung cấp: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                        break;
                                    case "M.04.02.00":
                                        obj.Action = "DuyetBOM";
                                        obj.MaPhieu = focusedRowData.MaPhieu || focusedRowData.MaDot;
                                        title = "BOM đã được ban hành";
                                        detail = `Đợt: ${MaPhieu || focusedRowData.MaDot}\nMã Hàng: ${focusedRowData.TenHang || ''}\nKhách hàng: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                        break;
                                    default:
                                        break;
                                }

                                arrDuyetPhieu.push(obj);
                             
                                DuyetPhieu(arrDuyetPhieu, true).then(result => {
                                    if (result) {
                                        
                                        var sendto = "PKH";
                                        var MaKH = "";
                                        var MaHang = "";
                                        var MaDot = "";
                                        if (ModuleID == "M.04.02.00") {
                                            if (currentModuleId == "overview") {
                                                MaKH = focusedRowData.MaNCC
                                                MaHang = focusedRowData.MaDH
                                                MaDot = focusedRowData.MaPhieu
                                            }
                                            else {
                                                MaKH = focusedRowData.MaKH
                                                MaHang = focusedRowData.MaHang
                                               MaDot = focusedRowData.MaDot


                                            }
                                            getTSAsync(MaKH, MaHang, UserID, MaDot)
                                               
                                        }
                                        sendNotify(ModuleID, title, detail, sendto)
                                    }

                                });
                            }

                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Hủy",
                        icon: "close",
                        elementAttr: { class: "btn-huy-duyet" },
                        onClick: function (e) {
                            var focusedRowData = getFocusedRowData();
                            var ModuleID = currentModuleId == "overview" ? focusedRowData.ModuleID : currentModuleId;
                            if (ModuleID == "M.12.03.00") {
                                DuyetMuaHang(false)
                            }
                            else {
                                var arrDuyetPhieu = [];
                            
                                let MaPhieu = focusedRowData ? focusedRowData.MaPhieu : "NONE";

                                if (MaPhieu === "NONE") {
                                    DevExpress.ui.notify("Vui lòng chọn phiếu để hủy duyệt", "warning", 2000);
                                    return;
                                }

                                
                                let obj = {
                                    Action: "",
                                    MaPhieu: MaPhieu,
                                    IsDuyet: false, // Hủy duyệt = false
                                    NguoiPheDuyet: UserID,
                                    GhiChu: ""
                                };

                                switch (ModuleID) {
                                    case "M.12.01.00":
                                        obj.Action = "DuyetCanDoiNPL";
                                        title = "Phiếu cân đối đã hủy duyệt";
                                        detail = `Phiếu CD : ${focusedRowData.TenPhieu}\nMã Hàng: ${focusedRowData.TenHang || ''} -${focusedRowData.MaDH || ''}`;
                                        break;
                                    case "M.12.02.00":
                                        obj.Action = "DuyetBaoGia";
                                        title = "Phiếu báo giá đã hủy duyệt";
                                        detail = `Phiếu BG: ${focusedRowData.TenPhieu}\nNhà cung cấp: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                        break;
                                    case "M.04.02.00":
                                        obj.Action = "DuyetBOM";
                                        obj.MaPhieu = focusedRowData.MaPhieu || focusedRowData.MaDot;
                                        title = "BOM đã được hủy ban hành";
                                        detail = `Đợt: ${MaPhieu || focusedRowData.MaDot}\nMã Hàng: ${focusedRowData.TenHang || ''}\nKhách hàng: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                        break;
                                    default:
                                        break;
                                }

                                arrDuyetPhieu.push(obj);
                                DuyetPhieu(arrDuyetPhieu, false).then(result => {
                                    if (result) {
                                      
                                        var sendto = "PKH";
                                        sendNotify(ModuleID, title, detail, sendto);
                                    }
                                });
                                //reloadPhieuCount();
                            }

                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Refresh",
                        icon: "refresh",
                        elementAttr: { class: "btn-refresh" },
                        onClick: function (e) {
                            if (currentModuleId === 'overview') {
                                SetupTabOverview("overview");
                            } else {
                                loadModuleData(ModuleID, IsDuyetSelected);
                            }
                        }
                    }
                }
            );
            break;
        case ModuleID == "M.12.03.00":
            toolbarItems.push(
                {
                    location: "after",
                    widget: "dxCheckBox",
                    options: {
                        text: !isSmallScreen ? "Duyệt" : "Duyệt",
                        value: IsDuyetSelected || false,
                        onValueChanged: function (e) {
                            IsDuyetSelected = e.value;
                            if (currentModuleId === 'overview') {
                                loadOverviewData();
                            } else {
                                loadModuleData(ModuleID, e.value);
                            }
                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Duyệt",
                        icon: "check",
                        elementAttr: { class: "btn-duyet" },
                        onClick: function (e) {
                            var focusedRowData = getFocusedRowData();
                            let MaPhieu = focusedRowData ? focusedRowData.MaPhieu : "NONE";

                            if (MaPhieu === "NONE") {
                                DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                                return;
                            }
                            DuyetMuaHang(true);
                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Hủy",
                        icon: "close",
                        elementAttr: { class: "btn-huy-duyet" },
                        onClick: function (e) {

                            var focusedRowData = getFocusedRowData();
                            let MaPhieu = focusedRowData ? focusedRowData.MaPhieu : "NONE";

                            if (MaPhieu === "NONE") {
                                DevExpress.ui.notify("Vui lòng chọn phiếu để hủy duyệt", "warning", 2000);
                                return;
                            }

                            DuyetMuaHang(false);
                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Refresh",
                        icon: "refresh",
                        elementAttr: { class: "btn-refresh" },
                        onClick: function (e) {
                            if (currentModuleId === 'overview') {
                                SetupTabOverview("overview");
                            } else {
                                loadModuleData(ModuleID, IsDuyetSelected);
                            }
                        }
                    }
                }
            );
            break;
        case ModuleID == "M.21.00.00":

            toolbarItems.push(
                {
                    location: "after",
                    widget: "dxCheckBox",
                    options: {
                        text: isSmallScreen ? "Duyệt" : "Đã duyệt",
                        value: IsDuyetSelected || false,
                        onValueChanged: function (e) {
                            IsDuyetSelected = e.value;
                            if (currentModuleId === 'overview') {
                                SetupTabOverview("overview");
                            } else {
                                loadModuleData(ModuleID, e.value);
                            }
                        }
                    }
                },
                //{
                //    location: "before",
                //    widget: "dxButton",
                //    options: {
                //        text: "Duyệt",
                //        icon: "check",
                //        elementAttr: { class: "btn-duyet" },
                //        onClick: function (e) {

                //        }
                //    }
                //},
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Refresh",
                        icon: "refresh",
                        elementAttr: { class: "btn-refresh" },
                        onClick: function (e) {
                            if (currentModuleId === 'overview') {
                                SetupTabOverview("overview");
                            } else {
                                loadModuleData(ModuleID, IsDuyetSelected);
                            }
                        }
                    }
                },

            );
            break;
        case ModuleID == "M.04.01.00": 
        case ModuleID == "overview" && selectedTab == 1:

            toolbarItems.push(
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Duyệt",
                        icon: "check",
                        elementAttr: { class: "btn-duyet" },
                        onClick: function (e) {
                            var focusedRowKey = getFocusedRowData();
                            if (!focusedRowKey) {
                                DevExpress.ui.notify("Vui lòng chọn lệnh SX trước khi duyệt", "warning", 2000);
                                return;
                            }
                            var MaLenh = focusedRowKey.MaLenh;
                            var maDH = focusedRowKey.MaDH;
                            DongBoLenhSX(maDH, MaLenh).then(result => {
                                if (!result) return;

                                var detail = `Đơn Hàng: ${focusedRowKey.MaDH || ''}\nMã Hàng: ${focusedRowKey.TenHang || ''}\nLệnh: ${focusedRowKey.MaLenh || ''}\nSố lượng: ${focusedRowKey.SoLuong || ''}`;
                                var title = focusedRowKey.Status_LSX == "Duyệt lại" ? "Lệnh sản xuất được ban hành lại": "Lệnh sản xuất đã được ban hành";
                                var sendto = "ALL";
                                sendNotify("M.04.01.00", title, detail, sendto);

                            });
                          

                        }
                    }
                },
                {
                    location: "before",
                    widget: "dxButton",
                    options: {
                        text: "Refresh",
                        icon: "refresh",
                        elementAttr: { class: "btn-refresh" },
                        onClick: function (e) {
                            if (currentModuleId === 'overview') {
                                SetupTabOverview("overview");
                            } else {
                                loadModuleData(ModuleID, IsDuyetSelected);
                            }
                        }
                    }
                }

            );
            break;
    }
    if (!isSmallScreen) {
        toolbarItems.push(
            {
                location: "after",
                name: "searchPanel"
            }
        );
    }

    return toolbarItems;
}

function showDetailPopup(rowData, ModuleID) {

    if (ModuleID == "overview") {
        ModuleID = rowData.ModuleID
    }

    const actionConfig = arrActionDetail.find(x => x.ModuleID === ModuleID);

    if (!actionConfig) {
        console.warn("Không tìm thấy cấu hình action cho module: " + ModuleID);
        return;
    }
   
    let maPhieu = rowData.MaPhieu;

    if (!maPhieu) {
        console.warn("Không tìm thấy mã phiếu trong dữ liệu");
        return;
    }
    let popup = $("#detailPopup").dxPopup({
        visible: false,
        title: `${rowData.TenPhieu}`,
        width: "95vw",
        height: "100vh",
        showCloseButton: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        shading: true,
        shadingColor: "rgba(0,0,0,0.5)",
        wrapperAttr: {
            class: "dx-popup"
        },
        toolbarItems: [
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Duyệt",
                    type: "success",
                    icon: "check",
                    onClick: function () {
                        var focusedRowData = getFocusedRowData();
                        var ModuleID = currentModuleId == "overview" ? focusedRowData.ModuleID : currentModuleId
                        if (ModuleID == "M.12.03.00") {
                            DuyetMuaHang(true);
                        } else {
                            var arrDuyetPhieu = [];
                          
                            let MaPhieu = focusedRowData ? focusedRowData.MaPhieu : "NONE";

                            if (MaPhieu === "NONE") {
                                DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                                return;
                            }
                           
                            let obj = {
                                Action: "",
                                MaPhieu: MaPhieu,
                                IsDuyet: true,
                                NguoiPheDuyet: UserID,
                                GhiChu: ""
                            };

                            switch (ModuleID) {
                                case "M.12.01.00":
                                    obj.Action = "DuyetCanDoiNPL";
                                    title = "Phiếu cân đối đã duyệt";
                                     detail = `Phiếu CD : ${focusedRowData.TenPhieu}\nMã Hàng: ${focusedRowData.TenHang || ''} -${focusedRowData.MaDH || ''}`;
                                    break;
                                case "M.12.02.00":
                                    obj.Action = "DuyetBaoGia";
                                    title = "Phiếu báo giá đã duyệt";
                                    detail = `Phiếu BG: ${focusedRowData.TenPhieu}\nNhà cung cấp: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                    break;
                                case "M.04.02.00":
                                    obj.Action = "DuyetBOM";
                                    obj.MaPhieu = focusedRowData.MaPhieu || focusedRowData.MaDot;
                                    title = "BOM đã được ban hành";
                                    detail = `Đợt: ${MaPhieu || focusedRowData.MaDot}\nMã Hàng: ${focusedRowData.TenHang || ''}\nKhách hàng: ${ffocusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                    break;
                                default:
                                    break;
                            }

                            arrDuyetPhieu.push(obj);
                            DuyetPhieu(arrDuyetPhieu, true).then(result => {
                                if (result) {
                                 
                                    var sendto = "PKH";
                                    sendNotify(ModuleID, title, detail, sendto);
                                }
                            });
                        }
                       
                        popup.hide();

                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Hủy duyệt",
                    type: "danger",          
                    icon: "close",           
                    onClick: function () {
                        var focusedRowData = getFocusedRowData();
                        var ModuleID = currentModuleId == "overview" ? focusedRowData.ModuleID : currentModuleId
                        if (ModuleID == "M.12.03.00") {
                            DuyetMuaHang(false);
                        } else {
                            var arrDuyetPhieu = [];
                           
                            let MaPhieu = focusedRowData ? focusedRowData.MaPhieu : "NONE";

                            if (MaPhieu === "NONE") {
                                DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                                return;
                            }
                           
                            let obj = {
                                Action: "",
                                MaPhieu: MaPhieu,
                                IsDuyet: false,
                                NguoiPheDuyet: UserID,
                                GhiChu: ""
                            };

                            switch (ModuleID) {
                                case "M.12.01.00":
                                    obj.Action = "DuyetCanDoiNPL";
                                    title = "Phiếu cân đối đã hủy duyệt";
                                    detail = `Phiếu CD : ${focusedRowData.TenPhieu}\nMã Hàng: ${focusedRowData.TenHang || ''} -${focusedRowData.MaDH || ''}`;
                                    break;
                                case "M.12.02.00":
                                    obj.Action = "DuyetBaoGia";
                                    title = "Phiếu báo giá đã hủy duyệt";
                                    detail = `Phiếu BG: ${focusedRowData.TenPhieu}\nNhà cung cấp: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                    break;
                                case "M.04.02.00":
                                    obj.Action = "DuyetBOM";
                                    obj.MaPhieu = focusedRowData.MaPhieu || focusedRowData.MaDot;
                                    title = "BOM đã được hủy ban hành";
                                    detail = `Đợt: ${MaPhieu || focusedRowData.MaDot}\nMã Hàng: ${focusedRowData.TenHang || ''}\nKhách hàng: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                    break;
                                default:
                                    break;
                            }

                            arrDuyetPhieu.push(obj);
                            DuyetPhieu(arrDuyetPhieu, false).then(result => {
                                if (result) {                                 
                                    var sendto = "PKH";
                                    sendNotify(ModuleID, title, detail, sendto);
                                }
                            });
                        }
                       
                        popup.hide();
                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Đóng",
                    type: "normal",
                    onClick: function () {
                        popup.hide();
                    }
                }
            }
        ],
       
       
    }).dxPopup("instance");

    popup.show();

    loadDetailData(maPhieu, actionConfig, ModuleID);
}

function loadDetailData(maPhieu, actionConfig, ModuleID) {
    const detailGrid = $("#detailGridContainer").dxDataGrid("instance");

    if (!detailGrid) {
        console.error("Detail grid chưa được khởi tạo");
        return;
    }
    const action = actionConfig.Action; 
    $.ajax({
        url: `/api/PheDuyet/Get?action=${action}&para1=${maPhieu}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            detailGrid.endCustomLoading();
            console.log('Detail data loaded:', data);

            if (data && data.length > 0) {
                const columns = generateDetailColumns(data[0], actionConfig, ModuleID);
                detailGrid.option("columns", columns);
                detailGrid.option("dataSource", data);
                const toolbarConfig = generateDetailToolbar(ModuleID, maPhieu);
                detailGrid.option("toolbar", {
                    items: toolbarConfig
                });         
            } else {
                detailGrid.option("dataSource", []);
                //DevExpress.ui.notify("Không có dữ liệu chi tiết", "warning", 2000);
            }
        },
        error: function (err) {
            detailGrid.endCustomLoading();       
            detailGrid.option("dataSource", []);
           
        }
    });
}

function generateDetailColumns(sampleRow, actionConfig, ModuleID) {
    if (!sampleRow) return [];

    let columns = [];

    switch (ModuleID) {
        case 'M.12.01.00':
            columns.push(
                {
                    dataField: "LoaiNPL", caption: "LoaiNPL",
                    groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {

                        cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.key[0]}</span>`);
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },          
                { dataField: "MaDH", caption: "Đơn hàng" },
                { dataField: "TenHang", caption: "Mã hàng" },
                {
                    dataField: "ChungLoaiVatTu", caption: "Chủng loại vật tư", groupIndex: 1,
                    groupCellTemplate: function (cellElement, cellInfo) {
                        const items = cellInfo.data.items;
                        cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.key[1]}</span>`);
                    }, },
                { dataField: "MaVT", caption: "Item Code" },
                { dataField: "ChiTiet", caption: "Mô tả" },
                { dataField: "CodeMau", caption: "Code màu" },
                { dataField: "MauVT", caption: "Màu vật tư" },
                { dataField: "KhoVai", caption: "Khổ/Size" },
                { dataField: "TenDVVT", caption: "Đơn vị" },
                {
                    dataField: "SLNhuCau", caption: "Nhu cầu"
                    , minWidth: 80,
                    width: 80,
                    allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }    },
                {

                    dataField: "SLCanDoiKho", caption: "Cân đối",
                    minWidth: 80,
                    width: 80,
                    allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                {
                    dataField: "SLMuaThem", caption: "Cần mua"
                    , minWidth: 80,
                    width: 80,
                    allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }                },
                {
                    dataField: "TyLeMuaThem", caption: "Tỷ lệ mua thêm (%)", minWidth: 80,
                    width: 80, allowFiltering: false,},
                {
                    caption: "Tổng nhu cầu cần mua (sau mua thêm)"
                    , minWidth: 80,
                    width: 80,
                    allowFiltering: false,
                    calculateCellValue: function (rowData) {
                        const tongNhuCau = rowData.SLMuaThem || 0;
                        const tyLeMuaThem = rowData.TyLeMuaThem || 0;       

                        const tongSauMuaThem = tongNhuCau * (1 + tyLeMuaThem / 100);
                        return tongSauMuaThem;
                    },
                    customizeText: function (cellInfo) {
                     
                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }
                }

            );
            break;
        case 'M.12.02.00':
            columns.push(
                {
                    dataField: "rowIndex",
                    caption: "STT",
                    width: 50,
                    alignment: "center",
                    allowFiltering: false,
                    cellTemplate: function (container, options) {
                        if (options.rowType == "data")
                        {
                            $("<div>").text(options.rowIndex + 1).appendTo(container);
                        }
                    }

                },
                {
                    dataField: "LoaiNPL", caption: "LoaiNPL",
                    groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {

                        cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.key[0]}</span>`);
                    },
                },
                {
                    dataField: "TenCL", caption: "TenCL", groupIndex: 1,
                    groupCellTemplate: function (cellElement, cellInfo) {
                        const items = cellInfo.data.items;
                        cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.key[1]}</span>`);
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },
                { dataField: "ItemCode", caption: "Item Code (1)" },
                { dataField: "MoTa", caption: "Mô tả (2)" },
                { dataField: "MaMauVT", caption: "Code màu (3)" },
                { dataField: "MauVT", caption: "Màu vật tư (4)" },
                { dataField: "KhoVai", caption: "Khổ/size (5)" },
                { dataField: "TenDVVT", caption: "Đơn vị (6)" },
                {
                    dataField: "DonGia", caption: "Đơn giá cơ bản (7)", minWidth: 80,
                    width: 80,
                    allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                {
                    dataField: "ChietKhau", caption: "% Chiết khấu (8)"
                    , minWidth: 80,
                    width: 80,
                    allowFiltering: false,                },
                {
                    caption: "Đơn giá sau chiết khấu (9)",
                    calculateCellValue: function (rowData) {
                        const donGia = rowData.DonGia || 0;
                        const chietKhau = rowData.ChietKhau || 0;
                        const giaSauCK = donGia - (donGia * chietKhau / 100);
                        return giaSauCK
                    },
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }
                },

                {
                    dataField: "Thue", caption: "% Thuế (10)", minWidth: 80,
                    width: 80,
                    allowFiltering: false,},
                {
                    caption: "Đơn giá sau thuế (11)",
                    calculateCellValue: function (rowData) {
                        const donGia = rowData.DonGia || 0;                
                        const ck = rowData.ChietKhau || 0;
                        const thue = rowData.Thue || 0;
                        const giaSauCK = donGia - (donGia * ck / 100);
                        const giaSauThue = giaSauCK + (giaSauCK * thue / 100);
                        return giaSauThue;
                    },
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }
                },

                { dataField: "DVTienTeVT", caption: "Tiền tệ (12)" },
                {
                    caption: "Đơn giá sau thuế quy đổi (13)",
                    calculateCellValue: function (rowData) {
                        const donGia = rowData.DonGia || 0;                 
                        const ck = rowData.ChietKhau || 0;
                        const thue = rowData.Thue || 0;
                        const giaQDVT = rowData.GiaQDVT || 1;               
                        const giaQDPhieuBG = rowData.GiaQDPhieuBG || 1;     

                        var maTienTe = rowData.TienTeID || "";
                    
                        const giaSauCK = donGia - (donGia * ck / 100);

                     
                        const giaSauThue = giaSauCK + (giaSauCK * thue / 100);

                    
                        const giaSauThueQD = giaSauThue * (giaQDVT / giaQDPhieuBG);

                        var valueQD = Intl.NumberFormat('vi-VN').format(giaSauThueQD);

                        return valueQD + " " + maTienTe;
                    },
                    
                },

                { dataField: "GhiChu", caption: "Ghi chú (14)" },
            

            );
            break;
        case 'M.12.03.00':
            columns.push(
                {
                    dataField: "rowIndex",
                    caption: "STT (1)",
                    width: 50,
                    alignment: "center",
                    allowFiltering: false,
                    cellTemplate: function (container, options) {
                        if (options.rowType == "data") {
                            $("<div>").text(options.rowIndex + 1).appendTo(container);
                        }
                    }

                },
                {
                    dataField: "LoaiNPL", caption: "LoaiNPL",
                    groupIndex: 0,
                    groupCellTemplate: function (cellElement, cellInfo) {

                        cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.key[0]}</span>`);
                    },
                },
                { dataField: "MaPhieu", caption: "MaPhieu", visible: false },
                { dataField: "TenCL", caption: "Chủng loại VT (2)" },
                { dataField: "ItemCode", caption: "Item code (3)" },
                { dataField: "MoTa", caption: "Mô tả (4)" },
                { dataField: "ColorCode", caption: "Code màu (5)", },
                { dataField: "MauVT", caption: "Màu VT (6)", },
                { dataField: "KhoVai", caption: "Khổ/Size (7)" },
                { dataField: "TenDVVT", caption: "Đơn vị (8)" },
                {
                    dataField: "TongSLMuaThem", caption: "Nhu cầu cần mua (9)", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    } },
                {
                    dataField: "DonGia", caption: "Đơn giá (10)", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                {
                    dataField: "ChiPhiVT", caption: "Chi phí vật tư (11) = (9)*(10)", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                { dataField: "ChietKhau", caption: "% Chiết khấu (12)", allowFiltering: false },
                {
                    dataField: "ChiPhiSauCK", caption: "Chi phí sau chiết khấu (13) = (11) - (12)", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    }},
                { dataField: "Thue", caption: "% Thuế (14)", allowFiltering: false },
                {
                    dataField: "ThanhTien", caption: "Thành tiền (14) = (11) + (11*13) ", allowFiltering: false,
                    customizeText: function (cellInfo) {

                        return new Intl.NumberFormat('en-US', {
                            minimumFractionDigits: 0,
                            maximumFractionDigits: 4
                        }).format(cellInfo.value);

                    } },
                { dataField: "TienTeID", caption: "Đơn vị tiền tệ (15)" },
                {
                    dataField: "MaTienTePhieuMH",
                    caption: "MaTienTePhieuMH",
                    visible: false
                },
                {
                    dataField: "ThanhTienQD",
                    caption: "Thành tiền quy đổi (16)",
                    allowFiltering: false,
                    calculateCellValue: function (rowData) {
                        // rowData là toàn bộ dữ liệu của dòng
                        var value = rowData.ThanhTienQD || 0;
                        var maTienTe = rowData.MaTienTePhieuMH || "";
                        return new Intl.NumberFormat('vi-VN').format(value) + " " + maTienTe;
                    }
                    
                },

                { dataField: "GhiChu", caption: "Ghi chú (17)" }
              
            );
            break;

        default:
            Object.keys(sampleRow).forEach(key => {
                if (key !== "ChildData") {
                    columns.push({
                        dataField: key,
                        caption: key,
                        minWidth: 100
                    });
                }
            });
            break;
    }

    return columns;
}

function generateDetailToolbar(ModuleID, maPhieu) {
    let toolbarItems = [];

    switch (ModuleID) {      
        case 'M.12.02.00':
            toolbarItems.push(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Xem danh mục khoản phí",
                        icon: "money",
                        onClick: function (e) {
                            initializeSubDetailPopup();
                            showSubDetailPopup(maPhieu, ModuleID);
                        }
                    }
                },
              
            );
            break;
        case 'M.12.03.00':
            toolbarItems.push(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Xem chi tiết chi phí",
                        icon: "money",
                        onClick: function (e) {
                            initializeSubDetailPopup();
                            showSubDetailPopup(maPhieu, ModuleID)
                        }
                    }
                }
            );
            break;
    
    }


    toolbarItems.push({
        location: "after",
        name: "searchPanel"
    });

    return toolbarItems;
}

var subDetailPopupInstance = null;

function initializeSubDetailPopup() {
    if (!subDetailPopupInstance) {
        subDetailPopupInstance = $("#Popup-SubDetail").dxPopup({
            visible: false,
            title: "Chi tiết",
            width: "95vw",
            height: "100vh",
            showCloseButton: true,
            dragEnabled: false,
            hideOnOutsideClick: true,
            shading: true,
            shadingColor: "rgba(0,0,0,0.5)",
            wrapperAttr: {
                class: "dx-popup"
            },


        }).dxPopup("instance");
    }
    return subDetailPopupInstance;
}

let arrActionSubDetail = [
    { ModuleID: "M.12.02.00", Action: "GetSubDetailBaoGia" },
    { ModuleID: "M.12.03.00", Action: "GetSubDetailMuaHang" },

];

function generateChildColumns(sampleRow) {
    if (!sampleRow) return [];

    let columns = [];

    Object.keys(sampleRow).forEach(key => {
        if (key !== "TableName") {
            const value = sampleRow[key];

            // Kiểm tra xem giá trị có phải là số không
            const isNumber = typeof value === 'number' ||
                (!isNaN(parseFloat(value)) && isFinite(value));

            columns.push({
                dataField: key,
                caption: key,
                minWidth: 100,
                allowFiltering: !isNumber,
                allowSorting: true,
                dataType: isNumber ? 'number' : undefined,
                customizeText: isNumber ? function (cellInfo) {
                    return cellInfo.value != null
                        ? new Intl.NumberFormat('vi-VN').format(cellInfo.value)
                        : '';
                } : undefined
            });
        }
    });

    return columns;
}

function generateSummaryItems(sampleRow) {
    if (!sampleRow) return [];

    let summaryItems = [];

    Object.keys(sampleRow).forEach(key => {
        if (key !== "TableName") {
            const value = sampleRow[key];
            const isNumber = typeof value === 'number' ||
                (!isNaN(parseFloat(value)) && isFinite(value));

            if (isNumber) {
                summaryItems.push({
                    column: key,
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 0
                    },
                    customizeText: function (data) {
                        return new Intl.NumberFormat('vi-VN').format(data.value);
                    }
                });
            }
        }
    });

    return summaryItems;
}

function renderMultipleGrids(container, dataArrays, titles) {

    container.empty();

    if (!dataArrays || dataArrays.length === 0) {
        container.append('<div class="no-data-message">Không có dữ liệu</div>');
        return;
    }

    dataArrays.forEach((dataArray, index) => {
        if (!dataArray || dataArray.length === 0) return;

        const gridWrapper = $('<div class="sub-grid-wrapper"></div>').css({
            'margin-bottom': '25px',
            'background': '#fff',
            'padding': '15px',
            'border-radius': '6px',
            'box-shadow': '0 2px 4px rgba(0,0,0,0.1)'
        });

        // Tiêu đề
        if (titles && titles[index]) {
            const titleElement = $('<div class="sub-grid-title"></div>')
                .html(`
                    <h3 style="margin: 0 0 15px 0; color: #333; font-size: 16px; font-weight: 600; border-bottom: 2px solid #1890ff; padding-bottom: 8px;">
                        ${titles[index]}
                        
                    </h3>
                `);
            gridWrapper.append(titleElement);
        }

        const gridDiv = $('<div class="sub-grid"></div>');
        gridWrapper.append(gridDiv);

        gridDiv.dxDataGrid({
            dataSource: dataArray,
            showBorders: true,
            columnAutoWidth: true,
            rowAlternationEnabled: true,
            showRowLines: true,
            showColumnLines: true,
            hoverStateEnabled: true,
            summary: {
                totalItems: generateSummaryItems(dataArray[0])
            },
            columns: generateChildColumns(dataArray[0]),
            paging: {
                enabled: false,

            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [5, 10, 20, 50],
                showInfo: true,
                showNavigationButtons: true
            },
            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            headerFilter: {
                visible: true
            },
            searchPanel: {
                visible: true,
                width: 240,
                placeholder: "Tìm kiếm..."
            },

            scrolling: {
                mode: "standard"
            }
        });

        container.append(gridWrapper);
    });
}

function loadSubDetailData(maPhieu, actionConfig, ModuleID) {
    const container = $("#subDetailContentContainer");
    container.html(`
        <div class="loading-container" style="display: flex; justify-content: center; align-items: center; min-height: 300px;">
            <div class="dx-loadpanel-content">
                <div class="dx-loadpanel-message">Đang tải dữ liệu...</div>
            </div>
        </div>
    `);

    const action = actionConfig.Action;

    $.ajax({
        url: `/api/PheDuyet/GETDS?action=${action}&para1=${maPhieu}`,
        method: 'GET',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            let dataArrays = [];
            let titles = [];

            if (response) {
                Object.keys(response).forEach((key, index) => {
                    const value = response[key];

                    if (Array.isArray(value) && value.length > 0) {
                        dataArrays.push(value);
                        titles.push(value[0].TableName || "Thông tin chi tiết");
                    }
                });


                if (dataArrays.length === 0) {
                    dataArrays = [response];
                    titles = ['Thông tin chi tiết'];
                }
            }
            if (dataArrays.length > 0) {
                renderMultipleGrids(container, dataArrays, titles);
            }
            else {
                container.html(`
                    <div class="no-data-message" style="text-align: center; padding: 50px; color: #999;">
                        <i class="fa fa-inbox" style="font-size: 48px; margin-bottom: 15px; display: block;"></i>
                        <p style="font-size: 16px;">Chưa có dữ liệu</p>
                    </div>
                `);
            }
        },
        error: function (err) {

        }
    });
}

function showSubDetailPopup(maPhieu, ModuleID) {

    const actionConfig = arrActionSubDetail.find(x => x.ModuleID === ModuleID);
    if (!actionConfig) {
        console.warn("Không tìm thấy cấu hình action cho module: " + ModuleID);
        return;
    }
    if (!maPhieu) {
        console.warn("Không tìm thấy mã phiếu trong dữ liệu");

        return;
    }
    const popup = initializeSubDetailPopup();
    popup.show();
    loadSubDetailData(maPhieu, actionConfig, ModuleID);
}

function getFocusedRowData() {
    let grid = $("#gridContainer").dxDataGrid("instance");
    let focusedRowIndex = grid.option("focusedRowIndex");

    if (focusedRowIndex >= 0) {
        return grid.getVisibleRows()[focusedRowIndex].data;
    }
    return null;
}
/*chi tiết đánh giá*/

/*BEGIN*/
function ShowDanhGiaPopup(rowData) {
    let maPhieu = rowData.MaPhieu;

    if (!maPhieu) {
        console.warn("Không tìm thấy mã phiếu trong dữ liệu");
        return;
    }
     
    let popup = $("#PopupDanhGia").dxPopup({
        visible: false,
        title: `Phiếu đánh giá : ${rowData.TenPhieu}`,
        width: "95vw",
        height: "100vh",
        showCloseButton: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        shading: true,
        shadingColor: "rgba(0,0,0,0.5)",
        wrapperAttr: {
            class: "dx-popup"
        },
        toolbarItems: [
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Xác nhận",
                    type: "success",
                    icon: "check",
                    onClick: function () {
                      
                        XacNhanPhieuDGNhaCC(rowData, "DanhGia")
                        popup.hide();
                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Đóng",
                    type: "normal",
                    onClick: function () {
                        popup.hide();
                    }
                }
            }
        ]
        , onShown: function (e) {
            InitSignalture();      
            GetPhieuDanhGia(maPhieu)
            GetKetQuaDanhGia(maPhieu);
            CheckPerminsion();
            
            setPermission('xac-nhan-de-xuat', rowData.NguoiDanhGiaPhieu == UserID);
        }


    }).dxPopup("instance");

    popup.show();
}

function GetPhieuDanhGia(maPhieu) {
    $.ajax({
    
        url: `/api/PheDuyet/Get?action=GetPhieuDanhGiaDetail&para1=${maPhieu}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {          
            if (data && data.length > 0) {
                fetchPhieuDanhGia(data);
               
            }
            
        },
        error: function (err) {
            
        }
    });
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
                minWidth: 120,
                width: 150,
            });

            return acc;
        }, []);

    $("#gridPhieuDanhGia").dxDataGrid({
        dataSource: data,
      
        columns: [
           
            {
                dataField: "TenNCC",
                caption: "Nhà cung cấp",
           
              
            },
            {
                caption: "Thông tin",
                columns: dynamicColumns
            },
            {
                dataField: "TongDiemUuTien",
                caption: "Tổng điểm",
                minWidth: 80,
                width: 80,
                allowEditing: false
            },
            {
                dataField: "ThuTuUuTien",
                caption: "TT ưu tiên",
                minWidth: 80,
                width: 80,
                allowEditing: false
            },
            {
                dataField: "GhiChu",
                caption: "Kết luận",
              
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

      
    });
}

function GetKetQuaDanhGia(MaPhieu) {
    $.ajax({
      
        url: `/api/PhieuDanhGiaNhaCC/Get?action=GetKetQuaDG&para1=${MaPhieu}&para2=DanhGia`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            clearKetQuaDanhGia();
            if (data.length > 0)
            {
                fetchKetQuaDanhGia(data[0]);
            }
        }
    });
}

function clearKetQuaDanhGia() {
    var canvasIds = ['signQC', 'signMer', 'signXN_KQ'];
    // Clear radio buttons
    $("#KQPass_NgDeXuat, #KQFail_NgDeXuat").prop("checked", false);
    $("#KQPass_SoatXet, #KQFail_SoatXet").prop("checked", false);
    $("#KQPass_TongGiamDoc, #KQFail_TongGiamDoc").prop("checked", false);

    // Clear textareas
    $("#txtDG_QC").val("");
    $("#txtDG_Mer").val("");
    $("#txtDG_KQ").val("");

    canvasIds.forEach(id => {
        clearSignature(id);
    })

}

function clearSignature(id) {
    if (signaturePads[id]) {
        signaturePads[id].clear();
        delete signatureData[id]; // Xóa dữ liệu đã lưu
    }
}

function fetchKetQuaDanhGia(item) {
  
    if (item.KQ_DG_NgDeXuat === "true") {
        $("#KQPass_NgDeXuat").prop("checked", true);
    } else if (item.KQ_DG_NgDeXuat === "false") {
        $("#KQFail_NgDeXuat").prop("checked", true);
    }
    $("#txtDG_QC").val(item.YKien_NgDeXuat || "");


    if (item.KQ_DG_SoatXet === "true") {
        $("#KQPass_SoatXet").prop("checked", true);
    } else if (item.KQ_DG_SoatXet === "false") {
        $("#KQFail_SoatXet").prop("checked", true);
    }
    $("#txtDG_Mer").val(item.YKien_SoatXet || "");


    if (item.KQ_DG_TongGiamDoc === "true") {
        $("#KQPass_TongGiamDoc").prop("checked", true);
    } else if (item.KQ_DG_TongGiamDoc === "false") {
        $("#KQFail_TongGiamDoc").prop("checked", true);
    }
    $("#txtDG_KQ").val(item.YKien_TongGiamDoc || "");
    loadSignatures(item);
 
    
}

function loadSignatures(item) {
    // Chữ ký người đề xuất
    if (item.SignDG_NgDeXuat) {
        const canvasQC = document.getElementById('signQC');
        if (canvasQC) {
            
            if (!signaturePads["signQC"]) {
                signaturePads["signQC"] = new SignaturePad(canvasQC);
            }

            try {
                signaturePads["signQC"].fromDataURL(item.SignDG_NgDeXuat);
               
            } catch (e) {
                console.error('Error loading signature QC:', e);
            }
        } else {
            console.warn('Canvas signQC not found');
        }
    }

    // Chữ ký soát xét
    if (item.SignDG_SoatXet) {
        const canvasMer = document.getElementById('signMer');
        if (canvasMer) {
            if (!signaturePads["signMer"]) {
                signaturePads["signMer"] = new SignaturePad(canvasMer);
            }

            try {
                signaturePads["signMer"].fromDataURL(item.SignDG_SoatXet);
             
            } catch (e) {
                console.error('Error loading signature Mer:', e);
            }
        } else {
            console.warn('Canvas signMer not found');
        }
    }

    // Chữ ký tổng giám đốc
    if (item.SignDG_TongGiamDoc) {
        const canvasKQ = document.getElementById('signXN_KQ');
        if (canvasKQ) {
            if (!signaturePads["signXN_KQ"]) {
                signaturePads["signXN_KQ"] = new SignaturePad(canvasKQ);
            }

            try {
                signaturePads["signXN_KQ"].fromDataURL(item.SignDG_TongGiamDoc);
             
            } catch (e) {
                console.error('Error loading signature KQ:', e);
            }
        } else {
            console.warn('Canvas signXN_KQ not found');
        }
    }
}

let IsSoatXet = false;

let allowXacNhan = false;

function CheckPerminsion() {

    var userName = localStorage.getItem("username1");
    var ModuleID = 'M.21.00.00'
    $.ajax({
        async: false,
        url: `/api/PhieuDanhGiaNhaCC/Get?action=CheckPerminsion&para1=${userName}&para2=${ModuleID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            setPermission('xac-nhan-de-xuat', false);
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

    // Radio (khóa theo name + label)
    $(selector).find('input[type="radio"]').prop('disabled', !hasPermission);
    $(selector).find('label').css('pointer-events', hasPermission ? 'auto' : 'none');

    // Textarea
    $(selector).find('textarea').prop('readonly', !hasPermission);

    // Button
    $(selector).find('.btn-clear')
        .prop('disabled', !hasPermission)
        .css('pointer-events', hasPermission ? 'auto' : 'none');

    // Canvas ký tên
    $(selector).find('canvas')
        .css('pointer-events', hasPermission ? 'auto' : 'none');

    // Hiệu ứng UI
    $(selector).css({
        opacity: hasPermission ? '1' : '0.6',
        cursor: hasPermission ? 'default' : 'not-allowed'
    });
}

function XacNhanPhieuDGNhaCC(rowData, LoaiDanhGia) {
    DevExpress.ui.dialog.confirm("Bạn có chắc muốn xác nhận phiếu đánh giá này?", "Xác nhận").done(function (dialogResult) {
        if (!dialogResult) {
            return;
        }

    var dataUpload = [];
    var arrSaveData = [];
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

    const getFileName = `DG-${rowData.MaPhieu}`;

    if (signQC) dataUpload.push({ img: signQC, name: 'NgDeXuat' });
    if (signMer) dataUpload.push({ img: signMer, name: 'SoatXet' });
    if (signKQ) dataUpload.push({ img: signKQ, name: 'TongGiamDoc' });

    const objSave = {
       
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
        MaPhieuDG: rowData.MaPhieu,
        LoaiDanhGia: LoaiDanhGia

    };
    arrSaveData.push(objSave);
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

                    item.NguoiSoatXet = IsSoatXet == true ? UserID : null;
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

                SaveKetQuaDanhGia(arrSaveData);
            },
            error: function () {
                DevExpress.ui.notify('Không thể lưu chữ ký. Vui lòng thử lại!', 'error', 3000);
            }
        });
    } else {
        SaveKetQuaDanhGia(arrSaveData);
    }

    });
}

function SaveKetQuaDanhGia(data) {  
    const currentActiveTab = $('.tree-item.active');
    const currentModuleId = currentActiveTab.data('module-id');
    fetch('/api/PheDuyet/PostDanhGiaNCC', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.ok ? res.json() : Promise.reject('Lỗi server'))
        .then(() => {
            DevExpress.ui.notify('Lưu đánh giá thành công!', 'success', 1500);

          
            const popup = $("#PopupDanhGia").dxPopup("instance");
            if (popup) {
                popup.hide();
            }

            setTimeout(() => {
                if (currentModuleId) {
                    if (currentModuleId === 'overview') {
                        SetupTabOverview("overview");
                    } else {
                        loadModuleData(currentModuleId, IsDuyetSelected);
                    }

                    setTimeout(() => {
                        $('.tree-item').removeClass('active');
                        $(`.tree-item[data-module-id="${currentModuleId}"]`).addClass('active');
                    }, 100);
                }

               
                reloadPhieuCount();
            }, 300); 
        })
        .catch(error => {
            console.error('Save error:', error);
            DevExpress.ui.dialog.alert('Lưu thất bại: ' + error, 'Lỗi');
        });
}
/*END */

/*Duyệt phiếu mua hàng*/
function DuyetMuaHang(isDuyetPhieu) {
    DevExpress.ui.dialog.confirm("Bạn có chắc muốn duyệt/hủy duyệt?", "Xác nhận").done(function (dialogResult) {
        if (!dialogResult) {
            return;
        }

    const urlPQ = `/api/PhanQuyenPOMH/Get?userID=${UserID}`;
    let IsDuyet1 = false;
    let IsDuyet2 = false;
 
    fetch(urlPQ)
        .then(response => response.json())
        .then(jsonpq => {
            jsonpq.forEach(r => {
                if (r.ModuleID === "M.12.03.00") {
                    if (r.AllowDuyet1 !== null && r.AllowDuyet1 !== undefined) {
                        IsDuyet1 = Boolean(r.AllowDuyet1);
                    }
                    if (r.AllowDuyet2 !== null && r.AllowDuyet2 !== undefined) {
                        IsDuyet2 = Boolean(r.AllowDuyet2);
                    }
                }
            });
            var focusedRowData = getFocusedRowData();
            MaPhieu = focusedRowData ? focusedRowData.MaPhieu : "NONE";
           
            if (MaPhieu === "NONE") {
                DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                return Promise.reject("Không có phiếu để duyệt");
            }

            var erpXacNhanPOMuaEntity = {
                Action: "",
                MaPhieu: MaPhieu,
                MaVTID: "",
                MaCLVT: "",
                MauVTID: "",
                KhoVaiID: "",
                NgayXN: formatDateSQL(new Date()),
                IsXacNhan: isDuyetPhieu,
                TrangThai: "",
                NguoiXN: UserID,
                GhiChu: ""
            };

            var action = "";
            if (IsDuyet1) {
                action = "DuyetMuaHang1";
            } else if (IsDuyet2) {
                action = "DuyetMuaHang2";
            } else {
                DevExpress.ui.notify("người dùng này không có quyền duyệt", "error", 2000);
                return;
            }
            erpXacNhanPOMuaEntity.Action = action;

            const urlPost = `/api/ERPCanDoiNguyenPhuLieu/Post`;
            return fetch(urlPost, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(erpXacNhanPOMuaEntity)
            });
        })
        .then(response => response.json())
        .then(result => {
            if (result === "false") {
                DevExpress.ui.notify("Duyệt phiếu thất bại", "error", 2000);
            } else {
                DevExpress.ui.notify("Duyệt phiếu thành công", "success", 2000);

                var focusedRowData = getFocusedRowData();
         
                var detail = `Đợt: ${focusedRowData.TenPhieu}\nPO Mua Hàng: ${focusedRowData.POMua || ''}\nNhà Cung cấp: ${focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;

                var title = `Phiếu Mua hàng ${IsDuyet1 ? "trưởng bộ phận" : IsDuyet2 ? "giám đốc" : ""} đã ${isDuyetPhieu ? "duyệt" : "hủy duyệt"}`;

                var sendto = "PKH";

                sendNotify("M.12.03.00", title, detail, sendto, "ALL", IsDuyet1 ? "0" : IsDuyet2 ? "-1" : "-1");
           
               
                if (currentModuleId) {
                    if (currentModuleId == "overview") {
                        SetupTabOverview("overview");
                    }
                    else {

                        loadModuleData(currentModuleId, IsDuyetSelected);

                    }

                    setTimeout(() => {
                        $('.tree-item').removeClass('active');
                        $(`.tree-item[data-module-id="${currentModuleId}"]`).addClass('active');
                    }, 100);
                }

                reloadPhieuCount();
            }
        })
        .catch(err => {
            console.error("Lỗi khi duyệt:", err);
            DevExpress.ui.notify("Có lỗi xảy ra khi duyệt phiếu", "error", 2000);
        });
    });
}

/**/

/*Duyệt BOM*/
/**/
function FetchBOM_TQ(IsDuyet) {
   
    loadDataFromAPI(`/api/KhoiTaoBOMV1/Get?Action=GETTRANGTHAIBOM&para1=ALL&para2=ALL&para3=null`)
        .then(data => {
            data = data.filter(x => x.TrangThai == (IsDuyet == true ? "Đã active BOM" :"Đã xác nhận BOM"))
            var dataWithIndex = data.map((item, index) => ({
                ...item,
                rowIndex: index + 1
            })).sort((a, b) => a.rowIndex - b.rowIndex);
            $("#gridContainer").dxDataGrid({
                dataSource: dataWithIndex,
                columns: [
                    {
                        dataField: "rowIndex",
                        caption: "STT",
                        width: 50,
                        alignment: "center",
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        sortIndex: 0,
                        visibleIndex: 0,
                        sortingMethod: function (a, b) {
                            return (Number(a) || 0) - (Number(b) || 0);
                        }
                    },
                    { dataField: "MaKH", caption: "MaKH", visible: false},
                    { dataField: "MaHang", caption: "MaHang", visible: false },
                    { dataField: "MaDot", caption: "MaDot", visible: false },
                    { dataField: "TenKH", caption: "Tên khách hàng" },
                    { dataField: "TenHang", caption: "Mã hàng", width: 100  },
                    { dataField: "Dot", caption: "Đợt"},
                    { dataField: "NguoiTao", caption: "Người tạo", },
                    { dataField: "NgayTao", caption: "Ngày tạo", customizeText: function (cellInfo) { return ConvertDate(cellInfo.value); }, allowFiltering: false  },
                    { dataField: "TrangThai", caption: "Trạng thái", },
                    { dataField: "NgayTT", caption: "Ngày TH", width: 100, customizeText: function (cellInfo) { return cellInfo.value.replace(/\//g, "-") }, allowFiltering: false },
                    {
                        caption: " ", allowFiltering: false,
                        minWidth: 110, width: 110,
                        cellTemplate: function (container, options) {

                            $("<span>").html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                                .css("cursor", "pointer")
                                .on("click", function () {
                                    initDetailGrid();
                                    var moduleFocused = options.data.ModuleID;
                                    showDetailBOMPopup(options.data, options.component.option("customModuleId"))

                                }).appendTo(container);
                        }
                    }                ],
                showBorders: true,
                showRowLines: true,
                rowAlternationEnabled: true,
                columnAutoWidth: true,
                hoverStateEnabled: true,
                searchPanel: {
                    visible: true,
                    placeholder: "Tìm kiếm..."
                   
                },
                onToolbarPreparing: function (e) {
                    
                    let searchItem = e.toolbarOptions.items.find(i => i.name === "searchPanel");

            
                    e.toolbarOptions.items = [];
            
                    if (searchItem) {
                        e.toolbarOptions.items.push(searchItem);
                    }

                    let toolbarConfig = generateToolbar(currentModuleId);
                    e.toolbarOptions.items.push(...toolbarConfig);
                },

                headerFilter: { visible: false },
                filterRow: { visible: false },
                scrolling: {
                    mode: 'standard',
                    useNative: true,
                    showScrollbar: 'always',
                },
            
            });
        })
        .catch(error => {
            console.error('Error loading BOM:', error);
            $("#gridContainer").dxDataGrid({ dataSource: [] });
            
        });
}

function showDetailBOMPopup(rowData, ModuleID) {
    if (!rowData) {
        console.warn("Không tìm thấy cấu hình action cho module: " + ModuleID);
        return;
    }

    let popup = $("#detailPopup").dxPopup({
        visible: false,
        title: `${rowData.TenPhieu || rowData.Dot}`,
        width: "100vw",
        height: "100vh",
        showCloseButton: false,
        dragEnabled: false,
        hideOnOutsideClick: true,
        shading: true,
        shadingColor: "rgba(0,0,0,0.5)",
        wrapperAttr: {
            class: "dx-popup"
        },
        toolbarItems: [
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Duyệt",
                    type: "success",
                    icon: "check",
                    onClick: function () {
                       
                        var arrDuyetPhieu = [];
                        var focusedRowData = getFocusedRowData();
                        let MaPhieu = focusedRowData ? focusedRowData.MaDot || focusedRowData.MaPhieu : "NONE";

                            if (MaPhieu === "NONE") {
                                DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                                return;
                            }
                           
                            let obj = {
                                Action: "DuyetBOM",
                                MaPhieu: MaPhieu,
                                IsDuyet: true,
                                NguoiPheDuyet: UserID,
                                GhiChu: ""
                            };
                        arrDuyetPhieu.push(obj);
                     
                        DuyetPhieu(arrDuyetPhieu, true).then(result => {
                            if (result) {
                                var detail = `Đợt: ${focusedRowData.Dot || focusedRowData.TenPhieu}\nMã Hàng: ${focusedRowData.TenHang || ''}\nKhách hàng: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                var title = "BOM đã được ban hành";
                                var sendto = "PKH";
                                sendNotify("M.04.02.00", title, detail, sendto);
                                if (currentModuleId == "overview") {
                                    MaKH = rowData.MaNCC
                                    MaHang = rowData.MaDH,
                                     MaDot = rowData.MaPhieu
                                }
                                else {
                                    MaKH = rowData.MaKH
                                    MaHang = rowData.MaHang,
                                    MaDot = rowData.MaDot


                                }
                                getTSAsync(MaKH, MaHang, UserID, MaDot)
                                popup.hide();
                            }
                        });
                        

                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Hủy duyệt",
                    type: "danger",
                    icon: "close",
                    onClick: function () {

                        var arrDuyetPhieu = [];
                        var focusedRowData = getFocusedRowData();
                        let MaPhieu = focusedRowData ? focusedRowData.MaDot || focusedRowData.MaPhieu : "NONE";

                        if (MaPhieu === "NONE") {
                            DevExpress.ui.notify("Vui lòng chọn phiếu để duyệt", "warning", 2000);
                            return;
                        }
                       
                        let obj = {
                            Action: "DuyetBOM",
                            MaPhieu: MaPhieu,
                            IsDuyet: false,
                            NguoiPheDuyet: UserID,
                            GhiChu: ""
                        };
                        arrDuyetPhieu.push(obj);
                        DuyetPhieu(arrDuyetPhieu, false).then(result => {
                            if (result) {
                                var detail = `Đợt: ${focusedRowData.Dot || focusedRowData.TenPhieu}\nMã Hàng: ${focusedRowData.TenHang || ''}\nKhách hàng: ${focusedRowData.TenNhaCC || focusedRowData.TenKH || focusedRowData.TenNhaCungCap}`;
                                var title = "BOM đã hủy ban hành";
                                var sendto = "PKH";                                                           
                                sendNotify("M.04.00.02", title, detail, sendto)
                                popup.hide();
                            }
                        });
                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Đóng",
                    type: "normal",
                    onClick: function () {
                        popup.hide();
                    }
                }
            }
        ],


    }).dxPopup("instance");

    popup.show();
    if (currentModuleId == "overview") {
        FetchBOM_Detail(rowData.MaNCC, rowData.MaDH, rowData.MaPhieu)
    }
    else {
        FetchBOM_Detail(rowData.MaKH, rowData.MaHang, rowData.MaDot)
    }
 
    
}

function FetchBOM_Detail(maKH, maHang, maDot) {
    const detailGrid = $("#detailGridContainer").dxDataGrid("instance");

    if (!detailGrid) {
        console.error("Detail grid chưa được khởi tạo");
        return;
    }

    loadDataFromAPI(`/api/PheDuyet/GETBOM?para1=${maKH}&para2=${maHang}&para3=${maDot}`)
        .then(data => {
            let colorsColumns = [];         
            if (data && data.length > 0) {
                const firstRow = data[0];
                Object.keys(firstRow).forEach(key => {
                    if (key.includes('@') && key !== 'Mau') {
                        const colorName = key.split('@')[0];
                        colorsColumns.push({
                            dataField: key,
                            caption: colorName,
                            width: 90,
                            alignment: "center",                           
                            cssClass: "col-soluong"
                        });
                    }
                });
            }
            $("#detailGridContainer").dxDataGrid({
                dataSource: data,
                columns: [
                    {
                        dataField: "STT", caption: "STT", width: 40, alignment: "center", sortOrder: "asc",
                        sortIndex: 1  },

                    {
                        dataField: "NPL", caption: "NPL", groupIndex: 1,
                        sortIndex: 0,
                        sortOrder: "desc",

                        groupCellTemplate: function (cellElement, cellInfo) {
                            const items = cellInfo.data.items;
                            cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.data.key == true ? "Nguyên liệu" : "Phụ liệu"}</span>`);
                        },
                    },
                    {
                        dataField: "TenNhom", caption: "Chủng loại"

                    },
                    {
                        dataField: "TenNhomChiTiet", caption: "Chủng loại chi tiết"

                    },

                    { dataField: "MaVTGhep", caption: "Mã vật tư", alignment: "center", visible: false },
                    { dataField: "MaVT", caption: "Item Code" },
                    { dataField: "ChiTiet", caption: "Mô tả" },


                    { dataField: "KhoVai", caption: "Khổ/Size", width: 80 },
                    { dataField: "TenDVVT", caption: "Đơn vị", width: 60 },

                    {
                        dataField: "DinhMucChung",
                        caption: "Định mức khách hàng",
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        width: 80,
                        alignment: "right",
                        format: "#,##0.####"
                    },

                    {
                        dataField: "DinhMucHaoHut", caption: "%Hao hụt", width: 60, allowFiltering: false,
                        allowHeaderFiltering: false
                    },
                    { dataField: "Size", caption: "Size", width: 120 },
                    {
                        caption: "Màu sản phẩm",   // band cấp 1
                        alignment: "center",
                        columns: [
                            ...colorsColumns
                        ]

                    },
                 
                {
                    dataField: "TrangThai",
                    caption: "Trạng thái",
                   
                    cellTemplate: function (container, options) {
                        const status = options.value;
                        let color = status === "Đã xác nhận" ? "#28a745" : "#6c757d";

                        $("<div>")
                            .css({
                                "padding": "3px 8px",
                                "background-color": color,
                                "color": "white",
                                "border-radius": "3px",
                                "text-align": "center",
                                "font-size": "11px"
                            })
                            .text(status || "")
                            .appendTo(container);
                    }
                },
                { dataField: "GhiChu", caption: "Ghi chú" },

                  
                   
                ],
                showBorders: true,
                showRowLines: true,
                rowAlternationEnabled: true,
                columnAutoWidth: true,
                hoverStateEnabled: true,
                searchPanel: {
                    visible: true,
                    placeholder: "Tìm kiếm...",
                 
                },
                headerFilter: { visible: false },
                filterRow: { visible: false },
                scrolling: {
                    mode: 'standard',
                    useNative: true,
                    showScrollbar: 'always',
                    scrollByContent: true,
                    scrollByThumb: true,
                },
               
            });
        })
        .catch(error => {
            console.error('Error loading BOM', error);
            $("#detailGridContainer").dxDataGrid({ dataSource: [] });

        });
}

function CheckActiveBOM(MaDot) {
    return $.ajax({
        url: `/api/PheDuyet/Get?action=CheckActiveBOM&para1=${MaDot}`,
        contentType: 'application/json;charset=utf-8',
        type: 'GET'
    });
}
/**/
function sendNotify(ModuleID, title, detail, sendTo,BoPhan = "ALL",Status = -1) {

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
/**/
/*Duyệt phiếu*/
async function DuyetPhieu(arrDuyetPhieu, isDuyetPhieu) {
    return new Promise(async (resolve, reject) => {
    
        if (arrDuyetPhieu[0].Action == "DuyetBOM") {
            try {
                const data = await CheckActiveBOM(arrDuyetPhieu[0].MaPhieu);
                if (data && data.length > 0) {
                    let msg = data.map(x => `• Dot: ${x.Dot}`).join('\n');
                    showNotify(`Còn ${data.length} vật tư chưa xác nhận!\n${msg}`, "warning", 5000);
                    return resolve(false);
                }
            } catch (err) {
                return resolve(false);
            }
        }

        // Hiện dialog xác nhận
        DevExpress.ui.dialog.confirm(
            `Bạn có chắc muốn ${isDuyetPhieu ? "duyệt" : "hủy duyệt"}?`,
            "Xác nhận"
        ).done(function (dialogResult) {
            if (!dialogResult) {
                return resolve(false);
            }

            fetch('/api/PheDuyet/Post', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(arrDuyetPhieu)
            })
                .then(res => res.ok ? res.json() : Promise.reject('Lỗi server'))
                .then(() => {
                    DevExpress.ui.notify(
                        isDuyetPhieu ? 'Duyệt phiếu thành công' : 'Đã hủy duyệt phiếu',
                        'success',
                        1500
                    );
                   
                    if (currentModuleId) {
                        if (currentModuleId == "overview") {
                            SetupTabOverview("overview");
                        } else {
                            loadModuleData(currentModuleId, IsDuyetSelected);
                        }
                        setTimeout(() => {
                            $('.tree-item').removeClass('active');
                            $(`.tree-item[data-module-id="${currentModuleId}"]`).addClass('active');
                            
                        }, 100);
                    }
                    reloadPhieuCount();
                    resolve(true); // ✅ Thành công
                })
                .catch(error => {
                    console.error('Save error:', error);
                    resolve(false); // ❌ Thất bại
                });
        });
    });
}

function reloadPhieuCount() {
    return new Promise(async (resolve, reject) => {
        try {
            const res = await fetch(`/api/PheDuyet/Get?action=GetCountPhieuCanDuyet&para1=${UserID}`);
            if (!res.ok) return resolve(false);

            const data = await res.json();
            arrCountPhieuChuaDuyet = data;

            data.forEach(item => {
                const moduleElement = $(`.tree-item[data-module-id="${item.ModuleID}"]`);
                const countSpan = moduleElement.find('.so-phieu-can-duyet');
                if (item.SoPhieu > 0) {
                    if (countSpan.length) {
                        countSpan.text(`(${item.SoPhieu})`);
                    } else {
                        moduleElement.find('span').first().after(
                            ` <span class="so-phieu-can-duyet">(${item.SoPhieu})</span>`
                        );
                    }
                } else {
                    countSpan.remove();
                }
            });

            resolve(true);
        } catch (err) {
            console.error('reloadPhieuCount error:', err);
            reject(err); 
        }
    });
}

$(function () {
    InitSignalture();
    initGrid();
    //GetCountPhieuCanDuyet();
    GetModule();
    initializeSubDetailPopup();   
    renderDefaultTab();
    reloadPhieuCount();
});