/**
 * @file dashboard-kho-modals.js
 * @description Quản lý toàn bộ logic đóng/mở và vẽ nội dung cho các cửa sổ Popup (Modals).
 * @version 2.7.26
 */

/**
 * Hàm chính để mở hộp thoại (Modal) hiển thị chi tiết (Drill-down) từ một thẻ KPI hoặc biểu đồ.
 */
function openDetail(detail, index) {
    // Push parent state lên stack nếu modal đang mở (= drilling)
    var modal = byId(ids.detailModal);
    if (modal && modal.classList.contains("open") && _detailStack.length === 0) {
        // First-time push: lưu lại detail HIỆN TẠI (parent)
        // (chỉ push 1 lần, không nested deeper)
    }
    if (modal && modal.classList.contains("open") && _currentDetail) {
        _detailStack.push({ detail: _currentDetail, index: _currentDetailIndex });
    }
    _currentDetail = detail;
    _currentDetailIndex = index;

    var content = byId(ids.detailModalContent);
    var modal = byId(ids.detailModal);
    if (modal && content) {
        document.documentElement.style.overflow = "hidden";
        document.body.style.overflow = "hidden";
        content.innerHTML =
            '<div style="padding:40px;text-align:center"><div class="dk-spinner" style="margin:0 auto 10px auto;border-top-color:#3b82f6;"></div><div style="color:#6b7280;font-size:13px">Đang tải dữ liệu...</div></div>';
        modal.classList.add("open");
        // Defer rendering so the browser paints the modal open animation and spinner first
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                renderDetailModal(detail, index);
            });
        });
    } else {
        renderDetailModal(detail, index);
    }
}

var _currentDetail = null;

var _currentDetailIndex = -1;

/**
 * Nạp cấu hình cột, gọi API tải dữ liệu và vẽ giao diện bảng bên trong Modal chi tiết.
 */
function renderDetailModal(detail, index) {
    var model;
    try {
        model = getDetailData(detail, index);
    } catch (e) {
        model = { title: detail, rows: [], columns: [], isWarehouseMap: false };
    }
    // Update title with back button if stack non-empty
    var titleEl = byId(ids.detailModalTitle);
    if (_detailStack.length > 0) {
        titleEl.innerHTML =
            '<button type="button" class="dk-back-btn js-modal-back" title="Quay lại">← Quay lại</button> ' +
            escapeHtml(model.title);
    } else {
        titleEl.textContent = model.title;
    }
    // v2.3.20 — Meta header chỉ hiện model.meta (nếu có), tổng dòng dời xuống footer
    byId(ids.detailModalMeta).textContent = model.meta || "";
    if (!model.meta) byId(ids.detailModalMeta).style.display = "none";
    else byId(ids.detailModalMeta).style.display = "";
    // Update footer row count
    var rowCountEl = byId("detailModalRowCount");
    if (rowCountEl) {
        rowCountEl.textContent = "Tổng số dòng: " + formatNumber((model.rows || []).length, 0);
    }

    // Reset search
    var searchInput = byId("detailSearchInput");
    if (searchInput) {
        searchInput.value = "";
    }
    var searchCount = byId("detailSearchCount");
    if (searchCount) {
        searchCount.textContent = "";
    }

    var rowCount = (model.rows || []).length;
    // v2.3.17 — detail có ít dòng → ẩn search bar
    var noSearchDetails = { capacitySummary: 1, materialCount: 1 };
    var searchBar = document.querySelector(".dk-modal-search-bar");
    if (searchBar) {
        searchBar.style.display = rowCount <= 5 || noSearchDetails[detail] ? "none" : "";
    }

    if (model.isWarehouseMap) {
        var overall = state.overall.length > 0 ? state.overall[0] : {};
        var wrapHtml = '<div class="dk-modal-wh-wrap">';

        wrapHtml += '<div class="dk-modal-wh-summary">';
        wrapHtml +=
            '<div class="dk-modal-wh-stat"><div class="dk-modal-wh-stat-label">Tổng sức chứa (CBM)</div><div class="dk-modal-wh-stat-value">' +
            formatNumber(toNumber(overall.TotalCapacity), 2) +
            "</div></div>";
        wrapHtml +=
            '<div class="dk-modal-wh-stat"><div class="dk-modal-wh-stat-label">Đã sử dụng</div><div class="dk-modal-wh-stat-value dk-text-danger">' +
            formatNumber(toNumber(overall.UsedNPL) + toNumber(overall.UsedPL), 2) +
            " CBM — " +
            formatNumber(toNumber(overall.TotalPercent), 1) +
            "%</div></div>";
        wrapHtml +=
            '<div class="dk-modal-wh-stat"><div class="dk-modal-wh-stat-label">Kho NL</div><div class="dk-modal-wh-stat-value dk-text-primary">' +
            formatNumber(toNumber(overall.UsedNPL), 2) +
            " / " +
            formatNumber(toNumber(overall.CapacityNPL), 2) +
            ' CBM</div><div class="dk-modal-wh-stat-sub">' +
            formatNumber(toNumber(overall.PercentNPL), 1) +
            "% lấp đầy</div></div>";
        wrapHtml +=
            '<div class="dk-modal-wh-stat"><div class="dk-modal-wh-stat-label">Kho PL</div><div class="dk-modal-wh-stat-value dk-text-warn">' +
            formatNumber(toNumber(overall.UsedPL), 2) +
            " / " +
            formatNumber(toNumber(overall.CapacityPL), 2) +
            ' CBM</div><div class="dk-modal-wh-stat-sub">' +
            formatNumber(toNumber(overall.PercentPL), 1) +
            "% lấp đầy</div></div>";
        wrapHtml += "</div>";

        // Sơ đồ tile map
        wrapHtml += '<div style="margin-bottom:14px">';
        wrapHtml += '<b style="font-size:13px">Sơ đồ lấp đầy kệ</b>';
        wrapHtml += '<div style="display:flex;gap:10px;font-size:11px;font-weight:700;margin:6px 0;">';
        wrapHtml +=
            '<span><i style="display:inline-block;width:12px;height:12px;background:#dcfce7;border:1px solid #86efac;border-radius:2px;vertical-align:middle"></i> &lt;50%</span>';
        wrapHtml +=
            '<span><i style="display:inline-block;width:12px;height:12px;background:#fef9c3;border:1px solid #fde047;border-radius:2px;vertical-align:middle"></i> 50-85%</span>';
        wrapHtml +=
            '<span><i style="display:inline-block;width:12px;height:12px;background:#fecdd3;border:1px solid #fda4af;border-radius:2px;vertical-align:middle"></i> 85-100%</span>';
        wrapHtml +=
            '<span><i style="display:inline-block;width:12px;height:12px;background:#e11d48;border:1px solid #be123c;border-radius:2px;vertical-align:middle"></i> &gt;100%</span>';
        wrapHtml += "</div>";

        // Build inline heatmap
        var racksSorted = state.racks.slice().sort(function (a, b) {
            var mA = toNumber(a.Module),
                mB = toNumber(b.Module);
            if (mA !== mB) return mA - mB;
            return (a.TenDay || "").localeCompare(b.TenDay || "");
        });
        var modGroups = {};
        for (var rr = 0; rr < racksSorted.length; rr++) {
            var rm = toNumber(racksSorted[rr].Module);
            var rmName = rm === 1 ? "NL" : rm === 2 ? "PL" : "Khác";
            if (!modGroups[rmName]) modGroups[rmName] = [];
            modGroups[rmName].push(racksSorted[rr]);
        }
        var groupKeys = ["NL", "PL", "Khác"];
        for (var gk = 0; gk < groupKeys.length; gk++) {
            var gName = groupKeys[gk];
            var gRacks = modGroups[gName];
            if (!gRacks || !gRacks.length) continue;
            wrapHtml +=
                '<div style="margin-bottom:10px"><b style="font-size:12px;color:' +
                (gName === "NL" ? "#2563eb" : "#d97706") +
                '">' +
                gName +
                '</b><div style="display:flex;flex-wrap:wrap;gap:5px;margin-top:4px">';
            for (var gr = 0; gr < gRacks.length; gr++) {
                var grItem = gRacks[gr];
                var grUsed = toNumber(grItem.TongCBMSuDungTrongKe);
                var grCap = toNumber(grItem.TongCBMTrongKe);
                var grPct = grCap > 0 ? (grUsed / grCap) * 100 : 0;
                var grClass =
                    grPct > 100 ? "dk-wh-over" : grPct >= 85 ? "dk-wh-full" : grPct >= 50 ? "dk-wh-warn" : "dk-wh-safe";
                // v2.4.16 — Tile fill animation
                var grFillH = Math.min(100, Math.max(0, grPct));
                wrapHtml +=
                    '<div class="dk-wh-tile ' +
                    grClass +
                    '" data-pct="' +
                    grFillH.toFixed(1) +
                    '" title="' +
                    escapeHtml(
                        (grItem.TenKe || "") + " | " + (grItem.TenDay || "") + " | " + formatNumber(grPct, 1) + "%",
                    ) +
                    '" style="cursor:default;animation-delay:' +
                    gr * 40 +
                    'ms">';
                wrapHtml += '<span class="dk-wh-tile-fill" style="height:' + grFillH.toFixed(1) + '%"></span>';
                if (grPct >= 100) wrapHtml += '<i class="fa-solid fa-triangle-exclamation dk-wh-tile-warn-icon"></i>';
                wrapHtml += '<span class="dk-wh-tile-name">' + escapeHtml(grItem.TenKe || "Kệ") + "</span>";
                wrapHtml += '<span class="dk-wh-tile-pct">' + formatNumber(grPct, 0) + "%</span>";
                wrapHtml += "</div>";
            }
            wrapHtml += "</div></div>";
        }
        wrapHtml += "</div>";

        // Table bên dưới (v2.7.1 — bỏ label "NL trước → PL")
        wrapHtml += renderDetailTable(model.columns, model.rows);
        wrapHtml += "</div>";

        byId(ids.detailModalContent).innerHTML = wrapHtml;
    } else {
        byId(ids.detailModalContent).innerHTML = renderDetailTable(model.columns, model.rows);
    }

    byId(ids.detailModal).classList.add("open");

    // v2.3.9 — Sau khi mở modal totalCapacity → fetch chi tiết theo Ô + append
    if (model.isWarehouseMap) {
        loadRackSlotDetailIntoModal();
    }

    // v2.3.30 — Async load cho drill "Mã vật tư" (toàn bộ ~2348 mã)
    if (model.customAsync === "allMaterials") {
        var contentEl = byId(ids.detailModalContent);
        if (contentEl)
            contentEl.innerHTML =
                '<div class="dk-empty" style="padding:30px">Đang tải tất cả mã vật tư trong kho (có thể mất 5-15 giây)...</div>';
        requestJson("/api/DashboardKhoDesktop/GetAllMaterialsInStock")
            .then(function (data) {
                var rows = normalizeArray(data).map(function (r, i) {
                    // v2.4.0 — Compose Màu = "Mã màu — Tên màu" + swatch; Khổ vải kèm đơn vị; alias MaVT → ItemCode
                    var maMau = r.MaMauVT ? String(r.MaMauVT).trim() : "";
                    var tenMau = r.Mau ? String(r.Mau).trim() : "";
                    var mauText = "";
                    if (maMau && tenMau) mauText = escapeHtml(maMau) + " — " + escapeHtml(tenMau);
                    else if (maMau) mauText = escapeHtml(maMau);
                    else if (tenMau) mauText = escapeHtml(tenMau);
                    var mauDisplay = mauText
                        ? '<span class="dk-mau-swatch" data-mau="' +
                        escapeHtml(maMau || tenMau) +
                        '"></span>' +
                        '<span class="dk-mau-text">' +
                        mauText +
                        "</span>"
                        : "";
                    var khoVai = r.KhoVai ? String(r.KhoVai).trim() : "";
                    var dvvt = r.TenDVVT ? String(r.TenDVVT).trim() : "";
                    var khoVaiDisplay = khoVai ? khoVai + (dvvt ? " " + dvvt : "") : "";

                    return Object.assign({}, r, {
                        STT: i + 1,
                        ItemCode: r.MaVT || "",
                        MauDisplay: mauDisplay,
                        KhoVai: khoVaiDisplay,
                    });
                });
                if (rows.length === 0) {
                    contentEl.innerHTML = '<div class="dk-empty" style="padding:30px">Không có mã vật tư nào</div>';
                } else {
                    contentEl.innerHTML = renderDetailTable(model.columns, rows);
                }
                // Update footer row count
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            })
            .catch(function (err) {
                // v2.3.35 — Hiện rõ lỗi để debug, không chỉ "Lỗi tải" chung chung
                console.error("[Dashboard Kho] AllMaterials error:", err);
                var msg = err && err.message ? err.message : "Lỗi không rõ";
                contentEl.innerHTML =
                    '<div style="padding:30px">' +
                    '<div class="dk-text-danger" style="margin-bottom:10px;font-size:14px">⚠ Lỗi tải danh sách mã vật tư</div>' +
                    '<div class="dk-error-box">' +
                    escapeHtml(msg) +
                    "</div>" +
                    '<div class="dk-text-muted" style="margin-top:14px;font-size:12.5px;line-height:1.6">' +
                    "<b>Có thể do:</b><br>" +
                    "• Backend chưa được Rebuild (SP <code>GetAllMaterialsInStock</code> chưa tồn tại) → Stop debug → Rebuild Solution → F5<br>" +
                    "• Query SQL bị lỗi → mở F12 Network → tab Response của request <code>GetAllMaterialsInStock</code> để xem chi tiết<br>" +
                    "• Connection timeout → query quá nặng, thử lại sau" +
                    "</div>" +
                    "</div>";
            });
    }

    // v2.4.4 — Dispatcher cho 6 modal "Xem chi tiết" mới
    if (model.customAsync === "todoDetail") {
        renderTodoDetailModal();
    }
    if (model.customAsync === "top5VTAll") {
        renderTop5VTAllModal();
    }
    if (model.customAsync === "top5KHAll") {
        renderTop5KHAllModal();
    }
    if (model.customAsync === "hetHanAll") {
        renderHetHanAllModal();
    }
    if (model.customAsync === "giaTriNhomAll") {
        renderGiaTriNhomAllModal();
    }
    if (model.customAsync === "kiemKeAll") {
        renderKiemKeAllModal();
    }
    // v2.4.15 — Dispatcher cho 5 modal KPI header
    if (model.customAsync === "tonDauKyDetail") {
        renderTonDauKyDetailModal();
    }
    if (model.customAsync === "tongNhapDetail") {
        renderTongNhapDetailModal();
    }
    if (model.customAsync === "tongXuatDetail") {
        renderTongXuatDetailModal();
    }
    if (model.customAsync === "tonKhoDetail") {
        renderTonKhoDetailModal();
    }
    if (model.customAsync === "poTreDetail") {
        renderPOTreDetailModal();
    }
    if (model.customAsync === "alertDetail") {
        renderAlertDetailModal();
    }

    if (model.customDrillCustomer) {
        var custTenKH = String(model.customDrillCustomer.TenKH || "").trim();
        var custMaKH = String(model.customDrillCustomer.MaKH || "").trim();
        loadSlotDrillIntoModal({
            filterFn: function (r) {
                var ds = String(r.DanhSachKH || "");
                if (custTenKH && ds.toLowerCase().indexOf(custTenKH.toLowerCase()) >= 0) return true;
                if (custMaKH && ds.toLowerCase().indexOf(custMaKH.toLowerCase()) >= 0) return true;
                return false;
            },
            emptyText: 'Khách hàng "' + (custTenKH || custMaKH) + '" chưa có vật tư nào trong kho',
            sectionTitle: "Vị trí hàng của khách trong kho — theo dãy / kệ / ô",
        });
    }

    // v2.3.26 — Rack drill: fetch GetRackSlotDetail và filter theo TenKe
    if (model.customDrillRack) {
        loadSlotDrillIntoModal({
            filterFn: function (r) {
                return String(r.TenKe || "").trim() === String(model.customDrillRack.TenKe || "").trim();
            },
            emptyText: "Kệ này chưa có vật tư nào",
            sectionTitle: "Danh sách ô và vật tư trong kệ này",
        });
    }
}

