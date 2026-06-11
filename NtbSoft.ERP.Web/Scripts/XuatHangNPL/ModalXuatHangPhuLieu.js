// ========== CACHE LAYER ==========
const _apiCache = {};

function getCacheKey(action, params) {
    return action + "|" + JSON.stringify(params ?? {});
}
function getCached(key) {
    return Object.prototype.hasOwnProperty.call(_apiCache, key)
        ? _apiCache[key]
        : null;
}
function setCache(key, data) {
    _apiCache[key] = data;
}

function clearCacheByPrefix(prefix) {
    Object.keys(_apiCache)
        .filter(k => k.startsWith(prefix))
        .forEach(k => delete _apiCache[k]);
}

/** Xóa toàn bộ cache (dùng khi đóng modal) */
function clearAllCache() {
    Object.keys(_apiCache).forEach(k => delete _apiCache[k]);
}


// ==== MODAL THỐNG KÊ ====

$(function () {

    $("#locPhieuCap").select2({
        dropdownParent: "#modalThongKe",
        minimumResultsForSearch: Infinity
    })

    $("#locPhieuCap").on("change", async function () {

        const valueLoc = $(this).val();
        const tab = getCurrentTab(); // lấy tab hiện tại

        // reset UI
        $("#itemCodeInput").val('');
        batchDataMalenh = [];
        itemCodeJoin = [];
        $("#itemList").empty();
        toggleContainer(valueLoc);
        if (valueLoc == "1") {
            // ===== CHƯA CẤP =====
            $(".colTuNgayPC").addClass("d-none");
            $(".colDenNgayPC").addClass("d-none");

            if (tab === 0) {
                await GetMaLenhChuaCapXuatHang();
            } else {
                await GetMaLenhChuaCapSoanHang();
            }

        } else {
            // ===== ĐÃ CẤP =====
            $(".colTuNgayPC").removeClass("d-none");
            $(".colDenNgayPC").removeClass("d-none");

            if (tab === 0) {
                await GetDaCapXuatHang(); 
            } else {
                await GetItemCode(); 
            }
        }
    });

    // ===== Mở Modal Thống kê =======
    $("#modalThongKe").on("show.bs.modal", function () {
        $("#tab-xuat-tab").tab("show");
        $("#locPhieuCap").val("1");
        $("#locPhieuCap").trigger("change");
    })
    //===== Đóng Modal Thống kê ==========
    $("#modalThongKe").on("hide.bs.modal", function () {
        // ----- Clear Cache -----
        clearAllCache();
        isFirstChange = true;
        selectedRowMaLenh = null;
        itemCodeJoin = [];
        batchDataMalenh = [];

       
        $("#itemCodeInput").val('');
        $("#itemList").empty();

        // ----- reset data picker 3, 4 từ ngày đến ngày -----
        try {
            const firstDayOfMonth = moment().startOf('month').toDate();
            const today = new Date();

           
            $("#tuNgayPhieuYC").off("change", handleDateChange);
            $("#denNgayPhieuYC").off("change", handleDateChange);

            if (window.pickerTuNgay) {
                window.pickerTuNgay.dates.setValue(
                    tempusDominus.DateTime.convert(firstDayOfMonth)
                );
            }
            if (window.pickerDenNgay) {
                window.pickerDenNgay.dates.setValue(
                    tempusDominus.DateTime.convert(today)
                );
            }

            // ----- Bật lại event -----
            $("#tuNgayPhieuYC").on("change", handleDateChange);
            $("#denNgayPhieuYC").on("change", handleDateChange);

        } catch (e) {
            console.warn("Reset datepicker lỗi:", e);
        }

        createViewDxDataGridPhieuCap1([]);
        createViewDxDataGridPhieuCap2([]);
        createViewDxDataGridPhieuCap3([]);
        createViewDxDataGridPhieuCap4([]);
    });

    $("#tuNgayPhieuYC").on("change", handleDateChange);
    $("#denNgayPhieuYC").on("change", handleDateChange);

    $("#thongKeTabs .nav-link").on("click", async function () {

        const loc = $("#locPhieuCap").val();
        const clickedId = $(this).attr("id");
        const tab = clickedId === "tab-xuat-tab" ? 0 : 1;

        toggleContainer(loc);
        createViewDxDataGridPhieuCap1([]);
        createViewDxDataGridPhieuCap2([]);
        createViewDxDataGridPhieuCap3([]);
        createViewDxDataGridPhieuCap4([]);
        if (loc == "1") {
            // ===== CHƯA CẤP =====
            if (tab === 0) {
                await GetMaLenhChuaCapXuatHang();
            } else {
                await GetMaLenhChuaCapSoanHang();

            }
        } else {
            // ===== ĐÃ CẤP =====
            if (tab === 0) {
                await GetDaCapXuatHang();
            } else {
                await GetItemCode();
            }
        }
    });
})

