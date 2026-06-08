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
    public partial class frmAddCongDoanChecklist : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;

        string _mahang = string.Empty; string _tenhang = string.Empty;
        int _version = 1;
        public frmAddCongDoanChecklist(string MaHang,string TenHang,int version)
        {


            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _mahang = MaHang;
            _tenhang = TenHang;
            _version = version;
            SetupGroupLevelColors();

            grvNhomCDCheckList.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            grvNhomCDCheckList.OptionsBehavior.Editable = true; // Cho phép edit
            grvNhomCDCheckList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            grvNhomCDCheckList.OptionsSelection.EnableAppearanceFocusedCell = false;

            this.grvNhomCDCheckList.NewItemRowText = "Thêm mới công đoạn";
            this.grvNhomCDCheckList.Appearance.TopNewRow.ForeColor = Color.Blue;
            this.grvNhomCDCheckList.Appearance.TopNewRow.Font = new Font("Tahoma", 9, FontStyle.Italic);

            grvNhomCDCheckList.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True; // Giữ group rows fixed ở top khi scroll
            LoadNhomCongDoan();
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
            DataTable tblSave = new DataTable("tblCheckListCD");
            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("ParentID", typeof(int));
            tblSave.Columns.Add("ParentNO", typeof(int));
            tblSave.Columns.Add("TenNhomCongDoan", typeof(string));
            tblSave.Columns.Add("StyleID", typeof(string));
            tblSave.Columns.Add("MaHang", typeof(string));
            tblSave.Columns.Add("NO", typeof(string));
            tblSave.Columns.Add("Description", typeof(string));
            tblSave.Columns.Add("Note", typeof(string));
            tblSave.Columns.Add("Tol", typeof(string));
            tblSave.Columns.Add("Sort", typeof(int));
            tblSave.Columns.Add("CreateDate", typeof(DateTime));
            tblSave.Columns.Add("NVien", typeof(string));
            tblSave.Columns.Add("Version", typeof(int));
            tblSave.Columns.Add("Tol_Negative", typeof(string));
            tblSave.Columns.Add("Donvi", typeof(string));
            tblSave.Columns.Add("Note_TS", typeof(string));
            tblSave.Columns.Add("MucDo", typeof(string));

            return tblSave;



        }

        private void LoadNhomCongDoan()
        {
            try
            {
                DataTable tblTVThongSoCheckList = CreateTable_CheckListCD();
                string url = string.Format("{0}?action={1}&para1={2}&para2={3}", URL + "CheckListCD/Get", "GetNhomCDCheckList", _mahang, _version);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblTVThongSoCheckList = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcNhomCDCheckList.DataSource = tblTVThongSoCheckList;
                grvNhomCDCheckList.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }
        }
        private void grvNhomCDCheckList_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                view.SetRowCellValue(e.RowHandle, colID, 0);
                view.SetRowCellValue(e.RowHandle, colMaHang, _mahang);
                view.SetRowCellValue(e.RowHandle, colTenHang, _tenhang);

                //view.RefreshData();
            }
            catch (Exception ex)
            {

            }

        }
        private void grvNhomCDCheckList_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.PrevFocusedRowHandle == DevExpress.XtraGrid.GridControl.NewItemRowHandle)
            {
                view.PostEditor();
                view.UpdateCurrentRow();
            }
        }
        private void grvNhomCDCheckList_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();

            info.GroupText = string.Format("{0}", info.GroupValueText);

            if (grvNhomCDCheckList.IsGroupRow(e.RowHandle))
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

        private void grvNhomCDCheckList_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void grvNhomCDCheckList_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            int rowHandle = view.FocusedRowHandle;
            //if (rowHandle < 0) return;
            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused == colTenNhomCongDoan)
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

        private void grvNhomCDCheckList_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
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
                DataTable tblThongSo = grcNhomCDCheckList.DataSource as DataTable;
                if (tblThongSo?.Rows?.Count == 0)
                {
                    return;
                }

                foreach (DataRow row in tblThongSo.Rows)
                {
                    DataRow rowSave = _dtSave.NewRow();
                    rowSave["ID"] = 0;
                    rowSave["ParentNO"] = row["ParentNO"];
                    rowSave["StyleID"] = row["StyleID"]?.ToString();
                    rowSave["TenNhomCongDoan"] = row["TenNhomCongDoan"]?.ToString();
                    rowSave["CreateDate"] = DateTime.Now;
                    rowSave["NVien"] = GlobleData.UserName;
                    rowSave["Version"] = _version;
                    _dtSave.Rows.Add(rowSave);

                }
                dsSave.Tables.Add(_dtSave);

                string url = string.Format("{0}", URL + "CheckListCD/POST?action=PostNhomCD");
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
            catch (Exception ex)
            {

            }



        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadNhomCongDoan();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataTable tblThongSo = grcNhomCDCheckList.DataSource as DataTable;
                if (tblThongSo == null) return;

                DataRow dr = grvNhomCDCheckList.GetFocusedDataRow();
                if (dr == null) return;

                string msg = "Bạn có muốn xóa dòng này không?";

              
                string urlGET = $"{URL}CheckListCD/GET?action=CheckUsing&para1={_mahang}&para2={_version}";
                string jsonCheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
                if (jsonCheck != "[]")
                {
                    DataTable tblCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                    if (tblCheck?.Rows?.Count > 0 &&
                        !string.IsNullOrEmpty(tblCheck.Rows[0]["Msg"]?.ToString()) &&
                        tblCheck.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                    {
                        msg = $"Bạn có muốn tiếp tục xóa dữ liệu {dr["TenNhomCongDoan"]?.ToString()} khi đã phát sinh dữ liệu QC kiểm hay không?";
                    }
                }

                
                DialogResult messResult = MessageBox.Show(msg, "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult != DialogResult.Yes) return;

              
                // Gọi API xóa nếu có ParentNO
                if (dr["ParentNO"].ToString() != "0")
                {
                    string url = $"{URL}CheckListCD/Delete?action=DeleteNhomCD&Para1={dr["ParentNO"]}&Para2={GlobleData.UserName}";
                    string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                    }
                }
                tblThongSo.Rows.Remove(dr);
                grcNhomCDCheckList.DataSource = tblThongSo;
                grcNhomCDCheckList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



    }
}