/**
 * Đóng Modal chi tiết hiện tại và quay về màn hình trước đó.
 */
function closeDetailModal() {
    if (window.__currentModalAbortController) {
        window.__currentModalAbortController.abort();
        window.__currentModalAbortController = null;
    }

    var modal = byId(ids.detailModal);
    if (!modal) return;
    modal.classList.remove("open");
    modal.style.display = ""; // clear any inline style from older code paths
    document.documentElement.style.overflow = "";
    document.body.style.overflow = "";

    // v2.3.27 — Reset drill stack khi đóng modal hoàn toàn
    _detailStack = [];
    _currentDetail = null;
    _currentDetailIndex = -1;

    // Restore search bar visibility (calendar-day modal hides it)
    var searchBar = document.querySelector(".dk-modal-search-bar");
    if (searchBar) searchBar.style.display = "";
}

// ─── Feature 10: Full-screen Panel Mode ─────────────────────────────────────
var fsBackdrop = null;

/**
 * Lọc nhanh dữ liệu trực tiếp trên bảng chi tiết bằng từ khóa gõ vào ô tìm kiếm.
 */
function filterDetailTable(query) {
    var content = byId(ids.detailModalContent);
    if (!content) return;

    var visiblePanels = content.querySelectorAll(".dk-day-panel");
    if (visiblePanels.length > 0 && window.__dkDetailData) {
        for (var vp = 0; vp < visiblePanels.length; vp++) {
            if (visiblePanels[vp].style.display === "none") continue;
            if (!visiblePanels[vp].querySelector(".dk-grid-table")) break;

            var tabKey = visiblePanels[vp].getAttribute("data-tabkey");
            var d = window.__dkDetailData[tabKey];
            if (!d) return;
            if (!d.allRows) d.allRows = d.rows || [];

            var lowerQuery = String(query || "").toLowerCase();
            if (!lowerQuery) {
                d.rows = d.allRows;
            } else {
                d.rows = d.allRows.filter(function (row) {
                    for (var key in row) {
                        if (!Object.prototype.hasOwnProperty.call(row, key)) continue;
                        if (key.indexOf("__dk") === 0) continue;
                        var value = row[key];
                        if (value === null || value === undefined) continue;
                        if (String(value).toLowerCase().indexOf(lowerQuery) >= 0) return true;
                    }
                    return false;
                });
            }

            d.page = 1;
            d.sortedRows = null;
            d.pageCache = {};
            d.collapsedGroupsByPage = {};
            initVirtualScrollGrid(tabKey);

            var countEl = byId("detailSearchCount");
            if (countEl) {
                countEl.textContent = lowerQuery ? formatNumber(d.rows.length, 0) + " / " + formatNumber(d.allRows.length, 0) + " dòng" : "";
            }
            return;
        }
    }

    // v2.3.9 — Nếu có tabbed UI (modal ngày calendar), chỉ filter tab đang hiện;
    // ngược lại filter tất cả tbody trong modal.
    var tbodies;
    if (visiblePanels.length > 0) {
        // Chỉ lấy tbody của panel đang visible
        tbodies = [];
        for (var p = 0; p < visiblePanels.length; p++) {
            if (visiblePanels[p].style.display !== "none") {
                var tb = visiblePanels[p].querySelectorAll("tbody");
                for (var tt = 0; tt < tb.length; tt++) tbodies.push(tb[tt]);
            }
        }
    } else {
        tbodies = content.querySelectorAll("tbody");
    }

    var lowerQuery = query.toLowerCase();
    var visibleTotal = 0,
        totalRows = 0;
    for (var b = 0; b < tbodies.length; b++) {
        var rows = tbodies[b].querySelectorAll("tr");
        for (var i = 0; i < rows.length; i++) {
            totalRows++;
            var match = !lowerQuery || rows[i].textContent.toLowerCase().indexOf(lowerQuery) >= 0;
            rows[i].style.display = match ? "" : "none";
            if (match) visibleTotal++;
        }
    }
    var countEl = byId("detailSearchCount");
    if (countEl) {
        countEl.textContent = query ? visibleTotal + " / " + totalRows + " dòng" : "";
    }
}

var loadedPages = { 1: false, 2: false, 3: false };

var flowRangeFrom = null;

var flowRangeTo = null;

window.dkShowToast = showToast;

/**
 * Hiển thị trạng thái đang tải (Loading Spinner) bên trong Modal.
 */
function modalLoading(text) {
    return (
        '<div class="dk-empty" style="padding:40px;text-align:center"><i class="fa-solid fa-spinner fa-spin" style="font-size:24px;margin-bottom:10px;display:block"></i>' +
        escapeHtml(text || "Đang tải...") +
        "</div>"
    );
}

/**
 * Hiển thị thông báo lỗi khi không thể nạp được dữ liệu vào Modal.
 */
function modalErrorBox(msg) {
    return (
        '<div style="padding:30px">' +
        '<div class="dk-text-danger" style="margin-bottom:10px;font-size:14px">⚠ Lỗi tải dữ liệu</div>' +
        '<div class="dk-error-box">' +
        escapeHtml(msg || "Không rõ") +
        "</div>" +
        "</div>"
    );
}

/**
 * Mở nhanh một Modal chi tiết cụ thể mà không cần thông qua phân tích thẻ (Card) gốc.
 */
function renderDetailModalDirect(model) {
    var titleEl = byId(ids.detailModalTitle);
    if (_detailStack.length > 0) {
        titleEl.innerHTML =
            '<button type="button" class="dk-back-btn js-modal-back" title="Quay lại">← Quay lại</button> ' +
            escapeHtml(model.title);
    } else {
        titleEl.textContent = model.title;
    }
    byId(ids.detailModalMeta).textContent = model.meta || "";
    byId(ids.detailModalMeta).style.display = model.meta ? "" : "none";
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
    if (model.customDrillCustomer) {
        loadCustomerMaterialDetail(model.customDrillCustomer);
    }
}

//#endregion


// ─── #0 Tồn đầu kỳ ────────────────────────────────────────────────
/**
 * Hiển thị Modal chi tiết cho Chỉ số: Tồn đầu kỳ.
 * Cung cấp các tab filter (Tất cả, NL, PL) và bảng danh sách vật tư.
 */
function renderTonDauKyDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var titleEl = byId(ids.detailModalTitle);
    if (titleEl) titleEl.textContent = "Tồn đầu kỳ — tại " + (state.dateFilter ? state.dateFilter.from : "");
    var tabs = [
        { key: "all", label: "Tất cả", icon: "fa-list" },
        { key: "nl", label: "Nguyên liệu", icon: "fa-leaf" },
        { key: "pl", label: "Phụ liệu", icon: "fa-boxes-stacked" },
    ];
    var activeKey = "all";
    var allRows = [];
    function paint() {
        var sumSL = 0,
            sumGT = 0,
            soVT = new Set() ? new Set() : {};
        allRows.forEach(function (r) {
            sumSL += toNumber(r.SLTonDau);
            sumGT += toNumber(r.ThanhTien);
            if (typeof soVT.add === "function") soVT.add(r.ItemCode);
            else soVT[r.ItemCode] = 1;
        });
        var numVT = typeof soVT.size === "number" ? soVT.size : Object.keys(soVT).length;
        content.innerHTML =
            renderKpiSummaryStrip([
                { label: "Tổng SL tồn đầu", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                { label: "Số mã VT", value: formatNumber(numVT, 0), sub: "ItemCode", cls: "warn" },
                { label: "Tổng giá trị", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" },
            ]) +
            renderKpiSubtabs(tabs, activeKey) +
            renderKpiFilterBar("Tìm ItemCode, tên VT...") +
            '<div id="kpiTbody">' +
            modalLoading() +
            "</div>";
        bindKpiSubtabs();
        bindKpiSearch(content, doFilter);
        doFilter();
    }
    function doFilter() {
        var q = (byId("kpiSearch").value || "").trim().toLowerCase();
        var filtered = allRows.filter(function (r) {
            if (!q) return true;
            var hay = (
                (r.MaNPL || "") +
                " " +
                (r.ItemCode || "") +
                " " +
                (r.TenVT || "") +
                " " +
                (r.Mau || "") +
                " " +
                (r.KhoVai || "")
            ).toLowerCase();
            return hay.indexOf(q) >= 0;
        });
        renderTable(filtered);
        var cnt = byId("kpiCount");
        if (cnt) cnt.textContent = filtered.length + " dòng";
        var rcEl = byId("detailModalRowCount");
        if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
    }
    function renderTable(rows) {
        var cols = [
            { key: "STT", label: "STT", number: 0, center: true, width: 50 },
            // v2.7.1 — Ẩn cột Mã NPL theo yêu cầu (chuỗi dài khó đọc)
            { key: "ItemCode", label: "ItemCode", width: 100 },
            { key: "TenVT", label: "Tên VT", width: 220 },
            { key: "Mau", label: "Màu", center: true, width: 80 },
            { key: "KhoVai", label: "Khổ", center: true, width: 80 },
            { key: "LoaiKho", label: "Loại", center: true, width: 60 },
            { key: "DonVi", label: "ĐV", center: true, width: 50 },
            { key: "SLTonDau", label: "SL tồn đầu", number: 1, width: 110 },
            { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
            { key: "ThanhTien", label: "Thành tiền", number: 0, width: 130 },
        ];
        byId("kpiTbody").innerHTML = renderDetailTable(cols, rows);
    }
    function bindKpiSubtabs() {
        content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
            b.addEventListener("click", function () {
                content.querySelectorAll(".dk-kk-subtab").forEach(function (x) {
                    x.classList.remove("active");
                });
                this.classList.add("active");
                activeKey = this.getAttribute("data-tab-key");
                fetchData();
            });
        });
    }
    function fetchData() {
        content.innerHTML = modalLoading();
        var from = state.dateFilter ? state.dateFilter.from : "";
        requestJson(
            "/api/DashboardKhoDesktop/GetTonDauKyChiTiet?tuNgay=" +
            encodeURIComponent(from) +
            "&loai=" +
            encodeURIComponent(activeKey),
        )
            .then(function (d) {
                allRows = normalizeArray(d);
                paint();
            })
            .catch(function (err) {
                content.innerHTML = modalErrorBox(err && err.message);
            });
    }
    fetchData();
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

// ─── #1 Tổng nhập ────────────────────────────────────────────────
/**
 * Hiển thị Modal chi tiết cho Chỉ số: Tổng nhập kho.
 * Hỗ trợ group dữ liệu theo Ngày, Theo PO, Nhà cung cấp hoặc Vật tư.
 */
function renderTongNhapDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var titleEl = byId(ids.detailModalTitle);
    if (titleEl)
        titleEl.textContent =
            "Chi tiết nhập kho — " + (state.dateFilter ? state.dateFilter.from + " → " + state.dateFilter.to : "");
    var tabs = [
        { key: "all", label: "Chi tiết", icon: "fa-list" },
        { key: "date", label: "Theo ngày", icon: "fa-calendar-days" },
        { key: "po", label: "Theo PO mua", icon: "fa-file-invoice" },
        { key: "ncc", label: "Theo nhà cung cấp", icon: "fa-building" },
        { key: "vt", label: "Theo vật tư", icon: "fa-cube" },
    ];
    var activeKey = "all";
    var allRows = [];
    function paint() {
        var sumSL = 0,
            sumGT = 0;
        allRows.forEach(function (r) {
            sumSL += toNumber(r.SLNhap || r.SL);
            sumGT += toNumber(r.GiaTri);
        });
        content.innerHTML =
            renderKpiSummaryStrip([
                { label: "Tổng SL nhập", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                { label: "Số dòng", value: formatNumber(allRows.length, 0), sub: "trong " + activeKey, cls: "warn" },
                { label: "Tổng giá trị", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" },
            ]) +
            renderKpiSubtabs(tabs, activeKey) +
            renderKpiFilterBar("Tìm nhanh trong bảng...") +
            '<div id="kpiTbody"></div>';
        bindKpiSubtabs();
        bindKpiSearch(content, doFilter);
        doFilter();
    }
    function doFilter() {
        var q = (byId("kpiSearch").value || "").trim().toLowerCase();
        var filtered = q
            ? allRows.filter(function (r) {
                return JSON.stringify(r).toLowerCase().indexOf(q) >= 0;
            })
            : allRows;
        renderTable(filtered);
        var cnt = byId("kpiCount");
        if (cnt) cnt.textContent = filtered.length + " dòng";
        var rcEl = byId("detailModalRowCount");
        if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
    }
    function renderTable(rows) {
        var cols;
        if (activeKey === "all") {
            cols = colsNhap();
        } else if (activeKey === "date") {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "Ngay", label: "Ngày", date: true, center: true, width: 100 },
                { key: "SoPhieu", label: "Số phiếu", number: 0, center: true, width: 90 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 80 },
                { key: "SLNhap", label: "SL nhập", number: 1, width: 120 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
            ];
        } else if (activeKey === "po") {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "POMua", label: "PO mua", center: true, width: 130 },
                { key: "NCC", label: "NCC", width: 180 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                { key: "SL", label: "SL", number: 1, width: 110 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
                { key: "NgayDuKien", label: "Dự kiến", date: true, center: true, width: 100 },
            ];
        } else if (activeKey === "ncc") {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "NCC", label: "NCC", width: 200 },
                { key: "SoPO", label: "Số PO", number: 0, center: true, width: 80 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 80 },
                { key: "SL", label: "SL", number: 1, width: 120 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 140 },
            ];
        } else {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "ItemCode", label: "ItemCode", width: 110 },
                { key: "TenVT", label: "Tên VT", width: 240 },
                { key: "LoaiKho", label: "Loại", center: true, width: 60 },
                { key: "DonVi", label: "ĐV", center: true, width: 50 },
                { key: "SLNhap", label: "SL nhập", number: 1, width: 110 },
                { key: "SoPO", label: "Số PO", number: 0, center: true, width: 70 },
                { key: "LanNhapCuoi", label: "Lần cuối", date: true, center: true, width: 100 },
                { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
            ];
        }
        byId("kpiTbody").innerHTML = renderDetailTable(cols, rows);
    }
    function bindKpiSubtabs() {
        content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
            b.addEventListener("click", function () {
                content.querySelectorAll(".dk-kk-subtab").forEach(function (x) {
                    x.classList.remove("active");
                });
                this.classList.add("active");
                activeKey = this.getAttribute("data-tab-key");
                fetchData();
            });
        });
    }
    function fetchData() {
        content.innerHTML = modalLoading();
        var qs =
            "?tuNgay=" +
            encodeURIComponent(state.dateFilter.from) +
            "&denNgay=" +
            encodeURIComponent(state.dateFilter.to) +
            "&groupBy=" +
            encodeURIComponent(activeKey);
        requestJson("/api/DashboardKhoDesktop/GetTongNhapChiTiet" + qs)
            .then(function (d) {
                allRows = normalizeArray(d);
                paint();
            })
            .catch(function (err) {
                content.innerHTML = modalErrorBox(err && err.message);
            });
    }
    fetchData();
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

// ─── #2 Tổng xuất (similar to Tổng nhập) ──────────────────────────
/**
 * Hiển thị Modal chi tiết cho Chỉ số: Tổng xuất kho.
 * Hỗ trợ group dữ liệu theo Ngày, Đơn hàng, Khách hàng hoặc Vật tư.
 */
function renderTongXuatDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var titleEl = byId(ids.detailModalTitle);
    if (titleEl)
        titleEl.textContent =
            "Chi tiết xuất kho — " + (state.dateFilter ? state.dateFilter.from + " → " + state.dateFilter.to : "");
    var tabs = [
        { key: "all", label: "Chi tiết", icon: "fa-list" },
        { key: "date", label: "Theo ngày", icon: "fa-calendar-days" },
        { key: "dh", label: "Theo đơn hàng", icon: "fa-file-lines" },
        { key: "kh", label: "Theo khách hàng", icon: "fa-users" },
        { key: "vt", label: "Theo vật tư", icon: "fa-cube" },
    ];
    var activeKey = "all";
    var allRows = [];
    function paint() {
        var sumSL = 0,
            sumGT = 0;
        allRows.forEach(function (r) {
            sumSL += toNumber(r.SLXuat || r.SL);
            sumGT += toNumber(r.GiaTri);
        });
        content.innerHTML =
            renderKpiSummaryStrip([
                { label: "Tổng SL xuất", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                { label: "Số dòng", value: formatNumber(allRows.length, 0), sub: "trong " + activeKey, cls: "warn" },
                { label: "Tổng giá trị", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" },
            ]) +
            renderKpiSubtabs(tabs, activeKey) +
            renderKpiFilterBar("Tìm nhanh trong bảng...") +
            '<div id="kpiTbody"></div>';
        bindKpiSubtabs();
        bindKpiSearch(content, doFilter);
        doFilter();
    }
    function doFilter() {
        var q = (byId("kpiSearch").value || "").trim().toLowerCase();
        var filtered = q
            ? allRows.filter(function (r) {
                return JSON.stringify(r).toLowerCase().indexOf(q) >= 0;
            })
            : allRows;
        renderTable(filtered);
        var cnt = byId("kpiCount");
        if (cnt) cnt.textContent = filtered.length + " dòng";
        var rcEl = byId("detailModalRowCount");
        if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
    }
    function renderTable(rows) {
        var cols;
        if (activeKey === "all") {
            cols = colsXuat();
        } else if (activeKey === "date") {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "Ngay", label: "Ngày", date: true, center: true, width: 100 },
                { key: "SoPhieu", label: "Số phiếu", number: 0, center: true, width: 90 },
                { key: "SoDH", label: "Số ĐH", number: 0, center: true, width: 80 },
                { key: "SLXuat", label: "SL xuất", number: 1, width: 120 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
            ];
        } else if (activeKey === "dh") {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "MaDH", label: "Mã ĐH", center: true, width: 130 },
                { key: "KhachHang", label: "Khách hàng", width: 180 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                { key: "SL", label: "SL", number: 1, width: 110 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
                { key: "NgayXuat", label: "Ngày", date: true, center: true, width: 100 },
            ];
        } else if (activeKey === "kh") {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "KhachHang", label: "Khách hàng", width: 220 },
                { key: "MaKH", label: "Mã KH", center: true, width: 90 },
                { key: "SoDH", label: "Số ĐH", number: 0, center: true, width: 70 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
                { key: "SL", label: "SL", number: 1, width: 110 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
            ];
        } else {
            cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "ItemCode", label: "ItemCode", width: 110 },
                { key: "TenVT", label: "Tên VT", width: 240 },
                { key: "LoaiKho", label: "Loại", center: true, width: 60 },
                { key: "DonVi", label: "ĐV", center: true, width: 50 },
                { key: "SLXuat", label: "SL xuất", number: 1, width: 110 },
                { key: "SoDH", label: "Số ĐH", number: 0, center: true, width: 70 },
                { key: "LanXuatCuoi", label: "Lần cuối", date: true, center: true, width: 100 },
                { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
                { key: "GiaTri", label: "Giá trị", number: 0, width: 130 },
            ];
        }
        byId("kpiTbody").innerHTML = renderDetailTable(cols, rows);
    }
    function bindKpiSubtabs() {
        content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
            b.addEventListener("click", function () {
                content.querySelectorAll(".dk-kk-subtab").forEach(function (x) {
                    x.classList.remove("active");
                });
                this.classList.add("active");
                activeKey = this.getAttribute("data-tab-key");
                fetchData();
            });
        });
    }
    function fetchData() {
        content.innerHTML = modalLoading();
        var qs =
            "?tuNgay=" +
            encodeURIComponent(state.dateFilter.from) +
            "&denNgay=" +
            encodeURIComponent(state.dateFilter.to) +
            "&groupBy=" +
            encodeURIComponent(activeKey);
        requestJson("/api/DashboardKhoDesktop/GetTongXuatChiTiet" + qs)
            .then(function (d) {
                allRows = normalizeArray(d);
                paint();
            })
            .catch(function (err) {
                content.innerHTML = modalErrorBox(err && err.message);
            });
    }
    fetchData();
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

