using NtbSoft.ERP.Model.SoTheoDoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.SoTheoDoi
{
    public interface ISoTheoDoiSealNoiBoRepository
    {
        DataTable Get();
        string Post(object ojSoTheoDoiSeal);
        string Delete(int parameter);
    }
    public class SoTheoDoiSealNoiBoRepository : ISoTheoDoiSealNoiBoRepository
    {
        SoTheoDoiSealNoiBoModel sotheodoisealmodel = new SoTheoDoiSealNoiBoModel();
        public DataTable Get()
        {
            var dt = sotheodoisealmodel.Get();
            return dt;
        }
        public string Post(object ojSoTheoDoiSeal)
        {
            var result = sotheodoisealmodel.Post(ojSoTheoDoiSeal);
            return result;
        }
        public string Delete(int parameter)
        {
            var result = sotheodoisealmodel.Delete(parameter);
            return result;
        }
    }
}