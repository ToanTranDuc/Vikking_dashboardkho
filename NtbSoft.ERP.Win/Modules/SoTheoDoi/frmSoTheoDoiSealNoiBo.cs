using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SoTheoDoi;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Properties;
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

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmSoTheoDoiSealNoiBo : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _tuNgay = string.Empty, _denNgay = string.Empty, _poid = string.Empty,_dvsx = string.Empty;

        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        List<SoTheoDoiSealNoiBoEntity> lstSealNoiBo;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true; bool isCheckFirst = false;
        public frmSoTheoDoiSealNoiBo()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstSealNoiBo = new List<SoTheoDoiSealNoiBoEntity>();
        }
        protected override void OnLoad(EventArgs e)
        {
            TuNgay.EditValue = DateTime.Now;
            DenNgay.EditValue = DateTime.Now;
            
            loadTenKho();
            CreateDefault();
            _dvsx = "All";
            LoadDS();
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gridView1.OptionsBehavior.Editable = false;
                    break;
                case ResourceURL.EventStatus.Edit:
                    gridView1.OptionsBehavior.Editable = true;
                    break;
                case ResourceURL.EventStatus.Add:
                    gridView1.OptionsBehavior.Editable = true;
                    break;
            }
        }
        private void CreateDefault()
        {
            searchLookUpEdit_PO.Properties.ValueMember = "MaSeal";
            searchLookUpEdit_PO.Properties.DisplayMember = "Seal";
            searchLookUpEdit_PO.Properties.NullText = "[Chọn seal]";

            searchLookUpEdit1.Properties.ValueMember = "MaDVSX";
            searchLookUpEdit1.Properties.DisplayMember = "TenDVSX";
            searchLookUpEdit1.Properties.NullText = "[Chọn Kho]";
        }
        private void LoadDS()
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            string url = $"{URL}BienBanLuuSeal/Get?Action=GetSealNB&para1={Convert.ToDateTime(_tuNgay).ToString("yyyy-MM-dd")}" +
                         $"&para2={Convert.ToDateTime(_denNgay).ToString("yyyy-MM-dd")}&para3={_poid}&para4={_dvsx}&para5={"a"}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            if (tbl == null || tbl.Rows.Count == 0)
                gridControl1.DataSource = null;
            else
                gridControl1.DataSource = tbl;
            loadSLSeal();

        }
        private void loadSLSeal()
        {
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetSLMaSeal&Para1={_dvsx}&Para2=A&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            int sealSuDung = tbl.AsEnumerable().Where(x => x["Status"].ToString() != "0").Count();
            int sealConlai = tbl.AsEnumerable().Where(x => x["Status"].ToString() == "0").Count();
            textSuDung.EditValue = sealSuDung;
            textConLai.EditValue = sealConlai;
            //loadSLSealDaSuDung();
        }
        private void loadSLSealDaSuDung()
        {
            string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GetSealSuDung&Para1=A&Para2=A&Para3=A");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = tbl;
        }
        public void loadTenKho()
        {
            string url = $"{URL}BienBanLuuSeal/Get?Action=GetTenKho&para1=a&para2=a&para3=a&para4=a&para5=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = tbl;
        }
        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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


        private void btThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
        }
        private void ThemDong()
        {
            SoTheoDoiSealNoiBoEntity obj = new SoTheoDoiSealNoiBoEntity(DateTime.Now.Date);
            lstSealNoiBo.Add(obj);

            _rowAdd = gridView1.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            repositoryItemDateEditNgay.MinValue = DateTime.Now;
            repositoryItemDateEditNgay.MaxValue = DateTime.Now;
            GridViewUpdateStatus(_status);
            gridView1.FocusedRowHandle = _rowAdd;


        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            focused(sender);
        }
        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                if (_status == ResourceURL.EventStatus.Edit)
                {
                    if (view.FocusedColumn == colID)
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;

                }
            }
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }
        private async void XoaDong()
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                SoTheoDoiSealNoiBoEntity row = gridView1.GetRow(gridView1.FocusedRowHandle) as SoTheoDoiSealNoiBoEntity;
                string url = string.Format("{0}?Parameter={1}", URL + "SoTheoDoiSeal/Delete", row.ID);
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                    LoadDS();
                else XtraMessageBox.Show(result);
            }
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SuaDong();
        }
        private void SuaDong()
        {
            _status = ResourceURL.EventStatus.Edit;
            GridViewUpdateStatus(_status);
            focused(this.gridView1);
        }

        private void Xuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataTable dtTable = gridControl1.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("SoTheoDoiSealNoiBo{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "SoTheoDoiSealNoiBo.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                string json = JsonConvert.SerializeObject(lstSealNoiBo);
                DataTable tbSoTheoDoiSeal = JsonConvert.DeserializeObject<DataTable>(json);
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

        public void Export(string TemplateFileName, string ExportFileName, DataTable lstSoTheoDoiNoiBo)
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
                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["Sheet1"];
                        ws.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells.Style.Font.Name = "Times New Roman";

                        // countRow là hàng bắt đầu thêm dữ liệu của gridview
                        int countRow = 7;
                        for (int i = 0; i < lstSoTheoDoiNoiBo.Rows.Count; i++)
                        {
                            ws.Row(countRow).Height = 14;
                            ws.Cells[countRow, 1].Value = string.Format("Ngày: {0} - SL : {1}", lstSoTheoDoiNoiBo.Rows[i]["NgayLap"].ToString(), Convert.ToInt32(lstSoTheoDoiNoiBo.Rows[i]["SLSeal"]));
                            ws.Cells[countRow, 2].Value = lstSoTheoDoiNoiBo.Rows[i]["TenKH"].ToString();
                            ws.Cells[countRow, 3].Value = lstSoTheoDoiNoiBo.Rows[i]["SLThung"];
                            ws.Cells[countRow, 4].Value = lstSoTheoDoiNoiBo.Rows[i]["Status"].ToString();
                            ws.Cells[countRow, 5].Value = lstSoTheoDoiNoiBo.Rows[i]["SoXe"].ToString();
                            ws.Cells[countRow, 6].Value = lstSoTheoDoiNoiBo.Rows[i]["Seal"].ToString();
                            ws.Cells[countRow, 7].Value = lstSoTheoDoiNoiBo.Rows[i]["TenTaiXe"].ToString();
                            ws.Cells[countRow, 8].Value = lstSoTheoDoiNoiBo.Rows[i]["NVienXuat"].ToString();
                            ws.Cells[countRow, 9].Value = lstSoTheoDoiNoiBo.Rows[i]["TuKho"].ToString();
                            ws.Cells[countRow, 10].Value = lstSoTheoDoiNoiBo.Rows[i]["DenKho"].ToString();
                            ws.Cells[countRow, 11].Value = lstSoTheoDoiNoiBo.Rows[i]["CangDen"].ToString();
                            countRow++;
                        }
                        var borderData = ws.Cells[7, 1, countRow - 1, 9].Style.Border;
                        borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;
                        //// 9: Là số cột
                        //// 7: Là hàng bắt đầu thêm dữ liệu



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

        private void DrawStyleCellExcel(ExcelWorksheet ws, List<SoTheoDoiSealNoiBoEntity> lstSoTheoDoi, int startRow, int countColumn)
        {
            //ws.Cells[countRow, countColumn].Value = lstSoTheoDoi[i].NgayThangNam.ToString("dd/MM/yyyy");
            for (int row = startRow; row < lstSoTheoDoi.Count + startRow; row++)
            {
                for (int column = 1; column <= countColumn; column++)
                {
                    ws.Cells[row, column].Style.Font.Size = 12;
                    //ws.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells[row, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    if (column == countColumn)
                    {
                        ws.Cells[row, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.IsGetData && e.Column.FieldName == "Ngay")
            {
                e.Value = DateTime.Today;
            }
        }

        private void NapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadSLSeal();
            _poid = "";
            searchLookUpEdit1.EditValue = null;

        }


        private void dateEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            _tuNgay = TuNgay.EditValue.ToString();
            if (isCheckFirst)
                LoadDS();
        }
        private void grvPO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValuesPO = string.Join(";", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.ValueMember)));
            searchLookUpEdit_PO.EditValue = selectedValuesPO;
            if (searchLookUpEdit_PO.EditValue is null) return;
            _poid = searchLookUpEdit_PO.EditValue.ToString();
            LoadDS();

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            _dvsx = searchLookUpEdit1.EditValue == null ? "All" : searchLookUpEdit1.EditValue.ToString();
            LoadDS();
        }

        string selectedValuessPO = "";
        private void searchLookUpEdit_PO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessPO = string.Join("; ", grvPO.GetSelectedRows().Select(rowHandle => grvPO.GetRowCellValue(rowHandle, searchLookUpEdit_PO.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessPO))
            {
                e.DisplayText = "[Chọn Seal]";
            }
            else
            {
                e.DisplayText = selectedValuessPO;
            }
        }
        private void DenNgay_Properties_EditValueChanged(object sender, EventArgs e)
        {
            _denNgay = DenNgay.EditValue.ToString();
            if (isCheckFirst)
                LoadDS();
            isCheckFirst = true;
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }
        private void LuuDong()
        {
            List<SoTheoDoiSealNoiBoEntity> _lstUpdate = new List<SoTheoDoiSealNoiBoEntity>();
            SoTheoDoiSealNoiBoEntity row = gridView1.GetRow(gridView1.FocusedRowHandle) as SoTheoDoiSealNoiBoEntity;
            if (row == null)
            {
                XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            lstRowUpdate = lstRowUpdate.Distinct().OrderBy(item => item).ToList();


            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                SoTheoDoiSealNoiBoEntity item = gridView1.GetRow(lstRowUpdate[i]) as SoTheoDoiSealNoiBoEntity;
                if (item != null && !string.IsNullOrEmpty(item.NoiDung))
                {
                    _lstUpdate.Add(item);
                }
            }
            string msResult = "";
            if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            {
                return;
            }
            string url = string.Format("{0}?", URL + "SoTheoDoiSeal/Post");
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
            if (msResult.ToLower() == "true")
                LoadDS();
            else XtraMessageBox.Show(msResult);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
        }
    }
}
