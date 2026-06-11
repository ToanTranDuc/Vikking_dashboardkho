using NtbSoft.ERP.Model.WipDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace NtbSoft.ERP.Web.Api.WipDonHang
{
    [RoutePrefix("api/ThuVienWip")]
    public class ThuVienWipApiController : ApiController
    {
        // ==================== GET ====================
        // GET: api/ThuVienWip/nang-luc-chuyen
        #region GET
        [HttpGet]
        [Route("nang-luc-chuyen")]
        public IHttpActionResult GetNangLucChuyen(int? MaChuyen = null, string action = "Get")
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", action),
                    new SqlParameter("@ID", DBNull.Value),
                    new SqlParameter("@MaChuyen", (object)MaChuyen ?? DBNull.Value),
                    new SqlParameter("@SLCN", DBNull.Value),
                    new SqlParameter("@NgayApDung", DBNull.Value),
                    new SqlParameter("@NguoiTao", DBNull.Value)
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_NangLucChuyen", parameters);

                var list = new List<NangLucChuyenViewModel>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapRow(row));
                    }
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // ==================== POST ====================
        // POST: api/ThuVienWip/nang-luc-chuyen
        #region POST
        [HttpPost]
        [Route("nang-luc-chuyen")]
        public IHttpActionResult PostNangLucChuyen(NangLucChuyenRequest req)
        {
            if (req == null)
                return BadRequest("Body is empty");

            try
            {
                // Validation theo Action
                if (req.Action == "Insert")
                {
                    if (req.MaChuyen <= 0)
                        return BadRequest("MaChuyen is required");
                    if (req.SLCN <= 0)
                        return BadRequest("SLCN must be > 0");
                    if (req.NgayApDung == null)
                        return BadRequest("NgayApDung is required");
                }
                else if (req.Action == "Update")
                {
                    if (req.ID <= 0)
                        return BadRequest("ID is required for Update");
                }
                else if (req.Action == "Delete")
                {
                    if (req.ID <= 0)
                        return BadRequest("ID is required for Delete");
                }
                else
                {
                    return BadRequest("Action must be: Insert, Update, or Delete");
                }

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", req.Action),
                    new SqlParameter("@ID", req.ID > 0 ? (object)req.ID : DBNull.Value),
                    new SqlParameter("@MaChuyen", req.MaChuyen > 0 ? (object)req.MaChuyen : DBNull.Value),
                    new SqlParameter("@SLCN", req.SLCN > 0 ? (object)req.SLCN : DBNull.Value),
                    new SqlParameter("@NgayApDung", req.NgayApDung != null ? (object)req.NgayApDung : DBNull.Value),
                    new SqlParameter("@NguoiTao", !string.IsNullOrEmpty(req.NguoiTao) ? (object)req.NguoiTao : DBNull.Value),
                    new SqlParameter("@ModifiedBy", !string.IsNullOrEmpty(req.ModifiedBy) ? (object)req.ModifiedBy : DBNull.Value)
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_NangLucChuyen", parameters);

                if (req.Action == "Delete")
                {
                    // Delete chỉ trả về success message
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            id = req.ID,
                            message = dt.Rows[0]["Message"].ToString()
                        });
                    }
                    return Ok(new { success = true, id = req.ID });
                }
                else
                {
                    // Insert/Update trả về record
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var result = MapRow(dt.Rows[0]);
                        return Ok(result);
                    }
                    return Ok(new { success = true });
                }
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // ==================== GET ====================
        // GET: api/ThuVienWip/thoi-gian-lam-viec
        #region GET_TGLV
        [HttpGet]
        [Route("thoi-gian-lam-viec")]
        public IHttpActionResult GetThoiGianLamViec(
            int? maChuyen = null,
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            int? skip = null,
            int? take = null,
            int? pageSize = null)
        {
            try
            {
                var hasPaging = skip.HasValue || take.HasValue || pageSize.HasValue;
                var effectiveSkip = skip.GetValueOrDefault(0);
                if (effectiveSkip < 0) effectiveSkip = 0;

                var effectiveTake = take ?? pageSize ?? 50;
                if (effectiveTake <= 0) effectiveTake = 50;

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@Action", "Get"),
            new SqlParameter("@ID", DBNull.Value),
            new SqlParameter("@SoGio", DBNull.Value),
            new SqlParameter("@GioBatDau", DBNull.Value),
            new SqlParameter("@GioKetThuc", DBNull.Value),
            new SqlParameter("@GioNghi", DBNull.Value),
            new SqlParameter("@SoGioTangCa", DBNull.Value),
            new SqlParameter("@MaChuyen", (object)maChuyen ?? DBNull.Value),
            new SqlParameter("@NgayApDung", DBNull.Value),
            new SqlParameter("@TuNgay", (object)tuNgay ?? DBNull.Value),
            new SqlParameter("@DenNgay", (object)denNgay ?? DBNull.Value),
            new SqlParameter("@Skip", hasPaging ? (object)effectiveSkip : DBNull.Value),
            new SqlParameter("@Take", hasPaging ? (object)effectiveTake : DBNull.Value),
            new SqlParameter("@PageSize", hasPaging ? (object)effectiveTake : DBNull.Value),
            new SqlParameter("@NguoiTao", DBNull.Value),
            new SqlParameter("@ModifiedBy", DBNull.Value),
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_ThoiGianLamViec", parameters);

                var list = new List<ThoiGianLamViecViewModel>();
                int totalCount = 0;
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapRow_TGLV(row));
                    }

                    if (dt.Rows.Count > 0 && dt.Columns.Contains("TotalCount") && dt.Rows[0]["TotalCount"] != DBNull.Value)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["TotalCount"]);
                    }
                }

                if (totalCount <= 0)
                {
                    totalCount = list.Count;
                }

                if (!hasPaging)
                {
                    return Ok(list);
                }

                return Ok(new
                {
                    data = list,
                    totalCount = totalCount,
                    skip = effectiveSkip,
                    take = effectiveTake,
                    pageSize = effectiveTake
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // ==================== POST ====================
        // POST: api/ThuVienWip/thoi-gian-lam-viec
        #region POST_TGLV
        [HttpPost]
        [Route("thoi-gian-lam-viec")]
        public IHttpActionResult PostTGLV(ThoiGianLamViecRequest req)
        {
            var parameters = new SqlParameter[] {
        new SqlParameter("@Action", req.Action),
        new SqlParameter("@ID", (object)req.ID ?? DBNull.Value),
        new SqlParameter("@SoGio", (object)req.SoGio ?? DBNull.Value),
        new SqlParameter("@GioBatDau", string.IsNullOrWhiteSpace(req.GioBatDau) ? (object)DBNull.Value : req.GioBatDau.Trim()),
        new SqlParameter("@GioKetThuc", string.IsNullOrWhiteSpace(req.GioKetThuc) ? (object)DBNull.Value : req.GioKetThuc.Trim()),
        new SqlParameter("@GioNghi", (object)req.GioNghi ?? DBNull.Value),
        new SqlParameter("@SoGioTangCa", (object)req.SoGioTangCa ?? DBNull.Value),
        new SqlParameter("@MaChuyen", (object)req.MaChuyen ?? DBNull.Value),
        new SqlParameter("@NgayApDung", (object)req.NgayApDung ?? DBNull.Value),
        new SqlParameter("@TuNgay", (object)req.TuNgay ?? DBNull.Value),
        new SqlParameter("@DenNgay", (object)req.DenNgay ?? DBNull.Value),
        new SqlParameter("@NguoiTao", !string.IsNullOrEmpty(req.NguoiTao) ? (object)req.NguoiTao : DBNull.Value),
        new SqlParameter("@ModifiedBy", !string.IsNullOrEmpty(req.ModifiedBy) ? (object)req.ModifiedBy : DBNull.Value)
    };

            WipDonHangModel.ExecStoredProcedure("SP_ThuVien_ThoiGianLamViec", parameters);
            return Ok(new { success = true });
        }
        #endregion
        // ====================
        // POST: api/ThuVienWip/thoi-gian-lam-viec/theo-thu
        #region POST_TGLV_THEO_THU
        [HttpPost]
        [Route("thoi-gian-lam-viec/theo-thu")]
        public IHttpActionResult PostTGLVTheoThu(ThoiGianLamViecTheoThuRequest req)
        {
            if (req == null)
                return BadRequest("Body is empty");

            if (req.Thu < 2 || req.Thu > 8)
                return BadRequest("Thu must be from 2 to 8. Use 2=Thu 2 ... 7=Thu 7, 8=Chu nhat.");

            if (!req.TuNgay.HasValue || !req.DenNgay.HasValue)
                return BadRequest("TuNgay and DenNgay are required.");

            if (req.DenNgay.Value.Date < req.TuNgay.Value.Date)
                return BadRequest("DenNgay must be greater than or equal to TuNgay.");

            if (string.IsNullOrWhiteSpace(req.GioBatDau) || string.IsNullOrWhiteSpace(req.GioKetThuc))
                return BadRequest("GioBatDau and GioKetThuc are required.");

            if (req.GioNghi.HasValue && req.GioNghi.Value < 0)
                return BadRequest("GioNghi must be >= 0.");

            if (req.SoGioTangCa.HasValue && req.SoGioTangCa.Value < 0)
                return BadRequest("SoGioTangCa must be >= 0.");

            var validLineIds = req.MaChuyenIds == null
                ? new List<int>()
                : req.MaChuyenIds.Where(x => x > 0).Distinct().ToList();

            if (req.MaChuyenIds != null && req.MaChuyenIds.Any() && !validLineIds.Any())
                return BadRequest("MaChuyenIds does not contain a valid line.");

            var lineCsv = validLineIds.Any()
                ? string.Join(",", validLineIds)
                : null;

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Thu", req.Thu),
                    new SqlParameter("@TuNgay", req.TuNgay.Value.Date),
                    new SqlParameter("@DenNgay", req.DenNgay.Value.Date),
                    new SqlParameter("@GioBatDau", req.GioBatDau.Trim()),
                    new SqlParameter("@GioKetThuc", req.GioKetThuc.Trim()),
                    new SqlParameter("@GioNghi", req.GioNghi.HasValue ? (object)req.GioNghi.Value : 0m),
                    new SqlParameter("@SoGioTangCa", req.SoGioTangCa.HasValue ? (object)req.SoGioTangCa.Value : 0m),
                    new SqlParameter("@MaChuyenList", string.IsNullOrWhiteSpace(lineCsv) ? (object)DBNull.Value : lineCsv),
                    new SqlParameter("@ModifiedBy", !string.IsNullOrWhiteSpace(req.ModifiedBy) ? (object)req.ModifiedBy.Trim() : DBNull.Value)
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_Update_ThoiGianLamViec_TheoThu", parameters);
                var row = dt != null && dt.Rows.Count > 0 ? dt.Rows[0] : null;

                return Ok(new
                {
                    success = true,
                    updatedRows = row != null && row.Table.Columns.Contains("UpdatedRows") && row["UpdatedRows"] != DBNull.Value ? Convert.ToInt32(row["UpdatedRows"]) : 0,
                    soGio = row != null && row.Table.Columns.Contains("SoGio") && row["SoGio"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["SoGio"]) : null,
                    maChuyenList = row != null && row.Table.Columns.Contains("MaChuyenList") ? row["MaChuyenList"]?.ToString() : (lineCsv ?? "ALL")
                });
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // ==================== GET ====================
        // GET: api/ThuVienWip/he-so-loi-nhuan
        #region GET_HSLN
        [HttpGet]
        [Route("he-so-loi-nhuan")]
        public IHttpActionResult GetHeSoLoiNhuan()
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@Action", "Get"),
            new SqlParameter("@ID", DBNull.Value),
            new SqlParameter("@HeSo", DBNull.Value),
            new SqlParameter("@NgayApDung", DBNull.Value),
            new SqlParameter("@NguoiTao", DBNull.Value),
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_HeSoLoiNhuan", parameters);

                var list = new List<HeSoLoiNhuanViewModel>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                        list.Add(MapRow_HSLN(row));
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion


        // ==================== POST ====================
        // POST: api/ThuVienWip/he-so-loi-nhuan
        #region POST_HSLN
        [HttpPost]
        [Route("he-so-loi-nhuan")]
        public IHttpActionResult PostHeSoLoiNhuan(HeSoLoiNhuanRequest req)
        {
            if (req == null) return BadRequest("Body is empty");

            try
            {
                // Validation theo Action
                if (req.Action == "Insert")
                {
                    if (req.HeSo <= 0) return BadRequest("HeSo must be > 0");
                    if (req.NgayApDung == null) return BadRequest("NgayApDung is required");
                }
                else if (req.Action == "Update")
                {
                    if (req.ID <= 0) return BadRequest("ID is required for Update");
                }
                else if (req.Action == "Delete")
                {
                    if (req.ID <= 0) return BadRequest("ID is required for Delete");
                }
                else
                {
                    return BadRequest("Action must be: Insert, Update, or Delete");
                }

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@Action", req.Action),
            new SqlParameter("@ID", req.ID > 0 ? (object)req.ID : DBNull.Value),
            new SqlParameter("@HeSo", req.HeSo > 0 ? (object)req.HeSo : DBNull.Value),
            new SqlParameter("@NgayApDung", req.NgayApDung != null ? (object)req.NgayApDung : DBNull.Value),
            new SqlParameter("@NguoiTao", !string.IsNullOrEmpty(req.NguoiTao) ? (object)req.NguoiTao : DBNull.Value),
            new SqlParameter("@ModifiedBy", !string.IsNullOrEmpty(req.ModifiedBy) ? (object)req.ModifiedBy : DBNull.Value),
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_HeSoLoiNhuan", parameters);

                if (req.Action == "Delete")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            id = req.ID,
                            message = dt.Rows[0]["Message"]?.ToString()
                        });
                    }
                    return Ok(new { success = true, id = req.ID });
                }

                // Insert/Update trả về record
                if (dt != null && dt.Rows.Count > 0)
                {
                    var result = MapRow_HSLN(dt.Rows[0]);
                    return Ok(result);
                }

                return Ok(new { success = true });
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // GET: api/ThuVienWip/smv
        #region GET_SMV
        [HttpGet]
        [Route("smv")]
        public IHttpActionResult GetSmv(string maHang = null, string user = null,
                                 DateTime? fromDate = null, DateTime? toDate = null,
                                 bool doSync = true, DateTime? ngayApDungSync = null, string action = "Get")
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Action", action),
            new SqlParameter("@ID", DBNull.Value),

            new SqlParameter("@MaHang", string.IsNullOrWhiteSpace(maHang) ? (object)DBNull.Value : maHang.Trim()),

            new SqlParameter("@NgayApDung", DBNull.Value),
            new SqlParameter("@SMV", DBNull.Value),
            new SqlParameter("@SMV_May", DBNull.Value),
            new SqlParameter("@SMV_Cat", DBNull.Value),
            new SqlParameter("@SMV_Laptrinh", DBNull.Value),
            new SqlParameter("@SMV_HoanThanh", DBNull.Value),
            new SqlParameter("@In_ep", DBNull.Value),
            new SqlParameter("@CM", DBNull.Value),
            new SqlParameter("@InTheuCT", DBNull.Value),
            new SqlParameter("@HutAm", DBNull.Value),
            new SqlParameter("@DoKim", DBNull.Value),

            new SqlParameter("@NguoiTao", DBNull.Value),
            new SqlParameter("@UpdatedBy", string.IsNullOrWhiteSpace(user) ? (object)DBNull.Value : user.Trim()),

            new SqlParameter("@FromDate", fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value),
            new SqlParameter("@ToDate", toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value),

            new SqlParameter("@DoSync", doSync),
            new SqlParameter("@NgayApDungSync", ngayApDungSync.HasValue ? (object)ngayApDungSync.Value.Date : DBNull.Value),
        };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_SMV", parameters);

                var list = new List<ThuVienSmvViewModel>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                        list.Add(MapRow_SMV(row));
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // POST: api/ThuVienWip/smv
        #region POST_SMV
        [HttpPost]
        [Route("smv")]
        public IHttpActionResult PostSmv(ThuVienSmvRequest req)
        {
            if (req == null) return BadRequest("Body is empty");

            try
            {
                var action = (req.Action ?? "").Trim();
                if (string.IsNullOrEmpty(action)) return BadRequest("Action is required");

                // normalize
                action = action.ToUpperInvariant();
                if (action != "INSERT" && action != "UPDATE" && action != "DELETE")
                    return BadRequest("Action must be: Insert | Update | Delete");

                var updatedBy = !string.IsNullOrWhiteSpace(req.UpdatedBy)
                    ? req.UpdatedBy.Trim()
                    : (User?.Identity != null && User.Identity.IsAuthenticated ? User.Identity.Name : null);

                // validate by action
                if (action == "DELETE")
                {
                    if (req.ID == null || req.ID < 0) return BadRequest("ID is required for Delete");
                }
                else if (action == "INSERT")
                {
                    if (string.IsNullOrWhiteSpace(req.MaHang)) return BadRequest("MaHang is required");
                    if (req.NgayApDung == null) return BadRequest("NgayApDung is required");
                }
                else if (action == "UPDATE")
                {
                    if (req.ID == null || req.ID < 0) return BadRequest("ID is required for Update");
                    // NgayApDung có thể null (giữ nguyên), SMV có thể null (clear)
                }
                var parameters = new[]
                {
            new SqlParameter("@Action", action),
            new SqlParameter("@ID", (object)(req.ID ?? (int?)null) ?? DBNull.Value),
            new SqlParameter("@MaHang", string.IsNullOrWhiteSpace(req.MaHang) ? (object)DBNull.Value : req.MaHang.Trim()),
            new SqlParameter("@NgayApDung", req.NgayApDung.HasValue ? (object)req.NgayApDung.Value.Date : DBNull.Value),
            new SqlParameter("@SMV", req.SMV.HasValue ? (object)req.SMV.Value : DBNull.Value),
            new SqlParameter("@SMV_May", req.SMV_May.HasValue ? (object)req.SMV_May.Value : DBNull.Value),
            new SqlParameter("@SMV_Cat", req.SMV_Cat.HasValue ? (object)req.SMV_Cat.Value : DBNull.Value),
            new SqlParameter("@SMV_Laptrinh", req.SMV_Laptrinh.HasValue ? (object)req.SMV_Laptrinh.Value : DBNull.Value),
            new SqlParameter("@SMV_HoanThanh", req.SMV_HoanThanh.HasValue ? (object)req.SMV_HoanThanh.Value : DBNull.Value),
            new SqlParameter("@In_ep", req.In_ep.HasValue ? (object)req.In_ep.Value : DBNull.Value),
            new SqlParameter("@CM", req.CM.HasValue ? (object)req.CM.Value : DBNull.Value),
            new SqlParameter("@InTheuCT", req.InTheuCT.HasValue ? (object)req.InTheuCT.Value : DBNull.Value),
            new SqlParameter("@HutAm", req.HutAm.HasValue ? (object)req.HutAm.Value : DBNull.Value),
            new SqlParameter("@DoKim", req.DoKim.HasValue ? (object)req.DoKim.Value : DBNull.Value),

            new SqlParameter("@NguoiTao", string.IsNullOrWhiteSpace(req.NguoiTao) ? (object)DBNull.Value : req.NguoiTao.Trim()),
            new SqlParameter("@UpdatedBy", string.IsNullOrWhiteSpace(updatedBy) ? (object)DBNull.Value : updatedBy),

            // GET-only params
            new SqlParameter("@FromDate", DBNull.Value),
            new SqlParameter("@ToDate", DBNull.Value),
            new SqlParameter("@DoSync", 0),
            new SqlParameter("@NgayApDungSync", DBNull.Value),
        };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_SMV", parameters);

                // DELETE trả message
                if (action == "DELETE")
                {
                    return Ok(new { success = true, id = req.ID });
                }

                // INSERT/UPDATE: SP trả record
                if (dt != null && dt.Rows.Count > 0)
                {
                    var result = MapRow_SMV(dt.Rows[0]);
                    return Ok(result);
                }

                return Ok(new { success = true });
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
        // GET: api/ThuVienWip/smv-cm
        #region GET_SMV_CM
        [HttpGet]
        [Route("get-cm")]
        public IHttpActionResult GetCm(string maHang = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Action", "GETCM"),
            new SqlParameter("@MaHang", string.IsNullOrWhiteSpace(maHang) ? (object)DBNull.Value : maHang.Trim()),
            new SqlParameter("@FromDate", fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value),
            new SqlParameter("@ToDate", toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value),
            new SqlParameter("@DoSync", 0),
        };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_SMV", parameters);

                var list = new List<ThuVienSmvCmViewModel>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapRow_CM(row));
                    }
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // GET: api/ThuVienWip/smv-hanghoa?keyword=abc
        #region GET_SMV_HANGHOA
        [HttpGet]
        [Route("smv-hanghoa")]
        public IHttpActionResult GetSmvHangHoa(string keyword = null)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Action", "GetMaHang"),
            new SqlParameter("@DoSync", 0),
            // ✅ NEW param
            new SqlParameter("@Keyword", string.IsNullOrWhiteSpace(keyword)
                ? (object)DBNull.Value
                : keyword.Trim())
        };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_ThuVien_SMV", parameters);

                var list = new List<ThuVienSmvHangHoaVM>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new ThuVienSmvHangHoaVM
                        {
                            MaHang = row["MaHang"]?.ToString(),
                            TenHang = row["TenHang"]?.ToString(),
                            InTheu = row.Table.Columns.Contains("InTheu") && row["InTheu"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["InTheu"]) : null,
                            InTheuCT = row.Table.Columns.Contains("InTheuCT") && row["InTheuCT"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["InTheuCT"]) : null,
                            HutAm = row.Table.Columns.Contains("HutAm") && row["HutAm"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["HutAm"]) : null,
                            DoKim = row.Table.Columns.Contains("DoKim") && row["DoKim"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["DoKim"]) : null
                        });
                    }
                }
                return Ok(list);
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // GET: api/ThuVienWip/get-history
        #region Get-History
        [HttpGet]
        [Route("get-history")]
        public IHttpActionResult GetHistoryLib(string libCode = null, int? libId = null)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Action", "GET"),
                    new SqlParameter("@LibCode", (object)libCode ?? DBNull.Value),
                    new SqlParameter("@LibId", (object)libId ?? DBNull.Value)
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_Lib_Wip_Audit", parameters);

                var list = new List<HistoryLibViewModel>();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapRow_HistoryLib(row));
                    }
                }

                var factory = new HistoryLibResolverFactory();
                List<HistoryLibDisplayViewModel> displayList;

                if (!string.IsNullOrWhiteSpace(libCode))
                {
                    var resolver = factory.Get(libCode);
                    displayList = resolver.Resolve(list);
                }
                else
                {
                    var indexed = list.Select((item, index) => new { item, index }).ToList();
                    var arranged = new HistoryLibDisplayViewModel[list.Count];

                    foreach (var group in indexed.GroupBy(x =>
                        string.IsNullOrWhiteSpace(x.item.LibCode)
                            ? string.Empty
                            : x.item.LibCode.Trim().ToUpperInvariant()))
                    {
                        var audits = group.Select(x => x.item).ToList();
                        var resolver = factory.Get(group.Key);
                        var resolved = resolver.Resolve(audits);

                        var i = 0;
                        foreach (var row in group)
                        {
                            arranged[row.index] = i < resolved.Count ? resolved[i] : null;
                            i++;
                        }
                    }

                    displayList = arranged.Where(x => x != null).ToList();
                }

                return Ok(displayList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return InternalServerError(ex);
            }
        }
        #endregion
        // GET: api/ThuVienWip/ngay-nghi?year=2026&month=3
        #region GET_NGAY_NGHI
        [HttpGet]
        [Route("ngay-nghi")]
        public IHttpActionResult GetNgayNghi(int year, int? month = null)
        {
            if (year <= 0) return BadRequest("year is required");
            if (month.HasValue && (month.Value < 1 || month.Value > 12))
                return BadRequest("month must be between 1 and 12");

            try
            {
                var newIdOutput = new SqlParameter("@NewID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "LIST"),
                    new SqlParameter("@Year", year),
                    new SqlParameter("@Month", month.HasValue ? (object)month.Value : DBNull.Value),
                    new SqlParameter("@ID", DBNull.Value),
                    new SqlParameter("@NgayNghi", DBNull.Value),
                    new SqlParameter("@GhiChu", DBNull.Value),
                    newIdOutput
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_DIC_NGAYNGHI_Action", parameters);

                var list = new List<NgayNghiViewModel>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(MapRow_NgayNghi(row));
                    }
                }

                return Ok(list);
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        // POST: api/ThuVienWip/ngay-nghi
        #region POST_NGAY_NGHI
        [HttpPost]
        [Route("ngay-nghi")]
        public IHttpActionResult PostNgayNghiAction(NgayNghiActionRequest req)
        {
            if (req == null) return BadRequest("Body is empty");

            var action = (req.Action ?? string.Empty).Trim().ToUpperInvariant();
            if (action != "CREATE" && action != "UPDATE" && action != "DELETE")
                return BadRequest("Action must be: CREATE, UPDATE, DELETE");

            if (action == "CREATE")
            {
                if (!req.NgayNghi.HasValue)
                    return BadRequest("NgayNghi is required for CREATE");
            }
            else
            {
                if (!req.ID.HasValue || req.ID.Value <= 0)
                    return BadRequest("ID is required for UPDATE/DELETE");
            }

            if (req.Month.HasValue && (req.Month.Value < 1 || req.Month.Value > 12))
                return BadRequest("month must be between 1 and 12");

            try
            {
                var newIdOutput = new SqlParameter("@NewID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", action),
                    new SqlParameter("@Year", req.Year.HasValue ? (object)req.Year.Value : DBNull.Value),
                    new SqlParameter("@Month", req.Month.HasValue ? (object)req.Month.Value : DBNull.Value),
                    new SqlParameter("@ID", req.ID.HasValue ? (object)req.ID.Value : DBNull.Value),
                    new SqlParameter("@ID_NgayNghi", req.ID_NgayNghi.HasValue ? (object)req.ID_NgayNghi.Value : DBNull.Value),
                    new SqlParameter("@NgayNghi", req.NgayNghi.HasValue ? (object)req.NgayNghi.Value.Date : DBNull.Value),
                    new SqlParameter("@GhiChu", string.IsNullOrWhiteSpace(req.GhiChu) ? (object)DBNull.Value : req.GhiChu.Trim()),
                    newIdOutput
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_DIC_NGAYNGHI_Action", parameters);

                int? newId = newIdOutput.Value != null && newIdOutput.Value != DBNull.Value
                    ? (int?)Convert.ToInt32(newIdOutput.Value)
                    : null;

                if (action == "CREATE")
                {
                    return Ok(new
                    {
                        success = true,
                        newId = newId,
                        data = (dt != null && dt.Rows.Count > 0) ? MapRow_NgayNghi(dt.Rows[0]) : null
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = (dt != null && dt.Rows.Count > 0) ? MapRow_NgayNghi(dt.Rows[0]) : null
                });
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(sqlEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
        // ==================== PRIVATE METHODS ====================
        private NangLucChuyenViewModel MapRow(DataRow row)
        {
            return new NangLucChuyenViewModel
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                MaChuyen = row["MaChuyen"] != DBNull.Value ? Convert.ToInt32(row["MaChuyen"]) : 0,
                TenChuyen = row["TenChuyen"]?.ToString(),
                SLCN = row["SLCN"] != DBNull.Value ? Convert.ToInt32(row["SLCN"]) : 0,
                NgayApDung = row["NgayApDung"] != DBNull.Value ? Convert.ToDateTime(row["NgayApDung"]).Date : (DateTime?)null,
                NguoiTao = row["NguoiTao"]?.ToString(),
                TenNguoiTao = row["TenNguoiTao"]?.ToString(),
                NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : (DateTime?)null
            };
        }
        private ThoiGianLamViecViewModel MapRow_TGLV(DataRow row)
        {
            return new ThoiGianLamViecViewModel
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                SoGio = row["SoGio"] != DBNull.Value ? Convert.ToDecimal(row["SoGio"]) : 0,
                MaChuyen = row.Table.Columns.Contains("MaChuyen") && row["MaChuyen"] != DBNull.Value ? Convert.ToInt32(row["MaChuyen"]) : 0,
                GioBatDau = row.Table.Columns.Contains("GioBatDau") ? row["GioBatDau"]?.ToString() : null,
                GioKetThuc = row.Table.Columns.Contains("GioKetThuc") ? row["GioKetThuc"]?.ToString() : null,
                GioNghi = row.Table.Columns.Contains("GioNghi") && row["GioNghi"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["GioNghi"]) : null,
                SoGioTangCa = row.Table.Columns.Contains("SoGioTangCa") && row["SoGioTangCa"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["SoGioTangCa"]) : null,
                NgayApDung = row["NgayApDung"] != DBNull.Value ? Convert.ToDateTime(row["NgayApDung"]).Date : (DateTime?)null,
                NguoiTao = row["NguoiTao"]?.ToString(),
                TenNguoiTao = row.Table.Columns.Contains("TenNguoiTao") ? row["TenNguoiTao"]?.ToString() : null,
                NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : (DateTime?)null
            };
        }
        private HeSoLoiNhuanViewModel MapRow_HSLN(DataRow row)
        {
            return new HeSoLoiNhuanViewModel
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                HeSo = row["HeSo"] != DBNull.Value ? Convert.ToDecimal(row["HeSo"]) : 0,
                NgayApDung = row["NgayApDung"] != DBNull.Value ? Convert.ToDateTime(row["NgayApDung"]).Date : (DateTime?)null,
                NguoiTao = row["NguoiTao"]?.ToString(),
                TenNguoiTao = row.Table.Columns.Contains("TenNguoiTao") ? row["TenNguoiTao"]?.ToString() : null,
                NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : (DateTime?)null
            };
        }
        private ThuVienSmvViewModel MapRow_SMV(DataRow row)
        {
            return new ThuVienSmvViewModel
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                MaHang = row["MaHang"]?.ToString(),
                TenHang = row["TenHang"]?.ToString(),
                TenCL = row["TenCL"]?.ToString(),
                SMV = row["SMV"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["SMV"]) : null,
                SMV_May = row.Table.Columns.Contains("SMV_May") && row["SMV_May"] != DBNull.Value ? (double?)Convert.ToDouble(row["SMV_May"]) : null,
                SMV_Cat = row.Table.Columns.Contains("SMV_Cat") && row["SMV_Cat"] != DBNull.Value ? (double?)Convert.ToDouble(row["SMV_Cat"]) : null,
                SMV_Laptrinh = row.Table.Columns.Contains("SMV_Laptrinh") && row["SMV_Laptrinh"] != DBNull.Value ? (double?)Convert.ToDouble(row["SMV_Laptrinh"]) : null,
                SMV_HoanThanh = row.Table.Columns.Contains("SMV_HoanThanh") && row["SMV_HoanThanh"] != DBNull.Value ? (double?)Convert.ToDouble(row["SMV_HoanThanh"]) : null,
                In_ep = row.Table.Columns.Contains("In_ep") && row["In_ep"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["In_ep"]) : null,
                CM = row.Table.Columns.Contains("CM") && row["CM"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["CM"]) : null,
                InTheuCT = row.Table.Columns.Contains("InTheuCT") && row["InTheuCT"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["InTheuCT"]) : null,
                HutAm = row.Table.Columns.Contains("HutAm") && row["HutAm"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["HutAm"]) : null,
                DoKim = row.Table.Columns.Contains("DoKim") && row["DoKim"] != DBNull.Value ? (bool?)Convert.ToBoolean(row["DoKim"]) : null,
                NgayApDung = row["NgayApDung"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["NgayApDung"]).Date : null,
                UpdatedAt = row["UpdatedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["UpdatedAt"]) : null,
                UpdatedBy = row["UpdatedBy"]?.ToString(),
                HasSMV = row["HasSMV"] != DBNull.Value ? Convert.ToInt32(row["HasSMV"]) : 0
            };
        }
        private ThuVienSmvCmViewModel MapRow_CM(DataRow row)
        {
            return new ThuVienSmvCmViewModel
            {
                ID = row.Table.Columns.Contains("ID") && row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                MaHang = row.Table.Columns.Contains("MaHang") ? row["MaHang"]?.ToString() : null,
                NgayApDung = row.Table.Columns.Contains("NgayApDung") && row["NgayApDung"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["NgayApDung"]).Date
                    : null,
                CM = row.Table.Columns.Contains("CM") && row["CM"] != DBNull.Value
                    ? (decimal?)Convert.ToDecimal(row["CM"])
                    : null
            };
        }
        private HistoryLibViewModel MapRow_HistoryLib(DataRow row)
        {
            return new HistoryLibViewModel
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                LibCode = row["LibCode"]?.ToString(),
                LibId = row["LibId"] != DBNull.Value ? (int?)Convert.ToInt32(row["LibId"]) : null,
                FieldName = row["FieldName"]?.ToString(),
                OldValue = row["OldValue"]?.ToString(),
                NewValue = row["NewValue"]?.ToString(),
                ModifiedBy = row["ModifiedBy"]?.ToString(),
                ModifiedByName = !string.IsNullOrWhiteSpace(row["ModifiedByName"]?.ToString())
                    ? row["ModifiedByName"]?.ToString()
                    : row["ModifiedBy"]?.ToString(),
                ModifiedAt = row["ModifiedAt"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ModifiedAt"]) : null
            };
        }

        private NgayNghiViewModel MapRow_NgayNghi(DataRow row)
        {
            return new NgayNghiViewModel
            {
                ID = row.Table.Columns.Contains("ID") && row["ID"] != DBNull.Value ? (int?)Convert.ToInt32(row["ID"]) : null,
                STT = row.Table.Columns.Contains("STT") && row["STT"] != DBNull.Value ? (int?)Convert.ToInt32(row["STT"]) : null,
                NgayNghi = row.Table.Columns.Contains("NgayNghi") && row["NgayNghi"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["NgayNghi"]) : null,
                GhiChu = row.Table.Columns.Contains("GhiChu") && row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : null,
                StrDayOfWeek = row.Table.Columns.Contains("StrDayOfWeek") && row["StrDayOfWeek"] != DBNull.Value ? row["StrDayOfWeek"].ToString() : null,
                Year = row.Table.Columns.Contains("Year") && row["Year"] != DBNull.Value ? (int?)Convert.ToInt32(row["Year"]) : null,
                Month = row.Table.Columns.Contains("Month") && row["Month"] != DBNull.Value ? (int?)Convert.ToInt32(row["Month"]) : null
            };
        }
    }
    // ==================== MODELS ====================
    public class HistoryLibViewModel
    {
        public int ID { get; set; }
        public string LibCode { get; set; }
        public int? LibId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
    public class HistoryLibDisplayViewModel
    {
        public int ID { get; set; }
        public string LibCode { get; set; }
        public int? LibId { get; set; }

        public string TenChuyen { get; set; }
        public string MaHang { get; set; }

        public DateTime? NgayApDung { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public string ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
    public interface IHistoryLibResolver
    {
        List<HistoryLibDisplayViewModel> Resolve(List<HistoryLibViewModel> audits);
    }

    public class HistoryLibContextModel
    {
        public int LibId { get; set; }
        public string TenChuyen { get; set; }
        public string MaHang { get; set; }
        public DateTime? NgayApDung { get; set; }
    }

    public class DefaultHistoryResolver : IHistoryLibResolver
    {
        public virtual List<HistoryLibDisplayViewModel> Resolve(List<HistoryLibViewModel> audits)
        {
            var source = audits ?? new List<HistoryLibViewModel>();
            return source.Select(MapDefault).ToList();
        }

        protected static HistoryLibDisplayViewModel MapDefault(HistoryLibViewModel x)
        {
            if (x == null) return new HistoryLibDisplayViewModel();

            return new HistoryLibDisplayViewModel
            {
                ID = x.ID,
                LibCode = x.LibCode,
                LibId = x.LibId,
                TenChuyen = null,
                MaHang = null,
                NgayApDung = null,
                FieldName = x.FieldName,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                ModifiedBy = x.ModifiedBy,
                ModifiedByName = x.ModifiedByName,
                ModifiedAt = x.ModifiedAt
            };
        }
    }

    public abstract class HistoryLibResolverBase : DefaultHistoryResolver
    {
        protected abstract string TargetLibCode { get; }

        protected abstract HistoryLibDisplayViewModel MergeDisplay(
            HistoryLibViewModel audit,
            Dictionary<int, HistoryLibContextModel> contextByLibId);

        public override List<HistoryLibDisplayViewModel> Resolve(List<HistoryLibViewModel> audits)
        {
            var source = audits ?? new List<HistoryLibViewModel>();
            var contextByLibId = LoadContextMap(source, TargetLibCode);

            return source.Select(audit => MergeDisplay(audit, contextByLibId)).ToList();
        }

        private Dictionary<int, HistoryLibContextModel> LoadContextMap(List<HistoryLibViewModel> audits, string libCode)
        {
            var libIds = (audits ?? new List<HistoryLibViewModel>())
                .Where(x => x != null && x.LibId.HasValue && x.LibId.Value > 0)
                .Select(x => x.LibId.Value)
                .Distinct()
                .ToList();

            var map = new Dictionary<int, HistoryLibContextModel>();
            if (!libIds.Any()) return map;

            var parameters = new[]
            {
                new SqlParameter("@LibCode", (object)libCode ?? DBNull.Value),
                new SqlParameter("@LibIds", string.Join(",", libIds))
            };

            var dt = WipDonHangModel.ExecStoredProcedure("Lib_Wip_Factory", parameters);
            if (dt == null || dt.Rows.Count == 0) return map;
            
            foreach (DataRow row in dt.Rows)
            {
                var id = ReadLibId(row);
                if (!id.HasValue || id.Value <= 0) continue;

                map[id.Value] = new HistoryLibContextModel
                {
                    LibId = id.Value,
                    TenChuyen = ReadString(row, "TenChuyen"),
                    MaHang = ReadString(row, "MaHang"),
                    NgayApDung = ReadDate(row, "NgayApDung")
                };
            }

            return map;
        }

        protected static int? ReadLibId(DataRow row)
        {
            if (row == null || row.Table == null) return null;

            var keys = new[] { "LibId", "ID", "Id", "RefId" };
            foreach (var key in keys)
            {
                if (!row.Table.Columns.Contains(key)) continue;
                if (row[key] == DBNull.Value) continue;

                int value;
                if (int.TryParse(Convert.ToString(row[key]), out value))
                {
                    return value;
                }
            }

            return null;
        }

        protected static string ReadString(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col)) return null;
            if (row[col] == DBNull.Value) return null;
            return Convert.ToString(row[col]);
        }
        protected static DateTime? ReadDate(DataRow row, string col)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(col)) return null;
            if (row[col] == DBNull.Value) return null;
            return Convert.ToDateTime(row[col]).Date;
        }
    }

    public class NangLucChuyenHistoryResolver : HistoryLibResolverBase
    {
        protected override string TargetLibCode { get { return "NANG_LUC_CHUYEN"; } }

        protected override HistoryLibDisplayViewModel MergeDisplay(
            HistoryLibViewModel audit,
            Dictionary<int, HistoryLibContextModel> contextByLibId)
        {
            var item = MapDefault(audit);
            if (audit != null && audit.LibId.HasValue && contextByLibId.ContainsKey(audit.LibId.Value))
            {
                item.TenChuyen = contextByLibId[audit.LibId.Value].TenChuyen;
                item.NgayApDung = contextByLibId[audit.LibId.Value].NgayApDung;
            }
            return item;
        }
    }

    public class LichLamViecHistoryResolver : HistoryLibResolverBase
    {
        protected override string TargetLibCode { get { return "LICH_LAM_VIEC"; } }

        protected override HistoryLibDisplayViewModel MergeDisplay(
            HistoryLibViewModel audit,
            Dictionary<int, HistoryLibContextModel> contextByLibId)
        {
            var item = MapDefault(audit);
            if (audit != null && audit.LibId.HasValue && contextByLibId.ContainsKey(audit.LibId.Value))
            {
                item.TenChuyen = contextByLibId[audit.LibId.Value].TenChuyen;
                item.NgayApDung = contextByLibId[audit.LibId.Value].NgayApDung;
            }
            return item;
        }
    }

    public class SmvHistoryResolver : HistoryLibResolverBase
    {
        protected override string TargetLibCode { get { return "SMV"; } }

        protected override HistoryLibDisplayViewModel MergeDisplay(
            HistoryLibViewModel audit,
            Dictionary<int, HistoryLibContextModel> contextByLibId)
        {
            var item = MapDefault(audit);
            if (audit != null && audit.LibId.HasValue && contextByLibId.ContainsKey(audit.LibId.Value))
            {
                item.MaHang = contextByLibId[audit.LibId.Value].MaHang;
                item.NgayApDung = contextByLibId[audit.LibId.Value].NgayApDung;
            }
            return item;
        }
    }
    public class HeSoLoiNhuanHistoryResolver : HistoryLibResolverBase
    {
        protected override string TargetLibCode { get { return "HE_SO_LOI_NHUAN"; } }

        protected override HistoryLibDisplayViewModel MergeDisplay(
            HistoryLibViewModel audit,
            Dictionary<int, HistoryLibContextModel> contextByLibId)
        {
            var item = MapDefault(audit);
            if (audit != null && audit.LibId.HasValue && contextByLibId.ContainsKey(audit.LibId.Value))
            {
                item.NgayApDung = contextByLibId[audit.LibId.Value].NgayApDung;
            }
            return item;
        }
    }

    public class HistoryLibResolverFactory
    {
        public IHistoryLibResolver Get(string libCode)
        {
            var code = string.IsNullOrWhiteSpace(libCode)
                ? string.Empty
                : libCode.Trim().ToUpperInvariant();

            switch (code)
            {
                case "NANG_LUC_CHUYEN":
                    return new NangLucChuyenHistoryResolver();
                case "LICH_LAM_VIEC":
                    return new LichLamViecHistoryResolver();
                case "SMV":
                    return new SmvHistoryResolver();
                case "HE_SO_LOI_NHUAN":
                    return new HeSoLoiNhuanHistoryResolver();
                default:
                    return new DefaultHistoryResolver();
            }
        }
    }
    public class ThuVienSmvHangHoaVM
    {
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public bool? InTheu { get; set; }
        public bool? InTheuCT { get; set; }
        public bool? HutAm { get; set; }
        public bool? DoKim { get; set; }
    }
    public class NangLucChuyenRequest
    {
        public string Action { get; set; } // "Insert", "Update", "Delete"
        public int ID { get; set; }
        public int MaChuyen { get; set; }
        public int SLCN { get; set; }
        public DateTime? NgayApDung { get; set; }
        public string NguoiTao { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class NangLucChuyenViewModel
    {
        public int ID { get; set; }
        public int MaChuyen { get; set; }
        public string TenChuyen { get; set; }
        public int SLCN { get; set; }
        public DateTime? NgayApDung { get; set; }
        public string NguoiTao { get; set; }
        public string TenNguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
    }
    public class ThoiGianLamViecRequest
    {
        public string Action { get; set; }
        public int? ID { get; set; }
        public decimal? SoGio { get; set; }
        public string GioBatDau { get; set; }
        public string GioKetThuc { get; set; }
        public decimal? GioNghi { get; set; }
        public decimal? SoGioTangCa { get; set; }
        public int MaChuyen { get; set; }
        public DateTime? NgayApDung { get; set; } // Dùng cho sửa lẻ
        public DateTime? TuNgay { get; set; }     // Khai báo nhanh
        public DateTime? DenNgay { get; set; }    // Khai báo nhanh
        public string NguoiTao { get; set; }
        public string ModifiedBy { get; set; }
    }
    public class ThoiGianLamViecTheoThuRequest
    {
        public int Thu { get; set; } // 2=Thu 2 ... 7=Thu 7, 8=Chu nhat
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string GioBatDau { get; set; }
        public string GioKetThuc { get; set; }
        public decimal? GioNghi { get; set; }
        public decimal? SoGioTangCa { get; set; }
        public List<int> MaChuyenIds { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class ThoiGianLamViecViewModel
    {
        public int ID { get; set; }
        public decimal SoGio { get; set; }
        public string GioBatDau { get; set; }
        public string GioKetThuc { get; set; }
        public int MaChuyen { get; set; }
        public decimal? GioNghi { get; set; }
        public decimal? SoGioTangCa { get; set; }
        public DateTime? NgayApDung { get; set; }
        public string NguoiTao { get; set; }
        public string TenNguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
    }
    public class HeSoLoiNhuanRequest
    {
        public string Action { get; set; } // Insert, Update, Delete
        public int ID { get; set; }
        public decimal HeSo { get; set; }
        public DateTime? NgayApDung { get; set; }
        public string NguoiTao { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class HeSoLoiNhuanViewModel
    {
        public int ID { get; set; }
        public decimal HeSo { get; set; }
        public DateTime? NgayApDung { get; set; }
        public string NguoiTao { get; set; }
        public string TenNguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
    }
    public class ThuVienSmvViewModel
    {
        public int ID { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string TenCL { get; set; }
        public decimal? SMV { get; set; }
        public double? SMV_May { get; set; }
        public double? SMV_Cat { get; set; }
        public double? SMV_Laptrinh { get; set; }
        public double? SMV_HoanThanh { get; set; }
        public bool? In_ep { get; set; }
        public decimal? CM { get; set; }
        public bool? InTheuCT { get; set; }
        public bool? HutAm { get; set; }
        public bool? DoKim { get; set; }
        public DateTime? NgayApDung { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public int HasSMV { get; set; }
    }

    public class ThuVienSmvRequest
    {
        public string Action { get; set; }
        public int? ID { get; set; }

        public string MaHang { get; set; }
        public DateTime? NgayApDung { get; set; }

        public decimal? SMV { get; set; }
        public double? SMV_May { get; set; }
        public double? SMV_Cat { get; set; }
        public double? SMV_Laptrinh { get; set; }
        public double? SMV_HoanThanh { get; set; }
        public bool? In_ep { get; set; }
        public decimal? CM { get; set; }
        public bool? InTheuCT { get; set; }
        public bool? HutAm { get; set; }
        public bool? DoKim { get; set; }
        public string UpdatedBy { get; set; }
        public string NguoiTao { get; set; }
    }
    public class ThuVienSmvCmViewModel
    {
        public int ID { get; set; }
        public string MaHang { get; set; }
        public DateTime? NgayApDung { get; set; }
        public decimal? CM { get; set; }
    }
    public class NgayNghiActionRequest
    {
        public string Action { get; set; } // CREATE, UPDATE, DELETE
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? ID { get; set; }
        public int? ID_NgayNghi { get; set; }
        public DateTime? NgayNghi { get; set; }
        public string GhiChu { get; set; }
    }
    public class NgayNghiViewModel
    {
        public int? ID { get; set; }
        public int? STT { get; set; }
        public DateTime? NgayNghi { get; set; }
        public string StrDayOfWeek { get; set; }
        public string GhiChu { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
    }

}