function getCurrentTab() {
    const activeTab = $("#thongKeTabs .nav-link.active").attr("id");

    if (activeTab === "tab-xuat-tab") return 0; // Xuất hàng
    if (activeTab === "tab-soan-tab") return 1; // Soạn hàng

    return 0;
}

function toggleContainer(loc) {
    const tab = getCurrentTab();
    const tabId = tab === 0 ? "#tab-xuat" : "#tab-soan";

    if (loc == "1") { // Chưa cấp
        $(`${tabId} .containerGrid1`).removeClass("d-none");
        $(`${tabId} .containerGrid2`).addClass("d-none");
    } else { // Đã cấp
        $(`${tabId} .containerGrid1`).addClass("d-none");
        $(`${tabId} .containerGrid2`).removeClass("d-none");
    }
}
function getGridId(baseId) {
    const tab = getCurrentTab();
    return tab === 0 ? `${baseId}_xuat` : `${baseId}_soan`;
}

// =========== API ========
//Đã cấp Xuất hàng
async function GetDaCapXuatHang() {
 
    const tuMoment = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY", true);
    const denMoment = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY", true);

    if (!tuMoment.isValid() || !denMoment.isValid() || tuMoment.isAfter(denMoment)) return;

    const tu = tuMoment.format("YYYY-MM-DD");
    const den = denMoment.format("YYYY-MM-DD");
    const capPhat = $(".select_loc").val();
    const key = getCacheKey("GetDSLenhDCapPL", { tu, den, capPhat });
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap1(cached);
        return;
    }

    try {
        const tab = getCurrentTab();
      
        const url = `/api/PhieuXuatHangNPL/Get?Action=GetDSLenhDCapPL&para1=${tu}&para2=${den}&para5=${capPhat}`;
        const response = await fetch(url);
        const data = await response.json();
        setCache(key, data); 
        createViewDxDataGridPhieuCap1([]);
        createViewDxDataGridPhieuCap2([]);
        createViewDxDataGridPhieuCap1(data);
    } catch (err) {
        console.error(err);
    }
}

//Đã cấp Soạn hàng
async function GetItemCode() {
    
    const tuMoment = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY", true);
    const denMoment = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY", true);

    if (!tuMoment.isValid() || !denMoment.isValid() || tuMoment.isAfter(denMoment)) return;

    const tu = tuMoment.format("YYYY-MM-DD");
    const den = denMoment.format("YYYY-MM-DD");
    const capphat = $(".select_loc").val();
    const key = getCacheKey("GetDSLenhDCapSoanHang", { tu, den, capphat });
    const cached = getCached(key);
    if (cached) {
        $("#itemCodeInput").val('');
        batchDataMalenh = cached;
        createViewDxDataGridPhieuCap1(cached);
        return;
    }

    try {
        const url = `/api/PhieuXuatHangNPL/Get?Action=GetDSLenhDCapSoanHang&para2=${tu}&para3=${den}&para1=0&para6=${capphat}`;
        const response = await fetch(url);
        const data = await response.json();
        setCache(key, data); 
        $("#itemCodeInput").val('');
        batchDataMalenh = data;
        createViewDxDataGridPhieuCap1(data);
    } catch (err) {
        console.error(err);
    }
}

