using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class frmPhieuMuaMMTB : DevExpress.XtraEditors.XtraForm
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
        string vt = string.Empty, _maphieumh = string.Empty, _mancc = string.Empty, _maphieubg = string.Empty, _tenphieumh = string.Empty, _nguoitao = string.Empty, _tienteID = string.Empty, _pomua = string.Empty
            , _mtt = string.Empty, _maphieuyc = string.Empty;
        bool _IsEdit;
        bool _IsDuyet;
        bool _IsAdd;
        bool _IsView;
        bool _IsXacNhan;
        bool _isCopyMode = false;
        DataRow _rowFocused = null;
        DataRow _rowFocusedpyc = null;
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
        public frmPhieuMuaMMTB(DataRow rowFocused = null, DataRow rowFocusedpyc = null, bool IsEdit = false, bool IsAdd = false, bool IsView = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            _IsEdit = IsEdit;
            _IsAdd = IsAdd;
            _IsView = IsView;
            _rowFocusedpyc = rowFocusedpyc;
            _maphieuyc = _rowFocusedpyc["MaPhieu"]?.ToString();
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
                _tongCPVND = GetDecimalFromObject(_rowFocused["TongTienVND"]);
                _tongCPVND = GetDecimalFromObject(_rowFocused["TongTien"]);
                _tongCPVT = GetDecimalFromObject(_rowFocused["TongCPVT"]);
                _pomua = _rowFocused["POMua"]?.ToString();
                _IsXacNhan = Convert.ToBoolean(_rowFocused["IsXacNhan"]);
                _mtt = _rowFocused["MaTienTe"]?.ToString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchlookupNCC();
            SetupNumericColumns();
            SetupNumericIntegerColumns();
            CreateSearchlookupTienTe();
            InitPopupTTChietKhau();
            barButtonItem3.Visibility = BarItemVisibility.Never;
            barButtonItem4.Visibility = BarItemVisibility.Never;
            if (_IsEdit == true)
            {
                if (_IsDuyet)
                {
                    barButtonItem3.Visibility = BarItemVisibility.Never;
                    barButtonItem4.Visibility = BarItemVisibility.Always;
                    gridView1.OptionsBehavior.Editable = false;
                    gridView1.OptionsBehavior.ReadOnly = true;
                    textEditPOMua.Properties.ReadOnly = true;
                }
                else
                {
                    barButtonItem3.Visibility = BarItemVisibility.Always;
                    barButtonItem4.Visibility = BarItemVisibility.Never;
                }
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEditNCC.EditValue = _mancc.ToString();
                searchLookUpEditPBG.EditValue = _maphieubg.ToString();
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
                    var mavtid = gridView1.GetRowCellValue(i, "MaHang")?.ToString();
                    var mauvtid = gridView1.GetRowCellValue(i, "MaCL")?.ToString();
                    var khovaiid = gridView1.GetRowCellValue(i, "MaNCC")?.ToString();
                    //var madvvt = gridView1.GetRowCellValue(i, "MaDVVT")?.ToString();

                    //string key = string.Join("|", mavtid, mauvtid, khovaiid, madvvt);
                    string key = string.Join("|", mavtid, mauvtid, khovaiid);
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
                searchLookUpEditPBG.EditValue = _maphieubg.ToString();
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
                    var mavtid = gridView1.GetRowCellValue(i, "MaHang")?.ToString();
                    var mauvtid = gridView1.GetRowCellValue(i, "MaCL")?.ToString();
                    var khovaiid = gridView1.GetRowCellValue(i, "MaNCC")?.ToString();
                    //var madvvt = gridView1.GetRowCellValue(i, "MaDVVT")?.ToString();

                    //string key = string.Join("|", mavtid, mauvtid, khovaiid, madvvt);
                    string key = string.Join("|", mavtid, mauvtid, khovaiid);
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
            //LoadTenPhieuMua();

            if (_rowFocused != null)
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
        //private void LoadTenPhieuMua()
        //{
        //    if (_IsAdd == true)
        //    {
        //        string url1 = $"{URL}NhaCC/GePMHMMTB?action=GETALLPHIEUMHCount&para1=&para2=&para3=&para4=&para5=";
        //        string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
        //        DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

        //        //DataTable tblDG = CreateDatatableToSave();

        //        int maxPhieu = 0;

        //        if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("MaPhieuMH"))
        //        {
        //            var validValues = _tbldv.AsEnumerable()
        //                .Select(r => r["MaPhieuMH"]?.ToString())
        //                .Where(s => !string.IsNullOrWhiteSpace(s) && s.StartsWith("PHIEU_MMTB_"))
        //                .Select(s =>
        //                {
        //                    string numberPart = s.Replace("PHIEU_MMTB_", "").Trim();
        //                    int num;
        //                    return int.TryParse(numberPart, out num) ? num : 0;
        //                });

        //            if (validValues.Any())
        //                maxPhieu = validValues.Max();
        //        }

        //        int newPhieu = maxPhieu + 1;

        //        int maxDot = 0;

        //        if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("Dot"))
        //        {
        //            var validValues = _tbldv.AsEnumerable()
        //                .Select(r => r["Dot"]?.ToString())
        //                .Where(s => !string.IsNullOrWhiteSpace(s))
        //                .Select(s =>
        //                {
        //                    int num;
        //                    return int.TryParse(s, out num) ? num : 0;
        //                });

        //            if (validValues.Any())
        //                maxDot = validValues.Max();
        //        }

        //        int newDot = maxDot + 1;

        //        _tenphieumh = "PHIẾU MH MMTB|" + newPhieu;

        //        txtPhieuMH.Text = _tenphieumh;
        //    }
        //    else
        //    {
        //        txtPhieuMH.Text = _tenphieumh;
        //    }
        //}
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
                gridColumn6, gridColumn27, gridColumn50,
                gridColumn32, gridColumn29, gridColumn38, gridColumn53,
                gridColumn57, gridColumn7, gridColumn62,gridColumn51
            };

            // Gán 1 repository cho tất cả cột
            foreach (GridColumn col in numericColumns)
                col.ColumnEdit = riNumeric;
        }

        private void SetupNumericIntegerColumns()
        {
            // Tạo 1 repository dùng chung
            RepositoryItemTextEdit riNumeric = new RepositoryItemTextEdit();
            riNumeric.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            riNumeric.Mask.EditMask = "n0";
            riNumeric.Mask.UseMaskAsDisplayFormat = true;

            // Chặn số âm
            BlockNegative(riNumeric);

            // Những cột cần format numeric
            GridColumn[] numericColumns = new GridColumn[]
            {
                gridColumn4
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

        private void txtTienTe_Properties_EditValueChanged(object sender, EventArgs e)
        {
            string matiente = txtTienTe.EditValue == null ? "" : txtTienTe.EditValue.ToString();
            string url = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _isLoadingTyGia = true;
            decimal tyGiaMoi = 1;
            if (tbl != null && tbl.Rows.Count > 0)
            {
                if (_IsEdit == true || _IsView == true)
                {
                    string urltgtt = $"{URL}NhaCC/GePMHMMTB?action=GetTyGiaThanhToan&para1={_maphieumh}&para2=&para3=&para4=&para5=";
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
        private void spinEditTyGia_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (_isLoadingTyGia) return;
        }

        private void spinEditTyGia_Properties_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

        }

        private void spinEditTyGia_Properties_Leave(object sender, EventArgs e)
        {

        }

        #region Load
        private void LoadData()
        {
            string url = $"{URL}NhaCC/GePMHMMTB?action=GETPMHChiTiet&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable apiTable = JsonConvert.DeserializeObject<DataTable>(json);
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
                    string url1 = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={maTienTe}&para2={"VND"}&para3=&para4=&para5=";

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
        private void LoadDataCP()
        {
            string mapbg = searchLookUpEditPBG.EditValue.ToString();
            string url = $"{URL}NhaCC/GetCP?action=GETPBGChiPhi&para1={mapbg}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableCP = JsonConvert.DeserializeObject<DataTable>(json);
            CreateSearchlookupcolTienTe();
            gridControl2.DataSource = _dtTableCP;
            if (_dtTableCP != null && _dtTableCP.Rows.Count > 0)
            {
                decimal tygiavnd = 0;
                object matiente = _dtTableCP.Rows[0]["DonViTienTe"];
                string urldvtt = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
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
        private void LoadDataHTTT()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETHTTT&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableHTTT = JsonConvert.DeserializeObject<DataTable>(json);
        
            if (_dtTableHTTT != null && _dtTableHTTT?.Rows?.Count > 0)
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
            //        // ✔ Giữ nguyên % – tính lại tiền theo tỷ giá hiện tại
            //        decimal soTien = tongTienQD * phanTram / 100m;
            //        r["SoTien"] = Math.Round(soTien, 2);
            //    }
            //    else
            //    {
            //        // ❗ Không có % → giữ nguyên số tiền (nếu có)
            //        r["SoTien"] = GetDecimalFromObject(r["SoTien"]);
            //    }
            //}
            //gridControl4.DataSource = _dtTableHTTT;
            //if (_dtTableHTTT != null && _dtTableHTTT.Rows.Count > 0)
            //{
            //    // Cách 1: Dùng LINQ
            //    int maxThuTu = _dtTableHTTT.AsEnumerable()
            //                              .Where(row => row["DotTra"] != DBNull.Value)
            //                              .Select(row => Convert.ToInt32(row["DotTra"]))
            //                              .DefaultIfEmpty(0)
            //                              .Max();

            //    thuTuDotTraTien = thuTuDotTraTien + maxThuTu;

            //}
        }

        private void LoadDataDotGH()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPMHMMTBDOT&para1={_maphieumh}&para2=&para3=&para4=&para5=";
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
        #endregion

        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            //string mancc = searchLookUpEditNCC.EditValue.ToString();
            //string maphieubg = searchLookUpEditPBG.EditValue.ToString();
            //CreateSearchlookupVatTu(mancc, maphieubg);
            //if (_IsAdd == true )
            //{
            //    string url1 = $"{URL}NhaCC/GePMHMMTB?action=GETALLPHIEUMHCount&para1=&para2=&para3=&para4=&para5=";
            //    string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            //    DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

            //    //DataTable tblDG = CreateDatatableToSave();

            //    int maxPhieu = 0;

            //    if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("MaPhieuMH"))
            //    {
            //        var validValues = _tbldv.AsEnumerable()
            //            .Select(r => r["MaPhieuMH"]?.ToString())
            //            .Where(s => !string.IsNullOrWhiteSpace(s) && s.StartsWith("PHIEU_MMTB_"))
            //            .Select(s =>
            //            {
            //                string numberPart = s.Replace("PHIEU_MMTB_", "").Trim();
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

            //    _tenphieumh = "PHIẾU MH MMTB|" + newPhieu;

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
            string url = $"{URL}NhaCC/GePMHMMTB?action=GetPhieuBG1&para1={mancc}&para2={maphieubg}&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            
            //if (tbl != null && tbl.Rows.Count > 0)
            //{
            //    searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
            //    txtTienTe.EditValue = tbl.Rows[0]["DonViTienTe"];
            //}

            if (_dtTable != null && _dtTable.Rows.Count > 0)
            {
                //searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
                txtTienTe.EditValue = _dtTable.Rows[0]["DonViTienTe"];
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

        private void searchLookUpEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            string mancc = searchLookUpEditNCC.EditValue == null ? "" : searchLookUpEditNCC.EditValue.ToString();
            CreateSearchlookupPhieuBG(mancc);
            GeneratePOMua();
            if (_IsAdd == true )
            {
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
                gridControl3.DataSource = null;
                gridControl4.DataSource = null;
                _dtTable = null;
                _dtTableCP = null;
                _dtTableDot = null;
                _dtTableChiTiet = null;
                _dtTableHTTT = null;
                if (!string.IsNullOrEmpty(mancc) && !string.IsNullOrEmpty(_maphieuyc))
                {
                    AutoSelection(mancc);
                }
            }
        }
        private void AutoSelection(string mancc)
        {
            try
            {
                string loaiPhieu = _rowFocusedpyc["LoaiPhieu"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(loaiPhieu))
                {
                    XtraMessageBox.Show("Không xác định được loại phiếu.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string url = $"{URL}NhaCC/GePMHMMTB?action=GetMMTBTheoNCC&para1={_maphieuyc}&para2={loaiPhieu}&para3={mancc}&para4=&para5=";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl == null || tbl.Rows.Count == 0)
                {
                    
                    return;
                }

                _dtTable = CreateDatatable();

                foreach (DataRow row in tbl.Rows)
                {
                    // === Fetch TyGia (giống CollectSelectedFromGrid) ===
                    string matiente = row["TienTeID"]?.ToString() ?? "VND";
                    string urlTyGia = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={matiente}&para2=VND&para3=&para4=&para5=";
                    string jsonTyGia = Task.Run(async () => await _clientExtension.GetAsnyc(urlTyGia)).Result;
                    DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(jsonTyGia);
                    decimal giaqd = (tblTyGia != null && tblTyGia.Rows.Count > 0 && tblTyGia.Rows[0]["TyGia"] != DBNull.Value)
                        ? Convert.ToDecimal(tblTyGia.Rows[0]["TyGia"])
                        : 1;

                    // === Tính toán (giống CollectSelectedFromGrid) ===
                    decimal thue = (row["Thue"] == DBNull.Value || row["Thue"].ToString() == "")
                        ? 0 : GetDecimalFromObject(GetThuePercentFromValue(row["Thue"]));
                    decimal chietkhau = (row["ChietKhau"] == DBNull.Value || row["ChietKhau"].ToString() == "")
                        ? 0 : GetDecimalFromObject(GetThuePercentFromValue(row["ChietKhau"]));
                    decimal donGia = row["DonGia"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DonGia"]);
                    decimal tongSL = row["TongSLMuaThem"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TongSLMuaThem"]);

                    decimal chiphi = donGia * tongSL;
                    decimal chiphichietkhau = chiphi - (chietkhau * chiphi);
                    decimal thanhtien = chiphichietkhau + (thue * chiphichietkhau);

                    // === Map TB / LK theo Status (giống frmChonMMTBPhieu) ===
                    int status = row.Table.Columns.Contains("Status")
                        ? Convert.ToInt32(row["Status"])
                        : (loaiPhieu == "2" ? 1 : 0);
                    bool isTB = status == 1;

                    DataRow newRow = _dtTable.NewRow();
                    newRow["IsTB"] = isTB;
                    newRow["MaNhom"] = DBNull.Value;
                    newRow["MaKho"] = "";
                    newRow["MaPhieuBG"] = row["MaPhieuBG"];
                    newRow["TenPhieu"] = row["TenPhieu"];
                    newRow["MauMa"] = isTB ? (row.Table.Columns.Contains("MauMa") ? row["MauMa"] : (object)DBNull.Value) : DBNull.Value;
                    newRow["XuatXu"] = isTB ? (row.Table.Columns.Contains("XuatXu") ? row["XuatXu"] : (object)DBNull.Value) : DBNull.Value;
                    newRow["HangSX"] = isTB ? (row.Table.Columns.Contains("HangSX") ? row["HangSX"] : (object)DBNull.Value) : DBNull.Value;

                    newRow["MaHang"] = row["MaHang"];
                    newRow["TenHang"] = row["TenHang"];
                    newRow["MaCL"] = row["MaCL"];
                    newRow["TenCL"] = row["TenCL"];
                    newRow["DonVi"] = row.Table.Columns.Contains("DonVi") ? row["DonVi"] : (object)DBNull.Value;

                    newRow["TongSLMuaThem"] = tongSL;
                    newRow["DonGia"] = donGia;
                    newRow["TienTeID"] = matiente;
                    newRow["Thue"] = row["Thue"];
                    newRow["ChietKhau"] = row["ChietKhau"];
                    newRow["PTVanChuyen"] = row.Table.Columns.Contains("PTVanChuyen") ? row["PTVanChuyen"] : (object)"";
                    newRow["PTThanhToan"] = row.Table.Columns.Contains("PTThanhToan") ? row["PTThanhToan"] : (object)"";
                    newRow["GhiChu"] = row["GhiChu"];
                    newRow["ChiPhiKhac"] = "";
                    newRow["NgayHieuLuc"] = DBNull.Value;
                    newRow["SoNgayGHSom"] = 0;
                    newRow["SoNgayGHTre"] = 0;
                    newRow["NgayGiaoHangYC"] = DBNull.Value;
                    newRow["TTChietKhau"] = "";

                    newRow["ChiPhiTB"] = chiphi;
                    newRow["ChiPhiSauCK"] = chiphichietkhau;
                    newRow["ThanhTien"] = thanhtien;
                    newRow["ThanhTienVND"] = thanhtien * giaqd;
                    newRow["ThanhTienQD"] = thanhtien * giaqd;

                    _dtTable.Rows.Add(newRow);
                }

                CreateSearchlookupcolPTThanhToan();
                gridControl1.DataSource = _dtTable;
                UpdateGridView5Summary();
                UpdateGridView5Values();

                int soChungLoai = _dtTable.AsEnumerable()
                    .Select(r => string.Join("|",
                        r["MaHang"]?.ToString() ?? "",
                        r["MaCL"]?.ToString() ?? "",
                        r["MaNhom"]?.ToString() ?? ""))
                    .Where(k => k != "||")
                    .Distinct()
                    .Count();
                spinEditSoLoaiVT.EditValue = soChungLoai;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Lỗi khi tải dữ liệu phiếu yêu cầu: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnChonVT_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditNCC.EditValue == null || searchLookUpEditNCC.EditValue == "")
            {
                XtraMessageBox.Show("Chưa chọn nhà cung cấp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //if (searchLookUpEditPBG.EditValue == null || searchLookUpEditPBG.EditValue == "")
            //{
            //    XtraMessageBox.Show("Chưa chọn phiếu báo giá.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            string mancc = searchLookUpEditNCC.EditValue.ToString();
            string maphieubg = searchLookUpEditPBG.EditValue?.ToString();
            string matiente = txtTienTe.EditValue.ToString();
            frmChonMMTBPhieu frm = new frmChonMMTBPhieu(mancc, maphieubg, matiente,_maphieuyc);
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
                    bool isTB = Convert.ToInt32(row["Status"]) == 1;
                    newRow["IsTB"] = isTB;

                    newRow["MaHang"] = row["MaHang"];
                    newRow["MaCL"] = row["MaCL"];
                    newRow["MaNhom"] = row["MaNhom"];
                    newRow["MaKho"] = "";
                    newRow["MaPhieuBG"] = row["MaPhieuBG"];

                    newRow["TenHang"] = row["TenHang"];
                    newRow["TenCL"] = row["TenCL"];
                    newRow["MauMa"] = row["MauMa"];
                    newRow["XuatXu"] = row["XuatXu"];
                    newRow["HangSX"] = row["HangSX"];
                    newRow["DonVi"] = row["DonVi"];
                    newRow["TenPhieu"] = row["TenPhieu"];

                    newRow["TongSLMuaThem"] = row["TongSLMuaThem"];
                    newRow["DonGia"] = row["DonGia"];
                    newRow["ThanhTien"] = row["ThanhTien"];
                    newRow["TienTeID"] = row["TienTeID"];
                    newRow["GhiChu"] = "";
                    newRow["PTVanChuyen"] = row["PTVanChuyen"];
                    newRow["PTThanhToan"] = row["PTThanhToan"];
                    newRow["Thue"] = row["Thue"];
                    newRow["NgayHieuLuc"] = row["NgayBDHieuLuc"];
                    newRow["SoNgayGHSom"] = row["SoNgayGHSom"];
                    newRow["SoNgayGHTre"] = row["SoNgayGHTre"];
                    newRow["ChiPhiTB"] = row["ChiPhiVT"];
                    newRow["ChiPhiSauCK"] = row["ChiPhiSauCK"];
                    newRow["ChietKhau"] = row["ChietKhau"];
                    newRow["ThanhTienVND"] = row["ThanhTienVND"];
                    newRow["ThanhTienQD"] =
                        GetDecimalFromObject(row["ThanhTienVND"]) /
                        Convert.ToDecimal(spinEditTyGia.EditValue);

                    newRow["ChiPhiKhac"] = "";
                    newRow["TTChietKhau"] = "";
                    newRow["NgayGiaoHangYC"] = DBNull.Value;

                    _dtTable.Rows.Add(newRow);
                }

                CreateSearchlookupcolPTThanhToan();
                gridControl1.DataSource = _dtTable;


                if (_IsEdit == false)
                {
                    LoadDataCP();
                    //themdot();
                }

                UpdateGridView5Summary();
                UpdateGridView5Values();

                int soChungLoai = _dtTable.AsEnumerable()
                                .Where(r => r != null)
                                .Select(r =>
                                    string.Join("|",
                                        r["MaHang"]?.ToString() ?? "",
                                        r["MaCL"]?.ToString() ?? "",
                                        r["MaNhom"]?.ToString() ?? ""
                                    )
                                )
                                .Where(k => k != "|||") // ❌ loại dòng rỗng
                                .Distinct()
                                .Count();

                spinEditSoLoaiVT.EditValue = soChungLoai;
            }
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

        private decimal GetDecimalValue(GridView view, int rowHandle, string fieldName)
        {
            var val = view.GetRowCellValue(rowHandle, fieldName);
            if (val is decimal d) return d;
            if (val is double db) return (decimal)db;
            if (val is int i) return i;
            decimal.TryParse(val?.ToString(), out decimal result);
            return result;
        }
        private decimal GetDecimalFromObject(object obj)
        {
            if (obj == null || obj == DBNull.Value) return 0m;
            decimal d;
            if (decimal.TryParse(obj.ToString(), out d)) return d;
            return 0m;
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

        private void GeneratePOMua()
        {
            string urlcheck = $"{URL}NhaCC/GePMHMMTB?action=GETPhieuMuaHangCheck&para1=&para2=&para3=&para4=&para5=";
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

            //        int dot;
            //        if (!int.TryParse(parts[0], out dot)) continue;

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

        #region CreateDataTable
        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("IsTB", typeof(bool));
            //tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaKho", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));

            tbl.Columns.Add("TenHang", typeof(string));
            tbl.Columns.Add("TenCL", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("MauMa", typeof(string));
            tbl.Columns.Add("XuatXu", typeof(string));
            tbl.Columns.Add("HangSX", typeof(string));
            tbl.Columns.Add("DonVi", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));

            tbl.Columns.Add("TongSLMuaThem", typeof(decimal));
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("PTVanChuyen", typeof(string));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("SoNgayGHSom", typeof(int));
            tbl.Columns.Add("SoNgayGHTre", typeof(int));
            tbl.Columns.Add("ChiPhiTB", typeof(decimal));
            tbl.Columns.Add("ChiPhiSauCK", typeof(decimal));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            tbl.Columns.Add("ThanhTienQD", typeof(decimal));
            return tbl;
        }

        private DataTable CreateDatatableload()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("LoaiHang", typeof(string));
            tbl.Columns.Add("IsTB", typeof(bool));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("TenCL", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaKho", typeof(string));
            tbl.Columns.Add("TenHang", typeof(string));
            tbl.Columns.Add("SoSeri", typeof(string));
            tbl.Columns.Add("DonVi", typeof(string));
            tbl.Columns.Add("MauMa", typeof(string));
            tbl.Columns.Add("XuatXu", typeof(string));
            tbl.Columns.Add("HangSX", typeof(string));
            tbl.Columns.Add("TongSLMuaThem", typeof(decimal));
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("CPVanChuyen", typeof(decimal));
            tbl.Columns.Add("ChiPhiTB", typeof(decimal));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("MaTienTePhieuMH", typeof(string));
            tbl.Columns.Add("TyGiaThanhToan", typeof(decimal));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("NgayXacNhan", typeof(DateTime));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("GhiChuTB", typeof(string));
            tbl.Columns.Add("IsDuyet", typeof(bool));
            tbl.Columns.Add("IsXacNhan", typeof(bool));

            return tbl;
        }


        private DataTable CreateDatatableToSave()
        {
            DataTable tbl = new DataTable("dtSize");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("LoaiHang", typeof(string));
            tbl.Columns.Add("IsTB", typeof(bool));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaKho", typeof(string));
            tbl.Columns.Add("TenHang", typeof(string));
            tbl.Columns.Add("SoSeri", typeof(string));
            tbl.Columns.Add("DonVi", typeof(string));
            tbl.Columns.Add("MauMa", typeof(string));
            tbl.Columns.Add("XuatXu", typeof(string));
            tbl.Columns.Add("HangSX", typeof(string));
            tbl.Columns.Add("SoLuongMuaThem", typeof(decimal));
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("CPVanChuyen", typeof(decimal));
            tbl.Columns.Add("ChiPhiTB", typeof(decimal));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("MaTienTePhieuMH", typeof(string));
            tbl.Columns.Add("TyGiaThanhToan", typeof(decimal));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("NgayXacNhan", typeof(DateTime));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("GhiChuTB", typeof(string));
            tbl.Columns.Add("IsDuyet", typeof(bool));
            tbl.Columns.Add("IsXacNhan", typeof(bool));
            tbl.Columns.Add("PhieuYC", typeof(string));
            tbl.Columns.Add("Action", typeof(string));
            tbl.Columns.Add("MaKhachHang", typeof(string));
            //tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            //tbl.Columns.Add("ThanhTienQD", typeof(decimal));
            return tbl;
        }

        private DataTable CreateDatatableDot()
        {
            DataTable tbl = new DataTable("_dtTableDot");
            tbl.Columns.Add("Thutu", typeof(int));
            tbl.Columns.Add("MaPTVC", typeof(string));
            tbl.Columns.Add("ChiPhiVC", typeof(decimal));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaKho", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("IsTB", typeof(bool));
            tbl.Columns.Add("SoLuongVC", typeof(decimal));
            tbl.Columns.Add("NgayBatDau", typeof(DateTime));
            tbl.Columns.Add("NhanMin", typeof(int));
            tbl.Columns.Add("NhanMax", typeof(int));
            tbl.Columns.Add("NgayNhanDK", typeof(DateTime));
            tbl.Columns.Add("NgayNhanTT", typeof(DateTime));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("TenHang", typeof(string));
            tbl.Columns.Add("TenCL", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("MauMa", typeof(string));
            tbl.Columns.Add("HangSX", typeof(string));
            tbl.Columns.Add("DonVi", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("ThanhTienVC", typeof(decimal));
            return tbl;
        }

        private DataTable CreateDatatableToSaveDot()
        {
            DataTable tbl = new DataTable("_dtTableDot");
            tbl.Columns.Add("MaPhieu", typeof(string));
            tbl.Columns.Add("ThuTu", typeof(int));
            tbl.Columns.Add("MaPTVC", typeof(string));
            tbl.Columns.Add("ChiPhiVC", typeof(float));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaKho", typeof(string));
            tbl.Columns.Add("IsTB", typeof(bool));
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
        #endregion

        #region searchlookup
        private void CreateSearchlookupTienTe()
        {
            string url = $"{URL}NhaCC/GePMHMMTB?action=GetALLTienTe&para1=&para2=&para3=&para4=&para5=";
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
                    txtTienTe.EditValue = matiente;
                }

            }
        }
        private void CreateSearchlookupNCC()
        {
            string url = $"{URL}NhaCC/GePMHMMTB?action=GetNCC&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditNCC.Properties.DataSource = tbl;
            searchLookUpEditNCC.Properties.ValueMember = "MaNhaCungCap";
            searchLookUpEditNCC.Properties.DisplayMember = "NhaCungCap";

        }
        private void CreateSearchlookupPhieuBG(string mancc)
        {
            string url = $"{URL}NhaCC/GePMHMMTB?action=GetPhieuBG&para1={mancc}&para2=&para3=&para4=&para5=";
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
            if (_IsAdd == true)
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
                    txtTienTe.EditValue = tbl.Rows[0]["DonViTienTe"];
                }
            }


        }
        private void CreateSearchlookupcolTienTe()
        {
            string url = $"{URL}NhaCC/GePMHMMTB?action=GetALLTienTe&para1=&para2=&para3=&para4=&para5=";
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
            gridColumn64.ColumnEdit = rCountryEditkho;

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
        #endregion

        #region SUM
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
                string url = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
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

        #endregion

        #region PaymentTerm
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

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
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
                string url = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={matiente}&para2={"VND"}&para3=&para4=&para5=";
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
                view.SetRowCellValue(e.RowHandle, "ChiPhiTB", Math.Round(chiphivt, 2));
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

        private void gridView3_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "IsTB"
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
                info.GroupText = isChecked ? "Máy Móc Thiết Bị" : "Linh Kiện";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn43)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            if (info.Column == gridColumn15)
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
                    src["MaHang"].ToString(),
                    src["MaCL"].ToString(),
                    src["MaNhom"].ToString()
                );

                decimal thieu = tong - sumCurrent;

                // BÙ PHẦN THIẾU VÀO ĐỢT CUỐI
                if (thieu > 0)
                {
                    DataRow lastDot = GetLastDotRow(
                        src["MaHang"].ToString(),
                        src["MaCL"].ToString(),
                        src["MaNhom"].ToString()
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

        private void FillRowDot(DataRow dr, int thuTu, DataRow src, decimal soLuong)
        {
            dr["Thutu"] = thuTu;
            dr["MaPTVC"] = "";
            dr["ChiPhiVC"] = 0;
            dr["MaHang"] = src["MaHang"];
            dr["MaCL"] = src["MaCL"];
            dr["MaNhom"] = src["MaNhom"];
            dr["MaKho"] = src["MaKho"];
            dr["MaPhieuBG"] = src["MaPhieuBG"];
            dr["IsTB"] = src["IsTB"];
            dr["SoLuongVC"] = soLuong;
            dr["NgayBatDau"] = DBNull.Value;
            dr["NhanMin"] = src["SoNgayGHSom"] ?? 0;
            dr["NhanMax"] = src["SoNgayGHTre"] ?? 0;
            dr["NgayNhanDK"] = DBNull.Value;
            dr["NgayNhanTT"] = DBNull.Value;
            dr["NguoiTao"] = GlobleData.UserName.ToString();
            dr["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
            dr["GhiChu"] = src["GhiChu"];
            dr["TenHang"] = src["TenHang"];
            dr["TenCL"] = src["TenCL"];
            dr["TenNhom"] = src["TenNhom"];
            dr["MauMa"] = src["MauMa"];
            dr["HangSX"] = src["HangSX"];
            dr["DonVi"] = src["DonVi"];
            dr["TenPhieu"] = src["TenPhieu"];
            dr["Thue"] = _thuevc;
            dr["ThanhTienVC"] = 0;
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
            string mahang,
            string macl,
            string manhom)
        {
            return _dtTableDot.AsEnumerable()
                .Where(r =>
                    r.Field<string>("MaHang") == mahang &&
                    r.Field<string>("MaCL") == macl &&
                    r.Field<string>("MaNhom") == manhom
                )
                .Sum(r => r.Field<decimal?>("SoLuongVC") ?? 0);
        }

        private DataRow GetLastDotRow(
            string mahang,
            string macl,
            string manhom)
        {
            return _dtTableDot.AsEnumerable()
                .Where(r =>
                    r.Field<string>("MaHang") == mahang &&
                    r.Field<string>("MaCL") == macl &&
                    r.Field<string>("MaNhom") == manhom
                )
                .OrderByDescending(r => r.Field<int>("ThuTu"))
                .FirstOrDefault();
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
            //if (e.Column.FieldName == "ChiPhiVC")
            //{
            //    string Manhom = view.GetRowCellValue(e.RowHandle, "MaHang")?.ToString();
            //    string Macl = view.GetRowCellValue(e.RowHandle, "MaCL")?.ToString();
            //    string manhom = view.GetRowCellValue(e.RowHandle, "MaNhom")?.ToString();
            //    CapNhatThanhTien_ByMaVT(maVTID, mauvtid, khovtid, maclvt);
            //}
            if (e.Column.FieldName == "SoLuongVC")
            {

                if (currentRow == null) return;

                string mahang = currentRow["MaHang"]?.ToString();
                string Macl = currentRow["MaCL"]?.ToString();
                string manhom = currentRow["MaNhom"]?.ToString();
                bool status = Convert.ToBoolean(currentRow["IsTB"]);
                //string maNhomVT = currentRow["MaNhomVT"]?.ToString();
                string Tenhh = currentRow["TenHang"]?.ToString();

                if (string.IsNullOrEmpty(mahang)) return;

                int thuTu = Convert.ToInt32(currentRow["ThuTu"]);

                decimal newVal = 0;
                decimal.TryParse(e.Value?.ToString(), out newVal);

                decimal oldVal = 0;
                decimal.TryParse(_oldSoLuongVC.ToString(), out oldVal);


                var result = ReCalcSoLuongVC(
                            mahang,
                            Macl,
                            manhom,
                            status,
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
                            $"Tổng phân bổ cho vật tư {Tenhh} vượt quá Tổng yêu cầu ({tong})",
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

                //CapNhatThanhTien_ByMaVT(maVTID, mauVTID, khoVTID, maNhomVT);
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

        private void gridView5_ShownEditor(object sender, EventArgs e)
        {
            var view = sender as GridView;
            if (view.FocusedColumn.FieldName == "PhanTram" || view.FocusedColumn.FieldName == "SoTien")
            {
                decimal.TryParse(view.GetFocusedRowCellValue("PhanTram")?.ToString(), out _oldPhanTram);
                decimal.TryParse(view.GetFocusedRowCellValue("SoTien")?.ToString(), out _oldSoTien);
            }
        }

        public enum ReCalcResult
        {
            Success,
            VuotTong,
            InvalidEdit
        }

        private ReCalcResult ReCalcSoLuongVC(
            string mahang,
            string Macl,
            string manhom,
            bool status,
            bool fromCellEdit,
            int? thuTuDangSua,
            decimal? newValDangSua,
            decimal? oldValDangSua,
            out decimal tong
        )
        {
            tong = LayTongTuGridView1(mahang, Macl, manhom);

            var dots = _dtTableDot.AsEnumerable()
                .Where(r =>
                    r["MaHang"]?.ToString() == mahang &&
                    r["MaCL"]?.ToString() == Macl &&
                    r["MaNhom"]?.ToString() == manhom
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

            //CapNhatThanhTien_ByMaVT(maVTID, mauVTID, khoVTID, maNhomVT);

            return ReCalcResult.Success;
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
                            URL + "NhaCC/DeleteDotPMHMMTB",
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
                    MaHang = r["MaHang"]?.ToString(),
                    MaCL = r["MaCL"]?.ToString(),
                    MaNhom = r["MaNhom"]?.ToString()
                });

            foreach (var g in groups)
            {
                string mahang = g.Key.MaHang;
                string macl = g.Key.MaCL;
                string manhom = g.Key.MaNhom;


                // 1. LẤY TỔNG TỪ GRIDVIEW1
                decimal tong = 0;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (
                        gridView1.GetRowCellValue(i, "MaHang")?.ToString() == mahang &&
                        gridView1.GetRowCellValue(i, "MaCL")?.ToString() == macl &&
                        gridView1.GetRowCellValue(i, "MaNhom")?.ToString() == manhom 
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

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
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
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );

                if (result != DialogResult.OK)
                {
                    return;
                }
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
                //    SaveDataHTTT(dt3);
                //}
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

        private decimal LayTongTuGridView1(string mahang, string Macl, string manhom)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if (
                    gridView1.GetRowCellValue(i, "MaHang")?.ToString() == mahang &&
                    gridView1.GetRowCellValue(i, "MaCL")?.ToString() == Macl &&
                    gridView1.GetRowCellValue(i, "MaNhom")?.ToString() == manhom
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
        #endregion

        #region chiphi
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

        private void gridView2_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;

            e.Appearance.BackColor = Color.LightYellow; // Màu nền

            e.Appearance.ForeColor = Color.Red;

            e.Appearance.Font = new Font("Tahoma", 8F, FontStyle.Bold);
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

        private void CapNhatTongChiPhiVC()
        {
            _tongvccpps = 0;

            if (_dtTableCP == null) return;

            DataRow[] rows = _dtTableCP.Select("MaChiPhi IN (3, 6)");

            if (rows.Length > 0)
            {
                foreach (DataRow r in rows)
                {
                    string donViTienTe = r["DonViTienTe"]?.ToString();
                    if (string.IsNullOrEmpty(donViTienTe))
                        continue;

                    // Lấy tỷ giá theo từng dòng
                    string urldvtt = $"{URL}NhaCC/GePMHMMTB?action=GETPTyGia&para1={donViTienTe}&para2=VND&para3=&para4=&para5=";
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
        private decimal TinhTongSoTien()
        {
            decimal tong = 0;

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
        #endregion

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "IsTB"
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
                info.GroupText = isChecked ? "Máy móc thiết bị" : "Linh kiện";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn3)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            if (info.Column == gridColumn14)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }
            


            e.Handled = false;
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

        private void gridView1_PopupMenuShowing_1(object sender, PopupMenuShowingEventArgs e)
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

        #region POST
        public void SaveData(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            string url1 = $"{URL}NhaCC/GePMHMMTB?action=GETALLPHIEUMHCount&para1=&para2=&para3=&para4=&para5=";
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

            DataTable tblDG = CreateDatatableToSave();

            int maxPhieu = 0;

            if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("MaPhieuMH"))
            {
                var validValues = _tbldv.AsEnumerable()
                    .Select(r => r["MaPhieuMH"]?.ToString())
                    .Where(s => !string.IsNullOrWhiteSpace(s) && s.StartsWith("PHIEU_MMTB_"))
                    .Select(s =>
                    {
                        string numberPart = s.Replace("PHIEU_MMTB_", "").Trim();
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
                //_maphieumh = "PHIEU_MMTB_" + newPhieu;
                //_tenphieumh = "PHIẾU MH|" +searchLookUpEditNCC.Text.ToString()+"|"+ searchLookUpEditPBG.Text.ToString()+ "|"+newPhieu;
                //_tenphieumh = "PHIẾU MH|" + searchLookUpEditNCC.Text.ToString() + "|" + newPhieu;
                DataRow newRow = tblDG.NewRow();
                newRow["ID"] = 0;
                newRow["LoaiHang"] = "";
                newRow["IsTB"] = dr["IsTB"];
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                newRow["TenPhieu"] = _tenphieumh;
                newRow["MaNCC"] = searchLookUpEditNCC.EditValue?.ToString();
                newRow["MaHang"] = dr["MaHang"]?.ToString();
                newRow["MaCL"] = dr["MaCL"]?.ToString();
                newRow["MaNhom"] = dr["MaNhom"];
                newRow["MaKho"] = dr["MaKho"];
                newRow["TenHang"] = dr["TenHang"];
                newRow["SoSeri"] = "";
                newRow["DonVi"] = dr["DonVi"];
                newRow["MauMa"] = dr["MauMa"];
                newRow["XuatXu"] = dr["XuatXu"];
                newRow["HangSX"] = dr["HangSX"];
                newRow["SoLuongMuaThem"] = dr["TongSLMuaThem"];
                newRow["DonGia"] = dr["DonGia"];
                newRow["ThanhTien"] = dr["ThanhTien"];
                newRow["Thue"] = dr["Thue"] == DBNull.Value ? 0 : dr["Thue"];
                newRow["CPVanChuyen"] = 0;
                newRow["ChiPhiTB"] = dr["ChiPhiTB"];
                newRow["ChiPhiKhac"] = "";
                newRow["TienTeID"] = dr["TienTeID"];
                newRow["MaTienTePhieuMH"] = txtTienTe.EditValue.ToString();
                newRow["TyGiaThanhToan"] = 0;
                newRow["ChietKhau"] = dr["ChietKhau"] == DBNull.Value ? 0 : dr["ChietKhau"];
                newRow["TTChietKhau"] = dr["TTChietKhau"] == null ? "" : dr["TTChietKhau"];
                newRow["POMua"] = textEditPOMua.Text;
                newRow["NgayDuKienHV"] = "";
                newRow["NgayGiaoHangYC"] = dr["NgayGiaoHangYC"] == null ? DBNull.Value : dr["NgayGiaoHangYC"];
                newRow["NgayHieuLuc"] = dr["NgayHieuLuc"] == null ? DBNull.Value : dr["NgayHieuLuc"];
                newRow["NgayXacNhan"] = DBNull.Value;
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["NguoiSua"] = "";
                newRow["NgaySua"] = DBNull.Value;
                newRow["GhiChu"] = dr["GhiChu"];
                newRow["GhiChuTB"] = "";
                newRow["IsDuyet"] = 0;
                newRow["IsXacNhan"] = 0;
                newRow["PhieuYC"] = _maphieuyc;
                newRow["Action"] = "NewPhieu";
                newRow["MaKhachHang"] = SeachLookupKhachHang.EditValue;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/PospmhMMTB");
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

            POSTPYC();
        }

        public void Update(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            DataTable tblDG = CreateDatatableToSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["ID"] = 0;
                newRow["LoaiHang"] = "";
                newRow["IsTB"] = dr["IsTB"];
                newRow["MaPhieuMH"] = _maphieumh.ToString().Trim();
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                newRow["TenPhieu"] = _tenphieumh;
                newRow["MaNCC"] = searchLookUpEditNCC.EditValue?.ToString().Trim();
                newRow["MaHang"] = dr["MaHang"]?.ToString().Trim();
                newRow["MaCL"] = dr["MaCL"]?.ToString().Trim();
                newRow["MaNhom"] = dr["MaNhom"];
                newRow["MaKho"] = dr["MaKho"];
                newRow["TenHang"] = dr["TenHang"];
                newRow["SoSeri"] = "";
                newRow["DonVi"] = dr["DonVi"];
                newRow["MauMa"] = dr["MauMa"];
                newRow["XuatXu"] = dr["XuatXu"];
                newRow["HangSX"] = dr["HangSX"];
                newRow["SoLuongMuaThem"] = dr["TongSLMuaThem"];
                newRow["DonGia"] = dr["DonGia"];
                newRow["ThanhTien"] = dr["ThanhTien"];
                newRow["Thue"] = dr["Thue"] == DBNull.Value ? 0 : dr["Thue"];
                newRow["CPVanChuyen"] = 0;
                newRow["ChiPhiTB"] = dr["ChiPhiTB"];
                newRow["ChiPhiKhac"] = "";
                newRow["TienTeID"] = dr["TienTeID"];
                newRow["MaTienTePhieuMH"] = txtTienTe.EditValue.ToString();
                newRow["TyGiaThanhToan"] = 0;
                newRow["ChietKhau"] = dr["ChietKhau"] == DBNull.Value ? 0 : dr["ChietKhau"];
                newRow["TTChietKhau"] = dr["TTChietKhau"] == null ? "" : dr["TTChietKhau"];
                newRow["POMua"] = textEditPOMua.Text;
                newRow["NgayDuKienHV"] = "";
                newRow["NgayGiaoHangYC"] = dr["NgayGiaoHangYC"] == null ? DBNull.Value : dr["NgayGiaoHangYC"];
                newRow["NgayHieuLuc"] = dr["NgayHieuLuc"] == null ? DBNull.Value : dr["NgayHieuLuc"];
                newRow["NgayXacNhan"] = DBNull.Value;
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["NguoiSua"] = GlobleData.UserName.ToString();
                newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["GhiChu"] = dr["GhiChu"];
                newRow["GhiChuTB"] = "";
                newRow["IsDuyet"] = 0;
                newRow["IsXacNhan"] = 0;
                newRow["PhieuYC"] = _maphieuyc;
                newRow["Action"] = "UpdatePhieu";
                newRow["MaKhachHang"] = SeachLookupKhachHang.EditValue;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/PospmhMMTB");
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
                newRow["ThuTu"] = dr["ThuTu"]?.ToString();
                newRow["MaPTVC"] = "";
                newRow["ChiPhiVC"] = dr["ChiPhiVC"]?.ToString();
                newRow["MaHang"] = dr["MaHang"]?.ToString();
                newRow["MaCL"] = dr["MaCL"]?.ToString();
                newRow["MaNhom"] = dr["MaNhom"]?.ToString();
                newRow["MaKho"] = "";
                newRow["IsTB"] = dr["IsTB"]?.ToString();
                newRow["SoLuongVC"] = dr["SoLuongVC"];
                newRow["NgayBatDau"] = dr["NgayBatDau"] == null ? DBNull.Value : dr["NgayBatDau"];
                newRow["NhanMin"] = dr["NhanMin"] == null ? 0 : dr["NhanMin"];
                newRow["NhanMax"] = dr["NhanMax"] == null ? 0 : dr["NhanMax"];
                newRow["NgayNhanDK"] = dr["NgayNhanDK"] == null ? DBNull.Value : dr["NgayNhanDK"];
                newRow["NgayNhanTT"] = dr["NgayNhanTT"] == null ? DBNull.Value : dr["NgayNhanTT"];
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["Thue"] = dr["Thue"];
                newRow["PTGiaoHang"] = "";
                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/PostDotGHPMHMMTB");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);
        }
        public void SaveDataHTTT(DataTable dt)
        {
            this.ActiveControl = simpleButton2;
            DataTable tblHT = CreateDatatableHTTTSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblHT.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["SoTien"] = dr["SoTien"];
                newRow["TGTra"] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow["DotTra"] = dr["DotTra"];
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["PhanTram"] = dr["PhanTram"];
                newRow["PTThanhToan"] = dr["PTThanhToan"];
                newRow["InvoiceTo"] = dr["InvoiceTo"];
                tblHT.Rows.Add(newRow);
            }

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
        #endregion
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

        private void POSTPYC()
        {
            string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
            ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
            objXetDuyet.Action = "POSTMaPhieuYC";
            objXetDuyet.MaPhieu = _maphieumh;
            objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            objXetDuyet.IsXacNhan = false;
            objXetDuyet.User = GlobleData.UserName;
            objXetDuyet.GhiChu = _maphieuyc;

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            if (msResult.ToLower() == "true")
            {
                //clsWaitForm.ShowSuccessForm(this, 2000);
                //this.DialogResult = DialogResult.OK;
                //LoadPhieuMuaHang();
            }
            else
            {
                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void GeneratePhieuMH()
        {
            string url = $"{URL}NhaCC/GePMHMMTB?action=GeneratePhieuMH&para1=NONE&para2=&para3=&para4=&para5=";
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
            if (tbl != null && tbl?.Rows?.Count > 0)
            {
                DataRow rowEInvoice = tbl.Rows[0];

                //clsForrmatUtils.ConvertDate(rowPhieu["NgayBDHieuLuc"]?.ToString())
                txtInVoiceNo.EditValue = rowEInvoice["SoHoaDon"];
                txtCustomsDeclarationNo.EditValue = rowEInvoice["SoToKhai"];
                txtContractNo.EditValue = rowEInvoice["SoHopDong"];

                DateTime InvoiceDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayHoaDon"]?.ToString());
                if (InvoiceDate != DateTime.MinValue)
                {
                    DateEditInvoice.EditValue = InvoiceDate;
                }
                DateTime DeclarationDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayMoToKhai"]?.ToString());
                if (DeclarationDate != DateTime.MinValue)
                {
                    DateEditDeclaration.EditValue = DeclarationDate;
                }
                DateTime ContractDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayKyHD"]?.ToString());
                if (ContractDate != DateTime.MinValue)
                {
                    DateEditContract.EditValue = ContractDate;
                }

                txtVatNum.EditValue = rowEInvoice["MaSoThue"];
                txtBillOfLading.EditValue = rowEInvoice["SoVanDon"];
                txtEWayBill.EditValue = rowEInvoice["E_Way_Bill"];
                txtShipped.EditValue = rowEInvoice["NoiGui"];
                DateTime ShippedDate = clsForrmatUtils.ConvertDate(rowEInvoice["NgayGui"]?.ToString());
                if (ShippedDate != DateTime.MinValue)
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

        private string LuuCangMoi(string tenCang, string diaChi)
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

        private string LuuTauMoi(string Tentau, string DiaChi)
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
            catch (Exception ex)
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
                    clsWaitForm.ShowSuccessForm(this, 3000);
            }
            catch (Exception ex)
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