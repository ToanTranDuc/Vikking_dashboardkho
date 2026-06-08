using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmBaoCaoKiemTraAnNinh : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;

        private HttpClientExtension _clientExtension;
        public frmBaoCaoKiemTraAnNinh()
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("vi-VN");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
            barEditItem5.EditValue = DateTime.Now;
            LoadData();
        }

        private void barEditItem5_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            string[] ngaythang = Convert.ToDateTime(barEditItem5.EditValue).ToString("MM/yyyy").Split('/');
            string url = $"{URL}BienBanLuuSeal/Get?Action=GetBCAnNinh&para1={ngaythang[0]}&para2={ngaythang[1]}&para3=a&para4=a&para5=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            grcBCAnNinh.DataSource = tbl;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
        private void Xuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataTable dtTable = grcBCAnNinh.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("BaoCaoAnNinh{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BAOCAOKTANNINH.xlsx";
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
        public void Export(string TemplateFileName, string ExportFileName, DataTable lstSoTheoDoiNoiBo)
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
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        excelPackage.Workbook.Properties.Title = "Bao-Cao-An-Ninh";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["Sheet1"];
                      

                        int countRow = 6;
                        ws.InsertRow(6, lstSoTheoDoiNoiBo.Rows.Count + 1);
                        for (int i = 0; i < lstSoTheoDoiNoiBo.Rows.Count; i++)
                        {
                            DataRow item = lstSoTheoDoiNoiBo.Rows[i];

                            ws.Row(countRow).Height = 14;
                            ws.Cells[countRow, 1].Value = i +1;
                            ws.Cells[countRow, 2].Value = item["TenKH"].ToString();
                            ws.Cells[countRow, 3].Value = item["ChungTu"];
                            ws.Cells[countRow, 4].Value = item["Soxe"].ToString();
                            ws.Cells[countRow, 5].Value = item["MaSeal"].ToString();
                            ws.Cells[countRow, 6].Value = item["NgayDH"].ToString();
                            ws.Cells[countRow, 7].Value = item["DuKienTG"].ToString();
                            countRow++;
                        }
                        string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');
                        ws.Cells[$"e{countRow+4}"].Value = $"Ngày {dateTime[0]} tháng {dateTime[1]} năm {dateTime[2]}";
                        var borderData = ws.Cells[6, 1, countRow - 1,7].Style.Border;
                        borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;
                        //// 9: Là số cột
                        //// 7: Là hàng bắt đầu thêm dữ liệu



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

       
    }
}