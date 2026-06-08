document.getElementById('groupSelect').addEventListener('change', initializeTop5)
document.getElementById('groupSelect').addEventListener('change', initializeTop5min)
document.getElementById('MaHangSelect').addEventListener('change', loadNPLTheoKHMH)
document.getElementById('itemSelect').addEventListener('change', loadSoLo)
document.getElementById('btnLocTonKho').addEventListener('click', loadDSTonKho)
document.getElementById('btnLocTonKho').addEventListener('click', loadDSTonKhoChiTiet)
document.getElementById('MaHangSelect2').addEventListener('change', loadTTDonHang)

document.querySelectorAll('button[data-bs-toggle="tab"]').forEach(function (tabButton) {
    tabButton.addEventListener('shown.bs.tab', function (event) {

        handleActiveTab(currentTTDH);
    });
});
let topChart = null;
let topChart2 = null;
let gridInstance = null;
let gridInstance2 = null;
let gridInstance3 = null;
let gridInstance4 = null;
let gridInstance5 = null;
let gridInstance6 = null;
let currentTTDH = null;
$(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    const today = new Date();
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    const picker1 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        },
        defaultDate: firstDayOfMonth
    });

    const picker2 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker2'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        }
    });
    const picker3 = new tempusDominus.TempusDominus(document.getElementById('datetimepicker3'), {
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                clock: false,
                hours: false,
                minutes: false,
                seconds: false
            }
        },
        localization: {
            format: 'dd/MM/yyyy'
        }
    });
    $(".dateInput").trigger("click")
    picker1.hide()
    picker2.hide()
    picker3.hide();
    $("#dateInput").on("change", function () {
        loadItemVai();
    })
    $("#dateInput2").on("change", function () {
        loadItemVai();
    })
    $(".select_2").select2()
    $('#MaKHSelect').on('change', function () {
        loadMaHang();
        loadNPLTheoKHMH();
    });
    $('#MaHangSelect').on('change', function () {
        loadNPLTheoKHMH();
    });
    $('#MaKHSelect2').on('change', function () {
        loadMaHang2();
        loadTTDonHang();
    });
    $('#itemSelect').on('change', function () {
        const manpl = document.getElementById('itemSelect').value
        //if (manpl == 'all') return;
        loadSoLo();
    });
    $('#MaHangSelect2').on('change', function () {
        loadTTDonHang();
    });
    $('#khachhang').on('change', function () {
        loadMHTonKho();
    });

    $('#mahang').on('change', function () {
        loadItemVai();
    });
    $('#MaKHSelectFilter').on('change', function () {
        loadMHFilter();
        initializeCharts();
        initializeChartsTuoi();
        initializeTop5();
        initializeTop5min();
    });

    $('#MHSelectFilter').on('change', function () {
        initializeCharts();
        initializeChartsTuoi();
        initializeTop5();
        initializeTop5min();
    });

    loadKHTonKho();
    loadKHFilter();
});

function showTab(tabName) {
    // Hide all tab contents
    const tabContents = document.querySelectorAll('.tab-content');
    tabContents.forEach(tab => {
        tab.classList.remove('active');
    });

    // Remove active class from all buttons
    const tabButtons = document.querySelectorAll('.tab-btn');
    tabButtons.forEach(btn => {
        btn.classList.remove('active');
    });

    // Show selected tab
    document.getElementById(tabName).classList.add('active');
    event.target.classList.add('active');
    if (tabName == "progress") {
        $("#myTabContent").addClass("active")
    }
}

// Initialize charts when page loads
window.addEventListener('load', async function () {

    //document.getElementById("loader").style.display = "flex";
    await Promise.all([
        loadDataSoLieu(),
        loadNhom(),
        loadMaKH(),
        loadItemVai(),
        initializeCharts(),
        initializeChartsTuoi(),
        initializeTop5(),
        initializeTop5min()
    ]);
    //document.getElementById("loader").style.display = "none";

});

async function initializeChartsTuoi() {
    const dataFromApi = await loadDataChartTuoi();
    const categoryCtx = document.getElementById('ageChart');
    const nhomChinh = ["FABRIC", "INTERLINING", "ADHESIVE", "PADDING", "MESH"];
    const categories = Array.from({ length: 12 }, (_, i) => `Tháng ${i + 1}`);

    let dataByGroup = {};

    nhomChinh.forEach(nhom => {
        dataByGroup[nhom] = Array.from({ length: 12 }, (_, i) => {
            return dataFromApi
                .filter(x => x.TenNhom === nhom && x.Thang === (i + 1))
                .reduce((sum, item) => sum + item.TonKhoCT, 0);
        });
    });

    dataByGroup["NGUYÊN LIỆU KHÁC"] = Array.from({ length: 12 }, (_, i) => {
        return dataFromApi
            .filter(x => !nhomChinh.includes(x.TenNhom) && x.NPL === true && x.Thang === (i + 1))
            .reduce((sum, item) => sum + item.TonKhoCT, 0);
    });

    dataByGroup["PHỤ LIỆU"] = Array.from({ length: 12 }, (_, i) => {
        return dataFromApi
            .filter(x => x.NPL === false && x.Thang === (i + 1))
            .reduce((sum, item) => sum + item.TonKhoCT, 0);
    });

    Highcharts.chart(categoryCtx, {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Biểu đồ tuổi tồn kho theo tháng'
        },
        xAxis: {
            categories: categories,
            title: { text: 'Tháng' }
        },
        yAxis: {
            min: 0,
            title: { text: 'Tồn kho' }
        },
        tooltip: {
            shared: true,
            pointFormat: '<b>{series.name}</b>: {point.y}<br/>'
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: { enabled: true }
            }
        },
        series: Object.keys(dataByGroup).map(groupName => ({
            name: groupName,
            data: dataByGroup[groupName]
        })),
        credits: {
            enabled: false
        }
    });
}

