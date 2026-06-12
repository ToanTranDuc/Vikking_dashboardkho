/// VARIABLE
var userNameSave = localStorage.getItem("username1")
let dxDataGridHangXuat;
let dxDataPhieuDKVT;
let checkIndex = 0;
let checkIndexNguoiGhiNhan = 0;
let lstDSPhieuDKVT = [];
// ============= SIGNATURE CANVAS CODE =============
let canvas, ctx;
let isDrawing = false;
let penColor = '#000000';
let penSize = 2;
let signatureHistory = [];
let currentStroke = [];
var imageSign, idNguoiKy, scrollPosition;
/// API
const API = {
    async Get(endpoint = "", method = "Get", action, para = {}, textSuccess = '') {
        const paraConvert = Object.entries(para || {})
            .map(([key, value]) => `${key}=${value}`)
            .join("&");

        const url = `/api/${endpoint}/${method}?action=${action}${paraConvert ? '&' + paraConvert : ''}`;

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error(`Response status: ${response.status}`);

            const data = await response.json();
            if (textSuccess) showToast("success", textSuccess);
            return data;
        } catch (error) {
            console.error(error);
        }
    },

    async Post(endpoint = "", method = "Post", action, arrSave, textSuccess = "Lưu thành công!") {
        const url = `/api/${endpoint}/${method}?action=${action}`;
        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(arrSave),
            });

            if (!response.ok) throw new Error(`Response status: ${response.status}`);

            const data = await response.json();
            if (data == "True") showToast('success', textSuccess, 1500);
        } catch (error) {
            console.error(error);
        }
    }
};


async function GetPYC() {
    const data = await API.Get("DangKyVatTu", "Get", "GetPYC", { para1: isNPL })
    let html = ``;
    data.map(x => {
        html += `<option value="${x.MaLenhSX}">${x.MaLenh}</option> `
    })
    $("#malenh").html(html)
    GetPhieuDKVT();
}

async function GetPhieuDKVT() {
    var maLenh = $("#malenh option:selected").val()
    const data = await API.Get("DangKyVatTu", "Get", "GetPhieuDKVT", { para1: maLenh, para2: isNPL })
    if (data.length == 0) {
        $("#phieuxuathang").empty()
    }
    let html = ``;
    data.map(x => {
        html += ` <option  value="${x.PhieuDK}">${x.Display}</option>`
    })
    $("#phieuxuathang").html(html)
    GetViewPDKVT()
}

async function GetViewPDKVT() {
    var para1 = $("#phieuxuathang").val()
    var para3 = $("#malenh option:selected").data("value2")
    var maLenh = $("#malenh option:selected").val()
    const data = await API.Get("DangKyVatTu", "Get", "GetViewPDKVTCT", { para1: para1, para2: isNPL, para3: maLenh })
    const dataXNHX = await API.Get("DangKyVatTu", "Get", "GetCheckExist", { para1: para1, para2: maLenh, para3: isNPL })
    if (dataXNHX.length > 0) {
        const { KyTen, NguoiKT, NgayKy } = dataXNHX[0];

        data.forEach(item => {
            item.KyTenXNHX = KyTen;
            item.NguoiKT = NguoiKT;
            item.NgayKy = NgayKy;
        });
    } else {
        data.forEach(item => {
            item.KyTenXNHX = "";
            item.NguoiKT = "";
            item.NgayKy = "";
        });
    }
    updateGrid(dxDataGridHangXuat, data)
}

async function ApiSaveKiTen(arrSaveKT, action) {
    await API.Post("DangKyVatTu", "PostXNHX", action, arrSaveKT, "Lưu chữ ký thành công")
    await GetViewPDKVT()
}

async function UpdateKT(image) {
    var dataUser = !window.CefSharp ? userNameSave : dataUser = userName
    var ArrPhieuXHKT = []
    const pxhData = $("#phieuxuathang").val()
    const maLenh = $("#malenh").val()
    if (pxhData == "" || pxhData == null) {
        showToast("warning", "Vui lòng chọn phiếu đăng ký")
        return
    }
    const phieuXuatHang = {
        MaLenh: maLenh,
        PhieuXH: pxhData,
        KyTen: image,
        NguoiKT: dataUser,
        isNPL: isNPL
    };
    ArrPhieuXHKT.push(phieuXuatHang)
    await ApiSaveKiTen(ArrPhieuXHKT, "PostXNHX")
}

