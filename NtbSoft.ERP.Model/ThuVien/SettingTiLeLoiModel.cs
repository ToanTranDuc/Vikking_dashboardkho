using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class SettingTiLeLoiModel
    {
        private DataTable ExecuteProcedure(string action, string p1 = "", string p2 = "", string p3 = "", string p4 = "", string p5 = "", string p6 = "", string p7 = "", DataTable tvp = null)
        {
            try
            {
                using (SqlConnection conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX())
                using (SqlCommand cmd = new SqlCommand("SP_QTY_SettingTiLeLoi", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Para1", p1 ?? "");
                    cmd.Parameters.AddWithValue("@Para2", p2 ?? "");
                    cmd.Parameters.AddWithValue("@Para3", p3 ?? "");
                    cmd.Parameters.AddWithValue("@Para4", p4 ?? "");
                    cmd.Parameters.AddWithValue("@Para5", p5 ?? "");
                    cmd.Parameters.AddWithValue("@Para6", p6 ?? "0");
                    cmd.Parameters.AddWithValue("@Para7", p7 ?? "");

                    if (tvp != null && tvp.Rows.Count > 0)
                    {
                        SqlParameter tvpParam = cmd.Parameters.AddWithValue("@ListID", tvp);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "TypeSettingTiLeLoi_ListID"; 
                    }

                    using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GetLineX() => ExecuteProcedure("GetLineX");

        public DataTable GetLenh(string lineX) => ExecuteProcedure("GetLenh", p1: lineX);

        public DataTable GetSettingTiLeLoi(string line, string lenh, string maHang) =>
            ExecuteProcedure("GetSettingTiLeLoi", p1: line, p2: lenh, p3: maHang);

        public DataTable SaveSettingTiLeLoi(string line, string lenh, string maHang, string tiLeLoi, string ngayCaiDat, string id) =>
            ExecuteProcedure("SaveSettingTiLeLoi", p1: line, p2: lenh, p3: maHang, p4: tiLeLoi, p5: ngayCaiDat, p6: id);
        public DataTable DeleteSettingTiLeLoi(string id, string listIdsStr = "")
        {
            DataTable dtIds = new DataTable();
            dtIds.Columns.Add("ID", typeof(int));

            if (!string.IsNullOrEmpty(listIdsStr))
            {
                string[] arr = listIdsStr.Split(',');
                foreach (string item in arr)
                {
                    if (int.TryParse(item.Trim(), out int val))
                    {
                        dtIds.Rows.Add(val); 
                    }
                }
            }

            return ExecuteProcedure("DeleteSettingTiLeLoi", p6: id, tvp: dtIds);
        }
    }
}