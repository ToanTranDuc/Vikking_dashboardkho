using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface ILoaiHangHoaRepository
    {
        DataTable Get();
        string Post(object ojLHH);
        string Delete(int parameter);
    }
    public class LoaiHangHoaRepository : ILoaiHangHoaRepository
    {
        LoaiHangHoaModel dvclmodel = new LoaiHangHoaModel();
        public DataTable Get()
        {
            var dt = dvclmodel.Get();
            return dt;
        }
        public string Post(object ojLHH)
        {
            var result = dvclmodel.Post(ojLHH);
            return result;
        }
        public string Delete(int parameter)
        {
            var result = dvclmodel.Delete(parameter);
            return result;
        }

    }
}