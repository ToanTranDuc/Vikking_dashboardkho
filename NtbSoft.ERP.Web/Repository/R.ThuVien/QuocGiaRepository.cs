using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IQuocGiaRepository
    {
        DataTable GetQuocGia();
        string Post(object ojQuocGia);
        string PostAutoQG(object ojQuocGia);
        string DeleteQuocGia(string parameter);
    }
    public class QuocGiaRepository : IQuocGiaRepository
    {
        QuocGiaModel quocgiamodel = new QuocGiaModel();
        public DataTable GetQuocGia()
        {
            var dt = quocgiamodel.GetQuocGia();
            return dt;
        }
        public string Post(object ojQuocGia)
        {
            var result = quocgiamodel.Post(ojQuocGia);
            return result;
        }
        public string PostAutoQG(object ojQuocGia)
        {
            var result = quocgiamodel.PostAutoQG(ojQuocGia);
            return result;
        }
        public string DeleteQuocGia(string parameter)
        {
            var result = quocgiamodel.DeleteQuocGia(parameter);
            return result;
        }
    }
}