async function initializeCharts() {
    const dataFromApi = await loadDataChart();

    const categoryCtx = document.getElementById('categoryChart');

    const nhomChinh = ["FABRIC", "INTERLINING", "ADHESIVE", "PADDING", "MESH"];

    let chartData = [];
    let nguyenLieuKhac = 0;
    let phuLieu = 0;

    dataFromApi.forEach(item => {
        if (nhomChinh.includes(item.TenNhom)) {
            console.log("Nhóm chính:", item.TenNhom, item.TonKhoCT);
            chartData.push({ name: item.TenNhom, y: item.TonKhoCT });
        } else {
            if (item.NPL === true) {
                console.log("-> Nhóm phụ, NGUYÊN LIỆU KHÁC:", item.TenNhom, item.TonKhoCT);
                nguyenLieuKhac += item.TonKhoCT;
            } else {
                console.log("-> Nhóm phụ, PHỤ LIỆU:", item.TenNhom, item.TonKhoCT);
                phuLieu += item.TonKhoCT;
            }
        }
    });

    if (nguyenLieuKhac > 0) {
        chartData.push({ name: "NGUYÊN LIỆU KHÁC", y: nguyenLieuKhac });
    }
    if (phuLieu > 0) {
        chartData.push({ name: "PHỤ LIỆU", y: phuLieu });
    }

    Highcharts.chart(categoryCtx, {
        chart: {
            type: 'pie'
        },
        title: {
            text: 'Biểu đồ tỉ lệ tồn kho theo nhóm'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b> ({point.y})'
        },
        accessibility: {
            point: { valueSuffix: '%' }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '{point.name}: {point.percentage:.1f} %',
                    style: {
                        fontWeight: 'normal',
                        fontSize: '12px',
                        color: '#000',
                        textTransform: "math- auto"
                    }
                }
            }
        },
        series: [{
            name: 'Tỷ lệ',
            colorByPoint: true,
            data: chartData
        }],
        credits: {
            enabled: false
        }
    });




}
async function initializeTop5() {
    // Category distribution chart
    const dataFromApi = await loadDataTop5(1);

    const categories = dataFromApi.map(i => i.MaVTGhep)
    const seriesData = dataFromApi.map(i => i.TonKho)
    if (!topChart) {
        topChart = Highcharts.chart('topItemsChartMax', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Top 5 NPL có số lượng tồn kho nhiều nhất'
            },
            xAxis: {
                categories: categories,
                title: { text: 'Mã vật tư' },
                labels: { rotation: -30 }
            },
            yAxis: {
                min: 0,
                title: { text: 'Số lượng tồn kho' }
            },
            tooltip: {
                headerFormat: '<b>{point.key}</b><br/>',
                pointFormat: 'Tồn kho: <b>{point.y}</b>'
            },
            series: [{
                name: 'Biểu độ cột thể hiện top 5 vật tư có số lượng tồn kho nhiều nhất',
                data: seriesData,
                colorByPoint: true
            }],
            plotOptions: {
                column: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            credits: {
                enabled: false
            }
        });
    }
    else {
        topChart.xAxis[0].setCategories(categories)
        topChart.series[0].setData(seriesData)
    }


}
async function initializeTop5min() {
    // Category distribution chart
    const dataFromApi = await loadDataTop5(0);

    const categories = dataFromApi.map(i => i.MaVTGhep)
    const seriesData = dataFromApi.map(i => i.TonKho)
    if (!topChart2) {
        topChart2 = Highcharts.chart('topItemsChartMin', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Top 5 NPL có số lượng tồn kho ít nhất'
            },
            xAxis: {
                categories: categories,
                title: { text: 'Mã vật tư' },
                labels: { rotation: -30 }
            },
            yAxis: {
                min: 0,
                title: { text: 'Số lượng tồn kho' }
            },
            tooltip: {
                headerFormat: '<b>{point.key}</b><br/>',
                pointFormat: 'Tồn kho: <b>{point.y}</b>'
            },
            series: [{
                name: 'Biểu độ cột thể hiện top 5 vật tư có số lượng tồn kho ít nhất',
                data: seriesData,
                colorByPoint: true
            }],
            plotOptions: {
                column: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            credits: {
                enabled: false
            }
        });
    }
    else {
        topChart2.xAxis[0].setCategories(categories)
        topChart2.series[0].setData(seriesData)
    }


}

async function loadDataChartTuoi() {
    const maKH = $("#MaKHSelectFilter").val();
    const maHang = $("#MHSelectFilter").val();
    var url = `/api/TongHopKhoNPL/Get?Action=GETTILETONKHONHOMTHEOTHANG&para1=all&para2=${maKH}&para3=${maHang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        return data;


    } catch (error) {   
        console.error(error.message);
    }
}
async function loadDataChart() {
    const maKH = $("#MaKHSelectFilter").val();
    const maHang = $("#MHSelectFilter").val();
    var url = `/api/TongHopKhoNPL/Get?Action=GETTILENHOMTONKHO&para1=all&para2=${maKH}&para3=${maHang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        return data;


    } catch (error) {
        console.error(error.message);
    }
}
async function loadDataTop5(isNhieuNhat) {
    const manhom = document.getElementById('groupSelect').value || "all";
    const maKH = $("#MaKHSelectFilter").val();
    const maHang = $("#MHSelectFilter").val();
    var url = `/api/TongHopKhoNPL/Get?Action=GETTOP5&para1=${manhom}&para2=${isNhieuNhat}&para3=all&para4=${maKH}&para5=${maHang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        console.log("Data Top5:", data); // Debug
        return data;


    } catch (error) {
        console.error(error.message);
    }
}
async function loadNhom() {
    var url = `/api/TongHopKhoNPL/Get?Action=GETNHOM`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const groupSelect = document.getElementById('groupSelect')
        groupSelect.length = 1;
        const optDefault = document.createElement('option')
        optDefault.value = "all"
        optDefault.textContent = "Tất cả"
        optDefault.selected = true
        groupSelect.appendChild(optDefault)
        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.MaNhom
            opt.textContent = item.TenNhom
            groupSelect.appendChild(opt)
        })


    } catch (error) {
        console.error(error.message);
    }
}
async function loadDataSoLieu() {
    const maKH = $("#MaKHSelectFilter").val();
    const maHang = $("#MHSelectFilter").val();
    var url = `/api/TongHopKhoNPL/Get?Action=GETSOLIEU&para1=all&para2=${maKH}&para3=${maHang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();

        document.getElementById("tongSL").innerText = data[0].TongSL.toLocaleString();
        document.getElementById("tongNPL").innerText = data[0].TongNPL.toLocaleString();
        document.getElementById("tonKho").innerText = data[0].ThanhTien.toLocaleString() + "$";
        document.getElementById("tongYD").innerText = data[0].TongSoYDNL.toLocaleString();
        let tyLe = data[0].TiLeSuDung * 100;
        document.getElementById("tyLe").innerText = tyLe.toFixed(2) + "%";


    } catch (error) {
        console.error(error.message);
    }
}
async function loadMaKH() {
    var url = `/api/TongHopKhoNPL/Get?Action=GetKH`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const groupSelect = document.getElementById('MaKHSelect')
        groupSelect.length = 1;

        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.MaKH
            opt.textContent = item.TenKH
            groupSelect.appendChild(opt)
        })
        const groupSelect2 = document.getElementById('MaKHSelect2')
        groupSelect2.length = 1;

        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.MaKH
            opt.textContent = item.TenKH
            groupSelect2.appendChild(opt)
        })

    } catch (error) {
        console.error(error.message);
    }
}
async function loadMaHang() {
    const makh = document.getElementById('MaKHSelect').value
    var url = `/api/TongHopKhoNPL/Get?Action=GetMaHang&para1=${makh}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const groupSelect = document.getElementById('MaHangSelect')
        groupSelect.length = 1;

        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.MaHang
            opt.textContent = item.TenHang
            groupSelect.appendChild(opt)
        })


    } catch (error) {
        console.error(error.message);
    }
}
async function loadMaHang2() {
    const makh = document.getElementById('MaKHSelect2').value
    var url = `/api/TongHopKhoNPL/Get?Action=GetMaHang&para1=${makh}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const groupSelect = document.getElementById('MaHangSelect2')
        groupSelect.length = 1;

        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.MaHang
            opt.textContent = item.TenHang
            groupSelect.appendChild(opt)
        })


    } catch (error) {
        console.error(error.message);
    }
}

