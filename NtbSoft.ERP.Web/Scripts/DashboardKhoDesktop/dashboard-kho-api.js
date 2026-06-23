/**
 * @file dashboard-kho-api.js
 * @description Đảm nhiệm toàn bộ việc giao tiếp với Server (Gọi API nạp dữ liệu). Không chứa logic vẽ giao diện.
 * @version 2.7.26
 */

//#region API FETCHING
/**
 * Hàm gọi API dùng chung (Promise-based), tự động xử lý loading và lỗi mạng.
 * Hàm này đẩy request vào hàng đợi __requestQueue để giới hạn số kết nối song song.
 * @param {string} url Đường dẫn API (ví dụ: "/api/DashboardKhoDesktop/GetRacks")
 * @returns {Promise<any>} Dữ liệu JSON trả về từ server
 */
function requestJson(url) {
    return new Promise(function (resolve, reject) {
        __requestQueue.push({ url: url, resolve: resolve, reject: reject });
        __processRequestQueue();
    });
}

function __requestJsonCore(url) {
    // Luôn luôn băm cache (cache bust) để ngăn chặn trình duyệt cache API GET
    var cacheBustUrl = url + (url.indexOf("?") !== -1 ? "&" : "?") + "_t=" + new Date().getTime();

    if (window.fetch) {
        return window
            .fetch(cacheBustUrl, {
                method: "GET",
                cache: "no-store",
                headers: { Accept: "application/json" },
            })
            .then(function (response) {
                if (!response.ok) {
                    return response.text().then(function (body) {
                        var errMsg = "HTTP " + response.status + " - " + response.statusText;
                        if (body) {
                            try {
                                var j = JSON.parse(body);
                                if (j && j.Message) errMsg += "\n" + j.Message;
                                else errMsg += "\n" + body.substring(0, 500);
                            } catch (e) {
                                errMsg += "\n" + body.substring(0, 500);
                            }
                        }
                        throw new Error(errMsg);
                    });
                }
                return response.json();
            });
    }

    return new Promise(function (resolve, reject) {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", cacheBustUrl, true);
        xhr.setRequestHeader("Accept", "application/json");
        xhr.onreadystatechange = function () {
            if (xhr.readyState !== 4) return;
            if (xhr.status < 200 || xhr.status >= 300) {
                var em = "HTTP " + xhr.status;
                if (xhr.responseText) {
                    try {
                        var jj = JSON.parse(xhr.responseText);
                        if (jj && jj.Message) em += "\n" + jj.Message;
                    } catch (e) {
                        em += "\n" + xhr.responseText.substring(0, 500);
                    }
                }
                reject(new Error(em));
                return;
            }
            try {
                resolve(JSON.parse(xhr.responseText));
            } catch (error) {
                reject(error);
            }
        };
        xhr.send();
    });
}

function buildDateRange() {
    var now = new Date();
    // Lấy 12 tháng cho biểu đồ xuất nhập tồn
    var from = new Date(now.getFullYear(), now.getMonth() - 11, 1);
    var to = new Date(now.getFullYear(), now.getMonth() + 1, 0);
    return {
        from: from.toISOString().slice(0, 10),
        to: to.toISOString().slice(0, 10),
    };
}

function getQueryParam(name) {
    if (!name) return "";
    if (window.URLSearchParams) {
        var params = new URLSearchParams(window.location.search || "");
        return params.get(name) || "";
    }
    var search = window.location.search || "";
    var match = new RegExp("[?&]" + name + "=([^&]*)", "i").exec(search);
    return match ? decodeURIComponent(match[1]) : "";
}

function setConnectionState(isOk, message) {
    var node = byId(ids.connectionStatus);
    if (!node) return;
    node.className = "dk-connection " + (isOk ? "ok" : "error");
    node.textContent = message;
}

