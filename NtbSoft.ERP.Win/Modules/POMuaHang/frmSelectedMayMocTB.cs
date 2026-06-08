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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmSelectedMayMocTB : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _loaicc, _maNCC, _chungloaiCC = string.Empty;
        string _maPhieuBG = string.Empty; string _ngayBDHieuLuc = string.Empty;

        private DataTable tblMayMocTBBaoGia;
        private DataTable tblLinhKien;

        public DataTable tblVatTuSelected { get; set; }

        DataTable _tbl = new DataTable();
        public frmSelectedMayMocTB(string LoaiCC, string MaNhaCC, string NCC, string MaPhieuBG, DataTable tbl, string ChungLoaiCC = "", string NgayBDHieuLuc = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl = tbl;
            this._tbl = tbl;
            _loaicc = LoaiCC;
            _maNCC = MaNhaCC;
            _chungloaiCC = ChungLoaiCC;
            txtNhaCC.EditValue = NCC;
            _maPhieuBG = MaPhieuBG;
            _ngayBDHieuLuc = NgayBDHieuLuc;
           
        }

        private DataTable InitSelectedTable()
        {
            tblVatTuSelected = new DataTable();
            tblVatTuSelected.Columns.Add("MaCL", typeof(string));
            tblVatTuSelected.Columns.Add("MaTB", typeof(string));
            tblVatTuSelected.Columns.Add("TenTB", typeof(string));
            tblVatTuSelected.Columns.Add("MauMa", typeof(string));
            tblVatTuSelected.Columns.Add("XuatXu", typeof(string));
            tblVatTuSelected.Columns.Add("HangSX", typeof(string));
            tblVatTuSelected.Columns.Add("TenCL", typeof(string));
            tblVatTuSelected.Columns.Add("TenCT", typeof(string));
            tblVatTuSelected.Columns.Add("MaNhom", typeof(string));
            tblVatTuSelected.Columns.Add("TenNhom", typeof(string));
            tblVatTuSelected.Columns.Add("MaLK", typeof(string));
            tblVatTuSelected.Columns.Add("TenLK", typeof(string));
            tblVatTuSelected.Columns.Add("MaCLLK", typeof(string));
            tblVatTuSelected.Columns.Add("CLLK", typeof(string));
            tblVatTuSelected.Columns.Add("DonVi", typeof(string));          
            tblVatTuSelected.Columns.Add("Status", typeof(bool));

            return tblVatTuSelected;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadLinhLKien();
            LoadMayMocTB();
        }

        private void LoadLinhLKien()
        {
            string url = $"{URL}PhieuBaoGiaMMTB/GET?action=GetMMTB&para1={_maNCC}&para2={_maPhieuBG}&para3={_ngayBDHieuLuc}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblMayMocTBBaoGia = new DataTable();
            if (json != "[]")
            {
                tblMayMocTBBaoGia = JsonConvert.DeserializeObject<DataTable>(json);
            }
            SetVatTuBG(tblMayMocTBBaoGia, true);


        }


        private void LoadMayMocTB()
        {
            string url = $"{URL}PhieuBaoGiaMMTB/GET?action=GetLinhKien&para1={_maNCC}&para2{_maPhieuBG}&para3={_ngayBDHieuLuc}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblLinhKien = new DataTable();
            if (json != "[]")
            {
                tblLinhKien = JsonConvert.DeserializeObject<DataTable>(json);
            }
            SetVatTuBG(tblLinhKien, false);
        }


        private void SetVatTuBG(DataTable tblVatTuBaoGia, bool Status)
        {
            try
            {
                DataTable tblCopyVatTu = tblVatTuBaoGia?.Copy();
                if (_tbl != null && _tbl.Rows.Count != 0)
                {
                    if (Status)
                    {
                        var keysToRemove = new HashSet<string>(
                      _tbl.AsEnumerable()
                     .Select(r => $"{r.Field<string>("ChungLoaiCC")}_{r.Field<string>("TenTB")}_{r.Field<string>("MauMa")}_{r.Field<string>("XuatXu")}_{r.Field<string>("HangSX")}_{r.Field<string>("MaNhom")}"));

                        var rowsToDelete = tblCopyVatTu.AsEnumerable()
                            .Where(r => keysToRemove.Contains($"{r.Field<string>("MaCL")}_{r.Field<string>("TenTB")}_{r.Field<string>("MauMa")}_{r.Field<string>("XuatXu")}_{r.Field<string>("HangSX")}_{r.Field<string>("MaNhom")}"))
                            .ToList();
                        foreach (var row in rowsToDelete)
                        {
                            tblCopyVatTu.Rows.Remove(row);

                        }

                        grcMMTB.DataSource = tblMayMocTBBaoGia;
                        grvMMTB.ExpandAllGroups();
                    }
                    else
                    {
                        var keysToRemove = new HashSet<string>(
                         _tbl.AsEnumerable()
                        .Select(r => $"{r.Field<string>("ChungLoaiCC")}_{r.Field<string>("TenLK")}_{r.Field<string>("MaCLLK")}_{r.Field<string>("MaNhom")}"));

                        var rowsToDelete = tblCopyVatTu.AsEnumerable()
                            .Where(r => keysToRemove.Contains($"{r.Field<string>("MaCL")}_{r.Field<string>("TenLK")}_{r.Field<string>("MaCLLK")}_{r.Field<string>("MaNhom")}"))
                            .ToList();
                        foreach (var row in rowsToDelete)
                        {
                            tblCopyVatTu.Rows.Remove(row);

                        }

                        grcLinhKien.DataSource = tblLinhKien;
                        grvLinhKien.ExpandAllGroups();
                    }



                }
                else
                {
                    if (Status)
                    {
                       
                        

                        grcMMTB.DataSource = tblVatTuBaoGia;
                        grvMMTB.ExpandAllGroups();
                    }
                    else
                    {
                       

                        grcLinhKien.DataSource = tblVatTuBaoGia;
                        grvLinhKien.ExpandAllGroups();
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = Button1;
            InitSelectedTable();
            DataTable tblMMTB = grcMMTB.DataSource as DataTable;
            DataTable tblLinhLien = grcLinhKien.DataSource as DataTable;

            if(tblMMTB != null &&  tblMMTB?.Rows?.Count > 0)
            {
                foreach(DataRow row in tblMMTB?.Rows)
                {


                    if (row["IsChecked"]?.ToString()?.ToLower() == "true")
                    {
                        DataRow rowSelected = tblVatTuSelected?.NewRow();
                  
                        rowSelected["MaTB"] = row["MaTB"];
                        rowSelected["TenTB"] = row["TenTB"];
                        rowSelected["MauMa"] = row["MauMa"];
                        rowSelected["XuatXu"] = row["XuatXu"];
                        rowSelected["HangSX"] = row["HangSX"];
                        rowSelected["MaCL"] = row["MaCL"];
                        rowSelected["TenCT"] = row["TenCT"];
                        rowSelected["TenCL"] = row["TenCL"];
                        rowSelected["MaNhom"] = row["MaNhom"];
                        rowSelected["TenNhom"] = row["TenNhom"];
                        rowSelected["MaLK"] =null;
                        rowSelected["TenLK"] = null;
                        rowSelected["MaCLLK"] = null;
                        rowSelected["CLLK"] = null;
                        rowSelected["DonVi"] = null;
                        rowSelected["Status"] = true;
                        tblVatTuSelected.Rows.Add(rowSelected);
                    }

                    
                }
            }

            if (tblLinhLien != null && tblLinhLien?.Rows?.Count > 0)
            {
                foreach (DataRow row in tblLinhLien?.Rows)
                {
                    if (row["IsChecked"]?.ToString()?.ToLower() == "true")
                    {
                        DataRow rowSelected = tblVatTuSelected?.NewRow();

                        rowSelected["MaTB"] = null;
                        rowSelected["TenTB"] = null;
                        rowSelected["MauMa"] = null;
                        rowSelected["XuatXu"] = null;
                        rowSelected["HangSX"] = null;
                        rowSelected["MaCL"] = row["MaCL"];
                        rowSelected["TenCT"] = row["TenCT"];
                        rowSelected["TenCL"] = row["TenCL"];
                        rowSelected["MaNhom"] = row["MaNhom"];
                        rowSelected["TenNhom"] = row["TenNhom"];
                        rowSelected["MaLK"] = row["MaLK"];
                        rowSelected["TenLK"] = row["TenLK"];
                        rowSelected["MaCLLK"] = row["MaCLLK"];
                        rowSelected["CLLK"] = row["CLLK"];
                        rowSelected["DonVi"] = row["DonVi"];
                        rowSelected["Status"] = false;
                        tblVatTuSelected.Rows.Add(rowSelected);
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
        }

      
        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadLinhLKien();
            LoadMayMocTB();
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


                if (e.RowHandle == GridControl.InvalidRowHandle)
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

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }

        private void searchControl1_TextChanged(object sender, EventArgs e)
        {
            string keyword = searchControl1.Text.Trim();

            ApplyFilterMMTB(keyword);
            ApplyFilterLinhKien(keyword);
        }
        private void ApplyFilterMMTB(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                grvMMTB.ActiveFilter.Clear();
                return;
            }

            grvMMTB.ActiveFilterString = $@"
            [TenTB] LIKE '%{keyword}%'
            OR [MauMa] LIKE '%{keyword}%'
            OR [XuatXu] LIKE '%{keyword}%'
            OR [HangSX] LIKE '%{keyword}%'
            OR [TenCL] LIKE '%{keyword}%'
            OR [TenCT] LIKE '%{keyword}%'
            OR [TenNhom] LIKE '%{keyword}%'
        ";
        }

        private void ApplyFilterLinhKien(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                grvLinhKien.ActiveFilter.Clear();
                return;
            }

            grvLinhKien.ActiveFilterString = $@"
        [TenLK] LIKE '%{keyword}%'
        OR [CLLK] LIKE '%{keyword}%'
        OR [TenCL] LIKE '%{keyword}%'
        OR [TenCT] LIKE '%{keyword}%'
        OR [TenNhom] LIKE '%{keyword}%'
        OR [DonVi] LIKE '%{keyword}%'
    ";
        }


        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle < 0) return;
            // Kiểm tra đúng cột cần style
            if (e.Column.FieldName == "IsUseBG" || e.Column.FieldName == "TinhTrang")
            {
                string tinhTrang = view.GetRowCellValue(e.RowHandle, e.Column).ToString();

                if (tinhTrang == "Còn hiệu lực")
                {
                    e.Appearance.ForeColor = Color.Green;   // chữ xanh lá
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold); // in đậm
                }
                else if (tinhTrang == "Hết hiệu lực")
                {
                    e.Appearance.ForeColor = Color.Gray;   // chữ xám
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold); // in đậm
                }
            }
        }
        private void grvSelectVTBaoGia_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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

        private void grvSelectVTBaoGia_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvSelectVTBaoGia_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void grvSelectVTBaoGia_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return; // bỏ qua header, group, v.v.

            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (view == null) return;
            object isUseValue = view.GetRowCellValue(e.RowHandle, "IsUse");

            if (isUseValue != null && isUseValue != DBNull.Value && Convert.ToBoolean(isUseValue))
            {

                e.Appearance.BackColor = Color.FromArgb(230, 255, 240);
                e.HighPriority = true;
                return;
            }
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(255, 253, 230);


                e.HighPriority = true;
            }
        }

        private void searchControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view == null) return;
                DataRow rowFocused = view.GetFocusedDataRow();
                if (rowFocused == null) return;
                bool.TryParse(rowFocused["IsUseBG"]?.ToString(), out bool IsUseBG);

                if (view.FocusedColumn.FieldName == "IsChecked")
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = !IsUseBG;
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
        private void grvSelectVTBaoGia_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

      
    }
}