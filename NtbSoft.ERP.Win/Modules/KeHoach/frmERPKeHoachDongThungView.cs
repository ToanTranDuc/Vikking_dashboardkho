using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;


namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmERPKeHoachDongThungView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        private string _maPKL = string.Empty, _madh = string.Empty, _madh_Detail = string.Empty, _madhDisplay = string.Empty, _maHang = string.Empty, _tenHang = string.Empty, _maDVSX = string.Empty,
            _maLenh = string.Empty, _dotSX = string.Empty, _po = string.Empty, _poid = string.Empty, _poid_detail = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty;
        private string _maDVSXL = string.Empty, _maDauSizeL = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private bool cbxAllPO, cbxPOInSeamA, cbxCodeKLA, cbxCodeSizeA, cbxArtA, cbxDVSXA, cbxKg_ThungA, cbxBarCode;
        List<string> _listSize = new List<string>();
        DataTable _dtInit = new DataTable();
        DataTable _dtDVSX = new DataTable();
        Object _lstDauSize = new Object();
        Object _lstColor = new Object();
        DataTable _dtSize = new DataTable();
        DataTable _dtCTDonHang = new DataTable();
        DataTable dtDataPiVot = new DataTable();
        DataTable dtQuiCach = new DataTable();
        KeyDownControlHandler keyDownControlHandler;
        int sttThung_CreateNew = 0;
        public frmERPKeHoachDongThungView()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Init();
            LoadDonHang();
            CheckPerminsion();
            KHDongThungLib.InitQuiCach(repoQuiCach);
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            checkPoA.EditValueChanged += CheckPoA_EditValueChanged;
            cbxPOInSeam.EditValueChanged += CbxPOInSeam_EditValueChanged;
            cbxCodeKL.EditValueChanged += CbxCodeKL_EditValueChanged;
            cbxCodeSize.EditValueChanged += CbxCodeSize_EditValueChanged;
            cbxArt.EditValueChanged += CbxArt_EditValueChanged;
            cbxDVSX.EditValueChanged += CbxDVSX_EditValueChanged;
            cbxKg_Thung.EditValueChanged += CbxKg_Thung_EditValueChanged;
            repocbxBarcode.EditValueChanged += RepocbxBarcode_EditValueChanged;
        }


        #region Check edit excel
        private void CbxDVSX_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxDVSXA = cb.Checked;
            if (cbxDVSXA)
            {
                BandDVSX.Visible = false;
                BandDotSX.Visible = false;
            }
            else
            {
                BandDVSX.Visible = true;
                BandDotSX.Visible = true;
            }
        }

        private void CbxArt_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxArtA = cb.Checked;
            if (cbxArtA) bandArt.Visible = true;
            else bandArt.Visible = false;
        }

        private void CbxCodeSize_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxCodeSizeA = cb.Checked;
        }

        private void CbxCodeKL_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxCodeKLA = cb.Checked;
        }

        private void CbxPOInSeam_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxPOInSeamA = cb.Checked;
        }

        private void CheckPoA_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxAllPO = cb.Checked;
        }
        private void RepocbxBarcode_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxBarCode = cb.Checked;
            if (cbxBarCode)
            {
                BandBarCode.Visible = true;
            }
            else
            {
                BandBarCode.Visible = false;
            }
        }

        private void CbxKg_Thung_EditValueChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            cbxKg_ThungA = cb.Checked;
            if (cbxKg_ThungA)
            {
                BandNW.Visible = true;
                BandGW.Visible = true;
            }
            else
            {
                BandNW.Visible = false;
                BandGW.Visible = false;
            }
        }
        #endregion

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtThemPODot, _allowAdd, ActionType.Add);
            AddActionControl(_lstActionControl, BtnThemPOPCB, _allowAdd, ActionType.AddPOPCB);
            AddActionControl(_lstActionControl, BtnSua, _allowEdit, ActionType.Edit);
            AddActionControl(_lstActionControl, BtnLuu, true, ActionType.Save);
            AddActionControl(_lstActionControl, BtnXuatExCel, true, ActionType.ExCel);
            AddActionControl(_lstActionControl, BtnNapLai, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, BtnGopThung, true, ActionType.GopThung);
            AddActionControl(_lstActionControl, BtnXoa, _allowDelete, ActionType.Delete);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserModuleEntity> List = JsonConvert.DeserializeObject<List<SystemUserModuleEntity>>(json);
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                btnLapKeHoach.Enabled = false;
            }
            if (!_allowEdit)
            {
                btnAddorEdit.Enabled = false;
                btSave.Enabled = false;
            }
            if (!_allowDelete)
                btnDelete.Enabled = false;

            string urlDuyet = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName);
            string jsonT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDuyet); }).Result;
            var dtDuyetT = JsonConvert.DeserializeObject<DataTable>(jsonT);
            var dr = dtDuyetT.AsEnumerable().Where(x => x["ModuleID"].ToString() == "M.08.01.00").FirstOrDefault();
            if (dr is null) return;
            if ((bool)dr["AllowDuyet"])
            {
                btnDuyet.Enabled = true;
            }
            else
            {
                btnDuyet.Enabled = false;
            }
        }
        private void Init()
        {
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.ValueMember = "MaPKL";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";

            searchLookUpEditDonHang.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEditDonHang.Properties.ValueMember = "MaDH";
            searchLookUpEditDonHang.Properties.NullText = "[Chọn đơn hàng]";
        }
        private void LoadDonHang()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDH&MaDH=DH00000064&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var _dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDonHang.Properties.DataSource = _dtDonHang;
        }
        private void LoadDonHangSMS()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDH_SMS&MaDH=DH00000064&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var _dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDonHang.Properties.DataSource = _dtDonHang;
        }
        private void LoadCTDonHang()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetCTDH&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            _dtCTDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            if (_dtCTDonHang is null) return;
            var rowfocus = grvDonHang.FocusedRowHandle;

            grcDonHang.DataSource = _dtCTDonHang;
            grcDonHang.RefreshDataSource();
            if (rowfocus == 0) LoadDataWhenChangeDH(true);
            //grvDonHang.FocusedRowHandle = rowfocus;

            checkEditAll.Checked = true;
            if (_dtCTDonHang.Rows.Count == 0)
            {
                dgrKHDongThung.DataSource = new DataTable();
                dgcTong.DataSource = new DataTable();
                dgrMauSize.DataSource = new DataTable();
            }
        }
        private void LoadKHDongThung()
        {
            if (_maDVSX == "" && !cbxSMS.Checked)
            {
                dgrKHDongThung.DataSource = new DataTable();
                dgcTong.DataSource = new DataTable();
                return;
            }
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _maPKL = searchLookUpEdit_MaPKL.EditValue == null ? "" : searchLookUpEdit_MaPKL.EditValue.ToString().Split('@')[0];
            var _maDVSXTemp = _maDVSX == "" ? "" : _maDVSX.Substring(1, _maDVSX.Length - 2) + '@' + _maLenh;
            if (checkEditAll.Checked) _maDVSXTemp = "All";
            _maDVSX1 = _maDVSXTemp;
            var tempPO = cbxSMS.Checked ? "A" : _poid;
            string action = cbxSMS.Checked ? "GetPivotKHDongThung_SMS" : "GetPivotKHDongThung";
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={_madh}&MaDVSX={_maDVSXTemp}&DotSX={_dotSX}&POID={tempPO}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtDataPiVot = JsonConvert.DeserializeObject<DataTable>(json);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            DataTable tbl = new DataTable();
            tbl = dtDataPiVot;
            KHDongThungLib.ProcessSttTrung(tbl);
            if (tbl == null || tbl.Rows.Count == 0)
                dgrKHDongThung.DataSource = new DataTable();
            else
            {
                DataTable dt = new DataTable();

                if (!checkEditAll.Checked && !cbxSMS.Checked)
                {
                    var tempdt = tbl.AsEnumerable().Where(x => x["MaDVSX"].ToString() + '@' + x["MaLenh"].ToString() == _maDVSXTemp);
                    tbl = tempdt.Count() == 0 ? new DataTable() : tempdt.CopyToDataTable();
                }
                CreateBandForSize(tbl);
                tbl = KHDongThungLib.CalculatorTB(tbl);
                dgrKHDongThung.DataSource = tbl;
                DataTable processedData = KHDongThungLib.sumToTalPCS(tbl);
                KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
                dgcTong.DataSource = processedData;
                dgrKHDongThung.RefreshDataSource();
                KHDongThungLib.AllowVieworNotPackV2(tbl, BandMaHang, BandDot);
                KHDongThungLib.AllowVieworNotPack(tbl, BandPCB, BandPack, BandStore);
            }
            if (tbl.Rows.Count == 0) return;
            sttThung_CreateNew = Convert.ToInt32(tbl.Rows[tbl.Rows.Count - 1]["SttThung"]) + 1;
            //DataTable tbl = GetPivotKHDongThung();
            //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
            //btnLapKeHoachNhanh.Enabled = true;
        }
        string _maDVSX1 = string.Empty;
        private void FilerKHDongThung()
        {
            if (checkEditAll.Checked || _maDVSX == "")
            {
                return;
            }

            var _maDVSXTemp = _maDVSX.Substring(1, _maDVSX.Length - 2) + '@' + _maLenh;
            if (checkEditAll.Checked) _maDVSXTemp = "All";
            _maDVSX1 = _maDVSXTemp;
            var tempdt = dtDataPiVot.AsEnumerable().Where(x => x["MaDVSX"].ToString() + '@' + x["MaLenh"].ToString() == _maDVSXTemp);
            DataTable dtData = tempdt.Count() == 0 ? new DataTable() : tempdt.CopyToDataTable();

            //CreateBandForSize(dtData);
            dgrKHDongThung.DataSource = dtData;
            DataTable processedData = KHDongThungLib.sumToTalPCS(dtData);
            KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
            dgcTong.DataSource = processedData;
            dgrKHDongThung.RefreshDataSource();
        }
        private void ProcessDataBeforeSave()
        {
            if (checkEditAll.Checked || _maDVSX == "")
            {
                dtDataPiVot = dgrKHDongThung.DataSource as DataTable;
                return;
            }
            var dataEdit = dgrKHDongThung.DataSource as DataTable;
            var drFirst = dtDataPiVot.AsEnumerable().Where(x => x["MaLenh"].ToString() == dataEdit.Rows[0]["MaLenh"].ToString()).FirstOrDefault();
            var index = dtDataPiVot.Rows.IndexOf(drFirst);
            var dtTemp = dtDataPiVot.AsEnumerable().Where(x => x["MaLenh"].ToString() != dataEdit.Rows[0]["MaLenh"].ToString());
            if (dtTemp.Count() > 0) dtDataPiVot = dtTemp.CopyToDataTable();
            else
            {
                dtDataPiVot = dataEdit;
                return;
            };
            //dtDataPiVot = dtTemp.Count() > 0 ? dtTemp.CopyToDataTable() : dtDataPiVot;


            foreach (DataRow dr in dataEdit.Rows)
            {
                var drNew = dtDataPiVot.NewRow();
                KHDongThungLib.CopyDataRow(dr, drNew);
                dtDataPiVot.Rows.InsertAt(drNew, index);
                index++;
            }
            KHDongThungLib.TinhToanLaiKhiXoa(dtDataPiVot);
        }
        private void SaveKHDT(DataTable tblPivot)
        {
            try
            {
                ProcessDataBeforeSave();
                DataTable tblSave = KHDongThungLib.CreateTblSave();
                int TuThung_Notdeca = 0, _valTuThung = 1, _valDenThung = 0;
                DateTime dtNow = DateTime.Now;
                int TuThungOld = 0;
                int SttThungOld = 0;
                int SttThung_temp = 0;
                string _colorIDOld = "";
                if (dtDataPiVot.Rows.Count == 0) return;
                string kieulap = dtDataPiVot.Rows[0]["KieuLapPCB"].ToString();
                string kieulapNew = dtDataPiVot.Rows[0]["KieuLap"].ToString();
                bool checkKTheoThuTu = false;
                if (kieulap != "2" && kieulap != "3")
                {
                    if (kieulapNew == "0" && dtDataPiVot.Rows[0]["Stt_size"].ToString() == "0") checkKTheoThuTu = KHDongThungLib.CheckKieuLapKhongTheoThuTu(dtDataPiVot);
                    foreach (DataRow dr in dtDataPiVot.Rows)
                    {
                        if (checkKTheoThuTu)
                        {
                            if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                            {
                                TuThung_Notdeca = 0;
                            }
                            else TuThung_Notdeca = 1;
                        }
                        int slThung = Convert.ToInt32(dr["SLThung"]);
                        int TuThung = 0, DenThung = 0;
                        TuThung = Convert.ToInt32(dr["TuThung"]);
                        DenThung = Convert.ToInt32(dr["DenThung"]);
                        var _colorID = dr["ColorID"].ToString();
                        if (_colorIDOld != _colorID)
                        {
                            SttThung_temp = 0;
                            _colorIDOld = _colorID;
                        }
                        string _strSizeID = string.Empty, _strKyHieu_1 = string.Empty, _strKyHieu_2 = string.Empty;

                        foreach (DataColumn dc in dtDataPiVot.Columns)
                        {
                            string colName = dc.ColumnName;
                            if (!colName.Contains('@')) continue;
                            string _sizeID = colName.Split('@')[1];
                            string _size = colName.Split('@')[0];
                            if (dr[colName].ToString() == "" || dr[colName].ToString() == "0") continue;
                            int index = 0;
                            int SttThungDeCat_Start = 0;
                            for (int i = 0; i < slThung; i++)
                            {
                                _strKyHieu_2 = string.Format("{0} -> {1}", TuThung, DenThung);
                                DataRow drNewRow = tblSave.NewRow();
                                drNewRow["ID"] = 0;
                                drNewRow["MaPKL"] = searchLookUpEdit_MaPKL.EditValue.ToString().Split('@')[0]; ;
                                drNewRow["MaDH"] = _madh;
                                drNewRow["MaDVSX"] = dr["MaDVSX"];
                                drNewRow["DotSX"] = dr["DotSX"];
                                drNewRow["MaLenh"] = dr["MaLenh"];
                                drNewRow["MaHang"] = dr["MaHang"];// _maHang;
                                drNewRow["POID"] = dr["POID"];// _poid;
                                drNewRow["MaDH_XH"] = dr["POID_G"];
                                drNewRow["POID_XH"] = dr["PO_G"];
                                drNewRow["PO"] = dr["PO"];
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
                                drNewRow["TrongLuong"] = dr["TrongLuong"];
                                drNewRow["KhoiLuong"] = dr["KhoiLuong"];
                                drNewRow["SoLuongThung"] = slThung;
                                drNewRow["TuThung"] = TuThung;
                                drNewRow["DenThung"] = DenThung;

                                if (dr["Stt_size"].ToString() != "" && Convert.ToInt32(dr["Stt_size"]) == 1)
                                {
                                    drNewRow["SttThung_decat"] = TuThung + index;
                                    if (_strSizeID != _sizeID && _strKyHieu_1 == _strKyHieu_2)
                                    {
                                        TuThung_Notdeca = TuThung_Notdeca;
                                        _strSizeID = _sizeID;
                                    }
                                    else if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                                    {
                                        TuThung_Notdeca++;
                                        index++;
                                        SttThung_temp++;
                                    }
                                    drNewRow["SttThung"] = TuThung_Notdeca;
                                    //if (SttThungOld != Convert.ToInt32(drNewRow["SttTuThung"])) index++;
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
                                //drNewRow["SttThung_temp"] = index == 0 ? 1 : index;
                                drNewRow["SttThung"] = checkKTheoThuTu ? (TuThung + TuThung_Notdeca - 1) : TuThung_Notdeca; ;
                                if (SttThungDeCat_Start == 0) SttThungDeCat_Start = Convert.ToInt32(drNewRow["SttThung"]);
                                drNewRow["SttThung_start"] = SttThungDeCat_Start;

                                drNewRow["SoLuongSP"] = Convert.ToInt32(dr[colName].ToString());
                                drNewRow["IsDongThung"] = false;
                                drNewRow["NgayDongThung"] = dtNow;
                                drNewRow["QRCode"] = "";
                                drNewRow["IsNhapKho"] = false;
                                drNewRow["KyHieu"] = dr["KyHieu"];
                                drNewRow["Chon"] = false;
                                drNewRow["IsThungLe"] = dr["IsThungLe"];
                                drNewRow["NVien"] = GlobleData.UserName;
                                drNewRow["PCB_Pack"] = dr["PCB_Pack"];
                                drNewRow["Pack_Ctn"] = dr["Pack_Ctn"];
                                drNewRow["QRCode"] = dr["StylePackNo"];
                                drNewRow["DeptNo"] = dr["DeptNo"];
                                drNewRow["Destination"] = dr["Destination"];
                                drNewRow["DeliveryTo"] = dr["DeliveryTo"];
                                drNewRow["Terms"] = dr["Terms"];
                                drNewRow["CountryOfOrigin"] = dr["CountryOfOrigin"];
                                drNewRow["KieuLap"] = dr["KieuLap"];
                                drNewRow["KieuLapPCB"] = dr["KieuLapPCB"];
                                tblSave.Rows.Add(drNewRow);
                            }
                            _strSizeID = _sizeID;
                            _strKyHieu_1 = _strKyHieu_2;
                            TuThungOld = Convert.ToInt32(dr["TuThung"]);
                            SttThungOld = Convert.ToInt32(dr["SttThung"]);
                        }
                    }
                }
                else
                {
                    int index1 = 0;
                    foreach (DataRow dr in dtDataPiVot.Rows)
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
                        int index = 1;
                        int SttThungDeCat_Start = 0;
                        for (int i = 0; i < slThung; i++)
                        {
                            for (int z = 0; z < dtDataPiVot.Columns.Count; z++)
                            {
                                string[] arrName = dtDataPiVot.Columns[z].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arrName.Length < 2) continue;
                                var sizeName = dtDataPiVot.Columns[z].ColumnName;
                                if (dr[sizeName].ToString() == "" || dr[sizeName].ToString() == "0") continue;
                                _sizeID = arrName[1];
                                _size = arrName[0];
                                //for (int i = 0; i < slThung; i++)
                                //{
                                DataRow drNewRow = tblSave.NewRow();
                                drNewRow["ID"] = 0;
                                drNewRow["MaPKL"] = searchLookUpEdit_MaPKL.EditValue.ToString().Split('@')[0];
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
                                drNewRow["PCB_Pack"] = dr["PCB_Pack"];
                                drNewRow["Pack_Ctn"] = dr["Pack_Ctn"];
                                drNewRow["DeptNo"] = dr["DeptNo"];
                                drNewRow["StyleName"] = dr["StyleName"];
                                drNewRow["QRCode"] = dr["StylePackNo"];
                                drNewRow["Destination"] = dr["Destination"];
                                drNewRow["DeliveryTo"] = dr["DeliveryTo"];
                                drNewRow["Terms"] = dr["Terms"];
                                drNewRow["CountryOfOrigin"] = dr["CountryOfOrigin"];
                                //drNewRow["SttThung"] = TuThung + index1;
                                if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                                {
                                    TuThung_Notdeca++;
                                    index++;
                                    SttThung_temp++;
                                }


                                drNewRow["SttThung_decat"] = 0;
                                if (SttThungOld != Convert.ToInt32(dr["SttThung"]))
                                {
                                    TuThung_Notdeca++;
                                    index++;
                                    SttThung_temp++;
                                }

                                drNewRow["SttThung_LapMau"] = SttThung_temp == 0 ? 1 : SttThung_temp;
                                drNewRow["SttThung_temp"] = index == 0 ? 1 : index;
                                drNewRow["SttThung"] = TuThung_Notdeca;
                                if (SttThungDeCat_Start == 0) SttThungDeCat_Start = Convert.ToInt32(drNewRow["SttThung"]);
                                drNewRow["SttThung_start"] = SttThungDeCat_Start;

                                drNewRow["SoLuongSP"] = Convert.ToInt32(dr[sizeName].ToString());
                                drNewRow["IsDongThung"] = false;
                                drNewRow["NgayDongThung"] = dtNow;
                                drNewRow["NgayNhapKho"] = dtNow;
                                drNewRow["IsNhapKho"] = false;
                                //drNewRow["KyHieu"] = string.Format("{0} -> {1}", TuThung, DenThung);
                                drNewRow["Chon"] = false;
                                drNewRow["IsThungLe"] = dr["IsThungLe"]; //-dr["IsThungLe"];
                                drNewRow["NVien"] = GlobleData.UserName;
                                drNewRow["KieuLap"] = dr["KieuLap"];
                                drNewRow["KieuLapPCB"] = dr["KieuLapPCB"];
                                drNewRow["Store"] = dr["Store"];
                                tblSave.Rows.Add(drNewRow);
                                // }

                                SttThungOld = Convert.ToInt32(dr["SttThung"]);

                                SttThung_temp++;
                                //}

                            }
                            index++;
                            index1++;
                        }
                    }
                }

                //List<ErpPCBEntity> lst = JsonConvert.DeserializeObject<List<ErpPCBEntity>>(json);
                string url = string.Format("{0}", URL + "KeHoachDongThung/Post?action=InsertKHDT");
                //return;
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;

                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadKHDongThung();
                    //string mss_PCB = await _service_Khdt_pcb.Post(URL + ResourceURL.UrlErpKHDongThung_PCB + "/Post", _lstSave);
                    ////string url = string.Format("{0}?maLenh={1}", URL + ResourceURL.UrlErpLenhSXPODinhMuc + "/AllowPack", txtLenhSX.Text);
                    ////string msP = Task.Run(async () => { return await _serviceLenhSXPODM.SPODMAllowWork(url); }).Result;
                    //if (string.Compare(mss_PCB, "True") != 0)
                    //{
                    //    XtraMessageBox.Show(mss_PCB, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void GetDotPKL()
        {
            var tempPO = cbxSMS.Checked ? "Para" : _poid;
            string action = cbxSMS.Checked ? "GetDotLapPKL_SMS" : "GetDotLapPKL";
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={_madh}&MaDVSX={_maDVSX}&DotSX=${_dotSX}&POID={tempPO}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL.Properties.DataSource = tbl;
            if (tbl.Rows.Count != 0)
            {
                searchLookUpEdit_MaPKL.EditValue = null;
                searchLookUpEdit_MaPKL.EditValue = tbl.Rows[0]["MaPKL"];

            }
            else
            {
                btnDuyet.EditValue = false;
                btnLock.EditValue = false;
                dgrKHDongThung.DataSource = new DataTable();
                dgcTong.DataSource = new DataTable();
                return;
            }
            _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
            _poid = tbl.AsEnumerable().Where(x => x["MaPKL"].ToString() == _maPKL).FirstOrDefault()?["POID"].ToString();
        }
        private void GopThung()
        {
            this.ActiveControl = grcDonHang;
            var data = dgrKHDongThung.DataSource as DataTable;
            // ProcessDataBeforeSave();
            //if (data is null || data.Rows.Count == 0) return;
            //var drCheck = data.Rows[0]["Stt_Size"].ToString();
            //va
            var dataT = KHDongThungLib.GopThung(data);
            if (dataT.Rows.Count == 0) return;
            int rowfocus = bandedGridViewKHDT.FocusedRowHandle;
            dgrKHDongThung.DataSource = dataT;
            bandedGridViewKHDT.FocusedRowHandle = rowfocus;
            bandedGridViewKHDT.RefreshData();
        }
        private void TinhToanLaiKhiXoa(DataTable tblPivot)
        {
            // DataTable tblPivot = dgrKHDongThung.DataSource as DataTable;
            int sttThungCur = 0;
            int tuThungOld = 0;
            int denThungOld = 0;
            bool flagFirst = false;
            foreach (DataRow drKH in tblPivot.Rows)
            {
                if (drKH["Stt_Size"].ToString() == "1")
                {

                }
                else
                {
                    int tuThung = 0, denThung = 0;
                    if (Convert.ToInt16(drKH["TuThung"]) == sttThungCur && flagFirst)
                    {
                        drKH["TuThung"] = tuThungOld;
                        drKH["DenThung"] = denThungOld;
                        continue;
                    }
                    tuThung = sttThungCur + 1;
                    denThung = Convert.ToInt32(drKH["SLThung"]) + sttThungCur;
                    sttThungCur = denThung;

                    drKH["TuThung"] = tuThung;
                    drKH["DenThung"] = denThung;
                    tuThungOld = tuThung;
                    denThungOld = denThung;
                    flagFirst = true;
                }

            }
            //SaveKHDT(tblPivot);
        }
        private void CopyDataRow(DataRow sourceRow, DataRow destinationRow)
        {
            // Loop through columns and copy data
            foreach (DataColumn column in sourceRow.Table.Columns)
            {
                destinationRow[column.ColumnName] = sourceRow[column.ColumnName];
            }
        }
        private void CreateBandForSize(DataTable dt)
        {
            try
            {
                ClearDataBandAndCol();
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
                    col.Width = 40;
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
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
                lst = bandedGridViewKHDT.Columns.Where(x => x.FieldName.Contains(@"Size@")).ToList();
                if (lst != null && lst.Count > 0)
                    foreach (GridColumn gc in lst) bandedGridViewKHDT.Columns.Remove(gc);
            }
            catch (Exception ex) { };
        }

        #region Event
        private void grvDonHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            LoadDataWhenChangeDH();
        }
        private void LoadDataWhenChangeDH(bool flagReload = false)
        {
            var dr = grvDonHang.GetFocusedDataRow() as DataRow;
            if (dr is null) return;
            _maDVSX = "'" + dr["MaDVSX"].ToString() + "'";
            _maLenh = dr["MaLenh"].ToString();
            _dotSX = dr["DotSX"].ToString();
            if (_maHang != dr["MaHang"].ToString())
            {
                dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, dr["MaHang"].ToString(), _clientExtension);
            }
            _maHang = dr["MaHang"].ToString();
            _tenHang = dr["TenHang"].ToString();
            _madh_Detail = dr["MaDH"].ToString();
            _poid_detail = dr["POID"].ToString();
            //var dh = dr["MaDH"].ToString();
            if (_madh != dr["MaDH"].ToString() || _poid != dr["POID"].ToString() || flagReload)
            {
                //if (_madh != dr["MaDH"].ToString())
                //{

                //}

                if (searchLookUpEditDonHang.EditValue is null) return;
                _madh = searchLookUpEditDonHang.EditValue.ToString();
                _madhDisplay = dr["GopDH"].ToString();
                _poid = dr["POID"].ToString();
                _po = dr["PO"].ToString();
                //LoadDVSX();
                GetDotPKL();
            }
            else FilerKHDongThung();

            GetDataSize();
        }
        private void bandedGridViewKHDT_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;

            DXMenuItem menuDelete = new DXMenuItem();
            menuDelete.Caption = "Xóa";
            menuDelete.Click += MenuDelete_Click;
            DXMenuItem menuCopyandCreateN = new DXMenuItem();
            menuCopyandCreateN.Caption = "Copy và tạo dòng mới phía dưới";
            menuCopyandCreateN.Click += MenuCopyandCreateN_Click;
            DXMenuItem menuCopyandCreate = new DXMenuItem();
            menuCopyandCreate.Caption = "Copy và tạo dòng mới";
            menuCopyandCreate.Click += MenuCopyandCreate_Click;
            e.Menu.Items.Add(menuCopyandCreate);
            e.Menu.Items.Add(menuCopyandCreateN);
            e.Menu.Items.Add(menuDelete);

        }
        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }
        private void dgwTong_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            KHDongThungLib.CustomColumnDisplay(e);
        }
        private void bandedGridViewKHDT_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var dt = dgrKHDongThung.DataSource as DataTable;
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
                        //CheckSTTThungInGrid(TuThung, DenThung);
                    }
                    else
                    {
                        drFocus["SLThung"] = 0;
                        //btSave.Enabled = false;
                    }

                    int sumSL = 0;
                    var dtThungTrung = dt.AsEnumerable().Where(x => x["SttThung"].ToString() == drFocus["SttThung"].ToString());
                    foreach (var drTrung in dtThungTrung)
                    {
                        foreach (DataColumn dc in drTrung.Table.Columns)
                        {
                            if (!dc.ColumnName.Contains('@')) continue;
                            sumSL += drTrung[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drTrung[dc.ColumnName]);
                        }
                    }
                    foreach (var drTrung in dtThungTrung)
                    {
                        drTrung["SoLuong"] = sumSL;
                    }
                    //drFocus["SoLuong"] = sumSL;
                    if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
                        drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
                    KHDongThungLib.TinhToanLaiKhiXoa(dt);
                }
                else if (e.Column.FieldName.Contains("@") || e.Column.FieldName.Contains("SLThung"))
                {
                    int sumSL = 0;
                    foreach (DataColumn dc in drFocus.Table.Columns)
                    {
                        if (!dc.ColumnName.Contains('@')) continue;
                        sumSL += drFocus[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drFocus[dc.ColumnName]);
                    }

                    var SttThung = drFocus["SttThung"];

                    var index = dt.Rows.IndexOf(drFocus);
                    var dtTrungTemp = dt.AsEnumerable().Where((x =>
                        x["SttThung"].ToString() == SttThung.ToString() && dt.Rows.IndexOf(x) != index));
                    foreach (DataRow dr in dtTrungTemp)
                    {
                        foreach (DataColumn dc in dr.Table.Columns)
                        {
                            if (!dc.ColumnName.Contains('@')) continue;
                            sumSL += dr[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(dr[dc.ColumnName]);
                        }
                    }
                    //var dtTrung = dtTrungTemp.Count() > 0 ? dtTrungTemp.CopyToDataTable()

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["SttThung"].ToString() == SttThung.ToString())
                        {
                            dr["SoLuong"] = sumSL;
                        }
                    }

                    if (drFocus["PCB_Pack"].ToString() != "0" && drFocus["PCB_Pack"].ToString() != "")
                    {
                        drFocus["PCB_Pack"] = sumSL;
                        drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["Pack_Ctn"]) * Convert.ToInt32(drFocus["SLThung"]);
                    }
                    else if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["SttThung"].ToString() == SttThung.ToString())
                            {
                                dr["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
                            }
                        }
                    }

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
                    KHDongThungLib.ProcessChangeNW_GW(dt, drFocus);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void bandedGridViewKHDT_ShowingEditor(object sender, CancelEventArgs e)
        {
            KHDongThungLib.EventShowingEditor(bandedGridViewKHDT, e, true);
        }
        private void bandedGridViewKHDT_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            e.Handled = KHDongThungLib.MergeAllowChangeValue(bandedGridViewKHDT, e);
        }
        private void bandedGridViewKHDT_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = (bool)bandedGridViewKHDT.GetRowCellValue(e.RowHandle, "IsThungLe");
            if (IsThungLe) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }
        private void btnGopThung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GopThung();
        }
        private void MenuCopyandCreateN_Click(object sender, EventArgs e)
        {
            CopyData(true);
        }
        private void MenuCopyandCreate_Click(object sender, EventArgs e)
        {
            CopyData(false);
        }
        private void CopyData(bool flagInsert)
        {
            try
            {
                var drCopy = bandedGridViewKHDT.GetFocusedDataRow();
                DataTable tbl = dgrKHDongThung.DataSource as DataTable;
                if (tbl.Columns.Count == 0) return;
                int index = tbl.Rows.IndexOf(drCopy);

                DataRow drAdd = tbl.NewRow();
                KHDongThungLib.CopyDataRow(drCopy, drAdd);
                drAdd["ID"] = 0;
                drAdd["TuThung"] = 0;
                drAdd["DenThung"] = 0;
                drAdd["SoLuong"] = 0;
                drAdd["SLThung"] = 0;
                drAdd["TotalPiece"] = 0;
                drAdd["TrongLuong"] = 0;
                drAdd["KhoiLuong"] = 0;
                drAdd["TrongLuongA"] = 0;
                drAdd["IsSave"] = false;
                drAdd["IsThungLe"] = flagInsert;
                drAdd["Chon"] = false;
                drAdd["SttThung"] = sttThung_CreateNew;


                //drAdd["ID"] = 0;
                //drAdd["POID"] = _poid;
                //drAdd["PO"] = _po;
                //drAdd["MaHang"] = _maHang;
                //drAdd["MaDVSX"] = drCopy["MaDVSX"];
                //drAdd["TenDVSX"] = drCopy["TenDVSX"];
                //drAdd["MaLenh"] = drCopy["MaLenh"];
                //drAdd["DotSX"] = drCopy["DotSX"];
                //drAdd["DauSizeID"] = drCopy["DauSizeID"];
                //drAdd["DauSize"] = drCopy["DauSize"];
                //drAdd["ColorID"] = drCopy["ColorID"];
                //drAdd["TenMau"] = drCopy["TenMau"];
                //drAdd["SoLuong"] = 0;
                //drAdd["SLThung"] = 0;
                //drAdd["TotalPiece"] = 0;
                //drAdd["TrongLuong"] = 0;
                //drAdd["KhoiLuong"] = 0;
                //drAdd["TrongLuongA"] = 0;
                //drAdd["KyHieu"] = drCopy["KyHieu"];
                //drAdd["IsSave"] = false;
                //drAdd["IsThungLe"] = flagInsert;
                //drAdd["Chon"] = false;
                //drAdd["KyHieu"] = drCopy["KyHieu"];
                ////var sttThung = Convert.ToInt32(tbl.Rows[tbl.Rows.Count - 1]["SttThung"]);
                //drAdd["SttThung"] = sttThung_CreateNew;
                //drAdd["KieuLap"] = drCopy["KieuLap"];
                //drAdd["KieuLapPCB"] = drCopy["KieuLapPCB"];
                //drAdd["Stt_size"] = drCopy["Stt_size"];

                if (flagInsert) tbl.Rows.InsertAt(drAdd, index + 1);
                else tbl.Rows.Add(drAdd);
                dgrKHDongThung.RefreshDataSource();
                if (flagInsert) bandedGridViewKHDT.FocusedRowHandle = index + 1;
                else bandedGridViewKHDT.FocusedRowHandle = tbl.Rows.Count - 1;
                sttThung_CreateNew++;
            }
            catch (Exception ex) { }

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
                    int index = tblPivot.Rows.IndexOf(drRemove);
                    tblPivot.Rows.RemoveAt(index);
                    dgrKHDongThung.RefreshDataSource();
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
                        KHDongThungLib.TinhToanLaiKhiXoa(tblPivot);
                        SaveKHDT(tblPivot);
                        //if (drCheckSave != null) SaveKHDT();
                        //string key = string.Format("{0}_{1}_{2}_{3}", drRemove["SPOID"], drRemove["ColorID"], drRemove["TuThung"], drRemove["DenThung"]);
                        //string url = string.Format("{0}?key={1}&&stt={2}", URL + ResourceURL.UrlErpNhanThanhPham + "/DeleteRowKH", key, drRemove["del"]);
                        //string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        //if (result.ToLower() == "true")
                        //{
                        //    clsWaitForm.ShowSuccessForm(this, 2000);
                        //    GetListSizeOfColor();
                        //    TinhToanLaiThungKhiXoa();
                        //    bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
                        //}
                    }

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void btnPKLPO_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnThemPOPCB();
        }
        private void btnAddorEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnSua();
        }
        private void btnLapKeHoach_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtThemPODot();
        }
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnXoa();
        }
        private void searchLookUpEditDonHang_EditValueChanged(object sender, EventArgs e)
        {
            var value = searchLookUpEditDonHang.EditValue;
            if (value is null) return;
            _madh = value.ToString();
            if (cbxSMS.Checked)
            {

                GetDotPKL();
                //return;
            }
            LoadCTDonHang();
        }

        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_MaPKL.EditValue is null) return;
            DataTable tbl = searchLookUpEdit_MaPKL.Properties.DataSource as DataTable;
            _poid = tbl.AsEnumerable().Where(x => x["MaPKL"].ToString() == searchLookUpEdit_MaPKL.EditValue.ToString()).FirstOrDefault()?["POID"].ToString();
            LoadKHDongThung();
            GetData_Description();
        }

        private void grcDonHang_DataSourceChanged(object sender, EventArgs e)
        {

        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (cbxSMS.Checked) return;
            dtQuiCach = KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension);
            LoadCTDonHang();
        }

        private void frmERPKeHoachDongThungView_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            //{
            //    // Do what you want here;
            //    this.ActiveControl = grcDonHang;
            //    DataTable dt = dgrKHDongThung.DataSource as DataTable;
            //    SaveKHDT(dt);
            //    e.SuppressKeyPress = true;  // Stops other controls on the form receiving event.
            //}
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnLuu();
        }

        #endregion

        private void checkEditAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadKHDongThung();
        }
        private void GetDataSize()
        {
            var _maDVSXTemp = _maDVSX.Substring(1, _maDVSX.Length - 2);
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDataSize&MaDH={_madh_Detail}&MaDVSX={_maDVSXTemp}&DotSX={_maLenh}&POID={_poid_detail}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            dgrMauSize.DataSource = tbl;
        }
        private bool CheckExistBand(string size)
        {
            GridBand gbCheck = gbSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
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

        private void repoQuiCach_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit sr = sender as SearchLookUpEdit;
            if (sr.EditValue is null) return;
            var dt = dgrKHDongThung.DataSource as DataTable;
            var drfocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drfocus is null) return;

            var quicach = sr.EditValue is null ? "" : sr.EditValue.ToString();
            var dtTemp = dt.AsEnumerable().Where(x => x["SttThung"].ToString() == drfocus["SttThung"].ToString());

            //var str = drfocus["KyHieu"].ToString();

            //var dtTemp = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).CopyToDataTable();
            var TrongLuong = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).FirstOrDefault()["TLThung"].ToString();
            var TotalPice = Convert.ToInt32(drfocus["TotalPiece"]);
            var SLThung = Convert.ToInt32(drfocus["SLThung"]);
            var KhoiLuong = Convert.ToDouble(drfocus["KhoiLuong"]);
            var value = Math.Round(KhoiLuong + SLThung * Convert.ToDouble(TrongLuong), 2);
            if (dtTemp.Count() > 0)
            {
                //var dtTrung = dtTemp.CopyToDataTable();
                foreach (var dr1 in dtTemp)
                {
                    dr1["KyHieu"] = quicach;
                    dr1["TrongLuong"] = value;
                    dr1["TrongLuongT"] = value;
                }
            }
        }



        private bool BetweenInt(int a, int minValue, int maxValue)
        {
            if (a >= minValue && a <= maxValue) return true;
            else return false;
        }
        #region manh
        private void btnExcelPLTong_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnXuatExCel();
        }
        private void dgwTong_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {

        }
        private DataTable GetCodeSize()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetCodeSize&MaDH={_maHang}&MaDVSX=Para&DotSX=Para&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return tbl;
        }
        private DataTable GetKhoiLuongSize()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetKLSize&MaDH={_maHang}&MaDVSX=Para&DotSX=Para&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return tbl;
        }
        private void ExSheet(ExcelWorksheet worksheet, DataTable dtCopy, ExcelRange range, string cang)
        {
            worksheet.Cells["O1"].Value = "Ký hiệu: BM12/QT14/KH01";
            worksheet.Cells["O2"].Value = "Lần sửa đổi: 01";
            worksheet.Cells["O3"].Value = "Ngày ban hành: 16/07/2025";

            string khachhang = dtCopy.Rows[0]["KhachHang"].ToString();
            string color = dtCopy.Rows[0]["TenMau"].ToString();
            string poG = string.Join(", ", dtCopy.AsEnumerable()
                                         .Select(x => x["PO"].ToString())
                                         .Distinct());
            string poTemp = dtCopy.Rows[0]["KieuLapPCB"].ToString() == "4" ? "" : poG;
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Cells.Style.Font.Name = "Times New Roman";
            worksheet.Cells.Style.Font.Size = 11;
            range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
            range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PACKING LIST"; range.Style.Font.Bold = true;
            int colum = 6;
            // isCang = true;
            if (cbxSMS.Checked)
            {
                var dtSourceDH = searchLookUpEditDonHang.Properties.DataSource as DataTable;
                var drTemp = dtSourceDH.AsEnumerable().Where(x => x["MaDH"].ToString() == searchLookUpEditDonHang.EditValue.ToString()).FirstOrDefault();
                if (drTemp != null)
                {
                    _tenHang = drTemp["TenHang"].ToString();
                    _madhDisplay = drTemp["GopDH"].ToString();
                }

            }

            string DeliveryTo = dtCopy.Rows[0]["DeliveryTo"].ToString();
            var lstDeliveryTo = DeliveryTo.Split('|');
            string StylePackNo = dtCopy.Rows[0]["StylePackNo"].ToString();
            string Terms = dtCopy.Rows[0]["Terms"].ToString();
            string CountryOfOrigin = dtCopy.Rows[0]["CountryOfOrigin"].ToString();
            string StyleName = _madhDisplay;// dtCopy.Rows[0]["StyleName"].ToString();
            string DeptNo = dtCopy.Rows[0]["DeptNo"].ToString();

            range = worksheet.Cells["A4:A4"];/* range.Merge = true;*/ range.Value = "BUYER: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
            range = worksheet.Cells["C4:C4"];/* range.Merge = true;*/ range.Value = khachhang; range.Style.Font.Bold = true;
            range = worksheet.Cells["A5:A5"];/* range.Merge = true;*/ range.Value = "Style name: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
            range = worksheet.Cells["C5:C5"];/* range.Merge = true;*/ range.Value = _tenHang; range.Style.Font.Bold = true;
            range = worksheet.Cells["A6:A6"];/* range.Merge = true;*/ range.Value = "Style no: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
            range = worksheet.Cells["C6:C6"];/* range.Merge = true;*/ range.Value = StyleName; range.Style.Font.Bold = true;
            range = worksheet.Cells["A7:A7"];/* range.Merge = true;*/ range.Value = "Style Pack No: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
            range = worksheet.Cells["C7:C7"];/* range.Merge = true;*/ range.Value = StylePackNo; range.Style.Font.Bold = true;
            range = worksheet.Cells["A8:A8"];/* range.Merge = true;*/ range.Value = "PO: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
            range = worksheet.Cells["C8:C8"];/* range.Merge = true;*/ range.Value = poTemp; range.Style.Font.Bold = true;
            if (isCang)
            {
                range = worksheet.Cells["A9:A9"]; /*range.Merge = true; */range.Value = "COLOR: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
                range = worksheet.Cells["C9:C9"]; /*range.Merge = true; */range.Value = color; range.Style.Font.Bold = true;
                range = worksheet.Cells["A10:A10"]; /*range.Merge = true; */range.Value = "DEPT NO: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
                range = worksheet.Cells["C10:C10"]; /*range.Merge = true; */range.Value = DeptNo; range.Style.Font.Bold = true;
            }

            range = worksheet.Cells["H4:H4"]; /*range.Merge = true; */range.Value = "DELIVERY TO: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;

            range = worksheet.Cells["K4:K4"]; /*range.Merge = true; */range.Value = lstDeliveryTo[0]; range.Style.Font.Bold = true;
            range = worksheet.Cells["K5:K5"]; /*range.Merge = true; */range.Value = lstDeliveryTo.Length > 1 ? lstDeliveryTo[1] : ""; range.Style.Font.Bold = true;
            range = worksheet.Cells["K6:K6"]; /*range.Merge = true; */range.Value = lstDeliveryTo.Length > 2 ? lstDeliveryTo[2] : ""; range.Style.Font.Bold = true;

            range = worksheet.Cells["H6:H6"]; /*range.Merge = true; */range.Value = "DESTINATION PORT: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;
            range = worksheet.Cells["H8:H8"]; /*range.Merge = true; */range.Value = "TERMS: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;

            range = worksheet.Cells["H9:H9"]; /*range.Merge = true; */range.Value = "COUNTRY OF ORIGIN: "; range.Style.Font.Bold = true; range.Style.Font.UnderLine = true;

            range = worksheet.Cells["k6:q7"]; range.Merge = true; range.Value = cang; range.Style.Font.Bold = true;
            range.Style.WrapText = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            range = worksheet.Cells["k8:k8"]; /*range.Merge = true; */range.Value = Terms; range.Style.Font.Bold = true;
            range = worksheet.Cells["k9:k9"]; /*range.Merge = true; */range.Value = CountryOfOrigin; range.Style.Font.Bold = true;
            for (int i = 4; i < 11; i++)
            {
                range = worksheet.Cells[i, 1, i, 100]; range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }
            range = worksheet.Cells["O1:O3"];
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            range.Style.Font.Bold = true;
            if (pictureEdit1.Image != null)
            {
                using (var bmp = new Bitmap(pictureEdit1.Image))
                {
                    string tempFolder = Path.Combine(Application.StartupPath, "TempImages");
                    Directory.CreateDirectory(tempFolder);

                    string imgPath = Path.Combine(
                        tempFolder,
                        $"TemImage.png"
                    );
                    bmp.Save(imgPath, ImageFormat.Png);
                    Image image = Image.FromFile(imgPath);
                    OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("picTemp", image);
                    picture.SetPosition(10, 0, 20, 0);
                    picture.SetSize(500, 500);
                }
            }

        }
        private void tinhtongEx(ExcelWorksheet worksheet, DataTable dtPKLXuatHang, ExcelRange range)
        {
            columnSheet = 1;
            List<string> columnNamesWithSize = dtPKLXuatHang.Columns.Cast<DataColumn>()
                                   .Where(column => column.ColumnName.Contains("@"))
                                   .Select(column => column.ColumnName)
                                   .Distinct()
                                   .ToList();
            var uniqueValues = dtPKLXuatHang.AsEnumerable().
          Select(x => new
          {
              MaPKL = x["MaPKLDisplay"],
              SttThung = x["SttThung"],
              SLThung = x["SLThung"],
              TotalPiece = x["TotalPiece"],
              SoLuong = x["SoLuong"],
              TrongLuong = x["TrongLuong"],
              KhoiLuong = x["KhoiLuong"],
              KyHieu = x["KyHieu"],
          }).Distinct().ToList();

            int totalSLThung = uniqueValues.Sum(x => Convert.ToInt32(x.SLThung));
            int totalTotalPiece = uniqueValues.Sum(x => Convert.ToInt32(x.TotalPiece));
            int totalSoLuong = uniqueValues.Sum(x => Convert.ToInt32(x.SoLuong));
            float totalTrongLuong = uniqueValues.Sum(x => Convert.ToSingle(x.TrongLuong));
            float roundedTotalTrongLuong = (float)Math.Round(totalTrongLuong, 1);
            float totalKhoiLuong = uniqueValues.Sum(x => Convert.ToSingle(x.KhoiLuong));
            float roundedTotalKhoiLuong = (float)Math.Round(totalKhoiLuong, 1);
            List<string> sumToTalAll = new List<string>() { totalSLThung.ToString(), totalTotalPiece.ToString(), roundedTotalKhoiLuong.ToString(), roundedTotalTrongLuong.ToString(), };

            foreach (DataRow item in dtPKLXuatHang.Rows)
            {
                foreach (var colums in columnNamesWithSize)
                {
                    int cellValue = Convert.ToInt32(item[colums.ToString()]);
                    item[colums] = cellValue * Convert.ToInt32(item["SLThung"]);
                }
            }
            worksheet.Cells[rowSheet, columnSheet].Value = "Total"; worksheet.Cells[rowSheet, columnSheet].Style.Font.Bold = true; columnSheet++;
            foreach (var colums in columnNamesWithSize)
            {

                int cellValue = dtPKLXuatHang.AsEnumerable().Sum(x => Convert.ToInt32(x[colums]));
                worksheet.Cells[rowSheet, columnSheet].Value = cellValue == 0 ? (object)" " : cellValue;
                worksheet.Cells[rowSheet, columnSheet].Style.Font.Bold = true;
                columnSheet++;
            }
            foreach (string sumAll in sumToTalAll)
            {
                worksheet.Cells[rowSheet, columnSheet + 4].Value = sumAll;
                worksheet.Cells[rowSheet, columnSheet + 4].Style.Font.Bold = true;
                columnSheet++;
            }
            Color customColor = ColorTranslator.FromHtml("#FFE699");
            for (int col = 1; col <= columnSheet + 4; col++)
            {
                worksheet.Cells[rowSheet, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowSheet, col].Style.Fill.BackgroundColor.SetColor(customColor);
            }
            var borderData = worksheet.Cells[rowSheet, 1, rowSheet, columnSheet + 4].Style.Border;
            borderData.Bottom.Style =
                borderData.Top.Style =
                borderData.Left.Style =
                borderData.Right.Style = ExcelBorderStyle.Thin;
            rowSheet += 2;
            range = worksheet.Cells[rowSheet, 1, rowSheet, 3]; range.Merge = true; range.Value = "TOTAL QUANTITY: "; range.Style.Font.Bold = true;
            range = worksheet.Cells[rowSheet, 4]; range.Value = totalTotalPiece.ToString();
            range = worksheet.Cells[rowSheet, 5]; range.Value = "PCS ";
            rowSheet++;
            range = worksheet.Cells[rowSheet, 1, rowSheet, 3]; range.Merge = true; range.Value = "TOTAL CARTONS: "; range.Style.Font.Bold = true;
            range = worksheet.Cells[rowSheet, 4]; range.Value = totalSLThung.ToString();
            range = worksheet.Cells[rowSheet, 5]; range.Value = "CTNS ";
            rowSheet++;
            range = worksheet.Cells[rowSheet, 1, rowSheet, 3]; range.Merge = true; range.Value = "TOTAL NET WT: "; range.Style.Font.Bold = true;
            range = worksheet.Cells[rowSheet, 4]; range.Value = roundedTotalKhoiLuong.ToString();
            range = worksheet.Cells[rowSheet, 5]; range.Value = "KGS ";
            rowSheet++;
            range = worksheet.Cells[rowSheet, 1, rowSheet, 3]; range.Merge = true; range.Value = "TOTAL GROSS WT: "; range.Style.Font.Bold = true;
            range = worksheet.Cells[rowSheet, 4]; range.Value = roundedTotalTrongLuong.ToString();
            range = worksheet.Cells[rowSheet, 5]; range.Value = "KGS ";
            rowSheet++;
            range = worksheet.Cells[rowSheet, 1, rowSheet, 3]; range.Merge = true; range.Value = "CARTON MEASUREMENTS: "; range.Style.Font.Bold = true;
            range = worksheet.Cells[rowSheet, 5]; range.Value = "CBM ";
            var carton = dtPKLXuatHang.AsEnumerable()
           .Select(x => new { KyHieu = x["KyHieu"].ToString() })
           .Distinct()
           .ToList();
            double slThung = 0;
            if (carton.Count > 1)
            {
                int rowCarton = 0;
                foreach (var item in carton)
                {
                    if (item.KyHieu == "") continue;
                    int sumCarton = uniqueValues.Where(x => x.KyHieu.ToString() == item.KyHieu.ToString()).Sum(x => Convert.ToInt32(x.SLThung));
                    range = worksheet.Cells[rowSheet, 6]; range.Value = item.KyHieu;

                    string[] totalSLThung1 = item.KyHieu.ToString().Split('x');
                    slThung += (Convert.ToInt32(totalSLThung1[0]) * 0.01) * (Convert.ToInt32(totalSLThung1[1]) * 0.01) * (Convert.ToInt32(totalSLThung1[2]) * 0.01) * sumCarton;
                    range = worksheet.Cells[rowSheet, 7];
                    range.Value = sumCarton;

                    rowSheet++;
                }
            }
            else
            {
                if (dtPKLXuatHang.Rows[0]["KyHieu"].ToString() != "")
                {
                    string[] totalSLThung1 = dtPKLXuatHang.Rows[0]["KyHieu"].ToString().Split('x');
                    slThung = (Convert.ToInt32(totalSLThung1[0]) * 0.01) * (Convert.ToInt32(totalSLThung1[1]) * 0.01) * (Convert.ToInt32(totalSLThung1[2]) * 0.01) * totalSLThung;
                    range = worksheet.Cells[rowSheet, 4]; range.Merge = true; range.Value = Math.Round(slThung, 2);
                }

            }
            for (int i = 1; i < 6; i++)
            {
                worksheet.Cells[rowSheet + 1 - i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet.Cells[rowSheet + 1 - i, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet.Cells[rowSheet + 1 - i, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }
            string phapdanhCty = dtPKLXuatHang.Rows[0]["TenCty"].ToString();
            range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = phapdanhCty; range.Style.Font.Bold = true;
            worksheet.Column(3).Width = 11;
        }
        int columnSheet = 0;
        int rowSheet = 0;
        private void dtExcelSheet(ExcelWorksheet worksheet, DataTable dtPKLXuatHang, ExcelRange range, DataTable dtCodeSize, int rowSheet1)
        {
            if (dtPKLXuatHang.Rows.Count == 0) return;

            int rowMerge = rowSheet1;
            int indexRow = 1;
            List<string> columnNamesWithSize = dtPKLXuatHang.Columns.Cast<DataColumn>()
                        .Where(column => column.ColumnName.Contains("@"))
                        .Select(column => column.ColumnName)
                        .Distinct()
                        .ToList();
            string cang = dtPKLXuatHang.Rows[0]["Destination"].ToString();
            var uniqueValues = dtPKLXuatHang.AsEnumerable().
           Select(x => new
           {
               MaPKL = x["MaPKLDisplay"],
               SttThung = x["SttThung"],
               SLThung = x["SLThung"],
               TotalPiece = x["TotalPiece"],
               SoLuong = x["SoLuong"],
               TrongLuong = x["TrongLuong"],
               KhoiLuong = x["KhoiLuong"],
               KyHieu = x["KyHieu"],
           }).Distinct().ToList();

            int totalSLThung = uniqueValues.Sum(x => Convert.ToInt32(x.SLThung));
            int totalTotalPiece = uniqueValues.Sum(x => Convert.ToInt32(x.TotalPiece));
            int totalSoLuong = uniqueValues.Sum(x => Convert.ToInt32(x.SoLuong));
            float totalTrongLuong = uniqueValues.Sum(x => Convert.ToSingle(x.TrongLuong));
            float roundedTotalTrongLuong = (float)Math.Round(totalTrongLuong, 1);
            float totalKhoiLuong = uniqueValues.Sum(x => Convert.ToSingle(x.KhoiLuong));
            float roundedTotalKhoiLuong = (float)Math.Round(totalKhoiLuong, 1);
            List<string> sumToTalAll = new List<string>() { totalSLThung.ToString(), totalTotalPiece.ToString(), roundedTotalKhoiLuong.ToString(), roundedTotalTrongLuong.ToString(), };
            int totalPCB_Pack = dtPKLXuatHang.AsEnumerable().Sum(x => Convert.ToInt32(x["PCB_Pack"]));
            columnSheet = 1;
            range = worksheet.Cells[rowSheet, columnSheet]; range.Value = cang; rowSheet++;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            range = worksheet.Cells[rowSheet, columnSheet]; range.Value = "Size";
            range = worksheet.Cells[rowSheet + 1, columnSheet]; range.Value = "Keycode"; columnSheet++;

            foreach (var colums in columnNamesWithSize)
            {
                var splitColum = colums.Split('@');
                worksheet.Cells[rowSheet, columnSheet].Value = splitColum[0].ToString();
                worksheet.Cells[rowSheet, columnSheet].Style.Font.Bold = true;
                var dtTempcodeSize = dtCodeSize.AsEnumerable().Where(x => x["SizeID"].ToString() == splitColum[1].ToString() && x["MaMau"].ToString() == dtPKLXuatHang.Rows[0]["ColorID"].ToString()).FirstOrDefault();
                if (dtTempcodeSize != null)
                    worksheet.Cells[rowSheet + 1, columnSheet].Value = dtTempcodeSize["CodeSize"].ToString(); worksheet.Cells[rowSheet + 1, columnSheet].Style.Font.Color.SetColor(Color.FromArgb(27, 109, 201));
                columnSheet++;
            }
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "PCS/ \n PACK"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "PACK/ \n CTN"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "Q'TY/ \n CTN"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "CARTON \n NO."; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "TOTAL \n CTN"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "TOTAL \n Q'TY"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "NW \n (kgs)"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "GW \n (kgs)"; range.Style.Font.Bold = true; range.Style.WrapText = true; columnSheet++;
            range = worksheet.Cells[rowSheet, columnSheet, rowSheet + 1, columnSheet]; range.Merge = true; range.Value = "Measurement \n (cm) "; range.Style.Font.Bold = true; range.Style.WrapText = true;
            worksheet.Column(columnSheet).Width = 15;
            columnSheet++;
            rowSheet++;
            foreach (DataRow item in dtPKLXuatHang.Rows)
            {
                rowSheet++;
                columnSheet = 1;
                worksheet.Cells[rowSheet, columnSheet].Value = "Inner"; columnSheet++;
                foreach (var colums in columnNamesWithSize)
                {
                    if (item[colums.ToString()].ToString() == "")
                    {
                        columnSheet++;
                        continue;
                    }
                    int cellValue = Convert.ToInt32(item[colums.ToString()]);
                    worksheet.Cells[rowSheet, columnSheet].Value = cellValue == 0 ? (object)" " : cellValue;
                    worksheet.Cells[rowSheet, columnSheet].Style.Font.Bold = false;
                    ;
                    columnSheet++;
                    item[colums] = cellValue * Convert.ToInt32(item["SLThung"]);
                }
                double khoiLuong = Convert.ToDouble(item["KhoiLuong"]);
                double roundedKhoiLuong = Math.Round(khoiLuong, 1);
                double trongLuong = Convert.ToDouble(item["TrongLuong"]);
                string tuthung = item["TuThung"].ToString();
                string denthung = item["DenThung"].ToString();
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = Convert.ToInt32(item["PCB_Pack"]); range.Style.Font.Bold = false; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = Convert.ToInt32(item["Pack_Ctn"]); range.Style.Font.Bold = false; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = Convert.ToInt32(item["SoLuong"]); range.Style.Font.Bold = false; columnSheet++;
                worksheet.Cells[rowSheet, columnSheet].Value = tuthung + "-" + denthung; worksheet.Column(columnSheet).Width = 12; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = Convert.ToInt32(item["SLThung"]); ; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = Convert.ToInt32(item["TotalPiece"]); ; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = khoiLuong == 0 ? (object)" " : roundedKhoiLuong; range.Style.Font.Bold = false;
                ; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = trongLuong == 0 ? (object)" " : Math.Round(trongLuong, 1); range.Style.Font.Bold = false;
                ; columnSheet++;
                range = worksheet.Cells[rowSheet, columnSheet]; range.Value = item["KyHieu"].ToString(); columnSheet++;
            }
            rowSheet += 2;
            columnSheet = 2;
            foreach (var colums in columnNamesWithSize)
            {

                int cellValue = dtPKLXuatHang.AsEnumerable().Sum(x => Convert.ToInt32(x[colums]));
                worksheet.Cells[rowSheet, columnSheet].Value = cellValue == 0 ? (object)" " : cellValue;
                worksheet.Cells[rowSheet, columnSheet].Style.Font.Bold = false;
                columnSheet++;
            }
            foreach (string sumAll in sumToTalAll)
            {
                worksheet.Cells[rowSheet, columnSheet + 4].Value = sumAll;
                worksheet.Cells[rowSheet, columnSheet + 4].Style.Font.Bold = true;
                columnSheet++;
            }
            var borderData = worksheet.Cells[rowMerge, 1, rowSheet, columnSheet + 4].Style.Border;
            borderData.Bottom.Style =
                borderData.Top.Style =
                borderData.Left.Style =
                borderData.Right.Style = ExcelBorderStyle.Thin;
            rowSheet += 2;
        }
        private void ExportExcel(string path)
        {
            try
            {
                var dtCodeSize = GetCodeSize();
                var dtKhoiLuongSize = GetKhoiLuongSize();

                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                rowSheet = 12;
                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {

                    DataTable dtPKL = searchLookUpEdit_MaPKL.Properties.DataSource as DataTable;
                    if (!cbxAllPO && Convert.ToInt32(dtPKL.Rows[0]["KieuLapPCB"]) == 10)
                    {
                        string cang = "";
                        DataTable dtCopy = new DataTable();
                        DataTable dtCopy1 = new DataTable();
                        cang = "";
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"{_po}_{cang}");
                        ExcelRange range = worksheet.Cells;
                        int index = 1;
                        columnSheet = 1;
                        string PathLoGo = "";
                        range = worksheet.Cells;
                        string phapdanhCty = "";

                        string action = "";
                        foreach (DataRow item in dtPKL.Rows)
                        {
                            string maPKL = item["MaPKL"].ToString().Split('@')[0];
                            action = cbxSMS.Checked ? "GetPivotKHDongThung_SMS" : "GetPivotKHDongThung";
                            string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={_madh}&MaDVSX={_maDVSX1}&DotSX={_dotSX}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={maPKL}");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            dtCopy = JsonConvert.DeserializeObject<DataTable>(json);
                            cang = "";
                            foreach (DataRow dr in dtCopy.Rows)
                            {
                                dr["KyHieu"] = dr["KyHieuA"];
                            }
                            phapdanhCty = dtCopy.Rows[0]["TenCty"].ToString();
                            if (index == 1)
                                ExSheet(worksheet, dtCopy, range, cang);
                            dtExcelSheet(worksheet, dtCopy, range, dtCodeSize, rowSheet);
                            index++;
                        }
                        action = cbxSMS.Checked ? "GetPivotKHDongThung_SMS" : "GetPivotKHDongThung";
                        string url1 = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action={action}&MaDH={_madh}&MaDVSX={_maDVSX1}&DotSX={_dotSX}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL=A");
                        string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                        dtCopy1 = JsonConvert.DeserializeObject<DataTable>(json1);
                        foreach (DataRow dr in dtCopy1.Rows)
                        {
                            dr["KyHieu"] = dr["KyHieuA"];
                        }
                        if (phapdanhCty.ToString() == "CÔNG TY TNHH VIKING VIỆT NAM")
                            PathLoGo = KHDongThungLib.getImgPath("texgiang.jpg");
                        else PathLoGo = KHDongThungLib.getImgPath("texgiang1.jpg");
                        worksheet.Cells["O1"].Value = "Ký hiệu:BM12/QT14/KH01";
                        worksheet.Cells["O2"].Value = "Lần sửa đổi: 01";
                        worksheet.Cells["O3"].Value = "Ngày ban hành: 16/07/2025";
                        range = worksheet.Cells["A1:B3"]; range.Merge = true;
                        Image image = Image.FromFile(PathLoGo);
                        OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                        picture.SetPosition(0, 0, 0, 0);
                        picture.SetSize(105, 55);
                        tinhtongEx(worksheet, dtCopy1, range);
                    }
                    else
                    {
                        if (cbxAllPO)
                        {
                            DataTable tblPo = grcDonHang.DataSource as DataTable;

                            var tblPoTemp = tblPo.AsEnumerable().Where(x => x["SLDaLapPKL"].ToString() != "0");
                            if (tblPoTemp.Count() == 0) return;
                            tblPo = tblPoTemp.CopyToDataTable();
                            foreach (DataRow item in tblPo.Rows)
                            {
                                string itempo = item["POID"].ToString();
                                string url1 = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetDotLapPKL&MaDH={_madh}&MaDVSX={_maDVSX}&DotSX=${_dotSX}&POID={itempo}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
                                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json1);
                                foreach (DataRow drRow in tbl.Rows)
                                {
                                    string cang = "";
                                    string maPKL = drRow["MaPKL"].ToString().Split('@')[0];
                                    DataTable dtCopy = new DataTable();
                                    DataTable dtCopyA = new DataTable();
                                    string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={_madh}&MaDVSX={_maDVSX1}&DotSX={_dotSX}&POID={itempo}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={maPKL}");
                                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                                    dtCopyA = JsonConvert.DeserializeObject<DataTable>(json);
                                    dtCopy = dtCopyA.Copy();
                                    foreach (DataRow dr in dtCopy.Rows)
                                    {
                                        dr["KyHieu"] = dr["KyHieuA"];
                                    }
                                    int Height = 100;
                                    int Width = 150;
                                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"{maPKL}_{itempo}_{cang}");
                                    ExcelRange range = worksheet.Cells;
                                    ExSheet(worksheet, dtCopy, range, cang);
                                    KHDongThungLib.dtXuatEX(dtCopy, worksheet, isCang ? false : true, 10, dtCodeSize, cbxCodeSizeA, cbxArtA,
                                                            true, dtKhoiLuongSize, false, cbxCodeKLA, cbxPOInSeamA, cbxDVSXA, cbxKg_ThungA, cbxBarCode);
                                }
                            }
                        }
                        else
                        {
                            if (!isCang)
                                dtPKL = dtPKL.AsEnumerable().Where(x => x["MaPKL"].ToString() == searchLookUpEdit_MaPKL.EditValue.ToString()).CopyToDataTable();
                            foreach (DataRow item in dtPKL.Rows)
                            {
                                string cang = "";
                                string maPKL = item["MaPKL"].ToString().Split('@')[0];
                                DataTable dtCopy = new DataTable();
                                DataTable dtCopyA = new DataTable();
                                string url = string.Format("{0}", URL + $"KeHoachDongThung/Get?Action=GetPivotKHDongThung&MaDH={_madh}&MaDVSX={_maDVSX1}&DotSX={_dotSX}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={maPKL}");
                                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                                dtCopyA = isCang ? JsonConvert.DeserializeObject<DataTable>(json) : dgrKHDongThung.DataSource as DataTable;
                                dtCopy = dtCopyA.Copy();
                                cang = isCang == true ? dtCopy.Rows[0]["Destination"].ToString() : "";
                                foreach (DataRow dr in dtCopy.Rows)
                                {
                                    dr["KyHieu"] = dr["KyHieuA"];
                                }
                                int Height = 100;
                                int Width = 150;

                                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(SanitizeFileName($"{item["MaPKL"].ToString()}_{item["POID"].ToString()}_{cang}"));
                                ExcelRange range = worksheet.Cells;
                                string cangTemp = dtCopy.Rows[0]["Destination"].ToString();
                                ExSheet(worksheet, dtCopy, range, cangTemp);
                                KHDongThungLib.dtXuatEX(dtCopy, worksheet, isCang ? false : true, 11, dtCodeSize, cbxCodeSizeA, cbxArtA,
                                                        false, dtKhoiLuongSize, false, cbxCodeKLA, cbxPOInSeamA, cbxDVSXA, cbxKg_ThungA, cbxBarCode);
                            }
                        }

                        #region  Tạm Đóng


                        #endregion


                    }

                    excelPackage.SaveAs(file);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        private string SanitizeFileName(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            string result = input;
            foreach (char c in invalid) result = result.Replace(c.ToString(), "");
            result = Regex.Replace(result.Trim(), @"\s+", " ");
            return result;
        }
        private void cbxViewArt_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }



        private void btnLoc_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                var dtTemp = dtDataPiVot.AsEnumerable().Where(x => Convert.ToInt32(x["TuThung"]) >= Convert.ToInt32(txtFrom.Text)
                                                && Convert.ToInt32(x["DenThung"]) <= Convert.ToInt32(txtTo.Text));
                if (dtTemp.Count() == 0) return;
                dt = dtTemp.CopyToDataTable();
                dgrKHDongThung.DataSource = dt;
                bandedGridViewKHDT.RefreshData();
            }
            catch (Exception ex)
            {

            }
        }
        private void btnLapTheoTL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnLapTheoTiLe();
        }

        private void btnDaSizeDaTiLe_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnLapTheoDaTiLeDaSize();
        }

        private void btnLapTheoPack_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnLapTheoPack();
        }

        private void bandedGridViewKHDT_DoubleClick(object sender, EventArgs e)
        {
            //DataRow drFocusCT = null;
            //if (bandedGridViewKHDT.FocusedRowHandle < 0)
            //{
            //    drFocusCT = null;
            //    return;
            //}
            //drFocusCT = bandedGridViewKHDT.GetFocusedDataRow();
            //if (drFocusCT is null) return;
            //frmChiTietScanThungXuatHang fr = new frmChiTietScanThungXuatHang(drFocusCT, true, true, false, cbxSMS.Checked);
            //fr.ShowDialog();
            //int rowHandle = bandedGridViewKHDT.FocusedRowHandle;
            ////GetDataCTPhieu(MaPhieuXH, Cont_Link);
            //bandedGridViewKHDT.FocusedRowHandle = rowHandle;

            DataTable dtSource = dgrKHDongThung.DataSource as DataTable;
            frmChiTietThungTonBarcodeV3 frm = new frmChiTietThungTonBarcodeV3(dtSource.Rows[0], true, true, false, false, cbxSMS.Checked);
            frm.ShowDialog();
        }



        private void cbxSMS_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckEdit;
            var cbxSMS = cb.Checked;
            if (cbxSMS)
            {
                dtQuiCach = KHDongThungLib.GetAllQuiCach(repoQuiCach, URL, "Para", _clientExtension);
                grcDonHang.DataSource = new DataTable();
                dgrMauSize.DataSource = new DataTable();
                LoadDonHangSMS();
            }
            else LoadDonHang();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var dtSource = dgrKHDongThung.DataSource as DataTable;

            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            var TuThungStart = dtSource.Rows[0]["TuThung"].ToString();
            if (dtSource == null || drFocus == null) return;
            var sttThung = drFocus["SttThung"].ToString();
            var dtMove = dtSource.AsEnumerable()
                                 .Where(x => x["SttThung"].ToString() == sttThung)
                                 .OrderBy(x => dtSource.Rows.IndexOf(x))
                                 .ToList();
            var indexStep = dtSource.Rows.IndexOf(dtMove[0]); // Lấy phần đầu trên cùng của hàng đc di chuyển đi
            if (indexStep == 0) return;
            var drStep = dtSource.Rows[indexStep - 1]; // Lấy stt thùng phía trên
            var drCheckStepUp = dtSource.AsEnumerable()
                                 .Where(x => Convert.ToInt16(x["SttThung"]) == Convert.ToInt16(drStep["sttThung"])).FirstOrDefault(); // Check có bn hàng gọp
            int stepUp = 0;
            if (drCheckStepUp != null)
                stepUp = dtSource.AsEnumerable()
                                     .Where(x => Convert.ToInt16(x["SttThung"]) == Convert.ToInt16(drCheckStepUp["SttThung"])).Count() - 1; // Tính ra số bước cần nhảy
            for (int i = 0; i < dtMove.Count; i++)
            {
                DataRow dr = dtMove[i];
                var index = dtSource.Rows.IndexOf(dr);
                if (index <= 0) continue;
                DataRow newRow = dtSource.NewRow();
                newRow.ItemArray = dr.ItemArray.Clone() as object[];
                dtSource.Rows.RemoveAt(index);
                dtSource.Rows.InsertAt(newRow, index - stepUp - 1);
            }
            if (dtMove.Count > 0)
            {
                var firstRow = dtSource.AsEnumerable().FirstOrDefault(x => x["SttThung"].ToString() == sttThung);
                if (firstRow != null)
                {
                    bandedGridViewKHDT.FocusedRowHandle = dtSource.Rows.IndexOf(firstRow);
                }
            }
            KHDongThungLib.TinhToanLaiKhiXoa(dtSource, -1, false, "", TuThungStart);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var dtSource = dgrKHDongThung.DataSource as DataTable;
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drFocus is null) return;
            var TuThungStart = dtSource.Rows[0]["TuThung"].ToString();
            var sttThung = drFocus["SttThung"].ToString();

            var dtMove = dtSource.AsEnumerable()
                                 .Where(x => x["SttThung"].ToString() == sttThung)
                                 .ToList();

            var indexStep = dtSource.Rows.IndexOf(dtMove[dtMove.Count() - 1]); // Lấy phần dưới trên cùng của hàng đc di chuyển đi
            if (indexStep == dtSource.Rows.Count - 1) return;
            var drStep = dtSource.Rows[indexStep + 1]; // Lấy stt thùng phía dưới
            var drCheckStepDown = dtSource.AsEnumerable()
                                 .Where(x => Convert.ToInt16(x["SttThung"]) == Convert.ToInt16(drStep["sttThung"])).FirstOrDefault(); // Check có bn hàng gọp

            //var drCheckStepDown = dtSource.AsEnumerable()
            //                     .Where(x => Convert.ToInt16(x["SttThung"]) > Convert.ToInt16(sttThung)).FirstOrDefault();
            int stepDown = 0;
            if (drCheckStepDown != null)
                stepDown = dtSource.AsEnumerable()
                                     .Where(x => Convert.ToInt16(x["SttThung"]) == Convert.ToInt16(drCheckStepDown["SttThung"])).Count() - 1;
            for (int i = dtMove.Count - 1; i >= 0; i--)
            {
                DataRow dr = dtMove[i];
                var index = dtSource.Rows.IndexOf(dr);
                if (index == dtSource.Rows.Count - 1) continue;
                DataRow newRow = dtSource.NewRow();
                newRow.ItemArray = dr.ItemArray.Clone() as object[];
                dtSource.Rows.RemoveAt(index);
                dtSource.Rows.InsertAt(newRow, index + 1 + stepDown);
            }
            if (dtMove.Count > 0)
            {
                var firstRow = dtSource.AsEnumerable().FirstOrDefault(x => x["SttThung"].ToString() == sttThung);
                if (firstRow != null)
                {
                    bandedGridViewKHDT.FocusedRowHandle = dtSource.Rows.IndexOf(firstRow);
                }
            }
            KHDongThungLib.TinhToanLaiKhiXoa(dtSource, -1, false, "", TuThungStart);
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            DataTable dtSource = dgrKHDongThung.DataSource as DataTable;
            frmChiTietThungTonBarcodeV3 frm = new frmChiTietThungTonBarcodeV3(dtSource.Rows[0], true, true, false, false, cbxSMS.Checked);
            frm.ShowDialog();
        }

        private void btnSMS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPKeHoachDongThung_SMS fr = new frmERPKeHoachDongThung_SMS();
            fr.ShowDialog();
            var _maDHSMS = frmERPKeHoachDongThung_SMS._maDHSMS.Replace(" ", "").ToUpper();
            if (cbxSMS.Checked && _maDHSMS != "")
            {
                LoadDonHangSMS();
                searchLookUpEditDonHang.EditValue = KHDongThungLib.ReplaceSpecialCharacters(_maDHSMS);
            }
            else if (!cbxSMS.Checked && _maDHSMS != "")
            {
                cbxSMS.Checked = true;
            }
            else if (_maDHSMS == "")
            {
                LoadDonHangSMS();
                var dtSMS = searchLookUpEditDonHang.Properties.DataSource as DataTable;
                if (dtSMS == null || dtSMS.Rows.Count == 0) return;
                searchLookUpEditDonHang.EditValue = dtSMS.Rows[0]["MaDH"];
            }
            //else if (cbxSMS.Checked)
            //{
            //    LoadDonHang();
            //}
        }

        private void btnLapKHStoreV2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThung_Store_V2 fr = new frmERPKeHoachDongThung_Store_V2(_madh, _madhDisplay, _maHang, _tenHang, _poid, "");
            fr.ShowDialog();
            if (frmERPKeHoachDongThung_Store_V2.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThung_Store_V2.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThung_Store_V2.POID;
            }

            GetDotPKL();
            LoadKHDongThung();
        }
        private void btnLapKH_Store_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThung_Store fr = new frmERPKeHoachDongThung_Store(_madh, _maHang);
            fr.ShowDialog();
            KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension);
            GetDotPKL();
            LoadKHDongThung();
        }
        private void BtnLapTheoTiLe()
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThung_TiLe fr = new frmERPKeHoachDongThung_TiLe(_madh, _madhDisplay, _maHang, _tenHang, _poid, "");
            fr.ShowDialog();
            if (frmERPKeHoachDongThung_TiLe.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThung_TiLe.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThung_TiLe.POID;
            }

            GetDotPKL();
            LoadKHDongThung();
        }
        private void BtnLapTheoDaTiLeDaSize()
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThung_DaTiLeDaSize fr = new frmERPKeHoachDongThung_DaTiLeDaSize(_madh, _madhDisplay, _maHang, _tenHang, _poid, "");
            fr.ShowDialog();
            if (frmERPKeHoachDongThung_DaTiLeDaSize.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThung_DaTiLeDaSize.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThung_DaTiLeDaSize.POID;
            }

            GetDotPKL();
            LoadKHDongThung();
        }
        private void btnHangTreo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThung_HangMocTreo fr = new frmERPKeHoachDongThung_HangMocTreo(_madh, _madhDisplay, _maHang, _tenHang, _poid, "");
            fr.ShowDialog();
            if (frmERPKeHoachDongThung_HangMocTreo.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThung_HangMocTreo.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThung_HangMocTreo.POID;
            }

            GetDotPKL();
            LoadKHDongThung();
        }
        private void BtnLapTheoPack()
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThungV5 fr = new frmERPKeHoachDongThungV5(_madh, _madhDisplay, _maHang, _tenHang, _poid, "");
            fr.ShowDialog();
            if (frmERPKeHoachDongThungV5.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThungV5.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThungV5.POID;
            }

            GetDotPKL();
            LoadKHDongThung();
        }



        private bool AlarmOptionPKL()
        {
            if (cbxSMS.Checked)
            {
                MessageBox.Show("Đang chọn hàng SMS. Vui lòng chọn lập PKL SMS để thực hiện!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }



        private void BtThemPODot()
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThungV4 fr = new frmERPKeHoachDongThungV4(_madh, _madhDisplay, _maHang, _tenHang, _poid, "");
            fr.ShowDialog();
            if (frmERPKeHoachDongThungV4.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThungV4.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThungV4.POID;
            }
            KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension);
            GetDotPKL();
            LoadKHDongThung();
        }
        private void BtnThemPOPCB()
        {
            if (AlarmOptionPKL()) return;
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThungV2 fr = new frmERPKeHoachDongThungV2(_madh, _maHang);
            fr.ShowDialog();
            if (frmERPKeHoachDongThungV3.POID != "")
            {
                LoadCTDonHang();
                var dr = _dtCTDonHang.AsEnumerable().Where(x => x["POID"].ToString() == frmERPKeHoachDongThungV3.POID).FirstOrDefault();
                int rowfocus = _dtCTDonHang.Rows.IndexOf(dr);
                grvDonHang.FocusedRowHandle = rowfocus;
                _poid = frmERPKeHoachDongThungV3.POID;
            }
            KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension);
            GetDotPKL();
            LoadKHDongThung();
        }




        private void BtnSua()
        {
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            var dt = dgrKHDongThung.DataSource as DataTable;
            if (dt.Rows.Count == 0) return;
            var _KieuLapPCB = dt.Rows[0]["KieuLapPCB"].ToString();
            if (cbxSMS.Checked)
            {
                frmERPKeHoachDongThung_SMS fr = new frmERPKeHoachDongThung_SMS(_madh, _maPKL);
                fr.ShowDialog();
                LoadKHDongThung();
                return;
            }
            switch (_KieuLapPCB)
            {
                case "0":
                    frmERPKeHoachDongThungV4 fr = new frmERPKeHoachDongThungV4(_madh, _madhDisplay, _maHang, _tenHang, _poid, _maPKL);
                    fr.ShowDialog();
                    break;
                case "1":
                    frmERPKeHoachDongThungV5 fr1 = new frmERPKeHoachDongThungV5(_madh, _madhDisplay, _maHang, _tenHang, _poid, _maPKL);
                    fr1.ShowDialog();
                    break;
                case "2":
                    frmERPKeHoachDongThung_TiLe fr2 = new frmERPKeHoachDongThung_TiLe(_madh, _madhDisplay, _maHang, _tenHang, _poid, _maPKL);
                    fr2.ShowDialog();
                    break;
            }
            KHDongThungLib.GetQuiCach(repoQuiCach, URL, _maHang, _clientExtension);
            //GetDotPKL();
            GetDataSize();
            LoadKHDongThung();
        }



        private void BtnXoa()
        {
            var data = dgrKHDongThung.DataSource as DataTable;
            var checkIsDongThung = data.AsEnumerable().Any(x => x["IsDongThung1"].ToString() == "1");
            if (checkIsDongThung)
            {
                MessageBox.Show("Đã tồn tại thùng đã được đóng. Không thể xóa PKL!", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            DialogResult resultDialog = MessageBox.Show("Xác nhận xóa package list?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDialog == DialogResult.Yes)
            {
                DataTable dtSave = KHDongThungLib.CreateTblSave();
                var drnew = dtSave.NewRow();
                drnew["ID"] = 0;
                drnew["MaPKL"] = searchLookUpEdit_MaPKL.EditValue.ToString().Split('@')[0];
                drnew["MaDH"] = _madh;
                drnew["POID"] = _poid;
                drnew["NVien"] = GlobleData.UserName;
                dtSave.Rows.Add(drnew);
                string url = string.Format("{0}", URL + "KeHoachDongThung/Post?action=DeleteKHDT");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                LoadCTDonHang();
                GetDotPKL();

            }
        }

        private void BtnLuu()
        {
            this.ActiveControl = grcDonHang;
            DataTable dt = dgrKHDongThung.DataSource as DataTable;
            SaveKHDT(dt);
        }
        private void BtnNapLai()
        {
            if (cbxSMS.Checked) return;
            LoadCTDonHang();
        }

        bool isCang = true;
        private void BtnXuatExCel()
        {
            DataTable dt = dgrKHDongThung.DataSource as DataTable;
            if (!cbxAllPO)
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Dữ liệu rỗng !");
                    return;
                }
                else
                {
                    var KieuLap = dt.Rows[0]["KieuLapPCB"].ToString();
                    isCang = KieuLap == "0" || KieuLap == "3" || KieuLap == "4" ? false : true;
                }
            }

            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            Sfd.FileName = SanitizeFileName(string.Format("PKL_{0}_{1}_" + DateTime.Now.ToString("ddMMyyyy"), _maHang == "" ? _madh : _maHang, _po));
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("Bao Cao");
                    ExportExcel(Sfd.FileName);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }


        private void BtnGopThung()
        {
            GopThung();
        }

        private string getImgPath(string Img)
        {
            string Paths = Directory.GetCurrentDirectory();
            return $"{Directory.GetParent(Paths).Parent.FullName}\\Resources\\{Img}";

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
        #region Copy
        private string MaDHSource = "";
        private void btnCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCopyERPKHDongThung fr = new frmCopyERPKHDongThung();
            fr.ShowDialog();
            //MaDHSource = _madh;
        }
        private void btnPaste_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string url = string.Format("{0}", URL + $"KeHoachDongThung/CopyPaste?action=CopyPasteKHDongThung&MaDHDes={_madh}&MaDHSource={MaDHSource}");           
            //string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
        }

        #endregion
        #region Bổ sung ghi chú hình ảnh file
        private void GetData_Description()
        {
            string url = string.Format("{0}", URL + $"KHDongThung_Description/Get?action=Get&para1={_madh}&para2={_poid}&para3={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt.Rows.Count == 0)
            {
                txtTaiLieu.Text = "";
                txtImage.Text = "";
                txtGhiChu.Text = "";
                pictureEdit1.Image = null;
                btnDuyet.EditValue = false;
                btnLock.EditValue = false;
                btnLock.Enabled = true;
                return;
            }
            if (dt.Rows[0]["StatusDuyet"].ToString() == "1")
            {
                btnDuyet.EditValue = true;
                btnLock.Enabled = false;
            }
            else
            {
                btnDuyet.EditValue = false;
                btnLock.Enabled = true;
            }
            if (dt.Rows[0]["StatusLock"].ToString() == "1")
            {
                btnLock.EditValue = true;
            }
            else
            {
                btnLock.EditValue = false;
            }
            txtTaiLieu.Text = dt.Rows[0]["UrlPdf"].ToString();
            txtImage.Text = dt.Rows[0]["UrlHinhAnh"].ToString();
            txtGhiChu.Text = dt.Rows[0]["GhiChu"].ToString();
            BindingImage(dt.Rows[0]["UrlHinhAnh"].ToString());
        }
        private void BindingImage(string image)
        {
            try
            {
                var urlHost = (string)settingsReader.GetValue("URLV2", typeof(String));
                string url = urlHost + "/Content/UrlImgDongThung/" + image;
                using (var wc = new System.Net.WebClient())
                {
                    byte[] data = wc.DownloadData(url);
                    using (var ms = new MemoryStream(data))
                        pictureEdit1.Image = Image.FromStream(ms);
                }
            }
            catch
            {

            }

        }
        private async Task<bool> UploadImage(string action, string sourceFileName, string fileName)
        {
            string UrlUpload = "";
            var client = new WebClient();
            //object objUrl = _regedit.get_RegistryKey(QtyUrlKeyName);
            //objUrl = "localhost:5480";

            UrlUpload = string.Format("{0}/KHDongThung_Description/UploadImage", URL);
            var uri = new Uri(UrlUpload);
            try
            {
                client.Headers.Add("action", action);
                client.Headers.Add("fileName", fileName);
                var data = System.IO.File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(uri, data);
                string result = Encoding.UTF8.GetString(responseBytes);
                if (result.ToUpper() == "TRUE")
                {
                    txtImage.Text = fileName;
                    BindingImage(fileName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private async void btnImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFileName = openFileDialog.FileName;
                var result = await UploadImage("", sourceFileName, openFileDialog.SafeFileName);
                BindingImage(openFileDialog.SafeFileName);
            }
        }
        private async Task<bool> UploadPDF(string TenTaiLieu, string sourceFileName, string fileName)
        {
            string Url = "";
            var client = new WebClient();
            Url = string.Format("{0}/KHDongThung_Description/UploadPDF", URL);
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


        private async void btnUploadTaiLieu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFileName = openFileDialog.FileName;
                var fileName = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName));

                var result = await UploadPDF("A", sourceFileName, fileName);
                if (result)
                {
                    txtTaiLieu.Text = fileName + ".pdf";
                }
            }
        }

        private void btnOpenTaiLieu_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTaiLieu.Text))
            {
                try
                {
                    var urlHost = (string)settingsReader.GetValue("URLV2", typeof(String));
                    string url = urlHost + "/Content/UrlPdfDongThung/" + txtTaiLieu.Text;
                    //string url = $"http://{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
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
        private void btnSaveDescription_Click(object sender, EventArgs e)
        {
            var dtSave = CreateTable();
            var drNew = dtSave.NewRow();
            drNew["ID"] = 0;
            drNew["MaDH"] = _madh;
            drNew["POID"] = _poid;
            drNew["MaPKL"] = _maPKL;
            drNew["GhiChu"] = txtGhiChu.Text;
            drNew["UrlHinhAnh"] = txtImage.Text;
            drNew["UrlPdf"] = txtTaiLieu.Text;
            drNew["NVien"] = GlobleData.UserName;
            dtSave.Rows.Add(drNew);
            string url = string.Format("{0}", URL + "KHDongThung_Description/Post?action=Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);

            }
        }
        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("MaPKL", typeof(string));
            dt.Columns.Add("KieuLapPCB", typeof(int));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("UrlHinhAnh", typeof(string));
            dt.Columns.Add("UrlPdf", typeof(string));
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("NVien", typeof(string));
            return dt;
        }
        private void btnDuyet_MouseDown(object sender, MouseEventArgs e)
        {
            var dtSource = dgrKHDongThung.DataSource as DataTable;
            if (dtSource.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu PKL để duyệt!");
                return;
            }
            bool current = (bool)btnDuyet.EditValue;
            bool next = !current;
            if (!(bool)btnLock.EditValue)
            {
                btnDuyet.EditValue = current;
                MessageBox.Show("PKL chưa được xác nhận!");
                return;
            }
            string mess = next
                ? "Bạn có chắc muốn duyệt PKL này không?"
                : "Bạn có chắc muốn hủy duyệt PKL này không?";

            if (MessageBox.Show(mess, "Thông báo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                btnDuyet.EditValue = next; // chỉ đổi khi Yes
                var dtSave = CreateTable();
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaDH"] = _madh;
                drNew["POID"] = _poid;
                drNew["MaPKL"] = _maPKL;
                drNew["Status"] = btnDuyet.EditValue;
                drNew["NVien"] = GlobleData.UserName;
                dtSave.Rows.Add(drNew);
                string url = string.Format("{0}", URL + "KHDongThung_Description/Post?action=PostDuyet");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    GetData_Description();
                    clsWaitForm.ShowSuccessForm(this, 2000);

                    string Title = next
                  ? "Packing list đã duyệt"
                  : "Packing list đã hủy duyệt";
                    string Detail = $"Đơn Hàng: {searchLookUpEditDonHang.Text}\nPKL đợt: {searchLookUpEdit_MaPKL.Text}";
                    string SendTo = "PKH";
                    SendNotify(Title, Detail, SendTo);


                }
            }
        }
        private void btnLock_MouseDown(object sender, MouseEventArgs e)
        {
            bool current = (bool)btnLock.EditValue;
            bool next = !current;

            string mess = next
                ? "Bạn có chắc muốn khóa PKL này không?"
                : "Bạn có chắc muốn hủy khóa PKL này không?";

            if (MessageBox.Show(mess, "Thông báo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                btnLock.EditValue = next; // chỉ đổi khi Yes
                var dtSave = CreateTable();
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaDH"] = _madh;
                drNew["POID"] = _poid;
                drNew["MaPKL"] = _maPKL;
                drNew["Status"] = btnLock.EditValue;
                drNew["NVien"] = GlobleData.UserName;
                dtSave.Rows.Add(drNew);
                string url = string.Format("{0}", URL + "KHDongThung_Description/Post?action=PostLock");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    string Title = next
                ? "Packing list đã khóa"
                : "Packing list đã mở khóa";
                    string Detail = $"Đơn Hàng: {searchLookUpEditDonHang.Text}\nPKL đợt: {searchLookUpEdit_MaPKL.Text}";
                    string SendTo = "PKH";
                    SendNotify(Title, Detail, SendTo);
                }
            }
        }

        #endregion


        #region Send To Notify
        private void SendNotify(string Title, string Detail, string SendTo, string BoPhan = "ALL", int Status = -1)
        {



            try
            {
                string url = $"{URL}SendToNotification/PushNotificationFrm?" +
                $"UserIDTao={HttpUtility.UrlEncode(GlobleData.UserName)}&" +
                $"FrmName={HttpUtility.UrlEncode(this.Name)}&" +
                $"Title={HttpUtility.UrlEncode(Title)}&" +
                $"Detail={HttpUtility.UrlEncode(Detail)}&" +
                $"SendTo={HttpUtility.UrlEncode(SendTo)}&" +
                $"BoPhan={HttpUtility.UrlEncode(BoPhan)}&" +
                $"Status={Status}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;

            }
            catch (Exception e)
            {

            }

        }
        #endregion
    }
}