function setLoading(loading) {
    state.loading = !!loading;

    var skeletonNodes = [
        "chartCapacityRing",
        "chartCustomerPie",
        "chartFlowTrend",
        "chartCapacityBar",
        "chartTop5MaxNL",
        "chartTop5MaxPL",
        "chartAgeStock",
        "chartActivityCalendarMonthly",
        "chartVolumePie",
        "chartTrendLine",
        "chartLoadBar",
        "lpcpDetailAssignments",
        "lpcpDetailPickOrders",
        "lpcpDetailWarnings",
    ];

    var textMetrics = [
        "metricTonDauKy",
        "metricTongNhap",
        "metricTongXuat",
        "metricTonKho",
        "metricPOChuanBiVe",
        "metricPODangTre",
        "metricGiaTriTonKho",
        "metricTongNhapDelta",
        "metricTongXuatDelta",
        "metricTonKhoDelta",
        "metricGiaTriTonKhoDelta",
    ];
    var tableBodies = ["customersBody", "racksBody"];

    if (loading) {
        for (var i = 0; i < skeletonNodes.length; i++) {
            var n = byId(skeletonNodes[i]);
            if (n) {
                n.innerHTML = '<div class="dk-skeleton"><div class="dk-skeleton-shimmer"></div></div>';
            }
        }
        for (var j = 0; j < textMetrics.length; j++) {
            var tm = byId(textMetrics[j]);
            if (tm)
                tm.innerHTML =
                    '<div class="dk-skeleton" style="width:60%; height:20px; display:inline-block;"><div class="dk-skeleton-shimmer"></div></div>';
        }
        for (var k = 0; k < tableBodies.length; k++) {
            var tb = byId(tableBodies[k]);
            if (tb)
                tb.innerHTML =
                    '<tr><td colspan="10"><div class="dk-skeleton" style="height:30px;"><div class="dk-skeleton-shimmer"></div></div></td></tr>';
        }
    }
    var button = byId(ids.refreshButton);
    if (!button) return;
    button.disabled = state.loading;
    button.textContent = state.loading ? "Đang tải..." : "Làm mới";
}

// v2.4.6 — Render delta chip "↑ 18.6% so với kỳ trước"
function formatDelta(delta) {
    if (delta === null || delta === undefined || isNaN(toNumber(delta))) return "";
    var d = toNumber(delta);
    var arrow = d > 0 ? "↑" : d < 0 ? "↓" : "→";
    var cls = d > 0 ? "dk-delta-up" : d < 0 ? "dk-delta-down" : "dk-delta-flat";
    return '<span class="' + cls + '">' + arrow + " " + formatNumber(Math.abs(d), 1) + "%</span> so với kỳ trước";
}


function rowDataAttr(type, index) {
    return 'class="clickable js-open-detail" data-detail="' + type + '" data-index="' + index + '"';
}


//#endregion


