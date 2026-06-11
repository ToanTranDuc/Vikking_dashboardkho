using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class QTY_KiemDauChuyenModel
    {
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("SP_QTY_KiemDauChuyen_Setting", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);  
                cmd.Parameters.AddWithValue("@Para1", para1);
                cmd.Parameters.AddWithValue("@Para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", para3);
                cmd.Parameters.AddWithValue("@Para4", para4);

                var p1 = new SqlParameter("@TypeTable", SqlDbType.Structured);
                p1.TypeName = "TYPE_QTY_QC_KiemDauChuyen";
                p1.Value = EmptyMain();
                cmd.Parameters.Add(p1);

                var p2 = new SqlParameter("@TypeTableTS", SqlDbType.Structured);
                p2.TypeName = "TYPE_QTY_QC_KiemDauChuyen_ThongSo";
                p2.Value = EmptyTS();
                cmd.Parameters.Add(p2);

                var p3 = new SqlParameter("@TypeTableImage", SqlDbType.Structured);
                p3.TypeName = "TYPE_QTY_QC_ImageKiemDauChuyen";
                p3.Value = EmptyImage();
                cmd.Parameters.Add(p3);

                var p4 = new SqlParameter("@TypeTableDanhMuc", SqlDbType.Structured);
                p4.TypeName = "TYPE_QTY_QC_KiemDauChuyen_DanhMuc";
                p4.Value = EmptyDanhMuc();
                cmd.Parameters.Add(p4);

                var p5 = new SqlParameter("@TypeTableDetail", SqlDbType.Structured);
                p5.TypeName = "TYPE_QTY_QC_KiemDauChuyenDetails";
                p5.Value = EmptyDetail();
                cmd.Parameters.Add(p5);

                var p6 = new SqlParameter("@TypeTableNhomKhacPhuc", SqlDbType.Structured);
                p6.TypeName = "TYPE_QTY_QC_KiemDauChuyenNhomKhacPhuc";
                p6.Value = EmptyNhomKhacPhuc();
                cmd.Parameters.Add(p6);

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

        public string Post(string action, object dt)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("SP_QTY_KiemDauChuyen_Setting", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");

                var dtData = dt as DataTable;

                bool isMain = action == "Post" || action == "UpdateNVKT";
                bool isTS = action == "PostThongSo";
                bool isImage = action == "PostImage" || action == "DeleteImage";
                bool isDanhMuc = action == "PostDanhMuc" || action == "PostDanhMucV2" || action == "DeleteDanhMuc";
                bool isDetail = action == "PostDetail" || action == "PostDetailV2" || action == "DeleteDetail";
                bool isNhomKhacPhuc = action == "PostNhomKhacPhuc" || action == "DeleteNhomKhacPhuc"; 

                var p1 = new SqlParameter("@TypeTable", SqlDbType.Structured);
                p1.TypeName = "TYPE_QTY_QC_KiemDauChuyen";
                p1.Value = isMain ? dtData : EmptyMain();
                cmd.Parameters.Add(p1);

                var p2 = new SqlParameter("@TypeTableTS", SqlDbType.Structured);
                p2.TypeName = "TYPE_QTY_QC_KiemDauChuyen_ThongSo";
                p2.Value = isTS ? dtData : EmptyTS();
                cmd.Parameters.Add(p2);

                var p3 = new SqlParameter("@TypeTableImage", SqlDbType.Structured);
                p3.TypeName = "TYPE_QTY_QC_ImageKiemDauChuyen";
                p3.Value = isImage ? dtData : EmptyImage();
                cmd.Parameters.Add(p3);

                var p4 = new SqlParameter("@TypeTableDanhMuc", SqlDbType.Structured);
                p4.TypeName = "TYPE_QTY_QC_KiemDauChuyen_DanhMuc";
                p4.Value = isDanhMuc ? dtData : EmptyDanhMuc();
                cmd.Parameters.Add(p4);

                var p5 = new SqlParameter("@TypeTableDetail", SqlDbType.Structured);
                p5.TypeName = "TYPE_QTY_QC_KiemDauChuyenDetails";
                p5.Value = isDetail ? dtData : EmptyDetail();
                cmd.Parameters.Add(p5);

                var p6 = new SqlParameter("@TypeTableNhomKhacPhuc", SqlDbType.Structured);
                p6.TypeName = "TYPE_QTY_QC_KiemDauChuyenNhomKhacPhuc";
                p6.Value = isNhomKhacPhuc ? dtData : EmptyNhomKhacPhuc();
                cmd.Parameters.Add(p6);

                if(action == "PostDanhMucLib" || action == "DeleteDanhMucLib")
                {
                    cmd.Parameters.AddWithValue("@TypeTableDanhMucLib", dt);
                }
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        private DataTable EmptyMain()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Line", typeof(string));
            dt.Columns.Add("LenhSX", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("SoLuongKiem", typeof(int));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("CreateUser", typeof(string));
            dt.Columns.Add("CreateDate", typeof(DateTime)); 
            dt.Columns.Add("TechnicalStaff", typeof(string));
            dt.Columns.Add("QCInLine", typeof(string));
            dt.Columns.Add("LineManager", typeof(string));
            dt.Columns.Add("Guid", typeof(string));
            return dt;
        }

        private DataTable EmptyTS()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_ThongSo_MaHang", typeof(int));
            dt.Columns.Add("ID_Link", typeof(string));
            dt.Columns.Add("ThongSoDo", typeof(float));
            dt.Columns.Add("Actions", typeof(string));
            return dt;
        }

        private DataTable EmptyImage()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_Kiem", typeof(int));
            dt.Columns.Add("Image_Link", typeof(string));
            dt.Columns.Add("Image_Type", typeof(string));
            dt.Columns.Add("NoteImage", typeof(string));
            dt.Columns.Add("STT", typeof(string));
            return dt;
        }

        private DataTable EmptyDanhMuc()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_Link", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            return dt;
        }

        private DataTable EmptyDetail()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_Link", typeof(string));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("MoTa", typeof(string));
            dt.Columns.Add("MucDoLoi", typeof(int));
            dt.Columns.Add("BienPhap", typeof(string));
            dt.Columns.Add("TheoDoi", typeof(string));
            dt.Columns.Add("MaNhom", typeof(int));
            dt.Columns.Add("TenNhom", typeof(string));
            dt.Columns.Add("MaLoi", typeof(string));
            dt.Columns.Add("Sign", typeof(string));
            dt.Columns.Add("NgaySign", typeof(string));
            dt.Columns.Add("NguoiSign", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            return dt;
        }

        private DataTable EmptyNhomKhacPhuc()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaNKP", typeof(string));
            dt.Columns.Add("NhomKhacPhuc", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            return dt;
        }

        public string Delete(string action,string para)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("SP_QTY_KiemDauChuyen_Setting", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para);
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Post(string action, string para1,string para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("SP_QTY_KiemDauChuyen_Setting", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1);
                cmd.Parameters.AddWithValue("@Para2", para2);
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
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