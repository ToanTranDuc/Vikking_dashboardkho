using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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
    public partial class frmChonVatTuThanhPhan : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private DataTable tblVatTu;
        DataTable _tbl = new DataTable();
        DataRow rowVatTuThanhPhan;
        public DataTable tblVatTuSelected { get; set; }
        
        public frmChonVatTuThanhPhan(DataTable tbl, DataRow rowVatTuThanhPhan)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl = tbl;
            this.rowVatTuThanhPhan = rowVatTuThanhPhan;
            LoadVaTu();
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
                grvChooseVatTu.ActiveFilter.Clear();
                return;
            }

            grvChooseVatTu.ActiveFilterString = $@"
            [TenCL] LIKE '%{keyword}%'
            OR [MoTa] LIKE '%{keyword}%'
            OR [ItemCode] LIKE '%{keyword}%'
            OR [CodeMau] LIKE '%{keyword}%'
            OR [MauVT] LIKE '%{keyword}%'
          
        ";
        }
        private void LoadVaTu()
        {
            string url = $"{URL}ERPVatTuThanhPhan/GET?action=GetThongSoVT&para1={rowVatTuThanhPhan["MaCLVTID"]}&para2={rowVatTuThanhPhan["MaVTID"]}&para3={rowVatTuThanhPhan["MauVTID"]}&para4={rowVatTuThanhPhan["KhoVaiID"]}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblVatTu = new DataTable();
            if (json != "[]")
            {
                tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            }
           
            SetVatTuBG();
        }
        private void SetVatTuBG()
        {
            try
            {
                DataTable tblCopyVatTu = tblVatTu?.Copy();
                if (_tbl != null && _tbl.Rows.Count != 0)
                {
                    var keysToRemove = new HashSet<string>(
                 _tbl.AsEnumerable()
                     .Select(r => $"{r.Field<string>("MaCLVTID")}_{r.Field<string>("MaVTID")}_{r.Field<string>("MauVTID")}_{r.Field<string>("KhoVaiID")}_{r.Field<string>("MaDVVT")}"));

                    var rowsToDelete = tblCopyVatTu.AsEnumerable()
                        .Where(r => keysToRemove.Contains($"{r.Field<string>("MaCLVTID")}_{r.Field<string>("MaVTID")}_{r.Field<string>("MauVTID")}_{r.Field<string>("KhoVaiID")}_{r.Field<string>("MaDVVT")}"))
                        .ToList();
                    foreach (var row in rowsToDelete)
                    {
                        tblCopyVatTu.Rows.Remove(row);
                       
                    }
                    
                }


                grcChooseVatTu.DataSource = tblCopyVatTu;


            }
            catch (Exception ex)
            {

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

        private void grvChooseVatTu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.ActiveControl = simpleButton1;
            DataTable tblSelected = grcChooseVatTu.DataSource as DataTable;
            if (tblSelected != null && tblSelected?.Rows?.Count > 0)
            {

                var lstRowSelect = tblSelected.AsEnumerable()
              .Where(r => !r.IsNull("IsCheck") && r.Field<bool>("IsCheck"));

                tblVatTuSelected = lstRowSelect.Any()
                    ? lstRowSelect.CopyToDataTable()
                    : tblSelected.Clone();

                this.DialogResult = DialogResult.OK;


            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadVaTu();
        }

        private void grvChooseVatTu_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvChooseVatTu_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                if (view.FocusedColumn.FieldName == "IsCheck")
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
    }
}