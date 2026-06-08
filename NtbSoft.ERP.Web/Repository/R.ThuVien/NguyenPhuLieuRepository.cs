using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.ThuVien
{
    public interface INguyenPLRepository
    {
       
        string Post(DataTable tbl);
    }
    public class NguyenPLRepository : INguyenPLRepository
    {
        NguyenPLModel model = new NguyenPLModel();
       
        public string Post(DataTable tbl)
        {
            var result = model.Post(tbl);
            return result;
        }
       
    }
}