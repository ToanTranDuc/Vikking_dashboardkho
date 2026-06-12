using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IDonViRepository
    {
        DataTable GetDonVi();
        string Post(object ojDonVi);
        string DeleteDonVi(int parameter);
    }
    public class DonViRepository : IDonViRepository
    {
        DonViModel donViModel = new DonViModel();
        public DataTable GetDonVi()
        {
            var dt = donViModel.GetDonVi();
            return dt;
        }
        public string Post(object ojDonVi)
        {
            var result = donViModel.Post(ojDonVi);
            return result;
        }
        public string DeleteDonVi(int parameter)
        {
            var result = donViModel.DeleteDonVi(parameter);
            return result;
        }
    }
}