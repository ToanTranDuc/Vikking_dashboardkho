using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmNhaCungCap : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        DataTable tbl = new DataTable();
        DataTable tbllichsu = new DataTable();
        DataTable tblDG = new DataTable();
        string _mancc = string.Empty, tenncc = string.Empty;
        public frmNhaCungCap()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchlookupNCC();
        }

        private void LoadData(string mancc)
        {
            string url = string.Format("{0}?mancc={1}", URL + "NhaCC/Get", mancc);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                gridControl2.DataSource = tbl;
            }
            else 
            {
                gridControl2.DataSource = null;
            }

        }


        private void gridView2_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LoadData();
        }

        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            //string mancc = gridView2.GetFocusedRowCellValue("MaNCC")?.ToString();
            //LoadLichSu(mancc);
            //CreateTableDG();
            //LoadDG(mancc);
        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }


        private async void LoadLichSu(string mancc)
        {
            try
            {
                string url = string.Format("{0}?mancc={1}", URL + "NhaCC/GetLS", mancc);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]")
                {
                    gridControl1.DataSource = null;
                }

                if (!string.IsNullOrEmpty(json))
                {
                    tbllichsu = JsonConvert.DeserializeObject<DataTable>(json);
                }

                gridControl1.DataSource = tbllichsu;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void CreateSearchlookupVatTu()
        {
            string url = $"{URL}NhaCC/GetPhieuBG?action=GETVATTU&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditvt = new RepositoryItemSearchLookUpEdit();
            rCountryEditvt.DataSource = tbl;
            rCountryEditvt.DisplayMember = "ChiTiet";
            rCountryEditvt.ValueMember = "MaVTID";
            rCountryEditvt.ShowClearButton = false;
            rCountryEditvt.NullText = "[Chọn giá trị]";

            GridView dvViewvt = rCountryEditvt.View;
            if (dvViewvt.Columns.Count == 0)
            {
                dvViewvt.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewvt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewvt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewvt.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewvt.Columns.Add(new GridColumn { FieldName = "MaVTID", Caption = "Mã vật tư", Name = "colMaHang", Visible = false });
                dvViewvt.Columns.Add(new GridColumn { FieldName = "ChiTiet", Caption = "Tên Vật Tư", Name = "colTenHang", Visible = true });

            }
            //gridColumn14.ColumnEdit = rCountryEditvt;

        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void CreateSearchlookupNCC()
        {
            string url = $"{URL}ERPThuVienNK/Get?Action=GETNCC";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit1.DataSource = tbl;
            repositoryItemSearchLookUpEdit1.ValueMember = "MaKH";
            repositoryItemSearchLookUpEdit1.DisplayMember = "TenKH";
            RepositoryItemSearchLookUpEdit rCountryEditvt = new RepositoryItemSearchLookUpEdit();
            rCountryEditvt.DataSource = tbl;
            rCountryEditvt.DisplayMember = "TenKH";
            rCountryEditvt.ValueMember = "MaKH";
            rCountryEditvt.ShowClearButton = false;
            rCountryEditvt.NullText = "[Chọn giá trị]";

            GridView dvViewvt = rCountryEditvt.View;
            if (dvViewvt.Columns.Count == 0)
            {
                dvViewvt.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewvt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewvt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewvt.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewvt.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã nhà cung cấp", Name = "colMaHang", Visible = false });
                dvViewvt.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên nhà cung cấp", Name = "colTenHang", Visible = true });

            }
            gridColumn2.ColumnEdit = rCountryEditvt;

        }

        private void gridView2_CustomDrawColumnHeader_1(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue != null);
            bool customTextHandled = false;

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                customTextHandled = true;  
            }

            if (info.Column == gridColumn17 || info.Column == gridColumn3)
            {
                info.GroupText = info.GroupValueText;
                customTextHandled = true;
            }

            // ===== Áp dụng màu theo group level =====
            int groupIndex = gridView2.GetRowLevel(e.RowHandle);
            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
            }

            if (customTextHandled)
            {
                // Tự vẽ text 
                e.DefaultDraw();
                e.Handled = true;
                return;
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("Bao Cao");

                    string fileName = "mauphieubaogia.xlsx";
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    //string phieu = gridView2.GetFocusedRowCellValue("Phieu")?.ToString();

                    int rowHandle = gridView2.FocusedRowHandle;
                    string phieu = null;

                    if (gridView2.IsGroupRow(rowHandle))
                    {
                        try
                        {
                            object gv = gridView2.GetGroupRowValue(rowHandle);
                            phieu = gv?.ToString();
                        }
                        catch
                        {
                            GridColumn phieuCol = gridView2.GroupedColumns
                            .FirstOrDefault(c => string.Equals(c.FieldName, "Phieu", StringComparison.OrdinalIgnoreCase));
                            if (phieuCol != null)
                            {
                                try
                                {
                                    object gv2 = gridView2.GetGroupRowValue(rowHandle, phieuCol);
                                    phieu = gv2?.ToString();
                                }
                                catch
                                {
                                    phieu = gridView2.GetGroupRowDisplayText(rowHandle);
                                }
                            }
                            else
                            {
                                phieu = gridView2.GetGroupRowDisplayText(rowHandle);
                            }
                        }
                    }
                    else
                    {
                        phieu = gridView2.GetFocusedRowCellValue("Phieu")?.ToString();
                    }

                    string url1 = $"{URL}NhaCC/GetPhieuBG?action=GetPhieuExcel&para={_mancc}&para1={phieu}&para2=&para3=&para4=&para5=";
                    string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json1);

                    ExportExcelLSX(TemplateFileName, ExportFileName, dt);

                    //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Lỗi: " + ex.Message);
                }
                finally
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }

        public void ExportExcelLSX(string TemplateFileName, string ExportFileName, DataTable dt)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
            using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
            {
                string templateFilePath = TemplateFileName;

                string resultFilePath = ExportFileName;
                FileInfo templateFile = new FileInfo(templateFilePath);
                ExcelPackage templatePackage = new ExcelPackage(templateFile);
                ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                ExcelRange range = worksheet.Cells;
                worksheet.Cells.Style.Font.Name = "Times New Roman";

                DateTime? ngay = null;

                if (dt.Rows[0]["ThoiDiem"] != DBNull.Value)
                {
                    ngay = Convert.ToDateTime(dt.Rows[0]["ThoiDiem"]);
                }

                worksheet.Cells["B3"].Value = tenncc;
                worksheet.Cells["B4"].Value = dt.Rows[0]["DonGiaMM"].ToString();
                worksheet.Cells["B5"].Value = dt.Rows[0]["UngTruoc"].ToString();
                worksheet.Cells["B6"].Value = ngay?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cells["B7"].Value = dt.Rows[0]["PhuongThucTT"].ToString();
                int startRow = 11;
                int row = startRow;

                foreach (DataRow dr in dt.Rows)
                {
                    string mavt = dr["ChiTiet"]?.ToString();
                    string mamau = dr["MauVT"]?.ToString();
                    string makho = dr["KhoVai"]?.ToString();
                    string donvi = dr["TenDVVT"]?.ToString();

                    worksheet.Cells[row, 1].Value = mavt;
                    worksheet.Cells[row, 2].Value = mamau;
                    worksheet.Cells[row, 3].Value = makho;
                    worksheet.Cells[row, 4].Value = donvi;
                    worksheet.Cells[row, 5].Value = dr["SoLuong"]?.ToString();
                    row++;
                }

                int lastDataRow = row - 1;

                var dataRange = worksheet.Cells[startRow, 1, lastDataRow, 5];
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.AutoFitColumns();


                FileInfo resultFile = new FileInfo(resultFilePath);
                templatePackage.SaveAs(resultFile);
            }

        }

        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            string mancc = editor.EditValue?.ToString();
            _mancc = editor.EditValue?.ToString();
            tenncc = editor.Text?.ToString();
            LoadData(mancc.ToString());
            LoadLichSu(mancc.ToString());
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPhieuBaoGia frm = new frmPhieuBaoGia();
            frm.ShowDialog();
        }
    }

}