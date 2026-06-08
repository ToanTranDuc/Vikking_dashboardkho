using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.WipDonHang;
using NtbSoft.ERP.Web.Models;
using NtbSoft.ERP.Web.Models.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NtbSoft.ERP.Web.Api.WipDonHang.PhanQuyenWIPController;

namespace NtbSoft.ERP.Win.Modules.SystemForm
{
    public partial class frmSysUser : DevExpress.XtraEditors.XtraForm
    {
        private class WipLineLookupModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        GetDataService _serviceGetData = new GetDataService();
        SystemUserService _serviceUser;
        SystemUserModuleService _serviceUserModule;
        SystemUserReportService _serviceUserReport;
        SystemGroupService _serviceGroup;
        SystemGroupModuleService _serviceGroupModule;
        SystemGroupReportService _serviceGroupReport;
        BindingList<SystemGroupEntity> _bindingGroupsEntity;
        SystemUserModuleService _serviceUserDuyet = new SystemUserModuleService();
        HttpClientExtension _clientExtension = new HttpClientExtension();
        private string _userId = string.Empty;
        private int _currentWipLineX = 0;
        private int _groupId = 0;
        ResourceURL.EventStatus _status;
        private bool _allowAdd = false;
        private bool _allowEdit = false;
        private bool _allowDelete = false;
        List<SystemUserModuleEntity> _lstUserModuleAdd; List<SystemUserReportEntity> _lstUserReportAdd;
        List<SystemGroupModuleEntity> _lstGroupModuleAdd; List<SystemGroupReportEntity> _lstGroupReportAdd;
        List<SystemUserModuleDVSX> _listUserModuleDVSX; List<SystemUserWeb> _listUserModuleWeb;
        private string testMode;
        public frmSysUser()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _serviceUser = new SystemUserService();
            _serviceUserModule = new SystemUserModuleService();
            _serviceUserReport = new SystemUserReportService();
            _serviceGroup = new SystemGroupService();
            _serviceGroupModule = new SystemGroupModuleService();
            _serviceGroupReport = new SystemGroupReportService();
            _lstUserModuleAdd = new List<SystemUserModuleEntity>();
            _lstUserReportAdd = new List<SystemUserReportEntity>();
            _lstGroupModuleAdd = new List<SystemGroupModuleEntity>();
            _lstGroupReportAdd = new List<SystemGroupReportEntity>();
            _listUserModuleDVSX = new List<SystemUserModuleDVSX>();
            _listUserModuleWeb = new List<SystemUserWeb>();
            testMode = (string)settingsReader.GetValue("testMode", typeof(String));


            DevExpress.Skins.Skin skin = DevExpress.Skins.GridSkins.GetSkin(tlModule.LookAndFeel);
            skin.Properties[DevExpress.Skins.GridSkins.OptShowTreeLine] = true;
            tlModule.LookAndFeel.UpdateStyleSettings();

            DevExpress.Skins.Skin skin1 = DevExpress.Skins.GridSkins.GetSkin(tlReport.LookAndFeel);
            skin1.Properties[DevExpress.Skins.GridSkins.OptShowTreeLine] = true;
            tlReport.LookAndFeel.UpdateStyleSettings();
            _status = ResourceURL.EventStatus.View;

