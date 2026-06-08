using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
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
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDonHangChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        SearchCheckSelection gridCheckMarksSizeType;
        SearchCheckSelection gridCheckMarksColor;
        SearchCheckSelection gridCheckMarksSize;
        SearchCheckSelection gridCheckMarksPO;
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        public bool _allowAdd = true, _allowEdit = true, _allowDelete = true, _allowMess = false, _checkKHSX = false, chkKHSX = true, IsCheckClose = false;
        private string _madh = string.Empty, _malenhsanxuat = string.Empty, _tenlenh = string.Empty, _magop = string.Empty, _mahang = string.Empty, _tenhang = string.Empty, _dot = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty, _poid = string.Empty, _poidAll = string.Empty, selectedValuesMH = string.Empty, _mh = string.Empty, selectedValuessMH = string.Empty, _madonhang = string.Empty, _sttlenh = string.Empty, _madhgop = string.Empty;
        private int _valueSum = 0, _valueConLai = 0, _totalSum = 0, _iskeove = 0;
        DataTable dtSizeID;
        DataTable _dtData;
        DataTable _dttemp;
        DataTable _dttemp_1;
        DataTable dtSize;
        DataTable dtdh;
        bool indicatorIcon = true;
        int checkrong = 0;
        string _sizeID = string.Empty;
        private string selectedValuessPO = string.Empty, selectedValuessDS = string.Empty, selectedValuessMau = string.Empty, selectedValuessSize = string.Empty, _Line = string.Empty;
        private string selectedValuesPO = string.Empty, selectedValuesDS = string.Empty, selectedValuesMau = string.Empty, selectedValuesSize = string.Empty;
        KeyDownControlHandler keyDownControlHandler;
        bool _isEdit;
        string _maSX, _dotSX, _maDVSX;
        public frmCanDoiDonHangChiTiet(bool isEdit, string maDH, string maHang, string TenHang, string Dot, string poID, string maSX, string dotSX, string maDVSX, string malenhsanxuat, string magop, string tenlenh, string sttlenh, int iskeove, string madhgop = "", string line = "")
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._madh = maDH;
            this._mahang = maHang;
            this._tenhang = TenHang;
            this._dot = Dot;
            this._poid = poID;
            this._maSX = maSX;
            this._dotSX = dotSX;
            this._maDVSX = maDVSX;
            this._malenhsanxuat = malenhsanxuat;
            this._magop = magop;
            this._tenlenh = tenlenh;
            this._sttlenh = sttlenh;
            this._iskeove = iskeove;
            this._Line = line;
            dtSizeID = new DataTable();
            _dtData = new DataTable();
            _dttemp_1 = new DataTable();
            _dttemp = new DataTable();
            dtSize = new DataTable();
            dtdh = new DataTable();
            _isEdit = isEdit;
            this._madhgop = madhgop;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
           
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            splitContainerControl1.SplitterPosition = ((int)(splitContainerControl1.Width * 3.2 / 4));
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

            if (!_allowAdd)
            {
                //btChiaSX.Enabled = false;
            }
            if (!_allowEdit)
            {
                btEdit.Enabled = false;
            }
            if (!_allowDelete)
                btDelete.Enabled = false;
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, Luu, true, ActionType.Save);
            AddActionControl(_lstActionControl, CreateDefault, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void CreateDefault()
        {
            string url = string.Format("{0}?", URL + "CanDoiDonHangTong/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtdh = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_DonHang.Properties.DataSource = dtdh;
            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.DisplayMember = "MaHang";
            searchLookUpEdit_DonHang.Properties.NullText = "----Chọn Mã Hàng----";
            if (_isEdit)
            {

                Init();
                GetLine();
                searchLookUpEdit_DonHang.Text = _madhgop;
                //searchLookUpEdit_DonHang.EditValue = _madhText;

            }
            searchLookUpEditPO.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditPO_CustomDisplayText);
            searchLookUpEditSizeType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditSizeType_CustomDisplayText);
            searchLookUpEditColor.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditColor_CustomDisplayText);
            searchLookUpEdit_Size.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditSize_CustomDisplayText);

            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            searchLookUpEdit_DVSX.Properties.DataSource = dtDVSX;
            searchLookUpEdit_DVSX.Properties.ValueMember = "MaDVSX";
            searchLookUpEdit_DVSX.Properties.DisplayMember = "TenDVSX";
            searchLookUpEdit_DVSX.Properties.NullText = "[Chọn DVSX]";
            searchLookUpEdit_DVSX.Properties.Appearance.ForeColor = Color.Red;

            this.repositoryItemSearchLookUpDVSX.DataSource = dtDVSX;

            if (_isEdit)
            {
                this.txtLenhSX.Text = _maSX;
                this.txtLenhSX.ReadOnly = true;
                this.txtDot.Text = _dotSX;
                this.txtDot.ReadOnly = true;
                this.searchLookUpEdit_DVSX.EditValue = _maDVSX;
                this.searchLookUpEdit_DVSX.ReadOnly = true;
                this.searchLookUpEdit_DonHang.ReadOnly = true;
                gridViewCongDoan.OptionsBehavior.Editable = false;
            }
        }

        private void LoadDSCongDoan(string maDVSXInit)
        {
            if (!_isEdit)
                gridControlCongDoan.DataSource = InitDataCongDoan(maDVSXInit);
            else
            {

                // _madh | _malenhsanxuat | _madvsx
                string url = string.Format("{0}?maDH={1}&&maLenhSX={2}", URL + "CongDoan/GetCongDoanAllowCondition", _magop, _malenhsanxuat);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                DataTable _tblCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
                if (_tblCongDoan != null && _tblCongDoan.Rows.Count > 0)
                {
                    gridControlCongDoan.DataSource = _tblCongDoan;
                }
                else
                {
                    gridControlCongDoan.DataSource = InitDataCongDoan(maDVSXInit);
                }

            }
        }

        private DataTable InitDataCongDoan(string maDVSXInit)
        {
            DataTable tblInitCongDoan = CreateTableInitCongDoan();
            DataRow rowInitCongDoan = tblInitCongDoan.NewRow();

            bool _isGiaCong = (bool)(repositoryItemSearchLookUpDVSX.DataSource as DataTable).AsEnumerable()
                .Where(x => x["MaDVSX"].ToString().Equals(maDVSXInit)).FirstOrDefault()["GiaCong"];

            rowInitCongDoan["MaDVSXCat"] = maDVSXInit;
            rowInitCongDoan["MaDVSXMay"] = maDVSXInit;
            if (!_isGiaCong)
            {
                rowInitCongDoan["MaDVSXHoanThanh"] = maDVSXInit;
                rowInitCongDoan["MaDVSXDongThung"] = maDVSXInit;
            }
            rowInitCongDoan["NguoiTao"] = GlobleData.UserName;

            tblInitCongDoan.Rows.Add(rowInitCongDoan);

            return tblInitCongDoan;
        }

        private DataTable CreateTableInitCongDoan()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaLenhSX", typeof(string));
            tbl.Columns.Add("MaDVSXCat", typeof(string));
            tbl.Columns.Add("MaDVSXMay", typeof(string));
            tbl.Columns.Add("MaDVSXHoanThanh", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("MaDVSXDongThung", typeof(string));
            return tbl;
        }

        private DataTable createTableSLKH()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("SoLuongSX", typeof(int));
            tbl.Columns.Add("CheckTruTK", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        void searchLookUpEdit_DonHang_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            // checkrong = 1;

        }
        void searchLookUpEditPO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessPO = string.Join("; ", gridView5.GetSelectedRows().Select(rowHandle => gridView5.GetRowCellValue(rowHandle, searchLookUpEditPO.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessPO.ToString()))
            {
                e.DisplayText = "----Chưa chọn PO----";
            }
            else
            {
                e.DisplayText = selectedValuessPO.ToString();
            }

        }
        void searchLookUpEditSizeType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessDS = string.Join("; ", searchLookUpEdit4View.GetSelectedRows().Select(rowHandle => searchLookUpEdit4View.GetRowCellValue(rowHandle, searchLookUpEditSizeType.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessDS.ToString()))
            {
                e.DisplayText = "----Chưa chọn InSeam----";
            }
            else
            {
                e.DisplayText = selectedValuessDS.ToString();
            }

        }
        void searchLookUpEditPO_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.ActiveControl;
            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
                {
                    c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
                }
            }
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["POID"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();

            }
        }
        void searchLookUpEditSizeType_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.ActiveControl;
            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
                {
                    c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
                }
            }
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["DauSizeID"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();

            }
        }
        void searchLookUpEditColor_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessMau = string.Join("; ", searchLookUpEdit3View.GetSelectedRows().Select(rowHandle => searchLookUpEdit3View.GetRowCellValue(rowHandle, searchLookUpEditColor.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessMau.ToString()))
            {
                e.DisplayText = "----Chưa chọn Màu----";
            }
            else
            {
                e.DisplayText = selectedValuessMau.ToString();
            }
        }

        void searchLookUpEditSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessSize = string.Join("; ", gridView4.GetSelectedRows().Select(rowHandle => gridView4.GetRowCellValue(rowHandle, searchLookUpEdit_Size.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessSize.ToString()))
            {
                e.DisplayText = "----Chưa chọn size----";
            }
            else
            {
                e.DisplayText = selectedValuessSize.ToString();
            }

        }
        private void searchLookUpEdit_DonHang_EditValueChanged(object sender, EventArgs e)
        {
            chkv_AllData.Checked = false;
            if (searchLookUpEdit_DonHang.EditValue != null)
            {
                _madh = searchLookUpEdit_DonHang.EditValue.ToString();
                _poidAll = "";

                string urlPO = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetPO", _madh);
                string jsonPO = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPO); }).Result;
                DataTable dtPO = JsonConvert.DeserializeObject<DataTable>(jsonPO);

                for (int i = 0; i < dtPO.Rows.Count; i++)
                {
                    if (_poidAll != "")
                    {
                        _poidAll = _poidAll + ";";
                    }
                    _poidAll = _poidAll + dtPO.Rows[i]["POID"].ToString();
                }
                searchLookUpEditPO.Properties.DataSource = dtPO;
                searchLookUpEditPO.Properties.DisplayMember = "PO";
                searchLookUpEditPO.Properties.ValueMember = "POID";
                searchLookUpEditPO.Properties.ShowClearButton = false;
                searchLookUpEditPO.Properties.NullText = "[Chọn PO]";
                searchLookUpEditPO.Properties.Appearance.ForeColor = Color.Red;
                GridView dvView = searchLookUpEditPO.Properties.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "POID", Caption = "POID", Name = "colPOID", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "PO", Caption = "PO", Name = "colPO", Visible = true });

                }
            }
        }

        private void SearchLookUpEditPO_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            // e.DisplayText = selectedValuesPO;
        }

        private void searchLookUpEditSizeType_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditSizeType.EditValue != null)
            {
                _dausize = searchLookUpEditSizeType.EditValue.ToString();

                string urlmau = string.Format("{0}?madh={1}&&dausizeID={2}", URL + "CanDoiDonHangTong/GetMau", _madh, _dausize);
                string jsonmau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmau); }).Result;
                DataTable dtmau = JsonConvert.DeserializeObject<DataTable>(jsonmau);
                _colorIDALL = "";
                for (int i = 0; i < dtmau.Rows.Count; i++)
                {
                    if (_colorIDALL != "")
                    {
                        _colorIDALL = _colorIDALL + ";";
                    }
                    _colorIDALL = _colorIDALL + dtmau.Rows[i]["MaMau"].ToString();
                }
                searchLookUpEditColor.Properties.DataSource = dtmau;
                searchLookUpEditColor.Properties.ValueMember = "MaMau";
                searchLookUpEditColor.Properties.DisplayMember = "TenMau";
                searchLookUpEditColor.Properties.NullText = "[Chọn Màu]";
                searchLookUpEditColor.Properties.Appearance.ForeColor = Color.Red;
                searchLookUpEditColor.Properties.ShowClearButton = false;
                GridView dvView = searchLookUpEditColor.Properties.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaMau", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Màu", Name = "colTenMau", Visible = true });

                }

            }
        }

        void searchLookUpEditColor_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.ActiveControl;
            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
                {
                    c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
                }
            }
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["MaMau"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
        }

        void searchLookUpEditSize_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.ActiveControl;
            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
                {
                    c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
                }
            }
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["SizeID"].ToString());
                }

                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
        }
        private void searchLookUpEditColor_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditColor.EditValue != null)
            {
                _mamau = searchLookUpEditColor.EditValue.ToString();
                string urlSize = string.Format("{0}?madh={1}&&dausizeID={2}&&mamau={3}", URL + "CanDoiDonHangTong/GetSizeID", _madh, _dausize, _mamau);
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                searchLookUpEdit_Size.Properties.DataSource = dtSize;
                searchLookUpEdit_Size.Properties.DisplayMember = "Size";
                searchLookUpEdit_Size.Properties.ValueMember = "SizeID";
                searchLookUpEdit_Size.Properties.ShowClearButton = false;
                searchLookUpEdit_Size.Properties.NullText = "[Chọn size]";
                searchLookUpEdit_Size.Properties.Appearance.ForeColor = Color.Red;
                GridView dvView = searchLookUpEdit_Size.Properties.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "SizeID", Caption = "Size ID", Name = "colSizeID", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "Size", Caption = "Size", Name = "colSize", Visible = true });

                }

            }
        }

        private void btnn_xacnhan_Click(object sender, EventArgs e)
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            LoadDataGrd_2();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

        }

        private void LoadDataGrd_2()
        {
            try
            {
                if (!chkv_AllData.Checked)
                {
                    if (string.IsNullOrEmpty(_madh))
                    {
                        MessageBox.Show("Vui lòng chọn đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_poid))
                        {
                            MessageBox.Show("Vui lòng chọn PO.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(_dausize))
                            {
                                MessageBox.Show("Vui lòng chọn InSeam.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(_mamau))
                                {
                                    MessageBox.Show("Vui lòng chọn màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                    }
                }
                string urlListSize = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetListSize", _madh);
                string jsonListSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlListSize); }).Result;
                dtSizeID = JsonConvert.DeserializeObject<DataTable>(jsonListSize);

                string urlSLDH = string.Format("{0}?madh={1}&&dausizeid={2}&&mamau={3}&&poid={4}", URL + "CanDoiDonHangTong/GetSoLuongDH", _madh, _dausize, _mamau, _poid);
                string jsonSLDH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSLDH); }).Result;
                _dtData = JsonConvert.DeserializeObject<DataTable>(jsonSLDH);

                if (!chkv_AllData.Checked)
                {
                    string[] sizes = null;
                    if (selectedValuesSize.ToString() != "")
                    {
                        sizes = selectedValuesSize.Split(';');
                    }
                    if (!string.IsNullOrEmpty(selectedValuesSize))
                    {
                        var dtTemp = dtSizeID.AsEnumerable().Where(x => sizes.Contains(x["SizeID"].ToString()));
                        dtSizeID = dtTemp.Count() > 0 ? dtTemp.CopyToDataTable() : new DataTable();
                    }
                    dtSizeID.AcceptChanges();

                    if (!string.IsNullOrEmpty(selectedValuesSize))
                    {
                        var dtTemp = _dtData.AsEnumerable().Where(x => sizes.Contains(x["SizeID"].ToString()));
                        _dtData = dtTemp.Count() > 0 ? dtTemp.CopyToDataTable() : new DataTable();
                    }
                    _dtData.AcceptChanges();
                }
                _dttemp.Clear();
                RemoveColumnsWithAtSign(_dttemp, gridView2);
                TaoCot(_dttemp, gridView2);
                _dttemp = getDataTemp(_dttemp, false);
                gridControl2.DataSource = _dttemp;
                gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void TaoCot(DataTable _dt, GridView grdV)
        {
            if (_dt.Columns.Count == 0)
            {
                _dt.Columns.Add("MaDH", typeof(string));
                _dt.Columns.Add("MaHang", typeof(string));
                _dt.Columns.Add("POID", typeof(string));
                _dt.Columns.Add("PO", typeof(string));
                _dt.Columns.Add("MaQG", typeof(string));
                _dt.Columns.Add("MaMau", typeof(string));
                _dt.Columns.Add("TenMau", typeof(string));
                _dt.Columns.Add("DauSizeID", typeof(string));
                _dt.Columns.Add("DauSize", typeof(string));
            }

            int _valuIndedx = 8;
            for (int i = 0; i < dtSizeID.Rows.Count; i++)
            {
                string sizeID = dtSizeID.Rows[i]["SizeID"].ToString() + "@Size@" + dtSizeID.Rows[i]["Size"].ToString();
                _dt.Columns.Add(sizeID, typeof(int));

                int columnIndexToInsert = 8; // Vị trí cột cần chèn (cột thứ 4)
                _valuIndedx = _valuIndedx + 1;
                GridColumn newColumn = new GridColumn
                {
                    FieldName = sizeID,
                    Caption = dtSizeID.Rows[i]["Size"].ToString(),
                    Name = "Size@" + dtSizeID.Rows[i]["SizeID"].ToString(),
                    Visible = true,
                    Width = 50,
                    VisibleIndex = _valuIndedx,

                };
                grdV.Columns.Insert(columnIndexToInsert, newColumn);
                string[] arrName = newColumn.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (grdV.Name == "gridView1")
                {
                    var slkh = "";
                    var slth = "";
                    if (arrName.Length > 1)
                    {
                        string tag1 = newColumn.FieldName + "|TT";
                        GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                        item1.Tag = tag1;
                        string tag2 = newColumn.FieldName + "|RM";
                        GridColumnSummaryItem item2 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                        item2.Tag = tag2;
                    }


                }
                else
                {
                    if (arrName.Length > 1)
                    {
                        string tag1 = newColumn.FieldName + "|KH";
                        GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                        item1.Tag = tag1;
                    }
                }

            }
            // Kiểm tra xem cột có tồn tại trong GridView
            GridColumn existingColumn = grdV.Columns["Amount"]; // name là tên của cột cần kiểm tra

            if (existingColumn != null)
            {
                // Xóa cột từ GridView
                grdV.Columns.Remove(existingColumn);
            }
            GridColumn newColumnS = new GridColumn();
            newColumnS.FieldName = "Amount"; // Tên trường dữ liệu
            newColumnS.Caption = "Tổng"; // Tiêu đề cột
            newColumnS.Visible = true; // Có hiển thị cột hay không
            newColumnS.Width = 40; // Độ rộng của cột
            newColumnS.OptionsColumn.ReadOnly = false;
            newColumnS.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            newColumnS.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            newColumnS.Summary.Add(DevExpress.Data.SummaryItemType.Sum, newColumnS.FieldName, "{0:n0}");

            grdV.Columns.Add(newColumnS);

            if (grdV.Name == "gridView1")
            {
                if (colSizeType_2.Summary.Count == 0)
                {
                    colSizeType_2.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "Size", "SLTH");
                    colSizeType_2.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "Size", "Còn lại");
                }
                string tag2Tong = newColumnS.FieldName + "|RM";
                GridColumnSummaryItem item2Tong = newColumnS.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumnS.FieldName, "{0:n0}");
                item2Tong.Tag = tag2Tong;

            }
            else
            {
                if (colSizeType_1.Summary.Count == 0)
                {
                    colSizeType_1.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "Size", "SLKH");
                }
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
        private void RemoveColumnsWithAtSign(DataTable dataTable, GridView grdV)
        {
            // Duyệt qua các cột của DataTable
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                DataColumn column = dataTable.Columns[i];

                // Kiểm tra xem tên của cột có chứa ký tự "@" không
                int numberOfColumns = grdV.Columns.Count;

                if (column.ColumnName.ToString().Contains("@"))
                {
                    if (grdV.Columns[column.ColumnName] != null)
                    {
                        grdV.Columns.Remove(grdV.Columns[column.ColumnName]);
                    }
                    // Nếu có, xóa cột
                    dataTable.Columns.Remove(column);
                }
            }
        }

        private DataTable getDataTemp(DataTable DTemp, bool IsGrd)
        {
            for (int i = 0; i < _dtData.Rows.Count; i++)
            {
                string _Sizeheader = _dtData.Rows[i]["SizeID"].ToString() + "@Size@" + _dtData.Rows[i]["Size"].ToString();
                // Lọc dữ liệu từ DataTable
                string filterExpression = $"MaDH = '{_dtData.Rows[i]["MaDH"].ToString()}' AND POID = '{_dtData.Rows[i]["POID"].ToString()}' AND MaMau = '{_dtData.Rows[i]["MaMau"].ToString()}' AND DauSizeID = '{_dtData.Rows[i]["DauSizeID"].ToString()}'";
                DataRow[] filteredRows = DTemp.Select(filterExpression);
                if (filteredRows.Length == 0)
                {

                    DataRow _dr = DTemp.NewRow();
                    _dr["MaDH"] = _dtData.Rows[i]["MaDH"].ToString();
                    _dr["MaHang"] = _dtData.Rows[i]["MaHang"].ToString();
                    _dr["POID"] = _dtData.Rows[i]["POID"].ToString();
                    _dr["PO"] = _dtData.Rows[i]["PO"].ToString();
                    _dr["MaQG"] = _dtData.Rows[i]["MaQG"].ToString();
                    _dr["MaMau"] = _dtData.Rows[i]["MaMau"].ToString();
                    _dr["TenMau"] = _dtData.Rows[i]["TenMau"].ToString();
                    _dr["DauSizeID"] = _dtData.Rows[i]["DauSizeID"].ToString();
                    _dr["DauSize"] = _dtData.Rows[i]["DauSize"].ToString();
                    {
                        _dr[_Sizeheader] = _dtData.Rows[i]["SoLuong"].ToString() == "" ? 0 : Convert.ToInt32(_dtData.Rows[i]["SoLuong"].ToString());
                    }
                    DTemp.Rows.Add(_dr);
                }
                else
                {
                    DataRow foundRow = filteredRows[0];
                    foundRow[_Sizeheader] = _dtData.Rows[i]["SoLuong"].ToString() == "" ? 0 : Convert.ToInt32(_dtData.Rows[i]["SoLuong"].ToString());
                }
            }
            return DTemp;
        }

        private void btn_LayDuLieu_Click(object sender, EventArgs e)
        {
            DataTable _dt = gridControl2.DataSource as DataTable;
            if (_dt != null)
            {
                LoadDataGrd_1();
            }
        }

        private void LoadDataGrd_1()
        {
            if (!_isEdit)
            {
                GetMaLenhSX();
                // lấy lệnh sx
                string maxLenhSXID = Task.Run(async () =>
                {
                    return await _clientExtension.GetAsnyc(URL + "CanDoiDonHangTong/GetMaxDH");

                }).Result;
                if (maxLenhSXID == "" || Convert.ToInt32(maxLenhSXID) == 0)
                    txtLenhSX.Text = "SX_1";
                else
                    txtLenhSX.Text = "SX_" + (Convert.ToInt32(maxLenhSXID) + 1).ToString().Trim();
                txtDot.Text = txtLenhSX.Text.ToString().Trim() + "-DOT-" + DateTime.Now.Day.ToString();
            }
            else
            {
                txtLenhSX.Text = _maSX;
                txtDot.Text = _dotSX;
                searchLookUpEdit_DVSX.EditValue = _maDVSX;
                txtLenhSX.Enabled = false;
                txtDot.Enabled = false;
                searchLookUpEdit_DVSX.Enabled = false;

            }
            _dttemp_1.Clear();
            RemoveColumnsWithAtSign(_dttemp_1, gridView1);
            TaoCot(_dttemp_1, gridView1);
            _dttemp_1 = getDataTemp(_dttemp_1, true);
            List<DataRow> rowsToRemove = new List<DataRow>();
            foreach (DataRow row in _dttemp_1.Rows)
            {
                double total = 0;
                foreach (DataColumn column in _dttemp_1.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        if (row[column] != DBNull.Value)
                        {
                            double value;
                            if (double.TryParse(row[column].ToString(), out value))
                            {
                                total += value;
                            }
                        }
                    }
                }
                if (total == 0)
                {
                    rowsToRemove.Add(row);
                }
            }
            foreach (DataRow row in rowsToRemove)
            {
                _dttemp_1.Rows.Remove(row);
            }
            gridControl1.DataSource = chk_CopyPasteSL.Checked ? GetDataTmp_CopySL(_dttemp_1) : _dttemp_1; ;
            gridControl1.RefreshDataSource();
        }

        private void searchLookUpEdit_Size_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Size.EditValue != null)
            {
                _sizeID = searchLookUpEdit_Size.EditValue.ToString();
            }
        }

        private void gridView2_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                GridSummaryItem item = (GridSummaryItem)e.Item;
                string tag = item.Tag as string;
                if (tag != null)
                {
                    _dttemp_1.AcceptChanges();
                    DataTable _dtgrd = ((DataTable)gridControl2.DataSource).Copy();
                    if (_dtgrd.Rows.Count > 0)
                    {
                        _valueSum = CalculateCustomValue(_dtgrd, tag);
                        string[] arrTagName = tag.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrTagName[1] == "KH")
                        {
                            e.TotalValue = _valueSum;
                        }

                    }
                }
            }

        }
        private void gridView1_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                //GridColumnSummaryItem item = (GridColumnSummaryItem)e.Item;
                GridSummaryItem item = (GridSummaryItem)e.Item;
                //foreach (BandedGridColumn col in view.Columns)

                string tag = item.Tag as string;
                if (tag != null)
                {
                    _dttemp_1.AcceptChanges();
                    //DataTable _dtgrd = gridControl1.DataSource as DataTable;
                    DataTable _dtgrd = ((DataTable)gridControl1.DataSource).Copy();
                    if (_dtgrd.Rows.Count > 0)
                    {
                        _valueSum = CalculateCustomValue(_dtgrd, tag);
                        string[] arrTagName = tag.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrTagName[1] == "TT")
                        {
                            e.TotalValue = _valueSum;
                        }
                        else if (arrTagName[1] == "RM")
                        {
                            string[] arrSize = arrTagName[0].Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            string tenCotCanLay = arrSize[0] + "@Size@" + arrSize[0];
                            int a = 0;
                            GridColumn column = gridView1.Columns[tenCotCanLay];
                            int _valueKH = gridView2.Columns[arrTagName[0]] == null ? 0 : Convert.ToInt32(gridView2.Columns[arrTagName[0]].SummaryItem.SummaryValue);
                            _valueConLai = _valueKH - _valueSum;
                            e.TotalValue = _valueConLai;
                        }
                    }
                }
            }
        }

        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {

            GridView view = (GridView)sender;

            // Kiểm tra xem dòng này có được chọn không
            if (view.IsRowSelected(e.RowHandle))
            {
                // Thiết lập màu sắc cho dòng được chọn
                e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            }
        }

        private void gridView2_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red; // Màu đỏ
        }

        private void gridView1_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red; // Màu đỏ
        }



        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = (GridView)sender;

            // Kiểm tra xem dòng này có được chọn không
            if (view.IsRowSelected(e.RowHandle))
            {
                // Thiết lập màu sắc cho dòng được chọn
                e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            }
        }

        private int CalculateCustomValue(DataTable _dt, string tagname)
        {
            int result = 0; // Ví dụ: Tính tổng của một cột.

            if (tagname.Contains("Amount"))
            {
                int sum = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    foreach (DataColumn column in _dt.Columns)
                    {
                        string[] arrColName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrColName.Length > 1)
                        {
                            int valueSize = string.IsNullOrEmpty(row[column].ToString()) ? 0 : Convert.ToInt32(row[column]);
                            sum += valueSize;
                        }
                    }
                }
                result = sum;
            }
            else
            {
                for (int i = 0; i < _dt.Columns.Count; i++)
                {
                    string[] arrColName = _dt.Columns[i].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                    string[] arrtagname = tagname.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] arrTN = arrtagname[0].Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrColName.Length > 1)
                    {
                        if (_dt.Columns[i].ColumnName == arrtagname[0])
                        {
                            for (int r = 0; r < _dt.Rows.Count; r++)
                            {
                                int valueSize = _dt.Rows[r][_dt.Columns[i].ColumnName].ToString() == "" ? 0 : Convert.ToInt32(_dt.Rows[r][_dt.Columns[i].ColumnName]);
                                //int valuesl = _dt.Rows[r]["SoLop"].ToString() == "" ? 0 : Convert.ToInt32(_dt.Rows[r]["SoLop"]);
                                result += Convert.ToInt32(valueSize);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // Lấy chỉ mục của dòng được chọn trong lưới 1
            int focusedRowIndex = gridView1.FocusedRowHandle < 0 ? 0 : gridView1.FocusedRowHandle;

            // Lấy chỉ mục của cột được chọn trong lưới 1
            int focusedColumnIndex = gridView1.FocusedColumn == null ? 0 : gridView1.FocusedColumn.VisibleIndex;

            // Thiết lập màu chữ cho ô tương ứng trong lưới thứ hai
            gridView2.Appearance.FocusedCell.ForeColor = Color.Red; // Màu chữ
            gridView2.Appearance.FocusedCell.Font = new Font("Tahoma", 12, FontStyle.Bold); // Kích cỡ chữ và in đậm
            gridView2.Appearance.FocusedCell.Options.UseForeColor = true;
            gridView2.Appearance.FocusedCell.Options.UseFont = true;

            // Chọn ô tương ứng trong lưới thứ hai
            gridView2.FocusedRowHandle = focusedRowIndex;
            gridView2.FocusedColumn = gridView2.VisibleColumns[focusedColumnIndex];
            gridView2.SelectCell(focusedRowIndex, gridView2.VisibleColumns[focusedColumnIndex]);
        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            //int a = 0;

            GridView view = (GridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (GridColumn col in view.Columns)
                {
                    if (col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "TenMau" &&
                        col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.UnboundType == UnboundColumnType.Bound)
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


        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            // Lấy chỉ mục của dòng được chọn trong lưới 1
            int focusedRowIndex = gridView1.FocusedRowHandle < 0 ? 0 : gridView1.FocusedRowHandle;

            // Lấy chỉ mục của cột được chọn trong lưới 1
            int focusedColumnIndex = gridView1.FocusedColumn == null ? 0 : gridView1.FocusedColumn.VisibleIndex;

            // Thiết lập màu chữ cho ô tương ứng trong lưới thứ hai
            gridView2.Appearance.FocusedCell.ForeColor = Color.Red; // Màu chữ
            gridView2.Appearance.FocusedCell.Font = new Font("Tahoma", 12, FontStyle.Bold); // Kích cỡ chữ và in đậm
            gridView2.Appearance.FocusedCell.Options.UseForeColor = true;
            gridView2.Appearance.FocusedCell.Options.UseFont = true;

            // Chọn ô tương ứng trong lưới thứ hai
            gridView2.FocusedRowHandle = focusedRowIndex;
            gridView2.FocusedColumn = gridView2.VisibleColumns[focusedColumnIndex];
            gridView2.SelectCell(focusedRowIndex, gridView2.VisibleColumns[focusedColumnIndex]);
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            // kiểm tra nhập liệu

            int num = 0;

            // in 
            if (gridView1.FocusedColumn.FieldName.Contains("@"))
            {
                if (!Int32.TryParse(e.Value as String, out num))
                {
                    e.Valid = false;
                    e.ErrorText = "Hãy nhập số nguyên";
                }
                else
                {
                    string _madh = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, colMaDH_2).ToString();
                    string _poid = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, colPOID_2).ToString();
                    string _colorid = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, colColorID_2).ToString();
                    string _sizetypeid = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, colSizeTypeID_2).ToString();
                    string _filename = gridView1.FocusedColumn.FieldName.ToString();
                    // Lọc dữ liệu từ DataTable
                    string filterExpression = $"MaDH = '{_madh}' AND POID = '{_poid}' AND MaMau = '{_colorid}' AND DauSizeID = '{_sizetypeid}'";
                    DataRow[] filteredRows = _dttemp.Select(filterExpression);
                    DataRow foundRow = filteredRows[0];
                    int SL_KH = Convert.ToInt32(foundRow[_filename]);
                    if (Convert.ToInt32(e.Value) > SL_KH)
                    {
                        e.Valid = false;
                        e.ErrorText = "Không được nhập vượt đơn hàng : " + SL_KH;
                    }
                    if (Convert.ToInt32(e.Value) < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng phải lớn hơn 0";
                    }
                }
            }
        }

        private void gridView4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedValuesSize = string.Join(";", gridView4.GetSelectedRows().Select(rowHandle => gridView4.GetRowCellValue(rowHandle, searchLookUpEdit_Size.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEdit_Size.EditValue = selectedValuesSize;
        }

        private void gridView5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedValuesPO = string.Join(";", gridView5.GetSelectedRows().Select(rowHandle => gridView5.GetRowCellValue(rowHandle, searchLookUpEditPO.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEditPO.EditValue = selectedValuesPO;
        }

        private void searchLookUpEdit_DVSX_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            LoadDSCongDoan(changingEventArgs.NewValue != null ? changingEventArgs.NewValue.ToString() : "");
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
        private DataTable CreateTableChiTietCongDoan()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaLenhSX", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("IsCheckCat", typeof(bool));
            tbl.Columns.Add("IsCheckMay", typeof(bool));
            tbl.Columns.Add("IsCheckHoanThanh", typeof(bool));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("IsCheckDongThung", typeof(bool));
            return tbl;
        }
        private async Task<string> LuuChiTietCongDoan(string magop)
        {
            try
            {
                DataTable tblSave = (gridViewCongDoan.DataSource as DataView).Table;
                if (tblSave.Rows.Count > 0)
                {
                    tblSave.Rows[0]["MaDH"] = magop;
                    tblSave.Rows[0]["MaHang"] = _mahang;
                    tblSave.Rows[0]["MaLenhSX"] = selectedValuesMH.Replace(";", "@").Trim() + "|" + _mahang.ToString().Trim().Replace(" ", "") + "|" + ReplaceSpecialCharacters(RemoveVietnameseTone(txtDot.Text.ToString().Trim().Replace(" ", "")).ToString().Trim().ToUpper().Replace(" ", "")) + "|" + searchLookUpEdit_DVSX.EditValue.ToString().Trim().Replace(" ", "") + "|" + Convert.ToInt32(txtLenhSX.Text.Split('_')[1]);
                    string urlChitietCongDoan = string.Format("{0}?", URL + "CongDoan/PostChiTietCongDoan");
                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlChitietCongDoan, tblSave); }).Result;
                    return msResult;
                }
                return "False";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                return "False";
            }

        }
        private void Luu()
        {
            try
            {
                this.ActiveControl = this.button1;
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                if (txtDot.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập thông tin Đợt trước khi lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (searchLookUpEdit_DVSX.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn thông tin Đơn Vị Sản Xuất trước khi lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataRow rowCongDoan = (gridViewCongDoan.DataSource as DataView).Table.AsEnumerable().FirstOrDefault();
                bool _isGiaCongKT = (bool)(repositoryItemSearchLookUpDVSX.DataSource as DataTable).AsEnumerable()
     .Where(x => x["MaDVSX"].ToString().Equals(rowCongDoan["MaDVSXCat"])).FirstOrDefault()["GiaCong"];
                if (!_isGiaCongKT)
                {
                    if (searchLookUpEditLine.EditValue == null)
                    {
                        MessageBox.Show("Vui lòng chọn chuuyền trước khi lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                }
                if (rowCongDoan != null && rowCongDoan["MaDVSXCat"] != null
                    && rowCongDoan["MaDVSXMay"] != null && rowCongDoan["MaDVSXHoanThanh"] != null
                    && rowCongDoan["MaDVSXDongThung"] != null)
                {
                    // Check Đơn Vị Sản Xuất của các Công Đoạn ! = NULL
                    if (string.IsNullOrEmpty(rowCongDoan["MaDVSXCat"].ToString())
                        || string.IsNullOrEmpty(rowCongDoan["MaDVSXMay"].ToString())
                        || string.IsNullOrEmpty(rowCongDoan["MaDVSXHoanThanh"].ToString())
                        || string.IsNullOrEmpty(rowCongDoan["MaDVSXDongThung"].ToString()))
                    {
                        MessageBox.Show("Vui lòng chọn thông tin Đơn Vị Sản Xuất cho tất cả Công Đoạn trước khi lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Check Đơn Vị Sản Xuất của Công Đoạn Nhập Kho != Đơn vị gia công
                    bool _isGiaCong = (bool)(repositoryItemSearchLookUpDVSX.DataSource as DataTable).AsEnumerable()
            .Where(x => x["MaDVSX"].ToString().Equals(rowCongDoan["MaDVSXHoanThanh"])).FirstOrDefault()["GiaCong"];
                    if (_isGiaCong)
                    {
                        // Đơn vị Nhập kho là Gia Công -> Cảnh báo không cho lưu
                        DialogResult messResult = MessageBox.Show("Công đoạn Nhập Kho không được chọn đơn vị Gia Công. Vui lòng chọn lại Đơn vị Nhập Kho!!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (messResult == DialogResult.No)
                        {

                            gridViewCongDoan.SetFocusedRowCellValue(gridColumnMaDVSXHoanThanh, null);
                            return;
                        }
                        //MessageBox.Show("Công đoạn Nhập Kho không được chọn đơn vị Gia Công. Vui lòng chọn lại Đơn vị Nhập Kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //return;
                    }
                }
                else
                {
                    MessageBox.Show("Đã xảy ra lỗi vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable _dt = gridControl1.DataSource as DataTable;
                if (_dt == null || _dt.Rows.Count == 0)
                    return;
                if (!_isEdit)
                {
                    string urlLSX = string.Format("{0}?madh={1}&&poid={2}", URL + "CanDoiDonHangTong/GetLenhSX", _madh, _poid);
                    string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
                    DataTable _dtDataLSX = JsonConvert.DeserializeObject<DataTable>(jsonLSX);
                    if (_dtDataLSX.Rows.Count != 0 || _dtDataLSX == null)
                    {
                        string filterExpression = $"DotSX = '{txtDot.Text.ToString()}' AND MaDVSX = '{searchLookUpEdit_DVSX.EditValue.ToString()}'";
                        DataRow[] filteredRows = _dtDataLSX.Select(filterExpression);
                        if (filteredRows.Length >= 1)
                        {
                            MessageBox.Show("Dữ liệu đã bị trùng đợt hoặc đơn vị sản xuất. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                List<CanDoiDonViSanXuatSaveEntity> ListCanDoiSX = new List<CanDoiDonViSanXuatSaveEntity>();
                int sum = 0;
                string[] mahang = _mh.Split(';');
                string[] mahanglist = _mh.Split(';');
                _mahang = mahanglist[0].ToString();
                int gopdh = 0;
                int SoId = 0;
                string[] mangChuoiCon = _madh.Split(';');
                DataTable _dtGop = new DataTable();
                if (!_isEdit)
                {
                    string urlKT = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetKiemTra", _madh);
                    string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
                    DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                    if (_dtKT.Rows[0][0].ToString() == "")
                    {
                        string urlID = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetGopID", _mahang);
                        string jsonID = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlID); }).Result;
                        DataTable _dtID = JsonConvert.DeserializeObject<DataTable>(jsonID);
                        if (_dtID.Rows[0][0].ToString() == "")
                            SoId = 1;
                        else
                            SoId = Convert.ToInt32(_dtID.Rows[0][0]) + 1;
                    }
                    else
                    {

                        SoId = Convert.ToInt32(_dtKT.Rows[0][0]);
                    }
                    var groupedDataWithoutPO = from row in _dt.AsEnumerable()
                                               group row by new
                                               {
                                                   MaDH = row["MaDH"],
                                                   MaHang = row["MaHang"]
                                               } into grp
                                               select new
                                               {
                                                   MaDH = grp.Key.MaDH,
                                                   MaHang = grp.Key.MaHang,
                                                   Values = grp.ToList()
                                               };

                    _dtGop.Columns.Add("ID", typeof(int));
                    _dtGop.Columns.Add("MaGop", typeof(string));
                    _dtGop.Columns.Add("GopDH", typeof(string));
                    _dtGop.Columns.Add("MaDH", typeof(string));
                    _dtGop.Columns.Add("MaHang", typeof(string));
                    _dtGop.Columns.Add("SoID", typeof(int));
                    foreach (var group in groupedDataWithoutPO)
                    {
                        foreach (string chuoiCon in mangChuoiCon)
                        {
                            string filterExpression = $"MaHang = '{group.MaHang}' AND MaDH = '{chuoiCon.ToString()}'";
                            DataRow[] filteredRows = _dtGop.Select(filterExpression);
                            if (filteredRows.Length == 0)
                            {
                                DataRow newRow = _dtGop.NewRow();
                                newRow["ID"] = 0;
                                newRow["MaGop"] = group.MaHang.ToString().Trim() + "|" + SoId;
                                newRow["GopDH"] = _madonhang;
                                newRow["MaDH"] = chuoiCon;
                                newRow["MaHang"] = group.MaHang;
                                newRow["SoID"] = SoId;
                                _dtGop.Rows.Add(newRow);
                            }

                        }
                    }

                }

                gopdh = 1;
                string magop = "";
                if (_isEdit)
                {

                }
                for (int i = 0; i <= _dt.Rows.Count - 1; i++)
                {
                    for (int j = 9; j <= _dt.Columns.Count - 1; j++)
                    {
                        CanDoiDonViSanXuatSaveEntity candoisx = new CanDoiDonViSanXuatSaveEntity();
                        if (!_isEdit)
                            candoisx.MaLenhSanXuat = selectedValuesMH.Replace(";", "@").Trim() + "|" + _dt.Rows[i][1].ToString().Trim().Replace(" ", "") + "|" + ReplaceSpecialCharacters(RemoveVietnameseTone(txtDot.Text.ToString().Trim().Replace(" ", "")).ToString().Trim().ToUpper().Replace(" ", "")) + "|" + searchLookUpEdit_DVSX.EditValue.ToString().Trim().Replace(" ", "") + "|" + Convert.ToInt32(txtLenhSX.Text.Split('_')[1]);
                        else
                            candoisx.MaLenhSanXuat = _malenhsanxuat;
                        candoisx.MaLenh = txtLenhSXnew.Text;
                        if (!_isEdit)
                            candoisx.TenLenh = selectedValuesMH.Replace(";", "@").Trim() + "|" + _dt.Rows[i][1].ToString().Trim().Replace(" ", "") + "|" + txtDot.Text.ToString().Trim().Replace(" ", "") + "|" + searchLookUpEdit_DVSX.EditValue.ToString().Trim().Replace(" ", "");
                        else
                            candoisx.TenLenh = _tenlenh;
                        candoisx.DotSX = txtDot.Text;
                        candoisx.MaDH = _dt.Rows[i][0].ToString();
                        candoisx.MaDVSX = searchLookUpEdit_DVSX.EditValue.ToString();
                        candoisx.POID = _dt.Rows[i][2].ToString();
                        candoisx.PO = _dt.Rows[i][3].ToString();
                        candoisx.MaQG = _dt.Rows[i][4].ToString();
                        candoisx.MaMau = _dt.Rows[i][5].ToString();
                        candoisx.DauSizeID = _dt.Rows[i][7].ToString();
                        candoisx.DauSize = _dt.Rows[i][8].ToString();
                        string[] arrNewHeader = _dt.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        candoisx.SizeID = arrNewHeader[0];
                        candoisx.Size = arrNewHeader[2];
                        candoisx.MaLenh = _maSX;
                        candoisx.SoLuong = _dt.Rows[i][j].ToString() == "" ? 0 : Convert.ToInt32(_dt.Rows[i][j].ToString());
                        candoisx.TrangThai = 1;

                        if (!_isEdit)
                            candoisx.STTLenh = Convert.ToInt32(txtLenhSX.Text.Split('_')[1]);
                        else
                            candoisx.STTLenh = Convert.ToInt32(_sttlenh);
                        if (!_isEdit)
                            candoisx.MaGop = _dt.Rows[i][1].ToString().Trim().Replace(" ", "") + "|" + SoId;
                        else
                            candoisx.MaGop = _magop;
                        candoisx.POID_T = ReplaceSpecialCharacters(_dt.Rows[i][3].ToString());
                        sum += _dt.Rows[i][j].ToString() == "" ? 0 : Convert.ToInt32(_dt.Rows[i][j].ToString());
                        if (!_isEdit)
                            magop = _dt.Rows[i][1].ToString().Trim().Replace(" ", "") + "|" + SoId;
                        candoisx.MaCu = txtLenhSX.Text;
                        candoisx.Line = searchLookUpEditLine.EditValue ==null ? "" : searchLookUpEditLine.EditValue.ToString();
                        ListCanDoiSX.Add(candoisx);

                    }
                }
                _totalSum = sum;
                if (_totalSum <= 0)
                {
                    XtraMessageBox.Show("Số lượng đã chia hết. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                List<CanDoiNPLSaveEntity> Listcandoinpl = new List<CanDoiNPLSaveEntity>();
                if (ListCanDoiSX.Count > 0)
                {
                   
                    if (!_isEdit)
                    {
                        string urlPDVSX = string.Format("{0}?", URL + "CanDoiDonHangTong/PostDVSX");
                        string msPDVSX = Task.Run(async () => { return await _clientExtension.PostAsync(urlPDVSX, ListCanDoiSX); }).Result;
                        if (msPDVSX.ToLower() != "true")
                            XtraMessageBox.Show(msPDVSX);
                        string urlGop = string.Format("{0}?", URL + "CanDoiDonHangTong/PostGopDH");
                        string msPGop = Task.Run(async () => { return await _clientExtension.PostAsync(urlGop, _dtGop); }).Result;
                        if (msPGop.ToLower() != "true")
                            XtraMessageBox.Show(msPGop);

                        string resultSaveChiTietCongDoan = Task.Run(async () => { return await LuuChiTietCongDoan(magop); }).Result;

                        if (resultSaveChiTietCongDoan.ToLower() == "true")
                            clsWaitForm.ShowSuccessForm(this, 3000);
                    }
                    else
                    {
                        //if (_iskeove == 1)
                        //{
                        //    string urliskeove = string.Format("{0}?", URL + "CanDoiDonHangTong/PostIsKeoVe");
                        //    string msiskeove = Task.Run(async () => { return await _clientExtension.PostAsync(urliskeove, ListCanDoiSX); }).Result;
                        //    if (msPDVSX.ToLower() != "true")
                        //        XtraMessageBox.Show(msiskeove);
                        //}
                        string urlPDVSX = string.Format("{0}?", URL + "CanDoiDonHangTong/PostDVSXV2");
                        string msPDVSX = Task.Run(async () => { return await _clientExtension.PostAsync(urlPDVSX, ListCanDoiSX); }).Result;
                        if (msPDVSX.ToLower() != "true")
                            XtraMessageBox.Show(msPDVSX);
                    }
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                return;
            }
        }
        private void btLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Luu();
        }
        private void gridView2_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        private void gridView2_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void gridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void chkv_AllData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkv_AllData.Checked)
            {
                if (!_isEdit)
                {
                    if (selectedValuesMH.ToString() == "")
                    {
                        XtraMessageBox.Show("Vui lòng chọn đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        chkv_AllData.Checked = false;
                        return;
                    }
                }
                else
                {
                    if (searchLookUpEdit_DonHang.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        chkv_AllData.Checked = false;
                        return;
                    }
                }

                if (searchLookUpEditPO.EditValue == null)
                {
                    _poidAll = "";
                    string urlDSPO = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetPO", _madh);
                    string jsonDSPO = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSPO); }).Result;
                    DataTable dtDSPO = JsonConvert.DeserializeObject<DataTable>(jsonDSPO);
                    for (int i = 0; i < dtDSPO.Rows.Count; i++)
                    {
                        if (_poidAll != "")
                        {
                            _poidAll = _poidAll + ";";
                        }
                        _poidAll = _poidAll + dtDSPO.Rows[i]["POID"].ToString();
                    }
                }
                _poid = _poidAll;
                if (searchLookUpEditSizeType.EditValue == null)
                {
                    _dausizeALL = "";
                    string urlDSDauSize = string.Format("{0}?madh={1}&&poid={2}", URL + "CanDoiDonHangTong/GetDauSize", _madh, _poid);
                    string jsonDSDauSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSDauSize); }).Result;
                    DataTable dtDS = JsonConvert.DeserializeObject<DataTable>(jsonDSDauSize);
                    for (int i = 0; i < dtDS.Rows.Count; i++)
                    {
                        if (_dausizeALL != "")
                        {
                            _dausizeALL = _dausizeALL + ";";
                        }
                        _dausizeALL = _dausizeALL + dtDS.Rows[i]["DauSizeID"].ToString();
                    }
                }
                _dausize = _dausizeALL;
                if (searchLookUpEditColor.EditValue == null)
                {
                    string urldtmau = string.Format("{0}?madh={1}&&dausizeID={2}", URL + "CanDoiDonHangTong/GetMau", _madh, _dausize);
                    string jsondtmau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldtmau); }).Result;
                    DataTable dtmau = JsonConvert.DeserializeObject<DataTable>(jsondtmau);
                    _colorIDALL = "";
                    for (int i = 0; i < dtmau.Rows.Count; i++)
                    {
                        if (_colorIDALL != "")
                        {
                            _colorIDALL = _colorIDALL + ";";
                        }
                        _colorIDALL = _colorIDALL + dtmau.Rows[i]["MaMau"].ToString();
                    }
                }

                _mamau = _colorIDALL;
            }
            else
            {
                _poid = selectedValuesPO;
                _dausize = selectedValuesDS;
                _mamau = selectedValuesMau;
                _sizeID = selectedValuesSize;

                //if (searchLookUpEditSizeType.EditValue != null)
                //{
                //    _dausize = searchLookUpEditSizeType.EditValue.ToString();
                //}
                //if (searchLookUpEditColor.EditValue != null)
                //{
                //    _mamau = searchLookUpEditColor.EditValue.ToString();
                //}

            }
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CreateDefault();
        }

        private void gridView2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridView2_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        private void gridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridView2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.N:
                    if (e.Control)
                    {
                        // Tổ hợp phím Ctrl + N
                        Luu();
                    }
                    break;
                case Keys.F5:
                    CreateDefault();
                    break;
            }
        }
        private void searchLookUpEditPO_EditValueChanged(object sender, EventArgs e)
        {
            //// checkrong = 0;
            // StringBuilder sb = new StringBuilder();
            // foreach (DataRowView rv in gridCheckMarksPO.Selection)
            // {
            //     if (sb.ToString().Length > 0) { sb.Append(";"); }
            //     sb.Append(rv["POID"].ToString());
            if (searchLookUpEditPO.EditValue != null)
            {
                _poid = searchLookUpEditPO.EditValue.ToString();
                _dausizeALL = "";
                string urlDauSize = string.Format("{0}?madh={1}&&poid={2}", URL + "CanDoiDonHangTong/GetDauSize", _madh, _poid);
                string jsonDauSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDauSize); }).Result;
                DataTable dtDauSize = JsonConvert.DeserializeObject<DataTable>(jsonDauSize);

                for (int i = 0; i < dtDauSize.Rows.Count; i++)
                {
                    if (_dausizeALL != "")
                    {
                        _dausizeALL = _dausizeALL + ";";
                    }
                    _dausizeALL = _dausizeALL + dtDauSize.Rows[i]["DauSizeID"].ToString();
                }

                searchLookUpEditSizeType.Properties.DataSource = dtDauSize;
                searchLookUpEditSizeType.Properties.DisplayMember = "DauSize";
                searchLookUpEditSizeType.Properties.ValueMember = "DauSizeID";
                searchLookUpEditSizeType.Properties.ShowClearButton = false;
                searchLookUpEditSizeType.Properties.NullText = "[Chọn InSeam]";
                searchLookUpEditSizeType.Properties.Appearance.ForeColor = Color.Red;
                GridView dvView = searchLookUpEditSizeType.Properties.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "DauSizeID", Caption = "Đầu size ID", Name = "colDauSizeID", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "DauSize", Caption = "InSeam", Name = "colDauSize", Visible = true });

                }
                selectedValuesDS = "";
                selectedValuessDS = "";
                selectedValuesMau = "";
                selectedValuessMau = "";
                selectedValuesSize = "";
                selectedValuessSize = "";
                _dausize = "";
                _mamau = "";
                _sizeID = "";
            }
        }

        private void searchLookUpEdit4View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedValuesDS = string.Join(";", searchLookUpEdit4View.GetSelectedRows().Select(rowHandle => searchLookUpEdit4View.GetRowCellValue(rowHandle, searchLookUpEditSizeType.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEditSizeType.EditValue = selectedValuesDS;

            // selectedValuess = string.Join(";", gridView5.GetSelectedRows().Select(rowHandle => gridView5.GetRowCellValue(rowHandle, searchLookUpEditPO.Properties.DisplayMember)));

        }

        private void searchLookUpEdit3View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            selectedValuesMau = string.Join(";", searchLookUpEdit3View.GetSelectedRows().Select(rowHandle => searchLookUpEdit3View.GetRowCellValue(rowHandle, searchLookUpEditColor.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEditColor.EditValue = selectedValuesMau;


        }

        private void txtDot_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void gridView2_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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
                        if (Convert.ToInt32(e.Value.ToString()) == 0)
                            e.DisplayText = "-";
                    }
                }
            }
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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
                        if (Convert.ToInt32(e.Value.ToString()) == 0)
                            e.DisplayText = "-";
                    }
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (gridView1.SelectedRowsCount > 0)
            {
                // Lấy chỉ mục của dòng được chọn đầu tiên
                int selectedIndex = gridView1.GetSelectedRows()[0];

                // Lấy dữ liệu của dòng được chọn
                DataRow row = gridView1.GetDataRow(selectedIndex);

                // Xóa dòng từ nguồn dữ liệu (ví dụ: DataTable)
                if (row != null)
                {
                    ((DataTable)gridControl1.DataSource).Rows.Remove(row);
                    // Hoặc, nếu sử dụng BindingList thay vì DataTable
                    // ((BindingList<MyDataObject>)gridControl1.DataSource).Remove(row);
                }

                // Cập nhật GridView
                gridView1.RefreshData();
            }
        }
        int z = -1;
        object ktmah = "", ktkh = "", ktcl = "";
        private bool programmaticChange = false;
        private void searchLookUpEdit1View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!programmaticChange)
            {
                GridView gridView = sender as GridView;
                //if (z == gridView.FocusedRowHandle) return;
                int selectedRowCount = gridView.SelectedRowsCount;
                int totalRowCount = gridView.RowCount;
                //if (selectedRowCount == totalRowCount)
                //{
                //    XtraMessageBox.Show("Không được chọn tất cả các mã hàng khác nhau. Vui lòng chọn lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //    gridView.ClearSelection();
                //    searchLookUpEdit_DonHang.EditValue = selectedValuesMH;
                //    return;
                //}

                string chuoi = string.Empty;
                selectedValuesMH = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
                string kiemtraMH = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
                _madonhang = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
                _mh = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
                string[] mangChuoiCon = kiemtraMH.Split(';');
                if (mangChuoiCon.Length > 1)
                {
                    foreach (string chuoiCon in mangChuoiCon)
                    {

                        if (chuoi.ToString() != "" && chuoi.ToString() != chuoiCon.ToString())
                        {
                            XtraMessageBox.Show("Không chung mã hàng. Vui lòng chọn chung mã hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            z = gridView.FocusedRowHandle;
                            programmaticChange = true;
                            gridView.UnselectRow(gridView.FocusedRowHandle);
                            if (selectedRowCount == totalRowCount) gridView.ClearSelection();
                            selectedValuesMH = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
                            searchLookUpEdit_DonHang.EditValue = selectedValuesMH;
                            return;
                        }
                        chuoi = chuoiCon;

                    }
                }
                z = 0;
                ktmah = gridView.GetRowCellValue(gridView.FocusedRowHandle, "GopDH");
                ktkh = gridView.GetRowCellValue(gridView.FocusedRowHandle, "MaKH");
                ktcl = gridView.GetRowCellValue(gridView.FocusedRowHandle, "MaCL");
                object ktmh = gridView.GetRowCellValue(gridView.FocusedRowHandle, "MaHang");
                int[] selectedRowsgop = gridView.GetSelectedRows();
                foreach (int rowHandle in selectedRowsgop)
                {
                    object ssGop = gridView.GetRowCellValue(rowHandle, "MaDH");
                    object ssmh = gridView.GetRowCellValue(rowHandle, "MaHang");
                    object sskh = gridView.GetRowCellValue(rowHandle, "MaKH");
                    object sscl = gridView.GetRowCellValue(rowHandle, "MaCL");
                    for (int i = 0; i < gridView.DataRowCount; i++)
                    {
                        object values = gridView.GetRowCellValue(i, "MaHang");
                        if (ktmah != null)
                        {
                            if (ktkh.ToString() != sskh.ToString() && sscl.ToString() != ssmh.ToString() && values.ToString() == ktmh.ToString())
                            {
                                XtraMessageBox.Show("Vui lòng chọn chung mã hàng - khách hàng - chủng loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                z = gridView.FocusedRowHandle;
                                ktkh = "";
                                ktcl = "";
                                programmaticChange = true;
                                gridView.UnselectRow(gridView.FocusedRowHandle);
                                return;
                            }
                        }

                    }
                }
                programmaticChange = false;
                List<int> selectedRowHandles = gridView.GetSelectedRows().ToList();
                searchLookUpEdit_DonHang.EditValue = selectedValuesMH;

            }
            else
            {
                programmaticChange = false;
                selectedValuesMH = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
                _madonhang = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.ValueMember)));
                _mh = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
            }
        }
        private void searchLookUpEdit_DonHang_CustomDisplayText_1(object sender, CustomDisplayTextEventArgs e)
        {
            selectedValuessMH = string.Join("; ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit_DonHang.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessMH.ToString()))
            {
                if (!_isEdit)
                    e.DisplayText = "----Chưa chọn Mã hàng----";
                else
                    e.DisplayText = _madh;

            }
            else
            {
                e.DisplayText = selectedValuessMH.ToString();
            }
        }


        #region Modify 13/02/2025 Thêm option copy paste số lượng

        DialogResult StatusMsg = DialogResult.None; int startRow = 0; int endRow = 0; bool copiedFromGrid = false;
        private DataTable GetDataTmp_CopySL(DataTable dt_tmp)
        {
            if (dt_tmp.Rows?.Count == 0) return new DataTable();
            foreach (DataRow row in dt_tmp.Rows)
            {
                foreach (DataColumn col in dt_tmp.Columns)
                {
                    if (col.ColumnName.Contains("Size@"))
                    {
                        row[col] = 0;
                    }
                }
            }
            return dt_tmp;
        }

        private void GridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
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
        private void ItemCopy_Click(object sender, EventArgs e)
        {
            GridView view = gridControl1.MainView as GridView;
            if (view != null)
            {
                view.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
                view.CopyToClipboard();
            }

        }
        private void ItemPaste_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                AddRow();
            }
            else
            {
                XtraMessageBox.Show("Không có dữ liệu trong Clipboard để dán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            GridView view = gridControl1.MainView as GridView;
            if (view == null) return;

            if (e.Control && e.KeyCode == Keys.C)
            {
                view.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
                view.CopyToClipboard();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }

            else if (e.Control && e.KeyCode == Keys.V)
            {
                if (!Clipboard.ContainsText()) return;
                AddRow();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void AddRow()
        {
            GridView view = gridControl1.MainView as GridView;
            string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 1) return;


            List<GridColumn> allSizeColumns = gridView1.Columns
                .Where(x => x.FieldName.Contains("Size@"))
                .OrderBy(x => x.VisibleIndex)
                .ToList();


            GridColumn focusedColumn = gridView1.FocusedColumn;
            int startColIndex = allSizeColumns.FindIndex(col => col == focusedColumn);

            if (startColIndex == -1) startColIndex = 0;


            List<string> lstSizeCol = allSizeColumns
                .Skip(startColIndex)
                .Select(col => col.FieldName)
                .ToList();

            StatusMsg = DialogResult.None;
            int startRow = view.FocusedRowHandle;
            int endRow = data.Length - 1;
            int rowHandle = startRow;

            foreach (string row in data)
            {
                if (string.IsNullOrEmpty(row)) continue;
                if (!view.IsValidRowHandle(rowHandle)) break;

                string[] rowData = row.Split('\t');

                string _madh = gridView1.GetRowCellValue(rowHandle, colMaDH_2)?.ToString();
                string _poid = gridView1.GetRowCellValue(rowHandle, colPOID_2)?.ToString();
                string _colorid = gridView1.GetRowCellValue(rowHandle, colColorID_2)?.ToString();
                string _sizetypeid = gridView1.GetRowCellValue(rowHandle, colSizeTypeID_2)?.ToString();

                for (int i = 0; i < rowData.Length; i++)
                {

                    if (i > lstSizeCol.Count - 1) continue;

                    string columnName = lstSizeCol[i];
                    string filterExpression = $"MaDH = '{_madh}' AND POID = '{_poid}' AND MaMau = '{_colorid}' AND DauSizeID = '{_sizetypeid}'";
                    DataRow[] filteredRows = _dttemp.Select(filterExpression);
                    //if (filteredRows.Length == 0) continue;
                    DataRow foundRow = filteredRows[0];
                    if (int.TryParse(rowData[i].Replace(",", ""), out int SoLuong))
                    {
                        int SL_KH = Convert.ToInt32(foundRow[columnName]);
                        if (SoLuong > SL_KH)
                        {
                            SoLuong = 0;
                            if (StatusMsg == DialogResult.None)
                            {
                                string msg = SL_KH > 0
                                    ? $"Số lượng PO <b><color='navy'>{foundRow["PO"]}</color></b> - Đầu Size <b><color='navy'>{foundRow["DauSize"]}</color></b> - Size <b><color='navy'>{columnName.Split('@')[2]}</color></b> không được vượt đơn hàng SLKH: <b><color='red'>{SL_KH}</color></b>"
                                    : $"Số lượng PO <b><color='navy'>{foundRow["PO"]}</color></b> - Đầu Size <b><color='navy'>{foundRow["DauSize"]}</color></b> - Size <b><color='navy'>{columnName.Split('@')[2]}</color></b> chưa có hoặc đã hết.";
                                msg += "<br>Nhấn <b><color='red'>OK</color></b> để tiếp tục dán nhưng bỏ qua các số lượng không đúng, hoặc nhấn <b><color='red'>Cancel</color></b> để hủy thao tác.";

                                DialogResult result = ShowMessger(msg, new DialogResult[] { DialogResult.OK, DialogResult.Cancel });
                                if (result == DialogResult.Cancel)
                                {
                                    StatusMsg = DialogResult.Cancel;
                                    RefreshSLCopy(startRow, endRow);
                                    return;
                                }
                                else
                                {
                                    StatusMsg = DialogResult.OK;
                                }
                            }
                        }
                        else if (SoLuong < 0)
                        {
                            SoLuong = 0;
                            if (StatusMsg == DialogResult.None)
                            {
                                string msg = $"Số lượng PO <b><color='navy'>{foundRow["PO"]}</color></b> - Đầu Size <b><color='navy'>{foundRow["DauSize"]}</color></b> - Size <b><color='navy'>{columnName.Split('@')[2]}</color></b> phải lớn hơn <color=red> <b>0</b></color>";
                                msg += "<br>Nhấn <b><color='red'>OK</color></b> để tiếp tục dán nhưng bỏ qua các số lượng không đúng, hoặc nhấn <b><color='red'>Cancel</color></b> để hủy thao tác.";

                                DialogResult result = ShowMessger(msg, new DialogResult[] { DialogResult.OK, DialogResult.Cancel });
                                if (result == DialogResult.Cancel)
                                {
                                    StatusMsg = DialogResult.Cancel;
                                    RefreshSLCopy(startRow, endRow);
                                    return;
                                }
                                else
                                {
                                    StatusMsg = DialogResult.OK;
                                }
                            }
                        }

                        view.SetRowCellValue(rowHandle, columnName, SoLuong);
                    }
                }

                rowHandle++;
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

        private void RefreshSLCopy(int rowStart, int rowEnd)
        {

            if (!chk_CopyPasteSL.Checked)
            {
                DataTable _dt = gridControl2.DataSource as DataTable;
                if (_dt != null)
                {
                    LoadDataGrd_1();
                }
            }
            else
            {
                List<GridColumn> allSizeColumns = gridView1.Columns
               .Where(x => x.FieldName.Contains("Size@"))
               .OrderBy(x => x.VisibleIndex)
               .ToList();


                GridColumn focusedColumn = gridView1.FocusedColumn;
                int startColIndex = allSizeColumns.FindIndex(col => col == focusedColumn);

                if (startColIndex == -1) startColIndex = 0;


                List<string> lstSizeCol = allSizeColumns
                    .Skip(startColIndex)
                    .Select(col => col.FieldName)
                    .ToList();


                for (int rowIndex = rowStart; rowIndex <= rowEnd; rowIndex++)
                {
                    foreach (string columnName in lstSizeCol)
                    {

                        gridView1.SetRowCellValue(rowIndex, columnName, 0);
                    }
                }
            }


        }

        private DialogResult ShowMessger(string message, DialogResult[] arrButton)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs
            {
                Caption = Resources.Warning,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Text = message,
                Buttons = arrButton,
                Icon = SystemIcons.Warning,
                MessageBeepSound = MessageBeepSound.Warning
            };

            return XtraMessageBox.Show(args);
        }


        #endregion


        #region Mạnh 
        private void GetMaLenhSX()
        {
            Init();
            GetLine();
            string url = $"{URL}ERPDonHangTong/Get?action=GetMaxLenh";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            txtLenhSXnew.EditValue = tbl.Rows[0]["LenhSX"].ToString();
        }
        private void Init()
        {
            searchLookUpEditLine.Properties.ValueMember = "Line";
            searchLookUpEditLine.Properties.DisplayMember = "Name";
            searchLookUpEditLine.Properties.NullValuePrompt = "Chọn chuyền";
        }
        private void GetLine()
        {

            string url = $"{URL}ERPDonHangTong/Get?action=GetLine";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if(_isEdit)
            {
                searchLookUpEditLine.Properties.DataSource = tbl;
                searchLookUpEditLine.EditValue = _Line;

            }   
            else
                searchLookUpEditLine.Properties.DataSource = tbl;


        }
        #endregion
    }
}
