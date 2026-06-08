var shelfData = [];
var shelfDataPL = [];
var detailData = [];
var dataDanhSachKe = []
$(function () {
    renDSKe()
    renderDSO()
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })

    // Checkbox events
    $('#cbShowKH, #cbShowMH, #cbShowItemCode').on('change', function () {
        refreshCellContent();
    });
})
// Hàm transform data
async function transformData(data) {
    const aislesMap = {};
    for (const item of data) {
        const aisleId = item.TenDay.split(" ")[1];
        const shelfId = item.TenKe.split(" ")[1];
        const levelId = item.TenTang.split(" ")[1];

        if (!aislesMap[aisleId]) {
            aislesMap[aisleId] = { id: aisleId, name: item.TenDay, description: `Khu vực ${item.TenDay}`, shelves: {} };
        }
        if (!aislesMap[aisleId].shelves[shelfId]) {
            aislesMap[aisleId].shelves[shelfId] = { id: shelfId, name: item.TenKe, textKe: item.TextKe, levels: {} };
        }
        if (!aislesMap[aisleId].shelves[shelfId].levels[levelId]) {
            aislesMap[aisleId].shelves[shelfId].levels[levelId] = { id: levelId, name: item.TenTang, textTang: item.TextTang, slots: [] };
        }

        aislesMap[aisleId].shelves[shelfId].levels[levelId].slots.push({
            id: item.TenO,
            cbm: item.CBM,
            usedCBM: parseFloat((item.CBM - item.CBMCL).toFixed(2)),
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
                    .sort((a, b) => Number(a.id.replace(/\D/g, '')) - Number(b.id.replace(/\D/g, '')))
                    .map(shelf => ({
                        ...shelf,
                        levels: Object.values(shelf.levels)
                            .sort((a, b) => Number(a.id) - Number(b.id))
                    }))
            }))
    };
}

// Hàm tính capacity class
function getCapacityClass(usedCBM, totalCBM) {
    const percentage = (usedCBM / totalCBM) * 100;
    const remainingPercentage = 100 - percentage;
    if (remainingPercentage === 0) return 'capacity-empty';
    if (remainingPercentage > 80) return 'capacity-high';
    if (remainingPercentage < 50) return 'capacity-low';
    return 'capacity-medium';
}

// Hàm tính remaining capacity
function getRemainingCapacity(usedCBM, totalCBM) {
    return Math.round(((totalCBM - usedCBM) / totalCBM) * 100);
}

// Hàm generate HTML cho modal
function generateWarehouseHTML(warehouseData, value) {
    let html = '<div class="row">';
    warehouseData.aisles.forEach(aisle => {
        html += `<div class="col-12 mb-4">
                        <div class="card aisle-card" data-aisle="${aisle.id}">
                            <div class="card-header bg-primary text-white">
                                <h5 class="mb-0"><i class="bi bi-building-fill"></i> ${aisle.name}</h5>
                            </div>
                            <div class="card-body">`;

        aisle.shelves.forEach(shelf => {
            html += `<div class="shelf-section card mb-3">
                                    <div class="card-header bg-secondary text-white py-2">
                                        <h6 class="mb-0"><i class="bi bi-bookshelf"></i> ${shelf.name} ${shelf.textKe != "" ? `(${shelf.textKe})` : ""}</h6>
                                    </div>
                                    <div class="card-body py-2">`;

            // Đảo ngược thứ tự levels để tầng 1 ở dưới cùng
            const reversedLevels = [...shelf.levels].reverse();
            reversedLevels.forEach(level => {
                html += `<div class="row align-items-center mb-2">
                                        <div class="col-auto"><div class="badge level-badge"  style="width: 90px;word-wrap: break-word;white-space: normal;">
                                    <span style="font-size:12px" class="badge level-badge d-flex"><i class="bi bi-layers"></i> ${level.name}</span>
                                    <div> ${level.textTang != "" ? `(${level.textTang})` : ""}</div>
                                    </div></div>
                    <div class="col"><div class="d-flex flex-wrap gap-1">`;

                level.slots.forEach(slot => {
                    const slotNumber = slot.id.split('.').pop();
                    const capacityClass = getCapacityClass(slot.usedCBM, slot.cbm);
                    const remainingCapacity = getRemainingCapacity(slot.usedCBM, slot.cbm);
                    const usedPercentage = Math.round(((slot.usedCBM / slot.cbm) * 100));
                    const itemCount = slot.count;
                    const cbmCL = parseFloat((parseFloat(slot.cbm.toFixed(2)) - parseFloat(slot.usedCBM.toFixed(2))).toFixed(2));

                    html += `<div class="position-relative">
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
                                        data-type="${value}"
                                        data-levelname="${level.name} ${level.textTang != "" ? `(${level.textTang})` : ""}">

                                        <div class="slot-used-section" style="height: ${usedPercentage}%"></div>
                                        <div class="slot-divider-line" style="bottom: ${usedPercentage}%"></div>

                                        <div class="slot-content">
                                            <div class="slot-number">${slotNumber}</div>
                                              <div class="slot-capacity">${parseFloat(slot.usedCBM.toFixed(2))} / ${parseFloat(slot.cbm.toFixed(2))} CBM</div>
                                            <div class="slot-capacity">${parseFloat((parseFloat(slot.cbm.toFixed(2)) - parseFloat(slot.usedCBM.toFixed(2))).toFixed(2))} CBMCL</div>
                                        </div>
                                        ${itemCount > 0 ? `<div class="item-count">${itemCount}</div>` : ''}
                                    </div>
                                </div>`;
                });

                html += `</div></div></div>`;
            });

            html += `</div></div>`;
        });

        html += `</div></div></div>`;
    });
    html += '</div>';
    return html;
}

// Hàm show tooltip
function showTooltip(e, $slot) {
    const $tooltip = $("#tooltip");
    if (!$tooltip.length) return;

    const slotId = $slot.data("id");
    const cbm = parseFloat($slot.data("cbm"));
    const usedCBM = parseFloat($slot.data("used-cbm"));
    const cbmCL = $slot.data("cbmcl");
    const remaining = cbm - usedCBM;
    const remainingPercentage = $slot.data("remaining");
    const nameO = $slot.data("textname");

    const aisleName = $slot.closest(".aisle-card").find(".card-header h5").text() || "N/A";
    const shelfName = $slot.closest(".shelf-section").find(".card-header h6").text() || "N/A";
    const levelName = $slot.data("levelname");

    let capacityStatus = "";
    if ($slot.hasClass("capacity-empty")) capacityStatus = "Đã đầy (0%)";
    else if ($slot.hasClass("capacity-high")) capacityStatus = `Còn nhiều chỗ (${remainingPercentage}%)`;
    else if ($slot.hasClass("capacity-medium")) capacityStatus = `Còn vừa phải (${remainingPercentage}%)`;
    else if ($slot.hasClass("capacity-low")) capacityStatus = `Sắp đầy (${remainingPercentage}%)`;

    $tooltip.html(`
                        <div><strong><i class="bi bi-geo-alt"></i> ${slotId}</strong></div>
                            <div><i class="bi bi-stack"></i> ${nameO}</div>
                            <div><i class="bi bi-building"></i> ${aisleName}</div>
                            <div><i class="bi bi-bookshelf"></i> ${shelfName}</div>
                            <div><i class="bi bi-layers"></i> ${levelName}</div>
                            <div><i class="bi bi-box"></i> CBM: ${parseFloat(usedCBM.toFixed(2))}/${parseFloat(cbm).toFixed(2)} (Còn: ${cbmCL})</div>
                            <div><i class="bi bi-speedometer2"></i> ${capacityStatus}</div>
                        `).css({ left: e.pageX + 15, top: e.pageY - 10, opacity: 1 });
}
function hideTooltip() {
    $("#tooltip").css("opacity", 0);
}
// Xử lý click vào kệ
async function handleShelfClick(keID, tenKe, value) {
    console.log(keID, tenKe)
    const filteredData = detailData.filter(item => item.KeID === keID);

    if (filteredData.length === 0) {
        $('#shelfModalBody').html('<div class="alert alert-warning">Không có dữ liệu cho kệ này</div>');
        $('#shelfModalLabel').text(`Chi Tiết ${tenKe}`);
        const modal = new bootstrap.Modal($('#shelfModal'));
        modal.show();
        return;
    }

    const warehouseData = await transformData(filteredData);
    const html = generateWarehouseHTML(warehouseData, value);

    $('#shelfModalBody').html(html);
    $('#shelfModalLabel').text(`Chi Tiết ${tenKe}`);

    const modal = new bootstrap.Modal($('#shelfModal'));
    modal.show();

    setTimeout(() => {
        $(document).on('mouseenter', '.slot-item', function (e) {
            showTooltip(e, $(this));
        });

        $(document).on('mousemove', '.slot-item', function (e) {
            $("#tooltip").css({ left: e.pageX + 15, top: e.pageY - 10 });
        });

        $(document).on('mouseleave', '.slot-item', function () {
            hideTooltip();
        });
    }, 300);
}
async function fetchSlotItems(slotId, type) {
    try {
        const response = await fetch(`/api/ViTriKhoNPL/Get?action=DetailO&para1=${slotId}&para2=${type}`);
        if (!response.ok) throw new Error(`Response status: ${response.status} `);
        const data = await response.json();

        return data.Table
    } catch (error) {
        console.error(error.message);
        return [];
    }
}

