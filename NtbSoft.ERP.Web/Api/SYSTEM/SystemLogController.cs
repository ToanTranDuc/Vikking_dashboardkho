using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Models.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    //[HMACAuthentication]
    [RoutePrefix("api/SystemLog")]
    public class SystemLogController : ApiController
    {
        ISystemLogRepository _repo = new SystemLogRepository();

        [HttpGet]
        [Route("Get")]
        public IEnumerable<SystemLogViewModel> Get(string fromStr, string toStr)
        {
            DateTime fromDate = Convert.ToDateTime(fromStr);
            DateTime toDate = Convert.ToDateTime(toStr);
            return _repo.Get(fromDate, toDate);
        }
        [HttpPost]
        [Route("")]
        public string Post(List<SystemLogConfig> items)
        {
            return _repo.Post(items);
        }
        [HttpPost]
        [Route("WriteFileLog")]
        public void WriteFileLog(object Content)
        {
            try
            {
                if (System.Web.HttpContext.Current == null)
                    return;
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Application Logs/");
                // check if directory exists
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = path + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
                // check if file exist
                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }

                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine("\r\nLog Entry");
                    w.WriteLine("{0}", DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                    string logMsg = string.Format("{0}", Content);
                    w.WriteLine(logMsg);
                    w.WriteLine("--------------------------------------------------------------------------------------");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteFileLog(ex.ToString());
            }
        }

        [HttpPost]
        [Route("ExeSql")]
        public string ExeSql(SystemLogExeSQLViewModel items)
        {
            return _repo.ExeSQL(items);
        }

        [HttpDelete]
        [Route("{id}")]
        public string Delete(long id)
        {
            return _repo.Delete(id);
        }
    }
}
