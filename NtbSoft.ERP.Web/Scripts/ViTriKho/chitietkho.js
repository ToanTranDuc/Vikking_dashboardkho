$(function () {
    $("#home").on("click", function () {
        window.location.href = '/Home/Dashboard'
    })
    if (window.CefSharp) {
        $("#home").css("display", "none");
    }
    $(".enterbarcode").on("click", function () {
        $("#globalSearchInput").val($("#globalBarcode").val())
        $("#globalSearchInput").focus()
    })
    GetViTriKho()
})

var warehouseDetailData

async function GetViTriKho() {
    const url = `/api/ViTriKhoNPL/Get?action=GetViTriKho`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        warehouseDetailData = await transformData(data.Table) // Thêm await ở đây
        showAisleSelection();
        setupSearch();
        setupGlobalSearch();

      
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
        const items = await fetchSlotItems(item.TenO);

        aislesMap[aisleId].shelves[shelfId].levels[levelId].slots.push({
            id: item.TenO,
            cbm: item.CBM,
            usedCBM: item.CBM - item.CBMCL,
            items: items // Thêm items vào slot
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



let currentView = 'selection'; // 'selection' hoặc 'detail'
let currentAisleData = null;

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

// Hiển thị giao diện chọn dãy
function showAisleSelection() {
    currentView = 'selection';
    document.getElementById('aisleSelectionView').style.display = 'block';
    document.getElementById('detailView').style.display = 'none';
    //document.getElementById('backBtn').style.display = 'none';
    //document.getElementById('breadcrumb').style.display = 'none';

    renderAisleCards();
}

// Render các card chọn dãy
function renderAisleCards() {
    const container = document.getElementById('aisleCards');
    let html = '';

    warehouseDetailData.aisles.forEach(aisle => {
        // Tính tổng số ô
        let totalSlots = 0;
        let occupiedSlots = 0;
        let availableSlots = 0;

        aisle.shelves.forEach(shelf => {
            shelf.levels.forEach(level => {
                level.slots.forEach(slot => {
                    totalSlots++;
                    const remainingPercent = getRemainingCapacity(slot.usedCBM, slot.cbm);
                    if (remainingPercent === 0) occupiedSlots++;
                    else if (remainingPercent > 80) availableSlots++;
                });
            });
        });

        html += `
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="card aisle-selection-card h-100" onclick="showAisleDetail('${aisle.id}')">
                        <div class="card-body text-center d-flex flex-column justify-content-center">
                            <div class="aisle-icon">
                                <i class="bi bi-building-fill"></i>
                            </div>
                            <h3 class="text-primary">${aisle.name}</h3>
                            
                            <div class="row mt-auto">
                                <div class="col-4">
                                    <div class="fw-bold text-primary">${totalSlots}</div>
                                    <small>Tổng ô</small>
                                </div>
                                <div class="col-4">
                                    <div class="fw-bold text-success">${availableSlots}</div>
                                    <small>Còn chỗ</small>
                                </div>
                                <div class="col-4">
                                    <div class="fw-bold text-danger">${occupiedSlots}</div>
                                    <small>Đã đầy</small>
                                </div>
                            </div>
                            
                            <button class="btn btn-primary mt-3">
                                <i class="bi bi-eye"></i> Xem Chi Tiết
                            </button>
                        </div>
                    </div>
                </div>`;
    });

    container.innerHTML = html;
}

// Hiển thị chi tiết dãy
function showAisleDetail(aisleId) {
    currentView = 'detail';
    currentAisleData = warehouseDetailData.aisles.find(a => a.id === aisleId);

    if (!currentAisleData) return;

    document.getElementById('aisleSelectionView').style.display = 'none';
    document.getElementById('detailView').style.display = 'block';
    //document.getElementById('backBtn').style.display = 'block';
    //document.getElementById('breadcrumb').style.display = 'block';

    // Update header
    document.getElementById('detailTitle').innerHTML = `
                <i onclick="showAisleSelection()"  id="backBtn" class="bi bi-arrow-left"></i>
                Chi Tiết Kho - ${currentAisleData.name}
            `;
    //document.getElementById('currentAisle').textContent = currentAisleData.name;

    renderDetailContent();
    updateDetailStats();
}

// Render nội dung chi tiết
function renderDetailContent() {
    if (!currentAisleData) return;

    const container = document.getElementById('detailContent');
    let html = '';

    currentAisleData.shelves.forEach(shelf => {
        html += `
                <div class="shelf-header">
                    <i class="bi bi-bookshelf"></i> ${shelf.name}
                </div>`;

        // Đảo ngược thứ tự levels để tầng cao hiển thị trước
        const reversedLevels = [...shelf.levels].reverse();

        reversedLevels.forEach(level => {
            html += `
                    <div class="level-section">
                        <div class="level-header">
                            <i class="bi bi-layers"></i> ${level.name}
                        </div>
                        <div class="row justify-content-center">`;

            level.slots.forEach(slot => {
                const slotNumber = slot.id.split('.').pop();
                const capacityClass = getCapacityClass(slot.usedCBM, slot.cbm);
                const remainingCapacity = getRemainingCapacity(slot.usedCBM, slot.cbm);
                const itemCount = slot.items ? slot.items.length : 0;

                html += `
                        <div class="col-auto">
                            <div class="detail-slot ${capacityClass}" 
                                 onclick="showSlotDetail('${slot.id}')"
                                 data-id="${slot.id}" 
                                 data-aisle="${currentAisleData.id}"
                                 data-shelf="${shelf.id}"
                                 data-level="${level.id}"
                                 data-cbm="${slot.cbm}"
                                 data-used-cbm="${slot.usedCBM}"
                                 data-remaining="${remainingCapacity}">
                                
                                <div class="slot-id">${slotNumber}</div>
                                <div class="slot-capacity">${slot.usedCBM}/${slot.cbm} CBM</div>
                                <div class="slot-capacity">${remainingCapacity}% còn lại</div>
                                
                                ${itemCount > 0 ? `<div class="item-count">${itemCount}</div>` : ''}
                            </div>
                        </div>`;
            });

            html += `
                        </div>
                    </div>`;
        });
    });

    container.innerHTML = html;
}

// Cập nhật thống kê chi tiết
function updateDetailStats() {
    if (!currentAisleData) return;

    let totalSlots = 0;
    let occupiedSlots = 0;
    let availableSlots = 0;

    currentAisleData.shelves.forEach(shelf => {
        shelf.levels.forEach(level => {
            level.slots.forEach(slot => {
                totalSlots++;
                const remainingPercent = getRemainingCapacity(slot.usedCBM, slot.cbm);
                if (remainingPercent === 0) occupiedSlots++;
                else if (remainingPercent > 80) availableSlots++;
            });
        });
    });

    document.getElementById('detailTotalSlots').textContent = totalSlots;
    document.getElementById('detailAvailableSlots').textContent = availableSlots;
    document.getElementById('detailOccupiedSlots').textContent = occupiedSlots;
}

// Hiển thị chi tiết ô - Hoàn thiện phần này
function showSlotDetail(slotId) {
    if (!currentAisleData) return;

    let slotData = null;

    // Tìm slot data
    currentAisleData.shelves.forEach(shelf => {
        shelf.levels.forEach(level => {
            level.slots.forEach(slot => {
                if (slot.id === slotId) {
                    slotData = slot;
                }
            });
        });
    });

    if (!slotData) return;

    // Cập nhật thông tin modal
    document.getElementById('modalSlotTitle').innerHTML = `
                <i class="bi bi-box"></i> Chi tiết ô kho ${slotId}
            `;

    // Thông tin ô
    document.getElementById('modalSlotInfo').innerHTML = `
                <div class="fs-5 fw-bold text-primary">${slotId}</div>
                <small class="text-muted">Mã ô kho</small>
            `;

    // Thông tin sức chứa
    const remainingCapacity = getRemainingCapacity(slotData.usedCBM, slotData.cbm);
    const capacityClass = getCapacityClass(slotData.usedCBM, slotData.cbm);
    let capacityColor = 'success';
    if (capacityClass === 'capacity-empty') capacityColor = 'danger';
    else if (capacityClass === 'capacity-low') capacityColor = 'warning';
    else if (capacityClass === 'capacity-medium') capacityColor = 'info';

    document.getElementById('modalCapacityInfo').innerHTML = `
                <div class="fs-5 fw-bold text-${capacityColor}">${slotData.usedCBM}/${slotData.cbm} CBM</div>
                <small class="text-muted">${remainingCapacity}% còn trống</small>
                <div class="progress mt-2" style="height: 10px;">
                    <div class="progress-bar bg-${capacityColor}" role="progressbar" 
                         style="width: ${100 - remainingCapacity}%" 
                         aria-valuenow="${100 - remainingCapacity}" 
                         aria-valuemin="0" 
                         aria-valuemax="100">
                    </div>
                </div>
            `;

    // Danh sách vật tư
    let itemsHtml = '';
    if (slotData.items && slotData.items.length > 0) {
        slotData.items.forEach((item, index) => {
            itemsHtml += `
                    <div class="item-card">
                        <div class="row align-items-center">
                            <div class="col-md-6">
                                <div class="fw-bold text-primary">${item.name}</div>
                                <small class="text-muted">Vật tư #${index + 1}</small>
                            </div>
                            <div class="col-md-3">
                                <div class="fw-bold">${item.quantity}</div>
                                <small class="text-muted">Số lượng</small>
                            </div>
                            <div class="col-md-3">
                                <div class="fw-bold text-info">${item.cbm} CBM</div>
                                <small class="text-muted">Thể tích</small>
                            </div>
                        </div>
                    </div>`;
        });
    } else {
        itemsHtml = `
                <div class="text-center text-muted py-4">
                    <i class="bi bi-inbox fs-1"></i>
                    <div class="mt-2">Ô kho trống</div>
                </div>`;
    }

    document.getElementById('modalItemList').innerHTML = itemsHtml;

    // Hiển thị modal
    const modal = new bootstrap.Modal(document.getElementById('itemDetailModal'));
    modal.show();
}

// Chức năng tìm kiếm
function setupSearch() {
    const searchInput = document.getElementById('detailSearchInput');

    searchInput.addEventListener('input', function (e) {
        const searchTerm = e.target.value.toLowerCase().trim();

        // Xóa highlight cũ
        document.querySelectorAll('.detail-slot.highlighted').forEach(slot => {
            slot.classList.remove('highlighted');
        });

        if (searchTerm === '') return;

        // Tìm kiếm và highlight
        let foundSlots = [];

        currentAisleData.shelves.forEach(shelf => {
            shelf.levels.forEach(level => {
                level.slots.forEach(slot => {
                    let shouldHighlight = false;

                    // Tìm theo ID ô
                    if (slot.id.toLowerCase().includes(searchTerm)) {
                        shouldHighlight = true;
                    }

                    // Tìm theo tên vật tư
                    if (slot.items) {
                        slot.items.forEach(item => {
                            if (item.name.toLowerCase().includes(searchTerm)) {
                                shouldHighlight = true;
                            }
                        });
                    }

                    if (shouldHighlight) {
                        foundSlots.push(slot.id);
                    }
                });
            });
        });

        // Highlight các ô tìm thấy
        foundSlots.forEach(slotId => {
            const slotElement = document.querySelector(`[data-id="${slotId}"]`);
            if (slotElement) {
                slotElement.classList.add('highlighted');

                // Scroll đến ô đầu tiên tìm thấy
                if (foundSlots.indexOf(slotId) === 0) {
                    slotElement.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                }
            }
        });
    });
}

// Global Search Functions
function setupGlobalSearch() {
    const searchInput = document.getElementById('globalSearchInput');
    const searchResults = document.getElementById('searchResults');
    const clearBtn = document.getElementById('clearSearch');

    let searchTimeout;

    // Search input event
    searchInput.addEventListener('input', function (e) {
        const searchTerm = e.target.value.trim();

        // Clear previous timeout
        clearTimeout(searchTimeout);

        if (searchTerm.length < 1) {
            searchResults.style.display = 'none';
            return;
        }

        // Debounce search
        searchTimeout = setTimeout(() => {
            performGlobalSearch(searchTerm);
        }, 300);
    });

    // Clear button
    clearBtn.addEventListener('click', function () {
        searchInput.value = '';
        searchResults.style.display = 'none';
        searchInput.focus();
    });

    // Hide results when clicking outside
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.position-relative')) {
            searchResults.style.display = 'none';
        }
    });

    // Show results when focusing input
    searchInput.addEventListener('focus', function () {
        if (this.value.trim().length >= 2) {
            performGlobalSearch(this.value.trim());
        }
    });
}

