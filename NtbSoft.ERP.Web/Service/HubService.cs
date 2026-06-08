using Microsoft.AspNet.SignalR;
using NtbSoft.ERP.Model.POMH;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace NtbSoft.ERP.Web.Service
{
    public class HubService:Hub
    {
        PheDuyetModel _model = new PheDuyetModel();
        private NotifyManagerModel _modelNotify = new NotifyManagerModel();
        private IHubContext _hubContext;
     
        public HubService()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<HubService>();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        // Sự kiện kết nối
        public override System.Threading.Tasks.Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
           
            return base.OnConnected();
        }


        public DataTable CreateNotificationTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("UserID", typeof(string));
            dt.Columns.Add("MaPB", typeof(string));
            dt.Columns.Add("MaBoPhan", typeof(string));
            dt.Columns.Add("NotificationID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Detail", typeof(string));
            dt.Columns.Add("NhanVien", typeof(string));
            dt.Columns.Add("Creater", typeof(string));
            dt.Columns.Add("TBP", typeof(int));
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("ModuleID", typeof(string));
            dt.Columns.Add("IsQLSX", typeof(int));
            dt.Columns.Add("MaPhieu", typeof(string));

            return dt;
        }


        private async Task SaveNotification(dynamic objNotify)
        {
            if (objNotify == null) return;
            try
            {

                string title = objNotify.Title?.ToString() ?? "Thông báo";
                string detail = objNotify.Detail?.ToString() ?? "";
                string ModuleID = objNotify.Action?.ToString() ?? "";
                string SendTo = objNotify.SendTo?.ToString() ?? "";
                string Creater = objNotify.Creater?.ToString() ?? "";
                string BoPhan = objNotify.BoPhan?.ToString() ?? "";
                string Status = objNotify.Status?.ToString() ?? "";
                string UserName = objNotify.UserName?.ToString() ?? "";
                string strIsQLSX = objNotify.IsQLSX.ToString() ?? "";
                int isQLSX = 0;
                if (!string.IsNullOrEmpty(strIsQLSX))
                {
                    int.TryParse(strIsQLSX, out isQLSX);
                }

                string maPhieu = objNotify.MaPhieu?.ToString() ?? "";

                DataTable tblSave = CreateNotificationTable();
                string action = "";
              
                if (!int.TryParse(Status, out int StatusBP)) StatusBP = -1;

                if (SendTo == "ALL")
                {
                    action = "GetALLUser";
                }else if (SendTo == "TBP")
                {
                    action = "GetTBP";
                }

                else if (SendTo == "QLSX")
                {
                    action = "GetUserQLSX";

                }
                else if (SendTo != "ALL" && (BoPhan == SendTo || BoPhan == "ALL") && StatusBP == -1)
                {
                    action = "GetUserbyDepartmnet";

                }
                else if (StatusBP != -1 && SendTo != "ALL")
                {
                    action = "GetUserbyModule";

                }
                
                DataRow row = tblSave.NewRow();
                row["ID"] = 0;
                row["UserID"] = "Admin";
                row["MaPB"] = SendTo;
                row["MaBoPhan"] = BoPhan;
                row["NotificationID"] = "";
                row["Title"] = title;
                row["Detail"] = detail;
                row["NhanVien"] = UserName;
                row["Creater"] = Creater;
                row["TBP"] = Status;
                row["Status"] = 0;
                row["ModuleID"] = ModuleID;
                row["IsQLSX"] = 0;
                row["MaPhieu"] = maPhieu;
                tblSave.Rows.Add(row);

                DataTable tblUses = await _modelNotify.Get(action, ModuleID, SendTo, BoPhan, StatusBP.ToString(), "ALL");
                if(tblUses!= null && tblUses?.Rows?.Count > 0)
                {
                    foreach(DataRow rowUser in tblUses.Rows)
                    {
                        //if (Creater?.ToUpper()?.Trim() == rowUser["UserID"]?.ToString()?.ToUpper()?.Trim()) continue;
                        row = tblSave.NewRow();
                        row["ID"] = 0;
                        row["UserID"] = rowUser["UserID"];
                        row["MaPB"] = SendTo;
                        row["MaBoPhan"] = BoPhan;
                        row["NotificationID"] = "";
                        row["Title"] = title;
                        row["Detail"] = detail;
                        row["NhanVien"] = UserName;
                        row["Creater"] = Creater;
                        row["TBP"] = Status;
                        row["Status"] = 0;
                        row["ModuleID"] = ModuleID;
                        row["IsQLSX"] = isQLSX;
                        row["MaPhieu"] = maPhieu;
                        tblSave.Rows.Add(row);
                    }
                   
                }



                await _modelNotify.Post("POST", tblSave);
               
            }
            catch (Exception ex)
            {

            }


        }

        public async Task<DataTable> GetNhanVien(string UserID)
        {
            DataTable tblNhanVien = new DataTable();
            try
            {
                 tblNhanVien = await _model.Get("GetNhanVien", UserID, "NONE", "NONE", "NONE", "NONE");
               
            }
            catch (Exception ex)
            {
                return tblNhanVien;
            }

            return tblNhanVien;
        }

        public async Task SendToUser(string MaPhieu,string UserIDTao, string ModuleId,string Title = "",string SendTo = "", string BoPhan = "ALL", int Status = -1,int IsQLSX = 0)
        {
            try
            {
                //var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubService>();
                string pattern = @"^\d+[\.\s]+\s*";
                string NhanVien = UserIDTao;
                DataTable tblModules = await _model.Get("GetModules", ModuleId, "NONE", "NONE", "NONE", "NONE");             
                //if (tblModules != null && tblModules?.Rows?.Count == 0) return;

                DataTable  tblNhanVien = await GetNhanVien(UserIDTao);
                string PhongBan = string.Empty;
                if (tblNhanVien != null && tblNhanVien?.Rows?.Count > 0)
                {
                    var Query = tblNhanVien.AsEnumerable().FirstOrDefault(x => x["UserID"]?.ToString()?.ToUpper() == UserIDTao?.ToUpper());
                    if (Query != null)
                    {
                        NhanVien = $"{Query["TenNhanVien"]}\nPhòng ban : {Query["PhongBan"]?.ToString() ?? ""}";
                        PhongBan = Query["PhongBan"]?.ToString();
                    }
                }
                string TenModule = string.Empty;
                if (tblModules != null && tblModules?.Rows?.Count > 0)
                {
                    TenModule = Regex.Replace(tblModules?.Rows[0]["Title"]?.ToString(), pattern, "").Trim();
                }
                    var notification = new
                {
                    SendTo = SendTo,
                    Title = $"{Title}\n{TenModule}",
                    Detail = $"Mã phiếu: {MaPhieu}\nThực hiện bởi {NhanVien}\nLúc: {DateTime.Now:dd/MM/yyyy HH:mm}",
                    MaPhieu = MaPhieu,
                    Action = ModuleId,
                    Creater=UserIDTao,
                    UserName = PhongBan,
                    Timestamp = DateTime.Now,
                    BoPhan = BoPhan,
                    Status = Status,
                    FrmName = ModuleId,
                    IsQLSX = IsQLSX,
                     
                    };
               await SaveNotification(notification);

                _hubContext.Clients.All.ReceiveNotification(notification);
            }
            catch(Exception ex)
            {

            }
          

        }

        public async Task SendNotification(string UserIDTao, string ModuleId, string Title = "", string Detail = "", string SendTo = "", string BoPhan = "ALL", int Status = -1, int IsQLSX = 0,string MaPhieu = "")
        {
            try
            {
                //var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubService>();
                string pattern = @"^\d+[\.\s]+\s*";
                string NhanVien = UserIDTao;
                DataTable tblModules = await _model.Get("GetModules", ModuleId, "NONE", "NONE", "NONE", "NONE");
            

                DataTable tblNhanVien = await GetNhanVien(UserIDTao);
                string PhongBan = string.Empty;
                if (tblNhanVien != null && tblNhanVien?.Rows?.Count > 0)
                {
                    var Query = tblNhanVien.AsEnumerable().FirstOrDefault(x => x["UserID"]?.ToString()?.ToUpper() == UserIDTao?.ToUpper());
                    if (Query != null)
                    {
                        NhanVien = $"{Query["TenNhanVien"]}\nPhòng ban : {Query["PhongBan"]?.ToString() ?? ""}";
                        PhongBan = Query["TenPB"]?.ToString();
                    }
                }
                string TenModule = string.Empty;
                if (tblModules != null && tblModules?.Rows?.Count > 0)
                {
                    TenModule = Regex.Replace(tblModules?.Rows[0]["Title"]?.ToString(), pattern, "").Trim();
                }
                var notification = new
                {
                    SendTo = SendTo,
                    Title = $"{Title}\n{TenModule}",                  
                    Detail = $"{Detail}\nThực hiện bởi {NhanVien}\nLúc: {DateTime.Now:dd/MM/yyyy HH:mm}",
                    Action = ModuleId,
                    Creater = UserIDTao,
                    UserName = PhongBan,
                    Timestamp = DateTime.Now,
                    BoPhan = BoPhan,
                    Status = Status,
                    FrmName = ModuleId,
                    IsQLSX = IsQLSX,
                    MaPhieu = MaPhieu,
                };
                await SaveNotification(notification);
                _hubContext.Clients.All.ReceiveNotification(notification);
            }
            catch (Exception ex)
            {

            }

        }

        }
    }