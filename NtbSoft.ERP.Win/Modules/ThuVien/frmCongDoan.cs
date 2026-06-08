using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmCongDoan : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;

        bool isValidData = true;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;


        public frmCongDoan()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadDSCongDoan();
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, true);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, true);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, true);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, true);

            lstActionControls = new List<ActionControl> {
                actionControlAdd,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
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
            if (!_allowAdd && !_allowEdit)
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;
        }

        private void LoadDSCongDoan()
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "CongDoan/GetCongDoan");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                DataTable tblCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
                gridControlCongDoan.DataSource = tblCongDoan;
                gridViewCongDoan.FocusedRowHandle = FocusedIndex;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void ThemDong()
        {
            // gridViewCongDoan.FocusedRowHandle = _rowAdd;
            this.ActiveControl = this.button1;
            gridViewCongDoan.AddNewRow();
            gridViewCongDoan.UpdateCurrentRow();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }

        private void XoaDong()
        {

        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gridViewCongDoan.FocusedRowHandle >= 0)
                    {
                        var id = gridViewCongDoan.GetFocusedRowCellValue(this.colID);

                        // Xóa trên view
                        gridViewCongDoan.DeleteRow(gridViewCongDoan.FocusedRowHandle);

                        // Xóa dưới dữ liệu
                        if (id != null && !string.IsNullOrEmpty(id.ToString()))
                        {
                            string url = string.Format("{0}?id={1}", URL + "CongDoan/DeleteCongDoan", id);
                            string msResult = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            if (msResult.ToLower() == "true")
                            {
                                // LoadDSCongDoan();
                                clsWaitForm.ShowSuccessForm(this, 1000);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LuuDong()
        {
            this.ActiveControl = this.button1;
            if (!isValidData)
                return;
            if (!CheckDataGridView())
                return;
            DataTable tblSave = (gridViewCongDoan.DataSource as DataView).Table;
            string url = string.Format("{0}?", URL + "CongDoan/PostCongDoan");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                LoadDSCongDoan();
                clsWaitForm.ShowSuccessForm(this, 1000);
            }
            else XtraMessageBox.Show(msResult);
            _status = ResourceURL.EventStatus.View;
        }

        private bool CheckDataGridView()
        {
            DataTable tbl = (gridViewCongDoan.DataSource as DataView).Table;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                DataRow row = tbl.Rows[i];
                // Validate 2 cột MaCongDoan, TenCongDoan
                var maCongDoan = row["MaCongDoan"];
                var tenCongDoan = row["TenCongDoan"];
                if (maCongDoan == null || (maCongDoan != null && string.IsNullOrEmpty(maCongDoan.ToString()))

                    || tenCongDoan == null || (tenCongDoan != null && string.IsNullOrEmpty(tenCongDoan.ToString())))
                {
                    // Vui lòng nhập đầy đủ thông tin trước khi lưu
                    gridViewCongDoan.FocusedRowHandle = i;
                    XtraMessageBox.Show("Vui lòng nhập đầy đủ thông tin Mã Công Đoạn và Tên Công Đoạn trước khi lưu.!", "Thông báo"
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void gridViewCongDoan_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (gridViewCongDoan.GetFocusedRowCellValue(this.colID) != null)
            {
                string id = gridViewCongDoan.GetFocusedRowCellValue(this.colID).ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    // Những dòng data đã có sẵn
                    if (_allowEdit)
                    {
                        if (gridViewCongDoan.FocusedColumn.Equals(this.colMaCD))
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void gridViewCongDoan_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridColumn focusedColumn = gridViewCongDoan.FocusedColumn;
            if (focusedColumn.Equals(this.colMaCD) && e != null)
            {
                // Check không để trống
                if (!(e.Value != null && !string.IsNullOrEmpty(e.Value.ToString())))
                {
                    e.ErrorText = "Mã công đoạn không được để trống";
                    e.Valid = false;
                    isValidData = false;
                    return;
                }
                else if (!CheckDuplicateData(focusedColumn, e.Value.ToString()))
                {
                    e.ErrorText = "Mã Công Đoạn đã bị trùng. Vui lòng nhập Mã Công Đoạn khác.!";
                    e.Valid = false;
                    isValidData = false;
                    return;
                }

            }
            else if (focusedColumn.Equals(this.colTenCD))
            {
                if (!(e.Value != null && !string.IsNullOrEmpty(e.Value.ToString())))
                {
                    e.ErrorText = "Tên công đoạn không được để trống";
                    e.Valid = false;
                    isValidData = false;
                    return;
                }
                else if (!CheckDuplicateData(focusedColumn, e.Value.ToString()))
                {
                    e.ErrorText = "Tên Công Đoạn đã bị trùng. Vui lòng nhập Tên Công Đoạn khác.!";
                    e.Valid = false;
                    isValidData = false;
                    return;
                }
            }
            isValidData = true;
        }

        private bool CheckDuplicateData(GridColumn column, string value)
        {
            DataTable tbl = (gridViewCongDoan.DataSource as DataView).Table;
            foreach (DataRow row in tbl.Rows)
            {
                if (tbl.Columns.Contains(column.FieldName)&&row[column.FieldName] != null
                    && row[column.FieldName].ToString().Equals(value))
                {
                    return false;
                }
            }
            return true;
        }

        private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridViewCongDoan_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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

        private void NapLaiDong()
        {
            LoadDSCongDoan();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void gridViewCongDoan_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridControlCongDoan_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.FocusedView as GridView;

            // chặn không cho nhập kí tự đặc biệt vào cột MaCongDoan và TenCongDoan
            // e.Handled = true -> là không được nhập vào
            // e.Handled = false -> là được nhập vào
            // mặc định e.Handled = false

            if (view.FocusedColumn == colMaCD)
            {
                e.Handled = CheckCharacterID(view, e);
            }
            else if (view.FocusedColumn == colTenCD)
            {
                e.Handled = CheckCharacterName(view, e);
            }

            Console.WriteLine("KeyPress: " + e.KeyChar);


            if (view.FocusedColumn == colMaCD)
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }

        }


        private bool CheckCharacterID(GridView view, KeyPressEventArgs e)
        {
            // 95 là giá trị của kí tự '_'. Chỉ cho phép nhập kí tự '_'
            int charEnable = 95;
            int charParse = (int)e.KeyChar;
            if (!charParse.Equals(charEnable) && !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check ID
                // Nếu là khoảng trắng -> Không cho nhập
                // Nếu là kí tự đặc biệt -> Không cho nhập
                // Nếu là dấu câu -> Không cho nhập
                if ((!char.IsSymbol(e.KeyChar) && !char.IsPunctuation(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)))
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


        private bool CheckCharacterName(GridView view, KeyPressEventArgs e)
        {
            // 95 là giá trị của kí tự '_'. Chỉ cho phép nhập kí tự '_'
            int charEnable = 95;
            int charParse = (int)e.KeyChar;
            if (!charParse.Equals(charEnable) && !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Check name
                // Nếu là khoảng trắng -> Cho nhập
                // Nếu là kí tự đặc biệt -> Không cho nhập
                // Nếu là dấu câu -> Không cho nhập
                if (char.IsWhiteSpace(e.KeyChar) || (!char.IsSymbol(e.KeyChar)
                    && !char.IsPunctuation(e.KeyChar)))
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
    }
}
