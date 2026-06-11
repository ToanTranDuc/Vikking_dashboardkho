using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IMaContRepository 
    {

        DataTable Get(string Action);
        string Post(object objMaCont);
        string Delete(string parameter);

    }


    public class MaContRepository : IMaContRepository
    {
        MaContModel _model = new MaContModel();
        public string Delete(string parameter)
        {
            return _model.Delete(parameter);
        }

        public DataTable Get(string Action)
        {
            var dt = _model.Get(Action);
            return dt;
        }

        public string Post(object objMaCont)
        {
            var result = _model.Post(objMaCont);
            return result;
        }
    }
}