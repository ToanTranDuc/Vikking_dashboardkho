using NtbSoft.ERP.Model.SoTheoDoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.SoTheoDoi
{
    public interface ITheKhoRepository
    {
        DataTable GetDSTheKho();
        string Post(object ojBangSize);
        string Delete(int parameter);
    }
    public class TheKhoRepository: ITheKhoRepository
    {
        TheKhoModel theKho = new TheKhoModel();
        public DataTable GetDSTheKho()
        {
            var dt = theKho.GetDSTheKho();
            return dt;
        }
        public string Post(object ojBangSize)
        {
            var result = theKho.Post(ojBangSize);
            return result;
        }
        public string Delete(int parameter)
        {
            var result = theKho.Delete(parameter);
            return result;
        }
    }
}