async function loadNPLTheoKHMH() {
    const makh = document.getElementById('MaKHSelect').value
    const mahang = document.getElementById('MaHangSelect').value
    var url = `/api/TongHopKhoNPL/Get?Action=GetTongQuanCapPhat&para1=${makh}&para2=${mahang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) {
            $("#productsGrid").dxDataGrid("dispose").empty();
            return;
        }
        $("#productsGrid").dxDataGrid({
            dataSource: data,
            keyExpr: "MaVatTu",
            showBorders: true,
            paging: { pageSize: 50 },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50, 100],
                showInfo: true
            },
            allowColumnResizing: true,
            columnResizingMode: "widget",
            columnAutoWidth: true,
            wordWrapEnabled: true,
            grouping: { autoExpandAll: true },
            height: "calc(100vh - 215px)",
            groupPanel: { visible: false },
            scrolling: {
                mode: "virtual" // hoặc "standard" nếu muốn scroll toàn bộ
            },
            filterRow: { visible: true },
            showColumnHeaders: true,
            columns: [
                { dataField: "MaVTGhep", caption: "MÃ VẬT TƯ", cssClass: "col-header" },
                { dataField: "MaVatTu", caption: "ITEM CODE", cssClass: "col-header" },

                {
                    dataField: "MaHang",
                    caption: "Mã Hàng",
                    groupIndex: 0,
                    cssClass: "col-header",
                    visible: false,
                    groupCellTemplate: function (container, options) {
                        container.text(options.value);
                    }
                },
                {
                    dataField: "NPL",
                    caption: "Loại NPL",
                    groupIndex: 1,
                    visible: false,
                    cssClass: "col-header",
                    sortOrder: "desc",
                    groupCellTemplate: function (container, options) {
                        // options.value = 0 (Nguyên Liệu), =1 (Phụ Liệu)
                        if (options.value == true) {
                            container.text("Nguyên Liệu");
                        } else {
                            container.text("Phụ Liệu");
                        }
                    }
                },
                {
                    dataField: "TenNhom",
                    caption: "Tên Nhóm",
                    groupIndex: 2,
                    visible: false,
                    cssClass: "col-header",
                    calculateGroupValue: function (rowData) {
                        return rowData.Sort;      // sort theo cột Sort
                    },
                    groupCellTemplate: function (container, options) {
                        container.text(options.data.items[0].TenNhom); // hiển thị Tên Nhóm
                    }
                },

                { cssClass: "col-header", dataField: "TenDV", caption: "ĐVT" },
                { cssClass: "col-header", dataField: "KhoVai", caption: "KHỔ/SIZE" },
                { cssClass: "col-header", dataField: "MauVTCode", caption: "MÃ MÀU VT" },
                { cssClass: "col-header", dataField: "MauVT", caption: "MÀU VT" },
                { cssClass: "col-header", dataField: "CapPhat", caption: "NHU CẦU", dataType: "number", format: { type: "fixedPoint", precision: 2 } },
                { cssClass: "col-header", dataField: "ThucNhap", caption: "NHẬP VỀ", dataType: "number", format: { type: "fixedPoint", precision: 2 } },
                { cssClass: "col-header", dataField: "SLXuat", caption: "XUẤT ĐI", dataType: "number", format: { type: "fixedPoint", precision: 2 } },
                { cssClass: "col-header", dataField: "SLThua", caption: "SL THỪA", dataType: "number", format: { type: "fixedPoint", precision: 2 } },
                { cssClass: "col-header", dataField: "SLThieu", caption: "SL THIẾU", dataType: "number", format: { type: "fixedPoint", precision: 2 } }
            ],
            onCellPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.column.dataField === "SLThieu" && e.value !== 0) {
                        e.cellElement.css("color", "red");
                    }
                    if (e.column.dataField === "SLThua" && e.value !== 0) {
                        e.cellElement.css("color", "green");
                    }
                }
            }
        });
        $("#Layer_1").click();

    } catch (error) {
        console.error(error.message);
    }
}

// cái tab tiến độ 

async function loadTTDonHang() {
    const makh = document.getElementById('MaKHSelect2').value
    const mahang = document.getElementById('MaHangSelect2').value
    var url = `/api/TongHopKhoNPL/Get?Action=GetDH&&para1=${makh}&&para2=${mahang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) {
            $("#gridTTDH").dxDataGrid("dispose").empty();
            return;
        }
        $("#gridTTDH").dxDataGrid({
            dataSource: data,
            keyExpr: "ID",
            showBorders: true,
            columnAutoWidth: false,
            columnResizingMode: "widget",
            height: "300px",
            scrolling: {
                mode: "virtual"
            },
            paging: {
                pageSize: 10
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50],
                showInfo: true
            },
            searchPanel: {
                visible: false
            },
            filterRow: {
                visible: true
            },
            grouping: {
                autoExpandAll: true
            },
            groupPanel: {
                visible: false
            },
            focusedRowEnabled: true,     // PHẢI CÓ
            focusedRowIndex: 0,

            onFocusedRowChanged: function (e) {
                if (e.row && e.row.data) {
                    const d = e.row.data
                    currentTTDH = {
                        MaDH: d.MaDH,
                        MaHang: d.MaHang,
                        POID: d.POID ?? d.POId ?? d.PO_ID ?? null,
                        MaMau: d.MaMau ?? d.ColorCode ?? null,
                        DauSizeID: d.DauSizeID ?? d.DauSizeId ?? null
                    }
                    onChangeTTDH(currentTTDH)

                } else if (e.rowKey) {
                    e.component.byKey(e.rowKey).done(function (d) {
                        if (!d) return
                        currentTTDH = {
                            MaDH: d.MaDH,
                            MaHang: d.MaHang,
                            POID: d.POID ?? d.POId ?? d.PO_ID ?? null,
                            MaMau: d.MaMau ?? d.ColorCode ?? null,
                            DauSizeID: d.DauSizeID ?? d.DauSizeId ?? null
                        }
                        onChangeTTDH(currentTTDH)
                    })
                }
            },
            //columns: [
            //    /*   { dataField: "MaHang", caption: "Mã Hàng", cssClass: "col-header" },*/
            //    {
            //        dataField: "TenHang",
            //        caption: "Chi tiết",
            //        allowEditing: false,
            //        groupIndex: 0,
            //        groupCellTemplate: function (cellElement, cellInfo) {
            //            const data = cellInfo.data.collapsedItems;
            //            if (data) {
            //                cellElement.html(`<span style="color: brown; font-weight: bold;"> Mã hàng : ${data[0].TenHang}</span>`);
            //            } else if (cellInfo.data.items) {
            //                cellElement.html(`<span style="color: brown; font-weight: bold;"> Mã hàng : ${cellInfo.data.items[0].TenHang}| Số lượng: </span>`);
            //            }

            //        },
            //        cssClass: "col-header"
            //    },
            //    { dataField: "PO", caption: "PO", cssClass: "col-header" },
            //    { dataField: "BookingMaHang", caption: "Số Booking", cssClass: "col-header" },
            //    { dataField: "MaKH", caption: "Mã KH", cssClass: "col-header",visible:false },
            //    { dataField: "MaDH", caption: "Đơn hàng", cssClass: "col-header" },
            //    { dataField: "ColorCode", caption: "Code Màu", cssClass: "col-header" },
            //    { dataField: "TenMau", caption: "Tên Màu", cssClass: "col-header", visible: false  },
            //    { dataField: "DauSize", caption: "Inseam", cssClass: "col-header" },
            //    { dataField: "SoLuong", caption: "Số Lượng", cssClass: "col-header", dataType: "number", format: "#,##0" },
            //    { dataField: "NgayTao", caption: "Ngày Tạo", cssClass: "col-header", dataType: "date", format: "dd/MM/yyyy", visible: false }
            //]
            columns: [
                {
                    dataField: "TenHang",
                    caption: "Mã hàng",
                    allowEditing: false,
                    groupIndex: 0,
                    //groupCellTemplate: function (cellElement, cellInfo) {
                    //    const data = cellInfo.data.collapsedItems;
                    //    if (data) {
                    //        cellElement.html(`<span style="color: brown; font-weight: bold;"> Mã hàng : ${data[0].TenHang}</span>`);
                    //    } else if (cellInfo.data.items) {
                    //        cellElement.html(`<span style="color: brown; font-weight: bold;"> Mã hàng : ${cellInfo.data.items[0].TenHang}</span>`);
                    //    }
                    //},
                    cssClass: "col-header"
                },
                { dataField: "PO", caption: "PO", groupIndex: 1, cssClass: "col-header" },
                { dataField: "BookingMaHang", caption: "Số Booking", cssClass: "col-header" },
                { dataField: "MaKH", caption: "Mã KH", cssClass: "col-header", visible: false },
                { dataField: "MaDH", caption: "Đơn hàng", cssClass: "col-header" },
                { dataField: "ColorCode", caption: "Code Màu", cssClass: "col-header" },
                { dataField: "TenMau", caption: "Tên Màu", cssClass: "col-header", visible: false },
                { dataField: "DauSize", caption: "Inseam", cssClass: "col-header" },
                { dataField: "SoLuong", caption: "Số Lượng", cssClass: "col-header", dataType: "number", format: "#,##0" },
                { dataField: "NgayTao", caption: "Ngày Tạo", cssClass: "col-header", dataType: "date", format: "dd/MM/yyyy", visible: false }
            ],

            summary: {
                groupItems: [
                    {
                        column: "SoLuong",
                        summaryType: "sum",
                        showInGroupFooter: false,
                        alignByColumn: true,
                        displayFormat: "SL:{0}"
                    }
                ]
            }
        });
        const grid = $("#gridTTDH").dxDataGrid("instance");
        grid.getDataSource().load().done(() => {
            if (grid.getDataSource().items().length > 0) {
                //grid.option("focusedRowIndex", 0);
                grid.option("focusedRowIndex", 2);
                //grid.option("focusedRowIndex", 0);
            }
        });
        $("#Layer_1").click();

    } catch (error) {
        console.error(error.message);
    }
}
function onChangeTTDH(payload) {

    var url = `/api/TongHopKhoNPL/Get?Action=GetPO_ThucHien&&para1=${payload.MaDH}&&para2=${payload.MaHang}&&para3=${payload.POID}&&para4=${payload.MaMau}&&para5=${payload.DauSizeID}`;
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.length == 0) {
                $("#gridTTTH").dxDataGrid("dispose").empty();
                return;
            }
            //$("#gridTTTH").dxDataGrid({
            //    dataSource: data,
            //    keyExpr: "MaLenhSanXuat",
            //    showBorders: true,
            //    columnAutoWidth: false,
            //    columnResizingMode: "widget",
            //    scrolling: { mode: "virtual" },
            //    height: "300px",
            //    paging: { pageSize: 10 },
            //    pager: { showPageSizeSelector: true, allowedPageSizes: [10, 20, 50], showInfo: true },
            //    filterRow: { visible: true },
            //    searchPanel: { visible: true },
            //    grouping: { autoExpandAll: false },
            //    groupPanel: { visible: false },
            //    searchPanel: {
            //        visible: false
            //    },
            //    columns: [
            //        { dataField: "MaLenh", caption: "Mã Lệnh", cssClass: "col-header" },
            //        { dataField: "BookingMaHang", caption: "Booking", cssClass: "col-header", visible: "false"  },
            //        { dataField: "ColorCode", caption: "Code Màu", cssClass: "col-header", visible: "true"  },
            //        { dataField: "TenMau", caption: "Màu", cssClass: "col-header" },
            //        { dataField: "DauSize", caption: "Inseam", cssClass: "col-header" },
            //        { dataField: "DauSizeID", caption: "Đầu Size ID", cssClass: "col-header", visible: "false"  },
            //        { dataField: "MaDH", caption: "Mã ĐH", cssClass: "col-header", visible: "false"  },
            //        { dataField: "MaHang", caption: "Mã Hàng", cssClass: "col-header", visible: "false"  },
            //        { dataField: "TenHang", caption: "Mã Hàng", cssClass: "col-header" },
            //        { dataField: "PO", caption: "PO", cssClass: "col-header" },
            //        { dataField: "POID", caption: "POID", cssClass: "col-header", visible: "false" },
            //        { dataField: "SL_KH_PO", caption: "SLKH PO", dataType: "number", format: "#,##0", cssClass: "col-header" },
            //        { dataField: "SL_BTP", caption: "SL BTP", dataType: "number", format: "#,##0", cssClass: "col-header" },
            //        { dataField: "SL_RC", caption: "SL RC", dataType: "number", format: "#,##0", cssClass: "col-header" },
            //        { dataField: "SL_NhapKho", caption: "SL Nhập Kho", dataType: "number", format: "#,##0", cssClass: "col-header" },
            //        { dataField: "SL_DongThung", caption: "SL Đóng Thùng", dataType: "number", format: "#,##0", cssClass: "col-header" },
            //        { dataField: "MaDVSX", caption: "Mã ĐVSX", cssClass: "col-header", visible: "false"  },
            //        { dataField: "TenDVSX", caption: "Tên ĐVSX", cssClass: "col-header" },

            //        { dataField: "MaLenhSanXuat", caption: "Mã Lệnh SX", cssClass: "col-header", visible: "false"  },
            //        { dataField: "MaGop", caption: "Mã Gộp", cssClass: "col-header", visible:"false" }
            //    ]
            //})
            $("#gridTTTH").dxDataGrid({
                dataSource: data,
                keyExpr: "MaLenhSanXuat",
                showBorders: true,
                columnAutoWidth: true,
                columnResizingMode: "widget",
                scrolling: { mode: "virtual" },
                height: "300px",
                paging: { pageSize: 20 },
                pager: { showPageSizeSelector: true, allowedPageSizes: [10, 20, 50], showInfo: true },
                filterRow: { visible: false },
                searchPanel: { visible: false },
                grouping: { autoExpandAll: true },
                groupPanel: { visible: false },

                columns: [
                    { dataField: "TenDVSX", caption: "Khu", groupIndex: 0 },
                    { dataField: "TenHang", caption: "Mã Hàng", groupIndex: 1 },
                    { dataField: "TenMau", caption: "Màu", groupIndex: 2 },

                    { dataField: "MaLenh", caption: "Mã Lệnh", cssClass: "col-header" },
                    { dataField: "PO", caption: "PO", cssClass: "col-header" },
                    { dataField: "ColorCode", caption: "Code Màu", cssClass: "col-header" },
                    { dataField: "DauSize", caption: "Inseam", cssClass: "col-header" },

                    { dataField: "SL_KH_PO", caption: "SLKH (SX)", dataType: "number", format: "#,##0", cssClass: "col-header" },
                    { dataField: "SL_BTP", caption: "Cắt", dataType: "number", format: "#,##0", cssClass: "col-header" },
                    { dataField: "SL_RC", caption: "May", dataType: "number", format: "#,##0", cssClass: "col-header" },
                    { dataField: "SL_DongThung", caption: "Đóng Thùng", dataType: "number", format: "#,##0", cssClass: "col-header" },
                    { dataField: "SL_NhapKho", caption: "Nhập Kho", dataType: "number", format: "#,##0", cssClass: "col-header" }
                ],

                summary: {
                    totalItems: [
                        { column: "SL_KH_PO", summaryType: "sum", valueFormat: "#,##0", displayFormat: "{0}" },
                        { column: "SL_BTP", summaryType: "sum", valueFormat: "#,##0", displayFormat: "{0}" },
                        { column: "SL_RC", summaryType: "sum", valueFormat: "#,##0", displayFormat: "{0}" },
                        { column: "SL_DongThung", summaryType: "sum", valueFormat: "#,##0", displayFormat: "{0}" },
                        { column: "SL_NhapKho", summaryType: "sum", valueFormat: "#,##0", displayFormat: "{0}" }
                    ]
                }
            })
            handleActiveTab(payload);
            $("#Layer_1").click();
        })
        .catch(error => {
            console.error(error.message);
        });

    handleActiveTab(payload);
}
function handleActiveTab(payload) {
    var activeTab = $("#myTabContent .tab-pane.active").attr("id");

    switch (activeTab) {
        case "tab1":
            loadgCCT(payload);
            break;
        case "tab2":
            loadgCKHSX(payload);
            break;
        case "tab3":
            loadgCCat(payload);
            break;
        case "tab4":
            loadgCMay(payload);
            break;
        case "tab5":
            loadgCDongThung(payload);
            break;
        case "tab6":
            loadgCNhapKho(payload);
            break;
    }
}
// Global variables for grid instances
let gridInstances = {};

