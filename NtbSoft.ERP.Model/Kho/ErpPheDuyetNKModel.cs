using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.ERP.Kho
{
    public class ErpPheDuyetNKModel
    {
        private const string ProcedureName = "spERP_PheDuyetNPL";

        public DataTable Get(
            string action,
            string parameter = null,
            string parameter1 = null,
            string parameter2 = null,
            string parameter3 = null,
            string parameter4 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand(ProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action ?? "");
                cmd.Parameters.AddWithValue("@parameter", parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", parameter3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", parameter4 ?? "");
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adt.Fill(dt);
                    return dt;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
        }

        public DataTable Get(
            string action,
            string parameter = null,
            string parameter1 = null,
            string parameter2 = null)
        {
            return Get(action, parameter, parameter1, parameter2, null, null);
        }

        public string Post(
            string action,
            object tb,
            string parameter = null,
            string parameter1 = null,
            string parameter2 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand(ProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action ?? "");
                cmd.Parameters.AddWithValue("@parameter", parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", parameter1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");

                DataTable typeTable = BuildTypeTable(tb as DataTable);
                SqlParameter tvp = cmd.Parameters.AddWithValue("@TypeTable", typeTable);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TypeERP_PheDuyetNPL";

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
        }

        public DataTable GetDetail(
            string parameter = null,
            string parameter2 = null,
            string parameter3 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand(ProcedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETDETAIL");
                cmd.Parameters.AddWithValue("@parameter", parameter ?? "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", parameter2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", parameter3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adt.Fill(dt);
                    return dt;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
        }

        private static DataTable BuildTypeTable(DataTable source)
        {
            DataTable result = new DataTable();
            result.Columns.Add("SoLoID", typeof(string));
            result.Columns.Add("MaNPL", typeof(string));
            result.Columns.Add("IsNPL", typeof(bool));

            if (source == null || source.Rows.Count == 0)
                return result;

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = result.NewRow();
                newRow["SoLoID"] = row.Table.Columns.Contains("SoLoID") && row["SoLoID"] != DBNull.Value
                    ? row["SoLoID"].ToString() : string.Empty;
                newRow["MaNPL"] = row.Table.Columns.Contains("MaNPL") && row["MaNPL"] != DBNull.Value
                    ? row["MaNPL"].ToString() : string.Empty;
                newRow["IsNPL"] = row.Table.Columns.Contains("IsNPL") && row["IsNPL"] != DBNull.Value
                    && Convert.ToInt32(row["IsNPL"]) == 1;
                result.Rows.Add(newRow);
            }

            return result;
        }
    }
}
