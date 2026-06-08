using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface IGetTenNVRepository
    {
        DataTable GetTenNV();
    }
    public class GetTenNVRepository : IGetTenNVRepository
    {
        GetTenNVModel _model = new GetTenNVModel();

        public DataTable GetTenNV()
        {
            return _model.GetTenNV();
        }
    }
}