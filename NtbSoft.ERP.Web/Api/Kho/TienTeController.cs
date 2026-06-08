using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/TIENTE")]
    public class TienTeController : ApiController
    {
        TienTeModel _model = new TienTeModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para, string para2, string para3)
        {
            return _model.Get(action, para, para2, para3 );
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,DataTable tblSave)
        {
            return _model.Post(action ,tblSave);
        }

        [HttpPost]
        [Route("Post2")]
        public string Post2(string action, DataTable tblSave)
        {
            return _model.Post2(action, tblSave);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string id, string matiente, string para2 ="")
        {
            return _model.Delete(action,id,matiente, para2);
        }

    }
}