// ─── #3 Tồn kho ──────────────────────────────────────────────────
function renderTonKhoDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var titleEl = byId(ids.detailModalTitle);
    if (titleEl) titleEl.textContent = "Chi tiết tồn kho — tại " + (state.dateFilter ? state.dateFilter.to : "");
    var tabs = [
        { key: "all", label: "Tất cả", icon: "fa-list" },
        { key: "nl", label: "Nguyên liệu", icon: "fa-leaf" },
        { key: "pl", label: "Phụ liệu", icon: "fa-boxes-stacked" },
        { key: "expired", label: "Sắp hết hạn", icon: "fa-clock" },
    ];
    var activeKey = "all";
    var allRows = [];
    function paint() {
        var sumSL = 0,
            sumGT = 0,
            soVT = {},
            soKe = {};
        allRows.forEach(function (r) {
            sumSL += toNumber(r.SLTon);
            sumGT += toNumber(r.SLTon) * toNumber(r.DonGia);
            soVT[r.ItemCode] = 1;
            soKe[r.ViTriKe] = 1;
        });
        content.innerHTML =
            renderKpiSummaryStrip([
                { label: "Tổng SL tồn", value: formatNumber(sumSL, 0), sub: "đơn vị", cls: "good" },
                { label: "Số mã VT", value: formatNumber(Object.keys(soVT).length, 0), sub: "ItemCode", cls: "warn" },
                { label: "Số kệ", value: formatNumber(Object.keys(soKe).length, 0), sub: "vị trí", cls: "neutral" },
                { label: "Giá trị tồn", value: formatVNDShort(sumGT), sub: "VND", cls: "danger" },
            ]) +
            renderKpiSubtabs(tabs, activeKey) +
            renderKpiFilterBar("Tìm ItemCode, tên VT, vị trí kệ...") +
            '<div id="kpiTbody"></div>';
        bindKpiSubtabs();
        bindKpiSearch(content, doFilter);
        doFilter();
    }
    function doFilter() {
        var q = (byId("kpiSearch").value || "").trim().toLowerCase();
        var filtered = q
            ? allRows.filter(function (r) {
                var hay = (
                    (r.ItemCode || "") +
                    " " +
                    (r.TenVT || "") +
                    " " +
                    (r.ViTriKe || "") +
                    " " +
                    (r.POMua || "") +
                    " " +
                    (r.SoLo || "") +
                    " " +
                    (r.TenKH || "")
                ).toLowerCase();
                return hay.indexOf(q) >= 0;
            })
            : allRows;
        renderTable(filtered);
        var cnt = byId("kpiCount");
        if (cnt) cnt.textContent = filtered.length + " dòng";
        var rcEl = byId("detailModalRowCount");
        if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
    }
    function renderTable(rows) {
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        var rowsView = rows.map(function (r) {
            var hd = r.HanDung ? new Date(r.HanDung) : null;
            var dleft = hd ? Math.round((hd - today) / 86400000) : 999;
            var chipCls = dleft < 7 ? "danger" : dleft < 30 ? "warn" : "good";
            var chipText = dleft < 0 ? "Quá hạn " + Math.abs(dleft) + "d" : dleft + " ngày";
            return Object.assign({}, r, {
                _HanDung: '<span class="dk-status-chip dk-status-chip-' + chipCls + '">' + chipText + "</span>",
            });
        });
        var cols = [
            { key: "STT", label: "STT", number: 0, center: true, width: 50 },
            { key: "ItemCode", label: "ItemCode", width: 110 },
            { key: "TenVT", label: "Tên VT", width: 220 },
            { key: "LoaiKho", label: "Loại", center: true, width: 60 },
            { key: "DonVi", label: "ĐV", center: true, width: 50 },
            { key: "SLTon", label: "SL tồn", number: 1, width: 110 },
            { key: "ViTriKe", label: "Vị trí", center: true, width: 90 },
            { key: "_HanDung", label: "Hạn dùng", raw: true, center: true, width: 110 },
            { key: "POMua", label: "Số PO gần nhất", center: true, width: 120 },
            { key: "SoLo", label: "Số lô gần nhất", center: true, width: 120 },
            { key: "TenKH", label: "Khách hàng", width: 160 },
            { key: "DonGia", label: "Đơn giá", number: 0, width: 110 },
            { key: "ThanhTien", label: "Thành tiền", number: 0, width: 130 },
        ];
        byId("kpiTbody").innerHTML = renderDetailTable(cols, rowsView);
    }
    function bindKpiSubtabs() {
        content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
            b.addEventListener("click", function () {
                content.querySelectorAll(".dk-kk-subtab").forEach(function (x) {
                    x.classList.remove("active");
                });
                this.classList.add("active");
                activeKey = this.getAttribute("data-tab-key");
                fetchData();
            });
        });
    }
    function fetchData() {
        content.innerHTML = modalLoading();
        requestJson(
            "/api/DashboardKhoDesktop/GetTonKhoChiTiet?denNgay=" +
            encodeURIComponent(state.dateFilter.to) +
            "&loai=" +
            encodeURIComponent(activeKey),
        )
            .then(function (d) {
                allRows = normalizeArray(d);
                paint();
            })
            .catch(function (err) {
                content.innerHTML = modalErrorBox(err && err.message);
            });
    }
    fetchData();
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

// ─── #5 PO đang trễ ──────────────────────────────────────────────
function renderPOTreDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var titleEl = byId(ids.detailModalTitle);
    if (titleEl) titleEl.textContent = "PO đã về kho — đang chờ kiểm";
    var tabs = [
        { key: "all", label: "Tất cả", icon: "fa-list" },
        { key: "chua_qc", label: "Chưa QC", icon: "fa-clipboard-question" },
        { key: "dang_qc", label: "Đang QC", icon: "fa-spinner" },
        { key: "da_qc_chua_nk", label: "Đã QC chưa nhập", icon: "fa-clipboard-check" },
    ];
    var activeKey = "all";
    var allRows = [];
    function paint() {
        var soNCC = {},
            treSmall = 0,
            treBig = 0;
        allRows.forEach(function (r) {
            soNCC[r.NCC] = 1;
            var h = toNumber(r.SoGioTre);
            if (h > 168) treBig++;
            else if (h <= 72) treSmall++;
        });
        content.innerHTML =
            renderKpiSummaryStrip([
                { label: "Tổng PO trễ", value: formatNumber(allRows.length, 0), sub: "PO", cls: "danger" },
                {
                    label: "Số NCC",
                    value: formatNumber(Object.keys(soNCC).length, 0),
                    sub: "nhà cung cấp",
                    cls: "warn",
                },
                { label: "Trễ ≤ 3 ngày", value: formatNumber(treSmall, 0), sub: "PO", cls: "good" },
                { label: "Trễ > 7 ngày", value: formatNumber(treBig, 0), sub: "PO", cls: "danger" },
            ]) +
            renderKpiSubtabs(tabs, activeKey) +
            renderKpiFilterBar("Tìm số PO, NCC...") +
            '<div id="kpiTbody"></div>';
        bindKpiSubtabs();
        bindKpiSearch(content, doFilter);
        doFilter();
    }
    function doFilter() {
        var q = (byId("kpiSearch").value || "").trim().toLowerCase();
        var filtered = q
            ? allRows.filter(function (r) {
                var hay = ((r.POMua || "") + " " + (r.NCC || "")).toLowerCase();
                return hay.indexOf(q) >= 0;
            })
            : allRows;
        renderTable(filtered);
        var cnt = byId("kpiCount");
        if (cnt) cnt.textContent = filtered.length + " dòng";
        var rcEl = byId("detailModalRowCount");
        if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
    }
    function renderTable(rows) {
        var rowsView = rows.map(function (r) {
            var h = toNumber(r.SoGioTre);
            var cellCls = h > 168 ? "dk-cell-late-extreme" : h >= 48 ? "dk-cell-late-warn" : "";
            var ttCls = r.MaTT === "chua_qc" ? "danger" : r.MaTT === "dang_qc" ? "warn" : "good";
            return Object.assign({}, r, {
                _SoGio: '<span class="dk-cell-num ' + cellCls + '">' + formatNumber(h, 0) + " h</span>",
                _TrangThai:
                    '<span class="dk-status-chip dk-status-chip-' +
                    ttCls +
                    '">' +
                    escapeHtml(r.TrangThai || "") +
                    "</span>",
            });
        });
        var cols = [
            { key: "STT", label: "STT", number: 0, center: true, width: 50 },
            { key: "POMua", label: "Số PO", center: true, width: 130 },
            { key: "NCC", label: "NCC", width: 140 },
            { key: "NgayDuKien", label: "Dự kiến", date: true, center: true, width: 100 },
            { key: "NgayVeThucTe", label: "Về thực tế", date: true, center: true, width: 100 },
            { key: "_SoGio", label: "Giờ trễ", raw: true, width: 90 },
            { key: "SoVT", label: "Số VT", number: 0, center: true, width: 70 },
            { key: "SL", label: "SL", number: 1, width: 100 },
            { key: "_TrangThai", label: "Trạng thái", raw: true, center: true, width: 140 },
        ];
        byId("kpiTbody").innerHTML = renderDetailTable(cols, rowsView);
    }
    function bindKpiSubtabs() {
        content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
            b.addEventListener("click", function () {
                content.querySelectorAll(".dk-kk-subtab").forEach(function (x) {
                    x.classList.remove("active");
                });
                this.classList.add("active");
                activeKey = this.getAttribute("data-tab-key");
                fetchData();
            });
        });
    }
    function fetchData() {
        content.innerHTML = modalLoading();
        requestJson("/api/DashboardKhoDesktop/GetPODangTreChiTiet?groupBy=" + encodeURIComponent(activeKey))
            .then(function (d) {
                allRows = normalizeArray(d);
                paint();
            })
            .catch(function (err) {
                content.innerHTML = modalErrorBox(err && err.message);
            });
    }
    fetchData();
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

/**
 * Mở Modal hiển thị danh sách các Công việc chờ xử lý (Nhập/Xuất/Tồn). Xử lý riêng biệt nhiều Tab bên trong.
 */
function renderTodoDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var BASE = "/api/DashboardKhoDesktop/";
    var tabs = [
        { key: "itemcode_cho_nk", label: "ItemCode chờ nhập kho", icon: "fa-clipboard-check", color: "blue" },
        { key: "kk_cho_duyet", label: "Kiểm kê chờ duyệt", icon: "fa-clipboard-list", color: "violet" },
    ];
    var tabHtml = '<div class="dk-todo-tab-strip">';
    for (var i = 0; i < tabs.length; i++) {
        tabHtml +=
            '<button type="button" class="dk-todo-tab dk-todo-tab-' +
            tabs[i].color +
            (i === 0 ? " active" : "") +
            '" data-todo-key="' +
            tabs[i].key +
            '">' +
            '<i class="fa-solid ' +
            tabs[i].icon +
            '"></i>' +
            '<span class="dk-todo-tab-label">' +
            escapeHtml(tabs[i].label) +
            "</span>" +
            '<span class="dk-todo-tab-badge" data-todo-badge="' +
            tabs[i].key +
            '">…</span>' +
            "</button>";
    }
    tabHtml += "</div>";
    tabHtml += '<div id="todoTabBody">' + modalLoading() + "</div>";
    content.innerHTML = tabHtml;

    var cache = {};
    function loadTab(key) {
        var body = byId("todoTabBody");
        if (!body) return;
        body.classList.remove("show");
        body.classList.add("fade-out");
        setTimeout(function () {
            if (cache[key]) {
                paint(key, cache[key]);
                return;
            }
            body.innerHTML = modalLoading();
            requestJson(BASE + "GetTodoDetail?type=" + encodeURIComponent(key))
                .then(function (data) {
                    var rows = normalizeArray(data);
                    cache[key] = rows;
                    paint(key, rows);
                    var badge = document.querySelector('[data-todo-badge="' + key + '"]');
                    if (badge) badge.textContent = String(rows.length).padStart(2, "0");
                })
                .catch(function (err) {
                    body.innerHTML = modalErrorBox(err && err.message);
                });
        }, 120);
    }
    function paint(key, rows) {
        var body = byId("todoTabBody");
        if (!body) return;
        if (rows.length === 0) {
            body.innerHTML =
                '<div class="dk-todo-empty"><i class="fa-solid fa-folder-open"></i><div>Không có item nào chờ xử lý</div></div>';
        } else {
            var searchBar =
                '<div class="dk-todo-searchbar">' +
                '<i class="fa-solid fa-magnifying-glass"></i>' +
                '<input type="text" id="todoSearch" placeholder="Tìm ItemCode, tên VT, PO mua, màu..." />' +
                '<button type="button" id="todoSearchClear" class="dk-todo-search-clear">&times;</button>' +
                '<span id="todoSearchCount" class="dk-todo-search-count">' +
                rows.length +
                " kết quả</span>" +
                "</div>";
            var commonHead = [
                { key: "STT", label: "STT", number: 0, center: true, width: 44 },
                { key: "ItemCode", label: "ItemCode", width: 110 },
                { key: "_TenVT", label: "Tên vật tư", raw: true, width: 180 },
                { key: "MaMauVT", label: "Mã màu", center: true, width: 80 },
                { key: "_MauVT", label: "Màu VT", raw: true, width: 130 },
            ];
            var commonTail = [{ key: "NgayTao", label: "Ngày tạo", date: true, center: true, width: 100 }];
            var cols;
            if (key === "itemcode_cho_nk") {
                cols = commonHead.concat(
                    [
                        { key: "WidthSize", label: "Width/Size", center: true, width: 100 },
                        { key: "_SLMua", label: "SL mua", raw: true, width: 90 },
                        { key: "_SLVe", label: "SL về", raw: true, width: 100 },
                        { key: "NCC", label: "NCC", width: 130 },
                    ],
                    commonTail,
                );
            } else if (key === "kk_cho_duyet") {
                cols = commonHead.concat(
                    [
                        { key: "WidthSize", label: "Width/Size", center: true, width: 100 },
                        { key: "DonVi", label: "ĐV", center: true, width: 50 },
                        { key: "_TonKho", label: "Tồn kho", raw: true, width: 90 },
                        { key: "_SLKiemKe", label: "SL kiểm kê", raw: true, width: 100 },
                        { key: "_ChenhLech", label: "Chênh lệch (m)", raw: true, center: true, width: 110 },
                    ],
                    commonTail,
                );
            } else {
                cols = commonHead.concat([{ key: "DonVi", label: "ĐV", center: true, width: 50 }], commonTail);
            }
            function fmtSL(val, unit, extraCls) {
                if (val == null || val === "" || val === 0 || val === "0") return "";
                var cls = "dk-cell-num" + (extraCls ? " " + extraCls : "");
                return (
                    '<span class="' +
                    cls +
                    '">' +
                    formatNumber(toNumber(val), 1) +
                    (unit ? " " + escapeHtml(unit) : "") +
                    "</span>"
                );
            }
            var rowsView = rows.map(function (r) {
                var tenMau = r.MauVT ? String(r.MauVT).trim() : "";
                var mauVTHtml = tenMau
                    ? '<span class="dk-mau-swatch"></span><span class="dk-mau-text">' + escapeHtml(tenMau) + "</span>"
                    : "";
                var tenVT = r.TenVT ? String(r.TenVT) : "";
                var tenVTHtml =
                    '<span class="dk-cell-tenvt" title="' + escapeHtml(tenVT) + '">' + escapeHtml(tenVT) + "</span>";

                var slMuaHtml = fmtSL(r.SLMua);
                var slVeHtml = "";
                if (r.SLVe != null && r.SLVe !== "") {
                    var mua = toNumber(r.SLMua),
                        ve = toNumber(r.SLVe);
                    if (mua > 0 && ve < mua) {
                        slVeHtml = fmtSL(r.SLVe, null, "dk-cell-warn");
                    } else if (mua > 0 && ve === mua) {
                        slVeHtml =
                            '<i class="fa-solid fa-circle-check dk-cell-good" style="margin-right:4px"></i>' +
                            fmtSL(r.SLVe, null, "dk-cell-good");
                    } else {
                        slVeHtml = fmtSL(r.SLVe);
                    }
                }

                var chenhHtml = "";
                if (r.ChenhLech != null && r.ChenhLech !== "") {
                    var d = toNumber(r.ChenhLech);
                    var sign = d > 0 ? "+" : d < 0 ? "−" : "";
                    var cls = d > 0 ? "dk-cell-good" : d < 0 ? "dk-cell-danger" : "dk-cell-muted";
                    chenhHtml =
                        '<span class="dk-cell-num ' + cls + '">' + sign + formatNumber(Math.abs(d), 1) + " m</span>";
                }

                return Object.assign({}, r, {
                    _TenVT: tenVTHtml,
                    _MauVT: mauVTHtml,
                    _SLMua: slMuaHtml,
                    _SLVe: slVeHtml,
                    _TonKho: fmtSL(r.TonKho, r.DonVi),
                    _SLKiemKe: fmtSL(r.SLKiemKe, r.DonVi),
                    _ChenhLech: chenhHtml,
                });
            });
            // v2.4.6 — Group rows by POMua → render với group header row có thể collapse
            body.innerHTML =
                searchBar + '<div id="todoTableWrap">' + renderTodoGroupedTable(cols, rowsView, false) + "</div>";
            // v2.7.1 — Cập nhật tổng số dòng ở footer modal
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            // Bind search
            var searchInput = byId("todoSearch");
            var searchCount = byId("todoSearchCount");
            if (searchInput) {
                setTimeout(function () {
                    searchInput.focus();
                }, 150);
                searchInput.addEventListener("input", function () {
                    var q = (this.value || "").trim().toLowerCase();
                    var trs = body.querySelectorAll("#todoTableWrap tbody tr");
                    var visible = 0;
                    for (var ti = 0; ti < trs.length; ti++) {
                        var match = q === "" || trs[ti].textContent.toLowerCase().indexOf(q) >= 0;
                        trs[ti].style.display = match ? "" : "none";
                        if (match) visible++;
                    }
                    searchCount.textContent = visible + " kết quả";
                });
            }
            var clearBtn = byId("todoSearchClear");
            if (clearBtn)
                clearBtn.onclick = function () {
                    searchInput.value = "";
                    var ev = new Event("input");
                    searchInput.dispatchEvent(ev);
                    searchInput.focus();
                };
        }
        body.classList.remove("fade-out");
        setTimeout(function () {
            body.classList.add("show");
        }, 10);
    }
    var tabBtns = content.querySelectorAll(".dk-todo-tab");
    for (var ti = 0; ti < tabBtns.length; ti++) {
        tabBtns[ti].addEventListener("click", function () {
            var allBtns = content.querySelectorAll(".dk-todo-tab");
            for (var k = 0; k < allBtns.length; k++) allBtns[k].classList.remove("active");
            this.classList.add("active");
            loadTab(this.getAttribute("data-todo-key"));
        });
    }

    var initialKey = tabs[0].key;
    if (window.__dkPendingTodoType) {
        for (var pi = 0; pi < tabs.length; pi++) {
            if (tabs[pi].key === window.__dkPendingTodoType) {
                initialKey = tabs[pi].key;
                break;
            }
        }
        var allBtns = content.querySelectorAll(".dk-todo-tab");
        for (var ki = 0; ki < allBtns.length; ki++) {
            allBtns[ki].classList.toggle("active", allBtns[ki].getAttribute("data-todo-key") === initialKey);
        }
        window.__dkPendingTodoType = null;
    }
    loadTab(initialKey);
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

