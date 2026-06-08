using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class KhoiTaoDMModel
    {
        public DataTable GetKH()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKH");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMH");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public DataTable GetThongSoDot(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGSODOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public DataTable GetSttDot(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSTTDOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public DataTable GetSizeDMCT(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZEDMCT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public DataTable Get(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetKTBom(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKTBOM");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetDM(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDM");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetDMGN(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDMGN");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetEditDinhMuc(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetEditDinhMuc");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetSizeDinhMucChung(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot, int isnew=0)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZEDMCT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter5", khovaiid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madot ?? "");

                cmd.Parameters.AddWithValue("@parameter7", isnew);
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
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
        public DataTable GetCheckSize(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "CheckSave");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter5", khovaiid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
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
        public DataTable GetDot(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDOT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetSuaDM(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSUADM");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable KiemTraDuyet(string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "DUYETBOM");
                cmd.Parameters.AddWithValue("@parameter", username ?? "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable KiemTraHuy(string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "HUYBOM");
                cmd.Parameters.AddWithValue("@parameter", username ?? "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable KiemTraDot(string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "KIEMTRADOT");
                cmd.Parameters.AddWithValue("@parameter", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public string Post(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
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
        public DataTable GetDinhMucChiTiet(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPSIZESP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter5", khovaiid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public string PostDMChiTiet(string para, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPSIZESP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", para);
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
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
        public string PostSaveSize(string para, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPSIZESP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "SaveSizeSP");
                cmd.Parameters.AddWithValue("@parameter", para);
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
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
        public string PostDMChungNULL(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPSIZESP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTNULL");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
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
        public DataTable DeleteDM(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPSIZESP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "DELETE");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter5", khovaiid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable CheckCanDoi(string para, string para1, string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "CheckCanDoi");
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable CheckVTCreate(string para, string para1, string para2, string para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "CheckVTCreate");
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public DataTable GetMaDot(string makh, string mahang, string stt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetMaDot");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", stt ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public string PostCopy(string makh, string mahang, string madotcopy,string madot, string dot, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCOPY");
                cmd.Parameters.AddWithValue("@parameter", makh ??"");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madotcopy ?? "");
                cmd.Parameters.AddWithValue("@parameter3", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter4", dot ?? "");
                cmd.Parameters.AddWithValue("@parameter5", username ?? "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DuyetAll(string makh, string mahang, string isduyet, string username, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "DuyetAll");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", isduyet ?? "");
                cmd.Parameters.AddWithValue("@parameter3", username?? "");
                cmd.Parameters.AddWithValue("@parameter4", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostCopyVT(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot,string mamauid, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPSIZESP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCOPYVT");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mavtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter5", khovaiid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madot??"");
                cmd.Parameters.AddWithValue("@parameter7", mamauid??"");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
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
        public DataTable GETVATTUEXCEL(string makh, string mahang, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DinhMucBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVATTUEXCEL");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
                cmd.Parameters.AddWithValue("@parameter7", "");
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", "");
                cmd.Parameters.AddWithValue("@parameter10", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);

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
        public string PostNew(DataTable tblSave, string para= "", string para1="")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "POSTNEW");
                cmd.Parameters.AddWithValue("@Parameter", para??"");
                cmd.Parameters.AddWithValue("@Parameter1", para1??"");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue("@TypeTable", tblSave);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetKiemTra(DataTable tbl, string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETKIEMTRA");
                cmd.Parameters.AddWithValue("@Parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
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
        public DataTable GetDonVi()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KHOITAOBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETDONVI");
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue("@Parameter7", "");
                cmd.Parameters.AddWithValue("@Parameter8", "");
                cmd.Parameters.AddWithValue("@Parameter9", "");
                cmd.Parameters.AddWithValue("@Parameter10", "");
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
    }
}
