using DevExpress.Data;
using DevExpress.DataAccess.Excel;
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
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmNhapDonHangTong : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        DataTable _dtTable;
        DataTable _dtSize;
        DataTable _dtSizeNhom;
        DataTable grid;
        private int currentPhieuNumber = 1;
        int SoDonHang = 0;
        int sum = 0, sort = 0;
        int check = 0;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _xacnhan = false;
        string searchMH = string.Empty, searchKH = string.Empty, searchCL = string.Empty, textDot = string.Empty;
        bool indicatorIcon = true;
        string size;
        string mau;
        string dausize;
        int _rowAdd = -1;
        ActionControl ActionControlSave;
        KeyDownControlHandler keyDownControlHandler;

        SearchCheckSelection gridCheckMarksColor;
        SearchCheckSelection gridCheckMarksSize;
        SearchCheckSelection gridCheckMarksDauSize;
        public frmNhapDonHangTong(int SDH, bool allowAdd, bool allowEdit, bool allowDelete)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dtTable = new DataTable();
            _dtSize = new DataTable();
            _dtSizeNhom = new DataTable();
            _allowAdd = allowAdd;
            _allowEdit = allowEdit;
            _allowDelete = allowDelete;
            SoDonHang = SDH;
            this.Luu.Enabled = true;
            _dtTable = CreateDatatable();
            _dtSize = CreateDatatableSize();
            _dtSizeNhom = CreateDatatableSizeNhom();
            checkEditImport.Checked = true;
            checkEditTV.Checked = true;
            //CreateDefaultSearchLookUpHH();
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookupCL();
            CreateSearchLookup();
            CreateDefaultSearchLookUpHH();
            CreateSearchLookupHT();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            string newPhieuNumber = GeneratePhieuNumber();
            txtMaDH.Text = newPhieuNumber;
            dateEditNgayTao.DateTime = DateTime.Now;
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            ActionControlSave = new ActionControl(BtLuu, _allowAdd, ActionType.Save, this.Luu.Enabled);
            _lstActionControl.Add(ActionControlSave);
            return _lstActionControl;
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
        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("MaQG", typeof(string));
            tbl.Columns.Add("NgayGH", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("NgayDKVC", typeof(DateTime));
            tbl.Columns.Add("NgayThucTeVC", typeof(DateTime));
            tbl.Columns.Add("NgayXuatHang", typeof(DateTime));


            return tbl;
        }
        private DataTable CreateDatatableSize()
        {
            DataTable tbl = new DataTable("dtSize");
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeSize", typeof(string));
            tbl.Columns.Add("TenSize", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("XacNhan", typeof(int));
            tbl.Columns.Add("SizeXacNhan", typeof(string));
            return tbl;
        }
        private DataTable CreateDatatableSizeNhom()
        {
            DataTable tbl = new DataTable("dtSizeNhom");
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            return tbl;
        }


        private DataTable CreateDatatableTheSize()
        {
            DataTable tbl = new DataTable("dtSizethe");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeSize", typeof(string));
            tbl.Columns.Add("TenSize", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("XacNhan", typeof(string));
            tbl.Columns.Add("SizeXacNhan", typeof(int));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("Sort", typeof(string));
            tbl.Columns.Add("MaTheSize", typeof(string));
            tbl.Columns.Add("MaTheInSeam", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableTheMau()
        {
            DataTable tbl = new DataTable("dtMauthe");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaTheMau", typeof(string));
            return tbl;
        }

        private void CreateSearchLookupMaMau()
        {
            string json = string.Empty;
            if (checkEditTV.Checked == false)
            {
                string url = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            }
            else
            {
                string url = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            }
            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tblmau;
            rCountryEdit.DisplayMember = "TenMau";
            rCountryEdit.ValueMember = "MaMau";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";
            rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaMau", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên màu", Name = "colTenMau", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "CodeMau", Caption = "Code màu", Name = "colTenMau", Visible = true });

                GridColumn colGroup = new GridColumn
                {
                    FieldName = "TheMau",
                    Caption = "",
                    Name = "colTheMau",
                    Visible = true
                };
                dvView.Columns.Add(colGroup);

                colGroup.GroupIndex = 0;
                dvView.OptionsView.ShowGroupPanel = false;
                dvView.OptionsBehavior.AutoExpandAllGroups = true;
                dvView.GroupFormat = "{1}";

            }
            bandedGridColumn3.ColumnEdit = rCountryEdit;

        }
        private void CreateSearchLookupQG()
        {
            string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            SearchLookUpEditQG.Properties.DataSource = tbl;
            SearchLookUpEditQG.Properties.DisplayMember = "TenQG";
            SearchLookUpEditQG.Properties.ValueMember = "MaQG";
            SearchLookUpEditQG.Properties.ShowClearButton = false;
            SearchLookUpEditQG.Properties.NullText = "Chọn QG";

            GridView dvView = SearchLookUpEditQG.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã QG", Name = "colMaQG", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Quốc gia", Name = "colTenQG", Visible = true });

            }
        }
        private void CreateSearchLookupCL()
        {
            //string urlCL = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetChungLoai", searchLookUpEditMH.EditValue == null ? "" :searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            //string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
            //DataTable tblCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);
            string urlCLL = $"{URL}ChungLoai/GetChungLoai?";
            string jsonCLL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCLL); }).Result;
            DataTable tblcll = JsonConvert.DeserializeObject<DataTable>(jsonCLL);
            searchLookUpEditCL.Properties.DataSource = tblcll;
            //searchLookUpEditCL.Properties.DataSource = tblCL;
            searchLookUpEditCL.Properties.ValueMember = "MaCL";
            searchLookUpEditCL.Properties.DisplayMember = "TenCL";

            if (tblcll != null && tblcll.Rows.Count > 0)
            {
                string urlHH = string.Format("{0}?makh={1}", URL + "DonHangTong/GetHangHoaKH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
                DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
                var rowcl = tblHH.AsEnumerable().FirstOrDefault(x => x["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString());
                if (rowcl != null)
                {
                    searchLookUpEditCL.EditValue = rowcl["MaCL"].ToString();
                }

            }
        }
        private void CreateSearchLookup()
        {
            try
            {
                string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditKH.Properties.DataSource = tblKH;
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";

                string urlMua = string.Format("{0}?", URL + "Mua/Get");
                string jsonMua = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMua); }).Result;
                DataTable tblMua = JsonConvert.DeserializeObject<DataTable>(jsonMua);
                txtDot.Properties.DataSource = tblMua;
                txtDot.Properties.ValueMember = "MaMua";
                txtDot.Properties.DisplayMember = "Mua";
                txtDot.ProcessNewValue += (s, e) =>
                {
                    if (e.DisplayValue == null)
                        return;

                    string txt = e.DisplayValue.ToString();

                    // Thêm nếu chưa có trong datasource
                    DataTable tbl = txtDot.Properties.DataSource as DataTable;
                    DataRow[] found = tbl.Select($"Mua = '{txt.Replace("'", "''")}'");

                    if (found.Length == 0)
                    {
                        DataRow r = tbl.NewRow();
                        r["MaMua"] = txt;
                        r["Mua"] = txt;
                        tbl.Rows.Add(r);
                    }

                    // Gán lại giá trị mới nhập
                    txtDot.EditValue = txt;
                    e.Handled = true;
                };


                string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
                string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
                DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);
                string username = GlobleData.UserName.ToString();
                DataRow ten = tblnv.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("UserID")?.Trim(), username.Trim(), StringComparison.OrdinalIgnoreCase));
                txtNguoiTao.Text = ten != null ? ten["Ten"].ToString() : username.ToString();

                string urlPD = string.Format("{0}?", URL + "PhapDanhCty/Get");
                string jsonPD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPD); }).Result;
                DataTable tblPD = JsonConvert.DeserializeObject<DataTable>(jsonPD);
                searchLookUpEditCTY.Properties.DataSource = tblPD;
                searchLookUpEditCTY.Properties.ValueMember = "MaCty";
                searchLookUpEditCTY.Properties.DisplayMember = "TenCty";
                searchLookUpEditCTY.EditValue = "MDN";

                if (tblKH != null && tblKH.Rows.Count > 0)
                {
                    searchLookUpEditCTY.EditValue = tblPD.Rows[0]["MaCty"];
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateSearchLookupQuocGia()
        {
            string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenQG";
            rCountryEdit.ValueMember = "MaQG";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";
            rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã quốc gia", Name = "colMaQG", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Quốc gia", Name = "colTenQG", Visible = true });

            }
            bandedGridColumn5.ColumnEdit = rCountryEdit;
        }

        //private void CreateSearchLookupMau()
        //{
        //    string url = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(json);
        //    RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
        //    rCountryEdit.DataSource = tblmau;
        //    rCountryEdit.DisplayMember = "MaMau";
        //    rCountryEdit.ValueMember = "TenMau";
        //    rCountryEdit.ShowClearButton = false;
        //    rCountryEdit.NullText = "[Chọn giá trị]";

        //    GridView dvView = rCountryEdit.View;
        //    if (dvView.Columns.Count == 0)
        //    {
        //        dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
        //        dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //        dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        //        dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
        //        dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaQG", Visible = false });
        //        dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên màu", Name = "colTenQG", Visible = true });

        //    }
        //    bandedGridColumn3.ColumnEdit = rCountryEdit;
        //}

        private void CreateSearchLookupDauSize()
        {
            string json = string.Empty;
            if (checkEditTV.Checked == false)
            {
                string url = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                rCountryEdit.DataSource = tbl;
                rCountryEdit.DisplayMember = "InSeam";
                rCountryEdit.ValueMember = "MaInSeam";
                rCountryEdit.ShowClearButton = false;
                rCountryEdit.NullText = "[Chọn giá trị]";
                rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
                rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvView = rCountryEdit.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "InSeam", Caption = "Mã Đầu Size", Name = "colMaQG", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "MaInSeam", Caption = "InSeam", Name = "colTenQG", Visible = true });
                    GridColumn colGroup = new GridColumn
                    {
                        FieldName = "TheInSeam",
                        Caption = "",
                        Name = "colTheMau",
                        Visible = true
                    };
                    dvView.Columns.Add(colGroup);

                    colGroup.GroupIndex = 0;
                    dvView.OptionsView.ShowGroupPanel = false;
                    dvView.OptionsBehavior.AutoExpandAllGroups = true;
                    dvView.GroupFormat = "{1}";
                }
                bandedGridColumn4.ColumnEdit = rCountryEdit;
            }
            else
            {
                string url = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetDauSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                rCountryEdit.DataSource = tbl;
                rCountryEdit.DisplayMember = "DauSize";
                rCountryEdit.ValueMember = "DauSize";
                rCountryEdit.ShowClearButton = false;
                rCountryEdit.NullText = "[Chọn giá trị]";
                rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
                rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvView = rCountryEdit.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "DauSizeID", Caption = "Mã Đầu Size", Name = "colMaQG", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "DauSize", Caption = "InSeam", Name = "colTenQG", Visible = true });
                    GridColumn colGroup = new GridColumn
                    {
                        FieldName = "TheInSeam",
                        Caption = "",
                        Name = "colTheMau",
                        Visible = true
                    };
                    dvView.Columns.Add(colGroup);

                    colGroup.GroupIndex = 0;
                    dvView.OptionsView.ShowGroupPanel = false;
                    dvView.OptionsBehavior.AutoExpandAllGroups = true;
                    dvView.GroupFormat = "{1}";
                }
                bandedGridColumn4.ColumnEdit = rCountryEdit;
            }

        }
        private void frmNhapDonHangTong_Load(object sender, EventArgs e)
        {
            btNhap.Enabled = false;
            txtNhapPO.Enabled = false;
            txtNhapSize.Enabled = false;
            txtDauSize.Enabled = false;
            txtMau.Enabled = false;
            string newPhieuNumber = GeneratePhieuNumber();
            txtMaDH.Text = newPhieuNumber;
            dateEditNgayTao.DateTime = DateTime.Now;

        }
        private string GeneratePhieuNumber()
        {
            currentPhieuNumber = currentPhieuNumber + SoDonHang;
            string formattedPhieuNumber = "DH_" + currentPhieuNumber;//SoDonHang.ToString("D8");
            return formattedPhieuNumber;
        }
        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = gridViewThongTinDonHang;
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            // bandedView.OptionsView.AllowCellMerge = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            if (tab.Columns.Count == 0) return bandedView;
            //for (int i = 0; i < 8; i++)
            //{
            //    List<string> _ListString = new List<string>();
            //    if (tab.Columns[i].ColumnName == "PO")
            //    {
            //        _ListString.Add("PO");
            //        SetGridBandedViewAmount(bandedView, "", "PO", _ListString);
            //        _ListString.Clear();
            //    }
            //    if (tab.Columns[i].ColumnName == "MaMau")
            //    {
            //        _ListString.Add("MaMau");
            //        SetGridBandedViewAmount(bandedView, "", "Màu", _ListString);
            //        _ListString.Clear();
            //    }
            //    if (tab.Columns[i].ColumnName == "DauSize")
            //    {
            //        _ListString.Add("DauSize");
            //        SetGridBandedViewAmount(bandedView, "", "Đầu Size", _ListString);
            //        _ListString.Clear();
            //    }

            //    if (tab.Columns[i].ColumnName == "MaQG")
            //    {
            //        _ListString.Add("MaQG");
            //        SetGridBandedViewAmount(bandedView, "", "Quốc gia", _ListString);
            //        _ListString.Clear();
            //    }

            //    if (tab.Columns[i].ColumnName == "NgayGH")
            //    {
            //        _ListString.Add("NgayGH");
            //        SetGridBandedViewAmount(bandedView, "", "Ngày giao hàng", _ListString);
            //        _ListString.Clear();
            //    }
            //    if (tab.Columns[i].ColumnName == "GhiChu")
            //    {
            //        _ListString.Add("GhiChu");
            //        SetGridBandedViewAmount(bandedView, "", "Ghi chú", _ListString);
            //        _ListString.Clear();
            //    }

            //}
            //List<string> listHeader = new List<string>();
            //int j = 7;
            //string maHang = string.Empty;
            //while (j < tab.Columns.Count)
            //{
            //    string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            //    maHang = arrName[1];
            //    listHeader.Add(tab.Columns[j].ColumnName);
            //    j++;

            //}

            //GridBand gridBand = new GridBand();
            //gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //gridBand.AppearanceHeader.Options.UseFont = true;
            //gridBand.AppearanceHeader.Options.UseTextOptions = true;
            //gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            //BandedGridColumn colAmount = bandedView.Columns.AddField("Amount");
            //SetGridBandedViewAmount(bandedView, maHang, "", listHeader);

            //colAmount.OptionsColumn.AllowEdit = false;
            //colAmount.Caption = "Tổng";
            //colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            ////colAmount.UnboundExpression = exp;
            //colAmount.Visible = true;
            //colAmount.OwnerBand = gridBand;
            //colAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            //gridBand.Visible = true;
            //gridBand.Caption = "Tổng";
            //GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            //item.FieldName = "Amount";
            //item.DisplayFormat = "{0:n0}";
            //bandedView.Bands.Add(gridBand);
            //foreach (BandedGridColumn col in bandedView.Columns)
            //{

            //    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG"
            //        && col.FieldName != "NgayGH")
            //    {
            //        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
            //        itemSize.FieldName = col.FieldName;
            //        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            //        itemSize.DisplayFormat = "{0:n0}";
            //        itemSize.ShowInGroupColumnFooter = col;
            //        bandedView.GroupSummary.Add(itemSize);
            //    }

            //    //if (col.FieldName == "Amount")
            //    //{
            //    //    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
            //    //}
            //    //string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            //    //if (arrName.Length > 1)
            //    //{
            //    //    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
            //    //}

            //    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            //    if (arrName.Length > 1)
            //    {
            //        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
            //    }
            //}

            bandedView.CustomDrawBandHeader += bandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += bandedView_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            bandedView.PopupMenuShowing += BandedView_PopupMenuShowing;
            bandedView.OptionsSelection.MultiSelect = true;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.RowCountChanged += BandedView_RowCountChanged;
            bandedView.CustomDrawRowIndicator += BandedView_CustomDrawRowIndicator;
            bandedView.KeyPress += gridviewThongTinDonHang_KeyPress;
            bandedView.ValidatingEditor += BandedView_ValidatingEditor;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        private void BandedView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.Contains("@"))
            {
                int outParse = -1;
                if (e != null && e.Value != null)
                {
                    bool flagParse = int.TryParse(e.Value.ToString(), out outParse);
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Value = 0;
                    }
                    else if (flagParse && outParse < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập > 0!.";
                        return;
                    }
                    else if (!flagParse)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!.";
                        return;
                    }
                    //if (Convert.ToInt32(e.Value) < 0)
                    //{
                    //    e.Valid = false;
                    //    e.ErrorText = "Số lượng phải lớn hơn 0";
                    //}
                }
            }

        }

        private void gridviewThongTinDonHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        private void BandedView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridViewThongTinDonHang.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        // Example of column names where you want the context menu to appear
                        List<string> allowedColumns = new List<string> { "ID", "MaDH", "POID", "PO", "MaQG", "DauSizeID", "DauSize", "NgayGH", "GhiChu", "MaMau" };

                        if (!allowedColumns.Contains(e.HitInfo.Column.FieldName))
                        {
                            // ColumnName.Contains("@Size@")
                            if (e.HitInfo.Column.FieldName.Contains("@Size@"))
                            {
                                // Add the "Copy" item to the context menu
                                DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", ItemCopy_Click);
                                e.Menu.Items.Add(menuCopyItem);

                                // Add the "Paste" item to the context menu
                                DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", ItemPaste_Click);
                                e.Menu.Items.Add(menuPasteItem);
                            }

                        }
                        DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", ItemCoppyPaste_Click);
                        e.Menu.Items.Add(menuCoppyPasteItem);
                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", ItemDelete_Click);
                        e.Menu.Items.Add(menuDeleteItem);
                    }

                }
            }
        }

        private void BandedView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        void bandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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
        void bandedView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DauSize")
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "0";
                }

            }
            else
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
                        string valueStr = e.Value.ToString();
                        int parsedValue;
                        bool isNumeric = Int32.TryParse(valueStr, out parsedValue);

                        if (!isNumeric)
                        {
                            e.DisplayText = "-";
                        }
                        else
                        {
                            if (Convert.ToInt32(e.Value.ToString()) == 0)
                                e.DisplayText = "-";
                        }

                    }
                }
            }

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
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (nrOfColumns == 1 && (columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "MaMau" || columnNames[0] == "DauSize" || columnNames[0] == "MaQG" || columnNames[0] == "NgayGH" || columnNames[0] == "GhiChu"))
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);
                bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                if (columnNames[0] == "MaQG")
                {
                    string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                    rCountryEdit.DataSource = tbl;
                    rCountryEdit.DisplayMember = "TenQG";
                    rCountryEdit.ValueMember = "MaQG";
                    rCountryEdit.ShowClearButton = false;
                    rCountryEdit.NullText = "[Chọn giá trị]";

                    GridView dvView = rCountryEdit.View;
                    if (dvView.Columns.Count == 0)
                    {
                        dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã quốc gia", Name = "colMaQG", Visible = false });
                        dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Quốc gia", Name = "colTenQG", Visible = true });

                    }
                    bandedColumns.ColumnEdit = rCountryEdit;
                }
                else if (columnNames[0] == "NgayGH")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                }
                else
                {
                    if (columnNames[0] == "MaMau")
                    {
                        string url = string.Format("{0}?", URL + "BangMau/GetBangMau");
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                        rCountryEdit.DataSource = tbl;
                        rCountryEdit.DisplayMember = "TenMau";
                        rCountryEdit.ValueMember = "MaMau";
                        rCountryEdit.ShowClearButton = false;
                        rCountryEdit.NullText = "[Chọn giá trị]";

                        GridView dvView = rCountryEdit.View;
                        if (dvView.Columns.Count == 0)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaMau", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên màu", Name = "colTenMau", Visible = true });

                        }
                        bandedColumns.ColumnEdit = rCountryEdit;
                    }
                    else
                    {
                        if (columnNames[0] == "PO")
                        {
                            bandedColumns.OptionsColumn.AllowEdit = false;
                            bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                        }

                    }
                }
                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;

                bandedColumns.Caption = GridBandCaption;
                bandedColumns.Visible = true;
                bandedColumns.Width = 120;

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
                    GridBand gridband3 = new GridBand();
                    gridband3.Caption = _colName[0];
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
        private void gridViewThongTinDonHang_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridViewThongTinDonHang_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        private void checkEditImport_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit checkEdit = sender as CheckEdit;
            _dtSize = CreateDatatableSize();
            _dtTable = CreateDatatable();
            gridBandSize.Children.Clear();
            if (checkEdit != null)
            {
                if (checkEdit.Checked)
                {
                    // btnImportNTB.Enabled = true;
                    btImportNew.Enabled = true;
                    btImport.Enabled = true;
                    btNhap.Enabled = false;
                    btnRosyV2.Enabled = true;
                    txtNhapPO.Enabled = false;
                    txtNhapSize.Enabled = false;
                    txtDauSize.Enabled = false;
                    txtMau.Enabled = false;
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //SearchLookUpEditQG.Enabled = false;
                    labelMoTa.Text = "";
                    txtNhapPO.Text = "";
                    txtNhapSize.Text = "";
                    txtMau.Text = "";
                    txtDauSize.Text = "";
                    SearchLookUpEditQG.EditValue = null;
                    gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                    gridControlThongTinDonHang.DataSource = _dtTable;
                    gridControl1.DataSource = _dtSize;

                }
                else
                {
                    //string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    //string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    //DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
                    //txtMau.EditValue = dtMau;

                    //string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
                    //string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                    //DataTable _dtKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);

                    // btnImportNTB.Enabled = false;
                    btImportNew.Enabled = false;
                    btImport.Enabled = false;
                    btnRosyV2.Enabled = false;
                    btNhap.Enabled = true;
                    txtNhapPO.Enabled = true;
                    txtNhapSize.Enabled = true;
                    txtDauSize.Enabled = true;
                    txtMau.Enabled = true;
                    //SearchLookUpEditQG.Enabled = true;
                    labelMoTa.Text = "Vui lòng nhập PO, Màu, InSeam, Size và Quốc gia. Mỗi đối tượng phải cách nhau bằng dấu : ";
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    System.Data.DataTable dtbNull = new DataTable();
                    gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                    gridControlThongTinDonHang.DataSource = _dtTable;
                    gridControl1.DataSource = _dtSize;
                }
            }
            CreateSearchLookupQG();
            InitSearInitSearchLookupMaMau();
            InitSearchLookupDauSize();
        }
        static string RemoveOneUnderscore(string input)
        {
            return Regex.Replace(input, @"_+", "_");
        }



        private bool CheckDuplicateData()
        {
            // Check PO, Mau, DauSize, Size: Nếu có 1 phần tử trùng lại => Thông báo lỗi
            bool checkPO = txtNhapPO.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
            int countPONullValue = txtNhapPO.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
            if (checkPO)
            {
                MessageBox.Show("Dữ liệu PO nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            else if (countPONullValue > 0)
            {
                MessageBox.Show("Dữ liệu PO nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            bool checkMau = mau.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
            int countMauNullValue = txtMau.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
            if (checkMau)
            {
                MessageBox.Show("Dữ liệu Màu nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            else if (countMauNullValue > 0)
            {
                MessageBox.Show("Dữ liệu Màu nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            if (txtDauSize.Text.ToString() != "")
            {
                bool checkDauSize = txtDauSize.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
                int countDauSizeNullValue = txtDauSize.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
                if (checkDauSize)
                {
                    MessageBox.Show("Dữ liệu Đầu Size nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                else if (countDauSizeNullValue > 0)
                {
                    MessageBox.Show("Dữ liệu Đầu Size nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
            }


            bool checkSize = txtNhapSize.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
            int countSizeNullValue = txtNhapSize.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
            if (checkSize)
            {
                MessageBox.Show("Dữ liệu Size nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            else if (countSizeNullValue > 0)
            {
                MessageBox.Show("Dữ liệu Size nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }

        private DataTable AddColumnSize(DataTable _tbl, List<string> _lstSize)
        {
            foreach (string size in _lstSize)
            {
                DataRow _dr = _dtSize.NewRow();
                _dr["MaSize"] = "";
                _dr["TenSize"] = size;
                _dr["SizeSanXuat"] = ReplaceSpecialCharacters(size);
                _dr["CodeSize"] = "";
                _dtSize.Rows.Add(_dr);
                // _tbl.Columns.Add(string.Format("{0}@Size@{1}", size, size), typeof(double));
            }
            return _tbl;
        }

        // Hàm CreateDataTableFromNhap() tạo ra DataTable chi tiết từ 4 field được nhập vào: PO, DauSize, Mau, SizeSanXuat
        // Chỉ dùng khi nhập, không dùng khi import
        private DataTable CreateDataTableFromNhap()
        {
            DataTable _dataTable = CreateColumnNhap();
            Console.WriteLine("CreateDataTableFromNhap");

            List<string> _lstPo = txtNhapPO.Text.Split(new char[] { ':' }).ToList();
            List<string> _lstDauSize = txtDauSize.Text.ToString() == "" ? new List<string> { "0" } : txtDauSize.Text.Split(new char[] { ':' }).ToList();
            //List<string> _lstMau = txtMau.Text.Split(new char[] { ':' }).ToList();
            List<string> _lstMau = mau.Split(new char[] { ':' }).ToList();
            _dataTable = CreateRowNhap(_dataTable, _lstPo, _lstDauSize, _lstMau);
            return _dataTable;
        }

        private DataTable CreateColumnNhap()
        {
            List<string> _lstSize = txtNhapSize.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            DataTable _dataTable = new DataTable();
            _dataTable.Columns.Add("ChungTuPo", typeof(string));
            _dataTable.Columns.Add("MauPO", typeof(string));
            _dataTable.Columns.Add("MaNhomSize", typeof(string));
            _dataTable.Columns.Add("MaQG", typeof(string));
            for (int i = 0; i < _lstSize.Count; i++)
            {
                _dataTable.Columns.Add(string.Format("Size {0}", _lstSize[i]), typeof(int));
            }
            return _dataTable;
        }

        private DataTable CreateRowNhap(DataTable _dtTable, List<string> _lstPO, List<string> _lstDauSize, List<string> _lstMau)
        {
            Console.WriteLine("CreateRowNhap");
            foreach (string _po in _lstPO)
            {
                foreach (string _dauSize in _lstDauSize)
                {
                    foreach (string _mau in _lstMau)
                    {
                        DataRow row = _dtTable.NewRow();
                        row["ChungTuPO"] = _po;
                        row["MauPO"] = _mau;
                        row["MaNhomSize"] = _dauSize;
                        row["MaQG"] = SearchLookUpEditQG.EditValue;
                        _dtTable.Rows.Add(row);
                    }
                }
            }
            return _dtTable;
        }

        void mainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }
        void mainView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
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
        private void txtNhapPO_TextChanged(object sender, EventArgs e)
        {
            txtNhapPO.Text = txtNhapPO.Text.ToString().ToUpper();
            txtNhapPO.SelectionStart = txtNhapPO.Text.Length;
        }

        private void txtNhapSize_TextChanged(object sender, EventArgs e)
        {
            txtNhapSize.Text = txtNhapSize.Text.ToString().ToUpper();
            txtNhapSize.SelectionStart = txtNhapSize.Text.Length;
        }

        private void txtNhapPO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '\b' && e.KeyChar != ' ')
            {
                e.Handled = true;
            }

        }

        private void txtNhapSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void txtMaDH_TextChanged(object sender, EventArgs e)
        {
            txtMaDH.Text = txtMaDH.Text.ToString().ToUpper();
            txtMaDH.SelectionStart = txtMaDH.Text.Length;
        }

        private void txtMaDH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void txtTenDH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
        static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        static string ProcessString(string text)
        {
            string trimmedString = text.Trim();
            string uppercaseString = trimmedString.ToUpper();
            string stringWithoutSpaces = uppercaseString.Replace(" ", "");
            return stringWithoutSpaces;
        }

        private void MainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;

            // Kiểm tra xem dòng này có được chọn không
            if (view.IsRowSelected(e.RowHandle))
            {
                // Thiết lập màu sắc cho dòng được chọn
                e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            }
        }

        private DataTable MapData(DataTable _dtTable, DataTable tblChanged)
        {
            // Tạo một DataTable tạm từ tblChanged. Chỉ giữ lại các cột số lượng size, xóa các cột còn lại
            // Dùng để map số lượng theo từng size cho bảng _dtTable
            // Sau khi xóa số lượng cột của _tblTempSLSize sẽ bằng với số lượng cột size của _dtTable
            string json = string.Empty;
            DataTable _tblTempSLSize = tblChanged.Copy();
            if (checkEditTV.Checked == false)
            {
                string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            }
            else
            {
                string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            }

            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(json);
            //for (int i = 0; i < _tblTempSLSize.Columns.Count;)
            //{
            //    DataColumn column = _tblTempSLSize.Columns[i];
            //    List<string> _lstTemp = column.ColumnName.Split(new char[] { ' ' }).ToList();
            //    if (!_lstTemp[0].ToUpper().Equals("SIZE"))
            //    {
            //        _tblTempSLSize.Columns.RemoveAt(i);
            //    }
            //    else
            //    {
            //        i++;
            //    }
            //}
            // Xóa 3 cột đầu tiên
            if ((bool)checkEditImport.Checked == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_tblTempSLSize.Columns.Count > 0)
                    {
                        _tblTempSLSize.Columns.RemoveAt(0);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_tblTempSLSize.Columns.Count > 0)
                    {
                        _tblTempSLSize.Columns.RemoveAt(0);
                    }
                }
            }


            // Xóa cột cuối cùng nếu có tên là "SỐ LƯỢNG"
            if (_tblTempSLSize.Columns.Count > 0)
            {
                DataColumn lastColumn = _tblTempSLSize.Columns[_tblTempSLSize.Columns.Count - 1];
                if (RemoveDiacritics(lastColumn.ColumnName.ToUpper()).Replace(" ", "").Trim() == "SOLUONG")
                {
                    _tblTempSLSize.Columns.Remove(lastColumn);
                }
            }

            // POID, PO, MaMau, DauSize, MaQG, NgayGH, GhiChu
            foreach (DataRow row in tblChanged.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                var rowmau = tblmau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == row[1].ToString().ToUpper().Trim());
                DataRow _rowAdd = _dtTable.NewRow();
                _rowAdd["POID"] = txtMaDH.Text.ToString() + '|' + ReplaceSpecialCharacters(RemoveVietnameseTone(row[0].ToString()));
                _rowAdd["PO"] = row[0].ToString();
                _rowAdd["MaMau"] = row[1].ToString().ToUpper().Trim();
                //_rowAdd["DauSize"] = Regex.Replace(row[2].ToString().Trim(), @"[^\w\d]+", "_");
                _rowAdd["DauSize"] = row[2].ToString().ToUpper().Trim();
                _rowAdd["MaQG"] = SearchLookUpEditQG.EditValue;
                //_rowAdd["NgayGH"] = DateTime.Now;
                //_rowAdd["GhiChu"] = txtGhiChu.Text;

                //_rowAdd["ColorCode"] = string.IsNullOrEmpty(rowmau["ColorCode"].ToString()) ? "": rowmau["ColorCode"].ToString();
                if (rowmau != null && rowmau["CodeMau"] != DBNull.Value)
                {
                    _rowAdd["ColorCode"] = rowmau["CodeMau"];
                }
                else
                {
                    _rowAdd["ColorCode"] = ""; // Giá trị mặc định nếu có lỗi
                }
                //_rowAdd["ColorCode"] = (rowmau != null && rowmau["ColorCode"] != DBNull.Value) ? rowmau["ColorCode"].ToString() : "";
                _dtTable.Rows.Add(_rowAdd);

            }


            // Map danh sách số lượng của từng size
            int index = 0;
            foreach (DataColumn column in _dtTable.Columns)
            {
                if (column.ColumnName.Contains("@Size@"))
                {
                    for (int i = 0; i < _dtTable.Rows.Count; i++)
                    {
                        _dtTable.Rows[i][column] = _tblTempSLSize.Rows[i][index];
                    }
                    index += 1;
                }
            }

            if (grid != null)
            {

                foreach (DataRow dtRow in _dtTable.Rows)
                {
                    string poid = dtRow["POID"].ToString();
                    string po = dtRow["PO"].ToString();
                    string mamau = dtRow["MaMau"].ToString();
                    string dausize = dtRow["DauSize"].ToString();

                    DataRow[] matchingRows = grid.Select($"POID = '{poid}'  AND PO ='{po}'  AND MaMau ='{mamau}' AND DauSize = '{dausize}'");

                    if (matchingRows.Length > 0)
                    {
                        // Cập nhật dữ liệu của dòng trùng lặp
                        DataRow gridRow = matchingRows[0];
                        dtRow.BeginEdit();

                        foreach (DataColumn column in grid.Columns)
                        {
                            if (column.ColumnName == "PO" || column.ColumnName == "POID")
                                continue;
                            if (dtRow.Table.Columns.Contains(column.ColumnName))
                            {
                                // Ghi đè dữ liệu từ gridRow sang dtRow
                                dtRow[column.ColumnName] = gridRow[column.ColumnName];
                            }
                            //dtRow[column.ColumnName] = gridRow[column.ColumnName];
                        }
                        dtRow.EndEdit();
                    }
                }
            }

            return _dtTable;
        }
        private DataTable MapDataV2(DataTable _dtTable, DataTable tblChanged)
        {
            DataTable _tblTempSLSize = tblChanged.Copy();
            for (int i = 0; i < 3; i++)
            {
                if (_tblTempSLSize.Columns.Count > 0)
                {
                    _tblTempSLSize.Columns.RemoveAt(0);
                }
            }

            // Xóa cột cuối cùng nếu có tên là "SỐ LƯỢNG"
            if (_tblTempSLSize.Columns.Count > 0)
            {
                DataColumn lastColumn = _tblTempSLSize.Columns[_tblTempSLSize.Columns.Count - 1];
                if (RemoveDiacritics(lastColumn.ColumnName.ToUpper()).Replace(" ", "").Trim() == "SOLUONG")
                {
                    _tblTempSLSize.Columns.Remove(lastColumn);
                }
            }
            //foreach (DataRow row in tblChanged.Rows)
            //{
            //    if (row[0].ToString() == "***")
            //    {
            //        break;
            //    }
            //    DataRow _rowAdd = _dtTable.NewRow();
            //    _rowAdd["POID"] = txtMaDH.Text.ToString() + '|' + row[0].ToString();
            //    _rowAdd["PO"] = row[0].ToString();
            //    _rowAdd["MaMau"] = row[1];
            //    _rowAdd["DauSize"] = row[2].ToString().Replace("\"", "");
            //    _rowAdd["MaQG"] = row[31].ToString().Trim();
            //    //DateTime parsedDate;
            //    //string[] format = { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss tt", "dd/MM/yyyy", "dd/MM/yyyy h:mm:ss tt", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy" };
            //    //if (DateTime.TryParseExact(row[30].ToString(), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate))
            //    //{
            //    //    _rowAdd["NgayGH"] = parsedDate.ToString("dd/MM/yyyy"); 
            //    //}
            //    //else
            //    //{
            //    //    _rowAdd["NgayGH"] = DBNull.Value;
            //    //}
            //    if (row[30].ToString() == "")
            //        _rowAdd["NgayGH"] = DBNull.Value;
            //    else
            //        _rowAdd["NgayGH"] = row[30].ToString();
            //    //_rowAdd["GhiChu"] = txtGhiChu.Text;
            //    _dtTable.Rows.Add(_rowAdd);

            //}
            for (int j = 0; j < tblChanged.Rows.Count; j++)
            {
                if (tblChanged.Rows[j].ToString() == "***")
                {
                    break;
                }
                DataRow _rowAdd = _dtTable.NewRow();
                _rowAdd["POID"] = txtMaDH.Text.ToString() + '|' + tblChanged.Rows[j][0].ToString();
                _rowAdd["PO"] = tblChanged.Rows[j][0].ToString();
                _rowAdd["MaMau"] = tblChanged.Rows[j][1];
                _rowAdd["DauSize"] = tblChanged.Rows[j][2].ToString().Replace("\"", "");
                for (int col = 3; col < tblChanged.Columns.Count; col++)
                {
                    if (RemoveVietnameseTone(tblChanged.Columns[col].ColumnName.ToString()).ToString().ToUpper().Trim().Replace(" ", "") == "NGAYGIAOHANG")
                    {
                        if (tblChanged.Rows[j][col].ToString() == "")
                            _rowAdd["NgayGH"] = "";
                        else
                        {
                            string formattedDate = string.Empty;
                            string[] formats =
                                {  "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss", "MM/dd/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy", "MM/yyyy/dd h:mm:ss","M/d/yyyy h:mm:ss","M/d/yy h:mm:ss","MM/dd/yy h:mm:ss", "yy/MM/dd h:mm:ss", "yyyy/MM/dd h:mm:ss","dd-MMM-yy h:mm:ss","yyyy-MM-dd h:mm:ss", "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy h:mm:ss tt", "dd\\MM\\yyyy", "dd\\MM\\yyyy HH:mm:ss","dd\\MM\\yyyy h:mm:ss tt","dd-MMM-yyyy","dd-MMMM-yyyy","dd-MMM-yyyy HH:mm:ss","dd-MMMM-yyyy HH:mm:ss","yyyy/MM/dd","yyyy-MM-dd","yyyy.MM.dd","yyyy\\MM\\dd","yyyyMMdd","dd/MM/yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss","dd\\MM\\yyyy HH:mm:ss","dd/MM/yyyy hh:mm:ss tt","dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt","dd\\MM\\yyyy hh:mm:ss tt","yyyy-MM-ddTHH:mm:ssZ","yyyy-MM-ddTHH:mm:ss.fffZ","yyyy-MM-ddTHH:mm:sszzz","yyyy-MM-ddTHH:mm:ss.fffzzz","dd/MM/yy","dd-MM-yy","dd.MM.yy","dd\\MM\\yy","MM/dd/yy","MM-dd-yy","MM.dd.yy","MM\\dd\\yy","yyyy/MM/dd HH:mm:ss","yyyy-MM-dd HH:mm:ss","yyyy.MM.dd HH:mm:ss","yyyy\\MM\\dd HH:mm:ss","yyyy/MM/dd hh:mm:ss tt","yyyy-MM-dd hh:mm:ss tt","yyyy.MM.dd hh:mm:ss tt","yyyy\\MM\\dd hh:mm:ss tt"
                            };
                            DateTime dateValue;
                            bool success = DateTime.TryParseExact(tblChanged.Rows[j][col].ToString(), formats, null, System.Globalization.DateTimeStyles.None, out dateValue);

                            if (success)
                            {
                                formattedDate = dateValue.ToString("dd/MM/yyyy");
                                Console.WriteLine(formattedDate);
                            }
                            else
                            {
                                Console.WriteLine("Định dạng không hợp lệ.");
                            }
                            DateTime date = DateTime.ParseExact(formattedDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None);
                            _rowAdd["NgayGH"] = date;

                        }
                        //if (tblChanged.Rows[j][col].ToString() == "")
                        //    _rowAdd["NgayGH"] = DBNull.Value;
                        //else
                        //    _rowAdd["NgayGH"] = tblChanged.Rows[j][col].ToString();
                    }
                    if (RemoveVietnameseTone(tblChanged.Columns[col].ColumnName.ToString()).ToString().ToUpper().Trim().Replace(" ", "") == "DIADIEMGIAOHANG")
                    {
                        _rowAdd["MaQG"] = tblChanged.Rows[j][col].ToString().Trim();
                    }


                }
                _dtTable.Rows.Add(_rowAdd);
            }

            int index = 0;
            foreach (DataColumn column in _dtTable.Columns)
            {
                if (column.ColumnName.Contains("@Size@"))
                {
                    for (int i = 0; i < _dtTable.Rows.Count; i++)
                    {
                        _dtTable.Rows[i][column] = _tblTempSLSize.Rows[i][index];
                    }
                    index += 1;
                }
            }

            return _dtTable;
        }
        public DataTable ChangeTenMauToMaMau(DataTable tbl, string _maHang)
        {
            try
            {
                DataTable tblChange = tbl.Copy();
                // lấy danh sách bảng màu
                string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                List<BangMauEntity> _lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
                string columnNameMauPO = "MAUPO";

                // Cập nhật field Màu PO
                for (int i = 0; i < tblChange.Rows.Count; i++)
                {
                    foreach (DataColumn dataColumn in tblChange.Columns)
                    {
                        if (tblChange.Rows[i][dataColumn].ToString() == "***")
                        {
                            break;
                        }
                        if (RemoveDiacritics(dataColumn.ColumnName).Replace(" ", "").ToUpper().Equals(columnNameMauPO))
                        {
                            Console.WriteLine("ChangedTenMauToMaMau");
                            if (tblChange.Rows[i][dataColumn] != null)
                            {
                                string tenMau = tblChange.Rows[i][dataColumn].ToString();
                                // Lấy ra mã màu từ _lstBangMau bằng field TenMau và MaHang
                                BangMauEntity bangMauEntity = _lstBangMau.Where(x => x.TenMau.ToString().Trim().ToUpper() == tenMau.ToString().Trim().ToUpper() && x.MaHang == _maHang).FirstOrDefault();

                                if (bangMauEntity != null)
                                {
                                    // Đổi DataTable
                                    tblChange.Rows[i][dataColumn] = bangMauEntity.MaMau;
                                }
                                else
                                {
                                    Console.WriteLine("Import màu không thành công => Nên không lấy được mã màu sau khi import");
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Change PO complete");
                return tblChange;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }

        }

        private void AutoImportCL(List<string> lstSaveCL)
        {
            List<ChungLoaiEntity> lstAutoImportCL = new List<ChungLoaiEntity>();
            ChungLoaiEntity autoImportKHItem;
            // string maHang = searchLookUpEditMH.EditValue.ToString().Trim();

            if (lstSaveCL != null && lstSaveCL.Count > 0)
            {
                lstSaveCL = lstSaveCL.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveCL.Count; i++)
                {
                    ChungLoaiEntity itemCL = new ChungLoaiEntity(lstSaveCL[i]);
                    lstAutoImportCL.Add(itemCL);
                }
                if (lstAutoImportCL.Count > 0)
                {
                    //string urlAutoImportMau = string.Format("{0}?", URL + "BangMau/PostAutoImportBangMau");
                    //string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportMau, lstAutoImportMau); }).Result;
                }
            }
        }
        private void AutoImportMau(List<string> lstSaveMau)
        {
            List<BangMauEntity> lstAutoImportMau = new List<BangMauEntity>();
            BangSizeEntity autoImportSizeItem;
            string maHang = searchLookUpEditMH.EditValue.ToString().Trim();

            if (lstSaveMau != null && lstSaveMau.Count > 0)
            {
                lstSaveMau = lstSaveMau.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveMau.Count; i++)
                {
                    BangMauEntity itemBangMau = new BangMauEntity(searchLookUpEditMH.EditValue.ToString(), lstSaveMau[i], "", "");
                    lstAutoImportMau.Add(itemBangMau);
                }
                if (lstAutoImportMau.Count > 0)
                {
                    string urlAutoImportMau = string.Format("{0}?", URL + "BangMau/PostAutoImportBangMau");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportMau, lstAutoImportMau); }).Result;
                }
            }
        }
        private void AutoImportQG(List<string> lstSaveQG)
        {
            List<QuocGiaEntity> lstAutoImportQG = new List<QuocGiaEntity>();
            QuocGiaEntity autoImportQGItem;
            if (lstSaveQG != null && lstSaveQG.Count > 0)
            {
                lstSaveQG = lstSaveQG.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveQG.Count; i++)
                {
                    QuocGiaEntity itemQG = new QuocGiaEntity(lstSaveQG[i]);
                    lstAutoImportQG.Add(itemQG);
                }
                if (lstAutoImportQG.Count > 0)
                {
                    string urlAutoImportQG = string.Format("{0}?", URL + "QuocGia/PostQuocGia");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportQG, lstAutoImportQG); }).Result;

                }
            }
        }

        private void AutoImportSize(List<string> lstColumnName, bool isFromImport)
        {
            List<BangSizeEntity> lstAutoImportSize = new List<BangSizeEntity>();
            BangSizeEntity autoImportSizeItem;
            string maHang = searchLookUpEditMH.EditValue.ToString().Trim();
            for (int i = 0; i < lstColumnName.Count; i++)
            {
                string columnName = lstColumnName[i];

                // Khi import list column chứa tên size
                if (isFromImport)
                {
                    if (columnName.Contains("Size"))
                    {
                        string tenSize = columnName;
                        string sizeSX = string.Empty;
                        string[] nameSize = columnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameSize != null && nameSize.Length > 1)
                        {

                            int sizeIndex = columnName.IndexOf("Size");
                            string sizePart = "Size";
                            string gia_tri = columnName.Substring(sizeIndex + sizePart.Length).Trim();
                            sizeSX = Regex.Replace(RemoveDiacritics(gia_tri), @"[^\w\d]+", "_");
                            autoImportSizeItem = new BangSizeEntity(sizeSX, tenSize, maHang);
                            lstAutoImportSize.Add(autoImportSizeItem);
                        }
                    }
                }
                else // Khi nhập list column chứa size sản xuất
                {
                    string sizeSX = columnName;
                    string tenSize = string.Format("Size {0}", columnName);
                    //string[] nameSize = columnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //sizeSX = nameSize[1];
                    autoImportSizeItem = new BangSizeEntity(sizeSX, tenSize, maHang);
                    lstAutoImportSize.Add(autoImportSizeItem);
                }

            }
            if (lstAutoImportSize != null && lstAutoImportSize.Count > 0)
            {
                string urlImportBangSize = string.Format("{0}", URL + "BangSize/PostAutoImportBangSize");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlImportBangSize, lstAutoImportSize); }).Result;
            }
        }

        private void mainView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.RowHandle >= 0 && (e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayGH" || e.Column.FieldName == "NgayDKVC" || e.Column.FieldName == "NgayThucTeVC"
                || e.Column.FieldName == "NgayXuatHang"))
            {
                GridView gridView = sender as GridView;

                DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
                DataTable dataTable = (gridView.DataSource as DataView).Table as DataTable;
                if (rowFocused != null && rowFocused["PO"] != null && dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (dataTable.Rows[i]["PO"] != null && (rowFocused["PO"].Equals(dataTable.Rows[i]["PO"])))
                        {
                            gridView.SetRowCellValue(i, e.Column.FieldName, e.Value);
                        }
                    }
                }
            }
        }

        private void MainView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridViewThongTinDonHang.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {

                    DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", ItemCopy_Click);
                    e.Menu.Items.Add(menuCopyItem);

                    DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", ItemPaste_Click);
                    e.Menu.Items.Add(menuPasteItem);

                }
            }
        }
        private void ItemCopy_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            view.CopyToClipboard();

        }
        private void ItemPaste_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 1) return;
            int startRow = view.FocusedRowHandle;
            foreach (string row in data)
            {
                AddRow(row, startRow++);
                if (!view.IsValidRowHandle(startRow)) break;
            }
        }

        private void ItemCoppyPaste_Click(object sender, EventArgs e)
        {
            // Lấy view từ GridControl
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;

            // Lấy vị trí của dòng được chọn
            //int focusedRowHandle = view.FocusedRowHandle;
            //if (focusedRowHandle < 0) return;
            int[] selectedRowHandles = view.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;
            // Tạo một từ điển chứa dữ liệu của dòng được chọn
            //var rowData = new Dictionary<string, object>();
            //foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.VisibleColumns)
            //{
            //    string fieldName = column.FieldName;
            //    if (!string.IsNullOrEmpty(fieldName))
            //    {
            //        rowData[fieldName] = view.GetRowCellValue(focusedRowHandle, column);
            //    }
            //}


            DataTable dataSource = gridControlThongTinDonHang.DataSource as DataTable;
            if (dataSource == null) return;

            //DataRow newRow = view.GetFocusedDataRow();


            //// Tạo một dòng mới và thêm vào dataSource
            //DataRow insertedRow = dataSource.NewRow();

            //// Gán giá trị cho các cột trong dòng mới 
            //foreach (DataColumn column in dataSource.Columns)
            //{
            //    if (newRow.Table.Columns.Contains(column.ColumnName))
            //    {
            //        insertedRow[column.ColumnName] = newRow[column.ColumnName];
            //    }
            //}

            //// Đặt giá trị mặc định cho "CheckColumn" trong dòng mới là false
            //insertedRow["CheckColumn"] = false;

            //// Chèn dòng mới vào DataTable (sau dòng hiện tại)
            ////dataSource.Rows.InsertAt(insertedRow, focusedRowHandle + 1);
            Array.Sort(selectedRowHandles);
            Array.Reverse(selectedRowHandles);

            foreach (int rowHandle in selectedRowHandles)
            {
                // Lấy dữ liệu của từng dòng được chọn
                DataRow selectedRow = view.GetDataRow(rowHandle);
                if (selectedRow == null) continue;

                // Tạo một dòng mới hoàn toàn
                DataRow newRow = dataSource.NewRow();

                // Sao chép dữ liệu từ dòng được chọn vào dòng mới
                foreach (DataColumn column in dataSource.Columns)
                {
                    if (selectedRow.Table.Columns.Contains(column.ColumnName))
                    {
                        newRow[column.ColumnName] = selectedRow[column.ColumnName];
                    }
                }

                // Đặt giá trị mặc định cho "CheckColumn" là false
                if (dataSource.Columns.Contains("CheckColumn"))
                {
                    newRow["CheckColumn"] = false;
                }

                // Xác định vị trí chèn
                int insertIndex = rowHandle + 1;
                if (insertIndex > dataSource.Rows.Count)
                {
                    insertIndex = dataSource.Rows.Count;
                }

                // Chèn dòng mới vào DataTable
                dataSource.Rows.InsertAt(newRow, insertIndex);
            }

            // Cập nhật lại GridControl
            view.RefreshData();

            // Cập nhật lại GridControl
            gridControlThongTinDonHang.DataSource = dataSource;
            gridControlThongTinDonHang.RefreshDataSource();
            view.RowStyle += (s, z) =>
            {
                var gridView = s as BandedGridView;
                if (gridView == null) return;

                // Lấy dữ liệu của dòng hiện tại
                DataRow row = gridView.GetDataRow(z.RowHandle);
                if (row == null) return;

                // Kiểm tra nếu dòng được đánh dấu là dòng mới
                if (row.Table.Columns.Contains("CheckColumn") && row["CheckColumn"] != DBNull.Value && (bool)row["CheckColumn"] == false)
                {
                    z.Appearance.BackColor = System.Drawing.Color.Yellow; // Tô màu nền
                                                                          //z.Appearance.BackColor2 = System.Drawing.Color.White;    // Hiệu ứng gradient
                }
            };
        }
        private void ItemDelete_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    ((DataTable)gridControlThongTinDonHang.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
            }
        }
        private void AddRow(string data, int rowHandle)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (data == string.Empty) return;
            string[] rowData = data.Split('\t');
            int column = view.FocusedColumn.VisibleIndex;
            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= view.VisibleColumns.Count) break;
                if (rowData[i] == "-")
                    rowData[i] = "0";
                List<string> allowedColumns = new List<string> { "ID", "MaDH", "POID", "PO", "MaQG", "DauSizeID", "DauSize", "NgayGH", "GhiChu", "MaMau" };
                if (view.VisibleColumns[column + i] != null)
                {
                    if (!allowedColumns.Contains(view.VisibleColumns[column + i].ToString()))
                    {
                        view.SetRowCellValue(rowHandle, view.VisibleColumns[column + i], rowData[i].Replace(",", ""));
                    }
                }

            }
        }
        private string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set
            {
                Clipboard.SetDataObject(value);
            }
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtLuu();
        }

        private void BtLuu()
        {
            try
            {
                //if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                //{
                //    MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                //if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                //{
                //    MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                //if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                //{
                //    MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                this.ActiveControl = button1;
                if (txtDot.Text == "" || txtDot.Text == null)
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập tần suất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (checkEditImport.Checked == false)
                {
                    if (SearchLookUpEditQG.EditValue == null || SearchLookUpEditQG.EditValue == "")
                    {
                        MessageBox.Show("Vui lòng chọn Quốc gia.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                }

                if (searchlookupEditHT.EditValue == null || searchlookupEditHT.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn hình thức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //if (txtSoVoice.Text == null || txtSoVoice.Text == "")
                //{
                //    MessageBox.Show("Vui lòng nhập số invoice.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (spinEditHSDH.Text == null || spinEditHSDH.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (spinEditFOB.Text == null || spinEditFOB.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập FOB.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (txtDot.Text == null || txtDot.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (spinEditUSD.Text == null || spinEditUSD.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                DataTable dtbsave = gridControlThongTinDonHang.DataSource as DataTable;
                if (dtbsave.Rows.Count == 0)
                {
                    return;
                }
                string SoBooking = txtSoBooking.Text?.ToString();
                //if (string.IsNullOrEmpty(SoBooking))
                //{
                //    XtraMessageBox.Show(" Số Booking không được để trống hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                {
                    DauSize = row.Field<string>("DauSize"),
                    POID = row.Field<string>("POID"),
                    PO = row.Field<string>("PO"),
                    MaMau = row.Field<string>("MaMau")
                }).Any(g => g.Count() > 1);

                if (checkDuplicateData)
                {
                    XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_dtTable.Columns.Contains("CheckColumn"))
                {
                    _dtTable.Columns.Remove("CheckColumn");
                }

                if (_dtTable.Columns.Contains("IsNew"))
                {
                    _dtTable.Columns.Remove("IsNew");
                }

                //for (int i = _dtTable.Rows.Count - 1; i >= 0; i--)
                //{
                //    int sumcheck = 0;
                //    for (int j = 11; j < _dtTable.Columns.Count; j++)
                //    {
                //        string soluong = _dtTable.Rows[i][j].ToString();
                //        int value;
                //        if (int.TryParse(soluong, out value))
                //            sumcheck += value;
                //    }
                //    if (sumcheck == 0)
                //        _dtTable.Rows.RemoveAt(i);
                //}

                string urlKTDH = string.Format("{0}?madh={1}", URL + "DonHangTong/GetKT", txtMaDH.Text.ToString());
                string jsonKTDH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKTDH); }).Result;
                DataTable tblKTDT = JsonConvert.DeserializeObject<DataTable>(jsonKTDH);
                if (tblKTDT.Rows.Count > 0)
                {
                    string urlDH = string.Format("{0}?", URL + "DonHangTong/GetDonHangTongID");
                    string jsonDH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDH); }).Result;
                    DataTable _tblDH = JsonConvert.DeserializeObject<DataTable>(jsonDH);
                    DialogResult messResult = MessageBox.Show("Đơn hàng " + txtMaDH.Text.ToString() + " đã tồn tại. Đơn hàng sẽ chuyển sang " + "DH_" + (Convert.ToInt32(_tblDH.Rows[0][0]) + 1) + ". Bạn có muốn thay đổi không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.No)
                    {
                        txtMaDH.Text = "DH_" + (Convert.ToInt32(_tblDH.Rows[0][0]) + 1);
                        currentPhieuNumber = currentPhieuNumber + 1;
                        foreach (DataRow _dr in _dtTable.Rows)
                        {
                            //_dr["POID"] = txtMaDH.Text.ToString().Trim() + "|" + Regex.Replace(RemoveVietnameseTone(_dr["PO"].ToString().Trim()), @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_");
                            _dr["POID"] = txtMaDH.Text.ToString().Trim() + "|" + ReplaceSpecialCharacters(RemoveVietnameseTone(_dr["PO"].ToString().Trim()));
                        }
                        return;
                    }
                    else
                    {
                        currentPhieuNumber = currentPhieuNumber + 1;
                        txtMaDH.Text = "DH_" + (Convert.ToInt32(_tblDH.Rows[0][0]) + 1);
                        foreach (DataRow _dr in _dtTable.Rows)
                        {
                            //_dr["POID"] = txtMaDH.Text.ToString().Trim() + "|" + Regex.Replace(RemoveVietnameseTone(_dr["PO"].ToString().Trim()), @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_");

                            _dr["POID"] = txtMaDH.Text.ToString().Trim() + "|" + ReplaceSpecialCharacters(RemoveVietnameseTone(_dr["PO"].ToString().Trim()));
                        }
                    }
                }

                sort = 0;
                this.ActiveControl = this.searchLookUpEditMH;
                if (check == 0)
                {
                    for (int j = 0; j < _dtTable.Rows.Count; j++)
                    {
                        if (_dtTable.Rows[j][0].ToString().ToString() == "***")
                        {
                            break;
                        }
                        if (!string.IsNullOrEmpty(_dtTable.Rows[j][0].ToString()))
                        {
                            //string ctpo = ReplaceSpecialCharacters(RemoveVietnameseTone(_dtTable.Rows[j][0].ToString()).Replace(" ", "").Trim());
                            string ctpo = RemoveVietnameseTone(_dtTable.Rows[j][0].ToString()).Replace(" ", "").Trim();
                            if (!string.IsNullOrEmpty(ctpo) && !ctpo.Trim().ToUpper().Contains("SOLUONG"))
                            {
                                if (ctpo.Contains(" "))
                                {
                                    ctpo = ctpo.Replace(" ", "");
                                }
                                if (string.IsNullOrEmpty(ctpo))
                                {
                                    XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            string col1 = RemoveVietnameseTone(_dtTable.Rows[j][1].ToString().Trim());
                            if (!string.IsNullOrEmpty(col1))
                            {
                                // Gán lại giá trị cho cột 0 theo yêu cầu
                                //_dtTable.Rows[j][0] = txtMaDH.Text.Trim() + "|" + Regex.Replace(col1, @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_");

                                _dtTable.Rows[j][0] = txtMaDH.Text.Trim() + "|" + ReplaceSpecialCharacters(col1);
                            }
                            else
                            {
                                XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        if (!string.IsNullOrEmpty(_dtTable.Rows[j][1].ToString()))
                        {
                            string mau = _dtTable.Rows[j][1].ToString().Replace(" ", "").Trim();
                            if (!string.IsNullOrEmpty(mau) && !mau.Trim().ToUpper().Contains("SOLUONG"))
                            {
                                if (mau.Contains(" "))
                                {
                                    mau = mau.Replace(" ", "");
                                }
                                if (string.IsNullOrEmpty(mau))
                                {
                                    XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }

                        }
                        else
                        {
                            XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                    int Amountcheck = 0, totalAmountcheck = 0;
                    foreach (DataRow row in _dtTable.Rows)
                    {
                        foreach (DataColumn col in _dtTable.Columns)
                        {
                            if (col.ColumnName != "ID" && col.ColumnName != "POID" && col.ColumnName != "PO" && col.ColumnName != "MaMau" && col.ColumnName != "DauSize" && col.ColumnName != "MaQG" && col.ColumnName != "NgayGH" && col.ColumnName != "GhiChu" && col.ColumnName != "ColorCode" && col.ColumnName != "NgayDKVC" && col.ColumnName != "NgayThucTeVC" && col.ColumnName != "NgayXuatHang")
                            {
                                try
                                {
                                    if (row[col.ColumnName] != null && !string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                    {
                                        int value = Convert.ToInt32(row[col.ColumnName]);
                                        totalAmountcheck += value;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                    string urlktdonhangcheck = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", searchLookUpEditMH.EditValue);
                    string jsonktdonhangcheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhangcheck); }).Result;
                    List<DonHangTongEntity> dohanglistcheck = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhangcheck);
                    foreach (DonHangTongEntity donhang in dohanglistcheck)
                    {
                        if (donhang.MaKH == searchLookUpEditKH.EditValue.ToString() && donhang.MaHang == searchLookUpEditMH.EditValue.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(txtDot.Text).Trim().ToUpper().ToString().Replace(" ", "")
                           && donhang.SoLuong == totalAmountcheck)
                        {
                            XtraMessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < _dtTable.Rows.Count; j++)
                    {
                        string col1 = RemoveVietnameseTone(_dtTable.Rows[j][1].ToString().Trim());
                        if (!string.IsNullOrEmpty(col1))
                        {
                            // Gán lại giá trị cho cột 0 theo yêu cầu
                            _dtTable.Rows[j][0] = txtMaDH.Text.Trim() + "|" + ReplaceSpecialCharacters(col1);
                        }
                        else
                        {
                            XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                DevExpress.XtraGrid.GridControl gridControl = gridControlThongTinDonHang;
                DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)gridControl.MainView;
                DataTable dtbm = gridViewThongTinDonHang.DataSource as DataTable;
                //object totalQuantity = gridView.Columns["Amount"].SummaryItem.SummaryValue;
                List<DonHangTongEntityV1> listDonHangTong = new List<DonHangTongEntityV1>();
                List<DonHangTongPOUSEREntity> listDonHangTongPO = new List<DonHangTongPOUSEREntity>();
                List<DonHangTongPOChiTietUSEREntity> listDonHangTongPOChiTiet = new List<DonHangTongPOChiTietUSEREntity>();
                List<BangSizeEntity> listbangsize = new List<BangSizeEntity>();
                List<BangMauEntity> listbangmau = new List<BangMauEntity>();
                string urlSizeLoad = string.Format("{0}?", URL + "BangSize/GetBangSize");
                string jsonSizeLoad = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSizeLoad); }).Result;
                List<BangSizeEntity> lstBangSizeLoad = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonSizeLoad);
                string poID = string.Empty, po = string.Empty;
                string jsonSize = string.Empty, jsonMau = string.Empty;
                //the Size chi tiet
                string urlDsTheSizect = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                string jsonDsTheSizect = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsTheSizect); }).Result;
                //the inseam chi tiet
                string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                //the Mau chi tiet
                string urlDSTheMauct = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                string jsonDSTheMauct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheMauct); }).Result;
                try
                {
                    foreach (DataRow dr in _dtSizeNhom.Rows)
                    {
                        dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                    }
                    string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                    DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

                    string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);

                    if (checkEditTV.Checked == true)
                    {
                        if (dtSize == null && dtSize.Rows.Count < 0)
                        {
                            MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                            return;
                        }
                        var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                          where !dtSize.AsEnumerable().Any(row2 =>
                                              row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                              row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                              row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                          select row1;
                        if (missingRows.Any())
                        {
                            // Nhóm các dòng bị thiếu theo NhomSize
                            var groupedMissing = missingRows
                                .GroupBy(row => row["NhomSize"].ToString())
                                .Select(group => new
                                {
                                    NhomSize = group.Key, // Tên nhóm size
                                    Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                                });

                            // Tạo chuỗi thông báo
                            var message = string.Join("\n", groupedMissing.Select(group =>
                                $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                            // Hiển thị thông báo
                            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }


                    string MaHang = searchLookUpEditMH.EditValue.ToString();
                    int totalAmount = 0;
                    string size = string.Empty, dauszie = string.Empty;
                    DonHangTongEntityV1 donhang = new DonHangTongEntityV1();
                    donhang.MaDH = txtMaDH.Text;
                    donhang.MaHS = txtMaHS.Text;
                    donhang.BookingMaHang = SoBooking;
                    if (searchLookUpEditMH.Text == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn mã hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                        donhang.MaHang = searchLookUpEditMH.EditValue.ToString();
                    if (searchLookUpEditKH.Text == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn khách hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                        donhang.MaKH = searchLookUpEditKH.EditValue.ToString();
                    //if (searchLookUpEditCL.Text == "")
                    //{
                    //    XtraMessageBox.Show("Vui lòng chọn Chủng loại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                    //else
                    donhang.MaCL = searchLookUpEditCL.EditValue.ToString().ToUpper() == "" ? "" : searchLookUpEditCL.EditValue.ToString().ToUpper();
                    donhang.NgayTao = Convert.ToDateTime(dateEditNgayTao.EditValue.ToString());
                    donhang.NguoiTao = GlobleData.UserName;
                    donhang.GhiChu = txtGhiChu.Text;
                    donhang.Dot = txtDot.Text;
                    donhang.NgaySua = DateTime.Now;
                    donhang.SoVoice = string.IsNullOrEmpty(txtSoVoice.Text) ? "" : txtSoVoice.Text;
                    donhang.STT = currentPhieuNumber;
                    for (int i = 0; i <= _dtTable.Rows.Count - 1; i++)
                    {
                        int sum = 0;
                        string tenmau = _dtTable.Rows[i][2].ToString();


                        string dausize = _dtTable.Rows[i][3].ToString();
                        DonHangTongPOUSEREntity donhangpo = new DonHangTongPOUSEREntity();
                        donhangpo.MaDH = txtMaDH.Text;
                        donhangpo.NguoiTao = GlobleData.UserName;
                        if (_dtTable.Rows[i][4].ToString() == "")
                        {
                            XtraMessageBox.Show("Quốc gia không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        donhangpo.MaQG = _dtTable.Rows[i][4].ToString();
                        //if (_dtTable.Rows[i][0] == "" || string.IsNullOrEmpty(_dtTable.Rows[i][0].ToString()))
                        //{
                        //    donhangpo.POID = txtMaDH.Text + "|" + _dtTable.Rows[i][1].ToString();
                        //}
                        //donhangpo.POID = _dtTable.Rows[i][0].ToString() == "" ? txtMaDH.Text.ToString().Trim() + "|" + Regex.Replace(_dtTable.Rows[i][1].ToString().Trim(), @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_") : _dtTable.Rows[i][0].ToString();
                        donhangpo.POID = _dtTable.Rows[i][0].ToString() == "" ? txtMaDH.Text.ToString().Trim() + "|" + ReplaceSpecialCharacters(_dtTable.Rows[i][1].ToString().Trim()) : _dtTable.Rows[i][0].ToString();
                        donhangpo.PO = _dtTable.Rows[i][1].ToString();
                        if (string.IsNullOrEmpty(tenmau.ToString().Trim()))
                        {
                            XtraMessageBox.Show("Màu không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        donhangpo.MaMau = tenmau.ToUpper().Trim();


                        if (_dtTable.Rows[i][2].ToString() == "")
                        {
                            XtraMessageBox.Show("InSeam không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        DataRow rowDs = null;
                        DataRow clcode = null;
                        if (checkEditTV.Checked == false)
                        {
                            DataTable tblDSize = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);
                            rowDs = tblDSize.AsEnumerable().FirstOrDefault(r => r["MaInSeam"].ToString() == Regex.Replace(dausize.ToUpper(), @"[^\w\d]+", "_"));

                            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(jsonDSTheMauct);
                            clcode = tblMau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == tenmau.ToString());
                        }
                        else
                        {
                            rowDs = dtSize.AsEnumerable().FirstOrDefault(r => r["MaNhomSize"].ToString() == Regex.Replace(dausize.ToUpper(), @"[^\w\d]+", "_"));
                            clcode = dtMau.AsEnumerable().FirstOrDefault(f => f["MaMau"].ToString() == tenmau.ToString());
                        }
                        //string doidausize = Regex.Replace(_dtTable.Rows[i][3].ToString(), @"[^\w\d]+", "_");
                        //string doidausizeid = RemoveOneUnderscore(doidausize);
                        donhangpo.DauSizeID = _dtTable.Rows[i][3].ToString() == "" ? "0" : Regex.Replace(dausize.ToUpper(), @"[^\w\d]+", "_");
                        donhangpo.DauSize = checkEditTV.Checked == false ? rowDs["InSeam"].ToString() : rowDs["NhomSize"].ToString();
                        if (_dtTable.Rows[i][5].ToString() == "")
                        {
                            XtraMessageBox.Show("Ngày giao hàng không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        //if (_dtTable.Rows[i][8].ToString() == "")
                        //{
                        //    XtraMessageBox.Show("Ngày dự kiến vào chuyền không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    return;
                        //}
                        //if (_dtTable.Rows[i][9].ToString() == "")
                        //{
                        //    XtraMessageBox.Show("Ngày thực tế vào chuyền không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    return;
                        //}
                        //if (_dtTable.Rows[i][10].ToString() == "")
                        //{
                        //    XtraMessageBox.Show("Ngày xuất hàng không được bỏ trống.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    return;
                        //}
                        donhangpo.NgayGH = Convert.ToDateTime(_dtTable.Rows[i][5].ToString());
                        donhangpo.GhiChu = _dtTable.Rows[i][6].ToString();
                        //bandedGridColumn3
                        //donhangpo.ColorCode = _dtTable.Rows[i][7].ToString() ==""? Regex.Replace(_dtTable.Rows[i][2].ToString(), @"[^\w\d]+", "_") : _dtTable.Rows[i][7].ToString();                        
                        donhangpo.ColorCode = string.IsNullOrEmpty(_dtTable.Rows[i][7].ToString()) ? clcode["TenMau"].ToString() : _dtTable.Rows[i][7].ToString();
                        donhangpo.NgayDKVC = string.IsNullOrEmpty(_dtTable.Rows[i][8].ToString()) ? (DateTime?)null : Convert.ToDateTime(_dtTable.Rows[i][8].ToString());
                        donhangpo.NgayThucTeVC = string.IsNullOrEmpty(_dtTable.Rows[i][9].ToString()) ? (DateTime?)null : Convert.ToDateTime(_dtTable.Rows[i][9].ToString());
                        donhangpo.NgayXuatHang = string.IsNullOrEmpty(_dtTable.Rows[i][10].ToString()) ? (DateTime?)null : Convert.ToDateTime(_dtTable.Rows[i][10].ToString());
                        for (int j = 11; j <= _dtTable.Columns.Count - 1; j++)
                        {
                            string sizeSX = string.Empty;
                            string[] arrNewHeader = _dtTable.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            string sizeID = arrNewHeader[0];
                            sizeID = arrNewHeader[0];
                            sizeSX = arrNewHeader[2];
                            #region //
                            //if (arrNewHeader.Length == 1)
                            //{
                            //    string resultStringSize = RemoveDiacritics(arrNewHeader[0]);
                            //    string doisize = ReplaceSpecialCharacterssize(resultStringSize.ToString());
                            //    string doisizeid = RemoveOneUnderscore(doisize);
                            //    BangSizeEntity bangsizeload = lstBangSizeLoad.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper() == doisizeid.ToString().Trim().ToUpper() + searchLookUpEditMH.EditValue.ToString().Trim()).FirstOrDefault();
                            //    sizeID = bangsizeload.MaSize;
                            //    sizeSX = bangsizeload.SizeSanXuat;
                            //}
                            //else
                            //{
                            //    string resultStringcongsize = RemoveDiacritics(arrNewHeader[0]);
                            //    string doisizecongsize = ReplaceSpecialCharacterssize(resultStringcongsize.ToString());
                            //    string doisizeidcongsize = RemoveOneUnderscore(doisizecongsize);
                            //    BangSizeEntity bangsizeload = lstBangSizeLoad.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper().Trim() == doisizeidcongsize.ToString().ToUpper().Trim() + searchLookUpEditMH.EditValue.ToString().Trim().ToUpper()).FirstOrDefault();
                            //    sizeID = bangsizeload.MaSize;
                            //    sizeSX = bangsizeload.SizeSanXuat;
                            //}

                            //if (string.IsNullOrEmpty(sizeID) || sizeID.Contains(" "))
                            //{
                            //    sizeID = sizeID.Replace(" ", "");
                            //}
                            #endregion
                            string soluong = _dtTable.Rows[i][j].ToString();
                            DonHangTongPOChiTietUSEREntity donhangtongpochitiet = new DonHangTongPOChiTietUSEREntity();
                            //donhangtongpochitiet.POID = /*_dtTable.Rows[i][0].ToString();*/_dtTable.Rows[i][0].ToString() == "" ? txtMaDH.Text.ToString().Trim() + "|" + Regex.Replace(_dtTable.Rows[i][1].ToString().Trim(), @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_") : _dtTable.Rows[i][0].ToString();

                            donhangtongpochitiet.POID = /*_dtTable.Rows[i][0].ToString();*/_dtTable.Rows[i][0].ToString() == "" ? txtMaDH.Text.ToString().Trim() + "|" + ReplaceSpecialCharacters(_dtTable.Rows[i][1].ToString().Trim()) : _dtTable.Rows[i][0].ToString();
                            donhangtongpochitiet.PO = _dtTable.Rows[i][1].ToString();
                            donhangtongpochitiet.MaMau = tenmau.ToUpper().Trim(); _dtTable.Rows[i][2].ToString();
                            //string doidausizepoct = ReplaceSpecialCharacters(_dtTable.Rows[i][3].ToString());
                            //string doidausizeidpoct = RemoveOneUnderscore(doidausize);
                            donhangtongpochitiet.DauSizeID = _dtTable.Rows[i][3].ToString() == "" ? "0" : Regex.Replace(dausize.ToUpper(), @"[^\w\d]+", "_");
                            donhangtongpochitiet.DauSize = checkEditTV.Checked == false ? rowDs["InSeam"].ToString() : rowDs["NhomSize"].ToString();
                            donhangtongpochitiet.SizeID = sizeID;
                            donhangtongpochitiet.Size = sizeSX;
                            donhangtongpochitiet.GhiChu = string.IsNullOrEmpty(_dtTable.Rows[i][6].ToString()) ? null : _dtTable.Rows[i][6].ToString();
                            donhangtongpochitiet.Sort = sort + 1;
                            donhangtongpochitiet.NguoiTao = GlobleData.UserName;
                            sort++;
                            if (System.Text.RegularExpressions.Regex.IsMatch(soluong, "[0-9]"))
                            {
                                string strValue = System.Text.RegularExpressions.Regex.Replace(soluong, "[^0-9a-zA-Z]+", "_");

                                donhangtongpochitiet.SoLuong = Convert.ToInt32(soluong);
                                sum += Convert.ToInt32(soluong);

                            }
                            else
                            {
                                donhangtongpochitiet.SoLuong = 0;
                            }
                            donhangtongpochitiet.MaKiemTra = txtMaDH.Text.ToString();
                            listDonHangTongPOChiTiet.Add(donhangtongpochitiet);

                        }
                        donhangpo.SoLuong = sum;
                        totalAmount += sum;
                        listDonHangTongPO.Add(donhangpo);


                    }
                    donhang.SoLuong = totalAmount;
                    donhang.TrangThai = 0;
                    donhang.VND = Convert.ToDecimal(spinEditVND.EditValue);
                    donhang.CM = Convert.ToDecimal(spinEditUSD.EditValue);
                    donhang.FOB = (float)Convert.ToDouble(spinEditFOB.EditValue);
                    donhang.HeSoDH = (float)Convert.ToDouble(spinEditHSDH.EditValue);
                    donhang.HinhThucXuatHang = searchlookupEditHT.EditValue.ToString().Trim();
                    donhang.MaCty = searchLookUpEditCTY.EditValue.ToString();
                    listDonHangTong.Add(donhang);
                    if (listDonHangTong.Count > 0 && listDonHangTongPO.Count > 0 && listDonHangTongPOChiTiet.Count > 0)
                    {
                        //var kiemtra = listDonHangTongPO.GroupBy(d => new { d.DauSizeID, d.POID, d.MaMau }).Where(g => g.Count() > 1).Select(g => new { DausizeId = g.Key.DauSizeID, POID = g.Key.POID, MaMau = g.Key.MaMau, Count = g.Count() }).ToList();
                        //if (kiemtra.Count != 0)
                        //{
                        //    Dictionary<string, int> dausizeIDPO = new Dictionary<string, int>();

                        //    foreach (var item in listDonHangTongPO)
                        //    {
                        //        if (dausizeIDPO.ContainsKey(item.DauSizeID))
                        //        {
                        //            dausizeIDPO[item.DauSizeID]++;
                        //            item.DauSizeID += "|" + dausizeIDPO[item.DauSizeID];
                        //        }
                        //        else
                        //        {
                        //            dausizeIDPO[item.DauSizeID] = 0;
                        //        }
                        //    }
                        //    foreach (var item1 in listDonHangTongPO)
                        //    {
                        //        foreach (var item2 in listDonHangTongPOChiTiet)
                        //        {
                        //            if (item1.DauSize.ToString() == item2.DauSize.ToString())
                        //            {
                        //                item2.DauSizeID = item1.DauSizeID;
                        //            }
                        //        }
                        //    }

                        //}

                        var distinctRows = _dtTable.AsEnumerable()
                             .GroupBy(row => new { Mamau = row["MaMau"], Colorcode = row["ColorCode"] })
                             .Select(group => group.First())
                             .ToList();

                        DataTable dtThuVienMau = new DataTable();
                        dtThuVienMau.Columns.Add("MaKH", typeof(string));
                        dtThuVienMau.Columns.Add("TenMauKH", typeof(string));
                        dtThuVienMau.Columns.Add("CodeMauKH", typeof(string));

                        //
                        if (checkEditTV.Checked == false)
                        {
                            var MauRows = listDonHangTongPO
                                 .GroupBy(row => new { row.MaMau, row.ColorCode })
                                 .Select(group => group.First())
                                 .ToList();

                            var distinctSizeRows = listDonHangTongPOChiTiet
                                .GroupBy(x => new { x.SizeID, x.DauSizeID })
                                .Select(g => g.First())
                                .ToList();


                            DataTable themau = JsonConvert.DeserializeObject<DataTable>(jsonDSTheMauct);
                            string urlsizecheck = $"{URL}BangSize/GetBangSizebom?para={searchLookUpEditMH.EditValue.ToString()}&para1={searchLookUpEditKH.EditValue.ToString()}";
                            string jsonsizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsizecheck); }).Result;
                            DataTable tblSizecheck = JsonConvert.DeserializeObject<DataTable>(jsonsizecheck);

                            string urlmaucheck = $"{URL}BangMau/GetBangMau3?para={searchLookUpEditMH.EditValue.ToString()}&para2={searchLookUpEditKH.EditValue.ToString()}";
                            string jsonmaucheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmaucheck); }).Result;
                            DataTable tblmaucheck = JsonConvert.DeserializeObject<DataTable>(jsonmaucheck);

                            DataTable tbmauSave = CreateDatatableTheMau();
                            //bangmau
                            foreach (var rowm in MauRows)
                            {
                                var tenmau = themau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == rowm.MaMau.ToString());
                                string mamau = rowm.MaMau.ToString();
                                string colorCode = rowm.ColorCode?.ToString().Trim() ?? mamau;

                                bool existsm = tblmaucheck.AsEnumerable().Any(r =>
                                    r["MaMau"].ToString() == mamau
                                );
                                if (!existsm)
                                {
                                    DataRow newRow = tbmauSave.NewRow();
                                    newRow["ID"] = 0;
                                    newRow["MaMau"] = mamau;
                                    newRow["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                                    newRow["TenMau"] = tenmau["TenMau"];
                                    newRow["CodeMau"] = tenmau["CodeMau"];
                                    newRow["GhiChu"] = "";
                                    newRow["MaKH"] = searchLookUpEditKH.EditValue.ToString();

                                    var thuVienMauRow = themau.AsEnumerable()
                                        .FirstOrDefault(r => (r["MaMau"]?.ToString() ?? "").Trim()
                                            .Equals(mamau, StringComparison.OrdinalIgnoreCase));

                                    if (thuVienMauRow != null)
                                    {
                                        newRow["MaTheMau"] =
                                            thuVienMauRow["MaTheMau"]?.ToString() ??
                                            thuVienMauRow["TheMau"]?.ToString() ??
                                            "";
                                    }
                                    else
                                    {
                                        newRow["MaTheMau"] = ""; // nếu không có dữ liệu mapping
                                    }
                                    tbmauSave.Rows.Add(newRow);
                                }

                            }
                            //bangsize
                            DataTable TheSize = JsonConvert.DeserializeObject<DataTable>(jsonDsTheSizect);
                            DataTable TheInSeam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                            var dictTheSize = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                            if (TheSize != null)
                            {
                                foreach (DataRow r in TheSize.Rows)
                                {
                                    // cố gắng map bằng SizeSanXuat (hoặc MaSize nếu bạn có)
                                    string key = (r["SizeSanXuat"]?.ToString() ?? "").Trim();
                                    if (!dictTheSize.ContainsKey(key)) dictTheSize[key] = r;
                                }
                            }
                            DataTable tblsizeSave = CreateDatatableTheSize();
                            foreach (var rows in distinctSizeRows)
                            {
                                string sizeID = rows.SizeID.ToString();
                                string dauSizeID = rows.DauSizeID.ToString();
                                string sizeText = rows.Size?.ToString() ?? "";
                                string dauSizeText = rows.DauSize?.ToString() ?? "";
                                bool exists = tblSizecheck.AsEnumerable().Any(r =>
                                    r["MaSize"].ToString() == sizeID &&
                                    r["MaNhomSize"].ToString() == dauSizeID
                                );
                                if (!exists)
                                {

                                    DataRow rowSize = tblsizeSave.NewRow();
                                    rowSize["ID"] = 0;
                                    rowSize["MaSize"] = sizeID;
                                    rowSize["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                                    rowSize["CodeSize"] = sizeText;
                                    rowSize["TenSize"] = sizeText;
                                    rowSize["SizeSanXuat"] = sizeText;
                                    rowSize["GhiChu"] = "";
                                    rowSize["XacNhan"] = "";
                                    rowSize["SizeXacNhan"] = 0;
                                    rowSize["MaNhomSize"] = dauSizeID;
                                    rowSize["NhomSize"] = dauSizeText;
                                    rowSize["MaKH"] = searchLookUpEditKH.EditValue.ToString();
                                    rowSize["Sort"] = rows.Sort.ToString();


                                    // ✅ lấy MaTheSize từ thư viện size
                                    var rsTheSize = TheSize.AsEnumerable().FirstOrDefault(r =>
                                        (r["SizeSanXuat"]?.ToString() ?? "").Trim().Equals(sizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                                    );

                                    rowSize["MaTheSize"] = rsTheSize?["MaTheSize"]?.ToString() ?? "";

                                    // ✅ lấy MaTheInSeam từ thư viện inseam (ưu tiên MaInSeam → InSeam)
                                    var rsInSeam = TheInSeam.AsEnumerable().FirstOrDefault(r =>
                                        (r["MaInSeam"]?.ToString() ?? "").Trim().Equals(dauSizeID.Trim(), StringComparison.OrdinalIgnoreCase)
                                        || (r["InSeam"]?.ToString() ?? "").Trim().Equals(dauSizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                                    );

                                    rowSize["MaTheInSeam"] = rsInSeam?["MaTheInSeam"]?.ToString() ?? "";

                                    tblsizeSave.Rows.Add(rowSize);
                                }
                            }

                            if (tbmauSave != null && tbmauSave.Rows.Count > 0)
                            {
                                string urlpostmau = string.Format("{0}?", URL + "DonHangTong/PostMau");
                                string msResultmau = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostmau, tbmauSave); }).Result;
                                if (msResultmau.ToLower() != "true")
                                    XtraMessageBox.Show(msResultmau);
                            }


                            if (tblsizeSave != null && tblsizeSave.Rows.Count > 0)
                            {
                                string urlpostsize = string.Format("{0}", URL + "DonHangTong/PostSize");
                                string msResultsize = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostsize, tblsizeSave); }).Result;
                                if (msResultsize.ToLower() != "true")
                                    XtraMessageBox.Show(msResultsize);
                            }

                        }

                        //

                        //thuvienmau
                        foreach (var row in distinctRows)
                        {
                            DataRow newRow = dtThuVienMau.NewRow();
                            newRow["MaKH"] = searchLookUpEditKH.EditValue.ToString();
                            newRow["TenMauKH"] = row["MaMau"];
                            newRow["CodeMauKH"] = row["ColorCode"];
                            dtThuVienMau.Rows.Add(newRow);
                        }

                        bool checkDuplicateData2 = _dtTable.AsEnumerable().GroupBy(row => new
                        {
                            DauSize = row.Field<string>("DauSize"),
                            POID = row.Field<string>("POID"),
                            PO = row.Field<string>("PO"),
                            MaMau = row.Field<string>("MaMau")
                        }).Any(g => g.Count() > 1);

                        if (checkDuplicateData2)
                        {
                            XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        string urlThuVienMau = string.Format("{0}?", URL + "DonHangTong/PostThuVienMau");
                        string msThuVienMau = Task.Run(async () => { return await _clientExtension.PostAsync(urlThuVienMau, dtThuVienMau); }).Result;
                        if (msThuVienMau.ToLower() != "true")
                            XtraMessageBox.Show(msThuVienMau);

                        string urlDonHangTong = string.Format("{0}?", URL + "DonHangTong/PostDonHangTongV1");
                        string msDonHangTong = Task.Run(async () => { return await _clientExtension.PostAsync(urlDonHangTong, listDonHangTong); }).Result;
                        if (msDonHangTong.ToLower() != "true")
                            XtraMessageBox.Show(msDonHangTong);
                        string urlDonHangTongPO = string.Format("{0}?", URL + "DonHangTong/PostDonHangTongPOUser");
                        string msDonHangTongPO = Task.Run(async () => { return await _clientExtension.PostAsync(urlDonHangTongPO, listDonHangTongPO); }).Result;
                        if (msDonHangTongPO.ToLower() != "true")
                            XtraMessageBox.Show(msDonHangTongPO);
                        string urlDonHangTongPOChiTiet = string.Format("{0}?", URL + "DonHangTong/PostDonHangTongPOChiTietUser");
                        string msDonHangTongPOChiTiet = Task.Run(async () => { return await _clientExtension.PostAsync(urlDonHangTongPOChiTiet, listDonHangTongPOChiTiet); }).Result;
                        if (msDonHangTongPOChiTiet.ToLower() != "true")
                            XtraMessageBox.Show(msDonHangTongPOChiTiet);
                        clsWaitForm.ShowSuccessForm(this, 3000);
                        this.Close();

                    }
                    else
                    {
                        XtraMessageBox.Show("Dữ liệu bị trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Dữ liệu bị trùng . Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //}

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtDauSize_Click(object sender, EventArgs e)
        {
            if (this.txtDauSize.Text.Trim().ToUpper() == "[D1:D2]")
            {
                this.txtDauSize.Text = "";
            }
        }

        private void txtNhapPO_Click(object sender, EventArgs e)
        {
            if (this.txtNhapPO.Text.Trim().ToUpper() == "[P1:P2]")
            {
                this.txtNhapPO.Text = "";
            }
        }

        private void txtNhapSize_Click(object sender, EventArgs e)
        {
            if (this.txtNhapSize.Text.Trim() == "[S1:S2]")
            {
                this.txtNhapSize.Text = "";
            }
        }

        private void txtDauSize_TextChanged(object sender, EventArgs e)
        {
            txtDauSize.Text = txtDauSize.Text.ToString().ToUpper();
            txtDauSize.SelectionStart = txtDauSize.Text.Length;
        }

        private void txtMau_TextChanged(object sender, EventArgs e)
        {
            txtMau.Text = txtMau.Text.ToString().ToUpper();
            txtMau.SelectionStart = txtMau.Text.Length;
        }

        private void txtMau_Click(object sender, EventArgs e)
        {
            //if (this.txtMau.Text.Trim().ToUpper() == "[M1:M2]")
            //{
            //    this.txtMau.Text = "";
            //}
        }

        public static string RemoveSpecialCharacters(string input)
        {
            string pattern = "[^a-zA-Z0-9]";
            string replaced = Regex.Replace(input, pattern, "");
            replaced = RemoveDiacritics(replaced);

            return replaced;
        }
        private void CheckBtnLuu()
        {
            //DataTable tbl = _dtTable;
            //if ((tbl != null && tbl.Rows.Count > 0) && (!string.IsNullOrEmpty(searchMH) && !string.IsNullOrEmpty(searchKH) && !string.IsNullOrEmpty(textDot)))
            //{
            //    this.Luu.Enabled = true;
            //    ActionControlSave.Enabled = true;
            //    //keyDownControlHandler.UpdateEventStatusEdit();
            //}
            //else
            //{
            //    this.Luu.Enabled = false;
            //    ActionControlSave.Enabled = false;
            //    //keyDownControlHandler.UpdateEventStatusView();
            //}
        }
        private void SearchLookupeditMH_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            if (changingEventArgs.NewValue == null) return;
            searchMH = changingEventArgs.NewValue.ToString();
            CheckBtnLuu();
            CreateSearchLookupCL();

            string urlCount = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetMHCount", searchLookUpEditMH.EditValue.ToString());
            string jsonCount = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCount); }).Result;
            DataTable tblCount = JsonConvert.DeserializeObject<DataTable>(jsonCount);

            int currentYear = DateTime.Now.Year;
            int countCurrentYear = 0;
            if (tblCount != null && tblCount.Rows.Count > 0)
            {
                countCurrentYear = tblCount.AsEnumerable()
                    .Count(r =>
                    {
                        DateTime ngayTao;
                        return DateTime.TryParse(r["NgayTao"]?.ToString(), out ngayTao) && ngayTao.Year == currentYear;
                    });
            }

            txtSoBooking.EditValue = currentYear + "-" + (countCurrentYear + 1);
        }

        private void SearchLookupeditKH_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            if (changingEventArgs.NewValue == null) return;
            searchKH = changingEventArgs.NewValue.ToString();
            string urlHH = string.Format("{0}?makh={1}", URL + "DonHangTong/GetHangHoaKH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditMH.Properties.DataSource = tblHH;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";

            string urlKH = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
            DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
            var rowqg = tblKH.AsEnumerable().FirstOrDefault(r => r["MaKH"].ToString().Trim() == searchLookUpEditKH.EditValue.ToString());
            SearchLookUpEditQG.EditValue = rowqg == null ? "" : rowqg["MaQG"].ToString().Trim();
            CheckBtnLuu();
        }

        private void txtNhapPO_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            //int ascii = Convert.ToInt32(e.KeyChar);
            //if (!(ascii == 58 || ascii == 32))
            //{
            //    if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            //    {
            //        e.Handled = true; // Chặn ký tự nhập vào
            //    }
            //}

            //// Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            //if (char.IsLetter(e.KeyChar))
            //{
            //    e.KeyChar = char.ToUpper(e.KeyChar);
            //}
        }

        private void txtDot_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtGhiChu_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtDauSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtNhapSize_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

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


        private void txtMau_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void btNhap_Click(object sender, EventArgs e)
        {
            grid = gridControlThongTinDonHang.DataSource as DataTable;
            try
            {
                check = 0;
                //if (searchLookUpEditMH.EditValue == null || searchLookUpEditKH.EditValue == null || searchLookUpEditCL.EditValue == null || txtDot.Text == "" || txtSoBooking.Text == null)
                //{
                //    MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu trên phần thông tin đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (_dtTable.Columns.Contains("CheckColumn"))
                {
                    _dtTable.Columns.Remove("CheckColumn");
                }
                if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtDot.Text == "" || txtDot.Text == null)
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập tần suất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (mau == null || mau == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin Màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (dausize == null || dausize == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin InSeam.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (size == null || size == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin Size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (SearchLookUpEditQG.EditValue == null || SearchLookUpEditQG.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Quốc gia.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchlookupEditHT.EditValue == null || searchlookupEditHT.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn hình thức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //if (txtSoVoice.Text == null || txtSoVoice.Text == "")
                //{
                //    MessageBox.Show("Vui lòng nhập số invoice.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (spinEditHSDH.Text == null || spinEditHSDH.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (spinEditFOB.Text == null || spinEditFOB.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập FOB.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (txtDot.Text == null || txtDot.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (spinEditUSD.Text == null || spinEditUSD.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (CheckDuplicateData())
                {
                    return;
                }
                string _maHang = searchLookUpEditMH.EditValue.ToString();

                List<string> _lstSize = txtNhapSize.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> _lstDauSize = txtDauSize.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                _dtTable = CreateDatatable();
                _dtSize = CreateDatatableSize();
                _dtSizeNhom = CreateDatatableSizeNhom();
                string jsonKtSize = string.Empty;
                string jsonMau = string.Empty;
                if (checkEditTV.Checked == false)
                {
                    string urlKtSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                    jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                }
                else
                {
                    string urlKtSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetBangSizeKT", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                }
                DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonKtSize);
                foreach (string Dausize in _lstDauSize)
                {
                    foreach (string size in _lstSize)
                    {
                        DataRow _dr = _dtSizeNhom.NewRow();
                        _dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                        _dr["SizeSanXuat"] = size.ToString().Trim();
                        _dr["MaNhomSize"] = Dausize.ToString().Replace(" ", "").Trim().ToUpper();
                        _dr["NhomSize"] = Dausize.ToString().Trim();
                        _dtSizeNhom.Rows.Add(_dr);
                    }
                }

                if (checkEditTV.Checked == true)
                {
                    var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                      where !tblSize.AsEnumerable().Any(row2 =>
                                          row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                          row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                          row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                      select row1;
                    if (missingRows.Any())
                    {
                        // Nhóm các dòng bị thiếu theo NhomSize
                        var groupedMissing = missingRows
                            .GroupBy(row => row["NhomSize"].ToString())
                            .Select(group => new
                            {
                                NhomSize = group.Key, // Tên nhóm size
                                Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                            });

                        // Tạo chuỗi thông báo
                        var message = string.Join("\n", groupedMissing.Select(group =>
                            $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                        // Hiển thị thông báo
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                //if (missingRows.Any())
                //{
                //    string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["SizeSanXuat"].ToString()).Distinct().ToArray());
                //    string nhomsizeMissing = string.Join("; ", missingRows.Select(r => r["NhomSize"].ToString()).Distinct().ToArray());
                //    MessageBox.Show("Size " + sizesanxuatMissing + " không có trong nhóm  " + nhomsizeMissing + " thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString() + " trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                //    return;
                //}
                var uniqueValues = _dtSizeNhom.AsEnumerable().Select(x => new { MaHang = x["MaHang"], SizeSanXuat = x["SizeSanXuat"] }).Distinct().ToList();
                foreach (var dr in uniqueValues)
                {
                    var rowmasize = tblSize.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == dr.SizeSanXuat.ToString().Trim());
                    if (rowmasize != null)
                        _dtTable.Columns.Add(rowmasize["MaSize"].ToString() + "@Size@" + dr.SizeSanXuat.ToString(), typeof(string));
                }
                DataTable _tblThongTin = CreateDataTableFromNhap();
                if (checkEditTV.Checked == false)
                {
                    string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                    jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                }
                else
                {
                    string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                }

                DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);

                if (checkEditTV.Checked == true)
                {
                    var RowsMau = from row1 in _tblThongTin.AsEnumerable()
                                  where !dtMau.AsEnumerable().Any(row2 =>
                                      searchLookUpEditMH.EditValue.ToString() == row2["MaHang"].ToString() &&
                                      row1["MauPO"].ToString() == row2["MaMau"].ToString())
                                  select row1;
                    if (RowsMau.Any())
                    {
                        string mauMissing = string.Join("; ", RowsMau.Select(r => r["MauPO"].ToString().Distinct().ToArray()));
                        MessageBox.Show("Màu " + mauMissing + " không có trong nhóm mã hàng " + searchLookUpEditMH.EditValue.ToString() + " trong thư viện màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        return;
                    }
                }

                foreach (DataRow _drmau in _tblThongTin.Rows)
                {
                    var rowmau = dtMau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString().Trim() == _drmau["MauPO"].ToString().Trim());
                    if (rowmau != null)
                        _drmau["MauPO"] = rowmau["MaMau"];

                }

                if (_tblThongTin != null && _tblThongTin.Rows.Count > 0)
                {
                    _dtTable = MapData(_dtTable, _tblThongTin);
                }
                _dtTable = _dtTable.AsEnumerable().OrderBy(x => x["MaMau"]).CopyToDataTable();
                gridBandSize.Children.Clear();
                createTable(_dtTable);
                CreateSearchLookupQuocGia();
                CreateSearchLookupMaMau();
                CreateSearchLookupDauSize();
                gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                mainView.CellValueChanging += mainView_CellValueChanging;
                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                gridControlThongTinDonHang.DataSource = _dtTable;
                //gridControl1.DataSource = _dtSize;
                //Luu.Enabled = true;
                CheckBtnLuu();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Thông tin nhập vào bị lỗi. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            DataTable dataSource1 = gridControlThongTinDonHang.DataSource as DataTable;
            if (!dataSource1.Columns.Contains("CheckColumn"))
            {
                dataSource1.Columns.Add("CheckColumn", typeof(bool));
            }

            // Gán giá trị mặc định true cho cột "CheckColumn" cho tất cả các dòng trong DataTable
            foreach (DataRow row in dataSource1.Rows)
            {
                row["CheckColumn"] = true;
            }

        }

        private void btImport_Click(object sender, EventArgs e)
        {
            try
            {
                check = 1;
                btnXacNhan.BackColor = Color.Navy;
                if (searchLookUpEditMH.EditValue == null || (searchLookUpEditMH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Mã Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditKH.EditValue == null || (searchLookUpEditKH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Khách Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditCL.EditValue == null || (searchLookUpEditCL.EditValue != null && string.IsNullOrEmpty(searchLookUpEditCL.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Chủng Loại trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (txtDot == null || (txtDot != null && string.IsNullOrEmpty(txtDot.Text.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Đợt trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (txtSoBooking == null || (txtSoBooking != null && string.IsNullOrEmpty(txtSoBooking.Text.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập số booking trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pathExecel = openFileDialog.FileName;
                    string conString = "", sheet1 = string.Empty;
                    int totalAmount = 0, Amount = 0;
                    using (OleDbConnection connection = new OleDbConnection(conString))
                    {
                        var source = new ExcelDataSource();
                        source.FileName = pathExecel;
                        string worksheetName = "";
                        using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                        {
                            DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                            worksheetName = worksheetCollection[0].Name;
                        }
                        var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A5:BZ500");
                        source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                        source.Fill();
                        DataTable tbl_ThongTinSize = new DataTable();
                        tbl_ThongTinSize = source.ToDataTable();
                        _dtTable = CreateDatatable();
                        _dtSize = CreateDatatableSize();
                        if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                        {

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString().ToString() == "***")
                                {
                                    break;
                                }
                                if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][0].ToString()))
                                {
                                    string po = tbl_ThongTinSize.Rows[j][0].ToString().Replace(" ", "").Trim();
                                    if (!string.IsNullOrEmpty(po) && !po.Trim().ToUpper().Contains("SOLUONG"))
                                    {
                                        if (po.Contains(" "))
                                        {
                                            po = po.Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(po))
                                        {
                                            XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][1].ToString()))
                                {
                                    string mau = tbl_ThongTinSize.Rows[j][1].ToString().Replace(" ", "").Trim();
                                    if (!string.IsNullOrEmpty(mau) && !mau.Trim().ToUpper().Contains("SOLUONG"))
                                    {
                                        if (mau.Contains(" "))
                                        {
                                            mau = mau.Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(mau))
                                        {
                                            XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }

                                }
                                else
                                {
                                    XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }

                            foreach (DataRow row in tbl_ThongTinSize.Rows)
                            {
                                foreach (DataColumn col in tbl_ThongTinSize.Columns)
                                {
                                    if (row[col].ToString().ToString() == "***")
                                    {
                                        break;
                                    }
                                    if (RemoveVietnameseTone(col.ToString().Trim()).ToUpper().Replace(" ", "").Contains("SOLUONG"))
                                    {
                                        try
                                        {
                                            Amount = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(row[col].ToString(), "[^0-9a-zA-Z]+", ""));
                                            totalAmount = totalAmount + Amount;
                                        }
                                        catch (Exception ex)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }

                            string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", searchLookUpEditMH.EditValue);
                            string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                            List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                            foreach (DonHangTongEntity donhang in dohanglist)
                            {
                                if (donhang.MaKH == searchLookUpEditKH.EditValue.ToString() && donhang.MaHang == searchLookUpEditMH.EditValue.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(txtDot.Text).Trim().ToUpper().ToString().Replace(" ", "")
                                   && donhang.SoLuong == totalAmount)
                                {
                                    DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (messResult == DialogResult.Yes)
                                    {
                                        return;
                                    }
                                    //XtraMessageBox.Show("Đã có đơn hàng cho mã hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //return;
                                }
                            }
                            string urlKtSize = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetBangSizeKT", searchLookUpEditMH.EditValue.ToString());
                            string jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonKtSize);
                            for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                            {
                                string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                                if (ProcessString(RemoveDiacritics(ColName.ToString())) == "SOLUONG")
                                    break;
                                string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                int sizeIndex = ColName.IndexOf("Size", StringComparison.OrdinalIgnoreCase);
                                if (sizeIndex < 0)
                                {
                                    string gia_tri = ColName.ToString().Trim();
                                    if (ProcessString(RemoveDiacritics(ColName.ToString())) == "SOLUONG")
                                        break;
                                    DataRow _dr = _dtSize.NewRow();
                                    _dr["MaSize"] = "";//"SIZE_" + ReplaceSpecialCharacters(gia_tri);
                                    _dr["TenSize"] = ColName;
                                    _dr["SizeSanXuat"] = ReplaceSpecialCharacterssize(gia_tri);
                                    _dr["CodeSize"] = "";
                                    _dtSize.Rows.Add(_dr);

                                }
                                else
                                {
                                    string sizePart = "Size";
                                    string gia_tri = ColName.Substring(sizeIndex + sizePart.Length).Trim();

                                    DataRow _dr = _dtSize.NewRow();
                                    _dr["MaSize"] = "";//"SIZE_" + ReplaceSpecialCharacters(gia_tri);
                                    _dr["TenSize"] = ColName;
                                    _dr["SizeSanXuat"] = ReplaceSpecialCharacterssize(gia_tri);
                                    _dr["CodeSize"] = "";
                                    _dtSize.Rows.Add(_dr);
                                }

                                //foreach (string gia_tri in mang_gia_tri)
                                //{
                                //    if (ProcessString(RemoveDiacritics(gia_tri.ToString())) != "SIZE")
                                // _dtTable.Columns.Add(gia_tri + "@Size@" + gia_tri, typeof(double));
                                //}

                            }
                            if (tblSize != null)
                            {
                                foreach (DataRow _drSize in _dtSize.Rows)
                                {
                                    foreach (DataRow _drsize in tblSize.Rows)
                                    {
                                        if (_drsize["XacNhan"].ToString() == "1")
                                        {
                                            if (_drsize["SizeXacNhan"].ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString())
                                            {
                                                //_drSize["MaSize"] = _drsize["MaSize"];
                                                // _drSize["TenSize"] = _drsize["TenSize"];
                                                //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                                                // _drSize["CodeSize"] = _drsize["CodeSize"];
                                                // _drSize["GhiChu"] = _drsize["GhiChu"];
                                                _drSize["XacNhan"] = _drsize["XacNhan"];
                                                _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                                            }
                                            else
                                            {
                                                if (_drsize["SizeSanXuat"].ToString().ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString())
                                                {
                                                    //_drSize["MaSize"] = _drsize["MaSize"];
                                                    //_drSize["TenSize"] = _drsize["TenSize"];
                                                    //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                                                    //_drSize["CodeSize"] = _drsize["CodeSize"];
                                                    //_drSize["GhiChu"] = _drsize["GhiChu"];
                                                    _drSize["XacNhan"] = _drsize["XacNhan"];
                                                    _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (_drsize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString())
                                            {
                                                //_drSize["MaSize"] = _drsize["MaSize"];
                                                //_drSize["TenSize"] = _drsize["TenSize"];
                                                //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                                                //_drSize["CodeSize"] = _drsize["CodeSize"];
                                                //_drSize["GhiChu"] = _drsize["GhiChu"];
                                                _drSize["XacNhan"] = _drsize["XacNhan"];
                                                _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                                            }
                                        }

                                    }
                                }
                            }


                            foreach (DataRow _drSize in _dtSize.Rows)
                            {
                                _dtTable.Columns.Add(_drSize["SizeSanXuat"].ToString().Trim().ToUpper() + "@Size@" + _drSize["TenSize"].ToString().Trim().ToUpper(), typeof(string));
                            }
                            string colorID = string.Empty, colorName = string.Empty, idPO = string.Empty, style = string.Empty, TenHang = string.Empty, colorStr = string.Empty, poID = string.Empty, countryName = string.Empty,
                            C_Size = string.Empty, C_DauSize = string.Empty, _ktLoi = string.Empty, ngayStr = string.Empty, SeaSon = string.Empty;
                            string newHeader = string.Empty, pp = string.Empty;
                            string dausize = string.Empty;
                            string _color = string.Empty, _po = string.Empty, _maQG = string.Empty;
                            string _ngayGH = string.Empty;
                            string error = string.Empty;

                            //Check và import Size
                            //List<string> lstImportSize = new List<string>();
                            //string urlsize = string.Format("{0}?", URL + "BangSize/GetBangSize");
                            //string jsonsize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsize); }).Result;
                            //List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonsize);
                            string _maHang = searchLookUpEditMH.EditValue.ToString().Trim();

                            //if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                            //{
                            //    for (int indexColumn = 3; indexColumn < tbl_ThongTinSize.Columns.Count - 1; indexColumn++)
                            //    {
                            //        DataColumn column = tbl_ThongTinSize.Columns[indexColumn];
                            //        if (column.ColumnName.ToUpper().Contains("SIZE"))
                            //        {
                            //            List<string> lstNameSize = column.ColumnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //            if (lstNameSize != null && lstNameSize.Count > 1)
                            //            {
                            //                string sizeSX = lstNameSize[1];
                            //                BangSizeEntity bangSize = lstBangSize.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper().Trim()
                            //                == sizeSX.ToString().Trim() + _maHang).FirstOrDefault();
                            //                if (bangSize == null && column != null && !string.IsNullOrEmpty(column.ColumnName))
                            //                {
                            //                    lstImportSize.Add(column.ColumnName);
                            //                }
                            //            }
                            //            Console.WriteLine("Check_Size");
                            //        }
                            //    }
                            //    AutoImportSize(lstImportSize, true);
                            //}
                            // Size

                            // Check và import màu
                            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);

                            List<string> lstSaveMau = new List<string>();

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString() == "***")
                                {
                                    break;
                                }
                                //
                                BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == tbl_ThongTinSize.Rows[j][1].ToString().Trim() + searchLookUpEditMH.EditValue.ToString().Trim()).FirstOrDefault();
                                if (bangMau == null)
                                {
                                    string hasDuplicates = lstSaveMau.Where(x => x.Equals(tbl_ThongTinSize.Rows[j][1].ToString())).FirstOrDefault();
                                    if (string.IsNullOrEmpty(hasDuplicates))
                                    {
                                        lstSaveMau.Add(tbl_ThongTinSize.Rows[j][1].ToString());
                                    }

                                }
                            }

                            if (lstSaveMau.Count > 0)
                            {
                                AutoImportMau(lstSaveMau);
                            }

                            DataTable dataTablChanged = ChangeTenMauToMaMau(tbl_ThongTinSize, _maHang);

                            if (dataTablChanged != null && dataTablChanged.Rows.Count > 0)
                            {
                                _dtTable = MapData(_dtTable, dataTablChanged);
                            }


                            // begin: code cũ
                            //for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            //{
                            //if (tbl_ThongTinSize.Rows[j][0].ToString() == "***")
                            //{
                            //    break;
                            //}
                            //    #region
                            //    //
                            //    BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == tbl_ThongTinSize.Rows[j][1].ToString().Trim() + searchLookUpEditMH.EditValue.ToString().Trim()).FirstOrDefault();
                            //    if (bangMau == null)
                            //    {
                            //        lstSaveMau.Add(tbl_ThongTinSize.Rows[j][1].ToString());
                            //        AutoImportMau(lstSaveMau);
                            //    }
                            //    //
                            //    string urlMauLoad = string.Format("{0}?", URL + "BangMau/GetBangMau");
                            //    string jsonMauLoad = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMauLoad); }).Result;
                            //    List<BangMauEntity> lstBangMauLoad = JsonConvert.DeserializeObject<List<BangMauEntity>>(jsonMauLoad);
                            //    string urlQGLoad = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                            //    string jsonQGLoad = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQGLoad); }).Result;
                            //    List<QuocGiaEntity> lstQGLoad = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQGLoad);
                            //    DataRow row = _dtTable.NewRow();
                            //    if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][0].ToString()))
                            //    {
                            //        row["POID"] = txtMaDH.Text.ToString() + '|' + tbl_ThongTinSize.Rows[j][0].ToString();
                            //        row["PO"] = tbl_ThongTinSize.Rows[j][0].ToString();
                            //        string resultStringipmau = RemoveDiacritics(tbl_ThongTinSize.Rows[j][1].ToString().Trim());
                            //        string doiqgipmau = Regex.Replace(resultStringipmau.ToString(), @"[^\w\d]+", "_");
                            //        string doiqgidipmau = RemoveOneUnderscore(doiqgipmau);
                            //        BangMauEntity mauloadip = lstBangMauLoad.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == doiqgidipmau.ToString().Trim() + searchLookUpEditMH.EditValue.ToString().Trim()).FirstOrDefault();
                            //        row["MaMau"] = mauloadip.MaMau;
                            //        row["DauSize"] = tbl_ThongTinSize.Rows[j][2].ToString();
                            //        row["NgayGH"] = DateTime.Now.Date;
                            //        string resultStringipqg = RemoveDiacritics(tbl_ThongTinSize.Rows[j][3].ToString().Trim());
                            //        string doiqgipqg = Regex.Replace(resultStringipqg.ToString(), @"[^\w\d]+", "_");
                            //        string doiqgidipqg = RemoveOneUnderscore(doiqgipqg);
                            //        QuocGiaEntity quocgiaload = lstQGLoad.Where(item => RemoveVietnameseTone(item.TenQG).ToUpper().Trim() == doiqgidipqg.ToString().ToUpper().Trim()).FirstOrDefault();
                            //        //row["MaQG"] = "";
                            //        //row["NgayGH"] =  null;
                            //    }
                            //    #endregion
                            //    for (int z = 3; z <= tbl_ThongTinSize.Columns.Count - 1; z++)
                            //    {
                            //        for (int h = 7; h <= _dtTable.Columns.Count - 1; h++)
                            //        {
                            //            string[] arrNewHeader = _dtTable.Columns[h].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            //            newHeader = arrNewHeader[2];
                            //            string[] arrNewHeader1 = tbl_ThongTinSize.Columns[z].ColumnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //            string sizeID = arrNewHeader1[1];
                            //            if (sizeID == newHeader)
                            //            {
                            //                string _size = sizeID.ToString().Replace(" ", "").Trim().ToUpper() + "@" + "Size" + "@" + sizeID.ToString().Replace(" ", "").Trim().ToUpper();
                            //                row[_size] = tbl_ThongTinSize.Rows[j][z].ToString();
                            //            }
                            //        }
                            //    }
                            //    _dtTable.Rows.Add(row);
                            //}
                            // end: code cũ
                            bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                            {
                                DauSize = row.Field<string>("DauSize"),
                                POID = row.Field<string>("POID"),
                                MaMau = row.Field<string>("MaMau")
                            }).Any(g => g.Count() > 1);
                            if (checkDuplicateData)
                            {
                                XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _dtTable.Clear();
                                return;
                            }
                            gridBandSize.Children.Clear();
                            createTable(_dtTable);
                            CreateSearchLookupQuocGia();
                            CreateSearchLookupMaMau();
                            CreateSearchLookupDauSize();
                            gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                            BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                            mainView.CellValueChanging += mainView_CellValueChanging;
                            //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                            mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                            //mainView.CustomDrawCell += MainView_CustomDrawCell;
                            gridControlThongTinDonHang.DataSource = _dtTable;
                            gridControl1.DataSource = _dtSize;
                            gridControl1.RefreshDataSource();
                            //gridControlThongTinDonHang.DataSource = dataTablChanged;
                            CreateSearchLookupQG();
                            CheckBtnLuu();

                        }
                        else
                        {
                            XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        private void SearchLookupeditCL_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            if (changingEventArgs.NewValue == null) return;
            searchCL = changingEventArgs.NewValue.ToString();
            CheckBtnLuu();
        }


        private void searchLookUpEditMH_Properties_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmHangHoa frm = new frmHangHoa();
            frm.ShowDialog();
            CreateSearchLookup();
        }

        private void searchLookUpEditKH_Properties_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmKhachHang frm = new frmKhachHang();
            frm.ShowDialog();
            CreateSearchLookup();
        }

        private void searchLookUpEditCL_Properties_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmChungLoai frm = new frmChungLoai();
            frm.ShowDialog();
            CreateSearchLookup();
        }

        private void SearchLookUpEditQG_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmQuocGia frm = new frmQuocGia();
            frm.ShowDialog();
            CreateSearchLookupQG();
        }

        private void TextEditDot_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            textDot = changingEventArgs.NewValue.ToString();
            CheckBtnLuu();
        }

        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {

                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'Đ')
                    {
                        stringBuilder.Append('D');
                    }
                    else if (c == 'đ')
                    {
                        stringBuilder.Append('d');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void txtMaHS_TextChanged(object sender, EventArgs e)
        {
            txtMaHS.Text = txtMaHS.Text.ToString().ToUpper();
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

        private void BandedView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        static string ReplaceSpecialCharacters(string input)
        {
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        static string ReplaceSpecialCharacterssize(string input)
        {
            string pattern = @"[^\w\s\(\)]+";
            // Thay thế các ký tự đặc biệt bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement).Replace(" ", "");
        }
        private void btImportNew_Click(object sender, EventArgs e)
        {
            try
            {
                check = 1;
                // btnXacNhan.BackColor = Color.Navy;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pathExecel = openFileDialog.FileName;
                    string conString = "", sheet1 = string.Empty;
                    int totalAmount = 0, Amount = 0;
                    using (OleDbConnection connection = new OleDbConnection(conString))
                    {
                        var source = new ExcelDataSource();
                        source.FileName = pathExecel;
                        string worksheetName = "";
                        using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                        {
                            DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                            worksheetName = worksheetCollection[0].Name;
                        }
                        var worksheetSettingss = new ExcelWorksheetSettings(worksheetName, "$A1:D7");
                        source.SourceOptions = new ExcelSourceOptions(worksheetSettingss);
                        source.Fill();
                        DataTable tbl_Styles = new DataTable();
                        tbl_Styles = source.ToDataTable();

                        string pp = string.Empty;
                        string style = tbl_Styles.Rows[1][3].ToString().TrimStart().TrimEnd();
                        var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A9:BZ500");
                        source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                        source.Fill();
                        DataTable tbColorPO = new DataTable();
                        tbColorPO = source.ToDataTable();
                        _dtTable = CreateDatatable();
                        _dtSizeNhom = CreateDatatableSizeNhom();
                        string styleID = "";
                        string cusName = "";
                        if (style.Length > 100)
                        {
                            XtraMessageBox.Show("Mã hàng không được dài quá 100 kí tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        string _styleID = style.ToString().TrimStart().TrimEnd();
                        if (string.IsNullOrEmpty(_styleID))
                        {
                            XtraMessageBox.Show("Mã hàng không được bỏ trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        string urlmhang = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
                        string jsonmhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmhang); }).Result;
                        DataTable _dtMaHang = JsonConvert.DeserializeObject<DataTable>(jsonmhang);
                        if (_dtMaHang == null)
                        {
                            XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var rowmh = _dtMaHang.AsEnumerable().FirstOrDefault(r => r["TenHang"].ToString() == _styleID.ToString());
                        if (rowmh != null)
                        {
                            styleID = rowmh["MaHang"].ToString();
                        }
                        else
                        {
                            XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // kiểm tra khách hàng
                        string _cusName = tbl_Styles.Rows[0][3].ToString().TrimStart().TrimEnd();
                        if (string.IsNullOrEmpty(_cusName))
                        {
                            XtraMessageBox.Show("Không được bỏ trống khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
                        string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                        DataTable _dtKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                        if (_dtKH == null)
                        {
                            XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var rowkh = _dtKH.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == _cusName.ToString().Trim());
                        if (rowkh != null)
                        {
                            cusName = rowkh["MaKH"].ToString();
                        }
                        else
                        {
                            XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        string urlSize = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetSize", styleID.ToString());
                        string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                        DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                        int Grand = 0;
                        for (int j = 1; j < tbColorPO.Rows.Count; j++)
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                            {
                                if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;
                                continue;
                            }
                            else
                            {
                                for (int c = 5; c < tbColorPO.Columns.Count - 1; c++)
                                {
                                    if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                        break;

                                    if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                    {
                                        MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                        return;
                                    }
                                    else
                                    {
                                        if (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") != "")
                                            totalAmount = totalAmount + (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") == "" ? 0 : Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", ""), "[^0-9a-zA-Z]+", "")));//row[col].ToString().Replace());
                                    }

                                }
                            }
                        }

                        //string cleanedStringktmh = Regex.Replace(styleID, @"[^\w\d-]", "_");
                        //cleanedStringktmh = cleanedStringktmh.Replace(" ", "_");
                        string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", styleID);
                        string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                        List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                        foreach (DonHangTongEntity donhang in dohanglist)
                        {
                            if (donhang.MaKH == cusName.ToString() && donhang.MaHang == styleID.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(tbl_Styles.Rows[2][3].ToString().TrimStart().TrimEnd()).Trim().ToUpper().ToString().Replace(" ", "")
                               && donhang.SoLuong == totalAmount)
                            {
                                DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (messResult == DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                        }

                        string sizetrung = "", _inseam = "";
                        for (int j = 1; j < tbColorPO.Columns.Count - 1; j++)
                        {
                            if (!string.IsNullOrEmpty(tbColorPO.Rows[j][1].ToString()))
                            {
                                _inseam = tbColorPO.Rows[j][2].ToString().Trim();
                            }
                            if (_inseam == "" && !_inseam.ToUpper().Contains("GRAND"))
                            {
                                MessageBox.Show("InSeam không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                return;
                            }

                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                            {
                                if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;
                            }
                            for (int i = 5; i < tbColorPO.Columns.Count - 1; i++)
                            {

                                if (!string.IsNullOrEmpty(tbColorPO.Columns[i].ColumnName.ToString()))
                                {
                                    pp = tbColorPO.Columns[i].ColumnName;
                                }

                                if (pp.Trim().ToUpper().Contains("TOTAL"))
                                {
                                    if (pp.Trim().ToUpper().Contains("GRAND"))
                                        break;
                                    continue;
                                }

                                string sizeID = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper() + "@Size@" + tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                                if (sizeID.ToString() == "")
                                {
                                    XtraMessageBox.Show("Size không được bỏ trống. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (sizetrung.ToString() == sizeID.ToString())
                                {
                                    XtraMessageBox.Show("Size bị trùng. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                var rowsize = _dtSizeNhom.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper()
                                && r["NhomSize"].ToString().Trim() == _inseam.ToString().Trim());
                                if (rowsize != null) break;
                                DataRow _dr = _dtSizeNhom.NewRow();
                                _dr["MaHang"] = styleID;
                                _dr["SizeSanXuat"] = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                                _dr["MaNhomSize"] = _inseam.ToString().Replace(" ", "").Trim().ToUpper();
                                _dr["NhomSize"] = _inseam.ToString().Trim();
                                _dtSizeNhom.Rows.Add(_dr);
                                sizetrung = sizeID;

                            }
                        }  // Đối chiếu dữ liệu và kiểm tra
                        var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                          where !dtSize.AsEnumerable().Any(row2 =>
                                              row1["mahang"].ToString() == row2["mahang"].ToString() &&
                                              row1["sizesanxuat"].ToString() == row2["sizesanxuat"].ToString() &&
                                              row1["nhomsize"].ToString() == row2["nhomsize"].ToString())
                                          select row1;
                        if (missingRows.Any())
                        {
                            string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["sizesanxuat"].ToString()));
                            MessageBox.Show("Size " + sizesanxuatMissing + " không có trong thư viện.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                            return;
                        }
                        foreach (DataRow dr in _dtSizeNhom.Rows)
                        {
                            var rowmasize = dtSize.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == dr["SizeSanXuat"].ToString().Trim());
                            if (rowmasize != null)
                                _dtTable.Columns.Add(rowmasize["MaSize"].ToString() + "@" + dr["SizeSanXuat"].ToString(), typeof(string));
                        }
                        string _strdausize = string.Empty, _sizetrung = string.Empty;
                        //string urlKtSize = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetBangSizeKT", Regex.Replace(styleID, @"[^\w\d-]", "_").Replace(" ", "_"));
                        //string jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                        //DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonKtSize);
                        //for (int i = 5; i < tbColorPO.Columns.Count - 1; i++)
                        //{
                        //    if (_strdausize != tbColorPO.Columns[i].ColumnName && !tbColorPO.Columns[i].ColumnName.Trim().ToUpper().Contains("COLUMN"))
                        //    {
                        //        _strdausize = tbColorPO.Columns[i].ColumnName;
                        //    }
                        //    if (!string.IsNullOrEmpty(tbColorPO.Columns[i].ColumnName.ToString()))
                        //    {
                        //        pp = tbColorPO.Columns[i].ColumnName;
                        //    }

                        //    if (pp.Trim().ToUpper().Contains("TOTAL"))
                        //    {
                        //        if (pp.Trim().ToUpper().Contains("GRAND"))
                        //            break;
                        //        continue;
                        //    }

                        //    string sizeID = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper() + "@Size@" + tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                        //    if (sizeID.ToString() == "")
                        //    {
                        //        XtraMessageBox.Show("Size không được bỏ trống. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //        return;
                        //    }
                        //    if (_sizetrung.ToString() == sizeID.ToString())
                        //    {
                        //        XtraMessageBox.Show("Size bị trùng. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //        return;
                        //    }
                        //    //DataRow _dr = _dtSize.NewRow();
                        //    //_dr["MaSize"] = "";//"SIZE_" + ReplaceSpecialCharacters(tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper());
                        //    //_dr["TenSize"] = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                        //    //_dr["SizeSanXuat"] = ReplaceSpecialCharacterssize(tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper());
                        //    //_dr["CodeSize"] = "";
                        //    //_dtSize.Rows.Add(_dr);
                        //    _dtTable.Columns.Add(sizeID, typeof(string));
                        //    _sizetrung = sizeID;

                        //}
                        //if (tblSize != null)
                        //{
                        //    foreach (DataRow _drSize in _dtSize.Rows)
                        //    {
                        //        foreach (DataRow _drsize in tblSize.Rows)
                        //        {
                        //            if (_drsize["XacNhan"].ToString() == "1")
                        //            {
                        //                if (_drsize["SizeXacNhan"].ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == cleanedStringktmh.ToString())
                        //                {
                        //                    //_drSize["MaSize"] = _drsize["MaSize"];
                        //                    //_drSize["TenSize"] = _drsize["TenSize"];
                        //                    //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                        //                    //_drSize["CodeSize"] = _drsize["CodeSize"];
                        //                    //_drSize["GhiChu"] = _drsize["GhiChu"];
                        //                    _drSize["XacNhan"] = _drsize["XacNhan"];
                        //                    _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                        //                }
                        //                else
                        //                {
                        //                    if (_drsize["SizeSanXuat"].ToString().ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == cleanedStringktmh.ToString())
                        //                    {
                        //                        //_drSize["MaSize"] = _drsize["MaSize"];
                        //                        //_drSize["TenSize"] = _drsize["TenSize"];
                        //                        //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                        //                        //_drSize["CodeSize"] = _drsize["CodeSize"];
                        //                        //_drSize["GhiChu"] = _drsize["GhiChu"];
                        //                        _drSize["XacNhan"] = _drsize["XacNhan"];
                        //                        _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (_drsize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == cleanedStringktmh.ToString())
                        //                {
                        //                    //_drSize["MaSize"] = _drsize["MaSize"];
                        //                    //_drSize["TenSize"] = _drsize["TenSize"];
                        //                    //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                        //                    //_drSize["CodeSize"] = _drsize["CodeSize"];
                        //                    //_drSize["GhiChu"] = _drsize["GhiChu"];
                        //                    _drSize["XacNhan"] = _drsize["XacNhan"];
                        //                    _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                        //                }
                        //            }

                        //        }
                        //    }
                        //}

                        //foreach (DataRow _drSize in _dtSize.Rows)
                        //{
                        //    _dtTable.Columns.Add(_drSize["SizeSanXuat"].ToString().Trim().ToUpper() + "@Size@" + _drSize["TenSize"].ToString().Trim().ToUpper(), typeof(string));
                        //}
                        string newHeader = string.Empty;
                        string dausize = string.Empty;
                        string _colorID = string.Empty, _colorName = string.Empty, _po = string.Empty, _maQG = string.Empty, _dausize = string.Empty;
                        DateTime _ngayGH;
                        string error = string.Empty, _potrung = string.Empty;

                        string ngayBHStr = string.Empty;
                        for (int j = 1; j < tbColorPO.Rows.Count; j++)
                        {
                            //_po = "";
                            //_colorName = "";
                            //ngayBHStr = "";
                            _colorID = tbColorPO.Rows[j][0].ToString();
                            if (!string.IsNullOrEmpty(tbColorPO.Rows[j][0].ToString()))
                            {
                                _colorName = tbColorPO.Rows[j][0].ToString().Trim();
                            }
                            if (_colorName == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                            {
                                MessageBox.Show("Màu không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                return;
                            }
                            if (!string.IsNullOrEmpty(tbColorPO.Rows[j][1].ToString()))
                            {
                                _po = tbColorPO.Rows[j][1].ToString().Trim();
                            }
                            if (_po == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                            {
                                MessageBox.Show("PO không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                return;
                            }
                            if (!string.IsNullOrEmpty(tbColorPO.Rows[j][2].ToString()))
                            {
                                _dausize = tbColorPO.Rows[j][2].ToString().Trim().Replace("\"", "");
                            }
                            if (!string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()))
                            {
                                _maQG = tbColorPO.Rows[j][3].ToString().Trim();
                            }

                            if (!string.IsNullOrEmpty(tbColorPO.Rows[j][4].ToString()))
                            {
                                DateTime date_ngayBH;
                                if (DateTime.TryParseExact(tbColorPO.Rows[j][4].ToString().Trim().Replace("-", "/"), new[] { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss", "MM/dd/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date_ngayBH))
                                {
                                    ngayBHStr = date_ngayBH.ToString("dd/MM/yyyy"); // Chuyển đổi định dạng thành "dd/MM/yyyy"
                                }
                                else
                                {
                                    MessageBox.Show("Kiểm tra lại Ngày Giao Hàng chưa đúng định dạng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                    return;
                                }

                            }
                            if (ngayBHStr == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                            {
                                MessageBox.Show("Ngày giao hàng không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                return;
                            }
                            if (_colorID.Trim().ToUpper().Contains("TOTAL"))
                            {
                                _po = "";
                                _colorName = "";
                                ngayBHStr = "";
                                _maQG = "";
                                if (_colorID.Trim().ToUpper().Contains("GRAND"))
                                    break;
                                continue;
                            }
                            else
                            {
                                DataRow _dr = _dtTable.NewRow();
                                int n = 7;
                                for (int c = 0; c < tbColorPO.Columns.Count - 1; c++)
                                {
                                    if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                        break;
                                    if (c <= 4)
                                    {
                                        _dr[0] = txtMaDH.Text + "|" + Regex.Replace(_po.Trim(), @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_");
                                        _dr[1] = _po.Trim();
                                        _dr[2] = _colorName.Trim();
                                        _dr[3] = _dausize.ToString().Trim() == "" ? "0" : _dausize.Trim();
                                        _dr[4] = _maQG.Trim();
                                        DateTime dateValue;

                                        try
                                        {
                                            dateValue = DateTime.ParseExact(ngayBHStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                            _dr[5] = dateValue;
                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine("Chuỗi không có định dạng hợp lệ.");
                                        }

                                    }
                                    else
                                    {
                                        if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                        {
                                            MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                            return;
                                        }
                                        else
                                        {
                                            _dr[n] = tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "");
                                            n++;
                                        }



                                    }

                                }

                                _dtTable.Rows.Add(_dr);
                            }

                        }
                        string urlMau = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetMau", styleID.ToString());
                        string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                        DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
                        var RowsMau = from row1 in _dtTable.AsEnumerable()
                                      where !dtMau.AsEnumerable().Any(row2 =>
                                          styleID.ToString() == row2["MaHang"].ToString() &&
                                          row1["MaMau"].ToString() == row2["TenMau"].ToString())
                                      select row1;
                        if (RowsMau.Any())
                        {
                            string mauMissing = string.Join("; ", RowsMau.Select(r => r["MaMau"].ToString()));
                            MessageBox.Show("Mau " + mauMissing + " không có trong thư viện.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                            return;
                        }
                        foreach (DataRow _drmau in _dtTable.Rows)
                        {
                            var rowmau = dtMau.AsEnumerable().FirstOrDefault(r => r["TenMau"].ToString().Trim() == _drmau["MaMau"].ToString().Trim());
                            if (rowmau != null)
                                _drmau["MaMau"] = rowmau["MaMau"];

                        }
                        //string mahang = string.Empty;
                        //string khachhang = string.Empty;
                        //string chungloai = string.Empty;
                        //string cleanedStringmh = Regex.Replace(styleID, @"[^\w\d-]", "_");
                        //cleanedStringmh = cleanedStringmh.Replace(" ", "_");
                        //string cleanedStringkh = Regex.Replace(cusName, @"[^\w\d-]", "_");
                        //cleanedStringkh = cleanedStringkh.Replace(" ", "_");

                        //// Check và import ma hang
                        //string urlMH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
                        //string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
                        //List<HangHoaEntity> lstMH = JsonConvert.DeserializeObject<List<HangHoaEntity>>(jsonMH);
                        //HangHoaEntity mh = lstMH.Where(item => RemoveVietnameseTone(item.TenHang).ToUpper().Trim() == RemoveVietnameseTone(styleID).ToUpper().Trim()).FirstOrDefault();
                        //List<string> lstSaveMH = new List<string>();
                        //if (mh == null)
                        //{
                        //    List<HangHoaEntity> lstAutoImportMH = new List<HangHoaEntity>();
                        //    HangHoaEntity itemMH = new HangHoaEntity();
                        //    itemMH.MaHang = cleanedStringmh;
                        //    itemMH.TenHang = styleID;
                        //    lstAutoImportMH.Add(itemMH);
                        //    string urlmh = string.Format("{0}?", URL + "HangHoa/PostHangHoa");
                        //    string msResultmh = Task.Run(async () => { return await _clientExtension.PostAsync(urlmh, lstAutoImportMH); }).Result;
                        //    if (msResultmh.ToLower() == "true")
                        //    {
                        //        mahang = cleanedStringmh;
                        //    }
                        //    else
                        //    {
                        //        XtraMessageBox.Show("Tạo tự động mã hàng đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //        return;
                        //    }


                        //}
                        //else
                        //{
                        //    mahang = mh.MaHang;
                        //}

                        //// Check và import khach hang
                        //string urlkhachhang = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
                        //string jsonkhachhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkhachhang); }).Result;
                        //List<KhachHangEntity> lstKH = JsonConvert.DeserializeObject<List<KhachHangEntity>>(jsonkhachhang);
                        //KhachHangEntity kh = lstKH.Where(item => RemoveVietnameseTone(item.TenKH).ToUpper().Trim() == RemoveVietnameseTone(cusName).ToUpper().Trim()).FirstOrDefault();
                        //List<string> lstSaveKH = new List<string>();
                        //if (kh == null)
                        //{
                        //    List<KhachHangEntity> lstAutoImportKH = new List<KhachHangEntity>();
                        //    KhachHangEntity itemKH = new KhachHangEntity();
                        //    itemKH.TenKH = cusName;
                        //    lstAutoImportKH.Add(itemKH);
                        //    string urlkh = string.Format("{0}?", URL + "KhachHang/PostAutoKH");
                        //    string msResultkh = Task.Run(async () => { return await _clientExtension.PostAsync(urlkh, lstAutoImportKH); }).Result;
                        //    if (msResultkh.ToLower() == "true")
                        //    {
                        //        string urlMaKH = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
                        //        string jsonMaKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMaKH); }).Result;
                        //        List<KhachHangEntity> lstMaKH = JsonConvert.DeserializeObject<List<KhachHangEntity>>(jsonMaKH);
                        //        KhachHangEntity makh = lstMaKH.Where(item => RemoveVietnameseTone(item.TenKH).ToUpper().Trim() == RemoveVietnameseTone(cusName).ToUpper().Trim()).FirstOrDefault();

                        //        khachhang = makh == null ? "" : makh.MaKH;
                        //    }
                        //    else
                        //    {
                        //        XtraMessageBox.Show("Tạo tự động khách hàng đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //    khachhang = kh.MaKH;
                        //}

                        //// Check và import chung loai
                        //if (tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd().ToUpper().Trim() != "")
                        //{
                        //    string urlCL = string.Format("{0}?", URL + "ChungLoai/GetChungLoai");
                        //    string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
                        //    List<ChungLoaiEntity> lstCL = JsonConvert.DeserializeObject<List<ChungLoaiEntity>>(jsonCL);
                        //    ChungLoaiEntity cl = lstCL.Where(item => item.TenCL.ToUpper().Trim() == tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd().ToUpper().Trim()).FirstOrDefault();
                        //    List<string> lstSaveCL = new List<string>();
                        //    if (cl == null)
                        //    {
                        //        DialogResult messResult = MessageBox.Show("Chủng loại hàng chưa được khai báo trong thư viện. Bạn có muốn tạo tự động chủng loại hàng không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //        if (messResult == DialogResult.No)
                        //        {
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            List<ChungLoaiEntity> lstAutoImportCL = new List<ChungLoaiEntity>();
                        //            ChungLoaiEntity itemCL = new ChungLoaiEntity();
                        //            itemCL.TenCL = tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd();
                        //            lstAutoImportCL.Add(itemCL);
                        //            string urlcl = string.Format("{0}?", URL + "ChungLoai/PostChungLoai");
                        //            string msResultcl = Task.Run(async () => { return await _clientExtension.PostAsync(urlcl, lstAutoImportCL); }).Result;
                        //            if (msResultcl.ToLower() == "true")
                        //            {
                        //                string urlMaCL = string.Format("{0}?", URL + "ChungLoai/GetChungLoai");
                        //                string jsonMaCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMaCL); }).Result;
                        //                List<ChungLoaiEntity> lstMaCL = JsonConvert.DeserializeObject<List<ChungLoaiEntity>>(jsonMaCL);
                        //                ChungLoaiEntity macl = lstMaCL.Where(item => RemoveVietnameseTone(item.TenCL).ToUpper().Trim() == RemoveVietnameseTone(tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd()).ToUpper().Trim()).FirstOrDefault();

                        //                chungloai = macl == null ? "" : macl.MaCL;
                        //            }
                        //            else
                        //            {
                        //                XtraMessageBox.Show("Tạo tự động chủng loại đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //                return;
                        //            }
                        //        }

                        //    }
                        //    else
                        //    {
                        //        chungloai = cl.MaCL;
                        //    }
                        //}
                        //else
                        //    chungloai = "";



                        // Check và import Size
                        //List<string> lstImportSize = new List<string>();
                        //string urlsize = string.Format("{0}?", URL + "BangSize/GetBangSize");
                        //string jsonsize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsize); }).Result;
                        //List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonsize);

                        //if (_dtTable != null && _dtTable.Rows.Count > 0)
                        //{
                        //    for (int indexColumn = 7; indexColumn < _dtTable.Columns.Count; indexColumn++)
                        //    {
                        //        DataColumn column = _dtTable.Columns[indexColumn];
                        //        if (column.ColumnName.ToUpper().Contains("SIZE"))
                        //        {
                        //            List<string> lstNameSize = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        //            if (lstNameSize != null && lstNameSize.Count > 1)
                        //            {
                        //                string sizeSX = lstNameSize[0];
                        //                BangSizeEntity bangSize = lstBangSize.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper().Trim()
                        //                == sizeSX.ToString().Trim() + mahang).FirstOrDefault();
                        //                if (bangSize == null && column != null && !string.IsNullOrEmpty(column.ColumnName))
                        //                {
                        //                    lstImportSize.Add(column.ColumnName);
                        //                }
                        //            }

                        //        }
                        //    }
                        //    AutoImportSizeNew(lstImportSize, true, mahang);
                        //}
                        //// Check và import màu
                        //string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                        //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                        //List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);

                        //List<string> lstSaveMau = new List<string>();

                        //for (int j = 0; j < _dtTable.Rows.Count; j++)
                        //{
                        //    BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim().Replace(" ", "") == (_dtTable.Rows[j][2].ToString().Trim() + styleID.Trim()).ToUpper().Trim().Replace(" ", "")).FirstOrDefault();
                        //    //BangMauEntity bangMauSave = lstSaveMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == _dtTable.Rows[j][2].ToString().Trim() + mahang.Trim()).FirstOrDefault();                       
                        //    if (bangMau == null)
                        //    {
                        //        //bool hasDuplicates = lstSaveMau.GroupBy(x => x).Any(g => g.Count() > 1);
                        //        string hasDuplicates = lstSaveMau.Where(x => x.Equals(_dtTable.Rows[j][2].ToString())).FirstOrDefault();
                        //        if (string.IsNullOrEmpty(hasDuplicates))
                        //        {
                        //            lstSaveMau.Add(_dtTable.Rows[j][2].ToString());
                        //        }

                        //    }
                        //}

                        //if (lstSaveMau.Count > 0)
                        //{
                        //    AutoImportMauNew(lstSaveMau, styleID);
                        //}

                        //_dtTable = ChangeTenMauToMaMauNew(_dtTable, styleID);


                        // Check và import QG
                        string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                        string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                        List<QuocGiaEntity> lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQG);

                        List<string> lstSaveQG = new List<string>();

                        for (int j = 0; j < _dtTable.Rows.Count; j++)
                        {

                            string KTQG = string.Empty;
                            QuocGiaEntity qg = lstQG.Where(item => RemoveVietnameseTone(item.TenQG).ToString().ToUpper().Trim() == RemoveVietnameseTone(_dtTable.Rows[j][4].ToString().ToUpper().Trim())).FirstOrDefault();
                            if (qg == null)
                            {
                                string hasDuplicates = lstSaveQG.Where(x => x.Equals(_dtTable.Rows[j][4].ToString())).FirstOrDefault();
                                if (string.IsNullOrEmpty(hasDuplicates))
                                {
                                    if (_dtTable.Rows[j][4].ToString() != "")
                                        lstSaveQG.Add(_dtTable.Rows[j][4].ToString());
                                }
                            }
                        }
                        if (lstSaveQG.Count > 0)
                        {
                            AutoImportQGNew(lstSaveQG);
                        }
                        _dtTable = ChangeTenQGToMaQGNew(_dtTable);
                        searchLookUpEditMH.EditValue = styleID;
                        searchLookUpEditKH.EditValue = cusName;
                        searchLookUpEditCL.EditValue = "";
                        txtDot.Text = tbl_Styles.Rows[2][3].ToString().TrimStart().TrimEnd();
                        CreateSearchLookup();
                        CreateSearchLookupQG();
                        CreateSearchLookupQuocGia();
                        CreateSearchLookupMaMau();
                        CreateSearchLookupDauSize();
                        bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                        {
                            DauSize = row.Field<string>("DauSize"),
                            POID = row.Field<string>("POID"),
                            MaMau = row.Field<string>("MaMau")
                        }).Any(g => g.Count() > 1);
                        if (checkDuplicateData)
                        {
                            XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _dtTable.Clear();
                            searchLookUpEditMH.EditValue = null;
                            searchLookUpEditKH.EditValue = null;
                            txtDot.Text = "";
                            searchLookUpEditCL.EditValue = null;
                            return;
                        }
                        gridBandSize.Children.Clear();
                        createTable(_dtTable);
                        gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                        BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                        mainView.CellValueChanging += mainView_CellValueChanging;
                        //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                        mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                        //mainView.CustomDrawCell += MainView_CustomDrawCell;
                        gridControlThongTinDonHang.DataSource = _dtTable;
                        gridControl1.DataSource = _dtSize;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void createTable(DataTable tab)
        {
            //gridViewThongTinDonHang.OptionsView.AllowCellMerge = false;

            gridBandSize.Children.Clear();
            GridBand parentBand = gridViewThongTinDonHang.Bands["gridBandSize"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < gridViewThongTinDonHang.Columns.Count;)
                {
                    if (gridViewThongTinDonHang.Columns[i].FieldName.Contains("@Size@"))
                    {
                        gridViewThongTinDonHang.Columns.RemoveAt(i);
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
                if (demColIndex > 10 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[2];
                    String colName1 = arrName[2];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = true;
                    col.Visible = true;
                    col.Width = 65;
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    gridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 65;

                    //GridBand gb2 = new GridBand();
                    //gb2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    //gb2.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    //gb2.AppearanceHeader.ForeColor = System.Drawing.Color.Blue;
                    //gb2.AppearanceHeader.Options.UseForeColor = true;
                    //gb2.AppearanceHeader.Options.UseTextOptions = true;
                    //gb2.Caption = colName1;
                    //gb2.Name = "gbCodeSize_" + col.Name.Replace("colSize_", "");
                    //gb2.VisibleIndex = 0;
                    //gb2.Width = 65;
                    //gb2.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }
        static bool KiemTraChuoiLaChu(string chuoi)
        {
            if (string.IsNullOrEmpty(chuoi))
            {
                return false; // Chuỗi rỗng không được coi là chứa toàn ký tự chữ
            }

            foreach (char kyTu in chuoi)
            {
                if (!char.IsLetter(kyTu))
                {
                    return false; // Nếu có ít nhất một ký tự không phải là chữ, trả về false
                }
            }

            return true; // Nếu không có ký tự nào không phải là chữ, trả về true
        }

        private void AutoImportSizeNew(List<string> lstColumnName, bool isFromImport, string mahang)
        {
            List<BangSizeEntity> lstAutoImportSize = new List<BangSizeEntity>();
            BangSizeEntity autoImportSizeItem;

            for (int i = 0; i < lstColumnName.Count; i++)
            {
                string columnName = lstColumnName[i];

                // Khi import list column chứa tên size
                if (isFromImport)
                {
                    if (columnName.Contains("Size"))
                    {

                        string tenSize = columnName;
                        string sizeSX = string.Empty;
                        string[] nameSize = columnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameSize != null && nameSize.Length > 1)
                        {
                            sizeSX = nameSize[0];
                            tenSize = nameSize[0];
                            sizeSX = Regex.Replace(RemoveDiacritics(sizeSX), @"[^\w\d]+", "_");
                            autoImportSizeItem = new BangSizeEntity(sizeSX, tenSize, mahang);
                            lstAutoImportSize.Add(autoImportSizeItem);
                        }
                    }
                }
                else // Khi nhập list column chứa size sản xuất
                {
                    string sizeSX = columnName;
                    string tenSize = string.Format("Size {0}", columnName);
                    //string[] nameSize = columnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //sizeSX = nameSize[1];
                    autoImportSizeItem = new BangSizeEntity(sizeSX, tenSize, mahang);
                    lstAutoImportSize.Add(autoImportSizeItem);
                }

            }
            if (lstAutoImportSize != null && lstAutoImportSize.Count > 0)
            {
                string urlImportBangSize = string.Format("{0}", URL + "BangSize/PostAutoImportBangSize");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlImportBangSize, lstAutoImportSize); }).Result;
            }
        }

        private void AutoImportMauNew(List<string> lstSaveMau, string mahang)
        {
            List<BangMauEntity> lstAutoImportMau = new List<BangMauEntity>();
            BangSizeEntity autoImportSizeItem;

            if (lstSaveMau != null && lstSaveMau.Count > 0)
            {
                lstSaveMau = lstSaveMau.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveMau.Count; i++)
                {
                    BangMauEntity itemBangMau = new BangMauEntity(mahang, lstSaveMau[i], "", "");
                    lstAutoImportMau.Add(itemBangMau);
                }
                if (lstAutoImportMau.Count > 0)
                {
                    string urlAutoImportMau = string.Format("{0}?", URL + "BangMau/PostAutoImportBangMau");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportMau, lstAutoImportMau); }).Result;
                }
            }
        }

        private void AutoImportQGNew(List<string> lstSaveQG)
        {
            List<QuocGiaEntity> lstAutoImportQG = new List<QuocGiaEntity>();
            QuocGiaEntity autoImportSizeItem;

            if (lstSaveQG != null && lstSaveQG.Count > 0)
            {
                lstSaveQG = lstSaveQG.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveQG.Count; i++)
                {
                    QuocGiaEntity itemQG = new QuocGiaEntity(lstSaveQG[i]);
                    lstAutoImportQG.Add(itemQG);
                }
                if (lstAutoImportQG.Count > 0)
                {
                    string urlAutoImportQG = string.Format("{0}?", URL + "QuocGia/PostAutoQG");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportQG, lstAutoImportQG); }).Result;
                }
            }
        }

        private void btnRosyV2_Click(object sender, EventArgs e)
        {
            try
            {
                check = 1;
                btnXacNhan.BackColor = Color.Navy;
                if (searchLookUpEditMH.EditValue == null || (searchLookUpEditMH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Mã Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditKH.EditValue == null || (searchLookUpEditKH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Khách Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditCL.EditValue == null || (searchLookUpEditCL.EditValue != null && string.IsNullOrEmpty(searchLookUpEditCL.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Chủng Loại trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (txtDot == null || (txtDot != null && string.IsNullOrEmpty(txtDot.Text.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Đợt trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pathExecel = openFileDialog.FileName;
                    string conString = "", sheet1 = string.Empty;
                    int totalAmount = 0, Amount = 0;
                    using (OleDbConnection connection = new OleDbConnection(conString))
                    {
                        var source = new ExcelDataSource();
                        source.FileName = pathExecel;
                        string worksheetName = "";
                        using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                        {
                            DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                            worksheetName = worksheetCollection[0].Name;
                        }
                        var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A5:BZ500");
                        source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                        source.Fill();
                        DataTable tbl_ThongTinSize = new DataTable();
                        tbl_ThongTinSize = source.ToDataTable();
                        _dtTable = CreateDatatable();
                        _dtSize = CreateDatatableSize();
                        if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                        {

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString().ToString() == "***")
                                {
                                    break;
                                }
                                if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][0].ToString()))
                                {
                                    string po = tbl_ThongTinSize.Rows[j][0].ToString().Replace(" ", "").Trim();
                                    if (!string.IsNullOrEmpty(po) && !po.Trim().ToUpper().Contains("SOLUONG"))
                                    {
                                        if (po.Contains(" "))
                                        {
                                            po = po.Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(po))
                                        {
                                            XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][1].ToString()))
                                {
                                    string mau = tbl_ThongTinSize.Rows[j][1].ToString().Replace(" ", "").Trim();
                                    if (!string.IsNullOrEmpty(mau) && !mau.Trim().ToUpper().Contains("SOLUONG"))
                                    {
                                        if (mau.Contains(" "))
                                        {
                                            mau = mau.Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(mau))
                                        {
                                            XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }

                                }
                                else
                                {
                                    XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                            }

                            //foreach (DataRow row in tbl_ThongTinSize.Rows)
                            //{
                            //    foreach (DataColumn col in tbl_ThongTinSize.Columns)
                            //    {
                            //        if (row[col].ToString().ToString() == "***")
                            //        {
                            //            break;
                            //        }
                            //        if (RemoveVietnameseTone(col.ToString().Trim()).ToUpper().Replace(" ", "").Contains("SOLUONG"))
                            //        {
                            //            try
                            //            {
                            //                Amount = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(row[col].ToString(), "[^0-9a-zA-Z]+", ""));
                            //                totalAmount = totalAmount + Amount;
                            //            }
                            //            catch (Exception ex)
                            //            {
                            //                return;
                            //            }
                            //        }
                            //    }
                            //}
                            //for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            //{
                            //    for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                            //    {
                            //        string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                            //        string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //        string normalizedColName = ProcessString(RemoveDiacritics(ColName));

                            //        bool containsSize = false;

                            //        // Kiểm tra tên cột
                            //        if (normalizedColName.Contains("SIZE"))
                            //        {
                            //            containsSize = true;
                            //        }
                            //        else
                            //        {
                            //            // Kiểm tra các phần của tên cột
                            //            foreach (string gia_tri in mang_gia_tri)
                            //            {
                            //                if (ProcessString(RemoveDiacritics(gia_tri)).Contains("SIZE"))
                            //                {
                            //                    containsSize = true;
                            //                    break;
                            //                }
                            //            }
                            //        }

                            //        // Nếu không chứa "SIZE", bỏ qua và tiếp tục vòng lặp
                            //        if (!containsSize)
                            //        {
                            //            continue;
                            //        }

                            //        // Thêm các cột vào _dtTable nếu chứa "SIZE"
                            //        foreach (string gia_tri in mang_gia_tri)
                            //        {
                            //            if (ProcessString(RemoveDiacritics(gia_tri)) != "SIZE")
                            //            {
                            //                if (tbl_ThongTinSize.Rows[j][col].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") != "")
                            //                    Amount = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(tbl_ThongTinSize.Rows[j][col].ToString(), "[^0-9a-zA-Z]+", ""));
                            //                totalAmount = totalAmount + Amount;
                            //            }
                            //        }
                            //    }
                            //}

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                                {
                                    string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();

                                    // Kiểm tra điều kiện để dừng vòng lặp
                                    if (string.IsNullOrWhiteSpace(ColName) || // Kiểm tra cột trống
                                        ColName.Equals("PWC010", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("PWC015", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("PWC020", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("PWC030", StringComparison.OrdinalIgnoreCase) ||
                                        ProcessString(RemoveDiacritics(ColName)).Equals("SOLUONG", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("NGÀY DỰ KIẾN ĐB VẢI", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("NGÀY DỰ KIẾN ĐB PL", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("ĐỊA ĐIỂM GIAO HÀNG", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.Equals("NGÀY GIAO HÀNG", StringComparison.OrdinalIgnoreCase) ||
                                        ColName.IndexOf("COLUMN", StringComparison.OrdinalIgnoreCase) >= 0) // Kiểm tra nếu cột có chữ "COLUMN"
                                    {
                                        break; // Dừng vòng lặp
                                    }



                                    // Tính tổng giá trị cho các cột chứa "SIZE"
                                    string cellValue = tbl_ThongTinSize.Rows[j][col].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "");
                                    if (!string.IsNullOrEmpty(cellValue))
                                    {
                                        Amount = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(cellValue, "[^0-9]", ""));
                                        totalAmount += Amount; // Cộng vào tổng
                                    }
                                }
                            }

                            string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", searchLookUpEditMH.EditValue);
                            string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                            List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                            foreach (DonHangTongEntity donhang in dohanglist)
                            {
                                if (donhang.MaKH == searchLookUpEditKH.EditValue.ToString() && donhang.MaHang == searchLookUpEditMH.EditValue.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(txtDot.Text).Trim().ToUpper().ToString().Replace(" ", "")
                                   && donhang.SoLuong == totalAmount)
                                {
                                    DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng  này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (messResult == DialogResult.Yes)
                                    {
                                        return;
                                    }
                                    //XtraMessageBox.Show("Đã có đơn hàng cho mã hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //return;
                                }
                            }
                            string ngayBHStr = string.Empty;
                            for (int j = 1; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                                {
                                    if (RemoveVietnameseTone(tbl_ThongTinSize.Columns[col].ColumnName.ToString()).ToString().ToUpper().Trim().Replace(" ", "") == "NGAYGIAOHANG")
                                    {
                                        if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][col].ToString()))
                                        {
                                            DateTime date_ngayBH;
                                            if (DateTime.TryParseExact(tbl_ThongTinSize.Rows[j][col].ToString().Trim().Replace("-", "/"), new[] { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date_ngayBH))
                                            {
                                                ngayBHStr = date_ngayBH.ToString("dd/MM/yyyy"); // Chuyển đổi định dạng thành "dd/MM/yyyy"
                                            }
                                            else
                                            {
                                                MessageBox.Show("Kiểm tra lại Ngày Giao Hàng chưa đúng định dạng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                                return;
                                            }

                                        }

                                    }

                                }

                            }
                            //for (int col = 3; col < 23; col++)
                            //{
                            //    string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                            //    string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //    if (ProcessString(RemoveDiacritics(ColName.ToString())) == "SOLUONG")
                            //        break;
                            //    foreach (string gia_tri in mang_gia_tri)
                            //    {
                            //        if (ProcessString(RemoveDiacritics(gia_tri.ToString())) != "SIZE")
                            //            _dtTable.Columns.Add(gia_tri + "@Size@" + gia_tri, typeof(double));
                            //    }
                            //}
                            string urlKtSize = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetBangSizeKT", searchLookUpEditMH.EditValue.ToString());
                            string jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonKtSize);

                            //for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                            //{
                            //    string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                            //    string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //    string normalizedColName = ProcessString(RemoveDiacritics(ColName));

                            //    bool containsSize = false;

                            //    //// Kiểm tra tên cột
                            //    //if (normalizedColName.Contains("SIZE"))
                            //    //{
                            //    //    containsSize = true;
                            //    //}
                            //    //else
                            //    //{
                            //    //    // Kiểm tra các phần của tên cột
                            //    //    foreach (string gia_tri in mang_gia_tri)
                            //    //    {
                            //    //        if (ProcessString(RemoveDiacritics(gia_tri)).Contains("SIZE"))
                            //    //        {
                            //    //            containsSize = true;
                            //    //            break;
                            //    //        }
                            //    //    }
                            //    //}

                            //    //// Nếu không chứa "SIZE", bỏ qua và tiếp tục vòng lặp
                            //    //if (!containsSize)
                            //    //{
                            //    //    continue;
                            //    //}
                            //    int sizeIndex = ColName.IndexOf("Size");
                            //    string sizePart = "Size";
                            //    string gia_tris = ColName.Substring(sizeIndex + sizePart.Length).Trim();
                            //    DataRow _dr = _dtSize.NewRow();
                            //    _dr["MaSize"] = "";//"SIZE_" + ReplaceSpecialCharacters(gia_tris);
                            //    _dr["TenSize"] = ColName;
                            //    _dr["SizeSanXuat"] = ReplaceSpecialCharacters(gia_tris);
                            //    _dr["CodeSize"] = "";
                            //    _dtSize.Rows.Add(_dr);
                            //    //foreach (string gia_tri in mang_gia_tri)
                            //    //{
                            //    //    if (ProcessString(RemoveDiacritics(gia_tri)) != "SIZE")
                            //    //    {
                            //    //        _dtTable.Columns.Add(gia_tri + "@Size@" + gia_tri, typeof(double));
                            //    //    }
                            //    //}
                            //}

                            for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                            {
                                string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();

                                // Kiểm tra điều kiện để dừng vòng lặp
                                if (string.IsNullOrWhiteSpace(ColName) || // Kiểm tra cột trống
                                    ColName.Equals("PWC010", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("PWC015", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("PWC020", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("PWC030", StringComparison.OrdinalIgnoreCase) ||
                                    ProcessString(RemoveDiacritics(ColName)).Equals("SOLUONG", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("NGÀY DỰ KIẾN ĐB VẢI", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("NGÀY DỰ KIẾN ĐB PL", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("ĐỊA ĐIỂM GIAO HÀNG", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.Equals("NGÀY GIAO HÀNG", StringComparison.OrdinalIgnoreCase) ||
                                    ColName.StartsWith("COLUMN", StringComparison.OrdinalIgnoreCase)) // Kiểm tra nếu cột bắt đầu bằng "COLUMN"
                                {
                                    break; // Dừng vòng lặp
                                }

                                string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                int sizeIndex = ColName.IndexOf("Size");
                                if (sizeIndex < 0)
                                {
                                    string gia_tri = ColName.ToString().Trim();
                                    DataRow _dr = _dtSize.NewRow();
                                    _dr["MaSize"] = ""; // "SIZE_" + ReplaceSpecialCharacters(gia_tri);
                                    _dr["TenSize"] = ColName;
                                    _dr["SizeSanXuat"] = ReplaceSpecialCharacterssize(gia_tri);
                                    _dr["CodeSize"] = "";
                                    _dtSize.Rows.Add(_dr);
                                }
                                else
                                {
                                    string sizePart = "Size";
                                    string gia_tri = ColName.Substring(sizeIndex + sizePart.Length).Trim();
                                    DataRow _dr = _dtSize.NewRow();
                                    _dr["MaSize"] = ""; // "SIZE_" + ReplaceSpecialCharacters(gia_tri);
                                    _dr["TenSize"] = ColName;
                                    _dr["SizeSanXuat"] = ReplaceSpecialCharacterssize(gia_tri);
                                    _dr["CodeSize"] = "";
                                    _dtSize.Rows.Add(_dr);
                                }
                            }
                            if (tblSize != null)
                            {
                                foreach (DataRow _drSize in _dtSize.Rows)
                                {
                                    foreach (DataRow _drsize in tblSize.Rows)
                                    {
                                        if (_drsize["XacNhan"].ToString() == "1")
                                        {
                                            if (_drsize["SizeXacNhan"].ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString())
                                            {
                                                //_drSize["MaSize"] = _drsize["MaSize"];
                                                //_drSize["TenSize"] = _drsize["TenSize"];
                                                //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                                                //_drSize["CodeSize"] = _drsize["CodeSize"];
                                                //_drSize["GhiChu"] = _drsize["GhiChu"];
                                                _drSize["XacNhan"] = _drsize["XacNhan"];
                                                _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                                            }
                                            else
                                            {
                                                if (_drsize["SizeSanXuat"].ToString().ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString())
                                                {
                                                    //    _drSize["MaSize"] = _drsize["MaSize"];
                                                    //    _drSize["TenSize"] = _drsize["TenSize"];
                                                    //    _drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                                                    //    _drSize["CodeSize"] = _drsize["CodeSize"];
                                                    //    _drSize["GhiChu"] = _drsize["GhiChu"];
                                                    _drSize["XacNhan"] = _drsize["XacNhan"];
                                                    _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (_drsize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") == _drSize["SizeSanXuat"].ToString().Replace(" ", "").Replace("_", "") && _drsize["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString())
                                            {
                                                //_drSize["MaSize"] = _drsize["MaSize"];
                                                //_drSize["TenSize"] = _drsize["TenSize"];
                                                //_drSize["SizeSanXuat"] = _drsize["SizeSanXuat"];
                                                //_drSize["CodeSize"] = _drsize["CodeSize"];
                                                //_drSize["GhiChu"] = _drsize["GhiChu"];
                                                _drSize["XacNhan"] = _drsize["XacNhan"];
                                                _drSize["SizeXacNhan"] = _drsize["SizeXacNhan"];
                                            }
                                        }

                                    }
                                }
                            }

                            foreach (DataRow _drSize in _dtSize.Rows)
                            {
                                _dtTable.Columns.Add(_drSize["SizeSanXuat"].ToString().Trim().ToUpper() + "@Size@" + _drSize["TenSize"].ToString().Trim().ToUpper(), typeof(string));
                            }
                            string colorID = string.Empty, colorName = string.Empty, idPO = string.Empty, style = string.Empty, TenHang = string.Empty, colorStr = string.Empty, poID = string.Empty, countryName = string.Empty,
                            C_Size = string.Empty, C_DauSize = string.Empty, _ktLoi = string.Empty, ngayStr = string.Empty, SeaSon = string.Empty;
                            string newHeader = string.Empty, pp = string.Empty;
                            string dausize = string.Empty;
                            string _color = string.Empty, _po = string.Empty, _maQG = string.Empty;
                            string _ngayGH = string.Empty;
                            string error = string.Empty;

                            // Check và import Size
                            //List<string> lstImportSize = new List<string>();
                            //string urlsize = string.Format("{0}?", URL + "BangSize/GetBangSize");
                            //string jsonsize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsize); }).Result;
                            //List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonsize);
                            string _maHang = searchLookUpEditMH.EditValue.ToString().Trim();

                            //if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                            //{
                            //    for (int indexColumn = 3; indexColumn < tbl_ThongTinSize.Columns.Count; indexColumn++)
                            //    {
                            //        DataColumn column = tbl_ThongTinSize.Columns[indexColumn];
                            //        if (column.ColumnName.ToUpper().Contains("SIZE"))
                            //        {
                            //            List<string> lstNameSize = column.ColumnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //            if (lstNameSize != null && lstNameSize.Count > 1)
                            //            {
                            //                string sizeSX = lstNameSize[1];
                            //                BangSizeEntity bangSize = lstBangSize.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper().Trim()
                            //                == sizeSX.ToString().Trim() + _maHang).FirstOrDefault();
                            //                if (bangSize == null && column != null && !string.IsNullOrEmpty(column.ColumnName))
                            //                {
                            //                    lstImportSize.Add(column.ColumnName);
                            //                }
                            //            }
                            //            //Console.WriteLine("Check_Size");
                            //        }
                            //    }
                            //    AutoImportSize(lstImportSize, true);
                            //}
                            // Size

                            // Check và import màu
                            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);

                            List<string> lstSaveMau = new List<string>();

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString() == "***")
                                {
                                    break;
                                }
                                //
                                BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == tbl_ThongTinSize.Rows[j][1].ToString().Trim() + searchLookUpEditMH.EditValue.ToString().Trim()).FirstOrDefault();
                                if (bangMau == null)
                                {
                                    string hasDuplicates = lstSaveMau.Where(x => x.Equals(tbl_ThongTinSize.Rows[j][1].ToString())).FirstOrDefault();
                                    if (string.IsNullOrEmpty(hasDuplicates))
                                    {
                                        lstSaveMau.Add(tbl_ThongTinSize.Rows[j][1].ToString());
                                    }

                                }
                            }

                            if (lstSaveMau.Count > 0)
                            {
                                AutoImportMau(lstSaveMau);
                            }

                            // Check và import QG
                            string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                            string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                            List<QuocGiaEntity> lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQG);

                            List<string> lstSaveQG = new List<string>();

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString() == "***")
                                {
                                    break;
                                }
                                for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                                {

                                    if (RemoveVietnameseTone(tbl_ThongTinSize.Columns[col].ColumnName.ToString()).ToString().ToUpper().Trim().Replace(" ", "") == "DIADIEMGIAOHANG")
                                    {
                                        QuocGiaEntity quocGia = lstQG.Where(item => item.TenQG.ToUpper().Trim().ToUpper() == tbl_ThongTinSize.Rows[j][col].ToString().Trim().ToUpper()).FirstOrDefault();
                                        if (quocGia == null)
                                        {
                                            string hasDuplicates = lstSaveQG.Where(x => x.Equals(tbl_ThongTinSize.Rows[j][col].ToString())).FirstOrDefault();
                                            if (string.IsNullOrEmpty(hasDuplicates))
                                            {
                                                lstSaveQG.Add(tbl_ThongTinSize.Rows[j][col].ToString());
                                            }

                                        }

                                    }


                                }

                            }

                            if (lstSaveQG.Count > 0)
                            {
                                AutoImportQG(lstSaveQG);
                            }

                            DataTable dataTablChanged = ChangeTenMauToMaMau(tbl_ThongTinSize, _maHang);
                            if (dataTablChanged != null && dataTablChanged.Rows.Count > 0)
                            {
                                _dtTable = MapDataV2(_dtTable, dataTablChanged);
                            }
                            _dtTable = ChangeTenQGToMaQGNew(_dtTable);
                            bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                            {
                                DauSize = row["DauSize"],
                                POID = row["POID"],
                                MaMau = row["MaMau"]
                            }).Any(g => g.Count() > 1);
                            if (checkDuplicateData)
                            {
                                XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _dtTable.Clear();
                                return;
                            }
                            gridBandSize.Children.Clear();
                            createTable(_dtTable);
                            CreateSearchLookupQuocGia();
                            CreateSearchLookupMaMau();
                            CreateSearchLookupDauSize();
                            gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                            BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                            mainView.CellValueChanging += mainView_CellValueChanging;
                            //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                            mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                            //mainView.CustomDrawCell += MainView_CustomDrawCell;
                            gridControlThongTinDonHang.DataSource = _dtTable;
                            gridControl1.DataSource = _dtSize;
                            //gridControlThongTinDonHang.DataSource = dataTablChanged;
                            CreateSearchLookupQG();
                            CheckBtnLuu();

                        }
                        else
                        {
                            XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                            //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                            //else
                            //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                //else
                //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                return;
            }
        }
        private DataTable MapDataNew(DataTable _dtTable, DataTable tblChanged)
        {
            // Tạo một DataTable tạm từ tblChanged. Chỉ giữ lại các cột số lượng size, xóa các cột còn lại
            // Dùng để map số lượng theo từng size cho bảng _dtTable
            // Sau khi xóa số lượng cột của _tblTempSLSize sẽ bằng với số lượng cột size của _dtTable
            DataTable _tblTempSLSize = tblChanged.Copy();
            for (int i = 0; i < _tblTempSLSize.Columns.Count;)
            {
                DataColumn column = _tblTempSLSize.Columns[i];
                List<string> _lstTemp = column.ColumnName.Split(new char[] { ' ' }).ToList();
                if (!_lstTemp[0].ToUpper().Equals("SIZE"))
                {
                    _tblTempSLSize.Columns.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // POID, PO, MaMau, DauSize, MaQG, NgayGH, GhiChu
            foreach (DataRow row in tblChanged.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                DataRow _rowAdd = _dtTable.NewRow();
                _rowAdd["POID"] = row[0].ToString();
                _rowAdd["PO"] = row[1].ToString();
                _rowAdd["MaMau"] = row[2];
                _rowAdd["DauSize"] = row[3].ToString();
                _rowAdd["MaQG"] = row[4].ToString();
                _rowAdd["NgayGH"] = row[5].ToString();
                //_rowAdd["GhiChu"] = txtGhiChu.Text;
                _dtTable.Rows.Add(_rowAdd);

            }

            // Map danh sách số lượng của từng size
            int index = 0;
            foreach (DataColumn column in _dtTable.Columns)
            {
                if (column.ColumnName.Contains("@Size@"))
                {
                    for (int i = 0; i < _dtTable.Rows.Count; i++)
                    {
                        _dtTable.Rows[i][column] = _tblTempSLSize.Rows[i][index];
                    }
                    index += 1;
                }
            }

            return _dtTable;
        }

        private void gridControl1_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            GridControl grid = sender as GridControl;
            GridView view = grid.FocusedView as GridView;

            if (view.FocusedColumn == gridColumn3)
            {
                // Kiểm tra phím xóa
                if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
                {
                    // Cho phép xóa
                    e.Handled = false;
                }
                else if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '(' && e.KeyChar != ')')
                {
                    e.Handled = true; // Ngăn chặn việc nhập ký tự không hợp lệ
                }
                else
                {
                    // Chuyển đổi ký tự thành chữ hoa
                    e.KeyChar = char.ToUpper(e.KeyChar);
                }
            }
        }
        public DataTable ChangeTenMauToMaMauNew(DataTable tbl, string _maHang)
        {
            try
            {
                DataTable tblChange = tbl.Copy();
                // lấy danh sách bảng màu
                string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                List<BangMauEntity> _lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
                string columnNameMauPO = "MAMAU";

                // Cập nhật field Màu PO
                for (int i = 0; i < tblChange.Rows.Count; i++)
                {
                    foreach (DataColumn dataColumn in tblChange.Columns)
                    {
                        if (tblChange.Rows[i][dataColumn].ToString() == "***")
                        {
                            break;
                        }
                        if (RemoveDiacritics(dataColumn.ColumnName).Replace(" ", "").ToUpper().Equals(columnNameMauPO))
                        {
                            //Console.WriteLine("ChangedTenMauToMaMau");
                            if (tblChange.Rows[i][dataColumn] != null)
                            {
                                string tenMau = tblChange.Rows[i][dataColumn].ToString();
                                // Lấy ra mã màu từ _lstBangMau bằng field TenMau và MaHang
                                BangMauEntity bangMauEntity = _lstBangMau.Where(x => x.TenMau.ToString().Trim().ToUpper().Replace(" ", "") == tenMau.ToString().Trim().ToUpper().Replace(" ", "") && x.MaHang == _maHang).FirstOrDefault();

                                if (bangMauEntity != null)
                                {
                                    // Đổi DataTable
                                    tblChange.Rows[i][dataColumn] = bangMauEntity.MaMau;
                                }
                                else
                                {
                                    Console.WriteLine("Import màu không thành công => Nên không lấy được mã màu sau khi import");
                                }
                            }
                        }
                    }
                }

                //Console.WriteLine("Change PO complete");
                return tblChange;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }

        }
        public DataTable ChangeTenQGToMaQGNew(DataTable tbl)
        {
            try
            {
                DataTable tblChange = tbl.Copy();
                // lấy danh sách bảng QG
                string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                List<QuocGiaEntity> _lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(json);
                string columnNameQG = "MAQG";

                // Cập nhật field Màu PO
                for (int i = 0; i < tblChange.Rows.Count; i++)
                {
                    foreach (DataColumn dataColumn in tblChange.Columns)
                    {
                        if (RemoveDiacritics(dataColumn.ColumnName).Replace(" ", "").ToUpper().Equals(columnNameQG))
                        {
                            //Console.WriteLine("ChangedTenMauToMaMau");
                            if (tblChange.Rows[i][dataColumn] != null)
                            {
                                string tenQG = tblChange.Rows[i][dataColumn].ToString();
                                // Lấy ra mã màu từ _lstBangMau bằng field TenMau và MaHang
                                QuocGiaEntity QGEntity = _lstQG.Where(x => x.TenQG.ToString().ToUpper().Trim() == tenQG.ToString().ToUpper().Trim()).FirstOrDefault();

                                if (QGEntity != null)
                                {
                                    // Đổi DataTable
                                    tblChange.Rows[i][dataColumn] = QGEntity.MaQG;
                                }
                                else
                                {
                                    Console.WriteLine("Import QG không thành công => Nên không lấy được mã QG sau khi import");
                                }
                            }
                        }
                    }
                }

                //Console.WriteLine("Change PO complete");
                return tblChange;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }

        }

        private void SearchLookUpEditQG_EditValueChanged(object sender, EventArgs e)
        {
            if (_dtTable == null || _dtTable.Rows.Count == 0)
                return;
            //foreach (DataRow row in _dtTable.Rows)
            //{
            //    row["MaQG"] = SearchLookUpEditQG.EditValue;
            //}

        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    ((DataTable)gridControlThongTinDonHang.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
            }
        }

        private void btnImportV2_Click(object sender, EventArgs e)
        {
            try
            {
                check = 1;
                if (searchLookUpEditMH.EditValue == null || (searchLookUpEditMH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Mã Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditKH.EditValue == null || (searchLookUpEditKH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Khách Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditCL.EditValue == null || (searchLookUpEditCL.EditValue != null && string.IsNullOrEmpty(searchLookUpEditCL.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Chủng Loại trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (txtDot == null || (txtDot != null && string.IsNullOrEmpty(txtDot.Text.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Đợt trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _dtTable = CreateDatatable();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pathExecel = openFileDialog.FileName;
                    string conString = "", sheet1 = string.Empty;
                    int totalAmount = 0, Amount = 0;
                    using (OleDbConnection connection = new OleDbConnection(conString))
                    {
                        var source = new ExcelDataSource();
                        source.FileName = pathExecel;
                        string worksheetName = "";
                        using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                        {
                            DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                            worksheetName = worksheetCollection[0].Name;
                        }
                        var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A5:BZ500");
                        source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                        source.Fill();
                        DataTable tbl_ThongTinSize = new DataTable();
                        tbl_ThongTinSize = source.ToDataTable();

                        if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                        {

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count - 1; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString().ToString() == "***")
                                {
                                    break;
                                }
                                if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][0].ToString()))
                                {
                                    string po = tbl_ThongTinSize.Rows[j][0].ToString().Replace(" ", "").Trim();
                                    if (!string.IsNullOrEmpty(po) && !po.Trim().ToUpper().Contains("SOLUONG"))
                                    {
                                        if (po.Contains(" "))
                                        {
                                            po = po.Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(po))
                                        {
                                            XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][1].ToString()))
                                {
                                    string mau = tbl_ThongTinSize.Rows[j][1].ToString().Replace(" ", "").Trim();
                                    if (!string.IsNullOrEmpty(mau) && !mau.Trim().ToUpper().Contains("SOLUONG"))
                                    {
                                        if (mau.Contains(" "))
                                        {
                                            mau = mau.Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(mau))
                                        {
                                            XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }

                                }
                                else
                                {
                                    XtraMessageBox.Show("Tên màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                            }

                            //foreach (DataRow row in tbl_ThongTinSize.Rows)
                            //{
                            //    foreach (DataColumn col in tbl_ThongTinSize.Columns)
                            //    {
                            //        if (row[col].ToString().ToString() == "***")
                            //        {
                            //            break;
                            //        }
                            //        if (RemoveVietnameseTone(col.ToString().Trim()).ToUpper().Replace(" ", "").Contains("SOLUONG"))
                            //        {
                            //            try
                            //            {
                            //                Amount = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(row[col].ToString(), "[^0-9a-zA-Z]+", ""));
                            //                totalAmount = totalAmount + Amount;
                            //            }
                            //            catch (Exception ex)
                            //            {
                            //                return;
                            //            }
                            //        }
                            //    }
                            //}
                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                                {
                                    string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                                    string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    string normalizedColName = ProcessString(RemoveDiacritics(ColName));

                                    bool containsSize = false;

                                    // Kiểm tra tên cột
                                    if (normalizedColName.Contains("SIZE"))
                                    {
                                        containsSize = true;
                                    }
                                    else
                                    {
                                        // Kiểm tra các phần của tên cột
                                        foreach (string gia_tri in mang_gia_tri)
                                        {
                                            if (ProcessString(RemoveDiacritics(gia_tri)).Contains("SIZE"))
                                            {
                                                containsSize = true;
                                                break;
                                            }
                                        }
                                    }

                                    // Nếu không chứa "SIZE", bỏ qua và tiếp tục vòng lặp
                                    if (!containsSize)
                                    {
                                        continue;
                                    }

                                    // Thêm các cột vào _dtTable nếu chứa "SIZE"
                                    foreach (string gia_tri in mang_gia_tri)
                                    {
                                        if (ProcessString(RemoveDiacritics(gia_tri)) != "SIZE")
                                        {
                                            if (tbl_ThongTinSize.Rows[j][col].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") != "")
                                                Amount = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(tbl_ThongTinSize.Rows[j][col].ToString(), "[^0-9a-zA-Z]+", ""));
                                            totalAmount = totalAmount + Amount;
                                        }
                                    }
                                }
                            }
                            string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", searchLookUpEditMH.EditValue);
                            string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                            List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                            foreach (DonHangTongEntity donhang in dohanglist)
                            {
                                if (donhang.MaKH == searchLookUpEditKH.EditValue.ToString() && donhang.MaHang == searchLookUpEditMH.EditValue.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(txtDot.Text).Trim().ToUpper().ToString().Replace(" ", "")
                                   && donhang.SoLuong == totalAmount)
                                {
                                    DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng  này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (messResult == DialogResult.Yes)
                                    {
                                        return;
                                    }
                                    //XtraMessageBox.Show("Đã có đơn hàng cho mã hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //return;
                                }
                            }
                            string ngayBHStr = string.Empty;
                            for (int j = 1; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                                {
                                    if (RemoveVietnameseTone(tbl_ThongTinSize.Columns[col].ColumnName.ToString()).ToString().ToUpper().Trim().Replace(" ", "") == "NGAYGIAOHANG")
                                    {
                                        if (!string.IsNullOrEmpty(tbl_ThongTinSize.Rows[j][col].ToString()))
                                        {
                                            DateTime date_ngayBH;
                                            if (DateTime.TryParseExact(tbl_ThongTinSize.Rows[j][col].ToString().Trim().Replace("-", "/"), new[] { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date_ngayBH))
                                            {
                                                ngayBHStr = date_ngayBH.ToString("MM/dd/yyyy"); // Chuyển đổi định dạng thành "dd/MM/yyyy"
                                            }
                                            else
                                            {
                                                MessageBox.Show("Kiểm tra lại Ngày Giao Hàng chưa đúng định dạng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                                return;
                                            }

                                        }

                                    }

                                }

                            }
                            //for (int col = 3; col < 23; col++)
                            //{
                            //    string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                            //    string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //    if (ProcessString(RemoveDiacritics(ColName.ToString())) == "SOLUONG")
                            //        break;
                            //    foreach (string gia_tri in mang_gia_tri)
                            //    {
                            //        if (ProcessString(RemoveDiacritics(gia_tri.ToString())) != "SIZE")
                            //            _dtTable.Columns.Add(gia_tri + "@Size@" + gia_tri, typeof(double));
                            //    }
                            //}

                            for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                            {
                                string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                                string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                string normalizedColName = ProcessString(RemoveDiacritics(ColName));

                                bool containsSize = false;

                                // Kiểm tra tên cột
                                if (normalizedColName.Contains("SIZE"))
                                {
                                    containsSize = true;
                                }
                                else
                                {
                                    // Kiểm tra các phần của tên cột
                                    foreach (string gia_tri in mang_gia_tri)
                                    {
                                        if (ProcessString(RemoveDiacritics(gia_tri)).Contains("SIZE"))
                                        {
                                            containsSize = true;
                                            break;
                                        }
                                    }
                                }

                                // Nếu không chứa "SIZE", bỏ qua và tiếp tục vòng lặp
                                if (!containsSize)
                                {
                                    continue;
                                }
                                int sizeIndex = ColName.IndexOf("Size");
                                string sizePart = "Size";
                                string gia_tris = ColName.Substring(sizeIndex + sizePart.Length).Trim();

                                _dtTable.Columns.Add(gia_tris + "@Size@" + gia_tris, typeof(double));
                                //foreach (string gia_tri in mang_gia_tri)
                                //{
                                //    if (ProcessString(RemoveDiacritics(gia_tri)) != "SIZE")
                                //    {
                                //        _dtTable.Columns.Add(gia_tri + "@Size@" + gia_tri, typeof(double));
                                //    }
                                //}
                            }
                            string colorID = string.Empty, colorName = string.Empty, idPO = string.Empty, style = string.Empty, TenHang = string.Empty, colorStr = string.Empty, poID = string.Empty, countryName = string.Empty,
                            C_Size = string.Empty, C_DauSize = string.Empty, _ktLoi = string.Empty, ngayStr = string.Empty, SeaSon = string.Empty;
                            string newHeader = string.Empty, pp = string.Empty;
                            string dausize = string.Empty;
                            string _color = string.Empty, _po = string.Empty, _maQG = string.Empty;
                            string _ngayGH = string.Empty;
                            string error = string.Empty;

                            // Check và import Size
                            List<string> lstImportSize = new List<string>();
                            string urlsize = string.Format("{0}?", URL + "BangSize/GetBangSize");
                            string jsonsize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsize); }).Result;
                            List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonsize);
                            string _maHang = searchLookUpEditMH.EditValue.ToString().Trim();

                            if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                            {
                                for (int indexColumn = 3; indexColumn < tbl_ThongTinSize.Columns.Count; indexColumn++)
                                {
                                    DataColumn column = tbl_ThongTinSize.Columns[indexColumn];
                                    if (column.ColumnName.ToUpper().Contains("SIZE"))
                                    {
                                        List<string> lstNameSize = column.ColumnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        if (lstNameSize != null && lstNameSize.Count > 1)
                                        {
                                            string sizeSX = lstNameSize[1];
                                            BangSizeEntity bangSize = lstBangSize.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper().Trim()
                                            == sizeSX.ToString().Trim() + _maHang).FirstOrDefault();
                                            if (bangSize == null && column != null && !string.IsNullOrEmpty(column.ColumnName))
                                            {
                                                lstImportSize.Add(column.ColumnName);
                                            }
                                        }
                                        //Console.WriteLine("Check_Size");
                                    }
                                }
                                AutoImportSize(lstImportSize, true);
                            }
                            // Size

                            // Check và import màu
                            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);

                            List<string> lstSaveMau = new List<string>();

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString() == "***")
                                {
                                    break;
                                }
                                //
                                BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == tbl_ThongTinSize.Rows[j][1].ToString().Trim() + searchLookUpEditMH.EditValue.ToString().Trim()).FirstOrDefault();
                                if (bangMau == null)
                                {
                                    string hasDuplicates = lstSaveMau.Where(x => x.Equals(tbl_ThongTinSize.Rows[j][1].ToString())).FirstOrDefault();
                                    if (string.IsNullOrEmpty(hasDuplicates))
                                    {
                                        lstSaveMau.Add(tbl_ThongTinSize.Rows[j][1].ToString());
                                    }

                                }
                            }

                            if (lstSaveMau.Count > 0)
                            {
                                AutoImportMau(lstSaveMau);
                            }

                            // Check và import QG
                            string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                            string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                            List<QuocGiaEntity> lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQG);

                            List<string> lstSaveQG = new List<string>();

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                            {
                                if (tbl_ThongTinSize.Rows[j][0].ToString() == "***")
                                {
                                    break;
                                }
                                //
                                QuocGiaEntity quocGia = lstQG.Where(item => item.TenQG.ToUpper().Trim().ToUpper() == tbl_ThongTinSize.Rows[j][31].ToString().Trim().ToUpper()).FirstOrDefault();
                                if (quocGia == null)
                                {
                                    string hasDuplicates = lstSaveQG.Where(x => x.Equals(tbl_ThongTinSize.Rows[j][31].ToString())).FirstOrDefault();
                                    if (string.IsNullOrEmpty(hasDuplicates))
                                    {
                                        lstSaveQG.Add(tbl_ThongTinSize.Rows[j][31].ToString());
                                    }

                                }
                            }

                            if (lstSaveQG.Count > 0)
                            {
                                AutoImportQG(lstSaveQG);
                            }

                            DataTable dataTablChanged = ChangeTenMauToMaMau(tbl_ThongTinSize, _maHang);
                            if (dataTablChanged != null && dataTablChanged.Rows.Count > 0)
                            {
                                _dtTable = MapDataV2(_dtTable, dataTablChanged);
                            }
                            _dtTable = ChangeTenQGToMaQGNew(_dtTable);
                            bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                            {
                                DauSize = row["DauSize"],
                                POID = row["POID"],
                                MaMau = row["MaMau"]
                            }).Any(g => g.Count() > 1);
                            if (checkDuplicateData)
                            {
                                XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _dtTable.Clear();
                                return;
                            }
                            gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                            BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                            mainView.CellValueChanging += mainView_CellValueChanging;
                            mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                            mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                            mainView.CustomDrawCell += MainView_CustomDrawCell;
                            gridControlThongTinDonHang.DataSource = _dtTable;
                            //gridControlThongTinDonHang.DataSource = dataTablChanged;
                            CreateSearchLookupQG();
                            CheckBtnLuu();

                        }
                        else
                        {
                            XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                            //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                            //else
                            //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                //else
                //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                return;
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (_dtSize == null || _dtSize.Rows.Count <= 0) return;
            //string url = string.Format("{0}?mahang={1}", URL + "BangSize/GetBangSizeMH", searchLookUpEditMH.EditValue.ToString());
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(json);
            foreach (DataRow _dr in _dtSize.Rows)
            {
                //    int sizeIndex = _dr["TenSize"].ToString().IndexOf("Size");
                //    if(sizeIndex >= 0)
                //    {
                //        string sizePart = "Size";
                //        string _gia_tri = ReplaceSpecialCharacters(_dr["TenSize"].ToString().Substring(sizeIndex + sizePart.Length).Trim());
                //        if (_gia_tri.ToString().Trim() != _dr["SizeSanXuat"].ToString().Trim())
                //        {
                //            _dr["XacNhan"] = 1;
                //            _dr["SizeXacNhan"] = ReplaceSpecialCharacters(_gia_tri);
                //        }
                //        else
                //        {
                //            _dr["XacNhan"] = 0;
                //        }
                //        _dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                //    }    
                //    else
                //    {
                string _gia_tri_SizeSX = ReplaceSpecialCharacterssize(_dr["SizeSanXuat"].ToString().Trim());
                //foreach (BangSizeEntity item in lstBangSize)
                //{
                //    if (item.SizeSanXuat != ReplaceSpecialCharacterssize(_dr["SizeSanXuat"].ToString().Trim()))
                //        _gia_tri_SizeSX = ReplaceSpecialCharacterssize(_dr["SizeSanXuat"].ToString().Trim());
                //    else
                //        _gia_tri_SizeSX = ReplaceSpecialCharacterssize(_dr["TenSize"].ToString().Trim());
                //}
                string _gia_tri = ReplaceSpecialCharacterssize(_dr["TenSize"].ToString().Trim());

                //        if (_gia_tri.ToString().Trim() != _dr["SizeSanXuat"].ToString().Trim())
                //        {
                _dr["XacNhan"] = 1;
                _dr["SizeXacNhan"] = ReplaceSpecialCharacterssize(_gia_tri);
                _dr["MaSize"] = _gia_tri_SizeSX;
                //}
                //else
                //{
                //    _dr["XacNhan"] = 0;
                //}
                _dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();

            }


            string urlImportBangSize = string.Format("{0}", URL + "DonHangTong/PostAutoImportBangSize");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlImportBangSize, _dtSize); }).Result;
            if (result.ToLower() != "true")
            {
                XtraMessageBox.Show("Lỗi xác nhận Size. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                int startIndex = 7;
                int endIndex = _dtTable.Columns.Count - 1;
                List<string> listHeader = new List<string>();
                int j = 0;
                while (j < _dtSize.Rows.Count)
                {
                    string arrName = _dtSize.Rows[j][4].ToString();
                    listHeader.Add(_dtSize.Rows[j][4].ToString() + "@Size@" + _dtSize.Rows[j][3].ToString());
                    j++;

                }
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (i - startIndex < listHeader.Count)
                    {
                        _dtTable.Columns[i].ColumnName = listHeader[i - startIndex];
                    }
                }
                createTable(_dtTable);
                gridViewThongTinDonHang.RefreshData();
                btnXacNhan.BackColor = Color.Green;
                _xacnhan = true;
                clsWaitForm.ShowSuccessForm(this, 2000);
            }


        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable gridcontrol = gridControlThongTinDonHang.DataSource as DataTable;
            //gridcontrol.Columns.Clear();
            gridcontrol.Rows.Clear();
            gridBandSize.Children.Clear();
            gridBandSize.Columns.Clear();
            //gridControlThongTinDonHang.RefreshDataSource();
            CreateSearchLookup();
            CreateSearchLookupQG();
            CreateSearchLookupQuocGia();
            //CreateSearchLookupMaMau();
            //CreateSearchLookupDauSize();
        }

        private void gridViewThongTinDonHang_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName.Contains("@Size@") && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (row.Table.Columns.Contains(col.FieldName) && row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                int value;
                                bool isValidNumber = Int32.TryParse(row[col.FieldName].ToString(), out value);

                                if (!isValidNumber)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    Convert.ToInt32(row[col.FieldName]);
                                }
                                //int value = Convert.ToInt32(row[col.FieldName]);
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

        private void btnKhachHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmKhachHang frm = new frmKhachHang();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookup(); // Gọi hàm khi frmKhachHang đóng
            };
            frm.ShowDialog();
        }

        private void btnMaHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmHangHoa frm = new frmHangHoa();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookup();
                InitSearchLookupSize();
                InitSearInitSearchLookupMaMau();
                CreateSearchLookupDauSize();
                string urlHH = string.Format("{0}?makh={1}", URL + "DonHangTong/GetHangHoaKH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
                DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
                searchLookUpEditMH.Properties.DataSource = tblHH;
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";
            };
            frm.ShowDialog();
        }

        private void btnBangMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBangMau frm = new frmBangMau();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookup(); // Gọi hàm khi frmKhachHang đóng
                InitSearInitSearchLookupMaMau();
                CreateSearchLookupMaMau();
            };
            frm.ShowDialog();
        }

        private void btnBangSize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBangSize frm = new frmBangSize();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookup(); // Gọi hàm khi frmKhachHang đóng
                InitSearchLookupDauSize();
            };
            frm.ShowDialog();
        }

        private void searchLookUpEditMH_Properties_EditValueChanged(object sender, EventArgs e)
        {
            InitSearInitSearchLookupMaMau();
            InitSearchLookupDauSize();
        }

        private void txtMau_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["TenMau"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Màu----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void btnChungLoai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmChungLoai frm = new frmChungLoai();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {
                CreateSearchLookup(); // Gọi hàm khi frmKhachHang đóng
            };
            frm.ShowDialog();
        }

        private void txtNhapSize_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["SizeSanXuat"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Size----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void btnImportNTB_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                check = 1;
                string filePath = string.Empty;
                string sheetIndex = string.Empty;
                bool resultFrm = false;
                // btnXacNhan.BackColor = Color.Navy;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";

                frmErpImport_DHT frm = new frmErpImport_DHT();
                frm.ShowDialog();
                filePath = frm.resultFilePath;
                sheetIndex = frm.resultIndex;
                resultFrm = frm.result;
                if (resultFrm == false) return;
                string pathExecel = filePath;
                string conString = "", sheet1 = string.Empty;
                int totalAmount = 0, Amount = 0;
                using (OleDbConnection connection = new OleDbConnection(conString))
                {
                    var source = new ExcelDataSource();
                    source.FileName = pathExecel;
                    string worksheetName = "";
                    using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                    {
                        DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                        worksheetName = worksheetCollection[sheetIndex].Name;
                    }
                    var worksheetSettingss = new ExcelWorksheetSettings(worksheetName, "$A1:D9");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettingss);
                    source.Fill();
                    DataTable tbl_Styles = new DataTable();
                    tbl_Styles = source.ToDataTable();

                    string pp = string.Empty;
                    string style = tbl_Styles.Rows[1][2].ToString().TrimStart().TrimEnd();
                    var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A11:BZ500");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                    source.Fill();
                    DataTable tbColorPO = new DataTable();
                    tbColorPO = source.ToDataTable();
                    _dtTable = CreateDatatable();
                    _dtSizeNhom = CreateDatatableSizeNhom();
                    string styleID = "";
                    string cusName = "";
                    string chungloai = "";

                    Dictionary<string, string> mauChuaCo = new Dictionary<string, string>();
                    if (style.Length > 100)
                    {
                        XtraMessageBox.Show("Mã hàng không được dài quá 100 kí tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string _styleID = style.ToString().TrimStart().TrimEnd();
                    if (string.IsNullOrEmpty(_styleID))
                    {
                        XtraMessageBox.Show("Mã hàng không được bỏ trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlmhang = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
                    string jsonmhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmhang); }).Result;
                    DataTable _dtMaHang = JsonConvert.DeserializeObject<DataTable>(jsonmhang);
                    if (_dtMaHang == null)
                    {
                        XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var rowmh = _dtMaHang.AsEnumerable().FirstOrDefault(r => r["TenHang"].ToString().Trim() == _styleID.ToString().Trim());
                    if (rowmh != null)
                    {
                        styleID = rowmh["MaHang"].ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // kiểm tra khách hàng
                    string _cusName = tbl_Styles.Rows[0][2].ToString().TrimStart().TrimEnd();
                    if (string.IsNullOrEmpty(_cusName))
                    {
                        XtraMessageBox.Show("Không được bỏ trống khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
                    string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                    DataTable _dtKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                    if (_dtKH == null)
                    {
                        XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var rowkh = _dtKH.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == _cusName.ToString().Trim());
                    if (rowkh != null)
                    {
                        cusName = rowkh["MaKH"].ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //if (string.IsNullOrEmpty(tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd()))
                    //{
                    //    XtraMessageBox.Show("Không được bỏ trống chủng loại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                    string urlCL = string.Format("{0}?chungloai={1}", URL + "DonHangTong/GetChungLoai1", tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd());
                    string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
                    DataTable _dtCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);
                    if (_dtCL != null && _dtCL.Rows.Count > 0)
                    {
                        var rowcl = _dtCL.AsEnumerable().FirstOrDefault(r => r["TenCL"].ToString().Trim().ToUpper() == tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd().ToUpper());
                        if (rowcl != null)
                        {
                            chungloai = rowcl["MaCL"].ToString();
                        }

                    }
                    string jsonSize = string.Empty, jsonMau = string.Empty;

                    if (checkEditTV.Checked == false)
                    {
                        //size
                        string urlSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                        jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;

                        //Mau
                        string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                        jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    }
                    else
                    {
                        //size
                        string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", styleID.ToString(), cusName.ToString());
                        jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;

                        //Mau
                        string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", styleID.ToString(), cusName.ToString());
                        jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    }

                    DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

                    DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);


                    int Grand = 0;
                    for (int j = 1; j < tbColorPO.Rows.Count; j++)
                    {
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        else
                        {
                            for (int c = 6; c < tbColorPO.Columns.Count - 1; c++)
                            {
                                if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;

                                if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                {
                                    MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                    return;
                                }
                                else
                                {
                                    if (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") != "")
                                        totalAmount = totalAmount + (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") == "" ? 0 : Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", ""), "[^0-9a-zA-Z]+", "")));//row[col].ToString().Replace());
                                }

                            }
                        }
                    }

                    //string cleanedStringktmh = Regex.Replace(styleID, @"[^\w\d-]", "_");
                    //cleanedStringktmh = cleanedStringktmh.Replace(" ", "_");
                    string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", styleID);
                    string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                    List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                    bool isMessageShow = false;
                    foreach (DonHangTongEntity donhang in dohanglist)
                    {

                        if (donhang.MaKH == cusName.ToString() && donhang.MaHang == styleID.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(tbl_Styles.Rows[1][2].ToString().TrimStart().TrimEnd()).Trim().ToUpper().ToString().Replace(" ", "")
                           && donhang.SoLuong == totalAmount)
                        {
                            if (!isMessageShow)
                            {
                                DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (messResult == DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            isMessageShow = true;
                        }
                    }

                    string sizetrung = "", _inseam = "";
                    for (int j = 1; j < tbColorPO.Columns.Count - 1; j++)
                    {
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()))
                        {
                            //_inseam = tbColorPO.Rows[j][3].ToString().Trim();
                            _inseam = Regex.Replace(tbColorPO.Rows[j][3].ToString().Trim().ToUpper(), @"[^\w\d]+", "_");
                        }
                        //if (_inseam == "" && !_inseam.ToUpper().Contains("GRAND"))
                        //{
                        //    MessageBox.Show("InSeam không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                        //    return;
                        //}
                        if (_inseam == "" && !_inseam.ToUpper().Contains("GRAND"))
                        {
                            _inseam = "0";
                        }

                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                        }
                        HashSet<string> sizeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 6; i < tbColorPO.Columns.Count - 1; i++)
                        {

                            if (!string.IsNullOrEmpty(tbColorPO.Columns[i].ColumnName.ToString()))
                            {
                                pp = tbColorPO.Columns[i].ColumnName;
                            }

                            if (pp.Trim().ToUpper().Contains("TOTAL"))
                            {
                                if (pp.Trim().ToUpper().Contains("GRAND"))
                                    break;
                                continue;
                            }

                            string sizeID = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper() + "@Size@" + tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                            string rawSize = tbColorPO.Rows[0][i].ToString().Trim().Replace(" ", "").ToUpper();
                            if (sizeID.ToString() == "")
                            {
                                XtraMessageBox.Show("Size không được bỏ trống. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (sizeSet.Contains(rawSize))
                            {
                                XtraMessageBox.Show($"File Excel có 2 cột size giống nhau: '{rawSize}'. Vui lòng kiểm tra lại!",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                sizeSet.Add(rawSize);
                            }

                            var rowsize = _dtSizeNhom.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper()
                            && r["MaNhomSize"].ToString().Trim() == _inseam.ToString().Trim());
                            if (rowsize != null) break;
                            DataRow _dr = _dtSizeNhom.NewRow();
                            _dr["MaHang"] = styleID;
                            _dr["SizeSanXuat"] = tbColorPO.Rows[0][i].ToString().Trim().ToUpper();
                            _dr["MaNhomSize"] = _inseam.ToString().Replace(" ", "").Trim().ToUpper();
                            _dr["NhomSize"] = string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()) ? "0" : tbColorPO.Rows[j][3].ToString().Trim().ToUpper();
                            _dtSizeNhom.Rows.Add(_dr);
                            sizetrung = sizeID;

                        }
                    }
                    if (checkEditTV.Checked == false)
                    {
                        string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                        string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                        DataTable theinseam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                        var missingSizeSanXuat = _dtSizeNhom.AsEnumerable()
                            .Where(row1 =>
                                !dtSize.AsEnumerable().Any(row2 =>
                                    row1["SizeSanXuat"].ToString().Trim()
                                        .Equals(row2["SizeSanXuat"].ToString().Trim(),
                                                StringComparison.OrdinalIgnoreCase)))
                            .Select(row => row["SizeSanXuat"].ToString())
                            .Distinct()
                            .ToList();
                        if (missingSizeSanXuat.Any())
                        {
                            string msgSize = "Các Size chưa khai báo trong Thẻ Size:\n" +
                                             string.Join(", ", missingSizeSanXuat);

                            MessageBox.Show(msgSize, "Thiếu Size", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var missingNhomSize = _dtSizeNhom.AsEnumerable()
                            .Where(row1 =>
                                !theinseam.AsEnumerable().Any(row2 =>
                                    row1["NhomSize"].ToString().Trim()
                                        .Equals((row2["InSeam"]?.ToString() ?? "").Trim(),
                                                StringComparison.OrdinalIgnoreCase)))
                            .Select(row => row["NhomSize"].ToString())
                            .Distinct()
                            .ToList();
                        if (missingNhomSize.Any())
                        {
                            string msgInSeam = "Các InSeam chưa khai báo trong Thẻ InSeam:\n" +
                                               string.Join(", ", missingNhomSize);

                            MessageBox.Show(msgInSeam, "Thiếu InSeam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        // Đối chiếu dữ liệu và kiểm tra
                        var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                          where !dtSize.AsEnumerable().Any(row2 =>
                                              row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                              row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                              row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                          select row1;
                        //if (missingRows.Any())
                        //{
                        //    string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["SizeSanXuat"].ToString()));
                        //    string nhomsizeMissing = string.Join("; ", missingRows.Select(r => r["NhomSize"].ToString()).Distinct().ToArray());
                        //    MessageBox.Show("Size " + sizesanxuatMissing + " không có trong nhóm  " + nhomsizeMissing + " thuộc mã hàng  " + styleID.ToString() + " trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        //    return;
                        //}
                        if (missingRows.Any())
                        {
                            // Nhóm các dòng bị thiếu theo NhomSize
                            var groupedMissing = missingRows
                                .GroupBy(row => row["NhomSize"].ToString())
                                .Select(group => new
                                {
                                    NhomSize = group.Key, // Tên nhóm size
                                        Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                                    });

                            // Tạo chuỗi thông báo
                            var message = string.Join("\n", groupedMissing.Select(group =>
                                $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {styleID.ToString()}"));

                            // Hiển thị thông báo
                            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }


                    var uniqueValues = _dtSizeNhom.AsEnumerable().Select(x => new { MaHang = x["MaHang"], SizeSanXuat = x["SizeSanXuat"] }).Distinct().ToList();
                    foreach (var dr in uniqueValues)
                    {
                        var rowmasize = dtSize.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == dr.SizeSanXuat.ToString().Trim());
                        if (rowmasize != null)
                            _dtTable.Columns.Add(rowmasize["MaSize"].ToString() + "@Size@" + dr.SizeSanXuat.ToString(), typeof(string));
                    }
                    string _strdausize = string.Empty, _sizetrung = string.Empty;
                    string newHeader = string.Empty;
                    string dausize = string.Empty;
                    string _colorID = string.Empty, _colorName = string.Empty, _po = string.Empty, _maQG = string.Empty, _dausize = string.Empty, _colorCode = string.Empty;
                    DateTime _ngayGH;
                    string error = string.Empty, _potrung = string.Empty;


                    string ngayBHStr = string.Empty;
                    for (int j = 1; j < tbColorPO.Rows.Count; j++)
                    {
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        _colorCode = tbColorPO.Rows[j][0].ToString();
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][0].ToString()))
                        {
                            _colorCode = tbColorPO.Rows[j][0].ToString().Trim();
                        }
                        if (_colorCode == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            _colorCode = "";
                        }

                        _colorID = tbColorPO.Rows[j][1].ToString().Trim();
                        var _rowcolorID = dtMau.AsEnumerable().FirstOrDefault(x => Regex.Replace(x["TenMau"].ToString().Trim(), @"\s+", "").ToUpper() == Regex.Replace(tbColorPO.Rows[j][1].ToString().Trim(), @"\s+", "").ToUpper()
                        && x["CodeMau"].ToString().Trim() == tbColorPO.Rows[j][0].ToString().Trim());
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][1].ToString().ToUpper()))
                        {
                            if (_rowcolorID != null)
                            {
                                _colorName = _rowcolorID == null ? "" : _rowcolorID["MaMau"].ToString();
                            }
                            else
                            {
                                if (_rowcolorID == null)
                                {
                                    string code = tbColorPO.Rows[j][0].ToString().Trim();
                                    string ten = tbColorPO.Rows[j][1].ToString().Trim();

                                    if (!mauChuaCo.ContainsKey(code))
                                    {
                                        mauChuaCo.Add(code, ten);
                                    }
                                    // mauChuaCo.Add(tbColorPO.Rows[j][0].ToString().Trim(), tbColorPO.Rows[j][1].ToString().Trim());
                                }

                            }


                        }
                        //if (_colorName == "" && !tbColorPO.Rows[j][1].ToString().Trim().ToUpper().Contains("GRAND"))
                        //{
                        //    MessageBox.Show("Màu không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][2].ToString()))
                        {
                            _po = tbColorPO.Rows[j][2].ToString().Trim();
                        }
                        if (_po == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            MessageBox.Show("PO không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        //if (!string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()))
                        //{
                        //    _dausize = tbColorPO.Rows[j][3].ToString().Trim();
                        //}
                        _dausize = string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()) ? "0" : tbColorPO.Rows[j][3].ToString().Trim();
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][4].ToString()))
                        {
                            _maQG = tbColorPO.Rows[j][4].ToString().Trim();
                        }

                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][5].ToString()))
                        {

                            DateTime date_ngayBH;
                            if (DateTime.TryParseExact(tbColorPO.Rows[j][5].ToString().Trim().Replace("-", "/"), new[]
                            { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss", "MM/dd/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy", "MM/yyyy/dd h:mm:ss","M/d/yyyy h:mm:ss","M/d/yy h:mm:ss","MM/dd/yy h:mm:ss", "yy/MM/dd h:mm:ss", "yyyy/MM/dd h:mm:ss","dd-MMM-yy h:mm:ss","yyyy-MM-dd h:mm:ss", "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy h:mm:ss tt", "dd\\MM\\yyyy", "dd\\MM\\yyyy HH:mm:ss","dd\\MM\\yyyy h:mm:ss tt","dd-MMM-yyyy","dd-MMMM-yyyy","dd-MMM-yyyy HH:mm:ss","dd-MMMM-yyyy HH:mm:ss","yyyy/MM/dd","yyyy-MM-dd","yyyy.MM.dd","yyyy\\MM\\dd","yyyyMMdd","dd/MM/yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss","dd\\MM\\yyyy HH:mm:ss","dd/MM/yyyy hh:mm:ss tt","dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt","dd\\MM\\yyyy hh:mm:ss tt","yyyy-MM-ddTHH:mm:ssZ","yyyy-MM-ddTHH:mm:ss.fffZ","yyyy-MM-ddTHH:mm:sszzz","yyyy-MM-ddTHH:mm:ss.fffzzz","dd/MM/yy","dd-MM-yy","dd.MM.yy","dd\\MM\\yy","MM/dd/yy","MM-dd-yy","MM.dd.yy","MM\\dd\\yy","yyyy/MM/dd HH:mm:ss","yyyy-MM-dd HH:mm:ss","yyyy.MM.dd HH:mm:ss","yyyy\\MM\\dd HH:mm:ss","yyyy/MM/dd hh:mm:ss tt","yyyy-MM-dd hh:mm:ss tt","yyyy.MM.dd hh:mm:ss tt","yyyy\\MM\\dd hh:mm:ss tt"
                                }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date_ngayBH))
                            {
                                ngayBHStr = date_ngayBH.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                MessageBox.Show("Kiểm tra lại Ngày Giao Hàng chưa đúng định dạng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                        }
                        if (ngayBHStr == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            MessageBox.Show("Ngày giao hàng không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (_colorID.Trim().ToUpper().Contains("TOTAL"))
                        {
                            _po = "";
                            _colorName = "";
                            ngayBHStr = "";
                            _maQG = "";
                            if (_colorID.Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        else
                        {
                            DataRow _dr = _dtTable.NewRow();
                            int n = 11;
                            for (int c = 0; c < tbColorPO.Columns.Count - 1; c++)
                            {
                                if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;
                                if (c <= 5)
                                {
                                    _dr[0] = txtMaDH.Text + "|" + ReplaceSpecialCharacters(RemoveVietnameseTone(_po.Trim()));
                                    _dr[1] = _po.Trim();
                                    _dr[2] = _colorName.Trim();
                                    _dr[3] = _dausize.ToString().Trim();
                                    _dr[4] = _maQG.Trim();
                                    DateTime dateValue;

                                    try
                                    {
                                        dateValue = DateTime.ParseExact(ngayBHStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        _dr[5] = dateValue;
                                        _dr[7] = _colorCode.Trim() == "" ? "" : _colorCode;
                                        _dr[8] = DBNull.Value;
                                        _dr[9] = DBNull.Value;
                                        _dr[10] = DBNull.Value;
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("Chuỗi không có định dạng hợp lệ.");
                                    }

                                }
                                else
                                {
                                    if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                    {
                                        MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                        return;
                                    }
                                    else
                                    {
                                        _dr[n] = tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "");
                                        n++;
                                    }



                                }

                            }

                            _dtTable.Rows.Add(_dr);
                        }

                    }

                    if (mauChuaCo.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Các màu sau chưa được khai báo trong thẻ màu:");

                        foreach (var kv in mauChuaCo)
                        {
                            string code = string.IsNullOrEmpty(kv.Key) ? "(Không có mã)" : kv.Key;
                            string ten = string.IsNullOrEmpty(kv.Value) ? "(Không có tên màu)" : kv.Value;
                            sb.AppendLine(string.Format("- Code Màu: {0} | Màu: {1}", code, ten));
                        }

                        XtraMessageBox.Show(sb.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //if (!string.IsNullOrEmpty(_dtTable.Rows[0]["MaMau"].ToString())) 
                    //{
                    //    var RowsMau = from row1 in _dtTable.AsEnumerable()
                    //                  where !dtMau.AsEnumerable().Any(row2 =>
                    //                      styleID.ToString().ToUpper().Trim() == row2["MaHang"].ToString().ToUpper().Trim() &&
                    //                      row1["MaMau"].ToString().ToUpper().Trim() == row2["MaMau"].ToString().ToUpper().Trim())
                    //                  select row1;
                    //    if (RowsMau.Any())
                    //    {
                    //        string mauMissing = string.Join("; ", RowsMau.Select(r => r["MaMau"].ToString()).Distinct().ToArray());
                    //        MessageBox.Show("Màu " + mauMissing + " không có trong nhóm mã hàng " + styleID.ToString() + " trong thư viện màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                    //        return;
                    //    }
                    //}

                    foreach (DataRow _drmau in _dtTable.Rows)
                    {
                        var rowmau = dtMau.AsEnumerable().FirstOrDefault(r => r["TenMau"].ToString().Trim().ToUpper() == _drmau["MaMau"].ToString().Trim().ToUpper());
                        if (rowmau != null)
                            _drmau["MaMau"] = rowmau["TenMau"];
                    }

                    string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                    string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                    List<QuocGiaEntity> lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQG);

                    List<string> lstSaveQG = new List<string>();

                    for (int j = 0; j < _dtTable.Rows.Count; j++)
                    {

                        string KTQG = string.Empty;
                        QuocGiaEntity qg = lstQG.Where(item => RemoveVietnameseTone(item.TenQG).ToString().ToUpper().Trim() == RemoveVietnameseTone(_dtTable.Rows[j][4].ToString().ToUpper().Trim())).FirstOrDefault();
                        if (qg == null)
                        {
                            string hasDuplicates = lstSaveQG.Where(x => x.Equals(_dtTable.Rows[j][4].ToString())).FirstOrDefault();
                            if (string.IsNullOrEmpty(hasDuplicates))
                            {
                                if (_dtTable.Rows[j][4].ToString() != "")
                                    lstSaveQG.Add(_dtTable.Rows[j][4].ToString());
                            }
                        }
                    }
                    if (lstSaveQG.Count > 0)
                    {
                        AutoImportQGNew(lstSaveQG);
                    }
                    _dtTable = ChangeTenQGToMaQGNew(_dtTable);
                    _dtTable = _dtTable.AsEnumerable().OrderBy(x => x["PO"]).CopyToDataTable();
                    searchLookUpEditKH.EditValue = cusName;
                    searchLookUpEditMH.EditValue = styleID;
                    searchLookUpEditCL.EditValue = chungloai;
                    txtDot.Text = tbl_Styles.Rows[2][2].ToString().TrimStart().TrimEnd();
                    //txtSoBooking.Text = tbl_Styles?.Rows?.Count < 4 ? string.Empty : tbl_Styles.Rows[5][2]?.ToString()?.TrimStart()?.TrimEnd();
                    CreateSearchLookup();
                    CreateSearchLookupQG();
                    CreateSearchLookupQuocGia();
                    CreateSearchLookupMaMau();
                    CreateSearchLookupDauSize();
                    CreateSearchLookupCL();
                    bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                    {
                        DauSize = row.Field<string>("DauSize"),
                        POID = row.Field<string>("POID"),
                        MaMau = row.Field<string>("MaMau")
                    }).Any(g => g.Count() > 1);
                    if (checkDuplicateData)
                    {
                        XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _dtTable.Clear();
                        searchLookUpEditMH.EditValue = null;
                        searchLookUpEditKH.EditValue = null;
                        txtDot.Text = "";
                        searchLookUpEditCL.EditValue = null;
                        txtSoBooking.Text = string.Empty;
                        return;
                    }
                    gridBandSize.Children.Clear();
                    createTable(_dtTable);
                    gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                    BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                    mainView.CellValueChanging += mainView_CellValueChanging;
                    mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                    gridControlThongTinDonHang.DataSource = _dtTable;
                    gridControl1.DataSource = _dtSize;
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable dataSource = gridControlThongTinDonHang.DataSource as DataTable;
            if (!dataSource.Columns.Contains("CheckColumn"))
            {
                dataSource.Columns.Add("CheckColumn", typeof(bool));
            }

            // Gán giá trị mặc định true cho cột "CheckColumn" cho tất cả các dòng trong DataTable
            foreach (DataRow row in dataSource.Rows)
            {
                row["CheckColumn"] = true;
            }

        }

        private void txtDauSize_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            if (checkEditTV.Checked == false)
            {
                foreach (DataRowView rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["MaInSeam"].ToString());
                }
            }
            else
            {
                foreach (DataRowView rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["DauSize"].ToString());
                }
            }


            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn nhóm size----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtThem();
        }
        private void BtThem()
        {
            DataTable tblthem = gridControlThongTinDonHang.DataSource as DataTable;
            if (tblthem.Rows.Count > 0)
            {
                if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtDot.Text == "" || txtDot.Text == null)
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập tần suất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (checkEditImport.Checked == false)
                {
                    if (mau == null || mau == "")
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin Màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (dausize == null || dausize == "")
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin InSeam.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (size == null || size == "")
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin Size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (SearchLookUpEditQG.EditValue == null || SearchLookUpEditQG.EditValue == "")
                    {
                        MessageBox.Show("Vui lòng chọn Quốc gia.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                }
                if (searchlookupEditHT.EditValue == null || searchlookupEditHT.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn hình thức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //if (txtSoVoice.Text == null || txtSoVoice.Text == "")
                //{
                //    MessageBox.Show("Vui lòng nhập số invoice.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (spinEditHSDH.Text == null || spinEditHSDH.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (spinEditFOB.Text == null || spinEditFOB.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập FOB.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (txtDot.Text == null || txtDot.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (spinEditUSD.Text == null || spinEditUSD.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (!tblthem.Columns.Contains("IsNew"))
                {
                    tblthem.Columns.Add("IsNew", typeof(bool));
                }

                _status = ResourceURL.EventStatus.Add;
                DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;
                DataRow newDataRow = tbl.Rows.Add();
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    foreach (DataColumn column in tbl.Columns)
                    {
                        if (column.ColumnName.Contains("NgayGH"))
                        {
                            newDataRow["NgayGH"] = DateTime.Now.Date;
                        }
                        else if (column.ColumnName.Contains("DauSize"))
                        {
                            newDataRow["DauSize"] = 0;
                        }
                    }
                }
                newDataRow["IsNew"] = true;
                _rowAdd = tbl.Rows.Count - 1;
                //tbl.Rows.Add();
                CreateSearchLookupQuocGia();
                CreateSearchLookupDauSize();
                CreateSearchLookupMaMau();
                gridControlThongTinDonHang.DataSource = tbl;
            }
            else
            {
                return;
            }

        }

        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (view.FocusedColumn == null)
            {
                return;
            }
            if (_status == ResourceURL.EventStatus.Add)
            {
                if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd)
                {
                    if (!view.FocusedColumn.FieldName.Equals("Amount"))
                    {
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    }
                }
                else
                {
                    foreach (GridColumn column in view.Columns)
                    {
                        column.OptionsColumn.AllowEdit = !column.FieldName.Equals("PO");
                    }
                }

            }
            else
            {
                if (_status == ResourceURL.EventStatus.View)
                {
                    if (view.FocusedColumn.FieldName.Equals("PO") || view.FocusedColumn.FieldName.Equals("DauSize") || view.FocusedColumn.FieldName.Equals("MaMau"))
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
            //if (view.FocusedColumn.FieldName.Equals("PO"))
            //{
            //    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            //}
            //else 
            //{
            //    view.FocusedColumn.OptionsColumn.AllowEdit = true;
            //}

        }

        private void gridViewThongTinDonHang_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {

        }

        private void gridControlThongTinDonHang_ProcessGridKey(object sender, KeyEventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (e.Control && e.KeyCode == Keys.V)
            {
                string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length < 1) return;
                int startRow = view.FocusedRowHandle;
                foreach (string row in data)
                {
                    AddRow(row, startRow++);
                    if (!view.IsValidRowHandle(startRow)) break;
                }
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void gridViewThongTinDonHang_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }

        private void gridViewThongTinDonHang_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void CreateDefaultSearchLookUpHH()
        {

            //Mau
            txtMau.Properties.ValueMember = "MaMau";
            txtMau.Properties.DisplayMember = "TenMau";
            txtMau.Properties.ShowClearButton = false;
            txtMau.Properties.Appearance.ForeColor = Color.Red;
            txtMau.Properties.NullText = "[Chọn Màu]";
            txtMau.Properties.View.OptionsSelection.MultiSelect = true;
            txtMau.Properties.PopulateViewColumns();
            gridCheckMarksColor = new SearchCheckSelection(txtMau.Properties);
            gridCheckMarksColor.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit_Mau_SelectionChanged);
            txtMau.Properties.Tag = gridCheckMarksColor;

            //Size
            txtNhapSize.Properties.ValueMember = "SizeSanXuat";
            txtNhapSize.Properties.DisplayMember = "SizeSanXuat";
            txtNhapSize.Properties.ShowClearButton = false;
            txtNhapSize.Properties.Appearance.ForeColor = Color.Red;
            txtNhapSize.Properties.View.OptionsSelection.MultiSelect = true;
            txtNhapSize.Properties.NullText = "[Chọn Size]";
            txtNhapSize.Properties.PopulateViewColumns();
            gridCheckMarksSize = new SearchCheckSelection(txtNhapSize.Properties);
            gridCheckMarksSize.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditSize_SelectionChanged);
            txtNhapSize.Properties.Tag = gridCheckMarksSize;
            // Đăng ký sự kiện Popup để sắp xếp ngược lại
            txtNhapSize.Properties.Popup += new EventHandler(txtNhapSize_Popup);

            //DauSize
            txtDauSize.Properties.ValueMember = "DauSizeID";
            txtDauSize.Properties.DisplayMember = "DauSize";
            txtDauSize.Properties.ShowClearButton = false;
            txtDauSize.Properties.Appearance.ForeColor = Color.Red;
            txtDauSize.Properties.View.OptionsSelection.MultiSelect = true;
            txtDauSize.Properties.NullText = "[Chọn nhóm size]";
            txtDauSize.Properties.PopulateViewColumns();
            gridCheckMarksDauSize = new SearchCheckSelection(txtDauSize.Properties);
            gridCheckMarksDauSize.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditDauSize_SelectionChanged);
            txtDauSize.Properties.Tag = gridCheckMarksDauSize;


            InitSearInitSearchLookupMaMau();
            InitSearchLookupDauSize();
        }


        private void txtNhapSize_Popup(object sender, EventArgs e)
        {
            // Lấy SearchLookUpEdit
            SearchLookUpEdit searchLookUpEdit = sender as SearchLookUpEdit;
            if (searchLookUpEdit != null)
            {
                // Lấy GridView từ SearchLookUpEdit
                GridView gridView = searchLookUpEdit.Properties.View as GridView;

                if (gridView != null)
                {
                    // Đảm bảo cột "SizeSanXuat" tồn tại
                    if (gridView.Columns["SizeSanXuat"] != null)
                    {
                        // Xóa các sắp xếp hiện tại
                        // gridView.SortInfo.Clear();

                        // Thêm sắp xếp ngược lại theo cột "SizeSanXuat"
                        //gridView.SortInfo.Add(new GridColumnSortInfo(gridView.Columns["SizeSanXuat"], ColumnSortOrder.Descending));

                        // Áp dụng sắp xếp
                        gridView.RefreshData();
                    }
                }
            }
        }
        private void gridViewThongTinDonHang_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow row = view.GetDataRow(e.RowHandle);
            if (row != null && row.Table.Columns.Contains("IsNew") && row["IsNew"] is bool isNew && isNew)
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.BackColor2 = Color.LightYellow;
            }
        }

        private void gridViewThongTinDonHang_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            DataTable checkdauszie = gridControlThongTinDonHang.DataSource as DataTable;
            try
            {
                if (e.Column.FieldName == "DauSize")
                {
                    if (checkEditTV.Checked == true)
                    {
                        foreach (DataRow dr in _dtSizeNhom.Rows)
                        {
                            dr["NhomSize"] = e.Value?.ToString();
                        }
                        string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                        string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                        DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                        if (dtSize == null && dtSize.Rows.Count < 0)
                        {
                            MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                            return;
                        }
                        var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                          where !dtSize.AsEnumerable().Any(row2 =>
                                              row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                              row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                              row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                          select row1;
                        if (missingRows.Any())
                        {
                            // Nhóm các dòng bị thiếu theo NhomSize
                            var groupedMissing = missingRows
                                .GroupBy(row => row["NhomSize"].ToString())
                                .Select(group => new
                                {
                                    NhomSize = group.Key, // Tên nhóm size
                                    Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                                });

                            // Tạo chuỗi thông báo
                            var message = string.Join("\n", groupedMissing.Select(group =>
                                $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                            // Hiển thị thông báo
                            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;

                        }
                    }

                }
                //if (e.Column.FieldName == "MaMau")
                //{
                //    string styleID = searchLookUpEditMH.EditValue.ToString();
                //    string cusName = searchLookUpEditKH.EditValue.ToString();
                //    foreach (DataRow dr in _dtTable.Rows)
                //    {
                //        dr["MaMau"] = e.Value?.ToString();
                //    }

                //    string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", styleID.ToString(), cusName.ToString());
                //    string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                //    DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
                //    var RowsMau = from row1 in _dtTable.AsEnumerable()
                //                  where !dtMau.AsEnumerable().Any(row2 =>
                //                      styleID.ToString() == row2["MaHang"].ToString() &&
                //                      row1["MaMau"].ToString().Trim() == row2["MaMau"].ToString().Trim())
                //                  select row1;
                //    if (RowsMau.Any())
                //    {
                //        string mauMissing = string.Join("; ", RowsMau.Select(r => r["MaMau"].ToString()).Distinct().ToArray());
                //        MessageBox.Show("Màu " + mauMissing + " không có trong nhóm mã hàng " + styleID.ToString() + " trong thư viện màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                //        return;
                //    }
                //}
            }
            catch (Exception z)
            {
                return;
            }


        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmKhaiBaoALL frm = new frmKhaiBaoALL();
            // frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookup();
            string urlHH = string.Format("{0}?makh={1}", URL + "DonHangTong/GetHangHoaKH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditMH.Properties.DataSource = tblHH;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";

            InitSearInitSearchLookupMaMau();
            InitSearchLookupDauSize();
        }

        private void gridViewThongTinDonHang_MouseDown(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = gridViewThongTinDonHang.CalcHitInfo(e.Location);

            // Kiểm tra nếu nhấn vào ô trong cột "DauSize"
            if (hitInfo.InRowCell == true)
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "DauSize" || hitInfo.Column.FieldName == "MaQG" || hitInfo.Column.FieldName == "MaMau")
                {
                    gridViewThongTinDonHang.FocusedColumn = hitInfo.Column;
                    gridViewThongTinDonHang.FocusedRowHandle = hitInfo.RowHandle;
                    gridViewThongTinDonHang.ShowEditor(); // Kích hoạt editor

                    // Kiểm tra nếu editor là SearchLookUpEdit thì hiển thị popup ngay lập tức
                    if (gridViewThongTinDonHang.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                    {
                        searchLookUpEdit.ShowPopup();
                    }
                }
            }
        }

        private void gridViewThongTinDonHang_ShownEditor(object sender, EventArgs e)
        {

        }

        private void gridViewThongTinDonHang_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (gridViewThongTinDonHang.FocusedColumn != null && gridViewThongTinDonHang.FocusedColumn.FieldName == "DauSize" || gridViewThongTinDonHang.FocusedColumn.FieldName == "MaQG" || gridViewThongTinDonHang.FocusedColumn.FieldName == "MaMau")
            {
                gridControl1.BeginInvoke(new Action(() =>
                {
                    if (gridViewThongTinDonHang.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                    {
                        searchLookUpEdit.ShowPopup();
                    }
                }));
            }
        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmQuocGia frm = new frmQuocGia();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.FormClosing += (s, args) =>
            {

                CreateSearchLookupQG();
            };
            frm.ShowDialog();
        }

        private void txtNhapSize_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            var sortedSelection = gridCheckMark.Selection
                .Cast<DataRowView>()
                .OrderBy(rv => Convert.ToInt32(rv["Sort"])) // Chuyển thành số để sắp xếp đúng thứ tự
                .ToList();
            foreach (DataRowView rv in sortedSelection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["SizeSanXuat"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Size----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void gridViewThongTinDonHang_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {

        }

        private void searchLookUpEditSize_SelectionChanged(object sender, EventArgs e)
        {

            Control c = this.txtNhapSize;

            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["SizeSanXuat"].ToString());
                }

                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
                size = sb.ToString();
            }
        }


        private void searchLookUpEdit_Mau_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.txtMau;
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["MaMau"].ToString());
                }

                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
                mau = sb.ToString();

                txtMau.EditValue = mau;
            }

        }

        private void searchLookUpEditDauSize_SelectionChanged(object sender, EventArgs e)
        {

            Control c = this.txtDauSize;

            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                if (checkEditTV.Checked == false)
                {
                    foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(":"); }
                        sb.Append(rv["MaInSeam"].ToString());
                    }
                }
                else
                {
                    foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(":"); }
                        sb.Append(rv["DauSizeID"].ToString());
                    }
                }


                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
                dausize = sb.ToString();
                InitSearchLookupSize();
            }
        }


        private void InitSearInitSearchLookupMaMau()
        {
            try
            {
                if (txtMau.Enabled == true)
                {
                    if (searchLookUpEditMH.EditValue != null)
                    {
                        gridCheckMarksColor.Selection.Clear();
                        if (checkEditTV.Checked == false)
                        {
                            string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                            txtMau.Properties.DataSource = tblMau;
                        }
                        else
                        {
                            string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                            txtMau.Properties.DataSource = tblMau;
                        }
                        GridView dvView = txtMau.Properties.View;
                        if (dvView.Columns.Count == 1)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                            dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã Màu", Name = "bandedGridColumn3", Visible = false });
                            dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên Màu", Name = "bandedGridColumn8", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "CodeMau", Caption = "Code Màu", Name = "bandedGridColumn14", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TheMau", Caption = "Thẻ Màu", Visible = true });

                            // Cấu hình giao diện header
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Appearance.HeaderPanel.Options.UseBackColor = true;

                            // Group theo MaMau
                            dvView.Columns["TheMau"].GroupIndex = 0;

                            // Cho phép hiển thị nhóm
                            dvView.OptionsView.ShowGroupPanel = false;
                            dvView.ExpandAllGroups();
                            dvView.OptionsBehavior.AutoExpandAllGroups = true;
                            dvView.GroupFormat = "{1}";
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                //else 
                //{
                //    return;
                //}

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void InitSearchLookupSize()
        {
            try
            {
                if (txtNhapSize.Enabled == true)
                {
                    if (searchLookUpEditMH.EditValue != null)
                    {
                        gridCheckMarksSize.Selection.Clear();

                        if (checkEditTV.Checked == false)
                        {
                            string urlSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtNhapSize.Properties.DataSource = tblSize;
                        }
                        else
                        {
                            string urlSize = string.Format("{0}?mahang={1}&&makh={2}&&dausize={3}", URL + "DonHangTong/GetSizeEditV1", searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), dausize == null ? "" : dausize.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtNhapSize.Properties.DataSource = tblSize;
                        }
                        //rMauEdit.EditValueChanged += rMauEdit_EditValueChanged;
                        GridView dvView = txtNhapSize.Properties.View;

                        if (dvView.Columns.Count == 1)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            //dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                            //dvView.Columns.Add(new GridColumn { FieldName = "Chon", Caption = "", Name = "colChonHH", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TenSize", Caption = " Size Sản Xuất", Name = "bandedGridColumn12", Visible = true });
                            //dvView.Columns.Add(new GridColumn { FieldName = "TenSize", Caption = "Tên Size", Name = "bandedGridColumn13", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TheSize", Caption = "Thẻ Size", Visible = true });

                            // Group theo MaMau
                            dvView.Columns["TheSize"].GroupIndex = 0;

                            // Cho phép hiển thị nhóm
                            dvView.OptionsView.ShowGroupPanel = false;
                            dvView.ExpandAllGroups();
                            dvView.OptionsBehavior.AutoExpandAllGroups = true;
                            dvView.GroupFormat = "{1}";
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                //else 
                //{
                //    return;
                //}

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitSearchLookupDauSize()
        {
            try
            {
                if (txtDauSize.Enabled == true)
                {
                    if (searchLookUpEditMH.EditValue != null)
                    {
                        gridCheckMarksDauSize.Selection.Clear();
                        if (checkEditTV.Checked == false)
                        {
                            string urlDSize = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSize); }).Result;
                            DataTable tblDSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtDauSize.Properties.DataSource = tblDSize;
                        }
                        else
                        {
                            string urlDSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetDauSizeV1", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSize); }).Result;
                            DataTable tblDSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtDauSize.Properties.DataSource = tblDSize;
                        }

                        //rMauEdit.EditValueChanged += rMauEdit_EditValueChanged;
                        GridView dvView = txtDauSize.Properties.View;
                        string[] colNeedRemove = new string[] { "MaInSeam", "DauSizeID", "InSeam", "DauSize", "TheInSeam" };
                        foreach (GridColumn col in dvView.Columns.Cast<GridColumn>().ToList())
                        {
                            if (colNeedRemove.Contains(col.FieldName))
                                dvView.Columns.Remove(col);
                        }
                        //if (dvView.Columns.Count == 1)
                        //{
                        //    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        //    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        //    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        //    //dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                        //    //dvView.Columns.Add(new GridColumn { FieldName = "Chon", Caption = "", Name = "colChonHH", Visible = true });
                        //    dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "MaInSeam" : "DauSizeID", Caption = " Mã Đầu Size", Name = "", Visible = false });
                        //    dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "InSeam" : "DauSize", Caption = "InSeam", Name = "bandedGridColumn4", Visible = true });
                        //    dvView.Columns.Add(new GridColumn { FieldName = "TheInSeam", Caption = "Nhóm thẻ InSeam", Visible = true });

                        //    // Group theo MaMau
                        //    dvView.Columns["TheInSeam"].GroupIndex = 0;

                        //    // Cho phép hiển thị nhóm
                        //    dvView.OptionsView.ShowGroupPanel = false;
                        //    dvView.ExpandAllGroups();
                        //    dvView.OptionsBehavior.AutoExpandAllGroups = true;
                        //    dvView.GroupFormat = "{1}";
                        //}
                        dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        //dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                        //dvView.Columns.Add(new GridColumn { FieldName = "Chon", Caption = "", Name = "colChonHH", Visible = true });
                        dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "MaInSeam" : "DauSizeID", Caption = " Mã Đầu Size", Name = "", Visible = false });
                        dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "InSeam" : "DauSize", Caption = "InSeam", Name = "bandedGridColumn4", Visible = true });
                        dvView.Columns.Add(new GridColumn { FieldName = "TheInSeam", Caption = "Nhóm thẻ InSeam", Visible = true });

                        // Group theo MaMau
                        dvView.Columns["TheInSeam"].GroupIndex = 0;

                        // Cho phép hiển thị nhóm
                        dvView.OptionsView.ShowGroupPanel = false;
                        dvView.ExpandAllGroups();
                        dvView.OptionsBehavior.AutoExpandAllGroups = true;
                        dvView.GroupFormat = "{1}";
                    }
                    else
                    {
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSearchLookupHT()
        {
            string url = string.Format("{0}?", URL + "HinhThuc/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbht = JsonConvert.DeserializeObject<DataTable>(json);
            tbht.Columns.Add("Chon", typeof(bool));
            searchlookupEditHT.Properties.DataSource = tbht;
            searchlookupEditHT.Properties.ValueMember = "MaHT";
            searchlookupEditHT.Properties.DisplayMember = "TenHT";
            if (tbht != null && tbht.Rows.Count > 0)
            {
                searchlookupEditHT.EditValue = tbht.Rows[0]["MaHT"];
            }
        }



        private void checkEditTV_CheckedChanged(object sender, EventArgs e)
        {
            InitSearchLookupDauSize();
            InitSearInitSearchLookupMaMau();
        }



        private void gridViewThongTinDonHang_KeyDown(object sender, KeyEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Phím tắt: Ctrl + D
            if (e.Control && e.KeyCode == Keys.D)
            {
                FillSelectedRowsWithDate(view);
                e.Handled = true;
            }
        }



        private void FillSelectedRowsWithDate(BandedGridView view)
        {
            // Lấy giá trị Date từ DateEdit
            if (dateEdit1.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn ngày trước khi fill!", "Thông báo");
                return;
            }

            DateTime date = Convert.ToDateTime(dateEdit1.EditValue);

            // Xác định cột đang focus
            var column = view.FocusedColumn;
            if (column == null) return;

            // Kiểm tra cột có phải cột ngày hợp lệ không
            if (!(column.FieldName == "NgayGH"
               || column.FieldName == "NgayDKVC"
               || column.FieldName == "NgayThucTeVC"
               || column.FieldName == "NgayXuatHang"))
            {
                XtraMessageBox.Show("Hãy focus vào cột ngày bạn muốn fill!", "Cảnh báo");
                return;
            }

            view.BeginUpdate();
            try
            {
                foreach (int handle in view.GetSelectedRows())
                {
                    if (handle >= 0)
                    {
                        view.SetRowCellValue(handle, column, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void dateEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit1.EditValue == null) return;

            // Lấy ngày chọn
            DateTime date = Convert.ToDateTime(dateEdit1.EditValue);

            // Lấy danh sách cell đang chọn
            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayGH"; // cột muốn fill
            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                // Nếu có cell được chọn
                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void dateEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit2.EditValue == null) return;

            DateTime date = Convert.ToDateTime(dateEdit2.EditValue);

            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayDKVC";

            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }
        private void dateEdit3_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit3.EditValue == null) return;

            DateTime date = Convert.ToDateTime(dateEdit3.EditValue);

            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayThucTeVC";
            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void dateEdit4_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit4.EditValue == null) return;

            DateTime date = Convert.ToDateTime(dateEdit4.EditValue);

            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayXuatHang";
            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCoppyMaHang frm = new frmCoppyMaHang();
            //frm.StartPosition = FormStartPosition.CenterParent;
            frm.OnCopyConfirmed += (dht, dhtct) =>
            {
                DataTable dtBDH_FromCopy = dht.Copy();
                _dtTable = dhtct;
                SetData(dtBDH_FromCopy);
                CreateSearchLookup();
                CreateSearchLookupQG();
                CreateSearchLookupQuocGia();
                CreateSearchLookupMaMau();
                CreateSearchLookupDauSize();
                CreateSearchLookupCL();
                gridBandSize.Children.Clear();
                for (int j = 0; j < _dtTable.Rows.Count; j++)
                {
                    string col1 = _dtTable.Rows[j][1].ToString().Trim();
                    if (!string.IsNullOrEmpty(col1))
                    {
                        _dtTable.Rows[j][0] = txtMaDH.Text.Trim() + "|" + Regex.Replace(col1, @"[^\w\s']", "").Replace(" ", "_").Replace("'", "_");
                    }

                }
                createTable2(_dtTable);
                gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                mainView.CellValueChanging += mainView_CellValueChanging;
                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                gridControlThongTinDonHang.DataSource = _dtTable;

            };


            frm.ShowDialog(this);
        }

        private void SetData(DataTable dhtcoppy)
        {
            this.Them.Enabled = true;
            searchLookUpEditKH.EditValue = dhtcoppy.Rows[0]["MaKH"];
            searchLookUpEditMH.EditValue = dhtcoppy.Rows[0]["MaHang"];
            searchLookUpEditCL.EditValue = dhtcoppy.Rows[0]["MaCL"];
            txtNguoiTao.Text = dhtcoppy.Rows[0]["NguoiTao"].ToString();
            spinEditVND.EditValue = dhtcoppy.Rows[0]["VND"];
            spinEditUSD.EditValue = dhtcoppy.Rows[0]["CM"];
            txtGhiChu.Text = dhtcoppy.Rows[0]["GhiChu"].ToString();
            txtSoVoice.Text = dhtcoppy.Rows[0]["SoVoice"].ToString();
            txtDot.EditValue = dhtcoppy.Rows[0]["Dot"];
            searchLookUpEditCTY.EditValue = dhtcoppy.Rows[0]["MaCty"];
            spinEditFOB.EditValue = dhtcoppy.Rows[0]["FOB"] != null ? dhtcoppy.Rows[0]["FOB"] : 0;
            spinEditHSDH.EditValue = dhtcoppy.Rows[0]["HeSoDH"] != null ? dhtcoppy.Rows[0]["HeSoDH"] : 0;
            searchlookupEditHT.EditValue = dhtcoppy.Rows[0]["HinhThucXuatHang"];
        }

        private void createTable2(DataTable tab)
        {
            //gridViewThongTinDonHang.OptionsView.AllowCellMerge = false;

            gridBandSize.Children.Clear();
            GridBand parentBand = gridViewThongTinDonHang.Bands["gridBandSize"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < gridViewThongTinDonHang.Columns.Count;)
                {
                    if (gridViewThongTinDonHang.Columns[i].FieldName.Contains("@Size@"))
                    {
                        gridViewThongTinDonHang.Columns.RemoveAt(i);
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
                if (demColIndex > 10 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[2];
                    String colName1 = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = true;
                    col.Visible = true;
                    col.Width = 65;
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    gridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 65;
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }
    }
}



