using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/CheckListCD")]
    public class CheckListCongDoanController : ApiController
    {
        private CheckListCongDoanModel _model = new CheckListCongDoanModel();

        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5);
        }
        [HttpPost]
        [Route("POST")]
        public async Task<string> POST(string action, [FromBody] string Json)
        {
          
            DataSet dsSave = JsonConvert.DeserializeObject<DataSet>(Json);
            return await _model.Post(action, dsSave);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<string> Delete(string action, string Para1, string Para2 = null,string Para3 = null,string Para4 = null)
        {
            Para1 = Para1 ?? "NONE";
            Para2 = Para2 ?? "NONE";
            Para3 = Para3 ?? "NONE";
            Para4 = Para4 ?? "NONE";
            return await _model.Delete(action, Para1, Para2, Para3, Para4);
        }

    }
}