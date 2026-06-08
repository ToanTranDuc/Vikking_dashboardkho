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
    public partial class frmTheKhoNew : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _tuNgay = string.Empty, _denNgay = string.Empty, _maDH = string.Empty, _ngayThang = string.Empty, _maKH = string.Empty, _dvTinh = string.Empty, _donhang = string.Empty, donhangtong = string.Empty, maPKL = string.Empty;
        int _soToNo = 0, cbLocValue = 1;
        int _totalSLTon = 0, totalNhap = 0, totalXuat = 0;
        string _dvsx = string.Empty;
        bool isCheckFirst = false, _isCheckHuy = true, isCoKhacHang = false, checkTon = true;
        private HttpClientExtension _clientExtension;


        public frmTheKhoNew()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string[] items = new string[] { "Lệnh xuất hàng", "Mã hàng" };

            tuNgay.EditValue = DateTime.Now;
            denNgay.EditValue = DateTime.Now;

            checkSK.Checked = true;
            checkEditTg.Checked = true;
            isCheckFirst = true;
            comboBoxEdit1.Properties.Items.Clear();
            CreateDefault();
            loadTenKho();
            foreach (string item in items)
            {
                comboBoxEdit1.Properties.Items.Add(item);
            }
            comboBoxEdit1.EditValue = items[1].ToString();

        }
        private void CreateDefault()
        {
            searchLookUpEdit11.Properties.ValueMember = "MaDVSX";
            searchLookUpEdit11.Properties.DisplayMember = "TenDVSX";
            searchLookUpEdit11.Properties.NullText = "[Chọn Kho]";

            searchLookUpEdit1.Properties.DisplayMember = "Display";
            searchLookUpEdit1.Properties.ValueMember = "MaHang";
            searchLookUpEdit1.Properties.NullText = "[Chọn mã hàng]";
        }
        public void loadTenKho()
        {
            string url = $"{URL}BienBanLuuSeal/Get?Action=GetTenKho&para1=a&para2=a&para3=a&para4=a&para5=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit11.Properties.DataSource = tbl;
        }
        private void searchLookUpEdit11_EditValueChanged(object sender, EventArgs e)
        {
            //_dvsx = searchLookUpEdit11.EditValue.ToString();
            //LoadDS();
        }
        private void LoadDS()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                DataRow drMaHang = grvDonHang.GetFocusedDataRow();
                if (drMaHang != null && searchLookUpEdit1.EditValue != null)
                    _maDH = drMaHang["MaHang"].ToString();
                else
                    _maDH = "";
                if (isCoKhacHang)
                {
                    if (searchLookUpEdit2.EditValue != null)
                        _maKH = searchLookUpEdit2.EditValue.ToString();
                    else
                        _maKH = "";
                }
                _dvsx = searchLookUpEdit11.EditValue == null ? "" : searchLookUpEdit11.EditValue.ToString();
                string url = $"{URL}ThongKeDongThung/Get?Action=GetTheKhoNew&Para1={_maKH}&Para2={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}&Para3={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}&Para4={_maDH}&Para5={_dvTinh}&Para6={cbLocValue}&Para7={maPKL}&Para8={_dvsx}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl.Rows.Count == 0)
                {
                    grcTheKho.DataSource = null;
                    grcCTNK.DataSource = null;
                    grcCTXH.DataSource = null;
                }
                else
                {
                    tbl = tbl.AsEnumerable().OrderBy(x => Convert.ToDateTime(x["NgayThang"])).ThenBy(x => Convert.ToInt32(x["LuyKeNgay"])).CopyToDataTable();
                    _ngayThang = tbl.Rows[0]["NgayThang"].ToString();
                    _donhang = tbl.Rows[0]["MaDH"].ToString();
                    LoadChiTietNK(_ngayThang, _donhang);
                    LoadChiTietXH(_ngayThang, _donhang);
                    grcTheKho.DataSource = tbl;
                    //addColumnLuyKe(tbl);
                    LoadTTTon();
                }

            }
            catch (Exception)
            {

            }
        }
        private void LoadTTTon()
        {
            try
            {
                string mahangTon = "";
                DataRow dataRow = grvDonHang.GetFocusedDataRow();
                if (dataRow != null && searchLookUpEdit1.EditValue != null)
                    mahangTon = dataRow["MaHang"].ToString();
                _dvsx = searchLookUpEdit11.EditValue == null ? "" : searchLookUpEdit11.EditValue.ToString();
                string url = $"{URL}ThongKeDongThung/Get?Action=GetSLTon&Para1={_maKH}&Para2={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}&Para3={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}" +
                    $"&Para4={mahangTon}&Para5={_dvTinh}&Para6={cbLocValue}&Para7={maPKL}&Para8={_dvsx}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblTonKho = JsonConvert.DeserializeObject<DataTable>(json);

                if (tblTonKho.Rows.Count == 0)
                {
                    grcTTTon.DataSource = null;
                }
                else
                {
                    grcTTTon.DataSource = tblTonKho;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
       
        private void addColumnLuyKe(DataTable tbl)
        {
            try
            {
                tbl.Columns.Add("LuyKe", typeof(int));
                int LuyKe = 0;
                foreach (DataRow item in tbl.Rows)
                {
                    int SLNhap = Convert.ToInt32(item["SLNhap"]);
                    int SLXuat = Convert.ToInt32(item["SLXuat"]);
                    int LuyKeNew = (SLNhap - SLXuat) + LuyKe;
                    LuyKe = LuyKeNew;
                    item["LuyKe"] = LuyKe;
                }
            }
            catch (Exception ex)
            {

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
            string url = $"{URL}ThongKeDongThung/GetNK?Action=GetDHPhieuNhap&Para1={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}&Para2={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}&Para3=a&Para4=A&Para5={string.Join(";", GlobleData.lstDVSX)}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0) return;

            searchLookUpEdit1.Properties.DataSource = tbl;

            donhangtong = tbl.Rows[0]["MaHang"].ToString();
        }
        private void LoadXuatHang()
        {
            string url = $"{URL}ThongKeDongThung/GetNK?Action=GetLenhXuatHang&Para1={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}&Para2={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}&Para3=a&Para4=A&Para5={string.Join(";", GlobleData.lstDVSX)}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0) return;

            searchLookUpEdit3.Properties.DataSource = tbl;
            searchLookUpEdit3.Properties.DisplayMember = "MaPKL_XH";
            searchLookUpEdit3.Properties.ValueMember = "MaPKL_XH";
            searchLookUpEdit3.EditValue = tbl.Rows[0]["MaPKL_XH"].ToString();
            maPKL = tbl.Rows[0]["MaPKL_XH"].ToString();
        }
        private void LoadKH()
        {
            string maDH = "";
            DataRow drMaHang = grvDonHang.GetFocusedDataRow();
            if (drMaHang != null && searchLookUpEdit1.EditValue != null)
                maDH = drMaHang["MaHang"].ToString();
            string url = $"{URL}ThongKeDongThung/GetNK?Action=GetKHPhieuNhap&Para1={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}&Para2={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}&Para3={maDH}&Para4=A&Para5={string.Join(";", GlobleData.lstDVSX)}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null) return;
            searchLookUpEdit2.Properties.DataSource = tbl;
            searchLookUpEdit2.Properties.DisplayMember = "Display";
            searchLookUpEdit2.Properties.ValueMember = "MaKH";
        }

        private void bandviewTheKho_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dataRow = bandviewTheKho.GetFocusedDataRow();
            if (dataRow == null) return;
            string _ngayThang1 = dataRow["NgayThang"].ToString();
            string _donhang2 = dataRow["MaDH"].ToString();
            LoadChiTietNK(_ngayThang1, _donhang2);
            LoadChiTietXH(_ngayThang1, _donhang2);

        }


        private void LoadChiTietNK(string ngaythang, string madh)
        {
            string url = $"{URL}ThongKeDongThung/Get?Action=GetChiTietNhaphang&Para1={Convert.ToDateTime(ngaythang).ToString("yyyy-MM-dd")}&Para2={madh}&Para3=a&Para4=A&Para5=A";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0)
            {
                grcCTNK.DataSource = null;
                return;
            }
            else
            {
                grcCTNK.DataSource = tbl;
                KHDongThungLib.CreateBandSize(tbl, grvCTNK, gbSizeNK, repotxtN0, false);
            }

        }


        private void LoadChiTietXH(string ngaythang, string madh)
        {
            string mapklNew = "";
            if (cbLocValue != 2)
                mapklNew = "";
            else mapklNew = maPKL;
            string url = $"{URL}ThongKeDongThung/GetCTXHTK?Para1={Convert.ToDateTime(ngaythang).ToString("yyyy-MM-dd")}&Para2={madh}&Para3={mapklNew}&Para4=A&Para5=A";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
            {
                KHDongThungLib.ClearBand(gbSizeXH);
                gbSizeXH.Width = 60;
                grcCTXH.DataSource = null;
                return;
            }
            else
            {
                grcCTXH.DataSource = tbl;
                KHDongThungLib.CreateBandSize(tbl, grcChiTietXH, gbSizeXH, repotxtN0, false);
            }
        }
        private void dateEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {

            _tuNgay = Convert.ToDateTime(tuNgay.EditValue).ToString("yyyy-MM-dd");
            if (!isCheckFirst)
                return;
            if (cbLocValue == 1)
            {
                LoadDH();
                LoadKH();
            }
            else LoadXuatHang();
        }
        private void dateEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {

            _denNgay = Convert.ToDateTime(denNgay.EditValue).ToString("yyyy-MM-dd");
            if (!isCheckFirst)
                return;
            if (cbLocValue == 1)
            {
                LoadDH();
                LoadKH();
            }
            else LoadXuatHang();


        }
        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            isCoKhacHang = false;
            _maKH = "";
            LoadKH();

        }
        private void checkEditTg_CheckedChanged(object sender, EventArgs e)
        {
            if (!isCheckFirst)
                return;
            if (checkEditTg.Checked)
            {
                layoutControlItem_Tungay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem_Denngay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                _tuNgay = tuNgay.EditValue.ToString(); _denNgay = denNgay.EditValue.ToString();
                searchLookUpEdit1.EditValue = "";
                if (cbLocValue == 1)
                {
                    LoadDH();
                    LoadKH();
                }
                else LoadXuatHang();
            }
            else
            {
                layoutControlItem_Tungay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem_Denngay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                _tuNgay = "1990-01-01"; _denNgay = DateTime.Now.ToString("yyyy-MM-dd");
                if (cbLocValue == 1)
                {
                    LoadDH();
                    LoadKH();
                    searchLookUpEdit1.EditValue = donhangtong;
                }
                else LoadXuatHang();
            }
        }
     

        private void grvCTNK_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }

        private void btnLocDH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadKH();
        }

        private void grcTheKho_Click(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit3_Properties_EditValueChanged(object sender, EventArgs e)
        {
            DataRow dr = grvLenhxuathang.GetFocusedDataRow();
            if (dr == null) return;
            maPKL = dr["MaPKL_XH"].ToString();
        }

        private void LocDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDS();
        }



        private void checkValueLoc(string cbxText)
        {
            try
            {
                LoadXuatHang();
                if (!isCheckFirst)
                    return;
                maPKL = cbxText;
                if (cbxText == "Lệnh xuất hàng")
                {
                    searchLLenhXH.Visibility = LayoutVisibility.Always;
                    layoutControlItem1.Visibility = LayoutVisibility.Never;
                    layoutControlItem2.Visibility = LayoutVisibility.Never;
                    cbLocValue = 2;

                }
                else
                {
                    layoutControlItem1.Visibility = LayoutVisibility.Always;
                    layoutControlItem2.Visibility = LayoutVisibility.Always;
                    searchLLenhXH.Visibility = LayoutVisibility.Never;
                    cbLocValue = 1;
                }
                if (cbLocValue == 1)
                {
                    LoadDH();
                    LoadKH();
                }
                else LoadXuatHang();
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

            if (checkSL.Checked)
            {
                _dvTinh = "1";
                checkSK.Checked = false;

            }
            else
            {
                _dvTinh = "2";
                checkSK.Checked = true;

            }
            if (!isCheckFirst)
                return;
        }

        private void btNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit2.EditValue = null;
            checkEditTg.Checked = true;
        }

        private void checkSK_EditValueChanged(object sender, EventArgs e)
        {

            if (checkSK.Checked)
            {
                checkSL.Checked = false;
                _dvTinh = "2";
            }
            else
            {
                _dvTinh = "1";
                checkSL.Checked = true;

            }
            if (!isCheckFirst)
                return;
            //checkSL.Checked = false;
        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            isCoKhacHang = true;
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

        private void grvCTNK_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = grcCTNK.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }
        private void grcChiTietXH_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = grcCTXH.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }
        private void grcChiTietXH_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
    }
}