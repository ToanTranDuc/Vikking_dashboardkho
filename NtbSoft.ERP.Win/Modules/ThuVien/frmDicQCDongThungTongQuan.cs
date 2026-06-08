using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.DicForm
{
    public partial class frmDicQCDongThungTongQuan : XtraForm
    {
        private readonly HttpClientExtension _clientExtension;
        private DataTable _dtData;
        private readonly string URL;

        public frmDicQCDongThungTongQuan()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            URL = (string)new System.Configuration.AppSettingsReader()
                .GetValue("URL", typeof(string));

            _clientExtension = new HttpClientExtension();
            _dtData = new DataTable();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadData();
        }

        private async void LoadData(string focusMaHang = null)
        {
            try
            {
                string url = $"{URL}DicQCDongThung/Get?action=GetTongQuanQCDT";
                string json = await _clientExtension.GetAsnyc(url);

                if (string.IsNullOrWhiteSpace(json))
                {
                    XtraMessageBox.Show("Không nhận được dữ liệu từ server.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _dtData = JsonConvert.DeserializeObject<DataTable>(json);

                if (_dtData == null || _dtData.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    return;
                }
                if (!_dtData.Columns.Contains("btnChiTietQC"))
                    _dtData.Columns.Add("btnChiTietQC", typeof(string));

                gridControl1.DataSource = _dtData;

                // Focus lại dòng đã chọn
                if (!string.IsNullOrEmpty(focusMaHang))
                {
                    RestoreFocus(focusMaHang);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void RestoreFocus(string maHang)
        {
            try
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    DataRow dr = gridView1.GetDataRow(i);
                    if (dr != null && dr["MaHang"]?.ToString() == maHang)
                    {
                        gridView1.FocusedRowHandle = i;
                        gridView1.MakeRowVisible(i);
                        break;
                    }
                }
            }
            catch { }
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportToExcel();
        }
        //private void ExportToExcel()
        //{
        //    if (gridControl1.DataSource == null || _dtData == null || _dtData.Rows.Count == 0)
        //    {
        //        XtraMessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    using (SaveFileDialog sfd = new SaveFileDialog())
        //    {
        //        sfd.Filter = "Excel Files|*.xlsx";
        //        sfd.Title = "Xuất danh sách carton list";
        //        sfd.FileName = $"TongQuanCartonList_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        //        if (sfd.ShowDialog() == DialogResult.OK)
        //        {
        //            try
        //            {
        //                using (ExcelPackage package = new ExcelPackage())
        //                {
        //                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Carton List");
        //                    string[] headers = new string[]
        //                    {
        //                        "No", "Style", "Kích thước\n(Dài*Rộng*Cao)", "Đơn vị","Size", "Số lớp", "Loại thùng", "Ghi chú"
        //                        //"No", "Số lớp", "Loại thùng", "Ngày", "Kích thước\n(Dài*Rộng*Cao)",
        //                        //"Style", "Số cái/thùng", "Size", "Ghi chú"
        //                    };

        //                    for (int col = 1; col <= headers.Length; col++)
        //                    {
        //                        ws.Cells[1, col].Value = headers[col - 1];
        //                        ws.Cells[1, col].Style.Font.Bold = true;
        //                        ws.Cells[1, col].Style.Font.Size = 13;
        //                        ws.Cells[1, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells[1, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                        ws.Cells[1, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                        ws.Cells[1, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 212, 128));
        //                        ws.Cells[1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //                        ws.Cells.Style.Font.Name = "Cambria";
        //                        ws.Cells[1, col].Style.WrapText = true;
        //                    }
        //                    ws.Row(1).Height = 35;
        //                    int row = 2;
        //                    int stt = 1;

        //                    // Duyệt qua tất cả các row VISIBLE trên View (đã filter)
        //                    for (int i = 0; i < gridView1.DataRowCount; i++)
        //                    {
        //                        // Chỉ lấy row thực (bỏ group row, summary row, v.v.)
        //                        if (gridView1.IsDataRow(i) && !gridView1.IsGroupRow(i))
        //                        {
        //                            // Lấy DataRow tương ứng
        //                            DataRow dr = gridView1.GetDataRow(i);

        //                            if (dr == null) continue;

        //                            string kichThuoc = dr["TenQuiCach"]?.ToString() ?? "";
        //                            string maDVStr = dr["MaDV"]?.ToString() ?? "";

        //                            kichThuoc = kichThuoc.Replace("x", "*").Replace("X", "*");

        //                            if (int.TryParse(maDVStr, out int maDV) && maDV == 2)
        //                            {
        //                                kichThuoc += "\n\"";
        //                            }

        //                            ws.Cells[row, 1].Value = stt++; 
        //                            ws.Cells[row, 2].Value = dr["MaHang"];
        //                            ws.Cells[row, 3].Value = kichThuoc;
        //                            ws.Cells[row, 4].Value = dr["SizeTheoKT"];
        //                            ws.Cells[row, 5].Value = dr["SoLop"];
        //                            ws.Cells[row, 6].Value = dr["LoaiThung"];
        //                            ws.Cells[row, 7].Value = dr["GhiChu"];

        //                            for (int col = 1; col <= 7; col++)
        //                            {
        //                                ws.Cells[row, col].Style.Font.Name = "Cambria";
        //                                ws.Cells[row, col].Style.Font.Size = 12;
        //                                ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //                                if (col == 1 || col == 3 || col == 4 || col == 5)
        //                                    ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                                else
        //                                    ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

        //                                ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                            }

        //                            ws.Cells[row, 5].Style.Font.Bold = true;
        //                            ws.Cells[row, 6].Style.Font.Bold = true;

        //                            row++;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return;
        //            }
        //        }
        //    }
        //}
        private void ExportToExcel()
        {
            if (gridControl1.DataSource == null || gridView1.RowCount == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.Title = "Xuất danh sách carton list (chỉ dòng đang hiển thị)";
                sfd.FileName = $"TongQuanCartonList_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets.Add("Carton List");

                        // Header – sửa để khớp 8 cột (thêm Đơn vị)
                        string[] headers = new string[]
                        {
                            "No", "Khách hàng","Style", "Kích thước\n(Dài*Rộng*Cao)", "Đơn vị", "Số cái/thùng","Size", "Số lớp", "Loại thùng", "Ghi chú"
                        };

                        for (int col = 1; col <= headers.Length; col++)
                        {
                            ws.Cells[1, col].Value = headers[col - 1];
                            ws.Cells[1, col].Style.Font.Bold = true;
                            ws.Cells[1, col].Style.Font.Size = 13;
                            ws.Cells[1, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[1, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[1, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[1, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 212, 128));
                            ws.Cells[1, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells[1, col].Style.WrapText = true;
                        }
                        ws.Row(1).Height = 35;

                        int row = 2;
                        int stt = 1;

                        // Duyệt visible rows
                        for (int i = 0; i < gridView1.DataRowCount; i++)
                        {
                            if (gridView1.IsDataRow(i) && !gridView1.IsGroupRow(i))
                            {
                                DataRow dr = gridView1.GetDataRow(i);
                                if (dr == null) continue;

                                string kichThuoc = (dr["TenQuiCach"]?.ToString() ?? "").Replace("x", "*").Replace("X", "*");
                                string maDVStr = dr["MaDV"]?.ToString() ?? "";

                                if (int.TryParse(maDVStr, out int maDV) && maDV == 2)
                                {
                                    kichThuoc += "";
                                    //kichThuoc += "\n\"";
                                }

                                ws.Cells[row, 1].Value = stt++;
                                ws.Cells[row, 2].Value = dr["GhiChu"];
                                ws.Cells[row, 3].Value = dr["MaHang"];
                                ws.Cells[row, 4].Value = kichThuoc;
                                ws.Cells[row, 5].Value = dr["TenDV"];
                                ws.Cells[row, 6].Value = dr["SizeTheoPCB"];
                                ws.Cells[row, 7].Value = dr["SizeTheoKT"];
                                ws.Cells[row, 8].Value = dr["SoLop"];
                                ws.Cells[row, 9].Value = dr["LoaiThung"];
                                ws.Cells[row, 10].Value = dr["GhiChu2"];

                                // Format
                                for (int col = 1; col <= 10; col++)
                                {
                                    ws.Cells[row, col].Style.Font.Name = "Cambria";
                                    ws.Cells[row, col].Style.Font.Size = 12;
                                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                    // Căn giữa cho các cột số liệu
                                    if (col == 1 || col == 4 || col == 5 || col == 8)
                                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    else
                                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                }
                                ws.Cells[row, 4].Style.Font.Bold = true;
                                ws.Cells[row, 6].Style.Font.Bold = true;
                                ws.Cells[row, 7].Style.Font.Bold = true;

                                row++;
                            }
                        }

                        // Điều chỉnh độ rộng cột
                        ws.Column(1).Width = 8;
                        ws.Column(2).Width = 25;
                        ws.Column(3).Width = 25;
                        ws.Column(4).Width = 25;
                        ws.Column(5).Width = 12;
                        ws.Column(6).Width = 30;
                        ws.Column(7).Width = 30;
                        ws.Column(8).Width = 15;
                        ws.Column(9).Width = 25;
                        ws.Column(10).Width = 30;
                        ws.View.FreezePanes(2, 1);

                        FileInfo fi = new FileInfo(sfd.FileName);
                        package.SaveAs(fi);
                    }
                    DialogResult res = XtraMessageBox.Show(
                        "Xuất Excel thành công!\nBạn có muốn mở file không?",
                        "Thành công",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (res == DialogResult.Yes)
                    {
                        try { System.Diagnostics.Process.Start(sfd.FileName); }
                        catch {}
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi khi xuất file:\n{ex.Message}",
                        "Lỗi Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = e.ListSourceRowIndex + 1;
            }

        }   

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "TenQuiCach")
            {
                if (e.Value != null && e.Value is string str && !string.IsNullOrWhiteSpace(str))
                {
                    e.DisplayText = str.Replace("x", "*");
                }
            }
        }

        private void btnCDQC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmSaveTL_KL frm = new frmSaveTL_KL();
            frm.WindowState = FormWindowState.Maximized;
            frm.Clear();
            frm.Show();

            frm.FormClosed += (s, args) =>
            {
                LoadData();
            };
        }

        private void repoChiTietQC_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;

            string styleId = dr["MaHang"]?.ToString();
            if (string.IsNullOrEmpty(styleId))
            {
                return;
            }

            using (frmSaveTL_KL frm = new frmSaveTL_KL())
            {
                frm.styleID = styleId;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog(this);
                LoadData(styleId);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "TrangThai" && e.RowHandle >= 0)
            {
                DataRow row = gridView1.GetDataRow(e.RowHandle);
                if (row != null)
                {
                    string trangThai = row["TrangThai"]?.ToString() ?? "";
                    if (trangThai.Contains("Đã tạo"))
                    {
                        e.Appearance.ForeColor = Color.DarkGreen;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            }
        }
    }
}