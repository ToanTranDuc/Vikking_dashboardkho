using DevExpress.Utils.Menu;
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
    public partial class frmSelectVTBaoGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _loaicc, _maNCC, _chungloaiCC = string.Empty;
        string _maPhieuBG = string.Empty;string _ngayBDHieuLuc  = string.Empty;

        private DataTable tblVatTuBaoGia;

        DataTable _tbl = new DataTable();

        public DataTable tblVatTuSelected { get; set; } = new DataTable();
        public int SoDotGH { get; set; } = -1;
        public bool IsAddDotGH { get; set; } = false;
        public frmSelectVTBaoGia(string LoaiCC, string MaNhaCC,string NCC,string MaPhieuBG,DataTable tbl,string ChungLoaiCC = "",string NgayBDHieuLuc = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl = tbl;
            _loaicc = LoaiCC;
            _maNCC = MaNhaCC;
            _chungloaiCC = ChungLoaiCC;
            txtNhaCC.EditValue = NCC;
            _maPhieuBG = MaPhieuBG;
            _ngayBDHieuLuc = NgayBDHieuLuc;
            spinSoDot.Properties.MaxValue = decimal.MaxValue;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadVaTu();
        }
        private void grvSelectVTBaoGia_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try {
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
            catch(Exception ex)
            {

            }
        }

        private void grvSelectVTBaoGia_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (e.Column.FieldName == "IsCheck")
            {
                view.UpdateCurrentRow();
            }
        }

        private void btnSubmit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = Button1;
            DataTable tblSelectVTBaoGia = grcSelectVTBaoGia.DataSource as DataTable;
            if(tblSelectVTBaoGia != null && tblSelectVTBaoGia?.Rows?.Count > 0)
            {
                      
                var lstRowSelect = tblSelectVTBaoGia.AsEnumerable()
              .Where(r => !r.IsNull("IsCheck") && r.Field<bool>("IsCheck"));

                tblVatTuSelected = lstRowSelect.Any()
                    ? lstRowSelect.CopyToDataTable()
                    : tblSelectVTBaoGia.Clone();

                this.DialogResult = DialogResult.OK;


            }



        }

        private void SetVatTuBG()
        {
            try
            {
                DataTable tblCopyVatTu = tblVatTuBaoGia?.Copy();
                if (_tbl != null && _tbl.Rows.Count != 0)
                {
                    var keysToRemove = new HashSet<string>(
                 _tbl.AsEnumerable()
                     .Select(r => $"{r.Field<string>("ChungLoaiCC")}_{r.Field<string>("MaVTID")}_{r.Field<string>("MauVTID")}_{r.Field<string>("KhoSizeID")}_{r.Field<string>("MaDVVT")}"));

                    var rowsToDelete = tblCopyVatTu.AsEnumerable()
                        .Where(r => keysToRemove.Contains($"{r.Field<string>("MaCLVT")}_{r.Field<string>("MaVTID")}_{r.Field<string>("MauVTID")}_{r.Field<string>("KhoVaiID")}_{r.Field<string>("MaDVVT")}"))
                        .ToList();
                    foreach (var row in rowsToDelete)
                    {
                        tblCopyVatTu.Rows.Remove(row);
                        //row["IsUse"] = true;
                    }
                    //tblCopyVatTu.Rows.Remove(row);
                }
               
             
                grcSelectVTBaoGia.DataSource = tblCopyVatTu;
               

            }
            catch(Exception ex)
            {

            }
            
        }

       
        private void LoadVaTu()
        {
            string url = $"{URL}PhieuBaoGia/GET?action=GetVatTu&para1={_loaicc}&para2={_maNCC}&para3={_maPhieuBG}&para4={_ngayBDHieuLuc}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblVatTuBaoGia = new DataTable();
            if (json != "[]")
            {
                tblVatTuBaoGia = JsonConvert.DeserializeObject<DataTable>(json);
            }
            //grcSelectVTBaoGia.DataSource = tblVatTuBaoGia;
            SetVatTuBG();
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

         
            object isUseValue = grvSelectVTBaoGia.GetRowCellValue(e.RowHandle, "IsUse");
            
            if (isUseValue != null && isUseValue != DBNull.Value && Convert.ToBoolean(isUseValue))
            {

                e.Appearance.BackColor = Color.FromArgb(230, 255, 240);
                e.HighPriority = true;
                return;
            }
            if (e.RowHandle == grvSelectVTBaoGia.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(255, 253, 230);


                e.HighPriority = true;
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadVaTu();
        }

        private void grvSelectVTBaoGia_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            //&& e.HitInfo.Column.FieldName == "IsCheck"
            //Chỉ hiện menu khi chuột phải trên cell của cột TyLeMuaThem
            if (e.HitInfo.InRowCell && e.HitInfo.Column != null)
            {
                e.Menu.Items.Clear(); // bỏ menu mặc định nếu muốn

                // Fill tất cả
                var fillAllItem = new DXMenuItem("Áp dụng tất cả", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                    var col = colIsCheck;
                    object value = view.GetRowCellValue(rowHandle, col);

                 
                   
                    view.BeginUpdate();
                    try
                    {
                        for (int i = 0; i < view.DataRowCount; i++)
                        {
                            int rh = view.GetRowHandle(i);
                            object IsUse = view.GetRowCellValue(rh, "IsUseBG") ?? "";
                            if (IsUse?.ToString()?.ToLower() == "true") continue;
                            view.SetRowCellValue(rh, col, value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                // Fill group
                var fillGroupItem = new DXMenuItem("Áp dụng theo nhóm", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                    var col = colIsCheck;
                    object value = view.GetRowCellValue(rowHandle, col);

                    // Xác định group row của row hiện tại
                    int groupRowHandle = view.GetParentRowHandle(rowHandle);

                    view.BeginUpdate();
                    try
                    {
                        // Duyệt tất cả row trong group đó
                        for (int i = 0; i < view.GetChildRowCount(groupRowHandle); i++)
                        {
                            int childHandle = view.GetChildRowHandle(groupRowHandle, i);
                            object IsUse = view.GetRowCellValue(childHandle, "IsUseBG") ?? "";
                            if (IsUse?.ToString()?.ToLower() == "true") continue;
                            if (view.IsDataRow(childHandle))
                                view.SetRowCellValue(childHandle, col, value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                e.Menu.Items.Add(fillAllItem);
                e.Menu.Items.Add(fillGroupItem);
            }
        }

        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

                DataRow rowFocused = grvSelectVTBaoGia.GetFocusedDataRow();
                if (rowFocused == null) return;
                bool.TryParse(rowFocused["IsUseBG"]?.ToString(), out bool IsUseBG);

                if (view.FocusedColumn == colIsCheck && !IsUseBG)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch(Exception ex)
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

        private void searchControl1_TextChanged(object sender, EventArgs e)
        {
            string keyword = searchControl1.Text.Trim();
            ApplyVatTu(keyword);
        }
        private void ApplyVatTu(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                grvSelectVTBaoGia.ActiveFilter.Clear();
                return;
            }

            grvSelectVTBaoGia.ActiveFilterString = $@"
            [TenCL] LIKE '%{keyword}%'
            OR [MoTa] LIKE '%{keyword}%'
            OR [ItemCode] LIKE '%{keyword}%'
            OR [MaMauVT] LIKE '%{keyword}%'
            OR [MauVT] LIKE '%{keyword}%'
          
        ";
        }
        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
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

       

    }
}