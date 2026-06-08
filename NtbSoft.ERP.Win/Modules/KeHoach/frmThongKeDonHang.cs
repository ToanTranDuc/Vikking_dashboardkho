using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.KeHoach;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    public partial class frmThongKeDonHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _po = string.Empty;
        public static   string _phieuBB;
        private HttpClientExtension _clientExtension;
        public frmThongKeDonHang()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            string[] items = new string[] { "Thời gian", "Mã hàng" };
            cbxLoc.Properties.Items.Clear();

            foreach (string item in items)
            {
                cbxLoc.Properties.Items.Add(item);
            }
            dateTuNgay.EditValue = DateTime.Now;
            dateDenNgay.EditValue = DateTime.Now;
            cbxLoc.EditValue = items[0].ToString();
        }

        private void grvThongKeDH_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, e.Location);
            }
        }


        private void grvThongKeDH_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }


        private void Load_lookUp(DateTime tuNgay, DateTime denNgay)
        {
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetDonHang&Para1={tuNgay.ToString("yyyy-MM-dd")}&Para2={denNgay.ToString("yyyy-MM-dd")}&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit2.Properties.DataSource = tbl;
            searchLookUpEdit2.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEdit2.Properties.ValueMember = "MaDH";
            searchLookUpEdit2.Properties.NullText = "[Chọn giá trị]";
            if (tbl.Rows.Count == 0)
            {
                grcChiTietTKDH.DataSource = null;
                grcThongKeDH.DataSource = null;
                return;
            }
            else
            {
                searchLookUpEdit2.EditValue = tbl.Rows[0]["MaDH"];
                Load_DonhangThongKe();
            }

        }



        private void grwChiTietTKDH_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(192, 255, 255), Color.FromArgb(192, 255, 255), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }


        private void grvThongKeDH_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            loadCT();
        }
        public void loadCT()
        {
            int focusedRowHandle = grvThongKeDH.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                string PoID = grvThongKeDH.GetRowCellValue(focusedRowHandle, colPOID).ToString();
                _po = PoID;
                loadChart();
                loadChiTietXuatHang(PoID);
            }
        }

        private void SLXHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int focusHandle = grvThongKeDH.FocusedRowHandle;
                string PoID = grvThongKeDH.GetRowCellValue(focusHandle, colPOID).ToString();

                frmChiTietXuatHang frm = new frmChiTietXuatHang(PoID);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SLDTNKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int focusHandle = grvThongKeDH.FocusedRowHandle;
            string PoID = grvThongKeDH.GetRowCellValue(focusHandle, colPOID).ToString();

            frmChiTietThongKeDTNK frm = new frmChiTietThongKeDTNK(_maDH, PoID);
            frm.ShowDialog();
        }

        public void Load_DonhangThongKe()
        {
            _maDH = searchLookUpEdit2.EditValue.ToString();
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetThongKe&Para1={_maDH}&Para2=A&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0)
            {
                grcChiTietTKDH.DataSource = null;
                grcThongKeDH.DataSource = null;
                return;
            }
            int rowHandle = grvThongKeDH.FocusedRowHandle;
            grcThongKeDH.DataSource = tbl;
            _po = tbl.Rows[0]["POID"].ToString();
            loadChart();
            if (rowHandle == 0)
            {
                loadCT();
            }
        }
        private void loadChart()
        {
            DataTable tblChart = grcThongKeDH.DataSource as DataTable;
            tblChart = tblChart.AsEnumerable().Where(x => x["POID"].ToString() == _po.ToString()).CopyToDataTable();
            Series barSeries = new Series("Số lượng ", ViewType.Bar);
            Series lineSeries = new Series("Số lượng KH", ViewType.Line);

            List<string> colBar = new List<string>() { "SLLapPKL","SoLuongDT", "SoLuongNK", "SoLuongLKH", "SoLuongXH" };
            List<string> nameBar = new List<string>() { "Lập PKL","Đóng thùng", "Nhập kho", "Kế hoạch XH", "Xuất hàng" };
            

            foreach (DataRow item in tblChart.Rows)
            {
                string SLKH = item["SLKH"].ToString();
                for (int i = 0; i < colBar.Count; i++)
                {
                    string columnName = nameBar[i];
                    object value = item[colBar[i]]; 
                   
                    lineSeries.Points.Add(new SeriesPoint(columnName, SLKH));
                    barSeries.Points.Add(new SeriesPoint(columnName, value));
                }
            }


            BarSeriesView barView = (BarSeriesView)barSeries.View;
            barView.Color = Color.Green;

            BarSeriesLabel barSeriesLabel = (BarSeriesLabel)barSeries.Label;
            barSeriesLabel.Position = BarSeriesLabelPosition.Top; // Đặt vị trí của nhãn ở trên đỉnh cột
            barSeriesLabel.BackColor = Color.Transparent; // Đặt nền nhãn trong suốt
            barSeriesLabel.TextColor = Color.Black; // Đặt màu chữ của nhãn
            barSeriesLabel.Visible = true;

            LineSeriesView lineView = (LineSeriesView)lineSeries.View;
            lineView.Color = Color.Blue; // Đặt màu sắc của đường
            lineView.LineMarkerOptions.Kind = MarkerKind.Diamond; // Đặt kiểu của điểm đánh dấu
            lineView.LineMarkerOptions.Color = Color.Blue; // Đặt màu của điểm đánh dấu
            lineView.LineMarkerOptions.Size = 10; // Đặt kích thước của điểm đánh dấu
            lineView.LineStyle.Thickness = 3; // Đặt độ dày của đường
            lineView.LineMarkerOptions.Visible = true;

            // Xóa tất cả series cũ nếu cần
            chartControl1.Series.Clear();

            // Thêm series mới vào biểu đồ
            chartControl1.Series.Add(barSeries);
            chartControl1.Series.Add(lineSeries);

            // Cấu hình trục X để hiển thị nhãn tháng
            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
            diagram.AxisX.Title.Text = "Số lượng";
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisX.Label.Angle = 0;  // Đảm bảo nhãn nằm ngang
            diagram.AxisX.Label.Visible = true;  // Hiển thị nhãn trục X

        }

        private void locNgay()
        {
            DateTime tuNgay = (DateTime)dateTuNgay.EditValue;
            DateTime denNgay = (DateTime)dateDenNgay.EditValue;
            Load_lookUp(tuNgay, denNgay);
        }
        private void dateTuNgay_Properties_EditValueChanged(object sender, EventArgs e)
        {
            DateTime tuNgay = (DateTime)dateTuNgay.EditValue;
            DateTime denNgay = (dateDenNgay.EditValue == null) ? DateTime.Now : (DateTime)dateDenNgay.EditValue;
            if (tuNgay > denNgay)
            {
                MessageBox.Show("Ngày bắt đầu không thể lớn hơn ngày kết thúc!");
                dateTuNgay.EditValue = tempTuNgay;
                checkCoNgay = false;
            }
            else
            {
                checkCoNgay = true;
                Load_lookUp(tuNgay, denNgay);
            }
            if (checkCoNgay)
                tempTuNgay = tuNgay;
        }

        private void cbxLoc_Properties_EditValueChanged(object sender, EventArgs e)
        {
            string cbxText = cbxLoc.Text;
            if (cbxText != "Thời gian")
            {
                layoutDenNgay.Visibility = LayoutVisibility.Never;
                layoutTuNgay.Visibility = LayoutVisibility.Never;
                DateTime tuNgay = Convert.ToDateTime("1990-1-1");
                DateTime denNgay = Convert.ToDateTime("1990-1-1");
                Load_lookUp(tuNgay, denNgay);

            }
            else
            {
                layoutDenNgay.Visibility = LayoutVisibility.Always;
                layoutTuNgay.Visibility = LayoutVisibility.Always;
                locNgay();
            }
        }
        DateTime tempTuNgay = DateTime.Now;
        bool checkCoNgay = true;
        private void dateDenNgay_Properties_EditValueChanged(object sender, EventArgs e)
        {
            locNgay();
        }

        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            Load_DonhangThongKe();
        }

        private void grvChiTietTKDH_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
           
        }

        private void grvChiTietTKDH_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (grvChiTietTKDH.FocusedColumn == colIcon)
            {
                DataRow drRow = grvChiTietTKDH.GetFocusedDataRow();
                if (drRow == null) return;
                string Phieu = drRow["MaPhieuBB"].ToString();
                string isExport = drRow["IsExport"].ToString();
                if (isExport != "2")
                {
                    MessageBox.Show("Phiếu này chưa được xuất hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _phieuBB = Phieu;
                frmBienBanKTXuatHang frm = new frmBienBanKTXuatHang();
                frm.ShowDialog();
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Load_DonhangThongKe();
        }

        private void loadChiTietXuatHang(string PoID)
        {
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=ChitietXH&Para1={_maDH}&Para2={PoID}&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0)
            {
                grcChiTietTKDH.DataSource = new DataTable();
                return;
            }
            grcChiTietTKDH.DataSource = tbl;
        }
    }
}