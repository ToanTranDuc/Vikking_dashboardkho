using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    // Giao diện repository cho VatTuChiTiet
    public interface IVatTuChiTietRepository
    {
        DataTable Get(); // Lấy danh sách tất cả vật tư chi tiết
        DataTable GetByMaVT(string MaVT); // Lấy vật tư chi tiết theo mã vật tư
        string Post(DataTable tblVatTuChiTiet); // Thêm hoặc cập nhật vật tư chi tiết
        string Delete(int ID); // Xóa vật tư chi tiết theo ID
    }

    // Triển khai repository cho VatTuChiTiet
    public class VatTuChiTietRepository : IVatTuChiTietRepository
    {
        VatTuChiTietModel _model = new VatTuChiTietModel();

        // Lấy danh sách vật tư chi tiết
        public DataTable Get()
        {
            return _model.Get();
        }

        // Lấy vật tư chi tiết theo mã vật tư
        public DataTable GetByMaVT(string MaVT)
        {
            return _model.GetByMaVT(MaVT);
        }

        // Thêm hoặc cập nhật vật tư chi tiết
        public string Post(DataTable tblVatTuChiTiet)
        {
            return _model.Post(tblVatTuChiTiet);
        }

        // Xóa vật tư chi tiết theo ID
        public string Delete(int ID)
        {
            return _model.Delete(ID);
        }
    }

}