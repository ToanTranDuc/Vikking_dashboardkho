using BaoCaoTongHop.Models.GetData;
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
    public partial class frmBaoCaoDongThungWeb : Form
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

        public frmBaoCaoDongThungWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCDongThungUrl", typeof(string)).ToString();
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
        public void ExportExcel(RunStoredViewModel json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoDongThung_{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoDongThung.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, RunStoredViewModel objView)
        {
            try
            {
                string fileName = $"BaoCaoDongThung";
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
                        package.Workbook.Properties.Title = "Bao-cao-dong-thung";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        string SV_api = settingsReader.GetValue(objView.server, typeof(string)).ToString();//ConfigurationManager.AppSettings[objView.server].ToString();
                        string SV_DH = settingsReader.GetValue("main", typeof(string)).ToString();

                        string querryGDT_client = createQuerry(objView.stored, objView.paramStr);
                        DataTable dbClient = GetDataFromOtherServer.GetData(SV_api, querryGDT_client);
                        string sqlQuery = createQuerryDH("GetDataDongThungTongQuan", objView.server, objView.paramStr.Replace("@Action = 'GetDSTongQuan', @para0 = '", "").Replace("', @para1 = '',@para2 = '',@para3 = '',@para4 = ''", ""));
                        DataTable dbServer = GetDataFromOtherServer.GetData(SV_DH, sqlQuery);
                        if (dbServer != null)
                            for (int i = 0; i < dbClient.Rows.Count; i++)
                            {
                                int tongDT = 0, tongNK = 0;
                                for (int j = 0; j < dbServer.Rows.Count; j++)
                                {
                                    if (dbServer.Rows[j]["SPOID"].ToString() == dbClient.Rows[i]["SPOID"].ToString())
                                    {
                                        tongDT += Convert.ToInt32(dbServer.Rows[j]["NhanDT"].ToString());
                                        tongNK += Convert.ToInt32(dbServer.Rows[j]["NhanNK"].ToString());
                                    }
                                }
                                dbClient.Rows[i]["NhanDT"] = tongDT;
                                dbClient.Rows[i]["NhanNK"] = tongNK;
                            }
                        DataTable db = dbClient;

                        int slLenh = db.Rows.Count;

                        db.Columns.Add("ConLaiTP");
                        db.Columns.Add("ConLaiDT");
                        db.Columns.Add("ConLaiNK");
                        for (int i = 0; i < db.Rows.Count; i++)
                        {
                            db.Rows[i]["ConLaiTP"] = Convert.ToInt32(db.Rows[i]["SoLuong"].ToString()) - Convert.ToInt32(db.Rows[i]["NhanTP"].ToString());
                            db.Rows[i]["ConLaiDT"] = Convert.ToInt32(db.Rows[i]["SoLuong"].ToString()) - Convert.ToInt32(db.Rows[i]["NhanDT"].ToString());
                            db.Rows[i]["ConLaiNK"] = Convert.ToInt32(db.Rows[i]["SoLuong"].ToString()) - Convert.ToInt32(db.Rows[i]["NhanNK"].ToString());
                        }
                        string malenhSS = db.Rows[0]["MaLenh"].ToString();
                        for (int i = 0; i < db.Rows.Count; i++)
                        {
                            string maLenhIndex = db.Rows[i]["MaLenh"].ToString();
                            if (malenhSS != maLenhIndex)
                            {
                                DataRow row = db.NewRow();
                                row["MaLenh"] = malenhSS;
                                row["NgayGH"] = "Sum";

                                int SLTong = SumColumn(db, malenhSS, "SoLuong");
                                int SLNhanTP = SumColumn(db, malenhSS, "NhanTP");
                                int SLNhanDT = SumColumn(db, malenhSS, "NhanDT");
                                int SLNhanNK = SumColumn(db, malenhSS, "NhanNK");

                                row["SoLuong"] = SLTong;

                                row["NhanTP"] = SLNhanTP;
                                row["NhanDT"] = SLNhanDT;
                                row["NhanNK"] = SLNhanNK;

                                row["ConLaiTP"] = SLTong - SLNhanTP;
                                row["ConLaiDT"] = SLTong - SLNhanDT;
                                row["ConLaiNK"] = SLTong - SLNhanNK;

                                db.Rows.InsertAt(row, i);

                                malenhSS = maLenhIndex;
                            }
                        }
                        DataRow _row = db.NewRow();
                        _row["MaLenh"] = malenhSS;
                        _row["NgayGH"] = "Sum";

                        int _SLTong = SumColumn(db, malenhSS, "SoLuong");
                        int _SLNhanTP = SumColumn(db, malenhSS, "NhanTP");
                        int _SLNhanDT = SumColumn(db, malenhSS, "NhanDT");
                        int _SLNhanNK = SumColumn(db, malenhSS, "NhanNK");

                        _row["SoLuong"] = _SLTong;

                        _row["NhanTP"] = _SLNhanTP;
                        _row["NhanDT"] = _SLNhanDT;
                        _row["NhanNK"] = _SLNhanNK;

                        _row["ConLaiTP"] = _SLTong - _SLNhanTP;
                        _row["ConLaiDT"] = _SLTong - _SLNhanDT;
                        _row["ConLaiNK"] = _SLTong - _SLNhanNK;

                        db.Rows.InsertAt(_row, db.Rows.Count);

                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Report"];
                        worksheet.Cells.Style.Font.Name = "Times New Roman";

                        worksheet.Cells["D2"].Value = slLenh;
                        int index = 5;
                        for (int i = 0; i < db.Rows.Count; i++)// +6
                        {
                            index++;
                            if (db.Rows[i]["NgayGH"].ToString().ToLower() == "sum")
                            {
                                worksheet.Cells["A" + index + ":F" + index].Merge = true;
                                worksheet.Cells["A" + index + ":F" + index].Value = string.Format(@"Tổng lệnh {0} (Mã hàng: {1})",
                                     db.Rows[i]["MaLenh"].ToString(), db.Rows[i - 1]["MaHang"].ToString()
                                    );
                                worksheet.Cells["G" + index].Value = db.Rows[i]["SoLuong"].ToString();
                                worksheet.Cells["H" + index].Value = db.Rows[i]["NhanTP"].ToString();
                                worksheet.Cells["I" + index].Value = db.Rows[i]["ConLaiTP"].ToString();
                                worksheet.Cells["J" + index].Value = db.Rows[i]["NhanDT"].ToString();
                                worksheet.Cells["K" + index].Value = db.Rows[i]["ConLaiDT"].ToString();
                                worksheet.Cells["L" + index].Value = db.Rows[i]["NhanNK"].ToString();
                                worksheet.Cells["M" + index].Value = db.Rows[i]["ConLaiNK"].ToString();
                            }
                            else
                            {
                                worksheet.Cells["A" + index].Value = db.Rows[i]["MaLenh"].ToString();
                                worksheet.Cells["B" + index].Value = db.Rows[i]["PO"].ToString();
                                worksheet.Cells["C" + index].Value = db.Rows[i]["MaHang"].ToString();
                                worksheet.Cells["D" + index].Value = db.Rows[i]["KhachHang"].ToString();
                                worksheet.Cells["E" + index].Value = db.Rows[i]["QuocGia"].ToString();
                                worksheet.Cells["F" + index].Value = db.Rows[i]["NgayGH"].ToString();
                                worksheet.Cells["G" + index].Value = db.Rows[i]["SoLuong"].ToString();
                                worksheet.Cells["H" + index].Value = db.Rows[i]["NhanTP"].ToString();
                                worksheet.Cells["I" + index].Value = db.Rows[i]["ConLaiTP"].ToString();
                                worksheet.Cells["J" + index].Value = db.Rows[i]["NhanDT"].ToString();
                                worksheet.Cells["K" + index].Value = db.Rows[i]["ConLaiDT"].ToString();
                                worksheet.Cells["L" + index].Value = db.Rows[i]["NhanNK"].ToString();
                                worksheet.Cells["M" + index].Value = db.Rows[i]["ConLaiNK"].ToString();
                            }
                        }
                        // VẼ LẠI GIAO DIỆN
                        worksheet.Cells["A4:M" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A4:M" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A4:M" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells["A4:M" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        // G H I J K L M
                        worksheet.Cells["H6:M" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["H6:M" + index].Style.Fill.BackgroundColor.SetColor(Color.White); // Màu nền
                        worksheet.Cells["H6:M" + index].Style.Font.Color.SetColor(Color.DarkGreen); // Màu chữ

                        worksheet.Cells["G6:G" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["G6:G" + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                        worksheet.Cells["G6:G" + index].Style.Font.Bold = true; // Màu chữ

                        worksheet.Cells["I6:I" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["I6:I" + index].Style.Fill.BackgroundColor.SetColor(Color.White);// Màu nền
                        worksheet.Cells["I6:I" + index].Style.Font.Color.SetColor(Color.Red); // Màu chữ
                        worksheet.Cells["K6:K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["K6:K" + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                        worksheet.Cells["K6:K" + index].Style.Font.Color.SetColor(Color.Red); // Màu chữ
                        worksheet.Cells["M6:M" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["M6:M" + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                        worksheet.Cells["M6:M" + index].Style.Font.Color.SetColor(Color.Red); // Màu chữ

                        for (int i = 6; i <= index; i++)
                        {
                            if (worksheet.Cells[string.Format(@"A{0}:F{0}", i)].Merge == true)
                            {
                                worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 242, 204)); // Màu nền
                                worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Font.Color.SetColor(Color.Red); // Màu chữ
                                worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Font.Bold = true;
                            }
                        }
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
                    RunStoredViewModel json = JsonConvert.DeserializeObject<RunStoredViewModel>(e.Message.Replace("ExportExcel@", ""));
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
        public string createQuerry(string stored, string sqlPara)
        {
            string result = string.Format(@"EXEC {0} {1}", stored, sqlPara);
            return result;
        }
        public string createQuerryDH(string action, string dvsx, string para)
        {
            string result = string.Format(@"EXEC SP_SendDataDongThungToClient @Action = '{0}', @maDVSX = '{1}' , @sqlPara = '{2}'", action, dvsx, para);
            return result;
        }
        //===================================================================
        private int SumColumn(DataTable db, string maLenh, string fieldName)
        {
            int result = 0;
            foreach (DataRow row in db.Rows)
            {
                if (row["MaLenh"].ToString() == maLenh)
                    result += Convert.ToInt32(row[fieldName].ToString());
            }
            return result;
        }
    }
}
