using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class GetThuVienModel
    {
        public List<DataTable> GetThuVienDaDung()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GetLibUsed", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetThuVienDaDung");
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    List<DataTable> lstThuVien = new List<DataTable>();
                    DataSet ds = new DataSet();
                    adt.Fill(ds);

                    foreach (DataTable table in ds.Tables)
                    {
                        lstThuVien.Add(table);
                    }
                    return lstThuVien;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
