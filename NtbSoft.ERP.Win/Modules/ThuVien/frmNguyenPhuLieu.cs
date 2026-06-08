using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{

    public partial class frmNguyenPhuLieu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        List<NguyenPhuLieuEntity> lstNPL;
        int _rowAdd = -1;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        int option = -1;
        List<int> lstRowUpdate;
        bool validData = false;
        int FocusedIndex = 0;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmNguyenPhuLieu()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstNPL = new List<NguyenPhuLieuEntity>();
            lstRowUpdate = new List<int>();
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(Save, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            
            CheckPermission();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            loadNPL(false);
        }
        private void CheckPermission()
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
        private void loadNPL(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}", URL + "NguyenPL/GetNPL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstNPL = JsonConvert.DeserializeObject<List<NguyenPhuLieuEntity>>(json);
                }

                bgrNguyenPL.DataSource = lstNPL;
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridView1.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = bgrNguyenPL;
                }
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private string CheckDuplicateDataUpdate(string colName, string value, NguyenPhuLieuEntity itemUpdate, int currentIndex)
        {
            List<NguyenPhuLieuEntity> lstDataSource = gridView1.DataSource as List<NguyenPhuLieuEntity>;
            for (int i = 0; i < lstDataSource.Count; i++)
            {
                // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không Update
                switch (colName)
                {
                    case "colmaVT":
                        if (currentIndex != i && (itemUpdate.TenNPL.Equals(lstDataSource[i].TenNPL) && itemUpdate.TenMauNPL.Equals(lstDataSource[i].TenMauNPL)))
                        {
                            if (value.Equals(lstDataSource[i].MaVatTu))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case "colTenNPL":
                        if (currentIndex != i && (itemUpdate.MaVatTu.Equals(lstDataSource[i].MaVatTu) && itemUpdate.TenMauNPL.Equals(lstDataSource[i].TenMauNPL)))
                        {
                            if (value.Equals(lstDataSource[i].TenNPL))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case "colTenMau":
                        if (currentIndex != i && (itemUpdate.MaVatTu.Equals(lstDataSource[i].MaVatTu) && itemUpdate.TenNPL.Equals(lstDataSource[i].TenNPL)))
                        {
                            if (value.Equals(lstDataSource[i].TenMauNPL))
                            {
                                return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                            }
                        }
                        break;
                    case null:
                        if (currentIndex != i && (itemUpdate.MaVatTu.Equals(lstDataSource[i].MaVatTu) && itemUpdate.TenNPL.Equals(lstDataSource[i].TenNPL) && itemUpdate.TenMauNPL.Equals(lstDataSource[i].TenMauNPL)))
                        {
                            return "Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!";
                        }
                        break;
                }
            }
            return "";
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            // Khi update check có đủ 3 field ở hàm validate
            if (gridView1.GetFocusedDataSourceRowIndex() >= 0)
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    switch (gridView1.FocusedColumn.Name)
                    {
                        case "colmaVT":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Mã vật tư không được để trống.!";
                            }
                            else if (ContainsVietnamese(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Mã vật tư không được chứa dấu tiếng Việt.!";
                            }

                            break;
                        case "colTenNPL":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Tên nguyên phụ liệu không được để trống.!";
                            }
                            break;
                        case "colTenMau":
                            if (string.IsNullOrEmpty(e.Value.ToString()))
                            {
                                e.Valid = false;
                                e.ErrorText = "Màu nguyên phụ liệu không được để trống.!";
                            }
                            break;
                    }
                    string result = CheckDuplicateDataUpdate(gridView1.FocusedColumn.Name, e.Value.ToString(), gridView1.GetFocusedRow() as NguyenPhuLieuEntity, gridView1.GetFocusedDataSourceRowIndex());

                    if (!string.IsNullOrEmpty(result))
                    {
                        e.Valid = false;
                        e.ErrorText = result;
                    }

                    validData = e.Valid;


                    // code cũ
                    //if (gridView1.FocusedColumn == this.colmaVT)
                    //{
                    //    if (e.Value.ToString() == "")
                    //    {
                    //        e.Valid = false;
                    //        e.ErrorText = "Thông tin không được để trống.!";
                    //    }
                    //    else if (ContainsVietnamese(e.Value.ToString()))
                    //    {
                    //        e.Valid = false;
                    //        e.ErrorText = "Mã vật tư không được chứa dấu tiếng việt.!";
                    //    }
                    //    else
                    //    {
                    //        //DataRow dr = ((DataTable)gridViewHangHoa.DataSource).AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();
                    //        //DataRow dr = gridViewHangHoa.DataSource as BindingList<HangHoaEntity>();
                    //        //    .AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();

                    //        List<HangHoaEntity> LstHangHoa = gridView1.DataSource as List<HangHoaEntity>;
                    //        HangHoaEntity itemHangHoa = gridView1.GetFocusedRow() as HangHoaEntity;
                    //        List<HangHoaEntity> lstHangHoaExceptFocused = LstHangHoa.Where(item => item.MaHang != itemHangHoa.MaHang).ToList();
                    //        HangHoaEntity hangHoa = lstHangHoaExceptFocused.Where(item => item.MaHang == e.Value.ToString()).FirstOrDefault();

                    //        if (hangHoa != null)
                    //        {
                    //            e.Valid = false;
                    //            e.ErrorText = "Mã hàng hóa đã bị trùng. Vui lòng nhập mã hàng hóa mới.!";
                    //        }
                    //    }
                    //}
                }
            }
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private void ThemDong()
        {
            NguyenPhuLieuEntity obj = new NguyenPhuLieuEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstNPL.Add(obj);

            _rowAdd = gridView1.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridView1.FocusedRowHandle = _rowAdd;
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }
        private bool ValidateDataInsert(NguyenPhuLieuEntity itemInsert)
        {
            if (string.IsNullOrEmpty(itemInsert.MaVatTu))
            {
                XtraMessageBox.Show("Mã vật tư không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            else if (ContainsVietnamese(itemInsert.MaVatTu))
            {
                XtraMessageBox.Show("Mã vật tư không được chứa dấu tiếng Việt.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            else if (string.IsNullOrEmpty(itemInsert.TenNPL))
            {
                XtraMessageBox.Show("Tên nguyên phụ liệu không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            else if (string.IsNullOrEmpty(itemInsert.TenMauNPL))
            {
                XtraMessageBox.Show("Màu không được để trống", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }

            List<NguyenPhuLieuEntity> lstDataSource = gridView1.DataSource as List<NguyenPhuLieuEntity>;

            for (int i = 0; i < lstDataSource.Count; i++)
            {
                // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không insert
                if (lstRowUpdate[0] != i && (itemInsert.MaVatTu.Equals(lstDataSource[i].MaVatTu) && itemInsert.TenNPL.Equals(lstDataSource[i].TenNPL) && itemInsert.TenMauNPL.Equals(lstDataSource[i].TenMauNPL)))
                {
                    XtraMessageBox.Show("Dữ liệu đã bị trùng. Vui lòng nhập lại thông tin!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }
        private void Save()
        {
            try
            {
                this.ActiveControl = this.button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridView1.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridView1.FocusedRowHandle;
                }
                List<NguyenPhuLieuEntity> _lstUpdate = new List<NguyenPhuLieuEntity>();
                NguyenPhuLieuEntity focusedRow = gridView1.GetFocusedRow() as NguyenPhuLieuEntity;

                if (focusedRow == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (lstRowUpdate.Count > 0)
                {
                    // Khi insert check phải nhập đủ 3 field ở hàm lưu
                    // Khi update check có đủ 3 field ở hàm validate
                    lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();

                    if (_status == ResourceURL.EventStatus.Add)
                    {
                        // Khi insert => lstRowUpdate chỉ có 1 phần tử
                        NguyenPhuLieuEntity itemInsert = gridView1.GetRow(lstRowUpdate[0]) as NguyenPhuLieuEntity;
                        bool resultValidateInsert = ValidateDataInsert(itemInsert);
                        if (!resultValidateInsert)
                        {
                            return;
                        }
                        _lstUpdate.Add(itemInsert);
                    }
                    else if (_status == ResourceURL.EventStatus.Edit)
                    {

                        for (int i = 0; i < lstRowUpdate.Count; i++)
                        {
                            NguyenPhuLieuEntity item = gridView1.GetRow(lstRowUpdate[i]) as NguyenPhuLieuEntity;
                            string resultCheckDuplicate = CheckDuplicateDataUpdate(null, null, item, lstRowUpdate[i]);
                            if (item != null && string.IsNullOrEmpty(resultCheckDuplicate))
                            {
                                _lstUpdate.Add(item);
                            }
                        }
                    }

                    if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                    {
                        return;
                    }
                    string urlNPL = string.Format("{0}", URL + "NguyenPL/PostNPL");
                    string json = JsonConvert.SerializeObject(_lstUpdate);
                    DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlNPL, tblSave); }).Result;
                    if (result.ToLower() == "true")
                    {
                        loadNPL(true);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(result);
                    _status = ResourceURL.EventStatus.View;
                    GridViewUpdateStatus(_status);
                    lstRowUpdate.Clear();
                }
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private async void XoaDong()
        {
            try
            {
                NguyenPhuLieuEntity row = gridView1.GetFocusedRow() as NguyenPhuLieuEntity;

                if (row == null) return;
                if (!string.IsNullOrEmpty(row.MaNPL))
                {
                    DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.Yes)
                    {
                        string urlDelete = string.Format("{0}/?manpl={1}", URL + "NguyenPL/DeleteNPL", row.MaNPL);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(urlDelete); }).Result;
                        loadNPL(false);
                    }
                }
                else
                {
                    DialogResult messResult = MessageBox.Show("Bạn cần chọn dòng để xóa? ", "Thông báo");
                }
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void NapLaiDong()
        {
            loadNPL(false);
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }
        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridview1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void bgrNguyenPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.FocusedColumn == colmaVT || view.FocusedColumn == colTenNPL || view.FocusedColumn == colTenMau || view.FocusedColumn == colGhiChu)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

       
        private void bgrNguyenPL_EditorKeyPress(object sender, KeyPressEventArgs e)
        {

            //GridControl grid = sender as GridControl;
            //gridView1_KeyPress(grid.FocusedView, e);
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridView1.OptionsBehavior.Editable = false;

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
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gridView1.OptionsBehavior.Editable = true;
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
                    gridView1.OptionsBehavior.Editable = true;
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
        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd && !(view.FocusedColumn == colMaNPL || view.FocusedColumn == colmamau))
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }

                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colMaNPL || view.FocusedColumn == colmamau)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                    {
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;

                    }
                }
            }
        }
    }
}
