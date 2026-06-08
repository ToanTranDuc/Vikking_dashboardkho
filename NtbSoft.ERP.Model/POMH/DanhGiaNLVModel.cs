using NtbSoft.ERP.Entity.POMuaHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.POMuaHang
{
    public class DanhGiaNLVModel
    {
        public DataTable Get(XuLyVTRequestGet req)
        {
            SqlConnection conn = null;
            try
            {
                ;
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIANLV", conn);
                SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, -1);
                messageParam.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", req?.Action ?? "");
                cmd.Parameters.AddWithValue("@parameter", req?.Parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", req?.Parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", req?.Parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", req?.Parameter3 ?? 0);
                cmd.Parameters.AddWithValue("@parameter4", req?.Parameter4 ?? 0);
                cmd.Parameters.AddWithValue("@parameter5", req?.Parameter5 ?? 0);
                cmd.Parameters.AddWithValue("@parameter6", req.Parameter6.HasValue ? (object)req.Parameter6.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter7", req.Parameter7.HasValue ? (object)req.Parameter7.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", DanhGiaNoiLamBViecTable.create());
                cmd.Parameters.Add(messageParam);
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
        public DataTable GetByTypeTable(XuLyVTRequestPost req)
        {
            SqlConnection conn = null;
            try
            {
                ;
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIANLV", conn);
                SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, -1);
                messageParam.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", req?.Action ?? "");
                cmd.Parameters.AddWithValue("@parameter", req?.Parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", req?.Parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", req?.Parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", req?.Parameter3 ?? 0);
                cmd.Parameters.AddWithValue("@parameter4", req?.Parameter4 ?? 0.0);
                cmd.Parameters.AddWithValue("@parameter5", req?.Parameter5 ?? 0.0);
                cmd.Parameters.AddWithValue("@parameter6", req.Parameter6.HasValue ? (object)req.Parameter6.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter7", req.Parameter7.HasValue ? (object)req.Parameter7.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", req.TypeTable.Columns.Count == 0 ? DanhGiaNoiLamBViecTable.create() : req.TypeTable);
                cmd.Parameters.Add(messageParam);
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
        public string Post(XuLyVTRequestPost req)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIANLV", conn);
                SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, 500);
                messageParam.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", req.Action ?? "");
                cmd.Parameters.AddWithValue("@parameter", req.Parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", req.Parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", req.Parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", req.Parameter3);
                cmd.Parameters.AddWithValue("@parameter4", req.Parameter4);
                cmd.Parameters.AddWithValue("@parameter5", req.Parameter5);
                cmd.Parameters.AddWithValue("@parameter6", req.Parameter6.HasValue ? (object)req.Parameter6.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter7", req.Parameter7.HasValue ? (object)req.Parameter7.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", req.TypeTable.Columns.Count == 0 ? DanhGiaNoiLamBViecTable.create() : req.TypeTable);
                cmd.Parameters.Add(messageParam);
                cmd.ExecuteNonQuery();

                string message = messageParam.Value != DBNull.Value ? (string)messageParam.Value : "True";

                return message;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }


        /////// THU VIEN DANH GIA TONG QUAN
        ///
        public DataTable GetTQ(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_POMH_DANHGIA_TQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        private DataTable CreateEmptyTypeTable1()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("TieuChiTQ", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("LoaiDanhGia", typeof(string));
            dt.Columns.Add("Version", typeof(int));
            dt.Columns.Add("IsAcTive", typeof(bool));
            return dt;
        }

        private DataTable CreateEmptyTypeTable2()
        {
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("ID", typeof(int));
            dt2.Columns.Add("STT", typeof(int));
            dt2.Columns.Add("TieuChiTQ_ID", typeof(int));
            dt2.Columns.Add("Diem", typeof(int));
            dt2.Columns.Add("ChiTiet", typeof(string));
            dt2.Columns.Add("Version", typeof(int));
            dt2.Columns.Add("IsActive", typeof(bool));
            return dt2;
        }

        private DataTable CreateEmptyTypeTable3()
        {
            DataTable dt3 = new DataTable();
            dt3.Columns.Add("ID", typeof(int));
            dt3.Columns.Add("MaDiemDanhGia", typeof(string));
            dt3.Columns.Add("DiemDanhGia", typeof(string));
            dt3.Columns.Add("Version", typeof(int));
            dt3.Columns.Add("IsActive", typeof(bool));
            return dt3;
        }

        public string PostTQ(
            string action,
            DataTable tbl,
            DataTable tbl2,
            DataTable tbl3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand("SP_POMH_DANHGIA_TQ", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Action & parameters
                    cmd.Parameters.Add("@Action", SqlDbType.VarChar, 20).Value = action;
                    cmd.Parameters.Add("@Parameter1", SqlDbType.NVarChar, 150).Value = DBNull.Value;
                    cmd.Parameters.Add("@Parameter2", SqlDbType.NVarChar, 150).Value = DBNull.Value;
                    cmd.Parameters.Add("@Parameter3", SqlDbType.NVarChar, 150).Value = DBNull.Value;
                    cmd.Parameters.Add("@Parameter4", SqlDbType.NVarChar, 150).Value = DBNull.Value;
                    cmd.Parameters.Add("@Parameter5", SqlDbType.NVarChar, 150).Value = DBNull.Value;

                    // TVP
                    SqlParameter p1 = cmd.Parameters.Add(
                                   "@TypeTable",
                                   SqlDbType.Structured);
                    p1.TypeName = "dbo.TYPE_POMH_DIC_DANHGIA_NCC_TIEUCHITQ";
                    p1.Value = tbl ?? CreateEmptyTypeTable1();

                    // ===== TVP 2 =====
                    SqlParameter p2 = cmd.Parameters.Add(
                        "@TypeTable2",
                        SqlDbType.Structured);
                    p2.TypeName = "dbo.ERP_POMH_DIC_DANHGIA_NCC_TIEUCHITQ_DIEM";
                    p2.Value = tbl2 ?? CreateEmptyTypeTable2(); // đúng schema, 0 row

                    // ===== TVP 3 =====
                    SqlParameter p3 = cmd.Parameters.Add(
                        "@TypeTable3",
                        SqlDbType.Structured);
                    p3.TypeName = "dbo.ERP_POMH_DIC_DANHGIA_NCC_TIEUCHITQ_DIEMDG";
                    p3.Value = tbl3 ?? CreateEmptyTypeTable3(); // đúng schema, 0 row

                    cmd.ExecuteNonQuery();
                    
                }
                return "True";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string Delete(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_POMH_DANHGIA_TQ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para5 ?? "");
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

