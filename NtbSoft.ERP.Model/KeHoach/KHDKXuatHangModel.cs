using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class KHDKXuatHangModel
    {
        public DataTable GetBrand(string maDH= "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHDKXuatHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETBrand");
                cmd.Parameters.AddWithValue("@MaDH", maDH);
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                SqlParameter tvp = cmd.Parameters.Add("@TypeTable", SqlDbType.Structured);
                tvp.TypeName = "dbo.TYPE_KHDKXuatHang";
                tvp.Value = BuildEmptyTypeTable();
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex) { throw new Exception(ex.Message); }
            finally { conn?.Close(); conn?.Dispose(); }
        }
        public DataTable Get(string maDH = "", string parameter = "", string parameter2 = "")
            {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHDKXuatHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET");
                cmd.Parameters.AddWithValue("@MaDH", maDH ?? "");
                cmd.Parameters.AddWithValue("@Parameter", parameter ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", parameter2 ?? "");
                SqlParameter tvp = cmd.Parameters.Add("@TypeTable", SqlDbType.Structured);
                tvp.TypeName = "dbo.TYPE_KHDKXuatHang";
                tvp.Value = BuildEmptyTypeTable(); 

                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex) { throw new Exception(ex.Message); }
            finally { conn?.Close(); conn?.Dispose(); }
        }

        public string Post(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHDKXuatHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");

                SqlParameter tvp = cmd.Parameters.AddWithValue("@TypeTable", tbl);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TYPE_KHDKXuatHang"; 

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex) { throw new Exception(ex.Message); }
            finally { conn?.Close(); conn?.Dispose(); }
        }

        public string Delete(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHDKXuatHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@Parameter", id.ToString());
                cmd.Parameters.AddWithValue("@Parameter2", "");
                SqlParameter tvp = cmd.Parameters.Add("@TypeTable", SqlDbType.Structured);
                tvp.TypeName = "dbo.TYPE_KHDKXuatHang";
                tvp.Value = BuildEmptyTypeTable();

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex) { throw new Exception(ex.Message); }
            finally { conn?.Close(); conn?.Dispose(); }
        }

        private DataTable BuildEmptyTypeTable()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("KHDKID", typeof(int));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("NgayDKGiao", typeof(DateTime)); 
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("SL_Size", typeof(string));
            tbl.Columns.Add("ThucXuat", typeof(int));
            tbl.Columns.Add("NgayXuatThucTe", typeof(DateTime));
            tbl.Columns.Add("SoKien", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MaBrand", typeof(string)); 
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NguoiSua", typeof(string));
            return tbl;
        }
    }
}