using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmERPKeHoachDongThung : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        SearchCheckSelection gridCheckMarksDVSX;
        SearchCheckSelection gridCheckMarksPO;
        SearchCheckSelection gridCheckMarksSizeType;
        private string _madh = string.Empty, _maHang = string.Empty, _maDVSX = string.Empty, _po = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty;
        DataTable dtSize = new DataTable();
        DataTable _dtData = new DataTable();
        DataTable _dtInit = new DataTable();
        int _carton = 0, thung_po = 0;
        KeyDownControlHandler keyDownControlHandler;
        public frmERPKeHoachDongThung(string MaDH, string MaHang)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _madh = MaDH;
            _maHang = MaHang;
            txtMahang.Text = MaHang;
            txtMaDH.Text = _madh;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
       
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtnLuu, true, ActionType.Save);
            AddActionControl(_lstActionControl, GetSLDM, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType,true);
            list.Add(actionControl);
        }
        private void CreateDefault()
        {
            string url = string.Format("{0}?", URL + $"KeHoachDongThung/Get?Action=GetDVSX&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            _dtInit = JsonConvert.DeserializeObject<DataTable>(json);
            var _lstDVSX = _dtInit.AsEnumerable().Select(x => new
            {
                MaDVSX = x["MaDVSX"].ToString(),
                TenDVSX = x["TenDVSX"].ToString(),
                DotSX = x["DotSX"].ToString(),
                value = x["value"].ToString(),
            }).Distinct().ToList();
            string jsonDVSX = JsonConvert.SerializeObject(_lstDVSX);
            DataTable _dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            //searchLookUpEdit_DonHang.Properties.DataSource = dtdh;
            //searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            //searchLookUpEdit_DonHang.Properties.DisplayMember = "Tenhang";
            //if (!string.IsNullOrEmpty(_madh))
            //{
            //    searchLookUpEdit_DonHang.EditValue = _madh;
            //}

            searchLookUpEditDVSX.Properties.ValueMember = "value";
            searchLookUpEditDVSX.Properties.DisplayMember = "TenDVSX";
            searchLookUpEditDVSX.Properties.NullText = "[Chọn ĐVSX]";
            // searchLookUpEditDVSX.Properties.Appearance.ForeColor = Color.Red;
            searchLookUpEditDVSX.Properties.View.OptionsSelection.MultiSelect = true;
            searchLookUpEditDVSX.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditDVSX_CustomDisplayText);
            searchLookUpEditDVSX.Properties.PopulateViewColumns();
            gridCheckMarksDVSX = new SearchCheckSelection(searchLookUpEditDVSX.Properties);
            gridCheckMarksDVSX.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditDVSX_SelectionChanged);
            searchLookUpEditDVSX.Properties.Tag = gridCheckMarksDVSX;
            searchLookUpEditDVSX.Properties.DataSource = _dtDVSX;

            searchLookUpEditPO.Properties.ValueMember = "POID";
            searchLookUpEditPO.Properties.DisplayMember = "PO";
            searchLookUpEditPO.Properties.NullText = "[Chọn PO]";
            // searchLookUpEditPO.Properties.Appearance.ForeColor = Color.Red;

            //searchLookUpEditPO.Properties.View.OptionsSelection.MultiSelect = true;
            //searchLookUpEditPO.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditPO_CustomDisplayText);
            //searchLookUpEditPO.Properties.PopulateViewColumns();
            //gridCheckMarksPO = new SearchCheckSelection(searchLookUpEditPO.Properties);
            //gridCheckMarksPO.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditPO_SelectionChanged);
            //searchLookUpEditPO.Properties.Tag = gridCheckMarksPO;


            searchLookUpEditSizeType.Properties.ValueMember = "valueSizeType";
            searchLookUpEditSizeType.Properties.DisplayMember = "SizeType";
            searchLookUpEditSizeType.Properties.NullText = "[Chọn đầu size]";
            //searchLookUpEditSizeType.Properties.Appearance.ForeColor = Color.Red;
            searchLookUpEditSizeType.Properties.View.OptionsSelection.MultiSelect = true;
            searchLookUpEditSizeType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditSizeType_CustomDisplayText);
            searchLookUpEditSizeType.Properties.PopulateViewColumns();
            gridCheckMarksSizeType = new SearchCheckSelection(searchLookUpEditSizeType.Properties);
            gridCheckMarksSizeType.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditSizeType_SelectionChanged);
            searchLookUpEditSizeType.Properties.Tag = gridCheckMarksSizeType;

            var _lstNgayGH = _dtInit.AsEnumerable().Select(y => new
            {
                NgayGH = y["NgayGH"].ToString()
            }).Distinct().ToList();
            cbxNgayGH.Properties.Items.Clear();
            foreach (var item in _lstNgayGH)
            {
                cbxNgayGH.Properties.Items.Add(item.NgayGH);
            }
        }
        private void cbxNgayGH_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadPO();
        }
        void searchLookUpEditDVSX_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append("; "); }
                sb.Append(rv["TenDVSX"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chưa chọn ĐVSX----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }

        }
        void searchLookUpEditDVSX_SelectionChanged(object sender, EventArgs e)
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
                    if (sb.ToString().Length > 0) { sb.Append(";"); }
                    sb.Append(rv["value"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
            if (searchLookUpEditDVSX.EditValue != null)
                _maDVSX = searchLookUpEditDVSX.EditValue.ToString();
            //string url = string.Format("{0}?", URL + $"KeHoachDongThung/Get?Action=GET_PO&MaDH=DH00000064&MaDVSX={searchLookUpEditDVSX.EditValue}&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            LoadPO();


        }
        private void LoadPO()
        {
            string[] lstDVSX = _maDVSX.Split(';');
            if (lstDVSX.Length == 0) return;
            var _lstTempPO = _dtInit.AsEnumerable().Where(x => lstDVSX.Contains(x["value"].ToString())
                                                            && x["NgayGH"].ToString() == (cbxNgayGH.EditValue is null ? x["NgayGH"].ToString() : cbxNgayGH.EditValue.ToString()))
                                                    .Select(y => new
                                                    {
                                                        POID = y["POID"].ToString(),
                                                        PO = y["PO"].ToString()
                                                    }).Distinct().ToList();
            string jsonPO = JsonConvert.SerializeObject(_lstTempPO);
            DataTable _dtPO = JsonConvert.DeserializeObject<DataTable>(jsonPO);

            searchLookUpEditPO.Properties.DataSource = _dtPO;
            if (_dtPO.Rows.Count > 0)
            {
                searchLookUpEditPO.EditValue = null;
                searchLookUpEditPO.EditValue = _dtPO.Rows[0]["POID"];
            }
        }
        void searchLookUpEditPO_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //StringBuilder sb = new StringBuilder();
            //SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            //if (gridCheckMark == null) return;
            //foreach (DataRowView rv in gridCheckMark.Selection)
            //{
            //    if (sb.ToString().Length > 0) { sb.Append("; "); }
            //    sb.Append(rv["PO"].ToString());
            //}
            //if (string.IsNullOrEmpty(sb.ToString()))
            //{
            //    e.DisplayText = "----Chưa chọn PO----";
            //}
            //else
            //{
            //    e.DisplayText = sb.ToString();
            //}
        }
        void searchLookUpEditPO_SelectionChanged(object sender, EventArgs e)
        {
            //Control c = this.ActiveControl;
            //if (c is DevExpress.XtraLayout.LayoutControl)
            //{
            //    if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
            //    {
            //        c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
            //    }
            //}
            //if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
            //    {
            //        if (sb.ToString().Length > 0) { sb.Append("; "); }
            //        sb.Append(rv["POID"].ToString());
            //    }
            //    (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();

            //}
        }
        void searchLookUpEditSizeType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append("; "); }
                sb.Append(rv["SizeType"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chưa chọn đầu size----";
            }
            else
            {
                e.DisplayText = sb.ToString();
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
                    if (sb.ToString().Length > 0) { sb.Append(";"); }
                    sb.Append(rv["valueSizeType"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
            if (searchLookUpEditSizeType.EditValue != null)
                _dausize = searchLookUpEditSizeType.EditValue.ToString();
            Console.WriteLine(_dausize);
            GetSLDM();

        }
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetSLDM();
        }
        private void searchLookUpEditPO_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditPO.EditValue is null) return;

            string[] lstDVSX = _maDVSX.Split(';');
            if (lstDVSX.Length == 0) return;
            var _lstTempSizeType = _dtInit.AsEnumerable().Where(x => lstDVSX.Contains(x["value"].ToString()) && x["POID"].ToString() == searchLookUpEditPO.EditValue.ToString())
                                                    .Select(y => new
                                                    {
                                                        TenDVSX = y["TenDVSX"].ToString(),
                                                        DotSX = y["DotSX"].ToString(),
                                                        SizeType = y["SizeType"].ToString(),
                                                        SizeTypeID = y["SizeTypeID"].ToString(),
                                                        valueSizeType = y["valueSizeType"].ToString()
                                                    }).Distinct().ToList();
            string jsonSizeType = JsonConvert.SerializeObject(_lstTempSizeType);
            DataTable _dtSizeType = JsonConvert.DeserializeObject<DataTable>(jsonSizeType);

            searchLookUpEditSizeType.Properties.DataSource = _dtSizeType;
            var a = searchLookUpEditSizeType.Properties.DataSource as DataTable;
            searchLookUpEditSizeType.SelectAll();
            searchLookUpEditSizeType.Reset();
            searchLookUpEditSizeType.RefreshEditValue();

        }
        private void btn_XC_Config_Click(object sender, EventArgs e)
        {
            var _dt = grcDetail.DataSource as DataTable;
            var groupedSum = _dt.AsEnumerable().Where(x => Convert.ToInt32(x["SLDT"]) != 0)
                .GroupBy(row => new
                {
                    SizeID = row.Field<string>("SizeID"),
                    Size = row.Field<string>("Size"),
                    SLPCB = Convert.ToInt32(row["SLPCB"]),
                    SLThung = Convert.ToInt32(row["SLThung"]),
                })
                .Select(group => new
                {
                    SizeID = group.Key.SizeID,
                    Size = group.Key.Size,
                    SLPCB = group.Key.SLPCB,
                    SLThung = group.Key.SLThung,
                    SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                    SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),
                }).ToList();
            string jsonT = JsonConvert.SerializeObject(groupedSum);
            DataTable dtSizetemp = JsonConvert.DeserializeObject<DataTable>(jsonT);
            gridControlSize.DataSource = dtSizetemp;
        }
        private void GetSLDM()
        {
            try
            {
                string[] lstDVSX = _dausize.Split(';');
                string _maDVSXF = "";
                for (int i = 0; i < lstDVSX.Length; i++)
                {
                    _maDVSXF += ",'" + lstDVSX[i].Trim() + "'";
                }
                _maDVSXF = _maDVSXF.TrimStart(',');
                string url = string.Format("{0}?", URL + $"KeHoachDongThung/Get?Action=Get_SLDM&MaDH={_madh}&MaDVSX={_maDVSXF}&DotSX=Para" +
                                                    $"&POID={searchLookUpEditPO.EditValue}&SizeTypeID=${_dausize}&ColorID=Para&ProductID=Para&SizeID=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtSize = JsonConvert.DeserializeObject<DataTable>(json);
                grcDetail.DataSource = dtSize;
                gridviewDetail.RefreshData();

            }
            catch (Exception ex)
            {

            }

        }
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ProcessPackage();
        }
        private void ProcessPackage()
        {
            try
            {
                DataTable _dtTemp = (DataTable)gridControlSize.DataSource;
                _dtData.Clear();
                //var _dtDVSX = dtSize.AsEnumerable().Select(x => new
                //{
                //    DVSX = x["MaDVSX"],
                //    TenDVSX = x["TenDVSX"]
                //}).Distinct().ToList();
                dtSize = grcDetail.DataSource as DataTable;
                var _dttempPO = dtSize.AsEnumerable().Select(x => new
                {
                    POID = x["POID"],
                    PO = x["PO"]
                }).Distinct().ToList();
                _dtData = new DataTable("dtData");
                _dtData.Columns.Add("ColGroup", typeof(string));
                _dtData.Columns.Add("TuThung", typeof(int));
                _dtData.Columns.Add("DenThung", typeof(int));
                _dtData.Columns.Add("MaDVSX", typeof(string));
                _dtData.Columns.Add("TenDVSX", typeof(string));
                _dtData.Columns.Add("MaLenh", typeof(string));
                _dtData.Columns.Add("DotSX", typeof(string));
                _dtData.Columns.Add("POID", typeof(string));
                _dtData.Columns.Add("PO", typeof(string));
                _dtData.Columns.Add("SizeType", typeof(string));
                _dtData.Columns.Add("SizeTypeID", typeof(string));
                _dtData.Columns.Add("TenMau", typeof(string));
                _dtData.Columns.Add("ColorID", typeof(string));
                _dtData.Columns.Add("StyleID", typeof(string));
                _dtData.Columns.Add("MaHang", typeof(string));

                //_dtData.Columns.Add("Qty", typeof(int));
                _dtData.Columns.Add("Carton", typeof(int));
                _dtData.Columns.Add("Total", typeof(int));
                _dtData.Columns.Add("ChieuDai", typeof(string));
                _dtData.Columns.Add("ChieuRong", typeof(string));
                _dtData.Columns.Add("ChieuCao", typeof(string));
                _dtData.Columns.Add("IsThungLe", typeof(string));

                DataTable _dtData_du = _dtData.Copy();
                int p = 0;
                int startThung = 0;
                int _tuthung = 0, _dednthung = 0, _soluongKH = 0, _sltrongthung = 0, _sltemp = 0, _songuyen = 0, _sodu = 0, _total;
                foreach (var itemPO in _dttempPO)
                {
                    var _dtdataPO = dtSize.AsEnumerable().Where(y => y["POID"].ToString() == itemPO.POID.ToString()).CopyToDataTable();
                    var _dtdistinctDVSX = dtSize.AsEnumerable().Select(x => new
                    {
                        MaDVSX = x["MaDVSX"],
                        TenDVSX = x["TenDVSX"],
                        MaLenh = x["MaLenh"],
                        DotSX = x["DotSX"]
                    }).Distinct().ToList();

                    foreach (var itemDVSX in _dtdistinctDVSX)
                    {
                        var POID = itemPO.POID;
                        var PO = itemPO.PO;
                        var _dtdata = _dtdataPO.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString()).CopyToDataTable();
                        var _dtDistinctDT = _dtdata.AsEnumerable().Select(x => new
                        {
                            SizeTypeID = x["SizeTypeID"],
                            SizeType = x["SizeType"],
                            TenMau = x["TenMau"],
                            ColorID = x["ColorID"],
                        }).Distinct();
                        foreach (var itemData in _dtDistinctDT)
                        {
                            for (int i = 0; i < _dtTemp.Rows.Count; i++)
                            {
                                if (chkb_size.Checked)
                                {
                                    string _size = string.Empty;
                                    if (_size != _dtTemp.Rows[i]["SizeID"].ToString())
                                    {
                                        _tuthung = 0;
                                        _dednthung = 0;
                                        _size = _dtTemp.Rows[i]["SizeID"].ToString();
                                    }
                                }

                                DataRow _dr = _dtData.NewRow();
                                _dr["POID"] = POID;
                                _dr["PO"] = PO;
                                //_dr["StyleID"] = _styleID;
                                _dr["MaHang"] = _maHang;
                                _dr["MaDVSX"] = itemDVSX.MaDVSX;
                                _dr["TenDVSX"] = itemDVSX.TenDVSX;
                                _dr["MaLenh"] = itemDVSX.MaLenh;
                                _dr["DotSX"] = itemDVSX.DotSX;
                                _dr["SizeType"] = itemData.SizeType;
                                _dr["SizeTypeID"] = itemData.SizeTypeID;
                                _dr["TenMau"] = itemData.TenMau;
                                _dr["ColorID"] = itemData.ColorID;
                                _dr["ChieuDai"] = 0;
                                _dr["ChieuRong"] = 0;
                                _dr["ChieuCao"] = 0;
                                string colSize = _dtTemp.Rows[i]["SizeID"].ToString() + "@" + _dtTemp.Rows[i]["Size"].ToString();
                                if (p == 0)
                                {
                                    Console.WriteLine(colSize);
                                    _dtData.Columns.Add(colSize, typeof(int));
                                    _dtData_du.Columns.Add(colSize, typeof(int));
                                }
                                var _dtslkhsize = _dtdata.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                                                                && x["ColorID"].ToString() == itemData.ColorID.ToString()
                                                                                && x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());
                                if (_dtslkhsize.Count() == 0) continue;
                                var _dtslkhsizea = _dtslkhsize.CopyToDataTable();
                                _soluongKH = _dtslkhsizea.AsEnumerable().Sum(x => Convert.ToInt32(x["SLDT"]));
                                _sltrongthung = Convert.ToInt32(_dtTemp.Rows[i]["SLPCB"].ToString());
                                int SlSize = _sltrongthung > _soluongKH ? _soluongKH : _sltrongthung;
                                if (_soluongKH == 0)
                                {
                                    continue;
                                }
                                if (_sltrongthung > 0)
                                {
                                    _songuyen = _soluongKH / _sltrongthung; // chia lấy phần nguyên
                                    _sodu = _soluongKH % _sltrongthung; //chia lấy dư
                                    _tuthung = _dednthung + 1 + startThung;
                                    _dednthung = _dednthung + (_songuyen == 0 ? 1 : _songuyen) + startThung;
                                    _carton = _dednthung - _tuthung + 1;
                                    _total = _carton * _sltrongthung;
                                    _dr["TuThung"] = _tuthung;
                                    _dr["DenThung"] = _dednthung;
                                    _dr[colSize] = SlSize/*_sltrongthung*/;
                                    _dr["Carton"] = _carton;
                                    _dr["Total"] = _total;
                                    _dr["IsThungLe"] = false;
                                    _dtData.Rows.Add(_dr);
                                    if (_sodu > 0 && _songuyen != 0)
                                    {
                                        //DataRow _drDu = _dtData.NewRow();
                                        DataRow _drDu = chkv_thungle_cuoi.Checked ? _dtData_du.NewRow() : _dtData.NewRow();

                                        _drDu["MaDVSX"] = itemDVSX.MaDVSX;
                                        _drDu["TenDVSX"] = itemDVSX.TenDVSX;
                                        _drDu["MaHang"] = _maHang;
                                        _drDu["POID"] = POID;
                                        _drDu["PO"] = PO;
                                        //_drDu["StyleID"] = _styleID;


                                        _drDu["MaLenh"] = itemDVSX.MaLenh;
                                        _drDu["DotSX"] = itemDVSX.DotSX;
                                        _drDu["SizeType"] = itemData.SizeType;
                                        _drDu["SizeTypeID"] = itemData.SizeTypeID;
                                        _drDu["TenMau"] = itemData.TenMau;
                                        _drDu["ColorID"] = itemData.ColorID;
                                        _drDu["ChieuDai"] = 0;//qcDongThungPCB.ChieuDai;
                                        _drDu["ChieuRong"] = 0;//qcDongThungPCB.ChieuRong;
                                        _drDu["ChieuCao"] = 0;//qcDongThungPCB.ChieuCao;
                                        _drDu["IsThungLe"] = true;
                                        if (chkv_thungle_cuoi.Checked)
                                        {
                                            _drDu[colSize] = _sodu;
                                            _dtData_du.Rows.Add(_drDu);
                                        }
                                        else
                                        {
                                            _tuthung = _dednthung + 1 + startThung;
                                            _dednthung = _dednthung + 1 + startThung;
                                            _drDu["TuThung"] = _tuthung;
                                            _drDu["DenThung"] = _dednthung;
                                            _carton = 1;
                                            _total = _sodu;
                                            _drDu[colSize] = _sodu;

                                            _drDu["Carton"] = _carton;
                                            _drDu["Total"] = _total;
                                            _dtData.Rows.Add(_drDu);
                                        }

                                    }

                                }
                            }
                            p++;
                        }
                        startThung = 0;
                    }
                    if (chkv_thungle_cuoi.Checked)
                    {
                        string _size = string.Empty;
                        for (int d = 0; d < _dtData_du.Rows.Count; d++)
                        {
                            if (chkb_size.Checked)
                            {
                                if (_size != _dtData_du.Columns[d].ColumnName)
                                {
                                    _tuthung = 0;
                                    _dednthung = 0;
                                    _size = _dtData_du.Columns[d].ColumnName;
                                }
                            }
                            DataRow _drDu = _dtData.NewRow();
                            //string _colgroup = "Đầu size: " + _dtData_du.Rows[d]["SizeType"].ToString() + "  - PO: " + _dtData_du.Rows[d]["PO"].ToString() + " - Màu: " + _dtData_du.Rows[d]["TenMau"].ToString();
                            //_drDu["ColGroup"] = _colgroup;

                            _drDu["MaDVSX"] = _dtData_du.Rows[d]["MaDVSX"].ToString();
                            _drDu["TenDVSX"] = _dtData_du.Rows[d]["TenDVSX"].ToString();
                            _drDu["POID"] = _dtData_du.Rows[d]["POID"].ToString();
                            _drDu["PO"] = _dtData_du.Rows[d]["PO"].ToString();
                            _drDu["MaLenh"] = _dtData_du.Rows[d]["MaLenh"].ToString();
                            _drDu["DotSX"] = _dtData_du.Rows[d]["DotSX"].ToString();

                            _drDu["MaHang"] = _dtData_du.Rows[d]["MaHang"].ToString();
                            _drDu["SizeType"] = _dtData_du.Rows[d]["SizeType"].ToString();
                            _drDu["SizeTypeID"] = _dtData_du.Rows[d]["SizeTypeID"].ToString();
                            _drDu["TenMau"] = _dtData_du.Rows[d]["TenMau"].ToString();
                            _drDu["ColorID"] = _dtData_du.Rows[d]["ColorID"].ToString();
                            _drDu["ChieuDai"] = 0;
                            _drDu["ChieuRong"] = 0;
                            _drDu["ChieuCao"] = 0;
                            _drDu["IsThungLe"] = true;
                            //_dr["Qty"] = _dtTemp.Rows[0]["Qty"].ToString();
                            //_dr["Total"] = _dtTemp.Rows[0]["Total"].ToString();


                            //_sodu = _sodu;
                            _tuthung = _dednthung + 1;
                            _dednthung = _dednthung + 1;
                            _drDu["TuThung"] = _tuthung;
                            _drDu["DenThung"] = _dednthung;
                            _carton = 1;
                            _total = 0;
                            for (int col = 21; col < _dtData_du.Columns.Count; col++)
                            {
                                int sl = _dtData_du.Rows[d][_dtData_du.Columns[col].ColumnName].ToString() == "" ? 0 : Convert.ToInt32(_dtData_du.Rows[d][_dtData_du.Columns[col].ColumnName]);
                                _drDu[_dtData_du.Columns[col].ColumnName] = sl;
                                _total = _total + sl;
                            }


                            _drDu["Carton"] = _carton;
                            _drDu["Total"] = _total;
                            _dtData.Rows.Add(_drDu);
                        }
                    }
                }
                gridControl1.MainView = GetBandGridViewAmount_grd1(_dtData);
                BandedGridView mainView = (BandedGridView)gridControl1.MainView;
                mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData_grd1;
                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate_grd1;
                gridControl1.DataSource = _dtData;
            }
            catch (Exception ex)
            {

            }
        }
        int sum = 0;
        void mainView_CustomSummaryCalculate_grd1(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName.Contains('@') && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }
        void mainView_CustomUnboundColumnData_grd1(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName.Contains('@') && col.UnboundType == UnboundColumnType.Bound)
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
        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnLuu();
        }
        private async void SaveRows()
        {
            try
            {
                DataTable tblSave = CreateTblSave();
                DateTime dtNow = DateTime.Now;
                int TuThung_Notdeca = 0;
                foreach (DataRow dr in _dtData.Rows)
                {
                    var _poid = dr["POID"].ToString();
                    var _po = dr["PO"].ToString();
                    var _sizeType = dr["SizeType"].ToString();
                    var _sizeTypeID = dr["SizeTypeID"].ToString();
                    var _colorID = dr["ColorID"].ToString();
                    string _sizeID = string.Empty;
                    string _size = string.Empty;
                    int slThung = Convert.ToInt32(dr["Carton"].ToString());
                    int TuThung = Convert.ToInt32(dr["TuThung"].ToString());
                    int DenThung = Convert.ToInt32(dr["DenThung"].ToString());

                    for (int z = 0; z < _dtData.Columns.Count; z++)
                    {
                        string[] arrName = _dtData.Columns[z].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrName.Length > 1)
                        {
                            if (dr[_dtData.Columns[z].ColumnName].ToString() == "" || dr[_dtData.Columns[z].ColumnName].ToString() == "0") continue;
                            _sizeID = arrName[0];
                            _size = arrName[1];
                            // string urlSize = string.Format("{0}/GET_SizeID?malenh={1}&&spoid={2}&&sizetypeid={3}&&colorid={4}&&sizeid={5}", URL + ResourceURL.UrlErpKHDongThung_PCB, _malenh, _spoid, _sizeTypeID, _colorID, _size);
                            //DataTable _listSize = Task.Run(async () => { return await _service_Khdt_pcb.GetDT(urlSize); }).Result;
                            //string[] arrPOID = _spoid.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            //if (dr["Size_" + size].ToString() == "" || dr["Size_" + size].ToString() == "0") continue;

                            for (int i = 0; i < slThung; i++)
                            {
                                TuThung_Notdeca++;
                                DataRow drNewRow = tblSave.NewRow();
                                drNewRow["ID"] = 0;
                                drNewRow["MaPKL"] = "";
                                drNewRow["MaDH"] = _madh;
                                drNewRow["MaDVSX"] = dr["MaDVSX"];
                                drNewRow["MaLenh"] = dr["MaLenh"];
                                drNewRow["DotSX"] = dr["DotSX"];
                                drNewRow["MaHang"] = _maHang;
                                drNewRow["POID"] = _poid;
                                drNewRow["PO"] = _po;
                                drNewRow["ColorID"] = dr["ColorID"].ToString();
                                drNewRow["TenMau"] = dr["TenMau"].ToString();
                                drNewRow["DauSize"] = dr["SizeType"].ToString();
                                drNewRow["DauSizeID"] = dr["SizeTypeID"].ToString();
                                drNewRow["SizeID"] = _sizeID;
                                drNewRow["Size"] = _size;
                                drNewRow["NgayLapKH"] = dtNow;
                                drNewRow["ChieuDai"] = dr["ChieuDai"];
                                drNewRow["ChieuRong"] = dr["ChieuRong"];
                                drNewRow["ChieuCao"] = dr["ChieuCao"];
                                drNewRow["TrongLuong"] = 0;
                                drNewRow["KhoiLuong"] = 0;
                                drNewRow["SoLuongThung"] = slThung;

                                drNewRow["TuThung"] = TuThung;
                                drNewRow["DenThung"] = DenThung;
                                if (chkb_size.Checked)
                                {
                                    drNewRow["SttThung_decat"] = TuThung + i;
                                    drNewRow["SttThung"] = TuThung_Notdeca;
                                }
                                else
                                {
                                    drNewRow["SttThung"] = TuThung + i;
                                    drNewRow["SttThung_decat"] = 0;
                                }

                                drNewRow["SoLuongSP"] = Convert.ToInt32(dr[_dtData.Columns[z].ColumnName].ToString());
                                drNewRow["IsDongThung"] = false;
                                drNewRow["NgayDongThung"] = dtNow;
                                drNewRow["NgayNhapKho"] = dtNow;
                                drNewRow["QRCode"] = "";
                                drNewRow["IsNhapKho"] = false;
                                drNewRow["KyHieu"] = string.Format("{0} -> {1}", TuThung, DenThung);
                                drNewRow["Chon"] = false;
                                drNewRow["IsThungLe"] = dr["IsThungLe"];
                                //drNewRow["SttThung_decat"] = dr["SttThung"];
                                tblSave.Rows.Add(drNewRow);
                            }
                        }
                    }
                }
                if (tblSave == null || tblSave.Rows.Count == 0)
                {
                    MessageBox.Show("Số lượng kế hoạch không được để trống, phải có ít nhất số lượng của 1 size");
                    return;
                }
                string json = JsonConvert.SerializeObject(tblSave);
                //List<ErpPCBEntity> lst = JsonConvert.DeserializeObject<List<ErpPCBEntity>>(json);
                string url = string.Format("{0}", URL + "KeHoachDongThung/Post?action=InsertKHDT");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.Close();
                    //string mss_PCB = await _service_Khdt_pcb.Post(URL + ResourceURL.UrlErpKHDongThung_PCB + "/Post", _lstSave);
                    ////string url = string.Format("{0}?maLenh={1}", URL + ResourceURL.UrlErpLenhSXPODinhMuc + "/AllowPack", txtLenhSX.Text);
                    ////string msP = Task.Run(async () => { return await _serviceLenhSXPODM.SPODMAllowWork(url); }).Result;
                    //if (string.Compare(mss_PCB, "True") != 0)
                    //{
                    //    XtraMessageBox.Show(mss_PCB, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                }
                //else
                //   MessageBox.Show(result);
                //this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable CreateTblSaveThungCuoi_DVSX()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaDVSX", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("STTThung", typeof(int));
            return dt;

        }

        private void chkv_thungle_cuoi_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProcessPackage();
        }

        private void chkb_size_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProcessPackage();
        }

        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SLPCB")
            {
                DataRow dr = grvSize.GetFocusedDataRow();
                dr["SLThung"] = Convert.ToDouble(dr["SLPCB"]) == 0 ? 0 : Math.Ceiling(Convert.ToDouble(dr["SLDT"]) / Convert.ToDouble(dr["SLPCB"]));
            }
        }

        private void gridviewDetail_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {

        }

        private void gridviewDetail_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void chbResetSLLap_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DataRow dr in dtSize.Rows)
            {
                if (chbResetSLLap.Checked)
                    dr["SLDT"] = 0;
                else
                    dr["SLDT"] = Convert.ToInt32(dr["SLKH"]) - Convert.ToInt32(dr["SLDaLapPKL"]);
            }
            grcDetail.DataSource = dtSize;
        }

        private void gridviewDetail_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            this.ActiveControl = gridControlSize;
            var dr = gridviewDetail.GetFocusedDataRow();
            if (dr is null) return;
            var SLKH = Convert.ToInt32(dr["SLKH"]);
            var SLDL = Convert.ToInt32(dr["SLDaLapPKL"]);
            var SLLap = Convert.ToInt32(e.Value);
            if (SLLap > (SLKH - SLDL))
            {
                e.Valid = false;
                e.ErrorText = "Số lượng vượt số lượng cho phép";
            }
        }

        private DataTable CreateTblSave()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPKL", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaLenh", typeof(string));
            tbl.Columns.Add("DotSX", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("ColorID", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("NgayLapKH", typeof(DateTime));
            tbl.Columns.Add("ChieuDai", typeof(double));
            tbl.Columns.Add("ChieuRong", typeof(double));
            tbl.Columns.Add("ChieuCao", typeof(double));
            tbl.Columns.Add("TrongLuong", typeof(double));
            tbl.Columns.Add("KhoiLuong", typeof(double));
            tbl.Columns.Add("SoLuongThung", typeof(int));
            tbl.Columns.Add("TuThung", typeof(int));
            tbl.Columns.Add("DenThung", typeof(int));
            tbl.Columns.Add("SttThung", typeof(int));
            tbl.Columns.Add("SoLuongSP", typeof(int));
            tbl.Columns.Add("IsDongThung", typeof(bool));
            tbl.Columns.Add("NgayDongThung", typeof(DateTime));
            tbl.Columns.Add("QRCode", typeof(string));
            tbl.Columns.Add("IsScan", typeof(bool));
            tbl.Columns.Add("IsNhapKho", typeof(bool));
            tbl.Columns.Add("NgayNhapKho", typeof(DateTime));
            tbl.Columns.Add("KyHieu", typeof(string));
            tbl.Columns.Add("Chon", typeof(bool));
            tbl.Columns.Add("SttThung_decat", typeof(int));
            tbl.Columns.Add("IsThungLe", typeof(bool));
            return tbl;
        }

        BandedGridView bandedView;
        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount_grd1(DataTable tab)
        {
            bandedView = new BandedGridView();
            //bandedView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            //bandedView.OptionsSelection.EnableAppearanceFocusedRow = false;
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsBehavior.Editable = true;
            //if (string.IsNullOrEmpty(url)) return bandedView;
            //DataTable tab = await _serviceBase.Get(url);

            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 16; i++)
            {
                List<string> _ListString = new List<string>();

                if (tab.Columns[i].ColumnName == "TuThung")
                {
                    _ListString.Add("TuThung");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Từ thùng", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "DenThung")
                {
                    _ListString.Add("DenThung");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Đến thùng", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "MaDVSX")
                {
                    _ListString.Add("MaDVSX");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Mã DVSX", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "TenDVSX")
                {
                    _ListString.Add("TenDVSX");
                    SetGridBandedViewAmount_grd1(bandedView, "", "ĐVSX", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "SizeType")
                {
                    _ListString.Add("SizeType");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Nhóm size", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Color", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "TenDVSX")
                {
                    _ListString.Add("TenDVSX");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Supplier", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "SttThung")
                {
                    _ListString.Add("SttThung");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Thùng", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "POID")
                {
                    _ListString.Add("POID");
                    SetGridBandedViewAmount_grd1(bandedView, "", "POID", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmount_grd1(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "StyleID")
                {
                    _ListString.Add("StyleID");
                    SetGridBandedViewAmount_grd1(bandedView, "", "StyleID", _ListString);
                    _ListString.Clear();
                }

                else if (tab.Columns[i].ColumnName == "MaHang")
                {
                    _ListString.Add("MaHang");
                    SetGridBandedViewAmount_grd1(bandedView, "", "MaHang", _ListString);
                    _ListString.Clear();
                }

                else if (tab.Columns[i].ColumnName == "SizeType")
                {
                    _ListString.Add("SizeType");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Nhóm size", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "SizeTypeID")
                {
                    _ListString.Add("SizeTypeID");
                    SetGridBandedViewAmount_grd1(bandedView, "", "SizeTypeID", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "ColorID")
                {
                    _ListString.Add("ColorID");
                    SetGridBandedViewAmount_grd1(bandedView, "", "ColorID", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmount_grd1(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
                else if (tab.Columns[i].ColumnName == "ColGroup")
                {
                    _ListString.Add("ColGroup");
                    SetGridBandedViewAmount_grd1(bandedView, "", "PKL =>  ", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 21;
            //_listSizeBig.Clear();
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[0];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;
            }
            SetGridBandedViewAmount_grd1(bandedView, maHang, "", listHeader);
            bandedView.FixedLineWidth = 1;
            bandedView.OptionsView.ShowFooter = true;
            //add summary on column bandedview
            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("Qty");
            colAmount.Caption = "Qty";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.OwnerBand = gridBand;
            colAmount.OptionsColumn.AllowEdit = false;
            gridBand.Visible = true;
            gridBand.Caption = "Qty";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Qty";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);

            GridBand gridBand_Carton = new GridBand();
            gridBand_Carton.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand_Carton.AppearanceHeader.Options.UseFont = true;
            gridBand_Carton.AppearanceHeader.Options.UseTextOptions = true;
            gridBand_Carton.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand_Carton.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colCarton = bandedView.Columns.AddField("Carton");
            //colCarton.Width = 60;
            colCarton.Caption = "Carton";
            colCarton.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            colCarton.Visible = true;
            //colCarton.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colCarton.OptionsColumn.AllowEdit = false;
            colCarton.OwnerBand = gridBand_Carton;
            gridBand_Carton.Visible = true;
            gridBand_Carton.Caption = "Carton";
            //GridColumnSummaryItem itemCarton = colCarton.Summary.Add(SummaryItemType.Custom);
            //itemCarton.FieldName = "Carton";
            //itemCarton.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand_Carton);

            GridBand gridBand_Total = new GridBand();
            gridBand_Total.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand_Total.AppearanceHeader.Options.UseFont = true;
            gridBand_Total.AppearanceHeader.Options.UseTextOptions = true;
            gridBand_Total.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand_Total.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            BandedGridColumn colTotal = bandedView.Columns.AddField("Total");
            colTotal.Width = 60;
            colTotal.Caption = "Total Piece";
            colTotal.Visible = true;
            colTotal.OptionsColumn.AllowEdit = false;
            colTotal.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colTotal.OwnerBand = gridBand_Total;
            gridBand_Total.Visible = true;
            gridBand_Total.Caption = "Total";

            bandedView.Bands.Add(gridBand_Total);

            GridBand gridBand_ChieuDai = new GridBand();
            gridBand_ChieuDai.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand_ChieuDai.AppearanceHeader.Options.UseFont = true;
            gridBand_ChieuDai.AppearanceHeader.Options.UseTextOptions = true;
            gridBand_ChieuDai.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand_ChieuDai.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            BandedGridColumn colChieuDai = bandedView.Columns.AddField("ChieuDai");
            colChieuDai.Width = 60;
            colChieuDai.Caption = "ChieuDai";
            colChieuDai.Visible = true;
            colChieuDai.OptionsColumn.AllowEdit = true;
            colChieuDai.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colChieuDai.AppearanceCell.Options.UseTextOptions = true;
            colChieuDai.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colChieuDai.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colChieuDai.DisplayFormat.FormatString = "n2"; // Hiển thị 2 chữ số sau dấu thập phân
            colChieuDai.OwnerBand = gridBand_ChieuDai;
            gridBand_ChieuDai.Visible = true;
            gridBand_ChieuDai.Caption = "Chiều dài";

            bandedView.Bands.Add(gridBand_ChieuDai);

            GridBand gridBand_ChieuRong = new GridBand();
            gridBand_ChieuRong.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand_ChieuRong.AppearanceHeader.Options.UseFont = true;
            gridBand_ChieuRong.AppearanceHeader.Options.UseTextOptions = true;
            gridBand_ChieuRong.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand_ChieuRong.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            BandedGridColumn colChieuRong = bandedView.Columns.AddField("ChieuRong");
            colChieuRong.Width = 60;
            colChieuRong.Caption = "ChieuRong";
            colChieuRong.Visible = true;
            //colChieuRong.OptionsColumn.AllowEdit = false;
            colChieuRong.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colChieuRong.AppearanceCell.Options.UseTextOptions = true;
            colChieuRong.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colChieuRong.OwnerBand = gridBand_ChieuRong;
            gridBand_ChieuRong.Visible = true;
            gridBand_ChieuRong.Caption = "Chiều rộng";
            bandedView.Bands.Add(gridBand_ChieuRong);

            GridBand gridBand_ChieuCao = new GridBand();
            gridBand_ChieuCao.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand_ChieuCao.AppearanceHeader.Options.UseFont = true;
            gridBand_ChieuCao.AppearanceHeader.Options.UseTextOptions = true;
            gridBand_ChieuCao.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand_ChieuCao.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            BandedGridColumn colChieuCao = bandedView.Columns.AddField("ChieuCao");
            colChieuCao.Width = 60;
            colChieuCao.Caption = "ChieuCao";
            colChieuCao.Visible = true;
            //colChieuCao.OptionsColumn.AllowEdit = false;
            colChieuCao.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colChieuCao.AppearanceCell.Options.UseTextOptions = true;
            colChieuCao.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colChieuCao.OwnerBand = gridBand_ChieuCao;
            gridBand_ChieuCao.Visible = true;
            gridBand_ChieuCao.Caption = "Chiều cao";
            bandedView.Bands.Add(gridBand_ChieuCao);

            foreach (BandedGridColumn col in bandedView.Columns)
            {
                if (col.FieldName != "TuThung" && col.FieldName != "DenThung" && col.FieldName != "SttThung" && col.FieldName != "ChieuDai" && col.FieldName != "ChieuRong" && col.FieldName != "ChieuCao" && col.FieldName != "SPOID" && col.FieldName != "PO" && col.FieldName != "StyleID" && col.FieldName != "MaHang" && col.FieldName != "SizeType" && col.FieldName != "SizeTypeID" && col.FieldName != "TenMau" && col.FieldName != "ColorID"/* && col.FieldName != "Carton"*//* && col.FieldName != "Total" */&& col.FieldName != "ColGroup")
                {
                    GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                    itemSize.FieldName = col.FieldName;
                    itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    itemSize.DisplayFormat = "{0:n0}";
                    itemSize.ShowInGroupColumnFooter = col;
                    bandedView.GroupSummary.Add(itemSize);
                }
                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1 || col.FieldName == "Carton" || col.FieldName == "Total")
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }

            }
            //bandedView.DoubleClick += bandedView_DoubleClick;
            //bandedView.CellValueChanged += bandedView_CellValueChanged;
            bandedView.CustomDrawBandHeader += bandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += bandedView_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += bandedView_CustomDrawFooter;

            bandedView.OptionsSelection.MultiSelect = true;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            bandedView.OptionsBehavior.AutoExpandAllGroups = true;
            return bandedView;
        }

        private void SetGridBandedViewAmount_grd1(BandedGridView bandedView, string GridBandHeader, string GridBandCaption, List<string> columnNames)
        {
            GridBand gridBand = new GridBand();
            if (GridBandHeader == "")
                gridBand.Caption = GridBandCaption;
            else
                gridBand.Caption = GridBandHeader;
            int nrOfColumns = columnNames.Count;
            gridBand.Fixed = FixedStyle.None;
            //gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (nrOfColumns == 1 && (columnNames[0] == "TuThung" || columnNames[0] == "DenThung" || columnNames[0] == "MaDVSX" || columnNames[0] == "TenDVSX" || columnNames[0] == "SizeType" || columnNames[0] == "TenMau" || columnNames[0] == "SttThung" || columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "StyleID" || columnNames[0] == "MaHang" || columnNames[0] == "SizeType" || columnNames[0] == "SizeTypeID" || columnNames[0] == "TenMau" || columnNames[0] == "ColorID") || columnNames[0] == "Carton" || columnNames[0] == "ColGroup" || columnNames[0] == "ChieuDai" || columnNames[0] == "ChieuRong" || columnNames[0] == "ChieuCao")
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);

                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;

                if (columnNames[0] == "ColGroup" || columnNames[0] == "SizeTypeID" || columnNames[0] == "SttThung" || columnNames[0] == "ColorID" || columnNames[0] == "POID" || columnNames[0] == "StyleID" || columnNames[0] == "MaHang" || columnNames[0] == "SizeTypeID" || columnNames[0] == "ColorID" || columnNames[0] == "ColGroup")
                {
                    gridBand.Visible = false;
                }
                else
                    gridBand.Visible = true;
                bandedColumns.OptionsColumn.AllowEdit = false;
                //if (columnNames[0] == "ColGroup")
                //{
                //    bandedColumns.GroupIndex = 0;
                //}

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
                    gridband3.Caption = _colName[1];
                    //columnNames[i];//
                    //string Name = string.Empty;
                    bandedColumns[i] = (BandedGridColumn)bandedView.Columns.AddField(columnNames[i]);
                    //Name = columnNames[i];
                    bandedColumns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //bandedColumns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    //bandedColumns[i].UnboundType = UnboundColumnType.Integer;
                    bandedColumns[i].DisplayFormat.FormatString = "{0:##,0}";
                    bandedColumns[i].Width = 60;
                    bandedColumns[i].OptionsColumn.AllowEdit = false;
                    gridband3.Columns.Add(bandedColumns[i]);
                    bandedColumns[i].OwnerBand = gridband3;
                    bandedColumns[i].Visible = true;
                    gridband3.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    gridband3.AppearanceHeader.Options.UseFont = true;
                    gridband3.AppearanceHeader.Options.UseTextOptions = true;
                    gridband3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    grHeader[i] = gridband3;

                }
                gridBand.Children.AddRange(grHeader);
                bandedView.Bands.Add(gridBand);

            }
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

            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.DisplayText = "-";
            }
            else
            {
                //if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
                //{
                //    var value = bandedView.GetRowCellValue(e.GroupRowHandle, "MaDVSX");
                //    if (value != null && value.ToString() == "DVSX002")
                //    {
                //        Console.WriteLine(value);
                //        e.DisplayText = (Convert.ToInt32(e.Value.ToString()) + 240).ToString();
                //    }

                //}



                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToInt32(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
        }

        void bandedView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;

        }
        private void BtnLuu()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            SaveRows();
        }
       
    }
}
