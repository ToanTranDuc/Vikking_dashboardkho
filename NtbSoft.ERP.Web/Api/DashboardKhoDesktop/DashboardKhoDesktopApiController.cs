using NtbSoft.ERP.Model.DashboardKho;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.DashboardKhoDesktop
{
    [RoutePrefix("api/DashboardKhoDesktop")]
    public class DashboardKhoDesktopApiController : ApiController
    {
        private static readonly Encoding Latin1 = Encoding.GetEncoding(1252);

        
        private static readonly Regex GarbledPattern = new Regex(
            @"[\xC0-\xDF][\x80-\xBF]|[\xE0-\xEF][\x80-\xBF]{2}",
            RegexOptions.Compiled);

        private static string FixUtf8(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            bool hasHigh = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] > 127) { hasHigh = true; break; }
            }
            if (!hasHigh) return s;
            // Check if it matches garbled UTF-8 
            if (!GarbledPattern.IsMatch(s)) return s;
            try
            {
                byte[] raw = Latin1.GetBytes(s);
                string recovered = Encoding.UTF8.GetString(raw);
                if (recovered.Contains("\uFFFD")) return s;
                return recovered;
            }
            catch { return s; }
        }

        private static System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>> ToList(DataTable dt)
        {
            var list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    var val = row[col] == DBNull.Value ? null : row[col];
                    if (val is string strVal)
                        val = FixUtf8(strVal);
                    dict[col.ColumnName] = val;
                }
                list.Add(dict);
            }
            return list;
        }

        [HttpGet]
        [Route("GetOverallCapacity")]
        public IHttpActionResult GetOverallCapacity()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetOverallCapacity())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        
        [Route("GetDistinctMaterialCount")]
        public IHttpActionResult GetDistinctMaterialCount()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetDistinctMaterialCount())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetCustomers")]
        public IHttpActionResult GetCustomers()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetCustomers())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetVatTuTheoKhachHang")]
        public IHttpActionResult GetVatTuTheoKhachHang(string maKH = "")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuTheoKhachHang(maKH))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetRacks")]
        public IHttpActionResult GetRacks()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetRacks())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

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

        [HttpGet]
        [Route("GetFlowTrend12T")]
        public IHttpActionResult GetFlowTrend12T()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetFlowTrend12T())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetFlowTrendWeekly")]
        public IHttpActionResult GetFlowTrendWeekly()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetFlowTrendWeekly())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
        [HttpGet]
        [Route("GetAgeStock")]
        public IHttpActionResult GetAgeStock()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetAgeStock())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetActivityCalendar")]
        public IHttpActionResult GetActivityCalendar(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                if (tuNgay.HasValue && denNgay.HasValue)
                    return Ok(ToList(DashboardKhoDesktopModel.GetActivityCalendar(tuNgay.Value, denNgay.Value)));
                return Ok(ToList(DashboardKhoDesktopModel.GetActivityCalendar()));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
        [HttpGet]
        [Route("GetFlowTrendByRange")]
        public IHttpActionResult GetFlowTrendByRange(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                DateTime to   = (denNgay ?? DateTime.Today).Date;
                DateTime from = (tuNgay  ?? to.AddDays(-30)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetFlowTrendByRange(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetRackSlotDetail")]
        public IHttpActionResult GetRackSlotDetail()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetRackSlotDetail())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetActivityRangeDetail")]
        public IHttpActionResult GetActivityRangeDetail(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                return Ok(new
                {
                    Nhap   = ToList(DashboardKhoDesktopModel.GetNhapDetailByRange(tuNgay, denNgay)),
                    Xuat   = ToList(DashboardKhoDesktopModel.GetXuatDetailByRange(tuNgay, denNgay)),
                    KiemKe = ToList(DashboardKhoDesktopModel.GetKiemKeDetailByRange(tuNgay, denNgay))
                });
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetThanhGiaHangTon")]
        public IHttpActionResult GetThanhGiaHangTon()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetThanhGiaHangTon())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetAllMaterialsInStock")]
        public IHttpActionResult GetAllMaterialsInStock()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetAllMaterialsInStock())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetNKDuKienByRange")]
        public IHttpActionResult GetNKDuKienByRange(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime from = (tuNgay  ?? today.AddDays(-30)).Date;
                DateTime to   = (denNgay ?? today.AddDays(60)).Date;
                return Ok(ToList(DashboardKhoDesktopModel.GetNKDuKienByRange(from, to)));
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("ClearCache")]
        public IHttpActionResult ClearCache()
        {
            try
            {
                DashboardKhoDesktopModel.ClearCache();
                return Ok(new { Message = "Cache cleared", At = DateTime.Now });
            }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

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

        [HttpGet]
        [Route("GetMoMComparison")]
        public IHttpActionResult GetMoMComparison()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetMoMComparison())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
        [HttpGet]
        [Route("GetTop5")]
        public IHttpActionResult GetTop5(int isNhieuNhat = 1, int loaiNPL = 0)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTop5(isNhieuNhat, loaiNPL))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GlobalSearch")]
        public IHttpActionResult GlobalSearch(string itemcode = "")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GlobalSearchByItemcode(itemcode ?? string.Empty))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GlobalSearchAll")]
        public IHttpActionResult GlobalSearchAll(string keyword = "")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GlobalSearchAll(keyword ?? string.Empty))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }


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

        [HttpGet]
        [Route("GetTop5VatTuDungTich")]
        public IHttpActionResult GetTop5VatTuDungTich()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTop5VatTuDungTich())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetTop5KhachHangTonKho")]
        public IHttpActionResult GetTop5KhachHangTonKho()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTop5KhachHangTonKho())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetVatTuSapHetHan")]
        public IHttpActionResult GetVatTuSapHetHan()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuSapHetHan())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }  
        }

        [HttpGet]
        [Route("GetGiaTriTonKhoTheoNhom")]
        public IHttpActionResult GetGiaTriTonKhoTheoNhom()
        { 
            try { return Ok(ToList(DashboardKhoDesktopModel.GetGiaTriTonKhoTheoNhom())); } 
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
         
        [HttpGet]
        [Route("GetTinhHinhKiemKe")]
        public IHttpActionResult GetTinhHinhKiemKe() 
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTinhHinhKiemKe())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }
 

        [HttpGet]
        [Route("GetTodoDetail")]
        public IHttpActionResult GetTodoDetail(string type = "itemcode_cho_nk")   
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTodoDetail(type))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetVatTuTheoDungTich")] 
        public IHttpActionResult GetVatTuTheoDungTich() 
        { 
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuTheoDungTich())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetKhachHangTonKhoChiTiet")]
        public IHttpActionResult GetKhachHangTonKhoChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetKhachHangTonKhoChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetVatTuSapHetHanChiTiet")]
        public IHttpActionResult GetVatTuSapHetHanChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetVatTuSapHetHanChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetGiaTriNhomChiTiet")]
        public IHttpActionResult GetGiaTriNhomChiTiet()
        {  
            try { return Ok(ToList(DashboardKhoDesktopModel.GetGiaTriNhomChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetKiemKeChiTiet")]
        public IHttpActionResult GetKiemKeChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetKiemKeChiTiet())); } 
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); } 
        } 


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

        [HttpGet]
        [Route("GetTonKho")]
        public IHttpActionResult GetTonKho(DateTime? denNgay = null)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTonKho((denNgay ?? DateTime.Today).Date))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetTonDauKy")]
        public IHttpActionResult GetTonDauKy(DateTime? tuNgay = null)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTonDauKy((tuNgay ?? DateTime.Today.AddDays(-30)).Date))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetPODangTre")]
        public IHttpActionResult GetPODangTre()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetPODangTre())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetGiaTriTon")]
        public IHttpActionResult GetGiaTriTon(DateTime? denNgay = null)
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetGiaTriTon((denNgay ?? DateTime.Today).Date))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetCanhBaoTonKho")]
        public IHttpActionResult GetCanhBaoTonKho()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetCanhBaoTonKho())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

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

        [HttpGet]
        [Route("GetPODangTreChiTiet")]
        public IHttpActionResult GetPODangTreChiTiet(string groupBy = "all")
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetPODangTreChiTiet(groupBy))); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }



        [HttpGet]
        [Route("GetNPLThieuChiTiet")]
        public IHttpActionResult GetNPLThieuChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetNPLThieuChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }

        [HttpGet]
        [Route("GetKiemKeLechChiTiet")]
        public IHttpActionResult GetKiemKeLechChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetKiemKeLechChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }



        [HttpGet]
        [Route("GetTonVuotDinhMucChiTiet")]
        public IHttpActionResult GetTonVuotDinhMucChiTiet()
        {
            try { return Ok(ToList(DashboardKhoDesktopModel.GetTonVuotDinhMucChiTiet())); }
            catch (Exception ex) { return BadRequest("Error: " + ex.Message); }
        }



        // ===================================================================
        // GET api/DashboardKhoDesktop/LichPhanCong_GetCalendarMonth
        // ===================================================================
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

                // Doc raw items tu SP
                System.Collections.Generic.List<LichPhanCong_CalendarItemModel> rawItems = new System.Collections.Generic.List<LichPhanCong_CalendarItemModel>();
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["strCnn_ln"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SP_LICH_PHAN_CONG_PHU_LIEU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@Action", "GetCalendarMonth");
                    cmd.Parameters.AddWithValue("@TuNgay", tuDate);
                    cmd.Parameters.AddWithValue("@DenNgay", denDate);

                    conn.Open();
                    System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LichPhanCong_CalendarItemModel item = new LichPhanCong_CalendarItemModel();

                        item.NgayLam = reader["NgayLam"] != DBNull.Value ? Convert.ToDateTime(reader["NgayLam"]).ToString("yyyy-MM-dd") : "";
                        item.MaLenhSX = reader["MaLenhSX"] != DBNull.Value ? reader["MaLenhSX"].ToString() : "";
                        item.TrangThai = reader["TrangThai"] != DBNull.Value ? Convert.ToInt32(reader["TrangThai"]) : 0;
                        item.TenNV = reader["TenNV"] != DBNull.Value ? reader["TenNV"].ToString() : "";
                        item.MaNV = reader["MaNV"] != DBNull.Value ? reader["MaNV"].ToString() : "";
                        item.MaKhachHang = reader["MaKhachHang"] != DBNull.Value ? reader["MaKhachHang"].ToString() : "";
                        item.TenBrand = reader["TenBrand"] != DBNull.Value ? reader["TenBrand"].ToString() : "";
                        item.CoCanhBao = reader["CoCanhBao"] != DBNull.Value && Convert.ToInt32(reader["CoCanhBao"]) == 1;
                        item.ThieuNPL = reader["ThieuNPL"] != DBNull.Value && Convert.ToInt32(reader["ThieuNPL"]) == 1;
                        item.GhiChu = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : "";
                        rawItems.Add(item);
                    }
                    reader.Close();
                }

                // Gom nhom theo ngay bang Dictionary (khong dung LINQ)
                System.Collections.Generic.Dictionary<string, LichPhanCong_CalendarDayModel> dict =
                    new System.Collections.Generic.Dictionary<string, LichPhanCong_CalendarDayModel>();

                foreach (LichPhanCong_CalendarItemModel item in rawItems)
                {
                    string key = item.NgayLam;
                    if (string.IsNullOrEmpty(key)) continue;

                    if (!dict.ContainsKey(key))
                    {
                        LichPhanCong_CalendarDayModel day = new LichPhanCong_CalendarDayModel();
                        day.NgayLam = key;
                        day.Workers = new System.Collections.Generic.List<string>();
                        day.Tasks = new System.Collections.Generic.List<LichPhanCong_TaskBadgeModel>();
                        day.HasAlert = false;
                        day.ThieuNPL = false;
                        dict[key] = day;
                    }

                    LichPhanCong_CalendarDayModel cur = dict[key];

                    // Them NV neu chua co
                    if (!string.IsNullOrEmpty(item.TenNV) && !cur.Workers.Contains(item.TenNV))
                        cur.Workers.Add(item.TenNV);

                    // Them task neu co ma lenh
                    if (!string.IsNullOrEmpty(item.MaLenhSX))
                    {
                        LichPhanCong_TaskBadgeModel badge = new LichPhanCong_TaskBadgeModel();
                        badge.MaLenhSX = item.MaLenhSX;
                        badge.TrangThai = item.TrangThai;
                        badge.TenNV = item.TenNV;
                        badge.TenBrand = item.TenBrand;
                        badge.MaKhachHang = item.MaKhachHang;
                        cur.Tasks.Add(badge);
                    }

                    if (item.CoCanhBao) cur.HasAlert = true;
                    if (item.ThieuNPL) cur.ThieuNPL = true;
                }

                // Chuyen sang List va sap xep theo ngay
                System.Collections.Generic.List<LichPhanCong_CalendarDayModel> result = new System.Collections.Generic.List<LichPhanCong_CalendarDayModel>();
                foreach (LichPhanCong_CalendarDayModel day in dict.Values)
                    result.Add(day);

                result.Sort(delegate (LichPhanCong_CalendarDayModel a, LichPhanCong_CalendarDayModel b)
                {
                    return string.Compare(a.NgayLam, b.NgayLam, StringComparison.Ordinal);
                });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ===================================================================
        // GET api/DashboardKhoDesktop/LichPhanCong_GetDayDetail
        // ===================================================================
        [HttpGet, Route("LichPhanCong_GetDayDetail")]
        public IHttpActionResult GetDayDetail(string ngay = null)
        {
            try
            {
                DateTime ngayDate = string.IsNullOrEmpty(ngay) ? DateTime.Today : DateTime.Parse(ngay);

                LichPhanCong_DayDetailModel result = new LichPhanCong_DayDetailModel();
                result.NgayLam = ngayDate.ToString("yyyy-MM-dd");
                result.Assignments = new System.Collections.Generic.List<LichPhanCong_AssignmentModel>();
                result.PickOrders = new System.Collections.Generic.List<LichPhanCong_PickOrderModel>();

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["strCnn_ln"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SP_LICH_PHAN_CONG_PHU_LIEU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@Action", "GetDayDetail");
                    cmd.Parameters.AddWithValue("@Ngay", ngayDate);

                    conn.Open();
                    System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();

                    // Helper: đọc cột an toàn
                    Func<System.Data.SqlClient.SqlDataReader, string, string> safeStr = (r, col) => {
                        try { return r[col] != DBNull.Value ? r[col].ToString() : ""; }
                        catch { return ""; }
                    };
                    Func<System.Data.SqlClient.SqlDataReader, string, int> safeInt = (r, col) => {
                        try { return r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : 0; }
                        catch { return 0; }
                    };
                    Func<System.Data.SqlClient.SqlDataReader, string, double> safeDbl = (r, col) => {
                        try { return r[col] != DBNull.Value ? Convert.ToDouble(r[col]) : 0; }
                        catch { return 0; }
                    };
                    Func<System.Data.SqlClient.SqlDataReader, string, bool> safeBool = (r, col) => {
                        try { return r[col] != DBNull.Value && Convert.ToInt32(r[col]) == 1; }
                        catch { return false; }
                    };

                    // ResultSet 1: Assignments
                    while (reader.Read())
                    {
                        LichPhanCong_AssignmentModel a = new LichPhanCong_AssignmentModel();
                        a.MaLenhSX = safeStr(reader, "MaLenhSX");
                        a.TrangThai = safeInt(reader, "TrangThai");
                        a.TenNV = safeStr(reader, "TenNV");
                        a.MaNV = safeStr(reader, "MaNV");
                        a.MaKhachHang = safeStr(reader, "MaKhachHang");
                        a.TenBrand = safeStr(reader, "TenBrand");
                        a.NgayThucHien = safeStr(reader, "NgayThucHien");
                        a.GioThucHien = safeStr(reader, "GioThucHien");
                        a.MoTaCongViec = safeStr(reader, "MoTaCongViec");
                        a.ThieuNPL = safeBool(reader, "ThieuNPL");
                        a.GhiChu = safeStr(reader, "GhiChu");
                        result.Assignments.Add(a);
                    }

                    // ResultSet 2: Pick Orders
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            LichPhanCong_PickOrderModel po = new LichPhanCong_PickOrderModel();
                            po.MaLenhSX = safeStr(reader, "MaLenhSX");
                            po.TrangThai = safeInt(reader, "TrangThai");
                            po.TenNV = safeStr(reader, "TenNV");
                            po.MaNV = safeStr(reader, "MaNV");
                            po.MaKhachHang = safeStr(reader, "MaKhachHang");
                            po.TenBrand = safeStr(reader, "TenBrand");
                            po.NgaySoan = safeStr(reader, "NgaySoan");
                            po.GioSoan = safeStr(reader, "GioSoan");
                            po.SoLoaiPL = safeInt(reader, "SoLoaiPL");
                            po.TongSLCanSoan = safeDbl(reader, "TongSLCanSoan");
                            po.SoPLThieu = safeInt(reader, "SoPLThieu");
                            po.GhiChu = safeStr(reader, "GhiChu");
                            po.Items = new System.Collections.Generic.List<LichPhanCong_PickItemModel>();
                            result.PickOrders.Add(po);
                        }
                    }
                    reader.Close();
                }
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message, detail = ex.GetType().Name });
            }
        }

        // ===================================================================
        // POST api/DashboardKhoDesktop/LichPhanCong_SavePhanCong
        // Lưu (tạo mới hoặc cập nhật) một phân công nhân viên cho lệnh SX
        // (Van's code — merged 2026-06-05)
        // ===================================================================
        [HttpPost]
        [Route("LichPhanCong_SavePhanCong")]
        public IHttpActionResult SavePhanCong([FromBody] NtbSoft.ERP.Model.DashboardKho.LichPhanCong_SavePhanCongRequest req)
        {
            if (req == null)
                return BadRequest("Dữ liệu không hợp lệ.");
            if (string.IsNullOrWhiteSpace(req.MaLenhSX))
                return BadRequest("MaLenhSX không được để trống.");
            if (string.IsNullOrWhiteSpace(req.MaNV))
                return BadRequest("MaNV không được để trống.");

            try
            {
                string connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["strCnn_ln"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection conn =
                    new System.Data.SqlClient.SqlConnection(connectionString))
                using (System.Data.SqlClient.SqlCommand cmd =
                    new System.Data.SqlClient.SqlCommand("SP_LICH_PHAN_CONG_PHU_LIEU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@Action",       "SavePhanCong");
                    cmd.Parameters.AddWithValue("@MaLenhSX",    req.MaLenhSX.Trim());
                    cmd.Parameters.AddWithValue("@MaNV",         req.MaNV.Trim());
                    cmd.Parameters.AddWithValue("@NgayThucHien", req.NgayThucHien.Date);
                    cmd.Parameters.AddWithValue("@GhiChu",
                        string.IsNullOrWhiteSpace(req.GhiChu) ? (object)DBNull.Value : req.GhiChu.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new NtbSoft.ERP.Model.DashboardKho.LichPhanCong_SaveResult
                {
                    Success  = true,
                    MaLenhSX = req.MaLenhSX
                });
            }
            catch (Exception ex)
            {
                return Ok(new NtbSoft.ERP.Model.DashboardKho.LichPhanCong_SaveResult
                {
                    Success = false,
                    Error   = ex.Message
                });
            }
        }

        // ===================================================================
        // POST api/DashboardKhoDesktop/LichPhanCong_UpdateTrangThai
        // Cập nhật trạng thái lệnh SX: 0=Chờ, 1=Đang, 2=Hoàn thành, 3=Chưa HT
        // (Van's code — merged 2026-06-05)
        // ===================================================================
        [HttpPost]
        [Route("LichPhanCong_UpdateTrangThai")]
        public IHttpActionResult UpdateTrangThai([FromBody] NtbSoft.ERP.Model.DashboardKho.LichPhanCong_UpdateTrangThaiRequest req)
        {
            if (req == null)
                return BadRequest("Dữ liệu không hợp lệ.");
            if (string.IsNullOrWhiteSpace(req.MaLenhSX))
                return BadRequest("MaLenhSX không được để trống.");

            try
            {
                string connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["strCnn_ln"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection conn =
                    new System.Data.SqlClient.SqlConnection(connectionString))
                using (System.Data.SqlClient.SqlCommand cmd =
                    new System.Data.SqlClient.SqlCommand("SP_LICH_PHAN_CONG_PHU_LIEU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@Action",    "UpdateTrangThai");
                    cmd.Parameters.AddWithValue("@MaLenhSX",  req.MaLenhSX.Trim());
                    cmd.Parameters.AddWithValue("@TrangThai", req.TrangThai);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Ok(new NtbSoft.ERP.Model.DashboardKho.LichPhanCong_SaveResult
                {
                    Success  = true,
                    MaLenhSX = req.MaLenhSX
                });
            }
            catch (Exception ex)
            {
                return Ok(new NtbSoft.ERP.Model.DashboardKho.LichPhanCong_SaveResult
                {
                    Success = false,
                    Error   = ex.Message
                });
            }
        }

        // ===================================================================
        // GET api/DashboardKhoDesktop/LichPhanCong_GetPickOrderDetail
        // Lấy chi tiết các phụ liệu cần soạn cho một lệnh sản xuất
        // (Van's code — merged 2026-06-05)
        // ===================================================================
        [HttpGet]
        [Route("LichPhanCong_GetPickOrderDetail")]
        public IHttpActionResult GetPickOrderDetail(string maLenhSX)
        {
            if (string.IsNullOrWhiteSpace(maLenhSX))
                return BadRequest("MaLenhSX không được để trống.");

            try
            {
                var result = new System.Collections.Generic.List<NtbSoft.ERP.Model.DashboardKho.LichPhanCong_PickItemModel>();
                string connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["strCnn_ln"].ConnectionString;

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SP_LICH_PHAN_CONG_PHU_LIEU", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@Action", "GetPickOrderDetail");
                    cmd.Parameters.AddWithValue("@MaLenhSX", maLenhSX.Trim());

                    conn.Open();
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Func<System.Data.SqlClient.SqlDataReader, string, string> safeStr = (r, col) => {
                            try { return r[col] != DBNull.Value ? FixUtf8(r[col].ToString()) : ""; }
                            catch { return ""; }
                        };
                        Func<System.Data.SqlClient.SqlDataReader, string, int> safeInt = (r, col) => {
                            try { return r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : 0; }
                            catch { return 0; }
                        };
                        Func<System.Data.SqlClient.SqlDataReader, string, double> safeDbl = (r, col) => {
                            try { return r[col] != DBNull.Value ? Convert.ToDouble(r[col]) : 0; }
                            catch { return 0; }
                        };
                        Func<System.Data.SqlClient.SqlDataReader, string, bool> safeBool = (r, col) => {
                            try { return r[col] != DBNull.Value && Convert.ToInt32(r[col]) == 1; }
                            catch { return false; }
                        };

                        while (reader.Read())
                        {
                            var item = new NtbSoft.ERP.Model.DashboardKho.LichPhanCong_PickItemModel();
                            item.ID = safeInt(reader, "ID");
                            item.MaLenhSX = safeStr(reader, "MaLenhSX");
                            item.MaNPL = safeStr(reader, "MaNPL");
                            item.TenNPL = safeStr(reader, "TenNPL");
                            item.SLCanSoan = safeDbl(reader, "SLCanSoan");
                            item.SLTonKho = safeDbl(reader, "SLTonKho");
                            item.SLDaSoan = safeDbl(reader, "SLDaSoan");
                            item.DonVi = safeStr(reader, "DonVi");
                            item.MaViTri = safeStr(reader, "MaViTri");
                            item.ThieuHang = safeBool(reader, "ThieuHang");
                            item.GhiChu = safeStr(reader, "GhiChu");
                            result.Add(item);
                        }
                    }
                }
                return Ok(new { success = true, data = result });
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
        [HttpGet]
        [Route("LichPhanCong_GetNhanVienList")]
        public IHttpActionResult GetNhanVienList()
        {
            try
            {
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
                            try { return r[col] != DBNull.Value ? FixUtf8(r[col].ToString()) : ""; }
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
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message, detail = ex.GetType().Name });
            }
        }
    }
}
