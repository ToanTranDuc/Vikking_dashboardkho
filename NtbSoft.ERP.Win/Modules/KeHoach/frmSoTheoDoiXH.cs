using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmSoTheoDoiXH : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _tuNgay = string.Empty, _denNgay = string.Empty, _maDH = string.Empty, _ngayThang = string.Empty, _maKH = string.Empty, _dvTinh = string.Empty, _donhang = string.Empty, donhangtong = string.Empty, maPKL = string.Empty;

        private void NapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TuNgay.EditValue = DateTime.Now;
            DenNgay.EditValue = DateTime.Now;
            searchLookUpEdit11.EditValue = null;
            LoadDS();
        }

        int _soToNo = 0, cbLocValue = 1;

       

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDS();
        }

        int _totalSLTon = 0, totalNhap = 0, totalXuat = 0;
        string _dvsx = string.Empty;
        bool isCheckFirst = false, _isCheckHuy = true, isCoKhacHang = false;
        private HttpClientExtension _clientExtension;
        public frmSoTheoDoiXH()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CreateDefault();
            loadTenKho();
            TuNgay.EditValue = DateTime.Now;
            DenNgay.EditValue = DateTime.Now;
            isCheckFirst = true;
            LoadDS();
        }
        public void loadTenKho()
        {
            string url = $"{URL}BienBanLuuSeal/Get?Action=GetTenKho&para1=a&para2=a&para3=a&para4=a&para5=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit11.Properties.DataSource = tbl;
        }
        private void TuNgay_EditValueChanged(object sender, EventArgs e)
        {
            _tuNgay = Convert.ToDateTime(TuNgay.EditValue).ToString("yyyy-MM-dd");
            if (!isCheckFirst)
                return;
        }
        private void DenNgay_EditValueChanged(object sender, EventArgs e)
        {
            _denNgay = Convert.ToDateTime(DenNgay.EditValue).ToString("yyyy-MM-dd");
            if (!isCheckFirst)
                return;
        }
        private void CreateDefault()
        {
            searchLookUpEdit11.Properties.ValueMember = "MaDVSX";
            searchLookUpEdit11.Properties.DisplayMember = "TenDVSX";
            searchLookUpEdit11.Properties.NullText = "[Chọn Kho]";
        }
        private void LoadDS()
        {

            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                _denNgay = Convert.ToDateTime(DenNgay.EditValue).ToString("yyyy-MM-dd");
                _tuNgay = Convert.ToDateTime(TuNgay.EditValue).ToString("yyyy-MM-dd");
                string tenkho = searchLookUpEdit11.EditValue ==null ? "All" : searchLookUpEdit11.EditValue.ToString();
                string userName = GlobleData.UserName;
                string url = $"{URL}SoTheoDoiXuatHangController/Get?Action=GetXuathang&Para1={tenkho}&Para2={_tuNgay}&Para3={_denNgay}&Para4=a";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcSoTheoDoi.DataSource = null;
                }
                else
                {
                 

                    grcSoTheoDoi.DataSource = tbl;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void Xuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dtTable = grcSoTheoDoi.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("SoTheoDoiXuatHang{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "SoTheoDoiHangNgay.xlsx";
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

        public void Export(string TemplateFileName, string ExportFileName, DataTable dtTable)
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
                        // đặt tên người tạo file
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        excelPackage.Workbook.Properties.Title = "Bao-Cao-Tien-Do-San-Xuat";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                        ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets["Sheet1"];
                        worksheet.Cells.Style.Font.Name = "Times New Roman";


                        int row = 6;

                        foreach (DataRow item in dtTable.Rows)
                        {
                            worksheet.Cells[row, 1].Value = Convert.ToDateTime(item["NgayGioKT"]).ToString("dd/MM/yyyy");
                            worksheet.Cells[row, 2].Value = item["TenBienBan"];
                            worksheet.Cells[row, 3].Value = item["KhachHang"];
                            worksheet.Cells[row, 4].Value = item["MaHang"];
                            worksheet.Cells[row, 5].Value = item["SLThung"];
                            worksheet.Cells[row, 6].Value = item["CangDen"];
                            worksheet.Cells[row, 7].Value = item["Display"];
                            row++;
                        }
                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        var borderData = worksheet.Cells[6, 1, row, 7].Style.Border;
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
    }
}