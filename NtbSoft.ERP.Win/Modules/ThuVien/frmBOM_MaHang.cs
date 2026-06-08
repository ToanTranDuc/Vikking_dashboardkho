using DevExpress.Data;
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
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmBOM_MaHang : DevExpress.XtraEditors.XtraForm
    {
        SearchCheckSelection gridCheckMarks_VatTu;

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();

        DataTable tblBOM, tblVT, tblSave, tblTenVT;
        DataTable tblSelectedVT;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        private string _mahang = string.Empty, nhomsize = string.Empty, _mavt = string.Empty;
        bool indicatorIcon = true;
        string _nhomSelected = "";

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        string _nguoiTao = string.Empty;
        DataTable tblQD;
        DataTable tblDot;
        private List<int> columnOrderList = new List<int>();
        public frmBOM_MaHang()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            _clientExtension = new HttpClientExtension();
            tblBOM = new DataTable();
            tblVT = new DataTable();
            tblSave = new DataTable();
            tblTenVT = new DataTable();
            tblSelectedVT = new DataTable();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            SaveColumnOrder();
            colQuyDoi.Visible = (bool)chkDMSize.Checked;
            colChieuDai.Visible = (bool)chkDMSize.Checked;
            colTiLeSP.Visible = (bool)chkDMSize.Checked;
            tblQD = new DataTable();
            tblDot = new DataTable();
            //_allowAdd = allowAdd;
            //_allowEdit = allowEdit;
            //_allowDelete = allowDelete;
            //this._nguoiTao = nguoitao;
         
        }
       
        private void BtLoad()
        {
            try
            {
                CreateSearchLookup();
                CreateDataTable();
                creatTable_BOM();
                CreateTableDot();
                this.ActiveControl = btn1;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại1.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        protected override void OnLoad(EventArgs e)
        {
            BtLoad();
            //CheckPerminsion();
            //keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            this.ActiveControl = btn1;
        }
        private void CreateDataTable()
        {
            tblSave = new DataTable("tblSave");

            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("MaBom", typeof(string));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("MaVatTu", typeof(string));
            tblSave.Columns.Add("TenVatTu", typeof(string));
            tblSave.Columns.Add("Mau", typeof(string));
            tblSave.Columns.Add("KhoVai", typeof(string));
            tblSave.Columns.Add("DonViTinh", typeof(string));
            tblSave.Columns.Add("DinhMucMuaHang", typeof(double));
            tblSave.Columns.Add("DinhMucThucTe", typeof(double));
            tblSave.Columns.Add("DinhMucKH", typeof(double));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NgayLap", typeof(string));
            tblSave.Columns.Add("MaSP", typeof(string));
            tblSave.Columns.Add("MaTiLe", typeof(string));
            tblSave.Columns.Add("SLChungTu", typeof(int));
            tblSave.Columns.Add("PhanLoaiVT", typeof(string));
            tblSave.Columns.Add("NhaCungCap", typeof(string));
            tblSave.Columns.Add("POID", typeof(string));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("DauSizeID", typeof(string));
            tblSave.Columns.Add("SizeID", typeof(string));
            tblSave.Columns.Add("SLSanPham", typeof(int));
            tblSave.Columns.Add("SLDinhMuc", typeof(int));
            tblSave.Columns.Add("HaoHut", typeof(double));
            tblSave.Columns.Add("SL", typeof(int));
            tblSave.Columns.Add("SoChungTu", typeof(string));
            tblSave.Columns.Add("SanPham", typeof(int));
            tblSave.Columns.Add("ChieuDaiCuon", typeof(double));
            tblSave.Columns.Add("SoLuongCan", typeof(int));
            tblSave.Columns.Add("SLCanBangMau", typeof(double));
            tblSave.Columns.Add("DonGia", typeof(double));
            tblSave.Columns.Add("ThanhTien", typeof(double));
            tblSave.Columns.Add("SLThucTe", typeof(int));
            tblSave.Columns.Add("SLDat", typeof(int));
            tblSave.Columns.Add("Chon", typeof(int));
            tblSave.Columns.Add("NgayCN", typeof(DateTime));
            tblSave.Columns.Add("ID_DM", typeof(string));
            tblSave.Columns.Add("MaKeToan", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("TiLeSP", typeof(double));
            tblSave.Columns.Add("ChieuDai", typeof(double));
            tblSave.Columns.Add("MaHangCopy", typeof(string));
            tblSave.Columns.Add("NguoiCopy", typeof(string));
            tblSave.Columns.Add("Dot", typeof(string));
            tblSave.Columns.Add("NguoiTao", typeof(string));
            tblSave.Columns.Add("IsXacNhan", typeof(string));

        }

        private void creatTable_BOM()
        {
            tblBOM = new DataTable("tblBOM");
            tblBOM.Columns.Add("ID", typeof(int));
            tblBOM.Columns.Add("ID_DM", typeof(string));
            tblBOM.Columns.Add("MaBom", typeof(string));
            tblBOM.Columns.Add("MaDH", typeof(string));
            tblBOM.Columns.Add("MaVtMau", typeof(string));
            tblBOM.Columns.Add("MaVatTu", typeof(string));
            tblBOM.Columns.Add("TenVatTu", typeof(string));
            tblBOM.Columns.Add("KhoVai", typeof(string));
            tblBOM.Columns.Add("MaNhomVT", typeof(string));
            tblBOM.Columns.Add("Mau", typeof(string));
            tblBOM.Columns.Add("DonViTinh", typeof(string));
            tblBOM.Columns.Add("DinhMucMuaHang", typeof(double));
            tblBOM.Columns.Add("DinhMucThucTe", typeof(double));
            tblBOM.Columns.Add("DinhMucKH", typeof(double));
            tblBOM.Columns.Add("GhiChu", typeof(string));
            tblBOM.Columns.Add("SPOID", typeof(string));
            tblBOM.Columns.Add("PO", typeof(string));
            tblBOM.Columns.Add("MaMau", typeof(string));
            //tblBOM.Columns.Add("MaMau", typeof(string));
            //tblBOM.Columns.Add("MaNhomSize", typeof(string));
            tblBOM.Columns.Add("DauSizeID", typeof(string));
            //tblBOM.Columns.Add("MaSize", typeof(string));
            tblBOM.Columns.Add("SizeID", typeof(string));
            tblBOM.Columns.Add("Nhom", typeof(string));
            tblBOM.Columns.Add("TiLeSP", typeof(double));
            tblBOM.Columns.Add("ChieuDai", typeof(double));
            tblBOM.Columns.Add("Dot", typeof(double));
            tblBOM.Columns.Add("NguoiTao", typeof(string));
            tblBOM.Columns.Add("IsXacNhan", typeof(bool));
            tblBOM.Columns.Add("IsXacNhanText", typeof(string));
        }
        private void CreateTableDot()
        {
            tblDot = new DataTable("tblDot");
            tblDot.Columns.Add("Dot", typeof(string));
        }
        private void LoadDataSource()
        {
            gridCtr_ChiTiet.RefreshDataSource();
            createSearchGridView();
           
            gridCtr_ChiTiet.DataSource = tblBOM;
        }
        //private void frm_KeyDown(object sender, KeyEventArgs e)
        //{
        //    keyDownControlHandler.PressKeyDown(e);
        //}
        //private List<ActionControl> InitActionKeyDown()
        //{
        //    actionControlAdd = new ActionControl(BtThem, _allowAdd, ActionType.Add, this.Them.Enabled);

        //    actionControlRefresh = new ActionControl(BtNapLai, true, ActionType.Refresh, this.Naplai.Enabled);

        //    lstActionControls = new List<ActionControl> {
        //        actionControlAdd, actionControlSave,actionControlRefresh };
        //    return lstActionControls;
        //}
        private void gridViewThongTinDonHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private void CreateSearchLookup()
        {
         
            string urlVT = string.Format("{0}?", URL + "BOM/GET_TenVatTu");
            string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
            tblTenVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
            searchLookUpEdit_VatTu.Properties.DataSource = tblTenVT;
            searchLookUpEdit_VatTu.Properties.ValueMember = "MaVtMau";
            searchLookUpEdit_VatTu.Properties.DisplayMember = "TenVT";

            searchLookUpEdit_VatTu.Properties.View.OptionsSelection.MultiSelect = true;
            searchLookUpEdit_VatTu.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEdit_VatTu_CustomDisplayText);
            searchLookUpEdit_VatTu.Properties.PopulateViewColumns();
            //searchLookUpEditPO.Properties.ShowAddNewButton = true;
            //searchLookUpEditPO.AddNewValue += searchLookUpEditPO_AddNewValue;

            gridCheckMarks_VatTu = new SearchCheckSelection(searchLookUpEdit_VatTu.Properties);
            gridCheckMarks_VatTu.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit_VatTu_SelectionChanged);
            searchLookUpEdit_VatTu.Properties.Tag = gridCheckMarks_VatTu;

            string urlHH = string.Format("{0}?", URL + "BOM/GETMH");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEdit_MH.Properties.DataSource = tblHH;
            searchLookUpEdit_MH.Properties.ValueMember = "MaHang";
            searchLookUpEdit_MH.Properties.DisplayMember = "TenHang";

            string url = string.Format("{0}?", URL + "QuyDoi/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblQD = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditQD = new RepositoryItemSearchLookUpEdit();
            rCountryEditQD.DataSource = tblQD;
            rCountryEditQD.DisplayMember = "QuyDoi";
            rCountryEditQD.ValueMember = "ID";
            rCountryEditQD.ShowClearButton = false;
            rCountryEditQD.NullText = "Chọn";

            GridView dvViewQD = rCountryEditQD.View;
            if (dvViewQD.Columns.Count == 0)
            {
                dvViewQD.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewQD.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewQD.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewQD.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewQD.Columns.Add(new GridColumn { FieldName = "QuyDoi", Caption = "Quy đổi", Name = "colQuyDoi", Visible = true });
                dvViewQD.Columns.Add(new GridColumn { FieldName = "SoLuong", Caption = "Số lượng", Name = "colSoLuong", Visible = true });

            }
            colQuyDoi.ColumnEdit = rCountryEditQD;
        }

        private void createSearchGridView()
        {
        
            //----------4

            string urlMauVaTu = string.Format("{0}?_mavt={1}", URL + "BOM/GET_MauVatTu", _mavt);
            string jsonMauVatTu = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMauVaTu); }).Result;
            DataTable tblMauVatTu = JsonConvert.DeserializeObject<DataTable>(jsonMauVatTu);

            reloadSearchGridView(tblMauVatTu, true);

            //----------4

            string urlKhoSize = string.Format("{0}?_mavt={1}", URL + "BOM/GET_KhoSizeVatTu", _mavt);
            string jsonKhoSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhoSize); }).Result;
            DataTable tblKhoSize = JsonConvert.DeserializeObject<DataTable>(jsonKhoSize);
            reloadSearchGridView(tblKhoSize,false);
        }

        private void reloadSearchGridView(DataTable tbl, bool isMau)
        {
            if(isMau)
            {
                RepositoryItemSearchLookUpEdit rMauVTEdit = new RepositoryItemSearchLookUpEdit();
                rMauVTEdit.DataSource = tbl;
                rMauVTEdit.DisplayMember = "Mau";
                rMauVTEdit.ValueMember = "Mau";
                rMauVTEdit.ShowClearButton = false;
                rMauVTEdit.NullText = "[Chọn giá trị]";

                GridView InMauVTView = rMauVTEdit.View;
                if (InMauVTView.Columns.Count == 0)
                {
                    InMauVTView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    InMauVTView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    InMauVTView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    InMauVTView.Appearance.HeaderPanel.Options.UseTextOptions = true;

                    InMauVTView.Columns.Add(new GridColumn { FieldName = "Mau", Caption = "Màu", Name = "colMauVT1", Visible = true });
                    InMauVTView.Columns.Add(new GridColumn { FieldName = "MaVT", Caption = "Mã VT", Name = "colMaVT1", Visible = false });
                }
                colMauVT.ColumnEdit = rMauVTEdit;
            }   
            else
            {
                RepositoryItemSearchLookUpEdit rKhoSizeEdit = new RepositoryItemSearchLookUpEdit();
                rKhoSizeEdit.DataSource = tbl;
                rKhoSizeEdit.DisplayMember = "KhoVai";
                rKhoSizeEdit.ValueMember = "KhoVai";
                rKhoSizeEdit.ShowClearButton = false;
                rKhoSizeEdit.NullText = "[Chọn giá trị]";

                GridView KhoSizeView = rKhoSizeEdit.View;
                if (KhoSizeView.Columns.Count == 0)
                {
                    KhoSizeView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    KhoSizeView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    KhoSizeView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    KhoSizeView.Appearance.HeaderPanel.Options.UseTextOptions = true;

                    KhoSizeView.Columns.Add(new GridColumn { FieldName = "KhoVai", Caption = "Size / Khổ", Name = "colSizeKho1", Visible = true });
                    KhoSizeView.Columns.Add(new GridColumn { FieldName = "MaVT", Caption = "Mã VT", Name = "colMaVT1", Visible = false });
                }
                colKhoVai.ColumnEdit = rKhoSizeEdit;
            }    
        }
        private void BtSave()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                if (tblBOM.Rows.Count > 0)
                {

                    for (int i = 0; i < tblBOM.Rows.Count; i++)
                    {
                        if(tblBOM.Rows[i]["Nhom"].ToString()=="Phụ liệu")
                        {
                            if(tblBOM.Rows[i]["KhoVai"].ToString()==null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Khổ / Size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }    
                        }
                        else
                        {
                            if(tblBOM.Rows[i]["KhoVai"].ToString() == ""|| tblBOM.Rows[i]["Mau"].ToString() == "")
                            {
                                XtraMessageBox.Show("Vui lòng chọn màu vật tư và Khổ / Size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }    
                        }
                        string urlVT = string.Format("{0}?mavt={1}", URL + "BOM/GET_VatTu", tblBOM.Rows[i]["MaVatTu"]);
                        string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                        tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
                        var result = tblVT.AsEnumerable().Where(row => (bool)row["IsPL"] == false);
                        if (result.Any()) 
                        {
                            var itemExists = tblVT.AsEnumerable().Any(row => row["TenVT"].ToString() == tblBOM.Rows[i]["TenVatTu"].ToString() &&
                                                        row["Mau"].ToString() == tblBOM.Rows[i]["Mau"].ToString() &&
                                                        row["SizeKho"] == tblBOM.Rows[i]["KhoVai"].ToString() 
                                                        );
                            if (itemExists)
                            {
                                XtraMessageBox.Show("Vui lòng chọn màu vật tư và Khổ / Size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        if (cbBoxDot.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn đợt.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        //var rows = tblVT.Select("TenVT = '" + tblBOM.Rows[i]["TenVatTu"] + "' and Mau = '" + tblBOM.Rows[i]["Mau"] + "' and SizeKho = '" + tblBOM.Rows[i]["KhoVai"] + "'");
                        //DataTable dtTempVatTu = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                        DataTable dtTempVatTu = tblBOM.Rows[i]["Mau"].ToString() == "" ? tblVT.Select("TenVT = '" + tblBOM.Rows[i]["TenVatTu"]  + "' and SizeKho = '" + tblBOM.Rows[i]["KhoVai"] + "'").CopyToDataTable() : tblVT.Select("TenVT = '" + tblBOM.Rows[i]["TenVatTu"] + "' and Mau = '" + tblBOM.Rows[i]["Mau"] + "' and SizeKho = '" + tblBOM.Rows[i]["KhoVai"] + "'").CopyToDataTable();
                        Random random = new Random();
                        string uniqueCode = /*DateTime.UtcNow.Ticks.ToString() +*/"|" + random.Next(0, 99999);


                        DataRow _dr = tblSave.NewRow();
                        if (Convert.ToInt32(tblBOM.Rows[i]["ID"]) != 0)
                        {
                            _dr["ID"] = tblBOM.Rows[i]["ID"];
                            _dr["ID_DM"] = tblBOM.Rows[i]["ID_DM"];
                            _dr["MaVTMau"] = tblBOM.Rows[i]["MaVTMau"];
                            _dr["TiLeSP"] = tblBOM.Rows[i]["TiLeSP"].ToString() == "" ? 0 : tblBOM.Rows[i]["TiLeSP"];
                            _dr["ChieuDai"] = tblBOM.Rows[i]["ChieuDai"].ToString() == "" ? 0 : tblBOM.Rows[i]["ChieuDai"];
                        }
                        else
                        {
                            _dr["ID"] = 0;
                            _dr["ID_DM"] = ""/*_mahang + "_" + dtTempVatTu.Rows[0]["MaVTMau"].ToString() *//*+ uniqueCode*/;
                            if (dtTempVatTu != null && dtTempVatTu.Rows.Count > 0)
                                _dr["MaVTMau"] = dtTempVatTu.Rows[0]["MaVTMau"];
                            else
                                _dr["MaVTMau"] = "";
                            _dr["TiLeSP"] = (bool)chkDMSize.Checked == false ? 0 : tblBOM.Rows[i]["TiLeSP"];
                            _dr["ChieuDai"] = (bool)chkDMSize.Checked == false ? 0 : tblBOM.Rows[i]["ChieuDai"];
                        }

                        _dr["MaBom"] = tblBOM.Rows[i]["MaBom"];
                        _dr["MaDH"] = tblBOM.Rows[i]["MaDH"];
                        _dr["MaVatTu"] = tblBOM.Rows[i]["MaVatTu"];
                        _dr["TenVatTu"] = tblBOM.Rows[i]["TenVatTu"];
                        _dr["Mau"] = tblBOM.Rows[i]["Mau"];
                        _dr["KhoVai"] = tblBOM.Rows[i]["KhoVai"];
                        _dr["DonViTinh"] = tblBOM.Rows[i]["DonViTinh"];
                        _dr["DinhMucMuaHang"] = tblBOM.Rows[i]["DinhMucMuaHang"];
                        _dr["DinhMucThucTe"] = tblBOM.Rows[i]["DinhMucThucTe"];
                        _dr["DinhMucKH"] = tblBOM.Rows[i]["DinhMucKH"];
                        _dr["GhiChu"] = tblBOM.Rows[i]["GhiChu"];
                        _dr["NgayLap"] = DateTime.Now;
                        _dr["MaSP"] = "";
                        _dr["MaTiLe"] = "";
                        _dr["SLChungTu"] = 0;
                        _dr["PhanLoaiVT"] = tblBOM.Rows[i]["MaNhomVT"];
                        _dr["NhaCungCap"] = "";
                        _dr["POID"] = "";
                        _dr["MaMau"] = tblBOM.Rows[i]["MaMau"].ToString() == "" ? "ALL" : tblBOM.Rows[i]["MaMau"];
                        _dr["DauSizeID"] = tblBOM.Rows[i]["DauSizeID"].ToString() == "" ? "ALL" : tblBOM.Rows[i]["DauSizeID"];
                        _dr["SizeID"] = tblBOM.Rows[i]["SizeID"].ToString() == "" ? "ALL" : tblBOM.Rows[i]["SizeID"];
                        _dr["SLSanPham"] = 0;
                        _dr["SLDinhMuc"] = 0;
                        _dr["HaoHut"] = 0;
                        _dr["SL"] = 0;
                        _dr["SoChungTu"] = "";
                        _dr["SanPham"] = 0;
                        _dr["ChieuDaiCuon"] = 0;
                        _dr["SoLuongCan"] = 0;
                        _dr["SLCanBangMau"] = 0;
                        _dr["DonGia"] = 0;
                        _dr["ThanhTien"] = 0;
                        _dr["SLThucTe"] = 0;
                        _dr["SLDat"] = 0;
                        _dr["Chon"] = 0;
                        _dr["NgayCN"] = DateTime.Now;
                        _dr["MaKeToan"] = "";
                        _dr["Dot"] = tblBOM.Rows[i]["Dot"].ToString();
                        _dr["NguoiTao"] = GlobleData.UserName;
                        _dr["IsXacNhan"] = tblBOM.Rows[i]["IsXacNhan"].ToString().ToLower()=="true"? true:false;
                        // Thêm dòng vào DataTable
                        tblSave.Rows.Add(_dr);
                    }
                    if(checkDuplicateRow(tblSave))
                    { tblSave.Clear(); return; }    
                    
                    string urlSaveBOMDH = string.Format("{0}?", URL + "BOM/PostBomVT");
                    string mssSaveBOMDH = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveBOMDH, tblSave); }).Result;
                    if (mssSaveBOMDH.ToLower() != "true")
                        XtraMessageBox.Show(mssSaveBOMDH);
                    else
                        clsWaitForm.ShowSuccessForm(this, 3000);

                    tblSave.Clear();
                    tblBOM.Clear();
                    TimKiem();
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool checkDuplicateRow(DataTable tbl)
        {
            if (tbl == null || tbl.Rows.Count == 0)
            {
                return false; 
            }
            var seenRows = new HashSet<string>();
            foreach (DataRow row in tbl.Rows)
            {
                string rowSignature = $"{row["MaVatTu"]}_{row["Mau"]}_{row["KhoVai"]}_{row["Dot"]}";
                if (seenRows.Contains(rowSignature))
                {
                    XtraMessageBox.Show($"Trùng lặp: MaVatTu = {row["MaVatTu"]}, Mau = {row["Mau"]}, KhoVai = {row["KhoVai"]}, Dot = {row["Dot"]}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
                seenRows.Add(rowSignature);
            }
            return false;
        }
        private void txtGhiChu_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtSoVoice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtMaHS_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
        private void searchLookUpEdit_VatTu_EditValueChanged(object sender, EventArgs e)
        {
            DataRow dr = searchLookUpEdit4View.GetFocusedDataRow();

            if (searchLookUpEdit_VatTu.EditValue is DataRowView)
            {
                gridCheckMarks_VatTu.Selection.Add(((DataRowView)searchLookUpEdit_VatTu.EditValue));
            }
            else
            {
                searchLookUpEdit_VatTu.Properties.Appearance.ForeColor = Color.DarkBlue;
            }

            StringBuilder sb = new StringBuilder();
            tblSelectedVT = tblTenVT.Clone();
            tblSelectedVT.Rows.Clear();
            foreach (DataRowView rv in gridCheckMarks_VatTu.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append(";"); }
                sb.Append(rv["MaVT"].ToString());
                tblSelectedVT.Rows.Add(rv);
            }
            
            searchLookUpEdit_VatTu.Text = sb.ToString();
            _mavt = searchLookUpEdit_VatTu.EditValue.ToString();
        }
        void searchLookUpEdit_VatTu_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {
                if (sb.ToString().Length > 0) {
                    sb.Append("; ");
                    sb1.Append("; ");
                }
                sb.Append(rv["TenVT"].ToString());
                sb1.Append(rv["Nhom"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chưa chọn Vật tư----";
                _nhomSelected = "";
            }
            else
            {
                e.DisplayText = sb.ToString();
                _nhomSelected = sb1.ToString();
            }
            //e.DisplayText = sb.ToString();
        }
        void searchLookUpEdit_VatTu_SelectionChanged(object sender, EventArgs e)
        {
            //Control c = this.ActiveControl;
            Control c = FindSearchLookUpEditInLayout(this.ActiveControl);

            //if (c is DevExpress.XtraLayout.LayoutControl)
            //{
            //    if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
            //    {
            //        c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
            //    }
            //}
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["MaVT"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
        }
        private Control FindSearchLookUpEditInLayout(Control control)
        {
            // Kiểm tra null
            if (control == null)
                return null;

            // Nếu control hiện tại là SearchLookUpEdit, trả về nó
            if (control is DevExpress.XtraEditors.SearchLookUpEdit)
                return control;

            // Nếu control là LayoutControl, lặp qua các item của nó
            if (control is DevExpress.XtraLayout.LayoutControl layoutControl)
            {
                foreach (var item in layoutControl.Items)
                {
                    if (item is DevExpress.XtraLayout.LayoutControlItem layoutItem && layoutItem.Control != null)
                    {
                        // Tìm SearchLookUpEdit trong LayoutControlItem
                        var result = FindSearchLookUpEditInLayout(layoutItem.Control);
                        if (result != null)
                            return result;
                    }
                }
            }

            // Nếu control có ActiveControl, tiếp tục tìm
            if (control is ContainerControl containerControl && containerControl.ActiveControl != null)
            {
                return FindSearchLookUpEditInLayout(containerControl.ActiveControl);
            }

            // Không tìm thấy
            return null;
        }
        
        private void btn_Save_Click(object sender, EventArgs e)
        {
            BtSave();
            cbBoxDot.EditValue = null;
            cbBoxDot.Text = string.Empty;
        }

        private void btn_TimKiem_Click(object sender, EventArgs e)
        {
            TimKiem();
            LocDuLieu();
        }
        private void TimKiem()
        {
           
            string urlTK = string.Format("{0}?_mahang={1}", URL + "BOM/Get_TikKiem", _mahang);
            string jsonTK = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTK); }).Result;

            tblBOM = JsonConvert.DeserializeObject<DataTable>(jsonTK);
            CreateLookUpDot(tblBOM);
            tblBOM.Columns.Add("IsXacNhanText", typeof(string));

            if(tblBOM.Rows.Count>0)
            {
                foreach (DataRow row in tblBOM.Rows)
                {
                    row["IsXacNhanText"] = row["IsXacNhan"] != DBNull.Value && Convert.ToBoolean(row["IsXacNhan"])? "Xác nhận": "Chưa xác nhận";
                }
            }    
            
            if (tblBOM.Rows.Count == 0)
            {
                creatTable_BOM();
            }

            for (int i = 0; i < tblBOM.Rows.Count; i++)
            {
                if (!_mavt.Contains(tblBOM.Rows[i]["MaVTMau"].ToString()))
                {
                    _mavt = _mavt != "" ? _mavt + ";" + tblBOM.Rows[i]["MaVTMau"].ToString() : tblBOM.Rows[i]["MaVTMau"].ToString();
                }
            }

            LoadDataSource();
        }
       
        private void btn_xoa_Click(object sender, EventArgs e)
        {
            DELETE(_mahang, "");
        }
        private void DELETE(string praMaha, string paraMavt)
        {

            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa lệnh này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    DataRow row = gridView1.GetFocusedDataRow();
                    if (row == null) return;
                    string _id = row["ID"].ToString();
                    if (_id == "0")
                    {
                        //int focusedRowHandle = gridView1.FocusedRowHandle; // Lấy chỉ số dòng đang chọn
                        //if (focusedRowHandle >= 0) // Đảm bảo chỉ số hợp lệư
                        //{
                        //    gridView1.DeleteRow(focusedRowHandle); // Xóa dòng
                        //    tblBOM.Rows.RemoveAt(focusedRowHandle);
                        //}

                        DataTable tbl = gridCtr_ChiTiet.DataSource as DataTable;
                        tbl.Rows.Remove(row);
                    }
                    else
                    {
                        if (paraMavt == null|| paraMavt == "")
                        {
                            foreach (DataRow row1 in ((DataTable)gridCtr_ChiTiet.DataSource).Rows)
                            {
                                if (row1["IsXacNhan"].ToString().ToLower() == "true")
                                {
                                    XtraMessageBox.Show("Có vật tư được xác nhận. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                       
                        if (checkDHTongCanDoi(praMaha, row))
                        {
                            string url = string.Format("{0}/?_mahang={1}&&_mavt={2}", URL + "BOM/DeleteBomNPL", praMaha, paraMavt);
                            string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;

                            TimKiem();
                        }
                        else
                        {
                            XtraMessageBox.Show("Vật tư đã được cấp phát. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }

                    LoadDataSource();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool checkDHTongCanDoi(string praMaha, DataRow row)
        {
            string urlDinhMucNPL = string.Format("{0}?_mahang={1}", URL + "BOM/GET_DinhMucNPL", praMaha);
            string jsonDinhMucNPL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDinhMucNPL); }).Result;
            DataTable tblDinhMucNPL = JsonConvert.DeserializeObject<DataTable>(jsonDinhMucNPL);
            if(tblDinhMucNPL.Rows.Count>0)
            {
            foreach (DataRow tblRow in tblDinhMucNPL.Rows)
            {             
                if (tblRow["MaVT"].ToString() == row["MaVatTu"].ToString() &&
                    tblRow["MaNPL"].ToString() == row["ID_DM"].ToString())
                {
                   
                    return false;
                }
            }
            }
            return true;
        }
       
        private void colBtn_Xoa_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if(row["IsXacNhanText"].ToString()=="Xác nhận")
            {
                XtraMessageBox.Show("Vật tư đã được Xác nhận. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }    
            string _idDeleteRow = row["ID_DM"].ToString();
            DELETE(_mahang, _idDeleteRow);
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {

            GridView view = sender as GridView;


            object idDM = null;
            if (view.IsGroupRow(e.RowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                idDM = view.GetRowCellValue(childHandle, colMaVatTu);
            }
            else
            {
                //idDM = view.GetFocusedRowCellValue(colMaVatTu);
                idDM = view.GetRowCellValue(e.RowHandle, colMaVatTu);
            }

            if (idDM == null || string.IsNullOrWhiteSpace(idDM.ToString()))
            {
                // Nếu không có giá trị ID_DM, không thực hiện gì thêm
                return;
            }

            // Nếu là cột "Mau"
            if (e.Column.FieldName == "Mau")
            {
                idDM = HttpUtility.UrlEncode(idDM.ToString());
                var _khosize = gridView1.GetRowCellValue(e.RowHandle, "KhoVai").ToString();
                // Gọi API để lấy danh sách dữ liệu dựa trên ID_DM
                
                string urlMauVaTu=string.Empty;
                if (_khosize !="")
                {
                     urlMauVaTu = string.Format("{0}?_mavt={1}&&_khosize={2}", URL + "BOM/GET_MaubyKhoSize", idDM, _khosize);
                }
                else
                {
                     urlMauVaTu = string.Format("{0}?_mavt={1}", URL + "BOM/GET_MauVatTu", idDM);
                }

                string jsonMauVatTu = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMauVaTu); }).Result;
                DataTable tblMauVatTu = JsonConvert.DeserializeObject<DataTable>(jsonMauVatTu);

                // Kiểm tra dữ liệu trả về
                if (tblMauVatTu == null || tblMauVatTu.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu cho cột Mau.");
                    return;
                }

                // Tạo RepositoryItemSearchLookUpEdit cho cột "Mau"
                RepositoryItemSearchLookUpEdit rMauVTEdit = new RepositoryItemSearchLookUpEdit
                {
                    DataSource = tblMauVatTu,
                    DisplayMember = "Mau",
                    ValueMember = "Mau",
                    ShowClearButton = false,
                    NullText = "[Chọn giá trị]"
                };

                // Cấu hình popup view
                GridView popupView = rMauVTEdit.View;
                popupView.Columns.Clear();
                popupView.Columns.Add(new GridColumn { FieldName = "Mau", Caption = "Màu", Visible = true });

                // Gán RepositoryItemSearchLookUpEdit vào cột "Mau"
                e.RepositoryItem = rMauVTEdit;
            }

            // Nếu là cột "KhoVai"
            else if (e.Column.FieldName == "KhoVai")
            {
                idDM = HttpUtility.UrlEncode(idDM.ToString());
                var _mau = gridView1.GetRowCellValue(e.RowHandle, "Mau").ToString();
                string urlKhoSize = string.Empty;
                if (_mau != "")
                {
                    urlKhoSize = string.Format("{0}?_mavt={1}&&_mau={2}", URL + "BOM/GET_KhoSizebyMau", idDM, _mau);
                }
                else
                {
                    urlKhoSize = string.Format("{0}?_mavt={1}", URL + "BOM/GET_KhoSizeVatTu", idDM);
                }
                //setSearchLookUp(a, true, idDM.ToString(), e);
                // Gọi API để lấy danh sách dữ liệu dựa trên ID_DM
                //string urlKhoSize = string.Format("{0}?_mavt={1}&&_mau={2}", URL + "BOM/GET_KhoSizebyMau", idDM,_mau);
                //string urlKhoSize = string.Format("{0}?_mavt={1}", URL + "BOM/GET_KhoSizeVatTu", idDM);
                string jsonKhoSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhoSize); }).Result;
                DataTable tblKhoSize = JsonConvert.DeserializeObject<DataTable>(jsonKhoSize);

                // Kiểm tra dữ liệu trả về
                if (tblKhoSize == null || tblKhoSize.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu cho cột KhoVai.");
                    return;
                }

                // Tạo RepositoryItemSearchLookUpEdit cho cột "KhoVai"
                RepositoryItemSearchLookUpEdit rKhoSizeEdit = new RepositoryItemSearchLookUpEdit
                {
                    DataSource = tblKhoSize,
                    DisplayMember = "KhoVai",
                    ValueMember = "KhoVai",
                    ShowClearButton = false,
                    NullText = "[Chọn giá trị]"
                };

                // Cấu hình popup view
                GridView popupView = rKhoSizeEdit.View;
                popupView.Columns.Clear();
                popupView.Columns.Add(new GridColumn { FieldName = "KhoVai", Caption = "Kho Vải", Visible = true });

                // Gán RepositoryItemSearchLookUpEdit vào cột "KhoVai"
                e.RepositoryItem = rKhoSizeEdit;
            }
        }

        private void colBtn_Tach_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            int rowIndex = gridView1.FocusedRowHandle;
            TachDong(rowIndex);
        }
        private void TachDong(int _idx)
        {
            DataRow checkChange = null;
            //DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle) as DataRow;
            DataRow row = gridView1.GetDataRow(_idx) as DataRow;
            if (row == null) return;
            int rowIndex = FindRowIndexByName(tblBOM, row["MaVtMau"].ToString()); // lấy ra index của bàn hiện tại trong dataTbale

            //row["STT"] = sttValuesList[rowIndex+1];
            if (rowIndex != -1)
            {
                if (row.ItemArray[2].ToString() == "") return;
                // Tạo một bản sao của DataRow
                DataRow newRow = tblBOM.NewRow();
                newRow.ItemArray = row.ItemArray;


                Random random = new Random();
                string uniqueCode = /*DateTime.UtcNow.Ticks.ToString() +*/"|" + random.Next(1000, 9999);

                //DataRow _dr = tblBOM.NewRow();
                newRow["ID"] = 0;
                newRow["ID_DM"] = ""/*_mahang + "_" + newRow["MaVTMau"] *//*+ uniqueCode*/;

                // Thêm dòng mới vào DataTable tại vị trí đã tìm được
                tblBOM.Rows.InsertAt(newRow, rowIndex + 1);
            }

            //update lại số bàn
            //for (int i = 0; i < tblBOM.Rows.Count; i++)
            //{
            //    if (i == sttValuesList.Count)
            //    {
            //        tblBOM.Rows[i]["STT"] = i + 1;
            //    }
            //    else
            //    {
            //        tblBOM.Rows[i]["STT"] = sttValuesList[i];
            //    }

            //}
        }
        //
        private int FindRowIndexByName(DataTable _dtTemp, string name)
        {
            // Sử dụng LINQ để tìm index của hàng
            var rowIndex = _dtTemp.AsEnumerable()
                .Select((row, index) => new { Row = row, Index = index })
                .FirstOrDefault(item => item.Row["MaVtMau"].ToString() == name)?.Index ?? -1;

            return rowIndex;
        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column == colSTT)
            {
                if (e.RowHandle < 0)
                {
                    e.DisplayText = "0";
                }
                else
                {
                    string sttValue = Convert.ToString(e.RowHandle + 1);
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);

                }
            }
        }
        
        private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa lệnh này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                //gridView1.SetRowCellValue(0, colTesst, "Giá trị mới");

            }
        }
        
        private void colBtn_InSeam_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            frmChonDauSize_Size frm = new frmChonDauSize_Size(true, _mahang);
            //frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();

            // Lấy giá trị từ property Data sau khi form con đóng
            string receivedData = frm.Data;

            // Xử lý dữ liệu (ví dụ: hiển thị dữ liệu vừa nhận)
            if (!string.IsNullOrEmpty(receivedData))
            {
                //MessageBox.Show($"Dữ liệu nhận được từ FormCon: {receivedData}");

                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, colDauSizeID, receivedData);
            }
            //else
            //{
            //    MessageBox.Show("Không có dữ liệu nào được nhập!");
            //}
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MauDH.EditValue != null && searchLookUpEdit_MauDH2.EditValue != null)
            {
                //tblBOM
                if (tblBOM.Select("MaMau = '" + searchLookUpEdit_MauDH.Text.ToString() + "'").Length == 0)
                {
                    return;
                }
                DataTable dtTempCopy = tblBOM.Select("MaMau = '" + searchLookUpEdit_MauDH.Text.ToString() + "'").CopyToDataTable();
                foreach (DataRow row in dtTempCopy.Rows)
                {
                    Random random = new Random();
                    string uniqueCode = /*DateTime.UtcNow.Ticks.ToString() +*/"|" + random.Next(0, 99999);

                    DataRow newRow = tblBOM.NewRow();
                    // Sao chép giá trị từ row trong dtTempCopy sang newRow
                    newRow.ItemArray = row.ItemArray.Clone() as object[];
                    newRow["ID"] = 0;
                    newRow["ID_DM"] = _mahang + "_" + row["MaVTMau"].ToString() /*+ uniqueCode*/;
                    newRow["MaMau"] = searchLookUpEdit_MauDH2.Text.ToString();
                    // Thêm dòng mới vào tblBOM
                    tblBOM.Rows.Add(newRow);
                }
                LoadDataSource();
            }
        }

        private void searchLookUpEdit_MauDH_EditValueChanged(object sender, EventArgs e)
        {

            //searchLookUpEdit_MauDH2
            if (searchLookUpEdit_MauDH.EditValue != null)
            {
                string urlMau_DH = string.Format("{0}?_mahang={1}", URL + "BOM/GET_Mau", _mahang);
                string jsonMau_DH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau_DH); }).Result;
                DataTable tblMau_DH = JsonConvert.DeserializeObject<DataTable>(jsonMau_DH);
                DataTable dtTempMau2 = tblMau_DH.Select("MaMau <> '" + searchLookUpEdit_MauDH.EditValue.ToString() + "'").CopyToDataTable();

                searchLookUpEdit_MauDH2.Properties.DataSource = dtTempMau2;
                searchLookUpEdit_MauDH2.Properties.ValueMember = "MaMau";
                searchLookUpEdit_MauDH2.Properties.DisplayMember = "TenMau";
            }

        }

        private void colBtn_size_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            frmChonDauSize_Size frm = new frmChonDauSize_Size(false, _mahang);
            //frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();

            // Lấy giá trị từ property Data sau khi form con đóng
            string receivedData = frm.Data;

            // Xử lý dữ liệu (ví dụ: hiển thị dữ liệu vừa nhận)
            if (!string.IsNullOrEmpty(receivedData))
            {
                //MessageBox.Show($"Dữ liệu nhận được từ FormCon: {receivedData}");

                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, colSizeID, receivedData);
            }
        }

        private void txtDot_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
        private void Xoa_ItemClick(object sender, ItemClickEventArgs e)
        {


        }

        private void btnQuyDoi_Click(object sender, EventArgs e)
        {
            frmQuyDoi frm = new frmQuyDoi();
            frm.ShowDialog();
        }

        private void searchLookUpEdit_MH_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MH.EditValue != null)
            {
                _mahang = searchLookUpEdit_MH.EditValue.ToString();
                string urlMau_DH = string.Format("{0}?_mahang={1}", URL + "BOM/GET_Mau", _mahang);
                string jsonMau_DH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau_DH); }).Result;
                DataTable tblMau_DH = JsonConvert.DeserializeObject<DataTable>(jsonMau_DH);


                searchLookUpEdit_MauDH.Properties.DataSource = tblMau_DH;
                searchLookUpEdit_MauDH.Properties.ValueMember = "MaMau";
                searchLookUpEdit_MauDH.Properties.DisplayMember = "TenMau";
            }
            TimKiem();
        }


        private void btn_Them_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbBoxDot.EditValue == null || cbBoxDot.EditValue == "")
                {
                    XtraMessageBox.Show("Vui lòng nhập đợt.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string idmavt = string.Empty;
                if (string.IsNullOrEmpty(searchLookUpEdit_MH.EditValue.ToString()) || string.IsNullOrEmpty(searchLookUpEdit_VatTu.EditValue.ToString()))
                {
                    return;
                }
                else
                {
                    idmavt = searchLookUpEdit_VatTu.EditValue.ToString();
                }
                string[] arrDataVatTu = idmavt.Split(new char[] { ';' });
                string[] arrNhom = _nhomSelected.Split(';');
                if ((bool)chkDMSize.Checked == false)
                {
                    for (int i = 0; i < arrDataVatTu.Count(); i++)
                    {

                        var a = tblSelectedVT;
                        string urlVT = string.Format("{0}?", URL + "BOM/GET_TenVatTu");
                        string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                        tblVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);

                        DataTable dtTempVatTu = tblTenVT.AsEnumerable().Where(x => x["MaVT"].ToString() == arrDataVatTu[i] && x["Nhom"].ToString() == arrNhom[i].ToString().Trim()).CopyToDataTable();

                        Random random = new Random();
                        string uniqueCode = /*DateTime.UtcNow.Ticks.ToString() +*/"|" + random.Next(0, 99999);

                        DataRow _dr = tblBOM.NewRow();
                        _dr["ID"] = 0;
                        //_dr["ID_DM"] = _mahang + "_" + arrDataVatTu[i].ToString() + uniqueCode;
                        _dr["MaBom"] = _mahang + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day;
                        _dr["MaDH"] = _mahang;
                        //_dr["MaVtMau"] = arrDataVatTu[i].ToString();
                        _dr["MaVatTu"] = dtTempVatTu.Rows[0]["MaVT"];
                        _dr["TenVatTu"] = dtTempVatTu.Rows[0]["TenVT"];
                        //_dr["KhoVai"] = "";
                        //_dr["MaNhomVT"] = "";
                        //_dr["Mau"] = "";
                        _dr["DonViTinh"] = dtTempVatTu.Rows[0]["DVTinh"];
                        _dr["DinhMucMuaHang"] = 0;
                        _dr["DinhMucThucTe"] = 0;
                        _dr["DinhMucKH"] = 0;
                        _dr["GhiChu"] = "";
                        //_dr["SPOID"] = search;
                        //_dr["PO"] = 0;
                        //_dr["MaMau"] = "";

                        //_dr["MaNhomVT"] = dtTempVatTu.Rows[0]["MaNhomVT"];
                        _dr["MaMau"] = searchLookUpEdit_MauDH.EditValue == null ? "ALL" : searchLookUpEdit_MauDH.Text.ToString();
                        //_dr["DauSizeID"] = "";
                        //_dr["NhomSize"] = "";
                        //_dr["MaSize"] = "";
                        //_dr["SizeID"] = "";
                        _dr["Nhom"] = dtTempVatTu.Rows[0]["Nhom"];
                        _dr["Dot"] = cbBoxDot.EditValue.ToString();
                        _dr["NguoiTao"] = GlobleData.UserName;
                        _dr["IsXacNhan"] = false;
                        _dr["IsXacNhanText"] = "Chưa xác nhận";
                        tblBOM.Rows.Add(_dr);
                    }
                    LoadDataSource();
                }
                else
                {
                    string urlSize = string.Format("{0}?_mahang={1}", URL + "BOM/GET_Size", searchLookUpEdit_MH.EditValue.ToString());
                    string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                    DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                    foreach (DataRow dr in tblSize.Rows)
                    {
                        for (int i = 0; i < arrDataVatTu.Count(); i++)
                        {
                            DataTable dtTempVatTu = tblTenVT.Select("MaVT = '" + arrDataVatTu[i] + "'").CopyToDataTable();
                            Random random = new Random();
                            string uniqueCode = "|" + random.Next(0, 99999);
                            DataRow _dr = tblBOM.NewRow();
                            _dr["ID"] = 0;
                            _dr["MaBom"] = _mahang + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day;
                            _dr["MaDH"] = _mahang;
                            _dr["MaVatTu"] = dtTempVatTu.Rows[0]["MaVT"];
                            _dr["TenVatTu"] = dtTempVatTu.Rows[0]["TenVT"];
                            _dr["DonViTinh"] = dtTempVatTu.Rows[0]["DVTinh"];
                            _dr["DinhMucMuaHang"] = 0;
                            _dr["DinhMucThucTe"] = 0;
                            _dr["DinhMucKH"] = 0;
                            _dr["GhiChu"] = "";
                            _dr["SizeID"] = dr["TenSize"].ToString();
                            _dr["MaMau"] = searchLookUpEdit_MauDH.EditValue == null ? "ALL" : searchLookUpEdit_MauDH.Text.ToString();
                            _dr["Nhom"] = dtTempVatTu.Rows[0]["Nhom"];
                            _dr["Dot"] = cbBoxDot.EditValue.ToString();
                            _dr["NguoiTao"] = GlobleData.UserName;
                            _dr["IsXacNhan"] = false;
                            _dr["IsXacNhanText"] = "Chưa xác nhận";
                            tblBOM.Rows.Add(_dr);
                        }
                    }
                    LoadDataSource();
                }


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BtNapLai()
        {

            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void Naplai_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            BtNapLai();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
           /* DataTable a = tblBOM;
            if (e.Column.FieldName == "IsXacNhan")
            {
               
                    e.DisplayText = e.Value?.ToString()=="true" ? "Xác nhận" : "Chưa xác nhận";
                
            }*/
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "IsXacNhan" && e.IsGetData)
            {
                // Lấy giá trị từ cột "IsXacNhan" và chuyển đổi
                bool isXacNhan = Convert.ToBoolean(((DataRowView)e.Row)["IsXacNhan"]);
                e.Value = isXacNhan ? "Xác nhận" : "Chưa xác nhận";
            }
        }
        
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            //kiểm tra quyền
            string urlPQ = string.Format("{0}?userID={1}", URL + "PhanQuyenXacNhanBOM/Get",GlobleData.UserName);
            string jsonPQ = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPQ); }).Result;
            DataTable pq = JsonConvert.DeserializeObject<DataTable>(jsonPQ);

            if(pq.Rows[0]["AllowEdit"].ToString() =="False")
            {
                XtraMessageBox.Show("Người dùng "+ GlobleData.UserName +" không có quyền Xác nhân B.O.M.!\n Vui lòng thử lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //
            if (_mahang != string.Empty)
            {
                using (frmXacNhanBOM_MaHang frm = new frmXacNhanBOM_MaHang(_mahang))
                {
                    frm.WindowState = FormWindowState.Maximized;

                    // Gắn thêm sự kiện FormClosing
                    frm.FormClosing += (s, args) =>
                    {
                        // Gọi hàm TimKiem() khi form bị đóng
                        TimKiem();
                    };

                    // Hiển thị form
                    frm.ShowDialog();
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
     
        private void chkDMSize_CheckedChanged(object sender, EventArgs e)
        {
            colQuyDoi.Visible = (bool)chkDMSize.Checked;
            colChieuDai.Visible = (bool)chkDMSize.Checked;
            colTiLeSP.Visible = (bool)chkDMSize.Checked;
            if ((bool)chkDMSize.Checked)
                RestoreColumnOrder();
        }

        private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row != null)
            {
                txtNguoiTao.Text = row["NguoiTao"].ToString();
            }
        }

        private void SaveColumnOrder()
        {
            columnOrderList.Clear();
            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                columnOrderList.Add(gridView1.Columns[i].VisibleIndex);
            }
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "IsXacNhanText")
            {
                string cellValue = e.CellValue?.ToString();
                if (cellValue == "Xác nhận")
                {
                    e.Appearance.ForeColor = Color.Green;
                }

                else if (cellValue == "Chưa xác nhận")
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }

        private void RestoreColumnOrder()
        {
            if (columnOrderList.Count != gridView1.Columns.Count)
                return;

            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                gridView1.Columns[i].VisibleIndex = columnOrderList[i];
            }
        }

        private void btnCopyDH_Click(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MH.EditValue.ToString() == "") return;
            frmCopyBOMMaHang frm = new frmCopyBOMMaHang(searchLookUpEdit_MH.EditValue.ToString());
            frm.ShowDialog();

        }
        private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //if (e.Column.FieldName == "Mau")
            //{
            //    var a = gridView1.GetRowCellValue(e.RowHandle, "Mau").ToString();
            //    setSearchLookUp(a,true);
            //}
            if (e.Column.FieldName == "TiLeSP" || e.Column.FieldName == "ChieuDai" | e.Column.FieldName == "ID")
            {
                var tilesp = gridView1.GetRowCellValue(e.RowHandle, "TiLeSP");
                var chieudai = gridView1.GetRowCellValue(e.RowHandle, "ChieuDai");
                var quyDoi = gridView1.GetRowCellValue(e.RowHandle, colQuyDoi);
                int quyDoiID = Convert.ToInt32(quyDoi);
                DataRow[] rows = tblQD.Select($"ID = {quyDoiID}");
                if (tilesp != null && chieudai != null && tilesp.ToString() != "" && tilesp.ToString() != "" && chieudai.ToString() != "" && Convert.ToDecimal(tilesp) != 0)
                {
                    if (rows.Length == 1)
                    {
                        switch (rows[0]["QuyDoi"].ToString())
                        {
                            case "Inch -> Yard":
                                decimal soLuong = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc = Math.Round((Convert.ToDecimal(chieudai) * soLuong) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc);
                                break;
                            case "Inch -> Mét":
                                decimal soLuong1 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc1 = Math.Round((Convert.ToDecimal(chieudai) * soLuong1) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc1);
                                break;
                            case "Yard -> Inch":
                                decimal soLuong2 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc2 = Math.Round((Convert.ToDecimal(chieudai) / soLuong2) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc2);
                                break;
                            case "Yard -> Mét":
                                decimal soLuong3 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc3 = Math.Round((Convert.ToDecimal(chieudai) * soLuong3) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc3);
                                break;
                            case "Mét -> Inch":
                                decimal soLuong4 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc4 = Math.Round((Convert.ToDecimal(chieudai) * soLuong4) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc4);
                                break;
                            case "Mét -> Yard":
                                decimal soLuong5 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc5 = Math.Round((Convert.ToDecimal(chieudai) * soLuong5) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc5);
                                break;
                            case "Yard -> Centimet":
                                decimal soLuong6 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc6 = Math.Round((Convert.ToDecimal(chieudai) * soLuong6) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc6);
                                break;
                            case "CM -> Yard":
                                decimal soLuong7 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc7 = Math.Round((Convert.ToDecimal(chieudai) / soLuong7) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc7);
                                break;
                            case "CM -> Inch":
                                decimal soLuong8 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc8 = Math.Round((Convert.ToDecimal(chieudai) / soLuong8) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc8);
                                break;
                            case "CM -> Mét":
                                decimal soLuong9 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc9 = Math.Round((Convert.ToDecimal(chieudai) / soLuong9) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc9);
                                break;
                            case "Mét -> Centimet":
                                decimal soLuong10 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc10 = Math.Round((Convert.ToDecimal(chieudai) * soLuong10) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc10);
                                break;
                            case "Inch -> CM":
                                decimal soLuong11 = Convert.ToDecimal(rows[0]["SoLuong"]);
                                decimal dinhmuc11 = Math.Round((Convert.ToDecimal(chieudai) * soLuong11) / Convert.ToDecimal(tilesp), 3);
                                gridView1.SetRowCellValue(e.RowHandle, "DinhMucThucTe", dinhmuc11);
                                break;
                        }
                    }
                }
            }
        }


        private void gridView1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if (gridView1.FocusedColumn.FieldName == "TiLeSP" || gridView1.FocusedColumn.FieldName == "ChieuDai")
            {
                string enteredValue = e.Value.ToString();
                if (!decimal.TryParse(enteredValue, out decimal result))
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng phải là một số hợp lệ.";
                }
                else
                {
                    e.Valid = true;
                }
            }
        }
        private void LocDuLieu()
        {
            
            string dotNhap = cbBoxDot.EditValue?.ToString();
   
            // Tạo điều kiện lọc
            string filterCondition = "";

            if (!string.IsNullOrEmpty(dotNhap))
            {
                filterCondition += $"[Dot] = '{dotNhap}'";
            }


            if (string.IsNullOrEmpty(filterCondition))
            {
                (gridCtr_ChiTiet.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
            }
            else
            {
                (gridCtr_ChiTiet.DataSource as DataTable).DefaultView.RowFilter = filterCondition;
            }
        }
        private void CreateLookUpDot(DataTable _tblBom)
        {
            if (_tblBom == null) return;
            var distinctDots = _tblBom.AsEnumerable()
                               .Select(row => row.Field<string>("Dot"))
                               .Distinct();
            if (distinctDots == null || !distinctDots.Any())
            {
              
                return;
            }
            if (tblDot == null)
            {
                tblDot = new DataTable();
                tblDot.Columns.Add("Dot", typeof(string));
            }
            tblDot.Rows.Clear();
            foreach (var dot in distinctDots)
            {
                tblDot.Rows.Add(dot);
            }
            cbBoxDot.Properties.Items.Clear();
            foreach (DataRow row in tblDot.Rows)
            {
                cbBoxDot.Properties.Items.Add(row["Dot"].ToString());
            }
            cbBoxDot.Properties.NullText = "--Chọn đợt--";
            if (tblDot.Rows.Count > 0)
            {
                cbBoxDot.EditValue = tblDot.Rows[0]["Dot"];
            }
            else
            {
                cbBoxDot.EditValue = null;
            }

        }
    }
}
