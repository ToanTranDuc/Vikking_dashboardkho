using NtbSoft.ERP.Model.SYSTEM;
using NtbSoft.ERP.Web.Models.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Web.Repository.R.SYSTEM
{
    public interface ISystemUserDuyetRepository
    {
        List<SystemUserDuyetViewModel> Get(string userID);
        string Post(List<SystemUserDuyetConfigViewModel> items);
        List<SystemUserDuyetConfigViewModel> GetUser(string userID);
    }
    public class SystemUserDuyetRepository : ISystemUserDuyetRepository
    {
        SysUserDuyetModel _model;

        public SystemUserDuyetRepository()
        {
            _model = new SysUserDuyetModel();
        }
        public List<Models.SYSTEM.SystemUserDuyetViewModel> Get(string userID)
        {
            DataTable tb = _model.Get(userID);
            NtbSoft.ERP.Libs.clsConvert<Models.SYSTEM.SystemUserDuyetViewModel> convert = new Libs.clsConvert<Models.SYSTEM.SystemUserDuyetViewModel>();
            return convert.ToList(tb);
        }
        public List<Models.SYSTEM.SystemUserDuyetConfigViewModel> GetUser(string userID)
        {
            DataTable tb = _model.GetUser(userID);
            NtbSoft.ERP.Libs.clsConvert<Models.SYSTEM.SystemUserDuyetConfigViewModel> convert = new Libs.clsConvert<Models.SYSTEM.SystemUserDuyetConfigViewModel>();
            return convert.ToList(tb);
        }

        public string Post(List<Models.SYSTEM.SystemUserDuyetConfigViewModel> items)
        {
            NtbSoft.ERP.Libs.clsConvert<Models.SYSTEM.SystemUserDuyetConfigViewModel> convert = new Libs.clsConvert<Models.SYSTEM.SystemUserDuyetConfigViewModel>();
            return _model.Post(convert.ToDataTable(items));
        }
    }
}