async function GetDSPhieuDKVT() {
    const data = await API.Get("DangKyVatTu", "Get", "GetViewPDKVT", { para1: 'all', para2: isNPL })

    lstDSPhieuDKVT = data
    updateGrid(dxDataPhieuDKVT, data)
}
/// HELPER FUNCTIONS
function updateGrid(grid, dataSource) {
    checkIndex = 0;
    checkIndexNguoiGhiNhan = 0;
    grid.beginUpdate();
    grid.option({ dataSource: dataSource });
    grid.endUpdate();
}

function setPenColor(color) {
    penColor = color;
    $('#penColor').val(color);
}

function setPenSize(size) {
    penSize = size;
    $('.pen-size-btn').removeClass('active');
    $(`.pen-size-btn[data-size="${size}"]`).addClass('active');
}

function saveSignature() {
    if (signatureHistory.length === 0) {
        showToast("warning", 'Vui lòng ký tên trước khi lưu!');
        return;
    }
    $('#signatureModal').modal('hide');
    // Chuyển canvas thành hình ảnh
    const signatureImage = canvas.toDataURL('image/png');
    UpdateKT(signatureImage)
}

function CallModal() {
    $('#signatureModal').modal('show');
}
$(document).ready(function () {
    canvas = document.getElementById('signatureCanvas');
    ctx = canvas.getContext('2d');

    // Khởi tạo kích thước canvas
    initCanvas();

    // Cấu hình canvas
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    // Sự kiện chuột
    $(canvas).on('mousedown', startDrawing);
    $(canvas).on('mousemove', draw);
    $(canvas).on('mouseup', stopDrawing);
    $(canvas).on('mouseleave', stopDrawing);

    // Sự kiện chạm (mobile)
    canvas.addEventListener('touchstart', handleTouchStart, { passive: false });
    canvas.addEventListener('touchmove', handleTouchMove, { passive: false });
    canvas.addEventListener('touchend', stopDrawing, { passive: false });

    // Cập nhật màu từ color picker
    $('#penColor').on('change', function () {
        penColor = $(this).val();
    });

    // Reset canvas khi mở modal
    $('#signatureModal').on('shown.bs.modal', function () {
        initCanvas();
        clearSignature();
    });

    // Resize canvas khi thay đổi kích thước màn hình
    $(window).on('resize', function () {
        if ($('#signatureModal').hasClass('show')) {
            initCanvas();
        }
    });
});

function initCanvas() {
    const container = canvas.parentElement;
    const containerWidth = container.offsetWidth;

    // Xác định kích thước canvas dựa trên màn hình
    if (window.innerWidth < 768) {
        canvas.width = Math.min(containerWidth - 40, 500);
        canvas.height = 250;
    } else {
        canvas.width = Math.min(containerWidth - 40, 700);
        canvas.height = 300;
    }

    // Cấu hình lại context sau khi thay đổi kích thước
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
}

function getMousePos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
    };
}

function getTouchPos(e) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;

    return {
        x: (e.touches[0].clientX - rect.left) * scaleX,
        y: (e.touches[0].clientY - rect.top) * scaleY
    };
}

function startDrawing(e) {
    isDrawing = true;
    const pos = getMousePos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!isDrawing) return;

    const pos = getMousePos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function stopDrawing() {
    if (isDrawing && currentStroke.length > 0) {
        signatureHistory.push([...currentStroke]);
        currentStroke = [];
    }
    isDrawing = false;
}

function handleTouchStart(e) {
    e.preventDefault();
    isDrawing = true;
    const pos = getTouchPos(e);
    currentStroke = [{ x: pos.x, y: pos.y, color: penColor, size: penSize }];

    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function handleTouchMove(e) {
    e.preventDefault();
    if (!isDrawing) return;

    const pos = getTouchPos(e);
    currentStroke.push({ x: pos.x, y: pos.y, color: penColor, size: penSize });

    ctx.strokeStyle = penColor;
    ctx.lineWidth = penSize;
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}

function clearSignature() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    signatureHistory = [];
    currentStroke = [];
}

function undoSignature() {
    if (signatureHistory.length === 0) return;

    signatureHistory.pop();
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Vẽ lại tất cả nét còn lại
    signatureHistory.forEach(stroke => {
        if (stroke.length === 0) return;

        ctx.beginPath();
        ctx.moveTo(stroke[0].x, stroke[0].y);

        stroke.forEach((point, index) => {
            if (index === 0) return;
            ctx.strokeStyle = point.color;
            ctx.lineWidth = point.size;
            ctx.lineTo(point.x, point.y);
            ctx.stroke();
        });
    });
}


