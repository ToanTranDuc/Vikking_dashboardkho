using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class PhanTichBomModel
    {
        public DataTable GetKH()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKH");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);  
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetMH(string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMH");
                cmd.Parameters.AddWithValue("@parameter", makh??"");
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetSttDot()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSTTDOT");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        
        public DataTable GetMauSanPham(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAUSANPHAM");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", dot ?? "");
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetMauSanPhamNew(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAUSANPHAM");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", dot ?? "");
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetMauSanPhamNull(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAUSANPHAMNULL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetDauSize(string makh, string mahang, string mavtID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDAUSIZE");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetDauSizeNull(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDAUSIZENULL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetSize(string makh, string mahang, string dauszie, string mavtID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZE");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", dauszie ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetSizeNull(string makh, string mahang, string dauszie)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZENULL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", dauszie ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetVatTu(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVATTU");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetVatTuNull(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVATTUNULL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetKTVatTu(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKTVATTU");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetVatTuSP(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVATTUSP");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetMauVTNull(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAUVTNULL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetMauVT(string makh, string mahang, string mavtID, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAUVT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetMauVTNew(string makh, string mahang, string mavtID, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAUVT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetKhoVai(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKHOVAI");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetChiTietSize(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHITIETSIZE");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", dot ?? "");
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetChiTietSizeNew(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHITIETSIZE");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", dot ?? "");
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetChiTietVT(string makh, string mahang, string mavtID, string mauID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHITIETVT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public DataTable GetDot(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
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
        public string PostMauSP(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUSP");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostMauSPNull(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUSPNULL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostSizeSP(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTSIZESP");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostSizeSPNull(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTSIZESPNULL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostMauVT(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_VATTUSP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUVT");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostMauVTNull(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_VATTUSP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUVTNULL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(string makh, string mahang, string mavtID, string mauID, string dausize, string madot)
        {
            SqlConnection _cnn = null;
            try
            {
                _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", _cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", dausize ?? "");
                //cmd.Parameters.AddWithValue("@parameter5", madot ?? "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
            }
        }
        public string DeleteDot(string makh, string mahang, string mavtID, string mauID, string madot)
        {
            SqlConnection _cnn = null;
            try
            {
                _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", _cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEDOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", madot ?? "");
                //cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
            }
        }
        #region quan
        public DataTable GetKHDotDM()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKH");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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
        public DataTable GetMHDotDM(string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMH");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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
        public DataTable GetVatTuDotDM(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVATTU");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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

        public DataTable GetChiTietVTDotDM(string makh, string mahang, string mavtID, string mauID, string makhoID, string madot, string tachmau, string manhom, string macode)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHOITAOBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHITIETVT");
                cmd.Parameters.AddWithValue("@parameter1", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mauID ?? "");
                cmd.Parameters.AddWithValue("@parameter5", makhoID ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter7", tachmau ?? "");
                cmd.Parameters.AddWithValue("@parameter8", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter9", macode ?? "");
                cmd.Parameters.AddWithValue("@parameter10", "");
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
        public DataTable GetDotDM(string makh, string mahang, string mavtID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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
        public string PostDotDM(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVTSPDUYET");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetVTSPDotDM(string makh, string mahang, string mavtID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVTSP");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mavtID ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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
        public string PostDotDMHuy(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVTSPHUY");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostDotDMDUYETALL(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDUYETALL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDotDMHUYALL(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVTSPHUYALL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDM(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDM");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDMChung(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDMChung");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #endregion
        public DataTable GetExcel(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETEXCEL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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
        public DataTable GetExcel2(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DuyetDMCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETEXCEL2");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
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
        public string DeleteDotBOM(string makh, string mahang, string madot, string userID)
        {
            SqlConnection _cnn = null;
            try
            {
                _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PHANTICHBOM", _cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEDOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", userID??"");
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
            }
        }
        public string CopyBOM(string maKH, string maHang, string maKH_copy, string maHang_copy, string maDot_copy, string dot_copy)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CopyBOM", conn);
                cmd.CommandTimeout = 5000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "CopyBOM");
                cmd.Parameters.AddWithValue("@MaKH", maKH);
                cmd.Parameters.AddWithValue("@MaHang", maHang);
                cmd.Parameters.AddWithValue("@MaKH_Copy", maKH_copy);
                cmd.Parameters.AddWithValue("@MaHang_Copy", maHang_copy);
                cmd.Parameters.AddWithValue("@MaDot_Copy", maDot_copy);
                cmd.Parameters.AddWithValue("@DotCopy", dot_copy);
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
