using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Model.SYSTEM;

namespace NtbSoft.ERP.Web.Repository.R.SYSTEM
{
    public interface ISystemUserXacNhanRepository
    {
        List<SystemUserXacNhanBomModel> Get(string userID);
        string Post(List<SystemUserXacNhanBomConfigViewModel> items);
        List<SystemUserXacNhanBomConfigViewModel> GetUser(string userID);
    }
    public class SystemUserXacNhanRepository: ISystemUserXacNhanRepository
    {
        SysUserXacNhanBOMModel _model;
        public SystemUserXacNhanRepository()
        {
            _model = new SysUserXacNhanBOMModel();
        }
        public List<SystemUserXacNhanBomModel> Get(string userID)
        {
            DataTable tb = _model.Get(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserXacNhanBomModel> convert = new Libs.clsConvert<SystemUserXacNhanBomModel>();
            return convert.ToList(tb);
        }
        public List<SystemUserXacNhanBomConfigViewModel> GetUser(string userID)
        {
            DataTable tb = _model.GetUser(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserXacNhanBomConfigViewModel> convert = new Libs.clsConvert<SystemUserXacNhanBomConfigViewModel>();
            return convert.ToList(tb);
        }

        public string Post(List<SystemUserXacNhanBomConfigViewModel> items)
        {
            NtbSoft.ERP.Libs.clsConvert<SystemUserXacNhanBomConfigViewModel> convert = new Libs.clsConvert<SystemUserXacNhanBomConfigViewModel>();
            return _model.Post(convert.ToDataTable(items));
        }
    }
}