function buildDemoData() {
    var now = new Date();
    return {
        overall: [
            {
                TotalCapacity: 18420.5,
                CapacityNPL: 11620.25,
                CapacityPL: 6800.25,
                UsedNPL: 8515.6,
                UsedPL: 4272.4,
                TotalVatTuNPL: 8120,
                TotalVatTuPL: 3250,
                TotalVatTu: 11370,
                TotalFreeCapacity: 5632.5,
                PercentNPL: 73.28,
                PercentPL: 62.83,
                TotalPercent: 69.42,
                FreePercent: 30.58,
            },
        ],
        customers: [
            { MaKH: "KH001", TenKH: "Viking Apparel", SLVatTu: 2140, CBMSDTrongKho: 1650.2 },
            { MaKH: "KH002", TenKH: "Northwind Garment", SLVatTu: 1830, CBMSDTrongKho: 1422.8 },
            { MaKH: "KH003", TenKH: "EverWin Textile", SLVatTu: 1484, CBMSDTrongKho: 1218.5 },
            { MaKH: "KH004", TenKH: "Sunrise Uniform", SLVatTu: 1202, CBMSDTrongKho: 980.3 },
            { MaKH: "KH005", TenKH: "Ocean Knit", SLVatTu: 1090, CBMSDTrongKho: 912.1 },
            { MaKH: "KH006", TenKH: "BlueSky Sport", SLVatTu: 960, CBMSDTrongKho: 840.4 },
            { MaKH: "KH007", TenKH: "Lotus Fashion", SLVatTu: 846, CBMSDTrongKho: 755.7 },
            { MaKH: "KH008", TenKH: "Apex Wear", SLVatTu: 732, CBMSDTrongKho: 640.2 },
        ],
        racks: [
            {
                Module: 1,
                TenDay: "Dãy A",
                TenKe: "A-01",
                TongCBMSuDungTrongKe: 315.5,
                TongCBMTrongKe: 342.0,
                SLVatTu: 268,
            },
            {
                Module: 1,
                TenDay: "Dãy A",
                TenKe: "A-02",
                TongCBMSuDungTrongKe: 302.1,
                TongCBMTrongKe: 330.0,
                SLVatTu: 254,
            },
            {
                Module: 1,
                TenDay: "Dãy B",
                TenKe: "B-05",
                TongCBMSuDungTrongKe: 288.7,
                TongCBMTrongKe: 328.0,
                SLVatTu: 247,
            },
            {
                Module: 1,
                TenDay: "Dãy C",
                TenKe: "C-03",
                TongCBMSuDungTrongKe: 275.2,
                TongCBMTrongKe: 320.0,
                SLVatTu: 231,
            },
            {
                Module: 2,
                TenDay: "Dãy P1",
                TenKe: "P1-01",
                TongCBMSuDungTrongKe: 192.6,
                TongCBMTrongKe: 212.0,
                SLVatTu: 198,
            },
            {
                Module: 2,
                TenDay: "Dãy P1",
                TenKe: "P1-02",
                TongCBMSuDungTrongKe: 180.8,
                TongCBMTrongKe: 208.0,
                SLVatTu: 176,
            },
            {
                Module: 2,
                TenDay: "Dãy P2",
                TenKe: "P2-03",
                TongCBMSuDungTrongKe: 171.4,
                TongCBMTrongKe: 205.0,
                SLVatTu: 168,
            },
            {
                Module: 2,
                TenDay: "Dãy P2",
                TenKe: "P2-05",
                TongCBMSuDungTrongKe: 162.9,
                TongCBMTrongKe: 198.0,
                SLVatTu: 161,
            },
        ],
        inbound: [
            {
                PO: "PO-240501",
                SoLo: "SL-001",
                MaDH: "DH-1001",
                MaHang: "MH-JACKET-01",
                SoLuongSP: 1280,
                NgayNKDuKien: asIsoDate(addDays(now, 1)),
            },
            {
                PO: "PO-240502",
                SoLo: "SL-002",
                MaDH: "DH-1002",
                MaHang: "MH-SHIRT-06",
                SoLuongSP: 960,
                NgayNKDuKien: asIsoDate(addDays(now, 2)),
            },
            {
                PO: "PO-240503",
                SoLo: "SL-003",
                MaDH: "DH-1003",
                MaHang: "MH-PANTS-03",
                SoLuongSP: 760,
                NgayNKDuKien: asIsoDate(addDays(now, 3)),
            },
            {
                PO: "PO-240504",
                SoLo: "SL-004",
                MaDH: "DH-1004",
                MaHang: "MH-POLO-02",
                SoLuongSP: 1140,
                NgayNKDuKien: asIsoDate(addDays(now, 4)),
            },
            {
                PO: "PO-240505",
                SoLo: "SL-005",
                MaDH: "DH-1005",
                MaHang: "MH-HOODIE-08",
                SoLuongSP: 520,
                NgayNKDuKien: asIsoDate(addDays(now, 5)),
            },
        ],
        outboundReady: [
            {
                MaLenhSanXuat: "LSX-240601",
                MaLenh: "601",
                MaDVSX: "DVSX_1",
                TenHang: "Jacket Running",
                KhachHang: "Viking Apparel",
                KHCat: asIsoDate(addDays(now, 2)),
                DuKienCat: asIsoDate(addDays(now, 9)),
            },
            {
                MaLenhSanXuat: "LSX-240602",
                MaLenh: "602",
                MaDVSX: "DVSX_2",
                TenHang: "Shirt Office",
                KhachHang: "Northwind Garment",
                KHCat: asIsoDate(addDays(now, 1)),
                DuKienCat: asIsoDate(addDays(now, 8)),
            },
            {
                MaLenhSanXuat: "LSX-240603",
                MaLenh: "603",
                MaDVSX: "DVSX_3",
                TenHang: "Polo Classic",
                KhachHang: "EverWin Textile",
                KHCat: asIsoDate(addDays(now, 3)),
                DuKienCat: asIsoDate(addDays(now, 10)),
            },
            {
                MaLenhSanXuat: "LSX-240604",
                MaLenh: "604",
                MaDVSX: "DVSX_5",
                TenHang: "Sport Pant",
                KhachHang: "Sunrise Uniform",
                KHCat: asIsoDate(addDays(now, 4)),
                DuKienCat: asIsoDate(addDays(now, 11)),
            },
        ],
        outboundRunning: [
            {
                MaLenhSX: "LSX-240511",
                MaLenh: "511",
                MaGop: "GOP-301",
                TenHang: "Jacket Running",
                TenKH: "Viking Apparel",
                NgayXuatHang: asIsoDate(addDays(now, -1)),
                SLXuat: 1350.5,
            },
            {
                MaLenhSX: "LSX-240512",
                MaLenh: "512",
                MaGop: "GOP-302",
                TenHang: "Shirt Office",
                TenKH: "Northwind Garment",
                NgayXuatHang: asIsoDate(now),
                SLXuat: 980.0,
            },
            {
                MaLenhSX: "LSX-240513",
                MaLenh: "513",
                MaGop: "GOP-303",
                TenHang: "Polo Classic",
                TenKH: "Lotus Fashion",
                NgayXuatHang: asIsoDate(addDays(now, -2)),
                SLXuat: 740.25,
            },
            {
                MaLenhSX: "LSX-240514",
                MaLenh: "514",
                MaGop: "GOP-304",
                TenHang: "Sport Pant",
                TenKH: "BlueSky Sport",
                NgayXuatHang: asIsoDate(now),
                SLXuat: 1125.0,
            },
        ],
    };
}