            repositoryItemCheckEdit5.EditValueChanged += repositoryItemCheckEdit5_EditValueChanged;
            gvFeatureWip.CellValueChanged += gvFeatureWip_CellValueChanged;
            gvLineWip.FocusedRowChanged += gvLineWip_FocusedRowChanged;
            treeListWIP.CellValueChanging += treeListWIP_CellValueChanging;
            gvLineWip.RowStyle += gvLineWip_RowStyle;
        }
        private void gvLineWip_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle == gvLineWip.FocusedRowHandle)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("#4169E1");
                e.Appearance.ForeColor = Color.White;
                e.HighPriority = true;
                return;
            }
        }
        private void repositoryItemCheckEdit5_EditValueChanged(object sender, EventArgs e)
        {
            gvFeatureWip.PostEditor();
            gvFeatureWip.UpdateCurrentRow();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadData();
            CreateDataTable();
        }

        private void LoadData()
        {
            CheckPerminsion();
            LoadNV();
            LoadLineWip();
            LoadUser();
            LoadGroup();
            GridViewUpdateStatus(_status);
            LoadUserNV();
        }

        private async void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = await _serviceUserModule.UserModuleGet(url);
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
                btAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            if (!_allowDelete)
                btDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            if (!_allowAdd && !_allowEdit)
                btSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gvUser.OptionsBehavior.Editable = false;
                    gvGroup.OptionsBehavior.Editable = false;
                    gvInGroup.OptionsBehavior.Editable = false;
                    gvOutGroup.OptionsBehavior.Editable = false;
                    tlModule.OptionsBehavior.Editable = false;
                    tlReport.OptionsBehavior.Editable = false;
                    break;
                case ResourceURL.EventStatus.Add:
                    if (tcUser.SelectedTabPage == tpGroup)
                    {
                        gvGroup.OptionsBehavior.Editable = true;
                    }
                    tlModule.OptionsBehavior.Editable = true;
                    tlReport.OptionsBehavior.Editable = true;
                    break;
                case ResourceURL.EventStatus.Edit:
                    if (tcUser.SelectedTabPage == tpGroup)
                    {
                        gvGroup.OptionsBehavior.Editable = true;
                    }
                    tlModule.OptionsBehavior.Editable = true;
                    tlReport.OptionsBehavior.Editable = true;
                    tlDVSX.OptionsBehavior.Editable = true;
                    break;
            }
        }
        private void LoadNV()
        {
            string urlLine = string.Format("{0}?", URL + "NhanVien/GetAllNV");
            string jsonLine = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLine); }).Result;
            DataTable _dtLine = JsonConvert.DeserializeObject<DataTable>(jsonLine);


            if (!_dtLine.Columns.Contains("TenNVFull"))
                _dtLine.Columns.Add("TenNVFull", typeof(string));

            foreach (DataRow row in _dtLine.Rows)
            {
                string tenNV = row["TenNV"] == DBNull.Value ? "" : row["TenNV"].ToString();
                string ten = row.Table.Columns.Contains("Ten") ? row["Ten"].ToString() : "";
                row["TenNVFull"] = tenNV + "" + ten;
            }

            RepositoryItemSearchLookUpEdit repoSearchLookUpEdit = new RepositoryItemSearchLookUpEdit();
            repoSearchLookUpEdit.DataSource = _dtLine;
            repoSearchLookUpEdit.DisplayMember = "TenNVFull";
            repoSearchLookUpEdit.ValueMember = "MaNV";

            repoSearchLookUpEdit.NullText = "[Chọn nhân viên]";
            repoSearchLookUpEdit.ShowClearButton = false;
            GridView dvViewLine = repoSearchLookUpEdit.View;

            if (dvViewLine.Columns.Count == 0)
            {
                dvViewLine.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewLine.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewLine.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewLine.Appearance.HeaderPanel.Options.UseTextOptions = true;

                dvViewLine.Columns.Add(new GridColumn { FieldName = "MaNV", Caption = "Mã Nhân viên", Name = "colNhanVien", Visible = true });
                dvViewLine.Columns.Add(new GridColumn { FieldName = "TenNVFull", Caption = "Nhân viên", Name = "colTenNV", Visible = true });
                dvViewLine.Columns.Add(new GridColumn { FieldName = "ChucVu", Caption = "Chức Vụ", Name = "colChucVu", Visible = true });
                dvViewLine.Columns.Add(new GridColumn { FieldName = "PhongBan", Caption = "Phòng Ban", Name = "colPhongBan", Visible = true });
            }
            colNhanVien.ColumnEdit = repoSearchLookUpEdit;

        }

        private async void LoadUserDH(string userId)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenDH/Get", userId);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblDH = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl1.DataSource = tblDH;
            gridControl1.Refresh();
        }
        private async void LoadUserNV()
        {
            string url = string.Format("{0}?", URL + "PhanQuyenDH/GetUserNV");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblNV = JsonConvert.DeserializeObject<DataTable>(json);
            tblNV.Columns.Add("LyDo", typeof(string));
            gridControl2.DataSource = tblNV;
            gridControl2.Refresh();
        }
        private async void LoadGroup()
        {
            string url = URL + ResourceURL.UrlGroup + "/Get";
            List<SystemGroupEntity> listGroup = await _serviceGroup.GroupGet(url);
            //if (_bindingGroupsEntity.Count > 0) _bindingGroupsEntity.Clear();

            _bindingGroupsEntity = new BindingList<SystemGroupEntity>(listGroup);
            _bindingGroupsEntity.AllowNew = true;
            _bindingGroupsEntity.AllowEdit = true;
            _bindingGroupsEntity.AllowRemove = true;
            gcGroup.DataSource = _bindingGroupsEntity;
        }


        private async void LoadUser()
        {
            int focusRowHandel = GetFocusRowHandel();
            string url = URL + ResourceURL.UrlUser + "/Get";
            List<SystemUserEntity> listUser = await _serviceUser.UserGet(url);
            gcUser.DataSource = listUser;
            if (focusRowHandel > 0)
                gvUser.FocusedRowHandle = focusRowHandel;
        }
        private async void LoadUserDuyet(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenDUYET/Get", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserDuyetEntity> listCM = JsonConvert.DeserializeObject<List<SystemUserDuyetEntity>>(json);
            gridControl3.DataSource = listCM;
            gridControl3.Refresh();
        }
        //private async void LoadUserCM(string userName)
        //{
        //    string url = string.Format("{0}?userID={1}", URL + "PhanQuyenDUYET/Get?userID={0}", userName);
        //    List<SystemUserCMEntity> listCM = await _serviceUserCM.UserCMGet(url);
        //    tlCM.DataSource = listCM;
        //    tlCM.ExpandAll();
        //    tlCM.Refresh();
        //}
        private int GetFocusRowHandel()
        {
            int focusRowHandel = gvUser.FocusedRowHandle;
            if (focusRowHandel == null || focusRowHandel < 0) return -1;
            return focusRowHandel;
        }
        private void gvUser_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object useName = view.GetFocusedRowCellValue(colUserName);
            if (useName == null) return;
            _userId = useName.ToString();
            if (tcUser.SelectedTabPage == tpUser)
            {
                LoadUserModule(useName.ToString());
                LoadUserReport(useName.ToString());
                LoadUserDVSX(useName.ToString());
                CheckUserWeb(useName.ToString());
                if (testMode.ToLower() != "true")
                    getChuyen(useName.ToString());
                LoadUserDH(useName.ToString());
                LoadUserDuyet(useName.ToString());
                LoadUserXacNhanBOM(useName.ToString());
                LoadPOMHPermissions(useName.ToString());
                LoadPOMHPWEBermissions(useName.ToString());
                LoadUserExcelTS(useName.ToString());
                LoadUserPQYeuCauNPL(useName.ToString());
                LoadLineWip(useName.ToString());
                LoadUserCosting(useName.ToString());
            }
            SystemUserEntity itemUser = view.GetFocusedRow() as SystemUserEntity;
        }
        private void ReLoad()
        {
            object useName = gvUser.GetFocusedRowCellValue(colUserName);
            if (useName == null) return;
            _userId = useName.ToString();
            if (tcUser.SelectedTabPage == tpUser)
            {
                LoadUserModule(useName.ToString());
                LoadUserReport(useName.ToString());
                LoadUserDVSX(useName.ToString());
                CheckUserWeb(useName.ToString());
                LoadLineWip(useName.ToString());
                if (testMode.ToLower() != "true")
                    getChuyen(useName.ToString());
            }
            SystemUserEntity itemUser = gvUser.GetFocusedRow() as SystemUserEntity;

        }

        private async void LoadUserReport(string userName)
        {
            string url = string.Format(URL + ResourceURL.UrlUserReport + "/Get?userID={0}", userName);
            List<SystemUserReportEntity> listReport = await _serviceUserReport.UserReportGet(url);
            tlReport.DataSource = listReport;
            tlReport.KeyFieldName = "ReportID";
            tlReport.ParentFieldName = "ParentID";
            tlReport.ExpandAll();
            tlReport.Refresh();
        }

        private async void LoadUserModule(string userName)
        {
            string url = string.Format(URL + ResourceURL.UrlUserModule + "/Get?userID={0}", userName);
            List<SystemUserModuleEntity> listModule = await _serviceUserModule.UserModuleGet(url);
            tlModule.DataSource = listModule;
            tlModule.KeyFieldName = "ModuleID";
            tlModule.ParentFieldName = "ParentID";
            tlModule.ExpandAll();
            tlModule.Refresh();
        }

        private async void LoadUserDVSX(string userName)
        {
            _listUserModuleDVSX.Clear();
            string url = string.Format(URL + ResourceURL.UrlUserModule + "/GetPhanQuyenUser?userID={0}", userName);
            List<SystemUserModuleDVSX> listModule = await _serviceUserModule.UserModuleGetDVSX(url);

            tlDVSX.DataSource = listModule;
            if (userName == "admin")
            {
                colTenDVSX.OptionsColumn.AllowEdit = false;
                colChon.OptionsColumn.AllowEdit = false;
            }
            else
            {
                colChon.OptionsColumn.AllowEdit = true;
            }
            tlDVSX.ExpandAll();
            tlDVSX.Refresh();
        }
        string _userName = string.Empty;
        private async void CheckUserWeb(string userName)
        {
            _userName = userName;
            _listUserModuleWeb.Clear();
            string url = $"{URL}SystemUserModule/GetUserWeb?Para1={userName}";
            List<SystemUserWeb> listTb = await _serviceUserModule.UserModuleWeb(url);
            string json = JsonConvert.SerializeObject(listTb);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            grcWeb.DataSource = tbl;


        }
        DataTable saveUserName = new DataTable();
        private void CreateDataTable()
        {
            saveUserName.Columns.Add("UserName", typeof(string));
            saveUserName.Columns.Add("Module", typeof(string));
            saveUserName.Columns.Add("IsCheckXem", typeof(bool));
            saveUserName.Columns.Add("IsCheckSua", typeof(bool));
            saveUserName.Columns.Add("IsCheckThem", typeof(bool));
            saveUserName.Columns.Add("IsCheckXoa", typeof(bool));
        }
        private void SaveUserWeb()
        {
            try
            {
                this.ActiveControl = gcUser;
                saveUserName.Clear();
                DataTable tbl = grcWeb.DataSource as DataTable;

                foreach (DataRow item in tbl.Rows)
                {
                    DataRow newRow = saveUserName.NewRow();
                    newRow["UserName"] = _userName;
                    newRow["Module"] = item["Module"].ToString();
                    newRow["IsCheckXem"] = Convert.ToInt32(item["IsCheckXem"]);
                    newRow["IsCheckSua"] = Convert.ToInt32(item["IsCheckSua"]);
                    newRow["IsCheckThem"] = Convert.ToInt32(item["IsCheckThem"]);
                    newRow["IsCheckXoa"] = Convert.ToInt32(item["IsCheckXoa"]);
                    saveUserName.Rows.Add(newRow);
                }
                string url = $"{URL}SystemUserModule/PostUserWeb";
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, saveUserName); }).Result;
                if (json.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
            catch (Exception)
            {


            }
        }

        private void tlModule_CustomDrawNodeButton(object sender, DevExpress.XtraTreeList.CustomDrawNodeButtonEventArgs e)
        {
            Brush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Green, Color.LightGreen,
             System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
            e.Graphics.FillRectangle(backBrush, e.Bounds);

            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedOuter);

            string displayCharacter = e.Expanded ? "-" : "+";

            StringFormat outCharacterFormat = new StringFormat();
            outCharacterFormat.Alignment = StringAlignment.Center;
            outCharacterFormat.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(displayCharacter, new Font("Verdana", 8),
              new SolidBrush(Color.White), e.Bounds, outCharacterFormat);

            e.Handled = true;
        }

        private void tlReport_CustomDrawNodeButton(object sender, DevExpress.XtraTreeList.CustomDrawNodeButtonEventArgs e)
        {
            Brush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Green, Color.LightGreen,
            System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
            e.Graphics.FillRectangle(backBrush, e.Bounds);

            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedOuter);

            string displayCharacter = e.Expanded ? "-" : "+";

            StringFormat outCharacterFormat = new StringFormat();
            outCharacterFormat.Alignment = StringAlignment.Center;
            outCharacterFormat.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(displayCharacter, new Font("Verdana", 8),
              new SolidBrush(Color.White), e.Bounds, outCharacterFormat);

            e.Handled = true;
        }

        private void tcUser_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page.Name == tpUser.Name)
            {
                //LoadUser();
                object userId = gvUser.GetFocusedRowCellValue(colUserName);
                if (userId != null)
                {
                    if (string.Compare(userId.ToString(), _userId) == 0)
                    {
                        LoadUserModule(_userId);
                        LoadUserReport(_userId);
                        LoadUserDVSX(_userId);
                        CheckUserWeb(_userId);
                    }
                }
            }
            else
            {
                //LoadGroup();
                object groupId = gvGroup.GetFocusedRowCellValue(colGroupID);
                if (groupId != null)
                {
                    int id = Convert.ToInt32(groupId.ToString());
                    if (id == _groupId)
                    {
                        LoadGroupModule(id);
                        LoadGroupReport(id);
                        LoadUInGroup(id);
                        LoadUOutGroup(id);
                    }
                }
            }
        }

        private void gvGroup_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object groupId = view.GetFocusedRowCellValue(colGGroupID);
            if (groupId == null)
                return;
            int id = Convert.ToInt32(groupId.ToString());
            _groupId = id;
            if (tcUser.SelectedTabPage == tpGroup)
            {
                LoadUInGroup(id);
                LoadUOutGroup(id);
                LoadGroupModule(id);
                LoadGroupReport(id);
            }


        }

        private async void LoadGroupReport(int id)
        {
            string url = string.Format(URL + ResourceURL.UrlGroupReport + "/Get?groupId={0}", id);
            List<SystemGroupReportEntity> listReport = await _serviceGroupReport.GroupReportGet(url);
            tlReport.DataSource = listReport;
            tlReport.KeyFieldName = "ReportID";
            tlReport.ParentFieldName = "ParentID";
            tlReport.ExpandAll();
            tlReport.Refresh();

        }

        private async void LoadGroupModule(int id)
        {
            string url = string.Format(URL + ResourceURL.UrlGroupModule + "/Get?groupId={0}", id);
            List<SystemGroupModuleEntity> listModule = await _serviceGroupModule.GroupModuleGet(url);
            tlModule.DataSource = listModule;
            tlModule.KeyFieldName = "ModuleID";
            tlModule.ParentFieldName = "ParentID";
            tlModule.ExpandAll();
            tlModule.Refresh();
        }

        private async void LoadUOutGroup(int id)
        {
            string url = URL + ResourceURL.UrlUser + "/Get";
            List<SystemUserEntity> listUser = await _serviceUser.UserGet(url);
            List<SystemUserEntity> listUOutG = (from l in listUser
                                                where l.GroupID == 0
                                                select l).ToList();
            gcOutGroup.DataSource = listUOutG;
        }

        private async void LoadUInGroup(int id)
        {
            string url = URL + ResourceURL.UrlUser + "/Get";
            List<SystemUserEntity> listUser = await _serviceUser.UserGet(url);
            List<SystemUserEntity> listUInG = (from l in listUser
                                               where l.GroupID == id
                                               select l).ToList();
            gcInGroup.DataSource = listUInG;
        }

        private void btAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tcUser.SelectedTabPage.Name == tpUser.Name)
            {
                //SystemUserEntity _userObj = gvUser.GetRow(gvUser.FocusedRowHandle) as SystemUserEntity;
                frmSysUserConfig frm = new frmSysUserConfig();
                frm.ShowDialog();
                LoadData();
            }
            else
            {
                SystemGroupEntity obj = new SystemGroupEntity();
                _bindingGroupsEntity.Add(obj);
                _status = ResourceURL.EventStatus.Add;
                GridViewUpdateStatus(_status);
            }
        }

        private void tlModule_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            if (_allowEdit)
            {
                DXMenuItem menuItem = new DXMenuItem(Properties.Resources.Edit, ItemsModuleEdit_CLick);
                e.Menu.Items.Add(menuItem);
            }
        }
        private void ItemsModuleEdit_CLick(object sender, EventArgs e)
        {
            if (tcUser.SelectedTabPage == tpUser)
            {
                object objAdmin = gvUser.GetFocusedRowCellValue(colUserName);
                if (objAdmin == null) return;
                if (!CheckAdminAllowEdit(objAdmin.ToString())) return;
            }
            else
            {
                object groupId = gvGroup.GetFocusedRowCellValue(colGroupID);
                if (groupId == null) return;
                int id = Convert.ToInt32(groupId);
                if (!CheckAdminAllowEdit(id)) return;
            }
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }

        private void tlModuleDVSX_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            if (_allowEdit)
            {
                DXMenuItem menuItem = new DXMenuItem(Properties.Resources.Edit, ItemsModuleDVSXEdit_CLick);
                e.Menu.Items.Add(menuItem);
            }
        }
        private void ItemsModuleDVSXEdit_CLick(object sender, EventArgs e)
        {
            if (tcUser.SelectedTabPage == tpUser)
            {
                object objAdmin = gvUser.GetFocusedRowCellValue(colUserName);
                if (objAdmin == null) return;
                if (!CheckAdminAllowEdit(objAdmin.ToString())) return;
            }
            else
            {
                object groupId = gvGroup.GetFocusedRowCellValue(colGroupID);
                if (groupId == null) return;
                int id = Convert.ToInt32(groupId);
                if (!CheckAdminAllowEdit(id)) return;
            }
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private bool CheckAdminAllowEdit(string userID)
        {
            if (userID.ToLower() == "admin")
                return false;
            return true;
        }
        private bool CheckAdminAllowEdit(int groupId)
        {
            if (groupId == 1)
                return false;
            return true;
        }
        private void ItemsReportEdit_CLick(object sender, EventArgs e)
        {
            if (tcUser.SelectedTabPage == tpUser)
            {
                object objAdmin = gvUser.GetFocusedRowCellValue(colUserName);
                if (objAdmin == null) return;
                if (!CheckAdminAllowEdit(objAdmin.ToString())) return;
            }
            else
            {
                object groupId = gvGroup.GetFocusedRowCellValue(colGroupID);
                if (groupId == null) return;
                int id = Convert.ToInt32(groupId);
                if (!CheckAdminAllowEdit(id)) return;
            }
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }

        private void tlReport_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            if (_allowEdit)
            {
                DXMenuItem menuItem = new DXMenuItem(Properties.Resources.Edit, ItemsReportEdit_CLick);
                e.Menu.Items.Add(menuItem);
            }
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            ReLoad();
        }

        //private void tlModule_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        //{


        //}

        private void GetGroupRowChange(TreeList tl, DevExpress.XtraTreeList.Nodes.TreeListNode node)
        {
            SystemGroupModuleEntity row = tl.GetDataRecordByNode(tl.FocusedNode) as SystemGroupModuleEntity;
            if (_lstGroupModuleAdd.Count > 0)
            {
                foreach (SystemGroupModuleEntity rowinfo in _lstGroupModuleAdd)
                {
                    if (string.Compare(rowinfo.ModuleID, row.ModuleID) == 0)
                    {
                        rowinfo.Action = row.Action;
                        rowinfo.Alias = row.Alias;
                        rowinfo.AllowAdd = row.AllowAdd;
                        rowinfo.AllowDelete = row.AllowDelete;
                        rowinfo.AllowEdit = row.AllowEdit;
                        rowinfo.AllowView = row.AllowView;
                        rowinfo.Class = row.Class;
                        rowinfo.Controller = row.Controller;
                        rowinfo.FormShow = row.FormShow;
                        rowinfo.Image = row.Image;
                        rowinfo.ImageUrl = row.ImageUrl;
                        rowinfo.IsAction = row.IsAction;
                        rowinfo.IsBarBelow = row.IsBarBelow;
                        rowinfo.Link = row.Link;
                        rowinfo.ModuleID = row.ModuleID;
                        rowinfo.ParentID = row.ParentID;
                        rowinfo.Procedured = row.Procedured;
                        rowinfo.Title = row.Title;
                        SetDataGroupForChileNode(node);
                        return;
                    }
                }
            }
            _lstGroupModuleAdd.Add(row);
            SetDataGroupForChileNode(node);
        }

        private void SetDataGroupForChileNode(DevExpress.XtraTreeList.Nodes.TreeListNode node)
        {
            SystemGroupModuleEntity row = tlModule.GetDataRecordByNode(node) as SystemGroupModuleEntity;
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode childNode in node.Nodes)
            {
                bool add = true;
                SystemGroupModuleEntity childRow = tlModule.GetDataRecordByNode(childNode) as SystemGroupModuleEntity;
                childRow.AllowAdd = row.AllowAdd;
                childRow.AllowDelete = row.AllowDelete;
                childRow.AllowEdit = row.AllowEdit;
                childRow.AllowView = row.AllowView;

                foreach (SystemGroupModuleEntity rowinfo in _lstGroupModuleAdd)
                {
                    if (rowinfo.ModuleID == childRow.ModuleID)
                    {
                        add = false;
                        rowinfo.Action = childRow.Action;
                        rowinfo.Alias = childRow.Alias;
                        rowinfo.AllowAdd = childRow.AllowAdd;
                        rowinfo.AllowDelete = childRow.AllowDelete;
                        rowinfo.AllowEdit = childRow.AllowEdit;
                        rowinfo.AllowView = childRow.AllowView;
                        rowinfo.Class = childRow.Class;
                        rowinfo.Controller = childRow.Controller;
                        rowinfo.FormShow = childRow.FormShow;
                        rowinfo.Image = childRow.Image;
                        rowinfo.ImageUrl = childRow.ImageUrl;
                        rowinfo.IsAction = childRow.IsAction;
                        rowinfo.IsBarBelow = childRow.IsBarBelow;
                        rowinfo.Link = childRow.Link;
                        rowinfo.ModuleID = childRow.ModuleID;
                        rowinfo.ParentID = childRow.ParentID;
                        rowinfo.Procedured = childRow.Procedured;
                        rowinfo.Title = childRow.Title;
                        break;
                    }
                }
                if (add)
                    _lstGroupModuleAdd.Add(childRow);

                if (childNode.HasChildren)
                {
                    SetDataGroupForChileNode(childNode);
                }
            }
        }

        private void GetUserRowChange(TreeList tl, DevExpress.XtraTreeList.Nodes.TreeListNode node)
        {

            SystemUserModuleEntity row = tl.GetDataRecordByNode(node) as SystemUserModuleEntity;
            if (_lstUserModuleAdd.Count > 0)
            {
                foreach (SystemUserModuleEntity rowinfo in _lstUserModuleAdd)
                {
                    if (string.Compare(rowinfo.ModuleID, row.ModuleID) == 0)
                    {
                        rowinfo.Action = row.Action;
                        rowinfo.Alias = row.Alias;
                        rowinfo.AllowAdd = row.AllowAdd;
                        rowinfo.AllowDelete = row.AllowDelete;
                        rowinfo.AllowEdit = row.AllowEdit;
                        rowinfo.AllowView = row.AllowView;
                        rowinfo.Class = row.Class;
                        rowinfo.Controller = row.Controller;
                        rowinfo.FormShow = row.FormShow;
                        rowinfo.Image = row.Image;
                        rowinfo.ImageUrl = row.ImageUrl;
                        rowinfo.IsAction = row.IsAction;
                        rowinfo.IsBarBelow = row.IsBarBelow;
                        rowinfo.Link = row.Link;
                        rowinfo.ModuleID = row.ModuleID;
                        rowinfo.ParentID = row.ParentID;
                        rowinfo.Procedured = row.Procedured;
                        rowinfo.Title = row.Title;
                        SetDataUserForChileNode(node);
                        return;
                    }
                }
            }
            _lstUserModuleAdd.Add(row);
            SetDataUserForChileNode(node);
        }
        private void SetDataUserForChileNode(DevExpress.XtraTreeList.Nodes.TreeListNode node)
        {
            SystemUserModuleEntity row = tlModule.GetDataRecordByNode(node) as SystemUserModuleEntity;
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode childNode in node.Nodes)
            {
                bool add = true;
                SystemUserModuleEntity childRow = tlModule.GetDataRecordByNode(childNode) as SystemUserModuleEntity;
                childRow.AllowAdd = row.AllowAdd;
                childRow.AllowDelete = row.AllowDelete;
                childRow.AllowEdit = row.AllowEdit;
                childRow.AllowView = row.AllowView;

                foreach (SystemUserModuleEntity rowinfo in _lstUserModuleAdd)
                {
                    if (rowinfo.ModuleID == childRow.ModuleID)
                    {
                        add = false;
                        rowinfo.Action = childRow.Action;
                        rowinfo.Alias = childRow.Alias;
                        rowinfo.AllowAdd = childRow.AllowAdd;
                        rowinfo.AllowDelete = childRow.AllowDelete;
                        rowinfo.AllowEdit = childRow.AllowEdit;
                        rowinfo.AllowView = childRow.AllowView;
                        rowinfo.Class = childRow.Class;
                        rowinfo.Controller = childRow.Controller;
                        rowinfo.FormShow = childRow.FormShow;
                        rowinfo.Image = childRow.Image;
                        rowinfo.ImageUrl = childRow.ImageUrl;
                        rowinfo.IsAction = childRow.IsAction;
                        rowinfo.IsBarBelow = childRow.IsBarBelow;
                        rowinfo.Link = childRow.Link;
                        rowinfo.ModuleID = childRow.ModuleID;
                        rowinfo.ParentID = childRow.ParentID;
                        rowinfo.Procedured = childRow.Procedured;
                        rowinfo.Title = childRow.Title;
                        break;
                    }
                }
                if (add)
                    _lstUserModuleAdd.Add(childRow);

                if (childNode.HasChildren)
                {
                    SetDataUserForChileNode(childNode);
                }
            }
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = gcUser;
                if (_lstGroupModuleAdd.Count == 0 && _lstGroupReportAdd.Count == 0 && _lstUserModuleAdd.Count == 0 && _lstUserReportAdd.Count == 0 && MaKhuVuc == "")
                {
                    XtraMessageBox.Show(Properties.Resources.UpdateDataBefore, Properties.Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!string.IsNullOrEmpty(MaKhuVuc))
                {
                    UpdateMaKhuVuc();
                }
                if (_lstGroupModuleAdd.Count > 0)
                {
                    EditGroupModule();
                    LoadGroupModule(_groupId);
                }
                if (_lstUserModuleAdd.Count > 0)
                {
                    EditUserModule();
                    LoadUserModule(_userId);
                }
                if (_lstGroupReportAdd.Count > 0)
                {
                    EditGroupReport();
                    LoadGroupReport(_groupId);
                }
                if (_lstUserReportAdd.Count > 0)
                {
                    EditUserReport();
                    LoadUserReport(_userId);
                }

                if (_listUserModuleDVSX.Count > 0)
                {
                    EditUserModuleDVSX();

                }
                SaveUserWeb();

                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                object useName = gvUser.GetFocusedRowCellValue(colUserName);
                if (useName == null) return;
                if (testMode.ToLower() != "true")
                    SaveRoleUser_Line(useName.ToString());
                LoadUserModule(useName.ToString());
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private async void UpdateMaKhuVuc()
        {
            SystemUserEntity systemUserEntity = new SystemUserEntity()
            {
                MaKho = MaKhuVuc,
                UserName = UserName,
                AddDate = DateTime.Now,
                LastView = DateTime.Now
            };

            string ms = await _serviceUser.UpdateData(systemUserEntity, URL + ResourceURL.UrlUser + "/UpdateMaKho");
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadUser();
            ReLoad();
        }

        private async void EditUserReport()
        {
            List<SystemUserReportConfig> items = new List<SystemUserReportConfig>();
            foreach (SystemUserReportEntity row in _lstUserReportAdd)
            {
                SystemUserReportConfig item = new SystemUserReportConfig();
                item.UserID = _userId;
                item.ReportID = row.ReportID;
                item.IsActive = row.IsActive;
                items.Add(item);
            }
            string ms = await _serviceUserReport.AddData(items, URL + ResourceURL.UrlUserReport);
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lstUserReportAdd.Clear();
            LoadUserDVSX(_userId);
        }

        private async void EditUserModuleDVSX()
        {
            List<SystemUserModuleDVSX> items = new List<SystemUserModuleDVSX>();
            items = _listUserModuleDVSX;
            string ms = await _serviceUserModule.AddDataDVSX(items, URL + ResourceURL.UrlUserModule + "/PostPhanQuyen");
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            _listUserModuleDVSX.Clear();
        }


        private async void EditGroupReport()
        {
            List<SystemGroupReportConfig> items = new List<SystemGroupReportConfig>();
            foreach (SystemGroupReportEntity row in _lstGroupReportAdd)
            {
                SystemGroupReportConfig item = new SystemGroupReportConfig();
                item.GroupID = _groupId;
                item.ReportID = row.ReportID;
                item.IsActive = row.IsActive;
                items.Add(item);
            }
            string ms = await _serviceGroupReport.AddData(items, URL + ResourceURL.UrlGroupReport);
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lstGroupReportAdd.Clear();
        }

        private async void EditUserModule()
        {
            List<SystemUserModuleConfig> items = new List<SystemUserModuleConfig>();
            foreach (SystemUserModuleEntity row in _lstUserModuleAdd)
            {
                SystemUserModuleConfig item = new SystemUserModuleConfig();
                item.ModuleID = row.ModuleID;
                item.UserID = _userId;
                item.AllowAdd = row.AllowAdd;
                item.AllowDelete = row.AllowDelete;
                item.AllowEdit = row.AllowEdit;
                item.AllowView = row.AllowView;
                items.Add(item);
            }
            string ms = await _serviceUserModule.AddData(items, URL + ResourceURL.UrlUserModule);
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lstUserModuleAdd.Clear();
        }

        private async void EditGroupModule()
        {
            List<SystemGroupModuleConfig> items = new List<SystemGroupModuleConfig>();
            foreach (SystemGroupModuleEntity row in _lstGroupModuleAdd)
            {
                SystemGroupModuleConfig item = new SystemGroupModuleConfig();
                item.GroupID = _groupId;
                item.ModuleID = row.ModuleID;
                item.AllowAdd = row.AllowAdd;
                item.AllowDelete = row.AllowDelete;
                item.AllowEdit = row.AllowEdit;
                item.AllowView = row.AllowView;
                items.Add(item);
            }
            string ms = await _serviceGroupModule.PostData(items, URL + ResourceURL.UrlGroupModule);
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lstGroupModuleAdd.Clear();
        }

        private void tlReport_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            TreeList tl = (TreeList)sender;
            if (tcUser.SelectedTabPage == tpUser)
            {
                GetUserReportRowChange(tl);
            }
            else
            {
                GetGroupReportRowChange(tl);
            }
        }

        private void GetGroupReportRowChange(TreeList tl)
        {
            SystemGroupReportEntity row = tl.GetDataRecordByNode(tl.FocusedNode) as SystemGroupReportEntity;
            if (_lstGroupReportAdd.Count > 0)
            {
                int j = 0;
                foreach (SystemGroupReportEntity rowinfo in _lstGroupReportAdd)
                {
                    if (rowinfo.ReportID == row.ReportID)
                    {
                        rowinfo.IsActive = row.IsActive;
                        rowinfo.ModuleID = row.ModuleID;
                        rowinfo.ParentID = row.ParentID;
                        rowinfo.ReportID = row.ReportID;
                        rowinfo.ReportName = row.ReportName;
                        return;
                    }
                    j++;
                }
            }
            _lstGroupReportAdd.Add(row);
        }

        private void GetUserReportRowChange(TreeList tl)
        {
            SystemUserReportEntity row = tl.GetDataRecordByNode(tl.FocusedNode) as SystemUserReportEntity;
            if (_lstUserReportAdd.Count > 0)
            {
                int j = 0;
                foreach (SystemUserReportEntity rowinfo in _lstUserReportAdd)
                {
                    if (rowinfo.ReportID == row.ReportID)
                    {
                        rowinfo.IsActive = row.IsActive;
                        rowinfo.ModuleID = row.ModuleID;
                        rowinfo.ParentID = row.ParentID;
                        rowinfo.ReportID = row.ReportID;
                        rowinfo.ReportName = row.ReportName;
                        return;
                    }
                    j++;
                }
            }
            _lstUserReportAdd.Add(row);
        }

        private void tlModule_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            this.ActiveControl = this.button1;
            TreeList tl = (TreeList)sender;
            if (tcUser.SelectedTabPage == tpUser)
            {
                GetUserRowChange(tl, e.Node);
            }
            else
            {
                GetGroupRowChange(tl, e.Node);
            }
        }

        private void gvUser_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 194, 179), Color.FromArgb(255, 194, 179), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true; 
        }

        private void tlModule_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 194, 179), Color.FromArgb(255, 194, 179), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Caption, e.CaptionRect);
            ////foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.InnerElements)
            ////{
            ////    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            ////}
            //e.Handled = true; 
        }

        private void gvGroup_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 194, 179), Color.FromArgb(255, 194, 179), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true; 
        }

        private void gvOutGroup_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 194, 179), Color.FromArgb(255, 194, 179), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true; 
        }

        private void gvInGroup_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 194, 179), Color.FromArgb(255, 194, 179), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true; 
        }

        private void tlReport_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 194, 179), Color.FromArgb(255, 194, 179), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Caption, e.CaptionRect);
            ////foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.InnerElements)
            ////{
            ////    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            ////}
            //e.Handled = true; 
        }

        private async void btInGroup_Click(object sender, EventArgs e)
        {
            SystemUserEntity userOut = gvOutGroup.GetRow(gvOutGroup.FocusedRowHandle) as SystemUserEntity;
            userOut.GroupID = _groupId;
            string ms = await _serviceUser.UpdateData(userOut, URL + ResourceURL.UrlUser);
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadUInGroup(_groupId);
            LoadUOutGroup(_groupId);
        }

        private async void btOutGroup_Click(object sender, EventArgs e)
        {
            if (gvInGroup.FocusedRowHandle < 0)
            {
                return;
            }
            SystemUserEntity userIn = gvInGroup.GetRow(gvInGroup.FocusedRowHandle) as SystemUserEntity;
            userIn.GroupID = 0;
            string ms = await _serviceUser.UpdateData(userIn, URL + ResourceURL.UrlUser);
            if (string.Compare(ms, "True") != 0)
                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadUOutGroup(_groupId);
            LoadUInGroup(_groupId);

        }

        private async void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (tcUser.SelectedTabPage == tpUser)
            {
                if (_userId == "admin")
                    return;
                if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    object userID = gvUser.GetFocusedRowCellValue(colUserName);
                    if (userID == null) return;
                    string url = string.Format("{0}/{1}", URL + ResourceURL.UrlUser, userID);
                    string ss = await _serviceUser.Deleted(url);
                    if (string.Compare(ss, "True") != 0)
                        XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadUser();
                }

            }
            else
            {
                if (_groupId == 1)
                    return;
                if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    object groupId = gvGroup.GetFocusedRowCellValue(colGroupID);
                    if (groupId == null) return;
                    string url = string.Format("{0}/{1}", URL + ResourceURL.UrlGroup, groupId);
                    string ss = await _serviceGroup.Deleted(url);
                    if (string.Compare(ss, "True") != 0)
                        XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadGroup();
                }
            }
        }

        private async void frmSysUser_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    if (tcUser.SelectedTabPage.Name == tpUser.Name)
                    {
                        //SystemUserEntity _userObj = gvUser.GetRow(gvUser.FocusedRowHandle) as SystemUserEntity;
                        frmSysUserConfig frm = new frmSysUserConfig();
                        frm.ShowDialog();
                        LoadData();
                    }
                    else
                    {
                        SystemGroupEntity obj = new SystemGroupEntity();
                        _bindingGroupsEntity.Add(obj);
                        _status = ResourceURL.EventStatus.Add;
                        GridViewUpdateStatus(_status);
                    }
                    break;
                case Keys.F2:

                    break;
                case Keys.F3:
                    if (tcUser.SelectedTabPage == tpUser)
                    {
                        if (_userId == "admin")
                            return;
                        if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            object userID = gvUser.GetFocusedRowCellValue(colUserName);
                            if (userID == null) return;
                            string url = string.Format("{0}/{1}", URL + ResourceURL.UrlUser, userID);
                            string ss = await _serviceUser.Deleted(url);
                            if (string.Compare(ss, "True") != 0)
                                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            LoadUser();
                        }

                    }
                    else
                    {
                        if (_groupId == 1)
                            return;
                        if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            object groupId = gvGroup.GetFocusedRowCellValue(colGroupID);
                            if (groupId == null) return;
                            string url = string.Format("{0}/{1}", URL + ResourceURL.UrlGroup, groupId);
                            string ss = await _serviceGroup.Deleted(url);
                            if (string.Compare(ss, "True") != 0)
                                XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            LoadGroup();
                        }
                    }
                    break;
                case Keys.F4:
                    if (_lstGroupModuleAdd.Count == 0 && _lstGroupReportAdd.Count == 0 && _lstUserModuleAdd.Count == 0 && _lstUserReportAdd.Count == 0)
                    {
                        XtraMessageBox.Show(Properties.Resources.UpdateDataBefore, Properties.Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (_lstGroupModuleAdd.Count > 0)
                    {
                        EditGroupModule();
                        LoadGroupModule(_groupId);
                    }
                    if (_lstUserModuleAdd.Count > 0)
                    {
                        EditUserModule();
                        LoadUserModule(_userId);
                    }
                    if (_lstGroupReportAdd.Count > 0)
                    {
                        EditGroupReport();
                        LoadGroupReport(_groupId);
                    }
                    if (_lstUserReportAdd.Count > 0)
                    {
                        EditUserReport();
                        LoadUserReport(_userId);
                    }
                    _status = ResourceURL.EventStatus.View;
                    GridViewUpdateStatus(_status);
                    break;
                case Keys.F5:
                    LoadData();
                    break;
            }
        }

        private void frmSysUser_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void frmSysUser_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void gvGroup_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            SystemGroupEntity groupEdit = view.GetRow(e.RowHandle) as SystemGroupEntity;

            if (groupEdit != null)
            {
                string ms = Task.Run(async () => { return await _serviceGroup.UpdateData(groupEdit, URL + ResourceURL.UrlGroup); }).Result;
                if (string.Compare(ms, "True") != 0)
                    XtraMessageBox.Show(ms, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadData();
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
            }

        }
        DataTable tbl = new DataTable();

        List<string> lstNodesChild = new List<string>();
        DataTable _tblKhoUser = new DataTable();
        private string GetChildNode(DataRow dr)
        {
            List<DataRow> lstChild = _tblKhoUser.AsEnumerable().Where(x => x["ParentMaKho"].ToString() == dr["MaKho"].ToString()).ToList();
            if (lstChild != null)
            {
                foreach (DataRow drChild in lstChild)
                {
                    lstNodesChild.Add(GetChildNode(drChild));
                }

            }
            return dr["MaKho"].ToString();
        }
        private string MaKhuVuc;
        private string UserName;
        private void repoColChon_CheckedChanged(object sender, EventArgs e)
        {
            MaKhuVuc = tlNhaMay.FocusedNode[tlNhaMay.KeyFieldName].ToString();
            CheckChildNode(MaKhuVuc);
        }
        private void CheckChildNode(string _MaKhuVuc)
        {
            foreach (DataRow dataRow in tbl.Rows)
            {
                dataRow["Chon"] = false;
            }
            DataRow dr = _tblKhoUser.AsEnumerable().Where(x => x["MaKho"].ToString() == _MaKhuVuc).FirstOrDefault();
            dr["Chon"] = true;
            string MaKho = GetChildNode(dr);
            if (lstNodesChild.Count != 0)
            {
                foreach (var item in lstNodesChild)
                {
                    DataRow drChildNode = tbl.AsEnumerable().Where(x => x["MaKho"].ToString() == item).FirstOrDefault();
                    drChildNode["Chon"] = true;
                }
            }
            lstNodesChild.Clear();
        }

        private void gvUser_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            object useName = view.GetFocusedRowCellValue(colUserName);
            if (useName == null) return;
            _userId = useName.ToString();
            if (tcUser.SelectedTabPage == tpUser)
            {
                LoadUserModule(useName.ToString());
                LoadUserReport(useName.ToString());
                CheckUserWeb(useName.ToString());

            }
            SystemUserEntity itemUser = view.GetFocusedRow() as SystemUserEntity;
        }

        private void tcModule_Click(object sender, EventArgs e)
        {

        }

        private void tlDVSX_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

        }

        private void tlDVSX_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            object _username = gvUser.GetFocusedRowCellValue(colUserName);
            object _madvsxFocus = tlDVSX.GetFocusedRowCellValue(colMaDVSX);
            object _tendvsxFocus = tlDVSX.GetFocusedRowCellValue(colTenDVSX);
            object _id = tlDVSX.GetFocusedRowCellValue(colIDPhanQuyen);
            string json = JsonConvert.SerializeObject(_listUserModuleDVSX);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (_listUserModuleDVSX.Count > 0)
            {
                //DataRow dr = tbl.AsEnum/*erable().Where(x => x["MaDVSX"].ToString() == _madvsxFocus.ToString() && x["Username"].ToString() == _username.ToString()).FirstOrDefault();*/
                var results = _listUserModuleDVSX.FirstOrDefault(x => x.MaDVSX == _madvsxFocus.ToString() && x.Username == _username.ToString());


                if (results == null)
                {
                    SystemUserModuleDVSX lst = new SystemUserModuleDVSX();
                    lst.ID = Convert.ToInt32(_id);
                    lst.Username = _username.ToString();
                    lst.TenDVSX = _tendvsxFocus.ToString();
                    lst.MaDVSX = _madvsxFocus.ToString();
                    lst.IsCheck = Convert.ToBoolean(e.Value);
                    _listUserModuleDVSX.Add(lst);
                }
                else
                {
                    results.IsCheck = Convert.ToBoolean(e.Value);
                }
            }
            else
            {
                //bool _ischeck = e.Value == true ? 1 : 0
                _listUserModuleDVSX.Add(new SystemUserModuleDVSX
                {
                    ID = Convert.ToInt32(_id),
                    Username = _username.ToString(),
                    TenDVSX = _tendvsxFocus.ToString(),
                    MaDVSX = _madvsxFocus.ToString(),
                    IsCheck = Convert.ToBoolean(e.Value)
                });
            }
        }
        public void getChuyen(string UserGetRole)
        {
            try
            {
                DataTable tbLineTH1 = getDBFromOtherServer_1("DVSX_1", "lineX", "@action='GetAll_LineX'");
                //DataTable tbLineTH2 = getDBFromOtherServer_1("DVSX_5", "lineX", "@action='GetAll_LineX'");
                DataTable tbLineCG = getDBFromOtherServer_1("DVSX_2", "lineX", "@action='GetAll_LineX'");
                //DataTable tbLineCL = getDBFromOtherServer_1("DVSX_3", "lineX", "@action='GetAll_LineX'");
                string getRoleUrl = URL + "SystemRoleUser_Line/GetRoleByUser?user=" + UserGetRole;
                DataTable GetQuyen = Task.Run(async () => { return await _serviceGetData.GetDataTable(getRoleUrl); }).Result;
                DataTable result = new DataTable();
                result.Columns.Add("MaDVSX");
                result.Columns.Add("TenDVSX");
                result.Columns.Add("ID");
                result.Columns.Add("LineName");
                result.Columns.Add("IsCheck", typeof(bool));
                List<DataTable> totalTbLine = new List<DataTable>();
                totalTbLine.Add(tbLineTH1);
                //totalTbLine.Add(tbLineTH2);
                //totalTbLine.Add(tbLineCL);
                totalTbLine.Add(tbLineCG);
                foreach (DataTable itemTable in totalTbLine)
                {
                    foreach (DataRow itemRow in itemTable.Rows)
                    {
                        DataRow row = result.NewRow();
                        row["MaDVSX"] = itemRow["MaDVSX"];
                        row["TenDVSX"] = itemRow["TenDVSX"];
                        row["ID"] = itemRow["MaDVSX"] + "@" + itemRow["ID"];
                        row["LineName"] = itemRow["LineName"];
                        row["IsCheck"] = false;
                        result.Rows.Add(row);
                    }
                }
                foreach (DataTable itemTable in totalTbLine)
                {
                    DataRow row = result.NewRow();
                    row["MaDVSX"] = itemTable.Rows[0]["MaDVSX"];
                    row["TenDVSX"] = itemTable.Rows[0]["TenDVSX"];
                    row["ID"] = itemTable.Rows[0]["MaDVSX"];
                    row["LineName"] = itemTable.Rows[0]["TenDVSX"];
                    row["IsCheck"] = false;
                    result.Rows.Add(row);
                }
                if (GetQuyen != null && GetQuyen.Rows.Count > 0)
                {
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        foreach (DataRow itemRow in GetQuyen.Rows)
                        {
                            string itemRow_MaDVSX = itemRow["MaDVSX"].ToString();
                            string itemRow_LineID = itemRow["LineID"].ToString();
                            if (result.Rows[i]["ID"].ToString().Split('@').Length == 2)
                            {
                                if (result.Rows[i]["MaDVSX"].ToString() == itemRow_MaDVSX && result.Rows[i]["ID"].ToString() == itemRow_MaDVSX + "@" + itemRow_LineID)
                                    result.Rows[i]["IsCheck"] = true;
                            }
                            else
                            {
                                if (result.Rows[i]["MaDVSX"].ToString() == itemRow_MaDVSX && result.Rows[i]["ID"].ToString() == itemRow_LineID)
                                    result.Rows[i]["IsCheck"] = true;
                            }
                        }
                    }
                }
                treeListChuyen.DataSource = result;
            }
            catch (Exception)
            {

            }
        }

        public static DataTable getDBFromOtherServer_1(string server, string stored, string paramStr)
        {
            System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();

            HttpClientExtension _clientExtension = new HttpClientExtension();
            RunStoredViewModel objViewM = new RunStoredViewModel { server = server, stored = stored, paramStr = paramStr };
            string maychu331 = (string)settingsReader.GetValue("URL", typeof(String)) + "sqlQ/RunStored";
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(maychu331, objViewM); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return null;
            if (tbl.Rows.Count == 0)
                return null;
            return tbl;
        }
        public class RunStoredViewModel
        {
            public string server { get; set; }
            public string stored { get; set; }
            public string paramStr { get; set; }
        }
        private void treeListChuyen_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

        }
        public void SaveRoleUser_Line(string UserGetRole)
        {
            string urlSaveRoleUser_line = URL + "SystemRoleUser_Line/UpdateRoleLine_User?user=" + UserGetRole;
            DataTable dataUpdate = new DataTable();
            dataUpdate.Columns.Add("UserName");
            dataUpdate.Columns.Add("MaDVSX");
            dataUpdate.Columns.Add("LineID");
            DataTable tbl = (DataTable)treeListChuyen.DataSource;
            foreach (DataRow rowItem in tbl.Rows)
            {
                if ((bool)rowItem["IsCheck"] == true)
                {
                    DataRow row = dataUpdate.NewRow();
                    row["UserName"] = UserGetRole;
                    string rowItem_maDVSX = rowItem["MaDVSX"].ToString();
                    row["MaDVSX"] = rowItem_maDVSX;
                    row["LineID"] = rowItem["ID"].ToString().Replace(rowItem_maDVSX + "@", "");
                    dataUpdate.Rows.Add(row);
                }
            }
            string result = Task.Run(async () => { return await _serviceGetData.PostData(urlSaveRoleUser_line, dataUpdate); }).Result;
            //return result;
            //if (result.ToString().ToLower() != "true")
            //{
            //    MessageBox.Show("Lưu phân quyền chuyền thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void treeListChuyen_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            string _username = gvUser.GetFocusedRowCellValue(colUserName).ToString();
            string cellVlChange_maDVSX = treeListChuyen.GetFocusedRowCellValue(treeListChuyen_MaDVSX).ToString();
            string cellVlChange_LineID = treeListChuyen.GetFocusedRowCellValue(treeListChuyen_ID).ToString();
            bool cellVlChange_check = (bool)treeListChuyen.GetFocusedRowCellValue(treeListChuyen_IsCheck);
            string Url_Str = "";

            if (cellVlChange_maDVSX == cellVlChange_LineID)
            {
                DataTable tbl = (DataTable)treeListChuyen.DataSource;
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    if (tbl.Rows[i]["MaDVSX"].ToString() == cellVlChange_maDVSX)
                        tbl.Rows[i]["IsCheck"] = e.Value;
                }
                treeListChuyen.DataSource = tbl;
                treeListChuyen.RefreshDataSource();
            }
        }
        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view != null)
            {
                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "AllowView")
                    {
                        DataTable _dtSave = new DataTable();
                        string url = string.Format("{0}?userId={1}&&ModuleId={2}&&allow={3}&&trangthai={4}", URL + "PhanQuyenDH/Post", _userId, row["ModuleID"], Convert.ToBoolean(e.Value), "view");
                        string msS = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                        if (msS.ToLower() != "true")
                        {
                            XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            clsWaitForm.ShowSuccessForm(this, 3000);
                    }
                    else
                    {
                        if (e.Column.FieldName == "AllowAdd")
                        {
                            DataTable _dtSave = new DataTable();
                            string url = string.Format("{0}?userId={1}&&ModuleId={2}&&allow={3}&&trangthai={4}", URL + "PhanQuyenDH/Post", _userId, row["ModuleID"], Convert.ToBoolean(e.Value), "add");
                            string msS = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                            if (msS.ToLower() != "true")
                            {
                                XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                                clsWaitForm.ShowSuccessForm(this, 3000);
                        }
                        else
                        {
                            if (e.Column.FieldName == "AllowEdit")
                            {
                                DataTable _dtSave = new DataTable();
                                string url = string.Format("{0}?userId={1}&&ModuleId={2}&&allow={3}&&trangthai={4}", URL + "PhanQuyenDH/Post", _userId, row["ModuleID"], Convert.ToBoolean(e.Value), "edit");
                                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                                if (msS.ToLower() != "true")
                                {
                                    XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                    clsWaitForm.ShowSuccessForm(this, 3000);
                            }
                            else
                            {
                                if (e.Column.FieldName == "AllowDelete")
                                {
                                    DataTable _dtSave = new DataTable();
                                    string url = string.Format("{0}?userId={1}&&ModuleId={2}&&allow={3}&&trangthai={4}", URL + "PhanQuyenDH/Post", _userId, row["ModuleID"], Convert.ToBoolean(e.Value), "delete");
                                    string msS = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                                    if (msS.ToLower() != "true")
                                    {
                                        XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                        clsWaitForm.ShowSuccessForm(this, 3000);
                                }

                            }

                        }

                    }

                }

            }

        }

        private void gridView2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "MaNV")
                {
                    GridView view = sender as GridView;
                    if (view != null)
                    {
                        DataTable nv = gridControl2.DataSource as DataTable;
                        bool isDuplicate = nv.AsEnumerable().Any(row => row["MaNV"]?.ToString() == e.Value?.ToString());

                        if (isDuplicate)
                        {
                            XtraMessageBox.Show("Nhân Viên Đã Được Chọn Trong Một User Khác!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            view.CancelUpdateCurrentRow();
                            LoadNV();
                            return;
                        }
                        else
                        {
                            DataRow row = view.GetDataRow(view.FocusedRowHandle);
                            string UserID = row["UserName"]?.ToString();
                            DataTable _dtSave = new DataTable();
                            string url = string.Format("{0}?userId={1}&&manv={2}", URL + "PhanQuyenDH/PostNV", UserID, e.Value.ToString());
                            string msS = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                            if (msS.ToLower() != "true")
                            {
                                XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                                clsWaitForm.ShowSuccessForm(this, 3000);
                            LoadUserNV();
                        }

                    }
                }
            }
            catch (Exception r)
            {
                return;
            }

        }

        private void gridView3_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view != null)
            {
                if (e.Column.FieldName == "AllowEdit")
                {
                    int focusedRowHandle = gridView3.FocusedRowHandle;
                    SystemUserDuyetEntity itemFocus = gridView3.GetRow(focusedRowHandle) as SystemUserDuyetEntity;

                    List<SystemUserDuyetConfigEntity> items = new List<SystemUserDuyetConfigEntity>();
                    SystemUserDuyetConfigEntity item = new SystemUserDuyetConfigEntity();

                    item.Pid = 0;
                    item.UserID = _userId;
                    item.AllowEdit = Convert.ToBoolean(e.Value);
                    item.AllowAdd = Convert.ToBoolean(itemFocus.AllowAdd);
                    item.AllowActive = Convert.ToBoolean(itemFocus.AllowActive);
                    items.Add(item);
                    string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserDuyet + "/Post?", items); }).Result;
                    if (string.Compare(msS, "True") != 0)
                        XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        clsWaitForm.ShowSuccessForm(this, 3000);
                }
                if (e.Column.FieldName == "AllowAdd")
                {
                    int focusedRowHandle = gridView3.FocusedRowHandle;
                    SystemUserDuyetEntity itemFocus = gridView3.GetRow(focusedRowHandle) as SystemUserDuyetEntity;


                    List<SystemUserDuyetConfigEntity> items = new List<SystemUserDuyetConfigEntity>();
                    SystemUserDuyetConfigEntity item = new SystemUserDuyetConfigEntity();

                    item.Pid = 0;
                    item.UserID = _userId;
                    item.AllowEdit = Convert.ToBoolean(itemFocus.AllowEdit);
                    item.AllowAdd = Convert.ToBoolean(e.Value);
                    item.AllowActive = Convert.ToBoolean(itemFocus.AllowActive);
                    items.Add(item);
                    string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserDuyet + "/Post?", items); }).Result;
                    if (string.Compare(msS, "True") != 0)
                        XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        clsWaitForm.ShowSuccessForm(this, 3000);
                }
                if (e.Column.FieldName == "AllowActive")
                {
                    int focusedRowHandle = gridView3.FocusedRowHandle;
                    SystemUserDuyetEntity itemFocus = gridView3.GetRow(focusedRowHandle) as SystemUserDuyetEntity;


                    List<SystemUserDuyetConfigEntity> items = new List<SystemUserDuyetConfigEntity>();
                    SystemUserDuyetConfigEntity item = new SystemUserDuyetConfigEntity();

                    item.Pid = 0;
                    item.UserID = _userId;
                    item.AllowEdit = Convert.ToBoolean(itemFocus.AllowEdit);
                    item.AllowAdd = Convert.ToBoolean(itemFocus.AllowAdd);
                    item.AllowActive = Convert.ToBoolean(e.Value);
                    items.Add(item);
                    string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserDuyet + "/Post?", items); }).Result;
                    if (string.Compare(msS, "True") != 0)
                        XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        clsWaitForm.ShowSuccessForm(this, 3000);
                }
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }


        private void repositoryItemButtonEdit1_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }
        private DataTable CreateTableSave()
        {
            DataTable _tblCreate = new DataTable();
            _tblCreate.Columns.Add("ID", typeof(int));
            _tblCreate.Columns.Add("MaNV", typeof(string));
            _tblCreate.Columns.Add("TenNV", typeof(string));
            _tblCreate.Columns.Add("LyDo", typeof(string));
            _tblCreate.Columns.Add("NguoiGo", typeof(string));
            _tblCreate.Columns.Add("UserID", typeof(string));
            _tblCreate.Columns.Add("NgayGo", typeof(DateTime));
            return _tblCreate;
        }
        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

            if (e.Column == gridColumn8)
            {
                GridView view = sender as GridView;
                if (view != null)
                {

                    DataRow row = gridView2.GetFocusedDataRow();
                    string Manv = row["MaNV"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(Manv)) return;

                    string lyDo = row["LyDo"]?.ToString();
                    if (string.IsNullOrWhiteSpace(lyDo))
                    {
                        lyDo = GetInputFromMessageBox("Nhập lý do", "Xin vui lòng nhập lý do để gỡ:");
                        if (lyDo == null) // Người dùng nhấn "X"
                        {
                            return; // Đóng form mà không thông báo gì cả
                        }
                        else if (string.IsNullOrWhiteSpace(lyDo))
                        {
                            XtraMessageBox.Show("Bạn chưa nhập lý do!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        row["LyDo"] = lyDo; // Cập nhật lý do vào DataRow
                    }
                    DataTable _tblSave = CreateTableSave();
                    string Userid = row["UserName"]?.ToString() ?? "";
                    string Tennv = row["TenNV"]?.ToString() ?? "";
                    //DataTable _dtSave = new DataTable();
                    DataRow rowInsert = _tblSave.NewRow();
                    rowInsert["ID"] = 0;
                    rowInsert["MaNV"] = Manv;
                    rowInsert["TenNV"] = row["TenNV"]?.ToString() ?? "";
                    rowInsert["LyDo"] = lyDo;
                    rowInsert["NguoiGo"] = GlobleData.UserName;
                    rowInsert["UserID"] = Userid;
                    rowInsert["NgayGo"] = DateTime.Now;
                    _tblSave.Rows.Add(rowInsert);
                    string url = string.Format("{0}?userId={1}&&manv={2}", URL + "PhanQuyenDH/GONV", Userid, Manv);
                    string msS = Task.Run(async () => { return await _clientExtension.PostAsync(url, _tblSave); }).Result;
                    if (msS.ToLower() != "true")
                    {
                        XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadUserNV();

                }
            }
        }
        private string GetInputFromMessageBox(string title, string prompt)
        {
            Form promptForm = new Form
            {
                Width = 400,
                Height = 150,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label
            {
                Left = 20,
                Top = 20,
                Text = prompt,
                AutoSize = true
            };

            TextBox inputBox = new TextBox
            {
                Left = 20,
                Top = 50,
                Width = 350
            };

            Button confirmation = new Button
            {
                Text = "OK",
                Left = 250,
                Width = 100,
                Top = 80,
                DialogResult = DialogResult.OK
            };

            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);
            promptForm.Controls.Add(confirmation);
            promptForm.AcceptButton = confirmation;

            return promptForm.ShowDialog() == DialogResult.OK ? inputBox.Text.Trim() : null;
        }

        private void gridView4_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


        }
        private async void LoadUserXacNhanBOM(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenXacNhanBOM/Get", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserXacNhanBomModel> listXN = JsonConvert.DeserializeObject<List<SystemUserXacNhanBomModel>>(json);
            gridControl4.DataSource = listXN;
            gridControl4.Refresh();
        }

        private void gridView4_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AllowAdd")
            {
                List<SystemUserXacNhanBomModel> items = gridControl4.DataSource as List<SystemUserXacNhanBomModel>;
                List<SystemUserXacNhanBomConfigViewModel> lst = new List<SystemUserXacNhanBomConfigViewModel>();
                int focusedRowHandle = gridView4.FocusedRowHandle;
                SystemUserXacNhanBomModel itemFocus = gridView4.GetRow(focusedRowHandle) as SystemUserXacNhanBomModel;
                SystemUserXacNhanBomConfigViewModel item = new SystemUserXacNhanBomConfigViewModel();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = Convert.ToBoolean(e.Value);
                item.AllowDelete = itemFocus.AllowDelete;
                item.isNPL = itemFocus.isNPL;
                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                lst.Add(item);
                foreach (SystemUserXacNhanBomModel obj in items)
                {
                    if (obj.isNPL != itemFocus.isNPL)
                    {
                        SystemUserXacNhanBomConfigViewModel item2 = new SystemUserXacNhanBomConfigViewModel();
                        item2.Pid = 0;
                        item2.UserID = _userId;
                        item2.AllowEdit = obj.AllowEdit;
                        item2.AllowAdd = obj.AllowAdd;
                        item2.AllowDelete = obj.AllowDelete;
                        item2.isNPL = obj.isNPL;
                        item2.TenChucNang = obj.TenChucNang;
                        item2.AllowView = obj.AllowView;
                        lst.Add(item2);
                    }

                }

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserXacNhanBOM + "/Post?", lst); }).Result;
                if (string.Compare(msS, "True") != 0)
                    XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }
            else if (e.Column.FieldName == "AllowEdit")
            {
                List<SystemUserXacNhanBomModel> items = gridControl4.DataSource as List<SystemUserXacNhanBomModel>;
                List<SystemUserXacNhanBomConfigViewModel> lst = new List<SystemUserXacNhanBomConfigViewModel>();
                int focusedRowHandle = gridView4.FocusedRowHandle;
                SystemUserXacNhanBomModel itemFocus = gridView4.GetRow(focusedRowHandle) as SystemUserXacNhanBomModel;
                SystemUserXacNhanBomConfigViewModel item = new SystemUserXacNhanBomConfigViewModel();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = Convert.ToBoolean(e.Value);
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.isNPL = itemFocus.isNPL;
                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                lst.Add(item);
                foreach (SystemUserXacNhanBomModel obj in items)
                {
                    if (obj.isNPL != itemFocus.isNPL)
                    {
                        SystemUserXacNhanBomConfigViewModel item2 = new SystemUserXacNhanBomConfigViewModel();
                        item2.Pid = 0;
                        item2.UserID = _userId;
                        item2.AllowEdit = obj.AllowEdit;
                        item2.AllowAdd = obj.AllowAdd;
                        item2.AllowDelete = obj.AllowDelete;
                        item2.isNPL = obj.isNPL;
                        item2.TenChucNang = obj.TenChucNang;
                        item2.AllowView = obj.AllowView;
                        lst.Add(item2);
                    }

                }

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserXacNhanBOM + "/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowDelete")
            {
                List<SystemUserXacNhanBomModel> items = gridControl4.DataSource as List<SystemUserXacNhanBomModel>;
                List<SystemUserXacNhanBomConfigViewModel> lst = new List<SystemUserXacNhanBomConfigViewModel>();
                int focusedRowHandle = gridView4.FocusedRowHandle;
                SystemUserXacNhanBomModel itemFocus = gridView4.GetRow(focusedRowHandle) as SystemUserXacNhanBomModel;
                SystemUserXacNhanBomConfigViewModel item = new SystemUserXacNhanBomConfigViewModel();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = Convert.ToBoolean(e.Value);
                item.isNPL = itemFocus.isNPL;
                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                lst.Add(item);
                foreach (SystemUserXacNhanBomModel obj in items)
                {
                    if (obj.isNPL != itemFocus.isNPL)
                    {
                        SystemUserXacNhanBomConfigViewModel item2 = new SystemUserXacNhanBomConfigViewModel();
                        item2.Pid = 0;
                        item2.UserID = _userId;
                        item2.AllowEdit = obj.AllowEdit;
                        item2.AllowAdd = obj.AllowAdd;
                        item2.AllowDelete = obj.AllowDelete;
                        item2.isNPL = obj.isNPL;
                        item2.TenChucNang = obj.TenChucNang;
                        item2.AllowView = obj.AllowView;
                        lst.Add(item2);
                    }

                }

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserXacNhanBOM + "/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowView")
            {
                List<SystemUserXacNhanBomModel> items = gridControl4.DataSource as List<SystemUserXacNhanBomModel>;
                List<SystemUserXacNhanBomConfigViewModel> lst = new List<SystemUserXacNhanBomConfigViewModel>();
                int focusedRowHandle = gridView4.FocusedRowHandle;
                SystemUserXacNhanBomModel itemFocus = gridView4.GetRow(focusedRowHandle) as SystemUserXacNhanBomModel;
                SystemUserXacNhanBomConfigViewModel item = new SystemUserXacNhanBomConfigViewModel();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.isNPL = itemFocus.isNPL;
                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = Convert.ToBoolean(e.Value);
                lst.Add(item);
                foreach (SystemUserXacNhanBomModel obj in items)
                {
                    if (obj.isNPL != itemFocus.isNPL)
                    {
                        SystemUserXacNhanBomConfigViewModel item2 = new SystemUserXacNhanBomConfigViewModel();
                        item2.Pid = 0;
                        item2.UserID = _userId;
                        item2.AllowEdit = obj.AllowEdit;
                        item2.AllowAdd = obj.AllowAdd;
                        item2.AllowDelete = obj.AllowDelete;
                        item2.isNPL = obj.isNPL;
                        item2.TenChucNang = obj.TenChucNang;
                        item2.AllowView = obj.AllowView;
                        lst.Add(item2);
                    }

                }

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + ResourceURL.UrlUserXacNhanBOM + "/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
        }

        private async void LoadPOMHPermissions(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl5.DataSource = tblpomh;
            gridControl5.Refresh();
        }
        private bool _isPosting = false;
        private void gridView5_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


        }

        private void gridView5_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_isPosting) return;
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;
            DataRow row = view.GetDataRow(e.RowHandle);
            if (row == null) return;

            // Chỉ xử lý các cột quyền
            if (!new[] { "AllowDuyet", "AllowXacNhan", "AllowDuyet1", "AllowDuyet2" }
                .Contains(e.Column.FieldName))
                return;

            try
            {
                _isPosting = true;
                row[e.Column.FieldName] = e.Value;

                // Tạo DataTable gửi lên API
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("ID", typeof(int));
                dtSave.Columns.Add("UserID", typeof(string));
                dtSave.Columns.Add("AllowDuyet", typeof(bool));
                dtSave.Columns.Add("AllowXacNhan", typeof(bool));
                dtSave.Columns.Add("ModuleID", typeof(string));
                dtSave.Columns.Add("AllowDuyet1", typeof(bool));
                dtSave.Columns.Add("AllowDuyet2", typeof(bool));

                DataRow r = dtSave.NewRow();
                r["ID"] = 0;
                r["UserID"] = _userId;
                r["AllowDuyet"] = Convert.ToBoolean(row["AllowDuyet"]);
                r["AllowXacNhan"] = Convert.ToBoolean(row["AllowXacNhan"]);
                r["ModuleID"] = row["ModuleID"];
                r["AllowDuyet1"] = Convert.ToBoolean(row["AllowDuyet1"]);
                r["AllowDuyet2"] = Convert.ToBoolean(row["AllowDuyet2"]);
                dtSave.Rows.Add(r);

                string url = string.Format("{0}?", URL + "PhanQuyenPOMH/Post");
                string mss = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, dtSave);
                }).Result;

                if (mss.ToLower() != "true")
                {
                    XtraMessageBox.Show(mss, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }
            finally
            {
                _isPosting = false;
            }
        }

        private async void LoadPOMHPWEBermissions(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Getweb", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl6.DataSource = tblpomh;
            gridControl6.Refresh();
        }
        private bool _isPosting1 = false;
        private void gridView6_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_isPosting1) return;
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;
            DataRow row = view.GetDataRow(e.RowHandle);
            if (row == null) return;

            // Chỉ xử lý các cột quyền
            if (!new[] { "AllowXacNhan", "AllowKy", "AllowDelete", "AllowKyGD" }
                .Contains(e.Column.FieldName))
                return;

            try
            {
                _isPosting1 = true;

                // Cập nhật giá trị mới vào row (quan trọng)
                row[e.Column.FieldName] = e.Value;

                // Tạo DataTable gửi lên API
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("ID", typeof(int));
                dtSave.Columns.Add("UserID", typeof(string));
                dtSave.Columns.Add("AllowXacNhan", typeof(bool));
                dtSave.Columns.Add("AllowKy", typeof(bool));
                dtSave.Columns.Add("AllowDelete", typeof(bool));
                dtSave.Columns.Add("ModuleID", typeof(string));
                dtSave.Columns.Add("AllowKyGD", typeof(bool));

                DataRow r = dtSave.NewRow();
                r["ID"] = 0;
                r["UserID"] = _userId;
                r["AllowXacNhan"] = Convert.ToBoolean(row["AllowXacNhan"]);
                r["AllowKy"] = Convert.ToBoolean(row["AllowKy"]);
                r["AllowDelete"] = Convert.ToBoolean(row["AllowDelete"]);
                r["ModuleID"] = row["ModuleID"];
                r["AllowKyGD"] = Convert.ToBoolean(row["AllowKyGD"]);
                dtSave.Rows.Add(r);

                string url = string.Format("{0}?", URL + "PhanQuyenPOMH/Postweb");
                string mss = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, dtSave);
                }).Result;

                if (mss.ToLower() != "true")
                {
                    XtraMessageBox.Show(mss, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }
            finally
            {
                _isPosting1 = false;
            }
        }

        private void gridView5_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName != "AllowDuyet1" &&
                view.FocusedColumn.FieldName != "AllowDuyet2")
                return;

            string moduleID = view.GetRowCellValue(view.FocusedRowHandle, "ModuleID")?.ToString();

            if (moduleID != "M.12.03.00")
            {
                e.Cancel = true; // KHÓA chỉnh sửa
            }
        }

        private void gridView5_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName != "AllowDuyet1" &&
            e.Column.FieldName != "AllowDuyet2")
                return;

            GridView view = sender as GridView;

            string moduleID = view.GetRowCellValue(e.RowHandle, "ModuleID")?.ToString();

            if (moduleID != "M.12.03.00")
            {
                e.Appearance.ForeColor = Color.Gray;
                e.Appearance.BackColor = Color.Gainsboro;
            }
        }

        private void gridView5_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            if (!hitInfo.InRowCell) return;

            if (hitInfo.Column.FieldName == "AllowDuyet1" ||
                hitInfo.Column.FieldName == "AllowDuyet2")
            {
                string moduleID = view.GetRowCellValue(hitInfo.RowHandle, "ModuleID")?.ToString();
                if (moduleID != "M.12.03.00")
                {
                    return;
                }
            }
        }

        #region Phú excel TS
        private async void LoadUserExcelTS(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenExcelTS/Get", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserExcelTSModel> listXN = JsonConvert.DeserializeObject<List<SystemUserExcelTSModel>>(json);
            gridControl7.DataSource = listXN;
            gridControl7.Refresh();
        }
        private void gridView7_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AllowExcel")
            {
                List<SystemUserExcelTSModel> items = gridControl7.DataSource as List<SystemUserExcelTSModel>;
                List<SystemUserExcelTSConfigViewModel> lst = new List<SystemUserExcelTSConfigViewModel>();
                int focusedRowHandle = gridView4.FocusedRowHandle;
                SystemUserExcelTSModel itemFocus = gridView7.GetRow(focusedRowHandle) as SystemUserExcelTSModel;
                SystemUserExcelTSConfigViewModel item = new SystemUserExcelTSConfigViewModel();
                item.Pid = 0;
                item.UserID = _userId;

                item.AllowExcel = Convert.ToBoolean(e.Value);

                lst.Add(item);


                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenExcelTS/Post?", lst); }).Result;
                if (string.Compare(msS, "True") != 0)
                    XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }
        #endregion

        private void gridView6_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName != "AllowKy" &&
                view.FocusedColumn.FieldName != "AllowKyGD")
                return;

            string moduleID = view.GetRowCellValue(view.FocusedRowHandle, "ModuleID")?.ToString();

            if (moduleID != "M.21.00.00")
            {
                e.Cancel = true; // KHÓA chỉnh sửa
            }
        }

        private void gridView6_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName != "AllowKy" &&
                e.Column.FieldName != "AllowKyGD")
                return;

            GridView view = sender as GridView;

            string moduleID = view.GetRowCellValue(e.RowHandle, "ModuleID")?.ToString();

            if (moduleID != "M.21.00.00")
            {
                e.Appearance.ForeColor = Color.Gray;
                e.Appearance.BackColor = Color.Gainsboro;
            }
        }

        private void gridView6_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            if (!hitInfo.InRowCell) return;

            if (hitInfo.Column.FieldName == "AllowKy" ||
                hitInfo.Column.FieldName == "AllowKyGD")
            {
                string moduleID = view.GetRowCellValue(hitInfo.RowHandle, "ModuleID")?.ToString();
                if (moduleID != "M.21.00.00")
                {
                    return;
                }
            }
        }


        #region Phú phân quyền yêu cầu NPL
        private async void LoadUserPQYeuCauNPL(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenYCNPL/Get", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserYeuCauNPLEntity> listXN = JsonConvert.DeserializeObject<List<SystemUserYeuCauNPLEntity>>(json);
            gridControl8.DataSource = listXN;
            gridControl8.Refresh();
        }
        #endregion

        private void gridView8_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AllowAdd")
            {
                List<SystemUserYeuCauNPLEntity> items = gridControl8.DataSource as List<SystemUserYeuCauNPLEntity>;
                List<SystemUserYeuCauNPLConfigViewEntity> lst = new List<SystemUserYeuCauNPLConfigViewEntity>();
                int focusedRowHandle = gridView8.FocusedRowHandle;
                SystemUserYeuCauNPLEntity itemFocus = gridView8.GetRow(focusedRowHandle) as SystemUserYeuCauNPLEntity;
                SystemUserYeuCauNPLConfigViewEntity item = new SystemUserYeuCauNPLConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = Convert.ToBoolean(e.Value);
                item.AllowDelete = itemFocus.AllowDelete;

                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                item.AllowSoatXet = itemFocus.AllowSoatXet;
                item.AllowDuyet = itemFocus.AllowDuyet;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenYCNPL/Post?", lst); }).Result;
                if (string.Compare(msS, "True") != 0)
                    XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }
            else if (e.Column.FieldName == "AllowEdit")
            {
                List<SystemUserYeuCauNPLEntity> items = gridControl8.DataSource as List<SystemUserYeuCauNPLEntity>;
                List<SystemUserYeuCauNPLConfigViewEntity> lst = new List<SystemUserYeuCauNPLConfigViewEntity>();
                int focusedRowHandle = gridView8.FocusedRowHandle;
                SystemUserYeuCauNPLEntity itemFocus = gridView8.GetRow(focusedRowHandle) as SystemUserYeuCauNPLEntity;
                SystemUserYeuCauNPLConfigViewEntity item = new SystemUserYeuCauNPLConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = Convert.ToBoolean(e.Value);
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;

                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                item.AllowSoatXet = itemFocus.AllowSoatXet;
                item.AllowDuyet = itemFocus.AllowDuyet;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenYCNPL/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowDelete")
            {
                List<SystemUserYeuCauNPLEntity> items = gridControl8.DataSource as List<SystemUserYeuCauNPLEntity>;
                List<SystemUserYeuCauNPLConfigViewEntity> lst = new List<SystemUserYeuCauNPLConfigViewEntity>();
                int focusedRowHandle = gridView8.FocusedRowHandle;
                SystemUserYeuCauNPLEntity itemFocus = gridView8.GetRow(focusedRowHandle) as SystemUserYeuCauNPLEntity;
                SystemUserYeuCauNPLConfigViewEntity item = new SystemUserYeuCauNPLConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = Convert.ToBoolean(e.Value);

                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                item.AllowSoatXet = itemFocus.AllowSoatXet;
                item.AllowDuyet = itemFocus.AllowDuyet;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenYCNPL/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowView")
            {
                List<SystemUserYeuCauNPLEntity> items = gridControl8.DataSource as List<SystemUserYeuCauNPLEntity>;
                List<SystemUserYeuCauNPLConfigViewEntity> lst = new List<SystemUserYeuCauNPLConfigViewEntity>();
                int focusedRowHandle = gridView8.FocusedRowHandle;
                SystemUserYeuCauNPLEntity itemFocus = gridView8.GetRow(focusedRowHandle) as SystemUserYeuCauNPLEntity;
                SystemUserYeuCauNPLConfigViewEntity item = new SystemUserYeuCauNPLConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;

                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = Convert.ToBoolean(e.Value);
                item.AllowSoatXet = itemFocus.AllowSoatXet;
                item.AllowDuyet = itemFocus.AllowDuyet;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenYCNPL/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowSoatXet")
            {
                List<SystemUserYeuCauNPLEntity> items = gridControl8.DataSource as List<SystemUserYeuCauNPLEntity>;
                List<SystemUserYeuCauNPLConfigViewEntity> lst = new List<SystemUserYeuCauNPLConfigViewEntity>();
                int focusedRowHandle = gridView8.FocusedRowHandle;
                SystemUserYeuCauNPLEntity itemFocus = gridView8.GetRow(focusedRowHandle) as SystemUserYeuCauNPLEntity;
                SystemUserYeuCauNPLConfigViewEntity item = new SystemUserYeuCauNPLConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;

                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                item.AllowSoatXet = Convert.ToBoolean(e.Value);
                item.AllowDuyet = itemFocus.AllowDuyet;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenYCNPL/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowDuyet")
            {
                List<SystemUserYeuCauNPLEntity> items = gridControl8.DataSource as List<SystemUserYeuCauNPLEntity>;
                List<SystemUserYeuCauNPLConfigViewEntity> lst = new List<SystemUserYeuCauNPLConfigViewEntity>();
                int focusedRowHandle = gridView8.FocusedRowHandle;
                SystemUserYeuCauNPLEntity itemFocus = gridView8.GetRow(focusedRowHandle) as SystemUserYeuCauNPLEntity;
                SystemUserYeuCauNPLConfigViewEntity item = new SystemUserYeuCauNPLConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;

                item.TenChucNang = itemFocus.TenChucNang;
                item.AllowView = itemFocus.AllowView;
                item.AllowSoatXet = itemFocus.AllowSoatXet;
                item.AllowDuyet = Convert.ToBoolean(e.Value);
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenYCNPL/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
        }
        //WIP_DONHANG
        private Dictionary<int, bool> _groupCheckedState = new Dictionary<int, bool>();
        private async void LoadPhanQuyenWIP(string userId, int lineX = 0)
        {
            try
            {
                string url = string.Format("{0}?action=GET&userID={1}&lineX={2}", URL + "PhanQuyenWIP/GetData", userId, lineX);
                string json = await _clientExtension.GetAsnyc(url);
                List<WipPermissionModel> listData = JsonConvert.DeserializeObject<List<WipPermissionModel>>(json);

                // BIND TreeList
                BindTreeWip(listData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi LoadPhanQuyenWIP: " + ex.Message);
            }
        }
        private async void LoadPhanQuyenChucNangWIP(string userId)
        {
            try
            {
                string url = string.Format("{0}?action=GET_USER_FEATURES&userID={1}", URL + "PhanQuyenWIP/Feature", userId);
                string json = await _clientExtension.GetAsnyc(url);
                List<WipPermissionFeatureModel> listData = JsonConvert.DeserializeObject<List<WipPermissionFeatureModel>>(json);
                grdFeatureWip.DataSource = listData;
                grdFeatureWip.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi LoadPhanQuyenWIP: " + ex.Message);
            }
        }

        private void BindTreeWip(List<WipPermissionModel> listData)
        {
            if (listData == null)
                listData = new List<WipPermissionModel>();

            var nodes = new List<WipPermNode>();
            int id = 1;

            // GroupName -> parentId
            var groupMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var r in listData.OrderBy(x => x.SortOrder))
            {
                string g = (r.GroupName ?? "").Trim();

                if (!groupMap.TryGetValue(g, out int parentId))
                {
                    parentId = id++;
                    groupMap[g] = parentId;

                    nodes.Add(new WipPermNode
                    {
                        Id = parentId,
                        ParentId = null,
                        GroupName = g,
                        ColName = g,     // hiển thị nhóm ở cột ColName
                        IsGroup = true,
                        SortOrder = r.SortOrder
                    });
                }

                nodes.Add(new WipPermNode
                {
                    Id = id++,
                    ParentId = parentId,
                    GroupName = g,
                    ColKey = r.ColKey,
                    ColName = r.Title,
                    IsCheckXem = r.IsCheckXem,
                    IsCheckSua = r.IsCheckSua,
                    IsGroup = false,
                    SortOrder = r.SortOrder
                });
            }

            treeListWIP.DataSource = nodes;
            //treeListWIP.ExpandAll();
            RecalcAllGroups("IsCheckXem");
            RecalcAllGroups("IsCheckSua");
        }
        private void RecalcAllGroups(string field)
        {
            treeListWIP.BeginUpdate();
            try
            {
                foreach (DevExpress.XtraTreeList.Nodes.TreeListNode root in treeListWIP.Nodes)
                {
                    RecalcGroupNodeRecursive(root, field);
                }
            }
            finally
            {
                treeListWIP.EndUpdate();
            }
        }

        private void RecalcGroupNodeRecursive(DevExpress.XtraTreeList.Nodes.TreeListNode node, string field)
        {
            bool isGroup = Convert.ToBoolean(node.GetValue("IsGroup"));

            // đi sâu trước (nếu bạn có nhiều level)
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode ch in node.Nodes)
                RecalcGroupNodeRecursive(ch, field);

            if (!isGroup) return;

            // tính trạng thái cha theo con (chỉ xét node con IsGroup=false)
            int total = 0, checkedCount = 0;

            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode ch in node.Nodes)
            {
                bool childIsGroup = Convert.ToBoolean(ch.GetValue("IsGroup"));
                if (childIsGroup) continue;

                total++;
                if (Convert.ToBoolean(ch.GetValue(field))) checkedCount++;
            }

            node.SetValue(field, total > 0 && checkedCount == total);
        }
        private void treeListWIP_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "IsCheckXem" && e.Column.FieldName != "IsCheckSua")
                return;

            var node = e.Node;
            if (node == null) return;

            bool newValue = Convert.ToBoolean(e.Value);
            bool isGroup = Convert.ToBoolean(node.GetValue("IsGroup"));

            // set giá trị ngay
            node.SetValue(e.Column, newValue);

            if (isGroup)
            {
                SetChildrenValue(node, e.Column.FieldName, newValue);

                SaveAllChildren(node, e.Column.FieldName, newValue);
            }
            else
            {
                string colKey = Convert.ToString(node.GetValue("ColKey"));
                SavePermission(colKey, e.Column.FieldName, newValue);

                // Update trạng thái nhóm (optional)
                UpdateParentState(node.ParentNode, e.Column.FieldName);
            }
        }
        private async void gvFeatureWip_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "IsAllow")
                return;

            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

            string featureKey = Convert.ToString(view.GetRowCellValue(e.RowHandle, "FeatureKey"));
            bool newValue = Convert.ToBoolean(view.GetRowCellValue(e.RowHandle, "IsAllow"));

            try
            {
                await SaveFeature(featureKey, newValue);
            }
            catch (Exception ex)
            {
                view.CellValueChanged -= gvFeatureWip_CellValueChanged;
                view.SetRowCellValue(e.RowHandle, e.Column, !newValue);
                view.CellValueChanged += gvFeatureWip_CellValueChanged;

                DevExpress.XtraEditors.XtraMessageBox.Show("Lưu thất bại: " + ex.Message);
            }
        }
        private void SetChildrenValue(DevExpress.XtraTreeList.Nodes.TreeListNode parent, string field, bool val)
        {
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode ch in parent.Nodes)
            {
                bool isGroup = Convert.ToBoolean(ch.GetValue("IsGroup"));
                ch.SetValue(field, val);

                if (isGroup) SetChildrenValue(ch, field, val);
            }
        }
        private void SaveAllChildren(DevExpress.XtraTreeList.Nodes.TreeListNode parent, string field, bool val)
        {
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode ch in parent.Nodes)
            {
                bool isGroup = Convert.ToBoolean(ch.GetValue("IsGroup"));
                if (isGroup) { SaveAllChildren(ch, field, val); continue; }

                string colKey = Convert.ToString(ch.GetValue("ColKey"));
                SavePermission(colKey, field, val);
            }
        }

        private void UpdateParentState(DevExpress.XtraTreeList.Nodes.TreeListNode parent, string field)
        {
            if (parent == null) return;

            int total = 0, checkedCount = 0;
            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode ch in parent.Nodes)
            {
                bool isGroup = Convert.ToBoolean(ch.GetValue("IsGroup"));
                if (isGroup) continue;

                total++;
                if (Convert.ToBoolean(ch.GetValue(field))) checkedCount++;
            }

            parent.SetValue(field, total > 0 && checkedCount == total);
        }
        private void SavePermission(string colKey, string fieldName, bool newValue)
        {
            string paramView = (fieldName == "IsCheckXem") ? newValue.ToString() : "";
            string paramEdit = (fieldName == "IsCheckSua") ? newValue.ToString() : "";
            int lineX = GetFocusedWipLineX();

            string url = string.Format("{0}?action=SAVE&userID={1}&lineX={2}&colKey={3}&isView={4}&isEdit={5}",
                URL + "PhanQuyenWIP/SaveData",
                _userId,
                lineX,
                colKey,
                paramView,
                paramEdit);

            Task.Run(async () => { await _clientExtension.PostAsync(url, null); });
        }
        private async void LoadUserCosting(string username)
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenCosting/Get", username);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserCostingEntity> listCosting = JsonConvert.DeserializeObject<List<SystemUserCostingEntity>>(json);
            gridControl9.DataSource = listCosting;
            gridControl9.Refresh();
        }
        private async Task SaveFeature(string featureKey, bool newValue)
        {
            var request = new WipFeaturePermissionRequest
            {
                Action = "SAVE_USER_FEATURE",
                UserID = _userId,
                FeatureKey = featureKey,
                IsAllow = newValue
            };

            string url = URL + "PhanQuyenWIP/Feature";
            Task.Run(async () => { await _clientExtension.PostAsync(url, request); });
        }
        private void gvLineWip_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            _currentWipLineX = GetFocusedWipLineX();

            if (string.IsNullOrWhiteSpace(_userId))
                return;

            LoadPhanQuyenWIP(_userId, _currentWipLineX);
        }

        private void LoadLineWip(string userId = null)
        {
            try
            {
                string urlLineWip = string.Format("{0}", URL + "wip-donhang/get-list-linex");
                string jsonLineWip = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLineWip); }).Result;
                DataTable dtLineWip = JsonConvert.DeserializeObject<DataTable>(jsonLineWip);

                if (dtLineWip == null)
                    dtLineWip = CreateLineWipDataTable();

                if (!dtLineWip.Columns.Contains("Id"))
                    dtLineWip.Columns.Add("Id", typeof(int));

                if (!dtLineWip.Columns.Contains("Name"))
                    dtLineWip.Columns.Add("Name", typeof(string));
                DataRow drDefault = dtLineWip.NewRow();
                drDefault["Id"] = 0;
                drDefault["Name"] = "Tất cả";
                dtLineWip.Rows.InsertAt(drDefault, 0);
                gcLineWip.DataSource = dtLineWip;
                gcLineWip.RefreshDataSource();

                if (gvLineWip.RowCount > 0)
                    gvLineWip.FocusedRowHandle = 0;

                _currentWipLineX = GetFocusedWipLineX();

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    LoadPhanQuyenWIP(userId, _currentWipLineX);
                    LoadPhanQuyenChucNangWIP(userId);
                }
            }
            catch (Exception ex)
            {
                gcLineWip.DataSource = CreateLineWipDataTable();
                gcLineWip.RefreshDataSource();
                if (!string.IsNullOrWhiteSpace(userId))
                    LoadPhanQuyenChucNangWIP(userId);
                Console.WriteLine("Lỗi LoadLineWip: " + ex.Message);
            }
        }

        private DataTable CreateLineWipDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            return dt;
        }

        private int GetFocusedWipLineX()
        {
            object lineValue = gvLineWip.GetFocusedRowCellValue(colLineX);
            if (lineValue == null || lineValue == DBNull.Value)
                return 0;

            int lineX;
            return int.TryParse(lineValue.ToString(), out lineX) ? lineX : 0;
        }

        private void gridView9_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AllowAdd")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.FuncName = itemFocus.FuncName;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = Convert.ToBoolean(e.Value);
                item.AllowDelete = itemFocus.AllowDelete;
                item.AllowView = itemFocus.AllowView;
                item.AllowCreateApproval = itemFocus.AllowCreateApproval;
                item.AllowSupplierApproval = itemFocus.AllowSupplierApproval;
                item.AllowSMVApproval = itemFocus.AllowSMVApproval;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") != 0)
                    XtraMessageBox.Show(msS, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }
            else if (e.Column.FieldName == "AllowEdit")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.FuncName = itemFocus.FuncName;
                item.UserID = _userId;
                item.AllowEdit = Convert.ToBoolean(e.Value);
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.AllowView = itemFocus.AllowView;
                item.AllowCreateApproval = itemFocus.AllowCreateApproval;
                item.AllowSupplierApproval = itemFocus.AllowSupplierApproval;
                item.AllowSMVApproval = itemFocus.AllowSMVApproval;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowDelete")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.FuncName = itemFocus.FuncName;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = Convert.ToBoolean(e.Value);
                item.AllowView = itemFocus.AllowView;
                item.AllowCreateApproval = itemFocus.AllowCreateApproval;
                item.AllowSupplierApproval = itemFocus.AllowSupplierApproval;
                item.AllowSMVApproval = itemFocus.AllowSMVApproval;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowView")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.FuncName = itemFocus.FuncName;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.AllowView = Convert.ToBoolean(e.Value);
                item.AllowCreateApproval = itemFocus.AllowCreateApproval;
                item.AllowSupplierApproval = itemFocus.AllowSupplierApproval;
                item.AllowSMVApproval = itemFocus.AllowSMVApproval;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowCreateApproval")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.FuncName = itemFocus.FuncName;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.AllowView = itemFocus.AllowView;
                item.AllowCreateApproval = Convert.ToBoolean(e.Value);
                item.AllowSupplierApproval = itemFocus.AllowSupplierApproval;
                item.AllowSMVApproval = itemFocus.AllowSMVApproval;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowSupplierApproval")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.FuncName = itemFocus.FuncName;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.AllowView = itemFocus.AllowView;
                item.AllowCreateApproval = itemFocus.AllowCreateApproval;
                item.AllowSupplierApproval = Convert.ToBoolean(e.Value);
                item.AllowSMVApproval = itemFocus.AllowSMVApproval;
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else if (e.Column.FieldName == "AllowSMVApproval")
            {
                List<SystemUserCostingEntity> items = gridControl9.DataSource as List<SystemUserCostingEntity>;
                List<SystemUserCostingConfigViewEntity> lst = new List<SystemUserCostingConfigViewEntity>();
                int focusedRowHandle = gridView9.FocusedRowHandle;
                SystemUserCostingEntity itemFocus = gridView9.GetRow(focusedRowHandle) as SystemUserCostingEntity;
                SystemUserCostingConfigViewEntity item = new SystemUserCostingConfigViewEntity();
                item.Pid = 0;
                item.UserID = _userId;
                item.AllowEdit = itemFocus.AllowEdit;
                item.AllowAdd = itemFocus.AllowAdd;
                item.AllowDelete = itemFocus.AllowDelete;
                item.FuncName = itemFocus.FuncName;
                item.AllowView = itemFocus.AllowView;
                item.AllowCreateApproval = itemFocus.AllowCreateApproval;
                item.AllowSupplierApproval = itemFocus.AllowSupplierApproval;
                item.AllowSMVApproval = Convert.ToBoolean(e.Value);
                lst.Add(item);

                string msS = Task.Run(async () => { return await _clientExtension.PostAsync(URL + "PhanQuyenCosting/Post?", lst); }).Result;
                if (string.Compare(msS, "True") == 0)
                    clsWaitForm.ShowSuccessForm(this, 1000);
            }
        }

        private void tcModule_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page.Name == tpCosting.Name)
            {
                if (!string.IsNullOrEmpty(_userId))
                    LoadUserCosting(_userId);
            }
        }
    }
}