function renderTop5VTAllModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    content.innerHTML = modalLoading("Đang tải danh sách vật tư theo dung tích...");
    requestJson("/api/DashboardKhoDesktop/GetVatTuTheoDungTich")
        .then(function (data) {
            var rows = normalizeArray(data);
            var tongCBM = 0,
                slNL = 0,
                slPL = 0;
            for (var i = 0; i < rows.length; i++) {
                tongCBM += toNumber(rows[i].CBM);
                if (rows[i].LoaiKho === "NL") slNL++;
                else if (rows[i].LoaiKho === "PL") slPL++;
            }
            var avg = rows.length > 0 ? tongCBM / rows.length : 0;
            var sumHtml = renderSummaryStrip([
                { label: "Tổng CBM", value: formatNumber(tongCBM, 1), sub: rows.length + " mã VT" },
                { label: "Vật tư NL", value: formatNumber(slNL, 0), sub: "mã", kind: "primary" },
                { label: "Vật tư PL", value: formatNumber(slPL, 0), sub: "mã", kind: "warn" },
                { label: "Avg/mã", value: formatNumber(avg, 2), sub: "CBM" },
            ]);
            var cols = [
                { key: "STT", label: "#", number: 0, center: true, width: 40 },
                { key: "MaVT", label: "Mã VT", drillTo: "matCountDrill_codes", width: 130 },
                { key: "TenVT", label: "Tên vật tư" },
                { key: "LoaiKho", label: "Loại", center: true, width: 70 },
                { key: "CBM", label: "CBM", number: 2, sortable: true, width: 100 },
                { key: "TyTrong", label: "Tỷ trọng", percent: true, sortable: true, width: 90 },
                { key: "ViTriKe", label: "Vị trí kệ", center: true, width: 100 },
            ];
            content.innerHTML =
                sumHtml + '<div id="top5VTBodyTbl">' + renderSortableTable(cols, rows, { highlightTopN: 5 }) + "</div>";
            wireSortableTable(byId("top5VTBodyTbl"), cols, rows, { highlightTopN: 5 });
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            // Click cell MaVT → drill về popup ItemCode
            content.querySelectorAll("[data-mavt-drill]").forEach(function (el) {
                el.addEventListener("click", function (e) {
                    e.preventDefault();
                    openDetail("matCountDrill_codes", -1);
                });
            });
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
}

// ════════════════════════════════════════════════════════════════
//  — Modal: Top 5 KH (full 15 với pct bar + drill vị trí kệ)
// ════════════════════════════════════════════════════════════════
function renderTop5KHAllModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    content.innerHTML = modalLoading("Đang tải danh sách khách hàng theo giá trị tồn...");
    requestJson("/api/DashboardKhoDesktop/GetKhachHangTonKhoChiTiet")
        .then(function (data) {
            var rows = normalizeArray(data);
            var tongGT = 0,
                kt = 0;
            for (var i = 0; i < rows.length; i++) {
                tongGT += toNumber(rows[i].GiaTri);
                if (rows[i].KhachHang === "Khách trống") kt = toNumber(rows[i].GiaTri);
            }
            var avg = rows.length > 0 ? tongGT / rows.length : 0;
            var sumHtml = renderSummaryStrip([
                { label: "Tổng giá trị (VND)", value: formatNumber(tongGT, 0) },
                { label: "Số khách", value: formatNumber(rows.length, 0), sub: "khách hàng", kind: "primary" },
                { label: "Khách trống", value: formatNumber(kt, 0), sub: "VND", kind: "warn" },
                { label: "Avg/khách", value: formatNumber(avg, 0), sub: "VND" },
            ]);
            // Tìm max ty trong for pct bar
            var maxPct = 0;
            for (var j = 0; j < rows.length; j++) {
                if (toNumber(rows[j].TyTrong) > maxPct) maxPct = toNumber(rows[j].TyTrong);
            }
            var tableHtml =
                '<table class="dk-detail-table dk-top5kh-table"><thead><tr>' +
                '<th style="width:40px;text-align:center">#</th>' +
                "<th>Khách hàng</th>" +
                '<th style="width:100px">Mã KH</th>' +
                '<th style="width:80px;text-align:right">Số mã VT</th>' +
                '<th style="width:100px;text-align:right">Tổng CBM</th>' +
                '<th style="width:140px;text-align:right">Giá trị tồn (VND)</th>' +
                '<th style="width:180px">Tỷ trọng</th>' +
                "</tr></thead><tbody>";
            for (var k = 0; k < rows.length; k++) {
                var r = rows[k];
                var p = toNumber(r.TyTrong);
                var pBarW = maxPct > 0 ? (p / maxPct) * 100 : 0;
                tableHtml +=
                    '<tr class="dk-row-kh js-open-detail clickable" data-detail="customerRow" data-makh="' +
                    escapeHtml(r.MaKH || "") +
                    '" data-tenkh="' +
                    escapeHtml(r.KhachHang || "") +
                    '">' +
                    '<td style="text-align:center">' +
                    escapeHtml(r.STT) +
                    "</td>" +
                    "<td>" +
                    escapeHtml(r.KhachHang || "") +
                    "</td>" +
                    '<td class="dk-cell-mono">' +
                    escapeHtml(r.MaKH || "") +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(r.SoMaVT), 0) +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(r.TongCBM), 4) +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(r.GiaTri), 0) +
                    "</td>" +
                    '<td><div class="dk-pct-bar-wrap"><div class="dk-pct-bar-fill" style="width:' +
                    pBarW.toFixed(1) +
                    '%"></div><span class="dk-pct-bar-label">' +
                    formatNumber(p, 1) +
                    "%</span></div></td>" +
                    "</tr>";
            }
            tableHtml += "</tbody></table>";
            content.innerHTML = sumHtml + tableHtml;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            content.querySelectorAll(".dk-row-kh").forEach(function (tr) {
                tr.addEventListener("click", function () {
                    var customerItem = { MaKH: this.getAttribute("data-makh"), TenKH: this.getAttribute("data-tenkh") };
                    _detailStack.push({ detail: _currentDetail, index: _currentDetailIndex });
                    _currentDetail = "_customerRow_drill";
                    _currentDetailIndex = -1;
                    renderDetailModalDirect({
                        title: "Vị trí kệ — " + (customerItem.TenKH || customerItem.MaKH),
                        rows: [],
                        columns: [],
                        customDrillCustomer: customerItem,
                    });
                });
            });
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
}

function loadCustomerMaterialDetail(customerItem) {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    content.innerHTML = modalLoading("Đang tải chi tiết vật tư của khách hàng...");

    var maOrTen = customerItem.MaKH || customerItem.TenKH;
    requestJson("/api/DashboardKhoDesktop/GetVatTuTheoKhachHang?maKH=" + encodeURIComponent(maOrTen))
        .then(function (data) {
            var rows = normalizeArray(data);
            if (rows.length === 0) {
                content.innerHTML =
                    '<div class="dk-empty" style="padding:30px">Khách hàng "' +
                    (customerItem.TenKH || customerItem.MaKH) +
                    '" chưa có vật tư tồn kho.</div>';
                return;
            }

            var sumQty = 0,
                sumCBM = 0;
            for (var i = 0; i < rows.length; i++) {
                sumQty += toNumber(rows[i].SoLuong);
                sumCBM += toNumber(rows[i].TongCBM);
            }

            var sumHtml = renderSummaryStrip([
                { label: "Tổng số lượng", value: formatNumber(sumQty, 0) },
                { label: "Tổng CBM", value: formatNumber(sumCBM, 4), kind: "primary" },
                { label: "Số mặt hàng", value: formatNumber(rows.length, 0), kind: "success" },
            ]);

            var tableHtml =
                '<div class="dk-detail-table-wrap"><table class="dk-detail-table"><thead><tr>' +
                '<th style="width:40px;text-align:center">#</th>' +
                '<th style="width:140px">Mã vật tư</th>' +
                '<th style="width:250px">Tên vật tư</th>' +
                '<th style="width:80px;text-align:center">Màu</th>' +
                '<th style="width:80px;text-align:center">Khổ</th>' +
                '<th style="width:110px;text-align:right">Số lượng</th>' +
                '<th style="width:110px;text-align:right">Tổng CBM</th>' +
                '<th style="width:120px;text-align:center">Vị trí kệ</th>' +
                "</tr></thead><tbody>";

            for (var k = 0; k < rows.length; k++) {
                var r = rows[k];
                tableHtml +=
                    "<tr>" +
                    '<td style="text-align:center">' +
                    (k + 1) +
                    "</td>" +
                    '<td class="dk-cell-mono">' +
                    escapeHtml(r.ItemCode || "") +
                    "</td>" +
                    '<td style="text-align:left">' +
                    escapeHtml(r.TenVT || "") +
                    "</td>" +
                    '<td class="text-center">' +
                    escapeHtml(r.Mau || "") +
                    "</td>" +
                    '<td class="text-center">' +
                    escapeHtml(r.KhoVai || "") +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(r.SoLuong), 0) +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(r.TongCBM), 4) +
                    "</td>" +
                    '<td class="text-center"><span class="dk-status-chip dk-status-chip-good">' +
                    escapeHtml(r.ViTriKe || "") +
                    "</span></td>" +
                    "</tr>";
            }
            tableHtml += "</tbody></table></div>";
            content.innerHTML = sumHtml + tableHtml;

            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
}