// Common function to add TongSL column
function addColumnSumRow(data, sizeKeyPattern) {
    return data.map(row => {
        let tong = 0;
        Object.keys(row).forEach(key => {
            if (key.includes(sizeKeyPattern)) {
                tong += parseInt(row[key]) || 0;
            }
        });
        return { ...row, TongSL: tong };
    });
}

// Common function to create or update banded grid
function createBandedGrid(gridId, data, columns, sizeColumns, sizeKeyPattern, extraOptions = {}) {
    const instanceKey = gridId.replace('#', '');
    if (gridInstances[instanceKey]) {
        gridInstances[instanceKey].option("dataSource", data);
        return;
    }

    const fullColumns = [
        ...columns,
        { caption: 'Size', alignment: 'center', columns: sizeColumns, cssClass: "col-header" },
        {
            dataField: 'TongSL',
            caption: 'Tổng SL',
            alignment: 'center',
            width: 100,
            fixed: true,
            fixedPosition: 'right',
            dataType: 'number',
            format: { type: 'fixedPoint', precision: 0 },
            cssClass: "col-header"
        }
    ];

    const summaryItems = [
        ...sizeColumns.map(col => ({
            column: col.dataField,
            summaryType: 'sum',
            displayFormat: '{0}',
            alignByColumn: true
        })),
        {
            column: 'TongSL',
            summaryType: 'sum',
            displayFormat: '{0}',
            alignByColumn: true
        }
    ];

    gridInstances[instanceKey] = $(gridId).dxDataGrid({
        dataSource: data,
        columns: fullColumns,
        showBorders: true,
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnResizingMode: 'widget',
        editing: { mode: 'cell', allowUpdating: true },
        summary: { totalItems: summaryItems },
        groupPanel: { visible: false },
        filterRow: { visible: true },
        onCellPrepared: e => {
            if (e.rowType === 'header' && e.column.cssClass === "size-column") {
                e.cellElement.css({ 'background': '#5da566', 'color': 'white', 'font-weight': 'bold', 'text-align': 'center' });
            } else if (e.rowType === 'header' && e.column.caption === "Size") {
                e.cellElement.css({ 'background': '#5da566', 'color': 'black', 'font-weight': 'bold', 'text-align': 'center' });
            }
        },
        ...extraOptions
    }).dxDataGrid('instance');
}

