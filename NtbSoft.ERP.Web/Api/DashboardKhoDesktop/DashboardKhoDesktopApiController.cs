using NtbSoft.ERP.Model.DashboardKho;
using System;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.DashboardKhoDesktop
{
    [RoutePrefix("api/DashboardKhoDesktop")]
    public class DashboardKhoDesktopApiController : ApiController
    {

        private static System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>> ToList(DataTable dt)
        {
            var list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    var val = row[col] == DBNull.Value ? null : row[col];
                    dict[col.ColumnName] = val;
                }
                list.Add(dict);
            }
            return list;
        }

        /// <summary>
        /// Lấy dung lượng lưu trữ tổng quan của kho, bao gồm dung lượng kệ NPL, PL, đã dùng và còn trống.
        /// </summary>
        [HttpGet]
        [Route("GetOverallCapacity")]
        public IHttpActionResult GetOverallCapacity()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetOverallCapacity())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        
        /// <summary>
        /// Đếm số lượng mã vật tư NPL/PL khác nhau đang có trong kho (tính theo Barcode và mã định danh).
        /// </summary>
        [Route("GetDistinctMaterialCount")]
        public IHttpActionResult GetDistinctMaterialCount()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetDistinctMaterialCount())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Lấy danh sách thông tin khách hàng đang có vật tư nằm trong kho, tổng số SKU và thể tích chiếm chỗ.
        /// </summary>
        [HttpGet]
        [Route("GetCustomers")]
        public IHttpActionResult GetCustomers()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetCustomers())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetVatTuTheoKhachHang
        /// </summary>
        [HttpGet]
        [Route("GetVatTuTheoKhachHang")]
        public IHttpActionResult GetVatTuTheoKhachHang(string maKH = "")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuTheoKhachHang(maKH))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Thống kê chi tiết thể tích chiếm chỗ của các ô và kệ trong kho.
        /// </summary>
        [HttpGet]
        [Route("GetRacks")]
        public IHttpActionResult GetRacks()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetRacks())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Lấy danh sách hàng hóa (PO) dự kiến nhập kho (chuẩn bị về) trong khoảng thời gian hoặc theo keyword.
        /// </summary>
        /// <param name="tuNgay">Ngày bắt đầu dự kiến</param>
        /// <param name="denNgay">Ngày kết thúc dự kiến</param>
        /// <param name="keyword">Từ khóa tìm kiếm (POMua, SoLoID)</param>
        /// <summary>
        /// Xử lý request cho GetChuanBiVe
        /// </summary>
        [HttpGet]
        [Route("GetChuanBiVe")]
        public IHttpActionResult GetChuanBiVe(DateTime? tuNgay = null, DateTime? denNgay = null, string keyword = "")
        {
            try
            {
                var from = (tuNgay ?? DateTime.Today).Date;
                var to   = (denNgay ?? from.AddDays(14)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetChuanBiVe(from, to, keyword)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }


        /// <summary>
        /// Lấy danh sách các lệnh sản xuất đã có yêu cầu cấp vật tư nhưng chưa được xuất kho (chuẩn bị xuất).
        /// </summary>
        [HttpGet]
        [Route("GetChuanBiXuat")]
        public IHttpActionResult GetChuanBiXuat(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetChuanBiXuat(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }


        /// <summary>
        /// Theo dõi tình trạng các phiếu yêu cầu đang xuất kho (tiến độ xuất vật tư).
        /// </summary>
        [HttpGet]
        [Route("GetDangXuat")]
        public IHttpActionResult GetDangXuat(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetDangXuat(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Lấy xu hướng luồng hàng (Nhập - Xuất - Tồn) theo 12 tháng gần nhất.
        /// </summary>
        [HttpGet]
        [Route("GetFlowTrend12T")]
        public IHttpActionResult GetFlowTrend12T()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetFlowTrend12T())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetFlowTrendWeekly
        /// </summary>
        [HttpGet]
        [Route("GetFlowTrendWeekly")]
        public IHttpActionResult GetFlowTrendWeekly()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetFlowTrendWeekly())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
        /// <summary>
        /// Xử lý request cho GetAgeStock
        /// </summary>
        [HttpGet]
        [Route("GetAgeStock")]
        public IHttpActionResult GetAgeStock()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetAgeStock())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetActivityCalendar
        /// </summary>
        [HttpGet]
        [Route("GetActivityCalendar")]
        public IHttpActionResult GetActivityCalendar(DateTime? tuNgay = null, DateTime? denNgay = null, string maNPL = "all", string soLoID = "all", string maHang = "all", string maKH = "all", string khoLoi = "0", string nhom = "all", int isNPL = 2)
        {
            try
            {
                if (tuNgay.HasValue && denNgay.HasValue) {
                    if (tuNgay.Value.Date > denNgay.Value.Date) return BadRequest("Invalid date range: tuNgay > denNgay");
                    return Ok(ToList(DashboardKhoDesktopModel.GetActivityCalendar(tuNgay.Value, denNgay.Value, maNPL, soLoID, maHang, maKH, khoLoi, nhom, isNPL)));
                }
                return Ok(ToList(DashboardKhoDesktopModel.GetActivityCalendar(maNPL, soLoID, maHang, maKH, khoLoi, nhom, isNPL)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
        /// <summary>
        /// Xử lý request cho GetFlowTrendByRange
        /// </summary>
        [HttpGet]
        [Route("GetFlowTrendByRange")]
        public IHttpActionResult GetFlowTrendByRange(DateTime? tuNgay = null, DateTime? denNgay = null, string maNPL = "all", string soLoID = "all", string maHang = "all", string maKH = "all", string khoLoi = "0", string nhom = "all", int isNPL = 2)
        {
            try
            {
                DateTime to   = (denNgay ?? DateTime.Today).Date;
                DateTime from = (tuNgay  ?? to.AddDays(-30)).Date;
                if (from > to) return BadRequest("Invalid date range: tuNgay > denNgay");
                return Ok(ToList(DashboardKhoDesktopModel.GetFlowTrendByRange(from, to, maNPL, soLoID, maHang, maKH, khoLoi, nhom, isNPL)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetRackSlotDetail
        /// </summary>
        [HttpGet]
        [Route("GetRackSlotDetail")]
        public IHttpActionResult GetRackSlotDetail()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetRackSlotDetail())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetActivityRangeDetail
        /// </summary>
        [HttpGet]
        [Route("GetActivityRangeDetail")]
        public HttpResponseMessage GetActivityRangeDetail(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                var dtNhap = DashboardKhoDesktopModel.GetNhapDetailByRange(tuNgay, denNgay);
                var dtXuat = DashboardKhoDesktopModel.GetXuatDetailByRange(tuNgay, denNgay);
                var dtKiemKe = DashboardKhoDesktopModel.GetKiemKeDetailByRange(tuNgay, denNgay);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { Nhap = dtNhap, Xuat = dtXuat, KiemKe = dtKiemKe });
                return new HttpResponseMessage()
                {
                    Content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Xử lý request cho GetThanhGiaHangTon
        /// </summary>
        [HttpGet]
        [Route("GetThanhGiaHangTon")]
        public IHttpActionResult GetThanhGiaHangTon()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetThanhGiaHangTon())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetAllMaterialsInStock
        /// </summary>
        [HttpGet]
        [Route("GetAllMaterialsInStock")]
        public IHttpActionResult GetAllMaterialsInStock()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetAllMaterialsInStock())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetNKDuKienByRange
        /// </summary>
        [HttpGet]
        [Route("GetNKDuKienByRange")]
        public IHttpActionResult GetNKDuKienByRange(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime from = (tuNgay  ?? today.AddDays(-30)).Date;
                DateTime to   = (denNgay ?? today.AddDays(60)).Date;
                if (from > to) return BadRequest("Invalid date range: tuNgay > denNgay");
                return Ok(ToList(DashboardKhoDesktopModel.GetNKDuKienByRange(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho ClearCache
        /// </summary>
        [HttpGet]
        [Route("ClearCache")]
        public IHttpActionResult ClearCache()
        {
            try
            {
                DashboardKhoDesktopModel.ClearCache();
                System.Runtime.Caching.MemoryCache.Default.Remove("dk_LichPhanCong_GetNhanVienList");
                return Ok(new { Message = "Cache cleared", At = DateTime.Now });
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetActivityDayDetail
        /// </summary>
        [HttpGet]
        [Route("GetActivityDayDetail")]
        public IHttpActionResult GetActivityDayDetail(DateTime ngay)
        {
            try
            {
                return Ok(new
                {
                    Nhap   = ToList(DashboardKhoDesktopModel.GetNhapDetailByDay(ngay)),
                    Xuat   = ToList(DashboardKhoDesktopModel.GetXuatDetailByDay(ngay)),
                    KiemKe = ToList(DashboardKhoDesktopModel.GetKiemKeDetailByDay(ngay))
                });
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho LichPhanCong_GetDayDetail
        /// </summary>
        [HttpGet]
        [Route("LichPhanCong_GetDayDetail")]
        public IHttpActionResult LichPhanCong_GetDayDetail(DateTime ngay)
        {
            try
            {
                var result = DashboardKhoDesktopModel.GetLichPhanCongDayDetail(ngay);
                if (result == null) 
                    return Ok(new { success = true, data = new { } });
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message, detail = ex.GetType().Name });
            }
        }

        /// <summary>
        /// Xử lý request cho GetMoMComparison
        /// </summary>
        [HttpGet]
        [Route("GetMoMComparison")]
        public IHttpActionResult GetMoMComparison()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetMoMComparison())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
        /// <summary>
        /// Xử lý request cho GetTop5
        /// </summary>
        [HttpGet]
        [Route("GetTop5")]
        public IHttpActionResult GetTop5(int isNhieuNhat = 1, int loaiNPL = 0)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTop5(isNhieuNhat, loaiNPL))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GlobalSearch
        /// </summary>
        [HttpGet]
        [Route("GlobalSearch")]
        public IHttpActionResult GlobalSearch(string itemcode = "")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GlobalSearchByItemcode(itemcode ?? string.Empty))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GlobalSearchAll
        /// </summary>
        [HttpGet]
        [Route("GlobalSearchAll")]
        public IHttpActionResult GlobalSearchAll(string keyword = "")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GlobalSearchAll(keyword ?? string.Empty))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }


        /// <summary>
        /// Xử lý request cho GetCongViecChoXuLy
        /// </summary>
        [HttpGet]
        [Route("GetCongViecChoXuLy")]
        public IHttpActionResult GetCongViecChoXuLy(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetCongViecChoXuLy(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTop5VatTuDungTich
        /// </summary>
        [HttpGet]
        [Route("GetTop5VatTuDungTich")]
        public IHttpActionResult GetTop5VatTuDungTich()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTop5VatTuDungTich())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTop5KhachHangTonKho
        /// </summary>
        [HttpGet]
        [Route("GetTop5KhachHangTonKho")]
        public IHttpActionResult GetTop5KhachHangTonKho()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTop5KhachHangTonKho())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetVatTuSapHetHan
        /// </summary>
        [HttpGet]
        [Route("GetVatTuSapHetHan")]
        public IHttpActionResult GetVatTuSapHetHan()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuSapHetHan())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }  
        }

        /// <summary>
        /// Xử lý request cho GetGiaTriTonKhoTheoNhom
        /// </summary>
        [HttpGet]
        [Route("GetGiaTriTonKhoTheoNhom")]
        public IHttpActionResult GetGiaTriTonKhoTheoNhom()
        { 
            try { return Ok(ToList(DashboardKhoDesktopModel.GetGiaTriTonKhoTheoNhom())); } 
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
         
        /// <summary>
        /// Xử lý request cho GetTinhHinhKiemKe
        /// </summary>
        [HttpGet]
        [Route("GetTinhHinhKiemKe")]
        public IHttpActionResult GetTinhHinhKiemKe() 
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTinhHinhKiemKe())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
 

        /// <summary>
        /// Xử lý request cho GetTodoDetail
        /// </summary>
        [HttpGet]
        [Route("GetTodoDetail")]
        public IHttpActionResult GetTodoDetail(string type = "itemcode_cho_nk")   
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTodoDetail(type))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetVatTuTheoDungTich
        /// </summary>
        [HttpGet]
        [Route("GetVatTuTheoDungTich")] 
        public IHttpActionResult GetVatTuTheoDungTich() 
        { 
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuTheoDungTich())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetKhachHangTonKhoChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetKhachHangTonKhoChiTiet")]
        public IHttpActionResult GetKhachHangTonKhoChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetKhachHangTonKhoChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetVatTuSapHetHanChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetVatTuSapHetHanChiTiet")]
        public IHttpActionResult GetVatTuSapHetHanChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuSapHetHanChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetGiaTriNhomChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetGiaTriNhomChiTiet")]
        public IHttpActionResult GetGiaTriNhomChiTiet()
        {  
            try { return Ok(ToList(DashboardKhoDesktopModel.GetGiaTriNhomChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetKiemKeChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetKiemKeChiTiet")]
        public IHttpActionResult GetKiemKeChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetKiemKeChiTiet())); } 
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); } 
        } 


        /// <summary>
        /// Xử lý request cho GetTongNhap
        /// </summary>
        [HttpGet]
        [Route("GetTongNhap")] 
        public IHttpActionResult GetTongNhap(DateTime? tuNgay = null, DateTime? denNgay = null) 
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetTongNhap(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTongXuat
        /// </summary>
        [HttpGet]
        [Route("GetTongXuat")]
        public IHttpActionResult GetTongXuat(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetTongXuat(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTonKho
        /// </summary>
        [HttpGet]
        [Route("GetTonKho")]
        public IHttpActionResult GetTonKho(DateTime? denNgay = null)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTonKho((denNgay ?? DateTime.Today).Date))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTonDauKy
        /// </summary>
        [HttpGet]
        [Route("GetTonDauKy")]
        public IHttpActionResult GetTonDauKy(DateTime? tuNgay = null)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTonDauKy((tuNgay ?? DateTime.Today.AddDays(-30)).Date))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetPODangTre
        /// </summary>
        [HttpGet]
        [Route("GetPODangTre")]
        public IHttpActionResult GetPODangTre()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetPODangTre())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetGiaTriTon
        /// </summary>
        [HttpGet]
        [Route("GetGiaTriTon")]
        public IHttpActionResult GetGiaTriTon(DateTime? denNgay = null)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetGiaTriTon((denNgay ?? DateTime.Today).Date))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetCanhBaoTonKho
        /// </summary>
        [HttpGet]
        [Route("GetCanhBaoTonKho")]
        public IHttpActionResult GetCanhBaoTonKho()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetCanhBaoTonKho())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetHieuSuatHoatDong
        /// </summary>
        [HttpGet]
        [Route("GetHieuSuatHoatDong")]
        public IHttpActionResult GetHieuSuatHoatDong(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-7)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetHieuSuatHoatDong(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetHieuSuatHoatDongChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetHieuSuatHoatDongChiTiet")]
        public IHttpActionResult GetHieuSuatHoatDongChiTiet(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-7)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetHieuSuatHoatDong(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }


        /// <summary>
        /// Xử lý request cho GetTonDauKyChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetTonDauKyChiTiet")]
        public IHttpActionResult GetTonDauKyChiTiet(DateTime? tuNgay = null, string loai = "all")
        {
            try
            {
                var from = (tuNgay ?? DateTime.Today.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetTonDauKyChiTiet(from, loai)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTongNhapChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetTongNhapChiTiet")]
        public IHttpActionResult GetTongNhapChiTiet(DateTime? tuNgay = null, DateTime? denNgay = null, string groupBy = "date")
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetTongNhapChiTiet(from, to, groupBy)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTongXuatChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetTongXuatChiTiet")]
        public IHttpActionResult GetTongXuatChiTiet(DateTime? tuNgay = null, DateTime? denNgay = null, string groupBy = "date")
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                var from = (tuNgay ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetTongXuatChiTiet(from, to, groupBy)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetTonKhoChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetTonKhoChiTiet")]
        public IHttpActionResult GetTonKhoChiTiet(DateTime? denNgay = null, string loai = "all")
        {
            try
            {
                var to = (denNgay ?? DateTime.Today).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetTonKhoChiTiet(to, loai)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetPODangTreChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetPODangTreChiTiet")]
        public IHttpActionResult GetPODangTreChiTiet(string groupBy = "all")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetPODangTreChiTiet(groupBy))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }



        /// <summary>
        /// Xử lý request cho GetNPLThieuChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetNPLThieuChiTiet")]
        public IHttpActionResult GetNPLThieuChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetNPLThieuChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        /// <summary>
        /// Xử lý request cho GetKiemKeLechChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetKiemKeLechChiTiet")]
        public IHttpActionResult GetKiemKeLechChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetKiemKeLechChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }



        /// <summary>
        /// Xử lý request cho GetTonVuotDinhMucChiTiet
        /// </summary>
        [HttpGet]
        [Route("GetTonVuotDinhMucChiTiet")]
        public IHttpActionResult GetTonVuotDinhMucChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTonVuotDinhMucChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }



        [HttpGet, Route("LichPhanCong_GetCalendarMonth")]
        public IHttpActionResult GetCalendarMonth(string tuNgay = null, string denNgay = null)
        {
            try
            {
                DateTime tuDate = string.IsNullOrEmpty(tuNgay)
                    ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                    : DateTime.Parse(tuNgay);
                DateTime denDate = string.IsNullOrEmpty(denNgay)
                    ? new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                        DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                    : DateTime.Parse(denNgay);

                if (tuDate.Date > denDate.Date) return BadRequest("Invalid date range: tuNgay > denNgay");

                var result = DashboardKhoDesktopModel.GetLichPhanCongCalendarMonth(tuDate, denDate);
                var inventory = ToList(DashboardKhoDesktopModel.GetActivityCalendar(tuDate, denDate, "all", "all", "all", "all", "0", "all", 2));

                return Ok(new
                {
                    success = true,
                    data = result,
                    Tasks = result,
                    Inventory = inventory
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message, detail = ex.GetType().Name });
            }
        }

        // ===================================================================
        // GET api/DashboardKhoDesktop/LichPhanCong_GetNhanVienList
        // Lấy danh sách nhân viên để chọn phân công
        // (Van's code — merged 2026-06-05)
        // ===================================================================
        /// <summary>
        /// Xử lý request cho GetNhanVienList
        /// </summary>
        [HttpGet]
        [Route("LichPhanCong_GetNhanVienList")]
        public IHttpActionResult GetNhanVienList()
        {
            try
            {
                var cache = System.Runtime.Caching.MemoryCache.Default;
                string cacheKey = "dk_LichPhanCong_GetNhanVienList";
                if (cache.Contains(cacheKey))
                {
                    return Ok(new { success = true, data = cache.Get(cacheKey) });
                }

                var result = new System.Collections.Generic.List<NtbSoft.ERP.Model.DashboardKho.LichPhanCong_NhanVienModel>();
                string connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["strCnn_ln"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SP_LICH_PHAN_CONG_PHU_LIEU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@Action", "GetNhanVienList");

                    conn.Open();
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Func<System.Data.SqlClient.SqlDataReader, string, string> safeStr = (r, col) => {
                            try { return r[col] != DBNull.Value ? r[col].ToString() : ""; }
                            catch { return ""; }
                        };
                        Func<System.Data.SqlClient.SqlDataReader, string, bool> safeBool = (r, col) => {
                            try { return r[col] != DBNull.Value && Convert.ToInt32(r[col]) == 1; }
                            catch { return false; }
                        };

                        while (reader.Read())
                        {
                            var nv = new NtbSoft.ERP.Model.DashboardKho.LichPhanCong_NhanVienModel();
                            nv.MaNV = safeStr(reader, "MaNV");
                            nv.TenNV = safeStr(reader, "TenNV");
                            nv.MaPhongBan = safeStr(reader, "MaPhongBan");
                            nv.TenPhongBan = safeStr(reader, "TenPhongBan");
                            nv.IsActive = safeBool(reader, "IsActive");
                            result.Add(nv);
                        }
                    }
                }
                cache.Add(cacheKey, result, DateTimeOffset.Now.AddMinutes(30));
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message, detail = ex.GetType().Name });
            }
        }
    }
}
