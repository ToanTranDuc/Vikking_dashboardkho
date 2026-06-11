using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IBangMauRepository
    {
        DataTable GetBangMau();
        DataTable GetBangMauAllowMaHang(string maHang);
        string Post(object ojChungLoai);
        string PostAutoImport(object ojChungLoai);
        string DeleteBangMau(int parameter);
    }
    public class BangMauRepository : IBangMauRepository
    {
        BangMauModel bangmaumodel = new BangMauModel();
        public DataTable GetBangMau()
        {
            var dt = bangmaumodel.GetBangMau();
            return dt;
        }
        public DataTable GetBangMauAllowMaHang(string maHang)
        {
            var dt = bangmaumodel.GetBangMauAllowMaHang(maHang);
            return dt;
        }
        public string Post(object ojBangMau)
        {
            var result = bangmaumodel.Post(ojBangMau);
            return result;
        }

        public string PostAutoImport(object ojBangMau)
        {
            var result = bangmaumodel.PostAutoImport(ojBangMau);
            return result;
        }
        public string DeleteBangMau(int parameter)
        {
            var result = bangmaumodel.DeleteBangMau(parameter);
            return result;
        }

    }
}