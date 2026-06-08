using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.Kho
{
    public static class DashboardKhoModel
    {
        private static DataTable ExecuteDashboardKho(string action, DateTime? tuNgay, DateTime? denNgay)
        {
            using (SqlConnection conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SP_DashboardKho", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 100).Value = action;

                var tuNgayParam = cmd.Parameters.Add("@TuNgay", SqlDbType.Date);
                tuNgayParam.Value = tuNgay.HasValue ? (object)tuNgay.Value.Date : DBNull.Value;

                var denNgayParam = cmd.Parameters.Add("@DenNgay", SqlDbType.Date);
                denNgayParam.Value = denNgay.HasValue ? (object)denNgay.Value.Date : DBNull.Value;

                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
        }

        public static DataTable GetChuanBiVe(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteDashboardKho("GET_DASHBOARD_KHO", tuNgay, denNgay);
        }

        public static DataTable GetWarehouseEfficiency()
        {
            return ExecuteDashboardKho("GET_WAREHOUSE_EFFICIENCY", null, null);
        }

        public static DataTable GetWarehouseCustomers()
        {
            return ExecuteDashboardKho("GET_WAREHOUSE_CUSTOMERS", null, null);
        }

        public static DataTable GetWarehouseEfficiencySummary()
        {
            return ExecuteDashboardKho("GET_WAREHOUSE_SUMMARY", null, null);
        }

        public static DataTable GetOverallCapacity()
        {
            return ExecuteDashboardKho("GET_OVERALL_CAPACITY", null, null);
        }

        public static DataTable GetChuanBiXuat(DateTime tuNgay, DateTime denNgay)
        {
            return ExecuteDashboardKho("GET_CHUAN_BI_XUAT", tuNgay, denNgay);
        }

        public static DataTable GetDangXuat(DateTime tuNgay, DateTime denNgay)
        {
            
            return ExecuteDashboardKho("GET_DANG_XUAT", tuNgay, denNgay);
        }
    
    }
}
