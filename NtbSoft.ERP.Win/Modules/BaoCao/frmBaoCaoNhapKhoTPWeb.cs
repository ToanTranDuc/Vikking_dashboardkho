using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class frmBaoCaoNhapKhoTPWeb : Form
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

        public frmBaoCaoNhapKhoTPWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCNKThanhPhamUrl", typeof(string)).ToString();
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
        public void ExportExcel(JObject json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoNhapKhoThanhPham_{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoNhapKhoTP.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, JObject Param)
        {
            try
            {
                string fileName = $"BaoCaoNhapKhoThanhPham";
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
                        package.Workbook.Properties.Title = "Bao-cao-nhap-kho-thanh-pham";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        string server = string.Empty;
                        string sql = string.Empty;
                        int Row = 6; int StartRow = 6;
                        object MergeMH = null; DateTime parsedDate = new DateTime();
                        int rowSpan = 0; int idx = 0;
                        string strDate = string.Empty;
                        string SV_api = settingsReader.GetValue("main", typeof(string)).ToString();

                        sql = $"EXEC SP_DB_BaoCaoNhapKhoTP @Action = 'Get', @Para1 = '{Param["FromDate"]?.ToString()}', @Para2= '{Param["ToDate"]?.ToString()}', @Para3 = '{Param["BranchID"]?.ToString()}', @Para4 = '', @Para5 = ''";
                        DataTable dt = GetDataFromOtherServer.GetData(SV_api, sql);
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                            worksheet.Cells.Style.Font.Name = "Times New Roman";
                            if (dt.Rows.Count > 0)
                            {
                                try
                                {
                                    worksheet.Cells["D3"].Value = DateTime.ParseExact(Param["FromDate"]?.ToString(), "yyyy-MM-dd", null).ToString("dd-MM-yyyy");
                                    worksheet.Cells["D4"].Value = DateTime.ParseExact(Param["ToDate"]?.ToString(), "yyyy-MM-dd", null).ToString("dd-MM-yyyy");

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception();
                                }


                                foreach (DataRow row in dt.Rows)
                                {
                                    Row++;
                                    object currentMH = row["MaHang"];
                                    MergeMH = (idx == dt.Rows.Count - 1) ? "" : dt.Rows[idx + 1]["MaHang"].ToString();
                                    bool isMHMearge = currentMH.ToString() != MergeMH.ToString();

                                    if (!isMHMearge)
                                    {
                                        worksheet.Cells[Row - rowSpan, 1, Row, 1].Merge = true;
                                        worksheet.Cells[Row - rowSpan, 1, Row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        worksheet.Cells[Row - rowSpan, 1, Row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                        rowSpan = 0;
                                    }
                                    else
                                    {
                                        rowSpan++;
                                    }

                                    if (DateTime.TryParse(row["NgayNhapKho"]?.ToString(), out parsedDate))
                                    {


                                        strDate = parsedDate.ToString("dd-MM-yyyy");

                                    }
                                    else
                                    {
                                        strDate = "";
                                    }

                                    worksheet.Cells[Row, 1].Value = row["MaHang"]?.ToString();
                                    worksheet.Cells[Row, 2].Value = row["Dot"]?.ToString();
                                    worksheet.Cells[Row, 3].Value = strDate;
                                    worksheet.Cells[Row, 4].Value = Convert.ToInt32(row["SLKH"]?.ToString());
                                    worksheet.Cells[Row, 5].Value = Convert.ToInt32(row["SLSPNK"]?.ToString());
                                    worksheet.Cells[Row, 6].Value = Convert.ToInt32(row["SoThung"]?.ToString());

                                    //worksheet.Cells[Row, 2].Style.WrapText = true;
                                    idx++;
                                }
                            }


                            var range = worksheet.Cells[$"A7:F{Row}"];
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[$"B7:C{Row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[$"D7:F{Row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ///
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
                    JObject json = JsonConvert.DeserializeObject<JObject>(e.Message.Replace("ExportExcel@", ""));
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
