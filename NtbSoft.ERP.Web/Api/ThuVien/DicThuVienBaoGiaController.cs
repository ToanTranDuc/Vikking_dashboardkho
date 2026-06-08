using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/DicThuVienBaoGia")]
    public class DicThuVienBaoGiaController : ApiController
    {
        private DicThuVienBaoGiaModel _model = new DicThuVienBaoGiaModel();
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
 
       
        [HttpDelete]
        [Route("Delete")]
        public async Task<string> Delete(string action, string Para1, string Para2 = null)
        {
            Para1 = Para1 ?? "NONE";
            Para2 = Para2 ?? "NONE";
            return await _model.Delete(action, Para1, Para2);
        }

        [HttpPost]
        [Route("PostVanChuyen")]
        public async Task<string> PostVanChuyen([FromBody] string Json)
        {

            DataTable dsSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.PostVanChuyen(dsSave);
        }
        [HttpPost]
        [Route("PostThanhToan")]
        public async Task<string> PostThanhToan([FromBody] string Json)
        {

            DataTable dsSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.PostThanhToan(dsSave);
        }
        [HttpPost]
        [Route("PostThue")]
        public async Task<string> PostThue([FromBody] string Json)
        {

            DataTable dsSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.PostThue(dsSave);
        }
        [HttpPost]
        [Route("PostChietKhau")]
        public async Task<string> PostChietKhau([FromBody] string Json)
        {

            DataTable dsSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.PostChietKhau(dsSave);
        }
    }
}