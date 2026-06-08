using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
    public partial class frmERPKeHoachDongThungV2 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        SearchCheckSelection gridCheckMarksDVSX;
        SearchCheckSelection gridCheckMarksPO;
        SearchCheckSelection gridCheckMarksSizeType;
        private string _madh = string.Empty, _maHang = string.Empty, _maDVSX = string.Empty, _po = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty;
        private string _maNoiDen = "0";
        DataTable dtNoiDen = new DataTable();
        DataTable dtSize = new DataTable();
        DataTable dtSizeAll = new DataTable();
        DataTable dtSizePCB = new DataTable();
        DataTable _dtData = new DataTable();
        DataTable _dtInit = new DataTable();
        DataTable dtQuiCach = new DataTable();

        int _carton = 0, thung_po = 0;
        bool CheckTheoMau = false;
        KeyDownControlHandler keyDownControlHandler;
        private bool flagChangePCB = false;
        public frmERPKeHoachDongThungV2(string MaDH, string MaHang)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _madh = MaDH;
            _maHang = MaHang;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            KHDongThungLib.InitQuiCach(repoQuiCach);
            dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension, _maNoiDen);
            barOption.EditValue = 4;
            barOption.EditValueChanged += BarOption_EditValueChanged;
            //GetQuiCach();
        }

        private void BarOption_EditValueChanged(object sender, EventArgs e)
        {
            ProcessPackage();
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GetSLDM();
            //GetSizePCB();
            LoadNoiDen();
            ProcessPackage();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtnSave, true, ActionType.Save);
            AddActionControl(_lstActionControl, GetSLDM, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, ProcessPackage, true, ActionType.LapKH);
            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        #region Function
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
                string url = string.Format("{0}?", URL + $"KeHoachDongThung/Get?Action=Get_SLDM_PCB&MaDH={_madh}&MaDVSX=Para&DotSX=Para" +
                                                    $"&POID=Para&SizeTypeID=Para&ColorID={_maNoiDen}&ProductID=Para&SizeID=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                dtSize = JsonConvert.DeserializeObject<DataTable>(json);
                var lstTemp = dtSize.AsEnumerable().GroupBy(x => new
                {
                    MaDH = x["MaDH"],
                    MaDVSX = x["MaDVSX"],
                    TenDVSX = x["TenDVSX"],
                    MaLenh = x["MaLenh"],
                    DotSX = x["DotSX"],
                    POID = x["POID"],
                    PO = x["PO"],
                    DauSizeID = x["SizeTypeID"],
                    DauSize = x["SizeType"],
                    ColorID = x["ColorID"],
                    TenMau = x["TenMau"],
                    //PCB = x["PCB"],
                }).Select(group => new
                {
                    MaDH = group.Key.MaDH,
                    MaDVSX = group.Key.MaDVSX,
                    TenDVSX = group.Key.TenDVSX,
                    MaLenh = group.Key.MaLenh,
                    DotSX = group.Key.DotSX,
                    POID = group.Key.POID,
                    PO = group.Key.PO,
                    DauSizeID = group.Key.DauSizeID,
                    DauSize = group.Key.MaDH,
                    ColorID = group.Key.ColorID,
                    TenMau = group.Key.TenMau,
                    //PCB = group.Key.PCB,
                    SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                    //SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),   
                    //SLThung = group.Sum(row => Convert.ToInt32(row["SLDT"])),
                }).ToList();

                string json1 = JsonConvert.SerializeObject(lstTemp);
                DataTable dtTemp = JsonConvert.DeserializeObject<DataTable>(json1);

                grcDetail.DataSource = dtSize;
                gridviewDetail.OptionsBehavior.AutoExpandAllGroups = true;
                gridviewDetail.RefreshData();
                LoadPCBSize();
            }
            catch (Exception ex)
            {

            }
        }
        private void LoadPCBSize()
        {
            var dt = grcDetail.DataSource as DataTable;
            var lstTempPCB = dt.AsEnumerable().GroupBy(x => new
            {
                SizeTypeID = x.Field<string>("SizeTypeID"),
                SizeType = x.Field<string>("SizeType"),
                Size = x["Size"],
                SizeID = x["SizeID"],
                SLPCB = Convert.ToInt32(x["SLPCB"]),
                //SapXep = Convert.ToInt32(x["SapXep"])
                NW = Convert.ToDouble(x["NW"]),
                GW = Convert.ToDouble(x["GW"]),
                //ChieuDai = Convert.ToInt32(x["ChieuDai"]),
                //ChieuRong = Convert.ToInt32(x["ChieuRong"]),
                //ChieuCao = Convert.ToInt32(x["ChieuCao"]),
                //KyHieu = x["KyHieu"],
            }).Select(group => new
            {
                SizeTypeID = group.Key.SizeTypeID,
                SizeType = group.Key.SizeType,
                Size = group.Key.Size,
                SizeID = group.Key.SizeID,
                SLPCB = group.Key.SLPCB,
                //SapXep = group.Key.SapXep,
                //SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),
                SLThung = Math.Ceiling(group.Sum(row => group.Key.SLPCB == 0 ? 0 : (Convert.ToDouble(row["SLDT"])) / group.Key.SLPCB)),
                NW = group.Key.NW,
                GW = group.Key.GW,
                //ChieuDai = group.Key.ChieuDai,
                //ChieuRong = group.Key.ChieuRong,
                //ChieuCao = group.Key.ChieuCao,
                //KyHieu = group.Key.KyHieu
            }).ToList();
            string json2 = JsonConvert.SerializeObject(lstTempPCB);
            DataTable dtTempPCB = JsonConvert.DeserializeObject<DataTable>(json2);
            //dtTempPCB = dtTempPCB.AsEnumerable().OrderBy(row => Convert.ToInt32(row["SapXep"])).CopyToDataTable();
            gridControlSize.DataSource = dtTempPCB;
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
            LoadPCBSize();
            _dtData.Clear();
            ProcessPackage();
            gridviewDetail.RefreshData();
            grvSize.RefreshData();
            bandedGridViewKHDT.RefreshData();
        }
        #endregion
        private void CreateBandSize(DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];
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
                gb.Name = "gb" + "Size_" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size_" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private void ProcessPackage()
        {
            try
            {

                this.ActiveControl = dgrKHDongThung;
                DataTable _dtTemp11 = (DataTable)gridControlSize.DataSource;
                if (_dtTemp11 is null || _dtTemp11.Rows.Count == 0) return;
                var _dtTemp1 = _dtTemp11.AsEnumerable().Select(x => new
                {
                    SizeID = x["SizeID"].ToString(),
                    Size = x["Size"].ToString()
                }).Distinct().ToList();
                string json = JsonConvert.SerializeObject(_dtTemp1);
                DataTable _dtTemp = JsonConvert.DeserializeObject<DataTable>(json);
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
                _dtData = KHDongThungLib.CreateTblPackage();

                DataTable _dtData_du = _dtData.Copy();
                int p = 0;
                int startThung = 0;
                int SttThung = 1;
                double ChieuDai = 0, ChieuRong = 0, ChieuCao = 0, TLThungChan = 0;
                double ChieuDaiLe = 0, ChieuRongLe = 0, ChieuCaoLe = 0, TLThungLe = 0;
                string KiHieu = "0x0x0", KiHieuLe = "0x0x0", MaQuiCach = "", MaQuiCachLe = "";
                bool CheckLayQCLe = false;
                string Option = barOption.EditValue.ToString();
                if (!int.TryParse(barThungLimit.EditValue.ToString(), out int SLThungLimit)) SLThungLimit = 0;
                if (dtQuiCach != null && dtQuiCach.Rows.Count > 0)
                {
                    ChieuDai = Convert.ToDouble(dtQuiCach.Rows[0]["ChieuDai"]);
                    ChieuRong = Convert.ToDouble(dtQuiCach.Rows[0]["ChieuRong"]);
                    ChieuCao = Convert.ToDouble(dtQuiCach.Rows[0]["ChieuCao"]);
                    TLThungChan = Convert.ToDouble(dtQuiCach.Rows[0]["TLThung"]);
                    KiHieu = dtQuiCach.Rows[0]["ChieuDai"] + "x" + dtQuiCach.Rows[0]["ChieuRong"] + 'x' + dtQuiCach.Rows[0]["ChieuCao"];
                    MaQuiCach = dtQuiCach.Rows[0]["MaQuiCach"].ToString();
                }
                if (dtQuiCach != null && dtQuiCach.Rows.Count > 1 && dtQuiCach.Rows[0]["SapXep"].ToString() != "0")
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
                    int _tuthung = 0, _dednthung = 0, _soluongKH = 0, _sltrongthung = 0, _sltemp = 0, _songuyen = 0, _sodu = 0, _total;
                    if (itemPO is null) continue;
                    var _dtdataPOT = dtSize.AsEnumerable().Where(y => y["POID"].ToString() == itemPO.POID.ToString());
                    if (_dtdataPOT.Count() == 0) continue;
                    var _dtdataPO = _dtdataPOT.CopyToDataTable();
                    var _dtdistinctDVSX = dtSize.AsEnumerable().Select(x => new
                    {
                        MaDVSX = x["MaDVSX"],
                        TenDVSX = x["TenDVSX"],
                        MaLenh = x["MaLenh"],
                        DotSX = x["DotSX"]
                    }).Distinct().ToList();

                    foreach (var itemDVSX in _dtdistinctDVSX)
                    {
                        if (itemDVSX is null) continue;
                        var POID = itemPO.POID;
                        var PO = itemPO.PO;
                        var _dtdataT = _dtdataPO.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString());
                        if (_dtdataT.Count() == 0) continue;
                        DataTable _dtdata = _dtdataT.CopyToDataTable();
                        var _dtDistinct_Mau = _dtdata.AsEnumerable().Select(x => new
                        {
                            TenMau = x["TenMau"],
                            ColorID = x["ColorID"],
                        }).Distinct().ToList();

                        foreach (var itemMau in _dtDistinct_Mau)
                        {
                            if (CheckTheoMau && !chkb_size.Checked)
                            {
                                _tuthung = 0;
                                _dednthung = 0;
                            }
                            if (itemMau is null) continue;
                            var _dtDistinctDT = _dtdata.AsEnumerable().Where(y => y["ColorID"].ToString() == itemMau.ColorID.ToString()).Select(x => new
                            {
                                SizeTypeID = x["SizeTypeID"],
                                SizeType = x["SizeType"],

                            }).Distinct().ToList();
                            foreach (var itemData in _dtDistinctDT)
                            {
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
                                    var _dtTempA = _dtTemp11.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString() &&
                                                                                      x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());

                                    //var _dtTempA = _dtdata.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                    //                                                   && x["ColorID"].ToString() == itemMau.ColorID.ToString()
                                    //                                                   && x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());
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
                                    // var NW = _dtTemp.Rows[i]["NW"].ToString() == "" ? 0 : Convert.ToDouble(_dtTemp.Rows[i]["NW"]);
                                    // var GW = _dtTemp.Rows[i]["GW"].ToString() == "" ? 0 : Convert.ToDouble(_dtTemp.Rows[i]["GW"]);
                                    DataRow _dr = _dtData.NewRow();
                                    string _colgroup = "PO: " + PO;
                                    _dr["ColGroup"] = _colgroup;
                                    _dr["POID"] = POID;
                                    _dr["PO"] = PO;
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
                                    _dr["IsThungLe"] = false;
                                    _dr["IsSave"] = false;
                                    _dr["Chon"] = false;
                                    _dr["SttThung"] = SttThung;
                                    _dr["KieuLap"] = CheckTheoMau ? "1" : "0";
                                    string colSize = _dtTemp.Rows[i]["Size"].ToString() + "@" + _dtTemp.Rows[i]["SizeID"].ToString();
                                    if (p == 0)
                                    {
                                        Console.WriteLine(colSize);
                                        _dtData.Columns.Add(colSize, typeof(int));
                                        _dtData_du.Columns.Add(colSize, typeof(int));
                                    }
                                    var _dtslkhsize = dtSize.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString()
                                                                                     && x["POID"].ToString() == POID.ToString()
                                                                                     && x["MaLenh"].ToString() == itemDVSX.MaLenh.ToString()
                                                                                     && x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                                                                     && x["ColorID"].ToString() == itemMau.ColorID.ToString()
                                                                                     && x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());
                                    if (_dtslkhsize.Count() == 0) continue;
                                    var _dtslkhsizea = _dtslkhsize.CopyToDataTable();
                                    _soluongKH = _dtslkhsizea.AsEnumerable().Sum(x => Convert.ToInt32(x["SLDT"]));
                                    if (_soluongKH == 0)
                                    {
                                        continue;
                                    }
                                    var _dtTempPCBT = _dtTemp11.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString() &&
                                                                                      x["SizeID"].ToString() == _dtTemp.Rows[i]["SizeID"].ToString());
                                    if (_dtTempPCBT.Count() == 0) continue;
                                    var _dtTempPCB = _dtTempPCBT.CopyToDataTable();

                                    var SLPCB = _dtTempPCB.Rows[0]["SLPCB"].ToString() == "" ? 0 : _dtTempPCB.Rows[0]["SLPCB"];
                                    _sltrongthung = Convert.ToInt32(SLPCB);
                                    int SlSize = _sltrongthung > _soluongKH ? _soluongKH : _sltrongthung;
                                    bool CheckIsThungLe = _sltrongthung > _soluongKH ? true : false;
                                    bool checkThungDu = false;
                                    if (_sltrongthung > 0)
                                    {
                                        _songuyen = _soluongKH / _sltrongthung; // chia lấy phần nguyên
                                        _sodu = _soluongKH % _sltrongthung; //chia lấy dư

                                        if (barOption.EditValue == "2")
                                            checkThungDu = (_sodu < _songuyen && _sodu < SLThungLimit) ? true : false;
                                        else checkThungDu = (_sodu < _songuyen) ? true : false;

                                        if (Option == "4")
                                        {
                                            checkThungDu = false;
                                        }
                                        //var checkThungDuOption4 = Option == "4" ? ((_sodu > 0) ? true : false) : false;
                                        if (Option != "1" || !checkThungDu)
                                        {
                                            if (checkThungDu)
                                            {
                                                _tuthung = _dednthung + 1 + startThung;
                                                _dednthung = _dednthung + (_songuyen == 0 ? 1 : _songuyen - _sodu) + startThung;
                                            }
                                            else
                                            {
                                                _tuthung = _dednthung + 1 + startThung;
                                                _dednthung = _dednthung + (_songuyen == 0 ? 1 : _songuyen) + startThung;
                                            }
                                        }
                                        double KhoiLuong, TrongLuong;
                                        if (Option != "1" || !checkThungDu)
                                        {
                                            _carton = _dednthung - _tuthung + 1;
                                            _total = _carton * SlSize;
                                            _dr["TuThung"] = _tuthung;
                                            _dr["DenThung"] = _dednthung;
                                            _dr[colSize] = SlSize/*_sltrongthung*/;
                                            _dr["SLThung"] = _carton;
                                            _dr["TotalPiece"] = _total;
                                            _dr["IsThungLe"] = false;
                                            KhoiLuong = Math.Round(_total * NW, 1);
                                            TrongLuong = KhoiLuong + TLThungChan * _carton;
                                            _dr["TrongLuong"] = TrongLuong;
                                            _dr["KhoiLuong"] = KhoiLuong;
                                            _dr["TrongLuongA"] = TLThungChan;
                                            if (!CheckIsThungLe) _dtData.Rows.Add(_dr);
                                        }
                                        if ((_sodu > 0 && _songuyen != 0) || CheckIsThungLe)
                                        {
                                            //DataRow _drDu = _dtData.NewRow();
                                            DataRow _drDu = ((chkv_thungle_cuoi.Checked || cbxThungLeVeCuoiMau.Checked) && Option == "4") ? _dtData_du.NewRow() : _dtData.NewRow();
                                            _colgroup = "PO: " + PO;
                                            _drDu["ColGroup"] = _colgroup;
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

                                            bool CheckKLThung = (_sodu > Math.Round(Convert.ToDouble(_sltrongthung) / 2, 1) ? true : false) || !CheckLayQCLe || checkThungDu;
                                            _drDu["ChieuDai"] = 0;
                                            _drDu["ChieuRong"] = 0;
                                            _drDu["ChieuCao"] = 0;
                                            _drDu["KyHieu"] = CheckKLThung ? MaQuiCach : MaQuiCachLe;
                                            _drDu["IsThungLe"] = Option == "4" ? true : false;
                                            _drDu["IsSave"] = false;
                                            _drDu["Chon"] = false;
                                            SttThung++;
                                            _drDu["SttThung"] = SttThung;
                                            KhoiLuong = Math.Round(_sodu * NW, 1);
                                            TrongLuong = KhoiLuong + (CheckKLThung ? TLThungChan : TLThungLe);
                                            _drDu["TrongLuong"] = TrongLuong;
                                            _drDu["KhoiLuong"] = KhoiLuong;
                                            _drDu["KieuLap"] = CheckTheoMau ? "1" : "0";
                                            if ((chkv_thungle_cuoi.Checked || cbxThungLeVeCuoiMau.Checked) && Option == "4")
                                            {
                                                if (CheckIsThungLe) _dednthung = _dednthung - 1;
                                                _drDu[colSize] = _sodu;
                                                _dtData_du.Rows.Add(_drDu);
                                            }
                                            else
                                            {
                                                _tuthung = _dednthung + 1 + startThung;
                                                _dednthung = _dednthung + startThung + (checkThungDu ? _sodu : 1);
                                                if (CheckIsThungLe)
                                                {
                                                    _tuthung -= 1;
                                                    _dednthung -= 1;
                                                }
                                                _drDu["TuThung"] = _tuthung;
                                                _drDu["DenThung"] = _dednthung;
                                                _carton = checkThungDu ? _sodu : 1;
                                                _total = checkThungDu ? (_sltrongthung + 1) * _sodu : _sodu; ;
                                                _drDu[colSize] = checkThungDu ? _sltrongthung + 1 : _sodu;
                                                _drDu["SLThung"] = _carton;
                                                _drDu["TotalPiece"] = _total;
                                                _dtData.Rows.Add(_drDu);
                                            }
                                        }
                                        if (Option == "1" && checkThungDu)
                                        {
                                            if (checkThungDu)
                                            {
                                                _tuthung = _dednthung + 1 + startThung;
                                                _dednthung = _dednthung + (_songuyen == 0 ? 1 : _songuyen - _sodu) + startThung;
                                            }
                                            else
                                            {
                                                _tuthung = _dednthung + 1 + startThung;
                                                _dednthung = _dednthung + (_songuyen == 0 ? 1 : _songuyen) + startThung;
                                            }
                                            _carton = _dednthung - _tuthung + 1;
                                            _total = _carton * SlSize;
                                            _dr["TuThung"] = _tuthung;
                                            _dr["DenThung"] = _dednthung;
                                            _dr[colSize] = SlSize/*_sltrongthung*/;
                                            _dr["SLThung"] = _carton;
                                            _dr["TotalPiece"] = _total;
                                            _dr["IsThungLe"] = false;
                                            KhoiLuong = Math.Round(_total * NW, 1);
                                            TrongLuong = KhoiLuong + TLThungChan * _carton;
                                            _dr["TrongLuong"] = TrongLuong;
                                            _dr["KhoiLuong"] = KhoiLuong;
                                            _dr["TrongLuongA"] = TLThungChan;
                                            if (!CheckIsThungLe) _dtData.Rows.Add(_dr);
                                        }

                                    }
                                    SttThung++;
                                }
                                p++;

                            }
                            if (((chkv_thungle_cuoi.Checked && CheckTheoMau) || cbxThungLeVeCuoiMau.Checked) && Option == "4")
                            {
                                string _size = string.Empty;
                                for (int d = 0; d < _dtData_du.Rows.Count; d++)
                                {
                                    if (chkb_size.Checked)
                                    {
                                        try
                                        {
                                            if (_size != _dtData_du.Columns[d].ColumnName)
                                            {
                                                _tuthung = 0;
                                                _dednthung = 0;
                                                _size = _dtData_du.Columns[d].ColumnName;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    DataRow _drDu = _dtData.NewRow();
                                    string _colgroup = "PO: " + _dtData_du.Rows[d]["PO"].ToString();
                                    _drDu["ColGroup"] = _colgroup;

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
                                    _drDu["IsSave"] = false;
                                    _drDu["Chon"] = false;
                                    _tuthung = _dednthung + 1;
                                    _dednthung = _dednthung + 1;
                                    _drDu["TuThung"] = _tuthung;
                                    _drDu["DenThung"] = _dednthung;
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
                                    _drDu["SttThung"] = _dtData_du.Rows[d]["SttThung"];
                                    _drDu["SLThung"] = _carton;
                                    _drDu["TotalPiece"] = _total;
                                    _drDu["TrongLuong"] = _dtData_du.Rows[d]["TrongLuong"];
                                    _drDu["KhoiLuong"] = _dtData_du.Rows[d]["KhoiLuong"];
                                    _drDu["KieuLap"] = _dtData_du.Rows[d]["KieuLap"];
                                    _dtData.Rows.Add(_drDu);
                                }
                                _dtData_du.Clear();
                            }
                        }
                        startThung = 0;
                    }
                    if (chkv_thungle_cuoi.Checked && !CheckTheoMau && !cbxThungLeVeCuoiMau.Checked && Option == "4")
                    {
                        string _size = string.Empty;
                        for (int d = 0; d < _dtData_du.Rows.Count; d++)
                        {
                            if (chkb_size.Checked)
                            {
                                try
                                {
                                    if (_size != _dtData_du.Columns[d].ColumnName)
                                    {
                                        _tuthung = 0;
                                        _dednthung = 0;
                                        _size = _dtData_du.Columns[d].ColumnName;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                            }
                            DataRow _drDu = _dtData.NewRow();
                            string _colgroup = "PO: " + _dtData_du.Rows[d]["PO"].ToString();
                            _drDu["ColGroup"] = _colgroup;

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
                            _drDu["IsSave"] = false;
                            _drDu["Chon"] = false;
                            _tuthung = _dednthung + 1;
                            _dednthung = _dednthung + 1;
                            _drDu["TuThung"] = _tuthung;
                            _drDu["DenThung"] = _dednthung;
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
                            _drDu["SttThung"] = _dtData_du.Rows[d]["SttThung"];
                            _drDu["SLThung"] = _carton;
                            _drDu["TotalPiece"] = _total;
                            _drDu["TrongLuong"] = _dtData_du.Rows[d]["TrongLuong"];
                            _drDu["KhoiLuong"] = _dtData_du.Rows[d]["KhoiLuong"];
                            _drDu["KieuLap"] = _dtData_du.Rows[d]["KieuLap"];
                            _dtData.Rows.Add(_drDu);
                        }
                        _dtData_du.Clear();
                    }
                }
                //dgrKHDongThung.MainView = GetBandGridViewAmount_grd1(_dtData);
                //BandedGridView mainView = (BandedGridView)dgrKHDongThung.MainView;

                //var dataOld = (DataTable)dgrKHDongThung.DataSource;
                foreach (DataRow dr in _dtData.Rows)
                {
                    Int64 slSP = 0;
                    foreach (DataColumn dc in _dtData.Columns)
                    {
                        var colName = dc.ColumnName;
                        if (!colName.Contains("@")) continue;
                        slSP += dr[dc].ToString() != "" ? Convert.ToInt64(dr[dc.ColumnName]) : 0;
                    }
                    dr["SoLuong"] = slSP;
                }
                CreateBandSize(_dtData);
                dgrKHDongThung.DataSource = _dtData;
                bandedGridViewKHDT.ExpandAllGroups();
            }
            catch (Exception ex)
            {

            }
        }
        private async void SaveRows()
        {
            try
            {
                DataTable tblSave = KHDongThungLib.CreateTblSave();
                DateTime dtNow = DateTime.Now;
                int TuThung_Notdeca = 0;
                int SttThung_temp = 0;
                string _colorIDOld = "";
                string _POIDOld = "";
                int sttThung = 0;
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
                    if (_POIDOld != _poid)
                    {
                        sttThung = 0;
                        _POIDOld = _poid;
                    }
                    string _sizeID = string.Empty;
                    string _size = string.Empty;
                    int slThung = Convert.ToInt32(dr["SLThung"].ToString());
                    int TuThung = Convert.ToInt32(dr["TuThung"].ToString());
                    int DenThung = Convert.ToInt32(dr["DenThung"].ToString());

                    for (int z = 0; z < _dtData.Columns.Count; z++)
                    {
                        string[] arrName = _dtData.Columns[z].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrName.Length > 1)
                        {
                            string colName = _dtData.Columns[z].ColumnName;
                            if (dr[colName].ToString() == "" || dr[colName].ToString() == "0") continue;
                            _sizeID = arrName[1];
                            _size = arrName[0];
                            // string urlSize = string.Format("{0}/GET_SizeID?malenh={1}&&spoid={2}&&sizetypeid={3}&&colorid={4}&&sizeid={5}", URL + ResourceURL.UrlErpKHDongThung_PCB, _malenh, _spoid, _sizeTypeID, _colorID, _size);
                            //DataTable _listSize = Task.Run(async () => { return await _service_Khdt_pcb.GetDT(urlSize); }).Result;
                            //string[] arrPOID = _spoid.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            //if (dr["Size_" + size].ToString() == "" || dr["Size_" + size].ToString() == "0") continue;
                            int SttThungDeCat_Start = 0;
                            int index = 0;
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
                                    drNewRow["SttThung_decat"] = TuThung + i;
                                    drNewRow["SttThung"] = TuThung_Notdeca;
                                }
                                else
                                {
                                    drNewRow["SttThung"] = TuThung + i;
                                    drNewRow["SttThung_decat"] = 0;
                                }
                                sttThung++;
                                index++;
                                SttThung_temp++;
                                drNewRow["SttThung"] = sttThung;
                                drNewRow["SttThung_LapMau"] = SttThung_temp == 0 ? 1 : SttThung_temp;
                                drNewRow["SttThung_temp"] = index == 0 ? 1 : index;
                                if (SttThungDeCat_Start == 0) SttThungDeCat_Start = Convert.ToInt32(drNewRow["SttThung"]);
                                drNewRow["SttThung_start"] = SttThungDeCat_Start;

                                drNewRow["SoLuongSP"] = Convert.ToInt32(dr[colName].ToString());
                                drNewRow["IsDongThung"] = false;
                                drNewRow["NgayDongThung"] = dtNow;
                                drNewRow["NgayNhapKho"] = dtNow;
                                drNewRow["QRCode"] = "";
                                drNewRow["IsNhapKho"] = false;
                                drNewRow["KyHieu"] = dr["KyHieu"];
                                drNewRow["Chon"] = false;
                                drNewRow["IsThungLe"] = dr["IsThungLe"];
                                drNewRow["KieuLap"] = CheckTheoMau ? "1" : "0";
                                drNewRow["NVien"] = GlobleData.UserName;
                                drNewRow["KieuLapPCB"] = "0";
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
        private void DeleteKHDT()
        {
            DataTable tblDelete = KHDongThungLib.CreateTblSave();
            var drNewRowD = tblDelete.NewRow();
            drNewRowD["ID"] = 0;
            drNewRowD["MaDH"] = _madh;
            tblDelete.Rows.Add(drNewRowD);
            string url = string.Format("{0}", URL + "KeHoachDongThung/Delete?action=DeleteLapPCB");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblDelete); }).Result;
        }

        #endregion
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetSLDM();
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
                    //SLThung = Convert.ToInt32(row["SLThung"]),
                })
                .Select(group => new
                {
                    SizeID = group.Key.SizeID,
                    Size = group.Key.Size,
                    SLPCB = group.Key.SLPCB,
                    //SLThung = group.Key.SLThung,
                    SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                    SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),
                }).ToList();
            string jsonT = JsonConvert.SerializeObject(groupedSum);
            DataTable dtSizetemp = JsonConvert.DeserializeObject<DataTable>(jsonT);
            gridControlSize.DataSource = dtSizetemp;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ProcessPackage();
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
            BtnSave();
        }

        private string GetSizeName(string SizeID)
        {
            return dtSizeAll.AsEnumerable().Where(x => x["SizeID"].ToString() == SizeID).FirstOrDefault()["Size"].ToString();
        }

        private void chkv_thungle_cuoi_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProcessPackage();
        }

        private void chkb_size_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (chkb_size.Checked) chbxTheoMau.Checked = false;
            ProcessPackage();
        }
        private void chbxTheoMau_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (chbxTheoMau.Checked) chkb_size.Checked = false;
            CheckTheoMau = chbxTheoMau.Checked;
            ProcessPackage();
        }
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SLPCB")
            {
                flagChangePCB = true;
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

        private void bandedGridViewKHDT_ShowingEditor(object sender, CancelEventArgs e)
        {
            KHDongThungLib.EventShowingEditor(bandedGridViewKHDT, e);
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

        private void bandedGridViewKHDT_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = (bool)bandedGridViewKHDT.GetRowCellValue(e.RowHandle, "IsThungLe");
            if (IsThungLe) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }

        private void gridviewDetail_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            //DataRow drFocus = gridviewDetail.GetFocusedDataRow();
            //if (drFocus == null || e.Menu == null) return;
            //// if ((bool)drFocus["IsSave"]) return;
            //DXMenuItem menuDelete = new DXMenuItem();
            //menuDelete.Caption = "Chỉnh sửa SL";
            //menuDelete.Click += MenuDelete_Click;
            //e.Menu.Items.Add(menuDelete);
        }

        private void cbxThungLeVeCuoiMau_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProcessPackage();
        }

        private void txtPCB_EditValueChanged(object sender, EventArgs e)
        {
            flagChangePCB = true;
            var dt = gridControlSize.DataSource as DataTable;
            if (int.TryParse(txtPCB.Text, out int PCB))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SLPCB"] = PCB;
                }
            }
            else if (txtPCB.Text == "")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["SLPCB"] = 0;
                }
            }
        }

        private void gridviewDetail_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                gridviewDetail.CloseEditor();
                gridviewDetail.UpdateCurrentRow();
                DataTable dt = grcDetail.DataSource as DataTable;
                string _fieldName = gridviewDetail.FocusedColumn.FieldName;
                if (_fieldName != "SLDT") return;
                KHDongThungLib.HandleGridViewKey(e, gridviewDetail, _fieldName);
            }
            catch (Exception ex) { }
        }

        private void btnCaiDatTSPKL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmSaveTL_KL._styleID = _maHang;
            //frmSaveTL_KL frm = new frmSaveTL_KL();
            //frm.ShowDialog();
            frmCaiDatTrongLuongThung.MaHang = _maHang;
            frmCaiDatTrongLuongThung frm = new frmCaiDatTrongLuongThung();
            frm.ShowDialog();
            dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension, _maNoiDen);
            LoadNoiDen();
        }

        private void btnLapKH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LoadPCBSize();
            //if (flagChangePCB)
            //{
            //    DialogResult dialogResult = MessageBox.Show("Bạn có muốn áp dụng PCB này cho các lần lập KH sau không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if (dialogResult == DialogResult.Yes)
            //    {

            //    }
            //}
            DataTable tblPCB = KHDongThungLib.CreateTblPCB();
            DataTable tblSource = gridControlSize.DataSource as DataTable;
            foreach (DataRow dr in tblSource.Rows)
            {
                DataRow drNew = tblPCB.NewRow();
                drNew["StyleID"] = _maHang;
                drNew["MaHang"] = _maHang;
                drNew["DauSizeID"] = dr["SizeTypeID"];
                drNew["DauSize"] = dr["SizeType"];
                drNew["SizeID"] = dr["SizeID"];
                drNew["Size"] = dr["Size"];
                drNew["PCB_Pack"] = 0;
                drNew["Pack_Ctn"] = 0;
                drNew["PCB"] = dr["SLPCB"];
                drNew["SL_TrongLuong"] = dr["GW"];
                drNew["SL_KhoiLuong"] = dr["NW"];
                drNew["MaNoiDen"] = _maNoiDen;
                tblPCB.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Post?action=UpdatePCB");
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblPCB); }).Result;
            flagChangePCB = false;
            ProcessPackage();
        }

        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value != null && (e.Value.ToString() == "0" || e.Value.ToString() == "")) e.DisplayText = "-";
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


        BandedGridView bandedView;
        void bandedView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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
        private void bandedGridViewKHDT_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
        }
        private void bandedGridViewKHDT_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }

        private void BtnSave()
        {

            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            SaveRows();
        }
    }
}
