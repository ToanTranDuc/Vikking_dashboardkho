using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
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
    public partial class frmERPKeHoachDongThung_Store : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        SearchCheckSelection gridCheckMarksDVSX;
        SearchCheckSelection gridCheckMarksPO;
        SearchCheckSelection gridCheckMarksSizeType;
        private string _madh = string.Empty, _maHang = string.Empty, _maDVSX = string.Empty, _po = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty;
        private string _tempPOStore = "";
        DataTable dtSize = new DataTable();
        DataTable dtStore = new DataTable();
        DataTable dtSizeAll = new DataTable();
        DataTable dtSizePCB = new DataTable();
        DataTable _dtData = new DataTable();
        DataTable _dtInit = new DataTable();
        DataTable dtQuiCach = new DataTable();

        int _carton = 0, thung_po = 0;
        bool CheckTheoMau = false;
        KeyDownControlHandler keyDownControlHandler;
        public frmERPKeHoachDongThung_Store(string MaDH, string MaHang)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _madh = MaDH;
            _maHang = MaHang;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            KHDongThungLib.InitQuiCach(repoQuiCach);
            dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension);
            LoadStore();
            //GetQuiCach();
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
        private void LoadStore()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetStore&MaDH={_maHang}&MaDVSX=Para&DotSX=Para" +
                                                   $"&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtStore = JsonConvert.DeserializeObject<DataTable>(json);

        }
        private void LoadStoreA()
        {
            DataTable _dtTemp11 = (DataTable)grcDetail.DataSource;
            if (_dtTemp11 is null || _dtTemp11.Rows.Count == 0) return;
            var _dtTemp1 = _dtTemp11.AsEnumerable().Where(x => x["POID"].ToString() == _tempPOStore.ToString()).AsEnumerable().Select(x => new
            {
                SizeID = x["SizeID"].ToString(),
                Size = x["Size"].ToString()
            }).Distinct().ToList();
            string json = JsonConvert.SerializeObject(_dtTemp1);
            DataTable _dtTemp = JsonConvert.DeserializeObject<DataTable>(json);
            var dtTemp = dtStore.AsEnumerable().Where(x => x["POID"].ToString() == _tempPOStore.ToString());
            if (dtTemp.Count() == 0)
            {
                gridControlSize.DataSource = new DataTable();
                return;
            }
            var dtViewStore = dtTemp.CopyToDataTable();

            if (dtViewStore.Rows.Count == 0) return;
            grvStore.Columns.Clear();
            CreateColumnStore("Store", "Store");
            CreateColumnStore("Màu", "MaMau");

            foreach (var item in _dtTemp1)
            {
                //string colName = dc.ColumnName;
                //if (!colName.Contains('@')) continue;
                CreateColumnStore(item.Size, item.Size + "@" + item.SizeID);
            }

            CreateColumnStore("SL Thùng", "SLThung");
            gridControlSize.DataSource = dtViewStore;
        }
        private void GetSLDM()
        {
            try
            {
                string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=Get_SLDM_PCB&MaDH={_madh}&MaDVSX=Para&DotSX=Para" +
                                                    $"&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
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
                var lstTempPCB = dtSize.AsEnumerable().GroupBy(x => new
                {
                    SizeTypeID = x.Field<string>("SizeTypeID"),
                    SizeType = x.Field<string>("SizeType"),
                    Size = x["Size"],
                    SizeID = x["SizeID"],
                    SLPCB = Convert.ToInt32(x["SLPCB"]),
                    //NW = Convert.ToDouble(x["NW"]),
                    //GW = Convert.ToDouble(x["GW"]),
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
                    //SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                    SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),
                    SLThung = Math.Ceiling(group.Sum(row => group.Key.SLPCB == 0 ? 0 : (Convert.ToDouble(row["SLDT"])) / group.Key.SLPCB)),
                    //NW = group.Key.NW,
                    //GW = group.Key.GW,
                    //ChieuDai = group.Key.ChieuDai,
                    //ChieuRong = group.Key.ChieuRong,
                    //ChieuCao = group.Key.ChieuCao,
                    //KyHieu = group.Key.KyHieu
                }).ToList();
                string json1 = JsonConvert.SerializeObject(lstTemp);
                DataTable dtTemp = JsonConvert.DeserializeObject<DataTable>(json1);
                string json2 = JsonConvert.SerializeObject(lstTempPCB);
                DataTable dtTempPCB = JsonConvert.DeserializeObject<DataTable>(json2);
                grcDetail.DataSource = dtSize;
                gridviewDetail.OptionsBehavior.AutoExpandAllGroups = true;
                gridviewDetail.RefreshData();
                //gridControlSize.DataSource = dtTempPCB;
            }
            catch (Exception ex)
            {

            }
        }
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
                DataTable _dtTemp11 = (DataTable)grcDetail.DataSource;
                List<Store_DongThungEntity> lstData = new List<Store_DongThungEntity>();
                List<DTStoreAlarm_Entity> lstAlarm = new List<DTStoreAlarm_Entity>();
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
                        var _dtdataT = _dtdataPO.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString());
                        if (_dtdataT.Count() == 0) continue;
                        DataTable _dtdata = _dtdataT.CopyToDataTable();
                        var _dtDistinct_Mau = _dtdata.AsEnumerable().Select(x => new
                        {
                            TenMau = x["TenMau"],
                            ColorID = x["ColorID"],
                        }).Distinct().ToList();
                        if (CheckTheoMau && !chkb_size.Checked)
                        {
                            _tuthung = 0;
                            _dednthung = 0;
                        }
                        foreach (var itemMau in _dtDistinct_Mau)
                        {
                            var _dtDistinctDT = _dtdata.AsEnumerable().Where(y => y["ColorID"].ToString() == itemMau.ColorID.ToString()).Select(x => new
                            {
                                SizeTypeID = x["SizeTypeID"],
                                SizeType = x["SizeType"],

                            }).Distinct().ToList();
                            foreach (var itemData in _dtDistinctDT)
                            {
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
                                //_dr["SttThung"] = SttThung;
                                _dr["KieuLap"] = "0";
                                lstData.Clear();
                                foreach (DataRow dr in _dtTemp.Rows)
                                {

                                    var _dtTempA = _dtdata.AsEnumerable().Where(x => x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                                                                       && x["ColorID"].ToString() == itemMau.ColorID.ToString()
                                                                                       && x["SizeID"].ToString() == dr["SizeID"].ToString());
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

                                    string colSize = dr["Size"].ToString() + "@" + dr["SizeID"].ToString();
                                    if (p == 0)
                                    {
                                        Console.WriteLine(colSize);
                                        _dtData.Columns.Add(colSize, typeof(int));
                                    }
                                    var _dtslkhsize = dtSize.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString()
                                                                                     && x["POID"].ToString() == POID.ToString()
                                                                                     && x["MaLenh"].ToString() == itemDVSX.MaLenh.ToString()
                                                                                     && x["SizeTypeID"].ToString() == itemData.SizeTypeID.ToString()
                                                                                     && x["ColorID"].ToString() == itemMau.ColorID.ToString()
                                                                                     && x["SizeID"].ToString() == dr["SizeID"].ToString());
                                    if (_dtslkhsize.Count() == 0) continue;
                                    var _dtslkhsizea = _dtslkhsize.CopyToDataTable();
                                    var SLDT = _dtslkhsizea.AsEnumerable().Sum(x => Convert.ToInt32(x["SLDT"]));
                                    if (SLDT == 0)
                                    {
                                        continue;
                                    }

                                    lstData.Add(new Store_DongThungEntity()
                                    {
                                        SizeID = dr["SizeID"].ToString(),
                                        Size = dr["Size"].ToString(),
                                        SLDT = SLDT,
                                        NW = NW,
                                        SLConLai = SLDT,
                                    });
                                }
                                p++;
                                var dtStoreTemp = dtStore.AsEnumerable().Where(x => x["POID"].ToString() == POID.ToString() && x["ColorID"].ToString() == itemMau.ColorID.ToString());
                                if (dtStoreTemp.Count() == 0) continue;
                                var dtStoreTempA = dtStoreTemp.CopyToDataTable();
                                foreach (DataRow dr in dtStoreTempA.Rows)
                                {
                                    var _drAdd = _dtData.NewRow();
                                    int totalpiece = 0;
                                    double _STrongLuong = 0;
                                    double _SKhoiLuong = 0;
                                    KHDongThungLib.CopyDataRow(_dr, _drAdd);
                                    bool IsThieu = false;
                                    foreach (DataColumn dc in dtStoreTempA.Columns)
                                    {
                                        if (!dc.ColumnName.Contains('@')) continue;
                                        var SizeID = dc.ColumnName.Split('@')[1].ToString().Trim();
                                        var Size = dc.ColumnName.Split('@')[0].ToString().Trim();
                                        //foreach(var item in lstData)
                                        //{
                                        //    if (item.SizeID.ToString() == dc.ColumnName.Split('@')[1].ToString()) ;
                                        //    item.SLConLai = item.SLConLai - Convert.ToInt32(dr[dc.ColumnName]);
                                        //}    
                                        var item = lstData.Where(x => x.SizeID.ToString() == SizeID).FirstOrDefault();
                                        if (item is null) continue;
                                        var SLSP = Convert.ToInt32(dr[dc.ColumnName]);

                                        if (item.SLConLai < SLSP)
                                        {
                                            lstAlarm.Add(new DTStoreAlarm_Entity()
                                            {
                                                PO = PO.ToString(),
                                                Size = Size,
                                                Store = dr["Store"].ToString(),
                                                SLuong = SLSP - item.SLConLai,
                                            });
                                            SLSP = item.SLConLai;
                                            if (!IsThieu) IsThieu = true;
                                        }
                                        int SLThung = Convert.ToInt16(dr["SLThung"]);
                                        item.SLConLai = item.SLConLai - SLSP * SLThung;
                                        _drAdd[dc.ColumnName] = SLSP;
                                        _SKhoiLuong += item.NW * SLSP * SLThung;
                                        totalpiece += SLSP * SLThung;

                                    }
                                    _drAdd["KyHieu"] = dr["QuiCach"];
                                    _drAdd["TrongLuong"] = _SKhoiLuong + Convert.ToDouble(dr["CanNang"]);
                                    _drAdd["KhoiLuong"] = _SKhoiLuong;
                                    _drAdd["TuThung"] = _dednthung + 1;
                                    _dednthung = _dednthung + Convert.ToInt16(dr["SLThung"]);
                                    _drAdd["DenThung"] = _dednthung;
                                    _drAdd["Store"] = dr["Store"].ToString();
                                    _drAdd["SLThung"] = Convert.ToInt16(dr["SLThung"]);
                                    _drAdd["TotalPiece"] = totalpiece;
                                    _drAdd["SttThung"] = SttThung;
                                    _drAdd["IsStoreThieu"] = IsThieu;
                                    SttThung++;

                                    _dtData.Rows.Add(_drAdd);

                                }

                            }

                        }
                        startThung = 0;
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
                string msg = "Số lượng không đủ ở \r\n";
                var lstStoreAlarm = lstAlarm.Select(x => x.Store).Distinct().ToList();
                foreach (var store in lstStoreAlarm)
                {
                    msg += $"Store: {store}\r\n";
                    var lstTemp = lstAlarm.Where(x => x.Store == store).ToList();
                    foreach (var item in lstTemp)
                    {
                        msg += $"  - PO: {item.PO} - Size: {item.Size} - SL: {item.SLuong}\r\n";
                    }
                }
                if (lstAlarm.Count > 0)
                {
                    MessageBox.Show(msg + "\r\nVui lòng kiểm tra lại số lượng");
                }
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
                int SttThungOld = 0;
                string _colorIDOld = "";
                string _POIDOld = "";
                int sttThung = 0;
                //int index1 = 0;
                foreach (DataRow dr in _dtData.Rows)
                {
                    var _poid = dr["POID"].ToString();
                    var _po = dr["PO"].ToString();
                    var _sizeType = dr["DauSize"].ToString();
                    var _sizeTypeID = dr["DauSizeID"].ToString();
                    var _colorID = dr["ColorID"].ToString();
                    if (_colorIDOld != _colorID)
                    {
                        SttThung_temp = 1;
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
                    int index = 1;
                    for (int i = 0; i < slThung; i++)
                    {
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
                                //if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                                //{
                                //    sttThung++;
                                //    TuThung_Notdeca++;
                                //    index++;
                                //    SttThung_temp++;
                                //}

                                //index++;
                                //SttThung_temp++;
                                drNewRow["SttThung"] = TuThung + index-1;
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
                                drNewRow["KieuLapPCB"] = "3";
                                drNewRow["Store"] = dr["Store"];
                                //drNewRow["SttThung_decat"] = dr["SttThung"];
                                tblSave.Rows.Add(drNewRow);
                                SttThung_temp++;
                            }
                            //SttThungOld = Convert.ToInt32(dr["SttThung"]);
                        }
                        index++;                      
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
        private void CreateColumnStore(string caption, string filedname)
        {
            GridColumn col = new GridColumn();
            col.Caption = caption;
            col.FieldName = filedname;
            col.AppearanceHeader.BackColor = Color.AliceBlue;
            col.Visible = true;
            grvStore.Columns.Add(col);
        }
        #endregion
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetSLDM();
            LoadStore();
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
            //gridControlSize.DataSource = dtSizetemp;
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
                DataRow dr = grvStore.GetFocusedDataRow();
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
            var IsThieu = (bool)bandedGridViewKHDT.GetRowCellValue(e.RowHandle, "IsStoreThieu");
            if (IsThungLe || IsThieu) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
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

        private void gridviewDetail_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            var value = gridviewDetail.GetFocusedRowCellValue("POID").ToString();
            if (_tempPOStore == value) return;
            _tempPOStore = value;
            LoadStoreA();
        }

        private void grvStore_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value != null && (e.Value.ToString() == "0" || e.Value.ToString() == "")) e.DisplayText = "-";
        }

        private void btnLapKH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