// Check1
async function showSlotDetail(slotId, type) {
    console.log(1)
    //const modal = new bootstrap.Modal($('#itemDetailModal')[0]);
    $('#itemDetailModal').modal("show");
    let datahtml = await fetchSlotItems(slotId, type);
    const tongSoKien = datahtml.length;

    $('#modalSlotTitle').html(`
        <i class="bi bi-box"></i> Chi tiết ô kho ${slotId}
    `);
    $('#modalSlotInfo').html(`
        <div class="fs-5 fw-bold text-primary">${slotId} / ${tongSoKien} Vật Tư</div>
    `);
    const capacityClass = getCapacityClass(datahtml[0].CBMSD, datahtml[0].CBMO);
    let capacityColor = 'success';
    if (capacityClass === 'capacity-empty') capacityColor = 'danger';
    else if (capacityClass === 'capacity-low') capacityColor = 'warning';
    else if (capacityClass === 'capacity-medium') capacityColor = 'info';
    succhuaOCL = parseFloat(datahtml[0].CBMO) - parseFloat(datahtml[0].CBMSD);
    $('#modalCapacityInfo').html(`
        <div class="fs-5 fw-bold text-${capacityColor}">${datahtml[0].CBMSD}/${datahtml[0].CBMO} CBM</div>
    `);

    let itemsHtml = "";
    if (datahtml && datahtml.length > 0 && datahtml[0].DisplayGroup != null) {

        // Group data theo MaVT
        const groupedData = {};
        datahtml.forEach(item => {
            const maVT = item.DisplayGroup;
            if (!groupedData[maVT]) {
                groupedData[maVT] = {
                    soLo: item.SoLo,
                    poMua: item.POMua,
                    totalCBM: 0,
                    totalSoKien: 0,
                    items: []
                };
            }
            groupedData[maVT].items.push(item);
            groupedData[maVT].totalCBM += parseFloat(item.CBM) || 0;
            groupedData[maVT].totalSoKien += 1 || 0;
        });

        itemsHtml = Object.entries(groupedData).map(([maVT, group], groupIndex) => {
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
            </div>`;
        }).join('');

    } else {
        itemsHtml = `
            <div class="text-center text-muted py-4">
                <i class="bi bi-inbox fs-1"></i>
                <div class="mt-2">Ô kho trống</div>
            </div>`;
    }

    setupSearchFunctionality();

    $('#modalItemList').html(itemsHtml);
}

// Events
$(document)
    .on("click", ".slot-item", function () {
        console.log(1)
        $(".slot-item").removeClass("highlighted");
        $(this).addClass("highlighted");

        const slotId = $(this).data("id");
        const typeID = $(this).data("type");
        console.log(typeID)
        showSlotDetail(slotId, typeID);

        setTimeout(() => {
            $(this).removeClass("highlighted");
        }, 5000);
    })

function LoadArea() {
    const aisleMap = {};
    walkwayDivMap = {};

    walkwayDivMap[1] = $('#materialArea').siblings('[data-walkwayid="1"]');
    walkwayDivMap[2] = $('#materialArea').siblings('[data-walkwayid="2"]');

    shelfData.forEach(shelf => {
        if (!aisleMap[shelf.DayID]) {
            aisleMap[shelf.DayID] = { name: shelf.TenDay, shelves: [] };
        }
        aisleMap[shelf.DayID].shelves.push(shelf);
    });

    const sortedAisles = Object.keys(aisleMap).sort((a, b) => {
        const nameA = aisleMap[a].name || '';
        const nameB = aisleMap[b].name || '';
        return nameA.localeCompare(nameB, 'vi', { numeric: true });
    });

    const materialArea = $('#materialArea');
    materialArea.empty();

    sortedAisles.forEach((aisleId, index) => {
        const aisleData = aisleMap[aisleId];
        const shelves = aisleData.shelves;

        shelves.sort((a, b) => {
            const nameA = a.TenKe || '';
            const nameB = b.TenKe || '';
            return nameA.localeCompare(nameB, 'vi', { numeric: true });
        });


        const isNAisle = shelves.every(s => (s.TenDay || '').toLowerCase().includes('n'));

        if (index > 0) {
            const walkwayId = index + 2;
            if (!walkwayDivMap[walkwayId]) {
                const walkwayDiv = $(`<div data-walkwayid="${walkwayId}" data-module="1" class="walkway dropzone"></div>`);
                walkwayDivMap[walkwayId] = walkwayDiv;
            }
            const $wv = walkwayDivMap[walkwayId];

            if (!$wv.hasClass('walkway-horizontal')
                && !$.contains(document, $wv[0])
                && !isNAisle) {
                materialArea.append($wv);
            }
        }

        const aisleDiv = $('<div class="aisle"></div>');
        const shelfPair = $('<div class="shelf-pair"></div>');
        const maxCells = Math.max(...shelves.map(s => s.MaxO || 0));

        shelves.forEach(shelf => {
            const tenDayIncludeN = (shelf.TenDay).toLowerCase().includes('n');

            const shelfDiv = $('<div class="shelf"></div>');

            if (tenDayIncludeN) shelfDiv.attr("draggable", true);
            shelfDiv.attr('id', shelf.ID);
            shelfDiv.attr('data-module', 1);
            shelfDiv.attr('data-keid', shelf.KeID);
            shelfDiv.attr('data-tenke', shelf.TenKe);
            shelfDiv.css({
                'width': '100px',
                'min-height': (maxCells * 50) + 20 + 'px',
                'display': 'flex',
                'flex-direction': 'column-reverse'
            });

            shelfDiv.append(`<div class="shelf-label">${shelf.TenKe}</div>`);

            for (let i = 0; i < (shelf.MaxO || 0); i++) {
                const cell = $('<div class="cell"></div>');
                cell.css({
                    'background': tenDayIncludeN
                        ? 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)'
                        : 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)'
                });
                if (shelf.KhachHang && shelf.KhachHang.trim() !== '') {
                    const customers = shelf.KhachHang.split(',').map(kh => kh.trim());
                    const customerList = customers.map(kh => `<li>${kh}</li>`).join('');
                    cell.append(`<div class="tooltip-custom"><strong>${shelf.TenKe}</strong><ul>${customerList}</ul></div>`);
                }
                shelfDiv.append(cell);
            }

            for (let i = (shelf.MaxO || 0); i < maxCells; i++) {
                shelfDiv.append('<div style="height: 30px;"></div>');
            }

            shelfDiv.on('click', function () {
                handleShelfClick($(this).data('keid'), $(this).data('tenke'), 1);
            });

            const walkwayID = parseInt(shelf.WalkwayID);
            shelfDiv.attr('data-current-walkwayid', !isNaN(walkwayID) && walkwayID > 0 ? walkwayID : ''); // ✅

            if (tenDayIncludeN) {
                const hasWalkwayID = !isNaN(walkwayID) && walkwayID > 0;

                if (hasWalkwayID) {
                    if (walkwayDivMap[walkwayID]) {
                        walkwayDivMap[walkwayID].append(shelfDiv);
                    } else {
                        shelfPair.append(shelfDiv);
                    }
                } else {
                    let placed = false;

                    const $allWalkways = $('[data-module="1"][data-walkwayid]').sort((a, b) => {
                        return parseInt($(a).data('walkwayid')) - parseInt($(b).data('walkwayid'));
                    });

                    $allWalkways.each(function () {
                        const $wv = $(this);
                        if ($wv.find('.shelf').length === 0) {
                            $wv.append(shelfDiv);
                            const wid = $wv.data('walkwayid');
                            shelfDiv.attr('data-current-walkwayid', wid);
                            shelfOnDrag(shelf.KeID, wid, 1);
                            placed = true;
                            return false;
                        }
                    });

                    if (!placed) {
                        shelfPair.append(shelfDiv);
                    }
                }
            } else {
                // Shelf thường giữ nguyên
                if (!isNaN(walkwayID) && walkwayID > 0 && walkwayDivMap[walkwayID]) {
                    walkwayDivMap[walkwayID].append(shelfDiv);
                } else {
                    shelfPair.append(shelfDiv);
                }
            }
        });

        aisleDiv.append(shelfPair);
        if (!isNAisle) {
            materialArea.append(aisleDiv);
        }
    });

    setTimeout(() => {
        const totalWidth = materialArea[0].scrollWidth;
        totalWidthWalkwayHorizontal = totalWidth;

        $('.walkway-horizontal[data-module="1"]').css({
            'width': totalWidthWalkwayHorizontal + 'px',
            'min-width': 'unset',
            'max-width': 'unset'
        });
    }, 100);

    initDragDrop();
}

function LoadAreaPL() {
    const aisleMap = {};
    walkwayDivMapPL = {};

    walkwayDivMapPL[1] = $('.container-shelve-right [data-walkwayid="1"]');

    shelfDataPL.forEach(shelf => {
        if (!aisleMap[shelf.DayID]) {
            aisleMap[shelf.DayID] = { name: shelf.TenDay, shelves: [] };
        }
        aisleMap[shelf.DayID].shelves.push(shelf);
    });

    const sortedAisles = Object.keys(aisleMap).sort((a, b) => {
        const nameA = aisleMap[a].name || '';
        const nameB = aisleMap[b].name || '';
        return nameA.localeCompare(nameB, 'vi', { numeric: true });
    });

    const materialArea = $('#container-khuvucphulieu');
    materialArea.empty();

    sortedAisles.forEach((aisleId, index) => {
        const aisleData = aisleMap[aisleId];
        const shelves = aisleData.shelves;

        shelves.sort((a, b) => {
            const nameA = a.TenKe || '';
            const nameB = b.TenKe || '';
            return nameA.localeCompare(nameB, 'vi', { numeric: true });
        });

        const isNAisle = shelves.every(s => (s.TenDay || '').toLowerCase().includes('n'));

        if (index > 0) {
            const walkwayId = index + 1;
            if (!walkwayDivMapPL[walkwayId]) {
                const walkwayDiv = $(`<div data-walkwayid="${walkwayId}" data-module="2" class="walkway dropzone"></div>`);
                walkwayDivMapPL[walkwayId] = walkwayDiv;
            }
            const $wv = walkwayDivMapPL[walkwayId];

            if (!$wv.hasClass('walkway-horizontal')
                && !$.contains(document, $wv[0])
                && !isNAisle) {
                materialArea.append($wv);
            }
        }


        const aisleDiv = $('<div class="aisle"></div>');
        const shelfPair = $('<div class="shelf-pair"></div>');
        const maxCells = Math.max(...shelves.map(s => s.MaxO || 0));

        shelves.forEach(shelf => {
            const tenDayIncludeN = (shelf.TenDay).toLowerCase().includes('n')
            const shelfDiv = $('<div class="shelf"></div>');

            if (tenDayIncludeN) shelfDiv.attr("draggable", true);
            shelfDiv.attr('type', 2);
            shelfDiv.attr('id', shelf.ID);
            shelfDiv.attr('data-module', 2);
            shelfDiv.attr('data-keid', shelf.KeID);
            shelfDiv.attr('data-tenke', shelf.TenKe);
            shelfDiv.css({
                'width': '100px',
                'min-height': (maxCells * 50) + 20 + 'px',
                'display': 'flex',
                'flex-direction': 'column-reverse'
            });

            shelfDiv.append(`<div class="shelf-label">${shelf.TenKe.replace('', '')}</div>`);

            for (let i = 0; i < (shelf.MaxO || 0); i++) {
                const cell = $('<div class="cell"></div>');
                cell.css({
                    'background': tenDayIncludeN
                        ? 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)'
                        : 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)'
                });

                if (shelf.KhachHang && shelf.KhachHang.trim() !== '') {
                    const customers = shelf.KhachHang.split(',').map(kh => kh.trim());
                    const customerList = customers.map(kh => `<li>${kh}</li>`).join('');
                    cell.append(`<div class="tooltip-custom"><strong>${shelf.TenKe}</strong><ul>${customerList}</ul></div>`);
                }
                shelfDiv.append(cell);
            }

            for (let i = (shelf.MaxO || 0); i < maxCells; i++) {
                shelfDiv.append('<div style="height: 100px !important;"></div>');
            }

            shelfDiv.on('click', function () {
                handleShelfClick($(this).data('keid'), $(this).data('tenke'), 2);
            });

            const walkwayID = parseInt(shelf.WalkwayID);
            shelfDiv.attr('data-current-walkwayid', !isNaN(walkwayID) && walkwayID > 0 ? walkwayID : ''); // ✅
            if (tenDayIncludeN) {
                const hasWalkwayID = !isNaN(walkwayID) && walkwayID > 0;

                if (hasWalkwayID) {
                    // ✅ Đã có WalkwayID → dùng đúng walkway đó
                    if (walkwayDivMapPL[walkwayID]) {
                        walkwayDivMapPL[walkwayID].append(shelfDiv);
                    } else {
                        shelfPair.append(shelfDiv);
                    }
                } else {
                    // ✅ Chưa có WalkwayID → tự tìm walkway trống đầu tiên
                    let placed = false;

                    const $allWalkways = $('[data-module="2"][data-walkwayid]').sort((a, b) => {
                        return parseInt($(a).data('walkwayid')) - parseInt($(b).data('walkwayid'));
                    });

                    $allWalkways.each(function () {
                        const $wv = $(this);
                        if ($wv.find('.shelf').length === 0) {
                            $wv.append(shelfDiv);
                            const wid = $wv.data('walkwayid');
                            shelfDiv.attr('data-current-walkwayid', wid);
                            shelfOnDrag(shelf.KeID, wid, 2);
                            placed = true;
                            return false;
                        }
                    });

                    if (!placed) {
                        shelfPair.append(shelfDiv);
                    }
                }
            } else {
                if (!isNaN(walkwayID) && walkwayID > 0 && walkwayDivMapPL[walkwayID]) {
                    walkwayDivMapPL[walkwayID].append(shelfDiv);
                } else {
                    shelfPair.append(shelfDiv);
                }
            }
        });

        aisleDiv.append(shelfPair);
        if (!isNAisle) {
            materialArea.append(aisleDiv);
        }
    });

    setTimeout(() => {
        const totalWidth = materialArea[0].scrollWidth;
        totalWidthWalkwayHorizontalPL = totalWidth;

        $('.walkway-horizontal[data-module="2"]').css({
            'width': totalWidthWalkwayHorizontalPL + 'px',
            'min-width': 'unset',
            'max-width': 'unset'
        });
    }, 100);

    materialArea.append('<div class="walkway" data-module="2"></div>');
    materialArea.append(`
        <div id="container-khuvucthanhpham" style="margin-top: 2px;">
            <div class="other-section">
                <div class="other-shelf" style="flex: 1; min-width: 60px;">
                    <div class="other-shelf-title">HÀNG THÀNH PHẨM</div>
                </div>
                <div class="other-shelf" style="min-width: 80px;">
                    <div class="other-shelf-title">HOÀN THÀNH</div>
                </div>
            </div>
        </div>
    `);
}

async function renDSKe() {
    const url = `/api/ViTriKhoNPL/Get?action=GetDanhSachKe`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();
        dataDanhSachKe = data.Table
          dataDanhSachKe.map(item => {

            item.TenHang = '1231231,123123,12312312,12313,12313,12313,12313,12313,12313,12313'
        })
        shelfData = dataDanhSachKe.filter(x => x.Module == 1)
        shelfDataPL = dataDanhSachKe.filter(x => x.Module == 2)
        LoadArea()
        LoadAreaPL()
        handleSearch()

        setTimeout(() => {
            initWalkwayStyles();
        }, 150);
        //LoadOtherArea();

    } catch (error) {
        console.error(error.message);
    }

}
async function renderDSO() {
    const url = `/api/ViTriKhoNPL/Get?action=GetViTriKho`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const data = await response.json();

        detailData = await data.Table;

    } catch (error) {
        console.error(error.message);
    }

}

/// Kiệt
// ==================== CONSTANTS ====================
const DEBOUNCE_DELAY = 600;
const MESSAGE_DURATION = 3000;

const COLORS = {
    SUCCESS: '#22c55e',
    ERROR: '#ef4444',
    INFO: '#3b82f6'
};

const ICONS = {
    SUCCESS: 'fa-circle-check',
    ERROR: 'fa-circle-xmark',
    INFO: 'fa-circle-info'
};

const GRADIENTS = {
    NL_NORMAL: 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)',
    NL_NDAY: 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)',
    PL_NORMAL: 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)',
    PL_NDAY: 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)',
    HIGHLIGHT: 'linear-gradient(135deg, rgb(254, 202, 202) 0%, rgb(255 165 165 / 77%) 100%)'
};

// ==================== UTILITY FUNCTIONS ====================
function extractShelfNumber(tenKe) {
    const numbers = (tenKe || '').match(/\d+/g);
    return numbers ? parseInt(numbers[0]) : 0;
}


function isPhuLieu(shelfNumber) {
    return shelfNumber >= 1 && shelfNumber <= 5;
}

function getBackground(shelfNumber) {
    return isPhuLieu(shelfNumber) ? GRADIENTS.PHU_LIEU : GRADIENTS.THANH_PHAM;
}

// ==================== UI FUNCTIONS ====================
function showMessage(message, type = 'ERROR') {
    const $notifi = $('#notifi-notfound');
    const color = COLORS[type];
    const icon = ICONS[type];

    $notifi.html(`
        <i class="fa-solid ${icon}" style="color: ${color};"></i>
        <span style="color: ${color}; margin-left: 5px;">${message}</span>
    `).fadeIn(300);

    setTimeout(() => {
        $notifi.fadeOut(300, () => $notifi.html(''));
    }, MESSAGE_DURATION);
}

function resetHighlight() {
    $('#materialArea').find('.shelf').each(function () {
        const tenDayIncludeN = (dataDanhSachKe.find(x => x.KeID == $(this).data('keid') && x.Module == 1)?.TenDay || '').toLowerCase().includes('n');
        $(this).find('.cell').css({
            background: tenDayIncludeN
                ? 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)'
                : 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)',
            boxShadow: '',
            border: '1px solid #dcd6d66b'
        });
    });

    $('[data-module="1"].walkway .shelf, [data-module="1"].walkway-horizontal .shelf').each(function () {
        $(this).find('.cell').css({
            background: 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)',
            boxShadow: '',
            border: '1px solid #dcd6d66b'
        });
    });

    $('#container-khuvucphulieu').find('.shelf').each(function () {
        const tenDayIncludeN = (dataDanhSachKe.find(x => x.KeID == $(this).data('keid') && x.Module == 2)?.TenDay || '').toLowerCase().includes('n');
        $(this).find('.cell').css({
            background: tenDayIncludeN
                ? 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)'
                : 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)',
            boxShadow: '',
            border: '1px solid #dcd6d66b'
        });
    });

    $('[data-module="2"].walkway .shelf, [data-module="2"].walkway-horizontal .shelf').each(function () {
        $(this).find('.cell').css({
            background: 'linear-gradient(135deg, #f1f5f9 0%, rgb(194 213 214) 100%)',
            boxShadow: '',
            border: '1px solid #dcd6d66b'
        });
    });
}

function highlightCells(keIds) {
    resetHighlight();
    if (!keIds.length) return;

    requestAnimationFrame(() => {
        const highlightStyle = {
            background: GRADIENTS.HIGHLIGHT,
            border: '1px solid rgb(183 158 158 / 42%)'
        };

        keIds.forEach(keId => {
            $(`[data-keid="${keId}"]`).find('.cell').css(highlightStyle);
        });
    });
}

// ==================== SEARCH FUNCTION ====================
async function handleSearch() {
    let debounceTimer = null;
    try {

        const data = dataDanhSachKe
        const keMap = new Map();
        data.forEach(item => {
            const khachHang = item.KhachHang?.trim().toLowerCase();
            const maVT = item.MaVT?.trim().toLowerCase();

            if (khachHang) {
                if (!keMap.has(khachHang)) {
                    keMap.set(khachHang, []);
                }
                keMap.get(khachHang).push(item.KeID);
            }
            if (maVT) {
                if (!keMap.has(maVT)) {
                    keMap.set(maVT, []);
                }
                keMap.get(maVT).push(item.KeID);
            }

        });

        // Setup search event
        $('#txtSearchKhachHang').on('input', function () {
            const keyword = $(this).val().trim().toLowerCase();
            clearTimeout(debounceTimer);

            if (!keyword) {
                resetHighlight();
                $('#notifi-notfound').fadeOut(300, function () {
                    $(this).html('');
                });
                return;
            }
            console.log("search: ", keMap)
            debounceTimer = setTimeout(() => {
                const matchedKeys = Array.from(keMap.keys())
                    .filter(k => k.includes(keyword));

                if (matchedKeys.length > 0) {
                    const keIds = matchedKeys.flatMap(k => keMap.get(k));
                    highlightCells(keIds);
                } else {
                    resetHighlight();
                    showMessage('Không tìm thấy thông tin này trong kho', 'ERROR');
                }
            }, DEBOUNCE_DELAY);
        });

    } catch (error) {
        showMessage('Lỗi tải dữ liệu kho', 'ERROR');
    }
}

// ==================== LOAD OTHER AREA ====================
function LoadOtherArea() {
    // Test
    /* shelfData = DATA_MOCK*/    // Group shelves by aisle
    const aisleMap = {};
    var shelfDataPL = shelfData.filter(x => x.Module == 2)
    shelfDataPL.forEach(shelf => {
        if (!aisleMap[shelf.DayID]) {
            aisleMap[shelf.DayID] = {
                name: shelf.TenDay,
                shelves: []
            };
        }
        aisleMap[shelf.DayID].shelves.push(shelf);
    });

    // Sort aisles by name
    const sortedAisles = Object.keys(aisleMap).sort((a, b) => {
        return (aisleMap[a].name || '').localeCompare(
            aisleMap[b].name || '',
            'vi',
            { numeric: true }
        );
    });

    // Categorize aisles
    const phuLieuAisles = [];
    const thanhPhamAisles = [];

    sortedAisles.forEach(aisleID => {
        const aisle = aisleMap[aisleID];

        // Sort shelves by number
        aisle.shelves.sort((a, b) => {
            return extractShelfNumber(a.TenKe) - extractShelfNumber(b.TenKe);
        });

        // Categorize based on first shelf
        const firstShelfNum = extractShelfNumber(aisle.shelves[0].TenKe);
        if (isPhuLieu(firstShelfNum)) {
            phuLieuAisles.push(aisle);
        } else {
            thanhPhamAisles.push(aisle);
        }
    });

    // Render
    $('#container-khuvucphulieu').empty();
    renderAisleGroups(phuLieuAisles, $('#container-khuvucphulieu'));

    $('#container-hangthanhpham').text("NHẬN THÀNH PHẨM");
    // Chờ
    //renderRowThanhPham(thanhPhamAisles, $('#container-hangthanhpham'));

    // Update header
    requestAnimationFrame(() => {
        setTimeout(() => updateHeaderLayout(), 50);
    });
}

// ==================== RENDER FUNCTIONS ====================
function renderAisleGroups(aisles, container) {
    if (!aisles.length) return;

    const mainWrapper = $('<div class="shelf-pair"></div>');

    aisles.forEach((aisle, aisleIndex) => {
        if (aisleIndex > 0) {
            mainWrapper.append('<div class="walkway"></div>');
        }

        const maxCells = Math.max(...aisle.shelves.map(s => s.MaxO || 0));
        const firstShelfNum = extractShelfNumber(aisle.shelves[0].TenKe);
        const background = getBackground(firstShelfNum);

        const aisleWrapper = $('<div class="aisle-wrapper"></div>').css({
            display: 'flex',
            gap: '0',
            alignItems: 'flex-start'
        });

        // Render each shelf
        aisle.shelves.forEach(shelf => {
            const shelfDiv = $('<div class="shelf"></div>')
                .attr({
                    id: shelf.ID,
                    'data-keid': shelf.KeID,
                    'data-tenke': shelf.TenKe
                })
                .css({
                    width: '45px',
                    minHeight: `${maxCells * 30 + 20}px`,
                    flexShrink: '0',
                    margin: '0px 2px'
                });

            // Add shelf label
            shelfDiv.append(`<div class="shelf-label">${shelf.TenKe}</div>`);

            // Add cells
            for (let i = 0; i < (shelf.MaxO || 0); i++) {
                const cell = $('<div class="cell"></div>').css({
                    marginTop: i === 0 ? '20px' : '0',
                    background: background,
                    border: '1px solid #000',
                    height: '30px'
                });

                // Add tooltip if customer exists
                if (shelf.KhachHang?.trim()) {
                    const customers = shelf.KhachHang.split(',').map(kh => kh.trim());
                    const customerList = customers.map(kh => `<li>${kh}</li>`).join('');
                    cell.append(`
                        <div class="tooltip-custom">
                            <strong>${shelf.TenKe}</strong>
                            <ul>${customerList}</ul>
                        </div>
                    `);
                }

                shelfDiv.append(cell);
            }

            // Add empty cells for alignment
            for (let i = (shelf.MaxO || 0); i < maxCells; i++) {
                shelfDiv.append('<div style="height: 50px;"></div>');
            }

            // Add click event
            shelfDiv.on('click', function () {
                handleShelfClick($(this).data('keid'), $(this).data('tenke'));
            });

            aisleWrapper.append(shelfDiv);
        });

        mainWrapper.append(aisleWrapper);
    });

    container.append(mainWrapper);
}
function renderRowThanhPham(aisles, container) {
    if (!aisles || !aisles.length) return;
    container.empty();
    const mainWrapper = $('<div>').addClass('row-shelf-pair');

    let totalRendered = 0;
    const maxShelves = 3;

    for (const aisle of aisles) {
        if (totalRendered >= maxShelves) break;

        for (const shelf of aisle.shelves) {
            if (totalRendered >= maxShelves) break;

            const rowDiv = $('<div>').addClass('row-shelf')
                .attr("id", shelf.KeID)
                .attr("data-keid", shelf.KeID);
            const labelCell = $('<div>').addClass('row-cell')
                .text(shelf.TenKe.replace("Kệ ", ""));

            rowDiv.append(labelCell);

            for (let i = 0; i < (shelf.MaxO || 0); i++) {
                const cell = $('<div>').addClass('row-cell');

                if (shelf.KhachHang && shelf.KhachHang.trim() !== '') {
                    const customers = shelf.KhachHang.split(',').map(kh => kh.trim());
                    const customerList = customers.map(kh => `<li>${kh}</li>`).join('');
                    cell.append(`
                        <div class="tooltip-custom">
                            <strong>${shelf.TenKe}</strong>
                            <ul>${customerList}</ul>
                        </div>
                    `);
                }
                rowDiv.append(cell);
            }

            rowDiv.on('click', function () {
                handleShelfClick(shelf.KeID, shelf.TenKe);
            });

            mainWrapper.append(rowDiv);
            totalRendered++;
        }
    }

    container.append(mainWrapper);
}
// ==================== UPDATE HEADER ====================
function updateHeaderLayout() {
    const containers = [
        { selector: '#container-khuvucphulieu', title: 'KHU VỰC PHỤ LIỆU' },
        { selector: '#container-khuvucthanhpham', title: '' },
    ];

    const headerRow2 = $('.header-row-2').empty();

    containers.forEach(({ selector, title }) => {
        const containerElement = $(selector);
        let width = 0;

        if (selector === '#container-khuvucphulieu') {
            width = containerElement.outerWidth(false);
        } else if (selector === '#container-khuvucthanhpham') {
            width = containerElement.outerWidth(false);
        }

        if (containerElement.length > 0 && width > 0) {
            const sectionDiv = $(`
                <div class="section-title" style="border-top: 0;">${title}</div>
            `).css({
                width: `${width}px`,
            });

            headerRow2.append(sectionDiv);
        }
    });
}

/// kiet - 16032026
let draggedShelfID = null;
let draggedKeID = null;
let totalWidthWalkwayHorizontal;
let totalWidthWalkwayHorizontalPL;
let totalHeightA;
let walkwayDivMap = {};
let walkwayDivMapPL = {};
/// EVENT
function initWalkwayStyles() {
    let maxHeight = 0;
    $('#materialArea .shelf, #container-khuvucphulieu .shelf').each(function () {
        if ($(this).closest('.walkway, .walkway-horizontal').length === 0) {
            const h = $(this).outerHeight();
            if (h > maxHeight) maxHeight = h;
        }
    });
    if (maxHeight > 0) totalHeightA = maxHeight;


    $('.walkway-horizontal').each(function () {
        const $wh = $(this);
        const module = $wh.data('module')
        const $shelves = $wh.find('.shelf');
        const minH = module == 1 ? totalWidthWalkwayHorizontal / 2 : totalWidthWalkwayHorizontalPL / 2;

        const $cells = $wh.find('.cell');
        const shelfCount = $shelves.length;
        if (shelfCount === 0) return;

        if (shelfCount === 1) {
            const $s = $shelves.first();
            const walkwayH = $wh.outerHeight(); 
            const shelfMinH = minH; 

            const translateY = (shelfMinH / 2) - (walkwayH / 2) + 12;

            $wh.css({
                'display': 'flex',
                'flex-direction': 'row',
                'align-items': 'center',
                'height': '90px',
                'overflow': 'hidden'
            });
            $s.css({
                'transform': `rotate(-90deg) translateY(${translateY}px)`,
                'transform-origin': 'center center',
                'width': '80px',
                'min-height': `${minH}px`,
                'justify-content': 'start',
            });

            $wh.find('.cell').css({
                'min-height': `${module == 1 ? totalWidthWalkwayHorizontal / 2 : totalWidthWalkwayHorizontalPL / 2}px`,
                'max-height': `${module == 1 ? totalWidthWalkwayHorizontal / 2 : totalWidthWalkwayHorizontalPL / 2}px`,
            });
        } else {

            $wh.css({

                'display': 'flex',
                'flex-direction': 'row',
                'align-items': 'center',
                'height': '110px',
                'gap': '8px',
                'overflow': 'hidden'
            });
            $shelves.each(function () {
                $(this).css({
                    'transform': '',
                    'transform-origin': '',
                    'width': '80px',
                    'min-height': '80px',
                    'max-height': '80px',
                    'justify-content': '',
                    'margin-top': ''
                });
            });
            $cells.each(function () {
                $(this).css({
                    'min-height': '78px',
                    'max-height': '78px',
                });
            });
        }
    });

    $('.walkway:not(.walkway-horizontal)').each(function () {
        const $wv = $(this);
        const $shelves = $wv.find('.shelf');
        const shelfCount = $shelves.length;

        if (shelfCount === 0) return;

        $wv.css({
            'width': 'auto',
            'min-width': '100px',
            'display': 'flex',
            'flex-wrap': 'wrap',
            'align-items': 'flex-start',
            'height': `${totalHeightA}px`,
            'gap': '0px',
            'padding': '0px 4px'
        });

        $shelves.each(function () {
            $(this).css({
                'transform': '',
                'transform-origin': '',
                'width': '100px',
                'min-height': `${totalHeightA}px`,
                'margin-top': '',
                'justify-content': ''
            });
            $(this).find('.cell').css({
                'min-height': `${totalHeightA}px`,
                'max-height': `${totalHeightA}px`,
            });
            $(this).find('.shelf-group-content').css({
                'max-height': `${totalHeightA - 100}px`,
            });
        });
    });
}

function initDragDrop() {

    $(document).on('dragstart', '.shelf[draggable="true"]', function (e) {
        draggedShelfID = this.id;
        draggedKeID = $(this).data('keid');
        $(this).addClass('shelf-dragging');
        e.originalEvent.dataTransfer.setData('shelfID', this.id);
        e.originalEvent.dataTransfer.setData('keID', draggedKeID);
        e.originalEvent.dataTransfer.effectAllowed = 'move';
    });

    $(document).on('dragend', '.shelf[draggable="true"]', function () {
        $(this).removeClass('shelf-dragging');
        $('.walkway, .walkway-horizontal').removeClass('walkway-dragover walkway-dragover-invalid');
        draggedShelfID = draggedKeID = null;
    });

    $(document).on('dragover', '.walkway, .walkway-horizontal', function (e) {
        e.preventDefault();
        const walkwayModule = parseInt($(this).attr('data-module')) || 1;
        const shelfModule = dataDanhSachKe.find(x => x.KeID == draggedKeID)?.Module ?? 1;

        if (shelfModule !== walkwayModule) {
            e.originalEvent.dataTransfer.dropEffect = 'none';
            $(this).addClass('walkway-dragover-invalid').removeClass('walkway-dragover');
        } else {
            e.originalEvent.dataTransfer.dropEffect = 'move';
            $(this).addClass('walkway-dragover').removeClass('walkway-dragover-invalid');
        }
    });

    $(document).on('dragleave', '.walkway, .walkway-horizontal', function (e) {
        if (!$(this).is(e.target)) return;
        const related = e.originalEvent.relatedTarget;
        if (related && $(this)[0].contains(related)) return;
        $(this).removeClass('walkway-dragover walkway-dragover-invalid');
    });

    $(document).on('drop', '.walkway, .walkway-horizontal', function (e) {
        e.preventDefault();
        $(this).removeClass('walkway-dragover walkway-dragover-invalid');

        const isAnyChecked = $('#cbShowKH, #cbShowMH, #cbShowItemCode').is(':checked');
        const shelfID = e.originalEvent.dataTransfer.getData('shelfID');
        const keID = e.originalEvent.dataTransfer.getData('keID');
        const walkwayID = $(this).data('walkwayid');
        if (!shelfID || !keID) return;

        const $shelf = $(`[id="${shelfID}"]`).first();
        const $walkway = $(this);
        const shelfModule = parseInt($shelf.attr('data-module')) || 1;
        const walkwayModule = parseInt($walkway.attr('data-module')) || 1;
        const isHorizontal = $walkway.hasClass('walkway-horizontal');

        //  Validate 
        if (shelfModule !== walkwayModule) {
            showToast('error', 'Không thể di chuyển kệ sang khu vực khác!');
            return;
        }
        if (!isHorizontal && $walkway.find('.shelf').not(`[id="${shelfID}"]`).length > 0) {
            showToast('warning', 'Lối đi này đã có kệ, không thể thêm tiếp');
            return;
        }

        //  Cleanup old walkway 
        const $oldWalkway = $shelf.closest('.walkway, .walkway-horizontal');
        if ($oldWalkway.length && !$oldWalkway.is($walkway)) {
            const isNDay = (dataDanhSachKe.find(x => x.KeID == draggedKeID)?.TenDay || '').toLowerCase().includes('n');
            const remaining = $oldWalkway.find('.shelf').length;
            if (isNDay && remaining === 0) {
                $oldWalkway.remove();
            } else if (!$oldWalkway.hasClass('walkway-horizontal')) {
                $oldWalkway.css({ 'width': '60px', 'min-width': '60px', 'background-color': '#fff', 'border-left': '1px solid #000', 'border-right': '1px solid #000' });
            }
        }

        //  Append shelf 
        $walkway.append($shelf);
        const shelfCount = $walkway.find('.shelf').length;

        //  Style walkway sau drop 
        if (isHorizontal) {
            const wWidth = shelfModule == 1 ? totalWidthWalkwayHorizontal : totalWidthWalkwayHorizontalPL;
            $walkway.css({ 'display': 'flex', 'flex-direction': 'row', 'align-items': 'center', 'width': `${wWidth}px`, 'overflow': 'hidden' });

            if (shelfCount === 1) {
                const $s = $walkway.find('.shelf').first();

                $walkway.css({
                    'display': 'flex',
                    'flex-direction': 'row',
                    'align-items': 'center',
                    'height': '100px',
                    'overflow': 'hidden'
                });
                const minH = wWidth / 2;
                const walkwayH = $walkway.outerHeight();
                const translateY = ((minH / 2) - (walkwayH / 2)) + 29;

                $s.css({
                    'transform': `rotate(-90deg) translateY(${translateY}px)`,
                    'transform-origin': 'center center',
                    'width': '80px',
                    'min-height': `${minH}px`,
                    'max-height': '',
                    'justify-content': 'start',
                });
                $walkway.find('.cell').css({
                    'min-height': `${minH}px`,
                    'max-height': `${minH}px`,
                });
            } else {
                $walkway.css({ 'height': isAnyChecked ? '220px' : '100px', 'gap': '8px' });
                $walkway.find('.shelf').css({ 'transform': '', 'width': '80px', 'min-height': isAnyChecked ? '200px' : '80px', 'max-height': isAnyChecked ? '200px' : '80px', 'overflow': 'hidden' });
                $walkway.find('.cell').css({ 'min-height': isAnyChecked ? '199px' : '78px', 'max-height': isAnyChecked ? '199px' : '78px' });
                $walkway.find('.shelf').each(function () {
                    const $groups = $(this).find('.shelf-group');
                    const groupCount = $groups.length;
                    if (groupCount === 0) return;

                    const containerH = 200;
                    const headerH = 16;
                    const totalHeaderH = groupCount * headerH;
                    const availableH = containerH - totalHeaderH - 26;
                    const perGroupH = Math.floor(availableH / groupCount);

                    $groups.find('.shelf-group-content').css({
                        'max-height': `${Math.max(perGroupH, 20)}px`
                    });
                });
            }
        } else {
            $walkway.css({ 'width': 'auto', 'min-width': '100px', 'display': 'flex', 'flex-wrap': 'wrap', 'align-items': 'flex-start', 'height': `${totalHeightA}px`, 'gap': '0', 'padding': '0 4px' });
            $shelf.css({ 'transform': '', 'width': '100px', 'min-height': `${totalHeightA}px`, 'margin-top': '', 'justify-content': '' });
            $shelf.find('.cell').css({ 'min-height': `${totalHeightA - 2}px`, 'max-height': `${totalHeightA - 2}px` });

            const $groups = $shelf.find('.shelf-group');
            const groupCount = $groups.length;
            if (groupCount > 0) {
                const availableH = totalHeightA - (groupCount * 18) - 20;

                $groups.each(function () {
                    const groupType = $(this).data('group');
                    let groupH;

                    if (groupType === 'ic') {
                        // IC lấy 60% available
                        groupH = Math.floor(availableH * 0.6);
                    } else {
                        // KH và MH chia đều phần còn lại 40%
                        const otherCount = groupCount - 1;
                        groupH = otherCount > 0 ? Math.floor((availableH * 0.4) / otherCount) : Math.floor(availableH * 0.4);
                    }

                    $(this).find('.shelf-group-content').css({
                        'max-height': `${Math.max(groupH, 40)}px`,
                        'overflow-y': 'auto',
                        'overflow-x': 'hidden'
                    });
                });
            }
        }

        //  Sync tất cả walkway-horizontal sau drop 
        $('.walkway-horizontal').each(function () {
            const $wh = $(this);
            const mod = $wh.data('module');
            const $shelves = $wh.find('.shelf');
            const $cells = $wh.find('.cell');
            const wWidth = mod == 1 ? totalWidthWalkwayHorizontal : totalWidthWalkwayHorizontalPL;
            const minH = wWidth / 2;

            if ($shelves.length === 0) {
                $wh.css('height', '48px');
            }
            if ($shelves.length === 1) {
                $wh.css('height', '100px');
                const walkwayH = $wh.outerHeight();
                const translateY = (minH / 2) - (walkwayH / 2) + 29;

                $shelves.first().css({
                    'transform': `rotate(-90deg) translateY(${translateY}px)`,
                    'transform-origin': 'center center',
                    'width': '80px',
                    'min-height': `${minH}px`,
                    'max-height': `${minH}px`,
                    'justify-content': 'start',
                    'margin-top': '',
                    'overflow': 'hidden'
                });
                $cells.css({ 'min-height': `${minH}px`, 'max-height': `${minH}px` });

                // Chia đều height cho các group
                const $groups = $shelves.first().find('.shelf-group');
                const groupCount = $groups.length;
                if (groupCount > 0) {
                    const availableH = minH - (groupCount * 18) - 20;
                    const perGroupH = Math.floor(availableH / groupCount);
                    $groups.find('.shelf-group-content').css({
                        'max-height': `${Math.max(perGroupH, 40)}px`,
                        'overflow-y': 'auto',
                    });
                }

            } else if ($shelves.length > 1)  {
                $wh.css({ 'height': isAnyChecked ? '220px' : '100px', 'gap': '8px' });
                $shelves.css({ 'transform': '', 'width': '80px', 'min-height': isAnyChecked ? '200px' : '80px', 'max-height': isAnyChecked ? '200px' : '80px', 'overflow': 'hidden' });
                $cells.css({ 'min-height': isAnyChecked ? '199px' : '78px', 'max-height': isAnyChecked ? '199px' : '78px' });
            }
        });

        shelfOnDrag(keID, walkwayID, shelfModule);
    });
}

function getCellDisplayHTML(shelf, keID) {
    const isKH = $('#cbShowKH').is(':checked');
    const isMH = $('#cbShowMH').is(':checked');
    const isIC = $('#cbShowItemCode').is(':checked');
    if (!isKH && !isMH && !isIC) return '';

    const base = totalHeightA || 300;
    const PADDING = 20;
    const TOGGLE_H = 18;
    const KH_FIXED = 30;
    const MH_FIXED = 40;

    // Tính tổng toggle header đang bật
    const toggleCount = [isKH, isMH, isIC].filter(Boolean).length;
    const totalToggleH = toggleCount * TOGGLE_H;

    // Tổng không gian usable
    const usable = base - PADDING - totalToggleH;

    // Tính phần KH , MH đã dùng
    let usedFixed = 0;
    if (isKH) usedFixed += KH_FIXED;
    if (isMH) usedFixed += MH_FIXED;

    // IC luôn lấy phần còn lại
    const icHeight = isIC ? Math.max(usable - usedFixed, 30) : 0;

    let groups = [];

    if (isKH && shelf.KhachHang?.trim()) {
        const items = shelf.KhachHang.split(',').map(x => x.trim()).filter(Boolean).join(', ');
        const id = `kh-${keID}`;
        groups.push(`
            <div class="shelf-group" data-group="kh">
                <div class="shelf-group-toggle" onclick="event.stopPropagation();event.preventDefault();$('#${id}').collapse('toggle');">
                    <b>Khách Hàng</b><span class="toggle-icon">▾</span>
                </div>
                <div class="collapse show" id="${id}">
                    <div class="shelf-group-content" style="max-height:${KH_FIXED}px;overflow-y:auto;overflow-x:hidden;">${items}</div>
                </div>
            </div>`);
    }

    if (isMH && shelf.TenHang?.trim()) {
        const items = shelf.TenHang.split(',').map(x => x.trim()).filter(Boolean).join(', ');
        const id = `mh-${keID}`;
        groups.push(`
            <div class="shelf-group" data-group="mh">
                <div class="shelf-group-toggle" onclick="event.stopPropagation();event.preventDefault();$('#${id}').collapse('toggle');">
                    <b>Mã Hàng</b><span class="toggle-icon">▾</span>
                </div>
                <div class="collapse show" id="${id}">
                    <div class="shelf-group-content" style="max-height:${MH_FIXED}px;overflow-y:auto;overflow-x:hidden;">${items}</div>
                </div>
            </div>`);
    }

    if (isIC && shelf.MaVT?.trim()) {
        const items = shelf.MaVT.split(',').map(x => `<li>${x.trim()}</li>`).join('');
        const id = `ic-${keID}`;
        groups.push(`
            <div class="shelf-group" data-group="ic">
                <div class="shelf-group-toggle" onclick="event.stopPropagation();event.preventDefault();$('#${id}').collapse('toggle');">
                    <b>Item Code</b><span class="toggle-icon">▾</span>
                </div>
                <div class="collapse show" id="${id}">
                    <div class="shelf-group-content" style="max-height:${icHeight}px;overflow-y:auto;overflow-x:hidden;">
                        <ul style="padding:0 12px;margin:0;">${items}</ul>
                    </div>
                </div>
            </div>`);
    }

    if (!groups.length) return '';

    return `
        <div class="shelf-display-content" style="
            position:absolute;top:20px;left:0;right:0;bottom:0;
            display:flex;flex-direction:column;gap:2px;
            padding:3px;overflow:hidden;
            pointer-events:auto;z-index:10;
            font-size:9px;word-break:break-word;line-height:1.4;background:transparent;">
            ${groups.join('')}
        </div>`;
}

function refreshCellContent() {
    const isAnyChecked = $('#cbShowKH, #cbShowMH, #cbShowItemCode').is(':checked');

    $('#materialArea .shelf, #container-khuvucphulieu .shelf, .walkway .shelf, .walkway-horizontal .shelf').each(function () {
        const keID = $(this).data('keid');
        const module = parseInt($(this).attr('data-module')) || 1;
        const shelf = dataDanhSachKe.find(x => x.KeID === keID && x.Module == module);
        if (!shelf) return;

        $(this).find('.shelf-display-content').remove();
        $(this).find('.cell').removeClass('hide-tooltip');

        const $walkwayH = $(this).closest('.walkway-horizontal');
        const shelfCountInWalkH = $walkwayH.length > 0 ? $walkwayH.find('.shelf').length : 0;
        const isNDay = (shelf.TenDay || '').toLowerCase().includes('n');

        if (isAnyChecked) {
            //  Tính targetHeight 
            if (isNDay) {
                const $walkway = $(this).closest('.walkway, .walkway-horizontal');
                let targetHeight = totalHeightA || 150;
                const module = $walkway.data('module')
                if ($walkwayH.length > 0) {
                    if (shelfCountInWalkH === 1) {
                        const minH = module == 1 ? totalWidthWalkwayHorizontal / 2 : totalWidthWalkwayHorizontalPL / 2;
                        $walkwayH.css('height', '120px');
                        const walkwayH = $walkwayH.outerHeight();
                        const translateY = (minH / 2) - (walkwayH / 2) + 29;
                        $(this).css('transform', `rotate(-90deg) translateY(${translateY}px)`);

                        // Chia đều height cho các group
                        const $groups = $(this).find('.shelf-group');
                        const groupCount = $groups.length;
                        if (groupCount > 0) {
                            const availableH = minH - (groupCount * 18) - 20;
                            const perGroupH = Math.floor(availableH / groupCount);
                            $groups.find('.shelf-group-content').css({
                                'max-height': `${Math.max(perGroupH, 40)}px`,
                                'overflow-y': 'auto',
                            });
                        }
                    } else {
                        // Horizontal nhiều shelf
                        $walkwayH.css({ 'height': '220px', 'gap': '8px' });
                        $walkwayH.find('.shelf').css({
                            'transform': '', 'width': '100px',
                            'min-height': '200px', 'max-height': '200px', 'overflow': 'hidden'
                        });
                        $walkwayH.find('.cell').css({ 'min-height': '199px', 'max-height': '199px' });

                        // Scroll cho shelf-group-content
                        $walkwayH.find('.shelf').each(function () {
                        const $groups = $(this).find('.shelf-group');
                            const groupCount = $groups.length;
                            if (groupCount === 0) return;

                            const containerH = 200;
                            const headerH = 18;
                            const availableH = containerH - (groupCount * headerH) - 20;
                            const perGroupH = Math.floor(availableH / groupCount);

                            $groups.find('.shelf-group-content').css({
                                'max-height': `${Math.max(perGroupH, 40)}px`,  
                                'overflow-y    ': 'auto',
                                'overflow-x': 'hidden'
                            });
                        });
                    }
                } else if ($walkway.length > 0) {
                    // Walkway dọc
                    const $outer = $walkway.parent();
                    let mh = 0;
                    $outer.find('.shelf').not(this).each(function () {
                        if (!$(this).closest('.walkway, .walkway-horizontal').length) {
                            mh = Math.max(mh, $(this).outerHeight());
                        }
                    });
                    if (mh > 0) targetHeight = mh;


                } else {
                    // Shelf thường ngoài walkway
                    const $cont = $(this).closest('#materialArea, #container-khuvucphulieu');
                    let mh = 0;
                    $cont.find('.shelf').not(this).each(function () {
                        mh = Math.max(mh, $(this).outerHeight());
                    });
                    if (mh > 0) targetHeight = mh;
                }

                if (shelfCountInWalkH !== 1) {
                    const h = shelfCountInWalkH > 1 ? 200 : targetHeight;
                    $(this).css({ 'min-height': `${h - 25}px`, 'max-height': `${h}px` });
                    $(this).find('.cell').css({ 'min-height': `${h - 3}px`, 'max-height': `${h}px` });
                }
            }

            //  Render HTML 
            const html = getCellDisplayHTML(shelf, keID);
            if (!html) return;
            $(this).append(html);
            $(this).find('.cell').addClass('hide-tooltip');

        } else {

            if ($walkwayH.length > 0) {
                if (shelfCountInWalkH === 1) {
                    const $s = $walkwayH.find('.shelf');
                    const minH = module == 1 ? totalWidthWalkwayHorizontal / 2 : totalWidthWalkwayHorizontalPL / 2;
                    $walkwayH.css('height', '120px');
                    const walkwayH = $walkwayH.outerHeight();
                    const translateY = (minH / 2) - (walkwayH / 2) + 29;
                    const cur = $s[0].style.transform;
                    if (cur) {
                        $s.css({
                            'transform': `rotate(-90deg) translateY(${translateY}px)`,
                            'min-height': `${minH}px`,
                            'max-height': `${minH}px`
                        });
                    }
                    $walkwayH.find('.cell').css({ 'min-height': `${minH - 1}px`, 'max-height': `${minH - 1}px` });
                } else {
                    // Reset horizontal nhiều shelf
                    $walkwayH.css({ 'height': '110px', 'gap': '8px' });
                    $walkwayH.find('.shelf').css({ 'transform': '', 'width': '100px', 'min-height': '80px', 'max-height': '80px' });
                    $walkwayH.find('.cell').css({ 'min-height': '78px', 'max-height': '78px' });
                }
            }
        }
    });
}

async function shelfOnDrag(keID, walkwayID, module = 1) {
    try {
        const url = `/api/ViTriKhoNPL/GetSoDoKhoDragDrop?action=Post&para1=${keID}&para2=${walkwayID}&para3=${module}`;
        const response = await fetch(url);
        if (!response.ok) {
            showToast("error", "Lỗi khi di chuyển kệ")
            return;
        }
       /* showToast("success", "Di chuyển kệ thành công")*/
    } catch (err) {
        showToast("error", "Lỗi khi di chuyển kệ")
    }
}

function getWalkwaysByModule(module) {
    return $('.walkway-horizontal').filter(function () {
        return parseInt($(this).data('module')) === parseInt(module);
    });
}
/// Module Xem Chi Tiết
let soDoKhoNL = [];
let soDoKhoPL = [];
let btnModule;
/// Event
$(document).ready(function () {
    GetThongTinKeNPL()
})
$(function () {
    $('.btnXemChiTiet').on('click', function () {
        btnModule = $(this).data('module')
        $('#txtKeNPL').text(`${btnModule == 1 ? 'Nguyên Liệu' : 'Phụ Liệu'}`)
        if (btnModule == 1) {
            buildSoDoKho(soDoKhoNL)
        } else {
            buildSoDoKho(soDoKhoPL)
        }
        $('#shelfDetailModal').modal("show");
    })
})

/// Api

async function GetThongTinKeNPL() {
    try {
        const url = `/api/ViTriKhoNPL/GetSoDoKhoDragDrop?action=GetThongTinSoDoKho`;

        const response = await fetch(url);
        const data = await response.json();

        soDoKhoNL = data.filter(item => item.Module == 1 && !(String(item.TenDay).toLowerCase().includes('n') || String(item.TenDay).toLowerCase().includes('lỗi')));
        soDoKhoPL = data.filter(item => item.Module == 2 && !(String(item.TenDay).toLowerCase().includes('n') || String(item.TenDay).toLowerCase().includes('lỗi')));
    } catch (err) {
        console.error(err)
    }
}

function buildSoDoKho(data) {
    let groupKeID = new Map();
    data.forEach(item => {
        let parts = item.TenO.split(".");
        let tang = parseInt(parts[2]);
        if (!groupKeID.has(item.KeID)) {
            groupKeID.set(item.KeID, {
                tenKe: item.TenKe,
                soTang: 0,
                maxO: 0,
                dayID: item.DayID,
                tang: {}
            });
        }
        let ke = groupKeID.get(item.KeID);
        if (!ke.tang[tang]) {
            ke.tang[tang] = [];
        }
        ke.tang[tang].push({ tenO: item.TenO });
        ke.soTang = Object.keys(ke.tang).length;
        ke.maxO = Math.max(ke.maxO, ke.tang[tang].length);
    });

    const sortedKe = [...groupKeID.entries()].sort((a, b) =>
        a[1].tenKe.localeCompare(b[1].tenKe, undefined, { numeric: true })
    );

    const $shelfBody = $("#shelfBody");

    let html = ``;
    const CELL_W = 102;
    const CELL_H = 24;
    const KE_W = 52;

    // Track DayID đã xuất hiện chưa
    const seenDayIDs = new Set();

    sortedKe.forEach(([key, value]) => {
        let tangHTML = ``;
        const sortedTangs = Object.keys(value.tang)
            .map(Number)
            .sort((a, b) => b - a);

        sortedTangs.forEach((tang, tangIndex) => {
            const cells = (value.tang[tang] || []).filter(o => o.tenO && o.tenO.trim() !== '');
            if (cells.length === 0) return;

            const isFirstRow = tangIndex === 0;
            const isLastRow = tangIndex === sortedTangs.length - 1;

            const fullCells = Array.from({ length: value.maxO }, (_, idx) => cells[idx] || null);

            tangHTML += `<div style="display:flex; flex-wrap:nowrap;">`;
            fullCells.forEach((o, colIndex) => {
                const isFirstCol = colIndex === 0;
                const isLastCol = colIndex === fullCells.length - 1;

                let bt = isFirstRow ? '2px solid #1a237e' : '1px solid #c5cae9';
                let bb = isLastRow ? '2px solid #1a237e' : '1px solid #c5cae9';
                let bl = isFirstCol ? '2px solid #1a237e' : '1px solid #c5cae9';
                let br = isLastCol ? '2px solid #1a237e' : '1px solid #c5cae9';

                if (!isFirstCol) bl = 'none';
                if (!isFirstRow) bt = 'none';

                tangHTML += `
                    <div style="
                        width:${CELL_W}px;
                        min-width:${CELL_W}px;
                        height:${CELL_H}px;
                        display:flex;
                        align-items:center;
                        justify-content:center;
                        font-size:10px;
                        color:#130eb2;
                        background:#fff;
                        box-sizing:border-box;
                        border-top:${bt};
                        border-bottom:${bb};
                        border-left:${bl};
                        border-right:${br};
                        overflow:hidden;
                        white-space:nowrap;
                    ">${o ? o.tenO : ''}</div>`;
            });
            tangHTML += `</div>`;
        });

        const gridW = value.maxO * CELL_W;

        const isFirstInDay = !seenDayIDs.has(value.dayID);
        const marginTop = isFirstInDay ? '24px' : '0px';
        seenDayIDs.add(value.dayID);

        html += `
            <div class="d-flex flex-column justify-content-start align-items-start">
                <div style="margin-top:${marginTop}; display:flex; flex-wrap:nowrap; align-items:flex-start; gap:6px;">
                <!-- Tên kệ -->
                <div class='shelfTenKeChiTiet' style="
                    flex-shrink:0;
                    width:${KE_W}px;
                    min-width:${KE_W}px;
                    min-height:${CELL_H * value.soTang}px;
                    display:flex;
                    align-items:center;
                    justify-content:center;
                    font-size:13px;
                    font-weight:600;
                    color:#130eb2;
                    text-align:center;
                    word-break:break-word;
                    padding:2px;
                ">${value.tenKe}</div>

                <!-- Grid ô - scroll ngang nếu tràn -->
                <div style="
                    overflow-x:auto;
                    -webkit-overflow-scrolling:touch;
                    flex:1;
                    min-width:0;
                ">
                    <div style="width:${gridW}px; min-width:${gridW}px;">
                        ${tangHTML}
                    </div>
                </div>
            </div>
        </div>
        `;
    });

    $shelfBody.html(html);
}

