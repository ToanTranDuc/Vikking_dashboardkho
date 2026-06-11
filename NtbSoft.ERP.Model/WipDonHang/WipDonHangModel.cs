using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.WipDonHang
{
    public class WipDonHangModel
    {
        public static DataTable ExecStoredProcedure(string spName, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1000;

                if (parameters != null)
                {
                    foreach (var pr in parameters)
                    {
                        cmd.Parameters.Add(pr);
                    }
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