// Perform global search across all items
function performGlobalSearch(searchTerm) {
    const searchResults = document.getElementById('searchResults');
    const results = [];

    // Search through all aisles, shelves, levels, and slots
    warehouseDetailData.aisles.forEach(aisle => {
        aisle.shelves.forEach(shelf => {
            shelf.levels.forEach(level => {
                level.slots.forEach(slot => {
                    if (slot.items && slot.items.length > 0) {
                        slot.items.forEach(item => {
                            if (item.name.toLowerCase().includes(searchTerm.toLowerCase())) {
                                results.push({
                                    item: item,
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
                    }
                });
            });
        });
    });

    displaySearchResults(results, searchTerm);
}

// Display search results
function displaySearchResults(results, searchTerm) {
    const searchResults = document.getElementById('searchResults');
    let html = '';

    if (results.length === 0) {
        html = `
                <div class="list-group-item search-result-item text-center py-3">
                    <i class="bi bi-search text-muted fs-1"></i>
                    <div class="mt-2 text-muted">Không tìm thấy vật tư "${searchTerm}"</div>
                    <small class="text-muted">Hãy thử từ khóa khác</small>
                </div>`;
    } else {
        // Group results by aisle for better organization
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

        // Header
        html += `
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
                const remainingCapacity = getRemainingCapacity(result.slot.usedCBM, result.slot.cbm);
                let capacityColor = 'success';
                if (remainingCapacity === 0) capacityColor = 'danger';
                else if (remainingCapacity < 30) capacityColor = 'warning';
                else if (remainingCapacity < 70) capacityColor = 'info';

                // mở row khi bắt đầu 1 cặp mới
                if (index % 2 === 0) {
                    html += `<div class="row pt-2 pb-2">`;
                }

                html += `
            <div class="col-12 col-lg-6">
                <div class="list-group-item search-result-item py-3 px-3">
                    <div class="row align-items-center">
                        <div class="col-md-5">
                            <div class="fw-bold text-primary mb-1">
                                <i class="bi bi-box-seam"></i> ${highlightSearchTerm(result.item.name, searchTerm)}
                            </div>
                            <div class="item-info text-muted">
                                <i class="bi bi-123"></i> ${result.item.quantity} • 
                                <i class="bi bi-rulers"></i> ${result.item.cbm} CBM
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
                                    onclick="goToSlotDetail('${result.location.aisleId}', '${result.location.slotId}')">
                                <i class="bi bi-eye"></i> Xem Chi Tiết
                            </button>
                        </div>
                    </div>
                </div>
            </div>`;

                // đóng row sau mỗi cặp 2 item hoặc khi tới item cuối
                if (index % 2 === 1 || index === group.items.length - 1) {
                    html += `</div>`;
                }
            });
        });
    }

    searchResults.innerHTML = html;
    searchResults.style.display = 'block';
}

// Highlight search term in results
function highlightSearchTerm(text, searchTerm) {
    const regex = new RegExp(`(${searchTerm})`, 'gi');
    return text.replace(regex, '<mark class="bg-warning">$1</mark>');
}

// Go directly to slot detail from search results
function goToSlotDetail(aisleId, slotId) {
    // Hide search results
    document.getElementById('searchResults').style.display = 'none';

    // Show aisle detail first
    showAisleDetail(aisleId);

    // Wait for the detail view to load, then show slot modal
    setTimeout(() => {
        // Highlight the slot
        const slotElement = document.querySelector(`[data-id="${slotId}"]`);
        if (slotElement) {
            slotElement.classList.add('highlighted');
            slotElement.scrollIntoView({
                behavior: 'smooth',
                block: 'center'
            });

            // Auto-open slot detail modal after a brief delay
            setTimeout(() => {
                showSlotDetail(slotId);
            }, 500);
        }
    }, 100);
}

// Khởi tạo khi trang load
document.addEventListener('DOMContentLoaded', function () {
    //showAisleSelection();
    //setupSearch();
    //setupGlobalSearch();
});

// Event listener cho việc chuyển view
document.addEventListener('DOMContentLoaded', function () {
    // Setup search khi chuyển sang detail view
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.target.id === 'detailView' &&
                mutation.target.style.display !== 'none' &&
                currentView === 'detail') {
                setTimeout(() => {
                    setupSearch();
                }, 100);
            }
        });
    });

    observer.observe(document.getElementById('detailView'), {
        attributes: true,
        attributeFilter: ['style']
    });
});

// Thêm animation khi hover slot
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('mouseover', function (e) {
        if (e.target.classList.contains('detail-slot')) {
            // Hiệu ứng hover
            e.target.style.transform = 'scale(1.05) translateY(-3px)';
        }
    });

    document.addEventListener('mouseout', function (e) {
        if (e.target.classList.contains('detail-slot')) {
            e.target.style.transform = '';
        }
    });
});

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    // ESC để quay lại hoặc đóng search results
    if (e.key === 'Escape') {
        if (document.getElementById('searchResults').style.display === 'block') {
            document.getElementById('searchResults').style.display = 'none';
        } else if (currentView === 'detail') {
            showAisleSelection();
        }
    }

    // Ctrl+F để focus vào search
    if (e.ctrlKey && e.key === 'f') {
        e.preventDefault();
        if (currentView === 'detail') {
            document.getElementById('detailSearchInput').focus();
        } else {
            document.getElementById('globalSearchInput').focus();
        }
    }

    // Ctrl+K để focus vào global search
    if (e.ctrlKey && e.key === 'k') {
        e.preventDefault();
        document.getElementById('globalSearchInput').focus();
        document.getElementById('globalSearchInput').select();
    }
});

// Touch support cho mobile
if ('ontouchstart' in window) {
    document.addEventListener('touchstart', function (e) {
        if (e.target.classList.contains('detail-slot')) {
            e.target.style.transform = 'scale(1.05) translateY(-3px)';
        }
    });

    document.addEventListener('touchend', function (e) {
        if (e.target.classList.contains('detail-slot')) {
            setTimeout(() => {
                e.target.style.transform = '';
            }, 200);
        }
    });
}