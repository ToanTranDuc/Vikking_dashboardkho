using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Model.SYSTEM;

namespace NtbSoft.ERP.Web.Repository.R.SYSTEM
{
    public interface ISystemUserPQYeuCauNPLRepository
    {
        List<SystemUserYeuCauNPLEntity> Get(string userID);
        string Post(List<SystemUserYeuCauNPLConfigViewEntity> items);
        List<SystemUserYeuCauNPLConfigViewEntity> GetUser(string userID);
    }
    public class SystemUserPQYeuCauNPLRepository : ISystemUserPQYeuCauNPLRepository
    {
        SysUserYeuCauNPLModel _model;
        public SystemUserPQYeuCauNPLRepository()
        {
            _model = new SysUserYeuCauNPLModel();
        }
        public List<SystemUserYeuCauNPLEntity> Get(string userID)
        {
            DataTable tb = _model.Get(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserYeuCauNPLEntity> convert = new Libs.clsConvert<SystemUserYeuCauNPLEntity>();
            return convert.ToList(tb);
        }
        public List<SystemUserYeuCauNPLConfigViewEntity> GetUser(string userID)
        {
            DataTable tb = _model.GetUser(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserYeuCauNPLConfigViewEntity> convert = new Libs.clsConvert<SystemUserYeuCauNPLConfigViewEntity>();
            return convert.ToList(tb);
        }

        public string Post(List<SystemUserYeuCauNPLConfigViewEntity> items)
        {
            NtbSoft.ERP.Libs.clsConvert<SystemUserYeuCauNPLConfigViewEntity> convert = new Libs.clsConvert<SystemUserYeuCauNPLConfigViewEntity>();
            return _model.Post(convert.ToDataTable(items));
        }
    }
}