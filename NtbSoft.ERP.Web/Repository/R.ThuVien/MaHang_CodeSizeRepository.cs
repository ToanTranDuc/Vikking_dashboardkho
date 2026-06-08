using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IMaHangRepository
    {
        DataTable Get(string Action,string Para1,string Para2);
        string Post(object objMaHang);
        string Delete(string Para1, string Para2,string Para3);
    }

    public class MaHang_CodeSizeRepository : IMaHangRepository
    {
        MaHang_CodeSizeModel _model = new MaHang_CodeSizeModel();
        public DataTable Get(string Action, string Para1, string Para2)
        {
            var result = _model.Get(Action,Para1,Para2);
            return result;
        }

        public string Delete(string Para1, string Para2, string Para3)
        {
            var result = _model.Delete(Para1,Para2,Para3);
            return result;
        }

        public string Post(object objMaHang)
        {
            var result = _model.Post(objMaHang);
            return result;
        }
    }
}