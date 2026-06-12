using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class KeHoachDongThungTonKhoModel
    {
        public DataTable Get(string Action, string MaDH, string MaDVSX, string DotSX, string POID, string SizeTypeID, string ColorID, string ProductID, string SizeID, string MaPKL = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG_TonKho", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@MaPKL", MaPKL ?? "");
                cmd.Parameters.AddWithValue("@MaDH", MaDH);
                cmd.Parameters.AddWithValue("@MaDVSX", MaDVSX);
                cmd.Parameters.AddWithValue("@DotSX", DotSX ?? "");
                cmd.Parameters.AddWithValue("@POID", POID);
                cmd.Parameters.AddWithValue("@SizeTypeID", SizeTypeID);
                cmd.Parameters.AddWithValue("@ColorID", ColorID);
                cmd.Parameters.AddWithValue("@ProductID", ProductID);
                cmd.Parameters.AddWithValue("@Size", SizeID);
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
        public string Post(string action, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_KHDONGTHUNG_TonKho", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaPKL", "");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@MaDVSX", "");
                cmd.Parameters.AddWithValue("@DotSX", "");
                cmd.Parameters.AddWithValue("@POID", "");
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@ColorID", "");
                cmd.Parameters.AddWithValue("@ProductID", "");
                cmd.Parameters.AddWithValue("@Size", "");
                cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dt);
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
