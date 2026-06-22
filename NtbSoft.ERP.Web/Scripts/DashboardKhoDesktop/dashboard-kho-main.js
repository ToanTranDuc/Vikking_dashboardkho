/**
 * @file dashboard-kho-main.js
 * @description Điểm khởi đầu của ứng dụng. Khởi tạo dữ liệu và gán các sự kiện (Events) toàn cục.
 * @version 2.7.26
 */

//#region INITIALIZATION
/**
 * Lắng nghe tất cả các sự kiện click, bộ lọc (Filters) trên toàn trang.
 */
function bindEvents() {
    // Refresh button (sidebar)
    function refreshWithClearCache() {
        sessionStorage.setItem("dk_restore_tab", currentPage);
        requestJson("/api/DashboardKhoDesktop/ClearCache")
            .then(function () {
                window.location.reload();
            })
            .catch(function () {
                window.location.reload();
            });
    }
    var refreshButton = byId("btnRefresh");
    if (refreshButton) {
        refreshButton.onclick = function () {
            refreshWithClearCache();
        };
    }
    var legacyRefresh = byId(ids.refreshButton);
    if (legacyRefresh) {
        legacyRefresh.onclick = function () {
            refreshWithClearCache();
        };
    }

    document.addEventListener("click", function (event) {
        var target = event.target;
        if (!target) return;

        // Feature 10: maximize button
        var maxBtn = target;
        while (maxBtn && maxBtn !== document) {
            if (maxBtn.classList && maxBtn.classList.contains("dk-maximize-btn")) break;
            maxBtn = maxBtn.parentNode;
        }
        if (maxBtn && maxBtn !== document && maxBtn.classList.contains("dk-maximize-btn")) {
            var panel = maxBtn.closest ? maxBtn.closest(".dk-panel") : null;
            if (!panel) {
                var p = maxBtn;
                while (p && p !== document) {
                    if (p.classList && p.classList.contains("dk-panel")) {
                        panel = p;
                        break;
                    }
                    p = p.parentNode;
                }
            }
            if (panel) {
                togglePanelFullscreen(panel);
                return;
            }
        }

        var detailNode = findDetailNode(target);
        if (detailNode) {
            // v2.3.21 — Prevent default anchor jump
            if (event.preventDefault) event.preventDefault();
            var detail = detailNode.getAttribute("data-detail");
            var index = parseInt(detailNode.getAttribute("data-index"), 10);
            // v2.4.7 — Lưu data-todo-type để renderTodoDetailModal mở đúng tab
            var todoType = detailNode.getAttribute("data-todo-type");
            window.__dkPendingTodoType = todoType || null;
            // v2.4.16 — Lưu data-alert-code + name để renderAlertDetailModal đọc
            var alertCode = detailNode.getAttribute("data-alert-code");
            var alertName = detailNode.getAttribute("data-alert-name");
            window.__dkPendingAlertCode = alertCode || null;
            window.__dkPendingAlertName = alertName || null;
            openDetail(detail, isNaN(index) ? -1 : index);
            return;
        }

        // v2.3.27 — Back button trong modal drill: pop stack thay vì close
        var backNode = target.closest ? target.closest(".js-modal-back") : null;
        if (!backNode) {
            var t2 = target;
            while (t2 && t2 !== document) {
                if (t2.classList && t2.classList.contains("js-modal-back")) {
                    backNode = t2;
                    break;
                }
                t2 = t2.parentNode;
            }
        }
        if (backNode) {
            if (event.preventDefault) event.preventDefault();
            if (_detailStack.length > 0) {
                var parent = _detailStack.pop();
                _currentDetail = parent.detail;
                _currentDetailIndex = parent.index;
                renderDetailModal(parent.detail, parent.index);
            } else {
                closeDetailModal();
            }
            return;
        }

        var closeNode = findCloseNode(target);
        if (closeNode) {
            // v2.3.27 — Nếu có stack thì X cũng quay lại parent (giống back)
            if (_detailStack.length > 0) {
                var p = _detailStack.pop();
                _currentDetail = p.detail;
                _currentDetailIndex = p.index;
                renderDetailModal(p.detail, p.index);
            } else {
                closeDetailModal();
            }
        }
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            // Close fullscreen first, then modal
            var fsPanel = document.querySelector(".dk-panel-fullscreen");
            if (fsPanel) {
                togglePanelFullscreen(fsPanel);
                return;
            }
            closeDetailModal();
        }
    });

    // Feature 11: Customer filter dropdown
    var custFilter = byId("customerFilterSelect");
    if (custFilter) {
        custFilter.addEventListener("change", function () {
            activeCustomerFilter = this.value;
            renderCustomerPieChart();
            renderCustomersTable();
        });
    }

    // Feature 6: Activity calendar week-count selector
    var actWeekSel = byId("activityWeeksSelect");
    if (actWeekSel) {
        actWeekSel.addEventListener("change", function () {
            activityWeeksCount = parseInt(this.value, 10) || 13;
            renderActivityCalendar();
        });
    }

    // Event listeners for trend and load filter selects
    var trendFilter = byId("trendFilterSelect");
    if (trendFilter) {
        trendFilter.addEventListener("change", function () {
            renderLpcpBottomCharts();
        });
    }
    var volumeFilter = byId("volumeFilterSelect");
    if (volumeFilter) {
        volumeFilter.addEventListener("change", function () {
            renderLpcpBottomCharts();
        });
    }

    var loadFilter = byId("loadFilterSelect");
    if (loadFilter) {
        loadFilter.addEventListener("change", function () {
            renderLpcpBottomCharts();
        });
    }

    // v2.3.60 — Tải stats LPCP cho đúng tháng đang hiển thị trên lịch
    function loadLpcpStatsForMonth(baseDate) {
        renderLpcpBottomCharts();
    }

    // Monthly calendar navigation — fetch data when month changes (Issue 2)
    function reloadCalendarForMonth() {
        var calNode = byId("chartActivityCalendarMonthly");
        if (calNode) calNode.innerHTML = '<div class="dk-skeleton"><div class="dk-skeleton-shimmer"></div></div>';

        if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
        // Fetch a 3-month window centered on displayed month
        var from = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
        var to = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 2, 0);
        var url =
            "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);

        // v2.3.60 — Reload LPCP calendar + stats cho tháng mới
        var urlLPCP =
            "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" +
            asIsoDate(from) +
            "&denNgay=" +
            asIsoDate(to);
        var urlTrendLich =
            "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
        var urlNKDK =
            "/api/DashboardKhoDesktop/GetNKDuKienByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);

        state.lpcpCalendar = {};

        Promise.all([
            requestJson(url).catch(function () {
                return [];
            }),
            requestJson(urlLPCP).catch(function () {
                return [];
            }),
            requestJson(urlTrendLich).catch(function () {
                return [];
            }),
            requestJson(urlNKDK).catch(function () {
                return [];
            }),
        ]).then(function (results) {
            state.activityCalendar = normalizeArray(results[0]);
            var lpcpArr = normalizeArray(results[1] && results[1].data ? results[1].data : results[1]);
            lpcpArr.forEach(function (d) {
                var k = String(d.NgayLam || d.ngayLam || "").substring(0, 10);
                if (k) state.lpcpCalendar[k] = d;
            });
            state.flowTrendByRange = normalizeArray(results[2]);
            state.nkDuKien = normalizeArray(results[3]);

            renderActivityCalendarMonthly();
            loadLpcpStatsForMonth(calMonthDate);
        });
    }

    var calPrev = byId("calPrevMonth");
    if (calPrev) {
        calPrev.addEventListener("click", function () {
            if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
            calRangeFrom = null;
            calRangeTo = null;
            calMonthDate = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() - 1, 1);
            reloadCalendarForMonth();
        });
    }
    var calNext = byId("calNextMonth");
    if (calNext) {
        calNext.addEventListener("click", function () {
            if (!calMonthDate) calMonthDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
            calRangeFrom = null;
            calRangeTo = null;
            calMonthDate = new Date(calMonthDate.getFullYear(), calMonthDate.getMonth() + 1, 1);
            reloadCalendarForMonth();
        });
    }
    var calApply = byId("calApplyRange");
    if (calApply) {
        calApply.addEventListener("click", function () {
            var fromEl = byId("calFromDate");
            var toEl = byId("calToDate");
            if (fromEl && fromEl.value && toEl && toEl.value) {
                var calNode = byId("chartActivityCalendarMonthly");
                if (calNode)
                    calNode.innerHTML = '<div class="dk-skeleton"><div class="dk-skeleton-shimmer"></div></div>';

                calRangeFrom = new Date(fromEl.value + "T00:00:00");
                calRangeTo = new Date(toEl.value + "T00:00:00");
                calMonthDate = new Date(calRangeFrom.getFullYear(), calRangeFrom.getMonth(), 1);

                var urlLichGoc =
                    "/api/DashboardKhoDesktop/GetActivityCalendar?tuNgay=" +
                    asIsoDate(calRangeFrom) +
                    "&denNgay=" +
                    asIsoDate(calRangeTo);
                var urlNKDK =
                    "/api/DashboardKhoDesktop/GetNKDuKienByRange?tuNgay=" +
                    asIsoDate(calRangeFrom) +
                    "&denNgay=" +
                    asIsoDate(calRangeTo);
                var urlLPCP =
                    "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" +
                    asIsoDate(calRangeFrom) +
                    "&denNgay=" +
                    asIsoDate(calRangeTo);

                Promise.all([
                    requestJson(urlNKDK).catch(function () {
                        return [];
                    }),
                    requestJson(urlLichGoc).catch(function () {
                        return [];
                    }),
                    requestJson(urlLPCP).catch(function () {
                        return [];
                    }),
                ])
                    .then(function (results) {
                        state.nkDuKien = normalizeArray(results[0]);

                        var rLPCP = results[2] || {};
                        var lpcpArr = normalizeArray(rLPCP.Tasks || rLPCP.data || rLPCP);
                        var invArr = normalizeArray(rLPCP.Inventory || []);

                        state.lpcpCalendar = {};
                        lpcpArr.forEach(function (d) {
                            var k = String(d.NgayLam || d.ngayLam || "").substring(0, 10);
                            if (k) state.lpcpCalendar[k] = d;
                        });

                        if (invArr.length > 0) {
                            state.activityCalendar = invArr.map(function (item) {
                                var inQty = toNumber(
                                    item.SoLuongNhapKho ||
                                    item.SoLuongNhap ||
                                    item.TotalIn ||
                                    item.totalIn ||
                                    item.SLNhap ||
                                    0,
                                );
                                var outQty = toNumber(
                                    item.SoLuongXuatHang ||
                                    item.SoLuongXuat ||
                                    item.TotalOut ||
                                    item.totalOut ||
                                    item.SLXuat ||
                                    0,
                                );
                                var kkQty = toNumber(
                                    item.SoLuongKiemKe ||
                                    item.SoLuongKK ||
                                    item.TotalKiemKe ||
                                    item.totalKiemKe ||
                                    item.SLKiemKe ||
                                    0,
                                );
                                return {
                                    NgayHoatDong: item.Ngay || item.ngay || item.NgayHoatDong,
                                    TotalIn: inQty,
                                    TotalOut: outQty,
                                    TotalKiemKe: kkQty,
                                    TotalActivity: inQty + outQty + kkQty,
                                };
                            });
                        } else {
                            state.activityCalendar = normalizeArray(results[1]);
                        }

                        renderActivityCalendarMonthly();
                    })
                    .catch(function () {
                        renderActivityCalendarMonthly();
                    });
            }
        });
    }

    // v2.3.46 — "Tổng quát" button: mở modal aggregate cho toàn bộ range
    var calOverview = byId("calOpenOverview");
    if (calOverview) {
        calOverview.addEventListener("click", function () {
            openCalendarOverviewModal();
        });
    }

    // v2.3.46 — Global Itemcode search
    var gsInput = byId("globalKhoSearch");
    var gsBtn = byId("globalKhoSearchBtn");
    var gsClear = byId("globalKhoSearchClear");
    var gsResult = byId("globalKhoSearchResult");
    var gsDebounceTimer = null;
    function runGlobalSearch(immediate) {
        if (!gsInput || !gsResult) return;
        var code = (gsInput.value || "").trim();
        if (!code) {
            gsResult.style.display = "none";
            gsResult.innerHTML = "";
            return;
        }
        // debounce
        if (!immediate) {
            if (gsDebounceTimer) clearTimeout(gsDebounceTimer);
            gsDebounceTimer = setTimeout(function () {
                runGlobalSearch(true);
            }, 300);
            return;
        }
        gsResult.style.display = "block";
        gsResult.innerHTML = '<div class="dk-empty" style="padding:14px">Đang tìm "' + escapeHtml(code) + '" ...</div>';
        if (gsBtn) gsBtn.disabled = true;
        requestJson("/api/DashboardKhoDesktop/GlobalSearch?itemcode=" + encodeURIComponent(code))
            .then(function (data) {
                var arr = normalizeArray(data);
                renderGlobalSearchResult(gsResult, code, arr[0] || null);
            })
            .catch(function (err) {
                gsResult.innerHTML =
                    '<div class="dk-empty dk-text-danger" style="padding:14px">Lỗi: ' +
                    escapeHtml(String((err && err.message) || err)) +
                    "</div>";
            })
            .finally(function () {
                if (gsBtn) gsBtn.disabled = false;
            });
    }
    if (gsBtn)
        gsBtn.addEventListener("click", function () {
            runGlobalSearch(true);
        });
    if (gsInput) {
        gsInput.addEventListener("input", function () {
            runGlobalSearch(false);
        });
        gsInput.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                runGlobalSearch(true);
            }
        });
    }
    if (gsClear) {
        gsClear.addEventListener("click", function () {
            if (gsInput) {
                gsInput.value = "";
                gsInput.focus();
            }
            if (gsResult) {
                gsResult.style.display = "none";
                gsResult.innerHTML = "";
            }
        });
    }

    // v2.3.33 — Legend toggle filter (click để bật/tắt hiển thị từng loại)
    var calLegend = byId("calActLegend");
    if (calLegend) {
        calLegend.addEventListener("click", function (e) {
            var btn = e.target.closest ? e.target.closest(".dk-cal-leg-toggle") : null;
            if (!btn) {
                var t = e.target;
                while (t && t !== calLegend) {
                    if (t.classList && t.classList.contains("dk-cal-leg-toggle")) {
                        btn = t;
                        break;
                    }
                    t = t.parentNode;
                }
            }
            if (!btn) return;
            var act = btn.getAttribute("data-act");
            if (!act) return;
            // Toggle filter state
            state.calActFilter[act] = !state.calActFilter[act];
            btn.classList.toggle("is-active", state.calActFilter[act]);
            // Re-render calendar
            renderActivityCalendarMonthly();
        });
    }

    // Modal search filter
    var searchInput = byId("detailSearchInput");
    if (searchInput) {
        searchInput.addEventListener("input", function () {
            filterDetailTable(this.value.trim());
        });
    }
}

