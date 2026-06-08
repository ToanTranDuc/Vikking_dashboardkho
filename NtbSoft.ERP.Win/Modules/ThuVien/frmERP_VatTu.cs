using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Erp.Kho;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_VatTu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblVatTu;
        SearchCheckSelection gridCheckMarksColor;
        string mau;
        DataTable _dttable;
        DataTable tblCNL;
        DataTable tblCPL;
        DataTable dttbmh;
        DataTable rowselectmh;
        int _rowAdd = -1;
        string _makh = string.Empty, _mahang = string.Empty, _nhom = string.Empty, _madvvt = string.Empty, _mamausp = string.Empty, _khovaiid = string.Empty;
        bool _isNPL = true;
        DataTable _tblNhom = new DataTable();
        bool _isSaveNL = true, _isSavePL = true;
        DataTable _tblMauSP = new DataTable();
        DataTable _tblKhoVai = new DataTable();
        DataTable tblItemCode = new DataTable();
        private HashSet<DataRow> selectedRowsKhoVai = new HashSet<DataRow>();

        string templateMaVTGhep = "{NhomVietTat}|{ItemCode}|{MauVT}|{KhoSize}";
        private List<GridCell> savedSelectedCells = new List<GridCell>();
        private string imagePath = string.Empty;
        Dictionary<string, string> MaVTGhep = new Dictionary<string, string>
        {

            { "NhomVietTat", "00" },
            { "ItemCode", "00" },
            { "MauVT", "00" },
            { "KhoSize", "00" }

        };
        string Host = string.Empty; bool indicatorIcon = true;
        private int indexFCNL = 0, indexFCPL = 0;
        private bool timAll = false;
        DataTable tblmsp = new DataTable();
        DataTable tblmvt = new DataTable();
        DataTable tblKH = new DataTable();
        RepositoryItemSearchLookUpEdit rCountryEditdv = new RepositoryItemSearchLookUpEdit();
        RepositoryItemSearchLookUpEdit rCountryEditkvNL;
        RepositoryItemSearchLookUpEdit rCountryEditkvPL;
        DataTable tblVatTuTVCheck = new DataTable();
        DataTable tblMauVTTVCheck = new DataTable();
        string _tudoTrung = string.Empty;
        DataRowView selectedRownlmau = null;
        DataRowView selectedRowpl = null;
        DataRowView selectedRowplmau = null;
        DataRowView selectedRow = null;
        DataRow _rowFocus = null;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private string selectedSourcePath;
        Dictionary<int, Image> imageCache = new Dictionary<int, Image>();
        Dictionary<int, Image> imageCachePL = new Dictionary<int, Image>();
        private readonly Dictionary<string, Image> inMemoryImages = new Dictionary<string, Image>();
        private bool isKV = true;
        private HashSet<int> loadingRows = new HashSet<int>();
        private SemaphoreSlim loadSem = new SemaphoreSlim(Environment.ProcessorCount);

        Dictionary<string, int> idToIndexMapNL = new Dictionary<string, int>();
        Dictionary<string, int> idToIndexMapPL = new Dictionary<string, int>();
        public frmERP_VatTu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblVatTu = new DataTable();
            Host = (string)settingsReader.GetValue("HostDH", typeof(String));
        }
        public frmERP_VatTu(string makh = "", string mahang = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblVatTu = new DataTable();
            _makh = makh;
            _mahang = mahang;
        }
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                picVT.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                picVT.Properties.PictureInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                imagePath = "";
                CheckPerminsion();
                CheckPerminsionExcel();
                CreateSearchLookUpNhom();
                CreateSearchLookUpKhoVai();
                CreateSearchLookUpDonVi();
                CreateSearchLookUpMauVT();
                CreateSearchLookUpDVVT();
                CreateSearchLookUpKHO();
                loadSearchLookUpMaMauVT();
                loadSearchLookUpMaVT();
                loadTimKiem();
                loadVatTuCheck();
                loadVatTu();
                LoadOptionSelectKV();
                loadLocChungLoai();

                this.ActiveControl = button1;

            }
            catch (Exception ex)
            {

            }


        }
        //chỉnh phím tắt
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (keyData == (Keys.Control | Keys.Delete))
            //{
            //    btnClear.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.S))
            //{
            //    btnLuu.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.A))
            //{
            //    simpleButton1.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.T))
            //{
            //    simpleButton2.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.Shift | Keys.E))
            //{
            //    btnNhapExcel.PerformClick();
            //    return true;
            //}
            //if (keyData == (Keys.Control | Keys.I))
            //{
            //    btnSelectImage.PerformClick();
            //    return true;
            //}
            return base.ProcessCmdKey(ref msg, keyData);
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
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            if (!_allowEdit && !_allowAdd)
            {

                btnLuu.Enabled = false;

            }



        }
        private void CheckPerminsionExcel()
        {

            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenExcelTS/Get", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl.Rows[0]["AllowExcel"].ToString() == "True")
                {
                    gridColumn63.Visible = true;
                    gridColumn64.Visible = true;
                    //       int maxIndex = gVNL.VisibleColumns
                    //   .Where(c => c != gridColumn63)
                    //   .Max(c => c.VisibleIndex);

                    //       int maxIndexPL = gVPL.VisibleColumns
                    //.Where(c =>  c != gridColumn64)
                    //.Max(c => c.VisibleIndex);
                    //       gridColumn63.VisibleIndex = 10;
                    //       gridColumn64.VisibleIndex = 10;
                    gridColumn63.VisibleIndex = gVNL.Columns.Count;
                    gridColumn64.VisibleIndex = gVPL.Columns.Count;
                }
            }



        }
        #region tạo searchLookUp
        private void CreateSearchLookUpMauVT()
        {
            try
            {
                searckLookUpEditMauVT.Properties.ValueMember = "MauVTID";
                searckLookUpEditMauVT.Properties.DisplayMember = "MaMauVT";
                searckLookUpEditMauVT.Properties.NullText = "Chọn màu vật tư";
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETCOLOR";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;
                searckLookUpEditMauVT.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {
            }
        }



        private void CreateSearchLookUpNhom()
        {
            try
            {
                searchLookUpEditNhom.Properties.ValueMember = "MaCLVT";
                searchLookUpEditNhom.Properties.DisplayMember = "TenCLVT";
                searchLookUpEditNhom.Properties.NullText = "Chọn chủng loại";
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETCHUNGLOAIV2";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                _tblNhom = JsonConvert.DeserializeObject<DataTable>(json);
                if (_tblNhom == null || _tblNhom.Rows.Count == 0) return;
                searchLookUpEditNhom.Properties.DataSource = _tblNhom;
            }
            catch (Exception ex)
            {


            }

        }
        private void CreateSearchLookUpDonVi()
        {
            try
            {
                searchLookUpEditDV.Properties.ValueMember = "MaDVVT";
                searchLookUpEditDV.Properties.DisplayMember = "TenDVVT";
                searchLookUpEditDV.Properties.NullText = "Chọn đơn vị";
                string url = string.Format("{0}?", URL + "ERPVatTuBOM/GetDonVi");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tblDV = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblDV == null || tblDV.Rows.Count == 0) return;
                searchLookUpEditDV.Properties.DataSource = tblDV;
            }
            catch (Exception ex)
            {
            }
        }

        //gán cho searchLookUp ở trên
        private void CreateSearchLookUpKhoVai()
        {
            try
            {
                string nhom = string.Empty;
                if (searchLookUpEditNhom.EditValue != null && !string.IsNullOrWhiteSpace(searchLookUpEditNhom.EditValue.ToString()))
                {
                    nhom = searchLookUpEditNhom.EditValue.ToString();
                }
                string ActionGet = CurrentSelectedOption == OptionType.SanPham ? "GETSizeSP" : "GETKHOSIZE";
                string url = $"{URL}ERPVatTuBOM/GetChung?Action={ActionGet}&para={isKV}&para1={nhom}&para2={CurrentSelectedOption}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                searchLookUpEditKV.Properties.ValueMember = "KhoVaiID";
                searchLookUpEditKV.Properties.DisplayMember = "KhoVai";
                searchLookUpEditKV.Properties.NullText = "Chọn khổ";

                if (json == "[]")
                {
                    searchLookUpEditKV.Properties.DataSource = new DataTable();
                    searchLookUpEditKV.EditValue = null;
                    return;
                }
                DataTable tblKV = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblKV == null || tblKV.Rows.Count == 0) return;
                for (int i = tblKV.Rows.Count - 1; i >= 0; i--)
                {
                    var value = tblKV.Rows[i]["KhoVai"];
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        tblKV.Rows.RemoveAt(i);
                    }
                }

                //if (tblKV.Rows.Count == 0) return;
                searchLookUpEditKV.Properties.DataSource = tblKV;
                searchLookUpEditKV.EditValue = null;
                searchLookUpEditViewKV.ClearGrouping();
                switch (CurrentSelectedOption)
                {
                    case OptionType.ChungLoai:
                        rcolTenNhom.Visible = false;
                        rcolTheKhoVai.Visible = true;
                        rcolDVKhoVai.Visible = false;
                        rcolTheKhoVai.GroupIndex = 0;
                        break;

                    case OptionType.SanPham:
                        rcolTheKhoVai.Visible = false;
                        rcolDVKhoVai.Visible = false;
                        rcolTenNhom.GroupIndex = 0;
                        break;

                    case OptionType.VatTu:
                        rcolTenNhom.Visible = true;
                        rcolTheKhoVai.Visible = true;
                        rcolDVKhoVai.Visible = false;
                        rcolTenNhom.GroupIndex = 0;
                        rcolTheKhoVai.GroupIndex = 1;
                        break;
                }



            }
            catch (Exception ex)
            {
            }

        }
        private void CreateSearchLookUpDVVT()
        {
            try
            {
                string urldv = string.Format("{0}", URL + "ERPVatTuBOM/GetDonVi");
                string jsondv = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldv); }).Result;
                DataTable tbldv = JsonConvert.DeserializeObject<DataTable>(jsondv);
                rCountryEditdv = new RepositoryItemSearchLookUpEdit();
                rCountryEditdv.DataSource = tbldv;
                rCountryEditdv.DisplayMember = "TenDVVT";
                rCountryEditdv.ValueMember = "MaDVVT";
                rCountryEditdv.ShowClearButton = false;
                rCountryEditdv.NullText = "[Chọn giá trị]";
                rCountryEditdv.ImmediatePopup = true;

                GridView dvViewdvvt = rCountryEditdv.View;
                if (dvViewdvvt.Columns.Count == 0)
                {
                    dvViewdvvt.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvViewdvvt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvViewdvvt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvViewdvvt.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvViewdvvt.Columns.Add(new GridColumn { FieldName = "MaDVVT", Caption = "Mã DV", Name = "colMaMau", Visible = false });
                    dvViewdvvt.Columns.Add(new GridColumn { FieldName = "TenDVVT", Caption = "Đơn Vị", Name = "colTenMau", Visible = true });

                }
                rCountryEditdv.Popup += rCountryEditdv_Popup;
                colTenDVVTNL.ColumnEdit = rCountryEditdv;
                colTenDVVTPL.ColumnEdit = rCountryEditdv;
                LoadDVSD(tbldv);

            }
            catch (Exception ex)
            {
            }
        }
        DataTable tblkv = new DataTable();
        private void CreateSearchLookUpKHO()
        {
            try
            {
                string urlkv = string.Format("{0}?makh={1}&&mahang={2}", URL + "ERPVatTuBOM/GetKhoVai", "", "");
                string jsonkv = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkv); }).Result;
                if (jsonkv == "[]") return;
                tblkv = JsonConvert.DeserializeObject<DataTable>(jsonkv);
                _tblKhoVai = tblkv;

                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETKVNL";
                string jsonkvNL = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (jsonkvNL == "[]") return;
                DataTable tblkvNL = JsonConvert.DeserializeObject<DataTable>(jsonkvNL);
                rCountryEditkvNL = new RepositoryItemSearchLookUpEdit();
                rCountryEditkvNL.DataSource = tblkvNL;
                rCountryEditkvNL.DisplayMember = "KhoVai";
                rCountryEditkvNL.ValueMember = "KhoVaiID";
                rCountryEditkvNL.ShowClearButton = false;
                rCountryEditkvNL.NullText = "[Chọn]";
                rCountryEditkvNL.ImmediatePopup = true;
                //rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvViewkv = rCountryEditkvNL.View;
                if (dvViewkv.Columns.Count == 0)
                {
                    dvViewkv.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvViewkv.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvViewkv.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvViewkv.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvViewkv.Columns.Add(new GridColumn { FieldName = "KhoVaiID", Caption = "Mã Khổ", Name = "colMaMau", Visible = false });
                    dvViewkv.Columns.Add(new GridColumn { FieldName = "KhoVai", Caption = "Khổ", Name = "colTenMau", Visible = true });
                    dvViewkv.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Chủng loại", Name = "colTenNhom", Visible = true });
                    dvViewkv.Columns.Add(new GridColumn { FieldName = "LoaiSize", Caption = "Loại", Name = "colLoaiVT", Visible = true });
                    dvViewkv.Columns.Add(new GridColumn { FieldName = "TenDVKV", Caption = "Đơn vị", Name = "colTenDVKV", Visible = false });

                }
                dvViewkv.Columns["LoaiSize"].GroupIndex = 0;
                dvViewkv.Columns["TenNhom"].GroupIndex = 1;
                dvViewkv.OptionsBehavior.AutoExpandAllGroups = true;
                dvViewkv.CustomDrawGroupRow += View_CustomDrawGroupRow;
                rCountryEditkvNL.Popup += rCountryEditkv_Popup;
                rCountryEditkvNL.EditValueChanged += rCountryEditkv_EditValueChanged;

                colKhoVaiNL.ColumnEdit = rCountryEditkvNL;


                string urlPL = $"{URL}ERPVatTuBOM/GetChung?Action=GETKVPL";
                string jsonkvPL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPL); }).Result;
                if (jsonkvPL == "[]") return;
                DataTable tblkvPL = JsonConvert.DeserializeObject<DataTable>(jsonkvPL);
                rCountryEditkvPL = new RepositoryItemSearchLookUpEdit();
                rCountryEditkvPL.DataSource = tblkvPL;
                rCountryEditkvPL.DisplayMember = "KhoVai";
                rCountryEditkvPL.ValueMember = "KhoVaiID";
                rCountryEditkvPL.ShowClearButton = false;
                rCountryEditkvPL.NullText = "[Chọn]";
                rCountryEditkvPL.ImmediatePopup = true;
                //rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvViewkvPL = rCountryEditkvPL.View;
                if (dvViewkvPL.Columns.Count == 0)
                {
                    dvViewkvPL.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvViewkvPL.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvViewkvPL.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvViewkvPL.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvViewkvPL.Columns.Add(new GridColumn { FieldName = "KhoVaiID", Caption = "Mã Khổ", Name = "colMaMau", Visible = false });
                    dvViewkvPL.Columns.Add(new GridColumn { FieldName = "KhoVai", Caption = "Khổ", Name = "colTenMau", Visible = true });
                    dvViewkvPL.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Chủng loại", Name = "colTenNhom", Visible = true });
                    dvViewkvPL.Columns.Add(new GridColumn { FieldName = "LoaiSize", Caption = "Loại", Name = "colLoaiVT", Visible = true });
                    dvViewkvPL.Columns.Add(new GridColumn { FieldName = "TenDVKV", Caption = "Đơn vị", Name = "colTenDVKV", Visible = false });

                }
                dvViewkvPL.CustomDrawGroupRow += View_CustomDrawGroupRow;
                dvViewkvPL.Columns["LoaiSize"].GroupIndex = 0;
                dvViewkvPL.Columns["TenNhom"].GroupIndex = 1;
                dvViewkvPL.OptionsBehavior.AutoExpandAllGroups = true;
                rCountryEditkvPL.Popup += rCountryEditkv_Popup;
                rCountryEditkvPL.EditValueChanged += rCountryEditkv_EditValueChanged;
                colKhoVaiNL.ColumnEdit = rCountryEditkvNL;
                colKhoVaiPL.ColumnEdit = rCountryEditkvPL;
            }
            catch (Exception ex)
            {
            }

        }
        private void View_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            var view = sender as GridView;
            var info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;
            info.GroupText = string.Format("{0}", info.GroupValueText);
            if (view.IsGroupRow(e.RowHandle))
            {


                int groupIndex = view.GetRowLevel(e.RowHandle);
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

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }
        private void rCountryEditkv_EditValueChanged(object sender, EventArgs e)
        {
            changeKhoVai(sender, e);
        }
        private void changeKhoVai(object sender, EventArgs e)
        {
            try
            {
                SearchLookUpEdit edit = sender as SearchLookUpEdit;
                GridView view = edit.Properties.View as GridView;
                DataRowView row = view.GetRow(view.FocusedRowHandle) as DataRowView;

                if (row != null)
                {

                    var value1 = row["KhoVai"];

                    DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                    if (dr == null) return;
                    dr["KhoVai"] = value1.ToString();
                    dr["IsKV"] = row["IsKV"].ToString() == "1" ? true : false;
                    dr["MaTheSize"] = row["MaTheSize"].ToString();

                }
            }
            catch (Exception ex)
            {
            }

        }

        #endregion

        private void loadVatTu()
        {
            try
            {

                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");






                string manhom = searchLookUpEditLocCL.EditValue == null ? "" : searchLookUpEditLocCL.EditValue.ToString();
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=Get&para={manhom}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;



                if (json == "[]")
                {
                    _dttable = CreateTable();
                    if (tblCNL != null && tblCNL.Rows.Count != 0)
                        tblCNL.Clear();
                    if (tblCPL != null && tblCPL.Rows.Count != 0)
                        tblCPL.Clear();
                    tblCNL = _dttable.Clone();
                    tblCPL = _dttable.Clone();
                    gCNL.DataSource = tblCNL;
                    gCPL.DataSource = tblCPL;
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                if (_dttable != null && _dttable.Rows.Count != 0)
                {
                    _dttable.Clear();
                }

                _dttable = JsonConvert.DeserializeObject<DataTable>(json);
                if (!_dttable.Columns.Contains("IsNew"))
                {
                    _dttable.Columns.Add("IsNew", typeof(int));
                }
                if (!_dttable.Columns.Contains("STT"))
                {
                    _dttable.Columns.Add("STT", typeof(int));
                }
                if (!_dttable.Columns.Contains("IsEdit"))
                {
                    _dttable.Columns.Add("IsEdit", typeof(int));
                }
                if (_dttable == null || _dttable.Rows.Count == 0)
                {
                    gCNL.DataSource = null;
                    gCNL.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }



                var rowsCNL = _dttable.AsEnumerable()
                                      .Where(row => row.Field<bool?>("NPL") == true);
                var rowsCPL = _dttable.AsEnumerable()
                                      .Where(row => row.Field<bool?>("NPL") == false);
                if (rowsCNL.Any())
                {
                    tblCNL = rowsCNL.CopyToDataTable();
                }
                else
                {
                    tblCNL = _dttable.Clone();
                }
                if (rowsCPL.Any())
                {
                    tblCPL = rowsCPL.CopyToDataTable();
                }
                else
                {
                    tblCPL = _dttable.Clone();
                }

                if (tblCNL.Rows.Count == 0) tblCNL = _dttable.Clone();
                if (tblCPL.Rows.Count == 0) tblCPL = _dttable.Clone();

                //RefreshSTTColumn(tblCNL);
                //RefreshSTTColumn(tblCPL);
                gVNL.SelectionChanged -= gVNL_SelectionChanged;
                gVPL.SelectionChanged -= gVPL_SelectionChanged;
                gCNL.DataSource = tblCNL;
                gCPL.DataSource = tblCPL;
                gVNL.ClearSelection();
                gVPL.ClearSelection();
                if (_isNPL)
                {
                    RestoreSelectedCells(gVNL);
                }
                else
                {
                    RestoreSelectedCells(gVPL);
                }
                gVNL.SelectionChanged += gVNL_SelectionChanged;
                gVPL.SelectionChanged += gVPL_SelectionChanged;
                
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);  //MessageBox.Show("loadVatTu " + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

        }


        private void loadVatTuCheck()
        {
            string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETVATTUTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                tblVatTuTVCheck = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                tblVatTuTVCheck = null;
                return;
            }
            tblVatTuTVCheck = tbl;

        }



        #region tạo bảng
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
                dtbcreate.Columns.Add("urlAnh", typeof(string));
                dtbcreate.Columns.Add("IsKV", typeof(bool));
                dtbcreate.Columns.Add("MaTheSize", typeof(string));
                dtbcreate.Columns.Add("VatTuCoSan", typeof(string));
                return dtbcreate;
            }
            catch (Exception ex)
            {
                return null;
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
                dtbthongso.Columns.Add("urlAnh", typeof(string));
                dtbthongso.Columns.Add("IsKV", typeof(bool));
                dtbthongso.Columns.Add("MaTheSize", typeof(string));

                dtbthongso.Columns.Add("TrongLuongChuan", typeof(decimal));
                dtbthongso.Columns.Add("DungSai", typeof(string));
                dtbthongso.Columns.Add("SpecificationPDF", typeof(string));
                dtbthongso.Columns.Add("TestReportPDF", typeof(string));
                dtbthongso.Columns.Add("NgayBC", typeof(DateTime));
                dtbthongso.Columns.Add("OekotexPDF", typeof(string));
                dtbthongso.Columns.Add("NgayHetHan", typeof(DateTime));
                dtbthongso.Columns.Add("MaDVVTSD", typeof(string));
                dtbthongso.Columns.Add("Tile", typeof(decimal));
                dtbthongso.Columns.Add("MaHaiQuan", typeof(string));
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

        private DataTable CreateTableSaveMauSP()
        {
            try
            {
                DataTable tblmauspcr = new DataTable("tblmauspcr");
                tblmauspcr.Columns.Add("ID", typeof(int));
                tblmauspcr.Columns.Add("MaKH", typeof(string));
                tblmauspcr.Columns.Add("MaHang", typeof(string));
                tblmauspcr.Columns.Add("MaNhom", typeof(string));
                tblmauspcr.Columns.Add("MaVT", typeof(string));
                tblmauspcr.Columns.Add("ChiTiet", typeof(string));
                tblmauspcr.Columns.Add("MaDVVT", typeof(string));
                tblmauspcr.Columns.Add("KhoVaiID", typeof(string));
                tblmauspcr.Columns.Add("MaMauVT", typeof(string));
                tblmauspcr.Columns.Add("MauVT", typeof(string));
                tblmauspcr.Columns.Add("MaVTID", typeof(string));
                tblmauspcr.Columns.Add("MauVTID", typeof(string));
                tblmauspcr.Columns.Add("MaMau", typeof(string));
                tblmauspcr.Columns.Add("Status", typeof(bool));
                tblmauspcr.Columns.Add("IsAll", typeof(bool));
                tblmauspcr.Columns.Add("MaVTGhep", typeof(string));
                return tblmauspcr;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private DataTable CreateDataTableFromNhap()
        {
            try
            {
                DataTable _dataTable = new DataTable();
                _dataTable.Columns.Add("ID", typeof(int));
                _dataTable.Columns.Add("MaHang", typeof(string));
                _dataTable.Columns.Add("MaKH", typeof(string));
                _dataTable.Columns.Add("NPL", typeof(bool));
                _dataTable.Columns.Add("MaNhom", typeof(string));
                _dataTable.Columns.Add("TenNhom", typeof(string));
                _dataTable.Columns.Add("MaVTID", typeof(string));
                _dataTable.Columns.Add("MaVT", typeof(string));
                _dataTable.Columns.Add("ChiTiet", typeof(string));
                _dataTable.Columns.Add("MaDVVT", typeof(string));
                _dataTable.Columns.Add("MauVTID", typeof(string));
                _dataTable.Columns.Add("MaMauVT", typeof(string));
                _dataTable.Columns.Add("MauVT", typeof(string));
                _dataTable.Columns.Add("KhoVaiID", typeof(string));
                _dataTable.Columns.Add("MaMau", typeof(string));
                _dataTable.Columns.Add("TenMau", typeof(string));
                _dataTable.Columns.Add("GhiChu", typeof(string));
                _dataTable.Columns.Add("IsNew", typeof(int));
                _dataTable.Columns.Add("MaVTGhep", typeof(string));
                _dataTable.Columns.Add("urlAnh", typeof(string));
                _dataTable.Columns.Add("IsKV", typeof(bool));
                _dataTable.Columns.Add("MaTheSize", typeof(string));
                _dataTable.Columns.Add("IsEdit", typeof(int));
                return _dataTable;
            }
            catch (Exception ex)
            {
                return null;

            }

        }
        #endregion

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (searchLookUpEditNhom.EditValue == null || searchLookUpEditNhom.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Nhóm.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (txtMaVT.EditValue == null || txtMaVT.EditValue == "")
                {
                    MessageBox.Show("Vui lòng nhập Item Code.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtChiTiet.EditValue == null || txtChiTiet.EditValue == "")
                {
                    MessageBox.Show("Vui lòng nhập Vật tư.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (_isNPL)
                {

                    if (searckLookUpEditMauVT.EditValue == null || searckLookUpEditMauVT.EditValue.ToString() == "")
                    {
                        MessageBox.Show("Vui nhập màu vật tư.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (searchLookUpEditKV.Text.ToString() == "" || selectedRowsKhoVai.Count == 0)
                    {
                        MessageBox.Show("Vui chọn Khổ/Size cho nguyên liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                }
                if (searchLookUpEditDV.EditValue == null || searchLookUpEditDV.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn đơn vị.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (checkMaVTGhep()) return;

                string _manhom = searchLookUpEditNhom.EditValue.ToString();


                DataRow nplSelect = _tblNhom.AsEnumerable().FirstOrDefault(r => r["MaCLVT"].ToString() == _manhom);

                if (nplSelect == null) return;
                if (_dttable == null)
                {
                    _dttable = CreateTable();
                }

                if (nplSelect["NPL"].ToString().ToLower() == "true")
                {
                    tabVT.SelectedTabPage = tabNL;
                    addRow(gCNL);
                }
                else
                {
                    tabVT.SelectedTabPage = tabPL;
                    addRow(gCPL);
                }

            }
            catch (Exception ex)
            {


            }

        }
        private bool checkMaVTGhep()
        {//true là dừng lại, false là đi tiếp
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaVTGhep.Text.ToString()))
                    return false;

                string[] textMaVTGhep = txtMaVTGhep.Text.ToString().Trim().Split(';');
                DataTable tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return false;

                var hasDuplicate = textMaVTGhep.Any(ma =>
                     tbl.AsEnumerable().Any(r => r.Field<string>("MaVTGhep").Trim() == ma.Trim())
                 );
                if (!hasDuplicate)
                    return false;
                if (tblVatTuTVCheck == null || tblVatTuTVCheck.Rows.Count == 0) return false;
                if (string.IsNullOrWhiteSpace(txtMaVT.Text.ToString())) return false;
                if (string.IsNullOrWhiteSpace(txtChiTiet.Text.ToString())) return false;
                string _mavt = txtMaVT.Text.ToString();
                string _chitiet = txtChiTiet.Text.ToString();
                string _mamauvt = searckLookUpEditMauVT.Text.ToString();
                string _mauvt = txtMauVT.Text.ToString();
                var rowsMaVT = tblVatTuTVCheck.AsEnumerable().Where(r => r["MaVT"].ToString() == _mavt);
                if (rowsMaVT.Any())
                {
                    bool matchAll = rowsMaVT.Any(r => r["ChiTiet"].ToString() == _chitiet);
                    if (!matchAll)
                    {

                        if (string.IsNullOrWhiteSpace(txtTuDo.Text.ToString()))
                        {
                            var result = MessageBox.Show(
                                 "ItemCode này tương ứng với vật tư khác. Bạn có muốn tự sinh ra Mã vật tư không?",
                                 "Cảnh báo",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Warning
                             );

                            if (result == DialogResult.Yes)
                            {
                                string guid = Guid.NewGuid().ToString("N").ToUpper();
                                textMaVTGhep = txtMaVTGhep.Text.ToString().Trim().Split(';');
                                for (int i = 0; i < textMaVTGhep.Length; i++)
                                {
                                    textMaVTGhep[i] += "|" + guid.Substring(0, 6);
                                }
                                string textR = string.Join(";", textMaVTGhep);
                                txtMaVTGhep.Text = textR;
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            string[] textTuDo = txtTuDo.Text.ToString().Trim().Split(';');
                            textMaVTGhep = txtMaVTGhep.Text.ToString().Trim().Split(';');
                            if (textTuDo.Length < textMaVTGhep.Length)
                            {
                                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu vào ô (7).!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                return true;
                            }

                            for (int i = 0; i < textMaVTGhep.Length; i++)
                            {
                                textMaVTGhep[i] += "|" + textTuDo[i];
                                textMaVTGhep[i].Trim('|');
                            }
                            string textR = string.Join(";", textMaVTGhep);

                            txtMaVTGhep.Text = textR;
                            return false;
                        }
                    }
                    else
                    {
                        if (tblMauVTTVCheck == null || tblMauVTTVCheck.Rows.Count == 0) return false;
                        var rowsMaMauVT = tblMauVTTVCheck.AsEnumerable().Where(r => r["MaMauVT"].ToString() == _mamauvt);
                        if (rowsMaMauVT.Any())
                        {
                            bool matchAll1 = rowsMaMauVT.Any(r => r["MauVT"].ToString() == _mauvt);
                            if (matchAll1)
                            {

                                if (string.IsNullOrWhiteSpace(txtTuDo.Text.ToString()))
                                {
                                    var result = MessageBox.Show(
                                         "ItemCode này tương ứng với vật tư khác. Bạn có muốn tự sinh ra Mã vật tư không?",
                                         "Cảnh báo",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning
                                     );

                                    if (result == DialogResult.Yes)
                                    {
                                        string guid = Guid.NewGuid().ToString("N").ToUpper();
                                        textMaVTGhep = txtMaVTGhep.Text.ToString().Trim().Split(';');
                                        for (int i = 0; i < textMaVTGhep.Length; i++)
                                        {
                                            textMaVTGhep[i] += "|" + guid.Substring(0, 6);
                                        }
                                        string textR = string.Join(";", textMaVTGhep);
                                        txtMaVTGhep.Text = textR;
                                        return false;
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    string[] textTuDo = txtTuDo.Text.ToString().Trim().Split(';');
                                    textMaVTGhep = txtMaVTGhep.Text.ToString().Trim().Split(';');
                                    if (textTuDo.Length < textMaVTGhep.Length)
                                    {
                                        MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu vào ô (7).!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                        return true;
                                    }

                                    for (int i = 0; i < textMaVTGhep.Length; i++)
                                    {
                                        textMaVTGhep[i] += "|" + textTuDo[i];
                                        textMaVTGhep[i].Trim('|');
                                    }
                                    string textR = string.Join(";", textMaVTGhep);

                                    txtMaVTGhep.Text = textR;
                                    return false;
                                }
                            }

                        }
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                return true;
            }

        }
        private void addRow(GridControl grc)
        {
            try
            {
                Random _random = new Random();
                string _manhom = searchLookUpEditNhom.EditValue.ToString();
                DataRow drRowVS = _tblNhom.AsEnumerable().FirstOrDefault(r => r["MaCLVT"].ToString() == _manhom);
                int maxSTT = 0;

                if (drRowVS == null) return;

                string _txtmavt = txtMaVT.EditValue.ToString().Trim();
                string _txtchitiet = txtChiTiet.EditValue.ToString().Trim();

                //string _txtmauvt = txtMauVT.Text.ToString() == "" ? "" : txtMauVT.Text.ToString().Trim();
                string mauvtid = string.Empty;
                string mamauvt = string.Empty;
                string mauvt = string.Empty;
                var dataRowSearch = searckLookUpEditMauVT.Properties.View.GetFocusedDataRow();
                if (dataRowSearch != null)
                {
                    mauvtid = dataRowSearch["MauVTID"].ToString();
                    mamauvt = dataRowSearch["MaMauVT"].ToString();
                    mauvt = dataRowSearch["MauVT"].ToString();
                }
                else
                {
                    DataTable tblMauVT = searckLookUpEditMauVT.Properties.DataSource as DataTable;
                    DataRow row = tblMauVT
    .AsEnumerable()
    .FirstOrDefault(r =>
        (r["MaMauVT"] == DBNull.Value || r["MaMauVT"].ToString() == "") &&
        (r["MauVT"] == DBNull.Value || r["MauVT"].ToString() == "")
    );
                    mauvtid = row["MauVTID"].ToString();
                    mamauvt = row["MaMauVT"].ToString();
                    mauvt = row["MauVT"].ToString();
                }

                //string _txtmamauvt = searckLookUpEditMauVT.EditValue.ToString() == "" ? "" : searckLookUpEditMauVT.Text.ToString().Trim();
                DataTable tbl = grc.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0)
                    tbl = CreateDataTableFromNhap();
                if (!tbl.Columns.Contains("Sort"))
                {
                    tbl.Columns.Add("Sort", typeof(int));
                }


                //nếu cái vật tư đó mới thì thêm vào searchLookUpEdit của lưới bên dưới
                DataTable tbl2 = (_isNPL ? repo_MaVT : repo_MaVTPL).DataSource as DataTable;
                if (tbl2 != null)
                {
                    string filter = $"MaVT = '{_txtmavt}' AND ChiTiet = '{_txtchitiet}'";
                    DataRow[] rows = tbl2.Select(filter);
                    if (rows.Length <= 0)
                    {
                        DataRow dr = tbl2.NewRow();
                        dr["ID"] = 0;
                        dr["MaVTID"] = "";
                        dr["MaVT"] = _txtmavt;
                        dr["ChiTiet"] = _txtchitiet;
                        tbl2.Rows.Add(dr);
                    }
                }
                else
                {
                    var repo = (_isNPL ? repo_MaVT : repo_MaVTPL);
                    repo.DataSource = null;
                    //bảng null thì tạo bảng mới rồi gán cho searchLookUp
                    DataTable tblVTTV = new DataTable();
                    tblVTTV.Columns.Add("ID", typeof(int));
                    tblVTTV.Columns.Add("MaVTID", typeof(string));
                    tblVTTV.Columns.Add("MaVT", typeof(string));
                    tblVTTV.Columns.Add("ChiTiet", typeof(string));

                    DataRow dr = tblVTTV.NewRow();
                    dr["ID"] = 0;
                    dr["MaVTID"] = "";
                    dr["MaVT"] = _txtmavt;
                    dr["ChiTiet"] = _txtchitiet;
                    tblVTTV.Rows.Add(dr);

                    repo.DisplayMember = "MaVT";
                    repo.ValueMember = "MaVT";
                    repo.DataSource = tblVTTV;


                }
                if (!tbl.Columns.Contains("NPL"))
                {
                    tbl.Columns.Add("NPL", typeof(bool));
                }
                if (!tbl.Columns.Contains("STT"))
                {
                    tbl.Columns.Add("STT", typeof(int));
                }
                if (selectedRowsKhoVai.Count > 0)
                {
                    int index = 0;
                    string[] _mavtghep = txtMaVTGhep.Text.Split(';');
                    string text7 = txtTuDo.Text;
                    foreach (DataRow rowSource in selectedRowsKhoVai)
                    {
                        DataRow row = tbl.NewRow();
                        row["ID"] = _random.Next(-9999999, -999999);
                        row["MaKH"] = "";
                        row["VatTuCoSan"] = "Vật tư mới";
                        row["NPL"] = _isNPL;
                        row["MaNhom"] = searchLookUpEditNhom.EditValue.ToString();
                        row["TenNhom"] = searchLookUpEditNhom.Text.ToString();
                        row["MaVTID"] = DBNull.Value;
                        row["MaVT"] = _txtmavt;
                        row["ChiTiet"] = _txtchitiet;
                        row["MaDVVT"] = searchLookUpEditDV.EditValue == null ? "" : searchLookUpEditDV.EditValue.ToString();
                        row["MauVTID"] = mauvtid;
                        row["MaMauVT"] = mamauvt;
                        row["MauVT"] = mauvt;
                        //DataRow emptyKhoVaiRow = null;
                        //if (_tblKhoVai != null && _tblKhoVai.Rows.Count > 0)
                        //{
                        //    emptyKhoVaiRow = _tblKhoVai.AsEnumerable()
                        //     .Where(row1 => row1.Field<string>("KhoVai") == "" && Convert.ToBoolean(row1["NPL"].ToString()) == _isNPL)

                        //     .FirstOrDefault();
                        //}
                        //if (emptyKhoVaiRow != null)
                        //{
                        //    row["KhoVaiID"] = emptyKhoVaiRow["KhoVaiID"];
                        //    row["MaTheSize"] = emptyKhoVaiRow["MaNhom"];
                        //    row["IsKV"] = emptyKhoVaiRow["IsKV"].ToString() == "1" ? true : false;
                        //}

                        //else
                        //{
                        row["KhoVaiID"] = rowSource["KhoVaiID"];
                        row["MaTheSize"] = rowSource["MaNhom"];
                        row["IsKV"] = rowSource["IsKV"].ToString() == "1" ? true : false;
                        //}
                        row["Sort"] = drRowVS["Sort"];
                        row["GhiChu"] = txtGhiChu.Text;
                        row["IsNew"] = 1;
                        row["IsEdit"] = 1;
                        row["MaVTGhep"] = _mavtghep[index].Trim() + "|" + text7;
                        if (picVT.Image != null)
                        {
                            var token = $"{Guid.NewGuid():N}.png";
                            inMemoryImages[token] = new Bitmap(picVT.Image);
                            row["urlAnh"] = token;
                        }

                        tbl.Rows.Add(row);
                        index++;

                    }
                }


                else
                {
                    string _mavtghep = txtMaVTGhep.Text.ToString();
                    string text7 = txtTuDo.Text;
                    DataRow row = tbl.NewRow();
                    row["ID"] = _random.Next(-9999999, -999999);

                    row["MaKH"] = "";
                    row["VatTuCoSan"] = "Vật tư mới";
                    row["NPL"] = _isNPL;
                    row["MaNhom"] = searchLookUpEditNhom.EditValue.ToString();
                    row["TenNhom"] = searchLookUpEditNhom.Text.ToString();
                    row["MaVTID"] = DBNull.Value;
                    row["MaVT"] = _txtmavt;
                    row["ChiTiet"] = _txtchitiet;
                    row["MaDVVT"] = searchLookUpEditDV.EditValue == null ? "" : searchLookUpEditDV.EditValue.ToString();
                    row["MauVTID"] = mauvtid;
                    row["MaMauVT"] = mamauvt;
                    row["MauVT"] = mauvt;
                    var emptyKhoVaiIds = "";
                    if (_tblKhoVai != null && _tblKhoVai.Rows.Count > 0)
                    {
                        emptyKhoVaiIds = _tblKhoVai.AsEnumerable()
                        .Where(row1 => row1.Field<string>("KhoVai") == "" && searchLookUpEditNhom.EditValue.ToString().Trim() == row1["MaNhom"].ToString().Trim())
                         .Select(row1 => row1.Field<string>("KhoVaiID"))
                         .FirstOrDefault();
                    }
                    if (string.IsNullOrWhiteSpace(emptyKhoVaiIds))
                    {
                        MessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    row["KhoVaiID"] = emptyKhoVaiIds;
                    row["MaTheSize"] = "";
                    row["IsKV"] = true;
                    row["Sort"] = drRowVS["Sort"];
                    row["GhiChu"] = txtGhiChu.Text;
                    row["MaVTGhep"] = _mavtghep + "|" + text7;
                    row["IsNew"] = 1;
                    row["IsEdit"] = 1;
                    if (picVT.Image != null)
                    {
                        var token = $"{Guid.NewGuid():N}.png";
                        inMemoryImages[token] = new Bitmap(picVT.Image);
                        row["urlAnh"] = token;
                    }
                    tbl.Rows.Add(row);
                }
                picVT.Image = null;
                grc.DataSource = tbl;
                searckLookUpEditMauVT.Text = "";
                txtMauVT.Text = "";
                txtGhiChu.Text = "";
                selectedRowsKhoVai.Clear();
                searchLookUpEditViewKV.ClearSelection();
                searchLookUpEditKV.EditValue = "";
                searchLookUpEditKV.Text = "";
                DataTable dataSource1 = grc.DataSource as DataTable;


            }
            catch (Exception ex)
            {


            }

        }

        private void RefreshSTTColumn(DataTable dt)
        {
            gCNL.BeginUpdate();
            gCPL.BeginUpdate();
            try
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

            finally
            {

                gCNL.EndUpdate();
                gCPL.EndUpdate();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {

                LuuVatTu();
                loadTimKiem();
                loadVatTuCheck();
            }
            catch (Exception ex)
            {

            }

        }
        private void LuuVatTu()
        {
            try
            {


                tblmvt = CreateTableSaveMauVT();

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

                if (tblgc == null || tblgc.Rows.Count == 0) return;
                if (checkTrungVT(tblgc))
                {
                    return;
                }
                DataTable filtered = tblgc.AsEnumerable()
                   .Where(r => !r.IsNull("IsEdit") && Convert.ToInt32(r["IsEdit"]) == 1)
                   .CopyToDataTable();
                foreach (DataRow item in filtered.Rows)
                {
                    string _textNPL = item["NPL"].ToString().ToLower() != "false" ? "Nguyên liệu" : "Phụ liệu";
                    if (item["MaVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn ItemCode ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (item["ChiTiet"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn ItemCode, Vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (item["NPL"].ToString().ToLower() != "false" && item["MauVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn màu vật tư ở " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (item["MaDVVT"].ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn đơn vị " + $"dòng {item["STT"].ToString()}" + " phần " + _textNPL + ". Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                tblgc.Clear();
                tblgc.Merge(filtered);



                //POSTVATTUTV t1
                string khachhang = "";
                string mahang = "";
                DataTable tblthongsovt = CreateTableSaveThongSo();
                foreach (DataRow row in tblgc.Rows)
                {
                    DataRow rowthongso = tblthongsovt.NewRow();

                    rowthongso["ID"] = row["ID"];
                    rowthongso["MaHang"] = mahang;
                    rowthongso["MaKH"] = khachhang;
                    rowthongso["MaNhom"] = row["MaNhom"].ToString();
                    rowthongso["MaVTID"] = row["MaVTID"].ToString();
                    rowthongso["MaVT"] = row["MaVT"].ToString();
                    rowthongso["ChiTiet"] = row["ChiTiet"].ToString();
                    rowthongso["MaDVVT"] = row["MaDVVT"].ToString();
                    rowthongso["MauVTID"] = row["MauVTID"].ToString();
                    rowthongso["MaMauVT"] = row["MaMauVT"].ToString();
                    rowthongso["KhoVaiID"] = row["KhoVaiID"].ToString();
                    rowthongso["MauVT"] = row["MauVT"].ToString();
                    rowthongso["GhiChu"] = row["GhiChu"].ToString();
                    rowthongso["MaVTGhep"] = row["MaVTGhep"].ToString();

                    rowthongso["DungSai"] = row["DungSai"];
                    rowthongso["TrongLuongChuan"] = row["TrongLuongChuan"];
                    rowthongso["NgayBC"] = row["NgayBC"];
                    rowthongso["NgayHetHan"] = row["NgayHetHan"];

                    tblthongsovt.Rows.Add(rowthongso);

                }
                string url1 = $"{URL}ERPVatTuBOM/PostT1?Action=POSTVATTUTV&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult1 = Task.Run(async () => { return await _clientExtension.PostAsync(url1, tblthongsovt); }).Result;
                if (msResult1.ToLower() != "true") return;
                tblthongsovt.Clear();
                if (!tblthongsovt.Columns.Contains("MaMau"))
                {
                    tblthongsovt.Columns.Add("MaMau", typeof(string));
                }

                foreach (DataRow row in tblgc.Rows)
                {


                    DataRow rowthongso = tblthongsovt.NewRow();
                    rowthongso["ID"] = row["ID"];
                    rowthongso["MaHang"] = mahang;
                    rowthongso["MaKH"] = khachhang;
                    rowthongso["MaNhom"] = row["MaNhom"].ToString();
                    rowthongso["MaVTID"] = row["MaVTID"].ToString();
                    rowthongso["MaVT"] = row["MaVT"].ToString();
                    rowthongso["ChiTiet"] = row["ChiTiet"].ToString();
                    rowthongso["MaDVVT"] = row["MaDVVT"].ToString();
                    //rowthongso["TenDVVT"] = row["TenDVVT"].ToString();
                    rowthongso["MauVTID"] = row["MauVTID"].ToString();
                    rowthongso["MaMauVT"] = row["MaMauVT"].ToString();
                    rowthongso["KhoVaiID"] = row["KhoVaiID"].ToString();
                    //rowthongso["KhoVai"] = row["KhoVai"].ToString();
                    rowthongso["MauVT"] = row["MauVT"].ToString();
                    rowthongso["MaMau"] = "";
                    rowthongso["GhiChu"] = row["GhiChu"].ToString();
                    rowthongso["MaVTGhep"] = row["MaVTGhep"].ToString();
                    rowthongso["urlAnh"] = row["urlAnh"].ToString();
                    rowthongso["MaTheSize"] = row["MaTheSize"].ToString();
                    rowthongso["IsKV"] = row["IsKV"];
                    rowthongso["DungSai"] = row["DungSai"];
                    rowthongso["TrongLuongChuan"] = row["TrongLuongChuan"];
                    rowthongso["NgayBC"] = row["NgayBC"];
                    rowthongso["NgayHetHan"] = row["NgayHetHan"];
                    rowthongso["MaDVVTSD"] = row["MaDVVTSD"];
                    rowthongso["Tile"] = row["Tile"];
                    rowthongso["MaHaiQuan"] = row["MaHaiQuan"];
                    tblthongsovt.Rows.Add(rowthongso);

                }
                //chuẩn bị dữ liệu để Post
                //foreach (DataRow resultRow in tblthongsovt.Rows)
                //{

                //    DataRow rowmvt = tblmvt.NewRow();
                //    rowmvt["ID"] = 0;
                //    rowmvt["MaHang"] = mahang;
                //    rowmvt["MaKH"] = khachhang;
                //    rowmvt["MaNhom"] = resultRow["MaNhom"];
                //    rowmvt["MaVT"] = resultRow["MaVT"];
                //    rowmvt["ChiTiet"] = resultRow["ChiTiet"];
                //    rowmvt["MaDVVT"] = resultRow["MaDVVT"];
                //    rowmvt["KhoVaiID"] = resultRow["KhoVaiID"];
                //    rowmvt["MaVTID"] = resultRow["MaVTID"];
                //    rowmvt["MauVTID"] = resultRow["MauVTID"];
                //    rowmvt["MaMauVT"] = resultRow["MaMauVT"];
                //    rowmvt["MauVT"] = resultRow["MauVT"];
                //    rowmvt["MaVTGhep"] = resultRow["MaVTGhep"].ToString();

                //    tblmvt.Rows.Add(rowmvt);



                //}

                //POSTMAUVTTV t2
                //string url2 = $"{URL}ERPVatTuBOM/PostT2?Action=POSTMAUVTTV&para={GlobleData.UserName}&para2=frmERP_VatTu";
                //string msResult2 = Task.Run(async () => { return await _clientExtension.PostAsync(url2, tblmvt); }).Result;
                //if (msResult2.ToLower() != "true") return;

                if (tblthongsovt.Columns.Contains("MaMau"))
                {
                    tblthongsovt.Columns.Remove("MaMau");
                }
                PersistRuntimeImages(tblgc);
                string url4 = $"{URL}ERPVatTuBOM/PostT1?Action=POSTTHONGSO&para={GlobleData.UserName}&para2=frmERP_VatTu";
                string msResult4 = Task.Run(async () => { return await _clientExtension.PostAsync(url4, tblthongsovt); }).Result;
                if (msResult4.ToLower() != "true") return;

                clsWaitForm.ShowSuccessForm(this, 3000);
                CreateSearchLookUpDVVT();
                CreateSearchLookUpKHO();

                loadSearchLookUpMaMauVT();
                loadSearchLookUpMaVT();

                loadVatTu();
                _isSaveNL = true;
                _isSavePL = true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("");
            }
        }
        private async Task PersistRuntimeImages(DataTable table)
        {
            if (table == null || table.Rows.Count == 0) return;

            string apiUrl = $"{Host}/api/ERPThuVienVT/UpLoadImg";
            string libFolder = "VatTu";
            List<dynamic> imageListNL = new List<dynamic>();
            List<dynamic> imageListPL = new List<dynamic>();

            try
            {
                // Duyệt qua table đã filter
                foreach (DataRow row in table.Rows)
                {
                    string id = row["ID"].ToString();
                    bool isNPL = row["NPL"].ToString().ToLower() != "false";

                    if (isNPL && idToIndexMapNL.TryGetValue(id, out int index))
                    {
                        if (imageCache.TryGetValue(index, out Image img))
                        {
                            string base64String = ImageToBase64String(img, ImageFormat.Png);
                            string imgName = row["urlAnh"].ToString();

                            imageListNL.Add(new
                            {
                                img = $"data:image/png;base64,{base64String}",
                                name = imgName,
                                ID = id
                            });
                        }
                    }
                    else if (!isNPL && idToIndexMapPL.TryGetValue(id, out int indexPL))
                    {
                        if (imageCachePL.TryGetValue(indexPL, out Image imgPL))
                        {
                            string base64String = ImageToBase64String(imgPL, ImageFormat.Png);
                            string imgName = row["urlAnh"].ToString();

                            imageListPL.Add(new
                            {
                                img = $"data:image/png;base64,{base64String}",
                                name = imgName,
                                ID = id
                            });
                        }
                    }
                }

                if (imageListNL.Count > 0)
                {
                    string getFileName = "1";
                    JArray imageDatas = JArray.FromObject(imageListNL);
                    var response = await UploadImagesToApi(apiUrl, imageDatas,
                        ReplaceSpecialCharacters(RemoveVietnameseTone(getFileName)), libFolder);
                }

                if (imageListPL.Count > 0)
                {
                    string getFileName = "1";
                    JArray imageDatas = JArray.FromObject(imageListPL);
                    var response = await UploadImagesToApi(apiUrl, imageDatas,
                        ReplaceSpecialCharacters(RemoveVietnameseTone(getFileName)), libFolder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ImageToBase64String(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private async Task<List<dynamic>> UploadImagesToApi(string apiUrl, JArray imageDatas, string getFileName, string libFolder)
        {
            using (var client = new HttpClient())
            {
                var queryParams = $"?getFileName={Uri.EscapeDataString(getFileName)}&LibFolder={Uri.EscapeDataString(libFolder)}";
                string fullUrl = apiUrl + queryParams;

                string jsonBody = imageDatas.ToString(Formatting.None);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(fullUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<dynamic>>(responseJson);
                }
                return null;
            }
        }
        private bool checkTrungVT(DataTable tbl)
        {
            try
            {
                if (tbl == null || tbl.Rows.Count == 0) return false;


                var keyRowGroups = new Dictionary<string, List<DataRow>>();

                // Duyệt qua từng dòng để gom nhóm theo key
                foreach (DataRow row in tbl.Rows)
                {
                    string key = string.Join("|",

    row["MaNhom"].ToString(),
    row["MaVT"].ToString(),
    row["ChiTiet"].ToString(),
    row["MaVTID"].ToString(),
    row["MaMauVT"].ToString(),
    row["MauVT"].ToString(),
    row["MauVTID"].ToString(),
    row["KhoVaiID"].ToString()
);

                    if (!keyRowGroups.ContainsKey(key))
                    {
                        keyRowGroups[key] = new List<DataRow>();
                    }
                    keyRowGroups[key].Add(row);
                }
                var duplicateGroups = keyRowGroups.Where(g => g.Value.Count > 1).ToList();

                if (duplicateGroups.Count == 0)
                {
                    return false;
                }
                StringBuilder errorMessage = new StringBuilder("Phát hiện các dòng dữ liệu bị trùng lặp:\n\n");
                foreach (var group in duplicateGroups)
                {
                    foreach (DataRow row in group.Value)
                    {
                        // Đánh dấu dòng là trùng lặp
                        row["IsNew"] = 2;

                        string tenNhom = row.Table.Columns.Contains("TenNhom") ? row["TenNhom"]?.ToString() : row["MaNhom"]?.ToString();
                        string khoVai = row.Table.Columns.Contains("KhoVai") ? row["KhoVai"]?.ToString() : row["KhoVaiID"]?.ToString();

                        errorMessage.AppendLine($"- Nhóm: {tenNhom}, ItemCode: {row["MaVT"]}, Vật tư: {row["ChiTiet"]}, Màu: {row["MauVT"]}, Khổ/Size: {khoVai}");

                    }
                    errorMessage.AppendLine("----------------------------------------------------------------");

                }

                XtraMessageBox.Show(errorMessage.ToString(), "Thông Báo Lỗi Trùng Lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            catch (Exception ex)
            {
                return true;

            }

        }
        private static string ExtractAlphaNumericAndSpecial(object value)
        {
            if (value == null)
                return string.Empty;

            return value
                .ToString()
                .ToUpperInvariant()
                .Replace(" ", string.Empty);
        }


        private void gVNL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                dr["IsEdit"] = 1;
                //setMaVTGhepChanged(sender, e);
                string[] relatedFields = new[] { "MaNhom", "MaVT", "ChiTiet", "MaMauVT", "KhoVaiID" }; // không có makh; không có manhom, manhom xử lý riêng

                if (relatedFields.Contains(e.Column.FieldName))
                {

                    string maVT = dr["MaVT"].ToString();
                    string ChiTiet = dr["ChiTiet"].ToString();
                    string mamauvt = dr["MaMauVT"].ToString();
                    string mauvt = dr["MauVT"].ToString();
                    UpdateMaVTGhep_ForFocusedRow(dr);
                    checkMaVTGhepCellValueChanged(e.Column.FieldName, dr);


                }

                _isSaveNL = false;
            }
            catch (Exception ex)
            {
            }
        }

        private void gVPL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                dr["IsEdit"] = 1;
                //setMaVTGhepChanged(sender, e);
                string[] relatedFields = new[] { "MaNhom", "MaVT", "ChiTiet", "MaMauVT", "KhoVaiID" }; // không có makh; không có manhom, manhom xử lý riêng

                if (relatedFields.Contains(e.Column.FieldName))
                {

                    string maVT = dr["MaVT"].ToString();
                    string ChiTiet = dr["ChiTiet"].ToString();
                    string mamauvt = dr["MaMauVT"].ToString();
                    string mauvt = dr["MauVT"].ToString();
                    UpdateMaVTGhep_ForFocusedRow(dr);
                    checkMaVTGhepCellValueChanged(e.Column.FieldName, dr);
                }
                _isSavePL = false;
            }
            catch (Exception ex)
            {


            }

        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {

                if (searchLookUpEditNhom.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Nhóm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (_dttable == null)
                {
                    _dttable = CreateTable();
                }
                string _manhom = searchLookUpEditNhom.EditValue.ToString();


                DataRow nplSelect = _tblNhom.AsEnumerable().FirstOrDefault(r => r["MaCLVT"].ToString() == _manhom);

                if (nplSelect == null) return;



                if (nplSelect["NPL"].ToString().ToLower() == "true")
                {
                    tabVT.SelectedTabPage = tabNL;
                    AddRowNew(gCNL, true);
                }
                else
                {
                    tabVT.SelectedTabPage = tabPL;
                    AddRowNew(gCPL, false);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void AddRowNew(GridControl grc, bool checkNL)
        {
            try
            {
                string _manhom = searchLookUpEditNhom.EditValue.ToString();
                DataRow drRowVS = _tblNhom.AsEnumerable().FirstOrDefault(r => r["MaCLVT"].ToString() == _manhom);

                if (drRowVS == null) return;
                DataTable tbl = grc.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    tbl = _dttable.Clone();
                }
                DataRow newRow = tbl.NewRow();
                newRow["ID"] = 0;
                newRow["MaKH"] = "";
                newRow["MaHang"] = "";
                newRow["MaNhom"] = searchLookUpEditNhom.EditValue.ToString();
                newRow["TenNhom"] = searchLookUpEditNhom.Text.ToString();
                newRow["Sort"] = drRowVS["Sort"];
                newRow["IsNew"] = 1;
                tbl.Rows.Add(newRow);
                //RefreshSTTColumn(tbl);
                grc.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void gVNL_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0) // Đảm bảo không áp dụng cho dòng nhóm hoặc dòng new row
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                    int isNew = 0;
                    if (isNewValue != null && int.TryParse(isNewValue.ToString(), out isNew))
                    {
                        if (isNew == 1)
                        {
                            e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 251, 204); // Vàng nhạt pastel
                        }
                        if (isNew == 2)
                        {
                            e.Appearance.BackColor = System.Drawing.Color.FromArgb(204, 255, 229);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void gVPL_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0) // Đảm bảo không áp dụng cho dòng nhóm hoặc dòng new row
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                    int isNew = 0;
                    if (isNewValue != null && int.TryParse(isNewValue.ToString(), out isNew))
                    {
                        if (isNew == 1)
                        {
                            e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 251, 204); // Vàng nhạt pastel
                        }
                        if (isNew == 2)
                        {
                            e.Appearance.BackColor = System.Drawing.Color.FromArgb(204, 255, 229);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                searchLookUpEditNhom.EditValue = null;
                loadTimKiem();
                loadSearchLookUpMaVT();
                loadSearchLookUpMaMauVT();
                string _kh = string.Empty, _mh = string.Empty, _manhom = string.Empty;

                CreateSearchLookUpNhom();
                searchLookUpEditNhom.EditValue = _nhom;


                CreateSearchLookUpKhoVai();
                CreateSearchLookUpKHO();
                selectedRowsKhoVai.Clear();
                loadVatTu();
            }
            catch (Exception ex)
            {


            }


        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                searchLookUpEditTimVT.EditValue = null;
                txtMaVT.Text = "";
                txtChiTiet.Text = "";
                searchLookUpEditDV.EditValue = null;
                searchLookUpEditKV.EditValue = null;
                searckLookUpEditMauVT.Text = "";
                txtMauVT.Text = "";
                selectedRowsKhoVai.Clear();
                searchLookUpEditKV.EditValue = null;
                searchLookUpEditViewKV.ClearSelection();
                MaVTGhep["ItemCode"] = "00";
                MaVTGhep["MauVT"] = "00";
                MaVTGhep["KhoSize"] = "00";
                var result = BuildDynamicString(templateMaVTGhep, MaVTGhep);
                txtMaVTGhep.Text = string.Join(" ; ", result);

            }
            catch (Exception ex)
            {


            }

        }

        private void gVPL_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DataRow drrow = gVPL.GetFocusedDataRow();
                //bool CheckVT = CheckCanDoi(drrow);
                //if (CheckVT)
                //{
                //    MessageBox.Show("Vật tư này đã được tạo BOM.Không thể sửa vật tư.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    e.Cancel = true;
                //};


                if (!_allowEdit)
                {
                    e.Cancel = true;
                    return;
                }

                if (gVPL.FocusedColumn != null && gVPL.FocusedColumn.FieldName == "MaDVVT" || gVPL.FocusedColumn.FieldName == "KhoVaiID")
                {
                    gCPL.BeginInvoke(new Action(() =>
                    {
                        if (gVPL.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                        {
                            if (!searchLookUpEdit.IsPopupOpen)
                            {
                                searchLookUpEdit.ShowPopup();
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
            }

        }
        private void gVNL_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DataRow drrow = gVNL.GetFocusedDataRow();
                //bool CheckVT = CheckCanDoi(drrow);
                //if (CheckVT)
                //{
                //    MessageBox.Show("Vật tư này đã được nhập định mức.Không thể sửa vật tư.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    e.Cancel = true;
                //};
                if (!_allowEdit)
                {
                    e.Cancel = true;
                    return;
                }
                if (gVNL.FocusedColumn != null && gVNL.FocusedColumn.FieldName == "MaDVVT" || gVNL.FocusedColumn.FieldName == "KhoVaiID")
                {
                    gCNL.BeginInvoke(new Action(() =>
                    {
                        if (gVNL.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                        {
                            if (!searchLookUpEdit.IsPopupOpen)
                            {
                                searchLookUpEdit.ShowPopup();
                            }
                        }
                    }));
                }

            }
            catch (Exception ex)
            {


            }


        }
        private bool CheckCanDoi(DataRow dr)
        {
            try
            {
                if (dr == null) return true;
                string urlCT = $"{URL}ERPVatTuBOM/GetChung?Action=GETCHUNG&para={dr["MaNhom"].ToString()}&para1={dr["MaVTID"].ToString()}&para2={dr["MauVTID"].ToString()}&para3={dr["KhoVaiID"].ToString()}";
                string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;

                if (jsonCT == "[]") return false;
                else return true;
            }
            catch (Exception ex)
            {
                return true;

            }
        }
        bool checkPopupMenu = true;
        private void gVNL_PopupMenuShowing_1(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                checkPopupMenu = true;
                CopyPast(e, gVNL);
            }
            catch (Exception ex)
            {


            }


        }
        private void gVPL_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                checkPopupMenu = false;
                CopyPast(e, gVPL);
            }
            catch (Exception ex)
            {


            }

        }
        private void CopyPast(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e, GridView grv)
        {
            try
            {
                DataRow drrow = grv.GetFocusedDataRow();
                bool CheckVT = CheckCanDoi(drrow);

                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (grv.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {
                            if (_allowAdd)
                            {
                                DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", ItemCoppyPaste_Click1);
                                e.Menu.Items.Add(menuCopyItem);
                            }

                            DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Copy ", ItemCopy_Click1);
                            e.Menu.Items.Add(menuCopyMauItem);

                            DevExpress.Utils.Menu.DXMenuItem menuPasteMauItem = new DevExpress.Utils.Menu.DXMenuItem("Paste ", ItemPaste_Click1);
                            e.Menu.Items.Add(menuPasteMauItem);

                            if (!string.IsNullOrEmpty(drrow["OekotexPDF"].ToString()))
                            {

                                DevExpress.Utils.Menu.DXMenuItem menuOekotexPDFItem = new DevExpress.Utils.Menu.DXMenuItem("Xem Oekotex", xemOekotexPDF);
                                e.Menu.Items.Add(menuOekotexPDFItem);

                            }

                            if (!string.IsNullOrEmpty(drrow["TestReportPDF"].ToString()))
                            {

                                DevExpress.Utils.Menu.DXMenuItem menuTestReportPDFItem = new DevExpress.Utils.Menu.DXMenuItem("Xem Test Report", xemTestReportPDF);
                                e.Menu.Items.Add(menuTestReportPDFItem);

                            }
                            if (!string.IsNullOrEmpty(drrow["SpecificationPDF"].ToString()))
                            {

                                DevExpress.Utils.Menu.DXMenuItem menuSpecificationPDFItem = new DevExpress.Utils.Menu.DXMenuItem("Xem Specification", xemSpecificationPDF);
                                e.Menu.Items.Add(menuSpecificationPDFItem);

                            }


                            if (!CheckVT)
                            {


                                if (_allowDelete)
                                {
                                    DevExpress.Utils.Menu.DXMenuItem menuDeleteMauItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa Dòng", ItemDelete_Click1);
                                    e.Menu.Items.Add(menuDeleteMauItem);
                                }

                                //if (_allowEdit)
                                //{
                                //    DevExpress.Utils.Menu.DXMenuItem menuDoiNhomItem = new DevExpress.Utils.Menu.DXMenuItem("Đổi nhóm", doiNhom_Click);
                                //    e.Menu.Items.Add(menuDoiNhomItem);
                                //}


                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void ItemCoppyPaste_Click1(object sender, EventArgs e)
        {
            try
            {
                string guid = Guid.NewGuid().ToString("N").ToUpper();



                DataTable tbl = checkPopupMenu ? gCNL.DataSource as DataTable : gCPL.DataSource as DataTable;

                DataRow drRow = checkPopupMenu ? gVNL.GetFocusedDataRow() : gVPL.GetFocusedDataRow();
                if (drRow == null) return;
                DataRow DrRowNew = tbl.NewRow();
                DrRowNew.ItemArray = (object[])drRow.ItemArray.Clone();
                DrRowNew["ID"] = 0;
                //DrRowNew["MaVTID"] = "";
                //DrRowNew["MauVTID"] = "";
                DrRowNew["MaVTGhep"] += "|" + guid.Substring(0, 6);
                DrRowNew["MaVTGhep"] = DrRowNew["MaVTGhep"].ToString().Trim('|');
                //DrRowNew["MaMauVT"] = "";
                //DrRowNew["MauVT"] = "";
                DrRowNew["IsNew"] = true;
                DrRowNew["IsEdit"] = 1;
                tbl.Rows.Add(DrRowNew);
                //RefreshSTTColumn(tbl);
            }
            catch (Exception ex)
            {


            }

        }
        private void ItemDelete_Click1(object sender, EventArgs e)
        {
            XoaDong();
        }
        private void doiNhom_Click(object sender, EventArgs e)
        {
            doiNhom();
        }
        private void doiNhom()
        {
            try
            {
                int[] selectedRowHandles = (checkPopupMenu ? gVNL : gVPL).GetSelectedRows();
                if (selectedRowHandles.Length == 0) return;

                List<DataRow> rowsFromTbl = new List<DataRow>();
                foreach (int rowHandle in selectedRowHandles)
                {
                    if (rowHandle < 0) continue;
                    if (!(checkPopupMenu ? gVNL : gVPL).IsValidRowHandle(rowHandle)) continue;
                    DataRow drSL = (checkPopupMenu ? gVNL : gVPL).GetDataRow(rowHandle);
                    if (drSL != null)
                        rowsFromTbl.Add(drSL);
                }



                DataRow dr = (checkPopupMenu ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string manhonm = dr["MaNhom"].ToString();
                string tennhom = dr["TenNhom"].ToString();
                string NPL = dr["NPL"].ToString().ToLower() == "true" ? "1" : "0";


                frmERP_VatTuDoiNhom frm = new frmERP_VatTuDoiNhom(rowsFromTbl, (_isNPL ? tblCNL : tblCPL), manhonm, tennhom, NPL);

                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;
                DataTable tbl = CreateTableSaveThongSo();
                string _manhom = frm.getMaNhom();
                string _tennhom = frm.getTenNhom();
                int _sortNhom = frm.getSortNhom();

                foreach (DataRow _r in rowsFromTbl)
                {
                    _r["MaNhom"] = _manhom;
                    _r["TenNhom"] = _tennhom;
                    _r["Sort"] = _sortNhom;


                }
                UpdateMaVTGhep_ForFocusedRow(dr, rowsFromTbl);

                foreach (DataRow _r in rowsFromTbl)
                {


                    tbl.ImportRow(_r);
                }
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    (checkPopupMenu ? gVNL : gVPL).RefreshData();
                    gCNL.DataSource = tblCNL;
                    gCPL.DataSource = tblCPL;
                    return;
                }

                string url = $"{URL}ERPVatTuBOM/PostUpdateNhom?para={GlobleData.UserName}";
                string ms = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (ms.ToLower() == "true")
                {
                    //loadVatTu();
                    //clsWaitForm.ShowSuccessForm(this, 2000);
                    loadTimKiem();
                }
                UpdateMaVTGhep_ForFocusedRow(dr, rowsFromTbl);

                (checkPopupMenu ? gVNL : gVPL).RefreshData();
                DataTable _tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                //RefreshSTTColumn(_tbl);

                (_isNPL ? gCNL : gCPL).DataSource = _tbl;


            }
            catch (Exception ex)
            {
            }
        }
        private async void XoaDong()
        {
            try
            {
               

                DataTable tbl = checkPopupMenu ? gCNL.DataSource as DataTable : gCPL.DataSource as DataTable;
                DataRow row = checkPopupMenu ? gVNL.GetFocusedDataRow() : gVPL.GetFocusedDataRow();
                if (row == null) return;

              
                if (row["ID"] != "0")
                {

                    DialogResult messResult = MessageBox.Show(
                "Xóa dòng này sẽ mất các vật tư thành phần và định ở trong sẽ xóa theo.Bạn có xóa không?",
                "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                    if (messResult != DialogResult.Yes) return;

                    string url = string.Format("{0}?id={1}&makh={2}&mahang={3}&mauvtid={4}", URL + "ERPVatTuBOM/Delete", row["ID"], row["MaKH"], GlobleData.UserName, row["MauVTID"]);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    ////if (result.ToLower() == "true")
                    ////    clsWaitForm.ShowSuccessForm(this, 1000);
                }
                tbl.Rows.Remove(row);
                //RefreshSTTColumn(tbl);

            }
            catch (Exception ex)
            {

            }

        }
        private void ItemCopy_Click1(object sender, EventArgs e)
        {
            try
            {
                GridView view = (checkPopupMenu ? gCNL.MainView : gCPL.MainView) as GridView;
                if (view == null) return;
                view.CopyToClipboard();
            }
            catch (Exception ex)
            {


            }

        }
        private void Paste()
        {
            try
            {
                GridView view = (checkPopupMenu ? gCNL.MainView : gCPL.MainView) as GridView;
                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 0) return;
                string checkDataRow = "Mã vật tư\tChi tiết\tĐơn vị\tKhổ/Size\tMã màu vật tư\tMàu vật tư\tMàu";
                if (checkDataRow.Contains(data[0]))
                {
                    data = data.Skip(1).ToArray();
                }
                int startRow = view.FocusedRowHandle;
                foreach (string row in data)
                {
                    AddRow(row, startRow++, view);
                    if (!view.IsValidRowHandle(startRow)) break;
                }

            }
            catch (Exception ex)
            {
            }
        }
        private void ItemPaste_Click1(object sender, EventArgs e)
        {
            Paste();
        }
        private void gCNL_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                checkPopupMenu = true;
                Keypass(e, gCNL);
            }
            catch (Exception ex)
            {


            }

        }
        private void gCPL_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                checkPopupMenu = false;
                Keypass(e, gCPL);
            }
            catch (Exception ex)
            {


            }

        }
        private void Keypass(KeyEventArgs e, GridControl grc)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                Paste();
            }

        }
        private void AddRow(string data, int rowHandle, GridView view)
        {
            try
            {
                if (string.IsNullOrEmpty(data)) return;

                string[] rowData = data.Split('\t');
                int columnIndex = view.FocusedColumn.VisibleIndex;

                int originalRowHandle = view.GetDataSourceRowIndex(rowHandle); // Đảm bảo lấy dòng gốc khi merge

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= view.VisibleColumns.Count) break;

                    try
                    {
                        GridColumn targetColumn = view.VisibleColumns[columnIndex + i];
                        if (targetColumn.ToString() == "Mã vật tư" || targetColumn.ToString() == "Vật tư" || targetColumn.ToString() == "Màu vật tư" || targetColumn.ToString() == "Màu sản phẩm") continue;

                        Type fieldType = view.Columns[targetColumn.FieldName].ColumnType;

                        if (fieldType == typeof(int))
                        {
                            if (int.TryParse(rowData[i].Replace(",", ""), out int intValue))
                            {
                                view.SetRowCellValue(rowHandle, targetColumn, intValue);
                            }
                            else
                            {

                                this.ActiveControl = button1;
                            }
                        }
                        else if (fieldType == typeof(float))
                        {
                            if (float.TryParse(rowData[i].Replace(",", ""), out float floatValue))
                            {
                                view.SetRowCellValue(rowHandle, targetColumn, floatValue);
                            }
                            else
                            {

                                this.ActiveControl = button1;
                            }
                        }
                        else
                        {
                            if (targetColumn.ToString() == "Đơn vị")
                            {
                                DataTable tbl = searchLookUpEditDV.Properties.DataSource as DataTable;
                                tbl = tbl.AsEnumerable().Where(x => x["TenDVVT"].ToString() == rowData[i]).CopyToDataTable();

                                view.SetRowCellValue(rowHandle, targetColumn, tbl.Rows[0]["MaDVVT"]);
                                this.ActiveControl = button1;
                            }
                            else if (targetColumn.ToString() == "Khổ")
                            {
                                DataTable tbl = searchLookUpEditKV.Properties.DataSource as DataTable;
                                tbl = tbl.AsEnumerable().Where(x => x["KhoVai"].ToString() == rowData[i]).CopyToDataTable();
                                this.ActiveControl = button1;
                                view.SetRowCellValue(rowHandle, targetColumn, tbl.Rows[0]["KhoVaiID"]);
                            }

                            else
                            {
                                view.SetRowCellValue(rowHandle, targetColumn, rowData[i]);
                                this.ActiveControl = button1;
                            }

                        }
                    }


                    catch (Exception ex)
                    {
                        //MessageBox.Show($"Đã xảy ra lỗi khi dán dữ liệu vào cột '{view.VisibleColumns[columnIndex + i].FieldName}': {ex.Message}",
                        //                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }
        static string ReplaceSpecialCharacters(string input)
        {
            try
            {
                string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
                string replacement = "_";
                Regex regex = new Regex(pattern);
                return regex.Replace(input, replacement);
            }
            catch (Exception ex)
            {
                return input;

            }

        }
        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;
            result = Regex.Replace(result, "[àáạảãâầấậẩẫăằắặẳẵ]", "a");
            result = Regex.Replace(result, "[ÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴ]", "A");
            result = Regex.Replace(result, "[èéẹẻẽêềếệểễ]", "e");
            result = Regex.Replace(result, "[ÈÉẸẺẼÊỀẾỆỂỄ]", "E");
            result = Regex.Replace(result, "[ìíịỉĩ]", "i");
            result = Regex.Replace(result, "[ÌÍỊỈĨ]", "I");
            result = Regex.Replace(result, "[òóọỏõôồốộổỗơờớợởỡ]", "o");
            result = Regex.Replace(result, "[ÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠ]", "O");
            result = Regex.Replace(result, "[ùúụủũưừứựửữ]", "u");
            result = Regex.Replace(result, "[ÙÚỤỦŨƯỪỨỰỬỮ]", "U");
            result = Regex.Replace(result, "[ỳýỵỷỹ]", "y");
            result = Regex.Replace(result, "[ỲÝỴỶỸ]", "Y");
            result = Regex.Replace(result, "[đ]", "d");
            result = Regex.Replace(result, "[Đ]", "D");
            return result;

        }



        private void searchLookUpEditNhom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEditNhom.EditValue != null && !string.IsNullOrWhiteSpace(searchLookUpEditNhom.EditValue.ToString()))
                    _nhom = searchLookUpEditNhom.EditValue.ToString();
                if (string.IsNullOrEmpty(_nhom))
                {
                    searchLookUpEditNhom.Properties.Appearance.ForeColor = Color.Red;
                    return;
                }
                else
                {
                    searchLookUpEditNhom.Properties.Appearance.ForeColor = Color.Black;
                }


                DataRow nplSelect = _tblNhom.AsEnumerable().FirstOrDefault(r => r["MaCLVT"].ToString() == _nhom);
                if (nplSelect == null) return;

                if (nplSelect["NPL"].ToString().ToLower() == "true")
                {
                    //CreateSearchLookUpKhoVai();
                    selectedRowsKhoVai.Clear();
                    tabVT.SelectedTabPage = tabNL;
                }
                else
                {
                    //CreateSearchLookUpKhoVai();
                    selectedRowsKhoVai.Clear();
                    tabVT.SelectedTabPage = tabPL;
                }
                searchLookUpEditKV.EditValue = null;
                CreateSearchLookUpKhoVai();
                MaVTGhep["NhomVietTat"] = nplSelect["VietTat"].ToString().ToUpper();
                var result = BuildDynamicString(templateMaVTGhep, MaVTGhep);
                txtMaVTGhep.Text = string.Join(" ; ", result);
                gV_search.RefreshData();
            }
            catch (Exception ex)
            {

            }

        }
        private void txtMaMauVT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    TextEdit textEdit = sender as TextEdit;
                    if (textEdit != null)
                    {
                        string clipboardText = Clipboard.GetText();
                        e.SuppressKeyPress = true;
                        e.Handled = true;

                        string formattedText = clipboardText.Replace(Environment.NewLine, " ")
                                                            .Replace("\r", " ")
                                                            .Replace("\n", " ");
                        formattedText = System.Text.RegularExpressions.Regex.Replace(formattedText, @"\s+", " ").Trim();

                        int selectionStart = textEdit.SelectionStart;
                        int selectionLength = textEdit.SelectionLength;

                        textEdit.Text = textEdit.Text.Remove(selectionStart, selectionLength)
                                                     .Insert(selectionStart, formattedText);

                        textEdit.SelectionStart = selectionStart + formattedText.Length;
                        textEdit.SelectionLength = 0;
                    }
                }
            }
            catch (Exception ex)
            {


            }



        }

        private void txtMaVT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    TextEdit textEdit = sender as TextEdit;
                    if (textEdit != null)
                    {
                        string clipboardText = Clipboard.GetText();
                        e.SuppressKeyPress = true;
                        e.Handled = true;

                        string formattedText = clipboardText.Replace(Environment.NewLine, " ")
                                                            .Replace("\r", " ")
                                                            .Replace("\n", " ");
                        formattedText = System.Text.RegularExpressions.Regex.Replace(formattedText, @"\s+", " ").Trim();

                        int selectionStart = textEdit.SelectionStart;
                        int selectionLength = textEdit.SelectionLength;

                        textEdit.Text = textEdit.Text.Remove(selectionStart, selectionLength)
                                                     .Insert(selectionStart, formattedText);

                        textEdit.SelectionStart = selectionStart + formattedText.Length;
                        textEdit.SelectionLength = 0;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void txtChiTiet_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    TextEdit textEdit = sender as TextEdit;
                    if (textEdit != null)
                    {
                        string clipboardText = Clipboard.GetText();
                        e.SuppressKeyPress = true;
                        e.Handled = true;

                        string formattedText = clipboardText.Replace(Environment.NewLine, " ")
                                                            .Replace("\r", " ")
                                                            .Replace("\n", " ");
                        formattedText = System.Text.RegularExpressions.Regex.Replace(formattedText, @"\s+", " ").Trim();

                        int selectionStart = textEdit.SelectionStart;
                        int selectionLength = textEdit.SelectionLength;

                        textEdit.Text = textEdit.Text.Remove(selectionStart, selectionLength)
                                                     .Insert(selectionStart, formattedText);

                        textEdit.SelectionStart = selectionStart + formattedText.Length;
                        textEdit.SelectionLength = 0;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void txtMauVT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    TextEdit textEdit = sender as TextEdit;
                    if (textEdit != null)
                    {
                        string clipboardText = Clipboard.GetText();
                        e.SuppressKeyPress = true;
                        e.Handled = true;

                        string formattedText = clipboardText.Replace(Environment.NewLine, " ")
                                                            .Replace("\r", " ")
                                                            .Replace("\n", " ");
                        formattedText = System.Text.RegularExpressions.Regex.Replace(formattedText, @"\s+", " ").Trim();

                        int selectionStart = textEdit.SelectionStart;
                        int selectionLength = textEdit.SelectionLength;

                        textEdit.Text = textEdit.Text.Remove(selectionStart, selectionLength)
                                                     .Insert(selectionStart, formattedText);

                        textEdit.SelectionStart = selectionStart + formattedText.Length;
                        textEdit.SelectionLength = 0;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void txtMaMauVT_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            catch (Exception ex)
            {


            }

        }

        private void txtMauVT_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            catch (Exception ex)
            {


            }

        }

        private void txtGhiChu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    TextEdit textEdit = sender as TextEdit;
                    if (textEdit != null)
                    {
                        string clipboardText = Clipboard.GetText();
                        e.SuppressKeyPress = true;
                        e.Handled = true;

                        string formattedText = clipboardText.Replace(Environment.NewLine, " ")
                                                            .Replace("\r", " ")
                                                            .Replace("\n", " ");
                        formattedText = System.Text.RegularExpressions.Regex.Replace(formattedText, @"\s+", " ").Trim();

                        int selectionStart = textEdit.SelectionStart;
                        int selectionLength = textEdit.SelectionLength;

                        textEdit.Text = textEdit.Text.Remove(selectionStart, selectionLength)
                                                     .Insert(selectionStart, formattedText);

                        textEdit.SelectionStart = selectionStart + formattedText.Length;
                        textEdit.SelectionLength = 0;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void gVNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;

                if (gridView != null && gridView.FocusedColumn != null)
                {
                    string columnName = gridView.FocusedColumn.FieldName;
                    if (columnName == "MaMauVT" || columnName == "MauVT")
                    {
                        if (Char.IsLetter(e.KeyChar))

                            e.KeyChar = Char.ToUpper(e.KeyChar);
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void gVPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;

                if (gridView != null && gridView.FocusedColumn != null)
                {
                    string columnName = gridView.FocusedColumn.FieldName;
                    if (columnName == "MaMauVT" || columnName == "MauVT")
                    {
                        if (Char.IsLetter(e.KeyChar))

                            e.KeyChar = Char.ToUpper(e.KeyChar);
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void gCNL_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                GridControl grid = sender as GridControl;
                gVNL_KeyPress(grid.FocusedView, e);
            }
            catch (Exception ex)
            {


            }

        }

        private void gCPL_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                GridControl grid = sender as GridControl;
                gVPL_KeyPress(grid.FocusedView, e);
            }
            catch (Exception ex)
            {


            }

        }



        private void khaiBaoDVButton_Click(object sender, EventArgs e)
        {
            try
            {
                frmERPDonViVT frm = new frmERPDonViVT();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
                CreateSearchLookUpDonVi();
                CreateSearchLookUpDVVT();
            }
            catch (Exception ex)
            {


            }

        }

        private void searchLookUpEditKV_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    // Tạo nhóm layout dạng bảng
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    // Thêm nhóm vào layout chính
                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);

                    // 1. Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // 2. Nút "Khai báo Khổ/Size"
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Khổ/Size" };
                    khaiBaoButton.Click += khaiBaoKhoButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    // 3. Empty space trên và dưới để căn giữa
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void khaiBaoKhoButton_Click(object sender, EventArgs e)
        {
            try
            {
                frmChungLoaiVatTu frm = new frmChungLoaiVatTu();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
                CreateSearchLookUpKhoVai();
                selectedRowsKhoVai.Clear();
                CreateSearchLookUpKHO();
            }
            catch (Exception ex)
            {


            }

        }


        private void khaiBaoNhomButton_Click(object sender, EventArgs e)
        {
            try
            {
                frmChungLoaiVatTu frm = new frmChungLoaiVatTu();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
                CreateSearchLookUpNhom();
            }
            catch (Exception ex)
            {
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
                    if (gridColumn63.Visible)
                    {
                        gridColumn63.VisibleIndex = gVNL.Columns.Count - 1;
                    }

                }
                else if (selectedTabPage.Name == "tabPL")
                {
                    _isNPL = false;
                    if (gridColumn64.Visible)
                    {
                        gridColumn64.VisibleIndex = gVPL.Columns.Count - 1;
                    }

                }
            }
            catch (Exception ex)
            {


            }

        }

        private void loadSearchLookUpMaMauVT()
        {
            try
            {
                string url = $"{URL}ERPVatTuBom/GetChung?Action=GETMAUVTTV";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    repo_MaMauVT.DataSource = null;
                    repo_MaMauVTPL.DataSource = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    repo_MaMauVT.DataSource = null;
                    repo_MaMauVTPL.DataSource = null;
                    return;
                }
                repo_MaMauVTPL.DisplayMember = "MaMauVT";
                repo_MaMauVTPL.ValueMember = "MaMauVT";
                repo_MaMauVTPL.DataSource = tbl;
                DataTable filteredTbl = tbl.AsEnumerable()
              .Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("MaMauVT")))
              .CopyToDataTable();


                repo_MaMauVT.DisplayMember = "MaMauVT";
                repo_MaMauVT.ValueMember = "MaMauVT";
                repo_MaMauVT.DataSource = filteredTbl;
                tblMauVTTVCheck = filteredTbl;


            }
            catch (Exception ex)
            {
            }
        }
        private void repo_MaMauVT_EditValueChanged(object sender, EventArgs e)
        {
            changeMauVT(sender, e);
        }
        private void repo_MaMauVTPL_EditValueChanged(object sender, EventArgs e)
        {
            changeMauVT(sender, e);
        }
        private void changeMauVT(object sender, EventArgs e)
        {
            try
            {
                SearchLookUpEdit edit = sender as SearchLookUpEdit;
                GridView view = edit.Properties.View as GridView;
                DataRowView row = view.GetRow(view.FocusedRowHandle) as DataRowView;

                if (row != null)
                {

                    var value1 = row["MauVTID"];
                    var value2 = row["MaMauVT"];
                    var value3 = row["MauVT"];
                    DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                    if (dr == null) return;
                    dr["MauVTID"] = value1.ToString();
                    dr["MaMauVT"] = value2.ToString();
                    dr["MauVT"] = value3.ToString();
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void khaiBaoMauButton_Click(object sender, EventArgs e)
        {
            try
            {
                frmERP_MauTV frm = new frmERP_MauTV();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                loadSearchLookUpMaMauVT();
            }
            catch (Exception ex)
            {


            }

        }
        private void loadSearchLookUpMaVT()
        {
            try
            {
                string url = $"{URL}ERPVatTuTV/Get?Action=GET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    repo_MaVT.DataSource = null;
                    repo_MaVTPL.DataSource = null;
                    return;
                }
                tblItemCode = JsonConvert.DeserializeObject<DataTable>(json);
                repo_MaVT.DisplayMember = "MaVT";
                repo_MaVT.ValueMember = "MaVT";
                repo_MaVT.DataSource = tblItemCode;

                repo_MaVTPL.DisplayMember = "MaVT";
                repo_MaVTPL.ValueMember = "MaVT";
                repo_MaVTPL.DataSource = tblItemCode;

            }
            catch (Exception ex)
            {
            }
        }
        private void repo_MaVTPL_EditValueChanged(object sender, EventArgs e)
        {
            changeMaVT(sender, e);
        }

        private void repo_MaVT_EditValueChanged(object sender, EventArgs e)
        {
            changeMaVT(sender, e);
        }
        private void changeMaVT(object sender, EventArgs e)
        {
            try
            {
                SearchLookUpEdit edit = sender as SearchLookUpEdit;
                GridView view = edit.Properties.View as GridView;
                DataRowView row = view.GetRow(view.FocusedRowHandle) as DataRowView;

                if (row != null)
                {

                    var value1 = row["MaVTID"];
                    var value2 = row["MaVT"];
                    var value3 = row["ChiTiet"];
                    DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                    if (dr == null) return;
                    dr["MaVTID"] = value1.ToString();
                    dr["MaVT"] = value2.ToString();
                    dr["ChiTiet"] = value3.ToString();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void khaiBaoVatTuButton_Click(object sender, EventArgs e)
        {
            try
            {
                frmERP_VatTuTV frm = new frmERP_VatTuTV();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                loadSearchLookUpMaVT();
            }
            catch (Exception ex)
            {


            }

        }

        #region mấy cái popup của searchLookUp
        private void rCountryEditkv_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    // Tạo nhóm layout dạng bảng
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    // Thêm nhóm vào layout chính
                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);

                    // 1. Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // 2. Nút "Khai báo nhóm NPL"
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Clear" };
                    khaiBaoButton.Click += clearButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    // 3. Empty space trên và dưới để căn giữa
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void searchLookUpEditDV_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    // Tạo nhóm layout dạng bảng
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    // Thêm nhóm vào layout chính
                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);

                    // 1. Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // 2. Nút "Khai báo Đơn vị"
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Đơn vị" };
                    khaiBaoButton.Click += khaiBaoDVButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    // 3. Empty space trên và dưới để căn giữa
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void searchLookUpEditNhom_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    // Tạo nhóm layout dạng bảng
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    // Thêm nhóm vào layout chính
                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);

                    // 1. Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // 2. Nút "Khai báo nhóm NPL"
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Loại vật tư" };
                    khaiBaoButton.Click += khaiBaoNhomButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    // 3. Empty space trên và dưới để căn giữa
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void rCountryEditdv_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    // Tạo nhóm layout dạng bảng
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    // Thêm nhóm vào layout chính
                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);

                    // 1. Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // 2. Nút "Khai báo nhóm NPL"
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Clear" };
                    khaiBaoButton.Click += clearButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    // 3. Empty space trên và dưới để căn giữa
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void repo_MaMauVT_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                view.FocusedRowChanged -= View_FocusedRowChangedNLMau; // tránh đăng ký nhiều lần
                view.FocusedRowChanged += View_FocusedRowChangedNLMau;

                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    // Tạo nhóm layout dạng bảng
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // Thiết lập 3 cột: EmptySpace - btnOK - btnKhaiBao
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                    // Thêm nhóm vào layout chính
                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);

                    // 1. Empty space bên trái
                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // 2. Nút "Xác nhận" (OK)
                    var okButton = new SimpleButton() { Name = "btnClear", Text = "Clear" };
                    okButton.Click += clearButton_Click;
                    var layoutItemOK = new LayoutControlItem()
                    {
                        Control = okButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(80, 30),
                        MaxSize = new Size(80, 30)
                    };
                    layoutItemOK.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemOK.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemOK);

                    // 3. Nút "Khai báo Màu SP"
                    var cancelButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo màu vật tư" };
                    cancelButton.Click += khaiBaoMauButton_Click;
                    var layoutItemCancel = new LayoutControlItem()
                    {
                        Control = cancelButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemCancel.OptionsTableLayoutItem.ColumnIndex = 2;
                    layoutItemCancel.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemCancel);

                    // 4. Empty space trên và dưới để căn giữa
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void repo_MaMauVTPL_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                view.FocusedRowChanged -= View_FocusedRowChangedPLMau; // tránh đăng ký nhiều lần
                view.FocusedRowChanged += View_FocusedRowChangedPLMau;

                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

                var layout = popupForm.Controls
                    .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                    .FirstOrDefault()
                    ?.Controls
                    .OfType<LayoutControl>()
                    .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
                {
                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
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

                    layout.BeginUpdate();
                    layout.Root.AddItem(buttonGroup);


                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(emptySpaceItemLeft);


                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Màu vật tư" };
                    khaiBaoButton.Click += khaiBaoMauButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {


            }

        }


        private void repo_MaVT_Popup(object sender, EventArgs e)
        {

            try
            {

                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                view.FocusedRowChanged -= View_FocusedRowChanged;
                view.FocusedRowChanged += View_FocusedRowChanged;
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
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
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });

                    int khaiBaoColIndex = 1;

                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Clear
                    khaiBaoColIndex = 2;

                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Khai Bao

                    // 3 hàng: trên - giữa (button) - dưới
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });


                    layout.Root.AddItem(buttonGroup);

                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // Nếu là filter row, tạo thêm nút Clear ở cột 1

                    var clearButton = new SimpleButton() { Name = "btnClear", Text = "Clear" };
                    clearButton.Click += clearButton_Click;
                    var layoutItemClear = new LayoutControlItem()
                    {
                        Control = clearButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(80, 30),
                        MaxSize = new Size(80, 30)
                    };
                    layoutItemClear.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemClear.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemClear);


                    // Nút Khai báo Vật tư luôn luôn add
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Vật tư" };
                    khaiBaoButton.Click += khaiBaoVatTuButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = khaiBaoColIndex;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            Control control = sender as Control;
            while (control != null && !(control is DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm))
                control = control.Parent;

            if (control is DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm popupForm)
            {
                var searchLookUpEdit = popupForm.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;
                if (searchLookUpEdit != null)
                {
                    var view = searchLookUpEdit.Properties.View;
                    if (view != null)
                    {
                        view.ClearSelection();
                        view.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
                    }
                    searchLookUpEdit.EditValue = null;

                    var gridControl = FindParentGridControl(searchLookUpEdit);
                    if (gridControl != null)
                    {
                        var gridView = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView
                                       ?? gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (gridView != null)
                        {
                            var column = gridView.FocusedColumn;
                            if (column != null && gridView.IsFilterRow(gridView.FocusedRowHandle))
                            {
                                gridView.SetAutoFilterValue(column, null, DevExpress.XtraGrid.Columns.AutoFilterCondition.Equals);
                                //gridView.CloseEditor();
                            }
                        }
                    }
                }
                searchLookUpEdit.ClosePopup();
            }
        }

        private DevExpress.XtraGrid.GridControl FindParentGridControl(Control control)
        {
            while (control != null && !(control is DevExpress.XtraGrid.GridControl))
                control = control.Parent;
            return control as DevExpress.XtraGrid.GridControl;
        }

        private void repo_MaVTPL_Popup(object sender, EventArgs e)
        {
            try
            {

                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                view.FocusedRowChanged -= View_FocusedRowChanged;
                view.FocusedRowChanged += View_FocusedRowChanged;
                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
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
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });

                    int khaiBaoColIndex = 1;

                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Clear
                    khaiBaoColIndex = 2;

                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize }); // Khai Bao

                    // 3 hàng: trên - giữa (button) - dưới
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });


                    layout.Root.AddItem(buttonGroup);

                    var emptySpaceItemLeft = new EmptySpaceItem();
                    emptySpaceItemLeft.AllowHotTrack = false;
                    emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(emptySpaceItemLeft);

                    // Nếu là filter row, tạo thêm nút Clear ở cột 1

                    var clearButton = new SimpleButton() { Name = "btnClear", Text = "Clear" };
                    clearButton.Click += clearButton_Click;
                    var layoutItemClear = new LayoutControlItem()
                    {
                        Control = clearButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(80, 30),
                        MaxSize = new Size(80, 30)
                    };
                    layoutItemClear.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemClear.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemClear);


                    // Nút Khai báo Vật tư luôn luôn add
                    var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Vật tư" };
                    khaiBaoButton.Click += khaiBaoVatTuButton_Click;
                    var layoutItemKhaiBao = new LayoutControlItem()
                    {
                        Control = khaiBaoButton,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(140, 30),
                        MaxSize = new Size(140, 30)
                    };
                    layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = khaiBaoColIndex;
                    layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemKhaiBao);

                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    layout.EndUpdate();
                }
            }
            catch (Exception ex)
            {

            }

        }

        #endregion
        private void tabVT_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            try
            {
                int tabIndex = tabVT.SelectedTabPageIndex;
                bool hasChange = false;
                bool isSaved = true;

                if (tabIndex == 0)
                {
                    if (tblCNL != null && tblCNL.Rows.Count != 0)
                    {
                        hasChange = tblCNL.GetChanges() != null;
                    }

                    isSaved = _isSaveNL;
                }
                else if (tabIndex == 1)
                {
                    if (tblCPL != null && tblCPL.Rows.Count != 0)
                    {
                        hasChange = tblCPL.GetChanges() != null;
                    }


                    isSaved = _isSavePL;
                }
            }
            catch (Exception ex)
            {
            }

        }
        private void searchLookUpEditTimVT_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                DataRow dr = gV_search.GetFocusedDataRow();
                if (dr == null) return;
                searchLookUpEditNhom.EditValue = dr["MaNhom"].ToString();
                txtMaVT.Text = dr["MaVT"].ToString();
                txtChiTiet.Text = dr["ChiTiet"].ToString();
                searchLookUpEditDV.EditValue = dr["MaDVVT"].ToString();
                searchLookUpEditKV.EditValue = dr["KhoVaiID"].ToString();
            }
            catch (Exception ex)
            {
            }
        }
        #region Xử lý style gridView
        private void gV_search_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn27)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }

        private void gVNL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == colTenNhomNL || info.Column == gridColumn61)
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
            if (info.Column == colTenNhomPL || info.Column == gridColumn62)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }
        private void gVNL_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
            }
        }

        private void gVPL_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
            }

        }
        #endregion

        private void loadTimKiem()
        {
            try
            {
                string url = $"{URL}ERPVatTuBOM/GetTimKiem";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;

                searchLookUpEditTimVT.Properties.DisplayMember = "MaVTGhep";
                searchLookUpEditTimVT.Properties.ValueMember = "ValueMember";
                searchLookUpEditTimVT.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {
            }

        }

        #region xử lý khố vải
        private void searchLookUpEditViewKV_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle = e.ControllerRow;
            if (view == null) return;

            if (e.Action == CollectionChangeAction.Add)
            {
                DataRow row = view.GetDataRow(e.ControllerRow);
                if (row != null && !selectedRowsKhoVai.Contains(row))
                {
                    selectedRowsKhoVai.Add(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DataRow row = view.GetDataRow(e.ControllerRow);
                if (row != null)
                {
                    selectedRowsKhoVai.Remove(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                selectedRowsKhoVai.Clear();


                int[] selectedRowHandles = view.GetSelectedRows();
                foreach (int rowHandle2 in selectedRowHandles)
                {
                    if (rowHandle2 >= 0)
                    {
                        DataRow row = view.GetDataRow(rowHandle2);
                        if (row != null)
                        {
                            selectedRowsKhoVai.Add(row);
                        }
                    }
                }
            }
            string selectedValues = string.Join(";", searchLookUpEditViewKV.GetSelectedRows().Select(rowHandle2 => searchLookUpEditViewKV.GetRowCellValue(rowHandle2, searchLookUpEditKV.Properties.ValueMember)));
            searchLookUpEditKV.EditValue = selectedValues;
            if (searchLookUpEditKV.EditValue is null) return;
        }
        private void searchLookUpEditViewKV_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRowsKhoVai.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }



        private void searchLookUpEditKV_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

            if (selectedRowsKhoVai == null || !selectedRowsKhoVai.Any())
            {
                e.DisplayText = "Chọn Khổ/Size";
                searchLookUpEditKV.Properties.Appearance.ForeColor = Color.Red;
                MaVTGhep["KhoSize"] = "";
            }
            else
            {
                var displayValues = new List<string>();
                for (int i = 0; i < searchLookUpEditViewKV.DataRowCount; i++)
                {
                    object rowValueObj = searchLookUpEditViewKV.GetRowCellValue(i, searchLookUpEditKV.Properties.ValueMember);
                    string rowValue = rowValueObj?.ToString();
                    if (rowValue != null && selectedRowsKhoVai.Any(dr => dr["KhoVaiID"]?.ToString() == rowValue))
                    {
                        string displayValue = searchLookUpEditViewKV.GetRowCellValue(i, searchLookUpEditKV.Properties.DisplayMember)?.ToString();
                        if (displayValue != null)
                        {
                            displayValues.Add(displayValue);
                        }
                    }
                }

                e.DisplayText = string.Join(";", displayValues);
                searchLookUpEditKV.Properties.Appearance.ForeColor = Color.Black;
                MaVTGhep["KhoSize"] = e.DisplayText;
            }
            var result = BuildDynamicString(templateMaVTGhep, MaVTGhep);
            txtMaVTGhep.Text = string.Join("; ", result);
        }
        #endregion
        private void gVNL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            indexFCNL = gVNL.FocusedRowHandle;
            DataRow srcRow = gVNL.GetFocusedDataRow();
            if (srcRow != null)
            {
                DataTable dt = srcRow.Table;
                DataRow rowCopy = dt.NewRow();
                rowCopy.ItemArray = (object[])srcRow.ItemArray.Clone(); // Clone giá trị
                _rowFocus = rowCopy;
            }
            else
            {
                _rowFocus = null;
            }
        }

        private void gVPL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            indexFCPL = gVPL.FocusedRowHandle;
            DataRow srcRow = gVPL.GetFocusedDataRow();
            if (srcRow != null)
            {
                DataTable dt = srcRow.Table;
                DataRow rowCopy = dt.NewRow();
                rowCopy.ItemArray = (object[])srcRow.ItemArray.Clone();
                _rowFocus = rowCopy;
            }
            else
            {
                _rowFocus = null;
            }
        }

        #region xử lý mã vật tư ghép
        public List<string> BuildDynamicString(string template, Dictionary<string, string> values)
        {


            //return listResult;
            var listResult = new List<string>();
            var khoSizeStr = values.ContainsKey("KhoSize") ? values["KhoSize"] : "";
            var khoSizes = khoSizeStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Nếu khoSizes không có giá trị (nghĩa là KhoSize rỗng), tạo một mảng chứa giá trị "00"
            if (khoSizes.Length == 0)
            {
                khoSizes = new[] { "00" };
            }

            // Xử lý như cũ, mỗi kho một dòng
            foreach (var kho in khoSizes)
            {
                var tempDict = new Dictionary<string, string>(values);
                tempDict["KhoSize"] = kho.Trim();

                var matches = Regex.Matches(template, @"\{(.*?)\}");
                var list = new List<string>();

                foreach (Match m in matches)
                {
                    var key = m.Groups[1].Value;
                    if (tempDict.ContainsKey(key) && !string.IsNullOrWhiteSpace(tempDict[key]))
                    {
                        list.Add(tempDict[key]);
                    }
                }
                listResult.Add(string.Join("|", list));
            }

            return listResult;
        }
        private void txtMaVT_EditValueChanged(object sender, EventArgs e)
        {
            MaVTGhep["ItemCode"] = txtMaVT.Text.ToUpper();
            var result = BuildDynamicString(templateMaVTGhep, MaVTGhep);
            txtMaVTGhep.Text = string.Join(" ; ", result);
            checkDupItemcode(txtMaVT.Text.ToUpper());
        }


        private void txtMaMauVT_EditValueChanged(object sender, EventArgs e)
        {
            MaVTGhep["MauVT"] = (searckLookUpEditMauVT.Text.ToString().ToUpper() == "" || searckLookUpEditMauVT.Text.ToString().ToUpper() == "CHỌN MÀU VẬT TƯ") ? "00" : searckLookUpEditMauVT.Text.ToString().ToUpper();
            var result = BuildDynamicString(templateMaVTGhep, MaVTGhep);
            txtMaVTGhep.Text = string.Join(" ; ", result);
        }
        private void UpdateMaVTGhep_ForFocusedRow(DataRow dr, List<DataRow> lst = null)
        {
            try
            {
                if (dr == null) return;
                if (lst == null)
                {
                    object maNhom = dr["MaNhom"].ToString();
                    object maVT = dr["MaVT"].ToString();
                    object ChiTiet = dr["ChiTiet"].ToString();
                    object mauVT = dr["MaMauVT"].ToString();
                    string khoSize = (from row in tblkv.AsEnumerable()
                                      where row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString()
                                      select row.Field<string>("KhoVai")).FirstOrDefault();
                    // object khoSize = dr["KhoVai"].ToString();//dr["KhoVai"] ? dr["KhoVai"].ToString():"";

                    // Tìm viết tắt nhóm vật tư
                    string nhomVT = "";
                    if (maNhom != null)
                    {
                        DataRow _rowNhom = _tblNhom.Select($"MaCLVT = '{maNhom.ToString()}'").FirstOrDefault();
                        if (_rowNhom != null)
                            nhomVT = _rowNhom["VietTat"].ToString();
                    }

                    // Viết tắt khách hàng (tuỳ business, có thể lấy theo dòng hoặc theo control ngoài lưới)
                    string khVT = "";


                    string maVTGhep = string.Join("|",
                          string.IsNullOrWhiteSpace(nhomVT) ? "00" : nhomVT,
                          string.IsNullOrWhiteSpace(Convert.ToString(maVT)) ? "00" : maVT.ToString(),
                          string.IsNullOrWhiteSpace(Convert.ToString(mauVT)) ? "00" : mauVT.ToString(),
                          string.IsNullOrWhiteSpace(Convert.ToString(khoSize)) ? "00" : khoSize.ToString(),

                          string.IsNullOrWhiteSpace(_tudoTrung) ? "" : _tudoTrung
                      );
                    maVTGhep = maVTGhep.Trim('|');
                    dr["MaVTGhep"] = maVTGhep;
                }
                else
                {
                    foreach (DataRow _dr in lst)
                    {
                        object maNhom = _dr["MaNhom"].ToString();
                        object maVT = _dr["MaVT"].ToString();
                        object ChiTiet = _dr["ChiTiet"].ToString();
                        object mauVT = _dr["MaMauVT"].ToString();
                        string khoSize = (from row in tblkv.AsEnumerable()
                                          where row["KHoVaiID"].ToString() == _dr["KHoVaiID"].ToString()
                                          select row.Field<string>("KhoVai")).FirstOrDefault();
                        // object khoSize = dr["KhoVai"].ToString();//dr["KhoVai"] ? dr["KhoVai"].ToString():"";

                        // Tìm viết tắt nhóm vật tư
                        string nhomVT = "";
                        if (maNhom != null)
                        {
                            DataRow _rowNhom = _tblNhom.Select($"MaCLVT = '{maNhom.ToString()}'").FirstOrDefault();
                            if (_rowNhom != null)
                                nhomVT = _rowNhom["VietTat"].ToString();
                        }

                        // Viết tắt khách hàng (tuỳ business, có thể lấy theo dòng hoặc theo control ngoài lưới)
                        string khVT = "";

                        string guid = Guid.NewGuid().ToString("N").ToUpper();
                        _tudoTrung = "DN" + guid.Substring(0, 6);
                        string maVTGhep = string.Join("|",
                              string.IsNullOrWhiteSpace(nhomVT) ? "00" : nhomVT,
                              string.IsNullOrWhiteSpace(Convert.ToString(maVT)) ? "00" : maVT.ToString(),
                              string.IsNullOrWhiteSpace(Convert.ToString(mauVT)) ? "00" : mauVT.ToString(),
                              string.IsNullOrWhiteSpace(Convert.ToString(khoSize)) ? "00" : khoSize.ToString(),
                              string.IsNullOrWhiteSpace(_tudoTrung) ? "" : _tudoTrung
                          );
                        maVTGhep = maVTGhep.Trim('|');
                        _dr["MaVTGhep"] = maVTGhep;
                    }
                }

            }
            catch (Exception ex)
            {

            }




        }
        private void checkMaVTGhepCellValueChanged(string fieldname, DataRow dr)
        {//true là dừng lại, false là đi tiếp
            try
            {

                //return ;
                DataTable _tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                object currentMaVTGhepValue = dr["MaVTGhep"];
                if (currentMaVTGhepValue == DBNull.Value || currentMaVTGhepValue == null)
                {
                    return;
                }
                string currentMaVTGhep = currentMaVTGhepValue.ToString().Replace(" ", "").ToUpper();
                if (string.IsNullOrEmpty(currentMaVTGhep))
                {
                    return;
                }

                int duplicateCount = _tbl.AsEnumerable()
                                         .Count(row =>
                                             row.RowState != DataRowState.Deleted &&
                                             row["MaVTGhep"] != DBNull.Value &&
                                             row["MaVTGhep"].ToString().Replace(" ", "").ToUpper().Equals(currentMaVTGhep)
                                         );


                if (duplicateCount > 1)
                {
                    var result = MessageBox.Show(
                            "Mã vật tư này đã tồn tại ở một dòng khác. Bạn có muốn tự sinh ra Mã vật tư mới không?",
                            "Cảnh báo trùng lặp",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                    if (result == DialogResult.Yes)
                    {
                        string guid = Guid.NewGuid().ToString("N").ToUpper();
                        _tudoTrung = guid.Substring(0, 6);
                        UpdateMaVTGhep_ForFocusedRow(dr);
                        return;
                    }
                    else
                    {

                        foreach (DataColumn col in _tbl.Columns)
                        {
                            dr[col.ColumnName] = _rowFocus[col.ColumnName];
                        }
                        _tudoTrung = string.Empty;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        #endregion

        #region lưu xong gán select lại
        private void gVNL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            savedSelectedCells.Clear();
            var selectedCells = gVNL.GetSelectedCells();
            foreach (GridCell cell in selectedCells)
            {
                savedSelectedCells.Add(cell);
            }
        }
        private void gVPL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            savedSelectedCells.Clear();
            var selectedCells = gVPL.GetSelectedCells();
            foreach (GridCell cell in selectedCells)
            {
                savedSelectedCells.Add(cell);
            }
        }

        private void txtTuDo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void txtMaVT_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (char.IsControl(e.KeyChar)) return;
            e.KeyChar = char.ToUpper(e.KeyChar);
            string currentText = txtMaVT.Text;
            int selectionStart = txtMaVT.SelectionStart;

            string inputChar = e.KeyChar.ToString();
            string noDiacritics = RemoveVietnameseTone(inputChar);

            if (inputChar != noDiacritics)
            {
                e.Handled = true;

                string result = currentText.Substring(0, selectionStart) +
                                noDiacritics +
                                currentText.Substring(selectionStart);
                txtMaVT.Text = result.ToUpper();
                txtMaVT.SelectionStart = selectionStart + noDiacritics.Length;
            }
        }

        private void txtChiTiet_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }


        private void RestoreSelectedCells(GridView view)
        {
            view.ClearSelection();
            foreach (GridCell cell in savedSelectedCells)
            {

                if (view.IsValidRowHandle(cell.RowHandle) && cell.Column != null)
                {
                    view.SelectCell(cell.RowHandle, cell.Column);
                }
            }
        }


        #endregion

        #region Enter để xuống dòng
        private void gVNL_KeyDown(object sender, KeyEventArgs e)
        {
            string _fieldName = gVNL.FocusedColumn.FieldName;
            HandleGridViewKey(e, gVNL, _fieldName);
        }

        private void searchLookUpEditDV_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditDV.EditValue == null) return;
            _madvvt = searchLookUpEditDV.EditValue.ToString();
            if (string.IsNullOrEmpty(_madvvt))
            {
                searchLookUpEditDV.Properties.Appearance.ForeColor = Color.Red;
            }
            else
            {
                searchLookUpEditDV.Properties.Appearance.ForeColor = Color.Black;
            }
        }

        private void txtGhiChu_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }
        #region xử lý repo_Search cũ
        private void View_FocusedRowChangedPLMau(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }
        private void View_FocusedRowChangedNLMau(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }
        private void View_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }
        private void View_FocusedRowChangedpl(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

        }



        #endregion

        private void repo_MaMauVT_CloseUp(object sender, CloseUpEventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor != null && e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            {
                DevExpress.XtraGrid.Views.Grid.GridView popupGridView = editor.Properties.View;
                object selectedRow = popupGridView.GetFocusedRow();

                if (selectedRow != null)
                {
                    System.Data.DataRowView rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {

                        object mamauVtValue = rowView["MaMauVT"];
                        object mauvtValue = rowView["MauVT"];
                        gVNL.SetFocusedRowCellValue("MaMauVT", mamauVtValue);
                        gVNL.SetFocusedRowCellValue("MauVT", mauvtValue);
                        gVNL.PostEditor();
                        gVNL.UpdateCurrentRow();
                    }
                    else
                    {

                    }
                }
            }
        }

        private void repo_MaVT_CloseUp(object sender, CloseUpEventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor != null && e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            {
                DevExpress.XtraGrid.Views.Grid.GridView popupGridView = editor.Properties.View;
                object selectedRow = popupGridView.GetFocusedRow();

                if (selectedRow != null)
                {
                    System.Data.DataRowView rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {

                        object maVtValue = rowView["MaVT"];
                        object chiTietValue = rowView["ChiTiet"];


                        gVNL.SetFocusedRowCellValue("MaVT", maVtValue);
                        gVNL.SetFocusedRowCellValue("ChiTiet", chiTietValue);

                        gVNL.PostEditor();
                        gVNL.UpdateCurrentRow();
                    }
                    else
                    {

                    }
                }
            }
        }

        private void repo_MaVTPL_CloseUp(object sender, CloseUpEventArgs e)
        {

            DevExpress.XtraEditors.SearchLookUpEdit editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor != null && e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            {
                DevExpress.XtraGrid.Views.Grid.GridView popupGridView = editor.Properties.View;
                object selectedRow = popupGridView.GetFocusedRow();

                if (selectedRow != null)
                {
                    System.Data.DataRowView rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {

                        object maVtValue = rowView["MaVT"];
                        object chiTietValue = rowView["ChiTiet"];


                        gVPL.SetFocusedRowCellValue("MaVT", maVtValue);
                        gVPL.SetFocusedRowCellValue("ChiTiet", chiTietValue);

                        gVPL.PostEditor();
                        gVPL.UpdateCurrentRow();
                    }
                    else
                    {

                    }
                }
            }
        }

        private void repo_MaMauVTPL_CloseUp(object sender, CloseUpEventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor != null && e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
            {
                DevExpress.XtraGrid.Views.Grid.GridView popupGridView = editor.Properties.View;
                object selectedRow = popupGridView.GetFocusedRow();

                if (selectedRow != null)
                {
                    System.Data.DataRowView rowView = selectedRow as System.Data.DataRowView;
                    if (rowView != null)
                    {

                        object mamauVtValue = rowView["MaMauVT"];
                        object mauvtValue = rowView["MauVT"];


                        gVPL.SetFocusedRowCellValue("MaMauVT", mamauVtValue);
                        gVPL.SetFocusedRowCellValue("MauVT", mauvtValue);

                        gVPL.PostEditor();
                        gVPL.UpdateCurrentRow();
                    }
                    else
                    {

                    }
                }
            }
        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                searchLookUpEditNhom.EditValue = null;
                loadTimKiem();
                loadSearchLookUpMaVT();
                loadSearchLookUpMaMauVT();
                string _manhom = string.Empty;

                CreateSearchLookUpNhom();
                searchLookUpEditNhom.EditValue = _nhom;
                CreateSearchLookUpKhoVai();
                CreateSearchLookUpKHO();
                selectedRowsKhoVai.Clear();
                loadVatTu();
            }
            catch (Exception ex)
            {


            }
        }

        private void gVPL_KeyDown(object sender, KeyEventArgs e)
        {
            string _fieldName = gVPL.FocusedColumn.FieldName;
            HandleGridViewKey(e, gVPL, _fieldName);
        }



        public static void HandleGridViewKey(KeyEventArgs e, GridView gridView, string _fieldName)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int RowHandle = gridView.FocusedRowHandle;
                if (RowHandle == gridView.RowCount - 1) RowHandle = -1;
                gridView.FocusedRowHandle = RowHandle + 1;
                gridView.FocusedColumn = gridView.Columns[_fieldName];
                //gridView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Down)
            {
                gridView.FocusedColumn = gridView.Columns[_fieldName];
                //gridView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Up)
            {
                gridView.FocusedColumn = gridView.Columns[_fieldName];
                //gridView.ShowEditor();
            }
        }
        #endregion

        #region Excel

        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("ThongSoVatTu{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "ThongSoTemplate.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName);

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
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {


                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "ThongSoVatTu";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {

                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    worksheet.Cells["C3"].Value = "";
                    worksheet.Cells["C3"].Style.Font.Bold = true;
                    worksheet.Cells["C3"].Style.Font.Size = 12;
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }
        private void btnNhapExcel_Click(object sender, EventArgs e)
        {
            ImportExcel();
        }

        private void ImportExcel()
        {
            try
            {


                frmERP_VatTuExcel frm = new frmERP_VatTuExcel("", "", "", false);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {

                    CreateSearchLookUpNhom();
                    CreateSearchLookUpKhoVai();
                    CreateSearchLookUpDonVi();

                    CreateSearchLookUpDVVT();
                    CreateSearchLookUpKHO();
                    loadSearchLookUpMaMauVT();
                    loadSearchLookUpMaVT();
                    loadTimKiem();
                    loadVatTuCheck();
                    loadVatTu();

                }

            }
            catch (Exception ex)
            {
            }

        }
        private void gVNL_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName == "MaVT" || e.Column.FieldName == "ChiTiet")
            {
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                object maVTIDValue = view.GetRowCellValue(e.RowHandle, "MaVTID");

                object maVTValue = view.GetRowCellValue(e.RowHandle, "MaVT");
                object chiTietValue = view.GetRowCellValue(e.RowHandle, "ChiTiet");

                if (isNewValue != null && isNewValue.ToString() == "2" && (string.IsNullOrEmpty(maVTIDValue?.ToString()) || string.IsNullOrEmpty(maVTValue?.ToString()) || string.IsNullOrEmpty(chiTietValue?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column.FieldName == "MaDVVT")
            {
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                string madvvtValue = view.GetRowCellValue(e.RowHandle, "MaDVVT").ToString();


                if (isNewValue != null && isNewValue.ToString() == "2" && (madvvtValue.Contains("DVVTTEST_") || string.IsNullOrEmpty(madvvtValue?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column.FieldName == "KhoVaiID")
            {
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                string khovaiid = view.GetRowCellValue(e.RowHandle, "KhoVaiID")?.ToString();


                if (isNewValue != null && isNewValue.ToString() == "2" && (khovaiid.Contains("KHOTEST_") || string.IsNullOrEmpty(khovaiid?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column.FieldName == "MaMauVT" || e.Column.FieldName == "MauVT")
            {
                object mauVTIDValue = view.GetRowCellValue(e.RowHandle, "MauVTID");
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                object mamauVTValue = view.GetRowCellValue(e.RowHandle, "MaMauVT");
                object mauvtValue = view.GetRowCellValue(e.RowHandle, "MauVT");

                if (isNewValue != null && isNewValue.ToString() == "2" && (string.IsNullOrEmpty(mauVTIDValue?.ToString()) || string.IsNullOrEmpty(mamauVTValue?.ToString()) || string.IsNullOrEmpty(mauvtValue?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if(e.Column == colThanhPhanNL)
            {
                DataRow row = gVNL.GetDataRow(e.RowHandle);
                if(row != null)
                {
                    if(row["HaveThanhPhan"]?.ToString() == "Đã thêm")
                    {
                        e.Appearance.ForeColor = Color.Olive;
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
        }
        private void gVPL_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName == "MaVT" || e.Column.FieldName == "ChiTiet")
            {
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                object maVTIDValue = view.GetRowCellValue(e.RowHandle, "MaVTID");

                object maVTValue = view.GetRowCellValue(e.RowHandle, "MaVT");
                object chiTietValue = view.GetRowCellValue(e.RowHandle, "ChiTiet");

                if (isNewValue != null && isNewValue.ToString() == "2" && (string.IsNullOrEmpty(maVTIDValue?.ToString()) || string.IsNullOrEmpty(maVTValue?.ToString()) || string.IsNullOrEmpty(chiTietValue?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column.FieldName == "MaDVVT")
            {
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                object madvvtValue = view.GetRowCellValue(e.RowHandle, "MaDVVT");


                if (isNewValue != null && isNewValue.ToString() == "2" && string.IsNullOrEmpty(madvvtValue?.ToString()))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column.FieldName == "KhoVaiID")
            {
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                string khovaiid = view.GetRowCellValue(e.RowHandle, "KhoVaiID")?.ToString();


                if (isNewValue != null && isNewValue.ToString() == "2" && (khovaiid.Contains("KHOTEST_") || string.IsNullOrEmpty(khovaiid?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column.FieldName == "MaMauVT" || e.Column.FieldName == "MauVT")
            {
                object mauVTIDValue = view.GetRowCellValue(e.RowHandle, "MauVTID");
                object isNewValue = view.GetRowCellValue(e.RowHandle, "IsNew");
                object mamauVTValue = view.GetRowCellValue(e.RowHandle, "MaMauVT");
                object mauvtValue = view.GetRowCellValue(e.RowHandle, "MauVT");

                if (isNewValue != null && isNewValue.ToString() == "2" && (string.IsNullOrEmpty(mauVTIDValue?.ToString()) || string.IsNullOrEmpty(mamauVTValue?.ToString()) || string.IsNullOrEmpty(mauvtValue?.ToString())))
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 229, 204);
                }
            }
            if (e.Column == colThanhPhanPL)
            {
                DataRow row = gVPL.GetDataRow(e.RowHandle);
                if (row != null)
                {
                    if (row["HaveThanhPhan"]?.ToString() == "Đã thêm")
                    {
                        e.Appearance.ForeColor = Color.Olive;
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
        }
        #endregion

        #region Image
        private void btnSelectImage_Click(object sender, EventArgs e)
        {

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    selectedSourcePath = dialog.FileName;

                    picVT.Image = System.Drawing.Image.FromFile(selectedSourcePath);

                }
            }
        }
        public static string GetWebContentPath()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null && !directory.GetDirectories("NtbSoft.ERP.Web").Any())
                directory = directory.Parent;
            if (directory == null)
                throw new DirectoryNotFoundException("Không tìm thấy thư mục Image");
            var contentPath = Path.Combine(directory.FullName, "NtbSoft.ERP.Web", "Content", "Image");
            if (!Directory.Exists(contentPath))
                throw new DirectoryNotFoundException(contentPath);
            return contentPath;
        }


        private void gVPL_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

            var view = (GridView)sender;

            if (e.Column.FieldName == "STT1" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;

            if (!e.IsGetData || e.Column.FieldName != "imagePL")
                return;

            int listSourceIndex = e.ListSourceRowIndex;

            if (imageCachePL.TryGetValue(listSourceIndex, out Image img))
            {
                e.Value = img;
                return;
            }

            string url = Convert.ToString(view.GetListSourceRowCellValue(listSourceIndex, "urlAnh"));
            if (string.IsNullOrEmpty(url))
            {
                e.Value = null;
                return;
            }

            e.Value = null;

            if (!loadingRows.Contains(listSourceIndex))
            {
                loadingRows.Add(listSourceIndex);
                Task.Run(() => LoadImageAsync(listSourceIndex, url, false));
            }
        }
        private async Task LoadImageAsync(int rowIndex, string url, bool isNPL)
        {
            await loadSem.WaitAsync();
            try
            {
                GridView gV = isNPL ? gVNL : gVPL;
                var colImage = isNPL ? gridColumn1 : gridColumn2;

                url = $"/Images/ImageThuVien/VatTu/1/" + url + ".png";
                Image img = await LoadImageFromUrlAsync(url);
                if (img == null) return;

                this.BeginInvoke(new MethodInvoker(delegate
                {
                    if (isNPL) imageCache[rowIndex] = img;
                    else imageCachePL[rowIndex] = img;

                    int rowHandle = gV.GetRowHandle(rowIndex);
                    if (gV.IsValidRowHandle(rowHandle))
                    {
                        gV.RefreshRow(rowHandle);
                        gV.RefreshRowCell(rowHandle, colImage);
                    }
                }));
            }
            finally
            {
                loadSem.Release();
                loadingRows.Remove(rowIndex);
            }
        }
        private async Task<Image> LoadImageFromUrlAsync(string url)
        {
            try
            {
                using (HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
                {
                    string fullUrl = new Uri(new Uri(Host), url).ToString();
                    byte[] data = await http.GetByteArrayAsync(fullUrl);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        Image origin = Image.FromStream(ms, true);
                        Bitmap resized = new Bitmap(50, 50);
                        using (Graphics g = Graphics.FromImage(resized))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(origin, new Rectangle(0, 0, 50, 50), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel);
                        }
                        origin.Dispose();
                        return resized;
                    }
                }
            }
            catch
            {
                return null;  // Trả null nếu lỗi (hiển thị blank)
            }
        }
        private void gVNL_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

            var view = (GridView)sender;

            if (e.Column.FieldName == "STT1" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;

            if (!e.IsGetData || e.Column.FieldName != "image")
                return;

            int listSourceIndex = e.ListSourceRowIndex;

            if (imageCache.TryGetValue(listSourceIndex, out Image img))
            {
                e.Value = img;
                return;
            }

            string url = Convert.ToString(view.GetListSourceRowCellValue(listSourceIndex, "urlAnh"));
            if (string.IsNullOrEmpty(url))
            {
                e.Value = null;
                return;
            }

            e.Value = null;

            if (!loadingRows.Contains(listSourceIndex))
            {
                loadingRows.Add(listSourceIndex);
                Task.Run(() => LoadImageAsync(listSourceIndex, url, true));
            }
        }



        private void txtMaVT_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            frmERP_VatTuTV frm = new frmERP_VatTuTV(true);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                DataRow dr = frm.getMau();
                if (dr == null) return;
                txtMaVT.Text = dr["MaVT"].ToString();
                txtChiTiet.Text = dr["ChiTiet"].ToString();
            }
            loadSearchLookUpMaVT();
            loadVatTuCheck();
        }



        private void ClearImageCache()
        {
            foreach (var img in imageCache.Values)
                img.Dispose();
            imageCache.Clear();

            foreach (var img in imageCachePL.Values)
                img.Dispose();
            imageCachePL.Clear();

            foreach (var img in inMemoryImages.Values)
                img.Dispose();
            inMemoryImages.Clear();

        }



        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearImageCache();
            base.OnFormClosed(e);
        }







        private void gVNL_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (gVNL.FocusedColumn == colThanhPhanNL)
            {
                PopupVatTuThanhPhan();
            }
            else
            {
                if (e.Column != null && e.Column.FieldName == "IsCheckExcel")
                {
                    var currentValue = view.GetRowCellValue(e.RowHandle, e.Column);
                    bool newValue = !Convert.ToBoolean(currentValue);
                    view.SetRowCellValue(e.RowHandle, e.Column, newValue);
                    view.RefreshRow(e.RowHandle);
                    this.ActiveControl = simpleButton1;
                }

                if (e.Column.FieldName != "image") return;
                if (e.RowHandle < 0) return;
                var row = gVNL.GetDataRow(e.RowHandle);
                if (row == null) return;

                using (var dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.webp|All Files|*.*";
                    dialog.Multiselect = false;

                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                    try
                    {
                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        if (fileInfo.Length > 50 * 1024 * 1024)
                        {
                            MessageBox.Show("File ảnh quá lớn (>50MB). Vui lòng chọn ảnh nhỏ hơn.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        Image resizedImage = null;
                        using (var origin = Image.FromFile(dialog.FileName))
                        {
                            resizedImage = new Bitmap(50, 50);
                            using (var g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(origin, new Rectangle(0, 0, 50, 50), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel);
                            }
                        }

                        // GIỮ NGUYÊN CODE CŨ - dùng dataSourceIndex
                        int dataSourceIndex = gVNL.GetDataSourceRowIndex(e.RowHandle);

                        if (imageCache.TryGetValue(dataSourceIndex, out Image oldImage))
                        {
                            oldImage?.Dispose();
                            imageCache.Remove(dataSourceIndex);
                        }

                        imageCache[dataSourceIndex] = resizedImage;

                        // LƯU THÊM MAP: ID -> dataSourceIndex
                        string id = row["ID"].ToString();
                        idToIndexMapNL[id] = dataSourceIndex;

                        row["urlAnh"] = $"temp_{Guid.NewGuid():N}";
                        row["IsEdit"] = 1;

                        gVNL.UpdateCurrentRow();
                        gVNL.RefreshRow(e.RowHandle);
                        gVNL.RefreshRowCell(e.RowHandle, e.Column);
                    }
                    catch (OutOfMemoryException)
                    {
                        MessageBox.Show("Kích thước ảnh quá lớn hoặc định dạng không hỗ trợ. Vui lòng thử ảnh khác.",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi load ảnh: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
           
        }



        private void gVPL_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

            if(gVPL.FocusedColumn == colThanhPhanPL)
            {
                PopupVatTuThanhPhan();
            }
            else
            {
                if (e.Column != null && e.Column.FieldName == "IsCheckExcel")
                {
                    var currentValue = view.GetRowCellValue(e.RowHandle, e.Column);
                    bool newValue = !Convert.ToBoolean(currentValue);
                    view.SetRowCellValue(e.RowHandle, e.Column, newValue);
                    view.RefreshRow(e.RowHandle);
                    this.ActiveControl = simpleButton1;
                }

                if (e.Column.FieldName != "imagePL") return;
                if (e.RowHandle < 0) return;
                var row = gVPL.GetDataRow(e.RowHandle);
                if (row == null) return;

                using (var dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.webp|All Files|*.*";
                    dialog.Multiselect = false;

                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                    try
                    {
                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        if (fileInfo.Length > 50 * 1024 * 1024)
                        {
                            MessageBox.Show("File ảnh quá lớn (>50MB). Vui lòng chọn ảnh nhỏ hơn.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        Image resizedImage = null;
                        using (var origin = Image.FromFile(dialog.FileName))
                        {
                            resizedImage = new Bitmap(50, 50);
                            using (var g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(origin, new Rectangle(0, 0, 50, 50), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel);
                            }
                        }

                        int dataSourceIndex = gVPL.GetDataSourceRowIndex(e.RowHandle);

                        if (imageCachePL.TryGetValue(dataSourceIndex, out Image oldImage))
                        {
                            oldImage?.Dispose();
                            imageCachePL.Remove(dataSourceIndex);
                        }

                        imageCachePL[dataSourceIndex] = resizedImage;

                        // LƯU MAP
                        string id = row["ID"].ToString();
                        idToIndexMapPL[id] = dataSourceIndex;

                        row["urlAnh"] = $"temp_{Guid.NewGuid():N}";
                        row["IsEdit"] = 1;

                        gVPL.UpdateCurrentRow();
                        gVPL.RefreshRow(e.RowHandle);
                        gVPL.RefreshRowCell(e.RowHandle, e.Column);
                    }
                    catch (OutOfMemoryException)
                    {
                        MessageBox.Show("Kích thước ảnh quá lớn hoặc định dạng không hỗ trợ. Vui lòng thử ảnh khác.",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi load ảnh: {ex.Message}", "Lỗi",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
           
        }
        private void gVNL_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            if (e.RowHandle == GridControl.AutoFilterRowHandle)
            {
                e.RowHeight = 30;
                return;
            }
            if (gVNL.IsGroupRow(e.RowHandle))
            {
                e.RowHeight = 30;
                return;
            }
            if (e.RowHeight <= 30)
                e.RowHeight = 100;
        }
        private void gVPL_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            if (e.RowHandle == GridControl.AutoFilterRowHandle)
            {
                e.RowHeight = 30;
                return;
            }
            if (gVNL.IsGroupRow(e.RowHandle))
            {
                e.RowHeight = 30;
                return;
            }
            if (e.RowHeight <= 30)
                e.RowHeight = 100;
        }

        #endregion
        #region Sửa chủng loại, khổ, bỏ khách hàng 14/10/2025
        private void searchLookUpEditViewNhom_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn47)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (searchLookUpEditViewNhom.IsGroupRow(e.RowHandle))
            {


                int groupIndex = searchLookUpEditViewNhom.GetRowLevel(e.RowHandle);
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

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void searchLookUpEditViewKV_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditViewKV_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == rcolTenNhom || info.Column == gridColumn54 || info.Column == rcolTheKhoVai)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (searchLookUpEditViewKV.IsGroupRow(e.RowHandle))
            {


                int groupIndex = searchLookUpEditViewKV.GetRowLevel(e.RowHandle);
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

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        // chỉnh màu hex cho cái grid Màu vật tư ở trên

        private void searchLookUpEdit1View_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {

            if (e.Column.FieldName != "Hex")
                return;

            string hex = e.CellValue?.ToString();
            if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#') return;

            try
            {
                Color color = ColorTranslator.FromHtml(hex);

                using (var brush = new SolidBrush(color))
                    e.Graphics.FillRectangle(brush, e.Bounds);

                e.Graphics.DrawRectangle(Pens.DarkGray, e.Bounds);
                e.Handled = true;
            }
            catch { }
        }



        private void repo_MaMauVTView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName != "Hex")
                return;

            string hex = e.CellValue?.ToString();
            if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#') return;

            try
            {
                Color color = ColorTranslator.FromHtml(hex);

                using (var brush = new SolidBrush(color))
                    e.Graphics.FillRectangle(brush, e.Bounds);

                e.Graphics.DrawRectangle(Pens.DarkGray, e.Bounds);
                e.Handled = true;
            }
            catch { }
        }
        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName != "Hex")
                return;

            string hex = e.CellValue?.ToString();
            if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#') return;

            try
            {
                Color color = ColorTranslator.FromHtml(hex);

                using (var brush = new SolidBrush(color))
                    e.Graphics.FillRectangle(brush, e.Bounds);

                e.Graphics.DrawRectangle(Pens.DarkGray, e.Bounds);
                e.Handled = true;
            }
            catch { }
        }



        private void searckLookUpEditMauVT_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

            var edit = sender as SearchLookUpEdit;
            if (edit == null) return;

            if (e.Value == null || string.IsNullOrEmpty(e.DisplayText))
            {
                e.DisplayText = "Chọn màu vật tư";
                edit.Properties.Appearance.ForeColor = Color.Red;
            }
            else
            {
                edit.Properties.Appearance.ForeColor = Color.Black;
            }
        }
        #endregion

        public enum OptionType
        {
            ChungLoai,
            SanPham,
            VatTu
        }

        private void LoadOptionSelectKV()
        {
            List<string> options = new List<string>
                    {
                        "Khổ/Size Chủng Loại",
                        "Khổ/Size Vật Tư",
                        "Size sản phẩm"

                    };

            cbxOptionSelectKV.Properties.Items.AddRange(options);

            if (cbxOptionSelectKV.Properties.Items.Count > 0)
                cbxOptionSelectKV.SelectedIndex = 0;
        }

        private OptionType CurrentSelectedOption = OptionType.ChungLoai;



        private void cbxOptionSelectKV_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbxOptionSelectKV.SelectedItem == null) return;

            string selectedText = cbxOptionSelectKV.Text.Trim();
            OptionType selectedOption;

            switch (selectedText)
            {
                case "Khổ/Size Chủng Loại":
                    selectedOption = OptionType.ChungLoai;
                    break;

                case "Size sản phẩm":
                    selectedOption = OptionType.SanPham;
                    break;

                case "Khổ/Size Vật Tư":
                    selectedOption = OptionType.VatTu;
                    break;

                default:
                    return;
            }




            CurrentSelectedOption = selectedOption;
            CreateSearchLookUpKhoVai();
        }



        private void loadLocChungLoai()
        {
            try
            {
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETLOCCHUNGLOAI";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;

                searchLookUpEditLocCL.Properties.DisplayMember = "ChungLoaiVatTu";
                searchLookUpEditLocCL.Properties.ValueMember = "MaCLVT";
                searchLookUpEditLocCL.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {
            }

        }



        private void searchLookUpEditLocCL_EditValueChanged(object sender, EventArgs e)
        {
            loadVatTu();
        }
        private void txtFind_EditValueChanged(object sender, EventArgs e)
        {
            string searchText = txtFind.Text.Trim();

            FilterGridView(gVNL, searchText);
            FilterGridView(gVPL, searchText);
        }


        private void FilterGridView(DevExpress.XtraGrid.Views.Grid.GridView gridView, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                gridView.ActiveFilterString = "";
                return;
            }

            List<string> conditions = new List<string>();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView.Columns)
            {
                if (col.Visible)
                {
                    // Escape dấu nháy đơn để tránh lỗi SQL
                    string escapedText = searchText.Replace("'", "''");
                    conditions.Add($"Contains([{col.FieldName}], '{escapedText}')");
                }
            }

            if (conditions.Count > 0)
            {
                gridView.ActiveFilterString = string.Join(" OR ", conditions);
            }
        }


        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("ThongSoVatTu{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateThongSoVatTu.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                ExportThongSo(TemplateFileName, ExportFileName);

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
        public void ExportThongSo(string TemplateFileName, string ExportFileName)
        {
            try
            {


                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "ThongSoVatTu";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    //DataTable tblNL = gCNL.DataSource as DataTable;
                    //DataTable tblPL = gCPL.DataSource as DataTable;
                    DataTable tblNL = GetFilteredDataTable(gVNL);
                    DataTable tblPL = GetFilteredDataTable(gVPL);
                    DataTable tblNPL = new DataTable();
                    if (tblNL != null)
                    {
                        tblNPL = tblNL.Clone();

                        foreach (DataRow row in tblNL.Rows)
                            tblNPL.ImportRow(row);
                    }

                    if (tblPL != null)
                    {
                        if (tblNPL.Columns.Count == 0)
                            tblNPL = tblPL.Clone();

                        foreach (DataRow row in tblPL.Rows)
                            tblNPL.ImportRow(row);
                    }

                    if (tblNPL == null || tblNPL.Rows.Count == 0)
                    {

                        excelPackage.Save();
                        return;
                    }

                    int checkedCount = tblNPL.AsEnumerable()
                            .Count(r => r.Field<bool?>("IsCheckExcel") == true);
                    if (checkedCount > 0)
                    {
                        DataTable filteredTable = tblNPL.AsEnumerable()
                                           .Where(r => r.Field<bool?>("IsCheckExcel") == true)
                                           .CopyToDataTable();

                        // Gán lại vào tblNPL
                        tblNPL = filteredTable;
                    }

                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    worksheet.Cells["A1"].Value = "CÔNG TY TNHH VIKING VIỆT NAM";
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.Font.Size = 12;

                    int headerRow = 3;

                    worksheet.Cells[headerRow, 1].Value = "CHỦNG LOẠI VẬT TƯ";
                    worksheet.Cells[headerRow, 2].Value = "ITEMCODE";
                    worksheet.Cells[headerRow, 3].Value = "MÔ TẢ";
                    worksheet.Cells[headerRow, 4].Value = "CODE MÀU VT";
                    worksheet.Cells[headerRow, 5].Value = "MÀU VT";
                    worksheet.Cells[headerRow, 6].Value = "KHỔ/SIZE";
                    worksheet.Cells[headerRow, 7].Value = "ĐƠN VỊ KHỔ/SIZE";
                    worksheet.Cells[headerRow, 8].Value = "ĐƠN VỊ VT";
                    worksheet.Cells[headerRow, 9].Value = "GHI CHÚ";

                    using (var range = worksheet.Cells[headerRow, 1, headerRow, 9])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    int startRow = headerRow + 1;
                    int currentRow = startRow;

                    foreach (DataRow row in tblNPL.Rows)
                    {
                        worksheet.Cells[currentRow, 1].Value = row["TenNhom"];
                        worksheet.Cells[currentRow, 2].Value = row["MaVT"];
                        worksheet.Cells[currentRow, 3].Value = row["ChiTiet"];
                        worksheet.Cells[currentRow, 4].Value = row["MaMauVT"];
                        worksheet.Cells[currentRow, 5].Value = row["MauVT"];
                        worksheet.Cells[currentRow, 6].Value = row["KhoVai"];
                        worksheet.Cells[currentRow, 7].Value = row["TenDVKV"];
                        worksheet.Cells[currentRow, 8].Value = row["TenDVVT"];
                        worksheet.Cells[currentRow, 9].Value = row["GhiChu"];

                        currentRow++;
                    }
                    int lastRow = currentRow - 1;
                    var tableRange = worksheet.Cells[headerRow, 1, lastRow, 9];
                    var table = worksheet.Tables.Add(tableRange, "TableThongSoVT");
                    table.TableStyle = OfficeOpenXml.Table.TableStyles.None;
                    table.ShowHeader = true;
                    table.ShowFilter = true;
                    table.ShowTotal = false;
                    table.ShowFirstColumn = false;
                    table.ShowLastColumn = false;
                    using (var headerCells = worksheet.Cells[headerRow, 1, headerRow, 9])
                    {
                        headerCells.Style.Font.Bold = true;
                        headerCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    }
                    worksheet.Column(1).Width = 25;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 60;
                    worksheet.Column(4).Width = 18;
                    worksheet.Column(5).Width = 25;
                    worksheet.Column(6).Width = 18;
                    worksheet.Column(7).Width = 18;
                    worksheet.Column(8).Width = 18;
                    worksheet.Column(9).Width = 20;
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }
        DataTable GetFilteredDataTable(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            DataTable result = ((DataView)view.DataSource).Table.Clone();

            for (int i = 0; i < view.RowCount; i++)
            {
                int rowHandle = view.GetVisibleRowHandle(i);
                if (rowHandle < 0) continue;

                DataRow row = view.GetDataRow(rowHandle);
                if (row != null)
                    result.ImportRow(row);
            }

            return result;
        }
        #region Thêm cột

        private void repotxtTLC_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repotxtDungSai_KeyPress(object sender, KeyPressEventArgs e)
        {
            //setTypeTxtEditDungSai(sender, e);
        }
        private void repotxtTLCPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repotxtDungSaiPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            //setTypeTxtEdit(sender, e);
        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }
            if (currentText.Contains("."))
            {
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);
                if (decimalPart.Length >= 4 && e.KeyChar != '\b') // '\b' là phím Backspace
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
            }
        }
        private async void gVNL_DoubleClick(object sender, EventArgs e)
        {
            var focusRow = gVNL.FocusedRowHandle;
            if (focusRow < 0) return;
            var fieldName = gVNL.FocusedColumn.FieldName;
            var drFocus = gVNL.GetFocusedDataRow();
            if (fieldName == "SpecificationPDF" || fieldName == "TestReportPDF" || fieldName == "OekotexPDF")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFileName = openFileDialog.FileName;
                    var fileName = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName));

                    var result = await UploadPDF(ReplaceSpecialCharacterssize(drFocus["MaNhom"].ToString()), sourceFileName, fileName + fieldName.ToString());
                    if (result)
                    {
                        gVNL.SetRowCellValue(gVNL.FocusedRowHandle, fieldName, fileName + fieldName.ToString() + ".pdf");
                        SaveTaiLieu();
                    }
                }
            }
        }

      

        private async void gVPL_DoubleClick(object sender, EventArgs e)
        {
            var focusRow = gVPL.FocusedRowHandle;
            if (focusRow < 0) return;
            var fieldName = gVPL.FocusedColumn.FieldName;
            var drFocus = gVPL.GetFocusedDataRow();
            if (fieldName == "SpecificationPDF" || fieldName == "TestReportPDF" || fieldName == "OekotexPDF")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFileName = openFileDialog.FileName;
                    var fileName = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName));

                    var result = await UploadPDF(ReplaceSpecialCharacterssize(drFocus["MaNhom"].ToString()), sourceFileName, fileName + fieldName.ToString());
                    if (result)
                    {
                        gVPL.SetRowCellValue(gVPL.FocusedRowHandle, fieldName, fileName + fieldName.ToString() + ".pdf");
                        SaveTaiLieu(false);
                    }
                }
            }
        }

      
        private async Task<bool> UploadPDF(string TenTaiLieu, string sourceFileName, string fileName)
        {
            string Url = "";

            var client = new WebClient();




            Url = string.Format($"{URL}ERPVatTuBOM/UploadFilePDF");
            var uri = new Uri(Url);
            try
            {
                client.Headers.Add("folderName", TenTaiLieu);
                client.Headers.Add("fileName", (fileName));
                var data = System.IO.File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(uri, data);
                string result = Encoding.UTF8.GetString(responseBytes);
                if (result.ToUpper() == "TRUE")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void SaveTaiLieu(bool isNL = true)
        {
            DataRow dr;
            if (isNL)
            {
                dr = gVNL.GetFocusedDataRow();
            }
            else
            {
                dr = gVPL.GetFocusedDataRow();
            }
            tblmvt = CreateTableSaveMauVT();




            //POSTVATTUTV t1
            string khachhang = "";
            string mahang = "";
            DataTable tblthongsovt = CreateTableSaveThongSo();


            tblthongsovt.Clear();
            if (!tblthongsovt.Columns.Contains("MaMau"))
            {
                tblthongsovt.Columns.Add("MaMau", typeof(string));
            }



            DataRow rowthongso2 = tblthongsovt.NewRow();
            rowthongso2["ID"] = dr["ID"];
            rowthongso2["MaHang"] = mahang;
            rowthongso2["MaKH"] = khachhang;
            rowthongso2["MaNhom"] = dr["MaNhom"].ToString();
            rowthongso2["MaVTID"] = dr["MaVTID"].ToString();
            rowthongso2["MaVT"] = dr["MaVT"].ToString();
            rowthongso2["ChiTiet"] = dr["ChiTiet"].ToString();
            rowthongso2["MaDVVT"] = dr["MaDVVT"].ToString();
            //rowthongso["TenDVVT"] = row["TenDVVT"].ToString();
            rowthongso2["MauVTID"] = dr["MauVTID"].ToString();
            rowthongso2["MaMauVT"] = dr["MaMauVT"].ToString();
            rowthongso2["KhoVaiID"] = dr["KhoVaiID"].ToString();
            //rowthongso["KhoVai"] = row["KhoVai"].ToString();
            rowthongso2["MauVT"] = dr["MauVT"].ToString();
            rowthongso2["MaMau"] = "";
            rowthongso2["GhiChu"] = dr["GhiChu"].ToString();
            rowthongso2["MaVTGhep"] = dr["MaVTGhep"].ToString();
            rowthongso2["urlAnh"] = dr["urlAnh"].ToString();
            rowthongso2["MaTheSize"] = dr["MaTheSize"].ToString();
            rowthongso2["IsKV"] = dr["IsKV"];

            rowthongso2["OekotexPDF"] = dr["OekotexPDF"].ToString();
            rowthongso2["TestReportPDF"] = dr["TestReportPDF"].ToString();
            rowthongso2["SpecificationPDF"] = dr["SpecificationPDF"].ToString();

            tblthongsovt.Rows.Add(rowthongso2);




            if (tblthongsovt.Columns.Contains("MaMau"))
            {
                tblthongsovt.Columns.Remove("MaMau");
            }

            string url4 = $"{URL}ERPVatTuBOM/PostT1?Action=POSTPDF&para={GlobleData.UserName}&para2=frmERP_VatTu";
            string msResult4 = Task.Run(async () => { return await _clientExtension.PostAsync(url4, tblthongsovt); }).Result;
            if (msResult4.ToLower() != "true") return;

            clsWaitForm.ShowSuccessForm(this, 3000);




        }

       
        private string ReplaceSpecialCharacterssize(string input)
        {
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement).ToUpper();
        }

        private void xemOekotexPDF(object sender, EventArgs e)
        {
            try
            {

                var dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr is null) return;
                var tailieu = dr["MaNhom"].ToString();
                var filename = dr["OekotexPDF"].ToString();
                string objUrl = (string)settingsReader.GetValue("HostDH", typeof(String));
              
                if (!string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        string url = $"{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
                      
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không mở được ảnh: " + ex.Message);
                    }
                }



            }
            catch (Exception ex)
            {


            }

        }
        private void xemTestReportPDF(object sender, EventArgs e)
        {
            try
            {
                var dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr is null) return;
                var tailieu = dr["MaNhom"].ToString();
                var filename = dr["TestReportPDF"].ToString();
                string objUrl = (string)settingsReader.GetValue("HostDH", typeof(String));

                if (!string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        string url = $"{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không mở được ảnh: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void xemSpecificationPDF(object sender, EventArgs e)
        {
            try
            {
                var dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr is null) return;
                var tailieu = dr["MaNhom"].ToString();
                var filename = dr["SpecificationPDF"].ToString();
                string objUrl = (string)settingsReader.GetValue("HostDH", typeof(String));

                if (!string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        string url = $"{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không mở được ảnh: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }
        #endregion


        #region Vật tư thành phần
        private void PopupVatTuThanhPhan()
        {
            DataRow rowVatTuFocused = null;
            if(tabVT.SelectedTabPage  == tabNL)
            {
                rowVatTuFocused = gVNL.GetFocusedDataRow();

            }
            else if(tabVT.SelectedTabPage == tabPL)
            {
                rowVatTuFocused = gVPL.GetFocusedDataRow();
            }
            if( rowVatTuFocused != null)
            {
                frmERPVatTuThanhPhan frm = new frmERPVatTuThanhPhan(rowVatTuFocused);
                frm.FormClosed += (s, e) => btnNapLai.PerformClick();
                frm.Show();
            }
        }

        #endregion
        #region Thêm DVSD,Tỉ lệ, HSCode 16/4/2026
        private void LoadDVSD(DataTable tbl)
        {
            if (tbl == null || tbl.Rows.Count == 0) return;
            repoDVSDNL.DisplayMember = "TenDVVT";
            repoDVSDNL.ValueMember = "MaDVVT";
            repoDVSDPL.DisplayMember = "TenDVVT";
            repoDVSDPL.ValueMember = "MaDVVT";

            repoDVSDNL.DataSource = tbl;
            repoDVSDPL.DataSource = tbl;
        }
        private void repoTileNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEditTile(sender, e);
        }
        private void repoTilePL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEditTile(sender, e);
        }
        private void setTypeTxtEditTile(object sender, KeyPressEventArgs e)
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
        #endregion

        #region Thêm ngân hàng giá vật tư -- 2024-04-24

        private void btnNganHangGia_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERP_VatTuNganHangGia frm = new frmERP_VatTuNganHangGia();
            frm.ShowDialog();
        }

        #endregion

        private void checkDupItemcode(string itemcode)
        {
            try
            {
                DataRow dr = tblItemCode.AsEnumerable().Where(x => x["MaVT"].ToString().ToUpper() == itemcode).FirstOrDefault();
                if (dr == null)
                {
                    txtChiTiet.ReadOnly = false;
                    txtChiTiet.ForeColor = Color.Black;
                    return;
                }

                string _mota = dr["ChiTiet"].ToString();
                txtChiTiet.Text = dr["ChiTiet"].ToString();

                txtChiTiet.ReadOnly = true;
                txtChiTiet.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {

              
            }
           
        }
    }
}
