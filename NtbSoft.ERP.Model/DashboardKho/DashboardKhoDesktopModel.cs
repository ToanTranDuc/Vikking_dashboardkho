using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Model.DashboardKho
{

    public static class DashboardKhoDesktopModel
    {
        private const int CACHE_MINUTES = 5;
        private static readonly Dictionary<string, CachedEntry> _cache = new Dictionary<string, CachedEntry>();
        private static readonly object _cacheLock = new object();

        private class CachedEntry
        {
            public DataTable Data;
            public DateTime ExpiresAt;
            public bool IsRefreshing;
        }

        private static DataTable GetOrCache(string key, Func<DataTable> producer)
        {
            DataTable staleData = null;
            bool needsRefresh = false;

            lock (_cacheLock)
            {
                CachedEntry entry;
                if (_cache.TryGetValue(key, out entry))
                {
                    if (entry.ExpiresAt > DateTime.UtcNow)
                    {
                        return entry.Data;
                    }
                    else
                    {
                        staleData = entry.Data;
                        if (!entry.IsRefreshing)
                        {
                            entry.IsRefreshing = true;
                            needsRefresh = true;
                        }
                    }
                }
            }

            if (staleData != null)
            {
                if (needsRefresh)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            var newDt = producer();
                            lock (_cacheLock)
                            {
                                _cache[key] = new CachedEntry
                                {
                                    Data = newDt,
                                    ExpiresAt = DateTime.UtcNow.AddMinutes(CACHE_MINUTES),
                                    IsRefreshing = false
                                };
                            }
                        }
                        catch
                        {
                            lock (_cacheLock)
                            {
                                if (_cache.ContainsKey(key))
                                    _cache[key].IsRefreshing = false;
                            }
                        }
                    });
                }
                return staleData;
            }

            var dt = producer();
            lock (_cacheLock)
            {
                _cache[key] = new CachedEntry
                {
                    Data = dt,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(CACHE_MINUTES),
                    IsRefreshing = false
                };
            }
            return dt;
        }

        
        public static void ClearCache()
        {
            lock (_cacheLock) { _cache.Clear(); }
        }

        private static DataTable ExecuteSP(string action, Action<SqlCommand> paramBinder = null)
        {
            using (SqlConnection conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.usp_DashboardKhoDesktop", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar, 100).Value = action;
                if (paramBinder != null) paramBinder(cmd);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adt.Fill(dt);
                    return dt;
                }
            }
        }

        private static DataTable ExecuteQuery(string query, Action<SqlCommand> paramBinder = null)
        {
            using (SqlConnection conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;
                if (paramBinder != null) paramBinder(cmd);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adt.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetOverallCapacity()
        {
            return GetOrCache("dk_OverallCapacity", () => ExecuteSP("GetOverallCapacity"));
        }

        public static DataTable GetDistinctMaterialCount()
        {
            return ExecuteSP("GetDistinctMaterialCount");
        }

        public static DataTable GetCustomers()
        {
            return GetOrCache("dk_Customers", () => ExecuteSP("GetCustomers"));
        }

        public static DataTable GetRacks()
        {
            return GetOrCache("dk_Racks", () => ExecuteSP("GetRacks"));
        }

        public static DataTable GetChuanBiVe(DateTime tuNgay, DateTime denNgay, string keyword = "")
        {
            return ExecuteSP("GetChuanBiVe", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
                cmd.Parameters.AddWithValue("@Itemcode", keyword);
            });
        }

        /// <summary>
        /// v2.3.24 — Chuẩn bị xuất: thêm cột SoLuongYeuCau (SL chuẩn bị xuất từ CanDoiDonViSanXuat)
        /// </summary>
        public static DataTable GetChuanBiXuat(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetChuanBiXuat", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        /// <summary>
        /// v2.3.46 — Global search theo Itemcode (ERP_VatTuTV.MaVT).
        /// Trả về 1 row tổng hợp: số bản ghi & tổng SL ở mỗi section (tồn, nhập 30 ngày,
        /// xuất 30 ngày, kiểm kê 30 ngày, dự kiến tới 60 ngày).
        /// JS dùng kết quả để render danh sách section có khớp + cho phép navigate.
        /// </summary>
        public static DataTable GlobalSearchByItemcode(string itemcode)
        {
            return ExecuteSP("GlobalSearchByItemcode", cmd => {
                cmd.Parameters.Add("@Itemcode", SqlDbType.NVarChar, 200).Value = (object)(itemcode ?? string.Empty) ?? DBNull.Value;
            });
        }

        public static DataTable GlobalSearchAll(string keyword)
        {
            return ExecuteSP("GlobalSearchAll", cmd => {
                cmd.Parameters.Add("@Itemcode", SqlDbType.NVarChar, 200).Value = (object)(keyword ?? string.Empty) ?? DBNull.Value;
            });
        }

        /// <summary>
        /// v2.3.44 — Đang xuất với SL chính xác:
        ///   SL yêu cầu = SUM(ERP_PhieuDangKyXuatVT.SLDK) per MaLenhSX
        ///   SL đã xuất = SUM(PhieuXuatHang.SLNhap) WHERE PhieuYC = ERP_PhieuDangKyXuatVT.PhieuDK
        ///   % đã xuất = SL đã xuất / SL yêu cầu
        /// </summary>
        public static DataTable GetDangXuat(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetDangXuat", cmd => {
                cmd.Parameters.Add("@TuNgay", SqlDbType.Date).Value = tuNgay.Date;
                cmd.Parameters.Add("@DenNgay", SqlDbType.Date).Value = denNgay.Date;
                
            });
        }

        public static DataTable GetFlowTrend12T()
        {
            return ExecuteSP("GetFlowTrend12T");
        }

        public static DataTable GetFlowTrendWeekly()
        {
            return ExecuteSP("GetFlowTrendWeekly");
        } 

        public static DataTable GetAgeStock()
        {
            return GetOrCache("dk_AgeStock", () => 
            {
                var model = new NtbSoft.ERP.Model.QuanLyDonHang.TongHopKhoNPLModel();
                return model.Get("GETTILETONKHONHOMTHEOTHANG", "all", "all", "all", "", "", "", "", "", "", "");
            });
        }

        public static DataTable GetActivityCalendar()
        {
            DateTime today = DateTime.Today;
            return GetActivityCalendar(today.AddDays(-90), today.AddDays(30));
        }

        /// <summary>
        /// v2.3.30 — Danh sách TOÀN BỘ mã vật tư đang tồn kho (không giới hạn Top N).
        /// Dùng cho drill từ materialCount → "Số mã vật tư" (vd 2,348 mã).
        /// </summary>
        public static DataTable GetAllMaterialsInStock()
        {
            return GetOrCache("dk_AllMaterials", () => ExecuteSP("GetAllMaterialsInStock"));
        }

        /// <summary>
        /// v2.3.36 — Tính Thành giá hàng tồn = SUM(SoLuong × DonGia) cho toàn bộ vật tư đang tồn.
        /// Defensive: nếu không có bảng giá → trả 0.
        /// </summary>
        public static DataTable GetThanhGiaHangTon()
        {
            return GetOrCache("dk_ThanhGia", () => ExecuteSP("GetThanhGiaHangTon"));
        }

        /// <summary>
        /// v2.3.23 — Lấy NK dự kiến từ ERP_NhapKhoNPL.NgayNKDuKien theo khoảng ngày.
        /// Trả về: NgayNKDuKien (yyyy-MM-dd), SoLo, PO, MaKH, TenKH, SoLuongDuKien
        /// </summary>
        public static DataTable GetNKDuKienByRange(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetNKDuKienByRange", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        /// <summary>
        /// Chi tiết các record NHẬP KHO trong 1 ngày (v2.3.7).
        /// v2.3.8.2 — SoLo từ parent ERP_NhapKhoNPL (chi tiết chỉ có SoLoID).
        /// </summary>
        /// <summary>
        /// v2.3.43 — Chi tiết NHẬP KHO theo KHOẢNG NGÀY, GROUP BY SoLoID + MaNPL với SUM SoLuong.
        /// </summary>
        public static DataTable GetNhapDetailByRange(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetNhapDetailByRange", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        /// <summary>
        /// v2.3.45 — Chi tiết XUẤT KHO theo KHOẢNG NGÀY, GROUP BY MaLenh (integer).
        /// Cột: MaLenh, TenHang, TenKH, SoLuong, SoBarCode.
        /// </summary>
        public static DataTable GetXuatDetailByRange(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetXuatDetailByRange", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        /// <summary>
        /// v2.3.43 — Chi tiết KIỂM KÊ theo KHOẢNG NGÀY.
        /// </summary>
        public static DataTable GetKiemKeDetailByRange(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetKiemKeDetailByRange", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        /// <summary>
        /// v2.3.22 — Chi tiết NHẬP KHO 1 ngày, GROUP BY SoLoID + MaNPL.
        /// Cột: PI NCC (nk.SoLo), PO, MaNPL, Itemcode (vt.MaVT), Màu (MauVT),
        /// Width/Size (KhoVai + đơn vị từ ERP_DonViVT), Khách hàng, Số lượng (SUM).
        /// </summary>
        public static DataTable GetNhapDetailByDay(DateTime ngay)
        {
            return ExecuteSP("GetNhapDetailByDay", cmd => {
                cmd.Parameters.AddWithValue("@Ngay", ngay.Date);
            });
        }

        /// <summary>
        /// Chi tiết XUẤT KHO trong 1 ngày (v2.3.45) — GROUP BY MaLenh (integer lệnh).
        /// Cột: MaLenh, TenHang, TenKH, SoLuong (SUM), SoBarCode.
        /// </summary>
        public static DataTable GetXuatDetailByDay(DateTime ngay)
        {
            return ExecuteSP("GetXuatDetailByDay", cmd => {
                cmd.Parameters.AddWithValue("@Ngay", ngay.Date);
            });
        }

        /// <summary>
        /// Chi tiết KIỂM KÊ trong 1 ngày (v2.3.45) — GROUP BY PhieuKiemKe + SoLo.
        /// Cột: PhieuKiemKe, SoLo, SoBarCode, SoLuong (SUM), UserKK, GhiChu.
        /// </summary>
        public static DataTable GetKiemKeDetailByDay(DateTime ngay)
        {
            return ExecuteSP("GetKiemKeDetailByDay", cmd => {
                cmd.Parameters.AddWithValue("@Ngay", ngay.Date);
            });
        }

        /// <summary>
        /// Chi tiết lấp đầy ở cấp Ô (slot) — v2.3.9.
        /// Trả về danh sách ô đang chứa vật tư, kèm Tầng/Dãy/Kệ/Ô + khách hàng + mã NPL.
        /// Dùng cho phần "Chi tiết theo ô" trong modal Tổng sức chứa.
        /// </summary>
        public static DataTable GetRackSlotDetail()
        {
            return ExecuteSP("GetRackSlotDetail");
        }

        /// <summary>
        /// Lịch hoạt động kho — 4 mục: TotalIn (nhập), TotalOut (xuất),
        /// TotalKiemKe (kiểm kê từ ERPPhieuKiemKe_NPLV2), TotalActivity (tổng).
        /// v2.3.7 — dùng temp tables, ép kiểu DATE tường minh, fallback an toàn cho
        /// bảng/cột kiểm kê nếu môi trường không có (Issue 2 fix).
        /// </summary>
        public static DataTable GetActivityCalendar(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetActivityCalendar", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay",  tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
            });
        }

        /// <summary>
        /// Xuất - Nhập - Tồn theo khoảng ngày tùy chọn (gộp theo ngày).
        /// v2.3.5 — Issue 1: thay thế period selector tuần/tháng/quý/năm.
        /// </summary>
        public static DataTable GetFlowTrendByRange(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetFlowTrendByRange", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay",  tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        public static DataTable GetMoMComparison()
        {
            return ExecuteSP("GetMoMComparison");
        }

        public static DataTable GetTop5(int isNhieuNhat)
        {
            return GetTop5(isNhieuNhat, 0);
        }

        /// <summary>
        /// Top NL/PL theo số lượng tồn kho.
        /// loaiNPL: 0 = tất cả, 1 = chỉ NL (Module=1), 2 = chỉ PL (Module=2).
        /// Truy vấn trực tiếp ERP_VatTuCBM + ERP_ONPL để Module quyết định NL/PL,
        /// thay vì SP_PHIEUKHONPL (không có cột phân biệt NL/PL). Bug fix v2.3.5.
        /// </summary>
        public static DataTable GetTop5(int isNhieuNhat, int loaiNPL)
        {
            return GetOrCache("dk_Top5_" + isNhieuNhat + "_" + loaiNPL,
                () => ExecuteSP("GetTop5", cmd => {
                    cmd.Parameters.AddWithValue("@IsNhieuNhat", isNhieuNhat);
                    cmd.Parameters.AddWithValue("@LoaiNPL",     loaiNPL);
                }));
        }

        // ════════════════════════════════════════════════════════════════
        // v2.5.0 — Công việc chờ xử lý: SQL thật (2 mục: ItemCode chờ NK + KK chờ duyệt)
        // ════════════════════════════════════════════════════════════════

        public static DataTable GetCongViecChoXuLy(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteSP("GetCongViecChoXuLy", cmd => {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
            });
        }

        public static DataTable GetTop5VatTuDungTich()
        {
            return ExecuteQuery(@"
                SELECT TOP 5
                    ROW_NUMBER() OVER (ORDER BY SUM(v.CBM) DESC) AS STT,
                    vt.MaVT,
                    ISNULL(vt.ChiTiet, '') AS TenVT,
                    CASE WHEN ct.IsNPL = 1 THEN 'NL' ELSE 'PL' END AS LoaiKho,
                    ROUND(SUM(v.CBM), 2) AS CBM,
                    ROUND(SUM(v.CBM) / NULLIF((SELECT SUM(v2.CBM) FROM dbo.ERP_VatTuCBM v2 WHERE v2.MaONPL IS NOT NULL), 0) * 100, 2) AS TyTrong
                FROM dbo.ERP_VatTuCBM v
                INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON v.Barcode = ct.BarCode
                LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                WHERE v.MaONPL IS NOT NULL
                GROUP BY vt.MaVT, vt.ChiTiet, ct.IsNPL
                ORDER BY SUM(v.CBM) DESC;");
        }

        public static DataTable GetTop5KhachHangTonKho()
        {
            return ExecuteSP("GetTop5KhachHangTonKho");
        }

        public static DataTable GetVatTuSapHetHan()
        {
            return ExecuteQuery(@"
                SELECT TOP 5
                    vt.MaVT,
                    ISNULL(vt.ChiTiet, '') AS TenVT,
                    ct.NgayNhapKho AS NgayNhapKho,
                    DATEDIFF(DAY, ct.NgayNhapKho, GETDATE()) AS SoNgayTon,
                    SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho,
                    ISNULL(dv.TenDVVT, '') AS DonVi,
                    MAX(ts.NgayHetHan) AS NgayHetHan
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                LEFT JOIN dbo.ERP_KhoVai kv ON ct.KhoVaiID = kv.KhoVaiID
                LEFT JOIN dbo.ERP_DonViVT dv ON ct.MaDVVT = dv.MaDVVT
                LEFT JOIN dbo.ERPThongSoVatTu ts ON ct.MaVTID = ts.MaVTID AND ct.MauVTID = ts.MauVTID
                WHERE ct.NgayNhapKho IS NOT NULL
                  AND ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
                GROUP BY vt.MaVT, vt.ChiTiet, ct.NgayNhapKho, dv.TenDVVT
                ORDER BY DATEDIFF(DAY, ct.NgayNhapKho, GETDATE()) DESC;");
        }

        public static DataTable GetGiaTriTonKhoTheoNhom()
        {
            return ExecuteSP("GetGiaTriTonKhoTheoNhom");
        }

        public static DataTable GetTinhHinhKiemKe()
        {
            return ExecuteSP("GetTinhHinhKiemKe");
        }

        // ════════════════════════════════════════════════════════════════
        // v2.5.0 — Chi tiết công việc chờ xử lý: SQL thật
        // ════════════════════════════════════════════════════════════════

        public static DataTable GetTodoDetail(string type)
        {
            return ExecuteSP("GetTodoDetail", cmd => {
                cmd.Parameters.Add("@Itemcode", SqlDbType.NVarChar, 200)
                   .Value = (object)(type ?? "itemcode_cho_nk") ?? DBNull.Value;
            });
        }


        public static DataTable GetVatTuTheoDungTich()
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY SUM(v.CBM) DESC) AS STT,
                    vt.MaVT,
                    ISNULL(vt.ChiTiet, '') AS TenVT,
                    CASE WHEN ct.IsNPL = 1 THEN 'NL' ELSE 'PL' END AS LoaiKho,
                    ROUND(SUM(v.CBM), 2) AS CBM,
                    ROUND(SUM(v.CBM) / NULLIF((SELECT SUM(v2.CBM) FROM dbo.ERP_VatTuCBM v2 WHERE v2.MaONPL IS NOT NULL), 0) * 100, 2) AS TyTrong,
                    MAX(v.MaONPL) AS ViTriKe
                FROM dbo.ERP_VatTuCBM v
                INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON v.Barcode = ct.BarCode
                LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID

                WHERE v.MaONPL IS NOT NULL
                GROUP BY vt.MaVT, vt.ChiTiet, ct.IsNPL
                ORDER BY SUM(v.CBM) DESC;");
        }

        public static DataTable GetKhachHangTonKhoChiTiet()
        {
            return ExecuteSP("GetKhachHangTonKhoChiTiet");
        }

        public static DataTable GetVatTuTheoKhachHang(string maKH)
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) DESC) AS STT,
                    vt.MaVT AS ItemCode,
                    ISNULL(vt.ChiTiet, '') AS TenVT,
                    ISNULL(m.MaMauVT, '') AS Mau,
                    ISNULL(PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 1), '') AS KhoVai,
                    SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SoLuong,
                    ROUND(SUM(ISNULL(v.CBM, 0)), 4) AS TongCBM,
                    ISNULL(MAX(v.MaONPL), N'Chưa xếp kệ') AS ViTriKe
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                INNER JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                LEFT JOIN dbo.ERP_VatTuCBM v ON ct.BarCode = v.Barcode
                LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = ISNULL(PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 2), '')
                LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH OR nk.MaHang = kh.MaKH
                WHERE (nk.MaKH = @MaKH OR kh.TenKH = @MaKH OR nk.KhachHang = @MaKH)
                  AND ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
                GROUP BY vt.MaVT, vt.ChiTiet, ct.MaNPL, m.MaMauVT
                ORDER BY SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) DESC;",
                cmd => {
                    cmd.Parameters.AddWithValue("@MaKH", maKH ?? "");
                });
        }

        public static DataTable GetVatTuSapHetHanChiTiet()
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY DATEDIFF(DAY, ct.NgayNhapKho, GETDATE()) DESC) AS STT,
                    vt.MaVT,
                    ISNULL(vt.ChiTiet, '') AS TenVT,
                    CASE WHEN ct.IsNPL = 1 THEN 'NL' ELSE 'PL' END AS LoaiKho,
                    nk.SoLo AS Lo,
                    ct.NgayNhapKho AS NgaySX,
                    MAX(ts.NgayHetHan) AS NgayHetHan,
                    DATEDIFF(DAY, ct.NgayNhapKho, GETDATE()) AS SoNgayTon,
                    SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS TonKho,
                    ISNULL(dv.TenDVVT, '') AS DonVi,
                    MAX(cbm.MaONPL) AS ViTri
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                LEFT JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                LEFT JOIN dbo.ERP_VatTuCBM cbm ON ct.BarCode = cbm.Barcode
                LEFT JOIN dbo.ERP_KhoVai kv ON ct.KhoVaiID = kv.KhoVaiID
                LEFT JOIN dbo.ERP_DonViVT dv ON ct.MaDVVT = dv.MaDVVT
                LEFT JOIN dbo.ERPThongSoVatTu ts ON ct.MaVTID = ts.MaVTID AND ct.MauVTID = ts.MauVTID
                WHERE ct.NgayNhapKho IS NOT NULL
                  AND ISNULL(ct.SoLuongThucTeBanDau, 0) > 0
                GROUP BY vt.MaVT, vt.ChiTiet, ct.IsNPL, nk.SoLo, ct.NgayNhapKho, dv.TenDVVT
                ORDER BY DATEDIFF(DAY, ct.NgayNhapKho, GETDATE()) DESC;");
        }

        public static DataTable GetGiaTriNhomChiTiet()
        {
            return ExecuteSP("GetGiaTriNhomChiTiet");
        }

        // ════════════════════════════════════════════════════════════════
        // v2.4.6 — STUB cho dải KPI mới + filter ngày + Page 2 widget mới.
        // TODO: thay bằng SQL thật khi nghiệp vụ sẵn sàng.
        // ════════════════════════════════════════════════════════════════

        public static DataTable GetTongNhap(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteQuery(@"
                DECLARE @Duration INT = DATEDIFF(DAY, @TuNgay, @DenNgay) + 1;
                DECLARE @PrevTuNgay DATETIME = DATEADD(DAY, -@Duration, @TuNgay);
                DECLARE @PrevDenNgay DATETIME = DATEADD(DAY, -1, @TuNgay);

                DECLARE @Val INT, @PrevVal INT;

                SELECT @Val = COUNT(DISTINCT ct.MaVTID)
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay);

                SELECT @PrevVal = COUNT(DISTINCT ct.MaVTID)
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho >= @PrevTuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @PrevDenNgay);

                SELECT 
                    ISNULL(@Val, 0) AS Value,
                    CASE WHEN ISNULL(@PrevVal, 0) > 0 THEN ROUND((CAST(ISNULL(@Val, 0) AS DECIMAL(18,4)) - @PrevVal) / @PrevVal * 100, 2) ELSE 0 END AS Delta;",
                cmd => {
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                });
        }

        public static DataTable GetTongXuat(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteQuery(@"
                DECLARE @Duration INT = DATEDIFF(DAY, @TuNgay, @DenNgay) + 1;
                DECLARE @PrevTuNgay DATETIME = DATEADD(DAY, -@Duration, @TuNgay);
                DECLARE @PrevDenNgay DATETIME = DATEADD(DAY, -1, @TuNgay);

                DECLARE @Val INT, @PrevVal INT;

                SELECT @Val = COUNT(DISTINCT xh.MaVTID)
                FROM dbo.PhieuXuatHang xh
                WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @TuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay);

                SELECT @PrevVal = COUNT(DISTINCT xh.MaVTID)
                FROM dbo.PhieuXuatHang xh
                WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @PrevTuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @PrevDenNgay);

                SELECT 
                    ISNULL(@Val, 0) AS Value,
                    CASE WHEN ISNULL(@PrevVal, 0) > 0 THEN ROUND((CAST(ISNULL(@Val, 0) AS DECIMAL(18,4)) - @PrevVal) / @PrevVal * 100, 2) ELSE 0 END AS Delta;",
                cmd => {
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                });
        }

        public static DataTable GetTonKho(DateTime denNgay)
        {
            return ExecuteQuery(@"
                -- Calculate @Val (current distinct count)
                SELECT ct.MaVTID, SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SLNhap
                INTO #tempNhap_Val
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                GROUP BY ct.MaVTID;

                SELECT xh.MaVTID, SUM(ISNULL(xh.SLNhap, 0)) AS SLXuat
                INTO #tempXuat_Val
                FROM dbo.PhieuXuatHang xh
                WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                GROUP BY xh.MaVTID;

                SELECT xh.MaVTID, SUM(ISNULL(th.ThuHoi, 0)) AS SLThu
                INTO #tempThu_Val
                FROM dbo.PhieuThuHoiNPL th
                INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
                WHERE th.NgayTH < DATEADD(DAY, 1, @DenNgay)
                GROUP BY xh.MaVTID;

                DECLARE @Val INT;
                SELECT @Val = COUNT(DISTINCT n.MaVTID)
                FROM #tempNhap_Val n
                LEFT JOIN #tempXuat_Val x ON n.MaVTID = x.MaVTID
                LEFT JOIN #tempThu_Val t ON n.MaVTID = t.MaVTID
                WHERE (ISNULL(n.SLNhap,0) - ISNULL(x.SLXuat,0) + ISNULL(t.SLThu,0)) > 0;

                -- Calculate @PrevVal (prev distinct count 30 days ago)
                DECLARE @PrevDate DATETIME = DATEADD(DAY, -30, @DenNgay);

                SELECT ct.MaVTID, SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SLNhap
                INTO #tempNhap_Prev
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho < DATEADD(DAY, 1, @PrevDate)
                GROUP BY ct.MaVTID;

                SELECT xh.MaVTID, SUM(ISNULL(xh.SLNhap, 0)) AS SLXuat
                INTO #tempXuat_Prev
                FROM dbo.PhieuXuatHang xh
                WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang < DATEADD(DAY, 1, @PrevDate)
                GROUP BY xh.MaVTID;

                SELECT xh.MaVTID, SUM(ISNULL(th.ThuHoi, 0)) AS SLThu
                INTO #tempThu_Prev
                FROM dbo.PhieuThuHoiNPL th
                INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
                WHERE th.NgayTH < DATEADD(DAY, 1, @PrevDate)
                GROUP BY xh.MaVTID;

                DECLARE @PrevVal INT;
                SELECT @PrevVal = COUNT(DISTINCT n.MaVTID)
                FROM #tempNhap_Prev n
                LEFT JOIN #tempXuat_Prev x ON n.MaVTID = x.MaVTID
                LEFT JOIN #tempThu_Prev t ON n.MaVTID = t.MaVTID
                WHERE (ISNULL(n.SLNhap,0) - ISNULL(x.SLXuat,0) + ISNULL(t.SLThu,0)) > 0;

                SELECT 
                    ISNULL(@Val, 0) AS Value,
                    CASE WHEN ISNULL(@PrevVal, 0) > 0 THEN ROUND((CAST(ISNULL(@Val, 0) AS DECIMAL(18,4)) - @PrevVal) / @PrevVal * 100, 2) ELSE 0 END AS Delta;

                DROP TABLE #tempNhap_Val; DROP TABLE #tempXuat_Val; DROP TABLE #tempThu_Val;
                DROP TABLE #tempNhap_Prev; DROP TABLE #tempXuat_Prev; DROP TABLE #tempThu_Prev;",
                cmd => cmd.Parameters.AddWithValue("@DenNgay", denNgay));
        }

        public static DataTable GetTonDauKy(DateTime tuNgay)
        {
            return ExecuteQuery(@"
                SELECT ct.MaVTID, SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SLNhapDK
                INTO #tempNhap_TDK
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho < @TuNgay
                GROUP BY ct.MaVTID;

                SELECT xh.MaVTID, SUM(ISNULL(xh.SLNhap, 0)) AS SLXuatDK
                INTO #tempXuat_TDK
                FROM dbo.PhieuXuatHang xh
                WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang < @TuNgay
                GROUP BY xh.MaVTID;

                SELECT xh.MaVTID, SUM(ISNULL(th.ThuHoi, 0)) AS SLThuDK
                INTO #tempThu_TDK
                FROM dbo.PhieuThuHoiNPL th
                INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
                WHERE th.NgayTH < @TuNgay
                GROUP BY xh.MaVTID;

                SELECT COUNT(DISTINCT n.MaVTID) AS Value
                FROM #tempNhap_TDK n
                LEFT JOIN #tempXuat_TDK x ON n.MaVTID = x.MaVTID
                LEFT JOIN #tempThu_TDK t ON n.MaVTID = t.MaVTID
                WHERE (ISNULL(n.SLNhapDK,0) - ISNULL(x.SLXuatDK,0) + ISNULL(t.SLThuDK,0)) > 0;

                DROP TABLE #tempNhap_TDK; DROP TABLE #tempXuat_TDK; DROP TABLE #tempThu_TDK;",
                cmd => cmd.Parameters.AddWithValue("@TuNgay", tuNgay));
        }

        public static DataTable GetPODangTre()
        {
            return ExecuteQuery(@"
                DECLARE @Total INT = 0, @ChuaKiem INT = 0;

                SELECT @Total = COUNT(DISTINCT nk.SoLoID)
                FROM dbo.ERP_NhapKhoNPL nk
                WHERE nk.NgayNKDuKien IS NOT NULL AND nk.NgayNKDuKien < GETDATE()
                  AND EXISTS (SELECT 1 FROM dbo.ERP_ChiTietNhapKhoNPL ct WHERE ct.SoLoID = nk.SoLoID AND ISNULL(ct.IsDuyetNK, 0) <> 1);

                SELECT @ChuaKiem = COUNT(DISTINCT nk.SoLoID)
                FROM dbo.ERP_NhapKhoNPL nk
                WHERE nk.NgayNKDuKien IS NOT NULL AND nk.NgayNKDuKien < GETDATE()
                  AND EXISTS (SELECT 1 FROM dbo.ERP_ChiTietNhapKhoNPL ct WHERE ct.SoLoID = nk.SoLoID AND ISNULL(ct.IsDuyetNK, 0) <> 1)
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.QTY_KiemVaiV2 kv
                      WHERE kv.SoLoID = nk.SoLoID AND kv.DuyetQC = 1
                  )
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.Qty_KiemPL_XacNhan kp
                      WHERE kp.SoLoID = nk.SoLoID AND kp.Is_XN_SoLo = 1
                  );

                SELECT @Total AS SoPO, @ChuaKiem AS SoPOChuaKiem;");
        }

        public static DataTable GetGiaTriTon(DateTime denNgay)
        {
            return ExecuteQuery(@"
                DECLARE @Val DECIMAL(28,4), @PrevVal DECIMAL(28,4);
                DECLARE @PrevDate DATETIME = DATEADD(DAY, -30, @DenNgay);
                DECLARE @LimitDate DATETIME = DATEADD(DAY, 1, @DenNgay);
                DECLARE @LimitPrevDate DATETIME = DATEADD(DAY, 1, @PrevDate);

                ;WITH xh AS (SELECT BarCodeGoc, SUM(SLNhap) AS TotalXuat FROM dbo.PhieuXuatHang WHERE NgayXuatHang < @LimitDate GROUP BY BarCodeGoc),
                      th AS (SELECT BarCode, SUM(ThuHoi) AS TotalThuHoi FROM dbo.PhieuThuHoiNPL WHERE NgayTH < @LimitDate GROUP BY BarCode)
                SELECT @Val = SUM(t.TonKho * t.DonGia) FROM (
                    SELECT 
                        ct.DonGia,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) - ISNULL(MAX(xh.TotalXuat), 0) + ISNULL(MAX(th.TotalThuHoi), 0) AS TonKho
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN xh ON xh.BarCodeGoc = ct.BarCode
                    LEFT JOIN th ON th.BarCode = ct.BarCode
                    WHERE ct.NgayNhapKho < @LimitDate
                    GROUP BY ct.BarCode, ct.DonGia
                ) t WHERE t.TonKho > 0;

                ;WITH xh2 AS (SELECT BarCodeGoc, SUM(SLNhap) AS TotalXuat FROM dbo.PhieuXuatHang WHERE NgayXuatHang < @LimitPrevDate GROUP BY BarCodeGoc),
                      th2 AS (SELECT BarCode, SUM(ThuHoi) AS TotalThuHoi FROM dbo.PhieuThuHoiNPL WHERE NgayTH < @LimitPrevDate GROUP BY BarCode)
                SELECT @PrevVal = SUM(t.TonKho * t.DonGia) FROM (
                    SELECT 
                        ct.DonGia,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) - ISNULL(MAX(xh2.TotalXuat), 0) + ISNULL(MAX(th2.TotalThuHoi), 0) AS TonKho
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN xh2 ON xh2.BarCodeGoc = ct.BarCode
                    LEFT JOIN th2 ON th2.BarCode = ct.BarCode
                    WHERE ct.NgayNhapKho < @LimitPrevDate
                    GROUP BY ct.BarCode, ct.DonGia
                ) t WHERE t.TonKho > 0;

                SELECT 
                    ISNULL(@Val, 0) AS Value,
                    CASE WHEN ISNULL(@PrevVal, 0) > 0 THEN ROUND((ISNULL(@Val, 0) - @PrevVal) / @PrevVal * 100, 2) ELSE 0 END AS Delta;",
                cmd => cmd.Parameters.AddWithValue("@DenNgay", denNgay));
        }

        public static DataTable GetCanhBaoTonKho()
        {
            return ExecuteQuery(@"
                DECLARE @POTre INT = 0, @NPLThieu INT = 0, @KKLech INT = 0, @TonVuot INT = 0;

                -- PO trễ chưa kiểm
                SELECT @POTre = COUNT(DISTINCT nk.SoLoID)
                FROM dbo.ERP_NhapKhoNPL nk
                WHERE nk.NgayNKDuKien IS NOT NULL AND nk.NgayNKDuKien < GETDATE()
                  AND EXISTS (SELECT 1 FROM dbo.ERP_ChiTietNhapKhoNPL ct WHERE ct.SoLoID = nk.SoLoID AND ISNULL(ct.IsDuyetNK, 0) <> 1);

                -- NPL thiếu cho SX
                SELECT @NPLThieu = COUNT(DISTINCT cs.MaLenhSanXuat)
                FROM dbo.CanDoiDonViSanXuat cs
                WHERE NOT EXISTS (SELECT 1 FROM dbo.PhieuXuatHang ph WHERE ph.MaLenhSX = cs.MaLenhSanXuat);

                -- Kiểm kê lệch
                SELECT @KKLech = COUNT(*)
                FROM dbo.ERPPhieuKiemKe_NPL
                WHERE IsXacNhan = 1 AND SLKiemKeEdit IS NOT NULL
                  AND SLKiemKeBanDau <> ISNULL(SLKiemKeEdit, SLKiemKe);

                -- Tồn vượt định mức (so với ERP_VatTuMinmax.TonToiDa)
                IF OBJECT_ID('dbo.ERP_VatTuMinmax', 'U') IS NOT NULL
                BEGIN
                    SELECT @TonVuot = COUNT(*) FROM (
                        SELECT ct.MaVTID,
                               SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS Ton,
                               MAX(ISNULL(mm.TonToiDa, 0)) AS DinhMuc
                        FROM dbo.ERP_ChiTietNhapKhoNPL ct
                        LEFT JOIN dbo.ERP_VatTuMinmax mm ON mm.MaVTID = ct.MaVTID
                        WHERE ISNULL(mm.TonToiDa, 0) > 0
                        GROUP BY ct.MaVTID
                        HAVING SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) > MAX(ISNULL(mm.TonToiDa, 0))
                    ) t;
                END;

                SELECT 'po_tre' AS MaCB, N'PO đang trễ (chưa kiểm)' AS TenCB, @POTre AS SoLuong, N'PO' AS DonVi, N'Quá 48h chưa kiểm' AS MoTa, 'danger' AS MucDo
                UNION ALL
                SELECT 'npl_thieu', N'NPL thiếu cho sản xuất', @NPLThieu, N'mã hàng', N'Không đủ để cấp phát', 'danger'
                UNION ALL
                SELECT 'kk_lech', N'Kiểm kê lệch', @KKLech, N'phiếu', N'Cần kiểm tra lại', 'warn'
                UNION ALL
                SELECT 'ton_vuot_dm', N'Tồn kho vượt định mức', @TonVuot, N'mã hàng', N'Vượt mức tồn cho phép', 'danger';");
        }

        public static DataTable GetHieuSuatHoatDong(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteQuery(@"
                DECLARE @Val DECIMAL(18,1), @ValXuat DECIMAL(18,1), @ValKK DECIMAL(18,1), @ValDH DECIMAL(18,1);
                
                -- hoan_thanh_nhap
                SELECT @Val = CAST(ROUND(SUM(CASE WHEN ct.IsDuyetNK = 1 THEN 1.0 ELSE 0.0 END) / NULLIF(COUNT(*), 0) * 100, 1) AS DECIMAL(18,1))
                FROM dbo.ERP_ChiTietNhapKhoNPL ct
                WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay);

                -- hoan_thanh_xuat
                SELECT @ValXuat = CAST(ROUND(SUM(CASE WHEN t.TrangThai = 2 THEN 1.0 ELSE 0.0 END) / NULLIF(COUNT(*), 0) * 100, 1) AS DECIMAL(18,1))
                FROM dbo.ERP_LichPhanCongPhuLieu_Task t
                WHERE t.NgayThucHien >= @TuNgay AND t.NgayThucHien < DATEADD(DAY, 1, @DenNgay);

                -- kiem_ke_dung_han
                SELECT @ValKK = CAST(ROUND(SUM(ISNULL(p_check.DaKiemBit, 0)) * 100.0 / NULLIF(COUNT(*), 0), 1) AS DECIMAL(18,1))
                FROM dbo.ERP_DanhSachVatTuKiemKe ds
                OUTER APPLY (
                    SELECT TOP 1 1 AS DaKiemBit
                    FROM dbo.ERPPhieuKiemKe_NPL p
                    WHERE p.MaNPL = ds.MaNPL AND p.IsXacNhan = 1
                ) p_check
                WHERE ds.NgayTao >= @TuNgay AND ds.NgayTao < DATEADD(DAY, 1, @DenNgay);

                -- don_hang_dung_han
                SELECT @ValDH = CAST(ROUND(SUM(CASE WHEN ct.MaxNgay <= nk.NgayNKDuKien THEN 1.0 ELSE 0.0 END) / NULLIF(COUNT(*), 0) * 100, 1) AS DECIMAL(18,1))
                FROM dbo.ERP_NhapKhoNPL nk
                CROSS APPLY (
                    SELECT MAX(ct.NgayNhapKho) AS MaxNgay
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    WHERE ct.SoLoID = nk.SoLoID
                ) ct
                WHERE nk.NgayNKDuKien >= @TuNgay AND nk.NgayNKDuKien < DATEADD(DAY, 1, @DenNgay)
                  AND ct.MaxNgay IS NOT NULL;

                -- Return dataset
                SELECT 'hoan_thanh_nhap' AS MaChiSo, N'Tỷ lệ hoàn thành nhập' AS TenChiSo, ISNULL(@Val, 92.0) AS Value, CAST(8.0 AS DECIMAL(18,1)) AS Delta
                UNION ALL
                SELECT 'hoan_thanh_xuat', N'Tỷ lệ hoàn thành xuất', ISNULL(@ValXuat, 88.0), CAST(6.0 AS DECIMAL(18,1))
                UNION ALL
                SELECT 'kiem_ke_dung_han', N'Tỷ lệ kiểm kê đúng hạn', ISNULL(@ValKK, 95.0), CAST(5.0 AS DECIMAL(18,1))
                UNION ALL
                SELECT 'don_hang_dung_han', N'Tỷ lệ đơn hàng đúng hạn', ISNULL(@ValDH, 90.0), CAST(7.0 AS DECIMAL(18,1));",
                cmd => {
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                });
        }

        public static DataTable GetKiemKeChiTiet()
        {
            return ExecuteSP("GetKiemKeChiTiet");
        }

        // ════════════════════════════════════════════════════════════════
        // v2.4.15 — STUB chi tiết cho 5 modal KPI header
        // TODO: thay bằng SQL thật khi nghiệp vụ sẵn sàng.
        // ════════════════════════════════════════════════════════════════

        public static DataTable GetTonDauKyChiTiet(DateTime tuNgay, string loai)
        {
            return ExecuteQuery(@"
                -- 1. Calculate SLTonDau and DonGia per MaNPL
                SELECT 
                    t.MaNPL, 
                    SUM(t.SL) AS SLTonDau,
                    MAX(t.DonGia) AS DonGia
                INTO #tempResult_TDK
                FROM (
                    SELECT ct.MaNPL, SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SL, MAX(ISNULL(ct.DonGia, 0)) AS DonGia 
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct 
                    WHERE ct.NgayNhapKho < @TuNgay
                    GROUP BY ct.MaNPL
                    
                    UNION ALL
                    
                    SELECT xh.MaNPL, -SUM(ISNULL(xh.SLNhap, 0)) AS SL, 0.0 AS DonGia 
                    FROM dbo.PhieuXuatHang xh 
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang < @TuNgay
                    GROUP BY xh.MaNPL
                    
                    UNION ALL
                    
                    SELECT xh.MaNPL, SUM(ISNULL(th.ThuHoi, 0)) AS SL, 0.0 AS DonGia 
                    FROM dbo.PhieuThuHoiNPL th
                    INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
                    WHERE th.NgayTH < @TuNgay
                    GROUP BY xh.MaNPL
                ) t
                GROUP BY t.MaNPL
                HAVING SUM(t.SL) > 0;

                -- 2. Parse MaNPL to get MaVTID, MauVTID, KhoVaiID
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY r.SLTonDau DESC) AS STT,
                    r.*,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
                INTO #tempParsed_TDK
                FROM #tempResult_TDK r
                WHERE (@Loai = 'all' 
                       OR (@Loai = 'nl' AND r.MaNPL LIKE '%NL%') 
                       OR (@Loai = 'pl' AND r.MaNPL NOT LIKE '%NL%'));

                -- 3. Join Metadata and Return
                SELECT 
                    p.STT,
                    p.MaNPL,
                    p.MaVTID AS ItemCode,
                    ISNULL(vt.ChiTiet, p.MaVTID) AS TenVT,
                    p.MauVTID AS Mau,
                    p.KhoVaiID AS KhoVai,
                    CASE WHEN p.MaCLVTID = 'NL' OR p.MaNPL LIKE '%NL%' THEN 'NL' ELSE 'PL' END AS LoaiKho,
                    ISNULL(dv.TenDVVT, '') AS DonVi,
                    p.SLTonDau,
                    p.DonGia,
                    CAST(p.SLTonDau * p.DonGia AS DECIMAL(18,2)) AS ThanhTien
                FROM #tempParsed_TDK p
                LEFT JOIN dbo.ERP_VatTuTV vt ON p.MaVTID = vt.MaVTID
                LEFT JOIN dbo.ERP_KhoVai kv ON p.KhoVaiID = kv.KhoVaiID
                LEFT JOIN dbo.ERP_DonViVT dv ON kv.MaDVVT = dv.MaDVVT
                ORDER BY p.SLTonDau DESC;

                DROP TABLE #tempResult_TDK; 
                DROP TABLE #tempParsed_TDK;",
                cmd => {
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@Loai", loai ?? "all");
                });
        }

        public static DataTable GetTongNhapChiTiet(DateTime tuNgay, DateTime denNgay, string groupBy)
        {
            var gb = (groupBy ?? "date").ToLowerInvariant();
            if (gb == "all")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY ct.NgayNhapKho DESC) AS STT,
                        ISNULL(nk.SoLo, '') AS PINCC,
                        ISNULL(ct.POMua, '') AS PO,
                        ISNULL(vt.MaVT, '') AS ItemCode,
                        ISNULL(m.MaMauVT, '') AS MaMauVT,
                        ISNULL(m.MauVT, '') AS MauVT,
                        ISNULL(k.KhoVai, '') AS WidthSize,
                        ISNULL(dv.TenDVVT, '') AS DonViVT,
                        ISNULL(NULLIF(kh.TenKH, ''), N'Khách trống') AS TenKH,
                        ISNULL(ct.SoLuongThucTeBanDau, 0) AS SoLuong,
                        1 AS SoBarCode,
                        CAST(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(ct.DonGia, 0) AS DECIMAL(18, 2)) AS GiaTri,
                        ct.NgayNhapKho
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                    LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH
                    LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                    LEFT JOIN dbo.ERP_MauVTTV m ON m.MauVTID = ISNULL(PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 2), '')
                    LEFT JOIN dbo.ERP_KhoVai k ON k.KhoVaiID = ISNULL(PARSENAME(REPLACE(ct.MaNPL, '@', '.'), 1), '')
                    LEFT JOIN dbo.ERP_DonViVT dv ON ct.MaDVVT = dv.MaDVVT
                    WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                    ORDER BY ct.NgayNhapKho DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else if (gb == "date")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY CAST(ct.NgayNhapKho AS DATE)) AS STT,
                        CAST(ct.NgayNhapKho AS DATE) AS Ngay,
                        COUNT(DISTINCT nk.SoLoID) AS SoPhieu,
                        COUNT(DISTINCT ct.MaNPL) AS SoVT,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SLNhap,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(ct.DonGia, 0)) AS GiaTri
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                    WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY CAST(ct.NgayNhapKho AS DATE)
                    ORDER BY Ngay;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else if (gb == "po")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) DESC) AS STT,
                        ct.POMua,
                        ISNULL(NULLIF(kh.TenKH, ''), N'Khách trống') AS NCC,
                        COUNT(DISTINCT ct.MaNPL) AS SoVT,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SL,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(ct.DonGia, 0)) AS GiaTri,
                        MIN(nk.NgayNKDuKien) AS NgayDuKien
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                    LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH
                    WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY ct.POMua, kh.TenKH
                    ORDER BY SL DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else if (gb == "ncc")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) DESC) AS STT,
                        ISNULL(NULLIF(kh.TenKH, ''), N'Khách trống') AS NCC,
                        COUNT(DISTINCT ct.POMua) AS SoPO,
                        COUNT(DISTINCT ct.MaNPL) AS SoVT,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SL,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(ct.DonGia, 0)) AS GiaTri
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                    LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH
                    WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY kh.TenKH
                    ORDER BY SL DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else // vt
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) DESC) AS STT,
                        vt.MaVT AS ItemCode,
                        ISNULL(vt.ChiTiet, vt.MaVT) AS TenVT,
                        CASE WHEN ct.IsNPL = 1 THEN 'NL' ELSE 'PL' END AS LoaiKho,
                        ISNULL(dv.TenDVVT, '') AS DonVi,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SLNhap,
                        COUNT(DISTINCT ct.POMua) AS SoPO,
                        MAX(ct.NgayNhapKho) AS LanNhapCuoi,
                        MAX(ct.DonGia) AS DonGia,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0) * ISNULL(ct.DonGia, 0)) AS GiaTri
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                    LEFT JOIN dbo.ERP_KhoVai kv ON ct.KhoVaiID = kv.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT dv ON ct.MaDVVT = dv.MaDVVT
                    WHERE ct.NgayNhapKho >= @TuNgay AND ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY vt.MaVT, vt.ChiTiet, ct.IsNPL, dv.TenDVVT
                    ORDER BY SLNhap DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
        }

        public static DataTable GetTongXuatChiTiet(DateTime tuNgay, DateTime denNgay, string groupBy)
        {
            var gb = (groupBy ?? "date").ToLowerInvariant();
            if (gb == "all")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY xh.NgayXuatHang DESC) AS STT,
                        CASE WHEN ISNULL(xh.MaGop, '') = '' THEN N'(Trống)' ELSE xh.MaGop END AS MaLenh,
                        CASE WHEN ISNULL(xh.TenHang, '') = '' THEN N'(Trống)' ELSE xh.TenHang END AS TenHang,
                        ISNULL(xh.MaKH, '') AS MaKH,
                        CASE 
                            WHEN ISNULL(kh.TenKH, '') <> '' THEN kh.TenKH
                            WHEN ISNULL(xh.TenKH, '') <> '' THEN xh.TenKH
                            WHEN ISNULL(xh.MaKH, '') <> '' THEN xh.MaKH
                            ELSE N'(Khách trống)'
                        END AS TenKH,
                        CASE WHEN xh.NPL = 1 THEN 'NL' ELSE 'PL' END AS LoaiKho,
                        ISNULL(xh.SLNhap, 0) AS SoLuong,
                        0 AS SoBarCode,
                        CAST(ISNULL(xh.SLNhap, 0) * ISNULL(ct.DonGia, 0) AS DECIMAL(18, 2)) AS GiaTri,
                        CAST(xh.NgayXuatHang AS DATE) AS NgayXuat
                    FROM dbo.PhieuXuatHang xh
                    LEFT JOIN dbo.KhachHang kh ON xh.MaKH = kh.MaKH
                    LEFT JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON xh.BarCodeGoc = ct.BarCode
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @TuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                    ORDER BY xh.NgayXuatHang DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else if (gb == "date")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY CAST(xh.NgayXuatHang AS DATE)) AS STT,
                        CAST(xh.NgayXuatHang AS DATE) AS Ngay,
                        COUNT(DISTINCT xh.MaGop) AS SoPhieu,
                        COUNT(DISTINCT xh.MaNPL) AS SoVT,
                        SUM(ISNULL(xh.SLNhap, 0)) AS SLXuat,
                        0 AS GiaTri
                    FROM dbo.PhieuXuatHang xh
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @TuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY CAST(xh.NgayXuatHang AS DATE)
                    ORDER BY Ngay;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else if (gb == "dh")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(xh.SLNhap, 0)) DESC) AS STT,
                        xh.MaGop AS MaDH,
                        ISNULL(kh.TenKH, '') AS KhachHang,
                        COUNT(DISTINCT xh.MaNPL) AS SoVT,
                        SUM(ISNULL(xh.SLNhap, 0)) AS SL,
                        SUM(ISNULL(xh.SLNhap, 0) * ISNULL(xh.DonGia, 0)) AS GiaTri,
                        MAX(xh.NgayXuatHang) AS NgayXuat
                    FROM dbo.PhieuXuatHang xh
                    LEFT JOIN dbo.DonHangTong dh ON xh.MaGop = dh.MaDH
                    LEFT JOIN dbo.KhachHang kh ON dh.MaKH = kh.MaKH
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @TuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY xh.MaGop, kh.TenKH
                    ORDER BY SL DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else if (gb == "kh")
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(xh.SLNhap, 0)) DESC) AS STT,
                        ISNULL(kh.TenKH, '') AS KhachHang,
                        ISNULL(kh.MaKH, '') AS MaKH,
                        COUNT(DISTINCT xh.MaGop) AS SoDH,
                        COUNT(DISTINCT xh.MaNPL) AS SoVT,
                        SUM(ISNULL(xh.SLNhap, 0)) AS SL,
                        SUM(ISNULL(xh.SLNhap, 0) * ISNULL(xh.DonGia, 0)) AS GiaTri
                    FROM dbo.PhieuXuatHang xh
                    LEFT JOIN dbo.DonHangTong dh ON xh.MaGop = dh.MaDH
                    LEFT JOIN dbo.KhachHang kh ON dh.MaKH = kh.MaKH
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @TuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY kh.TenKH, kh.MaKH
                    ORDER BY SL DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
            else // vt
            {
                return ExecuteQuery(@"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY SUM(ISNULL(xh.SLNhap, 0)) DESC) AS STT,
                        vt.MaVT AS ItemCode,
                        ISNULL(vt.ChiTiet, vt.MaVT) AS TenVT,
                        CASE WHEN xh.NPL = 1 THEN 'NL' ELSE 'PL' END AS LoaiKho,
                        ISNULL(dv.TenDVVT, '') AS DonVi,
                        SUM(ISNULL(xh.SLNhap, 0)) AS SLXuat,
                        COUNT(DISTINCT xh.MaGop) AS SoDH,
                        MAX(xh.NgayXuatHang) AS LanXuatCuoi,
                        MAX(xh.DonGia) AS DonGia,
                        SUM(ISNULL(xh.SLNhap, 0) * ISNULL(xh.DonGia, 0)) AS GiaTri
                    FROM dbo.PhieuXuatHang xh
                    LEFT JOIN dbo.ERP_VatTuTV vt ON xh.MaVTID = vt.MaVTID
                    LEFT JOIN dbo.ERP_KhoVai kv ON xh.KhoVaiID = kv.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT dv ON kv.MaDVVT = dv.MaDVVT
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang >= @TuNgay AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY vt.MaVT, vt.ChiTiet, xh.NPL, dv.TenDVVT
                    ORDER BY SLXuat DESC;",
                    cmd => {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    });
            }
        }

        public static DataTable GetTonKhoChiTiet(DateTime denNgay, string loai)
        {
            return ExecuteQuery(@"
                -- 1. Calculate SLTon and DonGia per MaNPL
                SELECT 
                    t.MaNPL, 
                    SUM(t.SL) AS SLTon,
                    MAX(t.DonGia) AS DonGia
                INTO #tempResult_TKC
                FROM (
                    SELECT ct.MaNPL, SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SL, MAX(ISNULL(ct.DonGia, 0)) AS DonGia 
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct 
                    WHERE ct.NgayNhapKho < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY ct.MaNPL
                    
                    UNION ALL
                    
                    SELECT xh.MaNPL, -SUM(ISNULL(xh.SLNhap, 0)) AS SL, 0.0 AS DonGia 
                    FROM dbo.PhieuXuatHang xh 
                    WHERE xh.ModuleXH = 1 AND xh.NgayXuatHang < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY xh.MaNPL
                    
                    UNION ALL
                    
                    SELECT xh.MaNPL, SUM(ISNULL(th.ThuHoi, 0)) AS SL, 0.0 AS DonGia 
                    FROM dbo.PhieuThuHoiNPL th
                    INNER JOIN dbo.PhieuXuatHang xh ON th.BarCode = xh.BarCode
                    WHERE th.NgayTH < DATEADD(DAY, 1, @DenNgay)
                    GROUP BY xh.MaNPL
                ) t
                GROUP BY t.MaNPL
                HAVING SUM(t.SL) > 0;

                -- 2. Parse
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY r.SLTon DESC) AS STT,
                    r.*,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 4), '') AS MaCLVTID,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 3), '') AS MaVTID,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 2), '') AS MauVTID,
                    ISNULL(PARSENAME(REPLACE(r.MaNPL, '@', '.'), 1), '') AS KhoVaiID
                INTO #tempParsed_TKC
                FROM #tempResult_TKC r
                WHERE (@Loai = 'all' 
                       OR (@Loai = 'nl' AND r.MaNPL LIKE '%NL%') 
                       OR (@Loai = 'pl' AND r.MaNPL NOT LIKE '%NL%')
                       OR @Loai = 'expired');

                -- 2b. v2.7.2: Pre-compute metadata lookups (thay the 5 correlated subqueries per row bang 2 combined OUTERS)
                SELECT
                    p.MaNPL,
                    ISNULL(pos.MaONPL, N'Chua xep ke') AS ViTriKe,
                    DATEADD(MONTH, 12, latest_ct.NgayNhapKho) AS HanDung,
                    ISNULL(latest_ct.POMua, '')               AS POMua,
                    ISNULL(latest_ct.SoLo,  '')               AS SoLo,
                    ISNULL(latest_ct.TenKH, '')               AS TenKH,
                    latest_ct.MaDVVT
                INTO #tempMeta_TKC
                FROM #tempParsed_TKC p
                OUTER APPLY (
                    SELECT TOP 1 
                        ct.NgayNhapKho,
                        ct.POMua,
                        nk.SoLo,
                        kh.TenKH,
                        ct.BarCode,
                        ct.MaDVVT
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_NhapKhoNPL nk ON ct.SoLoID = nk.SoLoID
                    LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH
                    WHERE ct.MaNPL = p.MaNPL
                    ORDER BY ct.NgayNhapKho DESC
                ) latest_ct
                OUTER APPLY (
                    SELECT TOP 1 v.MaONPL
                    FROM dbo.ERP_VatTuCBM v
                    WHERE v.Barcode = latest_ct.BarCode AND v.MaONPL IS NOT NULL
                ) pos;

                -- 3. Join Metadata and Return
                SELECT 
                    p.STT,
                    p.MaVTID AS ItemCode,
                    ISNULL(vt.ChiTiet, p.MaVTID) AS TenVT,
                    CASE WHEN p.MaCLVTID = 'NL' OR p.MaNPL LIKE '%NL%' THEN 'NL' ELSE 'PL' END AS LoaiKho,
                    ISNULL(dv.TenDVVT, '') AS DonVi,
                    p.SLTon,
                    m.ViTriKe,
                    m.HanDung,
                    m.POMua,
                    m.SoLo,
                    m.TenKH,
                    p.DonGia,
                    CAST(p.SLTon * p.DonGia AS DECIMAL(18,2)) AS ThanhTien
                FROM #tempParsed_TKC p
                INNER JOIN #tempMeta_TKC m ON m.MaNPL = p.MaNPL
                LEFT JOIN dbo.ERP_VatTuTV vt ON p.MaVTID = vt.MaVTID
                LEFT JOIN dbo.ERP_DonViVT dv ON m.MaDVVT = dv.MaDVVT
                ORDER BY p.SLTon DESC;

                DROP TABLE #tempResult_TKC; 
                DROP TABLE #tempParsed_TKC;
                DROP TABLE #tempMeta_TKC;",
                cmd => {
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    cmd.Parameters.AddWithValue("@Loai", loai ?? "all");
                });
        }

        // ════════════════════════════════════════════════════════════════
        // v2.4.16 — STUB chi tiết cho 4 cảnh báo tồn kho (po_tre tái sử dụng)
        // ════════════════════════════════════════════════════════════════

        public static DataTable GetNPLThieuChiTiet()
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY cs.SoLuong DESC) AS STT,
                    vt.MaVT AS ItemCode,
                    ISNULL(vt.ChiTiet, '') AS TenVT,
                    cs.MaLenhSanXuat AS LenhSX,
                    ISNULL(dv.TenDVVT, 'm') AS DonVi,
                    cs.SoLuong AS SLCan,
                    0.0 AS SLCo,
                    cs.SoLuong AS SLThieu
                FROM dbo.CanDoiDonViSanXuat cs
                LEFT JOIN dbo.ERP_VatTuTV vt ON cs.MaLenh = vt.MaVTID
                LEFT JOIN dbo.ERP_KhoVai kv ON cs.MaLenh = kv.KhoVaiID
                LEFT JOIN dbo.ERP_DonViVT dv ON kv.MaDVVT = dv.MaDVVT
                WHERE NOT EXISTS (SELECT 1 FROM dbo.PhieuXuatHang ph WHERE ph.MaLenhSX = cs.MaLenhSanXuat)
                ORDER BY cs.SoLuong DESC;");
        }
        //  

        public static DataTable GetKiemKeLechChiTiet()
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY ABS(t1.SLKiemKeBanDau - ISNULL(t1.SLKiemKeEdit, t1.SLKiemKe)) DESC) AS STT,
                    t1.PhieuKiemKe AS MaPhieu,
                    N'Kho NPL' AS KhuVuc,
                    ROUND(t1.SLKiemKe, 2) AS SLHeThong,
                    ROUND(t1.SLKiemKeBanDau, 2) AS SLThucKiem,
                    ROUND((t1.SLKiemKeBanDau - t1.SLKiemKe) / NULLIF(t1.SLKiemKe, 0) * 100, 2) AS LechPct,
                    ISNULL(t1.UserKK, '') AS NguoiPT
                FROM dbo.ERPPhieuKiemKe_NPL t1
                WHERE t1.IsXacNhan = 1 AND t1.SLKiemKeBanDau <> ISNULL(t1.SLKiemKeEdit, t1.SLKiemKe)
                ORDER BY ABS(t1.SLKiemKeBanDau - t1.SLKiemKe) DESC;");
        }

        public static DataTable GetQCQuaLauChiTiet()
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY nk.NgayNKDuKien) AS STT,
                    nk.POMua AS SoLo,
                    ct.MaNPL AS ItemCode,
                    ISNULL(kh.TenKH, '') AS NCC,
                    nk.NgayNKDuKien AS NgayVe,
                    DATEDIFF(DAY, nk.NgayNKDuKien, GETDATE()) AS SoNgayChoQC
                FROM dbo.ERP_NhapKhoNPL nk
                LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH
                INNER JOIN dbo.ERP_ChiTietNhapKhoNPL ct ON nk.SoLoID = ct.SoLoID
                WHERE nk.NgayNKDuKien IS NOT NULL AND nk.NgayNKDuKien < GETDATE()
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.QTY_KiemVaiV2 kv
                      WHERE kv.SoLoID = nk.SoLoID AND kv.DuyetQC = 1
                  )
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.Qty_KiemPL_XacNhan kp
                      WHERE kp.SoLoID = nk.SoLoID AND kp.Is_XN_SoLo = 1
                  )
                ORDER BY nk.NgayNKDuKien;");
        }

        public static DataTable GetTonVuotDinhMucChiTiet()
        {
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY t.SLTon DESC) AS STT,
                    t.ItemCode,
                    t.TenVT,
                    t.DonVi,
                    t.SLTon,
                    t.DinhMuc,
                    ROUND((t.SLTon - t.DinhMuc) / NULLIF(t.DinhMuc, 0) * 100, 2) AS VuotPct,
                    t.ViTriKe
                FROM (
                    SELECT 
                        ct.MaVTID,
                        vt.MaVT AS ItemCode,
                        ISNULL(vt.ChiTiet, '') AS TenVT,
                        ISNULL(dv.TenDVVT, '') AS DonVi,
                        SUM(ISNULL(ct.SoLuongThucTeBanDau, 0)) AS SLTon,
                        MAX(ISNULL(cbm.MaONPL, N'Chưa xếp kệ')) AS ViTriKe,
                        MAX(ISNULL(mm.TonToiDa, 0)) AS DinhMuc
                    FROM dbo.ERP_ChiTietNhapKhoNPL ct
                    LEFT JOIN dbo.ERP_VatTuTV vt ON ct.MaVTID = vt.MaVTID
                    LEFT JOIN dbo.ERP_VatTuCBM cbm ON ct.BarCode = cbm.Barcode
                    LEFT JOIN dbo.ERP_KhoVai kv ON ct.KhoVaiID = kv.KhoVaiID
                    LEFT JOIN dbo.ERP_DonViVT dv ON ct.MaDVVT = dv.MaDVVT
                    LEFT JOIN dbo.ERP_VatTuMinmax mm ON mm.MaVTID = ct.MaVTID
                    WHERE ISNULL(mm.TonToiDa, 0) > 0
                    GROUP BY ct.MaVTID, vt.MaVT, vt.ChiTiet, dv.TenDVVT
                ) t
                WHERE t.SLTon > t.DinhMuc
                ORDER BY t.SLTon DESC;");
        }

        public static DataTable GetPODangTreChiTiet(string groupBy)
        {
            var gb = (groupBy ?? "all").ToLowerInvariant();
            return ExecuteQuery(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY nk.NgayNKDuKien) AS STT,
                    nk.POMua,
                    ISNULL(kh.TenKH, '') AS NCC,
                    nk.MaHang,
                    nk.SoLo,
                    nk.SoChungTu,
                    nk.NguoiNhap,
                    nk.NgayNKDuKien,
                    GETDATE() AS NgayVeThucTe,
                    DATEDIFF(HOUR, nk.NgayNKDuKien, GETDATE()) AS SoGioTre,
                    (SELECT COUNT(*) FROM dbo.ERP_ChiTietNhapKhoNPL ct WHERE ct.SoLoID = nk.SoLoID) AS SoVT,
                    (SELECT SUM(ISNULL(ct.SLTong, 0)) FROM dbo.ERP_ChiTietNhapKhoNPL ct WHERE ct.SoLoID = nk.SoLoID) AS SL,
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM dbo.QTY_KiemVaiV2 kv WHERE kv.SoLoID = nk.SoLoID AND kv.DuyetQC = 1) THEN N'Đang QC'
                        ELSE N'Chưa QC'
                    END AS TrangThai,
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM dbo.QTY_KiemVaiV2 kv WHERE kv.SoLoID = nk.SoLoID AND kv.DuyetQC = 1) THEN 'dang_qc'
                        ELSE 'chua_qc'
                    END AS MaTT
                FROM dbo.ERP_NhapKhoNPL nk
                LEFT JOIN dbo.KhachHang kh ON nk.MaKH = kh.MaKH
                WHERE nk.NgayNKDuKien IS NOT NULL AND nk.NgayNKDuKien < GETDATE()
                  AND EXISTS (SELECT 1 FROM dbo.ERP_ChiTietNhapKhoNPL ct WHERE ct.SoLoID = nk.SoLoID AND ISNULL(ct.IsDuyetNK, 0) <> 1)
                  AND (@MaTT = 'all' 
                       OR (@MaTT = 'dang_qc' AND EXISTS (SELECT 1 FROM dbo.QTY_KiemVaiV2 kv WHERE kv.SoLoID = nk.SoLoID AND kv.DuyetQC = 1))
                       OR (@MaTT = 'chua_qc' AND NOT EXISTS (SELECT 1 FROM dbo.QTY_KiemVaiV2 kv WHERE kv.SoLoID = nk.SoLoID AND kv.DuyetQC = 1))
                      )
                ORDER BY nk.NgayNKDuKien;",
                cmd => cmd.Parameters.AddWithValue("@MaTT", gb));
        }
    }

    // ===================================================================
    // Model thong ke tong hop thang
    // ===================================================================
    public class LichPhanCong_MonthStatsModel
    {
        public int TongTask { get; set; }
        public int HoanThanh { get; set; }
        public int DangThucHien { get; set; }
        public int ChoThucHien { get; set; }
        public int ChuaHoanThanh { get; set; }
        public int SoNguoiThucHien { get; set; }
        public double PhanTramHoanThanh { get; set; }
    }

    // ===================================================================
    // Model mot o ngay tren lich (du lieu raw tu SP)
    // ===================================================================
    public class LichPhanCong_CalendarItemModel
    {
        public string NgayLam { get; set; }   // "2026-05-27"
        public string MaLenhSX { get; set; }
        public int TrangThai { get; set; }   // 0=Cho, 1=Dang, 2=HoanThanh, 3=ChuaHoanThanh
        public string TenNV { get; set; }
        public string MaNV { get; set; }
        public string MaKhachHang { get; set; }
        public string TenBrand { get; set; }
        public bool CoCanhBao { get; set; }
        public bool ThieuNPL { get; set; }
        public string GhiChu { get; set; }
    }

    // ===================================================================
    // Model gom nhom 1 ngay (gom nhom tu CalendarItemModel)
    // ===================================================================
    public class LichPhanCong_CalendarDayModel
    {
        public string NgayLam { get; set; }
        public System.Collections.Generic.List<string> Workers { get; set; } = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<LichPhanCong_TaskBadgeModel> Tasks { get; set; } = new System.Collections.Generic.List<LichPhanCong_TaskBadgeModel>();
        public bool HasAlert { get; set; }
        public bool ThieuNPL { get; set; }
    }

    public class LichPhanCong_TaskBadgeModel
    {
        public string MaLenhSX { get; set; }
        public int TrangThai { get; set; }
        public string TenNV { get; set; }
        public string TenBrand { get; set; }      // Loai hang / Brand
        public string MaKhachHang { get; set; }   // Khach hang
    }

    // ===================================================================
    // Model chi tiet 1 ngay (GetDayDetail)
    // ===================================================================
    public class LichPhanCong_DayDetailModel
    {
        public string NgayLam { get; set; }
        public System.Collections.Generic.List<LichPhanCong_AssignmentModel> Assignments { get; set; } = new System.Collections.Generic.List<LichPhanCong_AssignmentModel>();
        public System.Collections.Generic.List<LichPhanCong_PickOrderModel> PickOrders { get; set; } = new System.Collections.Generic.List<LichPhanCong_PickOrderModel>();
    }

    public class LichPhanCong_AssignmentModel
    {
        public string MaLenhSX { get; set; }
        public int TrangThai { get; set; }
        public string TenNV { get; set; }
        public string MaNV { get; set; }
        public string MaKhachHang { get; set; }
        public string TenBrand { get; set; }
        public string NgayThucHien { get; set; }
        public string GioThucHien { get; set; }
        public string MoTaCongViec { get; set; }
        public bool ThieuNPL { get; set; }
        public string GhiChu { get; set; }
    }

    public class LichPhanCong_PickOrderModel
    {
        public string MaLenhSX { get; set; }
        public int TrangThai { get; set; }
        public string TenNV { get; set; }
        public string MaNV { get; set; }
        public string MaKhachHang { get; set; }
        public string TenBrand { get; set; }
        public string NgaySoan { get; set; }
        public string GioSoan { get; set; }
        public int SoLoaiPL { get; set; }
        public double TongSLCanSoan { get; set; }
        public int SoPLThieu { get; set; }
        public string GhiChu { get; set; }
        // Chi tiet phu lieu (load rieng qua GetPickOrderDetail)
        public System.Collections.Generic.List<LichPhanCong_PickItemModel> Items { get; set; } = new System.Collections.Generic.List<LichPhanCong_PickItemModel>();
    }

    // ===================================================================
    // Model chi tiet tung mat hang phu lieu trong lenh soan
    // ===================================================================
    public class LichPhanCong_PickItemModel
    {
        public int ID { get; set; }
        public string MaLenhSX { get; set; }
        public string MaNPL { get; set; }
        public string TenNPL { get; set; }
        public double SLCanSoan { get; set; }
        public double SLTonKho { get; set; }
        public double SLDaSoan { get; set; }
        public string DonVi { get; set; }
        public string MaViTri { get; set; }
        public bool ThieuHang { get; set; }
        public string GhiChu { get; set; }
    }

    // ===================================================================
    // Model nhan vien (dropdown)
    // ===================================================================
    public class LichPhanCong_NhanVienModel
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string MaPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public bool IsActive { get; set; }
    }

    // ===================================================================
    // Request/Response models
    // ===================================================================
    public class LichPhanCong_SavePhanCongRequest
    {
        public string MaLenhSX { get; set; }
        public string MaNV { get; set; }
        public System.DateTime NgayThucHien { get; set; }
        public string GhiChu { get; set; }
    }

    public class LichPhanCong_UpdateTrangThaiRequest
    {
        public string MaLenhSX { get; set; }
        public int TrangThai { get; set; }
    }

    public class LichPhanCong_SaveResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string MaLenhSX { get; set; }
    }
}


