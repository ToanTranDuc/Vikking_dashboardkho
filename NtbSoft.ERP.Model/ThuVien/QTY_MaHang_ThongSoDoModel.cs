using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class QTY_MaHang_ThongSoDoModel
    {
        public DataTable GetDH(string action, string para1, string para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("spQTY_CaiDatThongSo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", para2 ?? "");
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
        public DataTable Get(string Action, string maLenh, string styleID, string status, string season, string sizeTypeID, string rap,string poid = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("sp_Dic_ThongSoMahang2", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@MaLenh", maLenh);
                cmd.Parameters.AddWithValue("@StyleID", styleID);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Season", season);
                cmd.Parameters.AddWithValue("@SizeTypeID", sizeTypeID);
                cmd.Parameters.AddWithValue("@Rap", rap);
                cmd.Parameters.AddWithValue("@TableQTY", poid);
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

        public string Post(string action, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("sp_Dic_ThongSoMahang2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaLenh", "");
                cmd.Parameters.AddWithValue("@StyleID", "");
                cmd.Parameters.AddWithValue("@Status", "");
                cmd.Parameters.AddWithValue("@Season", "");
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@Rap", "");
                cmd.Parameters.AddWithValue("@TableQTY", "");
                if (action.ToLower().Contains("save") || action.ToLower().Contains("delete"))
                    cmd.Parameters.AddWithValue("@TypeThongSoMaHang", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetMaHang_Season(string action, string maHang, string sizeTypeID, string season, string version)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("spQtyCaiDatThongSoMaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaHang", maHang);
                cmd.Parameters.AddWithValue("@SizeTypeID", sizeTypeID);
                cmd.Parameters.AddWithValue("@Season", season);
                cmd.Parameters.AddWithValue("@Version", version);

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
        public string PostMaHang_Season(string action, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("spQtyCaiDatThongSoMaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaHang", "");
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@Season", "");
                cmd.Parameters.AddWithValue("@Version", "");
                //if (action.ToLower().Contains("save") || action.ToLower().Contains("delete"))
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
        public DataTable GetCongDoanIE(string Action, string para, string para1, string para2, string para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("sp_Dic_CongDoanIE", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Parameter", para);
                cmd.Parameters.AddWithValue("@Parameter1", para1);
                cmd.Parameters.AddWithValue("@Parameter2", para2);
                cmd.Parameters.AddWithValue("@Parameter3", para3);
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
        public string PostCongDoanIE(string action, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("sp_Dic_CongDoanIE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                if (action.ToLower().Contains("save") || action.ToLower().Contains("delete"))
                    cmd.Parameters.AddWithValue("@TypeCongDoanIE", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetGhiChu(string action, string para1, string para2, string para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("SP_DIC_THONGSO_MAHANG_GHICHU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", para3 ?? "");
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

        public string PostGhiChu(string action, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("SP_DIC_THONGSO_MAHANG_GHICHU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
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
    }
}
