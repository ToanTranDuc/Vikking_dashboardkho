using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{
    public interface IDinhMucRepository
    {
        DataTable Get(int pageIndex, int pageSize,string parameter);
        DataTable GetKiemTraDinhMuc(string maDH);
        string PostDinhMuc(DataTable ojDinhMuc);
        string UpdateNPL(DataTable ojDinhMuc);
    }
    public class DinhMucRepository : IDinhMucRepository
    {
        DinhMucModel dinhmucmodel = new DinhMucModel();
        public DataTable Get(int pageIndex, int pageSize,string parameter)
        {
            var dt = dinhmucmodel.Get(pageIndex, pageSize,parameter);
            return dt;
        }
        public DataTable GetKiemTraDinhMuc(string donhang)
        {
            var dt = dinhmucmodel.GetKiemTraDinhMuc(donhang);
            return dt;
        }
        public string PostDinhMuc(DataTable ojDinhMuc)
        {
            var result = dinhmucmodel.PostDinhMuc(ojDinhMuc);
            return result;
        }
        public string UpdateNPL(DataTable ojDinhMuc)
        {
            var result = dinhmucmodel.UpdateNPL(ojDinhMuc);
            return result;
        }
    }
}