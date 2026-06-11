using NtbSoft.ERP.Model.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/SystemRoleUser_Line")]
    public class SystemRoleUserController : ApiController
    {
        SystemRoleUser_LineModel _model = new SystemRoleUser_LineModel();
        [HttpGet]
        [Route("GetRoleByUser")]
        public DataTable GetRole(string user)
        {
            DataTable result = _model.GetRole(user);
            return result;
        }
        [HttpPost]
        [Route("UpdateRoleLine_User")]
        public string Update(string user, DataTable dataUpdate)
        {
            string result = _model.Update(user, dataUpdate);
            return result;
        }
    }
}