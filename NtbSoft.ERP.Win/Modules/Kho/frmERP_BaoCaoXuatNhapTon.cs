using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraSplashScreen;
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
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERP_BaoCaoXuatNhapTon : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        bool flagChangeDate = true;
        private HttpClientExtension _clientExtension;
        public frmERP_BaoCaoXuatNhapTon()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            comboBoxEdit1.EditValue = "Thời gian";
            TuNgayEdit.EditValue = new DateTime(DateTime.Now.Year, 1, 1);
            DenNgayEdit.EditValue = DateTime.Now;

            ResetValue();
            InIt();
            GetKH();
            GetSoLo();

            loadCL();
        }
        private void ResetValue()
        {
            searchLookUpEditMaKH.EditValue = null;
            searchLookUpEditMaHang.EditValue = null;
            searchLookUpEditSoLo.EditValue = null;
            searchLookUpEditCL.EditValue = null;
        }
        private void InIt()
        {
            searchLookUpEditMaKH.Properties.ValueMember = "MaKH";
            searchLookUpEditMaKH.Properties.DisplayMember = "TenKH";

            searchLookUpEditMaHang.Properties.ValueMember = "MaHang";
            searchLookUpEditMaHang.Properties.DisplayMember = "TenHang";

            searchLookUpEditSoLo.Properties.ValueMember = "SoLoID";
            searchLookUpEditSoLo.Properties.DisplayMember = "SoLo";

            searchLookUpEditCL.Properties.DisplayMember = "ChungLoaiVatTu";
            searchLookUpEditCL.Properties.ValueMember = "MaCLVT";

        }
        private void GetKH()
        {
            try
            {
                string url = $"{URL}ERPBaoCaoXuatNhapTon/Get?action=GETKH";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditMaKH.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditMaKH.Properties.DataSource = tbl;
                //searchLookUpEditMaKH.EditValue = tbl.Rows[0]["MaNPL"];
                //GetSoLo();
            }
            catch (Exception ex)
            {

            }

        }
        private void loadCL()
        {
            string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GETCL";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //string json = await _clientExtension.GetAsnyc(url);
            if (json == "[]")
            {

                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {

                return;
            }

            searchLookUpEditCL.Properties.DataSource = tbl;
            //searchLookUpEditKH.EditValue = tbl.Rows[0]["MaKH"];
            //await LoadMH();
            //LoadTheKhoTong();
        }
        private void GetMaHang()
        {
            try
            {
                if (searchLookUpEditMaKH.EditValue == null) return;
                string makh = searchLookUpEditMaKH.EditValue.ToString();
                string url = $"{URL}ERPBaoCaoXuatNhapTon/Get?action=GETMH&para={makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditMaHang.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditMaHang.Properties.DataSource = tbl;
                if (tbl.Rows.Count == 1)
                {
                    searchLookUpEditMaHang.EditValue = tbl.Rows[0]["MaHang"];
                }    
               
                //GetSoLo();
            }
            catch (Exception ex)
            {

            }

        }
        private void GetSoLo()
        {
            try
            {
                string makh = string.Empty, mahang = string.Empty;

                if (searchLookUpEditMaKH.EditValue != null)
                    makh = searchLookUpEditMaKH.EditValue.ToString();
                if (searchLookUpEditMaHang.EditValue != null)
                    mahang = searchLookUpEditMaHang.EditValue.ToString();

                string url = $"{URL}ERPBaoCaoXuatNhapTon/Get?action=GETSOLO&para={makh}&para2={mahang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditSoLo.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditSoLo.Properties.DataSource = tbl;
                if(tbl.Rows.Count==1)
                {
                    searchLookUpEditSoLo.EditValue = tbl.Rows[0]["SoLoID"];
                }    
                //GetDataTong();
            }
            catch (Exception ex)
            {

            }

        }
        private void GetDataTong()
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");

                string tungay = "1990-01-01";
                string denngay = "2900-01-01";
                string makh = string.Empty;
                string mahang = string.Empty;

                tungay = TuNgayEdit.EditValue != null
       ? Convert.ToDateTime(TuNgayEdit.EditValue).ToString("yyyy-MM-dd")
       : "1990-01-01";

                denngay = DenNgayEdit.EditValue != null
                    ? Convert.ToDateTime(DenNgayEdit.EditValue).ToString("yyyy-MM-dd")
                    : "2900-01-01";

                makh = searchLookUpEditMaKH.EditValue?.ToString() ?? "";
                mahang = searchLookUpEditMaHang.EditValue?.ToString() ?? "";




                var soloid = searchLookUpEditSoLo.EditValue ?? "";
                var cl = searchLookUpEditCL.EditValue ?? "";
                string url = $"{URL}ERPBaoCaoXuatNhapTon/Get?action=GetTKTong&para={tungay}&para2={denngay}&para3={makh}&para4={mahang}&para5={soloid.ToString()}&para6={cl.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcTheKhoTong.DataSource = null;
                    grcTheKhoNPL.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }
                grcTheKhoTong.DataSource = tbl;
                GetData();
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
            }
        }
        private void GetData()
        {
            try
            {
                DataRow dr = grvTheKhoTong.GetFocusedDataRow();
                if (dr == null) return;

                string tungay = "1990-01-01";
                string denngay = "2900-01-01";
                string makh = string.Empty;
                string mahang = string.Empty;

                tungay = TuNgayEdit.EditValue != null
       ? Convert.ToDateTime(TuNgayEdit.EditValue).ToString("yyyy-MM-dd")
       : "1990-01-01";

                denngay = DenNgayEdit.EditValue != null
                    ? Convert.ToDateTime(DenNgayEdit.EditValue).ToString("yyyy-MM-dd")
                    : "2900-01-01";

                makh = searchLookUpEditMaKH.EditValue?.ToString() ?? "";
                mahang = searchLookUpEditMaHang.EditValue?.ToString() ?? "";

                string itemvai = dr["MaNPL"].ToString();
                var soloid = searchLookUpEditSoLo.EditValue ?? "";
                string url = $"{URL}ERPBaoCaoXuatNhapTon/Get?action=GETCHITIET&para={tungay}&para2={denngay}&para3={itemvai}&para4={soloid.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcTheKhoNPL.DataSource = null;
                    return;
                }
                grcTheKhoNPL.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }

        }

        private void LocDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetDataTong();
        }

        private void searchLookUpItemVai_EditValueChanged(object sender, EventArgs e)
        {
            GetSoLo();
        }

        private void grvTheKhoNPL_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            //GridSummaryItem item = e.Item as GridSummaryItem;

            //if (item.FieldName == "TonKho")
            //{
            //    if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            //    {
            //        int rowHandle = grvTheKhoNPL.RowCount - 1; // Dòng cuối trong grid view
            //        if (rowHandle >= 0)
            //        {
            //            object lastValue = grvTheKhoNPL.GetRowCellValue(rowHandle, "TonKho");
            //            e.TotalValue = lastValue ?? 0;
            //        }
            //        else
            //        {
            //            e.TotalValue = 0;
            //        }
            //    }
            //}
        }

        private void btnEX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dtTable = grcTheKhoNPL.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("TheKhoNPL{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TheKhoNPL.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, dtTable);

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
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["Sheet1"];

                        string datetime = DateTime.Now.ToString("dd/MM/yyyy");
                        ws.Cells["i4"].Value = datetime;
                        ws.Cells["f6"].Value = tbl.Rows[0]["ChiTiet"];
                        ws.Cells["e6"].Value = tbl.Rows[0]["MaHaiQuan"];
                        ws.Cells["b9"].Value = tbl.Rows[0]["MaHaiQuan"];
                        ws.Cells["g9"].Value = tbl.Rows[0]["TenDVVT"];

                        int row = 15;
                        foreach (DataRow item in tbl.Rows)
                        {
                            ws.Cells[row, 1].Value = item["NgayChungTu"].ToString();
                            ws.Cells[row, 2].Value = item["SoChungTu"].ToString();
                            ws.Cells[row, 3, row, 6].Merge = true;
                            ws.Cells[row, 3, row, 6].Value = item["TrichYeu"].ToString();
                            ws.Cells[row, 7].Value = item["NgayXH"].ToString();
                            ws.Cells[row, 8].Value = Math.Round(Convert.ToDouble(item["SLNKEX"]), 2);
                            ws.Cells[row, 9].Value = Math.Round(Convert.ToDouble(item["SLXH"]), 2);
                            ws.Cells[row, 10].Value = Math.Round(Convert.ToDouble(item["TonKho"]), 2);
                            ws.Cells[row, 11, row, 12].Value = "";
                            ws.Cells[row, 11, row, 12].Merge = true;
                            row++;
                        }

                        var borderData = ws.Cells[15, 1, row - 1, 12].Style.Border;
                        borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;


                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
            }


        }

        private void searchLookUpEditMaKH_EditValueChanged(object sender, EventArgs e)
        {
            GetDataTong();
            GetMaHang();
            GetSoLo();


        }

        private void searchLookUpEditMaHang_EditValueChanged(object sender, EventArgs e)
        {
            GetDataTong();
            GetSoLo();
        }

        private void grvTheKhoTong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GetData();
        }

        private void btNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetDataTong();
            GetData();
        }

        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
        {
          
        }

        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("BaoCaoXuatNhapTon{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoXuatNhapTon.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName);

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
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                FileInfo templateFile = new FileInfo(TemplateFileName);
                FileInfo resultFile = new FileInfo(ExportFileName);

                using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                {
                    templatePackage.Workbook.Properties.Author = "NTB";
                    templatePackage.Workbook.Properties.Title = "NhapKhoNPL";
                    templatePackage.SaveAs(resultFile);
                }



                var view = grcTheKhoTong.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;


                List<DataRow> visibleRows = new List<DataRow>();
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    int rowHandle = view.GetVisibleRowHandle(i);
                    if (rowHandle < 0) continue; // bỏ qua group row
                    DataRow row = view.GetDataRow(rowHandle);
                    if (row != null)
                        visibleRows.Add(row);
                }

                if (visibleRows.Count == 0) return;

                using (ExcelPackage excelPackage = new ExcelPackage(resultFile))
                {
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    int startRowInExcel = 8;

                    for (int i = 0; i < visibleRows.Count; i++)
                    {
                        DataRow row = visibleRows[i];
                        int excelRow = startRowInExcel + i;

                        worksheet.Cells[excelRow, 1].Value = row["MaVT"];
                        worksheet.Cells[excelRow, 2].Value = row["ChiTiet"];
                        worksheet.Cells[excelRow, 3].Value = row["MauVT"];
                        worksheet.Cells[excelRow, 4].Value = row["KhoVai"];
                        worksheet.Cells[excelRow, 5].Value = row["TenDVVT"];
                        worksheet.Cells[excelRow, 6].Value = row["TonKhoDK"];
                        worksheet.Cells[excelRow, 7].Value = row["SLNK"];


                        worksheet.Cells[excelRow, 8].Value = row["SLXH"];
                        worksheet.Cells[excelRow, 9].Value = row["TonKho"];
                        worksheet.Cells[excelRow, 10].Value = row["ThanhTien"];
                        worksheet.Cells[excelRow, 11].Value = row["ThanhTienXuat"];
                   

                    }

                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}