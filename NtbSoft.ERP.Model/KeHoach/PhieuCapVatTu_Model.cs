using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public static class PhieuCapVatTu_Model
    {
        public static DataTable GetData(string action, string mapphieu, string malenhsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Q_PhieuCapVatTu", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaPhieu", mapphieu == null ? "" : mapphieu);
                cmd.Parameters.AddWithValue("@MaPhieuYeuCau", malenhsx == null ? "" : malenhsx);
                cmd.Parameters.AddWithValue("@NguoiTao", "");
                cmd.Parameters.AddWithValue("@ThietBiTao", "");
                cmd.Parameters.AddWithValue("@GhiChu", "");
                cmd.Parameters.AddWithValue("@TB_CTiet_PhieuCap", null);
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
        public static string PostData(string maphieu, string maphieuyc, string nguoitao, string thietbi, string ghichu, DataTable ChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Q_PhieuCapVatTu", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST_PhieuCap");
                cmd.Parameters.AddWithValue("@MaPhieu", maphieu);
                cmd.Parameters.AddWithValue("@MaPhieuYeuCau", maphieuyc);
                cmd.Parameters.AddWithValue("@NguoiTao", nguoitao);
                cmd.Parameters.AddWithValue("@ThietBiTao", thietbi==null?"":thietbi);
                cmd.Parameters.AddWithValue("@GhiChu", ghichu==null?"":ghichu);
                cmd.Parameters.AddWithValue("@TB_CTiet_PhieuCap", ChiTiet);
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
