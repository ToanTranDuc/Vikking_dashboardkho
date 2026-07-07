/**
 * @file dashboard-kho-utils.js
 * @description Chứa các hàm tiện ích dùng chung (Format số, xử lý ngày tháng, hiển thị Toast).
 * @version 2.7.26
 */

//#region CONFIG & UTILS
window.DK_DETAIL_PAGE_SIZE = 200;

var ids = {
    currentDate: "currentDate",
    currentTime: "currentTime",
    chartCapacityRing: "chartCapacityRing",
    chartCapacityLegend: "chartCapacityLegend",
    chartCustomerPie: "chartCustomerPie",
    chartFlowTrend: "chartFlowTrend",
    chartTop5MaxNL: "chartTop5MaxNL",
    chartTop5MaxPL: "chartTop5MaxPL",
    chartAgeStock: "chartAgeStock",
    customersBody: "customersBody",
    racksBody: "racksBody",
    detailModal: "detailModal",
    detailModalTitle: "detailModalTitle",
    detailModalMeta: "detailModalMeta",
    detailModalContent: "detailModalContent",
};

/**
 * Lấy phần tử HTML theo ID (viết tắt của document.getElementById).
 */
function byId(id) {
    return document.getElementById(id);
}

/**
 * Chuyển đổi một giá trị bất kỳ thành số thực (Float). Trả về 0 nếu giá trị không hợp lệ.
 */
function toNumber(value) {
    if (value === null || value === undefined || value === "") return 0;
    var number = Number(value);
    return isNaN(number) ? 0 : number;
}

/**
 * Định dạng số thành chuỗi hiển thị theo chuẩn Việt Nam (vd: 1.000.000).
 * @param {any} value Số cần định dạng
 * @param {number} fractionDigits Số chữ số thập phân
 * @returns {string} Chuỗi số đã định dạng
 */
var __numFormatCache = {};
/**
 * Định dạng số theo chuẩn phân cách hàng nghìn (ví dụ: 1,000,000).
 */
function formatNumber(value, fractionDigits, forceFractionDigits) {
    var maxDigits = typeof fractionDigits === "number" ? fractionDigits : 0;
    var minDigits = forceFractionDigits === true ? maxDigits : 0;
    var cacheKey = minDigits + "_" + maxDigits;
    if (!__numFormatCache[cacheKey]) {
        __numFormatCache[cacheKey] = new Intl.NumberFormat("vi-VN", {
            minimumFractionDigits: minDigits,
            maximumFractionDigits: maxDigits,
        });
    }
    return __numFormatCache[cacheKey].format(toNumber(value));
}

/**
 * Định dạng số thập phân thành phần trăm (%) kèm dấu %.
 */
function formatPercent(value) {
    return formatNumber(value, 2) + "%";
}

/**
 * Phân tích chuỗi ngày tháng từ server (hoặc chuỗi ISO) thành đối tượng Date của Javascript.
 */
function parseDate(raw) {
    if (!raw) return null;
    if (Object.prototype.toString.call(raw) === "[object Date]") return raw;

    if (typeof raw === "string") {
        var match = /\/Date\((\d+)\)\//.exec(raw);
        if (match && match[1]) {
            return new Date(parseInt(match[1], 10));
        }

        var parsed = new Date(raw);
        if (!isNaN(parsed.getTime())) return parsed;
    }

    return null;
}

/**
 * Định dạng ngày thành chuỗi DD/MM (Ngày/Tháng).
 */
function formatDateShort(d) {
    if (!d) return "";
    if (!(d instanceof Date)) d = parseDate(d);
    if (!d) return "";
    return (
        String(d.getDate()).padStart(2, "0") + "/" + String(d.getMonth() + 1).padStart(2, "0") + "/" + d.getFullYear()
    );
}

/**
 * Định dạng ngày thành chuỗi DD/MM/YYYY.
 */
function formatDate(raw) {
    var date = parseDate(raw);
    if (!date) return "";
    return date.toLocaleDateString("vi-VN");
}

