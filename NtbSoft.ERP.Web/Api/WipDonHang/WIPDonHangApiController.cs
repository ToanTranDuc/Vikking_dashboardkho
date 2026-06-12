using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Model.WipDonHang;
using NtbSoft.ERP.Web.Models.WipDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.WipDonHang
{
    [RoutePrefix("api/wip-donhang")]
    public class WIPDonHangApiController : ApiController
    {
        private const string HeaderColorConfigFeatureKey = "HEADER_COLOR_CONFIG";
        private static readonly object _techStepCacheLock = new object();
        private static DateTime _techStepCacheAtUtc = DateTime.MinValue;
        private static List<TechStepMeta> _techStepCache = new List<TechStepMeta>();
        private static readonly object _saveDeltaFieldCacheLock = new object();
        private static DateTime _saveDeltaFieldCacheAtUtc = DateTime.MinValue;
        private static List<SaveDeltaFieldMeta> _saveDeltaFieldCache = new List<SaveDeltaFieldMeta>();

        // =========================
        // GET: api/wip-donhang/checkLineExists
        // =========================
        #region CheckLineExists
        [HttpGet]
        [Route("checkLineExists")]
        public IHttpActionResult CheckLineExists(
            string maDH,
            string maLenhSanXuat,
            int lineX
        )
        {
            if (string.IsNullOrWhiteSpace(maDH))
                return BadRequest("maDH is required");
            var maDHSplit = maDH.Split(';');
            if (string.IsNullOrWhiteSpace(maLenhSanXuat))
                return BadRequest("maLenhSanXuat is required");

            if (lineX <= 0)
                return BadRequest("lineX must be > 0");

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Action", "CheckLineExists"),
        new SqlParameter("@MaDH", (object)maDHSplit[0] ?? DBNull.Value),
        new SqlParameter("@MaLenhSanXuat", (object)maLenhSanXuat ?? DBNull.Value),
        new SqlParameter("@LineMoi", (object)lineX ?? DBNull.Value),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure(
                "SP_WIP_DonHang_ChiaChuyen",
                parameters
            );

            // SP trả về 1 dòng/1 cột Result = TRUE/FALSE
            var result = "FALSE";
            var maGopOut = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                var obj = dt.Rows[0]["Result"];
                result = Convert.ToString(obj ?? "FALSE");
                if (dt.Columns.Contains("MaGop"))
                    maGopOut = Convert.ToString(dt.Rows[0]["MaGop"] ?? "");
            }

            // trả JSON để phía web dùng dễ
            return Ok(new
            {
                ok = true,
                exists = string.Equals(result?.Trim(), "TRUE", StringComparison.OrdinalIgnoreCase),
                raw = result,
                maGop = maGopOut
            });
        }
        #endregion

        // =========================
        // GET: api/wip-donhang/audit
        // =========================    
        #region Audit
        [HttpGet]
        [Route("audit")]
        public IHttpActionResult GetAudit(int wipId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var list = new List<WIPAuditViewModel>();

            if (wipId <= 0)
                return Ok(list);
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
                return BadRequest("'fromDate' không được lớn hơn 'toDate'.");
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@WipId", wipId),
                    new SqlParameter("@FromDate", (object)fromDate?.Date ?? DBNull.Value),
                    new SqlParameter("@ToDate", (object)toDate?.Date ?? DBNull.Value)
                };

                DataTable dt = WipDonHangModel.ExecStoredProcedure("sp_Wip_Audit", parameters);
                if (dt == null) return Ok(list);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapAuditRow(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Ok(list);
        }
        #endregion
        // =========================
        // GET: api/wip-donhang/get
        // =========================
        #region Get
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? lineX = null,
            string mode = "CUT",
            double? lenhSX = null,
             bool? isGiaCong = null,
            string maHang = null,
            string khachHang = null,
            int skip = 0,
            int take = 1000,
            int isKetThuc = 0
        )
        {
            // normalize mode
            mode = (mode ?? "CUT").Trim().ToUpperInvariant();
            if (mode != "CUT" && mode != "OUT") mode = "CUT";

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Action", "Get"),
        new SqlParameter("@WIPId", DBNull.Value),

        new SqlParameter("@LineX", (object)lineX ?? DBNull.Value),
        new SqlParameter("@IsGiaCong", (object)isGiaCong ?? DBNull.Value),
        new SqlParameter("@LenhSX", (object)lenhSX ?? DBNull.Value),
        new SqlParameter("@StyleId", DBNull.Value),
        new SqlParameter("@MaHang", (object)maHang ?? DBNull.Value),
        new SqlParameter("@KhachHang", (object)khachHang ?? DBNull.Value),
        new SqlParameter("@PO", DBNull.Value),
        new SqlParameter("@InTheu", DBNull.Value),

        // ✅ NEW
        new SqlParameter("@Mode", (object)mode ?? DBNull.Value),
        new SqlParameter("@isKetThuc", (object)isKetThuc ?? DBNull.Value),

        new SqlParameter("@FromDate", (object)fromDate ?? DBNull.Value),
        new SqlParameter("@ToDate",   (object)toDate ?? DBNull.Value),

        // ❗ nếu SP vẫn còn param @KHCat/@ThoatChuyen thì set DBNull để khỏi lọc equality
        new SqlParameter("@KHCat", DBNull.Value),
        new SqlParameter("@ThoatChuyen", DBNull.Value),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            var list = new List<WIPDonHangChuyenViewModel>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            AttachStepProgressStatus(list);
            return Ok(list);
        }
        #endregion
        // =========================
        // GET: api/wip-donhang/loadline
        // =========================
        #region LoadLine (sync missing theo line + get)
        [HttpGet]
        [Route("loadline")]
        public IHttpActionResult LoadLine(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? lineX = null,
            double? lenhSX = null,
            string maHang = null,
            string khachHang = null,
            bool? isGiaCong = null
        )
        {
            var parameters = new SqlParameter[]
            {
    new SqlParameter("@Action", "LoadLine"),
    new SqlParameter("@WIPId", DBNull.Value),

    new SqlParameter("@LineX", (object)lineX ?? DBNull.Value),
    new SqlParameter("@IsGiaCong", (object)isGiaCong ?? DBNull.Value),
    new SqlParameter("@LenhSX", (object)lenhSX ?? DBNull.Value),
    new SqlParameter("@StyleId", DBNull.Value),
    new SqlParameter("@MaHang", (object)maHang ?? DBNull.Value),
    new SqlParameter("@KhachHang", (object)khachHang ?? DBNull.Value),
    new SqlParameter("@PO", DBNull.Value),
    new SqlParameter("@InTheu", DBNull.Value),

    new SqlParameter("@FromDate", (object)fromDate ?? DBNull.Value),
    new SqlParameter("@ToDate",   (object)toDate ?? DBNull.Value)
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            var list = new List<WIPDonHangChuyenViewModel>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            AttachStepProgressStatus(list);

            return Ok(list);
        }
        #endregion

        // =========================
        // POST: api/wip-donhang/save
        // =========================
        #region Save
        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(SaveWipRequest req)
        {
            if (req == null)
                return BadRequest("Body is empty");

            var model = req.Row;
            if (model == null)
                return BadRequest("Row is empty");
            string modifyBy = req.ModifyBy ?? "";
            // trim string fields trong model
            var properties = typeof(WIPDonHangChuyenViewModel).GetProperties();
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = prop.GetValue(model) as string;
                    if (string.IsNullOrWhiteSpace(value))
                        prop.SetValue(model, null);
                    else
                        prop.SetValue(model, value.Trim());
                }
            }

            var techSteps = GetTechStepMetadata();
            var fieldAliasMap = BuildFieldAliasMap(techSteps);
            var normalizedChanges = NormalizeChanges(req.Changes, fieldAliasMap);
            ApplyChangesToModel(model, normalizedChanges);

            model.ManualJsonPatch = NormalizeManualJsonPatchKeys(model.ManualJsonPatch, fieldAliasMap);

            // ChangedFieldsJson
            var changes = normalizedChanges
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Field))
                .Select(x => new SaveWipChange
                {
                    Field = x.Field.Trim(),
                    NewValue = x.NewValue
                })
                .GroupBy(x => x.Field, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last()) // lấy bản cuối nếu trùng
                .ToList();

            var changesJson = Newtonsoft.Json.JsonConvert.SerializeObject(changes);
            var parameters = new SqlParameter[]
            {
    new SqlParameter("@Action", "Save"),
    new SqlParameter("@ChangesJson", (object)changesJson ?? DBNull.Value),
    new SqlParameter("@ModifyBy", (object)modifyBy ?? DBNull.Value),
    new SqlParameter("@WIPId", (object)model.WIPId ?? DBNull.Value),
    new SqlParameter("@LineX", (object)model.LineX ?? DBNull.Value),
    new SqlParameter("@MaKH", (object)model.MaKH ?? DBNull.Value),
    new SqlParameter("@MaDH", (object)model.MaDH ?? DBNull.Value),
    new SqlParameter("@Season", (object)model.Season ?? DBNull.Value),
    new SqlParameter("@LenhSX", (object)model.LenhSX ?? DBNull.Value),
    new SqlParameter("@MaLenhSanXuat", (object)model.MaLenhSanXuat ?? DBNull.Value),
    new SqlParameter("@StyleId", (object)model.StyleId ?? DBNull.Value),
    new SqlParameter("@MaHang", (object)model.MaHang ?? DBNull.Value),
    new SqlParameter("@KhachHang", (object)model.KhachHang ?? DBNull.Value),
    new SqlParameter("@PO", (object)model.PO ?? DBNull.Value),
    new SqlParameter("@SLKH", (object)model.SLKH ?? DBNull.Value),
    new SqlParameter("@InTheu", (object)model.InTheu ?? DBNull.Value),
    new SqlParameter("@DoKim", (object)model.DoKim ?? DBNull.Value),
    new SqlParameter("@HutAm", (object)model.HutAm ?? DBNull.Value),
    // skip recalc
    new SqlParameter("@SkipCalc", (object)model.SkipCalc ?? DBNull.Value),
    // ===== In/Thêu mở rộng =====
    new SqlParameter("@InTheuTrong_PKH", (object)model.InTheuTrong_PKH ?? DBNull.Value),

    new SqlParameter("@KH_GuiInTheu", (object)model.KH_GuiInTheu ?? DBNull.Value),
    new SqlParameter("@TT_GuiInTheu", (object)model.TT_GuiInTheu ?? DBNull.Value),
    new SqlParameter("@KH_NhanInTheu", (object)model.KH_NhanInTheu ?? DBNull.Value),
    new SqlParameter("@TT_NhanInTheu", (object)model.TT_NhanInTheu ?? DBNull.Value),
    new SqlParameter("@NSGio", (object)model.NSGio ?? DBNull.Value),
    new SqlParameter("@SoGioSX", (object)model.SoGioSX ?? DBNull.Value),
    new SqlParameter("@SoNgay", (object)model.SoNgay ?? DBNull.Value),

    new SqlParameter("@KHCat", (object)model.KHCat ?? DBNull.Value),
    new SqlParameter("@TTCat", (object)model.TTCat ?? DBNull.Value),
    new SqlParameter("@KHLapTrinh", (object)model.KHLapTrinh ?? DBNull.Value),
    new SqlParameter("@TTLapTrinh", (object)model.TTLapTrinh ?? DBNull.Value),
    new SqlParameter("@KHMay", (object)model.KHMay ?? DBNull.Value),
    new SqlParameter("@TTMay", (object)model.TTMay ?? DBNull.Value),
    new SqlParameter("@ThoatChuyen", (object)model.ThoatChuyen ?? DBNull.Value),
    new SqlParameter("@TTThoatChuyen", (object)model.TTThoatChuyen ?? DBNull.Value),

    new SqlParameter("@TT_Cat", (object)model.TT_Cat ?? DBNull.Value),
    new SqlParameter("@RaChuyen", (object)model.RaChuyen ?? DBNull.Value),
    new SqlParameter("@BTP_DK", (object)model.BTP_DK ?? DBNull.Value),
    new SqlParameter("@Packing", (object)model.Packing ?? DBNull.Value),

    new SqlParameter("@KH_Top", (object)model.KH_Top ?? DBNull.Value),
    new SqlParameter("@SLSizeMau_Top", (object)model.SLSizeMau_Top ?? DBNull.Value),
    new SqlParameter("@TT_MauTop", (object)model.TT_MauTop ?? DBNull.Value),

    new SqlParameter("@KH_Ship", (object)model.KH_Ship ?? DBNull.Value),
    new SqlParameter("@SLSizeMau_Ship", (object)model.SLSizeMau_Ship ?? DBNull.Value),
    new SqlParameter("@TT_Ship", (object)model.TT_Ship ?? DBNull.Value),

    new SqlParameter("@KH_Vai", (object)model.KH_Vai ?? DBNull.Value),
    new SqlParameter("@TT_Vai", (object)model.TT_Vai ?? DBNull.Value),

    new SqlParameter("@KH_PLInEp", (object)model.KH_PLInEp ?? DBNull.Value),
    new SqlParameter("@TT_PLInEp", (object)model.TT_PLInEp ?? DBNull.Value),

    new SqlParameter("@KH_PLMay", (object)model.KH_PLMay ?? DBNull.Value),
    new SqlParameter("@TT_PLMay", (object)model.TT_PLMay ?? DBNull.Value),

    new SqlParameter("@KH_PackingList", (object)model.KH_PackingList ?? DBNull.Value),
    new SqlParameter("@TT_PackingList", (object)model.TT_PackingList ?? DBNull.Value),

    new SqlParameter("@KH_PLDongGoi", (object)model.KH_PLDongGoi ?? DBNull.Value),
    new SqlParameter("@TT_PLDongGoi", (object)model.TT_PLDongGoi ?? DBNull.Value),

    new SqlParameter("@KH_LenhSX", (object)model.KH_LenhSX ?? DBNull.Value),
    new SqlParameter("@TT_LenhSX", (object)model.TT_LenhSX ?? DBNull.Value),

    new SqlParameter("@SLCN", (object)model.SLCN ?? DBNull.Value),
    new SqlParameter("@TGLV", (object)model.TGLV ?? DBNull.Value),
    new SqlParameter("@OT", (object)model.OT ?? DBNull.Value),
    new SqlParameter("@HieuSuat", (object)model.HieuSuat ?? DBNull.Value),

    new SqlParameter("@SMV_WIP", (object)model.SMV_WIP ?? DBNull.Value),
    new SqlParameter("@NSCost", (object)model.NSCost ?? DBNull.Value),
    new SqlParameter("@NS_SMV", (object)model.NS_SMV ?? DBNull.Value),
    new SqlParameter("@CostPerM", (object)model.CostPerM ?? DBNull.Value),
    new SqlParameter("@CM", (object)model.CM ?? DBNull.Value),
    new SqlParameter("@HeSoCM", (object)model.HeSoCM ?? DBNull.Value),
    new SqlParameter("@DoanhThuCM", (object)model.DoanhThuCM ?? DBNull.Value),
    new SqlParameter("@Profit", (object)model.Profit ?? DBNull.Value),
    // ===== Phòng kỹ thuật =====
    new SqlParameter("@NgayNhanTT_BOM", (object)model.NgayNhanTT_BOM ?? DBNull.Value),
    new SqlParameter("@KH_BOM", (object)model.KH_BOM ?? DBNull.Value),
    new SqlParameter("@TT_BOM", (object)model.TT_BOM ?? DBNull.Value),

    new SqlParameter("@KH_NPL_BM_KTX_MayMau", (object)model.KH_NPL_BM_KTX_MayMau ?? DBNull.Value),
    new SqlParameter("@TT_NPL_BM_KTX_MayMau", (object)model.TT_NPL_BM_KTX_MayMau ?? DBNull.Value),

    new SqlParameter("@KH_TestKeoLogo_TapeLaser", (object)model.KH_TestKeoLogo_TapeLaser ?? DBNull.Value),
    new SqlParameter("@TT_TestKeoLogo_TapeLaser", (object)model.TT_TestKeoLogo_TapeLaser ?? DBNull.Value),

    new SqlParameter("@KH_Rap", (object)model.KH_Rap ?? DBNull.Value),
    new SqlParameter("@TT_Rap", (object)model.TT_Rap ?? DBNull.Value),

    new SqlParameter("@KH_TacNghiepCat", (object)model.KH_TacNghiepCat ?? DBNull.Value),
    new SqlParameter("@TT_TacNghiepCat", (object)model.TT_TacNghiepCat ?? DBNull.Value),

    new SqlParameter("@KH_SoDo", (object)model.KH_SoDo ?? DBNull.Value),
    new SqlParameter("@TT_SoDo", (object)model.TT_SoDo ?? DBNull.Value),

    new SqlParameter("@KH_BangMauVai_InEp", (object)model.KH_BangMauVai_InEp ?? DBNull.Value),
    new SqlParameter("@TT_BangMauVai_InEp", (object)model.TT_BangMauVai_InEp ?? DBNull.Value),

    new SqlParameter("@KH_BangMauPL_Full", (object)model.KH_BangMauPL_Full ?? DBNull.Value),
    new SqlParameter("@TT_BangMauPL_Full", (object)model.TT_BangMauPL_Full ?? DBNull.Value),

    new SqlParameter("@KH_BangDanhSo", (object)model.KH_BangDanhSo ?? DBNull.Value),
    new SqlParameter("@TT_BangDanhSo", (object)model.TT_BangDanhSo ?? DBNull.Value),

    new SqlParameter("@KH_TLieuInEp", (object)model.KH_TLieuInEp ?? DBNull.Value),
    new SqlParameter("@TT_TLieuInEp", (object)model.TT_TLieuInEp ?? DBNull.Value),

    new SqlParameter("@KH_TLieuLT", (object)model.KH_TLieuLT ?? DBNull.Value),
    new SqlParameter("@TT_TLieuLT", (object)model.TT_TLieuLT ?? DBNull.Value),

    new SqlParameter("@KH_TSCatPL", (object)model.KH_TSCatPL ?? DBNull.Value),
    new SqlParameter("@TT_TSCatPL", (object)model.TT_TSCatPL ?? DBNull.Value),

    new SqlParameter("@KH_CoiNut", (object)model.KH_CoiNut ?? DBNull.Value),
    new SqlParameter("@TT_CoiNut", (object)model.TT_CoiNut ?? DBNull.Value),

    new SqlParameter("@KH_Layout", (object)model.KH_Layout ?? DBNull.Value),
    new SqlParameter("@TT_Layout", (object)model.TT_Layout ?? DBNull.Value),

    new SqlParameter("@KH_NhanCare", (object)model.KH_NhanCare ?? DBNull.Value),
    new SqlParameter("@TT_NhanCare", (object)model.TT_NhanCare ?? DBNull.Value),
    new SqlParameter("@DateTrenNhanCare", (object)model.DateTrenNhanCare ?? DBNull.Value),

    new SqlParameter("@KH_CbiSXChoCoDien", (object)model.KH_CbiSXChoCoDien ?? DBNull.Value),
    new SqlParameter("@TT_CbiSXChoCoDien", (object)model.TT_CbiSXChoCoDien ?? DBNull.Value),

    new SqlParameter("@KH_TaiLieuHoanChinh", (object)model.KH_TaiLieuHoanChinh ?? DBNull.Value),
    new SqlParameter("@TT_TaiLieuHoanChinh", (object)model.TT_TaiLieuHoanChinh ?? DBNull.Value),

    new SqlParameter("@KH_KichThuoc", (object)model.KH_KichThuoc ?? DBNull.Value),
    new SqlParameter("@TT_KichThuoc", (object)model.TT_KichThuoc ?? DBNull.Value),

    new SqlParameter("@KH_QuyCachDongGoi", (object)model.KH_QuyCachDongGoi ?? DBNull.Value),
    new SqlParameter("@TT_QuyCachDongGoi", (object)model.TT_QuyCachDongGoi ?? DBNull.Value),

    new SqlParameter("@KH_HopTKSX", (object)model.KH_HopTKSX ?? DBNull.Value),
    new SqlParameter("@TT_HopTKSX", (object)model.TT_HopTKSX ?? DBNull.Value),

    new SqlParameter("@KH_NhanBaoThung", (object)model.KH_NhanBaoThung ?? DBNull.Value),
    new SqlParameter("@TT_NhanBaoThung", (object)model.TT_NhanBaoThung ?? DBNull.Value),

    new SqlParameter("@KH_MauDauChuyen", (object)model.KH_MauDauChuyen ?? DBNull.Value),
    new SqlParameter("@TT_MauDauChuyen", (object)model.TT_MauDauChuyen ?? DBNull.Value),

    new SqlParameter("@KH_DuyetMauDauChuyen", (object)model.KH_DuyetMauDauChuyen ?? DBNull.Value),
    new SqlParameter("@TT_DuyetMauDauChuyen", (object)model.TT_DuyetMauDauChuyen ?? DBNull.Value),

    // ===== IE =====
    new SqlParameter("@KH_NhuCauMayMoc", (object)model.KH_NhuCauMayMoc ?? DBNull.Value),
    new SqlParameter("@TT_NhuCauMayMoc", (object)model.TT_NhuCauMayMoc ?? DBNull.Value),

    new SqlParameter("@KH_Layout_IE", (object)model.KH_Layout_IE ?? DBNull.Value),
    new SqlParameter("@TT_Layout_IE", (object)model.TT_Layout_IE ?? DBNull.Value),

    new SqlParameter("@KH_QTMay", (object)model.KH_QTMay ?? DBNull.Value),
    new SqlParameter("@TT_QTMay", (object)model.TT_QTMay ?? DBNull.Value),

    new SqlParameter("@KH_DonGiaCongNhan", (object)model.KH_DonGiaCongNhan ?? DBNull.Value),
    new SqlParameter("@TT_DonGiaCongNhan", (object)model.TT_DonGiaCongNhan ?? DBNull.Value),

    // ===== Cơ điện =====
    new SqlParameter("@KH_ChuanBiMayMoc_KTX", (object)model.KH_ChuanBiMayMoc_KTX ?? DBNull.Value),
    new SqlParameter("@TT_ChuanBiMayMoc_KTX", (object)model.TT_ChuanBiMayMoc_KTX ?? DBNull.Value),

    new SqlParameter("@KH_ChuanBiMayMoc_Chuyen", (object)model.KH_ChuanBiMayMoc_Chuyen ?? DBNull.Value),
    new SqlParameter("@TT_ChuanBiMayMoc_Chuyen", (object)model.TT_ChuanBiMayMoc_Chuyen ?? DBNull.Value),

    // ===== Kho =====
    new SqlParameter("@KH_KhoVai", (object)model.KH_KhoVai ?? DBNull.Value),
    new SqlParameter("@TT_KhoVai", (object)model.TT_KhoVai ?? DBNull.Value),

    new SqlParameter("@KH_KhoPhuLieuMay", (object)model.KH_KhoPhuLieuMay ?? DBNull.Value),
    new SqlParameter("@TT_KhoPhuLieuMay", (object)model.TT_KhoPhuLieuMay ?? DBNull.Value),

    new SqlParameter("@KH_Thung", (object)model.KH_Thung ?? DBNull.Value),
    new SqlParameter("@TT_Thung", (object)model.TT_Thung ?? DBNull.Value),
    new SqlParameter("@GhiChuKho", (object)model.GhiChuKho ?? DBNull.Value),
    new SqlParameter("@ManualJsonPatch", (object)model.ManualJsonPatch ?? DBNull.Value),
    new SqlParameter("@Mer", (object)model.Mer ?? DBNull.Value),
    // SP có 2 param này nhưng Save không dùng: vẫn truyền NULL cho sạch
    new SqlParameter("@FromDate", DBNull.Value),
    new SqlParameter("@ToDate", DBNull.Value),
            };
            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            if (dt == null || dt.Rows.Count == 0)
                return Ok(new { success = true, items = new List<WIPDonHangChuyenViewModel>() });

            var items = new List<WIPDonHangChuyenViewModel>();
            foreach (DataRow row in dt.Rows)
                items.Add(MapRow(row));
            AttachStepProgressStatus(items);

            // ✅ trả list để FE patch vào datasource
            return Ok(new { success = true, items = items });
        }
        #endregion
        // =========================
        // POST: api/wip-donhang/save-delta
        // =========================
        #region SaveDelta
        [HttpPost]
        [Route("save-delta")]
        public IHttpActionResult SaveDelta(SaveWipDeltaRequest req)
        {
            if (req == null)
                return BadRequest("Body is empty");

            if (!req.WipId.HasValue || req.WipId.Value <= 0)
                return BadRequest("WipId is required");

            if (req.Changes == null || req.Changes.Count == 0)
                return BadRequest("Changes is required");

            var techSteps = GetTechStepMetadata();
            var fieldAliasMap = BuildFieldAliasMap(techSteps);
            var normalizedChanges = NormalizeDeltaChanges(req.Changes, fieldAliasMap);

            if (normalizedChanges.Count == 0)
                return BadRequest("Changes is empty after normalize");

            var saveDeltaFields = GetSaveDeltaFieldMetadata();
            var saveDeltaParentFields = BuildWritableSaveDeltaParentFieldSet(saveDeltaFields);
            var classified = ClassifyDeltaSave(normalizedChanges, req.ManualPatch, techSteps, fieldAliasMap, saveDeltaParentFields);
            if (classified.UnknownFields.Count > 0)
                return BadRequest("Unknown fields: " + string.Join(", ", classified.UnknownFields));

            var unsupportedParentFields = classified.ParentChanges
                .Where(x => !saveDeltaParentFields.Contains(x.Field))
                .Select(x => x.Field)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var unsupportedStepFields = classified.StepChanges
                .Where(x => !string.Equals(x.Role, "KH", StringComparison.OrdinalIgnoreCase)
                         && !string.Equals(x.Role, "TT", StringComparison.OrdinalIgnoreCase)
                         && !string.Equals(x.Role, "NoData", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Field)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var unsupportedManualFields = classified.ManualChanges
                .Where(x => !(
                    (string.Equals(x.Scope, "step", StringComparison.OrdinalIgnoreCase)
                        && (string.Equals(x.Target, "KH", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(x.Target, "TT", StringComparison.OrdinalIgnoreCase)))
                    || (string.Equals(x.Scope, "parent", StringComparison.OrdinalIgnoreCase)
                        && saveDeltaParentFields.Contains(x.Field))))
                .Select(x => x.Field)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (unsupportedParentFields.Count > 0 || unsupportedStepFields.Count > 0 || unsupportedManualFields.Count > 0)
            {
                var unsupported = unsupportedParentFields
                    .Concat(unsupportedStepFields)
                    .Concat(unsupportedManualFields)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return BadRequest("save-delta does not support these fields from registry/tech metadata: " + string.Join(", ", unsupported));
            }

            var saveJson = JsonConvert.SerializeObject(new
            {
                wipId = req.WipId.Value,
                changes = classified.ParentChanges
                    .Select(x => new
                    {
                        field = x.Field,
                        value = x.Value
                    })
                    .Concat(classified.StepChanges
                        .Where(x => string.Equals(x.Role, "KH", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(x.Role, "TT", StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(x.Role, "NoData", StringComparison.OrdinalIgnoreCase))
                        .Select(x => new
                        {
                            field = x.Field,
                            value = x.Value
                        }))
                    .ToList(),
                manualPatch = classified.ManualChanges
                    .Where(x =>
                        (string.Equals(x.Scope, "step", StringComparison.OrdinalIgnoreCase)
                            && (string.Equals(x.Target, "KH", StringComparison.OrdinalIgnoreCase)
                             || string.Equals(x.Target, "TT", StringComparison.OrdinalIgnoreCase)))
                        || (string.Equals(x.Scope, "parent", StringComparison.OrdinalIgnoreCase)
                            && saveDeltaParentFields.Contains(x.Field)))
                    .GroupBy(x => x.Field, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Last().Value, StringComparer.OrdinalIgnoreCase),
                modifyBy = req.ModifyBy
            });

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "SaveDelta"),
                new SqlParameter("@SaveJson", (object)saveJson ?? DBNull.Value),
                new SqlParameter("@ModifyBy", (object)req.ModifyBy ?? DBNull.Value),
                new SqlParameter("@isKetThuc", req.IsKetThuc)
            };

            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
            var items = new List<WIPDonHangChuyenViewModel>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                    items.Add(MapRow(row));
            }
            AttachStepProgressStatus(items);
            var resolvedLineX = items.Select(x => x.LineX).FirstOrDefault(x => x.HasValue);

            return Ok(new
            {
                success = true,
                mode = "save",
                wipId = req.WipId,
                lineX = resolvedLineX,
                modifyBy = req.ModifyBy,
                parentChanges = classified.ParentChanges,
                stepChanges = classified.StepChanges,
                manualChanges = classified.ManualChanges,
                items = items
            });
        }
        #endregion
        // POST: api/wip-donhang/reorder
        #region ReOrder
        [HttpPost]
        [Route("reorder")]
        public IHttpActionResult Reorder(WIPReorderRequest req)
        {
            if (req == null) return BadRequest("Body is empty");
            if (req.LineX <= 0) return BadRequest("LineX is required");
            if (req.FromThuTu <= 0 || req.ToThuTu <= 0) return BadRequest("FromThuTu/ToThuTu must be > 0");

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "Reorder"),
                new SqlParameter("@LineX", req.LineX),
                new SqlParameter("@FromThuTu", req.FromThuTu),
                new SqlParameter("@ToThuTu", req.ToThuTu),
                new SqlParameter("@isKetThuc", req.IsKetThuc),
            };

            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            // SP Reorder đang SELECT * trả list => map về list
            var list = new List<WIPDonHangChuyenViewModel>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                    list.Add(MapRow(row));
            }
            AttachStepProgressStatus(list);

            return Ok(list);
        }
        #endregion
        // =========================
        // POST: api/wip-donhang/recalc-line
        // =========================
        #region RecalcLine
        [HttpPost]
        [Route("recalc-line")]
        public IHttpActionResult RecalcLine(WIPRecalcLineRequest req)
        {
            if (req == null) return BadRequest("Body is empty");
            if (req.LineX <= 0) return BadRequest("LineX is required");

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "RecalcLine"),
                new SqlParameter("@LineX", req.LineX),
                new SqlParameter("@FromThuTu", (object)req.FromThuTu ?? DBNull.Value),
                new SqlParameter("@ModifyBy", (object)req.ModifyBy ?? DBNull.Value),
                new SqlParameter("@isKetThuc", (object)req.IsKetThuc ?? DBNull.Value),
            };

            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            var list = new List<WIPDonHangChuyenViewModel>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                    list.Add(MapRow(row));
            }
            AttachStepProgressStatus(list);

            return Ok(new { success = true, items = list });
        }
        #endregion
        // =========================
        // POST: api/wip-donhang/assign-to-line
        // =========================
        #region assignToLine
        [HttpPost]
        [Route("assign-to-line")]
        public IHttpActionResult AssignToLine(AssignToLineRequest req)
        {
            if (req == null) return BadRequest("Body is empty");
            if (!req.LineX.HasValue || req.LineX.Value <= 0) return BadRequest("LineX is required");
            if (req.LenhSX == null || req.LenhSX <= 0) return BadRequest("LenhSX is required");
            var items = req.Items
            .Where(x => x != null && x.WipId > 0)
            .GroupBy(x => x.WipId)
            .Select(g => g.First())
            .ToList();
            if (items.Count == 0) return BadRequest("Items must contain valid WipId");

            var itemsJson = JsonConvert.SerializeObject(items);

            var parameters = new SqlParameter[]
            {
    new SqlParameter("@Action", "AssignToLine"),
    new SqlParameter("@LineX", req.LineX.Value),
        new SqlParameter("@itemsJson", (object)itemsJson ?? DBNull.Value),
    new SqlParameter("@KeysJson", DBNull.Value),
    new SqlParameter("@Mer", req.mer),
    new SqlParameter("@MaDVSX", req.MaDVSX),
    // các param khác của SP: set NULL
    new SqlParameter("@WIPId", DBNull.Value),
    new SqlParameter("@ThuTuChuyen", DBNull.Value),
    new SqlParameter("@LenhSX", (object)req.LenhSX ?? DBNull.Value),
    new SqlParameter("@MaLenhSanXuat", (object)req.MaLenhSanXuat ?? DBNull.Value),
    new SqlParameter("@MaKH", DBNull.Value),
    new SqlParameter("@Season", DBNull.Value),
    new SqlParameter("@StyleId", DBNull.Value),
    new SqlParameter("@MaHang", DBNull.Value),
    new SqlParameter("@KhachHang", DBNull.Value),
    new SqlParameter("@PO", DBNull.Value),
    new SqlParameter("@SLKH", DBNull.Value),
    new SqlParameter("@InTheu", DBNull.Value),

    new SqlParameter("@NSGio", DBNull.Value),
    new SqlParameter("@SoGioSX", DBNull.Value),
    new SqlParameter("@SoNgay", DBNull.Value),

    new SqlParameter("@KHCat", DBNull.Value),
    new SqlParameter("@TTCat", DBNull.Value),
    new SqlParameter("@KHLapTrinh", DBNull.Value),
    new SqlParameter("@TTLapTrinh", DBNull.Value),
    new SqlParameter("@KHMay", DBNull.Value),
    new SqlParameter("@TTMay", DBNull.Value),
    new SqlParameter("@ThoatChuyen", DBNull.Value),
    new SqlParameter("@TTThoatChuyen", DBNull.Value),

    new SqlParameter("@TT_Cat", DBNull.Value),
    new SqlParameter("@RaChuyen", DBNull.Value),
    new SqlParameter("@BTP_DK", DBNull.Value),

    new SqlParameter("@KH_Top", DBNull.Value),
    new SqlParameter("@SLSizeMau_Top", DBNull.Value),
    new SqlParameter("@TT_MauTop", DBNull.Value),

    new SqlParameter("@KH_Ship", DBNull.Value),
    new SqlParameter("@SLSizeMau_Ship", DBNull.Value),
    new SqlParameter("@TT_Ship", DBNull.Value),

    new SqlParameter("@KH_Vai", DBNull.Value),
    new SqlParameter("@TT_Vai", DBNull.Value),

    new SqlParameter("@KH_PLInEp", DBNull.Value),
    new SqlParameter("@TT_PLInEp", DBNull.Value),

    new SqlParameter("@KH_PLMay", DBNull.Value),
    new SqlParameter("@TT_PLMay", DBNull.Value),

    new SqlParameter("@KH_PackingList", DBNull.Value),
    new SqlParameter("@TT_PackingList", DBNull.Value),

    new SqlParameter("@KH_PLDongGoi", DBNull.Value),
    new SqlParameter("@TT_PLDongGoi", DBNull.Value),

    new SqlParameter("@KH_LenhSX", DBNull.Value),
    new SqlParameter("@TT_LenhSX", DBNull.Value),

    new SqlParameter("@SLCN", DBNull.Value),
    new SqlParameter("@TGLV", DBNull.Value),
    new SqlParameter("@OT", DBNull.Value),
    new SqlParameter("@HieuSuat", DBNull.Value),

    new SqlParameter("@CostPerM", DBNull.Value),
    new SqlParameter("@Profit", DBNull.Value),

    new SqlParameter("@SMV_WIP", DBNull.Value),
    new SqlParameter("@NSCost", DBNull.Value),
    new SqlParameter("@NS_SMV", DBNull.Value),

    new SqlParameter("@CM", DBNull.Value),
    new SqlParameter("@HeSoCM", DBNull.Value),
    new SqlParameter("@DoanhThuCM", DBNull.Value),

    new SqlParameter("@FromDate", DBNull.Value),
    new SqlParameter("@ToDate", DBNull.Value),
    new SqlParameter("@FromThuTu", DBNull.Value),
    new SqlParameter("@ToThuTu", DBNull.Value),
            };
            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            // SP AssignToLine đang SELECT list của lineX => map về list
            int affected = items.Count;
            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("Affected"))
                affected = Convert.ToInt32(dt.Rows[0]["Affected"]);

            return Ok(new { success = true, lineX = req.LineX, lenhSX = req.LenhSX, affected });
        }
        #endregion
        // =========================
        // POST: api/wip-donhang/assign-outsource
        // =========================
        #region AssignOutsource
        [HttpPost]
        [Route("assign-outsource")]
        public IHttpActionResult AssignOutsource(AssignToLineRequest req)
        {
            if (req == null) return BadRequest("Body is empty");
            if (req.LenhSX == null || req.LenhSX <= 0) return BadRequest("LenhSX is required");

            var items = req.Items
            .Where(x => x != null && x.WipId > 0)
            .GroupBy(x => x.WipId)
            .Select(g => g.First())
            .ToList();
            if (items.Count == 0) return BadRequest("Items must contain valid WipId");

            var itemsJson = JsonConvert.SerializeObject(items);

            var parameters = new SqlParameter[]
            {
    new SqlParameter("@Action", "AssignToOutsource"),
    new SqlParameter("@LineX", DBNull.Value),
    new SqlParameter("@itemsJson", (object)itemsJson ?? DBNull.Value),
    new SqlParameter("@KeysJson", DBNull.Value),
    new SqlParameter("@Mer", req.mer),

    // các param khác của SP: set NULL
    //new SqlParameter("@WIPId", DBNull.Value),
    new SqlParameter("@LenhSX", (object)req.LenhSX ?? DBNull.Value),
    new SqlParameter("@MaLenhSanXuat", (object)req.MaLenhSanXuat ?? DBNull.Value),
    new SqlParameter("@MaDVSX", (object)req.MaDVSX ?? DBNull.Value),

            };

            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
            int affected = items.Count;
            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("Affected"))
                affected = Convert.ToInt32(dt.Rows[0]["Affected"]);

            return Ok(new { success = true, isGiaCong = true, lenhSX = req.LenhSX, affected });
        }
        #endregion
        // =========================
        // GET: api/linemap/get-list-linex
        // =========================
            #region GetLineName
            [HttpGet]
            [Route("get-list-linex")]
            public IHttpActionResult GetName()
            {
                var parameters = new SqlParameter[]
                {
        new SqlParameter("@Action", "GetLineX"),
                };

                DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

                var list = new List<LineMapListViewModel>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new LineMapListViewModel
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        Name = row["Name"].ToString(),
                    });
                }
                return Ok(list);
            }
        #endregion

        // =========================
        // POST: api/wip-donhang/changeLine
        // =========================
        #region ChangeLine
        [HttpPost]
        [Route("changeLine")]
        public IHttpActionResult ChangeLine(WipChangeLineRequest req)
        {
            if (req == null) return BadRequest("Body is empty");
            if (string.IsNullOrWhiteSpace(req.MaGop)) return BadRequest("MaGop is required");
            if (string.IsNullOrWhiteSpace(req.MaLenhSanXuat)) return BadRequest("MaLenhSanXuat is required");
            if (req.OldLine <= 0) return BadRequest("OldLine must be > 0");
            if (req.NewLine <= 0) return BadRequest("NewLine must be > 0");

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "ChangeLine"),
                new SqlParameter("@MaGop", req.MaGop),
                new SqlParameter("@MaLenhSanXuat", req.MaLenhSanXuat),
                new SqlParameter("@LineX", req.OldLine),
                new SqlParameter("@LineMoi", req.NewLine),
                new SqlParameter("@FromThuTu", (object)req.FromThuTu ?? DBNull.Value),
            };

            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            return Ok(new { ok = true, data = dt });
        }
        #endregion
        // =========================
        // GET: api/wip-donhang/sync-by-madh
        // =========================
        #region SyncByMaDH
        [HttpGet]
        [Route("sync-by-madh")]
        public IHttpActionResult SyncByMaDH(string maDH)
        {
            if (string.IsNullOrWhiteSpace(maDH))
                return BadRequest("maDH is required");

            maDH = maDH.Trim();

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Action", "SyncByMaDH"),

        new SqlParameter("@MaDH", (object)maDH ?? DBNull.Value),

        // giữ style truyền NULL cho các param còn lại
        new SqlParameter("@WIPId", DBNull.Value),
        new SqlParameter("@LineX", DBNull.Value),
        new SqlParameter("@LenhSX", DBNull.Value),
        new SqlParameter("@StyleId", DBNull.Value),
        new SqlParameter("@MaHang", DBNull.Value),
        new SqlParameter("@KhachHang", DBNull.Value),
        new SqlParameter("@PO", DBNull.Value),
        new SqlParameter("@InTheu", DBNull.Value),

        new SqlParameter("@FromDate", DBNull.Value),
        new SqlParameter("@ToDate", DBNull.Value),

        // các json params nếu SP có (vì signature bạn có KeysJson/itemsJson)
        new SqlParameter("@KeysJson", DBNull.Value),
        new SqlParameter("@itemsJson", DBNull.Value)
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);

            var list = new List<WIPDonHangChuyenViewModel>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                    list.Add(MapRow(row));
            }
            AttachStepProgressStatus(list);

            // bạn thích trả y chang get/loadline thì return Ok(list)
            // hoặc bọc thêm maDH cũng được
            return Ok(new { ok = true, maDH = maDH, items = list });
        }
        #endregion
        // =========================
        // GET: api/wip-donhang/header-colors
        // =========================
        #region GetHeaderColors
        [HttpGet]
        [Route("header-colors")]
        public IHttpActionResult GetHeaderColors()
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "get"),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("sp_wip_color", parameters);
            var list = new List<WipColorDto>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new WipColorDto
                    {
                        CssClass = row.Table.Columns.Contains("CssClass") && row["CssClass"] != DBNull.Value
                            ? row["CssClass"].ToString()
                            : null,
                        DisplayName = row.Table.Columns.Contains("DisplayName") && row["DisplayName"] != DBNull.Value
                            ? row["DisplayName"].ToString()
                            : null,
                        ColorCode = row.Table.Columns.Contains("ColorCode") && row["ColorCode"] != DBNull.Value
                            ? row["ColorCode"].ToString()
                            : null,
                    });
                }
            }

            return Ok(list);
        }
        #endregion

        // =========================
        // GET: api/wip-donhang/save-delta-fields
        // =========================
        #region GetSaveDeltaFields
        [HttpGet]
        [Route("save-delta-fields")]
        public IHttpActionResult GetSaveDeltaFields()
        {
            var fields = GetSaveDeltaFieldMetadata()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.FieldName)
                .Select(x => new
                {
                    x.FieldName,
                    x.TargetScope,
                    x.TargetColumn,
                    x.DataType,
                    x.RecalcGroup,
                    x.ManualGroup,
                    x.IsWritable,
                    x.SortOrder
                })
                .ToList();

            return Ok(new { fields = fields });
        }
        #endregion

        // =========================
        // GET: api/wip-donhang/step-status-meta
        // =========================
        #region GetStepStatusMeta
        [HttpGet]
        [Route("step-status-meta")]
        public IHttpActionResult GetStepStatusMeta()
        {
            var statusCodes = new List<int> { 0, 1, 2, 3 };
            var steps = GetTechStepMetadata();

            var items = (steps ?? new List<TechStepMeta>())
                .Where(x => x != null
                    && !string.IsNullOrWhiteSpace(x.StepCode)
                    && !string.IsNullOrWhiteSpace(x.LegacyKHColumn)
                    && !string.IsNullOrWhiteSpace(x.LegacyTTColumn))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.StepCode)
                .Select(x => new StepStatusMetaItemDto
                {
                    StepCode = x.StepCode,
                    StepName = x.StepName,
                    SortOrder = x.SortOrder,
                    KHField = x.LegacyKHColumn,
                    TTField = x.LegacyTTColumn,
                    IsCheckTT = x.IsCheckTT,
                    AllowManualKH = x.AllowManualKH,
                    AllowNoDataConfirm = x.AllowNoDataConfirm,
                    NoDataField = x.NoDataField,
                    NoDataDateField = x.NoDataDateField,
                    StatusField = x.LegacyTTColumn + "_Status",
                    LateLockedField = x.LegacyTTColumn + "_IsLateLocked"
                })
                .ToList();

            var compareTtColumns = items
                .Where(x => x.IsCheckTT && !string.IsNullOrWhiteSpace(x.TTField))
                .Select(x => x.TTField)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return Ok(new StepStatusMetaResponseDto
            {
                StatusCodes = statusCodes,
                CompareTtColumns = compareTtColumns,
                Steps = items
            });
        }
        #endregion

        // =========================
        // GET: api/wip-donhang/column-dynamic
        // =========================
        #region GetColumnDynamic
        [HttpGet]
        [Route("column-dynamic")]
        public IHttpActionResult GetColumnDynamic()
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "GetColumnDynamic"),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
            var list = new List<ColumnDynamicDto>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ColumnDynamicDto
                    {
                        Id = row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value
                            ? Convert.ToInt32(row["Id"])
                            : 0,
                        Code = row.Table.Columns.Contains("Code") && row["Code"] != DBNull.Value
                            ? row["Code"].ToString()
                            : null,
                        ParentId = row.Table.Columns.Contains("ParentId") && row["ParentId"] != DBNull.Value
                            ? (int?)Convert.ToInt32(row["ParentId"])
                            : null,
                        ParentCode = row.Table.Columns.Contains("ParentCode") && row["ParentCode"] != DBNull.Value
                            ? row["ParentCode"].ToString()
                            : null,
                        Module = row.Table.Columns.Contains("Module") && row["Module"] != DBNull.Value
                            ? Convert.ToInt32(row["Module"])
                            : 0,
                        BandCode = row.Table.Columns.Contains("BandCode") && row["BandCode"] != DBNull.Value
                            ? row["BandCode"].ToString()
                            : null,
                        Caption = row.Table.Columns.Contains("Caption") && row["Caption"] != DBNull.Value
                            ? row["Caption"].ToString()
                            : null,
                        DataField = row.Table.Columns.Contains("DataField") && row["DataField"] != DBNull.Value
                            ? row["DataField"].ToString()
                            : null,
                        StepCode = row.Table.Columns.Contains("StepCode") && row["StepCode"] != DBNull.Value
                            ? row["StepCode"].ToString()
                            : null,
                        DataType = row.Table.Columns.Contains("DataType") && row["DataType"] != DBNull.Value
                            ? row["DataType"].ToString()
                            : null,
                        RowSpan = row.Table.Columns.Contains("RowSpan") && row["RowSpan"] != DBNull.Value
                            ? Convert.ToInt32(row["RowSpan"])
                            : 1,
                        ColSpan = row.Table.Columns.Contains("ColSpan") && row["ColSpan"] != DBNull.Value
                            ? Convert.ToInt32(row["ColSpan"])
                            : 1,
                        STT = row.Table.Columns.Contains("STT") && row["STT"] != DBNull.Value
                            ? Convert.ToInt32(row["STT"])
                            : 0,
                        IsCheckTT = row.Table.Columns.Contains("IsCheckTT") && row["IsCheckTT"] != DBNull.Value
                            ? Convert.ToBoolean(row["IsCheckTT"])
                            : false,
                        AllowManualKH = row.Table.Columns.Contains("AllowManualKH") && row["AllowManualKH"] != DBNull.Value
                            ? Convert.ToBoolean(row["AllowManualKH"])
                            : false,
                    });
                }
            }

            return Ok(list);
        }
        #endregion

        // =========================
        // GET: api/wip-donhang/step-cell-status
        // =========================
        #region GetStepCellStatus
        [HttpGet]
        [Route("step-cell-status")]
        public IHttpActionResult GetStepCellStatus(
            int? wipId = null,
            int? lineX = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string mode = "CUT",
            double? lenhSX = null
        )
        {
            mode = (mode ?? "CUT").Trim().ToUpperInvariant();
            if (mode != "CUT" && mode != "OUT") mode = "CUT";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "GetStepCellStatus"),
                new SqlParameter("@WIPId", (object)wipId ?? DBNull.Value),
                new SqlParameter("@LineX", (object)lineX ?? DBNull.Value),
                new SqlParameter("@LenhSX", (object)lenhSX ?? DBNull.Value),
                new SqlParameter("@Mode", (object)mode ?? DBNull.Value),
                new SqlParameter("@FromDate", (object)fromDate ?? DBNull.Value),
                new SqlParameter("@ToDate", (object)toDate ?? DBNull.Value),
                new SqlParameter("@itemsJson", DBNull.Value),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
            var list = new List<StepCellStatusDto>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new StepCellStatusDto
                    {
                        WIPId = row.Table.Columns.Contains("WIPId") && row["WIPId"] != DBNull.Value
                            ? Convert.ToInt32(row["WIPId"])
                            : 0,
                        StepCode = row.Table.Columns.Contains("StepCode") && row["StepCode"] != DBNull.Value
                            ? row["StepCode"].ToString()
                            : null,
                        StepName = row.Table.Columns.Contains("StepName") && row["StepName"] != DBNull.Value
                            ? row["StepName"].ToString()
                            : null,
                        KHField = row.Table.Columns.Contains("KHField") && row["KHField"] != DBNull.Value
                            ? row["KHField"].ToString()
                            : null,
                        TTField = row.Table.Columns.Contains("TTField") && row["TTField"] != DBNull.Value
                            ? row["TTField"].ToString()
                            : null,
                        DataField = row.Table.Columns.Contains("DataField") && row["DataField"] != DBNull.Value
                            ? row["DataField"].ToString()
                            : null,
                        Status = row.Table.Columns.Contains("Status") && row["Status"] != DBNull.Value
                            ? (int?)Convert.ToInt32(row["Status"])
                            : null,
                        IsLateLocked = row.Table.Columns.Contains("IsLateLocked") && row["IsLateLocked"] != DBNull.Value
                            ? (bool?)Convert.ToBoolean(row["IsLateLocked"])
                            : null,
                        IsMissingTTWarning = row.Table.Columns.Contains("IsMissingTTWarning") && row["IsMissingTTWarning"] != DBNull.Value
                            ? (bool?)Convert.ToBoolean(row["IsMissingTTWarning"])
                            : null
                    });
                }
            }

            return Ok(list);
        }
        #endregion

        // =========================
        // POST: api/wip-donhang/header-colors/update
        // =========================
        #region UpdateHeaderColor
        [HttpPost]
        [Route("header-colors/update")]
        public IHttpActionResult UpdateHeaderColor(UpdateHeaderColorRequest req)
        {
            if (req == null)
                return BadRequest("Body is empty");
            if (string.IsNullOrWhiteSpace(req.CssClass))
                return BadRequest("CssClass is required");
            if (string.IsNullOrWhiteSpace(req.ColorCode))
                return BadRequest("ColorCode is required");

            var actorUser = ResolveActorUserName(req.UserID);
            if (string.IsNullOrWhiteSpace(actorUser))
                return BadRequest("UserID is required");

            if (!HasFeaturePermission(actorUser, HeaderColorConfigFeatureKey))
                return Content(HttpStatusCode.Forbidden, "Bạn không có quyền tùy chỉnh màu header.");

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "update"),
                new SqlParameter("@CssClass", req.CssClass.Trim()),
                new SqlParameter("@ColorCode", req.ColorCode.Trim()),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("sp_wip_color", parameters);

            var result = new WipColorUpdateResultDto
            {
                IsSuccess = false,
                Message = "Update header color failed"
            };

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                result.IsSuccess = row.Table.Columns.Contains("IsSuccess") && row["IsSuccess"] != DBNull.Value
                    ? Convert.ToBoolean(row["IsSuccess"])
                    : false;
                result.Message = row.Table.Columns.Contains("Message") && row["Message"] != DBNull.Value
                    ? row["Message"].ToString()
                    : result.Message;
            }

            return Ok(result);
        }
        #endregion
        // =========================
        // POST: api/wip-donhang/update-mer
        // =========================
        #region
        [HttpPost]
        [Route("update-mer")]
        public IHttpActionResult UpdateMer(UpdateMerRequest updateMer)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "UpdateMer"),
                new SqlParameter("@MaLenhSanXuat", updateMer.MaLenhSanXuat),
                new SqlParameter("@MaDH", updateMer.MaDH),
                new SqlParameter("@LineMoi", updateMer.NewLine),
                new SqlParameter("@Mer", updateMer.Mer),
            };
            WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
            return Ok();
        }
        #endregion
        // =========================
        // POST: api/wip-donhang/syncLib
        // =========================
        #region SyncLib
        [HttpPost]
        [Route("syncLib")]
        public IHttpActionResult SyncLib(SyncLibRequest req)
        {
            if(req.LineX <= 0) return BadRequest("LineX must be > 0");
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "SyncLib"),
                new SqlParameter("@LineX", req.LineX),
            };
            WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
            return Ok();
        }
        #endregion
        private string ResolveActorUserName(string fallbackUserId)
        {
            var principalName = RequestContext?.Principal?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(principalName))
                return principalName.Trim();

            return string.IsNullOrWhiteSpace(fallbackUserId) ? null : fallbackUserId.Trim();
        }

        // Feature permission check is centralized so protected endpoints share one rule.
        private static bool HasFeaturePermission(string userId, string featureKey)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(featureKey))
                return false;

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Action", "GET_ALLOWED_FEATURE_KEYS"),
                new SqlParameter("@UserID", userId.Trim()),
                new SqlParameter("@FeatureKey", DBNull.Value),
                new SqlParameter("@IsAllow", DBNull.Value)
            };

            var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_FEATURE", parameters);
            if (dt == null || dt.Rows.Count == 0)
                return false;

            foreach (DataRow row in dt.Rows)
            {
                var key = row.Table.Columns.Contains("FeatureKey") && row["FeatureKey"] != DBNull.Value
                    ? row["FeatureKey"].ToString()
                    : null;

                if (string.Equals((key ?? string.Empty).Trim(), featureKey.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static void AttachStepProgressStatus(IList<WIPDonHangChuyenViewModel> items)
        {
            if (items == null || items.Count == 0)
                return;

            var validItems = items
                .Where(x => x != null && x.WIPId > 0)
                .ToList();
            if (validItems.Count == 0)
                return;

            var unresolvedItems = validItems
                .Where(x =>
                {
                    var hasInlineMap =
                        (x.StepStatusByTtField != null && x.StepStatusByTtField.Count > 0)
                        || (x.StepLateLockedByTtField != null && x.StepLateLockedByTtField.Count > 0)
                        || (x.StepMissingTTWarningByTtField != null && x.StepMissingTTWarningByTtField.Count > 0);
                    var hasManualMap = x.StepManualByField != null;
                    if (!hasManualMap)
                        return true;
                    return !hasInlineMap;
                })
                .ToList();
            if (unresolvedItems.Count == 0)
                return;

            foreach (var model in unresolvedItems)
            {
                if (model.StepStatusByTtField == null)
                    model.StepStatusByTtField = new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase);
                if (model.StepLateLockedByTtField == null)
                    model.StepLateLockedByTtField = new Dictionary<string, bool?>(StringComparer.OrdinalIgnoreCase);
                if (model.StepMissingTTWarningByTtField == null)
                    model.StepMissingTTWarningByTtField = new Dictionary<string, bool?>(StringComparer.OrdinalIgnoreCase);
                if (model.StepManualByField == null)
                    model.StepManualByField = new Dictionary<string, bool?>(StringComparer.OrdinalIgnoreCase);
            }

            var byWipId = unresolvedItems
                .GroupBy(x => x.WIPId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var wipIds = byWipId.Keys.ToList();
            if (wipIds.Count == 0)
                return;

            try
            {
                var itemsJson = JsonConvert.SerializeObject(wipIds.Select(id => new { WipId = id }).ToList());
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "GetStepCellStatus"),
                    new SqlParameter("@itemsJson", (object)itemsJson ?? DBNull.Value),
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
                if (dt == null || dt.Rows.Count == 0)
                    return;

                foreach (DataRow row in dt.Rows)
                {
                    var wipId = row.Table.Columns.Contains("WIPId") && row["WIPId"] != DBNull.Value
                        ? Convert.ToInt32(row["WIPId"])
                        : 0;
                    var ttField = row.Table.Columns.Contains("TTField") && row["TTField"] != DBNull.Value
                        ? row["TTField"].ToString()
                        : (row.Table.Columns.Contains("LegacyTTColumn") && row["LegacyTTColumn"] != DBNull.Value
                            ? row["LegacyTTColumn"].ToString()
                            : null);
                    if (wipId <= 0 || string.IsNullOrWhiteSpace(ttField))
                        continue;

                    var status = row.Table.Columns.Contains("Status") && row["Status"] != DBNull.Value
                        ? (int?)Convert.ToInt32(row["Status"])
                        : null;
                    if (status.HasValue)
                    {
                        if (status.Value < 0) status = 0;
                        if (status.Value > 3) status = 3;
                    }

                    var isLateLocked = row.Table.Columns.Contains("IsLateLocked") && row["IsLateLocked"] != DBNull.Value
                        ? (bool?)Convert.ToBoolean(row["IsLateLocked"])
                        : null;
                    var isMissingWarning = row.Table.Columns.Contains("IsMissingTTWarning") && row["IsMissingTTWarning"] != DBNull.Value
                        ? (bool?)Convert.ToBoolean(row["IsMissingTTWarning"])
                        : null;
                    var khField = row.Table.Columns.Contains("KHField") && row["KHField"] != DBNull.Value
                        ? row["KHField"].ToString()
                        : (row.Table.Columns.Contains("LegacyKHColumn") && row["LegacyKHColumn"] != DBNull.Value
                            ? row["LegacyKHColumn"].ToString()
                            : null);
                    var isManualKH = row.Table.Columns.Contains("IsManualKH") && row["IsManualKH"] != DBNull.Value
                        ? (bool?)Convert.ToBoolean(row["IsManualKH"])
                        : null;
                    var isManualTT = row.Table.Columns.Contains("IsManualTT") && row["IsManualTT"] != DBNull.Value
                        ? (bool?)Convert.ToBoolean(row["IsManualTT"])
                        : null;
                    List<WIPDonHangChuyenViewModel> rows;
                    if (!byWipId.TryGetValue(wipId, out rows))
                        continue;

                    foreach (var model in rows)
                    {
                        model.StepStatusByTtField[ttField] = status;
                        model.StepLateLockedByTtField[ttField] = isLateLocked;
                        model.StepMissingTTWarningByTtField[ttField] = isMissingWarning;
                        if (!string.IsNullOrWhiteSpace(khField) && isManualKH.HasValue)
                            model.StepManualByField[khField] = isManualKH;
                        if (!string.IsNullOrWhiteSpace(ttField) && isManualTT.HasValue)
                            model.StepManualByField[ttField] = isManualTT;
                        if (string.Equals(ttField, "TT_BOM", StringComparison.OrdinalIgnoreCase))
                        {
                            model.TT_BOM_IsLateLocked = isLateLocked;
                            model.TT_BOM_Status = status.HasValue
                                ? (byte?)Convert.ToByte(status.Value)
                                : null;
                        }
                    }
                }
            }
            catch
            {
                // Best-effort adapter: nếu có lỗi query map status thì vẫn trả dữ liệu phẳng như cũ.
            }
        }

        #region Save Adapter
        private static List<NormalizedDeltaChange> NormalizeDeltaChanges(IEnumerable<SaveWipDeltaChange> rawChanges, IDictionary<string, string> fieldAliasMap)
        {
            var list = new List<NormalizedDeltaChange>();
            foreach (var c in rawChanges ?? Enumerable.Empty<SaveWipDeltaChange>())
            {
                if (c == null || string.IsNullOrWhiteSpace(c.Field))
                    continue;

                var field = ResolveFieldAlias(c.Field, fieldAliasMap);
                if (string.IsNullOrWhiteSpace(field))
                    continue;

                list.Add(new NormalizedDeltaChange
                {
                    Field = field.Trim(),
                    Value = UnwrapJsonValue(c.Value)
                });
            }

            return list
                .GroupBy(x => x.Field, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last())
                .ToList();
        }

        private static HashSet<string> BuildWritableSaveDeltaParentFieldSet(IEnumerable<SaveDeltaFieldMeta> fields)
        {
            return new HashSet<string>(
                (fields ?? Enumerable.Empty<SaveDeltaFieldMeta>())
                    .Where(x => x != null
                        && x.IsWritable
                        && (string.Equals(x.TargetScope, "parent", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(x.TargetScope, "wip", StringComparison.OrdinalIgnoreCase))
                        && !string.IsNullOrWhiteSpace(x.FieldName))
                    .Select(x => x.FieldName.Trim()),
                StringComparer.OrdinalIgnoreCase);
        }

        private static List<SaveDeltaFieldMeta> GetSaveDeltaFieldMetadata()
        {
            var nowUtc = DateTime.UtcNow;
            lock (_saveDeltaFieldCacheLock)
            {
                if ((nowUtc - _saveDeltaFieldCacheAtUtc).TotalMinutes < 5
                    && _saveDeltaFieldCache != null
                    && _saveDeltaFieldCache.Count > 0)
                    return _saveDeltaFieldCache.Select(CloneSaveDeltaFieldMeta).ToList();
            }

            var output = new List<SaveDeltaFieldMeta>();
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "GetSaveDeltaFieldMeta"),
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var fieldName = row.Table.Columns.Contains("FieldName") && row["FieldName"] != DBNull.Value
                            ? row["FieldName"].ToString()
                            : null;
                        if (string.IsNullOrWhiteSpace(fieldName))
                            continue;

                        output.Add(new SaveDeltaFieldMeta
                        {
                            FieldName = fieldName.Trim(),
                            TargetScope = row.Table.Columns.Contains("TargetScope") && row["TargetScope"] != DBNull.Value
                                ? row["TargetScope"].ToString()
                                : "parent",
                            TargetColumn = row.Table.Columns.Contains("TargetColumn") && row["TargetColumn"] != DBNull.Value
                                ? row["TargetColumn"].ToString()
                                : fieldName.Trim(),
                            DataType = row.Table.Columns.Contains("DataType") && row["DataType"] != DBNull.Value
                                ? row["DataType"].ToString()
                                : "nvarchar",
                            RecalcGroup = row.Table.Columns.Contains("RecalcGroup") && row["RecalcGroup"] != DBNull.Value
                                ? row["RecalcGroup"].ToString()
                                : null,
                            ManualGroup = row.Table.Columns.Contains("ManualGroup") && row["ManualGroup"] != DBNull.Value
                                ? row["ManualGroup"].ToString()
                                : null,
                            IsWritable = !row.Table.Columns.Contains("IsWritable")
                                || row["IsWritable"] == DBNull.Value
                                || Convert.ToBoolean(row["IsWritable"]),
                            SortOrder = row.Table.Columns.Contains("SortOrder") && row["SortOrder"] != DBNull.Value
                                ? Convert.ToInt32(row["SortOrder"])
                                : 0
                        });
                    }
                }
            }
            catch
            {
                output.Clear();
            }

            if (output.Count == 0)
                output = BuildFallbackSaveDeltaFieldMetadata();

            lock (_saveDeltaFieldCacheLock)
            {
                _saveDeltaFieldCache = output.Select(CloneSaveDeltaFieldMeta).ToList();
                _saveDeltaFieldCacheAtUtc = nowUtc;
                return output.Select(CloneSaveDeltaFieldMeta).ToList();
            }
        }

        private static List<SaveDeltaFieldMeta> BuildFallbackSaveDeltaFieldMetadata()
        {
            return new List<SaveDeltaFieldMeta>
            {
                new SaveDeltaFieldMeta { FieldName = "GhiChuKho", TargetScope = "parent", TargetColumn = "GhiChuKho", DataType = "nvarchar", RecalcGroup = null, IsWritable = true, SortOrder = 10 },
                new SaveDeltaFieldMeta { FieldName = "GhiChu", TargetScope = "parent", TargetColumn = "GhiChu", DataType = "nvarchar", RecalcGroup = null, IsWritable = true, SortOrder = 11 },
                new SaveDeltaFieldMeta { FieldName = "Mer", TargetScope = "parent", TargetColumn = "Mer", DataType = "nvarchar", RecalcGroup = null, IsWritable = true, SortOrder = 12 },
                new SaveDeltaFieldMeta { FieldName = "InTheu", TargetScope = "parent", TargetColumn = "InTheu", DataType = "bit", RecalcGroup = "IN_THEU", ManualGroup = "PARENT", IsWritable = true, SortOrder = 20 },
                new SaveDeltaFieldMeta { FieldName = "DoKim", TargetScope = "parent", TargetColumn = "DoKim", DataType = "bit", RecalcGroup = null, ManualGroup = "PARENT", IsWritable = true, SortOrder = 21 },
                new SaveDeltaFieldMeta { FieldName = "HutAm", TargetScope = "parent", TargetColumn = "HutAm", DataType = "bit", RecalcGroup = null, ManualGroup = "PARENT", IsWritable = true, SortOrder = 22 },
                new SaveDeltaFieldMeta { FieldName = "InTheuTrong_PKH", TargetScope = "parent", TargetColumn = "InTheuTrong_PKH", DataType = "bit", RecalcGroup = "IN_THEU", ManualGroup = "PARENT", IsWritable = true, SortOrder = 23 },
                new SaveDeltaFieldMeta { FieldName = "SLCN", TargetScope = "parent", TargetColumn = "SLCN", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 30 },
                new SaveDeltaFieldMeta { FieldName = "TGLV", TargetScope = "parent", TargetColumn = "TGLV", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 31 },
                new SaveDeltaFieldMeta { FieldName = "OT", TargetScope = "parent", TargetColumn = "OT", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 32 },
                new SaveDeltaFieldMeta { FieldName = "HieuSuat", TargetScope = "parent", TargetColumn = "HieuSuat", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 33 },
                new SaveDeltaFieldMeta { FieldName = "SMV_WIP", TargetScope = "parent", TargetColumn = "SMV_WIP", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 34 },
                new SaveDeltaFieldMeta { FieldName = "CostPerM", TargetScope = "parent", TargetColumn = "CostPerM", DataType = "float", RecalcGroup = "KPI", ManualGroup = "PARENT", IsWritable = true, SortOrder = 35 },
                new SaveDeltaFieldMeta { FieldName = "Profit", TargetScope = "parent", TargetColumn = "Profit", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 36 },
                new SaveDeltaFieldMeta { FieldName = "CM", TargetScope = "parent", TargetColumn = "CM", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 37 },
                new SaveDeltaFieldMeta { FieldName = "HeSoCM", TargetScope = "parent", TargetColumn = "HeSoCM", DataType = "float", RecalcGroup = "KPI", IsWritable = true, SortOrder = 38 },
                new SaveDeltaFieldMeta { FieldName = "NSGio", TargetScope = "parent", TargetColumn = "NSGio", DataType = "float", RecalcGroup = "KPI", ManualGroup = "PARENT", IsWritable = true, SortOrder = 39 },
                new SaveDeltaFieldMeta { FieldName = "SoGioSX", TargetScope = "parent", TargetColumn = "SoGioSX", DataType = "float", RecalcGroup = "KPI", ManualGroup = "PARENT", IsWritable = true, SortOrder = 40 },
                new SaveDeltaFieldMeta { FieldName = "SoNgay", TargetScope = "parent", TargetColumn = "SoNgay", DataType = "float", RecalcGroup = "TIMELINE", ManualGroup = "PARENT", IsWritable = true, SortOrder = 41 },
                new SaveDeltaFieldMeta { FieldName = "NSCost", TargetScope = "parent", TargetColumn = "NSCost", DataType = "float", RecalcGroup = "KPI", ManualGroup = "PARENT", IsWritable = true, SortOrder = 42 },
                new SaveDeltaFieldMeta { FieldName = "NS_SMV", TargetScope = "parent", TargetColumn = "NS_SMV", DataType = "float", RecalcGroup = "KPI", ManualGroup = "PARENT", IsWritable = true, SortOrder = 43 },
                new SaveDeltaFieldMeta { FieldName = "DoanhThuCM", TargetScope = "parent", TargetColumn = "DoanhThuCM", DataType = "float", RecalcGroup = "KPI", ManualGroup = "PARENT", IsWritable = true, SortOrder = 44 },
                new SaveDeltaFieldMeta { FieldName = "KH_Top", TargetScope = "parent", TargetColumn = "KH_Top", DataType = "date", RecalcGroup = "TIMELINE", ManualGroup = "PARENT", IsWritable = true, SortOrder = 60 },
                new SaveDeltaFieldMeta { FieldName = "SLSizeMau_Top", TargetScope = "parent", TargetColumn = "SLSizeMau_Top", DataType = "float", RecalcGroup = null, IsWritable = true, SortOrder = 61 },
                new SaveDeltaFieldMeta { FieldName = "TT_MauTop", TargetScope = "parent", TargetColumn = "TT_MauTop", DataType = "date", RecalcGroup = null, IsWritable = true, SortOrder = 62 },
                new SaveDeltaFieldMeta { FieldName = "KH_Ship", TargetScope = "parent", TargetColumn = "KH_Ship", DataType = "date", RecalcGroup = null, IsWritable = true, SortOrder = 63 },
                new SaveDeltaFieldMeta { FieldName = "SLSizeMau_Ship", TargetScope = "parent", TargetColumn = "SLSizeMau_Ship", DataType = "float", RecalcGroup = null, IsWritable = true, SortOrder = 64 },
                new SaveDeltaFieldMeta { FieldName = "TT_Ship", TargetScope = "parent", TargetColumn = "TT_Ship", DataType = "date", RecalcGroup = null, IsWritable = true, SortOrder = 65 },
                new SaveDeltaFieldMeta { FieldName = "NgayNhanTT_BOM", TargetScope = "parent", TargetColumn = "NgayNhanTT_BOM", DataType = "date", RecalcGroup = null, ManualGroup = "PARENT", IsWritable = true, SortOrder = 66 },
                new SaveDeltaFieldMeta { FieldName = "DateTrenNhanCare", TargetScope = "parent", TargetColumn = "DateTrenNhanCare", DataType = "date", RecalcGroup = null, IsWritable = true, SortOrder = 67 },
                new SaveDeltaFieldMeta { FieldName = "KH_MauDauChuyen", TargetScope = "parent", TargetColumn = "KH_MauDauChuyen", DataType = "date", RecalcGroup = "TIMELINE", ManualGroup = "PARENT", IsWritable = true, SortOrder = 68 },
                new SaveDeltaFieldMeta { FieldName = "TT_MauDauChuyen", TargetScope = "parent", TargetColumn = "TT_MauDauChuyen", DataType = "date", RecalcGroup = null, IsWritable = true, SortOrder = 69 }
            };
        }

        private static SaveDeltaFieldMeta CloneSaveDeltaFieldMeta(SaveDeltaFieldMeta src)
        {
            if (src == null) return new SaveDeltaFieldMeta();
            return new SaveDeltaFieldMeta
            {
                FieldName = src.FieldName,
                TargetScope = src.TargetScope,
                TargetColumn = src.TargetColumn,
                DataType = src.DataType,
                RecalcGroup = src.RecalcGroup,
                ManualGroup = src.ManualGroup,
                IsWritable = src.IsWritable,
                SortOrder = src.SortOrder
            };
        }

        private static DeltaClassifyResult ClassifyDeltaSave(
            IEnumerable<NormalizedDeltaChange> changes,
            IDictionary<string, bool?> manualPatch,
            IEnumerable<TechStepMeta> techSteps,
            IDictionary<string, string> fieldAliasMap,
            ISet<string> writableParentFields)
        {
            var result = new DeltaClassifyResult();
            var steps = (techSteps ?? Enumerable.Empty<TechStepMeta>()).ToList();
            var parentFields = writableParentFields ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var c in changes ?? Enumerable.Empty<NormalizedDeltaChange>())
            {
                var field = (c.Field ?? "").Trim();
                if (string.IsNullOrWhiteSpace(field))
                    continue;

                var binding = ResolveDeltaStepBinding(field, steps);
                if (binding != null)
                {
                    result.StepChanges.Add(new DeltaStepChange
                    {
                        StepCode = binding.Step.StepCode,
                        Field = field,
                        Role = binding.Role,
                        Value = c.Value,
                        AllowManualKH = binding.Step.AllowManualKH,
                        IsCheckTT = binding.Step.IsCheckTT
                    });
                    continue;
                }

                if (parentFields.Contains(field))
                {
                    result.ParentChanges.Add(new DeltaParentChange
                    {
                        Field = field,
                        Value = c.Value
                    });
                    continue;
                }

                result.UnknownFields.Add(field);
            }

            foreach (var patch in manualPatch ?? new Dictionary<string, bool?>())
            {
                var field = ResolveFieldAlias(patch.Key, fieldAliasMap);
                if (string.IsNullOrWhiteSpace(field))
                    continue;

                var binding = ResolveDeltaStepBinding(field, steps);
                if (binding != null)
                {
                    result.ManualChanges.Add(new DeltaManualChange
                    {
                        Scope = "step",
                        StepCode = binding.Step.StepCode,
                        Field = field,
                        Target = binding.Role,
                        Value = patch.Value == true
                    });
                    continue;
                }

                if (parentFields.Contains(field))
                {
                    result.ManualChanges.Add(new DeltaManualChange
                    {
                        Scope = "parent",
                        StepCode = null,
                        Field = field,
                        Target = field,
                        Value = patch.Value == true
                    });
                    continue;
                }

                if (!result.UnknownFields.Contains(field, StringComparer.OrdinalIgnoreCase))
                    result.UnknownFields.Add(field);
            }

            result.UnknownFields = result.UnknownFields
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return result;
        }

        private static DeltaStepBinding ResolveDeltaStepBinding(string field, IEnumerable<TechStepMeta> techSteps)
        {
            if (string.IsNullOrWhiteSpace(field))
                return null;

            foreach (var step in techSteps ?? Enumerable.Empty<TechStepMeta>())
            {
                if (step == null)
                    continue;

                if (!string.IsNullOrWhiteSpace(step.LegacyKHColumn)
                    && string.Equals(step.LegacyKHColumn.Trim(), field, StringComparison.OrdinalIgnoreCase))
                    return new DeltaStepBinding { Step = step, Role = "KH" };

                if (!string.IsNullOrWhiteSpace(step.LegacyTTColumn)
                    && string.Equals(step.LegacyTTColumn.Trim(), field, StringComparison.OrdinalIgnoreCase))
                    return new DeltaStepBinding { Step = step, Role = "TT" };

                if (!string.IsNullOrWhiteSpace(step.NoDataField)
                    && step.AllowNoDataConfirm
                    && string.Equals(step.NoDataField.Trim(), field, StringComparison.OrdinalIgnoreCase))
                    return new DeltaStepBinding { Step = step, Role = "NoData" };
            }

            return null;
        }
        private static List<TechStepMeta> GetTechStepMetadata()
        {
            var nowUtc = DateTime.UtcNow;
            lock (_techStepCacheLock)
            {
                if ((nowUtc - _techStepCacheAtUtc).TotalMinutes < 5 && _techStepCache != null && _techStepCache.Count > 0)
                    return _techStepCache.Select(CloneTechStepMeta).ToList();
            }

            var output = new List<TechStepMeta>();
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "GetStepStatusMeta"),
                };

                var dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_DonHang_Chuyen", parameters);
                if (dt == null || dt.Rows.Count == 0)
                    return output;

                foreach (DataRow row in dt.Rows)
                {
                    var stepCode = row.Table.Columns.Contains("StepCode") && row["StepCode"] != DBNull.Value
                        ? row["StepCode"].ToString()
                        : null;
                    if (string.IsNullOrWhiteSpace(stepCode))
                        continue;

                    var khField = row.Table.Columns.Contains("KHField") && row["KHField"] != DBNull.Value
                        ? row["KHField"].ToString()
                        : (row.Table.Columns.Contains("LegacyKHColumn") && row["LegacyKHColumn"] != DBNull.Value
                            ? row["LegacyKHColumn"].ToString()
                            : null);

                    var ttField = row.Table.Columns.Contains("TTField") && row["TTField"] != DBNull.Value
                        ? row["TTField"].ToString()
                        : (row.Table.Columns.Contains("LegacyTTColumn") && row["LegacyTTColumn"] != DBNull.Value
                            ? row["LegacyTTColumn"].ToString()
                            : null);

                    var stepName = row.Table.Columns.Contains("StepName") && row["StepName"] != DBNull.Value
                        ? row["StepName"].ToString()
                        : null;

                    var sortOrder = row.Table.Columns.Contains("SortOrder") && row["SortOrder"] != DBNull.Value
                        ? Convert.ToInt32(row["SortOrder"])
                        : 0;

                    var isCheckTt = row.Table.Columns.Contains("IsCheckTT")
                        ? (row["IsCheckTT"] != DBNull.Value && Convert.ToBoolean(row["IsCheckTT"]))
                        : true;

                    var allowManualKh = row.Table.Columns.Contains("AllowManualKH")
                        ? (row["AllowManualKH"] != DBNull.Value && Convert.ToBoolean(row["AllowManualKH"]))
                        : true;
                    var allowNoDataConfirm = row.Table.Columns.Contains("AllowNoDataConfirm")
                        ? (row["AllowNoDataConfirm"] != DBNull.Value && Convert.ToBoolean(row["AllowNoDataConfirm"]))
                        : false;

                    var noDataField = row.Table.Columns.Contains("NoDataField") && row["NoDataField"] != DBNull.Value
                        ? row["NoDataField"].ToString()
                        : (!string.IsNullOrWhiteSpace(ttField) ? ttField + "_NoData" : null);

                    var noDataDateField = row.Table.Columns.Contains("NoDataDateField") && row["NoDataDateField"] != DBNull.Value
                        ? row["NoDataDateField"].ToString()
                        : (!string.IsNullOrWhiteSpace(ttField) ? ttField + "_NoDataDate" : null);
                    output.Add(new TechStepMeta
                    {
                        StepCode = stepCode,
                        StepName = stepName,
                        SortOrder = sortOrder,
                        LegacyKHColumn = khField,
                        LegacyTTColumn = ttField,
                        IsCheckTT = isCheckTt,
                        AllowManualKH = allowManualKh,
                        AllowNoDataConfirm = allowNoDataConfirm,
                        NoDataField = noDataField,
                        NoDataDateField = noDataDateField
                    });
                }
            }
            catch
            {
                output.Clear();
            }

            lock (_techStepCacheLock)
            {
                _techStepCache = output.Select(CloneTechStepMeta).ToList();
                _techStepCacheAtUtc = nowUtc;
                return output.Select(CloneTechStepMeta).ToList();
            }
        }

        private static TechStepMeta CloneTechStepMeta(TechStepMeta src)
        {
            if (src == null) return new TechStepMeta();
            return new TechStepMeta
            {
                StepCode = src.StepCode,
                StepName = src.StepName,
                SortOrder = src.SortOrder,
                LegacyKHColumn = src.LegacyKHColumn,
                LegacyTTColumn = src.LegacyTTColumn,
                AllowManualKH = src.AllowManualKH,
                IsCheckTT = src.IsCheckTT,
                AllowNoDataConfirm = src.AllowNoDataConfirm,
                NoDataField = src.NoDataField,
                NoDataDateField = src.NoDataDateField
            };
        }

        private static Dictionary<string, string> BuildFieldAliasMap(IEnumerable<TechStepMeta> techSteps)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var p in typeof(WIPDonHangChuyenViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                map[p.Name] = p.Name;
            }

            foreach (var s in techSteps ?? Enumerable.Empty<TechStepMeta>())
            {
                var stepCode = (s.StepCode ?? "").Trim();
                var legacyKh = (s.LegacyKHColumn ?? "").Trim();
                var legacyTt = (s.LegacyTTColumn ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(legacyKh))
                {
                    map[legacyKh] = legacyKh;

                    if (legacyKh.StartsWith("KH_", StringComparison.OrdinalIgnoreCase) && legacyKh.Length > 3)
                        map["KH" + legacyKh.Substring(3)] = legacyKh;
                    else if (legacyKh.StartsWith("KH", StringComparison.OrdinalIgnoreCase) && legacyKh.Length > 2)
                        map["KH_" + legacyKh.Substring(2)] = legacyKh;
                    else
                    {
                        map["KH" + legacyKh] = legacyKh;
                        map["KH_" + legacyKh] = legacyKh;
                    }
                }

                if (!string.IsNullOrWhiteSpace(legacyTt))
                    map[legacyTt] = legacyTt;

                if (!string.IsNullOrWhiteSpace(stepCode))
                {
                    if (!string.IsNullOrWhiteSpace(legacyTt))
                        map[stepCode] = legacyTt;

                    if (!string.IsNullOrWhiteSpace(legacyKh))
                    {
                        foreach (var khAlias in BuildKhAliasesFromStepCode(stepCode))
                        {
                            if (!string.IsNullOrWhiteSpace(khAlias))
                                map[khAlias] = legacyKh;
                        }
                    }
                }
            }

            return map;
        }

        private static IEnumerable<string> BuildKhAliasesFromStepCode(string stepCode)
        {
            if (string.IsNullOrWhiteSpace(stepCode))
                yield break;

            var step = stepCode.Trim();
            string suffix = null;

            if (step.StartsWith("TT_", StringComparison.OrdinalIgnoreCase) && step.Length > 3)
                suffix = step.Substring(3);
            else if (step.StartsWith("TT", StringComparison.OrdinalIgnoreCase) && step.Length > 2)
                suffix = step.Substring(2);

            if (string.IsNullOrWhiteSpace(suffix))
                yield break;

            var trimmed = suffix.TrimStart('_');
            yield return "KH" + trimmed;
            yield return "KH_" + trimmed;
        }

        private static string ResolveFieldAlias(string fieldName, IDictionary<string, string> fieldAliasMap)
        {
            var field = (fieldName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(field))
                return field;

            string mapped;
            if (fieldAliasMap != null && fieldAliasMap.TryGetValue(field, out mapped) && !string.IsNullOrWhiteSpace(mapped))
                return mapped;

            return field;
        }

        private static List<SaveWipChange> NormalizeChanges(IEnumerable<SaveWipChange> rawChanges, IDictionary<string, string> fieldAliasMap)
        {
            var list = new List<SaveWipChange>();
            foreach (var c in rawChanges ?? Enumerable.Empty<SaveWipChange>())
            {
                if (c == null || string.IsNullOrWhiteSpace(c.Field))
                    continue;

                var normalizedField = ResolveFieldAlias(c.Field, fieldAliasMap);
                if (string.IsNullOrWhiteSpace(normalizedField))
                    continue;

                list.Add(new SaveWipChange
                {
                    Field = normalizedField,
                    NewValue = UnwrapJsonValue(c.NewValue)
                });
            }

            return list
                .GroupBy(x => x.Field, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last())
                .ToList();
        }

        private static object UnwrapJsonValue(object value)
        {
            var token = value as JToken;
            if (token == null) return value;

            if (token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
                return null;

            var scalar = token as JValue;
            return scalar != null ? scalar.Value : token.ToString(Formatting.None);
        }

        private static void ApplyChangesToModel(WIPDonHangChuyenViewModel model, IEnumerable<SaveWipChange> normalizedChanges)
        {
            if (model == null) return;

            var writableProps = typeof(WIPDonHangChuyenViewModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite)
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            foreach (var c in normalizedChanges ?? Enumerable.Empty<SaveWipChange>())
            {
                if (c == null || string.IsNullOrWhiteSpace(c.Field))
                    continue;

                PropertyInfo prop;
                if (!writableProps.TryGetValue(c.Field.Trim(), out prop))
                    continue;

                object converted;
                if (!TryConvertToType(c.NewValue, prop.PropertyType, out converted))
                    continue;

                prop.SetValue(model, converted);
            }
        }

        private static bool TryConvertToType(object input, Type targetType, out object converted)
        {
            converted = null;
            var value = UnwrapJsonValue(input);
            var target = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (value == null)
            {
                if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                    return false;
                converted = null;
                return true;
            }

            try
            {
                if (target == typeof(string))
                {
                    var s = Convert.ToString(value);
                    converted = string.IsNullOrWhiteSpace(s) ? null : s.Trim();
                    return true;
                }

                if (target == typeof(DateTime))
                {
                    if (value is DateTime)
                    {
                        converted = (DateTime)value;
                        return true;
                    }

                    DateTime dt;
                    if (DateTime.TryParse(Convert.ToString(value), out dt))
                    {
                        converted = dt;
                        return true;
                    }
                    return false;
                }

                if (target == typeof(bool))
                {
                    if (value is bool)
                    {
                        converted = (bool)value;
                        return true;
                    }

                    var s = Convert.ToString(value);
                    if (string.Equals(s, "1", StringComparison.OrdinalIgnoreCase))
                    {
                        converted = true;
                        return true;
                    }
                    if (string.Equals(s, "0", StringComparison.OrdinalIgnoreCase))
                    {
                        converted = false;
                        return true;
                    }

                    bool b;
                    if (bool.TryParse(s, out b))
                    {
                        converted = b;
                        return true;
                    }
                    return false;
                }

                if (target == typeof(int))
                {
                    int i;
                    if (value is int)
                    {
                        converted = (int)value;
                        return true;
                    }
                    if (int.TryParse(Convert.ToString(value), out i))
                    {
                        converted = i;
                        return true;
                    }
                    return false;
                }

                if (target == typeof(double))
                {
                    if (value is double)
                    {
                        converted = (double)value;
                        return true;
                    }
                    var s = (Convert.ToString(value) ?? "").Replace(",", "");
                    double d;
                    if (double.TryParse(s, out d))
                    {
                        converted = d;
                        return true;
                    }
                    return false;
                }

                if (target == typeof(byte))
                {
                    byte b;
                    if (value is byte)
                    {
                        converted = (byte)value;
                        return true;
                    }
                    if (byte.TryParse(Convert.ToString(value), out b))
                    {
                        converted = b;
                        return true;
                    }
                    return false;
                }

                converted = Convert.ChangeType(value, target);
                return true;
            }
            catch
            {
                converted = null;
                return false;
            }
        }

        private static string NormalizeManualJsonPatchKeys(string rawManualJsonPatch, IDictionary<string, string> fieldAliasMap)
        {
            if (string.IsNullOrWhiteSpace(rawManualJsonPatch))
                return rawManualJsonPatch;

            try
            {
                var token = JToken.Parse(rawManualJsonPatch);
                var obj = token as JObject;
                if (obj == null) return rawManualJsonPatch;

                var normalized = new JObject();
                foreach (var p in obj.Properties())
                {
                    var normalizedKey = ResolveFieldAlias(p.Name, fieldAliasMap);
                    if (string.IsNullOrWhiteSpace(normalizedKey))
                        continue;

                    normalized[normalizedKey] = ParseBoolToken(p.Value) ? JToken.FromObject(true) : JToken.FromObject(false);
                }

                return normalized.ToString(Formatting.None);
            }
            catch
            {
                return rawManualJsonPatch;
            }
        }

        private static bool ParseBoolToken(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
                return false;

            if (token.Type == JTokenType.Boolean)
                return token.Value<bool>();

            if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                return token.Value<double>() != 0;

            var s = token.ToString();
            return string.Equals(s, "1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(s, "\"true\"", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class TechStepMeta
        {
            public string StepCode { get; set; }
            public string StepName { get; set; }
            public int SortOrder { get; set; }
            public string LegacyKHColumn { get; set; }
            public string LegacyTTColumn { get; set; }
            public bool AllowManualKH { get; set; }
            public bool IsCheckTT { get; set; }
            public bool AllowNoDataConfirm { get; set; }
            public string NoDataField { get; set; }
            public string NoDataDateField { get; set; }
        }

        private sealed class SaveDeltaFieldMeta
        {
            public string FieldName { get; set; }
            public string TargetScope { get; set; }
            public string TargetColumn { get; set; }
            public string DataType { get; set; }
            public string RecalcGroup { get; set; }
            public string ManualGroup { get; set; }
            public bool IsWritable { get; set; }
            public int SortOrder { get; set; }
        }

        private class NormalizedDeltaChange
        {
            public string Field { get; set; }
            public object Value { get; set; }
        }

        private class DeltaClassifyResult
        {
            public List<DeltaParentChange> ParentChanges { get; set; } = new List<DeltaParentChange>();
            public List<DeltaStepChange> StepChanges { get; set; } = new List<DeltaStepChange>();
            public List<DeltaManualChange> ManualChanges { get; set; } = new List<DeltaManualChange>();
            public List<string> UnknownFields { get; set; } = new List<string>();
        }

        private class DeltaParentChange
        {
            public string Field { get; set; }
            public object Value { get; set; }
        }

        private class DeltaStepChange
        {
            public string StepCode { get; set; }
            public string Field { get; set; }
            public string Role { get; set; }
            public object Value { get; set; }
            public bool AllowManualKH { get; set; }
            public bool IsCheckTT { get; set; }
        }

        private class DeltaManualChange
        {
            public string Scope { get; set; }
            public string StepCode { get; set; }
            public string Field { get; set; }
            public string Target { get; set; }
            public bool Value { get; set; }
        }

        private class DeltaStepBinding
        {
            public TechStepMeta Step { get; set; }
            public string Role { get; set; }
        }
        #endregion

        // =========================
        #region MapRow
        private WIPDonHangChuyenViewModel MapRow(DataRow row)
        {
            var stepStatusByTtField = ParseStepStatusMap(row, "StepStatusByTtFieldJson");
            var stepLateLockedByTtField = ParseStepLateLockedMap(row, "StepLateLockedByTtFieldJson");
            var StepMissingTTWarningByTtFieldJson = ParseStepLateLockedMap(row, "StepMissingTTWarningByTtFieldJson");
            var stepManualByField = ParseStepLateLockedMap(row, "StepManualByFieldJson");
            var model = new WIPDonHangChuyenViewModel
            {
                WIPId = row["WIPId"] == DBNull.Value ? 0 : Convert.ToInt32(row["WIPId"]),
                ThuTuChuyen = row["ThuTuChuyen"] == DBNull.Value ? 0 : Convert.ToInt32(row["ThuTuChuyen"]),
                LineX = row["LineX"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["LineX"]),
                IsGiaCong = row.Table.Columns.Contains("IsGiaCong") && row["IsGiaCong"] != DBNull.Value
                ? Convert.ToBoolean(row["IsGiaCong"])
                : false,
                LineName = row.Table.Columns.Contains("LineName") && row["LineName"] != DBNull.Value
                ? row["LineName"].ToString()
                : null,
                LenhSX = row.Table.Columns.Contains("LenhSX") && row["LenhSX"] != DBNull.Value
                ? (int?)Convert.ToInt32(row["LenhSX"])
                : null,
                MaLenhSanXuat = row["MaLenhSanXuat"]?.ToString(),
                StyleId = row["StyleId"]?.ToString(),
                MaKH = row.Table.Columns.Contains("MaKH") && row["MaKH"] != DBNull.Value
                ? row["MaKH"].ToString()
                : null,
                MaDH = row.Table.Columns.Contains("MaDH") && row["MaDH"] != DBNull.Value
                ? row["MaDH"].ToString()
                : null,

                Season = row.Table.Columns.Contains("Season") && row["Season"] != DBNull.Value
                ? row["Season"].ToString()
                : null,

                UpdatedAt = row.Table.Columns.Contains("UpdatedAt") && row["UpdatedAt"] != DBNull.Value
                ? (DateTime?)Convert.ToDateTime(row["UpdatedAt"])
                : null,
                MaHang = row["MaHang"]?.ToString(),
                KhachHang = row["KhachHang"]?.ToString(),

                PO = row["PO"]?.ToString(),
                SLKH = row["SLKH"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SLKH"]),
                InTheu = row["InTheu"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["InTheu"]),
                DoKim = row.Table.Columns.Contains("DoKim") && row["DoKim"] != DBNull.Value
                    ? (bool?)Convert.ToBoolean(row["DoKim"])
                    : null,
                HutAm = row["HutAm"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["HutAm"]),
                // ===== In/Thêu mở rộng =====
                InTheuTrong_PKH = row["InTheuTrong_PKH"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["InTheuTrong_PKH"]),

                KH_GuiInTheu = row["KH_GuiInTheu"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_GuiInTheu"]),
                TT_GuiInTheu = row["TT_GuiInTheu"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_GuiInTheu"]),
                KH_NhanInTheu = row["KH_NhanInTheu"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_NhanInTheu"]),
                TT_NhanInTheu = row["TT_NhanInTheu"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_NhanInTheu"]),
                NSGio = row["NSGio"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["NSGio"]),
                SoGioSX = row["SoGioSX"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SoGioSX"]),
                SoNgay = row["SoNgay"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SoNgay"]),

                SLCN = row["SLCN"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SLCN"]),
                TGLV = row["TGLV"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["TGLV"]),
                OT = row["OT"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["OT"]),
                AllocatedRegularHours = row.Table.Columns.Contains("AllocatedRegularHours") && row["AllocatedRegularHours"] != DBNull.Value
                    ? (double?)Convert.ToDouble(row["AllocatedRegularHours"])
                    : null,
                AllocatedOTHours = row.Table.Columns.Contains("AllocatedOTHours") && row["AllocatedOTHours"] != DBNull.Value
                    ? (double?)Convert.ToDouble(row["AllocatedOTHours"])
                    : null,
                HieuSuat = row["HieuSuat"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["HieuSuat"]),

                KHCat = row["KHCat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KHCat"]),
                TTCat = row["TTCat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TTCat"]),
                KHLapTrinh = row["KHLapTrinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KHLapTrinh"]),
                TTLapTrinh = row["TTLapTrinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TTLapTrinh"]),
                KHMay = row["KHMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KHMay"]),
                TTMay = row["TTMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TTMay"]),
                ThoatChuyen = row["ThoatChuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ThoatChuyen"]),
                TTThoatChuyen = row["TTThoatChuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TTThoatChuyen"]),

                TT_Cat = row["TT_Cat"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["TT_Cat"]),
                RaChuyen = row["RaChuyen"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["RaChuyen"]),
                BTP_DK = row["BTP_DK"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["BTP_DK"]),
                TP_KiemDat = row["TP_KiemDat"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["TP_KiemDat"]),
                TP_Nhan = row["TP_Nhan"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["TP_Nhan"]),
                Packing = row["Packing"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["Packing"]),

                KH_Top = row["KH_Top"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Top"]),
                SLSizeMau_Top = row["SLSizeMau_Top"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SLSizeMau_Top"]),
                TT_MauTop = row["TT_MauTop"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_MauTop"]),

                KH_Ship = row["KH_Ship"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Ship"]),
                SLSizeMau_Ship = row["SLSizeMau_Ship"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SLSizeMau_Ship"]),
                TT_Ship = row["TT_Ship"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_Ship"]),

                KH_Vai = row["KH_Vai"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Vai"]),
                TT_Vai = row["TT_Vai"] == DBNull.Value ? null : row["TT_Vai"].ToString(),

                KH_PLInEp = row["KH_PLInEp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_PLInEp"]),
                TT_PLInEp = row["TT_PLInEp"] == DBNull.Value ? null : row["TT_PLInEp"].ToString(),

                KH_PLMay = row["KH_PLMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_PLMay"]),
                TT_PLMay = row["TT_PLMay"] == DBNull.Value ? null : row["TT_PLMay"].ToString(),

                KH_PackingList = row["KH_PackingList"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_PackingList"]),
                TT_PackingList = row["TT_PackingList"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_PackingList"]),

                KH_PLDongGoi = row["KH_PLDongGoi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_PLDongGoi"]),
                TT_PLDongGoi = row["TT_PLDongGoi"] == DBNull.Value ? null : row["TT_PLDongGoi"].ToString(),

                KH_LenhSX = row["KH_LenhSX"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_LenhSX"]),
                TT_LenhSX = row["TT_LenhSX"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_LenhSX"]),

                SMV_WIP = row["SMV_WIP"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["SMV_WIP"]),
                NSCost = row["NSCost"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["NSCost"]),
                NS_SMV = row["NS_SMV"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["NS_SMV"]),

                CostPerM = row["CostPerM"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["CostPerM"]),
                CM = row["CM"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["CM"]),
                HeSoCM = row["HeSoCM"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["HeSoCM"]),
                DoanhThuCM = row["DoanhThuCM"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["DoanhThuCM"]),
                Profit = row["Profit"] == DBNull.Value ? (double?)null : Convert.ToDouble(row["Profit"]),
                // ===== Phòng kỹ thuật =====
                NgayNhanTT_BOM = row["NgayNhanTT_BOM"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["NgayNhanTT_BOM"]),
                KH_BOM = row["KH_BOM"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_BOM"]),
                TT_BOM = row["TT_BOM"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_BOM"]),
                TT_BOM_IsLateLocked = row.Table.Columns.Contains("TT_BOM_IsLateLocked") && row["TT_BOM_IsLateLocked"] != DBNull.Value
                    ? (bool?)Convert.ToBoolean(row["TT_BOM_IsLateLocked"])
                    : null,
                TT_BOM_LateLockedAt = row.Table.Columns.Contains("TT_BOM_LateLockedAt") && row["TT_BOM_LateLockedAt"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["TT_BOM_LateLockedAt"])
                    : null,
                TT_BOM_Status = (byte?)ResolveTtBomStatus(row),

                KH_NPL_BM_KTX_MayMau = row["KH_NPL_BM_KTX_MayMau"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_NPL_BM_KTX_MayMau"]),
                TT_NPL_BM_KTX_MayMau = row["TT_NPL_BM_KTX_MayMau"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_NPL_BM_KTX_MayMau"]),
                TT_NPL_BM_KTX_MayMau_NoData = row.Table.Columns.Contains("TT_NPL_BM_KTX_MayMau_NoData") && row["TT_NPL_BM_KTX_MayMau_NoData"] != DBNull.Value
                    ? (bool?)Convert.ToBoolean(row["TT_NPL_BM_KTX_MayMau_NoData"])
                    : null,

                KH_TestKeoLogo_TapeLaser = row["KH_TestKeoLogo_TapeLaser"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_TestKeoLogo_TapeLaser"]),
                TT_TestKeoLogo_TapeLaser = row["TT_TestKeoLogo_TapeLaser"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_TestKeoLogo_TapeLaser"]),

                KH_Rap = row["KH_Rap"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Rap"]),
                TT_Rap = row["TT_Rap"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_Rap"]),

                KH_TacNghiepCat = row["KH_TacNghiepCat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_TacNghiepCat"]),
                TT_TacNghiepCat = row["TT_TacNghiepCat"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_TacNghiepCat"]),

                KH_SoDo = row["KH_SoDo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_SoDo"]),
                TT_SoDo = row["TT_SoDo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_SoDo"]),

                KH_BangMauVai_InEp = row["KH_BangMauVai_InEp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_BangMauVai_InEp"]),
                TT_BangMauVai_InEp = row["TT_BangMauVai_InEp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_BangMauVai_InEp"]),

                KH_BangMauPL_Full = row["KH_BangMauPL_Full"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_BangMauPL_Full"]),
                TT_BangMauPL_Full = row["TT_BangMauPL_Full"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_BangMauPL_Full"]),

                KH_BangDanhSo = row["KH_BangDanhSo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_BangDanhSo"]),
                TT_BangDanhSo = row["TT_BangDanhSo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_BangDanhSo"]),

                KH_TLieuInEp = row["KH_TLieuInEp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_TLieuInEp"]),
                TT_TLieuInEp = row["TT_TLieuInEp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_TLieuInEp"]),

                KH_TLieuLT = row["KH_TLieuLT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_TLieuLT"]),
                TT_TLieuLT = row["TT_TLieuLT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_TLieuLT"]),

                KH_TSCatPL = row["KH_TSCatPL"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_TSCatPL"]),
                TT_TSCatPL = row["TT_TSCatPL"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_TSCatPL"]),

                KH_CoiNut = row["KH_CoiNut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_CoiNut"]),
                TT_CoiNut = row["TT_CoiNut"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_CoiNut"]),

                KH_Layout = row["KH_Layout"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Layout"]),
                TT_Layout = row["TT_Layout"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_Layout"]),

                KH_NhanCare = row["KH_NhanCare"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_NhanCare"]),
                TT_NhanCare = row["TT_NhanCare"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_NhanCare"]),
                DateTrenNhanCare = row["DateTrenNhanCare"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DateTrenNhanCare"]),

                KH_CbiSXChoCoDien = row["KH_CbiSXChoCoDien"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_CbiSXChoCoDien"]),
                TT_CbiSXChoCoDien = row["TT_CbiSXChoCoDien"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_CbiSXChoCoDien"]),

                KH_TaiLieuHoanChinh = row["KH_TaiLieuHoanChinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_TaiLieuHoanChinh"]),
                TT_TaiLieuHoanChinh = row["TT_TaiLieuHoanChinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_TaiLieuHoanChinh"]),

                KH_KichThuoc = row["KH_KichThuoc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_KichThuoc"]),
                TT_KichThuoc = row["TT_KichThuoc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_KichThuoc"]),

                KH_QuyCachDongGoi = row["KH_QuyCachDongGoi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_QuyCachDongGoi"]),
                TT_QuyCachDongGoi = row["TT_QuyCachDongGoi"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_QuyCachDongGoi"]),

                KH_HopTKSX = row["KH_HopTKSX"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_HopTKSX"]),
                TT_HopTKSX = row["TT_HopTKSX"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_HopTKSX"]),

                KH_NhanBaoThung = row["KH_NhanBaoThung"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_NhanBaoThung"]),
                TT_NhanBaoThung = row["TT_NhanBaoThung"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_NhanBaoThung"]),

                KH_MauDauChuyen = row["KH_MauDauChuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_MauDauChuyen"]),
                TT_MauDauChuyen = row["TT_MauDauChuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_MauDauChuyen"]),

                KH_DuyetMauDauChuyen = row["KH_DuyetMauDauChuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_DuyetMauDauChuyen"]),
                TT_DuyetMauDauChuyen = row["TT_DuyetMauDauChuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_DuyetMauDauChuyen"]),
                // ===== IE =====
                KH_NhuCauMayMoc = row["KH_NhuCauMayMoc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_NhuCauMayMoc"]),
                TT_NhuCauMayMoc = row["TT_NhuCauMayMoc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_NhuCauMayMoc"]),

                KH_Layout_IE = row["KH_Layout_IE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Layout_IE"]),
                TT_Layout_IE = row["TT_Layout_IE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_Layout_IE"]),

                KH_QTMay = row["KH_QTMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_QTMay"]),
                TT_QTMay = row["TT_QTMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_QTMay"]),

                KH_DonGiaCongNhan = row["KH_DonGiaCongNhan"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_DonGiaCongNhan"]),
                TT_DonGiaCongNhan = row["TT_DonGiaCongNhan"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_DonGiaCongNhan"]),

                // ===== Cơ điện =====
                KH_ChuanBiMayMoc_KTX = row["KH_ChuanBiMayMoc_KTX"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_ChuanBiMayMoc_KTX"]),
                TT_ChuanBiMayMoc_KTX = row["TT_ChuanBiMayMoc_KTX"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_ChuanBiMayMoc_KTX"]),

                KH_ChuanBiMayMoc_Chuyen = row["KH_ChuanBiMayMoc_Chuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_ChuanBiMayMoc_Chuyen"]),
                TT_ChuanBiMayMoc_Chuyen = row["TT_ChuanBiMayMoc_Chuyen"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_ChuanBiMayMoc_Chuyen"]),

                // ===== Kho =====
                KH_KhoVai = row["KH_KhoVai"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_KhoVai"]),
                TT_KhoVai = row["TT_KhoVai"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_KhoVai"]),

                KH_KhoPhuLieuMay = row["KH_KhoPhuLieuMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_KhoPhuLieuMay"]),
                TT_KhoPhuLieuMay = row["TT_KhoPhuLieuMay"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_KhoPhuLieuMay"]),

                KH_Thung = row["KH_Thung"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["KH_Thung"]),
                TT_Thung = row["TT_Thung"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["TT_Thung"]),
                GhiChuKho = row["GhiChuKho"]?.ToString(),
                GhiChu = row["GhiChu"]?.ToString(),
                TenCL = row["TenCL"]?.ToString(),
                ManualJson = row["ManualJson"] == DBNull.Value ? null : row["ManualJson"].ToString(),
                Mer = row["Mer"] == DBNull.Value ? null : row["Mer"].ToString(),
                MaDVSX = row.Table.Columns.Contains("MaDVSX") && row["MaDVSX"] != DBNull.Value
                ? row["MaDVSX"].ToString() : null,
                TenDVSX = row.Table.Columns.Contains("TenDVSX") && row["TenDVSX"] != DBNull.Value
                ? row["TenDVSX"].ToString() : null,
                NameMer = row["NameMer"] == DBNull.Value ? null : row["NameMer"].ToString(),
                CapStatus = row.Table.Columns.Contains("CapStatus") && row["CapStatus"] != DBNull.Value
                    ? (int?)Convert.ToInt32(row["CapStatus"])
                    : null,
                CapMissingHours = row.Table.Columns.Contains("CapMissingHours") && row["CapMissingHours"] != DBNull.Value
                    ? (double?)Convert.ToDouble(row["CapMissingHours"])
                    : null,
                CapFirstMissingDate = row.Table.Columns.Contains("CapFirstMissingDate") && row["CapFirstMissingDate"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["CapFirstMissingDate"])
                    : null,
                CapCheckedAt = row.Table.Columns.Contains("CapCheckedAt") && row["CapCheckedAt"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["CapCheckedAt"])
                    : null,
                CapNote = row.Table.Columns.Contains("CapNote") && row["CapNote"] != DBNull.Value
                    ? row["CapNote"].ToString()
                    : null,
                StepStatusByTtField = stepStatusByTtField,
                StepLateLockedByTtField = stepLateLockedByTtField,
                StepMissingTTWarningByTtField = StepMissingTTWarningByTtFieldJson,
                StepManualByField = stepManualByField,
                CreatedAt = row.Table.Columns.Contains("CreatedAt") && row["CreatedAt"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["CreatedAt"])
                    : null
            };

            int? bomStatusFromMap;
            if (TryGetStatusFromMap(stepStatusByTtField, "TT_BOM", out bomStatusFromMap) && bomStatusFromMap.HasValue)
            {
                model.TT_BOM_Status = (byte)bomStatusFromMap.Value;
            }

            bool? bomLateLockedFromMap;
            if (TryGetBoolFromMap(stepLateLockedByTtField, "TT_BOM", out bomLateLockedFromMap))
            {
                model.TT_BOM_IsLateLocked = bomLateLockedFromMap;
                if (bomLateLockedFromMap == true)
                    model.TT_BOM_Status = 3;
            }

            return model;
        }

        private static Dictionary<string, int?> ParseStepStatusMap(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
                return null;

            var map = new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase);
            if (row[columnName] == DBNull.Value)
                return map;

            var raw = Convert.ToString(row[columnName] ?? string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
                return map;

            try
            {
                var obj = JObject.Parse(raw);
                foreach (var prop in obj.Properties())
                {
                    if (string.IsNullOrWhiteSpace(prop.Name))
                        continue;

                    int val;
                    if (int.TryParse(Convert.ToString(prop.Value), out val))
                    {
                        if (val < 0) val = 0;
                        if (val > 3) val = 3;
                        map[prop.Name.Trim()] = val;
                    }
                    else
                    {
                        map[prop.Name.Trim()] = null;
                    }
                }
            }
            catch
            {
                // Keep empty map on malformed JSON for backward compatibility.
            }

            return map;
        }

        private static Dictionary<string, bool?> ParseStepLateLockedMap(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
                return null;

            var map = new Dictionary<string, bool?>(StringComparer.OrdinalIgnoreCase);
            if (row[columnName] == DBNull.Value)
                return map;

            var raw = Convert.ToString(row[columnName] ?? string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
                return map;

            try
            {
                var obj = JObject.Parse(raw);
                foreach (var prop in obj.Properties())
                {
                    if (string.IsNullOrWhiteSpace(prop.Name))
                        continue;

                    bool parsed;
                    var token = prop.Value;
                    if (token != null && token.Type == JTokenType.Boolean)
                    {
                        map[prop.Name.Trim()] = token.Value<bool>();
                    }
                    else if (bool.TryParse(Convert.ToString(token), out parsed))
                    {
                        map[prop.Name.Trim()] = parsed;
                    }
                    else
                    {
                        var number = Convert.ToString(token);
                        map[prop.Name.Trim()] = string.Equals(number, "1", StringComparison.OrdinalIgnoreCase)
                            ? true
                            : (string.Equals(number, "0", StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);
                    }
                }
            }
            catch
            {
                // Keep empty map on malformed JSON for backward compatibility.
            }

            return map;
        }

        private static bool TryGetStatusFromMap(Dictionary<string, int?> map, string key, out int? value)
        {
            value = null;
            if (map == null || string.IsNullOrWhiteSpace(key))
                return false;

            int? raw;
            if (!map.TryGetValue(key, out raw))
                return false;

            if (raw.HasValue)
            {
                var val = raw.Value;
                if (val < 0) val = 0;
                if (val > 3) val = 3;
                value = val;
            }
            else
            {
                value = null;
            }

            return true;
        }

        private static bool TryGetBoolFromMap(Dictionary<string, bool?> map, string key, out bool? value)
        {
            value = null;
            if (map == null || string.IsNullOrWhiteSpace(key))
                return false;

            bool? raw;
            if (!map.TryGetValue(key, out raw))
                return false;

            value = raw;
            return true;
        }

        private static byte ResolveTtBomStatus(DataRow row)
        {
            // 1) Ưu tiên lock đỏ
            bool isLateLocked = row.Table.Columns.Contains("TT_BOM_IsLateLocked")
                && row["TT_BOM_IsLateLocked"] != DBNull.Value
                && Convert.ToBoolean(row["TT_BOM_IsLateLocked"]);
            if (isLateLocked) return 3;

            // 2) Nếu SQL đã trả TT_BOM_Status thì ưu tiên dùng thẳng
            if (row.Table.Columns.Contains("TT_BOM_Status") && row["TT_BOM_Status"] != DBNull.Value)
            {
                var statusFromSql = Convert.ToInt32(row["TT_BOM_Status"]);
                if (statusFromSql < 0 || statusFromSql > 3) return 0;
                return (byte)statusFromSql;
            }

            // 3) Fallback tính theo rule chuẩn
            DateTime? ttBom = row.Table.Columns.Contains("TT_BOM") && row["TT_BOM"] != DBNull.Value
                ? (DateTime?)Convert.ToDateTime(row["TT_BOM"])
                : null;
            DateTime? khBom = row.Table.Columns.Contains("KH_BOM") && row["KH_BOM"] != DBNull.Value
                ? (DateTime?)Convert.ToDateTime(row["KH_BOM"])
                : null;

            if (!ttBom.HasValue || !khBom.HasValue) return 0;
            if (ttBom.Value < khBom.Value) return 1;
            if (ttBom.Value > khBom.Value) return 2;
            return 0;
        }
        #endregion

        #region MAP AUDIT
        private static WIPAuditViewModel MapAuditRow(DataRow row)
        {
            return new WIPAuditViewModel
            {
                AuditId = row.Table.Columns.Contains("AuditId") && row["AuditId"] != DBNull.Value
                    ? Convert.ToInt32(row["AuditId"])
                    : 0,
                WipId = row.Table.Columns.Contains("WipId") && row["WipId"] != DBNull.Value
                    ? Convert.ToInt32(row["WipId"])
                    : 0,
                FieldName = row.Table.Columns.Contains("FieldName") && row["FieldName"] != DBNull.Value
                    ? Convert.ToString(row["FieldName"])
                    : null,
                OldValue = row.Table.Columns.Contains("OldValue") && row["OldValue"] != DBNull.Value
                    ? Convert.ToString(row["OldValue"])
                    : null,
                NewValue = row.Table.Columns.Contains("NewValue") && row["NewValue"] != DBNull.Value
                    ? Convert.ToString(row["NewValue"])
                    : null,
                ModifiedBy = row.Table.Columns.Contains("ModifiedBy") && row["ModifiedBy"] != DBNull.Value
                    ? Convert.ToString(row["ModifiedBy"])
                    : null,
                ModifiedAt = row.Table.Columns.Contains("ModifiedAt") && row["ModifiedAt"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["ModifiedAt"])
                    : null,
                ModifiedByName = row.Table.Columns.Contains("ModifiedByName") && row["ModifiedByName"] != DBNull.Value
                    ? Convert.ToString(row["ModifiedByName"])
                    : null,
            };
        }
        #endregion
        // =========================
        // GET: api/wip-donhang/get-md
        // =========================
        #region Get Mer
        [Route("get-md")]
        public IHttpActionResult GetMer()
        {
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Action", "Get"),
            };

            DataTable dt = WipDonHangModel.ExecStoredProcedure("Sp_Wip_Get_MD", parameters);

            var list = new List<WipGetMer>();
            foreach (DataRow row in dt.Rows)
                list.Add(new WipGetMer
                {
                    UserID = row["UserID"] == DBNull.Value ? null : row["UserID"].ToString(),
                    Ten = row["Ten"] == DBNull.Value ? null : row["Ten"].ToString(),
                });
            return Ok(list);
        }
        #endregion
    }
    public class WipGetMer
    {
        public string UserID { get; set; }
        public string Ten { get; set; }
    }
    public class WipSyncRequest
    {
        public string MaDH { get; set; }
    }
    public class LineMapListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class WIPKeyDto
    {
        public string StyleId { get; set; }
        public string MaKH { get; set; }
        public string Season { get; set; }
        public string MaDH { get; set; }
    }
    public class WipColorDto
    {
        public string CssClass { get; set; }
        public string DisplayName { get; set; }
        public string ColorCode { get; set; }
    }

    public class UpdateHeaderColorRequest
    {
        public string CssClass { get; set; }
        public string ColorCode { get; set; }
        public string UserID { get; set; }
    }

    public class WipColorUpdateResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class StepStatusMetaResponseDto
    {
        public List<int> StatusCodes { get; set; } = new List<int>();
        public List<string> CompareTtColumns { get; set; } = new List<string>();
        public List<StepStatusMetaItemDto> Steps { get; set; } = new List<StepStatusMetaItemDto>();
    }

    public class StepStatusMetaItemDto
    {
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public int SortOrder { get; set; }
        public string KHField { get; set; }
        public string TTField { get; set; }
        public bool IsCheckTT { get; set; }
        public bool AllowManualKH { get; set; }
        public string StatusField { get; set; }
        public string LateLockedField { get; set; }
        public bool AllowNoDataConfirm { get; set; }
        public string NoDataField { get; set; }
        public string NoDataDateField { get; set; }

    }

    public class ColumnDynamicDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
        public string ParentCode { get; set; }
        public int Module { get; set; }
        public string BandCode { get; set; }
        public string Caption { get; set; }
        public string DataField { get; set; }
        public string StepCode { get; set; }
        public string DataType { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public int STT { get; set; }
        public bool IsCheckTT { get; set; }
        public bool AllowManualKH { get; set; }
    }

    public class StepCellStatusDto
    {
        public int WIPId { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public string KHField { get; set; }
        public string TTField { get; set; }
        public string DataField { get; set; }
        public int? Status { get; set; }
        public bool? IsLateLocked { get; set; }
        public bool? IsMissingTTWarning { get; set; }
    }

    public class AssignToLineItem
    {
        public int WipId { get; set; }
        public double? SLKH { get; set; }
    }

    public class AssignToLineRequest
    {
        public int? LineX { get; set; }
        public bool? IsGiaCong { get; set; }
        public int? LenhSX { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string mer { get; set; }
        public string MaDVSX { get; set; }
        public List<AssignToLineItem> Items { get; set; } = new List<AssignToLineItem>();
    }
    public class WipChangeLineRequest
    {
        public string MaGop { get; set; }
        public string MaLenhSanXuat { get; set; }
        public int OldLine { get; set; }
        public int NewLine { get; set; }
        public int? FromThuTu { get; set; }
    }
    public class WIPRecalcLineRequest
    {
        public int LineX { get; set; }
        public int? FromThuTu { get; set; }
        public string ModifyBy { get; set; }
        public bool IsKetThuc { get; set; } = false;
    }
    public class WIPUnassignFromLineRequest
    {
        public List<WIPKeyDto> Keys { get; set; } = new List<WIPKeyDto>();
    }
    public class UpdateMerRequest
    {
        public string MaLenhSanXuat { get; set; }
        public string MaDH { get; set; }
        public int NewLine { get; set; }
        public string Mer { get; set; }
    }
    public class SyncLibRequest
    {
        public int LineX { get; set; }
    }
}
