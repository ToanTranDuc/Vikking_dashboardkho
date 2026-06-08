using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtbSoft.ERP.Model.ThuVien
{
    public class ERP_VatTuModel
    {
        public DataTable GetKH()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKH");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMH");
                cmd.Parameters.AddWithValue("@parameter", makh ??"");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public DataTable GetNhom()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETNHOM");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDONVI");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKHOVAI");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@parameter", makh??"");
                cmd.Parameters.AddWithValue("@parameter1", mahang??"");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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

        public DataTable GetMauVT()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMauVT");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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

        public DataTable GetMauSP(string mavtid, string mauvtid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMauSP");
                cmd.Parameters.AddWithValue("@parameter", mavtid ??"");
                cmd.Parameters.AddWithValue("@parameter1", mauvtid ??"");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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

        public DataTable GetMau(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAU");
                cmd.Parameters.AddWithValue("@parameter", makh??"");
                cmd.Parameters.AddWithValue("@parameter1", mahang??"");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public DataTable GetBangMau(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETBANGMAU");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public DataTable GetPost(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETPOST");
                cmd.Parameters.AddWithValue("@parameter",  "");
                cmd.Parameters.AddWithValue("@parameter1",  "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public string PostMauVT(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUVT");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", tbl);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostMauVTCopy(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUVTCOPY");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", tbl);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
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
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUSP");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostMauSPCopy(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTMAUSPCOPY");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostThongSo(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTTHONGSO");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostThongSoCopy(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTTHONGSOCOPY");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string Delete(string id,string makh, string mahang,string mauvtid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@parameter", id ?? "");
                cmd.Parameters.AddWithValue("@parameter1", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter2", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter3", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteMau(string mavtid, string mauvtid, string mamau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETEMAU");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", mavtid??"");
                cmd.Parameters.AddWithValue("@parameter3", mauvtid ?? "");
                cmd.Parameters.AddWithValue("@parameter4", mamau ?? "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetTenCT()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTENCT");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public DataTable GetDoiNhom(string NPL, string manhom)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDOINHOM");
                cmd.Parameters.AddWithValue("@parameter", NPL ?? "");
                cmd.Parameters.AddWithValue("@parameter1", manhom ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public string PostUpdateNhom(DataTable tbl, string para = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATENHOM");
                cmd.Parameters.AddWithValue("@parameter", para);
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostT1(string action, string para, string para2, string para3, string para4, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandTimeout = 60000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", tbl);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostT2(string action, string para, string para2, string para3, string para4, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandTimeout = 60000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
              
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", tbl);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostT3(string action, string para, string para2, string para3, string para4, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandTimeout = 60000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
         
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", tbl);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostT4(string action, string para, string para2, string para3, string para4, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);

                cmd.CommandTimeout = 60000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.Parameters.AddWithValue("@TypeTable4", tbl);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetTimKiem()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETTIMKIEM");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
                cmd.Parameters.AddWithValue("@TypeTable3", null);
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
        public DataTable GetChung(string action, string para, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPVatTuBOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
              
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
