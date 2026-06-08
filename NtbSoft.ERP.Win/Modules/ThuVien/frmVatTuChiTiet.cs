using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
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
    public partial class frmVatTuChiTiet : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        List<VatTuChiTietEntity> lstVatTuCT;
        List<int> lstRowUpdate = new List<int>();

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        DataTable dttb;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool IsVal = false;
        int FocusedIndex = 0;
        bool isThem = false;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        public frmVatTuChiTiet()
        {
            InitializeComponent();
            
            URL = (string)settingsReader.GetValue("URL", typeof(String));
           
            _clientExtension = new HttpClientExtension();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            lstVatTuCT = new List<VatTuChiTietEntity>();
        }
        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            LoadKhoSize(false);
            //Xoa.Enabled = true;
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);//SystemUser/GetPer/userID=...
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                //Luu.Enabled = false;
            }
            //else
            //{
            //    Luu.Enabled = false;
            //}
            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, true);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.NapLai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private void gridControl1_Click(object sender, EventArgs e)
        {

        }


        private void ThemDong()
        {
            isThem = true;
            if(lstVatTuCT == null)
            {
                lstVatTuCT = new List<VatTuChiTietEntity>();
            }    
            //textID.Text = "0";
          
            //txtTenVatTu.Text = "";
            //txtMaVT.Text = "";
            VatTuChiTietEntity obj = new VatTuChiTietEntity();
            obj.ID = 0;
            //obj.isNew = true;
            lstVatTuCT.Insert(0, obj);
            lstVatTuCT.Add(obj);
            _rowAdd = gVVatTuChiTiet.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            GridViewUpdateStatus(_status);
            gVVatTuChiTiet.FocusedRowHandle = 0;
            gCVatTuChiTiet.DataSource = lstVatTuCT;
            gCVatTuChiTiet.RefreshDataSource();
            //txtMaVT.Properties.ReadOnly = false;
            //txtTenVatTu.Properties.ReadOnly = false;
            
        }

     

        private void LuuDong()
        {
           
            try
            {
                this.ActiveControl = this.Button1;
                //  if (string.IsNullOrWhiteSpace(txtMaVT.Text) ||
                //string.IsNullOrWhiteSpace(txtTenVatTu.Text))
                //  {
                //      XtraMessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //      return;
                //  }
                //  VatTuChiTietEntity npl = new VatTuChiTietEntity
                //  {
                //      ID = int.TryParse(textID.Text, out int id) ? id : 0,
                //      MaVT="",
                //      TenVT=txtTenVatTu.Text,
                //      MaHienThi=txtMaVT.Text
                //  };

                if (gVVatTuChiTiet.FocusedRowHandle >=0) 
                {
                    FocusedIndex = gVVatTuChiTiet.FocusedRowHandle;
                }

                List<VatTuChiTietEntity> _lstUpdate = new List<VatTuChiTietEntity> {};

                VatTuChiTietEntity npl = gVVatTuChiTiet.GetRow(gVVatTuChiTiet.FocusedRowHandle) as VatTuChiTietEntity;
                bool isDup = ValidateDataInsert(npl);
                if(isDup)
                {
                    return;
                }
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstRowUpdate.Count; i++)
                {
                    VatTuChiTietEntity item = gVVatTuChiTiet.GetRow(lstRowUpdate[i]) as VatTuChiTietEntity;
                    if (item != null && !string.IsNullOrEmpty(item.TenVT))
                    {
                        _lstUpdate.Add(item);

                    }
                    else
                    {
                        XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }

                string url = URL + "VatTuChiTiet/Post";
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                if (msResult.ToLower() == "true")
                {
                    isThem = false;
                    LoadKhoSize(false);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                lstRowUpdate.Clear();
                Them.Enabled = true;
                Sua.Enabled = true;
                //Luu.Enabled = false;

                _status = ResourceURL.EventStatus.View;

                GridViewUpdateStatus(_status);

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
     
        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gVVatTuChiTiet.FocusedRowHandle >= 0)
                    {
                        int ID = Int32.Parse(gVVatTuChiTiet.GetFocusedRowCellValue(colID)?.ToString());
                        string url = URL + $"VatTuChiTiet/Delete?ID={ID}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                            LoadKhoSize(false);
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;

                }

                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A4:ZZ500");
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
            
            string _maVT = string.Empty, _tenVT = string.Empty;
            int lastIdxCol = dtSave.Columns.Count - 1;
            string url = string.Format("{0}", URL + $"VatTuChiTiet/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                lstVatTuCT = JsonConvert.DeserializeObject<List<VatTuChiTietEntity>>(json);
            }
            else if (json == "[]")
            {
                lstVatTuCT = new List<VatTuChiTietEntity>();
                lstVatTuCT.Clear();
            }
            bool ishasData = false;
            foreach (DataRow row in dtSave.Rows)
            {
                _maVT = row[1].ToString();
                _tenVT = row[2].ToString();
              
                if (string.IsNullOrWhiteSpace(_maVT) && string.IsNullOrWhiteSpace(_tenVT))
                {
                    continue;
                }
                else
                {
                    ishasData = true;
                }
                bool IsNPL = lstVatTuCT.Any(x => (x.MaVT == _maVT&&x.TenVT== _tenVT));
                if (IsNPL)
                {
                    XtraMessageBox.Show($"Trùng vật tư: {_maVT}- Tên vật tư: {_tenVT}! Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);return;
                }
                else
                {
                    lstVatTuCT.Add(new VatTuChiTietEntity
                    {
                        ID=0,
                        TenVT=_tenVT,
                        MaHienThi = _maVT

                    });
                    string msResult = "";
                    string url1 = URL + "VatTuChiTiet/Post";
                    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url1, lstVatTuCT); }).Result;
                    LoadKhoSize(false);
                }
            }
            if (!ishasData)
            {
                XtraMessageBox.Show($"File Excel không có dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }

     
      
        private void SuaDong()
        {
            //isThem = true;
            //txtMaVT.Properties.ReadOnly = false;
            //txtTenVatTu.Properties.ReadOnly = false;
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);

        }
        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gVVatTuChiTiet.OptionsBehavior.Editable = false;
                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gVVatTuChiTiet.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    gVVatTuChiTiet.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = true;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }

        
        private void NapLaiDong()
        {
            Them.Enabled = true;
            textID.Text = "0";
            txtMaVT.Text = "";
            txtTenVatTu.Text = "";
            LoadKhoSize(false);
        }

    
        private void loadThongTinKhoSize(int ID)
        {
            if (dttb != null)
            {
                Sua.Enabled = true;
                Xoa.Enabled = true;
                VatTuChiTietEntity result = lstVatTuCT.Find(nl => nl.ID == ID);
                textID.Text = ID.ToString();
                textID.Text = ID.ToString();
                txtMaVT.Text = result.MaHienThi;
                txtTenVatTu.Text = result.TenVT;
              
                txtMaVT.Properties.ReadOnly = true;
                txtTenVatTu.Properties.ReadOnly = true;
              
            }
            /*else
            {

                MessageBox.Show("Chưa có dữ liệu, vui lòng thêm dữ liệu ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }*/

        }

        private void LoadKhoSize(bool isFromSave)
        {
            try
            {
                clsWaitForm.ShowWaitForm(this, 1000);
                string url = string.Format("{0}", URL + $"VatTuChiTiet/Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    dttb = null;
                    lstVatTuCT = null;
                    gCVatTuChiTiet.DataSource = null;
                    gCVatTuChiTiet.RefreshDataSource();
                    Sua.Enabled = false;
                    Xoa.Enabled = false;
                    btnXuatExcel.Enabled = false;
                    //Luu.Enabled = false;
                    //XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng kiểm tra lại hoặc nhập thêm dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                    if (!string.IsNullOrEmpty(json))
                {
                    dttb = JsonConvert.DeserializeObject<DataTable>(json);
                    lstVatTuCT = JsonConvert.DeserializeObject<List<VatTuChiTietEntity>>(json);
              
                    loadThongTinKhoSize(lstVatTuCT[0].ID);
                }
                
                gCVatTuChiTiet.DataSource = lstVatTuCT.Count > 0 ? lstVatTuCT : null;
                gCVatTuChiTiet.RefreshDataSource();
                GridViewUpdateStatus(_status);
                if (lstVatTuCT.Count > 0)
                {
                    Sua.Enabled = true;
                    Xoa.Enabled = true;
                }
                if (isFromSave && FocusedIndex >= 0)
                {
                   
                    gVVatTuChiTiet.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gCVatTuChiTiet;
                }
                //Luu.Enabled = false;

            }
            catch
            {
                return;
                //XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng kiểm tra lại hoặc nhập thêm dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void gridNhomNPL_Click(object sender, EventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {
                if(!isThem)
                {
                    int rowHandle = view.FocusedRowHandle;


                    if (rowHandle >= 0)
                    {

                        int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
                        loadThongTinKhoSize(idValue);

                    }
                }    
               
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
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Thư viện Khổ/Size");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    string LoGo = KHDongThungLib.getImgPath("DongNai.jpg");
                    Image image = Image.FromFile(LoGo);
                    OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                    picture.SetPosition(0, 0, 0, 0);
                    picture.SetSize(105, 55);
                    range = worksheet.Cells["D1:M1"]; range.Merge = true; range.Value = ""; range.Style.Font.Bold = true;
                    range = worksheet.Cells["C2:E3"]; range.Merge = true; range.Value = "Vật tư chi tiết"; range.Style.Font.Bold = true;
                    dtXuatExcelTVKhoSize(dtsave, worksheet);
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        public void dtXuatExcelTVKhoSize(DataTable dtSave, ExcelWorksheet worksheet, bool flagFilter = true)
        {
            ExcelRange range = worksheet.Cells;
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Thiết lập tiêu đề bảng
            range = worksheet.Cells["C4"];
            range.Value = "STT";
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Kiểu tô màu
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Màu nền

            range = worksheet.Cells["D4"];
            range.Merge = true;
            range.Value = "Mã Vật Tư";
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Kiểu tô màu
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Màu nền

            range = worksheet.Cells["E4"];
            range.Merge = true;
            range.Value = "Tên vật tư";
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Kiểu tô màu
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Màu nền

            int row = 5; // Bắt đầu từ dòng 5
            int index = 0;

            // Duyệt qua các hàng dữ liệu trong DataTable
            foreach (DataRow rows in dtSave.Rows)
            {
                worksheet.Cells[row, 3].Value = index + 1;
                worksheet.Column(1).AutoFit();

                worksheet.Cells[row, 4].Value = rows["MaHienThi"].ToString();
                worksheet.Column(2).AutoFit();
                worksheet.Cells[row, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                worksheet.Cells[row, 5].Value = rows["TenVT"].ToString();
                worksheet.Column(3).AutoFit();
                worksheet.Cells[row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                // Tô màu nền cho dòng chẵn/lẻ
                if (index % 2 == 0) // Dòng chẵn
                {
                    worksheet.Cells[row, 3, row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 3, row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242)); // Màu xám nhạt
                }
                else // Dòng lẻ
                {
                    worksheet.Cells[row, 3, row, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 3, row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White); // Màu trắng
                }

                // Căn chỉnh dữ liệu
                worksheet.Cells[row, 3, row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                index++;
                row++;
            }

            // Thêm border cho toàn bộ bảng (bao gồm tiêu đề và dữ liệu)
            int totalRows = dtSave.Rows.Count + 4; // Tổng số dòng (bao gồm tiêu đề)
            var tableRange = worksheet.Cells[4, 3, totalRows, 5]; // Phạm vi bảng từ A4 đến C cuối cùng
            tableRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            tableRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            // Đặt màu cho border
            tableRange.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            tableRange.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);

            // Tự động điều chỉnh độ rộng các cột
            for (int col = 3; col <= 5; col++) // A=1, B=2, ..., C=3
            {
                worksheet.Column(col).AutoFit();
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
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        excelPackage.Workbook.Properties.Title = "VatTuChiTietMau";

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

     /*   private void gridNhomNPL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {

                int rowHandle = view.FocusedRowHandle;


                if (rowHandle >= 0)
                {

                    int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
                    loadThongTinKhoSize(idValue);

                }
            }
            Xoa.Enabled = true;
            Sua.Enabled = true;
        }*/

        private void gridNhomNPL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view= sender as GridView;
            if(e.Column.FieldName=="STT"&&e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }    
        }

        private void gVThuVienKhoSize_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if(!isThem)
            {
                int focusedRowHandle = e.FocusedRowHandle;
                if (focusedRowHandle >= 0)
                {

                    var focusedRowData = gVVatTuChiTiet.GetRow(focusedRowHandle) as VatTuChiTietEntity;
                    loadThongTinKhoSize(focusedRowData.ID);
                    btnXuatExcel.Enabled = true;
                    Xoa.Enabled = true;
                    Sua.Enabled = true;
                    NapLai.Enabled = true;

                }
            }    
           
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            //Them.Enabled = false;
            Sua.Enabled = false;
            Xoa.Enabled = false;
            Luu.Enabled = true;
            //txtMaVT.Properties.ReadOnly = false;
            //txtTenVatTu.Properties.ReadOnly = false;
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
            //Luu.Enabled = false;
        }

        private void NapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
        }

        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                ReadExcelPL(Sfd.FileName);
            }
        }

        private void btnXuatFileMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("NhapVatTuChiTiet{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateVatTuChiTiet.xlsx";
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

        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string mavattu = string.Empty;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (dttb is null || dttb.Rows.Count == 0) return;
            //mavattu = dttb.AsEnumerable().Where(x => x["MaVatTu"] == mavattu.ToString()).FirstOrDefault()["MaVatTu"].ToString();
            //int Size = 0;
            Sfd.FileName = string.Format("VatTuChiTiet");
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
                    op.SheetName = string.Format("VatTuChiTiet");
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

        private void gridNhomNPL_CustomDrawColumnHeader_1(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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


        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
/*
            if (this.gridNhomNPL.FocusedColumn != null)
            {
                e.Valid = true;

                if (this.gridNhomNPL.FocusedColumn == colMaKhoSize)
                {
                    List<VatTuChiTietEntity> lst = this.gridNhomNPL.DataSource as List<VatTuChiTietEntity>;
                    bool IsNNPL = lst.Any(x => x.MaNhom == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Mã nhóm không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsNNPL)
                    {

                        e.ErrorText = $"Mã nhóm {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }

                }
                else if (this.gridNhomNPL.FocusedColumn == colMaVT)
                {
                    List<VatTuChiTietEntity> lst = this.gridNhomNPL.DataSource as List<VatTuChiTietEntity>;
                    bool IsNNPL = lst.Any(x => x.TenNhom == e.Value.ToString());
                    if (string.IsNullOrEmpty(e.Value.ToString()) || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        e.ErrorText = $"Tên nhóm không được bỏ trống";
                        e.Valid = false;
                    }
                    else if (IsNNPL)
                    {

                        e.ErrorText = $"Tên nhóm {e.Value.ToString()} đã bị trùng";
                        e.Valid = false;
                    }
                }
            }*/
        }

        private void gVVatTuChiTiet_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.RowHandle < 0)
                return;
            lstRowUpdate.Add(e.RowHandle);
        }

        private void gCVatTuChiTiet_EditorKeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void gVVatTuChiTiet_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private string ReplaceSpecialCharactersAndRemoveSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Loại bỏ khoảng trắng trước
            input = Regex.Replace(input, @"\s+", "");

            // Thay thế tất cả các ký tự không phải chữ cái và số bằng ký tự '_'
            string result = Regex.Replace(input, @"[^a-zA-Z0-9]", "_");
            return result;
        }
        private bool ValidateDataInsert(VatTuChiTietEntity itemInsert)
        {

            List<VatTuChiTietEntity> lstDataSource = gVVatTuChiTiet.DataSource as List<VatTuChiTietEntity>;
            int focusedRowHandle = gVVatTuChiTiet.FocusedRowHandle;
            VatTuChiTietEntity focusedRowData = (VatTuChiTietEntity)gVVatTuChiTiet.GetRow(focusedRowHandle);
            for (int i = 0; i < lstDataSource.Count; i++)
            {
                if (lstDataSource[i].TenVT == null)
                {
                    continue;
                }
                if (focusedRowData != null)
                {
                    if (lstDataSource[i].TenVT == focusedRowData.TenVT) continue;
                }
                // Kiểm tra nếu đồng thời 3 field( MaVT, TenNPL và TenMauNPL) đã tồn tại thì không insert
                if (ReplaceSpecialCharactersAndRemoveSpaces(itemInsert.TenVT).ToUpper() == ReplaceSpecialCharactersAndRemoveSpaces(lstDataSource[i].TenVT).ToUpper())
                {
                    XtraMessageBox.Show("Tên vật tư đã bị trùng. Vui lòng nhập lại thông tin!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return true;
                }
            }
            return false;
        }
    }
}