using System;
using NtbSoft.ERP.Model.Kho;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface IPhuLieuRepository
    {
        DataTable GetPL();
        string PostPL(DataTable tblPhuLieu);
        string DeletePL(int ID);
    }
    public class PhuLieuRepository:IPhuLieuRepository
    {
        PhuLieuModel _model = new PhuLieuModel();

        public DataTable GetPL()
        {
            return _model.Get();
        }
        public string PostPL(DataTable tblPhuLieu)
        {
            return _model.Post(tblPhuLieu);
        }
        public string DeletePL(int ID)
        {
            return _model.Delete(ID);
        }

    }
}