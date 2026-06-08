var userNameSave = localStorage.getItem("username1")
var thisTr;
var checkNhap;
var solotIDBarCode = ""
var succhuaOCL = 0
// load trang
const params = new URLSearchParams(window.location.search);
// Lấy từng tham số
const KH = params.get("KH");
const MHSL = params.get("MHSL");
const NPL = params.get("NPL");
const SoLo = params.get("SoLo");
const MHSoLo = params.get("MHSoLo");
const IsNPL = params.get("IsNPL");
function playSuccessSound() {
    try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        oscillator.frequency.value = 1200;
        oscillator.type = 'sine';
        gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.2);

        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.2);
    } catch (err) {
        console.log('Không thể phát âm thanh:', err);
    }
}

// Phát âm thanh lỗi/cảnh báo
function playErrorSound() {
    try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        // Âm thanh thấp hơn cho lỗi
        oscillator.frequency.value = 400;
        oscillator.type = 'square';
        gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.5);

        // Rung mạnh hơn cho cảnh báo
        if (navigator.vibrate) {
            navigator.vibrate([200, 100, 200]);
        }
    } catch (err) {
        console.log('Không thể phát âm thanh lỗi:', err);
    }
}
function PlayAudio() {
    const beepSound = document.getElementById("beepSound");
    beepSound.currentTime = 0;
    beepSound.play();
}
function PlayAudioError() {
    const beepSoundE = document.getElementById("beepSoundError");
    beepSoundE.currentTime = 0;
    beepSoundE.play();
}
$(function () {
    if (window.CefSharp) {
        $("#home").hide();
    }
    $(".select_2").select2();
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    $(".enterbarcode").on("click", function () {
        //$("#globalSearchInput").val($("#globalBarcode").val())
        //$("#globalSearchInput").focus()
    })
    if (KH == null)
        GetKHViTriKho()
    else {
        $("#nguyenphulieuid").val(IsNPL).trigger("change")
    }
    $("#btnBack").on("click", function () {
        if (KH != null)
            window.history.back(); // Không load lại, giữ nguyên trạng thái trang trước
    });

});

var warehouseData;
let currentFilter = 'all';
var warehouseDataChiTiet;
async function GetKHViTriKho() {
    GetViTriKho()
    GetDanhSachO()

    const isNPL = $("#nguyenphulieuid").val()
    if (isNPL == 1) {
        $(".txtSoDoKho").text("SƠ ĐỒ KHO NGUYÊN LIỆU")
    } else {
        $(".txtSoDoKho").text("SƠ ĐỒ KHO PHỤ LIỆU")
    }
    const url = `/api/ERPVatTuCBM/Get?action=GetKhachHangVTK&para=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = `<option value="">Tất cả</option>`;
        data.map(x => {
            html += `
                <option value="${x.MaKH}">${x.TenKH}</option>
            `

        })
        $("#khachhangid").html(html)
        if (KH == null)
            GetMHViTriKho()
        else {
            $("#khachhangid").val(KH).trigger("change")
        }
    } catch (error) {
        console.error(error.message);
    }
}
async function GetMHViTriKho() {
    const isNPL = $("#nguyenphulieuid").val()
    const khachhang = $("#khachhangid").val()
    const url = `/api/ERPVatTuCBM/Get?action=GetMaHangVTK&para=${isNPL}&para1=${khachhang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;
        data.map(x => {
            html += `
                <option value="${x.MaHang}">${x.TenHang}</option>
            `

        })
        $("#mahangid").html(html)
        if (KH == null)
            GetVatTuViTriKho()
        else {
            $("#mahangid").val(MHSL).trigger("change")
        }
    } catch (error) {
        console.error(error.message);
    }
}
async function GetVatTuViTriKho() {
    const isNPL = $("#nguyenphulieuid").val()
    const khachhang = $("#khachhangid").val()
    const mahang = $("#mahangid").val()
    const url = `/api/ERPVatTuCBM/Get?action=GetVatTuVTK&para=${isNPL}&para1=${khachhang}&para2=${mahang}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;
        data.map(x => {
            html += `
                <option value="${x.MaNPL}">${x.Display}</option>
            `

        })
        $("#cayvai").html(html)
        if (KH == null)
            GetSoLoViTriKho()
        else {
            $("#cayvai").val(NPL).trigger("change")
        }

    } catch (error) {
        console.error(error.message);
    }
}
async function GetSoLoViTriKho() {
    const isNPL = $("#nguyenphulieuid").val()
    const khachhang = $("#khachhangid").val()
    const mahang = $("#mahangid").val()
    const cayvai = $("#cayvai").val()
    const url = `/api/ERPVatTuCBM/Get?action=GetSoLoVTK&para=${isNPL}&para1=${khachhang}&para2=${mahang}&para3=${cayvai}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = ``;
        data.map(x => {
            html += `
                <option data-mahang="${x.MaHang}" value="${x.SoLoID}">${x.Display}</option>
            `
        })
        $("#soloid").html(html)
        if (KH == null)
            GetCTietNKViTriKho()
        else {
            const option = $(`#soloid option[value='${SoLo}'][data-mahang='${MHSoLo}']`);
            if (option.length > 0) {
                option.prop("selected", true);
                $("#soloid").val(SoLo).trigger("change");
            }
            if (data.length == 1) {
                GetCTietNKViTriKho()
            }
        }
    } catch (error) {
        console.error(error.message);
    }
}
async function GetCTietNKViTriKho() {
    const isNPL = $("#nguyenphulieuid").val()
    const khachhang = $("#khachhangid").val()
    const mahang = $("#soloid option:selected").data("mahang")
    const cayvai = $("#cayvai").val()
    const solo = $("#soloid").val()
    const url = `/api/ERPVatTuCBM/Get?action=GetCTNhapKhoVTK&para=${isNPL}&para1=${khachhang}&para2=${mahang}&para3=${cayvai}&para4=${solo}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        renderNhapKhoTable(data)
        GetViTriKhoChiTiet()
    } catch (error) {
        console.error(error.message);
    }
}

function renderNhapKhoTable(data) {
    const sumTrongKe = data
        .filter(x => x.IsKho === 2 && x.CheckXH === 1)
        .reduce((sum, x) => sum + (x.SLNhap || 0), 0);

    const sumNgoaiKe = data
        .filter(x => x.IsKho === 0 && x.CheckXH === 1)
        .reduce((sum, x) => sum + (x.SLNhap || 0), 0);
    const sumXuatHang = data
        .filter(x => x.CheckXHV1 === 3)
        .reduce((sum, x) => sum + (x.SLXuat || 0), 0);
    data = data.filter(x => x.CheckXH === 1)
    $(".tongCBM").text(0)
    $("#tbody").dxDataGrid({
        dataSource: data,
        keyExpr: "ID",
        columns: [
            {
                dataField: "CayVai_Group",
                caption: "Số lô",
                cssClass: "col-header",
                minWidth: 100,
                groupIndex: 0,
                calculateCellValue: function (rowData) {
                    if (!rowData) return "";
                    return `${rowData.SoLo} - MH:${rowData.MaHang ?? ""} - Vật tư: ${rowData.DisplayGroup} - Màu vật tư: ${rowData.MauVT}`;
                }
            },
            {
                caption: "",
                width: 60,
                cssClass: "col-header",
                headerCellTemplate: function (container, options) {
                    // Tạo checkbox header
                    let $checkAll = $("<input>", {
                        type: "checkbox",
                        id: "checkAllNK",
                        style: "width:18px;height:18px;"
                    });

                    // Thêm vào header cell
                    $("<div>")
                        .css({
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        })
                        .append($checkAll)
                        .appendTo(container);
                },
                cellTemplate: function (container, options) {
                    container.addClass(`${options.data.IsKho == 2 ? "itemNK checknhapkho" : "itemNK"}`)
                    var $div = $(`<div class="divNK">`)
                        .css({
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        });

                    var $checkbox = $("<input>", {
                        type: "checkbox",
                        class: `checkNK ${options.data.IsKho == 2 ? "d-none" : ""}`,
                        style: "width:18px;height:18px;",
                        checked: false
                    });

                    if (options.data.IsKho == 2) {
                        $checkbox.css("pointer-events", "none");
                    }

                    $div.append($checkbox).appendTo(container);
                }
            },
            {
                caption: "STT",
                width: 50,
                cssClass: "col-header",
                cellTemplate: function (container, options) {
                    let grid = options.component;
                    let visibleRows = grid.getVisibleRows();
                    let dataIndex = visibleRows
                        .filter(r => r.rowType === "data")
                        .findIndex(r => r.rowIndex === options.rowIndex);

                    if (dataIndex >= 0) {
                        $("<div>")
                            .text(dataIndex + 1)
                            .appendTo(container);
                    }
                }
            },
            {
                dataField: "SoLoT",
                caption: "Số LOT",
                width: 100,
                cssClass: "solot col-header"
            },
            {
                dataField: "MaVT",
                caption: "ItemCode",
                width: 100,
                cssClass: "solot col-header"
            },
            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                minWidth: 200,
                cssClass: "chiTietNK col-header"
            },
            {
                dataField: "KhoVai",
                caption: "Khổ vải",
                cssClass: "col-header",
                width: 100
            },
            {
                dataField: "TenDVVT",
                caption: "Đơn vị",
                width: 120,
                cssClass: "donvi col-header"
            },
            {
                dataField: "SoKien",
                caption: "Vật tư",
                width: 100,
                cssClass: "sokien col-header"
            },
            {
                dataField: "SLChungTu",
                caption: "SL chứng từ",
                cssClass: "col-header",
                width: 100,
                cellTemplate: function (container, options) {
                    $("<div>")
                        .addClass("itemct")
                        .append(`<span  class="SLCT">${options.data.SLChungTu.toLocaleString()}</span>`)
                        .appendTo(container);
                }
            },
            {
                dataField: "SLNhap",
                caption: "SL nhập",
                width: 100,
                cssClass: "col-header",
                calculateCellValue: function (rowData) {
                    if (!rowData) return "";
                    return `${rowData.SLNhap.toLocaleString()}`;
                }
            },
            {
                dataField: "ViTriO",
                caption: "Vị trí Ô",
                width: 100,
                cssClass: "col-header vitrioitem"
            },

            {
                dataField: "CBM",
                caption: "CBM",
                width: 110,
                cssClass: "cbm col-header",
                cellTemplate: function (container, options) {
                    const cbmValue = options.data.CBM != null && !isNaN(options.data.CBM)
                        ? options.data.CBM.toFixed(4)
                        : "";
                    $("<div>")
                        .css({
                            width: "100%",
                            position: "relative",
                            paddingRight: "15px"
                        })
                        .append(
                            $("<input>", {
                                type: "number",
                                readonly: true,
                                value: parseFloat(cbmValue),
                                class: "inputSLN",
                                style: "border:none;outline:none;text-align:center;background:transparent;font-weight:700;width:100%;",

                            })
                        )

                        .append(`<i class="fa-solid fa-pen-to-square editcbm ${options.data.IsKho == 2 ? "d-none" : ""}"></i>`)
                        .appendTo(container);
                },
                customizeText: e => formatNumberUS(e.value || 0)
            },
            {
                caption: "",
                width: 60,
                cssClass: "col-header",
                headerCellTemplate: function (container, options) {
                    // Tạo checkbox header
                    let $checkAll = $("<input>", {
                        type: "checkbox",
                        id: "checkAllCBM",
                        style: "width:18px;height:18px;"
                    });

                    // Thêm vào header cell
                    $("<div>")
                        .css({
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        })
                        .append($checkAll)
                        .appendTo(container);
                },
                cellTemplate: function (container, options) {
                    var $div = $(`<div class="divNK">`)
                        .css({
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center"
                        });

                    var $checkbox = $("<input>", {
                        type: "checkbox",
                        class: `checkCBM ${options.data.IsKho == 2 ? "d-none" : ""}`,
                        style: "width:18px;height:18px;",
                        checked: false
                    })

                    if (options.data.IsKho == 2) {
                        $checkbox.css("pointer-events", "none");
                    }
                    $div.append($checkbox).appendTo(container);
                }
            }
        ],
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },

        showBorders: true,
        columnAutoWidth: true,
        wordWrapEnabled: true,
        noDataText: "Không có dữ liệu",
        paging: {
            enabled: false
        },
        scrolling: {
            mode: "standard"
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: false
        },
        onContentReady: function (e) {
            setTimeout(function () {
                $("#tbody .custom-footer-row").remove();

                const $footer = $(`
                    <div class="custom-footer-row" style="
                        position: sticky;
                        bottom: 0;
                        background: #f5f5f5;
                        font-weight: bold;
                        color: red;
                        padding: 8px 40px;
                        border-top: 2px solid #ddd;
                        z-index: 10;
                    ">
                        Trong kệ: ${parseFloat(sumTrongKe.toFixed(4))} -- Ngoài kệ: ${parseFloat(sumNgoaiKe.toFixed(4))} -- Xuất hàng: ${parseFloat(sumXuatHang.toFixed(4))}
                    </div>
                `);

                $("#tbody").append($footer);

            }, 100);
        },
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.attr("data-cbm", e.data.CBM);
                e.rowElement.attr("data-mavtghep", e.data.MaVTGhep);
                e.rowElement.attr("data-sokien", e.data.SoKien);
                e.rowElement.attr("data-npl", e.data.MaNPL);
                e.rowElement.attr("data-barcode", e.data.BarCode);
                e.rowElement.attr("data-soloid", e.data.SoLoID);

                e.rowElement.attr("data-typecbm", e.data.IsCBM);
                e.rowElement.attr("data-dai", e.data.Dai);
                e.rowElement.attr("data-rong", e.data.Rong);
                e.rowElement.attr("data-cao", e.data.Cao);
                e.rowElement.attr("data-duongkinh", e.data.DK);
                e.rowElement.attr("data-iskho", e.data.IsKho);
                if (e.data.IsKho == 2) {
                    e.rowElement.css("background", "#b5fff8");
                }
            }
        },
    });

    $("#Layer_1").click();
}
function formatNumberUS(num) {
    if (num == null) return "";
    let parts = num.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}
async function GetViTriKho() {
    const isNPL = $("#nguyenphulieuid").val() == 1 ? 1 : 2
    const url = `/api/ViTriKhoNPL/Get?action=GetViTriKho&para1=${isNPL}&para2=${$("#nguyenphulieuid").val()}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        GetViTriKhoChiTiet()
        warehouseData = await transformData(data.Table);
        renderFilters();
        $("#warehouseAisles").html(generateWarehouseHTML());
        setupGlobalSearch();
        updateStats();
        initAisleCollapse();
    } catch (error) {
        console.error(error.message);
    }
}
async function GetViTriKhoChiTiet() {
    const module = $("#nguyenphulieuid").val() == 1 ? 1 : 2
    const isNPL = $("#nguyenphulieuid").val()
    const url = `/api/ViTriKhoNPL/Get?action=DetailONo&para1=${module}&para2=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        warehouseDataChiTiet = await data.Table;
    } catch (error) {
        console.error(error.message);
    }
}
async function GetDanhSachO() {
    const isNPL = $("#nguyenphulieuid").val() == 1 ? 1 : 2
    const url = `/api/ViTriKhoNPL/Get?action=Get&para1=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);
        $("#vattuchuyen").empty()
        $("#vattuden").empty()
        const data = await response.json();
        let html = ``;
        let htmloCanChuyen = ``;
        data.Table4.map(x => {
            const partValue = x.TenO.split(".")
            html += `<option value="${x.TenO}">Dãy: ${partValue[0]} - Kệ: ${partValue[1]} - T: ${partValue[2]} - ${x.TenO}</option>`

            if (x.CBM > 0) {
                htmloCanChuyen += `<option value="${x.TenO}">Dãy: ${partValue[0]} - Kệ: ${partValue[1]} - T: ${partValue[2]} - ${x.TenO}</option>`
            }
        })
        $("#vattuchuyen").html(htmloCanChuyen).val("")
        $("#vattuden").html(html).val("")
    } catch (error) {
        console.error(error.message);
    }
}
async function fetchSlotItems(slotId) {
    try {
        const module = $("#nguyenphulieuid").val() == 1 ? 1 : 2
        const response = await fetch(`/api/ViTriKhoNPL/Get?action=DetailO&para1=${slotId}&para2=${module}`);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);
        const data = await response.json();

        return data.Table
    } catch (error) {
        console.error(error.message);
        return [];
    }
}

