using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.Kho
{
    public class DongThungModel
    {
        public DataTable GetDS()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
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
        public DataTable GetThongTin(string madh, string pageIndex, string pageSize, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTIN");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", pageIndex);
                cmd.Parameters.AddWithValue("@parameter2", pageSize);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", madvsx == null ? "" : madvsx);
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
        public DataTable GetThongTinSMS(string madh, string pageIndex, string pageSize, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTIN");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", pageIndex);
                cmd.Parameters.AddWithValue("@parameter2", pageSize);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", madvsx == null ? "" : madvsx);
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
        public DataTable GetThongTinPO(string madh, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
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
        public DataTable GetThongTinPOSMS(string madh, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
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
        public DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
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
        public DataTable GetThongTinPOCTB(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "KienDetail");
                cmd.Parameters.AddWithValue("@parameter", mapkl == null ? "" : mapkl);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter5", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter6", malenhsanxuat == null ? "" : malenhsanxuat);
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
        public DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
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
        public DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINCHITIET");
                cmd.Parameters.AddWithValue("@parameter", mapkl == null ? "" : mapkl);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter3", tuthung == 0 ? 0 : tuthung);
                cmd.Parameters.AddWithValue("@parameter4", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter5", dausizeid == null ? "" : dausizeid);
                cmd.Parameters.AddWithValue("@parameter6", mau == null ? "" : mau);
                cmd.Parameters.AddWithValue("@parameter7", denthung == 0 ? 0 : denthung);
                cmd.Parameters.AddWithValue("@parameter8", minsttthung == 0 ? 0 : minsttthung);
                cmd.Parameters.AddWithValue("@parameter9", maxsttthung == 0 ? 0 : maxsttthung);
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
        public DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTHONGTINCHITIET");
                cmd.Parameters.AddWithValue("@parameter", mapkl == null ? "" : mapkl);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter3", tuthung == 0 ? 0 : tuthung);
                cmd.Parameters.AddWithValue("@parameter4", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter5", dausizeid == null ? "" : dausizeid);
                cmd.Parameters.AddWithValue("@parameter6", mau == null ? "" : mau);
                cmd.Parameters.AddWithValue("@parameter7", denthung == 0 ? 0 : denthung);
                cmd.Parameters.AddWithValue("@parameter8", minsttthung == 0 ? 0 : minsttthung);
                cmd.Parameters.AddWithValue("@parameter9", maxsttthung == 0 ? 0 : maxsttthung);
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

        public string Post(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostSMS(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostDT(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDT");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostDTSMS(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDT");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostHuy(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHUY");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostHuySMS(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHUY");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostChiTiet(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCHITIET");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTCHITIET");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostALL(DataTable tblNhapKho, int value)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTALL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", value);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostALLSMS(DataTable tblNhapKho, int value)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTALL");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", value);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostViTri(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVITRI");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        public string PostViTriSMS(DataTable tblNhapKho, bool isdongthung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWINSMS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTVITRI");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isdongthung);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@parameter5", 0);
                cmd.Parameters.AddWithValue("@parameter6", 0);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", 0);
                cmd.Parameters.AddWithValue("@parameter9", 0);
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
        //coding by Dat
        //start
        public DataTable GetLenhSX(string statuschuyen, string username, string status)
        {
            if (statuschuyen == null)
            {
                statuschuyen = "";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNG", conn);
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

        public DataTable GetDS_KHDT(string maplk, string mahang, string po, string statuschuyen, string madvsx, string isDecat, string dotsx)
        {
            if (statuschuyen == null)
            {
                statuschuyen = "";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDS_KHDT");
                cmd.Parameters.AddWithValue("@parameter", maplk);
                cmd.Parameters.AddWithValue("@parameter1", mahang);
                cmd.Parameters.AddWithValue("@parameter2", po);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", statuschuyen);
                cmd.Parameters.AddWithValue("@parameter5", madvsx);
                cmd.Parameters.AddWithValue("@parameter6", isDecat);
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", dotsx);
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

        public DataTable GetDSThung(string maplk, string po, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSThung");
                cmd.Parameters.AddWithValue("@parameter", maplk ?? "");
                cmd.Parameters.AddWithValue("@parameter1", po ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter3", tuthung ?? "0");
                cmd.Parameters.AddWithValue("@parameter4", madvsx ?? "0");
                cmd.Parameters.AddWithValue("@parameter5", denthung ?? "0");
                cmd.Parameters.AddWithValue("@parameter6", isDeCat ?? "0");
                cmd.Parameters.AddWithValue("@parameter7", 0);
                cmd.Parameters.AddWithValue("@parameter8", dotsx ?? "");
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

        public string PostDongThung(DataTable tblDongThung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNG", conn);
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
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNG", conn);
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
        public DataTable GetDsTongQuanDongThung(string maHang, DateTime ngayDongThung, string maDVSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSTongQuanDongThung");
                cmd.Parameters.AddWithValue("@parameter", maHang !=null ?maHang:"");
                cmd.Parameters.AddWithValue("@parameter1", maDVSX !=null?maDVSX:"");
                cmd.Parameters.AddWithValue("@parameter2", ngayDongThung.ToString("yyyy-MM-dd"));
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
        public DataTable GetDsXuatNhapTon(string maHang, string maDVSX, int isChecked)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSXuatNhapTon");
                cmd.Parameters.AddWithValue("@parameter", maHang??"");
                cmd.Parameters.AddWithValue("@parameter1", maDVSX??"");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isChecked);
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

        public DataTable GetDsXuatNhapTonLuanChuyen(string maHang, string maDVSX, int isChecked)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSXuatNhapTonLuanChuyen");
                cmd.Parameters.AddWithValue("@parameter", maHang??"");
                cmd.Parameters.AddWithValue("@parameter1", maDVSX??"");
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", isChecked);
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

        // BaoCaoDongThung
        public DataTable GetChiTietDongThung(DataTable tblChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetChiTietDongThung");
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
                cmd.Parameters.AddWithValue("@TypeChiTietDongThung", tblChiTiet);
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
        public DataTable GetKiemTra(DataTable tblChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRA");
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
                cmd.Parameters.AddWithValue("@TypeTable", tblChiTiet);
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
        public DataTable GetKiemTraAll(DataTable tblChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRAALL");
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
                cmd.Parameters.AddWithValue("@TypeTable", tblChiTiet);
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
        public DataTable GetKiemTraViTri(DataTable tblChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNGWIN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRAVITRI");
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
                cmd.Parameters.AddWithValue("@TypeTable", tblChiTiet);
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

        // Mạnh
        public string UpdateDongThungNhapKho(string action,DataTable tblDongThung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONGTHUNG", conn);
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
