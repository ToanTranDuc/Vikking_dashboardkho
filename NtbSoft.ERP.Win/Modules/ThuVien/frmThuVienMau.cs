using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Json;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmThuVienMau : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        List<ThuVienMauEntity> lstMau;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        DataTable dttb;
        DataTable dtKH;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public frmThuVienMau()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstMau = new List<ThuVienMauEntity>();

            dtKH = new DataTable();
            loadKH();
        }
        private void loadKH()
        {
            searchLookUpEdit1.Properties.ValueMember = "MaKH";
            searchLookUpEdit1.Properties.DisplayMember = "MaKH";
            searchLookUpEdit1.Properties.NullText = "";
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            dtKH = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = dtKH;
            searchLookUpEdit1.RefreshEditValue();
            searchLookUpEdit1.Refresh();

        }

        private void searchlookupKH()
        {
            string url1 = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            DataTable dtbkh = JsonConvert.DeserializeObject<DataTable>(json1);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = dtbkh;
            rCountryEdit.DisplayMember = "TenKH";
            rCountryEdit.ValueMember = "TenKH";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";
            //rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            //rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã KH", Name = "colMaMau", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên Khách Hàng", Name = "colTenMau", Visible = true });

            }
            colTenKH.ColumnEdit = rCountryEdit;
            rCountryEdit.EditValueChanged += RCountryEdit_EditValueChanged;

        }
        private void RCountryEdit_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            GridView gridView = gCThuVienMau.FocusedView as GridView;
            if (gridView == null) return;

            // Lấy hàng hiện tại
            int rowHandle = gridView.FocusedRowHandle;
            if (rowHandle < 0) return;

            // Lấy giá trị MaKH vừa chọn
            string selectedMaKH = edit.EditValue?.ToString();
            if (string.IsNullOrEmpty(selectedMaKH)) return;

            // Lấy dữ liệu từ nguồn của SearchLookUpEdit
            DataTable dtbkh = edit.Properties.DataSource as DataTable;
            if (dtbkh == null) return;

            // Tìm TenKH tương ứng với MaKH
            DataRow[] selectedRow = dtbkh.Select($"TenKH = '{selectedMaKH}'");
            if (selectedRow.Length > 0)
            {
                string MaKH = selectedRow[0]["MaKH"].ToString();

                // Gán giá trị vào cột TenKH của hàng hiện tại
                gridView.SetRowCellValue(rowHandle, "MaKH", MaKH);
            }
        }
        private void ThemDong()
        {
            if(lstMau==null)
            {
                lstMau = new List<ThuVienMauEntity>();
            }    
            textID.Text = "0";
            txtMaKH.Text = "";
            txtTenKH.Text = "";
            txtCodeMau.Text = "";
            txtTenMau.Text = "";
            txtCodeKhac.Text = "";
            txtGhiChu.Text = "";
            ThuVienMauEntity obj = new ThuVienMauEntity();
            obj.ID = 0;
            obj.isNew = true;
            lstMau.Insert(0, obj);
            gVThuVienMau.FocusedRowHandle = 0;
            gCThuVienMau.DataSource = lstMau;
            gCThuVienMau.RefreshDataSource();
            gVThuVienMau.MakeRowVisible(0, true);
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = btn1;
                if (ValidateDataInsert())
                {
                    return;
                }    
               
                List<ThuVienMauEntity> _lstUpdate = gCThuVienMau.DataSource as List<ThuVienMauEntity>;

                for(int i=0;i< _lstUpdate.Count;i++)
                {
                    _lstUpdate[i].MaMauKH = ReplaceSpecialCharactersAndRemoveSpaces(_lstUpdate[i].MaKH + _lstUpdate[i].CodeMauKH);
                    if(_lstUpdate[i].CodeKhac ==null)
                    {
                        _lstUpdate[i].CodeKhac = "";
                    }
                    if (_lstUpdate[i].GhiChu == null)
                    {
                        _lstUpdate[i].GhiChu = "";
                    }
                }    
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
              
                string url = URL + "ThuVienMau/Post";
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                if (msResult.ToLower() == "true")
                {
                    LoadMau(false);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                  
                }
                
                lstRowUpdate.Clear();

               

            }
            catch (Exception ex)
            {
               /* XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
            }
        }
        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gVThuVienMau.FocusedRowHandle >= 0)
                    {
                        int ID = Int32.Parse(gVThuVienMau.GetFocusedRowCellValue(colID)?.ToString());
                        string url = URL + $"ThuVienMau/Delete?ID={ID}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadMau(false);
                        else XtraMessageBox.Show(result);
                    }
                    else { return; }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
       
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }

        private void NapLaiDong()
        {
            searchLookUpEdit1.EditValue = "";
            textID.Text = "0";
            txtMaKH.Text = "";
            txtTenKH.Text = "";
            txtCodeMau.Text = "";
            txtTenMau.Text = "";
            txtCodeKhac.Text = "";
            txtGhiChu.Text = "";
            LoadMau(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            
            LoadMau(false);
        }
     
        private void loadThongTinMau(int ID)
        {
            if (dttb != null)
            {
                
                ThuVienMauEntity result = lstMau.Find(nl => nl.ID == ID);
                
                textID.Text = ID.ToString();
                searchLookUpEdit1.EditValue = result.MaKH?.ToString()!=""? result.MaKH?.ToString() : "";
                txtTenKH.Text = result.TenKH;
                txtCodeMau.Text = result.CodeMauKH;
                txtTenMau.Text = result.TenMauKH;
                txtCodeKhac.Text = result.CodeKhac;
                txtGhiChu.Text = result.GhiChu;

            }
        }

        private void LoadMau(bool isFromSave)
        {
            try
            {
                clsWaitForm.ShowWaitForm(this, 1000);
                string url = string.Format("{0}", URL + $"ThuVienMau/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if(json=="[]")
                {
                    dttb = null;
                    lstMau = null;
                    gCThuVienMau.DataSource = null;
                    gCThuVienMau.RefreshDataSource();
                    btnXuatExcel.Enabled = false;
                    searchlookupKH();
                    searchLookUpEdit1.EditValue = "";
                    textID.Text = "0";
                    txtMaKH.Text = "";
                    txtTenKH.Text = "";
                    txtCodeMau.Text = "";
                    txtTenMau.Text = "";
                    txtCodeKhac.Text = "";
                    txtGhiChu.Text = "";
                    return;
                }
                if (!string.IsNullOrEmpty(json))
                {
                    dttb = JsonConvert.DeserializeObject<DataTable>(json);
                    lstMau = JsonConvert.DeserializeObject<List<ThuVienMauEntity>>(json);
                    loadThongTinMau(lstMau[0].ID);
                    
                }


              
                gCThuVienMau.DataSource = lstMau.Count > 0 ? lstMau : null;
                searchlookupKH();
                gCThuVienMau.RefreshDataSource();
         
                if (isFromSave && FocusedIndex >= 0)
                {
                  
                    gVThuVienMau.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gCThuVienMau;
                }
                
                Them.Enabled = true;

            }
            catch(Exception e)
            {
                //XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng kiểm tra lại hoặc nhập thêm dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #region excel
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                ReadExcelPL(Sfd.FileName);
            }
        }
        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            ReadExcelPL(Sfd.FileName);
        }

        private void ReadExcelPL(string FilePath)
        {

            try
            {
                string worksheetName = string.Empty;
                //lstMau.Clear();
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A3:ZZ500");
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSave = new DataTable();
                dtSave = source.ToDataTable();
                AddList(dtSave);
            }

            catch (Exception ex)
            {

                throw new Exception();
            }

        }

        private void AddList(DataTable dtSave)
        {

            string _tenKH = string.Empty, _codeMau = string.Empty, _tenMau = string.Empty, _codeKhac = string.Empty, _ghiChu = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}", URL + $"ThuVienMau/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstMau = JsonConvert.DeserializeObject<List<ThuVienMauEntity>>(json);
            }
            else if (json == "[]")
            {
                lstMau = new List<ThuVienMauEntity>();
                lstMau.Clear();
            }
            bool ishasData = false;
            foreach (DataRow row in dtSave.Rows)
            {
                _tenKH = row[1].ToString();
                _codeMau = row[2].ToString();
                _tenMau = row[3].ToString();
                _codeKhac = row[4].ToString();
                _ghiChu = row[5].ToString();
                if (string.IsNullOrWhiteSpace(_tenKH) && string.IsNullOrWhiteSpace(_codeMau) && string.IsNullOrWhiteSpace(_tenMau))
                {
                    continue;
                }
                else
                {
                    ishasData = true;
                }
                bool IsNPL = lstMau.Any(x => x.TenKH == _tenKH && x.CodeMauKH == _codeMau);
                if (IsNPL)
                {
                    XtraMessageBox.Show($"Trùng code màu: {_codeMau}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    lstMau.Clear();
                    lstMau.Add(new ThuVienMauEntity
                    {
                        ID = 0,
                        MaMauKH = "",
                        MaKH = "",
                        CodeMauKH = _codeMau,
                        TenMauKH = _tenMau,
                        CodeKhac = _codeMau,
                        GhiChu = _ghiChu,
                        TenKH= _tenKH
                    });
                    string msResult = "";
                    string url1 = URL + "ThuVienMau/POSTEXCEL";
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url1, lstMau); }).Result;
                    LoadMau(false);
                }
            }
            if (!ishasData)
            {
                XtraMessageBox.Show($"File Excel không có dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ExportExcel(string path, DataTable dtsave)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ThuVienMau");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["C1:F1"]; range.Merge = true; range.Value = "CÔNG TY CỔ PHẦN TỔNG CÔNG TY MAY ĐỒNG NAI"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["C2:F3"]; range.Merge = true; range.Value = "THƯ VIỆN MÀU"; range.Style.Font.Bold = true;
                    dtXuatExcelTVMau(dtsave, worksheet);
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        public void dtXuatExcelTVMau(DataTable dtSave, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            string[] headers = { "STT", "Mã khách hàng", "Tên khách hàng", "Code màu", "Tên màu", "Code khác", "Ghi chú" };

            // Thêm tiêu đề vào Excel
            for (int col = 0; col < headers.Length; col++)
            {
                int colIndex = col + 1;
                range = worksheet.Cells[4, colIndex];
                range.Value = headers[col];
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            string LoGo = KHDongThungLib.getImgPath("DongNai.jpg");
            Image image = Image.FromFile(LoGo);
            OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
            picture.SetPosition(0, 0, 0, 0);
            picture.SetSize(105, 55);

            int row = 5;
            int index = 0;


            int[] maxColumnWidths = new int[headers.Length];


            foreach (DataRow rows in dtSave.Rows)
            {
                worksheet.Cells[row, 1].Value = index + 1; // STT
                worksheet.Cells[row, 2].Value = rows["MaKH"].ToString(); 
                worksheet.Cells[row, 3].Value = rows["TenKH"].ToString();
                worksheet.Cells[row, 4].Value = rows["CodeMauKH"].ToString(); 
                worksheet.Cells[row, 5].Value = rows["TenMauKH"].ToString(); 
                worksheet.Cells[row, 6].Value = rows["CodeKhac"].ToString(); 
                worksheet.Cells[row, 7].Value = rows["GhiChu"].ToString(); 


                string[] rowData = {
                (index + 1).ToString(),
                rows["MaKH"].ToString(),
                rows["TenKH"].ToString(),
                rows["CodeMauKH"].ToString(),
                rows["TenMauKH"].ToString(),
                rows["CodeKhac"].ToString(),
                rows["GhiChu"].ToString()};

                for (int col = 0; col < headers.Length; col++)
                {
                    maxColumnWidths[col] = Math.Max(maxColumnWidths[col], rowData[col].Length);
                }

   
                if (index % 2 == 0)
                {
                    worksheet.Cells[row, 1, row, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                }
                else
                {
                    worksheet.Cells[row, 1, row, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }

                worksheet.Cells[row, 1, row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                index++;
                row++;
            }
            for (int col = 0; col < headers.Length; col++)
            {
                int colIndex = col + 1;
                int headerWidth = headers[col].Length;
                int maxWidth = Math.Max(headerWidth, maxColumnWidths[col]) + 4;
                worksheet.Column(colIndex).Width = maxWidth;
            }

            // Thêm border cho toàn bộ bảng
            int totalRows = dtSave.Rows.Count + 4;
            var tableRange = worksheet.Cells[4, 1, totalRows, 7]; // Từ A4 -> G cuối cùng
            tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("NhapThuVienMau{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateThuVienMau.xlsx";
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
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        excelPackage.Workbook.Properties.Author = "NTB";

                        // đặt tiêu đề cho file
                        excelPackage.Workbook.Properties.Title = "ThuVienMau";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);


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
        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string mavattu = string.Empty;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (dttb is null || dttb.Rows.Count == 0) return;
            //mavattu = dttb.AsEnumerable().Where(x => x["MaVatTu"] == mavattu.ToString()).FirstOrDefault()["MaVatTu"].ToString();
            //int Size = 0;
            Sfd.FileName = string.Format("ThuVienMau");
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("Bao Cao");
                    ExportExcel(Sfd.FileName, dttb);
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

        #endregion


        private void gVThuVienMau_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
           
            
        }

        private void NapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

       
        private void gVThuVienMau_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEdit1View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVThuVienMau_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            int focusedRowHandle = e.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
              
                var focusedRowData = gVThuVienMau.GetRow(focusedRowHandle) as ThuVienMauEntity;


                loadThongTinMau(focusedRowData.ID);
                btnXuatExcel.Enabled = true;
                Xoa.Enabled = true;
               
                NapLai.Enabled = true;
            }
        }
        private string ReplaceSpecialCharactersAndRemoveSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

           
            input = Regex.Replace(input, @"\s+", "");

            // Thay thế tất cả các ký tự không phải chữ cái và số bằng ký tự '_'
            string result = Regex.Replace(input, @"[^a-zA-Z0-9]", "_");
            return result;
        }

        private bool ValidateDataInsert()
        {
            List<ThuVienMauEntity> lstDataSource = gVThuVienMau.DataSource as List<ThuVienMauEntity>;

            if (lstDataSource == null || lstDataSource.Count <= 1)
            {
                return false; 
            }

            HashSet<string> checkSet = new HashSet<string>();

            foreach (var item in lstDataSource)
            {
                if(item.isNew)
                {
                    if (string.IsNullOrWhiteSpace(item.CodeMauKH) || string.IsNullOrWhiteSpace(item.MaKH) || string.IsNullOrWhiteSpace(item.TenMauKH))
                    {
                        XtraMessageBox.Show("Vui lòng điền đủ thông tin và thử lại!!!",
                                           "Cảnh báo",
                                           MessageBoxButtons.OK,
                                           MessageBoxIcon.Warning);
                        return true;
                    }

                }
                string uniqueKey = item.CodeMauKH + "|" + item.TenMauKH+"|" + item.MaKH;
                if (checkSet.Contains(uniqueKey))

                {
                    XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại!"+ item.CodeMauKH+"   hehe    "+ item.TenMauKH+"   hehe    " + item.MaKH,
                                        "Cảnh báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                    return true; 
                }

                checkSet.Add(uniqueKey);
            }

            return false; 
        }
    }
}