/// EVENT
$(document).ready(function () {
    $(".select_2").select2();
    createViewDxDataGridHangXuat();
    createViewDxGridDanhSachPhieuDKVT();
    GetPYC();
})

$(function () {
    $("#home").on("click", () => window.location.href = '/Home/Dashboard');

    $("#btnEditAllPhieu").on("click", async function () {
        const phieuXH = $("#phieuxuathang").val();
        const maLenh = $("#malenh option:selected").val()
        const dataXH = await API.Get("DangKyVatTu", "Get", "GetCheckExistPXH", { para1: phieuXH, para2: maLenh, para3: isNPL })

        const data = await API.Get("DangKyVatTu", "Get", "GetCheckExist", { para1: phieuXH, para2: maLenh, para3: isNPL })

        if (dataXH.length == 0) {
            showToast("warning", "Phiếu này đã chưa được xuất hàng.Không được xác nhận")
            return;
        }
        if (data.length > 0) {
            showToast("warning", "Phiếu này đã được xác nhận")
            return;
        }

        CallModal()
    })

    $("#modalTimNhanh").on("shown.bs.modal", function () {
        setTimeout(function () {
            $("#inputTimKiem").focus();
        }, 100);
    });

    $("#modalTimNhanh").on("hide.bs.modal", function () {
        $("#inputTimKiem").val("").trigger("input")
    });

    $("#btnTimNhanh").on("click", function () {
        GetDSPhieuDKVT();

        $("#modalTimNhanh").modal('show')
    })

    // Nút tìm kiếm
    let searchTimeout;
    $("#inputTimKiem").on("input", function () {
        clearTimeout(searchTimeout);
        const search = $(this).val().trim().toLowerCase();

        searchTimeout = setTimeout(() => {
            if (search === "") {
                // Nếu rỗng thì hiện tất cả
                updateGrid(dxDataPhieuDKVT, lstDSPhieuDKVT);
            } else {
                // Lọc dữ liệu
                const dataFiltered = lstDSPhieuDKVT.filter(item =>
                    String(item.MaVT).trim().toLowerCase().includes(search) ||
                    String(item.MauVT).trim().toLowerCase().includes(search)
                );
                updateGrid(dxDataPhieuDKVT, dataFiltered);
            }
        }, 100); // Delay 300ms sau khi ngừng gõ
    });

})
function mergeCell(options, container, state, renderContent) {
    if (!options.data) return;

    const grid = options.component;
    const dataSource = grid.option("dataSource") || [];

    const key1 = options.data.PhieuXH;
    const key2 = options.data.MaNPL;

    const rowIndex = dataSource.findIndex(
        d => d.PhieuXH === key1 && d.MaNPL == key2
    );

    let rowspan = 0;

    if (state.value === 0) {
        for (let i = rowIndex; i < dataSource.length; i++) {
            if (
                dataSource[i].PhieuXH === key1 &&
                dataSource[i].MaNPL == key2
            ) {
                rowspan++;
            } else {
                break;
            }
        }
    }

    if (state.value === 0) {
        renderContent(container, options);
        container.attr("rowspan", rowspan);
        state.value = rowspan;
    } else {
        container.addClass("d-none");
    }

    state.value--;
}
const mergeState = {
    SLDK: { value: 0 },
    NguoiKT: { value: 0 },
    KyTen: { value: 0 }
};
/// DxDataGrid
function createViewDxDataGridHangXuat() {
    dxDataGridHangXuat = $("#dxDataGridHangXuat").dxDataGrid({
        dataSource: [],
        width: '100%',
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
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "MaVT",
                caption: "ItemCode",
                minWidth: 100,
                groupIndex: 0,
                calculateCellValue: function (rowData) {
                    if (!rowData) return "";
                    return `${rowData.MaVT} - SL yêu cầu: ${rowData.SLDK ?? ""} `;
                }
            },
            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                width: 500,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                minWidth: 80,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                minWidth: 80,
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn Vị",
                minWidth: 80,
            },
            {
                dataField: "SoKienHienThi",
                caption: `${isNPL == 1 ? "Số kiện / roll" : "Vật tư"}`,
                minWidth: 80,
            },
            {
                dataField: "SLDK",
                caption: "SL Yêu Cầu",
                minWidth: 80,
                visible: false,
                cellTemplate: function (container, options) {
                    mergeCell(options, container, mergeState.SLDK, (ctn, opt) => {
                        $("<div>")
                            .text(
                                Math.trunc(Number(opt.data.SLDK || 0) * 10000) / 10000
                            )
                            .appendTo(ctn);
                    });
                }
            },
            {
                dataField: "SLNhap",
                caption: "Thực xuất",
                width: 100,
                cellTemplate: function (container, options) {
                    const value = formatNumber(options.value)
                    $("<div>")
                        .text(value)
                        .addClass("thucnhap")
                        .toggleClass("text-danger", options.data.isCheckVuot === true)
                        .appendTo(container);
                }
            },
            {
                dataField: "NguoiKT",
                caption: "Người kí tên",
                minWidth: 80,
                cellTemplate: function (container, options) {
                    mergeCell(options, container, mergeState.NguoiKT, (ctn, opt) => {
                        $("<div>")
                            .text(opt.data.NguoiKT || "")
                            .appendTo(ctn);
                    });
                }
            },
            {
                dataField: "KyTenXNHX",
                caption: "Ký tên",
                minWidth: 120,
                cellTemplate: function (container, options) {

                    if (!options.data) return;

                    const grid = options.component;
                    const dataSource = grid.option("dataSource") || [];

                    const valueCheck = options.data.PhieuXH;
                    const valueCheckMaNPL = options.data.MaNPL;
                    const rowIndexInData = dataSource.findIndex(
                        d => d.PhieuXH === valueCheck && d.MaNPL == valueCheckMaNPL
                    );

                    let rowspan = 0;

                    if (checkIndex === 0) {
                        for (let i = rowIndexInData; i < dataSource.length; i++) {
                            if (dataSource[i].PhieuXH === valueCheck && dataSource[i].MaNPL == valueCheckMaNPL) {
                                rowspan++;
                            } else {
                                break;
                            }
                        }
                    }

                    if (checkIndex === 0) {

                        const containerDiv = $("<div>").css({
                            width: "100%",
                            height: "40px",
                            position: "relative",
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        });

                        // ảnh ký tên
                        if (options.data.KyTenXNHX) {
                            $("<img>")
                                .attr(
                                    "src",
                                    `/Images/XuatHangNPL/XacNhanKT/${options.data.KyTenXNHX}?${Date.now()}`
                                )
                                .css({ width: "70px", height: "35px" })
                                .appendTo(containerDiv);
                        }

                        containerDiv.appendTo(container);
                        container.attr("rowspan", rowspan);
                        checkIndex = rowspan;

                    } else {
                        container.addClass("d-none");
                    }

                    checkIndex--;
                }
            },


        ],
        summary: {
            totalItems: [
                {
                    name: "tongKho",
                    summaryType: "custom",
                    showInColumn: "ChiTiet",
                },
                {
                    column: "SLDK",
                    summaryType: "max",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return Math.trunc(Number(e.value) * 10000) / 10000;
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    customizeText(e) {
                        if (e.value == null) return "";
                        return formatNumber(e.value)
                    }
                }
            ],

            // ✅ SUMMARY THEO GROUP
            groupItems: [
                {
                    column: "SLDK",
                    summaryType: "max",
                    showInGroupFooter: true,
                    alignByColumn: true,
                    customizeText(e) {
                        return (
                            Math.trunc(Number(e.value || 0) * 10000) / 10000
                        );
                    }
                },
                {
                    column: "SLNhap",
                    summaryType: "sum",
                    showInGroupFooter: true,
                    alignByColumn: true,
                    customizeText(e) {
                        return formatNumber(e.value)
                    }
                }
            ],

            calculateCustomSummary: function (options) {
                if (options.name === "tongKho") {
                    if (options.summaryProcess === "start") {
                        options.totalValue = { totalRoll: 0 };
                    }
                    if (options.summaryProcess === "calculate") {
                        options.totalValue.totalRoll += 1;
                    }
                    if (options.summaryProcess === "finalize") {
                        options.totalValue =
                            `Tổng số roll/số kiện: ${options.totalValue.totalRoll.toLocaleString()}`;
                    }
                }
            }
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
        onContentReady: function () {
            Object.values(mergeState).forEach(s => s.value = 0);
        }
    }).dxDataGrid("instance");
    $("#Layer_1").click()
}

