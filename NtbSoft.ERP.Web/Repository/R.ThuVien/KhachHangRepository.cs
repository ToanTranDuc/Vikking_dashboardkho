using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IKhachHangRepository
    {
        DataTable GetKhachHang();
        string Post(object ojKhachHang);
        string PostAutoKH(object ojKhachHang);
        string DeleteKhachHang(int parameter);
    }
    public class KhachHangRepository : IKhachHangRepository
    {
        KhachHangModel khachhangmodel = new KhachHangModel();
        public DataTable GetKhachHang()
        {
            var dt = khachhangmodel.GetKhachHang();
            return dt;
        }
        public string Post(object ojKhachHang)
        {
            var result = khachhangmodel.Post(ojKhachHang);
            return result;
        }
        public string PostAutoKH(object ojKhachHang)
        {
            var result = khachhangmodel.PostAutoKH(ojKhachHang);
            return result;
        }
        public string DeleteKhachHang(int parameter)
        {
            var result = khachhangmodel.DeleteKhachHang(parameter);
            return result;
        }
    }
}