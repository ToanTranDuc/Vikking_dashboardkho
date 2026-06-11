let user = localStorage.getItem("username1")
let dxTbody;
let dxTbodyTotal;
let arrCheck = [];
let rawData = [];
toastr.options = {
    "closeButton": true,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
/// EVENT
$(document).ready(function () {
    $(".select_2").select2();
    initializeDatePickers();
    createViewDxtbody([]);
    createViewDxtbodyTotal([]);
})
$(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })

    $("#tuNgay").on("change", function () {
        getMaHang();
    })
    $("#denNgay").on("change", function () {
        getMaHang();
    })
    $("#mahang").on("change", function () {
        arrCheck = []; 
        filterAndRender();
    });
    getMaHang()
    $("#xacnhan").on("click", function () {
        const isNotValid = arrCheck.some(item => item.IsXacNhan == "Đã xác nhận")
        if (arrCheck.length === 0) {
            toastr.error("Vui lòng chọn đơn hàng để xác nhận")
            return;
        }
        if (isNotValid) {
            toastr.error("Có đơn hàng đã được xác nhận")
            return;
        }

        showConfirmation()
    })

    $("#huyxacnhan").on("click", function () {
        const isNotValid = arrCheck.some(item => item.IsXacNhan == "Chưa xác nhận")

        if (arrCheck.length === 0) {
            toastr.error("Vui lòng chọn đơn hàng để xác nhận")
            return;
        }

        if (isNotValid) {
            toastr.error("Có đơn hàng chưa được xác nhận")
            return;
        }

        console.log("")

        showConfirmationCancle()
    })
})

// Confirmt
function showConfirmation() {
    Swal.fire({
        title: 'Bạn muốn xác nhận?',
        text: "Hành động này không thể hoàn tác!",
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy bỏ',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                let filterUpdateXNNK = arrCheck.filter(item => {
                    return item.MaHang === $("#mahang").val() &&
                        item.MaDH === $("#mahang option:selected").data("dh");
                });

                const promises = filterUpdateXNNK.map(item => {
                    let object = {
                        DonHang: $("#mahang option:selected").data("dh"),
                        MaHang: $("#mahang").val(),
                        MaPKL: item.MaPKL,
                        NgayNhapKho: chuyendoidate(item.NgayNhapKho),
                        User: user
                    };
                    return updateXNNK(object);
                });

                

                await Promise.all(promises); 

                filterUpdateXNNK.forEach(item => {
                    const found = rawData.find(r =>
                        r.MaPKL === item.MaPKL &&
                        r.NgayNhapKho === item.NgayNhapKho &&
                        r.MaDH === item.MaDH
                    );
                    if (found) found.IsXacNhan = "Đã xác nhận";
                });

                arrCheck = [];

                // Cập nhật lại dataSource cho dxTbody
                const selectedMaDH = $("#mahang option:selected").data("dh");
                const dataFiltered = rawData.filter(r => r.MaDH === selectedMaDH);
                dxTbody.option("dataSource", dataFiltered);

                Swal.fire({
                    title: 'Đã xác nhận!',
                    icon: 'success',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });

            } catch (error) {
                console.error("Lỗi xảy ra:", error);
                Swal.fire({ title: 'Có lỗi xảy ra!', text: error.message, icon: 'error' });
            }
        }
    });
}

function showConfirmationCancle() {
    Swal.fire({
        title: 'Bạn muốn hủy xác nhận?',
        text: "Hành động này không thể hoàn tác!",
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy bỏ',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                let filterUpdateXNNK = arrCheck.filter(item => {
                    return item.MaHang === $("#mahang").val() &&
                        item.MaDH === $("#mahang option:selected").data("dh")
                })

                const promises = filterUpdateXNNK.map(item => {
                    let object = {
                        DonHang: $("#mahang option:selected").data("dh"),
                        MaHang: $("#mahang").val(),
                        MaPKL: item.MaPKL,
                        NgayNhapKho: chuyendoidate(item.NgayNhapKho),
                        User: user,
                    }
                    return updateXNNK(object)
                })

                await Promise.all(promises); 

                filterUpdateXNNK.forEach(item => {
                    const found = rawData.find(r =>
                        r.MaPKL === item.MaPKL &&
                        r.NgayNhapKho === item.NgayNhapKho &&
                        r.MaDH === item.MaDH
                    );
                    if (found) found.IsXacNhan = "Chưa xác nhận";
                });

                arrCheck = [];

                // Cập nhật lại dataSource cho dxTbody
                const selectedMaDH = $("#mahang option:selected").data("dh");
                const dataFiltered = rawData.filter(r => r.MaDH === selectedMaDH);
                dxTbody.option("dataSource", dataFiltered);

                Swal.fire({
                    title: 'Đã hủy xác nhận!',
                    icon: 'success',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });

            } catch (error) {
                console.error("Lỗi xảy ra:", error);
                Swal.fire({ title: 'Có lỗi xảy ra!', text: error.message, icon: 'error' });
            }
        }
    });
}