//Chưa cấp xuất hàng
async function GetMaLenhChuaCapXuatHang() {
    const key = getCacheKey("GetDSLenhCCapPL");
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap4(cached);
        return;
    }
    try {

        var capPhat = $(".select_loc").val();
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetDSLenhCCapPL&para1=0&para6=${capPhat}`;
        const response = await fetch(url)
        const data = await response.json();

        setCache(key, data);
        createViewDxDataGridPhieuCap4(data)

    } catch (err) {
        console.error(err)
    }
}

//Chưa cấp soạn hàng
async function GetMaLenhChuaCapSoanHang() {
    const key = getCacheKey("GetDSLenhCCapSoanHang");
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap4(cached);
        return;
    }
    try {
        var capPhat = $(".select_loc").val();
        const tab = getCurrentTab();
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetDSLenhCCapSoanHang&para1=0&para6=${capPhat}`;
        const response = await fetch(url)
        const data = await response.json();

        /*  batchDataMalenh = data
          renderListItemCode(data)*/
        setCache(key, data);
        createViewDxDataGridPhieuCap4(data)

    } catch (err) {
        console.error(err)
    }
}
// Chi tiết Đã cấp xuất hàng
async function GetChiTietMaLenhDaCapXuatHang() {
    const key = getCacheKey("GetChiTietLenhDaCap", { selectedRowMaLenh, itemCodeJoin });
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap2(cached);
        return;
    }
    try {
        var capPhat = $(".select_loc").val();
        var loc = $("#locPhieuCap").val();
        const tab = getCurrentTab();
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")

        var url = `/api/PhieuXuatHangNPL/Get?Action=GetChiTietLenhDaCap&para1=${selectedRowMaLenh}&para2=0&para3=${capPhat}&para4=${itemCodeJoin}&para5=${tuNgay}&para6=${denNgay}`;

        const response = await fetch(url)
        const data = await response.json();

        /*batchDataMalenh = data
        renderListItemCode(data)*/

        createViewDxDataGridPhieuCap2(data)

    } catch (err) {
        console.error(err)
    }
}
// Chi tiết Đã cấp soạn hàng
async function GetChiTietMaLenhDaCapSoanHang() {
     const key = getCacheKey("GetChiTietLenhDaCap", { selectedRowMaLenh, itemCodeJoin });
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap2(cached);
        return;
    }
    try {
        var capPhat = $(".select_loc").val();
        const tab = getCurrentTab();
        const tuNgay = moment($("#tuNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        const denNgay = moment($("#denNgayPhieuYC").val(), "DD/MM/YYYY").format("YYYY-MM-DD")

        var url = `/api/PhieuXuatHangNPL/Get?Action=GetChiTietLenhDSDCapSoanHang&para2=${tuNgay}&para3=${denNgay}&para4=${selectedRowMaLenh}&para5=${itemCodeJoin}&para6=${capPhat}`;
        const response = await fetch(url)
        const data = await response.json();

        /*batchDataMalenh = data
        renderListItemCode(data)*/

        createViewDxDataGridPhieuCap2(data)

    } catch (err) {
        console.error(err)
    }
}

//Chi tiết lệnh chưa cấp xuất hàng
async function GetChiTietLenhChuaCapXuatHang() {
    const key = getCacheKey("GetDSLenhChiTietCCapPL", { selectedRowMaLenh });
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap3(cached);
        return;
    }
    try {
        var capPhat = $(".select_loc").val();
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetDSLenhChiTietCCapPL&para1=${selectedRowMaLenh}&para2=0&para6=${capPhat}`;
        const response = await fetch(url);
        const data = await response.json();

        console.log(data);
        createViewDxDataGridPhieuCap3(data)
    } catch (err) {
        console.error(err)
    }
}
// chi tiết chưa cấp soạn hàng
async function GetChiTietLenhChuaCapSoanHang() {
    const key = getCacheKey("GetChiTietDSLenhCCapSoanHang", { selectedRowMaLenh });
    const cached = getCached(key);
    if (cached) {
        createViewDxDataGridPhieuCap3(cached);
        return;
    }
    try {
        var capPhat = $(".select_loc").val();
        var url = `/api/PhieuXuatHangNPL/Get?Action=GetChiTietDSLenhCCapSoanHang&para1=${selectedRowMaLenh}&para2=0&para6=${capPhat}`;
        const response = await fetch(url);
        const data = await response.json();

        console.log(selectedRowMaLenh);
        console.log(data);
        createViewDxDataGridPhieuCap3(data)
    } catch (err) {
        console.error(err)
    }
}

// ====== END API ======

// ========== XÓA CACHE KHI ĐỔI NGÀY hoặc ĐÓNG MODAL ==========

async function handleDateChange() {
    $("#itemCodeInput").val('');
    createViewDxDataGridPhieuCap1([]);
    createViewDxDataGridPhieuCap2([]);
    isFirstChange = true;

    // Invalidate cache liên quan đến ngày
    clearCacheByPrefix("GetDSLenhDCapPL");
    clearCacheByPrefix("GetDSLenhDCapSoanHang");

    const loc = $("#locPhieuCap").val();
    const tab = getCurrentTab();
    if (loc == "2") {
        if (tab === 0) await GetDaCapXuatHang();
        else await GetItemCode();
    }
}

/// DxDatagrid
//bảng trái đã cấp 
function createViewDxDataGridPhieuCap1(data) {
    dxPhieuCap1 = $(`#${getGridId("dxPhieuCap1")}`).dxDataGrid({
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
            enabled: false,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        selection: {
            mode: 'single',
            showCheckBoxesmode: "onclick",
            allowSelectAll: false,
        },
        columns: getColumnsDaCapByTab(getCurrentTab()),
        summary: {
            groupItems: [
                {
                    column: "SLXuat", summaryType: "sum", showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        if (!e.value) return '';
                        return formatNumber(e.value);

                    }
                },
            ]
        },
        onContentReady: function (e) {
            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === 'data');
                if (firstDataRow) {
                    const rowIndex = firstDataRow.rowIndex;
                    // selectedRowMaLenh = firstDataRow.data.MaLenhSX;

                    const tab = getCurrentTab();

                    if (tab === 0) {
                        itemCodeJoin = firstDataRow.data.MaNPL;
                        selectedRowMaLenh = firstDataRow.data.MaLenhSX;
                        GetChiTietMaLenhDaCapXuatHang();
                    } else {
                        itemCodeJoin = firstDataRow.data.MaNPL;
                        selectedRowMaLenh = firstDataRow.data.MaLenhSX;
                        GetChiTietMaLenhDaCapSoanHang();
                    }
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
            //selectedRowMaLenh = e.data.MaLenhSX;
            const tab = getCurrentTab();
            if (tab === 0) {
                itemCodeJoin = e.data.MaNPL;
                selectedRowMaLenh = e.data.MaLenhSX;
                GetChiTietMaLenhDaCapXuatHang();
            } else {
                itemCodeJoin = e.data.MaNPL;
                selectedRowMaLenh = e.data.MaLenhSX;
                GetChiTietMaLenhDaCapSoanHang();
            }

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
//==== Dữ liệu column của đã cấp by tab =====
function getColumnsDaCapByTab(tab) {
    if (tab === 0) {
        return [
            {
                dataField: "MaNPL",
                caption: "ItemCode",
                calculateCellValue: function (row) {
                    return row.MaVT;
                },
                groupIndex: 0,
                visible: false,
            },
            { dataField: "MaLenh", caption: "Mã Lệnh", width: 90, },
            { dataField: "TenHang", caption: "Tên Hàng", width: 230, },
            {
                dataField: "SLXuat",
                caption: "SL Xuất",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            {
                dataField: "NgayXuatHang",
                caption: "Ngày Xuất",
                cellTemplate: function (container, options) {
                    container.text(options.value || "");
                },
                minWidth: 120
            },
        ];
    } else {
        return [
            {
                dataField: "MaNPL",
                caption: "ItemCode",
                calculateCellValue: function (row) {
                    return row.MaVT;
                },
                groupIndex: 0,
                visible: false,
            },
            { dataField: "MaLenh", caption: "Mã Lệnh", width: 90, },
            { dataField: "MaHang", caption: "Tên Hàng", width: 230, },
            {
                dataField: "SLSoanHang",
                caption: "SL Soạn Hàng",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            {
                dataField: "NgaySoanHang",
                caption: "Ngày Xuất",
                cellTemplate: function (container, options) {
                    container.text(options.value);
                },
                minWidth: 120
            },
        ];
    }
}

//===== Table chi tiết đã cấp =====
function createViewDxDataGridPhieuCap2(data) {
    dxPhieuCap2 = $(`#${getGridId("dxPhieuCap2")}`).dxDataGrid({
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
            enabled: false,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: getColumnsChiTietDaCapByTab(getCurrentTab()),
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
                    column: getCurrentTab() === 0 ? "SLNhap" : "SLSoanHang",
                    summaryType: "sum",
                    showInGroupFooter: true, alignByColumn: true,
                    customizeText: function (e) {
                        if (!e.value) return '';
                        return formatNumber(e.value);

                    }
                },
            ]
        }
    }).dxDataGrid('instance');
}
//Dữ liệu column của chi tiết đã cấp by tab
function getColumnsChiTietDaCapByTab(tab) {
    if (tab === 0) {
        return [
            { dataField: "PhieuXH", groupIndex: 0, caption: "Phiếu Xuất hàng", width: 90, },
            { dataField: "POMua", caption: "PO Mua", width: 100, },
            { dataField: "SoLo", caption: "Số Lô", minWidth: 100, },
            { dataField: "MaVT", caption: "Item Code", width: 90, },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "SoKienHienThi", caption: "Số Roll", minWidth: 100, },
            { dataField: "SLNhap", caption: "Thực Xuất", minWidth: 100, },
            { dataField: "NgayXuatHang", caption: "Ngày Xuất", minWidth: 100, },
        ];
    } else {
        return [
            { dataField: "PhieuXH", groupIndex: 0, caption: "Phiếu Xuất hàng", width: 90, },
            { dataField: "POMua", caption: "PO Mua", width: 100, },
            { dataField: "SoLo", caption: "Số Lô", minWidth: 100, },
            { dataField: "MaVT", caption: "Item Code", width: 90, },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "SoKienHienThi", caption: "Số Roll", minWidth: 100, },
            { dataField: "SLSoanHang", caption: "Thực Xuất", minWidth: 100, },
            { dataField: "NgaySoanHang", caption: "Ngày Xuất", minWidth: 100, },
        ];
    }
}

