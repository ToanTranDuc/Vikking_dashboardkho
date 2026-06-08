using DevExpress.XtraBars.Alerter;
using Newtonsoft.Json;
using NtbSoft.ERP.Win;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Web.Services
{
    public class NotificationManager
    {
        private static NotificationManager _instance;
        private AlertControl _alertControl;
        private Form _parentForm;
      
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private readonly Dictionary<string, AlertInfo> _activeAlerts = new Dictionary<string, AlertInfo>();
        public static NotificationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationManager();
                }
                return _instance;
            }
        }

        private NotificationManager() {

            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }

        public void Initialize(Form parentForm)
        {
            _parentForm = parentForm;
            _alertControl = new AlertControl();

            // Cấu hình AlertControl
            _alertControl.AllowHtmlText = true;
            _alertControl.AutoFormDelay = 8000; 
            _alertControl.AutoHeight = true;
            _alertControl.ShowPinButton = true;
            _alertControl.ShowCloseButton = true;
            _alertControl.FormLocation = AlertFormLocation.BottomRight;
            _alertControl.FormMaxCount = 5;
          

            
            _alertControl.LookAndFeel.UseDefaultLookAndFeel = false;
            _alertControl.LookAndFeel.SkinName = "VS Dark"; 

            _alertControl.AppearanceCaption.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            _alertControl.AppearanceCaption.ForeColor = Color.Blue;
            _alertControl.AppearanceCaption.Options.UseFont = true;
            _alertControl.AppearanceCaption.Options.UseForeColor = true;
            _alertControl.AppearanceCaption.Options.UseBackColor = true;

            _alertControl.AppearanceText.ForeColor = Color.Black;      
            _alertControl.AppearanceText.Options.UseForeColor = true;
            _alertControl.AppearanceText.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            AlertButton btnGoTo = new AlertButton();
            btnGoTo.Name = "btnGoTo";
            btnGoTo.Hint = "Xem chi tiết";


            _alertControl.AllowHtmlText = true;

            _alertControl.Buttons.Add(btnGoTo);

            // 3. Xử lý sự kiện click button
            _alertControl.ButtonClick += AlertControl1_ButtonClick;


            _alertControl.AlertClick += AlertControl_AlertClick;
           
            _alertControl.BeforeFormShow += (s, e) =>
            {
                e.AlertForm.Width = 425;
            };
        }
        private void AlertControl1_ButtonClick(object sender, AlertButtonClickEventArgs e)
        {
            dynamic tag = e.Info.Tag;
            if (tag == null) return;

            if (e.ButtonName == "Close")
            {
                try
                {
                    
                    string title = tag.Title;
                    string detail = tag.Detail;

                    string url = URL + $"NotifyManger/Update?action=SetNotifyWatchedByContent" +
                                        $"&para1={GlobleData.UserName}" +
                                        $"&para2={Uri.EscapeDataString(title)}" +
                                        $"&para3={Uri.EscapeDataString(detail)}";

                    Task.Run(async () =>
                    {
                        await _clientExtension.PostAsync(url, null);
                    }).Wait();
                }
                catch (Exception ex) { }
                finally
                {
                    e.AlertForm.Close();
                }
            }
            else if (e.ButtonName == "btnGoTo")
            {
                string id = tag.Id?.ToString();
                // navigate với id
            }
        }
        //public DataTable CreateNotificationTable()
        //{
        //    DataTable dt = new DataTable();

        //    dt.Columns.Add("ID", typeof(long));
        //    dt.Columns.Add("UserID", typeof(string));
        //    dt.Columns.Add("MaPB", typeof(string));
        //    dt.Columns.Add("MaBoPhan", typeof(string));
        //    dt.Columns.Add("NotificationID", typeof(string));
        //    dt.Columns.Add("Title", typeof(string));
        //    dt.Columns.Add("Detail", typeof(string));
        //    dt.Columns.Add("NhanVien", typeof(string));
        //    dt.Columns.Add("Creater", typeof(string));
        //    dt.Columns.Add("TBP", typeof(int));
        //    dt.Columns.Add("Status", typeof(int));
        //    dt.Columns.Add("ModuleID", typeof(string));
        //    return dt;
        //}


        //private void SaveNotification(dynamic objNotify)
        //{
        //    if (objNotify == null) return;
        //    try
        //    {

        //        string title = objNotify.Title?.ToString() ?? "Thông báo";
        //        string detail = objNotify.Detail?.ToString() ?? "";
        //        string ModuleID = objNotify.Action?.ToString() ?? "";
        //        string SendTo = objNotify.SendTo?.ToString() ?? "";
        //        string Creater = objNotify.Creater?.ToString() ?? "";
        //        string BoPhan = objNotify.BoPhan?.ToString() ?? "";
        //        string Status = objNotify.Status?.ToString() ?? "";
        //        string UserName = objNotify.UserName?.ToString() ?? "";

        //        DataTable tblSave = CreateNotificationTable();



        //        DataRow row = tblSave.NewRow();
        //        row["ID"] = 0;
        //        row["UserID"] = GlobleData.UserName;
        //        row["MaPB"] = SendTo;
        //        row["MaBoPhan"] = BoPhan;
        //        row["NotificationID"] = "";
        //        row["Title"] = title;
        //        row["Detail"] = detail;
        //        row["NhanVien"] = UserName;
        //        row["Creater"] = Creater;
        //        row["TBP"] = Status;
        //        row["Status"] = 0;
        //        row["ModuleID"] = ModuleID;

        //        tblSave.Rows.Add(row);
        //        string url = string.Format("{0}", URL + "NotifyManger/Post?action=POST");
        //        string jsonSave = JsonConvert.SerializeObject(tblSave);
        //        string msResult = Task.Run(async () =>
        //        {
        //            return await _clientExtension.PostAsync(url, jsonSave);
        //        }).Result;
        //    }
        //    catch(Exception ex)
        //    {

        //    }


        //}

        public void ShowNotificationUser(string Creater, string title, string detail,
            NotificationType type = NotificationType.Info,string ModuleID = "", string SendTo = "" ,dynamic objNotify = null)
        {
            if (_alertControl == null || objNotify == null)
            {
             
                return;
            }
          
            DataTable tbl = new DataTable();
            string url = string.Empty;
            string action = string.Empty;
            if(GlobleData.UserName?.ToLower() == "admin")
            {
                Image icon = GetIconByType(type);
                string formattedDetail = detail;

                AlertInfo info = new AlertInfo(title, formattedDetail, icon);

                info.Tag = objNotify;
                string tag = $"{title ?? "Thông báo"}-{ detail ?? ""}";

                if (!string.IsNullOrEmpty(tag) && _activeAlerts.ContainsKey(tag))
                    return;

                if (!string.IsNullOrEmpty(tag))
                    _activeAlerts[tag] = info;

                _alertControl.Show(_parentForm, info);
            }
            else
            {

                string BoPhan = objNotify.BoPhan?.ToString() ?? "";
                string Status = objNotify.Status?.ToString() ?? "";
                if(!int.TryParse(Status,out int StatusBP)) StatusBP = -1;

               

                if (SendTo == "ALL")
                {
                    action = "GetALLUser";
                }
                else if (SendTo == "TBP")
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

                url = $"{URL}NotifyManger/GET?action={action}&para1={ModuleID}&para2={SendTo}&para3={BoPhan}&para4={StatusBP}&para5={GlobleData.UserName}";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                }

                if (tbl != null && tbl?.Rows?.Count > 0)
                {
                    Image icon = GetIconByType(type);
                    string formattedDetail = detail;

                    AlertInfo info = new AlertInfo(title, formattedDetail, icon);

                    info.Tag = objNotify;
                    string tag = $"{title ?? "Thông báo"}-{ detail ?? ""}";

                    if (!string.IsNullOrEmpty(tag) && _activeAlerts.ContainsKey(tag))
                        return;

                    if (!string.IsNullOrEmpty(tag))
                        _activeAlerts[tag] = info;
                    _alertControl.Show(_parentForm, info);
                    
                }
            }

            
           
        }
        public void ShowNotificationALLUser(string Creater, string title, string detail,
           NotificationType type = NotificationType.Info, string ModuleID = "", object objNotify = null)
        {
            if (_alertControl == null|| objNotify==null)
            {

                return;
            }


            Image icon = GetIconByType(type);
            string formattedDetail = detail;

            AlertInfo info = new AlertInfo(title, formattedDetail, icon);

            info.Tag = objNotify;

            string tag = $"{title ?? "Thông báo"}-{ detail ?? ""}";

            if (!string.IsNullOrEmpty(tag) && _activeAlerts.ContainsKey(tag))
                return;

            if (!string.IsNullOrEmpty(tag))
                _activeAlerts[tag] = info;
            _alertControl.Show(_parentForm, info);

            
        }

        private Image GetIconByType(NotificationType type)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.ico");

                if (System.IO.File.Exists(iconPath))
                {
                    Image img = Image.FromFile(iconPath);
                    return new Bitmap(img, new Size(32, 32));
                }

                return SystemIcons.Information.ToBitmap();
            }
            catch
            {
                return SystemIcons.Information.ToBitmap();
            }
        }

        private void AlertControl_AlertClick(object sender, AlertClickEventArgs e)
        {

           /* dynamic tag = e.Info.Tag;
            if (tag == null) return;

            if (e.AlertForm. == "Close")
            {
                try
                {

                    string title = tag.Title;
                    string detail = tag.Detail;

                    string url = URL + $"NotifyManger/Update?action=SetNotifyWatchedByContent" +
                                        $"&para1={GlobleData.UserName}" +
                                        $"&para2={Uri.EscapeDataString(title)}" +
                                        $"&para3={Uri.EscapeDataString(detail)}";

                    Task.Run(async () =>
                    {
                        await _clientExtension.PostAsync(url, null);
                    }).Wait();
                }
                catch (Exception ex) { }
                finally
                {
                    e.AlertForm.Close();
                }
            }
            else if (e.ButtonName == "btnGoTo")
            {
                string id = tag.Id?.ToString();
                // navigate với id
            }*/
        }

    
        public void CloseAllNotifications()
        {
            
            if (_alertControl != null)
            {
                // Cách 1: Lấy tất cả AlertForm và đóng
                foreach (var form in _alertControl.AlertFormList)
                {
                    form.Close();
                }

                // Hoặc Cách 2: Dispose và tạo lại AlertControl
                // _alertControl.Dispose();
                // Initialize(_parentForm);
            }
        }

     
        public void CloseNotification(AlertInfo alertInfo)
        {
            if (_alertControl != null && alertInfo != null)
            {
                foreach (var form in _alertControl.AlertFormList)
                {
                    if (form.AlertInfo == alertInfo)
                    {
                        form.Close();
                        break;
                    }
                }
            }
        }

      
        public bool HasActiveNotifications()
        {
            return _alertControl != null && _alertControl.AlertFormList.Count > 0;
        }

        public int GetActiveNotificationCount()
        {
            return _alertControl?.AlertFormList.Count ?? 0;
        }

        int _countPhieu = 0;

        public int GetCountPhieuCanDuyet()
        {
            string url = string.Empty;
            DataTable tbl = new DataTable();
            url = $"{URL}NotifyManger/GET?action=CountNotify&para1={GlobleData.UserName}&para2=0";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }
            if(tbl != null && tbl?.Rows?.Count > 0)
            {
                int.TryParse(tbl?.Rows[0]["TongPhieu"]?.ToString(), out _countPhieu);
            }
            return _countPhieu;


        }


        
        public async void UpdateTaskbarBadge(DevExpress.XtraEditors.XtraForm frmMain)
        {
            _countPhieu = 0;
            try
            {
                if (_taskbarList == null) InitTaskbar();

          
                await Task.Run(() => GetCountPhieuCanDuyet());

                IntPtr hIcon = IntPtr.Zero;

                if (_countPhieu > 0)
                {
                    
                    hIcon = await Task.Run(() =>
                    {
                        using (Bitmap bmp = new Bitmap(64, 64))
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.Transparent);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                            int circleSize = 64;
                            g.FillEllipse(Brushes.Crimson, 0, 0, circleSize, circleSize);

                            using (Pen pen = new Pen(Color.White, 3))
                                g.DrawEllipse(pen, 1, 1, circleSize - 3, circleSize - 3);

                            using (Font f = new Font("Segoe UI", 30, FontStyle.Bold))
                            {
                                string text = _countPhieu > 99 ? "99+" : _countPhieu.ToString();
                                SizeF textSize = g.MeasureString(text, f);
                                g.DrawString(text, f, Brushes.White,
                                    (circleSize - textSize.Width) / 2,
                                    (circleSize - textSize.Height) / 2);
                            }
                            return bmp.GetHicon();
                        }
                    });
                }

           
                Action updateUI = () =>
                {
                    try
                    {
                        if (hIcon != IntPtr.Zero)
                            _taskbarList.SetOverlayIcon(frmMain.Handle, hIcon, $"{_countPhieu} thông báo mới");
                        else
                            _taskbarList.SetOverlayIcon(frmMain.Handle, IntPtr.Zero, string.Empty);
                    }
                    finally
                    {
                        if (hIcon != IntPtr.Zero)
                            DestroyIcon(hIcon);
                    }
                };

               
                if (frmMain.InvokeRequired)
                    frmMain.Invoke(updateUI);
                else
                    updateUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTaskbarBadge Error: {ex.Message}");
            }
        }

        // ─── Windows Taskbar COM Interface ────────────────────────────────────────────

        private ITaskbarList3 _taskbarList;

        private void InitTaskbar()
        {
            _taskbarList = (ITaskbarList3)new CTaskbarList();
            _taskbarList.HrInit();
        }

        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            void HrInit();
            void AddTab(IntPtr hwnd);
            void DeleteTab(IntPtr hwnd);
            void ActivateTab(IntPtr hwnd);
            void SetActiveAlt(IntPtr hwnd);
            void MarkFullscreenWindow(IntPtr hwnd, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fFullscreen);
            void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
            void SetProgressState(IntPtr hwnd, int tbpFlags);
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            void UnregisterTab(IntPtr hwndTab);
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);
            void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszDescription);
            void SetThumbnailTooltip(IntPtr hwnd, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszTip);
            void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
        }

        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
        [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.None)]
        private class CTaskbarList { }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }

       public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}