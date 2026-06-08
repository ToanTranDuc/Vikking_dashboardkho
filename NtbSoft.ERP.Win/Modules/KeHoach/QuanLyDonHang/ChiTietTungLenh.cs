using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Newtonsoft.Json;
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
    public partial class ChiTietTungLenh : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        int pageIndex = 1;
        int pageSize = 100;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public ChiTietTungLenh()
        {
            InitializeComponent();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            barEditItemSpinPageSize.EditValue = pageSize;
        }

        private void barButtonItemPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            LoadDSDonHangTong();
        }

        private void barButtonItemNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.Caption = pageIndex.ToString();
            LoadDSDonHangTong();
        }
        private void LoadDSDonHangTong()
        {
            string urlGetTQ = URL + "ChiTietTungLenh/GetTongQuanTungLenh?pageIndex=" + barStaticItemPageIndex.Caption + "&&pageSize=" + pageSize;
            string jsonResult = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetTQ); }).Result;
            DataTable tblResult = JsonConvert.DeserializeObject<DataTable>(jsonResult);

            gridControl1.DataSource = tblResult;

            if (tblResult == null || tblResult.Rows.Count == 0)
            {
                gridControl2.DataSource = null;
                return;
            }

            LoadChiTiet(tblResult.Rows[0]["MaLenhSanXuat"].ToString());
        }
        private void LoadDSDonHangTongTheoML()
        {
            if (barEditItem1.EditValue == null)
            {
                LoadDSDonHangTong();
                return;
            }
            string urlGetTQ = string.Format(@"{0}?maHang={1}", URL + "ChiTietTungLenh/GetTongQuanTungLenhTheoMaHang", barEditItem1.EditValue == null ? "" : barEditItem1.EditValue.ToString());
            string jsonResult = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetTQ); }).Result;
            DataTable tblResult = JsonConvert.DeserializeObject<DataTable>(jsonResult);

            gridControl1.DataSource = tblResult;

            if (tblResult == null || tblResult.Rows.Count == 0)
            {
                gridControl2.DataSource = null;
                return;
            }

            LoadChiTiet(tblResult.Rows[0]["MaLenhSanXuat"].ToString());
        }
        private void GetMaHang()
        {
            string urlGetTQ = URL + "ChiTietTungLenh/GetMaHang";
            string jsonResult = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetTQ); }).Result;
            DataTable tblResult = JsonConvert.DeserializeObject<DataTable>(jsonResult);

            RepositoryItemSearchLookUpEdit comboBox = barEditItem1.Edit as RepositoryItemSearchLookUpEdit;

            if (tblResult.Rows.Count > 0)
            {
                if (comboBox != null)
                {
                    comboBox.DataSource = tblResult;

                }
            }
        }
        private void LoadChiTiet(string maLenhSX)
        {

            if (maLenhSX == null)
            {
                int rowHandle = gridView1.FocusedRowHandle;
                if (rowHandle < 0)
                    return;
                object maLenhSXVal = (gridView1.GetRowCellValue(rowHandle, gridColumn1));
                maLenhSX = maLenhSXVal == null ? string.Empty : maLenhSXVal.ToString();
                if (string.IsNullOrEmpty(maLenhSX))
                    return;
            }

            string urlGetTQ = URL + "ChiTietTungLenh/GetChiTietTungLenh?maLenhSX=" + maLenhSX;
            string jsonResult = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetTQ); }).Result;
            DataTable tblResult = JsonConvert.DeserializeObject<DataTable>(jsonResult);
            gridBand_Size.Children.Clear();
            foreach (DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn gridColumn in bandedGridView1.Columns)
            {
                if (gridColumn.FieldName.Contains("@@Size@@"))
                    gridColumn.Summary.Clear();
            }
            for (int i = bandedGridView1.GroupSummary.Count - 1; i >= 0; i--)
            {
                GridGroupSummaryItem item = bandedGridView1.GroupSummary[i] as GridGroupSummaryItem;
                if (item.FieldName.Contains("@@Size@@"))
                {
                    bandedGridView1.GroupSummary.RemoveAt(i);
                }
            }

            if (tblResult.Rows.Count > 0)
            {
                for (int i = 0; i < tblResult.Columns.Count; i++)
                {
                    string colName = tblResult.Columns[i].ColumnName.ToString();
                    if (colName.Contains("@@Size@@"))
                    {
                        string size = colName.Replace("@@Size@@", "☺").Split('☺')[0];
                        string sizeID = colName.Replace("@@Size@@", "☺").Split('☺')[1];

                        bandedGridView1.OptionsView.ShowFooter = true;
                        GridBand band_chidren = new GridBand();
                        band_chidren.Name = "bandGrid_" + sizeID;
                        band_chidren.Caption = size;

                        BandedGridColumn colSize = bandedGridView1.Columns.AddField(colName);
                        colSize.Name = "gridColSize_" + sizeID;
                        colSize.OptionsColumn.AllowEdit = false;
                        colSize.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                        colSize.Visible = true;
                        colSize.OwnerBand = band_chidren;
                        band_chidren.Visible = true;

                        gridBand_Size.Children.Add(band_chidren);
                    }
                }
                for (int i = 0; i < bandedGridView1.Columns.Count; i++)
                {
                    string colName = bandedGridView1.Columns[i].FieldName;
                    if (colName.Contains("@@Size@@"))
                    {
                        GridColumnSummaryItem columnSummaryItem = new GridColumnSummaryItem(SummaryItemType.Sum, colName, "{0:n0}");
                        columnSummaryItem.DisplayFormat = "{0:n0}";
                        bandedGridView1.Columns[i].Summary.Add(columnSummaryItem);
                    }
                }
            }
            gridControl2.DataSource = tblResult;
            gridControl2.RefreshDataSource();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            LoadDSDonHangTong();
            barEditItem1.EditValue = null;
        }

        //private void bandedGridView1_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        //{
        //    e.Appearance.BackColor = Color.LightBlue;
        //    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
        //    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //    e.Handled = false;
        //}

        private void bandedGridView1_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            Font boldFont = new Font(new FontFamily("Tahoma"), 9, FontStyle.Bold);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect, boldFont, Brushes.Black, format);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            Font boldFont = new Font(new FontFamily("Tahoma"), 9, FontStyle.Bold);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect, boldFont, Brushes.Black, format);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadChiTiet(null);
        }

        private void bandedGridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            if (bandedGridView1.IsGroupRow(e.RowHandle))
            {
                int groupIndex = bandedGridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.SeaGreen;
                }
                e.Appearance.ForeColor = textColor;
                e.DefaultDraw();
                e.Graphics.DrawLine(Pens.Black, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                e.Handled = true;
            }
        }

        private void barEditItemSpinPageSize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void ChiTietTungLenh_Load(object sender, EventArgs e)
        {
            LoadDSDonHangTong();
        }

        private void barEditItemSpinPageSize_EditValueChanged(object sender, EventArgs e)
        {
            GetMaHang();
            pageSize = int.Parse(barEditItemSpinPageSize.EditValue.ToString());
            LoadDSDonHangTong();
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void barEditItem1_EditValueChanged_1(object sender, EventArgs e)
        {
            LoadDSDonHangTongTheoML();
        }
    }
}