// Helper to generate size columns
function generateSizeColumns(data, sizeKeyPattern, captionTransform) {
    const sizeColumns = [];
    if (data && data.length > 0) {
        Object.keys(data[0]).forEach(key => {
            if (key.includes(sizeKeyPattern)) {
                const parts = key.split('@');
                const caption = captionTransform(parts);
                sizeColumns.push({
                    dataField: key,
                    caption: caption,
                    alignment: 'center',
                    width: 80,
                    allowEditing: false,
                    dataType: 'number',
                    format: { type: 'fixedPoint', precision: 0 },
                    cssClass: "size-column"
                });
            }
        });
    }
    return sizeColumns;
}

// Common load function
function loadGrid(payload, action, gridId, columns, sizeKeyPattern, captionTransform, extraOptions = {}) {
    const url = `/api/TongHopKhoNPL/Get?Action=${action}&&para1=${payload.MaDH}&&para2=${payload.MaHang}&&para3=${payload.POID}&&para4=${payload.MaMau}&&para5=${payload.DauSizeID}`;
    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error(`Response status: ${response.status}`);
            return response.json();
        })
        .then(data => {
            if (data.length === 0) {
                $(gridId).dxDataGrid("dispose").empty();
                return;
            }
            const processedData = addColumnSumRow(data, sizeKeyPattern);
            const sizeColumns = generateSizeColumns(processedData, sizeKeyPattern, captionTransform);
            createBandedGrid(gridId, processedData, columns, sizeColumns, sizeKeyPattern, extraOptions);
            $("#Layer_1").click();
        })
        .catch(error => console.error(error.message));
}

