using System;
using System.Collections.Generic;
using System.Data;
using NtbSoft.ERP.Entity.SYSTEM;
using System.Data.SqlClient;


namespace NtbSoft.ERP.Model.THIETBI
{
    public class KhoModel
    {
        public DataTable Get()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@MaTB", 0);
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
        public DataTable GetPhieuDD(string matb)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETPhieuDD");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@MaTB", string.IsNullOrEmpty(matb) ? "" : matb);
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
        public DataTable GetDetail()
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETDETAIL");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@MaTB", 0);
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
        public DataTable DeleteMaTB(string makho)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEMATB");
                cmd.Parameters.AddWithValue("@ID", string.IsNullOrEmpty(makho) ? "" : makho);
                cmd.Parameters.AddWithValue("@MaTB", 0);
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
        public string Post(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@MaTB", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tb);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(string id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@MaTB", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetNhomNPL()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET");
               
              
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
        public DataTable GetNhomNL()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetNhomNL");


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
        public DataTable GetNhomPL()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetNhomPL");


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
        //////KHONPL
        //public DataTable GetNhomNPL()
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
        //        SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Action", "GETNhomNPL");
        //        cmd.Parameters.AddWithValue("@ID", 0);
        //        cmd.Parameters.AddWithValue("@Parameter", 0);
        //        //cmd.Parameters.AddWithValue("@MaTB", 0);
        //        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
        //        {
        //            DataTable ds = new DataTable();
        //            adt.Fill(ds);
        //            return ds;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        //}
        public string PostNNPL(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostNhomNPL");
                cmd.Parameters.AddWithValue("@TypeTable1", tb);
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteNNPL(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteNhomNPL");
                cmd.Parameters.AddWithValue("@Parameter", id);
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetPL()
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetPL");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@Parameter", 0);

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
        public string PostPL(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostPL");
                cmd.Parameters.AddWithValue("@TypeTable3", tb);
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeletePL(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeletePL");
                cmd.Parameters.AddWithValue("@Parameter", id);
                cmd.Parameters.AddWithValue("@ID", 0);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #region QLKho Thoai
        public DataTable GetKhoList()
        {
            //List<KhoEntity> lstKho = new List<KhoEntity>();
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DANH_MUC_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Get");
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
        public DataTable GetLookupDVSX()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DANH_MUC_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetLookupDVSX");

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
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }


        public string DeleteKho(string maKho) 
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DANH_MUC_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Delete");
                cmd.Parameters.AddWithValue("@Parameter", maKho);

                cmd.ExecuteNonQuery();
                return "True"; 

            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xóa kho: {ex.Message}";
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public string InsertKho(string maKho, string tenKho, string parentMaKho, string tenDVSX, string ghiChu, int? statusVT, double? cbm)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DANH_MUC_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "Insert");
                cmd.Parameters.AddWithValue("@MaKho",(object)maKho?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TenKho", tenKho);
                cmd.Parameters.AddWithValue("@ParentMaKho", (object)parentMaKho ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TenDVSX", (object)tenDVSX ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ghichu", (object)ghiChu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status_VT", (object)statusVT ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CBM", (object)cbm ?? DBNull.Value);
                object result = cmd.ExecuteScalar();

                if (result != null && result.ToString() != "")
                    return result.ToString();
                else
                    return "True"; 
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return $"Lỗi không xác định khi thêm kho: {ex.Message}";
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public string UpdateKho(string maKho, string newTenKho, string newParrentMaKho,string newTenDVSX, string newGhiChu, int? statusVT, double? cbm)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DANH_MUC_KHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "Update");
                cmd.Parameters.AddWithValue("@MaKho", maKho);
                cmd.Parameters.AddWithValue("@TenKho", newTenKho);
                cmd.Parameters.AddWithValue("@ParentMaKho", (object)newParrentMaKho ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TenDVSX", (object)newTenDVSX ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ghichu", (object)newGhiChu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status_VT", (object)statusVT ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CBM", (object)cbm ?? DBNull.Value);

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return $"Lỗi không xác định khi cập nhật: {ex.Message}";
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        #endregion
    }
}

