using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IDonViChungLoaiRepository
    {
        DataTable Get();
        string Post(object ojDVCL);
        string Delete(int parameter);
    }
    public class DonViChungLoaiRepository : IDonViChungLoaiRepository
    {
        DonViChungLoaiModel dvclmodel = new DonViChungLoaiModel();
        public DataTable Get()
        {
            var dt = dvclmodel.Get();
            return dt;
        }
        public string Post(object ojDVCL)
        {
            var result = dvclmodel.Post(ojDVCL);
            return result;
        }
        public string Delete(int parameter)
        {
            var result = dvclmodel.Delete(parameter);
            return result;
        }
    }
}