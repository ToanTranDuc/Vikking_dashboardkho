using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
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
    public partial class frmBaoTongQuanDongThung : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        bool indicatorIcon = true;
        string dateFilter = string.Empty;
        string maHHFilter = string.Empty;
        string maDVSXFilter = string.Empty;
        List<DongThungBaoCaoEntity> lstDsDongThungEntity;
        string json = string.Empty;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlRefresh;
        ActionControl actionControlPrintExcel;
        List<ActionControl> lstActionControls;
        public frmBaoTongQuanDongThung()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDsDongThungEntity = new List<DongThungBaoCaoEntity>();
        }
        protected override void OnLoad(EventArgs e)
        {
            //LoadDataSource();
            CreateSearchLookup();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlRefresh = new ActionControl(NapLai, true, ActionType.Refresh, true);
            actionControlPrintExcel = new ActionControl(XuatExcelTongQuanDongThung, true, ActionType.ExCel, true);

            lstActionControls = new List<ActionControl> {
                actionControlRefresh, actionControlPrintExcel};
            return lstActionControls;
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void CreateSearchLookup()
        {
            string urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblhangHoa = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            repositoryItemHHFilter.DataSource = tblhangHoa;

            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable tblDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            repositoryItemDVSXFilter.DataSource = tblDVSX;
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
            //if (!_allowAdd)
            //{
            //    Them.Enabled = false;
            //}
            //if (!_allowEdit)
            //{
            //    Sua.Enabled = false;
            //    Luu.Enabled = false;
            //}
            //else
            //{
            //    Luu.Enabled = false;
            //}
            //if (!_allowDelete)
            //    Xoa.Enabled = false;

        }

        private void GridView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        private void GridView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void repositoryItemDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;

            if (changing.NewValue != null)
            {
                dateFilter = changing.NewValue.ToString();
            }
            else
            {
                dateFilter = string.Empty;
            }
            // FilterDataLocal(dateFilter, maHHFilter, maDVSXFilter);
        }

        private void repositoryItemSearchHH_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;

            if (changing.NewValue != null)
            {
                maHHFilter = changing.NewValue.ToString();

            }
            else
            {
                maHHFilter = string.Empty;
            }

            // FilterDataLocal(dateFilter, maHHFilter, maDVSXFilter);
        }

        private void repositoryItemDVSXFilter_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;

            if (changing.NewValue != null)
            {
                maDVSXFilter = changing.NewValue.ToString();

            }
            else
            {
                maDVSXFilter = string.Empty;
            }

            // FilterDataLocal(dateFilter, maHHFilter,maDVSXFilter);
        }

        // Filter data allow date, MaKho, MaHang
        //private void FilterDataLocal(string _dateFilter, string _maHHFilter, string _maDVSXFilter)
        //{
        //    List<DongThungBaoCaoEntity> lstDsTheKhoFilter = JsonConvert.DeserializeObject<List<DongThungBaoCaoEntity>>(json);

        //    if (lstDsTheKhoFilter != null && lstDsTheKhoFilter.Count > 0)
        //    {
        //        if (!string.IsNullOrEmpty(_dateFilter))
        //        {
        //            lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.NgayDongThung.Date == DateTime.Parse(_dateFilter).Date)).ToList();
        //        }

        //        if (!string.IsNullOrEmpty(_maHHFilter))
        //        {
        //            lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.MaHang == _maHHFilter)).ToList();
        //        }

        //        if (!string.IsNullOrEmpty(_maDVSXFilter))
        //        {
        //            lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.MaDVSX == _maDVSXFilter)).ToList();
        //        }
        //    }



        //    //if (!string.IsNullOrEmpty(_maHangFilter))
        //    //{
        //    //    lstDsTheKhoFilter = lstDsTheKhoFilter.Where(x => (x.MaHang == _maHangFilter)).ToList();
        //    //}
        //    gridControl1.DataSource = lstDsTheKhoFilter;
        //}
        private void LoadDataSource()
        {
            DateTime _parseDateTime = DateTime.Now;
            if (CheckLoadData(maHHFilter,dateFilter,maDVSXFilter))
            {
                string _maHang = maHHFilter;

                string _maDVSX = maDVSXFilter;

                string url = string.Format("{0}?maHang={1}&&ngayDongThung={2}&&maDVSX={3}"
                    , URL + "DongThung/GetDsTongQuanDongThung", _maHang, dateFilter, _maDVSX);
                string _json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                json = _json;
                lstDsDongThungEntity = JsonConvert.DeserializeObject<List<DongThungBaoCaoEntity>>(_json);
                gridControl1.DataSource = lstDsDongThungEntity;
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn Mã hàng, Ngày đóng thùng và Đơn vị sản xuất để lọc dữ liệu.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }

        }

        private bool CheckLoadData(string _maHH, string _date, string _maDVSX)
        {
            if ((_maHH == null || string.IsNullOrEmpty(_maHH))
                && (_date == null || string.IsNullOrEmpty(_date))
                && (_maDVSX == null || string.IsNullOrEmpty(_maDVSX)))
            {
                return false;
            }
            return true;
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
        private void XuatExcelTongQuanDongThung()
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
                Sfd.FileName = string.Format("baocaotongquandongthung{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    string fileName = "baocaotongquandongthung.xlsx";
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    //gridView1.ExportToXlsx(Sfd.FileName);
                    Export(TemplateFileName, ExportFileName, lstExport);

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
            //}
            //else
            //{
            //    XtraMessageBox.Show("Vui lòng lọc dữ liệu theo Mã DVSX, Mã Hàng và Ngày Lập Phiếu để xuất Excel.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //    return;
            //}
        }
        private void XuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XuatExcelTongQuanDongThung();
        }
        void gridView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            int sum = 0;
            GridView view = (GridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                //Console.WriteLine("gridView_CustomSummaryCalculate");
                string fieldname = (e.Item as GridColumnSummaryItem).FieldName;
                if ((fieldname.Equals(this.gridColumn11.FieldName) || fieldname.Equals(this.gridColumn12.FieldName)))
                {
                    for (int i = 0; i < lstDsDongThungEntity.Count; i++)
                    {
                        //Console.WriteLine("i: " + i);
                        if (!IsFirstRowInGroup(i))
                        {
                            //if (fieldname.Equals(this.gridColumn11.FieldName))
                            //{
                            //    Console.WriteLine("sum_chua_dong: " + lstDsDongThungEntity[i].SoThungChuaDong);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("sum_tong: " + lstDsDongThungEntity[i].TongSoThung);
                            //}
                            sum += fieldname.Equals(this.gridColumn11.FieldName) ? lstDsDongThungEntity[i].SoThungChuaDong : lstDsDongThungEntity[i].TongSoThung;
                        }
                        else
                        {
                            //if (fieldname.Equals(this.gridColumn11.FieldName))
                            //{
                            //    Console.WriteLine("not_sum_chua_dong: " + lstDsDongThungEntity[i].SoThungChuaDong);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("not_sum_tong: " + lstDsDongThungEntity[i].TongSoThung);
                            //}
                        }

                    }
                }
                e.TotalValue = sum;
            }
        }

        // Hàm kiểm tra xem có phải là dòng đầu tiên trong nhóm không
        private bool IsFirstRowInGroup(int rowHandle)
        {
            int prevRowHandle = rowHandle - 1;
            if (prevRowHandle == GridControl.InvalidRowHandle || prevRowHandle < 0)
                return false; ;
            return CheckMergeRow(gridView1, rowHandle, prevRowHandle);
        }
        private void gridView1_CellMerge(object sender, CellMergeEventArgs e)
        {
            // Truy cập đến GridView
            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            // gridColumn11: Số thùng chưa đóng
            // gridColumn12: Tổng số thùng

            // Kiểm tra nếu hai ô thuộc cột cần kiểm tra
            if (e.Column.Equals(this.gridColumn11) || e.Column.Equals(this.gridColumn12))
            {
                // Lấy giá trị của hai ô đang được xem xét

                // So sánh giá trị để quyết định có hợp nhất hay không
                e.Merge = CheckMergeRow(gridView1, e.RowHandle1, e.RowHandle2);
                // Đặt Handled thành true để không cần kiểm tra thêm
                e.Handled = true;
            }
            else
            {
                // Đối với các cột khác, không cho phép hợp nhất
                e.Merge = false;
                e.Handled = true;
            }
        }
        private bool CheckMergeRow(GridView gridView, int rowHandle, int prevRow)
        {
            try
            {
                // POID
                DongThungBaoCaoEntity entity1 = gridView.GetRow(rowHandle) as DongThungBaoCaoEntity;
                DongThungBaoCaoEntity entity2 = gridView.GetRow(prevRow) as DongThungBaoCaoEntity;
                if (entity1 != null && entity2 != null)
                {
                    string poIDValue1 = entity1.POID;
                    string poIDValue2 = entity2.POID;

                    // PO
                    string poValue1 = entity1.PO;
                    string poValue2 = entity2.PO;

                    // MaDVSX
                    var maDVSXValue1 = entity1.MaDVSX;
                    var maDVSXValue2 = entity2.MaDVSX;

                    // MaHang
                    var maHangValue1 = entity1.MaHang;
                    var maHangValue2 = entity2.MaHang;


                    //if (poIDValue1.Equals(poIDValue2) && poValue1.Equals(poValue2) &&
                    //        dauSizeIDValue1.Equals(dauSizeIDValue2) && dauSizeValue1.Equals(dauSizeValue2) &&
                    //        maDVSXValue1.Equals(maDVSXValue2) && maHangValue1.Equals(maHangValue2) &&
                    //        maMauValue1.Equals(maMauValue2) && tenMauValue1.Equals(tenMauValue2))
                    //{
                    //    Console.WriteLine("True_CheckMergeRow - dòng: " + rowHandle);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("False_CheckMergeRow - dòng: " + rowHandle);
                    //}
                    return (poIDValue1.Equals(poIDValue2) && poValue1.Equals(poValue2) &&
                            maDVSXValue1.Equals(maDVSXValue2) && maHangValue1.Equals(maHangValue2));
                }
                return false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void gridView1_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gridView1_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Level1";
        }

        private void gridView1_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            // Giả sử có một phương thức lấy dữ liệu chi tiết dựa trên dòng hiện tại
            Console.WriteLine("MasterRowGetChildList");
            e.ChildList = GetDetailData().DefaultView;
        }

        private DataTable GetDetailData()
        {
            try
            {
                DongThungBaoCaoEntity focusedItem = gridView1.GetFocusedRow() as DongThungBaoCaoEntity;
                DataTable tblFocusedItem = new DataTable();
                tblFocusedItem.Columns.Add("MaDVSX", typeof(string));
                tblFocusedItem.Columns.Add("MaHang", typeof(string)); tblFocusedItem.Columns.Add("POID", typeof(string));
                tblFocusedItem.Columns.Add("PO", typeof(string));
                tblFocusedItem.Columns.Add("DauSizeID", typeof(string));
                tblFocusedItem.Columns.Add("DauSize", typeof(string));
                tblFocusedItem.Columns.Add("MaMau", typeof(string));
                tblFocusedItem.Columns.Add("NgayDongThung", typeof(DateTime));

                DataRow rowFocused = tblFocusedItem.NewRow();
                rowFocused["MaDVSX"] = focusedItem.MaDVSX;
                rowFocused["MaHang"] = focusedItem.MaHang;
                rowFocused["POID"] = focusedItem.POID;
                rowFocused["PO"] = focusedItem.PO;
                rowFocused["DauSizeID"] = focusedItem.DauSizeID;
                rowFocused["DauSize"] = focusedItem.DauSize;
                rowFocused["MaMau"] = focusedItem.MaMau;
                rowFocused["NgayDongThung"] = focusedItem.NgayDongThung;

                tblFocusedItem.Rows.Add(rowFocused);
                //
                string url = string.Format("{0}?", URL + "DongThung/GetChiTietDongThung");
                string _jsonDetail = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblFocusedItem); }).Result;
                DataTable _tblDetail = JsonConvert.DeserializeObject<DataTable>(_jsonDetail);

                // Xử lý chuỗi
                _tblDetail = RemoveDuplicateDataFromChiTietDongThung(_tblDetail);

                // Canh lề
                //_tblDetail = HandleStringFromChiTietDongThung(_tblDetail);
                return _tblDetail;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return new DataTable();
        }

        // Xử lý chuỗi cho Đầu size( DauSize), Màu( TenMau), Size( Size)
        // Chuỗi DauSize: DauSize_1-DauSize_2
        // Chuỗi TenMau: TenMau_1-TenMau_2
        // Chuỗi Size: Size_1-Size_2

        // Nếu chuỗi DauSize, TenMau, Size có tất cả item phân tách bằng kí tự '-' đều giống
        // thì chỉ lấy một item để hiển thị
        private DataTable RemoveDuplicateDataFromChiTietDongThung(DataTable _tblDetail)
        {
            foreach (DataRow row in _tblDetail.Rows)
            {
                foreach (DataColumn column in _tblDetail.Columns)
                {
                    string value = row[column].ToString();
                    if (column.ColumnName.Equals("DauSize") || column.ColumnName.Equals("TenMau")
                        || column.ColumnName.Equals("Size") || column.ColumnName.Equals("SoLuongSP")
                        || column.ColumnName.Equals("PO"))
                    {
                        if (value.Contains('@') && value.Split('@').Length > 1)
                        {
                            List<string> lstValue = value.Split('@').ToList();
                            bool checkDuplicate = lstValue.GroupBy(x => x).Any(g => g.Count() == lstValue.Count);
                            if (checkDuplicate && !column.ColumnName.Equals("SoLuongSP"))
                            {
                                row[column] = lstValue[0];
                            }
                            else
                            {
                                row[column] = value.Replace("@", "  -  ");
                            }
                        }
                    }
                }
            }
            return _tblDetail;
        }

        // Xử lý chuỗi cho Đầu size( DauSize), Màu( TenMau), Size( Size), Số lượng Sản phẩm( SoLuongSP)
        // Canh lề
        private DataTable HandleStringFromChiTietDongThung(DataTable _tblDetail)
        {
            foreach (DataRow row in _tblDetail.Rows)
            {
                string columnName = string.Empty;
                int maxLengthValue = -1;

                // Tìm value có chứa kí tự '@' và giá trị có độ dài lớn nhất làm chuẩn để canh lề
                foreach (DataColumn column in _tblDetail.Columns)
                {
                    if (column.ColumnName.Equals("DauSize") || column.ColumnName.Equals("TenMau")
                        || column.ColumnName.Equals("Size") || column.ColumnName.Equals("SoLuongSP"))
                    {
                        if (row[column] != null && row[column].ToString().Contains("@"))
                        {
                            string value = row[column].ToString();

                            if (value.Length > maxLengthValue)
                            {
                                maxLengthValue = value.Length;
                                columnName = column.ColumnName;
                            }
                        }
                    }
                }

                //if (int.Parse(row["SttThung"].ToString()) == 15)
                //{
                //    Console.WriteLine("15");
                //}

                // Canh lề
                foreach (DataColumn column in _tblDetail.Columns)
                {
                    if (column.ColumnName.Equals("DauSize") || column.ColumnName.Equals("TenMau")
                        || column.ColumnName.Equals("Size") || column.ColumnName.Equals("SoLuongSP"))
                    {
                        if (row[column] != null && row[column].ToString().Contains("@"))
                        {
                            string value = row[column].ToString();
                            int _whiteSpaceFirst = (maxLengthValue - value.Length) / 2;
                            int _whiteSpaceBack = (maxLengthValue - value.Length - _whiteSpaceFirst);

                            row[column] = string.Format("{0}{1}{2}", new string(' ', _whiteSpaceFirst), value, new string(' ', _whiteSpaceBack));
                        }
                    }
                }
            }
            return _tblDetail;
        }

        private void gridControl1_ViewRegistered(object sender, ViewOperationEventArgs e)
        {
            if (e.View is CardView)
            {
                CardView cardView = e.View as CardView;
                // Cấu hình card view ở đây
                //cardView.CardCaptionFormat = "Chi tiết thùng";

                //Console.WriteLine("focused_row: " + cardView.GetFocusedDataSourceRowIndex());
            }
        }

        private void gridView1_MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e)
        {
            //e.Caption = "Tên mới cho Level 2";
            e.RelationName = "Chi tiết thùng đã đóng";
        }

        private void cardView1_CustomDrawCardCaption(object sender, CardCaptionCustomDrawEventArgs e)
        {
            CardView _cardView = sender as CardView;
            DataRow row = (_cardView.GetRow(e.RowHandle) as DataRowView).Row;
            if (row != null)
            {
                //Console.WriteLine("custom_draw_card_caption");
                //Console.WriteLine("STT thùng: " + row["SttThung"].ToString());
                e.CardCaption = string.Format("Chi tiết thùng: {0}", row["SttThung"].ToString());
            }
            else
            {
                e.CardCaption = string.Format("Chi tiết thùng: _");
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDataSource();
        }

        public void Export(string TemplateFileName, string ExportFileName, List<DongThungBaoCaoEntity> _lstTongQuanDongThung)
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

                        // Xét dữ liệu cho các field thông tin kho: Kho, Ngày lập thẻ

                        //// Ô dữ liệu C3: Kho
                        //DataRow dataRowDVSX = tblDVSX.AsEnumerable().Where(x => x["MaDVSX"].ToString().Equals(lstTheKho[0].MaKho)).FirstOrDefault();
                        //if (dataRowDVSX != null && dataRowDVSX["TenDVSX"] != null)
                        //{
                        //    string tenKho = dataRowDVSX["TenDVSX"].ToString();
                        //    ws.Cells[3, 3].Value = string.Format("Kho - Warehouse Address: {0}", tenKho);
                        //}

                        //// Ô dữ liệu A4: Ngày lập thẻ
                        //if (lstTheKho[0].NgayLapThe != null && lstTheKho[0].ToSo != null)
                        //{
                        //    ws.Cells[4, 1].Value = string.Format("Ngày lập thẻ - Date: {0}                                                                                               Tờ số: {1}", lstTheKho[0].NgayLapThe.ToString("dd/MM/yyyy"), lstTheKho[0].ToSo);
                        //    ws.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //}

                        //// Ô dữ liệu A5: Mã hàng
                        //DataRow dataRowHangHoa = tblHangHoa.AsEnumerable().Where(x => x["MaHang"].ToString().Equals(lstTheKho[0].MaHang)).FirstOrDefault();
                        //if (dataRowHangHoa != null && dataRowHangHoa["TenHang"] != null)
                        //{
                        //    string tenHang = dataRowHangHoa["TenHang"].ToString();
                        //    ws.Cells[5, 1].Value = string.Format("Tên thiết bị, vật tư, hàng hóa - Equiments, Material, Products: {0}", tenHang);
                        //}

                        //// Ô dữ liệu A6: Mã hàng
                        //DataRow dataRowMaKH = tblKhachHang.AsEnumerable().Where(x => x["MaKH"].ToString().Equals(lstTheKho[0].MaKhachHang)).FirstOrDefault();
                        //if (dataRowMaKH != null && dataRowMaKH["TenKH"] != null)
                        //{
                        //    string tenKH = dataRowMaKH["TenKH"].ToString();
                        //    ws.Cells[6, 1].Value = string.Format("Khách hàng - Customer: {0}", tenKH);
                        //}

                        int sumSoThungDaDong = 0;
                        int sumSoThungChuaDong = 0;
                        int sumTongSoThung = 0;

                        // countRow là hàng bắt đầu thêm dữ liệu của gridview
                        int countRow = 3;
                        for (int i = 0; i < _lstTongQuanDongThung.Count; i++)
                        {
                            //DataRow dataRowDV = tblDonVi.AsEnumerable().Where(x => x["MaDV"].ToString().Equals(lstTheKho[i].MaDV)).FirstOrDefault();
                            //string tenDV = string.Empty;
                            //if (dataRowDV != null && dataRowDV["TenDV"] != null)
                            //{
                            //    tenDV = dataRowDV["TenDV"].ToString();
                            //}

                            ws.Row(countRow).Height = 14;
                            ws.Cells[countRow, 1].Value = _lstTongQuanDongThung[i].NgayDongThung.ToString("dd-MM-yyyy");
                            ws.Cells[countRow, 2].Value = _lstTongQuanDongThung[i].TenDVSX;
                            //ws.Cells[countRow, 3].Value = _lstTongQuanDongThung[i].MaDH;
                            ws.Cells[countRow, 3].Value = _lstTongQuanDongThung[i].TenHang;
                            ws.Cells[countRow, 4].Value = _lstTongQuanDongThung[i].PO;
                            ws.Cells[countRow, 5].Value = _lstTongQuanDongThung[i].SoThungDaDong;
                            ws.Cells[countRow, 6].Value = _lstTongQuanDongThung[i].SoThungChuaDong;
                            ws.Cells[countRow, 7].Value = _lstTongQuanDongThung[i].TongSoThung;

                            sumSoThungDaDong += _lstTongQuanDongThung[i].SoThungDaDong;
                            if (CheckMergeRow(this.gridView1, i, i - 1))
                            {
                                // merge cột Số thùng chưa đóng
                                // H là cột chứa SoThungChuaDong
                                string strMergeSoThungChuaDong = string.Format("F{0}:F{1}", countRow - 1, countRow);
                                ws.Cells[strMergeSoThungChuaDong].Merge = true;

                                // merge cột Tổng số thùng
                                // I là cột chứa TongSoThung
                                string strMergeTongSoThung = string.Format("G{0}:G{1}", countRow - 1, countRow);
                                ws.Cells[strMergeTongSoThung].Merge = true;
                            }
                            else
                            {
                                sumSoThungChuaDong += _lstTongQuanDongThung[i].SoThungChuaDong;
                                sumTongSoThung += _lstTongQuanDongThung[i].TongSoThung;
                            }

                            countRow++;
                        }
                        string strMergeTong = string.Format("A{0}:D{1}", countRow, countRow);
                        ws.Cells[strMergeTong].Merge = true;
                        ws.Cells[countRow, 1].Value = "Tổng";
                        ws.Cells[countRow, 1].Style.Font.Color.SetColor(Color.Red);
                        ws.Cells[countRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells[countRow, 5].Value = sumSoThungDaDong;
                        ws.Cells[countRow, 5].Style.Font.Color.SetColor(Color.Red);
                        ws.Cells[countRow, 6].Value = sumSoThungChuaDong;
                        ws.Cells[countRow, 7].Style.Font.Color.SetColor(Color.Red);
                        ws.Cells[countRow, 7].Value = sumTongSoThung;
                        ws.Cells[countRow, 7].Style.Font.Color.SetColor(Color.Red);


                        // 7: Là số cột
                        // 3: Là Row bắt đầu
                        DrawStyleCellExcel(ws, 7, 3, _lstTongQuanDongThung.Count + 1);

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
