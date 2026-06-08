using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IHangHoaRepository
    {
        DataTable GetHangHoa();
        string Post(object ojHangHoa);
        string DeleteHangHoa(int parameter);
        DataTable GetHangHoaWithDVSX();

        DataTable GetHangHoaCheckKeoVe();
        DataTable GetKiemTraHH();
    }
    public class HangHoaRepository : IHangHoaRepository
    {
        HangHoaModel hanghoamodel = new HangHoaModel();
        public DataTable GetHangHoa()
        {
            var dt = hanghoamodel.GetHangHoa();
            return dt;
        }
        public string Post(object ojHangHoa)
        {
            var result = hanghoamodel.Post(ojHangHoa);
            return result;
        }
        public string DeleteHangHoa(int parameter)
        {
            var result = hanghoamodel.DeleteHangHoa(parameter);
            return result;
        }
        public DataTable GetHangHoaWithDVSX()
        {
            var dt = hanghoamodel.GetHangHoaWithDVSX();
            return dt;
        }

        public DataTable GetHangHoaCheckKeoVe()
        {
            var dt = hanghoamodel.GetHangHoaCheckKeoVe();
            return dt;
        }
        public DataTable GetKiemTraHH()
        {
            var dt = hanghoamodel.GetKiemTraHH();
            return dt;
        }

    }
}