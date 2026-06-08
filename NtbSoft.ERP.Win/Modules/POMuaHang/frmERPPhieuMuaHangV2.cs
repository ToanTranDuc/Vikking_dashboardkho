using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.POMuaHang;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmERPPhieuMuaHangV2 : DevExpress.XtraEditors.XtraForm
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
        string vt = string.Empty, _maphieumh = string.Empty, _mancc = string.Empty, _maphieubg = string.Empty, _tenphieumh = string.Empty, _nguoitao = string.Empty, _tienteID = string.Empty, _pomua = string.Empty;
        bool _IsEdit;
        bool _IsDuyet;
        bool _IsAdd;
        bool _IsView;
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


        //tam
        private DataTable gridDotGiaoHangTable;
        private DataTable gridTienDoGiaoHangTable;
        private DataTable chitietnhapkhoTable;
        private DataTable chitietnhapkhotongTable;

        public frmERPPhieuMuaHangV2(DataRow rowFocused = null, bool IsEdit = false, bool IsAdd = false, bool IsView = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            _IsEdit = IsEdit;
            _IsAdd = IsAdd;
            _IsView = IsView;
            if (_IsEdit == true || _IsView == true)
            {
                _rowFocused = rowFocused;
                _maphieumh = _rowFocused["MaPhieuMH"]?.ToString();
                _mancc = _rowFocused["MaNCC"]?.ToString();
                _maphieubg = _rowFocused["MaPhieuBG"]?.ToString();
                _tenphieumh = _rowFocused["TenPhieu"]?.ToString();
                _nguoitao = _rowFocused["NguoiTao"]?.ToString();
                _NgayTao = _rowFocused["NgayTao"]?.ToString();
                _IsDuyet = Convert.ToBoolean(_rowFocused["IsDuyet"]);
                _tongCPPS = GetDecimalFromObject(_rowFocused["TongCPPS"]);
                _tongCPVC = GetDecimalFromObject(_rowFocused["TongCPVC"]);
                _tongCP = GetDecimalFromObject(_rowFocused["TongTien"]);
                _tongCPVND = GetDecimalFromObject(_rowFocused["TongTienVND"]);
                _tongCPVT = GetDecimalFromObject(_rowFocused["TongCPVT"]);
                _pomua = _rowFocused["POMua"]?.ToString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchlookupNCC();
            SetupNumericColumns();
            barButtonItem3.Visibility = BarItemVisibility.Never;
            barButtonItem4.Visibility = BarItemVisibility.Never;
            //CreateDefaultSearchLookUp();
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

                searchLookUpEditNCC.EditValue = _mancc.ToString();
                searchLookUpEditPBG.EditValue = _maphieubg.ToString();
                txtNguoiTao.Text = _nguoitao;
                DEditNgayTao.EditValue = _NgayTao;
                textEditPOMua.Text = _pomua;


                spinEditTongCPVT.EditValue = _tongCPVT;
                spinEditTongCPPS.EditValue = _tongCPPS;
                spinEditTongCP.EditValue = _tongCP;
                spinEditTongCPVND.EditValue = _tongCPVND;
                LoadData();
                LoadDataChiPhi();
                LoadDataDotGH();
                LoadDataHTTT();
                getChiTietNhapKho();
                getChiTietNhapKhoTong();
                calcJoinVattuWithNhapKho();
                calcInnerJoinVattuWithNhapKho();
                loadgridDotGiaoHang();
                loadgridTienDoGiaoHang();
                int soChungLoai = 0;

                var list = new HashSet<string>();

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    var value = gridView1.GetRowCellValue(i, "MaCLVT")?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        list.Add(value);
                }

                soChungLoai = list.Count;

                spinEditSoLoaiVT.EditValue = soChungLoai;
            }
            if (_IsAdd)
            {
                DEditNgayTao.EditValue = DateTime.Now;
                CreateSearchLookup();
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
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                barButtonItem1.Visibility = BarItemVisibility.Never;
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem23.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem46.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem40.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem44.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                searchLookUpEditNCC.EditValue = _mancc.ToString();
                searchLookUpEditPBG.EditValue = _maphieubg.ToString();
                txtNguoiTao.Text = _nguoitao;
                DEditNgayTao.EditValue = _NgayTao;

                spinEditTongCPVT.EditValue = _tongCPVT;
                spinEditTongCPPS.EditValue = _tongCPPS;
                spinEditTongCP.EditValue = _tongCP;
                spinEditTongCPVND.EditValue = _tongCPVND;
                textEditPOMua.Text = _pomua;
                LoadData();
                LoadDataChiPhi();
                LoadDataDotGH();
                LoadDataHTTT();
                getChiTietNhapKho();
                getChiTietNhapKhoTong();
                calcJoinVattuWithNhapKho();
                calcInnerJoinVattuWithNhapKho();
                loadgridDotGiaoHang();
                loadgridTienDoGiaoHang();
                int soChungLoai = 0;

                var list = new HashSet<string>();

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    var value = gridView1.GetRowCellValue(i, "MaCLVT")?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        list.Add(value);
                }

                soChungLoai = list.Count;

                spinEditSoLoaiVT.EditValue = soChungLoai;

                gridView1.OptionsBehavior.Editable = false;
                gridView2.OptionsBehavior.Editable = false;
                gridView31.OptionsBehavior.Editable = false;
                gridView52.OptionsBehavior.Editable = false;
            }
        }

        private void CreateSearchLookup()
        {
            try
            {
                string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
                string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
                DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);
                string username = GlobleData.UserName.ToString();
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
            _dtTable = JsonConvert.DeserializeObject<DataTable>(json);
            CreateSearchlookupcolPTThanhToan();

            if (!_dtTable.Columns.Contains("ThanhTienVND"))
                _dtTable.Columns.Add("ThanhTienVND", typeof(decimal));

            foreach (DataRow r in _dtTable.Rows)
            {

                decimal ThanhTien = GetDecimalFromObject(r["ThanhTien"]);

                string mancc = r["MaNCC"]?.ToString() ?? "";

                string maTienTe = r["MaTienTe"]?.ToString() ?? "VND";

                decimal gia = 1;

                if (!string.IsNullOrEmpty(mancc) && maTienTe != "VND")
                {
                    string url1 = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={mancc}&para2=&para3=&para4=&para5=";

                    string json1 = Task.Run(async () => await _clientExtension.GetAsnyc(url1)).Result;

                    DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(json1);

                    if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        gia = Convert.ToDecimal(tblTyGia.Rows[0]["Gia"]);
                }

                if (maTienTe == "VND")
                    r["ThanhTienVND"] = ThanhTien;
                else
                    r["ThanhTienVND"] = ThanhTien * gia;
            }
            gridControl12.DataSource = _dtTable;

        }

        private void LoadDataChiPhi()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPMHChiPhi&para1={_maphieumh}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableCP = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl2.DataSource = _dtTableCP;
            if (_dtTableCP != null && _dtTableCP.Rows.Count > 0)
            {
                DataRow[] rows = _dtTableCP.Select("MaChiPhi = 3 OR MaChiPhi = 6");

                if (rows.Length > 0)
                {
                    decimal totalSoTien = 0;
                    decimal totalThue = 0;

                    foreach (var r in rows)
                    {
                        decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                        decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);

                        totalSoTien += soTien;
                        totalThue += (thue / 100m) * soTien;
                    }

                    _tongvccpps = totalSoTien + totalThue;

                    spinEditTongCPVC.EditValue = _tongvccpps;
                }
                else
                {
                    _tongvccpps = _tongCPPS;
                    spinEditTongCPVC.EditValue = _tongCPVC;
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
            gridControl31.DataSource = _dtTableDot;
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
            CreateSearchlookupcolPTThanhToanHT();
            gridControl42.DataSource = _dtTableHTTT;
            if (_dtTableHTTT != null && _dtTableHTTT.Rows.Count > 0)
            {
                // Cách 1: Dùng LINQ
                int maxThuTu = _dtTableHTTT.AsEnumerable()
                                          .Where(row => row["DotTra"] != DBNull.Value)
                                          .Select(row => Convert.ToInt32(row["DotTra"]))
                                          .DefaultIfEmpty(0)
                                          .Max();

                thuTuDotTraTien = thuTuDotTraTien + maxThuTu;

            }
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
            string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={mancc}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _isLoadingTyGia = true;
            if (tbl != null && tbl.Rows.Count > 0)
            {
                txtTienTe.EditValue = tbl.Rows[0]["TenTienTe"];
                spinEditTyGia.EditValue = tbl.Rows[0]["Gia"];
                _tienteID = tbl.Rows[0]["TIENTE"].ToString();
                _mancc = tbl.Rows[0]["MaNhaCC"].ToString();
            }
            _isLoadingTyGia = false;
            ApplyCurrencyFormat(spinEditTongCPVT, txtTienTe.Text);
            ApplyCurrencyFormat(spinEditTongCP, txtTienTe.Text);
            ApplyCurrencyFormat(spinEditTongCPVC, txtTienTe.Text);
            ApplyCurrencyFormat(spinEditTongCPPS, txtTienTe.Text);
            CreateSearchlookupPhieuBG(mancc);


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
            tbl.Columns.Add("MaTienTe", typeof(string));
            tbl.Columns.Add("SoNgayGHSom", typeof(int));
            tbl.Columns.Add("SoNgayGHTre", typeof(int));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            tbl.Columns.Add("ChiPhiSauCK", typeof(decimal));
            tbl.Columns.Add("MaThue_Gop", typeof(string));
            tbl.Columns.Add("MaChietKhau_Gop", typeof(string));
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
            tbl.Columns.Add("SoLuongVC", typeof(float));
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
            return tbl;
        }

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
            if (_IsEdit == false)
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    searchLookUpEditPBG.EditValue = tbl.Rows[0]["MaPhieuBG"];
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


            e.Handled = false;
        }



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DataTable dt = gridControl12.DataSource as DataTable;
            DataTable dt1 = gridControl2.DataSource as DataTable;
            DataTable dt2 = gridControl31.DataSource as DataTable;
            DataTable dt3 = gridControl42.DataSource as DataTable;
            if (searchLookUpEditNCC.EditValue == null || searchLookUpEditNCC.EditValue == "")
            {
                return;
            }
            if (searchLookUpEditPBG.EditValue == null || searchLookUpEditPBG.EditValue == "")
            {
                return;
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
                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    SaveDataHTTT(dt3);
                }
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
                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    SaveDataHTTT(dt3);
                }
            }


            gridControl12.DataSource = null;
            searchLookUpEditPBG.EditValue = null;
            searchLookUpEditNCC.EditValue = null;
            _dtTable = null;

            this.DialogResult = DialogResult.OK;
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_IsEdit == true)
            {
                searchLookUpEditNCC.EditValue = _mancc.ToString();
                searchLookUpEditPBG.EditValue = _maphieubg.ToString();
                LoadData();
                LoadDataChiPhi();
                LoadDataDotGH();
                LoadDataHTTT();
            }
            else
            {
                gridControl12.DataSource = null;
                gridControl2.DataSource = null;
                gridControl31.DataSource = null;
                gridControl42.DataSource = null;
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
                decimal soLuong = GetDecimalValue(view, e.RowHandle, "TongSLMuaThem");
                decimal donGia = GetDecimalValue(view, e.RowHandle, "DonGia");
                decimal thue = GetThuePercent(view, e.RowHandle, "Thue");
                decimal chietkhau = GetThuePercent(view, e.RowHandle, "ChietKhau");
                decimal thanhtienchietkhau = soLuong * donGia - (chietkhau * (soLuong * donGia));
                decimal thanhTien = thanhtienchietkhau + (thue * thanhtienchietkhau);

                decimal chiphivt = soLuong * donGia;
                decimal thanhTienvnd = thanhTien * Convert.ToDecimal(spinEditTyGia.EditValue);
                view.SetRowCellValue(e.RowHandle, "ChiPhiSauCK", Math.Round(thanhTien, 2));
                view.SetRowCellValue(e.RowHandle, "ChiPhiVT", Math.Round(chiphivt, 2));
                view.SetRowCellValue(e.RowHandle, "ThanhTien", Math.Round(thanhTien, 2));
                if (txtTienTe.Text != "VND")
                {
                    view.SetRowCellValue(e.RowHandle, "ThanhTienVND", Math.Round(thanhTienvnd, 2));
                }
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
            this.ActiveControl = simpleButton3;
            string url1 = $"{URL}NhaCC/GePMH?action=GETALLPHIEUMH&para1=&para2=&para3=&para4=&para5=";
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
                _maphieumh = "PHIEU_" + newPhieu;
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
                newRow["MaPhieuBG"] = searchLookUpEditPBG.EditValue?.ToString();
                newRow["MauVTID"] = dr["MauVTID"];
                newRow["KhoVaiID"] = dr["KhoVaiID"];
                newRow["MaDVVT"] = dr["MaDVVT"];
                newRow["NgayTao"] = DateTime.Now.Date;
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
                newRow["Thue"] = dr["Thue"];
                newRow["CPVanChuyen"] = dr["ChiPhiVanChuyen"];
                newRow["ChiPhiKhac"] = "";
                newRow["Dot"] = newDot;
                newRow["IsXacNhan"] = 0;
                newRow["NgayHieuLuc"] = dr["NgayHieuLuc"] == null ? DBNull.Value : dr["NgayHieuLuc"];
                newRow["ChiPhiVT"] = dr["ChiPhiVT"];
                // newRow["POMua"] = _mancc + "|" + textEditPOMua.Text;
                newRow["POMua"] = textEditPOMua.Text;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/Pospmh");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

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


        public void SaveDataCP(DataTable dt)
        {
            this.ActiveControl = simpleButton3;
            DataTable tblDG = CreateDatatableCPSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["MaPhieu"] = _maphieumh;
                newRow["MaChiPhi"] = dr["MaChiPhi"]?.ToString();
                newRow["SoTien"] = dr["SoTien"];
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now;
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["Thue"] = dr["Thue"];
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
            this.ActiveControl = simpleButton3;
            DataTable tblDG = CreateDatatableToSaveDot();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["MaPhieu"] = _maphieumh;
                newRow["ThuTu"] = dr["ThuTu"]?.ToString();
                newRow["MaPTVC"] = "";
                newRow["ChiPhiVC"] = dr["ChiPhiVC"]?.ToString();
                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                newRow["MaNhomVT"] = dr["MaNhomVT"]?.ToString();
                newRow["MaMauVT"] = dr["MaMauVT"]?.ToString();
                newRow["MaKhoVT"] = dr["MaKhoVT"]?.ToString();
                newRow["SoLuongVC"] = dr["SoLuongVC"];
                newRow["NgayBatDau"] = dr["NgayBatDau"] == null ? DBNull.Value : dr["NgayBatDau"];
                newRow["NhanMin"] = dr["NhanMin"] == null ? 0 : dr["NhanMin"];
                newRow["NhanMax"] = dr["NhanMax"] == null ? 0 : dr["NhanMax"];
                newRow["NgayNhanDK"] = dr["NgayNhanDK"] == null ? DBNull.Value : dr["NgayNhanDK"];
                newRow["NgayNhanTT"] = dr["NgayNhanTT"] == null ? DBNull.Value : dr["NgayNhanTT"];
                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                newRow["NgayTao"] = DateTime.Now;
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["Thue"] = dr["Thue"];
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
            DataTable dt = gridControl12.DataSource as DataTable;
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
                row["NgayXacNhan"] = DateTime.Now;
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
            this.ActiveControl = simpleButton3;
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
                newRow["MaPhieuBG"] = searchLookUpEditPBG.EditValue?.ToString();
                newRow["MauVTID"] = dr["MauVTID"];
                newRow["KhoVaiID"] = dr["KhoVaiID"];
                newRow["MaDVVT"] = dr["MaDVVT"];
                newRow["NgayTao"] =
                    dr.IsNull("NgayTao") ? DateTime.Now : dr.Field<DateTime>("NgayTao");
                newRow["NguoiTao"] = string.IsNullOrEmpty(dr["NguoiTao"].ToString()) ? GlobleData.UserName.ToString() : dr["NguoiTao"];
                bool isDuyetEmpty = dr.IsNull("IsDuyet")
                                    || string.IsNullOrWhiteSpace(dr["IsDuyet"].ToString());

                if (tblDG.Rows.Count > 0 && isDuyetEmpty)
                    newRow["IsDuyet"] = tblDG.Rows[tblDG.Rows.Count - 1]["IsDuyet"];
                else
                    newRow["IsDuyet"] = dr["IsDuyet"];

                newRow["MaCLVT"] = dr["MaCLVT"];
                newRow["TienTeID"] = dr["TienTeID"];
                newRow["NgaySua"] = DateTime.Now;
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
                newRow["POMua"] = _mancc + "|" + textEditPOMua.Text;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/Pospmh");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

        }

        public void UpdateXacNhan(DataTable dt)
        {
            this.ActiveControl = simpleButton3;
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
                newRow["MaPhieuBG"] = searchLookUpEditPBG.EditValue?.ToString();
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
                newRow["NgayXacNhan"] = DateTime.Now;
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

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = gridControl12.DataSource as DataTable;
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
                row["IsDuyet"] = 1;
            }
            if (_IsEdit == true)
            {
                Update(dt);
            }
            this.DialogResult = DialogResult.OK;
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


            int groupIndex = gridView31.GetRowLevel(e.RowHandle);

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

            e.Handled = false;
        }



        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            //string mancc = searchLookUpEditNCC.EditValue.ToString();
            //string maphieubg = searchLookUpEditPBG.EditValue.ToString();
            //CreateSearchlookupVatTu(mancc, maphieubg);
            if (_IsAdd == true)
            {
                string url1 = $"{URL}NhaCC/GePMH?action=GETALLPHIEUMH&para1=&para2=&para3=&para4=&para5=";
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

                _tenphieumh = "PHIẾU MH|" + searchLookUpEditNCC.Text.ToString() + "|" + newPhieu;

                txtPhieuMH.Text = _tenphieumh;
            }
            else
            {
                txtPhieuMH.Text = _tenphieumh;
            }

        }



        private void SetupNumericColumns()
        {
            // Tạo 1 repository dùng chung
            RepositoryItemTextEdit riNumeric = new RepositoryItemTextEdit();
            riNumeric.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            riNumeric.Mask.EditMask = "n2";
            riNumeric.Mask.UseMaskAsDisplayFormat = true;

            // Chặn số âm
            BlockNegative(riNumeric);

            // Những cột cần format numeric
            GridColumn[] numericColumns = new GridColumn[]
            {
                gridColumn610, gridColumn410, gridColumn271, gridColumn501,
                gridColumn32, gridColumn291, gridColumn38, gridColumn532,
                gridColumn571, gridColumn72, gridColumn621,gridColumn41, gridColumn61
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
            frmChiPhi frm = new frmChiPhi();
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
                    _dtTableCP.Rows.Add(newRow);
                }

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
            string mapbg = searchLookUpEditPBG.EditValue.ToString();
            string url = $"{URL}NhaCC/GetCP?action=GETPBGChiPhi&para1={mapbg}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtTableCP = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl2.DataSource = _dtTableCP;
            if (_dtTableCP != null && _dtTableCP.Rows.Count > 0)
            {

                DataRow[] rows = _dtTableCP.Select("MaChiPhi = 3 OR MaChiPhi = 6");

                if (rows.Length > 0)
                {
                    decimal totalSoTien = 0;
                    decimal totalThue = 0;

                    foreach (var r in rows)
                    {
                        decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                        decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);

                        totalSoTien += soTien;
                        totalThue += (thue / 100m) * soTien;
                    }

                    _tongvccpps = totalSoTien + totalThue;

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
            this.ActiveControl = simpleButton3;

            GridView view = gridView1;
            //int[] selectedRows = view.GetSelectedRows();

            if (view.RowCount == 0)
            {
                XtraMessageBox.Show("Không có dòng nào trong grid để thêm đợt.", "Thông báo");
                return;
            }

            if (_dtTableDot == null)
                _dtTableDot = CreateDatatableDot();

            bool isTableEmpty = (_dtTableDot.Rows.Count == 0);

            // Lấy danh sách vật tư đã chọn
            List<DataRow> vtList = new List<DataRow>();
            //foreach (int rowHandle in selectedRows)
            //{
            //    if (rowHandle >= 0)
            //    {
            //        var r = view.GetDataRow(rowHandle);
            //        if (r != null) vtList.Add(r);
            //    }
            //}
            for (int i = 0; i < view.RowCount; i++)
            {
                // GetDataRow trả về null cho group rows — an toàn
                var r = view.GetDataRow(i);
                if (r != null) vtList.Add(r);
            }
            if (vtList.Count == 0)
            {
                XtraMessageBox.Show("Không có dòng dữ liệu hợp lệ trong grid để thêm đợt.", "Thông báo");
                return;
            }

            // =====================
            //   LẦN ĐẦU → TẠO 2 ĐỢT
            // =====================
            if (isTableEmpty)
            {
                int dot1 = 1;
                int dot2 = 2;

                foreach (var src in vtList)
                {
                    decimal tong = Convert.ToDecimal(src["TongSLMuaThem"] == DBNull.Value ? 0 : src["TongSLMuaThem"]);

                    // --- ĐỢT 1 ---
                    DataRow dr1 = _dtTableDot.NewRow();
                    FillRowDot(dr1, dot1, src, tong);
                    _dtTableDot.Rows.Add(dr1);

                    // --- ĐỢT 2 ---
                    DataRow dr2 = _dtTableDot.NewRow();
                    FillRowDot(dr2, dot2, src, 0);
                    _dtTableDot.Rows.Add(dr2);
                }

                currentThuTu = GetNextAvailableThuTu();
                gridControl31.DataSource = _dtTableDot;
                gridControl31.RefreshDataSource();
                return;
            }

            // =============================
            //  KHÔNG PHẢI LẦN ĐẦU → THÊM 1 ĐỢT
            // =============================
            int nextThuTu = GetNextAvailableThuTu();

            foreach (var src in vtList)
            {
                decimal tong = Convert.ToDecimal(src["TongSLMuaThem"] == DBNull.Value ? 0 : src["TongSLMuaThem"]);

                // Tính phần còn lại
                decimal allocated = GetAllocatedSoLuongByMaVT(
                    src["MaVTID"].ToString(),
                    src["MauVTID"].ToString(),
                    src["KhoVaiID"].ToString(),
                    src["MaCLVT"].ToString()
                );

                decimal remaining = tong - allocated;
                if (remaining < 0) remaining = 0;

                DataRow dr = _dtTableDot.NewRow();
                FillRowDot(dr, nextThuTu, src, remaining);
                _dtTableDot.Rows.Add(dr);
            }

            currentThuTu = GetNextAvailableThuTu();
            gridControl31.DataSource = _dtTableDot;
            gridControl31.RefreshDataSource();
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
            dr["NgayTao"] = DateTime.Now;
            dr["MoTa"] = src["MoTa"];
            dr["NPL"] = src["NPL"];
            dr["ItemCode"] = src["ItemCode"];
            dr["MauVT"] = src["MauVT"];
            dr["KhoVai"] = src["KhoVai"];
            dr["TenDVVT"] = src["TenDVVT"];
            dr["Thue"] = _thuevc;
            dr["ThanhTienVC"] = 0;
            dr["ColorCode"] = src["ColorCode"];
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
                // validate tổng phân bổ không vượt quá TongSLMuaThem của gridView1
                string maVTID = view.GetRowCellValue(e.RowHandle, "MaVTID")?.ToString();
                string TenVT = view.GetRowCellValue(e.RowHandle, "MoTa")?.ToString();
                string mauvtid = view.GetRowCellValue(e.RowHandle, "MaMauVT")?.ToString();
                string khovtid = view.GetRowCellValue(e.RowHandle, "MaKhoVT")?.ToString();
                string maclvt = view.GetRowCellValue(e.RowHandle, "MaNhomVT")?.ToString();
                if (string.IsNullOrEmpty(maVTID)) return;

                // lấy giá trị TongSLMuaThem từ gridView1
                decimal tong = 0;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    var vt = gridView1.GetRowCellValue(i, "MaVTID");
                    if (vt != null && vt.ToString() == maVTID)
                    {
                        var vTong = gridView1.GetRowCellValue(i, "TongSLMuaThem");
                        if (vTong != null && vTong != DBNull.Value)
                            decimal.TryParse(vTong.ToString(), out tong);
                        break;
                    }
                }

                decimal totalAllocatedBefore = GetAllocatedSoLuongByMaVT(maVTID, mauvtid, khovtid, maclvt);

                // giá trị cũ
                decimal oldVal = 0;
                var oldObj = view.GetRowCellValue(e.RowHandle, "SoLuongVC");
                if (oldObj != null && oldObj != DBNull.Value)
                    decimal.TryParse(oldObj.ToString(), out oldVal);

                // giá trị mới (e.Value)
                decimal newVal = 0;
                if (e.Value != null && e.Value != DBNull.Value)
                    decimal.TryParse(e.Value.ToString(), out newVal);

                decimal totalAfter = totalAllocatedBefore - oldVal + newVal;

                if (totalAfter > tong)
                {
                    decimal allowedForThisRow = tong - (totalAllocatedBefore - oldVal);
                    if (allowedForThisRow < 0) allowedForThisRow = 0;

                    // unsubscribe để tránh recursion khi set lại
                    view.CellValueChanged -= gridView3_CellValueChanged;
                    view.SetRowCellValue(e.RowHandle, "SoLuongVC", allowedForThisRow);
                    view.CellValueChanged += gridView3_CellValueChanged;

                    XtraMessageBox.Show(
                        string.Format("Không thể nhập {0}. Tổng phân bổ cho vật tư {1} vượt quá Tổng yêu cầu ({2}). Giá trị đã được điều chỉnh về {3}.", newVal, TenVT, tong, allowedForThisRow),
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    CapNhatThanhTien_ByMaVT(maVTID, mauvtid, khovtid, maclvt);
                    return;
                }

                // --- MỚI: nếu sửa ở 1 đợt > 1 (đợt sau), thì tự trừ (hoặc cộng) vào đợt trước cùng mã để giữ tổng = tong ---
                // Lấy giá trị ThuTu/Thutu của hàng vừa sửa
                object curThuTuObj = view.GetRowCellValue(e.RowHandle, "ThuTu") ?? view.GetRowCellValue(e.RowHandle, "Thutu");
                int curThuTu = -1;
                if (curThuTuObj != null && curThuTuObj != DBNull.Value)
                    int.TryParse(curThuTuObj.ToString(), out curThuTu);

                // Nếu curThuTu > minimal (tức là có đợt trước), tìm đợt trước nhất (thuTu nhỏ hơn) cùng mã và điều chỉnh
                if (curThuTu > 0)
                {
                    // tìm hàng đợt trước cùng MaVTID, MaMauVT, MaKhoVT, MaNhomVT có ThuTu nhỏ nhất nhưng < curThuTu
                    DataRow rowToAdjust = null;
                    int bestThuTu = int.MaxValue;
                    foreach (DataRow r in _dtTableDot.Rows)
                    {
                        if (r == null) continue;
                        var vt = r["MaVTID"];
                        var mau = r["MaMauVT"];
                        var kho = r["MaKhoVT"];
                        var nhom = r["MaNhomVT"];
                        if (vt == null) continue;
                        if (vt.ToString() == maVTID && (mau ?? "").ToString() == (mauvtid ?? "").ToString() && (kho ?? "").ToString() == (khovtid ?? "").ToString() && (nhom ?? "").ToString() == (maclvt ?? "").ToString())
                        {
                            object tObj = r["ThuTu"] ?? r["Thutu"];
                            int t = -1;
                            if (tObj != null && tObj != DBNull.Value) int.TryParse(tObj.ToString(), out t);
                            if (t >= 0 && t < curThuTu)
                            {
                                if (t < bestThuTu)
                                {
                                    bestThuTu = t;
                                    rowToAdjust = r;
                                }
                            }
                        }
                    }

                    if (rowToAdjust != null)
                    {
                        // delta = newVal - oldVal: nếu delta > 0 (tăng ở đợt hiện), thì giảm tương ứng ở đợt trước; nếu delta < 0 thì tăng lại ở đợt trước
                        decimal delta = newVal - oldVal;

                        decimal prevVal = 0;
                        if (rowToAdjust["SoLuongVC"] != null && rowToAdjust["SoLuongVC"] != DBNull.Value)
                            decimal.TryParse(rowToAdjust["SoLuongVC"].ToString(), out prevVal);

                        decimal newPrevVal = prevVal - delta;

                        // đảm bảo không âm và không vượt tong
                        if (newPrevVal < 0) newPrevVal = 0;
                        // nếu newPrevVal > tong thì trim (không khả năng xảy ra thường)
                        if (newPrevVal > tong) newPrevVal = tong;

                        // cập nhật giá trị trong _dtTableDot và in lên grid (cẩn thận tránh recursion)
                        gridView3.CellValueChanged -= gridView3_CellValueChanged;
                        rowToAdjust["SoLuongVC"] = newPrevVal;
                        gridView31.RefreshData();
                        gridView3.CellValueChanged += gridView3_CellValueChanged;
                    }
                }

                // cập nhật tiền / tổng cho vật tư
                CapNhatThanhTien_ByMaVT(maVTID, mauvtid, khovtid, maclvt);
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
        }

        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            if (e.Column.FieldName == "SoTien" || e.Column.FieldName == "Thue")
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

            for (int i = 0; i < gridView31.RowCount; i++)
            {
                var vt = gridView31.GetRowCellValue(i, "MaVTID");
                var mauvt = gridView31.GetRowCellValue(i, "MaMauVT");
                var khovai = gridView31.GetRowCellValue(i, "MaKhoVT");
                var nhom = gridView31.GetRowCellValue(i, "MaNhomVT");
                if (vt == null) continue;

                if (vt.ToString() == maVTID && mauvt.ToString() == mauvtid && khovai.ToString() == khovtid && nhom.ToString() == maclvt)
                {
                    //var val = gridView3.GetRowCellValue(i, "ChiPhiVC");
                    //if (val != null && val != DBNull.Value)
                    //    tong += Convert.ToDecimal(val);

                    var valChiPhi = gridView31.GetRowCellValue(i, "ChiPhiVC");
                    var valThue = gridView31.GetRowCellValue(i, "Thue");

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
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            GridView view = gridControl31.MainView as GridView;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    if (_IsEdit == true)
                    {
                        string thutu = row["ThuTu"].ToString();
                        string mavtid = row["MaVTID"].ToString();
                        string mauvt = row["MaMauVT"].ToString();
                        string makho = row["MaKhoVT"].ToString();
                        string url = string.Format("{0}?para={1}&para1={2}&para2={3}&para3={4}&para4={5}", URL + "NhaCC/DeleteDotPMH", _maphieumh, thutu, mavtid, mauvt, makho);
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    }
                    ((DataTable)gridControl31.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
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
            gridView31.PostEditor();

            gridView1.UpdateCurrentRow();
            gridView2.UpdateCurrentRow();
            gridView31.UpdateCurrentRow();

            decimal tongThanhTien = 0;
            decimal tongchiphips = 0;
            decimal tongchiphivc = 0;

            // Grid1: tổng ThanhTien
            for (int i = 0; i < gridView1.RowCount; i++)
                tongThanhTien += GetDecimalValue(gridView1, i, "ThanhTien");

            // Grid2: tổng chi phí phụ: SoTien * (1 + Thue%)
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                object maChiPhiObj = gridView2.GetRowCellValue(i, "MaChiPhi");
                if (maChiPhiObj == null) continue;

                int maChiPhi = Convert.ToInt32(maChiPhiObj);

                if (maChiPhi == 3 || maChiPhi == 6)
                    continue;

                decimal soTien = GetDecimalValue(gridView2, i, "SoTien");
                decimal thue = GetDecimalValue(gridView2, i, "Thue");

                decimal thuePercent = thue / 100m;
                decimal thanhTien = soTien + (thuePercent * soTien);
                tongchiphips += thanhTien;
            }

            // Grid3: tổng chi phí vận chuyển theo từng dòng: SoLuongVC * ChiPhiVC * (1 + Thue%)
            //if (_dtTableDot != null && _dtTableDot.Rows.Count > 0)
            //{
            //    for (int i = 0; i < gridView31.RowCount; i++)
            //    {
            //        //decimal sl = GetDecimalValue(gridView31, i, "SoLuongVC");
            //        decimal cp = GetDecimalValue(gridView31, i, "ChiPhiVC");
            //        decimal thue = GetDecimalValue(gridView31, i, "Thue");
            //        decimal thuePercent = thue / 100m;
            //        decimal thanhTienVC =  cp + (thuePercent * cp);
            //        tongchiphivc += thanhTienVC;
            //    }
            //}
            //else
            //{
            //    tongchiphivc = _tongvccpps;
            //}
            tongchiphivc = _tongvccpps;
            // Gán các spinEdit con
            spinEditTongCPVT.EditValue = tongThanhTien;
            spinEditTongCPPS.EditValue = tongchiphips;
            spinEditTongCPVC.EditValue = tongchiphivc;

            // Tổng CP (VT + PS)
            decimal totalCP = GetDecimalFromObject(spinEditTongCPVT.EditValue) + GetDecimalFromObject(spinEditTongCPPS.EditValue) + GetDecimalFromObject(spinEditTongCPVC.EditValue);
            spinEditTongCP.EditValue = totalCP;

            // Nếu tiền tệ khác VND thì chuyển đổi
            string loaitien = txtTienTe.Text;
            decimal tyGia = GetDecimalFromObject(spinEditTyGia.EditValue);
            if (!string.IsNullOrEmpty(loaitien) && loaitien != "VND")
            {
                spinEditTongCPVND.EditValue = totalCP * tyGia;
            }
            else
            {
                spinEditTongCPVND.EditValue = totalCP;
            }



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
            //if (searchLookUpEditNCC.EditValue == null || searchLookUpEditNCC.EditValue == "")
            //{
            //    XtraMessageBox.Show("Chưa chọn nhà cung cấp.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //if (searchLookUpEditPBG.EditValue == null || searchLookUpEditPBG.EditValue == "")
            //{
            //    XtraMessageBox.Show("Chưa chọn phiếu báo giá.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //string mancc = searchLookUpEditNCC.EditValue.ToString();
            //string maphieubg = searchLookUpEditPBG.EditValue.ToString();
            //frmChonVTPhieu frm = new frmChonVTPhieu(mancc, maphieubg);
            //if (_dtTable != null && _dtTable.Rows.Count > 0)
            //{
            //    frm.ListExistingVT = _dtTable.Copy();
            //}
            //if (frm.ShowDialog() == DialogResult.OK)
            //{
            //    DataTable selectedTable = frm._tbl;
            //    if (selectedTable == null || selectedTable.Rows.Count == 0)
            //        return;
            //    if (_dtTable == null || _dtTable.Rows.Count == 0)
            //    {
            //        _dtTable = CreateDatatable();
            //    }
            //    _dtTableChiTiet = CreateDatatablePhieuDH();
            //    foreach (DataRow row in selectedTable.Rows)
            //    {
            //        DataRow newRow = _dtTable.NewRow();
            //        newRow["MaVTID"] = row["MaVTID"];
            //        newRow["MauVTID"] = row["MauVTID"];
            //        newRow["KhoVaiID"] = row["KhoVaiID"];
            //        newRow["MaDVVT"] = row["MaDVVT"];
            //        newRow["ItemCode"] = row["ItemCode"];
            //        newRow["NPL"] = row["NPL"];
            //        newRow["DonGia"] = row["DonGia"];
            //        newRow["TongSL"] = row["TongSL"];
            //        newRow["TongSLMuaThem"] = row["TongSLMuaThem"];
            //        newRow["ThanhTien"] = row["ThanhTien"];
            //        newRow["MoTa"] = row["MoTa"];
            //        newRow["MaCLVT"] = row["MaCLVT"];
            //        newRow["KhoVai"] = row["KhoVai"];
            //        newRow["TenDVVT"] = row["TenDVVT"];
            //        newRow["MauVT"] = row["MauVT"];
            //        newRow["TenCL"] = row["TenCL"];
            //        newRow["Thue"] = row["Thue"];
            //        newRow["PTThanhToan"] = row["PTThanhToan"];
            //        newRow["ChiPhiVanChuyen"] = row["ChiPhiVanChuyen"];
            //        newRow["ChiPhiKhac"] = "";
            //        newRow["PTVanChuyen"] = row["PTVanChuyen"];
            //        newRow["NgayDuKienHV"] = "";
            //        newRow["GhiChu"] = row["GhiChu"];
            //        newRow["ChiPhiVT"] = row["ChiPhiVT"];
            //        newRow["NgayHieuLuc"] = row["NgayBDHieuLuc"];
            //        newRow["TienTeID"] = row["TienTeID"];
            //        newRow["MaTienTe"] = row["MaTienTe"];
            //        newRow["SoNgayGHSom"] = row["SoNgayGHSom"];
            //        newRow["SoNgayGHTre"] = row["SoNgayGHTre"];
            //        newRow["ColorCode"] = row["MaMauVT"];
            //        newRow["ChietKhau"] = row["ChietKhau"];
            //        newRow["ThanhTienVND"] = row["ThanhTienVND"];
            //        newRow["ChiPhiSauCK"] = row["ChiPhiSauCK"];
            //        newRow["MaThue_Gop"] = row["MaThue_Gop"];
            //        newRow["MaChietKhau_Gop"] = row["MaChietKhau_Gop"];

            //        _dtTable.Rows.Add(newRow);
            //    }
            //    HashSet<string> addedMaDH = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //    foreach (DataRow row in selectedTable.Rows)
            //    {
            //        string rawDH = row["MaDHGOP"].ToString();
            //        string[] listDH = rawDH.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            //        foreach (string dh in listDH)
            //        {
            //            string maDH = dh.Trim();
            //            if (addedMaDH.Contains(maDH)) continue;

            //            string urlml = $"{URL}NhaCC/GePMH?action=GETMaLenh&para1={maDH}&para2=&para3=&para4=&para5=";
            //            string jsonml = Task.Run(async () => await _clientExtension.GetAsnyc(urlml)).Result;

            //            string maLenhSX = "";
            //            if (!string.IsNullOrEmpty(jsonml))
            //            {
            //                try
            //                {
            //                    JArray arr = JArray.Parse(jsonml);
            //                    if (arr.Count > 0 && arr[0]["MaLenhSanXuat"] != null)
            //                    {
            //                        maLenhSX = arr[0]["MaLenhSanXuat"].ToString().Trim();
            //                    }
            //                }
            //                catch
            //                {
            //                    maLenhSX = jsonml.Replace("\"", "").Trim();
            //                }
            //            }
            //            DataRow newRow = _dtTableChiTiet.NewRow();
            //            newRow["ID"] = 0;
            //            newRow["MaPhieuMH"] = "";
            //            newRow["MaDH"] = maDH;
            //            newRow["MaLenhSX"] = maLenhSX;
            //            _dtTableChiTiet.Rows.Add(newRow);

            //            addedMaDH.Add(maDH);
            //        }
            //    }

            //    CreateSearchlookupcolPTThanhToan();
            //    gridControl12.DataSource = _dtTable;

            //    if (_IsEdit == false)
            //    {
            //        LoadDataCP();
            //        //themdot();
            //    }

            //    UpdateGridView5Summary();
            //    UpdateGridView5Values();

            //    int soChungLoai = _dtTable.AsEnumerable()
            //                  .Select(r => r["MaCLVT"].ToString())
            //                  .Distinct()
            //                  .Count();

            //    spinEditSoLoaiVT.EditValue = soChungLoai;
            //}

        }

        private void btnThemHT_Click(object sender, EventArgs e)
        {
            this.ActiveControl = simpleButton3;

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
                    .Select(r =>
                    {
                        int val;
                        return int.TryParse(r["DotTra"].ToString(), out val) ? val : 0;
                    })
                    .Max() + 1;
            }
            // ====== TẠO DÒNG MỚI ======
            DataRow dr = _dtTableHTTT.NewRow();
            dr["ID"] = 0;
            dr["MaPhieuMH"] = "";
            dr["TGTra"] = DateTime.Now.ToString("dd/MM/yyyy");
            dr["GhiChu"] = "";
            dr["PTThanhToan"] = "";

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
            gridControl42.DataSource = _dtTableHTTT;
            gridControl42.RefreshDataSource();
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
                    XtraMessageBox.Show("Tổng tỷ lệ phần trăm không được vượt quá 100%.",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    _isUpdatingGV5 = true;
                    view.SetRowCellValue(e.RowHandle, e.Column, _oldValue);
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

        public void SaveDataHTTT(DataTable dt)
        {
            this.ActiveControl = simpleButton3;
            DataTable tblHT = CreateDatatableHTTTSave();

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblHT.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieuMH"] = _maphieumh;
                newRow["SoTien"] = dr["SoTien"];
                newRow["TGTra"] = DateTime.Now;
                newRow["DotTra"] = dr["DotTra"];
                newRow["GhiChu"] = dr["GhiChu"]?.ToString();
                newRow["PhanTram"] = dr["PhanTram"];
                newRow["PTThanhToan"] = dr["PTThanhToan"];
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

        private void btnXoaHT_Click(object sender, EventArgs e)
        {
            GridView view = gridControl42.MainView as GridView;
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
                    ((DataTable)gridControl42.DataSource).Rows.Remove(row);
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

            for (int i = 0; i < gridView31.RowCount; i++)
            {
                //decimal sl = Convert.ToDecimal(gridView3.GetRowCellValue(i, "SoLuongVC") ?? 0);
                decimal cp = Convert.ToDecimal(gridView31.GetRowCellValue(i, "ChiPhiVC") ?? 0);
                decimal thue = Convert.ToDecimal(gridView31.GetRowCellValue(i, "Thue") ?? 0);

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


        private void gridView52_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;

            e.Appearance.BackColor = Color.LightYellow;

            e.Appearance.ForeColor = Color.Red;

            e.Appearance.Font = new Font("Tahoma", 8F, FontStyle.Bold);
        }



        private async void SaveTyGia()
        {
            this.ActiveControl = simpleButton3;
            DataTable tblQD = CreateDatatableQuyDoiSave();
            DataRow newRow = tblQD.NewRow();
            newRow["ID"] = 0;
            newRow["QuyDoiID"] = "";
            newRow["MaTienTe"] = _tienteID;
            newRow["Gia"] = spinEditTyGia.EditValue;
            newRow["Ngay"] = DateTime.Now.ToString("yyyy-MM-dd");
            newRow["UserID"] = GlobleData.UserName.ToString();
            newRow["NguoiSua"] = "";
            newRow["NgaySua"] = DBNull.Value;
            tblQD.Rows.Add(newRow);
            string mgQD = await _clientExtension.PostAsync(URL + $"TIENTE/Post2?action=POSTQUYDOI", tblQD);
            if (mgQD == string.Empty)
            {
                clsWaitForm.ShowErrorForm(this, 3000);
            }
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

                gridControl42.RefreshDataSource();
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
            gridColumn642.ColumnEdit = rCountryEdittt;
        }


        private void CapNhatTongChiPhiVC()
        {
            _tongvccpps = 0;

            if (_dtTableCP == null) return;

            // Chỉ chọn MaChiPhi = 3 hoặc 6
            DataRow[] rows = _dtTableCP.Select("MaChiPhi IN (3, 6)");

            foreach (DataRow r in rows)
            {
                decimal soTien = Convert.ToDecimal(r["SoTien"] ?? 0);
                decimal thue = Convert.ToDecimal(r["Thue"] ?? 0);

                decimal thuePercent = thue / 100m;
                decimal tien = soTien + (thuePercent * soTien);

                _tongvccpps += tien;
            }
        }

       

        private void textEditPOMua_Properties_Enter(object sender, EventArgs e)
        {

        }

        

        private void textEditPOMua_Properties_Leave(object sender, EventArgs e)
        {

        }

        

        private void textEditPOMua_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

        }

        


        /**/
        #region Đợt Giao Hàng

        #endregion

        #region Tam


        private void loadgridDotGiaoHang()
        {
            gridDotGiaoHang.DataSource = gridDotGiaoHangTable;
            foreach (GridColumn col in gridDotGiaoHangView.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }

        }
        private void getChiTietNhapKho()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getchitietnhapkhonpl";
            request.Parameter = _maphieumh;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            chitietnhapkhoTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!chitietnhapkhoTable.Columns.Contains("generatedID"))
                chitietnhapkhoTable.Columns.Add("generatedID");
            foreach (DataRow row in chitietnhapkhoTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }        
        private void calcInnerJoinVattuWithNhapKho()
        {
            DataTable dt = gridControl12.DataSource as DataTable;
            DataTable vattuTable = dt.Copy();
            DataTable nhapkhoTable = chitietnhapkhoTable.Copy();

            var queryJoin = from t1 in vattuTable.AsEnumerable()
                            join t2 in nhapkhoTable.AsEnumerable()
                            on new
                            {
                                MaVTID = t1.Field<string>("MaVTID"),
                                MaNhom = t1.Field<string>("MaCLVT"),
                                MauVTID = t1.Field<string>("MauVTID"),
                                KhoVaiID = t1.Field<string>("KhoVaiID")
                            }
                            equals new
                            {
                                MaVTID = t2.Field<string>("MaVTID"),
                                MaNhom = t2.Field<string>("MaNhom"),
                                MauVTID = t2.Field<string>("MauVTID"),
                                KhoVaiID = t2.Field<string>("KhoVaiID")
                            }
                            select new
                            {
                                TenCL = t1.Field<string>("TenCL"),
                                MaVTID = t1.Field<string>("MaVTID"),
                                MaNhom = t1.Field<string>("MaCLVT"),
                                MauVTID = t1.Field<string>("MauVTID"),
                                KhoVaiID = t1.Field<string>("KhoVaiID"),
                                ItemCode = t1.Field<string>("ItemCode"),
                                MoTa = t1.Field<string>("MoTa"),
                                ColorCode = t1.Field<string>("ColorCode"),
                                MauVT = t1.Field<string>("MauVT"),
                                KhoVai = t1.Field<string>("KhoVai"),
                                TenDVVT = t1.Field<string>("TenDVVT"),
                                LoaiNPL = t1.Field<string>("LoaiNPL"),
                                TongSLMuaThem = XuLyVTUnits.SmartTryParse<decimal>(
                                                    t1.Field<double>("TongSLMuaThem").ToString()),
                                SoLuongThucTe = XuLyVTUnits.SmartTryParse<decimal>(
                                                    t2.Field<double>("SoLuongThucTe")),
                                NgayNhapKho = t2.Field<string>("NgayNhapKho")
                            };

            DataTable result = new DataTable();
            result.Columns.Add("TenCL", typeof(string));
            result.Columns.Add("MaVTID", typeof(string));
            result.Columns.Add("MaNhom", typeof(string));
            result.Columns.Add("MauVTID", typeof(string));
            result.Columns.Add("KhoVaiID", typeof(string));
            result.Columns.Add("ItemCode", typeof(string));
            result.Columns.Add("MoTa", typeof(string));
            result.Columns.Add("ColorCode", typeof(string));
            result.Columns.Add("MauVT", typeof(string));
            result.Columns.Add("KhoVai", typeof(string));
            result.Columns.Add("TenDVVT", typeof(string));
            result.Columns.Add("LoaiNPL", typeof(string));
            result.Columns.Add("TongSLMuaThem", typeof(decimal));
            result.Columns.Add("SoLuongThucTe", typeof(decimal));
            result.Columns.Add("NgayNhapKho", typeof(string));

            foreach (var row in queryJoin)
            {
                result.Rows.Add(row.TenCL, row.MaVTID, row.MaNhom, row.MauVTID, row.KhoVaiID,
                                row.ItemCode, row.MoTa, row.ColorCode, row.MauVT, row.KhoVai,
                                row.TenDVVT, row.LoaiNPL, row.TongSLMuaThem,
                                row.SoLuongThucTe, row.NgayNhapKho);
            }
            foreach(DataRow rsRow in result.Rows)
            {
                rsRow["NgayNhapKho"] = "Đợt giao ngày " + rsRow["NgayNhapKho"].ToString();
            }
            gridDotGiaoHangTable = result.Copy();
        }
        private void gridDotGiaoHangView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "SoLuongThucTe")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
        }


        private void loadgridTienDoGiaoHang()
        {
            gridTienDoGiaoHang.DataSource = gridTienDoGiaoHangTable;
            foreach (GridColumn col in gridTienDoGiaoHangView.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }

        }
        private void getChiTietNhapKhoTong()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getchitietnhapkhonpltong";
            request.Parameter = _maphieumh;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            chitietnhapkhotongTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!chitietnhapkhotongTable.Columns.Contains("generatedID"))
                chitietnhapkhotongTable.Columns.Add("generatedID");
            foreach (DataRow row in chitietnhapkhotongTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void calcJoinVattuWithNhapKho()
        {
            DataTable dt = gridControl12.DataSource as DataTable;
            DataTable vattuTable = dt.Copy();
            DataTable nhapkhoTable = chitietnhapkhotongTable.Copy();

            var queryJoin = from t1 in vattuTable.AsEnumerable()
                            join t2 in nhapkhoTable.AsEnumerable()
                            on new
                            {
                                MaVTID = t1.Field<string>("MaVTID"),
                                MaNhom = t1.Field<string>("MaCLVT"),
                                MauVTID = t1.Field<string>("MauVTID"),
                                KhoVaiID = t1.Field<string>("KhoVaiID")
                            }
                            equals new
                            {
                                MaVTID = t2.Field<string>("MaVTID"),
                                MaNhom = t2.Field<string>("MaNhom"),
                                MauVTID = t2.Field<string>("MauVTID"),
                                KhoVaiID = t2.Field<string>("KhoVaiID")
                            }
                            into gj
                            from subT2 in gj.DefaultIfEmpty()   // đây chính là LEFT JOIN
                            select new
                            {
                                TenCL = t1.Field<string>("TenCL"),
                                MaVTID = t1.Field<string>("MaVTID"),
                                MaNhom = t1.Field<string>("MaCLVT"),
                                MauVTID = t1.Field<string>("MauVTID"),
                                KhoVaiID = t1.Field<string>("KhoVaiID"),
                                ItemCode = t1.Field<string>("ItemCode"),   // cột từ vattuTable
                                MoTa = t1.Field<string>("MoTa"),
                                ColorCode = t1.Field<string>("ColorCode"),
                                MauVT = t1.Field<string>("MauVT"),
                                KhoVai = t1.Field<string>("KhoVai"),
                                TenDVVT = t1.Field<string>("TenDVVT"),
                                LoaiNPL = t1.Field<string>("LoaiNPL"),
                                TongSLMuaThem = XuLyVTUnits.SmartTryParse<decimal>(t1.Field<double>("TongSLMuaThem").ToString()),
                                SoLuongThucTe = subT2 == null ? 0 : XuLyVTUnits.SmartTryParse<decimal>(subT2.Field<double>("SoLuongThucTe")), // cột từ nhapkhoTable
                            };

            DataTable result = new DataTable();
            result.Columns.Add("TenCL", typeof(string));
            result.Columns.Add("MaVTID", typeof(string));
            result.Columns.Add("MaNhom", typeof(string));
            result.Columns.Add("MauVTID", typeof(string));
            result.Columns.Add("KhoVaiID", typeof(string));
            result.Columns.Add("ItemCode", typeof(string));
            result.Columns.Add("MoTa", typeof(string));
            result.Columns.Add("ColorCode", typeof(string));
            result.Columns.Add("MauVT", typeof(string));
            result.Columns.Add("KhoVai", typeof(string));
            result.Columns.Add("TenDVVT", typeof(string));
            result.Columns.Add("LoaiNPL", typeof(string));
            result.Columns.Add("TongSLMuaThem", typeof(decimal));
            result.Columns.Add("SoLuongThucTe", typeof(decimal));

            foreach (var row in queryJoin)
            {
                result.Rows.Add(row.TenCL, row.MaVTID, row.MaNhom, row.MauVTID, row.KhoVaiID, row.ItemCode, row.MoTa, row.ColorCode, row.MauVT, row.KhoVai,
                    row.TenDVVT, row.LoaiNPL, row.TongSLMuaThem, row.SoLuongThucTe);
            }
            gridTienDoGiaoHangTable = result.Copy();
        }
        private void gridTienDoGiaoHangView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "SoLuongThucTe")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TongSLMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TyLeNhanHang")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
        }
        private void gridTienDoGiaoHangView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "TyLeNhanHang" && e.IsGetData)
            {
                DataRow row = ((DataRowView)e.Row).Row;
                decimal slDaMua = XuLyVTUnits.SmartTryParse<decimal>(row["SoLuongThucTe"].ToString());
                decimal slTong = XuLyVTUnits.SmartTryParse<decimal>(row["TongSLMuaThem"].ToString());
                e.Value = slDaMua * 100 / slTong;
            }
        }
        private void gridTienDoGiaoHangView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeNhanHang")
            {
                object val = view.GetRowCellValue(e.RowHandle, e.Column);
                if (val == null || val == DBNull.Value) return;

                decimal percent;
                if (!decimal.TryParse(val.ToString(), out percent)) return;
                if (percent == 0) return;
                percent = percent / 100;
                // Giới hạn từ 0 đến 1 (0% đến 100%)
                if (percent < 0) percent = 0;
                if (percent > 1) percent = 1;

                int fillWidth = (int)(e.Bounds.Width * percent);
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, fillWidth, e.Bounds.Height);
                e.Graphics.FillRectangle(Brushes.LightGreen, rect);

                // Vẽ viền (tuỳ chọn)
                //e.Graphics.DrawRectangle(Pens.DarkGreen, rect);

                // Vẽ chữ sau cùng để không bị đè
                e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);

                // Đánh dấu đã custom draw
                e.Handled = true;
            }
        }
        #endregion
    }
}
