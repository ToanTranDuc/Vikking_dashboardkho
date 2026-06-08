using DevExpress.Data;
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
    public partial class frmPackageListChuyenKhoView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        string _maPhieuXH = string.Empty, _tenHang = string.Empty, _status = string.Empty;

        string _StatusFiter = string.Empty; string _MaDH = string.Empty, _maKH = string.Empty;
        string _toDate = string.Empty; string _fromDate = string.Empty;

        bool flagAdd = false, isShowMaDH = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        DataTable dtDonHang = new DataTable();
        KeyDownControlHandler keyDownControlHandler;

        public frmPackageListChuyenKhoView()
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
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IntFilter();
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
        private void LoadPhieuXuatHang(bool filterMH = false)
        {
            int focusRow = grvPhieuXH.FocusedRowHandle;
            _fromDate = Convert.ToDateTime(fromDateEdit.EditValue).ToString("yyyy-MM-dd");
            _toDate = Convert.ToDateTime(toDateEdit.EditValue).ToString("yyyy-MM-dd");
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetPhieuXH&Para1={(!filterMH ? _fromDate : "1990-01-01")}&Para2={(!filterMH ? _toDate : "1990-01-01")}&Para3={(filterMH ? _MaDH : "Para")}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPhieuXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPhieuXuatHang.Rows.Count == 0)
            {
                dgrPKLXuatHang.DataSource = null;
                grcDSPhieuXH.DataSource = null;
                return;
            }
            grcDSPhieuXH.DataSource = dtPhieuXuatHang;
            if (!flagAdd)
            {
                if (focusRow == 0)
                {
                    var dr = grvPhieuXH.GetFocusedDataRow();
                    _maPhieuXH = dr["MaPKL_XH"].ToString();
                    LoadPKLXuatHang();
                    return;
                }
                grvPhieuXH.FocusedRowHandle = focusRow;
                if (focusRow < 0)
                    LoadPKLXuatHang();
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
            //string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetCTPhieuXH&Para1={_maPhieuXH}&Para2=Para");
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //var dtPKLXuatHang = JsonConvert.DeserializeObject<DataTable>(json);
            var dtPKLXuatHang = GetDataXuatHang(_maPhieuXH);
            CreateBandSize(dtPKLXuatHang);
            KHDongThungLib.ProcessSttTrung1(dtPKLXuatHang);
            dtPKLXuatHang = KHDongThungLib.CalculatorTB(dtPKLXuatHang);
            dgrPKLXuatHang.DataSource = dtPKLXuatHang;

            DataTable processedData = KHDongThungLib.sumToTalPCS(dtPKLXuatHang);
            KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
            dgcTong.DataSource = processedData;
            KHDongThungLib.AllowVieworNotPack(dtPKLXuatHang, BandPCB, BandPack, BandStore);

        }
        private DataTable GetDataXuatHang(string maPhieu)
        {
            DataTable dtData = new DataTable();
            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetCTPhieuXH&Para1={maPhieu}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtPKLXuatHang1 = JsonConvert.DeserializeObject<DataTable>(json);
            url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetCTPhieuXHTon&Para1={maPhieu}&Para2=Para");
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
            _maPhieuXH = dr["MaPKL_XH"].ToString();
            _status = dr["Status"].ToString();
            _maKH = dr["MaKH"].ToString();
            LoadPKLXuatHang();
        }
        private void grvPKLXuatHang_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains('@'))
                if (e.Value is null || e.Value.ToString() == "" || e.Value.ToString() == "0") e.DisplayText = "-";
        }
        private void grvPKLXuatHang_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            return;
            if (e.Column.FieldName == "SLThung")
            {
                // Perform your condition to determine if cells should be merged
                if (e.RowHandle > 0)
                {
                    var preSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
                    var curSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
                    object prevValue = grvPKLXuatHang.GetRowCellValue(e.RowHandle - 1, e.Column);
                    object currValue = grvPKLXuatHang.GetRowCellValue(e.RowHandle, e.Column);

                    if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            {
                var preSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle - 1, "SttThung");
                var curSTTThung = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "SttThung");
                if (preSTTThung != null && curSTTThung != null && preSTTThung.Equals(curSTTThung))
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = false;
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
        private void grvPKLXuatHang_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var Check = grvPKLXuatHang.GetRowCellValue(e.RowHandle, "IsTon");
            if (Check.ToString() != "0")
                e.Appearance.BackColor = Color.FromArgb(255, 255, 153);
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
            frmPackageListChuyenKho fr = new frmPackageListChuyenKho();
            fr.ShowDialog();
            flagAdd = true;
            //_MaDH = frmPackageListChuyenKho._donhang;
            //if (_MaDH == "") return;
            ChangeFilter();
        }
        private void BtnRefresh()
        {
            flagAdd = false;
            LoadPhieuXuatHang();
        }
        private void BtnEditPKL()
        {
            isShowMaDH = false;
            frmPackageListChuyenKho fr = new frmPackageListChuyenKho(_maPhieuXH, _maKH);
            fr.ShowDialog();
            flagAdd = false;
            //_MaDH = frmPackageListChuyenKho._donhang;
            //if (_MaDH == "") return;
            ChangeFilter();
        }
        private void BtnDelete()
        {
            if (_status != "0")
            {
                MessageBox.Show("Phiếu đã được kiểm xuất kho. Không thể xóa?", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult resultDialog = MessageBox.Show("Xác nhận xóa package list?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDialog == DialogResult.Yes)
            {
                string url = string.Format("{0}", URL + $"ChuyenKho/Delete?action=DeletePKLXH&Para={_maPhieuXH}");
                DataSet ds = new DataSet();
                ds.Tables.Add(new DataTable());
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, ds); }).Result;
                if (result.ToLower() == "true")
                {
                    flagAdd = false;
                    LoadPhieuXuatHang(StatusSelected == "0" ? false : true);
                }
            }
        }
        private void BtnEX()
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (dtDonHang is null || dtDonHang.Rows.Count == 0) return;
            _tenHang = dtDonHang.AsEnumerable().Where(x => x["MaDH"].ToString() == searchLookUp_MaHang.EditValue.ToString()).FirstOrDefault()["MaHang"].ToString();
            Sfd.FileName = string.Format("PhieuChuyenKho_{0}_" + DateTime.Now.ToString("ddMMyyyy"), _tenHang);
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

        private void btnEX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnEX();
        }
        private void ExportExcel(string path)
        {
            try
            {
                DataTable dtPhieuXuatHang = grcDSPhieuXH.DataSource as DataTable;
                dtPhieuXuatHang = dtPhieuXuatHang.AsEnumerable().Where(x => x["MaPKL_XH"].ToString() == _maPhieuXH.ToString()).CopyToDataTable();
                if (dtPhieuXuatHang.Rows.Count == 0) return;
                var dtXuatHang = dgrPKLXuatHang.DataSource as DataTable;
                string PathLoGo = KHDongThungLib.getImgPath("texgiang.jpg");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    var dtExport = dtPhieuXuatHang.AsEnumerable().Select(x => new
                    {
                        MaPKL_XH = x["MaPKL_XH"].ToString(),
                        MaCont = x["MaCont"].ToString(),
                        Cont = x["Cont"].ToString(),
                        Seal = x["Seal"].ToString(),
                        TuKho = x["TuKho"].ToString(),
                        DenKho = x["DenKho"].ToString()

                    }).Distinct().ToList();
                    foreach (var itemCont in dtExport)
                    {
                        var dttempXH = dtXuatHang.AsEnumerable().Where(x => x["Cont"].ToString() == itemCont.MaCont.ToString()).CopyToDataTable();
                        List<string> MahangGop = dttempXH.AsEnumerable().Select(x => x["MaHang"].ToString()).Distinct().ToList();
                        string result = string.Join(" + ", MahangGop);
                        var item = dttempXH.Rows[0];
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(itemCont.MaPKL_XH.ToString() + "_" + itemCont.Cont.ToString());
                        ExcelRange range = worksheet.Cells;
                        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        worksheet.Cells.Style.Font.Size = 13;
                        range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PACKING LIST"; range.Style.Font.Bold = true;

                        //string url1 = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GetCTPhieuXH&Para1={item["MaPKL_XH"].ToString()}&Para2=Para");
                        //string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                        //DataTable dtPKLXuatHang = JsonConvert.DeserializeObject<DataTable>(json1);

                        //range = worksheet.Cells["A1:B3"]; range.Merge = true;
                        //Image image = Image.FromFile(PathLoGo);
                        //OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                        //picture.SetPosition(0, 0, 0, 0);
                        //picture.SetSize(105, 55);

                        worksheet.Cells.Style.Font.Size = 11;
                        range = worksheet.Cells["D4"]; range.Value = "STYLE :";
                        range = worksheet.Cells["e4:f4"]; range.Merge = true; range.Value = result;
                        range = worksheet.Cells["D5"]; range.Value = "Từ kho :";
                        range = worksheet.Cells["e5:f5"]; range.Merge = true; range.Value = itemCont.TuKho.ToString();
                        range = worksheet.Cells["D6"]; range.Value = "Đến kho :";
                        range = worksheet.Cells["e6:f6"]; range.Merge = true; range.Value = itemCont.DenKho.ToString();
                        range = worksheet.Cells["D7"]; range.Value = "Cont :";
                        range = worksheet.Cells["e7:f7"]; range.Merge = true; range.Value = itemCont.Cont.ToString();
                        range = worksheet.Cells["D8"]; range.Value = "Seal :";
                        range = worksheet.Cells["e8:f8"]; range.Merge = true; range.Value = itemCont.Seal.ToString();

                        KHDongThungLib.dtXuatEX(dttempXH, worksheet, false);
                        //foreach (DataRow item in dttempXH.Rows)
                        //{


                        //}
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
        private void grvPKLXuatHang_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e, true);
        }
        private void grvPKLXuatHang_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrPKLXuatHang.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e, true);
        }
        #endregion

        #region Luân
        private void IntFilter()
        {
            CreateFilter();
            IntiDateEdit();

        }

        private void CreateFilter()
        {
            DataTable dtFilter = new DataTable();
            DataColumn colMaHang = new DataColumn("Option", typeof(string));
            dtFilter.Columns.Add(colMaHang);
            DataColumn colStatus = new DataColumn("Status", typeof(string));
            dtFilter.Columns.Add(colStatus);



            DataRow newRow2 = dtFilter.NewRow();
            newRow2["Option"] = "Thời Gian";
            newRow2["Status"] = "0";
            dtFilter.Rows.Add(newRow2);

            DataRow newRow1 = dtFilter.NewRow();
            newRow1["Option"] = "Mã Hàng";
            newRow1["Status"] = "1";
            dtFilter.Rows.Add(newRow1);

            //int Cbx Filter

            cbxFilter.DataSource = dtFilter;
            cbxFilter.DisplayMember = "Option";
            cbxFilter.ValueMember = "Status";

        }

        private void IntiDateEdit()
        {
            toDateEdit.EditValue = DateTime.Now;
            toDateEdit.EditValueChanged += ToDateEdit_EditValueChanged;
            fromDateEdit.EditValueChanged += FromDateEdit_EditValueChanged;
            fromDateEdit.EditValue = DateTime.Now;
        }

        private void FromDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.Controls.ChangingEventArgs changingEventArgs = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
            if (changingEventArgs != null)
            {
                DateTime newValue = (DateTime)changingEventArgs.NewValue;

                LoadPhieuXuatHang();
            }
        }



        private void ToDateEdit_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.Controls.ChangingEventArgs changingEventArgs = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
            if (changingEventArgs != null)
            {
                DateTime newValue = (DateTime)changingEventArgs.NewValue;
                _toDate = newValue.ToString("yyyy-MM-dd");
                LoadPhieuXuatHang();
                //Init_searchLookUp_MaHang();
            }
        }

        private void Init_searchLookUp_MaHang()
        {
            grcDSPhieuXH.DataSource = new DataTable();
            dgrPKLXuatHang.DataSource = new DataTable();

            string url = string.Format("{0}", URL + $"ChuyenKho/Get?Action=GETMaHang&Para1=1&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUp_MaHang.Properties.DataSource = dtDonHang;
            searchLookUp_MaHang.Properties.DisplayMember = dtDonHang.Rows.Count == 0 ? "---Chưa có dữ liệu---" : "TenHangDisplay";
            searchLookUp_MaHang.Properties.ValueMember = "MaDH";
            searchLookUp_MaHang.Properties.NullText = dtDonHang.Rows.Count == 0 ? "---Chưa có dữ liệu---" : "[Chọn Mã Hàng ]";

            if (dtDonHang != null && dtDonHang.Rows.Count > 0)
            {
                if (isShowMaDH || _MaDH == "")
                {
                    searchLookUp_MaHang.EditValue = dtDonHang.Rows[0]["MaDH"];
                    _MaDH = dtDonHang.Rows[0]["MaDH"].ToString();
                    LoadPhieuXuatHang(true);
                }
                else
                {
                    searchLookUp_MaHang.EditValue = _MaDH;
                    LoadPhieuXuatHang(true);
                }
            }
            else return;
        }



        ///envent 
        ///

        private void cbxFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            ChangeFilter();
        }
        string StatusSelected = "0";
        private void ChangeFilter()
        {
            if (cbxFilter.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)cbxFilter.SelectedItem;
                StatusSelected = selectedRow["Status"].ToString();
                if (StatusSelected == "1")
                {
                    layoutToDate.Visibility = LayoutVisibility.Never;
                    layoutFromDate.Visibility = LayoutVisibility.Never;
                    lblMaHang.Visibility = LayoutVisibility.Always;
                    Init_searchLookUp_MaHang();
                }
                else
                {
                    layoutToDate.Visibility = LayoutVisibility.Always;
                    layoutFromDate.Visibility = LayoutVisibility.Always;
                    lblMaHang.Visibility = LayoutVisibility.Never;
                    LoadPhieuXuatHang();
                }
                //if (!_StatusFiter.Equals(StatusSelected))
                //{
                //    _StatusFiter = StatusSelected;

                //    if (_StatusFiter == "1")
                //    {
                //        layoutToDate.Visibility = LayoutVisibility.Never;
                //        layoutFromDate.Visibility = LayoutVisibility.Never;
                //        lblMaHang.Visibility = LayoutVisibility.Always;
                //        Init_searchLookUp_MaHang();
                //    }
                //    else
                //    {
                //        layoutToDate.Visibility = LayoutVisibility.Always;
                //        layoutFromDate.Visibility = LayoutVisibility.Always;
                //        lblMaHang.Visibility = LayoutVisibility.Never;
                //        LoadPhieuXuatHang();
                //    }

                //}
            }
        }


        private void searchLookUp_MaHang_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUp_MaHang.EditValue != null)
            {
                _MaDH = searchLookUp_MaHang.EditValue.ToString();
                LoadPhieuXuatHang(true);
            }

        }


        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = grvPhieuXH.GetFocusedDataRow();
            if (dr == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu ");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("PhieuChuyenKho{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "PHIEUCHUYENKHO.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, dr);

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
        public void Export(string TemplateFileName, string ExportFileName, DataRow drRow)
        {
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        excelPackage.Workbook.Properties.Title = "";

                        string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                        ExcelWorksheet ws = templatePackage.Workbook.Worksheets["Sheet1"];

                        ws.Cells["c17"].Value = drRow["TuKho"].ToString();
                        ws.Cells["c18"].Value = drRow["DenKho"].ToString();

                        int countRow = 22;
                        ws.InsertRow(22, 2);
                        ws.Cells["A22"].Value = 1;
                        ws.Cells["B22"].Value = drRow["TenHang"].ToString();
                        ws.Cells["d22"].Value = "Ch";
                        ws.Cells["e22"].Value = drRow["SoLuong"].ToString();

                        ws.Cells["B23"].Value = "Tổng";
                        ws.Cells["e23"].Value = $"{drRow["SoLuong"].ToString()}/{drRow["Carton"].ToString()}";


                        ws.Cells["D12"].Value = dateTime[0];
                        ws.Cells["f12"].Value = dateTime[1];
                        ws.Cells["h12"].Value = dateTime[2];


                        ws.Cells["h13"].Value = dateTime[0];
                        ws.Cells["j13"].Value = dateTime[1];
                        ws.Cells["l13"].Value = dateTime[2];

                        var borderData = ws.Cells[22, 1, 23, 8].Style.Border;
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
            }
            #endregion
        }

    }
}
