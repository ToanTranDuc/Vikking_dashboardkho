using NtbSoft.ERP.Entity.XuLyVT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.XuLyVT
{
    public class XLVTMUAHANGModel
    {
        public DataTable Get(XuLyVTRequestGet req)
        {
            SqlConnection conn = null;
            try
            {
                ;
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_XLVT_MUAHANG", conn);
                SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, -1);
                messageParam.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", req?.Action ?? "");
                cmd.Parameters.AddWithValue("@parameter", req?.Parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", req?.Parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", req?.Parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", req?.Parameter3 ?? 0);
                cmd.Parameters.AddWithValue("@parameter4", req?.Parameter4 ?? 0);
                cmd.Parameters.AddWithValue("@parameter5", req?.Parameter5 ?? 0);
                cmd.Parameters.AddWithValue("@parameter6", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter7", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", XLVTMUAHANGTable.create());
                cmd.Parameters.Add(messageParam);
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
        public DataTable GetByTypeTable(XuLyVTRequestPost req)
        {
            SqlConnection conn = null;
            try
            {
                ;
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_XLVT_MUAHANG", conn);
                SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, -1);
                messageParam.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", req?.Action ?? "");
                cmd.Parameters.AddWithValue("@parameter", req?.Parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", req?.Parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", req?.Parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", req?.Parameter3 ?? 0);
                cmd.Parameters.AddWithValue("@parameter4", req?.Parameter4 ?? 0.0);
                cmd.Parameters.AddWithValue("@parameter5", req?.Parameter5 ?? 0.0);
                cmd.Parameters.AddWithValue("@parameter6", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter7", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", req.TypeTable.Columns.Count == 0 ? XLVTMUAHANGTable.create() : req.TypeTable);
                cmd.Parameters.Add(messageParam);
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
        public string Post(XuLyVTRequestPost req)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_XLVT_MUAHANG", conn);
                SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, -1);
                messageParam.Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", req.Action ?? "");
                cmd.Parameters.AddWithValue("@parameter", req.Parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", req.Parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", req.Parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", req.Parameter3);
                cmd.Parameters.AddWithValue("@parameter4", req.Parameter4);
                cmd.Parameters.AddWithValue("@parameter5", req.Parameter5);
                cmd.Parameters.AddWithValue("@parameter6", req.Parameter6.HasValue ? (object)req.Parameter6.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter7", req.Parameter7.HasValue ? (object)req.Parameter7.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", req.TypeTable.Columns.Count == 0? XLVTMUAHANGTable.create() : req.TypeTable);
                cmd.Parameters.Add(messageParam);
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