// ════════════════════════════════════════════════════════════════
// Modal: Vật tư sắp hết hạn
// ════════════════════════════════════════════════════════════════
function renderHetHanAllModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    content.innerHTML = modalLoading();
    requestJson("/api/DashboardKhoDesktop/GetVatTuSapHetHanChiTiet")
        .then(function (data) {
            var rows = normalizeArray(data);
            var today = new Date();
            today.setHours(0, 0, 0, 0);
            rows.forEach(function (r) {
                r._ConLai = r.NgayHetHan ? Math.round((new Date(r.NgayHetHan) - today) / 86400000) : 0;
            });
            rows.sort(function (a, b) {
                return a._ConLai - b._ConLai;
            });
            var quaHan = 0,
                w7 = 0,
                w30 = 0;
            for (var i = 0; i < rows.length; i++) {
                var cl = rows[i]._ConLai;
                if (cl <= 0) quaHan++;
                else if (cl <= 7) w7++;
                else if (cl <= 30) w30++;
            }
            var sumHtml = renderSummaryStrip([
                { label: "Tổng vật tư", value: formatNumber(rows.length, 0) },
                { label: "Quá hạn", value: formatNumber(quaHan, 0), kind: "danger" },
                { label: "≤ 7 ngày", value: formatNumber(w7, 0), kind: "danger" },
                { label: "≤ 30 ngày", value: formatNumber(w30, 0), kind: "warn" },
            ]);
            var filterHtml =
                '<div class="dk-modal-filter-bar">' +
                '<label class="dk-text-muted">Lọc:</label>' +
                '<select id="hetHanFilter" class="dk-filter-select-inline">' +
                '<option value="all">Tất cả</option>' +
                '<option value="qh">Quá hạn</option>' +
                '<option value="7">≤ 7 ngày</option>' +
                '<option value="30">≤ 30 ngày</option>' +
                '<option value="90">≤ 90 ngày</option>' +
                "</select>" +
                "</div>";
            var tableHtml =
                '<table class="dk-detail-table"><thead><tr>' +
                '<th style="width:110px">Mã VT</th><th>Tên vật tư</th>' +
                '<th style="width:50px;text-align:center">Loại</th>' +
                '<th style="width:80px">Lô</th>' +
                '<th style="width:90px;text-align:center">Ngày SX</th>' +
                '<th style="width:100px;text-align:center">Hết hạn</th>' +
                '<th style="width:90px;text-align:center">Còn lại</th>' +
                '<th style="width:80px;text-align:right">Tồn kho</th>' +
                '<th style="width:50px;text-align:center">ĐV</th>' +
                '<th style="width:80px">Vị trí</th></tr></thead><tbody id="hetHanTbody">';
            tableHtml += hetHanRowsHtml(rows);
            tableHtml += "</tbody></table>";
            content.innerHTML = sumHtml + filterHtml + tableHtml;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(rows.length, 0);
            byId("hetHanFilter").addEventListener("change", function () {
                var v = this.value;
                var filtered = rows.filter(function (r) {
                    var cl = r._ConLai;
                    if (v === "qh") return cl <= 0;
                    if (v === "7") return cl > 0 && cl <= 7;
                    if (v === "30") return cl > 0 && cl <= 30;
                    if (v === "90") return cl > 0 && cl <= 90;
                    return true;
                });
                byId("hetHanTbody").innerHTML = hetHanRowsHtml(filtered);
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
            });
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
}

function hetHanRowsHtml(rows) {
    if (rows.length === 0) return '<tr><td colspan="10" class="dk-empty">Không có vật tư phù hợp bộ lọc</td></tr>';
    return rows
        .map(function (r) {
            var cl = r._ConLai;
            var cls =
                cl <= 0
                    ? "dk-conlai-overdue"
                    : cl <= 7
                        ? "dk-conlai-urgent"
                        : cl <= 30
                            ? "dk-conlai-warn"
                            : "dk-conlai-soft";
            var icon = cl <= 0 ? '<i class="fa-solid fa-triangle-exclamation"></i> ' : "";
            return (
                "<tr>" +
                '<td class="dk-cell-mono">' +
                escapeHtml(r.MaVT || "") +
                "</td>" +
                "<td>" +
                escapeHtml(r.TenVT || "") +
                "</td>" +
                '<td class="text-center">' +
                escapeHtml(r.LoaiKho || "") +
                "</td>" +
                "<td>" +
                escapeHtml(r.Lo || "") +
                "</td>" +
                '<td class="text-center">' +
                formatDate(r.NgaySX) +
                "</td>" +
                '<td class="text-center">' +
                formatDate(r.NgayHetHan) +
                "</td>" +
                '<td class="text-center ' +
                cls +
                '">' +
                icon +
                (cl <= 0 ? "Quá " + Math.abs(cl) : cl) +
                " ngày</td>" +
                '<td class="text-end dk-cell-num">' +
                formatNumber(toNumber(r.TonKho), 1) +
                "</td>" +
                '<td class="text-center">' +
                escapeHtml(r.DonVi || "") +
                "</td>" +
                "<td>" +
                escapeHtml(r.ViTri || "") +
                "</td>" +
                "</tr>"
            );
        })
        .join("");
}

// ════════════════════════════════════════════════════════════════
// Modal: Giá trị nhóm
// ════════════════════════════════════════════════════════════════
function renderGiaTriNhomAllModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    content.innerHTML = modalLoading();
    requestJson("/api/DashboardKhoDesktop/GetGiaTriNhomChiTiet")
        .then(function (data) {
            var rows = normalizeArray(data);
            var groups = rows.filter(function (r) {
                return toNumber(r.IsGroup) === 1;
            });
            var subs = rows.filter(function (r) {
                return toNumber(r.IsGroup) === 0;
            });
            var byParent = {};
            subs.forEach(function (s) {
                var p = s.ParentNhom;
                if (!byParent[p]) byParent[p] = [];
                byParent[p].push(s);
            });
            var html =
                '<div id="giaTriNhomDonut" class="dk-nhom-donut-modal" style="min-height:500px; height:500px; margin-bottom:30px; overflow:visible;"></div>';
            html +=
                '<table class="dk-detail-table dk-tree-table"><thead><tr>' +
                '<th style="width:36px"></th>' +
                '<th style="width:40px">STT</th>' +
                "<th>Nhóm</th>" +
                '<th style="width:90px;text-align:right">Số mã VT</th>' +
                '<th style="width:100px;text-align:right">Tổng CBM</th>' +
                '<th style="width:140px;text-align:right">Giá trị</th>' +
                '<th style="width:100px;text-align:right">Tỷ trọng</th>' +
                "</tr></thead><tbody>";
            for (var i = 0; i < groups.length; i++) {
                var g = groups[i];
                var subList = byParent[g.Nhom] || [];
                html +=
                    '<tr class="dk-tree-row dk-tree-group" data-nhom="' +
                    escapeHtml(g.Nhom) +
                    '">' +
                    '<td class="text-center"><button type="button" class="dk-tree-toggle" data-nhom-toggle="' +
                    escapeHtml(g.Nhom) +
                    '"><i class="fa-solid fa-chevron-right"></i></button></td>' +
                    '<td class="text-center">' +
                    g.STT +
                    "</td>" +
                    "<td><b>" +
                    escapeHtml(g.Nhom) +
                    "</b></td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(g.SoMaVT), 0) +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(g.TongCBM), 4) +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(g.GiaTri), 0) +
                    "</td>" +
                    '<td class="text-end dk-cell-num">' +
                    formatNumber(toNumber(g.TyTrong), 1) +
                    "%</td>" +
                    "</tr>";
                if (subList.length > 0) {
                    html +=
                        '<tr class="dk-tree-sub-wrap" data-nhom-sub="' +
                        escapeHtml(g.Nhom) +
                        '" style="display:none"><td colspan="7">' +
                        '<table class="dk-tree-sub-table"><thead><tr>' +
                        '<th style="width:130px">Mã VT</th><th>Tên vật tư</th>' +
                        '<th style="width:90px;text-align:right">CBM</th>' +
                        '<th style="width:140px;text-align:right">Giá trị</th>' +
                        "</tr></thead><tbody>";
                    for (var j = 0; j < subList.length; j++) {
                        var s = subList[j];
                        html +=
                            '<tr><td class="dk-cell-mono">' +
                            escapeHtml(s.MaVT || "") +
                            "</td>" +
                            "<td>" +
                            escapeHtml(s.Nhom || "") +
                            "</td>" +
                            '<td class="text-end dk-cell-num">' +
                            formatNumber(toNumber(s.TongCBM), 4) +
                            "</td>" +
                            '<td class="text-end dk-cell-num">' +
                            formatNumber(toNumber(s.GiaTri), 0) +
                            "</td></tr>";
                    }
                    html += "</tbody></table></td></tr>";
                }
            }
            html += "</tbody></table>";
            content.innerHTML = html;
            var rcEl = byId("detailModalRowCount");
            if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(groups.length, 0);
            // Render donut Highcharts
            if (typeof Highcharts !== "undefined" && groups.length > 0) {
                applyHighchartsTheme();
                Highcharts.chart("giaTriNhomDonut", {
                    chart: { type: "pie", backgroundColor: "transparent", height: 460 },
                    title: { text: null },
                    credits: { enabled: false },
                    tooltip: {
                        useHTML: true,
                        pointFormat:
                            "<b>{point.name}</b><br/>Giá trị: <b>{point.y:,.0f} VND</b><br/>Tỷ trọng: <b>{point.pct:.1f}%</b>",
                    },
                    plotOptions: {
                        pie: {
                            innerSize: "55%",
                            borderWidth: 2,
                            showInLegend: true,
                            dataLabels: {
                                enabled: true,
                                distance: 15,
                                allowOverlap: true,
                                format: "{point.name}<br/>{point.pct:.1f}%",
                                style: { fontSize: "10px", textOutline: "none" },
                            },
                        },
                    },
                    legend: {
                        enabled: true,
                        layout: "horizontal",
                        align: "center",
                        verticalAlign: "bottom",
                        itemStyle: { fontSize: "11px", fontWeight: "500" },
                    },
                    series: [
                        {
                            name: "Giá trị",
                            colorByPoint: true,
                            data: groups.map(function (g) {
                                return { name: g.Nhom, y: toNumber(g.GiaTri), pct: toNumber(g.TyTrong) };
                            }),
                        },
                    ],
                });
            }
            content.querySelectorAll(".dk-tree-toggle").forEach(function (btn) {
                btn.addEventListener("click", function (e) {
                    e.stopPropagation();
                    var nhom = this.getAttribute("data-nhom-toggle");
                    var sub = content.querySelector('[data-nhom-sub="' + nhom + '"]');
                    if (!sub) return;
                    var icon = this.querySelector("i");
                    if (sub.style.display === "none") {
                        sub.style.display = "";
                        if (icon) icon.classList.replace("fa-chevron-right", "fa-chevron-down");
                    } else {
                        sub.style.display = "none";
                        if (icon) icon.classList.replace("fa-chevron-down", "fa-chevron-right");
                    }
                });
            });
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
}

// ════════════════════════════════════════════════════════════════
//  Modal: Kiểm kê
// ════════════════════════════════════════════════════════════════
function renderKiemKeAllModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    content.innerHTML = modalLoading();
    requestJson("/api/DashboardKhoDesktop/GetKiemKeChiTiet")
        .then(function (data) {
            var rows = normalizeArray(data);
            var totalCheckedItems = 0;
            var totalUncheckedItems = 0;
            var cSheetsCompleted = 0;
            var cSheetsUnchecked = 0;
            var kvs = {};
            for (var i = 0; i < rows.length; i++) {
                var r = rows[i];
                var s = toNumber(r.Status);
                var da = toNumber(r.DaKiem);
                var t = toNumber(r.Tong);

                totalCheckedItems += da;
                totalUncheckedItems += t - da;
                kvs[r.KhuVuc] = 1;

                if (s === 1 || s === 2) {
                    cSheetsCompleted++;
                } else {
                    cSheetsUnchecked++;
                }
            }
            var sumHtml = renderKpiSummaryStrip(
                [
                    {
                        label: "Đã kiểm",
                        value: formatNumber(totalCheckedItems, 0),
                        sub: "Trong " + cSheetsCompleted + " phiếu",
                        cls: "good",
                    },
                    {
                        label: "Chưa kiểm",
                        value: formatNumber(totalUncheckedItems, 0),
                        sub: "Trong " + cSheetsUnchecked + " phiếu",
                        cls: "danger",
                    },
                ],
                "dk-kiemke-summary",
            );
            var tabs = [
                { key: "0", label: "Tất cả (" + rows.length + ")", icon: "fa-list" },
                { key: "1", label: "Đã kiểm (" + cSheetsCompleted + ")", icon: "fa-circle-check" },
                { key: "3", label: "Chưa kiểm (" + cSheetsUnchecked + ")", icon: "fa-circle-xmark" },
            ];
            var activeKey = "0";
            var tabHtml = renderKpiSubtabs(tabs, activeKey);
            var kvOpts = '<option value="">Tất cả khu vực</option>';
            Object.keys(kvs)
                .sort()
                .forEach(function (k) {
                    kvOpts += '<option value="' + escapeHtml(k) + '">' + escapeHtml(k) + "</option>";
                });
            var filterHtml =
                '<div class="dk-modal-filter-bar">' +
                '<i class="fa-solid fa-magnifying-glass"></i>' +
                '<input type="text" id="kkSearch" class="dk-kk-search" placeholder="Tìm mã phiếu, người phụ trách..." />' +
                '<select id="kkKhuVuc" class="dk-filter-select-inline">' +
                kvOpts +
                "</select>" +
                '<span id="kkCount" class="dk-text-muted" style="display: none;"></span>' +
                "</div>";
            var tableWrapHtml = '<div id="kpiTbody"></div>';
            content.innerHTML = sumHtml + tabHtml + filterHtml + tableWrapHtml;
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "MaPhieu", label: "Mã phiếu", center: true, width: 120 },
                { key: "NgayBatDau", label: "Ngày BĐ", date: true, center: true, width: 110 },
                { key: "KhuVuc", label: "Khu vực" },
                { key: "SoMa", label: "Số mã", number: 0, center: true, width: 80 },
                { key: "_DaTrenTong", label: "Đã/Tổng", raw: true, center: true, width: 120 },
                { key: "Pct", label: "%", percent: true, width: 80 },
                { key: "NguoiPT", label: "Người phụ trách" },
                { key: "_TrangThai", label: "Trạng thái", raw: true, center: true, width: 120 },
            ];
            function refilter() {
                var q = (byId("kkSearch").value || "").trim().toLowerCase();
                var kv = byId("kkKhuVuc").value;
                var filtered = rows.filter(function (r) {
                    var s = toNumber(r.Status);
                    if (activeKey === "1") {
                        if (s !== 1 && s !== 2) return false;
                    } else if (activeKey === "3") {
                        if (s !== 3) return false;
                    }
                    if (kv && r.KhuVuc !== kv) return false;
                    if (q) {
                        var hay = ((r.MaPhieu || "") + " " + (r.NguoiPT || "") + " " + (r.KhuVuc || "")).toLowerCase();
                        if (hay.indexOf(q) < 0) return false;
                    }
                    return true;
                });
                var mappedRows = filtered.map(function (r, idx) {
                    var s = toNumber(r.Status);
                    var chipCls = s === 1 ? "good" : s === 2 ? "warn" : "danger";
                    return Object.assign({}, r, {
                        STT: idx + 1,
                        _DaTrenTong: formatNumber(toNumber(r.DaKiem), 0) + " / " + formatNumber(toNumber(r.Tong), 0),
                        _TrangThai:
                            '<span class="dk-status-chip dk-status-chip-' +
                            chipCls +
                            '">' +
                            escapeHtml(r.TrangThai || "") +
                            "</span>",
                    });
                });
                byId("kpiTbody").innerHTML = renderDetailTable(cols, mappedRows);
                byId("kkCount").textContent = filtered.length + " phiếu";
                var rcEl = byId("detailModalRowCount");
                if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
            }
            content.querySelectorAll(".dk-kk-subtab").forEach(function (b) {
                b.addEventListener("click", function () {
                    content.querySelectorAll(".dk-kk-subtab").forEach(function (x) {
                        x.classList.remove("active");
                    });
                    this.classList.add("active");
                    activeKey = this.getAttribute("data-tab-key");
                    refilter();
                });
            });
            byId("kkSearch").addEventListener("input", refilter);
            byId("kkKhuVuc").addEventListener("change", refilter);
            refilter();
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
}

/**
 * Mở Modal danh sách các Lỗi cảnh báo Tồn kho. Hỗ trợ hiển thị riêng theo mã lỗi cụ thể.
 */
function renderAlertDetailModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var code = window.__dkPendingAlertCode || "po_tre";
    var name = window.__dkPendingAlertName || "Cảnh báo";
    window.__dkPendingAlertCode = null;
    window.__dkPendingAlertName = null;

    var titleEl = byId(ids.detailModalTitle);
    if (titleEl) titleEl.textContent = "Chi tiết cảnh báo — " + name;

    // Map MaCB → endpoint + columns + cls
    var alertMap = {
        po_tre: {
            api: "/api/DashboardKhoDesktop/GetPODangTreChiTiet?groupBy=all",
            cls: "danger",
            cols: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "POMua", label: "Số PO", center: true, width: 130 },
                { key: "NCC", label: "NCC" },
                { key: "MaHang", label: "Mã hàng", center: true, width: 120 },
                { key: "SoVT", label: "Số VT", number: 0, center: true, width: 80 },
                { key: "SL", label: "Tổng SL", number: 2, width: 100 },
                { key: "NgayNKDuKien", label: "Dự kiến", date: true, center: true, width: 100 },
                { key: "SoGioTre", label: "Trễ (giờ)", number: 0, width: 90 },
                { key: "TrangThai", label: "Trạng thái", center: true, width: 110 },
            ],
        },
        npl_thieu: {
            api: "/api/DashboardKhoDesktop/GetNPLThieuChiTiet",
            cls: "danger",
            cols: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "ItemCode", label: "ItemCode", width: 120 },
                { key: "TenVT", label: "Tên VT" },
                { key: "LenhSX", label: "Lệnh SX", center: true, width: 130 },
                { key: "DonVi", label: "ĐV", center: true, width: 50 },
                { key: "SLCan", label: "SL cần", number: 1, width: 100 },
                { key: "SLCo", label: "SL có", number: 1, width: 100 },
                { key: "SLThieu", label: "Thiếu", number: 1, width: 100 },
            ],
        },
        kk_lech: {
            api: "/api/DashboardKhoDesktop/GetKiemKeLechChiTiet",
            cls: "warn",
            cols: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "MaPhieu", label: "Mã phiếu", center: true, width: 110 },
                { key: "KhuVuc", label: "Khu vực" },
                { key: "SLHeThong", label: "SL hệ thống", number: 1, width: 120 },
                { key: "SLThucKiem", label: "SL thực kiểm", number: 1, width: 120 },
                { key: "LechPct", label: "Lệch (%)", number: 2, width: 100 },
                { key: "NguoiPT", label: "Người phụ trách" },
            ],
        },
        ton_vuot_dm: {
            api: "/api/DashboardKhoDesktop/GetTonVuotDinhMucChiTiet",
            cls: "danger",
            cols: [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "ItemCode", label: "ItemCode", width: 120 },
                { key: "TenVT", label: "Tên VT" },
                { key: "DonVi", label: "ĐV", center: true, width: 50 },
                { key: "SLTon", label: "SL tồn", number: 1, width: 100 },
                { key: "DinhMuc", label: "Định mức", number: 1, width: 100 },
                { key: "VuotPct", label: "Vượt (%)", number: 2, width: 100 },
                { key: "ViTriKe", label: "Vị trí", center: true, width: 90 },
            ],
        },
    };
    var cfg = alertMap[code] || alertMap.po_tre;
    var allRows = [];
    function paint() {
        content.innerHTML =
            renderKpiSummaryStrip([
                { label: "Tổng cảnh báo", value: formatNumber(allRows.length, 0), sub: "dòng", cls: cfg.cls },
                {
                    label: "Mức độ",
                    value: cfg.cls === "danger" ? "Nghiêm trọng" : cfg.cls === "warn" ? "Cảnh báo" : "Thông tin",
                    sub: "",
                    cls: cfg.cls,
                },
                {
                    label: "Cập nhật",
                    value: new Date().toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" }),
                    sub: "hôm nay",
                    cls: "neutral",
                },
            ]) +
            renderKpiFilterBar("Tìm nhanh trong bảng...") +
            '<div id="kpiTbody"></div>';
        bindKpiSearch(content, doFilter);
        doFilter();
    }
    function doFilter() {
        var q = (byId("kpiSearch").value || "").trim().toLowerCase();
        var filtered = q
            ? allRows.filter(function (r) {
                return JSON.stringify(r).toLowerCase().indexOf(q) >= 0;
            })
            : allRows;
        byId("kpiTbody").innerHTML = renderDetailTable(cfg.cols, filtered);
        var cnt = byId("kpiCount");
        if (cnt) cnt.textContent = filtered.length + " dòng";
        var rcEl = byId("detailModalRowCount");
        if (rcEl) rcEl.textContent = "Tổng số dòng: " + formatNumber(filtered.length, 0);
    }
    content.innerHTML = modalLoading();
    requestJson(cfg.api)
        .then(function (d) {
            allRows = normalizeArray(d);
            paint();
        })
        .catch(function (err) {
            content.innerHTML = modalErrorBox(err && err.message);
        });
    var sb = document.querySelector(".dk-modal-search-bar");
    if (sb) sb.style.display = "none";
}

