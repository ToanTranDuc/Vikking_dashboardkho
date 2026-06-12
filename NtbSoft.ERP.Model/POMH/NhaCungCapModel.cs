using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.POMuaHang
{
    public class NhaCungCapModel
    {
        public DataTable Get(string action, string para, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
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
        public string Post(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTLoai");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable2", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(int parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETELOAI");
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteMMTB_LK(string para, string para1, string para2, string para3, string para4, string para5, string action)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
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

        public string Postkhloai(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTKHLoai");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostCLMMTB(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostCLMMTB");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostKH(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTKH");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostCLLKMMTB(DataTable tbl, string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostCLLKMMTB");
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string InsertOrUpdate(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "InsertOrUpdate");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteDG(int parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEDG");
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }


        public DataTable GetPhieuBG(string action, string para, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuBaoGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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

        public string Postphieu(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuBaoGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeletePhieu(string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeletePhieu");
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteLoaiNCC(string para, string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETELOAINCC");
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteCLMMTB(string para, string para1, string para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteCLMMTB");
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteChungLoaiNCC(string para, string para1, string para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEChungLOAINCC");
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteNCC(string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteNCC");
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostCLKH(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTKHChungLoai");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }


        public DataTable Getdanhgia(string action, string para, string para1, string para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DanhGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public string Postdanhgia(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DanhGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Deletedanhgia(int parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DanhGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        ////PHIẾU MUA HÀNG
        #region phieumuahang VT
        public DataTable GetPhieuMuaHang(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandTimeout = 3000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@para5", para5 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable1", null);
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

        public string PostPMH(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@Action", "POSTPhieu");
                cmd.Parameters.AddWithValue("@para1", 0);
                cmd.Parameters.AddWithValue("@para2", 0);
                cmd.Parameters.AddWithValue("@para3", 0);
                cmd.Parameters.AddWithValue("@para4", 0);
                cmd.Parameters.AddWithValue("@para5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable1", null);

                
                SqlParameter outputParam = new SqlParameter("@result", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Output,
                    Size = 500  
                };
                cmd.Parameters.Add(outputParam);

                cmd.ExecuteNonQuery();

            
                string result = outputParam.Value != null && outputParam.Value != DBNull.Value
                    ? outputParam.Value.ToString()
                    : "True";

                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
        }

        public string PostPMHDH(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTPHIEUDH");
                cmd.Parameters.AddWithValue("@para1", 0);
                cmd.Parameters.AddWithValue("@para2", 0);
                cmd.Parameters.AddWithValue("@para3", 0);
                cmd.Parameters.AddWithValue("@para4", 0);
                cmd.Parameters.AddWithValue("@para5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostXN(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTXacNhan");
                cmd.Parameters.AddWithValue("@para1", 0);
                cmd.Parameters.AddWithValue("@para2", 0);
                cmd.Parameters.AddWithValue("@para3", 0);
                cmd.Parameters.AddWithValue("@para4", 0);
                cmd.Parameters.AddWithValue("@para5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostHuy(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHuy");
                cmd.Parameters.AddWithValue("@para1", 0);
                cmd.Parameters.AddWithValue("@para2", 0);
                cmd.Parameters.AddWithValue("@para3", 0);
                cmd.Parameters.AddWithValue("@para4", 0);
                cmd.Parameters.AddWithValue("@para5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteRowPMH(string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteVatTuPhieuMuaHang");
                cmd.Parameters.AddWithValue("@para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@para5", para5 ?? "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion

        //PhieuMuaHang chi phí phát sinh và đợt giao hàng
        //
        #region phieumuahang CP&DGH
        public DataTable GetCP_DOT(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
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
                cmd.Parameters.AddWithValue("@TypeTable4", null);
                cmd.Parameters.AddWithValue("@TypeTable5", null);
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

        public string PostCP(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCP");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.Parameters.AddWithValue("@TypeTable4", null);
                cmd.Parameters.AddWithValue("@TypeTable5", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteCP(int parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETECP");
                cmd.Parameters.AddWithValue("@Parameter1", parameter);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostCPPhieuMH(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCPPMH");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", tbl);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.Parameters.AddWithValue("@TypeTable4", null);
                cmd.Parameters.AddWithValue("@TypeTable5", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostDotGHPMH(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDotGHPMH");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.Parameters.AddWithValue("@TypeTable4", null);
                cmd.Parameters.AddWithValue("@TypeTable5", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostHT(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHTTT");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.Parameters.AddWithValue("@TypeTable4", tbl);
                cmd.Parameters.AddWithValue("@TypeTable5", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteTT(string para, string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEHTTT");
                cmd.Parameters.AddWithValue("@Parameter1", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteCPPMH(string para, string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETECPPMH");
                cmd.Parameters.AddWithValue("@Parameter1", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteDotPMH(string para, string para1, string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEDotGHPMH");
                cmd.Parameters.AddWithValue("@Parameter1", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para4 ?? "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion
        // invoice
        #region invoice
        public DataTable GetInvoice(string action, string para, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Invoice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public string PostInvoice(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Invoice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTINVOICE");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteInvoice(int parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NhaCungCap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEINVOICE");
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion

        #region QC
        public string PostGhiChuNL(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GhiChuQC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTNL");
                cmd.Parameters.AddWithValue("@para1", "");
                cmd.Parameters.AddWithValue("@para2", "");
                cmd.Parameters.AddWithValue("@para3", "");
                cmd.Parameters.AddWithValue("@para4", "");
                cmd.Parameters.AddWithValue("@para5", "");
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostGhiChuPL(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GhiChuQC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTPL");
                cmd.Parameters.AddWithValue("@para1", "");
                cmd.Parameters.AddWithValue("@para2", "");
                cmd.Parameters.AddWithValue("@para3", "");
                cmd.Parameters.AddWithValue("@para4", "");
                cmd.Parameters.AddWithValue("@para5", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion

        #region phieumuahangMMTB
        public DataTable GetPhieuMuaHangMMTB(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang_MMTB", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@para5", para5 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public string PostPMHMMTB(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang_MMTB", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTPhieuMHTB");
                cmd.Parameters.AddWithValue("@para1", 0);
                cmd.Parameters.AddWithValue("@para2", 0);
                cmd.Parameters.AddWithValue("@para3", 0);
                cmd.Parameters.AddWithValue("@para4", 0);
                cmd.Parameters.AddWithValue("@para5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                SqlParameter outputParam = new SqlParameter("@result", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Output,
                    Size = 500
                };
                cmd.Parameters.Add(outputParam);

                cmd.ExecuteNonQuery();


                string result = outputParam.Value != null && outputParam.Value != DBNull.Value
                    ? outputParam.Value.ToString()
                    : "True";

                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostDotGHPMHMMTB(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDotGHPMHMMTB");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.Parameters.AddWithValue("@TypeTable4", null);
                cmd.Parameters.AddWithValue("@TypeTable5", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteDotPMHMMTB(string para, string para1, string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ThuVienPMH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEDotGHPMH");
                cmd.Parameters.AddWithValue("@Parameter1", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para4 ?? "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostXNMMTB(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang_MMTB", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTXacNhan");
                cmd.Parameters.AddWithValue("@para1", 0);
                cmd.Parameters.AddWithValue("@para2", 0);
                cmd.Parameters.AddWithValue("@para3", 0);
                cmd.Parameters.AddWithValue("@para4", 0);
                cmd.Parameters.AddWithValue("@para5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion

        #region Thư viện phiếu mua
        public string PostTau(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTTAU");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTableTau", tbl);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostCang(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCANG");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTableCang", tbl);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion

        #region E Invoice Phiếu mua hàng

        public string PostEInvoice(string action,DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuMuaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable_POMH_PhieuMuaHang_E_Invoice", tbl);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        #endregion

    }
}
