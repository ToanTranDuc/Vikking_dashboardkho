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
    public partial class frmThuVienThue : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public frmThuVienThue()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            LoadThue();
        }

        private DataTable CreateTable()
        {

            DataTable tblSave = new DataTable("tblThue");
            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("Thue", typeof(string));
            tblSave.Columns.Add("MaThue", typeof(string));
            tblSave.Columns.Add("NhomThue", typeof(string));   
            tblSave.Columns.Add("GhiChu", typeof(string));
            return tblSave;

        }
        private void LoadThue()
        {
            try
            {
                DataTable tblTVThue = CreateTable();
                string url = string.Format("{0}?action={1}", URL + "DicThuVienBaoGia/Get", "GetThue");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblTVThue = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcThue.DataSource = tblTVThue;
                grvThue.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }
        }
        private void grvThue_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                view.SetRowCellValue(e.RowHandle, colID, 0);
                view.SetRowCellValue(e.RowHandle, colMaThue, null);
                //view.RefreshData();
            }
            catch (Exception ex)
            {

            }

        }
        private void grvThue_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        private void grvThue_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            int rowHandle = view.FocusedRowHandle;
            //if (rowHandle < 0) return;
            GridColumn col_focused = view.FocusedColumn;
            if (col_focused == null) return;

            if (col_focused == colThue)
            {
                string newValue = e.Value?.ToString()?.Trim();
                //if (string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
                //{
                //    e.Valid = false;
                //    e.ErrorText = $"{col_focused.Caption} không được bỏ trống!";
                //    return;
                //}


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
                    e.ErrorText = $"{col_focused.Caption} {newValue} đã tồn tại! Vui lòng nhập loại thuế khác.";
                    return;
                }


                e.Valid = true;
                e.ErrorText = string.Empty;

            }
        }

        private void grvThue_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
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
                DataTable _dtSave = CreateTable();

                DataTable tblThue = grcThue.DataSource as DataTable;
                if (tblThue?.Rows?.Count == 0)
                {
                    return;
                }

                foreach (DataRow row in tblThue.Rows)
                {
                    string Thue = row["Thue"]?.ToString();
                    if (!string.IsNullOrEmpty(Thue))
                    {
                        DataRow rowSave = _dtSave.NewRow();
                        rowSave["ID"] = row["ID"];
                        rowSave["Thue"] = Thue;
                        rowSave["MaThue"] = row["MaThue"]?.ToString();
                        rowSave["NhomThue"] = row["NhomThue"]?.ToString();                      
                        rowSave["GhiChu"] = row["GhiChu"]?.ToString();
                        _dtSave.Rows.Add(rowSave);
                    }


                }

                string url = string.Format("{0}", URL + "DicThuVienBaoGia/PostThue");
                string jsonData = JsonConvert.SerializeObject(_dtSave);
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadThue();
                    //this.DialogResult = DialogResult.OK;
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
            LoadThue();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataTable tblThue = grcThue.DataSource as DataTable;
                if (tblThue == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvThue.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvThue.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    //Nếu cần kiểm tra dữ liệu dùng lại, bạn bật đoạn này lên


                    if (dr["ID"].ToString() != "0")
                    {
                        if (dr["IsUse"]?.ToString()?.ToLower() == "true")
                        {
                            XtraMessageBox.Show($"Thuế {dr["Thue"]?.ToString()} này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        string url = $"{URL}DicThuVienBaoGia/Delete?action=DeleteThue&Para1={dr["MaThue"]}&Para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }

                    // Xóa dòng trong DataTable
                    tblThue.Rows.Remove(dr);
                }

                grcThue.DataSource = tblThue;
                grcThue.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grvThue_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.PrevFocusedRowHandle == DevExpress.XtraGrid.GridControl.NewItemRowHandle)
            {
                view.PostEditor();
                view.UpdateCurrentRow();
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grvThue.RowCount == 0)
            {
                grcThue.DataSource = CreateTable();
            }
            grvThue.AddNewRow();

        }
    }
}