/// API
async function getMaHang() {
    const tuNgay = moment($("#tuNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD");
    const denNgay = moment($("#denNgay").val(), "DD/MM/YYYY").format("YYYY-MM-DD");
    const url = `/api/ThongKeDongThung/GetNK?Action=Get&Para1=${tuNgay}&Para2=${denNgay}&Para3=A&Para4=A&Para5=A`

    try {
        const selectMaHang = $("#mahang");
        selectMaHang.empty();

        const response = await fetch(url);
        const data = await response.json();
        rawData = data; // ← Lưu lại data gốc

        let dataMap = data.map((item) => ({
            MaHang: item.MaHang,
            MaDH: item.MaDH,
            NgayNK: item.NgayNhapKho,
            MaPKL: item.MaPKL,
            Display: item.Display
        }));

        let uniqueMap = dataMap.reduce((unique, item) => {
            return unique.some(t => t.MaDH === item.MaDH) ? unique : [...unique, item];
        }, []);

        let html = ``;
        uniqueMap.forEach(item => {
            html += `<option data-dh="${item.MaDH}" value="${item.MaHang}">${item.Display}</option>`;
        });

        selectMaHang.append(html);
        selectMaHang.select2();

        // Filter theo option đầu tiên sau khi load
        filterAndRender();

    } catch (err) {
        console.error(err);
    }
}

function filterAndRender() {
    const selectedMaDH = $("#mahang option:selected").data("dh");
    const dataFiltered = rawData.filter(item => item.MaDH === selectedMaDH);
    createViewDxtbody(dataFiltered);
}

async function GetCT_PKL(maDH, maPKL, poID, ngayNK) {
    const url = `/api/ThongKeDongThung/GetCTNK?Action=GetCT_PKL&Para1=${maDH}&Para2=${poID}&Para3=${maPKL}&Para4=${ngayNK}&Para5=A`
    try {
        const response = await fetch(url)

        const data = await response.json();
        createViewDxtbodyTotal(data);
    } catch (err) {
        console.error(err)
    }
}

async function updateXNNK(object) {
    const url = `/api/ThongKeDongThung/PostNK`
    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-type": "application/json" },
            body: JSON.stringify(object),
        })
        const result = await response.json();
        return result; // 
    } catch (err) {
        console.error(err)
        return null;
    }
}


/// HELPER FUNCTIONS
function getFirstDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
}

function getLastDayOfCurrentMonth() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 1, 0);
}

function chuyendoidate(date) {
    let dataformat = date.split('/')
    let resultDateFormat = dataformat[2].trim() + '-' + dataformat[1].trim() + '-' + dataformat[0].trim()
    return resultDateFormat;
}

function initializeDatePickers() {
    const picker1 = new tempusDominus.TempusDominus(document.getElementById("datetimepicker"), {
        container: document.body,
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            },
            placement: 'bottom'
        },
        localization: { format: 'dd/MM/yyyy' },
        defaultDate: getFirstDayOfCurrentMonth()
    });

    const picker2 = new tempusDominus.TempusDominus(document.getElementById("datetimepicker1"), {
        container: document.body,
        display: {
            components: {
                calendar: true, date: true, month: true, year: true,
                clock: false, hours: false, minutes: false, seconds: false
            },
            placement: 'bottom'
        },
        localization: { format: 'dd/MM/yyyy' },
        defaultDate: getLastDayOfCurrentMonth()
    });



    $("#tuNgay").trigger("click");
    $("#denNgay").trigger("click");

    picker1.hide();
    picker2.hide();
}


