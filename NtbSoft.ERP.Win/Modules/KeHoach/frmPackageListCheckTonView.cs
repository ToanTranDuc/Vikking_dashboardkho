using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmPackageListCheckTonView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        string _maPhieuXH = string.Empty, _maPKL = string.Empty, _maDH = string.Empty, _poid = string.Empty, _maPKLDisplay = string.Empty;
        bool flagAdd = false, isShowMaDH = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        KeyDownControlHandler keyDownControlHandler;
        public frmPackageListCheckTonView()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
            
        }
       
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtnEditPKL, _allowEdit, ActionType.Edit);
            AddActionControl(_lstActionControl, BtnRefresh, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, BtnAddPKL, _allowAdd, ActionType.LapKH);
            AddActionControl(_lstActionControl, BtnDelete, _allowDelete, ActionType.Delete);
            AddActionControl(_lstActionControl, BtnEX, true, ActionType.ExCel);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType,true);
            list.Add(actionControl);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string[] items = new string[] { "Thời gian", "Mã hàng" };
            cbxLoc.Properties.Items.Clear();

            foreach (string item in items)
            {
                cbxLoc.Properties.Items.Add(item);
            }
            dateTuNgay.EditValue = DateTime.Now;
            dateDenNgay.EditValue = DateTime.Now;
            cbxLoc.EditValue = items[0].ToString();
            Init();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserModuleEntity> List = JsonConvert.DeserializeObject<List<SystemUserModuleEntity>>(json);
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                //btnLapKeHoach.Enabled = false;
            }
            if (!_allowEdit)
            {
                btnAddorEdit.Enabled = false;
                btSave.Enabled = false;
            }
            if (!_allowDelete)
                btnDelete.Enabled = false;
        }
        private void LoadPhieuXuatHang()
        {
            int focusRow = grvPhieuXH.FocusedRowHandle;
            if (searchLookUpEdit1.EditValue == null)
                return;
            _maDH = searchLookUpEdit1.EditValue.ToString();
            string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetPhieuXH&Para1={_maDH}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPhieuXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPhieuXuatHang.Rows.Count == 0)
            {
                grcDSPhieuXH.DataSource = null;
                dgrPKLXuatHang.DataSource = null;
                return;
            }
            grcDSPhieuXH.DataSource = dtPhieuXuatHang;
            if (!flagAdd)
            {
                grvPhieuXH.FocusedRowHandle = -1;
                grvPhieuXH.FocusedRowHandle = focusRow;
                if (focusRow == 0)
                {
                    var dr = grvPhieuXH.GetFocusedDataRow();
                    _maPhieuXH = dr["MaPKL_TK"].ToString();
                    LoadPKLXuatHang();
                }
            }
            else
            {
                if (focusRow == 0)
                {
                    var dr = grvPhieuXH.GetFocusedDataRow();
                    _maPhieuXH = dr["MaPKL_TK"].ToString();
                    LoadPKLXuatHang();
                }
                grvPhieuXH.FocusedRowHandle = 0;
            }
        }
        private void LoadPKLXuatHang()
        {
            if (_maPhieuXH == "") return;
            string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetCTPhieuTK&Para1={_maPhieuXH}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtData = JsonConvert.DeserializeObject<DataTable>(json);

            CreateBandSize(dtData);
            KHDongThungLib.ProcessSttTrung1(dtData);

            DataTable processedData = KHDongThungLib.sumToTalPCS(dtData);
            KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
            dgcTong.DataSource = processedData;
            dgrPKLXuatHang.DataSource = dtData;
            KHDongThungLib.AllowVieworNotPack(dtData, BandPCB, BandPack,BandStore);
        }
        private void CreateBandSize(DataTable dt)
        {
            ClearBand();
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBand(_sizeID)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = this.repotxtN0;
                col.Visible = true;
                col.Width = 40;
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                grvPKLXuatHang.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void ClearBand()
        {
            gbSize.Children.Clear();
        }
        private bool CheckExistBand(string size)
        {

            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        #region Event
        private void grvPhieuXH_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            var dr = grvPhieuXH.GetFocusedDataRow();
            _maPhieuXH = dr["MaPKL_TK"].ToString();
            LoadPKLXuatHang();
        }
        private void grvPKLXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void grvPKLXuatHang_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "SLThung" || e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            {
                var preSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
                var curSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.Column.FieldName == "MaPKLDisplay")
            {
                var preSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle - 1, "MaPKLDisplay");
                var curSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "MaPKLDisplay");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    e.Handled = true;
                    return;
                }
            }


            e.Handled = false;

        }
        private void grvPKLXuatHang_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            //if (e.RowHandle < 0) return;
            //var Check = (bool)grvPKLXuatHang.GetRowCellValue(e.RowHandle, "IsTon");
            //if (Check)
            //    e.Appearance.BackColor = Color.DarkOrange;
        }
        private void grvPKLXuatHang_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = grvPKLXuatHang.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            _maPKL = drFocus["MaPKL"].ToString();
            _poid = drFocus["POID"].ToString();
            _maDH = drFocus["MaDH"].ToString();
            _maPKLDisplay = drFocus["MaPKLDisplay"].ToString();
            DXMenuItem menuViewPKL = new DXMenuItem();
            menuViewPKL.Caption = "Chi tiết PKL";
            menuViewPKL.Click += MenuViewPKL_Click;
            e.Menu.Items.Add(menuViewPKL);
        }

        private void MenuViewPKL_Click(object sender, EventArgs e)
        {
            //frmPackageListXuatHangViewCT fr = new frmPackageListXuatHangViewCT(_maDH, _maPKL, _poid,_maPKLDisplay);
            //fr.ShowDialog();

        }

        private void btnAddorEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnEditPKL();
        }

        private void btnAddNewPKL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnAddPKL();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnDelete();
        }

        #endregion      
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnRefresh();
        }

        #region Manh
        private void BtnAddPKL()
        {
            isShowMaDH = false;
            frmPackageListCheckTon fr = new frmPackageListCheckTon();
            fr.ShowDialog();
            flagAdd = true;
            _maDH = frmPackageListCheckTon._donhang;
            if (_maDH == "") return;
            checkValueLoc();
        }
        private void BtnRefresh()
        {
            flagAdd = false;
            LoadPhieuXuatHang();
        }
        private void BtnEditPKL()
        {
            isShowMaDH = false;
            frmPackageListCheckTon fr = new frmPackageListCheckTon(_maPhieuXH);
            fr.ShowDialog();
            flagAdd = false;
            _maDH = frmPackageListCheckTon._donhang;
            if (_maDH == "") return;
            checkValueLoc();
        }
        private void BtnDelete()
        {
            DialogResult resultDialog = MessageBox.Show("Xác nhận xóa package list?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDialog == DialogResult.Yes)
            {
                string url = string.Format("{0}", URL + $"CheckTonKho/Delete?action=DeletePKLXH&Para={_maPhieuXH}");
                DataSet ds = new DataSet();
                ds.Tables.Add(new DataTable());
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (result.ToLower() == "true")
                {
                    flagAdd = false;
                    LoadPhieuXuatHang();
                }
            }
        }
        private void BtnEX()
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            Sfd.FileName = string.Format("BaoCaoNhapTon_" + DateTime.Now.ToString("ddMMyyyy"));
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


                    ExportExcel(Sfd.FileName);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }
        private void checkValueLoc()
        {
            string cbxText = cbxLoc.Text;
            if (cbxText != "Thời gian")
            {
                layoutDenNgay.Visibility = LayoutVisibility.Never;
                layoutTuNgay.Visibility = LayoutVisibility.Never;
                DateTime tuNgay = Convert.ToDateTime("1990-1-1");
                DateTime denNgay = Convert.ToDateTime("1990-1-1");
                load_DonHang(tuNgay, denNgay);

            }
            else
            {
                layoutDenNgay.Visibility = LayoutVisibility.Always;
                layoutTuNgay.Visibility = LayoutVisibility.Always;
                locNgay();
            }
        }
        private void cbxLoc_Properties_EditValueChanged(object sender, EventArgs e)
        {
            checkValueLoc();
        }
        private void Init()
        {
            searchLookUpEdit1.Properties.DisplayMember = "TenHangDisplay";
            searchLookUpEdit1.Properties.ValueMember = "MaDH";
            searchLookUpEdit1.Properties.NullText = "[Chọn Đơn hàng]";
        }
        private void locNgay()
        {
            DateTime tuNgay = (DateTime)dateTuNgay.EditValue;
            DateTime denNgay = (DateTime)dateDenNgay.EditValue;
            load_DonHang(tuNgay, denNgay);
        }
        private void load_DonHang(DateTime tuNgay, DateTime denNgay)
        {
            searchLookUpEdit1.Properties.DataSource = null;
            grcDSPhieuXH.DataSource = null;
            dgrPKLXuatHang.DataSource = null;
            string url = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetDonHangTong&Para1={tuNgay}&Para2={denNgay}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = tbl;

            if (tbl.Rows.Count == 0) return;
            else
            {
                if (isShowMaDH)
                {
                    searchLookUpEdit1.EditValue = tbl.Rows[0]["MaDH"];
                }
                else
                {
                    searchLookUpEdit1.EditValue = _maDH;
                    isShowMaDH = true;
                }
            }
            LoadPhieuXuatHang();
        }
        DateTime tempTuNgay = DateTime.Now;
        bool checkCoNgay = true;
        private void dateTuNgay_Properties_EditValueChanged(object sender, EventArgs e)
        {
            DateTime tuNgay = (DateTime)dateTuNgay.EditValue;
            DateTime denNgay = (dateDenNgay.EditValue == null) ? DateTime.Now : (DateTime)dateDenNgay.EditValue;
            if (tuNgay > denNgay)
            {
                MessageBox.Show("Ngày bắt đầu không thể lớn hơn ngày kết thúc!");
                dateTuNgay.EditValue = tempTuNgay;
                checkCoNgay = false;
            }
            else
            {
                checkCoNgay = true;
                load_DonHang(tuNgay, denNgay);
            }
            if (checkCoNgay)
                tempTuNgay = tuNgay;

        }

        private void dateDenNgay_Properties_EditValueChanged(object sender, EventArgs e)
        {
            locNgay();
        }

        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            LoadPhieuXuatHang();
        }      

        private void btnEX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnEX();
        }
        private void ExportExcel(string path)
        {
            try
            {
                DataTable dtPhieuXuatHang = grcDSPhieuXH.DataSource as DataTable;

                string PathLoGo = KHDongThungLib.getImgPath("texgiang.jpg");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    foreach (DataRow item in dtPhieuXuatHang.Rows)
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(item["MaPKL_TK"].ToString());
                        ExcelRange range = worksheet.Cells;
                        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        worksheet.Cells.Style.Font.Size = 13;
                        range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PACKING LIST"; range.Style.Font.Bold = true;


                        string url1 = string.Format("{0}", URL + $"CheckTonKho/Get?Action=GetCTPhieuTK&Para1={item["MaPKL_TK"].ToString()}&Para2=Para");
                        string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                        DataTable dtPKLXuatHang = JsonConvert.DeserializeObject<DataTable>(json1);

                        List<string> columnNamesWithSize = dtPKLXuatHang.Columns.Cast<DataColumn>()
                                    .Where(column => column.ColumnName.Contains("@SIZE"))
                                    .Select(column => column.ColumnName)
                                    .Distinct()
                                    .ToList();
                        int colum = 7;
                        //range = worksheet.Cells["A1:B3"]; range.Merge = true;
                        //Image image = Image.FromFile(PathLoGo);
                        //OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                        //picture.SetPosition(0, 0, 0, 0);
                        //picture.SetSize(105, 55);
                        worksheet.Cells.Style.Font.Size = 11;
                        range = worksheet.Cells["D4"]; range.Value = "STYLE :";
                        range = worksheet.Cells["e4:f4"]; range.Merge = true; range.Value = item["MaHang"].ToString();
                        range = worksheet.Cells["D5"]; range.Value = "Shipper: :";
                        range = worksheet.Cells["D6"]; range.Value = "Invoice No : ";
                        range = worksheet.Cells["D7"]; range.Value = "Consignee : ";

                        KHDongThungLib.dtXuatEX(dtPKLXuatHang, worksheet,false);                       
                    }
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }         
        private void grvPKLXuatHang_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e,true);
        }

        private void grvPKLXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e,true);
        }

        #endregion

    }
}
