using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmDicDanhGiaNCC : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;
        private int Version = 1;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private DataTable danhgiaTable;
        private bool isInsertingRow = false;
        private int newVer = 1;
        private string selectedVer;
        private bool isActive;
        DataTable tblTC;
        DataTable tblCT;
        DataTable tblScore;
        public frmDicDanhGiaNCC()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            SetupLayout();
            //LoadVersion();
            loadSearchLookupEdit1();
            //LoadTieuChiDG();

            selectedpage(xtraTabControl1.SelectedTabPage);
        }


        private void SetupLayout()
        {
            //RepositoryItemMemoEdit memoEdit = new RepositoryItemMemoEdit();
            //memoEdit.WordWrap = true;
            //grcTCDanhGia.RepositoryItems.Add(memoEdit);

            //var noteColumn = grvTCDanhGia.Columns["TieuChi"];
            //if (noteColumn != null)
            //{
            //    noteColumn.ColumnEdit = memoEdit;
            //    noteColumn.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            //    noteColumn.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
             
               
            //}

            SetupGroupLevelColors();
        }


        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
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
                Import.Enabled = false;
                barButtonItem4.Enabled = false;
                simpleButton2.Enabled = false;
                simpleButton4.Enabled = false;
                simpleButton6.Enabled = false;
                //barButtonItem7.Enabled = false;
            }
            if (!_allowAdd && !_allowEdit)
            {
                Luu.Enabled = false;
                activeBtn.Enabled = false;
            }
            if (!_allowDelete)
            {
                Xoa.Enabled = false;
                simpleButton3.Enabled = false;
                simpleButton5.Enabled = false;
                simpleButton7.Enabled = false;
                //barButtonItem10.Enabled = false;
            }

        }
        private void Import_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmImportExcelDG frm = new frmImportExcelDG(newVer);
            frm.ShowDialog();
            if (!string.IsNullOrEmpty(frm.Path) && !string.IsNullOrEmpty(frm.SheetName))
            {
                ReadFileExcel(frm.Path, frm.SheetName, frm.versionNameText);
            }
            else
            {
                XtraMessageBox.Show("Bạn chưa chọn file hoặc sheet!");
            }
        }
      
        private void SetupGroupLevelColors()
        {
            groupLevelColorBackground = new Dictionary<int, Color>
    {
        { -1, ColorTranslator.FromHtml("#DDEBFB") },
        { 0, ColorTranslator.FromHtml("#DDEBFB") },
        { 1, ColorTranslator.FromHtml("#FFE2D3") },
        { 2, ColorTranslator.FromHtml("#D7F5E8") }
    };

            groupLevelColors = new Dictionary<int, Color>
    {
        { -1, ColorTranslator.FromHtml("#2A5D9F") },
        { 0, ColorTranslator.FromHtml("#2A5D9F") },
        { 1, ColorTranslator.FromHtml("#A53E25") },
        { 2, ColorTranslator.FromHtml("#2E7D5B") }
    };
        }
        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();
         
            dt.Columns.Add("ID", typeof(int));               
            dt.Columns.Add("NhomDanhGia", typeof(int));      
            dt.Columns.Add("No", typeof(string));              
            dt.Columns.Add("TieuChi", typeof(string));         
            dt.Columns.Add("DiemToiDa", typeof(decimal));       
            dt.Columns.Add("GhiChu", typeof(string));          
            dt.Columns.Add("GhiChuNhom", typeof(string));     
            dt.Columns.Add("TenNhomDanhGia", typeof(string));            
            dt.Columns.Add("Version", typeof(int));         
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("TenVersion", typeof(string));
            return dt;
        }

        private void LoadVersion()
        {
            try
            {
              
                string urlGET = $"{URL}PhieuDanhGiaNhaCC/Get?action=GetVersion";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;

           
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

              
                repositoryItemComboBox1.Items.Clear();

                // Thêm dữ liệu mới từ bảng
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (row["Version"] != null)
                        {
                            int.TryParse(row["Version"].ToString(), out int value);
                            if (Version <= value)
                            {
                                Version = value;
                            }
                            repositoryItemComboBox1.Items.Add(row["Version"].ToString());
                        }
                    }
                }


                RepositoryItemComboBox repoCombo = cbxVersion.Edit as RepositoryItemComboBox;
                if (repoCombo != null && repoCombo.Items.Count > 0)
                {
                    // chọn phần tử đầu tiên
                    cbxVersion.EditValue = repoCombo.Items[0];
                }

            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ghi log hoặc hiển thị thông báo)
                MessageBox.Show($"Lỗi khi tải Version: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadSearchLookupEdit1()
        {
            string urlGET = $"{URL}PhieuDanhGiaNhaCC/Get?action=GetVersion";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            getMaxVersion(dt);

            repositoryItemSearchLookUpEdit1.DataSource = dt;
            repositoryItemSearchLookUpEdit1.DisplayMember = "Version";
            repositoryItemSearchLookUpEdit1.ValueMember = "Version";
            repositoryItemSearchLookUpEdit1.PopulateViewColumns();

            GridView view = repositoryItemSearchLookUpEdit1View;

            view.Columns["IsActive"].Caption = "Đang áp dụng";
            view.Columns["TenVersion"].Caption = "Tên version";
            view.Columns["Version"].MinWidth = 70;
            view.Columns["Version"].MaxWidth = 70;
            view.Columns["Version"].OptionsColumn.FixedWidth = true;

            DataRow[] activeRows = dt.Select("IsActive = true");

            //DataRow[] verSionRows = dt.Select("Version = 1");
            barEditItem1.EditValue = activeRows[0]["Version"];
            barEditItem2.EditValue = activeRows[0]["IsActive"];
            view.OptionsFind.AlwaysVisible = true;
        }
        public void getMaxVersion(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            int maxVersion = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["Version"] != DBNull.Value)
                {
                    int version;
                    if (int.TryParse(row["Version"].ToString(), out version))
                    {
                        if (version > maxVersion)
                            maxVersion = version;
                    }
                }
            }
            newVer = maxVersion + 1;
        }
        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            if (barEditItem1.EditValue == null) return;
            selectedVer = barEditItem1.EditValue.ToString();

            DataTable dt = repositoryItemSearchLookUpEdit1.DataSource as DataTable;
            if (dt == null) return;

            DataRow[] rows = dt.Select(
                $"Version = '{selectedVer.Replace("'", "''")}'"
            );

            if (rows.Length == 0) return;
            isActive = (bool)rows[0]["IsActive"];
            if(isActive)
            {
                activeBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            else
            {
                activeBtn.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }

            barEditItem2.EditValue = isActive;
            LoadTieuChiDG();
            LoadData();
            LoadScore();
            LoaDetail();


        }

        private void ReadFileExcel(string filePath, string sheetName, string versionName)
        {
            DataTable tblSaveNhomTChi = CreateTable();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowStart = 4;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName);
                    if (worksheet == null)
                    {
                        Console.WriteLine("File Excel không chứa dữ liệu hoặc không hợp lệ!");
                        return;
                    }

                    worksheet.Protection.AllowSelectLockedCells = true;
                    worksheet.Protection.AllowSelectUnlockedCells = true;

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 1)
                    {
                        Console.WriteLine("File Excel không có dữ liệu!");
                        return;
                    }

                    int sortGroup = 0;
                    string currentGroupName = "";

                    for (int row = rowStart; row <= rowCount + 5; row++)
                    {
                        string colA = GetMergedCellValue(worksheet, row, 1).Trim();
                        string colB = GetMergedCellValue(worksheet, row, 2).Trim();
                        string colC = GetMergedCellValue(worksheet, row, 3).Trim();
                        string colD = GetMergedCellValue(worksheet, row, 4).Trim();

                        if (colA == "***" || colA == "****") break;
                        if (string.IsNullOrEmpty(colA)) continue;

                        
                        var mergedAddress = worksheet.MergedCells[row, 1];
                        if (!string.IsNullOrEmpty(mergedAddress))
                        {
                            var range = worksheet.Cells[mergedAddress];
                            int startRow = range.Start.Row;
                            int endRow = range.End.Row;

                           
                            if (row == startRow)
                            {
                                Regex regex = new Regex(@"^(?!No$)(?!^\d+$).+");
                                if (colA == colB && regex.IsMatch(colA))
                                {
                                    sortGroup++;
                                    currentGroupName = colA;
                                    row = endRow; 
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                          
                            Regex regex = new Regex(@"^(?!No$)(?!^\d+$).+");
                            if (colA == colB && regex.IsMatch(colA))
                            {
                                sortGroup++;
                                currentGroupName = colA;
                                continue;
                            }
                        }

                     
                        string no = colA;
                        string tieuChi = colB;
                        string diemToiDaStr = colC;
                        string ghiChu = colD;

                        if (string.IsNullOrEmpty(no) && string.IsNullOrEmpty(tieuChi)) continue;

                        decimal diemToiDa = 0;
                        decimal.TryParse(diemToiDaStr, out diemToiDa);

                        DataRow dr = tblSaveNhomTChi.NewRow();
                        dr["ID"] = 0;
                        dr["NhomDanhGia"] = 0;
                        dr["No"] = no;
                        dr["TieuChi"] = tieuChi;
                        dr["DiemToiDa"] = diemToiDa;
                        dr["GhiChu"] = ghiChu;
                        dr["GhiChuNhom"] = "";
                        dr["TenNhomDanhGia"] = currentGroupName;
                        dr["Version"] = newVer;
                        dr["Sort"] = sortGroup;
                        dr["TenVersion"] = versionName;

                        tblSaveNhomTChi.Rows.Add(dr);
                    }
                }

                string url = URL + "PhieuDanhGiaNhaCC/ImportTCNoiLV";
                string jsonData = JsonConvert.SerializeObject(tblSaveNhomTChi);
                string msResult = Task.Run(async () => await _clientExtension.PostAsync(url, jsonData)).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    //LoadVersion();
                    loadSearchLookupEdit1();
                    LoadTieuChiDG();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetMergedCellValue(ExcelWorksheet sheet, int row, int col)
        {
            var cell = sheet.Cells[row, col];

         
            if (cell.Merge)
            {
                foreach (var mergeCell in sheet.MergedCells)
                {
                    var range = sheet.Cells[mergeCell];
                    if (row >= range.Start.Row && row <= range.End.Row &&
                        col >= range.Start.Column && col <= range.End.Column)
                    {
                        return sheet.Cells[range.Start.Row, range.Start.Column].Text;
                    }
                }
            }

            // Nếu ô không merge
            return cell.Text;
        }

        private void LoadTieuChiDG()
        {
            try
            {
                this.ActiveControl  = simpleButton1;
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
              

                string urlGET = $"{URL}PhieuDanhGiaNhaCC/Get?action=CheckUsing&para1={selectedVer}";
                string url = string.Format("{0}?action={1}&&para1={2}", URL + "PhieuDanhGiaNhaCC/Get", "GetTieuChiDG_NoiLV", selectedVer);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                DataTable tblCheck = new DataTable();
                Import.Enabled = true;
                if (json == "[]")
                {
                    grcTCDanhGia.DataSource = null;
                   
                    return;
                }
                string jsonCheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                if (jsonCheck != "[]")
                {
                    tblCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                    if (tblCheck?.Rows?.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(tblCheck.Rows[0]["Msg"]?.ToString()) && tblCheck.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                        {
                            Import.Enabled = false;
                            Import.AppearanceDisabled.BackColor = Color.LightGray;
                            Import.AppearanceDisabled.ForeColor = Color.DarkGray;
                            Import.Refresh();

                        }
                    }

                }

                
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (!tbl.Columns.Contains("generatedID"))
                    tbl.Columns.Add("generatedID");
                int asckey = 1;
                foreach (DataRow row in tbl.Rows)
                {
                    row["generatedID"] = XuLyVTUnits.generatedTimeKey("id") + asckey.ToString();
                    asckey++;
                }
                danhgiaTable = tbl.Copy();
                grcTCDanhGia.DataSource = tbl;
                grcTCDanhGia.RefreshDataSource();
                grvTCDanhGia.ExpandAllGroups();
                grvTCDanhGia.FocusedRowHandle = 0;



            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadSearchLookupEdit1();
            LoadData();
            LoadScore();
            LoaDetail();
            
        }

        private void repositoryItemComboBox1_EditValueChanged(object sender, EventArgs e)
        {

            LoadTieuChiDG();
        }

        private void repositoryItemComboBox1_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            if (e.Value == null) return;

            string newValue = e.Value.ToString().Trim();
            if (newValue == "") return;

            var combo = repositoryItemComboBox1;

            // Nếu chưa có thì thêm
            if (!combo.Items.Contains(newValue))
            {
                combo.Items.Add(newValue);
            }

            e.Value = newValue;
            e.Handled = true;
        }

  



        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmImportExcelDG frm = new frmImportExcelDG(newVer);
            frm.ShowDialog();
            if (!string.IsNullOrEmpty(frm.Path) && !string.IsNullOrEmpty(frm.SheetName))
            {
                ReadFileExcel(frm.Path, frm.SheetName, frm.versionNameText);
            }
            //else
            //{
            //    XtraMessageBox.Show("Bạn chưa chọn file hoặc sheet!");
            //}
        }

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }
        private void grv_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;

                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;

                int groupLevel = view.GetRowLevel(e.RowHandle);

                GridColumn groupColumn = info.Column;


                info.GroupText = string.Format("{0}", info.GroupValueText);

                if (view.IsGroupRow(e.RowHandle))

                {

                    Color textColor = Color.Black;

                    switch (groupLevel)

                    {

                        case 0: textColor = Color.MediumBlue; break;

                        case 1: textColor = Color.Maroon; break;

                    }

                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();

                    e.Handled = true;

                }

            }
            catch (Exception ex)
            {

            }
        }

        private void grvTCDanhGia_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (isInsertingRow)
                return;
            if (e.Column.FieldName == "Sort" || e.Column.FieldName == "No")
            {
                GridView view = sender as GridView;

                // Lấy giá trị Sort
                int sort1 = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRowIndex1, "Sort") ?? 0);
                int sort2 = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRowIndex2, "Sort") ?? 0);

                // Nếu khác Sort → so sánh theo Sort
                if (sort1 != sort2)
                {
                    e.Result = sort1.CompareTo(sort2);
                }
                else
                {
                    // Cùng Sort → so sánh theo No (dạng phân cấp)
                    string noStr1 = view.GetListSourceRowCellValue(e.ListSourceRowIndex1, "No")?.ToString() ?? "";
                    string noStr2 = view.GetListSourceRowCellValue(e.ListSourceRowIndex2, "No")?.ToString() ?? "";

                    e.Result = CompareHierarchicalNumbers(noStr1, noStr2);
                }

                e.Handled = true;
            }
        }

        private int CompareHierarchicalNumbers(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
                return 0;
            if (string.IsNullOrEmpty(str1))
                return 1;
            if (string.IsNullOrEmpty(str2))
                return -1;

            // Tách các phần số (ví dụ: "1.2.3" -> [1, 2, 3])
            string[] parts1 = str1.Split('.');
            string[] parts2 = str2.Split('.');

            int maxLength = Math.Max(parts1.Length, parts2.Length);

            for (int i = 0; i < maxLength; i++)
            {
                // Nếu hết phần để so sánh, chuỗi ngắn hơn đứng trước
                if (i >= parts1.Length)
                    return -1;
                if (i >= parts2.Length)
                    return 1;

                // Parse từng phần thành số
                int num1, num2;
                bool isParsed1 = int.TryParse(parts1[i].Trim(), out num1);
                bool isParsed2 = int.TryParse(parts2[i].Trim(), out num2);

                // Nếu không parse được, so sánh string
                if (!isParsed1 && !isParsed2)
                {
                    int strCompare = string.Compare(parts1[i], parts2[i], StringComparison.Ordinal);
                    if (strCompare != 0)
                        return strCompare;
                }
                else if (!isParsed1)
                    return 1;
                else if (!isParsed2)
                    return -1;
                else
                {
                    // So sánh số
                    if (num1 != num2)
                        return num1.CompareTo(num2);
                }
            }

            return 0;
        }

        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("DanhGia{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "MauImportDanhGia.xlsx";
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

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "DanhGiaNCCNoiLamViec";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {

                    var worksheet = excelPackage.Workbook.Worksheets[0];

                 
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                if (XtraMessageBox.Show(
                            "Bạn có chắc muốn lưu thay đổi?",
                            "Lưu",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                calcDanhGiaChange();
                return;
            }
            if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
            {
                if (tblTC != null && tblTC.Rows.Count > 0)
                {
                    SaveDataTC(tblTC);
                   
                }
                if (tblScore != null && tblScore.Rows.Count > 0)
                {
                    SaveDataScore(tblScore);
                    
                }
                if (tblCT != null && tblCT.Rows.Count > 0)
                {
                   
                    SaveDataCT(tblCT);
                    
                }
                LoadData();
                LoadScore();
                LoaDetail();
            }


        }
        private void calcDanhGiaChange()
        {
            grvTCDanhGia.CloseEditor();
            grvTCDanhGia.UpdateCurrentRow();
            if (danhgiaTable == null || danhgiaTable.Columns.Count <= 0 || danhgiaTable.Rows.Count <= 0) return;
            DataTable dt = grcTCDanhGia.DataSource as DataTable;
            DataTable dtcheck = danhgiaTable.Copy();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            if (checkEmpty(dt)) return;
        
            dt.PrimaryKey = new DataColumn[] { dt.Columns["generatedID"] };
            dtcheck.PrimaryKey = new DataColumn[] { dtcheck.Columns["generatedID"] };
            DataTable tableInsert = dt.Clone();
            DataTable tableDelete = dtcheck.Clone();
            DataTable tableUpdate = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                var key = row["generatedID"];
                if (dtcheck.Rows.Find(key) == null)
                {
                    tableInsert.ImportRow(row);
                }
            }

            foreach (DataRow row in dtcheck.Rows)
            {
                var key = row["generatedID"];
                if (dt.Rows.Find(key) == null)
                {
                    tableDelete.ImportRow(row);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                var key = row["generatedID"];
                if (dtcheck.Rows.Find(key) != null)
                {
                    tableUpdate.ImportRow(row);
                }
            }

            addDanhGiaChange(tableInsert);
            removeDanhGiaChange(tableDelete);
            editDanhGiaChange(tableUpdate);
        }
        private bool checkEmpty(DataTable dt)
        {
            GridView view = grvTCDanhGia;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row["DiemToiDa"] == DBNull.Value || string.IsNullOrWhiteSpace(row["TieuChi"].ToString()))
                {
                    XtraMessageBox.Show(
                        "Tồn tại hàng có dữ liệu trống!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    focusCell(view, i, "DiemToiDa");                   
                    return true;
                }
                if (row["TieuChi"] == DBNull.Value || string.IsNullOrWhiteSpace(row["TieuChi"].ToString()))
                {
                    XtraMessageBox.Show(
                        "Tồn tại hàng có dữ liệu trống!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    focusCell(view, i, "TieuChi");                    
                    return true;
                }
            }
            return false;
        }
        private void focusCell(GridView view, int rowIndex, string columnName)
        {
            int rowHandle = view.GetRowHandle(rowIndex);
            GridColumn col = view.Columns[columnName];

            if (rowHandle < 0 || col == null) return;

            view.FocusedRowHandle = rowHandle;
            view.FocusedColumn = col;
            view.MakeRowVisible(rowHandle);
            view.ShowEditor();
        }
        private void addDanhGiaChange(DataTable dt)
        {
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("DanhGiaNLV");
            request.Action = "adddanhgiachange";
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = request.TypeTable.NewRow();
                newRow["TenNhomDanhGia"] = row["TenNhomDanhGia"];
                newRow["NhomDanhGia"] = row["NhomDanhGia"];
                newRow["Version"] = row["Version"];
                newRow["Sort"] = row["Sort"];
                newRow["No"] = row["No"].ToString();
                newRow["GhiChu"] = row["GhiChu"];
                newRow["DiemToiDa"] = row["DiemToiDa"];
                newRow["TieuChi"] = row["TieuChi"];
                request.TypeTable.Rows.Add(newRow);
            }
            string urlGetListDataTable = URL + "DanhGiaNLV/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void removeDanhGiaChange(DataTable dt)
        {
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("DanhGiaNLV");
            request.Action = "removedanhgiachange";
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = request.TypeTable.NewRow();
                newRow["ID"] = row["ID"];
                newRow["NhomDanhGia"] = row["NhomDanhGia"];
                newRow["Version"] = row["Version"];
                request.TypeTable.Rows.Add(newRow);
            }
            string urlGetListDataTable = URL + "DanhGiaNLV/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void editDanhGiaChange(DataTable dt)
        {
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("DanhGiaNLV");
            request.Action = "editdanhgiachange";
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = request.TypeTable.NewRow();
                newRow["ID"] = row["ID"];
                newRow["No"] = row["No"].ToString();
                newRow["GhiChu"] = row["GhiChu"];
                newRow["DiemToiDa"] = row["DiemToiDa"];
                newRow["TieuChi"] = row["TieuChi"];
                request.TypeTable.Rows.Add(newRow);
            }
            string urlGetListDataTable = URL + "DanhGiaNLV/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void grvTCDanhGia_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            if (e.HitInfo.InRowCell && e.HitInfo.Column != null)
            {
                e.Menu.Items.Clear();
                int rowHandle = e.HitInfo.RowHandle;
                int dataRowIndex = view.GetDataSourceRowIndex(rowHandle);

                var newRow = new DXMenuItem("Thêm hàng mới", (o, args) =>
                {
                    var dt = view.GridControl.DataSource as DataTable;
                    if (dt == null) return;

                    int clickedRowHandle = rowHandle;
                    int insertIndex = view.GetDataSourceRowIndex(clickedRowHandle);

                    string tenNhom = view.GetRowCellValue(clickedRowHandle, "TenNhomDanhGia")?.ToString();
                    int nhom = XuLyVTUnits.SmartTryParse<int>(view.GetRowCellValue(clickedRowHandle, "NhomDanhGia")?.ToString());
                    int ver = XuLyVTUnits.SmartTryParse<int>(view.GetRowCellValue(clickedRowHandle, "Version")?.ToString());
                    int sortVal = Convert.ToInt32(view.GetRowCellValue(clickedRowHandle, "Sort") ?? 0);

                    try
                    {
                        isInsertingRow = true; // 🔥 TẠM NGƯNG SORT

                        // 1️⃣ Insert đúng vị trí
                        DataRow newDr = dt.NewRow();
                        newDr["TenNhomDanhGia"] = tenNhom;
                        newDr["NhomDanhGia"] = nhom;
                        newDr["Version"] = ver;
                        newDr["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
                        newDr["Sort"] = sortVal;
                        newDr["No"] = 0;

                        dt.Rows.InsertAt(newDr, insertIndex + 1);

                        // 2️⃣ Đánh lại No trong nhóm (DATA)
                        int stt = 1;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["TenNhomDanhGia"]?.ToString() == tenNhom)
                                dr["No"] = stt++;
                        }

                        dt.AcceptChanges();
                    }
                    finally
                    {
                        isInsertingRow = false; // 🔥 BẬT SORT LẠI
                    }

                    // 3️⃣ Refresh → Grid sort lại ĐÚNG
                    view.RefreshData();

                    // focus dòng mới
                    int newHandle = view.GetRowHandle(insertIndex + 1);
                    view.FocusedRowHandle = newHandle;
                });


                var delRow = new DXMenuItem("Xoá hàng", (o, args) =>
                {
                    var dt = view.GridControl.DataSource as DataTable;
                    if (dt == null) return;
                    if (XtraMessageBox.Show(
                        "Bạn có chắc muốn xoá dòng này?",
                        "Xác nhận xoá",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;
                    var tenNhom = view.GetRowCellValue(rowHandle, "TenNhomDanhGia")?.ToString();                    
                    view.DeleteRow(rowHandle);
                    int stt = 1;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted)
                            continue;

                        if (dr["TenNhomDanhGia"]?.ToString() == tenNhom)
                        {
                            dr["No"] = stt++;
                        }
                    }

                    dt.AcceptChanges();
                    view.RefreshData();
                });

                e.Menu.Items.Add(newRow);
                e.Menu.Items.Add(delRow);
            }
            if (e.HitInfo.InGroupRow)
            {
                e.Menu.Items.Clear();

                int groupRowHandle = e.HitInfo.RowHandle;

                var delGroup = new DXMenuItem("Xoá nhóm này", (o, args) =>
                {
                    if (XtraMessageBox.Show(
                        "Bạn có chắc muốn xoá toàn bộ nhóm này?",
                        "Xác nhận xoá nhóm",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;

                    // duyệt ngược tránh lỗi
                    var dt = view.GridControl.DataSource as DataTable;
                    string tenNhom = view.GetGroupRowValue(groupRowHandle)?.ToString();
                    if (string.IsNullOrEmpty(tenNhom)) return;

                    // XOÁ TRÊN DATATABLE (AN TOÀN NHẤT)
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = dt.Rows[i];
                        if (dr.RowState == DataRowState.Deleted) continue;

                        if (dr["TenNhomDanhGia"]?.ToString() == tenNhom)
                        {
                            dt.Rows[i].Delete();
                        }
                    }

                    dt.AcceptChanges();
                    view.RefreshData();
                });

                e.Menu.Items.Add(delGroup);
            }
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show(
                        "Bạn có chắc muốn xoá version này?",
                        "Xác nhận xoá",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            xoaDanhGia();
            clsWaitForm.ShowWaitForm(this, 2000);
            loadSearchLookupEdit1();
        }
        private void xoaDanhGia()
        {
            string urlcheck = $"{URL}DanhGiaNLV/GetTQ?action=GETCheckNLV&para1={selectedVer}&para2=&para3=&para4=&para5=";
            string jsoncheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
            DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheck);

            if (tblcheck != null && tblcheck.Rows.Count > 0)
            {
                XtraMessageBox.Show("Version này đã được sử dụng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var request = XuLyVTRequestPost.createDefault("DanhGiaNLV");
            request.Action = "deletedanhgia";
            request.Parameter3 = XuLyVTUnits.SmartTryParse<int>(selectedVer);
            string urlGetListDataTable = URL + "DanhGiaNLV/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private void activeBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(!isActive)
            {
                saveActive();
                
                clsWaitForm.ShowSuccessForm(this, 2000);
                loadSearchLookupEdit1();
            }
        }
        private void saveActive()
        {
            var request = XuLyVTRequestPost.createDefault("DanhGiaNLV");
            request.Action = "setisactive";
            request.Parameter3 = XuLyVTUnits.SmartTryParse<int>(selectedVer);          
            string urlGetListDataTable = URL + "DanhGiaNLV/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            activeBtn.Visibility = BarItemVisibility.Never;
        }

        private void repositoryItemMemoEdit1_EditValueChanged(object sender, EventArgs e)
        {
            GridView view = grvTCDanhGia;
            if (!(sender is MemoEdit memo)) return;

            int row = view.FocusedRowHandle;
            GridColumn col = view.FocusedColumn;

            // 🔐 LƯU CARET
            int caret = memo.SelectionStart;

            // ÉP GRID RECALC HEIGHT
            view.CloseEditor();
            view.UpdateCurrentRow();

            // MỞ LẠI EDITOR
            view.FocusedRowHandle = row;
            view.FocusedColumn = col;
            view.ShowEditor();

            // 🔐 KHÔI PHỤC CARET
            if (view.ActiveEditor is MemoEdit newMemo)
            {
                newMemo.SelectionStart = Math.Min(caret, newMemo.Text.Length);
                newMemo.SelectionLength = 0; // ❗ rất quan trọng
            }
        }

        private void grvTCDanhGia_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (!(view.ActiveEditor is MemoEdit memo)) return;

            memo.EditValueChanged -= repositoryItemMemoEdit1_EditValueChanged;
            memo.EditValueChanged += repositoryItemMemoEdit1_EditValueChanged;
        }

        private void grvTCDanhGia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && grvTCDanhGia.ActiveEditor is MemoEdit)
            {
                e.Handled = false; // cho MemoEdit xử lý
            }
        }


        ///////THƯ VIỆN ĐÁNH GIÁ TỔNG QUAN
        ///

        private void LoadData()
        {
            try
            {
                this.ActiveControl = simpleButton1;
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Lấy Dữ Liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");


                string urlGETTQ = $"{URL}DanhGiaNLV/GetTQ?action=GETTIEUCHITQ&para1={selectedVer}&para2=&para3=&para4=&para5=";
                string jsonTQ = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGETTQ); }).Result;
                if (jsonTQ == "[]")
                {
                    tblTC = null;
                    gridControl1.DataSource = null;

                    return;
                }
                tblTC = JsonConvert.DeserializeObject<DataTable>(jsonTQ);
                gridControl1.DataSource = tblTC;
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }

        private void gridView2_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoaDetail();
        }

        private void LoaDetail()
        {
            try
            {
                DataRow rowFocused = gridView1.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    int id = Convert.ToInt32(rowFocused["No"]);
                    string urlGETD = $"{URL}DanhGiaNLV/GetTQ?action=GETTIEUCHITQ_DIEM&para1={id}&para2={selectedVer}&para3=&para4=&para5=";
                    string jsonD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGETD); }).Result;
                    if (jsonD == "[]")
                    {
                        CreateSearchlookupcolScore();
                        tblCT = null;
                        gridControl3.DataSource = null;

                        return;
                    }

                    tblCT = JsonConvert.DeserializeObject<DataTable>(jsonD);
                    CreateSearchlookupcolScore();
                    gridControl3.DataSource = tblCT;
                }
                else 
                {
                    CreateSearchlookupcolScore();
                    gridControl3.DataSource = null;
                }

            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }


        private void CreateSearchlookupcolScore()
        {
            string urlGETDDG = $"{URL}DanhGiaNLV/GetTQ?action=GETTIEUCHITQ_DIEMDG&para1={selectedVer}&para2=&para3=&para4=&para5=";
            string jsonDDG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGETDDG); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonDDG);
            RepositoryItemSearchLookUpEdit rCountryEdittt = new RepositoryItemSearchLookUpEdit();
            rCountryEdittt.DataSource = tbl;
            rCountryEdittt.DisplayMember = "DiemDanhGia";
            rCountryEdittt.ValueMember = "MaDiemDanhGia";
            rCountryEdittt.ShowClearButton = false;
            rCountryEdittt.NullText = "[Chọn giá trị]";

            GridView view = rCountryEdittt.View;
            if (view.Columns.Count == 0)
            {
                view.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                view.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                view.Appearance.HeaderPanel.Options.UseTextOptions = true;
                GridColumn colMa = new GridColumn
                {
                    FieldName = "DiemDanhGia",
                    Caption = "Điểm",
                    Visible = true
                };
                view.Columns.Add(colMa);
                // Tắt auto-resize tránh nhảy cột
                view.OptionsView.ColumnAutoWidth = true;

            }
            gridColumn21.ColumnEdit = rCountryEdittt;
        }

        private void LoadScore()
        {
            try
            {
                this.ActiveControl = simpleButton1;
                string urlGETDDG = $"{URL}DanhGiaNLV/GetTQ?action=GETTIEUCHITQ_DIEMDG&para1={selectedVer}&para2=&para3=&para4=&para5=";
                string jsonDDG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGETDDG); }).Result;
                if (jsonDDG == "[]")
                {
                    tblScore = null;
                    gridControl2.DataSource = null;

                    return;
                }
                tblScore = JsonConvert.DeserializeObject<DataTable>(jsonDDG);
                gridControl2.DataSource = tblScore;
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            finally
            {
                //clsWaitForm.ShowSuccessForm(this, 1000);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }

        private DataTable CreateDatatableTC()
        {
            DataTable tbl = new DataTable("dtTableTC");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("No", typeof(string));
            tbl.Columns.Add("TieuChiTQ", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("LoaiDanhGia", typeof(string));
            tbl.Columns.Add("Version", typeof(int));
            tbl.Columns.Add("IsAcTive", typeof(bool));
            return tbl;
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (tblTC == null || tblTC.Rows.Count == 0)
            {
                tblTC = CreateDatatableTC();
            }
            string nextNo = GetNextNo(tblTC);
            DataRow dr = tblTC.NewRow();
            dr["ID"] = 0;
            dr["No"] = nextNo;
            dr["TieuChiTQ"] = "";
            dr["GhiChu"] = "";
            dr["LoaiDanhGia"] = "";
            dr["Version"] = selectedVer;
            dr["IsAcTive"] = isActive;
            tblTC.Rows.Add(dr);
            gridControl1.DataSource = tblTC;
            gridControl1.RefreshDataSource();
        }

        private DataTable CreateDatatableScore()
        {
            DataTable tbl = new DataTable("dtTableD");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaDiemDanhGia", typeof(string));
            tbl.Columns.Add("DiemDanhGia", typeof(string));
            tbl.Columns.Add("Version", typeof(int));
            tbl.Columns.Add("IsActive", typeof(bool));
            return tbl;
        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (tblScore == null || tblScore.Rows.Count == 0)
            {
                tblScore = CreateDatatableScore();
            }

            DataRow dr = tblScore.NewRow();
            dr["ID"] = 0;
            dr["MaDiemDanhGia"] = "";
            dr["DiemDanhGia"] = "";
            dr["Version"] = selectedVer;
            dr["IsAcTive"] = isActive;
            tblScore.Rows.Add(dr);
            gridControl2.DataSource = tblScore;
            gridControl2.RefreshDataSource();
        }

        private DataTable CreateDatatableCT()
        {
            DataTable tbl = new DataTable("dtTableCT");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("STT", typeof(int));
            tbl.Columns.Add("TieuChiTQ_ID", typeof(int));
            tbl.Columns.Add("Diem", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("Version", typeof(int));
            tbl.Columns.Add("IsAcTive", typeof(bool));
            return tbl;
        }
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            DataRow rowFocused = gridView1.GetFocusedDataRow() as DataRow;
            if (rowFocused == null) return;

            int id = Convert.ToInt32(rowFocused["ID"]);
            int no = Convert.ToInt32(rowFocused["No"]);
            if (tblCT == null || tblCT.Rows.Count == 0)
            {
                tblCT = CreateDatatableCT();
            }

            DataRow dr = tblCT.NewRow();
            dr["ID"] = 0;
            dr["STT"] = id;
            dr["TieuChiTQ_ID"] = no;
            dr["Diem"] = "";
            dr["ChiTiet"] = "";
            dr["Version"] = selectedVer;
            dr["IsAcTive"] = isActive;
            tblCT.Rows.Add(dr);
            gridControl3.DataSource = tblCT;
            gridControl3.RefreshDataSource();
        }

        public void SaveDataTC(DataTable dt)
        {
            this.ActiveControl = simpleButton1;
            //DataTable tblsaveTC = CreateDatatableTC();
            var req = new PostTQRequest
            {
                Action = "POSTTIEUCHITQ",
                Tbl = tblTC
            };
            // GỌI API ĐỂ LƯU PHIẾU
            string url = URL + "DanhGiaNLV/PostTQ";
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, req);
            }).Result;

             if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

        }

        private string GetNextNo(DataTable tbl)
        {
            if (tbl == null || tbl.Rows.Count == 0)
                return "1";

            int maxNo = tbl.AsEnumerable()
                .Where(r => r["No"] != DBNull.Value
                         && !string.IsNullOrWhiteSpace(r["No"].ToString())
                         && int.TryParse(r["No"].ToString(), out _))
                .Select(r => int.Parse(r["No"].ToString()))
                .DefaultIfEmpty(0)
                .Max();

            return (maxNo + 1).ToString();
        }



        public void SaveDataScore(DataTable dt)
        {
            this.ActiveControl = simpleButton1;
            //DataTable tblsaveTC = CreateDatatableTC();
            var req = new PostTQRequest
            {
                Action = "POSTTIEUCHITQ_DIEMDG",
                Tbl = (DataTable)null,
                Tbl2 = (DataTable)null,
                Tbl3 = tblScore
            };
            // GỌI API ĐỂ LƯU PHIẾU
            string url = URL + "DanhGiaNLV/PostTQ";
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, req);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

        }

        public void SaveDataCT(DataTable dt)
        {
            this.ActiveControl = simpleButton1;
            //DataTable tblsaveTC = CreateDatatableTC();
            var req = new PostTQRequest
            {
                Action = "POSTTIEUCHITQ_DIEM",
                Tbl = (DataTable)null,
                Tbl2 = tblCT,
                Tbl3 = (DataTable)null
            };
            // GỌI API ĐỂ LƯU PHIẾU
            string url = URL + "DanhGiaNLV/PostTQ";
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, req);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            XoaDong();
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    string id = dr["No"].ToString();
                    string version = dr["Version"].ToString();
                    //string macl = dr["MaLoaiNCC"].ToString();
                    string tcid = dr["ID"].ToString();

                    string urlcheck = $"{URL}DanhGiaNLV/GetTQ?action=GETCheckTC1&para1={tcid}&para2=&para3=&para4=&para5=";
                    string jsoncheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
                    DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheck);

                    if (tblcheck != null && tblcheck.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("tiêu chí này đã được sử dụng để đánh giá. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string url = $"{URL}DanhGiaNLV/Delete?action=DELETETIEUCHITQ&para1={id}&para2={version}&para3=&para4=&para5=";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            XoaDongScore();
        }

        private async void XoaDongScore()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView2.GetFocusedDataRow();
                    if (dr == null) return;
                    long id = Convert.ToInt64(dr["ID"]);
                    string maddg = dr["MaDiemDanhGia"].ToString();
                    string version = dr["Version"].ToString();

                    string urlcheck = $"{URL}DanhGiaNLV/GetTQ?action=GETCheckSCore&para1={maddg}&para2=&para3=&para4=&para5=";
                    string jsoncheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheck); }).Result;
                    DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheck);

                    if (tblcheck != null && tblcheck.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Điểm này đã được sử dụng để đánh giá. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string url = $"{URL}DanhGiaNLV/Delete?action=DELETETIEUCHITQ_DIEMDG&para1={maddg}&para2=&para3=&para4=&para5=";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadScore();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            XoaDong1();
        }

        private async void XoaDong1()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView3.GetFocusedDataRow();
                    if (dr == null) return;
                    string stt = dr["STT"].ToString();
                    string tctq = dr["TieuChiTQ_ID"].ToString();
                    string diem = dr["Diem"].ToString();
                    string version = dr["Version"].ToString();
                    //string macl = dr["MaLoaiNCC"].ToString();
                    string url = $"{URL}DanhGiaNLV/Delete?action=DELETETIEUCHITQ_DIEM&para1={stt}&para2={tctq}&para3={diem}&para4={version}&para5=";
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoaDetail();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "Diem") return;

            GridView view = sender as GridView;
            string selectedValue = e.Value?.ToString();
            if (string.IsNullOrEmpty(selectedValue)) return;

            int currentRow = e.RowHandle;

            bool isDuplicate = tblCT.AsEnumerable().Any(r =>
                r.RowState != DataRowState.Deleted &&
                r["Diem"].ToString() == selectedValue &&
                tblCT.Rows.IndexOf(r) != view.GetDataSourceRowIndex(currentRow)
            );

            if (isDuplicate)
            {
                XtraMessageBox.Show(
                    "Điểm đánh giá này đã được chọn!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                view.SetRowCellValue(currentRow, e.Column, DBNull.Value);
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "TieuChiTQ")
                return;

            GridView view = sender as GridView;
            string value = e.Value?.ToString().Trim();
            if (string.IsNullOrEmpty(value)) return;

            DataRow currentRow = view.GetDataRow(e.RowHandle);
            if (currentRow == null) return;

            bool isDuplicate = tblTC.AsEnumerable()
                .Where(r => r.RowState != DataRowState.Deleted)
                .Any(r =>
                    !object.ReferenceEquals(r, currentRow) &&
                    r["TieuChiTQ"] != DBNull.Value &&
                    r["TieuChiTQ"].ToString().Trim()
                        .Equals(value, StringComparison.OrdinalIgnoreCase)
                );

            if (isDuplicate)
            {
                XtraMessageBox.Show(
                    "Tiêu chí này đã tồn tại!",
                    "Trùng dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                view.CellValueChanged -= gridView1_CellValueChanged;
                view.SetRowCellValue(e.RowHandle, e.Column, DBNull.Value);
                view.CellValueChanged += gridView1_CellValueChanged;
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName != "DiemDanhGia")
                return;

            GridView view = sender as GridView;
            DataRow currentRow = view.GetDataRow(e.RowHandle);
            if (currentRow == null) return;

            string value = e.Value?.ToString().Trim();
            if (string.IsNullOrEmpty(value)) return;

            string version = currentRow["Version"]?.ToString();

            bool isDuplicate = tblScore.AsEnumerable()
                .Where(r => r.RowState != DataRowState.Deleted)
                .Any(r =>
                    !object.ReferenceEquals(r, currentRow) &&
                    r["DiemDanhGia"] != DBNull.Value &&
                    r["DiemDanhGia"].ToString().Trim()
                        .Equals(value, StringComparison.OrdinalIgnoreCase)
                );

            if (isDuplicate)
            {
                XtraMessageBox.Show(
                    "Điểm đánh giá này đã tồn tại!",
                    "Trùng dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // rollback giá trị
                view.CellValueChanged -= gridView2_CellValueChanged;
                view.SetRowCellValue(e.RowHandle, e.Column, DBNull.Value);
                view.CellValueChanged += gridView2_CellValueChanged;
            }
        }

        private void repositoryItemMemoEdit3_EditValueChanged(object sender, EventArgs e)
        {
            GridView view = gridView3;
            if (!(sender is MemoEdit memo)) return;

            int row = view.FocusedRowHandle;
            GridColumn col = view.FocusedColumn;

            // 🔐 LƯU CARET
            int caret = memo.SelectionStart;

            // ÉP GRID RECALC HEIGHT
            view.CloseEditor();
            view.UpdateCurrentRow();

            // MỞ LẠI EDITOR
            view.FocusedRowHandle = row;
            view.FocusedColumn = col;
            view.ShowEditor();

            // 🔐 KHÔI PHỤC CARET
            if (view.ActiveEditor is MemoEdit newMemo)
            {
                newMemo.SelectionStart = Math.Min(caret, newMemo.Text.Length);
                newMemo.SelectionLength = 0; // ❗ rất quan trọng
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            selectedpage(e.Page);
        }



        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }


        private void repositoryItemMemoEdit2_EditValueChanged(object sender, EventArgs e)
        {
            GridView view = gridView1;
            if (!(sender is MemoEdit memo)) return;

            int row = view.FocusedRowHandle;
            GridColumn col = view.FocusedColumn;

            // 🔐 LƯU CARET
            int caret = memo.SelectionStart;

            // ÉP GRID RECALC HEIGHT
            view.CloseEditor();
            view.UpdateCurrentRow();

            // MỞ LẠI EDITOR
            view.FocusedRowHandle = row;
            view.FocusedColumn = col;
            view.ShowEditor();

            // 🔐 KHÔI PHỤC CARET
            if (view.ActiveEditor is MemoEdit newMemo)
            {
                newMemo.SelectionStart = Math.Min(caret, newMemo.Text.Length);
                newMemo.SelectionLength = 0; // ❗ rất quan trọng
            }
        }

        private void selectedpage(DevExpress.XtraTab.XtraTabPage currentTab)
        {
            if (currentTab == xtraTabPage1)
            {
                activeBtn.Visibility = BarItemVisibility.Always;
                Import.Visibility = BarItemVisibility.Always;
                Xoa.Visibility = BarItemVisibility.Always;
                barButtonItem4.Visibility = BarItemVisibility.Always;
                barEditItem2.Visibility = BarItemVisibility.Always;
                barEditItem1.Visibility = BarItemVisibility.Always;
            }
            else if (currentTab == xtraTabPage2)
            {
                activeBtn.Visibility = BarItemVisibility.Never;
                Import.Visibility = BarItemVisibility.Never;
                Xoa.Visibility = BarItemVisibility.Never;
                barButtonItem4.Visibility = BarItemVisibility.Never;
                barEditItem2.Visibility = BarItemVisibility.Never;
                barEditItem1.Visibility = BarItemVisibility.Never;

                string urlGET = $"{URL}PhieuDanhGiaNhaCC/Get?action=GetVersion";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(urlGET)).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;

                DataRow[] verSionRows = dt.Select("Version = 1");
                barEditItem1.EditValue = verSionRows[0]["Version"];
            }
        }
    }
}