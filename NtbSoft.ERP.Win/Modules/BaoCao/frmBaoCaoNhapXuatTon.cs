using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Libs;
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

namespace NtbSoft.ERP.Win.Modules.BaoCao
{
    public partial class frmBaoCaoNhapXuatTon : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        bool indicatorIcon = true;
        //string maDVSXFilter = string.Empty;
        //string maHHFilter = string.Empty;
        //string maDHFilter = string.Empty;
        List<DongThungBaoCaoEntity> lstDsXuatNhapTonEntity;
        string json = string.Empty;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool isCheckChanged = false;
        string layout;

        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlRefresh;
        ActionControl actionControlPrintExcel;
        List<ActionControl> lstActionControls;

        DataTable tblDVSX;
        DataTable tblhangHoa;
        DataTable _tblhangHoa;
        public frmBaoCaoNhapXuatTon()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDsXuatNhapTonEntity = new List<DongThungBaoCaoEntity>();
        }
        protected override void OnLoad(EventArgs e)
        {
            //LoadDataSource();
            CreateSearchLookup();
            CheckPerminsion();
            Console.WriteLine("OnLoad");
            isCheckChanged = barCheckItem1.Checked;
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            SetIndexVisibleGridColumn();
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlRefresh = new ActionControl(NapLai, true, ActionType.Refresh, true);
            actionControlPrintExcel = new ActionControl(XuatExcelSoLuongXuatNhapTon, true, ActionType.ExCel, true);

            lstActionControls = new List<ActionControl> {
                actionControlRefresh, actionControlPrintExcel};
            return lstActionControls;
        }

        private void CreateSearchLookup()
        {
            //
            string urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoaWithDVSX");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            tblhangHoa = JsonConvert.DeserializeObject<DataTable>(jsonHH);

            //
            string _urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string _jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(_urlHH); }).Result;
            _tblhangHoa = JsonConvert.DeserializeObject<DataTable>(_jsonHH);
            repositoryItemHHFilter.DataSource = _tblhangHoa;
            //repositoryItemHHFilter.DataSource = tblhangHoa.AsEnumerable().;
            //string urlDH = string.Format("{0}", URL + "DonHangTonKho/GetDonHangTonKho");
            //string jsonDH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDH); }).Result;
            //DataTable tblDH = JsonConvert.DeserializeObject<DataTable>(jsonDH);
            //repositoryItemDHFilter.DataSource = tblDH;

            // DonViSanXuat
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            tblDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            tblDVSX = tblDVSX.AsEnumerable().Where(x => (bool)x["GiaCong"].Equals(false)).CopyToDataTable();
            repositoryItemDVSXFilter.DataSource = tblDVSX;
            this.repositoryItemDVSXFilter.DisplayMember = "TenDVSX";
            this.repositoryItemDVSXFilter.ValueMember = "MaDVSX";
            //barCheckItem2.Checked = false;
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

        }

        private void repositoryItemSearchDVSX_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;
            string maDVSX = string.Empty;
            if (changing.NewValue != null)
            {
                maDVSX = changing.NewValue.ToString();
            }
            else
            {
                maDVSX = string.Empty;
            }
            // Filter danh sách hàng hóa của từng DVSX
            if (maDVSX != null && !string.IsNullOrEmpty(maDVSX))
            {
                repositoryItemHHFilter.DataSource = tblhangHoa.AsEnumerable().Where(x => x["MaDVSX"].Equals(maDVSX)).CopyToDataTable();
            }
            else
            {
                repositoryItemHHFilter.DataSource = _tblhangHoa;
            }

            barEditItem1.EditValue = null;


            //FilterData(maDVSXFilter, maHHFilter);

        }

        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void repositoryItemSearchHH_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void repositoryItemSearchDH_EditValueChanged(object sender, EventArgs e)
        {
            //ChangingEventArgs changing = e as ChangingEventArgs;

            //if (changing.NewValue != null)
            //{
            //    maDHFilter = changing.NewValue.ToString();
            //}
            //else
            //{
            //    maDHFilter = string.Empty;
            //}

            //FilterData(maDVSXFilter, maHHFilter, maDHFilter);
        }

        private void LoadDataSource()
        {
            // check DVSX và Hàng hóa khác null mới gọi
            string maDVS = barEditItem2.EditValue != null ? barEditItem2.EditValue.ToString() : string.Empty;
            string maHH = barEditItem1.EditValue != null ? barEditItem1.EditValue.ToString() : string.Empty;
            bool isChecked = this.barCheckItem2.Checked;
            bool isCheckLuanChuyen = this.barCheckItem1.Checked;
            if ((maHH != null && !string.IsNullOrEmpty(maHH)) ||
                (maDVS != null && !string.IsNullOrEmpty(maDVS)))
            {
                string url = string.Empty;
                if (isCheckLuanChuyen)
                {
                    //this.gridColumn6.Visible = true;
                    //this.gridColumn19.Visible = true;
                    //this.gridColumn16.Visible = false;
                    url = string.Format("{0}?maHang={1}&&maDVSX={2}&&isChecked={3}", URL + "DongThung/GetDSXuatNhapTonLuanChuyen", maHH, maDVS, isChecked ? 1 : 0);
                }
                else
                {
                    //this.gridColumn6.Visible = false;
                    //this.gridColumn19.Visible = false;
                    //this.gridColumn16.Visible = true;
                    url = string.Format("{0}?maHang={1}&&maDVSX={2}&&isChecked={3}", URL + "DongThung/GetDSXuatNhapTon", maHH, maDVS, isChecked ? 1 : 0);
                }




                string _json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                json = _json;
                lstDsXuatNhapTonEntity = JsonConvert.DeserializeObject<List<DongThungBaoCaoEntity>>(_json);
                //DataTable tblXuatNhapTon = JsonConvert.DeserializeObject<DataTable>(_json);
                SetIndexVisibleGridColumn();
                gridControl1.DataSource = lstDsXuatNhapTonEntity;
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn Đơn vị sản xuất hoặc Mã hàng để lọc dữ liệu.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
        }
        // 
        private void SetIndexVisibleGridColumn()
        {
            gridColumn4.VisibleIndex = 0;
            gridColumn20.VisibleIndex = 5;
            gridColumn9.VisibleIndex = 6;
            gridColumn13.VisibleIndex = 7;
            gridColumn14.VisibleIndex = 8;
            gridColumn15.VisibleIndex = 9;
            //
            if (isCheckChanged)
            {
                gridColumn2.VisibleIndex = 1;
                gridColumn23.VisibleIndex = -1;
                gridColumn6.VisibleIndex = 3;
                gridColumn22.VisibleIndex = 4;
                gridColumn19.VisibleIndex = 5;
                gridColumn16.VisibleIndex = -1;
            }
            else
            {
                gridColumn2.VisibleIndex = -1;
                gridColumn23.VisibleIndex = 1;
                gridColumn6.VisibleIndex = -1;
                gridColumn19.VisibleIndex = -1;
                gridColumn22.VisibleIndex = -1;
                gridColumn16.VisibleIndex = 2;
            }



        }
        private void gridview1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);

            // Chỉnh font chữ cho header
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Chỉnh alignment cho header
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Info.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
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

        private void NapLai()
        {
            LoadDataSource();
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
        }

        private void XuatExcelSoLuongXuatNhapTon()
        {
            List<DongThungBaoCaoEntity> lstExport = gridView1.DataSource as List<DongThungBaoCaoEntity>;

            //bool isExport = lstExport.All(x => (x.MaKho.Equals(lstExport[0].MaKho)) && (x.MaHang.Equals(lstExport[0].MaHang)));
            //bool isExport = lstExport.All(x => (x.MaKho.Equals(lstExport[0].MaKho)) && (x.MaHang.Equals(lstExport[0].MaHang)) && (x.NgayLapThe.Date.Equals(lstExport[0].NgayLapThe.Date)));

            //Console.WriteLine("isExport: " + isExport);

            //if (isExport)
            //{
            if (lstExport != null && lstExport.Count > 0)
            {
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

                Sfd.FileName = string.Format("baocaosoluongxuatnhapton{0}", DateTime.Now.Millisecond.ToString());
                if (isCheckChanged)
                {
                    Sfd.FileName = string.Format("baocaosoluongxuatnhaptonluanchuyen{0}", DateTime.Now.Millisecond.ToString());
                }
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    string fileName = "baocaonhapxuatton.xlsx";
                    if (isCheckChanged)
                    {
                        fileName = "baocaonhapxuattonluanchuyen.xlsx";
                    }
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    //gridView1.ExportToXlsx(Sfd.FileName);
                    Export(TemplateFileName, ExportFileName, lstExport, isCheckChanged);

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
            else
            {
                //DevExpress.XtraEditors.XtraMessageBox.Show("Không có dữ liệu xuất excel, vui lòng kiểm tra lại.!", "Thông báo");

                clsCommonBS.ConfirmError("Không có dữ liệu xuất excel, Vui lòng kiểm tra lại.!");
            }
        }

        private void XuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XuatExcelSoLuongXuatNhapTon();
        }

        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Console.WriteLine("check_changed");
            if (e.Item is BarCheckItem)
            {
                isCheckChanged = (e.Item as BarCheckItem).Checked;
            }
            LoadDataSource();
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadDataSource();
        }

        private void barCheckItem2_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            //if (e.Item is BarCheckItem)
            //{
            //    bool isCheckGiaCong = (e.Item as BarCheckItem).Checked;
            //    if (isCheckGiaCong)
            //    {
            //        repositoryItemDVSXFilter.DataSource = tblDVSX.AsEnumerable().Where(x => (bool)x["GiaCong"]).CopyToDataTable();
            //    }
            //    else
            //    {
            //        repositoryItemDVSXFilter.DataSource = tblDVSX.AsEnumerable().Where(x => (bool)x["GiaCong"].Equals(false)).CopyToDataTable();
            //    }
            //}
            LoadDataSource();
        }

        private void barCheckItem5_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            // Đổi số lượng giữa số SP và số thùng
            if (e.Item is BarCheckItem)
            {
                bool isCheckSoThung = (e.Item as BarCheckItem).Checked;
                if (isCheckSoThung)
                {
                    //  số lượng nhập
                    this.gridColumn13.FieldName = "SoThungNhap";
                    this.gridColumn13.Summary.Clear();
                    this.gridColumn13.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoThungNhap", "{0:n0}")});
                    this.gridColumn14.Summary.Clear();
                    this.gridColumn14.FieldName = "SoThungXuat";
                    this.gridColumn14.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoThungXuat", "{0:n0}")});
                    this.gridColumn15.Summary.Clear();
                    this.gridColumn15.FieldName = "SoThungTon";
                    this.gridColumn15.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoThungTon", "{0:n0}")});
                }
                else
                {
                    //  số lượng nhập
                    this.gridColumn13.Summary.Clear();
                    this.gridColumn13.FieldName = "SoSPNhap";
                    this.gridColumn13.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoSPNhap", "{0:n0}")});
                    this.gridColumn14.Summary.Clear();
                    this.gridColumn14.FieldName = "SoSPXuat";
                    this.gridColumn14.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoSPXuat", "{0:n0}")});
                    this.gridColumn15.Summary.Clear();
                    this.gridColumn15.FieldName = "SoSPTon";
                    this.gridColumn15.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoSPTon", "{0:n0}")});
                }
            }

        }

        public void Export(string TemplateFileName, string ExportFileName, List<DongThungBaoCaoEntity> _lstTongQuanDongThung, bool _isCheckChanged)
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
                        excelPackage.Workbook.Properties.Title = "Bao-cao-dong-thung";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["Sheet1"];


                        // countRow là hàng bắt đầu thêm dữ liệu của gridview
                        int countRow = 3;

                        int tongSoLuongNhap = 0;
                        int tongSoLuongXuat = 0;
                        int tongSoLuongTon = 0;
                        for (int i = 0; i < _lstTongQuanDongThung.Count; i++)
                        {
                            //DataRow dataRowDV = tblDonVi.AsEnumerable().Where(x => x["MaDV"].ToString().Equals(lstTheKho[i].MaDV)).FirstOrDefault();
                            //string tenDV = string.Empty;
                            //if (dataRowDV != null && dataRowDV["TenDV"] != null)
                            //{
                            //    tenDV = dataRowDV["TenDV"].ToString();
                            //}

                            ws.Row(countRow).Height = 14;
                            //ws.Cells[countRow, 3].Value = _lstTongQuanDongThung[i].MaDH;
                            //ws.Cells[countRow, 4].Value = _lstTongQuanDongThung[i].MaPKL;
                            if (!_isCheckChanged)
                            {
                                // XuatBaoCaoNhapXuatTon
                                ws.Cells[countRow, 1].Value = _lstTongQuanDongThung[i].TenHang;
                                ws.Cells[countRow, 2].Value = _lstTongQuanDongThung[i].TenDVSX;
                                ws.Cells[countRow, 3].Value = _lstTongQuanDongThung[i].TenKho;
                                ws.Cells[countRow, 4].Value = _lstTongQuanDongThung[i].DotSX;
                                ws.Cells[countRow, 5].Value = _lstTongQuanDongThung[i].PO;
                                if (barCheckItem5.Checked)
                                {
                                    ws.Cells[countRow, 6].Value = _lstTongQuanDongThung[i].SoThungNhap;
                                    ws.Cells[countRow, 7].Value = _lstTongQuanDongThung[i].SoThungXuat;
                                    ws.Cells[countRow, 8].Value = _lstTongQuanDongThung[i].SoThungTon;
                                }
                                else
                                {
                                    ws.Cells[countRow, 6].Value = _lstTongQuanDongThung[i].SoSPNhap;
                                    ws.Cells[countRow, 7].Value = _lstTongQuanDongThung[i].SoSPXuat;
                                    ws.Cells[countRow, 8].Value = _lstTongQuanDongThung[i].SoSPTon;
                                }
                            }
                            else
                            {
                                // XuatBaoCaoNhapXuatTonLuanChuyen
                                ws.Cells[countRow, 1].Value = _lstTongQuanDongThung[i].TenHang;
                                ws.Cells[countRow, 2].Value = _lstTongQuanDongThung[i].TenDVSX;
                                ws.Cells[countRow, 3].Value = _lstTongQuanDongThung[i].TenKhoTu;
                                ws.Cells[countRow, 4].Value = _lstTongQuanDongThung[i].TenDVSXDen;
                                ws.Cells[countRow, 5].Value = _lstTongQuanDongThung[i].TenKhoDen;
                                ws.Cells[countRow, 6].Value = _lstTongQuanDongThung[i].DotSX;
                                ws.Cells[countRow, 7].Value = _lstTongQuanDongThung[i].PO;
                                if (barCheckItem5.Checked)
                                {
                                    ws.Cells[countRow, 8].Value = _lstTongQuanDongThung[i].SoThungNhap;
                                    ws.Cells[countRow, 9].Value = _lstTongQuanDongThung[i].SoThungXuat;
                                    ws.Cells[countRow, 10].Value = _lstTongQuanDongThung[i].SoThungTon;
                                }
                                else
                                {
                                    ws.Cells[countRow, 8].Value = _lstTongQuanDongThung[i].SoSPNhap;
                                    ws.Cells[countRow, 9].Value = _lstTongQuanDongThung[i].SoSPXuat;
                                    ws.Cells[countRow, 10].Value = _lstTongQuanDongThung[i].SoSPTon;
                                }
                            }

                            if (barCheckItem5.Checked)
                            {
                                tongSoLuongNhap += _lstTongQuanDongThung[i].SoThungNhap;
                                tongSoLuongXuat += _lstTongQuanDongThung[i].SoThungXuat;
                                tongSoLuongTon += _lstTongQuanDongThung[i].SoThungTon;
                            }
                            else
                            {
                                tongSoLuongNhap += _lstTongQuanDongThung[i].SoSPNhap;
                                tongSoLuongXuat += _lstTongQuanDongThung[i].SoSPXuat;
                                tongSoLuongTon += _lstTongQuanDongThung[i].SoSPTon;
                            }

                            //ws.Cells[countRow, 8].Value = lstSoTheoDoi[i].NguoiBamSeal;

                            countRow++;
                        }

                        if (!_isCheckChanged)
                        {
                            ws.Cells[countRow, 6].Value = tongSoLuongNhap;
                            ws.Cells[countRow, 6].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[countRow, 7].Value = tongSoLuongXuat;
                            ws.Cells[countRow, 7].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[countRow, 8].Value = tongSoLuongTon;
                            ws.Cells[countRow, 8].Style.Font.Color.SetColor(Color.Red);

                            if (barCheckItem5.Checked)
                            {
                                ws.Cells[1, 6].Value = "Đơn vị: Số thùng";
                            }
                            else
                            {
                                ws.Cells[1, 6].Value = "Đơn vị: Số sản phẩm";
                            }
                            ws.Cells[1, 6].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[1, 6].Style.Font.Size = 14;



                            // Tổng số lượng
                            ws.Cells[countRow, 1, countRow, 5].Merge = true;
                            ws.Cells[countRow, 1].Value = "Tổng:";
                            ws.Cells[countRow, 1].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[countRow, 1].Style.Font.Size = 14;
                            ws.Cells[countRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            ws.Cells[countRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }
                        else
                        {
                            ws.Cells[countRow, 8].Value = tongSoLuongNhap;
                            ws.Cells[countRow, 8].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[countRow, 9].Value = tongSoLuongXuat;
                            ws.Cells[countRow, 9].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[countRow, 10].Value = tongSoLuongTon;
                            ws.Cells[countRow, 10].Style.Font.Color.SetColor(Color.Red);

                            if (barCheckItem5.Checked)
                            {
                                ws.Cells[1, 8].Value = "Đơn vị: Số thùng";
                            }
                            else
                            {
                                ws.Cells[1, 8].Value = "Đơn vị: Số sản phẩm";
                            }
                            ws.Cells[1, 8].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[1, 8].Style.Font.Size = 14;

                            // Tổng số lượng
                            ws.Cells[countRow, 1, countRow, 7].Merge = true;
                            ws.Cells[countRow, 1].Value = "Tổng:";
                            ws.Cells[countRow, 1].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells[countRow, 1].Style.Font.Size = 14;
                            ws.Cells[countRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            ws.Cells[countRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }


                        // 10||11: Là số cột
                        // 3: Là Row bắt đầu
                        // _lstTongQuanDongThung.Count + 1: +1 là thêm 1 dòng cho hàng Tổng
                        DrawStyleCellExcel(ws, !_isCheckChanged ? 8 : 10, 3, _lstTongQuanDongThung.Count + 1);

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

        private void DrawStyleCellExcel(ExcelWorksheet ws, int countColumn, int startRow, int endRow)
        {
            //ws.Cells[countRow, countColumn].Value = lstSoTheoDoi[i].NgayThangNam.ToString("dd/MM/yyyy");
            for (int row = startRow; row < endRow + startRow; row++)
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
    }
}
