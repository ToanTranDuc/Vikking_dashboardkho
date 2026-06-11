using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;
namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/SettingTiLeLoi")]
    public class SettingTiLeLoiController : ApiController
    {
        private readonly ISettingTiLeLoiRepository _repo = new SettingTiLeLoiRepository();
        [HttpGet]
        [Route("GetLineX")]
        public DataTable GetLineX()
            => _repo.GetLineX();
        [HttpGet]
        [Route("GetLenh")]
        public DataTable GetLenh(string lineX = "")
            => _repo.GetLenh(lineX);
        [HttpGet]
        [Route("GetSettingTiLeLoi")]
        public DataTable GetSettingTiLeLoi(string line = "", string lenh = "", string maHang = "")
            => _repo.GetSettingTiLeLoi(line, lenh, maHang);
        [HttpGet]
        [Route("SaveSettingTiLeLoi")]
        public DataTable SaveSettingTiLeLoi(
            string line = "", string lenh = "", string maHang = "",
            string tiLeLoi = "", string ngayCaiDat = "", string id = "")
            => _repo.SaveSettingTiLeLoi(line, lenh, maHang, tiLeLoi, ngayCaiDat, id);
        [HttpGet]
        [Route("DeleteSettingTiLeLoi")]
        public DataTable DeleteSettingTiLeLoi(string id = "", string jsonIds = "")
            => _repo.DeleteSettingTiLeLoi(id, jsonIds);
    }
}