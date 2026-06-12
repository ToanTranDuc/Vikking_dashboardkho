using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using NtbSoft.ERP.Model.POMH;
using NtbSoft.ERP.Web.Service;
using System.Linq; 
using System.Collections.Generic;
using NtbSoft.ERP.Entity.Notification;

namespace NtbSoft.ERP.Web.Api.POMH
{
    [RoutePrefix("api/SendToNotification")]
    public class PushNotifyController : ApiController
    {
        private HubService _hubService = new HubService();
        private PheDuyetModel _model = new PheDuyetModel();
        [HttpPost]
        [Route("PushNotification")]
        public async Task PushNotification(string UserIDTao, string ModuleId, string Title = "", string Detail = "", string SendTo = "", string BoPhan = "ALL", int Status = -1, int IsQLSX = 0, string MaPhieu = "")
        {
            try
            {
                await _hubService.SendNotification(UserIDTao, ModuleId, Title, Detail, SendTo, BoPhan, Status, IsQLSX, MaPhieu);
            }
            catch (Exception ex)
            {

            }
        }
        [HttpPost]
        [Route("PushNotificationFrm")]
        public async Task<string> PushNotificationFrm(string UserIDTao, string FrmName, string Title = "", string Detail = "", string SendTo = "", string BoPhan = "ALL", int Status = -1, int IsQLSX = 0, string MaPhieu = "")
        {
            try
            {
                string ModuleId = FrmName;
                await _hubService.SendNotification(UserIDTao, ModuleId, Title, Detail, SendTo, BoPhan, Status, IsQLSX, MaPhieu);
                return "True";
            }
            catch (Exception ex)
            {
                return "False";

            }
        }


        [HttpPost]
        [Route("PushMultiNotifications")]
        public async Task<string> PushMultiNotifications(List<NotificationEntity> lstNotify)
        {
            try
            {
                if (lstNotify == null || lstNotify.Count == 0) return "False";

                await Task.WhenAll(lstNotify.Select(item =>
                    _hubService.SendNotification(
                        item.UserIDTao,
                        item.FrmName,
                        item.Title,
                        item.Detail,
                        item.SendTo,
                        item.BoPhan,
                        item.Status,
                        item.IsQLSX,
                        item.MaPhieu
                    )
                ));

                return "True";
            }
            catch (Exception ex)
            {
                return "False";
            }
        }
    }
   
}