//===== Table chi tiết chưa cấp =====
function createViewDxDataGridPhieuCap3(data) {
    dxPhieuCap3 = $(`#${getGridId("dxPhieuCap3")}`).dxDataGrid({
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
            enabled: false,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: getColumnsChiTietChuaCapByTab(getCurrentTab()),
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
// ===== Dữ liệu column chi tiết chưa cấp by tab =====
function getColumnsChiTietChuaCapByTab(tab) {
    if (tab === 0) {
        return [
            { dataField: "PhieuDK", caption: "Phiếu Đăng Ký", groupIndex: 0, visible: false },
            { dataField: "NgayCap", caption: "Ngày YC Cấp", },
            { dataField: "MaVT", caption: "Item Code", width: 90, },
            //{
            //    dataField: "CapPhat",
            //    caption: "Cấp Phát",
            //    width: 100,
            //    cellTemplate: function (container, options) {
            //        container.text(formatNumber(options.value))
            //    }
            //},
            {
                dataField: "SLDK",
                caption: "SL Đăng Ký",
                width: 100,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "MaONPL", caption: "Vị Trí Ô", minWidth: 100, },
        ];
    } else {
        return [
            { dataField: "PhieuSH", caption: "Phiếu Soạn Hàng", groupIndex: 0, visible: false },
            { dataField: "NgaySoanHang", caption: "Ngày Soạn Cấp", },
            { dataField: "MaVT", caption: "Item Code", width: 90, },
            //{
            //    dataField: "CapPhat",
            //    caption: "Cấp Phát",
            //    width: 100,
            //    cellTemplate: function (container, options) {
            //        container.text(formatNumber(options.value))
            //    }
            //},
            {
                dataField: "SLSoanHang",
                caption: "SL Soạn Hàng",
                width: 100,
                cellTemplate: function (container, options) {
                    container.text(formatNumber(options.value))
                }
            },
            { dataField: "MauVT", caption: "Màu", minWidth: 100, },
            { dataField: "KhoVai", caption: "Width/size", minWidth: 100, },
            { dataField: "TenDVVT", caption: "Đơn vị", minWidth: 100, },
            { dataField: "MaONPL", caption: "Vị Trí Ô", minWidth: 100, },
        ];
    }
}

// Table chưa cấp 
function createViewDxDataGridPhieuCap4(data) {
    dxPhieuCap4 = $(`#${getGridId("dxPhieuCap4")}`).dxDataGrid({
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
            enabled: false,
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
                    // selectedRowMaLenh = firstDataRow.data.MaLenhSanXuat;
                    const tab = getCurrentTab();
                    if (tab === 0) {
                        selectedRowMaLenh = firstDataRow.data.MaLenhSanXuat;
                        GetChiTietLenhChuaCapXuatHang();
                    } else {
                        selectedRowMaLenh = firstDataRow.data.MaLenhSX;
                        GetChiTietLenhChuaCapSoanHang();
                    }
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
            //selectedRowMaLenh = e.data.MaLenhSanXuat;
            const tab = getCurrentTab();
            if (tab === 0) {
                selectedRowMaLenh = e.data.MaLenhSanXuat;
                GetChiTietLenhChuaCapXuatHang();
            } else {
                selectedRowMaLenh = e.data.MaLenhSX;
                GetChiTietLenhChuaCapSoanHang();
            }

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

// ======== END MODAL ========