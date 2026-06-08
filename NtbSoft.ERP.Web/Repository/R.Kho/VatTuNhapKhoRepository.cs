using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NtbSoft.ERP.Model.Kho;
namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface IVatTuNhapKhoRepository
    {
        DataTable Get(string para2 = null);
        DataTable GetDonHang(string para2 = null);
        DataTable GetMaKho();
        DataTable GetViTri();
        DataTable GetTimKiem(string para2);
        string Post(DataTable tb); 
        string Delete(int id); 
        DataTable GetDotNhap(string para2 = null); 
        DataTable GetNgayNhap(string para2 = null);
        DataTable GetNguoiNhap(string para2 = null); 
    }
    public class VatTuNhapKhoRepository : IVatTuNhapKhoRepository
    {
       
        VatTuNhapKhoModel _model = new VatTuNhapKhoModel();

    
        public DataTable Get(string para2 = null)
        {
            return _model.Get(para2);
        }
        public DataTable GetMaKho()
        {
            return _model.GetMaKho();
        }
        public DataTable GetViTri()
        {
            return _model.GetViTri();
        }
        public DataTable GetDonHang(string para2 = null)
        {
            return _model.GetDonHang(para2);
        }
        public DataTable GetTimKiem(string para2)
        {
            return _model.GetTimKiem(para2);
        }
        public string Post(DataTable tb)
        {
            return _model.Post(tb);
        }

        public string Delete(int id)
        {
            return _model.Delete(id);
        }

        public DataTable GetDotNhap(string para2 = null)
        {
            return _model.GetDotNhap(para2);
        }

        public DataTable GetNgayNhap(string para2 = null)
        {
            return _model.GetNgayNhap(para2);
        }

        public DataTable GetNguoiNhap(string para2 = null)
        {
            return _model.GetNguoiNhap(para2);
        }
    }
}