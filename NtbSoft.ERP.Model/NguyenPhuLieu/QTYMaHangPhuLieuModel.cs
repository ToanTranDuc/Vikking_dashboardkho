using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.Qty
{
    public class QTYMaHangPhuLieuModel
    {
        public DataSet Get(string action, string Para1 = "", string Para2 = "", string Para3 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_QTY_MaHang_PhuLieu", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3 ?? "");
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataSet dt = new DataSet();
                    adt.Fill(dt);
                    return dt;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public bool Post(string action, object dt, string para1 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_QTY_MaHang_PhuLieu", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1);
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@TypeTable", dt);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                string Message = $"Function: QtyKiemQA/SaveSLKiem \nMessage: {ex.Message.ToString()}";
                // new WriteLogModel().WriteFileLog("KiemQA", Message);
                return false;
            }
        }
        public string SaveGopMH(string action, object ojdata)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("spQTY_GopMaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.NVarChar, 100);
                cmd.Parameters.AddWithValue("@TypeTable", ojdata);
                returnParameter.Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;
                return result.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public DataSet GetVai(string action, string Para1, string Para2, string Para3, string Para4,string Para5,string Para6, string Para7, string Para8)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_QTY_MaHang_KiemVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", Para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", Para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", Para6 ?? "");
                cmd.Parameters.AddWithValue("@Para7", Para7 ?? "");
                cmd.Parameters.AddWithValue("@Para8", Para8 ?? "");
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataSet dt = new DataSet();
                    adt.Fill(dt);
                    return dt;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public bool PostVai(string action, object dt, string para1 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_QTY_MaHang_KiemVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1);
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
                cmd.Parameters.AddWithValue("@Para5", "");
                cmd.Parameters.AddWithValue("@Para6",  "");
                cmd.Parameters.AddWithValue("@Para7",  "");
                cmd.Parameters.AddWithValue("@Para8", "");
                cmd.Parameters.AddWithValue("@TypeTable", dt);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                string Message = $"Function: QtyKiemQA/SaveSLKiem \nMessage: {ex.Message.ToString()}";
                //new WriteLogModel().WriteFileLog("KiemQA", Message);
                return false;
            }
        }
        public string PostVaiV2(string action, object dt,string type = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_QTY_MaHang_KiemVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
                cmd.Parameters.AddWithValue("@Para5",  "");
                cmd.Parameters.AddWithValue("@Para6",   "");
                cmd.Parameters.AddWithValue("@Para7", "");
                cmd.Parameters.AddWithValue("@Para8",  "");
                cmd.Parameters.AddWithValue(type, dt);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (Exception ex)
            {
                string Message = $"Function: QtyKiemQA/SaveSLKiem \nMessage: {ex.Message.ToString()}";
                //new WriteLogModel().WriteFileLog("KiemQA", Message);
                return "false";
            }
        }
        public string SaveGopMH_KiemVai(string action, object ojdata)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("spQTY_GopMaHang_KiemVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.NVarChar, 100);
                cmd.Parameters.AddWithValue("@TypeTable", ojdata);
                returnParameter.Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;
                return result.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}
