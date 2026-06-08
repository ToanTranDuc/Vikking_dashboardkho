using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Properties;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using NtbSoft.ERP.Win.Modules.ResourceForm;
using DevExpress.XtraSplashScreen;
using System.Threading;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid;
using DevExpress.Data;
using NtbSoft.ERP.Win.Service.SYSTEM;
using DevExpress.XtraEditors.Controls;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Win.Service.ERP.QuanLyDonHang;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace NtbSoft.ERP.Win.Modules.Erp.QuanLyDonHang
{
    public partial class frmTNC_QLSX_View : DevExpress.XtraEditors.XtraForm
    {

        HttpClientExtension _clientExtension = new HttpClientExtension();

        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        string URL_DH = string.Empty;
        int ValStatus_T, ValSTT = 0; double _vlSumTieuHao = 0, _valueSum = 0, _valueConLai = 0, _valueLK = 0;
        private string _maLenh = string.Empty, _mahang = string.Empty, _nhomNPL = string.Empty, IDColor = string.Empty, ColorTong = string.Empty, MaMau = string.Empty,
                StrSizeType = string.Empty, _StrSizeTypeID = string.Empty, _StrSPOID = string.Empty, _StrPO = string.Empty,
                _StrMaMau = string.Empty, _StrTenMau = string.Empty, _MaVai = string.Empty, _maKH = string.Empty, _mauvai = string.Empty, _ID_DMNL = string.Empty, _codeTNC = string.Empty;

        public bool _allowAdd = true, _allowEdit = true, _allowDelete = true, _allowMess = false, _checkKHSX = false, _IsClose = true, IsCheckClose = false, IsMoveRow = false, _isRebuildingColumns = true;

        SystemLogService _serviceLog;
        SystemUserModuleService _serviceUserModule;
        ErpNhap_TNCService _serviceNhapTNC;


        int _rowAdd = -1;
        ArrayList _listRow;
        DataTable _dtData;
        DataTable _dtSave;
        DataTable dtNPL;
        DataTable dtSLDM;
        DataTable _dtSumKH;
        DataTable dtKH;
        DataTable dtSize;

        string _DetailNotify = string.Empty;

        DataRow _rowFocusedMaLenh;
        DataRow _rowFocusedCapPhat;
        DataTable _tblLenhPO;


        public frmTNC_QLSX_View(DataRow rowFocusedMaLenh, DataRow rowFocusedCapPhat, DataTable tblLenhPO)
        {
            InitializeComponent();

            URL = (string)settingsReader.GetValue("URL_QLSX", typeof(String));

            URL_DH = (string)settingsReader.GetValue("URL", typeof(String));
            _dtData = new DataTable();
            _dtSave = new DataTable();
            dtSLDM = new DataTable();
            dtNPL = new DataTable();
            dtKH = new DataTable();
            _dtSumKH = new DataTable();
            dtSize = new DataTable();

            _rowFocusedMaLenh = rowFocusedMaLenh;
            _rowFocusedCapPhat = rowFocusedCapPhat;
            _tblLenhPO = tblLenhPO;

            _serviceUserModule = new SystemUserModuleService();
            _serviceNhapTNC = new ErpNhap_TNCService();

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetTNC();
            CreateDefault();
            XacNhanConfig();
        }



        private void XacNhanConfig()
        {
            gridView1.OptionsBehavior.Editable = true;
            _valueLK = 0;

            //listHangHoa = Task.Run(async () => { return await _serviceHangHoa.HangHoaGet(string.Format(URL + ResourceURL.UrlErpHangHoa + "/Get?maHang={0}", _maLenh)); }).Result;

            string urlVai = string.Format("{0}/GetSize?malenh={1}&&sizetypeID={2}&&spoid={3}&&colorid={4}", URL + "ErpNhap_TNC", _maLenh, _StrSizeTypeID, _StrSPOID, _StrMaMau);
            dtSize = Task.Run(async () => { return await _serviceNhapTNC.GetNPL(urlVai); }).Result;
            string _StrSize = string.Empty;
            bool AllowAddColum = true;
            int _valuIndedx = 3;
            if (chk_TachNhomSize.Checked)
            {
                string sizeTypeOrder = _StrSizeTypeID;
                var orderMap = sizeTypeOrder
                                        .Split(';')
                                        .Select((v, i) => new { v, i })
                                        .ToDictionary(x => x.v, x => x.i);
                if (!dtSize.Columns.Contains("SizeTypeOrder"))
                    dtSize.Columns.Add("SizeTypeOrder", typeof(int));
                foreach (DataRow row in dtSize.Rows)
                {
                    string sizeTypeID = row["SizeTypeID"].ToString();

                    row["SizeTypeOrder"] = orderMap.ContainsKey(sizeTypeID)
                        ? orderMap[sizeTypeID]
                        : int.MaxValue;   // các SizeTypeID không nằm trong chuỗi → đẩy xuống cuối
                }
                DataView dv = dtSize.DefaultView;
                dv.Sort = "SizeTypeOrder ASC";

                dtSize = dv.ToTable();
                dtSize.Columns.Remove("SizeTypeOrder");
            }
            _isRebuildingColumns = false;
            if (_dtData.Columns.Count > 15)
            {
                for (int i = _dtData.Columns.Count - 1; i > 17; i--)
                {
                    _dtData.Columns.RemoveAt(i);
                }
                _dtData.Clear();
                AddRowEmty();
                gridView1.BeginUpdate();
                for (int i = gridView1.Columns.Count - 1; i >= 0; i--)
                {
                    if (gridView1.Columns[i].FieldName.Contains("@"))
                    {
                        gridView1.Columns.RemoveAt(i);
                    }
                }
                gridView1.EndUpdate();
            }
            _isRebuildingColumns = true;
            if (!chk_TachNhomSize.Checked)
            {
                foreach (DataRow row in dtSize.Rows)
                {
                    row["SizeTypeID"] = DBNull.Value;   // hoặc 0 nếu là số
                    row["SizeType"] = DBNull.Value;    // hoặc string.Empty
                }

                // Loại bỏ các dòng trùng nhau
                dtSize = dtSize.DefaultView.ToTable(true);
            }
            for (int i = 0; i < dtSize.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(_StrSize))
                {
                    _StrSize = dtSize.Rows[i]["Size"].ToString();
                }
                else
                {
                    _StrSize = _StrSize + "; " + dtSize.Rows[i]["Size"].ToString();
                }
                if (!ColumnExists(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString())/*_dtData.Columns.Count < dtSize.Rows.Count + 12*/)
                {
                    AllowAddColum = true;
                    if (!chk_TachNhomSize.Checked)
                    {
                        _dtData.Columns.Add(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString(), typeof(int));
                    }
                    else
                    {
                        _dtData.Columns.Add(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString() + "@" + dtSize.Rows[i]["SizeTypeID"].ToString(), typeof(int));
                    }
                }
                else
                {
                    AllowAddColum = false;
                }
                if (AllowAddColum)
                {
                    //gridView1.Columns.Add(new GridColumn { FieldName = dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["Size"].ToString(), Caption = dtSize.Rows[i]["Size"].ToString(), Name = "Size@" + dtSize.Rows[i]["Size"].ToString(), Visible = true, Width = 50 });
                    int columnIndexToInsert = 3; // Vị trí cột cần chèn (cột thứ 4)
                    _valuIndedx = _valuIndedx + 1;
                    if (!chk_TachNhomSize.Checked)
                    {
                        GridColumn newColumn = new GridColumn
                        {
                            FieldName = dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString(),
                            Caption = dtSize.Rows[i]["Size"].ToString(),
                            Name = "Size@" + dtSize.Rows[i]["Size"].ToString(),
                            Visible = true,
                            //Width = 50,
                            VisibleIndex = _valuIndedx,

                        };
                        //----------------------------
                        int autoWidth = CalcColumnWidthByCaption(gridView1, newColumn.Caption);
                        newColumn.Width = autoWidth < 50 ? 50 : autoWidth;
                        string[] arrName = newColumn.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrName.Length > 1)
                        {
                            //newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Sum, newColumn.FieldName, slth);
                            string tag1 = newColumn.FieldName + "|TT";
                            GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                            item1.Tag = tag1;
                            string tag2 = newColumn.FieldName + "|RM";
                            GridColumnSummaryItem item2 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                            item2.Tag = tag2;
                        }
                        //----------------------------
                        gridView1.Columns.Insert(columnIndexToInsert, newColumn);
                    }
                    else
                    {
                        GridColumn newColumn = new GridColumn
                        {
                            FieldName = dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString() + "@" + dtSize.Rows[i]["SizeTypeID"].ToString(),
                            Caption = dtSize.Rows[i]["Size"].ToString() + "|" + dtSize.Rows[i]["SizeType"].ToString(),
                            Name = "Size@" + dtSize.Rows[i]["Size"].ToString() + "|" + dtSize.Rows[i]["SizeType"].ToString(),
                            Visible = true,
                            //Width = 50,
                            VisibleIndex = _valuIndedx,

                        };
                        string[] arrName = newColumn.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        int autoWidth = CalcColumnWidthByCaption(gridView1, newColumn.Caption);
                        newColumn.Width = autoWidth < 50 ? 50 : autoWidth;
                        if (arrName.Length > 1)
                        {
                            //newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Sum, newColumn.FieldName, slth);
                            string tag1 = newColumn.FieldName + "|TT";
                            GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                            item1.Tag = tag1;
                            string tag2 = newColumn.FieldName + "|RM";
                            GridColumnSummaryItem item2 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                            item2.Tag = tag2;
                        }
                        //----------------------------
                        gridView1.Columns.Insert(columnIndexToInsert, newColumn);
                    }

                }
            }
            //gridView1.Columns.Clear();
            gridView1.Columns.AddRange(gridView1.Columns.OrderBy(c => c.VisibleIndex).ToArray());
            lblSize.Text = "Size: " + _StrSize;
            if (_allowAdd)
            {
                colSoDo.Summary.Clear();
                colSoDo.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "SoDo", "Tổng");
                colSoDo.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "SoDo", "Còn lại");
                _allowAdd = false;
            }
            LoadDataDetail();
            //LoadData();
            _dtSumKH.Clear();
            LoadData();
            gridView1.RefreshData();
        }



        private void SetTNC()
        {
            _maLenh = _rowFocusedMaLenh["MaLenh"]?.ToString();
            _mahang = _rowFocusedMaLenh["MaHang"]?.ToString();
            //this._nhomNPL = nhomNPL;
            //this._MaVai = mavai;
            this._maKH = _rowFocusedMaLenh["MaKH"]?.ToString();
            this._ID_DMNL = _rowFocusedCapPhat["IDVai"]?.ToString();
            this._codeTNC = _rowFocusedCapPhat["Code_TNC"]?.ToString();
            tbKhachHang.Text = _rowFocusedMaLenh["TenKH"]?.ToString();
            tbKhoVai.EditValue = _rowFocusedCapPhat["KhoVai"]?.ToString(); ;



            string url = $"{URL_DH}ERPDonHangTong/Get?action=GetSPOID&para={_codeTNC}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                _StrSPOID = string.Join(";", tbl.AsEnumerable().Select(x => x["SPOID"]));
                _StrPO = string.Join(";", tbl.AsEnumerable().Select(x => x["PO"]?.ToString()).Where(x => !string.IsNullOrEmpty(x)).Distinct());

                _StrMaMau = string.Join(";", tbl.AsEnumerable().Select(x => x["MaMau"]?.ToString()).Where(x => !string.IsNullOrEmpty(x)).Distinct());
                _StrTenMau = string.Join(";", tbl.AsEnumerable().Select(x => x["TenMau"]?.ToString()).Where(x => !string.IsNullOrEmpty(x)).Distinct());
                _StrSizeTypeID = string.Join(";", tbl.AsEnumerable().Select(x => x["DauSizeID"]?.ToString()).Where(x => !string.IsNullOrEmpty(x)).Distinct());
                StrSizeType = string.Join(";", tbl.AsEnumerable().Select(x => x["DauSize"]?.ToString()).Where(x => !string.IsNullOrEmpty(x)).Distinct());
            }


            txtColor.EditValue = _StrTenMau;
            txtDauSize.EditValue = StrSizeType;
            txtVai.EditValue = $"{_rowFocusedCapPhat["MaVT"]} - {_rowFocusedCapPhat["ChiTiet"]?.ToString()}";
            tbDMKH.EditValue = _rowFocusedCapPhat["DMKH"]?.ToString();
            txtPO.EditValue = _StrPO;
        }
        private void CreateDefault()
        {


            tbSL_BanCat.Text = "1000";
            txtLenhSX.Text = _maLenh;
            tbMaHang.Text = _mahang;
            //CkbKHSX.Enabled = Convert.ToInt32(_nhomNPL) == 1 ? true : false;
            lbNote.Text = "<color=Blue>Lưu ý: dùng dấu ' ; ' phân cách giữ 2 size, dùng dấu ' * ' giữa size và số lần đi sơ đồ" + Environment.NewLine + " Ví dụ:  S*1;M*2;L*2;XL*1</color>";

            if (_dtData.Columns.Count == 0)
            {
                _dtData = new DataTable("dtData");
                _dtData.Columns.Add("STT", typeof(int));
                _dtData.Columns.Add("SoDo", typeof(string));
                _dtData.Columns.Add("SoLop", typeof(string));
                _dtData.Columns.Add("SoLuong", typeof(double));
                _dtData.Columns.Add("SoBo", typeof(double));
                _dtData.Columns.Add("Dai", typeof(double));
                _dtData.Columns.Add("Rong", typeof(double));
                _dtData.Columns.Add("Mau", typeof(string));
                _dtData.Columns.Add("MoTa", typeof(string));
                _dtData.Columns.Add("IsSave", typeof(int));
                _dtData.Columns.Add("LuyKe", typeof(double));
                _dtData.Columns.Add("Dai_DB", typeof(double));
                _dtData.Columns.Add("TieuHao", typeof(double));
                _dtData.Columns.Add("IsTachBan", typeof(int));
                _dtData.Columns.Add("SLSoDo", typeof(int));
                _dtData.Columns.Add("TT_TenSD", typeof(string));
                _dtData.Columns.Add("DMTT", typeof(double));
                _dtData.Columns.Add("CheckDuyetSD", typeof(bool));
            }


            gridControl1.DataSource = _dtData;
            //colSoDo.OptionsColumn.AllowEdit = false;
        }

        private async void LoadData()
        {


            string urlVai = string.Format("{0}/GetSLDM?malenh={1}&&sizetypeID={2}&&spoid={3}&&colorid={4}", URL + "ErpNhap_TNC", _maLenh, HttpUtility.UrlEncode(_StrSizeTypeID), HttpUtility.UrlEncode(_StrSPOID), HttpUtility.UrlEncode(_StrMaMau));
            dtSLDM = new DataTable();
            dtSLDM = Task.Run(async () => { return await _serviceNhapTNC.GetNPL(urlVai); }).Result;

            gridControl2.MainView = GetBandGridViewAmount(dtSLDM);
            BandedGridView mainView = (BandedGridView)gridControl2.MainView;
            //mainView.CellValueChanged += mainView_CellValueChanged;
            mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
            mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;

            gridControl2.DataSource = dtSLDM;


        }
        private async void LoadDataDetail()
        {
            if (_dtData.Rows.Count == 0)
            {
                AddNewColumn();

            }


            gridControl1.DataSource = _dtData;
            //gridView1.CustomDrawFooterCell += gridView1_CustomDrawFooterCell;
            gridView1.CustomSummaryCalculate += gridView1_CustomSummaryCalculate;
            TinhTongDM();
            CheckAllowDuyetSD();
        }


        private void AddNewColumn()
        {
            try
            {
                gridView1.OptionsBehavior.Editable = false;

                string urlSD = string.Format("{0}/GetSoDoEdit?codetnc={1}", URL + "ErpNhap_TNC", _codeTNC);
                DataTable dtSoDo = Task.Run(async () => { return await _serviceNhapTNC.GetNPL(urlSD); }).Result;
                if (dtSoDo == null || dtSoDo?.Rows?.Count == 0) return;


                CkbKHSX.Checked = Convert.ToBoolean(dtSoDo.Rows[0]["IsKHSX"]);
                chk_TachNhomSize.Checked = Convert.ToBoolean(dtSoDo.Rows[0]["TachNhomSize"]);

                ;
                string urlVai = string.Format("{0}/GetSize?malenh={1}&&sizetypeID={2}&&spoid={3}&&colorid={4}", URL + "ErpNhap_TNC", _maLenh, HttpUtility.UrlEncode(_StrSizeTypeID), HttpUtility.UrlEncode(_StrSPOID), HttpUtility.UrlEncode(_StrMaMau));
                dtSize = Task.Run(async () => { return await _serviceNhapTNC.GetNPL(urlVai); }).Result;
                if (dtSize == null || dtSize?.Rows?.Count == 0) return;
                string _StrSize = string.Empty;
                bool AllowAddColum = true;
                int _valuIndedx = 2;

                if (chk_TachNhomSize.Checked)
                {
                    string sizeTypeOrder = _StrSizeTypeID;
                    var orderMap = sizeTypeOrder
                                            .Split(';')
                                            .Select((v, i) => new { v, i })
                                            .ToDictionary(x => x.v, x => x.i);
                    if (!dtSize.Columns.Contains("SizeTypeOrder"))
                        dtSize.Columns.Add("SizeTypeOrder", typeof(int));
                    foreach (DataRow row in dtSize.Rows)
                    {
                        string sizeTypeID = row["SizeTypeID"].ToString();

                        row["SizeTypeOrder"] = orderMap.ContainsKey(sizeTypeID)
                            ? orderMap[sizeTypeID]
                            : int.MaxValue;   // các SizeTypeID không nằm trong chuỗi → đẩy xuống cuối
                    }
                    DataView dv = dtSize.DefaultView;
                    dv.Sort = "SizeTypeOrder ASC";

                    dtSize = dv.ToTable();
                    dtSize.Columns.Remove("SizeTypeOrder");
                }

                tbKhoVai1.Text = dtSoDo.Rows[0]["KhoVai"].ToString();
                txtSoLop_PCS.Text = dtSoDo.Rows[0]["SoLop_pcs"].ToString();
                tbDungSaiDM.Text = dtSoDo.Rows[0]["HaoHutDM"].ToString();
                tbGhiChu.Text = dtSoDo.Rows[0]["GhiChu"].ToString();

                if (_dtData.Columns.Count > 15)
                {
                    for (int i = _dtData.Columns.Count - 1; i > 17; i--)
                    {
                        _dtData.Columns.RemoveAt(i);
                    }
                    _dtData.Clear();
                    AddRowEmty();
                    gridView1.BeginUpdate();
                    for (int i = gridView1.Columns.Count - 1; i >= 0; i--)
                    {
                        if (gridView1.Columns[i].FieldName.Contains("@"))
                        {
                            gridView1.Columns.RemoveAt(i);
                        }
                    }
                    gridView1.EndUpdate();
                }
                _isRebuildingColumns = true;
                if (!chk_TachNhomSize.Checked)
                {
                    foreach (DataRow row in dtSize.Rows)
                    {
                        row["SizeTypeID"] = DBNull.Value;   // hoặc 0 nếu là số
                        row["SizeType"] = DBNull.Value;    // hoặc string.Empty
                    }

                    // Loại bỏ các dòng trùng nhau
                    dtSize = dtSize.DefaultView.ToTable(true);
                }

                for (int i = 0; i < dtSize.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(_StrSize))
                    {
                        _StrSize = dtSize.Rows[i]["Size"].ToString();
                    }
                    else
                    {
                        _StrSize = _StrSize + "; " + dtSize.Rows[i]["Size"].ToString();
                    }
                    if (!ColumnExists(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString())/*_dtData.Columns.Count <= dtSize.Rows.Count + 12*/)
                    {
                        AllowAddColum = true;
                        //_dtData.Columns.Add(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString(), typeof(int));
                        if (!chk_TachNhomSize.Checked)
                        {
                            _dtData.Columns.Add(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString(), typeof(double));
                        }
                        else
                        {
                            _dtData.Columns.Add(dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString() + "@" + dtSize.Rows[i]["SizeTypeID"].ToString(), typeof(double));
                        }
                    }
                    else
                    {
                        AllowAddColum = false;
                    }
                    if (AllowAddColum)
                    {
                        //gridView1.Columns.Add(new GridColumn { FieldName = dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["Size"].ToString(), Caption = dtSize.Rows[i]["Size"].ToString(), Name = "Size@" + dtSize.Rows[i]["Size"].ToString(), Visible = true, Width = 50 });
                        int columnIndexToInsert = 3; // Vị trí cột cần chèn (cột thứ 4)
                        _valuIndedx = _valuIndedx + 1;
                        if (!chk_TachNhomSize.Checked)
                        {
                            GridColumn newColumn = new GridColumn
                            {
                                FieldName = dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString(),
                                Caption = dtSize.Rows[i]["Size"].ToString(),
                                Name = "Size@" + dtSize.Rows[i]["SizeID"].ToString(),
                                Visible = true,
                                //Width = 50,
                                VisibleIndex = _valuIndedx,

                            };
                            //----------------------------


                            var slkh = "";
                            var slth = "";

                            int autoWidth = CalcColumnWidthByCaption(gridView1, newColumn.Caption);
                            newColumn.Width = autoWidth < 50 ? 50 : autoWidth;
                            string[] arrName = newColumn.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                            if (arrName.Length > 1)
                            {
                                //slkh = "0";
                                //slth = "{0:n0}";

                                //newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Sum, newColumn.FieldName, slth);
                                string tag1 = newColumn.FieldName + "|TT";
                                GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                                item1.Tag = tag1;
                                string tag2 = newColumn.FieldName + "|RM";
                                GridColumnSummaryItem item2 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                                item2.Tag = tag2;
                            }
                            //----------------------------
                            gridView1.Columns.Insert(columnIndexToInsert, newColumn);
                        }
                        else
                        {
                            GridColumn newColumn = new GridColumn
                            {
                                FieldName = dtSize.Rows[i]["Size"].ToString() + "@" + dtSize.Rows[i]["SizeID"].ToString() + "@" + dtSize.Rows[i]["SizeTypeID"].ToString(),
                                Caption = dtSize.Rows[i]["Size"].ToString() + "|" + dtSize.Rows[i]["SizeType"].ToString(),
                                Name = "Size@" + dtSize.Rows[i]["Size"].ToString() + "|" + dtSize.Rows[i]["SizeType"].ToString(),
                                Visible = true,
                                //Width = 50,
                                VisibleIndex = _valuIndedx,

                            };
                            //----------------------------


                            var slkh = "";
                            var slth = "";

                            int autoWidth = CalcColumnWidthByCaption(gridView1, newColumn.Caption);
                            newColumn.Width = autoWidth < 50 ? 50 : autoWidth;
                            string[] arrName = newColumn.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                            if (arrName.Length > 1)
                            {
                                //slkh = "0";
                                //slth = "{0:n0}";

                                //newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Sum, newColumn.FieldName, slth);
                                string tag1 = newColumn.FieldName + "|TT";
                                GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                                item1.Tag = tag1;
                                string tag2 = newColumn.FieldName + "|RM";
                                GridColumnSummaryItem item2 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                                item2.Tag = tag2;
                            }
                            //----------------------------
                            gridView1.Columns.Insert(columnIndexToInsert, newColumn);
                        }

                    }
                }
                //gridView1.Columns.Clear();
                gridView1.Columns.AddRange(gridView1.Columns.OrderBy(c => c.VisibleIndex).ToArray());
                lblSize.Text = "Size: " + _StrSize;
                colSoDo.Summary.Clear();
                colSoDo.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "SoDo", "Tổng");
                colSoDo.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "SoDo", "Còn lại");
                //-- add dư liệu cũ vào
                if (dtSoDo.Rows.Count > 0)
                {
                    double _valLK = 0;
                    for (int i = 0; i < dtSoDo.Rows.Count; i++)
                    {

                        ValSTT++;
                        DataRow _dr = _dtData.NewRow();
                        _dr["STT"] = dtSoDo.Rows[i]["STT"].ToString();
                        _dr["SoDo"] = dtSoDo.Rows[i]["SoDo"].ToString();
                        _dr["SoLop"] = dtSoDo.Rows[i]["SoLop"].ToString();
                        _dr["SoLuong"] = Convert.ToDouble(dtSoDo.Rows[i]["SoLuong"].ToString()) * Convert.ToDouble(dtSoDo.Rows[i]["SLSoDo"]) / Convert.ToDouble(dtSoDo.Rows[i]["SoLop_pcs"]);
                        _dr["Dai"] = dtSoDo.Rows[i]["dai"].ToString();
                        _dr["Rong"] = dtSoDo.Rows[i]["rong"].ToString();
                        _dr["Mau"] = dtSoDo.Rows[i]["Mau"].ToString();
                        _dr["MoTa"] = dtSoDo.Rows[i]["MoTa"].ToString();
                        _dr["IsSave"] = dtSoDo.Rows[i]["IsSave"].ToString();
                        _valLK += Convert.ToDouble(dtSoDo.Rows[i]["SoLuong"]) / Convert.ToDouble(dtSoDo.Rows[i]["SoLop_pcs"]);
                        _dr["LuyKe"] = _valLK;
                        _dr["Dai_DB"] = dtSoDo.Rows[i]["dai_db"].ToString();
                        _dr["TieuHao"] = Convert.ToDouble(dtSoDo.Rows[i]["dai_db"]) * Convert.ToDouble(dtSoDo.Rows[i]["SoLop"]) * Convert.ToInt32(dtSoDo.Rows[i]["SLSoDo"]) /*/ Convert.ToDouble(dtSoDo.Rows[i]["SoLop_pcs"])*/;
                        _dr["SoBo"] = dtSoDo.Rows[i]["SoLan"].ToString();
                        _dr["IsTachBan"] = dtSoDo.Rows[i]["IsTachBan"].ToString();
                        _dr["TT_TenSD"] = dtSoDo.Rows[i]["TT_TenSD"].ToString();
                        _dr["SLSoDo"] = dtSoDo.Rows[i]["SLSoDo"].ToString();
                        _dr["DMTT"] = dtSoDo.Rows[i]["DMTT"].ToString();
                        _dr["CheckDuyetSD"] = dtSoDo.Rows[i]["CheckDuyetSD"].ToString();
                        //-- xử lý sodo
                        for (int z = 0; z < _dtData.Columns.Count; z++)
                        {
                            string[] arrSDName = dtSoDo.Rows[i]["SoDo"].ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            bool contains = _dtData.Columns[z].ColumnName.Contains("@");
                            if (contains)
                            {
                                for (int k = 0; k < arrSDName.Length; k++)
                                {
                                    string[] arrSDSize = arrSDName[k].Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                                    // kiểm tra và lấy sizeID trong danh sách size
                                    DataRow rowFoundSizeID = null;
                                    string[] arrDTSType = new string[] { string.Empty };
                                    if (chk_TachNhomSize.Checked)
                                    {
                                        arrDTSType = arrSDSize[0].Split(new char[] { '|' });
                                        rowFoundSizeID = dtSize.AsEnumerable()
                                                                .FirstOrDefault(r =>
                                                                    r.Field<string>("Size") == arrDTSType[0] &&
                                                                    r.Field<string>("SizeType") == arrDTSType[1]);
                                    }
                                    else
                                    {
                                        rowFoundSizeID = dtSize.AsEnumerable()
                                                                    .FirstOrDefault(r => r["Size"].ToString() == arrSDSize[0].ToString());

                                    }
                                    //DataRow rowFoundSizeID = dtSize.AsEnumerable()
                                    //.FirstOrDefault(r => r["Size"].ToString() == arrSDSize[0].ToString());
                                    if (rowFoundSizeID != null)
                                    {
                                        var valSizeID = rowFoundSizeID["SizeID"].ToString();
                                        //string _strSizename = arrSDSize[0].ToString().Trim() + "@" + valSizeID.Trim();
                                        string _strSizename = string.Empty;
                                        if (chk_TachNhomSize.Checked)
                                        {
                                            _strSizename = arrDTSType[0].ToString().Trim() + "@" + valSizeID.Trim() + "@" + arrDTSType[1];
                                        }
                                        else
                                        {
                                            _strSizename = arrSDSize[0].ToString().Trim() + "@" + valSizeID.Trim();
                                        }
                                        if (_strSizename.Trim() == _dtData.Columns[z].ColumnName.ToString())
                                        {
                                            _dr[_dtData.Columns[z].ColumnName] = arrSDSize[1].Trim();
                                        }
                                    }

                                }
                            }
                        }
                        _dtData.Rows.Add(_dr);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void AddRowEmty()
        {
            gridView1.OptionsBehavior.Editable = false;
            if (_dtData.Columns.Count > 0 && _dtData.Rows.Count > 0)
            {
                if (!_dtData.AsEnumerable().Any(x => x["SoDo"].ToString() == "" && x["SoLop"].ToString() == ""))
                {
                    DataRow _dr = _dtData.NewRow();
                    ValSTT = ValSTT + 1;
                    _dr["STT"] = ValSTT;
                    _dr["Dai"] = 0;
                    _dr["Rong"] = 0;
                    _dr["Dai_DB"] = 0;
                    _dr["SLSoDo"] = 1;
                    _dtData.Rows.Add(_dr);
                    //TinhTongDM();
                }
            }
        }
        private void CheckAllowDuyetSD()
        {
            if (_dtData.Rows.Count > 0)
            {
                bool _blKTDuyet = Convert.ToBoolean(_dtData.Rows[0]["CheckDuyetSD"]);
                if (_blKTDuyet)
                {
                    //gridControl1.Enabled = false;
                    gridView1.OptionsBehavior.ReadOnly = true;
                    btSave.Enabled = false;
                    lbNote.Text = "<color=Red>⚠️Chú ý: sơ đồ đã được duyệt, không thể thực hiện chỉnh sửa</color>"; // tô đỏ
                    lbNote.Appearance.Font = new Font(lbNote.Appearance.Font.FontFamily, 14, FontStyle.Bold);

                }
            }

        }
        private string BuildSizeKey(string baseKey)
        {
            string[] arrSize = baseKey.Split('@');

            if (chk_TachNhomSize.Checked)
            {
                if (arrSize.Length != 3)
                    return string.Empty;

                return $"{arrSize[0]}@Size@{arrSize[1]}@{arrSize[2]}";
            }

            return $"{arrSize[0]}@Size@{arrSize[1]}";
        }
        private void TinhTongDM()
        {

            if (!chkB_SLSize.Checked)
            {
                double _ValTTTieuHao = Convert.ToDouble(colTieuHao.SummaryItem.SummaryValue);
                double _ValTTSoSP = Convert.ToDouble(colSoLuong.SummaryItem.SummaryValue);
                _vlSumTieuHao = _ValTTTieuHao;
                tbDMTT.Text = _ValTTSoSP == 0 ? "0" : (Math.Round(_ValTTTieuHao / _ValTTSoSP, 4)).ToString();
            }
        }
        private void gridView1_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {

            if (e.SummaryProcess != CustomSummaryProcess.Finalize)
                return;

            GridSummaryItem item = e.Item as GridSummaryItem;
            if (item?.Tag == null)
                return;

            DataTable _dtgrd = gridControl1.DataSource as DataTable;
            if (_dtgrd == null || _dtgrd.Rows.Count == 0)
                return;

            string tag = item.Tag.ToString();

            double _valueSum = CalculateCustomValue(_dtgrd, tag);

            string[] arrTag = tag.Split('|');
            if (arrTag.Length == 2 && arrTag[1] == "RM")
            {
                string tenCotCanLay = BuildSizeKey(arrTag[0]);

                DataRow rowKH = _dtSumKH.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("Size") == tenCotCanLay);

                double _valueKH = rowKH == null ? 0 : Convert.ToDouble(rowKH["SLSum"]);
                e.TotalValue = _valueKH - _valueSum;
            }
            else
            {
                e.TotalValue = _valueSum;
            }
        }

        private int CalcColumnWidthByCaption(GridView view, string caption)
        {
            using (Graphics g = view.GridControl.CreateGraphics())
            {
                Font font = view.Appearance.HeaderPanel.Font;
                SizeF size = g.MeasureString(caption, font);

                // + padding để không sát mép
                return (int)size.Width + 20;
            }
        }
        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {

            BandedGridView bandedView = new BandedGridView();
            //bandedView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            //bandedView.OptionsSelection.EnableAppearanceFocusedRow = false;
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsBehavior.Editable = false;
            //if (string.IsNullOrEmpty(url)) return bandedView;
            //DataTable tab = await _serviceBase.Get(url);

            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 10; i++)
            {
                List<string> _ListString = new List<string>();

                if (tab.Columns[i].ColumnName == "SPOID")
                {
                    _ListString.Add("SPOID");
                    SetGridBandedViewAmount(bandedView, "", "SPOID", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "MaMau")
                {
                    _ListString.Add("MaMau");
                    SetGridBandedViewAmount(bandedView, "", "Mã màu", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmount(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmount(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "SizeType")
                {
                    _ListString.Add("SizeType");
                    SetGridBandedViewAmount(bandedView, "", "Đầu size", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "SizeTypeID")
                {
                    _ListString.Add("SizeTypeID");
                    SetGridBandedViewAmount(bandedView, "", "Đầu size", _ListString);
                    _ListString.Clear();
                }

                if (tab.Columns[i].ColumnName == "ColorID")
                {
                    _ListString.Add("ColorID");
                    SetGridBandedViewAmount(bandedView, "", "ColorID", _ListString);
                    _ListString.Clear();
                }

            }
            List<string> listHeader = new List<string>();
            int j = 9;
            //_listSizeBig.Clear();
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                //if (arrName.Length > 1)
                //{
                //    string s = arrName[0].Substring(0, 1);
                //    int x;
                //    if (Int32.TryParse(s, out x))
                //    {
                //        _listSizeBig.Add(arrName[0]);
                //    }
                //}
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;

            }
            SetGridBandedViewAmount(bandedView, maHang, "", listHeader);
            bandedView.FixedLineWidth = 1;



            bandedView.OptionsView.ShowFooter = true;
            //add summary on column bandedview


            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("Amount");
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.OwnerBand = gridBand;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Amount";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);

            foreach (BandedGridColumn col in bandedView.Columns)
            {
                if (col.FieldName != "MaMau" && col.FieldName != "ColorID" && col.FieldName != "TenMau" && col.FieldName != "SPOID" && col.FieldName != "SizeType" && col.FieldName != "SizeTypeID" && col.FieldName != "PO")
                {
                    GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                    itemSize.FieldName = col.FieldName;
                    itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    itemSize.DisplayFormat = "{0:n0}";
                    itemSize.ShowInGroupColumnFooter = col;
                    bandedView.GroupSummary.Add(itemSize);
                }

                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
            }

            //bandedView.DoubleClick += bandedView_DoubleClick;
            bandedView.CustomDrawBandHeader += bandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += bandedView_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += bandedView_CustomDrawFooter;
            bandedView.OptionsSelection.MultiSelect = true;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;

            return bandedView;
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
            //gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (nrOfColumns == 1 && (columnNames[0] == "PO" || columnNames[0] == "MaMau" || columnNames[0] == "TenMau" || columnNames[0] == "ColorID" || columnNames[0] == "PO" || columnNames[0] == "SPOID" || columnNames[0] == "SizeType" || columnNames[0] == "SizeTypeID"))
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);

                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;

                if (columnNames[0] == "MaMau" || columnNames[0] == "ColorID" || columnNames[0] == "SizeTypeID" || columnNames[0] == "SPOID")
                {
                    gridBand.Visible = false;
                }
                else
                    gridBand.Visible = true;
                bandedColumns.OptionsColumn.AllowEdit = false;

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
                    //columnNames[i];//
                    //string Name = string.Empty;
                    bandedColumns[i] = (BandedGridColumn)bandedView.Columns.AddField(columnNames[i]);
                    //Name = columnNames[i];
                    bandedColumns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //bandedColumns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    //bandedColumns[i].UnboundType = UnboundColumnType.Integer;
                    bandedColumns[i].DisplayFormat.FormatString = "{0:##,0}";
                    bandedColumns[i].Name = columnNames[i];
                    bandedColumns[i].Width = 60;
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
        void bandedView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
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
                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToInt32(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
        }
        int sum = 0;
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
                    if (col.FieldName != "MaMau" && col.FieldName != "ColorID" && col.FieldName != "TenMau" && col.FieldName != "SPOID" && col.FieldName != "SizeType" && col.FieldName != "SizeTypeID" && col.FieldName != "PO" && col.UnboundType == UnboundColumnType.Bound)
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
        void mainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {

            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "MaMau" && col.FieldName != "ColorID" && col.FieldName != "TenMau" && col.FieldName != "SPOID" && col.FieldName != "SizeType" && col.FieldName != "SizeTypeID" && col.FieldName != "PO" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);

                        //-- thêm để lấy sum của các size
                        if (_dtSumKH.Columns.Count <= 0)
                        {
                            _dtSumKH.Columns.Add("Size", typeof(string));
                            _dtSumKH.Columns.Add("SLSum", typeof(int));
                        }
                        if (_dtSumKH.Rows.Count == 0)
                        {
                            DataRow _dr = _dtSumKH.NewRow();

                            if (chk_TachNhomSize.Checked)
                            {
                                //_dtData.AcceptChanges();
                                //DataTable _dtgrd = gridControl1.DataSource as DataTable;
                                //string filter_1 = string.Empty;
                                string[] arrSize = col.Name.Split(new char[] { '@' });
                                if (chk_TachNhomSize.Checked)
                                {
                                    int soLuong = 0;
                                    for (int x = 0; x < dtSize.Rows.Count; x++)
                                    {
                                        if (arrSize[0].ToString().ToUpper() == dtSize.Rows[x]["Size"].ToString().ToUpper())
                                        {
                                            string size = dtSize.Rows[x]["Size"].ToString().ToUpper();   // M
                                            string sizeType = dtSize.Rows[x]["SizeType"].ToString().ToUpper();

                                            string sizeColumn = dtSLDM.Columns
                                                .Cast<DataColumn>()
                                                .FirstOrDefault(c => c.ColumnName.StartsWith(size + "@"))?
                                                .ColumnName;


                                            if (!string.IsNullOrEmpty(sizeColumn))
                                            {
                                                soLuong = dtSLDM.AsEnumerable()
                                                                .Where(r => r.Field<string>("SizeType").Trim().ToUpper() == sizeType)
                                                                .Sum(r => r[sizeColumn] == DBNull.Value ? 0 : Convert.ToInt32(r[sizeColumn]));
                                            }

                                            _dr = _dtSumKH.NewRow();
                                            _dr["Size"] = col.Name + "@" + dtSize.Rows[x]["SizeTypeID"].ToString().ToUpper();
                                            _dr["SLSum"] = Convert.ToInt32(soLuong);
                                            _dtSumKH.Rows.Add(_dr);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _dr["Size"] = col.Name;
                                _dr["SLSum"] = Convert.ToInt32(col.Summary[0].SummaryValue);
                                _dtSumKH.Rows.Add(_dr);
                            }

                        }
                        else
                        {
                            if (chk_TachNhomSize.Checked)
                            {
                                string[] arrSize = col.Name.Split(new char[] { '@' });
                                if (chk_TachNhomSize.Checked)
                                {
                                    int soLuong = 0;
                                    for (int x = 0; x < dtSize.Rows.Count; x++)
                                    {
                                        if (arrSize[0].ToString().ToUpper() == dtSize.Rows[x]["Size"].ToString().ToUpper())
                                        {
                                            string size = dtSize.Rows[x]["Size"].ToString().ToUpper();   // M
                                            string sizeType = dtSize.Rows[x]["SizeType"].ToString().ToUpper();

                                            string sizeColumn = dtSLDM.Columns
                                                .Cast<DataColumn>()
                                                .FirstOrDefault(c => c.ColumnName.StartsWith(size + "@"))?
                                                .ColumnName;


                                            if (!string.IsNullOrEmpty(sizeColumn))
                                            {
                                                soLuong = dtSLDM.AsEnumerable()
                                                                .Where(r => r.Field<string>("SizeType").Trim().ToUpper() == sizeType)
                                                                .Sum(r => r[sizeColumn] == DBNull.Value ? 0 : Convert.ToInt32(r[sizeColumn]));
                                            }

                                            DataRow _dr = _dtSumKH.NewRow();
                                            _dr["Size"] = col.Name + "@" + dtSize.Rows[x]["SizeTypeID"].ToString().ToUpper();
                                            _dr["SLSum"] = Convert.ToInt32(soLuong);
                                            _dtSumKH.Rows.Add(_dr);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (_dtSumKH.AsEnumerable().Any(x => (string)x["Size"] == col.Name))
                                {
                                    foreach (DataRow _dr in _dtSumKH.Rows)
                                    {
                                        if (_dr["Size"] == col.Name)
                                        {
                                            _dr["SLSum"] = Convert.ToInt32(col.Summary[0].SummaryValue);
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow _dr = _dtSumKH.NewRow();
                                    _dr["Size"] = col.Name;
                                    _dr["SLSum"] = Convert.ToInt32(col.Summary[0].SummaryValue);

                                    _dtSumKH.Rows.Add(_dr);
                                }
                            }

                        }
                        // end---
                    }
                }
                e.TotalValue = sum;
            }
        }

        private double CalculateCustomValue(DataTable _dt, string tagname)
        {

            //return result;
            double result = 0;

            string[] arrTag = tagname.Split('|');
            if (arrTag.Length == 0)
                return 0;

            string colName = arrTag[0];
            if (!_dt.Columns.Contains(colName))
                return 0;

            bool isTinhTheoSoLop = !chkB_SLSize.Checked;

            foreach (DataRow row in _dt.Rows)
            {
                double valueSize = row[colName] == DBNull.Value ? 0 : Convert.ToDouble(row[colName]);
                int valuesluongSD = row["SLSoDo"] == DBNull.Value ? 0 : Convert.ToInt32(row["SLSoDo"]);
                double SoLopPCS = txtSoLop_PCS.Text == "" ? 1 : Convert.ToDouble(txtSoLop_PCS.Text);
                if (isTinhTheoSoLop)
                {
                double_toggle:
                    double valuesl = row["SoLop"] == DBNull.Value ? 0 : Convert.ToDouble(row["SoLop"]);
                    result += valueSize * valuesl * valuesluongSD / SoLopPCS;
                }
                else
                {
                    result += valueSize * valuesluongSD / SoLopPCS;
                }
            }

            return result;
        }
        private bool ColumnExists(string columnName)
        {
            foreach (DataColumn column in _dtData.Columns)
            {
                if (column.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        bool indicatorIcon = true;
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }


                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("*", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize + 20;
                    }

                    e.Info.DisplayText = "*";
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 20;
        }
        private List<string> sttValuesList = new List<string>();
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column == colSTT)
            {
                if (e.RowHandle < 0)
                {
                    e.DisplayText = "0";
                }
                else
                {
                    string sttValue = Convert.ToString(e.RowHandle + 1);
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);

                    if (!sttValuesList.Contains(sttValue))
                    {

                        sttValuesList.Add(sttValue);
                    }
                }
            }
        }

        private void gridView1_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            // Chỉ áp dụng cho những cột có FieldName chứa ký tự "@"
            if (e.Column.FieldName.Contains("@"))
            {
                if (decimal.TryParse(e.Info.DisplayText, out decimal value))
                {
                    if (value < 0)
                    {


                        e.Appearance.Options.UseBackColor = true;
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);

                        e.Appearance.ForeColor = Color.Red;
                        //e.Appearance.BackColor = Color.Yellow;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);


                        e.Handled = true;


                        e.Appearance.BackColor = Color.Yellow; // Or any other color


                        e.Appearance.DrawBackground(e.Cache, e.Bounds);


                        e.Appearance.DrawString(e.Cache, e.Info.DisplayText, e.Bounds);
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black; // Mặc định khi >= 0

                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold); // Tô đậm nếu muốn
                    }
                }
            }
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Lưu file Excel",
                Filter = "Excel 2010 (*.xlsx)|*.xlsx|Excel 2003 (*.xls)|*.xls",
                FileName = string.Format("so-do-tac-nghiep-cat-{0}", DateTime.Now.ToString("ddMMyyyyHHss"))
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");

            try
            {
                Export(sfd.FileName);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }

            if (XtraMessageBox.Show("Mở file vừa xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (File.Exists(sfd.FileName))
                        System.Diagnostics.Process.Start("explorer.exe", sfd.FileName);
                }
                catch
                {
                    XtraMessageBox.Show("Không thể mở file.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        } 
        private void Export(string exportFileName)
            {
                try
                {
                DataTable tblSoDoCat = gridControl1.DataSource as DataTable;
                if (tblSoDoCat == null || tblSoDoCat.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                string templatePath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Templates",
                        "Template-so-do-tnc.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        XtraMessageBox.Show(
                            string.Format("Không tìm thấy template:\n{0}", templatePath),
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                int row = 5;
                int stt = 1;

                File.Copy(templatePath, exportFileName, overwrite: true);


                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                    using (var pkg = new ExcelPackage(new FileInfo(exportFileName)))
                    {
                        var ws = pkg.Workbook.Worksheets[0];


                        

                    /*TT LSX*/

                    ws.Cells[row, 1].Value = $"Mã lệnh: {_maLenh}";
                    ws.Cells[row + 1, 1].Value = $"Mã hàng: {_mahang}";
                    ws.Cells[row + 2, 1].Value = $"Khách hàng : {SafeStr(_rowFocusedMaLenh, "TenKH")}" ;
                    ws.Cells[row + 3, 1].Value = $"PO: {_StrPO}";

                    ws.Cells[row + 4, 1].Value = $"Đầu size: {StrSizeType}";
                    ws.Cells[row + 5, 1].Value = $"Màu: {_StrTenMau}";




                    ws.Cells[row, 6].Value = $"Vải: {SafeStr(_rowFocusedCapPhat, "ChiTiet")}";
                    ws.Cells[row + 1, 6].Value = $"Khổ vải: {SafeStr(_rowFocusedCapPhat, "KhoVai")}";
                    ws.Cells[row + 2, 6].Value = $"Khổ vải TT: {tbKhoVai1.EditValue}";
                    ws.Cells[row + 3, 6].Value = $"ĐMKH : {tbDMKH.EditValue}";
                    ws.Cells[row + 4, 6].Value = $"ĐMSX: {tbDMTT.EditValue}";
                    ws.Cells[row + 5, 6].Value = $"Số lớp cho 1pcs: {txtSoLop_PCS.EditValue}";

                    ws.Cells[row + 5, 6].Value = $"%ĐM: {tbDungSaiDM.EditValue}";
                    ws.Cells[row + 6, 6].Value = $"Số lớp cho 1 pcs: {txtSoLop_PCS.EditValue}";
                    ws.Cells[row + 6, 1].Value = $"Ghi chú: {tbGhiChu.EditValue}";



                    ws.Cells[5,1,row + 6, 6].Style.Font.Bold = true;
                    row = row + 8;
                    int startRowPO = row;
                    DataTable tblPOChiTiet = gridControl2.DataSource as DataTable;
                    DataTable tblSoDo = gridControl1.DataSource as DataTable;
                    /*Table PO Chi tiết*/
                    /*Header PO*/
                    ws.Cells[row, 1].Value = "PO";
                    ws.Cells[row, 1, row + 1, 1].Merge = true;
                    ws.Cells[row, 1, row + 1, 1].Style.Font.Bold = true;


                    ws.Cells[row, 2].Value = "Màu";
                    ws.Cells[row, 2, row + 1, 2].Merge = true;
                    ws.Cells[row, 2, row + 1, 2].Style.Font.Bold = true;

                    ws.Cells[row, 3].Value = "Đầu size";
                    ws.Cells[row, 3, row + 1, 3].Merge = true;
                    ws.Cells[row, 3, row + 1, 3].Style.Font.Bold = true;

                    ws.Cells[row, 4].Value = "Size";
                    ws.Cells[row, 4].Style.Font.Bold = true;

                    int rowHeader = row + 1;
                    int colHeader = 4;
                    if (tblPOChiTiet?.Columns?.Count > 0)
                    {
                        foreach (DataColumn col in tblPOChiTiet.Columns)
                        {
                            if (col.ColumnName.Contains("@"))
                            {
                                string[] arrSize = col.ColumnName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                                ws.Cells[rowHeader, colHeader].Value = arrSize[0];
                                ws.Cells[rowHeader, colHeader].Style.Font.Bold = true;
                                colHeader++;
                            }
                        }
                    }
                    ws.Cells[row, 4, row, colHeader-1].Merge = true;
                    ws.Cells[row, colHeader].Value = "Tổng";
                    ws.Cells[row, colHeader, row + 1, colHeader].Merge = true;
                    ws.Cells[row, colHeader, row + 1, colHeader].Style.Font.Bold = true;
                    ws.Cells[row, colHeader].Style.Font.Bold = true;
                    ws.Cells[row, colHeader].Style.Font.Color.SetColor(System.Drawing.Color.Red);


                    ws.Cells[row, colHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, colHeader].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    row = 15;
                    foreach (DataRow dr in tblPOChiTiet.Rows)
                    {
                        ws.Cells[row, 1].Value = dr["PO"];
                        ws.Cells[row, 2].Value = dr["TenMau"];
                        ws.Cells[row, 3].Value = dr["SizeType"];
                        colHeader = 4;
                        foreach (DataColumn col in tblPOChiTiet.Columns)
                        {
                            if (col.ColumnName.Contains("@"))
                            {
                                //if (dr[col.ColumnName]?.ToString() == "0") continue;
                                int.TryParse(dr[col.ColumnName]?.ToString(), out int SoLuong);
                                ws.Cells[row, colHeader].Value = SoLuong;
                                colHeader++;
                            }
                            
                          
                        }
                        ws.Cells[row, colHeader].Formula =
                         $"SUM({ws.Cells[row, 4].Address}:{ws.Cells[row, colHeader-1].Address})";

                        ws.Cells[row, colHeader].Style.Font.Bold = true;
                        ws.Cells[row, colHeader].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        row++;
                    }

                   

                    var range = ws.Cells[startRowPO, 1, row, colHeader];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;


                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[row, 1, row, 3].Merge = true;
                    ws.Cells[row, 1, row, 3].Value = "Tổng";

                    for (int i = 4; i <= colHeader; i++)
                    {
                        ws.Cells[row, i].Formula =
                            $"SUM({ws.Cells[15, i].Address}:{ws.Cells[row - 1, i].Address})";
                      
                    }

                    ws.Cells[row, 1, row, colHeader].Style.Font.Bold = true;
                    ws.Cells[row, 1, row, colHeader].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    //ws.Cells[row, colHeader+1].Formula = $"SUM({15}{colHeader+1}:{row - 1}{colHeader+1})";
                    row = row + 3;
                    int startRowSD = row;

                    /*Table SoDo*/
                    /*Header PO*/
                    ws.Cells[row, 1].Value = "STT";
                    ws.Cells[row, 1, row + 1, 1].Merge = true;
                    ws.Cells[row, 1, row + 1, 1].Style.Font.Bold = true;


                    ws.Cells[row, 2].Value = "Tên SĐ";
                    ws.Cells[row, 2, row + 1, 2].Merge = true;
                    ws.Cells[row, 2, row + 1, 2].Style.Font.Bold = true;

                    ws.Cells[row, 3].Value = "Sơ đồ";
                    ws.Cells[row, 3, row + 1, 3].Merge = true;
                    ws.Cells[row, 3, row + 1, 3].Style.Font.Bold = true;

                    ws.Cells[row, 4].Value = "Size";
                    ws.Cells[row, 4].Style.Font.Bold = true;

                    int colHeaderSoDo = 4;
                    if (tblSoDo?.Columns?.Count > 0)
                    {
                        foreach (DataColumn col in tblSoDo.Columns)
                        {
                            if (col.ColumnName.Contains("@"))
                            {
                                string[] arrSize = col.ColumnName.Split(new string[] { "@" }, StringSplitOptions.None);
                                if (arrSize.Length == 2)
                                {
                                    ws.Cells[row + 1, colHeaderSoDo].Value = $"{arrSize[0]}";
                                }
                                else
                                {
                                    ws.Cells[row + 1, colHeaderSoDo].Value = $"{arrSize[0]}|{arrSize[2]}";
                                }
                              
                                ws.Cells[row + 1, colHeaderSoDo].Style.Font.Bold = true;
                                colHeaderSoDo++;
                            }
                        }
                    }

                    ws.Cells[row, 4, row, colHeaderSoDo - 1].Merge = true;
                    ws.Cells[row, colHeaderSoDo].Value = "Pcs";
                    ws.Cells[row, colHeaderSoDo, row + 1, colHeaderSoDo].Merge = true;
                    ws.Cells[row, colHeaderSoDo , row + 1, colHeaderSoDo].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo, row + 1, colHeaderSoDo].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 1].Value = "Số lớp";
                    ws.Cells[row, colHeaderSoDo + 1, row + 1, colHeaderSoDo + 1].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 1, row + 1, colHeaderSoDo + 1].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 1, row + 1, colHeaderSoDo + 1].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 2].Value = "SL Sơ đồ";
                    ws.Cells[row, colHeaderSoDo + 2, row + 1, colHeaderSoDo + 2].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 2, row + 1, colHeaderSoDo + 2].Style.Font.Bold = true; 
                    ws.Cells[row, colHeaderSoDo + 2, row + 1, colHeaderSoDo + 2].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 3].Value = "Số SP";
                    ws.Cells[row, colHeaderSoDo + 3, row + 1, colHeaderSoDo + 3].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 3, row + 1, colHeaderSoDo + 3].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 3, row + 1, colHeaderSoDo + 3].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 4].Value = "Lũy kế";
                    ws.Cells[row, colHeaderSoDo + 4, row + 1, colHeaderSoDo + 4].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 4, row + 1, colHeaderSoDo + 4].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 4, row + 1, colHeaderSoDo + 4].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 5].Value = "Dài SĐ(met)";
                    ws.Cells[row, colHeaderSoDo + 5, row + 1, colHeaderSoDo + 5].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 5, row + 1, colHeaderSoDo + 5].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 5, row + 1, colHeaderSoDo + 5].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 6].Value = "Dài B/cắt (met)";
                    ws.Cells[row, colHeaderSoDo + 6, row + 1, colHeaderSoDo + 6].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 6, row + 1, colHeaderSoDo + 6].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 6, row + 1, colHeaderSoDo + 6].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 7].Value = "Rộng";
                    ws.Cells[row, colHeaderSoDo + 7, row + 1, colHeaderSoDo + 7].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 7, row + 1, colHeaderSoDo + 7].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 7, row + 1, colHeaderSoDo + 7].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 8].Value = "Tiêu hao (met)";
                    ws.Cells[row, colHeaderSoDo + 8, row + 1, colHeaderSoDo + 8].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 8, row + 1, colHeaderSoDo + 8].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 8, row + 1, colHeaderSoDo + 8].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 9].Value = "Đ/mức";
                    ws.Cells[row, colHeaderSoDo + 9, row + 1, colHeaderSoDo + 9].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 9, row + 1, colHeaderSoDo + 9].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 9, row + 1, colHeaderSoDo + 9].Style.WrapText = true;

                    ws.Cells[row, colHeaderSoDo + 10].Value = "Ghi chú";
                    ws.Cells[row, colHeaderSoDo + 10, row + 1, colHeaderSoDo + 10].Merge = true;
                    ws.Cells[row, colHeaderSoDo + 10, row + 1, colHeaderSoDo + 10].Style.Font.Bold = true;
                    ws.Cells[row, colHeaderSoDo + 10, row + 1, colHeaderSoDo + 10].Style.WrapText = true;



                    row = row + 2;
                    foreach (DataRow dr in tblSoDo.Rows)
                    {
                        ws.Cells[row, 1].Value = dr["STT"];
                        ws.Cells[row, 2].Value = dr["TT_TenSD"];
                        ws.Cells[row, 3].Value = dr["SoDo"];
                        colHeaderSoDo = 4;
                        foreach (DataColumn col in tblSoDo.Columns)
                        {
                            if (col.ColumnName.Contains("@"))
                            {
                                //if (dr[col.ColumnName]?.ToString() == "0") continue;
                                
                                ws.Cells[row, colHeaderSoDo].Value = dr[col.ColumnName];
                                colHeaderSoDo++;
                            }


                        }
                        ws.Cells[row, colHeaderSoDo].Value = dr["SoBo"];
                        ws.Cells[row, colHeaderSoDo + 1].Value = int.TryParse(dr["SoLop"]?.ToString(), out int soLop) ? soLop : 0;
                        ws.Cells[row, colHeaderSoDo + 2].Value = dr["SLSoDo"];
                        ws.Cells[row, colHeaderSoDo + 3].Value = dr["SoLuong"];
                        ws.Cells[row, colHeaderSoDo + 4].Value = dr["LuyKe"]; //Lũy kế
                        ws.Cells[row, colHeaderSoDo + 5].Value = dr["Dai"];
                        ws.Cells[row, colHeaderSoDo + 6].Value = dr["Dai_DB"];
                        ws.Cells[row, colHeaderSoDo + 7].Value = dr["Rong"];
                        ws.Cells[row, colHeaderSoDo + 8].Value = dr["TieuHao"];//Tiêu hao
                        ws.Cells[row, colHeaderSoDo + 9].Value = dr["DMTT"];
                        ws.Cells[row, colHeaderSoDo + 10].Value = dr["MoTa"];


                        row++;
                    }

                    ws.Cells[row, colHeaderSoDo].Formula =
                            $"SUM({ws.Cells[startRowSD, colHeaderSoDo].Address}:{ws.Cells[row - 1, colHeaderSoDo].Address})";

                    ws.Cells[row, colHeaderSoDo +1].Formula =
                        $"SUM({ws.Cells[startRowSD, colHeaderSoDo +1].Address}:{ws.Cells[row - 1, colHeaderSoDo+1].Address})";

                    ws.Cells[row, colHeaderSoDo +3].Formula =
                        $"SUM({ws.Cells[startRowSD, colHeaderSoDo +3].Address}:{ws.Cells[row - 1, colHeaderSoDo+3].Address})";

                    ws.Cells[row, colHeaderSoDo + 8].Formula =
                      $"SUM({ws.Cells[startRowSD, colHeaderSoDo + 8].Address}:{ws.Cells[row - 1, colHeaderSoDo + 8].Address})";

                    ws.Cells[row, 1, row, 3].Merge = true;
                    ws.Cells[row, 1, row, 3].Value = "Tổng";

                    ws.Cells[row + 1, 1, row +1, 3].Merge = true;
                    ws.Cells[row +1, 1, row +1, 3].Value = "Còn lại";

                    int colSizeSummay = 4;
                    foreach (DataColumn col in tblSoDo.Columns)
                    {
                        if (col.ColumnName.Contains("@"))
                        {
                            string tag = $"{col.ColumnName}|RM";
                          
                            double _valueSum = CalculateCustomValue(tblSoDo, tag);
                            DataRow rowKH = null;
                         
                            string[] arrSize = col.ColumnName.Split(new string[] { "@" }, StringSplitOptions.None);
                            if (arrSize.Length == 2)
                            {
                                 rowKH =  _dtSumKH.AsEnumerable()
                           .FirstOrDefault(r => r["Size"]?.ToString() == $"{arrSize[0]}@Size@{arrSize[1]}");

                                
                            }
                            else
                            {
                                rowKH = _dtSumKH.AsEnumerable()
                            .FirstOrDefault(r => r["Size"]?.ToString() == $"{arrSize[0]}@Size@{arrSize[1]}@{arrSize[2]}");
                            }

                            double _valueKH = rowKH == null ? 0 : Convert.ToDouble(rowKH["SLSum"]);

                            ws.Cells[row, colSizeSummay].Value = _valueSum;
                            ws.Cells[row +1, colSizeSummay].Value = _valueKH - _valueSum; ;
                            colSizeSummay++;
                        }


                    }

                    ws.Cells[row, 1, row + 1, colHeader].Style.Font.Bold = true;
                    ws.Cells[row, 1, row +1, colHeaderSoDo + 10].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                    var rangeSoDo = ws.Cells[startRowSD, 1, row + 1, colHeaderSoDo+10];
                    rangeSoDo.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rangeSoDo.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rangeSoDo.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rangeSoDo.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    rangeSoDo.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rangeSoDo.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    //ws.Calculate();
                    pkg.Save();

                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        private static string SafeStr(DataRow dr, string col)
        {
            return dr.Table.Columns.Contains(col) && dr[col] != DBNull.Value
                ? dr[col].ToString()
                : string.Empty;
        }
    }
}