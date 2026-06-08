using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmTheKhoNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        bool flagChangeDate = true;
        private string currentFilterMode = "Khách hàng";
        private HttpClientExtension _clientExtension;
        private string isLoi ="0";
        public frmTheKhoNPL()
        {
            try
            {
              
                InitializeComponent();
                URL = (string)settingsReader.GetValue("URL", typeof(String));
                _clientExtension = new HttpClientExtension();
                //TuNgayEdit.EditValue = DateTime.Now;
                //DenNgayEdit.EditValue = DateTime.Now;
                Console.WriteLine("Cc");
              
            }
            catch (Exception ex)
            {
              
            }

        }
        protected override void OnLoad(EventArgs e)
        {
            InIt();
            LoadKH();
            loadCL();

            GetItemVai();
            LoadTheKhoTong();
            cbBoxLoc.SelectedIndexChanged += cbBoxLoc_SelectedIndexChanged;
        }
        private void InitializeFilterControl()
        {
            try
            {
                // Tạm thời ngừng layout updates
                layoutControl1.BeginUpdate();
                try
                {
                    //cbBoxLoc.Properties.Items.AddRange(new object[] { "Khách hàng", "Thời gian" });
                    cbBoxLoc.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

                    // Tạm thời bỏ event để tránh trigger khi set EditValue 
                 
                    cbBoxLoc.EditValue = "Khách hàng";
                   

                    //SetFilterMode("Khách hàng");
                }
                finally
                {
                    layoutControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void SetFilterMode(string mode)
        {
            string normalizedMode = mode.ToLower().Trim();
            bool isKhachHang = (normalizedMode == "khách hàng");

            // Suspend layout để thay đổi tất cả properties một lượt
            layoutControl1.BeginUpdate();
            try
            {
                layoutControlItem14.Visibility = isKhachHang ?
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Always :
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                layoutControlItem13.Visibility = isKhachHang ?
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Always :
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                layoutControlItem10.Visibility = !isKhachHang ?
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Always :
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                layoutControlItem11.Visibility = !isKhachHang ?
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Always :
                    DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Default;
                currentFilterMode = mode;
            }
            finally
            {
                layoutControl1.EndUpdate();
            }
        }

        
        private void InIt()
        {
            searchLookUpItemVai.Properties.ValueMember = "MaNPL";
            searchLookUpItemVai.Properties.DisplayMember = "Display";
            searchLookUpEditSoLo.Properties.ValueMember = "SoLoID";
            searchLookUpEditSoLo.Properties.DisplayMember = "SoLo";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";
            searchLookUpEditMH.Properties.ValueMember = "MaHang";

            searchLookUpEditCL.Properties.DisplayMember = "ChungLoaiVatTu";
            searchLookUpEditCL.Properties.ValueMember = "MaCLVT";
        }
        private void GetItemVai()
        {
            try
            {
                string tungay;
                string denngay;
                if (currentFilterMode.ToLower() == "khách hàng")
                {
                    tungay = ("1900-01-01");
                    denngay = ("2900-01-01");
                }
                else
                {
                    tungay = Convert.ToDateTime(TuNgayEdit.EditValue).ToString("yyyy-MM-dd");
                    denngay = Convert.ToDateTime(DenNgayEdit.EditValue).ToString("yyyy-MM-dd");
                }
                string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "all";
                string maHang = searchLookUpEditMH.EditValue?.ToString() ?? "all";
                string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GetVatTu&para2={maKH}&para3={maHang}&para4={tungay}&para5={denngay}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpItemVai.Properties.DataSource = null;
                    return;
                }
                searchLookUpItemVai.Properties.DataSource = tbl;

                //searchLookUpItemVai.EditValue = tbl.Rows[0]["MaNPL"];
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
                string tungay;
                string denngay;
                if (currentFilterMode.ToLower() == "khách hàng")
                {
                    tungay = ("1900-01-01");
                    denngay = ("2900-01-01");
                }
                else
                {
                    tungay = Convert.ToDateTime(TuNgayEdit.EditValue).ToString("yyyy-MM-dd");
                    denngay = Convert.ToDateTime(DenNgayEdit.EditValue).ToString("yyyy-MM-dd");
                }
                string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "all";
                string maHang = searchLookUpEditMH.EditValue?.ToString() ?? "all";
                string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GetSoLoMK&para1={maKH}&para2={maHang}&para3={tungay}&para4={denngay}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditSoLo.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditSoLo.Properties.DataSource = tbl;
                //searchLookUpEditSoLo.EditValue = tbl.Rows[0]["SoLoID"];
                //GetItemVai();
                //await LoadTheKhoTong();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi tải dữ liệu chi tiết: " + ex.Message);
            }

        }

        private void GetData()
        {
            try
            {
                DataRow dr = grvTheKhoTong.GetFocusedDataRow();
                if (dr == null) return;
                string tungay;
                string denngay;
                if (currentFilterMode.ToLower() == "khách hàng")
                {
                    tungay = ("1900-01-01");
                    denngay = ("2900-01-01");
                }
                else
                {
                    tungay = Convert.ToDateTime(TuNgayEdit.EditValue).ToString("yyyy-MM-dd");
                    denngay = Convert.ToDateTime(DenNgayEdit.EditValue).ToString("yyyy-MM-dd");
                }
                string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "all";
                string maHang = searchLookUpEditMH.EditValue?.ToString() ?? "all";
                string itemvai = dr["MaNPL"].ToString();
                var soloid = searchLookUpEditSoLo.EditValue?.ToString() ?? "all";
                string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GetTheKho&para1={tungay}&para2={denngay}&para3={itemvai}&para4={soloid}&para5={maHang}&para6={maKH}&para7={isLoi}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    grcTheKhoNPL.DataSource = null;
                    return;
                }
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
             LoadTheKhoTong();
        }

        private void searchLookUpItemVai_EditValueChanged(object sender, EventArgs e)
        {
            GetSoLo();
        }

        private void grvTheKhoNPL_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            //GridSummaryItem item = e.Item as GridSummaryItem;

            //if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            //{
            //    if (item.FieldName == "TonKho")
            //    {
            //        int visibleRowCount = grvTheKhoNPL.RowCount; // số dòng sau khi lọc

            //        if (visibleRowCount > 0)
            //        {
            //            var dataTable = grcTheKhoTong.DataSource as DataTable;
            //            object lastValue = dataTable.Rows[dataTable.Rows.Count - 1]["TonKho"];
            //            e.TotalValue = lastValue;
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

        private void cbBoxLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuNgayEdit.EditValueChanged -= TuNgayEdit_EditValueChanged;
            DenNgayEdit.EditValueChanged -= DenNgayEdit_EditValueChanged;
            if (cbBoxLoc.EditValue != null)
            {
                SetFilterMode(cbBoxLoc.EditValue.ToString());
                SetTextNull();
                LoadTheKhoTong();
            }
            TuNgayEdit.EditValueChanged += TuNgayEdit_EditValueChanged;
            DenNgayEdit.EditValueChanged += DenNgayEdit_EditValueChanged;
        }
        private void LoadKH()
        {
            string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GETKHTK";
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

            searchLookUpEditKH.Properties.DataSource = tbl;
            //searchLookUpEditKH.EditValue = tbl.Rows[0]["MaKH"];
            //await LoadMH();
            //LoadTheKhoTong();
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
        private void LoadMH()
        {
            string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "all";
            string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GETMHTK&para2={maKH}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
          
            if (json == "[]")
            {
              
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
              
                return;
            }
            //searchLookUpEditMH.EditValue = tbl.Rows[0]["MaHang"];
            searchLookUpEditMH.Properties.DataSource = tbl;
        }
        private void LoadTheKhoTong()
        {
            string tungay;
            string denngay;

            if (currentFilterMode.ToLower() == "khách hàng")
            {
                tungay = ("1900-01-01");
                denngay = ("2900-01-01");
            }
            else
            {
                tungay = Convert.ToDateTime(TuNgayEdit.EditValue).ToString("yyyy-MM-dd");
                denngay = Convert.ToDateTime(DenNgayEdit.EditValue).ToString("yyyy-MM-dd");
            }
            string itemvai = searchLookUpItemVai.EditValue?.ToString() ?? "all";
            string soloid = searchLookUpEditSoLo.EditValue?.ToString() ?? "all";
            string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "all";
            string maHang = searchLookUpEditMH.EditValue?.ToString() ?? "all";
            string cl = searchLookUpEditCL.EditValue?.ToString() ?? "all";
            if (string.IsNullOrEmpty(cl)) cl = "all";
            string url = $"{URL}PhieuKhoNPL/GetTheKhoNPL?action=GetTKTong&para1={tungay}&para2={denngay}&para3={itemvai}&para4={soloid}&para5={maHang}&para6={maKH}&para7={isLoi}&para8={cl}";
          
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                grcTheKhoTong.DataSource = null;
                grcTheKhoNPL.DataSource = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcTheKhoTong.DataSource = null;
                grcTheKhoNPL.DataSource = null;
                return;
            }
            grcTheKhoTong.DataSource = tbl;
            GetData();
        }

        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            LoadMH();
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            GetItemVai();
        }

        private void searchLookUpEditSoLo_EditValueChanged(object sender, EventArgs e)
        {
            GetItemVai();
        }

        private void grvTheKhoTong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GetData();
        }

        private void TuNgayEdit_EditValueChanged(object sender, EventArgs e)
        {
            if (TuNgayEdit.EditValue == null || DenNgayEdit.EditValue == null) return;
            DateTime tuNgay = Convert.ToDateTime(TuNgayEdit.EditValue);
            DateTime denNgay = Convert.ToDateTime(DenNgayEdit.EditValue);
            //if(tuNgay > denNgay)
            //{
            //    XtraMessageBox.Show("Vui lòng chọn thời gian hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            GetItemVai();
        }

        private void DenNgayEdit_EditValueChanged(object sender, EventArgs e)
        {
            if (TuNgayEdit.EditValue == null || DenNgayEdit.EditValue == null) return;
            DateTime tuNgay = Convert.ToDateTime(TuNgayEdit.EditValue);
            DateTime denNgay = Convert.ToDateTime(DenNgayEdit.EditValue);
            //if (tuNgay > denNgay)
            //{
            //    XtraMessageBox.Show("Vui lòng chọn thời gian hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            GetItemVai();
        }
        private void SetTextNull()
        {
            //searchLookUpEditKH.EditValue = "all";
            //searchLookUpEditMH.EditValue = "all";
            //searchLookUpEditSoLo.EditValue = "all";
            //searchLookUpItemVai.EditValue = "all";
            //TuNgayEdit.EditValue = DateTime.Now;
            //DenNgayEdit.EditValue = DateTime.Now;
            //grcTheKhoTong.DataSource = null;
            //grcTheKhoNPL.DataSource = null;
        }

        private void checkLoi_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isLoi = (bool)checkLoi.Checked ? "1" : "0";
            //grvTheKhoNPL.Columns["SLLoi"].Visible = (bool)checkLoi.Checked;
            grvTheKhoTong.Columns["SLLoi"].Visible = (bool)checkLoi.Checked;
        }
    }
}