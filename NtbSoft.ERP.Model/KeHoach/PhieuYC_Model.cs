using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public static class PhieuYC_Model
    {
        public static DataTable GetData(string action, string mapphieu, string malenhsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Q_PhieuYeuCau", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@maphieu", mapphieu == null ? "" : mapphieu);
                cmd.Parameters.AddWithValue("@malenhsx", malenhsx == null ? "" : malenhsx);
                cmd.Parameters.AddWithValue("@nguoitao", "");
                cmd.Parameters.AddWithValue("@thietbi", "");
                cmd.Parameters.AddWithValue("@ghichu", "");
                cmd.Parameters.AddWithValue("@TB_CTiet_PhieuYC", null);
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
        public static string PostData(string maphieu, string malenhsx, string nguoitao, string thietbi, string ghichu, DataTable ChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Q_PhieuYeuCau", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST_Phieu");
                cmd.Parameters.AddWithValue("@maphieu", maphieu);
                cmd.Parameters.AddWithValue("@malenhsx", malenhsx);
                cmd.Parameters.AddWithValue("@nguoitao", nguoitao);
                cmd.Parameters.AddWithValue("@thietbi", thietbi);
                cmd.Parameters.AddWithValue("@ghichu", ghichu == null ? "" : ghichu);
                cmd.Parameters.AddWithValue("@TB_CTiet_PhieuYC", ChiTiet);
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
