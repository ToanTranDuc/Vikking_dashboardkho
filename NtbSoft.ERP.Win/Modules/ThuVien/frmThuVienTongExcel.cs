using DevExpress.DataProcessing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmThuVienTongExcel : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable _dtTS = new DataTable();
        DataTable _dtDM = new DataTable();
        DataTable _dtMauSP = new DataTable();
        DataTable _dtSize = new DataTable();

        DataTable _dtThongTinVT = new DataTable();
        DataTable _dtThongTinDM = new DataTable();
        string _makh = string.Empty, _tenKH = string.Empty, _mahang = string.Empty, _tenhang = string.Empty;
        string _magop = string.Empty, _maLSX = string.Empty, _madh = string.Empty;
        RepositoryItemSearchLookUpEdit rCountryEditdv = new RepositoryItemSearchLookUpEdit();
        RepositoryItemSearchLookUpEdit rCountryEditkvNL;
        RepositoryItemSearchLookUpEdit rCountryEditkvPL;
        DataTable _tblNhom = new DataTable();
        int _stt = 0;
        string _madot = string.Empty;
        DataTable tblCNL = new DataTable();
        DataTable tblCPL = new DataTable();
        DataTable tblts = new DataTable();
        DataTable tblThongSo = new DataTable();
        DataTable tblChiTiet = new DataTable();
        DataTable tblDinhMucCapPhat = new DataTable();
        bool _isNPL = true;
        bool _isDinhMuc = true;
        public frmThuVienTongExcel(DataTable dtTS, DataTable dtDM, DataTable dtMauSP, DataTable dtSize, string makh, string tenKH, string mahang, string tenhang, bool IsDinhMuc)
        {
            InitializeComponent(); URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dtTS = dtTS;
            _dtDM = dtDM;
            _dtMauSP = dtMauSP;
            _dtSize = dtSize;
            _makh = makh;
            _tenKH = tenKH;
            _mahang = mahang;
            _tenhang = tenhang;
            searchLookUpEditKH.Text = _tenKH;
            searchLookUpEditMH.Text = _tenhang;
            txtKH.Text = _tenKH;
            txtMH.Text = _tenhang;
            _isDinhMuc = IsDinhMuc;
            if(!IsDinhMuc)
            {
                xtraTabPage2.PageVisible = false;
            }    
        }
        protected override void OnLoad(EventArgs e)
        {
            _dtThongTinVT = createTableThongTinVatTu();
            _dtThongTinDM = createTableThongTinDinhMuc();
            tblChiTiet = CreateDatatable();
            CreateRepoSearchLookUpChungLoaiCT();
            CreateDatatable();
            LoadDaTaVatTu();
            LoadDaTaDinhMuc();
            CreateDotSTT();
            LoadThongSoVatTu();
            loadDataTSVTCoSan();
            loadDataCheckTrung();
            setTrung(tblCNL);
            setTrung(tblCPL);
            //loadSearchLookUpLenhSX();

        }
        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("tblChiTiet");
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaNPL", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("TenVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("MauID", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("DinhMuc", typeof(decimal));
            tbl.Columns.Add("CapPhat", typeof(decimal));
            tbl.Columns.Add("CapThem", typeof(decimal));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("DinhMucHaoHut", typeof(float));
            tbl.Columns.Add("NPL", typeof(bool));
            tbl.Columns.Add("ThucXuat", typeof(decimal));
            tbl.Columns.Add("DinhMucChung", typeof(decimal));
            tbl.Columns.Add("CapPhatTK", typeof(decimal));
            tbl.Columns.Add("NhuCau", typeof(decimal));
            tbl.Columns.Add("TachMau", typeof(decimal));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("TachMauSPDH", typeof(int));
            tbl.Columns.Add("MaCode", typeof(string));
            tbl.Columns.Add("QDLe", typeof(bool));
            tbl.Columns.Add("STTIndex", typeof(int));
            return tbl;
        }
        private DataTable createTableThongTinVatTu()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("NPL", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DinhMucChung", typeof(decimal));
            tbl.Columns.Add("DinhMucHaoHut", typeof(decimal));
            tbl.Columns.Add("TachMau", typeof(bool));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("IsNew", typeof(int));
            tbl.Columns.Add("STTCode", typeof(int));
            tbl.Columns.Add("MaCode", typeof(string));
            tbl.Columns.Add("STT", typeof(int));
            tbl.Columns.Add("STTIndex", typeof(int));
            tbl.Columns.Add("SLCap", typeof(decimal));
            tbl.Columns.Add("SLCL", typeof(decimal));
            tbl.Columns.Add("TonKho", typeof(decimal));
            tbl.Columns.Add("Status", typeof(int));
            tbl.Columns.Add("QDLe", typeof(bool));

            tbl.Columns.Add("IsActive", typeof(bool));
            tbl.Columns.Add("MaNhomChiTiet", typeof(string));
            tbl.Columns.Add("TenDVKV", typeof(string));
            tbl.Columns.Add("KhoVaiHienThi", typeof(string));
          
            return tbl;
        }
        private DataTable createTableThongTinDinhMuc()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("AllSize", typeof(decimal));
            tbl.Columns.Add("CheckAll", typeof(bool));
            tbl.Columns.Add("DinhMucGY", typeof(decimal));
            tbl.Columns.Add("IsXetDuyet", typeof(string));
            tbl.Columns.Add("NgayXet", typeof(string));
            tbl.Columns.Add("NguoiXet", typeof(string));
            tbl.Columns.Add("DinhMucHaoHut", typeof(decimal));
            tbl.Columns.Add("TachMau", typeof(bool));
            tbl.Columns.Add("MaCode", typeof(string));
            tbl.Columns.Add("STTCode", typeof(int));
            tbl.Columns.Add("STTIndex", typeof(int));
            tbl.Columns.Add("IsActive", typeof(bool));
            tbl.Columns.Add("MaNhomChiTiet", typeof(string));
            tbl.Columns.Add("TenDVKV", typeof(string));
            tbl.Columns.Add("KhoVaiHienThi", typeof(string));
            return tbl;
        }
        private void loadSearchLookUpLenhSX()
        {
            searchLookUpEditLenhSX.Properties.DisplayMember = "DisplayMember";
            searchLookUpEditLenhSX.Properties.ValueMember = "MaLenhSX";
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETLENHSX&para1={_makh}&para2={_mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditLenhSX.Properties.DataSource = tbl;

        }
        private void LoadDaTaVatTu()
        {
            if (_dtDM == null || _dtDM.Rows.Count == 0) return;
           
            foreach (DataRow _dr in _dtDM.Rows)
            {

                var existingRow = _dtThongTinVT.AsEnumerable().FirstOrDefault(row =>
                                             row.Field<string>("MaNhom") == _dr["MaNhom"].ToString() &&
                                             row.Field<string>("MaVT") == _dr["MaVT"].ToString() &&
                                             row.Field<string>("ChiTiet") == _dr["ChiTiet"].ToString() &&
                                             row.Field<string>("MaMauVT") == _dr["MaMauVT"].ToString() &&
                                             row.Field<string>("MauVT") == _dr["MauVT"].ToString() &&
                                             row.Field<string>("KhoVai") == _dr["KhoVai"].ToString()
                                             && row["STTIndex"].ToString() == _dr["STTIndex"].ToString()
                                             );

                if (existingRow != null)
                {
                    string existingMaMau = existingRow["MaMau"].ToString();
                    string existingTenMau = existingRow["TenMau"].ToString();
                    string newMaMau = _dr["MaMau"].ToString();
                    string newTenMau = _dr["TenMau"].ToString();

                    if (!existingMaMau.Split('|').Contains(newMaMau))
                    {
                        existingRow["MaMau"] = string.IsNullOrEmpty(existingMaMau) ? newMaMau : existingMaMau + "|" + newMaMau;
                        existingRow["TenMau"] = string.IsNullOrEmpty(existingTenMau) ? newTenMau : existingTenMau + "|" + newTenMau;
                    }
                }
                else
                {
                    DataRow nr = _dtThongTinVT.NewRow();
                    nr["MaNhom"] = _dr["MaNhom"].ToString();
                    nr["TenNhom"] = _dr["TenNhom"].ToString();
                    nr["NPL"] = _dr["NPL"];
                    nr["Sort"] = _dr["Sort"];
                    nr["MaVTID"] = _dr["MaVTID"].ToString();
                    nr["MaVT"] = _dr["MaVT"].ToString();
                    nr["ChiTiet"] = _dr["ChiTiet"].ToString();
                    nr["MaDVVT"] = _dr["MaDVVT"].ToString();
                    nr["TenDVVT"] = _dr["TenDVVT"].ToString();
                    nr["MauVTID"] = _dr["MauVTID"].ToString();
                    nr["MaMauVT"] = _dr["MaMauVT"].ToString();
                    nr["MauVT"] = _dr["MauVT"].ToString();
                    nr["KhoVaiID"] = _dr["KhoVaiID"].ToString();
                    nr["KhoVai"] = _dr["KhoVai"].ToString();
                    nr["MaMau"] = _dr["MaMau"].ToString();
                    nr["TenMau"] = _dr["TenMau"].ToString();
                    nr["DinhMucChung"] = 0;
                    nr["DinhMucHaoHut"] = _dr["HaoHut"];
                    nr["TachMau"] = true;
                    nr["MaVTGhep"] = _dr["MaVTGhep"].ToString();
                    nr["IsNew"] = 0;
                    nr["MaCode"] = "";
                    nr["STTCode"] = 0;
                    nr["STTIndex"] = _dr["STTIndex"].ToString();
                    nr["TenDVKV"] = _dr["TenDVKV"].ToString();
                    nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"].ToString();
                    //nr["TenDVKV"] = _dr["TenMau"].ToString();
                    _dtThongTinVT.Rows.Add(nr);

                }
            }
            var groupeds = _dtThongTinVT.AsEnumerable()
               .Where(row => !(row.IsNull("MaVT") || row.IsNull("ChiTiet") || row.IsNull("MaNhom") ||
                               row.IsNull("MaMauVT") || row.IsNull("MauVT") || row.IsNull("KhoVai") || row.IsNull("STTIndex")))
               .GroupBy(row => string.Join("|", new string[]
               {
        row["MaVT"].ToString().Trim(),
        row["ChiTiet"].ToString().Trim(),
        row["MaNhom"].ToString().Trim(),
        row["MaMauVT"].ToString().Trim(),
        row["MauVT"].ToString().Trim(),
        row["KhoVai"].ToString().Trim()
               }));

            foreach (var group in groupeds)
            {
                // Lấy danh sách STTIndex duy nhất (Distinct), sắp xếp nếu cần
                var distinctSTTIndexes = group
                    .Select(r => r["STTIndex"].ToString().Trim())
                    .Distinct()
                    .ToList();

                int sttCode = 0;
                foreach (var sttIndex in distinctSTTIndexes)
                {
                    // Gán STTCode cho tất cả dòng có STTIndex tương ứng
                    foreach (var row in group.Where(r => r["STTIndex"].ToString().Trim() == sttIndex))
                    {
                        row["STTCode"] = sttCode;
                    }
                    sttCode++;
                }
            }
           
            //RefreshSTTColumn(_dtThongTinVT);
            tblThongSo = _dtThongTinVT.Copy();
            string urldv = string.Format("{0}?", URL + "KhoiTaoDM/GetDonVi");
            string jsondv = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldv); }).Result;
            if (jsondv != "[]")
            {
                DataTable tbldv = JsonConvert.DeserializeObject<DataTable>(jsondv);
                foreach (DataRow _drow in tblThongSo.Rows)
                {
                    string tenDV = _drow["TenDVVT"]?.ToString().Trim().ToUpper();

                    if (!string.IsNullOrEmpty(tenDV))
                    {
                        // tìm hàng tương ứng trong tbldv
                        var match = tbldv.AsEnumerable()
                                         .FirstOrDefault(r => r["TenDVVT"]?.ToString().Trim().ToUpper() == tenDV);

                        if (match != null)
                        {
                            _drow["QDLe"] = match["QDLamTronLe"];
                        }
                    }
                }
            }
            DataTable tblKT = new DataTable("tblKT");
            #region tạo cột 
            tblKT.Columns.Add("ID", typeof(long));
            tblKT.Columns.Add("MaDH", typeof(string));
            tblKT.Columns.Add("MaLenhSanXuat", typeof(string));
            tblKT.Columns.Add("MaNPL", typeof(string));
            tblKT.Columns.Add("MaVT", typeof(string));
            tblKT.Columns.Add("TenVT", typeof(string));
            tblKT.Columns.Add("MaMau", typeof(string));
            tblKT.Columns.Add("KhoVai", typeof(string));
            tblKT.Columns.Add("MaDV", typeof(string));
            tblKT.Columns.Add("DinhMuc", typeof(float));
            tblKT.Columns.Add("SoLuong", typeof(int));
            tblKT.Columns.Add("CapPhat", typeof(float));
            tblKT.Columns.Add("CapThem", typeof(float));
            tblKT.Columns.Add("ThuHoi", typeof(float));
            tblKT.Columns.Add("TrangThai", typeof(int));
            tblKT.Columns.Add("GhiChu", typeof(string));
            tblKT.Columns.Add("NguoiTao", typeof(string));
            tblKT.Columns.Add("NguoiSua", typeof(string));
            tblKT.Columns.Add("NgayTao", typeof(string));
            tblKT.Columns.Add("NgaySua", typeof(string));
            tblKT.Columns.Add("MaMauLenh", typeof(string));
            tblKT.Columns.Add("DauSizeLenh", typeof(string));
            tblKT.Columns.Add("SizeLenh", typeof(string));
            tblKT.Columns.Add("MaBom", typeof(string));
            tblKT.Columns.Add("IsXetDuyet", typeof(bool));
            tblKT.Columns.Add("NguoiXet", typeof(string));
            tblKT.Columns.Add("NguoiHuy", typeof(string));
            tblKT.Columns.Add("NgayXet", typeof(string));
            tblKT.Columns.Add("NgayHuy", typeof(string));
            tblKT.Columns.Add("MaVTMau", typeof(string));
            tblKT.Columns.Add("QtyES", typeof(float));
            tblKT.Columns.Add("CostES", typeof(float));
            tblKT.Columns.Add("NeedES", typeof(float));
            tblKT.Columns.Add("ToTalRec", typeof(float));
            tblKT.Columns.Add("ID_MaNPL", typeof(string));
            tblKT.Columns.Add("ThucNhan", typeof(float));
            tblKT.Columns.Add("TenTAVT", typeof(string));
            tblKT.Columns.Add("DinhMucHaoHut", typeof(float));
            tblKT.Columns.Add("MaNhomVT", typeof(string));
            tblKT.Columns.Add("MaVTTheoCayVai", typeof(string));
            tblKT.Columns.Add("TenLoaiVT", typeof(string));
            tblKT.Columns.Add("MaMauVT", typeof(string));
            tblKT.Columns.Add("MaKhoVT", typeof(string));
            tblKT.Columns.Add("MaDVVT", typeof(string));
            tblKT.Columns.Add("MaVTID", typeof(string));
            tblKT.Columns.Add("MaDot", typeof(string));
            tblKT.Columns.Add("ThucXuat", typeof(string));
            tblKT.Columns.Add("MauSP", typeof(string));
            tblKT.Columns.Add("DinhMucChung", typeof(decimal));
            tblKT.Columns.Add("CapPhatTK", typeof(decimal));
            tblKT.Columns.Add("NhuCau", typeof(decimal));
            tblKT.Columns.Add("MaVTGhep", typeof(string));
            tblKT.Columns.Add("TachMauSPDH", typeof(int));
            tblKT.Columns.Add("MaCode", typeof(string));
            #endregion
            foreach (DataRow row in tblThongSo.Rows)
            {

                DataRow dr = tblKT.NewRow();
                dr["ID"] = 0;
                dr["MaDH"] = _magop;
                dr["MaLenhSanXuat"] = _maLSX;
                dr["MaVT"] = row["MaVT"];
                dr["TenVT"] = row["ChiTiet"];
                dr["MaMau"] = row["MauVT"];
                dr["KhoVai"] = row["KhoVai"];
                dr["MaDV"] = row["TenDVVT"];
                dr["MaNhomVT"] = row["MaNhom"];
                dr["MaVTTheoCayVai"] = 0;
                dr["TenLoaiVT"] = row["TenNhom"];
                dr["MaMauVT"] = row["MaMauVT"];
                dr["MaKhoVT"] = row["KhoVaiID"];
                dr["MaDVVT"] = row["MaDVVT"];
                dr["MaVTID"] = row["MaVTID"];
                dr["MaDot"] = _madot.ToString();
                dr["MauSP"] = row["TenMau"];
                dr["MaVTGhep"] = row["MaVTGhep"];
                tblKT.Rows.Add(dr);
            }
            string urlKT = string.Format("{0}?makh={1}&&mahang={2}", URL + "KhoiTaoDM/GetKiemTra", _makh, _mahang);
            string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, tblKT); }).Result;
            DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
            if (_dtKT != null && _dtKT.Rows.Count > 0)
            {
                foreach (DataRow dr in tblThongSo.Rows)
                {
                    var tonKho = (from kt in _dtKT.AsEnumerable()
                                  where kt["MaVT"].ToString().Trim() == dr["MaVT"].ToString().Trim()
                                     && kt["ChiTiet"].ToString().Trim() == dr["ChiTiet"].ToString().Trim()
                                     && kt["MaMauVT"].ToString().Trim() == dr["MaMauVT"].ToString().Trim()
                                     && kt["MauVT"].ToString().Trim() == dr["MauVT"].ToString().Trim()
                                     && kt["KhoVai"].ToString().Trim() == dr["KhoVai"].ToString().Trim()
                                     && kt["MaNhom"].ToString().Trim() == dr["MaNhom"].ToString().Trim()
                                  select kt["TonKho"]
                 ).FirstOrDefault();
                    dr["TonKho"] = tonKho ?? 0;
                    //if(Convert.ToDecimal(tonKho) != 0)
                    //    dr["SLCL"] = Math.Round(Convert.ToDecimal(tonKho) - Convert.ToDecimal(dr["SLCap"]),2);
                    //else
                    //    dr["SLCL"] = Math.Round(0 - Convert.ToDecimal(dr["SLCap"]), 2);
                }
            }

            gCTTVT.DataSource = _dtThongTinVT;
            gCThongSo.DataSource = tblThongSo;


        }
        private void LoadDaTaDinhMuc()
        {
            if (_dtSize == null || _dtSize.Rows.Count == 0) return;
            if (_dtDM == null || _dtDM.Rows.Count == 0) return;
            var distinctSizes = _dtSize.AsEnumerable()
                .Select(row => new
                {
                    MaSize = row["MaSize"].ToString().Trim(),
                    TenSize = row["TenSize"].ToString().Trim()
                })
                .Distinct()
                .ToList();
            var sizeStrings = distinctSizes
                .Select(x => $"{x.TenSize}@{x.MaSize}")
                .ToList();
            foreach (string colName in sizeStrings)
            {
                if (!_dtThongTinDM.Columns.Contains(colName))
                {
                    _dtThongTinDM.Columns.Add(colName, typeof(decimal));
                }
            }

            var pivotColumns = new List<string>
            {
                "MaKH", "MaHang", "MaMau", "TenMau", "MaNhomSize", "NhomSize",
                "MaVTGhep", "MaNhom", "MaVTID", "MaVT", "ChiTiet","MauVTID", "MaMauVT","MauVT", "KhoVaiID", "KhoVai",
                "AllSize", "CheckAll", "DinhMucGY", "IsXetDuyet", "NgayXet",
                "NguoiXet", "DinhMucHaoHut", "TachMau", "MaCode","STTIndex"
            };

            var grouped = _dtDM.AsEnumerable()
                .GroupBy(row => new
                {
                    MaKH = row["MaKH"].ToString(),
                    MaHang = row["MaHang"].ToString(),
                    MaMau = row["MaMau"].ToString(),
                    TenMau = row["TenMau"].ToString(),
                    MaNhomSize = row["MaNhomSize"].ToString(),
                    NhomSize = row["NhomSize"].ToString(),
                    MaVTGhep = row["MaVTGhep"].ToString(),
                    MaNhom = row["MaNhom"].ToString(),
                    MaVTID = row["MaVTID"].ToString(),
                    MauVTID = row["MauVTID"].ToString(),
                    KhoVaiID = row["KhoVaiID"].ToString(),
                    KhoVai = row["KhoVai"].ToString(),
                    MauVT = row["MauVT"].ToString(),
                    MaMauVT = row["MaMauVT"].ToString(),
                    MaVT = row["MaVT"].ToString(),
                    ChiTiet = row["ChiTiet"].ToString(),
                    STTIndex = row["STTIndex"].ToString()
                });

            foreach (var group in grouped)
            {
                DataRow newRow = _dtThongTinDM.NewRow();
                newRow["MaKH"] = group.Key.MaKH;
                newRow["MaHang"] = group.Key.MaHang;
                newRow["MaMau"] = group.Key.MaMau;
                newRow["TenMau"] = group.Key.TenMau;
                newRow["MaNhomSize"] = group.Key.MaNhomSize;
                newRow["NhomSize"] = group.Key.NhomSize;
                newRow["MaVTGhep"] = group.Key.MaVTGhep;
                newRow["MaNhom"] = group.Key.MaNhom;
                newRow["MaVTID"] = group.Key.MaVTID;
                newRow["MauVTID"] = group.Key.MauVTID;
                newRow["KhoVaiID"] = group.Key.KhoVaiID;
                newRow["KhoVai"] = group.Key.KhoVai;
                newRow["MauVT"] = group.Key.MauVT;
                newRow["MaMauVT"] = group.Key.MaMauVT;
                newRow["MaVT"] = group.Key.MaVT;
                newRow["ChiTiet"] = group.Key.ChiTiet;
                newRow["STTIndex"] = group.Key.STTIndex;
                newRow["STTCode"] = 0;
                foreach (var row in group)
                {
                    string colName = row["TenSize"].ToString().Trim() + "@" + row["MaSize"].ToString().Trim();
                    double dinhMuc;
                    if (double.TryParse(row["DinhMuc"].ToString(), out dinhMuc))
                        newRow[colName] = dinhMuc;
                }

                _dtThongTinDM.Rows.Add(newRow);
            }
            var groupeds = _dtThongTinDM.AsEnumerable()
        .Where(row => !(row.IsNull("MaVT") || row.IsNull("ChiTiet") || row.IsNull("MaNhom") ||
                        row.IsNull("MaMauVT") || row.IsNull("MauVT") || row.IsNull("KhoVai") || row.IsNull("STTIndex")))
        .GroupBy(row => string.Join("|", new string[]
        {
        row["MaVT"].ToString().Trim(),
        row["ChiTiet"].ToString().Trim(),
        row["MaNhom"].ToString().Trim(),
        row["MaMauVT"].ToString().Trim(),
        row["MauVT"].ToString().Trim(),
        row["KhoVai"].ToString().Trim()
        }));

            foreach (var group in groupeds)
            {
                // Lấy danh sách STTIndex duy nhất (Distinct), sắp xếp nếu cần
                var distinctSTTIndexes = group
                    .Select(r => r["STTIndex"].ToString().Trim())
                    .Distinct()
                    .ToList();

                int sttCode = 0;
                foreach (var sttIndex in distinctSTTIndexes)
                {
                    // Gán STTCode cho tất cả dòng có STTIndex tương ứng
                    foreach (var row in group.Where(r => r["STTIndex"].ToString().Trim() == sttIndex))
                    {
                        row["STTCode"] = sttCode;
                    }
                    sttCode++;
                }
            }
            DataRow _dr = gridView1.GetFocusedDataRow();
            createTable(_dtThongTinDM);
            DataTable tblSizeFiltered = _dtThongTinDM.Clone();
            var filteredRows = _dtThongTinDM.AsEnumerable()
            .Where(row => row["MaVT"].ToString() == _dr["MaVT"].ToString() &&
                          row["ChiTiet"].ToString() == _dr["ChiTiet"].ToString() &&
                          row["MaMauVT"].ToString() == _dr["MaMauVT"].ToString() &&
                          row["MauVT"].ToString() == _dr["MauVT"].ToString() &&
                          row["KhoVai"].ToString() == _dr["KhoVai"].ToString() &&
                          row["MaNhom"].ToString() == _dr["MaNhom"].ToString() &&
                          row["STTIndex"].ToString() == _dr["STTIndex"].ToString());
            foreach (var row in filteredRows)
            {
                tblSizeFiltered.ImportRow(row);
            }

            gCTTDM.DataSource = tblSizeFiltered;
            tinhDinhMucGoiY();
            tinhDinhMucChung();
            tinhDinhMucGoiYAll();
            tinhDinhMucChungALL();

        }
        private void LoadNhomSize()
        {
            var distinctSize = _dtDM.AsEnumerable()
               .Select(row => new
               {
                   MaNhomSize = row["MaNhomSize"].ToString().Trim(),
                   NhomSize = row["NhomSize"].ToString().Trim()
               })
               .Distinct()
               .ToList();
            DataTable tblNhomSize = new DataTable();
            tblNhomSize.Columns.Add("MaNhomSize", typeof(string));
            tblNhomSize.Columns.Add("NhomSize", typeof(string));
            foreach (var size in distinctSize)
            {
                DataRow newRow = tblNhomSize.NewRow();
                newRow["MaNhomSize"] = size.MaNhomSize;
                newRow["NhomSize"] = size.NhomSize;
                tblNhomSize.Rows.Add(newRow);
            }
            searchLookUpEditInseam.Properties.ValueMember = "MaNhomSize";
            searchLookUpEditInseam.Properties.DisplayMember = "NhomSize";
            searchLookUpEditInseam.Properties.DataSource = tblNhomSize;
        }
        private void CreateDotSTT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSTTDOT&para1={_makh}&para2={_mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                textBox1.Text = "";

                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;
            textBox1.Text = searchLookUpEditKH.Text.ToString() + '-' + searchLookUpEditMH.Text.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
            searchLookUpEditDot.Text = searchLookUpEditKH.Text.ToString() + '-' + searchLookUpEditMH.Text.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
            _madot = _makh.ToString() + '_' + _mahang.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
        }
        private void CreateSearchLookUpNhom()
        {
            try
            {

                string url = string.Format("{0}?", URL + "ERPVatTuBOM/GetNhom");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                _tblNhom = JsonConvert.DeserializeObject<DataTable>(json);

            }
            catch (Exception ex)
            {


            }

        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow _dr = gridView1.GetFocusedDataRow();
            createTable(_dtThongTinDM);
            DataTable tblSizeFiltered = _dtThongTinDM.Clone();
            var filteredRows = _dtThongTinDM.AsEnumerable()
             .Where(row => row["MaVT"].ToString() == _dr["MaVT"].ToString() &&
                           row["ChiTiet"].ToString() == _dr["ChiTiet"].ToString() &&
                           row["MaMauVT"].ToString() == _dr["MaMauVT"].ToString() &&
                           row["MauVT"].ToString() == _dr["MauVT"].ToString() &&
                           row["KhoVai"].ToString() == _dr["KhoVai"].ToString() &&
                           row["MaNhom"].ToString() == _dr["MaNhom"].ToString() &&
                           row["STTIndex"].ToString() == _dr["STTIndex"].ToString());
            foreach (var row in filteredRows)
            {
                tblSizeFiltered.ImportRow(row);
            }
            gCTTDM.DataSource = tblSizeFiltered;
            loadSearchLookUpFocues(_dr);



        }
        private void loadSearchLookUpFocues(DataRow _dr)
        {
            string[] mamauArr = _dr["MaMau"].ToString().Replace(" ", "").Split('|');
            string[] tenmauArr = _dr["TenMau"].ToString().Replace(" ", "").Split('|');
            DataTable tblMau = new DataTable();
            tblMau.Columns.Add("MaMau", typeof(string));
            tblMau.Columns.Add("TenMau", typeof(string));
            for (int i = 0; i < mamauArr.Length; i++)
            {
                tblMau.Rows.Add(mamauArr[i], tenmauArr[i]);
            }
            searchLookUpEditMauSP.Properties.ValueMember = "MaMau";
            searchLookUpEditMauSP.Properties.DisplayMember = "TenMau";
            searchLookUpEditMauSP.Properties.DataSource = tblMau;
            gridView6.ClearSelection();
            searchLookUpEdit3View.ClearSelection();
            gridView51.ClearSelection();
            searchLookUpEditMauSP.EditValue = null;
            searchLookUpEditInseam.EditValue = null;
            searchLookUpEditSize.EditValue = null;
            txtDinhMuc.Text = "";
        }
        private void createTable(DataTable tab)
        {
            bandedGridView1.OptionsView.AllowCellMerge = true;
            GridBand parentBand = bandedGridView1.Bands["gridBandSizeDM"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView1.Columns.Count;)
                {
                    if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView1.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }
            int demColIndex = -1;

            foreach (DataColumn column in tab.Columns)
            {
                demColIndex++;
                if (demColIndex > 29 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    //col.OptionsColumn.AllowEdit = false;
                    col.Visible = true;
                    col.Width = 85;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 85;
                    gridBandSizeDM.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }



        private void LoadThongSoVatTu()
        {
            gCNL.BeginUpdate();
            gCPL.BeginUpdate();
            try
            {

                CreateSearchLookUpNhom();

                if (tblCNL == null || tblCNL.Rows.Count == 0)
                {
                    tblCNL = CreateTable();
                }
                if (tblCPL == null || tblCPL.Rows.Count == 0)
                {
                    tblCPL = CreateTable();
                }

                if (_dtTS == null || _dtTS.Rows.Count == 0) return;


                foreach (DataRow dr in _dtTS.Rows)
                {
                    if (dr["NPL"].ToString() == "True")
                    {
                        tblCNL.ImportRow(dr);
                    }
                    else
                    {
                        tblCPL.ImportRow(dr);
                    }
                }



                gCNL.DataSource = tblCNL;
                gCPL.DataSource = tblCPL;




                gCNL.RefreshDataSource();
                gCPL.RefreshDataSource();



                RefreshSTTColumn(tblCNL);
                RefreshSTTColumn(tblCPL);
            }

            finally
            {

                gCNL.EndUpdate();
                gCPL.EndUpdate();
            }
        }



        private void RefreshSTTColumn(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            var sortedRows = dt.AsEnumerable()
             .OrderBy(r =>
             {
                 var value = r["Sort"];
                 long result;
                 return (value != DBNull.Value && long.TryParse(value.ToString(), out result)) ? result : long.MinValue;
             })
             .ThenBy(r =>
             {
                 var value = r["ID"];
                 long result;
                 return ((value != DBNull.Value && long.TryParse(value.ToString(), out result)) ? result : long.MinValue) == 0 ? 1 : 0;
             })
             .ThenBy(r =>
             {
                 var value = r["ID"];
                 long result;
                 return (value != DBNull.Value && long.TryParse(value.ToString(), out result)) ? result : long.MinValue;
             })
             .ToArray();
            for (int i = 0; i < sortedRows.Length; i++)
                sortedRows[i]["STT"] = i + 1;

            // Tạo bảng mới với cấu trúc giống
            var newTable = dt.Clone();

            foreach (var row in sortedRows)
            {
                // row vẫn là DataRow của bảng gốc, nhưng detached khỏi dt sau khi clear
                var newRow = newTable.NewRow();
                newRow.ItemArray = row.ItemArray.Clone() as object[];
                newTable.Rows.Add(newRow);
            }

            // Clear dt cũ và import lại
            dt.Clear();
            foreach (DataRow r in newTable.Rows)
            {
                dt.ImportRow(r);
            }
        }



        private DataTable CreateTable()
        {
            try
            {
                DataTable dtbcreate = new DataTable("dtbcreate");
                dtbcreate.Columns.Add("ID", typeof(int));
                dtbcreate.Columns.Add("MaHang", typeof(string));
                dtbcreate.Columns.Add("Makh", typeof(string));
                dtbcreate.Columns.Add("MaNhom", typeof(string));
                dtbcreate.Columns.Add("MaVTID", typeof(string));
                dtbcreate.Columns.Add("MaVT", typeof(string));
                dtbcreate.Columns.Add("ChiTiet", typeof(string));
                dtbcreate.Columns.Add("MaDVVT", typeof(string));
                dtbcreate.Columns.Add("TenDVVT", typeof(string));
                dtbcreate.Columns.Add("MauVTID", typeof(string));
                dtbcreate.Columns.Add("MaMauVT", typeof(string));
                dtbcreate.Columns.Add("MauVT", typeof(string));
                dtbcreate.Columns.Add("KhoVaiID", typeof(string));
                dtbcreate.Columns.Add("KhoVai", typeof(string));
                dtbcreate.Columns.Add("MaMau", typeof(string));
                dtbcreate.Columns.Add("NPL", typeof(bool));
                dtbcreate.Columns.Add("TenMau", typeof(string));
                dtbcreate.Columns.Add("TenNhom", typeof(string));
                dtbcreate.Columns.Add("Sort", typeof(int));
                dtbcreate.Columns.Add("GhiChu", typeof(string));
                dtbcreate.Columns.Add("IsNew", typeof(int));
                dtbcreate.Columns.Add("STT", typeof(int));
                dtbcreate.Columns.Add("MaVTGhep", typeof(string));
                dtbcreate.Columns.Add("STTIndex", typeof(int));
                dtbcreate.Columns.Add("Trung", typeof(string));
                dtbcreate.Columns.Add("TenDVKV", typeof(string));
                dtbcreate.Columns.Add("KhoVaiHienThi", typeof(string));
                dtbcreate.Columns.Add("MaNhomChiTiet", typeof(string));
                return dtbcreate;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private void gVNL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == colTenNhomNL)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }
        private void gVPL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == colTenNhomPL)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }
        private async void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
            SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
            SplashScreenManager.Default.SetWaitFormDescription("0%"); try
            {
                // Thực thi luuVatTu
                bool vatTuSuccess = false;

                try { vatTuSuccess = await luuVatTuAsync(); } catch { vatTuSuccess = true; };
                if (!vatTuSuccess)
                {
                    // Nếu thất bại, đóng form và thoát. Thông báo lỗi đã được hiển thị bên trong.
                    SplashScreenManager.CloseForm(false);
                    return;
                }
                if (!_isDinhMuc)
                {
                    SplashScreenManager.Default.SetWaitFormDescription("100%");
                    SplashScreenManager.CloseForm(false);
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    this.DialogResult = DialogResult.OK;
                }
                // Nếu thành công, tiếp tục thực thi luuDinhMuc
                bool dinhMucSuccess = await luuDinhMucAsync();
                if (!dinhMucSuccess)
                {
                    // Nếu thất bại, đóng form và thoát. Thông báo lỗi đã được hiển thị bên trong.
                    SplashScreenManager.CloseForm(false);
                    return;
                }
                // Hoàn tất 100%
                SplashScreenManager.Default.SetWaitFormDescription("100%");
                SplashScreenManager.CloseForm(false);
                clsWaitForm.ShowSuccessForm(this, 3000);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {// Xử lý lỗi và đóng form
                SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại!!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Đã xảy ra lỗi trong quá trình lưu: " + ex.Message);
            }
        }
        private async Task<bool> luuVatTuAsync()
        {
            // Các phần khởi tạo của bạn giữ nguyên
            try
            {
                this.ActiveControl = button1;
                DataTable tblmvt = CreateTableSaveMauVT();
                string khachhang = _makh;
                string mahang = _mahang;
                DataTable tblthongsovt = CreateTableSaveThongSo();
                DataTable table1 = gCNL.DataSource as DataTable;
                DataTable table2 = gCPL.DataSource as DataTable;
                DataTable tblgc = table1.Clone();

                if (table1 != null) { foreach (DataRow row in table1.Rows) tblgc.ImportRow(row); }
                if (table2 != null) { foreach (DataRow row in table2.Rows) tblgc.ImportRow(row); }

                if (tblgc == null || tblgc.Rows.Count == 0) return true;


                int totalStepsInMethod = (tblgc.Rows.Count * 2) + 3; // Kiểm tra + Chuẩn bị + 3 API calls
                int currentStep = 0;

                // Hàm nội bộ để tính và cập nhật %
                Action<int> reportProgress = (step) =>
                {
                    // Ánh xạ tiến trình của hàm này (0 -> totalStepsInMethod) vào thang đo chung (0 -> 40)
                    int percentage = (step * 40) / totalStepsInMethod;
                    SplashScreenManager.Default.SetWaitFormDescription($"{percentage}%");
                };

                foreach (DataRow item in tblgc.Rows)
                {



                    string _textNPL = item["NPL"].ToString().ToLower() != "false" ? "Nguyên liệu" : "Phụ liệu";
                    //if (item["IsNew"].ToString() == "1")
                    //{
                    //    XtraMessageBox.Show("Dữ liệu bị trùng với vật tư có sẵn trong hệ thống ở" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return false;
                    //}
                    if (item["IsNew"].ToString() == "2")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Dữ liệu bị trùng ở" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["MaVT"].ToString() == "")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Vui lòng nhập đầy đủ ItemCode ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["ChiTiet"].ToString() == "")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Vui lòng nhập đầy đủ Vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["NPL"].ToString().ToLower() != "false" && item["MaMauVT"].ToString() == "")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Vui lòng nhập thông tin mã màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["NPL"].ToString().ToLower() != "false" && item["MauVT"].ToString() == "")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Vui lòng nhập thông tin Màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["NPL"].ToString().ToLower() != "false" && item["MaMauVT"].ToString() == "")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Vui lòng nhập thông tin Mã màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["TenDVVT"].ToString() == "")
                    {
                        SplashScreenManager.CloseForm(false);
                        XtraMessageBox.Show("Vui lòng nhập đầy đủ đơn vị ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    currentStep++;
                    reportProgress(currentStep);
                }

                if (checkTrungVT(tblgc)) return false;
                currentStep++;
                reportProgress(currentStep);


                foreach (DataRow row in tblgc.Rows)
                {
                    DataRow rowthongso = tblthongsovt.NewRow();
                    rowthongso["ID"] = row["ID"];
                    rowthongso["MaHang"] = mahang;
                    rowthongso["MaKH"] = khachhang;
                    rowthongso["MaNhom"] = row["MaNhom"].ToString();
                    rowthongso["MaVTID"] = row["MaVTID"].ToString();
                    rowthongso["MaVT"] = row["MaVT"].ToString().Trim();
                    rowthongso["ChiTiet"] = row["ChiTiet"].ToString().Trim();
                    rowthongso["MaDVVT"] = row["MaDVVT"].ToString();
                    rowthongso["TenDVVT"] = row["TenDVVT"].ToString().Trim();
                    rowthongso["MauVTID"] = row["MauVTID"].ToString();
                    rowthongso["MaMauVT"] = row["MaMauVT"].ToString().Trim();
                    rowthongso["KhoVaiID"] = row["KhoVaiID"].ToString();
                    rowthongso["KhoVai"] = row["KhoVai"].ToString().Trim();
                    rowthongso["MauVT"] = row["MauVT"].ToString().Trim();
                    rowthongso["GhiChu"] = row["GhiChu"].ToString();
                    rowthongso["MaVTGhep"] = row["MaVTGhep"].ToString();
                    rowthongso["IsNew"] = row["IsNew"];
                    rowthongso["TenDVKV"] = row["TenDVKV"];
                    rowthongso["MaNhomChiTiet"] = row["MaNhomChiTiet"];
                    tblthongsovt.Rows.Add(rowthongso);
                    currentStep++;
                    reportProgress(currentStep);
                }
                //chuẩn bị dữ liệu để Post
                //foreach (DataRow resultRow in tblthongsovt.Rows)
                //{

                //    DataRow rowmvt = tblmvt.NewRow();
                //    rowmvt["ID"] = 0;
                //    rowmvt["MaHang"] = mahang;
                //    rowmvt["MaKH"] = khachhang;
                //    rowmvt["MaNhom"] = resultRow["MaNhom"];
                //    rowmvt["MaVT"] = resultRow["MaVT"].ToString().Trim();
                //    rowmvt["ChiTiet"] = resultRow["ChiTiet"].ToString().Trim();
                //    rowmvt["MaDVVT"] = resultRow["MaDVVT"];
                //    rowmvt["KhoVaiID"] = resultRow["KhoVaiID"];
                //    rowmvt["MaVTID"] = resultRow["MaVTID"];
                //    rowmvt["MauVTID"] = resultRow["MauVTID"];
                //    rowmvt["MaMauVT"] = resultRow["MaMauVT"];
                //    rowmvt["MauVT"] = resultRow["MauVT"];
                //    rowmvt["MaVTGhep"] = resultRow["MaVTGhep"].ToString();
                //    tblmvt.Rows.Add(rowmvt);
                //    currentStep++;
                //    reportProgress(currentStep);

                //}
                string url1 = $"{URL}ERPVatTuBOM/PostT4?Action=POSTVATTUTVEXCEL&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult1 = await _clientExtension.PostAsync(url1, tblthongsovt);
                if (msResult1.ToLower() != "true") {
                   // XtraMessageBox.Show("Lỗi khi lưu " + msResult1, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false; }
                currentStep++; reportProgress(currentStep);

                //string url2 = $"{URL}ERPVatTuBOM/PostT2?Action=POSTMAUVTTVEXCEL&para={GlobleData.UserName}&para2=frmERP_VatTu";
                //string msResult2 = await _clientExtension.PostAsync(url2, tblmvt);
                //if (msResult2.ToLower() != "true") return false;
                currentStep++; reportProgress(currentStep);

                string url4 = $"{URL}ERPVatTuBOM/PostT4?Action=POSTTHONGSOEXCEL&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult4 = await _clientExtension.PostAsync(url4, tblthongsovt);
                if (msResult4.ToLower() != "true") {
                    //XtraMessageBox.Show("Lỗi khi lưu " + msResult4, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return  false; }

                // Đảm bảo kết thúc hàm này ở 40%
                SplashScreenManager.Default.SetWaitFormDescription("100%");
                return true;
            }
            catch (Exception ex)
            {
                //XtraMessageBox.Show("Lỗi khi lưu "+ ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
 
            }
           
        }
        private bool luuVatTu()
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblmvt = CreateTableSaveMauVT();
                string khachhang = _makh;
                string mahang = _mahang;
                DataTable tblthongsovt = CreateTableSaveThongSo();
                DataTable table1 = gCNL.DataSource as DataTable;
                DataTable table2 = gCPL.DataSource as DataTable;
                DataTable tblgc = table1.Clone();

                if (table1 != null && table1.Rows.Count != 0)
                {
                    foreach (DataRow row in table1.Rows)
                    {
                        tblgc.ImportRow(row);
                    }
                }

                if (table2 != null && table2.Rows.Count != 0)
                {
                    foreach (DataRow row in table2.Rows)
                    {
                        tblgc.ImportRow(row);
                    }
                }

                if (tblgc == null || tblgc.Rows.Count == 0) return true;
                foreach (DataRow item in tblgc.Rows)
                {



                    string _textNPL = item["NPL"].ToString().ToLower() != "false" ? "Nguyên liệu" : "Phụ liệu";
                    //if (item["IsNew"].ToString() == "1")
                    //{
                    //    XtraMessageBox.Show("Dữ liệu bị trùng với vật tư có sẵn trong hệ thống ở" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return false;
                    //}
                    if (item["IsNew"].ToString() == "2")
                    {
                        XtraMessageBox.Show("Dữ liệu bị trùng ở" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["MaVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng nhập đầy đủ ItemCode ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["ChiTiet"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng nhập đầy đủ Vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["NPL"].ToString().ToLower() != "false" && item["MaMauVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng nhập thông tin mã màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["NPL"].ToString().ToLower() != "false" && item["MauVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng nhập thông tin Màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["NPL"].ToString().ToLower() != "false" && item["MaMauVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng nhập thông tin Mã màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (item["TenDVVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng nhập đầy đủ đơn vị ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                if (checkTrungVT(tblgc))
                {
                    return false;
                }

                foreach (DataRow row in tblgc.Rows)
                {
                    DataRow rowthongso = tblthongsovt.NewRow();
                    rowthongso["ID"] = row["ID"];
                    rowthongso["MaHang"] = mahang;
                    rowthongso["MaKH"] = khachhang;
                    rowthongso["MaNhom"] = row["MaNhom"].ToString();
                    rowthongso["MaVTID"] = row["MaVTID"].ToString();
                    rowthongso["MaVT"] = row["MaVT"].ToString().Trim();
                    rowthongso["ChiTiet"] = row["ChiTiet"].ToString().Trim();
                    rowthongso["MaDVVT"] = row["MaDVVT"].ToString();
                    rowthongso["TenDVVT"] = row["TenDVVT"].ToString().Trim();
                    rowthongso["MauVTID"] = row["MauVTID"].ToString();
                    rowthongso["MaMauVT"] = row["MaMauVT"].ToString().Trim();
                    rowthongso["KhoVaiID"] = row["KhoVaiID"].ToString();
                    rowthongso["KhoVai"] = row["KhoVai"].ToString().Trim();
                    rowthongso["MauVT"] = row["MauVT"].ToString().Trim();
                    rowthongso["GhiChu"] = row["GhiChu"].ToString();
                    rowthongso["MaVTGhep"] = row["MaVTGhep"].ToString();
                    rowthongso["IsNew"] = row["IsNew"];
                    tblthongsovt.Rows.Add(rowthongso);

                }
                //chuẩn bị dữ liệu để Post
                foreach (DataRow resultRow in tblthongsovt.Rows)
                {

                    DataRow rowmvt = tblmvt.NewRow();
                    rowmvt["ID"] = 0;
                    rowmvt["MaHang"] = mahang;
                    rowmvt["MaKH"] = khachhang;
                    rowmvt["MaNhom"] = resultRow["MaNhom"];
                    rowmvt["MaVT"] = resultRow["MaVT"].ToString().Trim();
                    rowmvt["ChiTiet"] = resultRow["ChiTiet"].ToString().Trim();
                    rowmvt["MaDVVT"] = resultRow["MaDVVT"];
                    rowmvt["KhoVaiID"] = resultRow["KhoVaiID"];
                    rowmvt["MaVTID"] = resultRow["MaVTID"];
                    rowmvt["MauVTID"] = resultRow["MauVTID"];
                    rowmvt["MaMauVT"] = resultRow["MaMauVT"];
                    rowmvt["MauVT"] = resultRow["MauVT"];
                    rowmvt["MaVTGhep"] = resultRow["MaVTGhep"].ToString();
                    tblmvt.Rows.Add(rowmvt);

                }
                string url1 = $"{URL}ERPVatTuBOM/PostT4?Action=POSTVATTUTVEXCEL&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, tblthongsovt); }).Result;
                if (msResult1.ToLower() != "true") return false;


                //POSTMAUVTTV t2
                string url2 = $"{URL}ERPVatTuBOM/PostT2?Action=POSTMAUVTTVEXCEL&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tblmvt); }).Result;
                if (msResult2.ToLower() != "true") return false;


                string url4 = $"{URL}ERPVatTuBOM/PostT4?Action=POSTTHONGSOEXCEL&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult4 = Task.Run(async () => { return await _clientExtension.PostAsync(url4, tblthongsovt); }).Result;
                if (msResult4.ToLower() != "true") return false;



                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private DataTable CreateTableSaveThongSo()
        {
            try
            {
                DataTable dtbthongso = new DataTable("dtbthongso");
                dtbthongso.Columns.Add("ID", typeof(int));
                dtbthongso.Columns.Add("MaHang", typeof(string));
                dtbthongso.Columns.Add("MaKH", typeof(string));
                dtbthongso.Columns.Add("MaNhom", typeof(string));
                dtbthongso.Columns.Add("MaVTID", typeof(string));
                dtbthongso.Columns.Add("MaVT", typeof(string));
                dtbthongso.Columns.Add("ChiTiet", typeof(string));
                dtbthongso.Columns.Add("MaDVVT", typeof(string));
                dtbthongso.Columns.Add("TenDVVT", typeof(string));
                dtbthongso.Columns.Add("MauVTID", typeof(string));
                dtbthongso.Columns.Add("MaMauVT", typeof(string));
                dtbthongso.Columns.Add("KhoVaiID", typeof(string));
                dtbthongso.Columns.Add("KhoVai", typeof(string));
                dtbthongso.Columns.Add("MauVT", typeof(string));
                dtbthongso.Columns.Add("GhiChu", typeof(string));
                dtbthongso.Columns.Add("MaVTGhep", typeof(string));
                dtbthongso.Columns.Add("IsNew", typeof(int));
                dtbthongso.Columns.Add("TenDVKV", typeof(string));
                dtbthongso.Columns.Add("MaNhomChiTiet", typeof(string));
                return dtbthongso;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private DataTable CreateTableSaveMauVT()
        {
            try
            {
                DataTable tblmaucreate = new DataTable("tblmaucreate");
                tblmaucreate.Columns.Add("ID", typeof(int));
                tblmaucreate.Columns.Add("MaHang", typeof(string));
                tblmaucreate.Columns.Add("MaKH", typeof(string));
                tblmaucreate.Columns.Add("MaNhom", typeof(string));
                tblmaucreate.Columns.Add("MaVT", typeof(string));
                tblmaucreate.Columns.Add("ChiTiet", typeof(string));
                tblmaucreate.Columns.Add("MaDVVT", typeof(string));
                tblmaucreate.Columns.Add("KhoVaiID", typeof(string));
                tblmaucreate.Columns.Add("MaVTID", typeof(string));
                tblmaucreate.Columns.Add("MauVTID", typeof(string));
                tblmaucreate.Columns.Add("MaMauVT", typeof(string));
                tblmaucreate.Columns.Add("MauVT", typeof(string));
                tblmaucreate.Columns.Add("MaVTGhep", typeof(string));
                return tblmaucreate;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        private async Task<bool> luuDinhMucAsync()
        {
            // Các phần khởi tạo của bạn
            string madotpost = _madot;
            string tendotpost = textBox1.Text;
            this.ActiveControl = button1;

            DataTable tblGC1 = gCTTVT.DataSource as DataTable;
            if (tblGC1 == null || tblGC1.Rows.Count == 0) return false;

            var filteredRows = _dtThongTinDM.AsEnumerable().Where(row => !string.IsNullOrWhiteSpace(row["MaMau"]?.ToString()));
            DataTable tblGC2 = filteredRows.Any() ? filteredRows.CopyToDataTable() : _dtThongTinDM.Clone();
            if (tblGC2 == null || tblGC2.Rows.Count == 0) return false;

            // --- Tính toán tiến trình cho luuDinhMuc (phạm vi 40 -> 100%) ---
            int totalStepsInMethod = tblGC1.Rows.Count + tblGC2.Rows.Count + tblGC1.Rows.Count + 3; // 3 vòng lặp + 3 API calls
            int currentStep = 0;

            // Hàm nội bộ để tính và cập nhật %
            Action<int> reportProgress = (step) =>
            {
                // Ánh xạ tiến trình của hàm này (0 -> totalStepsInMethod) vào thang đo chung (40 -> 100)
                // 60 là khoảng % mà hàm này chiếm (100 - 40)
                int percentage = 40 + (step * 60) / totalStepsInMethod;
                SplashScreenManager.Default.SetWaitFormDescription($"{percentage}%");
            };

            // Chuẩn bị dữ liệu
            DataTable dtMauSP = createTypeMauSP();
            foreach (DataRow _dr in tblGC1.Rows)
            {

                string[] mamauArr = _dr["MaMau"].ToString().Split('|');
                foreach (string mamau in mamauArr)
                {
                    DataRow _nr = dtMauSP.NewRow();
                    _nr["ID"] = 0;

                    _nr["MaKH"] = _makh;
                    _nr["MaHang"] = _mahang;
                    _nr["MaNhom"] = _dr["MaNhom"].ToString();
                    _nr["MaVT"] = _dr["MaVT"];
                    _nr["ChiTiet"] = _dr["ChiTiet"];
                    _nr["MaMauVT"] = _dr["MaMauVT"];
                    _nr["MauVT"] = _dr["MauVT"];
                    _nr["KhoVai"] = _dr["KhoVai"];
                    _nr["MaVTID"] = _dr["MaVTID"].ToString();
                    _nr["MauVTID"] = _dr["MauVTID"].ToString();
                    _nr["KhoVaiID"] = _dr["KhoVaiID"].ToString();
                    _nr["MaMau"] = mamau.ToString().Trim();
                    _nr["MaDot"] = madotpost;
                    _nr["MaCode"] = _dr["MaCode"];
                    _nr["STTCode"] = _dr["STTCode"];
                    _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                    dtMauSP.Rows.Add(_nr);
                }
                currentStep++; reportProgress(currentStep);
            }

            SplashScreenManager.Default.SetWaitFormDescription("50%");
            DataTable dtSizeSP = createTypeSizeSP();
            foreach (DataRow _dr in tblGC2.Rows)
            {
                string[] mamauArr = _dr["MaMau"].ToString().Split('|');

                foreach (string mamau in mamauArr)
                {
                    for (int i = 31; i < tblGC2.Columns.Count; i++)
                    {
                        DataRow _nr = dtSizeSP.NewRow();
                        _nr["MaKH"] = _makh;
                        _nr["MaHang"] = _mahang;
                        _nr["MaNhom"] = _dr["MaNhom"];
                        _nr["MaVTID"] = _dr["MaVTID"];
                        _nr["MaVT"] = _dr["MaVT"];
                        _nr["ChiTiet"] = _dr["ChiTiet"];
                        _nr["MaMauVT"] = _dr["MaMauVT"];
                        _nr["MauVT"] = _dr["MauVT"];
                        _nr["KhoVai"] = _dr["KhoVai"];
                        _nr["MauVTID"] = _dr["MauVTID"];
                        _nr["KhoVaiID"] = _dr["KhoVaiID"];
                        _nr["MaNhomSize"] = _dr["MaNhomSize"];
                        _nr["MaSize"] = tblGC2.Columns[i].ColumnName.Split('@')[1].ToString();
                        _nr["DinhMuc"] = _dr[i].ToString() == "" ? 0 : _dr[i];
                        _nr["MaDot"] = madotpost;
                        _nr["Dot"] = tendotpost;
                        _nr["NguoiTao"] = GlobleData.UserName;
                        _nr["MaMau"] = mamau.ToString().Trim();
                        _nr["TachMau"] = true;// _dr["TachMau"];
                        _nr["MaCode"] = _dr["MaCode"];
                        _nr["STTCode"] = _dr["STTCode"];
                        _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                        dtSizeSP.Rows.Add(_nr);
                    }

                }
                currentStep++; reportProgress(currentStep);
            }
            SplashScreenManager.Default.SetWaitFormDescription("60%");
            DataTable dtVatTuSP = createTypeVattuSP();
            foreach (DataRow _dr in tblGC1.Rows)
            {
                if (_dr["MaMau"] == "") continue;
                DataRow _nr = dtVatTuSP.NewRow();
                _nr["ID"] = 0;
                _nr["MaKH"] = _makh;
                _nr["MaHang"] = _mahang;
                _nr["MaVT"] = _dr["MaVT"];
                _nr["ChiTiet"] = _dr["ChiTiet"];
                _nr["MaMauVT"] = _dr["MaMauVT"];
                _nr["MauVT"] = _dr["MauVT"];
                _nr["KhoVai"] = _dr["KhoVai"];
                _nr["MaNhom"] = _dr["MaNhom"];
                _nr["MaVTID"] = _dr["MaVTID"];
                _nr["MauVTID"] = _dr["MauVTID"];
                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                _nr["MaDot"] = madotpost;
                _nr["Dot"] = tendotpost;
                _nr["NguoiTao"] = GlobleData.UserName;
                _nr["DinhMucChung"] = _dr["DinhMucChung"];
                _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                _nr["TachMau"] = true;//_dr["TachMau"];
                _nr["STT"] = _stt;
                _nr["MaCode"] = _dr["MaCode"];
                _nr["STTCode"] = _dr["STTCode"];
                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
             
                dtVatTuSP.Rows.Add(_nr);
                currentStep++; reportProgress(currentStep);
            }
            DataTable tblGrid = gridControl3.DataSource as DataTable;
            //DataTable tblSave = new DataTable("tblSave");
            //if(tblGrid!=null&& tblGrid.Rows.Count!=0)
            //{
            //    #region tạo cột 
            //    tblSave.Columns.Add("ID", typeof(long));
            //    tblSave.Columns.Add("MaDH", typeof(string));
            //    tblSave.Columns.Add("MaLenhSanXuat", typeof(string));
            //    tblSave.Columns.Add("MaNPL", typeof(string));
            //    tblSave.Columns.Add("MaVT", typeof(string));
            //    tblSave.Columns.Add("TenVT", typeof(string));
            //    tblSave.Columns.Add("MaMau", typeof(string));
            //    tblSave.Columns.Add("KhoVai", typeof(string));
            //    tblSave.Columns.Add("MaDV", typeof(string));
            //    tblSave.Columns.Add("DinhMuc", typeof(float));
            //    tblSave.Columns.Add("SoLuong", typeof(int));
            //    tblSave.Columns.Add("CapPhat", typeof(float));
            //    tblSave.Columns.Add("CapThem", typeof(float));
            //    tblSave.Columns.Add("ThuHoi", typeof(float));
            //    tblSave.Columns.Add("TrangThai", typeof(int));
            //    tblSave.Columns.Add("GhiChu", typeof(string));
            //    tblSave.Columns.Add("NguoiTao", typeof(string));
            //    tblSave.Columns.Add("NguoiSua", typeof(string));
            //    tblSave.Columns.Add("NgayTao", typeof(string));
            //    tblSave.Columns.Add("NgaySua", typeof(string));
            //    tblSave.Columns.Add("MaMauLenh", typeof(string));
            //    tblSave.Columns.Add("DauSizeLenh", typeof(string));
            //    tblSave.Columns.Add("SizeLenh", typeof(string));
            //    tblSave.Columns.Add("MaBom", typeof(string));
            //    tblSave.Columns.Add("IsXetDuyet", typeof(bool));
            //    tblSave.Columns.Add("NguoiXet", typeof(string));
            //    tblSave.Columns.Add("NguoiHuy", typeof(string));
            //    tblSave.Columns.Add("NgayXet", typeof(string));
            //    tblSave.Columns.Add("NgayHuy", typeof(string));
            //    tblSave.Columns.Add("MaVTMau", typeof(string));
            //    tblSave.Columns.Add("QtyES", typeof(float));
            //    tblSave.Columns.Add("CostES", typeof(float));
            //    tblSave.Columns.Add("NeedES", typeof(float));
            //    tblSave.Columns.Add("ToTalRec", typeof(float));
            //    tblSave.Columns.Add("ID_MaNPL", typeof(string));
            //    tblSave.Columns.Add("ThucNhan", typeof(float));
            //    tblSave.Columns.Add("TenTAVT", typeof(string));
            //    tblSave.Columns.Add("DinhMucHaoHut", typeof(float));
            //    tblSave.Columns.Add("MaNhomVT", typeof(string));
            //    tblSave.Columns.Add("MaVTTheoCayVai", typeof(string));
            //    tblSave.Columns.Add("TenLoaiVT", typeof(string));
            //    tblSave.Columns.Add("MaMauVT", typeof(string));
            //    tblSave.Columns.Add("MaKhoVT", typeof(string));
            //    tblSave.Columns.Add("MaDVVT", typeof(string));
            //    tblSave.Columns.Add("MaVTID", typeof(string));
            //    tblSave.Columns.Add("MaDot", typeof(string));
            //    tblSave.Columns.Add("ThucXuat", typeof(string));
            //    tblSave.Columns.Add("MauSP", typeof(string));
            //    tblSave.Columns.Add("DinhMucChung", typeof(decimal));
            //    tblSave.Columns.Add("CapPhatTK", typeof(decimal));
            //    tblSave.Columns.Add("NhuCau", typeof(decimal));
            //    tblSave.Columns.Add("MaVTGhep", typeof(string));
            //    tblSave.Columns.Add("TachMauSPDH", typeof(int));
            //    tblSave.Columns.Add("MaCode", typeof(string));
            //    #endregion
            //    foreach (DataRow row in tblGrid.Rows)
            //    {
            //        DataRow dr = tblSave.NewRow();
            //        dr["ID"] = row["ID"];
            //        dr["MaDH"] = _magop;
            //        dr["MaLenhSanXuat"] = _maLSX;
            //        dr["MaNPL"] = row["MaNPL"].ToString() == "" ? "0" : row["MaNPL"].ToString();
            //        dr["MaVT"] = row["MaVT"];
            //        dr["TenVT"] = row["ChiTiet"];
            //        dr["MaMau"] = row["MauVT"];
            //        dr["KhoVai"] = row["KhoVai"];
            //        dr["MaDV"] = row["TenDVVT"];
            //        dr["DinhMuc"] = row["DinhMuc"];
            //        dr["SoLuong"] = row["SoLuong"];
            //        dr["CapPhat"] = row["CapPhat"];
            //        dr["CapThem"] = row["CapThem"];
            //        dr["ThuHoi"] = 0;
            //        dr["TrangThai"] = 0;
            //        dr["GhiChu"] = row["GhiChu"];
            //        dr["NguoiTao"] = GlobleData.UserName;
            //        dr["NguoiSua"] = 0;
            //        dr["NgayTao"] = "";
            //        dr["NgaySua"] = "";
            //        dr["MaMauLenh"] = 0;
            //        dr["DauSizeLenh"] = 0;
            //        dr["SizeLenh"] = 0;
            //        dr["MaBom"] = 0;
            //        dr["IsXetDuyet"] = false;
            //        dr["NguoiXet"] = 0;
            //        dr["NguoiHuy"] = 0;
            //        dr["NgayXet"] = "";
            //        dr["NgayHuy"] = "";
            //        dr["MaVTMau"] = 0;
            //        dr["QtyES"] = 0;
            //        dr["CostES"] = 0;
            //        dr["NeedES"] = 0;
            //        dr["ToTalRec"] = 0;
            //        dr["ID_MaNPL"] = 0;
            //        dr["ThucNhan"] = 0;
            //        dr["TenTAVT"] = 0;
            //        dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
            //        dr["MaNhomVT"] = row["MaNhom"];
            //        dr["MaVTTheoCayVai"] = 0;
            //        dr["TenLoaiVT"] = row["TenNhom"];
            //        dr["MaMauVT"] = row["MaMauVT"];
            //        dr["MaKhoVT"] = row["KhoVaiID"];
            //        dr["MaDVVT"] = row["MaDVVT"];
            //        dr["MaVTID"] = row["MaVTID"];
            //        dr["MaDot"] = _madot.ToString();
            //        dr["ThucXuat"] = row["ThucXuat"];
            //        dr["MauSP"] = row["TenMau"];
            //        dr["DinhMucChung"] = row["DinhMucChung"];
            //        dr["CapPhatTK"] = row["CapPhatTK"];
            //        dr["NhuCau"] = row["NhuCau"];
            //        dr["MaVTGhep"] = row["MaVTGhep"];
            //        dr["TachMauSPDH"] = row["TachMauSPDH"];
            //        dr["MaCode"] = row["MaCode"];
            //        tblSave.Rows.Add(dr);
            //    }
            //}
            //var rowsWithIssue = tblSave.AsEnumerable()
            //                         .Where(row => row.IsNull("DinhMuc") || Convert.ToDecimal(row["DinhMuc"]) == 0)
            //                         .ToList();

            //if (rowsWithIssue.Any())
            //{
            //    MessageBox.Show("Định mức Khách hàng không bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return false;
            //}

            SplashScreenManager.Default.SetWaitFormDescription("70%");
            // Các lệnh gọi API
            string url1 = $"{URL}KhoiTaoBOMV1/Post4?Action=POSTMAUSPEXCEL&para1={GlobleData.UserName}";
            string result1 = await _clientExtension.PostAsync(url1, dtMauSP);
            if (result1 != "True") return false;
            currentStep++; reportProgress(currentStep);
            SplashScreenManager.Default.SetWaitFormDescription("80%");
            string url2 = $"{URL}KhoiTaoBOMV1/Post5?Action=POSTDMSIZEEXCEL&para1={GlobleData.UserName}";
            string result2 = await _clientExtension.PostAsync(url2, dtSizeSP);
            if (result2 != "True") return false;
            currentStep++; reportProgress(currentStep);
            SplashScreenManager.Default.SetWaitFormDescription("95%");
            string url3 = $"{URL}KhoiTaoBOMV1/Post6?Action=POSTVTSPEXCEL&para1={GlobleData.UserName}";
            string result3 = await _clientExtension.PostAsync(url3, dtVatTuSP);
            if (result3 != "True") return false;
          
            //string url = $"{URL}KhoiTaoDM/PostNew?para={_madot}&para1={GlobleData.UserName}";
            //if (tblSave.Rows.Count == 0) return false;
            //string result4 = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            //if (result4 != "True") return false;
            return true;
        }
        private void luuDinhMuc()
        {
            try
            {

                string madotpost = _madot;
                string tendotpost = textBox1.Text;


                this.ActiveControl = button1;

                DataTable tblGC1 = gCTTVT.DataSource as DataTable;
                if (tblGC1 == null || tblGC1.Rows.Count == 0) return;

                if (_dtThongTinDM == null || _dtThongTinDM.Rows.Count == 0) return;
                var filteredRows = _dtThongTinDM.AsEnumerable().Where(row => !string.IsNullOrWhiteSpace(row["MaMau"]?.ToString()));

                DataTable tblGC2 = filteredRows.Any() ? filteredRows.CopyToDataTable() : _dtThongTinDM.Clone();
                if (tblGC2 == null || tblGC2.Rows.Count == 0) return;


                DataTable dtMauSP = createTypeMauSP();
                foreach (DataRow _dr in tblGC1.Rows)
                {

                    string[] mamauArr = _dr["MaMau"].ToString().Split('|');
                    foreach (string mamau in mamauArr)
                    {
                        DataRow _nr = dtMauSP.NewRow();
                        _nr["ID"] = 0;

                        _nr["MaKH"] = _makh;
                        _nr["MaHang"] = _mahang;
                        _nr["MaNhom"] = _dr["MaNhom"].ToString();
                        _nr["MaVT"] = _dr["MaVT"];
                        _nr["ChiTiet"] = _dr["ChiTiet"];
                        _nr["MaMauVT"] = _dr["MaMauVT"];
                        _nr["MauVT"] = _dr["MauVT"];
                        _nr["KhoVai"] = _dr["KhoVai"];
                        _nr["MaVTID"] = _dr["MaVTID"].ToString();
                        _nr["MauVTID"] = _dr["MauVTID"].ToString();
                        _nr["KhoVaiID"] = _dr["KhoVaiID"].ToString();
                        _nr["MaMau"] = mamau.ToString().Trim();
                        _nr["MaDot"] = madotpost;
                        _nr["MaCode"] = _dr["MaCode"];
                        _nr["STTCode"] = _dr["STTCode"];
                        dtMauSP.Rows.Add(_nr);
                    }
                }

                DataTable dtSizeSP = createTypeSizeSP();
                foreach (DataRow _dr in tblGC2.Rows)
                {
                    string[] mamauArr = _dr["MaMau"].ToString().Split('|');

                    foreach (string mamau in mamauArr)
                    {
                        for (int i = 27; i < tblGC2.Columns.Count; i++)
                        {
                            DataRow _nr = dtSizeSP.NewRow();
                            _nr["MaKH"] = _makh;
                            _nr["MaHang"] = _mahang;
                            _nr["MaNhom"] = _dr["MaNhom"];
                            _nr["MaVTID"] = _dr["MaVTID"];
                            _nr["MaVT"] = _dr["MaVT"];
                            _nr["ChiTiet"] = _dr["ChiTiet"];
                            _nr["MaMauVT"] = _dr["MaMauVT"];
                            _nr["MauVT"] = _dr["MauVT"];
                            _nr["KhoVai"] = _dr["KhoVai"];
                            _nr["MauVTID"] = _dr["MauVTID"];
                            _nr["KhoVaiID"] = _dr["KhoVaiID"];
                            _nr["MaNhomSize"] = _dr["MaNhomSize"];
                            _nr["MaSize"] = tblGC2.Columns[i].ColumnName.Split('@')[1].ToString();
                            _nr["DinhMuc"] = _dr[i].ToString() == "" ? 0 : _dr[i];
                            _nr["MaDot"] = madotpost;
                            _nr["Dot"] = tendotpost;
                            _nr["NguoiTao"] = GlobleData.UserName;
                            _nr["MaMau"] = mamau.ToString().Trim();
                            _nr["TachMau"] = true;// _dr["TachMau"];
                            _nr["MaCode"] = _dr["MaCode"];
                            _nr["STTCode"] = _dr["STTCode"];
                            dtSizeSP.Rows.Add(_nr);
                        }

                    }

                }

                DataTable dtVatTuSP = createTypeVattuSP();
                foreach (DataRow _dr in tblGC1.Rows)
                {
                    if (_dr["MaMau"] == "") continue;
                    DataRow _nr = dtVatTuSP.NewRow();
                    _nr["ID"] = 0;
                    _nr["MaKH"] = _makh;
                    _nr["MaHang"] = _mahang;
                    _nr["MaVT"] = _dr["MaVT"];
                    _nr["ChiTiet"] = _dr["ChiTiet"];
                    _nr["MaMauVT"] = _dr["MaMauVT"];
                    _nr["MauVT"] = _dr["MauVT"];
                    _nr["KhoVai"] = _dr["KhoVai"];
                    _nr["MaNhom"] = _dr["MaNhom"];
                    _nr["MaVTID"] = _dr["MaVTID"];
                    _nr["MauVTID"] = _dr["MauVTID"];
                    _nr["KhoVaiID"] = _dr["KhoVaiID"];
                    _nr["MaDot"] = madotpost;
                    _nr["Dot"] = tendotpost;
                    _nr["NguoiTao"] = GlobleData.UserName;
                    _nr["DinhMucChung"] = _dr["DinhMucChung"];
                    _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                    _nr["TachMau"] = true;//_dr["TachMau"];
                    _nr["STT"] = _stt;
                    _nr["MaCode"] = _dr["MaCode"];
                    _nr["STTCode"] = _dr["STTCode"];
                    dtVatTuSP.Rows.Add(_nr);
                }
                string url1 = $"{URL}KhoiTaoBOMV1/Post4?Action=POSTMAUSPEXCEL&para1={GlobleData.UserName}";
                string result1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, dtMauSP); }).Result;
                if (result1 != "True") return;
                string url2 = $"{URL}KhoiTaoBOMV1/Post5?Action=POSTDMSIZEEXCEL";
                string result2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, dtSizeSP); }).Result;
                if (result2 != "True") return;
                string url3 = $"{URL}KhoiTaoBOMV1/Post6?Action=POSTVTSPEXCEL";
                string result3 = Task.Run(async () => { return await _clientExtension.PostAsync(url3, dtVatTuSP); }).Result;
                if (result3 == "True")
                {
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    else
                        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    return;
                }
            }
            catch (Exception ex)
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            }
        }
        private DataTable createTypeVattuSP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("ChiTiet", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("MauVT", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("DinhMucChung", typeof(double));
            dt.Columns.Add("DinhMucChiTiet", typeof(double));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaVTGhep", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private DataTable createTypeMauSP()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("ChiTiet", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("MauVT", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private DataTable createTypeSizeSP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("ChiTiet", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("MauVT", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("MaNhomSize", typeof(string));
            dt.Columns.Add("MaSize", typeof(string));
            dt.Columns.Add("DinhMuc", typeof(double));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }


        #region PHÚUUUUUUUUUUUUUUUUUUUU
        private void loadDataTSVTCoSan()
        {
            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + $"ERPVatTuBOM/Get", _makh, "");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return;
            }
            tblts = JsonConvert.DeserializeObject<DataTable>(json);

        }
        private void gVNL_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            loadDataCheckTrung();
            loadCheckTrungTrongGrid();

            setTrung(tblCNL);
        }
        private void gVNL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            setMaVTGhep();
            loadDataCheckTrung();
            loadCheckTrungTrongGrid();

            setVatTuDinhMucChanged();
            setVatTuCapPhatChanged();
            setTrung(tblCNL);
        }
        private void gVPL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            setMaVTGhep();
            loadDataCheckTrung();
            loadCheckTrungTrongGrid();

            setVatTuDinhMucChanged();
            setVatTuCapPhatChanged();
            setTrung(tblCPL);
        }
        private void gVPL_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            loadDataCheckTrung();
            loadCheckTrungTrongGrid();
            //UpdateIsNewStatus();
            setTrung(tblCPL);

        }
        private void setTrung(DataTable tbl)
        {
            foreach (DataRow row in tbl.Rows)
            {

                int isNewValue = Convert.ToInt32(row["IsNew"]);

                if (isNewValue == 1)
                {
                    row["Trung"] = "Trùng vật tư có sẵn";
                }
                else if (isNewValue == 2)
                {
                    row["Trung"] = "Trùng giữa các vật tư trong Excel";
                }
                else
                {
                    row["Trung"] = "";
                }
            }
        }
        private void loadCheckTrungTrongGrid()
        {

            var grouped = tblCNL.AsEnumerable()
    .GroupBy(row => string.Join("|", new string[]
    {
        row["MaVT"].ToString().Trim(),
        row["ChiTiet"].ToString().Trim(),
        row["MaMauVT"].ToString().Trim(),
        row["MauVT"].ToString().Trim(),
        row["KhoVai"].ToString().Trim(),
        row["MaNhom"].ToString().Trim()
    }))
    .Where(g => g.Count() > 1);

            foreach (var group in grouped)
            {
                foreach (var row in group)
                {
                    row["IsNew"] = 2;
                }
            }
            var grouped2 = tblCPL.AsEnumerable()
    .GroupBy(row => string.Join("|", new string[]
    {
        row["MaVT"].ToString().Trim(),
        row["ChiTiet"].ToString().Trim(),
        row["MaMauVT"].ToString().Trim(),
        row["MauVT"].ToString().Trim(),
        row["KhoVai"].ToString().Trim(),
        row["MaNhom"].ToString().Trim()
    }))
    .Where(g => g.Count() > 1);


            foreach (var group in grouped2)
            {
                foreach (var row in group)
                {
                    row["IsNew"] = 2;
                }
            }
        }


        private void loadDataCheckTrung()
        {

            var existingKeys = new HashSet<string>(tblts.AsEnumerable().Select(row =>
         (row["MaNhom"] == DBNull.Value ? "" : row["MaNhom"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
         (row["MaVT"] == DBNull.Value ? "" : row["MaVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
         (row["ChiTiet"] == DBNull.Value ? "" : row["ChiTiet"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
         (row["MaMauVT"] == DBNull.Value ? "" : row["MaMauVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
         (row["MauVT"] == DBNull.Value ? "" : row["MauVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
         (row["KhoVai"] == DBNull.Value ? "" : row["KhoVai"].ToString().Trim().ToUpper().Replace(" ", ""))
     ));


            foreach (DataRow rowCNL in tblCNL.Rows)
            {

                string keyCNL = (rowCNL["MaNhom"] == DBNull.Value ? "" : rowCNL["MaNhom"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["MaVT"] == DBNull.Value ? "" : rowCNL["MaVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["ChiTiet"] == DBNull.Value ? "" : rowCNL["ChiTiet"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["MaMauVT"] == DBNull.Value ? "" : rowCNL["MaMauVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["MauVT"] == DBNull.Value ? "" : rowCNL["MauVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["KhoVai"] == DBNull.Value ? "" : rowCNL["KhoVai"].ToString().Trim().ToUpper().Replace(" ", ""));
                if (existingKeys.Contains(keyCNL))
                {
                    rowCNL["IsNew"] = 1;
                }
                else
                {
                    rowCNL["IsNew"] = 0;
                }
            }
            foreach (DataRow rowCNL in tblCPL.Rows)
            {
                string keyCNL = (rowCNL["MaNhom"] == DBNull.Value ? "" : rowCNL["MaNhom"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["MaVT"] == DBNull.Value ? "" : rowCNL["MaVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["ChiTiet"] == DBNull.Value ? "" : rowCNL["ChiTiet"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["MaMauVT"] == DBNull.Value ? "" : rowCNL["MaMauVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["MauVT"] == DBNull.Value ? "" : rowCNL["MauVT"].ToString().Trim().ToUpper().Replace(" ", "")) + "|" +
                                (rowCNL["KhoVai"] == DBNull.Value ? "" : rowCNL["KhoVai"].ToString().Trim().ToUpper().Replace(" ", ""));
                if (existingKeys.Contains(keyCNL))
                {
                    rowCNL["IsNew"] = 1;
                }
                else
                {
                    rowCNL["IsNew"] = 0;
                }
            }
        }





        private void gVNL_RowStyle(object sender, RowStyleEventArgs e)
        {
            //try
            //{
            //    if (e.RowHandle >= 0)
            //    {
            //        var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //        var isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
            //        int isNew = 0;
            //        if (isNewValue != null && int.TryParse(isNewValue.ToString(), out isNew))
            //        {
            //            if (isNew == 1)
            //            {
            //                e.Appearance.BackColor = Color.FromArgb(255, 235, 232);
            //                e.Appearance.ForeColor = Color.FromArgb(156, 0, 6);
            //            }
            //            else if (isNew == 2)
            //            {
            //                e.Appearance.BackColor = Color.LightGoldenrodYellow;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
        }

        private void gVPL_RowStyle(object sender, RowStyleEventArgs e)
        {
            //try
            //{
            //    if (e.RowHandle >= 0)
            //    {
            //        var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //        var isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
            //        int isNew = 0;
            //        if (isNewValue != null && int.TryParse(isNewValue.ToString(), out isNew))
            //        {
            //            if (isNew == 1)
            //            {
            //                e.Appearance.BackColor = Color.FromArgb(255, 235, 232);
            //                e.Appearance.ForeColor = Color.FromArgb(156, 0, 6);
            //            }
            //            else if (isNew == 2)
            //            { e.Appearance.BackColor = Color.LightGoldenrodYellow; }


            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

        }



        private void btnXoaDongTrung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("Bạn có chắc chắn muốn xóa tất cả các dòng trùng không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }



            if (tblCNL != null && tblCNL.Columns.Contains("IsNew"))
            {

                var rowsToDelete = tblCNL.AsEnumerable()
                                          .Where(row => row.Field<int?>("IsNew") != 0)
                                          .ToList();
                foreach (var row in rowsToDelete)
                {
                    row.Delete();
                }

                tblCNL.AcceptChanges();
            }
            if (tblCPL != null && tblCPL.Columns.Contains("IsNew"))
            {

                var rowsToDelete = tblCPL.AsEnumerable()
                                          .Where(row => row.Field<int?>("IsNew") != 0)
                                          .ToList();
                foreach (var row in rowsToDelete)
                {
                    row.Delete();
                }

                tblCPL.AcceptChanges();
            }

            RefreshSTTColumn(tblCNL);
            RefreshSTTColumn(tblCPL);
        }



        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            var selectedTabPage = e.Page;
            if (selectedTabPage == null) return;
            if (selectedTabPage.Name == "xtraTabPage1")
            {

                //_isNPL = true;
                //btnXoaDongTrung.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            }
            else if (selectedTabPage.Name == "xtraTabPage2")
            {
                //btnXoaDongTrung.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            }
            else if (selectedTabPage.Name == "xtraTabPage3")
            {
               
                

            }
        }
        private void setVatTuDinhMucChanged()
        {
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (dr == null) return;
            string sttindex = dr["STTIndex"].ToString().Trim();
            var rowsTrung = _dtThongTinVT.AsEnumerable()
                .Where(r => r["STTIndex"].ToString().Trim() == sttindex);
            foreach (var row in rowsTrung)
            {
                row["MaVTGhep"] = dr["MaVTGhep"];
                row["MaVT"] = dr["MaVT"];
                row["ChiTiet"] = dr["ChiTiet"];
                row["MaMauVT"] = dr["MaMauVT"];
                row["MauVT"] = dr["MauVT"];
                row["KhoVai"] = dr["KhoVai"];
                row["TenDVVT"] = dr["TenDVVT"];
            }
            var rowsTrung2 = _dtThongTinDM.AsEnumerable()
              .Where(r => r["STTIndex"].ToString().Trim() == sttindex);
            foreach (var row in rowsTrung2)
            {
                row["MaVTGhep"] = dr["MaVTGhep"];
                row["MaVT"] = dr["MaVT"];
                row["ChiTiet"] = dr["ChiTiet"];
                row["MaMauVT"] = dr["MaMauVT"];
                row["MauVT"] = dr["MauVT"];
                row["KhoVai"] = dr["KhoVai"];
                //  row["TenDVVT"] = dr["TenDVVT"];
            }
        }
        private void setVatTuCapPhatChanged()
        {
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (dr == null) return;
            string sttindex = dr["STTIndex"].ToString().Trim();
            var rowsTrung = tblThongSo.AsEnumerable()
                .Where(r => r["STTIndex"].ToString().Trim() == sttindex);
            foreach (var row in rowsTrung)
            {
                row["MaVTGhep"] = dr["MaVTGhep"];
                row["MaVT"] = dr["MaVT"];
                row["ChiTiet"] = dr["ChiTiet"];
                row["MaMauVT"] = dr["MaMauVT"];
                row["MauVT"] = dr["MauVT"];
                row["KhoVai"] = dr["KhoVai"];
                row["TenDVVT"] = dr["TenDVVT"];
            }
            var rowsTrung2 = tblChiTiet.AsEnumerable()
              .Where(r => r["STTIndex"].ToString().Trim() == sttindex);
            foreach (var row in rowsTrung2)
            {
                row["MaVTGhep"] = dr["MaVTGhep"];
                row["MaVT"] = dr["MaVT"];
                row["ChiTiet"] = dr["ChiTiet"];
                row["MaMauVT"] = dr["MaMauVT"];
                row["MauVT"] = dr["MauVT"];
                row["KhoVai"] = dr["KhoVai"];
                //  row["TenDVVT"] = dr["TenDVVT"];
            }
        }
        private void tabVT_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                var selectedTabPage = e.Page;
                if (selectedTabPage == null) return;
                if (selectedTabPage.Name == "tabNL")
                {

                    _isNPL = true;

                }
                else if (selectedTabPage.Name == "tabPL")
                {
                    _isNPL = false;

                }
            }
            catch (Exception ex)
            {


            }
        }

        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            tinhDinhMucGoiY();
            tinhDinhMucChung();
            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;
            gridView.BeginUpdate();
            try
            {
                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;
                    var targetRow = _dtThongTinDM.AsEnumerable().FirstOrDefault(row =>
                        row["MaVT"].ToString() == changedDataRow["MaVT"].ToString() &&
                         row["ChiTiet"].ToString() == changedDataRow["ChiTiet"].ToString() &&
                        row["MaMauVT"].ToString() == changedDataRow["MaMauVT"].ToString() &&
                         row["MauVT"].ToString() == changedDataRow["MauVT"].ToString() &&
                        row["KhoVai"].ToString() == changedDataRow["KhoVai"].ToString() &&
                        row["MaNhom"].ToString() == changedDataRow["MaNhom"].ToString() &&
                        row["MaNhomSize"].ToString() == changedDataRow["MaNhomSize"].ToString() &&
                        row["STTCode"].ToString() == changedDataRow["STTCode"].ToString() &&
                        row["MaMau"].ToString() == changedDataRow["MaMau"].ToString()
                    );

                    if (targetRow != null)
                    {
                        if (changedDataRow.Table.Columns.Contains(e.Column.FieldName))
                        {
                            targetRow[e.Column.FieldName] = changedDataRow[e.Column.FieldName];
                        }
                    }
                }
            }
            finally
            {
                gridView.EndUpdate();
            }

            tinhDinhMucGoiY();
            tinhDinhMucChung();
            setCapPhatDinhMucChanged();
        }

        private void setMaVTGhep()
        {
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (dr == null) return;
            string[] parts = dr["MaVTGhep"].ToString().Split('|');
            if (parts.Length > 0)
            {
                string nhomVT = parts.First();       // phần tử đầu
                string khvt = parts.Last();         // phần tử cuối

                dr["MaVTGhep"] = nhomVT + "|" + dr["MaVT"].ToString() + "|" + (dr["MaMauVT"].ToString() == "" ? "00" : dr["MaMauVT"].ToString()) + "|" + (dr["KhoVai"].ToString() == "" ? "00" : dr["KhoVai"].ToString());
            }

        }
        private void tinhDinhMucGoiY()
        {
            try
            {
                DataTable tbl = gCTTDM.DataSource as DataTable;

                if (tbl == null || tbl.Rows.Count == 0) return;

                List<string> sizeColumns = tbl.Columns
              .Cast<DataColumn>()
              .Where(c => c.ColumnName.Contains("@"))
              .Select(c => c.ColumnName)
              .ToList();

                //// 2. Nhóm theo MaMau
                //var groupedByMaMau = tbl.AsEnumerable()
                //    .Where(row => !row.IsNull("MaMau"))
                //    .GroupBy(row => row["MaMau"].ToString());
                var groupedByMaMau = _dtThongTinDM.AsEnumerable()
              .Where(row => !row.IsNull("MaMau") && !row.IsNull("STTIndex"))
              .GroupBy(row => new
              {
                  MaMau = row["MaMau"].ToString().Trim(),
                  STTIndex = row["STTIndex"].ToString().Trim()
              });
                var groupedByMaMau2 = tbl.AsEnumerable()
              .Where(row => !row.IsNull("MaMau") && !row.IsNull("STTIndex"))
              .GroupBy(row => new
              {
                  MaMau = row["MaMau"].ToString().Trim(),
                  STTIndex = row["STTIndex"].ToString().Trim()
              });
                // 3. Tính trung bình theo từng nhóm MaMau
                foreach (var group in groupedByMaMau)
                {
                    double total = 0;
                    int count = 0;

                    foreach (DataRow row in group)
                    {
                        foreach (string col in sizeColumns)
                        {
                            if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                            {
                                total += val;
                                count++;
                            }
                        }
                    }

                    double average = count > 0 ? total / count : 0;

                    // Gán giá trị trung bình cho tất cả dòng cùng MaMau
                    foreach (DataRow row in group)
                    {
                        row["DinhMucGY"] = Math.Round(average, 4);
                    }
                }
                foreach (var group in groupedByMaMau2)
                {
                    double total = 0;
                    int count = 0;

                    foreach (DataRow row in group)
                    {
                        foreach (string col in sizeColumns)
                        {
                            if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                            {
                                total += val;
                                count++;
                            }
                        }
                    }

                    double average = count > 0 ? total / count : 0;

                    // Gán giá trị trung bình cho tất cả dòng cùng MaMau
                    foreach (DataRow row in group)
                    {
                        row["DinhMucGY"] = Math.Round(average, 4);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void tinhDinhMucGoiYAll()
        {
            try
            {


                if (_dtThongTinDM == null || _dtThongTinDM.Rows.Count == 0) return;

                List<string> sizeColumns = _dtThongTinDM.Columns
              .Cast<DataColumn>()
              .Where(c => c.ColumnName.Contains("@"))
              .Select(c => c.ColumnName)
              .ToList();

                // 2. Nhóm theo MaMau
                var groupedByMaMau = _dtThongTinDM.AsEnumerable()
              .Where(row => !row.IsNull("MaMau") && !row.IsNull("STTIndex"))
              .GroupBy(row => new
              {
                  MaMau = row["MaMau"].ToString().Trim(),
                  STTIndex = row["STTIndex"].ToString().Trim()
              });


                // 3. Tính trung bình theo từng nhóm MaMau
                foreach (var group in groupedByMaMau)
                {
                    double total = 0;
                    int count = 0;

                    foreach (DataRow row in group)
                    {
                        foreach (string col in sizeColumns)
                        {
                            if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                            {
                                total += val;
                                count++;
                            }
                        }
                    }

                    double average = count > 0 ? total / count : 0;

                    // Gán giá trị trung bình cho tất cả dòng cùng MaMau
                    foreach (DataRow row in group)
                    {
                        row["DinhMucGY"] = Math.Round(average, 4);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void tinhDinhMucChung()
        {

            DataRow dr = gridView1.GetFocusedDataRow();
            DataTable tbl = gCTTDM.DataSource as DataTable;
            List<string> sizeColumns = tbl.Columns
                   .Cast<DataColumn>()
                   .Where(c => c.ColumnName.Contains("@"))
                   .Select(c => c.ColumnName)
                   .ToList();
            double total = 0;
            int count = 0;
            foreach (DataRow row in tbl.Rows)
            {


                foreach (string col in sizeColumns)
                {
                    if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                    {
                        total += val;
                        count++;
                    }
                }




            }
            double average = count > 0 ? total / count : 0;
            dr["DinhMucChung"] = Math.Round(average, 4);
        }

        private void tinhDinhMucChungALL()
        {
            foreach (DataRow dr in _dtThongTinVT.Rows)
            {


                DataTable tbl = _dtThongTinDM.Clone();
                var filteredRows = _dtThongTinDM.AsEnumerable()
                .Where(row => row["MaVT"].ToString() == dr["MaVT"].ToString() &&
                              row["ChiTiet"].ToString() == dr["ChiTiet"].ToString() &&
                              row["MaMauVT"].ToString() == dr["MaMauVT"].ToString() &&
                              row["MauVT"].ToString() == dr["MauVT"].ToString() &&
                              row["KhoVai"].ToString() == dr["KhoVai"].ToString() &&
                              row["MaNhom"].ToString() == dr["MaNhom"].ToString() &&
                              row["STTIndex"].ToString() == dr["STTIndex"].ToString());
                foreach (var row in filteredRows)
                {
                    tbl.ImportRow(row);
                }

                List<string> sizeColumns = tbl.Columns
                       .Cast<DataColumn>()
                       .Where(c => c.ColumnName.Contains("@"))
                       .Select(c => c.ColumnName)
                       .ToList();
                double total = 0;
                int count = 0;
                foreach (DataRow row in tbl.Rows)
                {


                    foreach (string col in sizeColumns)
                    {
                        if (row[col] != DBNull.Value && double.TryParse(row[col].ToString(), out double val) && val != 0)
                        {
                            total += val;
                            count++;
                        }
                    }




                }
                double average = count > 0 ? total / count : 0;
                dr["DinhMucChung"] = Math.Round(average, 4);
            }

        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn5)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn10)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                else if (groupIndex == 2)
                {
                    textColor = Color.Maroon;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }
        #endregion

        private bool checkTrungVT(DataTable tbl)
        {
            try
            {
                if (tbl == null || tbl.Rows.Count == 0) return false;

                HashSet<string> set = new HashSet<string>();
                foreach (DataRow row in tbl.Rows)
                {
                    string key =
                        (row["TenNhom"]?.ToString().Replace(" ", "").ToUpper() ?? "") + "|" +
                        (row["MaVT"]?.ToString().Replace(" ", "").ToUpper() ?? "") + "|" +
                        (row["ChiTiet"]?.ToString().Replace(" ", "").ToUpper() ?? "") + "|" +
                        (row["MaMauVT"]?.ToString().Replace(" ", "").ToUpper() ?? "") + "|" +
                        (row["MauVT"]?.ToString().Replace(" ", "").ToUpper() ?? "") + "|" +
                        (row["KhoVai"]?.ToString().Replace(" ", "").ToUpper() ?? "");

                    if (set.Contains(key))
                    {
                        XtraMessageBox.Show($"Dữ liệu bị trùng:\n{key}\nVui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                    set.Add(key);
                }

                return false;
            }
            catch (Exception ex)
            {
                return true;

            }
        }

        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            int cursorPosition = textEdit.SelectionStart;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                string newText = currentText.Insert(cursorPosition, e.KeyChar.ToString());
                if (newText.Contains("."))
                {
                    int indexOfDot = newText.IndexOf('.');
                    string decimalPart = newText.Substring(indexOfDot + 1);
                    if (decimalPart.Length > 4)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }
        private void bandedGridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == bandedGridColumn2)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT1" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gridView6_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", gridView6.GetSelectedRows().Select(rowHandle2 => gridView6.GetRowCellValue(rowHandle2, searchLookUpEditMauSP.Properties.ValueMember)));
            searchLookUpEditMauSP.EditValue = selectedValues;
            if (searchLookUpEditMauSP.EditValue is null)
            {
                searchLookUpEditInseam.Properties.DataSource = null;
                searchLookUpEditInseam.EditValue = null;
                searchLookUpEditSize.EditValue = null;
                return;
            }

            LoadNhomSize();
        }
        private void searchLookUpEditInseam_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessIS = string.Join("; ", gridView51.GetSelectedRows()
                .Select(rowHandle => gridView51.GetRowCellValue(rowHandle, searchLookUpEditInseam.Properties.DisplayMember)?.ToString().Trim())
                .Where(val => !string.IsNullOrEmpty(val))
                .Distinct());

            if (string.IsNullOrEmpty(selectedValuessIS))
            {
                e.DisplayText = "---Chưa chọn InSeam---";
            }
            else
            {
                e.DisplayText = selectedValuessIS;
            }
        }
        private void searchLookUpEditMauSP_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessS = string.Join("; ", gridView6.GetSelectedRows()
               .Select(rowHandle => gridView6.GetRowCellValue(rowHandle, searchLookUpEditMauSP.Properties.DisplayMember)?.ToString().Trim())
               .Where(val => !string.IsNullOrEmpty(val))
               .Distinct());

            if (string.IsNullOrEmpty(selectedValuessS))
            {
                e.DisplayText = "---Chưa chọn màu sản phẩm---";
            }
            else
            {
                e.DisplayText = selectedValuessS;
            }
        }

        private void searchLookUpEditSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedValuessS = string.Join("; ", searchLookUpEdit3View.GetSelectedRows()
               .Select(rowHandle => searchLookUpEdit3View.GetRowCellValue(rowHandle, searchLookUpEditSize.Properties.DisplayMember)?.ToString().Trim())
               .Where(val => !string.IsNullOrEmpty(val))
               .Distinct());

            if (string.IsNullOrEmpty(selectedValuessS))
            {
                e.DisplayText = "---Chưa chọn Size---";
            }
            else
            {
                e.DisplayText = selectedValuessS;
            }
        }
        private void btnXacNhanDinhMuc_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr1 = gridView1.GetFocusedDataRow();
                decimal _dm = decimal.TryParse(txtDinhMuc.Text, out var val) ? val : 0;

                int[] selectedRowHandles = searchLookUpEdit3View.GetSelectedRows();
                if (selectedRowHandles.Length == 0) return;
                DataTable selectedData = ((DataView)searchLookUpEdit3View.DataSource).Table.Clone();

                foreach (int handle in selectedRowHandles)
                {
                    DataRowView rowView = (DataRowView)searchLookUpEdit3View.GetRow(handle);
                    if (rowView != null)
                    {
                        selectedData.ImportRow(rowView.Row);
                    }
                }
                DataTable tblGC = gCTTDM.DataSource as DataTable;
                int[] selectedRowHandles2 = gridView6.GetSelectedRows();
                if (selectedRowHandles2.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn Màu sản phẩm.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;

                }
                DataTable tblMauSP = ((DataView)gridView6.DataSource).Table.Clone();

                foreach (int handle in selectedRowHandles2)
                {
                    DataRowView rowView = (DataRowView)gridView6.GetRow(handle);
                    if (rowView != null)
                    {
                        tblMauSP.ImportRow(rowView.Row);
                    }
                }
                HashSet<string> validMaMauSet = new HashSet<string>(
                tblMauSP.AsEnumerable().Select(r => r["MaMau"].ToString())
            );

                var selectedDict = new Dictionary<string, HashSet<string>>();

                foreach (DataRow row in selectedData.Rows)
                {
                    string maNhomSize = row["MaNhomSize"].ToString();
                    string sizeID = row["SizeID"].ToString();

                    if (!selectedDict.ContainsKey(maNhomSize))
                        selectedDict[maNhomSize] = new HashSet<string>();

                    selectedDict[maNhomSize].Add(sizeID);
                }
                foreach (DataRow dr in tblGC.Rows)
                {
                    string maMau = dr["MaMau"].ToString();
                    if (!validMaMauSet.Contains(maMau)) continue;
                    string maNhomSize = dr["MaNhomSize"].ToString();

                    if (!selectedDict.ContainsKey(maNhomSize)) continue;

                    var sizeIDs = selectedDict[maNhomSize];

                    for (int i = 19; i < tblGC.Columns.Count; i++)
                    {
                        string[] parts = tblGC.Columns[i].ColumnName.Split('@');
                        if (parts.Length < 2) continue;

                        string colSizeID = parts[1];

                        if (sizeIDs.Contains(colSizeID))
                        {
                            dr[i] = _dm;
                        }
                    }
                }
                Dictionary<string, DataRow> dictSize = _dtThongTinDM.AsEnumerable()
                  .ToDictionary(
                      row => $"{row["MaVT"]}_{row["ChiTiet"]}_{row["MaMauVT"]}_{row["MauVT"]}_{row["MaNhom"]}_{row["MaMau"]}_{row["MaNhomSize"]}_{row["STTCode"]}_{row["KhoVai"]}",
                      row => row
                  );
                foreach (DataRow dr in tblGC.Rows)
                {
                    string maMau = dr["MaMau"].ToString();
                    if (!validMaMauSet.Contains(maMau)) continue;
                    string key = $"{dr["MaVT"]}_{dr["ChiTiet"]}_{dr["MaMauVT"]}_{dr["MauVT"]}_{dr["MaNhom"]}_{dr["MaMau"]}_{dr["MaNhomSize"]}_{dr["MaCode"]}_{dr["KhoVai"]}";

                    if (dictSize.TryGetValue(key, out DataRow matchedRow))
                    {
                        foreach (DataColumn col in tblGC.Columns)
                        {
                            if (col.ColumnName.Contains("@"))
                            {
                                matchedRow[col.ColumnName] = dr[col.ColumnName];
                            }
                        }
                    }
                }


                tinhDinhMucGoiY();
                tinhDinhMucChung();
            }
            catch (Exception ex) { }

        }


        private void searchLookUpEdit3View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", searchLookUpEdit3View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit3View.GetRowCellValue(rowHandle2, searchLookUpEditSize.Properties.ValueMember)));
            searchLookUpEditSize.EditValue = selectedValues;
            if (searchLookUpEditSize.EditValue is null) return;
        }



        private void txtDinhMuc_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void gridView51_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", gridView51.GetSelectedRows().Select(rowHandle2 => gridView51.GetRowCellValue(rowHandle2, searchLookUpEditInseam.Properties.ValueMember)));
            searchLookUpEditInseam.EditValue = selectedValues;
            if (searchLookUpEditInseam.EditValue is null)
            {
                searchLookUpEditSize.Properties.DataSource = null;
                searchLookUpEditSize.EditValue = null;
                return;
            }
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETINSEAMSIZE&para1={_makh.ToString()}&para2={_mahang.ToString()}&para3={selectedValues.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditSize.Properties.DataSource = tbl;
            searchLookUpEditSize.Properties.ValueMember = "SizeID";
            searchLookUpEditSize.Properties.DisplayMember = "Size";
        }


        private void gVNL_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVNL.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongMenu);
                        e.Menu.Items.Add(menuCopyMauItem);
                    }

                }
            }
        }
        private void gVPL_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVNL.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongMenu);
                        e.Menu.Items.Add(menuCopyMauItem);
                    }

                }
            }
        }

        private void XoaDongMenu(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                DataTable tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0)
                    return;
                dr.Delete();
                tbl.AcceptChanges();

            }
            catch (Exception ex)
            {


            }

        }
        private void repositoryItemButtonEditMauSP_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //DataRow row = gridView1.GetFocusedDataRow();

            //if (row == null) return;
            //frmMauSanPhamV1 frm = new frmMauSanPhamV1(_makh, _mahang, row["MaMau"].ToString(), row["TenMau"].ToString());
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.ShowDialog();
            //if (frm.DialogResult != DialogResult.OK) return;
            //List<KeyValuePair<string, string>> result = frm.GetSelectedDataAsList();

            //if (result != null && result.Count > 0)
            //{
            //    row["MaMau"] = string.Join("|", result.Select(x => x.Value));
            //    row["TenMau"] = string.Join("|", result.Select(x => x.Key));

            //}
            //else
            //{
            //    row["MaMau"] = "";
            //    row["TenMau"] = "";
            //}
            ////row["TachMau"] = true;
            //this.ActiveControl = button1;

            //LoadSizeDMNew(row);
            //loadSearchLookUpFocues(row);
            ////checkBox1.Checked = true;
        }
        private void LoadSizeDMNew(DataRow _dr)
        {
            try
            {
                string[] mamau = _dr["MaMau"].ToString().Split('|');
                string[] tenmau = _dr["TenMau"].ToString().Split('|');
                HashSet<string> validMaMauSet = new HashSet<string>(mamau);
                //xóa màu 
                List<DataRow> rowsToDelete = new List<DataRow>();

                foreach (DataRow row in _dtThongTinDM.Rows)
                {

                    string currentRowMaMau = row["MaMau"].ToString();

                    bool areKeysMatching = row["MaVT"].ToString() == _dr["MaVT"].ToString() &&
                                           row["ChiTiet"].ToString() == _dr["ChiTiet"].ToString() &&
                                           row["MaMauVT"].ToString() == _dr["MaMauVT"].ToString() &&
                                           row["MauVT"].ToString() == _dr["MauVT"].ToString() &&
                                           row["KhoVai"].ToString() == _dr["KhoVai"].ToString() &&
                                           row["MaNhom"].ToString() == _dr["MaNhom"].ToString() &&
                                           row["STTIndex"].ToString() == _dr["STTIndex"].ToString();
                    if (areKeysMatching && !validMaMauSet.Contains(currentRowMaMau))
                    {

                        rowsToDelete.Add(row);
                    }
                }
                foreach (DataRow row in rowsToDelete)
                {
                    _dtThongTinDM.Rows.Remove(row);
                }
                _dtThongTinDM.AcceptChanges();
                var distinctSizes = _dtSize.AsEnumerable()
              .Select(row => new
              {
                  MaNhomSize = row["MaNhomSize"].ToString().Trim(),
                  NhomSize = row["NhomSize"].ToString().Trim()
              })
              .Distinct()
              .ToList();

                for (int i = 0; i < mamau.Length; i++)
                {
                    // Giả định _dr là một DataRow mà bạn đang muốn so sánh
                    bool areKeysMatching = _dtThongTinDM.AsEnumerable().Any(row =>
                        row["MaVT"].ToString() == _dr["MaVT"].ToString() &&
                        row["ChiTiet"].ToString() == _dr["ChiTiet"].ToString() &&
                        row["MaMauVT"].ToString() == _dr["MaMauVT"].ToString() &&
                        row["MauVT"].ToString() == _dr["MauVT"].ToString() &&
                        row["KhoVai"].ToString() == _dr["KhoVai"].ToString() &&
                        row["MaNhom"].ToString() == _dr["MaNhom"].ToString() &&
                        row["STTIndex"].ToString() == _dr["STTIndex"].ToString() &&
                        row["MaMau"].ToString() == mamau[i]
                    );
                    if (!areKeysMatching)
                    {
                        foreach (var nhomsize in distinctSizes)
                        {
                            DataRow _nr = _dtThongTinDM.NewRow();
                            _nr["MaKH"] = _makh;
                            _nr["MaHang"] = _mahang;
                            _nr["MaNhom"] = _dr["MaNhom"].ToString();
                            _nr["MaMau"] = mamau[i];
                            _nr["TenMau"] = tenmau[i];
                            _nr["MaNhomSize"] = nhomsize.MaNhomSize;
                            _nr["NhomSize"] = nhomsize.NhomSize;
                            _nr["MaVTGhep"] = _dr["MaVTGhep"].ToString();
                            _nr["MaVTID"] = _dr["MaVTID"].ToString();
                            _nr["MaVT"] = _dr["MaVT"].ToString();
                            _nr["ChiTiet"] = _dr["ChiTiet"].ToString();
                            _nr["MauVTID"] = _dr["MauVTID"].ToString();
                            _nr["MaMauVT"] = _dr["MaMauVT"].ToString();
                            _nr["MauVT"] = _dr["MauVT"].ToString();
                            _nr["KhoVaiID"] = _dr["KhoVaiID"].ToString();
                            _nr["KhoVai"] = _dr["KhoVai"].ToString();
                            _nr["STTCode"] = _dr["STTCode"].ToString();
                            _nr["STTIndex"] = _dr["STTIndex"].ToString();

                            _dtThongTinDM.Rows.Add(_nr);
                        }


                    }
                }

                DataTable tblSizeFiltered = _dtThongTinDM.Clone();
                var filteredRows = _dtThongTinDM.AsEnumerable()
                .Where(row => row["MaVT"].ToString() == _dr["MaVT"].ToString() &&
                              row["ChiTiet"].ToString() == _dr["ChiTiet"].ToString() &&
                              row["MaMauVT"].ToString() == _dr["MaMauVT"].ToString() &&
                              row["MauVT"].ToString() == _dr["MauVT"].ToString() &&
                              row["KhoVai"].ToString() == _dr["KhoVai"].ToString() &&
                              row["MaNhom"].ToString() == _dr["MaNhom"].ToString() &&
                              row["STTIndex"].ToString() == _dr["STTIndex"].ToString());
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }

                gCTTDM.DataSource = tblSizeFiltered;
                tinhDinhMucGoiY();
                tinhDinhMucChung();





            }
            catch (Exception ex) { }
        }
        private void searchLookUpEditLenhSX_EditValueChanged(object sender, EventArgs e)
        {
            DataRow dr = searchLookUpEdit1View.GetFocusedDataRow();
            if (dr == null) return;
            _magop = dr["MaGop"].ToString();
            _maLSX = dr["MaLenhSanXuat"].ToString();
            _madh = dr["MaDH"].ToString();
            string url = string.Format("{0}?makh={1}&&mahang={2}&&madot={3}&&magop={4}&&malenh={5}&&madh={6}",
                URL + "DonHangTong/GetCapPhatThongSo", _makh, _mahang,
                searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(),
                _magop.ToString(), _maLSX.ToString(), _madh.ToString());

            loaddataCanDoi();
            //LoaDM();
            LoadCanDoi();
        }

        private void gVThongSo_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == colTenNhom)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVThongSo.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gVThongSo.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void gridView5_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn77)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView5.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView5.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }


        private void loaddataCanDoi()
        {
            string urldvsx = string.Format("{0}?magop={1}&&malenhsanxuat={2}", URL + "DonHangTong/GetChiTietLenhSX", _magop, _maLSX);
            string jsondvsx = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvsx); }).Result;
            if (jsondvsx == "[]")
            {
                gridControl4.DataSource = null;
                return;
            }
            DataTable tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
            if (!tbldvsx.Columns.Contains("Tong"))
            {
                tbldvsx.Columns.Add("Tong", typeof(string));

            }

            var listMaMauDvsx = new HashSet<string>(tbldvsx.AsEnumerable().Select(r => r.Field<string>("MaMau")), StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in tblThongSo.Rows)
            {
                string maMauChuoi = row.Field<string>("MaMau") ?? "";
                var arrMau = maMauChuoi.Split('|')
                                       .Select(x => x.Trim());
                row["Status"] = arrMau.Any(m => listMaMauDvsx.Contains(m)) ? 1 : 0;
            }
            createTableCT(tbldvsx);
            gridControl4.DataSource = tbldvsx;
        }
        private void createTableCT(DataTable tab)
        {
            GridBand parentBand = bandedGridView3.Bands["gridBandSizeCT"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView3.Columns.Count;)
                {
                    if (bandedGridView3.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView3.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }
            int demColIndex = -1;

            foreach (DataColumn column in tab.Columns)
            {
                demColIndex++;
                if (demColIndex > 3 && demColIndex < tab.Columns.Count - 1)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = false;
                    col.Visible = true;
                    col.Width = 50;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    col.SummaryItem.DisplayFormat = "{0:N0}";
                    //col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView3.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    gridBandSizeCT.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

            foreach (DataRow row in tab.Rows)
            {
                int sum = 0;


                foreach (DataColumn col in tab.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        if (row[col] != DBNull.Value)
                        {
                            sum += Convert.ToInt32(row[col]);
                        }
                    }
                }
                row["Tong"] = sum;
            }

        }
        private decimal TinhCapPhatTong(string _mavtid, string _mauID, string mausanpham, string tachmau, DataTable dtDM)
        {

            DataTable dtSL = this.gridControl4.DataSource as DataTable;

            DataTable tbl = new DataTable();

            var mausp = mausanpham.ToString()
                           .Split('|')
                           .Select(m => m.Trim())
                           .ToList();
            var filteredRows = dtSL.AsEnumerable()
                .Where(row => mausp.Contains(row["MaMau"].ToString()));
            if (tachmau.ToString() == "False")
            {
                tbl.Columns.Add("DauSizeID", typeof(string));
                tbl.Columns.Add("DauSize", typeof(string));
                foreach (DataColumn column in dtSL.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        tbl.Columns.Add(column.ColumnName, typeof(float));
                    }
                }
                var validDauSizeIDs = new HashSet<string>(
                                 dtDM.AsEnumerable()
                                     .Select(row => row["MaNhomSize"].ToString())
                                     .Distinct()
                             );

                foreach (DataRow rowSL in dtSL.Rows)
                {
                    string dauSizeID = rowSL["DauSizeID"].ToString();


                    // Chỉ thêm nếu DauSizeID có trong dtDM
                    if (validDauSizeIDs.Contains(dauSizeID))
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                }
                DataTable dtSL_Filtered = filteredRows.Any()
                    ? filteredRows.CopyToDataTable()
                    : dtSL.Clone();

                var distinctDauSizeIds = dtDM.AsEnumerable().Select(row => row["MaNhomSize"].ToString())
                                                                  .Distinct()
                                                                  .ToList();
                var columnsWithAt = dtSL_Filtered.Columns.Cast<DataColumn>()
                                                .Where(col => col.ColumnName.Contains("@"))
                                                .ToList();
                foreach (DataRow rowDM in dtDM.Rows)
                {
                    foreach (var column in columnsWithAt)
                    {

                        string dauSizeIDDM = rowDM["DauSizeID"].ToString();
                        decimal totalQuantityInSL = dtSL_Filtered.AsEnumerable()
                                                           .Where(row => mausp.Contains(row["MaMau"]) &&
                                                                        row["DauSizeID"].ToString() == dauSizeIDDM.ToString())
                                                           .Sum(row =>
                                                               decimal.TryParse(row[column].ToString(), out decimal value)
                                                               ? Math.Round(value, 4)
                                                               : 0);
                        if (totalQuantityInSL > 0)
                        {
                            var rowDMs = dtDM.AsEnumerable()
                                            .FirstOrDefault(row => row["MaNhomSize"].ToString() == dauSizeIDDM.ToString() && rowDM.Table.Columns.Contains(column.ColumnName));

                            if (rowDM != null && rowDM.Table.Columns.Contains(column.ColumnName))
                            {
                                decimal quantityInDM = Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4);
                                decimal totalQuantity = totalQuantityInSL * quantityInDM;
                                DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}'");

                                foreach (DataRow rowTbl in rowsInTbl)
                                {
                                    foreach (DataColumn col in rowTbl.Table.Columns)
                                    {
                                        if (col.ToString() == column.ToString())
                                        {

                                            rowTbl[column.ColumnName] = totalQuantity;
                                        }
                                    }

                                }
                            }
                        }
                    }

                }
            }
            else
            {
                tbl.Columns.Add("DauSizeID", typeof(string));
                tbl.Columns.Add("DauSize", typeof(string));
                tbl.Columns.Add("MaMau", typeof(string));
                foreach (DataColumn column in dtSL.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        tbl.Columns.Add(column.ColumnName, typeof(float));
                    }
                }
                var validDauSizeIDs = new HashSet<string>(
                                 dtDM.AsEnumerable()
                                     .Select(row => row["MaNhomSize"].ToString())
                                     .Distinct()
                             );

                foreach (DataRow rowSL in dtSL.Rows)
                {
                    string dauSizeID = rowSL["DauSizeID"].ToString();
                    string maMau = rowSL["MaMau"].ToString();


                    // Chỉ thêm nếu DauSizeID có trong dtDM
                    if (validDauSizeIDs.Contains(dauSizeID))
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID && r["MAMau"].ToString() == maMau);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            newRow["MaMau"] = rowSL["MaMau"];
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                }
                foreach (var mamau in mausp)
                {
                    var filteredRowsByMau = dtSL.AsEnumerable()
                        .Where(row => row["MaMau"].ToString() == mamau)
                        .ToList();

                    DataTable dtSL_Filtered_ByMau = filteredRowsByMau.Any()
                        ? filteredRowsByMau.CopyToDataTable()
                        : dtSL.Clone();

                    var columnsWithAt = dtSL_Filtered_ByMau.Columns.Cast<DataColumn>()
                        .Where(col => col.ColumnName.Contains("@"))
                        .ToList();

                    foreach (DataRow rowDM in dtDM.AsEnumerable().Where(r => r["MaMau"].ToString() == mamau))
                    {
                        string dauSizeIDDM = rowDM["MaNhomSize"].ToString();

                        foreach (var column in columnsWithAt)
                        {
                            decimal totalQuantityInSL = dtSL_Filtered_ByMau.AsEnumerable()
                                .Where(row => row["DauSizeID"].ToString() == dauSizeIDDM)
                                .Sum(row =>
                                    decimal.TryParse(row[column].ToString(), out decimal value)
                                        ? Math.Round(value, 4)
                                        : 0);

                            if (totalQuantityInSL > 0)
                            {
                                decimal quantityInDM = rowDM.Table.Columns.Contains(column.ColumnName)
                                    ? Math.Round(decimal.TryParse(rowDM[column.ColumnName]?.ToString(), out var tmp) ? tmp : 0, 4)
                                    : 0;

                                decimal totalQuantity = totalQuantityInSL * quantityInDM;

                                DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}' AND MaMau = '{mamau}'");

                                foreach (DataRow rowTbl in rowsInTbl)
                                {
                                    if (rowTbl.Table.Columns.Contains(column.ColumnName))
                                    {
                                        rowTbl[column.ColumnName] = totalQuantity;
                                    }
                                }
                            }
                        }
                    }
                }

            }

            decimal sum = 0;
            foreach (DataColumn dc in tbl.Columns)
            {
                if (dc.ColumnName.Contains("@"))
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (decimal.TryParse(row[dc].ToString(), out decimal value))
                        {
                            sum += value;
                        }
                    }
                }
            }
            return sum;
        }
        private void LoadCanDoi()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));

                tblChiTiet.Clear();
                tblDinhMucCapPhat = pivotCapPhat(_dtDM);
                foreach (DataRow rowData in tblThongSo.Rows)
                {
                    if (Convert.ToInt32(rowData["Status"]) == 1)
                    {
                        string _mavtid = rowData["ChiTiet"].ToString();
                        string _mauID = rowData["MaMauVT"].ToString();
                        string maVTID = rowData["MaVT"].ToString();
                        string mauVTID = rowData["MaMau"].ToString();
                        string khoVaiID = rowData["KhoVai"].ToString();
                        string maNhom = rowData["MaNhom"].ToString();
                        string maCode = rowData["MauVT"].ToString();

                        // Lọc các dòng trùng khớp từ tblDM
                        var matchingRows = tblDinhMucCapPhat.AsEnumerable()
                            .Where(row =>
                                row["ChiTiet"].ToString().Trim() == _mavtid.ToString().Trim() &&
                                row["MaMauVT"].ToString().Trim() == _mauID.ToString().Trim() &&
                                row["KhoVai"].ToString().Trim() == khoVaiID.ToString().Trim() &&
                                row["MaNhom"].ToString().Trim() == maNhom.ToString().Trim() &&
                                row["MaVT"].ToString().Trim() == maVTID.ToString().Trim() &
                                row["MauVT"].ToString().Trim() == maCode.ToString().Trim()
                            );

                        // Copy về một DataTable mới
                        DataTable dtDM = matchingRows.Any() ? matchingRows.CopyToDataTable() : tblDinhMucCapPhat.Clone();

                        DataTable dtSL = this.gridControl4.DataSource as DataTable;
                        var mausp = new HashSet<string>(rowData["MaMau"].ToString()
                                                             .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(m => m.Trim()));

                        var distinctDauSizeIds = new HashSet<string>(dtDM.AsEnumerable()
                                                                         .Select(row => row["MaNhomSize"].ToString())
                                                                         .Distinct());

                        var duLieuDM = dtDM.AsEnumerable()
                            .ToDictionary(
                                row => $"{row["MaNhomSize"]}_{row["MaMau"]}",
                                row => row
                            );
                        var sizeColumns = dtSL.Columns.Cast<DataColumn>()
                                              .Where(c => c.ColumnName.Contains("@"))
                                              .Select(c => c.ColumnName)
                                              .ToList();

                        decimal totalQuantity = 0;
                        if (rowData["TachMau"].ToString() == "False")
                        {
                            foreach (DataRow rowSL in dtSL.Rows)
                            {
                                string dauSizeIdSL = rowSL["MaNhomSize"].ToString();
                                string maMauSL = rowSL["MaMau"].ToString();
                                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                                {
                                    string key = $"{dauSizeIdSL}_{rowData["MaMau"].ToString()}";
                                    if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                    {
                                        foreach (string columnName in sizeColumns)
                                        {
                                            if (dtDM.Columns.Contains(columnName))
                                            {
                                                var valueDM = rowDM[columnName];
                                                if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                                {
                                                    var valueSL = rowSL[columnName];
                                                    if (valueSL != DBNull.Value)
                                                    {
                                                        totalQuantity += Convert.ToDecimal(valueSL);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (DataRow rowSL in dtSL.Rows)
                            {
                                string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                                string maMauSL = rowSL["MaMau"].ToString();
                                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                                {
                                    string key = $"{dauSizeIdSL}_{maMauSL.ToString()}";
                                    if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                    {
                                        foreach (string columnName in sizeColumns)
                                        {
                                            if (dtDM.Columns.Contains(columnName))
                                            {
                                                var valueDM = rowDM[columnName];
                                                if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                                {
                                                    var valueSL = rowSL[columnName];
                                                    if (valueSL != DBNull.Value)
                                                    {
                                                        totalQuantity += Convert.ToDecimal(valueSL);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        decimal totalAverage = TinhCapPhatTong(_mavtid, _mauID, rowData["MaMau"].ToString(), rowData["TachMau"].ToString(), dtDM);
                        Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);

                        DataRow newRow = tblChiTiet.NewRow();
                        newRow["ID"] = 0;
                        newRow["MaKH"] = _makh;
                        newRow["MaHang"] = _mahang;
                        newRow["MaNPL"] = 0;
                        newRow["MaVTID"] = rowData["MaVTID"];
                        newRow["MaNhom"] = rowData["MaNhom"];
                        newRow["TenNhom"] = rowData["TenNhom"];
                        newRow["TenVT"] = rowData["ChiTiet"];
                        newRow["ChiTiet"] = rowData["ChiTiet"];
                        newRow["MaVT"] = rowData["MaVT"];
                        newRow["MaDot"] = _madot;
                        newRow["Dot"] = searchLookUpEditDot.Text.ToString();
                        newRow["MauID"] = rowData["MauVTID"];
                        newRow["MaMau"] = rowData["MaMau"];
                        newRow["TenMau"] = rowData["TenMau"];
                        newRow["MaMauVT"] = rowData["MaMauVT"];
                        newRow["MauVT"] = rowData["MauVT"];
                        newRow["KhoVaiID"] = rowData["KhoVaiID"];
                        newRow["KhoVai"] = rowData["KhoVai"];
                        newRow["MaDVVT"] = rowData["MaDVVT"];
                        newRow["TenDVVT"] = rowData["TenDVVT"];
                        newRow["STTIndex"] = rowData["STTIndex"];
                        newRow["DinhMucChung"] = finalAverage;
                        newRow["SoLuong"] = totalQuantity;
                        decimal _tk = 0, _slc=0;
                        _tk =rowData["TonKho"].ToString()==""?0: Convert.ToDecimal(rowData["TonKho"]);
                        _slc= rowData["SLCap"].ToString() == "" ? 0 : Convert.ToDecimal(rowData["SLCap"]);
                        int isCL = 0;
                        if (!string.IsNullOrEmpty(rowData["QDLe"].ToString()) && Convert.ToBoolean(rowData["QDLe"]))
                            isCL = 2;
                        else
                            isCL = 0;
                        if (rowData["DinhMucHaoHut"].ToString() != "" && rowData["DinhMucHaoHut"].ToString() != "0")
                        {
                            //Math.Round(Convert.ToDecimal(dr["CapPhat"]), 2, MidpointRounding.AwayFromZero);
                            decimal _dmhh = (Convert.ToDecimal(rowData["DinhMucHaoHut"]) / 100) + 1;
                            newRow["CapPhatTK"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                            newRow["CapPhat"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                            rowData["SLCap"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                            rowData["SLCL"] = Math.Round(_tk - _slc, isCL, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            newRow["CapPhatTK"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                            newRow["CapPhat"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                            rowData["SLCap"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                            rowData["SLCL"] = Math.Round(_tk - _slc, isCL, MidpointRounding.AwayFromZero);
                        }
                        newRow["CapThem"] = 0;
                        newRow["Sort"] = rowData["Sort"];
                        newRow["DinhMucHaoHut"] = rowData["DinhMucHaoHut"];
                        newRow["NPL"] = rowData["NPL"];
                        newRow["ThucXuat"] = Math.Round(totalAverage, 2); //Convert.ToDecimal(rowData["SLCL"]) < 0 ? 0 : rowData["SLCL"];
                        newRow["DinhMuc"] = finalAverage;
                        newRow["NhuCau"] = 0;
                        newRow["TachMau"] = 0;
                        newRow["MaVTGhep"] = rowData["MaVTGhep"];
                        newRow["TachMauSPDH"] = 0;
                        newRow["MaCode"] = "";
                        newRow["QDLe"] = rowData["QDLe"];
                        tblChiTiet.Rows.Add(newRow);

                    }
                }
                //var groupedRowss = tblThongSo.AsEnumerable()
                //.GroupBy(row => new
                //{
                //    MaVTID = row["ChiTiet"].ToString(),
                //    MauID = row["MauID"].ToString(),
                //    KhoVaiID = row["KhoVaiID"].ToString(),
                //    MaDVVT = row["MaDVVT"].ToString(),
                //    MaNhom = row["MaNhom"].ToString()
                //});

                //foreach (var group in groupedRowss)
                //{

                //    var orderedRows = group.OrderBy(r => r["MaCode"].ToString()).ToList();

                //    if (orderedRows.Count == 0)
                //        continue;

                //    var firstRow = orderedRows[0];
                //    decimal tonKho = firstRow["TonKho"] != DBNull.Value ? Convert.ToDecimal(firstRow["TonKho"]) : 0;
                //    decimal tongSLCap = orderedRows.Sum(r => r["SLCap"] != DBNull.Value ? Convert.ToDecimal(r["SLCap"]) : 0);

                //    decimal slcl = tonKho - tongSLCap;
                //    foreach (var row in orderedRows)
                //    {
                //        row["SLCL"] = slcl;
                //    }

                //}
                gridControl3.DataSource = tblChiTiet;
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            }
            catch (Exception ex)
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            }
        }
        private DataTable pivotCapPhat(DataTable sourceTable)
        {
            var uniqueSizes = sourceTable.AsEnumerable()
     .Select(row => row.Field<string>("TenSize") + "@Size@"
                   + row.Field<string>("MaSize")
                   )
     .Distinct()
     .OrderBy(x => x)
     .ToList();
            var groupedData = sourceTable.AsEnumerable()
                .GroupBy(row => new
                {
                    MaNhomSize = row.Field<string>("MaNhomSize"),
                    MaVT = row.Field<string>("MaVT"),
                    ChiTiet = row.Field<string>("ChiTiet"),
                    MaMauVT = row.Field<string>("MaMauVT"),
                    MauVT = row.Field<string>("MauVT"),
                    KhoVai = row.Field<string>("KhoVai"),
                    MaNhom = row.Field<string>("MaNhom"),
                    MaMau = row.Field<string>("MaMau")

                })
                .ToList();
            DataTable pivotTable = new DataTable();
            var excludeColumns = new[] { "MaSize", "TenSize", "DinhMuc" };
            foreach (DataColumn col in sourceTable.Columns)
            {
                if (!excludeColumns.Contains(col.ColumnName))
                {
                    pivotTable.Columns.Add(col.ColumnName, col.DataType);
                }
            }

            // Thêm các cột size làm cột pivot
            foreach (string size in uniqueSizes)
            {
                pivotTable.Columns.Add(size, typeof(object)); // Dùng object để linh hoạt với kiểu dữ liệu
            }

            // Điền dữ liệu vào pivot table
            foreach (var group in groupedData)
            {
                // Lấy dòng đại diện của nhóm để copy các cột không pivot
                var representativeRow = group.FirstOrDefault();

                if (representativeRow != null)
                {
                    DataRow newRow = pivotTable.NewRow();

                    // Copy các giá trị từ cột không pivot
                    foreach (DataColumn col in pivotTable.Columns)
                    {
                        if (sourceTable.Columns.Contains(col.ColumnName))
                        {
                            newRow[col.ColumnName] = representativeRow[col.ColumnName];
                        }
                    }
                    //var allMaMau = group.Select(row => row.Field<string>("MaMau")).Distinct().OrderBy(x => x).ToList();
                    //newRow["MaMau"] = string.Join("|", allMaMau);
                    //var allTenMau = group.Select(row => row.Field<string>("TenMau")).Distinct().OrderBy(x => x).ToList();
                    //newRow["TenMau"] = string.Join("|", allTenMau);
                    // Điền định mức cho từng size trong nhóm này
                    foreach (string size in uniqueSizes)
                    {
                        string[] arrNewHeader = size.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        var dinhMuc = group
                            .Where(row => row.Field<string>("MaSize") == arrNewHeader[2])
                            .Select(row => row["DinhMuc"])
                            .FirstOrDefault();

                        newRow[size] = dinhMuc ?? DBNull.Value;
                    }

                    pivotTable.Rows.Add(newRow);
                }
            }

            return pivotTable;
        }
        int quidoi = 1;

        private void checkBox3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.ToString() == "") return;
                if ((bool)checkBox3.Checked == true)
                {
                    DataTable _dt = gridControl3.DataSource as DataTable;
                    if (_dt == null) return;
                    foreach (DataRow row in _dt.Rows)
                    {
                        row["DinhMucHaoHut"] = Convert.ToDecimal(textBox2.Text.ToString().Trim());

                        tinhCapPhatHH(row);

                    }
                    reCalculateCanDoi();
                }

            }
            catch (Exception ex) { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if ((bool)checkBox1.Checked == true)
            {
                quidoi = 0;
                DataTable _dt = gridControl3.DataSource as DataTable;
                foreach (DataRow dr in _dt.Rows)
                {

                    dr["CapPhat"] = dr["CapPhat"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(dr["CapPhat"]), quidoi, MidpointRounding.AwayFromZero);
                    dr["ThucXuat"] = dr["ThucXuat"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(dr["ThucXuat"]), quidoi, MidpointRounding.AwayFromZero);

                }
            }
            else
            {
                quidoi = 2;
                DataTable _dt = gridControl3.DataSource as DataTable;
                foreach (DataRow dr in _dt.Rows)
                {
                    tinhCapPhat(dr);
                }


            }
            ganLaiNhuCau();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.ToString() == "") return;
                if ((bool)checkBox3.Checked == true)
                {
                    DataTable _dt = gridControl3.DataSource as DataTable;
                    if (_dt == null) return;
                    foreach (DataRow row in _dt.Rows)
                    {
                        row["DinhMucHaoHut"] = Convert.ToDecimal(textBox2.Text.ToString().Trim());
                        tinhCapPhatHH(row);

                    }
                    reCalculateCanDoi();
                }
            }
            catch (Exception ex) { }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                gVThongSo.ActiveFilterString = "[Status] = '1'";
            }
            else
            {

                gVThongSo.ActiveFilterString = string.Empty;
            }
        }

        private void tinhCapPhat(DataRow drChange)
        {

            try
            {
                int isCL = Convert.ToBoolean(drChange["QDLe"].ToString()) ? 2 : 0;
                Decimal _dmhh = 1, _nhucau = 0, _dm = 1, sl = 0;
                if (drChange["DinhMucHaoHut"].ToString() != "0" && drChange["DinhMucHaoHut"].ToString() != "")
                {
                    _dmhh += (Convert.ToDecimal(drChange["DinhMucHaoHut"]) / 100);
                }
                if (drChange["NhuCau"].ToString() != "0" && drChange["NhuCau"].ToString() != "")
                {
                    _nhucau += Convert.ToDecimal(drChange["NhuCau"]);
                }
                if (drChange["DinhMuc"].ToString() != "0" && drChange["DinhMuc"].ToString() != "")
                {
                    _dm = Convert.ToDecimal(drChange["DinhMuc"]);
                }
                if (drChange["SoLuong"].ToString() != "0" && drChange["SoLuong"].ToString() != "")
                {
                    sl = Convert.ToDecimal(drChange["SoLuong"]);
                }

                drChange["CapPhat"] = Math.Round(_dm * _dmhh * sl + _nhucau, isCL, MidpointRounding.AwayFromZero);
                if (_dmhh == 1 && _dm == 1 && sl == 1 && _nhucau == 0)
                {
                    drChange["CapPhat"] = 0;
                }
                else if(_dm==0 && _nhucau == 0)
                {
                    drChange["CapPhat"] = 0;
                }
                reCalculateNhuCau(drChange);
            }
            catch (Exception ex) { }

        }

        private void gVThongSo_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["Status"]);
                string Strflths = "0";//view.GetRowCellDisplayText(e.RowHandle, view.Columns["CheckTinh"]);
                if (Strflth == "1" && Strflths == "0")
                {
                    if (e.Column.VisibleIndex >= 0 && e.Column.VisibleIndex <= 7)
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFE4E9");

                    }
                }
                if (Strflth == "1" && Strflths == "1")
                {
                    if (e.Column.VisibleIndex >= 0 && e.Column.VisibleIndex <= 7)
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0FFF0");

                    }
                }
                if (Strflth == "0")
                {
                    if (e.Column.VisibleIndex >= 7 && e.Column.VisibleIndex <= 7)
                    {
                        e.Appearance.ForeColor = Color.Red;

                    }
                }

            }
        }

        private void gridView5_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;
            string _mausp = dr["MaMau"].ToString();
            string maVTID = dr["ChiTiet"].ToString();
            string mauVTID = dr["MaMauVT"].ToString();
            string khoVaiID = dr["KhoVai"].ToString();
            string maNhom = dr["MaNhom"].ToString();
            string maCode = dr["MauVT"].ToString();
            tblDinhMucCapPhat = pivotCapPhat(_dtDM);

            // Lọc các dòng trùng khớp từ tblDM
            var matchingRows = tblDinhMucCapPhat.AsEnumerable()
                .Where(row =>
                    row["ChiTiet"].ToString() == maVTID &&
                    row["MaMauVT"].ToString() == mauVTID &&
                    row["KhoVai"].ToString() == khoVaiID &&
                    row["MaNhom"].ToString() == maNhom &&
                    row["MauVT"].ToString() == maCode
                );

            // Copy về một DataTable mới
            DataTable dtDM = matchingRows.Any() ? matchingRows.CopyToDataTable() : tblDinhMucCapPhat.Clone();
            //DataTable dtDM = this.gridControl1.DataSource as DataTable;
            DataTable dtSL = this.gridControl4.DataSource as DataTable;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            foreach (DataColumn column in dtSL.Columns)
            {
                if (column.ColumnName.Contains("@"))
                {
                    tbl.Columns.Add(column.ColumnName, typeof(float));
                }
            }

            var validDauSizeIDs = new HashSet<string>(
                             dtDM.AsEnumerable()
                                 .Select(row => row["MaNhomSize"].ToString())
                                 .Distinct()
                         );

            foreach (DataRow rowSL in dtSL.Rows)
            {
                string dauSizeID = rowSL["DauSizeID"].ToString();

                // Chỉ thêm nếu DauSizeID có trong dtDM
                if (validDauSizeIDs.Contains(dauSizeID))
                {
                    if (dr["TachMau"].ToString() == "1")
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID && r["TenMau"].ToString() == rowSL["TenMau"].ToString());
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            if (dr["TachMau"].ToString() == "1")
                            {
                                newRow["TenMau"] = rowSL["TenMau"];
                            }
                            else
                            {
                                newRow["TenMau"] = "";
                            }
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            //if (dr["TachMau"].ToString() == "1")
                            //{
                            newRow["TenMau"] = rowSL["TenMau"];
                            //}
                            //else
                            //{
                            //    newRow["TenMau"] = "";
                            //}
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }

                    }
                }
            }
            GridView view = gVThongSo;
            object MaMau = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaMau = view.GetRowCellValue(childHandle, gridColumn2);
            }
            else
            {
                MaMau = view.GetFocusedRowCellValue(gridColumn2);
            }
            if (_mausp == null) return;
            var mausp = _mausp.ToString()
                           .Split('|')
                           .Select(m => m.Trim())
                           .ToList();
            var distinctDauSizeIds = dtDM.AsEnumerable().Select(row => row["MaNhomSize"].ToString())
                                                              .Distinct()
                                                              .ToList();
            var columnsWithAt = dtSL.Columns.Cast<DataColumn>()
                                            .Where(col => col.ColumnName.Contains("@"))
                                            .ToList();
            foreach (DataRow rowDM in dtDM.Rows)
            {
                foreach (var column in columnsWithAt)
                {

                    string dauSizeIDDM = rowDM["MaNhomSize"].ToString();
                    decimal totalQuantityInSL = dtSL.AsEnumerable()
                                                       .Where(row => mausp.Contains(row["MaMau"]) &&
                                                                    row["DauSizeID"].ToString() == dauSizeIDDM.ToString())
                                                       .Sum(row =>
                                                           decimal.TryParse(row[column].ToString(), out decimal value)
                                                           ? Math.Round(value, 4)
                                                           : 0);
                    if (totalQuantityInSL > 0)
                    {
                        var rowDMs = dtDM.AsEnumerable()
                                        .FirstOrDefault(row => row["MaNhomSize"].ToString() == dauSizeIDDM.ToString() && rowDM.Table.Columns.Contains(column.ColumnName));

                        if (rowDM != null && rowDM.Table.Columns.Contains(column.ColumnName))
                        {
                            decimal quantityInDM = Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4);
                            decimal totalQuantity = totalQuantityInSL * quantityInDM;
                            DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}'");

                            foreach (DataRow rowTbl in rowsInTbl)
                            {
                                foreach (DataColumn col in rowTbl.Table.Columns)
                                {
                                    if (col.ToString() == column.ToString())
                                    {

                                        rowTbl[column.ColumnName] = totalQuantity;
                                    }
                                }

                            }
                        }
                    }
                }

            }

            createTableCTCP(tbl);
            if (dr["TachMau"].ToString() == "1")
            {
                bandedGridColumn7.GroupIndex = 0;
            }
            else
            {
                bandedGridColumn7.GroupIndex = -1;
            }

            gridControl2.DataSource = tbl;
        }
        private void createTableCTCP(DataTable tab)
        {
            bandedGridView2.OptionsView.AllowCellMerge = false;
            GridBand parentBand = bandedGridView2.Bands["gridBandSizeCP"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView2.Columns.Count;)
                {
                    if (bandedGridView2.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView2.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }
            int demColIndex = -1;

            foreach (DataColumn column in tab.Columns)
            {
                demColIndex++;
                if (demColIndex > 2 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = false;
                    col.Visible = true;
                    col.Width = 75;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView2.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 75;
                    gridBandSizeCP.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }
        }

        private void gridView5_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Value == null || e.Value == "") return;
                GridView view = (GridView)sender;
                DataRow drChange = view.GetFocusedDataRow();
                if (drChange == null) return;

                if (e.Column.FieldName != "CapPhat")
                {
                    tinhCapPhat(drChange);
                }
                else if (e.Column.FieldName == "CapPhat")
                {
                    if (drChange == null) return;
                    reCalculateNhuCau(drChange);
                }

            }
            catch (Exception ex) { }
        }

        private void bandedGridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;


            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == bandedGridColumn23)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }

        private void reCalculateNhuCau(DataRow dr)
        {

            //DataRow dr = gridView1.GetFocusedDataRow();
            //if (dr == null) return;
            DataTable dt = gCThongSo.DataSource as DataTable;


            // Lấy giá trị từ dòng dr
            string maVTID = dr["ChiTiet"].ToString();
            string mauVTID = dr["MaMauVT"].ToString();
            string khoVaiID = dr["KhoVai"].ToString();
            string maNhom = dr["MaNhom"].ToString();
            string maCode = dr["MauVT"].ToString();
            foreach (DataRow row in dt.Rows)
            {
                if (row["ChiTiet"].ToString() == maVTID &&
                    row["MaMauVT"].ToString() == mauVTID &&
                    row["KhoVai"].ToString() == khoVaiID &&
                    row["MaNhom"].ToString() == maNhom &&
                    row["MauVT"].ToString() == maCode)
                {
                    row["SLCap"] = Convert.ToDecimal(dr["CapPhat"]) + Convert.ToDecimal(dr["CapThem"]);
                }
            }

        }

       
        private void ganLaiNhuCau()
        {
            DataTable dt = gCThongSo.DataSource as DataTable;
            DataTable tbl = gridControl3.DataSource as DataTable;

            foreach (DataRow dr in tbl.Rows)
            {
                string maVTID = dr["ChiTiet"].ToString();
                string mauVTID = dr["MaMauVT"].ToString();
                string khoVaiID = dr["KhoVai"].ToString();
                string maNhom = dr["MaNhom"].ToString();
                string maCode = dr["MauVT"].ToString();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["ChiTiet"].ToString() == maVTID &&
                        row["MaMauVT"].ToString() == mauVTID &&
                        row["KhoVai"].ToString() == khoVaiID &&
                        row["MaNhom"].ToString() == maNhom &&
                        row["MauVT"].ToString() == maCode)
                    {
                        row["SLCap"] = Convert.ToDecimal(dr["CapPhat"]) + Convert.ToDecimal(dr["CapThem"]);
                    }
                }
            }
            reCalculateCanDoi();
        }

       
        private void reCalculateCanDoi()
        {
            DataTable tbl = gCThongSo.DataSource as DataTable;
            foreach (DataRow dr in tbl.Rows)
            {
                dr["SLCL"] = Math.Round((decimal.TryParse(dr["TonKho"]?.ToString(), out var tk) ? tk : 0) - (decimal.TryParse(dr["SLCap"]?.ToString(), out var sc) ? sc : 0), 2);
            }
        }
        private void tinhCapPhatHH(DataRow drChange)
        {

            try
            {
                int isCL = Convert.ToBoolean(drChange["QDLe"].ToString()) ? 2 : 0;
                Decimal _dmhh = 1, _nhucau = 0, _dm = 1, sl = 1;
                if (drChange["DinhMucHaoHut"].ToString() != "0" && drChange["DinhMucHaoHut"].ToString() != "")
                {
                    _dmhh += (Convert.ToDecimal(drChange["DinhMucHaoHut"]) / 100);
                }
                if (drChange["NhuCau"].ToString() != "0" && drChange["NhuCau"].ToString() != "")
                {
                    _nhucau += Convert.ToDecimal(drChange["NhuCau"]);
                }
                if (drChange["DinhMuc"].ToString() != "0" && drChange["DinhMuc"].ToString() != "")
                {
                    _dm = Convert.ToDecimal(drChange["DinhMuc"]);
                }
                if (drChange["SoLuong"].ToString() != "0" && drChange["SoLuong"].ToString() != "")
                {
                    sl = Convert.ToDecimal(drChange["SoLuong"]);
                }

                drChange["CapPhat"] = Math.Round(_dm * _dmhh * sl + _nhucau, isCL, MidpointRounding.AwayFromZero);
                reCalculateNhuCau(drChange);

            }
            catch (Exception ex) { }

        }


        private void setCapPhatDinhMucChanged()
        {
            try
            {
                DataRow dr = bandedGridView1.GetFocusedDataRow();
                if (dr == null) return;
                string sttindex = dr["STTIndex"].ToString().Trim();
                string manhomsize = dr["MaNhomSize"].ToString().Trim();
                DataTable tbl = _dtDM.Copy();

                var rowsTrung = _dtDM.AsEnumerable()
                    .Where(r => r["STTIndex"].ToString().Trim() == sttindex && r["MaNhomSize"].ToString().Trim() == manhomsize);

                foreach (DataColumn column in dr.Table.Columns)
                {

                    if (column.ColumnName.Contains("@"))
                    {

                        string columnName = column.ColumnName.ToString().Split('@')[1];
                        object columnValue = dr[column.ColumnName];

                        rowsTrung
                 .Where(row => row.Table.Columns.Contains("MaSize") &&
                               row["MaSize"]?.ToString().Trim() == columnName)
                 .ToList()
                 .ForEach(row => row["DinhMuc"] = columnValue ?? DBNull.Value);
                    }
                }
                if (searchLookUpEditLenhSX.EditValue != null && !string.IsNullOrWhiteSpace(searchLookUpEditLenhSX.EditValue.ToString()))
                    LoadCanDoi();
            }
            catch (Exception ex)
            {

            }

            //foreach (var row in rowsTrung)
            //{
            //    row["MaVTGhep"] = dr["MaVTGhep"];
            //    row["MaVT"] = dr["MaVT"];
            //    row["ChiTiet"] = dr["ChiTiet"];
            //    row["MaMauVT"] = dr["MaMauVT"];
            //    row["MauVT"] = dr["MauVT"];
            //    row["KhoVai"] = dr["KhoVai"];
            //    row["TenDVVT"] = dr["TenDVVT"];
            //}
            //var rowsTrung2 = _dtThongTinDM.AsEnumerable()
            //  .Where(r => r["STTIndex"].ToString().Trim() == sttindex);
            //foreach (var row in rowsTrung2)
            //{
            //    row["MaVTGhep"] = dr["MaVTGhep"];
            //    row["MaVT"] = dr["MaVT"];
            //    row["ChiTiet"] = dr["ChiTiet"];
            //    row["MaMauVT"] = dr["MaMauVT"];
            //    row["MauVT"] = dr["MauVT"];
            //    row["KhoVai"] = dr["KhoVai"];
            //    //  row["TenDVVT"] = dr["TenDVVT"];
            //}
        }
        #region Phú sửa bom
        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";
              
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tbl;
             

            }
            catch (Exception ex)
            {
            }
        }
        private void gridView7_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {

            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;

            string manhonm = dr["MaNhom"].ToString();
            string tenNhom = dr["TenNhom"].ToString();
            if (string.IsNullOrEmpty(tenNhom))
                return; // Không có điều kiện ⇒ hiển thị toàn bộ

            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            var value = Convert.ToString(
                view.GetListSourceRowCellValue(e.ListSourceRow, "TenNhom")
            );

            if (!string.Equals(value, tenNhom, StringComparison.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }


        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow drF = gridView1.GetFocusedDataRow();
            if (string.IsNullOrWhiteSpace(drF["MaMau"].ToString())) return;
            if (e.Column.FieldName == "MaNhomChiTiet")
            {



                foreach (DataRow row in _dtSize.Rows)
                {
                    if (row["MaVTID"].ToString() == drF["MaVTID"].ToString() && row["MauVTID"].ToString() == drF["MauVTID"].ToString() && row["KhoVaiID"].ToString() == drF["KhoVaiID"].ToString() && row["MaNhom"].ToString() == drF["MaNhom"].ToString())
                    {
                        row["MaNhomChiTiet"] = drF["MaNhomChiTiet"];

                    }

                }


            }
        }

        #endregion
    }
}
