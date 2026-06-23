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
} catch (e) { }
window.__DK_VERSION__ = DK_VERSION;

// Biến lưu trữ toàn bộ dữ liệu trạng thái (State) của Dashboard
var state = {
    overall: [],          // Dữ liệu tổng quan chung (dung lượng, tỷ lệ lấp đầy)
    customers: [],        // Dữ liệu tồn kho theo từng khách hàng
    racks: [],            // Dữ liệu tồn kho theo kệ/vị trí
    inbound: [],          // Dữ liệu hàng chuẩn bị nhập kho (PO sắp về)
    outboundReady: [],    // Dữ liệu hàng chuẩn bị xuất kho
    outboundRunning: [],  // Dữ liệu hàng đang xuất/đang lấy
    top15MaxNL: [],       // Top 15 nguyên liệu tồn nhiều nhất
    top15MaxPL: [],       // Top 15 phụ liệu tồn nhiều nhất
    nkDuKien: [],         // Dữ liệu nhập kho dự kiến cho Lịch
    calActFilter: { in: true, out: true, kk: true, plan: true }, // Trạng thái bộ lọc hiển thị trên Lịch (Nhập/Xuất/Kiểm kê/Kế hoạch)
    thanhGia: null,       // Dữ liệu thành tiền/giá trị tồn kho
    ageStock: [],         // Dữ liệu biểu đồ tuổi tồn kho
    distinctMat: [],      // Số lượng mã vật tư phân biệt
    activityCalendar: [], // Dữ liệu lịch sử hoạt động kho (lịch tháng)
    momComparison: [],    // So sánh tháng này với tháng trước (Month-over-Month)
    flowTrend12T: [],     // Dữ liệu biểu đồ xu hướng xuất nhập tồn (12 tháng)
    flowTrendRangeRaw: [],// Dữ liệu gốc của biểu đồ xu hướng theo khoảng thời gian
    lastUpdated: null,    // Thời điểm cập nhật dữ liệu gần nhất (Date object)
    lpcpCalendar: {},     // Dữ liệu Lịch phân công (dạng Object mapping theo ngày)
    lpcpStats: null,      // Thống kê Lịch phân công
    loading: false,       // Cờ đánh dấu hệ thống đang call API/đang tải dữ liệu
};
// Biến lưu mã khách hàng đang được chọn để lọc (nếu có)
var activeCustomerFilter = "";

// v2.4.6 — Bộ lọc ngày tháng áp dụng chung cho toàn bộ dashboard (mặc định 30 ngày gần nhất)
state.dateFilter = (function () {
    var to = new Date();
    var from = new Date();
    from.setDate(to.getDate() - 30);
    return {
        from: from.toISOString().slice(0, 10),
        to: to.toISOString().slice(0, 10),
    };
})();

// --- Các biến lưu trữ dữ liệu KPI Header (Chỉ số đo lường chính) ---
state.kpiTongNhap = null;     // KPI: Tổng số lượng hàng đã nhập
state.kpiTongXuat = null;     // KPI: Tổng số lượng hàng đã xuất
state.kpiTonKho = null;       // KPI: Tổng số lượng hàng đang tồn
state.kpiTonDauKy = null;     // KPI: Tổng số lượng hàng tồn đầu kỳ
state.kpiPODangTre = null;    // KPI: Số lượng Đơn hàng (PO) đang trễ tiến độ
state.kpiGiaTriTon = null;    // KPI: Tổng giá trị hàng tồn kho
state.alerts = [];            // Danh sách các cảnh báo (VD: sắp hết hạn, tồn quá lâu)
state.hieuSuat = [];          // Dữ liệu hiệu suất hoạt động của kho

// Số tuần hiển thị trên biểu đồ hoạt động (mặc định 13 tuần ~ 1 quý)
var activityWeeksCount = 13;

// Cờ kiểm tra xem có đang chạy ở chế độ demo (hiển thị dữ liệu giả) hay không
var isDemoMode = getQueryParam("demo") === "1";

// --- Các biến lưu trữ trạng thái hiển thị của Lịch hoạt động ---
var calMonthDate = null;      // Tháng đang hiển thị trên Lịch
var calRangeFrom = null;      // Ngày bắt đầu của khoảng filter trên Lịch
var calRangeTo = null;        // Ngày kết thúc của khoảng filter trên Lịch