/**
 * Định dạng ngày và giờ thành chuỗi DD/MM/YYYY HH:mm.
 */
function formatDateTime(date) {
    if (!date) return "--:--:--";
    return date.toLocaleTimeString("vi-VN", { hour12: false });
}

/**
 * Hiển thị đồng hồ thời gian thực (Giờ:Phút:Giây) lên Topbar của giao diện.
 */
function renderClockNow() {
    var now = new Date();
    var timeNode = byId(ids.currentTime);
    var dateNode = byId(ids.currentDate);
    if (timeNode) timeNode.textContent = now.toLocaleTimeString("vi-VN", { hour12: false });
    if (dateNode)
        dateNode.textContent = now.toLocaleDateString("vi-VN", {
            weekday: "long",
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
        });
    // v2.5.0 — Also update topbar clock
    var tbTime = byId("topbarTime");
    var tbDate = byId("topbarDate");
    if (tbTime) tbTime.textContent = now.toLocaleTimeString("vi-VN", { hour12: false });
    if (tbDate)
        tbDate.textContent = now.toLocaleDateString("vi-VN", {
            weekday: "short",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });
}

/**
 * Định dạng ngày tháng rút gọn (bỏ số năm) để tiết kiệm diện tích trên biểu đồ.
 */
function formatCompactDate(raw) {
    var date = parseDate(raw);
    if (!date) return "";
    var day = String(date.getDate()).padStart(2, "0");
    var month = String(date.getMonth() + 1).padStart(2, "0");
    return day + "/" + month;
}

/**
 * Mã hóa (Encode) các ký tự HTML đặc biệt để phòng chống tấn công XSS (Cross-Site Scripting).
 */
function escapeHtml(value) {
    var text = String(value === null || value === undefined ? "" : value);
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}

/**
 * Chuẩn hóa định dạng Mã Lệnh Sản Xuất để đồng bộ hiển thị.
 */
function formatMaLenhSX(maLenh) {
    if (!maLenh) return "-";
    if (maLenh.indexOf("|") !== -1 || maLenh.length > 30) return "";
    return escapeHtml(maLenh);
}

/**
 * Rút gọn Mã Lệnh nếu quá dài, thêm dấu ... để tránh vỡ giao diện.
 */
function shortMaLenh(value) {
    if (!value) return "";
    var str = String(value).trim();
    var idx = str.indexOf("|");
    return idx >= 0 ? str.substring(0, idx).trim() : str;
}

/**
 * Chuẩn hóa tên khách hàng (cắt khoảng trắng, xử lý lỗi font nếu có).
 */
function normalizeCustomerName(value) {
    var name = value === null || value === undefined ? "" : String(value).trim();
    return name ? name : "Khách trống";
}

/**
 * Đảm bảo dữ liệu trả về từ API luôn là một mảng. Nếu null thì trả về mảng rỗng [].
 */
function normalizeArray(data) {
    if (!data) return [];
    if (Array.isArray(data)) return data;
    if (data.d && Array.isArray(data.d)) return data.d;
    if (data.Data && Array.isArray(data.Data)) return data.Data;
    return [];
}

var __requestQueue = [];
var __activeRequests = 0;
var __maxConcurrent = 4; // Max 4 concurrent API calls to prevent network bottleneck

/**
 * Xử lý hàng đợi các request API (Queue) để tránh quá tải server khi gọi liên tục.
 */
function __processRequestQueue() {
    if (__activeRequests >= __maxConcurrent || __requestQueue.length === 0) return;
    var req = __requestQueue.shift();
    __activeRequests++;
    __requestJsonCore(req.url)
        .then(function (res) {
            req.resolve(res);
        })
        .catch(function (err) {
            req.reject(err);
        })
        .finally(function () {
            __activeRequests--;
            __processRequestQueue();
        });
}

//#endregion


/**
 * Tự động chèn nút Phóng to (Maximize) vào góc phải của các bảng/biểu đồ.
 */
