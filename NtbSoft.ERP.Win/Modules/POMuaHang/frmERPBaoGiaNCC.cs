using DevExpress.Data;
using DevExpress.Utils.Menu;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PopupMenuShowingEventArgs = DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmERPBaoGiaNCC : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        string _maPhieuBG = string.Empty;
        string _action = string.Empty;
        string _tenPhieu = string.Empty;
        string _phieuBGCopy = string.Empty;
        List<string> lstFormatFieldName = new List<string> { "ThanhTien", "SoLuongMuaThem", "DonGia", "ChiPhi", "DGSuaCKhau" , "DGSauThue" , "TotalCP_BG", "DGSauThueQD" };
        List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColEdit = new List<BandedGridColumn>();
        DataTable _tblPhieuBaoGia = new DataTable();
        DataTable _tblSaveChiPhiBG = new DataTable();
        DataTable _tblSaveChietKhauBG = new DataTable();
        DataTable _tblSaveThueBG = new DataTable();
        DataTable _tblQuyDoiTT = new DataTable();
        string _TienTe = string.Empty;
        string TienTeID_QD = string.Empty;string TienTeDeault = string.Empty;
        bool _isDuyet = false;
        public frmERPBaoGiaNCC(string action,bool IsDuyet ,string MaPhieuBG = "", string TenPhieu = "",string PhieuBGCopy = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _action = action;
            _maPhieuBG = MaPhieuBG;
            _tenPhieu = TenPhieu;
            _isDuyet = IsDuyet;
            _phieuBGCopy = PhieuBGCopy;
            lstColEdit.AddRange(new[]
            {
              colDonGia,colGhiChu,colTienTe,colChietKhauBG,colThueBG,colLeadtimeVatu,colDonGia
            });

            LoadLoaiVatTu();
            LoadTienTe();
            LoadPTThanhToan();
            SetupGridView();
            SetupLayout();
           
        }
      
        #region Init
        private DataTable CreateDataTable_PhieuBaoGia_ChiTiet()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoSizeID", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("MaDVTTe", typeof(string));
            dt.Columns.Add("TienTeID", typeof(string));
            dt.Columns.Add("PTThanhToan", typeof(string));
            dt.Columns.Add("Thue", typeof(decimal));
            dt.Columns.Add("PTVanChuyen", typeof(string));
            dt.Columns.Add("TenPhieu", typeof(string));
            dt.Columns.Add("NhomCLCC", typeof(string));
            dt.Columns.Add("MaNCC", typeof(string));
            dt.Columns.Add("ChungLoaiCC", typeof(string));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("MoTa", typeof(string));
            dt.Columns.Add("TenDVVT", typeof(string));
            dt.Columns.Add("MauVT", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("KhoVai", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("TenCL", typeof(string));
            dt.Columns.Add("LoaiNPL", typeof(string));
            dt.Columns.Add("IsNPL", typeof(bool));
            dt.Columns.Add("ChietKhau", typeof(decimal));
            dt.Columns.Add("NgayDuyet", typeof(DateTime));
            dt.Columns["NgayDuyet"].AllowDBNull = true;
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("NgayBDHieuLuc", typeof(DateTime));
            dt.Columns["NgayBDHieuLuc"].AllowDBNull = true;
            dt.Columns.Add("SoNgayHieuLuc", typeof(int));
            dt.Columns["SoNgayHieuLuc"].AllowDBNull = true;
            dt.Columns.Add("SoNgayGHSom", typeof(int));
            dt.Columns["SoNgayGHSom"].AllowDBNull = true;
            dt.Columns.Add("SoNgayGHTre", typeof(int));
            dt.Columns["SoNgayGHTre"].AllowDBNull = true;
            dt.Columns.Add("GhiChuPhieu", typeof(string));
            dt.Columns["GhiChuPhieu"].AllowDBNull = true;
            dt.Columns.Add("HinhThucThanhToan", typeof(string));
            dt.Columns.Add("TTThue", typeof(string));
            dt.Columns.Add("TTChietKhau", typeof(string));
            dt.Columns.Add("DonViTG_HieuLuc", typeof(string));
            dt.Columns.Add("DVTienTeVT", typeof(string));
            dt.Columns.Add("LeadtimeVatu", typeof(string));
            return dt;
        }
        private DataTable CreateTableSavePhieuBG()
        {
            DataTable dt = new DataTable("PhieuBaoGia");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("TenPhieu", typeof(string));
            dt.Columns.Add("NhomCLCC", typeof(string));
            dt.Columns.Add("MaNCC", typeof(string));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("NgayDuyet", typeof(string));
            dt.Columns.Add("NgayBDHieuLuc", typeof(string));
            dt.Columns.Add("SoNgayHieuLuc", typeof(int));
            dt.Columns.Add("SoNgayGHSom", typeof(int));
            dt.Columns.Add("SoNgayGHTre", typeof(int));
            dt.Columns.Add("NgayTao", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NgaySua", typeof(string));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("MaDVTTe", typeof(string));
            dt.Columns.Add("HinhThucThanhToan", typeof(string));
            dt.Columns.Add("PTThanhToan", typeof(string));
            dt.Columns.Add("PTVanChuyen", typeof(string));
            dt.Columns.Add("DonViTG_HieuLuc", typeof(string));
            dt.Columns.Add("Action", typeof(string));
            return dt;
        }
        private DataTable CreateTableSaveBaoGiaChiTiet()
        {
            DataTable dt = new DataTable("BaoGia_ChiTiet");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("NhomCLCC", typeof(string));
            dt.Columns.Add("MaNCC", typeof(string));
            dt.Columns.Add("ChungLoaiCC", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoSizeID", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("Thue", typeof(decimal));
            dt.Columns.Add("ChietKhau", typeof(decimal));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NgaySua", typeof(string));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("ThoiGianSua", typeof(string));
            dt.Columns.Add("DonViTienTe", typeof(string));
            dt.Columns.Add("Leadtime", typeof(string));
            return dt;
        }
        private DataTable CreateTableSaveChiPhi()
        {
            DataTable dt = new DataTable("ChiPhi");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("MaChiPhi", typeof(string));
            dt.Columns.Add("ChiPhi", typeof(decimal));
            dt.Columns.Add("Thue", typeof(decimal));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("DonViTienTe", typeof(string));
            return dt;
        }
        private DataTable CreateTableSaveChietKhau()
        {
            DataTable dt = new DataTable("ChietKhau");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("MaChietKhau", typeof(string));
            dt.Columns.Add("MaCLVTID", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("ChietKhau", typeof(decimal));
            return dt;
        }
        private DataTable CreateTableSaveThue()
        {
            DataTable dt = new DataTable("Thue");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaPhieuBG", typeof(string));
            dt.Columns.Add("MaThue", typeof(string));
            dt.Columns.Add("MaCLVTID", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("Thue", typeof(decimal));
            return dt;
        }
        private void FormatTextEditSoTien(params RepositoryItemTextEdit[] edits)
        {
       
            foreach (var edit in edits)
            {
                edit.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                edit.Appearance.Options.UseTextOptions = true;

                edit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                edit.Mask.EditMask = "n2";                    
                edit.Mask.UseMaskAsDisplayFormat = true;     

               
                edit.Mask.Culture = System.Globalization.CultureInfo.InvariantCulture;
                // hoặc: edit.Mask.Culture = new CultureInfo("en-US");
            }
        }

        private void SetCaptionBGVatTu()
        {
            if (string.IsNullOrEmpty(_TienTe))
            {
                gbDonGiaCoSo.Caption = $"Đơn giá cơ bản (7)";
                bgDonGiaSauThue.Caption = $"Đơn giá sau thuế  (13) = (10)  +( (10*12)/100)";
                bgDonGiaSuCK.Caption = $"Đơn giá sau chiết khấu (10) = (7) -( (7*9)/100)";
            }
            else
            {
                gbDonGiaCoSo.Caption = $"Đơn giá cơ bản ({_TienTe}) (7)";
                bgDonGiaSauThue.Caption = $"Đơn giá sau thuế ({_TienTe})  (13) = (10) +( (10*12)/100) ";
                bgDonGiaSuCK.Caption = $"Đơn giá sau chiết khấu ({_TienTe})  (10) = (7) -( (7*9)/100)";
            }
            
        }
        private void SetupLayout()
        {
            LoadQuyDoi();

            GroupHeader.ExpandButtonVisible = true;
            GroupHeader.Expanded = true;


            cbxDonViTG.Properties.Items.Clear();
            cbxDonViTG.Properties.Items.AddRange(new object[] { "Ngày", "Tháng", "Năm" });
            cbxDonViTG.SelectedIndex = 0;

            DateEditBDHieuLuc.Properties.DisplayFormat.FormatString = "dd-MM-yyyy";
            DateEditBDHieuLuc.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            DateEditBDHieuLuc.Properties.EditFormat.FormatString = "dd-MM-yyyy";
            DateEditBDHieuLuc.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;


            DateEditBDHieuLuc.Properties.Mask.EditMask = "dd-MM-yyyy";
            DateEditBDHieuLuc.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            DateEditBDHieuLuc.Properties.Mask.UseMaskAsDisplayFormat = true;

            if (_action == "add")
            {
                DateEditBDHieuLuc.EditValue = DateTime.Now;
                GenerateMaPhieu();
                _isDuyet = false;

            }
            else if (_action == "edit")
            {

                LoadThueBG();
                LoadChietKhau();             
                LoadPhieuBG();
            }
            else if(_action== "copy")
            {
             
                LoadThueBG();
                LoadChietKhau();
                LoadPhieuBG();
                _isDuyet = false;
            }
            SetCaptionGridView();
            LoadChiPhiBG();
            FormatTextEditSoTien(
            txtDGiaVatTu.Properties,
            txtChiPhatSinh.Properties,
            txtChiPhiSauThueCK.Properties
        );
           
        }

        private void Txt_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value)
            {
                e.DisplayText = "";
                return;
            }

            decimal val;
            if (decimal.TryParse(e.Value.ToString(), out val))
            {
               
                e.DisplayText = string.Format("{0:n2} {1}", val, _TienTe);
            }
        }

        private void GenerateMaPhieu()
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=GeneratePhieu";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                int CountSoPhieu = 0;
                if (json != "[]")
                {
                    DataTable tblMaPhieu = JsonConvert.DeserializeObject<DataTable>(json);
                    _maPhieuBG = tblMaPhieu.Rows[0]["SoPhieu"]?.ToString();
                }
                else
                {
                    XtraMessageBox.Show("Lỗi tạo mã phiếu vui lòng thử lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void LoadTenPhieu()
        {
            try
            {
                string maLoaiCC = SearchLookUpLoaiCC.EditValue?.ToString();
                string maNCC = SearchLookupNhaCC.EditValue?.ToString();
                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string url = $"{URL}PhieuBaoGia/GET?action=GetCountPhieu&para1={maLoaiCC}&para2={maNCC}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                int CountSoPhieu = 0;
                if (json != "[]")
                {
                    DataTable tblPhieu = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tblPhieu != null && tblPhieu?.Rows?.Count > 0)
                    {
                        var validCounts = tblPhieu.AsEnumerable()
                        .Select(row => row["CountPhieu"]?.ToString().Trim())
                        .Where(s => !string.IsNullOrEmpty(s) && int.TryParse(s, out _))
                        .Select(s => int.Parse(s));

                        if (validCounts.Any())
                        {
                            CountSoPhieu = validCounts.Max();
                        }
                    }
                }
                _tenPhieu = $"PBG|{SearchLookupNhaCC?.Text?.ToString()}|{DateTime.Now.ToString("ddMMyyyy-hhss")}|{CountSoPhieu + 1}";
                txtSoPhieu.Text = _tenPhieu;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        private void LoadNhaCC()
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=GetNCC&para1={SearchLookUpLoaiCC.EditValue?.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblNCC = JsonConvert.DeserializeObject<DataTable>(json);
                SearchLookupNhaCC.Properties.DataSource = tblNCC;
                SearchLookupNhaCC.Properties.ValueMember = "MaNhaCC";
                SearchLookupNhaCC.Properties.DisplayMember = "TenNCC";

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
        private void LoadLoaiVatTu()
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=LoaiCC";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblNCC = JsonConvert.DeserializeObject<DataTable>(json);
                SearchLookUpLoaiCC.Properties.DataSource = tblNCC;
                SearchLookUpLoaiCC.Properties.ValueMember = "MaLoaiNCC";
                SearchLookUpLoaiCC.Properties.DisplayMember = "TenNhom";
                if(tblNCC != null && tblNCC?.Rows?.Count > 0)
                {
                    SearchLookUpLoaiCC.EditValue = tblNCC?.Rows[0]["MaLoaiNCC"];
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }
        private void LoadTienTe()
        {
            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=GetTienTe";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblTTe = JsonConvert.DeserializeObject<DataTable>(json);
                SearchLookupDVTienTe.Properties.DataSource = tblTTe;
                SearchLookupDVTienTe.Properties.ValueMember = "TienTeID";
                SearchLookupDVTienTe.Properties.DisplayMember = "MaTienTe";


            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
        private void LoadQuyDoi()
        {

            try
            {
                string url = $"{URL}PhieuBaoGia/GET?action=GetQuyDoiTT";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                 _tblQuyDoiTT = JsonConvert.DeserializeObject<DataTable>(json);
               if(_tblQuyDoiTT != null && _tblQuyDoiTT?.Rows?.Count > 0)
                {
                    var Query = _tblQuyDoiTT.AsEnumerable().FirstOrDefault(x => x["MaTienTe"]?.ToString()?.ToUpper() == "VND");
                    if(Query!= null)
                    {
                        TienTeDeault = Query["TienTeID"]?.ToString();
                        TienTeID_QD = Query["TienTeID"]?.ToString();
                        _TienTe = Query["MaTienTe"]?.ToString();
                    }
                }
                if (_action == "add")
                {
                    SearchLookupDVTienTe.EditValue = TienTeDeault;
                }
              
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
           
        }
        private void LoadPTThanhToan()
        {
            try
            {
                string url = string.Format("{0}?action={1}", URL + "DicThuVienBaoGia/Get", "GetPTThanhToan");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblTTe = JsonConvert.DeserializeObject<DataTable>(json);
                SearchLookupPTThanhToan.Properties.DataSource = tblTTe;
                SearchLookupPTThanhToan.Properties.ValueMember = "MaPTThanhToan";
                SearchLookupPTThanhToan.Properties.DisplayMember = "ThanhToan";


            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
        private void searchLookUpEditLoaiCC_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchLookUpLoaiCC.EditValue?.ToString()))
            {
                if(_action == "add")
                {
                    SearchLookupNhaCC.EditValue = null;
                    gcVatTuPhieuBG_NPL.DataSource = null;
                }
               
                LoadNhaCC();
            }
        }
        private void SearchLookupNhaCC_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            DataRow row = edit.Properties.View.GetFocusedDataRow();
            if (edit.EditValue == null || edit.EditValue == DBNull.Value || row == null)
            {
                return;
            }
            if (_action == "add")
            {
                LoadTenPhieu();
                gcVatTuPhieuBG_NPL.DataSource = null;
            }
            //SearchLookupDVTienTe.EditValue = row["TienTeID"]?.ToString();
            //_TienTe = row["MaTienTe"]?.ToString();
            //SetCaptionGridView();
            //SetCaptionBGVatTu();


        }
        private void SearchLookupDVTienTe_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            DataRow row = edit.Properties.View.GetFocusedDataRow();
            if (edit.EditValue == null || edit.EditValue == DBNull.Value || row == null)
            {
                return;
            }
            _TienTe = row["MaTienTe"]?.ToString();
            TienTeID_QD = row["TienTeID"]?.ToString();

            SetCaptionGridView();
           
            bgvVatTuPhieuBG_NPL.RefreshData();
            bgvVatTuPhieuBG_NPL.UpdateSummary();

            grvChiPhi.RefreshData();
            grvChiPhi.UpdateSummary();

            TinhToanTongGiaTri(bgvVatTuPhieuBG_NPL);
        }

        private void SetCaptionGridView()
        {
            colTongChiPhi.Caption = $"Total amount ({ _TienTe})";
            gridBand3.Caption = $"Đơn giá sau thuế quy đổi ({_TienTe}) (15)";
        }
        private void SetupGridView()
        {
            grvLSBaoGia.ColumnPanelRowHeight = 50;
            bgvVatTuPhieuBG_NPL.ColumnPanelRowHeight = 50;
            grvChiPhi.ColumnPanelRowHeight = 50;
            grvChietKhau.ColumnPanelRowHeight = 50;
            grvThue.ColumnPanelRowHeight = 50;

            SetUpColChungLoai();
            SetUpColLoaiThue();
            SetUpColLoaiChietKhau();
            SetUpColLoaiChiPhi();
            SetUpColVatTuBGEdit();
            SetUpColDonViTienTe();
        }
        private void SetUpColVatTuBGEdit()
        {

            var rItemDecimal = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit
            {
                AllowNullInput = DevExpress.Utils.DefaultBoolean.True,
            
                DisplayFormat = { FormatType = DevExpress.Utils.FormatType.None },
                EditFormat = { FormatType = DevExpress.Utils.FormatType.None }
            };

            var rItemDecimalCP = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            

            var rItemMoneySpinEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
            {

                IsFloatValue = true,

                MinValue = 0,
                MaxValue = decimal.MaxValue,

                AllowNullInput = DevExpress.Utils.DefaultBoolean.True,
                Mask = { MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric },
            };
            rItemMoneySpinEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            rItemMoneySpinEdit.EditFormat.FormatString = "#,##0.####";

            colDonGia.ColumnEdit = rItemMoneySpinEdit;
            colDGSuaCKhau.ColumnEdit = rItemMoneySpinEdit;
            colDGSauThue.ColumnEdit = rItemMoneySpinEdit;
            colDGSauThue_QD.ColumnEdit = rItemMoneySpinEdit;

            colChietKhau.ColumnEdit = rItemDecimal;
            colChietKhauBG.ColumnEdit = rItemDecimal;
            colChiPhi_CP.ColumnEdit = rItemDecimalCP;
            colThueCP.ColumnEdit = rItemDecimal;
            colThueBG.ColumnEdit = rItemDecimal;
            colSoThue.ColumnEdit = rItemDecimal;
          

        }
     
        private DataTable tblThuVienChiPhi = new DataTable();
        private void SetUpColLoaiChiPhi()
        {
            string url = string.Format("{0}?action={1}", URL + "PhieuBaoGia/Get", "GetChiPhi");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                tblThuVienChiPhi = JsonConvert.DeserializeObject<DataTable>(json);
            }

            RepositoryItemSearchLookUpEdit rItemEdit = new RepositoryItemSearchLookUpEdit();
            rItemEdit.DataSource = tblThuVienChiPhi;
            rItemEdit.DisplayMember = "TenChiPhi";
            rItemEdit.ValueMember = "MaChiPhi";
            rItemEdit.ShowClearButton = false;
            rItemEdit.NullText = "[Chọn Tên Chi Phí]";
            rItemEdit.ImmediatePopup = true;
            rItemEdit.TextEditStyle = TextEditStyles.Standard;
            rItemEdit.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;

            GridView dvView = rItemEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;

                dvView.Columns.Add(new GridColumn { FieldName = "MaChiPhi", Caption = "MaChiPhi", Name = "rMaChiPhi", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenChiPhi", Caption = "Tên Chi Phí", Name = "rTenChiPhi", Visible = true, Width = 150 });
                dvView.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Nhóm Chi Phí", Name = "rTenNhom", Visible = true, Width = 150 });
                dvView.Columns.Add(new GridColumn { FieldName = "GhiChu", Caption = "Ghi chú", Name = "rGhiChuChiPhi", Visible = true, Width = 200 });
            }

            colTenChiPhi.ColumnEdit = rItemEdit;
            rItemEdit.Popup += rItemEdit_ChiPhi_Popup;
        }

        private void rItemEdit_ChiPhi_Popup(object sender, EventArgs e)
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

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddChiPhiNew"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroupChiPhi",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    layout.Root.AddItem(buttonGroup);

                    var txtChiPhiNew = new TextEdit()
                    {
                        Name = "txtChiPhiNew",
                        Properties =
                {
                    NullText = "Nhập tên chi phí mới...",
                    NullValuePrompt = "Nhập tên chi phí mới..."
                }
                    };

                    txtChiPhiNew.Tag = ownerEdit;

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtChiPhiNew,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    var btnAddChiPhi = new SimpleButton()
                    {
                        Name = "btnAddChiPhiNew",
                        Text = "Thêm chi phí"
                    };

                    btnAddChiPhi.Tag = txtChiPhiNew;
                    btnAddChiPhi.Click += btnAddChiPhi_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddChiPhi,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(110, 30),
                        MaxSize = new Size(110, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);

                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    txtChiPhiNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddChiPhi.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    var controlTextEdit = layout?.Controls.OfType<TextEdit>().FirstOrDefault(c => c.Name == "txtChiPhiNew");
                    if (controlTextEdit != null)
                    {
                        controlTextEdit.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddChiPhi_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtChiPhiNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtChiPhiNew?.Tag as SearchLookUpEdit;

                if (txtChiPhiNew != null && searchLookUpEdit != null)
                {
                    string tenChiPhi = txtChiPhiNew.Text.Trim();
                    string maChiPhi = string.Empty;

                    if (string.IsNullOrWhiteSpace(tenChiPhi))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên chi phí!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtChiPhiNew.Focus();
                        return;
                    }

                   
                    if (tblThuVienChiPhi != null && tblThuVienChiPhi.Rows.Count > 0)
                    {
                        var existingRow = tblThuVienChiPhi.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenChiPhi")) &&
                                r.Field<string>("TenChiPhi").Trim().Equals(tenChiPhi.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            SetUpColLoaiChiPhi();
                            maChiPhi = existingRow.Field<int>("MaChiPhi").ToString();
                       
                        }
                        else
                        {
                           
                            maChiPhi = LuuChiPhiMoi(tenChiPhi);
                        }
                    }
                    else
                    {
                        maChiPhi = LuuChiPhiMoi(tenChiPhi);
                    }

                    if (!string.IsNullOrEmpty(maChiPhi))
                    {
                        int currentRowHandle = grvChiPhi.FocusedRowHandle;

                        SetUpColLoaiChiPhi();

                        if (currentRowHandle >= 0)
                        {
                   
                            grvChiPhi.SetRowCellValue(currentRowHandle, colTenChiPhi, maChiPhi);
                            grvChiPhi.PostEditor();
                            grvChiPhi.UpdateCurrentRow();
                        }

                        grcChiPhi.RefreshDataSource();
                        

                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu chi phí!", "Lỗi",
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

        private string LuuChiPhiMoi(string tenChiPhi)
        {
            try
            {
      
                DataTable dtChiPhi = new DataTable();
                dtChiPhi.Columns.Add("MaChiPhi", typeof(int));
                dtChiPhi.Columns.Add("MaNhom", typeof(string));
                dtChiPhi.Columns.Add("ChiPhi", typeof(string));
                dtChiPhi.Columns.Add("GhiChu", typeof(string));

                DataRow row = dtChiPhi.NewRow();
                row["MaChiPhi"] = 0;
                row["ChiPhi"] = tenChiPhi.Trim();
                row["MaNhom"] = "1";
                row["GhiChu"] = "";
                dtChiPhi.Rows.Add(row);

           

         
                string url = string.Format("{0}?", URL + "NhaCC/PostCP");
                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, dtChiPhi);
                }).Result;

           
                if (result?.ToLower() == "true")
                {
                    SetUpColLoaiChiPhi();

                   
                    // Bước 5: Tìm MaChiPhi
                    if (tblThuVienChiPhi != null && tblThuVienChiPhi.Rows.Count > 0)
                    {
                        var newRow = tblThuVienChiPhi.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenChiPhi")) &&
                                r.Field<string>("TenChiPhi").Trim().Equals(tenChiPhi.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            
                            return newRow["MaChiPhi"].ToString();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu chi phí: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    

        private void SetUpColChungLoai()
        {
            DataTable tbl = new DataTable();
            string url = string.Format("{0}?action={1}", URL + "PhieuBaoGia/Get", "GetChungLoaiVT");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }
            RepositoryItemSearchLookUpEdit rItemEdit = new RepositoryItemSearchLookUpEdit();
            rItemEdit.DataSource = tbl;
            rItemEdit.DisplayMember = "ChungLoaiVatTu";
            rItemEdit.ValueMember = "MaCLVT";
            rItemEdit.ShowClearButton = false;
            rItemEdit.NullText = "[Chọn chủng loại vật tư]";
            rItemEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rItemEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rItemEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaCLVT", Caption = "MaCLVT", Name = "rcolMaCLVT", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "ChungLoaiVatTu", Caption = "Chủng loại vật tư", Name = "rcolChungLoaiVatTu", Visible = true });

            }
            colNhomVT_CK.ColumnEdit = rItemEdit;
            colLoaiVTThue.ColumnEdit = rItemEdit;
            colLoaiVTThue.ColumnEdit.EditValueChanged += RItemCLVT_EditValueChanged;
            colNhomVT_CK.ColumnEdit.EditValueChanged += ColumnCLVatTuChietKhauEdit_EditValueChanged;
        }

        DataTable tblThuVienThue = new DataTable();
        private void SetUpColLoaiThue()
        {
            tblThuVienThue = new DataTable();
            string url = string.Format("{0}?action={1}", URL + "PhieuBaoGia/Get", "GetThue");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                tblThuVienThue = JsonConvert.DeserializeObject<DataTable>(json);
            }
            RepositoryItemSearchLookUpEdit rItemEdit = new RepositoryItemSearchLookUpEdit();
            rItemEdit.DataSource = tblThuVienThue;
            rItemEdit.DisplayMember = "Thue";
            rItemEdit.ValueMember = "MaThue";
            rItemEdit.ShowClearButton = false;
            rItemEdit.NullText = "[Chọn thuế]";
            rItemEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rItemEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rItemEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaThue", Caption = "MaThue", Name = "rcolMaThue", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "Thue", Caption = "Thuế", Name = "rThue", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "NhomThue", Caption = "Nhóm nhóm thuế", Name = "rNhomThue", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "GhiChu", Caption = "Ghi chú", Name = "rGhiChuThue", Visible = true });
            }

            colLoaiThue.ColumnEdit = rItemEdit;
            rItemEdit.EditValueChanged += RItemLoaiThue_EditValueChanged;
            rItemEdit.Popup += new System.EventHandler(this.rItemEdit_Popup);
        }

        private void rItemEdit_Popup(object sender, EventArgs e)
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

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddThueNew"))
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

                    // Thiết lập cột layout
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // TextEdit
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Button Thêm

                    // 3 hàng: trên - giữa (controls) - dưới
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    layout.Root.AddItem(buttonGroup);

                    // TextEdit để nhập thuế mới
                    var txtThueNew = new TextEdit()
                    {
                        Name = "txtThueNew",
                        Properties =
                {
                    NullText = "Nhập tên thuế mới...",
                    NullValuePrompt = "Nhập tên thuế mới..."
                }
                    };

                    // Lưu reference để sử dụng trong event button
                    txtThueNew.Tag = ownerEdit;

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtThueNew,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30) // Width = 0 để tự động điều chỉnh theo Percent
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    // Nút Thêm Thuế
                    var btnAddThue = new SimpleButton()
                    {
                        Name = "btnAddThueNew",
                        Text = "Thêm Thuế"
                    };

                    // Lưu reference của TextEdit vào Tag của button
                    btnAddThue.Tag = txtThueNew;
                    btnAddThue.Click += btnAddThueNew_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddThue,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(100, 30),
                        MaxSize = new Size(100, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);

                    // Empty spaces
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    // Thêm event KeyDown cho TextEdit để nhấn Enter cũng thêm được
                    txtThueNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddThue.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    var ControlTextEdit = layout.GetControlByName("txtThueNew");
                    if(ControlTextEdit!= null)
                    {
                        ControlTextEdit.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddThueNew_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtThueNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtThueNew?.Tag as SearchLookUpEdit;

                if (txtThueNew != null && searchLookUpEdit != null)
                {
                    string tenThue = txtThueNew.Text.Trim();
                    string maThue = string.Empty;
                    if (string.IsNullOrWhiteSpace(tenThue))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên thuế!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtThueNew.Focus();
                        return;
                    }
                 
                    if (tblThuVienThue != null && tblThuVienThue.Rows.Count > 0)
                    {
                        var existingRow = tblThuVienThue.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("Thue")) &&
                                r.Field<string>("Thue").Trim().Equals(tenThue.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            SetUpColLoaiThue();
                            maThue = existingRow.Field<string>("MaThue");


                        }
                        else
                        {
                             maThue = LuuThueMoi(tenThue);
                        }
                    }


                    if (!string.IsNullOrEmpty(maThue))
                    {
                        int currentRowHandle = grvThue.FocusedRowHandle;

                        
                        

                        if (currentRowHandle >= 0)
                        {
                            grvThue.SetRowCellValue(currentRowHandle, colLoaiThue, maThue);
                        }

                        grcThue.RefreshDataSource();
                        UpdateTTThue4PhieuBG();
                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu thuế!", "Lỗi",
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

        private string LuuThueMoi(string tenThue)
        {
            try
            {
              

                // Bước 2: Gọi API lưu thuế mới
                string url = string.Format("{0}", URL + "DicThuVienBaoGia/PostThue");
                DataTable dtThue = new DataTable();
                dtThue.Columns.Add("ID", typeof(int));
                dtThue.Columns.Add("Thue", typeof(string));
                dtThue.Columns.Add("MaThue", typeof(string));
                dtThue.Columns.Add("NhomThue", typeof(string));
                dtThue.Columns.Add("GhiChu", typeof(string));

                DataRow row = dtThue.NewRow();
                row["ID"] = 0;
                row["Thue"] = tenThue.Trim();
                row["MaThue"] = "";
                row["NhomThue"] = "";
                row["GhiChu"] = "";
                dtThue.Rows.Add(row);
                string jsonData = JsonConvert.SerializeObject(dtThue);
                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, jsonData);
                }).Result;

                // Bước 3: Kiểm tra result
            

                if (result?.ToLower() == "true")
                {
                    SetUpColLoaiThue();


                    if (tblThuVienThue != null && tblThuVienThue.Rows.Count > 0)
                    {
                        var newRow = tblThuVienThue.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("Thue")) &&
                                r.Field<string>("Thue").Trim().Equals(tenThue.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            return newRow.Field<string>("MaThue");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu thuế: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

     
        private void ColumnCLVatTuChietKhauEdit_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            UpdateTTChietKhau4PhieuBG();
        }

        private void RItemCLVT_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            UpdateTTThue4PhieuBG();
          

        }
        private void UpdateTTThue4PhieuBG()
        {
            var view = grvThue;
            view.PostEditor();
            view.UpdateCurrentRow();
            //clear cache Thue
            for (int i = 0; i <= bgvVatTuPhieuBG_NPL.RowCount; i++)
            {
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTThue, "");
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colThueBG, 0);
            }

            for (int i = 0; i <= grvThue.RowCount; i++)
            {
                UpdateTongThueTheoCLVT(i);
            }
        }
        private void RItemLoaiThue_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;


            UpdateTTThue4PhieuBG();

        }

        private void UpdateTongThueTheoCLVT(int rowHandle)
        {
            if (rowHandle < 0) return;

            var view = grvThue;

            var maCLVT = view.GetRowCellValue(rowHandle, "MaCLVTID");
            if (maCLVT == null) return;
     
            decimal tong = 0;
            List<string> lstTTThue = new List<string>();
            string LoaiThue = string.Empty;
            for (int i = 0; i < view.DataRowCount; i++)
            {
                if (!view.IsDataRow(i)) continue;

                var clvt = view.GetRowCellValue(i, "MaCLVTID");
                LoaiThue = view.GetRowCellDisplayText(i, "MaThue")?.ToString();
                if (clvt == null || clvt.ToString() != maCLVT.ToString())
                    continue;
                if (string.IsNullOrEmpty(LoaiThue)) continue;
                decimal val = 0;

                var cell = view.GetRowCellValue(i, colSoThue);
                if (cell != null && decimal.TryParse(cell.ToString(), out decimal oldVal))
                    val = oldVal;

                tong += val;

                string currentLoaiThue = LoaiThue.Trim();

                if(currentLoaiThue != "[Chọn thuế]")
                {
                    lstTTThue = lstTTThue
                    .Where(x => !x.StartsWith(currentLoaiThue, StringComparison.OrdinalIgnoreCase))
                    .ToList();


                    lstTTThue.Add($"{currentLoaiThue} {val}%");
                }
                


            }


            var main = bgvVatTuPhieuBG_NPL;

            for (int i = 0; i < main.DataRowCount; i++)
            {
                if (!main.IsDataRow(i)) continue;

                var idMain = main.GetRowCellValue(i, "ChungLoaiCC");
                if (idMain != null && idMain.ToString() == maCLVT.ToString())
                {
                    main.SetRowCellValue(i, "Thue", tong);
                    main.SetRowCellValue(i, "TTThue", string.Join(";", lstTTThue.Distinct()));
                }

                main.UpdateCurrentRow();
            }
            bgvVatTuPhieuBG_NPL.RefreshData();
            bgvVatTuPhieuBG_NPL.UpdateSummary();

            grvChiPhi.RefreshData();
            grvChiPhi.UpdateSummary();
        }
        DataTable tblThuVienChietKhau = new DataTable();
        private void SetUpColLoaiChietKhau()
        {
            tblThuVienChietKhau = new DataTable();
            string url = string.Format("{0}?action={1}", URL + "PhieuBaoGia/Get", "GetChietKhau");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                tblThuVienChietKhau = JsonConvert.DeserializeObject<DataTable>(json);
            }
            RepositoryItemSearchLookUpEdit rItemEdit = new RepositoryItemSearchLookUpEdit();
            rItemEdit.DataSource = tblThuVienChietKhau;
            rItemEdit.DisplayMember = "ChietKhau";
            rItemEdit.ValueMember = "MaChietKhau";
            rItemEdit.ShowClearButton = false;
            rItemEdit.NullText = "[Chọn chiết khấu]";
            rItemEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rItemEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rItemEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaChietKhau", Caption = "MaChietKhau", Name = "rMaChietKhau", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "ChietKhau", Caption = "Chiết khấu", Name = "rTenChiPhi", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "GhiChu", Caption = "Nhóm Chi Phí", Name = "rGhiChuChietKhau", Visible = true });
            }

            colLoaiCK.ColumnEdit = rItemEdit;
            colLoaiCK.ColumnEdit.EditValueChanged += RChietKhauItemEdit_EditValueChanged;
            rItemEdit.Popup += new System.EventHandler(this.rItemEdit_ChietKhau_Popup);
        }

        // ============== CHIẾT KHẤU ==============
        private void rItemEdit_ChietKhau_Popup(object sender, EventArgs e)
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

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddChietKhauNew"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroupChietKhau",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    layout.Root.AddItem(buttonGroup);

                    var txtChietKhauNew = new TextEdit()
                    {
                        Name = "txtChietKhauNew",
                        Properties =
                {
                    NullText = "Nhập tên chiết khấu mới...",
                    NullValuePrompt = "Nhập tên chiết khấu mới..."
                }
                    };

                    txtChietKhauNew.Tag = ownerEdit; // Lưu SearchLookUpEdit

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtChietKhauNew,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    var btnAddChietKhau = new SimpleButton()
                    {
                        Name = "btnAddChietKhauNew",
                        Text = "Thêm chiết khấu"
                    };

                    btnAddChietKhau.Tag = txtChietKhauNew;
                    btnAddChietKhau.Click += btnAddChietKhau_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddChietKhau,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(120, 30),
                        MaxSize = new Size(120, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);

                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    txtChietKhauNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddChietKhau.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    // Nếu đã có controls, clear TextEdit
                    var controlTextEdit = layout?.Controls.OfType<TextEdit>().FirstOrDefault(c => c.Name == "txtChietKhauNew");
                    if (controlTextEdit != null)
                    {
                        controlTextEdit.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddChietKhau_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtChietKhauNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtChietKhauNew?.Tag as SearchLookUpEdit;

                if (txtChietKhauNew != null && searchLookUpEdit != null)
                {
                    string tenChietKhau = txtChietKhauNew.Text.Trim();
                    string maChietKhau = string.Empty;

                    if (string.IsNullOrWhiteSpace(tenChietKhau))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên chiết khấu!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtChietKhauNew.Focus();
                        return;
                    }

                  
                    if (tblThuVienChietKhau != null && tblThuVienChietKhau.Rows.Count > 0)
                    {
                        var existingRow = tblThuVienChietKhau.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("ChietKhau")) &&
                                r.Field<string>("ChietKhau").Trim().Equals(tenChietKhau.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            SetUpColLoaiChietKhau();
                            maChietKhau = existingRow.Field<string>("MaChietKhau");
                      
                        }
                        else
                        {
                           
                            maChietKhau = LuuChietKhauMoi(tenChietKhau);
                        }
                    }
                    else
                    {
                     
                        maChietKhau = LuuChietKhauMoi(tenChietKhau);
                    }

                    if (!string.IsNullOrEmpty(maChietKhau))
                    {
                        int currentRowHandle = grvChietKhau.FocusedRowHandle;

                       
                      
                        if (currentRowHandle >= 0)
                        {
                            grvChietKhau.SetRowCellValue(currentRowHandle, colLoaiCK, maChietKhau);
                            grvChietKhau.PostEditor();
                            grvChietKhau.UpdateCurrentRow();
                        }

                        grcChietKhau.RefreshDataSource();
                        UpdateTTChietKhau4PhieuBG();

                        // Đóng popup
                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu chiết khấu!", "Lỗi",
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

        private string LuuChietKhauMoi(string tenChietKhau)
        {
            try
            {
              
                string url = string.Format("{0}", URL + "DicThuVienBaoGia/PostChietKhau");
                DataTable dtChietKhau = new DataTable();
                dtChietKhau.Columns.Add("ID", typeof(int));
                dtChietKhau.Columns.Add("ChietKhau", typeof(string));
                dtChietKhau.Columns.Add("MaChietKhau", typeof(string));
                dtChietKhau.Columns.Add("GhiChu", typeof(string));

                DataRow row = dtChietKhau.NewRow();
                row["ID"] = 0;
                row["ChietKhau"] = tenChietKhau.Trim();
                row["MaChietKhau"] = "";
                row["GhiChu"] = "";
                dtChietKhau.Rows.Add(row);

                string jsonData = JsonConvert.SerializeObject(dtChietKhau);
                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, jsonData);
                }).Result;

             
                if (result?.ToLower() == "true")
                {
                    SetUpColLoaiChietKhau();
                    if (tblThuVienChietKhau != null && tblThuVienChietKhau.Rows.Count > 0)
                    {
                        var newRow = tblThuVienChietKhau.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("ChietKhau")) &&
                                r.Field<string>("ChietKhau").Trim().Equals(tenChietKhau.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            
                            return newRow.Field<string>("MaChietKhau");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu chiết khấu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void UpdateTTChietKhau4PhieuBG()
        {
            var view = grvChietKhau;
            view.PostEditor();
            view.UpdateCurrentRow();

          
            for (int i = 0; i < bgvVatTuPhieuBG_NPL.RowCount; i++)
            {
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTChietKhau, "");
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colChietKhauBG, 0);
            }

            for (int i = 0; i < view.RowCount; i++)
            {
                UpdateTongCheckKhauTheoCLVT(i);
            }
        }
        private void RChietKhauItemEdit_EditValueChanged(object sender, EventArgs e)
        {
            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;
            UpdateTTChietKhau4PhieuBG();

        }

        private void UpdateTongCheckKhauTheoCLVT(int rowHandle)
        {
            var view = grvChietKhau;
            if (rowHandle < 0 || !view.IsDataRow(rowHandle)) return;

            var maCLVT = view.GetRowCellValue(rowHandle, "MaCLVTID");

            if (maCLVT == null) return;

            decimal tong = 0;

            List<string> lstTTChietKhau = new List<string>();
            string LoaiChietKhau = string.Empty;
            for (int i = 0; i < view.DataRowCount; i++)
            {
                if (!view.IsDataRow(i)) continue;

                var clvt = view.GetRowCellValue(i, "MaCLVTID");
                LoaiChietKhau = view.GetRowCellDisplayText(i, "MaChietKhau")?.ToString();
                if (clvt == null || clvt.ToString() != maCLVT.ToString())
                    continue;

                decimal val = 0;

                var cell = view.GetRowCellValue(i, colChietKhau);
                if (cell != null && decimal.TryParse(cell.ToString(), out decimal oldVal))
                    val = oldVal;

                tong += val;

                string currentChietKhau = LoaiChietKhau.Trim();


                lstTTChietKhau = lstTTChietKhau
                    .Where(x => !x.StartsWith(currentChietKhau, StringComparison.OrdinalIgnoreCase))
                    .ToList();


                lstTTChietKhau.Add($"{currentChietKhau} {val}%");

            }


            var main = bgvVatTuPhieuBG_NPL;

            for (int i = 0; i < main.DataRowCount; i++)
            {
                if (!main.IsDataRow(i)) continue;

                var idMain = main.GetRowCellValue(i, "ChungLoaiCC");
                if (idMain != null && idMain.ToString() == maCLVT.ToString())
                {
                    main.SetRowCellValue(i, "ChietKhau", tong);
                    main.SetRowCellValue(i, "TTChietKhau", string.Join(";", lstTTChietKhau.Distinct()));
                }
            }

            main.UpdateCurrentRow();

            bgvVatTuPhieuBG_NPL.RefreshData();
            bgvVatTuPhieuBG_NPL.UpdateSummary();

            grvChiPhi.RefreshData();
            grvChiPhi.UpdateSummary();
        }

        private void SetUpColDonViTienTe()
        {
            DataTable tbl = new DataTable();
            string url = string.Format("{0}?action={1}", URL + "PhieuBaoGia/Get", "GetTienTe");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (!string.IsNullOrEmpty(json))
            {
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }
            RepositoryItemSearchLookUpEdit rItemEdit = new RepositoryItemSearchLookUpEdit();
            rItemEdit.DataSource = tbl;
            rItemEdit.DisplayMember = "MaTienTe";
            rItemEdit.ValueMember = "TienTeID";
            rItemEdit.ShowClearButton = false;
            rItemEdit.NullText = "[Chọn tiền tệ]";
            rItemEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rItemEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rItemEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TienTeID", Caption = "TienTeID", Name = "rTienTeID", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "MaTienTe", Caption = "Mã tiền tệ", Name = "rMaTienTe", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenTienTe", Caption = "Tiền tệ", Name = "rTenTienTe", Visible = true });
            }
            colTienTe.ColumnEdit = rItemEdit;
            colTienTe_CP.ColumnEdit = rItemEdit;

        }

        private void LoadPhieuBG()
        {
            try
            {
                string MaPhieu = _action == "copy" ? _phieuBGCopy : _maPhieuBG;
                string url = $"{URL}PhieuBaoGia/GET?action=GetPhieuBG&para1={MaPhieu}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblPhieuBaoGia = CreateDataTable_PhieuBaoGia_ChiTiet();
                if (json != "[]")
                {
                    _tblPhieuBaoGia = JsonConvert.DeserializeObject<DataTable>(json);
                }
                SetLayoutPhieuBG_Detail(_tblPhieuBaoGia);
                gcVatTuPhieuBG_NPL.DataSource = _tblPhieuBaoGia;
                bgvVatTuPhieuBG_NPL.ExpandAllGroups();
                gcVatTuPhieuBG_NPL.RefreshDataSource();
                SetCaptionGridView();
                //SetCaptionBGVatTu();
            }
            catch (Exception ex)
            {

            }
        }
        
        private void LoadChietKhau()
        {
            try
            {
                string MaPhieu = _action == "copy" ? _phieuBGCopy : _maPhieuBG;
                string url = $"{URL}PhieuBaoGia/GET?action=GetChietKhauBG&para1={MaPhieu}&para2={_action}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblSaveChietKhauBG = CreateTableSaveChietKhau();
                if (json != "[]")
                {
                    _tblSaveChietKhauBG = JsonConvert.DeserializeObject<DataTable>(json);
                }

                grcChietKhau.DataSource = _tblSaveChietKhauBG;
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadThueBG()
        {
            try
            {
                string MaPhieu = _action == "copy" ? _phieuBGCopy : _maPhieuBG;
                string url = $"{URL}PhieuBaoGia/GET?action=GetThueBG&para1={MaPhieu}&para2={_action}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblSaveThueBG = CreateTableSaveThue();
                if (json != "[]")
                {
                    _tblSaveThueBG = JsonConvert.DeserializeObject<DataTable>(json);
                }

                grcThue.DataSource = _tblSaveThueBG;
                grcThue.RefreshDataSource();
            }
            catch (Exception ex)
            {

            }
        }

        private void SetLayoutPhieuBG_Detail(DataTable tblChiTietPhieu)
        {


            if (tblChiTietPhieu?.Rows?.Count > 0)
            {


                //gridBand33.Visible = true;                          
                DataRow rowPhieu = tblChiTietPhieu.Rows[0];
                
                txtGhiChu.EditValue = rowPhieu["GhiChuPhieu"]?.ToString();

             
               
                txtSoPhieu.EditValue = _tenPhieu;
              
                SearchLookupDVTienTe.EditValue = rowPhieu["TienTeID"]?.ToString();
                SearchLookupNhaCC.EditValue =  rowPhieu["MaNCC"]?.ToString();
                SearchLookUpLoaiCC.EditValue = rowPhieu["NhomCLCC"]?.ToString();
                SearchLookupPTThanhToan.EditValue = rowPhieu["PTThanhToan"]?.ToString();
                _TienTe = rowPhieu["TienTeID"]?.ToString();
                TienTeID_QD = rowPhieu["TienTeID"]?.ToString();
                
             
                if (_action == "edit")
                {
                    int.TryParse(rowPhieu["SoNgayGHSom"]?.ToString(), out int SoNgayGHSom);
                    int.TryParse(rowPhieu["SoNgayGHTre"]?.ToString(), out int SoNgayGHTre);
                    int.TryParse(rowPhieu["SoNgayHieuLuc"]?.ToString(), out int SoNgayHieuLuc);
                    //double.TryParse(rowPhieu["ChietKhau"]?.ToString(), out double ChietKhau);
                    decimal.TryParse(rowPhieu["HinhThucThanhToan"]?.ToString(), out decimal HinhThucThanhToan);
                    spinMinGH.EditValue = SoNgayGHSom;
                    spinMaxGH.EditValue = SoNgayGHTre;
                    DateTime ngayBDHieuLuc = clsForrmatUtils.ConvertDate(rowPhieu["NgayBDHieuLuc"]?.ToString());
                    DateTime ngayHetHieuLuc = clsForrmatUtils.ConvertDate(rowPhieu["NgayHetHieuLuc"]?.ToString());
                    spinHThucThanhToan.EditValue = HinhThucThanhToan;
                    DateEditBDHieuLuc.EditValue = _action == "copy" ? ngayHetHieuLuc.AddDays(1) : ngayBDHieuLuc;

                    if (rowPhieu["DonViTG_HieuLuc"]?.ToString()?.ToLower() == "tháng")
                    {
                        SoNgayHieuLuc = SoNgayHieuLuc / 30;
                    }
                    else if (rowPhieu["DonViTG_HieuLuc"]?.ToString()?.ToLower() == "năm")
                    {
                        SoNgayHieuLuc = SoNgayHieuLuc / 365;
                    }
                    cbxDonViTG.EditValue = rowPhieu["DonViTG_HieuLuc"]?.ToString();
                    spinSoThoiHanHL.EditValue = SoNgayHieuLuc;

                    SearchLookUpLoaiCC.ReadOnly = true;
                    SearchLookupNhaCC.ReadOnly = true;
                    DateEditBDHieuLuc.ReadOnly = true;
                  
                }
                if(_action == "copy")
                {
                    GenerateMaPhieu();
                    LoadTenPhieu();
                }
                
                if (_isDuyet)
                {
                    btnDeleteVTBaoGia.Enabled = false;
                    btnLuuPhieuBG.Enabled = false;
                    btnChonVatTu.Enabled = false;
                    spinSoThoiHanHL.ReadOnly = true;
                    cbxDonViTG.ReadOnly = true;
                    txtGhiChu.ReadOnly = true;
                    SearchLookUpLoaiCC.ReadOnly = true;
                    SearchLookupNhaCC.ReadOnly = true;
                    spinMinGH.ReadOnly = true;
                    spinMaxGH.ReadOnly = true;


                    SearchLookupPTThanhToan.ReadOnly = true;
                    grvChietKhau.OptionsBehavior.ReadOnly = true;
                    grvChiPhi.OptionsBehavior.ReadOnly = true;
                    grvChietKhau.OptionsBehavior.ReadOnly = true;
                    grvChietKhau.OptionsBehavior.Editable = false;
                    grvChiPhi.OptionsBehavior.Editable = false;
                    grvThue.OptionsBehavior.Editable = false;

                    btnAddCPVatTu.Enabled = false;
                    btnXoaCPVatTu.Enabled = false;

                    btnAddThue.Enabled = false;
                    btnDeleteThue.Enabled = false;

                    btnAddCKhau.Enabled = false;
                    btnXoaCKhau.Enabled = false;

                    //SearchLookupDVTienTe.Enabled = false;
                }


            }
    
            else
            {
                //searchLookUpEditPhieuBG.EditValue = null;
                //gridBand33.Visible = false;
                txtGhiChu.EditValue = "";
                spinMaxGH.EditValue = 0;
                spinMinGH.EditValue = 0;
                spinSoThoiHanHL.EditValue = 0;
                DateEditBDHieuLuc.EditValue = DateTime.Now;
                SearchLookupDVTienTe.EditValue = null;
                SearchLookupNhaCC.EditValue = null;
                SearchLookUpLoaiCC.EditValue = null;
                SearchLookupPTThanhToan.EditValue = null;
                cbxDonViTG.SelectedIndex = 0;
            }
        }

        private void LoadChiPhiBG()
        {
            try
            {
                string MaPhieu = _action == "copy" ? _phieuBGCopy : _maPhieuBG;
                string url = $"{URL}PhieuBaoGia/GET?action=GetChiPhiBG&para1={MaPhieu}&para2={_action}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblSaveChiPhiBG = CreateTableSaveChiPhi();
                if (json != "[]")
                {
                    _tblSaveChiPhiBG = JsonConvert.DeserializeObject<DataTable>(json);
                }

                grcChiPhi.DataSource = _tblSaveChiPhiBG;
            }
            catch (Exception ex)
            {

            }
        }

        private void bgvVatTuPhieuBG_NPL_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            if (_isDuyet) return;
            int rowHandle = bgvVatTuPhieuBG_NPL.FocusedRowHandle;


            if (bgvVatTuPhieuBG_NPL.IsGroupRow(rowHandle))
            {
                rowHandle = bgvVatTuPhieuBG_NPL.GetDataRowHandleByGroupRowHandle(rowHandle);
            }


            if (rowHandle < 0)
            {

                return;
            }

            DataRow rowFocused = bgvVatTuPhieuBG_NPL.GetDataRow(rowHandle);
            if (rowFocused == null)
            {
                return;
            }
            // Chỉ hiện menu khi chuột phải trên cell của cột TyLeMuaThem
            if (e.HitInfo.InRowCell && e.HitInfo.Column != null && (e.HitInfo.Column.FieldName == "DonGia" || e.HitInfo.Column.FieldName == "Thue" || e.HitInfo.Column.FieldName == "ChietKhau"))
            {
                e.Menu.Items.Clear(); // bỏ menu mặc định nếu muốn

                // Fill tất cả
                var fillAllItem = new DXMenuItem("Áp dụng toàn cột", (o, args) =>
                {
                    rowHandle = e.HitInfo.RowHandle;
                    var col = e.HitInfo.Column;
                    object value = view.GetRowCellValue(rowHandle, col);

                    view.BeginUpdate();
                    try
                    {
                        for (int i = 0; i < view.DataRowCount; i++)
                        {
                            string ID = view.GetRowCellValue(i, colID)?.ToString();
                            int rh = view.GetRowHandle(i);
                            if(ID == "0")
                            {
                                view.SetRowCellValue(rh, col, value);
                            }
                           
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                // Fill group
                var fillGroupItem = new DXMenuItem("Áp dụng theo nhóm", (o, args) =>
                {
                     rowHandle = e.HitInfo.RowHandle;
                    var col = e.HitInfo.Column;
                    object value = view.GetRowCellValue(rowHandle, col);

                    // Xác định group row của row hiện tại
                    int groupRowHandle = view.GetParentRowHandle(rowHandle);

                    view.BeginUpdate();
                    try
                    {
                        // Duyệt tất cả row trong group đó
                        for (int i = 0; i < view.GetChildRowCount(groupRowHandle); i++)
                        {
                            int childHandle = view.GetChildRowHandle(groupRowHandle, i);
                            if (view.IsDataRow(childHandle))
                            {
                                string ID = view.GetRowCellValue(childHandle, colID)?.ToString();
                                if (ID == "0")
                                {
                                    view.SetRowCellValue(childHandle, col, value);
                                }
                            }
                               
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                e.Menu.Items.Add(fillAllItem);
                e.Menu.Items.Add(fillGroupItem);
              
            
            }

            if (_action == "edit" && rowFocused["ID"]?.ToString() != "0")
            {
                var itemDieuChinhGia = new DXMenuItem("Điều chỉnh đơn giá");
                itemDieuChinhGia.Click += (s, args) =>
                {

                    rowHandle = e.HitInfo.RowHandle;
                    if (rowHandle < 0) return;

                    var row = view.GetRow(rowHandle);   

                    DataRow rowFocsed = bgvVatTuPhieuBG_NPL.GetFocusedDataRow() as DataRow;
                    if (rowFocsed != null)
                    {
                        string MaPhieu = _maPhieuBG;
                        if (string.IsNullOrEmpty(MaPhieu)) return;
                        frmPhieuBGUpdateDonGia frm = new frmPhieuBGUpdateDonGia(rowFocsed, MaPhieu, _tenPhieu);

                        frm.Show();
                        LoadLoaiVatTu();
                        LoadTienTe();
                        LoadPTThanhToan();
                        SetupGridView();
                        SetupLayout();
                    }
                };


                e.Menu.Items.Add(itemDieuChinhGia);
            }
        }

        private void grvChiPhi_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            if (e.Column == colTongChiPhi && e.IsGetData)
            {
                decimal chiPhi = 0;
                decimal thueCP = 0;

                object valChiPhi = view.GetListSourceRowCellValue(e.ListSourceRowIndex, colChiPhi_CP);
                object valThueCP = view.GetListSourceRowCellValue(e.ListSourceRowIndex, colThueCP);
                object valTienTe = view.GetListSourceRowCellValue(e.ListSourceRowIndex, colTienTe_CP);

                if (!string.IsNullOrEmpty(valTienTe?.ToString()))
                {
                  

                    if (valChiPhi != null && valChiPhi != DBNull.Value)
                        decimal.TryParse(valChiPhi.ToString(), out chiPhi);

                    if (valThueCP != null && valThueCP != DBNull.Value)
                        decimal.TryParse(valThueCP.ToString(), out thueCP);



                    decimal tongChiPhi = chiPhi + (chiPhi * thueCP / 100);
                    e.Value = QuyDoiDonGia(valTienTe.ToString(), tongChiPhi);
                   
                }
                
            }
        }

        private void grvChiPhi_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            view.RefreshData();
            decimal tongChiPhi = 0;

            var summaryValue = view.Columns["TotalCP_BG"].SummaryItem.SummaryValue;

            if (summaryValue != null && summaryValue != DBNull.Value)
                decimal.TryParse(summaryValue.ToString(), out tongChiPhi);

            txtChiPhatSinh.Text = tongChiPhi.ToString("n2");
        }

        private void grvChiPhi_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;


            if (e.Column.FieldName == "ChiPhi" || e.Column.FieldName == "Thue" || e.Column == colTienTe_CP)
            {
                string MaChiPhi = grvChiPhi.GetFocusedRowCellValue(colTenChiPhi)?.ToString();
              
                if (string.IsNullOrEmpty(MaChiPhi))
                {
                    //MessageBox.Show("Bạn phải chọn dữ liệu cho Mã Chi Phí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                decimal tongChiPhi = 0;
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    object val = view.GetRowCellValue(i, colTongChiPhi);
                    tongChiPhi += GetDecimalValue(val);

                }
                txtChiPhatSinh.Text = Math.Round(tongChiPhi, 2).ToString();

            }
        }

        private decimal GetDecimalValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return decimal.TryParse(value.ToString(), out var result) ? result : 0;
        }



        //private void grvChiPhi_CellValueChanged(object sender, CellValueChangedEventArgs e)
        //{
        //    GridView view = sender as GridView;
        //    if (view == null) return;

        //    if (e.Column.FieldName == "ChiPhi" || e.Column.FieldName == "Thue")
        //    {

        //        //if (view.IsEditing) return; !

        //        string MaChiPhi = view.GetFocusedRowCellValue(colTenChiPhi)?.ToString();
        //        if (string.IsNullOrEmpty(MaChiPhi)) return;

        //        decimal tongChiPhi = 0;
        //        for (int i = 0; i < view.DataRowCount; i++)
        //        {
        //            object val = view.GetRowCellValue(i, colTongChiPhi);
        //            if (val != null && val != DBNull.Value)
        //            {
        //                decimal chiPhi = GetDecimalSafe(view, i, colChiPhi_CP);
        //                decimal thueCP = GetDecimalSafe(view,i, colThueCP);


        //             tongChiPhi += Math.Round(chiPhi + (chiPhi * thueCP) / 100, 2);
        //            }
        //        }

        //        txtChiPhatSinh.Text = tongChiPhi.ToString("N2");
        //    }
        //}


        private void grvThue_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;


            if (e.Column != colSoThue)
                return;


            var maCLVT = view.GetRowCellValue(e.RowHandle, "MaCLVTID");
            if (maCLVT == null) return;

            decimal tong = 0;
            List<string> lstTTThue = new List<string>();
            string LoaiThue = string.Empty;
            for (int i = 0; i < view.DataRowCount; i++)
            {
                if (!view.IsDataRow(i)) continue;

                var clvt = view.GetRowCellValue(i, "MaCLVTID");
                LoaiThue = view.GetRowCellDisplayText(i, "MaThue")?.ToString();
                if (clvt == null || clvt.ToString() != maCLVT.ToString())
                    continue;
                if (string.IsNullOrEmpty(LoaiThue)) continue;
                decimal val = 0;

                var cell = view.GetRowCellValue(i, colSoThue);
                if (cell != null && decimal.TryParse(cell.ToString(), out decimal oldVal))
                    val = oldVal;

                tong += val;

                string currentLoaiThue = LoaiThue.Trim();


                lstTTThue = lstTTThue
                    .Where(x => !x.StartsWith(currentLoaiThue, StringComparison.OrdinalIgnoreCase))
                    .ToList();


                lstTTThue.Add($"{currentLoaiThue} {val}%");


            }


            var main = bgvVatTuPhieuBG_NPL;

            for (int i = 0; i < main.DataRowCount; i++)
            {
                if (!main.IsDataRow(i)) continue;

                var idMain = main.GetRowCellValue(i, "ChungLoaiCC");
                if (idMain != null && idMain.ToString() == maCLVT.ToString())
                {
                    main.SetRowCellValue(i, "Thue", tong);
                    main.SetRowCellValue(i, "TTThue", string.Join(";", lstTTThue.Distinct()));
                }

                main.UpdateCurrentRow();
            }

            bgvVatTuPhieuBG_NPL.RefreshData();
            bgvVatTuPhieuBG_NPL.UpdateSummary();
        }

        private void grvChietKhau_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

         
            if (e.Column != colChietKhau)
                return;

           
            var maCLVT = view.GetRowCellValue(e.RowHandle, "MaCLVTID");
        
            if (maCLVT == null) return;

            decimal tong = 0;

            List<string> lstTTChietKhau = new List<string>();
            string LoaiChietKhau = string.Empty;
            for (int i = 0; i < view.DataRowCount; i++)
            {
                if (!view.IsDataRow(i)) continue;

                var clvt = view.GetRowCellValue(i, "MaCLVTID");
                LoaiChietKhau = view.GetRowCellDisplayText(i, "MaChietKhau")?.ToString();
                if (clvt == null || clvt.ToString() != maCLVT.ToString())
                    continue;

                decimal val = 0;

                var cell = view.GetRowCellValue(i, colChietKhau);
                if (cell != null && decimal.TryParse(cell.ToString(), out decimal oldVal))
                    val = oldVal;

                tong += val;

                string currentChietKhau = LoaiChietKhau.Trim();


                lstTTChietKhau = lstTTChietKhau
                    .Where(x => !x.StartsWith(currentChietKhau, StringComparison.OrdinalIgnoreCase))
                    .ToList();


                lstTTChietKhau.Add($"{currentChietKhau} {val}%");

            }


            var main = bgvVatTuPhieuBG_NPL;

            for (int i = 0; i < main.DataRowCount; i++)
            {
                if (!main.IsDataRow(i)) continue;

                var idMain = main.GetRowCellValue(i, "ChungLoaiCC");
                if (idMain != null && idMain.ToString() == maCLVT.ToString())
                {
                    main.SetRowCellValue(i, "ChietKhau", tong);
                    main.SetRowCellValue(i, "TTChietKhau", string.Join(";", lstTTChietKhau.Distinct()));
                }
            }

            main.UpdateCurrentRow();

            bgvVatTuPhieuBG_NPL.RefreshData();
            bgvVatTuPhieuBG_NPL.UpdateSummary();

            grvChiPhi.RefreshData();
            grvChiPhi.UpdateSummary();
        }

        #region Enven Group Danh Mục CP
        private void btnAddCPVatTu_Click(object sender, EventArgs e)
        {
            if (_tblSaveChiPhiBG == null || _tblSaveChiPhiBG?.Rows?.Count == 0)
            {
                _tblSaveChiPhiBG = CreateTableSaveChiPhi();
                LoadChiPhiBG();
            }
            DataRow newRow = _tblSaveChiPhiBG.NewRow();
            newRow["ID"] = 0;
            newRow["MaPhieuBG"] = _maPhieuBG;
            //newRow["MaChiPhi"] = string.Empty;
            newRow["ChiPhi"] = 0.0;
            newRow["Thue"] = 0.0;
            newRow["GhiChu"] = string.Empty;
            newRow["DonViTienTe"] = TienTeDeault;
            _tblSaveChiPhiBG.Rows.InsertAt(newRow, 0);
            grcChiPhi.DataSource = _tblSaveChiPhiBG;

        }

        private void btnXoaCPVatTu_Click(object sender, EventArgs e)
        {
            try
            {

                if (_tblSaveChiPhiBG == null || _tblSaveChiPhiBG?.Rows?.Count == 0) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = grvChiPhi.GetSelectedRows();

                // Duyệt ngược để tránh xung đột handle khi xóa
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = grvChiPhi.GetDataRow(rowHandle);
                    if (dr == null) continue;

                    if (dr["ID"].ToString() != "0")
                    {
                        DataTable tbl = new DataTable();
                        string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={_maPhieuBG}";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                        if (json != "[]")
                        {
                            tbl = JsonConvert.DeserializeObject<DataTable>(json);
                            if (tbl?.Rows?.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                                {
                                    XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                        }

                        string url = $"{URL}PhieuBaoGia/Delete?action=DeleteChiPhiBG&Para1={dr["ID"]}&Para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }


                    _tblSaveChiPhiBG.Rows.Remove(dr);
                }
                grcChiPhi.DataSource = _tblSaveChiPhiBG;
                grcChiPhi.RefreshDataSource();

                grvChiPhi.RefreshData();
                grvChiPhi.UpdateSummary();
                decimal tongChiPhi = 0;

                var summaryValue = grvChiPhi.Columns["TotalCP_BG"].SummaryItem.SummaryValue;

                if (summaryValue != null && summaryValue != DBNull.Value)
                    decimal.TryParse(summaryValue.ToString(), out tongChiPhi);

                txtChiPhatSinh.Text = tongChiPhi.ToString("n2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddThue_Click(object sender, EventArgs e)
        {
            if (_tblSaveThueBG == null || _tblSaveThueBG?.Rows?.Count == 0)
            {
                _tblSaveThueBG = CreateTableSaveThue();

            }
            DataRow newRow = _tblSaveThueBG.NewRow();
            newRow["ID"] = 0;
            newRow["MaPhieuBG"] = _maPhieuBG;
            //newRow["MaThue"] = null;
            //newRow["MaCLVTID"] = null;
            newRow["Thue"] = 0.0;
            newRow["GhiChu"] = string.Empty;
            _tblSaveThueBG.Rows.InsertAt(newRow, 0);
            grcThue.DataSource = _tblSaveThueBG;
        }

        private void btnDeleteThue_Click(object sender, EventArgs e)
        {
            try
            {

                if (_tblSaveThueBG == null) return;
                int rowHandle = grvThue.FocusedRowHandle;
                DataRow dr = grvThue.GetDataRow(rowHandle);
                if (dr == null) return;
                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                         
                if (dr["ID"].ToString() != "0")
                {
                    DataTable tbl = new DataTable();
                    string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={_maPhieuBG}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                    if (json != "[]")
                    {
                        tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        if (tbl?.Rows?.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                            {
                                XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                    }

                    string url = $"{URL}PhieuBaoGia/Delete?action=DeleteThueBG&Para1={dr["ID"]}&Para2=None";
                    string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                    }
                }


                _tblSaveThueBG.Rows.Remove(dr);

                //clear cache Thue
                for (int i = 0; i <= bgvVatTuPhieuBG_NPL.RowCount; i++)
                {
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTThue, "");
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colThueBG, 0);
                }

                grcThue.DataSource = _tblSaveThueBG;
                grcThue.RefreshDataSource();

                for (int i = 0; i <= grvChietKhau.RowCount; i++)
                {
                    UpdateTongThueTheoCLVT(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddCKhau_Click(object sender, EventArgs e)
        {
            if (_tblSaveChietKhauBG == null || _tblSaveChietKhauBG?.Rows?.Count == 0)
            {
                _tblSaveChietKhauBG = CreateTableSaveChietKhau();

            }
            DataRow newRow = _tblSaveChietKhauBG.NewRow();
            newRow["ID"] = 0;
            newRow["MaPhieuBG"] = _maPhieuBG;
            newRow["MaChietKhau"] = null;
            newRow["MaCLVTID"] = null;
            newRow["ChietKhau"] = 0.0;
            newRow["GhiChu"] = string.Empty;
            _tblSaveChietKhauBG.Rows.InsertAt(newRow, 0);
            grcChietKhau.DataSource = _tblSaveChietKhauBG;
        }
        private void btnXoaCKhau_Click(object sender, EventArgs e)
        {
            try
            {

              
                if (_tblSaveChietKhauBG == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;
                int rowHandle = grvChietKhau.FocusedRowHandle;
                DataRow dr = grvChietKhau.GetDataRow(rowHandle);
                if (dr == null) return;

                if (dr["ID"].ToString() != "0")
                {
                    DataTable tbl = new DataTable();
                    string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={_maPhieuBG}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                    if (json != "[]")
                    {
                        tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        if (tbl?.Rows?.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                            {
                                XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                    }

                    string url = $"{URL}PhieuBaoGia/Delete?action=DeleteChietKhauBG&Para1={dr["ID"]}&Para2=None";
                    string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                    }
                }


                if (_tblSaveChietKhauBG.Rows.Cast<DataRow>().Any(r => r == dr))
                {
                    _tblSaveChietKhauBG.Rows.Remove(dr);
                }


                grcChietKhau.DataSource = _tblSaveChietKhauBG;
                grcChietKhau.RefreshDataSource();

                //clear cache ChietKhau
                for (int i = 0; i <= bgvVatTuPhieuBG_NPL.RowCount; i++)
                {
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTChietKhau, "");
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colChietKhauBG, 0);
                }

                       
                for (int i = 0; i <= grvChietKhau.RowCount; i++)
                {
                    UpdateTongCheckKhauTheoCLVT(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void btnLuuPhieuBG_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = simpleButton1;

                string Action = "EditPhieu";
                if(_action == "add" || _action == "copy")
                {
                    Action = "NewPhieu";
                }

                string url = $"{URL}PhieuBaoGia/POSTPhieuBG?action=POST";

                DataTable tblHienThi = gcVatTuPhieuBG_NPL.DataSource as DataTable;
                if (tblHienThi == null || tblHienThi.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Chưa có dữ liệu chi tiết vật tư để lưu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string maLoaiCC = SearchLookUpLoaiCC.EditValue?.ToString();
                string maNCC = SearchLookupNhaCC.EditValue?.ToString();
                if (string.IsNullOrEmpty(maLoaiCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                DataSet dsSave = new DataSet();
                string maPhieuBG = _maPhieuBG;
                DataTable tblPhieu = CreateTableSavePhieuBG();
                DataRow rowPhieu = tblPhieu.NewRow();
                rowPhieu["ID"] = 0;
                rowPhieu["Action"] = Action;
                rowPhieu["MaPhieuBG"] = !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null;
                rowPhieu["TenPhieu"] = txtSoPhieu.Text.Trim();
                rowPhieu["NhomCLCC"] = maLoaiCC ?? "";
                rowPhieu["MaNCC"] = maNCC ?? "";
                rowPhieu["IsDuyet"] = false;
                rowPhieu["NgayDuyet"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                DateTime ngayBDHieuLuc = DateEditBDHieuLuc.EditValue == null ? DateTime.MinValue :(DateTime)DateEditBDHieuLuc.EditValue;
                if (ngayBDHieuLuc == DateTime.MinValue)
                {
                    XtraMessageBox.Show("Vui lòng chọn ngày bắt đầu hiệu lực", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                rowPhieu["NgayBDHieuLuc"] = ngayBDHieuLuc.ToString("yyyy-MM-ddTHH:mm:ss");
                ///*Check ngày bắt đầu hiệu lực*/
                if (ValidateAndCleanData())
                {
                    return;
                }


                if (int.TryParse(spinSoThoiHanHL.EditValue?.ToString(), out int SoTGHieuLuc)) ;
                if (string.IsNullOrEmpty(cbxDonViTG.EditValue?.ToString()) || SoTGHieuLuc <= 0)
                {
                    XtraMessageBox.Show("Vui lòng nhập thời hạn hiệu lực", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cbxDonViTG.EditValue?.ToString()?.ToLower() == "tháng")
                {
                    SoTGHieuLuc = SoTGHieuLuc * 30;
                }
                else if (cbxDonViTG.EditValue?.ToString()?.ToLower() == "năm")
                {
                    SoTGHieuLuc = SoTGHieuLuc * 365;
                }
                rowPhieu["SoNgayHieuLuc"] = SoTGHieuLuc;

                // DateTime ngayHieuLuc = clsForrmatUtils.ConvertDate(NgayHieuLuc.EditValue);
                if (string.IsNullOrEmpty(spinMinGH?.EditValue?.ToString()) || !int.TryParse(spinMinGH?.EditValue?.ToString(), out int SoTGGiaoSom))
                {
                    XtraMessageBox.Show("Vui lòng nhập khoảng thời gian giao dự kiến", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(spinMaxGH?.EditValue?.ToString()) || !int.TryParse(spinMaxGH?.EditValue?.ToString(), out int SoTGGiaoTre))
                {
                    XtraMessageBox.Show("Vui lòng nhập khoảng thời gian giao dự kiến", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (SoTGGiaoSom < 0)
                {
                    XtraMessageBox.Show("Vui lòng nhập khoảng thời gian giao dự kiến lớn hơn không", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (SoTGGiaoTre < 1)
                {
                    XtraMessageBox.Show("Vui lòng nhập khoảng thời gian giao dự kiến", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (SoTGGiaoTre < SoTGGiaoSom)
                {
                    XtraMessageBox.Show("Vui lòng nhập khoảng thời gian giao dự kiến sớm nhỏ nhất hơn thời gian giao dự kiến trễ nhất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                rowPhieu["GhiChu"] = txtGhiChu.EditValue;
                rowPhieu["SoNgayGHSom"] = SoTGGiaoSom;
                rowPhieu["SoNgayGHTre"] = SoTGGiaoTre;
                rowPhieu["NgayTao"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                rowPhieu["NguoiTao"] = GlobleData.UserName;
                rowPhieu["NgaySua"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                rowPhieu["NguoiSua"] = GlobleData.UserName;
                //rowPhieu["MaDVTTe"] = SearchLookupDVTienTe.EditValue;
                rowPhieu["MaDVTTe"] = tblHienThi != null && tblHienThi?.Rows?.Count > 0 ? tblHienThi?.Rows[0]["DVTienTeVT"] :TienTeDeault;
                decimal.TryParse(spinHThucThanhToan?.EditValue?.ToString(), out decimal HTThanhToan);
                rowPhieu["HinhThucThanhToan"] = HTThanhToan;
                rowPhieu["PTThanhToan"] = SearchLookupPTThanhToan.EditValue;
                rowPhieu["PTVanChuyen"] = null;
                rowPhieu["DonViTG_HieuLuc"] = cbxDonViTG.EditValue;
                tblPhieu.Rows.Add(rowPhieu);
                dsSave.Tables.Add(tblPhieu);
                DataTable tblChiTiet = CreateTableSaveBaoGiaChiTiet();
                foreach (DataRow row in tblHienThi.Rows)
                {
                    decimal.TryParse(row["DonGia"]?.ToString(), out decimal DonGia);
                    if (DonGia < 0)
                    {
                        XtraMessageBox.Show("Vui lòng nhập đơn giá lớn hơn hoặc bằng 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }


                    decimal thue = 0;
                    decimal chietkhau = 0;
                    if(row["Thue"] != DBNull.Value)
                    {
                        decimal.TryParse(row["Thue"]?.ToString(), out thue);
                    }
                    if (row["ChietKhau"] != DBNull.Value)
                    {
                        decimal.TryParse(row["ChietKhau"]?.ToString(), out chietkhau);
                    }
                   
                    DataRow newRow = tblChiTiet.NewRow();
                    newRow["ID"] = row["ID"] == DBNull.Value || Convert.ToInt64(row["ID"]) <= 0 ? 0 : row["ID"];
                    newRow["MaPhieuBG"] = !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null;
                    newRow["NhomCLCC"] = maLoaiCC;
                    newRow["MaNCC"] = maNCC;
                    newRow["ChungLoaiCC"] = row["ChungLoaiCC"]?.ToString() ?? "";
                    newRow["ItemCode"] = row["ItemCode"]?.ToString();
                    newRow["MaVTID"] = row["MaVTID"]?.ToString();
                    newRow["MauVTID"] = row["MauVTID"]?.ToString();
                    newRow["KhoSizeID"] = row["KhoSizeID"]?.ToString();
                    newRow["MaDVVT"] = row["MaDVVT"]?.ToString();
                    newRow["DonGia"] = row["DonGia"] == DBNull.Value ? 0.0 : Convert.ToDouble(row["DonGia"]);
                    newRow["Thue"] = thue;
                    newRow["ChietKhau"] = chietkhau;
                    newRow["GhiChu"] = row["GhiChu"]?.ToString();
                    newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    newRow["NguoiSua"] = GlobleData.UserName;
                    newRow["ThoiGianSua"] = clsForrmatUtils.ParseMinutesFromTime(DateTime.Now.TimeOfDay.ToString());
                    newRow["DonViTienTe"] = row["DVTienTeVT"]?.ToString();
                    newRow["Leadtime"] = row["LeadtimeVatu"]?.ToString();
                    tblChiTiet.Rows.Add(newRow);
                }
                dsSave.Tables.Add(tblChiTiet);
                dsSave.Tables[0].TableName = "PhieuBaoGia";
                dsSave.Tables[1].TableName = "BaoGia_ChiTiet";


                string jsonData = JsonConvert.SerializeObject(dsSave);
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, jsonData); }).Result;
                if (result.Contains("PBG"))
                {
                     
                    _maPhieuBG = result;
                    result = "True";
                }
                string urlPostChiPhi = $"{URL}PhieuBaoGia/POST?action=PostChiPhi";
                _tblSaveChiPhiBG = CreateTableSaveChiPhi();
                DataTable tblChiphi = grcChiPhi.DataSource as DataTable;
                if (tblChiphi != null && tblChiphi?.Rows?.Count > 0)
                {
                    int rowHandel = 0;
                    foreach (DataRow rowCP in tblChiphi?.Rows)
                    {
                        if (string.IsNullOrEmpty(rowCP["MaChiPhi"]?.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng nhập loại chi phí", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        bool IsDulicate = _tblSaveChiPhiBG.AsEnumerable().Any(x => x["MaChiPhi"]?.ToString() == rowCP["MaChiPhi"]);
                        if (IsDulicate)
                        {
                            XtraMessageBox.Show($"Chi phí {grvChiPhi.GetRowCellDisplayText(rowHandel, colTenChiPhi)} đã có !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        DataRow row = _tblSaveChiPhiBG.NewRow();
                        row["ID"] = rowCP["ID"];
                        row["Thue"] = rowCP["Thue"];
                        row["GhiChu"] = rowCP["GhiChu"];
                        row["ChiPhi"] = rowCP["ChiPhi"];
                        row["MaPhieuBG"] = maPhieuBG;
                        row["MaChiPhi"] = rowCP["MaChiPhi"];
                        row["DonViTienTe"] = rowCP["DonViTienTe"];
                        _tblSaveChiPhiBG.Rows.Add(row);
                        
                    }
                    string jsonDataPostChiPhi = JsonConvert.SerializeObject(_tblSaveChiPhiBG);
                    string resultPostChiPhi = Task.Run(async () => { return await _clientExtension.PostAsync(urlPostChiPhi, jsonDataPostChiPhi); }).Result;
                }

                string urlPostChietKhauBG = $"{URL}PhieuBaoGia/POST?action=PostChietKhauBG";
                DataTable tblChietKhau = grcChietKhau.DataSource as DataTable;
                _tblSaveChietKhauBG = CreateTableSaveChietKhau();
                if (tblChietKhau != null && tblChietKhau?.Rows?.Count > 0)
                {
                    int rowHandel = 0;
                    foreach (DataRow rowCK in tblChietKhau?.Rows)
                    {
                        if (string.IsNullOrEmpty(rowCK["MaChietKhau"]?.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng nhập loại chiết khấu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (string.IsNullOrEmpty(rowCK["MaCLVTID"]?.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng nhập chủng loại vật tư cho chiết khấu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        bool IsDulicate = _tblSaveChietKhauBG.AsEnumerable().Any(x => x["MaChietKhau"]?.ToString() == rowCK["MaChietKhau"]?.ToString()
                        && x["MaCLVTID"]?.ToString() == rowCK["MaCLVTID"]?.ToString());
                        if (IsDulicate)
                        {
                            XtraMessageBox.Show($"Chiết khấu {grvChietKhau.GetRowCellDisplayText(rowHandel, colLoaiCK)} và Chủng loại" +
                                $" vật tư {grvChietKhau.GetRowCellDisplayText(rowHandel, colNhomVT_CK)} đã có !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        DataRow row = _tblSaveChietKhauBG.NewRow();
                        row["ID"] = rowCK["ID"];
                        row["ChietKhau"] = rowCK["ChietKhau"];
                        row["GhiChu"] = rowCK["GhiChu"];
                        row["MaCLVTID"] = rowCK["MaCLVTID"];
                        row["MaPhieuBG"] = maPhieuBG;
                        row["MaChietKhau"] = rowCK["MaChietKhau"];
                        _tblSaveChietKhauBG.Rows.Add(row);
                        rowHandel++;
                    }

                    string jsonDataPostChietKhauBG = JsonConvert.SerializeObject(_tblSaveChietKhauBG);
                    string resultPostChietKhauBG = Task.Run(async () => { return await _clientExtension.PostAsync(urlPostChietKhauBG, jsonDataPostChietKhauBG); }).Result;
                }

                string urlPostThue = $"{URL}PhieuBaoGia/POST?action=PostThue";
                _tblSaveThueBG = CreateTableSaveThue();
                DataTable tblThue = grcThue.DataSource as DataTable;
                if (tblThue != null && tblThue.Rows?.Count > 0)
                {
                    int rowHandel = 0;
                    foreach (DataRow rowThue in tblThue?.Rows)
                    {

                        if (string.IsNullOrEmpty(rowThue["MaThue"]?.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng nhập loại thuế", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (string.IsNullOrEmpty(rowThue["MaCLVTID"]?.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng nhập chủng loại vật tư cho thuế", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        bool IsDulicate = _tblSaveThueBG.AsEnumerable().Any(x => x["MaThue"]?.ToString() == rowThue["MaThue"]?.ToString()
                        && x["MaCLVTID"]?.ToString() == rowThue["MaCLVTID"]?.ToString());
                        if (IsDulicate)
                        {
                            XtraMessageBox.Show($"Thuế {grvThue.GetRowCellDisplayText(rowHandel, colLoaiThue)} và Chủng loại" +
                                $" vật tư {grvThue.GetRowCellDisplayText(rowHandel, colLoaiVTThue)} đã có !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        DataRow row = _tblSaveThueBG.NewRow();
                        row["ID"] = rowThue["ID"];
                        row["Thue"] = rowThue["Thue"];
                        row["GhiChu"] = rowThue["GhiChu"];
                        row["MaCLVTID"] = rowThue["MaCLVTID"];
                        row["MaPhieuBG"] = maPhieuBG;
                        row["MaThue"] = rowThue["MaThue"];
                        _tblSaveThueBG.Rows.Add(row);
                        rowHandel++;
                    }
                    string jsonDataPostThue = JsonConvert.SerializeObject(_tblSaveThueBG);
                    string resultPostThue = Task.Run(async () => { return await _clientExtension.PostAsync(urlPostThue, jsonDataPostThue); }).Result;
                }
                if (result.Trim().ToLower() == "true")
                {
              

                    clsWaitForm.ShowSuccessForm(this, 2000);
                    //this.DialogResult = DialogResult.OK;
                    this.Close();

                }
                else
                {
                    XtraMessageBox.Show("Lỗi khi lưu:\n" + result, "Lỗi lưu dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       

        private void btnDeleteVTBaoGia_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataTable tblPhieuVatTuBG = gcVatTuPhieuBG_NPL.DataSource as DataTable;
                if (tblPhieuVatTuBG == null) return;

                DialogResult messResult = MessageBox.Show(
                    "Bạn có muốn xóa dòng này không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;

                int[] selectedHandles = bgvVatTuPhieuBG_NPL.GetSelectedRows();

               
                for (int i = selectedHandles.Length - 1; i >= 0; i--)
                {
                    int rowHandle = selectedHandles[i];
                    DataRow dr = bgvVatTuPhieuBG_NPL.GetDataRow(rowHandle);
                    if (dr == null) continue;



                    if (dr["ID"].ToString() != "0")
                    {
                        DataTable tbl = new DataTable();
                        string urlGET = $"{URL}PhieuBaoGia/GET?action=CheckXetDuyet&para1={_maPhieuBG}";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                        if (json != "[]")
                        {
                            tbl = JsonConvert.DeserializeObject<DataTable>(json);
                            if (tbl?.Rows?.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(tbl.Rows[0]["Msg"]?.ToString()) && tbl.Rows[0]["Isval"]?.ToString()?.ToLower() == "true")
                                {
                                    XtraMessageBox.Show(tbl.Rows[0]["Msg"]?.ToString(), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                        }

                        string url = $"{URL}PhieuBaoGia/Delete?action=Delete&Para1={dr["ID"]}&Para2=None";
                        string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }


                    tblPhieuVatTuBG.Rows.Remove(dr);
                }

                gcVatTuPhieuBG_NPL.DataSource = tblPhieuVatTuBG;
                gcVatTuPhieuBG_NPL.RefreshDataSource();



                bgvVatTuPhieuBG_NPL.RefreshData();
                bgvVatTuPhieuBG_NPL.UpdateSummary();

                grvChiPhi.RefreshData();
                grvChiPhi.UpdateSummary();

                TinhToanTongGiaTri(bgvVatTuPhieuBG_NPL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private decimal ToDecimal(object val)
        {
            if (val == null || val == DBNull.Value) return 0m;
            decimal d;
            return decimal.TryParse(val.ToString(), out d) ? d : 0m;
        }

        private void AddVatTuBaoGia()
        {
            DateTime ngayBDHieuLuc = clsForrmatUtils.ConvertDate(DateEditBDHieuLuc.EditValue?.ToString());
            if (ngayBDHieuLuc == DateTime.MinValue)
            {
                XtraMessageBox.Show("Vui lòng chọn ngày bắt đầu hiệu lực", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string NgayBDHieuLuc = ngayBDHieuLuc.ToString("yyyy-MM-ddTHH:mm:ss"); ;
            string maPhieuBG = _maPhieuBG;
            string maLoaiCC = SearchLookUpLoaiCC.EditValue?.ToString();
            string maNCC = SearchLookupNhaCC.EditValue?.ToString();
            string TenNCC = SearchLookupNhaCC.Text?.ToString();
            if (string.IsNullOrEmpty(maLoaiCC))
            {
                XtraMessageBox.Show("Vui lòng chọn loại cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(maNCC))
            {
                XtraMessageBox.Show("Vui lòng chọn nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable tbl = gcVatTuPhieuBG_NPL.DataSource as DataTable;
            frmSelectVTBaoGia frm = new frmSelectVTBaoGia(maLoaiCC, maNCC, TenNCC, (!string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null), tbl,"", NgayBDHieuLuc);
            frm.ShowDialog();
            if (frm.tblVatTuSelected != null && frm.tblVatTuSelected.Rows.Count > 0)
            {
                if (_tblPhieuBaoGia == null || _tblPhieuBaoGia.Rows.Count == 0)
                {
                    _tblPhieuBaoGia = CreateDataTable_PhieuBaoGia_ChiTiet();
                }

                this.ActiveControl = simpleButton1;

                DataTable tblThue = grcThue.DataSource as DataTable;
                DataTable tblChietKhau = grcChietKhau.DataSource as DataTable;

              
                Dictionary<string, decimal> thueGroup = null;
                Dictionary<string, decimal> ckGroup = null;

                if (tblThue != null && tblThue.Rows.Count > 0)
                {
                    thueGroup = tblThue.AsEnumerable()
                        .GroupBy(r => (r["MaCLVTID"] ?? "").ToString())
                        .Select(g => new
                        {
                            MaCLVTID = g.Key,
                            TongThue = g.Sum(r => ToDecimal(r["Thue"]))
                        })
                        .ToDictionary(x => x.MaCLVTID, x => x.TongThue);
                }

               
                if (tblChietKhau != null && tblChietKhau.Rows.Count > 0)
                {
                    ckGroup = tblChietKhau.AsEnumerable()
                        .GroupBy(r => (r["MaCLVTID"] ?? "").ToString())
                        .Select(g => new
                        {
                            MaCLVTID = g.Key,
                            TongChietKhau = g.Sum(r => ToDecimal(r["ChietKhau"]))
                        })
                        .ToDictionary(x => x.MaCLVTID, x => x.TongChietKhau);
                }

                List<DataRow> newRows = new List<DataRow>();

                foreach (DataRow vt in frm.tblVatTuSelected.Rows)
                {
                    string maVTID = vt["MaVTID"]?.ToString() ?? "";
                    string mauVTID = vt["MauVTID"]?.ToString() ?? "";
                    string chungLoaiCC = vt["MaCLVT"]?.ToString() ?? "";
                    string khoSizeID = vt["KhoVaiID"]?.ToString() ?? "";
                    string maDVVT = vt["MaDVVT"]?.ToString() ?? "";

                    DataRow newRow = _tblPhieuBaoGia.NewRow();

                    newRow["STT"] = 0;
                    newRow["ID"] = 0;
                    newRow["MaPhieuBG"] = _maPhieuBG;
                    newRow["ItemCode"] = vt["ItemCode"]?.ToString() ?? "";
                    newRow["MaVTID"] = maVTID;
                    newRow["MauVTID"] = mauVTID;
                    newRow["MaMauVT"] = vt["MaMauVT"]?.ToString() ?? "";
                    newRow["KhoSizeID"] = khoSizeID;
                    newRow["MaDVVT"] = maDVVT;
                    newRow["TenCL"] = vt["TenCL"]?.ToString() ?? "";
                    newRow["MoTa"] = vt["MoTa"]?.ToString() ?? "";
                    newRow["TenDVVT"] = vt["TenDVVT"]?.ToString() ?? "";
                    newRow["MauVT"] = vt["MauVT"]?.ToString() ?? "";
                    newRow["KhoVai"] = vt["KhoVai"]?.ToString() ?? "";
                    newRow["Sort"] = vt["Sort"];

                    bool isNPL = vt["IsNPL"] != DBNull.Value && Convert.ToBoolean(vt["IsNPL"]);
                    newRow["IsNPL"] = isNPL;
                    newRow["LoaiNPL"] = isNPL ? "Nguyên Liệu" : "Phụ Liệu";
                    newRow["NhomCLCC"] = maLoaiCC;
                    newRow["MaNCC"] = maNCC;
                    newRow["ChungLoaiCC"] = chungLoaiCC;
                    newRow["TenPhieu"] = _tenPhieu;
                    newRow["IsDuyet"] = false;
                    newRow["NgayDuyet"] = DBNull.Value;
                    newRow["DonGia"] = 0.0;
                    newRow["MaDVTTe"] = null;
                    newRow["PTThanhToan"] = "";
                    newRow["PTVanChuyen"] = "";
                    newRow["NgayBDHieuLuc"] = DateTime.Now;
                    newRow["SoNgayHieuLuc"] = 0;
                    newRow["SoNgayGHSom"] = 0;
                    newRow["SoNgayGHTre"] = 0;
                    newRow["GhiChu"] = "";
                    newRow["GhiChuPhieu"] = "";
                    newRow["HinhThucThanhToan"] = "";
                    newRow["DVTienTeVT"] =TienTeDeault;
                    decimal thueByCLVT = 0m;
                    decimal ckByCLVT = 0m;

                    string maCLVTID = chungLoaiCC;

                    if (thueGroup != null && thueGroup.ContainsKey(maCLVTID))
                        thueByCLVT = thueGroup[maCLVTID];

                    if (ckGroup != null && ckGroup.ContainsKey(maCLVTID))
                        ckByCLVT = ckGroup[maCLVTID];

                    newRow["Thue"] = thueByCLVT;
                    newRow["ChietKhau"] = ckByCLVT;

                    newRows.Add(newRow);
                }

                for (int i = newRows.Count - 1; i >= 0; i--)
                {
                    _tblPhieuBaoGia.Rows.InsertAt(newRows[i], 0);
                }

                gcVatTuPhieuBG_NPL.DataSource = _tblPhieuBaoGia;
                bgvVatTuPhieuBG_NPL.ExpandAllGroups();

            }
        }
        private void btnAddVatTu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddVatTuBaoGia();
        }
        #region Thư Viện Event
        private void btnChiPhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmChiPhi frm = new frmChiPhi();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                SetUpColLoaiChiPhi();
            };

            frm.Show();
        }

        private void btnLoaiHH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLoaiNhaCungCap frm = new frmLoaiNhaCungCap();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                LoadLoaiVatTu();
            };

            frm.Show();
        }

        private void btnNhaCC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPNhaCungCapNK frm = new frmERPNhaCungCapNK();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                LoadNhaCC();
            };

            frm.Show();
        }

        private void btnPTThanhToan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDicPTThanhToan frm = new frmDicPTThanhToan();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                LoadPTThanhToan();
            };

            frm.Show();
        }

        private void btnPTVanChuyen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDicPTVanChuyen frm = new frmDicPTVanChuyen();
            frm.StartPosition = FormStartPosition.CenterScreen;

            //frm.FormClosed += (s, args) =>
            //{
            //    LoadPTVanChuyen(); // nếu có hàm load
            //};

            frm.Show();
        }

        private void btnTienTeQD_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmQuyDoiTyGia frm = new frmQuyDoiTyGia();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                LoadTienTe();
            };

            frm.Show();
        }


        //private void btnThue_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    frmThuVienThue frm = new frmThuVienThue();
        //    frm.Show();
        //    frm.StartPosition = FormStartPosition.CenterScreen;
        //    SetUpColLoaiThue();

        //    for (int i = 0; i <= bgvVatTuPhieuBG_NPL.RowCount; i++)
        //    {
        //        bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTThue, "");
        //        bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colThueBG, 0);
        //    }

        //    for (int i = 0; i <= grvChietKhau.RowCount; i++)
        //    {
        //        UpdateTongThueTheoCLVT(i);
        //    }


        //}
        private void btnThue_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmThuVienThue frm = new frmThuVienThue();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                SetUpColLoaiThue();

                for (int i = 0; i < bgvVatTuPhieuBG_NPL.RowCount; i++)
                {
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTThue, "");
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colThueBG, 0);
                }

                for (int i = 0; i < grvChietKhau.RowCount; i++)
                {
                    UpdateTongThueTheoCLVT(i);
                }
            };

            frm.Show();
        }

        private void btnChietKhau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmThuVienChietKhau frm = new frmThuVienChietKhau();
            frm.StartPosition = FormStartPosition.CenterScreen;

            frm.FormClosed += (s, args) =>
            {
                
                SetUpColLoaiChietKhau();

                
                for (int i = 0; i < bgvVatTuPhieuBG_NPL.RowCount; i++)
                {
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTChietKhau, "");
                    bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colChietKhauBG, 0);
                }

                for (int i = 0; i < grvChietKhau.RowCount; i++)
                {
                    UpdateTongCheckKhauTheoCLVT(i);
                }
            };

            frm.Show(); 
        }

        #endregion

        #region Validate

        private void gridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();
        }
        private void grvChiPhiVatTu_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            GridColumn col = view.FocusedColumn;
            if (col == null) return;

            // === 1. Validate cột Tên Chi Phí (giữ nguyên của bạn) ===
            if (col == colTenChiPhi)
            {
                string newValue = e.Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(newValue))
                {
                    e.Valid = false;
                    e.ErrorText = "Tên chi phí không được bỏ trống!";
                    return;
                }

                // Kiểm tra trùng
                bool isDuplicate = Enumerable.Range(0, view.DataRowCount)
                    .Where(i => i != view.FocusedRowHandle)
                    .Any(i => string.Equals(view.GetRowCellValue(i, col)?.ToString()?.Trim(), newValue, StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    e.Valid = false;
                    e.ErrorText = "Tên chi phí đã tồn tại! Vui lòng chọn tên khác.";
                    return;
                }

                e.Valid = true;
                return;
            }

            if (col == colChiPhi_CP)
            {
                string text = e.Value?.ToString()?.Trim();


                if (string.IsNullOrWhiteSpace(text))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập số tiền chi phí!";
                    return;
                }


                if (!decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                                      System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    e.Valid = false;
                    e.ErrorText = "Chỉ được nhập số";
                    return;
                }

                // Kiểm tra số âm (nếu không cho phép âm)
                if (value < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số tiền không được âm!";
                    return;
                }


            }
          else  if (col == colThueCP)
            {
                string text = e.Value?.ToString()?.Trim();


                if (string.IsNullOrWhiteSpace(text))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập số  thuế!";
                    return;
                }


                if (!decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                                      System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    e.Valid = false;
                    e.ErrorText = "Chỉ được nhập số";
                    return;
                }

                // Kiểm tra số âm (nếu không cho phép âm)
                if (value < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số thuế không được âm!";
                    return;
                }
                //if (value > 100)
                //{
                //    e.Valid = false;
                //    e.ErrorText = "Số % thuế nhỏ hơn hoặc bằng 100";
                //    return;
                //}

                e.Valid = true;
                e.ErrorText = string.Empty;
            }
        }
        private void grvThue_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || view.FocusedRowHandle < 0) return;

           
            if (view.FocusedColumn == colSoThue)
            {
                string text = e.Value?.ToString()?.Trim();


                if (string.IsNullOrWhiteSpace(text))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập số  thuế!";
                    return;
                }


                if (!decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                                      System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    e.Valid = false;
                    e.ErrorText = "Chỉ được nhập số";
                    return;
                }

                // Kiểm tra số âm (nếu không cho phép âm)
                if (value < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số thuế không được âm!";
                    return;
                }

                //if (value > 100)
                //{
                //    e.Valid = false;
                //    e.ErrorText = "Số % thuế nhỏ hơn hoặc bằng 100";
                //    return;
                //}
                e.Valid = true;
                e.ErrorText = string.Empty;
            }
            else if(view.FocusedColumn == colLoaiThue || view.FocusedColumn == colLoaiVTThue)
            {
                string newValue = Convert.ToString(e.Value);
                if (string.IsNullOrEmpty(newValue)) return;

                // Lấy giá trị của cột còn lại
                string loaiThue = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, colLoaiThue));
                string loaiVTThue = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, colLoaiVTThue));

                // Xác định giá trị sẽ kiểm tra
                string valueLoaiThue = view.FocusedColumn == colLoaiThue ? newValue : loaiThue;
                string valueLoaiVTThue = view.FocusedColumn == colLoaiVTThue ? newValue : loaiVTThue;

                // Kiểm tra 2 giá trị không được trùng nhau
                if (valueLoaiThue == valueLoaiVTThue && !string.IsNullOrEmpty(valueLoaiThue))
                {
                    e.Valid = false;
                    e.ErrorText = "Chủng loại vật tư và loại Thuế không được trùng.";
                    return;
                }

                // Kiểm tra xem đã tồn tại dòng nào có cả 2 giá trị này chưa (trừ dòng hiện tại)
                bool exists = Enumerable.Range(0, view.DataRowCount)
                    .Where(rowHandle => rowHandle != view.FocusedRowHandle) // Bỏ qua dòng hiện tại
                    .Any(rowHandle =>
                    {
                        string existingLoaiThue = Convert.ToString(view.GetRowCellValue(rowHandle, colLoaiThue));
                        string existingLoaiVTThue = Convert.ToString(view.GetRowCellValue(rowHandle, colLoaiVTThue));

                    // Kiểm tra cả 2 cột đều khớp
                    return existingLoaiThue == valueLoaiThue && existingLoaiVTThue == valueLoaiVTThue;
                    });

                if (exists)
                {
                    e.Valid = false;
                    e.ErrorText = "Cặp giá trị Loại Thuế và Loại VT Thuế này đã tồn tại.";
                }
            }
        }

        private void grvChietKhau_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || view.FocusedRowHandle < 0) return;

            if (view.FocusedColumn == colChietKhau)
            {
                string text = e.Value?.ToString()?.Trim();


                if (string.IsNullOrWhiteSpace(text))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập số  thuế!";
                    return;
                }


                if (!decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                                      System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    e.Valid = false;
                    e.ErrorText = "Chỉ được nhập số";
                    return;
                }

                // Kiểm tra số âm (nếu không cho phép âm)
                if (value < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số thuế không được âm!";
                    return;
                }
                //if (value > 100)
                //{
                //    e.Valid = false;
                //    e.ErrorText = "Số % chiết khấu nhỏ hơn hoặc bằng 100";
                //    return;
                //}


                e.Valid = true;
                e.ErrorText = string.Empty;
            }
            else if(view.FocusedColumn== colNhomVT_CK || view.FocusedColumn == colLoaiCK)
            {
                string newValue = Convert.ToString(e.Value);
                if (string.IsNullOrEmpty(newValue)) return;

                // Lấy giá trị của cột còn lại
                string nhomVT_CK = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, colNhomVT_CK));
                string loaiCK = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, colLoaiCK));

                // Xác định giá trị sẽ kiểm tra
                string valueNhomVT_CK = view.FocusedColumn == colNhomVT_CK ? newValue : nhomVT_CK;
                string valueLoaiCK = view.FocusedColumn == colLoaiCK ? newValue : loaiCK;

                // Kiểm tra 2 giá trị không được trùng nhau
                if (valueNhomVT_CK == valueLoaiCK && !string.IsNullOrEmpty(valueNhomVT_CK))
                {
                    e.Valid = false;
                    e.ErrorText = "Chủng loại vật tư và Loại chiết khấu không được trùng nhau.";
                    return;
                }

                // Kiểm tra xem đã tồn tại dòng nào có cả 2 giá trị này chưa (trừ dòng hiện tại)
                bool exists = Enumerable.Range(0, view.DataRowCount)
                    .Where(rowHandle => rowHandle != view.FocusedRowHandle) // Bỏ qua dòng hiện tại
                    .Any(rowHandle =>
                    {
                        string existingNhomVT_CK = Convert.ToString(view.GetRowCellValue(rowHandle, colNhomVT_CK));
                        string existingLoaiCK = Convert.ToString(view.GetRowCellValue(rowHandle, colLoaiCK));

                        // Kiểm tra cả 2 cột đều khớp
                        return existingNhomVT_CK == valueNhomVT_CK && existingLoaiCK == valueLoaiCK;
                    });

                if (exists)
                {
                    e.Valid = false;
                    e.ErrorText = "Cặp giá trị Nhóm VT CK và Loại CK này đã tồn tại.";
                }
            }
        }

        #endregion

        #region Style Grid

        private void grv_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;

                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;

                int groupLevel = view.GetRowLevel(e.RowHandle);

                GridColumn groupColumn = info.Column;


                info.GroupText = string.Format("{0}", info.GroupValueText);

                if (view.IsGroupRow(e.RowHandle))

                {

                    Color textColor = Color.Black;

                    switch (groupLevel)

                    {

                        case 0: textColor = Color.MediumBlue; break;

                        case 1: textColor = Color.Maroon; break;

                    }

                    e.Appearance.ForeColor = textColor;

                    e.DefaultDraw();

                    e.Handled = true;

                }

            }
            catch (Exception ex)
            {

            }
        }

        private void grv_RowStyle(object sender, RowStyleEventArgs e)
        {

            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle == view.FocusedRowHandle)
            {

                e.Appearance.BackColor = Color.FromArgb(220, 237, 252);
                e.HighPriority = true;
            }
        }

        private void grv_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Value == null || e.Value == DBNull.Value || e.Value.ToString() == "")
                {
                    if (lstFormatFieldName.Contains(e.Column.FieldName)) e.DisplayText = "-";
                    return;
                }

                var view = sender as GridView;
                if (view?.IsEditing == true && view.FocusedColumn == e.Column)
                    return; 
                if (lstFormatFieldName.Contains(e.Column.FieldName))
                {

                    if (e.Value == null || e.Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(e.Value?.ToString()) ||
                        !decimal.TryParse(e.Value.ToString(), out decimal value) ||
                        value == 0m)
                    {
                        e.DisplayText = "-";
                        return;
                    }


                    var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    nfi.NumberGroupSeparator = ",";

                    int decimals = (decimal.GetBits(value)[3] >> 16) & 0x000000FF;
                    decimals = decimals == 0 ? 0 : Math.Min(decimals, 4);

                    string format = decimals == 0 ? "N0" : $"N{decimals}";

                    e.DisplayText = value.ToString(format, nfi);
                    return;
                }
                if (e.Column == colNgaySuaLS)
                {
                    if (e.Value == null || e.Value == DBNull.Value || string.IsNullOrWhiteSpace(e.Value?.ToString()))
                    {
                        e.DisplayText = "";
                        return;
                    }
                    DateTime dt = clsForrmatUtils.ConvertDate(e.Value.ToString());
                    e.DisplayText = dt == DateTime.MinValue ? "" : dt.ToString("dd-MM-yyyy");
                }

                else if (e.Column == colThoiGianSua)
                {
                    e.DisplayText = clsForrmatUtils.ParseMinutesFromTime(e.Value?.ToString());
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void bgv_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            var column = view.FocusedColumn;
            var cellValue = view.GetRowCellValue(view.FocusedRowHandle, column);
            if (cellValue != null && cellValue.ToString() == "0")
            {
                view.SetRowCellValue(view.FocusedRowHandle, column, string.Empty);
            }
        }
        #endregion
  
        private void bgvVatTuPhieuBG_NPL_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void bgvVatTuPhieuBG_NPL_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = bgvVatTuPhieuBG_NPL.GetDataRow(e.FocusedRowHandle);
                if (rowFocused == null) return;

                LoadLichSuBG(rowFocused);
            }
            focused(sender);
        }

        private void focused(object sender)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                bool.TryParse(_tblPhieuBaoGia?.Rows[0]["IsDuyet"]?.ToString(), out bool IsDuyet);

                if (view == null) return;
                if (IsDuyet && _action != "copy")
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    return;
                }
                else if (lstColEdit.Contains(view.FocusedColumn))
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    if (view.FocusedColumn == colDonGia)
                    {
                        DataRow rowFocused = view.GetFocusedDataRow();
                        if (rowFocused != null)
                        {
                            int.TryParse(rowFocused["ID"]?.ToString(), out int ID);
                            if (ID > 0   && _action != "copy")
                            {
                                view.FocusedColumn.OptionsColumn.AllowEdit = false;
                            }
                            else
                            {
                                view.FocusedColumn.OptionsColumn.AllowEdit = true;
                            }
                        }
                    }

                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void LoadLichSuBG(DataRow rowFocused)
        {
             try
            {
               

                DataTable tblLichSu = new DataTable();
                string url = $"{URL}PhieuBaoGia/GET?action=GetLSBaoGia&para1={_maPhieuBG}&para2={rowFocused["ChungLoaiCC"]}&para3={rowFocused["MaVTID"]}&para4={rowFocused["MauVTID"]}&para5={rowFocused["KhoSizeID"]}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tblLichSu = JsonConvert.DeserializeObject<DataTable>(json);
                }
                grcLSBaoGia.DataSource = tblLichSu;
                grcLSBaoGia.RefreshDataSource();
            }
            catch (Exception ex)
            {

            }
        }

        private void bgvVatTuPhieuBG_NPL_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                BandedGridView view = sender as BandedGridView;
                if (view == null) return;

               
                if (e.Column.FieldName == "DonGia" ||
                    e.Column.FieldName == "Thue" ||
                    e.Column.FieldName == "ChietKhau" || e.Column == colTienTe)
                {
                    string TienTeID = view.GetRowCellValue(e.RowHandle, colTienTe)?.ToString();
                    decimal donGia = Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, "DonGia") ?? 0);
                    decimal thue = Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, "Thue") ?? 0);
                    decimal chietKhau = Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, "ChietKhau") ?? 0);

                   
                    decimal giamGia = donGia * chietKhau / 100;
                    decimal donGiaSauCK = donGia - giamGia;
                    decimal tienThue = donGiaSauCK * thue / 100;
                    decimal donGiaSauThueCK = donGiaSauCK + tienThue;

                    
                                   
                    decimal tongDonGia = 0;
                    decimal tongDonGiaSauThueCK = 0;

                    for (int i = 0; i < view.DataRowCount; i++)
                    {
                        decimal dg = Convert.ToDecimal(view.GetRowCellValue(i, "DonGia") ?? 0);
                        decimal t = Convert.ToDecimal(view.GetRowCellValue(i, "Thue") ?? 0);
                        decimal ck = Convert.ToDecimal(view.GetRowCellValue(i, "ChietKhau") ?? 0);

                        tongDonGia += dg;

                        // Tính lại đơn giá sau thuế CK cho từng dòng
                        decimal gg = dg * ck / 100;
                        decimal dgSauCK = dg - gg;
                        decimal tThue = dgSauCK * t / 100;
                        decimal dgSauThueCK = dgSauCK + tThue;

                        tongDonGiaSauThueCK += QuyDoiDonGia(TienTeID,dgSauThueCK);
                    }

                  
                    txtDGiaVatTu.EditValue = tongDonGiaSauThueCK;


                    decimal chiPhiSinh = 0;

                    if (txtChiPhatSinh.EditValue != null && txtChiPhatSinh.EditValue != DBNull.Value)
                        decimal.TryParse(txtChiPhatSinh.EditValue.ToString(), out chiPhiSinh);


                    txtChiPhiCoBan.EditValue = tongDonGia + chiPhiSinh;

                   
                    txtChiPhiSauThueCK.EditValue = tongDonGiaSauThueCK + chiPhiSinh;

                    view.UpdateCurrentRow();
                    TinhToanTongGiaTri(view);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Lỗi tính toán: " + ex.Message, "Lỗi",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bgvVatTuPhieuBG_NPL_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                var view = bgvVatTuPhieuBG_NPL;
                if (view == null || view.DataSource == null) return;

                if (bgvVatTuPhieuBG_NPL.RowCount > 0)
                {
                    DataRow rowFocused = bgvVatTuPhieuBG_NPL.GetDataRow(0);
                    if (rowFocused == null) return;
                    LoadLichSuBG(rowFocused);
                    bgvVatTuPhieuBG_NPL.FocusedRowHandle = 0;
                }

                view.RefreshData();


                TinhToanTongGiaTri(view);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Lỗi tính toán khi load dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm tính toán tổng giá trị (dùng chung cho cả DataSourceChanged và CellValueChanged)
        private void TinhToanTongGiaTri(BandedGridView view)
        {
            try
            {
                txtChiPhatSinh.EditValue = 0;
                txtDGiaVatTu.EditValue = 0;
                txtChiPhiSauThueCK.EditValue = 0;

                grvChiPhi.RefreshData();
                grvChiPhi.UpdateSummary();

                if (view == null || view.DataRowCount == 0) return;

                decimal tongDonGia = 0;
                decimal tongDonGiaSauThueCK = 0;

                if (grvChiPhi != null && grvChiPhi?.RowCount > 0)
                {
                    decimal tongChiPhi = 0;

                    var summaryValue = grvChiPhi.Columns["TotalCP_BG"].SummaryItem.SummaryValue;

                    if (summaryValue != null && summaryValue != DBNull.Value)
                        decimal.TryParse(summaryValue.ToString(), out tongChiPhi);

                    txtChiPhatSinh.Text = tongChiPhi.ToString("n2");
                }
                else
                {
                    txtChiPhatSinh.Text = "0.00";
                }
                // Duyệt qua tất cả các dòng
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    string TienTeID = view.GetRowCellValue(i, colTienTe)?.ToString();
                    decimal donGia = 0, thue = 0, chietKhau = 0;

                    var valDonGia = view.GetRowCellValue(i, "DonGia");
                    if (valDonGia != null && valDonGia != DBNull.Value)
                        decimal.TryParse(valDonGia.ToString(), out donGia);

                    var valThue = view.GetRowCellValue(i, "Thue");
                    if (valThue != null && valThue != DBNull.Value)
                        decimal.TryParse(valThue.ToString(), out thue);

                    var valChietKhau = view.GetRowCellValue(i, "ChietKhau");
                    if (valChietKhau != null && valChietKhau != DBNull.Value)
                        decimal.TryParse(valChietKhau.ToString(), out chietKhau);


                    tongDonGia += donGia;


                    decimal giamGia = donGia * chietKhau / 100;
                    decimal donGiaSauCK = donGia - giamGia;
                    decimal tienThue = donGiaSauCK * thue / 100;
                    decimal donGiaSauThueCK = donGiaSauCK + tienThue;

                    tongDonGiaSauThueCK += QuyDoiDonGia(TienTeID, donGiaSauThueCK) ;


                }


                txtDGiaVatTu.EditValue = tongDonGiaSauThueCK;


                decimal chiPhiSinh = 0;

                if (txtChiPhatSinh.EditValue != null && txtChiPhatSinh.EditValue != DBNull.Value)
                    decimal.TryParse(txtChiPhatSinh.EditValue.ToString(), out chiPhiSinh);

                txtChiPhiSauThueCK.EditValue = tongDonGiaSauThueCK + chiPhiSinh;
                //txtChiPhiSauThueCK.EditValue = tongDonGiaSauThueCK + chiPhiSinh;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Lỗi trong quá trình tính toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtChiPhatSinh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                decimal tongDonGia = 0m;
                decimal chiPhiSinh = 0m;

             

                //grvChiPhi.RefreshData();
                //grvChiPhi.UpdateSummary();
          

                if (txtDGiaVatTu.EditValue != null)
                {
                    string giaVatTuText = txtDGiaVatTu.EditValue.ToString();
                    if (!decimal.TryParse(giaVatTuText, out tongDonGia))
                    {
                        tongDonGia = 0m; 
                                         
                    }
                }

              
                if (txtChiPhatSinh.EditValue != null)
                {
                    string chiPhiText = txtChiPhatSinh.EditValue.ToString();
                    if (!decimal.TryParse(chiPhiText, out chiPhiSinh))
                    {
                        chiPhiSinh = 0m; 
                                      
                    }
                }

               
                txtChiPhiSauThueCK.EditValue = tongDonGia + chiPhiSinh;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ bất ngờ (nếu có)
                MessageBox.Show($"Đã xảy ra lỗi khi tính chi phí: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Gán giá trị mặc định khi có lỗi
                txtChiPhiSauThueCK.EditValue = 0m;
            }

        }
        private void bgvVatTuPhieuBG_NPL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                BandedGridView view = sender as BandedGridView;
                if (view == null) return;
                if (!e.IsGetData) return;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                if (view.IsGroupRow(rowHandle))
                {
                    decimal tongGiaTri = 0m;
                    int childCount = view.GetChildRowCount(rowHandle);

                    for (int i = 0; i < childCount; i++)
                    {
                        int childRowHandle = view.GetChildRowHandle(rowHandle, i);

                        if (view.IsGroupRow(childRowHandle))
                        {
                            int subChildCount = view.GetChildRowCount(childRowHandle);
                            for (int j = 0; j < subChildCount; j++)
                            {
                                int subChildRowHandle = view.GetChildRowHandle(childRowHandle, j);
                                if (!view.IsDataRow(subChildRowHandle)) continue;

                                try
                                {
                                    decimal donGia = 0, chietKhau = 0, thueSuat = 0;

                                    var valDonGiaSub = view.GetRowCellValue(subChildRowHandle, "DonGia");
                                    if (valDonGiaSub != null && valDonGiaSub != DBNull.Value)
                                        decimal.TryParse(valDonGiaSub.ToString(), out donGia);

                                    var valChietKhauSub = view.GetRowCellValue(subChildRowHandle, "ChietKhau");
                                    if (valChietKhauSub != null && valChietKhauSub != DBNull.Value)
                                        decimal.TryParse(valChietKhauSub.ToString(), out chietKhau);

                                    var valThueSuatSub = view.GetRowCellValue(subChildRowHandle, "Thue");
                                    if (valThueSuatSub != null && valThueSuatSub != DBNull.Value)
                                        decimal.TryParse(valThueSuatSub.ToString(), out thueSuat);

                                    string DonViTienTe = view.GetRowCellValue(subChildRowHandle, "DVTienTeVT")?.ToString();

                                    if (e.Column == colDGSuaCKhau)
                                        tongGiaTri += donGia * (1 - chietKhau / 100m);
                                    else if (e.Column == colDGSauThue)
                                        tongGiaTri += (donGia * (1 - chietKhau / 100m)) * (1 + thueSuat / 100m);
                                    else if (e.Column == colDGSauThue_QD)
                                        tongGiaTri += QuyDoiDonGia(DonViTienTe, (donGia * (1 - chietKhau / 100m)) * (1 + thueSuat / 100m));
                                }
                                catch { }
                            }
                            continue;
                        }

                        if (!view.IsDataRow(childRowHandle)) continue;

                        try
                        {
                            decimal donGia = 0, chietKhau = 0, thueSuat = 0;

                            var valDonGiaChild = view.GetRowCellValue(childRowHandle, "DonGia");
                            if (valDonGiaChild != null && valDonGiaChild != DBNull.Value)
                                decimal.TryParse(valDonGiaChild.ToString(), out donGia);

                            var valChietKhauChild = view.GetRowCellValue(childRowHandle, "ChietKhau");
                            if (valChietKhauChild != null && valChietKhauChild != DBNull.Value)
                                decimal.TryParse(valChietKhauChild.ToString(), out chietKhau);

                            var valThueSuatChild = view.GetRowCellValue(childRowHandle, "Thue");
                            if (valThueSuatChild != null && valThueSuatChild != DBNull.Value)
                                decimal.TryParse(valThueSuatChild.ToString(), out thueSuat);

                            string DonViTienTeChild = view.GetRowCellValue(childRowHandle, "DVTienTeVT")?.ToString();

                            if (e.Column == colDGSuaCKhau)
                                tongGiaTri += donGia * (1 - chietKhau / 100m);
                            else if (e.Column == colDGSauThue)
                                tongGiaTri += (donGia * (1 - chietKhau / 100m)) * (1 + thueSuat / 100m);
                            else if (e.Column == colDGSauThue_QD)
                                tongGiaTri += QuyDoiDonGia(DonViTienTeChild, (donGia * (1 - chietKhau / 100m)) * (1 + thueSuat / 100m));
                        }
                        catch { }
                    }

                    e.Value = Math.Round(tongGiaTri, 2, MidpointRounding.AwayFromZero);
                    return;
                }

                if (rowHandle < 0 || !view.IsDataRow(rowHandle))
                {
                    e.Value = 0m;
                    return;
                }

                decimal dg = 0, ck = 0, thue = 0;

                var valDonGiaMain = view.GetRowCellValue(rowHandle, "DonGia");
                if (valDonGiaMain != null && valDonGiaMain != DBNull.Value)
                    decimal.TryParse(valDonGiaMain.ToString(), out dg);

                var valChietKhauMain = view.GetRowCellValue(rowHandle, "ChietKhau");
                if (valChietKhauMain != null && valChietKhauMain != DBNull.Value)
                    decimal.TryParse(valChietKhauMain.ToString(), out ck);

                var valThueMain = view.GetRowCellValue(rowHandle, "Thue");
                if (valThueMain != null && valThueMain != DBNull.Value)
                    decimal.TryParse(valThueMain.ToString(), out thue);

                string DonViTienTeQD = view.GetRowCellValue(rowHandle, "DVTienTeVT")?.ToString();

                if (e.Column == colDGSuaCKhau)
                {
                    decimal ketQua = dg * (1 - ck / 100m);
                    e.Value = Math.Round(ketQua, 2, MidpointRounding.AwayFromZero);
                }
                else if (e.Column == colDGSauThue)
                {
                    decimal donGiaSauCK = dg * (1 - ck / 100m);
                    decimal ketQua = donGiaSauCK * (1 + thue / 100m);
                    e.Value = Math.Round(ketQua, 2, MidpointRounding.AwayFromZero);
                }
                else if (e.Column == colDGSauThue_QD)
                {
                    decimal donGiaSauCK = dg * (1 - ck / 100m);
                    decimal ketQua = donGiaSauCK * (1 + thue / 100m);
                    e.Value = Math.Round(QuyDoiDonGia(DonViTienTeQD, ketQua), 2, MidpointRounding.AwayFromZero);
                }
            }
            catch (Exception ex)
            {
                e.Value = 0m;
                System.Diagnostics.Debug.WriteLine($"Lỗi CustomUnboundColumnData: {ex.Message}");
            }
        }

        //private void bgvVatTuPhieuBG_NPL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        //{
        //    try
        //    {
        //        BandedGridView view = sender as BandedGridView;
        //        if (view == null) return;


        //        if (!e.IsGetData) return;


        //        int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

        //        if (view.IsGroupRow(rowHandle))
        //        {

        //            // Xử lý cho dòng Group - tính tổng
        //            decimal tongGiaTri = 0m;
        //            int childCount = view.GetChildRowCount(rowHandle);

        //            for (int i = 0; i < childCount; i++)
        //            {
        //                int childRowHandle = view.GetChildRowHandle(rowHandle, i);

        //                if (view.IsGroupRow(childRowHandle))
        //                {
        //                    // Nếu là group con (level 2), xử lý đệ quy
        //                    int subChildCount = view.GetChildRowCount(childRowHandle);
        //                    for (int j = 0; j < subChildCount; j++)
        //                    {
        //                        int subChildRowHandle = view.GetChildRowHandle(childRowHandle, j);
        //                        if (!view.IsDataRow(subChildRowHandle)) continue;

        //                        try
        //                        {
        //                            decimal donGia = 0, chietKhau = 0, thueSuat = 0;

        //                            object valDonGia = view.GetRowCellValue(subChildRowHandle, "DonGia");
        //                            if (valDonGia != null && valDonGia != DBNull.Value)
        //                                decimal.TryParse(valDonGia.ToString(), out donGia);

        //                            object valChietKhau = view.GetRowCellValue(subChildRowHandle, "ChietKhau");
        //                            if (valChietKhau != null && valChietKhau != DBNull.Value)
        //                                decimal.TryParse(valChietKhau.ToString(), out chietKhau);

        //                            object valThueSuat = view.GetRowCellValue(subChildRowHandle, "Thue");
        //                            if (valThueSuat != null && valThueSuat != DBNull.Value)
        //                                decimal.TryParse(valThueSuat.ToString(), out thueSuat);

        //                            string DonViTienTe = view.GetRowCellValue(subChildRowHandle, "DVTienTeVT")?.ToString();

        //                            if (e.Column == colDGSuaCKhau)
        //                            {
        //                                tongGiaTri += donGia * (1 - chietKhau / 100m);
        //                            }
        //                            else if (e.Column == colDGSauThue)
        //                            {
        //                                decimal donGiaSauCK = donGia * (1 - chietKhau / 100m);
        //                                tongGiaTri += donGiaSauCK * (1 + thueSuat / 100m);
        //                            }
        //                            else if (e.Column == colDGSauThue_QD)
        //                            {
        //                                decimal donGiaSauCK = donGia * (1 - chietKhau / 100m);
        //                                tongGiaTri += QuyDoiDonGia(DonViTienTe, (donGiaSauCK * (1 + thueSuat / 100m)));
        //                            }

        //                        }
        //                        catch { }
        //                    }
        //                    continue;
        //                }

        //                // Xử lý child row thông thường
        //                if (!view.IsDataRow(childRowHandle)) continue;

        //                try
        //                {
        //                    decimal donGia = 0, chietKhau = 0, thueSuat = 0;

        //                    var valDonGia = view.GetRowCellValue(childRowHandle, "DonGia");
        //                    if (valDonGia != null && valDonGia != DBNull.Value)
        //                        decimal.TryParse(valDonGia.ToString(), out donGia);

        //                    var valChietKhau = view.GetRowCellValue(childRowHandle, "ChietKhau");
        //                    if (valChietKhau != null && valChietKhau != DBNull.Value)
        //                        decimal.TryParse(valChietKhau.ToString(), out chietKhau);

        //                    var valThueSuat = view.GetRowCellValue(childRowHandle, "Thue");
        //                    if (valThueSuat != null && valThueSuat != DBNull.Value)
        //                        decimal.TryParse(valThueSuat.ToString(), out thueSuat);

        //                    string DonViTienTeChild = view.GetRowCellValue(childRowHandle, "DVTienTeVT")?.ToString();
        //                    if (e.Column == colDGSuaCKhau)
        //                    {
        //                        tongGiaTri += donGia * (1 - chietKhau / 100m);
        //                    }
        //                    else if (e.Column == colDGSauThue)
        //                    {
        //                        decimal donGiaSauCK = donGia * (1 - chietKhau / 100m);
        //                        tongGiaTri += donGiaSauCK * (1 + thueSuat / 100m);
        //                    }
        //                    else if (e.Column == colDGSauThue_QD)
        //                    {
        //                        decimal donGiaSauCK = donGia * (1 - chietKhau / 100m);
        //                        tongGiaTri += QuyDoiDonGia(DonViTienTeChild,( donGiaSauCK * (1 + thueSuat / 100m)));
        //                    }

        //                }
        //                catch { }
        //            }

        //            e.Value = Math.Round(tongGiaTri, 2, MidpointRounding.AwayFromZero);
        //            return;
        //        }

        //        // Xử lý cho dòng dữ liệu thông thường
        //        if (rowHandle < 0 || !view.IsDataRow(rowHandle))
        //        {
        //            e.Value = 0m;
        //            return;
        //        }

        //        decimal dg = 0, ck = 0, thue = 0;

        //        var valDonGia = view.GetRowCellValue(rowHandle, "DonGia");
        //        if (valDonGia != null && valDonGia != DBNull.Value)
        //            decimal.TryParse(valDonGia.ToString(), out dg);

        //        var valChietKhau = view.GetRowCellValue(rowHandle, "ChietKhau");
        //        if (valChietKhau != null && valChietKhau != DBNull.Value)
        //            decimal.TryParse(valChietKhau.ToString(), out ck);

        //        var valThue = view.GetRowCellValue(rowHandle, "Thue");
        //        if (valThue != null && valThue != DBNull.Value)
        //            decimal.TryParse(valThue.ToString(), out thue);

        //        string DonViTienTeQD = view.GetRowCellValue(rowHandle, "DVTienTeVT")?.ToString();

        //        if (e.Column == colDGSuaCKhau)
        //        {
        //            decimal ketQua = dg * (1 - ck / 100m);
        //            e.Value = Math.Round(ketQua, 2, MidpointRounding.AwayFromZero);
        //        }
        //        else if (e.Column == colDGSauThue)
        //        {
        //            decimal donGiaSauCK = dg * (1 - ck / 100m);
        //            decimal ketQua = donGiaSauCK * (1 + thue / 100m);
        //            e.Value = Math.Round(ketQua, 2, MidpointRounding.AwayFromZero);
        //        }else if (e.Column == colDGSauThue_QD)
        //        {

        //            decimal donGiaSauCK = dg * (1 - ck / 100m);
        //            decimal ketQua = donGiaSauCK * (1 + thue / 100m);
        //            e.Value = Math.Round(QuyDoiDonGia(DonViTienTeQD, ketQua), 2, MidpointRounding.AwayFromZero);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        e.Value = 0m;
        //        System.Diagnostics.Debug.WriteLine($"Lỗi CustomUnboundColumnData: {ex.Message}");
        //    }
        //}

        private void bttRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            LoadLoaiVatTu();
            LoadTienTe();
            LoadPTThanhToan();
            SetupGridView();
            SetupLayout();

        }

        private void btnChonVatTu_Click(object sender, EventArgs e)
        {
          
  
            AddVatTuBaoGia();

            //clear cache ChietKhau
            for (int i = 0; i <= bgvVatTuPhieuBG_NPL.RowCount; i++)
            {
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTChietKhau, "");
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colChietKhauBG, 0);
            }

            for (int i = 0; i <= grvChietKhau.RowCount; i++)
            {
                UpdateTongCheckKhauTheoCLVT(i);
            }
            //clear cache Thue
            for (int i = 0; i <= bgvVatTuPhieuBG_NPL.RowCount; i++)
            {
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colTTThue, "");
                bgvVatTuPhieuBG_NPL.SetRowCellValue(i, colThueBG, 0);
            }

            for (int i = 0; i <= grvChietKhau.RowCount; i++)
            {
                UpdateTongThueTheoCLVT(i);
            }
        }

        private decimal QuyDoiDonGia(string TienTeQuyDoi,decimal DonGia)
        {
            decimal DonGiaQD = 0;
            decimal result = 0;
            decimal DonGiaTpm = 0;
            if (_tblQuyDoiTT != null && _tblQuyDoiTT?.Rows?.Count > 0)
            {
                var QueryTienTeGoc = _tblQuyDoiTT.AsEnumerable().FirstOrDefault(x => x["TienTeID"]?.ToString() == TienTeQuyDoi);
                if (QueryTienTeGoc != null)
                {
                    decimal.TryParse(QueryTienTeGoc["Gia"]?.ToString(), out DonGiaTpm);
                }

                var Query = _tblQuyDoiTT.AsEnumerable().FirstOrDefault(x => x["TienTeID"]?.ToString() == TienTeID_QD);
                if (Query != null)
                {
                    decimal.TryParse(Query["Gia"]?.ToString(), out DonGiaQD);
                }
                if (DonGiaQD == 0) return 0;
                decimal result_tmp = DonGia * DonGiaTpm;
                result = result_tmp / DonGiaQD;
            }
            return Math.Round(result,2);
        }

        private void bgvVatTuPhieuBG_NPL_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            
            if (view.FocusedColumn.FieldName == "DonGia" || view.FocusedColumn == colThueBG || view.FocusedColumn == colChietKhauBG)
            {
                decimal donGia;
                if (!decimal.TryParse(Convert.ToString(e.Value), out donGia))
                {
                    e.Valid = false;
                    e.ErrorText = "Đơn giá phải là số";
                    return;
                }


                if (donGia < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Đơn giá phải lớn hơn 0.";
                }
            }
                

       
            
        }

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        

        private void SearchLookupPTThanhToan_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnXoaPhieuBG_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            //GridView gridview = ((GridView)sender);
            //if (!gridview.GridControl.IsHandleCreated) return;
            //Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            //SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            //gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }


        private void spinEdit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewValue?.ToString()))
            {
                return;
            }

            int value;
            if (!int.TryParse(e.NewValue.ToString(), out value))
            {
                // Nếu không parse được thì hủy nhập
                e.Cancel = true;
                return;
            }

            if (value < 0)
            {
                e.Cancel = true; // chặn nhập số âm
            }

        }
        

        private void spinEdit_Enter(object sender, EventArgs e)
        {
            SpinEdit editor = sender as SpinEdit;
            if (editor.EditValue != null && editor.EditValue != DBNull.Value)
            {
                if (decimal.TryParse(editor.EditValue.ToString(), out var value) && value == 0)
                {
                    editor.EditValue = null;
                }
            }

        }
        decimal tongDonGiaSum = 0;
        decimal tongDonGiaSauThueCKSum = 0;
        decimal tongDonGiaCKSum = 0;

        private void bgvVatTuPhieuBG_NPL_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            BandedGridView view = sender as BandedGridView;

            // Kiểm tra đúng summary item bạn muốn tính
       
                string TienTeID = view.GetRowCellValue(e.RowHandle, colTienTe)?.ToString();

                if (e.SummaryProcess == CustomSummaryProcess.Start)
                {
                    tongDonGiaSum = 0;
                    tongDonGiaSauThueCKSum = 0;
                    tongDonGiaCKSum = 0;
                }

                if (e.SummaryProcess == CustomSummaryProcess.Calculate)
                {
                decimal dg = 0, t = 0, ck = 0;

                var donGiaObj = view.GetRowCellValue(e.RowHandle, "DonGia");
                if (decimal.TryParse(donGiaObj?.ToString(), out var dgParsed))
                    dg = dgParsed;

                var thueObj = view.GetRowCellValue(e.RowHandle, "Thue");
                if (decimal.TryParse(thueObj?.ToString(), out var tParsed))
                    t = tParsed;

                var ckObj = view.GetRowCellValue(e.RowHandle, "ChietKhau");
                if (decimal.TryParse(ckObj?.ToString(), out var ckParsed))
                    ck = ckParsed;


                tongDonGiaSum += QuyDoiDonGia(TienTeID,dg);

                    // Tính lại đơn giá sau thuế + CK cho từng dòng
                    decimal gg = dg * ck / 100;
                    decimal dgSauCK = dg - gg;
                    decimal tThue = dgSauCK * t / 100;
                    decimal dgSauThueCK = dgSauCK + tThue;

                    tongDonGiaSauThueCKSum += QuyDoiDonGia(TienTeID, dgSauThueCK);
                    tongDonGiaCKSum+= QuyDoiDonGia(TienTeID, dgSauCK);

                }
                if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                {
                if (((DevExpress.XtraGrid.GridColumnSummaryItem)e.Item).FieldName == "DGSauThue")
                {
                    e.TotalValue = tongDonGiaSauThueCKSum.ToString("N2") + " " + _TienTe;
                }

                else if (((DevExpress.XtraGrid.GridColumnSummaryItem)e.Item).FieldName == "DonGia")
                {
                    e.TotalValue = tongDonGiaSum.ToString("N2") + " " + _TienTe;
                }
                else if (((DevExpress.XtraGrid.GridColumnSummaryItem)e.Item).FieldName == "DGSuaCKhau")
                {
                    e.TotalValue = tongDonGiaCKSum.ToString("N2") + " " + _TienTe;
                }
                else if (((DevExpress.XtraGrid.GridColumnSummaryItem)e.Item).FieldName == "DGSauThueQD")
                {
                    e.TotalValue = tongDonGiaSauThueCKSum.ToString("N2") + " " + _TienTe;
                }
             
                }


            
        }

        private bool ValidateAndCleanData()
        {
            string maPhieuBG = _maPhieuBG;
            string maLoaiCC = SearchLookUpLoaiCC.EditValue?.ToString();
            string maNCC = SearchLookupNhaCC.EditValue?.ToString();

            DateTime ngayBDHieuLuc = clsForrmatUtils.ConvertDate(DateEditBDHieuLuc.EditValue?.ToString());
            if (ngayBDHieuLuc == DateTime.MinValue)
            {
                XtraMessageBox.Show("Vui lòng chọn ngày bắt đầu hiệu lực", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        
       
            var errors = new List<string>();
            var combinedKeyList = new HashSet<string>(); // Kiểm tra trùng dựa trên key ghép CodeMau + TenMau
            DataTable tblHienThi = gcVatTuPhieuBG_NPL.DataSource as DataTable;
            if (tblHienThi == null || tblHienThi?.Rows?.Count == 0) return false;
       
            DataTable tblChiTiet = CreateTableSaveBaoGiaChiTiet();
            foreach (DataRow row in tblHienThi.Rows)
            {               
                DataRow newRow = tblChiTiet.NewRow();
                newRow["ID"] = row["ID"] == DBNull.Value || Convert.ToInt64(row["ID"]) <= 0 ? 0 : row["ID"];
                newRow["MaPhieuBG"] = !string.IsNullOrEmpty(maPhieuBG) ? maPhieuBG : null;
                newRow["NhomCLCC"] = maLoaiCC;
                newRow["MaNCC"] = maNCC;
                newRow["ChungLoaiCC"] = row["ChungLoaiCC"]?.ToString() ?? "";
                newRow["ItemCode"] = row["ItemCode"]?.ToString();
                newRow["MaVTID"] = row["MaVTID"]?.ToString();
                newRow["MauVTID"] = row["MauVTID"]?.ToString();
                newRow["KhoSizeID"] = row["KhoSizeID"]?.ToString();
                newRow["MaDVVT"] = row["MaDVVT"]?.ToString();
                double donGia = 0.0;

                if (row["DonGia"] != null && row["DonGia"] != DBNull.Value)
                    double.TryParse(row["DonGia"].ToString(), out donGia);

                newRow["DonGia"] = donGia;

                newRow["Thue"] = 0;
                newRow["ChietKhau"] = 0;
                newRow["GhiChu"] = "";
                newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                newRow["NguoiSua"] = GlobleData.UserName;
                newRow["ThoiGianSua"] = clsForrmatUtils.ParseMinutesFromTime(DateTime.Now.TimeOfDay.ToString());
                newRow["DonViTienTe"] = row["DVTienTeVT"]?.ToString();
                tblChiTiet.Rows.Add(newRow);
            }
            string url = $"{URL}PhieuBaoGia/CheckHieuLuc?action=CheckNgayHieuLuc&para1={maNCC}&para2={ngayBDHieuLuc.ToString("yyyy-MM-ddTHH:mm:ss")}&para3={txtSoPhieu.Text}&para4={_action}&para5=NONE";
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblChiTiet); }).Result;
            if (json != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl?.Rows.Count > 0)
                {
                    if (tbl.Rows.Count > 0)
                    {
                        foreach (DataRow row in tbl?.Rows)
                        {

                            bool.TryParse(row["Isval"]?.ToString(), out bool Isval);
                            if (Isval)
                            {
                               
                                errors.Add($"• {row["Msg"]?.ToString()}.\n");

                            }

                        }
                    }
                }


            }
            if (errors.Count > 0)
            {
                string htmlMessage = string.Join("", errors);

                ShowWarning(htmlMessage);
                return true; // Có lỗi
            }

            //gV.RefreshData();
            return false;
        }

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

        private void DateEditBDHieuLuc_Validated(object sender, EventArgs e)
        {
           
        }
        private void DateEditBDHieuLuc_EditValueChanged(object sender, EventArgs e)
        {
            DateEdit dateEdit = sender as DateEdit;
            if (dateEdit != null)
            {
                DateTime? selectedDate = dateEdit.EditValue as DateTime?;
                if (!selectedDate.HasValue)
                {
                    XtraMessageBox.Show("Vui lòng chọn ngày hiệu lực", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

              
                if (selectedDate.Value.Date < DateTime.Today && _action == "add")
                {
                    XtraMessageBox.Show("Ngày hiệu lực không được nhỏ hơn ngày hiện tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dateEdit.Focus();
                    return;
                }
            }


            ValidateAndCleanData();
        }

        private void grvChiPhi_CustomSummaryExists(object sender, CustomSummaryExistEventArgs e)
        {

        }

        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl2.SplitterPosition = (int)(splitContainerControl2.Width * 1);
        }

        private void BandedGridView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;

            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);
            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {
                ControlPaint.DrawBorder3D(e.Graphics, rect);
                rect.Inflate(-1, -1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    // Màu mặc định
                    Color backColor = Color.FromArgb(255, 192, 128);



                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }
                }

                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                    e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.None,
                            FormatFlags = StringFormatFlags.LineLimit
                        };

                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }
                }

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                // log nếu cần
            }
        }

        private void grv_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;

                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);


                Color backColor = Color.FromArgb(255, 192, 128);




                Brush brush = e.Cache.GetGradientBrush(rect, backColor, backColor, e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);


                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166


                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                // log nếu cần
            }
        }


        private void bgvVatTuMMTB_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            bool isEditable = lstColEdit.Contains(e.Column);

            if (isEditable)
            {
                e.Appearance.BackColor = Color.FromArgb(192, 255, 255);

            }

        }

        private void grvChiPhi_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;
            bool isEditable = e.Column.OptionsColumn.AllowEdit;

            if (isEditable)
            {
                e.Appearance.BackColor = Color.FromArgb(192, 255, 255);

            }
        }

    }

}