async function transformData(data) {
    const aislesMap = {};
    for (const item of data) {
        const aisleId = item.TenDay.split(" ")[1];
        const shelfId = item.TenKe.split(" ")[1];
        const levelId = item.TenTang.split(" ")[1];

        if (!aislesMap[aisleId]) {
            aislesMap[aisleId] = { id: aisleId, name: item.TenDay, dayID: item.DayID, status: item.Status, description: `Khu vực ${item.TenDay} `, shelves: {} };
        }
        if (!aislesMap[aisleId].shelves[shelfId]) {
            aislesMap[aisleId].shelves[shelfId] = { id: shelfId, name: item.TenKe, keID: item.KeID, textKe: item.TextKe, levels: {} };
        }
        if (!aislesMap[aisleId].shelves[shelfId].levels[levelId]) {
            aislesMap[aisleId].shelves[shelfId].levels[levelId] = { id: levelId, name: item.TenTang, textTang: item.TextTang, slots: [] };
        }

        const items = [];
        aislesMap[aisleId].shelves[shelfId].levels[levelId].slots.push({
            id: item.TenO,
            cbm: item.CBM,
            usedCBM: parseFloat(item.CBM.toFixed(4)) - parseFloat(item.CBMCL.toFixed(4)),
            count: item.CountO,
            cbmCL: item.CBMCL,
            nameO: item.TextO ?? "",
            items: []
        });
    }

    return {
        aisles: Object.values(aislesMap)
            .sort((a, b) => a.id.localeCompare(b.id))
            .map(aisle => ({
                ...aisle,
                shelves: Object.values(aisle.shelves)
                    .sort((a, b) => Number(a.id) - Number(b.id))
                    .map(shelf => ({
                        ...shelf,
                        levels: Object.values(shelf.levels)
                            .sort((a, b) => Number(a.id) - Number(b.id))
                    }))
            }))
    };
}

function getCapacityClass(usedCBM, totalCBM) {
    const percentage = (usedCBM / totalCBM) * 100;
    const remainingPercentage = 100 - percentage;
    if (remainingPercentage === 0) return 'capacity-empty';
    if (remainingPercentage > 80) return 'capacity-high';
    if (remainingPercentage < 50) return 'capacity-low';
    return 'capacity-medium';
}

function getRemainingCapacity(usedCBM, totalCBM) {
    return Math.round(((totalCBM - usedCBM) / totalCBM) * 100);
}

// 2. Sửa lại hàm generateWarehouseHTML()
function generateWarehouseHTML() {
    let html = '';
    warehouseData.aisles.forEach(aisle => {

        const collapseId = `collapse-aisle-${aisle.id}`;
        html += `
        <div class="col-xl-12 col-lg-12 col-12 p-0 m-0 mb-4 ">
            <div class="card aisle-card" data-aisle="${aisle.id}">
                <div class="collapsible-group">
                    <div class="group-header position-relative" data-bs-toggle="collapse" data-bs-target="#${collapseId}" aria-expanded="true">
                        <h4><i class="bi bi-building-fill"></i> ${aisle.name} ${aisle.status == 1 ? "(hàng đạt)" : "(hàng lỗi)"}</h4>
                             <button data-dayid="${aisle.dayID}" data-dayname="${aisle.name}" class="btn btn-sm btn-outline-danger btn_itemXoaDay" data-bs-toggle="modal" >
                            <i class="bi bi-trash3 me-1"></i>Xóa vật tư theo dãy
                          </button>
                        <span class="toggle-icon">▼</span>
                    </div>
                    <div class="collapse show" id="${collapseId}">
                        <div class="card-body">`;

        aisle.shelves.forEach(shelf => {
            html += `
                <div class="shelf-section card mb-3">
                    <div class="card-header bg-secondary text-white py-2 position-relative">
                        <h6 class="mb-0"><i class="bi bi-bookshelf"></i> ${shelf.name} ${shelf.textKe != "" ? `(${shelf.textKe})` : ""}</h6>
                         <button data-keid="${shelf.keID}" data-kename="${shelf.name}" class="btn btn-sm btn-outline-danger btn_itemXoaKe" data-bs-toggle="modal" >
                            <i class="bi bi-trash3 me-1"></i>Xóa vật tư theo kệ
                          </button>
                    </div>
                    <div class="card-body py-2">`;

            const reversedLevels = [...shelf.levels].reverse();
            reversedLevels.forEach(level => {
                html += `
                    <div class="row align-items-center mb-2">
                        <div class="col-auto">
                            <div class="badge level-badge" style="width: 90px;word-wrap: break-word;white-space: normal;">
                                <span style="font-size:12px" class="badge level-badge d-flex">
                                    <i class="bi bi-layers"></i> ${level.name}
                                </span>
                                <div>${level.textTang != "" ? `(${level.textTang})` : ""}</div>
                            </div>
                        </div>
                        <div class="col">
                            <div class="d-flex flex-wrap gap-1">`;

                level.slots.sort((a, b) => {
                    const numA = parseInt(a.id.split('.').pop(), 10);
                    const numB = parseInt(b.id.split('.').pop(), 10);
                    return numA - numB;
                }).forEach(slot => {
                    const slotNumber = slot.id.split('.').pop();
                    const capacityClass = getCapacityClass(slot.usedCBM, slot.cbm);
                    const remainingCapacity = getRemainingCapacity(slot.usedCBM, slot.cbm);
                    const usedPercentage = Math.round(((slot.usedCBM / slot.cbm) * 100));
                    const itemCount = slot.count;
                    const cbmCL = parseFloat((parseFloat(slot.cbm.toFixed(4)) - parseFloat(slot.usedCBM.toFixed(4))).toFixed(4));

                    html += `
                        <div class="position-relative">
                            <div style="background:white !important" class="slot-item ${capacityClass}"
                                data-id="${slot.id}"
                                data-aisle="${aisle.id}"
                                data-shelf="${shelf.id}"
                                data-level="${level.id}"
                                data-cbm="${slot.cbm}"
                                data-used-cbm="${slot.usedCBM}"
                                data-remaining="${remainingCapacity}"
                                data-textName="${slot.nameO}"
                                data-cbmcl="${cbmCL}"
                                data-levelname="${level.name} ${level.textTang != "" ? `(${level.textTang})` : ""}">
                                
                                <div class="slot-used-section" style="height: ${usedPercentage}%"></div>
                                <div class="slot-divider-line" style="bottom: ${usedPercentage}%"></div>
                                
                                <div class="slot-content">
                                    <div class="slot-number">${slotNumber}</div>
                                    <div class="slot-capacity text-center">${parseFloat(slot.usedCBM.toFixed(4))} / ${parseFloat(slot.cbm.toFixed(4))} CBM</div>
                                    <div class="slot-capacity text-center">${cbmCL} CBMCL</div>
                                </div>
                                ${itemCount > 0 ? `<div class="item-count">${itemCount}</div>` : ''}
                            </div>
                            <div data-id="${slot.id}" data-cbmcl="${cbmCL}" class="item-add" style="display:none">
                                <i class="fa-solid fa-plus"></i>
                            </div>
                        </div>`;
                });

                html += `
                            </div>
                        </div>
                    </div>`;
            });

            html += `
                    </div>
                </div>`;
        });

        html += `
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
    });
    return html;
}

// 3. Thêm JavaScript để xử lý toggle icon
document.addEventListener('DOMContentLoaded', function () {
    // Xử lý khi collapse được toggle
    document.querySelectorAll('[data-bs-toggle="collapse"]').forEach(element => {
        element.addEventListener('click', function () {
            const icon = this.querySelector('.toggle-icon');
            const target = document.querySelector(this.getAttribute('data-bs-target'));

            // Toggle icon khi click
            setTimeout(() => {
                if (target.classList.contains('show')) {
                    icon.textContent = '▲';
                } else {
                    icon.textContent = '▼';
                }
            }, 10);
        });
    });
});
function collapseAllAisles() {
    $('.aisle-collapse').collapse('hide');
}

function expandAllAisles() {
    $('.aisle-collapse').collapse('show');
}
// Khởi tạo sự kiện thu gọn/mở rộng
function initAisleCollapse() {
    // Xử lý khi click vào nút toggle
    $(document).on('click', '.toggle-aisle-btn', function () {
        const $btn = $(this);
        const $icon = $btn.find('i');
        const targetId = $btn.data('bs-target');
        const $collapse = $(targetId);

        // Đợi animation hoàn thành rồi mới đổi icon
        $collapse.on('shown.bs.collapse', function () {
            $icon.removeClass('bi-chevron-down').addClass('bi-chevron-up');
        });

        $collapse.on('hidden.bs.collapse', function () {
            $icon.removeClass('bi-chevron-up').addClass('bi-chevron-down');
        });
    });

    // Khởi tạo tất cả collapse với transition mượt
    $('.aisle-collapse').each(function () {
        const $collapse = $(this);

        // Thêm transition mượt mà
        $collapse.on('show.bs.collapse', function () {
            $(this).css('transition', 'height 0.2s ease-in-out');
        });

        $collapse.on('hide.bs.collapse', function () {
            $(this).css('transition', 'height 0.6s ease-in-out');
        });
    });
}
function updateStats() {
    let total = 0, capacityEmpty = 0, capacityHigh = 0;
    $(".slot-item").each(function () {
        const aisle = $(this).data("aisle");
        if (currentFilter === 'all' || aisle === currentFilter) {
            const $aisleCard = $(this).closest('.aisle-card');
            if ($aisleCard.length && !$aisleCard.hasClass("filtered-out")) {
                total++;
                if ($(this).hasClass("capacity-empty")) capacityEmpty++;
                if ($(this).hasClass("capacity-high")) capacityHigh++;
            }
        }
    });
    const warningRate = total > 0 ? Math.round(((capacityEmpty + $(".capacity-low").length) / total) * 100) : 0;
    $("#totalSlots").text(total);
    $("#availableSlots").text(capacityHigh);
    $("#occupiedSlots").text(capacityEmpty);
    $("#utilizationRate").text(warningRate + "%");
}

function applyFilter(filter) {
    currentFilter = filter;
    $(".aisle-card").removeClass("filtered-out").each(function () {
        if (filter !== 'all' && $(this).data("aisle") !== filter) {
            $(this).addClass("filtered-out");
        }
    });
    updateStats();
}

function searchSlots(term) {
    $(".slot-item").removeClass("highlighted");
    if (!term) return;
    term = term.toLowerCase();
    let firstMatch = null;
    $(".slot-item").each(function () {
        const slotId = ($(this).data("id") || "").toLowerCase();
        const aisleName = $(this).closest(".aisle-card").find(".card-header h5").text().toLowerCase();
        const shelfName = $(this).closest(".shelf-section").find(".card-header h6").text().toLowerCase();
        const levelName = $(this).closest(".row").find(".level-badge").text().toLowerCase();
        if (slotId.includes(term) || aisleName.includes(term) || shelfName.includes(term) || levelName.includes(term)) {
            $(this).addClass("highlighted");
            if (!firstMatch) {
                firstMatch = this;
                this.scrollIntoView({ behavior: "smooth", block: "center" });
            }
        }
    });
}

function showTooltip(e, $slot) {
    const $tooltip = $("#tooltip");
    if (!$tooltip.length) return;

    const slotId = $slot.data("id");
    const cbm = parseFloat($slot.data("cbm"));
    const usedCBM = parseFloat($slot.data("used-cbm"));
    const cbmCL = $slot.data("cbmcl")
    const remaining = cbm - usedCBM;
    const remainingPercentage = $slot.data("remaining");
    const nameO = $slot.data("textname");

    const aisleName = $slot.closest(".aisle-card").find(".card-header h5").text() || "N/A";
    const shelfName = $slot.closest(".shelf-section").find(".card-header h6").text() || "N/A";
    const levelName = $slot.data("levelname");

    const items = $slot.data("items") || [];

    let capacityStatus = "";
    if ($slot.hasClass("capacity-empty")) capacityStatus = "Đã đầy (0%)";
    else if ($slot.hasClass("capacity-high")) capacityStatus = `Còn nhiều chỗ(${remainingPercentage} %)`;
    else if ($slot.hasClass("capacity-medium")) capacityStatus = `Còn vừa phải(${remainingPercentage} %)`;
    else if ($slot.hasClass("capacity-low")) capacityStatus = `Sắp đầy(${remainingPercentage} %)`;

    $tooltip.html(`
    <div> <strong><i class="bi bi-geo-alt"></i> ${slotId}</strong></div>
        <div><i class="bi bi-stack"></i> ${nameO}</div>
        <div><i class="bi bi-building"></i> ${aisleName}</div>
        <div><i class="bi bi-bookshelf"></i> ${shelfName}</div>
        <div><i class="bi bi-layers"></i> ${levelName}</div>
        <div><i class="bi bi-box"></i> CBM: ${parseFloat(usedCBM.toFixed(4))}/${parseFloat(cbm).toFixed(4)} (Còn: ${cbmCL})</div>
        <div><i class="bi bi-speedometer2"></i> ${capacityStatus}</div>
    `).css({ left: e.pageX + 15, top: e.pageY - 10, opacity: 1 });
}

function hideTooltip() {
    $("#tooltip").css("opacity", 0);
}

// Events
$(document)
    .on("click", "label[data-filter]", function () {
        applyFilter($(this).data("filter"));
    })
    .on("mouseenter", ".slot-item", function (e) {
        showTooltip(e, $(this));
    })
    .on("mouseleave", ".slot-item", function () {
        hideTooltip();
    })
    .on("mousemove", ".slot-item", function (e) {
        $("#tooltip").css({ left: e.pageX + 15, top: e.pageY - 10 });
    })
    .on("click", ".item-add", function (e) {
        e.preventDefault()
        const slotId = $(this).data("id");
        SaveTableONPL(slotId)
    })
    .on("click", ".slot-item", function () {
        $(".slot-item").removeClass("highlighted");
        $(this).addClass("highlighted");

        const slotId = $(this).data("id");
        showSlotDetail(slotId);

        setTimeout(() => {
            $(this).removeClass("highlighted");
        }, 5000);
    })
    .on("click", 'body', function (e) {
        if (!$(e.target).closest('.position-relative').length) {
            setTimeout(() => {
                $(".slot-item").removeClass("highlighted");
            }, 5000)
        }
    });

$(document).on("keydown", function (e) {
    if (e.key === "Escape") {
        $("#searchInput").val("");
        $(".slot-item").removeClass("highlighted");
    }
});

function renderFilters() {
    const $filterGroup = $("#filterGroup");
    if (!$filterGroup.length) return;
    let html = `
    <div>
    <input type="radio" class="btn-check" name="filter" id="filter-all" checked>
        <label class="btn btn-outline-primary" for="filter-all" data-filter="all">Tất cả</label>
        </div>`;
    warehouseData.aisles.forEach(aisle => {
        html += `<div>
    <input type="radio" class="btn-check" name="filter" id="filter-${aisle.id}">
        <label class="btn btn-outline-primary" for="filter-${aisle.id}" data-filter="${aisle.id}">${aisle.name}</label>
        </div>`;
    });
    $filterGroup.html(html);
}

async function showSlotDetail(slotId) {
    const modal = new bootstrap.Modal($('#itemDetailModal')[0]);
    modal.show();
    solotIDBarCode = slotId
    await loadVTKho()
}
function toggleReturnMode(showReturn) {
    if (showReturn) {
        // Hiện chức năng trả hàng
        $('.check-all-group').show();
        $('.item-checkbox').show();
        $('#btnReturnItems').show();

        // Hiện cột "Trả hàng" trong header - CHỈ TRONG MODAL
        $('#modalItemList thead tr td:first-child').show();
        $('#modalItemList tbody tr td:first-child').show();

        // Thay đổi placeholder
        $('#barcodeInput').attr('placeholder', 'Quét barcode để đánh dấu trả hàng...');
    } else {
        // Ẩn chức năng trả hàng
        $('.check-all-group').hide();
        $('.item-checkbox').hide();
        $('#btnReturnItems').hide();

        // Ẩn cột "Trả hàng" trong header - CHỈ TRONG MODAL
        $('#modalItemList thead tr td:first-child').hide();
        $('#modalItemList tbody tr td:first-child').hide();

        // Thay đổi placeholder
        $('#barcodeInput').attr('placeholder', 'Quét barcode để nhập kho...');

        // Bỏ check tất cả khi chuyển sang import mode
        $('.item-checkbox').prop('checked', false);
        $('.check-all-group').prop('checked', false).prop('indeterminate', false);
        updateSelectedCount();
    }
}
// Hàm lấy chế độ scan hiện tại
function getScanMode() {
    return $('input[name="scanMode"]:checked').val();
}

// Setup event listener cho radio buttons
function setupScanModeListener() {
    $('input[name="scanMode"]').on('change', function () {
        const mode = $(this).val();

        if (mode === 'import') {
            toggleReturnMode(false);
        } else if (mode === 'return') {
            toggleReturnMode(true);
        }
    });
}
async function UpdateTraHang(arrSave, value) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    const request = new Request(`/api/ERPVatTuCBM/Post?action=UpdateReturnVT&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        if (value != 1) {
            loadVTKho()
            showToast('success', `Trả vật tư thành công!`, 1500);
        } else
            showToast('success', `Hủy trả vật tư thành công!`, 1500);
        GetCTietNKViTriKho()
        GetViTriKho()
        GetDanhSachO()
        if (value != 1)
            GetDanhSachItemReturn()

    }
    else {
        showToast('error', `Trả vật tư thất bại!`, 2000);
        return
    }
}
async function loadVTKho() {
    var slotId = solotIDBarCode;
    let datahtml = await fetchSlotItems(slotId);

    $('#modalSlotTitle').html(`
        <i class="bi bi-box"></i> Chi tiết ô kho ${slotId}
    `);


    let itemsHtml = "";
    if (datahtml?.length > 0) {
        succhuaOCL = parseFloat(datahtml[0].CBMO) - parseFloat(datahtml[0].CBMSD);

        const capacityClass = getCapacityClass(datahtml[0].CBMSD, datahtml[0].CBMO);
        let capacityColor = 'success';
        if (capacityClass === 'capacity-empty') capacityColor = 'danger';
        else if (capacityClass === 'capacity-low') capacityColor = 'warning';
        else if (capacityClass === 'capacity-medium') capacityColor = 'info';

        $('#modalCapacityInfo').html(`
        <div class="fs-5 fw-bold text-${capacityColor}">${parseFloat(datahtml[0].CBMSD.toFixed(4))}/${parseFloat(datahtml[0].CBMO.toFixed(4))} CBM</div>
    `);
    }
    $('#modalSlotInfo').html(`
        <div class="fs-5 fw-bold text-primary">${slotId} / 0 Vật Tư</div>
    `);

    if (datahtml?.length > 0 && datahtml[0].DisplayGroup != null) {
        if (datahtml[0].Status == 2)
            $(".item-trahang").show()
        else $(".item-trahang").hide()
        $('#modalSlotInfo').html(`
        <div class="fs-5 fw-bold text-primary">${slotId} / ${datahtml?.length} Vật Tư</div>
    `);

        const groupedData = datahtml.reduce((acc, item) => {
            const maVT = item.DisplayGroup;
            if (!acc[maVT]) {
                acc[maVT] = {
                    items: [],
                    soLo: item.SoLo,
                    poMua: item.POMua,
                    totalCBM: 0,
                    totalSoKien: 0
                };
            }
            acc[maVT].items.push(item);
            acc[maVT].totalCBM += parseFloat(item.CBM || 0);
            acc[maVT].totalSoKien += 1;
            return acc;
        }, {});

        // Lưu groupedData vào biến global để search
        window.currentGroupedData = groupedData;

        itemsHtml = renderGroupedItems(groupedData);
    } else {
        itemsHtml = `
        <div class="text-center text-muted py-4">
            <i class="bi bi-inbox fs-1"></i>
            <div class="mt-2">Ô kho trống</div>
        </div>`;
    }

    $('#modalItemList').html(itemsHtml);

    // Setup search functionality
    setupSearchFunctionality();

    // Setup checkbox functionality
    setupCheckboxFunctionality();

    // Cập nhật số lượng ban đầu
    updateSelectedCount();

    setupScanModeListener();

    const currentMode = getScanMode();
    toggleReturnMode(currentMode === 'return');
}
function findAndCheckBarcode(barcode) {
    // Tìm row có barcode tương ứng
    const $row = $(`#modalItemList tr[data-barcode="${barcode}"]`);

    if ($row.length === 0) {
        showToast('warning', `Không tìm thấy vật tư với barcode: ${barcode} !!`);
        PlayAudioError();
        return false;
    }

    // Tìm checkbox trong row
    const $checkbox = $row.find('.item-checkbox');

    if ($checkbox.length === 0) {
        console.error('Không tìm thấy checkbox trong row');
        return false;
    }

    // Tìm collapse group chứa row này và MỞ NÓ RA
    const $collapseGroup = $row.closest('.collapse');
    if ($collapseGroup.length > 0) {
        // Mở group nếu đang đóng
        if (!$collapseGroup.hasClass('show')) {
            $collapseGroup.addClass('show');
        }
    }

    // Toggle checkbox (nếu đã check thì bỏ, chưa check thì check)
    const isCurrentlyChecked = $checkbox.prop('checked');
    $checkbox.prop('checked', true);

    // Trigger change event để cập nhật trạng thái check-all
    $checkbox.trigger('change');

    // ĐỢI 1 chút để collapse mở hoàn toàn, rồi mới scroll và highlight
    setTimeout(() => {
        scrollToItem($row);

        // Highlight row
        highlightRow($row);
    }, 100); // Đợi 100ms để collapse animation hoàn thành

    // Hiển thị thông báo
    const action = !isCurrentlyChecked ? 'đã chọn' : 'đã bỏ chọn';
    showToast('success', `Barcode ${action} !!`);
    PlayAudio();

    return true;
}
// Khởi tạo grid trả hàng (gọi 1 lần khi load trang)
function initReturnHistoryGrid() {
    $("#dxLichSuTraHang").dxDataGrid({
        dataSource: returnHistory,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
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
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "POMua",
                caption: "PO Mua",
                width: 100,
            },
            {
                caption: "LOT/Batch",
                dataField: "LOTBATCH",
                width: 120,
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                width: 90,
            },
            {
                dataField: "SoKienHienThi",
                caption: "Vật tư",
                minWidth: 80,
                width: 140,
            },
            {
                dataField: "CBM",
                caption: "CBM",
                width: 80,
                calculateCellValue: function (row) {
                    return parseFloat(parseFloat(row.CBM || 0).toFixed(2));
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
            }
        },
    });
    $("#Layer_1").click();
}