// Specific configurations
function loadgCCT(payload) {
    const columns = [
        { dataField: 'PO', caption: 'PO', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenMau', caption: 'Màu', allowEditing: false, cssClass: "col-header" },
        { dataField: 'DauSize', caption: 'Inseam', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenQG', caption: 'Quốc gia', allowEditing: false, cssClass: "col-header" },
        { dataField: 'NgayGH', caption: 'Ngày Giao', allowEditing: false, dataType: 'date', format: 'dd/MM/yyyy', cssClass: "col-header" }
    ];
    loadGrid(payload, 'GETCHITIETDH', '#gCCT', columns, 'Size@', parts => parts.pop());
}

function loadgCKHSX(payload) {
    const columns = [
        { dataField: 'MaLenh', caption: 'Mã Lệnh', allowEditing: false, cssClass: "col-header" },
        { dataField: 'PO', caption: 'PO', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenMau', caption: 'Màu', allowEditing: false, cssClass: "col-header" },
        { dataField: 'SizeType', caption: 'Inseam', allowEditing: false, cssClass: "col-header" }
    ];
    loadGrid(payload, 'GetgCKHSX', '#gCKHSX', columns, 'Size@', parts => parts.pop());
}

function loadgCCat(payload) {
    const columns = [
        { dataField: 'MaLenh', caption: 'Mã Lệnh', allowEditing: false, cssClass: "col-header" },
        { dataField: 'PO', caption: 'PO', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenMau', caption: 'Màu', allowEditing: false, cssClass: "col-header" },
        { dataField: 'Code_TNC', caption: 'Code TNC', allowEditing: false, cssClass: "col-header" },
        { dataField: 'SizeType', caption: 'Inseam', allowEditing: false, cssClass: "col-header" }
    ];
    loadGrid(payload, 'GetgCCat', '#gCCat', columns, 'Size@', parts => parts.pop());
}

function loadgCMay(payload) {
    const columns = [
        { dataField: 'MaLenh', caption: 'Mã Lệnh', allowEditing: false, cssClass: "col-header" },
        { dataField: 'PO', caption: 'PO', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenMau', caption: 'Màu', allowEditing: false, cssClass: "col-header" },
        { dataField: 'SizeType', caption: 'Inseam', allowEditing: false, cssClass: "col-header" }
    ];
    loadGrid(payload, 'GetgCMay', '#gCMay', columns, 'Size@', parts => parts.pop());
}

function loadgCDongThung(payload) {
    const columns = [
        { dataField: 'PO', caption: 'PO', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenMau', caption: 'Màu', allowEditing: false, cssClass: "col-header" },
        { dataField: 'SizeType', caption: 'Inseam', allowEditing: false, cssClass: "col-header" }
    ];
    loadGrid(payload, 'GetgCDongThung', '#gCDongThung', columns, 'Size@', parts => parts.pop());
}

function loadgCNhapKho(payload) {
    const columns = [
        { dataField: 'PO', caption: 'PO', allowEditing: false, cssClass: "col-header" },
        { dataField: 'TenMau', caption: 'Màu', allowEditing: false, cssClass: "col-header" },
        { dataField: 'SizeType', caption: 'Inseam', allowEditing: false, cssClass: "col-header" }
    ];
    loadGrid(payload, 'GetgCNhapKho', '#gCNhapKho', columns, 'Size@', parts => parts.pop());
}


document.querySelectorAll('#progress .tab-header li').forEach(li => {
    li.addEventListener('click', () => {
        document.querySelectorAll('#progress .tab-header li').forEach(e => e.classList.remove('active'));
        document.querySelectorAll('#progress .tab-content-inner').forEach(e => e.classList.remove('active'));
        li.classList.add('active');
        document.getElementById(li.dataset.tab).classList.add('active');
    });
});


//tab tồn kho 
async function loadItemVai() {
    const makh = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    const mahang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();
    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#dateInput").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#dateInput2").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    var url = `/api/TongHopKhoNPL/Get?Action=GetVatTu&para2=${makh}&para3=${mahang}&para4=${tuNgayFormat}&para5=${denNgayFormat}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const groupSelect = document.getElementById('itemSelect')
        groupSelect.length = 1;

        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.MaNPL
            opt.textContent = item.Display
            groupSelect.appendChild(opt)
        })
        if (groupSelect.options.length > 1) {
            groupSelect.selectedIndex = 1
        }
        loadSoLo();
    } catch (error) {
        console.error(error.message);
    }
}
async function loadSoLo() {
    const tuNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#dateInput").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const denNgayFormat = $("#filterType").val() != "khachhang"
        ? moment($("#dateInput2").val(), "DD/MM/YYYY").format("YYYY-MM-DD")
        : "1990-01-01";
    const maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    const maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();
    var url = `/api/TongHopKhoNPL/Get?Action=GetSoLoMK&para1=${maKH}&para2=${maHang}&para3=${tuNgayFormat}&para4=${denNgayFormat}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const groupSelect = document.getElementById('SoLoSelect')
        groupSelect.length = 1;
        if (data.length == 0) return;

        data.forEach(item => {
            const opt = document.createElement('option')
            opt.value = item.SoLoID
            opt.textContent = item.SoLo
            groupSelect.appendChild(opt)
        })

    } catch (error) {
        console.error(error.message);
    }
}

async function loadDSTonKho() {
    const itemvai = $("#itemSelect").val() == "" ? "all" : $("#itemSelect").val();
    const soloid = $("#SoLoSelect").val();
    var maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    var maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();
    const fromDate = $("#filterType").val() != "khachhang" ? moment($("#dateInput").val(), "DD/MM/YYYY").format("YYYY-MM-DD") : "1900-01-01";
    const toDate = $("#filterType").val() != "khachhang" ? moment($("#dateInput2").val(), "DD/MM/YYYY").format("YYYY-MM-DD"):"2900-01-01";
    var url = `/api/TongHopKhoNPL/Get?action=GetTKTong&para1=${fromDate}&para2=${toDate}&para3=${itemvai}&para4=${soloid}&para5=${maHang}&para6=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        console.log(data);

        $("#summaryTonKho").dxDataGrid({
            dataSource: data,
            keyExpr: "MaVT",
            showBorders: true,
            columnAutoWidth: true,

            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            paging: {
                pageSize: 20
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50, 100]
            },
            columns: [
                { cssClass: "col-header", dataField: "MaNPL", caption: "MaNPL", visible: false },
                { dataField: "MaVT", caption: "ITEMCODE", cssClass: "col-header" },
                { dataField: "ChiTiet", caption: "Vật tư", cssClass: "col-header" },
                { dataField: "KhoVai", caption: "Khổ/Size", cssClass: "col-header" },
                { dataField: "MauVT", caption: "Màu VT", cssClass: "col-header" },
                { dataField: "TenDVVT", caption: "Đơn Vị", cssClass: "col-header" },
                { dataField: "TonKhoDK", caption: "Tồn ĐK", cssClass: "col-header" },
                { dataField: "NKDK", caption: "Nhập ĐK", cssClass: "col-header" },
                { dataField: "SLNK", caption: "SL Nhập", cssClass: "col-header" },
                { dataField: "SLXH", caption: "SL Xuất", cssClass: "col-header" },
                { dataField: "SLTH", caption: "SL Thừa", cssClass: "col-header" },
                { dataField: "THDK", caption: "Thừa ĐK", cssClass: "col-header" },
                { dataField: "XHDK", caption: "Xuất ĐK", cssClass: "col-header" },
                { dataField: "TonKho", caption: "Tồn Kho", cssClass: "col-header", format: { type: "fixedPoint", precision: 0 } }
            ],
            summary: {
                totalItems: [
                    { column: "TonKho", summaryType: "sum", displayFormat: " {0}" },
                    { column: "SLNK", summaryType: "sum", displayFormat: "{0}" },
                    { column: "SLXH", summaryType: "sum", displayFormat: "{0}" },

                    { column: "SLTH", summaryType: "sum", displayFormat: " {0}" },
                    { column: "THDK", summaryType: "sum", displayFormat: "{0}" },
                    { column: "XHDK", summaryType: "sum", displayFormat: "{0}" }
                ]
            },
            onRowClick: function (e) {
                const manpl = e.data.MaNPL;
                if (manpl) {
                    loadDSTonKhoChiTietFocus(manpl);
                }
            },
            onContentReady: function (e) {

                if (!e.component.__focusedFirstRow) {
                    let firstRow = e.component.getVisibleRows()[0];
                    if (firstRow) {
                        e.component.selectRows([firstRow.key], true);
                        e.component.option("focusedRowIndex", 0);
                        const manpl = firstRow.data.MaNPL || null;
                        if (manpl) {
                            loadDSTonKhoChiTietFocus(manpl);
                        }
                        e.component.__focusedFirstRow = true;
                    }
                }
            }
        });
        $("#Layer_1").click();

    } catch (error) {
        console.error(error.message);
    }
}
async function loadDSTonKhoChiTiet() {
    const fromDate = $("#filterType").val() != "khachhang" ? moment($("#dateInput").val(), "DD/MM/YYYY").format("YYYY-MM-DD") : "1900-01-01";
    const toDate = $("#filterType").val() != "khachhang" ? moment($("#dateInput2").val(), "DD/MM/YYYY").format("YYYY-MM-DD") : "2900-01-01";
    var maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    var maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();
    const itemvai = $("#itemSelect").val() == "" ? "all" : $("#itemSelect").val();
    const soloid = $("#SoLoSelect").val();
    var url = `/api/TongHopKhoNPL/Get?Action=GetTheKho&&para1=${fromDate}&para2=${toDate}&para3=${itemvai}&para4=${soloid}&para5=${maHang}&para6=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        console.log(data);
        if (data.length == 0) return;
        $("#detailTonKho").dxDataGrid({
            dataSource: data,
            keyExpr: "MaVT",
            showBorders: true,
            columnAutoWidth: false,
            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            columns: [

                { cssClass: "col-header", dataField: "NgayChungTu", caption: "Ngày chứng từ" },
                { cssClass: "col-header", dataField: "SoChungTu", caption: "Số chứng từ" },
                { cssClass: "col-header", dataField: "TrichYeu", caption: "Trích yếu" },
                { cssClass: "col-header", dataField: "NgayXH", caption: "Ngày nhập xuất" },
                { cssClass: "col-header", dataField: "MaPhieu", caption: "Phiếu" },
                { cssClass: "col-header", dataField: "ChiTiet", caption: "Chi tiết" },
                { cssClass: "col-header", dataField: "MaVT", caption: "Item Code" },
                { cssClass: "col-header", dataField: "MauVT", caption: "Màu VT" },
                { cssClass: "col-header", dataField: "KhoVai", caption: "Khổ/Size" },
                { cssClass: "col-header", dataField: "TenDVVT", caption: "Đơn vị" },
                { cssClass: "col-header", dataField: "SLNK", caption: "SL nhập kho", dataType: "number" },
                { cssClass: "col-header", dataField: "SLXH", caption: "SL đã xuất", dataType: "number" },
                { cssClass: "col-header", dataField: "SLTH", caption: "SL thu hồi", dataType: "number" },
                { cssClass: "col-header", dataField: "TonKho", caption: "Tồn kho", dataType: "number" }
            ],
            paging: {
                pageSize: 10
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [5, 10, 20],
                showInfo: true
            }

        });
        $("#Layer_1").click();

    } catch (error) {
        console.error(error.message);
    }
}
async function loadDSTonKhoChiTietFocus(manpl) {
    const fromDate = $("#filterType").val() != "khachhang" ? moment($("#dateInput").val(), "DD/MM/YYYY").format("YYYY-MM-DD") : "1900-01-01";
    const toDate = $("#filterType").val() != "khachhang" ? moment($("#dateInput2").val(), "DD/MM/YYYY").format("YYYY-MM-DD") : "2900-01-01";
    const maKH = $("#filterType").val() != "khachhang" ? "all" : $("#khachhang").val();
    const maHang = $("#filterType").val() != "khachhang" ? "all" : $("#mahang").val();
    const soloid = $("#SoLoSelect").val(); 
    try {
        var url = `/api/TongHopKhoNPL/Get?Action=GetTheKho&para1=${fromDate}&para2=${toDate}&para3=${manpl}&para4=${soloid}&para5=${maHang}&para6=${maKH}`;
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        if (data.length == 0) return;
        $("#detailTonKho").dxDataGrid({
            dataSource: data,
            keyExpr: "MaVT",
            showBorders: true,
            columnAutoWidth: false,
            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            columns: [
                { cssClass: "col-header", dataField: "NgayChungTu", caption: "Ngày chứng từ" },
                { cssClass: "col-header", dataField: "SoChungTu", caption: "Số chứng từ" },
                { cssClass: "col-header", dataField: "TrichYeu", caption: "Trích yếu" },
                { cssClass: "col-header", dataField: "NgayXH", caption: "Ngày nhập xuất" },
                { cssClass: "col-header", dataField: "MaPhieu", caption: "Phiếu" },
                { cssClass: "col-header", dataField: "ChiTiet", caption: "Chi tiết" },
                { cssClass: "col-header", dataField: "MaVT", caption: "Item Code" },
                { cssClass: "col-header", dataField: "MauVT", caption: "Màu VT" },
                { cssClass: "col-header", dataField: "KhoVai", caption: "Khổ/Size" },
                { cssClass: "col-header", dataField: "TenDVVT", caption: "Đơn vị" },
                { cssClass: "col-header", dataField: "SLNK", caption: "SL nhập kho", dataType: "number" },
                { cssClass: "col-header", dataField: "SLXH", caption: "SL đã xuất", dataType: "number" },
                { cssClass: "col-header", dataField: "SLTH", caption: "SL thu hồi", dataType: "number" },
                { cssClass: "col-header", dataField: "TonKho", caption: "Tồn kho", dataType: "number" }
            ],
            paging: {
                pageSize: 10
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [5, 10, 20],
                showInfo: true
            }
        });
        $("#Layer_1").click();

    } catch (error) {
        console.error(error.message);
    }
}
//lấy value từ lưới
function getRowFocusedValue() {
    const grid = $("#gridTTDH").dxDataGrid("instance");
    const key = grid.option("focusedRowKey");
    if (!key) return Promise.resolve(null);

    return grid.byKey(key).then(function (d) {
        return {
            MaDH: d.MaDH,
            MaHang: d.MaHang,
            POID: d.POID ?? d.POId ?? d.PO_ID ?? null,
            MaMau: d.MaMau ?? d.ColorCode ?? null,
            DauSizeID: d.DauSizeID ?? d.DauSizeId ?? null
        };
    });
}

function ChangeFilter() {
    const val = $("#filterType").val();
    if (val === "khachhang") {
        $(".filterKH").show();
        $(".filterDate").hide();

    }
    else {
        $(".filterDate").show();
        $(".filterKH").hide();
    }
    loadItemVai();
}


async function loadKHTonKho() {
    const url = `/api/TongHopKhoNPL/Get?Action=GETKHTK`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        const $khachHangSelect = $("#khachhang");
        if ($khachHangSelect.data('select2')) { $khachHangSelect.select2('destroy'); }
        $khachHangSelect.empty();

        $khachHangSelect.append(`<option value="all">Tất cả</option>`);

        if (data.length > 0) {
            let html = '';
            data.map(x => {
                html += `<option value="${x.MaKH}">${x.TenKH}</option>`;
            });
            $khachHangSelect.append(html);
        }
        $khachHangSelect.select2();
        loadMHTonKho();
    } catch (error) {
        console.error("Lỗi khi tải danh sách khách hàng:", error.message);
    }
}
async function loadMHTonKho() {
    const maKH = $("#khachhang").val();
    const url = `/api/TongHopKhoNPL/Get?Action=GETMHTK&para2=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const $maHangSelect = $("#mahang");
        if ($maHangSelect.data('select2')) { $maHangSelect.select2('destroy'); }
        $maHangSelect.empty();
        $maHangSelect.append(`<option value="all">Tất cả</option>`);
        if (data.length > 0) {
            let html = '';
            data.map(x => {
                html += `<option value="${x.MaHang}">${x.TenHang}</option>`;
            });
            $maHangSelect.append(html);
        }
        $maHangSelect.select2();
        loadItemVai();
    } catch (error) {
        console.error("Lỗi khi tải danh sách mã hàng:", error.message);
    }
}

