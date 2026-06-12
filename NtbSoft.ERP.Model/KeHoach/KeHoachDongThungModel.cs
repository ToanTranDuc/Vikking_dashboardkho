using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class KeHoachDongThungModel
    {
        public DataTable Get(string Action, string MaDH, string MaDVSX, string DotSX, string POID, string SizeTypeID, string ColorID, string ProductID, string SizeID, string MaPKL = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@MaPKL", MaPKL ?? "");
                cmd.Parameters.AddWithValue("@MaDH", MaDH ?? "");
                cmd.Parameters.AddWithValue("@MaDVSX", MaDVSX ?? "");
                cmd.Parameters.AddWithValue("@DotSX", DotSX ?? "");
                cmd.Parameters.AddWithValue("@POID", POID ?? "");
                cmd.Parameters.AddWithValue("@SizeTypeID", SizeTypeID ?? "");
                cmd.Parameters.AddWithValue("@ColorID", ColorID ?? "");
                cmd.Parameters.AddWithValue("@ProductID", ProductID ?? "");
                cmd.Parameters.AddWithValue("@Size", SizeID ?? "");
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetV2(string Action, string MaDH, string POID, string SizeTypeID, string ColorID, string MaPKL = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG_V2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@MaPKL", MaPKL ?? "");
                cmd.Parameters.AddWithValue("@MaDH", MaDH ?? "");
                cmd.Parameters.AddWithValue("@POID", POID ?? "");
                cmd.Parameters.AddWithValue("@SizeTypeID", SizeTypeID);
                cmd.Parameters.AddWithValue("@ColorID", ColorID);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Post(string action, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaPKL", "");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@MaDVSX", "");
                cmd.Parameters.AddWithValue("@DotSX", "");
                cmd.Parameters.AddWithValue("@POID", "");
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@ColorID", "");
                cmd.Parameters.AddWithValue("@ProductID", "");
                cmd.Parameters.AddWithValue("@Size", "");
                cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dt);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string CopyPaste(string action, string MaDHDes, string MaPKL, DataTable dt, DataTable dt1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaPKL", MaPKL);
                cmd.Parameters.AddWithValue("@MaDH", MaDHDes);
                cmd.Parameters.AddWithValue("@MaDVSX", "");
                cmd.Parameters.AddWithValue("@DotSX", "");
                cmd.Parameters.AddWithValue("@POID", "");
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@ColorID", "");
                cmd.Parameters.AddWithValue("@ProductID", "");
                cmd.Parameters.AddWithValue("@Size", "");
                if (action == "CopyPasteKHDongThung")
                {
                    cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dt);
                    cmd.Parameters.AddWithValue("@TypeCopy", dt1);
                }

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string CancelCopyPaste(string action, string MaDHDes, string POID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaPKL", "");
                cmd.Parameters.AddWithValue("@MaDH", MaDHDes);
                cmd.Parameters.AddWithValue("@MaDVSX", "");
                cmd.Parameters.AddWithValue("@DotSX", "");
                cmd.Parameters.AddWithValue("@POID", POID);
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@ColorID", "");
                cmd.Parameters.AddWithValue("@ProductID", "");
                cmd.Parameters.AddWithValue("@Size", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public async Task<DataTable> Get(string Action, string Para1, string Para2, string Para3, string Para4, string UserName, string status, string IsNhapKho, string IsLuanChuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("Sp_Option_DongThung_NhapKho", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@para1", string.IsNullOrEmpty(Para1) ? (object)DBNull.Value : Para1);
                cmd.Parameters.AddWithValue("@para2", string.IsNullOrEmpty(Para2) ? (object)DBNull.Value : Para2);
                cmd.Parameters.AddWithValue("@para3", string.IsNullOrEmpty(Para3) ? (object)DBNull.Value : Para3);
                cmd.Parameters.AddWithValue("@para4", string.IsNullOrEmpty(Para4) ? (object)DBNull.Value : Para4);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@IsNhapKho", IsNhapKho);
                cmd.Parameters.AddWithValue("@IsLuanChuyen", IsLuanChuyen);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    await Task.Run(() => adt.Fill(ds));
                    return ds;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostGopPO(string action, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_GopPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@TypeTable", dt);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
