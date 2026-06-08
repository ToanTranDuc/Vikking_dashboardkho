using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraLayout.Utils;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmTheKhoNewV2 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _tuNgay = string.Empty, _denNgay = string.Empty, _maDH = string.Empty, _ngayThang = string.Empty, _maKH = string.Empty, _dvTinh = string.Empty, _donhang = string.Empty, maPKL = string.Empty;
        int _soToNo = 0, cbLocValue = 1;
        int _totalSLTon = 0, totalNhap = 0, totalXuat = 0;
        string _dvsx = string.Empty;
        bool isCheckFirst = false, _isCheckHuy = true, isCoKhacHang = false, checkTon = true;
        private HttpClientExtension _clientExtension;


        public frmTheKhoNewV2()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
         
            string[] items = new string[] { "Thời gian", "Mã hàng" };

            tuNgay.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Ngày đầu tháng
            denNgay.EditValue = DateTime.Now; // Ngày hiện tại
           

            _dvTinh = "1";
          
            comboBoxEdit1.Properties.Items.Clear();
            CreateDefault();
            LoadKH();

            foreach (string item in items)
            {
                comboBoxEdit1.Properties.Items.Add(item);
            }
            comboBoxEdit1.EditValue = items[0].ToString();
            isCheckFirst = true;

        }
        private void CreateDefault()
        {
            searchLookUpEdit1.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEdit1.Properties.ValueMember = "MaDH";
        }

        private void LoadDS()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default == null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(
                        typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                string maDH = (searchLookUpEdit1.EditValue as string ?? "");
                string maKH = (searchLookUpEdit2.EditValue as string  ?? "");
                string url = $"{URL}TheKhoThanhPham/Get?Action=GetTQNhapKhoTP" +
                    $"&para={(cbLocValue == 1 ? "1990-01-01" : Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd"))}" +
                    $"&para2={(cbLocValue == 1 ? "3000-01-01" : Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd"))}" +
                    $"&para3={maKH}&para4={maDH}&para5={_dvTinh}";

                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl == null || tbl.Rows.Count == 0)
                    grcTheKho.DataSource = null;
                else
                {
                    grcTheKho.DataSource = tbl;
                    //LoadChiTietNhapXuatThanhPham();
                }
                   
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
        }

        private void LoadChiTietNhapXuatThanhPham()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default == null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(
                        typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));

                DataRow dataRow = bandviewTheKho.GetFocusedDataRow();
                if (dataRow == null) return;
                string _madhRow = dataRow["MaDH"].ToString();
                string _poRow = dataRow["POID"].ToString();
                string _maPKLRow = dataRow["MaPKL"].ToString();
                string maDH = (searchLookUpEdit1.EditValue as string ?? "");
                string maKH = (searchLookUpEdit2.EditValue as string ?? "");
                string url = $"{URL}TheKhoThanhPham/Get?Action=GetChiTietNhapXuatThanhPham" +
                    $"&para={(cbLocValue == 1 ? "1990-01-01" : Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd"))}" +
                    $"&para2={(cbLocValue == 1 ? "3000-01-01" : Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd"))}" +
                    $"&para3={maKH}" +
                    $"&para4={maDH}" +
                    $"&para5={_dvTinh}" +
                    $"&para6={_madhRow}" +
                    $"&para7={_poRow}" +
                    $"&para8={_maPKLRow}";

                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl == null || tbl.Rows.Count == 0)
                    grcChiTietXuatNhap.DataSource = null;
                else
                    grcChiTietXuatNhap.DataSource = tbl;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }

        }



        private void bandviewTheKho_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {

            int _valueSumaryCaton = 0;
            DataTable tbl = grcTheKho.DataSource as DataTable;
            if (tbl != null && tbl.Rows.Count != 0)
            {
                GridSummaryItem item = e.Item as GridSummaryItem;

                int valueSLN_LastRow = 0; // Giá trị SLNhap dòng cuối
                int valueSLX_LastRow = 0; // Giá trị SLXuat1 dòng cuối

                if ("SLTon".Contains(item.FieldName))
                {
                    // Lấy dòng cuối cùng
                    DataRow lastRow = tbl.Rows[tbl.Rows.Count - 1];

                    // Lấy giá trị của SLNhap và SLXuat1 dòng cuối
                    valueSLN_LastRow =
                    valueSLX_LastRow = Convert.ToInt32(lastRow["LuyKeNgay"]);

                    // Tổng giá trị lũy kế
                    e.TotalValue = valueSLX_LastRow;
                }
            }

        }

        private void LoadDH()
        {

            string url = $"{URL}TheKhoThanhPham/Get?Action=GetDonHang&para={(cbLocValue == 1 ? "1990-01-01" : Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd"))}&para2={(cbLocValue == 1 ? "3000-01-01" : Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd"))}&para3={_maKH}"; 
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0) return;
            searchLookUpEdit1.Properties.DataSource = tbl;
            LoadDS();
        }


        private void LoadKH()
        {
            string url = $"{URL}TheKhoThanhPham/Get?Action=GetKhachHang&para={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}&para2={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null) return;
            searchLookUpEdit2.Properties.DataSource = tbl;
            searchLookUpEdit2.Properties.DisplayMember = "TenKH";
            searchLookUpEdit2.Properties.ValueMember = "MaKH";
            LoadDH();
        }

        private void bandviewTheKho_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadChiTietNhapXuatThanhPham();
        }

        private void dateEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {

            _tuNgay = Convert.ToDateTime(tuNgay.EditValue).ToString("yyyy-MM-dd");
            if (!isCheckFirst)
                return;
            if (cbLocValue == 2)
            {
                LoadKH();
            }
        }

        private void dateEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {

            _denNgay = Convert.ToDateTime(denNgay.EditValue).ToString("yyyy-MM-dd");
            if (!isCheckFirst)
                return;
            if (cbLocValue == 2)
            {
                LoadKH();
            }


        }
      

        private void btnLocDH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadKH();
        }

        private void LocDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadKH();
        }

        private void checkValueLoc(string cbxText)
        {
            try
            {
                if (!isCheckFirst)
                    return;

                // Reset lại các giá trị đã chọn
                searchLookUpEdit1.EditValue = null;
                searchLookUpEdit2.EditValue = null;
                _maDH = "";
                _maKH = "";
                isCoKhacHang = false;

                maPKL = cbxText;
                if (cbxText == "Thời gian")
                {
                    layoutControlItem1.Visibility = LayoutVisibility.Always;
                    layoutControlItem2.Visibility = LayoutVisibility.Always;
                    layoutControlItem_Tungay.Visibility = LayoutVisibility.Always;
                    layoutControlItem_Denngay.Visibility = LayoutVisibility.Always;
                    cbLocValue = 2;
                }
                else
                {
                    layoutControlItem_Tungay.Visibility = LayoutVisibility.Never;
                    layoutControlItem_Denngay.Visibility = LayoutVisibility.Never;
                    cbLocValue = 1;
                }

                if (cbLocValue == 1)
                {
                    LoadKH();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void comboBoxEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            checkValueLoc(e.NewValue.ToString());
        }

        private void grcChiTietXH_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }

        private void checkSL_EditValueChanged(object sender, EventArgs e)
        {

            if (Convert.ToBoolean(checkSL.EditValue))
            {
                _dvTinh = "2";
                checkSK.EditValue = false;

            }
            else
            {
                _dvTinh = "1";
                checkSK.EditValue = true;

            }
            if (!isCheckFirst)
                return;
            LoadDS();
            LoadChiTietNhapXuatThanhPham();
        }

        private void checkSK_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(checkSK.EditValue)) 
            {
                checkSL.EditValue = false;
                _dvTinh = "1";
            }
            else
            {
                _dvTinh = "2";
                checkSL.EditValue = true;
            }
            LoadDS();
            LoadChiTietNhapXuatThanhPham();
        }

        private void btNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit2.EditValue = null;
        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            isCoKhacHang = true;

            if (searchLookUpEdit2.EditValue != null)
            {
                string maKH = searchLookUpEdit2.EditValue.ToString(); 
                _maKH = maKH;

            }

            LoadDH();
            LoadDS();
            LoadChiTietNhapXuatThanhPham();
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit1.EditValue != null)
            {
                string maDH = searchLookUpEdit1.EditValue.ToString(); 
                _maDH = maDH;

            }

            LoadDS();
            LoadChiTietNhapXuatThanhPham();
        }
        private void btnEX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmNhapSoNo frm = new frmNhapSoNo();
            //frm.ShowDialog();
            //_soToNo = frmNhapSoNo.soToNo;
            //_isCheckHuy = frmNhapSoNo.isCheckHuy;
            //if (!_isCheckHuy) return;
            DataTable dtTable = grcTheKho.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("TheKho_{0}_{1}", dtTable.Rows[0]["MaHang"], DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TheKho.xlsx";
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

        public void Export(string TemplateFileName, string ExportFileName, DataTable tblPhieuNhapKho)
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


                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["KH-TT02-BM18"];


                        ExcelRange range = ws.Cells;
                        string tenhang = "", khachHang = "", mahang = "", _mahang = "";

                        var chungloai = tblPhieuNhapKho.AsEnumerable().Where(x => x["TenCL"].ToString() != "").Select(x => x["TenCL"]).ToList().Distinct();
                        var chungloaiJoin = string.Join(", ", chungloai);
                        ws.Cells["e5"].Value = chungloaiJoin;

                        int countRow = 13;
                        foreach (DataRow item in tblPhieuNhapKho.Rows)
                        {

                            ws.Cells[countRow, 1].Value = Convert.ToDateTime(item["NgayThang"]).ToString("dd/MM/yyyy");
                            ws.Cells[countRow, 2].Value = item["TenBienBan"].ToString();
                            ws.Cells[countRow, 3, countRow, 4].Value = item["TenHang"];
                            ws.Cells[countRow, 3, countRow, 4].Merge = true;
                            ws.Cells[countRow, 6].Value = Convert.ToInt32(item["SLNhap"]);
                            ws.Cells[countRow, 7].Value = Convert.ToInt32(item["SLXuat"]);
                            ws.Cells[countRow, 8].Value = Convert.ToInt32(item["SLTon"]);
                            //ws.Cells[countRow, 9].Value = item["TenDVCL"].ToString();
                            //ws.Cells[countRow, 8].Value ="";
                            //ws.Cells[countRow, 9].Value ="";
                            countRow++;
                        }
                        int sumNhap = tblPhieuNhapKho.AsEnumerable().Sum(x => Convert.ToInt32(x["SLNhap"]));
                        int sumXuat = tblPhieuNhapKho.AsEnumerable().Sum(x => Convert.ToInt32(x["SLXuat"]));
                        int sumTon = tblPhieuNhapKho.AsEnumerable().Sum(x => Convert.ToInt32(x["SLTon"]));
                        ws.Cells[countRow, 6].Value = sumNhap;
                        ws.Cells[countRow,7].Value = sumXuat;
                        ws.Cells[countRow, 8].Value = sumTon;

                        var borderData = ws.Cells[13, 1, countRow - 1, 10].Style.Border;
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

            }
        }

        private void grvCTNK_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void grcChiTietXH_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
    }
}