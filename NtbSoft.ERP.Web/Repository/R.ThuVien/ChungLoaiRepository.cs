using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IChungLoaiRepository
    {
        DataTable GetChungLoai();
        string Post(object ojChungLoai);
        string DeleteChungLoai(int parameter);
    }
    public class ChungLoaiRepository : IChungLoaiRepository
    {
        ChungLoaiModel chungloaimodel = new ChungLoaiModel();
        public DataTable GetChungLoai()
        {
            var dt = chungloaimodel.GetChungLoai();
            return dt;
        }
        public string Post(object ojChungLoai)
        {
            var result = chungloaimodel.Post(ojChungLoai);
            return result;
        }
        public string DeleteChungLoai(int parameter)
        {
            var result = chungloaimodel.DeleteChungLoai(parameter);
            return result;
        }
    }
}