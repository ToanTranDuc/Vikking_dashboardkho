using NtbSoft.ERP.Libs;
using NtbSoft.ERP.Model.WipDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.WipDonHang
{
    [RoutePrefix("api/wip-donhang-gantt")]
    public class WIPDonHangGanttApiController : ApiController
    {
        private const int DefaultPageSize = 50;
        private const int MaxPageSize = 500;
        private const string ProcedureName = "SP_WIP_DonHang_Gantt";
        private static readonly object ParameterCacheLock = new object();
        private static HashSet<string> _procedureParameters;

        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get(
            string keyword = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? lineX = null,
            int? lenhSX = null,
            string maDH = null,
            string maKH = null,
            string season = null,
            string styleId = null,
            string maHang = null,
            string khachHang = null,
            string po = null,
            bool? isGiaCong = null,
            int isKetThuc = 0,
            int pageIndex = 1,
            int pageSize = DefaultPageSize,
            bool includeEmptySteps = false)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                return BadRequest("'fromDate' không được lớn hơn 'toDate'.");
            }

            pageIndex = Math.Max(pageIndex, 1);
            pageSize = Math.Max(1, Math.Min(pageSize, MaxPageSize));
            var skipRows = (pageIndex - 1) * pageSize;

            var masterParameters = new[]
            {
                Param("@Action", "GetMaster"),
                Param("@WIPId", null),
                Param("@LineX", lineX),
                Param("@LenhSX", lenhSX),
                Param("@MaLenhSanXuat", null),
                Param("@MaDH", maDH),
                Param("@MaKH", maKH),
                Param("@Season", season),
                Param("@StyleId", styleId),
                Param("@MaHang", maHang),
                Param("@KhachHang", khachHang),
                Param("@PO", po),
                Param("@Keyword", keyword),
                Param("@IsGiaCong", isGiaCong),
                Param("@IsKetThuc", isKetThuc),
                Param("@FromDate", fromDate),
                Param("@ToDate", toDate),
                Param("@PageIndex", pageIndex),
                Param("@PageSize", pageSize),
                Param("@SkipRows", skipRows),
                Param("@TakeRows", pageSize),
                Param("@IncludeEmptySteps", includeEmptySteps)
            };

            var masterTable = WipDonHangModel.ExecStoredProcedure(ProcedureName, FilterSupportedParameters(masterParameters));
            var masters = MapMasters(masterTable);
            var details = new List<WipGanttDetailDto>();
            return Ok(new
            {
                ok = true,
                pageIndex,
                pageSize,
                totalRows = masters.Count > 0 ? masters[0].TotalRows : 0,
                masters,
            });
        }

        [HttpGet]
        [Route("detail")]
        public IHttpActionResult Detail(int wipId, bool includeEmptySteps = true)
        {
            if (wipId <= 0)
            {
                return BadRequest("wipId must be > 0.");
            }

            return Ok(new
            {
                ok = true,
                wipId,
                details = LoadDetails(wipId, includeEmptySteps)
            });
        }

        private static List<WipGanttMasterDto> MapMasters(DataTable table)
        {
            var list = new List<WipGanttMasterDto>();
            if (table == null) return list;

            foreach (DataRow row in table.Rows)
            {
                var wipId = GetInt(row, "WIPId");
                if (!wipId.HasValue) continue;

                list.Add(new WipGanttMasterDto
                {
                    WIPId = wipId.Value,
                    ThuTuChuyen = GetInt(row, "ThuTuChuyen"),
                    LineX = GetInt(row, "LineX"),
                    LineName = GetString(row, "LineName"),
                    RowNo = GetInt(row, "RowNo"),
                    IsGiaCong = GetBool(row, "IsGiaCong"),
                    LenhSX = GetInt(row, "LenhSX"),
                    MaDH = GetString(row, "MaDH"),
                    MaKH = GetString(row, "MaKH"),
                    Season = GetString(row, "Season"),
                    StyleId = GetString(row, "StyleId"),
                    MaHang = GetString(row, "MaHang"),
                    KhachHang = GetString(row, "KhachHang"),
                    PO = GetString(row, "PO"),
                    SLKH = GetDouble(row, "SLKH"),
                    NameMer = GetString(row, "NameMer"),
                    TimelineStart = GetDate(row, "TimelineStart"),
                    TimelineEnd = GetDate(row, "TimelineEnd"),
                    TimelineStartStepCode = GetString(row, "TimelineStartStepCode"),
                    TimelineEndStepCode = GetString(row, "TimelineEndStepCode"),
                    DetailCount = GetInt(row, "DetailCount"),
                    HasDetail = GetBool(row, "HasDetail"),
                    TotalRows = GetInt(row, "TotalRows") ?? 0
                });
            }

            return list;
        }

        private static List<WipGanttDetailDto> LoadDetails(int wipId, bool includeEmptySteps)
        {
            var parameters = new[]
            {
                Param("@Action", "GetDetail"),
                Param("@WIPId", wipId),
                Param("@LineX", null),
                Param("@LenhSX", null),
                Param("@MaLenhSanXuat", null),
                Param("@MaDH", null),
                Param("@MaKH", null),
                Param("@Season", null),
                Param("@StyleId", null),
                Param("@MaHang", null),
                Param("@KhachHang", null),
                Param("@PO", null),
                Param("@Keyword", null),
                Param("@IsGiaCong", null),
                Param("@IsKetThuc", null),
                Param("@FromDate", null),
                Param("@ToDate", null),
                Param("@PageIndex", 1),
                Param("@PageSize", DefaultPageSize),
                Param("@SkipRows", 0),
                Param("@TakeRows", DefaultPageSize),
                Param("@IncludeEmptySteps", includeEmptySteps)
            };

            var table = WipDonHangModel.ExecStoredProcedure(ProcedureName, FilterSupportedParameters(parameters));
            var list = new List<WipGanttDetailDto>();
            if (table == null) return list;

            foreach (DataRow row in table.Rows)
            {
                var stepCode = GetString(row, "StepCode");
                if (string.IsNullOrWhiteSpace(stepCode)) continue;

                list.Add(new WipGanttDetailDto
                {
                    WIPId = GetInt(row, "WIPId") ?? wipId,
                    StepCode = stepCode,
                    StepName = GetString(row, "StepName"),
                    SortOrder = GetInt(row, "SortOrder"),
                    KHField = GetString(row, "KHField"),
                    TTField = GetString(row, "TTField"),
                    PlannedDate = GetDate(row, "PlannedDate"),
                    ActualDate = GetDate(row, "ActualDate"),
                    ActualText = GetString(row, "ActualText"),
                    Status = GetByte(row, "Status"),
                    IsLateLocked = GetBool(row, "IsLateLocked"),
                    IsMissingTTWarning = GetBool(row, "IsMissingTTWarning"),
                    IsNoDataConfirmed = GetBool(row, "IsNoDataConfirmed"),
                    NoDataConfirmedDate = GetDate(row, "NoDataConfirmedDate"),
                    IsManualKH = GetBool(row, "IsManualKH"),
                    IsManualTT = GetBool(row, "IsManualTT"),
                    IsCheckTT = GetBool(row, "IsCheckTT"),
                    AllowManualKH = GetBool(row, "AllowManualKH"),
                    AllowNoDataConfirm = GetBool(row, "AllowNoDataConfirm"),
                    IsTimelineStartStep = GetBool(row, "IsTimelineStartStep"),
                    IsTimelineEndStep = GetBool(row, "IsTimelineEndStep"),
                    HasProgress = GetBool(row, "HasProgress")
                });
            }

            return list
                .OrderBy(x => x.SortOrder ?? int.MaxValue)
                .ThenBy(x => x.StepCode)
                .ToList();
        }

        private static SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        private static SqlParameter[] FilterSupportedParameters(SqlParameter[] parameters)
        {
            var supportedNames = GetProcedureParameterNames();
            if (supportedNames == null || supportedNames.Count == 0) return parameters;

            return parameters
                .Where(p => supportedNames.Contains(p.ParameterName))
                .ToArray();
        }

        private static HashSet<string> GetProcedureParameterNames()
        {
            if (_procedureParameters != null) return _procedureParameters;

            lock (ParameterCacheLock)
            {
                if (_procedureParameters != null) return _procedureParameters;

                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var connection = SqlHelper.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
SELECT p.name
FROM sys.parameters p
INNER JOIN sys.objects o ON p.object_id = o.object_id
WHERE o.name = @ProcedureName
ORDER BY p.parameter_id";
                    command.Parameters.Add(new SqlParameter("@ProcedureName", ProcedureName));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add(Convert.ToString(reader["name"]));
                        }
                    }
                }

                _procedureParameters = names;
                return _procedureParameters;
            }
        }

        private static bool HasColumn(DataRow row, string columnName)
        {
            return row != null && row.Table != null && row.Table.Columns.Contains(columnName);
        }

        private static object GetValue(DataRow row, string columnName)
        {
            if (!HasColumn(row, columnName)) return null;
            var value = row[columnName];
            return value == DBNull.Value ? null : value;
        }

        private static string GetString(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? null : Convert.ToString(value);
        }

        private static int? GetInt(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (int?)null : Convert.ToInt32(value);
        }

        private static double? GetDouble(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (double?)null : Convert.ToDouble(value);
        }

        private static byte? GetByte(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (byte?)null : Convert.ToByte(value);
        }

        private static bool? GetBool(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (bool?)null : Convert.ToBoolean(value);
        }

        private static DateTime? GetDate(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (DateTime?)null : Convert.ToDateTime(value);
        }

        public class WipGanttMasterDto
        {
            public int WIPId { get; set; }
            public int? ThuTuChuyen { get; set; }
            public int? LineX { get; set; }
            public string LineName { get; set; }
            public int? RowNo { get; set; }
            public bool? IsGiaCong { get; set; }
            public int? LenhSX { get; set; }
            public string MaDH { get; set; }
            public string MaKH { get; set; }
            public string Season { get; set; }
            public string StyleId { get; set; }
            public string MaHang { get; set; }
            public string KhachHang { get; set; }
            public string PO { get; set; }
            public double? SLKH { get; set; }
            public string NameMer { get; set; }
            public DateTime? TimelineStart { get; set; }
            public DateTime? TimelineEnd { get; set; }
            public string TimelineStartStepCode { get; set; }
            public string TimelineEndStepCode { get; set; }
            public int? DetailCount { get; set; }
            public bool? HasDetail { get; set; }
            public int TotalRows { get; set; }
        }

        public class WipGanttDetailDto
        {
            public int WIPId { get; set; }
            public string StepCode { get; set; }
            public string StepName { get; set; }
            public int? SortOrder { get; set; }
            public string KHField { get; set; }
            public string TTField { get; set; }
            public DateTime? PlannedDate { get; set; }
            public DateTime? ActualDate { get; set; }
            public string ActualText { get; set; }
            public byte? Status { get; set; }
            public bool? IsLateLocked { get; set; }
            public bool? IsMissingTTWarning { get; set; }
            public bool? IsNoDataConfirmed { get; set; }
            public DateTime? NoDataConfirmedDate { get; set; }
            public bool? IsManualKH { get; set; }
            public bool? IsManualTT { get; set; }
            public bool? IsCheckTT { get; set; }
            public bool? AllowManualKH { get; set; }
            public bool? AllowNoDataConfirm { get; set; }
            public bool? IsTimelineStartStep { get; set; }
            public bool? IsTimelineEndStep { get; set; }
            public bool? HasProgress { get; set; }
        }
    }
}
