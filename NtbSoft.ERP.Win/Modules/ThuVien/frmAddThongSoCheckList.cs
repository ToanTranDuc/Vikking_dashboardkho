using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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
    public partial class frmAddThongSoCheckList : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;

        string _mahang = string.Empty;string _tenhang = string.Empty;
        int _ParentNO = 0;

        public frmAddThongSoCheckList( string MaHang,string TenHang)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _mahang = MaHang;
            _tenhang = TenHang;
          
            LoadCongDoanTS();
            SetupGroupLevelColors();

            grvThongSoCheckList.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            grvThongSoCheckList.OptionsBehavior.Editable = true; // Cho phép edit
            grvThongSoCheckList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            grvThongSoCheckList.OptionsSelection.EnableAppearanceFocusedCell = false;

            this.grvThongSoCheckList.NewItemRowText = "Thêm mới công đoạn";
            this.grvThongSoCheckList.Appearance.TopNewRow.ForeColor = Color.Blue;
            this.grvThongSoCheckList.Appearance.TopNewRow.Font = new Font("Tahoma", 9, FontStyle.Italic);

            grvThongSoCheckList.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True; // Giữ group rows fixed ở top khi scroll
        }

        private void SetupGroupLevelColors()
        {
            groupLevelColorBackground = new Dictionary<int, Color>
            {
                { -1, ColorTranslator.FromHtml("#DDEBFB") },
                { 0, ColorTranslator.FromHtml("#DDEBFB") },
                { 1, ColorTranslator.FromHtml("#FFE2D3") },
                { 2, ColorTranslator.FromHtml("#D7F5E8") }
            };

            groupLevelColors = new Dictionary<int, Color>
            {
                { -1, ColorTranslator.FromHtml("#2A5D9F") },
                { 0, ColorTranslator.FromHtml("#2A5D9F") },
                { 1, ColorTranslator.FromHtml("#A53E25") },
                { 2, ColorTranslator.FromHtml("#2E7D5B") }
            };


        }
        private DataTable CreateTable_CheckListCD()
        {
            
            DataTable  tblSave = new DataTable("tblCheckListTS");
           tblSave.Columns.Add("ID", typeof(int));
           tblSave.Columns.Add("CongDoan", typeof(string));
           tblSave.Columns.Add("ParentID", typeof(int));
           tblSave.Columns.Add("ParentNO", typeof(int));
           tblSave.Columns.Add("Size", typeof(string));
           tblSave.Columns.Add("SizeID", typeof(string));
           tblSave.Columns.Add("ThongSo", typeof(string));
           tblSave.Columns.Add("Sort", typeof(string));
           tblSave.Columns.Add("StyleID", typeof(string));
           tblSave.Columns.Add("TenNhomCongDoan", typeof(string));
           tblSave.Columns.Add("DonViTS", typeof(string));
           tblSave.Columns.Add("MaCongDoan_ThongSo", typeof(string));
            return tblSave;

        }
        private void LoadCongDoanTS()
        {
            try
            {
              DataTable  tblTVThongSoCheckList = CreateTable_CheckListCD();
                string url = string.Format("{0}?action={1}&para1={2}", URL + "CheckListCD/Get", "GetTVThongSoCheckList", _mahang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblTVThongSoCheckList = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcThongSoCheckList.DataSource = tblTVThongSoCheckList;
                grvThongSoCheckList.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }
        }
        private void grvThongSoCheckList_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                view.SetRowCellValue(e.RowHandle, colID, 0);
                view.SetRowCellValue(e.RowHandle, colMaHang, _mahang);
               view.SetRowCellValue(e.RowHandle, colTenHang, _tenhang);
                //view.RefreshData();
            }
            catch(Exception ex)
            {

            }
            
        }
        private void grvThongSoCheckList_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            
                info.GroupText = string.Format("{0}", info.GroupValueText);

            if (grvThongSoCheckList.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }


        private void grvThongSoCheckList_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void grvThongSoCheckList_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            int rowHandle = view.FocusedRowHandle;
            //if (rowHandle < 0) return;
            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused == colCongDoan_ThongSo)
            {
                string newValue = e.Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
                {
                    e.Valid = false;
                    e.ErrorText = $"{col_focused.Caption} không được bỏ trống!";
                    return;
                }


                if (view.DataRowCount > 0 &&
                    Enumerable.Range(0, view.DataRowCount)
                        .Where(i => i != rowHandle)
                        .Any(i =>
                        {
                            string existingValue = view.GetRowCellValue(i, col_focused)?.ToString()?.Trim();
                            return !string.IsNullOrEmpty(existingValue) &&
                                   string.Equals(existingValue, newValue, StringComparison.OrdinalIgnoreCase);
                        }))
                {
                    e.Valid = false;
                    e.ErrorText = $"{col_focused.Caption} {newValue} đã tồn tại! Vui lòng nhập công đoạn thông số khác.";
                    return;
                }


                e.Valid = true;
                e.ErrorText = string.Empty;

            } 
        }

        private void grvThongSoCheckList_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataTable _dtSave = CreateTable_CheckListCD();
                DataSet dsSave = new DataSet();
                DataTable tblThongSo = grcThongSoCheckList.DataSource as DataTable;
                if (tblThongSo?.Rows?.Count == 0)
                {
                    return;
                }

                foreach (DataRow row in tblThongSo.Rows)
                {
                    DataRow rowSave = _dtSave.NewRow();
                    rowSave["ID"] = row["ID"];
                    rowSave["StyleID"] = row["StyleID"]?.ToString();
                    rowSave["CongDoan"] = row["CongDoan"]?.ToString();
                    rowSave["MaCongDoan_ThongSo"] = row["MaCongDoan_ThongSo"]?.ToString();
                    rowSave["ParentNO"] = _ParentNO;
                    _dtSave.Rows.Add(rowSave);

                }
                dsSave.Tables.Add(_dtSave);

                string url = string.Format("{0}", URL + "CheckListCD/POST?action=PostTVThongSoCheckList");
                string jsonData = JsonConvert.SerializeObject(dsSave);
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {

            }
            


        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadCongDoanTS();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataTable tblThongSo = grcThongSoCheckList.DataSource as DataTable;
                if (tblThongSo == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvThongSoCheckList.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvThongSoCheckList.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    //Nếu cần kiểm tra dữ liệu dùng lại, bạn bật đoạn này lên
                    if (dr["IsUse"]?.ToString()?.ToLower() == "true")
                    {
                        XtraMessageBox.Show($"Công đoạn thông số {dr["CongDoan"]?.ToString()} này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }


                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}CheckListCD/Delete?action=DeleteTVThongSoCheckList&Para1={dr["MaCDTS"]}&Para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                    // Xóa dòng trong DataTable
                    tblThongSo.Rows.Remove(dr);
                }

                grcThongSoCheckList.DataSource = tblThongSo;
                grcThongSoCheckList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grvThongSoCheckList_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.PrevFocusedRowHandle == DevExpress.XtraGrid.GridControl.NewItemRowHandle)
            {
                view.PostEditor();
                view.UpdateCurrentRow();
            }
        }
    }
}