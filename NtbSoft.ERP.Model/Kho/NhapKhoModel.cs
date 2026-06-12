using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.Kho
{
    public class NhapKhoModel
    {
        public DataTable GetDS()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDS");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTin(int lcsx, string pageIndex, string pageSize, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTIN");
                cmd.Parameters.AddWithValue("@parameter", pageIndex);
                cmd.Parameters.AddWithValue("@parameter1", pageSize);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTinSMS(int lcsx, string pageIndex, string pageSize, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTIN");
                cmd.Parameters.AddWithValue("@parameter", pageIndex);
                cmd.Parameters.AddWithValue("@parameter1", pageSize);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTinPO(string madh, int lcsx, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINPO");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTinPOSMS(string madh, int lcsx, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINPO");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetMaKho(string madvsx, int lcsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKHO");
                cmd.Parameters.AddWithValue("@parameter", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINPOCT");
                cmd.Parameters.AddWithValue("@parameter", mapkl == null ? "" : mapkl);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter5", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter6", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINPOCT");
                cmd.Parameters.AddWithValue("@parameter", mapkl == null ? "" : mapkl);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter5", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter6", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
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
        public DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINCHITIET");
                cmd.Parameters.AddWithValue("@parameter", mapkl ?? "");
                cmd.Parameters.AddWithValue("@parameter1", madvsx ?? "");
                cmd.Parameters.AddWithValue("@parameter2", malenh ?? "");
                cmd.Parameters.AddWithValue("@parameter3", tuthung);
                cmd.Parameters.AddWithValue("@parameter4", poid ?? "");
                cmd.Parameters.AddWithValue("@parameter5", dausizeid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", mau ?? "");
                cmd.Parameters.AddWithValue("@parameter7", denthung);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", minsttthung);
                cmd.Parameters.AddWithValue("@parameter10", maxsttthung);
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
        public DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINCHITIET");
                cmd.Parameters.AddWithValue("@parameter", mapkl);
                cmd.Parameters.AddWithValue("@parameter1", madvsx);
                cmd.Parameters.AddWithValue("@parameter2", malenh ?? "");
                cmd.Parameters.AddWithValue("@parameter3", tuthung);
                cmd.Parameters.AddWithValue("@parameter4", poid);
                cmd.Parameters.AddWithValue("@parameter5", dausizeid ?? "");
                cmd.Parameters.AddWithValue("@parameter6", mau);
                cmd.Parameters.AddWithValue("@parameter7", denthung);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", minsttthung);
                cmd.Parameters.AddWithValue("@parameter10", maxsttthung);
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
        public string Post(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostALL(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTALL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter5", po ?? "");
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostALLSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTALL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter5", po ?? "");
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDT(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDT");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDTSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDT");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostHuy(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHUY");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostHuySMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHUY");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostChiTiet(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int _isTonKho)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCHITIET");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", _isTonKho);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int _isTonKho)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCHITIET");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", _isTonKho);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostViTri(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVITRI");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostViTriSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAMWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVITRI");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter8", lcsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetDay(string madvsx, string makho, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDAY");
                cmd.Parameters.AddWithValue("@parameter", madvsx ?? "");
                cmd.Parameters.AddWithValue("@parameter1", makho ?? "");
                cmd.Parameters.AddWithValue("@parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
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
        public DataTable GetKe(string madvsx, string makho, string maday)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKE");
                cmd.Parameters.AddWithValue("@parameter", madvsx ?? "");
                cmd.Parameters.AddWithValue("@parameter1", makho ?? "");
                cmd.Parameters.AddWithValue("@parameter2", maday ?? "");
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
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
        public DataTable GetTang(string madvsx, string makho, string maday, string make)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTANG");
                cmd.Parameters.AddWithValue("@parameter", madvsx ?? "");
                cmd.Parameters.AddWithValue("@parameter1", makho ?? "");
                cmd.Parameters.AddWithValue("@parameter2", maday ?? "");
                cmd.Parameters.AddWithValue("@parameter3", make ?? "");
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
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
        public DataTable GetO(string madvsx, string makho, string maday, string make, string matang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETO");
                cmd.Parameters.AddWithValue("@parameter", madvsx ?? "");
                cmd.Parameters.AddWithValue("@parameter1", makho ?? "");
                cmd.Parameters.AddWithValue("@parameter2", maday ?? "");
                cmd.Parameters.AddWithValue("@parameter3", make ?? "");
                cmd.Parameters.AddWithValue("@parameter4", matang ?? "");
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
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
        public DataTable GetCBM(string madvsx, string makho, string maday, string make, string matang, string mao, string madh, string mapkl, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCBM");
                cmd.Parameters.AddWithValue("@parameter", madvsx ?? "");
                cmd.Parameters.AddWithValue("@parameter1", makho ?? "");
                cmd.Parameters.AddWithValue("@parameter2", maday ?? "");
                cmd.Parameters.AddWithValue("@parameter3", make ?? "");
                cmd.Parameters.AddWithValue("@parameter4", matang ?? "");
                cmd.Parameters.AddWithValue("@parameter5", mao ?? "");
                cmd.Parameters.AddWithValue("@parameter6", madh ?? "");
                cmd.Parameters.AddWithValue("@parameter7", mapkl ?? "");
                cmd.Parameters.AddWithValue("@parameter8", poid ?? "");
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
        public DataTable GetVTK(DataTable data)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVTK");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@TypeTable", data);
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
        public DataTable GetVTKV1(DataTable data)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVTKV1");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@TypeTable", data);
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
        public string PostVTK(DataTable tblNhapKho)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoTong", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Post");
                cmd.Parameters.AddWithValue("@Para1", 0);
                cmd.Parameters.AddWithValue("@Para2", 0);
                cmd.Parameters.AddWithValue("@Para3", 0);
                cmd.Parameters.AddWithValue("@Para4", 0);
                cmd.Parameters.AddWithValue("@Para5", 0);
                cmd.Parameters.AddWithValue("@Para6", 0);
                cmd.Parameters.AddWithValue("@Para7", 0);
                cmd.Parameters.AddWithValue("@Para8", 0);
                cmd.Parameters.AddWithValue("@Para9", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblNhapKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetVTKV2(DataTable data)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VITRIKHODAY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETVTKV2");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@TypeTable", data);
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
        //coding by Dat
        //start
        public DataTable GetLenhSX(string statuschuyen, string username, string status)
        {
            if (statuschuyen == null || statuschuyen == "0")
            {
                statuschuyen = "";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetLenhSX");
                cmd.Parameters.AddWithValue("@parameter", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter1", username);
                cmd.Parameters.AddWithValue("@parameter2", status);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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

        public DataTable GetLenhSXLuuChuyen(string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetLenhSXLuuChuyen");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", username);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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

        public DataTable GetDS_KHDT(string maplk, string mahang, string poid, string statuschuyen, string madvsx, string isDecat, string dotsx)
        {
            if (statuschuyen == null)
            {
                statuschuyen = "";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDS_KHDT");
                cmd.Parameters.AddWithValue("@parameter", maplk);
                cmd.Parameters.AddWithValue("@parameter1", mahang);
                cmd.Parameters.AddWithValue("@parameter2", poid);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter5", madvsx);
                cmd.Parameters.AddWithValue("@parameter6", isDecat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", dotsx ?? "");
                cmd.Parameters.AddWithValue("@parameter9", 0);
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

        public DataTable GetDS_KHDTLuanChuyen(string maplk, string mahang, string poid, string mapklchuyen, string madvsx, string isDecat, string dotsx)
        {
            if (dotsx == null)
            {
                dotsx = "";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDS_KHDTLuuChuyen");
                cmd.Parameters.AddWithValue("@parameter", maplk);
                cmd.Parameters.AddWithValue("@parameter1", mahang);
                cmd.Parameters.AddWithValue("@parameter2", poid);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", mapklchuyen ?? "");
                cmd.Parameters.AddWithValue("@parameter5", madvsx);
                cmd.Parameters.AddWithValue("@parameter6", isDecat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", dotsx);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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

        public DataTable GetDSThung(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDecat, string dotsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSThung");
                cmd.Parameters.AddWithValue("@parameter", maplk);
                cmd.Parameters.AddWithValue("@parameter1", poid);
                cmd.Parameters.AddWithValue("@parameter2", mahang);
                cmd.Parameters.AddWithValue("@parameter3", tuthung);
                cmd.Parameters.AddWithValue("@parameter4", madvsx);
                cmd.Parameters.AddWithValue("@parameter5", denthung);
                cmd.Parameters.AddWithValue("@parameter6", isDecat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", dotsx ?? "");
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public DataTable GetDSThungV2(string maplk, string poid, string mahang, string tuthung, string isDongThung, string denthung, string isDeCat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSThung");
                cmd.Parameters.AddWithValue("@parameter", maplk);
                cmd.Parameters.AddWithValue("@parameter1", poid);
                cmd.Parameters.AddWithValue("@parameter2", mahang);
                cmd.Parameters.AddWithValue("@parameter3", tuthung);
                cmd.Parameters.AddWithValue("@parameter4", isDongThung);
                cmd.Parameters.AddWithValue("@parameter5", denthung);
                cmd.Parameters.AddWithValue("@parameter6", isDeCat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", "");
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public DataTable GetDSThungLuanChuyen(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDecat, string dotsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSThungLuanChuyen");
                cmd.Parameters.AddWithValue("@parameter", maplk);
                cmd.Parameters.AddWithValue("@parameter1", poid);
                cmd.Parameters.AddWithValue("@parameter2", mahang);
                cmd.Parameters.AddWithValue("@parameter3", tuthung);
                cmd.Parameters.AddWithValue("@parameter4", madvsx);
                cmd.Parameters.AddWithValue("@parameter5", denthung);
                cmd.Parameters.AddWithValue("@parameter6", isDecat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", dotsx ?? "");
                cmd.Parameters.AddWithValue("@parameter9", 0);
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

        public string PostNhapKho(DataTable tblDongThung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST1");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", tblDongThung);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostNhapKhoLuuChuyen(string isTonKho, DataTable tblDongThung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST_LuuChuyen");
                cmd.Parameters.AddWithValue("@parameter", isTonKho);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", tblDongThung);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable Search(string malenh, string para)
        {
            if (para == null)
            {
                para = "";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "search");
                cmd.Parameters.AddWithValue("@parameter", malenh);
                cmd.Parameters.AddWithValue("@parameter1", para);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public DataTable GetKho(string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetKho");
                cmd.Parameters.AddWithValue("@parameter", madvsx);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public DataTable GetNhapKhoWeb(string action, string para, string para1, string para2, string para3 = "", string para4 = "", string para5 = "",
                                    string para6 = "", string para7 = "", string para8 = "", string para9 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();

                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandTimeout = 10;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@parameter", para);
                cmd.Parameters.AddWithValue("@parameter1", para1);
                cmd.Parameters.AddWithValue("@parameter2", para2);
                cmd.Parameters.AddWithValue("@parameter3", para3);
                cmd.Parameters.AddWithValue("@parameter4", para4);
                cmd.Parameters.AddWithValue("@parameter5", para5);
                cmd.Parameters.AddWithValue("@parameter6", para6 == "" ? "0" : para6);
                cmd.Parameters.AddWithValue("@parameter7", para7);
                cmd.Parameters.AddWithValue("@parameter8", para8);
                cmd.Parameters.AddWithValue("@parameter9", para9);
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
        //end
        public DataTable GetKiemTra(DataTable data)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KTNHAPKHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRA");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", data);
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
        public DataTable GetKiemTraAll(DataTable data)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KTNHAPKHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRAALL");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", data);
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
        public DataTable GetKiemTraViTri(DataTable data)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KTNHAPKHO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRAVITRI");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable", data);
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
        public string UpdateDongThungNhapKho(string action, DataTable tblDongThung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_NHAPKHOTHANHPHAM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", tblDongThung);
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
