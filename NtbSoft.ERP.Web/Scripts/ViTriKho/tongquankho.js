$(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
    GetViTriKho()
})

var warehouseData;

async function GetViTriKho() {
    const url = `/api/ViTriKhoNPL/Get?action=GetViTriKho`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        warehouseData = await transformData(data.Table) // Thêm await ở đây
        renderFilters()
        document.getElementById('warehouseAisles').innerHTML = generateWarehouseHTML(); // Bỏ comment
        updateStats(); // Bỏ comment
    } catch (error) {
        console.error(error.message);
    }
}

async function fetchSlotItems(slotId) {
    const url = `/api/ViTriKhoNPL/Get?action=GetViTriKhoITem&slotId=${slotId}`; // Thêm slotId param
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        //return data.Table || []; // Trả về Table hoặc array rỗng
        var dataa = [
            { name: "Dây điện 2.5mm", quantity: "5 cuộn", cbm: 1.2 },
            { name: "Công tắc điện", quantity: "20 cái", cbm: 0.7 }
        ]
        return dataa
    } catch (error) {
        console.error(error.message);
        return [];
    }
}

async function transformData(data) {
    const aislesMap = {};

    // Sử dụng for...of thay vì forEach để handle async properly
    for (const item of data) {
        const aisleId = item.TenDay.split(" ")[1]; // "A", "B", "C"
        const shelfId = item.TenKe.split(" ")[1];  // "1", "2", "3"
        const levelId = item.TenTang.split(" ")[1]; // "1", "2", ...

        if (!aislesMap[aisleId]) {
            aislesMap[aisleId] = {
                id: aisleId,
                name: item.TenDay,
                description: `Khu vực ${item.TenDay}`, // Thêm description
                shelves: {}
            };
        }

        if (!aislesMap[aisleId].shelves[shelfId]) {
            aislesMap[aisleId].shelves[shelfId] = {
                id: shelfId,
                name: item.TenKe,
                levels: {}
            };
        }

        if (!aislesMap[aisleId].shelves[shelfId].levels[levelId]) {
            aislesMap[aisleId].shelves[shelfId].levels[levelId] = {
                id: levelId,
                name: item.TenTang,
                slots: []
            };
        }

        // Fetch items cho slot này
      

        aislesMap[aisleId].shelves[shelfId].levels[levelId].slots.push({
            id: item.TenO,
            cbm: item.CBM,
            usedCBM: item.CBM - item.CBMCL,
            items: [] // Thêm items vào slot
        });
    }

    // Convert object về array + sort
    const aisles = Object.values(aislesMap)
        .sort((a, b) => a.id.localeCompare(b.id)) // sort dãy
        .map(aisle => ({
            ...aisle,
            shelves: Object.values(aisle.shelves)
                .sort((a, b) => Number(a.id) - Number(b.id)) // sort kệ
                .map(shelf => ({
                    ...shelf,
                    levels: Object.values(shelf.levels)
                        .sort((a, b) => Number(a.id) - Number(b.id)) // sort tầng
                }))
        }));
    return { aisles };
}

let currentFilter = 'all';

// Tính toán class dựa trên capacity
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

