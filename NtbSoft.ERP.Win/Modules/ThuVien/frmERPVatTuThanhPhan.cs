using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERPVatTuThanhPhan : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        DataRow _rowFocused;

        private List<string> lstColumnEdit = new List<string>() { "DinhMuc", "GhiChu" };
        public frmERPVatTuThanhPhan(DataRow rowThongSoFocused)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _rowFocused = rowThongSoFocused;
            if(_rowFocused != null)
            {
                this.Text = $"ItemCode: {rowThongSoFocused["MaVT"]} - {rowThongSoFocused["ChiTiet"]}";
            }
            LoadVatTuThanhPhan();
        }
       
        private DataTable CreateTableVatTuTP_Detail()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaCLVTID", typeof(object));
            dt.Columns.Add("MaVTID", typeof(object));
            dt.Columns.Add("MauVTID", typeof(object));
            dt.Columns.Add("KhoVaiID", typeof(object));
            dt.Columns.Add("MaDVVT", typeof(object));
            dt.Columns.Add("DinhMuc", typeof(object));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("TenCL", typeof(string));
            dt.Columns.Add("MoTa", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("MauVT", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("TenDVVT", typeof(string));
            dt.Columns.Add("LoaiNPL", typeof(string));
            dt.Columns.Add("IsNPL", typeof(object));
            
            return dt;
        }

        private DataTable CreateTableVatTuTP_Save()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("MaCLVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("DinhMuc", typeof(double));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("IsParent", typeof(int));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            dt.Columns.Add("Creater", typeof(string));

            return dt;
        }
        private void LoadVatTuThanhPhan()
        {
            try
            {
                DataTable tblVatTuThanhPham = new DataTable();
                string url = $"{URL}ERPVatTuThanhPhan/GET?Action=GetVatTuTP&para1={_rowFocused["MaNhom"]}&para2={_rowFocused["MaVTID"]}&para3={_rowFocused["MauVTID"]}&para4={_rowFocused["KhoVaiID"]}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblVatTuThanhPham = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcVattuTP.DataSource = tblVatTuThanhPham;
            }
            catch(Exception ex) { 
            }
            
        }
        private void LoadVatTuThanhPhanDetail(DataRow rowVatTuThanhPhan)
        {
            DataTable tblVatTuThanhPhamDetail = new DataTable();
            string url = $"{URL}ERPVatTuThanhPhan/GET?Action=GetVatTuTPChiTiet&para1={rowVatTuThanhPhan["MaCLVTID"]}&para2={rowVatTuThanhPhan["MaVTID"]}&para3={rowVatTuThanhPhan["MauVTID"]}&para4={rowVatTuThanhPhan["KhoVaiID"]}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblVatTuThanhPhamDetail = JsonConvert.DeserializeObject<DataTable>(json);
            }
            grcVattuTP_Detail.DataSource = tblVatTuThanhPhamDetail;
            grcVattuTP_Detail.RefreshDataSource();
            grvVattuTP_Detail.FocusedRowHandle = 0;
        }
        private void grvVattuTP_DataSourceChanged(object sender, EventArgs e)
        {
            DataRow rowVatTuTP = grvVattuTP.GetDataRow(0);
            if(rowVatTuTP != null)
            {
                LoadVatTuThanhPhanDetail(rowVatTuTP);
            }
          
        }

        #region Styte grid
        private void griview_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
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
        private void griview_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;

                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;

                int groupLevel = view.GetRowLevel(e.RowHandle);

                GridColumn groupColumn = info.Column;


                info.GroupText = string.Format("{0}", info.GroupValueText);

                if (view.IsGroupRow(e.RowHandle))
                {

                    Color textColor = Color.Black;

                    switch (groupLevel)

                    {

                        case 0: textColor = Color.MediumBlue; break;

                        case 1: textColor = Color.Maroon; break;

                    }

                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();

                    e.Handled = true;

                }

            }
            catch (Exception ex)
            {

            }
        }
        private void GrvRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            bool isEditable = lstColumnEdit.Contains(e.Column.FieldName);

            if (isEditable)
            {
                e.Appearance.BackColor = Color.FromArgb(192, 255, 255);
                if(e.Column.FieldName == "DinhMuc")
                {
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                }
               

            }

        }
        private void grv_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null || e.Value == DBNull.Value || e.Value.ToString() == "")
                {
                    if (e.Column.FieldName == "DinhMuc") e.DisplayText = "-";
                    return;
                }

                var view = sender as GridView;

                if (e.Column.FieldName == "DinhMuc")
                {

                    if (e.Value == null || e.Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(e.Value?.ToString()) ||
                        !decimal.TryParse(e.Value.ToString(), out decimal value) ||
                        value == 0m)
                    {
                        e.DisplayText = "-";
                        return;
                    }


                    var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    nfi.NumberGroupSeparator = ",";

                    int decimals = (decimal.GetBits(value)[3] >> 16) & 0x000000FF;
                    decimals = decimals == 0 ? 0 : Math.Min(decimals, 4);

                    string format = decimals == 0 ? "N0" : $"N{decimals}";

                    e.DisplayText = value.ToString(format, nfi);
                    return;
                }
                



            }
            catch (Exception ex)
            {

            }
        }


        #endregion

        private void grvVattuTP_Detail_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvVattuTP_Detail_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvVattuTP_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvVattuTP_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }


        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                 if (lstColumnEdit.Contains(view.FocusedColumn.FieldName))
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;


                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {

            }


        }
        private void gridView_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            string fieldName = view.FocusedColumn?.FieldName;

            if (fieldName == "DinhMuc")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Định mức không được để trống!";
                    return;
                }

                if (!double.TryParse(e.Value.ToString(), out double dinhMuc))
                {
                    e.Valid = false;
                    e.ErrorText = "Định mức không hợp lệ!";
                    return;
                }

              
                if (dinhMuc <= 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Định mức phải lớn hơn 0!";
                    return;
                }
            }
        }

        private void gridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }
        private void btnAddVatTuTP_Click(object sender, EventArgs e)
        {

            DataTable tblVatTuTP_Detail = grcVattuTP_Detail.DataSource as DataTable;
            if(tblVatTuTP_Detail == null || tblVatTuTP_Detail?.Rows?.Count == 0)
            {
                tblVatTuTP_Detail = CreateTableVatTuTP_Detail();
            }
            DataRow rowVatTuTP = grvVattuTP.GetDataRow(0);
            if (rowVatTuTP != null)
            {
                frmChonVatTuThanhPhan frm = new frmChonVatTuThanhPhan(tblVatTuTP_Detail, rowVatTuTP);
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    if (frm.tblVatTuSelected != null && frm.tblVatTuSelected?.Rows?.Count > 0)
                    {
                        if (tblVatTuTP_Detail == null || tblVatTuTP_Detail.Rows.Count == 0)
                        {
                            tblVatTuTP_Detail = CreateTableVatTuTP_Detail();
                        }

                        foreach (DataRow rowSelected in frm.tblVatTuSelected.Rows)
                        {
                            // Kiểm tra trùng ID nếu cần
                            bool isDuplicate = tblVatTuTP_Detail.AsEnumerable()
                                .Any(r => r["MaVTID"]?.ToString() == rowSelected["MaVTID"]?.ToString()
                                       && r["MaCLVTID"]?.ToString() == rowSelected["MaCLVTID"]?.ToString()
                                       && r["MaVTID"]?.ToString() == rowSelected["MaVTID"]?.ToString()
                                       && r["KhoVaiID"]?.ToString() == rowSelected["KhoVaiID"]?.ToString()

                                       );

                            if (isDuplicate) continue; // bỏ qua nếu đã tồn tại

                            DataRow newRow = tblVatTuTP_Detail.NewRow();

                            newRow["ID"] = rowSelected["ID"] != DBNull.Value ? rowSelected["ID"] : 0;
                            newRow["MaCLVTID"] = rowSelected["MaCLVTID"];
                            newRow["MaVTID"] = rowSelected["MaVTID"];
                            newRow["MauVTID"] = rowSelected["MauVTID"];
                            newRow["KhoVaiID"] = rowSelected["KhoVaiID"];
                            newRow["MaDVVT"] = rowSelected["MaDVVT"] != DBNull.Value ? rowSelected["MaDVVT"] : (object)DBNull.Value;
                            newRow["DinhMuc"] = 0;
                            newRow["GhiChu"] = "";
                            newRow["TenCL"] = rowSelected["TenCL"] != DBNull.Value ? rowSelected["TenCL"] : (object)DBNull.Value;
                            newRow["MoTa"] = rowSelected["MoTa"] != DBNull.Value ? rowSelected["MoTa"] : (object)DBNull.Value;
                            newRow["ItemCode"] = rowSelected["ItemCode"] != DBNull.Value ? rowSelected["ItemCode"] : (object)DBNull.Value;
                            newRow["KhoVai"] = rowSelected["KhoVai"] != DBNull.Value ? rowSelected["KhoVai"] : (object)DBNull.Value;
                            newRow["MauVT"] = rowSelected["MauVT"] != DBNull.Value ? rowSelected["MauVT"] : (object)DBNull.Value;
                            newRow["CodeMau"] = rowSelected["CodeMau"] != DBNull.Value ? rowSelected["CodeMau"] : (object)DBNull.Value;
                            newRow["TenDVVT"] = rowSelected["TenDVVT"] != DBNull.Value ? rowSelected["TenDVVT"] : (object)DBNull.Value;
                            newRow["LoaiNPL"] = rowSelected["LoaiNPL"] != DBNull.Value ? rowSelected["LoaiNPL"] : (object)DBNull.Value;
                            newRow["IsNPL"] = rowSelected["IsNPL"] != DBNull.Value ? rowSelected["IsNPL"] : (object)DBNull.Value;
                           

                            tblVatTuTP_Detail.Rows.Add(newRow);
                        }

                       
                        grcVattuTP_Detail.DataSource = tblVatTuTP_Detail;
                        grcVattuTP_Detail.RefreshDataSource();


                    }
                }
            }
            
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadVatTuThanhPhan();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DataTable tblSaveVatTuThanhPhan = CreateTableVatTuTP_Save();
                DataTable tblVatTuThanhPhan = grcVattuTP.DataSource as DataTable;         // table header
                DataTable tblVatTuThanhPhan_Detail = grcVattuTP_Detail.DataSource as DataTable;  // table detail

           
                if (tblVatTuThanhPhan_Detail != null && tblVatTuThanhPhan_Detail.Rows.Count > 0)
                {
                    foreach (DataRow row in tblVatTuThanhPhan_Detail.Rows)
                    {
                        DataRow newRow = tblSaveVatTuThanhPhan.NewRow();

                        newRow["ID"] = row["ID"] != DBNull.Value ? Convert.ToInt64(row["ID"]) : (object)DBNull.Value;
                        newRow["MauVTID"] = row["MauVTID"] != DBNull.Value ? row["MauVTID"].ToString() : (object)DBNull.Value;
                        newRow["MaCLVTID"] = row["MaCLVTID"] != DBNull.Value ? row["MaCLVTID"].ToString() : (object)DBNull.Value;
                        newRow["KhoVaiID"] = row["KhoVaiID"] != DBNull.Value ? row["KhoVaiID"].ToString() : (object)DBNull.Value;
                        newRow["MaVTID"] = row["MaVTID"] != DBNull.Value ? row["MaVTID"].ToString() : (object)DBNull.Value;
                        newRow["MaDVVT"] = row["MaDVVT"] != DBNull.Value ? row["MaDVVT"].ToString() : (object)DBNull.Value;
                        newRow["DinhMuc"] = row["DinhMuc"] != DBNull.Value ? Convert.ToDouble(row["DinhMuc"]) : (object)DBNull.Value;
                        newRow["GhiChu"] = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : (object)DBNull.Value;
                        newRow["IsParent"] = 0;
                        newRow["CreateDate"] = DateTime.Now;
                        newRow["Creater"] = GlobleData.UserName;

                        tblSaveVatTuThanhPhan.Rows.Add(newRow);
                    }
                }

            
                if (tblVatTuThanhPhan != null && tblVatTuThanhPhan.Rows.Count > 0)
                {
                    foreach (DataRow row in tblVatTuThanhPhan.Rows)
                    {
                        
                        bool isDuplicate = tblSaveVatTuThanhPhan.AsEnumerable()
                            .Any(r => r["MaVTID"]?.ToString() == row["MaVTID"]?.ToString()
                                   && r["MauVTID"]?.ToString() == row["MauVTID"]?.ToString());

                        if (isDuplicate) continue;

                        DataRow newRow = tblSaveVatTuThanhPhan.NewRow();

                        newRow["ID"] = row["ID"] != DBNull.Value ? Convert.ToInt64(row["ID"]) : (object)DBNull.Value;
                        newRow["MauVTID"] = row["MauVTID"] != DBNull.Value ? row["MauVTID"].ToString() : (object)DBNull.Value;
                        newRow["MaCLVTID"] = row["MaCLVTID"] != DBNull.Value ? row["MaCLVTID"].ToString() : (object)DBNull.Value;
                        newRow["KhoVaiID"] = row["KhoVaiID"] != DBNull.Value ? row["KhoVaiID"].ToString() : (object)DBNull.Value;
                        newRow["MaVTID"] = row["MaVTID"] != DBNull.Value ? row["MaVTID"].ToString() : (object)DBNull.Value;
                        newRow["MaDVVT"] = row["MaDVVT"] != DBNull.Value ? row["MaDVVT"].ToString() : (object)DBNull.Value;
                        newRow["DinhMuc"] = row["DinhMuc"] != DBNull.Value ? Convert.ToDouble(row["DinhMuc"]) : (object)DBNull.Value;
                        newRow["GhiChu"] = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : (object)DBNull.Value;
                        newRow["IsParent"] = 1;
                        newRow["CreateDate"] = DateTime.Now;
                        newRow["Creater"] = GlobleData.UserName;

                        tblSaveVatTuThanhPhan.Rows.Add(newRow);
                    }
                }
                string url = string.Format("{0}", URL + "ERPVatTuThanhPhan/Post?action=POST");
                string jsonSave = JsonConvert.SerializeObject(tblSaveVatTuThanhPhan);
                string msResult = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, jsonSave);
                }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadVatTuThanhPhan();
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

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {

                int idxFocused = grvVattuTP_Detail.FocusedRowHandle;
            
                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvVattuTP_Detail.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    if(rowHandle < 0)
                    {
                        rowHandle = grvVattuTP_Detail.GetChildRowHandle(selectedHandles[i],0);
                    }
                    DataRow dr = grvVattuTP_Detail.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    if (dr["ID"].ToString() != "0")
                    {
                       

                        string url = $"{URL}ERPVatTuThanhPhan/Delete?action=Delete&para1={dr["ID"]}&para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                        grvVattuTP_Detail.DeleteRow(rowHandle);
                    }

                   



                }
          

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void grv_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            var column = view.FocusedColumn;
            var cellValue = view.GetRowCellValue(view.FocusedRowHandle, column);
            if (cellValue != null && cellValue.ToString() == "0")
            {
                view.SetRowCellValue(view.FocusedRowHandle, column, string.Empty);
            }
        }

        private void searchControl1_TextChanged(object sender, EventArgs e)
        {
            string keyword = searchControl1.Text.Trim();
            ApplyVatTu(keyword);
        }
        private void ApplyVatTu(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                grvVattuTP_Detail.ActiveFilter.Clear();
                return;
            }

            grvVattuTP_Detail.ActiveFilterString = $@"
            [TenCL] LIKE '%{keyword}%'
            OR [MoTa] LIKE '%{keyword}%'
            OR [ItemCode] LIKE '%{keyword}%'
            OR [CodeMau] LIKE '%{keyword}%'
            OR [MauVT] LIKE '%{keyword}%'
          
        ";
        }

        private void grvVattuTP_Detail_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
         
            if (e.HitInfo.InRowCell && e.HitInfo.Column != null)
            {
                e.Menu.Items.Clear(); // bỏ menu mặc định nếu muốn

                // Fill tất cả
                var fillAllItem = new DXMenuItem("Áp dụng tất cả", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                   
                    object value = view.GetRowCellValue(rowHandle, "DinhMuc");



                    view.BeginUpdate();
                    try
                    {
                        for (int i = 0; i < view.DataRowCount; i++)
                        {
                            int rh = view.GetRowHandle(i);
                         
                            view.SetRowCellValue(rh, "DinhMuc", value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                // Fill group
                var fillGroupItem = new DXMenuItem("Áp dụng theo nhóm", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                  
                    object value = view.GetRowCellValue(rowHandle, "DinhMuc");

                    // Xác định group row của row hiện tại
                    int groupRowHandle = view.GetParentRowHandle(rowHandle);

                    view.BeginUpdate();
                    try
                    {
                        // Duyệt tất cả row trong group đó
                        for (int i = 0; i < view.GetChildRowCount(groupRowHandle); i++)
                        {
                            int childHandle = view.GetChildRowHandle(groupRowHandle, i);
                           
                            if (view.IsDataRow(childHandle))
                                view.SetRowCellValue(childHandle, "DinhMuc", value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                e.Menu.Items.Add(fillAllItem);
                e.Menu.Items.Add(fillGroupItem);
            }
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }
        bool indicatorIcon = true;
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}