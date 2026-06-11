using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class PhieuGiaoNhanModel
    {
        public DataTable GetPhieuGiaoNhan(DateTime tuNgay, DateTime denNgay)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_PHIEU_GIAO_NHAN");
                cmd.Parameters.AddWithValue("@parameter",tuNgay);
                cmd.Parameters.AddWithValue("@parameter1",denNgay);
                cmd.Parameters.AddWithValue("@parameter2", "");
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
        public DataTable GetChiTietPhieuGiaoNhan(DataTable tblPGN)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_CHITIET_PHIEU_GIAO_NHAN");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", tblPGN);
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

        public DataSet GetChiTietPhieuGiaoNhanExcel(DataTable tblPGN)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_CHITIET_PHIEU_GIAO_NHAN_EXCEL");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", tblPGN);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
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

        public string UpdateIsXacNhan(DataTable tblPGN)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATE_ISXACNHAN");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", tblPGN);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string HuyIsXacNhan(string nVien,DataTable tblPGN)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "HUY_ISXACNHAN");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1","");
                cmd.Parameters.AddWithValue("@parameter2", nVien != null ? nVien : "");
                cmd.Parameters.AddWithValue("@TypeTable", tblPGN);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        // Lấy report của những thùng không gộp
        public DataSet GetReportPGN(DataTable tblPGN)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_REPORT_PGN");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", tblPGN);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
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

        public DataTable GetDSDonHangNhapKho()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_DS_DH_NHAPKHO");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
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

        public DataTable GetPhieuGiaoNhanDonHang(string maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_PHIEU_GIAO_NHAN_MADH");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", maDH);
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

        public string UpdateNgayNhapKho(DataTable tblPGN)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhieuGiaoNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATE_NGAYNHAPKHO");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2","");
                cmd.Parameters.AddWithValue("@TypeTable", tblPGN);
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