function bindSidebarToggle() {
    var sidebar = byId("dkSidebar");
    if (!sidebar) return;
    function doToggle() {
        sidebar.classList.toggle("collapsed");
        try {
            localStorage.setItem("dkSidebarCollapsed", sidebar.classList.contains("collapsed") ? "1" : "0");
        } catch (e) { }
    }
    var toggle = byId("sidebarToggle");
    if (toggle) toggle.addEventListener("click", doToggle);
    var topToggle = byId("sidebarToggleTop");
    if (topToggle) topToggle.addEventListener("click", doToggle);

    var searchBox = document.querySelector(".dk-sb-search-box");
    if (searchBox) {
        searchBox.addEventListener("click", function () {
            if (sidebar.classList.contains("collapsed")) {
                sidebar.classList.remove("collapsed");
                try {
                    localStorage.setItem("dkSidebarCollapsed", "0");
                } catch (e) { }
                var inp = byId("globalKhoSearch");
                if (inp)
                    setTimeout(function () {
                        inp.focus();
                    }, 280);
            }
        });
    }
    try {
        if (localStorage.getItem("dkSidebarCollapsed") === "1") {
            sidebar.classList.add("collapsed");
        }
    } catch (e) { }
}

function bindFlowRangePicker() {
    var fromEl = byId("flowFromDate");
    var toEl = byId("flowToDate");
    var applyBtn = byId("flowApplyRange");
    if (!fromEl || !toEl || !applyBtn) return;

    // Default range: 30 days back → today
    var today = new Date();
    var startDefault = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
    fromEl.value = asIsoDate(startDefault);
    toEl.value = asIsoDate(today);

    applyBtn.addEventListener("click", function () {
        if (!fromEl.value || !toEl.value) return;
        var from = new Date(fromEl.value + "T00:00:00");
        var to = new Date(toEl.value + "T00:00:00");
        if (isNaN(from.getTime()) || isNaN(to.getTime())) return;
        if (from > to) {
            var tmp = from;
            from = to;
            to = tmp;
        }
        flowRangeFrom = from;
        flowRangeTo = to;
        loadAndRenderFlowByRange(from, to);
    });
}

