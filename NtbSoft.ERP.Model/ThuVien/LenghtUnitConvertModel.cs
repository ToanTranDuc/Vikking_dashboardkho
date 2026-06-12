using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public static class LenghtUnitConvertModel
    {
        public static DataTable getHeQuyChieu()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand(string.Format(@"select unitID as maDV, TenDVCL as tenDV, simple as kiHieu, convert_to_meter as heQuyChieu_m
                                                        from LenghtUnitConvert luc
                                                        left join DonViChungLoai dvcl on luc.unitID = dvcl.MaDVCL"), conn);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex) { throw new Exception(ex.Message); }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

    }
}