function generateWarehouseHTML() { // Bỏ tham số data không dùng
    let html = '';
    warehouseData.aisles.forEach(aisle => {
        html += `
                <div class="col-xl-4 col-12 mb-4">
                    <div class="card aisle-card " data-aisle="${aisle.id}">
                        <div class="card-header bg-primary text-white">
                            <h5 class="mb-0">
                                <i class="bi bi-building-fill"></i> ${aisle.name}
                            </h5>
                            ${aisle.description ? `<small>${aisle.description}</small>` : ''}
                        </div>
                        <div class="card-body">`;

        aisle.shelves.forEach(shelf => {
            html += `
                    <div class="shelf-section card mb-3">
                        <div class="card-header bg-secondary text-white py-2">
                            <h6 class="mb-0">
                                <i class="bi bi-bookshelf"></i> ${shelf.name}
                            </h6>
                        </div>
                        <div class="card-body py-2">`;

            // Đảo ngược thứ tự levels để tầng cao hiển thị trên tầng thấp
            const reversedLevels = [...shelf.levels];
            reversedLevels.forEach(level => {
                html += `
                        <div class="row align-items-center mb-2">
                            <div class="col-auto">
                                <span class="badge level-badge">
                                    <i class="bi bi-layers"></i> ${level.name}
                                </span>
                            </div>
                            <div class="col">
                                <div class="d-flex flex-wrap gap-1">`;

                level.slots.forEach(slot => {
                    const slotNumber = slot.id.split('.').pop();
                    const capacityClass = getCapacityClass(slot.usedCBM, slot.cbm);
                    const remainingCapacity = getRemainingCapacity(slot.usedCBM, slot.cbm);
                    const itemCount = slot.items ? slot.items.length : 0;
               /*        ${itemCount > 0 ? `<div class="item-count">${itemCount}</div>` : ''}*/
                    html += `
                            <div class="slot-item ${capacityClass} position-relative" 
                                 data-id="${slot.id}" 
                                 data-aisle="${aisle.id}"
                                 data-shelf="${shelf.id}"
                                 data-level="${level.id}"
                                 data-cbm="${slot.cbm}"
                                 data-used-cbm="${slot.usedCBM}"
                                 data-remaining="${remainingCapacity}"
                                 data-items='${JSON.stringify(slot.items)}'>
                                ${slotNumber}
                              
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
                </div>`;
    });
    return html;
}

function updateStats() {
    let total = 0, capacityEmpty = 0, capacityHigh = 0;
    document.querySelectorAll('.slot-item').forEach(slot => {
        const aisle = slot.getAttribute('data-aisle');
        if (currentFilter === 'all' || aisle === currentFilter) {
            const aisleCard = slot.closest('.aisle-card');
            if (aisleCard && !aisleCard.classList.contains('filtered-out')) {
                total++;
                if (slot.classList.contains('capacity-empty')) capacityEmpty++;
                if (slot.classList.contains('capacity-high')) capacityHigh++;
            }
        }
    });

    const warningRate = total > 0 ? Math.round(((capacityEmpty + document.querySelectorAll('.capacity-low').length) / total) * 100) : 0;

    const totalSlotsEl = document.getElementById('totalSlots');
    const availableSlotsEl = document.getElementById('availableSlots');
    const occupiedSlotsEl = document.getElementById('occupiedSlots');
    const utilizationRateEl = document.getElementById('utilizationRate');

    if (totalSlotsEl) totalSlotsEl.textContent = total;
    if (availableSlotsEl) availableSlotsEl.textContent = capacityHigh;
    if (occupiedSlotsEl) occupiedSlotsEl.textContent = capacityEmpty;
    if (utilizationRateEl) utilizationRateEl.textContent = warningRate + '%';
}

function applyFilter(filter) {
    currentFilter = filter;
    document.querySelectorAll('.aisle-card').forEach(card => {
        card.classList.remove('filtered-out');
        if (filter !== 'all' && card.getAttribute('data-aisle') !== filter) {
            card.classList.add('filtered-out');
        }
    });
    updateStats();
}

function searchSlots(searchTerm) {
    document.querySelectorAll('.slot-item').forEach(slot => {
        slot.classList.remove('highlighted');
    });

    if (!searchTerm) return;

    const term = searchTerm.toLowerCase();
    let firstMatch = null;

    document.querySelectorAll('.slot-item').forEach(slot => {
        const slotId = slot.getAttribute('data-id')?.toLowerCase() || '';

        const aisleCard = slot.closest('.aisle-card');
        const shelfSection = slot.closest('.shelf-section');
        const levelRow = slot.closest('.row');

        const aisleName = aisleCard ? aisleCard.querySelector('.card-header h5')?.textContent?.toLowerCase() || '' : '';
        const shelfName = shelfSection ? shelfSection.querySelector('.card-header h6')?.textContent?.toLowerCase() || '' : '';
        const levelName = levelRow ? levelRow.querySelector('.level-badge')?.textContent?.toLowerCase() || '' : '';

        if (slotId.includes(term) ||
            aisleName.includes(term) ||
            shelfName.includes(term) ||
            levelName.includes(term)) {
            slot.classList.add('highlighted');
            if (!firstMatch) {
                firstMatch = slot;
                slot.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    });
}

function showTooltip(e, slot) {
    const tooltip = document.getElementById('tooltip');
    if (!tooltip) return;

    const slotId = slot.getAttribute('data-id');
    const cbm = parseFloat(slot.getAttribute('data-cbm'));
    const usedCBM = parseFloat(slot.getAttribute('data-used-cbm'));
    const remaining = cbm - usedCBM;
    const remainingPercentage = slot.getAttribute('data-remaining');

    const aisleCard = slot.closest('.aisle-card');
    const shelfSection = slot.closest('.shelf-section');
    const levelRow = slot.closest('.row');

    // Kiểm tra tồn tại của các elements
    const aisleName = aisleCard ? aisleCard.querySelector('.card-header h5')?.textContent || 'N/A' : 'N/A';
    const shelfName = shelfSection ? shelfSection.querySelector('.card-header h6')?.textContent || 'N/A' : 'N/A';
    const levelName = levelRow ? levelRow.querySelector('.level-badge')?.textContent || 'N/A' : 'N/A';

    // Lấy items từ data attribute
    const items = JSON.parse(slot.getAttribute('data-items') || '[]');

    let capacityStatus = '';
    if (slot.classList.contains('capacity-empty')) capacityStatus = 'Đã đầy (0%)';
    else if (slot.classList.contains('capacity-high')) capacityStatus = `Còn nhiều chỗ (${remainingPercentage}%)`;
    else if (slot.classList.contains('capacity-medium')) capacityStatus = `Còn vừa phải (${remainingPercentage}%)`;
    else if (slot.classList.contains('capacity-low')) capacityStatus = `Sắp đầy (${remainingPercentage}%)`;

    let itemsHtml = '';
    if (items.length > 0) {
        itemsHtml = '<div><strong>Vật phẩm:</strong></div>';
        items.forEach(item => {
            itemsHtml += `<div style="font-size: 12px;">- ${item.name} (${item.quantity})</div>`;
        });
    }

    tooltip.innerHTML = `
                <div><strong><i class="bi bi-geo-alt"></i> ${slotId}</strong></div>
                <div><i class="bi bi-building"></i> ${aisleName}</div>
                <div><i class="bi bi-bookshelf"></i> ${shelfName}</div>
                <div><i class="bi bi-layers"></i> ${levelName}</div>
                <div><i class="bi bi-box"></i> CBM: ${usedCBM}/${cbm} (Còn: ${remaining.toFixed(1)})</div>
                <div><i class="bi bi-speedometer2"></i> ${capacityStatus}</div>
            `;
    tooltip.style.left = (e.pageX + 15) + 'px';
    tooltip.style.top = (e.pageY - 10) + 'px';
    tooltip.style.opacity = '1';
}

function hideTooltip() {
    const tooltip = document.getElementById('tooltip');
    if (tooltip) {
        tooltip.style.opacity = '0';
    }
}

// Initialize
document.addEventListener('DOMContentLoaded', function () {
    // Filter buttons
    document.querySelectorAll('label[data-filter]').forEach(btn => {
        btn.addEventListener('click', function () {
            const filter = this.getAttribute('data-filter');
            applyFilter(filter);
        });
    });

    // Search functionality
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const searchTerm = this.value;
            searchSlots(searchTerm);
        });
    }

    // Tooltip events
    document.addEventListener('mouseenter', function (e) {
        if (e.target.classList.contains('slot-item')) {
            showTooltip(e, e.target);
        }
    }, true);

    document.addEventListener('mouseleave', function (e) {
        if (e.target.classList.contains('slot-item')) {
            hideTooltip();
        }
    }, true);

    document.addEventListener('mousemove', function (e) {
        if (e.target.classList.contains('slot-item')) {
            const tooltip = document.getElementById('tooltip');
            tooltip.style.left = (e.pageX + 15) + 'px';
            tooltip.style.top = (e.pageY - 10) + 'px';
        }
    });

    // Click events for slots
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('slot-item')) {
            document.querySelectorAll('.slot-item').forEach(slot => {
                slot.classList.remove('highlighted');
            });
            e.target.classList.add('highlighted');

            // Show toast notification
            const slotId = e.target.getAttribute('data-id');
            const toast = document.createElement('div');
            toast.className = 'toast position-fixed top-0 end-0 m-3';
            toast.style.zIndex = '9999';
            toast.innerHTML = `
                        <div class="toast-header bg-primary text-white">
                            <strong class="me-auto">Đã chọn vị trí</strong>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
                        </div>
                        <div class="toast-body">
                            Vị trí: <strong>${slotId}</strong>
                        </div>
                    `;
            document.body.appendChild(toast);
            const bsToast = new bootstrap.Toast(toast);
            bsToast.show();

            toast.addEventListener('hidden.bs.toast', function () {
                document.body.removeChild(toast);
            });

            // Auto remove highlight after 3 seconds
            setTimeout(() => {
                e.target.classList.remove('highlighted');
            }, 3000);
        }
    });

    // Keyboard shortcuts
    document.addEventListener('keydown', function (e) {
        if (e.ctrlKey && e.key === 'f') {
            e.preventDefault();
            const searchInput = document.getElementById('searchInput');
            if (searchInput) {
                searchInput.focus();
            }
        }
        if (e.key === 'Escape') {
            const searchInput = document.getElementById('searchInput');
            if (searchInput) {
                searchInput.value = '';
            }
            document.querySelectorAll('.slot-item').forEach(slot => {
                slot.classList.remove('highlighted');
            });
        }
    });
});

// Additional utility functions
function getSlotsByCapacity(capacityClass) {
    return Array.from(document.querySelectorAll('.slot-item')).filter(slot => {
        return slot.classList.contains(capacityClass);
    });
}

function getAisleStats(aisleId) {
    const aisleSlots = document.querySelectorAll(`[data-aisle="${aisleId}"]`);
    const total = aisleSlots.length;
    let totalCBM = 0;
    let usedCBM = 0;

    Array.from(aisleSlots).forEach(slot => {
        totalCBM += parseFloat(slot.getAttribute('data-cbm'));
        usedCBM += parseFloat(slot.getAttribute('data-used-cbm'));
    });

    const capacityUtilization = totalCBM > 0 ? Math.round((usedCBM / totalCBM) * 100) : 0;
    const remainingCapacity = totalCBM > 0 ? Math.round(((totalCBM - usedCBM) / totalCBM) * 100) : 0;

    return {
        total,
        totalCBM: totalCBM.toFixed(1),
        usedCBM: usedCBM.toFixed(1),
        capacityUtilization,
        remainingCapacity
    };
}

// Export function for future use
function exportWarehouseData() {
    const exportData = {
        timestamp: new Date().toISOString(),
        filter: currentFilter,
        data: warehouseData,
        capacityAnalysis: {
            totalSlots: document.getElementById('totalSlots').textContent,
            highCapacitySlots: document.getElementById('availableSlots').textContent,
            fullSlots: document.getElementById('occupiedSlots').textContent,
            warningRate: document.getElementById('utilizationRate').textContent
        }
    };
    console.log('Warehouse capacity data export:', exportData);
    return exportData;
}

// Function để tạo báo cáo
function generateReport() {
    const report = {
        summary: {
            totalSlots: parseInt(document.getElementById('totalSlots').textContent),
            availableSlots: parseInt(document.getElementById('availableSlots').textContent),
            occupiedSlots: parseInt(document.getElementById('occupiedSlots').textContent),
            warningRate: document.getElementById('utilizationRate').textContent
        },
        aisles: []
    };

    warehouseData.aisles.forEach(aisle => {
        const stats = getAisleStats(aisle.id);
        report.aisles.push({
            id: aisle.id,
            name: aisle.name,
            stats: stats
        });
    });

    return report;
}

// Function để highlight các slot theo điều kiện
function highlightSlotsByCondition(condition) {
    document.querySelectorAll('.slot-item').forEach(slot => {
        slot.classList.remove('highlighted');
        const usedCBM = parseFloat(slot.getAttribute('data-used-cbm'));
        const totalCBM = parseFloat(slot.getAttribute('data-cbm'));
        const remainingPercent = ((totalCBM - usedCBM) / totalCBM) * 100;

        let shouldHighlight = false;
        switch (condition) {
            case 'empty':
                shouldHighlight = usedCBM === 0;
                break;
            case 'full':
                shouldHighlight = remainingPercent === 0;
                break;
            case 'warning':
                shouldHighlight = remainingPercent < 50 && remainingPercent > 0;
                break;
            case 'available':
                shouldHighlight = remainingPercent > 80;
                break;
        }

        if (shouldHighlight) {
            slot.classList.add('highlighted');
        }
    });
}

function renderFilters() {
    const filterGroup = document.getElementById('filterGroup');
    if (!filterGroup) return; // Kiểm tra element tồn tại

    let html = '';

    // Nút Tất cả
    html += `
       <div>
             <input type="radio" class="btn-check" name="filter" id="filter-all" checked>
             <label class="btn btn-outline-primary" for="filter-all" data-filter="all">Tất cả</label>
       </div>
    `;

    // Nút động cho từng Dãy
    warehouseData.aisles.forEach(aisle => {
        const id = `filter-${aisle.id}`;
        html += `
           <div>
            <input type="radio" class="btn-check" name="filter" id="${id}">
            <label class="btn btn-outline-primary" for="${id}" data-filter="${aisle.id}">${aisle.name}</label>
             </div>
        `;
    });

    filterGroup.innerHTML = html;

    // Gắn sự kiện click sau khi render
    filterGroup.querySelectorAll('label[data-filter]').forEach(btn => {
        btn.addEventListener('click', function () {
            const filter = this.getAttribute('data-filter');
            applyFilter(filter);
        });
    });
}
function chitietkho() {
    location.href = '/ViTriKho/NhapViTri'
}