using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.POMH
{
    public class PheDuyetModel
    {
        public async Task<DataTable> Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            using (SqlConnection _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_POMH_PHEDUYET", _cnn))
                    {
                        cmd.CommandTimeout = 300000;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Para1", para1 == "NONE" ? DBNull.Value : (object)para1);
                        cmd.Parameters.AddWithValue("@Para2", para2 == "NONE" ? DBNull.Value : (object)para2);
                        cmd.Parameters.AddWithValue("@Para3", para3 == "NONE" ? DBNull.Value : (object)para3);
                        cmd.Parameters.AddWithValue("@Para4", para4 == "NONE" ? DBNull.Value : (object)para4);
                        cmd.Parameters.AddWithValue("@Para5", para5 == "NONE" ? DBNull.Value : (object)para5);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var tb = new DataTable();
                            tb.Load(reader);
                            return tb;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
                }
            }
        }
        public async Task<DataSet> GETDS(string action, string para1, string para2, string para3, string para4, string para5)
        {
            using (SqlConnection _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            {
                try
                {
                    if (_cnn.State != ConnectionState.Open)
                    {
                        await _cnn.OpenAsync();
                    }

                    using (SqlCommand cmd = new SqlCommand("SP_ERP_POMH_PHEDUYET", _cnn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Para1", para1 == "NONE" ? DBNull.Value : (object)para1);
                        cmd.Parameters.AddWithValue("@Para2", para2 == "NONE" ? DBNull.Value : (object)para2);
                        cmd.Parameters.AddWithValue("@Para3", para3 == "NONE" ? DBNull.Value : (object)para3);
                        cmd.Parameters.AddWithValue("@Para4", para4 == "NONE" ? DBNull.Value : (object)para4);
                        cmd.Parameters.AddWithValue("@Para5", para5 == "NONE" ? DBNull.Value : (object)para5);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            DataSet ds = new DataSet();
                            int tableIndex = 0;
                            

                            do
                            {
                                DataTable dt = new DataTable();
                                
                                dt.Load(reader);

                                // Chỉ thêm bảng nếu có dữ liệu
                                if (dt.Rows.Count > 0 || dt.Columns.Count > 0)
                                {
                                    ds.Tables.Add(dt);
                                    tableIndex++;
                                }
                            }
                            while (!reader.IsClosed); // reader.Load() tự động next result

                            return ds;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in GETDS_WithReader: {ex.Message}", ex);
                }
                finally
                {
                    if (_cnn != null && _cnn.State == ConnectionState.Open)
                    {
                        _cnn.Close();
                        _cnn.Dispose();
                    }
                }
            }
        }

        public async Task<string> Post(string Action,DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_PHEDUYET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public async Task<string> PostDanhGiaNCC(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_PHEDUYET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostDanhGiaNCC");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable_DanhGia", tbl);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}