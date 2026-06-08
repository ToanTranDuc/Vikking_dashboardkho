using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmBienBanMoKien : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _isTime = true;
        public frmBienBanMoKien()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            barEditItemFromDate.EditValue = DateTime.Now;
            barEditItemToDate.EditValue = DateTime.Now;
            CreateSearchLookUpSoLo();
            CreateSearchLookUpSoLot();
            CreateSearchLookUpCay();
            string[] items = new string[] { "Thời gian", "Lô"};
            ItemComboBoxLoc.Properties.Items.Clear();

            foreach (string item in items)
            {
                ItemComboBoxLoc.Properties.Items.Add(item);
            }
         
            cbBoxLoc.EditValue = items[0].ToString();
        }
        private void CreateSearchLookUpSoLo()
        {
            string url = string.Format("{0}?", URL + "BienBanMoKien/GetSoLo");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repSoLo.DataSource = tbl;
            repSoLo.DisplayMember = "SoLo";
            repSoLo.ValueMember = "SoLoID";
        }
        private void CreateSearchLookUpSoLot()
        {
            string url = string.Format("{0}?", URL + "BienBanMoKien/GetSoLot");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEditlot.DataSource = tbl;
            repositoryItemSearchLookUpEditlot.DisplayMember = "SoLoT";
            repositoryItemSearchLookUpEditlot.ValueMember = "SoLoT";
        }
        private void CreateSearchLookUpCay()
        {
            string url = string.Format("{0}?", URL + "BienBanMoKien/GetCay");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEditCay.DataSource = tbl;
            repositoryItemSearchLookUpEditCay.DisplayMember = "ChiTiet";
            repositoryItemSearchLookUpEditCay.ValueMember = "MaVTID";
        }
        private void LoadData()
        {
            try
            {
                string fromDate = Convert.ToDateTime(barEditItemFromDate.EditValue).ToString("yyyy-MM-dd");
                string toDate = Convert.ToDateTime(barEditItemToDate.EditValue).ToString("yyyy-MM-dd");
                if (barEditItemFromDate.EditValue == null || barEditItemToDate.EditValue == null) return;
                string mavtID = "";
                if (barCay.EditValue != null) mavtID = barCay.EditValue?.ToString();

                string url;
                if (_isTime)
                {
                    url = string.Format("{0}?fromDate={1}&&toDate={2}&&solo={3}&&solot={4}", URL + "BienBanMoKien/Get", fromDate.ToString(), toDate.ToString(), barSoLo.EditValue == null ? "" : barSoLo.EditValue.ToString(),
              mavtID);
                }
                else
                {
                    url = string.Format("{0}?&&solo={1}&&solot={2}", URL + "BienBanMoKien/GetNoDate", barSoLo.EditValue == null ? "" : barSoLo.EditValue.ToString(),
              mavtID);
                }


                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]" || json == "")
                {
                    gridControl1.DataSource = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (!tbl.Columns.Contains("Thua")) tbl.Columns.Add("Thua", typeof(decimal));
                if (!tbl.Columns.Contains("Thieu")) tbl.Columns.Add("Thieu", typeof(decimal));
                foreach (DataRow dr in tbl.Rows)
                {
                    decimal _theoct = Convert.ToDecimal(dr["TheoCT"].ToString());
                    decimal _thucnhap = Convert.ToDecimal(dr["ThucNhap"].ToString());
                    if(_thucnhap> _theoct)
                    {
                        dr["Thua"] = _thucnhap-_theoct;
                        dr["Thieu"] = 0;
                    }   
                    else if(_thucnhap<_theoct)
                    {
                        dr["Thua"] = 0;
                        dr["Thieu"] = _theoct-_thucnhap;
                    }
                    else
                    {
                        dr["Thua"] = 0;
                        dr["Thieu"] = 0;
                    }
                }
                gridControl1.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
          
        }

        private void btLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditItemFromDate.EditValue != null && barEditItemToDate.EditValue != null)
            {
                DateTime _fromDate = Convert.ToDateTime(barEditItemFromDate.EditValue);
                DateTime _toDate = Convert.ToDateTime(barEditItemToDate.EditValue);
                int kq = DateTime.Compare(_fromDate, _toDate);
                if (kq > 0)
                {
                    XtraMessageBox.Show("Giá trị Từ ngày phải nhỏ hơn giá trị Đến ngày!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string fromDate = string.Empty, toDate = string.Empty;
                fromDate = barEditItemFromDate.EditValue.ToString();
                toDate = barEditItemToDate.EditValue.ToString();
                LoadData();
            }
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu ");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("BienBanMoKien{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BienBanMoKien.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, tbl);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(Sfd.FileName))
                            System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                    catch
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                    }
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }

        public void Export(string TemplateFileName, string ExportFileName, DataTable tbl)
        {
            try
            {
                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                    FileInfo templateFile = new FileInfo(TemplateFileName);

                    var tblSoLo = tbl.AsEnumerable()
                                        .Select(x => new
                                        {
                                            SoLo = x["SoLoID"]
                                        }).ToList().Distinct();
                    foreach (var item in tblSoLo)
                    {
                        using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                        {
                            DataTable tblNew = tbl.AsEnumerable().Where(x => x["SoLoID"].ToString() == item.SoLo.ToString()).CopyToDataTable();
                            ExcelWorksheet templateSheet = templatePackage.Workbook.Worksheets["Sheet1"];
                            ExcelWorksheet newSheet = excelPackage.Workbook.Worksheets.Add(item.SoLo.ToString(), templateSheet);
                            string vattuNext = "";
                            int row = 14;

                            newSheet.Cells["b4"].Value = tblNew.Rows[0]["Tau"];
                            newSheet.Cells["b5"].Value = tblNew.Rows[0]["Cang"];
                            newSheet.Cells["c6"].Value = tblNew.Rows[0]["NhaCungCap"];
                            newSheet.Cells["c7"].Value = tblNew.Rows[0]["KhachHang"];
                            newSheet.Cells["c8"].Value = tblNew.Rows[0]["SoHopDong"];
                            newSheet.Cells["c9"].Value = tblNew.Rows[0]["SoToKhai"];
                            newSheet.Cells["b10"].Value = tblNew.Rows[0]["SoLo"];

                            newSheet.Cells["h5"].Value = tblNew.Rows[0]["NgayNhapKho"] != DBNull.Value && DateTime.TryParse(tblNew.Rows[0]["NgayNhapKho"].ToString(), out DateTime d1) ? d1.ToString("dd/MM/yyyy") : "";

                            newSheet.Cells["h9"].Value = tblNew.Rows[0]["NgayMoTK"] != DBNull.Value && DateTime.TryParse(tblNew.Rows[0]["NgayMoTK"].ToString(), out DateTime d2) ? d2.ToString("dd/MM/yyyy") : "";

                            newSheet.Cells["k5"].Value = tblNew.Rows[0]["SoBienBan"];
                            //newSheet.Cells["k8"].Value = Convert.ToDateTime(tblNew.Rows[0]["NgayKyHD"]).ToString("dd/MM/yyyy");

                            newSheet.Cells["k8"].Value = tblNew.Rows[0]["NgayKyHD"] != DBNull.Value && DateTime.TryParse(tblNew.Rows[0]["NgayKyHD"].ToString(), out DateTime d3) ? d3.ToString("dd/MM/yyyy") : "";
                            newSheet.Cells["k9"].Value = tblNew.Rows[0]["SoVanDon"];
                         

                            foreach (DataRow dr in tblNew.Rows)
                            {
                                if (dr["MaVTID"].ToString() != vattuNext)
                                {
                                    newSheet.InsertRow(row, 1);
                                    newSheet.Cells[row, 1].Value = dr["ChiTiet"].ToString();
                                    newSheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    newSheet.Cells[row, 1, row, 9].Merge = true;
                                    newSheet.Cells[row, 10, row, 12].Merge = true;
                                   
                                    row++;
                                }
                                newSheet.InsertRow(row, 1);
                                newSheet.Cells[row, 1, row, 4].Value = dr["MauVT"].ToString();
                                newSheet.Cells[row, 1, row, 4].Merge = true;
                                newSheet.Cells[row, 5].Value = dr["TenDVVT"].ToString();
                                newSheet.Cells[row, 6].Value = dr["ThucNhap"].ToString();
                                newSheet.Cells[row, 7].Value = dr["TheoCT"].ToString();
                                newSheet.Cells[row, 8].Value = dr["Thua"].ToString() ;
                                newSheet.Cells[row, 9].Value = dr["Thieu"].ToString();
                                newSheet.Cells[row, 10,row,12].Value = dr["MaHaiQuan"].ToString();
                                newSheet.Cells[row, 10, row, 12].Merge = true;
                                vattuNext = dr["MaVTID"].ToString();
                                row++;
                            }
                            newSheet.Cells[row + 1, 10].Value = $"Ngày: { DateTime.Now.ToString("dd/MM/yyyy")}";
                            var borderData = newSheet.Cells[14, 1, row - 1, 12].Style.Border;
                            borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                // Bạn nên log hoặc throw lỗi để dễ debug
                Console.WriteLine(ex.Message);
            }
        }

        private void cbBoxLoc_EditValueChanged(object sender, EventArgs e)
        {
            checkValueLoc();
        }
        private void checkValueLoc()
        {
            string cbxText = cbBoxLoc.EditValue.ToString();
            if (cbxText == "Thời gian")
            {
                _isTime = true;
                barStaticItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barSoLo.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;


                barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItemFromDate.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItemToDate.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barEditItemFromDate.EditValue = DateTime.Now;
                barEditItemToDate.EditValue = DateTime.Now;
                barCay.EditValue = null;

            }
            else
            {
                _isTime = false;
                barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barEditItemFromDate.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barEditItemToDate.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barSoLo.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barSoLo.EditValue = null;
                barCay.EditValue = null;

            }
        }

        private void bandedGridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void bandedGridView1_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void bandedGridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == bandedGridColumn1)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == bandedGridColumn13)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (bandedGridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = bandedGridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }
    }
}
