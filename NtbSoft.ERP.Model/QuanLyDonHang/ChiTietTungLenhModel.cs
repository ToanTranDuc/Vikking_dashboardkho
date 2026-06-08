using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class ChiTietTungLenhModel
    {
        public DataTable GetTongQuanTungLenh(string pageIndex, string pageSize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ChiTietTungLenh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetTongQuanTungLenh");
                cmd.Parameters.AddWithValue("@pageIndex", int.Parse(pageIndex));
                cmd.Parameters.AddWithValue("@pageSize", int.Parse(pageSize));
                cmd.Parameters.AddWithValue("@maLenhSanXuat", "");
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
        public DataTable GetTongQuanTungLenhTheoMaHang(string maHang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ChiTietTungLenh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetTongQuanTungLenhTheoMaHang");
                cmd.Parameters.AddWithValue("@pageIndex", 0);
                cmd.Parameters.AddWithValue("@pageSize", 0);
                cmd.Parameters.AddWithValue("@maLenhSanXuat", maHang);
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
        public DataTable GetChiTietTungLenh(string maLenhSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ChiTietTungLenh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetChiTietTungLenh");
                cmd.Parameters.AddWithValue("@pageIndex", 0);
                cmd.Parameters.AddWithValue("@pageSize", 0);
                cmd.Parameters.AddWithValue("@maLenhSanXuat", maLenhSX ?? "");
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
        public DataTable GetMaHang()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ChiTietTungLenh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetMaHang");
                cmd.Parameters.AddWithValue("@pageIndex", 0);
                cmd.Parameters.AddWithValue("@pageSize", 0);
                cmd.Parameters.AddWithValue("@maLenhSanXuat", "");
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
    }
}
