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
let slotID;
// Phát âm thanh lỗi/cảnh báo
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
    if (KH == null) {
    }
    else {
        $("#nguyenphulieuid").val(IsNPL).trigger("change")
    }
});

var warehouseData;
let currentFilter = 'all';
var warehouseDataChiTiet;

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
async function GetPOViTriKho() {
    const mahang = $("#mahangid").val()
    const url = `/api/KeHoachXuatHangThanhPham/GetThanhPham?Action=GetPO&Para1=${mahang}&Para2=Para`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        let html = `<option value="all">Tất cả</option>`;
        data.map(x => {
            html += `
                <option value="${x.POID}">${x.PO}</option>
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

function createViewLstDongThung() {
    const mockData = {

    }
}
function renderNhapKhoTable(data) {
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
                    return `${rowData.SoLo} - MH:${rowData.MaHang ?? ""} - Thùng: ${rowData.DisplayGroup} - Màu vật tư: ${rowData.MauVT}`;
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
                caption: "Số kiện",
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
                        ? options.data.CBM.toFixed(2)
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
                                value: cbmValue,
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
async function GetDanhSachO() {
    const url = `/api/KeHoachXuatHangThanhPham/GetThanhPham?action=Get&para1=3`;
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
        const response = await fetch(`/api/KeHoachXuatHangThanhPham/GetThanhPham?action=DetailO&para1=${slotId}&para2=3`);
        if (!response.ok) throw new Error(`Response status: ${response.status}`);
        const data = await response.json();
        console.log("slotId: ", slotId)
        console.log("data: ", data)
        // Kiểm tra data.Table có tồn tại không
        if (!data.Table || data.Table.length === 0) {
/*            console.warn(`Không có dữ liệu cho ô: ${slotId}`);
*/            return [];
        }

        return data.Table;
    } catch (error) {
        console.error('Lỗi khi lấy thông tin ô:', error.message);
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
            aislesMap[aisleId] = {
                id: aisleId,
                name: item.TenDay,
                description: `Khu vực ${item.TenDay}`,
                shelves: {}
            };
        }
        if (!aislesMap[aisleId].shelves[shelfId]) {
            aislesMap[aisleId].shelves[shelfId] = {
                id: shelfId,
                name: item.TenKe,
                textKe: item.TextKe || "",
                levels: {}
            };
        }
        if (!aislesMap[aisleId].shelves[shelfId].levels[levelId]) {
            aislesMap[aisleId].shelves[shelfId].levels[levelId] = {
                id: levelId,
                name: item.TenTang,
                textTang: item.TextTang || "",
                slots: []
            };
        }

        // CÔNG THỨC ĐÚNG
        const usedCBM = parseFloat((item.CBM - item.CBMCL).toFixed(2));

        aislesMap[aisleId].shelves[shelfId].levels[levelId].slots.push({
            id: item.TenO,
            cbm: item.CBM,
            usedCBM: usedCBM,
            count: item.CountO,
            cbmCL: item.CBMCL,
            nameO: item.TextO || "",
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
} function getCapacityClass(usedCBM, totalCBM) {
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
                    <div class="group-header" data-bs-toggle="collapse" data-bs-target="#${collapseId}" aria-expanded="true">
                        <h4><i class="bi bi-building-fill"></i> ${aisle.name}</h4>
                        <span class="toggle-icon">▼</span>
                    </div>
                    <div class="collapse show" id="${collapseId}">
                        <div class="card-body">`;

        aisle.shelves.forEach(shelf => {
            html += `
                <div class="shelf-section card mb-3">
                    <div class="card-header bg-secondary text-white py-2">
                        <h6 class="mb-0"><i class="bi bi-bookshelf"></i> ${shelf.name} ${shelf.textKe != "" ? `(${shelf.textKe})` : ""}</h6>
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

                level.slots.forEach(slot => {
                    const slotNumber = slot.id.split('.').pop();
                    const capacityClass = getCapacityClass(slot.usedCBM, slot.cbm);
                    const remainingCapacity = getRemainingCapacity(slot.usedCBM, slot.cbm);
                    const usedPercentage = Math.round(((slot.usedCBM / slot.cbm) * 100));
                    const itemCount = slot.count;
                    const cbmCL = parseFloat((parseFloat(slot.cbm.toFixed(2)) - parseFloat(slot.usedCBM.toFixed(2))).toFixed(2));

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
                                    <div class="slot-capacity">${parseFloat(slot.usedCBM.toFixed(2))}/${slot.cbm} CBM</div>
                                    <div class="slot-capacity">${cbmCL} CBMCL</div>
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
        <div><i class="bi bi-box"></i> CBM: ${parseFloat(usedCBM.toFixed(2))}/${cbm} (Còn: ${cbmCL})</div>
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
    .on("click", ".slot-item", async function () {
        $(".slot-item").removeClass("highlighted");
        $(this).addClass("highlighted");

        const slotId = $(this).data("id");
        await GetCBMThungTPSoLo(slotId)
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

// edit1.1
async function loadVTKho() {
    var slotId = solotIDBarCode;
    let dataThungTP = await GetCBMThungTPSoLo(slotId);
    const dataO = warehouseSlots.find(wh => wh.TenO === slotId);
    // Kiểm tra nếu không tìm thấy thông tin ô trong warehouseSlots
    if (!dataO) {
        $('#modalSlotTitle').html(`
            <i class="bi bi-box"></i> Chi tiết ô kho ${slotId}
        `);
        $('#modalSlotInfo').html(`
            <div class="fs-5 fw-bold text-primary">${slotId}</div>
        `);
        $('#modalCapacityInfo').html(`
            <div class="fs-5 fw-bold text-muted">Không tìm thấy thông tin ô</div>
        `);
        $('#modalItemList').html(`
            <div class="text-center text-muted py-4">
                <i class="bi bi-inbox fs-1"></i>
                <div class="mt-2">Không có dữ liệu</div>
            </div>
        `);
        return;
    }

    // Tính toán CBM
    const cbmCL = parseFloat(dataO.CBMCL) || 0;  // Sức chứa còn lại
    const cbmSD = parseFloat(dataO.CBMSD) || 0;  // CBM đã sử dụng

    // Tính tổng sức chứa ban đầu của ô
    const cbmTotal = cbmCL + cbmSD;

    // Set modal title và info
    $('#modalSlotTitle').html(`
        <i class="bi bi-box"></i> Chi tiết ô kho ${slotId}
    `);

    $('#modalSlotInfo').html(`
        <div class="fs-5 fw-bold text-primary">${slotId}</div>
        ${dataO.TextKe ? `<div class="text-muted small d-none">${dataO.TextKe}</div>` : ''}
        ${dataO.TenTang ? `<div class="text-muted small d-none">${dataO.TenTang}</div>` : ''}
    `);

    // Tính capacity class
    const capacityClass = getCapacityClass(cbmSD, cbmTotal);
    let capacityColor = 'success';
    if (capacityClass === 'capacity-empty') capacityColor = 'danger';
    else if (capacityClass === 'capacity-low') capacityColor = 'warning';
    else if (capacityClass === 'capacity-medium') capacityColor = 'info';

    // Lưu sức chứa còn lại
    succhuaOCL = cbmCL;

    // Hiển thị thông tin CBM
    $('#modalCapacityInfo').html(`
        <div class="fs-5 fw-bold text-${capacityColor}">
            ${parseFloat(cbmSD.toFixed(4))}/${parseFloat(cbmTotal.toFixed(4))} CBM
        </div>
        <div class="small text-muted d-none">
            Còn lại: ${cbmCL.toFixed(4)} CBM
        </div>
    `);

    // Render danh sách thùng
    let itemsHtml = "";
    if (dataThungTP && dataThungTP.length > 0) {
        const groupedData = dataThungTP.reduce((acc, item) => {
            const displayGroup = `${item.MaDH}_${item.PO}_${item.MaPKL}_${item.DauSize}_${item.Size}`
            if (!acc[displayGroup]) {
                acc[displayGroup] = {
                    items: [],
                    MaDH: item.MaDH,
                    PO: item.PO,
                    MaPKL: item.MaPKL,
                    DauSize: item.DauSize,
                    TenMau: item.TenMau,
                    Size: item.Size,
                    totalCBM: 0,
                    totalSoThung: 0
                }
            }
            acc[displayGroup].items.push(item);
            acc[displayGroup].totalCBM += parseFloat(item.CBM || 0);
            acc[displayGroup].totalSoThung += 1;
            return acc;

        }, {});

        itemsHtml = Object.entries(groupedData).map(([displayGroup, group], groupIndex) => {
            const groupId = `group-${groupIndex}`
            const itemsRows = group.items.map(item => `
             <div class="col-2 item-thung-base item-thung-exist position-relative"
                     data-index="${item.SttThung}"
                     data-qrscan="${item.qrscan}"
                     tabindex="0"
                     data-bs-toggle="tooltip"
                     data-bs-html="true"
                      title="Số lượng: ${item.SLSP}">
                  
                    <h6>${item.CBM}</h6>
                    <hr/>
                    <div class="position-relative d-flex justify-content-around align-items-center">
                        <i class="fa-thin fa-box" style="color:#492FF6;font-size:28px;transform: scaleX(1.28);"></i>
                        <small class=" position-absolute" style="font-size: 10px;color:#492FF6;  top: 58%;left: 50%;transform: translate(-50%, -55%);font-size: 10px;color: #492FF6;font-weight: 400;pointer-events: none;">${item.SttThung}</small>
                    </div>
                </div>
        `).join('');
            return `
            <div class="collapsible-group mb-3 p-0">
                <div class="group-header d-flex justify-content-between align-items-center"
                     style="background:linear-gradient(45deg,#3B82F6, #9c71ffcc);"
                     data-bs-toggle="collapse"
                     data-bs-target="#${groupId}"
                     role="button"
                     aria-expanded="false">
                    <div>
                        <h6 class="mb-1" style="font-size: 13px">Mã ĐH: ${group.MaDH} - PO: ${group.PO} - MaPKL: ${group.MaPKL} - Đầu Size: ${group.DauSize} - Màu: ${group.TenMau} - Size: ${group.Size}</h6>
                        <small class="text-muted" style="color: #fff500 !important; font-weight: bold; font-size: 13px">CBM: ${group.totalCBM.toFixed(4)} CBM</small>
                        -
                        <small class="text-muted" style="color: white !important; font-size: 13px">Tổng số kiện: ${group.totalSoThung}</small>

                    </div>
                    <span class="toggle-icon">▼</span>
                </div>
                
                <div class="collapse show" id="${groupId}">
                    <div class="item-card row gap-3 mb-1 mt-1" style="padding: 8px 22px">
                        ${itemsRows}
                    </div>
                </div>
            </div>
        `;
        }).join('');
    } else {
        itemsHtml = `
            <div class="text-center text-muted py-4">
                <i class="bi bi-inbox fs-1"></i>
                <div class="mt-2">Ô kho trống</div>
                <div class="small">Sức chứa: ${cbmTotal.toFixed(2)} CBM</div>
            </div>`;
    }

    $('#modalItemList').html(itemsHtml);

    $('[data-bs-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });
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
        $('#cbmResult').text(0.00);
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
                showToast('warning', 'Vui lòng nhập CBM của roll!');
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
        const tongCBM = parseFloat($(".tongCBM").text()).toFixed(2)
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
    warehouseDataChiTiet.forEach(row => {
        const match =
            (row.DisplayGroup && row.DisplayGroup.toLowerCase().includes(lowerSearch)) ||
            (row.MauVT && row.MauVT.toLowerCase().includes(lowerSearch)) ||
            (row.DauSize && row.DauSize.toLowerCase().includes(lowerSearch)) ||
            (row.SttThung !== null && String(row.SttThung ?? '').toLowerCase().includes(lowerSearch)) ||
            (row.BarCode && row.BarCode.toLowerCase().includes(lowerSearch)) ||
            (row.NgayNhapKho && row.NgayNhapKho.toLowerCase().includes(lowerSearch)) ||
            (row.CBM !== null && String(row.CBM ?? '').toLowerCase().includes(lowerSearch))
            ;

        if (match) {
            results.push({
                slot: {
                    id: row.MaONPL,
                    cbm: row.CBM,
                    barcode: row.BarCode,
                    displayGroup: row.DisplayGroup,
                    usedCBM: row.CBMSD,
                    cbmO: row.CBMO,
                    ngayNhapKho: row.NgayNhapKho,
                    chiTiet: row.ChiTiet,
                    mauVT: row.MauVT,
                    maVT: row.MaVT,
                    DauSize: row.DauSize,
                    SttThung: row.SttThung
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
                    <div class="mt-2 text-muted">Không tìm thấy thùng "${searchTerm}"</div>
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
                    <small><i class="bi bi-search"></i> Tìm thấy ${results.length} kết quả cho "${searchTerm}"</small>
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
                                     Màu: ${highlightSearchTerm(result.slot.mauVT, searchTerm)} -  Đầu size: ${highlightSearchTerm(result.slot.DauSize, searchTerm)}
                            </div>
                                
                            <div class="item-info text-muted">
                                Thùng: ${highlightSearchTerm(result.slot.SttThung, searchTerm)} - CBM: ${highlightSearchTerm(result.slot.cbm, searchTerm)}
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
    // Kiểm tra text trước khi xử lý
    if (!text || text === null || text === undefined) {
        return '';
    }

    // Chuyển về string để đảm bảo an toàn
    text = String(text);

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
// Thêm biến global để lưu giá trị ô nguồn và ô đích
let currentFromSlotId = "";
let currentToSlotId = "";

async function showMoveItemModal(fromSlotId, toSlotId) {
    // Lưu vào biến global
    currentFromSlotId = fromSlotId;
    currentToSlotId = toSlotId;

    const modal = new bootstrap.Modal($('#moveItemModal')[0]);
    modal.show();

    $("#fromSlotName").text(fromSlotId);
    $("#toSlotName").text(toSlotId);

    const fromItems = await fetchSlotItems(fromSlotId);
    const toItems = await fetchSlotItems(toSlotId);

    // Lấy thông tin CBM từ warehouseSlots thay vì dựa vào items
    const fromSlotInfo = warehouseSlots.find(s => s.TenO === fromSlotId);
    const toSlotInfo = warehouseSlots.find(s => s.TenO === toSlotId);
    const fromCBMCL = fromSlotInfo?.CBMCL || 0;
    const fromCBMSD = fromSlotInfo?.CBMSD || 0;
    const toCBMO = toSlotInfo?.CBMCL || 0;
    const toCBMSD = toSlotInfo?.CBMSD || 0;


    $("#fromSlotNameCBM").text(fromCBMCL.toFixed(2));
    $("#totalCBM").text(fromCBMSD.toFixed(2));
    $("#toSlotNameCBM").text(toCBMO.toFixed(2));
    $("#totalToCBMDC").text(toCBMSD.toFixed(2));

    renderSlotItems("fromSlotItems", fromItems, "from");
    renderSlotItems("toSlotItems", toItems, "to");

    initMoveModalBarcodeScanner();

    $("#btnMoveToRight").off('click').on('click', function () {
        moveCheckedItems("fromSlotItems", "toSlotItems");
    });

    $("#btnConfirmMove").off('click').on('click', function () {
        const movedItems = collectSlotItems("toSlotItems");
        showToast('success', `Đã di chuyển thành công ${movedItems.length} thùng!`, 1500);
        modal.hide();
    });
}
// Khởi tạo scanner barcode cho modal di chuyển
function initMoveModalBarcodeScanner() {
    // Xử lý tìm kiếm barcode từ button
    $('#searchMoveBarcodeBtnLeft, #searchMoveBarcodeBtnRight').off('click').on('click', function () {
        const side = $(this).data('side');
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
async function searchBarcodeInMoveModal(barcode) {
    return new Promise((resolve) => {
        // 1. Tìm thùng theo qrscan trong ô cần chuyển
        const $thung = $(`.item-thung-move[data-qrscan="${barcode}"]`);

        if ($thung.length === 0) {
            showToast('error', `Không tìm thấy barcode "${barcode}" trong Ô cần chuyển!`);
            PlayAudioError();
            return resolve(false);
        }

        const itemData = $thung.data("item");
        $("#txtStt").text(itemData.SttThung);
        $("#txtDH").text(itemData.MaDH);
        $("#txtPO").text(itemData.POID);
        $("#txtMaPKL").text(itemData.MaPKL);
        $("#txtSLSP").text(itemData.SLSP);

        // 2. Tính CBM
        const itemCBM = Number(itemData.CBM) || 0;
        const toCBMRemaining = Number($('#toSlotNameCBM').text()) || 0;
        const currentSelectedCBM = Number($('#totalCBMDC').text()) || 0;

        // 3. Kiểm tra đã chọn chưa
        const isSelected = lstCBMThungMove.some(x => isSameThung(x, itemData));

        // ================= BỎ CHỌN =================
        if (isSelected) {
            $thung.removeClass('selected');
            $thung.find('.tick-icon').hide();

            lstCBMThungMove = lstCBMThungMove.filter(x => !isSameThung(x, itemData));
            lstDataLichSuQuet.push(itemData);
            showToast('success', `Đã bỏ chọn thùng : ${itemData.SttThung} (${itemCBM.toFixed(4)} CBM)`);

            // ===== UPDATE GRID =====
            dxGridLichSuQuet.beginUpdate();
            dxGridLichSuQuet.option("dataSource", lstDataLichSuQuet);
            dxGridLichSuQuet.endUpdate();

            PlayAudio();
            updateTotalCBM();
            return resolve(true);
        }

        // ================= KIỂM TRA CBM =================
        if ((currentSelectedCBM + itemCBM) > toCBMRemaining) {
            showToast(
                'warning',
                `Không đủ sức chứa! CBM thùng: ${itemCBM.toFixed(4)}, Còn lại: ${toCBMRemaining.toFixed(4)}`
            );
            PlayAudioError();

            $thung.css('background', '#ffe6e6');
            setTimeout(() => $thung.css('background', ''), 1500);

            return resolve(false);
        }

        // ================= CHỌN =================
        $thung.addClass('selected');
        $thung.find('.tick-icon').show();

        lstCBMThungMove.push(itemData);

        // ===== THÊM VÀO LỊCH SỬ QUÉT =====
        lstDataLichSuQuet.push(itemData);

        // ===== UPDATE GRID =====
        dxGridLichSuQuet.beginUpdate();
        dxGridLichSuQuet.option("dataSource", lstDataLichSuQuet);
        dxGridLichSuQuet.endUpdate();

        showToast('success', `Đã chọn thùng : ${itemData.SttThung} (${itemCBM.toFixed(4)} CBM)`);
        PlayAudio();

        $thung[0].scrollIntoView({ behavior: 'smooth', block: 'center' });

        updateTotalCBM();
        resolve(true);
    });
}
function updateTotalCBM() {
    const total = lstCBMThungMove.reduce((sum, x) => {
        return sum + (Number(x.CBM) || 0);
    }, 0);

    tongCBMDC = total;
    $("#totalCBMDC").text(total.toFixed(2));
    updateAddButtonVisibility();
}

// edit1
function renderSlotItems(containerId, items, side) {
    const $container = $('#' + containerId);
    const isFromSlotItems = containerId === "fromSlotItems";

    $container.empty();

    if (!items || items.length === 0) {
        $container.html(`<div class="text-center text-muted py-3 text-rong"><i class="bi bi-inbox"></i> Không có vật thùng</div>`);
        return;
    }

    // Group theo MaDH_PO_MaPKL_DauSize_Size
    const groupedData = items.reduce((acc, item) => {
        const displayGroup = `${item.MaDH}_${item.PO}_${item.MaPKL}_${item.DauSize}_${item.Size}`;
        if (!acc[displayGroup]) {
            acc[displayGroup] = {
                items: [],
                MaDH: item.MaDH,
                PO: item.PO,
                MaPKL: item.MaPKL,
                DauSize: item.DauSize,
                TenMau: item.TenMau,
                Size: item.Size,
                totalCBM: 0,
                totalSoThung: 0
            };
        }
        acc[displayGroup].items.push(item);
        acc[displayGroup].totalCBM += parseFloat(item.CBM || 0);
        acc[displayGroup].totalSoThung += 1;
        return acc;
    }, {});

    Object.entries(groupedData).forEach(([displayGroup, group], groupIndex) => {
        const groupId = `${containerId}-group-${groupIndex}`;

        const itemsHtml = group.items.map(item => `
            <div class="col-2 item-thung-base ${isFromSlotItems ? 'item-thung-move' : 'item-thung-to'} position-relative"
                 data-index="${item.SttThung}"
                 data-qrscan="${item.qrscan}"
                 tabindex="0"
                 data-bs-toggle="tooltip"
                 data-bs-html="true"
                 title="+ Số lượng: ${item.SLSP}">
                ${isFromSlotItems ? `
                    <i class="fa-solid fa-circle-check position-absolute tick-icon"
                       style="top: -10px; right: -4px;color: rgb(25, 135, 84); font-size: 18px;display:none;"></i>
                ` : ""}
                <h6>${item.CBM}</h6>
                <hr/>
                <div class="position-relative d-flex justify-content-around align-items-center">
                    <i class="fa-thin fa-box" style="color:${isFromSlotItems ? '#0d6efd' : '#198754'};font-size:28px;transform: scaleX(1.28);"></i>
                    <small class="position-absolute" style="font-size:10px;color:${isFromSlotItems ? '#0d6efd' : '#198754'};top:58%;left:50%;transform:translate(-50%,-55%);font-weight:400;pointer-events:none;">
                        ${item.SttThung}
                    </small>
                </div>
            </div>
        `).join('');

        const $group = $(`
            <div class="collapsible-group mb-3 p-0">
                <div class="group-header d-flex justify-content-between align-items-center"
                     style="background:linear-gradient(45deg,#3B82F6,#9c71ffcc);"
                     data-bs-toggle="collapse"
                     data-bs-target="#${groupId}"
                     role="button"
                     aria-expanded="true">
                    <div>
                        <h6 class="mb-1" style="font-size:13px">
                            Mã ĐH: ${group.MaDH} - PO: ${group.PO} - MaPKL: ${group.MaPKL} - Đầu Size: ${group.DauSize} - Màu: ${group.TenMau} - Size: ${group.Size}
                        </h6>
                        <small style="color:#fff500;font-weight:bold;font-size:13px">CBM: ${group.totalCBM.toFixed(4)} CBM</small>
                        -
                        <small style="color:white;font-size:13px">Tổng số kiện: ${group.totalSoThung}</small>
                    </div>
                    <span class="toggle-icon">▼</span>
                </div>
                <div class="collapse show" id="${groupId}">
                    <div class="item-card row gap-3 mb-1 mt-1" style="padding:8px 22px">
                        ${itemsHtml}
                    </div>
                </div>
            </div>
        `);

        // Gán data("item") cho từng thùng sau khi append
        $group.find('.item-thung-move, .item-thung-to').each(function (i) {
            $(this).data("item", group.items[i]);
        });

        $container.append($group);
    });

    $('[data-bs-toggle="tooltip"]').tooltip('dispose').tooltip();
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

    $("#totalCBMDC").text(value.toFixed(2));
});

function moveCheckedItems(fromContainerId, toContainerId) {
    if (lstCBMThungMove.length === 0) {
        showToast('warning', `Vui lòng chọn ít nhất một thùng để chuyển!`, 1500);
        return;
    }

    const totalCBMMove = parseFloat($("#totalCBMDC").text()) || 0;
    const toCBMRemaining = parseFloat($("#toSlotNameCBM").text()) || 0;

    if (totalCBMMove > toCBMRemaining) {
        showToast('error',
            `Không thể chuyển! Tổng CBM (${totalCBMMove.toFixed(2)}) vượt quá sức chứa còn lại (${toCBMRemaining.toFixed(2)} CBM).`,
            3000
        );
        return;
    }

    // Dùng biến global thay vì select box
    const fromOid = currentFromSlotId;
    const toOid = currentToSlotId;

    // Validate
    if (!fromOid || !toOid) {
        showToast('error', 'Không tìm thấy thông tin ô nguồn/đích!', 2000);
        console.error("Missing slot IDs:", { fromOid, toOid });
        return;
    }

    var SaveFromTo = [];

    lstCBMThungMove.forEach(item => {
        let object = {
            Barcode: item.qrscan || "",
            MaONPLFrom: fromOid || "",
            MaONPLTo: toOid || ""
        };
        SaveFromTo.push(object);
    });

    // Gọi API
    SaveFormToONPL(SaveFromTo);
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
    var tenOTo = $("#vattuden").val()
    if (!tenOFrom) {
        showToast('warning', `Vui lòng chọn ô cần chuyển `, 2000);
        return
    }
    if (!tenOTo) {
        showToast('warning', `Vui lòng chọn ô nhận thùng `, 2000);
        return
    }
    if (tenOFrom == tenOTo) {
        showToast('warning', `Vui lòng chọn ô cần chuyển khác ô nhận thùng!`, 2000);
        return
    }
    showMoveItemModal(tenOFrom, tenOTo);
}
async function SaveFormToONPL(arrSave) {
    // Validate data
    const hasInvalidData = arrSave.some(item =>
        !item.Barcode || item.Barcode === "" ||
        !item.MaONPLTo || item.MaONPLTo === ""
    );

    if (hasInvalidData) {
        showToast('error', 'Dữ liệu không hợp lệ! Thiếu Barcode hoặc Ô đích.', 2000);
        console.error("Invalid data:", arrSave);
        return;
    }

    var dataUser = !window.CefSharp ? userNameSave : userName;

    try {
        const response = await fetch(
            `/api/KeHoachXuatHangThanhPham/PostThungThanhPham?action=UpdateONPKL&para1=${dataUser}`,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(arrSave),
            }
        );

        const data = await response.json();

        // Dùng biến global
        const fromOid = currentFromSlotId;
        const toOid = currentToSlotId;

        if (data == "True") {
            lstCBMThungMove = [];
            tongCBMDC = 0;
            $("#totalCBMDC").text("0.00");

            showToast('success', `Chuyển ${arrSave.length} thùng thành công!`, 1500);
            PlayAudio();
            await GetViTriKho();
            await GetDanhSachO();
            await GetOSugget();

            // Reload modal với giá trị từ biến global
            await reloadMoveModalData(fromOid, toOid);

        } else {
            showToast('error', `Chuyển đổi ô thất bại!`, 2000);
            console.error("API response:", data);
        }
    } catch (error) {
        console.error("Error calling API:", error);
        showToast('error', 'Có lỗi xảy ra khi gọi API!', 2000);
    }
}
async function reloadMoveModalData(fromSlotId, toSlotId) {
    // Fetch lại data mới từ server
    const fromItems = await fetchSlotItems(fromSlotId);
    const toItems = await fetchSlotItems(toSlotId);

    // Cập nhật lại thông tin CBM
    const fromCBMO = fromItems[0]?.CBMO ?? 0;
    const fromCBMSD = fromItems[0]?.CBMSD ?? 0;
    const toCBMO = toItems[0]?.CBMO ?? 0;
    const toCBMSD = toItems[0]?.CBMSD ?? 0;

    $("#fromSlotNameCBM").text((fromCBMO - fromCBMSD).toFixed(2));
    $("#totalCBM").text(fromCBMSD.toFixed(2));
    $("#toSlotNameCBM").text((toCBMO - toCBMSD).toFixed(2));
    $("#totalToCBMDC").text(toCBMSD.toFixed(2));

    // Clear và render lại 2 bên
    $("#fromSlotItems").empty();
    $("#toSlotItems").empty();

    renderSlotItems("fromSlotItems", fromItems, "from");
    renderSlotItems("toSlotItems", toItems, "to");

    // Kiểm tra nếu ô nguồn hết thùng
    if (!fromItems || fromItems.length === 0) {
        showToast('info', 'Ô nguồn đã hết thùng!', 2000);
    }

    // Re-init tooltips
    $('[data-bs-toggle="tooltip"]').tooltip('dispose').tooltip();
}
// Gợi ý
var warehouseSlots = []
async function GetOSugget() {
    warehouseSlots = []
    const url = `/api/KeHoachXuatHangThanhPham/GetThanhPham?action=GetSuggetO`;
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
    await GetOSugget()
    if (lstThung.length == 0) {
        showToast('error', `Vui lòng chọn thùng để đưa vào kho!`, 2000);
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
    const totalCBM = parseFloat(lstThung.reduce((sum, item) => sum + item.CBM, 0).toFixed(2));

    $('#totalCBMNeed').text(totalCBM);
    $('#totalRolls').text(lstThung.length);


    const uniqueMaGop = [...new Set(lstThung.map(item => item.MaGop))];

    // Tìm ô đã chứa cùng loại thùng
    const sameTypeSlotGroups = [];
    $.each(uniqueMaGop, function (index, maGop) {
        const itemsOfType = lstThung.filter(item => item.MaGop === maGop);
        const totalCBMOfType = itemsOfType.reduce((sum, item) => sum + item.CBM, 0);
        const availableSlots = warehouseSlots
            .filter(slot => slot.MaGop === maGop && slot.CBMCL > 0)
            .sort((a, b) => b.CBMCL - a.CBMCL)
            .filter((slot, index, self) =>
                index === self.findIndex(s => s.TenO === slot.TenO)
            );

        const selectedSlots = findOptimalSlotCombination(availableSlots, parseFloat(totalCBMOfType.toFixed(2)));
        if (selectedSlots.length > 0) {
            sameTypeSlotGroups.push({
                maGop: maGop,
                slots: selectedSlots,
                totalRequired: totalCBMOfType,
                totalAvailable: selectedSlots.reduce((sum, s) => sum + s.CBMCL, 0),
                itemCount: itemsOfType.length
            });
        }
    });

    const emptySlots = warehouseSlots
        .filter(slot => slot.MaGop === null && slot.CBMCL > 0)
        .sort((a, b) => b.CBMCL - a.CBMCL);
    const emptySlotGroups = findOptimalSlotCombination(emptySlots);

    // Render
    renderSameTypeSlots(sameTypeSlotGroups);
    renderEmptySlots(emptySlotGroups, totalCBM, lstThung);
}
function findOptimalSlotCombination(slots) {
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
        const itemCBM = parseFloat(item.CBM.toFixed(4));
        for (let slot of allocated) {
            const usedCBM = parseFloat(
                slot.assignedItems.reduce((sum, i) => sum + i.CBM, 0).toFixed(4)
            );
            const slotCBMCL = parseFloat(slot.CBMCL.toFixed(4));

            if (slotCBMCL >= parseFloat((usedCBM + itemCBM).toFixed(4))) {
                slot.assignedItems.push(item);
                break;
            }
        }
    });

    return allocated;
}

function allocateItemsToSameTypeSlots(slots, items) {
    console.log("slots allocateItemsToSameTypeSlots: ", slots)
    const allocated = slots.map(slot => ({
        ...slot,
        assignedItems: []
    }));

    let remainingItems = [...items];

    for (let slot of allocated) {
        let availableSpace = parseFloat(slot.CBMCL.toFixed(4));
        let i = 0;

        while (i < remainingItems.length && availableSpace > 0) {
            const item = remainingItems[i];
            const itemCBM = parseFloat(item.CBM.toFixed(4));
            if (itemCBM <= availableSpace) {
                slot.assignedItems.push(item);
                availableSpace = parseFloat((availableSpace - itemCBM).toFixed(4));
                remainingItems.splice(i, 1);
            } else {
                i++;
            }
        }
    }

    return allocated;
}

function renderSameTypeSlots(slotGroups) {
    console.log("slotGroups: ", slotGroups)
    const $container = $('#sameTypeSlots');

    if (slotGroups.length === 0) {
        $container.html(`
            <div class="no-data-placeholder">
                <i class="bi bi-inbox fs-1"></i>
                <div class="mt-2">Không có ô nào đang chứa cùng loại thùng</div>
            </div>
        `);
        return;
    }

    slotGroups.sort((a, b) => a.maGop.localeCompare(b.maGop, 'vi', { sensitivity: 'base' }));

    let html = '';
    $.each(slotGroups, function (groupIndex, group) {
        const shortName = group.maGop;
        const itemsOfType = lstThung.filter(item => item.MaGop === group.maGop);
        const allocatedSlots = allocateItemsToSameTypeSlots(group.slots, itemsOfType);

        html += `
            <div class="slot-group">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h6 class="mb-0 text-primary">
                        <i class="bi bi-box-seam"></i> ${shortName}
                    </h6>
                    <span class="combination-badge">
                       ${group.totalRequired.toFixed(2)} CBM cần lưu
                    </span>
                </div>
                <div class="row g-3">
        `;

        $.each(allocatedSlots, function (index, slot) {
            const totalThungInSlot = slot.assignedItems.length;
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
                                <small class="text-success fw-bold">Cùng loại thùng</small>
                            </div>

                            <div class="slot-info">
                                <i class="bi bi-inbox"></i>
                                <small><strong>Còn trống:</strong> ${slot.CBMCL.toFixed(2)} CBM</small>
                            </div>

                            <div class="slot-info">
                                <i class="bi bi-stack"></i>
                                <small><strong>Đang chứa:</strong> ${slot.CBMSD.toFixed(2)} CBM</small>
                            </div>

                            ${slot.assignedItems.length > 0 ? `
                                <div class="alert alert-success mt-3 mb-2 py-2">
                                    <div class="fw-bold small mb-1">
                                        <i class="bi bi-arrow-down-circle"></i> Sẽ nhận:
                                    </div>
                                    <div class="text-dark mb-2">
                                        <i class="bi bi-stack"></i> <strong>${totalThungInSlot} Thùng</strong> - 
                                        <strong>${parseFloat(totalCBMInSlot).toFixed(2)} CBM</strong>
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
                                            <i class="bi bi-box-seam"></i> Thùng số: ${item.SttThung} - SLSP: ${item.SLSP}
                                        </span>
                                        <span class="text-muted">
                                          ${item.CBM.toFixed(2)}
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
                        (${totalAvailable.toFixed(2)} CBM)
                    </button>
                </div>
            </div>
        `;
    });

    $container.html(html);
}

function renderEmptySlots(slots, totalRequired, lstThung) {
    console.log("slots: ", slots)
    const $container = $('#emptySlots');

    if (slots.length === 0) {
        $container.html(`
            <div class="no-data-placeholder">
                <i class="bi bi-exclamation-triangle fs-1"></i>
                <div class="mt-2">Không có đủ ô trống để chứa ${totalRequired.toFixed(2)} CBM</div>
                <small class="text-muted">Vui lòng chọn ít thùng hơn hoặc giải phóng ô khác</small>
            </div>
        `);
        return;
    }

    // ===== ƯU TIÊN CÙNG KỆ - THU THẬP TẤT CẢ CÁC KỆ =====
    const shelfRecommendations = new Map();

    lstThung.forEach(item => {
        const maGop = item.MaGop;

        // Lấy TẤT CẢ thùng cùng loại đã có trong kho
        const existingBoxes = lstCBMThungTP.filter(stored =>
            stored.MaGop === maGop && stored.MaONPL
        );

        if (existingBoxes.length > 0) {
            // Thu thập TẤT CẢ các kệ đang chứa loại thùng này
            const shelvesForThisType = new Set();

            existingBoxes.forEach(box => {
                const shelf = extractShelfFromSlot(box.MaONPL);
                if (shelf) {
                    shelvesForThisType.add(shelf);
                }
            });

            // Lưu tất cả các kệ cho MaGop này
            if (!shelfRecommendations.has(maGop)) {
                shelfRecommendations.set(maGop, new Set());
            }

            shelvesForThisType.forEach(shelf => {
                shelfRecommendations.get(maGop).add(shelf);
            });
        }
    });

    // ===== PHÂN BỔ THEO THỨ TỰ ƯU TIÊN =====
    let allocatedSlots = [];
    let remainingItems = [...lstThung];

    shelfRecommendations.forEach((shelves, maGop) => {
        shelves.forEach(shelfCode => {
            const itemsOfType = remainingItems.filter(item => item.MaGop === maGop);

            if (itemsOfType.length === 0) return;

            const slotsInSameShelf = slots
                .filter(slot => extractShelfFromSlot(slot.TenO) === shelfCode)
                .sort((a, b) => {
                    const levelA = extractLevelFromSlot(a.TenO);
                    const levelB = extractLevelFromSlot(b.TenO);

                    if (levelA !== levelB) {
                        return levelA - levelB;
                    }

                    return b.CBMCL - a.CBMCL;
                });

            if (slotsInSameShelf.length > 0) {
                const allocated = allocateItemsToSlots(slotsInSameShelf, itemsOfType);
                allocatedSlots.push(...allocated);

                allocated.forEach(slot => {
                    slot.assignedItems.forEach(assignedItem => {
                        const index = remainingItems.findIndex(item =>
                            isSameThung(item, assignedItem)
                        );
                        if (index !== -1) {
                            remainingItems.splice(index, 1);
                        }
                    });
                });
            }
        });
    });

    if (remainingItems.length > 0) {
        const usedSlotIds = allocatedSlots.map(s => s.TenO);
        const remainingSlots = slots.filter(slot => !usedSlotIds.includes(slot.TenO));

        const allocated = allocateItemsToSlots(remainingSlots, remainingItems);
        allocatedSlots.push(...allocated);
    }

    // ===== RENDER HTML - CHECK TẤT CẢ CÁC KỆ =====
    let html = `
        <div class="slot-group">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h6 class="mb-0 text-info">
                    <i class="bi bi-inbox-fill"></i> Phân bổ tối ưu - ${allocatedSlots.filter(s => s.assignedItems?.length > 0).length} ô trống
                </h6>
                <span class="combination-badge">
                    ${totalRequired.toFixed(2)} CBM cần lưu
                </span>
            </div>

            <div class="row g-3">
    `;

    $.each(allocatedSlots, function (index, slot) {
        const totalThungInSlot = slot.assignedItems.length;
        const totalCBMInSlot = slot.assignedItems.reduce((sum, i) => sum + i.CBM, 0);

        if ((slot.assignedItems || []).length == 0) return;

        const slotShelf = extractShelfFromSlot(slot.TenO);

        // CHECK TẤT CẢ CÁC KỆ ĐANG CHỨA CÙNG LOẠI THÙNG
        const isSameShelf = slot.assignedItems.some(item => {
            const existingBoxes = lstCBMThungTP.filter(stored =>
                stored.MaGop === item.MaGop && stored.MaONPL
            );

            if (existingBoxes.length > 0) {
                // Kiểm tra xem có BẤT KỲ kệ nào khớp không
                return existingBoxes.some(box => {
                    const existingShelf = extractShelfFromSlot(box.MaONPL);
                    return existingShelf === slotShelf;
                });
            }
            return false;
        });

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
                                <small><strong>Kệ:</strong> ${slot.TextKe} ${isSameShelf ? '<span class="badge bg-success">Cùng kệ</span>' : ''}</small>
                            </div>
                        ` : ''}

                        <div class="slot-info">
                            <i class="bi bi-inbox"></i>
                            <small><strong>Dung lượng:</strong> ${slot.CBMCL.toFixed(2)} CBM</small>
                        </div>

                        ${slot.assignedItems.length > 0 ? `
                            <div class="alert ${isSameShelf ? 'alert-success' : 'alert-info'} mt-3 mb-2 py-2">
                                <div class="fw-bold small mb-1">
                                    <i class="bi bi-arrow-down-circle"></i> Sẽ nhận:
                                </div>
                                <div class="text-dark mb-2">
                                    <i class="bi bi-stack"></i> <strong>${totalThungInSlot} Thùng</strong> - 
                                    <strong>${parseFloat(totalCBMInSlot).toFixed(2)} CBM</strong>
                                </div>
                                <hr class="my-2">
                                <div class="small">
                        ` : ''}
                        
                        ${slot.assignedItems.map(item => {
            const shortName = item.MaGop;
            return `
                                <div class="d-flex justify-content-between py-1 border-bottom">
                                    <span class="" style="max-width: 75%;" title="${shortName}">
                                        <i class="bi bi-box-seam"></i> Thùng: ${item.SttThung} - SLSP: ${item.SLSP}
                                    </span>
                                    <span class="text-muted">
                                        ${item.CBM.toFixed(2)}
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
                    (${totalAvailable.toFixed(2)} CBM)
                </button>
            </div>
        </div>
    `;

    $container.html(html);
}

function selectSingleSlot(slotId, checkvalue = true) {
    let $card = $(`.suggest-card[data-ten="${slotId}"]`);
    console.log("check3")
    if ($card.length > 0) {
        let items = $card.data("items") || [];

        // Tính tổng CBM của các item bị xóa
        let totalCBMRemoved = 0;

        console.log("lstThung trước khi chọn: ", lstThung.length)
        lstThung = lstThung.filter(sel => {
            let matched = items.some(it => isSameThung(it, sel));
            if (matched) {
                totalCBMRemoved += sel.CBM;
            }
            return !matched;
        });

        console.log("lstThung sau khi chọn: ", lstThung.length)


        // Tạo array để lưu
        const arrONPL = items.map(x => ({
            MaDH: x.MaDH,
            MaGop: x.MaGop,
            MaPKL: x.MaPKL,
            PO: x.PO,
            POID: x.POID,
            ColorID: x.ColorID,
            TenMau: x.TenMau,
            DauSize: x.DauSize,
            DauSizeID: x.DauSizeID,
            SLSP: x.SLSP,
            SttThung: x.SttThung,
            CBM: x.CBM,
            MaONPL: slotId,
            qrscan: x.qrscan || null,
        }));

        // Lưu vào database
        SaveONPLThanhPham(arrONPL, slotId);

        // Cập nhật thông tin slot
        let slot = warehouseSlots.find(w => w.TenO === slotId);
        if (slot) {
            slot.CBMCL -= totalCBMRemoved;
            slot.CBMSD += totalCBMRemoved;
            if (slot.CBMCL < 0) slot.CBMCL = 0;
        }


    }
    if (checkvalue) {
        showSuggestionModal();
    }
}

function selectSlotGroup(slotIds) {
    console.log("check4")
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
$(document).on
    ("click", '#checkAllCBM', function () {
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
    if (checker && hasEmpty) {
        showToast('warning', `Trong danh sách có ít nhất 1 roll chưa có CBM.!`, 2000);

    }
});

// Khởi tạo scanner khi mở modal
$('#itemDetailModal').on('shown.bs.modal', function () {
    $('#barcodeInput').focus()
});

/// Kiệt
/// GLOBAL
let lstCBMThungMove = [];
let lstCBMThungTPSlot = [];
let lstThung = [];
let lstCBMThungTP = [];
let lstDataLichSuQuet = [];
let tongCBM;
let tongCBMDC;
let dxGridLichSuQuet;
/// UTILS
function shouldAutoSelect(data, maxAuto = 1) {
    return Array.isArray(data.Table) && data.Table.length <= maxAuto;
}
function disableSelect(id, text = '') {
    $(`#${id}`)
        .html(`<option value="all">${text}</option>`)
        .prop('disabled', false);
}
function enableSelect(id) {
    $(`#${id}`).prop('disabled', false);
}
function resetDropdowns(ids) {
    ids.forEach(id => disableSelect(id, ''));
    $("#lstDongThung").empty();
    lstThung = [];
}
function renderSelect(id, data, renderOptionFn, autoSelect = true) {

    if (!data || data.length === 0) {
        $(`#${id}`)
            .html(`<option value=""></option>`)
            .prop('disabled', false);
        return false;
    }

    let html = `<option value=""></option>`;
    data.forEach(item => {
        html += renderOptionFn(item);
    });

    const $select = $(`#${id}`)
        .html(html)
        .prop('disabled', false);

    if (autoSelect) {
        const $opts = $select.find("option").not('[value=""]');
        if ($opts.length > 0) {
            $select.val($opts.first().val()).trigger("change");
        }
    }

    return true;
}
function FilterThung() {
    $("#filterThungModal").modal("show")
}
function updateAddButtonVisibility() {
    // Kiểm tra có chọn thùng hoặc vật tư không
    const hasThung = lstThung.length > 0;
    $(".slot-item").removeClass("highlighted");
    $(".item-add").each(function () {
        const cbmcl = parseFloat($(this).data("cbmcl"));

        if (cbmcl >= tongCBM && hasThung && tongCBM > 0) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}
function isSameThung(thung1, thung2) {
    // Nếu cả 2 đều có qrscan => so sánh qrscan
    if (thung1.qrscan && thung2.qrscan) {
        return thung1.qrscan === thung2.qrscan;
    }

    // Nếu không có qrscan => so sánh theo MaDH + MaGop + SttThung
    return thung1.MaDH === thung2.MaDH &&
        thung1.MaGop === thung2.MaGop &&
        thung1.SttThung === thung2.SttThung;
}
function getThungUniqueKey(thung) {
    if (thung.qrscan) {
        return `qr_${thung.qrscan}`;
    }
    return `${thung.MaDH}_${thung.MaGop}_${thung.SttThung}`;
}
function extractShelfFromSlot(maONPL) {
    if (!maONPL) return null;
    const parts = maONPL.split('.');
    if (parts.length >= 3) {
        return parts[2]; // Lấy phần thứ 3 (f1, f2)
    }
    return null;
}
function extractLevelFromSlot(maONPL) {
    if (!maONPL) return null;
    const parts = maONPL.split('.');
    if (parts.length >= 2) {
        const level = parts[1]
        return parseInt(level) || 0;
    }
    return 0;
}
function getShelfFromSelectedItems(items) {
    const shelves = items
        .map(item => extractShelfFromSlot(item.MaONPL))
        .filter(shelf => shelf !== null);

    // Lấy kệ xuất hiện nhiều nhất
    if (shelves.length === 0) return null;

    const shelfCounts = {};
    shelves.forEach(shelf => {
        shelfCounts[shelf] = (shelfCounts[shelf] || 0) + 1;
    });

    const mostCommonShelf = Object.keys(shelfCounts).reduce((a, b) =>
        shelfCounts[a] > shelfCounts[b] ? a : b
    );

    return mostCommonShelf;
}
// Lưu danh sách thùng thành phẩm vào ô kho
async function SaveThungToONPL(oid) {
    // Kiểm tra có thùng được chọn không
    if (lstThung.length === 0) {
        showToast('error', 'Vui lòng chọn thùng trước khi lưu vào ô!', 2000);
        return;
    }

    // Lấy thông tin ô từ data attribute
    const $slotElement = $(`.item-add[data-id="${oid}"]`);
    const cbmcl = parseFloat($slotElement.data("cbmcl")) || 0;
    // Kiểm tra sức chứa
    if (tongCBM > cbmcl) {
        showToast('error', `Ô ${oid} chỉ còn ${cbmcl.toFixed(2)} CBM, không đủ chứa ${tongCBM.toFixed(2)} CBM!`, 3000);
        return;
    }

    let arrSave = [];

    // Duyệt qua danh sách thùng đã chọn
    for (const thung of lstThung) {
        let object = {
            MaDH: thung.MaDH,
            MaGop: thung.MaGop,
            MaPKL: thung.MaPKL,
            PO: thung.PO,
            POID: thung.POID,
            ColorID: thung.ColorID,
            TenMau: thung.TenMau,
            DauSize: thung.DauSize,
            DauSizeID: thung.DauSizeID,
            SLSP: thung.SLSP,
            SttThung: thung.SttThung,
            CBM: thung.CBM,
            MaONPL: oid,
            qrscan: thung.qrscan
        };

        arrSave.push(object);
    }

    // Gọi API lưu
    await SaveONPLThanhPham(arrSave, oid);

    // Reset sau khi lưu thành công
    lstThung = [];
    tongCBM = 0;
    $(".tongCBM").text(0);
    $(".item-thung").removeClass("selected");
    $(".item-thung .tick-icon").hide();
    $(".item-add").hide();
}
// API lưu thành phẩm vào ô
async function SaveONPLThanhPham(arrSave, oid) {
    if (!arrSave || arrSave.length === 0) return;
    arrSave.forEach(x => {
        x.qrscan = 1
    })
    try {
        const response = await fetch(
            '/api/KeHoachXuatHangThanhPham/PostThungThanhPham?action=PostThung',
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(arrSave),
            }
        );

        const data = await response.json();

        if (data === "True") {
            showToast(
                'success',
                `Đã lưu ${arrSave.length} thùng vào ô ${oid} thành công!`,
                2000
            );

            // Reset lại UI
            if (lstThung.length === 0) {
                console.log("Đã lưu hết reset UI")
                $(".tongCBM").text(0);
                $(".item-thung").removeClass("selected");
                $(".item-thung .tick-icon").hide();
                $(".item-add").hide();
                $('#suggestionModal').modal('hide');
            }

            await GetOSugget();
            await GetThongTinThung();
            await GetViTriKho();
            await GetDanhSachO();

            // CHỈ RELOAD KHI MODAL ĐANG MỞ
            if ($('#itemDetailModal').hasClass('show') && solotIDBarCode === oid) {
                await loadVTKho();
            }

            PlayAudio();
        } else {
            showToast('error', 'Lưu thùng thất bại!', 2000);
        }
    }
    catch (error) {
        console.error(error);
        showToast('error', 'Có lỗi khi lưu thùng!', 2000);
    }
}

/// API
async function getFilterData(action, params) {
    const queryString = Object.entries(params)
        .map(([key, val], i) => `Para${i + 1}=${val}`)
        .join('&');

    const url = `/api/KeHoachXuatHangThanhPham/GetThanhPham?Action=${action}&${queryString}`;

    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(response.status);
        return await response.json();
    } catch (e) {
        console.error(action, e);
        return [];
    }
}
/// EVENTS
function filterThungByRange() {
    const start = Number($("#startThung").val());
    const end = Number($("#endThung").val());
    if (!start || !end || start > end) {
        showToast("warning", "Khoảng thùng không hợp lệ");
        return;
    }

    // Clear chọn cũ
    lstThung = [];
    $(".item-thung").removeClass("selected");
    $(".item-thung .tick-icon").hide();

    let isInvalid = false;

    $(".item-thung").each(function () {
        const sttThung = $(this).data("index");
        const item = $(this).data("item");

        // Kiểm tra thùng đã tồn tại trong kho chưa
        const isExists = lstCBMThungTP.some(itp => isSameThung(itp, item));

        if (isExists) {
            return; // Bỏ qua thùng đã có trong kho
        }

        if (sttThung >= start && sttThung <= end) {
            if (!item.CBM || Number(item.CBM) === 0) {
                showToast(
                    'warning',
                    `Vui lòng nhập CBM của thùng ${item.SttThung}, của ĐH: ${item.MaGop}`
                );
                isInvalid = true;
                return false;
            }

            $(this).addClass("selected");
            $(this).find(".tick-icon").show();
            lstThung.push({ ...item });
        }
    });

    if (isInvalid) return;

    tongCBM = lstThung.reduce((sum, x) => sum + (Number(x.CBM) || 0), 0);
    $(".tongCBM").text(tongCBM.toFixed(2));
    updateAddButtonVisibility();
    $("#filterThungModal").modal("hide");
    $("#startThung, #endThung").val("");
}
$('#moveItemModal').on('hidden.bs.modal', function () {
    // Reset khi đóng modal
    lstCBMThungMove = [];
    tongCBMDC = 0;
    $("#totalCBMDC").text("0.00");
    $(".item-thung-move .tick-icon").hide();
    $(".item-thung-move").removeClass("selected");

    // Clear containers
    $("#fromSlotItems").empty();
    $("#toSlotItems").empty();

    // Clear lịch sử quét nếu cần
    lstDataLichSuQuet = [];
    if (dxGridLichSuQuet) {
        dxGridLichSuQuet.option("dataSource", []);
    }
});

$(document).on("click", ".item-add", function (e) {
    e.preventDefault();
    const slotId = $(this).data("id");

    // Kiểm tra xem đang chọn thùng hay vật tư
    if (lstThung.length > 0) {
        // Trường hợp chọn thùng thành phẩm
        SaveThungToONPL(slotId);
    }
});

$(document).on("input", "#startThung, #endThung", function () {
    let val = $(this).val();

    if (val < 1) {
        $(this).val();
    }
});

$("#btnSaveFilterThung").on("click", function () {
    filterThungByRange()
});
$(document).on("keydown", "#startThung, #endThung", function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        filterThungByRange();
    }
});
$("#khachhangid").on("change", () => {
    resetDropdowns(['mahangid', 'poid', 'pklid', 'dausizeid', 'colorid', 'sizeid']);
    GetMHViTriKho();
});
$("#mahangid").on("change", () => {
    resetDropdowns(['poid', 'pklid', 'dausizeid', 'colorid', 'sizeid']);
    GetPOViTriKho();
    GetCBMThungTP();
});
$("#poid").on("change", () => {
    resetDropdowns(['pklid', 'dausizeid', 'colorid', 'sizeid']);
    GetMaPKL();
});
$("#pklid").on("change", () => {
    resetDropdowns(['dausizeid', 'colorid', 'sizeid']);
    GetDauSize();
});
$("#dausizeid").on("change", () => {
    resetDropdowns(['colorid', 'sizeid']);
    GetColor();
});
$("#colorid").on("change", () => {
    resetDropdowns(['sizeid']);
    GetSize();
});
$("#sizeid").on("change", GetThongTinThung);
$(document).on("click", ".item-thung", function (e) {
    $(this).find('.tick-icon').toggle();
    const sttThung = $(this).data("index");
    const item = $(this).data("item");
    // Kiểm tra có trong danh sách hay chưa
    const existingIndex = lstThung.findIndex(x => isSameThung(x, item));

    if (existingIndex !== -1) {
        lstThung.splice(existingIndex, 1);
    } else {
        if (item.CBM === 0) {
            showToast('warning', `Vui lòng nhập CBM của thùng ${item.SttThung}, của ĐH: ${item.MaGop}`);
            $("#cbmModal").modal("show")
        } else {
            $(this).toggleClass('selected');
            lstThung.push({
                ...item
            });
        }

    }

    tongCBM = lstThung.reduce((sum, x) => {
        return sum + (Number(x.CBM) || 0);
    }, 0);
    $(".tongCBM").text(tongCBM.toFixed(2));
    updateAddButtonVisibility();
});
// Thêm sự kiện khi đóng modal chi tiết ô
$('#itemDetailModal').on('hidden.bs.modal', function () {
    // Đóng tất cả các group
    $('#modalItemList .collapse').collapse('hide');
});
$(document).on("click", ".item-thung-move", function (e) {
    const sttThung = $(this).data("index");
    const item = $(this).data("item");

    // Kiểm tra có trong danh sách hay chưa
    const existingIndex = lstCBMThungMove.findIndex(x => isSameThung(x, item));

    if (existingIndex !== -1) {
        // BỎ CHỌN
        lstCBMThungMove.splice(existingIndex, 1);
        $(this).removeClass('selected');
        $(this).find('.tick-icon').hide();
    } else {
        // CHỌN MỚI
        if (item.CBM === 0) {
            showToast('warning', `Vui lòng nhập CBM của thùng ${item.SttThung}, của ĐH: ${item.MaGop}`);
            $("#cbmModal").modal("show");
            return; // ❌ Không thêm vào list nếu CBM = 0
        }

        $(this).addClass('selected');
        $(this).find('.tick-icon').show();
        lstCBMThungMove.push({ ...item });
    }

    updateTotalCBM();
});

$(document).ready(() => {
    $(".tongCBM").text(0);
    resetDropdowns(['mahangid', 'poid', 'pklid', 'dausizeid', 'colorid', 'sizeid']);
    GetViTriKho();
    GetMHViTriKho();
    GetDanhSachO();
    GetOSugget();
    createDxLichSuQuet();
});
async function GetViTriKho() {
    const url = `/api/KeHoachXuatHangThanhPham/GetThanhPham?action=GetViTriKho&para1=3`;
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
    const url = `/api/KeHoachXuatHangThanhPham/GetThanhPham?Action=DetailONo&Para1&Para2=3`;
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);

        const data = await response.json();
        warehouseDataChiTiet = data.Table;
    } catch (error) {
        console.error(error.message);
    }
}
async function GetMHViTriKho() {
    disableSelect('mahangid', 'Đang tải...');
    const data = await getFilterData('GetDH', {});
    renderSelect(
        'mahangid',
        data.Table,
        x => `<option value="${x.MaDH}">${x.TenHangDisplay}</option>`,
        //    shouldAutoSelect(data)
    );
}
async function GetPOViTriKho() {
    disableSelect('poid', 'Đang tải...');
    const mahang = $("#mahangid").val();
    const data = await getFilterData('GetPO', { mahang });
    renderSelect(
        'poid',
        data.Table,
        x => `<option value="${x.POID}" data-po="${x.PO}">${x.PO}</option>`,
        //    shouldAutoSelect(data)
    );
}
async function GetMaPKL() {
    disableSelect('pklid', 'Đang tải...');
    const maDonHang = $("#mahangid").val();
    const POID = $("#poid").val();

    const data = await getFilterData('GetMaPKL', { maDonHang, POID });

    renderSelect(
        'pklid',
        data.Table,
        x => `<option value="${x.MaPKL}">${x.MaPKL1}</option>`,
        //    shouldAutoSelect(data)
    );
}
async function GetDauSize() {
    disableSelect('dausizeid', 'Đang tải...');
    const data = await getFilterData('GetDauSize', {
        maDonHang: $("#mahangid").val(),
        poID: $("#poid").val(),
        maPKL: $("#pklid").val()
    });

    renderSelect(
        'dausizeid',
        data.Table,
        x => `<option value="${x.DauSize}">${x.DauSize}</option>`,
        //    shouldAutoSelect(data)
    );
}
async function GetColor() {
    disableSelect('colorid', 'Đang tải...');
    const data = await getFilterData('GetColorID', {
        maDonHang: $("#mahangid").val(),
        poID: $("#poid").val(),
        maPKL: $("#pklid").val(),
        dauSize: $("#dausizeid").val()
    });

    renderSelect(
        'colorid',
        data.Table,
        x => `<option value="${x.ColorID}">${x.TenMau}</option>`,
        //    shouldAutoSelect(data)
    );
}
async function GetSize() {
    disableSelect('sizeid', 'Đang tải...');
    const data = await getFilterData('GetSize', {
        maDonHang: $("#mahangid").val(),
        poID: $("#poid").val(),
        maPKL: $("#pklid").val(),
        dauSize: $("#dausizeid").val(),
        colorID: $("#colorid").val()
    });

    renderSelect(
        'sizeid',
        data.Table,
        x => `<option value="${x.SizeID}">${x.Size}</option>`,
        //    shouldAutoSelect(data)
    );
}
async function GetThongTinThung() {
    const data = await getFilterData('GetThongTinThung', {
        maDonHang: $("#mahangid").val(),
        poID: $("#poid").val(),
        maPKL: $("#pklid").val(),
        dauSize: $("#dausizeid").val(),
        colorID: $("#colorid").val(),
        sizeID: $("#sizeid").val()
    });

    lstCBMThungTP = await GetCBMThungTP();
    await createViewThung(data.Table)
    $("#btnLocThung").removeClass("d-none");
}
async function GetCBMThungTP() {
    const maDH = $("#mahangid").val()
    const data = await getFilterData('GetCBMThungTP', {
        Para1: '',
        Para2: maDH
    }); return data.Table
}
async function GetCBMThungTPSoLo(soloid) {
    const data = await getFilterData('GetCBMThungTP', {
        Para1: soloid,
        Para2: ''
    });
    lstCBMThungTPSlot = data.Table
    return data.Table;
}
/// VIEW THÙNG
async function createViewThung(data) {
    $("#lstDongThung").empty();
    data?.forEach((item, index) => {
        const title = item["Size@SLSP"]
            .split(",")
            .map(x => x.replace("@", "/"));
        const isExist = lstCBMThungTP.some(itp => isSameThung(itp, item));
        const $col = $(`
            <div class="col-2 item-thung-base ${isExist ? "item-thung-exist" : "item-thung"} position-relative"
                 data-index="${item.SttThung}"
                 tabindex="0"
                 data-bs-toggle="tooltip"
                 data-bs-html="true"
                 title="Size/SL:${title.map(data => `<br>${data}`).join('')}">
                <i class="fa-solid fa-circle-check position-absolute tick-icon"
                   style="top: -7px; right: -3px;color: rgb(182 130 84); font-size: 18px; display: none;"></i>
                <h6>${parseFloat(item.CBM.toFixed(2))}</h6>
                <hr/>
                <div class="position-relative d-flex justify-content-around align-items-center">
                        <i class="fa-thin fa-box" style="color:${isExist ? '#492FF6' : '#ff7b2a'};font-size:28px;transform: scaleX(1.28);"></i>
                        <small class=" position-absolute" style="font-size: 12px !important;color:${isExist ? '#492FF6' : '#000 !important'};  top: 58%;left: 50%;transform: translate(-50%, -55%);font-size: 10px;color: #492FF6;font-weight: 400;pointer-events: none;">${item.SttThung}</small>
                    </div>
            </div>
            `);
        $col.data("item", item);
        $("#lstDongThung").append($col);
    });
    $('[data-bs-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });
}
async function createDxLichSuQuet() {
    dxGridLichSuQuet = $("#dxLichSuQuet").dxDataGrid({
        dataSource: lstDataLichSuQuet,
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
            enabled: true,
            text: "Đang tải dữ liệu...",
            showIndicator: true,
            showPane: true
        },
        columns: [
            {
                dataField: "SttThung",
                caption: "STT",
                minWidth: 80,
                width: 50,
            },
            {
                dataField: "MaDH",
                caption: "Đơn Hàng",
                width: 150,
            },
            {
                dataField: "POID",
                caption: "PO",
                width: 120,
            },
            {
                dataField: "MaPKL",
                caption: "MaPKL",
                width: 90,
            },
            {
                dataField: "SLSP",
                caption: "SLSP",
                width: 80,
            }

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
    }).dxDataGrid("instance");
    $("#Layer_1").click()
}