function loadDemoData() {
    var demo = buildDemoData();
    state.overall = demo.overall;
    state.customers = demo.customers;
    state.racks = demo.racks;
    state.inbound = demo.inbound;
    state.outboundReady = demo.outboundReady;
    state.outboundRunning = demo.outboundRunning;
    state.lastUpdated = new Date();
    renderAll();
}

/**
 * Hàm cốt lõi: Nạp toàn bộ dữ liệu tổng quan cho trang Dashboard khi mới tải trang hoặc khi ấn "Làm mới".
 * Quá trình: Gọi hàng loạt API, sau khi tất cả Promise hoàn tất thì vẽ lại giao diện.
 * @param {boolean} skipLoadingState Bỏ qua hiệu ứng bộ xương (skeleton loading) màn hình
 * @param {boolean} skipReloadCurrent Không tải lại dữ liệu của tab/page hiện tại
 */
function loadData(skipLoadingState, skipReloadCurrent) {
    if (state.loading) return;
    if (isDemoMode) {
        loadDemoData();
        return;
    }

    var now = new Date();
    var inFromStr = now.toISOString().slice(0, 10);
    var inTo = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 14);
    var inToStr = inTo.toISOString().slice(0, 10);

    setLoading(true);

    function safeJson(url) {
        return requestJson(url)
            .then(function (r) {
                return r;
            })
            .catch(function (e) {
                console.warn("API lỗi [" + url + "]:", e.message);
                return [];
            });
    }

    var BASE = "/api/DashboardKhoDesktop/";
    // v2.4.6 — Query string filter ngày global
    var dfFrom = state.dateFilter && state.dateFilter.from ? state.dateFilter.from : "";
    var dfTo = state.dateFilter && state.dateFilter.to ? state.dateFilter.to : "";
    var dfQS = dfFrom && dfTo ? "?tuNgay=" + encodeURIComponent(dfFrom) + "&denNgay=" + encodeURIComponent(dfTo) : "";

    var lpcpFrom = new Date(now.getFullYear(), now.getMonth() - 1, 1);
    var lpcpTo = new Date(now.getFullYear(), now.getMonth() + 2, 0);
    function _isoDate(d) {
        return (
            d.getFullYear() +
            "-" +
            String(d.getMonth() + 1).padStart(2, "0") +
            "-" +
            String(d.getDate()).padStart(2, "0")
        );
    }
    var LPCP_URL =
        "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" +
        encodeURIComponent(_isoDate(lpcpFrom)) +
        "&denNgay=" +
        encodeURIComponent(_isoDate(lpcpTo));

    // === PAGE 1 priority — load + render NGAY (user thấy ngay khi vào) ===
    loadedPages[1] = true;
    var p1a = safeJson(BASE + "GetOverallCapacity").then(function (r) {
        state.overall = normalizeArray(r);
        renderMetricCards();
        renderCapacityChart();
        renderCapacityBarChart();
    });
    var p1b = safeJson(BASE + "GetCustomers").then(function (r) {
        state.customers = normalizeArray(r);
        populateCustomerFilter();
        renderCustomerPieChart();
        renderCustomersTable();
    });
    var p1c = safeJson(BASE + "GetDistinctMaterialCount").then(function (r) {
        state.distinctMat = normalizeArray(r);
        renderMetricCards();
    });
    var inboundTo = new Date(now);
    inboundTo.setDate(inboundTo.getDate() + 365);
    var inboundQS =
        "?tuNgay=" +
        encodeURIComponent(now.toISOString().slice(0, 10)) +
        "&denNgay=" +
        encodeURIComponent(inboundTo.toISOString().slice(0, 10));

    var p1d = safeJson(BASE + "GetChuanBiVe" + inboundQS).then(function (r) {
        state.inbound = normalizeArray(r);
        renderMetricCards();
    });
    var p1e = safeJson(BASE + "GetChuanBiXuat" + dfQS).then(function (r) {
        state.outboundReady = normalizeArray(r);
        renderMetricCards();
    });
    var p1f = safeJson(BASE + "GetDangXuat" + dfQS).then(function (r) {
        state.outboundRunning = normalizeArray(r);
        renderMetricCards();
    });
    var p1g = safeJson(BASE + "GetMoMComparison").then(function (r) {
        state.momComparison = normalizeArray(r);
        renderMoMStrip();
    });
    var p1h = safeJson(BASE + "GetRacks").then(function (r) {
        state.racks = normalizeArray(r);
    });
    var p1i = safeJson(BASE + "GetTop5?isNhieuNhat=1&loaiNPL=1").then(function (r) {
        state.top15MaxNL = normalizeArray(r);
    });
    var p1j = safeJson(BASE + "GetTop5?isNhieuNhat=1&loaiNPL=2").then(function (r) {
        state.top15MaxPL = normalizeArray(r);
    });
    var p1k = safeJson(BASE + "GetThanhGiaHangTon").then(function (r) {
        var arr = normalizeArray(r);
        state.thanhGia = arr.length > 0 ? arr[0] : null;
        renderMetricCards();
    });

    // v2.4.0 — 6 section mới của Tổng quan (stub API)
    var p1l = safeJson(BASE + "GetCongViecChoXuLy").then(function (r) {
        state.todoList = normalizeArray(r);
        renderTodoList();
    });
    var p1m = safeJson(BASE + "GetTop5VatTuDungTich").then(function (r) {
        state.top5VT = normalizeArray(r);
        renderTop5VTTable();
    });
    var p1n = safeJson(BASE + "GetTop5KhachHangTonKho").then(function (r) {
        state.top5KH = normalizeArray(r);
        renderTop5KHTable();
    });
    var p1o = safeJson(BASE + "GetVatTuSapHetHan").then(function (r) {
        state.vtHetHan = normalizeArray(r);
        renderHetHanTable();
    });
    var p1p = safeJson(BASE + "GetGiaTriTonKhoTheoNhom").then(function (r) {
        state.giaTriNhom = normalizeArray(r);
        renderGiaTriTheoNhomChart();
    });
    var p1q = safeJson(BASE + "GetTinhHinhKiemKe").then(function (r) {
        var arr = normalizeArray(r);
        state.kiemKe = arr.length > 0 ? arr[0] : null;
        renderKiemKeBox();
    });

    // v2.4.6 — 6 KPI mới (filter ngày) + alerts + hieusuat
    var p1r = safeJson(BASE + "GetTongNhap" + dfQS).then(function (r) {
        var arr = normalizeArray(r);
        state.kpiTongNhap = arr[0] || null;
        renderMetricCards();
    });
    var p1s = safeJson(BASE + "GetTongXuat" + dfQS).then(function (r) {
        var arr = normalizeArray(r);
        state.kpiTongXuat = arr[0] || null;
        renderMetricCards();
    });
    var p1t = safeJson(BASE + "GetTonKho" + (dfTo ? "?denNgay=" + encodeURIComponent(dfTo) : "")).then(function (r) {
        var arr = normalizeArray(r);
        state.kpiTonKho = arr[0] || null;
        renderMetricCards();
    });
    var p1u = safeJson(BASE + "GetTonDauKy" + (dfFrom ? "?tuNgay=" + encodeURIComponent(dfFrom) : "")).then(
        function (r) {
            var arr = normalizeArray(r);
            state.kpiTonDauKy = arr[0] || null;
            renderMetricCards();
        },
    );
    var p1v = safeJson(BASE + "GetPODangTre").then(function (r) {
        var arr = normalizeArray(r);
        state.kpiPODangTre = arr[0] || null;
        renderMetricCards();
    });
    var p1w = safeJson(BASE + "GetGiaTriTon" + (dfTo ? "?denNgay=" + encodeURIComponent(dfTo) : "")).then(function (r) {
        var arr = normalizeArray(r);
        state.kpiGiaTriTon = arr[0] || null;
        renderMetricCards();
    });
    // Page 2 widget mới — load lazy nhưng đặt vào Promise.all để re-fetch khi Áp dụng
    var p1x = safeJson(BASE + "GetCanhBaoTonKho").then(function (r) {
        state.alerts = normalizeArray(r);
        renderAlertsList();
    });
    var p1y = safeJson(BASE + "GetHieuSuatHoatDong" + dfQS).then(function (r) {
        state.hieuSuat = normalizeArray(r);
        renderHieuSuatGauges();
    });
    var pLPCP = requestJson(LPCP_URL)
        .then(function (res) {
            // Đọc Tasks và Inventory từ cấu trúc API mới
            var lpcpArr = normalizeArray(res.Tasks || res.data || res);
            var invArr = normalizeArray(res.Inventory || []);

            lpcpArr.forEach(function (d) {
                var k = String(d.NgayLam || "").substring(0, 10);
                if (k) state.lpcpCalendar[k] = d;
            });

            // Ánh xạ dữ liệu Inventory mới gán đè vào activityCalendar
            if (invArr.length > 0) {
                state.activityCalendar = invArr.map(function (item) {
                    var inQty = toNumber(item.SoLuongNhapKho);
                    var outQty = toNumber(item.SoLuongXuatHang);
                    var kkQty = toNumber(item.SoLuongKiemKe);
                    return {
                        NgayHoatDong: item.Ngay,
                        TotalIn: inQty,
                        TotalOut: outQty,
                        TotalKiemKe: kkQty,
                        TotalActivity: inQty + outQty + kkQty,
                    };
                });
            }
        })
        .catch(function (e) {
            console.warn("Lỗi tải lịch phân công:", e);
            return [];
        });

    // Bỏ gọi API GetActivityCalendar cũ để không bị ghi đè dữ liệu
    var pActivity = Promise.resolve([]);

    Promise.all([
        p1a,
        p1b,
        p1c,
        p1d,
        p1e,
        p1f,
        p1g,
        p1h,
        p1i,
        p1j,
        p1k,
        p1l,
        p1m,
        p1n,
        p1o,
        p1p,
        p1q,
        p1r,
        p1s,
        p1t,
        p1u,
        p1v,
        p1w,
        p1x,
        p1y,
        pLPCP,
        pActivity,
    ])
        .then(function () {
            state.lastUpdated = new Date();
            if (!skipLoadingState) {
                setLoading(false);
            } else {
                var button = byId(ids.refreshButton);
                if (button) {
                    button.disabled = false;
                    button.textContent = "Làm mới";
                }
                state.loading = false;
            }

            // Render lại lịch kho (Hàm này giờ sẽ có state.lpcpCalendar để vẽ chấm)
            renderActivityCalendarMonthly();

            // Tự động load nội dung của ngày hôm nay
            var today = new Date();
            if (typeof showLpcpInlineDetail === "function") {
                showLpcpInlineDetail(_isoDate(today));
            }

            // Render 3 biểu đồ mới ở cuối trang với dữ liệu thực tế
            if (typeof renderLpcpBottomCharts === "function") {
                renderLpcpBottomCharts();
            }

            if ((currentPage === 2 || currentPage === 3) && !skipReloadCurrent) {
                loadedPages[currentPage] = false;
                loadPageData(currentPage);
            }
            setTimeout(injectMaximizeButtons, 60);
        })
        .catch(function (e) {
            console.error("loadData Promise.all error:", e);
            if (!skipLoadingState) {
                setLoading(false);
            } else {
                var button = byId(ids.refreshButton);
                if (button) {
                    button.disabled = false;
                    button.textContent = "Làm mới";
                }
                state.loading = false;
            }
        });
}

