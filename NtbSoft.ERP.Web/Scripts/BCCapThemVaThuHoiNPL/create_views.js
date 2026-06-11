$(document).ready(function () {
    localStorage.removeItem('MaHang');
    localStorage.removeItem('MaLenh');
    localStorage.removeItem('MaKH');

    $("#grid-cap-them").dxDataGrid({
        /*dataSource: [],*/
        dataSource: new DevExpress.data.CustomStore({
            key: "MaLenh", // Thay bằng key thực tế của bạn
            load: function () {
                const startDate = $("#select-startdate").dxDateBox("instance").option("value");
                const endDate = $("#select-enddate").dxDateBox("instance").option("value");

                const formatDate = (date) => {
                    if (!date) return null;
                    const year = date.getFullYear();
                    const month = String(date.getMonth() + 1).padStart(2, '0');
                    const day = String(date.getDate()).padStart(2, '0');
                    return `${year}-${month}-${day}`;
                };
                return $.getJSON('/api/baocao/get', {
                    action: 'baocaocapthem',
                    param1: localStorage.getItem("MaHang"),
                    param2: localStorage.getItem("MaLenh"),
                    param3: localStorage.getItem("MaKH"),
                    param4: formatDate(startDate),
                    param5: formatDate(endDate)
                });
            }
        }),
        showBorders: true,
        paging: {
            enabled: false
        },
        showRowLines: true,
        showColumnLines: true,
        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        filterRow: { visible: true },
        width: "100%",
        columns: [
            {
                caption: "STT",
                alignment: "center",
                width: 50,
                cellTemplate: function (container, options) {
                    const index = options.component.pageIndex() * options.component.pageSize() + options.rowIndex + 1;
                    container.text(options.row.data ? (options.component.getVisibleRows().filter(r => r.rowType === 'data').indexOf(options.row) + 1) : "");
                }
            },
            { dataField: "MaLenh", caption: "Mã lệnh", visible: false },
            { dataField: "PhieuTH", caption: "Phiếu CT" },
            { dataField: "MaNPL", caption: "Mã NPL", visible: false },
            { dataField: "CustomGr1", caption: "Mã lệnh", groupIndex: 0 },
            { dataField: "ItemCode", caption: "ItemCode" },
            { dataField: "MoTa", caption: "Mô tả" },
            { dataField: "MauVT", caption: "Màu vật tư" },
            { dataField: "KhoVaiID", caption: "Khổ/size" },
            {
                dataField: "NgayDK",
                caption: "Ngày đăng ký",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            {
                dataField: "NgayTH",
                caption: "Ngày cấp thêm",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            {
                dataField: "NgayThucHien",
                caption: "Ngày thực hiện",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            {
                dataField: "TenNV",
                caption: "Người thực hiện",
                calculateCellValue: function (data) {
                    return data.TenNV ?? data.NguoiTH;
                }
            },
            { dataField: "SLDK", caption: "SL đăng ký cấp thêm" },
            { dataField: "SLNhap", caption: "SL cấp thực tế" },
            { dataField: "TenDVCD", caption: "Đơn vị" },
            { dataField: "DonGia", caption: "Đơn giá" },
            {
                dataField: "ThanhTien",
                caption: "Thành tiền",
                format: {
                    type: "fixedPoint",
                    precision: 3
                }
            },
            {
                dataField: "TrongDinhMuc",
                caption: "Trong định mức",
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    $("<div/>").dxCheckBox({
                        value: options.value === 1,
                        readOnly: true
                    }).appendTo(container);
                }
            },
            {
                dataField: "NgoaiDinhMuc",
                caption: "Ngoài định mức",
                alignment: "center",
                allowEditing: false,
                cellTemplate: function (container, options) {
                    $("<div/>").dxCheckBox({
                        value: options.value === 1,
                        readOnly: true
                    }).appendTo(container);
                }
            },
            { dataField: "GhiChu", caption: "Lí do" },
        ],
        summary: {
            totalItems: [{
                column: "ThanhTien",
                summaryType: "sum",
                valueFormat: "#,##0",
                displayFormat: "Tổng: {0}",
                showInColumn: "ThanhTien"
            }]
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).css({
                    "background-color": "#f39c12",
                    "color": "#ffffff",
                    "text-align": "center",
                    "vertical-align": "middle",
                    "font-weight": "bold"
                });
            }
            if (e.rowType === "data") {
                //$(e.cellElement).css("text-align", "center");
            }
        }
    });

    $("#grid-thu-hoi").dxDataGrid({
        dataSource: [],
        showBorders: true,
        paging: {
            enabled: false
        },
        showRowLines: true,
        showColumnLines: true,
        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        filterRow: { visible: true },
        width: "100%",
        columns: [
            {
                caption: "STT",
                alignment: "center",
                width: 50,
                cellTemplate: function (container, options) {
                    const index = options.component.pageIndex() * options.component.pageSize() + options.rowIndex + 1;
                    container.text(options.row.data ? (options.component.getVisibleRows().filter(r => r.rowType === 'data').indexOf(options.row) + 1) : "");
                }
            },
            { dataField: "PhieuTH", caption: "Phiếu TH" },
            { dataField: "MaNPL", caption: "Mã NPL", visible: false },
            { dataField: "ItemCode", caption: "ItemCode" },
            { dataField: "MoTa", caption: "Mô tả" },
            { dataField: "MaLenh", caption: "Mã lệnh", groupIndex: 0 },
            { dataField: "MaVT", caption: "Mã vật tư", visible: false },
            { dataField: "MauVT", caption: "Màu vật tư" },
            { dataField: "KhoVai", caption: "Khổ/size" },
            {
                dataField: "NgayDK",
                caption: "Ngày đăng ký",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            {
                dataField: "NgayTH",
                caption: "Ngày thu hồi",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            { dataField: "NguoiTH", caption: "Người thực hiện" },
            { dataField: "Dot", caption: "Đợt", visible: true },
            { dataField: "SLDK", caption: "SL đăng ký thu hồi" },
            { dataField: "ThuHoi", caption: "SL thu hồi thực tế" },
            { dataField: "TenDVCD", caption: "Đơn vị" },
            { dataField: "DonGia", caption: "Đơn giá" },
            {
                dataField: "ThanhTien",
                caption: "Thành tiền",
                format: {
                    type: "fixedPoint",
                    precision: 3
                }
            },
            { dataField: "Palet", caption: "Lí do" },
        ],
        summary: {
            totalItems: [{
                column: "ThanhTien",
                summaryType: "sum",
                valueFormat: "#,##0",
                displayFormat: "Tổng: {0}",
                showInColumn: "ThanhTien"
            }]
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).css({
                    "background-color": "#f39c12",
                    "color": "#ffffff",
                    "text-align": "center",
                    "vertical-align": "middle",
                    "font-weight": "bold"
                });
            }
            if (e.rowType === "data") {
                //$(e.cellElement).css("text-align", "center");
            }
        }
    });

    $("#grid-cap-them-ngoai-don-hang").dxDataGrid({
        dataSource: [],
        showBorders: true,
        paging: {
            enabled: false
        },
        showRowLines: true,
        showColumnLines: true,
        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        filterRow: { visible: true },
        width: "100%",
        columns: [
            {
                caption: "STT",
                width: 50,
                allowEditing: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    container.text(options.rowIndex + 1);
                }
            },
            { dataField: "PhieuCT_NgoaiDH", caption: "Phiếu CT_BN" },
            { dataField: "MaNPL", caption: "Mã NPL", visible: false },
            { dataField: "ItemCode", caption: "ItemCode" },
            { dataField: "MoTa", caption: "Mô tả" },
            { dataField: "MaVT", caption: "Mã vật tư", visible: false },
            { dataField: "MauVT", caption: "Màu vật tư" },
            { dataField: "KhoVai", caption: "Khổ/size" },
            {
                dataField: "NgayDK",
                caption: "Ngày đăng ký",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            {
                dataField: "NgayTH",
                caption: "Ngày cấp thêm",
                dataType: "date",
                format: "dd/MM/yyyy"
            },
            { dataField: "NguoiTH", caption: "Người thực hiện" },
            { dataField: "Dot", caption: "Đợt", visible: true },
            { dataField: "SLDK", caption: "SL đăng ký cấp thêm" },
            { dataField: "SLCapThem", caption: "SL cấp thêm thực tế" },
            { dataField: "TenDVVT", caption: "Đơn vị" },
            { dataField: "DonGia", caption: "Đơn giá" },
            {
                dataField: "ThanhTien",
                caption: "Thành tiền",
                format: {
                    type: "fixedPoint",
                    precision: 3
                }
            },
            { dataField: "LiDo", caption: "Lí do" },
        ],
        summary: {
            totalItems: [{
                column: "ThanhTien",
                summaryType: "sum",
                valueFormat: "#,##0",
                displayFormat: "Tổng: {0}",
                showInColumn: "ThanhTien"
            }]
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).css({
                    "background-color": "#f39c12",
                    "color": "#ffffff",
                    "text-align": "center",
                    "vertical-align": "middle",
                    "font-weight": "bold"
                });
            }
            if (e.rowType === "data") {
                //$(e.cellElement).css("text-align", "center");
            }
        }
    });

    $('a[href="#tab-cap-them"]').tab('show');
    $('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
        var targetTab = $(e.target).attr("href");

        if (targetTab === "#tab-thu-hoi") {
            const maHang = localStorage.getItem("MaHang")
            const maLenh = localStorage.getItem("MaLenh")
            const maKH = localStorage.getItem("MaKH")

            if (maHang) {
                loadDataThuHoi(maHang, maLenh, maKH);
            } else {
                loadDataThuHoi();
            }
            $("#grid-thu-hoi").dxDataGrid("instance").repaint();
        }
        else if (targetTab === "#tab-cap-them") {
            const maHang = localStorage.getItem("MaHang")
            const maLenh = localStorage.getItem("MaLenh")
            const maKH = localStorage.getItem("MaKH")

            if (maHang) {
                loadDataCapThem(maHang, maLenh, maKH);
            } else {
                loadDataCapThem();
            }
            $("#grid-cap-them").dxDataGrid("instance").repaint();
        } else if (targetTab === "#tab-cap-them-ngoai-don-hang") {
            loadDataCapThemNgoaiDH();
            $("#grid-cap-them-ngoai-don-hang").dxDataGrid("instance").repaint();
        }
    });

    $('#Layer_1').click();
});