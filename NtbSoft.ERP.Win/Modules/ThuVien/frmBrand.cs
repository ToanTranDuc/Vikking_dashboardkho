using DevExpress.DataAccess.Excel;
using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmBrand : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
            new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        //int _rowAdd = -1;
        List<int> _lstRowAdd = new List<int>();
        List<int> lstRowUpdate = new List<int>();
        BindingList<BrandEntity> lstBrandEntity = new BindingList<BrandEntity>();
        List<KhachHangEntity> lstKhachHangEntity = new List<KhachHangEntity>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        //ContextMenuStrip ctxMenu = new ContextMenuStrip();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        private int _maxBrandNum = -1;
        int FocusedIndex = 0;
        string _maKH = string.Empty;
        KeyDownControlHandler keyDownControlHandler;
        private int _pendingAddCount = 0;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        public frmBrand()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(string));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }
        public frmBrand(string maKH)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(string));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            _maKH = maKH;
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, _allowAdd || _allowEdit, ActionType.Save, Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave, actionControlRefresh };
            return lstActionControls;
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void CheckPermission()
        {
            string url = string.Format("{0}/GetPer?userID={1}",URL + ResourceURL.UrlUserModule, GlobleData.UserName);

            List<SystemUserModuleEntity> list =
                Task.Run(async () => await _serviceUserModule.UserModuleGet(url)).Result;

            SystemUserModuleEntity obj = list.FirstOrDefault(m => m.FormShow == this.Name);
            if (obj == null) return;

            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;

            if (!_allowAdd) Them.Enabled = false;
            if (!_allowEdit) { Sua.Enabled = false; Luu.Enabled = false; }
            else Luu.Enabled = false;
            if (!_allowDelete) Xoa.Enabled = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CheckPermission();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            tenNVHienThi = GetTenNhanVien();
            loadRepoKH();
            if (!string.IsNullOrEmpty(_maKH))
                loadCTBrand();
            else
                LoadDSBrand(false);
            gridBrand.ContextMenuStrip = null;
            //InitContextMenu();
        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridViewBrand.OptionsBehavior.Editable = false;
                    if (_allowAdd) { Them.Enabled = true; actionControlAdd.Enabled = true; }
                    if (_allowEdit) { Sua.Enabled = true; actionControlEdit.Enabled = true; }
                    if (_allowDelete) { Xoa.Enabled = true; actionControlDelete.Enabled = true; }
                    if (_allowAdd || _allowEdit)
                    { Luu.Enabled = false; actionControlSave.Enabled = false; }
                    break;

                case ResourceURL.EventStatus.Add:
                    gridViewBrand.OptionsBehavior.Editable = true;
                    if (_allowAdd) { Them.Enabled = true; actionControlAdd.Enabled = true; }
                    if (_allowEdit) { Sua.Enabled = false; actionControlEdit.Enabled = false; }
                    if (_allowDelete) { Xoa.Enabled = false; actionControlDelete.Enabled = false; }
                    if (_allowAdd || _allowEdit)
                    { Luu.Enabled = true; actionControlSave.Enabled = true; }
                    break;

                case ResourceURL.EventStatus.Edit:
                    gridViewBrand.OptionsBehavior.Editable = true;
                    if (_allowAdd) { Them.Enabled = false; actionControlAdd.Enabled = false; }
                    if (_allowEdit) { Sua.Enabled = false; actionControlEdit.Enabled = false; }
                    if (_allowDelete) { Xoa.Enabled = false; actionControlDelete.Enabled = false; }
                    if (_allowAdd || _allowEdit)
                    { Luu.Enabled = true; actionControlSave.Enabled = true; }
                    break;
            }
        }
        private void loadCTBrand()
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = $"{URL}Brand/Get?action=GETBRANDTHEOKH&para={_maKH}";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                if (!string.IsNullOrEmpty(json))
                    lstBrandEntity = JsonConvert.DeserializeObject<BindingList<BrandEntity>>(json);
                gridBrand.DataSource = lstBrandEntity;
                GridViewUpdateStatus(_status);
                colKH.OptionsColumn.AllowEdit = false;
                colKH.OptionsColumn.ReadOnly = true;
            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra lại!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadDSBrand(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = $"{URL}Brand/Get?action=Get";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                if (!string.IsNullOrEmpty(json))
                    lstBrandEntity = JsonConvert.DeserializeObject<BindingList<BrandEntity>>(json);
                gridBrand.DataSource = lstBrandEntity;
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewBrand.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridBrand;
                }
            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra lại!","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadRepoKH()
        {
            repoKH.DisplayMember = "TenKH";
            repoKH.ValueMember = "MaKH";
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
            repoKH.DataSource = tblChungLoaiChiTiet;
            colKH.ColumnEdit = repoKH;
            this.ActiveControl = button1;
        }
        //private void InitContextMenu()
        //{
        //    ctxMenu.Items.Clear();
        //    var mnuApDungKH = new ToolStripMenuItem("Áp dụng KH cho các dòng chọn");
        //    mnuApDungKH.Click += (s, e) => ApDungMaKHChoDongChon();
        //    var mnuXoaDong = new ToolStripMenuItem("Xóa dòng được chọn");
        //    mnuXoaDong.ForeColor = Color.Red;
        //    mnuXoaDong.Click += (s, e) => XoaDongChon();

        //    ctxMenu.Items.Add(mnuApDungKH);
        //    ctxMenu.Items.Add(new ToolStripSeparator());
        //    ctxMenu.Items.Add(mnuXoaDong);
        //    gridViewBrand.PopupMenuShowing += gridViewBrand_PopupMenuShowing;
        //}
        //private void InitContextMenu()
        //{
        //    ctxMenu.Items.Clear();
        //    var mnuApDungKH = new ToolStripMenuItem("Áp dụng KH cho các dòng chọn");
        //    mnuApDungKH.Click += (s, e) => ApDungMaKHChoDongChon();
        //    var mnuXoaDong = new ToolStripMenuItem("Xóa dòng được chọn");
        //    mnuXoaDong.ForeColor = Color.Red;
        //    mnuXoaDong.Click += (s, e) => XoaDongChon();

        //    ctxMenu.Items.Add(mnuApDungKH);
        //    ctxMenu.Items.Add(new ToolStripSeparator());
        //    ctxMenu.Items.Add(mnuXoaDong);
        //    gridBrand.MouseDown += GridBrand_MouseClick;
        //}

        //private void GridBrand_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button != MouseButtons.Right) return;
        //    GridHitInfo hitInfo = gridViewBrand.CalcHitInfo(e.Location);
        //    if (hitInfo.RowHandle < 0) return;

        //    if (gridViewBrand.FocusedRowHandle != hitInfo.RowHandle)
        //        gridViewBrand.FocusedRowHandle = hitInfo.RowHandle;

        //    bool isKHColumn = hitInfo.InRowCell && hitInfo.Column == colKH;
        //    bool isEditMode = _status == ResourceURL.EventStatus.Add
        //                   || _status == ResourceURL.EventStatus.Edit;

        //    ctxMenu.Items[0].Visible = isKHColumn && isEditMode;
        //    ctxMenu.Items[1].Visible = isKHColumn && isEditMode;
        //    ctxMenu.Items[2].Visible = _allowDelete;

        //    bool hasVisible = ctxMenu.Items.Cast<ToolStripItem>().Any(x => x.Visible);
        //    if (!hasVisible) return;

        //    ctxMenu.Show(gridBrand, e.Location);
        //}
        //private void ApDungMaKHChoDongChon()
        //{
        //    BrandEntity currentRow = gridViewBrand.GetRow(gridViewBrand.FocusedRowHandle) as BrandEntity;
        //    if (currentRow == null || string.IsNullOrEmpty(currentRow.MaKH))
        //    {
        //        XtraMessageBox.Show("Dòng hiện tại chưa có Khách Hàng để áp dụng!",
        //            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    int[] selectedRows = gridViewBrand.GetSelectedRows();
        //    if (selectedRows == null || selectedRows.Length == 0) return;

        //    foreach (int rowHandle in selectedRows)
        //    {
        //        BrandEntity item = gridViewBrand.GetRow(rowHandle) as BrandEntity;
        //        if (item == null) continue;

        //        item.MaKH = currentRow.MaKH;

        //        if (!lstRowUpdate.Contains(rowHandle))
        //            lstRowUpdate.Add(rowHandle);
        //    }

        //    gridViewBrand.RefreshData();
        //    XtraMessageBox.Show($"Đã áp dụng KH cho {selectedRows.Length} dòng!",
        //        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}
        private void ThemDong()
        {
            gridViewBrand.BeginUpdate();
            try
            {
                var obj = new BrandEntity();
                int maxNum = 0;
                if (_maxBrandNum < 0)
                {
                    try
                    {
                        string urlAll = $"{URL}Brand/Get?action=Get";
                        string jsonAll = Task.Run(async () => await _clientExtension.GetAsnyc(urlAll)).Result;
                        if (!string.IsNullOrEmpty(jsonAll))
                        {
                            var lstAll = JsonConvert.DeserializeObject<List<BrandEntity>>(jsonAll);
                            _maxBrandNum = lstAll
                                .Where(x => !string.IsNullOrEmpty(x.MaBrand))
                                .Select(x => {
                                    int n;
                                    return int.TryParse(x.MaBrand.Replace("BRAND_", ""), out n) ? n : 0;
                                })
                                .DefaultIfEmpty(0).Max();
                        }
                        else _maxBrandNum = 0;
                    }
                    catch { _maxBrandNum = 0; }
                }

                obj.MaBrand = "BRAND_" + (_maxBrandNum + _pendingAddCount + 1);
                _pendingAddCount++;

                if (!string.IsNullOrEmpty(_maKH))
                    obj.MaKH = _maKH;
                string currentUser = GlobleData.UserName;

                if (!string.IsNullOrEmpty(currentUser))
                    obj.NguoiTao = currentUser;
                lstBrandEntity.Add(obj);
                int newRowHandle = gridViewBrand.RowCount - 1;
                _lstRowAdd.Add(newRowHandle);
                if (_status != ResourceURL.EventStatus.Add)
                {
                    _status = ResourceURL.EventStatus.Add;
                    GridViewUpdateStatus(_status);
                }
                gridViewBrand.FocusedRowHandle = newRowHandle;
            }
            finally
            {
                gridViewBrand.EndUpdate();
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }

        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            UpdateFocusedColumnEdit();
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void XoaDong()
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Thông báo",MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                BrandEntity row = gridViewBrand.GetRow(gridViewBrand.FocusedRowHandle) as BrandEntity;
                if (row == null) return;
                string url = $"{URL}Brand/Delete?Parameter={row.MaBrand}";
                string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                if (result.ToLower() != "true")
                    return;
                if (!string.IsNullOrEmpty(_maKH))
                    loadCTBrand();
                else
                    LoadDSBrand(true);
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            catch
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng kiểm tra lại!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void XoaDongChon()
        {
            int[] selectedRows = gridViewBrand.GetSelectedRows();
            if (selectedRows == null || selectedRows.Length == 0) return;

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa {selectedRows.Length} dòng không?",
                "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var lstXoaDB = new List<string>();
            var lstXoaLocal = new List<BrandEntity>();

            foreach (int rowHandle in selectedRows)
            {
                BrandEntity item = gridViewBrand.GetRow(rowHandle) as BrandEntity;
                if (item == null) continue;

                if (item.ID == 0)
                    lstXoaLocal.Add(item);
                else
                    lstXoaDB.Add(item.MaBrand);
            }

            // Xóa các dòng mới local
            foreach (var item in lstXoaLocal)
            {
                lstBrandEntity.Remove(item);
                _lstRowAdd.Clear();
            }
            foreach (string maBrand in lstXoaDB)
            {
                string url = $"{URL}Brand/Delete?Parameter={maBrand}";
                string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
            }

            LoadDSBrand(false);
            clsWaitForm.ShowSuccessForm(this, 2000);
        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private string GetTenKH(string maKH)
        {
            if (string.IsNullOrEmpty(maKH)) return maKH;
            DataTable dt = repoKH.DataSource as DataTable;
            if (dt == null) return maKH;
            DataRow row = dt.AsEnumerable()
                .FirstOrDefault(r => (r["MaKH"] ?? "").ToString().Trim().ToUpper()
                                      == maKH.Trim().ToUpper());
            return row != null ? row["TenKH"].ToString() : maKH;
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                var dongRong = lstBrandEntity
                    .Where(x => string.IsNullOrWhiteSpace(x.MaKH) && string.IsNullOrWhiteSpace(x.TenBrand))
                    .ToList();
                foreach (var item in dongRong)
                    lstBrandEntity.Remove(item);

                if (_status != ResourceURL.EventStatus.View && gridViewBrand.FocusedRowHandle >= 0)
                    FocusedIndex = gridViewBrand.FocusedRowHandle;

                BrandEntity row = gridViewBrand.GetRow(gridViewBrand.FocusedRowHandle) as BrandEntity;
                if (row == null)
                {
                    if (lstBrandEntity.Count == 0)
                    {
                        XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    row = lstBrandEntity[0];
                }
                if (string.IsNullOrWhiteSpace(row.MaKH))
                {
                    MessageBox.Show("Vui lòng chọn Khách hàng.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(row.TenBrand))
                {
                    MessageBox.Show("Vui lòng nhập tên Brand.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var lstUpdate = new List<BrandEntity>();
                for (int i = 0; i < gridViewBrand.RowCount; i++)
                {
                    BrandEntity item = gridViewBrand.GetRow(i) as BrandEntity;
                    if (item == null || string.IsNullOrWhiteSpace(item.TenBrand)|| string.IsNullOrWhiteSpace(item.MaKH)) continue;

                    bool isNewRow = item.ID == 0;
                    bool isChanged = lstRowUpdate.Contains(i);
                    if (!isNewRow && !isChanged) continue;

                    string currentUser = GlobleData.UserName;
                    if (!string.IsNullOrEmpty(currentUser))
                    {
                        if (item.ID == 0) item.NguoiTao = currentUser;
                        else item.NguoiSua = currentUser;
                    }
                    lstUpdate.Add(item);
                }

                if (lstUpdate.Count == 0) return;
                for (int i = 0; i < lstUpdate.Count; i++)
                {
                    for (int j = i + 1; j < lstUpdate.Count; j++)
                    {
                        bool sameKH = (lstUpdate[i].MaKH ?? "").Trim().ToUpper() == (lstUpdate[j].MaKH ?? "").Trim().ToUpper();
                        bool sameTen = (lstUpdate[i].TenBrand ?? "").Trim().ToUpper() == (lstUpdate[j].TenBrand ?? "").Trim().ToUpper();
                        if (sameKH && sameTen)
                        {
                            string tenKH = GetTenKH(lstUpdate[i].MaKH);
                            MessageBox.Show($"Khách hàng {tenKH} đã có Brand '{lstUpdate[i].TenBrand}' bị trùng trong danh sách. Vui lòng kiểm tra lại!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                foreach (var item in lstUpdate)
                {
                    string ten = (item.TenBrand ?? "").Trim().ToUpper();
                    string maKH = (item.MaKH ?? "").Trim().ToUpper();
                    bool isDuplicate = lstBrandEntity.Any(x =>
                        (x.MaKH ?? "").Trim().ToUpper() == maKH &&
                        (x.TenBrand ?? "").Trim().ToUpper() == ten &&
                        !lstUpdate.Any(u => u.MaBrand == x.MaBrand)
                    );
                    if (isDuplicate)
                    {
                        string tenKH = GetTenKH(item.MaKH);
                        MessageBox.Show($"Khách hàng '{tenKH}' đã có Brand '{item.TenBrand}' tồn tại. Vui lòng nhập tên khác!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string url = $"{URL}Brand/Post";
                string msResult = Task.Run(async () => await _clientExtension.PostAsync(url, lstUpdate)).Result;
                if (msResult.ToLower() != "true")
                {
                    XtraMessageBox.Show(msResult);
                    return;
                }

                if (!string.IsNullOrEmpty(_maKH))
                    loadCTBrand();
                else
                    LoadDSBrand(true);

                clsWaitForm.ShowSuccessForm(this, 2000);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();
                _lstRowAdd.Clear();
                _pendingAddCount = 0;
                _maxBrandNum = -1;
            }
            catch
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi. Vui lòng kiểm tra lại!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void NapLaiDong()
        {
            if (!string.IsNullOrEmpty(_maKH))
                loadCTBrand();
            else
                LoadDSBrand(false);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            _lstRowAdd.Clear();
            lstRowUpdate.Clear();
            _pendingAddCount = 0;
            _maxBrandNum = -1;
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }       
        private void gridViewBrand_CellValueChanging(object sender,DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            BrandEntity item = view.GetRow(e.RowHandle) as BrandEntity;
            if (item == null) return;
            string currentUser = GlobleData.UserName;
            if (!string.IsNullOrEmpty(currentUser))
            {
                if (item.ID == 0)
                { if (string.IsNullOrEmpty(item.NguoiTao)) item.NguoiTao = currentUser; }
                else
                    item.NguoiSua = currentUser;
            }
            if (!lstRowUpdate.Contains(e.RowHandle))
                lstRowUpdate.Add(e.RowHandle);
            UpdateFocusedColumnEdit();
        }

        private void gridViewBrand_FocusedRowChanged(object sender,DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateFocusedColumnEdit();
        }


        private void gridViewBrand_FocusedColumnChanged(object sender,DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            UpdateFocusedColumnEdit();
        }
        //private void UpdateFocusedColumnEdit()
        //{
        //    if (gridViewBrand.FocusedColumn == null) return;
        //    if (_status == ResourceURL.EventStatus.Add)
        //    {
        //        bool isAddRow = gridViewBrand.FocusedRowHandle == _rowAdd;
        //        gridViewBrand.FocusedColumn.OptionsColumn.AllowEdit =
        //            isAddRow && gridViewBrand.FocusedColumn != colMaBrand;
        //    }
        //    else if (_status == ResourceURL.EventStatus.Edit)
        //    {
        //        gridViewBrand.FocusedColumn.OptionsColumn.AllowEdit =
        //            gridViewBrand.FocusedColumn != colMaBrand;
        //    }
        //}
        private void UpdateFocusedColumnEdit()
        {
            if (gridViewBrand.FocusedColumn == null) return;
            if (!string.IsNullOrEmpty(_maKH) && gridViewBrand.FocusedColumn == colKH)
            {
                gridViewBrand.FocusedColumn.OptionsColumn.AllowEdit = false;
                return;
            }

            if (_status == ResourceURL.EventStatus.Add)
            {
                bool isAddRow = _lstRowAdd.Contains(gridViewBrand.FocusedRowHandle);
                gridViewBrand.FocusedColumn.OptionsColumn.AllowEdit =
                    isAddRow && gridViewBrand.FocusedColumn != colMaBrand;
            }
            else if (_status == ResourceURL.EventStatus.Edit)
            {
                bool isLockedColumn = gridViewBrand.FocusedColumn == colMaBrand
                                   || gridViewBrand.FocusedColumn == colKH;
                gridViewBrand.FocusedColumn.OptionsColumn.AllowEdit = !isLockedColumn;
            }
        }
        private void gridViewBrand_ValidatingEditor(object sender,DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewBrand.FocusedRowHandle < 0) return;
            if (gridViewBrand.FocusedColumn == colMaBrand)
            {
                if (string.IsNullOrWhiteSpace(e.Value?.ToString()))
                {
                    e.Valid = false; e.ErrorText = "Mã Brand không được để trống!";
                }
                else
                {
                    bool trung = lstBrandEntity.Any(x =>
                        x.MaBrand == e.Value.ToString() &&
                        x != gridViewBrand.GetRow(gridViewBrand.FocusedRowHandle) as BrandEntity);
                    if (trung)
                    { e.Valid = false; e.ErrorText = "Mã Brand đã tồn tại!"; }
                }
            }
            if (gridViewBrand.FocusedColumn == colTenBrand)
            {
                if (string.IsNullOrWhiteSpace(e.Value?.ToString()))
                {
                    e.Valid = false; e.ErrorText = "Tên Brand không được để trống!";
                }
            }
            Luu.Enabled = e.Valid;
        }
        private void gridViewBrand_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    var size = e.Info.Graphics.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = (int)size.Width + GridPainter.Indicator.ImageSize.Width + 15;
                    if (view.IndicatorWidth < nNewSize) view.IndicatorWidth = nNewSize;

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon) e.Info.ImageIndex = -1;
            }
            catch {}
        }

        private void gridViewBrand_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            var brush = e.Cache.GetGradientBrush(rect,
                Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128),
                e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            e.Handled = true;
        }

        //private void gridViewBrand_PopupMenuShowing(object sender,DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        //{
        //    GridView view = sender as GridView;
        //    if (view == null) return;

        //    // Chỉ cần RowHandle hợp lệ, không cần kiểm tra InRow
        //    if (e.HitInfo.RowHandle < 0) return;

        //    if (view.FocusedRowHandle != e.HitInfo.RowHandle)
        //        view.FocusedRowHandle = e.HitInfo.RowHandle;

        //    bool isKHColumn = e.HitInfo.InRowCell && e.HitInfo.Column == colKH;
        //    bool isEditMode = _status == ResourceURL.EventStatus.Add
        //                   || _status == ResourceURL.EventStatus.Edit;

        //    // "Áp dụng KH" chỉ hiện khi click vào cột KH + đang edit/add
        //    ctxMenu.Items[0].Visible = isKHColumn && isEditMode;
        //    ctxMenu.Items[1].Visible = isKHColumn && isEditMode; // separator

        //    // "Xóa dòng" hiện khi có quyền xóa, bất kể đang ở mode nào
        //    ctxMenu.Items[2].Visible = _allowDelete;

        //    bool hasVisible = ctxMenu.Items.Cast<ToolStripItem>().Any(x => x.Visible);
        //    if (!hasVisible) return;

        //    e.Allow = false;
        //    ctxMenu.Show(view.GridControl,
        //                 view.GridControl.PointToClient(Cursor.Position));
        //}
        private void gridViewBrand_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTao || e.Column == colNguoiSua)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upper = username.Trim().ToUpper();
                    if (tenNVHienThi.ContainsKey(upper))
                        e.DisplayText = tenNVHienThi[upper];
                }
            }
        }
        private Dictionary<string, string> GetTenNhanVien()
        {
            try
            {
                string url = $"{URL}GetTenNV/GetTenNV";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    var dt = JsonConvert.DeserializeObject<System.Data.DataTable>(json);
                    return dt.AsEnumerable()
                        .Where(r => r["UserName"] != System.DBNull.Value &&
                                    !string.IsNullOrWhiteSpace(r.Field<string>("UserName")))
                        .GroupBy(r => r.Field<string>("UserName").Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Field<string>("TenNV") ?? g.Key);
                }
            }
            catch { clsWaitForm.ShowErrorForm(this, 2000); }
            return new Dictionary<string, string>();
        }
    }
}
