using NtbSoft.ERP.Model.SoTheoDoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.SoTheoDoi
{
    public interface ISoTheoDoiContainerRepository
    {
        //DataTable GetDSSoTheoDoiContainer();
        string Post(object ojBangSize);
        string DeleteSoTheoDoiContainer(int parameter);
    }

    public class SoTheoDoiContainerRepository: ISoTheoDoiContainerRepository
    {

        SoTheoDoiContainerModel soTheoDoimodel = new SoTheoDoiContainerModel();
        //public DataTable GetDSSoTheoDoiContainer()
        //{
        //    var dt = soTheoDoimodel.GetDSSoTheoDoiContainer();
        //    return dt;
        //}
        public string Post(object ojBangSize)
        {
            var result = soTheoDoimodel.Post(ojBangSize);
            return result;
        }
        public string DeleteSoTheoDoiContainer(int parameter)
        {
            var result = soTheoDoimodel.DeleteDSSoTheoDoi(parameter);
            return result;
        }

    }
}