function addToReturnHistory(itemInfo) {
    // Thêm timestamp
    itemInfo.ScanTime = new Date().toLocaleString('vi-VN');

    // Thêm vào đầu mảng
    returnHistory.unshift(itemInfo);

    // Cập nhật grid
    const grid = $("#dxLichSuTraHang").dxDataGrid("instance");
    if (grid) {
        grid.option("dataSource", returnHistory);
        grid.refresh();
    }
}
function setTextAndTitle2(selector, value) {
    $(selector).text(value ?? "").attr("title", value ?? "");
}
async function findAndCheckBarcode2(barcode) {
    const $row = $(`#modalItemList tr[data-barcode="${barcode}"]`);
    if ($row.length === 0) {
        showToast('warning', `Không tìm thấy vật tư với barcode: ${barcode} !!`);
        PlayAudioError();
        return false;
    }

    const $checkbox = $row.find('.item-checkbox');

    setTextAndTitle2("#txtPoMua", $checkbox.data('mavt'));
    setTextAndTitle2("#txtLotBatch", $checkbox.data('solot') + '/' + $checkbox.data('batch'));
    setTextAndTitle2("#txtItemCode", $checkbox.data('mavt'));
    setTextAndTitle2("#txtRoll", $checkbox.data('sokien'));
    setTextAndTitle2("#txtCBM", $checkbox.data('cbm'));
    const itemInfo = {
        Barcode: barcode,
        POMua: $checkbox.data('mavt'),
        LOTBATCH: $checkbox.data('solot') + '/' + $checkbox.data('batch'),
        MaVT: $checkbox.data('mavt'),
        SoKienHienThi: $checkbox.data('sokien'),
        CBM: $checkbox.data('cbm')
    }
    addToReturnHistory(itemInfo);

    // Tìm collapse group chứa row này và MỞ NÓ RA
    const $collapseGroup = $row.closest('.collapse');
    if ($collapseGroup.length > 0) {
        // Mở group nếu đang đóng
        if (!$collapseGroup.hasClass('show')) {
            $collapseGroup.addClass('show');
        }
    }

    // Toggle checkbox (nếu đã check thì bỏ, chưa check thì check)
    const isCurrentlyChecked = $checkbox.prop('checked');
    $checkbox.prop('checked', true);

    // Trigger change event để cập nhật trạng thái check-all
    $checkbox.trigger('change');

    // ĐỢI 1 chút để collapse mở hoàn toàn, rồi mới scroll và highlight
    setTimeout(() => {
        scrollToItem($row);

        // Highlight row
        highlightRow($row);
    }, 100); // Đợi 100ms để collapse animation hoàn thành
    PlayAudio();

}
// Hàm scroll đến item
function scrollToItem($row) {
    const $modalItemList = $('#modalItemList');

    // Kiểm tra xem row có visible không
    if (!$row.is(':visible')) {
        console.error('Row không visible');
        return;
    }

    // Lấy vị trí của row so với container
    const rowOffsetTop = $row.offset().top;
    const containerOffsetTop = $modalItemList.offset().top;
    const currentScroll = $modalItemList.scrollTop();

    // Tính toán vị trí scroll mới
    const rowRelativeTop = rowOffsetTop - containerOffsetTop + currentScroll;
    const containerHeight = $modalItemList.height();
    const rowHeight = $row.outerHeight();

    // Scroll để row ở giữa container
    const scrollTo = rowRelativeTop - (containerHeight / 2) + (rowHeight / 2);

    $modalItemList.animate({
        scrollTop: scrollTo
    }, 500);
}

// Hàm highlight row
function highlightRow($row) {
    if (!$row.is(':visible')) {
        console.error('Row không visible để highlight');
        return;
    }

    $('#modalItemList tr').removeClass('highlight-row');

    $row.addClass('highlight-row');


}
function renderGroupedItems(groupedData) {
    return Object.entries(groupedData).map(([maVT, group], groupIndex) => {
        const groupId = `group-${groupIndex}`;
        const itemsRows = group.items.map((item, itemIndex) => `
            <tr data-barcode="${item.BarCode}">
                <td class="text-center">
                    <input type="checkbox" class="form-check-input item-checkbox" 
                           data-mavt="${maVT}" 
                           data-group-index="${groupIndex}"
                           data-item-index="${itemIndex}"
                           data-sokien="${item.SoKien}"
                           data-solot="${item.SoLoT}"
                           data-batch="${item.Batch}"
                           data-soluong="${item.SoLuongThucTe}"
                           data-cbm="${item.CBM}"
                           id="check-${groupIndex}-${itemIndex}">
                </td>
                <td class="text-center">${item.SoKien}</td>
                <td class="text-center">${item.SoLoT}</td>
                <td class="text-center">${item.Batch}</td>
                <td class="text-center">${item.SoLuongThucTe}</td>
                <td class="text-center">${item.CBM}</td>
            </tr>
        `).join('');

        return `
        <div class="collapsible-group collapsible-itemcode mb-3" 
             data-mavt="${maVT}" 
             data-solo="${group.soLo}" 
             data-pomua="${group.poMua}">
            <div class="group-header d-flex justify-content-between align-items-center"
                 style="background:linear-gradient(45deg,#3B82F6, #9c71ffcc); padding: 12px; border-radius: 0px; cursor: pointer;"
                 data-bs-toggle="collapse"
                 data-bs-target="#${groupId}"
                 role="button"
                 aria-expanded="true">
                <div>
                    <h6 class="mb-1" style="font-size: 14px; color: white;">Mã VT: ${maVT} - Số Lô: ${group.soLo} - PO mua: ${group.poMua}</h6>
                    <small style="color: #fff500; font-weight: bold">CBM: ${group.totalCBM.toFixed(4)} CBM</small>
                    -
                    <small style="color: white;">Tổng vật tư: ${group.totalSoKien}</small>
                </div>
                <span class="toggle-icon" style="color: white;">▼</span>
            </div>
            
            <div class="collapse show" id="${groupId}">
                <div class="table-responsive mt-2 px-1">
                    <table class="table table-bordered table-hover table-sm">
                        <thead class="table-light">
                            <tr>
                                <td class="text-center" style="width: 80px;">
                                    <div class="d-flex flex-column align-items-center">
                                        <input type="checkbox"
                                               class="form-check-input check-all-group mb-1"
                                               data-group-index="${groupIndex}"
                                               id="checkAll-${groupIndex}"
                                               title="Chọn tất cả">
                                        <small style="font-size: 10px;">Trả vật tư</small>
                                    </div>
                                </td>
                                <td class="text-center">Vật tư</td>
                                <td class="text-center">Số LOT</td>
                                <td class="text-center">Batch</td>
                                <td class="text-center">Số lượng</td>
                                <td class="text-center" style="width: 100px;">CBM</td>
                            </tr>
                        </thead>
                        <tbody>
                            ${itemsRows}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        `;
    }).join('');
}

