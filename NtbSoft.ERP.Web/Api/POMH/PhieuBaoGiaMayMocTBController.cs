using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using NtbSoft.ERP.Model.POMuaHang;
using Microsoft.AspNet.SignalR;
using NtbSoft.ERP.Web.Service;
using System;

namespace NtbSoft.ERP.Web.Api.POMuaHang
{
    [RoutePrefix("api/PhieuBaoGiaMMTB")]
    public class PhieuBaoGiaMayMocTBController : ApiController
    {
       
        private PhieuBaoGiaMayMocTBModel _model = new PhieuBaoGiaMayMocTBModel();
        private HubService _hubService = new HubService();

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
        [Route("POSTPhieuBG")]
        public async Task<string> POSTPhieuBG(string action, [FromBody] string Json)
        {

            DataSet dsSave = JsonConvert.DeserializeObject<DataSet>(Json);
            string result = await _model.Post(action, dsSave);
            try
            {
                if (!string.IsNullOrEmpty(result))
                {
                    // Lấy HubContext
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubService>();


                    string maPhieu = "";
                    string UserIDTao = "";


                    if (dsSave.Tables[0] != null && dsSave.Tables[0].Rows.Count > 0 && dsSave.Tables[0].Columns.Contains("TenPhieu"))
                    {
                        maPhieu = dsSave?.Tables[0].Rows[0]["TenPhieu"]?.ToString() ?? "";
                        UserIDTao = dsSave?.Tables[0].Rows[0]["NguoiTao"]?.ToString() ?? dsSave?.Tables[0].Rows[0]["NguoiSua"]?.ToString();
                    }

                    await _hubService.SendToUser(maPhieu, UserIDTao, "M.12.02.00", "Phiếu cần duyệt", "PKH","ALL",1);

                }

            }

            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Lỗi gửi thông báo SignalR: {ex.Message}");
            }
            return result;
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
        [Route("CheckHieuLuc")]
        public DataTable CheckHieuLuc([FromBody] DataTable tbl,
                                      string action, string para1, string para2,
                                      string para3, string para4, string para5)
        { 

            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return _model.Get(action, tbl, para1, para2, para3, para4, para5);
        }

           
       
    }
}