using Newtonsoft.Json;
using NtbSoft.ERP.Entity.WipDonHang;
using NtbSoft.ERP.Model.WipDonHang;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.WipDonHang
{
    [RoutePrefix("api/PhanQuyenWIP")]
    public class PhanQuyenWIPController : ApiController
    {
        public class FieldPermissionDto
        {
            public string ColKey { get; set; }
            public bool IsView { get; set; }
            public bool IsEdit { get; set; }
            public int LineX { get; set; }
        }

        public class WipFeaturePermissionRequest
        {
            public string Action { get; set; }
            public string UserID { get; set; }
            public string FeatureKey { get; set; }
            public bool? IsAllow { get; set; }
        }

        // --- 1. API GET DATA (LOAD GRID) ---
        [HttpGet]
        [Route("GetData")]
        public IHttpActionResult GetData(string userID, int lineX = 0)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "GET"),
                    new SqlParameter("@UserID", (object)userID ?? DBNull.Value),
                    new SqlParameter("@LineX", lineX),
                };

                // Gọi qua Helper của bạn
                DataTable dt = WipDonHangModel.ExecStoredProcedure("Sp_PhanQuyenWIP", parameters);

                if (dt == null) dt = new DataTable();

                return Ok(dt);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // --- 2. API SAVE DATA (UPDATE 1 DÒNG) ---
        [HttpPost]
        [Route("SaveData")]
        public IHttpActionResult SaveData(string userID, string colKey, string isView, string isEdit, int lineX = 0)
        {
            try
            {
                object valView = DBNull.Value;
                if (!string.IsNullOrEmpty(isView)) valView = bool.Parse(isView);

                object valEdit = DBNull.Value;
                if (!string.IsNullOrEmpty(isEdit)) valEdit = bool.Parse(isEdit);

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "SAVE"),
                    new SqlParameter("@UserID", (object)userID ?? DBNull.Value),
                    new SqlParameter("@LineX", lineX),
                    new SqlParameter("@ColKey", (object)colKey ?? DBNull.Value),
                    new SqlParameter("@IsView", valView),
                    new SqlParameter("@IsEdit", valEdit)
                };

                WipDonHangModel.ExecStoredProcedure("Sp_PhanQuyenWIP", parameters);

                return Ok("True");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // --- 3. API GET FIELD PERMISSIONS (DANH SÁCH CỘT + QUYỀN VIEW/EDIT) ---
        [HttpGet]
        [Route("GetFieldPermissions")]
        public IHttpActionResult GetFieldPermissions(string userID, int lineX = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userID))
                    return BadRequest("userID is required");

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", "GET_FIELD_PERMISSIONS"),
                    new SqlParameter("@UserID", (object)userID ?? DBNull.Value),
                    new SqlParameter("@LineX", lineX),
                };

                DataTable dt = WipDonHangModel.ExecStoredProcedure("Sp_PhanQuyenWIP", parameters);

                return Ok(MapFieldPermissions(dt));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static List<FieldPermissionDto> MapFieldPermissions(DataTable dt)
        {
            var result = new List<FieldPermissionDto>();
            if (dt == null || dt.Rows.Count == 0) return result;

            foreach (DataRow row in dt.Rows)
            {
                var colKey = row["ColKey"]?.ToString();
                if (string.IsNullOrWhiteSpace(colKey)) continue;

                result.Add(new FieldPermissionDto
                {
                    ColKey = colKey.Trim(),
                    IsView = ReadBoolean(row, "IsView"),
                    IsEdit = ReadBoolean(row, "IsEdit"),
                    LineX = ReadInt(row, "LineX")
                });
            }

            return result;
        }

        private static bool ReadBoolean(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName)) return false;

            var value = row[columnName];
            if (value == null || value == DBNull.Value) return false;

            return Convert.ToBoolean(value);
        }
        private static int ReadInt(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName)) return 0;

            var value = row[columnName];
            if (value == null || value == DBNull.Value) return 0;

            return Convert.ToInt32(value);
        }

        [HttpGet]
        [Route("Feature")]
        public IHttpActionResult GetFeatureData(string action, string userID = null)
        {
            try
            {
                action = (action ?? string.Empty).Trim().ToUpperInvariant();
                if (!IsValidFeatureGetAction(action))
                    return BadRequest("Invalid action");

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", action),
                    new SqlParameter("@UserID", (object)userID ?? DBNull.Value),
                    new SqlParameter("@FeatureKey", DBNull.Value),
                    new SqlParameter("@IsAllow", DBNull.Value)
                };

                DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_FEATURE", parameters);
                if (dt == null) dt = new DataTable();

                return Ok(dt);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("Feature")]
        public IHttpActionResult SaveFeatureData(WipFeaturePermissionRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Body is empty");

                var action = (request.Action ?? string.Empty).Trim().ToUpperInvariant();
                if (action != "SAVE_USER_FEATURE")
                    return BadRequest("Invalid action");

                if (string.IsNullOrWhiteSpace(request.UserID))
                    return BadRequest("UserID is required");

                if (string.IsNullOrWhiteSpace(request.FeatureKey))
                    return BadRequest("FeatureKey is required");

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Action", action),
                    new SqlParameter("@UserID", (object)request.UserID ?? DBNull.Value),
                    new SqlParameter("@FeatureKey", (object)request.FeatureKey ?? DBNull.Value),
                    new SqlParameter("@IsAllow", (object)request.IsAllow ?? DBNull.Value)
                };

                DataTable dt = WipDonHangModel.ExecStoredProcedure("SP_WIP_FEATURE", parameters);
                if (dt == null) dt = new DataTable();

                return Ok(dt);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Keep GET feature actions explicit so FE cannot call arbitrary proc branches.
        private static bool IsValidFeatureGetAction(string action)
        {
            return action == "GET_FEATURES"
                || action == "GET_USER_FEATURES"
                || action == "GET_ALLOWED_FEATURE_KEYS";
        }
    }
}