/**
 * Tải danh sách vật tư hiện có tại một vị trí (Slot/Rack) cụ thể và đưa vào Modal.
 */
function loadSlotDrillIntoModal(opts) {
    var content = byId(ids.detailModalContent);
    if (!content) return;
    var section = document.createElement("div");
    section.style.cssText = "margin-top:18px;padding-top:14px;border-top:2px solid #e2e8f0";
    section.innerHTML =
        '<div style="display:flex;align-items:center;gap:10px;margin-bottom:10px">' +
        '<b class="dk-text-title" style="font-size:14px">' +
        opts.sectionTitle +
        "</b>" +
        "</div>" +
        '<div class="dk-slot-drill-body"><div class="dk-empty" style="padding:20px">Đang tải...</div></div>';
    content.appendChild(section);
    var body = section.querySelector(".dk-slot-drill-body");

    requestJson("/api/DashboardKhoDesktop/GetRackSlotDetail")
        .then(function (data) {
            var allRows = normalizeArray(data);
            var rows = allRows.filter(opts.filterFn).map(function (r, i) {
                return Object.assign({ STT: i + 1 }, r);
            });
            if (rows.length === 0) {
                body.innerHTML = '<div class="dk-empty" style="padding:20px">' + opts.emptyText + "</div>";
                return;
            }
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true },
                { key: "LoaiKho", label: "Loại", center: true },
                { key: "TenDay", label: "Dãy", center: true },
                { key: "TenKe", label: "Kệ", center: true },
                { key: "TenO", label: "Ô", center: true },
                { key: "DanhSachKH", label: "Khách hàng" },
                { key: "DanhSachMaNPL", label: "Mã NPL" },
                { key: "SoBarCode", label: "Số barcode", number: 0, center: true },
                { key: "TongCBM", label: "Tổng CBM", number: 4, center: true },
            ];
            body.innerHTML = renderDetailTable(cols, rows);
        })
        .catch(function () {
            body.innerHTML =
                '<div class="dk-empty dk-text-danger" style="padding:20px">Lỗi tải dữ liệu — cần rebuild backend</div>';
        });
}

// v2.3.9 — Tải chi tiết cấp Ô và chèn vào cuối modal Tổng sức chứa
function loadRackSlotDetailIntoModal() {
    var content = byId(ids.detailModalContent);
    if (!content) return;

    // Placeholder section
    var section = document.createElement("div");
    section.style.cssText = "margin-top:18px;padding-top:14px;border-top:2px solid #e2e8f0";
    section.innerHTML =
        '<div style="display:flex;align-items:center;gap:10px;margin-bottom:10px">' +
        '<b class="dk-text-title" style="font-size:14px">Chi tiết theo ô (slot)</b>' +
        '<span class="dk-text-muted" style="font-size:12px">— Group theo loại kho NL / PL</span>' +
        "</div>" +
        '<div id="dkRackSlotBody" style="min-height:80px">' +
        '<div class="dk-empty" style="padding:20px">Đang tải chi tiết theo ô...</div>' +
        "</div>";
    content.appendChild(section);

    requestJson("/api/DashboardKhoDesktop/GetRackSlotDetail")
        .then(function (data) {
            var rows = normalizeArray(data);
            var body = byId("dkRackSlotBody");
            if (!body) return;
            if (rows.length === 0) {
                body.innerHTML = '<div class="dk-empty" style="padding:20px">Không có dữ liệu chi tiết theo ô</div>';
                return;
            }

            // v2.7.1 — Cols: bỏ cột Dãy (TenDay) và Mã màu (MaMauVT)
            var cols = [
                { key: "STT", label: "STT", number: 0, center: true, width: 50 },
                { key: "TenKe", label: "Kệ", center: true, width: 90 },
                { key: "TenO", label: "Ô", center: true, width: 80 },
                { key: "DanhSachKH", label: "Khách hàng" },
                { key: "MauVT", label: "Màu", center: true, width: 90 },
                { key: "WidthSize", label: "Width/Size", center: true, width: 110 },
                { key: "SoBarCode", label: "Số BC", number: 0, center: true, width: 60 },
                { key: "TongCBM", label: "CBM", number: 4, center: true, width: 90 },
            ];

            // v2.3.41 — Tách NL và PL
            var nlRows = rows
                .filter(function (r) {
                    return toNumber(r.Module) === 1;
                })
                .map(function (r, i) {
                    return Object.assign({ STT: i + 1 }, r);
                });
            var plRows = rows
                .filter(function (r) {
                    return toNumber(r.Module) === 2;
                })
                .map(function (r, i) {
                    return Object.assign({ STT: i + 1 }, r);
                });

            var totalBC = 0;
            rows.forEach(function (r) {
                totalBC += toNumber(r.SoBarCode);
            });

            var html =
                '<div style="display:flex;gap:14px;flex-wrap:wrap;margin-bottom:12px;font-size:12.5px;font-weight:700">' +
                '<span class="dk-text-title">Tổng ô đang dùng: ' +
                formatNumber(rows.length, 0) +
                "</span>" +
                '<span class="dk-text-primary">Ô NL: ' +
                formatNumber(nlRows.length, 0) +
                "</span>" +
                '<span class="dk-text-warn">Ô PL: ' +
                formatNumber(plRows.length, 0) +
                "</span>" +
                '<span class="dk-text-muted">Tổng barcode: ' +
                formatNumber(totalBC, 0) +
                "</span>" +
                "</div>";

            // Section NL
            if (nlRows.length > 0) {
                html +=
                    '<div class="dk-slot-section">' +
                    '<div class="dk-slot-section-head dk-slot-nl">' +
                    '<span class="dk-slot-section-badge">NL</span>' +
                    '<span class="dk-slot-section-title">Kho Nguyên liệu</span>' +
                    '<span class="dk-slot-section-count">' +
                    nlRows.length +
                    " ô</span>" +
                    "</div>" +
                    renderDetailTable(cols, nlRows) +
                    "</div>";
            }
            // Section PL
            if (plRows.length > 0) {
                html +=
                    '<div class="dk-slot-section" style="margin-top:16px">' +
                    '<div class="dk-slot-section-head dk-slot-pl">' +
                    '<span class="dk-slot-section-badge">PL</span>' +
                    '<span class="dk-slot-section-title">Kho Phụ liệu</span>' +
                    '<span class="dk-slot-section-count">' +
                    plRows.length +
                    " ô</span>" +
                    "</div>" +
                    renderDetailTable(cols, plRows) +
                    "</div>";
            }
            body.innerHTML = html;
        })
        .catch(function (err) {
            console.error("[Dashboard Kho] Rack slot detail error:", err);
            var body = byId("dkRackSlotBody");
            if (body)
                body.innerHTML =
                    '<div class="dk-empty dk-text-danger" style="padding:20px">Lỗi tải chi tiết theo ô. (Cần rebuild backend nếu chưa)</div>';
        });
}


// ─── Sidebar Toggle ─────────────────────────────────────────────────────────



// ─── Theme Toggle (light/dark) ─────────────────────────────────────────────
// v2.6.0 — Theme Toggle: sidebar removed, only topbar button



// v2.6.11 - Global Search All functionality
