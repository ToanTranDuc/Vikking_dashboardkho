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
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmERPKeHoachDongThungTonKhoView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string URL = string.Empty;
        private string _maPKL = string.Empty, _madh = string.Empty, _maHang = string.Empty, _tenHang = string.Empty, _maDVSX = string.Empty, _maLenh = string.Empty, _dotSX = string.Empty, _po = string.Empty, _poid = string.Empty, _dausize = string.Empty, _mamau = string.Empty, _dausizeALL = string.Empty, _colorIDALL = string.Empty;
        private string _maDVSXL = string.Empty, _maDauSizeL = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        List<string> _listSize = new List<string>();
        DataTable _dtInit = new DataTable();
        DataTable _dtDVSX = new DataTable();
        Object _lstDauSize = new Object();
        Object _lstColor = new Object();
        DataTable _dtSize = new DataTable();
        DataTable dtDataPiVot = new DataTable();
        DataTable dtQuiCach = new DataTable();
        KeyDownControlHandler keyDownControlHandler;
        int sttThung_CreateNew = 0;
        public frmERPKeHoachDongThungTonKhoView()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            KHDongThungLib.InitQuiCach(repoQuiCach);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Init();
            LoadDonHang();
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            AddActionControl(_lstActionControl, BtnThemPODot, _allowAdd, ActionType.Add);
            AddActionControl(_lstActionControl, BtnSua, _allowEdit, ActionType.Edit);
            AddActionControl(_lstActionControl, BtnLuu, true, ActionType.Save);
            AddActionControl(_lstActionControl, BtnXuatExCel, true, ActionType.ExCel);
            AddActionControl(_lstActionControl, LoadKHDongThung, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, GopThung, true, ActionType.GopThung);
            AddActionControl(_lstActionControl, BtnXoa, _allowDelete, ActionType.Delete);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
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
        }
        private void Init()
        {
            searchLookUpEdit_MaPKL.Properties.DisplayMember = "MaPKLDisplay";
            searchLookUpEdit_MaPKL.Properties.ValueMember = "MaPKL";
            searchLookUpEdit_MaPKL.Properties.NullText = "[Chọn PKL]";

            searchLookUpEdit_DonHang.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEdit_DonHang.Properties.ValueMember = "MaDH";
            searchLookUpEdit_DonHang.Properties.NullText = "[Chọn đơn hàng]";
        }
        private void LoadDonHang()
        {
            string url = string.Format("{0}?", URL + $"KeHoachDongThungTonKho/Get?Action=GetDSDH&MaDH=DH00000064&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            var _dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_DonHang.Properties.DataSource = _dtDonHang;
        }
        private void LoadCTDonHang()
        {
            string url = string.Format("{0}", URL + $"KeHoachDongThungTonKho/Get?Action=GetCTDH&MaDH={_madh}&MaDVSX=Para&DotSX=Para&POID=Para&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var rowfocus = grvDonHang.FocusedRowHandle;
            var _dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
            if (rowfocus == 0) LoadDataWhenChangeDH();
            grcDonHang.DataSource = _dtDonHang;
            checkEditAll.Checked = true;
        }
        private void LoadKHDongThung()
        {
            _maPKL = searchLookUpEdit_MaPKL.EditValue == null ? "" : searchLookUpEdit_MaPKL.EditValue.ToString();
            //var _maDVSXTemp = _maDVSX.Substring(1, _maDVSX.Length - 2) + '@' + _maLenh;
            //if (checkEditAll.Checked) _maDVSXTemp = "All";
            var _maDVSXTemp = "All";
            string url = string.Format("{0}", URL + $"KeHoachDongThungTonKho/Get?Action=GetPivotKHDongThung&MaDH={_madh}&MaDVSX={_maDVSXTemp}&DotSX={_dotSX}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            dtDataPiVot = tbl.Copy();
            if (tbl == null || tbl.Rows.Count == 0)
                dgrKHDongThung.DataSource = new DataTable();
            else
            {

                DataTable dt = new DataTable();
                if (!checkEditAll.Checked)
                {
                    var tempdt = tbl.AsEnumerable().Where(x => x["MaDVSX"].ToString() + '@' + x["MaLenh"].ToString() == _maDVSXTemp);
                    tbl = tempdt.Count() == 0 ? new DataTable() : tempdt.CopyToDataTable();
                }
                CreateBandForSize(tbl);
                KHDongThungLib.ProcessSttTrung(tbl);
                dgrKHDongThung.DataSource = tbl;

                DataTable processedData = KHDongThungLib.sumToTalPCS(tbl);
                KHDongThungLib.CreateBandSize(processedData, dgwTong, gbSizeTotal, repotxtN0);
                dgcTong.DataSource = processedData;
                dgrKHDongThung.RefreshDataSource();
                if (tbl.Rows.Count == 0) return;
                sttThung_CreateNew = Convert.ToInt32(tbl.Rows[tbl.Rows.Count - 1]["SttThung"]) + 1;

            }


            //DataTable tbl = GetPivotKHDongThung();
            //bandedGridViewKHDT.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
            //btnLapKeHoachNhanh.Enabled = true;
        }
        private void SaveKHDT(DataTable tblPivot)
        {
            try
            {
                DataTable tblSave = KHDongThungLib.CreateTblSave();
                int TuThung_Notdeca = 0, _valTuThung = 1, _valDenThung = 0;
                DateTime dtNow = DateTime.Now;
                int TuThungOld = 0;
                int SttThungOld = 0;
                int SttThung_temp = 0;
                string _colorIDOld = "";
                foreach (DataRow dr in tblPivot.Rows)
                {
                    int slThung = Convert.ToInt32(dr["SLThung"]);
                    int TuThung = 0, DenThung = 0;
                    TuThung = Convert.ToInt32(dr["TuThung"]);
                    DenThung = Convert.ToInt32(dr["DenThung"]);
                    string _strSizeID = string.Empty, _strKyHieu_1 = string.Empty, _strKyHieu_2 = string.Empty;
                    var _colorID = dr["ColorID"].ToString();
                    if (_colorIDOld != _colorID)
                    {
                        SttThung_temp = 0;
                        _colorIDOld = _colorID;
                    }
                    foreach (DataColumn dc in tblPivot.Columns)
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
                            drNewRow["MaPKL"] = searchLookUpEdit_MaPKL.EditValue;
                            drNewRow["MaDH"] = _madh;
                            drNewRow["MaDVSX"] = dr["MaDVSX"];
                            drNewRow["DotSX"] = dr["DotSX"];
                            drNewRow["MaLenh"] = dr["MaLenh"];
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
                            drNewRow["SttThung"] = TuThung_Notdeca;
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
                            drNewRow["Cont"] = dr["KieuLap"];
                            drNewRow["KieuLap"] = "0";
                            drNewRow["KieuLapPCB"] = "0";
                            tblSave.Rows.Add(drNewRow);
                        }

                        _strSizeID = _sizeID;
                        _strKyHieu_1 = _strKyHieu_2;
                        TuThungOld = Convert.ToInt32(dr["TuThung"]);
                        SttThungOld = Convert.ToInt32(dr["SttThung"]);
                    }
                }
                //List<ErpPCBEntity> lst = JsonConvert.DeserializeObject<List<ErpPCBEntity>>(json);
                string url = string.Format("{0}", URL + "KeHoachDongThungTonKho/Post?action=InsertKHDT");
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
            string url = string.Format("{0}", URL + $"KeHoachDongThungTonKho/Get?Action=GetDotLapPKL&MaDH={_madh}&MaDVSX={_maDVSX}&DotSX=${_dotSX}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaPKL.Properties.DataSource = tbl;
            if (tbl.Rows.Count != 0) searchLookUpEdit_MaPKL.EditValue = tbl.Rows[0]["MaPKL"];
            else return;
            _maPKL = searchLookUpEdit_MaPKL.EditValue.ToString();
        }
        private void GopThung()
        {
            this.ActiveControl = grcDonHang;
            var data = dgrKHDongThung.DataSource as DataTable;
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
                    bandedGridViewKHDT.Columns.AddRange(new BandedGridColumn[] { col });
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
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
            //GetListSizeOfColor();
        }
        private void LoadDataWhenChangeDH()
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
            if (_madh != dr["MaDH"].ToString() || _poid != dr["POID"].ToString())
            {
                _madh = dr["MaDH"].ToString();
                _poid = dr["POID"].ToString();
                _po = dr["PO"].ToString();
                //LoadDVSX();
                GetDotPKL();
            }

            GetDataSize();
            LoadKHDongThung();
        }
        private void bandedGridViewKHDT_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            DXMenuItem menuDelete = new DXMenuItem();
            menuDelete.Caption = "Xóa";
            menuDelete.Click += MenuDelete_Click;
            DXMenuItem menuCopyandCreate = new DXMenuItem();
            menuCopyandCreate.Caption = "Copy và tạo dòng mới";
            menuCopyandCreate.Click += MenuCopyandCreate_Click;

            DXMenuItem menuCopyandCreateN = new DXMenuItem();
            menuCopyandCreateN.Caption = "Copy và tạo dòng mới phía dưới";
            menuCopyandCreateN.Click += MenuCopyandCreateN_Click;

            e.Menu.Items.Add(menuCopyandCreate);
            e.Menu.Items.Add(menuCopyandCreateN);
            e.Menu.Items.Add(menuDelete);

        }
        private void bandedGridViewKHDT_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value != null && (e.Value.ToString() == "0" || e.Value.ToString() == "")) e.DisplayText = "-";
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
                    if (TuThung == DenThung || DenThung == 0) drFocus["IsThungLe"] = true;
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
            //DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            //if (e.Column.FieldName == "TuThung" || e.Column.FieldName == "DenThung")
            //{

            //    int TuThung = drFocus["TuThung"].ToString() == "" ? 0 : Convert.ToInt32(drFocus["TuThung"]);
            //    int DenThung = drFocus["DenThung"].ToString() == "" ? 0 : Convert.ToInt32(drFocus["DenThung"]);
            //    if (TuThung == DenThung) drFocus["IsThungLe"] = true;
            //    else drFocus["IsThungLe"] = false;
            //    if (TuThung != 0 && DenThung != 0 && DenThung >= TuThung)
            //    {
            //        drFocus["SLThung"] = DenThung + 1 - TuThung;
            //        //if (CheckSTTThungInData(TuThung, DenThung))
            //        CheckSTTThungInGrid(TuThung, DenThung);
            //    }
            //    else
            //    {
            //        drFocus["SLThung"] = 0;
            //        btSave.Enabled = false;
            //    }

            //    int sumSL = 0;
            //    foreach (DataColumn dc in drFocus.Table.Columns)
            //    {
            //        if (!dc.ColumnName.Contains('@')) continue;
            //        sumSL += drFocus[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drFocus[dc.ColumnName]);
            //    }
            //    drFocus["SoLuong"] = sumSL;
            //    if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
            //        drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
            //}
            //else if (e.Column.FieldName.Contains("@") || e.Column.FieldName.Contains("SLThung"))
            //{
            //    int sumSL = 0;
            //    foreach (DataColumn dc in drFocus.Table.Columns)
            //    {
            //        if (!dc.ColumnName.Contains('@')) continue;
            //        sumSL += drFocus[dc.ColumnName].ToString() == "" ? 0 : Convert.ToInt32(drFocus[dc.ColumnName]);
            //    }
            //    drFocus["SoLuong"] = sumSL;
            //    if (drFocus["SLThung"].ToString() != "0" || drFocus["SLThung"].ToString() != "")
            //        drFocus["TotalPiece"] = sumSL * Convert.ToInt32(drFocus["SLThung"]);
            //    btSave.Enabled = true;
            //}
            //else if (e.Column.FieldName == "ChieuDai" || e.Column.FieldName == "ChieuRong" || e.Column.FieldName == "ChieuCao")
            //{
            //    double ChieuDai = drFocus["ChieuDai"].ToString() == "" ? 0 : Convert.ToDouble(drFocus["ChieuDai"]);
            //    double ChieuRong = drFocus["ChieuRong"].ToString() == "" ? 0 : Convert.ToDouble(drFocus["ChieuRong"]);
            //    double ChieuCao = drFocus["ChieuCao"].ToString() == "" ? 0 : Convert.ToDouble(drFocus["ChieuCao"]);
            //    drFocus["KyHieu"] = string.Format("{0}x{1}x{2}", ChieuDai, ChieuRong, ChieuCao);
            //    btSave.Enabled = true;
            //}
            //else if (e.Column.FieldName == "TrongLuong" || e.Column.FieldName == "KhoiLuong")
            //{
            //    var dt = dgrKHDongThung.DataSource as DataTable;
            //    KHDongThungLib.ProcessChangeNW_GW(dt, drFocus);
            //}
        }
        private void bandedGridViewKHDT_ShowingEditor(object sender, CancelEventArgs e)
        {

            if (bandedGridViewKHDT.FocusedColumn.FieldName == "TenDVSX" || bandedGridViewKHDT.FocusedColumn.FieldName == "DauSize" ||
                bandedGridViewKHDT.FocusedColumn.FieldName == "TenMau"
                )
            {
                if ((bool)bandedGridViewKHDT.GetFocusedRowCellValue("IsSave") == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }
            if (bandedGridViewKHDT.FocusedColumn.FieldName == "Chon")
            {
                if ((bool)bandedGridViewKHDT.GetFocusedRowCellValue("IsThungLe") == true)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;

                }
            }
            //e.Cancel = true;
        }
        private void bandedGridViewKHDT_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            e.Handled = KHDongThungLib.MergeAllowChangeValue(bandedGridViewKHDT, e);
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
            CopyData(true);
        }
        private void CopyData(bool flagInsert)
        {
            var drCopy = bandedGridViewKHDT.GetFocusedDataRow();
            DataTable tbl = dgrKHDongThung.DataSource as DataTable;
            if (tbl.Columns.Count == 0) return;
            int index = tbl.Rows.IndexOf(drCopy);
            DataRow drAdd = tbl.NewRow();
            drAdd["ID"] = 0;
            drAdd["POID"] = _poid;
            drAdd["PO"] = _po;
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
            drAdd["KyHieu"] = drCopy["KyHieu"];
            drAdd["IsSave"] = false;
            drAdd["Chon"] = false;
            drAdd["IsThungLe"] = flagInsert;
            drAdd["KyHieu"] = drCopy["KyHieu"];
            var sttThung = sttThung_CreateNew;
            drAdd["SttThung"] = sttThung + 1;
            drAdd["KieuLap"] = drCopy["KieuLap"];
            drAdd["KieuLapPCB"] = drCopy["KieuLapPCB"];
            drAdd["Stt_size"] = drCopy["Stt_size"];
            if (flagInsert) tbl.Rows.InsertAt(drAdd, index + 1);
            else tbl.Rows.Add(drAdd);
            dgrKHDongThung.RefreshDataSource();
            if (flagInsert) bandedGridViewKHDT.FocusedRowHandle = index + 1;
            else bandedGridViewKHDT.FocusedRowHandle = tbl.Rows.Count - 1;
            sttThung_CreateNew++;
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
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
                return;
            }
            frmERPKeHoachDongThungV2 fr = new frmERPKeHoachDongThungV2(_madh, _maHang);
            fr.ShowDialog();
            GetDotPKL();
            GetDataSize();
            LoadKHDongThung();
        }

        private void btnAddorEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnSua();
        }
        private void btnLapKeHoach_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnThemPODot();
        }
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtnXoa();
        }
        private void comboBoxEditPKLDot_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit_MaPKL_EditValueChanged(object sender, EventArgs e)
        {
            LoadKHDongThung();
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

        private void searchLookUpEdit_DonHang_EditValueChanged(object sender, EventArgs e)
        {
            var value = searchLookUpEdit_DonHang.EditValue;
            if (value is null) return;
            _madh = value.ToString();
            LoadCTDonHang();
        }

        private void bandedGridViewKHDT_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var IsThungLe = (bool)bandedGridViewKHDT.GetRowCellValue(e.RowHandle, "IsThungLe");
            if (IsThungLe) e.Appearance.BackColor = Color.FromArgb(255, 212, 128);
        }

        private void grvDonHang_DataSourceChanged(object sender, EventArgs e)
        {
            var dr = grvDonHang.GetFocusedDataRow() as DataRow;
            if (dr is null) return;
            _maDVSX = "'" + dr["MaDVSX"].ToString() + "'";
            _maLenh = dr["MaLenh"].ToString();
            _dotSX = dr["DotSX"].ToString();
            _maHang = dr["MaHang"].ToString();
            _tenHang = dr["TenHang"].ToString();
            if (_madh != dr["MaDH"].ToString() || _poid != dr["POID"].ToString())
            {
                //if (_madh != dr["MaDH"].ToString())
                //{

                //}
                _madh = dr["MaDH"].ToString();
                _poid = dr["POID"].ToString();
                _po = dr["PO"].ToString();
                //LoadDVSX();
                GetDotPKL();
            }

            GetDataSize();
            LoadKHDongThung();
        }

        private void GetDataSize()
        {
            var _maDVSXTemp = _maDVSX.Substring(1, _maDVSX.Length - 2);
            string url = string.Format("{0}", URL + $"KeHoachDongThungTonKho/Get?Action=GetDataSize&MaDH={_madh}&MaDVSX={_maDVSXTemp}&DotSX={_maLenh}&POID={_poid}&SizeTypeID=Para&ColorID=Para&ProductID=Para&SizeID=Para&MaPKL={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            dgrMauSize.DataSource = tbl;
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadCTDonHang();
        }

        private void xtraTabPage1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmERPKeHoachDongThungTonKhoView_Load(object sender, EventArgs e)
        {

        }

        private void repoQuiCach_EditValueChanged(object sender, EventArgs e)
        {
            var drfocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drfocus is null) return;
            SearchLookUpEdit sr = sender as SearchLookUpEdit;
            var quicach = sr.EditValue.ToString();
            //var str = drfocus["KyHieu"].ToString();

            //var dtTemp = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).CopyToDataTable();
            var TrongLuong = dtQuiCach.AsEnumerable().Where(x => x["MaQuiCach"].ToString() == quicach).FirstOrDefault()["TLThung"].ToString();
            var TotalPice = Convert.ToInt32(drfocus["TotalPiece"]);
            var SLThung = Convert.ToInt32(drfocus["SLThung"]);
            var KhoiLuong = Convert.ToDouble(drfocus["KhoiLuong"]);
            var value = Math.Round(KhoiLuong + SLThung * Convert.ToDouble(TrongLuong), 1);
            drfocus["TrongLuong"] = value;
            drfocus["KyHieu"] = quicach;
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            DataTable dtSource = dgrKHDongThung.DataSource as DataTable;
            frmChiTietThungTonBarcodeV3 frm = new frmChiTietThungTonBarcodeV3(dtSource.Rows[0], true, false, false, true);
            frm.ShowDialog();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var dtSource = dgrKHDongThung.DataSource as DataTable;
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
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
            KHDongThungLib.TinhToanLaiKhiXoa(dtSource, -1, false, "");
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var dtSource = dgrKHDongThung.DataSource as DataTable;
            DataRow drFocus = bandedGridViewKHDT.GetFocusedDataRow();
            if (drFocus is null) return;

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
            KHDongThungLib.TinhToanLaiKhiXoa(dtSource, -1, false, "");
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
        private bool BetweenInt(int a, int minValue, int maxValue)
        {
            if (a >= minValue && a <= maxValue) return true;
            else return false;
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
            tbl.Columns.Add("IsThungLe", typeof(int));
            return tbl;
        }
        #region  Manh
        private void btnEX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        private void ExportExcel(string path, DataTable dtPKLXuatHang)
        {
            try
            {
                string PathLoGo = KHDongThungLib.getImgPath("texgiang.jpg");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"{_maPKL}_{_maHang}");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "PACKING LIST"; range.Style.Font.Bold = true;
                    int colum = 6;
                    //range = worksheet.Cells["A1:B3"]; range.Merge = true;
                    //Image image = Image.FromFile(PathLoGo);
                    //OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                    //picture.SetPosition(0, 0, 0, 0);
                    //picture.SetSize(105, 55);
                    worksheet.Cells.Style.Font.Size = 11;
                    range = worksheet.Cells["D4"]; range.Value = "STYLE :";
                    range = worksheet.Cells["e4:f4"]; range.Merge = true; range.Value = _tenHang;
                    range = worksheet.Cells["D5"]; range.Value = "Shipper: :";
                    range = worksheet.Cells["D6"]; range.Value = "Invoice No : ";
                    range = worksheet.Cells["D7"]; range.Value = "Consignee : ";
                    KHDongThungLib.dtXuatEX(dtPKLXuatHang, worksheet);
                    excelPackage.SaveAs(file);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }
        private static void MergeCellsByRow(ExcelWorksheet worksheet, int colum, int startrow, int endrow, string align)
        {
            worksheet.Cells[startrow, colum, endrow, colum].Merge = true;

            using (var range = worksheet.Cells[startrow, colum, endrow, colum])
            {
                if (align == "center")
                {
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                range.Style.WrapText = true;
            }
        }
        private string getImgPath(string Img)
        {
            string Paths = Directory.GetCurrentDirectory();
            return $"{Directory.GetParent(Paths).Parent.FullName}\\Resources\\{Img}";

        }
        private void bandedGridViewKHDT_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var dt = dgrKHDongThung.DataSource as DataTable;
            KHDongThungLib.SumGroup(dt, e);
        }

        private void bandedGridViewKHDT_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLib.Merge(sender, e);
        }
        private void BtnThemPODot()
        {
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
            }
            frmERPKeHoachDongThungTonKhoV3 fr = new frmERPKeHoachDongThungTonKhoV3(_madh, _maHang, _poid, "", _tenHang);
            fr.ShowDialog();
            GetDotPKL();
            GetDataSize();
            LoadKHDongThung();
        }

        private void BtnSua()
        {
            if (string.IsNullOrEmpty(_madh))
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để lập PKL");
            }
            frmERPKeHoachDongThungTonKhoV3 fr = new frmERPKeHoachDongThungTonKhoV3(_madh, _maHang, _poid, _maPKL, _tenHang);
            fr.ShowDialog();
            //GetDotPKL();
            GetDataSize();
            LoadKHDongThung();
        }

        private void BtnXoa()
        {
            DialogResult resultDialog = MessageBox.Show("Xác nhận xóa package list?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDialog == DialogResult.Yes)
            {
                DataTable dtSave = KHDongThungLib.CreateTblSave();
                var drnew = dtSave.NewRow();
                drnew["ID"] = 0;
                drnew["MaPKL"] = _maPKL;
                drnew["MaDH"] = _madh;
                drnew["POID"] = _poid;
                drnew["NVien"] = GlobleData.UserName;
                dtSave.Rows.Add(drnew);
                string url = string.Format("{0}", URL + "KeHoachDongThungTonKho/Post?action=DeleteKHDT");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                LoadCTDonHang();
            }
        }

        private void BtnLuu()
        {
            this.ActiveControl = grcDonHang;
            DataTable dt = dgrKHDongThung.DataSource as DataTable;
            SaveKHDT(dt);
        }
        private void BtnXuatExCel()
        {
            DataTable tblXuatEX = dgrKHDongThung.DataSource as DataTable;
            if (tblXuatEX == null)
            {
                MessageBox.Show("Dữ liệu rỗng !");
                return;
            };
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            Sfd.FileName = string.Format("PKL_{0}" + DateTime.Now.ToString("ddMMyyyy"), _maHang);
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
                    ExportExcel(Sfd.FileName, tblXuatEX);
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



        #endregion
    }
}
