using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
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
    public partial class frmExportDonHangTQ : DevExpress.XtraEditors.XtraForm
    {
        public DataTable tblSelectedPO { get; set; }
        public bool IsAll { get; set; }
        private HashSet<DataRow> selectedRowsPOID = new HashSet<DataRow>();
       
        public frmExportDonHangTQ(int x, int y,string TenKH,string TenHang,List<DataRow> tblPO)
        {
            InitializeComponent();     
            SetPosition(x, y);
            SetupComponent(TenKH, TenHang, tblPO);
        }
        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x, y);
        }

       
        private void SetupComponent(string TenKH, string TenHang, List<DataRow> tblPO)
        {
            txtKhachHang.Text = TenKH;
            txtTenHang.Text = TenHang;
            if(tblPO?.Count > 0)
            {
                searchLookUpEdit_PO.Properties.DataSource = tblPO?.CopyToDataTable();
                searchLookUpEdit_PO.Properties.ValueMember = "POID";
                searchLookUpEdit_PO.Properties.DisplayMember = "PO";
               
            }



        }
        private void searchLookUpEditViewPO_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle = e.ControllerRow;
            if (view == null) return;
            DataRow row = view.GetDataRow(e.ControllerRow);
            if (e.Action == CollectionChangeAction.Add)
            {

                if (row != null && !selectedRowsPOID.Contains(row))
                {
                    selectedRowsPOID.Add(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                //object row = view.GetRow(e.ControllerRow);
                if (row != null)
                {
                    selectedRowsPOID.Remove(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                selectedRowsPOID.Clear();


                int[] selectedRowHandles = view.GetSelectedRows();
                foreach (int rowHandle2 in selectedRowHandles)
                {
                    if (rowHandle2 >= 0)
                    {
                        row = view.GetDataRow(rowHandle2);
                        if (row != null)
                        {
                            selectedRowsPOID.Add(row);
                        }
                    }
                }
            }

            string selectedValues = string.Join(";", searchLookUpEditView_PO.GetSelectedRows().Select(rowHandle2 => searchLookUpEditView_PO.GetRowCellValue(rowHandle2, searchLookUpEdit_PO.Properties.ValueMember)));
            searchLookUpEdit_PO.EditValue = selectedValues;
            if (searchLookUpEdit_PO.EditValue is null) return;

        }

        private void searchLookUpEditViewPO_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRowsPOID.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

                //e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);
                if (e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);
                }

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    if (info.ElementPainter != null && info.ElementInfo != null) 
                    {
                        DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }


        private void searchLookUpEditViewPO_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

            if (selectedRowsPOID == null || !selectedRowsPOID.Any())
            {
                e.DisplayText = "Chọn PO";
                searchLookUpEdit_PO.Properties.Appearance.ForeColor = Color.Red;
               
            }
            else
            {
                var displayValues = new List<string>();
                for (int i = 0; i < searchLookUpEditView_PO.DataRowCount; i++)
                {
                    object rowValueObj = searchLookUpEditView_PO.GetRowCellValue(i, searchLookUpEdit_PO.Properties.ValueMember);
                    string rowValue = rowValueObj?.ToString();
                    if (rowValue != null && selectedRowsPOID.Any(dr => dr["POID"]?.ToString() == rowValue))
                    {
                        string displayValue = searchLookUpEditView_PO.GetRowCellValue(i, searchLookUpEdit_PO.Properties.DisplayMember)?.ToString();
                        if (displayValue != null)
                        {
                            displayValues.Add(displayValue);
                        }
                    }
                }

                e.DisplayText = string.Join(";", displayValues);
                searchLookUpEdit_PO.Properties.Appearance.ForeColor = Color.Black;

            }

        }

        private void gVtong_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                if (view.IsGroupRow(e.RowHandle) && view != null && info != null)
                {

                    Color textColor = ColorTranslator.FromHtml("#2A5D9F");
                    Font boldFont = new Font(e.Appearance.Font, FontStyle.Bold);
                    e.Appearance.Font = boldFont;
                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();
                    e.Handled = true;
                }
            }
            catch(Exception ex)
            {

            }
            



        }
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedRowsPOID.Count > 0)
                {
                    // Clone schema từ dòng đầu tiên
                    tblSelectedPO = selectedRowsPOID.First().Table.Clone();

                    foreach (DataRow row in selectedRowsPOID)
                    {
                        tblSelectedPO.ImportRow(row);
                    }

                    IsAll = false;
                }
                else
                {
                    tblSelectedPO = new DataTable(); // hoặc giữ null tùy ý
                    IsAll = true;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu ! Vui lòng thử lại sau."); // gợi ý nên hiển thị để biết lỗi gì
                this.Close();
            }


        }

        private void searchLookUpEdit_PO_Popup(object sender, EventArgs e)
        {
            
            if (searchLookUpEditView_PO != null)
            {
                searchLookUpEditView_PO.ExpandAllGroups();
            }
            
        }

       
    }
}
