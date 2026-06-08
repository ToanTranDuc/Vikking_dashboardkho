using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class DonHangTongModel
    {
        public DataTable GetDonHangTong(int pageIndex, int pageSize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", '0');
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
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
        public DataTable GetKT(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKT");
                cmd.Parameters.AddWithValue("@MaDH", madh);
                cmd.Parameters.AddWithValue("@Parameter", '0');
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetMaHangCT(string donhang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "DonHangChiTiet");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", donhang);
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetPODS(string donhang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDauSize");
                cmd.Parameters.AddWithValue("@Parameter", donhang);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string PostDonHangTong(object ojDonHangTong)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDONHANGTONG");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDonHangTong);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDonHangTongV1(object ojDonHangTong)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGV1", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDONHANGTONG");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDonHangTong);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostAutoImportBangSize(object ojBangSize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BANGSIZENEW", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTAUTOIMPORT");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojBangSize);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDonHangTongPO(object ojDonHangTongPO)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDONHANGTONGPO");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePO", ojDonHangTongPO);
                cmd.Parameters.AddWithValue("@TypeTableUser", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDonHangTongPOUser(object ojDonHangTongPO)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDONHANGTONGPOUSER");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePO", null);
                cmd.Parameters.AddWithValue("@TypeTableUser", ojDonHangTongPO);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetChiTietDonHangTongPO(string maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetPivotPOChiTiet");
                cmd.Parameters.AddWithValue("@MaDH", maDH);
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string PostDonHangTongPOChiTiet(object ojDonHangTongPOChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPOCHITIET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDONHANGTONGPOCHITIET");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePOChiTiet", ojDonHangTongPOChiTiet);
                cmd.Parameters.AddWithValue("@TypeTablePOChiTiet_USER", null);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostDonHangTongPOChiTietUser(object ojDonHangTongPOChiTiet)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPOCHITIET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDONHANGTONGPOCHITIETUSER");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePOChiTiet", null);
                cmd.Parameters.AddWithValue("@TypeTablePOChiTiet_USER", ojDonHangTongPOChiTiet);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string UpdateSLDonHangTongPOChiTiet(DataTable dataTable)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPOCHITIET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATESL");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePOChiTietSL", dataTable);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(string madonhang, string username)
        {
            SqlConnection _cnn = null;
            try
            {
                _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", _cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteDonhang");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", madonhang ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", username ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string DeleteNPL(string madonhang)
        {
            SqlConnection _cnn = null;
            try
            {
                _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", _cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteNPL");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", madonhang);
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetDonHangTongID()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDONHANGID");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetDonHangTongMaHang(string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDONHANGMAHANG");
                cmd.Parameters.AddWithValue("@Parameter", mahang);
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetDsBaoCao(string maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetDsBaoCao");
                cmd.Parameters.AddWithValue("@Parameter", '0');
                cmd.Parameters.AddWithValue("@MaDH", maDH != null ? maDH : "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string DeleteDonHangTongPOChiTiet(DataTable dataTable)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPOCHITIET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeletePOChiTiet");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePOChiTiet", dataTable);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteDonHangTongPO(DataTable dataTable)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteDonHangTongPO");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePO", dataTable);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string DeleteDonHangTong(DataTable dataTable)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DeleteDonHangTong");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTable", dataTable);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetPOCanDoi(DataTable dataTable)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetPOCanDoi");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTablePO", dataTable);
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
        public DataTable CheckCanDoi(string _maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "CheckCanDoi");
                cmd.Parameters.AddWithValue("@MaDH", _maDH ?? "");
                cmd.Parameters.AddWithValue("@Parameter", '0');
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string GetKiemTraDH(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "KIEMTRADH");
                cmd.Parameters.AddWithValue("@MaDH", madh);
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    if (ds.Rows.Count > 0)
                        return "true";
                    else
                        return "flase";
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable SearchDH(string maHang, string khachHang, string chungLoai, string dot, int soLuong, string malenh, string dvsx, string po)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 200;
                cmd.Parameters.AddWithValue("Action", "SearchDH");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@Parameter", maHang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", khachHang ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", chungLoai ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", dot ?? "");
                cmd.Parameters.AddWithValue("@PageIndex", soLuong);
                cmd.Parameters.AddWithValue("@PageSize", 0);
                cmd.Parameters.AddWithValue("@Parameter5", malenh ?? "");
                cmd.Parameters.AddWithValue("@Parameter6", dvsx ?? "");
                cmd.Parameters.AddWithValue("@Parameter7", po ?? "");
                cmd.Parameters.AddWithValue("@Parameter8", "");
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
        public DataTable GetBangSizeKT(string mahang, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang);
                cmd.Parameters.AddWithValue("@Parameter2", makh);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetKhachHang()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKHACHHANG");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", '0');
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetHangHoaKH(string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETHANGHOAKH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetSize(string mahang, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetMau(string mahang, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMAU");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetChungLoai(string mahang, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHUNGLOAI");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetChungLoai(string chungloai)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHUNGLOAI1");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", chungloai ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetDauSize(string mahang, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDAUSIZEEDIT");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetDauSizeV1(string mahang, string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDAUSIZEV1");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetSizeEdit(string mahang, string makh, string dausize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZEEDIT");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", dausize ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public DataTable GetSizeEditV1(string mahang, string makh, string dausize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETSIZEEDITV1");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", dausize ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        public string PostThuVienMau(DataTable tblThuVienMau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_THUVIENMAUSAVE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tblThuVienMau);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        #region Phú
        public DataTable GetCapPhatThongSo(string makh, string mahang, string madot, string magop, string malenh, string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatLSX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_TS");
                cmd.Parameters.AddWithValue("@Parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", madot ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", magop ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", malenh ?? "");
                cmd.Parameters.AddWithValue("@Parameter6", madh ?? "");
                cmd.Parameters.AddWithValue("@Parameter7", 0);
                cmd.Parameters.AddWithValue("@Parameter8", 0);
                cmd.Parameters.AddWithValue("@Parameter9", 0);
                cmd.Parameters.AddWithValue("@Parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
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
        public DataTable GetCapPhatThongSoChiTiet(string makh, string mahang, string mavtid, string mauid, string malsx, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatLSX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_CHITIET");

                cmd.Parameters.AddWithValue("@Parameter", mavtid ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", mauid ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", malsx ?? "");
                cmd.Parameters.AddWithValue("@Parameter6", madot ?? "");
                cmd.Parameters.AddWithValue("@Parameter7", 0);
                cmd.Parameters.AddWithValue("@Parameter8", 0);
                cmd.Parameters.AddWithValue("@Parameter9", 0);
                cmd.Parameters.AddWithValue("@Parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
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

        public DataTable GetCapPhatThongSoDot(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatLSX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_DOT");
                cmd.Parameters.AddWithValue("@Parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue("@Parameter7", 0);
                cmd.Parameters.AddWithValue("@Parameter8", 0);
                cmd.Parameters.AddWithValue("@Parameter9", 0);
                cmd.Parameters.AddWithValue("@Parameter10", 0);
                cmd.Parameters.AddWithValue("@TypeTable1", null);
                cmd.Parameters.AddWithValue("@TypeTable2", null);
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
        public DataTable GetChiTietDM(string makh, string mahang, string mavtID, string madot, string mauID, string tachmau, string manhom, string khovaiID, string macode)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatLSX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_CHITIETDM");
                cmd.Parameters.AddWithValue("@Parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", mavtID ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", madot ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", mauID ?? "");
                cmd.Parameters.AddWithValue("@Parameter6", tachmau ?? "");
                cmd.Parameters.AddWithValue("@Parameter7", manhom ?? "");
                cmd.Parameters.AddWithValue("@Parameter8", khovaiID ?? "");
                cmd.Parameters.AddWithValue("@Parameter9", macode ?? "");
                cmd.Parameters.AddWithValue("@Parameter10", 0);
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
        public DataTable GetChiTietLenhSX(string magop, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatLSX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHITIETLENHSX");
                cmd.Parameters.AddWithValue("@Parameter", magop ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue("@Parameter7", 0);
                cmd.Parameters.AddWithValue("@Parameter8", 0);
                cmd.Parameters.AddWithValue("@Parameter9", 0);
                cmd.Parameters.AddWithValue("@Parameter10", 0);
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
        public string PostCapPhatLSX(DataTable tblSave)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "POST");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
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
        public string PostCapThemLSX(DataTable tblSave)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPLCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "POSTCT");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
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
        public string PostCapThemLSXSua(DataTable tblSave)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPLCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "POSTCTSUA");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter1", "");
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
        public DataTable GetCapPhatCanDoi(string magop, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_CAPPHATCANDOI");
                cmd.Parameters.AddWithValue("@Parameter", magop ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
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
        public DataTable GetCapPhatCanDoiCT(string magop, string malenhsanxuat, string madot, string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPLCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_CAPPHATCANDOICT");
                cmd.Parameters.AddWithValue("@Parameter", magop ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
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
        public DataTable GetCapThemLichSu(string magop, string malenhsanxuat, string madot, string manpl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPLCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_LICHSU");
                cmd.Parameters.AddWithValue("@Parameter", magop ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", manpl ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
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
        public DataTable GetCapPhatDotCanDoi(string magop, string malenhsanxuat)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_DOTCANDOI");
                cmd.Parameters.AddWithValue("@parameter", magop ?? "");
                cmd.Parameters.AddWithValue("@parameter1", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
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
        public DataTable GetCapPhatCanDoi(string magop, string malenhsanxuat, string madot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET_CAPPHATCANDOI");
                cmd.Parameters.AddWithValue("@parameter", magop ?? "");
                cmd.Parameters.AddWithValue("@parameter1", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@parameter2", madot ?? "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
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
        #endregion

        public DataTable GetCD(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCD");
                cmd.Parameters.AddWithValue("@MaDH", madh);
                cmd.Parameters.AddWithValue("@Parameter", '0');
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string DeleteCapThem(string madh, string malenhsanxuat, string manpl, string madot, string dot)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_CapPhatCanDoiNPLCapThem", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "DELETE");
                cmd.Parameters.AddWithValue("@Parameter", madh ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", malenhsanxuat ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", manpl ?? ""); ;
                cmd.Parameters.AddWithValue("@Parameter3", madot ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", dot ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        #region suadonhangtong_GetSort
        public DataTable GetSort(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONGPOCHITIET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GetSort");
                cmd.Parameters.AddWithValue("@Parameter", madh ?? "");
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
        #endregion

        #region TanSuat
        public DataTable GetMaHangCount(string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETMHCOUNT");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", "");
                cmd.Parameters.AddWithValue("@PageSize", "");
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
        #endregion

        public string PostMau(DataTable tblmau)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BANGMAU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTBMDHT");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@MaHang", 0);
                cmd.Parameters.AddWithValue("@TypeTable2", tblmau);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostSize(DataTable tblsize)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BANGSIZE", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTBSDHT");
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@TypeTable2", tblsize);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }


        #region coppy mahang
        public DataTable GetDonHangTongCoppy(string makh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDHCOPPY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", makh);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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

        public DataTable GetChiTietDonHangTongPOCoppy(string maDH)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDHCHITIETCOPPY");
                cmd.Parameters.AddWithValue("@MaDH", maDH);
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        #endregion

        public DataTable GetAllDH()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETAllDH");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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

        public DataTable GetDeleteSize(string madh)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETDELETESIZE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", madh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable GetKTSize(string madh, string sizeid)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETCHECKSIZE");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter", madh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", sizeid ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string DeleteSize(string madh, string sizeid, string username)
        {
            SqlConnection _cnn = null;
            try
            {
                _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", _cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETESIZE");
                cmd.Parameters.AddWithValue("@Parameter", madh ?? "");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@Parameter2", sizeid ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", username ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public DataTable CheckDuyetLenh(string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "CheckDuyetLenh");
                cmd.Parameters.AddWithValue("@MaDH", "");
                cmd.Parameters.AddWithValue("@Parameter", para);
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string GhiLogSuaDH(string mahang, string makh, string madh, string username)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DONHANGTONG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GhiLogSuaDH");
                cmd.Parameters.AddWithValue("@Parameter", mahang ?? "");
                cmd.Parameters.AddWithValue("@MaDH", madh ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", makh ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", username ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", 0);
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