/**
 * Tải dữ liệu bổ sung khi người dùng chuyển sang các trang/tab khác (Trang 2 hoặc Trang 3).
 * Giúp tối ưu hóa hiệu suất (Lazy Loading) bằng cách không tải tất cả API ngay từ đầu.
 * @param {number} pageNum Số thứ tự của trang cần tải (2 hoặc 3)
 * @returns {Promise} Trạng thái Promise khi gọi xong API
 */
function loadPageData(pageNum) {
    if (loadedPages[pageNum]) return Promise.resolve();
    loadedPages[pageNum] = true;

    function safeJson(url) {
        return requestJson(url)
            .then(function (r) {
                return r;
            })
            .catch(function (e) {
                console.warn("API lỗi [" + url + "]:", e.message);
                return [];
            });
    }
    var BASE = "/api/DashboardKhoDesktop/";

    if (pageNum === 2) {
        var promises = [];
        promises.push(
            safeJson(BASE + "GetRacks").then(function (r) {
                state.racks = normalizeArray(r);
                renderRacksTable();
                // renderRacksHeatmap();
            }),
        );
        promises.push(
            safeJson(BASE + "GetFlowTrend12T").then(function (r) {
                state.flowTrend12T = normalizeArray(r);
                var fromEl = byId("flowFromDate"),
                    toEl = byId("flowToDate");
                if (fromEl && toEl) {
                    var today = new Date();
                    var defStart = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);
                    if (!fromEl.value) fromEl.value = asIsoDate(defStart);
                    if (!toEl.value) toEl.value = asIsoDate(today);
                    loadAndRenderFlowByRange(new Date(fromEl.value + "T00:00:00"), new Date(toEl.value + "T00:00:00"));
                } else {
                    renderFlowTrendChart();
                }
            }),
        );
        promises.push(
            safeJson(BASE + "GetAgeStock").then(function (r) {
                state.ageStock = normalizeArray(r);
                renderAgeStockChart();
            }),
        );
        // Top 5 NL/PL charts — fire and render independently
        renderTop5MaxChart();
        renderTop5MinChart();
        // v2.7.2 FIX: Re-fetch alerts nếu chưa có data (khi navigate sang page 2 trước khi loadData hoàn thành)
        if (!state.alerts || state.alerts.length === 0) {
            promises.push(
                safeJson(BASE + "GetCanhBaoTonKho").then(function (r) {
                    state.alerts = normalizeArray(r);
                    renderAlertsList();
                }),
            );
        } else {
            renderAlertsList();
        }
        // v2.7.1 FIX: Fetch hieuSuat nếu chưa có data (khi navigate sang page 2 trước khi loadData hoàn thành)
        if (!state.hieuSuat || state.hieuSuat.length === 0) {
            var dfQS2 =
                "?tuNgay=" +
                encodeURIComponent(state.dateFilter.from) +
                "&denNgay=" +
                encodeURIComponent(state.dateFilter.to);
            promises.push(
                safeJson(BASE + "GetHieuSuatHoatDong" + dfQS2).then(function (r) {
                    state.hieuSuat = normalizeArray(r);
                    renderHieuSuatGauges();
                }),
            );
        } else {
            renderHieuSuatGauges();
        }
        setTimeout(injectMaximizeButtons, 60);
        return Promise.all(promises);
    }

    if (pageNum === 3) {
        // Sử dụng calMonthDate nếu có (để giữ nguyên tháng đang xem), nếu không thì dùng now
        var baseMonth = typeof calMonthDate !== "undefined" && calMonthDate ? calMonthDate : new Date();
        var from = new Date(baseMonth.getFullYear(), baseMonth.getMonth() - 1, 1);
        var to = new Date(baseMonth.getFullYear(), baseMonth.getMonth() + 2, 0);

        var urlLichGoc = BASE + "GetActivityCalendar?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
        var urlTrendLich = BASE + "GetFlowTrendByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
        var urlNKDK = BASE + "GetNKDuKienByRange?tuNgay=" + asIsoDate(from) + "&denNgay=" + asIsoDate(to);
        var urlLPCP =
            "/api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth?tuNgay=" +
            asIsoDate(from) +
            "&denNgay=" +
            asIsoDate(to);

        // Khởi tạo state rỗng
        state.nkDuKien = [];
        state.activityCalendar = [];
        state.lpcpCalendar = {};
        state.flowTrendByRange = [];

        // GỌI 4 API SONG SONG VÀ ĐỢI TẤT CẢ HOÀN TẤT
        return Promise.all([
            safeJson(urlNKDK),
            safeJson(urlLichGoc),
            requestJson(urlLPCP).catch(function (e) {
                console.warn("API LPCP lỗi, bỏ qua hiển thị:", e);
                return [];
            }),
            safeJson(urlTrendLich),
        ])
            .then(function (results) {
                state.nkDuKien = normalizeArray(results[0]);
                state.flowTrendByRange = normalizeArray(results[3]);

                // Xử lý dữ liệu LPCP và Inventory mới từ kết quả API
                var rLPCP = results[2] || {};
                var lpcpArr = normalizeArray(rLPCP.Tasks || rLPCP.data || rLPCP);
                var invArr = normalizeArray(rLPCP.Inventory || []);

                lpcpArr.forEach(function (d) {
                    var k = String(d.NgayLam || d.ngayLam || "").substring(0, 10);
                    if (k) state.lpcpCalendar[k] = d;
                });

                // [FIX] Cập nhật Inventory cho Lịch — ưu tiên từ API LPCP, fallback API cũ
                if (invArr.length > 0) {
                    state.activityCalendar = invArr.map(function (item) {
                        var inQty = toNumber(
                            item.SoLuongNhapKho || item.SoLuongNhap || item.TotalIn || item.totalIn || item.SLNhap || 0,
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
                    // Fallback: dùng dữ liệu từ GetActivityCalendar (API cũ)
                    state.activityCalendar = normalizeArray(results[1]);
                }

                // RENDER LỊCH 1 LẦN DUY NHẤT SAU KHI ĐÃ CÓ FULL DỮ LIỆU
                renderActivityCalendarMonthly();
                setTimeout(injectMaximizeButtons, 60);

                // Load LPCP stats after data is ready
                loadLpcpStatsForMonth(calMonthDate || now);

                // Tự động load chi tiết ngày hiện tại cho sidebar
                if (typeof showLpcpInlineDetail === "function") {
                    showLpcpInlineDetail(asIsoDate(new Date()));
                }
            })
            .catch(function (e) {
                console.warn("loadPageData page 3 error:", e);
            });
    }

    return Promise.resolve();
}

/**
 * Hàm tải lại dữ liệu theo trình tự: Tải lại trang hiện tại (nếu là trang 2 hoặc 3) trước,
 * sau đó mới tải lại dữ liệu tổng quan. Được gọi khi áp dụng bộ lọc ngày tháng.
 */
function triggerSequentialReload() {
    if (currentPage === 2 || currentPage === 3) {
        setLoading(true); // Hiển thị skeleton cho toàn bộ các tab
        var p = loadPageData(currentPage);
        if (p && p.then) {
            p.then(function () {
                loadData(true, true);
            }).catch(function () {
                loadData(true, true);
            });
        } else {
            loadData();
        }
    } else {
        loadData();
    }
}

/**
 * Hàm tải và vẽ biểu đồ luồng xuất nhập tồn (Flow Trend) cho một khoảng thời gian được chọn.
 * @param {Date} fromDate Ngày bắt đầu
 * @param {Date} toDate Ngày kết thúc
 */
function loadAndRenderFlowByRange(fromDate, toDate) {
    var node = byId(ids.chartFlowTrend);
    if (node) node.innerHTML = '<div class="dk-empty" style="padding:20px">Đang tải dữ liệu...</div>';
    var url =
        "/api/DashboardKhoDesktop/GetFlowTrendByRange?tuNgay=" + asIsoDate(fromDate) + "&denNgay=" + asIsoDate(toDate);
    requestJson(url)
        .then(function (data) {
            var arr = normalizeArray(data);
            if (!arr || arr.length === 0) {
                if (node) node.innerHTML = '<div class="dk-empty">Không có dữ liệu trong khoảng đã chọn</div>';
                return;
            }
            var labels = [],
                inbound = [],
                outbound = [],
                stock = [];
            for (var i = 0; i < arr.length; i++) {
                var ngay = String(arr[i].Ngay || "").substring(0, 10);
                var parts = ngay.split("-");
                var shortLbl = parts.length === 3 ? parts[2] + "/" + parts[1] : ngay;
                labels.push(shortLbl);
                inbound.push(toNumber(arr[i].TotalIn));
                outbound.push(toNumber(arr[i].TotalOut));
                stock.push(toNumber(arr[i].TotalStock));
            }
            // Lưu lại vào state để "Xem chi tiết" có thể đọc
            state.flowTrendRangeRaw = arr;
            renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
        })
        .catch(function () {
            if (node) node.innerHTML = '<div class="dk-empty">Lỗi tải dữ liệu</div>';
        });
}

/**
 * Hàm tải và vẽ biểu đồ luồng xuất nhập tồn (Flow Trend) hiển thị theo tuần (Weekly).
 */
function loadAndRenderFlowWeekly() {
    var node = byId(ids.chartFlowTrend);
    if (node) node.innerHTML = '<div class="dk-empty" style="padding:20px">Đang tải dữ liệu tuần...</div>';
    requestJson("/api/DashboardKhoDesktop/GetFlowTrendWeekly")
        .then(function (data) {
            var arr = normalizeArray(data);
            if (!arr || arr.length === 0) {
                renderFlowTrendChart();
                return;
            }
            var labels = [],
                inbound = [],
                outbound = [],
                stock = [];
            for (var i = 0; i < arr.length; i++) {
                labels.push("T" + arr[i].Tuan + "/" + String(arr[i].Nam).slice(-2));
                inbound.push(toNumber(arr[i].TotalIn));
                outbound.push(toNumber(arr[i].TotalOut));
                stock.push(toNumber(arr[i].TotalStock));
            }
            renderFlowTrendCustom({ labels: labels, inbound: inbound, outbound: outbound, stock: stock });
        })
        .catch(function () {
            renderFlowTrendChart();
        });
}