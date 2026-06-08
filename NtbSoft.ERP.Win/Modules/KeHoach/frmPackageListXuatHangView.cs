using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
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
using DevExpress.XtraGrid.Columns;
using NtbSoft.ERP.Win.Properties;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmPackageListXuatHangView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        string _maPhieuXH = string.Empty, _maPKL = string.Empty, _maDH = string.Empty, _tenhang = string.Empty, _poid = string.Empty, _po = string.Empty, _maPKLDisplay = string.Empty,
                    _statusXH = string.Empty, _statusTonKho = string.Empty, _maKH = string.Empty, _maCont = string.Empty;
        bool flagAdd = false, isShowMaDH = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private DataTable dtDonHang = new DataTable();
        private DataTable dtPhieuHang = new DataTable();
        KeyDownControlHandler keyDownControlHandler;
        Dictionary<string, QuantityInfo> dicSLThungPO = new Dictionary<string, QuantityInfo>();
        public frmPackageListXuatHangView()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string[] items = new string[] { "Thời gian", "Mã hàng", "Phiếu" };
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
            dateTuNgay.EditValueChanged += DateTuNgay_EditValueChanged;
            dateDenNgay.EditValueChanged += DateDenNgay_EditValueChanged;
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
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
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
        private void LoadPhieuXuatHang(bool flagLocMH = false, bool flagCheckPhieu = false)
        {
            int focusRow = grvPhieuXH.FocusedRowHandle;
            string _maPhieu = null;
            //if (focusRow < 0) return;

            if (searchLookUpEdit1.EditValue != null && flagLocMH)
            {
                _maDH = searchLookUpEdit1.EditValue.ToString();
            }

            string tuNgay = ((DateTime)dateTuNgay.EditValue).ToString("yyyy-MM-dd");
            string denNgay = ((DateTime)dateDenNgay.EditValue).ToString("yyyy-MM-dd");
            string Para1 = flagLocMH ? "1990-01-01" : tuNgay;
            string Para2 = flagLocMH ? "1990-01-01" : denNgay;
            if (flagCheckPhieu)
            {
                Para1 = "1990-01-01";
                Para2 = "1990-01-02";
                _maDH = searchLookUpEdit2.EditValue.ToString();
                flagLocMH = true;
            }


            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetPhieuXH&Para1={Para1}&Para2={Para2}&Para3={(flagLocMH ? _maDH : "Para")}");
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
                if (focusRow == 0)
                {
                    var dr = grvPhieuXH.GetFocusedDataRow();
                    _maPhieuXH = dr["MaPKL_XH"]?.ToString();
                    LoadPKLXuatHang();
                    return;
                }
                grvPhieuXH.FocusedRowHandle = focusRow;
                //LoadPKLXuatHang();
            }
            else
            {
                if (focusRow == 0)
                {
                    var dr = grvPhieuXH.GetFocusedDataRow();
                    _maPhieuXH = dr["MaPKL_XH"].ToString();
                    LoadPKLXuatHang();
                }
                grvPhieuXH.FocusedRowHandle = 0;
            }
        }
        private void LoadPKLXuatHang()
        {
            if (_maPhieuXH == "") return;
            DataTable dtData = GetDataXuatHang(_maPhieuXH);
            var dtTemp = dtData.Copy();
            CreateBandSize(dtData, grvPKLXuatHang, gbSize);
            KHDongThungLib.ProcessSttTrung1(dtData);
            CalSLThungPO(dtData);
            if (CheckBoxIsReal(dtData)) BandThungReal.Visible = true;
            else BandThungReal.Visible = false;
            dtData = KHDongThungLib.CalculatorTB(dtData);
            dgrPKLXuatHang.DataSource = dtData;
            grvPKLXuatHang.ExpandAllGroups();
            DataTable processedData = KHDongThungLib.sumToTalPCS(dtTemp);
            KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
            dgcTong.DataSource = processedData;
            KHDongThungLib.AllowVieworNotPackV2(dtData, BandMaHang, BandDot);
            KHDongThungLib.AllowVieworNotPack(dtData, BandPCB, BandPack, BandStore);
        }
        private DataTable GetDataXuatHang(string maPhieu)
        {
            DataTable dtData = new DataTable();
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXH&Para1={maPhieu}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang1 = JsonConvert.DeserializeObject<DataTable>(json);
            url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetCTPhieuXHTon&Para1={maPhieu}&Para2=Para");
            json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang2 = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPKLXuatHang2 is null) dtData = dtPKLXuatHang1;
            else
            {
                dtData = dtPKLXuatHang2;
                dtData.Merge(dtPKLXuatHang1);
            }
            return dtData;
        }
        private bool CheckBoxIsReal(DataTable dt)
        {
            if (dt is null) return false;
            var check = dt.AsEnumerable().Any(x => x["TuThung"].ToString() != x["TuThung_XHR"].ToString() || x["DenThung"].ToString() != x["DenThung_XHR"].ToString());
            return check;
        }
        private void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
                if (!CheckExistBand(_sizeID, gbSizeA)) continue;
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
                if (grvShared == dgwTong)
                {
                    if (col.FieldName.Contains("@"))
                    {
                        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                        itemSize.FieldName = col.FieldName;
                        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        itemSize.DisplayFormat = "{0:n0}";
                        itemSize.ShowInGroupColumnFooter = col;
                        grvShared.GroupSummary.Add(itemSize);
                    }

                    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrName.Length > 1)
                    {
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    }
                }
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });
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
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }
        private bool CheckExistBand(string size, GridBand gbSizeA)
        {

            GridBand gbCheck = gbSizeA.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        #region Event
        private void grvPhieuXH_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            var dr = grvPhieuXH.GetFocusedDataRow();
            _maDH = dr["MaDH"].ToString();
            _statusXH = dr["IsExport"].ToString();
            _tenhang = dr["MaHang"].ToString();
            _maKH = dr["MaKH"].ToString();
            _maCont = dr["MaCont"].ToString();
            _poid = dr["POID"].ToString();
            _po = dr["PO"].ToString();
            //if (_maPhieuXH == dr["MaPKL_XH"].ToString()) return;
            _maPhieuXH = dr["MaPKL_XH"].ToString();

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

                var prePKL = grvPKLXuatHang.GetRowCellValue(e.RowHandle - 1, "MaPKLDisplay");
                var curPKL = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "MaPKLDisplay");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung) && prePKL != null && curSTTThung != null && prePKL.Equals(curPKL))
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
            if (e.RowHandle < 0) return;
            var Check = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "IsTon");
            if (Check.ToString() != "0")
                e.Appearance.BackColor = Color.FromArgb(255, 255, 153);
        }
        private void grvPKLXuatHang_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = grvPKLXuatHang.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            _maPKL = drFocus["MaPKL"].ToString();
            _poid = drFocus["POID"].ToString();
            _maDH = drFocus["MaDH"].ToString();
            _maPKLDisplay = drFocus["MaPKLDisplay"].ToString();
            _statusTonKho = drFocus["IsTon"].ToString();
            DXMenuItem menuViewPKL = new DXMenuItem();
            menuViewPKL.Caption = "Chi tiết PKL";
            menuViewPKL.Click += MenuViewPKL_Click;
            e.Menu.Items.Add(menuViewPKL);
        }

        private void MenuViewPKL_Click(object sender, EventArgs e)
        {
            string controller = "KeHoachDongThung";
            if (_statusTonKho == "1") controller = "KeHoachDongThungTonKho";
            frmPackageListXuatHangViewCT fr = new frmPackageListXuatHangViewCT(controller, _maDH, _maPKL, _poid, _maPKLDisplay, _statusTonKho == "2" ? "1" : _statusTonKho);
            fr.ShowDialog();

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
        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPKL", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("DotSX", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("ColorID", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("NgayLapKH", typeof(DateTime));
            tbl.Columns.Add("ChieuDai", typeof(double));
            tbl.Columns.Add("ChieuRong", typeof(double));
            tbl.Columns.Add("ChieuCao", typeof(double));
            tbl.Columns.Add("TrongLuong", typeof(double));
            tbl.Columns.Add("KhoiLuong", typeof(double));
            tbl.Columns.Add("SoLuongThung", typeof(int));
            tbl.Columns.Add("TuThung", typeof(int));
            tbl.Columns.Add("DenThung", typeof(int));
            tbl.Columns.Add("SttThung", typeof(int));
            tbl.Columns.Add("SoLuongSP", typeof(int));
            tbl.Columns.Add("IsDongThung", typeof(bool));
            tbl.Columns.Add("NgayDongThung", typeof(DateTime));
            tbl.Columns.Add("QRCode", typeof(string));
            tbl.Columns.Add("IsScan", typeof(bool));
            tbl.Columns.Add("IsNhapKho", typeof(bool));
            tbl.Columns.Add("NgayNhapKho", typeof(DateTime));
            tbl.Columns.Add("KyHieu", typeof(string));
            tbl.Columns.Add("Chon", typeof(bool));
            tbl.Columns.Add("SttThung_decat", typeof(int));
            tbl.Columns.Add("IsThungLe", typeof(bool));
            return tbl;
        }



        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnRefresh();
        }
        private void BtnAddPKL()
        {
            isShowMaDH = false;
            frmPackageListXuatHangV1 fr = new frmPackageListXuatHangV1();
            fr.ShowDialog();
            flagAdd = true;
            //_maDH = frmPackageListXuatHangV1._donHang;
            //if (_maDH == "") isShowMaDH = true;
            checkValueLoc();
        }
        private void BtnRefresh()
        {
            flagAdd = false;
            LoadPhieuXuatHang(cbxLoc.Text != "Thời gian" ? true : false);
        }
        private void BtnEditPKL()
        {
            isShowMaDH = false;
            string ShippingCode = grvPhieuXH.FocusedRowHandle < 0 ?  ""  : grvPhieuXH.GetFocusedRowCellValue(colShippingCode)?.ToString();
            frmPackageListXuatHangV1 fr = new frmPackageListXuatHangV1(_maPhieuXH, _maDH, _maKH, ShippingCode);
            fr.ShowDialog();
            flagAdd = false;
            //_maDH = frmPackageListXuatHangV1._donHang;
            //if (_maDH == "") isShowMaDH = true;
            checkValueLoc();
        }
        private void BtnDelete()
        {
            var dr = grvPhieuXH.GetFocusedDataRow();
            var statucCheck = dr["IsExport"].ToString();

            if (statucCheck == "2" || statucCheck == "3")
            {
                MessageBox.Show("Phiếu đã được xuất hàng đi. Không thể xóa!", "Thông báo");
                return;
            }
            DialogResult resultDialog = MessageBox.Show("Xác nhận xóa package list?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDialog == DialogResult.Yes)
            {
                string url = string.Format("{0}", URL + $"XuatHang/Delete?action=DeletePKLXH&Para={_maPhieuXH + "||" + _maCont}");
                DataSet ds = new DataSet();
                ds.Tables.Add(new DataTable());
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (result.ToLower() == "true")
                {
                    flagAdd = false;
                    LoadPhieuXuatHang(cbxLoc.EditValue.ToString() != "Thời gian" ? true : false);
                }
            }
        }
        private void btnDeletePO_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dr = grvPhieuXH.GetFocusedDataRow();
            var statucCheck = dr["IsExport"].ToString();
            if (statucCheck == "2" || statucCheck == "3")
            {
                MessageBox.Show("Phiếu đã được xuất hàng đi. Không thể xóa!", "Thông báo");
                return;
            }
            DialogResult resultDialog = MessageBox.Show($"Xác nhận xóa PO: {_po} trong package list?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDialog == DialogResult.Yes)
            {
                string url = string.Format("{0}", URL + $"XuatHang/Delete?action=DeletePKLXH_PO&Para={_maPhieuXH + "||" + _maCont + "||" + _poid}");
                DataSet ds = new DataSet();
                ds.Tables.Add(new DataTable());
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (result.ToLower() == "true")
                {
                    flagAdd = false;
                    LoadPhieuXuatHang(cbxLoc.EditValue.ToString() != "Thời gian" ? true : false);
                }
            }
        }
        private void BtnEX()
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";

            Sfd.FileName = string.Format("PhieuXuatHang_{0}_" + DateTime.Now.ToString("ddMMyyyy"), _maPhieuXH);
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
            if (cbxText == "Mã hàng")
            {
                layoutDenNgay.Visibility = LayoutVisibility.Never;
                layoutTuNgay.Visibility = LayoutVisibility.Never;
                layoutControlItemMP.Visibility = LayoutVisibility.Never;
                DateTime tuNgay = Convert.ToDateTime("1990-1-1");
                DateTime denNgay = Convert.ToDateTime("1990-1-1");
                layoutControlItemMH.Visibility = LayoutVisibility.Always;
                load_DonHang(tuNgay, denNgay);
            }
            else if (cbxText == "Phiếu")
            {
                layoutControlItemMH.Visibility = LayoutVisibility.Never;
                layoutDenNgay.Visibility = LayoutVisibility.Never;
                layoutTuNgay.Visibility = LayoutVisibility.Never;
                layoutControlItemMP.Visibility = LayoutVisibility.Always;
                locPhieu();
            }
            else
            {

                layoutControlItemMH.Visibility = LayoutVisibility.Never;
                layoutControlItemMP.Visibility = LayoutVisibility.Never;
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

            searchLookUpEdit2.Properties.DisplayMember = "PhieuHangDisplay";
            searchLookUpEdit2.Properties.ValueMember = "MaPhieu";
            searchLookUpEdit2.Properties.NullText = "[Chọn Phiếu hàng]";
        }

        private void locNgay()
        {
            LoadPhieuXuatHang();
            //load_DonHang(tuNgay, denNgay);
        }
        private void load_DonHang(DateTime tuNgay, DateTime denNgay)
        {
            searchLookUpEdit1.Properties.DataSource = null;
            grcDSPhieuXH.DataSource = null;
            dgrPKLXuatHang.DataSource = null;
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetDonHangTong&Para1={tuNgay}&Para2={denNgay}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = dtDonHang;

            if (dtDonHang.Rows.Count == 0) return;
            else
            {
                if (isShowMaDH)
                {
                    searchLookUpEdit1.EditValue = dtDonHang.Rows[0]["MaDH"];
                }
                else
                {
                    searchLookUpEdit1.EditValue = _maDH;
                    isShowMaDH = true;
                }
            }
            LoadPhieuXuatHang(true);
        }
        private void locPhieu()
        {
            searchLookUpEdit2.Properties.DataSource = null;
            grcDSPhieuXH.DataSource = null;
            dgrPKLXuatHang.DataSource = null;
            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetListPhieuXH&Para1=para&Para2=para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            dtPhieuHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit2.Properties.DataSource = dtPhieuHang;

            if (dtPhieuHang.Rows.Count == 0) return;
            else
            {
                searchLookUpEdit2.EditValue = dtPhieuHang.Rows[0]["MaPhieu"];
            }
            LoadPhieuXuatHang(flagCheckPhieu: true);
        }
        private void CalSLThungPO(DataTable dt)
        {
            dicSLThungPO.Clear();
            var dtCopy = dt.Copy();
            DataRow drOld = dt.NewRow();
            foreach (DataRow dr in dtCopy.Rows)
            {
                if (dr["KieuLapPCB"].ToString() == "4") {
                    //grvPKLXuatHang.OptionsView.ShowGroupPanel = false;
                    //grvPKLXuatHang.OptionsCustomization.AllowGroup = false;
                    grvPKLXuatHang.Columns["PO"].GroupIndex = -1;
                    return;
                }
                else
                {
                    grvPKLXuatHang.Columns["PO"].GroupIndex = 0;
                }
                if (drOld["MaPKLDisplay"].ToString() == dr["MaPKLDisplay"].ToString() && drOld["SttThung"].ToString() == dr["SttThung"].ToString())
                {
                    dr["SLThung"] = 0;
                    dr["TotalPiece"] = 0;
                }
                else drOld = dr;
            }
            var dtDistinctPO = dtCopy.AsEnumerable().Select(x => x["POID_G"].ToString()).Distinct().ToList();

            foreach (var item in dtDistinctPO)
            {
                int SumT = dtCopy.AsEnumerable().Where(x => x["POID_G"].ToString() == item).Sum(y => Convert.ToInt16(y["SLThung"]));
                int SumSP = dtCopy.AsEnumerable().Where(x => x["POID_G"].ToString() == item).Sum(y => Convert.ToInt16(y["TotalPiece"]));
                dicSLThungPO.Add(item, new QuantityInfo()
                {
                    SLThung = SumT,
                    SLSP = SumSP
                });
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = grvPhieuXH.GetFocusedDataRow();
            if (dr == null) return;
            if (dr["IsExport"].ToString() == "2")
            {
                MessageBox.Show("Phiếu xuất này không phải là gia công không được sửa!");
                return;
            }

            frmPKLXuatHangGiaCong frm = new frmPKLXuatHangGiaCong(dr);
            frm.ShowDialog();
            bool check = frmPKLXuatHangGiaCong._checkSave;
            if (check)
                BtnRefresh();


        }

        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            LoadPhieuXuatHang(true, flagCheckPhieu: true);
        }



        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (cbxThungXuatReal.Checked)
                BandThungReal.Visible = true;
            else
                BandThungReal.Visible = false;
        }



        private void grvPKLXuatHang_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            if (info.Column == colPOGr)
            {
                var value = view.GetGroupRowValue(info.RowHandle, new GridColumn());
                dicSLThungPO.TryGetValue(view.GetGroupRowValue(info.RowHandle, colPOID).ToString(), out QuantityInfo infoQ);
                if (infoQ is null) return;
                info.GroupText = $"PO: {view.GetGroupRowValue(info.RowHandle, colPOG)} -- SL SP: {infoQ.SLSP} -- SL Thùng: {infoQ.SLThung}";
            }
        }

        DateTime tempTuNgay = DateTime.Now;

      

        bool checkCoNgay = true;
        private void DateDenNgay_EditValueChanged(object sender, EventArgs e)
        {
            locNgay();
        }

        private void DateTuNgay_EditValueChanged(object sender, EventArgs e)
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
                LoadPhieuXuatHang();
            }
            if (checkCoNgay)
                tempTuNgay = tuNgay;
        }
        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            LoadPhieuXuatHang(true);

        }

        #region Luan
        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnEX();
        }
        private void ExcelTitle(ExcelWorksheet worksheet, ExcelRange range, string _mahang)
        {
            string PathLoGo = KHDongThungLib.getImgPath("texgiang.jpg");

            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Cells.Style.Font.Name = "Times New Roman";
            worksheet.Cells.Style.Font.Size = 13;
            range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
            range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PACKING LIST"; range.Style.Font.Bold = true;

            range = worksheet.Cells["A1:B3"]; range.Merge = true;
            //Image image = Image.FromFile(PathLoGo);
            //OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
            //picture.SetPosition(0, 0, 0, 0);
            //picture.SetSize(105, 55);

            worksheet.Cells.Style.Font.Size = 11;
            range = worksheet.Cells["D4"]; range.Value = "STYLE :";
            range = worksheet.Cells["e4:f4"]; range.Merge = true; range.Value = _mahang;
            range = worksheet.Cells["D5"]; range.Value = "Shipper: :";
            range = worksheet.Cells["D6"]; range.Value = "Invoice No : ";
            range = worksheet.Cells["D7"]; range.Value = "Consignee : ";
        }
        private void ExportExcel(string path)
        {
            try
            {
                DataTable dtPhieuXuatHang = grcDSPhieuXH.DataSource as DataTable;
                dtPhieuXuatHang = dtPhieuXuatHang.AsEnumerable().Where(x => x["MaPKL_XH"].ToString() == _maPhieuXH.ToString()).CopyToDataTable();
                if (dtPhieuXuatHang.Rows.Count == 0) return;
                var lstMaDH = dtPhieuXuatHang.AsEnumerable().Select(x => new
                {
                    DH = x["MaDH"].ToString(),
                    MaHang = x["MaHang"].ToString(),
                }).Distinct().ToList();


                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    foreach (var item in lstMaDH)
                    {
                        string mahang = item.DH.ToString();
                        string check = item.MaHang.ToString();
                        string action = check == "" ? "GetPivotKHDongThung_SMS" : "GetPivotKHDongThung";
                        string urlTbl = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={mahang}&MaDVSX=A&DotSX=Para&POID=A&SizeTypeID=A&ColorID=Para&MaPKL=A&ProductID=Para&SizeID=Para");
                        string jsonTbl = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTbl); }).Result;
                        DataTable renderTable = JsonConvert.DeserializeObject<DataTable>(jsonTbl);

                        DataTable orderTable = renderTable;
                        orderTable = KHDongThungLib.CalculatorTB(orderTable);
                        foreach (DataRow dr in orderTable.Rows)
                        {
                            dr["KyHieu"] = dr["KyHieuA"];
                        }
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(mahang + "Total");
                        ExcelRange range = worksheet.Cells;
                        ExcelTitle(worksheet, range, orderTable.Rows[0]["TenHang"].ToString());
                        KHDongThungLib.dtXuatEX(orderTable, worksheet, false);
                    }

                    bool checkIndex = true;
                    int index = 0;
                    int indexCont = 1;
                    List<string> MahangGop = dtPhieuXuatHang.AsEnumerable().Select(x => x["MaHang"].ToString()).Distinct().ToList();
                    string result = string.Join(" + ", MahangGop);
                    var dtExport = dtPhieuXuatHang.AsEnumerable().Select(x => new
                    {
                        MaPKL_XH = x["MaPKL_XH"].ToString(),
                        // MaHang = x["MaHang"].ToString(),
                        Cont = x["MaCont"].ToString(),
                        TenCont = x["Cont"].ToString(),
                        Seal = x["Seal"].ToString(),


                    }).Distinct().ToList();
                    foreach (var item in dtExport)
                    {
                        if (checkIndex) indexCont = 1;
                        else indexCont++;

                        DataTable dtPKLXuatHang = GetDataXuatHang(item.MaPKL_XH);
                        KHDongThungLib.sumToTalPCS(dtPKLXuatHang);
                        dtPKLXuatHang = dtPKLXuatHang.AsEnumerable().Where(x => x["Cont"].ToString() == item.Cont.ToString()).CopyToDataTable();
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(item.MaPKL_XH + "_" + item.TenCont);
                        ExcelRange range = worksheet.Cells;
                        range = worksheet.Cells;

                        string mahang = result;

                        ExcelTitle(worksheet, range, mahang);

                        range = worksheet.Cells["D8"]; range.Value = "Cont :";
                        range = worksheet.Cells["e8:f8"]; range.Merge = true; range.Value = item.TenCont;
                        range = worksheet.Cells["H8:I8"]; range.Merge = true; range.Value = "Seal :" + item.Seal;
                        dtPKLXuatHang = KHDongThungLib.CalculatorTB(dtPKLXuatHang);

                        KHDongThungLib.dtXuatEX(dtPKLXuatHang, worksheet, false, 9, null, false, false, true);


                        string renMaPKL = (index + 1 < dtPhieuXuatHang.Rows.Count) ? dtPhieuXuatHang.Rows[index + 1]["MaPKL_XH"].ToString() : "aaaabbbbb";
                        if (renMaPKL == item.MaPKL_XH) checkIndex = false;
                        else checkIndex = true;
                        index++;
                    }
                    //foreach (DataRow item in dtPhieuXuatHang.Rows)
                    //{
                    //    if (checkIndex) indexCont = 1;
                    //    else indexCont++;

                    //    DataTable dtPKLXuatHang = GetDataXuatHang(item["MaPKL_XH"].ToString());
                    //    KHDongThungLib.sumToTalPCS(dtPKLXuatHang);
                    //    dtPKLXuatHang = dtPKLXuatHang.AsEnumerable().Where(x => x["Cont"].ToString() == item["Cont"].ToString()).CopyToDataTable();
                    //    worksheet = excelPackage.Workbook.Worksheets.Add(item["MaPKL_XH"].ToString() + "_" + item["Cont"].ToString() + "_" + indexCont);
                    //    range = worksheet.Cells;

                    //    string mahang = item["MaHang"].ToString();

                    //    ExcelTitle(worksheet, range, mahang);

                    //    range = worksheet.Cells["D8"]; range.Value = "Cont :";
                    //    range = worksheet.Cells["e8:f8"]; range.Merge = true; range.Value = item["Cont"].ToString();
                    //    range = worksheet.Cells["H8:I8"]; range.Merge = true; range.Value = "Seal :" + item["Seal"].ToString();

                    //    KHDongThungLib.dtXuatEX(dtPKLXuatHang, worksheet, false);


                    //    string renMaPKL = (index + 1 < dtPhieuXuatHang.Rows.Count) ? dtPhieuXuatHang.Rows[index + 1]["MaPKL_XH"].ToString() : "aaaabbbbb";
                    //    if (renMaPKL == item["MaPKL_XH"].ToString()) checkIndex = false;
                    //    else checkIndex = true;
                    //    index++;
                    //}
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        //private void ExportExcel(string path)
        //{
        //    try
        //    {
        //        DataTable dtPhieuXuatHang = grcDSPhieuXH.DataSource as DataTable;
        //        if (dtPhieuXuatHang.Rows.Count == 0) return;
        //        //dtPhieuXuatHang = dtPhieuXuatHang.AsEnumerable().OrderBy(x => x["MaPKL_XH"]).CopyToDataTable();

        //        string urlTbl = string.Format("{0}?", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={_maDH}&MaDVSX=A&DotSX=Para&POID=A&SizeTypeID=A&ColorID=Para&MaPKL=A&ProductID=Para&SizeID=Para");
        //        string jsonTbl = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTbl); }).Result;
        //        DataTable renderTable = JsonConvert.DeserializeObject<DataTable>(jsonTbl);

        //        DataTable orderTable = renderTable.AsEnumerable().OrderBy(x => x["PO"].ToString()).ThenBy(x => x["MaPKL"].ToString()).CopyToDataTable();
        //        foreach(DataRow dr in orderTable.Rows)
        //        {
        //            dr["KyHieu"] = dr["KyHieuA"];
        //        }
        //        FileInfo file = new FileInfo(path);
        //        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

        //        string fileName = "";
        //        string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
        //        using (ExcelPackage excelPackage = new ExcelPackage())
        //        {
        //            int Height = 100;
        //            int Width = 150;
        //            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(orderTable.Rows[0]["TenHang"] + "Total");
        //            ExcelRange range = worksheet.Cells;
        //            ExcelTitle(worksheet, range, orderTable.Rows[0]["TenHang"].ToString());
        //            KHDongThungLib.dtXuatEX(orderTable, worksheet, false);
        //            bool checkIndex = true;
        //            int index = 0;
        //            int indexCont = 1;
        //            var dtExport = dtPhieuXuatHang.AsEnumerable().Select(x => new
        //            {
        //                MaPKL_XH = x["MaPKL_XH"].ToString(),
        //                MaHang = x["MaHang"].ToString(),
        //                Cont = x["MaCont"].ToString(),
        //                Seal = x["Seal"].ToString()

        //            }).Distinct().ToList();
        //            foreach (var item in dtExport)
        //            {
        //                if (checkIndex) indexCont = 1;
        //                else indexCont++;

        //                DataTable dtPKLXuatHang = GetDataXuatHang(item.MaPKL_XH);
        //                KHDongThungLib.sumToTalPCS(dtPKLXuatHang);
        //                dtPKLXuatHang = dtPKLXuatHang.AsEnumerable().Where(x => x["Cont"].ToString() == item.Cont.ToString()).CopyToDataTable();
        //                worksheet = excelPackage.Workbook.Worksheets.Add(item.MaPKL_XH + "_" + item.Cont + "_" + indexCont);
        //                range = worksheet.Cells;

        //                string mahang = item.MaHang;

        //                ExcelTitle(worksheet, range, mahang);

        //                range = worksheet.Cells["D8"]; range.Value = "Cont :";
        //                range = worksheet.Cells["e8:f8"]; range.Merge = true; range.Value = item.Cont;
        //                range = worksheet.Cells["H8:I8"]; range.Merge = true; range.Value = "Seal :" + item.Seal;

        //                KHDongThungLib.dtXuatEX(dtPKLXuatHang, worksheet, false);


        //                string renMaPKL = (index + 1 < dtPhieuXuatHang.Rows.Count) ? dtPhieuXuatHang.Rows[index + 1]["MaPKL_XH"].ToString() : "aaaabbbbb";
        //                if (renMaPKL == item.MaPKL_XH) checkIndex = false;
        //                else checkIndex = true;
        //                index++;
        //            }
        //            //foreach (DataRow item in dtPhieuXuatHang.Rows)
        //            //{
        //            //    if (checkIndex) indexCont = 1;
        //            //    else indexCont++;

        //            //    DataTable dtPKLXuatHang = GetDataXuatHang(item["MaPKL_XH"].ToString());
        //            //    KHDongThungLib.sumToTalPCS(dtPKLXuatHang);
        //            //    dtPKLXuatHang = dtPKLXuatHang.AsEnumerable().Where(x => x["Cont"].ToString() == item["Cont"].ToString()).CopyToDataTable();
        //            //    worksheet = excelPackage.Workbook.Worksheets.Add(item["MaPKL_XH"].ToString() + "_" + item["Cont"].ToString() + "_" + indexCont);
        //            //    range = worksheet.Cells;

        //            //    string mahang = item["MaHang"].ToString();

        //            //    ExcelTitle(worksheet, range, mahang);

        //            //    range = worksheet.Cells["D8"]; range.Value = "Cont :";
        //            //    range = worksheet.Cells["e8:f8"]; range.Merge = true; range.Value = item["Cont"].ToString();
        //            //    range = worksheet.Cells["H8:I8"]; range.Merge = true; range.Value = "Seal :" + item["Seal"].ToString();

        //            //    KHDongThungLib.dtXuatEX(dtPKLXuatHang, worksheet, false);


        //            //    string renMaPKL = (index + 1 < dtPhieuXuatHang.Rows.Count) ? dtPhieuXuatHang.Rows[index + 1]["MaPKL_XH"].ToString() : "aaaabbbbb";
        //            //    if (renMaPKL == item["MaPKL_XH"].ToString()) checkIndex = false;
        //            //    else checkIndex = true;
        //            //    index++;
        //            //}
        //            excelPackage.SaveAs(file);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
        //        throw;
        //    }
        //}


        private string getImgPath(string Img)
        {
            string Paths = Directory.GetCurrentDirectory();
            return $"{Paths}\\Resources\\{Img}";
        }

        private void grvPKLXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }

        private void grvPKLXuatHang_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";

            Sfd.FileName = string.Format("BaoCaoKHXuatHang_{0}_" + DateTime.Now.ToString("ddMMyyyyHHmmss"), _maPhieuXH);
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = "TemplateBaoCaoKHXuatHang.xlsx";
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = "Sheet1";
                    string RootPath = Application.StartupPath;
                    string path = Path.Combine(RootPath, @"Templates\", fileName);
                    ExportExcel_KHXH(Sfd.FileName, path);
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
        private void ExportExcel_KHXH(string fileName, string pathTemplate)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ShowWarning("File name  không hợp lệ.");
                return;
            }

            if (!File.Exists(pathTemplate))
            {

                return;
            }
            bool IsFilterMaHang = cbxLoc.Text.ToLower() == "mã hàng";
            bool IsFilterPhieu = cbxLoc.Text.ToLower() == "phiếu";


            string tuNgay = ((DateTime)dateTuNgay.EditValue).ToString("yyyy-MM-dd");
            string denNgay = ((DateTime)dateDenNgay.EditValue).ToString("yyyy-MM-dd");

            string Para1 = IsFilterMaHang && !IsFilterPhieu ? DateTime.MinValue.ToString("yyyy-MM-dd") : tuNgay;
            string Para2 = IsFilterMaHang && !IsFilterPhieu ? DateTime.MinValue.ToString("yyyy-MM-dd") : denNgay;
            if (IsFilterPhieu)
            {
                Para2 = DateTime.MaxValue.ToString("yyyy-MM-dd");
                Para1 = DateTime.MaxValue.ToString("yyyy-MM-dd");
            }

            string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetBCao_KHXH&Para1={Para1}&Para2={Para2}&Para3={(IsFilterMaHang && !IsFilterPhieu ? _maDH : searchLookUpEdit2?.EditValue?.ToString())}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblBCao_KHXH = JsonConvert.DeserializeObject<DataTable>(json);
            int rowStart = 8; int rowIdx = 8;
            if (tblBCao_KHXH?.Rows?.Count == 0)
            {
                ShowWarning("Chưa Có Dữ Liệu Để Xuất");
                return;

            }


            try
            {
                FileInfo file = new FileInfo(pathTemplate);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Sheet1"];

                    foreach (DataRow row in tblBCao_KHXH.Rows)
                    {
                        int SoKien, SoChiec;
                        float SoKg, SoKhoi;

                        worksheet.Cells[rowIdx, 8].Value = row["MaPKL_XH"]?.ToString();
                        worksheet.Cells[rowIdx, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        worksheet.Cells[rowIdx, 1].Value = row["MaHang"]?.ToString();
                        worksheet.Cells[rowIdx, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;



                        worksheet.Cells[rowIdx, 2].Value = row["PO"]?.ToString();
                        worksheet.Cells[rowIdx, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowIdx, 3].Value = !Int32.TryParse(row["Sokien"]?.ToString(), out SoKien) || string.IsNullOrEmpty(row["Sokien"]?.ToString()) ? 0 : SoKien;
                        worksheet.Cells[rowIdx, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Cells[rowIdx, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowIdx, 4].Value = !Int32.TryParse(row["SoChiec"]?.ToString(), out SoChiec) || string.IsNullOrEmpty(row["SoChiec"]?.ToString()) ? 0 : SoChiec;
                        worksheet.Cells[rowIdx, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Cells[rowIdx, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        worksheet.Cells[rowIdx, 5].Value = !float.TryParse(row["SoKG"]?.ToString(), out SoKg) || string.IsNullOrEmpty(row["SoKG"]?.ToString()) ? 0 : Math.Round(SoKg, 4);
                        worksheet.Cells[rowIdx, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Cells[rowIdx, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowIdx, 6].Value = !float.TryParse(row["SoKhoi"]?.ToString(), out SoKhoi) || string.IsNullOrEmpty(row["SoKhoi"]?.ToString()) ? 0 : Math.Round(SoKhoi, 4);
                        worksheet.Cells[rowIdx, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Cells[rowIdx, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowIdx, 9].Value = row["Cont"]?.ToString();
                        worksheet.Cells[rowIdx, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 9].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowIdx, 10].Value = row["Seal"]?.ToString();
                        worksheet.Cells[rowIdx, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 10].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        worksheet.Cells[rowIdx, 7].Value = row["DVSX"]?.ToString();
                        worksheet.Cells[rowIdx, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        rowIdx++;
                    }


                    worksheet.Cells[rowIdx + 1, 3].Formula = $"=sum(C{rowStart}:C{rowIdx - 1})";
                    worksheet.Cells[rowIdx + 1, 4].Formula = $"=sum(D{rowStart}:D{rowIdx - 1})";
                    worksheet.Cells[rowIdx + 1, 5].Formula = $"=sum(E{rowStart}:E{rowIdx - 1})";
                    worksheet.Cells[rowIdx + 1, 6].Formula = $"=sum(F{rowStart}:F{rowIdx - 1})";

                    var borderRange = worksheet.Cells[rowStart, 1, rowIdx + 1, 10];
                    borderRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                    var footerRange = worksheet.Cells[$"A{rowIdx + 1}:B{rowIdx + 1}"];
                    footerRange.Value = "Tổng";
                    footerRange.Merge = true;
                    footerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    footerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    footerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    footerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
                    footerRange.Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                    footerRange.Style.Font.Bold = true;


                    var footerSumRange = worksheet.Cells[$"C{rowIdx + 1}:J{rowIdx + 1}"];
                    footerSumRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    footerSumRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    footerSumRange.Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                    footerSumRange.Style.Font.Bold = true;
                    footerSumRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    footerSumRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    FileInfo newFile = new FileInfo(fileName);
                    excelPackage.SaveAs(newFile);


                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void ExportExcel_KHXH(string fileName, string pathTemplate)
        //{
        //    if (string.IsNullOrEmpty(fileName))
        //    {
        //        ShowWarning("File name  không hợp lệ.");
        //        return;
        //    }

        //    if (!File.Exists(pathTemplate))
        //    {

        //        return;
        //    }
        //    bool IsFilterMaHang = cbxLoc.Text.ToLower() == "mã hàng";
        //    bool IsFilterPhieu = cbxLoc.Text.ToLower() == "phiếu";


        //    string tuNgay = ((DateTime)dateTuNgay.EditValue).ToString("yyyy-MM-dd");
        //    string denNgay = ((DateTime)dateDenNgay.EditValue).ToString("yyyy-MM-dd");

        //    string Para1 = IsFilterMaHang && !IsFilterPhieu ? DateTime.MinValue.ToString("yyyy-MM-dd") : tuNgay;
        //    string Para2 = IsFilterMaHang && !IsFilterPhieu ? DateTime.MinValue.ToString("yyyy-MM-dd") : denNgay;
        //    if (IsFilterPhieu)
        //    {
        //        Para2 = DateTime.MaxValue.ToString("yyyy-MM-dd");
        //        Para1 = DateTime.MaxValue.ToString("yyyy-MM-dd");
        //    }

        //    string url = string.Format("{0}", URL + $"XuatHang/Get?Action=GetBCao_KHXH&Para1={Para1}&Para2={Para2}&Para3={(IsFilterMaHang && !IsFilterPhieu ? _maDH : searchLookUpEdit2?.EditValue?.ToString())}");
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    DataTable tblBCao_KHXH = JsonConvert.DeserializeObject<DataTable>(json);
        //    int rowStart = 8; int rowIdx = 8;
        //    if (tblBCao_KHXH?.Rows?.Count == 0)
        //    {
        //        ShowWarning("Chưa Có Dữ Liệu Để Xuất");
        //        return;

        //    }


        //    try
        //    {
        //        FileInfo file = new FileInfo(pathTemplate);
        //        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

        //        using (ExcelPackage excelPackage = new ExcelPackage(file))
        //        {
        //            var worksheet = excelPackage.Workbook.Worksheets["Sheet1"];

        //            foreach (DataRow row in tblBCao_KHXH.Rows)
        //            {
        //                int SoKien, SoChiec;
        //                float SoKg, SoKhoi;
        //                worksheet.Cells[rowIdx, 1].Value = row["MaHang"]?.ToString();
        //                worksheet.Cells[rowIdx, 2].Value = row["PO"]?.ToString();
        //                worksheet.Cells[rowIdx, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
        //                worksheet.Cells[rowIdx, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //                worksheet.Cells[rowIdx, 3].Value = !Int32.TryParse(row["Sokien"]?.ToString(), out SoKien) || string.IsNullOrEmpty(row["Sokien"]?.ToString()) ? 0 : SoKien;
        //                worksheet.Cells[rowIdx, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                worksheet.Cells[rowIdx, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //                worksheet.Cells[rowIdx, 4].Value = !Int32.TryParse(row["SoChiec"]?.ToString(), out SoChiec) || string.IsNullOrEmpty(row["SoChiec"]?.ToString()) ? 0 : SoChiec;
        //                worksheet.Cells[rowIdx, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                worksheet.Cells[rowIdx, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


        //                worksheet.Cells[rowIdx, 5].Value = !float.TryParse(row["SoKG"]?.ToString(), out SoKg) || string.IsNullOrEmpty(row["SoKG"]?.ToString()) ? 0 : Math.Round(SoKg, 4);
        //                worksheet.Cells[rowIdx, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                worksheet.Cells[rowIdx, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //                worksheet.Cells[rowIdx, 6].Value = !float.TryParse(row["SoKhoi"]?.ToString(), out SoKhoi) || string.IsNullOrEmpty(row["SoKhoi"]?.ToString()) ? 0 : Math.Round(SoKhoi, 4);
        //                worksheet.Cells[rowIdx, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                worksheet.Cells[rowIdx, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //                worksheet.Cells[rowIdx, 7].Value = row["DVSX"]?.ToString();
        //                rowIdx++;
        //            }


        //            worksheet.Cells[rowIdx + 1, 3].Formula = $"=sum(C{rowStart}:C{rowIdx - 1})";
        //            worksheet.Cells[rowIdx + 1, 4].Formula = $"=sum(D{rowStart}:D{rowIdx - 1})";
        //            worksheet.Cells[rowIdx + 1, 5].Formula = $"=sum(E{rowStart}:E{rowIdx - 1})";
        //            worksheet.Cells[rowIdx + 1, 6].Formula = $"=sum(F{rowStart}:F{rowIdx - 1})";

        //            var borderRange = worksheet.Cells[rowStart, 1, rowIdx + 1, 7];
        //            borderRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //            borderRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //            borderRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //            borderRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


        //            var footerRange = worksheet.Cells[$"A{rowIdx + 1}:B{rowIdx + 1}"];
        //            footerRange.Value = "Tổng";
        //            footerRange.Merge = true;
        //            footerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            footerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //            footerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //            footerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);
        //            footerRange.Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
        //            footerRange.Style.Font.Bold = true;


        //            var footerSumRange = worksheet.Cells[$"C{rowIdx + 1}:G{rowIdx + 1}"];
        //            footerSumRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //            footerSumRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
        //            footerSumRange.Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
        //            footerSumRange.Style.Font.Bold = true;
        //            footerSumRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //            footerSumRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //            FileInfo newFile = new FileInfo(fileName);
        //            excelPackage.SaveAs(newFile);


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        void ShowWarning(string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            args.Caption = Resources.Warning;
            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Text = $"{message}";
            args.Buttons = new DialogResult[] { DialogResult.OK };
            args.Icon = SystemIcons.Warning;
            args.MessageBeepSound = MessageBeepSound.Warning;
            XtraMessageBox.Show(args);
        }

        #endregion
    }
    public class QuantityInfo
    {
        public int SLThung { get; set; }
        public int SLSP { get; set; }
    }
}
