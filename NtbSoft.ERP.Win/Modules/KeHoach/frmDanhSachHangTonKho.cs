using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    public partial class frmDanhSachHangTonKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<DonHangTonKhoDSEntity> ListDSTonKho;
        string maHang, maMau;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int sum = 0;
        List<string> lstSize;
        string json = string.Empty;
        KeyDownControlHandler keyDownControlHandler;
        bool isRowChanged;
        int pageIndex;
        int pageSize;
        DataTable tblKHDTTonKho;

        public frmDanhSachHangTonKho()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            ListDSTonKho = new List<DonHangTonKhoDSEntity>();
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                pageIndex = 1;
                pageSize = 150;
                barEditItemSpinPageSize.EditValue = pageSize;
                lstSize = new List<string>();
                maHang = string.Empty;
                maMau = string.Empty;
                LoadDS();
                CreateSearchLookupHT();
                CreateSearchLookup();
                CheckPerminsion();
                keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtThem, true, ActionType.Add);
            AddActionControl(_lstActionControl, BtSua, true, ActionType.Edit);
            AddActionControl(_lstActionControl, BtXoa, true, ActionType.Delete);
            AddActionControl(_lstActionControl, BtNapLai, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void CheckPerminsion()
        {
            SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;

            this.Them.Enabled = _allowAdd;
            this.Sua.Enabled = _allowEdit;
            this.Xoa.Enabled = _allowDelete;
        }

        private void CreateSearchLookup()
        {
            // Hàng hóa
            string urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            this.repositoryMaHangSearchLookUpEdit.DataSource = tblHH;

            // Màu
            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
            this.repositoryMauSearchLookUpEdit.DataSource = tblMau;
        }
        private void LoadDS()
        {
            isRowChanged = false;
            string url = string.Format("{0}?pageIndex={1}&&pageSize={2}", URL + "DonHangTonKho/GetTonKho",pageIndex,pageSize);
            json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            gridControlDSTonKho.DataSource = tbl;
            if (tbl != null && tbl.Rows.Count > 0)
            {
                if (!isRowChanged)
                {
                    GetChiTietTonKho();
                }
            }
            else
            {
                gridControl1.DataSource = null;
            }

            // GetKHDTTonKho
            string urlKHDTTonKho = string.Format("{0}", URL + "DonHangTonKho/GetKHDTTonKho");
            string jsonKHDTTonKHo = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKHDTTonKho); }).Result;
            tblKHDTTonKho = JsonConvert.DeserializeObject<DataTable>(jsonKHDTTonKHo);
        }

        private void repositorySearchLookUpEditMaHang_EditValuedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Edit_valued_changed");

            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            if (changingEventArgs.NewValue != null)
            {
                maHang = changingEventArgs.NewValue.ToString();
            }
            else
            {
                maHang = string.Empty;
            }
            FilterData(maHang);
        }

        private void FilterData(string _maHangFilter)
        {
            try
            {
                isRowChanged = false;
                DataTable tblFilter = JsonConvert.DeserializeObject<DataTable>(json);

                if (!string.IsNullOrEmpty(_maHangFilter))
                {
                    DataRow _row = tblFilter.AsEnumerable().FirstOrDefault(x => (x["MaHang"].ToString() == _maHangFilter.ToString()));
                    if (_row != null)
                    {
                        tblFilter = tblFilter.AsEnumerable().Where(x => (x["MaHang"].ToString() == _maHangFilter.ToString())).CopyToDataTable();
                    }
                    else
                    {
                        tblFilter = null;
                        gridControl1.DataSource = null;
                    }
                }
                gridControlDSTonKho.DataSource = tblFilter;
                if (!isRowChanged&& tblFilter!=null)
                {
                    GetChiTietTonKho();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void repositorySearchLookUpEditMau_EditValuedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Edit_valued_changed");

            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            if (changingEventArgs.NewValue != null)
            {
                maMau = changingEventArgs.NewValue.ToString();
            }
            else
            {
                maMau = string.Empty;
            }

            LoadDS();
        }

        private void BtSua()
        {
            if (gridControlDSTonKho.DataSource == null || gridControl1.DataSource == null) 
            {
                return;
            }
            DataRow dataRow = (gridViewThongTinTonKho.GetFocusedRow() as DataRowView).Row;

            DataTable tbl = gridControlDSTonKho.DataSource as DataTable;
            DataTable tbl1 = gridControl1.DataSource as DataTable;
            // Danh sách lọc theo mã hàng, mã màu, POID, DauSizeID
            //DataTable tblFilter = 
            //DataTable tblFilter =
            //  tbl.AsEnumerable().Where(x =>
            // x["MaHang"].ToString() == dataRow["MaHang"].ToString() &&
            // x["MaMau"].ToString() == dataRow["MaMau"].ToString() &&
            // x["POID"].ToString() == dataRow["POID"].ToString() &&
            // x["DauSizeID"].ToString() == tbl.Rows[0]["DauSizeID"].ToString()
            //  ).CopyToDataTable();

            DataTable tblFilter =
              tbl.AsEnumerable().Where(x =>
             (x["MaDH"].ToString() == dataRow["MaDH"].ToString()) && (x["MaHang"].ToString() == dataRow["MaHang"].ToString())
              ).CopyToDataTable();

            //DataTable tblUnpivot = UnPivot(tbl1,tblFilter.Rows[0]["TenHang"].ToString(),tblFilter.Rows[0]["MaHang"].ToString());
            frmNhapTonKho frm = new frmNhapTonKho(true, tblFilter, tbl1, repositoryMaHangSearchLookUpEdit.DataSource as DataTable, repositoryMauSearchLookUpEdit.DataSource as DataTable);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            maHang = maMau = string.Empty;
            LoadDS();
        }

        private void GetDsBangMauAsynchronous()
        {
            // Màu
            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
            DataTable _tblMauAsynchronous;
            Task<string> a = Task.Run<string>(() =>
            {
                Console.WriteLine("thực thi gọi api bdb");
                _tblMauAsynchronous = JsonConvert.DeserializeObject<DataTable>(_clientExtension.GetAsnyc(urlMau).Result);
                this.repositoryMauSearchLookUpEdit.DataSource = _tblMauAsynchronous;
                return "complete";
            });
            Console.WriteLine("a: " + a.ToString());

        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtSua();
        }
        private DataTable CreateTableChiTietTonKho()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaHS", typeof(string));
            tbl.Columns.Add("SoVoice", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            
            tbl.Columns.Add("TenHang", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));

            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(string));
            tbl.Columns.Add("TrangThai", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        private DataTable UnPivot(DataTable tblThongTinHangTonKho, string tenHang, string maHang)
        {
            DataTable tblSave = CreateTableChiTietTonKho();
            if (tblThongTinHangTonKho == null || (tblThongTinHangTonKho != null && tblThongTinHangTonKho.Rows.Count == 0)) return null;
            DataRow tbl_kho = tblThongTinHangTonKho.Rows[0];
            foreach (DataRow dataRows in tblThongTinHangTonKho.Rows)
            {
                for (int i = 7; i < dataRows.ItemArray.Length; i++)
                {
                    Console.WriteLine("dataRows");
                    //tblSave[""]
                    DataRow dataRowSave = tblSave.NewRow();
                    dataRowSave["TenHang"] = tenHang;
                    dataRowSave["MaHang"] = maHang;
                    dataRowSave["POID"] = dataRows["POID"];
                    dataRowSave["PO"] = dataRows["PO"];
                    dataRowSave["DauSizeID"] = dataRows["DauSizeID"];
                    dataRowSave["DauSize"] = dataRows["DauSize"];
                    dataRowSave["MaMau"] = dataRows["MaMau"];
                    dataRowSave["TenMau"] = dataRows["TenMau"];
                    dataRowSave["Size"] = lstSize[i - 7];
                    dataRowSave["SoLuong"] = dataRows.ItemArray[i];
                    //dataRowSave["GhiChu"] = dataRows["GhiChu"];
                    tblSave.Rows.Add(dataRowSave);
                }
            }
            Console.WriteLine("end for");
            return tblSave;
        }

        private void BtThem()
        {
            DataTable tbl = new DataTable();
            frmNhapTonKho frm = new frmNhapTonKho(false, tbl, tbl, repositoryMaHangSearchLookUpEdit.DataSource as DataTable, repositoryMauSearchLookUpEdit.DataSource as DataTable);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            maHang = maMau = string.Empty;
            LoadDS();
            GetDsBangMauAsynchronous();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtThem();
        }

        private void gridViewThongTinTonKho_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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

        private void BandedView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }


        private void gridViewThongTinTonKho_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            //GridView view = (GridView)sender;
            //if (e.ListSourceRowIndex < 0) return;
            //if (e.Column == colMaHang)
            //{
            //    int chilHandle = view.GetChildRowHandle(e.GroupRowHandle, 0);
            //    if (chilHandle < 0) chilHandle = 0;
            //    e.DisplayText = string.Format(" {0}", view.GetRowCellValue(chilHandle, colTenHang));
            //}
        }
        private void BtNapLai()
        {
            maHang = string.Empty;
            maMau = string.Empty;
            //repositoryMaHangSearchLookUpEdit.ValueMember = null;
            barEditItem1.EditValue = null;
            barEditItem2.EditValue = null;
            LoadDS();
        }
        private void NapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtNapLai();
        }

        private void BtXoa()
        {
            if (gridControlDSTonKho.DataSource == null || gridControl1.DataSource == null)
            {
                return;
            }
            DataRow dataRow = (gridViewThongTinTonKho.GetFocusedRow() as DataRowView).Row;

            // Check đã có trong KHDongThungTonKho thì không cho xóa
            DataRow rowFilter = tblKHDTTonKho.AsEnumerable().Where(
                x => x["MaDH"].Equals(dataRow["MaDH"])
                && x["MaHang"].Equals(dataRow["MaHang"])).FirstOrDefault();
            //
            if (rowFilter != null)
            {
                XtraMessageBox.Show("Đơn hàng này đã được lập kế hoạch đóng thùng. Vui lòng không xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //

            DialogResult result = MessageBox.Show("Bạn có muốn xóa dòng này không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //string url = string.Format("{0}/?madonhang={1}", URL + "DonHangTong/Delete", _maDonHang);
                //string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                if (string.IsNullOrEmpty(dataRow["MaDH"].ToString())) return;
                string url = string.Format("{0}?parameter={1}", URL + "DonHangTonKho/DeleteTonKho", dataRow["MaDH"]);
                string resultDelete = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (resultDelete.ToLower() == "true")
                {
                    Console.WriteLine("delete_true");
                    //LoadDS(searchLookUpEditMH.EditValue as string);
                }
                //LoadDSHangHoa();
                else XtraMessageBox.Show(resultDelete);

            }
            // maHang = maMau = string.Empty;
            // barEditItem1.EditValue = null;
            LoadDS();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtXoa();
        }

        private void gridViewThongTinTonKho_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewThongTinTonKho_RowCountChanged(object sender, EventArgs e)
        {
            //GridView gridview = ((GridView)sender);
            //if (!gridview.GridControl.IsHandleCreated) return;
            //Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            //SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            //gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void GetChiTietTonKho()
        {
            try
            {
                if (gridViewThongTinTonKho.FocusedRowHandle >= 0)
                {
                    DataRow focusedRow = gridViewThongTinTonKho.GetDataRow(gridViewThongTinTonKho.FocusedRowHandle);
                    if (focusedRow == null || string.IsNullOrEmpty(focusedRow["MaDH"].ToString()) || string.IsNullOrEmpty(focusedRow["MaDH"].ToString())) return;
                    string url = string.Format("{0}?madh={1}&&maHang={2}", URL + "DonHangTonKho/GetTonKhoChiTiet", focusedRow["MaDH"], focusedRow["MaHang"]);
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        gridControl1.MainView = GetBandGridViewAmount(tbl);
                        gridControl1.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridViewThongTinTonKho_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            isRowChanged = true;

            GridView view = (GridView)sender;
            object _objmapkl = null, _objmadh = null, _objpoid = null;
            int _objsttthung = 0;

            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null || string.IsNullOrEmpty(focusedRow["MaDH"].ToString()) || string.IsNullOrEmpty(focusedRow["MaDH"].ToString())) return;
            string url = string.Format("{0}?madh={1}&&maHang={2}", URL + "DonHangTonKho/GetTonKhoChiTiet", focusedRow["MaDH"], focusedRow["MaHang"]);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count >= 0)
            {
                gridControl1.MainView = GetBandGridViewAmount(tbl);
            }
            gridControl1.DataSource = tbl;
        }

        private void MainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "TenMau" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void MainView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "TenMau" && col.FieldName != "NgayGH" && col.FieldName != "NgayDuKienVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                int value = Convert.ToInt32(row[col.FieldName]);
                                sum += value;
                            }
                            isEmpty = false;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                if (!isEmpty)
                    e.Value = sum;
            }
        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 15; i++)
            {
                List<string> _ListString = new List<string>();
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmount(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                    
                }
                if (tab.Columns[i].ColumnName == "ColorCode")
                {
                    _ListString.Add("ColorCode");
                    SetGridBandedViewAmount(bandedView, "", "Color Code", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmount(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSize")
                {
                    _ListString.Add("DauSize");
                    SetGridBandedViewAmount(bandedView, "", "Nhóm size", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "TenQG")
                {
                    _ListString.Add("TenQG");
                    SetGridBandedViewAmount(bandedView, "", "Quốc Gia", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayGH")
                {
                    _ListString.Add("NgayGH");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Giao Hàng", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayDuKienVC")
                {
                    _ListString.Add("NgayDuKienVC");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Dự Kiến VC", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayThucTeVC")
                {
                    _ListString.Add("NgayThucTeVC");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Thực Tế VC", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayXuatHang")
                {
                    _ListString.Add("NgayXuatHang");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Xuất Hàng", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "GhiChu")
                {
                    _ListString.Add("GhiChu");
                    SetGridBandedViewAmount(bandedView, "", "Ghi chú", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 15;
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;

            }

            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("Amount");
            SetGridBandedViewAmount(bandedView, maHang, "", listHeader);

            colAmount.OptionsColumn.AllowEdit = false;
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.OwnerBand = gridBand;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Amount";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);
            foreach (BandedGridColumn col in bandedView.Columns)
            {

                if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "ColorCode" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG"
                    && col.FieldName != "NgayGH" && col.FieldName != "NgayDuKienVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang")
                {
                    GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                    itemSize.FieldName = col.FieldName;
                    itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    itemSize.DisplayFormat = "{0:n0}";
                    itemSize.ShowInGroupColumnFooter = col;
                    bandedView.GroupSummary.Add(itemSize);
                }

                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
            }

            bandedView.CustomDrawBandHeader += BandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += BandedView_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.CustomDrawRowIndicator += BandedView_CustomDrawRowIndicator;
            bandedView.RowCountChanged += BandedView_RowCountChanged;
            bandedView.CustomSummaryCalculate += BandedView_CustomSummaryCalculate;
            bandedView.CustomUnboundColumnData += BandedView_CustomUnboundColumnData1;
            bandedView.CustomDrawCell += BandedView_CustomDrawCell;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        private void BandedView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;

            if (view.IsRowSelected(e.RowHandle))
            {
                // Thiết lập màu sắc cho dòng được chọn
                e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            }
        }

        private void BandedView_CustomUnboundColumnData1(object sender, CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "ColorCode" && col.FieldName != "MaMau" && col.FieldName != "DauSize" 
                        && col.FieldName != "TenMau" && col.FieldName != "MaQG" && col.FieldName != "TenQG" && col.FieldName != "NgayGH" && col.FieldName != "NgayDuKienVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang" && col.FieldName != "GhiChu" 
                        && row.Table.Columns.Contains(col.FieldName) && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                int value = Convert.ToInt32(row[col.FieldName]);
                                sum += value;
                            }
                            isEmpty = false;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                if (!isEmpty)
                    e.Value = sum;
            }
        }

        private void BandedView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "ColorCode" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "TenMau" && col.FieldName != "TenQG" && col.FieldName != "NgayGH" && col.FieldName != "NgayDuKienVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void BandedView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void BandedView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        private void barButtonItemPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            //LoadDSDonHangTong();
            LoadDS();
        }

        private void barButtonItemNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.Caption = pageIndex.ToString();
            //LoadDSDonHangTong();
            LoadDS();
        }

        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                //LoadDSDonHangTong();
                LoadDS();
            }
        }

        private void BandedView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.DisplayText = "-";
            }
            else
            {
                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToInt32(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
        }

        private void BandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void SetGridBandedViewAmount(BandedGridView bandedView, string GridBandHeader, string GridBandCaption, List<string> columnNames)
        {
            if (lstSize != null && lstSize.Count > 0)
            {
                lstSize.Clear();
            }

            GridBand gridBand = new GridBand();
            if (GridBandHeader == "")
                gridBand.Caption = GridBandCaption;
            else
                gridBand.Caption = GridBandHeader;
            int nrOfColumns = columnNames.Count;
            gridBand.Fixed = FixedStyle.None;
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (nrOfColumns == 1 && (columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "ColorCode" || columnNames[0] == "MaMau" || columnNames[0] == "DauSize" || columnNames[0] == "TenMau" || columnNames[0] == "DauSizeID" || columnNames[0] == "MaQG" || columnNames[0] == "TenQG" || columnNames[0] == "NgayGH" || columnNames[0] == "NgayDuKienVC" || columnNames[0] == "NgayThucTeVC" || columnNames[0] == "NgayXuatHang" || columnNames[0] == "GhiChu"))
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);

                if (columnNames[0] == "PO")
                {
                    bandedColumns.OptionsColumn.AllowEdit = false;
                   bandedColumns.Visible = false;
                }

                String CaptionName = columnNames[0];

                if (CaptionName.ToString() != "PO")
                {
                    bandedColumns.OwnerBand = gridBand;
                    bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                    bandedColumns.Caption = GridBandCaption;
                    bandedColumns.Visible = true;
                    bandedColumns.Width = 75;
                    if (CaptionName.ToString() == "PO" || CaptionName.ToString() == "ColorCode" || CaptionName.ToString() == "MaMau" || CaptionName.ToString() == "DauSize" || CaptionName.ToString() == "TenMau"
                        || CaptionName.ToString() == "DauSizeID" || CaptionName.ToString() == "MaQG" || CaptionName.ToString() == "TenQG" || CaptionName.ToString() == "NgayGH")
                    {
                        gridBand.Fixed = FixedStyle.Left;
                    }
                    gridBand.RowCount = 1;
                    bandedView.Bands.Add(gridBand);
                }    
                   

            }
            else
            {
                BandedGridColumn[] bandedColumns = new BandedGridColumn[nrOfColumns];
                GridBand[] grHeader = new GridBand[nrOfColumns];
                for (int i = 0; i < nrOfColumns; i++)
                {
                    String[] _colName = columnNames[i].Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    GridBand gridband3 = new GridBand();
                    gridband3.Caption = _colName[0];
                    lstSize.Add(_colName[0]);
                    bandedColumns[i] = (BandedGridColumn)bandedView.Columns.AddField(columnNames[i]);
                    bandedColumns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    bandedColumns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedColumns[i].DisplayFormat.FormatString = "{0:##,0}";
                    bandedColumns[i].Width = 60;
                    gridband3.Columns.Add(bandedColumns[i]);
                    bandedColumns[i].OwnerBand = gridband3;
                    bandedColumns[i].Visible = true;
                    gridband3.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    gridband3.AppearanceHeader.Options.UseFont = true;
                    gridband3.AppearanceHeader.Options.UseTextOptions = true;
                    gridband3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    grHeader[i] = gridband3;

                }
                gridBand.Children.AddRange(grHeader);
                bandedView.Bands.Add(gridBand);
            }
        }

        private void CreateSearchLookupHT()
        {
            string url = string.Format("{0}?", URL + "HinhThuc/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbht = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbht;
            rCountryEdit.DisplayMember = "TenHT";
            rCountryEdit.ValueMember = "MaHT";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "";
            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHT", Caption = "Mã màu", Name = "colMaMau", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHT", Caption = "Tên màu", Name = "colTenMau", Visible = true });

            }
            gridColumn18.ColumnEdit = rCountryEdit;
        }
    }
}
