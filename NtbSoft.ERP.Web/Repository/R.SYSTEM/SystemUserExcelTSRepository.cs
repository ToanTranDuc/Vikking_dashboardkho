using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Model.SYSTEM;

namespace NtbSoft.ERP.Web.Repository.R.SYSTEM
{
    public interface ISystemUserExcelTSRepository
    {
        List<SystemUserExcelTSModel> Get(string userID);
        string Post(List<SystemUserExcelTSConfigViewModel> items);
        List<SystemUserExcelTSConfigViewModel> GetUser(string userID);
    }
    public class SystemUserExcelTSRepository : ISystemUserExcelTSRepository
    {
        SysUserExcelTSModel _model;
        public SystemUserExcelTSRepository()
        {
            _model = new SysUserExcelTSModel();
        }
        public List<SystemUserExcelTSModel> Get(string userID)
        {
            DataTable tb = _model.Get(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserExcelTSModel> convert = new Libs.clsConvert<SystemUserExcelTSModel>();
            return convert.ToList(tb);
        }
        public List<SystemUserExcelTSConfigViewModel> GetUser(string userID)
        {
            DataTable tb = _model.GetUser(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserExcelTSConfigViewModel> convert = new Libs.clsConvert<SystemUserExcelTSConfigViewModel>();
            return convert.ToList(tb);
        }

        public string Post(List<SystemUserExcelTSConfigViewModel> items)
        {
            NtbSoft.ERP.Libs.clsConvert<SystemUserExcelTSConfigViewModel> convert = new Libs.clsConvert<SystemUserExcelTSConfigViewModel>();
            return _model.Post(convert.ToDataTable(items));
        }
    }
}