function injectMaximizeButtons() {
    var heads = document.querySelectorAll(".dk-panel-head");
    for (var hi = 0; hi < heads.length; hi++) {
        if (heads[hi].querySelector(".dk-maximize-btn")) continue; // already added
        var btn = document.createElement("button");
        btn.type = "button";
        btn.className = "dk-maximize-btn";
        btn.title = "Mở rộng";
        btn.setAttribute("aria-label", "Mở rộng");
        btn.innerHTML =
            '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" y1="3" x2="14" y2="10"/><line x1="3" y1="21" x2="10" y2="14"/></svg>';
        heads[hi].appendChild(btn);
    }
}

/**
 * Kích hoạt/Hủy chế độ Toàn màn hình (Fullscreen) cho một Panel giao diện.
 */
function togglePanelFullscreen(panel) {
    var isFs = panel.classList.contains("dk-panel-fullscreen");
    var maxBtn = panel.querySelector(".dk-maximize-btn");
    if (isFs) {
        panel.classList.remove("dk-panel-fullscreen");
        if (fsBackdrop && fsBackdrop.parentNode) fsBackdrop.parentNode.removeChild(fsBackdrop);
        fsBackdrop = null;
        if (maxBtn) {
            maxBtn.title = "Mở rộng";
            maxBtn.setAttribute("aria-label", "Mở rộng");
            maxBtn.innerHTML =
                '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" y1="3" x2="14" y2="10"/><line x1="3" y1="21" x2="10" y2="14"/></svg>';
        }
    } else {
        // Remove any existing fullscreen
        var existing = document.querySelector(".dk-panel-fullscreen");
        if (existing) {
            existing.classList.remove("dk-panel-fullscreen");
            var extBtn = existing.querySelector(".dk-maximize-btn");
            if (extBtn) {
                extBtn.title = "Mở rộng";
                extBtn.setAttribute("aria-label", "Mở rộng");
                extBtn.innerHTML =
                    '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" y1="3" x2="14" y2="10"/><line x1="3" y1="21" x2="10" y2="14"/></svg>';
            }
        }

        panel.classList.add("dk-panel-fullscreen");
        fsBackdrop = document.createElement("div");
        fsBackdrop.className = "dk-fs-backdrop";
        fsBackdrop.addEventListener("click", function () {
            togglePanelFullscreen(panel);
        });
        document.body.appendChild(fsBackdrop);
        if (maxBtn) {
            maxBtn.title = "Thu nhỏ";
            maxBtn.setAttribute("aria-label", "Thu nhỏ");
            maxBtn.innerHTML =
                '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><polyline points="4 14 10 14 10 20"/><polyline points="20 10 14 10 14 4"/><line x1="14" y1="10" x2="21" y2="3"/><line x1="10" y1="14" x2="3" y2="21"/></svg>';
        }
    }

    /**
     * Cập nhật lại kích thước (resize) cho toàn bộ biểu đồ Highcharts trên trang.
     */
    function reflowAllHighcharts() {
        if (typeof Highcharts === "undefined") return;
        var charts = Highcharts.charts || [];
        for (var c = 0; c < charts.length; c++) {
            if (!charts[c]) continue;
            try {
                var container = charts[c].renderTo;
                var isFsGeneral = false;
                var targetHeight = 300;
                if (container) {
                    var p = container.closest(".dk-panel");
                    if (p && p.classList.contains("dk-panel-fullscreen")) {
                        isFsGeneral = true;
                        targetHeight = window.innerHeight - 100;
                        container.style.height = targetHeight + "px";
                        if (container.id === "chartVolumePie") {
                            container.style.maxWidth = "calc(100vh + 400px)";
                        }
                    } else {
                        container.style.height = "300px";
                        if (container.id === "chartVolumePie") {
                            container.style.maxWidth = "100%";
                        }
                    }
                }
                var targetWidth = container ? container.clientWidth : null;
                charts[c].setSize(targetWidth, targetHeight, false);
                charts[c].reflow();
                if (container && container.id === "chartVolumePie") {
                    var isFs = !!document.getElementById("chartVolumePie").closest(".dk-panel-fullscreen");
                    charts[c].update(
                        {
                            plotOptions: {
                                pie: {
                                    size: isFs ? "95%" : "80%",
                                    center: isFs ? ["40%", "50%"] : ["35%", "50%"],
                                },
                            },
                            legend: {
                                align: "right",
                                x: 0,
                                itemStyle: {
                                    fontSize: isFs ? "26px" : "13px",
                                },
                                itemMarginTop: isFs ? 10 : 0,
                                itemMarginBottom: isFs ? 10 : 0,
                            },
                        },
                        true,
                    );
                }
            } catch (e) { }
        }
    }

    // Delay reflow để chờ CSS transition
    setTimeout(reflowAllHighcharts, 60);
    setTimeout(reflowAllHighcharts, 260);
    setTimeout(reflowAllHighcharts, 600);
    setTimeout(function () {
        try {
            window.dispatchEvent(new Event("resize"));
        } catch (e) { }
    }, 80);
}

