using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface ICongDoanRepository
    {
        DataTable GetCongDoan();
        string Post(DataTable tblCongDoan);

        string Delete(int id);

        DataTable GetPivotCongDoan();
        string PostChitietCongDoan(DataTable tblChiTietCongDoan);
        DataTable GetCongDoanAllowCondition(string maDH, string maLenhSX);
    }
    public class CongDoanRepository : ICongDoanRepository
    {
        CongDoanModel model = new CongDoanModel();
        public DataTable GetCongDoan()
        {
            return model.GetCongDoan();
        }

        public string Post(DataTable tblCongDoan)
        {
            return model.Post(tblCongDoan);
        }

        public string Delete(int id)
        {
            return model.Delete(id);
        }
        public DataTable GetPivotCongDoan()
        {
            return model.GetPivotCongDoan();
        }
        public string PostChitietCongDoan(DataTable tblChiTietCongDoan)
        {
            return model.PostChiTietCongDoan(tblChiTietCongDoan);
        }
        public DataTable GetCongDoanAllowCondition(string maDH, string maLenhSX)
        {
            return model.GetCongDoanAllowCondition(maDH,maLenhSX);
        }
    }
}