function setupCheckboxFunctionality() {
    $(document).off('change', '.check-all-group').on('change', '.check-all-group', function () {
        const groupIndex = $(this).data('group-index');
        const isChecked = $(this).prop('checked');

        $(`.item-checkbox[data-group-index="${groupIndex}"]`).prop('checked', isChecked);

        updateSelectedCount();

        const count = $(`.item-checkbox[data-group-index="${groupIndex}"]`).length;
        const action = isChecked ? 'chọn' : 'bỏ chọn';
    });

    $(document).off('change', '.item-checkbox').on('change', '.item-checkbox', function () {
        const groupIndex = $(this).data('group-index');
        const totalInGroup = $(`.item-checkbox[data-group-index="${groupIndex}"]`).length;
        const checkedInGroup = $(`.item-checkbox[data-group-index="${groupIndex}"]:checked`).length;

        const checkAllBox = $(`#checkAll-${groupIndex}`);

        if (checkedInGroup === 0) {
            checkAllBox.prop('checked', false);
            checkAllBox.prop('indeterminate', false);
        } else if (checkedInGroup === totalInGroup) {
            checkAllBox.prop('checked', true);
            checkAllBox.prop('indeterminate', false);
        } else {
            checkAllBox.prop('checked', false);
            checkAllBox.prop('indeterminate', true);
        }

        updateSelectedCount();
    });

    $(document).off('click', '#btnReturnItems').on('click', '#btnReturnItems', function () {
        const returnedItems = getReturnedItems();
        UpdateTraHang(returnedItems)
    });
}

// Hàm cập nhật số lượng đã chọn
function updateSelectedCount() {
    const count = $('.item-checkbox:checked').length;

    // Cập nhật text trên nút
    if (count > 0) {
        $('#btnReturnItems').html(`
            <i class="bi bi-box-arrow-up"></i> Trả vật tư (${count})
        `);
        $('#btnReturnItems').prop('disabled', false);
    } else {
        $('#btnReturnItems').html(`
            <i class="bi bi-box-arrow-up"></i> Trả vật tư
        `);
        $('#btnReturnItems').prop('disabled', true);
    }
}

// Hàm lấy danh sách items đã chọn trả hàng
function getReturnedItems() {
    const returnedItems = [];

    $('.item-checkbox:checked').each(function () {
        const $checkbox = $(this);
        returnedItems.push({
            ID: 0,
            SoLoID: 0,
            MaNPL: 0,
            MaVTGhep: 0,
            BarCode: $checkbox.closest("tr").data('barcode'),
            Dai: 0,
            Rong: 0,
            Cao: 0,
            DK: 0,
            CBM: 0,
            IsNPL: 0,
            IsCBM: 0,
            MaONPL: "",
            Status: 2,
        });
    });

    return returnedItems;
}

// Hàm xử lý trả hàng
async function processReturnItems(returnedItems) {

}

// Hàm setup tìm kiếm
function setupSearchFunctionality() {
    const searchInput = $('#searchItemInput');
    const clearBtn = $('#clearSearchBtn');

    // Tìm kiếm khi gõ
    searchInput.off('input').on('input', function () {
        const searchTerm = $(this).val().toLowerCase().trim();
        filterItems(searchTerm);
    });

    // Xóa tìm kiếm
    clearBtn.off('click').on('click', function () {
        searchInput.val('');
        filterItems('');
        searchInput.focus();
    });

    // Enter để tìm
    searchInput.off('keypress').on('keypress', function (e) {
        if (e.which === 13) {
            const searchTerm = $(this).val().toLowerCase().trim();
            filterItems(searchTerm);
        }
    });
}

// Hàm lọc items
function filterItems(searchTerm) {
    const groups = $('.collapsible-itemcode');
    let visibleCount = 0;

    if (!searchTerm) {
        // Hiển thị tất cả
        groups.show();
        visibleCount = groups.length;
    } else {
        // Lọc theo từ khóa
        groups.each(function () {
            const $group = $(this);
            const maVT = $group.data('mavt')?.toString().toLowerCase() || '';
            const soLo = $group.data('solo')?.toString().toLowerCase() || '';
            const poMua = $group.data('pomua')?.toString().toLowerCase() || '';

            const isMatch = maVT.includes(searchTerm) ||
                soLo.includes(searchTerm) ||
                poMua.includes(searchTerm);

            if (isMatch) {
                $group.show();
                // Tự động mở collapse khi tìm thấy
                $group.find('.collapse').addClass('show');
                visibleCount++;
            } else {
                $group.hide();
            }
        });
    }
}



function calculateCBM() {
    const type = $('input[name="typeCBM"]:checked').val();
    let cbm = 0;

    if (type === 'rect') {
        const l = parseFloat($('.input-length').val()) || 0;
        const w = parseFloat($('.input-width').val()) || 0;
        const h = parseFloat($('.input-height').val()) || 0;
        cbm = l * w * h;
    } else {
        const length = parseFloat($('.input-cylinder-length').val()) || 0;
        const diameter = parseFloat($('.input-cylinder-diameter').val()) || 0;
        const radius = diameter / 2;
        cbm = Math.PI * Math.pow(radius, 2) * length;
    }

    $('#cbmResult').text(cbm.toFixed(4));
    return cbm;
}

$(function () {
    $('input[name="typeCBM"]').on('change', function () {
        if (this.value === 'rect') {
            $('.cell-rect').removeClass('d-none');
            $('#headerRect').removeClass('d-none');
            $('.cell-cylinder').addClass('d-none');
            $('#headerCylinder').addClass('d-none');
        } else {
            $('.cell-rect').addClass('d-none');
            $('#headerRect').addClass('d-none');
            $('.cell-cylinder').removeClass('d-none');
            $('#headerCylinder').removeClass('d-none');
        }
        $('#cbmResult').text(0.0);
        $(".cell-rect .cbm-input").val("")
        $(".cell-cylinder .cbm-input").val("")
    });

    $(document).on('input', '.cbm-input', calculateCBM);

    $('#btnSaveCBM').on('click', function () {
        const cbmValue = calculateCBM();
        const type = $('input[name="typeCBM"]:checked').val();

        let dai = type === 'rect' ? parseFloat($('.input-length').val()) || 0 : parseFloat($('.input-cylinder-length').val()) || 0;
        let rong = parseFloat($('.input-width').val()) || 0;
        let cao = parseFloat($('.input-height').val()) || 0;
        let duongkinh = parseFloat($('.input-cylinder-diameter').val()) || 0;

        $("#tbody tr").each(function () {
            const $trRow = $(this)
            var checkbarCode = $trRow.find("input.checkCBM").is(":checked");

            if (!checkbarCode) return;

            if (checkNhap == 1 && !$trRow.is(thisTr)) return;

            $trRow.find("input.inputSLN").val(cbmValue.toFixed(4))
            $trRow.attr("data-dai", dai)
            $trRow.attr("data-rong", rong)
            $trRow.attr("data-cao", cao)
            $trRow.attr("data-duongkinh", duongkinh)
            $trRow.attr("data-cbm", cbmValue.toFixed(4))
            $trRow.attr("data-typecbm", type === 'rect' ? 1 : 2)
        })
        SaveTableCBM(cbmValue)
    });

    $("#tbody").on("click", '.editcbm', function () {
        const trRow = $(this).closest("tr");
        $(".cell-rect .cbm-input").val("")
        $(".cell-cylinder .cbm-input").val("")
        thisTr = trRow
        checkNhap = 1
        const isCBM = trRow.data("typecbm")
        const dai = trRow.data("dai")
        const rong = trRow.data("rong")
        const cao = trRow.data("cao")
        const dk = trRow.data("duongkinh")
        const cbm = trRow.data("cbm")

        if (isCBM == "2") {
            $(`input[name="typeCBM"][value="cylinder"]`).trigger("click");
            $(".input-cylinder-length").val(dai)
            $(".input-cylinder-diameter").val(dk)
            $('#cbmResult').text(cbm);
        }
        else {
            $(`input[name="typeCBM"][value="rect"]`).trigger("click");
            $(".input-length").val(dai)
            $(".input-width").val(rong)
            $(".input-height").val(cao)
            $('#cbmResult').text(cbm);
        }

        $(this).closest("tr").find(".checkCBM").prop("checked", true);
        $("#cbmModal").modal("show")
    })

    $("#tbody").on("click", '.checkNK', function () {
        let isChecked = $(this).is(":checked");

        const $trRow = $(this).closest("tr")
        if (isChecked) {
            const checkVal = $trRow.find("input.inputSLN").val()
            if (checkVal == "" || checkVal == 0) {
                showToast('warning', 'Vui lòng nhập CBM của vật tư!');
                $(this).prop("checked", false);
            }
        }
        sumKienCBM();
    })

    $(".btnNhapCBM").on("click", function () {
        checkNhap = 2
        $("#cbmModal").modal("show")
    })
})

function showToast(type, message, delay = 2000) {
    let toastId, messageId;

    switch (type) {
        case 'success':
            toastId = 'successToast';
            messageId = 'successMessage';
            break;
        case 'error':
            toastId = 'errorToast';
            messageId = 'errorMessage';
            break;
        case 'warning':
            toastId = 'warningToast';
            messageId = 'warningMessage';
            break;
        default:
            console.error('Unknown toast type:', type);
            return;
    }

    $('#' + messageId).text(message);

    const toastElement = $('#' + toastId)[0];
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: delay
    });

    toast.show();
}

async function SaveTableCBM(cbm) {
    let arrSave = [];
    let rows = $("#tbody tr").toArray();

    for (const row of rows) {
        const $row = $(row);
        let $checkbox = $row.find(".checkCBM");
        if (!$checkbox.length) continue;
        const isChecked = $row.find('.checkCBM').is(':checked');

        if (!isChecked) continue;

        if (checkNhap == 1 && !$row.is(thisTr)) continue;

        let object = await handleListCBM($(row), cbm);
        arrSave.push(object);
    }
    await SaveCBM(arrSave);
}

async function SaveCBM(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    const request = new Request(`/api/ERPVatTuCBM/Post?action=PostCBM&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        showToast('success', `Lưu CBM thành công!`, 1500);
        $('#cbmModal').modal('hide');
        if (checkNhap == 1) {
            renderCheckFalse()
        } else
            $("#tbody").find(".checkCBM").prop("checked", false);
        $("#tbody").find("#checkAllCBM ").prop("checked", false)
        $('#cbmResult').text(0.00);
        $(".cell-rect .cbm-input").val("")
        $(".cell-cylinder .cbm-input").val("")
    }
    else {
        showToast('error', `Lưu CBM thất bại!`, 2000);
        return
    }
}

async function handleListCBM($this, cbm) {
    let SoLo = $this.data("soloid");
    let npl = $this.data("npl");
    let mavtG = $this.data("mavtghep");
    let barcode = $this.data("barcode");

    const type = $('input[name="typeCBM"]:checked').val();

    let dai = type === 'rect' ? parseFloat($('.input-length').val()) || 0 : parseFloat($('.input-cylinder-length').val()) || 0;
    let rong = parseFloat($('.input-width').val()) || 0;
    let cao = parseFloat($('.input-height').val()) || 0;
    let duongkinh = parseFloat($('.input-cylinder-diameter').val()) || 0;

    let CBM = cbm;

    let isnpl = $("#nguyenphulieuid").val();
    let object = {
        ID: 0,
        SoLoID: SoLo,
        MaNPL: npl,
        MaVTGhep: mavtG,
        BarCode: barcode,
        Dai: dai,
        Rong: rong,
        Cao: cao,
        DK: duongkinh,
        CBM: CBM,
        IsNPL: isnpl,
        IsCBM: type === 'rect' ? 1 : 2,
        MaONPL: "",
    };

    return object;
}

function renderCheckFalse() {
    if (checkNhap == 1)
        thisTr.find(".checkCBM").prop("checked", false);
}

function sumKienCBM() {
    let cbm = 0;
    let length = 0
    $("#tbody tr").each(function () {
        const $trRow = $(this);
        const check = $trRow.find("input.checkNK").is(":checked");
        if (!check) return;

        const val = parseFloat($trRow.find("input.inputSLN").val()) || 0;
        cbm += val;
        length++;
    });

    cbm = cbm.toFixed(4);
    $(".tongCBM").text(cbm)
    $(".slot-item").removeClass("highlighted");
    $(".item-add").each(function () {
        const cbmcl = parseFloat($(this).data("cbmcl"));
        const tongCBM = parseFloat($(".tongCBM").text()).toFixed(4)
        if (cbmcl >= tongCBM && length > 0) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

async function SaveTableONPL(oid) {
    let arrSave = [];
    let rows = $("#tbody tr").toArray();

    for (const row of rows) {
        const $row = $(row);
        let $checkbox = $row.find(".checkNK");
        if (!$checkbox.length) continue;
        const isChecked = $row.find('.checkNK').is(':checked');

        if (!isChecked) continue;

        let object = await handleListONPL($(row), oid);
        arrSave.push(object);
    }

    await SaveONPL(arrSave);
}

async function handleListONPL($this, oid) {
    let SoLo = $this.data("soloid");
    let npl = $this.data("npl");
    let mavtG = $this.data("mavtghep");
    let barcode = $this.data("barcode");

    let isnpl = $("#nguyenphulieuid").val();
    let object = {
        ID: 0,
        SoLoID: SoLo,
        MaNPL: npl,
        MaVTGhep: mavtG,
        BarCode: barcode,
        Dai: 0,
        Rong: 0,
        Cao: 0,
        DK: 0,
        CBM: 0,
        IsNPL: isnpl,
        IsCBM: 0,
        MaONPL: oid,
    };

    return object;
}

async function SaveONPL(arrSave, checkLoad = true) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    const request = new Request(`/api/ERPVatTuCBM/Post?action=PostMaONPL&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        showToast('success', `Lưu vào vị trí ô thành công!`, 1500);
        if (checkLoad)
            GetCTietNKViTriKho()
        GetViTriKho()
        GetDanhSachO()
    }
    else {
        showToast('error', `Lưu vào vị trí ô thất bại!`, 2000);
        return
    }
}

