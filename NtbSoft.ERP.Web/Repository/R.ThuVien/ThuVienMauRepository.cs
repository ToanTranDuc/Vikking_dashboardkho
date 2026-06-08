using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IThuVienMauRepository
    {
        DataTable Get();
        DataTable GetByMaKH(string MaKH);
        string Post(DataTable tblThuVienMau);
        string POSTEXCEL(DataTable tblThuVienMau);
        string Delete(int ID); 
    }
    public class ThuVienMauRepository: IThuVienMauRepository
    {
        ThuVienMauModel _model = new ThuVienMauModel();
        

        // Lấy danh sách thư viện mẫu
        public DataTable Get()
        {
            return _model.Get();
        }

        public DataTable GetByMaKH(string MaKH)
        {
            return _model.GetByMaKH(MaKH);
        }
        // Thêm hoặc cập nhật mẫu
        public string Post(DataTable tblThuVienMau)
        {
            return _model.Post(tblThuVienMau);
        }

        public string POSTEXCEL(DataTable tblThuVienMau)
        {
            return _model.POSTEXCEL(tblThuVienMau);
        }

        // Xóa mẫu theo ID
        public string Delete(int ID)
        {
            return _model.Delete(ID);
        }
    }
}