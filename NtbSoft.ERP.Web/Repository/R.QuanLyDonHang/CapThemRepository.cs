using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtbSoft.ERP.Model.QuanLyDonHang;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{
    public interface ICapThemRepository
    {
        string PostDotDinhMuc(DataTable ojDinhMuc);
        DataTable GetLichSu(string madh, string malenh, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau);
        string DeleteDot(string madh, string malenhsanxuat, string dot, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau);
    }
    public class CapThemRepository : ICapThemRepository 
    {
        CapThemModel capThemModel = new CapThemModel();

        public DataTable GetLichSu(string madh, string malenh, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau) 
        {
            var result = capThemModel.GetLichSu(madh,malenh,manpl,mavt,mamau,dausize,size,mabom, mavtmau);
            return result;
        }
        public string PostDotDinhMuc(DataTable ojDinhMuc)
        {
            var result = capThemModel.PostDotDinhMuc(ojDinhMuc);
            return result;
        }
        public string DeleteDot(string madh, string malenhsanxuat, string dot, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau)
        {
            var result = capThemModel.DeleteDot(madh, malenhsanxuat, dot, manpl, mavt, mamau, dausize, size, mabom, mavtmau);
            return result;
        }
    }
}
