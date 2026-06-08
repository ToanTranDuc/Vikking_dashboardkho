using DevExpress.Data;
using DevExpress.Utils.Menu;
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
    public partial class frmERPKeHoachDongThungV5 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        SearchCheckSelection gridCheckMarksDVSX;
        SearchCheckSelection gridCheckMarksPO;
        SearchCheckSelection gridCheckMarksSizeType;
        public static string POID = "";
        private string _maPKL = string.Empty, _madh = string.Empty, _maHang = string.Empty, _maDVSX = string.Empty, _poid = string.Empty, _po = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty;
        private string _maNoiDen = "0";
        DataTable dtNoiDen = new DataTable();
        DataTable dtSize = new DataTable();
        DataTable dtSizeAll = new DataTable();
        DataTable _dtData = new DataTable();
        DataTable _dtInit = new DataTable();
        DataTable dtDataPiVot = new DataTable();
        DataTable dtQuiCach = new DataTable();
        int _carton = 0, thung_po = 0;
        bool CheckTheoMau = false;
        KeyDownControlHandler keyDownControlHandler;
        public frmERPKeHoachDongThungV5(string MaDH, string MaDHDisplay, string MaHang, string TenHang, string POID, string MaPKL)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _madh = MaDH;
            _maHang = MaHang;
            _poid = POID;
            _maPKL = MaPKL;
            txtMahang.Text = TenHang;
            txtMaDH.Text = MaDHDisplay;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            chbxTheoMau.CheckedChanged += RepocbxTheoMau_CheckedChanged;
            KHDongThungLib.InitQuiCach(repoQuiCach);
            dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension, _maNoiDen);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateDefault();
            GetKHDongThung();
            LoadNoiDen();
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
            AddActionControl(_lstActionControl, BtnSave, true, ActionType.Save);
            AddActionControl(_lstActionControl, GetSLDM, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, GopThung, true, ActionType.GopThung);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void CreateDefault()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDVSX&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
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
            DataTable _dtDVSXTemp = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            var _dtDVSXTemp1 = _dtDVSXTemp.AsEnumerable().Where(x => KHDongThungLib.CheckRole(x["MaDVSX"].ToString()));
            DataTable _dtDVSX = _dtDVSXTemp1.Count() > 0 ? _dtDVSXTemp1.CopyToDataTable() : new DataTable();


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

            searchLookUpEditSizeType.Properties.ValueMember = "valueSizeType";
            searchLookUpEditSizeType.Properties.DisplayMember = "SizeType";
            searchLookUpEditSizeType.Properties.NullText = "[Chọn đầu size]";
            //searchLookUpEditSizeType.Properties.Appearance.ForeColor = Color.Red;
            searchLookUpEditSizeType.Properties.View.OptionsSelection.MultiSelect = true;
            searchLookUpEditSizeType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditSizeType_CustomDisplayText);
            searchLookUpEditSizeType.Properties.PopulateViewColumns();
            //gridCheckMarksSizeType = new SearchCheckSelection(searchLookUpEditSizeType.Properties);
            //gridCheckMarksSizeType.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditSizeType_SelectionChanged);
            //searchLookUpEditSizeType.Properties.Tag = gridCheckMarksSizeType;

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

        #region View KH đóng thùng
        private void GetDSAllSize()
        {
            if (_maPKL == "") return;
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetSizeA&MaDH={_madh}&MaDVSX=Para&DotSX=Para" +
                                                    $"&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtSizeAll = JsonConvert.DeserializeObject<DataTable>(json);
            CreateBandSize(dtSizeAll);
        }
        private void ProcessPackage()
        {
            try
            {
                DataTable _dtTemp11 = (DataTable)gridControlSize.DataSource;

                if (_dtTemp11 is null) return;
                List<CheckThungLe> lstCheckThungLe = new List<CheckThungLe>();
                var _dtTemp1 = _dtTemp11.AsEnumerable().Select(x => new
                {
                    SizeID = x["SizeID"].ToString(),
                    Size = x["Size"].ToString()
                }).Distinct().ToList();
                string json = JsonConvert.SerializeObject(_dtTemp1);
                DataTable _dtTemp = JsonConvert.DeserializeObject<DataTable>(json);
                CreateBandSize(_dtTemp);
                _dtData.Clear();
                dtSize = grcDetail.DataSource as DataTable;
                if (_maPKL != "" && dtSize.Rows.Count > 0)
                {
                    var PO_Old = dtDataPiVot.Rows[0]["POID"].ToString();
                    var PO_New = dtSize.Rows[0]["POID"].ToString();
                    if (PO_Old != PO_New)
                    {
                        MessageBox.Show("Packing list đã tồn tại PO khác!");
                        return;
                    }
                }
                var _dttempPO = dtSize.AsEnumerable().Select(x => new
                {
                    POID = x["POID"],
                    PO = x["PO"]
                }).Distinct().ToList();
                _dtData = KHDongThungLib.CreateTblPackage();

                DataTable _dtData_du = _dtData.Copy();
                int p = 0;
                int ID = 10000;

                int startThung = 0;
                int _tuthung = 0, _dednthung = 0, _soluongKH = 0, _sltrongthung = 0, _sltemp = 0, _songuyen = 0, _sodu = 0, _total;
                _dednthung = txtStart.Text != "" ? Convert.ToInt16(txtStart.Text) - 1 : GetMaxThung();
                double ChieuDai = 0, ChieuRong = 0, ChieuCao = 0, TLThungChan = 0;
                double ChieuDaiLe = 0, ChieuRongLe = 0, ChieuCaoLe = 0, TLThungLe = 0;
                string KiHieu = "0x0x0", KiHieuLe = "0x0x0", MaQuiCach = "", MaQuiCachLe = "";
                bool CheckLayQCLe = false;
                bool CheckLapPack = false;
                if (dtQuiCach != null && dtQuiCach.Rows.Count > 0)
                {
                    ChieuDai = Convert.ToDouble(dtQuiCach.Rows[0]["ChieuDai"]);
                    ChieuRong = Convert.ToDouble(dtQuiCach.Rows[0]["ChieuRong"]);
                    ChieuCao = Convert.ToDouble(dtQuiCach.Rows[0]["ChieuCao"]);
                    TLThungChan = Convert.ToDouble(dtQuiCach.Rows[0]["TLThung"]);
                    KiHieu = dtQuiCach.Rows[0]["ChieuDai"] + "x" + dtQuiCach.Rows[0]["ChieuRong"] + 'x' + dtQuiCach.Rows[0]["ChieuCao"];
                    MaQuiCach = dtQuiCach.Rows[0]["MaQuiCach"].ToString();
                }
                if (dtQuiCach != null && dtQuiCach.Rows.Count > 1)
                {
                    ChieuDaiLe = Convert.ToDouble(dtQuiCach.Rows[1]["ChieuDai"]);
                    ChieuRongLe = Convert.ToDouble(dtQuiCach.Rows[1]["ChieuRong"]);
                    ChieuCaoLe = Convert.ToDouble(dtQuiCach.Rows[1]["ChieuCao"]);
                    TLThungLe = Convert.ToDouble(dtQuiCach.Rows[1]["TLThung"]);
                    KiHieuLe = dtQuiCach.Rows[1]["ChieuDai"] + "x" + dtQuiCach.Rows[1]["ChieuRong"] + 'x' + dtQuiCach.Rows[1]["ChieuCao"];
                    MaQuiCachLe = dtQuiCach.Rows[1]["MaQuiCach"].ToString();
                    CheckLayQCLe = true;
                }

                foreach (var itemPO in _dttempPO)
                {
                    if (itemPO.POID.ToString() != searchLookUpEditPO.EditValue.ToString()) continue;
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
                        var _dtdata = _dtdataPO.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString() && x["MaLenh"].ToString() == itemDVSX.MaLenh.ToString()).CopyToDataTable();
                        var _dtDistinctDT = _dtdata.AsEnumerable().Select(x => new
                        {
                            SizeTypeID = x["SizeTypeID"],
                            SizeType = x["SizeType"],
                        }).Distinct();
                        foreach (var itemData in _dtDistinctDT)
                        {
                            var _dtDistinct_Mau = _dtdata.AsEnumerable().Where(y => y["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()).Select(x => new
                            {
                                TenMau = x["TenMau"],
                                ColorID = x["ColorID"],
                            }).Distinct();
                            foreach (var itemMau in _dtDistinct_Mau)
                            {
                                if (CheckTheoMau && !chkb_size.Checked)
                                {
                                    _tuthung = 0;
                                    _dednthung = 0;
                                }
                                for (int i = 0; i < _dtTemp.Rows.Count; i++)
                                {
                                    var resultQC = KHDongThungLib.GetQuiCachSize(dtQuiCach, itemData.SizeTypeID.ToString(), _dtTemp.Rows[i]["SizeID"].ToString());
                                    MaQuiCach = resultQC.Item1;
                                    MaQuiCachLe = resultQC.Item2;
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
                                    var _dtTempA = _dtdata.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                                                                      && x["ColorID"].ToString() == itemMau.ColorID.ToString()
                                                                                      && x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());
                                    var _dtTempA1 = _dtTempA.Count() > 0 ? _dtTempA.CopyToDataTable() : new DataTable();
                                    double NW, GW;
                                    if (_dtTempA1.Rows.Count != 0)
                                    {
                                        NW = _dtTempA1.Rows[0]["NW"].ToString() == "" ? 0 : Convert.ToDouble(_dtTempA1.Rows[0]["NW"]);
                                        GW = _dtTempA1.Rows[0]["GW"].ToString() == "" ? 0 : Convert.ToDouble(_dtTempA1.Rows[0]["GW"]);
                                    }
                                    else
                                    {
                                        NW = 0;
                                        GW = 0;
                                    }
                                    //var GW = _dtTemp.Rows[i]["GW"].ToString() == "" ? 0 : Convert.ToDouble(_dtTemp.Rows[i]["GW"]);
                                    DataRow _dr = _dtData.NewRow();
                                    _dr["ID"] = ID;
                                    _dr["POID"] = POID;
                                    _dr["PO"] = PO;
                                    //_dr["StyleID"] = _styleID;
                                    _dr["MaHang"] = _maHang;
                                    _dr["MaDVSX"] = itemDVSX.MaDVSX;
                                    _dr["TenDVSX"] = itemDVSX.TenDVSX;
                                    _dr["MaLenh"] = itemDVSX.MaLenh;
                                    _dr["DotSX"] = itemDVSX.DotSX;
                                    _dr["DauSize"] = itemData.SizeType;
                                    _dr["DauSizeID"] = itemData.SizeTypeID;
                                    _dr["TenMau"] = itemMau.TenMau;
                                    _dr["ColorID"] = itemMau.ColorID;
                                    _dr["ChieuDai"] = ChieuDai;
                                    _dr["ChieuRong"] = ChieuRong;
                                    _dr["ChieuCao"] = ChieuCao;
                                    _dr["KyHieu"] = MaQuiCach;
                                    _dr["SttThung"] = ID;
                                    _dr["Chon"] = false;
                                    _dr["IsSave"] = false;
                                    _dr["KieuLap"] = CheckTheoMau ? "1" : "0";
                                    string colSize = _dtTemp.Rows[i]["Size"].ToString() + "@" + _dtTemp.Rows[i]["SizeID"].ToString();
                                    if (p == 0)
                                    {
                                        Console.WriteLine(colSize);
                                        _dtData.Columns.Add(colSize, typeof(int));
                                        _dtData_du.Columns.Add(colSize, typeof(int));
                                        lstCheckThungLe.Add(new CheckThungLe()
                                        {
                                            SizeID = _dtTemp.Rows[i]["SizeID"].ToString()
                                        });
                                    }
                                    if (_dtTemp.Rows[i]["SizeID"].ToString() == "SIZE_L")
                                    {

                                    }
                                    var checkActive = lstCheckThungLe.Where(x => x.SizeID == _dtTemp.Rows[i]["SizeID"].ToString()).FirstOrDefault();
                                    var _dtslkhsize = dtSize.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString()
                                                                                    && x["MaLenh"].ToString() == itemDVSX.MaLenh.ToString()
                                                                                    && x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                                                                    && x["ColorID"].ToString() == itemMau.ColorID.ToString()
                                                                                    && x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());
                                    if (_dtslkhsize.Count() == 0) continue;
                                    var _dtslkhsizea = _dtslkhsize.CopyToDataTable();
                                    _soluongKH = _dtslkhsizea.AsEnumerable().Sum(x => Convert.ToInt32(x["SLDT"]));

                                    var _dtTempPCB = _dtTemp11.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString() &&
                                                                                     x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString()).CopyToDataTable();
                                    var SLPCB = _dtTempPCB.Rows[0]["SLPCB"].ToString() == "" ? 0 : _dtTempPCB.Rows[0]["SLPCB"];
                                    //var SLPCB = _dtslkhsizea.Rows[0]["SLPCB"].ToString() == "" ? 0 : _dtslkhsizea.Rows[0]["SLPCB"];

                                    var PCB_Pack = _dtTempPCB.Rows[0]["PCB_Pack"].ToString();
                                    var Pack_Cnt = _dtTempPCB.Rows[0]["Pack_Ctn"].ToString();
                                    var SLPCBTemp = Convert.ToInt16(PCB_Pack) * Convert.ToInt16(Pack_Cnt);
                                    CheckLapPack = (PCB_Pack != "0" && Pack_Cnt != "0") ? true : false;
                                    _sltrongthung = CheckLapPack ? Convert.ToInt16(PCB_Pack) * Convert.ToInt16(Pack_Cnt) : Convert.ToInt32(SLPCB);
                                    int SlSize = _sltrongthung > _soluongKH ? _soluongKH : _sltrongthung;
                                    bool CheckIsThungLe = _sltrongthung > _soluongKH ? true : false;
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

                                        var _PCB_Pack = Convert.ToInt16(PCB_Pack);
                                        var _soLuong = _songuyen == 0 ? 0 : _sltrongthung;
                                        var _Pack_Cnt = Convert.ToInt32(Pack_Cnt);

                                        _total = _carton * SlSize;
                                        _dr["TuThung"] = _tuthung;
                                        _dr["DenThung"] = _dednthung;
                                        _dr[colSize] = CheckLapPack ? _PCB_Pack : SlSize/*_sltrongthung*/;
                                        _dr["PCB_Pack"] = _PCB_Pack;
                                        _dr["Pack_Ctn"] = _Pack_Cnt;
                                        _dr["SoLuong"] = _soLuong;
                                        _dr["SLThung"] = _carton;
                                        _dr["TotalPiece"] = _total;
                                        _dr["IsThungLe"] = false;
                                        var KhoiLuong = Math.Round(_total * NW, 1);
                                        var TrongLuong = KhoiLuong + TLThungChan * _carton;
                                        _dr["TrongLuong"] = TrongLuong;
                                        _dr["KhoiLuong"] = KhoiLuong;
                                        _dr["TrongLuongA"] = TLThungChan;

                                        //_dr["TrongLuong"] = Math.Round(_carton * GW, 1);
                                        //_dr["KhoiLuong"] = Math.Round(_carton * NW, 1);
                                        if (!CheckIsThungLe)
                                        {
                                            checkActive.IsActive = true;
                                            _dtData.Rows.Add(_dr);
                                        }

                                        if ((_sodu > 0 && _songuyen != 0) || CheckIsThungLe)
                                        {
                                            ID++;
                                            DataRow _drDu = (chkv_thungle_cuoi.Checked && checkActive.IsActive) ? _dtData_du.NewRow() : _dtData.NewRow();
                                            DataRow _drDuA = (chkv_thungle_cuoi.Checked && checkActive.IsActive) ? _dtData_du.NewRow() : _dtData.NewRow();
                                            _Pack_Cnt = (_sodu / Convert.ToInt16(PCB_Pack));
                                            var _sodu2 = (_sodu % Convert.ToInt16(PCB_Pack));
                                            if (_sodu2 > 0) _sodu = _sodu - _sodu2;
                                            _drDu["ID"] = ID;
                                            _drDu["MaDVSX"] = itemDVSX.MaDVSX;
                                            _drDu["TenDVSX"] = itemDVSX.TenDVSX;
                                            _drDu["MaHang"] = _maHang;
                                            _drDu["POID"] = POID;
                                            _drDu["PO"] = PO;
                                            _drDu["MaLenh"] = itemDVSX.MaLenh;
                                            _drDu["DotSX"] = itemDVSX.DotSX;
                                            _drDu["DauSize"] = itemData.SizeType;
                                            _drDu["DauSizeID"] = itemData.SizeTypeID;
                                            _drDu["TenMau"] = itemMau.TenMau;
                                            _drDu["ColorID"] = itemMau.ColorID;
                                            bool CheckKLThung = (_sodu > Math.Round(Convert.ToDouble(_sltrongthung) / 2, 1) ? true : false) || !CheckLayQCLe;
                                            _drDu["ChieuDai"] = CheckKLThung ? ChieuDai : ChieuDaiLe;
                                            _drDu["ChieuRong"] = CheckKLThung ? ChieuRong : ChieuRongLe;
                                            _drDu["ChieuCao"] = CheckKLThung ? ChieuCao : ChieuCaoLe;
                                            _drDu["KyHieu"] = CheckKLThung ? MaQuiCach : MaQuiCachLe;
                                            _drDu["IsThungLe"] = true;// checkActive.IsActive ?  true :false;
                                            _drDu["SttThung"] = ID;
                                            _drDu["Chon"] = false;
                                            _drDu["IsSave"] = false;
                                            KhoiLuong = Math.Round(_sodu * NW, 1);
                                            TrongLuong = KhoiLuong + (CheckKLThung ? TLThungChan : TLThungLe);
                                            _drDu["TrongLuong"] = TrongLuong;
                                            _drDu["KhoiLuong"] = KhoiLuong;
                                            _drDu["TrongLuongA"] = CheckKLThung ? TLThungChan : TLThungLe;
                                            _drDu["KieuLap"] = CheckTheoMau ? "1" : "0";
                                            _drDu["SoLuong"] = _sodu;
                                            _drDu["PCB_Pack"] = _PCB_Pack;
                                            _drDu["Pack_Ctn"] = _Pack_Cnt == 0 ? 1 : _Pack_Cnt;

                                            if (chkv_thungle_cuoi.Checked && checkActive.IsActive)
                                            {
                                                if (CheckIsThungLe) _dednthung = _dednthung - 1;
                                                _drDu["TotalPiece"] = _sodu;
                                                _drDu[colSize] = CheckLapPack ? _PCB_Pack : _sodu;
                                                // _dtData_du.Rows.Add(_drDu);
                                            }
                                            else
                                            {
                                                _tuthung = _dednthung + 1 + startThung;
                                                _dednthung = _dednthung + 1 + startThung;
                                                if (CheckIsThungLe)
                                                {
                                                    _tuthung -= 1;
                                                    _dednthung -= 1;
                                                }
                                                _drDu["TuThung"] = _tuthung;
                                                _drDu["DenThung"] = _dednthung;
                                                _carton = 1;
                                                _total = _sodu;
                                                _drDu[colSize] = CheckLapPack ? _PCB_Pack : _sodu;
                                                _drDu["SLThung"] = _carton;
                                                _drDu["TotalPiece"] = _total;
                                                // _dtData.Rows.Add(_drDu);
                                            }

                                            if (_sodu2 > 0)
                                            {

                                                KHDongThungLib.CopyDataRow(_drDu, _drDuA);
                                                KhoiLuong += Math.Round(_sodu2 * NW, 1);
                                                TrongLuong += KhoiLuong + (CheckKLThung ? TLThungChan : TLThungLe);
                                                _drDu["TrongLuong"] = TrongLuong;
                                                _drDu["KhoiLuong"] = KhoiLuong;
                                                _drDuA["TrongLuong"] = TrongLuong;
                                                _drDuA["KhoiLuong"] = KhoiLuong;
                                                _drDuA["TrongLuongA"] = CheckKLThung ? TLThungChan : TLThungLe;

                                                _drDuA["SoLuong"] = _sodu2;
                                                _drDuA["PCB_Pack"] = _sodu2;
                                                _drDuA["Pack_Ctn"] = 1;
                                                if (chkv_thungle_cuoi.Checked && checkActive.IsActive)
                                                {
                                                    if (CheckIsThungLe) _dednthung = _dednthung - 1;
                                                    _drDuA["TotalPiece"] = _sodu2;
                                                    _drDuA[colSize] = _sodu2;

                                                    _dtData_du.Rows.Add(_drDu);
                                                    _dtData_du.Rows.Add(_drDuA);
                                                }
                                                else
                                                {
                                                    _drDuA["TuThung"] = _tuthung;
                                                    _drDuA["DenThung"] = _dednthung;
                                                    _carton = 1;
                                                    _total = _sodu2;
                                                    _drDuA[colSize] = _sodu2;
                                                    _drDuA["SLThung"] = _carton;
                                                    _drDuA["TotalPiece"] = _total;
                                                    _drDu["IsThungLe"] = false;
                                                    _drDuA["IsThungLe"] = false;
                                                    _dtData.Rows.Add(_drDu);
                                                    _dtData.Rows.Add(_drDuA);
                                                }
                                            }
                                            else
                                            {
                                                if (!checkActive.IsActive) _drDu["IsThungLe"] = false;
                                                if (chkv_thungle_cuoi.Checked && checkActive.IsActive) _dtData_du.Rows.Add(_drDu);
                                                else
                                                {

                                                    _dtData.Rows.Add(_drDu);
                                                }

                                            }
                                        }
                                    }
                                    ID++;
                                    checkActive.IsActive = true;
                                }
                                p++;
                                if (chkv_thungle_cuoi.Checked && CheckTheoMau)
                                {
                                    string _size = string.Empty;
                                    string _SttThungOld = "";
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
                                        _drDu["ID"] = ID;
                                        _drDu["MaDVSX"] = _dtData_du.Rows[d]["MaDVSX"].ToString();
                                        _drDu["TenDVSX"] = _dtData_du.Rows[d]["TenDVSX"].ToString();
                                        _drDu["POID"] = _dtData_du.Rows[d]["POID"].ToString();
                                        _drDu["PO"] = _dtData_du.Rows[d]["PO"].ToString();
                                        _drDu["MaLenh"] = _dtData_du.Rows[d]["MaLenh"].ToString();
                                        _drDu["DotSX"] = _dtData_du.Rows[d]["DotSX"].ToString();
                                        _drDu["MaHang"] = _dtData_du.Rows[d]["MaHang"].ToString();
                                        _drDu["DauSize"] = _dtData_du.Rows[d]["DauSize"].ToString();
                                        _drDu["DauSizeID"] = _dtData_du.Rows[d]["DauSizeID"].ToString();
                                        _drDu["TenMau"] = _dtData_du.Rows[d]["TenMau"].ToString();
                                        _drDu["ColorID"] = _dtData_du.Rows[d]["ColorID"].ToString();
                                        _drDu["ChieuDai"] = Convert.ToInt32(_dtData_du.Rows[d]["ChieuDai"]);
                                        _drDu["ChieuRong"] = Convert.ToInt32(_dtData_du.Rows[d]["ChieuRong"]);
                                        _drDu["ChieuCao"] = Convert.ToInt32(_dtData_du.Rows[d]["ChieuCao"]);
                                        _drDu["KyHieu"] = _dtData_du.Rows[d]["KyHieu"];
                                        _drDu["IsThungLe"] = true;
                                        _drDu["Chon"] = false;
                                        _drDu["IsSave"] = false;
                                        //_dr["Qty"] = _dtTemp.Rows[0]["Qty"].ToString();
                                        //_dr["Total"] = _dtTemp.Rows[0]["Total"].ToString();
                                        if (_SttThungOld != _dtData_du.Rows[d]["SttThung"].ToString())
                                        {
                                            _tuthung = _dednthung + 1;
                                            _dednthung = _dednthung + 1;
                                            _SttThungOld = _dtData_du.Rows[d]["SttThung"].ToString();
                                        }
                                        _drDu["TuThung"] = _tuthung;
                                        _drDu["DenThung"] = _dednthung;
                                        _drDu["SttThung"] = _dtData_du.Rows[d]["SttThung"].ToString();
                                        _carton = 1;
                                        _total = 0;
                                        foreach (DataColumn dc in _dtData_du.Columns)
                                        {
                                            var colName = dc.ColumnName;
                                            if (!colName.Contains("@")) continue;
                                            int sl = _dtData_du.Rows[d][colName].ToString() == "" ? 0 : Convert.ToInt32(_dtData_du.Rows[d][colName]);
                                            _drDu[colName] = sl;
                                            _total = _total + sl;
                                        }
                                        _drDu["SoLuong"] = _dtData_du.Rows[d]["SoLuong"];
                                        _drDu["PCB_Pack"] = _dtData_du.Rows[d]["PCB_Pack"];
                                        _drDu["Pack_Ctn"] = _dtData_du.Rows[d]["Pack_Ctn"];
                                        _drDu["TuThung"] = _tuthung;
                                        _drDu["SLThung"] = _carton;
                                        _drDu["TotalPiece"] = _dtData_du.Rows[d]["TotalPiece"];
                                        _drDu["TrongLuong"] = _dtData_du.Rows[d]["TrongLuong"];
                                        _drDu["TrongLuongA"] = _dtData_du.Rows[d]["TrongLuongA"];
                                        _drDu["KhoiLuong"] = _dtData_du.Rows[d]["KhoiLuong"];
                                        _drDu["KieuLap"] = _dtData_du.Rows[d]["KieuLap"];
                                        _dtData.Rows.Add(_drDu);
                                        ID++;
                                    }
                                    _dtData_du.Clear();
                                }
                            }
                        }
                        startThung = 0;

                    }
                    if (chkv_thungle_cuoi.Checked && !CheckTheoMau)
                    {
                        string _size = string.Empty;
                        string _SttThungOld = "";
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
                            _drDu["ID"] = ID;
                            _drDu["MaDVSX"] = _dtData_du.Rows[d]["MaDVSX"].ToString();
                            _drDu["TenDVSX"] = _dtData_du.Rows[d]["TenDVSX"].ToString();
                            _drDu["POID"] = _dtData_du.Rows[d]["POID"].ToString();
                            _drDu["PO"] = _dtData_du.Rows[d]["PO"].ToString();
                            _drDu["MaLenh"] = _dtData_du.Rows[d]["MaLenh"].ToString();
                            _drDu["DotSX"] = _dtData_du.Rows[d]["DotSX"].ToString();
                            _drDu["MaHang"] = _dtData_du.Rows[d]["MaHang"].ToString();
                            _drDu["DauSize"] = _dtData_du.Rows[d]["DauSize"].ToString();
                            _drDu["DauSizeID"] = _dtData_du.Rows[d]["DauSizeID"].ToString();
                            _drDu["TenMau"] = _dtData_du.Rows[d]["TenMau"].ToString();
                            _drDu["ColorID"] = _dtData_du.Rows[d]["ColorID"].ToString();
                            _drDu["ChieuDai"] = Convert.ToInt32(_dtData_du.Rows[d]["ChieuDai"]);
                            _drDu["ChieuRong"] = Convert.ToInt32(_dtData_du.Rows[d]["ChieuRong"]);
                            _drDu["ChieuCao"] = Convert.ToInt32(_dtData_du.Rows[d]["ChieuCao"]);
                            _drDu["KyHieu"] = _dtData_du.Rows[d]["KyHieu"];
                            _drDu["IsThungLe"] = true;
                            _drDu["Chon"] = false;
                            _drDu["IsSave"] = false;
                            //_dr["Qty"] = _dtTemp.Rows[0]["Qty"].ToString();
                            //_dr["Total"] = _dtTemp.Rows[0]["Total"].ToString();
                            if (_SttThungOld != _dtData_du.Rows[d]["SttThung"].ToString())
                            {
                                _tuthung = _dednthung + 1;
                                _dednthung = _dednthung + 1;
                                _SttThungOld = _dtData_du.Rows[d]["SttThung"].ToString();
                            }

                            _drDu["TuThung"] = _tuthung;
                            _drDu["DenThung"] = _dednthung;
                            _drDu["SttThung"] = _dtData_du.Rows[d]["SttThung"].ToString();
                            _carton = 1;
                            _total = 0;
                            foreach (DataColumn dc in _dtData_du.Columns)
                            {
                                var colName = dc.ColumnName;
                                if (!colName.Contains("@")) continue;
                                int sl = _dtData_du.Rows[d][colName].ToString() == "" ? 0 : Convert.ToInt32(_dtData_du.Rows[d][colName]);
                                _drDu[colName] = sl;
                                _total = _total + sl;
                            }
                            _drDu["SoLuong"] = _dtData_du.Rows[d]["SoLuong"];
                            _drDu["PCB_Pack"] = _dtData_du.Rows[d]["PCB_Pack"];
                            _drDu["Pack_Ctn"] = _dtData_du.Rows[d]["Pack_Ctn"];
                            _drDu["SLThung"] = _carton;
                            _drDu["TotalPiece"] = _dtData_du.Rows[d]["TotalPiece"];
                            //_drDu["TotalPiece"] = _total;
                            _drDu["TrongLuong"] = _dtData_du.Rows[d]["TrongLuong"];
                            _drDu["KhoiLuong"] = _dtData_du.Rows[d]["KhoiLuong"];
                            _drDu["TrongLuongA"] = _dtData_du.Rows[d]["TrongLuongA"];
                            _drDu["KieuLap"] = _dtData_du.Rows[d]["KieuLap"];
                            _dtData.Rows.Add(_drDu);
                            ID++;
                        }
                        _dtData_du.Clear();
                    }
                }
                var dataNew = dtDataPiVot.Copy();
                if (!CheckLapPack)
                {
                    foreach (DataRow dr in _dtData.Rows)
                    {

                        UInt32 slSP = 0;
                        foreach (DataColumn dc in _dtData.Columns)
                        {
                            if (!dc.ColumnName.Contains("@")) continue;
                            var _sizeID = dc.ColumnName.Split('@')[0];
                            slSP += dr[dc.ColumnName].ToString() != "" ? Convert.ToUInt32(dr[dc.ColumnName]) : 0;
                        }
                        dr["SoLuong"] = slSP;
                    }
                }

                int ydex = 10000;
                if (_maPKL == "") dataNew = _dtData;
                else
                {
                    foreach (DataRow dr in _dtData.Rows)
                    {
                        UInt32 slSP = 0;
                        var drNew = dataNew.NewRow();
                        drNew["ID"] = dr["ID"];
                        drNew["MaDVSX"] = dr["MaDVSX"];
                        drNew["TenDVSX"] = dr["TenDVSX"];
                        drNew["MaLenh"] = dr["MaLenh"];
                        drNew["DotSX"] = dr["DotSX"];
                        drNew["MaHang"] = dr["Mahang"];
                        drNew["DauSize"] = dr["DauSize"];
                        drNew["DauSizeID"] = dr["DauSizeID"];
                        drNew["POID"] = dr["POID"];
                        drNew["PO"] = dr["PO"];
                        drNew["ColorID"] = dr["ColorID"];
                        drNew["TenMau"] = dr["TenMau"];
                        drNew["TuThung"] = dr["TuThung"];
                        drNew["DenThung"] = dr["DenThung"];
                        drNew["SLThung"] = dr["SLThung"];
                        drNew["TotalPiece"] = dr["TotalPiece"];
                        drNew["MaDVSX"] = dr["MaDVSX"];
                        drNew["ChieuDai"] = dr["ChieuDai"];
                        drNew["ChieuRong"] = dr["ChieuRong"];
                        drNew["ChieuCao"] = dr["ChieuCao"];
                        drNew["TrongLuong"] = Math.Round(Convert.ToDouble(dr["TrongLuong"]), 1);
                        drNew["KhoiLuong"] = Math.Round(Convert.ToDouble(dr["KhoiLuong"]), 1);
                        drNew["Chon"] = false;
                        drNew["SttThung"] = ydex++;
                        drNew["IsThungLe"] = dr["IsThungLe"];
                        drNew["IsSave"] = false;
                        drNew["KyHieu"] = dr["KyHieu"];
                        drNew["KieuLap"] = dr["KieuLap"];
                        foreach (DataColumn dc in _dtData.Columns)
                        {
                            if (!dc.ColumnName.Contains("@")) continue;
                            var _sizeID = dc.ColumnName.Split('@')[1];
                            //drNew["IsThungLe"] = dr["IsThungLe"];
                            drNew[dc.ColumnName] = dr[dc.ColumnName];
                            //slSP += dr[dc.ColumnName].ToString() != "" ? Convert.ToUInt32(dr[dc.ColumnName]) : 0;
                        }
                        drNew["SoLuong"] = dr["SoLuong"];
                        drNew["PCB_Pack"] = dr["PCB_Pack"];
                        drNew["Pack_Ctn"] = dr["Pack_Ctn"];
                        dataNew.Rows.Add(drNew);
                    }
                }
                // if (_maPKL == "")
                //     dgrKHDongThung.MainView = GetBandGridViewAmount_grd1(_dtData);
                CreateBandForSize(dataNew);
                KHDongThungLib.ProcessSttTrung(dataNew);
                dgrKHDongThung.DataSource = dataNew;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private async void SaveRows()
        {
            try
            {
                DataTable tblSave = KHDongThungLib.CreateTblSave();
                DateTime dtNow = DateTime.Now;
                int TuThung_Notdeca = 0;
                int TuThungOld = 0;
                int SttThungOld = 0;
                int SttThung_temp = 0;
                this.ActiveControl = grcDetail;
                _dtData = dgrKHDongThung.DataSource as DataTable;
                string _colorIDOld = "";
                //var 
                foreach (DataRow dr in _dtData.Rows)
                {
                    var _poid = dr["POID"].ToString();
                    var _po = dr["PO"].ToString();
                    var _sizeType = dr["DauSize"].ToString();
                    var _sizeTypeID = dr["DauSizeID"].ToString();
                    var _colorID = dr["ColorID"].ToString();
                    if (_colorIDOld != _colorID)
                    {
                        SttThung_temp = 0;
                        _colorIDOld = _colorID;
                    }
                    string _sizeID = string.Empty;
                    string _size = string.Empty;
                    int slThung = Convert.ToInt32(dr["SLThung"].ToString());
                    int TuThung = Convert.ToInt32(dr["TuThung"].ToString());
                    int DenThung = Convert.ToInt32(dr["DenThung"].ToString());

                    for (int z = 0; z < _dtData.Columns.Count; z++)
                    {
                        string[] arrName = _dtData.Columns[z].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrName.Length < 2) continue;
                        var sizeName = _dtData.Columns[z].ColumnName;
                        if (dr[sizeName].ToString() == "" || dr[sizeName].ToString() == "0") continue;
                        _sizeID = arrName[1];
                        _size = arrName[0];
                        int index = 0;
                        int SttThungDeCat_Start = 0;

                        for (int i = 0; i < slThung; i++)
                        {
                            DataRow drNewRow = tblSave.NewRow();
                            drNewRow["ID"] = 0;
                            drNewRow["MaPKL"] = _maPKL;
                            drNewRow["MaDH"] = _madh;
                            drNewRow["MaDVSX"] = dr["MaDVSX"];
                            drNewRow["MaLenh"] = dr["MaLenh"];
                            drNewRow["DotSX"] = dr["DotSX"];
                            drNewRow["MaHang"] = _maHang;
                            drNewRow["POID"] = _poid;
                            drNewRow["PO"] = _po;
                            drNewRow["ColorID"] = dr["ColorID"].ToString();
                            drNewRow["TenMau"] = dr["TenMau"].ToString();
                            drNewRow["DauSize"] = dr["DauSize"].ToString();
                            drNewRow["DauSizeID"] = dr["DauSizeID"].ToString();
                            drNewRow["SizeID"] = _sizeID;
                            drNewRow["Size"] = _size;
                            drNewRow["NgayLapKH"] = dtNow;
                            drNewRow["ChieuDai"] = dr["ChieuDai"];
                            drNewRow["ChieuRong"] = dr["ChieuRong"];
                            drNewRow["ChieuCao"] = dr["ChieuCao"];
                            drNewRow["KyHieu"] = dr["KyHieu"];
                            drNewRow["TrongLuong"] = Math.Round(Convert.ToDouble(dr["TrongLuong"]), 1);
                            drNewRow["KhoiLuong"] = Math.Round(Convert.ToDouble(dr["KhoiLuong"]), 1);
                            drNewRow["SoLuongThung"] = slThung;
                            drNewRow["TuThung"] = TuThung;
                            drNewRow["DenThung"] = DenThung;
                            if (chkb_size.Checked)
                            {
                                drNewRow["SttThung_decat"] = TuThung + index;
                                if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                                {
                                    TuThung_Notdeca++;
                                    index++;
                                    SttThung_temp++;
                                }
                                drNewRow["SttThung"] = TuThung_Notdeca;
                            }
                            else
                            {
                                drNewRow["SttThung"] = TuThung + index;
                                drNewRow["SttThung_decat"] = 0;
                                if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                                {
                                    TuThung_Notdeca++;
                                    index++;
                                    SttThung_temp++;
                                }
                            }
                            drNewRow["SttThung_LapMau"] = SttThung_temp == 0 ? 1 : SttThung_temp;
                            drNewRow["SttThung_temp"] = index == 0 ? 1 : index;
                            drNewRow["SttThung"] = TuThung_Notdeca;
                            if (SttThungDeCat_Start == 0) SttThungDeCat_Start = Convert.ToInt32(drNewRow["SttThung"]);
                            drNewRow["SttThung_start"] = SttThungDeCat_Start;

                            drNewRow["SoLuongSP"] = Convert.ToInt32(dr[sizeName].ToString());
                            drNewRow["PCB_Pack"] = dr["PCB_Pack"];
                            drNewRow["Pack_Ctn"] = dr["Pack_Ctn"];
                            drNewRow["IsDongThung"] = false;
                            drNewRow["NgayDongThung"] = dtNow;
                            drNewRow["NgayNhapKho"] = dtNow;                           
                            drNewRow["IsNhapKho"] = false;
                            //drNewRow["KyHieu"] = string.Format("{0} -> {1}", TuThung, DenThung);
                            drNewRow["Chon"] = false;
                            drNewRow["IsThungLe"] = dr["IsThungLe"]; //-dr["IsThungLe"];
                            drNewRow["NVien"] = GlobleData.UserName;
                            drNewRow["KieuLap"] = CheckTheoMau ? "1" : "0";
                            drNewRow["QRCode"] = txtStylePackNo.Text;
                            drNewRow["StyleName"] = txtStyleName.Text;
                            drNewRow["DeptNo"] = txtDeptNo.Text;
                            drNewRow["Destination"] = txtCangDen.Text;
                            drNewRow["DeliveryTo"] = txtDeliveryTo.Text;
                            drNewRow["Terms"] = txtTerms.Text;
                            drNewRow["CountryOfOrigin"] = txtCountry.Text;
                            drNewRow["KieuLapPCB"] = "1";
                            tblSave.Rows.Add(drNewRow);
                        }
                        SttThungOld = Convert.ToInt32(dr["SttThung"]);
                    }
                }
                if (tblSave == null || tblSave.Rows.Count == 0)
                {
                    MessageBox.Show("Số lượng kế hoạch không được để trống, phải có ít nhất số lượng của 1 size");
                    return;
                }
                string url = string.Format("{0}", URL + "KeHoachDongThung/Post?action=InsertKHDT");
                //return;
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;

                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    POID = searchLookUpEditPO.EditValue == null ? _poid : searchLookUpEditPO.EditValue.ToString();
                    this.Close();
                }
                else
                    MessageBox.Show(result);
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }
        private void GopThung()
        {
            this.ActiveControl = grcDetail;
            var data = dgrKHDongThung.DataSource as DataTable;
            var dataT = KHDongThungLib.GopThung(data, chkb_size.Checked ? 1 : 0);
            if (dataT.Rows.Count == 0) return;
            _dtData = dataT;
            dgrKHDongThung.DataSource = dataT;
            bandedGridViewKHDT.RefreshData();
        }
        private void GetQuiCach()
        {
            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetQuiCachMH&Para1={_maHang}&Para2={_maNoiDen}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            dtQuiCach = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void GetSLDM()
        {
            try
            {
                string[] lstDVSX = _dausize.Split(';');
                string _maDVSXF = "";
                for (int i = 0; i < lstDVSX.Length; i++)
                {
                    _maDVSXF += "," + lstDVSX[i].Trim() + "";
                }
                _maDVSXF = _dausize;
                if (searchLookUpEditPO.EditValue is null) return;
                string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=Get_SLDM&MaDH={_madh}&MaDVSX={_maDVSXF}&DotSX=Para" +
                                                    $"&POID={searchLookUpEditPO.EditValue}&SizeTypeID=${_dausize}&ColorID={_maNoiDen}&ProductID=Para&SizeID=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtSize = JsonConvert.DeserializeObject<DataTable>(json);
                grcDetail.DataSource = dtSize;
                gridviewDetail.RefreshData();

            }
            catch (Exception ex)
            {

            }

        }
        private void GetKHDongThung()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            dtDataPiVot = tbl.Copy();
            if (tbl == null || tbl.Rows.Count == 0)
                dgrKHDongThung.DataSource = new DataTable();
            else
            {

                DataTable dt = new DataTable();
                //if (!checkEditAll.Checked)
                //{
                //    var tempdt = tbl.AsEnumerable().Where(x => x["MaDVSX"].ToString() + '@' + x["MaLenh"].ToString() == _maDVSXTemp);
                //    tbl = tempdt.Count() == 0 ? new DataTable() : tempdt.CopyToDataTable();
                //}
                var drCheck = tbl.Rows[0];
                if (drCheck["Stt_Size"].ToString() == "1") chkb_size.Checked = true;
                else if (drCheck["KieuLap"].ToString() == "1") chbxTheoMau.Checked = true;
                txtCangDen.Text = drCheck["Destination"].ToString();
                txtDeliveryTo.Text = drCheck["DeliveryTo"].ToString();
                txtStyleName.Text = drCheck["StyleName"].ToString();
                txtStylePackNo.Text = drCheck["StylePackNo"].ToString();
                txtDeptNo.Text = drCheck["DeptNo"].ToString();
                //txtDeptNo.Text = drCheck["DeptNo"].ToString();
                CreateBandForSize(tbl);
                KHDongThungLib.ProcessSttTrung(tbl);
                dgrKHDongThung.DataSource = tbl;
                dgrKHDongThung.RefreshDataSource();


            }
        }

        #region Thoai
        public void LoadNoiDen()
        {
            dtNoiDen = KHDongThungLib.LoadNoiDen(URL, _maHang, _clientExtension);
            searchLookUpEditNoiDen.Properties.DataSource = dtNoiDen;
            searchLookUpEditNoiDen.Properties.DisplayMember = "NoiDen";
            searchLookUpEditNoiDen.Properties.ValueMember = "MaNoiDen";
            searchLookUpEditNoiDen.Properties.NullText = "Chọn nơi đến";
            searchLookUpEditNoiDen.Properties.PopupView.Columns.Clear();
            searchLookUpEditNoiDen.Properties.PopupView.Columns.AddVisible("NoiDen", "Nơi đến");
            if (dtNoiDen.Rows.Count == 1)
            {
                string defaultMaNoiDen = dtNoiDen.Rows[0]["MaNoiDen"].ToString();
                searchLookUpEditNoiDen.EditValue = defaultMaNoiDen;
                _maNoiDen = defaultMaNoiDen;
            }
            else if (dtNoiDen.Rows.Count > 1)
            {
                using (var frm = new frmChonNoiDen_KHDTV4(dtNoiDen))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        _maNoiDen = frm.SelectedMaNoiDen;
                        searchLookUpEditNoiDen.EditValue = _maNoiDen;
                    }
                    else
                    {
                        XtraMessageBox.Show("Bạn chưa chọn nơi đến!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void searchLookUpEditNoiDen_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditNoiDen.EditValue == null) return;
            if (_maNoiDen == searchLookUpEditNoiDen.EditValue.ToString()) return;

            _maNoiDen = searchLookUpEditNoiDen.EditValue.ToString();
            dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension, _maNoiDen);
            GetSLDM();
            btn_XC_Config_Click(null, null);
            ClearDataBandAndCol();
            _dtData.Clear();
            dtDataPiVot.Clear();
            ProcessPackage();
            gridviewDetail.RefreshData();
            grvSize.RefreshData();
            bandedGridViewKHDT.RefreshData();
        }
        #endregion

        private void CreateBandSize(DataTable dt)
        {
            foreach (DataRow drsize in dt.Rows)
            {
                var _sizeID = drsize["SizeID"].ToString();
                var _size = drsize["Size"].ToString();
                if (!CheckExistBand(_sizeID)) continue;
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = _size + "@" + _sizeID;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = this.repotxtN0;
                col.Visible = true;
                col.Width = 50;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private void CreateBandForSize(DataTable dt)
        {
            try
            {
                //ClearDataBandAndCol();
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    if (!colName.Contains('@')) continue;
                    var _sizeID = colName.Split('@')[1];
                    var _size = colName.Split('@')[0]; ;
                    if (!CheckExistBand(_sizeID)) continue;
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = _size;
                    col.FieldName = _size + "@" + _sizeID;
                    col.Name = "col" + _sizeID;
                    col.OptionsColumn.AllowEdit = true;
                    col.ColumnEdit = this.repotxtN0;
                    col.Visible = true;
                    col.Width = 40;
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                    gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                    gb.AppearanceHeader.Options.UseBackColor = true;
                    gb.AppearanceHeader.Options.UseFont = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.Caption = _size;
                    gb.Columns.Add(col);
                    gb.Name = "gb" + "Size@" + _sizeID;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void ClearDataBandAndCol()
        {
            try
            {
                //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                gbSize.Children.Clear();
                dgrKHDongThung.DataSource = new DataTable();
                List<GridColumn> lst = new List<GridColumn>();
                lst = bandedGridViewKHDT.Columns.Where(x => x.FieldName.Contains(@"Size_")).ToList();
                if (lst != null && lst.Count > 0)
                    foreach (GridColumn gc in lst) bandedGridViewKHDT.Columns.Remove(gc);
            }
            catch (Exception ex) { };
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private int GetMaxThung()
        {
            if (_maPKL == "") return 0;
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetMaxThung&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dtMaxThung = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtMaxThung.Rows.Count == 0) return 0;
            else return Convert.ToInt32(dtMaxThung.Rows[0]["SttThung"]);
        }
        #endregion
        private void LoadPO()
        {
            string[] lstDVSX = _maDVSX.Split(';');
            if (lstDVSX.Length == 0) return;
            var _lstTempPO = _dtInit.AsEnumerable().Where(x => lstDVSX.Contains(x["value"].ToString())
                                                            && x["NgayGH"].ToString() == (cbxNgayGH.EditValue is null ? x["NgayGH"].ToString() : cbxNgayGH.EditValue.ToString()))
                .GroupBy(gr => new
                {
                    POID = gr["POID"].ToString(),
                    PO = gr["PO"].ToString(),
                }).Select(y => new
                {
                    POID = y.Key.POID,
                    PO = y.Key.PO,
                    SLKH = y.Sum(row => Convert.ToInt32(row["SoLuong"])),
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
        private DataTable CreateTblPackage()
        {
            var dt = new DataTable("dtData");
            //_dtData.Columns.Add("ColGroup", typeof(string));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("TuThung", typeof(int));
            dt.Columns.Add("DenThung", typeof(int));
            dt.Columns.Add("MaDVSX", typeof(string));
            dt.Columns.Add("TenDVSX", typeof(string));
            dt.Columns.Add("MaLenh", typeof(string));
            dt.Columns.Add("DotSX", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("StyleID", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("PCB_Pack", typeof(int));
            dt.Columns.Add("Pack_Ctn", typeof(int));
            dt.Columns.Add("SLThung", typeof(int));
            dt.Columns.Add("TotalPiece", typeof(int));
            dt.Columns.Add("TrongLuong", typeof(float));
            dt.Columns.Add("KhoiLuong", typeof(float));
            dt.Columns.Add("KyHieu", typeof(string));
            dt.Columns.Add("ChieuDai", typeof(string));
            dt.Columns.Add("ChieuRong", typeof(string));
            dt.Columns.Add("ChieuCao", typeof(string));
            dt.Columns.Add("IsThungLe", typeof(bool));
            dt.Columns.Add("SttThung", typeof(int));
            dt.Columns.Add("Chon", typeof(bool));
            dt.Columns.Add("IsSave", typeof(bool));
            dt.Columns.Add("KieuLap", typeof(string));
            dt.Columns.Add("Stt_Size", typeof(string));
            return dt;
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
        #region Event KH Đóng thùng
        private void bandedGridViewKHDT_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            e.Handled = KHDongThungLib.MergeAllowChangeValue(bandedGridViewKHDT, e);
        }
        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }
        private void bandedGridViewKHDT_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = (bool)bandedGridViewKHDT.GetRowCellValue(e.RowHandle, "IsThungLe");
            if (IsThungLe) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }
        private void bandedGridViewKHDT_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            // if ((bool)drFocus["IsSave"]) return;
            DXMenuItem menuDelete = new DXMenuItem();
            menuDelete.Caption = "Xóa";
            menuDelete.Click += MenuDelete_Click;
            DXMenuItem menuCopyandCreate = new DXMenuItem();
            menuCopyandCreate.Caption = "Copy và tạo dòng mới";
            menuCopyandCreate.Click += MenuCopyandCreate_Click;
            e.Menu.Items.Add(menuCopyandCreate);
            e.Menu.Items.Add(menuDelete);
        }
        private void bandedGridViewKHDT_ShowingEditor(object sender, CancelEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            var lstColumn = dt.Columns;
            KHDongThungLib.EventShowingEditor(bandedGridViewKHDT, e, false, lstColumn);
            //var _maDVSX = bandedGridViewKHDT.GetFocusedRowCellValue("MaDVSX").ToString();
            //if (!KHDongThungLib.CheckRole(_maDVSX))
            //{
            //    e.Cancel = true;
            //    return;
            //}
            //var value = bandedGridViewKHDT.GetFocusedRowCellValue("IsSave");
            //if ((bool)value) e.Cancel = true;
            //if (bandedGridViewKHDT.FocusedColumn.FieldName == "Chon")
            //{
            //    if ((bool)bandedGridViewKHDT.GetFocusedRowCellValue("IsThungLe") == true)
            //    {
            //        e.Cancel = false;
            //    }
            //    else
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }
        private void MenuCopyandCreate_Click(object sender, EventArgs e)
        {
            var drCopy = bandedGridViewKHDT.GetFocusedDataRow();
            DataTable tbl = dgrKHDongThung.DataSource as DataTable;
            if (tbl.Columns.Count == 0) return;
            DataRow drAdd = tbl.NewRow();
            drAdd["POID"] = drCopy["POID"];
            drAdd["PO"] = drCopy["PO"];
            drAdd["MaHang"] = _maHang;
            drAdd["MaDVSX"] = drCopy["MaDVSX"];
            drAdd["TenDVSX"] = drCopy["TenDVSX"];
            drAdd["MaLenh"] = drCopy["MaLenh"];
            drAdd["DotSX"] = drCopy["DotSX"];
            drAdd["DauSizeID"] = drCopy["DauSizeID"];
            drAdd["DauSize"] = drCopy["DauSize"];
            drAdd["ColorID"] = drCopy["ColorID"];
            drAdd["TenMau"] = drCopy["TenMau"];
            drAdd["SoLuong"] = 0;
            drAdd["SLThung"] = 0;
            drAdd["TotalPiece"] = 0;
            drAdd["TrongLuong"] = 0;
            drAdd["KhoiLuong"] = 0;
            drAdd["IsSave"] = false;
            drAdd["Chon"] = false;
            drAdd["IsThungLe"] = false;
            var sttThung = Convert.ToInt32(tbl.Rows[tbl.Rows.Count - 1]["SttThung"]);
            drAdd["SttThung"] = sttThung + 1;
            drAdd["KieuLap"] = drCopy["KieuLap"];
            drAdd["Stt_size"] = chkb_size.Checked ? 1 : 0;
            tbl.Rows.Add(drAdd);
            dgrKHDongThung.RefreshDataSource();
        }
        private void bandedGridViewKHDT_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();

            if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            {

                int TuThung = drFocus["TuThung"].ToString() == "" ? 0 : Convert.ToInt32(drFocus["TuThung"]);
                int DenThung = drFocus["DenThung"].ToString() == "" ? 0 : Convert.ToInt32(drFocus["DenThung"]);
                if (TuThung == DenThung) drFocus["IsThungLe"] = true;
                else drFocus["IsThungLe"] = false;
                if (TuThung != 0 && DenThung != 0 && DenThung >= TuThung)
                {
                    drFocus["SLThung"] = DenThung + 1 - TuThung;
                    //if (CheckSTTThungInData(TuThung, DenThung))
                    CheckSTTThungInGrid(TuThung, DenThung);
                }
                else
                {
                    drFocus["SLThung"] = 0;
                    btSave.Enabled = false;
                }

                int sumSL = 0;
                foreach (DataColumn dc in drFocus.Table.Columns)
                {
                    if (!dc.ColumnName.Contains('@')) continue;
                    sumSL += drFocus[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drFocus[dc.ColumnName]);
                }
                drFocus["SoLuong"] = sumSL;
                if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
                    drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
            }
            else if (e.Column.FieldName.Contains("@") || e.Column.FieldName.Contains("SLThung"))
            {
                int sumSL = 0;
                foreach (DataColumn dc in drFocus.Table.Columns)
                {
                    if (!dc.ColumnName.Contains('@')) continue;
                    sumSL += drFocus[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drFocus[dc.ColumnName]);
                }
                drFocus["SoLuong"] = sumSL;
                if (drFocus["PCB_Pack"].ToString() != "0")
                {
                    drFocus["PCB_Pack"] = sumSL;
                    drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["Pack_Ctn"]) * Convert.ToInt32(drFocus["SLThung"]);
                }
                else if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
                    drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
                btSave.Enabled = true;
            }
            else if (e.Column.FieldName == "ChieuDai" || e.Column.FieldName == "ChieuRong" || e.Column.FieldName == "ChieuCao")
            {
                double ChieuDai = drFocus["ChieuDai"].ToString() == "" ? 0 : Convert.ToDouble(drFocus["ChieuDai"]);
                double ChieuRong = drFocus["ChieuRong"].ToString() == "" ? 0 : Convert.ToDouble(drFocus["ChieuRong"]);
                double ChieuCao = drFocus["ChieuCao"].ToString() == "" ? 0 : Convert.ToDouble(drFocus["ChieuCao"]);
                drFocus["KyHieu"] = string.Format("{0}x{1}x{2}", ChieuDai, ChieuRong, ChieuCao);
                btSave.Enabled = true;
            }
            else if (e.Column.FieldName == "TrongLuong" || e.Column.FieldName == "KhoiLuong")
            {
                var dt = dgrKHDongThung.DataSource as DataTable;
                KHDongThungLib.ProcessChangeNW_GW(dt, drFocus);
            }
        }
        private void MenuDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow drRemove = bandedGridViewKHDT.GetFocusedDataRow();
                DataTable tblPivot = dgrKHDongThung.DataSource as DataTable;
                if (drRemove == null) return;
                if ((bool)drRemove["IsSave"] == false)
                {
                    if (_maPKL != "")
                    {
                        var sttThungRemove = drRemove["SttThung"].ToString();
                        var drRemoveTemp = _dtData.AsEnumerable().Where(x => x["SttThung"].ToString() == sttThungRemove).FirstOrDefault();
                        _dtData.Rows.Remove(drRemoveTemp);
                    }
                    int index = tblPivot.Rows.IndexOf(drRemove);
                    tblPivot.Rows.RemoveAt(index);
                    dgrKHDongThung.RefreshDataSource();
                    KHDongThungLib.TinhToanLaiKhiXoa(tblPivot, chkb_size.Checked ? 1 : 0);
                }
                else
                {
                    if (drRemove["IsDongThung1"].ToString() == "1")
                    {
                        MessageBox.Show("Đã tồn tại thùng đã được đóng. Không thể xóa dữ liệu!", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                    DataRow drCheckSave = tblPivot.AsEnumerable().Where(x => (bool)x["IsSave"] == false).FirstOrDefault();
                    DialogResult resultDialog = DialogResult.None;
                    if (drCheckSave != null)
                        resultDialog = MessageBox.Show("Phát hiện dữ liệu thay đổi. Bạn có muốn lưu dữ liệu rồi tiếp tục thao tác không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    else resultDialog = MessageBox.Show("Xác nhận xóa dữ liệu?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultDialog == DialogResult.Yes)
                    {
                        int index = tblPivot.Rows.IndexOf(drRemove);
                        tblPivot.Rows.RemoveAt(index);
                        KHDongThungLib.TinhToanLaiKhiXoa(tblPivot, chkb_size.Checked ? 1 : 0);
                    }

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
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
                e.DisplayText = "[Chọn ĐVSX]";
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

        string selectedValuessDauSize = "";
        string selectedValuesDS = "";
        void searchLookUpEditSizeType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //StringBuilder sb = new StringBuilder();
            //SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            //if (gridCheckMark == null) return;
            //foreach (DataRowView rv in gridCheckMark.Selection)
            //{
            //    if (sb.ToString().Length > 0) { sb.Append("; "); }
            //    sb.Append(rv["SizeType"].ToString());
            //}
            //if (string.IsNullOrEmpty(sb.ToString()))
            //{
            //    e.DisplayText = "[Chọn đầu size]";
            //}
            //else
            //{
            //    e.DisplayText = sb.ToString();
            //}
            selectedValuessDauSize = string.Join("; ", grvDauSize.GetSelectedRows().Select(rowHandle => grvDauSize.GetRowCellValue(rowHandle, searchLookUpEditSizeType.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessDauSize.ToString()))
            {
                e.DisplayText = "----Chọn đầu size----";
            }
            else
            {
                e.DisplayText = selectedValuessDauSize.ToString();
            }
        }
        private void grvDauSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedValuesDS = string.Join(";", grvDauSize.GetSelectedRows().Select(rowHandle => grvDauSize.GetRowCellValue(rowHandle, searchLookUpEditSizeType.Properties.ValueMember)));
            // Thiết lập giá trị EditValue cho control
            searchLookUpEditSizeType.EditValue = selectedValuesDS;
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

        }
        private void btn_XC_Config_Click(object sender, EventArgs e)
        {
            var _dt = grcDetail.DataSource as DataTable;
            if (_dt is null) return;
            var groupedSum = _dt.AsEnumerable().Where(x => Convert.ToInt32(x["SLDT"].ToString() == "" ? 0 : x["SLDT"]) != 0)
                .GroupBy(row => new
                {
                    SizeTypeID = row.Field<string>("SizeTypeID"),
                    SizeType = row.Field<string>("SizeType"),
                    SizeID = row.Field<string>("SizeID"),
                    Size = row.Field<string>("Size"),
                    SLPCB = Convert.ToInt32(row["SLPCB"]),
                    PCB_Pack = Convert.ToInt32(row["PCB_Pack"]),
                    Pack_Ctn = Convert.ToInt32(row["Pack_Ctn"]),
                    SLThung = Convert.ToInt32(row["SLThung"]),
                    //NW = Convert.ToDouble(row["NW"]),
                    //  GW = Convert.ToDouble(row["GW"]),
                    //ChieuDai = Convert.ToInt32(row["ChieuDai"]),
                    //ChieuRong = Convert.ToInt32(row["ChieuRong"]),
                    //ChieuCao = Convert.ToInt32(row["ChieuCao"]),
                    //KyHieu = row["KyHieu"],

                })
                .Select(group => new
                {
                    SizeTypeID = group.Key.SizeTypeID,
                    SizeType = group.Key.SizeType,
                    SizeID = group.Key.SizeID,
                    Size = group.Key.Size,
                    SLPCB = group.Key.SLPCB,
                    PCB_Pack = group.Key.PCB_Pack,
                    Pack_Ctn = group.Key.Pack_Ctn,
                    SLThung = group.Key.SLPCB == 0 ? 0 : Math.Ceiling(group.Sum(row => Convert.ToDouble(row["SLDT"])) / group.Key.SLPCB),
                    SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                    SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),
                    //NW = group.Key.NW,
                    // GW = group.Key.GW,
                    //ChieuDai = group.Key.ChieuDai,
                    //ChieuRong = group.Key.ChieuRong,
                    //ChieuCao = group.Key.ChieuCao,
                    //KyHieu = group.Key.KyHieu
                }).ToList();
            string jsonT = JsonConvert.SerializeObject(groupedSum);
            DataTable dtSizetemp = JsonConvert.DeserializeObject<DataTable>(jsonT);
            gridControlSize.DataSource = dtSizetemp;
        }
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ProcessPackage();
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnSave();
        }
        private void chkv_thungle_cuoi_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProcessPackage();
            DataTable _dtTemp11 = (DataTable)gridControlSize.DataSource;

            if (_dtTemp11 is null) DuaThungLeVeCuoi();
        }
        DataTable dtOLd = new DataTable();
        bool flagFirst = true;
        public void DuaThungLeVeCuoi()
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            if (flagFirst)
            {
                dtOLd = dt;
                flagFirst = false;
            }
            if(!chkv_thungle_cuoi.Checked)
            {
                dgrKHDongThung.DataSource = dtOLd;
                return;
            }
            
            var flagCheckDT = dt.AsEnumerable().Any(x => x["IsDongThung1"].ToString() == "1");
            if (flagCheckDT)
            {
                MessageBox.Show("PKL đã được đóng thùng.Không thể chỉnh sữa !");
                return;
            }
            var dtThungLeA = dt.AsEnumerable().Where(x => (bool)x["IsThungLe"]);
            var dtThungNguyenA = dt.AsEnumerable().Where(x => !(bool)x["IsThungLe"]);
            var dtThungLe = dtThungLeA.Count() > 0 ? dtThungLeA.CopyToDataTable() : dt.Clone();
            var dtThungNguyen = dtThungNguyenA.Count() > 0 ? dtThungNguyenA.CopyToDataTable() : dt.Clone();
            //List<CheckThungLe> lstCheck = new List<CheckThungLe>();
            //foreach (DataColumn dc in dt.Columns)
            //{
            //    if (dc.ColumnName.Contains('@'))
            //    {
            //        lstCheck.Add(new CheckThungLe()
            //        {
            //            SizeID = dc.ColumnName,
            //            IsActive = false
            //        });
            //    }
            //}
            dtThungNguyen.Merge(dtThungLe);
            KHDongThungLib.TinhToanLaiKhiXoa(dtThungNguyen);
            dgrKHDongThung.DataSource = dtThungNguyen;
        }

        private void chkb_size_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (chkb_size.Checked) chbxTheoMau.Checked = false;
            ProcessPackage();
        }

        private void repoQuiCach_EditValueChanged(object sender, EventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            var drfocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drfocus is null) return;
            SearchLookUpEdit sr = sender as SearchLookUpEdit;
            var quicach = sr.EditValue.ToString();
            var dtTemp = dt.AsEnumerable().Where(x => x["SttThung"].ToString() == drfocus["SttThung"].ToString());
            if (dtTemp.Count() > 0)
            {
                var dtTrung = dtTemp.CopyToDataTable();
                foreach (DataRow dr1 in dtTrung.Rows)
                {
                    dr1["KyHieu"] = quicach;
                }
            }

            //var dtTemp = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).CopyToDataTable();
            var TrongLuong = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).FirstOrDefault()["TLThung"].ToString();
            var TotalPice = Convert.ToInt32(drfocus["TotalPiece"]);
            var SLThung = Convert.ToInt32(drfocus["SLThung"]);
            var KhoiLuong = Convert.ToDouble(drfocus["KhoiLuong"]);
            var value = Math.Round(KhoiLuong + SLThung * Convert.ToDouble(TrongLuong), 1);
            drfocus["TrongLuong"] = value;
            drfocus["KyHieu"] = quicach;
        }

        private void RepocbxTheoMau_CheckedChanged(object sender, EventArgs e)
        {
            if (chbxTheoMau.Checked) chkb_size.Checked = false;
            CheckTheoMau = chbxTheoMau.Checked;
            ProcessPackage();
        }
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SLPCB")
            {
                DataRow dr = grvSize.GetFocusedDataRow();
                dr["SLThung"] = Convert.ToDouble(dr["SLPCB"].ToString() == "" ? 0 : dr["SLPCB"]) == 0 ? 0 : Math.Ceiling(Convert.ToDouble(dr["SLDT"]) / Convert.ToDouble(dr["SLPCB"]));
            }
        }
        private void btnGopThung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GopThung();
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
            try
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
            catch (Exception ex)
            {
                e.Valid = false;
            }

        }
        private bool CheckSTTThungInGrid(int tuThung, int denThung)
        {
            string mess = "", colorID = "";
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            colorID = drFocus["ColorID"].ToString();
            DataTable tblData = dgrKHDongThung.DataSource as DataTable;
            List<DataRow> lstCheck = tblData.AsEnumerable().Where(x => x["ColorID"].ToString() == colorID && ((BetweenInt(tuThung, Convert.ToInt32(x["TuThung"]), Convert.ToInt32(x["DenThung"])) == true) ||
            (BetweenInt(denThung, Convert.ToInt32(x["TuThung"]), Convert.ToInt32(x["DenThung"])) == true))).ToList();
            if (lstCheck != null && lstCheck.Count > 1)
            {
                foreach (DataRow dr in tblData.Rows) mess += string.Format("{0} -> {1}\r\n", dr["TuThung"], dr["DenThung"]);
            }
            if (mess != "")
            {
                MessageBox.Show(string.Format("Mã hàng này đang lập kế hoạch ở các thùng \r\n {0} vui lòng chọn số thùng khác.", mess));
                btSave.Enabled = false;
                return false;
            }
            else
            {
                btSave.Enabled = true;
                return true;
            }

        }
        private bool BetweenInt(int a, int minValue, int maxValue)
        {
            if (a >= minValue && a <= maxValue) return true;
            else return false;
        }
        #endregion
        #region Manh

        private void BtnSave()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            SaveRows();
        }
        private void bandedGridViewKHDT_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
        }
        private void bandedGridViewKHDT_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }
        #endregion
    }
    public class CheckThungLe
    {
        public string SizeID { get; set; }
        public bool IsActive { get; set; }
    }
}