function setupGlobalSearch() {
    const $searchInput = $('#globalSearchInput');
    const $searchInputBarcode = $('#globalBarcode');
    const $searchResults = $('#searchResults');
    const $clearBtn = $('#clearSearch');

    let searchTimeout;

    $searchInput.on('input', function (e) {
        const searchTerm = $(this).val().trim();

        clearTimeout(searchTimeout);

        if (searchTerm.length < 1) {
            $("#searchLable").hide()
            $("#searchResults").hide()
            return;
        }

        searchTimeout = setTimeout(() => {
            performGlobalSearch(searchTerm);
        }, 300);
    });

    $searchInputBarcode.on('input', function (e) {
        const searchTerm = $(this).val().trim();

        clearTimeout(searchTimeout);

        if (searchTerm.length < 1) {
            $("#searchLable").hide()
            $("#searchResults").hide()
            return;
        }

        searchTimeout = setTimeout(() => {
            performGlobalSearchBarCode(searchTerm);
        }, 300);
    });

    $clearBtn.on('click', function () {
        $searchInput.val('');
        $searchInputBarcode.val("")
        $searchResults.hide();
        $searchInput.focus();
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.position-relative').length) {
            $("#searchLable").hide()
            $("#searchResults").hide()
        }
    });

    $searchInput.on('focus', function () {
        if ($(this).val().trim().length >= 1) {
            performGlobalSearch($(this).val().trim());
        }
    });

    $searchInputBarcode.on('focus', function () {
        if ($(this).val().trim().length >= 1) {
            performGlobalSearchBarCode($(this).val().trim());
        }
    });
}

function performGlobalSearch(searchTerm) {
    const $searchResults = $('#searchResults');
    const results = [];

    warehouseData.aisles.forEach(aisle => {
        aisle.shelves.forEach(shelf => {
            shelf.levels.forEach(level => {
                level.slots.forEach(slot => {
                    if (slot.id && slot.id.toLowerCase().includes(searchTerm.toLowerCase())) {
                        results.push({
                            slot: slot,
                            location: {
                                aisle: aisle.name,
                                aisleId: aisle.id,
                                shelf: shelf.name,
                                level: level.name,
                                slotId: slot.id
                            }
                        });
                    }
                });
            });
        });
    });

    displaySearchResults(results, searchTerm);
}

function performGlobalSearchBarCode(searchTerm) {
    const $searchResults = $('#searchResults');
    const results = [];

    if (!searchTerm || searchTerm.trim() === "") return;

    const lowerSearch = searchTerm.toLowerCase();
    console.log(warehouseDataChiTiet)
    warehouseDataChiTiet.forEach(row => {
        const match =
            (row.DisplayGroup && row.DisplayGroup.toLowerCase().includes(lowerSearch)) ||
            (row.SoLo && row.SoLo.toLowerCase().includes(lowerSearch)) ||
            (row.SoKien && row.SoKien.toLowerCase().includes(lowerSearch)) ||
            (row.BarCode && row.BarCode.toLowerCase().includes(lowerSearch)) ||
            (row.NgayNhapKho && row.NgayNhapKho.toLowerCase().includes(lowerSearch)) ||
            (row.SoLoT && row.SoLoT.toLowerCase().includes(lowerSearch))
            ;

        if (match) {
            results.push({
                slot: {
                    id: row.MaONPL,
                    cbm: row.CBM,
                    barcode: row.BarCode,
                    displayGroup: row.DisplayGroup,
                    soLo: row.SoLo,
                    soKien: row.SoKien,
                    usedCBM: row.CBMSD,
                    cbmO: row.CBMO,
                    ngayNhapKho: row.NgayNhapKho,
                    soLoT: row.SoLoT,
                    chiTiet: row.ChiTiet,
                    mauVT: row.MauVT,
                    khoVai: row.SoLoT,
                    maVT: row.MaVT
                },
                location: {
                    aisle: row.TenDay,
                    shelf: row.TenKe,
                    level: row.TenTang,
                    slotId: row.MaONPL
                }
            });
        }
    });

    displaySearchResultsBarcode(results, searchTerm);
}

function displaySearchResults(results, searchTerm) {
    const $searchResults = $('#searchResults');
    const $searchLable = $('#searchLable');
    let html = '';
    let htmllable = '';

    if (results.length === 0) {
        html = `
            <div class="list-group-item search-result-item text-center py-3">
                <i class="bi bi-search text-muted fs-1"></i>
                <div class="mt-2 text-muted">Không tìm thấy ô "${searchTerm}"</div>
                <small class="text-muted">Hãy thử từ khóa khác</small>
            </div>`;
    } else {
        const groupedResults = {};

        results.forEach(result => {
            const aisleId = result.location.aisleId;
            if (!groupedResults[aisleId]) {
                groupedResults[aisleId] = {
                    aisleName: result.location.aisle,
                    items: []
                };
            }
            groupedResults[aisleId].items.push(result);
        });

        htmllable += `
            <div class="list-group-item bg-primary text-white py-2 px-3">
                <small><i class="bi bi-search"></i> Tìm thấy ${results.length} ô chứa "${searchTerm}"</small>
            </div>`;

        Object.keys(groupedResults).forEach(aisleId => {
            const group = groupedResults[aisleId];

            html += `
                <div class="list-group-item bg-light py-2 px-3">
                    <small class="fw-bold text-primary">
                        <i class="bi bi-building-fill"></i> ${group.aisleName}
                    </small>
                </div>`;

            group.items.forEach((result, index) => {
                const remainingCapacity = getRemainingCapacity(result.slot.usedCBM, result.slot.cbm);

                let capacityColor = 'success';
                if (remainingCapacity === 0) capacityColor = 'danger';
                else if (remainingCapacity < 30) capacityColor = 'warning';
                else if (remainingCapacity < 70) capacityColor = 'info';

                if (index % 2 === 0) html += `<div class="row pt-2 pb-2">`;

                html += `
                    <div class="col-6 col-lg-6">
                        <div class="list-group-item search-result-item py-3 px-3">
                           <div class="row w-100">
                              <div class="col-6">
                                 <div class="fw-bold text-primary mb-1">
                                <i class="bi bi-box"></i> ${highlightSearchTerm(result.location.slotId, searchTerm)}
                            </div>
                            <div class="text-muted">
                                <i class="bi bi-layers"></i> ${result.location.shelf} • ${result.location.level}
                            </div>
                           </div>
                           <div class="col-6">
                                <div class="text-muted text-end">
                                <small class="text-${capacityColor}">
                                    <i class="bi bi-pie-chart-fill"></i> ${remainingCapacity}% còn trống
                                </small>
                                 </div>
                            <div class="mt-2 text-end">
                                <button class="btn btn-primary btn-sm view-detail-btn" 
                                    onclick="goToSlotDetail('${result.location.aisleId}', '${result.location.slotId}')">
                                    <i class="bi bi-eye"></i> Xem chi tiết
                                </button>
                            </div>
                            </div>
                        </div>
                        </div>
                    </div>`;

                if (index % 2 === 1 || index === group.items.length - 1) html += `</div>`;
            });
        });
    }

    $searchResults.html(html);
    $searchLable.html(`<div>${htmllable}</div>`);
    $searchResults.show();
}

