using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmSelectVTBaoGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _loaicc, _maNCC, _chungloaiCC = string.Empty;
        string _maPhieuBG = string.Empty;

        private DataTable tblVatTuBaoGia;

        DataTable _tbl = new DataTable();

        public DataTable tblVatTuSelected { get; set; } = new DataTable();
        public int SoDotGH { get; set; } = -1;
        public bool IsAddDotGH { get; set; } = false;
        public frmSelectVTBaoGia(string LoaiCC, string MaNhaCC,string NCC,string MaPhieuBG,DataTable tbl,string ChungLoaiCC = "")
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
       
                if (Int32.TryParse(spinSoDot.EditValue?.ToString(), out int value))
                {
                   
                    SoDotGH = value;
                }
                else
                {
                    SoDotGH = 1;
                }

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
                        row["IsUse"] = true;
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
            string url = $"{URL}PhieuBaoGia/GET?action=GetVatTu&para1={_loaicc}&para2={_maNCC}&para3={_maPhieuBG}";
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

        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

                if (view.FocusedColumn == colIsCheck)
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
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
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