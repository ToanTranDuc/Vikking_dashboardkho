using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class CaiDatBarCodeModel
    {
        public DataTable Get(string action, string Para1, string Para2, string Para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dic_CaiDatBarCode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para1);
                cmd.Parameters.AddWithValue("@Para2", Para2);
                cmd.Parameters.AddWithValue("@Para3", Para3);
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
        public string Post(string action, string Para1, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dic_CaiDatBarCode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para1);
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@TypeTable", dt);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetNew(string Para1, string Para2, string Para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dic_CaiDatBarCodeNew", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "Get");
                cmd.Parameters.AddWithValue("@Para1", Para1);
                cmd.Parameters.AddWithValue("@Para2", Para2);
                cmd.Parameters.AddWithValue("@Para3", Para3);
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
        public string PostNew(string action,string Para1, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Dic_CaiDatBarCodeNew", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para1);
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@TypeTable", dt);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public static bool CheckBarCodeFromApp(DataTable tblConvertSaveCaiDaBarcode, DataRow dr)
        {

            var drCheck = tblConvertSaveCaiDaBarcode.AsEnumerable().Where(x => x["POID"].ToString() == dr["POID"].ToString()
                                                                              && x["Season"].ToString() == dr["Dot"].ToString()
                                                                              && x["DauSizeID"].ToString() == dr["DauSizeID"].ToString()
                                                                              && x["MaMau"].ToString() == dr["ColorID"].ToString()
                                                                              && x["SizeID"].ToString() == dr["SizeID"].ToString()).FirstOrDefault();
            if (drCheck != null)
            {
                //if ((bool)dr["IsThungLe"]) drCheck["BarCodeChan"] = dr["Barcode"];
                foreach (DataRow x in tblConvertSaveCaiDaBarcode.Rows)
                {
                    if (x["POID"].ToString() == drCheck["POID"].ToString()
                       && x["Season"].ToString() == drCheck["Season"].ToString()
                       && x["DauSizeID"].ToString() == drCheck["DauSizeID"].ToString()
                       && x["MaMau"].ToString() == drCheck["MaMau"].ToString()
                       && x["SizeID"].ToString() == drCheck["SizeID"].ToString())
                    {
                        if ((bool)dr["IsThungLe"]) x["BarCodeLe"] = dr["Barcode"];
                        else x["BarCodeChan"] = dr["Barcode"];
                    }
                    break;
                }
                return false;
            }
            return true;
        }
    }
}