/**
 * Kiểm tra xem một phần tử HTML có chứa class cụ thể hay không.
 */
function hasClass(node, className) {
    if (!node || !node.classList) return false;
    return node.classList.contains(className);
}

/**
 * Tìm kiếm thẻ HTML chứa thông tin cấu hình mở Modal chi tiết (thuộc tính data-detail).
 */
function findDetailNode(node) {
    var current = node;
    while (current && current !== document) {
        if (hasClass(current, "js-open-detail")) return current;
        current = current.parentNode;
    }
    return null;
}

/**
 * Tìm kiếm nút Đóng (Close) của Modal hiện tại.
 */
function findCloseNode(node) {
    var current = node;
    while (current && current !== document) {
        if (hasClass(current, "js-close-modal")) return current;
        current = current.parentNode;
    }
    return null;
}

/**
 * Cộng/trừ số ngày vào một mốc thời gian (Date).
 */
function addDays(baseDate, diffDays) {
    var date = new Date(baseDate.getFullYear(), baseDate.getMonth(), baseDate.getDate());
    date.setDate(date.getDate() + diffDays);
    return date;
}

/**
 * Cộng/trừ số tháng vào một mốc thời gian (Date).
 */
function addMonths(baseDate, diffMonths) {
    return new Date(baseDate.getFullYear(), baseDate.getMonth() + diffMonths, 1);
}

/**
 * Chuyển đổi đối tượng Date thành chuỗi định dạng ISO (YYYY-MM-DD).
 */
function asIsoDate(date) {
    var year = date.getFullYear();
    var month = String(date.getMonth() + 1).padStart(2, "0");
    var day = String(date.getDate()).padStart(2, "0");
    return year + "-" + month + "-" + day;
}

/**
 * Chuẩn hóa khoảng ngày, đảm bảo tuNgay <= denNgay. Trả về obj { from: "...", to: "..." }.
 */
function normalizeDateRange(tuNgay, denNgay) {
    var d1 = typeof tuNgay === "string" ? parseDate(tuNgay) : (tuNgay instanceof Date ? tuNgay : null);
    var d2 = typeof denNgay === "string" ? parseDate(denNgay) : (denNgay instanceof Date ? denNgay : null);
    
    var now = new Date();
    if (!d1) d1 = addDays(now, -30); // Fallback if missing
    if (!d2) d2 = now;               // Fallback if missing

    if (d1 > d2) {
        console.warn("Date range inverted, swapping: from " + asIsoDate(d1) + " to " + asIsoDate(d2));
        var temp = d1;
        d1 = d2;
        d2 = temp;
    }

    return {
        from: asIsoDate(d1),
        to: asIsoDate(d2)
    };
}

/**
 * Xây dựng URL chuẩn với khoảng ngày đã được validate.
 */
