/**
 * @file dashboard-kho-state.js
 * @description Chứa các biến trạng thái toàn cục (State) dùng chung cho toàn bộ Dashboard.
 * @version 2.7.26
 */

var DK_VERSION = "2.6.16";
try {
    console.log(
        "%c[Dashboard Kho Desktop] v" + DK_VERSION + " loaded",
        "background:#2563eb;color:#fff;padding:4px 10px;border-radius:4px;font-weight:700;font-size:13px",
    );
} catch (e) {}
window.__DK_VERSION__ = DK_VERSION;

var state = {
    overall: [],
    customers: [],
    racks: [],
    inbound: [],
    outboundReady: [],
    outboundRunning: [],
    top15MaxNL: [],
    top15MaxPL: [],
    nkDuKien: [],
    calActFilter: { in: true, out: true, kk: true, plan: true }, // v2.3.33
    thanhGia: null,
    ageStock: [],
    distinctMat: [],
    activityCalendar: [],
    momComparison: [],
    flowTrend12T: [],
    flowTrendRangeRaw: [],
    lastUpdated: null,
    lpcpCalendar: {},
    lpcpStats: null,
    loading: false,
};
var activeCustomerFilter = "";
// v2.4.6 — Filter ngày global cho dashboard
state.dateFilter = (function () {
    var to = new Date();
    var from = new Date();
    from.setDate(to.getDate() - 30);
    return {
        from: from.toISOString().slice(0, 10),
        to: to.toISOString().slice(0, 10),
    };
})();
state.kpiTongNhap = null;
state.kpiTongXuat = null;
state.kpiTonKho = null;
state.kpiTonDauKy = null;
state.kpiPODangTre = null;
state.kpiGiaTriTon = null;
state.alerts = [];
state.hieuSuat = [];
var activityWeeksCount = 13;
var isDemoMode = getQueryParam("demo") === "1";
var calMonthDate = null;
var calRangeFrom = null;
var calRangeTo = null;