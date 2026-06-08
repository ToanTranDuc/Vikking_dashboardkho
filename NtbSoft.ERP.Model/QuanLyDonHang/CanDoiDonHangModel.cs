using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class CanDoiDonHangModel
    {
        public DataTable GetDonHangTong(int pageIndex, int pageSize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", pageIndex);
                cmd.Parameters.AddWithValue("@parameter1", pageSize);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetDonHangTong_MaDH(string maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDH_MaDH");
                cmd.Parameters.AddWithValue("@MaDH", maDH);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetDonHangTongView(int pageIndex, int pageSize, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDHDV");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", pageIndex);
                cmd.Parameters.AddWithValue("@parameter1", pageSize);
                cmd.Parameters.AddWithValue("@parameter2", madvsx == null ? "0" : madvsx);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetDonHangTongViewLoc(int pageIndex, int pageSize, string madvsx, string tungay, string denngay, string mahang, string dot, string sl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_VIEWCHITIETLENHSX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDHDV");
                cmd.Parameters.AddWithValue("@parameter", pageIndex);
                cmd.Parameters.AddWithValue("@parameter1", pageSize);
                cmd.Parameters.AddWithValue("@parameter2", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter3", tungay == null ? "" : tungay);
                cmd.Parameters.AddWithValue("@parameter4", denngay == null ? "" : denngay);
                cmd.Parameters.AddWithValue("@parameter5", mahang == null ? "" : mahang);
                cmd.Parameters.AddWithValue("@parameter6", dot == null ? "" : dot);
                cmd.Parameters.AddWithValue("@parameter7", sl == null ? "" : sl);
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
        public DataTable Get()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetPO(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETPO");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetDauSize(string madh, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDAUSIZE");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetMau(string madh, string dausizeID)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAU");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", dausizeID == null ? "" : dausizeID);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetSizeID(string madh, string dausizeID, string mamau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZEID");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", dausizeID == null ? "" : dausizeID);
                cmd.Parameters.AddWithValue("@parameter2", mamau == null ? "" : mamau);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetListSize(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETLISTSIZE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetSoLuongDH(string madh, string dausizeID, string mamau, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSOLUONGDH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", dausizeID == null ? "" : dausizeID);
                cmd.Parameters.AddWithValue("@parameter2", mamau == null ? "" : mamau);
                cmd.Parameters.AddWithValue("@parameter3", poid == null ? "" : poid);
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
        public string GetMax()
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETMaxDH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                object data = cmd.ExecuteScalar();
                if (data == null || string.IsNullOrEmpty(data.ToString()))
                    return "0";
                else return data.ToString();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetNPL(string madh, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETNPL");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public DataTable GetGomNPL(string parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETGOMNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", parameter == null ? "" : parameter);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public string PostDVSX(object ojDVSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDVSX);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostIsKeoVe(object ojDVSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTISKEOVE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDVSX);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostNPL(object ojNPL)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojNPL);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetLenhSX(string madh, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETLENHSX");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetChiTietCanDoiNPL(string madh, string malenh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDINHMUCNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public DataTable GetChiTietCanDoiDVSX(string malenhsanxuat)//(string madh, string malenh, string malenhsanxuat, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCANDOISX");
                //cmd.Parameters.AddWithValue("@MaDH", 0);
                //cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                //cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                //cmd.Parameters.AddWithValue("@parameter2", malenhsanxuat == null ? "" : malenhsanxuat);
                //cmd.Parameters.AddWithValue("@parameter3", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@MaDH", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter1", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter2", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter3", DBNull.Value);
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

        public DataTable GetChiTietLenhSX(string madh, string madvsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandTimeout = 5000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCTLENHSX");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", madvsx == null ? "" : madvsx);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetCTTotal(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCTLENHSXTOTAL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetCTTotalDetail(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCTLENHTRU");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public string PostUpdateDVSX(object ojDVSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTUPDATE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@TypeTableEditCanDoi", ojDVSX);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostUDDVSX(object ojDVSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDVSX");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDVSX);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostUpdateNPL(object ojNPL)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTUPDATE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojNPL);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(string madh, string malenh, string malenhsanxuat, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter2", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter3", username ?? "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteDM(string madh, string malenh, string malenhsanxuat, string manpl, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEDM");
                cmd.Parameters.AddWithValue("@MaDH", username ?? "");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter2", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter3", manpl == null ? "" : manpl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteNPL(string madh, string malenhsanxuat, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter1", username == null ? "" : username);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetDSHangHoa()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSHangHoa");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetDSThongTinLenhSX(int pageIndex, int pageSize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSThongTinLenhSX");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", "0");
                cmd.Parameters.AddWithValue("@parameter1", "0");
                cmd.Parameters.AddWithValue("@parameter2", pageIndex);
                cmd.Parameters.AddWithValue("@parameter3", pageSize);
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
        public DataTable GetDSChiTietLenhSX(string id, int pageIndex, int pageSize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDSChiTietLenhSX");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", id);
                cmd.Parameters.AddWithValue("@parameter1", pageIndex);
                cmd.Parameters.AddWithValue("@parameter2", pageSize);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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
        public DataTable GetKTCanDoiNPL(string madh, string malenh, string poid, string mamau, string dausizeid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKTCANDOINPL");
                cmd.Parameters.AddWithValue("@MaDH", madh);
                cmd.Parameters.AddWithValue("@parameter", malenh);
                cmd.Parameters.AddWithValue("@parameter1", poid);
                cmd.Parameters.AddWithValue("@parameter2", mamau);
                cmd.Parameters.AddWithValue("@parameter3", dausizeid);
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
        public DataTable GetDsCanDoiDonHang(string maDH, string poID, string dauSizeID, string maMau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDsCanDoiDonHang");
                cmd.Parameters.AddWithValue("@MaDH", maDH);
                cmd.Parameters.AddWithValue("@parameter", poID);
                cmd.Parameters.AddWithValue("@parameter1", dauSizeID);
                cmd.Parameters.AddWithValue("@parameter2", maMau);
                cmd.Parameters.AddWithValue("@parameter3", "");
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
        public DataTable GetDsBaoCao(string maLenhSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDsBaoCao");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@Parameter", maLenhSX);
                cmd.Parameters.AddWithValue("@Parameter1", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
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
        public DataTable GetKiemTraDinhMuc(string madh, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRADINHMUC");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@Parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@Parameter1", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
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
        public string PostDinhMuc(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDinhMucImport(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLIMPORT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostGopDH(object ojGopDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GOPDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTGOPDH");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojGopDH);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetKiemTra(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GOPDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETKIEMTRA");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
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
        public DataTable GetGopID(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GOPDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETGOPID");
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
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

        public string DeleteItemCanDoi(DataTable _tblDeleteItemCanDoi)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteItemCanDoi");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@TypeTable", _tblDeleteItemCanDoi);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string SearchOrder(string maDVSX, string maLenh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                string query = "SELECT DISTINCT MaDH FROM CanDoiDonViSanXuat WHERE MaDVSX = @MaDVSX AND MaLenh = @MaLenh";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@MaDVSX", maDVSX);
                cmd.Parameters.AddWithValue("@MaLenh", maLenh);
                DataTable dataTable = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
                if (dataTable.Rows.Count > 0)
                    return dataTable.Rows[0]["MaDH"].ToString();
                else
                    return "NoData";
            }
            catch (SqlException ex)
            {
                return "false";
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDinhMucNhap(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetCapPhat(string madh, string _madh, string _malenhsx, string _magop)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_CAPPHAT");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", _madh == null ? "" : _madh);
                cmd.Parameters.AddWithValue("@parameter1", _malenhsx == null ? "" : _malenhsx);
                cmd.Parameters.AddWithValue("@parameter2", _magop == null ? "" : _magop);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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

        public DataTable GetVatTuNPL(string madh, string malenhsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_VATTUNPL");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", malenhsx == null ? "" : malenhsx);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public string DeleteNPLNhap(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", id);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetSoLuongNhap(string para, string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetSoLuong");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", para == null ? "" : para);
                cmd.Parameters.AddWithValue("@parameter1", para1 == null ? "" : para1);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetSoBookingNhap(string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetSoBooking");
                cmd.Parameters.AddWithValue("@MaDH", para == null ? "" : para);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetCDNPL(string madh, string malenh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetCDNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public DataTable GetTenVTGoiY()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "TenVTGoiY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetMauGoiY()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "MauGoiY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetKhoSizeGoiY()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "KhoSizeGoiY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetDVTinhGoiY()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DVTinhGoiY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetNhomGoiY()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "NhomGoiY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetSoLuongByMauDH(string para, string para1, string para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetSoLuongByMauDH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", para);
                cmd.Parameters.AddWithValue("@parameter1", para1);
                cmd.Parameters.AddWithValue("@parameter2", para2);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public DataTable GetVatTuCapThem(string madh, string malenhsx)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CanDoiDinhMucNPLChiTietCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_VATTUNPL");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", malenhsx == null ? "" : malenhsx);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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
        public string PostDinhMucNhapCapThem(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CanDoiDinhMucNPLChiTietCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetVatTuDotCapThem(string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CanDoiDinhMucNPLChiTietCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetDot");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@parameter", para == null ? "" : para);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
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

        public string DeleteAllCapThem(string madh, string malenhsanxuat, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CanDoiDinhMucNPLChiTietCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteAll");
                cmd.Parameters.AddWithValue("@MaDH", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter1", username == null ? "" : username);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteCapThem(string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CanDoiDinhMucNPLChiTietCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteByMaNPL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", para == null ? "" : para);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDot(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDOT");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteCapThemByDot(string para, string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CanDoiDinhMucNPLChiTietCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteByMaNPLDot");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", para == null ? "" : para);
                cmd.Parameters.AddWithValue("@parameter1", para1 == null ? "" : para1);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDinhMucNhapMoi(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST1");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDinhMucnpl(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPLNHAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST2");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@parameter4", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetChuyenChinh(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetChuyenChinh");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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

        #region xoa lenh sx
        public DataTable GetIsLenChuyen()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetIsLenChuyen");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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

        public string DeleteLenhSX(string madh, string malenh, string malenhsanxuat, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETELENHSX");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", malenh == null ? "" : malenh);
                cmd.Parameters.AddWithValue("@parameter2", malenhsanxuat == null ? "" : malenhsanxuat);
                cmd.Parameters.AddWithValue("@parameter3", username ?? "");
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


        public DataTable GetCheckEdiCanDoiSX(string madh, string malenh, string poid, string mamau, string dausizeid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetCheckEdiCanDoiSX");
                cmd.Parameters.AddWithValue("@MaDH", madh);
                cmd.Parameters.AddWithValue("@parameter", malenh);
                cmd.Parameters.AddWithValue("@parameter1", poid);
                cmd.Parameters.AddWithValue("@parameter2", mamau);
                cmd.Parameters.AddWithValue("@parameter3", dausizeid);
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
        public string PostDVSXV2(object ojDVSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTV2");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDVSX);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetIsLSX(string madh, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetIsLSX");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
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

        public string DeletePOcd(string madh, string poid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEPOCD");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh == null ? "" : madh);
                cmd.Parameters.AddWithValue("@parameter1", poid == null ? "" : poid);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetGomNPLKH(string parameter, string parameter1, string parameter2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETGOMNPLKH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", parameter == null ? "" : parameter);
                cmd.Parameters.AddWithValue("@parameter1", parameter1 == null ? "" : parameter1);
                cmd.Parameters.AddWithValue("@parameter2", parameter2 == null ? "" : parameter2);
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
        public string UpdateGhiChuQLSX(string maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOIDONHANG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UpdateGhiChuQLSX");
                cmd.Parameters.AddWithValue("@MaDH", maDH == null ? "" : maDH);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@parameter3", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetSize(string parameter, string action)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CANDOINPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", parameter??"");
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
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
