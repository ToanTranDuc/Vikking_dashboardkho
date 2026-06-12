using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface INhanVienRepository
    {
        DataTable GetAllNV();
        DataTable GetNV(string manv);
        string PostNV(object ojNV);
        string DeleteNV(string manv);
    }
    public class NhanVienRepository : INhanVienRepository
    {
        NhanVienModel nvmodel = new NhanVienModel();

        public DataTable GetAllNV()
        {
            var dt = nvmodel.GetAllNV();
            return dt;
        }
        public DataTable GetNV(string manv)
        {
            var dt = nvmodel.GetNV(manv);
            return dt;
        }

        public string PostNV(object ojNV) 
        {
            var result = nvmodel.PostNV(ojNV);
            return result;
        }

        public string DeleteNV(string manv)
        {
            var result = nvmodel.DeleteNV(manv);
            return result;
        }

    }
}