function buildUrlWithDateRange(endpoint, tuNgay, denNgay) {
    var range = normalizeDateRange(tuNgay, denNgay);
    var joinChar = endpoint.indexOf("?") !== -1 ? "&" : "?";
    return endpoint + joinChar + "tuNgay=" + encodeURIComponent(range.from) + "&denNgay=" + encodeURIComponent(range.to);
}

/**
 * Lấy khoảng ngày gần đây (VD: 30 ngày)
 */
function getRecentRange(days, baseDate) {
    var end = baseDate ? (typeof baseDate === "string" ? parseDate(baseDate) : baseDate) : new Date();
    if (!end || isNaN(end.getTime())) end = new Date();

    var start = new Date(end);
    start.setDate(start.getDate() - (days - 1)); // -29 ngày để có đủ 30 ngày

    return {
        tuNgay: asIsoDate(start),
        denNgay: asIsoDate(end)
    };
}

/**
 * Chuẩn hóa tham số range của chart
 */
function normalizeChartRange(mode, baseDate) {
    var end = baseDate ? (typeof baseDate === "string" ? parseDate(baseDate) : baseDate) : new Date();
    if (!end || isNaN(end.getTime())) end = new Date();

    var days = 30;
    if (mode === "7days" || mode === 7 || mode === "7") days = 7;
    else if (mode === "14days" || mode === 14 || mode === "14") days = 14;
    else if (mode === "30days" || mode === 30 || mode === "30") days = 30;
    else if (mode === "60days" || mode === 60 || mode === "60") days = 60;
    else if (mode === "today" || mode === 1 || mode === "1") days = 1;

    var start = new Date(end);
    start.setDate(start.getDate() - (days - 1));

    // Đảm bảo tuNgay <= denNgay
    if (start > end) {
        var temp = start;
        start = end;
        end = temp;
    }

    return {
        tuNgay: asIsoDate(start),
        denNgay: asIsoDate(end)
    };
}

/**
 * Tạo hiệu ứng đếm số (nhảy số liên tục) cho các thẻ KPI trên giao diện.
 */
function animateCountUp(scope) {
    var els = (scope || document).querySelectorAll(".dk-count-up:not([data-counted])");
    for (var i = 0; i < els.length; i++) {
        (function (el) {
            el.setAttribute("data-counted", "1");
            var to = toNumber(el.getAttribute("data-count-to"));
            var dur = 800;
            var start = performance.now();
            function tick(now) {
                var t = Math.min(1, (now - start) / dur);
                var ease = 1 - Math.pow(1 - t, 3); // easeOutCubic
                el.textContent = formatNumber(to * ease, 0);
                if (t < 1) requestAnimationFrame(tick);
            }
            requestAnimationFrame(tick);
        })(els[i]);
    }
}

/**
 * Hiển thị thông báo nổi (Toast / Notification) trên màn hình.
 */
function showToast(msg, kind) {
    var cont = byId("dkToastContainer");
    if (!cont) return;
    var el = document.createElement("div");
    el.className = "dk-toast dk-toast-" + (kind || "info");
    el.innerHTML =
        '<i class="fa-solid ' +
        (kind === "success" ? "fa-circle-check" : kind === "error" ? "fa-circle-exclamation" : "fa-circle-info") +
        '"></i><span>' +
        escapeHtml(msg) +
        "</span>";
    cont.appendChild(el);
    setTimeout(function () {
        el.classList.add("show");
    }, 10);
    setTimeout(function () {
        el.classList.remove("show");
        setTimeout(function () {
            if (el.parentNode) el.parentNode.removeChild(el);
        }, 250);
    }, 2500);
}

/**
 * Định dạng số tiền VNĐ dạng rút gọn (ví dụ: 1.5 Tỷ, 500 Tr).
 */
function formatVNDShort(v) {
    var n = toNumber(v);
    if (n >= 1e9) return (n / 1e9).toFixed(2).replace(/\.?0+$/, "") + " tỷ";
    if (n >= 1e6) return (n / 1e6).toFixed(1).replace(/\.0$/, "") + " tr";
    return formatNumber(n, 0);
}