using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IDonViSanXuatRepository
    {
        DataTable GetDonViSanXuat();
        DataTable GetDonViSanXuatDetail(string MaDVSX);
        string Post(object ojDonViSanXuat);
        string PostDonViSanXuatDetail(DataTable tbl);
        string DeleteDonViSanXuat(int parameter);

        DataTable GetKhoList();
        DataTable GetLookupDVSX();
        string DeleteKho(string maKho);
        string PostKho(DataTable tbl);
    }
    public class DonViSanXuatRepository : IDonViSanXuatRepository
    {
        DonViSanXuatModel donvisanxuatmodel = new DonViSanXuatModel();
        public DataTable GetDonViSanXuat()
        {
            var dt = donvisanxuatmodel.GetDonViSanXuat();
            return dt;
        }

        public DataTable GetDonViSanXuatDetail(string MaDVSX)
        {
            var dt = donvisanxuatmodel.GetDonViSanXuatDetail(MaDVSX);
            return dt;
        }

        public string Post(object ojDonViSanXuat)
        {
            var result = donvisanxuatmodel.Post(ojDonViSanXuat);
            return result;
        }
        public string PostDonViSanXuatDetail(DataTable tbl)
        {
            var result = donvisanxuatmodel.PostDonViSanXuatDetail(tbl);
            return result;
        }
        public string DeleteDonViSanXuat(int parameter)
        {
            var result = donvisanxuatmodel.DeleteDonViSanXuat(parameter);
            return result;
        }
        public DataTable GetKhoList()
        {
            return donvisanxuatmodel.GetKhoList();
            

        }
        public DataTable GetLookupDVSX()
        {
            return donvisanxuatmodel.GetLookupDVSX();
        }
        public string DeleteKho(string maKho)
        {
            return donvisanxuatmodel.DeleteKho(maKho);
        }
        public string PostKho(DataTable tbl)
        {
            return donvisanxuatmodel.PostKho(tbl);
        }
    }
}