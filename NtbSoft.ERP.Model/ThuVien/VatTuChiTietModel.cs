using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class VatTuChiTietModel
    {
        public DataTable Get()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VatTuChiTiet", conn); // Gọi stored procedure SP_VatTuChiTiet
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET"); // Action để lấy dữ liệu
                cmd.Parameters.AddWithValue("@Parameter", "0"); 
                cmd.Parameters.AddWithValue("@Parameter2", "");

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

        // Lấy dữ liệu theo MaVT
        public DataTable GetByMaVT(string MaVT)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VatTuChiTiet", conn); // Gọi stored procedure SP_VatTuChiTiet
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETByMaVT"); // Action để lấy dữ liệu theo MaVT
                cmd.Parameters.AddWithValue("@Parameter", "0"); 
                cmd.Parameters.AddWithValue("@Parameter2", MaVT); // Truyền MaVT để lọc dữ liệu

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

        // Thêm hoặc cập nhật dữ liệu vào bảng VatTuChiTiet
        public string Post(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VatTuChiTiet", conn); // Gọi stored procedure SP_VatTuChiTiet
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST"); // Action để thêm/cập nhật dữ liệu
                cmd.Parameters.AddWithValue("@Parameter", "0"); 
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", tb); // Truyền DataTable vào stored procedure
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
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

        // Xóa dữ liệu trong bảng VatTuChiTiet
        public string Delete(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VatTuChiTiet", conn); // Gọi stored procedure SP_VatTuChiTiet
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE"); // Action để xóa dữ liệu
                cmd.Parameters.AddWithValue("@Parameter", id); // Truyền ID của bản ghi cần xóa
                cmd.Parameters.AddWithValue("@Parameter2", "");

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
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
    }
}