async function loadKHFilter() {
    const url = `/api/TongHopKhoNPL/Get?Action=GETKHTK`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        const $khachHangSelect = $("#MaKHSelectFilter");
        if ($khachHangSelect.data('select2')) { $khachHangSelect.select2('destroy'); }
        $khachHangSelect.empty();

        $khachHangSelect.append(`<option value="all">Tất cả</option>`);

        if (data.length > 0) {
            let html = '';
            data.map(x => {
                html += `<option value="${x.MaKH}">${x.TenKH}</option>`;
            });
            $khachHangSelect.append(html);
        }
        $khachHangSelect.select2();
        loadMHFilter();
    } catch (error) {
        console.error("Lỗi khi tải danh sách khách hàng:", error.message);
    }
}
async function loadMHFilter() {
    const maKH = $("#MaKHSelectFilter").val();
    const url = `/api/TongHopKhoNPL/Get?Action=GETMHTK&para2=${maKH}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        const $maHangSelect = $("#MHSelectFilter");
        if ($maHangSelect.data('select2')) { $maHangSelect.select2('destroy'); }
        $maHangSelect.empty();
        $maHangSelect.append(`<option value="all">Tất cả</option>`);
        if (data.length > 0) {
            let html = '';
            data.map(x => {
                html += `<option value="${x.MaHang}">${x.TenHang || 'Mã hàng rỗng'}</option>`;
            });
            $maHangSelect.append(html);
        }
        $maHangSelect.select2();
    } catch (error) {
        console.error("Lỗi khi tải danh sách mã hàng:", error.message);
    }
}