function bindPeriodSelector() {
    bindFlowRangePicker();
}

function bindThemeToggle() {
    var btn = byId("btnThemeToggle");
    var btnTop = byId("btnThemeToggleTop");

    function doThemeToggle(e) {
        if (e) e.preventDefault();
        var mainContainer = document.getElementById("dkMain");
        var scrollY = mainContainer ? mainContainer.scrollTop : window.scrollY || document.documentElement.scrollTop;

        var isDark = document.body.classList.toggle("dark-theme");
        try {
            localStorage.setItem("dkTheme", isDark ? "dark" : "light");
        } catch (err) { }
        try {
            applyHighchartsTheme();
        } catch (err) { }

        if (mainContainer) {
            mainContainer.scrollTop = scrollY;
        } else {
            window.scrollTo(0, scrollY);
        }

        setTimeout(function () {
            if (mainContainer) {
                mainContainer.scrollTop = scrollY;
            } else {
                window.scrollTo(0, scrollY);
            }
        }, 50);
    }

    if (btn) btn.addEventListener("click", doThemeToggle);
    if (btnTop) btnTop.addEventListener("click", doThemeToggle);
}

function bindDateFilter() {
    var fromEl = byId("dashFromDate");
    var toEl = byId("dashToDate");
    var btnEl = byId("dashApplyFilter");
    if (fromEl) fromEl.value = state.dateFilter.from;
    if (toEl) toEl.value = state.dateFilter.to;
    if (btnEl) {
        btnEl.addEventListener("click", function () {
            var f = fromEl ? fromEl.value : "";
            var t = toEl ? toEl.value : "";
            if (!f || !t) {
                showToast("Vui lòng nhập đủ Từ ngày và Đến ngày", "warn");
                return;
            }
            if (f > t) {
                showToast("Từ ngày phải <= Đến ngày", "warn");
                return;
            }
            state.dateFilter.from = f;
            state.dateFilter.to = t;
            state.loading = false;
            loadedPages = { 1: false, 2: false, 3: false };
            showToast("Đang áp dụng filter " + f + " → " + t + "...", "info");
            triggerSequentialReload();
        });
    }
    // v2.5.0 — Quick range chip buttons
    var chips = document.querySelectorAll(".dk-topbar-chip");
    for (var ci = 0; ci < chips.length; ci++) {
        (function (chip) {
            chip.addEventListener("click", function () {
                // Clear active on all chips
                for (var x = 0; x < chips.length; x++) chips[x].classList.remove("active");
                chip.classList.add("active");
                var range = chip.getAttribute("data-range");
                var now = new Date();
                var toDate = now.toISOString().slice(0, 10);
                var fromDate;
                if (range === "mtd") {
                    fromDate = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0, 10);
                } else {
                    var days = parseInt(range, 10);
                    var d = new Date(now);
                    d.setDate(d.getDate() - days + 1);
                    fromDate = d.toISOString().slice(0, 10);
                }
                if (fromEl) fromEl.value = fromDate;
                if (toEl) toEl.value = toDate;
                state.dateFilter.from = fromDate;
                state.dateFilter.to = toDate;
                state.loading = false;
                loadedPages = { 1: false, 2: false, 3: false };
                showToast("Đang tải dữ liệu " + fromDate + " → " + toDate + "...", "info");
                triggerSequentialReload();
            });
        })(chips[ci]);
    }
    // v2.5.0 — Topbar refresh button
    var btnRefreshTop = byId("btnRefreshTop");
    if (btnRefreshTop) {
        btnRefreshTop.addEventListener("click", function () {
            sessionStorage.setItem("dk_restore_tab", currentPage);
            requestJson("/api/DashboardKhoDesktop/ClearCache")
                .then(function () {
                    window.location.reload();
                })
                .catch(function () {
                    window.location.reload();
                });
        });
    }
}