// DxDatagrid
function createViewDxtbody(data) {
    dxTbody = $("#dxtbody").dxDataGrid({
        dataSource: data,
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        rowAlternationEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        selection: {
            mode: "single",        // "single" | "multiple" | "none"
            showCheckBoxesMode: "onClick", // "always" | "onClick" | "onLongTap" | "none"
            allowSelectAll: false,
        },
        columns: [
            {
                dataField: "MaPKL",
                caption: "Phiếu",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "PO",
                caption: "PO",
                alignment: "center",
                minWidth: 80,
            },

            {
                dataField: "TenKH",
                caption: "Khách hàng",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "carton",
                caption: "Carton",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "IsXacNhan",
                caption: "Trạng thái",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "NgayNhapKho",
                caption: "Ngày nhập kho",
                alignment: "center",
                minWidth: 80,
            },
            {
                dataField: "CheckA",
                caption: "Xác Nhận",
                alignment: "center",
                minWidth: 80,
                headerCellTemplate: function (container) {
                    const $headerCheckbox = $(`<input>`)
                        .prop("type", "checkbox")
                        .css({ "width": 20, "height": 20 })
                        .on("click", function (e) {
                            e.stopPropagation();
                            const isChecked = $(this).prop("checked");
                            $("#dxtbody input[type='checkbox']").not(this).each(function () {
                                $(this).prop("checked", isChecked);
                            });
                            if (isChecked) {
                                arrCheck = [];
                                dxTbody.getVisibleRows().forEach(function (row) {
                                    if (row.rowType === "data") {
                                        arrCheck.push({
                                            ...row.data,
                                            rowIndex: row.rowIndex
                                        });
                                    }
                                });
                            } else {
                                arrCheck = [];
                            }
                        });
                    container.append($headerCheckbox);
                   
                },
                cellTemplate: function (container, options) {
                    const $input = $(`<input>`)
                        .prop("type", "checkbox")
                        .css({ "width": 20, "height": 20 })
                        .on("click", function (e) {
                            e.stopPropagation();
                            const isChecked = $(this).prop("checked")
                            const data = {
                                ...options.data,
                                rowIndex: options.rowIndex,
                            }
                            if (isChecked) {
                                arrCheck.push(data)
                            } else {
                                arrCheck = arrCheck.filter(item => item.rowIndex !== options.rowIndex)
                            }
                        })
                    container.append($input)
                }
            },

        ],
        onContentReady: function (e) {

            const dataSource = e.component.option("dataSource");
            if (dataSource && dataSource.length > 0) {
                const firstDataRow = e.component.getVisibleRows().find(r => r.rowType === "data");
                if (firstDataRow) {
                    e.component.selectRowsByIndexes([firstDataRow.rowIndex]);
                    let item = firstDataRow.data;
                    let maDH = item.MaDH;
                    let maPKL = item.MaPKL;
                    let poID = item.POID;
                    let ngayNK = chuyendoidate(item.NgayNhapKho);
                    GetCT_PKL(maDH, maPKL, poID, ngayNK);
                }
            }
        },
        onRowClick: function (e) {
            let item = e.data;
            let maDH = item.MaDH;
            let maPKL = item.MaPKL;
            let poID = item.POID;
            let ngayNK = chuyendoidate(item.NgayNhapKho);
            GetCT_PKL(maDH, maPKL, poID, ngayNK);
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                if (e.column.dataField == "IsXacNhan") {
                    if (e.data.IsXacNhan === "Đã xác nhận") {
                        $(e.cellElement).addClass("text-success");
                        $(e.cellElement).css("font-weight", "600");
                    } else {
                        $(e.cellElement).addClass("text-danger");
                        $(e.cellElement).css("font-weight", "600");
                    }
                }
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },
    }).dxDataGrid("instance");

    $("#Layer_1").click()
}

function createViewDxtbodyTotal(data) {

    let sizeColumns = [];
    if (data && data.length > 0) {
        let keys = Object.keys(data[0]);
        let filteredKeys = keys.filter(key => key.includes('@SIZE_'));
        sizeColumns = filteredKeys.map(key => ({
            dataField: key,
            caption: key.split("@")[0],
            alignment: "center",
            minWidth: 50,
            calculateCellValue: function (rowData) {
                return rowData[key] == 0 ? '' : rowData[key];
            }
        }));
    }
    let fixedColumns = [
        {
            caption: "Carton Number",
            columns: [
                { dataField: "TuThung", caption: "Từ thùng", alignment: "center", minWidth: 80 },
                { dataField: "DenThung", caption: "Đến thùng", alignment: "center", minWidth: 80 },
            ]
        },
        { dataField: "PO", caption: "PO", alignment: "center", minWidth: 120 },
        { dataField: "TenDVSX", caption: "Đơn vị sản xuất", alignment: "center", minWidth: 120 },
        { dataField: "DotSX", caption: "Đợt SX", alignment: "center", minWidth: 120 },
        { dataField: "DauSize", caption: "InSeam", alignment: "center", minWidth: 70 },
        { dataField: "TenMau", caption: "Color", alignment: "center", minWidth: 80 },
        { caption: "Size", columns: sizeColumns },
        { dataField: "SoLuong", caption: "Qty", alignment: "center", minWidth: 60 },
        { dataField: "SLThung", caption: "Carton", alignment: "center", minWidth: 60 },
        { dataField: "TotalPiece", caption: "Total Piece", alignment: "center", minWidth: 80 },
        { dataField: "SLThung_NK", caption: "SL Nhập kho", alignment: "center", minWidth: 80 },
    ];

    if (dxTbodyTotal) {
        dxTbodyTotal.dispose();
        dxTbodyTotal = null;
        $("#dxtbodytotal").empty();
    }

    dxTbodyTotal = $("#dxtbodytotal").dxDataGrid({
        dataSource: data,
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        rowAlternationEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: false },
        headerFilter: { visible: false },
        paging: { enabled: false },
        columns: fixedColumns,
        summary: {
            totalItems: [
                { column: "SoLuong", summaryType: "sum", displayFormat: "{0}" },
                { column: "SLThung", summaryType: "sum", displayFormat: "{0}" },
                { column: "TotalPiece", summaryType: "sum", displayFormat: "{0}" },
            ]
        },
        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
        },
    }).dxDataGrid("instance");
}