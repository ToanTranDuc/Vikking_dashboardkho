$(function () {

    // =========================
    // FIX DEVEXTREME OVERLAY
    // =========================
    $(document).ready(function () {
        setTimeout(function () {
            $('.dx-loadpanel').remove();
            $('.dx-overlay-wrapper.dx-state-invisible').remove();
            $('.dx-overlay-shader').remove();
        }, 500);
    });

    // Disable loading indicator của DevExtreme
    DevExpress.ui.dxLoadPanel.defaultOptions({
        device: { deviceType: "desktop" },
        options: {
            showIndicator: false,
            showPane: false,
            shading: false,
            closeOnOutsideClick: true
        }
    });

    // =========================
    // DATE PICKER
    // =========================
    var fromDateBox = $("#fromDatePicker").dxDateBox({
        type: "date",
        displayFormat: "dd/MM/yyyy",
        value: new Date(new Date().getFullYear(), 0, 1),
        dropDownOptions: {
            hideOnOutsideClick: true
        }
    }).dxDateBox("instance");

    var toDateBox = $("#toDatePicker").dxDateBox({
        type: "date",
        displayFormat: "dd/MM/yyyy",
        value: new Date(new Date().getFullYear(), 11, 31),
        dropDownOptions: {
            hideOnOutsideClick: true
        }
    }).dxDateBox("instance");

    // =========================
    // DATAGRID
    // =========================
    var grid = $("#pivotGrid").dxDataGrid({
        dataSource: [],
        showBorders: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        wordWrapEnabled: true,

        // QUAN TRỌNG: Bật scrolling
        scrolling: {
            mode: "standard",
            showScrollbar: "always",
            useNative: true
        },

        // QUAN TRỌNG: Set height
        height: "100%",
        toolbar: {
            visible: false
        },
        sorting: { mode: "none" },
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },

        // TẮT PHÂN TRANG
        paging: {
            enabled: false
        },

        // EXPORT EXCEL - QUAN TRỌNG!
        export: {
            enabled: true,
            fileName: "BaoCaoTongHop",
            allowExportSelectedData: false
        },

        summary: {
            totalItems: []
        },

        columns: [],

        onCellPrepared: function (e) {
            if (e.rowType === "header") {

                if (
                    e.column.caption === "No" ||
                    e.column.dataField === "SupplierName" ||
                    e.column.dataField === "Company"
                ) {
                    e.cellElement.css({
                        "background-color": "#e3f2fd",
                        "font-weight": "bold",
                        "text-align": "center"
                    });
                    return;
                }

                if (
                    e.column.caption === "Grand Total Year" ||
                    e.column.dataField === "TotalItemFailed" ||
                    e.column.dataField === "TotalItemPass" ||
                    e.column.dataField === "year_percent_hidden"
                ) {
                    e.cellElement.css({
                        "background-color": "#ffe0b2",
                        "font-weight": "bold",
                        "text-align": "center",
                        "border-left": "1px solid #bbb",
                        "border-right": "1px solid #bbb"
                    });
                    return;
                }

                const monthColors = ["#f8d7da", "#d4edda"];

                const monthMap = {
                    jan: 0, feb: 1, mar: 2, apr: 3,
                    may: 4, jun: 5, jul: 6, aug: 7,
                    sep: 8, oct: 9, nov: 10, dec: 11
                };

                let monthIndex = null;

                if (monthMap[e.column.caption?.toLowerCase()] !== undefined) {
                    monthIndex = monthMap[e.column.caption.toLowerCase()];
                }

                if (e.column.dataField) {
                    Object.keys(monthMap).forEach(m => {
                        if (e.column.dataField.startsWith(m + "_")) {
                            monthIndex = monthMap[m];
                        }
                    });
                }

                if (monthIndex !== null) {
                    e.cellElement.css({
                        "background-color": monthColors[monthIndex % 2],
                        "font-weight": "bold",
                        "text-align": "center",
                        "border-left": "1px solid #bbb",
                        "border-right": "1px solid #bbb"
                    });
                }
            }

            if (e.rowType === "totalFooter") {
                e.cellElement.css({
                    "background-color": "#fff3cd",
                    "font-weight": "bold",
                    "border-top": "2px solid #000"
                });
            }

            if (e.rowType === "data" && e.column.caption === "%") {
                var value = parseFloat(e.value);
                if (value > 10) {
                    e.cellElement.css({
                        "background-color": "#f8d7da",
                        "color": "#721c24",
                        "font-weight": "bold"
                    });
                } else if (value > 5) {
                    e.cellElement.css({
                        "background-color": "#fff3cd",
                        "color": "#856404"
                    });
                }
            }
        }

    }).dxDataGrid("instance");

    // =========================
    // LOAD DATA
    // =========================
    $("#btnLoadReport").on("click", function () {
        loadData();
    });

    // FIX EXPORT EXCEL - ĐÚNG METHOD!
    $("#btnExportExcel").on("click", function () {
        // Import ExcelJS và FileSaver nếu cần
        var workbook = new ExcelJS.Workbook();
        var worksheet = workbook.addWorksheet('Báo Cáo');

        // Cách 1: Nếu có DevExpress.excelExporter
        if (DevExpress.excelExporter) {
            DevExpress.excelExporter.exportDataGrid({
                component: grid,
                worksheet: worksheet,
                autoFilterEnabled: true
            }).then(function () {
                workbook.xlsx.writeBuffer().then(function (buffer) {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'BaoCaoTongHop.xlsx');
                });
            });
        }
        // Cách 2: Fallback - Export đơn giản
        else {
            var dataSource = grid.getDataSource();
            if (dataSource) {
                var items = dataSource.items();
                console.log("Exporting data:", items);
                // Có thể tự export bằng cách khác hoặc hiển thị message
                DevExpress.ui.notify("Đang xuất dữ liệu... (cần cài thêm ExcelJS)", "info", 2000);
            }
        }
    });

    $("#btnRefresh").on("click", function () {
        loadData();
    });

    loadData();

    function loadData() {
        var fromDate = fromDateBox.option("value");
        var toDate = toDateBox.option("value");

        // Xóa overlay
        $('.dx-loadpanel').remove();
        $('.dx-overlay-wrapper').remove();

        $.ajax({
            url: "/api/NhaCC/Get",
            method: "GET",
            data: {
                action: 'GETBAOCAO',
                para: formatDate(fromDate),
                para1: formatDate(toDate),
                para2: '',
                para3: '',
                para4: '',
                para5: ''
            },
            success: function (res) {
                res.forEach(x => {
                    if (!x.Company) x.Company = "";
                });

                var groupedData = [];
                var supplierMap = {};

                res.forEach(function (row) {
                    var key = row.SupplierCode + "_" + row.SupplierName + "_" + row.Company;

                    if (!supplierMap[key]) {
                        supplierMap[key] = {
                            SupplierCode: row.SupplierCode,
                            SupplierName: row.SupplierName,
                            Company: row.Company,
                            monthlyData: {}
                        };
                        groupedData.push(supplierMap[key]);
                    }

                    supplierMap[key].monthlyData[row.Thang] = {
                        ItemFailed: row.ItemFailed || 0,
                        ItemPass: row.ItemPass || 0,
                        TotalItems: row.TotalItems || 0
                    };
                });

                var monthPrefixes = ["jan", "feb", "mar", "apr", "may", "jun",
                    "jul", "aug", "sep", "oct", "nov", "dec"];

                groupedData.forEach(function (supplier) {
                    supplier.TotalItemFailed = 0;
                    supplier.TotalItemPass = 0;
                    supplier.TotalItems = 0;

                    for (var i = 0; i < 12; i++) {
                        supplier[monthPrefixes[i] + "_failed_hidden"] = 0;
                        supplier[monthPrefixes[i] + "_total_hidden"] = 0;
                    }

                    for (var month = 1; month <= 12; month++) {
                        if (!supplier.monthlyData[month]) continue;

                        var d = supplier.monthlyData[month];
                        supplier.TotalItemFailed += d.ItemFailed;
                        supplier.TotalItemPass += d.ItemPass;
                        supplier.TotalItems += d.TotalItems;

                        var prefix = monthPrefixes[month - 1];
                        supplier[prefix + "_failed_hidden"] = d.ItemFailed;
                        supplier[prefix + "_total_hidden"] = d.ItemPass;
                    }
                });

                var monthColumns = generateMonthColumns(fromDate, toDate);

                // FIX CỘT NO - DÙNG DATA INDEX THAY VÌ ROW INDEX
                grid.option("columns", [
                    {
                        caption: "No",
                        width: 60,
                        alignment: "center",
                        fixed: true,
                        fixedPosition: "left",
                        allowExporting: true,
                        calculateCellValue: function (rowData) {
                            // Tìm index thực tế trong dataSource
                            var dataSource = grid.getDataSource();
                            if (dataSource && dataSource.items()) {
                                return dataSource.items().indexOf(rowData) + 1;
                            }
                            return "";
                        }
                    },
                    {
                        caption: "Supplier",
                        dataField: "SupplierName",
                        width: 250,
                        fixed: true,
                        fixedPosition: "left"
                    },
                    {
                        caption: "Customer send / VKVN buy",
                        dataField: "Company",
                        width: 200,
                        fixed: true,
                        fixedPosition: "left"
                    },
                    ...monthColumns,
                    {
                        caption: "Grand Total Year",
                        alignment: "center",
                        columns: [
                            {
                                caption: "ITEM FAILED",
                                alignment: "center",
                                width: 100,
                                dataField: "TotalItemFailed",
                                calculateCellValue: function (row) {
                                    return row.TotalItemFailed || 0;
                                }
                            },
                            {
                                caption: "TOTAL ITEMS RECEIVED",
                                alignment: "center",
                                width: 150,
                                dataField: "TotalItemPass",
                                calculateCellValue: function (row) {
                                    return row.TotalItemPass || 0;
                                }
                            },
                            {
                                caption: "%",
                                alignment: "center",
                                dataField: "year_percent_hidden",
                                width: 80,
                                calculateCellValue: function (row) {
                                    if (!row.TotalItemPass || row.TotalItemPass === 0) {
                                        row.year_percent_hidden = 0;
                                        return "0%";
                                    }
                                    var percent = (row.TotalItemFailed / row.TotalItemPass * 100).toFixed(0);
                                    row.year_percent_hidden = parseFloat(percent);
                                    return percent + "%";
                                }
                            }
                        ]
                    }
                ]);

                grid.option("summary", {
                    totalItems: generateSummaryItems(fromDate, toDate)
                });

                grid.option("dataSource", groupedData);
                console.log("Grouped data:", groupedData);

                // Xóa overlay sau khi load xong
                setTimeout(function () {
                    $('.dx-loadpanel').remove();
                    $('.dx-overlay-wrapper').remove();
                }, 100);
            },
            error: function (xhr, status, error) {
                console.error("Error loading data:", error);
                DevExpress.ui.notify("Error loading data: " + error, "error", 3000);

                $('.dx-loadpanel').remove();
                $('.dx-overlay-wrapper').remove();
            }
        });
    }

    // =========================
    // HELPERS
    // =========================

    function generateMonthColumns(fromDate, toDate) {
        var monthNames = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN",
            "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
        var monthPrefixes = ["jan", "feb", "mar", "apr", "may", "jun",
            "jul", "aug", "sep", "oct", "nov", "dec"];

        var startMonth = fromDate.getMonth();
        var endMonth = toDate.getMonth();
        var startYear = fromDate.getFullYear();
        var endYear = toDate.getFullYear();
        var columns = [];

        if (startYear === endYear) {
            for (var i = startMonth; i <= endMonth; i++) {
                columns.push(createMonthBand(monthNames[i], i + 1, monthPrefixes[i]));
            }
        } else {
            for (var i = startMonth; i <= 11; i++) {
                columns.push(createMonthBand(monthNames[i], i + 1, monthPrefixes[i]));
            }
            for (var i = 0; i <= endMonth; i++) {
                columns.push(createMonthBand(monthNames[i], i + 1, monthPrefixes[i]));
            }
        }

        return columns;
    }

    function generateSummaryItems(fromDate, toDate) {
        var monthPrefixes = ["jan", "feb", "mar", "apr", "may", "jun",
            "jul", "aug", "sep", "oct", "nov", "dec"];
        var startMonth = fromDate.getMonth();
        var endMonth = toDate.getMonth();
        var startYear = fromDate.getFullYear();
        var endYear = toDate.getFullYear();

        var summaryItems = [
            {
                column: "SupplierName",
                summaryType: "sum",
                customizeText: function () {
                    return "Grand Total";
                }
            }
        ];

        var monthsToInclude = [];

        if (startYear === endYear) {
            for (var i = startMonth; i <= endMonth; i++) {
                monthsToInclude.push(i);
            }
        } else {
            for (var i = startMonth; i <= 11; i++) {
                monthsToInclude.push(i);
            }
            for (var i = 0; i <= endMonth; i++) {
                monthsToInclude.push(i);
            }
        }

        monthsToInclude.forEach(function (monthIndex) {
            var prefix = monthPrefixes[monthIndex];
            summaryItems.push(
                {
                    name: prefix + "_failed",
                    summaryType: "sum",
                    column: prefix + "_failed_hidden",
                    customizeText: function (e) { return e.value; }
                },
                {
                    name: prefix + "_total",
                    summaryType: "sum",
                    column: prefix + "_total_hidden",
                    customizeText: function (e) { return e.value; }
                },
                {
                    name: prefix + "_percent",
                    summaryType: "sum",
                    column: prefix + "_percent_hidden",
                    customizeText: function (e) {
                        var totalFailed = grid.getTotalSummaryValue(prefix + "_failed");
                        var totalPass = grid.getTotalSummaryValue(prefix + "_total");
                        if (!totalPass || totalPass === 0) return "0%";
                        return ((totalFailed / totalPass) * 100).toFixed(0) + "%";
                    }
                }
            );
        });

        summaryItems.push(
            { name: "year_failed", summaryType: "sum", column: "TotalItemFailed", customizeText: function (e) { return e.value; } },
            { name: "year_pass", summaryType: "sum", column: "TotalItemPass", customizeText: function (e) { return e.value; } },
            {
                name: "year_percent",
                summaryType: "sum",
                showInColumn: "TotalItemPass",
                customizeText: function (e) {
                    var totalFailed = grid.getTotalSummaryValue("year_failed");
                    var totalPass = grid.getTotalSummaryValue("year_pass");
                    if (!totalPass || totalPass === 0) return "0%";
                    return ((totalFailed / totalPass) * 100).toFixed(0) + "%";
                }
            }
        );

        return summaryItems;
    }

    function createMonthBand(caption, month, prefix) {
        return {
            caption: caption,
            alignment: "center",
            columns: [
                {
                    caption: "ITEM FAILED",
                    dataField: prefix + "_failed_hidden",
                    alignment: "center",
                    width: 100
                },
                {
                    caption: "TOTAL ITEMS RECEIVED",
                    dataField: prefix + "_total_hidden",
                    alignment: "center",
                    width: 150
                },
                {
                    caption: "%",
                    alignment: "center",
                    dataField: prefix + "_percent_hidden",
                    width: 80,
                    calculateCellValue: function (row) {
                        var pass = row[prefix + "_total_hidden"];
                        if (!pass) return "0%";
                        return ((row[prefix + "_failed_hidden"]) / pass * 100).toFixed(0) + "%";
                    }
                }
            ]
        };
    }

    function formatDate(date) {
        if (!date) return null;
        var d = new Date(date);
        return d.getFullYear() + "-"
            + String(d.getMonth() + 1).padStart(2, "0") + "-"
            + String(d.getDate()).padStart(2, "0");
    }
});