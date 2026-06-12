using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class CapThemModel
    {
        public DataTable GetLichSu(string madh, string malenh, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dot", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@MaNPL", manpl == null ? "" : manpl);
                cmd.Parameters.AddWithValue("@MaVT", mavt == null ? "" : mavt);
                cmd.Parameters.AddWithValue("@MaMauLenh", mamau == null ? "" : mamau);
                cmd.Parameters.AddWithValue("@DauSizeLenh", dausize == null ? "" : dausize);
                cmd.Parameters.AddWithValue("@SizeLenh", size == null ? "" : size);
                cmd.Parameters.AddWithValue("@MaBom", mabom == null ? "" : mabom);
                cmd.Parameters.AddWithValue("@MaVtMau", mavtmau == null ? "" : mavtmau);
                //cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public DataTable GetDinhMucCD(string madh, string maLenhSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dot", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDINHMUCNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", maLenhSX ?? "");
                //cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public string PostDotDinhMuc(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dot", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                //cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteDot(string madh, string malenhsanxuat, string dot, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dot", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@Dot", dot == null ? "" : dot);
                cmd.Parameters.AddWithValue("@parameter", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@MaNPL", manpl == null ? "" : manpl);
                cmd.Parameters.AddWithValue("@MaVT", mavt == null ? "" : mavt);
                cmd.Parameters.AddWithValue("@MaMauLenh", mamau == null ? "" : mamau);
                cmd.Parameters.AddWithValue("@DauSizeLenh", dausize == null ? "" : dausize);
                cmd.Parameters.AddWithValue("@SizeLenh", size == null ? "" : size);
                cmd.Parameters.AddWithValue("@MaBom", mabom == null ? "" : mabom);
                cmd.Parameters.AddWithValue("@MaVtMau", mavtmau == null ? "" : mavtmau);
                //cmd.Parameters.AddWithValue("@parameter2", 0);
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
