using DevExpress.DataAccess.Excel;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraRichEdit;
using DevExpress.XtraTreeList;
using Newtonsoft.Json;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.XtraGrid.Views.Base;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmBangSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, NhomSize_old = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<BangSizeEntity> lstBangSize;
        List<BangSizeEntity> lstBangSizeKT;
        List<int> lstRowUpdate = new List<int>();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        List<DataTable> lstThuVienDaDung = new List<DataTable>();
        DataRow _draggedRow;
        Bitmap dragBitmap = null;
        private int draggedRowHandle1 = GridControl.InvalidRowHandle;
        bool isDragging = false; 

        public frmBangSize()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstBangSize = new List<BangSizeEntity>();
            lstBangSizeKT = new List<BangSizeEntity>();
            lstThuVienDaDung = GET_ListTableThuVienDaDung();

            //gridBangSize.MouseDown += gridBangSize_MouseDown;
            //gridBangSize.DragEnter += gridBangSize_DragEnter;
            //gridBangSize.DragDrop += gridBangSize_DragDrop;

        }
        public List<DataTable> GET_ListTableThuVienDaDung()
        {
            string urlGetListDataTable = URL + "GetThuVien/GetThuVienDaDung";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetListDataTable); }).Result;
            List<DataTable> _ListDataTable = JsonConvert.DeserializeObject<List<DataTable>>(jsonLstDataTable);
            return _ListDataTable;
        }
        protected override void OnLoad(EventArgs e)
        {
            gridBangSize.AllowDrop = true;

            gridViewBangSize.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            gridViewBangSize.OptionsSelection.MultiSelect = false;
            gridViewBangSize.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            CreateSearchLookup();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            Init();

        }


        private void Init()
        {
            searchLookUpEditNS.Properties.ValueMember = "MaNhomSize";
            searchLookUpEditNS.Properties.DisplayMember = "NhomSize";

            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            //GetNS();
            GetKH();
        }
        private void GetNS(bool checkSave = false)
        {
            string mahang = searchLookUpEditFilter.EditValue?.ToString() ?? "All";
            string makh = searchLookUpEditKH.EditValue?.ToString() ?? "All";
            string url = $"{URL}KhaiBaoAll/Get?action=GetNSBangSize2&para1={mahang}&para2={makh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditNS.Properties.DataSource = null;
                searchLookUpEditNS.Properties.DataSource = null;
                return;
            }
            searchLookUpEditNS.Properties.DataSource = tbl;
            if (!checkSave)
                searchLookUpEditNS.EditValue = tbl.Rows[0]["MaNhomSize"];
            else searchLookUpEditNS.EditValue = NhomSize_old;
            LoadDSBangSize(false);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh};
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
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
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
            }
            if (_allowAdd || _allowEdit)
            {
                Luu.Enabled = true;
            }
            else
            {
                Luu.Enabled = false;
            }

            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridViewBangSize.OptionsBehavior.Editable = false;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        barButtonItem2.Enabled = true;
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gridViewBangSize.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        barButtonItem2.Enabled = false;
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    gridViewBangSize.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        barButtonItem2.Enabled = false;
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        private void CreateSearchLookup()
        {

            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenHang";
            rCountryEdit.ValueMember = "MaHang";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKH", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });
            }
            colMaHang.ColumnEdit = rCountryEdit;
            rCountryEdit.EditValueChanged += RCountryEdit_EditValueChanged;
            //searchLookUpEditFilter.Properties.DataSource = tbl;

            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable tblkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            RepositoryItemSearchLookUpEdit rCountryEditkh = new RepositoryItemSearchLookUpEdit();
            rCountryEditkh.DataSource = tblkh;
            rCountryEditkh.DisplayMember = "TenKH";
            rCountryEditkh.ValueMember = "MaKH";
            rCountryEditkh.ShowClearButton = false;
            rCountryEditkh.NullText = "[Chọn giá trị]";

            GridView dvViewkh = rCountryEditkh.View;
            if (dvViewkh.Columns.Count == 0)
            {
                dvViewkh.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkh.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkh.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkh.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkh.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = true });
                dvViewkh.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });

            }
            colMaKH.ColumnEdit = rCountryEditkh;
            searchLookUpEditKhachHang.Properties.DataSource = tblkh;
        }

        private void RCountryEdit_EditValueChanged(object sender, EventArgs e)
        {
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gridBangSize.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "MaKH", focusedRow["MaKH"].ToString());
                }
            }
        }

        private void LoadDSBangSize(bool isFromSave)
        {
            try
            {
                string mahang = searchLookUpEditFilter.EditValue?.ToString() ?? "All";
                string nhomsize = searchLookUpEditNS.EditValue?.ToString() ?? "All";
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "All";
                _status = ResourceURL.EventStatus.View;
                //string url = $"{URL}BangSize/GetBangSize2?para={mahang}&para2={nhomsize}" /*string.Format("{0}?", URL + "BangSize/GetBangSize")*/;

                string url = $"{URL}BangSize/GetBangSize3?para={mahang}&para2={nhomsize}&para1={makh}" /*string.Format("{0}?", URL + "BangSize/GetBangSize")*/;
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(json);
                }
                gridBangSize.DataSource = lstBangSize;
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewBangSize.FocusedRowHandle = FocusedIndex;
                    gridViewBangSize.ClearSelection();
                    gridViewBangSize.SelectRow(FocusedIndex);
                    this.ActiveControl = gridBangSize;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void ThemDong()
        {
            BangSizeEntity obj = new BangSizeEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstBangSize.Add(obj);
            _rowAdd = gridViewBangSize.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewBangSize.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;

            GridViewUpdateStatus(_status);
            focused(this.gridViewBangSize);
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private bool ValidateDataInsert(BangSizeEntity itemInsert)
        {
            if (string.IsNullOrEmpty(itemInsert.TenSize))
            {
                XtraMessageBox.Show("Size không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            else if (string.IsNullOrEmpty(itemInsert.SizeSanXuat))
            {
                XtraMessageBox.Show("Size sản xuất không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            else if (string.IsNullOrEmpty(itemInsert.MaHang))
            {
                XtraMessageBox.Show("Mã hàng không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }

            List<BangSizeEntity> lstDataSource = gridViewBangSize.DataSource as List<BangSizeEntity>;

            for (int i = 0; i < lstDataSource.Count; i++)
            {
                // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không insert
                if (lstRowUpdate[0] != i && lstRowUpdate[0] != i && (itemInsert.TenSize.Equals(lstDataSource[i].TenSize) && itemInsert.SizeSanXuat.Equals(lstDataSource[i].SizeSanXuat) && itemInsert.NhomSize.Equals(lstDataSource[i].NhomSize) && itemInsert.MaHang.Equals(lstDataSource[i].MaHang)))
                {
                    XtraMessageBox.Show("Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private bool ValidateDataAddMulti()
        {
            List<BangSizeEntity> lstDataSource = gridViewBangSize.DataSource as List<BangSizeEntity>;
            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                //BangSizeEntity itemSize = lstRowUpdate[i];
                BangSizeEntity itemSize = (gridViewBangSize.DataSource as List<BangSizeEntity>)[lstRowUpdate[i]];

                if (string.IsNullOrEmpty(itemSize.TenSize))
                {
                    XtraMessageBox.Show("Size không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                else if (string.IsNullOrEmpty(itemSize.SizeSanXuat))
                {
                    XtraMessageBox.Show("Size sản xuất không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                else if (string.IsNullOrEmpty(itemSize.MaHang))
                {
                    XtraMessageBox.Show("Mã hàng không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                //
                for (int j = 0; j < lstDataSource.Count; j++)
                {
                    // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không insert
                    if (lstRowUpdate[i] != j && (itemSize.TenSize.Equals(lstDataSource[j].TenSize) && itemSize.SizeSanXuat.Equals(lstDataSource[j].SizeSanXuat)
                        && itemSize.NhomSize.Equals(lstDataSource[j].NhomSize.ToString() == "" ? "" : lstDataSource[j].NhomSize.ToString()) && itemSize.MaHang.Equals(lstDataSource[j].MaHang)))
                    {
                        XtraMessageBox.Show("Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return false;
                    }
                }
            }




            return true;
        }

        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                string msResult = "";
                string url = string.Format("{0}", URL + "BangSize/PostBangSize");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstBangSize); }).Result;

                if (msResult.ToLower() == "true")
                {
                    //LoadDSBangSize(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    GetNS(true);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;

                GridViewUpdateStatus(_status);
                lstRowUpdate.Clear();


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    int rowHandle = gridViewBangSize.FocusedRowHandle;
                    if (gridViewBangSize.IsGroupRow(rowHandle))
                    {
                        rowHandle = gridViewBangSize.GetChildRowHandle(rowHandle, 0);
                    }
                    var lstBangSizeGet = lstBangSize[rowHandle];
                    if (lstBangSizeGet == null) return;
                    string mahang = lstBangSizeGet.MaHang.ToString();
                    string nhomsize = lstBangSizeGet.MaNhomSize.ToString();
                    string makh = lstBangSizeGet.MaKH.ToString();
                    string sizeID = lstBangSizeGet.MaSize.ToString();
                    string size = lstBangSizeGet.TenSize.ToString();
                    string url2 = $"{URL}KhaiBaoAll/Get?action=GetSizeSuDung&para1={mahang}&para2={nhomsize}&para3={makh}&para4={sizeID}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tbl != null && tbl.Rows.Count != 0)
                    {

                        MessageBox.Show($"Size {size} đã được sử dụng không được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }


                    string url = string.Format("{0}?parameter={1}", URL + "BangSize/DeleteBangSize", lstBangSizeGet.ID.ToString());
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                        gridViewBangSize.DeleteRow(rowHandle);
                    }
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void NapLaiDong()
        {
            if (searchLookUpEditKhachHang != null)
                searchLookUpEditKhachHang.EditValue = DBNull.Value; 

            if (searchLookUpEditMaHang != null)
                searchLookUpEditMaHang.EditValue = DBNull.Value;

            CreateSearchLookup();
            LoadDSBangSize(false);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status.Equals(ResourceURL.EventStatus.Add))
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && !(view.FocusedColumn == colMaSize))
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status.Equals(ResourceURL.EventStatus.Edit))
                {
                    if (view.FocusedColumn == colMaSize || view.FocusedColumn == colMaHang)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
        }

        private void gridViewBangSize_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            // Tìm index của dòng đang focus trong thứ tự của all danh sách bảng size( Không filter)
            List<BangSizeEntity> _lstBangSize = gridViewBangSize.DataSource as List<BangSizeEntity>;
            BangSizeEntity _itemBangSize = gridViewBangSize.GetFocusedRow() as BangSizeEntity;
            int index = _lstBangSize.IndexOf(_itemBangSize);

            lstRowUpdate.Add(index);
            //focused(sender);
        }

        private void gridViewBangSize_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewBangSize_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private string CheckDuplicateDataUpdate(string colName, string value, BangSizeEntity itemUpdate, int currentIndex)
        {
            List<BangSizeEntity> lstDataSource = gridViewBangSize.DataSource as List<BangSizeEntity>;

            for (int i = 0; i < lstDataSource.Count; i++)
            {
                // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không Update
                switch (colName)
                {
                    case "colTenSize":
                        if (currentIndex != i && (itemUpdate.SizeSanXuat.Equals(lstDataSource[i].SizeSanXuat) && itemUpdate.MaHang.Equals(lstDataSource[i].MaHang)))
                        {
                            if (value.Equals(lstDataSource[i].TenSize))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case "colSizeSanXuat":
                        if (currentIndex != i && (itemUpdate.TenSize.Equals(lstDataSource[i].TenSize) && itemUpdate.MaHang.Equals(lstDataSource[i].MaHang)))
                        {
                            if (value.Equals(lstDataSource[i].SizeSanXuat))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case "colMaHang":
                        if (currentIndex != i && (itemUpdate.TenSize.Equals(lstDataSource[i].TenSize) && itemUpdate.SizeSanXuat.Equals(lstDataSource[i].SizeSanXuat)))
                        {
                            if (value.Equals(lstDataSource[i].MaHang))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case null:
                        if (currentIndex != i && (itemUpdate.TenSize.Equals(lstDataSource[i].TenSize)
                            && itemUpdate.SizeSanXuat.Equals(lstDataSource[i].SizeSanXuat)
                            && itemUpdate.MaHang.Equals(lstDataSource[i].MaHang)))
                        {
                            return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                        }
                        break;
                }
            }
            return "";
        }

        private void gridViewBangSize_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {

            if (gridViewBangSize.GetFocusedDataSourceRowIndex() >= 0)
            {
                if (_status.Equals(ResourceURL.EventStatus.Edit))
                {
                    switch (gridViewBangSize.FocusedColumn.Name)
                    {
                        case "colTenSize":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Tên size không được để trống.!";
                            }
                            break;
                        case "colSizeSanXuat":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Size sản xuất không được để trống.!";
                            }
                            var focusedRowHandle = gridViewBangSize.GetFocusedDataSourceRowIndex();
                            var focusedRow = gridViewBangSize.GetDataRow(focusedRowHandle);
                            var tempLstsize = new List<BangSizeEntity>(lstBangSize);
                            tempLstsize[focusedRowHandle].SizeSanXuat = e.Value.ToString();
                            var duplicateItems = tempLstsize
                                .GroupBy(bm => new { bm.SizeSanXuat, bm.MaHang, bm.NhomSize, bm.MaKH })
                                .Where(group => group.Count() > 1)
                                .Select(group => new { group.Key.SizeSanXuat, group.Key.MaHang, group.Key.NhomSize, group.Key.MaKH })
                                .ToList();
                            if (duplicateItems.Any())
                            {
                                string duplicates = string.Join(", ", duplicateItems.Select(item => $"{item.SizeSanXuat} - {item.MaHang} - {item.NhomSize}"));
                                e.Valid = false;
                                e.ErrorText = $"Có các size và InSeam trong mã hàng bị trùng: {duplicates}";
                            }
                            break;
                        case "colMaHang":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Mã hàng không được để trống.!";
                            }
                            var focusedRowHandles = gridViewBangSize.GetFocusedDataSourceRowIndex();
                            var focusedRows = gridViewBangSize.GetDataRow(focusedRowHandles);
                            var tempLstsizes = new List<BangSizeEntity>(lstBangSize);
                            tempLstsizes[focusedRowHandles].MaHang = e.Value.ToString();
                            var duplicateItemss = tempLstsizes
                                .GroupBy(bm => new { bm.SizeSanXuat, bm.MaHang, bm.NhomSize, bm.MaKH })
                                .Where(group => group.Count() > 1)
                                .Select(group => new { group.Key.SizeSanXuat, group.Key.MaHang, group.Key.NhomSize, group.Key.MaKH })
                                .ToList();
                            if (duplicateItemss.Any())
                            {
                                string duplicates = string.Join(", ", duplicateItemss.Select(item => $"{item.SizeSanXuat} - {item.MaHang} - {item.NhomSize}"));
                                e.Valid = false;
                                e.ErrorText = $"Có các size và InSeam trong mã hàng bị trùng: {duplicates}";
                            }
                            break;
                        case "colNhomSize":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "InSeam không được để trống.!";
                            }
                            var focusedRowHandless = gridViewBangSize.GetFocusedDataSourceRowIndex();
                            var focusedRowss = gridViewBangSize.GetDataRow(focusedRowHandless);
                            var tempLstsizess = new List<BangSizeEntity>(lstBangSize);
                            tempLstsizess[focusedRowHandless].NhomSize = e.Value.ToString();
                            var duplicateItemsss = tempLstsizess
                                .GroupBy(bm => new { bm.SizeSanXuat, bm.MaHang, bm.NhomSize, bm.MaKH })
                                .Where(group => group.Count() > 1)
                                .Select(group => new { group.Key.SizeSanXuat, group.Key.MaHang, group.Key.NhomSize, group.Key.MaKH })
                                .ToList();
                            if (duplicateItemsss.Any())
                            {
                                string duplicates = string.Join(", ", duplicateItemsss.Select(item => $"{item.SizeSanXuat} - {item.MaHang} - {item.NhomSize}"));
                                e.Valid = false;
                                e.ErrorText = $"Có các size và InSeam trong mã hàng bị trùng: {duplicates}";
                            }
                            break;
                        case "colSortSize":
                            if ((e.Value?.ToString() ?? "") == "")
                            {
                                e.Valid = false;
                                e.ErrorText = "Vui lòng không được để rỗng.!";
                            }
                            if ((e.Value?.ToString() ?? "") == "0")
                            {
                                e.Valid = false;
                                e.ErrorText = "Vui lòng không nhập số 0.!";
                            }
                            int rowHandle = gridViewBangSize.FocusedRowHandle;
                            var lstBangSizeGet = lstBangSize[rowHandle];
                            if (lstBangSizeGet == null) return;

                            string mahang = lstBangSizeGet.MaHang.ToString();
                            string nhomsize = lstBangSizeGet.MaNhomSize.ToString();
                            string makh = lstBangSizeGet.MaKH.ToString();
                            string sort = e.Value.ToString() ;
                            string tenSize = lstBangSizeGet.TenSize.ToString();

                            // Kiểm tra trùng lặp với các dòng khác, không tính dòng đang focus
                            bool checkDuplicate = lstBangSize.AsEnumerable()
                                .Where(x => x != lstBangSizeGet)  // Loại bỏ dòng hiện tại ra khỏi danh sách
                                .Any(x => x.MaKH == makh
                                          && x.MaHang == mahang.Trim()
                                          && x.MaNhomSize == nhomsize
                                          && x.Sort == sort);  

                            if (checkDuplicate)
                            {
                                e.Valid = false;
                                e.ErrorText = $"Cột sắp xếp {tenSize} theo InSeam đã tồn tại";
                            }

                            break;
                    }
                }
            }
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private bool ContainsSpecialCharacters(string input)
        {
            // Kiểm tra xem input có chứa kí tự đặc biệt hay không
            // Bạn có thể tùy chỉnh biểu thức chính quy này theo nhu cầu của mình
            string pattern = "[~`!@#$%^&*()_+={}\\[\\]:;<>,.?/\"'-]";
            return Regex.IsMatch(input, pattern);
        }

        private void gridControlBangSize_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys.Equals(Keys.Control)) return;
            GridControl grid = sender as GridControl;
            GridView view = grid.FocusedView as GridView;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = fals

            if (view.FocusedColumn == colMaSize)
            {
                e.Handled = CheckCharacterID(view, e);
            }
            else if (view.FocusedColumn == colTenSize)
            {
                e.Handled = CheckCharacterName(view, e);
            }

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (view.FocusedColumn == colMaSize || view.FocusedColumn == colTenSize || view.FocusedColumn == colGhiChu || view.FocusedColumn == colCodeSize)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
            if (view.FocusedColumn == colSortSize)
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true; // Ngừng nhập nếu không phải là số hoặc Backspace
                }
            }
        }

        private bool CheckCharacterName(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                (view.FocusedColumn == colMaSize || view.FocusedColumn == colTenSize))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if (char.IsWhiteSpace(e.KeyChar) || (!char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCharacterID(GridView view, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                (view.FocusedColumn == colMaSize || view.FocusedColumn == colTenSize))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu không phải là kí tự đặc biệt -> Cho nhập
                // Nếu không phải là dấu câu -> Cho nhập
                if ((!char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private void gridViewBangSize_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private string ReplaceToVietNamese(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sử dụng regex để chuyển đổi dấu thành không dấu
            string pattern = @"\p{IsCombiningDiacriticalMarks}+";
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            string result = Regex.Replace(normalizedString, pattern, string.Empty);
            result = Regex.Replace(result, @"Đ", "D");
            result = result.Replace(" ", "_");

            return result;
        }

        private void gridViewBangSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            //if (ModifierKeys == Keys.Control)
            //{
            //    e.Handled = true;
            //}
            Console.WriteLine("key_press");
        }
        static string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        private int GetMaxSortSize(string nhomsizeA)
        {
            DataRow row = searchLookUpEdit1View.GetFocusedDataRow();
            if (row == null) return 1;
            string mahang = row["MaHang"].ToString();
            string nhomsize = nhomsizeA;
            string makh = row["MaKH"].ToString();
            string url = $"{URL}KhaiBaoAll/Get?action=GetSortSize&para1={mahang}&para2={nhomsize}&para3={makh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                return 1;
            }
            return Convert.ToInt32(tbl.Rows[0]["Sort"]) + 1;
        }
        private void btnXacNhanThem_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditKhachHang.EditValue == null || searchLookUpEditKhachHang.EditValue.ToString() == "")
            {
                XtraMessageBox.Show("Vui lòng chọn khách hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string urlKT = $"{URL}BangSize/GetBangSize";
            string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
            if (!string.IsNullOrEmpty(jsonKT))
            {
                lstBangSizeKT = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonKT);
            }
            string _txtSize = txtSize?.EditValue?.ToString() ?? "";
            List<string> _lstSize = _txtSize.Split(':').ToList();
            string _txtNhomSize = txtNhomSize?.EditValue?.ToString() ?? "0";
            List<string> _lstNhomSize = _txtNhomSize.Split(':').ToList();
            string _maHang = searchLookUpEditMaHang?.EditValue?.ToString() ?? "";
            List<string> _lstMaHang = _maHang.Split(':').ToList();
            if (string.IsNullOrEmpty(_maHang))
            {
                XtraMessageBox.Show("Mã hàng không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            //if (string.IsNullOrEmpty(_txtNhomSize))
            //{
            //    XtraMessageBox.Show("Nhóm Size không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
            //    return;
            //}
            if (string.IsNullOrEmpty(_txtSize))
            {
                XtraMessageBox.Show("Size sản xuất không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            List<BangSizeEntity> _lstAddBangSize = new List<BangSizeEntity>();
            int focusedRow = lstBangSize.Count;

            for (int j = 0; j < _lstNhomSize.Count; j++)
            {
                int indexSize = 0;

                for (int i = 0; i < _lstSize.Count; i++)
                {
                    if (!string.IsNullOrEmpty(_lstSize[i]) && !string.IsNullOrEmpty(_lstNhomSize[j]))
                    {
                        string manhomsize = ReplaceSpecialCharacters(RemoveVietnameseTone(_lstNhomSize[j].ToString().Trim())).ToString().ToUpper();
                        if (lstBangSize.Count > 0)
                            indexSize = lstBangSize.AsEnumerable()
                            .Where(x => x.MaKH == _maKH.ToString()
                                        && x.MaHang == _maHang.ToString().Trim()
                                        && x.MaNhomSize == manhomsize)
                            .Select(x => Convert.ToInt32(x.Sort))
                            .DefaultIfEmpty(0)
                            .Max();
                        indexSize++;
                        string masize = "SIZE_" + ReplaceSpecialCharactersPLUS(RemoveVietnameseTone(_lstSize[i].ToString().Trim())).ToString().ToUpper();
                        BangSizeEntity sizeEntity = new BangSizeEntity(
                            _lstSize[i].ToString().Trim(),
                            _lstSize[i].ToString().Trim(),
                            _lstNhomSize[j].ToString().Trim(),
                            _maHang.ToString().Trim(),
                            txtCodeSize.Text.ToString().Trim(),
                            txtGhiChu.Text.ToString().Trim(),
                            _maKH.ToString(),
                            masize,
                            manhomsize,
                            indexSize.ToString()
                        );
                        bool existsInList2 = lstBangSize.Any(existing =>
                            existing.SizeSanXuat == sizeEntity.SizeSanXuat &&
                            existing.NhomSize == sizeEntity.NhomSize &&
                            existing.MaHang == sizeEntity.MaHang &&
                            existing.MaKH == sizeEntity.MaKH
                        );
                        bool existsInList = lstBangSizeKT.Any(existing =>
                            existing.SizeSanXuat == sizeEntity.SizeSanXuat &&
                            existing.NhomSize == sizeEntity.NhomSize &&
                            existing.MaHang == sizeEntity.MaHang && 
                            existing.MaKH == sizeEntity.MaKH
                        );

                        if (existsInList || existsInList2)
                        {
                            XtraMessageBox.Show($"Size '{sizeEntity.SizeSanXuat}' của InSeam '{sizeEntity.NhomSize}' và mã hàng '{sizeEntity.MaHang}' đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        lstBangSize.Insert(0,sizeEntity);
                    }
                }
            }

            gridBangSize.DataSource = lstBangSize;
            //gridViewBangSize.FocusedRowHandle = lstBangSize.Count - lstRowUpdate.Count;
            gridViewBangSize.FocusedRowHandle = 0;
            gridViewBangSize.RefreshData();
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            txtSize.EditValue = null;
            txtNhomSize.EditValue = null;
            //searchLookUpEditMaHang.EditValue = "";ssss
            txtCodeSize.Text = null;
            txtGhiChu.Text = "";
        }

        private void searchLookUpEditFilter_EditValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("filter");
            //ChangingEventArgs changing = e as ChangingEventArgs;
            //if (changing == null)
            //    return;
            //if (changing.NewValue != null)
            //{
            //    gridBangSize.DataSource = lstBangSize.Where(x => x.MaHang == changing.NewValue.ToString());
            //}
            //else
            //{
            //    gridBangSize.DataSource = lstBangSize;
            //}
            GetNS();
        }

        private void txtNhomSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            string vietnameseCharacters = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđíị";

            if (vietnameseCharacters.Contains(e.KeyChar.ToString().ToLower()))
            {
                e.Handled = true;
            }
            else
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void txtSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            // e.KeyChar = Char.ToUpper(e.KeyChar);
            //int ascii = Convert.ToInt32(e.KeyChar);
            //if (!(ascii == 58))
            //{
            //    if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            //    {
            //        e.Handled = true; // Chặn ký tự nhập vào
            //    }
            //}

            e.KeyChar = Char.ToUpper(e.KeyChar);

        }

        private void txtCodeSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            // e.KeyChar = Char.ToUpper(e.KeyChar);
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
        string _maKH = "";
        private void searchLookUpEditMaHang_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)searchLookUpEditMaHang.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    _maKH = focusedRow["MaKH"].ToString();
                }
            }
        }



        private void gridViewBangSize_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                ReadExcelPL(Sfd.FileName);
            }
        }

        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                lstBangSize.Clear();
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A1:ZZ500");
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSave = new DataTable();
                dtSave = source.ToDataTable();
                AddList(dtSave);
            }

            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        private void AddList(DataTable dtSave)
        {

            string MaSize = string.Empty, MaHang = string.Empty, CodeSize = string.Empty, TenSize = string.Empty, SizeSanXuat = string.Empty, GhiChu = string.Empty, SizeXacNhan = string.Empty, MaNhomSize = string.Empty, NhomSize = string.Empty, MaKH = string.Empty;
            int XacNhan;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}?", URL + "BangSize/GetBangSize");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(json);
            }
            string urlhh = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonhh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlhh); }).Result;
            DataTable dthh = JsonConvert.DeserializeObject<DataTable>(jsonhh);

            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable dtkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            List<string> danhsachtrung = new List<string>();
            List<string> chuakhaibaohh = new List<string>();
            List<string> chuakhaibaokh = new List<string>();
            foreach (DataRow row in dtSave.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                var rowhh = dthh.AsEnumerable().FirstOrDefault(r => r["TenHang"].ToString().Trim() == row[0].ToString().Trim());
                var rowlkh = dtkh.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == row[1].ToString().Trim());

                if (rowhh == null)
                {
                    chuakhaibaohh.Add(row[0].ToString());
                }
                if (rowlkh == null)
                {
                    chuakhaibaokh.Add(row[1].ToString());
                }

                if (rowhh == null || rowlkh == null)
                {
                    continue;
                }

                MaSize = "SIZE_" + ReplaceSpecialCharacters(RemoveVietnameseTone(row[3].ToString().Trim().ToUpper()));
                MaHang = rowhh["MaHang"].ToString();
                MaKH = rowlkh["MaKH"].ToString();
                NhomSize = row[2].ToString();
                TenSize = row[3].ToString();
                SizeSanXuat = row[4].ToString();
                CodeSize = row[5].ToString();
                GhiChu = row[6].ToString();
                MaNhomSize = ReplaceSpecialCharacters(RemoveVietnameseTone(row[2].ToString().Trim().ToUpper()));
                bool Istrung = lstBangSize.Any(x => x.MaSize == MaSize && MaNhomSize == MaNhomSize && MaHang == MaHang);
                if (Istrung)
                {
                    //XtraMessageBox.Show($"Mã Khách Hàng bị trùng: {MaDT}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhsachtrung.Add(SizeSanXuat + "\n");
                }
                else
                {
                    lstBangSize.Add(new BangSizeEntity
                    {
                        MaSize = MaSize,
                        MaHang = MaHang,
                        MaKH = MaKH,
                        NhomSize = NhomSize,
                        TenSize = TenSize,
                        SizeSanXuat = SizeSanXuat,
                        CodeSize = CodeSize,
                        GhiChu = GhiChu,
                        MaNhomSize = MaNhomSize
                    });
                    string msResult = "";
                    string urlpost = string.Format("{0}", URL + "BangSize/PostBangSize");
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlpost, lstBangSize); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        LoadDSBangSize(false);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(msResult);

                }
            }
            if (chuakhaibaohh.Any())
            {
                XtraMessageBox.Show($"Các Hàng Hóa chưa khai báo:\n{string.Join("\n", chuakhaibaohh.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chuakhaibaokh.Any())
            {
                XtraMessageBox.Show($"Các Khách Hàng chưa khai báo:\n{string.Join("\n", chuakhaibaokh.Distinct())}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (danhsachtrung.Any())
            {
                string dstrungma = string.Join(", ", danhsachtrung.Distinct());
                XtraMessageBox.Show($"Các mã Màu bị trùng:{dstrungma} của mã hàng :{MaHang}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void gridBangSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (gridViewBangSize.FocusedColumn.FieldName == "Sort")
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true; // Ngừng nhập nếu không phải là số hoặc Backspace
                }
            }
            
        }


        private void gridBangSize_DragDrop_1(object sender, DragEventArgs e)
        {
            isDragging = false;
            // Lấy GridControl từ sender
            GridControl grid = sender as GridControl;
            if (grid == null) return;

            // Lấy View (GridView hoặc CardView, v.v.)
            GridView view = grid.MainView as GridView;
            if (view == null) return;

            // Lấy vị trí chuột khi thả
            Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));

            // Lấy thông tin hàng tại vị trí thả
            GridHitInfo hitInfo = view.CalcHitInfo(clientPoint);

            // Kiểm tra xem vị trí thả có hợp lệ không
            if (!hitInfo.InRow || hitInfo.RowHandle < 0)
            {
                // Xóa hiệu ứng khoảng trống
                //isDragging = false;
                draggedRowHandle1 = GridControl.InvalidRowHandle;
                dragBitmap?.Dispose(); // Giải phóng bitmap
                dragBitmap = null;
                return;
            }

            // Lấy row handle của hàng được kéo
            int draggedRowHandle = (int)e.Data.GetData(typeof(int));

            // Lấy row handle của hàng đích (nơi thả)
            int targetRowHandle = hitInfo.RowHandle;

            // Nếu hàng được kéo và hàng đích giống nhau, không làm gì
            if (draggedRowHandle == targetRowHandle)
            {
                // Xóa hiệu ứng khoảng trống
                isDragging = false;
                draggedRowHandle1 = GridControl.InvalidRowHandle;
                view.LayoutChanged();
                dragBitmap?.Dispose(); // Giải phóng bitmap
                dragBitmap = null;
                return;
            }

            object draggedGroup = view.GetRowCellValue(draggedRowHandle, "NhomSize");
            object targetGroup = view.GetRowCellValue(targetRowHandle, "NhomSize");
            object draggedGroupMH = view.GetRowCellValue(draggedRowHandle, "MaHang");
            object targetGroupMH = view.GetRowCellValue(targetRowHandle, "MaHang");

            if (!object.Equals(draggedGroup, targetGroup))
            {
                isDragging = false;
                draggedRowHandle1 = GridControl.InvalidRowHandle;
                MessageBox.Show("Chỉ có thể kéo trong cùng một InSeam!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                dragBitmap?.Dispose(); // Giải phóng bitmap
                dragBitmap = null;
                return;
            }
            if (!object.Equals(draggedGroupMH, targetGroupMH))
            {
                isDragging = false;
                draggedRowHandle1 = GridControl.InvalidRowHandle;
                MessageBox.Show("Chỉ có thể kéo trong cùng một Mã Hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                dragBitmap?.Dispose(); // Giải phóng bitmap
                dragBitmap = null;
                return;
            }
            // Hoán đổi vị trí giữa hai hàng
            SwapRows(view, draggedRowHandle, targetRowHandle);

            // Xóa hiệu ứng khoảng trống
            draggedRowHandle1 = GridControl.InvalidRowHandle;
            dragBitmap?.Dispose(); // Giải phóng bitmap
            dragBitmap = null;
        }

        private void SwapRows(GridView view, int rowHandle1, int rowHandle2)
        {
            // Tạo một từ điển tạm để lưu trữ dữ liệu của hàng 1
            Dictionary<GridColumn, object> tempRowData = new Dictionary<GridColumn, object>();

            // Lưu dữ liệu của hàng 1 vào biến tạm
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName == "Sort") continue;
                tempRowData[column] = view.GetRowCellValue(rowHandle1, column);
            }

            // Sao chép dữ liệu từ hàng 2 sang hàng 1
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName == "Sort") continue;
                object value2 = view.GetRowCellValue(rowHandle2, column);
                view.SetRowCellValue(rowHandle1, column, value2);
            }

            // Sao chép dữ liệu từ biến tạm (hàng 1) sang hàng 2
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName == "Sort") continue;
                view.SetRowCellValue(rowHandle2, column, tempRowData[column]);
            }
        }

        private Rectangle GetRowBounds(GridView view, int rowHandle)
        {
            // Lấy GridControl từ GridView
            GridControl grid = view.GridControl;
            if (grid == null) return Rectangle.Empty;

            // Lấy thông tin về dòng từ GridViewInfo
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            if (viewInfo == null) return Rectangle.Empty;

            // Sử dụng phương thức GetGridRowInfo để lấy thông tin dòng chính xác
            GridDataRowInfo rowInfo = viewInfo.GetGridRowInfo(rowHandle) as GridDataRowInfo;
            if (rowInfo == null) return Rectangle.Empty;

            // Trả về kích thước và vị trí của dòng
            return rowInfo.Bounds;
        }

        private Cursor CreateCursor(Bitmap bmp, Point hotSpot)
        {
            // Tạo con trỏ chuột từ Bitmap và HotSpot
            IntPtr hIcon = bmp.GetHicon();
            IconInfo iconInfo = new IconInfo();
            GetIconInfo(hIcon, ref iconInfo);

            // Thiết lập HotSpot
            iconInfo.xHotspot = hotSpot.X;
            iconInfo.yHotspot = hotSpot.Y;
            iconInfo.fIcon = false; // Đặt là false để tạo con trỏ chuột (cursor), không phải icon

            // Tạo con trỏ chuột mới
            IntPtr hCursor = CreateIconIndirect(ref iconInfo);

            // Giải phóng tài nguyên
            DestroyIcon(hIcon);
            if (iconInfo.hbmMask != IntPtr.Zero) DeleteObject(iconInfo.hbmMask);
            if (iconInfo.hbmColor != IntPtr.Zero) DeleteObject(iconInfo.hbmColor);

            return new Cursor(hCursor);
        }

        // Cấu trúc IconInfo để lưu thông tin về icon
        private struct IconInfo
        {
            public bool fIcon;      // true nếu là icon, false nếu là cursor
            public int xHotspot;    // Tọa độ X của HotSpot
            public int yHotspot;    // Tọa độ Y của HotSpot
            public IntPtr hbmMask;  // Handle của bitmap mask
            public IntPtr hbmColor; // Handle của bitmap màu
        }

        // Các hàm API cần thiết
        [DllImport("user32.dll")]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        private void gridBangSize_DragEnter(object sender, DragEventArgs e)
        {
            // Kiểm tra xem dữ liệu được kéo có phải là một số nguyên (row handle) không
            if (e.Data.GetDataPresent(typeof(int)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void gridBangSize_MouseDown(object sender, MouseEventArgs e)
        {

            GridControl grid = sender as GridControl;
            if (grid == null) return;

            GridView view = grid.MainView as GridView;
            if (view == null) return;

            // Lấy thông tin về vị trí chuột
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            if (hitInfo.InRow || hitInfo.InRowCell)
            {
                int rowHandle = hitInfo.RowHandle;
                isDragging = true;
                draggedRowHandle1 = rowHandle;
                view.LayoutChanged();
                // Lấy kích thước của dòng
                Rectangle rowBounds = GetRowBounds(view, rowHandle);

                // Kiểm tra tính hợp lệ của rowBounds
                if (rowBounds.IsEmpty || rowBounds.Width <= 0 || rowBounds.Height <= 0)
                {
                    isDragging = false;
                    return;
                }

                // Chuyển đổi tọa độ từ GridControl sang screen
                Point screenLocation = grid.PointToScreen(new Point(rowBounds.Left, rowBounds.Top));

                // Chụp hình ảnh của dòng
                try
                {
                    using (Bitmap tempBitmap = new Bitmap(rowBounds.Width, rowBounds.Height))
                    {
                        using (Graphics g = Graphics.FromImage(tempBitmap))
                        {
                            g.CopyFromScreen(screenLocation, Point.Empty, rowBounds.Size);
                        }
                        dragBitmap = new Bitmap(tempBitmap);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi chụp hình ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dragBitmap?.Dispose();
                    dragBitmap = null;
                    return;
                }

                // Bắt đầu kéo
                grid.DoDragDrop(rowHandle, DragDropEffects.Move);
            }
        }

        private void gridBangSize_MouseMove(object sender, MouseEventArgs e)
        {
            GridControl grid = sender as GridControl;
            if (grid == null) return;

            GridView view = grid.MainView as GridView;
            if (view == null) return;

            // Kiểm tra nếu người dùng đang nhấn chuột trái và di chuyển
            if (e.Button == MouseButtons.Left)
            {
                // Lấy thông tin về vị trí chuột
                GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
                if (hitInfo.InRow || hitInfo.InRowCell)
                {
                    // Đánh dấu là đang kéo
                   // isDragging = true;
                    draggedRowHandle1 = hitInfo.RowHandle;

                    // Yêu cầu GridView vẽ lại để hiển thị khoảng trống
                    view.LayoutChanged();

                    // Bắt đầu kéo
                    grid.DoDragDrop(hitInfo.RowHandle, DragDropEffects.Move);
                }
            }
        }

        private void gridBangSize_DragOver(object sender, DragEventArgs e)
        {


        }

        private void gridBangSize_DragLeave(object sender, EventArgs e)
        {
            dragBitmap?.Dispose(); // Giải phóng bitmap
            dragBitmap = null;
        }

        private void gridBangSize_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (dragBitmap != null)
            {
                // Hiển thị hình ảnh của dòng ngay trên con trỏ chuột
                e.UseDefaultCursors = false;
                Cursor.Current = CreateCursor(dragBitmap, new Point(10, 10)); // Điều chỉnh vị trí của hình ảnh
            }
            else
            {
                e.UseDefaultCursors = true;
            }
        }

        private void gridViewBangSize_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            // Kiểm tra xem có đang kéo và có phải dòng được kéo không
            if (isDragging && e.RowHandle == draggedRowHandle1)
            {
                // Vẽ hình chữ nhật trong suốt hoặc có màu nền nhạt
                using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.LightGray))) // Màu nền nhạt với độ trong suốt
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                // Ngăn không cho GridView vẽ lại ô này
                e.Handled = true;
            }
        }

        private void gridBangSize_MouseUp(object sender, MouseEventArgs e)
        {
            // Đặt lại trạng thái khi người dùng nhấp chuột mà không kéo
            isDragging = false;
            draggedRowHandle1 = GridControl.InvalidRowHandle;

            // Yêu cầu GridView vẽ lại
            GridView view = gridBangSize.MainView as GridView;
            if (view != null)
            {
                view.LayoutChanged();
            }
        }

        private void gridBangSize_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void gridViewBangSize_MouseUp(object sender, MouseEventArgs e)
        {
            // Đặt lại trạng thái khi người dùng nhấp chuột mà không kéo
            isDragging = false;
            draggedRowHandle1 = GridControl.InvalidRowHandle;

            // Yêu cầu GridView vẽ lại
            GridView view = gridBangSize.MainView as GridView;
            if (view != null)
            {
                view.LayoutChanged();
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaNhom();
        }

        private void searchLookUpEditNS_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSBangSize(false);
            NhomSize_old = searchLookUpEditNS.EditValue.ToString();
        }

        private void gridViewBangSize_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }



        private void gridViewBangSize_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info.Column == gridColNhomSize)
            {
                info.GroupText = "InSeam: " + info.GroupValueText;
                //string mahang = searchLookUpEditFilter.EditValue?.ToString() ?? "All";
                //string nhomsize = searchLookUpEditNS.EditValue?.ToString() ?? "All";
                //if(mahang.ToString() != "All" || nhomsize.ToString() != "All")
                //{
                gridViewBangSize.ExpandAllGroups();
                //}
            }
        }

        #region quan
        private async void XoaNhom()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa InSeam không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    int rowHandle = gridViewBangSize.FocusedRowHandle;
                    //if (rowHandle < 0) return;
                    if (gridViewBangSize.IsGroupRow(rowHandle))
                    {
                        rowHandle = gridViewBangSize.GetChildRowHandle(rowHandle, 0);
                    }
                    if (rowHandle < 0 || rowHandle >= lstBangSize.Count) return;
                    var lstBangSizeGet = lstBangSize[rowHandle];
                    if (lstBangSizeGet == null) return;
                    string manhomsize = lstBangSizeGet.MaNhomSize.ToString();
                    string mahang = lstBangSizeGet.MaHang.ToString();
                    string nhomsize = lstBangSizeGet.MaNhomSize.ToString();
                    string makh = lstBangSizeGet.MaKH.ToString();
                    string sizeID = lstBangSizeGet.MaSize.ToString();
                    string size = lstBangSizeGet.TenSize.ToString();
                    string url2 = $"{URL}KhaiBaoAll/Get?action=GetSizeSuDung&para1={mahang}&para2={nhomsize}&para3={makh}&para4={sizeID}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tbl != null && tbl.Rows.Count != 0)
                    {

                        MessageBox.Show($"Size {size} trong InSeam size {nhomsize} đã được sử dụng không được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    

                    string url = string.Format("{0}?manhomsize={1}&&mahang={2}&&makh={3}", URL + "BangSize/DeleteNhomBangSize", manhomsize, mahang, makh);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                        LoadDSBangSize(false);
                    }
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa InSeam đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void searchLookUpEditKhachHang_EditValueChanged(object sender, EventArgs e)
        {
            CreateSearchLookupMH();
        }

        private void CreateSearchLookupMH()
        {
            string url = string.Format("{0}?makh={1}", URL + "BangSize/GetBangSizeMH", searchLookUpEditKhachHang.EditValue == null ? "" : searchLookUpEditKhachHang.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblmh = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tblmh;
            rCountryEdit.DisplayMember = "TenHang";
            rCountryEdit.ValueMember = "MaHang";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKH", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });
            }

            searchLookUpEditMaHang.Properties.DataSource = tblmh;
        }

        #endregion

        private void GetKH()
        {

            string url = $"{URL}KhaiBaoAll/Get?action=GetKHBangSize";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditKH.Properties.DataSource = null;
                return;
            }
            searchLookUpEditKH.Properties.DataSource = tbl;
            searchLookUpEditKH.EditValue = tbl.Rows[0]["MaKH"];
        }
        private void GetMH()
        {
            string khachhang = searchLookUpEditKH.EditValue?.ToString() ?? "All";

            string url = $"{URL}KhaiBaoAll/Get?action=GetHangHoaBangMau&para1={khachhang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEditFilter.Properties.DataSource = null;
                return;
            }
            searchLookUpEditFilter.Properties.DataSource = tbl;
            searchLookUpEditFilter.EditValue = tbl.Rows[0]["MaHang"];
            LoadDSBangSize(false);
        }
        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            GetMH();
            GetNS();
        }
        static string ReplaceSpecialCharactersPLUS(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(input, replacement);
            int plusCount = input.Count(c => c == '+');
            result += new string('_', plusCount);

            return result;
        }
    }
}
