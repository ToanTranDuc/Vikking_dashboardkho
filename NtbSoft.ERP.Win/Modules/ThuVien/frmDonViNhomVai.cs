using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Printing;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmDonViNhomVai : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        string CheckStatus = "0"; //0 là Thêm, 1 là sửa, 2 là view dữ liệu
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        //List<QuocGiaEntity> lstQuocGia;
        //DataTable tbl;

        List<int> lstRowUpdate = new List<int>();
        List<DonViEntity> lstDonViEntity;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;

        public frmDonViNhomVai()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonViEntity = new List<DonViEntity>();
            List<string> items = new List<string>
            { "Yard", "Met"};

            // Gán danh sách mục cho ComboBox
            repositoryItemComboBox1.Items.AddRange(items);
            repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(BtThem, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(BtSua, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(Xoadulieu, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(Luudulieu, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
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
          
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSDonVi(false);
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
                    gridViewDonVi.OptionsBehavior.Editable = false;

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
                    gridViewDonVi.OptionsBehavior.Editable = true;
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
                    gridViewDonVi.OptionsBehavior.Editable = true;
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

        private void LoadDSDonVi(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "DonVi/GetDonVi");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    lstDonViEntity = JsonConvert.DeserializeObject<List<DonViEntity>>(json);
                }
                
                gridDonVi.DataSource = lstDonViEntity;

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gridViewDonVi.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridDonVi;
                }
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void BtThem()
        {
            DonViEntity obj = new DonViEntity();
            //_bindingHangHoaEntity.Add(obj);
            lstDonViEntity.Add(obj);

            _rowAdd = gridViewDonVi.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gridViewDonVi.FocusedRowHandle = _rowAdd;
        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtThem();
        }


        private void NapLaiDong()
        {
            LoadDSDonVi(false);
            //gridViewDonVi.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void BtSua()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            focused(this.gridViewDonVi);
        }
        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtSua();
        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Xoadulieu();
        }
        private async void Xoadulieu()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    DonViEntity row = gridViewDonVi.GetRow(gridViewDonVi.FocusedRowHandle) as DonViEntity;
                    string url = string.Format("{0}?Parameter={1}", URL + "DonVi/DeleteDonVi", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSDonVi(false);
                    else XtraMessageBox.Show(result);
                }
            }catch(Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Luudulieu();
        }
        private void repositoryItemComboBox1_Validating(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            DonViEntity row = gridViewDonVi.GetRow(gridViewDonVi.FocusedRowHandle) as DonViEntity;
            // Kiểm tra giá trị hợp lệ
            if (row.MaDV == null)
            {
                e.Valid = false;
                e.ErrorText = "Mã đơn vị không được bỏ trống.!";
                return;
            }
            if (row.TenDV == null)
            {
                e.Valid = false;
                e.ErrorText = "Tên đơn vị không được bỏ trống.!";
                return;
            }
            if (row.NhomDV == null)
            {
                e.Valid = false;
                e.ErrorText = "Nhóm đơn vị không được bỏ trống.!";
                return;
            }
        }
        private void Luudulieu()
        {
            try
            {
                this.ActiveControl = button1;
                // Set hàng cần focused
                if ((_status != ResourceURL.EventStatus.View) && gridViewDonVi.FocusedRowHandle >= 0)
                {
                    FocusedIndex = gridViewDonVi.FocusedRowHandle;
                }

                List<DonViEntity> _lstUpdate = new List<DonViEntity>();
                //this.ActiveControl = this.button1;
                DonViEntity row = gridViewDonVi.GetRow(gridViewDonVi.FocusedRowHandle) as DonViEntity;

                if (row == null)
                {
                    XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (CheckStatus == "0")
                {
                    //if (row.MaDV == null)
                    //{
                    //    DialogResult messResult = MessageBox.Show("Vui lòng nhập mã đơn vị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return;
                    //}
                    //if (row.TenDV == null)
                    if (string.IsNullOrEmpty(row.TenDV))
                    {
                        DialogResult messResult = MessageBox.Show("Vui lòng nhập tên đơn vị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //if (row.NhomDV == null)
                    if (string.IsNullOrEmpty(row.NhomDV))
                    {
                        DialogResult messResult = MessageBox.Show("Vui chọn nhóm đơn vị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    DonViEntity item = gridViewDonVi.GetRow(lstRowUpdate[i]) as DonViEntity;
                    if (item != null && !string.IsNullOrEmpty(item.TenDV))
                    {
                        //item.ID = 0;
                        item.MaDV = ReplaceTenDonVi(item.TenDV);
                        _lstUpdate.Add(item);
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
                string url = string.Format("{0}?", URL + "DonVi/PostDonVi");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                if (msResult.ToLower() == "true")
                {
                    LoadDSDonVi(true);
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

        private void gridViewDonVi_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }

        private void gridViewDonVi_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }

        private void gridViewDonVi_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
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
                    if (view.FocusedColumn == colMaDV)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
        }

        private void gridViewDonVi_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewDonVi_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //focused(sender);
            //Console.WriteLine("customDrawHeader");
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

        private void gridViewDonVi_KeyPress(object sender, KeyPressEventArgs e)
        {
            GridView view = sender as GridView;

            //HangHoaEntity row = gridViewDonVi.GetRow(gridViewDonVi.FocusedRowHandle) as HangHoaEntity;

            // chặn không cho nhập kí tự đặc biệt vào cột Mã Hàng và Tên Hàng
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            // Kiểm tra xem ký tự được nhập vào có phải là chữ cái hay không
            // char.IsLetterOrDigit -> Kiểm tra phải số hoặc chữ cái không
            // char.IsControl -> Kiểm tra phải kí tự điều khiển không( control character)
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar) &&
                (view.FocusedColumn == colMaDV || view.FocusedColumn == colTenDV))
            {
                // Nếu là ký tự đặc biệt, chặn sự kiện và không cho phép nhập
                if (char.IsWhiteSpace(e.KeyChar))
                {
                    if (view.FocusedColumn == colMaDV)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenDV)
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
                    if (view.FocusedColumn == colMaDV)
                    {
                        e.Handled = true;
                    }
                    else if (view.FocusedColumn == colTenDV)
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }

            if (view.FocusedColumn == colMaDV || view.FocusedColumn == colTenDV || view.FocusedColumn == colGhiChu)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void gridDonVi_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            gridViewDonVi_KeyPress(grid.FocusedView, e);
        }

        private void gridViewDonVi_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridViewDonVi.GetFocusedDataSourceRowIndex() >= 0)
            {
                if (gridViewDonVi.FocusedColumn == this.colTenDV)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Thông tin không được để trống.!";
                    }
                    else
                    {
                        List<DonViEntity> LstDonVi = gridViewDonVi.DataSource as List<DonViEntity>;
                        DonViEntity donvi = LstDonVi.Where(item => item.MaDV != null && item.MaDV != gridViewDonVi.GetFocusedRowCellValue(colMaDV) && (item.NhomDV != null && item.NhomDV.Equals(gridViewDonVi.GetFocusedRowCellValue(colNhomDV))) && (item.TenDV != null && item.TenDV.Equals(e.Value.ToString()))).FirstOrDefault();
                        if (donvi != null)
                        {
                            e.Valid = false;
                            e.ErrorText = "Tên đơn vị và nhóm đơn vị đã trùng lập. Vui lòng kiểm tra lại.!";
                        }
                    }
                }
                if (gridViewDonVi.FocusedColumn == this.colNhomDV)
                {
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Valid = false;
                        e.ErrorText = "Nhóm đơn vị không được để trống.!";
                    }
                    else
                    {
                        List<DonViEntity> LstDonVi = gridViewDonVi.DataSource as List<DonViEntity>;
                        DonViEntity donvi = LstDonVi.Where(item => item.MaDV != null && item.MaDV != gridViewDonVi.GetFocusedRowCellValue(colMaDV) && (item.NhomDV != null && item.NhomDV.Equals(e.Value.ToString())) && (item.TenDV != null && item.TenDV.Equals(gridViewDonVi.GetFocusedRowCellValue(colTenDV)))).FirstOrDefault();
                        if (donvi != null)
                        {
                            e.Valid = false;
                            e.ErrorText = "Tên đơn vị và nhóm đơn vị đã trùng lập. Vui lòng kiểm tra lại.!";
                        }
                    }
                }
                if (e.Valid == false)
                    Luu.Enabled = false;
                else Luu.Enabled = true;
            }
        }

        public bool ContainsVietnamese(string input)
        {
            Regex vietnameseRegex = new Regex(@"[^\u0000-\u007F]+");
            return vietnameseRegex.IsMatch(input);
        }

        private string ReplaceTenDonVi(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Sử dụng regex để chuyển đổi dấu thành không dấu
            string pattern = @"\p{IsCombiningDiacriticalMarks}+";
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            string result = Regex.Replace(normalizedString, pattern, string.Empty);
            result = Regex.Replace(result, @"Đ", "D");
            result = result.Replace(" ", "_");

            // Thêm hậu tố "_"+_nextID
            //result +=string.Format("{0}_{1}");
            //return string.Format("{0}_{1}",result,_nextID);
            return result;

        }

        private void gridViewDonVi_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewDonVi_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridViewDonVi_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
    }
}
