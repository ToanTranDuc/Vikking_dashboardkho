using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
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
    public partial class frmDonViChungLoai : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        //List<QuocGiaEntity> lstQuocGia;
        //DataTable tbl;

        List<int> lstRowUpdate = new List<int>();
        List<DonViChungLoaiEntity> lstDVCLEntity;
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


        public frmDonViChungLoai()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDVCLEntity = new List<DonViChungLoaiEntity>();
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
            LoadDSKhachHang(false);
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
                case ResourceURL.EventStatus.View:
                    gridViewKhachHang.OptionsBehavior.Editable = false;

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
                    gridViewKhachHang.OptionsBehavior.Editable = true;
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
                    gridViewKhachHang.OptionsBehavior.Editable = true;
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
                string url = string.Format("{0}?", URL + "DonViChungLoai/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstDVCLEntity = JsonConvert.DeserializeObject<List<DonViChungLoaiEntity>>(json);
                }
                gridKhachHang.DataSource = lstDVCLEntity;

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
            DonViChungLoaiEntity obj = new DonViChungLoaiEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstDVCLEntity.Add(obj);

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
                    DonViChungLoaiEntity row = gridViewKhachHang.GetRow(gridViewKhachHang.FocusedRowHandle) as DonViChungLoaiEntity;
                    string url = string.Format("{0}?Parameter={1}", URL + "DonViChungLoai/Delete", row.ID);
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
                List<DonViChungLoaiEntity> _lstUpdate = new List<DonViChungLoaiEntity>();
                //this.ActiveControl = this.button1;
                DonViChungLoaiEntity row = gridViewKhachHang.GetRow(gridViewKhachHang.FocusedRowHandle) as DonViChungLoaiEntity;
                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }

                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(item => item).ToList();


                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    DonViChungLoaiEntity item = gridViewKhachHang.GetRow(lstRowUpdate[i]) as DonViChungLoaiEntity;
                    if (item != null && !string.IsNullOrEmpty(item.TenDVCL))
                    {
                        _lstUpdate.Add(item);
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
                string url = string.Format("{0}?", URL + "DonViChungLoai/Post");
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
        private void gridViewKhachHang_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }

        private void gridViewKhachHang_CellValueChanging(object sender, CellValueChangedEventArgs e)
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
                    if (view.FocusedColumn == colMaDVCL)
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
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                (view.FocusedColumn == colMaDVCL || view.FocusedColumn == colTenDVCL))
            {
                // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
                if (char.IsWhiteSpace(e.KeyChar))
                {
                    if (view.FocusedColumn == colMaDVCL)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenDVCL)
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }

                // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
                if (!char.IsPunctuation(e.KeyChar))
                {
                    if (view.FocusedColumn == colMaDVCL)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenDVCL)
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }

            if (view.FocusedColumn == colMaDVCL || view.FocusedColumn == colTenDVCL || view.FocusedColumn == colGhiChu)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void gridKhachHang_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gridViewKhachHang_KeyPress(grid.FocusedView, e);
        }

        private void gridViewKhachHang_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewKhachHang.GetFocusedDataSourceRowIndex() >= 0)
            {
                if (gridViewKhachHang.FocusedColumn == this.colMaDVCL)
                {
                    if (e.Value.ToString() == "")
                    {
                        e.Valid = false;
                        e.ErrorText = "Thông tin không được để trống.!";
                    }
                    else
                    {
                        //DataRow dr = ((DataTable)gridViewHangHoa.DataSource).AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();
                        //DataRow dr = gridViewHangHoa.DataSource as BindingList<HangHoaEntity>();
                        //    .AsEnumerable().Where(x => x["MaHangHoa"].ToString() == e.Value.ToString()).FirstOrDefault();
                        List<DonViChungLoaiEntity> LstKhachHang = gridViewKhachHang.DataSource as List<DonViChungLoaiEntity>;
                        DonViChungLoaiEntity khachhang = LstKhachHang.Where(item => item.MaDVCL == e.Value.ToString()).FirstOrDefault();
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
                else if (gridViewKhachHang.FocusedColumn == this.colTenDVCL)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Thông tin không được để trống!";
                    }
                    else
                    {
                        List<DonViChungLoaiEntity> LstDvcl = gridViewKhachHang.DataSource as List<DonViChungLoaiEntity>;
                        DonViChungLoaiEntity dvcl = LstDvcl.Where(item => CheckDupLiCateString(item.TenDVCL, e.Value.ToString())).FirstOrDefault();
                        if (dvcl != null)
                        {
                            e.Valid = false;
                            e.ErrorText = "Đơn vị chủng loại đã bị trùng. Vui lòng nhập tên đơn vị chủng loại khác.!";
                        }
                    }
                }
                if (e.Valid == false)
                    Luu.Enabled = false;
                else Luu.Enabled = true;
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

    }
}
