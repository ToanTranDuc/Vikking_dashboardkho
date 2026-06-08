using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
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
    public partial class frmBaoCaoTienDoSXWeb : Form
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();

        string webUrl = string.Empty;
        string branchID = string.Empty;
        string account = string.Empty;
        string useNameLogin = string.Empty;
        string urlMain = string.Empty;
        string urlWeb = string.Empty;
        DataTable userRoleDb = new DataTable();
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        private ChromiumWebBrowser chromiumWebBrowser;

        public frmBaoCaoTienDoSXWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCTienDoUrl", typeof(string)).ToString();
            InitializeComponent();

            LoadQuyenTruyCapCacMayChu();
        }
        private void LoadQuyenTruyCapCacMayChu()
        {
            try
            {
                string sqlQuery = string.Format(@"select * from PhanQuyen_Users PQ_user 
                                                            left join DonViSanXuat dvsx on PQ_user.MaDVSX = dvsx.MaDVSX 
                                                            where Username = '{0}' and host is not null ", useNameLogin);
                userRoleDb = GetDataFromOtherServer.GetData(urlMain, sqlQuery);
                if (userRoleDb == null)
                    MessageBox.Show("Tài khoản người dùng chưa được cấp quyền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    foreach (DataRow row in userRoleDb.Rows)
                        cbxChooseDVSX.Properties.Items.Add(row["TenDVSX"].ToString());
                    cbxChooseDVSX_Bar.EditValue = "Chọn đơn vị sản xuất";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void loadWebView(string BranchId, string brandName)
        {
            try
            {
                chromiumWebBrowser = new ChromiumWebBrowser();
                chromiumWebBrowser.Dock = DockStyle.Fill;
                Controls.Add(chromiumWebBrowser);
                chromiumWebBrowser.ConsoleMessage += ChromiumWebBrowser_ConsoleMessage;
                chromiumWebBrowser.Load(urlWeb);
                LoadUrlAsyncResponse resultStart = Task.Run(
                    async () => { return await chromiumWebBrowser.WaitForInitialLoadAsync(); }
                    ).Result;
                // Execute the script after the page has loaded
                var script = string.Format(@"setBranchIdAndUser('{0}','{1}','{2}');", BranchId, brandName, useNameLogin); // Replace with your actual script
                await chromiumWebBrowser.EvaluateScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ExportExcel(DataTable json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoTienDoTongHopCacKhu_{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BAOCAOTIENDO_SX.xlsx";

                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;

                Export(TemplateFileName, ExportFileName, json);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                DialogResult dr = MessageBox.Show("Mở file?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
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
        public void Export(string TemplateFileName, string ExportFileName, DataTable json)
        {
            try
            {
                string fileName = $"BaoCaoTienDo";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;


                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        package.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        package.Workbook.Properties.Title = "Bao-cao-tien-so-san-xuat";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        string branch = json.Rows[0]["BranchID"].ToString();
                        string storeName = json.Rows[0]["StoredName"].ToString();
                        string sqlPara = json.Rows[0]["SqlParam"].ToString();
                        string SV_api = settingsReader.GetValue(branch, typeof(string)).ToString();
                        string sqlQuery_Str = string.Format(@"EXEC {0} {1}", storeName, sqlPara);
                        DataTable dtExcel = GetDataFromOtherServer.GetData(SV_api, sqlQuery_Str);

                        DateTime currentTime = DateTime.Now;

                        ExcelWorksheet worksheet = package.Workbook.Worksheets["BAOCAOTIENDOSX"];
                        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells.Style.Font.Name = "Times New Roman";

                        int countRow = 11, _oldRow = 0, _row14_21 = 0, _rowMerge = 0, _rowwMerge_MH = 0;
                        string _lineName = string.Empty, _odlLineNameMerge = string.Empty, _oldmaHang = string.Empty; ;
                        for (int i = 0; i < dtExcel.Rows.Count; i++)
                        {
                            if (_oldRow == 0)
                            {
                                _oldRow = countRow;
                            }
                            _lineName = dtExcel.Rows[i]["Name"].ToString();
                            if (_odlLineNameMerge != dtExcel.Rows[i]["Name"].ToString())
                            {
                                _odlLineNameMerge = dtExcel.Rows[i]["Name"].ToString();
                                _rowMerge = countRow;
                                if (_oldmaHang != dtExcel.Rows[i]["MaHang"].ToString())
                                {
                                    _rowwMerge_MH = countRow;
                                    _oldmaHang = dtExcel.Rows[i]["MaHang"].ToString();
                                }
                            }
                            else
                            {
                                worksheet.Cells[_rowMerge, 1, countRow, 1].Merge = true;
                                worksheet.Cells[_rowMerge, 2, countRow, 2].Merge = true;
                                worksheet.Cells[_rowMerge, 3, countRow, 3].Merge = true;

                                if (_oldmaHang != dtExcel.Rows[i]["MaHang"].ToString())
                                {
                                    _rowwMerge_MH = countRow;
                                    _oldmaHang = dtExcel.Rows[i]["MaHang"].ToString();
                                }
                                else
                                {
                                    worksheet.Cells[countRow, 4].Merge = true;
                                }

                            }
                            int cuLK = Convert.ToInt32(dtExcel.Rows[i]["BTP_LK"]);
                            worksheet.Cells[countRow, 1].Value = dtExcel.Rows[i]["Name"];
                            worksheet.Cells[countRow, 2].Value = dtExcel.Rows[i]["NoWorked"];
                            worksheet.Cells[countRow, 3].Value = dtExcel.Rows[i]["HC"];
                            worksheet.Cells[countRow, 4].Value = dtExcel.Rows[i]["MaHang"];
                            worksheet.Cells[countRow, 5].Value = dtExcel.Rows[i]["MaMau"];
                            worksheet.Cells[countRow, 6].Value = dtExcel.Rows[i]["SLKH"];
                            worksheet.Cells[countRow, 7].Value = dtExcel.Rows[i]["SLDLL"];
                            worksheet.Cells[countRow, 8].Value = dtExcel.Rows[i]["SLTT"];
                            worksheet.Cells[countRow, 9].Value = dtExcel.Rows[i]["NgayCat"];
                            worksheet.Cells[countRow, 10].Value = dtExcel.Rows[i]["SLCut_TH"];
                            worksheet.Cells[countRow, 11].Value = dtExcel.Rows[i]["SLCut_LK"];
                            worksheet.Cells[countRow, 12].Value = dtExcel.Rows[i]["NgayBTP"];
                            worksheet.Cells[countRow, 13].Value = dtExcel.Rows[i]["BTP_TH"];
                            worksheet.Cells[countRow, 14].Value = dtExcel.Rows[i]["BTP_LK"];
                            worksheet.Cells[countRow, 15].Value = Convert.ToInt32(dtExcel.Rows[i]["SLCut_LK"]) - Convert.ToInt32(dtExcel.Rows[i]["BTP_LK"]);
                            worksheet.Cells[countRow, 16].Value = dtExcel.Rows[i]["DonGia"];
                            worksheet.Cells[countRow, 17].Value = dtExcel.Rows[i]["RC_HDTH"];
                            worksheet.Cells[countRow, 18].Value = dtExcel.Rows[i]["RC_HDLK"];
                            worksheet.Cells[countRow, 19].Value = dtExcel.Rows[i]["DT_TH"];
                            worksheet.Cells[countRow, 20].Value = dtExcel.Rows[i]["DT_LK"];
                            worksheet.Cells[countRow, 21].Value = dtExcel.Rows[i]["KH_ConLai"];
                            worksheet.Cells[countRow, 22].Value = dtExcel.Rows[i]["NTP_TH"];
                            worksheet.Cells[countRow, 23].Value = dtExcel.Rows[i]["NTP_LK"];
                            worksheet.Cells[countRow, 24].Value = dtExcel.Rows[i]["Ton_TPChuyen"];
                            worksheet.Cells[countRow, 25].Value = dtExcel.Rows[i]["UI_TH"];
                            worksheet.Cells[countRow, 26].Value = dtExcel.Rows[i]["UI_LK"];
                            worksheet.Cells[countRow, 27].Value = dtExcel.Rows[i]["Ton_UI"];
                            worksheet.Cells[countRow, 28].Value = dtExcel.Rows[i]["SU_TH"];
                            worksheet.Cells[countRow, 29].Value = dtExcel.Rows[i]["SU_LK"];
                            worksheet.Cells[countRow, 30].Value = dtExcel.Rows[i]["LKHangDat"];
                            worksheet.Cells[countRow, 31].Value = dtExcel.Rows[i]["TonKiem"];
                            worksheet.Cells[countRow, 32].Value = dtExcel.Rows[i]["HangHu"];
                            worksheet.Cells[countRow, 33].Value = dtExcel.Rows[i]["GX_TH"];
                            worksheet.Cells[countRow, 34].Value = dtExcel.Rows[i]["GX_LK"];
                            worksheet.Cells[countRow, 35].Value = dtExcel.Rows[i]["Ton_TPHT"];
                            worksheet.Cells[countRow, 36].Value = dtExcel.Rows[i]["DT_THHT"];
                            worksheet.Cells[countRow, 37].Value = dtExcel.Rows[i]["DT_LKHT"];
                            worksheet.Cells[countRow, 38].Value = dtExcel.Rows[i]["ChenhLech"];

                            worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 14;

                            countRow++;
                            if (i != dtExcel.Rows.Count - 1)
                            {
                                if (_lineName != dtExcel.Rows[i + 1]["Name"].ToString())
                                {
                                    if (!string.IsNullOrEmpty(_lineName))
                                    {
                                        worksheet.Cells[countRow, 1].Value = "TOTAL";
                                        worksheet.Cells[countRow, 1, countRow, 4].Merge = true;
                                        for (int col = 6; col < 39; col++)
                                        {
                                            if (col != 9 && col != 12)
                                            {
                                                worksheet.Cells[countRow, col].Formula = "=Sum(" + worksheet.Cells[_oldRow, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";
                                            }
                                        }
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Bold = true;
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 12;
                                        Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFBE9");
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                        worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 14;
                                        //worksheet.Cells[countRow, 1, countRow, 37].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                                        countRow++;


                                        _oldRow = 0;
                                    }
                                }
                            }
                            else
                            {
                                worksheet.Cells[countRow, 1].Value = "TOTAL";
                                worksheet.Cells[countRow, 1, countRow, 4].Merge = true;
                                for (int col = 6; col < 39; col++)
                                {
                                    if (col != 9 && col != 12)
                                    {
                                        worksheet.Cells[countRow, col].Formula = "=Sum(" + worksheet.Cells[_oldRow, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";
                                    }
                                }
                                worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Bold = true;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 12;
                                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFBE9");
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 14;
                                countRow++;

                                Color colFromHex1 = System.Drawing.ColorTranslator.FromHtml("#C4DFDF");
                                worksheet.Cells[countRow, 1].Value = "TOTAL " + dtExcel.Rows[0]["Name"].ToString() + " -> " + dtExcel.Rows[i]["Name"].ToString();
                                worksheet.Cells[countRow, 1].Style.Font.Bold = true;
                                worksheet.Cells[countRow, 1].Style.Font.Size = 14;
                                worksheet.Cells[countRow, 1, countRow, 4].Merge = true;

                                worksheet.Cells[countRow, 5].Value = 1;
                                worksheet.Cells[countRow, 5].Style.Font.Color.SetColor(colFromHex1);
                                for (int col = 6; col < 39; col++)
                                {
                                    if (col != 9 && col != 12)
                                    {
                                        worksheet.Cells[countRow, col].Formula = "=SumIF(" + worksheet.Cells[11, 1].Address + ":" + worksheet.Cells[countRow - 1, 5].Address + ",\"TOTAL\"," + worksheet.Cells[11, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";

                                        worksheet.Cells[countRow, col].Style.Font.Bold = true;
                                        worksheet.Cells[countRow, col].Style.Font.Size = 14;

                                        _row14_21 = countRow + 1;
                                    }
                                }

                                worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex1);
                                countRow++;
                                //
                                worksheet.Cells[countRow, 1].Value = "TOTAL " + dtExcel.Rows[0]["Name"].ToString() + " -> " + dtExcel.Rows[i]["Name"].ToString();
                                worksheet.Cells[countRow, 1].Style.Font.Bold = true;
                                worksheet.Cells[countRow, 1].Style.Font.Size = 14;
                                worksheet.Cells[countRow, 1, countRow, 4].Merge = true;
                                for (int col = 6; col < 39; col++)
                                {
                                    if (col != 9 && col != 12)
                                    {
                                        worksheet.Cells[countRow, col].Formula = "=SumIF(" + worksheet.Cells[11, 5].Address + ":" + worksheet.Cells[countRow - 1, 5].Address + ",\"1\"," + worksheet.Cells[11, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";

                                        worksheet.Cells[countRow, col].Style.Font.Bold = true;
                                        worksheet.Cells[countRow, col].Style.Font.Size = 14;

                                    }
                                }
                                worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                Color colFromHex2 = System.Drawing.ColorTranslator.FromHtml("#B3C890");
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex2);

                                _oldRow = 0;
                                countRow++;
                            }

                        }

                        worksheet.Cells[4, 1].Value = "DAILY PRODUCTION REPORT - Date: ";// + fromDate.Day + "-" + fromDate.Month + "-" + fromDate.Year;
                        var border_gc = worksheet.Cells[11, 1, countRow - 1, 38].Style.Border;
                        border_gc.Bottom.Style =
                            border_gc.Top.Style =
                            border_gc.Left.Style =
                            border_gc.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                        FileInfo resultFile = new FileInfo(resultFilePath);
                        package.SaveAs(resultFile);
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
        private void ChromiumWebBrowser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            // e.Message: Nội dung của thông báo
            // e.Source: Nguồn gốc của thông báo (ví dụ: script, console-api)
            // e.Line: Số dòng trong file script
            // e.SourceId: ID của nguồn

            // Ví dụ: nếu bắt được log "ExportExcel"
            Console.WriteLine(e.Message);
            if (e.Message.Contains("ExportExcel") == true)
            {
                try
                {
                    DataTable json = JsonConvert.DeserializeObject<DataTable>(e.Message.Replace("ExportExcel@", ""));
                    ExportExcel(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else if (e.Message == "FULL_RELOAD")
            {
                chromiumWebBrowser.Reload();
            }
        }

        private void cbxChooseDVSX_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cbxChooseDVSX_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            Controls.Remove(chromiumWebBrowser);
            string maDVSXRequest = "";
            string tenDVSXRequest = "";
            foreach (DataRow row in userRoleDb.Rows)
            {
                if (e.NewValue == row["TenDVSX"].ToString())
                {
                    maDVSXRequest = row["MaDVSX"].ToString();
                    tenDVSXRequest = row["TenDVSX"].ToString();
                }
            }
            loadWebView(maDVSXRequest, tenDVSXRequest);
            cbxChooseDVSX_Bar.EditValue = tenDVSXRequest;
        }
    }
}
