using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PopupMenuShowingEventArgs = DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmPhieuMuaHang : DevExpress.XtraEditors.XtraForm
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
        DataTable _dtTable;
        DataTable _dtTableCP;
        DataTable _dtTableDot;
        DataTable _dtTableChiTiet;
        DataTable _dtTableHTTT;
        SearchCheckSelection gridCheckMarksVT;
        string vt = string.Empty, _maphieumh = string.Empty, _mancc = string.Empty, _maphieubg = string.Empty, _tenphieumh = string.Empty, _nguoitao = string.Empty, _tienteID = string.Empty, _pomua = string.Empty
            , _mtt = string.Empty;
        bool _IsEdit;
        bool _IsDuyet;
        bool _IsAdd;
        bool _IsView;
        bool _IsMuaTheoVT;
        bool _IsXacNhan;
        bool _isCopyMode = false;
        DataRow _rowFocused = null;
        int currentThuTu = 1;
        int thuTuDotTraTien = 1;
        decimal _tongVC = 0;
        decimal _tongvccpps = 0;
        decimal _thuevc = 0;
        private bool _isUpdatingGV5 = false;
        private bool _coDotThanhToan = false;
        string _NgayTao;
        private bool _isLoadingTyGia = false;
        decimal _tongCPPS = 0;
        decimal _tongCPVC = 0;
        decimal _tongCP = 0;
        decimal _tongCPVND = 0;
        decimal _tongCPVT = 0;
        decimal _oldValue = 0;
        private decimal _oldSoLuongVC = 0;
        decimal _oldPhanTram;
        decimal _oldSoTien;
        PopupMenu popupTTChietKhau;
        BarButtonItem btnApplyTTChietKhau;
        GridColumn _popupColumn;
        public frmPhieuMuaHang(DataRow rowFocused = null, bool IsEdit = false, bool IsAdd = false, bool IsView = false, bool IsMuaTVT = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            _IsEdit = IsEdit;
            _IsAdd = IsAdd;
            _IsView = IsView;
            _IsMuaTheoVT = IsMuaTVT;
            if (_IsEdit == true || _IsView == true)
            {
                _rowFocused = rowFocused;
                _maphieumh = _rowFocused["MaPhieuMH"]?.ToString();
                _mancc = _rowFocused["MaNCC"]?.ToString();
                //_maphieubg = _rowFocused["MaPhieuBG"]?.ToString();
                _tenphieumh = _rowFocused["TenPhieu"]?.ToString();
                _nguoitao = _rowFocused["NguoiTao"]?.ToString();
                _NgayTao = _rowFocused["NgayTao"]?.ToString();
                _IsDuyet = Convert.ToBoolean(_rowFocused["IsDuyet"]);
                _tongCPPS = GetDecimalFromObject(_rowFocused["TongCPPS"]);
                _tongCPVC = GetDecimalFromObject(_rowFocused["TongCPVC"]);
                _tongCP = GetDecimalFromObject(_rowFocused["TongTien"]);
                //_tongCPVND = GetDecimalFromObject(_rowFocused["TongTienVND"]);
                _tongCPVND = GetDecimalFromObject(_rowFocused["TongTien"]);
                _tongCPVT = GetDecimalFromObject(_rowFocused["TongCPVT"]);
                _pomua = _rowFocused["POMua"]?.ToString();
                _IsXacNhan = Convert.ToBoolean(_rowFocused["IsXacNhan"]);
                _mtt = _rowFocused["MaTienTe"]?.ToString();
                bool muatheovt = string.IsNullOrEmpty(_rowFocused["MaDH_Gop"]?.ToString()) ? true : false;
                _IsMuaTheoVT = muatheovt;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchlookupNCC();
            SetupNumericColumns();
            CreateSearchlookupTienTe();
            InitPopupTTChietKhau();
            //barButtonItem3.Visibility = BarItemVisibility.Never;
            barButtonItem4.Visibility = BarItemVisibility.Never;
            //CreateDefaultSearchLookUp();
            if (_IsEdit == true)
            {
                if (_IsDuyet)
                {
                    barButtonItem3.Visibility = BarItemVisibility.Never;
                    barButtonItem4.Visibility = BarItemVisibility.Always;
                    layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    gridView1.OptionsBehavior.Editable = false;
                    gridView1.OptionsBehavior.ReadOnly = true;
                    textEditPOMua.Properties.ReadOnly = true;
                }
                else
                {
                    barButtonItem3.Visibility = BarItemVisibility.Always;
                    barButtonItem4.Visibility = BarItemVisibility.Never;
                }
                barButtonItem3.Visibility = BarItemVisibility.Never;
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEditNCC.EditValue = _mancc.ToString();
                //searchLookUpEditPBG.EditValue = _maphieubg.ToString();
                txtNguoiTao.EditValue = _nguoitao;
                DEditNgayTao.EditValue = _NgayTao;
                textEditPOMua.Text = _pomua;
                searchLookUpEditPBG.ReadOnly = true;
                txtPhieuMH.ReadOnly = true;
                searchLookUpEditNCC.ReadOnly = true;
                textEditPOMua.ReadOnly = true;
                spinEditTyGia.ReadOnly = true;
                txtTienTe.EditValue = _mtt;

                spinEditTongCPVT.EditValue = _tongCPVT;
                spinEditTongCPPS.EditValue = _tongCPPS;
                spinEditTongCP.EditValue = _tongCP;
                spinEditTongCPVND.EditValue = _tongCPVND;
                CreateSearchLookup();
                LoadData();
                LoadDataChiPhi();
                LoadDataDotGH();
                LoadDataHTTT();
                int soChungLoai = 0;

                var list = new HashSet<string>();

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    var mavtid = gridView1.GetRowCellValue(i, "MaVTID")?.ToString();
                    var mauvtid = gridView1.GetRowCellValue(i, "MauVTID")?.ToString();
                    var khovaiid = gridView1.GetRowCellValue(i, "KhoVaiID")?.ToString();
                    var madvvt = gridView1.GetRowCellValue(i, "MaDVVT")?.ToString();

                    string key = string.Join("|", mavtid, mauvtid, khovaiid, madvvt);
                    if (key == "|||") continue;
                    list.Add(key);
                }

                soChungLoai = list.Count;

                spinEditSoLoaiVT.EditValue = soChungLoai;
            }
            if (_IsAdd)
            {
                DEditNgayTao.EditValue = DateTime.Now;
                CreateSearchLookup();

                GeneratePhieuMH();
            }
            if (_IsView == true)
            {
                if (_IsDuyet)
                {
                    barButtonItem3.Visibility = BarItemVisibility.Never;
                    barButtonItem4.Visibility = BarItemVisibility.Always;
                    gridView1.OptionsBehavior.Editable = false;
                    gridView1.OptionsBehavior.ReadOnly = true;
                }
                else
                {
                    barButtonItem3.Visibility = BarItemVisibility.Always;
                    barButtonItem4.Visibility = BarItemVisibility.Never;
                }
                barButtonItem1.Visibility = BarItemVisibility.Never;
                barButtonItem2.Visibility = BarItemVisibility.Never;
                layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEditNCC.EditValue = _mancc.ToString();
                //searchLookUpEditPBG.EditValue = _maphieubg.ToString();
                txtNguoiTao.EditValue = _nguoitao;
                DEditNgayTao.EditValue = _NgayTao;
                textEditPOMua.Text = _pomua;
                searchLookUpEditPBG.ReadOnly = true;
                txtPhieuMH.ReadOnly = true;
                searchLookUpEditNCC.ReadOnly = true;
                textEditPOMua.ReadOnly = true;
                spinEditTyGia.ReadOnly = true;
                txtTienTe.EditValue = _mtt;

                spinEditTongCPVT.EditValue = _tongCPVT;
                spinEditTongCPPS.EditValue = _tongCPPS;
                spinEditTongCP.EditValue = _tongCP;
                spinEditTongCPVND.EditValue = _tongCPVND;
                CreateSearchLookup();
                LoadData();
                LoadDataChiPhi();
                LoadDataDotGH();
                LoadDataHTTT();
                int soChungLoai = 0;

                var list = new HashSet<string>();

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    var mavtid = gridView1.GetRowCellValue(i, "MaVTID")?.ToString();
                    var mauvtid = gridView1.GetRowCellValue(i, "MauVTID")?.ToString();
                    var khovaiid = gridView1.GetRowCellValue(i, "KhoVaiID")?.ToString();
                    var madvvt = gridView1.GetRowCellValue(i, "MaDVVT")?.ToString();

                    string key = string.Join("|", mavtid, mauvtid, khovaiid, madvvt);
                    if (key == "|||") continue;
                    list.Add(key);
                }

                soChungLoai = list.Count;

                spinEditSoLoaiVT.EditValue = soChungLoai;

                gridView1.OptionsBehavior.Editable = false;
                gridView2.OptionsBehavior.Editable = false;
                gridView3.OptionsBehavior.Editable = false;
                gridView5.OptionsBehavior.Editable = false;
            }
            if (_IsMuaTheoVT == true)
            {
                DEditNgayTao.EditValue = DateTime.Now;
                CreateSearchLookup();
            }

            if(_rowFocused != null)
            {
                bool.TryParse(_rowFocused["IsDuyet1"]?.ToString(), out bool HasDuyet1);
                if (HasDuyet1)
                {
                    txtTienTe.Enabled = false;
                }
            }

            InitE_Invoice();
            InitPaymentTerm();
            CreateDefaultDateEdit();
            LoadIPAdress();
            LoadE_Invoice();


            textEditPOMua.EditValue = _pomua;
            txtPhieuMH.EditValue = _tenphieumh;
        }


        private void CreateSearchLookup()
        {
            try
            {
                string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
                string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
                DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);
                string username = string.Empty;
                if (_IsEdit == true || _IsView == true)
                {
                    username = _nguoitao;
                }
                else
                {
                    username = GlobleData.UserName.ToString();
                }
                DataRow ten = tblnv.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("UserID")?.Trim(), username.Trim(), StringComparison.OrdinalIgnoreCase));
                txtNguoiTao.Text = ten != null ? ten["Ten"].ToString() : username.ToString();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable apiTable = JsonConvert.DeserializeObject<DataTable>(json);
            if(apiTable?.Rows?.Count > 0)
            {
                SeachLookupKhachHang.EditValue = apiTable?.Rows[0]["MaKhachHang"];
            }
            _dtTable = CreateDatatableload();
            MapDataFromApi(apiTable, _dtTable);
            CreateSearchlookupcolPTThanhToan();

            if (!_dtTable.Columns.Contains("ThanhTienVND"))
                _dtTable.Columns.Add("ThanhTienVND", typeof(decimal));


            if (!_dtTable.Columns.Contains("ThanhTienQD"))
                _dtTable.Columns.Add("ThanhTienQD", typeof(decimal));


            foreach (DataRow r in _dtTable.Rows)
            {

                decimal ThanhTien = GetDecimalFromObject(r["ThanhTien"]);

                string mancc = r["MaNCC"]?.ToString() ?? "";

                string maTienTe = r["TienTeID"]?.ToString() ?? "VND";

                decimal gia = 1;

                if (!string.IsNullOrEmpty(mancc) && maTienTe != "VND")
                {
                    string url1 = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={maTienTe}&para2={"VND"}&para3=&para4=&para5=";

                    string json1 = Task.Run(async () => await _clientExtension.GetAsnyc(url1)).Result;

                    DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(json1);

                    if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        gia = Convert.ToDecimal(tblTyGia.Rows[0]["TyGia"]);
                }
                decimal tygia = GetDecimalFromObject(spinEditTyGia.EditValue);
                decimal thanhtienvnd = ThanhTien * gia;
                if (maTienTe == "VND")
                    r["ThanhTienVND"] = thanhtienvnd;
                else
                    r["ThanhTienVND"] = thanhtienvnd;

                r["ThanhTienQD"] = thanhtienvnd / tygia;
            }
            gridControl1.DataSource = _dtTable;

        }

        private void LoadDataChiPhi()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPMHChiPhi&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableCP = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl2.DataSource = _dtTableCP;
            CreateSearchlookupcolTienTe();
            if (_dtTableCP != null && _dtTableCP.Rows.Count > 0)
            {
                //decimal tygiavnd = 0;
                //object matiente = _dtTableCP.Rows[0]["DonViTienTe"];
                //string urldvtt = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
                //string jsondvtt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvtt); }).Result;
                //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsondvtt);

                //if (tbl != null && tbl.Rows.Count > 0)
                //{
                //    tygiavnd = GetDecimalFromObject(tbl.Rows[0]["TyGia"]);
                //}

                _isLoadingTyGia = true;
                DataRow[] rows = _dtTableCP.Select("MaChiPhi IN (3, 6)");

                if (rows.Length > 0)
                {
                    //foreach (DataRow r in rows)
                    //{
                    //    decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                    //    decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);
                    //    decimal sotienquydoi = soTien * tygiavnd;
                    //    decimal thuePercent = thue / 100m;
                    //    decimal tien = sotienquydoi + (thuePercent * sotienquydoi);
                    //    decimal tienqd = tien/ GetDecimalFromObject(spinEditTyGia.EditValue);
                    //    _tongvccpps += tienqd;
                    //}

                    foreach (DataRow r in rows)
                    {
                        string donViTienTe = r["DonViTienTe"]?.ToString();
                        if (string.IsNullOrEmpty(donViTienTe))
                            continue;

                        // Lấy tỷ giá theo từng dòng
                        string urldvtt = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={donViTienTe}&para2=VND&para3=&para4=&para5=";
                        string jsondvtt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvtt); }).Result;
                        DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(jsondvtt);

                        decimal tygiavnd = 0;
                        if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        {
                            tygiavnd = GetDecimalFromObject(tblTyGia.Rows[0]["TyGia"]);
                        }

                        decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                        decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);
                        decimal sotienquydoi = soTien * tygiavnd;
                        decimal thuePercent = thue / 100m;
                        decimal tien = sotienquydoi + (thuePercent * sotienquydoi);
                        decimal tienqd = tien / GetDecimalFromObject(spinEditTyGia.EditValue);
                        _tongvccpps += tienqd;
                    }

                    spinEditTongCPVC.EditValue = _tongvccpps;
                }
                else
                {
                    _tongvccpps = 0;
                    spinEditTongCPVC.EditValue = 0;
                }
            }


        }

        private void LoadDataDotGH()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPMHDOT&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = CreateDatatableDot();

            DataTable temp = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow r in temp.Rows)
            {
                DataRow n = dt.NewRow();

                foreach (DataColumn col in temp.Columns)
                {
                    string colName = col.ColumnName;

                    if (!dt.Columns.Contains(colName))
                        continue;

                    if (colName == "NgayNhanDK" || colName == "NgayNhanTT")
                    {
                        if (r[colName] == DBNull.Value || string.IsNullOrWhiteSpace(r[colName].ToString()))
                            n[colName] = DBNull.Value;
                        else
                            n[colName] = Convert.ToDateTime(r[colName]);
                    }
                    else
                    {
                        n[colName] = r[colName];
                    }
                }

                dt.Rows.Add(n);
            }

            _dtTableDot = dt;
            gridControl3.DataSource = _dtTableDot;
            if (_dtTableDot != null && _dtTableDot.Rows.Count > 0)
            {
                // Cách 1: Dùng LINQ
                int maxThuTu = _dtTableDot.AsEnumerable()
                                          .Where(row => row["ThuTu"] != DBNull.Value)
                                          .Select(row => Convert.ToInt32(row["ThuTu"]))
                                          .DefaultIfEmpty(0)
                                          .Max();

                currentThuTu = currentThuTu + maxThuTu;

            }
        }

        private void LoadDataHTTT()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETHTTT&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableHTTT = JsonConvert.DeserializeObject<DataTable>(json);
            if(_dtTableHTTT != null && _dtTableHTTT?.Rows?.Count > 0)
            {
                DataRow rowPaymnet = _dtTableHTTT?.Rows[0];
                SearchLookupPayment.EditValue = rowPaymnet["PTThanhToan"];
                SearchLookupInvoice.EditValue = rowPaymnet["InvoiceTo"];
                txtIncoterm.EditValue = rowPaymnet["Incoterm"];
                txtRemarkPayment.EditValue = rowPaymnet["GhiChu"];
            }


            //CreateSearchlookupcolPTThanhToanHT();
            //decimal tongTienQD = GetDecimalFromObject(spinEditTongCP.EditValue);
            //foreach (DataRow r in _dtTableHTTT.Rows)
            //{
            //    decimal phanTram = GetDecimalFromObject(r["PhanTram"]);

            //    if (phanTram > 0)
            //    {
                    
            //        decimal soTien = tongTienQD * phanTram / 100m;
            //        r["SoTien"] = Math.Round(soTien, 2);
            //    }
            //    else
            //    {
                    
            //        r["SoTien"] = GetDecimalFromObject(r["SoTien"]);
            //    }
            //}
            //gridControl4.DataSource = _dtTableHTTT;
            //if (_dtTableHTTT != null && _dtTableHTTT.Rows.Count > 0)
            //{
               
            //    int maxThuTu = _dtTableHTTT.AsEnumerable()
            //                              .Where(row => row["DotTra"] != DBNull.Value)
            //                              .Select(row => Convert.ToInt32(row["DotTra"]))
            //                              .DefaultIfEmpty(0)
            //                              .Max();

            //    thuTuDotTraTien = thuTuDotTraTien + maxThuTu;

            //}
        }

        private void CreateSearchlookupNCC()
        {
            string url = $"{URL}NhaCC/GePMH?action=GetNCC&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditNCC.Properties.DataSource = tbl;
            searchLookUpEditNCC.Properties.ValueMember = "MaNhaCC";
            searchLookUpEditNCC.Properties.DisplayMember = "TenKH";

        }

        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {

            string mancc = searchLookUpEditNCC.EditValue == null ? "" : searchLookUpEditNCC.EditValue.ToString();
            //string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={mancc}&para2=&para3=&para4=&para5=";
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            //_isLoadingTyGia = true;
            //if (tbl != null && tbl.Rows.Count > 0)
            //{
            //    txtTienTe.EditValue = tbl.Rows[0]["TenTienTe"];
            //    if(_IsEdit == true || _IsView == true)
            //    {
            //        string urltgtt = $"{URL}NhaCC/GePMH?action=GetTyGiaThanhToan&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            //        string jsontgtt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urltgtt); }).Result;
            //        DataTable tbltgtt = JsonConvert.DeserializeObject<DataTable>(json);
            //        if (_IsXacNhan)
            //        {
            //            spinEditTyGia.EditValue = tbltgtt.Rows[0]["TyGiaThanhToan"];
            //        }
            //        else 
            //        {
            //            spinEditTyGia.EditValue = tbl.Rows[0]["Gia"];
            //        }
            //    }
            //    else 
            //    {
            //        spinEditTyGia.EditValue = tbl.Rows[0]["Gia"];
            //    }

            //    _tienteID = tbl.Rows[0]["TIENTE"].ToString();
            //}
            //_isLoadingTyGia = false;
            //ApplyCurrencyFormat(spinEditTongCPVT, txtTienTe.Text);
            //ApplyCurrencyFormat(spinEditTongCP, txtTienTe.Text);
            //ApplyCurrencyFormat(spinEditTongCPVC, txtTienTe.Text);
            //ApplyCurrencyFormat(spinEditTongCPPS, txtTienTe.Text);
            //if (_IsAdd == true || (_IsMuaTheoVT == true && _IsView == false && _IsEdit == false))
            //{
            //    string url1 = $"{URL}NhaCC/GePMH?action=GETALLPHIEUMHCount&para1=&para2=&para3=&para4=&para5=";
            //    string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            //    DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

            //    DataTable tblDG = CreateDatatableToSave();

            //    int maxPhieu = 0;

            //    if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("MaPhieuMH"))
            //    {
            //        var validValues = _tbldv.AsEnumerable()
            //            .Select(r => r["MaPhieuMH"]?.ToString())
            //            .Where(s => !string.IsNullOrWhiteSpace(s) && s.StartsWith("PHIEU_"))
            //            .Select(s =>
            //            {
            //                string numberPart = s.Replace("PHIEU_", "").Trim();
            //                int num;
            //                return int.TryParse(numberPart, out num) ? num : 0;
            //            });

            //        if (validValues.Any())
            //            maxPhieu = validValues.Max();
            //    }

            //    int newPhieu = maxPhieu + 1;

            //    int maxDot = 0;

            //    if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("Dot"))
            //    {
            //        var validValues = _tbldv.AsEnumerable()
            //            .Select(r => r["Dot"]?.ToString())
            //            .Where(s => !string.IsNullOrWhiteSpace(s))
            //            .Select(s =>
            //            {
            //                int num;
            //                return int.TryParse(s, out num) ? num : 0;
            //            });

            //        if (validValues.Any())
            //            maxDot = validValues.Max();
            //    }

            //    int newDot = maxDot + 1;

            //    _tenphieumh = "PHIẾU MH|" + newPhieu;

            //    txtPhieuMH.Text = _tenphieumh;
            //}
            //else
            //{
            //    txtPhieuMH.Text = _tenphieumh;
            //}


            string url = $"{URL}NhaCC/GePMH?action=GetPhieuBG1&para1={mancc}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                //searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
                txtTienTe.EditValue = tbl.Rows[0]["DonViTienTe"];
            }
            CreateSearchlookupPhieuBG(mancc);
            //GeneratePOMua();
            if (_IsAdd == true || _IsMuaTheoVT == true)
            {
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
                gridControl3.DataSource = null;
                gridControl4.DataSource = null;
                //searchLookUpEditPBG.EditValue = null;
                //searchLookUpEditNCC.EditValue = null;
                _dtTable = null;
                _dtTableCP = null;
                _dtTableDot = null;
                _dtTableChiTiet = null;
                _dtTableHTTT = null;
            }

        }

        private void GeneratePOMua()
        {
            string urlcheck = $"{URL}NhaCC/GePMH?action=GETPhieuMuaHangCheck&para1=&para2=&para3=&para4=&para5=";
            string jsoncheck = Task.Run(async () => await _clientExtension.GetAsnyc(urlcheck)).Result;

            DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheck);

            DateTime now = DateTime.Now;
            int thang = now.Month;
            int nam = now.Year;

            int maxDot = 0;
            string currentThangNam = $"{thang:00}{nam}";
            string currentNam = nam.ToString();

            //if (tblcheck != null && tblcheck.Rows.Count > 0)
            //{
            //    foreach (DataRow r in tblcheck.Rows)
            //    {
            //        string pomua = r["POMua"]?.ToString();
            //        if (string.IsNullOrWhiteSpace(pomua)) continue;

            //        // Format: 1_122025
            //        var parts = pomua.Split('_');
            //        if (parts.Length != 2) continue;

            //        if (!int.TryParse(parts[0], out int dot)) continue;

            //        if (parts[1] == currentThangNam)
            //        {
            //            if (dot > maxDot)
            //                maxDot = dot;
            //        }
            //    }
            //}

            if (tblcheck != null && tblcheck.Rows.Count > 0)
            {
                foreach (DataRow r in tblcheck.Rows)
                {
                    string pomua = r["POMua"]?.ToString();
                    if (string.IsNullOrWhiteSpace(pomua)) continue;

                    // Format: x_MMyyyy  (vd: 1_22026)
                    var parts = pomua.Split('_');
                    if (parts.Length != 2) continue;

                    if (!int.TryParse(parts[0], out int dot)) continue;

                    string mmYYYY = parts[1];
                    if (mmYYYY.Length < 4) continue;

                    string yearInPO = mmYYYY.Substring(mmYYYY.Length - 4); // lấy yyyy

                    if (yearInPO == currentNam)
                    {
                        if (dot > maxDot)
                            maxDot = dot;
                    }
                }
            }


            int newDot = maxDot + 1;
            textEditPOMua.Text = $"{newDot}_{currentThangNam}";
        }

        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("ItemCode", typeof(string));
            tbl.Columns.Add("NPL", typeof(bool));
            tbl.Columns.Add("DonGia", typeof(float));
            tbl.Columns.Add("TongSL", typeof(float));
            tbl.Columns.Add("TongSLMuaThem", typeof(float));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("MoTa", typeof(string));
            tbl.Columns.Add("MaCLVT", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("TenCL", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("ChiPhiVanChuyen", typeof(string));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("PTVanChuyen", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("TGGiao", typeof(DateTime));
            tbl.Columns.Add("ChiPhiVT", typeof(decimal));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("SoNgayGHSom", typeof(int));
            tbl.Columns.Add("SoNgayGHTre", typeof(int));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            tbl.Columns.Add("ChiPhiSauCK", typeof(decimal));
            tbl.Columns.Add("MaThue_Gop", typeof(string));
            tbl.Columns.Add("MaChietKhau_Gop", typeof(string));
            tbl.Columns.Add("ThanhTienQD", typeof(decimal));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableload()
        {
            DataTable tbl = new DataTable("dtTable");

            // ===== KHÓA / ID =====
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("MaCLVT", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));

            // ===== VẬT TƯ / HIỂN THỊ =====
            tbl.Columns.Add("ItemCode", typeof(string));
            tbl.Columns.Add("MoTa", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("TenCL", typeof(string));
            tbl.Columns.Add("LoaiNPL", typeof(string));
            tbl.Columns.Add("NPL", typeof(bool));

            // ===== SỐ LƯỢNG / TIỀN =====
            tbl.Columns.Add("TongSL", typeof(decimal));
            tbl.Columns.Add("TongSLMuaThem", typeof(decimal));
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("Thue", typeof(decimal));
            tbl.Columns.Add("ChietKhau", typeof(decimal));
            tbl.Columns.Add("ChiPhiVT", typeof(decimal));
            tbl.Columns.Add("ChiPhiSauCK", typeof(decimal));
            tbl.Columns.Add("ChiPhiVanChuyen", typeof(decimal));
            tbl.Columns.Add("CPVanChuyen", typeof(decimal));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            tbl.Columns.Add("ThanhTienQD", typeof(decimal));

            // ===== THANH TOÁN =====
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("MaTienTe", typeof(string));
            tbl.Columns.Add("MaTienTePhieuMH", typeof(string));
            tbl.Columns.Add("TyGiaThanhToan", typeof(decimal));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("PTVanChuyen", typeof(string));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("POMua", typeof(string));

            // ===== NGÀY THÁNG =====
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            tbl.Columns.Add("NgayXacNhan", typeof(DateTime));
            tbl.Columns.Add("TGGiao", typeof(DateTime));

            // ===== TRẠNG THÁI =====
            tbl.Columns.Add("IsDuyet", typeof(bool));
            tbl.Columns.Add("IsXacNhan", typeof(bool));
            tbl.Columns.Add("SoNgayGHSom", typeof(int));
            tbl.Columns.Add("SoNgayGHTre", typeof(int));

            // ===== NGƯỜI THAO TÁC =====
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NguoiSua", typeof(string));

            // ===== KHÁC =====
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MaThue_Gop", typeof(string));
            tbl.Columns.Add("MaChietKhau_Gop", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            return tbl;
        }


        private DataTable CreateDatatableToSave()
        {
            DataTable tbl = new DataTable("dtSize");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("NPL", typeof(string));
            tbl.Columns.Add("SoLuongMuaThem", typeof(float));
            tbl.Columns.Add("DonGia", typeof(float));
            tbl.Columns.Add("ThanhTien", typeof(float));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("IsDuyet", typeof(bool));
            tbl.Columns.Add("MaCLVT", typeof(string));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("TrangThai", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("NgayXacNhan", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("PTVanChuyen", typeof(string));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("TGGiao", typeof(DateTime));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("CPVanChuyen", typeof(float));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("IsXacNhan", typeof(bool));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("ChiPhiVT", typeof(float));
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("TyGiaThanhToan", typeof(decimal));
            tbl.Columns.Add("MaTienTePhieuMH", typeof(string));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("Action", typeof(string));
            tbl.Columns.Add("MaKhachHang", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableToSaveDot()
        {
            DataTable tbl = new DataTable("_dtTableDot");
            tbl.Columns.Add("MaPhieu", typeof(string));
            tbl.Columns.Add("ThuTu", typeof(int));
            tbl.Columns.Add("MaPTVC", typeof(string));
            tbl.Columns.Add("ChiPhiVC", typeof(float));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaNhomVT", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MaKhoVT", typeof(string));
            tbl.Columns.Add("SoLuongVC", typeof(float));
            tbl.Columns.Add("NgayBatDau", typeof(DateTime));
            tbl.Columns.Add("NhanMin", typeof(int));
            tbl.Columns.Add("NhanMax", typeof(int));
            tbl.Columns.Add("NgayNhanDK", typeof(DateTime));
            tbl.Columns.Add("NgayNhanTT", typeof(DateTime));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("PTGiaoHang", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            return tbl;
        }
        private DataTable CreateDatatableCPSave()
        {
            DataTable tbl = new DataTable("dtTableCP");
            tbl.Columns.Add("MaPhieu", typeof(string));
            tbl.Columns.Add("MaChiPhi", typeof(int));
            tbl.Columns.Add("SoTien", typeof(float));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("DonViTienTe", typeof(string));
            return tbl;
        }
        private DataTable CreateDatatableCP()
        {
            DataTable tbl = new DataTable("dtTableCP");
            tbl.Columns.Add("MaChiPhi", typeof(int));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("ChiPhi", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("SoTien", typeof(float));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("ThanhTienCPPS", typeof(decimal));
            tbl.Columns.Add("DonViTienTe", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableDot()
        {
            DataTable tbl = new DataTable("_dtTableDot");
            tbl.Columns.Add("Thutu", typeof(int));
            tbl.Columns.Add("MaPTVC", typeof(string));
            tbl.Columns.Add("ChiPhiVC", typeof(decimal));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaNhomVT", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MaKhoVT", typeof(string));
            tbl.Columns.Add("SoLuongVC", typeof(decimal));
            tbl.Columns.Add("NgayBatDau", typeof(DateTime));
            tbl.Columns.Add("NhanMin", typeof(int));
            tbl.Columns.Add("NhanMax", typeof(int));
            tbl.Columns.Add("NgayNhanDK", typeof(DateTime));
            tbl.Columns.Add("NgayNhanTT", typeof(DateTime));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MoTa", typeof(string));
            tbl.Columns.Add("NPL", typeof(bool));
            tbl.Columns.Add("ItemCode", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("ThanhTienVC", typeof(decimal));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatablePhieuDH()
        {
            DataTable tbl = new DataTable("dtTableCP");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaLenhSX", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableHTTT()
        {
            DataTable tbl = new DataTable("dtTableHTTT");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("SoTien", typeof(decimal));
            tbl.Columns.Add("TGTra", typeof(DateTime));
            tbl.Columns.Add("DotTra", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("PhanTram", typeof(float));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("InvoiceTo", typeof(string));
            return tbl;
        }
        /*Payment*/
        private DataTable CreateDatatableHTTTSave()
        {
            DataTable tbl = new DataTable("dtTableHTTT");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("SoTien", typeof(decimal));
            tbl.Columns.Add("TGTra", typeof(DateTime));
            tbl.Columns.Add("DotTra", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("PhanTram", typeof(float));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("InvoiceTo", typeof(string));
            tbl.Columns.Add("Incoterm", typeof(string));
            tbl.Columns.Add("UserName", typeof(string));
            tbl.Columns.Add("CreaDate", typeof(DateTime));
            tbl.Columns.Add("Mac", typeof(string));
            tbl.Columns.Add("IPAdress", typeof(string));
            tbl.Columns.Add("MachineName", typeof(string));

            return tbl;
        }


        private DataTable CreateTable_E_Invoice()
        {
            DataTable tbl = new DataTable("tbl_E_Invoice");
            tbl.Columns.Add("ID", typeof(long));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaKhachHang", typeof(string));
            tbl.Columns.Add("SoHoaDon", typeof(string));
            tbl.Columns.Add("SoToKhai", typeof(string));
            tbl.Columns.Add("SoHopDong", typeof(string));
            tbl.Columns.Add("NgayHoaDon", typeof(string));
            tbl.Columns.Add("NgayMoToKhai", typeof(string));
            tbl.Columns.Add("NgayKyHD", typeof(string));
            tbl.Columns.Add("MaSoThue", typeof(string));
            tbl.Columns.Add("SoVanDon", typeof(string));
            tbl.Columns.Add("E_Way_Bill", typeof(string));
            tbl.Columns.Add("NoiGui", typeof(string));
            tbl.Columns.Add("NgayGui", typeof(string));
            tbl.Columns.Add("Cang", typeof(string));
            tbl.Columns.Add("Tau", typeof(string));
            tbl.Columns.Add("UserName", typeof(string));
            tbl.Columns.Add("CreaDate", typeof(string));
            tbl.Columns.Add("Mac", typeof(string));
            tbl.Columns.Add("IPAdress", typeof(string));
            tbl.Columns.Add("MachineName", typeof(string));

            return tbl;
        }

        private void CreateSearchlookupPhieuBG(string mancc)
        {
            string url = $"{URL}NhaCC/GePMH?action=GetPhieuBG&para1={mancc}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditPBG.Properties.DataSource = tbl;
            searchLookUpEditPBG.Properties.ValueMember = "MaPhieuBG";
            searchLookUpEditPBG.Properties.DisplayMember = "TenPhieu";
            //if (_IsEdit == false) 
            //{
            //    if (tbl != null && tbl.Rows.Count > 0)
            //    {
            //        searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
            //        txtTienTe.EditValue = tbl.Rows[0]["DonViTienTe"];
            //    }
            //}
            if (_IsAdd == true || _IsMuaTheoVT == true)
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
                    //txtTienTe.EditValue = tbl.Rows[0]["DonViTienTe"];
                }
            }


        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "NPL"
                               && info.EditValue != null);

            int groupIndex = gridView1.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn3)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            if (info.Column == gridColumn80)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            e.Handled = false;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataTable dt = gridControl1.DataSource as DataTable;
            DataTable dt1 = gridControl2.DataSource as DataTable;
            DataTable dt2 = gridControl3.DataSource as DataTable;
            DataTable dt3 = gridControl4.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                XtraMessageBox.Show(
                    "Bạn chưa thêm vật tư cho phiếu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (KiemTraDotSoLuongBang0(dt2))
            {
                return;
            }

            if (string.IsNullOrEmpty(SeachLookupKhachHang.EditValue?.ToString()))
            {
                XtraMessageBox.Show(
                    "Vui lòng chọn khách hàng.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            //if (KiemTraDotSoTienbang0(dt1))
            //{
            //    return;
            //}
            if (searchLookUpEditNCC.EditValue == null || searchLookUpEditNCC.EditValue == "")
            {
                return;
            }
            //if (searchLookUpEditPBG.EditValue == null || searchLookUpEditPBG.EditValue == "")
            //{
            //    return;
            //}

            List<string> missing = new List<string>();

            if (dt1 == null || dt1.Rows.Count == 0) missing.Add("chi phí");
            if (string.IsNullOrEmpty(SearchLookupPayment.EditValue?.ToString())) missing.Add("hình thức thanh toán");

            if (missing.Count > 0)
            {
                DialogResult result = XtraMessageBox.Show(
                    $"Bạn chưa thêm {string.Join(" và ", missing)} cho phiếu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                //if (result != DialogResult.OK)
                //{
                //    return;
                //}
            }

            if (_IsEdit == true)
            {
                Update(dt);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    SaveDataCP(dt1);
                }
                if (dt2 != null && dt2.Rows.Count > 0)
                {

                    SaveDataDot(dt2);
                }
                //if (dt3 != null && dt3.Rows.Count > 0)
                //{

                //}
                //SaveDataHTTT();
            }
            else
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    SaveData(dt);
                }
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    SaveDataCP(dt1);
                }
                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    SaveDataDot(dt2);
                }
                //if (dt3 != null && dt3.Rows.Count > 0)
                //{
                //    SaveDataHTTT(dt3);
                //}
               

            }

            SaveDataHTTT();
            Save_E_Invoice();

            gridControl1.DataSource = null;
            searchLookUpEditPBG.EditValue = null;
            searchLookUpEditNCC.EditValue = null;
            _dtTable = null;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_IsEdit == true)
            {
                searchLookUpEditNCC.EditValue = _mancc.ToString();
                //searchLookUpEditPBG.EditValue = _maphieubg.ToString();
                LoadData();
                LoadDataChiPhi();
                LoadDataDotGH();
                LoadDataHTTT();

                UpdateGridView5Summary();
                UpdateGridView5Values();
            }
            else
            {
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
                gridControl3.DataSource = null;
                gridControl4.DataSource = null;
                //searchLookUpEditPBG.EditValue = null;
                //searchLookUpEditNCC.EditValue = null;
                _dtTable = null;
                _dtTableCP = null;
                _dtTableDot = null;
                _dtTableChiTiet = null;
                _dtTableHTTT = null;

            }

        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            if (e.Column.FieldName == "TongSLMuaThem" || e.Column.FieldName == "DonGia" || e.Column.FieldName == "Thue" || e.Column.FieldName == "ChietKhau")
            {
                decimal tygiavnd = 1;
                decimal soLuong = GetDecimalValue(view, e.RowHandle, "TongSLMuaThem");
                decimal donGia = GetDecimalValue(view, e.RowHandle, "DonGia");
                decimal thue = GetThuePercent(view, e.RowHandle, "Thue");
                decimal chietkhau = GetThuePercent(view, e.RowHandle, "ChietKhau");
                string matiente = Convert.ToString(
                    view.GetRowCellValue(e.RowHandle, "TienTeID")
                );
                string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                _isLoadingTyGia = true;
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    tygiavnd = GetDecimalFromObject(tbl.Rows[0]["TyGia"]);

                    //_tienteID = tbl.Rows[0]["MaTienTe"].ToString();
                }
                decimal thanhtienchietkhau = soLuong * donGia - (chietkhau * (soLuong * donGia));
                decimal thanhTien = thanhtienchietkhau + (thue * thanhtienchietkhau);
                decimal tygia = Convert.ToDecimal(spinEditTyGia.EditValue);
                decimal chiphivt = soLuong * donGia;
                decimal thanhTienvnd = thanhTien * tygiavnd;
                decimal thanhtienqd = thanhTienvnd / tygia;
                view.SetRowCellValue(e.RowHandle, "ChiPhiSauCK", Math.Round(thanhTien, 2));
                view.SetRowCellValue(e.RowHandle, "ChiPhiVT", Math.Round(chiphivt, 2));
                view.SetRowCellValue(e.RowHandle, "ThanhTien", Math.Round(thanhTien, 2));
                view.SetRowCellValue(e.RowHandle, "ThanhTienVND", Math.Round(thanhTienvnd, 2));
                view.SetRowCellValue(e.RowHandle, "ThanhTienQD", Math.Round(thanhtienqd, 2));
                //if (txtTienTe.Text !="VND") 
                //{

                //}
            }
            if (e.Column.FieldName == "NgayGiaoHangYC")
            {
                gridView1.PostEditor();
                gridView1.UpdateCurrentRow();
            }

            UpdateGridView5Summary();
            UpdateGridView5Values();
        }

        private void gridView1_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {

            if (e.Column == null)
                return;

            e.Appearance.BackColor = Color.LightYellow; // Màu nền

            e.Appearance.ForeColor = Color.Red;

            e.Appearance.Font = new Font("Tahoma", 8F, FontStyle.Bold);
        }

        public void SaveData(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            string url1 = $"{URL}NhaCC/GePMH?action=GETALLPHIEUMHCount&para1=&para2=&para3=&para4=&para5=";
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

            DataTable tblDG = CreateDatatableToSave();

            int maxPhieu = 0;

            if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("MaPhieuMH"))
            {
                var validValues = _tbldv.AsEnumerable()
                    .Select(r => r["MaPhieuMH"]?.ToString())
                    .Where(s => !string.IsNullOrWhiteSpace(s) && s.StartsWith("PHIEU_"))
                    .Select(s =>
                    {
                        string numberPart = s.Replace("PHIEU_", "").Trim();
                        int num;
                        return int.TryParse(numberPart, out num) ? num : 0;
                    });

                if (validValues.Any())
                    maxPhieu = validValues.Max();
            }

            int newPhieu = maxPhieu + 1;

            int maxDot = 0;

            if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("Dot"))
            {
                var validValues = _tbldv.AsEnumerable()
                    .Select(r => r["Dot"]?.ToString())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s =>
                    {
                        int num;
                        return int.TryParse(s, out num) ? num : 0;
                    });

                if (validValues.Any())
                    maxDot = validValues.Max();
            }

            int newDot = maxDot + 1;

            foreach (DataRow dr in dt.Rows)
            {
               // _maphieumh = "PHIEU_" + newPhieu;
                //_tenphieumh = "PHIẾU MH|" +searchLookUpEditNCC.Text.ToString()+"|"+ searchLookUpEditPBG.Text.ToString()+ "|"+newPhieu;
                //_tenphieumh = "PHIẾU MH|" + searchLookUpEditNCC.Text.ToString() + "|" + newPhieu;
                DataRow newRow = tblDG.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["TenPhieu"] = _tenphieumh;
                newRow["MaNCC"] = searchLookUpEditNCC.EditValue?.ToString();
                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                newRow["MaVT"] = dr["ItemCode"]?.ToString();
                newRow["ChiTiet"] = dr["MoTa"];
                newRow["NPL"] = dr["NPL"];
                newRow["SoLuongMuaThem"] = dr["TongSLMuaThem"];
                newRow["DonGia"] = dr["DonGia"];
                newRow["ThanhTien"] = dr["ThanhTien"];
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                newRow["MauVTID"] = dr["MauVTID"];
                newRow["KhoVaiID"] = dr["KhoVaiID"];
                newRow["MaDVVT"] = dr["MaDVVT"];
                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["IsDuyet"] = 0;
                newRow["MaCLVT"] = dr["MaCLVT"];
                newRow["TienTeID"] = dr["TienTeID"];
                newRow["NgaySua"] = DBNull.Value;
                newRow["NguoiSua"] = "";
                newRow["NgayDuKienHV"] = dr["NgayDuKienHV"] == null ? "" : dr["NgayDuKienHV"];
                newRow["NgayXacNhan"] = DBNull.Value;
                newRow["GhiChu"] = dr["GhiChu"];
                newRow["PTVanChuyen"] = dr["PTVanChuyen"];
                newRow["PTThanhToan"] = dr["PTThanhToan"];
                newRow["TGGiao"] = dr["TGGiao"] == null ? DBNull.Value : dr["TGGiao"];
                newRow["Thue"] = dr["Thue"] == DBNull.Value ? 0 : dr["Thue"];
                newRow["CPVanChuyen"] = dr["ChiPhiVanChuyen"];
                newRow["ChiPhiKhac"] = "";
                newRow["Dot"] = newDot;
                newRow["IsXacNhan"] = 0;
                newRow["NgayHieuLuc"] = dr["NgayHieuLuc"] == null ? DBNull.Value : dr["NgayHieuLuc"];
                newRow["ChiPhiVT"] = dr["ChiPhiVT"];
                newRow["POMua"] = textEditPOMua.Text;
                newRow["TyGiaThanhToan"] = 0;
                newRow["MaTienTePhieuMH"] = txtTienTe.EditValue.ToString();
                newRow["ChietKhau"] = dr["ChietKhau"] == DBNull.Value ? 0 : dr["ChietKhau"];
                newRow["TTChietKhau"] = dr["TTChietKhau"] == null ? "" : dr["TTChietKhau"];
                newRow["NgayGiaoHangYC"] = dr["NgayGiaoHangYC"] == null ? DBNull.Value : dr["NgayGiaoHangYC"];
                newRow["Action"] = "NewPhieu";
                newRow["MaKhachHang"] = SeachLookupKhachHang.EditValue;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/Pospmh");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (string.IsNullOrEmpty(mss))
            {
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _maphieumh = mss;
                clsWaitForm.ShowSuccessForm(this, 3000);
            }
               
            
              
            if (_dtTableChiTiet != null && _dtTableChiTiet.Rows.Count > 0)
            {
                foreach (DataRow dr in _dtTableChiTiet.Rows)
                {
                    dr["MaPhieuMH"] = _maphieumh;
                }
                string url2 = string.Format("{0}?", URL + "NhaCC/Pospmhdh");
                string mss1 = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url2, _dtTableChiTiet);
                }).Result;

                if (mss1.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        public void SaveDataCP(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            DataTable tblDG = CreateDatatableCPSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["MaPhieu"] = _maphieumh;
                newRow["MaChiPhi"] = dr["MaChiPhi"]?.ToString();
                newRow["SoTien"] = dr["SoTien"];
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["Thue"] = dr["Thue"];
                newRow["DonViTienTe"] = dr["DonViTienTe"]?.ToString();
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/PostCPPhieuMH");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);
        }

        public void SaveDataDot(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            DataTable tblDG = CreateDatatableToSaveDot();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["MaPhieu"] = _maphieumh;
                newRow["ThuTu"] = Convert.ToInt32(dr["ThuTu"]);
                newRow["MaPTVC"] = "";
                newRow["ChiPhiVC"] = dr.IsNull("ChiPhiVC")
                    ? 0m
                    : Convert.ToDecimal(dr["ChiPhiVC"]);
                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                newRow["MaNhomVT"] = dr["MaNhomVT"]?.ToString();
                newRow["MaMauVT"] = dr["MaMauVT"]?.ToString();
                newRow["MaKhoVT"] = dr["MaKhoVT"]?.ToString();
                newRow["SoLuongVC"] = dr.IsNull("SoLuongVC")
                    ? 0m
                    : dr["SoLuongVC"];
                newRow["NgayBatDau"] = dr.IsNull("NgayBatDau") ? DBNull.Value : dr["NgayBatDau"];
                newRow["NhanMin"] = dr.IsNull("NhanMin")
                    ? 0
                    : Convert.ToInt32(dr["NhanMin"]);
                newRow["NhanMax"] = dr.IsNull("NhanMax")
                    ? 0
                    : Convert.ToInt32(dr["NhanMax"]);
                newRow["NgayNhanDK"] = dr.IsNull("NgayNhanDK") ? DBNull.Value : dr["NgayNhanDK"];
                newRow["NgayNhanTT"] = dr.IsNull("NgayNhanTT") ? DBNull.Value : dr["NgayNhanTT"];
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["Thue"] = dr.IsNull("Thue")
                    ? 0d
                    : Convert.ToDouble(dr["Thue"]);
                newRow["PTGiaoHang"] = "";
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/PostDotGHPMH");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            if (searchLookUpEditNCC.EditValue == null || searchLookUpEditNCC.EditValue == "")
            {
                return;
            }
            if (searchLookUpEditPBG.EditValue == null || searchLookUpEditPBG.EditValue == "")
            {
                return;
            }
            foreach (DataRow row in dt.Rows)
            {
                row["IsXacNhan"] = 1;
                row["NgayXacNhan"] = DateTime.Now.ToString("yyyy-MM-dd");
                //row["NgayDuKienHV"] = Convert.ToDateTime(row["TGGiao"]);
                if (row["SoNgayGHSom"] == DBNull.Value || row["TGGiao"] == null || string.IsNullOrWhiteSpace(row["TGGiao"].ToString()))
                {
                    row["NgayDuKienHV"] = "";
                }
                else
                {
                    DateTime tg;
                    if (DateTime.TryParse(row["TGGiao"].ToString(), out tg))
                        row["NgayDuKienHV"] = tg;
                    else
                        row["NgayDuKienHV"] = DBNull.Value;
                }
            }
            if (_IsEdit == true)
            {
                UpdateXacNhan(dt);
            }
            this.DialogResult = DialogResult.OK;
        }

        public void Update(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            DataTable tblDG = CreateDatatableToSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["TenPhieu"] = _tenphieumh;
                newRow["MaNCC"] = searchLookUpEditNCC.EditValue?.ToString();
                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                newRow["MaVT"] = dr["ItemCode"]?.ToString();
                newRow["ChiTiet"] = dr["MoTa"];
                newRow["NPL"] = dr["NPL"];
                newRow["SoLuongMuaThem"] = dr["TongSLMuaThem"];
                newRow["DonGia"] = dr["DonGia"];
                newRow["ThanhTien"] = dr["ThanhTien"];
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                newRow["MauVTID"] = dr["MauVTID"];
                newRow["KhoVaiID"] = dr["KhoVaiID"];
                newRow["MaDVVT"] = dr["MaDVVT"];
                newRow["NgayTao"] =
                    dr.IsNull("NgayTao") ? DateTime.Now.ToString("yyyy-MM-dd") : dr.Field<DateTime>("NgayTao").ToString("yyyy-MM-dd");
                newRow["NguoiTao"] = string.IsNullOrEmpty(dr["NguoiTao"].ToString()) ? GlobleData.UserName.ToString() : dr["NguoiTao"];
                bool isDuyetEmpty = dr.IsNull("IsDuyet")
                                    || string.IsNullOrWhiteSpace(dr["IsDuyet"].ToString());

                if (tblDG.Rows.Count > 0 && isDuyetEmpty)
                    newRow["IsDuyet"] = tblDG.Rows[tblDG.Rows.Count - 1]["IsDuyet"];
                else
                    newRow["IsDuyet"] = dr["IsDuyet"];

                newRow["MaCLVT"] = dr["MaCLVT"];
                newRow["TienTeID"] = dr["TienTeID"];
                newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["NguoiSua"] = GlobleData.UserName.ToString();
                newRow["TrangThai"] = "";
                newRow["NgayDuKienHV"] = dr["NgayDuKienHV"] == null ? "" : dr["NgayDuKienHV"];
                newRow["NgayXacNhan"] = DBNull.Value;
                newRow["GhiChu"] = dr["GhiChu"];
                newRow["PTVanChuyen"] = dr["PTVanChuyen"];
                newRow["PTThanhToan"] = dr["PTThanhToan"];
                newRow["Thue"] = dr["Thue"];
                newRow["CPVanChuyen"] = dr["CPVanChuyen"];
                newRow["ChiPhiKhac"] = "";
                newRow["Dot"] = dr["Dot"];
                bool isXNEmpty = dr.IsNull("IsXacNhan")
                                 || string.IsNullOrWhiteSpace(dr["IsXacNhan"].ToString());

                if (tblDG.Rows.Count > 0 && isXNEmpty)
                    newRow["IsXacNhan"] = tblDG.Rows[tblDG.Rows.Count - 1]["IsXacNhan"];
                else
                    newRow["IsXacNhan"] = dr["IsXacNhan"];
                newRow["NgayHieuLuc"] = dr["NgayHieuLuc"] == null ? DBNull.Value : dr["NgayHieuLuc"];
                newRow["ChiPhiVT"] = dr["ChiPhiVT"];
                newRow["POMua"] = textEditPOMua.Text;
                newRow["TyGiaThanhToan"] = dr["TyGiaThanhToan"];
                newRow["MaTienTePhieuMH"] = txtTienTe.EditValue;
                newRow["ChietKhau"] = dr["ChietKhau"];
                newRow["TTChietKhau"] = dr["TTChietKhau"] == null ? "" : dr["TTChietKhau"];
                newRow["NgayGiaoHangYC"] = dr["NgayGiaoHangYC"] == null ? DBNull.Value : dr["NgayGiaoHangYC"];
                newRow["Action"] = "UpdatePhieu";
                newRow["MaKhachHang"] = SeachLookupKhachHang.EditValue;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/Pospmh");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (string.IsNullOrEmpty(mss))
            {
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _maphieumh = mss;
                clsWaitForm.ShowSuccessForm(this, 3000);
            }

        }

        public void UpdateXacNhan(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            DataTable tblDG = CreateDatatableToSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["TenPhieu"] = _tenphieumh;
                newRow["MaNCC"] = searchLookUpEditNCC.EditValue?.ToString();
                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                newRow["MaVT"] = dr["ItemCode"]?.ToString();
                newRow["ChiTiet"] = dr["MoTa"];
                newRow["NPL"] = dr["NPL"];
                newRow["SoLuongMuaThem"] = dr["TongSLMuaThem"];
                newRow["DonGia"] = dr["DonGia"];
                newRow["ThanhTien"] = dr["ThanhTien"];
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                newRow["MauVTID"] = dr["MauVTID"];
                newRow["KhoVaiID"] = dr["KhoVaiID"];
                newRow["MaDVVT"] = dr["MaDVVT"];
                newRow["NgayTao"] = dr["NgayTao"];
                newRow["NguoiTao"] = dr["NguoiTao"];
                newRow["IsDuyet"] = dr["IsDuyet"];
                newRow["MaCLVT"] = dr["MaCLVT"];
                newRow["TienTeID"] = "";
                newRow["NgaySua"] = dr["NgaySua"];
                newRow["NguoiSua"] = dr["NguoiSua"];
                newRow["TrangThai"] = "";
                newRow["NgayDuKienHV"] = dr["NgayDuKienHV"] == null ? "" : dr["NgayDuKienHV"];
                newRow["NgayXacNhan"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["GhiChu"] = dr["GhiChu"];
                newRow["PTVanChuyen"] = dr["PTVanChuyen"];
                newRow["PTThanhToan"] = dr["PTThanhToan"];
                newRow["TGGiao"] = dr["TGGiao"] == null ? DBNull.Value : dr["TGGiao"];
                newRow["Thue"] = dr["Thue"];
                newRow["CPVanChuyen"] = dr["CPVanChuyen"];
                newRow["ChiPhiKhac"] = "";
                newRow["Dot"] = dr["Dot"];
                newRow["IsXacNhan"] = dr["IsXacNhan"];
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/Pospmh");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;



            if (mss.ToLower() != "true" || string.IsNullOrEmpty(mss))
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                _maphieumh = mss;
                clsWaitForm.ShowSuccessForm(this, 3000);

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridControl1.MainView as GridView;
            if (view == null) return;

            int[] selectedRows = view.GetSelectedRows();
            if (selectedRows == null || selectedRows.Length == 0) return;

            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null) return;

            // Xóa từ dưới lên để không lệch index
            for (int i = selectedRows.Length - 1; i >= 0; i--)
            {
                DataRow row = view.GetDataRow(selectedRows[i]);
                if (row == null) continue;

                // 1️⃣ Xóa server nếu đang edit
                if (_IsEdit)
                {
                    try
                    {
                        string url = string.Format(
                            "{0}?para1={1}&para2={2}&para3={3}&para4={4}&para5={5}",
                            URL + "NhaCC/DeleteRowPMH",
                            _maphieumh,
                            row["MaVTID"],
                            row["MauVTID"],
                            row["KhoVaiID"],
                            row["MaCLVT"]
                        );

                        Task.Run(async () => await _clientExtension.DeletedAsync(url)).Wait();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(
                            "Lỗi xóa dữ liệu trên server:\n" + ex.Message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                }

                dt.Rows.Remove(row);
            }

            view.RefreshData();

      
            UpdateGridView5Summary();
            UpdateGridView5Values();

            
            int soChungLoai = 0;
            if (dt.Rows.Count > 0)
            {
                soChungLoai = dt.AsEnumerable()
                    .Where(r => r.RowState != DataRowState.Deleted)
                    .Select(r =>
                        string.Join("|",
                            r["MaVTID"],
                            r["MauVTID"],
                            r["KhoVaiID"],
                            r["MaDVVT"]
                        )
                    )
                    .Distinct()
                    .Count();
            }

            spinEditSoLoaiVT.EditValue = soChungLoai;

          
            if (dt.Rows.Count == 0)
            {
                _maphieubg = string.Empty;
            }

        }

        private decimal GetDecimalValue(GridView view, int rowHandle, string fieldName)
        {
            var val = view.GetRowCellValue(rowHandle, fieldName);
            if (val is decimal d) return d;
            if (val is double db) return (decimal)db;
            if (val is int i) return i;
            decimal.TryParse(val?.ToString(), out decimal result);
            return result;
        }

        private void simpleButton2_Click_1(object sender, EventArgs e)
        {

        }

        private decimal GetThuePercent(GridView view, int rowHandle, string fieldName)
        {
            var val = view.GetRowCellValue(rowHandle, fieldName);
            return GetThuePercentFromValue(val);
        }


        private decimal GetThuePercentFromValue(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;

            string str = value.ToString().Replace("%", "").Trim();
            if (decimal.TryParse(str, out decimal percent))
                return percent / 100m;

            return 0m;
        }

        private void gridView3_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "NPL"
                               && info.EditValue != null);


            int groupIndex = gridView3.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn43)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            if (info.Column == gridColumn81)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            e.Handled = false;
        }



        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            //string mancc = searchLookUpEditNCC.EditValue.ToString();
            //string maphieubg = searchLookUpEditPBG.EditValue.ToString();
            //CreateSearchlookupVatTu(mancc, maphieubg);
            //if (_IsAdd == true || (_IsMuaTheoVT == true && _IsView == false && _IsEdit == false))
            //{
            //    string url1 = $"{URL}NhaCC/GePMH?action=GETALLPHIEUMHCount&para1=&para2=&para3=&para4=&para5=";
            //    string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            //    DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

            //    DataTable tblDG = CreateDatatableToSave();

            //    int maxPhieu = 0;

            //    if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("MaPhieuMH"))
            //    {
            //        var validValues = _tbldv.AsEnumerable()
            //            .Select(r => r["MaPhieuMH"]?.ToString())
            //            .Where(s => !string.IsNullOrWhiteSpace(s) && s.StartsWith("PHIEU_"))
            //            .Select(s =>
            //            {
            //                string numberPart = s.Replace("PHIEU_", "").Trim();
            //                int num;
            //                return int.TryParse(numberPart, out num) ? num : 0;
            //            });

            //        if (validValues.Any())
            //            maxPhieu = validValues.Max();
            //    }

            //    int newPhieu = maxPhieu + 1;

            //    int maxDot = 0;

            //    if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("Dot"))
            //    {
            //        var validValues = _tbldv.AsEnumerable()
            //            .Select(r => r["Dot"]?.ToString())
            //            .Where(s => !string.IsNullOrWhiteSpace(s))
            //            .Select(s =>
            //            {
            //                int num;
            //                return int.TryParse(s, out num) ? num : 0;
            //            });

            //        if (validValues.Any())
            //            maxDot = validValues.Max();
            //    }

            //    int newDot = maxDot + 1;

            //    _tenphieumh = "PHIẾU MH|" + newPhieu;

            //    txtPhieuMH.Text = _tenphieumh;
            //}
            //else
            //{
            //    txtPhieuMH.Text = _tenphieumh;
            //}

            string mancc = searchLookUpEditNCC.EditValue?.ToString();
            string maphieubg = searchLookUpEditPBG.EditValue?.ToString();

            if (string.IsNullOrEmpty(mancc) || string.IsNullOrEmpty(maphieubg))
                return;
            string url = $"{URL}NhaCC/GePMH?action=GetPhieuBG1&para1={mancc}&para2={maphieubg}&para3=&para4=&para5=";
            //string url = $"{URL}NhaCC/GePMH?action=GetPhieuBG1&para1={mancc}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
                txtTienTe.EditValue = tbl.Rows[0]["DonViTienTe"];
            }


            if (_dtTable == null || _dtTable.Rows.Count == 0) return;

            //UpdateGiaTheoPhieuBG(mancc, maphieubg);

            gridControl1.DataSource = null;
            gridControl2.DataSource = null;
            gridControl3.DataSource = null;
            gridControl4.DataSource = null;
            //searchLookUpEditPBG.EditValue = null;
            //searchLookUpEditNCC.EditValue = null;
            _dtTable = null;
            _dtTableCP = null;
            _dtTableDot = null;
            _dtTableChiTiet = null;
            _dtTableHTTT = null;

            UpdateGridView5Summary();
            UpdateGridView5Values();
            spinEditTongCPVT.EditValue = 0;
            spinEditTongCPPS.EditValue = 0;
            spinEditTongCPVC.EditValue = 0;
            spinEditTongCPVND.EditValue = 0;
            spinEditTongCP.EditValue = 0;
        }

        private void UpdateGiaTheoPhieuBG(string mancc, string maphieubg)
        {
            if (_IsAdd == true)
            {
                string urlVT = $"{URL}NhaCC/GePMH?action=GetVatTu&para1={mancc}&para2={maphieubg}&para3=&para4=&para5=";
                string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;

                string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={mancc}&para2=&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (jsonVT == "[]") return;

                DataTable tblGiaMoi = JsonConvert.DeserializeObject<DataTable>(jsonVT);

                foreach (DataRow row in _dtTable.Rows)
                {
                    string maVT = row["MaVTID"]?.ToString();
                    string mauVT = row["MauVTID"]?.ToString();
                    string khoVai = row["KhoVaiID"]?.ToString();
                    string dvvt = row["MaDVVT"]?.ToString();
                    string clvt = row["MaCLVT"]?.ToString();

                    var giaMoi = tblGiaMoi.AsEnumerable().FirstOrDefault(x =>
                        x["MaVTID"]?.ToString() == maVT &&
                        x["MauVTID"]?.ToString() == mauVT &&
                        x["KhoVaiID"]?.ToString() == khoVai &&
                        x["MaDVVT"]?.ToString() == dvvt &&
                        x["MaCLVT"]?.ToString() == clvt
                    );

                    if (giaMoi == null) continue;

                    decimal thue = giaMoi["Thue"] == null ? 0 : GetThuePercentFromValue(Convert.ToDecimal(giaMoi["Thue"]));
                    decimal chietkhau = giaMoi["ChietKhau"] == DBNull.Value ? 0 : GetThuePercentFromValue(Convert.ToDecimal(giaMoi["ChietKhau"]));
                    decimal chiphi = Convert.ToDecimal(giaMoi["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]);
                    decimal chiphichietkhau = Convert.ToDecimal(giaMoi["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) - (chietkhau * (Convert.ToDecimal(giaMoi["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"])));
                    decimal thanhtien = chiphichietkhau + (thue * chiphichietkhau);
                    //decimal thanhTienvnd = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) * (1 + thue - chietkhau);
                    decimal giaqd = tbl.Rows[0]["Gia"] == null ? 0 : Convert.ToDecimal(tbl.Rows[0]["Gia"]);

                    row["DonGia"] = giaMoi["DonGia"];
                    row["Thue"] = giaMoi["Thue"];
                    row["ChietKhau"] = giaMoi["ChietKhau"];
                    row["ChiPhiVT"] = chiphi;
                    string matiente = row["MaTienTe"]?.ToString().Trim();
                    row["ThanhTien"] = thanhtien;
                    if (row["MaTienTe"]?.ToString().Trim() == "VND")
                    {

                        row["ThanhTienVND"] = thanhtien;
                    }
                    else
                    {
                        row["ThanhTienVND"] = thanhtien * giaqd;
                    }
                    row["ChiPhiSauCK"] = chiphichietkhau;
                    row["MaThue_Gop"] = giaMoi["MaThue_Gop"];
                    row["MaChietKhau_Gop"] = giaMoi["MaChietKhau_Gop"];
                }
            }
            if (_IsMuaTheoVT == true)
            {
                string urlVT = $"{URL}NhaCC/GePMH?action=GETGiaMuaTheoVT&para1={mancc}&para2={maphieubg}&para3=&para4=&para5=";
                string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;

                string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={mancc}&para2=&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (jsonVT == "[]") return;

                DataTable tblGiaMoi = JsonConvert.DeserializeObject<DataTable>(jsonVT);

                foreach (DataRow row in _dtTable.Rows)
                {
                    string maVT = row["MaVTID"]?.ToString();
                    string mauVT = row["MauVTID"]?.ToString();
                    string khoVai = row["KhoVaiID"]?.ToString();
                    string dvvt = row["MaDVVT"]?.ToString();
                    string clvt = row["MaCLVT"]?.ToString();

                    var giaMoi = tblGiaMoi.AsEnumerable().FirstOrDefault(x =>
                        x["MaVTID"]?.ToString() == maVT &&
                        x["MauVTID"]?.ToString() == mauVT &&
                        x["KhoVaiID"]?.ToString() == khoVai &&
                        x["MaDVVT"]?.ToString() == dvvt &&
                        x["MaCLVT"]?.ToString() == clvt
                    );

                    if (giaMoi == null) continue;

                    decimal thue = giaMoi["Thue"] == null ? 0 : GetThuePercentFromValue(Convert.ToDecimal(giaMoi["Thue"]));
                    decimal chietkhau = giaMoi["ChietKhau"] == DBNull.Value ? 0 : GetThuePercentFromValue(Convert.ToDecimal(giaMoi["ChietKhau"]));
                    decimal chiphi = Convert.ToDecimal(giaMoi["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]);
                    decimal chiphichietkhau = Convert.ToDecimal(giaMoi["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) - (chietkhau * (Convert.ToDecimal(giaMoi["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"])));
                    decimal thanhtien = chiphichietkhau + (thue * chiphichietkhau);
                    //decimal thanhTienvnd = Convert.ToDecimal(row["DonGia"]) * Convert.ToDecimal(row["TongSLMuaThem"]) * (1 + thue - chietkhau);
                    decimal giaqd = tbl.Rows[0]["Gia"] == null ? 0 : Convert.ToDecimal(tbl.Rows[0]["Gia"]);

                    row["DonGia"] = giaMoi["DonGia"];
                    row["Thue"] = giaMoi["Thue"];
                    row["ChietKhau"] = giaMoi["ChietKhau"];
                    row["ChiPhiVT"] = chiphi;
                    row["ThanhTien"] = thanhtien;
                    string matiente = row["MaTienTe"]?.ToString().Trim();
                    if (row["MaTienTe"]?.ToString().Trim() == "VND")
                    {

                        row["ThanhTienVND"] = thanhtien;
                    }
                    else
                    {
                        row["ThanhTienVND"] = thanhtien * giaqd;
                    }
                    row["ChiPhiSauCK"] = chiphichietkhau;
                    row["MaThue_Gop"] = giaMoi["MaThue_Gop"];
                    row["MaChietKhau_Gop"] = giaMoi["MaChietKhau_Gop"];
                }
            }

            if (_IsEdit == false)
            {
                LoadDataCP();
                //themdot();
            }

            UpdateGridView5Summary();
            UpdateGridView5Values();

            gridView1.RefreshData();
        }



        private void SetupNumericColumns()
        {
            // Tạo 1 repository dùng chung
            RepositoryItemTextEdit riNumeric = new RepositoryItemTextEdit();
            riNumeric.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            riNumeric.Mask.EditMask = "n3";
            riNumeric.Mask.UseMaskAsDisplayFormat = true;

            // Chặn số âm
            BlockNegative(riNumeric);

            // Những cột cần format numeric
            GridColumn[] numericColumns = new GridColumn[]
            {
                gridColumn6, gridColumn4, gridColumn27, gridColumn50,
                gridColumn32, gridColumn29, gridColumn38, gridColumn53,
                gridColumn57, gridColumn7, gridColumn62,gridColumn51
            };

            // Gán 1 repository cho tất cả cột
            foreach (GridColumn col in numericColumns)
                col.ColumnEdit = riNumeric;
        }


        private void BlockNegative(RepositoryItemTextEdit ri)
        {
            ri.EditValueChanging += (s, e) =>
            {
                if (e.NewValue == null) return;

                decimal val;
                if (decimal.TryParse(e.NewValue.ToString(), out val))
                {
                    if (val < 0)
                    {
                        e.Cancel = true;
                    }
                }
            };
        }


        private void btnThemCP_Click(object sender, EventArgs e)
        {
            frmChiPhi frm = new frmChiPhi(true);
            if (_dtTableCP != null && _dtTableCP.Rows.Count > 0)
            {
                frm.ListExistingCP = _dtTableCP.Copy();
            }
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataTable selectedTable = frm._tbl;
                if (selectedTable == null || selectedTable.Rows.Count == 0)
                    return;
                if (_dtTableCP == null || _dtTableCP.Rows.Count == 0)
                {
                    _dtTableCP = CreateDatatableCP();
                }
                foreach (DataRow row in selectedTable.Rows)
                {
                    DataRow newRow = _dtTableCP.NewRow();
                    newRow["MaChiPhi"] = row["MaChiPhi"];
                    newRow["MaNhom"] = row["MaNhom"];
                    newRow["ChiPhi"] = row["ChiPhi"];
                    newRow["GhiChu"] = row["GhiChu"];
                    newRow["SoTien"] = 0;
                    newRow["Thue"] = 0;
                    newRow["ThanhTienCPPS"] = 0;
                    newRow["DonViTienTe"] = txtTienTe.EditValue;
                    _dtTableCP.Rows.Add(newRow);
                }
                CreateSearchlookupcolTienTe();
                gridControl2.DataSource = _dtTableCP;
            }
            //if (_coDotThanhToan) 
            //{
            //    if (_dtTableDot != null && _dtTableDot.Rows.Count > 0) 
            //{
            //        HideMaChiPhi3_Row();
            //        UpdateTongCPVC_From_Grid3();
            //    }
            //}


        }

        private void LoadDataCP()
        {
            //string mapbg = searchLookUpEditPBG.EditValue.ToString();
            string url = $"{URL}NhaCC/GetCP?action=GETPBGChiPhi&para1={_maphieubg}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableCP = JsonConvert.DeserializeObject<DataTable>(json);
            CreateSearchlookupcolTienTe();
            gridControl2.DataSource = _dtTableCP;
            if (_dtTableCP != null && _dtTableCP.Rows.Count > 0)
            {
                decimal tygiavnd = 0;
                object matiente = _dtTableCP.Rows[0]["DonViTienTe"];
                string urldvtt = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
                string jsondvtt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvtt); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsondvtt);
                _isLoadingTyGia = true;
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    tygiavnd = GetDecimalFromObject(tbl.Rows[0]["TyGia"]);

                    //_tienteID = tbl.Rows[0]["MaTienTe"].ToString();
                }
                //DataRow[] rows = _dtTableCP.Select("MaChiPhi = 3 OR MaChiPhi = 6");
                DataRow[] rows = _dtTableCP.Select("MaChiPhi IN (3, 6)");

                if (rows.Length > 0)
                {
                    foreach (DataRow r in rows)
                    {
                        decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                        decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);
                        decimal sotienquydoi = soTien * tygiavnd;
                        decimal thuePercent = thue / 100m;
                        decimal tien = sotienquydoi + (thuePercent * sotienquydoi);

                        _tongvccpps += tien;
                    }

                    spinEditTongCPVC.EditValue = _tongvccpps;
                }
                else
                {
                    _tongvccpps = 0;
                    spinEditTongCPVC.EditValue = 0;
                }
            }

        }



        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "MaNhom"
                               && info.EditValue != null);
            int groupIndex = gridView2.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }

            if (isNplGroup)
            {
                int isChecked = Convert.ToInt32(info.EditValue);
                info.GroupText = isChecked == 1 ? "Nhà cung cấp" : "Nội bộ";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn40)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }

            e.Handled = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            themdotbtn();

            //_coDotThanhToan = true;
            //HideMaChiPhi3_Row();

        }


        private void themdotbtn()
        {
            this.ActiveControl = simpleButton2;

            GridView view = gridView1;

            if (view.RowCount == 0)
            {
                XtraMessageBox.Show("Không có dòng nào trong phiếu để thêm đợt.", "Thông báo");
                return;
            }

            if (_dtTableDot == null)
                _dtTableDot = CreateDatatableDot();

            bool isTableEmpty = (_dtTableDot.Rows.Count == 0);

            // Lấy toàn bộ vật tư trong gridView1
            List<DataRow> vtList = new List<DataRow>();
            for (int i = 0; i < view.RowCount; i++)
            {
                var r = view.GetDataRow(i);
                if (r != null) vtList.Add(r);
            }

            if (vtList.Count == 0)
            {
                XtraMessageBox.Show("Không có dòng dữ liệu hợp lệ.", "Thông báo");
                return;
            }

            // =====================
            // LẦN ĐẦU → TẠO ĐỢT 1 & 2
            // =====================
            if (isTableEmpty)
            {
                int dot1 = 1;
                int dot2 = 2;

                foreach (var src in vtList)
                {
                    decimal tong = Convert.ToDecimal(
                        src["TongSLMuaThem"] == DBNull.Value ? 0 : src["TongSLMuaThem"]
                    );

                    // ĐỢT 1 = TỔNG
                    DataRow dr1 = _dtTableDot.NewRow();
                    FillRowDot(dr1, dot1, src, tong);
                    _dtTableDot.Rows.Add(dr1);

                    // ĐỢT 2 = 0
                    DataRow dr2 = _dtTableDot.NewRow();
                    FillRowDot(dr2, dot2, src, 0);
                    _dtTableDot.Rows.Add(dr2);
                }

                currentThuTu = GetNextAvailableThuTu();
                gridControl3.DataSource = _dtTableDot;
                gridControl3.RefreshDataSource();
                return;
            }

            int nextThuTu = GetNextAvailableThuTu();

            foreach (var src in vtList)
            {
                decimal tong = Convert.ToDecimal(
                    src["TongSLMuaThem"] == DBNull.Value ? 0 : src["TongSLMuaThem"]
                );

                decimal sumCurrent = GetSumSoLuongByMaVT(
                    src["MaVTID"].ToString(),
                    src["MauVTID"].ToString(),
                    src["KhoVaiID"].ToString(),
                    src["MaCLVT"].ToString()
                );

                decimal thieu = tong - sumCurrent;

                // BÙ PHẦN THIẾU VÀO ĐỢT CUỐI
                if (thieu > 0)
                {
                    DataRow lastDot = GetLastDotRow(
                        src["MaVTID"].ToString(),
                        src["MauVTID"].ToString(),
                        src["KhoVaiID"].ToString(),
                        src["MaCLVT"].ToString()
                    );

                    if (lastDot != null)
                    {
                        decimal slCuoi = Convert.ToDecimal(
                            lastDot["SoLuongVC"] == DBNull.Value ? 0 : lastDot["SoLuongVC"]
                        );

                        lastDot["SoLuongVC"] = slCuoi + thieu;
                    }
                }

                // ĐỢT MỚI LUÔN = 0
                DataRow newRow = _dtTableDot.NewRow();
                FillRowDot(newRow, nextThuTu, src, 0);
                _dtTableDot.Rows.Add(newRow);
            }

            currentThuTu = GetNextAvailableThuTu();
            gridControl3.DataSource = _dtTableDot;
            gridControl3.RefreshDataSource();
        }


        // Hàm fill dữ liệu chung
        private void FillRowDot(DataRow dr, int thuTu, DataRow src, decimal soLuong)
        {
            dr["Thutu"] = thuTu;
            dr["MaPTVC"] = "";
            dr["ChiPhiVC"] = 0;
            dr["MaVTID"] = src["MaVTID"];
            dr["MaNhomVT"] = src["MaCLVT"];
            dr["MaMauVT"] = src["MauVTID"];
            dr["MaKhoVT"] = src["KhoVaiID"];
            dr["SoLuongVC"] = soLuong;
            dr["NgayBatDau"] = DBNull.Value;
            dr["NhanMin"] = src["SoNgayGHSom"] ?? 0;
            dr["NhanMax"] = src["SoNgayGHTre"] ?? 0;
            dr["NgayNhanDK"] = DBNull.Value;
            dr["NgayNhanTT"] = DBNull.Value;
            dr["GhiChu"] = src["GhiChu"];
            dr["NguoiTao"] = GlobleData.UserName.ToString();
            dr["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
            dr["MoTa"] = src["MoTa"];
            dr["NPL"] = src["NPL"];
            dr["ItemCode"] = src["ItemCode"];
            dr["MauVT"] = src["MauVT"];
            dr["KhoVai"] = src["KhoVai"];
            dr["TenDVVT"] = src["TenDVVT"];
            dr["Thue"] = _thuevc;
            dr["ThanhTienVC"] = 0;
            dr["ColorCode"] = src["ColorCode"];
            dr["MaPhieuBG"] = src["MaPhieuBG"];
            dr["TenPhieu"] = src["TenPhieu"];
        }


        private int GetNextAvailableThuTu()
        {
            if (_dtTableDot == null || _dtTableDot.Rows.Count == 0)
                return 1;

            // Lấy danh sách các ThuTu đã dùng (loại bỏ null và <1)
            var used = _dtTableDot.AsEnumerable()
                        .Where(r => r["Thutu"] != DBNull.Value)
                        .Select(r =>
                        {
                            int v = 0;
                            int.TryParse(r["Thutu"].ToString(), out v);
                            return v;
                        })
                        .Where(x => x > 0)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

            // Tìm số nguyên dương nhỏ nhất không nằm trong used (smallest missing positive)
            int expect = 1;
            foreach (int u in used)
            {
                if (u == expect) expect++;
                else if (u > expect) break;
            }
            return expect;
        }

        private decimal GetSumSoLuongByMaVT(
            string maVT,
            string mauVT,
            string khoVai,
            string maCLVT)
        {
            return _dtTableDot.AsEnumerable()
                .Where(r =>
                    r.Field<string>("MaVTID") == maVT &&
                    r.Field<string>("MaMauVT") == mauVT &&
                    r.Field<string>("MaKhoVT") == khoVai &&
                    r.Field<string>("MaNhomVT") == maCLVT
                )
                .Sum(r => r.Field<decimal?>("SoLuongVC") ?? 0);
        }

        private DataRow GetLastDotRow(
            string maVT,
            string mauVT,
            string khoVai,
            string maCLVT)
        {
            return _dtTableDot.AsEnumerable()
                .Where(r =>
                    r.Field<string>("MaVTID") == maVT &&
                    r.Field<string>("MaMauVT") == mauVT &&
                    r.Field<string>("MaKhoVT") == khoVai &&
                    r.Field<string>("MaNhomVT") == maCLVT
                )
                .OrderByDescending(r => r.Field<int>("ThuTu"))
                .FirstOrDefault();
        }


        private void gridView2_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;

            e.Appearance.BackColor = Color.LightYellow; // Màu nền

            e.Appearance.ForeColor = Color.Red;

            e.Appearance.Font = new Font("Tahoma", 8F, FontStyle.Bold);
        }

        private void gridView3_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;

            e.Appearance.BackColor = Color.LightYellow; // Màu nền

            e.Appearance.ForeColor = Color.Red;

            e.Appearance.Font = new Font("Tahoma", 9F, FontStyle.Bold);
        }


        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e.Column.FieldName == "ChiPhiVC")
            //{
            //    string maVTID = gridView3.GetRowCellValue(e.RowHandle, "MaVTID").ToString();
            //    CapNhatThanhTien_ByMaVT(maVTID);
            //}

            var view = sender as GridView;
            DataRow currentRow = view.GetDataRow(e.RowHandle);
            if (e.Column.FieldName == "ChiPhiVC")
            {
                string maVTID = view.GetRowCellValue(e.RowHandle, "MaVTID")?.ToString();
                string mauvtid = view.GetRowCellValue(e.RowHandle, "MaMauVT")?.ToString();
                string khovtid = view.GetRowCellValue(e.RowHandle, "MaKhoVT")?.ToString();
                string maclvt = view.GetRowCellValue(e.RowHandle, "MaNhomVT")?.ToString();
                CapNhatThanhTien_ByMaVT(maVTID, mauvtid, khovtid, maclvt);
            }
            else if (e.Column.FieldName == "SoLuongVC")
            {

                if (currentRow == null) return;

                string maVTID = currentRow["MaVTID"]?.ToString();
                string mauVTID = currentRow["MaMauVT"]?.ToString();
                string khoVTID = currentRow["MaKhoVT"]?.ToString();
                string maNhomVT = currentRow["MaNhomVT"]?.ToString();
                string tenVT = currentRow["MoTa"]?.ToString();

                if (string.IsNullOrEmpty(maVTID)) return;

                int thuTu = Convert.ToInt32(currentRow["ThuTu"]);

                decimal newVal = 0;
                decimal.TryParse(e.Value?.ToString(), out newVal);

                decimal oldVal = 0;
                decimal.TryParse(_oldSoLuongVC.ToString(), out oldVal);

                //decimal tong = 0;
                //for (int i = 0; i < gridView1.RowCount; i++)
                //{
                //    if (
                //        gridView1.GetRowCellValue(i, "MaVTID")?.ToString() == maVTID &&
                //        gridView1.GetRowCellValue(i, "MauVTID")?.ToString() == mauVTID &&
                //        gridView1.GetRowCellValue(i, "KhoVaiID")?.ToString() == khoVTID &&
                //        gridView1.GetRowCellValue(i, "MaCLVT")?.ToString() == maNhomVT
                //    )
                //    {
                //        decimal.TryParse(
                //            gridView1.GetRowCellValue(i, "TongSLMuaThem")?.ToString(),
                //            out tong);
                //        break;
                //    }
                //}

                //var sameDots = _dtTableDot.AsEnumerable()
                //    .Where(r =>
                //        r["MaVTID"]?.ToString() == maVTID &&
                //        r["MaMauVT"]?.ToString() == mauVTID &&
                //        r["MaKhoVT"]?.ToString() == khoVTID &&
                //        r["MaNhomVT"]?.ToString() == maNhomVT
                //    )
                //    .OrderBy(r => Convert.ToInt32(r["ThuTu"]))
                //    .ToList();

                //int soDot = sameDots.Count;
                //if (soDot < 2) return;

                //var lastDot = sameDots.Last();
                //int thuTuLast = Convert.ToInt32(lastDot["ThuTu"]);

                //if (thuTu == thuTuLast)
                //{
                //    view.CellValueChanged -= gridView3_CellValueChanged;
                //    view.SetRowCellValue(e.RowHandle, "SoLuongVC", oldVal);
                //    view.CellValueChanged += gridView3_CellValueChanged;

                //    XtraMessageBox.Show(
                //        "Đợt cuối sẽ được hệ thống tự động phân bổ.",
                //        "Thông báo",
                //        MessageBoxButtons.OK,
                //        MessageBoxIcon.Warning);

                //    return;
                //}

                //decimal sumBeforeLast = 0;

                //foreach (var r in sameDots)
                //{
                //    int t = Convert.ToInt32(r["ThuTu"]);
                //    if (t == thuTuLast) continue;

                //    if (t == thuTu)
                //        sumBeforeLast += newVal;
                //    else
                //        sumBeforeLast += Convert.ToDecimal(r["SoLuongVC"] ?? 0);
                //}

                //if (sumBeforeLast > tong)
                //{
                //    view.CellValueChanged -= gridView3_CellValueChanged;
                //    view.SetRowCellValue(e.RowHandle, "SoLuongVC", oldVal);
                //    view.CellValueChanged += gridView3_CellValueChanged;

                //    XtraMessageBox.Show(
                //        $"Tổng phân bổ cho vật tư {tenVT} vượt quá Tổng yêu cầu ({tong})",
                //        "Cảnh báo",
                //        MessageBoxButtons.OK,
                //        MessageBoxIcon.Warning);

                //    return;
                //}

                //decimal slLast = tong - sumBeforeLast;
                //if (slLast < 0) slLast = 0;

                //view.CellValueChanged -= gridView3_CellValueChanged;
                //lastDot["SoLuongVC"] = slLast;
                //view.RefreshData();
                //view.CellValueChanged += gridView3_CellValueChanged;
                var result = ReCalcSoLuongVC(
                            maVTID,
                            mauVTID,
                            khoVTID,
                            maNhomVT,
                            true,
                            thuTu,
                            newVal,
                            oldVal,
                            out decimal tong
                        );

                // 👉 XỬ LÝ UI TẠI ĐÂY
                if (result != ReCalcResult.Success)
                {
                    view.CellValueChanged -= gridView3_CellValueChanged;
                    view.SetRowCellValue(e.RowHandle, "SoLuongVC", oldVal);
                    view.CellValueChanged += gridView3_CellValueChanged;

                    if (result == ReCalcResult.VuotTong)
                    {
                        XtraMessageBox.Show(
                            $"Tổng phân bổ cho vật tư {tenVT} vượt quá Tổng yêu cầu ({tong})",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else if (result == ReCalcResult.InvalidEdit)
                    {
                        XtraMessageBox.Show(
                            "Không được chỉnh sửa trực tiếp đợt cuối.\nHệ thống sẽ tự phân bổ.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    return;
                }

                CapNhatThanhTien_ByMaVT(maVTID, mauVTID, khoVTID, maNhomVT);
            }
            ////tong cua grid
            if (e.Column.FieldName == "ChiPhiVC" || e.Column.FieldName == "SoLuongVC" || e.Column.FieldName == "Thue")
            {
                decimal sotienvc = GetDecimalValue(view, e.RowHandle, "ChiPhiVC");
                decimal soluongvc = GetDecimalValue(view, e.RowHandle, "SoLuongVC");
                decimal thue = GetThuePercent(view, e.RowHandle, "Thue");
                decimal thanhTien = sotienvc + (thue * (sotienvc));
                view.SetRowCellValue(e.RowHandle, "ThanhTienVC", Math.Round(thanhTien, 2));

                string maVTID = view.GetRowCellValue(e.RowHandle, "MaVTID")?.ToString();
                string mauvtid = view.GetRowCellValue(e.RowHandle, "MaMauVT")?.ToString();
                string khovtid = view.GetRowCellValue(e.RowHandle, "MaKhoVT")?.ToString();
                string maclvt = view.GetRowCellValue(e.RowHandle, "MaNhomVT")?.ToString();
                CapNhatThanhTien_ByMaVT(maVTID, mauvtid, khovtid, maclvt);

                // CẬP NHẬT TỔNG
                UpdateGridView5Summary();
                UpdateGridView5Values();
            }
            if (e.Column.FieldName == "NgayNhanDK" || e.Column.FieldName == "NgayNhanTT")
            {
                if (currentRow == null) return;

                int thuTu = Convert.ToInt32(currentRow["ThuTu"]);
                DateTime? newDate = e.Value == null || e.Value == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(e.Value);

                view.CellValueChanged -= gridView3_CellValueChanged;

                foreach (DataRow r in _dtTableDot.Rows)
                {
                    if (Convert.ToInt32(r["ThuTu"]) == thuTu)
                    {
                        r[e.Column.FieldName] = newDate ?? (object)DBNull.Value;
                    }
                }

                view.CellValueChanged += gridView3_CellValueChanged;
                view.RefreshData();

                return;
            }
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            if (e.Column.FieldName == "SoTien" || e.Column.FieldName == "Thue" || e.Column.FieldName == "DonViTienTe")
            {
                gridView2.PostEditor();
                gridView2.UpdateCurrentRow();
                decimal tongSoTien = TinhTongSoTien();

                decimal sotien = GetDecimalValue(view, e.RowHandle, "SoTien");
                decimal thue = GetThuePercent(view, e.RowHandle, "Thue");
                decimal thanhTien = sotien + (thue * sotien);
                view.SetRowCellValue(e.RowHandle, "ThanhTienCPPS", Math.Round(thanhTien, 2));

                object maChiPhiObj = view.GetRowCellValue(e.RowHandle, "MaChiPhi");
                if (maChiPhiObj != null)
                {
                    int maChiPhi = Convert.ToInt32(maChiPhiObj);

                    // Nếu là 3 hoặc 6 → cập nhật _tongvccpps
                    if (maChiPhi == 3 || maChiPhi == 6)
                    {
                        CapNhatTongChiPhiVC();
                    }
                }
            }

            UpdateGridView5Summary();
            UpdateGridView5Values();
        }

        private decimal TinhTongChiPhiVC_ByMaVT(string maVTID, string mauvtid, string khovtid, string maclvt)
        {
            decimal tong = 0;

            for (int i = 0; i < gridView3.RowCount; i++)
            {
                var vt = gridView3.GetRowCellValue(i, "MaVTID");
                var mauvt = gridView3.GetRowCellValue(i, "MaMauVT");
                var khovai = gridView3.GetRowCellValue(i, "MaKhoVT");
                var nhom = gridView3.GetRowCellValue(i, "MaNhomVT");
                if (vt == null) continue;

                if (vt.ToString() == maVTID && mauvt.ToString() == mauvtid && khovai.ToString() == khovtid && nhom.ToString() == maclvt)
                {
                    //var val = gridView3.GetRowCellValue(i, "ChiPhiVC");
                    //if (val != null && val != DBNull.Value)
                    //    tong += Convert.ToDecimal(val);

                    var valChiPhi = gridView3.GetRowCellValue(i, "ChiPhiVC");
                    var valThue = gridView3.GetRowCellValue(i, "Thue");

                    if (valChiPhi != null && valChiPhi != DBNull.Value)
                    {
                        decimal chiPhiVC = Convert.ToDecimal(valChiPhi);
                        decimal thue = 0;

                        if (valThue != null && valThue != DBNull.Value)
                            thue = Convert.ToDecimal(valThue);


                        tong += chiPhiVC + (chiPhiVC * (thue / 100));
                    }
                }
            }
            return tong;
        }

        private void btnLuuCP_Click(object sender, EventArgs e)
        {

        }

        private decimal TinhTongSoTien()
        {
            decimal tong = 0;

            //for (int i = 0; i < gridView2.RowCount; i++)
            //{
            //    var val = gridView2.GetRowCellValue(i, "SoTien");
            //    if (val != null && val != DBNull.Value)
            //        tong += Convert.ToDecimal(val);
            //}
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                var valTien = gridView2.GetRowCellValue(i, "SoTien");
                var valThue = gridView2.GetRowCellValue(i, "Thue");

                if (valTien != null && valTien != DBNull.Value)
                {
                    decimal soTien = Convert.ToDecimal(valTien);
                    decimal thue = 0;

                    if (valThue != null && valThue != DBNull.Value)
                        thue = Convert.ToDecimal(valThue);

                    // Cộng tiền + tiền thuế
                    tong += soTien + (soTien * (thue / 100));
                }
            }
            return tong;
        }


        private void CapNhatThanhTien_ByMaVT(string maVTID, string mauvtid, string khovtid, string maclvt)
        {
            int rowMain = -1;

            // tìm dòng vật tư tương ứng trong gridView1
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if (gridView1.GetRowCellValue(i, "MaVTID").ToString() == maVTID && gridView1.GetRowCellValue(i, "MauVTID").ToString() == mauvtid
                    && gridView1.GetRowCellValue(i, "KhoVaiID").ToString() == khovtid && gridView1.GetRowCellValue(i, "MaCLVT").ToString() == maclvt)
                {
                    rowMain = i;
                    break;
                }
            }
            if (rowMain < 0) return;

            decimal thanhTienGoc = 0;
            var val = gridView1.GetRowCellValue(rowMain, "ThanhTien");
            if (val != null && val != DBNull.Value)
                thanhTienGoc = Convert.ToDecimal(val);

            _tongVC = TinhTongChiPhiVC_ByMaVT(maVTID, mauvtid, khovtid, maclvt);

            decimal tongSoTien = TinhTongSoTien();

            //gridView1.SetRowCellValue(rowMain, "ChiPhiVanChuyen", _tongVC);
            //decimal newThanhTien = thanhTienGoc + _tongVC + tongSoTien;

            //gridView1.SetRowCellValue(rowMain, "ThanhTien", newThanhTien);
        }

        private void CreateSearchlookupcolPTThanhToan()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPTThanhToan&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditkho = new RepositoryItemSearchLookUpEdit();
            rCountryEditkho.DataSource = _tbldv;
            rCountryEditkho.DisplayMember = "ThanhToan";
            rCountryEditkho.ValueMember = "MaPTThanhToan";
            rCountryEditkho.ShowClearButton = false;
            rCountryEditkho.NullText = "[Chọn giá trị]";

            GridView dvViewkho = rCountryEditkho.View;
            if (dvViewkho.Columns.Count == 0)
            {
                dvViewkho.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkho.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkho.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkho.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkho.Columns.Add(new GridColumn { FieldName = "MaPTThanhToan", Caption = "Mã", Name = "colMaHang", Visible = false });
                dvViewkho.Columns.Add(new GridColumn { FieldName = "ThanhToan", Caption = "Phương Thức Thanh Toán", Name = "colTenHang", Visible = true });
                dvViewkho.Columns.Add(new GridColumn { FieldName = "GhiChu", Caption = "Ghi Chú", Name = "colTenHang", Visible = true });

            }
            gridColumn28.ColumnEdit = rCountryEditkho;

        }

        private void btnXoaCP_Click(object sender, EventArgs e)
        {
            GridView view = gridControl2.MainView as GridView;

            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    if (_IsEdit == true)
                    {
                        string machiphi = row["MaChiPhi"].ToString();
                        string url = string.Format("{0}?para={1}&para1={2}", URL + "NhaCC/DeleteCPPMH", _maphieumh, machiphi);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                    ((DataTable)gridControl2.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
            }

            UpdateGridView5Summary();
            UpdateGridView5Values();
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            GridView view = gridControl3.MainView as GridView;
            DataTable dt = gridControl3.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
                return;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                int selecrow = Convert.ToInt32(row["ThuTu"]);
                int maxnum = dt.AsEnumerable()
                 .Max(r => Convert.ToInt32(r["ThuTu"]));

                if (maxnum > 2)
                {
                    if (selecrow != maxnum)
                    {
                        XtraMessageBox.Show(
                            "Chỉ được phép xóa đợt cuối cùng.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }


                // 1. Lấy danh sách các đợt (ThuTu DISTINCT)
                var listThuTu = dt.AsEnumerable()
                                  .Where(r => r["ThuTu"] != DBNull.Value)
                                  .Select(r => Convert.ToInt32(r["ThuTu"]))
                                  .Distinct()
                                  .OrderBy(x => x)
                                  .ToList();

                int soDot = listThuTu.Count;

                if (soDot <= 1)
                {
                    XtraMessageBox.Show(
                        "Không thể xóa khi chỉ còn 1 đợt.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 2. Xác định các ThuTu cần xóa
                List<int> thuTuCanXoa = new List<int>();

                if (soDot == 2)
                {
                    // Xóa cả 2 đợt
                    thuTuCanXoa = listThuTu;
                }
                else
                {
                    // >= 3 đợt → chỉ xóa đợt cuối
                    thuTuCanXoa.Add(listThuTu.Max());
                }

                string msg = soDot == 2
                    ? "Bạn có chắc muốn xóa TOÀN BỘ 2 ĐỢT không?"
                    : $"Bạn có chắc muốn xóa ĐỢT {thuTuCanXoa.First()} không?";

                if (XtraMessageBox.Show(
                    msg,
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                // 3. GỌI API
                if (_IsEdit)
                {
                    foreach (int thutu in thuTuCanXoa)
                    {
                        string url = string.Format(
                            "{0}?para={1}&para1={2}",
                            URL + "NhaCC/DeleteDotPMH",
                            _maphieumh, // MaPhieuMH
                            thutu       // ThuTu
                        );

                        string result = Task.Run(async () =>
                        {
                            return await _clientExtension.DeletedAsync(url);
                        }).Result;
                    }
                }

                // 4. Xóa trong DataTable
                var rowsToDelete = dt.AsEnumerable()
                                     .Where(r => thuTuCanXoa.Contains(Convert.ToInt32(r["ThuTu"])))
                                     .ToList();

                foreach (var r in rowsToDelete)
                    dt.Rows.Remove(r);

                dt.AcceptChanges();
                view.RefreshData();
                ReCalcSoLuongVC_SauKhiXoa();
            }

            //DataTable dt = gridControl3.DataSource as DataTable;


        }

        private void ReCalcSoLuongVC_SauKhiXoa()
        {
            if (_dtTableDot == null || _dtTableDot.Rows.Count == 0)
                return;

            // Gom nhóm theo từng vật tư
            var groups = _dtTableDot.AsEnumerable()
                .GroupBy(r => new
                {
                    MaVTID = r["MaVTID"]?.ToString(),
                    MaMauVT = r["MaMauVT"]?.ToString(),
                    MaKhoVT = r["MaKhoVT"]?.ToString(),
                    MaNhomVT = r["MaNhomVT"]?.ToString()
                });

            foreach (var g in groups)
            {
                string maVTID = g.Key.MaVTID;
                string mauVTID = g.Key.MaMauVT;
                string khoVTID = g.Key.MaKhoVT;
                string maNhomVT = g.Key.MaNhomVT;

                // 1. LẤY TỔNG TỪ GRIDVIEW1
                decimal tong = 0;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (
                        gridView1.GetRowCellValue(i, "MaVTID")?.ToString() == maVTID &&
                        gridView1.GetRowCellValue(i, "MauVTID")?.ToString() == mauVTID &&
                        gridView1.GetRowCellValue(i, "KhoVaiID")?.ToString() == khoVTID &&
                        gridView1.GetRowCellValue(i, "MaCLVT")?.ToString() == maNhomVT
                    )
                    {
                        decimal.TryParse(
                            gridView1.GetRowCellValue(i, "TongSLMuaThem")?.ToString(),
                            out tong);
                        break;
                    }
                }

                // 2. LẤY CÁC ĐỢT CÒN LẠI
                var dots = g.OrderBy(r => Convert.ToInt32(r["ThuTu"])).ToList();
                if (dots.Count < 2)
                    continue;

                var lastDot = dots.Last();
                int thuTuLast = Convert.ToInt32(lastDot["ThuTu"]);

                // 3. TÍNH TỔNG CÁC ĐỢT TRƯỚC
                decimal sumBeforeLast = 0;
                foreach (var r in dots)
                {
                    if (Convert.ToInt32(r["ThuTu"]) == thuTuLast)
                        continue;

                    sumBeforeLast += Convert.ToDecimal(r["SoLuongVC"] ?? 0);
                }

                // 4. GÁN LẠI ĐỢT CUỐI = PHẦN DƯ
                decimal slLast = tong - sumBeforeLast;
                if (slLast < 0) slLast = 0;

                lastDot["SoLuongVC"] = slLast;
            }
        }


        private decimal GetAllocatedSoLuongByMaVT(string maVTID, string mauvtid, string khovtid, string maclvt)
        {
            if (_dtTableDot == null) return 0;
            decimal total = 0;
            foreach (DataRow r in _dtTableDot.Rows)
            {
                if (r == null) continue;
                var vt = r["MaVTID"];
                var nhomvt = r["MaNhomVT"];
                var mauvt = r["MaMauVT"];
                var khovt = r["MaKhoVT"];
                if (vt == null) continue;
                if (vt.ToString() == maVTID && mauvt.ToString() == mauvtid && khovt.ToString() == khovtid && nhomvt.ToString() == maclvt)
                {
                    decimal v = 0;
                    if (r["SoLuongVC"] != null && r["SoLuongVC"] != DBNull.Value)
                        decimal.TryParse(r["SoLuongVC"].ToString(), out v);
                    total += v;
                }
            }
            return total;
        }



        //private void UpdateGridView5Summary()
        //{
        //    gridView1.PostEditor();
        //    gridView2.PostEditor();
        //    gridView3.PostEditor();

        //    gridView1.UpdateCurrentRow();
        //    gridView2.UpdateCurrentRow();
        //    gridView3.UpdateCurrentRow();

        //    decimal tongThanhTien = 0;
        //    decimal tongchiphips = 0;
        //    decimal tongchiphivc = 0;

        //    for (int i = 0; i < gridView1.RowCount; i++)
        //        tongThanhTien += Convert.ToDecimal(gridView1.GetRowCellValue(i, "ThanhTien") ?? 0);

        //    for (int i = 0; i < gridView2.RowCount; i++)
        //    {
        //        decimal soTien = Convert.ToDecimal(gridView2.GetRowCellValue(i, "SoTien") ?? 0);
        //        decimal thue = Convert.ToDecimal(gridView2.GetRowCellValue(i, "Thue") ?? 0);

        //        decimal thanhTien = soTien * (1 + (thue/100));

        //        tongchiphips += thanhTien;
        //    }

        //    if (_dtTableDot != null && _dtTableDot.Rows.Count >0) 
        //    {
        //        for (int i = 0; i < gridView3.RowCount; i++)
        //        {
        //            decimal sl = Convert.ToDecimal(gridView3.GetRowCellValue(i, "SoLuongVC") ?? 0);
        //            decimal cp = Convert.ToDecimal(gridView3.GetRowCellValue(i, "ChiPhiVC") ?? 0);
        //            decimal thue = Convert.ToDecimal(gridView3.GetRowCellValue(i, "Thue") ?? 0);

        //            decimal thanhTienVC = sl * cp * (1 + (thue / 100));

        //            tongchiphivc += thanhTienVC;
        //        }

        //        spinEditTongCPVC.EditValue = tongchiphivc;
        //    }


        //    spinEditTongCPVT.EditValue = tongThanhTien;
        //    spinEditTongCPPS.EditValue = tongchiphips;

        //}


        private void UpdateGridView5Summary()
        {
            // Ensure editors commit first
            gridView1.PostEditor();
            gridView2.PostEditor();
            gridView3.PostEditor();

            gridView1.UpdateCurrentRow();
            gridView2.UpdateCurrentRow();
            gridView3.UpdateCurrentRow();

            decimal tongThanhTien = 0;
            decimal tongchiphips = 0;
            decimal tongchiphivc = 0;
            decimal tygiavnd = 0;
            decimal tyGia = GetDecimalFromObject(spinEditTyGia.EditValue);
            // Grid1: tổng ThanhTien
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                tongThanhTien += GetDecimalValue(gridView1, i, "ThanhTienQD");
            }

            // Grid2: tổng chi phí phụ: SoTien * (1 + Thue%)
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                object maChiPhiObj = gridView2.GetRowCellValue(i, "MaChiPhi");
                object matiente = gridView2.GetRowCellValue(i, "DonViTienTe");
                string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                //_isLoadingTyGia = true;
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    tygiavnd = GetDecimalFromObject(tbl.Rows[0]["TyGia"]);

                    //_tienteID = tbl.Rows[0]["MaTienTe"].ToString();
                }
                if (maChiPhiObj == null) continue;

                int maChiPhi = Convert.ToInt32(maChiPhiObj);

                if (maChiPhi == 3 || maChiPhi == 6)
                    continue;

                decimal soTien = GetDecimalValue(gridView2, i, "SoTien");
                decimal thue = GetDecimalValue(gridView2, i, "Thue");
                decimal tienquydoitheovnd = soTien * tygiavnd;
                decimal thuePercent = thue / 100m;
                decimal thanhTien = (tienquydoitheovnd + (thuePercent * tienquydoitheovnd));
                tongchiphips += thanhTien;
            }

            for (int i = 0; i < gridView2.RowCount; i++)
            {
                object maChiPhiObj = gridView2.GetRowCellValue(i, "MaChiPhi");
                object matiente = gridView2.GetRowCellValue(i, "DonViTienTe");

                if (maChiPhiObj == null)
                    continue;

                int maChiPhi = Convert.ToInt32(maChiPhiObj);

                // 👉 CHỈ LẤY MaChiPhi = 3 hoặc 6
                if (maChiPhi != 3 && maChiPhi != 6)
                    continue;

                string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2=VND&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl != null && tbl.Rows.Count > 0)
                {
                    tygiavnd = GetDecimalFromObject(tbl.Rows[0]["TyGia"]);
                }

                decimal soTien = GetDecimalValue(gridView2, i, "SoTien");
                decimal thue = GetDecimalValue(gridView2, i, "Thue");

                decimal tienquydoitheovnd = soTien * tygiavnd;
                decimal thuePercent = thue / 100m;
                decimal thanhTien = tienquydoitheovnd + (thuePercent * tienquydoitheovnd);

                tongchiphivc += thanhTien;
            }

            // Grid3: tổng chi phí vận chuyển theo từng dòng: SoLuongVC * ChiPhiVC * (1 + Thue%)
            //if (_dtTableDot != null && _dtTableDot.Rows.Count > 0)
            //{
            //    for (int i = 0; i < gridView3.RowCount; i++)
            //    {
            //        //decimal sl = GetDecimalValue(gridView3, i, "SoLuongVC");
            //        decimal cp = GetDecimalValue(gridView3, i, "ChiPhiVC");
            //        decimal thue = GetDecimalValue(gridView3, i, "Thue");
            //        decimal thuePercent = thue / 100m;
            //        decimal thanhTienVC =  cp + (thuePercent * cp);
            //        tongchiphivc += thanhTienVC;
            //    }
            //}
            //else
            //{
            //    tongchiphivc = _tongvccpps;
            //}
            // Gán các spinEdit con
            spinEditTongCPVT.EditValue = tongThanhTien;
            //spinEditTongCPPS.EditValue = tongchiphips/tyGia;
            //spinEditTongCPVC.EditValue = tongchiphivc/tyGia;
            spinEditTongCPPS.EditValue = tyGia == 0 ? 0 : tongchiphips / tyGia;
            spinEditTongCPVC.EditValue = tyGia == 0 ? 0 : tongchiphivc / tyGia;
            //spinEditTongCPVC.EditValue = tongchiphivc;
            decimal totalcpvnd = tongThanhTien + tongchiphips + tongchiphivc;
            // Tổng CP (VT + PS)
            decimal totalCP = GetDecimalFromObject(spinEditTongCPVT.EditValue) + GetDecimalFromObject(spinEditTongCPPS.EditValue) + GetDecimalFromObject(spinEditTongCPVC.EditValue);
            spinEditTongCP.EditValue = totalCP;
            spinEditTongCPVND.EditValue = totalCP;
            // Nếu tiền tệ khác VND thì chuyển đổi
            //string loaitien = txtTienTe.Text;
            //if (!string.IsNullOrEmpty(loaitien) && loaitien != "VND")
            //{
            //    spinEditTongCPVND.EditValue = totalcpvnd;
            //}
            //else
            //{
            //    spinEditTongCPVND.EditValue = totalcpvnd;
            //}



            //spinEditTongCP.EditValue = Convert.ToDecimal(spinEditTongCPVT.EditValue) + Convert.ToDecimal(spinEditTongCPPS.EditValue);

            //string loaitien = txtTienTe.Text;
            //if (loaitien != "VND")
            //{
            //    spinEditTongCPVND.EditValue = Convert.ToDecimal(spinEditTongCP.EditValue) * Convert.ToDecimal(spinEditTyGia.EditValue);
            //}
            //else
            //{
            //    spinEditTongCPVND.EditValue = Convert.ToDecimal(spinEditTongCP.EditValue);
            //}
        }

        // Hỗ trợ đọc decimal an toàn từ editValue
        private decimal GetDecimalFromObject(object obj)
        {
            if (obj == null || obj == DBNull.Value) return 0m;
            decimal d;
            if (decimal.TryParse(obj.ToString(), out d)) return d;
            return 0m;
        }


        private void btnChonVT_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditNCC.EditValue == null || searchLookUpEditNCC.EditValue == "")
            {
                XtraMessageBox.Show("Chưa chọn nhà cung cấp.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //if (searchLookUpEditPBG.EditValue == null || searchLookUpEditPBG.EditValue == "")
            //{
            //    XtraMessageBox.Show("Chưa chọn phiếu báo giá.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            if (txtTienTe.EditValue == null || txtTienTe.EditValue == "")
            {
                return;
            }
            string mancc = searchLookUpEditNCC.EditValue.ToString();
            //string maphieubg = searchLookUpEditPBG.EditValue.ToString();
            string maphieubg = string.Empty;
            string matiente = txtTienTe.EditValue.ToString();
            frmChonVTPhieu frm = new frmChonVTPhieu(mancc, maphieubg, matiente, _IsMuaTheoVT);
            if (_dtTable != null && _dtTable.Rows.Count > 0)
            {
                frm.ListExistingVT = _dtTable.Copy();
            }
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataTable selectedTable = frm._tbl;
                if (selectedTable == null || selectedTable.Rows.Count == 0)
                    return;
                if (_dtTable == null || _dtTable.Rows.Count == 0)
                {
                    _dtTable = CreateDatatable();
                }

                foreach (DataRow row in selectedTable.Rows)
                {
                    DataRow newRow = _dtTable.NewRow();
                    newRow["MaVTID"] = row["MaVTID"];
                    newRow["MauVTID"] = row["MauVTID"];
                    newRow["KhoVaiID"] = row["KhoVaiID"];
                    newRow["MaDVVT"] = row["MaDVVT"];
                    newRow["ItemCode"] = row["ItemCode"];
                    newRow["NPL"] = row["NPL"];
                    newRow["DonGia"] = row["DonGia"];
                    newRow["TongSL"] = row["TongSL"];
                    newRow["TongSLMuaThem"] = row["TongSLMuaThem"];
                    newRow["ThanhTien"] = row["ThanhTien"];
                    newRow["MoTa"] = row["MoTa"];
                    newRow["MaCLVT"] = row["MaCLVT"];
                    newRow["KhoVai"] = row["KhoVai"];
                    newRow["TenDVVT"] = row["TenDVVT"];
                    newRow["MauVT"] = row["MauVT"];
                    newRow["TenCL"] = row["TenCL"];
                    newRow["Thue"] = row["Thue"];
                    newRow["PTThanhToan"] = row["PTThanhToan"];
                    newRow["ChiPhiVanChuyen"] = row["ChiPhiVanChuyen"];
                    newRow["ChiPhiKhac"] = "";
                    newRow["PTVanChuyen"] = row["PTVanChuyen"];
                    newRow["NgayDuKienHV"] = "";
                    newRow["GhiChu"] = row["GhiChu"];
                    newRow["ChiPhiVT"] = row["ChiPhiVT"];
                    newRow["NgayHieuLuc"] = row["NgayBDHieuLuc"];
                    newRow["TienTeID"] = row["TienTeID"];
                    newRow["SoNgayGHSom"] = row["SoNgayGHSom"];
                    newRow["SoNgayGHTre"] = row["SoNgayGHTre"];
                    newRow["ColorCode"] = row["MaMauVT"];
                    newRow["ChietKhau"] = row["ChietKhau"];
                    newRow["ThanhTienVND"] = row["ThanhTienVND"];
                    newRow["ChiPhiSauCK"] = row["ChiPhiSauCK"];
                    newRow["MaThue_Gop"] = row["MaThue_Gop"];
                    newRow["MaChietKhau_Gop"] = row["MaChietKhau_Gop"];
                    newRow["ThanhTienQD"] = GetDecimalFromObject(row["ThanhTienVND"]) / Convert.ToDecimal(spinEditTyGia.EditValue);
                    newRow["TTChietKhau"] = "";
                    newRow["NgayGiaoHangYC"] = DBNull.Value;
                    newRow["MaPhieuBG"] = row["MaPhieuBG"];
                    newRow["TenPhieu"] = row["TenPhieu"];
                    _dtTable.Rows.Add(newRow);
                }
                if (_IsMuaTheoVT == false)
                {
                    _dtTableChiTiet = CreateDatatablePhieuDH();
                    HashSet<string> addedMaDH = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (DataRow row in selectedTable.Rows)
                    {
                        string rawDH = row["MaDHGOP"].ToString();
                        string[] listDH = rawDH.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string dh in listDH)
                        {
                            string maDH = dh.Trim();
                            if (addedMaDH.Contains(maDH)) continue;

                            string urlml = $"{URL}NhaCC/GePMH?action=GETMaLenh&para1={maDH}&para2=&para3=&para4=&para5=";
                            string jsonml = Task.Run(async () => await _clientExtension.GetAsnyc(urlml)).Result;

                            string maLenhSX = "";
                            if (!string.IsNullOrEmpty(jsonml))
                            {
                                try
                                {
                                    JArray arr = JArray.Parse(jsonml);
                                    if (arr.Count > 0 && arr[0]["MaLenhSanXuat"] != null)
                                    {
                                        maLenhSX = arr[0]["MaLenhSanXuat"].ToString().Trim();
                                    }
                                }
                                catch
                                {
                                    maLenhSX = jsonml.Replace("\"", "").Trim();
                                }
                            }
                            DataRow newRow = _dtTableChiTiet.NewRow();
                            newRow["ID"] = 0;
                            newRow["MaPhieuMH"] = "";
                            newRow["MaDH"] = maDH;
                            newRow["MaLenhSX"] = maLenhSX;
                            _dtTableChiTiet.Rows.Add(newRow);

                            addedMaDH.Add(maDH);
                        }
                    }
                }

                CreateSearchlookupcolPTThanhToan();
                gridControl1.DataSource = _dtTable;
                _maphieubg = _dtTable.Rows[0]["MaPhieuBG"].ToString();

                if (_IsEdit == false)
                {
                    LoadDataCP();
                    //themdot();
                }

                UpdateGridView5Summary();
                UpdateGridView5Values();

                //int soChungLoai = _dtTable.AsEnumerable()
                //              .Select(r => r["MaCLVT"].ToString())
                //              .Distinct()
                //              .Count();

                //spinEditSoLoaiVT.EditValue = soChungLoai;
                int soChungLoai = _dtTable.AsEnumerable()
                                .Where(r => r != null)
                                .Select(r =>
                                    string.Join("|",
                                        r["MaVTID"]?.ToString() ?? "",
                                        r["MauVTID"]?.ToString() ?? "",
                                        r["KhoVaiID"]?.ToString() ?? "",
                                        r["MaDVVT"]?.ToString() ?? ""
                                    )
                                )
                                .Where(k => k != "|||") // ❌ loại dòng rỗng
                                .Distinct()
                                .Count();

                spinEditSoLoaiVT.EditValue = soChungLoai;
            }

        }

        private void btnThemHT_Click(object sender, EventArgs e)
        {
            this.ActiveControl = simpleButton2;

            if (_dtTableHTTT == null || _dtTableHTTT.Rows.Count == 0)
            {
                _dtTableHTTT = CreateDatatableHTTT();
                thuTuDotTraTien = 1;
            }

            // ====== TÍNH TỔNG PHẦN TRĂM HIỆN CÓ ======
            decimal tongPT = 0;

            foreach (DataRow r in _dtTableHTTT.Rows)
            {
                if (r["PhanTram"] != DBNull.Value)
                    tongPT += Convert.ToDecimal(r["PhanTram"]);
            }

            decimal conLai = 100 - tongPT;
            if (conLai < 0) conLai = 0;
            int nextDotTra = 1;
            if (_dtTableHTTT.Rows.Count > 0)
            {
                nextDotTra = _dtTableHTTT.AsEnumerable()
                    .Select(r => {
                        int val;
                        return int.TryParse(r["DotTra"].ToString(), out val) ? val : 0;
                    })
                    .Max() + 1;
            }
            // ====== TẠO DÒNG MỚI ======
            DataRow dr = _dtTableHTTT.NewRow();
            dr["ID"] = 0;
            dr["MaPhieuMH"] = "";
            dr["TGTra"] = DateTime.Today.ToString("yyyy-MM-dd");
            dr["GhiChu"] = "";
            dr["PTThanhToan"] = "";
            dr["InvoiceTo"] = "";
            dr["DotTra"] = nextDotTra.ToString();
            // Gán % còn lại
            dr["PhanTram"] = conLai;

            // ====== TÍNH SỐ TIỀN TỰ ĐỘNG ======
            decimal tongCPVND = 0;
            if (spinEditTongCPVND.EditValue != null)
                tongCPVND = Convert.ToDecimal(spinEditTongCPVND.EditValue);

            decimal soTien = (tongCPVND * conLai) / 100;
            dr["SoTien"] = Math.Round(soTien, 2);

            // ====== THÊM DÒNG ======
            _dtTableHTTT.Rows.Add(dr);
            CreateSearchlookupcolPTThanhToanHT();
            gridControl4.DataSource = _dtTableHTTT;
            gridControl4.RefreshDataSource();
        }



        private void gridView5_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_isUpdatingGV5) return;

            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            decimal tongCPVND = Convert.ToDecimal(spinEditTongCPVND.EditValue);
            if (tongCPVND <= 0) return;

            try
            {
                _isUpdatingGV5 = true;
                // --- KIỂM TRA TỔNG PHẦN TRĂM -------------
                decimal totalPercent = 0;
                for (int i = 0; i < view.RowCount; i++)
                {
                    decimal val = 0;
                    decimal.TryParse(view.GetRowCellValue(i, "PhanTram")?.ToString(), out val);
                    totalPercent += val;
                }

                if (totalPercent > 100m)
                {
                    XtraMessageBox.Show(
                        "Tổng tỷ lệ phần trăm tiền trả không được vượt quá 100%.",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    view.CloseEditor();
                    view.UpdateCurrentRow();

                    view.SetRowCellValue(e.RowHandle, "PhanTram", _oldPhanTram);
                    view.SetRowCellValue(e.RowHandle, "SoTien", _oldSoTien);

                    return;
                }

                if (e.Column.FieldName == "PhanTram")
                {
                    decimal phantram = 0;
                    decimal.TryParse(view.GetRowCellValue(e.RowHandle, "PhanTram")?.ToString(), out phantram);

                    decimal sotien = tongCPVND * (phantram / 100m);

                    view.SetRowCellValue(e.RowHandle, "SoTien", Math.Round(sotien, 2));
                }

                // Khi sửa SỐ TIỀN -> tính PHẦN TRĂM
                if (e.Column.FieldName == "SoTien")
                {
                    decimal sotien = 0;
                    decimal.TryParse(view.GetRowCellValue(e.RowHandle, "SoTien")?.ToString(), out sotien);

                    decimal phantram = (sotien / tongCPVND) * 100m;

                    view.SetRowCellValue(e.RowHandle, "PhanTram", Math.Round(phantram, 2));
                }
            }
            finally
            {
                _isUpdatingGV5 = false;
            }
        }
     
        private void btnXoaHT_Click(object sender, EventArgs e)
        {
            GridView view = gridControl4.MainView as GridView;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    if (_IsEdit == true)
                    {
                        string dottra = row["DotTra"].ToString();
                        string url = string.Format("{0}?para={1}&para1={2}", URL + "NhaCC/DeleteHTTT", _maphieumh, dottra);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                    ((DataTable)gridControl4.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
            }

        }



        private void HideMaChiPhi3_Row()
        {
            if (_dtTableCP == null) return;

            // Tìm các dòng cần xóa
            var rows = _dtTableCP.Select("MaChiPhi = 3");

            foreach (DataRow r in rows)
                _dtTableCP.Rows.Remove(r);

            _dtTableCP.AcceptChanges();

            gridControl2.DataSource = _dtTableCP;
        }


        private void UpdateTongCPVC_From_Grid3()
        {
            decimal tong = 0;

            for (int i = 0; i < gridView3.RowCount; i++)
            {
                //decimal sl = Convert.ToDecimal(gridView3.GetRowCellValue(i, "SoLuongVC") ?? 0);
                decimal cp = Convert.ToDecimal(gridView3.GetRowCellValue(i, "ChiPhiVC") ?? 0);
                decimal thue = Convert.ToDecimal(gridView3.GetRowCellValue(i, "Thue") ?? 0);

                tong += cp * ((thue / 100) * cp);
            }

            spinEditTongCPVC.EditValue = tong;
        }

        private DataTable CreateDatatableQuyDoiSave()
        {
            DataTable tbl = new DataTable("dtTableCP");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("QuyDoiID", typeof(string));
            tbl.Columns.Add("MaTienTe", typeof(string));
            tbl.Columns.Add("Gia", typeof(float));
            tbl.Columns.Add("Ngay", typeof(DateTime));
            tbl.Columns.Add("UserID", typeof(string));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            return tbl;
        }
        private void spinEditTyGia_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (_isLoadingTyGia) return;
        }

        private void spinEditTyGia_Properties_Leave(object sender, EventArgs e)
        {
            SaveTyGia();
            decimal tyGia = Convert.ToDecimal(spinEditTyGia.EditValue);

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                decimal thanhTien = GetDecimalValue(gridView1, i, "ThanhTien");
                decimal thanhTienVND = thanhTien * tyGia;

                gridView1.SetRowCellValue(i, "ThanhTienVND", Math.Round(thanhTienVND, 2));
            }

            UpdateGridView5Summary();
            UpdateGridView5Values();
        }

        private void gridView5_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "PhanTram" || e.Column.FieldName == "SoTien")
            {
                var view = sender as GridView;
                if (view != null && e.RowHandle >= 0)
                {
                    decimal.TryParse(view.GetRowCellValue(e.RowHandle, e.Column)?.ToString(), out _oldValue);
                }
            }
        }

        private void gridView5_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;

            e.Appearance.BackColor = Color.LightYellow;

            e.Appearance.ForeColor = Color.Red;

            e.Appearance.Font = new Font("Tahoma", 8F, FontStyle.Bold);
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void gridView5_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void gridView2_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }

        private void spinEditTyGia_Properties_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (e.NewValue == null) return;

            decimal val;
            if (decimal.TryParse(e.NewValue.ToString(), out val))
            {
                if (val < 0)
                {
                    e.Cancel = true;
                }
            }
        }

        private void gridView3_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SoLuongVC")
            {
                _oldSoLuongVC = Convert.ToDecimal(
                    gridView3.GetRowCellValue(e.RowHandle, "SoLuongVC") ?? 0);
            }
        }

        private void gridView3_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName != "SoLuongVC") return;

            DataRow row = view.GetFocusedDataRow();
            if (row == null) return;

            int thuTuHienTai = Convert.ToInt32(row["ThuTu"]);

            int maxThuTu = _dtTableDot.AsEnumerable()
                .Max(r => Convert.ToInt32(r["ThuTu"]));

            if (thuTuHienTai == maxThuTu)
            {
                e.Cancel = true;
            }
        }

        private async void SaveTyGia()
        {
            //this.ActiveControl = simpleButton2;
            //DataTable tblQD = CreateDatatableQuyDoiSave();
            //DataRow newRow = tblQD.NewRow();
            //newRow["ID"] = 0;
            //newRow["QuyDoiID"] = "";
            //newRow["MaTienTe"] = _tienteID;
            //newRow["Gia"] = spinEditTyGia.EditValue;
            //newRow["Ngay"] = DateTime.Now.ToString("yyyy-MM-dd");
            //newRow["UserID"] = GlobleData.UserName.ToString();
            //newRow["NguoiSua"] = "";
            //newRow["NgaySua"] = DBNull.Value;
            //tblQD.Rows.Add(newRow);
            //string mgQD = await _clientExtension.PostAsync(URL + $"TIENTE/Post2?action=POSTQUYDOI", tblQD);
            //if (mgQD == string.Empty)
            //{
            //    clsWaitForm.ShowErrorForm(this, 3000);
            //}
        }

        private void UpdateGridView5Values()
        {
            if (_dtTableHTTT != null && _dtTableHTTT.Rows.Count > 0)
            {
                decimal tongVND = Convert.ToDecimal(spinEditTongCPVND.EditValue);
                if (tongVND <= 0) return;

                _isUpdatingGV5 = true;
                try
                {
                    foreach (DataRow r in _dtTableHTTT.Rows)
                    {
                        decimal pt = Convert.ToDecimal(r["PhanTram"]);
                        decimal soTien = tongVND * pt / 100;

                        r["SoTien"] = Math.Round(soTien, 2);
                    }
                }
                finally
                {
                    _isUpdatingGV5 = false;
                }

                gridControl4.RefreshDataSource();
            }

        }


        //private void HideOtherGrids(bool hide)
        //{
        //    var v = hide
        //        ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never
        //        : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

        //    layoutControlGroup8.Visibility = v;
        //    layoutControlGroup5.Visibility = v;
        //    layoutControlGroup6.Visibility = v;
        //}

        private void gridView5_GotFocus(object sender, EventArgs e)
        {
            //HideOtherGrids(true);
        }

        private void gridView5_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            //HideOtherGrids(true);
        }



        private void gridView5_LostFocus(object sender, EventArgs e)
        {
            //HideOtherGrids(false);
        }

        private void ApplyCurrencyFormat(SpinEdit spin, string currency)
        {
            if (spin == null) return;

            spin.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            spin.Properties.DisplayFormat.FormatString = "#,##0.00 " + currency;

            spin.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            spin.Properties.EditFormat.FormatString = "#,##0.00 " + currency;

            spin.Properties.Mask.UseMaskAsDisplayFormat = true;
        }


        private void CreateSearchlookupcolPTThanhToanHT()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPTThanhToan&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdittt = new RepositoryItemSearchLookUpEdit();
            rCountryEdittt.DataSource = _tbldv;
            rCountryEdittt.DisplayMember = "ThanhToan";
            rCountryEdittt.ValueMember = "MaPTThanhToan";
            rCountryEdittt.ShowClearButton = false;
            rCountryEdittt.NullText = "[Chọn giá trị]";

            GridView view = rCountryEdittt.View;
            if (view.Columns.Count == 0)
            {
                view.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                view.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                view.Appearance.HeaderPanel.Options.UseTextOptions = true;
                GridColumn colMa = new GridColumn
                {
                    FieldName = "MaPTThanhToan",
                    Caption = "Mã",
                    Visible = false
                };
                view.Columns.Add(colMa);

                GridColumn colThanhToan = new GridColumn
                {
                    FieldName = "ThanhToan",
                    Caption = "Phương Thức Thanh Toán",
                    Visible = true,
                    Width = 200
                };
                view.Columns.Add(colThanhToan);
                colThanhToan.VisibleIndex = 0;

                GridColumn colGhiChu = new GridColumn
                {
                    FieldName = "GhiChu",
                    Caption = "Ghi Chú",
                    Visible = true,
                    Width = 200
                };
                view.Columns.Add(colGhiChu);
                colGhiChu.VisibleIndex = 1;

                // Tắt auto-resize tránh nhảy cột
                view.OptionsView.ColumnAutoWidth = false;

            }
            gridColumn64.ColumnEdit = rCountryEdittt;

            //invoice to
            string urliv = $"{URL}NhaCC/GetInvoice?action=GETINVOICE&para=&para1=&para2=&para3=&para4=&para5=";
            string jsoniv = Task.Run(async () => { return await _clientExtension.GetAsnyc(urliv); }).Result;
            DataTable _tbliv = JsonConvert.DeserializeObject<DataTable>(jsoniv);
            RepositoryItemSearchLookUpEdit rCountryEditiv = new RepositoryItemSearchLookUpEdit();
            rCountryEditiv.DataSource = _tbliv;
            rCountryEditiv.DisplayMember = "Company";
            rCountryEditiv.ValueMember = "CompanyID";
            rCountryEditiv.ShowClearButton = false;
            rCountryEditiv.NullText = "[Chọn giá trị]";

            GridView view1 = rCountryEditiv.View;
            if (view1.Columns.Count == 0)
            {
                view1.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                view1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                view1.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                view1.Appearance.HeaderPanel.Options.UseTextOptions = true;
                GridColumn colCompanyID = new GridColumn
                {
                    FieldName = "CompanyID",
                    Caption = "Mã",
                    Visible = false
                };
                view1.Columns.Add(colCompanyID);

                GridColumn colCompany = new GridColumn
                {
                    FieldName = "Company",
                    Caption = "Company",
                    Visible = true,
                    Width = 200
                };
                view1.Columns.Add(colCompany);
                colCompany.VisibleIndex = 0;

                GridColumn colAddress = new GridColumn
                {
                    FieldName = "Address",
                    Caption = "Address",
                    Visible = true,
                    Width = 200
                };
                view1.Columns.Add(colAddress);
                colAddress.VisibleIndex = 1;

                GridColumn colTAX = new GridColumn
                {
                    FieldName = "TAX",
                    Caption = "TAX",
                    Visible = true,
                    Width = 150
                };
                view1.Columns.Add(colTAX);
                colTAX.VisibleIndex = 2;

                GridColumn colVAT = new GridColumn
                {
                    FieldName = "VAT",
                    Caption = "VAT",
                    Visible = true,
                    Width = 150
                };
                view1.Columns.Add(colVAT);
                colVAT.VisibleIndex = 3;

                // Tắt auto-resize tránh nhảy cột
                view1.OptionsView.ColumnAutoWidth = false;

            }
            gridColumn78.ColumnEdit = rCountryEditiv;
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "ThanhTien")
            {
                e.Appearance.ForeColor = Color.Blue;
            }

            if (e.Column.FieldName == "TienTeID")
            {
                e.Appearance.ForeColor = Color.Blue;
            }
        }

        private void CapNhatTongChiPhiVC()
        {
            _tongvccpps = 0;

            if (_dtTableCP == null) return;

            // Chỉ chọn MaChiPhi = 3 hoặc 6
            //DataRow[] rows = _dtTableCP.Select("MaChiPhi IN (3, 6)");
            //foreach (DataRow r in rows)
            //{
            //    decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
            //    decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);

            //    decimal thuePercent = thue / 100m;
            //    decimal tien = soTien + (thuePercent * soTien);

            //    _tongvccpps += tien;
            //}

            DataRow[] rows = _dtTableCP.Select("MaChiPhi IN (3, 6)");

            if (rows.Length > 0)
            {
                foreach (DataRow r in rows)
                {
                    string donViTienTe = r["DonViTienTe"]?.ToString();
                    if (string.IsNullOrEmpty(donViTienTe))
                        continue;

                    // Lấy tỷ giá theo từng dòng
                    string urldvtt = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={donViTienTe}&para2=VND&para3=&para4=&para5=";
                    string jsondvtt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvtt); }).Result;
                    DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(jsondvtt);

                    decimal tygiavnd = 0;
                    if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                    {
                        tygiavnd = GetDecimalFromObject(tblTyGia.Rows[0]["TyGia"]);
                    }

                    decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                    decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);
                    decimal sotienquydoi = soTien * tygiavnd;
                    decimal thuePercent = thue / 100m;
                    decimal tien = sotienquydoi + (thuePercent * sotienquydoi);
                    decimal tienqd = tien / GetDecimalFromObject(spinEditTyGia.EditValue);
                    _tongvccpps += tienqd;
                }
            }
            else
            {
                _tongvccpps = 0;
            }
        }



        private decimal GetTongSoLuongFromGridView1(
        string maVT, string mauVT, string khoVT)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                DataRow r = gridView1.GetDataRow(i);
                if (r == null) continue;

                if (r["MaVTID"]?.ToString() == maVT &&
                    r["MauVTID"]?.ToString() == mauVT &&
                    r["KhoVaiID"]?.ToString() == khoVT)
                {
                    return Convert.ToDecimal(r["TongSLMuaThem"] ?? 0);
                }
            }
            return 0;
        }

        public enum ReCalcResult
        {
            Success,
            VuotTong,
            InvalidEdit
        }

        private ReCalcResult ReCalcSoLuongVC(
            string maVTID,
            string mauVTID,
            string khoVTID,
            string maNhomVT,
            bool fromCellEdit,
            int? thuTuDangSua,
            decimal? newValDangSua,
            decimal? oldValDangSua,
            out decimal tong
        )
        {
            tong = LayTongTuGridView1(maVTID, mauVTID, khoVTID, maNhomVT);

            var dots = _dtTableDot.AsEnumerable()
                .Where(r =>
                    r["MaVTID"]?.ToString() == maVTID &&
                    r["MaMauVT"]?.ToString() == mauVTID &&
                    r["MaKhoVT"]?.ToString() == khoVTID &&
                    r["MaNhomVT"]?.ToString() == maNhomVT
                )
                .OrderBy(r => Convert.ToInt32(r["ThuTu"]))
                .ToList();

            if (dots.Count < 2)
                return ReCalcResult.Success;

            var lastDot = dots.Last();
            int thuTuLast = Convert.ToInt32(lastDot["ThuTu"]);

            if (fromCellEdit && thuTuDangSua == thuTuLast)
                return ReCalcResult.InvalidEdit;

            decimal? sumBeforeLast = 0;

            foreach (var r in dots)
            {
                int t = Convert.ToInt32(r["ThuTu"]);
                if (t == thuTuLast) continue;

                if (fromCellEdit && t == thuTuDangSua)
                    sumBeforeLast += newValDangSua;
                else
                    sumBeforeLast += Convert.ToDecimal(r["SoLuongVC"] ?? 0);
            }

            if (sumBeforeLast > tong)
                return ReCalcResult.VuotTong;

            decimal? slLast = tong - sumBeforeLast;
            if (slLast < 0) slLast = 0;

            lastDot["SoLuongVC"] = slLast;

            CapNhatThanhTien_ByMaVT(maVTID, mauVTID, khoVTID, maNhomVT);

            return ReCalcResult.Success;
        }



        private decimal LayTongTuGridView1(string maVTID, string mauVTID, string khoVTID, string maNhomVT)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if (
                    gridView1.GetRowCellValue(i, "MaVTID")?.ToString() == maVTID &&
                    gridView1.GetRowCellValue(i, "MauVTID")?.ToString() == mauVTID &&
                    gridView1.GetRowCellValue(i, "KhoVaiID")?.ToString() == khoVTID &&
                    gridView1.GetRowCellValue(i, "MaCLVT")?.ToString() == maNhomVT
                )
                {
                    decimal.TryParse(
                        gridView1.GetRowCellValue(i, "TongSLMuaThem")?.ToString(),
                        out decimal tong);
                    return tong;
                }
            }
            return 0;
        }

        private void CreateSearchlookupTienTe()
        {
            string url = $"{URL}NhaCC/GePMH?action=GetALLTienTe&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            txtTienTe.Properties.DataSource = tbl;
            txtTienTe.Properties.ValueMember = "MaTienTe";
            txtTienTe.Properties.DisplayMember = "MaTienTe";
            if (tbl != null && tbl.Rows.Count > 0)
            {
                DataRow[] rows = tbl.Select("MaTienTe = 'VND'");
                if (rows.Length > 0)
                {
                    string matiente = rows[0]["MaTienTe"].ToString();
                    //txtTienTe.EditValue = matiente;
                }

            }
        }

        private void txtTienTe_Properties_EditValueChanged(object sender, EventArgs e)
        {
            string matiente = txtTienTe.EditValue == null ? "" : txtTienTe.EditValue.ToString();
            string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _isLoadingTyGia = true;
            decimal tyGiaMoi = 1;
            if (tbl != null && tbl.Rows.Count > 0)
            {
                if (_IsEdit == true || _IsView == true)
                {
                    string urltgtt = $"{URL}NhaCC/GePMH?action=GetTyGiaThanhToan&para1={_maphieumh}&para2=&para3=&para4=&para5=";
                    string jsontgtt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urltgtt); }).Result;
                    DataTable tbltgtt = JsonConvert.DeserializeObject<DataTable>(jsontgtt);
                    if (_IsXacNhan)
                    {
                        spinEditTyGia.EditValue = tbltgtt.Rows[0]["TyGiaThanhToan"];
                    }
                    else
                    {
                        spinEditTyGia.EditValue = tbl.Rows[0]["TyGia"];
                    }
                }
                else
                {
                    spinEditTyGia.EditValue = tbl.Rows[0]["TyGia"];
                }

                //_tienteID = tbl.Rows[0]["MaTienTe"].ToString();
            }
            tyGiaMoi = GetDecimalFromObject(spinEditTyGia.EditValue);
            CapNhatThanhTienQDChoGrid(tyGiaMoi);
            _isLoadingTyGia = false;
            ApplyCurrencyFormat(spinEditTongCPVT, txtTienTe.Text);
            ApplyCurrencyFormat(spinEditTongCP, txtTienTe.Text);
            ApplyCurrencyFormat(spinEditTongCPVC, txtTienTe.Text);
            ApplyCurrencyFormat(spinEditTongCPPS, txtTienTe.Text);
            gridColumn73.Caption = $"Total converted amount ({txtTienTe.Text})";
            UpdateGridView5Summary();
            UpdateGridView5Values();
        }

        private void CapNhatThanhTienQDChoGrid(decimal tyGiaMoi)
        {
            GridView view = gridView1;

            view.BeginUpdate();
            try
            {
                for (int i = 0; i < view.RowCount; i++)
                {
                    decimal thanhTienVND = GetDecimalValue(view, i, "ThanhTienVND");

                    decimal thanhTienQD = tyGiaMoi == 0
                        ? thanhTienVND
                        : thanhTienVND / tyGiaMoi;

                    view.SetRowCellValue(i, "ThanhTienQD", Math.Round(thanhTienQD, 2));
                }
            }
            finally
            {
                view.EndUpdate();
            }

            UpdateGridView5Summary();
            UpdateGridView5Values();
        }

        private void CreateSearchlookupcolTienTe()
        {
            string url = $"{URL}NhaCC/GePMH?action=GetALLTienTe&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdittt = new RepositoryItemSearchLookUpEdit();
            rCountryEdittt.DataSource = tbl;
            rCountryEdittt.DisplayMember = "MaTienTe";
            rCountryEdittt.ValueMember = "MaTienTe";
            rCountryEdittt.ShowClearButton = false;
            rCountryEdittt.NullText = "[Chọn giá trị]";

            GridView view = rCountryEdittt.View;
            if (view.Columns.Count == 0)
            {
                view.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                view.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                view.Appearance.HeaderPanel.Options.UseTextOptions = true;
                GridColumn colMa = new GridColumn
                {
                    FieldName = "MaTienTe",
                    Caption = "Mã Tiền Tệ",
                    Visible = true
                };
                view.Columns.Add(colMa);

                GridColumn colThanhToan = new GridColumn
                {
                    FieldName = "TienTe",
                    Caption = "Tiền Tệ",
                    Visible = true,
                    Width = 200
                };
                view.Columns.Add(colThanhToan);
                colThanhToan.VisibleIndex = 0;

                GridColumn colGhiChu = new GridColumn
                {
                    FieldName = "KyHieu",
                    Caption = "Ký Hiệu",
                    Visible = true,
                    Width = 200
                };
                view.Columns.Add(colGhiChu);
                colGhiChu.VisibleIndex = 1;

                // Tắt auto-resize tránh nhảy cột
                view.OptionsView.ColumnAutoWidth = false;

            }
            gridColumn72.ColumnEdit = rCountryEdittt;
        }

        private bool KiemTraDotSoLuongBang0(DataTable dtDot)
        {
            if (dtDot == null || dtDot.Rows.Count == 0)
                return false;

            var dotsBang0 = dtDot.AsEnumerable()
                .Where(r => r["Thutu"] != DBNull.Value)
                .GroupBy(r => Convert.ToInt32(r["Thutu"]))
                .Select(g => new
                {
                    ThuTu = g.Key,
                    TongSL = g.Sum(x => x["SoLuongVC"] == DBNull.Value
                                        ? 0
                                        : Convert.ToDecimal(x["SoLuongVC"]))
                })
                .Where(x => x.TongSL == 0)
                .ToList();

            if (dotsBang0.Any())
            {
                string danhSachDot = string.Join(", ", dotsBang0.Select(x => x.ThuTu));

                XtraMessageBox.Show(
                    $"Đợt [{danhSachDot}] có tổng số lượng = 0. Vui lòng kiểm tra lại!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return true;
            }

            return false;
        }

        private bool KiemTraDotSoTienbang0(DataTable dtcp)
        {
            if (dtcp == null || dtcp.Rows.Count == 0)
                return false;

            // Commit dữ liệu editor trước khi kiểm tra
            gridView2.PostEditor();
            gridView2.UpdateCurrentRow();

            var rowsSai = dtcp.AsEnumerable()
                            .Where(r =>
                                r.RowState != DataRowState.Deleted &&
                                (r["SoTien"] == DBNull.Value ||
                                 Convert.ToDecimal(r["SoTien"]) == 0)
                            )
                            .ToList();

            if (rowsSai.Any())
            {
                MessageBox.Show(
                    "Tồn tại dòng chi phí có Số tiền = 0.\nVui lòng kiểm tra lại trước khi lưu!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                // Focus vào dòng lỗi đầu tiên
                int rowIndex = dtcp.Rows.IndexOf(rowsSai.First());
                if (rowIndex >= 0)
                {
                    gridView2.FocusedRowHandle = rowIndex;
                    gridView2.FocusedColumn = gridView2.Columns["SoTien"];
                    gridView2.ShowEditor();
                }

                return true;
            }

            return false;
        }

        private void gridView5_ShownEditor(object sender, EventArgs e)
        {
            var view = sender as GridView;
            if (view.FocusedColumn.FieldName == "PhanTram" || view.FocusedColumn.FieldName == "SoTien")
            {
                decimal.TryParse(view.GetFocusedRowCellValue("PhanTram")?.ToString(), out _oldPhanTram);
                decimal.TryParse(view.GetFocusedRowCellValue("SoTien")?.ToString(), out _oldSoTien);
            }
        }

        public void LoadCopyData(
        DataTable dtChiTiet,
        DataTable dtChiPhi,
        DataTable dtDot,
        DataTable dtHTTT,
        //string maphieubgpmh,
        string manccpmh)
        {
            CreateSearchlookupNCC();
            SetupNumericColumns();
            CreateSearchlookupTienTe();
            CreateSearchlookupcolPTThanhToanHT();
            //CreateSearchlookupPhieuBG();
            txtPhieuMH.Text = "";
            DEditNgayTao.EditValue = DateTime.Now;
            txtNguoiTao.Text = GlobleData.UserName;

            _IsEdit = false;
            _IsAdd = true;
            searchLookUpEditNCC.EditValue = manccpmh;
            //searchLookUpEditPBG.EditValue = maphieubgpmh;

            if (!dtChiTiet.Columns.Contains("ThanhTienVND"))
                dtChiTiet.Columns.Add("ThanhTienVND", typeof(decimal));

            if (!dtChiTiet.Columns.Contains("TGGiao"))
                dtChiTiet.Columns.Add("TGGiao", typeof(DateTime));

            if (!dtChiTiet.Columns.Contains("ThanhTienQD"))
                dtChiTiet.Columns.Add("ThanhTienQD", typeof(decimal));

            _dtTable = CreateDatatableload();
            MapDataFromApi(dtChiTiet, _dtTable);
            // ===== DETAIL =====
            foreach (DataRow r in _dtTable.Rows)
            {
                r["MaPhieuMH"] = "";
                r["IsDuyet"] = false;
                r["NgayTao"] = DateTime.Now;
                r["NguoiTao"] = GlobleData.UserName;
                decimal ThanhTien = GetDecimalFromObject(r["ThanhTien"]);

                string mancc = r["MaNCC"]?.ToString() ?? "";

                string maTienTe = r["TienTeID"]?.ToString() ?? "VND";

                decimal gia = 1;

                if (!string.IsNullOrEmpty(mancc) && maTienTe != "VND")
                {
                    string url1 = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={maTienTe}&para2={"VND"}&para3=&para4=&para5=";

                    string json1 = Task.Run(async () => await _clientExtension.GetAsnyc(url1)).Result;

                    DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(json1);

                    if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        gia = Convert.ToDecimal(tblTyGia.Rows[0]["TyGia"]);
                }
                decimal tygia = GetDecimalFromObject(spinEditTyGia.EditValue);
                decimal thanhtienvnd = ThanhTien * gia;
                if (maTienTe == "VND")
                    r["ThanhTienVND"] = thanhtienvnd;
                else
                    r["ThanhTienVND"] = thanhtienvnd;

                r["ThanhTienQD"] = thanhtienvnd / tygia;
                r["NgayGiaoHangYC"] = DBNull.Value;
            }
            //_dtTable = dtChiTiet.Copy();
            gridControl1.DataSource = _dtTable;

            if (dtChiPhi != null)
            {
                _dtTableCP = dtChiPhi.Copy();
                gridControl2.DataSource = _dtTableCP;
            }

            if (dtDot != null)
            {
                foreach (DataRow r in dtDot.Rows)
                {
                    //r["MaPhieu"] = "";
                    r["NgayTao"] = DateTime.Now;
                    r["NguoiTao"] = GlobleData.UserName;
                }
                _dtTableDot = dtDot.Copy();
                gridControl3.DataSource = _dtTableDot;
            }

            if (dtHTTT != null)
            {
                foreach (DataRow r in dtHTTT.Rows)
                {
                    r["MaPhieuMH"] = "";
                }
                _dtTableHTTT = dtHTTT.Copy();
                gridControl4.DataSource = _dtTableHTTT;
            }
            UpdateGridView5Summary();
            UpdateGridView5Values();

        }

        private void InitPopupTTChietKhau()
        {
            popupTTChietKhau = new PopupMenu();
            popupTTChietKhau.Manager = barManager1; // form của bạn đã có BarManager

            btnApplyTTChietKhau = new BarButtonItem(barManager1,
                "Áp dụng cho toàn cột");

            btnApplyTTChietKhau.ItemClick += BtnApplyTTChietKhau_ItemClick;

            popupTTChietKhau.AddItem(btnApplyTTChietKhau);
        }

        private void BtnApplyTTChietKhau_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_popupColumn == null)
                return;

            GridView view = gridView1;

            object value = view.GetFocusedRowCellValue(_popupColumn);

            if (value == null || value == DBNull.Value)
                return;

            view.BeginUpdate();
            try
            {
                for (int i = 0; i < view.RowCount; i++)
                {
                    if (view.IsGroupRow(i)) continue;

                    view.SetRowCellValue(i, _popupColumn, value);
                }
            }
            finally
            {
                view.EndUpdate();
            }

            // Giữ nguyên logic hiện tại của bạn
            UpdateGridView5Summary();
            UpdateGridView5Values();
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (_IsView == true) return;

            if (e.MenuType != GridMenuType.Row)
                return;

            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Point);

            if (!hitInfo.InRowCell)
                return;

            // ❗ Lưu lại cột đang click
            _popupColumn = hitInfo.Column;

            // (OPTIONAL) Chỉ cho phép 1 số cột
            if (!IsAllowApplyColumn(_popupColumn))
                return;

            view.FocusedRowHandle = hitInfo.RowHandle;
            view.FocusedColumn = _popupColumn;

            // Đổi text menu theo tên cột
            btnApplyTTChietKhau.Caption =
                $"Áp dụng cho toàn bộ cột [{_popupColumn.Caption}]";

            popupTTChietKhau.ShowPopup(Control.MousePosition);
        }

        private bool IsAllowApplyColumn(GridColumn col)
        {
            string[] allowFields =
            {
                "TTChietKhau",
                "ChietKhau",
                "Thue",
                "TongSLMuaThem",
                "NgayGiaoHangYC"
            };

            return allowFields.Contains(col.FieldName);
        }

        private void MapDataFromApi(DataTable apiTable, DataTable targetTable)
        {
            targetTable.BeginLoadData();

            foreach (DataRow srcRow in apiTable.Rows)
            {
                DataRow destRow = targetTable.NewRow();

                foreach (DataColumn destCol in targetTable.Columns)
                {
                    // API không có cột này thì bỏ qua
                    if (!apiTable.Columns.Contains(destCol.ColumnName))
                        continue;

                    object value = srcRow[destCol.ColumnName];

                    // XỬ LÝ RIÊNG CHO DATETIME
                    if (destCol.DataType == typeof(DateTime))
                    {
                        if (value == DBNull.Value ||
                            string.IsNullOrWhiteSpace(value?.ToString()))
                        {
                            destRow[destCol.ColumnName] = DBNull.Value;
                        }
                        else if (value is DateTime dt)
                        {
                            destRow[destCol.ColumnName] = dt;
                        }
                        else
                        {
                            string[] formats =
                                    {
                                "MM/dd/yyyy HH:mm:ss",
                                "MM/dd/yyyy",
                                "yyyy-MM-dd HH:mm:ss",
                                "yyyy-MM-dd",
                                "dd/MM/yyyy HH:mm:ss",
                                "dd/MM/yyyy"
                            };

                            if (DateTime.TryParseExact(
                                    value.ToString(),
                                    formats,
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime parsed))
                            {
                                destRow[destCol.ColumnName] = parsed;
                            }
                            else
                            {
                                destRow[destCol.ColumnName] = DBNull.Value;
                            }
                        }
                    }
                    else
                    {
                        destRow[destCol.ColumnName] = value ?? DBNull.Value;
                    }
                }

                targetTable.Rows.Add(destRow);
            }

            targetTable.EndLoadData();
        }


        private void GeneratePhieuMH()
        {
            string url = $"{URL}NhaCC/GePMH?action=GeneratePhieuMH&para1=NONE&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl?.Rows?.Count > 0)
            {
                DataRow rowPhieuMua = tbl.Rows[0];
                _tenphieumh = rowPhieuMua["NextTenPhieu"]?.ToString();
                _pomua = rowPhieuMua["NextPOMua"]?.ToString();
                _maphieumh = rowPhieuMua["NextMaPhieuMH"]?.ToString();


            }
        }


        #region E-Way-Bill

        private void LoadE_Invoice()
        {
            string url = $"{URL}NhaCC/GePMH?action=GetEInvoice&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if(tbl != null && tbl?.Rows?.Count > 0)
            {
                DataRow rowEInvoice = tbl.Rows[0];

                //clsForrmatUtils.ConvertDate(rowPhieu["NgayBDHieuLuc"]?.ToString())
                txtInVoiceNo.EditValue = rowEInvoice["SoHoaDon"];
                txtCustomsDeclarationNo.EditValue = rowEInvoice["SoToKhai"];
                txtContractNo.EditValue = rowEInvoice["SoHopDong"];

                DateTime InvoiceDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayHoaDon"]?.ToString());
                if(InvoiceDate != DateTime.MinValue)
                {
                    DateEditInvoice.EditValue = InvoiceDate;
                }
                DateTime DeclarationDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayMoToKhai"]?.ToString());
                if(DeclarationDate != DateTime.MinValue)
                {
                    DateEditDeclaration.EditValue = DeclarationDate;
                }
                DateTime ContractDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayKyHD"]?.ToString());
                if(ContractDate != DateTime.MinValue)
                {
                    DateEditContract.EditValue = ContractDate;
                }
              
                txtVatNum.EditValue = rowEInvoice["MaSoThue"];
                txtBillOfLading.EditValue = rowEInvoice["SoVanDon"];
                txtEWayBill.EditValue = rowEInvoice["E_Way_Bill"];
                txtShipped.EditValue = rowEInvoice["NoiGui"];
                DateTime ShippedDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayGui"]?.ToString());
                if(ShippedDate != DateTime.MinValue)
                {
                    DateEditShipped.EditValue = ShippedDate;
                }
             
                SearchLookupPort.EditValue = rowEInvoice["Cang"];
                SeachLookupVessel.EditValue = rowEInvoice["Tau"];
            }
        }

        private void InitPaymentTerm()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPTThanhToan&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json);


            SearchLookupPayment.Properties.DataSource = _tbldv;
            SearchLookupPayment.Properties.DisplayMember = "ThanhToan";
            SearchLookupPayment.Properties.ValueMember = "MaPTThanhToan";
            SearchLookupPayment.Properties.ShowClearButton = false;
            SearchLookupPayment.Properties.NullText = "[Chọn giá trị]";

       
            //invoice to
            string urliv = $"{URL}NhaCC/GetInvoice?action=GETINVOICE&para=&para1=&para2=&para3=&para4=&para5=";
            string jsoniv = Task.Run(async () => { return await _clientExtension.GetAsnyc(urliv); }).Result;
            DataTable _tbliv = JsonConvert.DeserializeObject<DataTable>(jsoniv);

            SearchLookupInvoice.Properties.DataSource = _tbliv;
            SearchLookupInvoice.Properties.DisplayMember = "Company";
            SearchLookupInvoice.Properties.ValueMember = "CompanyID";
            SearchLookupInvoice.Properties.ShowClearButton = false;
            SearchLookupInvoice.Properties.NullText = "[Chọn giá trị]";

         
        }
        DataTable tblCang = new DataTable();
        DataTable tblTau = new DataTable();
        string macAddress = "";
        string ipAddress = "";
        //Environment.MachineName;
        private void InitE_Invoice()
        {
            string urlKhachHang = $"{URL}NhaCC/GePMH?action=GetKhachHang&para1=&para2=&para3=&para4=&para5=";
            string jsonKhachHang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhachHang); }).Result;
            DataTable tblKhachHang = JsonConvert.DeserializeObject<DataTable>(jsonKhachHang);


            SeachLookupKhachHang.Properties.DataSource = tblKhachHang;
            SeachLookupKhachHang.Properties.DisplayMember = "TenKH";
            SeachLookupKhachHang.Properties.ValueMember = "MaKH";
            SeachLookupKhachHang.Properties.ShowClearButton = false;
            SeachLookupKhachHang.Properties.NullText = "[Chọn giá trị]";


            string urlCang = $"{URL}NhaCC/GePMH?action=GETCANG&para1=&para2=&para3=&para4=&para5=";
            string jsonCang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCang); }).Result;
            tblCang = JsonConvert.DeserializeObject<DataTable>(jsonCang);

            SearchLookupPort.Properties.DataSource = tblCang;
            SearchLookupPort.Properties.DisplayMember = "TenCang";
            SearchLookupPort.Properties.ValueMember = "MaCang";
            SearchLookupPort.Properties.ShowClearButton = false;
            SearchLookupPort.Properties.NullText = "[Chọn giá trị]";
            SearchLookupPort.Popup += ItemEditCang_Popup;


            string urlTau = $"{URL}NhaCC/GePMH?action=GETTAU&para1=&para2=&para3=&para4=&para5=";
            string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
            tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);


            SeachLookupVessel.Properties.DataSource = tblTau;
            SeachLookupVessel.Properties.DisplayMember = "TenTau";
            SeachLookupVessel.Properties.ValueMember = "MaTau";
            SeachLookupVessel.Properties.ShowClearButton = false;
            SeachLookupVessel.Properties.NullText = "[Chọn giá trị]";
            SeachLookupVessel.Popup += ItemEdit_Tau_Popup;
        }

        /*Sự kiện chọn or nhập cảng*/
        private void ItemEditCang_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddCang"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // 2 cột: TextEdit (%) + Button (AutoSize)
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // 4 hàng:
                    // Row 0 - empty top
                    // Row 1 - Tên cảng + Button Thêm
                    // Row 2 - Địa chỉ (span 2 cột)
                    // Row 3 - empty bottom
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 }); // Row 0: top empty
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });             // Row 1: tên cảng + button
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });             // Row 2: địa chỉ
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 }); // Row 3: bottom empty

                    layout.Root.AddItem(buttonGroup);

                    // ── ROW 1, COL 0: TextEdit Tên cảng ──
                    var txtCangNew = new TextEdit()
                    {
                        Name = "txtCangNew",
                        Properties =
                {
                    NullText = "Nhập tên cảng mới...",
                    NullValuePrompt = "Nhập tên cảng mới..."
                }
                    };
                    txtCangNew.Tag = ownerEdit;

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtCangNew,
                        Text = "Tên cảng:",
                        TextVisible = true,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    // ── ROW 1, COL 1: Button Thêm cảng ──
                    var btnAddCang = new SimpleButton()
                    {
                        Name = "btnAddCang",
                        Text = "Thêm cảng"
                    };
                    btnAddCang.Tag = txtCangNew;
                    btnAddCang.Click += btnAddCang_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddCang,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(100, 30),
                        MaxSize = new Size(100, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);

                    // ── ROW 2, COL 0-1: TextEdit Địa chỉ (span 2 cột) ──
                    var txtDiaChi = new TextEdit()
                    {
                        Name = "txtDiaChi",
                        Properties =
                {
                    NullText = "Nhập địa chỉ...",
                    NullValuePrompt = "Nhập địa chỉ..."
                }
                    };

                    var layoutItemDiaChi = new LayoutControlItem()
                    {
                        Control = txtDiaChi,
                        Text = "Địa chỉ:",
                        TextVisible = true,
                        TextLocation = DevExpress.Utils.Locations.Left,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemDiaChi.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemDiaChi.OptionsTableLayoutItem.RowIndex = 2;
                    layoutItemDiaChi.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(layoutItemDiaChi);

                    // ── ROW 0: Empty space top ──
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    // ── ROW 3: Empty space bottom ──
                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 3;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    // Enter trên txtCangNew → click button
                    txtCangNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddCang.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    var ControlTextEdit = layout?.GetControlByName("txtCangNew");
                    if (ControlTextEdit != null) ControlTextEdit.Text = "";

                    var ControlDiaChi = layout?.GetControlByName("txtDiaChi");
                    if (ControlDiaChi != null) ControlDiaChi.Text = "";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddCang_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtCangNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtCangNew?.Tag as SearchLookUpEdit;

                if (txtCangNew != null && searchLookUpEdit != null)
                {
                    string tenCang = txtCangNew.Text.Trim();
                    string maCang = string.Empty;

                    if (string.IsNullOrWhiteSpace(tenCang))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên cảng!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCangNew.Focus();
                        return;
                    }

                    // Lấy txtDiaChi từ cùng parent với txtCangNew
                    var txtDiaChi = txtCangNew.Parent?.Controls
                        .OfType<TextEdit>()
                        .FirstOrDefault(c => c.Name == "txtDiaChi");

                    string diaChi = txtDiaChi?.Text.Trim() ?? string.Empty;

                    if (tblCang != null && tblCang.Rows.Count > 0)
                    {
                        var existingRow = tblCang.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenCang")) &&
                                r.Field<string>("TenCang").Trim().Equals(tenCang, StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            maCang = existingRow.Field<string>("MaCang");
                        }
                        else
                        {
                            maCang = LuuCangMoi(tenCang, diaChi); // truyền thêm diaChi
                        }
                    }

                    if (!string.IsNullOrEmpty(maCang))
                    {
                        SearchLookupPort.Properties.DataSource = tblCang;
                        SearchLookupPort.EditValue = maCang;
                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu cảng!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string LuuCangMoi(string tenCang,string diaChi)
        {
            try
            {
               
                string url = string.Format("{0}", URL + "NhaCC/PostCang");
                DataTable tblThuVien = new DataTable("tblThuVien");
                tblThuVien.Columns.Add("ID", typeof(int));
                tblThuVien.Columns.Add("MaCang", typeof(string));
                tblThuVien.Columns.Add("TenCang", typeof(string));
                tblThuVien.Columns.Add("DiaChi", typeof(string));
                tblThuVien.Columns.Add("GhiChu", typeof(string));

                DataRow row = tblThuVien.NewRow();
                row["ID"] = 0;
                row["TenCang"] = tenCang.Trim();
                row["MaCang"] = "";
                row["DiaChi"] = diaChi;
                row["GhiChu"] = "";
                tblThuVien.Rows.Add(row);
                
                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblThuVien);
                }).Result;
             
                if (result?.ToLower() == "true")
                {

                    string urlCang = $"{URL}NhaCC/GePMH?action=GETCANG&para1=&para2=&para3=&para4=&para5=";
                    string jsonCang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCang); }).Result;
                    tblCang = JsonConvert.DeserializeObject<DataTable>(jsonCang);

                    if (tblCang != null && tblCang.Rows.Count > 0)
                    {
                        var newRow = tblCang.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenCang")) &&
                                r.Field<string>("TenCang").Trim().Equals(tenCang.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            return newRow.Field<string>("MaCang");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu cảng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /*Sự kiện chọn or nhập tàu */

        private void ItemEdit_Tau_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddTau"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // 2 cột: TextEdit (%) + Button (AutoSize)
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // 5 hàng:
                    // Row 0 - empty top
                    // Row 1 - Tên tàu + Button Thêm
                    // Row 2 - Ghi chú (span 2 cột)
                    // Row 3 - empty bottom
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });  // Row 0: top empty
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });              // Row 1: tên tàu + button
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });              // Row 2: ghi chú
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 }); // Row 3: bottom empty

                    layout.Root.AddItem(buttonGroup);

                    // ── ROW 1, COL 0: TextEdit Tên tàu ──
                    var txtTauNew = new TextEdit()
                    {
                        Name = "txtTauNew",
                        Properties =
                {
                    NullText = "Nhập tên tàu mới...",
                    NullValuePrompt = "Nhập tên tàu mới..."
                }
                    };
                    txtTauNew.Tag = ownerEdit;

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtTauNew,
                        Text = "Tên tàu:",
                        TextVisible = true,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    // ── ROW 1, COL 1: Button Thêm tàu ──
                    var btnAddTau = new SimpleButton()
                    {
                        Name = "btnAddTau",
                        Text = "Thêm tàu"
                    };
                    btnAddTau.Tag = txtTauNew;
                    btnAddTau.Click += btnAddTau_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddTau,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(100, 30),
                        MaxSize = new Size(100, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);

                   
                    var txtDiaChi = new TextEdit()
                    {
                        Name = "txtDiaChi",
                        Properties =
                {
                    NullText = "Nhập địa chỉ...",
                    NullValuePrompt = "Nhập  địa chỉ..."
                }
                    };

                    var layoutItemGhiChu = new LayoutControlItem()
                    {
                        Control = txtDiaChi,
                        Text = "Địa chỉ:",
                        TextVisible = true,
                        TextLocation = DevExpress.Utils.Locations.Left,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemGhiChu.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemGhiChu.OptionsTableLayoutItem.RowIndex = 2;
                    layoutItemGhiChu.OptionsTableLayoutItem.ColumnSpan = 2; // Span cả 2 cột
                    buttonGroup.AddItem(layoutItemGhiChu);

                    
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);

                 
                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 3;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    
                    txtTauNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddTau.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    var ControlTextEdit = layout?.GetControlByName("txtTauNew");
                    if (ControlTextEdit != null) ControlTextEdit.Text = "";

                    var ControlGhiChu = layout?.GetControlByName("txtDiaChi");
                    if (ControlGhiChu != null) ControlGhiChu.Text = "";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddTau_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtTauNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtTauNew?.Tag as SearchLookUpEdit;

                if (txtTauNew != null && searchLookUpEdit != null)
                {
                    string tenTau = txtTauNew.Text.Trim();
                    string maTau = string.Empty;

                    if (string.IsNullOrWhiteSpace(tenTau))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên tàu!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTauNew.Focus();
                        return;
                    }

                 
                    var txtDiaChi = txtTauNew.Parent?.Controls
                        .OfType<TextEdit>()
                        .FirstOrDefault(c => c.Name == "txtDiaChi");

                    string DiaChi = txtDiaChi?.Text.Trim() ?? string.Empty;
                   
                    if (tblTau != null && tblTau.Rows.Count > 0)
                    {
                        var existingRow = tblTau.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenTau")) &&
                                r.Field<string>("TenTau").Trim().Equals(tenTau, StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            maTau = existingRow.Field<string>("MaTau");
                        }
                        else
                        {
                            maTau = LuuTauMoi(tenTau, DiaChi); 
                        }
                    }

                    if (!string.IsNullOrEmpty(maTau))
                    {
                        SeachLookupVessel.Properties.DataSource = tblTau;
                        SeachLookupVessel.EditValue = maTau;
                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu tàu!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private string LuuTauMoi(string Tentau,string DiaChi)
        {
            try
            {


                // Bước 2: Gọi API lưu thuế mới
                string url = string.Format("{0}", URL + "NhaCC/PostTau");
                DataTable tblThuVien = new DataTable();
                tblThuVien = new DataTable("tblThuVien");
                tblThuVien.Columns.Add("ID", typeof(int));
                tblThuVien.Columns.Add("MaTau", typeof(string));
                tblThuVien.Columns.Add("TenTau", typeof(string));
                tblThuVien.Columns.Add("DiaChi", typeof(string));
                tblThuVien.Columns.Add("GhiChu", typeof(string));

                DataRow row = tblThuVien.NewRow();
                row["ID"] = 0;
                row["TenTau"] = Tentau.Trim();
                row["MaTau"] = "";
                row["DiaChi"] = DiaChi;
                row["GhiChu"] = "";
                tblThuVien.Rows.Add(row);       
                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblThuVien);
                }).Result;
           

                if (result?.ToLower() == "true")
                {

                    string urlTau = $"{URL}NhaCC/GePMH?action=GETTAU&para1=&para2=&para3=&para4=&para5=";
                    string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
                    tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);

                    if (tblTau != null && tblTau.Rows.Count > 0)
                    {
                        var newRow = tblTau.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenTau")) &&
                                r.Field<string>("TenTau").Trim().Equals(Tentau.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            return newRow.Field<string>("MaTau");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
              
                return null;
            }
        }

        private void CreateDefaultDateEdit()
        {
            SetDateEdit(DateEditInvoice);
            SetDateEdit(DateEditDeclaration);
            SetDateEdit(DateEditContract);
            SetDateEdit(DateEditShipped);
        }

        private void SetDateEdit(DevExpress.XtraEditors.DateEdit DateEditControl)
        {
            DateEditControl.Properties.DisplayFormat.FormatString = "dd-MM-yyyy";
            DateEditControl.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            DateEditControl.Properties.EditFormat.FormatString = "dd-MM-yyyy";
            DateEditControl.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;


            DateEditControl.Properties.Mask.EditMask = "dd-MM-yyyy";
            DateEditControl.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            DateEditControl.Properties.Mask.UseMaskAsDisplayFormat = true;
        }
        private void splitContainerControl_Paint(object sender, PaintEventArgs e)
        {
            //splitContainerControl4.SplitterPosition = (int)(splitContainerControl4.Width * 1);
        }
        #endregion
        #region Save
        public void SaveDataHTTT()
        {
            try
            {
                this.ActiveControl = simpleButton2;
                DataTable tblHT = CreateDatatableHTTTSave();
                DataRow newRow = tblHT.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["SoTien"] = spinEditTongCPVND.EditValue;
                newRow["TGTra"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["DotTra"] = 1;
                newRow["GhiChu"] = txtRemarkPayment.EditValue;
                newRow["PhanTram"] = 100;
                newRow["PTThanhToan"] = SearchLookupPayment.EditValue;
                newRow["InvoiceTo"] = SearchLookupInvoice.EditValue;
                newRow["Incoterm"] = txtIncoterm.EditValue;
                newRow["UserName"] = GlobleData.UserName;
                newRow["CreaDate"] = DateTime.Now;
                newRow["Mac"] = macAddress;
                newRow["IPAdress"] = ipAddress;
                newRow["MachineName"] = Environment.MachineName;
                tblHT.Rows.Add(newRow);

                // GỌI API ĐỂ LƯU PHIẾU
                string url = string.Format("{0}?", URL + "NhaCC/PostHT");
                string mss = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblHT);
                }).Result;

                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }
            catch(Exception ex)
            {

            }
           
        }

        public void Save_E_Invoice()
        {
            try
            {
                DataTable tblSaveEInvoice = CreateTable_E_Invoice();

                DataRow rowEInvoice = tblSaveEInvoice.NewRow();

                rowEInvoice["ID"] = 0;
                rowEInvoice["MaPhieuMH"] = _maphieumh;
                rowEInvoice["MaKhachHang"] = SeachLookupKhachHang.EditValue;
                rowEInvoice["SoHoaDon"] = txtInVoiceNo.EditValue;
                rowEInvoice["SoToKhai"] = txtCustomsDeclarationNo.EditValue;
                rowEInvoice["SoHopDong"] = txtContractNo.EditValue;
                DateTime InvoiceDate = DateEditInvoice.EditValue == null ? DateTime.MinValue : (DateTime)DateEditInvoice.EditValue;
                rowEInvoice["NgayHoaDon"] = InvoiceDate == DateTime.MinValue ? null : InvoiceDate.ToString("yyyy-MM-dd 00:00:00");
                DateTime DeclarationDate = DateEditDeclaration.EditValue == null ? DateTime.MinValue : (DateTime)DateEditDeclaration.EditValue;
                rowEInvoice["NgayMoToKhai"] = DeclarationDate == DateTime.MinValue ? null : DeclarationDate.ToString("yyyy-MM-dd 00:00:00");
                DateTime ContractDate = DateEditContract.EditValue == null ? DateTime.MinValue : (DateTime)DateEditContract.EditValue;
                rowEInvoice["NgayKyHD"] = ContractDate == DateTime.MinValue ? null : ContractDate.ToString("yyyy-MM-dd 00:00:00");
                rowEInvoice["MaSoThue"] = txtVatNum.EditValue;
                rowEInvoice["SoVanDon"] = txtBillOfLading.EditValue;
                rowEInvoice["E_Way_Bill"] = txtEWayBill.EditValue;
                rowEInvoice["NoiGui"] = txtShipped.EditValue;
                DateTime ShippingDate = DateEditShipped.EditValue == null ? DateTime.MinValue : (DateTime)DateEditShipped.EditValue;
                rowEInvoice["NgayGui"] = ShippingDate == DateTime.MinValue ? null : ShippingDate.ToString("yyyy-MM-dd 00:00:00"); 
                rowEInvoice["Cang"] = SearchLookupPort.EditValue;
                rowEInvoice["Tau"] = SeachLookupVessel.EditValue;
                rowEInvoice["UserName"] = GlobleData.UserName;
                rowEInvoice["CreaDate"] = DateTime.Now;
                rowEInvoice["Mac"] = macAddress;
                rowEInvoice["IPAdress"] = ipAddress;
                rowEInvoice["MachineName"] = Environment.MachineName;

                tblSaveEInvoice.Rows.Add(rowEInvoice);
                string url = string.Format("{0}?", URL + "NhaCC/PostEInvoice");
                string mss = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblSaveEInvoice);
                }).Result;

                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                   
                    string urlNhapKho = string.Format("{0}?action=Update_NhapKho", URL + "NhaCC/PostEInvoice");
                    string msgNhaKho = Task.Run(async () =>
                    {
                        return await _clientExtension.PostAsync(urlNhapKho, tblSaveEInvoice);
                    }).Result;

                    if(msgNhaKho.ToLower() != "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 3000);
                    }
                    
                }
                    
            }
            catch(Exception ex)
            {

            }
            

        }


        #endregion


        private void LoadIPAdress()
        {
            macAddress = "";
            ipAddress = "";

            // Lấy MAC + IP từ card mạng đang hoạt động (không loopback, không ảo)
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .OrderByDescending(ni => ni.Speed)
                .FirstOrDefault();

            if (networkInterface != null)
            {

                PhysicalAddress pa = networkInterface.GetPhysicalAddress();
                macAddress = string.Join("-", pa.GetAddressBytes().Select(b => b.ToString("X2")));


                var ipProps = networkInterface.GetIPProperties();
                var ipv4 = ipProps.UnicastAddresses
                    .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ipv4 != null)
                {
                    ipAddress = ipv4.Address.ToString();
                }
            }
        }



    }
}