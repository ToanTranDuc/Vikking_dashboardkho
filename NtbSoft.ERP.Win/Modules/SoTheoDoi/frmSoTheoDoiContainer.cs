using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SoTheoDoi;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
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

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmSoTheoDoiContainer : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        //List<SoTheoDoiContainerEntity> lstSoTheoDoiCont;
        //List<SoTheoDoiContainerEntity> lstSoTheoDoiContFilter;
        List<int> lstRowUpdate = new List<int>();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        string repositoryDateEditFrom;
        string repositoryDateEditTo;
        string json = string.Empty;
        DateTime minTime = new DateTime(1900, 1, 1);
        string allWareHouse = "Tất cả Kho";
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlRefresh;
        ActionControl actionControlExportExcel;
        List<ActionControl> lstActionControls;
        public frmSoTheoDoiContainer()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            barEditItem2.EditValue = allWareHouse;
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            Init();
        }

        private void NapLai()
        {
            barEditItem1.EditValue = null;
            barEditItem3.EditValue = null;
            repositoryItemSearchLookUpEdit1.DataSource = null;
            Init();
            LoadDSSealCont();
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlRefresh = new ActionControl(NapLai, true, ActionType.Refresh, true);
            actionControlExportExcel = new ActionControl(XuatExcelTheoDoiCont, true, ActionType.ExCel, true);

            lstActionControls = new List<ActionControl> { actionControlRefresh, actionControlExportExcel };
            return lstActionControls;
        }
        private void Init()
        {

            repositoryItemSearchLookUpEdit1.ValueMember = "MaKho";
            repositoryItemSearchLookUpEdit1.DisplayMember = "TenDVSX";
            GridView dvView = repositoryItemSearchLookUpEdit1.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaDVSX", Caption = "Mã DVSX", Name = "colMaDV", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVSX", Caption = "Tên DVSX", Name = "colTenDVSX", Visible = true });

            }
            LoadMaKho();
        }
        private void LoadMaKho()
        {
            string url = $"{URL}SoTheoDoiContainer/GetDSSoTheoDoiContainer?action=GetMaKho&tungay=a&denngay=a&madvsx=a";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit1.DataSource = tbl;
        }
        private void LoadDSSealCont()
        {
            try
            {
                
                if (barEditItem1.EditValue == null || barEditItem1.EditValue == ""
               || barEditItem3.EditValue == null || barEditItem3.EditValue == "")
                {
                    gridSealCont.DataSource = null;
                    return;
                };
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                string tungay = Convert.ToDateTime(barEditItem1.EditValue).ToString("yyyy-MM-dd");
                string denngay = Convert.ToDateTime(barEditItem3.EditValue).ToString("yyyy-MM-dd");
                string madvsx = barEditItem4.EditValue == null ? "" : barEditItem4.EditValue.ToString();
                string url = $"{URL}SoTheoDoiContainer/GetDSSoTheoDoiContainer?action=Get&tungay={tungay}&denngay={denngay}&madvsx={madvsx}";
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    gridSealCont.DataSource = null;
                    return;
                }

                gridSealCont.DataSource = tbl;

                //_status = ResourceURL.EventStatus.View;
                //GridViewUpdateStatus(_status);

                //MapDataCBXKho(gridViewSealCont);
                //if (barEditItem2.EditValue != null
                //    && !string.IsNullOrEmpty(barEditItem2.EditValue.ToString())
                //    && !barEditItem2.EditValue.ToString().Equals(allWareHouse))
                //{
                //    Filter();
                //}
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện.\nVui lòng kiểm tra và thực hiện lại", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
        }

        //private void MapDataCBXKho(GridView view)
        //{
        //    Console.WriteLine("Test_MapDataCBXDH");
        //    repositoryItemComboBox1.Items.Clear();
        //    repositoryItemComboBox1.Items.Add(allWareHouse);
        //    DataTable _tblPGN = (view.DataSource as DataView).Table;
        //    foreach (DataRow row in _tblPGN.Rows)
        //    {
        //        string _tenDVSX = string.Empty;
        //        foreach (DataColumn column in _tblPGN.Columns)
        //        {
        //            if (row[column] != null &&
        //                !string.IsNullOrEmpty(row[column].ToString()))
        //            {
        //                if (column.ColumnName.Equals(this.colTenDVSX.FieldName))
        //                {

        //                    _tenDVSX = row[column].ToString();
        //                }
        //            }
        //        }
        //        Console.WriteLine("Test_row.count: " + _tblPGN.Rows.Count);

        //        if (_tenDVSX != null && !string.IsNullOrEmpty(_tenDVSX) &&
        //            !repositoryItemComboBox1.Items.Contains(_tenDVSX))
        //        {
        //            repositoryItemComboBox1.Items.Add(_tenDVSX);
        //        }
        //    }
        //}

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            //switch (status)
            //{
            //    case ResourceURL.EventStatus.View:
            //        gridViewSealCont.OptionsBehavior.Editable = false;

            //        if (_allowAdd)
            //            Them.Enabled = true;
            //        if (_allowEdit)
            //            Sua.Enabled = true;
            //        if (_allowAdd || _allowEdit)
            //            Luu.Enabled = false;
            //        break;
            //    case ResourceURL.EventStatus.Edit:
            //        gridViewSealCont.OptionsBehavior.Editable = true;
            //        if (_allowAdd)
            //            Them.Enabled = false;
            //        if (_allowEdit)
            //            Sua.Enabled = false;
            //        if (_allowAdd || _allowEdit)
            //            Luu.Enabled = true;

            //        break;
            //    case ResourceURL.EventStatus.Add:
            //        gridViewSealCont.OptionsBehavior.Editable = true;
            //        if (_allowAdd)
            //            Them.Enabled = false;
            //        if (_allowEdit)
            //            Sua.Enabled = false;
            //        if (_allowAdd || _allowEdit)
            //            Luu.Enabled = true;
            //        break;
            //}
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
                Them.Enabled = false;
            }
            //if (!_allowEdit)
            //{
            //    Sua.Enabled = false;
            //}
            //if (_allowAdd || _allowEdit)
            //{
            //    Luu.Enabled = true;
            //}
            //else
            //{
            //    Luu.Enabled = false;
            //}

            //if (!_allowDelete)
            //    Xoa.Enabled = false;

        }

        private void gridViewSoTheoDoiContainer_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        //private void ThemDong()
        //{
        //    SoTheoDoiContainerEntity obj = new SoTheoDoiContainerEntity(DateTime.Now.Date);
        //    //_bindingHangHoaEntity.Add(obj);
        //    lstSoTheoDoiCont.Add(obj);

        //    _rowAdd = gridViewSealCont.RowCount - 1;
        //    _status = ResourceURL.EventStatus.Add;
        //    GridViewUpdateStatus(_status);
        //    gridViewSealCont.FocusedRowHandle = _rowAdd;
        //}

        //private void SuaDong()
        //{
        //    _status = ResourceURL.EventStatus.Edit;
        //    GridViewUpdateStatus(_status);
        //    focused(this.gridViewSealCont);
        //}

        //private async void XoaDong()
        //{
        //    DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //    if (messResult == DialogResult.Yes)
        //    {
        //        SoTheoDoiContainerEntity row = gridViewSealCont.GetRow(gridViewSealCont.FocusedRowHandle) as SoTheoDoiContainerEntity;
        //        string url = string.Format("{0}?Parameter={1}", URL + "SoTheoDoiContainer/DeleteDSSoTheoDoiContainer", row.ID);
        //        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
        //        if (result.ToLower() == "true")
        //            LoadDSSealCont();
        //        else XtraMessageBox.Show(result);
        //    }
        //}

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //ThemDong();
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //XoaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
        }

        private void LuuDong()
        {
            this.ActiveControl = this.button1;
            List<SoTheoDoiContainerEntity> _lstUpdate = new List<SoTheoDoiContainerEntity>();

            SoTheoDoiContainerEntity focusedRow = gridViewSealCont.GetFocusedRow() as SoTheoDoiContainerEntity;

            if (focusedRow == null)
            {
                XtraMessageBox.Show(Resources.UpdateDataBefore, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();

            for (int i = 0; i < lstRowUpdate.Count; i++)
            {
                SoTheoDoiContainerEntity item = gridViewSealCont.GetRow(lstRowUpdate[i]) as SoTheoDoiContainerEntity;
                _lstUpdate.Add(item);
            }

            string msResult = "";
            if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            {
                return;
            }
            string url = string.Format("{0}", URL + "SoTheoDoiContainer/Post");
            msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                LoadDSSealCont();
            }
            else XtraMessageBox.Show(msResult);
            _status = ResourceURL.EventStatus.View;
            GridViewUpdateStatus(_status);
            lstRowUpdate.Clear();
        }

        private void gridViewSoTheoDoiContainer_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
            //focused(sender);
        }

        private void XuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XuatExcelTheoDoiCont();
        }

        private void XuatExcelTheoDoiCont()
        {
            DataTable tblExport = (gridViewSealCont.DataSource as DataView).Table;
            if (tblExport != null && tblExport.Rows.Count > 0)
            {
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
                Sfd.FileName = string.Format("SoTheoDoiSealContainer{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    string fileName = "sotheodoi_container.xlsx";
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    //string json = JsonConvert.SerializeObject(lstSoTheoDoiCont);
                    //DataTable tbSoTheoDoiSeal = JsonConvert.DeserializeObject<DataTable>(json);
                    Export(TemplateFileName, ExportFileName, tblExport);

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

        public void Export(string TemplateFileName, string ExportFileName, DataTable tblSoTheoDoi)
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
                        excelPackage.Workbook.Properties.Author = "Cty NTB";


                        excelPackage.Workbook.Properties.Title = "Bien-Ban-Seal-Container";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["seal-cont"];
                        ExcelRange range = ws.Cells;
                        if (!(tblSoTheoDoi != null) || (tblSoTheoDoi != null && tblSoTheoDoi.Rows.Count == 0)) return;


                        int countRow = 9;
                        int SumSLThung = tblSoTheoDoi.AsEnumerable().Sum(x => Convert.ToInt32(x["SoThung"]));

                        foreach (DataRow item in tblSoTheoDoi.Rows)
                        {
                            ws.Cells[countRow, 1].Value = countRow - 8;
                            ws.Cells[countRow, 2].Value = item["NgayThangNam"];
                            ws.Cells[countRow, 3].Value = item["SoSeal"];
                            ws.Cells[countRow, 4].Value = item["NgayThangNam"];
                            ws.Cells[countRow, 5].Value = item["TinhTrangSeal"];
                            ws.Cells[countRow, 6].Value = item["SoContBamSeal"];
                            ws.Cells[countRow, 7].Value = item["SoThung"];
                            ws.Cells[countRow, 8].Value = item["NguoiBamSeal"];
                            countRow++;
                        }
                        ws.Column(6).AutoFit();
                        range = ws.Cells[countRow, 1, countRow, 6]; range.Value = "Tổng"; range.Merge = true; range.Style.Font.Bold = true;
                        range = ws.Cells[countRow, 7]; range.Value = SumSLThung; range.Style.Font.Bold = true;
                        var borderData1 = ws.Cells[9, 1, countRow, 9].Style.Border;
                        borderData1.Bottom.Style =
                            borderData1.Top.Style =
                            borderData1.Left.Style =
                            borderData1.Right.Style = ExcelBorderStyle.Thin;
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }



        private void repositoryItemDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            //ChangingEventArgs changing = e as ChangingEventArgs;

            //DataTable _tblFilter = JsonConvert.DeserializeObject<DataTable>(json);

            //if (changing.NewValue != null)
            //{
            //    repositoryDateEditFrom = changing.NewValue.ToString();

            //    //_tbl = lstSoTheoDoiCont.Where(x => (x.NgayThangNam == DateTime.Parse(repositoryDateEdit))).ToList();
            //    if (repositoryDateEditFrom != null && repositoryDateEditFrom != ""
            //        && repositoryDateEditTo != null && repositoryDateEditTo != "")
            //    {
            //        Filter();
            //    }
            //}
            //else
            //{
            //    gridSealCont.DataSource = _tblFilter;
            //}

            //Console.WriteLine("EditValueChanged");
            LoadDSSealCont();
        }

        //private void gridViewSealCont_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        //{
        //    // Nếu cột NgayThangNam và NgayCap null, sẽ gán minTime cho nó(01-01-1900)( Xử lý từ SQL)
        //    // Xử lý View:
        //    // Nếu cột NgayThangNam || NgayCap mang giá trị minTime thì ẩn không hiển thị giá trị
        //    // -> Mục đích khi NgayThangNam trả về Null thì các dòng khác trong gridview không chỉnh displayformat được


        //    string daXuat = "Đã xuất";
        //    string xuatLoi = "Xuất lỗi";
        //    string chuaXuat = "Chưa xuất";
        //    int outTryParse = -1;
        //    // Console.WriteLine("gridViewSealCont_CustomColumnDisplayText");
        //    if (e.Column.Name.Equals(this.colTinhTrangSeal.Name)
        //        && e.Value != null && !string.IsNullOrEmpty(e.Value.ToString())
        //        && int.TryParse(e.Value.ToString(), out outTryParse))
        //    {
        //        switch (Convert.ToInt32(e.Value.ToString()))
        //        {
        //            case 0:
        //                e.DisplayText = chuaXuat;
        //                break;
        //            case 1:
        //                e.DisplayText = xuatLoi;
        //                break;
        //            case 2:
        //                e.DisplayText = daXuat;
        //                break;
        //        }
        //    }

        //    if ((e.Column.Name.Equals(this.colNgayThangNam.Name) || (e.Column.Name.Equals(this.colNgayCap.Name)))
        //        && e.Value != null && !string.IsNullOrEmpty(e.Value.ToString())
        //        && e.Value.Equals(minTime))
        //    {
        //        e.DisplayText = "";
        //    }
        //}

        private void gridViewSealCont_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            // Console.WriteLine("gridViewSealCont_RowCellStyle");
            int outTryParse = -1;
            // e.Appearance.ForeColor = Color.Red;
            if (e.Column.Name.Equals(this.colTinhTrangSeal.Name)
                && e.CellValue != null && !string.IsNullOrEmpty(e.CellValue.ToString())
                && int.TryParse(e.CellValue.ToString(), out outTryParse))
            {
                switch (Convert.ToInt32(e.CellValue.ToString()))
                {
                    //case 0:
                    //    e.DisplayText = chuaXuat;
                    //    break;
                    case 1:
                        e.Appearance.ForeColor = Color.Red;
                        break;
                    case 2:
                        e.Appearance.ForeColor = Color.Green;
                        break;
                }
            }
        }

        private void repositoryItemComboBox1_EditValueChanged(object sender, EventArgs e)
        {
            barEditItem2.EditValue = (e as ChangingEventArgs).NewValue;
            Filter();
        }

        private void Filter()
        {
            LoadDSSealCont();
            //try
            //{
            //    DataTable _tblFilter = JsonConvert.DeserializeObject<DataTable>(json);

            //    _tblFilter = filterDate(_tblFilter);

            //    // Filter data theo mã đơn hàng
            //    if (this.barEditItem2.EditValue != null && !string.IsNullOrEmpty(this.barEditItem2.EditValue.ToString())
            //        && !this.barEditItem2.EditValue.Equals(allWareHouse))
            //    {
            //        string _tenDVSX = this.barEditItem2.EditValue.ToString();
            //        DataRow row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["TenDVSX"].Equals(_tenDVSX));
            //        if (row != null)
            //        {
            //            _tblFilter = _tblFilter.AsEnumerable().Where(x => x["TenDVSX"].Equals(_tenDVSX)).CopyToDataTable();
            //        }
            //        else
            //        {
            //            _tblFilter = null;
            //        }
            //        Console.WriteLine("Test_FilterMaDH");
            //    }

            //    this.gridSealCont.DataSource = _tblFilter;
            //}
            //catch (Exception ex)
            //{
            //    XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private DataTable filterDate(DataTable _tblFilter)
        {
            if (repositoryDateEditFrom != null && repositoryDateEditFrom != ""
                && repositoryDateEditTo != null && repositoryDateEditTo != "")
            {
                DataRow _row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["NgayThangNam"] != null && !string.IsNullOrEmpty(x["NgayThangNam"].ToString())
                    && (DateTime.Parse(x["NgayThangNam"].ToString()).Date >= DateTime.Parse(repositoryDateEditFrom).Date)
                    && (DateTime.Parse(x["NgayThangNam"].ToString()).Date <= DateTime.Parse(repositoryDateEditTo).Date));
                if (_row != null)
                {
                    _tblFilter = _tblFilter.AsEnumerable().Where(x => (
                    DateTime.Parse(x["NgayThangNam"].ToString()).Date >= DateTime.Parse(repositoryDateEditFrom).Date
                     && (DateTime.Parse(x["NgayThangNam"].ToString()).Date <= DateTime.Parse(repositoryDateEditTo).Date))).CopyToDataTable();
                    return _tblFilter;
                }
                else
                {
                    return new DataTable();
                }
            }
            return _tblFilter;
        }

        private void repositoryItemDateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSSealCont();
            //ChangingEventArgs changing = e as ChangingEventArgs;

            //DataTable _tblFilter = JsonConvert.DeserializeObject<DataTable>(json);

            //if (changing.NewValue != null)
            //{
            //    repositoryDateEditTo = changing.NewValue.ToString();

            //    //_tbl = lstSoTheoDoiCont.Where(x => (x.NgayThangNam == DateTime.Parse(repositoryDateEdit))).ToList();
            //    if (repositoryDateEditFrom != null && repositoryDateEditFrom != ""
            //        && repositoryDateEditTo != null && repositoryDateEditTo != "")
            //    {
            //        Filter();
            //    }
            //}
            //else
            //{
            //    gridSealCont.DataSource = _tblFilter;
            //}

            //Console.WriteLine("EditValueChanged");
        }

        private void gridViewSealCont_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            e.Appearance.ForeColor = Color.Red;
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSSealCont();
        }

        private void barEditItem3_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSSealCont();
        }

        private void barEditItem2_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSSealCont();
        }

        private void barEditItem4_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSSealCont();
        }

        private void gridViewSoTheoDoiContainer_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewSoTheoDoiContainer_RowCountChanged(object sender, EventArgs e)
        {
            //GridView gridview = ((GridView)sender);
            //if (!gridview.GridControl.IsHandleCreated) return;
            //Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            //SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            //gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
    }
}
