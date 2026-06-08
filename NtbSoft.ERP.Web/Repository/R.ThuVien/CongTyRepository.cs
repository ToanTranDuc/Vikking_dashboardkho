using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface ICongTyRepository
    {
        DataTable Get();
        string Post(object ojCongTy); 
        string Delete(string parameter);
    }

    public class CongTyRepository : ICongTyRepository
    {
        CongTyModel congTyModel = new CongTyModel();
        public string Delete(string parameter)
        {
            var result = congTyModel.Delete(parameter);
            return result;
        }

        public DataTable Get()
        {
            var dt = congTyModel.Get();
            return dt;
        }

        public string Post(object ojCongTy)
        {
            var result = congTyModel.Post(ojCongTy);
            return result;
        }
    }
}