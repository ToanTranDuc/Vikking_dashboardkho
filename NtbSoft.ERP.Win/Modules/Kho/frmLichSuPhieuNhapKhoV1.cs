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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmLichSuPhieuNhapKhoV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _soLo = string.Empty, _cayVai = string.Empty;
        public frmLichSuPhieuNhapKhoV1()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            string[] items = new string[] { "Thời gian", "Lô" };

            foreach (string item in items)
            {
                comboBoxEdit11.Properties.Items.Add(item);
            }
            comboBoxEdit11.EditValue = items[0].ToString();
            tuNgay.EditValue = DateTime.Now;
            denNgay.EditValue = DateTime.Now;
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateDefault();
        }
        private void CreateDefault()
        {

            searchLookUpEdit1.Properties.ValueMember = "SoLoID";
            searchLookUpEdit1.Properties.DisplayMember = "Display";
            searchLookUpEdit1.Properties.NullText = "[Chọn lô]";
        }
        //private void CreateSearchLookUpSoLo()
        //{
        //    dateEditNgayNhapKho.EditValue = DateTime.Now;
        //    string url = string.Format("{0}?", URL + "PhieuNhapKho/GetSoLoLS");
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]") return;
        //    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
        //    searchLookUpEditSoLo.Properties.DataSource = tbl;
        //    searchLookUpEditSoLo.Properties.DisplayMember = "SoLo";
        //    searchLookUpEditSoLo.Properties.ValueMember = "SoLoID";
        //    searchLookUpEditSoLo.RefreshEditValue();
        //    searchLookUpEditSoLo.Refresh();
        //}
        //private void LoadData()
        //{
        //    string fromDate = Convert.ToDateTime(barEditItemFromDate.EditValue).ToString("yyyy-MM-dd");
        //    string toDate = Convert.ToDateTime(barEditItemToDate.EditValue).ToString("yyyy-MM-dd");
        //    if (barEditItemFromDate.EditValue == null || barEditItemToDate.EditValue == null) return;
        //    string url = string.Format("{0}?fromDate={1}&&toDate={2}", URL + "PhieuNhapKho/GetLichSu", fromDate.ToString(), toDate.ToString());
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        gridControl1.DataSource = null;
        //        gridControl2.DataSource = null;
        //        textBoxSoChungTu.Text = "";
        //        textBoxSoHopDong.Text = "";
        //        textBoxBienBanKiem.Text = "";
        //        dateEditNgayChungTu.EditValue = null;
        //        dateEditNgayBBKiem.EditValue = null;
        //        dateEditNgayNhapKho.EditValue = null;
        //        searchLookUpEditSoLo.EditValue = null;

        //        return;
        //    }
        //    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
        //    gridControl1.DataSource = tbl;
        //}

        private void btLoad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (barEditItemFromDate.EditValue != null && barEditItemToDate.EditValue != null)
            //{
            //    DateTime _fromDate = Convert.ToDateTime(barEditItemFromDate.EditValue);
            //    DateTime _toDate = Convert.ToDateTime(barEditItemToDate.EditValue);
            //    int kq = DateTime.Compare(_fromDate, _toDate);
            //    if (kq > 0)
            //    {
            //        XtraMessageBox.Show("Giá trị Từ ngày phải nhỏ hơn giá trị Đến ngày!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //    string fromDate = string.Empty, toDate = string.Empty;
            //    fromDate = barEditItemFromDate.EditValue.ToString();
            //    toDate = barEditItemToDate.EditValue.ToString();
            //    //LoadData();
            //}
            if (comboBoxEdit11.EditValue.ToString() == "Lô")
            {
                GetReport(1);
            }
            else
            {
                GetReport(2);
            }
        }
        //private void LoadChiTiet()
        //{
        //    object sophieu = null;

        //    if (gridView1.IsGroupRow(gridView1.FocusedRowHandle))
        //    {
        //        int childHandle = gridView1.GetChildRowHandle(gridView1.FocusedRowHandle, 0);
        //        sophieu = gridView1.GetRowCellValue(childHandle, gridColumn11);
        //    }
        //    else
        //    {
        //        sophieu = gridView1.GetFocusedRowCellValue(gridColumn11);
        //    }
        //    string url = string.Format("{0}?sophieu={1}", URL + "PhieuNhapKho/GetChiTietPhieu", sophieu == null ? "" : sophieu.ToString());
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        gridControl2.DataSource = null;
        //        return;
        //    }
        //    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
        //    var Rows = tbl.AsEnumerable()
        //     .GroupBy(row => new
        //     {
        //         SoLoID = row["SoLoID"],
        //         NhaCungCap = row["NhaCungCap"],
        //         SoChungTu = row["SoChungTu"],
        //         NgayChungTu = row["NgayChungTu"],
        //         SoBienBan = row["SoBienBan"],
        //         NgayBienBan = row["NgayBienBan"],
        //         SoHopDong = row["SoHopDong"]
        //     })
        //    .Select(group => group.First())
        //     .ToList();
        //    searchLookUpEditSoLo.EditValue = Rows[0]["SoLoID"].ToString();
        //    textBoxSoChungTu.Text = Rows[0]["SoChungTu"].ToString();
        //    textBoxSoHopDong.Text = Rows[0]["SoHopDong"].ToString();
        //    textBoxBienBanKiem.Text = Rows[0]["SoBienBan"].ToString();
        //    dateEditNgayChungTu.EditValue = Rows[0]["NgayChungTu"];
        //    dateEditNgayBBKiem.EditValue = Rows[0]["NgayBienBan"];
        //    gridControl2.DataSource = tbl;
        //}
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            //LoadChiTiet();
        }

        //private void simpleButton1_Click(object sender, EventArgs e)
        //{
        //    DataTable tbl = gridControl2.DataSource as DataTable;
        //    if (tbl == null) return;
        //    var rows = tbl.AsEnumerable()
        //                            .Where(row => row.IsNull("DonGia") || Convert.ToDecimal(row["DonGia"]) <= 0)
        //                            .ToList();

        //    if (rows.Any())
        //    {
        //        XtraMessageBox.Show("Đơn giá không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    DataTable dtSave = new DataTable();
        //    dtSave.Columns.Add("SoPhieu", typeof(string));
        //    dtSave.Columns.Add("NgayNhapKho", typeof(string));
        //    dtSave.Columns.Add("SoLoID", typeof(string));
        //    dtSave.Columns.Add("MaVTID", typeof(string));
        //    dtSave.Columns.Add("MaMauVT", typeof(string));
        //    dtSave.Columns.Add("MaDVVT", typeof(string));
        //    dtSave.Columns.Add("KhoVaiID", typeof(string));
        //    dtSave.Columns.Add("TheoCT", typeof(decimal));
        //    dtSave.Columns.Add("ThucNhap", typeof(decimal));
        //    dtSave.Columns.Add("DonGia", typeof(decimal));
        //    dtSave.Columns.Add("ThanhTien", typeof(decimal));
        //    dtSave.Columns.Add("GhiChu", typeof(string));
        //    dtSave.Columns.Add("STT", typeof(int));
        //    dtSave.Columns.Add("NguoiTao", typeof(string));
        //    foreach (DataRow dr in tbl.Rows)
        //    {

        //        DataRow _dr = dtSave.NewRow();
        //        _dr["SoPhieu"] = dr["SoPhieu"]; 
        //        _dr["NgayNhapKho"] = Convert.ToDateTime(dateEditNgayNhapKho.EditValue).ToString("dd-MM-yyyy");
        //        _dr["SoLoID"] = dr["SoLoID"];
        //        _dr["MaVTID"] = dr["MaVTID"];
        //        _dr["MaMauVT"] = dr["MaMauVT"];
        //        _dr["MaDVVT"] = dr["MaDVVT"];
        //        _dr["KhoVaiID"] = dr["KhoVaiID"];
        //        _dr["TheoCT"] = dr["TheoCT"];
        //        _dr["ThucNhap"] = dr["ThucNhap"];
        //        _dr["DonGia"] = dr["DonGia"];
        //        _dr["ThanhTien"] = dr["ThanhTien"];
        //        _dr["GhiChu"] = dr["GhiChu"];
        //        _dr["STT"] = 0;
        //        _dr["NguoiTao"] = GlobleData.UserName;
        //        dtSave.Rows.Add(_dr);
        //    }
        //    string url = string.Format("{0}?", URL + "PhieuNhapKho/Post");
        //    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
        //    if (result.ToLower() != "true")
        //        XtraMessageBox.Show(result);
        //    else
        //    {
        //        clsWaitForm.ShowSuccessForm(this, 2000);
        //        LoadChiTiet();
        //    }
        //}

        private void gridView2_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "DonGia" && e.Value.ToString() != "")
            {
                decimal theoCT = Convert.ToDecimal(gridView2.GetRowCellValue(e.RowHandle, "TheoCT"));
                decimal donGia = Convert.ToDecimal(e.Value);
                decimal thanhTien = theoCT * donGia;
                gridView2.SetRowCellValue(e.RowHandle, "ThanhTien", thanhTien);
            }
        }

        //private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        object sophieu = null;

        //        if (gridView1.IsGroupRow(gridView1.FocusedRowHandle))
        //        {
        //            int childHandle = gridView1.GetChildRowHandle(gridView1.FocusedRowHandle, 0);
        //            sophieu = gridView1.GetRowCellValue(childHandle, gridColumn11);
        //        }
        //        else
        //        {
        //            sophieu = gridView1.GetFocusedRowCellValue(gridColumn11);
        //        }
        //        if (sophieu == null) return;
        //        DialogResult messResult = MessageBox.Show("Bạn có muốn xóa số phiếu này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (messResult == DialogResult.Yes)
        //        {


        //            string url = string.Format("{0}?sophieu={1}", URL + "PhieuNhapKho/Delete", sophieu == null ? "" : sophieu.ToString());
        //            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
        //            if (result.ToLower() == "true")
        //                LoadData();
        //            else XtraMessageBox.Show(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    } 

        //}

        private void gridView2_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView2.FocusedColumn.FieldName.Contains("@"))
            {
                int outParse = -1;
                if (e != null && e.Value != null)
                {
                    bool flagParse = int.TryParse(e.Value.ToString(), out outParse);
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Value = 0;
                    }
                    else if (flagParse && outParse < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập > 0!.";
                        return;
                    }
                    else if (!flagParse)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!.";
                        return;
                    }
                }
            }
        }

       

        public void Export(string TemplateFileName, string ExportFileName, DataTable tbl)
        {
            try
            {
                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                List<string> SoLo = tbl.AsEnumerable().Select(x => x["SoLoID"].ToString()).Distinct().ToList();

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                    FileInfo templateFile = new FileInfo(TemplateFileName);


                    for (int i = 0; i < SoLo.Count; i++)
                    {
                        string soLo = SoLo[i].ToString();


                        using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                        {
                            DataTable tbl2 = tbl.AsEnumerable().Where(x => x["SoLoID"].ToString() == soLo).CopyToDataTable();
                          
                            ExcelWorksheet templateSheet = templatePackage.Workbook.Worksheets["Sheet1"];

                            ExcelWorksheet newSheet = excelPackage.Workbook.Worksheets.Add(tbl2.Rows[0]["SoLo"].ToString(), templateSheet);

                           

                            newSheet.Cells["B8"].Value = tbl2.Rows[0]["SoLo"].ToString();
                            newSheet.Cells["C5"].Value = tbl2.Rows[0]["SoChungTu"].ToString();
                            newSheet.Cells["C7"].Value = tbl2.Rows[0]["SoHopDong"].ToString();
                            newSheet.Cells["D6"].Value = tbl2.Rows[0]["SoBienBan"].ToString();
                            newSheet.Cells["j5"].Value = Convert.ToDateTime(tbl2.Rows[0]["NgayChungTu"]).ToString("dd/MM/yyyy");
                            newSheet.Cells["j6"].Value = Convert.ToDateTime(tbl2.Rows[0]["NgayBienBan"]).ToString("dd/MM/yyyy");
                            int row = 10;
                            
                            newSheet.InsertRow(10, tbl2.Rows.Count);
                            foreach (DataRow item in tbl2.Rows)
                            {
                                newSheet.Cells[row, 1].Value = row - 9;
                                newSheet.Cells[row, 2, row, 5].Value = item["ChiTiet"];
                                newSheet.Cells[row, 2, row, 5].Merge = true;
                                newSheet.Cells[row, 6].Value = item["KhoVai"];
                                newSheet.Cells[row, 7].Value = item["MauVT"];
                                newSheet.Cells[row, 8].Value = item["TenDVVT"];
                                newSheet.Cells[row, 9].Value = item["TheoCT"];
                                newSheet.Cells[row, 10].Value = item["ThucNhap"];
                                newSheet.Cells[row, 11].Value = item["DonGia"];
                                newSheet.Cells[row, 12].Value = item["ThanhTien"];
                                newSheet.Cells[row, 13].Value = item["MaHaiQuan"];
                                newSheet.Cells[row, 14].Value = item["GhiChu"];
                                newSheet.Cells[row, 14, row, 17].Merge = true;
                                row++;
                               
                            }
                            float sumTT = tbl2.AsEnumerable().Sum(x => Convert.ToSingle(x["ThanhTien"]));
                            newSheet.Cells[row, 12].Value =Math.Round(sumTT,2);
                            var borderData = newSheet.Cells[10, 1, row - 1, 17].Style.Border;
                            borderData.Bottom.Style =
                                borderData.Top.Style =
                                borderData.Left.Style =
                                borderData.Right.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    // Save all sheets into one file
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                // Bạn nên log hoặc throw lỗi để dễ debug
                Console.WriteLine(ex.Message);
            }
        }

        private void gridView2_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;


            if (column.FieldName == "TheoCT" || column.FieldName == "ThucNhap" || column.FieldName == "DonGia" || column.FieldName == "ThanhTien")
            {
                var value = view.GetFocusedValue();

                if (value != null &&
                    ((value is int && (int)value == 0) ||
                     (value is decimal && (decimal)value == 0) ||
                     (value is double && (double)value == 0) ||
                     (value is float && (float)value == 0)))
                {

                    view.SetFocusedValue(null);
                }
            }
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValuesLo = string.Join(";",
                 searchLookUpEdit1View.GetSelectedRows()
                     .Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, "SoLoID"))
             );
            searchLookUpEdit1.EditValue = selectedValuesLo;
            _soLo = selectedValuesLo;
            string selectedValuesCayVai = string.Join(";",
                searchLookUpEdit1View.GetSelectedRows()
                    .Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, "MaNPL"))

            );
            _cayVai = selectedValuesCayVai;
            if (searchLookUpEdit1.EditValue is null) return;

            if (comboBoxEdit11.EditValue.ToString() == "Lô")
            {
                GetReport(1);
            }
            else
            {
                GetReport(2);
            }
            
        }
        string selectedValuessLo = "";
        private void searchLookUpEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessLo = string.Join(";",
                searchLookUpEdit1View.GetSelectedRows()
                    .Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, "Display")));
            if (string.IsNullOrEmpty(selectedValuessLo))
            {
                e.DisplayText = "[Chọn lô]";
            }
            else
            {
                e.DisplayText = selectedValuessLo;
            }
        }
        private void LoadLo(int value)
        {
            if (value == 2 && (tuNgay.EditValue == null || denNgay.EditValue == null)) {
                searchLookUpEdit1.Properties.DataSource = null;
                gridControl2.DataSource = null;
                return;
            };
            string tungay = value == 1 ? "1990-01-01" : Convert.ToDateTime(tuNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string denngay = value == 1 ? "1990-01-01" : Convert.ToDateTime(denNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string url =  $"{URL}PhieuNhapKho/GetMV1?action=GetSearchLo&para={tungay}&Para1={denngay}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                searchLookUpEdit1.Properties.DataSource = null;
                return;
            }   
             searchLookUpEdit1.Properties.DataSource = tbl;
            if(value ==2)
            {
                GetReport(value);

            }
        }

        private void comboBoxEdit11_EditValueChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit11.EditValue.ToString() == "Lô")
            {
                layoutControlItem_Tungay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem_Denngay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEdit1.Properties.DataSource = null;
                gridControl2.DataSource = null;
                LoadLo(1);
            }
            else
            {
                layoutControlItem_Tungay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem_Denngay.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                searchLookUpEdit1.Properties.DataSource = null;
                gridControl2.DataSource = null;
                LoadLo(2);
            }
        }

        private void tuNgay_EditValueChanged(object sender, EventArgs e)
        {
            gridControl2.DataSource = null;
            LoadLo(2);
        }

        private void denNgay_EditValueChanged(object sender, EventArgs e)
        {
            gridControl2.DataSource = null;
            LoadLo(2);
        }
        private void GetReport(int value)
        {
            string checkGet = "0";
            if (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() =="")
            {
                checkGet = "1";
            }
            string tungay = value == 1 ? "1990-01-01" : Convert.ToDateTime(tuNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string denngay = value == 1 ? "1990-01-01" : Convert.ToDateTime(denNgay.EditValue.ToString()).ToString("yyyy-MM-dd");
            string url =  $"{URL}PhieuNhapKho/GetMV1?action=GetReport&para={_soLo}&Para1={_cayVai}&para3={tungay}&para4={denngay}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if(tbl == null || tbl.Rows.Count == 0)
            {
                gridControl2.DataSource = null;
                return;
            }
            gridControl2.DataSource = tbl;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = gridControl2.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lô ");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("PhieuNhapKho{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "PhieuNhapKhoV1.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, tbl);

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

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //DataRow dr = gridView2.GetFocusedDataRow();
            //if (dr == null) return;
            //frmPhieuNhapKhoV1 frm = new frmPhieuNhapKhoV1(dr["SoLoID"].ToString(), Convert.ToDateTime(dr["NgayNhapKho"].ToString()).ToString("yyyy-MM-dd"));
            //frm.WindowState = FormWindowState.Maximized;
            //frm.Show();
            //if (comboBoxEdit11.EditValue.ToString() == "Lô")
            //{
            //    GetReport(1);
            //}
            //else
            //{
            //    GetReport(2);
            //}
        }
    }
}