(function () {
    const DEFAULT_ZOOM = 65;
    let zoomPct = DEFAULT_ZOOM;
    const STEP = 5;
    const MIN = 30;
    const MAX = 200;

    function applyZoom() {
        const content = document.querySelector('#shelfDetailModal .modal-body');
        if (!content) return;
        content.style.zoom = zoomPct / 100;
        document.getElementById('zoomLabel').textContent = zoomPct + '%';
    }

    document.addEventListener('click', function (e) {
        if (e.target.closest('#btnZoomIn')) {
            zoomPct = Math.min(MAX, zoomPct + STEP);
            applyZoom();
        }
        if (e.target.closest('#btnZoomOut')) {
            zoomPct = Math.max(MIN, zoomPct - STEP);
            applyZoom();
        }
        if (e.target.closest('#btnZoomReset')) {
            zoomPct = DEFAULT_ZOOM;
            applyZoom();
        }
    });

    document.getElementById('shelfDetailModal')?.addEventListener('show.bs.modal', function () {
        zoomPct = DEFAULT_ZOOM;
        applyZoom();
    });
})();

document.addEventListener('click', async function (e) {
    if (!e.target.closest('#btnExportPDF')) return;

    const btnExport = document.getElementById('btnExportPDF');
    const modalBody = document.querySelector('#shelfDetailModal .modal-body');
    const content = document.getElementById('shelfBody');

    btnExport.disabled = true;
    btnExport.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

    // Lưu lại style gốc
    const savedZoom = modalBody.style.zoom || '';
    const savedOverflowMB = modalBody.style.overflow || '';
    const savedOverflowC = content.style.overflow || '';
    const savedWidth = content.style.width || '';
    const savedMaxWidth = content.style.maxWidth || '';

    try {
        // Reset zoom về 1 để đo đúng
        modalBody.style.zoom = '1';
        modalBody.style.overflow = 'visible';

        // Bỏ giới hạn width để content trải ra đủ
        content.style.overflow = 'visible';
        content.style.width = 'max-content';
        content.style.maxWidth = 'none';

        // Bỏ overflow-x của tất cả scroll containers bên trong
        const scrollDivs = content.querySelectorAll('[style*="overflow-x"]');
        const savedScrollStyles = [];
        scrollDivs.forEach(el => {
            savedScrollStyles.push(el.style.overflowX);
            el.style.overflowX = 'visible';
        });

        // Đợi browser reflow
        await new Promise(r => setTimeout(r, 150));

        const realW = content.scrollWidth;
        const realH = content.scrollHeight;

        const canvas = await html2canvas(content, {
            scale: 2,
            useCORS: true,
            backgroundColor: '#ffffff',
            scrollX: 0,
            scrollY: -window.scrollY,
            x: 0,
            y: 0,
            width: realW,
            height: realH,
            windowWidth: realW + 50,
            windowHeight: realH + 50
        });

        // Restore scroll divs
        scrollDivs.forEach((el, i) => {
            el.style.overflowX = savedScrollStyles[i];
        });

        const { jsPDF } = window.jspdf;

        // Chọn orientation dựa trên tỷ lệ canvas
        const isLandscape = canvas.width > canvas.height;
        const pdf = new jsPDF(isLandscape ? 'l' : 'p', 'pt', 'a4');

        const A4_W = isLandscape ? 841.89 : 595.28;
        const A4_H = isLandscape ? 595.28 : 841.89;
        const MARGIN = 20;
        const usableW = A4_W - MARGIN * 2;
        const usableH = A4_H - MARGIN * 2;

        const imgW = canvas.width;
        const imgH = canvas.height;
        const fitScale = Math.min(usableW / imgW, usableH / imgH);

        const finalW = imgW * fitScale;
        const finalH = imgH * fitScale;
        const offsetX = MARGIN + (usableW - finalW) / 2;
        const offsetY = MARGIN + (usableH - finalH) / 2;

        const imgData = canvas.toDataURL('image/png');
        pdf.addImage(imgData, 'PNG', offsetX, offsetY, finalW, finalH);
        pdf.save(`SoDoKho_${btnModule == 1 ? 'NL' : 'PL'}.pdf`);

    } catch (err) {
        console.error(err);
        alert('Xuất PDF thất bại: ' + err.message);
    } finally {
        // Restore tất cả
        modalBody.style.zoom = savedZoom;
        modalBody.style.overflow = savedOverflowMB;
        content.style.overflow = savedOverflowC;
        content.style.width = savedWidth;
        content.style.maxWidth = savedMaxWidth;

        btnExport.disabled = false;
        btnExport.innerHTML = '<i class="bi bi-file-earmark-pdf-fill text-danger"></i>';
    }
});

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