function displaySearchResultsBarcode(results, searchTerm) {
    const $searchResults = $('#searchResults');
    const $searchLable = $('#searchLable');
    let html = '';
    let htmllable = '';

    if (results.length === 0) {
        html = `
                <div class="list-group-item search-result-item text-center py-3">
                    <i class="bi bi-search text-muted fs-1"></i>
                    <div class="mt-2 text-muted">Không tìm thấy vật tư "${searchTerm}"</div>
                    <small class="text-muted">Hãy thử từ khóa khác</small>
                </div>`;
    } else {
        const groupedResults = {};
        console.log(results)
        results.forEach(result => {
            const aisleId = result.location.aisleId;
            if (!groupedResults[aisleId]) {
                groupedResults[aisleId] = {
                    aisleName: result.location.aisle,
                    items: []
                };
            }
            groupedResults[aisleId].items.push(result);
        });

        htmllable += `
                <div class="list-group-item bg-primary text-white py-2 px-3">
                    <small><i class="bi bi-search"></i> Tìm thấy ${results.length} kết quả cho "${searchTerm}"</small>
                </div>`;

        Object.keys(groupedResults).forEach(aisleId => {
            const group = groupedResults[aisleId];
            console.log(group)
            html += `
        <div class="list-group-item bg-light py-2 px-3">
            <small class="fw-bold text-primary">
                <i class="bi bi-building-fill"></i> ${group.aisleName}
            </small>
        </div>`;

            group.items.forEach((result, index) => {

                const remainingCapacity = getRemainingCapacity(result.slot.usedCBM, result.slot.cbmO);

                let capacityColor = 'success';
                if (remainingCapacity === 0) capacityColor = 'danger';
                else if (remainingCapacity < 30) capacityColor = 'warning';
                else if (remainingCapacity < 70) capacityColor = 'info';

                if (index % 2 === 0) {
                    html += `<div class="row pt-2 pb-2">`;
                }

                if (result.location.slotId == null) return
                html += `
            <div class="col-12 col-lg-6">
                <div class="list-group-item search-result-item py-3 px-3">
                    <div class="row align-items-center">
                        <div class="col-md-5">
                            <div class=" text-primary mb-1">
                                <i class="bi bi-box-seam"></i> ${highlightSearchTerm(result.slot.displayGroup, searchTerm)}
                            </div>
                               <div class="item-info text-muted">
                               ItemCode: ${highlightSearchTerm(result.slot.mauVT, searchTerm)}
                            </div>
                                <div class="item-info text-muted">
                                     Màu: ${result.slot.mauVT} -   Khổ/size: ${result.slot.khoVai}
                            </div>
                                
                            <div class="item-info text-muted">
                                 Số lô: ${highlightSearchTerm(result.slot.soLo, searchTerm)} -
                                Số LOT ${highlightSearchTerm(result.slot.soLoT, searchTerm)}
                            </div>
                                <div class="item-info text-muted">
                               <i class="fa-solid fa-clock"></i> Ngày nhập kho: ${highlightSearchTerm(result.slot.ngayNhapKho, searchTerm)}
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="item-location-badge text-center mb-2">
                                <i class="bi bi-geo-alt-fill"></i> ${result.location.slotId}
                            </div>
                            <div class="text-center">
                                <small class="text-muted">${result.location.shelf} • ${result.location.level}</small>
                            </div>
                        </div>
                        <div class="col-md-3 text-end">
                            <div class="mb-2">
                                <small class="text-${capacityColor}">
                                    <i class="bi bi-pie-chart-fill"></i> ${remainingCapacity}% còn trống
                                </small>
                            </div>
                            <button class="btn btn-primary btn-sm view-detail-btn" 
                                    onclick="goToSlotDetail('${result.location.aisleId}', '${result.location.slotId}',false)">
                                <i class="bi bi-eye"></i> Di chuyển tới ô
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

                if (index % 2 === 1 || index === group.items.length - 1) {
                    html += `</div>`;
                }
            });
        });
    }

    $searchResults.html(`<div>${html}</div>`);
    $searchLable.html(`<div>${htmllable}</div>`);
    $searchResults.show();
}

function highlightSearchTerm(text, searchTerm) {
    const regex = new RegExp(`(${searchTerm})`, 'gi');
    return text.replace(regex, '<mark class="bg-warning">$1</mark>');
}

function goToSlotDetail(aisleId, slotId, checkPop = true) {
    $('#searchResults').hide();

    setTimeout(() => {
        const $slotElement = $(`[data-id="${slotId}"]`);
        if ($slotElement.length) {
            $slotElement.addClass('highlighted');
            $slotElement[0].scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });

            if (checkPop) {
                setTimeout(() => {
                    showSlotDetail(slotId);
                }, 500);
            }
        }
    }, 100);
}

$(document).on('keydown', function (e) {
    if (e.key === 'Escape') {
        if ($('#searchResults').is(':visible')) {
            $('#searchResults').hide();
        }
    }

    if (e.ctrlKey && e.key === 'f') {
        e.preventDefault();
        $('#globalSearchInput').focus();
    }

    if (e.ctrlKey && e.key === 'k') {
        e.preventDefault();
        $('#globalSearchInput').focus().select();
    }
});

//async function showMoveItemModal(fromSlotId, toSlotId) {
//    const modal = new bootstrap.Modal($('#moveItemModal')[0]);
//    modal.show();

//    $("#fromSlotName").text(fromSlotId);
//    $("#toSlotName").text(toSlotId);

//    const fromItems = await fetchSlotItems(fromSlotId);
//    const toItems = await fetchSlotItems(toSlotId);

//    const fromCBMO = fromItems[0]?.CBMO ?? 0;
//    const fromCBMSD = fromItems[0]?.CBMSD ?? 0;
//    const toCBMO = toItems[0]?.CBMO ?? 0;
//    const toCBMSD = toItems[0]?.CBMSD ?? 0;

//    $("#fromSlotNameCBM").text(fromCBMO - fromCBMSD);
//    $("#totalCBM").text(fromCBMSD);
//    $("#toSlotNameCBM").text(toCBMO - toCBMSD);
//    $("#totalToCBMDC").text(toCBMSD)
//    renderSlotItems("fromSlotItems", fromItems, "from");
//    renderSlotItems("toSlotItems", toItems, "to");

//    $("#btnMoveToRight").off('click').on('click', function () {
//        moveCheckedItems("fromSlotItems", "toSlotItems");
//    });

//    $("#btnConfirmMove").off('click').on('click', function () {
//        const movedItems = collectSlotItems("toSlotItems");
//        showToast('success', `Đã di chuyển thành công " + movedItems.length + " vật tư.!`, 1500);
//        modal.hide();
//    });
//}

// Modal di chuyển vật tư với tính năng quét barcode
async function showMoveItemModal(fromSlotId, toSlotId) {
    const modal = new bootstrap.Modal($('#moveItemModal')[0]);
    modal.show();

    $("#fromSlotName").text(fromSlotId);
    $("#toSlotName").text(toSlotId);

    const fromItems = await fetchSlotItems(fromSlotId);
    const toItems = await fetchSlotItems(toSlotId);

    const fromCBMO = fromItems[0]?.CBMO ?? 0;
    const fromCBMSD = fromItems[0]?.CBMSD ?? 0;
    const toCBMO = toItems[0]?.CBMO ?? 0;
    const toCBMSD = toItems[0]?.CBMSD ?? 0;
    const formtoCBM = fromCBMO - fromCBMSD
    const toFromCBM = toCBMO - toCBMSD
    $("#fromSlotNameCBM").text(parseFloat(formtoCBM.toFixed(4)));
    $("#totalCBM").text(fromCBMSD);
    $("#toSlotNameCBM").text(parseFloat(toFromCBM.toFixed(4)));
    $("#totalToCBMDC").text(toCBMSD);

    renderSlotItems("fromSlotItems", fromItems, "from");
    renderSlotItems("toSlotItems", toItems, "to");

    // Khởi tạo tính năng quét barcode cho modal di chuyển
    initMoveModalBarcodeScanner();

    $("#btnMoveToRight").off('click').on('click', function () {
        moveCheckedItems("fromSlotItems", "toSlotItems");
    });

    $("#btnConfirmMove").off('click').on('click', function () {
        const movedItems = collectSlotItems("toSlotItems");
        showToast('success', `Đã di chuyển thành công ${movedItems.length} vật tư!`, 1500);
        modal.hide();
    });
}
// Khởi tạo scanner barcode cho modal di chuyển
function initMoveModalBarcodeScanner() {
    // Xử lý tìm kiếm barcode từ button
    $('#searchMoveBarcodeBtnLeft, #searchMoveBarcodeBtnRight').off('click').on('click', function () {
        const side = $(this).data('side'); // 'left' hoặc 'right'
        const inputId = side === 'left' ? '#moveBarcodeInputLeft' : '#moveBarcodeInputRight';
        const barcode = $(inputId).val().trim();

        if (barcode) {
            searchBarcodeInMoveModal(barcode, side);
            $(inputId).val('');
        } else {
            showToast('warning', 'Vui lòng nhập mã barcode!');
        }
    });

    // Xử lý Enter trong ô input
    $('#moveBarcodeInputLeft, #moveBarcodeInputRight').off('keypress').on('keypress', function (e) {
        if (e.which === 13) { // Enter key
            const barcode = $(this).val().trim();
            const side = $(this).attr('id') === 'moveBarcodeInputLeft' ? 'left' : 'right';

            if (barcode) {
                searchBarcodeInMoveModal(barcode, side);
                $(this).val('');
            }
        }
    });

    // Focus vào input khi modal mở
    setTimeout(() => {
        $('#moveBarcodeInputLeft').focus();
    }, 500);
}
// Hàm tìm kiếm barcode trong modal di chuyển
async function searchBarcodeInMoveModal(barcode, side) {
    const containerId = 'fromSlotItems'; // Chỉ tìm bên trái
    const $container = $(`#${containerId}`);

    // Tìm checkbox có barcode khớp
    let found = false;

    return new Promise((resolve) => {
        $container.find('input[type="checkbox"]').each(function () {
            const itemData = JSON.parse($(this).attr("data-item"));

            if (itemData.BarCode === barcode) {
                found = true;
                const $checkbox = $(this);
                const $itemRow = $checkbox.closest('.item-row');

                // Bên trái - Ô cần chuyển
                if ($checkbox.hasClass('d-none')) {
                    showToast('warning', '⚠️ Vật tư này không thể di chuyển!');
                    PlayAudioError()();
                    resolve(false);
                    return false;
                }

                // Check xem có đủ sức chứa bên phải không
                const itemCBM = parseFloat(itemData.CBM) || 0;
                const toCBMRemaining = parseFloat($('#toSlotNameCBM').text()) || 0;
                const currentSelectedCBM = parseFloat($('#totalCBMDC').text()) || 0;

                if ($checkbox.is(':checked')) {
                    // Nếu đã check rồi thì bỏ check
                    $checkbox.prop('checked', false).trigger('change');
                    showToast('success', `✅ Đã bỏ chọn: ${itemData.DisplayGroup}`);
                    PlayAudio();
                    resolve(true);
                } else {
                    // Kiểm tra CBM trước khi check
                    if ((currentSelectedCBM + itemCBM) > toCBMRemaining) {
                        const message = `⚠️ CẢNH BÁO: Không đủ sức chứa! Roll "${itemData.SoKien}" có CBM ${itemCBM.toFixed(4)} m³. Đã chọn: ${currentSelectedCBM.toFixed(4)} m³. Sức chứa còn lại: ${toCBMRemaining.toFixed(4)} m³`;

                        showToast('warning', message, 4000);
                        PlayAudioError()();

                        // Highlight màu đỏ
                        $itemRow.css('background-color', '#ffe6e6');
                        setTimeout(() => {
                            $itemRow.css('background-color', '');
                        }, 2000);
                        resolve(false);
                    } else {
                        // Đủ chỗ - cho phép check
                        $checkbox.prop('checked', true).trigger('change');
                        showToast('success', `✅ Đã chọn: ${itemData.DisplayGroup} (${itemCBM.toFixed(4)} CBM)`);
                        PlayAudio();

                        // Highlight màu xanh
                        $itemRow.css('background-color', '#e6ffe6');

                        // Scroll đến item
                        $itemRow[0].scrollIntoView({ behavior: 'smooth', block: 'center' });

                        setTimeout(() => {
                            $itemRow.css('background-color', '');
                        }, 1000);
                        resolve(true);
                    }
                }

                return false; // Break loop
            }
        });

        if (!found) {
            showToast('error', `❌ Không tìm thấy barcode "${barcode}" trong Ô cần chuyển!`);
            PlayAudioError()();
            resolve(false);
        }
    });
}
function renderSlotItems(containerId, items, side) {
    const $container = $('#' + containerId);
    let html = "";

    if (!items || items.length === 0 || items[0].DisplayGroup == null) {
        html = `<div class="text-center text-muted py-3 text-rong"><i class="bi bi-inbox"></i> Không có vật tư</div>`;
    } else {
        items.forEach((item, i) => {
            html += `
                <div class="form-check item-row border-bottom py-2">
                    <input data-cbm="${item.CBM}" 
                       class="form-check-input move-check-${side} ${side == "to" ? "d-none" : ""}" 
                       type="checkbox" 
                       id="${side}-chk-${i}"
                       data-item='${btoa(encodeURIComponent(JSON.stringify(item)))}'>
                    <label class="form-check-label w-100" for="${side}-chk-${i}">
                        <div class="fw-bold text-primary">${item.DisplayGroup}</div>
                        <small class="text-muted">Số lô: ${item.SoLo} | Vật tư: ${item.SoKien} | ${item.CBM} CBM</small>
                    </label>
                </div>`;
        });
    }

    $container.html(html);
}
$(document).on("change", '.move-check-from', function () {
    var totalCBMDC = parseFloat($("#totalCBMDC").text()) || 0;
    var valueCBM = parseFloat($(this).data("cbm")) || 0;

    if ($(this).is(":checked")) {
        var value = totalCBMDC + valueCBM;
    } else {
        // nếu bỏ check thì trừ đi
        var value = totalCBMDC - valueCBM;
    }

    $("#totalCBMDC").text(value.toFixed(4));
});
function moveCheckedItems(fromContainerId, toContainerId) {
    const $fromContainer = $('#' + fromContainerId);
    const $toContainer = $('#' + toContainerId);

    const $checkedBoxes = $fromContainer.find('input[type="checkbox"]:checked');
    if ($checkedBoxes.length === 0) {
        showToast('warning', `Vui lòng chọn ít nhất một vật tư để chuyển!`, 1500);
        return;
    }

    let totalCBMMove = 0;
    const SaveFromTo = [];

    // Validate trước
    $checkedBoxes.each(function () {
        const encodedData = $(this).attr("data-item");
        const itemData = JSON.parse(decodeURIComponent(atob(encodedData)));
        totalCBMMove += parseFloat(itemData.CBM) || 0;
    });

    const toCBMRemaining = parseFloat($("#toSlotNameCBM").text()) || 0;

    if (totalCBMMove > toCBMRemaining) {
        alert(`Không thể chuyển! Tổng CBM (${totalCBMMove.toFixed(4)}) vượt quá sức chứa còn lại của ô đích (${toCBMRemaining.toFixed(4)} CBM).`);
        return;
    }

    // Thực hiện chuyển
    $checkedBoxes.each(function () {
        const encodedData = $(this).attr("data-item");
        const itemData = JSON.parse(decodeURIComponent(atob(encodedData)));

        let object = {
            BarCode: itemData.BarCode,
            MaVTGhep: itemData.DisplayGroup,
            MaONPLFrom: "",
            MaONPLTo: $("#vattuden").val(),
        };
        SaveFromTo.push(object);

        const $newDiv = $('<div>', {
            class: 'form-check item-row border-bottom py-2'
        }).html(`
            <input data-cbm="${itemData.CBM}" 
                   class="form-check-input move-check-to d-none" 
                   type="checkbox"
                   data-item='${btoa(encodeURIComponent(JSON.stringify(itemData)))}'>
            <label class="form-check-label w-100">
                <div class="fw-bold text-primary">${itemData.DisplayGroup}</div>
                <small class="text-muted">Số lô: ${itemData.SoLo} | Vật tư: ${itemData.SoKien} | ${itemData.CBM} CBM</small>
            </label>
        `);

        $toContainer.append($newDiv);
        $(this).closest(".item-row").remove();
    });

    $(".text-rong").remove();
    SaveFormToONPL(SaveFromTo);

    // Cập nhật CBM
    $("#toSlotNameCBM").text((toCBMRemaining - totalCBMMove).toFixed(4));
    const fromCBMRemaining = parseFloat($("#fromSlotNameCBM").text()) || 0;
    $("#fromSlotNameCBM").text((fromCBMRemaining + totalCBMMove).toFixed(4));

    var totalCBMDC = parseFloat($("#totalCBMDC").text()) || 0;
    var totalCBMSD = parseFloat($("#totalCBM").text()) || 0;
    var totalToCBMSD = parseFloat($("#totalToCBMDC").text()) || 0;

    $("#totalCBM").text((totalCBMSD - totalCBMDC).toFixed(4));
    $("#totalToCBMDC").text((totalToCBMSD + totalCBMDC).toFixed(4));
    $("#totalCBMDC").text(0);
}

function collectSlotItems(containerId) {
    const $container = $('#' + containerId);
    const items = [];
    $container.find('input[type="checkbox"]').each(function () {
        items.push(JSON.parse($(this).attr("data-item")));
    });
    return items;
}

function ToFromVT() {
    var tenOFrom = $("#vattuchuyen").val()
    console.log(tenOFrom)
    var tenOTo = $("#vattuden").val()
    if (tenOFrom == "") {
        showToast('warning', `Vui lòng chọn ô cần chuyển `, 2000);
        return
    }
    if (tenOTo == "") {
        showToast('warning', `Vui lòng chọn ô nhận vật tư `, 2000);
        return
    }
    if (tenOFrom == tenOTo) {
        showToast('warning', `Vui lòng chọn ô cần chuyển khác ô nhận vật tư!`, 2000);
        return
    }
    showMoveItemModal(tenOFrom, tenOTo);
}
async function SaveFormToONPL(arrSave) {
    var dataUser = !window.CefSharp ? userNameSave : userName
    const request = new Request(`/api/ERPVatTuCBM/Post?action=UpdateONPKL&para1=${dataUser}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(arrSave),
    });
    let response = await fetch(request)
    let data = await response.json()
    if (data == "True") {
        showToast('success', `Chuyển đổi ô thành công!`, 1500);
        /*ToFromVT()*/

        GetCTietNKViTriKho()
        GetViTriKho()
        GetDanhSachO()
    }
    else {
        showToast('error', `Chuyển đổi ô thất bại!`, 2000);
        return
    }
}
// Gợi ý
var warehouseSlots = []
var selectedItems = []
async function GetOSugget() {
    warehouseSlots = []
    const module = $("#nguyenphulieuid").val() == 1 ? 1 : 2
    const isNPL = $("#nguyenphulieuid").val()
    const url = `/api/ViTriKhoNPL/Get?action=GetSuggetO&para1=${module}&para2=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();

        warehouseSlots = data.Table

    } catch (error) {
        console.error(error.message);
    }
}
async function Sugget() {
    selectedItems = []
    await GetOSugget()
    $("#tbody tr").each(function () {
        const $trRow = $(this)
        var checkbarCode = $trRow.find("input.checkNK").is(":checked");
        var checkHassClass = $trRow.find("input.checkNK").hasClass("d-none")
        if (!checkbarCode || checkHassClass) return;

        const cbm = $trRow.data("cbm")
        const mavtghep = $trRow.data("mavtghep")
        const soroll = $trRow.data("sokien")
        const barcode = $trRow.data("barcode")
        let parts = barcode.split("|");

        // Lấy cụm F20NA (phần thứ 2)
        let code = parts[1];

        // Lấy 2 ký tự đầu (F2)
        let first2 = code.substring(0, 2);
        let object = {
            CBM: parseFloat(cbm),
            MaVTGhep: mavtghep,
            SoRoll: soroll,
            BarCode: barcode,
            CheckKe: first2
        }
        selectedItems.push(object)
    })
    if (selectedItems.length == 0) {
        showToast('error', `Vui lòng chọn vật tư để đưa vào kho!`, 2000);
        return
    }
    $('#suggestionModal').modal('show');
    showSuggestionModal();
}


// Xử lý sự kiện click button chọn ô đơn (delegate event)
$(document).on('click', '.btn-select-single', function () {
    const slotId = $(this).data('slot');
    selectSingleSlot(slotId);
});

// Xử lý sự kiện click button chọn nhóm ô (delegate event)
$(document).on('click', '.btn-select-group', function () {
    const slotIds = $(this).data('slots');
    selectSlotGroup(slotIds);
});

function showSuggestionModal() {
    // Tính tổng CBM cần lưu
    const totalCBM = parseFloat(parseFloat(selectedItems.reduce((sum, item) => sum + item.CBM, 0)).toFixed(4));

    $('#totalCBMNeed').text(totalCBM);

    $('#totalRolls').text(selectedItems.length);

    // Lấy danh sách MaVTGhep duy nhất
    const uniqueMaVTGhep = [...new Set(selectedItems.map(item => item.MaVTGhep))];

    // Phần 1: Tìm ô đã chứa cùng loại vật tư (có thể kết hợp nhiều ô)
    const sameTypeSlotGroups = [];
    $.each(uniqueMaVTGhep, function (index, maVT) {
        const itemsOfType = selectedItems.filter(item => item.MaVTGhep === maVT);
        const totalCBMOfType = itemsOfType.reduce((sum, item) => sum + item.CBM, 0);
        const totalRollOfType = itemsOfType.reduce((sum, item) => sum + (item.SoRoll || 0), 0);
        // Lấy tất cả ô có cùng loại vật tư, ưu tiên ô còn nhiều chỗ trống
        const availableSlots = warehouseSlots
            .filter(slot => slot.MaVTGhep == maVT && slot.CBMCL > 0)
            .sort((a, b) => b.CBMCL - a.CBMCL)
            .filter((slot, index, self) =>
                index === self.findIndex(s => s.TenO === slot.TenO)
            );
        // Tìm tổ hợp ô vừa đủ
        const selectedSlots = findOptimalSlotCombination(availableSlots, parseFloat(totalCBMOfType).toFixed(4));

        if (selectedSlots.length > 0) {
            sameTypeSlotGroups.push({
                maVT: maVT,
                slots: selectedSlots,
                totalRequired: totalCBMOfType,
                totalRoll: totalRollOfType,
                totalAvailable: selectedSlots.reduce((sum, s) => sum + s.CBMCL, 0),
                itemCount: itemsOfType.length
            });
        }
    });

    const emptySlots = warehouseSlots
        .filter(slot => slot.MaVTGhep !== null && slot.CBMCL > 0)
        .sort((a, b) => b.CBMCL - a.CBMCL);
    const emptySlotGroups = findOptimalSlotCombination(emptySlots, totalCBM);

    // Render
    renderSameTypeSlots(sameTypeSlotGroups);
    renderEmptySlots(emptySlotGroups, totalCBM, selectedItems);

    // Show modal

}

function findOptimalSlotCombination(slots, requiredCBM) {
    const selected = [];
    let accumulated = 0;

    for (const slot of slots) {
        selected.push(slot);
        accumulated += slot.CBMCL;
    }

    return selected;
}

function allocateItemsToSlots(slots, items) {
    const allocated = slots.map(slot => ({
        ...slot,
        assignedItems: []
    }));

    const sortedItems = [...items].sort((a, b) => b.CBM - a.CBM);

    $.each(sortedItems, function (index, item) {
        for (let slot of allocated) {
            const usedCBM = slot.assignedItems.reduce((sum, i) => sum + i.CBM, 0);
            if (slot.CBMCL >= usedCBM + item.CBM) {
                slot.assignedItems.push(item);
                break;
            }
        }
    });

    return allocated;
}

function allocateItemsToSameTypeSlots(slots, items) {
    const allocated = slots.map(slot => ({
        ...slot,
        assignedItems: []
    }));

    let remainingItems = [...items];

    for (let slot of allocated) {
        let availableSpace = slot.CBMCL;
        let i = 0;

        while (i < remainingItems.length && availableSpace > 0) {
            const item = remainingItems[i];

            if (item.CBM <= availableSpace) {
                slot.assignedItems.push(item);
                availableSpace -= item.CBM;
                remainingItems.splice(i, 1);
            } else {
                i++;
            }
        }
    }

    return allocated;
}

function renderSameTypeSlots(slotGroups) {
    const $container = $('#sameTypeSlots');

    if (slotGroups.length === 0) {
        $container.html(`
                        <div class="no-data-placeholder">
                            <i class="bi bi-inbox fs-1"></i>
                            <div class="mt-2">Không có ô nào đang chứa cùng loại vật tư</div>
                        </div>
                    `);
        return;
    }
    slotGroups.sort((a, b) => a.maVT.localeCompare(b.maVT, 'vi', { sensitivity: 'base' }));
    let html = '';
    $.each(slotGroups, function (groupIndex, group) {
        const shortName = group.maVT;
        const itemsOfType = selectedItems.filter(item => item.MaVTGhep === group.maVT);
        const allocatedSlots = allocateItemsToSameTypeSlots(group.slots, itemsOfType);

        html += `
                        <div class="slot-group" >
                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <h6 class="mb-0 text-primary">
                                    <i class="bi bi-box-seam"></i> ${shortName}
                                </h6>
                                <span class="combination-badge">
                                   ${group.totalRequired.toFixed(4)} CBM cần lưu
                                </span>
                            </div>
                            <div class="row g-3">
                    `;

        $.each(allocatedSlots, function (index, slot) {
            const totalRollInSlot = slot.assignedItems.length
            const totalCBMInSlot = slot.assignedItems.reduce((sum, i) => sum + i.CBM, 0);
            if ((slot.assignedItems || []).length == 0) return;
            html += `
                            <div class="col-md-6 col-lg-4">
                                <div class="card suggest-card position-relative h-100" data-ten="${slot.TenO}" data-items='${JSON.stringify(slot.assignedItems || [])}'>
                                    <div class="priority-badge">${index + 1}</div>
                                    <div class="card-body">
                                        <h6 class="card-title text-primary mb-3">
                                            <i class="bi bi-geo-alt-fill"></i> ${slot.TenO}
                                        </h6>
                                        
                                        <div class="slot-info">
                                            <i class="bi bi-check-circle text-success"></i>
                                            <small class="text-success fw-bold">Cùng loại vật tư</small>
                                        </div>

                                        <div class="slot-info">
                                            <i class="bi bi-inbox"></i>
                                            <small><strong>Còn trống:</strong> ${parseFloat(slot.CBMCL.toFixed(4))} CBM</small>
                                        </div>

                                        <div class="slot-info">
                                            <i class="bi bi-stack"></i>
                                            <small><strong>Đang chứa:</strong> ${parseFloat(slot.CBMSD.toFixed(4))} CBM</small>
                                        </div>

                                        ${slot.assignedItems.length > 0 ? `
                                            <div class="alert alert-success mt-3 mb-2 py-2">
                                                <div class="fw-bold small mb-1">
                                                    <i class="bi bi-arrow-down-circle"></i> Sẽ nhận:
                                                </div>
                                                <div class="text-dark mb-2">
                                                    <i class="bi bi-stack"></i> <strong>${totalRollInSlot} vật tư</strong> - 
                                                    <strong>${parseFloat(totalCBMInSlot).toFixed(4)} CBM</strong>
                                                </div>
                                                <hr class="my-2">
                                                <div class="small">
                                        ` : `
                                            <div class="alert alert-warning mt-3 mb-2 py-2">
                                                <small><i class="bi bi-info-circle"></i> Không đủ chỗ trống</small>
                                            </div>
                                        `}
                                        
                                        ${slot.assignedItems.map(item => {
                return `
                                                <div class="d-flex justify-content-between py-1 border-bottom">
                                                    <span class="" style="max-width: 75%;">
                                                        <i class="bi bi-box-seam"></i>  ${shortName} - Vật tư ${item.SoRoll} 
                                                    </span>
                                                    <span class="text-muted">
                                                      ${item.CBM.toFixed(4)}
                                                    </span>
                                                </div>
                                            `;
            }).join('')}
                                        
                                        ${slot.assignedItems.length > 0 ? `
                                                </div>
                                            </div>
                                        ` : ''}

                                        <button class="btn btn-sm btn-primary w-100 select-btn btn-select-single mt-2" 
                                                data-slot="${slot.TenO}"
                                                ${slot.assignedItems.length === 0 ? 'disabled' : ''}>
                                            <i class="bi bi-check-circle"></i> Chọn ô này
                                        </button>
                                    </div>
                                </div>
                            </div>
                        `;
        });

        const slotNames = allocatedSlots.map(s => s.TenO);
        const allOLength = allocatedSlots.filter(x => (x.assignedItems?.length || 0) > 0);

        const totalAvailable = allOLength.reduce((sum, s) => sum + (s.CBMCL || 0), 0);
        html += `
                            </div>
                            <div class="text-center mt-3">
                                <button class="btn btn-success select-btn btn-select-group" 
                                        data-slots='${JSON.stringify(slotNames)}'>
                                    <i class="bi bi-collection"></i> Chọn tất cả ${allOLength.length} ô
                                    (${totalAvailable.toFixed(4)} CBM)
                                </button>
                            </div>
                        </div>
                    `;
    });

    $container.html(html);
}
function renderEmptySlots(slots, totalRequired, selectedItems) {
    const $container = $('#emptySlots');

    if (slots.length === 0) {
        $container.html(`
            <div class="no-data-placeholder">
                <i class="bi bi-exclamation-triangle fs-1"></i>
                <div class="mt-2">Không có đủ ô trống để chứa ${totalRequired.toFixed(4)} CBM</div>
                <small class="text-muted">Vui lòng chọn ít vật tư hơn hoặc giải phóng ô khác</small>
            </div>
        `);
        return;
    }

    // Lấy danh sách CheckKe duy nhất từ selectedItems
    const uniqueCheckKe = [...new Set(selectedItems.map(item => item.CheckKe).filter(Boolean))];

    // Ưu tiên phân bổ theo kệ
    let allocatedSlots = [];
    let remainingItems = [...selectedItems];

    // Bước 1: Phân bổ vào các ô cùng kệ
    uniqueCheckKe.forEach(checkKe => {
        // Lọc items thuộc kệ này
        const itemsInSameShelf = remainingItems.filter(item => item.CheckKe === checkKe);

        // Lọc slots cùng kệ (TextKe chứa CheckKe)
        const slotsInSameShelf = slots.filter(slot =>
            slot.TextKe && slot.TextKe.includes(checkKe)
        ).sort((a, b) => b.CBMCL - a.CBMCL);

        if (slotsInSameShelf.length > 0 && itemsInSameShelf.length > 0) {
            const allocated = allocateItemsToSlots(slotsInSameShelf, itemsInSameShelf);
            allocatedSlots.push(...allocated);

            // Loại bỏ các items đã được phân bổ
            allocated.forEach(slot => {
                slot.assignedItems.forEach(assignedItem => {
                    const index = remainingItems.findIndex(item =>
                        item.BarCode === assignedItem.BarCode
                    );
                    if (index !== -1) {
                        remainingItems.splice(index, 1);
                    }
                });
            });
        }
    });

    // Bước 2: Phân bổ items còn lại vào các ô trống còn lại
    if (remainingItems.length > 0) {
        // Lọc các ô chưa được sử dụng
        const usedSlotIds = allocatedSlots.map(s => s.TenO);
        const remainingSlots = slots.filter(slot => !usedSlotIds.includes(slot.TenO));

        const allocated = allocateItemsToSlots(remainingSlots, remainingItems);
        allocatedSlots.push(...allocated);
    }

    console.log(allocatedSlots)

    // ========== PHẦN RENDER HTML ==========
    const totalRolls = selectedItems.reduce((sum, item) => sum + (item.SoRoll || 0), 0);

    let html = `
        <div class="slot-group">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h6 class="mb-0 text-info">
                    <i class="bi bi-inbox-fill"></i> Phân bổ tối ưu - ${allocatedSlots.filter(s => s.assignedItems?.length > 0).length} ô trống
                </h6>
                <span class="combination-badge">
                    ${totalRequired.toFixed(4)} CBM cần lưu
                </span>
            </div>

            <div class="row g-3">
    `;

    $.each(allocatedSlots, function (index, slot) {
        const totalRollInSlot = slot.assignedItems.length;
        const totalCBMInSlot = slot.assignedItems.reduce((sum, i) => sum + i.CBM, 0);

        if ((slot.assignedItems || []).length == 0) return;

        // Kiểm tra xem có phải ô được ưu tiên (cùng kệ) không
        const isSameShelf = slot.assignedItems.some(item =>
            item.CheckKe && slot.TextKe && slot.TextKe.includes(item.CheckKe)
        );

        html += `
            <div class="col-md-6 col-lg-4">
                <div class="card suggest-card h-100 ${isSameShelf ? 'border-success' : ''}" 
                     data-ten="${slot.TenO}" 
                     data-items='${JSON.stringify(slot.assignedItems || [])}'>
                    <div class="card-body">
                        <h6 class="card-title ${isSameShelf ? 'text-success' : 'text-info'} mb-3">
                            <i class="bi bi-geo-alt"></i> ${slot.TenO}
                        </h6>
                        
                        <div class="slot-info">
                            <i class="bi bi-inbox-fill ${isSameShelf ? 'text-success' : 'text-info'}"></i>
                            <small class="${isSameShelf ? 'text-success' : 'text-info'} fw-bold">
                                ${isSameShelf ? 'Ô trống - Cùng kệ' : 'Ô trống'}
                            </small>
                        </div>
                        
                        ${slot.TextKe ? `
                            <div class="slot-info">
                                <i class="bi bi-bookshelf"></i>
                                <small><strong>Kệ:</strong> ${slot.TextKe}</small>
                            </div>
                        ` : ''}

                        <div class="slot-info">
                            <i class="bi bi-inbox"></i>
                            <small><strong>Dung lượng:</strong> ${slot.CBMCL.toFixed(4)} CBM</small>
                        </div>

                        ${slot.assignedItems.length > 0 ? `
                            <div class="alert ${isSameShelf ? 'alert-success' : 'alert-info'} mt-3 mb-2 py-2">
                                <div class="fw-bold small mb-1">
                                    <i class="bi bi-arrow-down-circle"></i> Sẽ nhận:
                                </div>
                                <div class="text-dark">
                                    <i class="bi bi-stack"></i> <strong>${totalRollInSlot} vật tư</strong> - 
                                    <strong>${parseFloat(totalCBMInSlot).toFixed(4)} CBM</strong>
                                </div>
                                <hr class="my-2">
                                <div class="small">
                        ` : ''}
                        
                        ${slot.assignedItems.map(item => {
            const shortName = item.MaVTGhep;
            return `
                                <div class="d-flex justify-content-between py-1 border-bottom">
                                    <span class="" style="max-width: 75%;" title="${shortName}">
                                        <i class="bi bi-box-seam"></i> ${shortName} - Vật tư ${item.SoRoll}
                                    </span>
                                    <span class="text-muted">
                                        ${item.CBM.toFixed(4)}
                                    </span>
                                </div>
                            `;
        }).join('')}
                        
                        ${slot.assignedItems.length > 0 ? `
                                </div>
                            </div>
                        ` : ''}

                        <button class="btn btn-sm ${isSameShelf ? 'btn-success' : 'btn-outline-primary'} w-100 select-btn btn-select-single mt-2" 
                                data-slot="${slot.TenO}">
                            <i class="bi bi-check-circle"></i> Chọn ô này
                        </button>
                    </div>
                </div>
            </div>
        `;
    });

    const slotNames = allocatedSlots.map(s => s.TenO);
    const allOLength = allocatedSlots.filter(x => (x.assignedItems?.length || 0) > 0);
    const totalAvailable = allOLength.reduce((sum, s) => sum + (s.CBMCL || 0), 0);

    html += `
            </div>
            <div class="text-center mt-3">
                <button class="btn btn-info select-btn btn-select-group" 
                        data-slots='${JSON.stringify(slotNames)}'>
                    <i class="bi bi-collection"></i> Chọn tất cả ${allOLength.length} ô
                    (${totalAvailable.toFixed(4)} CBM)
                </button>
            </div>
        </div>
    `;

    $container.html(html);
}
function selectSingleSlot(slotId, checkvalue = true) {
    let $card = $(`.suggest-card[data-ten="${slotId}"]`);

    if ($card.length > 0) {
        let items = $card.data("items") || [];

        // Tính tổng CBM của các item bị xóa
        let totalCBMRemoved = 0;

        selectedItems = selectedItems.filter(sel => {
            let matched = items.some(it =>
                it.MaVTGhep === sel.MaVTGhep && it.BarCode == sel.BarCode
            );
            if (matched) totalCBMRemoved += sel.CBM;
            return !matched;
        });
        const arrONPL = []
        items.map(x => {
            let $tr = $(`table tr[data-barcode="${x.BarCode}"]`)
            $tr.css("background", "#b5fff8");
            $tr.find(".divNK").css("display", "none")
            $tr.find(".itemNK").addClass("checknhapkho")
            $tr.find(".vitrioitem").text(slotId)
            $tr.find(".editcbm ").css("display", "none")
            $tr.find(".checkNK ").prop("checked", false)


            let object = {
                ID: 0,
                SoLoID: "",
                MaNPL: "",
                MaVTGhep: x.MaVTGhep,
                BarCode: x.BarCode,
                Dai: 0,
                Rong: 0,
                Cao: 0,
                DK: 0,
                CBM: 0,
                IsNPL: 0,
                IsCBM: 0,
                MaONPL: slotId,
            };
            arrONPL.push(object)
        })
        SaveONPL(arrONPL, false)
        let slot = warehouseSlots.find(w => w.TenO === slotId);
        if (slot) {
            slot.CBMCL -= totalCBMRemoved;
            slot.CBMSD += totalCBMRemoved;
            if (slot.CBMSD < 0) slot.CBMSD = 0; // tránh âm
        }
    }
    if (checkvalue)
        showSuggestionModal();
}

function selectSlotGroup(slotIds) {
    slotIds.forEach(x => {
        selectSingleSlot(x, false)
    });
    showSuggestionModal();

}
function viewTbody() {
    if ($(".eyeView").hasClass("fa-eye")) {
        $(".eyeView").removeClass("fa-eye")
        $(".eyeView").addClass("fa-eye-slash")
        $(".dx-scrollable-container").addClass("hide")

    } else {
        $(".eyeView").removeClass("fa-eye-slash")
        $(".eyeView").addClass("fa-eye")
        $(".dx-scrollable-container").removeClass("hide")

    }
}
$(document).on("click", '#checkAllCBM', function () {
    const checker = $(this).is(":checked")
    $("#tbody").find(".checkCBM:not(.d-none)").prop("checked", checker)
})
$(document).on("click", "#checkAllNK", function () {
    const checker = $(this).is(":checked");
    const hasEmpty = $("#tbody").find("tr").filter(function () {
        const $tr = $(this);
        if ($tr.hasClass("d-none") || $tr.data("sokien") == null) return false; // bỏ qua hàng ẩn
        const sln = parseFloat($tr.find(".inputSLN").val()) || 0;
        return sln === 0;
    }).length > 0;
    // Nếu đang check all và có ít nhất 1 dòng chưa nhập -> cảnh báo + bỏ checkAll

    $("#tbody")
        .find(".checkNK")
        .filter(function () {
            const $tr = $(this).closest("tr");
            const sln = parseFloat($tr.find(".inputSLN").val()) || 0;

            return !$(this).closest(".d-none").length && sln > 0;
        })
        .prop("checked", checker);
    sumKienCBM()
    if (checker && hasEmpty) {
        showToast('warning', `Trong danh sách có ít nhất 1 vật tư chưa có CBM.!`, 2000);

    }
});

// Khởi tạo scanner khi mở modal
$('#itemDetailModal').on('shown.bs.modal', function () {
    $('#barcodeInput').focus()
});
// trả hàng 



// === BIẾN GLOBAL ===
let dxGridTraHangInstance = null;

function initGridTraHang(data) {
    dxGridTraHangInstance = $("#trahang").dxDataGrid({
        dataSource: data,
        noDataText: "Chưa có dữ liệu",
        columnAutoWidth: false,
        allowColumnResizing: false,
        columnHidingEnabled: false,
        wordWrapEnabled: true,
        showRowLines: true,
        showBorders: true,
        scrolling: { mode: 'standard' },
        filterRow: { visible: true },
        headerFilter: { visible: false },
        paging: {
            enabled: false
        },
        editing: {
            mode: "row",
            allowDeleting: true,
            confirmDelete: false  // TẮT hỏi lại mặc định
        },
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        width: "100%",
        columns: [
            {
                dataField: "SoLo",
                caption: "Số Lô",
                minWidth: 100,
                alignment: "center"
            },
            {
                dataField: "MaVT",
                caption: "Item Code",
                minWidth: 100,
                alignment: "center"
            },
            {
                dataField: "MoTa",
                caption: "Mô Tả",
                width: 350,
                alignment: "left"
            },
            {
                dataField: "MauVT",
                caption: "Màu VT",
                minWidth: 100,
                alignment: "center"
            },
            {
                dataField: "KhoVai",
                caption: "Kho/Size",
                minWidth: 100,
                alignment: "center"
            },
            {
                dataField: "TenDVVT",
                caption: "Tên ĐVVT",
                minWidth: 80,
                alignment: "center"
            },
            {
                dataField: "SoLuongThucTe",
                caption: "SLCT",
                minWidth: 80,
                alignment: "right",
                dataType: "number"
            },
            {
                dataField: "SoKienHienThi",
                caption: "Vật Tư",
                minWidth: 100,
                alignment: "center"
            },
            {
                dataField: "MaONPL",
                caption: "Vị Trí Ô",
                minWidth: 100,
                alignment: "center"
            },
            {
                dataField: "CBM",
                caption: "CBM",
                minWidth: 80,
                alignment: "right",
                dataType: "number",
                format: {
                    type: "fixedPoint",
                    precision: 4
                }
            },
            {
                type: "buttons",
                caption: "Hủy trả",
                width: 80,
                alignment: "center",
                buttons: [
                    {
                        hint: "Xóa",
                        icon: "trash",
                        cssClass: "btn-delete-row",
                        onClick: function (e) {
                            const rowData = e.row.data;
                            // rowData, e.row.rowIndex
                            showConfirmModal(async function () {
                                let returnedItems = []
                                e.component.deleteRow(e.row.rowIndex);
                                returnedItems.push({
                                    ID: 0,
                                    SoLoID: 0,
                                    MaNPL: 0,
                                    MaVTGhep: 0,
                                    BarCode: rowData.BarCode,
                                    Dai: 0,
                                    Rong: 0,
                                    Cao: 0,
                                    DK: 0,
                                    CBM: 0,
                                    IsNPL: 0,
                                    IsCBM: 0,
                                    MaONPL: "",
                                    Status: 1,
                                });
                                await UpdateTraHang(returnedItems, 1)
                            });
                        }
                    }
                ]
            }
        ],

        summary: {
            totalItems: [
                {
                    column: "SoLo",
                    summaryType: "count",
                    displayFormat: "Vật tư: {0}"
                },
                {
                    column: "SLCT",
                    summaryType: "sum",
                    displayFormat: "{0}"
                },

            ]
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

    }).dxDataGrid("instance");
}
function showConfirmModal(onConfirm) {

    $('#btnCancel').off('click');
    // Khi nhấn Đồng ý
    $('#btnCancel').on('click', function () {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
        // Ẩn modal sau khi xử lý
        $("#deleteItemModal").modal("hide")
    });

    // Hiện modal
    $("#deleteItemModal").modal("show")
}

$('#filterItemCode').on('change', function () {
    GetDanhSachReturn()
});

$(document).ready(function () {
    $("#filterItemCode").select2()
    GetDanhSachItemReturn()
    createViewDxDataTongQuan([])
});
async function GetDanhSachReturn() {
    const maNPL = $("#filterItemCode").val()
    const isNPL = $("#nguyenphulieuid").val()

    const url = `/api/ERPVatTuCBM/Get?action=GetDanhSachReturn&para=${maNPL}&para1=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        initGridTraHang(data)

    } catch (error) {
        console.error(error.message);
    }
}
async function GetDanhSachItemReturn() {

    const isNPL = $("#nguyenphulieuid").val()

    const url = `/api/ERPVatTuCBM/Get?action=GetItemReturn&para1=${isNPL}`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = `<option value="">Tất cả</option>`;
        data.map(x => {
            html += `
                <option value="${x.MaNPL}">${x.Display}</option>
            `

        })
        $("#filterItemCode").html(html)
        $("#filterItemCode").select2()
        GetDanhSachReturn()
    } catch (error) {
        console.error(error.message);
    }
}

$(document).on("click", ".btn_itemXoaKe", function () {
    var idKe = $(this).data("keid")
    console.log(idKe)
    showConfirmModalDeleteRollInKe(async function () {
        await DeleteCBM("DeleteRollKe", idKe)
    })
})
async function DeleteCBM(action, para) {

    const request = new Request(
        `/api/ERPVatTuCBM/Delete?action=${action}&para=${para}`,
        {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            }
        }
    );

    let response = await fetch(request);
    let data = await response.json();

    if (data == "True") {
        showToast('success', `Xóa vật tư thành công! `, 1500);
        GetViTriKho()
    } else {
        showToast('error', `Xóa vật tư thất bại!`, 2000);
        return;
    }
}
$(document).on("click", ".btn_itemXoaDay", function () {
    var idDay = $(this).data("dayid")
    showConfirmModalDeleteRollInDay(async function () {
        await DeleteCBM("DeleteRollDay", idDay)
    })
})
function showConfirmModalDeleteRollInKe(onConfirm) {
    $('#confirmDeleteRollInKe').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#modalXoaKien").modal("hide");
    });

    $("#modalXoaKien").modal("show");
}

function showConfirmModalDeleteRollInDay(onConfirm) {
    $('#confirmDeleteRollInDay').off('click').on('click', async function () {
        if (typeof onConfirm === 'function') {
            const result = await onConfirm();

            if (result === false) {
                return; // ❌ KHÔNG đóng modal
            }
        }

        // ✅ Chỉ đóng khi hợp lệ
        $("#modalXoaKe").modal("hide");
    });

    $("#modalXoaKe").modal("show");
}

let dxChiTietVatTu;
function createViewDxDataTongQuan(data) {
    const sumTonKho = data
        .reduce((sum, x) => sum + (x.SLNK || 0), 0);

    const sumXuatHang = data
        .reduce((sum, x) => sum + (x.SLXH || 0), 0);

    dxChiTietVatTu = $("#dxChiTietVatTu").dxDataGrid({
        dataSource: data,
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
        width: "100%",
        renderAsync: false,
        grouping: { autoExpandAll: true },
        groupPanel: { visible: false },
        loadPanel: {
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            { dataField: "SoLo", caption: "PI NCC", alignment: "center", width: 90 },
            { dataField: "POMua", caption: "PO Mua", alignment: "center", width: 90 },
            { dataField: "MaVT", caption: "Item Code", alignment: "center", width: 110 },

            {
                dataField: "ChiTiet",
                caption: "Mô tả",
                alignment: "center",
                width: 300
            },

            { dataField: "MauVT", caption: "Màu", alignment: "center", width: 80 },
            { dataField: "SoLoT", caption: "LOT/BATCH", alignment: "center", width: 120 },
            { dataField: "KhoVai", caption: "Width/Size", alignment: "center", width: 90 },
            { dataField: "SoKien", caption: "Số Roll / Số Kiện", alignment: "center", width: 130 },

            {
                dataField: "BarCode",
                caption: "Barcode",
                alignment: "center",
                width: 160,
                visible: false,
                showInColumnChooser: false,
                allowFiltering: true,
                allowSorting: true
            },

            {
                dataField: "Status",
                caption: "Trạng thái",
                alignment: "center",
                width: 120,
                cellTemplate: function (container, options) {
                    const [text, color] =
                        options.value === 0 ? ["Nhập kho", "green"] :
                            options.value === 1 ? ["Xuất hàng", "red"] :
                                options.value === 2 ? ["Thu hồi", "orange"] :
                                    ["Chưa nhập kho", "gray"];

                    container.text(text).css({
                        color: color,
                        fontWeight: "bold"
                    });
                }
            },

            { dataField: "NgayNK", caption: "Ngày nhập kho", alignment: "center", width: 120 },

            {
                dataField: "SLNK",
                caption: "SL N.Kho",
                alignment: "center",
                width: 90,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    const value = options.value;
                    const status = options.data.Status;

                    const color =
                        status === 0 ? "green" :
                            status === 1 ? "red" :
                                status === 2 ? "orange" : "gray";

                    // Nếu value == 0 thì để trống
                    container
                        .text(value === 0 || value == null ? "" : value)
                        .css({
                            color: color,
                            fontWeight: "bold"
                        });
                }
            },


            { dataField: "NgayXH", caption: "Ngày xuất", alignment: "center", width: 120 },

            {
                dataField: "SLXH",
                caption: "SL Xuất",
                alignment: "center",
                width: 90,
                type: "number",
                format: "#.##0",
                cellTemplate: function (container, options) {
                    if (!options.value) return;
                    const status = options.data.Status;
                    const color = status === 0 ? "green" : status === 1 ? "red" : status === 2 ? "orange" : "gray";
                    container.text(options.value).css({ color, fontWeight: "bold" });
                }
            },

            { dataField: "NgayTK", caption: "Ngày T.Hồi", alignment: "center", width: 120 },
            {
                dataField: "SLTK", caption: "SL T.Hồi", alignment: "center", width: 90,
                cellTemplate: function (container, options) {
                    if (!options.value) return;
                    const status = options.data.Status;
                    const color = status === 0 ? "green" : status === 1 ? "red" : status === 2 ? "orange" : "gray";
                    container.text(options.value).css({ color, fontWeight: "bold" });
                },
                customizeText: function (e) {
                    return e.value === 0 ? "" : e.value;
                }
            },
            { dataField: "DonGia", caption: "Đơn giá", alignment: "center", width: 100, visible: false },
            { dataField: "ThanhTien", caption: "Thành tiền", alignment: "center", width: 120, visible: false },

            {
                caption: "Chi Tiết",
                alignment: "center",
                width: 70,
                visible: false,
                cellTemplate: function (container, options) {
                    const $icon = $('<i class="fa-solid fa-server" style="cursor:pointer;"></i>');
                    $icon.on("click", async function () {
                        const rowData = options.data;

                        if (rowData.Status === 0 || rowData.Status === 4) {
                            iziToast.warning({
                                message: `Cây vải ${rowData.SoKien} chưa được xuất hàng`,
                                position: 'topRight',
                                timeout: 2500
                            });
                            return;
                        }

                        $("#modalVatTuBC").text(rowData.ChiTiet || '');
                        $("#modalSoLoBC").text(rowData.SoLo || '');
                        $("#modalNgayNK").text(rowData.NgayNK || '');
                        $("#modalSLNK").text(rowData.SLNK || '');

                        await GetChiTietBarCode(rowData.BarCode || '');
                        $("#myModalDetail").modal("show");
                    });
                    container.append($icon);
                }
            }
        ],
        onContentReady: function (e) {
            setTimeout(function () {
                $("#dxChiTietVatTu .custom-footer-row").remove();

                const $footer = $(`
                    <div class="custom-footer-row" style="
                        position: sticky;
                        bottom: 0;
                        background: #f5f5f5;
                        font-weight: bold;
                        color: red;
                        padding: 8px 40px;
                        border-top: 2px solid #ddd;
                        z-index: 10;
                    ">
                        Tồn kho: ${parseFloat(sumTonKho.toFixed(4)).toLocaleString()}  -- Xuất hàng: ${parseFloat(sumXuatHang.toFixed(4)).toLocaleString()}
                    </div>
                `);

                $("#dxChiTietVatTu").append($footer);

            }, 100);
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
    }).dxDataGrid("instance");

    $("#Layer_1").click()
}


$("#barcodeDetailItem").on("keypress", function (e) {
    if (e.which === 13) {
        const barcode = $(this).val().trim();
        if (barcode) {
            GetVTBarCode(barcode);
        } else {
            showToast('warning', 'Vui lòng nhập mã barcode!');
        }
    }
});

$("#searchMoveBarcodeBtnDetailItem").on("click", function () {
    const barcode = $("#barcodeDetailItem").val().trim();
    if (barcode) {
        GetVTBarCode(barcode);
        $("#barcodeDetailItem").blur();
        $("#barcodeDetailItem").val('');
    } else {
        showToast('warning', 'Vui lòng nhập mã barcode!');
    }
});


async function GetVTBarCode(barcode) {
    const url = `/api/ViTriKhoNPL/Get?action=ChiTietSuDungRoll&para1=${barcode}`;
    try {
        const response = await fetch(url);
        const data = await response.json();

        if (data.Table.length === 0) {
            showToast("warning", "Barcode không tồn tại !");
            return;
        }

        // Đóng modal nếu đang mở
        const modalInstance = bootstrap.Modal.getInstance(document.getElementById('scannerModal'));
        if (modalInstance) {
            modalInstance.hide();
        }

        createViewDxDataTongQuan(data.Table);

    } catch (err) {
        console.error(err);
    }
}

$(function () {
    $('#openScannerBtnDetailItem').on('click', function () {
        window.scannerSource = 'detail';
        const scannerModal = new bootstrap.Modal($('#scannerModal')[0]);
        scannerModal.show();
    });

    $('#scannerModal').on('hidden.bs.modal', function () {
        $(".groupHidden").removeClass("d-none")
    });
})
