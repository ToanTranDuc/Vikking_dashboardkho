using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.Serialization;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
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
using DevExpress.XtraEditors.Repository;
using System.Globalization;

namespace NtbSoft.ERP.Win.Modules.DicForm
{
    public partial class frmDicQCDongThung : DevExpress.XtraEditors.XtraForm
    {
        public bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        ResourceURL.EventStatus _status;
        string URL = string.Empty;
        int _rowAdd = -1;
        ArrayList _listRow;
        List<DicQCDongThungEntity> lstUpdate = new List<DicQCDongThungEntity>();
        List<int> lstRowUpdate = new List<int>();
        private HttpClientExtension _clientExtension;
        List<DicQCDongThungEntity> _listData;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        KeyDownControlHandler keyDownControlHandler;
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        private List<int> _pastedRows = new List<int>();
        public frmDicQCDongThung()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _listRow = new ArrayList();
            _clientExtension = new HttpClientExtension();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            _listData = new List<DicQCDongThungEntity>();
            Init();
            LoadDonVi();
        }
        private void Init()
        {
            repoDonVi.ValueMember = "MaDV";
            repoDonVi.DisplayMember = "TenDV";
            repoDonVi.NullText = "[Chọn đvị]";

            repoDonVi.PopupView.Columns.Clear();
            repoDonVi.PopupView.Columns.AddVisible("TenDV", "Đơn vị");
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }


        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(AddRow, true, ActionType.Add, this.btAdd.Enabled);
            actionControlEdit = new ActionControl(EditRow, true, ActionType.Edit, this.btEdit.Enabled);
            actionControlDelete = new ActionControl(DeletedRow, true, ActionType.Delete, this.btDelete.Enabled);
            actionControlSave = new ActionControl(SaveRows, (true || _allowEdit), ActionType.Save, this.btSave.Enabled);
            actionControlRefresh = new ActionControl(NapLai, true, ActionType.Refresh, this.btRefresh.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            tenNVHienThi = GetTenNhanVien();
            LoadData();
        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridView1.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        btAdd.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        btEdit.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        btDelete.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        btSave.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gridView1.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        btAdd.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        btEdit.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        btDelete.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        btSave.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    gridView1.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        btAdd.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        btEdit.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        btDelete.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    btEdit.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        btSave.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit; ;
            _allowDelete = obj.AllowDelete;

            //if (!_allowAdd)
            //    btAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //if (!_allowEdit)
            //    btEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //if (GlobleData.UserName == "admin")
            //    btDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //if (!_allowAdd && !_allowEdit)
            //    btSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void LoadDonVi()
        {
            string url = string.Format("{0}", URL + $"DicQCDongThung/Get?action=GetDV");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            repoDonVi.DataSource = dt;
        }
        private async void LoadData()
        {
            try
            {
                string url = string.Format("{0}", URL + $"DicQCDongThung/Get?action=Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    _listData = JsonConvert.DeserializeObject<List<DicQCDongThungEntity>>(json);
                    _listData = _listData
                    .OrderBy(x => x.ChieuDai)
                    .ThenBy(x => x.ChieuRong)
                    .ThenBy(x => x.ChieuCao)
                    .ThenBy(x => x.SoLop ?? 0)
                    .ThenBy(x => x.LoaiThung ?? "")
                    .ToList();
                }

                gridControl1.DataSource = _listData;

                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Kiểm tra mã công đoạn có đang trong bảng công đoạn mã hàng hay không-> để hiển thị thông báo khi xóa
        private async System.Threading.Tasks.Task<int> GetCountQuiCach(string maCongDoan)
        {
            //string url = URL + ResourceURL.UrlDicCongDoanMaHang + "/" + "GetAllCongDoan?maCongDoan=" + maCongDoan;
            //List<DicCongDoanMaHangEntity> lstCongDoanMH = await _serviceCongDoanMaHang.GetAllCongDoan(url);
            //return lstCongDoanMH.Count;
            return 0;
        }

        private void btAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddRow();
        }

        private void AddRow()
        {
            DicQCDongThungEntity obj = new DicQCDongThungEntity();
            obj.MaDV = "1";
            //obj.TenQuiCach = "";
            _listData.Add(obj);
            gridControl1.RefreshDataSource();
            gridView1.MoveLast();
            _rowAdd = gridView1.FocusedRowHandle;
            //_rowAdd = gridView1.RowCount - 1;        
            //gridView1.FocusedRowHandle = _rowAdd;
            gridView1.FocusedColumn = gridView1.Columns["ChieuDai"];
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
        }

        private void btEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Console.WriteLine("btn_edit");
            EditRow();
        }

        private void EditRow()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            int focusedRow = gridView1.FocusedRowHandle;
            if (focusedRow >= 0 && !lstRowUpdate.Contains(focusedRow))
                lstRowUpdate.Add(focusedRow);
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveRows();
            btEdit.Enabled = true;

        }
        private string NormalizeTenQuiCach(string tenQC)
        {
            if (string.IsNullOrWhiteSpace(tenQC)) return tenQC;
            return tenQC.Replace("*", "x").Replace("X", "x");
        }
        private async void SaveRows()
        {
            try
            {
                this.ActiveControl = button1;
                int focusedRowHandle = gridView1.FocusedRowHandle;
                DicQCDongThungEntity row = gridView1.GetRow(gridView1.FocusedRowHandle) as DicQCDongThungEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                if (lstRowUpdate.Count == 0)
                {
                    return;
                }
                string tenQCFocused = row.TenQuiCach;
                string maQCFocused = row.MaQuiCach;
                bool isAddMode = (_status == ResourceURL.EventStatus.Add);

                var groupedItems = new Dictionary<string, DicQCDongThungEntity>();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    DicQCDongThungEntity item = gridView1.GetRow(lstRowUpdate[i]) as DicQCDongThungEntity;
                    if (item == null) return;
                    if (!string.IsNullOrEmpty(item.TenQuiCach))
                        item.TenQuiCach = NormalizeTenQuiCach(item.TenQuiCach);
                    if (item.SoLop == null || item.SoLop == 0)
                    {
                        XtraMessageBox.Show($"Dữ liệu số lớp không được để trống!",
                            "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (item.LoaiThung == null)
                    {
                        XtraMessageBox.Show($"Dữ liệu loại thùng không được để trống!",
                            "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (item.ChieuCao == null || item.ChieuCao == 0 || item.ChieuDai == null || item.ChieuDai == 0 || item.ChieuRong == null || item.ChieuRong == 0)
                    {
                        XtraMessageBox.Show($"Dữ liệu Chiều dài, Chiều rộng, Chiều cao phải hợp lệ !!!",
                            "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (CheckDuplicated(gridView1, item, lstRowUpdate[i]))
                    {
                        XtraMessageBox.Show($"Quy cách dòng {lstRowUpdate[i] + 1} đã tồn tại (cùng kích thước, số lớp, loại thùng)!",
                            "Dữ liệu trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string key = $"{item.TenQuiCach?.Trim().ToLower()}|{item.SoLop}|{item.LoaiThung?.Trim().ToLower()}";
                    if (groupedItems.ContainsKey(key))
                    {
                        var existing = groupedItems[key];
                        if (!string.IsNullOrWhiteSpace(item.GhiChu))
                        {
                            if (!string.IsNullOrWhiteSpace(existing.GhiChu))
                                existing.GhiChu += "," + item.GhiChu.Trim();
                            else
                                existing.GhiChu = item.GhiChu.Trim();
                        }
                        existing.NguoiSua = GlobleData.UserName;
                    }
                    else
                    {
                        if (_status == ResourceURL.EventStatus.Add)
                        {
                            item.MaQuiCach = "";
                            if (string.IsNullOrEmpty(item.NguoiTao))
                            {
                                item.NguoiTao = GlobleData.UserName;
                            }
                        }
                        else if (_status == ResourceURL.EventStatus.Edit)
                        {
                            item.NguoiSua = GlobleData.UserName;
                        }
                        groupedItems.Add(key, item);
                    }
                }

                lstUpdate.AddRange(groupedItems.Values);
                string mg = string.Empty;
                if (lstUpdate != null && lstUpdate.Count > 0)
                {
                    if (_status == ResourceURL.EventStatus.Add)
                    {
                        mg = await _clientExtension.PostAsync(URL + $"DicQCDongThung/Insert", lstUpdate);

                        _rowAdd = -1;
                    }
                    else
                    {
                        mg = await _clientExtension.PostAsync(URL + $"DicQCDongThung/Update", lstUpdate);
                    }
                }
                _listRow.Clear();
                lstRowUpdate.Clear();
                lstUpdate.Clear();
                _pastedRows.Clear();
                //if (string.Compare("True", mg) != 0)
                if (mg == string.Empty)
                {
                    clsWaitForm.ShowErrorForm(this, 3000);
                }
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
                LoadData();
                await Task.Delay(100);
                if (isAddMode && !string.IsNullOrEmpty(tenQCFocused))
                {
                    FocusToRow(tenQCFocused, isAddMode);
                }
                else if (!isAddMode && !string.IsNullOrEmpty(maQCFocused))
                {
                    FocusToRow(maQCFocused, isAddMode);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DeletedRow();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa dữ liệu lỗi!Vui lòng thử lại", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }

        }
        private async void DeletedRow()
        {
            try
            {
                var view = gridView1;
                if (view == null) return;

                // Lấy tất cả các dòng được chọn (selected rows)
                int[] selectedRows = view.GetSelectedRows();
                if (selectedRows == null || selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng để xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var newAddedRows = selectedRows.Where(r =>
                    string.IsNullOrEmpty(view.GetRowCellValue(r, colMaQC_DongThung)?.ToString())).ToList();

                var savedRows = selectedRows.Except(newAddedRows).ToList();

                string message = $"Bạn có chắc chắn muốn xóa {selectedRows.Length} dòng đã chọn?\n";

                if (XtraMessageBox.Show(message, "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));

                bool hasError = false;
                foreach (int rowHandle in savedRows)
                {
                    string maQuiCach = view.GetRowCellValue(rowHandle, colMaQC_DongThung)?.ToString();
                    if (string.IsNullOrEmpty(maQuiCach)) continue;

                    string url = $"{URL}DicQCDongThung/Delete?maQuiCach={maQuiCach}";
                    string result = await _clientExtension.DeletedAsync(url);

                    if (result != "True")
                    {
                        hasError = true;
                        break;
                    }
                }

                var rowsToRemove = newAddedRows.OrderByDescending(r => r).ToList();
                foreach (int rowHandle in rowsToRemove)
                {
                    if (view.IsValidRowHandle(rowHandle))
                        _listData.RemoveAt(view.GetDataSourceRowIndex(rowHandle));
                }

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                if (hasError)
                    return;
                else
                    clsWaitForm.ShowSuccessForm(this, 2000);

                LoadData();
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                return;
            }
        }
        //private async void DeletedRow()
        //{
        //    int rowHandle = gridView1.FocusedRowHandle;
        //    if (rowHandle < 0) return;

        //    object Id = gridView1.GetFocusedRowCellValue(colMaQC_DongThung);

        //    if (Id == null || string.IsNullOrEmpty(Id.ToString()))
        //    {
        //        if (XtraMessageBox.Show("Xóa dòng?", Properties.Resources.Warning,
        //            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        //        {
        //            _listData.RemoveAt(rowHandle);
        //            gridControl1.DataSource = _listData;
        //            gridControl1.RefreshDataSource();
        //        }
        //        return;
        //    }
        //    if (XtraMessageBox.Show(Properties.Resources.CheckDelete, Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
        //    {
        //        //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));
        //        string url = string.Format("{0}?maQuiCach={1}", URL + "DicQCDongThung/Delete", Id);
        //        if (Id == null) return;
        //        int count = await GetCountQuiCach(Id.ToString());
        //        if (count > 0)
        //        {
        //            if (XtraMessageBox.Show("Dữ liệu đang được sử dụng ở một bảng khác, bấm yes để xóa dữ liệu", Properties.Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
        //            {
        //                //string ss = await _serviceQCDongThung.Delete(url);
        //                string urlDeleteCongDoanMaHang = string.Format("{0}?maQuiCach={1}", URL + "DicQCDongThung/Update", Id);
        //                string ss = await _clientExtension.DeletedAsync(urlDeleteCongDoanMaHang);
        //                if (string.Compare(ss, "True") != 0)
        //                    XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                string ss = await _clientExtension.DeletedAsync(url);
        //                if (string.Compare(ss, "True") != 0)
        //                    XtraMessageBox.Show(Properties.Resources.SaveError, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //            catch (Exception e) 
        //            {
        //                return;
        //            }

        //        }
        //        //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        //        LoadData();
        //    }
        //}

        private void NapLai()
        {
            _pastedRows.Clear();
            LoadData();
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
            btEdit.Enabled = true;
        }

        private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridView1_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }
        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            Console.WriteLine("e.value: " + e.Value);
            Console.WriteLine("gridView1_CellValueChanging");
            GridView gridView = sender as GridView;
            if (gridView == null) return;
            int focusedRowHandle = gridView1.FocusedRowHandle;
            DicQCDongThungEntity item = gridView.GetRow(e.RowHandle) as DicQCDongThungEntity;
            if (item != null)
            {
                var currentUser = GlobleData.UserName;
                if (!string.IsNullOrEmpty(currentUser))
                {
                    if (string.IsNullOrEmpty(item.MaQuiCach))
                    {
                        if (string.IsNullOrEmpty(item.NguoiTao))
                        {
                            item.NguoiTao = currentUser;
                        }
                    }
                    else
                    {
                        item.NguoiSua = currentUser;
                    }
                }
            }
            if (e.Column.FieldName == "ChieuDai" || e.Column.FieldName == "ChieuRong" || e.Column.FieldName == "ChieuCao")
            {
                if (focusedRowHandle >= 0)
                {
                    object chieuDai = gridView.GetRowCellValue(focusedRowHandle, "ChieuDai");
                    object chieuRong = gridView.GetRowCellValue(focusedRowHandle, "ChieuRong");
                    object chieuCao = gridView.GetRowCellValue(focusedRowHandle, "ChieuCao");
                    if (e.Column.FieldName == "ChieuDai") chieuDai = e.Value;
                    else if (e.Column.FieldName == "ChieuRong") chieuRong = e.Value;
                    else if (e.Column.FieldName == "ChieuCao") chieuCao = e.Value;
                    string dai = chieuDai?.ToString() ?? "";
                    string rong = chieuRong?.ToString() ?? "";
                    string cao = chieuCao?.ToString() ?? "";
                    string tenQuiCach = $"{dai}x{rong}x{cao}";
                    gridView.SetRowCellValue(focusedRowHandle, "TenQuiCach", tenQuiCach);
                }
            }

            focused(sender);
        }
        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if ((_rowAdd != -1 && view.FocusedRowHandle == _rowAdd) || _pastedRows.Contains(view.FocusedRowHandle))
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colMaQC_DongThung)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (keyData == (Keys.Control | Keys.Delete))
            //{
            //    btnClear.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.S))
            //{
            //    btnLuu.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.A))
            //{
            //    simpleButton1.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.T))
            //{
            //    simpleButton2.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.Shift | Keys.E))
            //{
            //    btnNhapExcel.PerformClick();
            //    return true;
            //}
            if (keyData == (Keys.Control | Keys.V))
            {
                PasteCtrlV();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (gridView1.SelectedRowsCount > 1)
                {
                    CopySelectedRowsToClipboard(includeHeader: true);
                }
                else
                {
                    CopySelectedRowsToClipboard(includeHeader: false);
                }
                e.Handled = true;
            }
            //else if (e.Control && e.KeyCode == Keys.V)
            //{
            //    PasteCell(sender, e);
            //    e.Handled = true;
            //}
            switch (e.KeyCode)
            {
                case Keys.F1:
                    if (_allowAdd)
                    {
                        AddRow();
                    }
                    break;
                case Keys.F2:
                    if (_allowEdit)
                    {
                        EditRow();
                    }
                    break;
                //case Keys.F3:
                //    if (_allowDelete)
                //    {
                //        DeletedRow();
                //        e.Handled = true;
                //    }
                //    break;
                case Keys.F4:
                    SaveRows();
                    break;
                case Keys.F5:
                    LoadData();
                    break;
                //case Keys.Delete:
                //    if (_allowDelete)
                //    {
                //        DeletedRow();
                //    }
                //    break;
                default:
                    break;
            }
        }

        private void gridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView view = sender as GridView;

            DicQCDongThungEntity row = gridView1.GetRow(gridView1.FocusedRowHandle) as DicQCDongThungEntity;

            // chặn không cho nhập kí tự "_" ở cột MaCongDoan
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false
            if (view.FocusedColumn == colChieuDai || view.FocusedColumn == colChieuCao ||
                view.FocusedColumn == colChieuRong || view.FocusedColumn == colCanNang || view.FocusedColumn == colSoLop)
            {
                e.Handled = true;
            }

            // Chỉ cho nhập số và nút delete trong cột SoLuong
            string includeCharSoLuong = "0123456789.,";

            if ((view.FocusedColumn == colChieuDai || view.FocusedColumn == colChieuCao ||
                view.FocusedColumn == colChieuRong || view.FocusedColumn == colCanNang || view.FocusedColumn == colSoLop) && includeCharSoLuong.IndexOf(e.KeyChar) >= 0)
            {
                e.Handled = false;
            }

            // 8 là mã ascii của nút delete. Nút delete luôn luôn được bấm
            if ((int)e.KeyChar == 8)
            {
                e.Handled = false;
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = e.ListSourceRowIndex + 1;
            }
        }

        //private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        //{
        //    if (e.Column.FieldName == "STT")
        //    {
        //        e.DisplayText = (e.ListSourceRowIndex + 1).ToString();
        //    }
        //}

        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportExcelQCDongThung();
        }

        private void ImportExcelQCDongThung()
        {
            using (var frm = new frmDicQCDongThung_Excel())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                    if (frm.ErrorLines != null && frm.ErrorLines.Count > 0)
                    {
                        string errorMsg = $"Import thành công với {frm.ErrorLines.Count} dòng dữ liệu bị bỏ qua:\n";
                        errorMsg += string.Join("\n", frm.ErrorLines.Take(50));

                        if (frm.ErrorLines.Count > 50)
                            errorMsg += $"\n và {frm.ErrorLines.Count - 50} dòng lỗi khác";

                        XtraMessageBox.Show(errorMsg, "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        XtraMessageBox.Show("Import từ Excel thành công!\nDữ liệu đã được lưu.",
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void gridControl1_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gridView1_KeyPress(grid.FocusedView, e);
        }

        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTao || e.Column == colNguoiSua)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upperUser = username.ToUpper();
                    if (tenNVHienThi.ContainsKey(upperUser))
                    {
                        e.DisplayText = tenNVHienThi[upperUser];
                    }
                }
            }
            if (e.Column.FieldName == "TenQuiCach")
            {
                if (e.Value != null && e.Value is string str && !string.IsNullOrWhiteSpace(str))
                {
                    e.DisplayText = str.Replace("x", "*");
                }
            }
        }

        private bool CheckDuplicated(GridView gridView, DicQCDongThungEntity curEntity, int currentRowHandle)
        {

            //for (int i = 0; i < gridView.DataRowCount; i++)
            //{

            //    if (i == currentRowHandle) continue;

            //    object existingTenQuiCach = gridView.GetRowCellValue(i, "TenQuiCach");
            //    if (existingTenQuiCach != null && existingTenQuiCach.ToString() == tenQuiCach)
            //    {
            //        return true; 
            //    }
            //}
            //string tenQC = (curEntity.TenQuiCach ?? "").Trim().ToLower();
            string tenQC = NormalizeTenQuiCach((curEntity.TenQuiCach ?? "").Trim().ToLower());
            int soLop = curEntity.SoLop ?? 0;
            string loaiThung = (curEntity.LoaiThung ?? "").Trim().ToLower();
            //string ghiChu = (curEntity.GhiChu ?? "").Trim().ToLower();

            for (int i = 0; i < gridView.DataRowCount; i++)
            {
                if (i == currentRowHandle) continue;

                var rowEntity = gridView.GetRow(i) as DicQCDongThungEntity;
                if (rowEntity == null) continue;

                //string exsistingTenQC = (rowEntity.TenQuiCach ?? "").Trim().ToLower();
                string exsistingTenQC = NormalizeTenQuiCach((rowEntity.TenQuiCach ?? "").Trim().ToLower());
                int exsistingSoLop = (rowEntity.SoLop ?? 0);
                string exsistingLoaiThung = (rowEntity.LoaiThung ?? "").Trim().ToLower();
                //string exsistingGhiChu = (rowEntity.GhiChu ?? "").Trim().ToLower();

                if (exsistingTenQC == tenQC && exsistingSoLop == soLop && exsistingLoaiThung == loaiThung)
                {
                    return true;
                }
            }

            return false;
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridView1.RowCount > 0)
            {
                //if (e.Menu == null)
                //    return;
                //e.Menu.Items.Clear();

                //if (e.HitInfo.InRow)
                //{
                //    if (e.HitInfo.Column != null)
                //    {

                //        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", CopyCell);
                //        e.Menu.Items.Add(menuCopyItem);
                //        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", PasteCell);
                //        e.Menu.Items.Add(menuPasteItem);
                //        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", DeleteRow);
                //        e.Menu.Items.Add(menuDeleteItem);
                //    }
                //}
                if (e.Menu == null) return;
                e.Menu.Items.Clear();

                // Luôn thêm các mục cơ bản
                var copyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", OnCopyMenuItemClicked);
                var pasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", PasteCell);
                var deleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", DeleteRow);

                if (e.HitInfo.InRow && e.HitInfo.HitTest == DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.RowIndicator)
                {
                    copyItem.Caption = "Copy dòng đã chọn";
                    copyItem.Tag = "copy_rows";
                }
                else
                {
                    copyItem.Caption = "Copy";
                    copyItem.Tag = "copy_cells";
                }

                e.Menu.Items.Add(copyItem);
                e.Menu.Items.Add(pasteItem);
                e.Menu.Items.Add(deleteItem);
            }
        }
        private void OnCopyMenuItemClicked(object sender, EventArgs e)
        {
            var menuItem = sender as DevExpress.Utils.Menu.DXMenuItem;
            if (menuItem == null) return;

            string mode = menuItem.Tag?.ToString() ?? "copy_cells";

            if (mode == "copy_rows")
            {
                CopySelectedRowsToClipboard(includeHeader: true);
            }
            else
            {
                CopySelectedRowsToClipboard(includeHeader: false);
                //CopyCell(sender, e);
            }
        }

        private void CopySelectedRowsToClipboard(bool includeHeader = false)
        {
            var view = gridView1;
            if (view == null || view.SelectedRowsCount == 0) return;

            var sb = new StringBuilder();

            var visibleColumns = view.VisibleColumns.Cast<GridColumn>()
                .Where(c => c.Visible && c.FieldName != "STT")
                .OrderBy(c => c.VisibleIndex)
                .ToList();
            if (includeHeader)
            {
                var headers = visibleColumns.Select(c => c.Caption ?? c.FieldName);
                sb.AppendLine(string.Join("\t", headers));
            }
            var selectedHandles = view.GetSelectedRows()
                .Where(h => h >= 0 && view.IsDataRow(h))
                .OrderBy(h => h)
                .ToArray();

            foreach (int rowHandle in selectedHandles)
            {
                var rowValues = new List<string>();
                foreach (var col in visibleColumns)
                {
                    object val = view.GetRowCellValue(rowHandle, col);
                    string text = Convert.ToString(val ?? "");
                    if (text.Contains("\t") || text.Contains("\r") || text.Contains("\n") || text.Contains("\""))
                        text = "\"" + text.Replace("\"", "\"\"") + "\"";

                    rowValues.Add(text);
                }
                sb.AppendLine(string.Join("\t", rowValues));
            }

            if (sb.Length > 0)
                Clipboard.SetText(sb.ToString().TrimEnd('\r', '\n'));
        }
        private void DeleteRow(object sender, EventArgs e)
        {
            DeletedRow();
        }
        private void PasteCell(object sender, EventArgs e)
        {
            try
            {
                GridView view = gridControl1.FocusedView as GridView;
                if (view == null) return;

                string clipboardData = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardData)) return;

                string[] lines = clipboardData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0) return;

                bool skipFirstLine = false;
                string firstLine = lines[0].ToLowerInvariant();
                if (firstLine.Contains("stt") || firstLine.Contains("qui cách thùng") || firstLine.Contains("số lớp") ||
                    firstLine.Contains("loại thùng") || firstLine.Contains("chiều dài") || firstLine.Contains("chiều rộng") ||
                    firstLine.Contains("chiều cao") || firstLine.Contains("đơn vị") || firstLine.Contains("ghi chú") ||
                    firstLine.Contains("người tạo") || firstLine.Contains("người sửa"))
                {
                    skipFirstLine = true;
                }

                if (skipFirstLine)
                {
                    lines = lines.Skip(1).ToArray();
                    if (lines.Length == 0) return;
                }

                int startRow = view.FocusedRowHandle;
                if (startRow < 0) startRow = view.RowCount;

                if (_status == ResourceURL.EventStatus.View)
                {
                    _status = ResourceURL.EventStatus.Add;
                    GridViewUpdateStatus(_status);
                }

                _pastedRows.Clear();
                int rowsNeeded = lines.Length;
                int currentRowCount = view.RowCount;

                for (int i = 0; i < rowsNeeded; i++)
                {
                    if (startRow + i >= currentRowCount)
                    {
                        DicQCDongThungEntity newObj = new DicQCDongThungEntity();
                        newObj.MaDV = "1";
                        newObj.NguoiTao = GlobleData.UserName;
                        _listData.Add(newObj);
                    }
                }
                gridControl1.RefreshDataSource();
                int currentRow = startRow;
                foreach (string row in lines)
                {
                    if (string.IsNullOrWhiteSpace(row)) continue;

                    DicQCDongThungEntity entity = view.GetRow(currentRow) as DicQCDongThungEntity;
                    if (entity != null && string.IsNullOrEmpty(entity.NguoiTao))
                    {
                        entity.NguoiTao = GlobleData.UserName;
                    }

                    AddRowClipBoard(row, currentRow, view);

                    if (!lstRowUpdate.Contains(currentRow))
                        lstRowUpdate.Add(currentRow);

                    _pastedRows.Add(currentRow);
                    currentRow++;
                }

                if (_pastedRows.Count > 0)
                {
                    int lastPastedRow = _pastedRows.Last();
                    view.FocusedRowHandle = lastPastedRow;
                    view.MakeRowVisible(lastPastedRow);
                    view.FocusedColumn = view.Columns["ChieuDai"];
                }

                view.CloseEditor();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void PasteCtrlV()
        {
            try
            {
                GridView view = gridControl1.FocusedView as GridView;
                if (view == null) return;

                string clipboardData = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardData)) return;

                string[] lines = clipboardData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0) return;

                bool skipFirstLine = false;
                string firstLine = lines[0].ToLowerInvariant();
                if (firstLine.Contains("stt") || firstLine.Contains("qui cách thùng") || firstLine.Contains("số lớp") ||
                    firstLine.Contains("loại thùng") || firstLine.Contains("chiều dài") || firstLine.Contains("chiều rộng") ||
                    firstLine.Contains("chiều cao") || firstLine.Contains("đơn vị") || firstLine.Contains("ghi chú") ||
                    firstLine.Contains("người tạo") || firstLine.Contains("người sửa"))
                {
                    skipFirstLine = true;
                }

                if (skipFirstLine)
                {
                    lines = lines.Skip(1).ToArray();
                    if (lines.Length == 0) return;
                }

                int startRow = view.FocusedRowHandle;
                if (startRow < 0) startRow = view.RowCount;

                if (_status == ResourceURL.EventStatus.View)
                {
                    _status = ResourceURL.EventStatus.Add;
                    GridViewUpdateStatus(_status);
                }

                _pastedRows.Clear();
                int rowsNeeded = lines.Length;
                int currentRowCount = view.RowCount;

                for (int i = 0; i < rowsNeeded; i++)
                {
                    if (startRow + i >= currentRowCount)
                    {
                        DicQCDongThungEntity newObj = new DicQCDongThungEntity();
                        newObj.MaDV = "1";
                        newObj.NguoiTao = GlobleData.UserName;
                        _listData.Add(newObj);
                    }
                }

                gridControl1.RefreshDataSource();
                int currentRow = startRow;
                foreach (string row in lines)
                {
                    if (string.IsNullOrWhiteSpace(row)) continue;

                    DicQCDongThungEntity entity = view.GetRow(currentRow) as DicQCDongThungEntity;
                    if (entity != null && string.IsNullOrEmpty(entity.NguoiTao))
                    {
                        entity.NguoiTao = GlobleData.UserName;
                    }

                    AddRowClipBoard(row, currentRow, view);

                    if (!lstRowUpdate.Contains(currentRow))
                        lstRowUpdate.Add(currentRow);

                    _pastedRows.Add(currentRow);
                    currentRow++;
                }

                if (_pastedRows.Count > 0)
                {
                    int lastPastedRow = _pastedRows.Last();
                    view.FocusedRowHandle = lastPastedRow;
                    view.MakeRowVisible(lastPastedRow);
                    view.FocusedColumn = view.Columns["ChieuDai"];
                }

                view.CloseEditor();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void AddRowClipBoard(string data, int rowHandle, GridView view)
        {
            if (string.IsNullOrEmpty(data)) return;

            string[] rowData = data.Split('\t');
            var columnMapping = new Dictionary<int, string>
            {
                { 0, "GhiChu" },
                { 1, "TenQuiCach" },
                { 2, "SoLop" },
                { 3, "LoaiThung" },
                { 4, "ChieuDai" },
                { 5, "ChieuRong" },
                { 6, "ChieuCao" },
                { 7, "MaDV" },
                { 8, "GhiChu2" }
            };
            int maxColumns = Math.Min(rowData.Length, columnMapping.Count);

            for (int i = 0; i < maxColumns; i++)
            {
                if (!columnMapping.ContainsKey(i))
                    continue;

                string fieldName = columnMapping[i];
                string cellValue = rowData[i].Trim();
                if (string.IsNullOrEmpty(cellValue))
                {
                    if (fieldName == "SoLop" || fieldName == "ChieuDai" ||
                        fieldName == "ChieuRong" || fieldName == "ChieuCao")
                    {
                        continue;
                    }
                    else if (fieldName == "MaDV")
                    {
                        try
                        {
                            GridColumn targetColumn = view.Columns[fieldName];
                            if (targetColumn != null)
                                view.SetRowCellValue(rowHandle, targetColumn, "1");
                        }
                        catch { }
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                try
                {
                    GridColumn targetColumn = view.Columns[fieldName];
                    if (targetColumn == null) continue;

                    Type fieldType = targetColumn.ColumnType;

                    if (fieldName == "MaDV" && targetColumn.ColumnEdit is RepositoryItemSearchLookUpEdit lookupEdit)
                    {
                        string cellValueTrimmed = cellValue.Trim();
                        if (string.IsNullOrEmpty(cellValueTrimmed))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, "1");
                            continue;
                        }

                        var dataSource = lookupEdit.DataSource as DataTable;
                        if (dataSource == null)
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, "1");
                            continue;
                        }

                        object matchedValue = null;
                        foreach (DataRow dr in dataSource.Rows)
                        {
                            string tenDV = dr[lookupEdit.DisplayMember]?.ToString()?.Trim();
                            if (string.IsNullOrEmpty(tenDV)) continue;
                            if (string.Equals(tenDV, cellValueTrimmed, StringComparison.OrdinalIgnoreCase) ||
                                tenDV.IndexOf(cellValueTrimmed, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                cellValueTrimmed.IndexOf(tenDV, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                matchedValue = dr[lookupEdit.ValueMember];
                                break;
                            }
                        }
                        if (matchedValue != null)
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, matchedValue);
                        }
                        else
                        {
                            if (int.TryParse(cellValueTrimmed, out int maDV))
                            {
                                view.SetRowCellValue(rowHandle, targetColumn, maDV.ToString());
                            }
                            else
                            {
                                view.SetRowCellValue(rowHandle, targetColumn, "1");
                            }
                        }
                    }
                    else if (fieldName == "GhiChu" || fieldName == "TenQuiCach" || fieldName == "LoaiThung")
                    {
                        if (fieldName == "TenQuiCach")
                            cellValue = NormalizeTenQuiCach(cellValue);
                        view.SetRowCellValue(rowHandle, targetColumn, cellValue);
                    }
                    else if (fieldName == "SoLop")
                    {
                        if (int.TryParse(cellValue.Replace(",", "").Replace(".", ""), out int soLop))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, soLop);
                        }
                    }
                    else if (fieldType == typeof(int) || fieldType == typeof(int?))
                    {
                        if (int.TryParse(cellValue.Replace(",", "").Replace(".", ""), out int intValue))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, intValue);
                        }
                    }
                    else if (fieldType == typeof(decimal) || fieldType == typeof(decimal?) ||
                             fieldType == typeof(float) || fieldType == typeof(float?) ||
                             fieldType == typeof(double) || fieldType == typeof(double?))
                    {
                        string cleaned = cellValue.Replace(",", "").Replace(".", "");
                        if (decimal.TryParse(cleaned, NumberStyles.Any,
                            CultureInfo.InvariantCulture, out decimal decValue))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, decValue);
                        }
                    }
                    else
                    {
                        view.SetRowCellValue(rowHandle, targetColumn, cellValue);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            AutoGenerateTenQuiCach(view, rowHandle);
        }
        private void AutoGenerateTenQuiCach(GridView view, int rowHandle)
        {
            object tenQC = view.GetRowCellValue(rowHandle, "TenQuiCach");
            if (!string.IsNullOrEmpty(tenQC?.ToString())) return;

            object daiObj = view.GetRowCellValue(rowHandle, "ChieuDai");
            object rongObj = view.GetRowCellValue(rowHandle, "ChieuRong");
            object caoObj = view.GetRowCellValue(rowHandle, "ChieuCao");

            if (daiObj == null || rongObj == null || caoObj == null) return;

            if (decimal.TryParse(daiObj.ToString(), out decimal dai) &&
                decimal.TryParse(rongObj.ToString(), out decimal rong) &&
                decimal.TryParse(caoObj.ToString(), out decimal cao))
            {
                if (dai > 0 && rong > 0 && cao > 0)
                {
                    string autoName = $"{dai}x{rong}x{cao}";
                    view.SetRowCellValue(rowHandle, "TenQuiCach", autoName);
                }
            }
        }

        private void CopyCell(object sender, EventArgs e)
        {
            GridView view = (gridControl1.MainView) as GridView;
            if (view == null) return;
            view.CopyToClipboard();
        }

        private void FocusToRow(string identifier, bool isAddMode)
        {
            string normalizedIdentifier = NormalizeTenQuiCach(identifier);
            gridView1.BeginUpdate();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                DicQCDongThungEntity item = gridView1.GetRow(i) as DicQCDongThungEntity;
                if (item == null) continue;
                bool found = false;
                if (isAddMode)
                {
                    if (NormalizeTenQuiCach(item.TenQuiCach) == normalizedIdentifier)
                        found = true;
                }
                else
                {
                    if (item.MaQuiCach == identifier)
                        found = true;
                }
                if (found)
                {
                    gridView1.FocusedRowHandle = i;
                    gridView1.MakeRowVisible(i);
                    break;
                }
            }
            gridView1.EndUpdate();
        }
        private Dictionary<string, string> GetTenNhanVien()
        {
            try
            {
                string url = string.Format("{0}?", URL + "GetTenNV/GetTenNV");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    DataTable dtNhanVien = JsonConvert.DeserializeObject<DataTable>(json);
                    string userNameCol = "UserName";
                    string tenNVCol = "TenNV";
                    return dtNhanVien.AsEnumerable()
                        .Where(row => row[userNameCol] != DBNull.Value && row[userNameCol] != null && !string.IsNullOrWhiteSpace(row.Field<string>(userNameCol))) // Lọc row null/rỗng
                        .GroupBy(row => row.Field<string>(userNameCol).Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Field<string>(tenNVCol) ?? g.Key
                        );
                }
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
            return new Dictionary<string, string>();
        }
    }
}
