using DevExpress.XtraGrid.Views.BandedGrid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
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

namespace NtbSoft.ERP.Win.Modules.BaoCao
{
    public partial class frmBaoCaoScanBarcode : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                        new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        private string MaPhieuXH = "",Cont = "";
        public frmBaoCaoScanBarcode()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            searchLookUpEdit_Phieu.EditValueChanged += SearchLookUpEdit_Phieu_EditValueChanged;
            grvPKLXuatHang.CustomSummaryCalculate += GrvPKLXuatHang_CustomSummaryCalculate;
            grvPKLXuatHang.CustomColumnDisplayText += GrvPKLXuatHang_CustomColumnDisplayText;
            grvPKLXuatHang.CellMerge += GrvPKLXuatHang_CellMerge;          
        }

        private void GrvPKLXuatHang_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }

        private void GrvPKLXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }

        private void GrvPKLXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }

        private void SearchLookUpEdit_Phieu_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Phieu.EditValue is null || searchLookUpEdit_Phieu.EditValue.ToString() == "") return;
            var value = searchLookUpEdit_Phieu.EditValue.ToString().Split('|');
            MaPhieuXH = value[0];
            Cont = value[1];
            GetData();
            GetDataCTPhieu(MaPhieuXH);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            searchLookUpEdit_Phieu.Properties.DisplayMember = "MaPKLXH_Display";
            searchLookUpEdit_Phieu.Properties.ValueMember = "value";
            searchLookUpEdit_Phieu.Properties.NullText = "[Chọn giá trị]";
            GetPhieu();
        }
        private void GetPhieu()
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetPhieuXHScan&Para1=All&Para2={DateTime.Now.ToString("yyyy-MM-dd")}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtDSPhieu = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Phieu.Properties.DataSource = dtDSPhieu;
        }
        private void GetData()
        {
            string url = string.Format("{0}", URL + $"ScanBarcode/Get?Action=GetCTPhieuScan&Para1={MaPhieuXH}&Para2={Cont}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtData = JsonConvert.DeserializeObject<DataTable>(json);
            dgcTong.DataSource = dtData;
        }
        private void GetDataCTPhieu(string maPhieuXH)
        {
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXH&Para1={maPhieuXH}&Para2=All");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var dtTempA = JsonConvert.DeserializeObject<DataTable>(json);
            var dtTemp = dtTempA.AsEnumerable().Where(x => x["Cont"].ToString() == Cont);
            var dtDSCTPhieu = dtTemp.Count() > 0 ? dtTemp.CopyToDataTable() : new DataTable();
            CreateBandSize(dtDSCTPhieu, grvPKLXuatHang, gbSize);
            dgrPKLXuatHang.DataSource = dtDSCTPhieu;
            KHDongThungLib.AllowVieworNotPack(dtDSCTPhieu, BandPCB, BandPack, BandStore);
        }
        private void ExportExcel(string pathSave)
        {
            DataTable dt = dgcTong.DataSource as DataTable;
            if (dt is null || dt.Rows.Count < 0) return;
            string Paths = Directory.GetCurrentDirectory() + "\\Templates\\ScanBarcode.xlsx";
            FileInfo file = new FileInfo(Paths);
            FileInfo fileSave = new FileInfo(pathSave);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            ExcelRange range;
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                int row = 7;
                worksheet.Cells[3, 1].Value = searchLookUpEdit_Phieu.Text;
                foreach (DataRow dr in dt.Rows)
                {
                    int col = 1;
                    worksheet.Cells[row, col].Value = dr["TenHang"]; col++;
                    worksheet.Cells[row, col].Value = dr["PO"]; col++;
                    worksheet.Cells[row, col].Value = dr["DauSize"]; col++;
                    worksheet.Cells[row, col].Value = dr["TenMau"]; col++;
                    worksheet.Cells[row, col].Value = dr["Size"]; col++;
                    worksheet.Cells[row, col].Value = dr["SLThung"]; col++;
                    worksheet.Cells[row, col].Value = dr["STDaQuet"]; col++;
                    worksheet.Cells[row, col].Value = dr["Barcode"]; col++;
                    row++;
                }
                var sumSLThung = dt.AsEnumerable().Sum(x => Convert.ToInt16(x["SLThung"]));
                var sumSLThungDaQuet = dt.AsEnumerable().Sum(x => Convert.ToInt16(x["STDaQuet"]));
                range = worksheet.Cells[row, 1, row, 5];
                range.Merge = true;
                range.Value = "Tổng";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Font.Bold = true;
                worksheet.Cells[row, 6].Value = sumSLThung;
                worksheet.Cells[row, 7].Value = sumSLThungDaQuet;
                worksheet.Cells[row, 6, row, 7].Style.Font.Color.SetColor(Color.Red);
                worksheet.Cells[row, 6, row, 7].Style.Font.Bold = true;
                var rangeA = worksheet.Cells[7, 1, row, 8].Style.Border;
                rangeA.Bottom.Style =
                    rangeA.Top.Style =
                     rangeA.Left.Style =
                        rangeA.Right.Style = ExcelBorderStyle.Thin;
                excelPackage.SaveAs(fileSave);
            }
        }

        private void grvTong_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = Convert.ToInt16(grvTong.GetRowCellValue(e.RowHandle, "IsThungLe"));
            if (IsThungLe == 1) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            Sfd.FileName = string.Format("PhieuScanThung_{0}", DateTime.Now.ToString("ddMMyyyy"));
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("Sheet1");
                    ExportExcel(Sfd.FileName);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }

        private void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBand(_sizeID, gbSizeA)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = this.repotxtN0;
                col.Visible = true;
                col.Width = 40;
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                //if (grvShared == dgwTong)
                //{
                //    if (col.FieldName.Contains("@"))
                //    {
                //        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                //        itemSize.FieldName = col.FieldName;
                //        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                //        itemSize.DisplayFormat = "{0:n0}";
                //        itemSize.ShowInGroupColumnFooter = col;
                //        grvShared.GroupSummary.Add(itemSize);
                //    }

                //    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                //    if (arrName.Length > 1)
                //    {
                //        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                //    }
                //}
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetData();
            GetDataCTPhieu(MaPhieuXH);
        }

        private bool CheckExistBand(string size, GridBand gbSizeA)
        {
            GridBand gbCheck = gbSizeA.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
    }
}
