using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class ScanBarcodeModel
    {
        public DataSet Get(string action, string Para1, string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "",
                                                                            string Para6 = "", string Para7 = "", string Para8 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ScanBarcodeXuatHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3);
                cmd.Parameters.AddWithValue("@Para4", Para4);
                cmd.Parameters.AddWithValue("@Para5", Para5);
                cmd.Parameters.AddWithValue("@Para6", Para6);
                cmd.Parameters.AddWithValue("@Para7", Para7);
                cmd.Parameters.AddWithValue("@Para8", Para8);


                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex)
            {
                return null;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string UpdateScan(string action, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ScanBarcodeXuatHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
                cmd.Parameters.AddWithValue("@Para5", "");
                cmd.Parameters.AddWithValue("@Para6", "");
                cmd.Parameters.AddWithValue("@Para7", "");
                cmd.Parameters.AddWithValue("@Para8", "");
                
                if (action == "UpdateScan_CT" || action == "Update_InitBarcode") cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dt);
                else cmd.Parameters.AddWithValue("@TypeTable", dt);
                cmd.ExecuteNonQuery();
                return "true";
            }
            catch (SqlException ex)
            {
                return "false";
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
