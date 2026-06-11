using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using NtbSoft.ERP.Entity.ThuVien;
using System.Linq;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class VatTuGiaModel
    {
        // ===============================
        // CONNECTION
        // ===============================
        private SqlConnection GetConn()
        {
            return NtbSoft.ERP.Libs.SqlHelper.GetConnection();
        }

        // ===============================
        // GET ALL
        // ===============================
        public DataTable Get(string maNhom = null)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GET_BY_TENNHOM");
                cmd.Parameters.AddWithValue("@MaNhom", (object)maNhom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter", DBNull.Value);

                DataTable dtEmpty = CreateEmptyTable();
                SqlParameter tvp = cmd.Parameters.AddWithValue("@tbl", dtEmpty);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TYPE_VatTuGia";

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }


        // ===============================
        // GET BY TEN NHOM
        // ===============================
        public DataTable GetByTenNhom(string tenNhom)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GET");
                cmd.Parameters.AddWithValue("@MaNhom", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter", (object)tenNhom ?? DBNull.Value);

                DataTable dtEmpty = CreateEmptyTable();
                SqlParameter tvp = cmd.Parameters.AddWithValue("@tbl", dtEmpty);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TYPE_VatTuGia";

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }


        private string GetMauVTID(SqlConnection conn, string mau)
        {
            if (string.IsNullOrWhiteSpace(mau)) return null;

            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 MauVTID FROM ERP_MauVTTV WHERE LTRIM(RTRIM(MauVT)) = @mau", conn))
            {
                cmd.Parameters.AddWithValue("@mau", mau.Trim());
                var rs = cmd.ExecuteScalar();
                return rs?.ToString();
            }
        }

        private string GetKhoVaiID(SqlConnection conn, string kho)
        {
            if (string.IsNullOrWhiteSpace(kho)) return null;

            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 KhoVaiID FROM ERP_KhoVai WHERE LTRIM(RTRIM(KhoVai)) = @kho", conn))
            {
                cmd.Parameters.AddWithValue("@kho", kho.Trim());
                var rs = cmd.ExecuteScalar();
                return rs?.ToString();
            }
        }
        // ===============================
        // SAVE
        // ===============================
        public string Save(List<VatTuGiaDto> list)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    DataTable dt = CreateEmptyTable();

                    foreach (VatTuGiaDto item in list)
                    {




                        DateTime? from = item.NgayApDung;
                        DateTime? to = item.NgayKetThuc;

                    

                        if (from.HasValue && to.HasValue && from > to)
                            continue;

                        string tienTe = item.DonViTienTe;
                        if (item.DonGia.HasValue && string.IsNullOrWhiteSpace(tienTe))
                            tienTe = "VND";

                        if (!item.DonGia.HasValue)
                            tienTe = null;
                        
                        string mauID = item.MauVTID;
                        string khoID = item.KhoVaiID;

                        if (string.IsNullOrEmpty(mauID))
                            mauID = GetMauVTID(conn, item.MauVT);

                        if (string.IsNullOrEmpty(khoID))
                            khoID = GetKhoVaiID(conn, item.KhoVai);
                        DataRow row = dt.NewRow();
                        row["ID"] = item.ID;
                        row["MaNhom"] = (object)item.MaNhom ?? DBNull.Value;
                        row["MaVTID"] = (object)item.MaVTID ?? DBNull.Value;
                        row["MaVT"] = (object)item.MaVT ?? DBNull.Value;
                        row["ChiTiet"] = (object)item.ChiTiet ?? DBNull.Value;
                        row["MaDVVT"] = (object)item.MaDVVT ?? DBNull.Value;
                        row["TenDVVT"] = (object)item.TenDVVT ?? DBNull.Value;
                        row["MaCLVTID"] = (object)item.MaCLVTID ?? DBNull.Value;
                        row["MauVTID"] = (object)mauID ?? DBNull.Value;
                        row["KhoVaiID"] = (object)khoID ?? DBNull.Value;
                        row["MauVT"] = (object)item.MauVT ?? DBNull.Value;
                        row["KhoVai"] = (object)item.KhoVai ?? DBNull.Value;
                        row["NgayApDung"] = (object)from ?? DBNull.Value;
                        row["NgayKetThuc"] = (object)to ?? DBNull.Value;
                        row["DonGia"] = (object)item.DonGia ?? DBNull.Value;
                        row["DonViTienTe"] = (object)tienTe ?? DBNull.Value;
                        row["GhiChu"] = (object)item.GhiChu ?? DBNull.Value;
                        row["NguoiTao"] = (object)item.NguoiTao ?? DBNull.Value;


                     

                 

                        bool isDuplicate = dt.AsEnumerable().Any(r =>
                        {
                            bool isUpdate = item.ID > 0;

                            if (!isUpdate)
                            {
                                if (r["NgayApDung"] == DBNull.Value || r["DonGia"] == DBNull.Value)
                                    return false;
                            }

                            DateTime existingTime = Convert.ToDateTime(r["NgayApDung"]);
                            decimal existingPrice = Convert.ToDecimal(r["DonGia"]);

                            string existingMaVTID = r["MaVTID"]?.ToString()?.Trim();
                            string existingMauVTID = r["MauVTID"]?.ToString()?.Trim();
                            string existingKhoVaiID = r["KhoVaiID"]?.ToString()?.Trim();

                            bool sameKey =
                                string.Equals(existingMaVTID, item.MaVTID?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(existingMauVTID, mauID?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(existingKhoVaiID, khoID?.Trim(), StringComparison.OrdinalIgnoreCase);
                            bool sameDate =
                             from.HasValue &&
                             existingTime.Date == from.Value.Date;
                            bool samePrice =
                                item.DonGia.HasValue &&
                                existingPrice == item.DonGia.Value;

                            return sameKey && sameDate && samePrice;
                        });
                        bool isInsert = item.ID == 0;

                        if (isInsert && isDuplicate)
                        {
                            continue;
                        }
                   
                        bool isUpdateOnly =
                            item.ID > 0 &&
                            (
                                item.NgayKetThuc.HasValue ||
                                !string.IsNullOrWhiteSpace(item.DonViTienTe)
                            );

                        if (isUpdateOnly)
                        {
                            dt.Rows.Add(row);
                            continue;
                        }
                        dt.Rows.Add(row);

                    }
       
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "POST");
                    cmd.Parameters.AddWithValue("@MaNhom", DBNull.Value);
                    cmd.Parameters.AddWithValue("@parameter", DBNull.Value);

                    SqlParameter tvp = cmd.Parameters.AddWithValue("@tbl", dt);
                    tvp.SqlDbType = SqlDbType.Structured;
                    tvp.TypeName = "dbo.TYPE_VatTuGia";

                    cmd.ExecuteNonQuery();

                    return "True";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        // ===============================
        // GET TIEN TE
        // ===============================
        public DataTable GetTienTe()
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GET_TIENTE");
                cmd.Parameters.AddWithValue("@MaNhom", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter", DBNull.Value);

                DataTable dtEmpty = CreateEmptyTable();
                SqlParameter tvp = cmd.Parameters.AddWithValue("@tbl", dtEmpty);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TYPE_VatTuGia";

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }

        // ===============================
        // GET HISTORY
        // ===============================
        public DataTable GetHistory(string maVTID, string extra)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GET_HISTORY");
                cmd.Parameters.AddWithValue("@parameter", maVTID);
                cmd.Parameters.AddWithValue("@MaNhom", extra);

                DataTable dtEmpty = CreateEmptyTable();
                var tvp = cmd.Parameters.AddWithValue("@tbl", dtEmpty);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TYPE_VatTuGia";

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                return dt;
            }
        }

        public string PostFromPO(string maPhieuMH)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_FromPO", conn))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaPhieuMH", maPhieuMH);

                    cmd.ExecuteNonQuery();

                    return "True";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public DataTable GetVatTuByMaVT(string keyword)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GET_BY_MAVT");
                cmd.Parameters.AddWithValue("@parameter", keyword);
                cmd.Parameters.AddWithValue("@MaNhom", DBNull.Value);

                // 🔥 TVP bắt buộc
                DataTable dtEmpty = CreateEmptyTable();
                var tvp = cmd.Parameters.AddWithValue("@tbl", dtEmpty);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.TYPE_VatTuGia";

                DataTable dt = new DataTable();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }
 
        private DataTable CreateEmptyTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("ChiTiet", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("TenDVVT", typeof(string));
            dt.Columns.Add("MaCLVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MauVT", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("NgayApDung", typeof(DateTime));
            dt.Columns.Add("NgayKetThuc", typeof(DateTime));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("DonViTienTe", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));

            dt.Columns.Add("NguoiTao", typeof(string));

            return dt;
        }

        public string DeleteHistory(List<VatTuGiaDto> list)
        {
            using (SqlConnection conn = GetConn())
            using (SqlCommand cmd = new SqlCommand("SP_VatTuGia_Save", conn))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    DataTable dt = CreateEmptyTable();

                    foreach (VatTuGiaDto item in list)
                    {
                        DataRow row = dt.NewRow();
                        row["ID"] = item.ID;
                        dt.Rows.Add(row);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE_HISTORY");
                    cmd.Parameters.AddWithValue("@MaNhom", DBNull.Value);
                    cmd.Parameters.AddWithValue("@parameter", DBNull.Value);

                    SqlParameter tvp = cmd.Parameters.AddWithValue("@tbl", dt);
                    tvp.SqlDbType = SqlDbType.Structured;
                    tvp.TypeName = "dbo.TYPE_VatTuGia";

                    cmd.ExecuteNonQuery();

                    return "True";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}