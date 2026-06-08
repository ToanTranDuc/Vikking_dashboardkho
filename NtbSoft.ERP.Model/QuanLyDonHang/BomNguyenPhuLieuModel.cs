using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class BomNguyenPhuLieuModel
    {
        //get
        public DataTable Get_TikKiem(string _mahang)
        {
            _mahang = _mahang == null ? "" : _mahang;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Get_TikKiem");
                cmd.Parameters.AddWithValue("@Parameter", _mahang);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_VatTu(string mavt)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_VatTu");
                cmd.Parameters.AddWithValue("@Parameter", mavt);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_TenVatTu()
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_TenVatTu");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GETMH()
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETMH");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_Mau(string _mahang)
        {
            _mahang = _mahang == null ? "" : _mahang;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_Mau");
                cmd.Parameters.AddWithValue("@Parameter", _mahang);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_InSeam(string _mahang)
        {
            _mahang = _mahang == null ? "" : _mahang;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_InSeam");
                cmd.Parameters.AddWithValue("@Parameter", _mahang);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_Size(string _mahang/*, string _inseam*/)
        {
            _mahang = _mahang == null ? "" : _mahang;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_Size");
                cmd.Parameters.AddWithValue("@Parameter", _mahang);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_MauVatTu(string _mavt/*, string _inseam*/)
        {
            _mavt = _mavt == null ? "" : _mavt;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_MauVatTu");
                cmd.Parameters.AddWithValue("@Parameter", _mavt);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_KhoSizeVatTu(string _mavt/*, string _inseam*/)
        {
            _mavt = _mavt == null ? "" : _mavt;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_KhoSizeVatTu");
                cmd.Parameters.AddWithValue("@Parameter", _mavt);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_KhoSizebyMau(string _mavt/*, string _inseam*/,string _mau)
        {
            _mavt = _mavt == null ? "" : _mavt;
            _mau = _mau == null ? "" : _mau;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetKhoSizebyMau");
                cmd.Parameters.AddWithValue("@Parameter", _mavt);
                cmd.Parameters.AddWithValue("@Parameter1", _mau);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_MaubyKhoSize(string _mavt/*, string _inseam*/, string _khosize)
        {
            _mavt = _mavt == null ? "" : _mavt;
            _khosize = _khosize == null ? "" : _khosize;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_MaubyKhoSize");
                cmd.Parameters.AddWithValue("@Parameter", _mavt);
                cmd.Parameters.AddWithValue("@Parameter1", _khosize);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        //POST
        public string PostBomVT(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTBOMVT");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        //DELETE
        public string DeleteBomVT(string _mahang, string _mavt)
        {
            _mavt = _mavt == null ? "" : _mavt;
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Parameter", _mahang);
                cmd.Parameters.AddWithValue("@Parameter1", _mavt);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        //public string DeleteBomDongVT(DataTable tb, string MaBom)
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
        //        SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Action", "DeleteBOMDong");
        //        cmd.Parameters.AddWithValue("@TypeTable", tb);
        //        cmd.Parameters.AddWithValue("@Parameter", MaBom);

        //        cmd.ExecuteNonQuery();
        //        return "True";
        //    }
        //    catch (SqlException ex)
        //    {
        //        return ex.Message;
        //    }
        //    finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        //}
        public string PostBomVTCopy(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTBOMVTCOPY");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public DataTable GET_DinhMucNPL(string _mahang)
        {
            _mahang = _mahang == null ? "" : _mahang;
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetDinhMucNPL");
                cmd.Parameters.AddWithValue("@Parameter", _mahang);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
        public string UpdateXacNhan(string action, string id, string value)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BOMVATTU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", id);
                cmd.Parameters.AddWithValue("@Parameter1", value);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
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
