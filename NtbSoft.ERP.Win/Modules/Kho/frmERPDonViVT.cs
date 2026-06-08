using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity;
using NtbSoft.ERP.Entity.Kho;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPDonViVT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        List<int> lstRowUpdate = new List<int>();
        List<ERPDonViVTEntity> lstDonViVTEntity;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        bool _IsDialog = false;
        //Thoai thêm biến lấy tên nv
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        public frmERPDonViVT()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonViVTEntity = new List<ERPDonViVTEntity>();
            txtTiLe.Properties.NullValuePrompt = "0.0000";
            txtTiLe.Properties.NullValuePromptShowForEmptyValue = true;
            
          

        }
        public frmERPDonViVT(bool IsDialog = false)
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonViVTEntity = new List<ERPDonViVTEntity>();
            txtTiLe.Properties.NullValuePrompt = "0.0000";
            txtTiLe.Properties.NullValuePromptShowForEmptyValue = true;
            _IsDialog = IsDialog;
           

        }

        //private List<ActionControl> InitActionKeyDown()
        //{
        //    actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
        //    //actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
        //    //actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
        //    actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
        //    actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

        //    lstActionControls = new List<ActionControl> {
        //        actionControlAdd, actionControlEdit,
        //        actionControlDelete, actionControlSave,actionControlRefresh };
        //    return lstActionControls;
        //}
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            //if (_IsDialog)
            //{
            //    Luu.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //    btnSubmit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //}
            //else
            //{
            //    Luu.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //    btnSubmit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;


            //}
            
            CheckPerminsion();
            tenNVHienThi = GetTenNhanVien(); //Khởi tạo biến lấy ds tên nv
            //keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSDonViVT(false);
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);//SystemUser/GetPer/userID=...
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
                layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
             
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            float TiLe = 0.0f;
            string strTiLe = txtTiLe.EditValue?.ToString() ?? "";
            bool isFloat = Regex.IsMatch(strTiLe, @"^[+-]?(\d+(\.\d*)?|\.\d+)$");
            if (!isFloat && !string.IsNullOrEmpty(strTiLe))
            {
                XtraMessageBox.Show("Vui lòng nhập tỉ lệ đúng định dạng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            float.TryParse(strTiLe, out  TiLe);
            if(TiLe < 0)
            {
                XtraMessageBox.Show("Vui lòng nhập tỉ lệ lớn hơn số 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string _txtTendvvt = txtTendvvt?.EditValue?.ToString() ?? "";
            List<string> _lstdvvt = _txtTendvvt.Split(':').ToList();

            if (string.IsNullOrEmpty(_txtTendvvt))
            {
                XtraMessageBox.Show("Tên đơn vị vật tư không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<ERPDonViVTEntity> _lstAdddvvt = new List<ERPDonViVTEntity>();
            int focusedRow = lstDonViVTEntity.Count;
            int dvvtIndex = -1;
            foreach (var dvvt in _lstdvvt)
            {
                dvvtIndex++;

                if (!string.IsNullOrEmpty(dvvt))
                {
                    ERPDonViVTEntity dvvtEntity = new ERPDonViVTEntity(dvvt.ToString().Trim(), TiLe);
                    dvvtEntity.NguoiTao = GlobleData.UserName;
                    dvvtEntity.NgayTao = DateTime.Now;
                    bool existsInList = lstDonViVTEntity.Any(existing =>
                        existing.TenDVVT.Trim().ToUpper() == dvvtEntity.TenDVVT.Trim().ToUpper()
                    );
                    if (existsInList)
                    {
                        XtraMessageBox.Show($"Đơn vị vật tư '{dvvtEntity.TenDVVT}' đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    else
                    {
                        bool existsInAddList = _lstAdddvvt.Any(existing =>
                        existing.TenDVVT == dvvtEntity.TenDVVT
                        );
                        if (!existsInAddList)
                        {
                            _lstAdddvvt.Add(dvvtEntity);
                            lstRowUpdate.Add(focusedRow);
                            focusedRow += 1;
                        }
                    }
                }
            }

            lstDonViVTEntity.AddRange(_lstAdddvvt);
            gridDonViVT.DataSource = lstDonViVTEntity;
            gridViewDonViVT.FocusedRowHandle = lstDonViVTEntity.Count - lstRowUpdate.Count;
            gridViewDonViVT.RefreshData();
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            txtTendvvt.EditValue = null;
            txtTiLe.EditValue = null;
          
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }
        private void NapLaiDong()
        {
            LoadDSDonViVT(false);
          
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewDonViVT.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewDonViVT.FocusedRowHandle;
                }
                List<ERPDonViVTEntity> _lstUpdate = new List<ERPDonViVTEntity>();
                List<ERPDonViVTEntity> _lstAll = new List<ERPDonViVTEntity>();

                _lstAll = gridDonViVT.DataSource as List<ERPDonViVTEntity>;
                if(_lstAll==null||_lstAll.Count==0)
                {
                    return;
                }    
                if (KiemTraTrung(_lstAll))
                {
                    XtraMessageBox.Show("Có dữ liệu trùng.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                List<ERPDonViVTEntity> _lstDVVT = new List<ERPDonViVTEntity>();
                foreach(ERPDonViVTEntity item in _lstAll)
                {
                    if (item.ID == 0 || item.NgaySua.HasValue) _lstDVVT.Add(item);
                }
                if(_lstDVVT.Count == 0)
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    return;
                }
                
                //lstRowUpdate = lstRowUpdate.Distinct().OrderBy(item => item).ToList();
                int RowCount = this.gridViewDonViVT.RowCount;

                var tblrow1 = gridViewDonViVT.DataSource as List<ERPDonViVTEntity>;
                for (int i = 0; i < RowCount; i++)
                {
                    //ERPDonViVTEntity item = tblrow1[lstRowUpdate[i]];
                    ERPDonViVTEntity item = gridViewDonViVT.GetRow(i) as ERPDonViVTEntity;
                    if (item != null && !string.IsNullOrEmpty(item.TenDVVT))
                    {
                        _lstUpdate.Add(item);
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0) /*|| !Validatemamau(_lstUpdate)*/)
                {
                    XtraMessageBox.Show("Có đơn vị rỗng. Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                clsWriteLogThuVienLib.WriteLog(URL, _clientExtension, lstLog, "ERP_DonViVT");
                string url = string.Format("{0}?", URL + "ERPThuVienVT/PostDVT");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstDVVT); }).Result;
                if (msResult.ToLower() == "true")
                {
                    LoadDSDonViVT(false);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
               // gridViewDonViVT.OptionsBehavior.Editable = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
            this.DialogResult = DialogResult.OK;
        }
        private bool KiemTraTrung(List<ERPDonViVTEntity> danhSach)
        {
            return danhSach
               .GroupBy(x => (x.TenDVVT ?? "").Trim().Replace(" ", "").ToUpper())
                .Any(g => g.Count() > 1);
        }
    
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
        }
        private async void XoaDong()
        {
            try
            {
                int[] selectedHandles = gridViewDonViVT.GetSelectedRows();
                if (selectedHandles.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa các dòng đã chọn không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                // Duyệt ngược để tránh lỗi handle
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    ERPDonViVTEntity dr = gridViewDonViVT.GetRow(rowHandle) as ERPDonViVTEntity;
                    if (dr == null) continue;

                    string madvvt = dr.MaDVVT?.ToString();
                    string urlCheck = $"{URL}ERPThuVienVT/GetCheckDV?madvvt={madvvt}";
                    string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlCheck)).Result;

                    if (jsonCheck != "[]")
                    {
                        XtraMessageBox.Show(
                            $"Đơn vị '{madvvt}' đã được sử dụng. Không thể xóa!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        continue;
                    }

                    int id = Convert.ToInt32(dr.ID);
                    string url = $"{URL}ERPThuVienVT/DeleteDVT?id={id}&user={GlobleData.UserName}";
                    string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                    if (result.ToLower() == "true")
                    {
                        // Xóa khỏi grid
                        gridViewDonViVT.DeleteRow(rowHandle);
                    }
                    else
                    {
                        XtraMessageBox.Show(result, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Reload lại danh sách nếu cần
                LoadDSDonViVT(false);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng kiểm tra lại và thực hiện lại!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridViewDonViVT.OptionsBehavior.Editable = true;

                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        //actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        //actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        //actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        //actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gridViewDonViVT.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        //actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        //actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        //actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        //actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    gridViewDonViVT.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        //actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        //actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        //actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        //actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
        private void LoadDSDonViVT(bool isFromSave)
        {
            try
            {
                colTiLe.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colTiLe.DisplayFormat.FormatString = "F4";  // hoặc "F2"

                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "ERPThuVienVT/GetDVVT");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstDonViVTEntity = JsonConvert.DeserializeObject<List<ERPDonViVTEntity>>(json);
                }
                gridDonViVT.DataSource = lstDonViVTEntity.OrderBy(x => x.TenDVVT).ToList();

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewDonViVT.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridDonViVT;
                }
                //gridViewDonViVT.OptionsBehavior.Editable = true;
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void gridViewDonViVT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Tile" && e.Value != null && e.Value != DBNull.Value)
            {
                double value;
                if (double.TryParse(e.Value.ToString(), out value))
                {
                  
                    double rounded = Math.Round(value, 4, MidpointRounding.AwayFromZero);

               
                    if (rounded % 1 == 0)
                        e.DisplayText = ((int)rounded).ToString();
                    else
                        e.DisplayText = rounded.ToString("0.####"); 
                }
            }
            #region Thoai hiển thị username --> tên
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
            #endregion
        }
        private void grv_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

     

        private void grv_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewDonViVT.FocusedColumn == colTiLe)
            {
                string strTiLe = e.Value?.ToString()?.Trim() ?? "";

             
                if (!Regex.IsMatch(strTiLe, @"^[+-]?(\d+(\.\d+)?|\.\d+)$") && !string.IsNullOrEmpty(strTiLe))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập tỉ lệ đúng định dạng số thập phân!";
                }
                else if (!float.TryParse(strTiLe, out float tiLe))
                {
                    e.Valid = false;
                    e.ErrorText = "Không thể chuyển đổi tỉ lệ sang số thực!";
                }
                else if (tiLe < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập tỉ lệ lớn hơn 0!";
                }
                else
                {
                    // Hợp lệ: Làm tròn giá trị trước khi gán
                    e.Value = Math.Round(tiLe, 4);
                }
            }
        }



        private void txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void gV_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView gridView = sender as GridView;

            if (gridView != null && gridView.FocusedColumn != null)
            {         
                if (gridView.FocusedColumn == colTenDVVT)
                {
                    if (Char.IsLetter(e.KeyChar))

                        e.KeyChar = Char.ToUpper(e.KeyChar);
                }
            }
        }


        private void gC_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gV_KeyPress(grid.FocusedView, e);
        }
        private void gC_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {

                GridControl grid = sender as GridControl;
                GridView view = grid.FocusedView as GridView;
                if (view != null && view.OptionsBehavior.Editable)
                {
                    string clipboardText = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(clipboardText) && (clipboardText.Contains("\n") || clipboardText.Contains("\r")))
                    {
                        string singleLineText = Regex.Replace(clipboardText, @"\r\n?|\n", " ");
                        view.ShowEditor();
                        if (view.ActiveEditor is DevExpress.XtraEditors.TextEdit editor)
                        {
                            editor.SelectedText = singleLineText;
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        else
                        {
                            view.SetFocusedValue(singleLineText);
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                    }
                }
            }
        }

        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
            this.Close();
        }

        /*Write log when modify row*/
        List<LogThuvienEntity> lstLog = new List<LogThuvienEntity>();
        private void gridViewDonViVT_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            ERPDonViVTEntity row_changed = view.GetRow(e.RowHandle) as ERPDonViVTEntity;
            if (row_changed == null) return;
            if (row_changed.ID > 0)
            {
                row_changed.NguoiSua = GlobleData.UserName;
                row_changed.NgaySua = DateTime.Now;
            }
            else
            {
                if (string.IsNullOrEmpty(row_changed.NguoiTao))
                {
                    row_changed.NguoiTao = GlobleData.UserName;
                    row_changed.NgayTao = DateTime.Now;
                }
            }

            lstRowUpdate.Add(e.RowHandle);
            if (row_changed != null && row_changed.ID > 0)
            {
                string content = clsWriteLogThuVienLib.FormatRow(row_changed);
                var Query = lstLog.FirstOrDefault(x => x.ID == row_changed.ID);
                if (Query != null)
                {
                    Query.Content = content;
                }
                else
                {
                    lstLog.Add(new LogThuvienEntity
                    {
                        ID = row_changed.ID,
                        Action = $"Sửa {this.Text}",
                        Module = this.Name,
                        Content = content,
                        UserID = GlobleData.UserName,
                        CreatedDate = DateTime.Now

                    });
                }

            }
        }
        #region Thoai
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
        #endregion
    }
}