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
    public partial class frmERPKeHoachDongThungTonKho : DevExpress.XtraEditors.XtraForm
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
        DataTable dtSizeAll = new DataTable();
        DataTable dtSizePCB = new DataTable();
        DataTable _dtData = new DataTable();
        DataTable _dtInit = new DataTable();
        int _carton = 0, thung_po = 0;
        KeyDownControlHandler keyDownControlHandler;
        public frmERPKeHoachDongThungTonKho(string MaDH, string MaHang)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _madh = MaDH;
            _maHang = MaHang;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
           
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GetSLDM();

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
            ActionControl actionControl = new ActionControl(action, permission, actionType,true);
            list.Add(actionControl);
        }

        #region Function
        private void GetSLDM()
        {
            try
            {
                string url = string.Format("{0}?", URL + $"SP_ERP_KHDONGTHUNG_TonKho/Get?Action=Get_SLDM_PCB&MaDH={_maHang}&MaDVSX=Para&DotSX=Para" +
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
                }).Select(group => new {
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
                    Size = x["Size"],
                    SizeID = x["SizeID"],                   
                    SLPCB = Convert.ToInt32(x["SLPCB"]),
                }).Select(group => new {
                    Size = group.Key.Size,
                    SizeID = group.Key.SizeID,
                    SLPCB = group.Key.SLPCB,                    
                    //SLKH = group.Sum(row => Convert.ToInt32(row["SLKH"])),
                    SLDT = group.Sum(row => Convert.ToInt32(row["SLDT"])),   
                    SLThung = Math.Ceiling(group.Sum(row => Convert.ToDouble(row["SLDT"]))/ group.Key.SLPCB),
                }).ToList();
                string json1 = JsonConvert.SerializeObject(lstTemp);
                DataTable dtTemp = JsonConvert.DeserializeObject<DataTable>(json1);
                string json2 = JsonConvert.SerializeObject(lstTempPCB);
                DataTable dtTempPCB = JsonConvert.DeserializeObject<DataTable>(json2);
                grcDetail.DataSource = dtTemp;
                gridviewDetail.RefreshData();
                gridControlSize.DataSource = dtTempPCB;
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
                DataTable _dtTemp = (DataTable)gridControlSize.DataSource;
                _dtData.Clear();
                //var _dtDVSX = dtSize.AsEnumerable().Select(x => new
                //{
                //    DVSX = x["MaDVSX"],
                //    TenDVSX = x["TenDVSX"]
                //}).Distinct().ToList();
                //dtSize = grcDetail.DataSource as DataTable;
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
                _dtData.Columns.Add("DauSize", typeof(string));
                _dtData.Columns.Add("DauSizeID", typeof(string));
                _dtData.Columns.Add("TenMau", typeof(string));
                _dtData.Columns.Add("ColorID", typeof(string));
                _dtData.Columns.Add("StyleID", typeof(string));
                _dtData.Columns.Add("MaHang", typeof(string));

                _dtData.Columns.Add("SoLuong", typeof(int));
                _dtData.Columns.Add("SLThung", typeof(int));
                _dtData.Columns.Add("TotalPiece", typeof(int));
                _dtData.Columns.Add("ChieuDai", typeof(string));
                _dtData.Columns.Add("ChieuRong", typeof(string));
                _dtData.Columns.Add("ChieuCao", typeof(string));
                _dtData.Columns.Add("IsThungLe", typeof(bool));
                _dtData.Columns.Add("Chon", typeof(bool));
                _dtData.Columns.Add("IsSave", typeof(bool));

                DataTable _dtData_du = _dtData.Copy();
                int p = 0;
                int startThung = 0;
                            
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
                        var _dtdata = _dtdataPO.AsEnumerable().Where(x => x["MaDVSX"].ToString() == itemDVSX.MaDVSX.ToString()).CopyToDataTable();
                        var _dtDistinctDT = _dtdata.AsEnumerable().Select(x => new
                        {
                            SizeTypeID = x["SizeTypeID"],
                            SizeType = x["SizeType"],
                            TenMau = x["TenMau"],
                            ColorID = x["ColorID"],
                        }).Distinct().ToList();
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
                                _dr["TenMau"] = itemData.TenMau;
                                _dr["ColorID"] = itemData.ColorID;
                                _dr["ChieuDai"] = 0;
                                _dr["ChieuRong"] = 0;
                                _dr["ChieuCao"] = 0;
                                _dr["IsThungLe"] = false;
                                _dr["IsSave"] = false;
                                _dr["Chon"] = false;
                                string colSize = _dtTemp.Rows[i]["Size"].ToString() + "@" + _dtTemp.Rows[i]["SizeID"].ToString();
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
                                    _dr["SLThung"] = _carton;
                                    _dr["TotalPiece"] = _total;                                    
                                    _dtData.Rows.Add(_dr);
                                    if (_sodu > 0 && _songuyen != 0)
                                    {
                                        //DataRow _drDu = _dtData.NewRow();
                                        DataRow _drDu = chkv_thungle_cuoi.Checked ? _dtData_du.NewRow() : _dtData.NewRow();
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
                                        _drDu["TenMau"] = itemData.TenMau;
                                        _drDu["ColorID"] = itemData.ColorID;
                                        _drDu["ChieuDai"] = 0;//qcDongThungPCB.ChieuDai;
                                        _drDu["ChieuRong"] = 0;//qcDongThungPCB.ChieuRong;
                                        _drDu["ChieuCao"] = 0;//qcDongThungPCB.ChieuCao;
                                        _drDu["IsThungLe"] = true;
                                        _drDu["IsSave"] = false;
                                        _drDu["Chon"] = false;
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

                                            _drDu["SLThung"] = _carton;
                                            _drDu["TotalPiece"] = _total;
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
                            string _colgroup = "PO: " + _dtData_du.Rows[d]["PO"].ToString();
                            _drDu["ColGroup"] = _colgroup;

                            _drDu["MaDVSX"] = _dtData_du.Rows[d]["MaDVSX"].ToString();
                            _drDu["TenDVSX"] = _dtData_du.Rows[d]["TenDVSX"].ToString();
                            _drDu["POID"] = _dtData_du.Rows[d]["POID"].ToString();
                            _drDu["PO"] = _dtData_du.Rows[d]["PO"].ToString();
                            _drDu["MaLenh"] = _dtData_du.Rows[d]["MaLenh"].ToString();
                            _drDu["DotSX"] = _dtData_du.Rows[d]["DotSX"].ToString();

                            _drDu["MaHang"] = _dtData_du.Rows[d]["MaHang"].ToString();
                            _drDu["DauSize"] = _dtData_du.Rows[d]["DauSizeID"].ToString();
                            _drDu["DauSizeID"] = _dtData_du.Rows[d]["DauSizeID"].ToString();
                            _drDu["TenMau"] = _dtData_du.Rows[d]["TenMau"].ToString();
                            _drDu["ColorID"] = _dtData_du.Rows[d]["ColorID"].ToString();
                            _drDu["ChieuDai"] = 0;
                            _drDu["ChieuRong"] = 0;
                            _drDu["ChieuCao"] = 0;
                            _drDu["IsThungLe"] = true;
                            _drDu["IsSave"] = false;
                            _drDu["Chon"] = false;
                            //_dr["Qty"] = _dtTemp.Rows[0]["Qty"].ToString();
                            //_dr["Total"] = _dtTemp.Rows[0]["Total"].ToString();


                            //_sodu = _sodu;
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
                            //for (int col = 0; col < _dtData_du.Columns.Count; col++)
                            //{
                            //    if (!_dtData_du.Columns[col].ColumnName.Contains("Size@")) continue;
                            //    int sl = _dtData_du.Rows[d][_dtData_du.Columns[col].ColumnName].ToString() == "" ? 0 : Convert.ToInt32(_dtData_du.Rows[d][_dtData_du.Columns[col].ColumnName]);
                            //    _drDu[_dtData_du.Columns[col].ColumnName] = sl;
                            //    _total = _total + sl;
                            //}


                            _drDu["SLThung"] = _carton;
                            _drDu["TotalPiece"] = _total;
                            _dtData.Rows.Add(_drDu);
                        }
                    }
                }
                //dgrKHDongThung.MainView = GetBandGridViewAmount_grd1(_dtData);
                //BandedGridView mainView = (BandedGridView)dgrKHDongThung.MainView;

                //var dataOld = (DataTable)dgrKHDongThung.DataSource;
                foreach(DataRow dr in _dtData.Rows)
                {
                    UInt32 slSP = 0;
                    foreach (DataColumn dc in _dtData.Columns)
                    {
                        var colName = dc.ColumnName;
                        if (!colName.Contains("@")) continue;
                        slSP += dr[dc].ToString() != "" ? Convert.ToUInt32(dr[dc.ColumnName]) : 0;
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
                DataTable tblSave = CreateTblSave();
                DateTime dtNow = DateTime.Now;
                int TuThung_Notdeca = 0;
                foreach (DataRow dr in _dtData.Rows)
                {
                    var _poid = dr["POID"].ToString();
                    var _po = dr["PO"].ToString();
                    var _sizeType = dr["DauSize"].ToString();
                    var _sizeTypeID = dr["DauSizeID"].ToString();
                    var _colorID = dr["ColorID"].ToString();
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

                                drNewRow["SoLuongSP"] = Convert.ToInt32(dr[colName].ToString());
                                drNewRow["IsDongThung"] = false;
                                drNewRow["NgayDongThung"] = dtNow;
                                drNewRow["NgayNhapKho"] = dtNow;
                                drNewRow["QRCode"] = "";
                                drNewRow["IsNhapKho"] = false;
                                drNewRow["KyHieu"] = string.Format("{0} -> {1}", TuThung, DenThung);
                                drNewRow["Chon"] = false;
                                drNewRow["IsThungLe"] = dr["IsThungLe"];
                                drNewRow["KieuLap"] = "0";
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
        private void BtnSave()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            SaveRows();
        }
      
    }
}
