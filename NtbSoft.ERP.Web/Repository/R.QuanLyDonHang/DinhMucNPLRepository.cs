using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{

    public interface IDinhMucNPLRepository
    {
        DataTable Get(string maDH);
        string Post(DataTable ojHangHoa);
        string Delete(int id);
    }
    public class DinhMucNPLRepository : IDinhMucNPLRepository
    {
        DinhMucNPLModel DMNPLModel = new DinhMucNPLModel();
        public DataTable Get(string maDH)
        {
            var dt = DMNPLModel.Get(maDH);
            return dt;
        }
        public string Post(DataTable ojHangHoa)
        {
            var result = DMNPLModel.Post(ojHangHoa);
            //var result = DMNPLModel.CheckVT(ojHangHoa);
            return result;
        }
        public string Delete(int id)
        {
            var result = DMNPLModel.Delete(id);
            return result;
        }
    }
}