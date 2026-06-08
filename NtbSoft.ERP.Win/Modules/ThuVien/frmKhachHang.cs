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
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmKhachHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        //List<KhachHangEntity> lstQuocGia;
        //DataTable tbl;

        List<int> lstRowUpdate = new List<int>();
        List<KhachHangEntity> lstKhachHangEntity;
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
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();

        public frmKhachHang()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstKhachHangEntity = new List<KhachHangEntity>();
            lstThuVienDaDung = GET_ListTableThuVienDaDung();
        }
        public List<DataTable> GET_ListTableThuVienDaDung()
        {
            string urlGetListDataTable = URL + "GetThuVien/GetThuVienDaDung";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetListDataTable); }).Result;
            List<DataTable> _ListDataTable = JsonConvert.DeserializeObject<List<DataTable>>(jsonLstDataTable);
            return _ListDataTable;
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
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private void gridKhachHang_ProcessGridKey(object sender, KeyEventArgs e)
        {



        }

        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            #region Thoai gettennv
            tenNVHienThi = GetTenNhanVien();
            #endregion
            LoadDSKhachHang(false);
            CreateSearchlookup();
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
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View: //gridViewKhachHang.OptionsBehavior.Editable = false; 
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
                case ResourceURL.EventStatus.Edit:gridViewKhachHang.OptionsBehavior.Editable = true;
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
                    case ResourceURL.EventStatus.Add:gridViewKhachHang.OptionsBehavior.Editable = true;
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
        private void LoadDSKhachHang(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstKhachHangEntity = JsonConvert.DeserializeObject<List<KhachHangEntity>>(json);
                }

                gridKhachHang.DataSource = lstKhachHangEntity;
                //CreateSearchlookup();
                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewKhachHang.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridKhachHang;
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private void ThemDong()
        {
            KhachHangEntity obj = new KhachHangEntity();
            //_bindingHangHoaEntity.Add(obj);
            if (lstKhachHangEntity.Count > 0)
            {

                var maxMaKH = lstKhachHangEntity
                    .Where(x => !string.IsNullOrEmpty(x.MaKH))
                    .Select(x => {
                        int num;
                        return int.TryParse(x.MaKH.Replace("VKVN-", ""), out num) ? num : 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max();

                obj.MaKH = "VKVN-" + (maxMaKH + 1).ToString("D2");
            }
            else
            {
                obj.MaKH = "VKVN-01";
            }
            var currentUser = GlobleData.UserName;
            string tenNV = currentUser;
            if (!string.IsNullOrEmpty(currentUser))
            {
                obj.NguoiTao = currentUser;
                string upperUser = currentUser.ToUpper();
                if (tenNV != null && tenNV.Contains(upperUser)) tenNV = tenNVHienThi[upperUser];
            }
            lstKhachHangEntity.Add(obj);
            gridKhachHang.RefreshDataSource();
            _rowAdd = gridViewKhachHang.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewKhachHang.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            Them.Enabled = false;
            Sua.Enabled = false;
        }

        private void NapLaiDong()
        {
            LoadDSKhachHang(false);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            gridViewKhachHang.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            focused(this.gridViewKhachHang);
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
                    lstThuVienDaDung = GET_ListTableThuVienDaDung();

                    KhachHangEntity row = gridViewKhachHang.GetRow(gridViewKhachHang.FocusedRowHandle) as KhachHangEntity;

                    string maKH_delete = row.MaKH;

                    DataTable _dtKhachHang = lstThuVienDaDung[1];
                    DataTable _dthh = lstThuVienDaDung[6];
                    bool isAcceptDelete = true;
                    var rowmh = _dtKhachHang.AsEnumerable().FirstOrDefault(x => x["MaKH"].ToString() == maKH_delete);
                    for (int i = 0; i < _dtKhachHang.Rows.Count; i++)
                    {
                        if (maKH_delete == _dtKhachHang.Rows[i]["MaKH"].ToString())
                            isAcceptDelete = false;
                        if (_dthh.AsEnumerable().Any(x => x["MaKH"].ToString() == maKH_delete))
                        {
                            isAcceptDelete = false;
                        }
                    }
                    if (isAcceptDelete == false)
                    {
                        string message = string.Format(@"Khách hàng đang được sử dụng, không thể xóa!") ;
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string url = string.Format("{0}?Parameter={1}", URL + "KhachHang/DeleteKhachHang", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSKhachHang(false);
                    else XtraMessageBox.Show(result);
                }
            }catch(Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewKhachHang.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewKhachHang.FocusedRowHandle;
                }
                List<KhachHangEntity> _lstUpdate = new List<KhachHangEntity>();
                //this.ActiveControl = this.button1;
                KhachHangEntity row = gridViewKhachHang.GetRow(gridViewKhachHang.FocusedRowHandle) as KhachHangEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                    
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(item => item).ToList();
                foreach (int rowIndex in lstRowUpdate)
                {
                    KhachHangEntity item = gridViewKhachHang.GetRow(rowIndex) as KhachHangEntity;
                    if (item != null)
                    {
                        // Kiểm tra nếu TenKH trống (bạn đã có logic này)
                        if (string.IsNullOrEmpty(item.TenKH))
                        {
                            // Chuyển focus đến dòng và cột bị lỗi
                            gridViewKhachHang.FocusedRowHandle = rowIndex;
                            gridViewKhachHang.FocusedColumn = gridViewKhachHang.Columns["TenKH"];
                            XtraMessageBox.Show($"Dòng {rowIndex + 1}: Tên khách hàng không được để trống.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Dừng hàm
                        }

                        // **Kiểm tra nếu VietTat trống**
                        if (string.IsNullOrEmpty(item.VietTat))
                        {
                            // Chuyển focus đến dòng và cột bị lỗi
                            gridViewKhachHang.FocusedRowHandle = rowIndex;
                            gridViewKhachHang.FocusedColumn = gridViewKhachHang.Columns["VietTat"];
                            XtraMessageBox.Show($"Dòng {rowIndex + 1}: Mã viết tắt không được để trống.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Dừng hàm, không lưu nữa
                        }
                    }
                }

                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    KhachHangEntity item = gridViewKhachHang.GetRow(lstRowUpdate[i]) as KhachHangEntity;
                    if (item != null && !string.IsNullOrEmpty(item.TenKH))
                    {
                        _lstUpdate.Add(item);
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
                string url = string.Format("{0}?", URL + "KhachHang/PostKhachHang");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
                if (msResult.ToLower() == "true")
                {
                    LoadDSKhachHang(false);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                _status = ResourceURL.EventStatus.View;
                GridViewUpdateStatus(_status);
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private bool ValidateKhachHang(List<KhachHangEntity> lstKhachHang)
        {
            // Danh sách chứa các thông báo lỗi
            List<string> errorMessages = new List<string>();

            foreach (var row in lstKhachHang)
            {
               
               

                // Kiểm tra cột TenKH
                if (string.IsNullOrEmpty(row.TenKH))
                {
                    errorMessages.Add($"Tên khách hàng không được để trống!");
                }

                // Kiểm tra cột MaDT
                //if (string.IsNullOrEmpty(row.MaDT))
                //{
                //    errorMessages.Add($"Mã khách hàng không được để trống!");
                //}

                // Kiểm tra cột NguoiDaiDien
                //if (string.IsNullOrEmpty(row.NguoiDaiDien))
                //{
                //    errorMessages.Add($"Người đại diện không được để trống!");
                //}

                // Kiểm tra trùng lặp
                //bool isDuplicate = lstKhachHangEntity.Any(kh => kh.MaDT == row.MaDT && kh != row);
                //if (isDuplicate&&row.MaDT!="")
                //{
                //    errorMessages.Add($"Mã khách hàng đã bị trùng. Vui lòng nhập mã khác!");
                //}
            }

            // Nếu có lỗi, hiển thị toàn bộ thông báo lỗi và trả về false
            if (errorMessages.Count > 0)
            {
                string errorMessage = string.Join("\n", errorMessages); // Ghép tất cả lỗi thành 1 chuỗi
                MessageBox.Show(errorMessage, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Nếu không có lỗi, trả về true
            return true;
        }
        private void gridViewKhachHang_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            #region Thoai hiển thị tên nv
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle < 0) return;
            KhachHangEntity item = view.GetRow(e.RowHandle) as KhachHangEntity;
            if (item == null) return;
            var currentUser = GlobleData.UserName;
            if (string.IsNullOrEmpty(currentUser)) return;

            if (item.ID == 0 || item.ID == null)
            {
                if (string.IsNullOrEmpty(item.NguoiTao)) item.NguoiTao = currentUser;
            }
            else
            {
                item.NguoiSua = currentUser;
            }
            #endregion
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }
        #region Thoai thay đổi hiển thị username -> tên
        private void gridViewKhachHang_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
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
        }
        #endregion
        private void gridViewKhachHang_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }

        private void gridViewKhachHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colMaKH)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;

                }
            }
        }

        private void gridViewKhachHang_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewKhachHang_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridViewKhachHang_KeyPress(object sender, KeyPressEventArgs e)
        {
           GridView view = sender as GridView;

            HangHoaEntity row = gridViewKhachHang.GetRow(gridViewKhachHang.FocusedRowHandle) as HangHoaEntity;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            // Kiểm tra xem ký tự được nhập vào có phải là chữ cái hay không
            // char.IsLetterOrDigit -> Kiểm tra phải số hoặc chữ cái không
            // char.IsControl -> Kiểm tra phải kí tự điều khiển không( control character)
            //if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
            //    (view.FocusedColumn == colMaKH || view.FocusedColumn == colTenKH))
            //{
            //    // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
            //    if (char.IsWhiteSpace(e.KeyChar))
            //    {
            //        if (view.FocusedColumn == colMaKH)
            //        {
            //            e.Handled = true;
            //        }
            //        else if (view.FocusedColumn == colTenKH)
            //        {
            //            e.Handled = false;
            //        }
            //    }
            //    else
            //    {
            //        e.Handled = true;
            //    }

            //    // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
            //    if (!char.IsPunctuation(e.KeyChar))
            //    {
            //        if (view.FocusedColumn == colMaKH)
            //        {
            //            e.Handled = true;
            //        }
            //        else if (view.FocusedColumn == colTenKH)
            //        {
            //            e.Handled = false;
            //        }
            //    }
            //    else
            //    {
            //        e.Handled = true;
            //    }
            //}

            if (view.FocusedColumn == colMaKH || view.FocusedColumn == colTenKH || view.FocusedColumn == colGhiChu|| view.FocusedColumn==colKH_VTat)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
            if (view.FocusedColumn == colSDT || view.FocusedColumn == colMaSoThue)
            {
                
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void gridKhachHang_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gridViewKhachHang_KeyPress(grid.FocusedView, e);
        }

        private void gridViewKhachHang_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            
            if (view != null && view.GetFocusedDataSourceRowIndex() >= 0)
            {
                if (view.FocusedColumn == this.colMaKH)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Mã khách hàng không được để trống.!";
                    }
                    else
                    {
                        List<KhachHangEntity> LstKhachHang = gridViewKhachHang.DataSource as List<KhachHangEntity>;
                        KhachHangEntity khachhang = LstKhachHang.Where(item => item.MaKH == e.Value.ToString()).FirstOrDefault();
                        if (khachhang != null)
                        {
                            e.Valid = false;
                            e.ErrorText = "Mã khách hàng đã bị trùng. Vui lòng nhập mã khách hàng mới.!";
                        }
                        if (ContainsVietnamese(e.Value.ToString()))
                        {
                            e.Valid = false;
                            e.ErrorText = "Mã khách hàng không được chứa dấu tiếng việt.!";
                        }
                    }
                }
                else if (view.FocusedColumn == this.colTenKH)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Khách hàng không được để trống.!";
                    }

                    List<KhachHangEntity> LstKhachHang = gridViewKhachHang.DataSource as List<KhachHangEntity>;
                    KhachHangEntity khachHang = LstKhachHang.Where(item => CheckDupLiCateString(item.TenKH, e.Value.ToString())).FirstOrDefault();
                    if (khachHang != null)
                    {
                        e.Valid = false;
                        e.ErrorText = $"Khách Hàng {khachHang.TenKH} đã có!";
                    }

                }
                //else if (view.FocusedColumn == this.colMaDT)
                //{
                //    if (string.IsNullOrEmpty(e.Value.ToString()))
                //    {
                //        e.Valid = false;
                //        e.ErrorText = "Mã đối tác không được để trống.!";
                //    }
                //    else
                //    {
                //        List<KhachHangEntity> LstKhachHang = gridViewKhachHang.DataSource as List<KhachHangEntity>;
                //        KhachHangEntity khachHang = LstKhachHang.Where(item => CheckDupLiCateString(item.MaDT, e.Value.ToString())).FirstOrDefault();
                //        if (khachHang != null)
                //        {
                //            e.Valid = false;
                //            e.ErrorText = "Mã đối tác đã bị trùng. Vui lòng nhập Mã khách hàng khác.!";
                //        }
                //    }


                //}
                else if (view.FocusedColumn == this.colNguoiDaiDien)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Người đại diện không được để trống.!";
                    }

                }
                else if (view.FocusedColumn == colSDT)
                {
                    // Kiểm tra độ dài tối đa cho SDT
                    if (e.Value != null && e.Value.ToString().Length > 10)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số điện thoại không được vượt quá 10 ký tự.";
                    }
                }
                else if (view.FocusedColumn == colMaSoThue)
                {
                    // Kiểm tra độ dài tối đa cho MaSoThue
                    if (e.Value != null && e.Value.ToString().Length > 13)
                    {
                        e.Valid = false;
                        e.ErrorText = "Mã số thuế không được vượt quá 13 ký tự.";
                    }
                }
                else if (view.FocusedColumn == colKH_VTat)
                {
                    List<KhachHangEntity> LstKhachHang = gridViewKhachHang.DataSource as List<KhachHangEntity>;
                    KhachHangEntity khachHang = LstKhachHang.Where(item => CheckDupLiCateString(item.VietTat, e.Value.ToString())).FirstOrDefault();
                    if (khachHang != null)
                    {
                        e.Valid = false;
                        e.ErrorText = $"Từ viết tắt {khachHang.VietTat} đã có. Vui lòng nhập từ viết tắt khác.!";
                    }
                }
             
               /* if (e.Valid == false)
                    Luu.Enabled = false;
                else Luu.Enabled = true;*/
            }
        }

        private bool CheckDupLiCateString(string firstString, string secondString)
        {
            if (firstString == null || (firstString != null && string.IsNullOrEmpty(firstString)))
            {
                return false;
            }
            return firstString.Replace(" ", "").ToUpper().Equals(secondString.Replace(" ", "").ToUpper());
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private void gridViewKhachHang_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string thuVien = "KhachHang";
            frmImPortExcelThuVien f = new frmImPortExcelThuVien(thuVien);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();

            //OpenFileDialog Sfd = new OpenFileDialog();
            //Sfd.Title = "File To Save";
            //Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            //if (Sfd.ShowDialog() == DialogResult.OK)
            //{
            //    ReadExcelPL(Sfd.FileName);
            //}
            LoadDSKhachHang(false);
        }

        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                lstKhachHangEntity.Clear();
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

            string TenKH = string.Empty, MaDT = string.Empty, SDT = string.Empty, GhiChu = string.Empty, Email = string.Empty, MaKH = string.Empty, DiaChi = string.Empty, MaSoThue = string.Empty, NguoiDaiDien = string.Empty;
            int MaLoaiDT;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstKhachHangEntity = JsonConvert.DeserializeObject<List<KhachHangEntity>>(json);
            }

            List<string> danhsachtrung = new List<string>();
            foreach (DataRow row in dtSave.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                MaDT = row[0].ToString();
                TenKH = row[1].ToString();
                SDT = row[2].ToString();
                Email = row[3].ToString();
                DiaChi = row[4].ToString();
                MaSoThue = row[5].ToString();
                NguoiDaiDien =row[6].ToString();
                GhiChu = row[7].ToString();
                MaLoaiDT = 1;

                bool Istrung = lstKhachHangEntity.Any(x => x.MaDT == MaDT);
                if (Istrung)
                {
                    //XtraMessageBox.Show($"Mã Khách Hàng bị trùng: {MaDT}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    danhsachtrung.Add(MaDT);
                }
                else
                {
                    lstKhachHangEntity.Add(new KhachHangEntity
                    {
                        TenKH = TenKH,
                        GhiChu = GhiChu,
                        MaDT = MaDT,
                        SDT = SDT,
                        Email = Email,
                        DiaChi = DiaChi,
                        MaSoThue = MaSoThue,
                        NguoiDaiDien = NguoiDaiDien,
                        MaLoaiDT = MaLoaiDT
                    });
                    string msResult = "";
                    string urlpost = string.Format("{0}?", URL + "KhachHang/PostKhachHang");
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlpost, lstKhachHangEntity); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        LoadDSKhachHang(false);
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else XtraMessageBox.Show(msResult);
                    //_status = ResourceURL.EventStatus.View;
                    //GridViewUpdateStatus(_status);
                    lstRowUpdate.Clear();
                }
            }
            if (danhsachtrung.Any())
            {
                string dstrungmadt = string.Join(", ", danhsachtrung.Distinct());
                XtraMessageBox.Show($"Các mã Khách Hàng bị trùng:{dstrungmadt}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void gridViewKhachHang_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewKhachHang_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("MauImportThuVien_{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Hiển thị form chờ
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Đang xuất mẫu Excel...");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Vui lòng đợi trong giây lát...");

                    // Đường dẫn file mẫu gốc
                    string templateName = "MauImportThuVien.xlsx";
                    string templatePath = Path.Combine(Application.StartupPath, "Templates", templateName);

                    if (!File.Exists(templatePath))
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file mẫu: " + templatePath, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Copy file mẫu ra vị trí người dùng chọn
                    File.Copy(templatePath, Sfd.FileName, true);

                    // Đóng form chờ
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                    // Hỏi người dùng có muốn mở file không
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Xuất thành công! Bạn có muốn mở file không?", "Hoàn tất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(Sfd.FileName);
                        }
                        catch
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không thể mở file vừa xuất.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    DevExpress.XtraEditors.XtraMessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {


                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "ThongSoVatTu";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {

                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void grv_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


        }

        private void repoBrand_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            KhachHangEntity currentRow = gridViewKhachHang.GetFocusedRow() as KhachHangEntity;

            if (currentRow == null)
            {
                return;
            }
            string makh = currentRow.MaKH;

            if (string.IsNullOrEmpty(makh))
            {
                return;
            }
            frmBrand frm = new frmBrand(makh);
            frm.WindowState = FormWindowState.Normal;
            frm.ShowDialog();
        }

        private void gridViewKhachHang_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (_status == ResourceURL.EventStatus.View)
            {
                if (view.FocusedColumn != colBrand)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CreateSearchlookup() 
        {
            string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenQG";
            rCountryEdit.ValueMember = "MaQG";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã quốc gia", Name = "colMaQG", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Tên quốc gia", Name = "colTenQG", Visible = true });

            }
            colMaQG.ColumnEdit = rCountryEdit;
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
