using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IBangSizeRepository
    {
        DataTable GetBangSize();
        string Post(object ojBangSize);
        string PostAutoImport(object ojBangSize);
        string DeleteBangSize(int parameter);
        DataTable GetBangSizeMH(string makh);
    }
    public class BangSizeRepository : IBangSizeRepository
    {
        BangSizeModel bangsizemodel = new BangSizeModel();
        public DataTable GetBangSize()
        {
            var dt = bangsizemodel.GetBangSize();
            return dt;
        }
        public string Post(object ojBangSize)
        {
            var result = bangsizemodel.Post(ojBangSize);
            return result;
        }

        public string PostAutoImport(object ojBangSize)
        {
            var result = bangsizemodel.PostAutoImport(ojBangSize);
            return result;
        }
        public string DeleteBangSize(int parameter)
        {
            var result = bangsizemodel.DeleteBangSize(parameter);
            return result;
        }
        public DataTable GetBangSizeMH(string makh)
        {
            var dt = bangsizemodel.GetBangSizeMH(makh);
            return dt;
        }
    }
}