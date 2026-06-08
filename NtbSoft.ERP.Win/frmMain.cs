using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraTabbedMdi;
using System.Net.Http;
using Newtonsoft.Json;
using NtbSoft.HRM.Win.Account;
using System.Configuration;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Modules.ResourceForm;
using System.Threading.Tasks;
using AutoUpdaterDotNET;
using System.IO;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraNavBar;
using System.Diagnostics;
using NtbSoft.ERP.Web.Services;
using DevExpress.Utils.Taskbar.Core;
using NtbSoft.ERP.Win.Modules.SystemForm;
using DevExpress.Utils.Svg;

namespace NtbSoft.ERP.Win
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
      
        System.Configuration.AppSettingsReader settingsReader =
                                               new AppSettingsReader();
        List<SystemUserModuleEntity> _listUserModule;
        SystemUserModuleService _serviceUserModule;
        private string _User = string.Empty;
        frmLogin _frm = new frmLogin();
        string _URL = string.Empty;
        string _menu = string.Empty;
        public frmMain()
        {
            InitializeComponent();
            this.Load += frmMain_Load;//new EventHandler(frmMain_Load);
            _URL = (string)settingsReader.GetValue("URL", typeof(String));
            _menu = (string)settingsReader.GetValue("ADMIN_MODE", typeof(String));
            _serviceUserModule = new SystemUserModuleService();
            this.KeyPreview = true;
            Task.Run(() => TyGiaSetUp.startAsync());
            this.FormClosing += frmMain_FormClosing;
           
        }
        
        
        void barManager_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarSubItem subMenu = e.Item as BarSubItem;
            if (subMenu != null) return;
            //MessageBox.Show("Item '" + e.Item.Caption + "' has been clicked");
            //if (e.Item == null) return;
            if (e.Item.Tag == null) return;
            string name = e.Item.Tag.ToString();
            if (name == "logout" || name == "changepassword" || name == "PO")
            {
                if (name == "logout")
                {
                    GlobleData.IsUserLoggedIn = false;
                    this.Visible = false;
                    _frm = new frmLogin();
                    _frm._User = GlobleData.UserName;
                    if (_frm.ShowDialog() == DialogResult.OK)
                    {
                        if (this.Visible == false && GlobleData.IsUserLoggedIn)
                        {
                            this.Controls.Clear();
                            LoadDataMenuVertical();
                            this.Visible = true;
                            
                        }
                    }
                }
                else
                {
                    if (name == "changepassword")
                    {
                        frmChangePassword frmChangePass = new frmChangePassword();
                        frmChangePass._userID = GlobleData.UserName;
                        frmChangePass.ShowDialog();
                    }
                    else
                    {
                        //frmErpUpdateLenhSXPO frmPO = new frmErpUpdateLenhSXPO();
                        //frmPO.ShowDialog();
                    }    
                }
                return;
            }
            
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(frmWait));
            string formName = (from l in _listUserModule
                               where l.Procedured == name
                               select l.FormShow).First<string>();// GetFormName(_listUserModule,name);

            if (string.IsNullOrEmpty(formName))
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(NtbSoft.ERP.Win.Properties.Resources.NoSupportNow, NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            //{
            //    XtraMessageBox.Show(type.Name);
            //}

            var XformType = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(XtraForm) && a.Name == formName).FirstOrDefault();
            

            if (XformType != null)
            {
                if (ExitForm(XformType))
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    return;
                }
                XtraForm f = (XtraForm)Activator.CreateInstance(XformType); 
                f.MdiParent = this;
                f.Show();

                f.Activate();
            }
            else
            {
                var formType = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(Form) && a.Name == formName).FirstOrDefault();
                if (formType != null)
                {
                    if (ExitForm(formType))
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        return;
                    }

                    Form f = (Form)Activator.CreateInstance(formType);
                    f.MdiParent = this;
                    f.Show();
                    f.Activate();
                    
                }
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private bool ExitForm(Type type)
        {
            if (type == typeof(XtraForm) || type.BaseType ==typeof(XtraForm))
            {

                XtraForm f = (XtraForm)Activator.CreateInstance(type);
                if (f != null)
                {
                    foreach (var item in MdiChildren)
                    {
                        if (item.Name == f.Name)
                        {
                            item.Activate();
                            return true;
                        }
                    }
                }
            }
            else
            {
                //check create instance
                Form f = (Form)Activator.CreateInstance(type);
                if (f != null)
                {
                    foreach (var item in MdiChildren)
                    {
                        if (item.Name == f.Name)
                        {
                            item.Activate();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            _frm.ShowDialog(this);
            _User = _frm._User;
            if (_frm.IsDisposed)
            {
                this.Close();
                this.Dispose();
            }
            if (GlobleData.IsUserLoggedIn)
            {
                // start kiểm tra bản cập nhật trước khi load pm
               // CheckUpdate();
                // end  kiểm tra bản cập nhật trước khi load pm
                LoadDataMenuVertical();
              

                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                Application.Exit();
            }
            InitializeSignalRAsync();
            Task.Run(() => SetNotifyBadge());
        }
        private void CheckUpdate()
        {
            System.Configuration.AppSettingsReader settingsReader =
                                               new AppSettingsReader();
            string _url_update = (string)settingsReader.GetValue("URL_UP", typeof(String));

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            AutoUpdater.CheckForUpdateEvent += new AutoUpdater.CheckForUpdateEventHandler(AutoUpdaterOnCheckForUpdateEvent);
            string version = fvi.FileVersion;
            //label1.Text = "Phiên bản: " + version;
            AutoUpdater.DownloadPath = "update";
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 15 * 60 * 1000,
                SynchronizingObject = this
            };
            timer.Elapsed += delegate
            {
                AutoUpdater.Start(_url_update + "Version.XML");
            };
            timer.Start();
            AutoUpdater.Start(_url_update + "Version.XML");
        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.IsUpdateAvailable)
            {
                DialogResult dialogResult;
                dialogResult =
                        MessageBox.Show(
                            $@"Bạn ơi, phần mềm của bạn có phiên bản mới {args.CurrentVersion}. Phiên bản bạn đang sử dụng hiện tại  {args.InstalledVersion}. Bạn có muốn cập nhật phần mềm không?", @"Cập nhật phần mềm",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                {
                    try
                    {
                        string fileName = string.Empty, path = string.Empty;
                        string currentDir = Environment.CurrentDirectory;
                        DirectoryInfo directory = new DirectoryInfo(currentDir);

                        string fullDirectory = System.IO.Directory.GetParent(currentDir).FullName;

                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.InitialDirectory = fullDirectory;
                        dlg.FileName = System.IO.Path.GetFileName(args.DownloadURL);

                        fileName = dlg.FileName;

                        var currentDirectory = new DirectoryInfo(fileName);
                        if (currentDirectory.Parent != null)
                        {
                            AutoUpdater.InstallationPath = fullDirectory;
                        }
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            Application.Exit();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadData()
        {
            //MessageBox.Show(Properties.Resources.admin16x16);
           
            if (_listUserModule != null) _listUserModule.Clear();
            string url =string.Format(_URL+ResourceURL.UrlUserModule+ "/GetPer?userID={0}",GlobleData.UserName);
            _listUserModule = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;

            BarManager barManager = new BarManager();
            BarAndDockingController bdController = new BarAndDockingController();
            barManager.Form = this;
            barManager.Controller = bdController;
            bdController.LookAndFeel.SkinName = "Office 2013";
            //bdController.LookAndFeel.SkinMaskColor = Color.Azure;
            bdController.LookAndFeel.UseDefaultLookAndFeel = false;
            bdController.LookAndFeel.SkinMaskColor = Color.FromArgb(128, 255, 128);
            bdController.AppearancesBar.ItemsFont = new System.Drawing.Font("Tahoma", 9.75F);


            barManager.BeginUpdate();
            Bar barMain = new Bar(barManager, "Main Manu");
            barMain.OptionsBar.UseWholeRow = true;
            barMain.OptionsBar.DrawDragBorder = false;
            Bar bar = new Bar(barManager, "Menu");
            bar.OptionsBar.DrawDragBorder = false;
            bar.OptionsBar.UseWholeRow = true;

            barMain.DockStyle = BarDockStyle.Top;
            bar.DockStyle = BarDockStyle.Top;
            barMain.OptionsBar.AllowQuickCustomization = false;
            bar.OptionsBar.AllowQuickCustomization = false;
            barMain.DockRow = 0;
            barManager.MainMenu = barMain;



            if (_listUserModule.Count == 0)
            {
                XtraMessageBox.Show("Lỗi server!");
                return;
            }
            List<SystemUserModuleEntity> dataTab = (from l in _listUserModule
                                                    where l.ParentID == "0"
                                                    select l).ToList();
            for (int i = 0; i < dataTab.Count; i++)
            {
                BarSubItem subSystem = new BarSubItem(barManager, dataTab[i].Title);
                if (!string.IsNullOrEmpty(dataTab[i].Image))
                {
                    object imageIcon = Resources.ResourceManager.GetObject(dataTab[i].Image);
                    subSystem.Glyph = (Image)imageIcon;//new Bitmap(path);

                    subSystem.BorderStyle = BarItemBorderStyle.Lowered;
                    subSystem.Tag = _listUserModule[i].Alias;
                    subSystem.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                }
                
                AddButtonInBarManager(_listUserModule, dataTab[i].ModuleID, barManager, bar, subSystem);
                
                barMain.AddItem(subSystem);
            }
            List<SystemUserModuleEntity> dataBar = (from l in _listUserModule
                                                    where l.IsBarBelow == true && l.AllowView == true
                                                    select l).ToList();
            for (int j = 0; j < dataBar.Count; j++)
            {
                BarButtonItem bbiUser = new BarButtonItem(barManager, dataBar[j].Title);
                bbiUser.BorderStyle = BarItemBorderStyle.Lowered;
                if (!string.IsNullOrEmpty(dataBar[j].Image))
                {
                    object imageIcon = Resources.ResourceManager.GetObject(dataBar[j].Image);
                    bbiUser.Glyph = (Image)imageIcon;
                    bbiUser.PaintStyle = BarItemPaintStyle.CaptionGlyph;

                }
                bbiUser.Tag = dataBar[j].Procedured;
                bar.AddItem(bbiUser);
            }
            if (_menu.ToUpper().ToString() == "TRUE")
            {
                BarSubItem subPO = new BarSubItem(barManager, "Cập nhật");
                subPO.Name = "Cập nhật";
                subPO.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                BarButtonItem bbiPO = new BarButtonItem(barManager, "PO");
                bbiPO.Tag = "PO";
                subPO.AddItem(bbiPO);
                barMain.AddItem(subPO);
            }

            BarSubItem subUser = new BarSubItem(barManager, GlobleData.UserName);
            subUser.Name = GlobleData.UserName;
            subUser.Glyph = new Bitmap(NtbSoft.ERP.Win.Properties.Resources.administrator);
            subUser.PaintStyle = BarItemPaintStyle.CaptionGlyph;

            BarButtonItem bbiLogout = new BarButtonItem(barManager, "Đăng xuất");
            bbiLogout.Tag = "logout";
            BarButtonItem bbiChangePass = new BarButtonItem(barManager, "Đổi mật khẩu");
            bbiChangePass.Tag = "changepassword";
            subUser.AddItem(bbiLogout);
            subUser.AddItem(bbiChangePass);
            barMain.AddItem(subUser);

            barManager.ItemClick += new ItemClickEventHandler(barManager_ItemClick);

            Bar barFooter = new Bar(barManager, "Status");
            barFooter.CanDockStyle = BarCanDockStyle.Bottom;
            barFooter.DockStyle = BarDockStyle.Bottom;
            barFooter.OptionsBar.AllowQuickCustomization = false;
            barFooter.OptionsBar.DrawDragBorder = false;
            barFooter.OptionsBar.UseWholeRow = true;
            BarStaticItem barStatic = new BarStaticItem();
            barStatic.Caption = NtbSoft.ERP.Win.Properties.Resources.Version;
            barStatic.Alignment = BarItemLinkAlignment.Right;

            BarStaticItem barDateStatic = new BarStaticItem();
            barDateStatic.Caption = "Ngày " + string.Format("{0:dd/MM/yyy}", DateTime.Now);
            barDateStatic.Alignment = BarItemLinkAlignment.Right;

            BarStaticItem barUser = new BarStaticItem();
            barUser.Caption = "Người dùng: " + GlobleData.UserName;
            barUser.Alignment = BarItemLinkAlignment.Right;


            barFooter.AddItems(new BarItem[] { barStatic, barDateStatic, barUser });
            barManager.EndUpdate();

            XtraTabbedMdiManager xtmManager = new XtraTabbedMdiManager();
            xtmManager.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
            xtmManager.MdiParent = this;
            xtmManager.AppearancePage.Header.Font = new Font(this.Font.Name, this.Font.Size);
  //          Process.Start(
  //    @"D:\Phan mem Cty\Quanlysanxuat\QLSX_MayDongNai\Source\QLSX_MayDongNai\NtbSoft.ERP.Win\bin\Debug\N-Garmentmanager.exe"
  //);
            frmFirstRun frm = new frmFirstRun();
            frm.MdiParent = this;
            frm.Show();
        }

        private void LoadDataMenuVertical()
        {
            if (_listUserModule != null) _listUserModule.Clear();
            string url = string.Format(_URL + ResourceURL.UrlUserModule + "/GetPer?userID={0}", GlobleData.UserName);
            _listUserModule = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;

            dockManager1.AutoHiddenPanelShowMode = AutoHiddenPanelShowMode.MouseHover;
            DockPanel panel1 = dockManager1.AddPanel(DockingStyle.Left);
            panel1.Text = "Chức năng";
            NavBarControl navBar = new NavBarControl();
            navBar.MouseDown += NavBar_MouseDown;
            panel1.Controls.Add(navBar);
            navBar.Dock = DockStyle.Fill;
            navBar.LookAndFeel.UseDefaultLookAndFeel = false;
            navBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            navBar.Appearance.NavigationPaneHeader.FontSizeDelta = 9;
            navBar.Appearance.NavPaneContentButton.FontSizeDelta = 9;
            navBar.View = new DevExpress.XtraNavBar.ViewInfo.StandardSkinExplorerBarViewInfoRegistrator("Office 2010 Blue");
            panel1.MouseLeave += Panel1_MouseLeave;

            if (_listUserModule.Count == 0)
            {
                XtraMessageBox.Show("Lỗi server!");
                return;
            }
            List<SystemUserModuleEntity> dataTab = (from l in _listUserModule
                                                    where l.ParentID == "0"
                                                    select l).ToList();
            navBar.BeginUpdate();
            for (int i = 0; i < dataTab.Count; i++)
            {
                NavBarGroup groupLocal = new NavBarGroup(dataTab[i].Title);
                if (!string.IsNullOrEmpty(dataTab[i].Image))
                {
                    object imageIcon = Resources.ResourceManager.GetObject(dataTab[i].Image);
                    groupLocal.Tag = _listUserModule[i].Alias;
                }
                navBar.Groups.Add(groupLocal);
        
                AddItemInGroup(_listUserModule, dataTab[i].ModuleID, groupLocal);

                //foreach (NavBarItemLink itemLink in groupLocal.ItemLinks)
                //{
                //    NavBarItem item = itemLink.Item;  // Lấy NavBarItem từ NavBarItemLink
                //    item.Appearance.Font = new Font("Arial", 10, FontStyle.Regular); // Cài đặt font cho item
                //}
                //// Đăng ký sự kiện MouseMove cho NavBarControl để phát hiện chuột vào mục nào
                //navBar.MouseMove += (s, e) =>
                //{
                //    // Kiểm tra nếu chuột đang di vào nhóm
                //    if (navBar.CalcHitInfo(e.Location).InGroup && navBar.CalcHitInfo(e.Location).Group == groupLocal)
                //    {
                //        // Mở rộng nhóm khi chuột vào
                //        groupLocal.Expanded = true;

                //        // Thay đổi font và màu sắc của nhóm
                //        groupLocal.Appearance.Font = new Font(groupLocal.Appearance.Font, FontStyle.Bold); // Phóng to font
                //        groupLocal.Appearance.Options.UseFont = true;
                //        groupLocal.Appearance.BackColor = Color.LightBlue; // Đổi màu nền khi chuột vào
                //    }
                //    else
                //    {
                //        // Thu nhỏ nhóm khi chuột rời khỏi
                //        groupLocal.Expanded = false;

                //        // Đổi lại font và màu sắc
                //        groupLocal.Appearance.Font = new Font(groupLocal.Appearance.Font, FontStyle.Regular); // Phục hồi font
                //        groupLocal.Appearance.Options.UseFont = true;
                //        groupLocal.Appearance.BackColor = Color.Transparent; // Phục hồi màu nền
                //    }
                //};

            }

          

            NavBarGroup subUser = new NavBarGroup(GlobleData.NhanVien);
            subUser.Name = GlobleData.NhanVien;
            navBar.Groups.Add(subUser);
            NavBarItem bbiLogout = new NavBarItem("Đăng xuất");
            bbiLogout.Tag = "logout";
            NavBarItem bbiChangePass = new NavBarItem("Đổi mật khẩu");
            bbiChangePass.Tag = "changepassword";
            lbNgay.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lbNguoiDung.Text = GlobleData.UserName;
            subUser.ItemLinks.Add(bbiLogout);
            bbiLogout.LinkClicked += ItemInbox_LinkClicked;
            subUser.ItemLinks.Add(bbiChangePass);
            bbiChangePass.LinkClicked += ItemInbox_LinkClicked;
            navBar.EndUpdate();

            XtraTabbedMdiManager xtmManager = new XtraTabbedMdiManager();
            xtmManager.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
            xtmManager.MdiParent = this;
            xtmManager.AppearancePage.Header.Font = new Font(this.Font.Name, this.Font.Size);
            //            Process.Start(
            //@"D:\Phan mem Cty\Quanlysanxuat\QLSX_MayDongNai\QLSX_MDN_Backup2h\NtbSoft.ERP.Win\bin\\QLSX\N-Garmentmanager.exe"
            //);
            frmFirstRun frm = new frmFirstRun();
            frm.MdiParent = this;
            frm.Show();
        }

        private void NavBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NavBarControl navBar = sender as NavBarControl;
                NavBarHitInfo hitInfo = navBar.CalcHitInfo(new Point(e.X, e.Y));
                if (hitInfo.InGroupCaption && !hitInfo.InGroupButton)
                    hitInfo.Group.Expanded = !hitInfo.Group.Expanded;
            }
        }

        private void Panel1_MouseLeave(object sender, EventArgs e)
        {
        //    DockPanel dockPanelCN = sender as DockPanel;
        //    dockPanelCN.ActiveControl = null;
        //    dockManager1.ActivePanel = null;
        //    this.ActiveControl = null;
        }

        private void AddItemInGroup(List<SystemUserModuleEntity> listSub, string moduleId, NavBarGroup subSystem)
        {

            List<SystemUserModuleEntity> lsubMenu = (from l in listSub
                                                     where l.ParentID == moduleId && l.AllowView == true
                                                     select l).ToList();

            foreach (SystemUserModuleEntity item in lsubMenu)
            {
                if (!string.IsNullOrEmpty(item.Procedured))
                {
                    NavBarItem itemInbox = new NavBarItem(item.Title);
                    itemInbox.Tag = item.Procedured;
                    subSystem.ItemLinks.Add(itemInbox);
                    itemInbox.LinkClicked += ItemInbox_LinkClicked;


                }
                
            }

        }

        private void ItemInbox_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (e.Link.Item.Tag == null) return;
            string name = e.Link.Item.Tag.ToString();
            if (name == "logout" || name == "changepassword" || name == "PO")
            {
                if (name == "logout")
                {
                    GlobleData.IsUserLoggedIn = false;
                    this.Visible = false;
                    _frm = new frmLogin();
                    _frm._User = GlobleData.UserName;
                    if (_frm.ShowDialog() == DialogResult.OK)
                    {
                        if (this.Visible == false && GlobleData.IsUserLoggedIn)
                        {
                            this.Controls.Clear();
                            LoadDataMenuVertical();
                            this.Visible = true;
                        }
                    }
                }
                else
                {
                    if (name == "changepassword")
                    {
                        frmChangePassword frmChangePass = new frmChangePassword();
                        frmChangePass._userID = GlobleData.UserName;
                        frmChangePass.ShowDialog();
                    }
                    else
                    {
                    }
                }
                return;
            }
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(frmWait));
            string formName = (from l in _listUserModule
                               where l.Procedured == name
                               select l.FormShow).First<string>();// GetFormName(_listUserModule,name);

            if (string.IsNullOrEmpty(formName))
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(NtbSoft.ERP.Win.Properties.Resources.NoSupportNow, NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var XformType = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(XtraForm) && a.Name == formName).FirstOrDefault();


            if (XformType != null)
            {
                if (ExitForm(XformType))
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    return;
                }
                lbModule.Text = XformType.Name;
                XtraForm f = (XtraForm)Activator.CreateInstance(XformType);
                f.MdiParent = this;
                f.Show();
                f.Activate();
            }
            else
            {
                var formType = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(Form) && a.Name == formName).FirstOrDefault();
                if (formType != null)
                {
                    if (ExitForm(formType))
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        return;
                    }
                    lbModule.Text = formType.Name;
                    Form f = (Form)Activator.CreateInstance(formType);
                    f.MdiParent = this;
                    f.Show();
                    f.Activate();

                }
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void AddButtonInBarManager(List<SystemUserModuleEntity> listSub, string moduleId, BarManager barManager, Bar bar, BarSubItem subSystem)
        {

            List<SystemUserModuleEntity> lsubMenu = (from l in listSub
                                                    where l.ParentID == moduleId && l.AllowView == true
                                                    select l).ToList();

            foreach (SystemUserModuleEntity item in lsubMenu)
            {
                if (!string.IsNullOrEmpty(item.Procedured))
                {
                    BarButtonItem bbiUser = new BarButtonItem(barManager, item.Title);
                    //bbiUser.Tag = dataModule[j];
                    //bbiUser.Description = dataModule[j]["Alias"].ToString();
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        //3
                        //string wanted_path = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                        //string path = System.IO.Path.Combine(wanted_path, @"Resources\" + item.Image);
                        object imageIcon = Resources.ResourceManager.GetObject(item.Image);
                        bbiUser.Glyph = (Image)imageIcon;
                        //bbiUser.Glyph = new Bitmap(path);
                        bbiUser.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                    }
                    bbiUser.Tag = item.Procedured;
                    subSystem.AddItem(bbiUser);
                }
                else
                {
                    BarSubItem bsiUser = new BarSubItem(barManager, item.Title);
                    bsiUser.Tag = item.Alias;
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        //4
                        //string wanted_path = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                        //string path = System.IO.Path.Combine(wanted_path, @"Resources\" + item.Image);
                        object imageIcon = Resources.ResourceManager.GetObject(item.Image);
                        bsiUser.Glyph = (Image)imageIcon;
                        //bsiUser.Glyph = new Bitmap(path);
                        bsiUser.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                    }
                    subSystem.AddItem(bsiUser);
                    //bar.AddItem(bsiUser);
                    AddButtonInBarManager(_listUserModule, item.ModuleID, barManager, bar, bsiUser);
                }
            }
           
        }

        #region Config SignR

        private async void InitializeSignalRAsync()
        {
            try
            {
               
                NotificationManager.Instance.Initialize(this);

               
                bool connected = await SignalRManager.Instance.Initialize(_URL);
                
                if (connected)
                {
                   
                    RegisterSignalREvents();

                  
                }
                

    
            }
            catch (Exception ex)
            {
                
            }
        }

        private void RegisterSignalREvents()
        {
           
            SignalRManager.Instance.OnNotificationReceived += (message) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() =>
                    {
                        ShowPhieuNotification(message);
                    }));
                }
                else
                {
                    ShowPhieuNotification(message);
                }

                //Task.Run()
                //NotificationManager.Instance.UpdateTaskbarBadge(this);
                Task.Run( () =>  SetNotifyBadge());

            };

            

        }

        private void ShowPhieuNotification(dynamic message)
        {
            try
            {
                string title = message.Title?.ToString() ?? "Thông báo";
                string detail = message.Detail?.ToString() ?? "";            
                string ModuleID = message.Action?.ToString() ?? "";
                string SendTo = message.SendTo?.ToString() ?? "";
                string Creater = message.Creater?.ToString() ?? "";
                if (SendTo == "ALL")
                {
                    NotificationManager.Instance.ShowNotificationALLUser(
                        Creater,
                        title,
                        detail,
                        NotificationType.Info,
                        ModuleID,
                        message
                    );                  
                }
                else
                {                  
                    NotificationManager.Instance.ShowNotificationUser(
                        Creater,
                        title,
                        detail,
                        NotificationType.Info,
                        ModuleID,
                        SendTo,
                        message
                    );
                }
                                        
                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị thông báo: {ex.Message}");
            }
        } 

        private async void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            await SignalRManager.Instance.Disconnect();
        }

        #endregion
        private Image _badgeCompositeImage = null;
        private void lblNotification_Click(object sender, EventArgs e)
        {
            frnNotification frm = new frnNotification();
            frm.Show();
        }

        public async void SetNotifyBadge()
        {
            int _countPhieu = 0;
            try
            {
                _countPhieu = await Task.Run(() =>
                    NotificationManager.Instance.GetCountPhieuCanDuyet());

                Image compositeImage = null;

              
                Action prepareAndUpdate = () =>
                {
                    try
                    {

                        NotificationManager.Instance.UpdateTaskbarBadge(this);


                        compositeImage = CreateIconWithBadge(_countPhieu);

                        
                        var oldImage = lblNotification.ImageOptions.Image;

                     
                        lblNotification.ImageOptions.SvgImage = null;
                        lblNotification.ImageOptions.Image = compositeImage;

                        
                        lblNotification.ImageAlignToText = ImageAlignToText.None;
                        lblNotification.Appearance.TextOptions.HAlignment =
                            DevExpress.Utils.HorzAlignment.Center;

                        if (oldImage != null && oldImage.Tag as string == "badge_composite")
                            oldImage.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"prepareAndUpdate Error: {ex.Message}");
                    }
                };

                if (lblNotification.InvokeRequired)
                    lblNotification.Invoke(prepareAndUpdate);
                else
                    prepareAndUpdate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetNotifyBadge Error: {ex.Message}");
            }
        }

        private Image CreateIconWithBadge(int count)
        {
            int iconW = 24;
            int iconH = 24;
            int padTop = 8;   
            int padRight = 6;   
            int canvasW = iconW + padRight; 
            int canvasH = iconH + padTop;  

            Bitmap result = new Bitmap(canvasW, canvasH,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            result.Tag = "badge_composite";

            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                Image baseIcon = null;
                if (lblNotification.ImageOptions.Image != null && _badgeCompositeImage == null)
                {
                    baseIcon = new Bitmap(lblNotification.ImageOptions.Image, 24, 24);
                    _badgeCompositeImage = baseIcon;
                   
                }

                g.DrawImage(_badgeCompositeImage, 0, padTop, iconW, iconH);


                if (count > 0)
                    DrawBadge(g, count, canvasW);
            }

            return result;
        }

        private void DrawBadge(Graphics g, int count, int canvasW)
        {
            string text = count > 99 ? "99+" : count.ToString();
            bool twoDigits = count > 9;
            bool threeUp = count > 99;
            int badgeH = 18;  // +2
            int padX = threeUp ? 3 : twoDigits ? 4 : 3;

            using (Font f = new Font("Segoe UI", 11f, FontStyle.Bold, GraphicsUnit.Point))  // +2pt
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                StringFormat sfMeasure = StringFormat.GenericTypographic;
                SizeF textSize = g.MeasureString(text, f, PointF.Empty, sfMeasure);
                int badgeW = Math.Max(badgeH, (int)Math.Ceiling(textSize.Width) + padX * 2);
                RectangleF rect = new RectangleF(canvasW - badgeW, 0f, badgeW, badgeH);

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (SolidBrush b = new SolidBrush(Color.FromArgb(192, 57, 43)))
                using (var path = RoundRectPath(rect, badgeH / 2f))
                {
                    g.FillPath(b, path);
                }

                RectangleF borderRect = RectangleF.Inflate(rect, -0.75f, -0.75f);
                using (Pen pen = new Pen(Color.White, 1.5f))
                using (var path = RoundRectPath(borderRect, (badgeH - 1.5f) / 2f))
                {
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                    g.DrawPath(pen, path);
                }

                using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic)
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    RectangleF textRect = new RectangleF(rect.X, rect.Y + 0.5f, rect.Width, rect.Height);
                    g.DrawString(text, f, Brushes.White, textRect, sf);
                }
            }
        }



        private System.Drawing.Drawing2D.GraphicsPath RoundRectPath(RectangleF r, float radius)
        {
            float d = radius * 2f;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

      
            
    }
}