function bindTodoPOHandlers() {
    document.addEventListener("click", function (e) {
        var tog = e.target.closest ? e.target.closest("[data-po-toggle]") : null;
        if (tog) {
            e.preventDefault();
            var po = tog.getAttribute("data-po-toggle");
            var rows = document.querySelectorAll('[data-po-sub="' + po.replace(/"/g, '\\"') + '"]');
            var isHidden = tog.classList.toggle("dk-collapsed");
            var icon = tog.querySelector("i");
            if (icon) icon.className = isHidden ? "fa-solid fa-chevron-right" : "fa-solid fa-chevron-down";
            for (var i = 0; i < rows.length; i++) rows[i].style.display = isHidden ? "none" : "";
            return;
        }
        var lnk = e.target.closest ? e.target.closest("[data-po-link]") : null;
        if (lnk) {
            e.preventDefault();
            showToast("Chi tiết PO sẽ triển khai sau (" + lnk.getAttribute("data-po-link") + ")", "info");
            return;
        }
    });
}

function bindGlobalSearchAll() {
    var searchInput = document.getElementById("dkGlobalSearch");
    var searchDropdown = document.getElementById("dkSearchDropdown");
    var searchResults = document.getElementById("dkSearchResults");

    if (!searchInput || !searchDropdown || !searchResults) return;

    var dashboardSections = [
        { label: "Tồn đầu kỳ", icon: "fa-warehouse", elementId: "metricTonDauKy", page: 1 },
        { label: "Tổng nhập", icon: "fa-cloud-arrow-down", elementId: "metricTongNhap", page: 1 },
        { label: "Tổng xuất", icon: "fa-truck-fast", elementId: "metricTongXuat", page: 1 },
        { label: "Tồn kho", icon: "fa-cube", elementId: "metricTonKho", page: 1 },
        { label: "PO chuẩn bị về", icon: "fa-clipboard-list", elementId: "metricInboundReady", page: 1 },
        { label: "PO đã về kho", icon: "fa-truck-ramp-box", elementId: "metricPODangTre", page: 1 },
        { label: "Giá trị tồn kho", icon: "fa-sack-dollar", elementId: "metricThanhGia", page: 1 },
        { label: "Biểu đồ lấp đầy kho", icon: "fa-chart-pie", elementId: "chartCapacityRing", page: 1 },
        { label: "Tỷ trọng khách hàng theo CBM", icon: "fa-users", elementId: "chartCustomerPie", page: 1 },
        { label: "Công việc chờ xử lý", icon: "fa-clipboard-check", elementId: "todoList", page: 1 },
        { label: "Lấp đầy theo loại kho", icon: "fa-chart-bar", elementId: "chartCapacityBar", page: 1 },
        { label: "Tình hình kiểm kê", icon: "fa-check-double", elementId: "kiemKeBox", page: 1 },
        { label: "Giá trị tồn kho theo nhóm", icon: "fa-chart-pie", elementId: "chartGiaTriTheoNhom", page: 1 },
        { label: "Top 5 vật tư chiếm dung tích", icon: "fa-ranking-star", elementId: "chartCapacityBar", page: 1 },
        { label: "Top 5 khách hàng giá trị tồn", icon: "fa-building", elementId: "chartCustomerPie", page: 1 },
        { label: "Biểu đồ xuất nhập tồn", icon: "fa-chart-line", elementId: "chartFlowTrend", page: 2 },
        { label: "Top kệ sử dụng cao", icon: "fa-layer-group", elementId: "chartFlowTrend", page: 2 },
        { label: "Top 5 NL tồn kho nhiều nhất", icon: "fa-boxes-stacked", elementId: "chartTop5MaxNL", page: 2 },
        { label: "Top 5 PL tồn kho nhiều nhất", icon: "fa-boxes-stacked", elementId: "chartTop5MaxPL", page: 2 },
        { label: "Cảnh báo tồn kho", icon: "fa-triangle-exclamation", elementId: "alertsList", page: 2 },
        { label: "Tuổi tồn kho theo nhóm vật tư", icon: "fa-hourglass-half", elementId: "chartAgeStock", page: 2 },
        { label: "Hiệu suất hoạt động", icon: "fa-gauge-high", elementId: "chartVolumePie", page: 2 },
        { label: "Vật tư sắp hết hạn", icon: "fa-clock", elementId: "chartAgeStock", page: 2 },
        { label: "Lịch hoạt động kho", icon: "fa-calendar-days", elementId: "chartActivityCalendarMonthly", page: 3 },
    ];

    function highlightElement(el) {
        // Scroll to element
        el.scrollIntoView({ behavior: "smooth", block: "center" });

        // Add highlight class with strong glow
        el.classList.add("dk-nav-highlight");

        // Remove after animation completes
        setTimeout(function () {
            el.classList.remove("dk-nav-highlight");
        }, 3500);
    }

    function navigateToSection(section) {
        var targetPage = section.page || 1;
        // Use switchPage directly
        if (typeof switchPage === "function") {
            switchPage(targetPage);
        }
        setTimeout(function () {
            var panel = document.getElementById(section.elementId);
            if (panel) {
                var target = panel.closest
                    ? panel.closest(".dk-panel") || panel.closest(".dk-metric-card") || panel
                    : panel;
                highlightElement(target);
            }
        }, 500);
    }

    function filterSections(query) {
        var q = query.toLowerCase();
        var words = q.split(/\s+/);
        return dashboardSections.filter(function (s) {
            var lbl = s.label.toLowerCase();
            return words.every(function (w) {
                return lbl.indexOf(w) >= 0;
            });
        });
    }

    var timer;
    searchInput.addEventListener("input", function () {
        clearTimeout(timer);
        var query = this.value.trim();
        if (!query || query.length < 1) {
            searchDropdown.style.display = "none";
            return;
        }

        var sectionMatches = filterSections(query);
        var html = "";

        if (sectionMatches.length > 0) {
            html +=
                '<div class="dk-search-group-title"><i class="fa-solid fa-compass" style="margin-right:5px"></i>Mục Dashboard</div>';
            html += '<ul class="dk-search-list">';
            sectionMatches.forEach(function (s) {
                html +=
                    '<li class="dk-search-item dk-search-section" data-section-id="' +
                    s.elementId +
                    '" data-section-page="' +
                    s.page +
                    '">';
                html +=
                    '  <div class="dk-search-item-title"><i class="fa-solid ' +
                    s.icon +
                    '" style="margin-right:6px;opacity:0.7;color:#60a5fa"></i>' +
                    escapeHtml(s.label) +
                    "</div>";
                html +=
                    '  <div class="dk-search-item-desc"><i class="fa-solid fa-location-dot" style="margin-right:4px;font-size:10px"></i>Trang ' +
                    s.page +
                    " — Nhấn để đi tới mục này</div>";
                html += "</li>";
            });
            html += "</ul>";
        }

        searchResults.innerHTML = html;
        if (html) searchDropdown.style.display = "block";

        if (query.length >= 2) {
            timer = setTimeout(function () {
                requestJson("/api/DashboardKhoDesktop/GlobalSearchAll?keyword=" + encodeURIComponent(query))
                    .then(function (res) {
                        var arr = normalizeArray(res);
                        var apiHtml = "";
                        if (arr && arr.length > 0) {
                            var grouped = {};
                            arr.forEach(function (item) {
                                var cat = item.Category || "OTHER";
                                if (!grouped[cat]) grouped[cat] = [];
                                grouped[cat].push(item);
                            });
                            var catLabels = { PO: "PO & Đơn hàng", ITEM_RACK: "Vị trí hàng" };
                            var catTypes = { PO: "po", ITEM_RACK: "rack" };
                            Object.keys(grouped).forEach(function (cat) {
                                apiHtml +=
                                    '<div class="dk-search-group-title"><i class="fa-solid ' +
                                    (cat === "PO" ? "fa-file-invoice" : "fa-map-pin") +
                                    '" style="margin-right:5px"></i>' +
                                    escapeHtml(catLabels[cat] || cat) +
                                    "</div>";
                                apiHtml += '<ul class="dk-search-list">';
                                grouped[cat].forEach(function (item) {
                                    var type = catTypes[cat] || "other";
                                    apiHtml +=
                                        '<li class="dk-search-item" data-type="' +
                                        type +
                                        '" data-id="' +
                                        escapeHtml(item.TargetID || "") +
                                        '" data-title="' +
                                        escapeHtml(item.Title || "") +
                                        '">';
                                    apiHtml +=
                                        '  <div class="dk-search-item-title">' +
                                        escapeHtml(item.Title || "") +
                                        "</div>";
                                    apiHtml +=
                                        '  <div class="dk-search-item-desc">' +
                                        escapeHtml(item.Subtitle || "") +
                                        "</div>";
                                    apiHtml += "</li>";
                                });
                                apiHtml += "</ul>";
                            });
                        }
                        var sectionBlock = searchResults.querySelector(".dk-search-group-title");
                        var sectionList = searchResults.querySelector(".dk-search-list");
                        var keepHtml = "";
                        if (sectionBlock) keepHtml += sectionBlock.outerHTML;
                        if (sectionList) keepHtml += sectionList.outerHTML;
                        searchResults.innerHTML = keepHtml + apiHtml;
                        searchDropdown.style.display = "block";
                    })
                    .catch(function () { });
            }, 300);
        }
    });

    document.addEventListener("click", function (e) {
        if (!searchInput.contains(e.target) && !searchDropdown.contains(e.target)) {
            searchDropdown.style.display = "none";
        }
    });

    searchResults.addEventListener("click", function (e) {
        var itemEl = e.target.closest ? e.target.closest(".dk-search-item") : null;
        if (!itemEl) return;

        if (itemEl.classList.contains("dk-search-section")) {
            var sectionId = itemEl.getAttribute("data-section-id");
            var sectionPage = parseInt(itemEl.getAttribute("data-section-page") || "1");
            searchDropdown.style.display = "none";
            searchInput.value = "";
            var matchedSection = dashboardSections.filter(function (s) {
                return s.elementId === sectionId && s.page === sectionPage;
            })[0];
            if (matchedSection) navigateToSection(matchedSection);
            return;
        }

        var type = itemEl.getAttribute("data-type");
        var id = itemEl.getAttribute("data-id");
        var title = itemEl.getAttribute("data-title");

        searchDropdown.style.display = "none";
        searchInput.value = title;

        if (type === "po") {
            requestJson(
                "/api/DashboardKhoDesktop/GetChuanBiVe?tuNgay=2000-01-01&denNgay=2099-12-31&keyword=" +
                encodeURIComponent(id),
            ).then(function (res) {
                var newItems = normalizeArray(res);
                if (newItems && newItems.length > 0) {
                    newItems.forEach(function (ni) {
                        var exists = state.inbound.some(function (x) {
                            return x.SoLo === ni.SoLo;
                        });
                        if (!exists) state.inbound.unshift(ni);
                    });
                }
                if (typeof openDetail === "function") {
                    openDetail("inboundReady", 0);
                    setTimeout(function () {
                        var dInput = document.getElementById("detailSearchInput");
                        if (dInput && typeof filterDetailTable === "function") {
                            dInput.value = title;
                            filterDetailTable(title);
                        }
                    }, 400);
                }
            });
        } else if (type === "rack") {
            if (typeof openDetail === "function") {
                openDetail("top5VTAll", 0);
                setTimeout(function () {
                    var dInput = document.getElementById("detailSearchInput");
                    if (dInput && typeof filterDetailTable === "function") {
                        dInput.value = title;
                        filterDetailTable(title);
                    }
                }, 800);
            }
        }
    });
}

function initFlatpickr() {
    if (typeof flatpickr !== "undefined") {
        function bindPair(fromSelector, toSelector) {
            var fromEl = document.querySelector(fromSelector);
            var toEl = document.querySelector(toSelector);
            if (!fromEl || !toEl) return;

            var fpTo = flatpickr(toEl, {
                dateFormat: "Y-m-d",
                altInput: true,
                altFormat: "d-m-Y",
                altInputClass: toEl.className,
                locale: "vn",
                defaultDate: toEl.value,
                onChange: function (selectedDates, dateStr) {
                    if (fpFrom) fpFrom.set("maxDate", dateStr);
                },
            });
            var fpFrom = flatpickr(fromEl, {
                dateFormat: "Y-m-d",
                altInput: true,
                altFormat: "d-m-Y",
                altInputClass: fromEl.className,
                locale: "vn",
                defaultDate: fromEl.value,
                onChange: function (selectedDates, dateStr) {
                    if (fpTo) fpTo.set("minDate", dateStr);
                },
            });

            if (fromEl.value) fpTo.set("minDate", fromEl.value);
            if (toEl.value) fpFrom.set("maxDate", toEl.value);
        }

        bindPair("#dashFromDate", "#dashToDate");
        bindPair("#flowFromDate", "#flowToDate");
        bindPair("#calFromDate", "#calToDate");
    }
}

/**
 * Hàm khởi chạy đầu tiên khi tải trang. Nạp dữ liệu và khởi tạo giao diện.
 */
function init() {
    var restoreTab = sessionStorage.getItem("dk_restore_tab");
    if (restoreTab) {
        sessionStorage.removeItem("dk_restore_tab");

        // Defeat browser form auto-fill
        var inputsToClear = ["flowFromDate", "flowToDate", "calFromDate", "calToDate", "searchAll"];
        for (var i = 0; i < inputsToClear.length; i++) {
            var el = document.getElementById(inputsToClear[i]);
            if (el) el.value = "";
        }
        var selectsToClear = ["customerFilterSelect", "trendFilterSelect", "loadFilterSelect", "volumeFilterSelect"];
        for (var j = 0; j < selectsToClear.length; j++) {
            var selEl = document.getElementById(selectsToClear[j]);
            if (selEl && selEl.options && selEl.options.length > 0) {
                selEl.value = selEl.options[0].value;
            }
        }

        var chksToClear = ["chkActIn", "chkActOut", "chkActKK", "chkActPlan"];
        for (var k = 0; k < chksToClear.length; k++) {
            var chkEl = document.getElementById(chksToClear[k]);
            if (chkEl) chkEl.checked = true;
        }

        var tabId = parseInt(restoreTab, 10);
        if (!isNaN(tabId) && tabId >= 1 && tabId <= 3) {
            currentPage = tabId;
        }
    }

    bindEvents();
    bindPageNav();
    bindSidebarToggle();
    bindThemeToggle();
    bindPeriodSelector();
    bindDateFilter();
    bindTodoPOHandlers();
    bindGlobalSearchAll();
    renderClockNow();
    applyHighchartsTheme();
    initFlatpickr();
    setTimeout(injectMaximizeButtons, 50);

    switchPage(currentPage || 1);
    loadData();
    window.setInterval(renderClockNow, 1000);

    window.setInterval(function () {
        loadData();
    }, 300000);
}

init();
//#endregion