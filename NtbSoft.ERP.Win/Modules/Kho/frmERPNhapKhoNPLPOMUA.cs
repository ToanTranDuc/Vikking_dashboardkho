using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.POMuaHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPLPOMUA : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _tenTau = "";
        public frmERPNhapKhoNPLPOMUA()
        {
            InitializeComponent();


            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }
        protected override void OnLoad(EventArgs e)
        {
          
            loadTQ();
            base.OnLoad(e);
        }
        //private void loadTQ()
        //{
        //    try
        //    {
        //        string kh = searchLookUpEditKH.EditValue == null ? "ALL" : searchLookUpEditKH.EditValue.ToString();
        //        string mh = searchLookUpEditMH.EditValue == null ? "ALL" : searchLookUpEditMH.EditValue.ToString();
        //        string tt = searchLookUpEditTT.EditValue == null ? "ALL" : searchLookUpEditTT.EditValue.ToString();
        //        string urlTau = $"{URL}ERPNhapKhoNPLPOMUA/Get?Action=GETTONGPOMUA&para1={kh}&para2={mh}&para3={tt}";
        //        string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
        //        if (jsonTau == "[]")
        //        {
        //            gC.DataSource = null;
        //        }
        //        else
        //        {
        //            DataTable tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);
        //            if (!tblTau.Columns.Contains("btnNhapKho"))
        //                tblTau.Columns.Add("btnNhapKho", typeof(string));
        //            gC.DataSource = tblTau;

        //            DataTable tblDetail = loadDataDetail();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        private void loadTQ()
        {
            try
            {
                string kh =  "ALL";
                string mh = "ALL";
                string tt = "ALL";

                string urlTau = $"{URL}ERPNhapKhoNPLPOMUA/Get?Action=GETTONGPOMUA&para1={kh}&para2={mh}&para3={tt}";
                string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;

                if (jsonTau == "[]")
                {
                    gC.DataSource = null;
                }
                else
                {
                    DataTable tblMaster = JsonConvert.DeserializeObject<DataTable>(jsonTau);
                    if (!tblMaster.Columns.Contains("btnNhapKho"))
                        tblMaster.Columns.Add("btnNhapKho", typeof(string));

                    DataSet ds = new DataSet();
                    ds.Tables.Add(tblMaster);

                    // Load chi tiết
                    DataTable tblDetail = loadDataDetail();
                    if (tblDetail != null)
                    {
                        ds.Tables.Add(tblDetail);

                        // Tạo quan hệ Master-Detail (giả sử dựa trên cột POMua)
                        DataRelation relation = new DataRelation("gridViewDetail",
                            ds.Tables[0].Columns["SoLoID"], // Cột khóa ở bảng Master
                            ds.Tables[1].Columns["SoLoID"],
                            false// Cột khóa ở bảng Detail
                        );
                        ds.Relations.Add(relation);
                    }
                    if (!ds.Tables[0].Columns.Contains("NgayGHTT_DT"))
                    {
                        ds.Tables[0].Columns.Add("NgayGHTT_DT", typeof(DateTime));
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (DateTime.TryParseExact(row["NgayDuKienHV"].ToString(), "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                            {
                                row["NgayGHTT_DT"] = parsed;
                            }
                        }
                    }
                    // Thêm vào loadTQ() khi xử lý tblMaster
                    if (!tblMaster.Columns.Contains("NgayGHTT_DT2"))
                    {
                        tblMaster.Columns.Add("NgayGHTT_DT2", typeof(DateTime));
                        foreach (DataRow row in tblMaster.Rows)
                        {
                            if (DateTime.TryParseExact(row["NgayGHTT"]?.ToString(), "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                            {
                                row["NgayGHTT_DT2"] = parsed;
                            }
                        }
                    }

                    if (!ds.Tables[0].Columns.Contains("SortOrder"))
                        ds.Tables[0].Columns.Add("SortOrder", typeof(int));

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string trangThai = row["TrangThai"]?.ToString().Trim() ?? "";
                        string chatLuong = row["ChatLuongQC"]?.ToString().Trim() ?? "";

                        if (chatLuong == "Pass" && trangThai == "Đã sẵn sàng NK")
                            row["SortOrder"] = 0; // Lên đầu
                        else if (chatLuong == "Pass" && trangThai == "Đã nhập kho")
                            row["SortOrder"] = 2; // Xuống cuối
                        else
                            row["SortOrder"] = 1; // Ở giữa (mặc định)
                    }

                    // ✅ Sắp xếp theo SortOrder


                    gC.DataSource = ds.Tables[0];
              
                  

                    filterGrid();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable loadDataDetail()
        {
            try
            {
                string kh = "ALL";
                string mh = "ALL";
                string tt = "ALL";

                // Gọi API lấy dữ liệu chi tiết
                string urlDetail = $"{URL}ERPNhapKhoNPLPOMUA/Get?Action=GETDETAILPOMUA&para1={kh}&para2={mh}&para3={tt}";
                string jsonDetail = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDetail); }).Result;

                if (jsonDetail != "[]")
                {
                    DataTable tblDetail = JsonConvert.DeserializeObject<DataTable>(jsonDetail);
                    tblDetail.TableName = "DetailTable"; // Đặt tên cho bảng detail
                    return tblDetail;
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load detail: {ex.Message}");
                return null;
            }
        }
        private void filterGrid()
        {
            DateTime? date1 = dateEditTuNgay.EditValue as DateTime?;
            DateTime? date2 = dateEditDenNgay.EditValue as DateTime?;
            DateTime? date33 = date3.EditValue as DateTime?;
            DateTime? date44 = date4.EditValue as DateTime?;

            DataTable dt = gV.DataSource is DataView dv ? dv.Table : (DataTable)gV.DataSource;

            var filters = new List<string>();

            // Lọc NgayGHTT_DT (kiểu DateTime)
            if (date1.HasValue)
                filters.Add($"NgayGHTT_DT >= #{date1.Value:MM/dd/yyyy}#");
            if (date2.HasValue)
                filters.Add($"NgayGHTT_DT <= #{date2.Value:MM/dd/yyyy}#");

            // Lọc NgayGHTT_DT2 (kiểu DateTime - từ cột string NgayGHTT đã convert)
            if (date33.HasValue)
                filters.Add($"NgayGHTT_DT2 >= #{date33.Value:MM/dd/yyyy}#");
            if (date44.HasValue)
                filters.Add($"NgayGHTT_DT2 <= #{date44.Value:MM/dd/yyyy}#");

            dt.DefaultView.RowFilter = string.Join(" AND ", filters);

            //DateTime? date1 = dateEditTuNgay.EditValue as DateTime?;
            //DateTime? date2 = dateEditDenNgay.EditValue as DateTime?;
            //DateTime? date33 = date3.EditValue as DateTime?;
            //DateTime? date44 = date4.EditValue as DateTime?;

            //DataTable dt = gV.DataSource is DataView dv ? dv.Table : (DataTable)gV.DataSource;

            //var query = dt.AsEnumerable().Where(row =>
            //{
            //    // Lọc NgayGHTT_DT (kiểu DateTime)
            //    if (date1.HasValue || date2.HasValue)
            //    {
            //        var val = row["NgayGHTT_DT"];
            //        if (val == null || val == DBNull.Value) return false;
            //        DateTime ngayDT = Convert.ToDateTime(val);
            //        if (date1.HasValue && ngayDT.Date < date1.Value.Date) return false;
            //        if (date2.HasValue && ngayDT.Date > date2.Value.Date) return false;
            //    }

            //    // Lọc NgayGHTT (kiểu string)
            //    if (date33.HasValue || date44.HasValue)
            //    {
            //        var raw = row["NgayGHTT"]?.ToString();
            //        if (string.IsNullOrWhiteSpace(raw)) return false;
            //        if (!DateTime.TryParseExact(raw, "dd/MM/yyyy",
            //                System.Globalization.CultureInfo.InvariantCulture,
            //                System.Globalization.DateTimeStyles.None, out DateTime ngay))
            //            if (!DateTime.TryParse(raw, out ngay)) return false;

            //        if (date33.HasValue && ngay.Date < date33.Value.Date) return false;
            //        if (date44.HasValue && ngay.Date > date44.Value.Date) return false;
            //    }

            //    return true;
            //});

            //dt.DefaultView.RowFilter = "";
            //var resultTable = query.Any() ? query.CopyToDataTable() : dt.Clone();
            //gV.GridControl.DataSource = resultTable;

        }
   
        private void btnLoc_Click(object sender, EventArgs e)
        {
            loadTQ();
        }

        private void repobtnNhapKho_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow dr = gV.GetFocusedDataRow();
            if (dr == null) return;

            frmERPNhapKhoNPLSua frm = new frmERPNhapKhoNPLSua(dr);
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

    
        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
        }
        private void NapLai()
        {
           
            loadTQ();
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPNhapKhoNPL frm = new frmERPNhapKhoNPL();
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

        private void btnSuaLo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmERPNhapKhoNPLSua frm = new frmERPNhapKhoNPLSua();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

        private void btnSuaVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmERPNhapKhoNPLSuaCay frm = new frmERPNhapKhoNPLSuaCay();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
            frm.FormClosed += (s, args) =>
            {
                loadTQ();
            };
        }

        private void gV_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            string chatLuongQC = Convert.ToString(view.GetRowCellValue(e.RowHandle, "ChatLuongQC"));
            int sortOrder = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, "SortOrder"));

            if (chatLuongQC == "Fail")
            {
                e.Appearance.ForeColor = Color.Red;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
            else if (sortOrder == 2)
            {
                // Giữ nguyên màu đen mặc định, không set gì thêm
                e.Appearance.ForeColor = Color.Black;
            }
            else
            {
                e.Appearance.ForeColor = Color.Blue;
            }
        }

        private void gV_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "IsDuyetNK")
            {
                var oldValue = gV.GetRowCellValue(e.RowHandle, "IsDuyetNK");
                var chatLuongQC = gV.GetRowCellValue(e.RowHandle, "ChatLuongQC");
                var slNhap = gV.GetRowCellValue(e.RowHandle, "SLNhap");
                //XuLyDuyetNK();
                if (chatLuongQC != null && chatLuongQC.ToString() == "Pass" &&
                    slNhap != null && Convert.ToDecimal(slNhap) > 0)
                {
                    // Gọi hàm xử lý
                    XuLyDuyetNK();
                }
                else
                {
                    // Thông báo
                    XtraMessageBox.Show("Không thể duyệt!\nPO chưa kiểm tra Chất lượng/Số lượng. Vui lòng kiểm tra lại !!!",
                                      "Thông báo",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);

                    int rowHandle = e.RowHandle;
                    this.BeginInvoke(new Action(() =>
                    {
                        gV.SetRowCellValue(rowHandle, "IsDuyetNK", oldValue);
                    }));
                }
            }
        }
        private void XuLyDuyetNK()
        {
            if (GlobleData.UserName.ToLower() == "admin" || GlobleData.UserName.ToString() == "QuanLyKhoNPL" || GlobleData.UserName.ToString() == "QLDH_admin")
            {
                DataRow dr = gV.GetFocusedDataRow();
                if (dr == null) return;

                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTDUYET&para={GlobleData.UserName}&para2={dr["SoLoID"].ToString()}&para3={dr["POMua"].ToString()}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1000);
                }
            }

        }

        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("TheodoiKhoNPL{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateNhapKhoPOMUA.xlsx";
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
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                FileInfo templateFile = new FileInfo(TemplateFileName);
                FileInfo resultFile = new FileInfo(ExportFileName);

                using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                {
                    templatePackage.Workbook.Properties.Author = "NTB";
                    templatePackage.Workbook.Properties.Title = "NhapKhoNPL";
                    templatePackage.SaveAs(resultFile);
                }

               

                var view = gC.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;


                List<DataRow> visibleRows = new List<DataRow>();
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    int rowHandle = view.GetVisibleRowHandle(i);
                    if (rowHandle < 0) continue; // bỏ qua group row
                    DataRow row = view.GetDataRow(rowHandle);
                    if (row != null)
                        visibleRows.Add(row);
                }

                if (visibleRows.Count == 0) return;

                using (ExcelPackage excelPackage = new ExcelPackage(resultFile))
                {
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    int startRowInExcel = 8;

                    for (int i = 0; i < visibleRows.Count; i++)
                    {
                        DataRow row = visibleRows[i];
                        int excelRow = startRowInExcel + i;

                        worksheet.Cells[excelRow, 1].Value = row["TenPhieu"];
                        worksheet.Cells[excelRow, 2].Value = row["TenKH"];
                        worksheet.Cells[excelRow, 3].Value = row["TenHang"];
                        worksheet.Cells[excelRow, 4].Value = row["POMua"];
                        worksheet.Cells[excelRow, 5].Value = row["TenNCC"];
                        //worksheet.Cells[excelRow, 6].Value = row["TrangThai"];
                        string trangThai = Convert.ToString(row["TrangThai"]);
                        worksheet.Cells[excelRow, 6].Value = trangThai;
                        if (trangThai == "Chưa nhập kho")
                            worksheet.Cells[excelRow, 6].Style.Font.Color.SetColor(Color.Red);
                        else if (trangThai == "Đang nhập kho")
                            worksheet.Cells[excelRow, 6].Style.Font.Color.SetColor(Color.Green);
                        else if (trangThai == "Đã nhập kho")
                            worksheet.Cells[excelRow, 6].Style.Font.Color.SetColor(Color.Blue);

                        string chatLuong = Convert.ToString(row["ChatLuongQC"]);
                        worksheet.Cells[excelRow, 7].Value = chatLuong;
                        if (chatLuong == "Fail")
                            worksheet.Cells[excelRow, 7].Style.Font.Color.SetColor(Color.Red);
                        else if (chatLuong == "Đang kiểm")
                            worksheet.Cells[excelRow, 7].Style.Font.Color.SetColor(Color.Green);
                        else if (chatLuong == "Pass")
                            worksheet.Cells[excelRow, 7].Style.Font.Color.SetColor(Color.Blue);

                        //worksheet.Cells[excelRow, 7].Value = row["ChatLuongQC"];
                        worksheet.Cells[excelRow, 8].Value = row["IsDuyetNK"].ToString() =="True" ? "Đã duyệt NK":"Chưa duyệt";
                        worksheet.Cells[excelRow, 9].Value = row["NgayDuKienHV"];
                        worksheet.Cells[excelRow, 10].Value = row["NgayGHTT"];
                        worksheet.Cells[excelRow, 11].Value = row["SLMua"];
                        worksheet.Cells[excelRow, 12].Value = row["SLNhap"];
                        worksheet.Cells[excelRow, 13].Value = row["SoLanNhap"];
                        worksheet.Cells[excelRow, 14].Value = row["GhiChu"];
                       
                    }

                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void gV_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = e.ListSourceRowIndex + 1;
            }
        }

        private void repoQC_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView detailView = gV.GetDetailView(gV.FocusedRowHandle, 0) as GridView;
            if (detailView == null) return;

            int focusedHandle = detailView.FocusedRowHandle;
            if (focusedHandle < 0) return;

            DataRowView drv = detailView.GetFocusedRow() as DataRowView;
            if (drv == null) return;

            DataRow dr = drv.Row;
            string soLoID = dr["SoLoID"].ToString();
            string maNPL = dr["MaNPL"].ToString();
            string isnpl = dr["IsNPL"].ToString();

            if(isnpl=="True")
            {
                var frm = new frmViewKiemNL(soLoID, "", maNPL);
                frm.ShowDialog();
            }
            else
            {
                var frm = new frmViewKiemPL(soLoID, "", maNPL);
                frm.ShowDialog();
            }
          
        }
    }
}
