using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Model.ThuVien;
namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IThuVienKhoSizeRepository
    {
        DataTable Get();
        DataTable GetByMaVT(string MaVT);
        string Post(DataTable tblThuVienKhoSize);
        string Delete(int ID); 
    }
    public class ThuVienKhoSizeRepository: IThuVienKhoSizeRepository
    {
        ThuVienKhoSizeModel _model = new ThuVienKhoSizeModel();

        // Lấy danh sách thư viện kho size
        public DataTable Get()
        {
            return _model.Get();
        }
        public DataTable GetByMaVT(string MaVT)
        {
            return _model.GetByMaVT(MaVT);
        }

        // Thêm hoặc cập nhật kho size
        public string Post(DataTable tblThuVienKhoSize)
        {
            return _model.Post(tblThuVienKhoSize);
        }

        // Xóa kho size theo ID
        public string Delete(int ID)
        {
            return _model.Delete(ID);
        }
    }
}