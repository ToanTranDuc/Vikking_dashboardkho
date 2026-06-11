using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{
    public interface IDuyetRepository
    {
       string PostDuyet(DataTable ojDinhMuc);
       string PostHuy(DataTable ojDinhMuc);
        DataTable GetEx(string madh, string malenh);
    }
    public class DuyetRepository : IDuyetRepository
    {
        DuyetDonHangModel duyetmodel = new DuyetDonHangModel();

        public string PostDuyet(DataTable ojDinhMuc)
        {
            var result = duyetmodel.PostDuyet(ojDinhMuc);
            return result;
        }

        public string PostHuy(DataTable ojDinhMuc)
        {
            var result = duyetmodel.PostHuy(ojDinhMuc);
            return result;
        }
        public DataTable GetEx(string madh, string malenh)
        {
            var result = duyetmodel.GetEx(madh, malenh);
            return result;
        }

    }
}
