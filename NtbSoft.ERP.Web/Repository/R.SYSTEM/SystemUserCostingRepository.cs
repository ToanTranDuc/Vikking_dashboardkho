using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Model.SYSTEM;

namespace NtbSoft.ERP.Web.Repository.R.SYSTEM
{
    public interface ISystemUserCostingRepository
    {
        List<SystemUserCostingEntity> Get(string userID);
        string Post(List<SystemUserCostingConfigViewEntity> items);
        List<SystemUserCostingConfigViewEntity> GetUser(string userID);
    }
    public class SystemUserCostingRepository : ISystemUserCostingRepository
    {
        SystemUserCostingModel _model;
        public SystemUserCostingRepository()
        {
            _model = new SystemUserCostingModel();
        }
        public List<SystemUserCostingEntity> Get(string userID)
        {
            DataTable tb = _model.Get(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserCostingEntity> convert = new Libs.clsConvert<SystemUserCostingEntity>();
            return convert.ToList(tb);
        }
        public List<SystemUserCostingConfigViewEntity> GetUser(string userID)
        {
            DataTable tb = _model.GetUser(userID);
            NtbSoft.ERP.Libs.clsConvert<SystemUserCostingConfigViewEntity> convert = new Libs.clsConvert<SystemUserCostingConfigViewEntity>();
            return convert.ToList(tb);
        }

        public string Post(List<SystemUserCostingConfigViewEntity> items)
        {
            NtbSoft.ERP.Libs.clsConvert<SystemUserCostingConfigViewEntity> convert = new Libs.clsConvert<SystemUserCostingConfigViewEntity>();
            return _model.Post(convert.ToDataTable(items));
        }
    }
}