function createViewDxGridDanhSachPhieuDKVT() {
    dxDataPhieuDKVT = $("#dxDataPhieuDKVT").dxDataGrid({
        dataSource: [],
        rowAlternationEnabled: true,
        width: '100%',
        noDataText: "",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: false },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        columns: [
            {
                dataField: "PhieuDK",
                caption: "Phiếu",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const $wrapper = $(`<div></div>`).css({
                        display: "flex",
                        justifyContent: "center",
                        height: "25px",
                        gap: "8px"
                    })

                    // Text
                    const $span = $(`<span>${options.data.PhieuDK}</span>`)

                    // Icon
                    const $btnNext = $(`<i class="fa-solid fa-circle-right"></i>`)
                        .css({ fontSize: '16px', color: 'green', cursor: 'pointer' })
                        .on("click", function () {
                            $("#malenh").val(`${options.data.MaLenhSX}`).trigger("change");
                            setTimeout(() => {
                                $("#phieuxuathang").val(`${options.data.PhieuDK}`).trigger("change");
                            }, 300)

                            $("#modalTimNhanh").modal('hide');
                            $("#inputTimKiem").val('').trigger('input');
                            updateGrid(dxDataPhieuDKVT, []);
                        })

                    $wrapper.append($span, $btnNext).appendTo(container)
                }
            },
            {
                dataField: "MaLenh",
                caption: "Mã Lệnh Sản Xuất",
                width: 400,
                cellTemplate: function (container, options) {
                    const $wrapper = $(`<div></div>`).css({ display: "flex", justifyContent: "center", height: "25px", gap: "8px" })

                    // Text
                    const $span = $(`<span>${options.data.MaLenh}</span>`)

                    // Icon
                    const $btnNext = $(`<i class="fa-solid fa-circle-right"></i>`)
                        .css({ fontSize: '16px', color: 'green' })
                        .on("click", function () {
                            $("#malenh").val(`${options.data.MaLenhSX}`).trigger("change");
                            $("#modalTimNhanh").modal('hide');
                            $("#inputTimKiem").val('').trigger('input');
                            updateGrid(dxDataPhieuDKVT, []);
                        })

                    $wrapper.append($span, $btnNext).appendTo(container)

                }
            },
            {
                dataField: "MaVT",
                caption: "ItemCode",
                minWidth: 100,
            },
            {
                dataField: "MauVT",
                caption: "Màu",
                minWidth: 100,
            },
            {
                dataField: "KhoVai",
                caption: "Width/Size",
                minWidth: 100,
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn Vị",
                minWidth: 100,
                width: 100,
            },
            {
                dataField: "SLDK",
                caption: "SL Yêu Cầu",
                minWidth: 100,
            },
            {
                dataField: "SLNhap",
                caption: "Thực xuất",
                minWidth: 100,
                cellTemplate: function (container, options) {
                    const value = formatNumber(options.value);
                    $("<div>")
                        .text(value)
                        .addClass("thucnhap")
                        .toggleClass("text-danger", options.data.isCheckVuot === true)
                        .appendTo(container);
                }
            },
        ],

        onCellPrepared: function (e) {
            if (e.rowType === "header") {
                $(e.cellElement).addClass("col-header");
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");
            }
            if (e.rowType === "data") {
                $(e.cellElement).addClass("text-center");
                $(e.cellElement).css("vertical-align", "middle");

                const searchTerm = $("#inputTimKiem").val().trim().toLowerCase();
                if (searchTerm && (e.column.dataField === "MaVT" || e.column.dataField === "MauVT")) {
                    const cellValue = String(e.value || "");
                    const cellValueLower = cellValue.toLowerCase();

                    if (cellValueLower.includes(searchTerm)) {
                        // Tìm vị trí bắt đầu của text match
                        const startIndex = cellValueLower.indexOf(searchTerm);
                        const endIndex = startIndex + searchTerm.length;

                        // Tạo HTML với phần match được highlight
                        const before = cellValue.substring(0, startIndex);
                        const match = cellValue.substring(startIndex, endIndex);
                        const after = cellValue.substring(endIndex);

                        const highlightedHTML = `${before}<mark style="background-color: #ffeb3b; font-weight: 600; padding: 2px 4px; border-radius: 3px;">${match}</mark>${after}`;

                        $(e.cellElement).html(highlightedHTML);
                    }
                }
            }
        },
    }).dxDataGrid("instance");

    $("#Layer_1").click()
}

function formatNumber(value) {
    let str = value.toString();
    let intPart = str;
    let decPart = "";

    if (str.includes(".")) {
        [intPart, decPart] = str.split(".");
        decPart = decPart.substring(0, 4); // cắt, không làm tròn
    }

    // format phần nguyên
    let formattedInt = Number(intPart).toLocaleString();

    return decPart ? `${formattedInt}.${decPart}` : formattedInt;
}