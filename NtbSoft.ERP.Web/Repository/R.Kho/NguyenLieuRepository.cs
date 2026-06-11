using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface INguyenLieuRepository
    {
        DataTable GetNL();
        string PostNL(DataTable tblNguyenLieu);
        string DeleteNL(int ID);
    }
    public class NguyenLieuRepository: INguyenLieuRepository
    {
        NguyenLieuModel _model = new NguyenLieuModel();

        public DataTable GetNL()
        {
            return _model.Get();
        }
        public string PostNL(DataTable tblNguyenLieu)
        {
            return _model.Post(tblNguyenLieu);
        }
        public string DeleteNL(int ID)
        {
            return _model.Delete(ID);
        }
    }
}