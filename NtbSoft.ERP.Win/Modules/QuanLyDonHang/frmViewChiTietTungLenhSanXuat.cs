using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmViewChiTietTungLenhSanXuat : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<CanDoiDonHangTongEntity> lstDonHangTong;
        bool indicatorIcon = true;
        private string _madh = string.Empty, _mahang = string.Empty, _tenhang = string.Empty, _malenh = string.Empty, _dot = string.Empty, _madvsx = string.Empty, _malenhsanxuat = string.Empty, _poid = string.Empty, _tenlenh = string.Empty, _tendvsx = string.Empty, _po = string.Empty;
        private int rowhandel = 0, rowhandel1 = 0, sum = 0;
        DataTable _dtDataLSX;
        DataTable _dtDataEx;
        DataTable _dtDataExd;
        private int _valueSum = 0;
        int pageIndex = 1;
        int pageSize = 100;
        DataTable tbl;
        int loc = 0;
        string selectedValuesMH = string.Empty;
        string selectedValuessMH = string.Empty;
        public frmViewChiTietTungLenhSanXuat()
        {
            InitializeComponent();

            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonHangTong = new List<CanDoiDonHangTongEntity>();
            _dtDataLSX = new DataTable();
            _dtDataEx = new DataTable();
            _dtDataExd = new DataTable();
            tbl = new DataTable();
        }
        protected override void OnLoad(EventArgs e)
        {
            barEditItemSpinPageSize.EditValue = pageSize;
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            barEditItemcbSort.EditValue = repositoryItemComboBox2.Items[0].ToString();
            comboBoxEditLoc.SelectedIndex = 0;
            barEditItemDVSX.EditValue = "0";
            LoadLoc();
            CreateDefault();
            LoadData();

        }
        private void LoadLoc()
        {
            string urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable dtHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditMaHang.Properties.DataSource = dtHH;
            searchLookUpEditMaHang.Properties.ValueMember = "MaHang";
            searchLookUpEditMaHang.Properties.DisplayMember = "TenHang";
            searchLookUpEditMaHang.Properties.Appearance.ForeColor = Color.Red;
            GridView dvView = searchLookUpEditMaHang.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });

            }
            searchLookUpEditMaHang.CustomDisplayText += SearchLookUpEditMaHang_CustomDisplayText;
            //string urlKH = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            //string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
            //DataTable dtKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
            //searchLookUpEditKhachHang.Properties.DataSource = dtKH;
            //searchLookUpEditKhachHang.Properties.ValueMember = "MaHang";
            //searchLookUpEditKhachHang.Properties.DisplayMember = "TenHang";
            //searchLookUpEditKhachHang.Properties.Appearance.ForeColor = Color.Red;
            //GridView dvViewKH = searchLookUpEditKhachHang.Properties.View;
            //if (dvView.Columns.Count == 0)
            //{
            //    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
            //    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            //    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            //    dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã KH", Name = "colMaKH", Visible = false });
            //    dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "KhachHang", Name = "colTenKH", Visible = true });

            //}
            #region
            //if (comboBoxEditLoc.SelectedIndex == 1)
            //{
            //    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //    searchLookUpEditMaHang.Visible = false;
            //    searchLookUpEditKhachHang.Visible = false;
            //    textEditDot.Visible = false;
            //    textEditSoLuong.Visible = false;
            //    btnTimKiem.Size = new Size(95, 44);
            //}
            //else
            //{
            //    if (comboBoxEditLoc.SelectedIndex == 2)
            //    {
            //        layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //        layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //        layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //        layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //        layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //        layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //        layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //        btnTimKiem.Size = new Size(95, 44);
            //    }
            //    else
            //    {
            //        if (comboBoxEditLoc.SelectedIndex == 3)
            //        {
            //            layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //            layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //            layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //            layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //            layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //            btnTimKiem.Size = new Size(95, 44);
            //        }
            //        else
            //        {
            //            if (comboBoxEditLoc.SelectedIndex == 3)
            //            {
            //                layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                btnTimKiem.Size = new Size(95, 44);
            //            }
            //            else
            //            {
            //                if (comboBoxEditLoc.SelectedIndex == 4)
            //                {
            //                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    btnTimKiem.Size = new Size(95, 44);
            //                }
            //                else
            //                {
            //                    layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //                    btnTimKiem.Size = new Size(95, 44);
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion
        }


        private void CreateDefault()
        {
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            //DataRow newRow = dtDVSX.NewRow();
            //newRow["ID"] = 1;
            //newRow["MaDVSX"] = "0";
            //newRow["TenDVSX"] = "TẤT CẢ";
            //dtDVSX.Rows.InsertAt(newRow, 0);
            searchLookUpEditDVSX1.Properties.DataSource = dtDVSX;
            searchLookUpEditDVSX1.Properties.ValueMember = "MaDVSX";
            searchLookUpEditDVSX1.Properties.DisplayMember = "TenDVSX";
            searchLookUpEditDVSX1.Properties.Appearance.ForeColor = Color.Red;
            GridView dvView = searchLookUpEditDVSX1.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaDVSX", Caption = "Mã DVSX", Name = "colMaDVSX", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVSX", Caption = "Đơn vị sản xuất", Name = "colTenDVSX", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "GiaCong", Caption = "Gia công", Name = "colGiaCong", Visible = true });

            }
           
        }

        private void LoadData()
        {
           
            if (loc == 0)
            {
                string url = string.Format("{0}?pageIndex={1}&&pageSize={2}&&madvsx={3}", URL + "CanDoiDonHangTong/GetDonHangTongView", pageIndex, pageSize, searchLookUpEditDVSX1.EditValue == null ? "" : searchLookUpEditDVSX1.EditValue.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string urlloc = string.Format("{0}?pageIndex={1}&&pageSize={2}&&madvsx={3}&&tungay={4}&&denngay={5}&&mahang={6}&&dot={7}&&sl={8}", URL + "CanDoiDonHangTong/GetDonHangTongViewLoc", 
                    pageIndex, pageSize, searchLookUpEditDVSX1.EditValue == null ? "" : searchLookUpEditDVSX1.EditValue.ToString(),
                    dateEditTuNgay.EditValue == null ? "" : dateEditTuNgay.EditValue.ToString(), dateEditDenNgay.EditValue == null ? "" : dateEditDenNgay.EditValue.ToString(), selectedValuesMH.ToString() == "" ? "" : selectedValuesMH.ToString(),
                    textEditDot.Text.ToString() == "" ? "" : textEditDot.Text.ToString(), textEditSoLuong.Text.ToString() == "" ? "" : textEditSoLuong.Text.ToString());
                string jsonloc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlloc); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(jsonloc);
            }    
           
            if (tbl == null || (tbl != null && tbl.Rows.Count <= 0))
            {
                NtbSoft.ERP.Libs.clsConvert<CanDoiDonHangTongEntity> convert = new Libs.clsConvert<CanDoiDonHangTongEntity>();
                DataTable tblnull = convert.ToDataTable(lstDonHangTong);
                gridDonHangTong.DataSource = tblnull;
            }
            else
            {
                if(barEditItemcbLoc.EditValue != null)
                {
                    if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "CHUACANDOISANXUAT")
                    {
                        var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLTH"]) == 0 && Convert.ToInt32(row["SLCL"]) != 0);
                        if (selectedRows.Any())
                            tbl = selectedRows.CopyToDataTable();
                        else
                        {
                            MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        var sortedRows = tbl.AsEnumerable().OrderBy(row => row["MaDH"].ToString().Substring(4, row["MaDH"].ToString().Length));

                    }
                    if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "DACANDOISANXUATXONG")
                    {
                        var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLCL"]) == 0);
                        if (selectedRows.Any())
                            tbl = selectedRows.CopyToDataTable();
                        else
                        {
                            MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "CANDOISANXUATCHUAXONG")
                    {
                        var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLTH"]) != 0 && Convert.ToInt32(row["SLCL"]) != 0);
                        if (selectedRows.Any())
                            tbl = selectedRows.CopyToDataTable();
                        else
                        {
                            MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }               
                if(barEditItemcbSort.EditValue != null)
                {
                    if (RemoveVietnameseTone(barEditItemcbSort.EditValue.ToString().Trim().Replace(" ", "").ToUpper()).ToUpper() == "DONHANGTANGDAN")
                    {
                        var sortedRows = tbl.AsEnumerable().OrderBy(row => Convert.ToInt32(row["SortDH"]));
                        tbl = sortedRows.CopyToDataTable();
                    }
                    else
                    {
                        if (RemoveVietnameseTone(barEditItemcbSort.EditValue.ToString().Trim().Replace(" ", "").ToUpper()).ToUpper().ToUpper() == "DONHANGGIAMDAN")
                        {
                            var sortedRows = tbl.AsEnumerable().OrderByDescending(row => Convert.ToInt32(row["SortDH"]));
                            tbl = sortedRows.CopyToDataTable();

                        }
                        else
                        {
                            if (RemoveVietnameseTone(barEditItemcbSort.EditValue.ToString().Trim().Replace(" ", "").ToUpper()).ToUpper() == "NGAYTAOTANGDAN")
                            {
                                var sortedRows = tbl.AsEnumerable().OrderBy(row => Convert.ToDateTime(row["NgayTao"]));
                                tbl = sortedRows.CopyToDataTable();
                            }
                            else
                            {
                                if (RemoveVietnameseTone(barEditItemcbSort.EditValue.ToString().Trim().Replace(" ", "").ToUpper()).ToUpper() == "NGAYTAOGIAMDAN")
                                {
                                    var sortedRows = tbl.AsEnumerable().OrderByDescending(row => Convert.ToDateTime(row["NgayTao"]));
                                    tbl = sortedRows.CopyToDataTable();
                                }
                            }

                        }
                    }
                }    
               
                gridDonHangTong.DataSource = tbl;
                gridViewDonHangTong.FocusedRowHandle = rowhandel;
                LoadLenhSX(this.gridViewDonHangTong);
                gridView1.FocusedRowHandle = rowhandel1;
            }
        }
        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        private void LoadLenhSX(GridView view)
        {
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();
            else
                _madh = "";

            string urlLSX = string.Format("{0}?madh={1}&&madvsx={2}", URL + "CanDoiDonHangTong/GetChiTietLenhSX", _madh, searchLookUpEditDVSX1.EditValue == null ? "" : searchLookUpEditDVSX1.EditValue.ToString());
            string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
            _dtDataLSX = JsonConvert.DeserializeObject<DataTable>(jsonLSX);
            gridControl1.MainView = GetBandGridViewAmount(_dtDataLSX);
            BandedGridView mainView = (BandedGridView)gridControl1.MainView;
            mainView.CustomUnboundColumnData += MainView_CustomUnboundColumnData;
            mainView.CustomSummaryCalculate += MainView_CustomSummaryCalculate;
            mainView.CustomDrawBandHeader += MainView_CustomDrawBandHeader;
            mainView.CustomDrawFooter += MainView_CustomDrawFooter;
            mainView.CustomDrawFooterCell += MainView_CustomDrawFooterCell;
            mainView.CustomColumnDisplayText += MainView_CustomColumnDisplayText;
            mainView.CustomDrawGroupRow += MainView_CustomDrawGroupRow;
            mainView.CustomDrawRowFooter += MainView_CustomDrawRowFooter;
            mainView.RowClick += gridView1_RowClick;
            gridControl1.DataSource = _dtDataLSX;

        }

        private void MainView_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            // Kiểm tra xem dòng này có phải là footer của group không
            if (view.IsGroupRow(e.RowHandle))
            {
                // Đổi màu nền
                e.Appearance.BackColor = Color.Yellow; // Thay đổi thành màu bạn muốn
            }
        }

        private void gridViewDonHangTong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedRowHandle >= 0)
            {
                rowhandel = view.FocusedRowHandle;
            }
            LoadLenhSX(this.gridViewDonHangTong);
        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            bandedView.OptionsView.AllowCellMerge = true;
            bandedView.OptionsBehavior.AutoExpandAllGroups = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            //bandedView.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 16; i++)
            {
                List<string> _ListString = new List<string>();
                if (tab.Columns[i].ColumnName == "TenDVSX")
                {
                    _ListString.Add("TenDVSX");
                    SetGridBandedViewAmount(bandedView, "", "ĐƠN VỊ SX", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "MaLenh")
                {
                    _ListString.Add("MaLenh");
                    SetGridBandedViewAmount(bandedView, "", "Mã lệnh SX", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmount(bandedView, "", "PO", _ListString);
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
                    SetGridBandedViewAmount(bandedView, "", "Đầu Size", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 16;
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;

            }
            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            colAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Amount";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);
            foreach (BandedGridColumn col in bandedView.Columns)
            {

                if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh")
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
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        private void SetGridBandedViewAmount(BandedGridView bandedView, string GridBandHeader, string GridBandCaption, List<string> columnNames)
        {

            GridBand gridBand = new GridBand();
            if (GridBandHeader == "")
                gridBand.Caption = GridBandCaption;
            else
                gridBand.Caption = GridBandHeader;

            int nrOfColumns = columnNames.Count;
            gridBand.Fixed = FixedStyle.None;
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (columnNames[0] == "TenDVSX")
            {
                gridBand.Visible = false;

            }

            if (nrOfColumns == 1 && (columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "TenMau" || columnNames[0] == "DauSize" || columnNames[0] == "MaQG" || columnNames[0] == "MaDH" || columnNames[0] == "DauSizeID")
                || columnNames[0] == "MaKH" || columnNames[0] == "MaHang" || columnNames[0] == "TenHang" || columnNames[0] == "DauSizeID" || columnNames[0] == "TenDVSX" || columnNames[0] == "MaLenh")
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);
                bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                if (columnNames[0] == "PO" || columnNames[0] == "MaLenh")
                {
                    bandedColumns.Visible = false;
                    bandedColumns.OptionsColumn.AllowEdit = false;
                    bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                }
                if (columnNames[0] == "TenDVSX")
                {
                    bandedColumns.Visible = false;
                    bandedColumns.GroupIndex = 0;

                }

                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;

                bandedColumns.Caption = GridBandCaption;
                bandedColumns.Visible = true;
                bandedColumns.Width = 85;

                gridBand.Fixed = FixedStyle.Left;
                gridBand.RowCount = 1;

                bandedView.Bands.Add(gridBand);


            }
            else
            {
                BandedGridColumn[] bandedColumns = new BandedGridColumn[nrOfColumns];
                GridBand[] grHeader = new GridBand[nrOfColumns];
                for (int i = 0; i < nrOfColumns; i++)
                {
                    String[] _colName = columnNames[i].Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (_colName[0].ToString() != "TenDVSX")
                    {
                        GridBand gridband3 = new GridBand();
                        gridband3.Caption = _colName[2];
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

                }

                gridBand.Children.AddRange(grHeader);
                bandedView.Bands.Add(gridBand);


            }


        }

        private void barButtonItemExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = this.button1;
            DataTable tbl = gridDonHangTong.DataSource as DataTable;
            var selectedRows = tbl.AsEnumerable().Where(row => (bool)(row["Chon"]) == true);
            if (selectedRows.Any())
                tbl = selectedRows.CopyToDataTable();
            else
            {
                MessageBox.Show("Bạn chưa chọn dữ liệu xuất Excel. Vui lòng chọn dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (tbl == null && tbl.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng !");
                return;
            };
            _row = 10;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            Sfd.FileName = string.Format("ChiTietLenhSX_" + DateTime.Now.ToString("ddMMyyyy"));
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
                    ExportExcel(Sfd.FileName, tbl);
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
        int _row = 10;

        private void TheadTable(DataTable tbl, ExcelWorksheet worksheet, bool checkColor = false)
        {
            int rowFisrt = _row;
            List<string> columnNamesWithSize = tbl.Columns.Cast<DataColumn>()
                       .Where(column => column.ColumnName.Contains("@"))
                       .Select(column => column.ColumnName)
                       .Distinct()
                       .ToList();
            ExcelRange range = worksheet.Cells;
            range = worksheet.Cells[_row, 1, _row + 1, 1]; range.Merge = true; range.Value = "COLOR CODE"; range.Style.Font.Color.SetColor(Color.Red);
            range = worksheet.Cells[_row, 2, _row + 1, 2]; range.Merge = true; range.Value = "PO"; range.Style.Font.Color.SetColor(Color.Red);
            range = worksheet.Cells[_row, 3, _row + 1, 3]; range.Merge = true; range.Value = "SIZE/ INSEAM";
            range = worksheet.Cells[_row, 4, _row + 1, 4]; range.Merge = true; range.Value = "COUNTRY";
            range = worksheet.Cells[_row, 5, _row + 1, 5]; range.Merge = true; range.Value = "Ngày GH"; range.Style.Font.Color.SetColor(Color.Red);
            int colums = 6;
            range = worksheet.Cells[_row, colums, _row, colums + columnNamesWithSize.Count - 1]; range.Value = "Size"; range.Merge = true; range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;
            foreach (var item in columnNamesWithSize)
            {
                string[] _split = item.Split('@');
                string _size = _split[2].ToString();
                worksheet.Cells[_row + 1, colums].Value = _size;
                worksheet.Cells[_row + 1, colums].Style.Font.Bold = true;
                colums++;
            }
            for (int i = 1; i < 5; i++)
            {
                if (i != 2 && i != 5)
                    worksheet.Column(i).Width = 15;
            }
            range = worksheet.Cells[_row, colums, _row + 1, colums]; range.Value = "Grand Total"; range.Style.WrapText = true; range.Merge = true; range.Style.Font.Bold = true;
            range = worksheet.Cells[_row, colums + 1, _row + 1, colums + 1]; range.Value = "Note"; range.Style.WrapText = true; range.Merge = true; range.Style.Font.Bold = true;
            _row += 2;
            foreach (DataRow item in tbl.Rows)
            {
                range = worksheet.Cells[_row, 1]; range.Value = item["TenMau"].ToString(); range.Style.Font.Color.SetColor(Color.Red);
                worksheet.Cells[_row, 2].Value = item["PO"].ToString();
                worksheet.Cells[_row, 3].Value = item["DauSize"].ToString();
                worksheet.Cells[_row, 4].Value = item["TenQG"].ToString();
                range = worksheet.Cells[_row, 5]; range.Value = "";
                int sumSize = 0;
                colums = 6;
                foreach (var lstSize in columnNamesWithSize)
                {
                    object value = item[lstSize];
                    int convertedValue = value.ToString() != "" ? Convert.ToInt32(value) : 0;
                    worksheet.Cells[_row, colums].Value = convertedValue;
                    if (checkColor)
                        worksheet.Cells[_row, colums].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
                    sumSize += convertedValue;
                    colums++;
                }
                worksheet.Cells[_row, colums].Value = sumSize;
                worksheet.Cells[_row, colums + 1].Value = item["TenDVSX"].ToString();
                worksheet.Column(colums + 1).AutoFit();
                worksheet.Column(2).AutoFit(); worksheet.Column(5).AutoFit();
                _row++;
            }
            range = worksheet.Cells[_row, 1, _row, 5]; range.Value = "Tổng"; range.Merge = true; range.Style.Font.Bold = true;
            for (int i = 6; i < colums + 1; i++)
            {
                worksheet.Cells[_row, i].Formula = "=SUM(" + worksheet.Cells[rowFisrt, i].Address + ":" + worksheet.Cells[_row - 1, i].Address + ")";
                worksheet.Cells[_row, i].Style.Font.Bold = true;
            }
            var borderData = worksheet.Cells[rowFisrt, 1, _row, colums + 1].Style.Border;
            borderData.Bottom.Style =
              borderData.Top.Style =
              borderData.Left.Style =
              borderData.Right.Style = ExcelBorderStyle.Thin;
            _row++;
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadLoc();
            CreateDefault();
            loc = 0;
            selectedValuessMH = "";
            selectedValuesMH = "";
            searchLookUpEditDVSX1.EditValue = null;
            dateEditTuNgay.EditValue = null;
            dateEditDenNgay.EditValue = null;
            searchLookUpEditMaHang.EditValue = null;
            searchLookUpEditMaHang.Text = null;
            textEditDot.Text = null;
            textEditSoLuong.Text = null;
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            barEditItemcbSort.EditValue = repositoryItemComboBox2.Items[0].ToString();
            LoadData();
        }

        private void barButtonItemPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            LoadData();
        }

        private void barButtonItemNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.Caption = pageIndex.ToString();
            LoadData();
        }

        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                LoadData();
            }
        }
        private void gridView1_RowClick(object sender, RowClickEventArgs e)
        {
            if (gridViewDonHangTong.FocusedRowHandle >= 0)
            {
                rowhandel1 = gridViewDonHangTong.FocusedRowHandle;
            }
        }
        private void gridViewDonHangTong_RowClick(object sender, RowClickEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedRowHandle >= 0)
            {
                rowhandel = view.FocusedRowHandle;
            }
            LoadLenhSX(this.gridViewDonHangTong);
        }

        private void gridViewDonHangTong_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLTH"]);
                string Strfcl = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLCL"]);
                if (Strfcl == "0")
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
                    e.HighPriority = true;
                }
                if (Strflth != "0" && Strfcl != "0")
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99");
                    e.HighPriority = true;
                }
                if(e.RowHandle == gridViewDonHangTong.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold ;
                    e.HighPriority = true;
                }
            }
        }

        private void barEditItemcbLoc_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void gridViewDonHangTong_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {

                foreach (DataRow _dr in tbl.Rows)
                {
                    _dr["Chon"] = true;
                }
                gridDonHangTong.RefreshDataSource();
                e.Handled = true;
            }
            switch (e.KeyCode)
            {
                case Keys.F5:
                    LoadData();
                    break;
            }
        }

        private void gridViewDonHangTong_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedRowHandle >= 0)
            {
                rowhandel = view.FocusedRowHandle;
            }
            LoadLenhSX(this.gridViewDonHangTong);
        }

        private void barEditItemDVSX_EditValueChanged(object sender, EventArgs e)
        {
          
           // LoadData();
        }

        private void ExportExcel(string path, DataTable tbl)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    //ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    for (int r = 0; r < tbl.Rows.Count; r++)
                    {
                        _row = 10;
                        string urlLSX = string.Format("{0}?madh={1}&&madvsx={2}", URL + "CanDoiDonHangTong/GetChiTietLenhSX", tbl.Rows[r]["MaDH"].ToString(), barEditItemDVSX.EditValue == null ? "0" : barEditItemDVSX.EditValue.ToString());
                        string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
                        DataTable _dtDataLSXT = JsonConvert.DeserializeObject<DataTable>(jsonLSX);
                        string urlEx = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetCTTotal", tbl.Rows[r]["MaDH"].ToString());
                        string jsonEx = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlEx); }).Result;
                        _dtDataEx = JsonConvert.DeserializeObject<DataTable>(jsonEx);
                        string urlExd = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetCTTotalDetail", tbl.Rows[r]["MaDH"].ToString());
                        string jsonExd = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlExd); }).Result;
                        _dtDataExd = JsonConvert.DeserializeObject<DataTable>(jsonExd);
                        string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCELLenhSX&Para1=A&Para2={ tbl.Rows[r]["MaDH"].ToString()}&Para3=A");
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                        DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                        int Val = r + 1;
                        string StrSheet = tbl.Rows[r]["MaDH"].ToString() + "|" + tbl.Rows[r]["MaHang"].ToString();

                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(StrSheet);
                        int Height = 100;
                        int Width = 150;

                        ExcelRange range = worksheet.Cells;
                        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        worksheet.Cells.Style.Font.Size = 11;

                        range = worksheet.Cells["A1:C2"]; range.Merge = true; range.Value = "Công ty Cổ Phần \n Tex - Giang"; range.Style.Font.Bold = true; range.Style.WrapText = true;
                        range = worksheet.Cells["e2:g2"]; range.Merge = true; range.Value = "Lệnh Sản Xuất"; range.Style.Font.Size = 18; range.Style.Font.Bold = true;
                        range = worksheet.Cells["A3:b3"]; range.Merge = true; range.Value = "BUYER"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["c3"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenKH"].ToString(); range.Style.Font.Bold = true;
                        range = worksheet.Cells["A4:b4"]; range.Merge = true; range.Value = "MÃ HÀNG"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["c4"]; range.Merge = true; range.Value = dtTable.Rows[0]["MaKH"].ToString(); range.Style.Font.Bold = true;
                        range = worksheet.Cells["A5:b5"]; range.Merge = true; range.Value = "SEASON"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["c5"]; range.Merge = true; range.Value = dtTable.Rows[0]["Dot"].ToString(); range.Style.Font.Bold = true;
                        range = worksheet.Cells["A6:b6"]; range.Merge = true; range.Value = "XƯỞNG SẢN XUẤT"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["c6"]; range.Merge = true; range.Value = ""; range.Style.Font.Bold = true;
                        range = worksheet.Cells["A7:b7"]; range.Merge = true; range.Value = "TÊN HÀNG"; range.Style.Font.Bold = true;
                        range = worksheet.Cells["c7"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenHang"].ToString(); range.Style.Font.Bold = true; range.Style.WrapText = true;
                        List<string> lstDVSX = _dtDataLSXT.AsEnumerable().Select(x => x.Field<string>("MaDVSX")).Distinct().ToList();
                        foreach (var item in lstDVSX)
                        {
                            DataTable lstTbale = _dtDataLSXT.AsEnumerable().Where(x => x.Field<string>("MaDVSX") == item.ToString()).CopyToDataTable();
                            TheadTable(lstTbale, worksheet); _row++;
                        }
                        TheadTable(_dtDataEx, worksheet, true); _row++;
                        TheadTable(_dtDataExd, worksheet, true); _row++;
                        for (int i = 10; i < 300; i++)
                        {
                            worksheet.Row(i).Height = 18;
                        }

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

        private void comboBoxEditLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLoc();
        }


        private void gridViewDonHangTong_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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


        private void gridViewDonHangTong_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void MainView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void textEditSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void MainView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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

        private void MainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void MainView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh" && col.UnboundType == UnboundColumnType.Bound)
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

        private void gridViewDonHangTong_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "SLKH")
            {
                e.Appearance.ForeColor = Color.Green;
            }
            else
            {
                if (e.Column.FieldName == "SLTH")
                {
                    e.Appearance.ForeColor = Color.Navy;
                }
                else
                {
                    if (e.Column.FieldName == "SLCL")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }

            }
        }
        private void MainView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red; // Màu đỏ
        }


        private void MainView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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

        private void MainView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info != null)
            {
                // Kiểm tra xem nó là group footer bạn muốn tùy chỉnh
                if (info.Column.FieldName == "PO" && view.IsGroupRow(e.RowHandle))
                {
                    // Đổi màu nền
                    e.Appearance.BackColor = Color.Yellow; // Thay đổi thành màu bạn muốn
                }
            }
            e.Appearance.ForeColor = Color.Red;
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void barEditItemcbSort_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void searchLookUpEdit1View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedValuesMH = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditMaHang.Properties.ValueMember)));
            searchLookUpEditMaHang.EditValue = selectedValuesMH;
        }

        private void SearchLookUpEditMaHang_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessMH = string.Join(", ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditMaHang.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessMH.ToString()))
            {
                e.DisplayText = "Chọn mã hàng";
            }
            else
            {
                e.DisplayText = selectedValuessMH.ToString();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if(dateEditTuNgay.EditValue != null || dateEditTuNgay.EditValue != null || selectedValuesMH.ToString() != "" || textEditDot.Text.ToString() != "" || textEditSoLuong.Text.ToString() != "" || searchLookUpEditDVSX1.EditValue != null)
                loc = 1;
            else
            {
                XtraMessageBox.Show("Vui lòng chọn dữ liệu cần lọc.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }    
            LoadData();
        }
    }
}
