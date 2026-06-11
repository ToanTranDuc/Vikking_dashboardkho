using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using NtbSoft.ERP.Model.THIETBI;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Models.THIETBI;

namespace NtbSoft.ERP.Web.Repository.THIETBI
{
    public interface IKhoRepository
    {
        List<KhoViewModels> Get();
        DataTable GetPhieuDD(string matb);
        DataTable/*List<KhoDetaiViewModels>*/ GetDetail();
        DataTable GetNhomNPL();
        DataTable GetNhomNL();
        DataTable GetNhomPL();
        string PostNNPL(DataTable tblNNPL);
        string DeleteNNPL(int ID);
        //DataTable GetNL();
        DataTable DeleteMaTB(string makho);
        string Post(List<KhoViewModels> items);
        string Delete(string IdKho);
        DataTable GetPL();
        string PostPL(DataTable tblPhuLieu);
        string DeletePL(int ID);
    
    }
    public class KhoRepository : IKhoRepository
    {
        //KHO
        KhoModel _model;
        public KhoRepository()
        {
            _model = new KhoModel();
        }
        public List<KhoViewModels> Get()
        {
            DataTable tb = _model.Get();
            NtbSoft.ERP.Libs.clsConvert<KhoViewModels> convert = new Libs.clsConvert<KhoViewModels>();
            return convert.ToList(tb);
        }
        public DataTable GetPhieuDD(string matb)
        {
            var dt = _model.GetPhieuDD(matb);
            return dt;
        }
        public /*List<KhoDetaiViewModels>*/DataTable GetDetail()
        {
            var dt = _model.GetDetail();
            return dt;
        }
        public DataTable DeleteMaTB(string makho)
        {
            var dt = _model.DeleteMaTB(makho);
            return dt;
        }
        public string Post(List<KhoViewModels> items)
        {
            NtbSoft.ERP.Libs.clsConvert<KhoViewModels> convert = new Libs.clsConvert<KhoViewModels>();
            return _model.Post(convert.ToDataTable(items));
        }

        public string Delete(string IdKho)
        {
            return _model.Delete(IdKho);
        }
        public DataTable GetNhomNL()
        {
            return _model.GetNhomNL();
        }
        public DataTable GetNhomPL()
        {
            return _model.GetNhomPL();
        }

        //KHONPL
        public DataTable GetNhomNPL()
        {
            return _model.GetNhomNPL();
        }
        public string PostNNPL(DataTable tblNNPL)
        {
            return _model.PostNNPL(tblNNPL);
        }
        public string DeleteNNPL(int ID)
        {
            return _model.DeleteNNPL(ID);
        }

        public DataTable GetPL()
        {
            return _model.GetPL();
        }
        public string PostPL(DataTable tblPhuLieu)
        {
            return _model.PostPL(tblPhuLieu);
        }
        public string DeletePL(int ID)
        {
            return _model.DeletePL(ID);
        }
     
    }
}