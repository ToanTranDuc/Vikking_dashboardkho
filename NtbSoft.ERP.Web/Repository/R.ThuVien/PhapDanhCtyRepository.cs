using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{

    public interface IPhapDanhCtyRepository
    {
        DataTable Get();
        string Post(object objSave);
        string Delete(string parameter);
    }
    public class PhapDanhCtyRepository : IPhapDanhCtyRepository
    {
        PhapDanhCtyModel _model = new PhapDanhCtyModel();
        public string Delete(string parameter)
        {
            return _model.Delete(parameter);
        }

        public DataTable Get()
        {
            return _model.Get();
        }

        public string Post(object objSave)
        {
            var result = _model.Post(objSave);
            return result;
        }
    }
}