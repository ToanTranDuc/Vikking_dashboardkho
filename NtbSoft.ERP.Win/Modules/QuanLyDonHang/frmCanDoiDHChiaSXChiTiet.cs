using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Erp.QuanLyDonHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
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
using System.Web;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDHChiaSXChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _maLenhSX = string.Empty, _lenhSX = string.Empty, _maHang = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkCapPhat = false;
        private HttpClientExtension _clientExtension;
        private DataTable _lastSourceData = null;
        private bool _isFromSearch = false;
        private string _searchMaHang = string.Empty;
        private string _searchMaLenh = string.Empty;
        private string _searchChungLoai = string.Empty;
        private string _searchKhachHang = string.Empty;
        private string _searchDot = string.Empty;
        private string _searchPO = string.Empty;
        private string madhFocus = string.Empty;
        private bool _isGiaCong = false;
        private bool _isDuyetCancelled = false;
        bool IsDuyetKichHoatlenh = false;
        private bool _isFromAll = false;
        #region variable My
        private string _MaLenhSX = string.Empty;
        private string _TenLenhSX = string.Empty;
        private string _MaGop = string.Empty;
        private string _MaDH_Send = string.Empty;
        private string _MaLenhSX_Send = string.Empty;
        private string _MaVTID = string.Empty;
        private string _MauVTID = string.Empty;
        private string _KhoVaiID = string.Empty;
        private string _MaCLVT = string.Empty;
        #endregion

        public frmCanDoiDHChiaSXChiTiet(string maDH = "", string maHang = "", string maLenhSX = "", bool allowAdd = false, bool allowEdit = false,
            bool allowDelete = false, bool checkCapPhat = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._maDH = maDH;
            this._maHang = maHang;
            this._maLenhSX = maLenhSX;
            bandedGridView1.RowCellStyle += bandedGridView1_RowCellStyle;
            bandedGridView1.ShowingEditor += bandedGridView1_ShowingEditor;

            bandedGridView1.Appearance.FocusedCell.Options.UseBackColor = true;
            bandedGridView1.Appearance.FocusedCell.BackColor = Color.FromArgb(234, 234, 234);

            bandedGridView1.CustomColumnDisplayText += bandedGridView1_CustomColumnDisplayText;
            bandedGridView1.CellValueChanged += bandedGridView1_CellValueChanged;

            bandedGridView1.GridControl.ProcessGridKey += GridControl_ProcessGridKey;
            grvLenhChia.FocusedRowChanged += grvLenhChia_FocusedRowChanged;
            grvLenhChia.ShowingEditor += grvLenhChia_ShowingEditor;
            grvLenhChia.RowCellStyle += grv_RowCellStyle;
            grvCanDoi.RowCellStyle += grvCanDoi_RowCellStyle;

            bandedGridView1.OptionsSelection.MultiSelect = true;
            bandedGridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            bandedGridView1.SelectionChanged += BandedGridView1_SelectionChanged;
            this._allowAdd = allowAdd;
            this._allowEdit = allowEdit;
            this._allowDelete = allowDelete;
            this._checkCapPhat = checkCapPhat;
            //this.madhFocus = madhFocus;
            InIt();
            CheckPerminsion();
            if (grvCanDoi == null)
            {
                layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
        }
        private void CheckPerminsion()
        {
            if (!_allowAdd || !_allowEdit || !_allowDelete)
            {
                simpleButton6.Enabled = false;
                btnXuatEX.Enabled = false;
                btnChiaChuyenItem.Enabled = false;
                btnDoiChuyenItem.Enabled = false;
                btnXoaChuyenItem.Enabled = false;
                btnDuyetItem.Enabled = false;
                btnSaveItem.Enabled = false;
            }
        }
        private void InIt()
        {
            searchLookUpEditPO.Properties.ValueMember = "POID";
            searchLookUpEditPO.Properties.DisplayMember = "PO";
            searchLookUpEditPO.Properties.NullValuePrompt = "Chọn PO";

            searchLookUpEditMau.Properties.ValueMember = "MaMau";
            searchLookUpEditMau.Properties.DisplayMember = "TenMau";
            searchLookUpEditMau.Properties.NullValuePrompt = "Chọn màu";

            searchLookUpEditLine.Properties.ValueMember = "Line";
            searchLookUpEditLine.Properties.DisplayMember = "Name";
            searchLookUpEditLine.Properties.NullValuePrompt = "Chọn chuyền";
            LoadLenhSanXuat();
            grvLenhChia.FocusedRowChanged += grvLenhChia_FocusedRowChanged;
            if (_checkCapPhat)
            {
                colPOMua.Visible = false;
                layoutControlGroup10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
        }
        private void LoadLenhSanXuat()
        {

            IsDuyetKichHoatlenh = false;
            string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para={_maHang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                grcLenhChia.DataSource = null;
                return;
            }
            if (!tbl.Columns.Contains("Sort"))
            {
                tbl.Columns.Add("Sort", typeof(int));
                foreach (DataRow r in tbl.Rows)
                    r["Sort"] = 0;
            }
            DataTable tblMaDHGop = GopVaTongLenhChiaSX(tbl);
            foreach (DataRow r in tblMaDHGop.Rows)
            {
                if (r["GopDH"].ToString() == _maDH || r["GopDH"].ToString().Contains(_maDH))
                    r["Sort"] = 1;
            }
            DataView dv = tblMaDHGop.DefaultView;
            dv.Sort = "Sort DESC";
            tblMaDHGop = dv.ToTable();
            if (!tblMaDHGop.Columns.Contains("IsChon"))
            {
                tblMaDHGop.Columns.Add("IsChon", typeof(bool));
                foreach (DataRow r in tblMaDHGop.Rows)
                    r["IsChon"] = false;
            }
            grcLenhChia.DataSource = tblMaDHGop;
            _lastSourceData = tblMaDHGop.Copy();
            _maDH = tblMaDHGop.Rows[0]["MaDH"].ToString();
            _maLenhSX = tblMaDHGop.Rows[0]["MaLenhSanXuat"].ToString();

            //AutoClickRow();
            //CheckIsUpdateBOM();
        }
        private DataTable GopVaTongLenhChiaSX(DataTable tbl)
        {
            if (tbl == null || tbl.Rows.Count == 0)
                return tbl;
            var grouped = tbl.AsEnumerable()
                .GroupBy(r => new
                {
                    MaLenhSanXuat = r["MaLenhSanXuat"]?.ToString() ?? "",
                })
                .Select(g =>
                {
                    var firstRow = g.First();
                    var distinctMaHang = g.Select(r => r["MaDH"]?.ToString()).Distinct().Count();

                    int totalSL = 0;
                    if (distinctMaHang > 1)
                    {
                        totalSL = g.Sum(r =>
                        {
                            int sl = 0;
                            int.TryParse(r["SoLuong"]?.ToString(), out sl);
                            return sl;
                        });
                    }
                    else
                    {
                        int.TryParse(firstRow["SoLuong"]?.ToString(), out totalSL);
                    }
                    int sort = 0;
                    if (firstRow.Table.Columns.Contains("Sort"))
                        int.TryParse(firstRow["Sort"]?.ToString(), out sort);
                    return new
                    {
                        FirstRow = firstRow,
                        TotalSL = totalSL,
                        Sort = sort
                    };
                })
                .ToList();

            DataTable dtResult = tbl.Clone();
            dtResult.Clear();

            foreach (var item in grouped)
            {
                DataRow newRow = dtResult.NewRow();
                foreach (DataColumn col in dtResult.Columns)
                {
                    newRow[col.ColumnName] = item.FirstRow[col.ColumnName];
                }
                if (dtResult.Columns.Contains("SoLuong"))
                    newRow["SoLuong"] = item.TotalSL;

                dtResult.Rows.Add(newRow);
            }

            return dtResult;
        }
        private void AutoClickRow()
        {
            if (!this.IsHandleCreated)
            {
                this.HandleCreated += (s, e) => AutoClickRow();
                return;
            }
            this.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < grvLenhChia.RowCount; i++)
                {
                    var row = grvLenhChia.GetDataRow(i);
                    if (row != null && row["MaDH"].ToString() == _maDH && row["MaLenhSanXuat"].ToString() == _maLenhSX)
                    {
                        grvLenhChia.FocusedRowHandle = i;
                        grvLenhChia.SelectRow(i);
                        break;
                    }
                }
            }));
        }
        private void LoadCanDoi()
        {
            string url = $"{URL}ERPDonHangTong/Get?action=GetNPL&para={_maDH}&para5={_maLenhSX}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcCanDoi.DataSource = null;
                return;
            }
            if (!tbl.Columns.Contains("IsEdited"))
            {
                tbl.Columns.Add("IsEdited", typeof(bool));
                foreach (DataRow r in tbl.Rows)
                    r["IsEdited"] = false;
            }
            if (!tbl.Columns.Contains("IsCapPhatEdited"))
            {
                tbl.Columns.Add("IsCapPhatEdited", typeof(bool));
                foreach (DataRow r in tbl.Rows)
                    r["IsCapPhatEdited"] = false;
            }
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return;
            string magop = dr["MaGop"].ToString();
            DataTable dtAllSizes = GetBangSize(magop);
            DataTable dtERPSizes = GetERPSIZESP(magop);
            tbl = XuLyCotSize(tbl, dtAllSizes, dtERPSizes);
            grcCanDoi.DataSource = tbl;
            //CheckIsUpdateBOM();
        }
        private void CheckGiaCong()
        {
            _isGiaCong = false;
            if (string.IsNullOrEmpty(_maLenhSX)) return;

            string url = $"{URL}ERPDonHangTong/Get?action=CheckGiaCong&para={_maLenhSX}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            if (string.IsNullOrEmpty(json)) return;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;

            int giaCong = 0;
            int.TryParse(tbl.Rows[0]["GiaCong"]?.ToString(), out giaCong);
            _isGiaCong = giaCong == 1;
            var colSL = grvCanDoi.Columns["SoLuong"];
            var colDM = grvCanDoi.Columns["DinhMuc"];
            var colCP = grvCanDoi.Columns["CapPhat"];
            if (colSL != null)
                colSL.OptionsColumn.AllowEdit = _isGiaCong;
            if (colDM != null)
                colDM.OptionsColumn.AllowEdit = _isGiaCong;
            if (colCP != null)
                colCP.OptionsColumn.AllowEdit = _isGiaCong;
            grvCanDoi.RefreshData();
        }
        public DataTable GetBangSize(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetBangSize", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null || tbl.Rows.Count == 0)
            {
                return null;
            }
            return tbl;
        }

        public DataTable GetERPSIZESP(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetERPSIZESP", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null || tbl.Rows.Count == 0)
            {
                return null;
            }
            return tbl;
        }

        private DataTable XuLyCotSize(DataTable tblMain, DataTable dtAllSizes, DataTable dtERPSizes)
        {
            if (!tblMain.Columns.Contains("Size"))
            {
                tblMain.Columns.Add("Size", typeof(string));
            }

            // Dictionary chứa danh sách size theo NhomSize (Bảng 1 - BangSize)
            Dictionary<string, List<string>> dictAllSizes = new Dictionary<string, List<string>>();

            if (dtAllSizes != null && dtAllSizes.Rows.Count > 0)
            {
                var groupedAllSizes = dtAllSizes.AsEnumerable()
                    .GroupBy(r => r.Field<string>("NhomSize"))
                    .Select(g => new
                    {
                        NhomSize = g.Key,
                        Sizes = g.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                 .Distinct()
                                 .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                 .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                 .ThenBy(s => s)
                                 .ToList()
                    });

                foreach (var item in groupedAllSizes)
                {
                    dictAllSizes[item.NhomSize] = item.Sizes;
                }
            }
            Dictionary<string, Dictionary<string, List<string>>> dictVTSizes = new Dictionary<string, Dictionary<string, List<string>>>();

            if (dtERPSizes != null && dtERPSizes.Rows.Count > 0)
            {
                var groupedVTSizes = dtERPSizes.AsEnumerable()
                    .GroupBy(r => new
                    {
                        ID = r.Field<object>("ID")?.ToString(),
                        MaVTID = r.Field<object>("MaVTID")?.ToString(),
                        MaMauVT = r.Field<object>("MaMauVT")?.ToString(),
                        MaKhoVT = r.Field<object>("MaKhoVT")?.ToString(),
                        MaNhomVT = r.Field<object>("MaNhomVT")?.ToString(),
                        MaNhomChiTiet = r.Field<object>("MaNhomChiTiet")?.ToString(),
                        MaCode = r.Field<object>("MaCode")?.ToString()
                    })
                    .Select(g => new
                    {
                        Key = g.Key,
                        SizesByNhom = g.GroupBy(r => r.Field<string>("NhomSize"))
                                       .Select(ng => new
                                       {
                                           NhomSize = ng.Key,
                                           Sizes = ng.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                                     .Distinct()
                                                     .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                                     .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                                     .ThenBy(s => s)
                                                     .ToList()
                                       })
                                       .ToDictionary(x => x.NhomSize, x => x.Sizes)
                    });

                foreach (var item in groupedVTSizes)
                {
                    string key = item.Key.ID;
                    dictVTSizes[key] = item.SizesByNhom;
                }
            }
            foreach (DataRow row in tblMain.Rows)
            {
                string id = row["ID"]?.ToString();
                string sizeValue = "";

                if (!string.IsNullOrEmpty(id) && dictVTSizes.ContainsKey(id))
                {
                    var vtSizesByNhom = dictVTSizes[id];
                    bool isAllSize = true;
                    List<string> sizeStrings = new List<string>();
                    var sortedNhoms = vtSizesByNhom
                        .Select(kv => new
                        {
                            Key = kv.Key,
                            Value = kv.Value,
                            IsNum = int.TryParse(kv.Key, out int n),
                            NumVal = int.TryParse(kv.Key, out int n2) ? n2 : int.MaxValue
                        })
                        .OrderBy(x => x.IsNum ? 0 : 1)
                        .ThenBy(x => x.NumVal)
                        .ThenBy(x => x.Key);

                    foreach (var nhom in sortedNhoms)
                    {
                        string nhomSize = nhom.Key;
                        List<string> vtSizes = nhom.Value;
                        if (dictAllSizes.ContainsKey(nhomSize))
                        {
                            List<string> allSizes = dictAllSizes[nhomSize];
                            bool containsAllSizes = allSizes.All(s => vtSizes.Contains(s));

                            if (!containsAllSizes)
                            {
                                isAllSize = false;
                            }
                        }
                        else
                        {
                            isAllSize = false;
                        }
                        string sizeString = nhomSize + ":" + string.Join(",", vtSizes);
                        sizeStrings.Add(sizeString);
                    }
                    foreach (var nhomKey in dictAllSizes.Keys)
                    {
                        if (!vtSizesByNhom.ContainsKey(nhomKey))
                        {
                            isAllSize = false;
                            break;
                        }
                    }
                    if (isAllSize && sizeStrings.Count > 0)
                    {
                        sizeValue = "AllSize";
                    }
                    else
                    {
                        sizeValue = string.Join("; ", sizeStrings);
                    }
                }

                row["Size"] = sizeValue;
            }

            return tblMain;
        }
        private void GetIsDuyetLai()
        {
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return;
            string malenh = dr["MaLenh"].ToString();
            string malenhsx = dr["MaLenhSanXuat"].ToString();
            string magop = dr["MaGop"].ToString();
            string url = $"{URL}ERPDonHangTong/Get?action=CheckDuyetLai&para={malenhsx}&para2={magop}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (string.IsNullOrEmpty(json)) return;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;

            int isDuyetLai = 0;
            int.TryParse(tbl.Rows[0]["IsDuyetLai"]?.ToString(), out isDuyetLai);
            if (isDuyetLai == 0)
            {
                layoutControlItem34.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else
            {
                layoutControlItem34.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }

        }
        private void CheckChangedSoLuong()
        {
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return;
            string malenh = dr["MaLenh"].ToString();
            string malenhsx = dr["MaLenhSanXuat"].ToString();
            string magop = dr["MaGop"].ToString();
            string url = $"{URL}ERPDonHangTong/Get?action=GetTrangThai&para={malenhsx}&para2={malenh}&para3={magop}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int trangThai = 0;
                    int.TryParse(tbl.Rows[0]["TrangThai"]?.ToString(), out trangThai);

                    if (trangThai == 1 && grvCanDoi.DataSource != null)
                    {
                        layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        layoutControlItem35.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
            }
        }
        private bool IsChangedSL(DataRow dr = null)
        {
            if (dr == null)
            {
                dr = grvLenhChia.GetFocusedDataRow();
            }

            if (dr == null) return false;

            string malenh = dr["MaLenh"]?.ToString() ?? "";
            string malenhsx = dr["MaLenhSanXuat"]?.ToString() ?? "";
            string magop = dr["MaGop"]?.ToString() ?? "";

            string url = $"{URL}ERPDonHangTong/Get?action=GetTrangThai&para={malenhsx}&para2={malenh}&para3={magop}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int trangThaiValue = 0;
                    int.TryParse(tbl.Rows[0]["TrangThai"]?.ToString(), out trangThaiValue);
                    return trangThaiValue == 1;
                }
            }
            return false;
        }
        private bool HasDataCanDoiNPL()
        {
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return false;

            string malenhsx = dr["MaLenhSanXuat"]?.ToString() ?? "";
            string url = $"{URL}ERPDonHangTong/Get?action=HasData&para={malenhsx}&para2=&para3=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                return tbl != null && tbl.Rows.Count > 0;
            }
            return false;
        }
        private void CheckIsUpdateBOM()
        {
            DataTable tbl = grcCanDoi.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
            {
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                return;
            }

            bool isChangeBOM = tbl.AsEnumerable()
                .Any(row => row["IsSuaBOM"] != DBNull.Value && Convert.ToInt32(row["IsSuaBOM"]) == 0);

            if (!isChangeBOM)
            {
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                return;
            }

            bool hasUnconfirmed = tbl.AsEnumerable()
                .Any(row => row.Table.Columns.Contains("Status")
                         && row["Status"] != DBNull.Value
                         && row["Status"].ToString() == "Chưa xác nhận");

            if (hasUnconfirmed && isChangeBOM)
            {
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                DataRow focusedRow = grvLenhChia.GetFocusedDataRow();
                if (focusedRow == null) return;

                string makh = focusedRow["MaKH"]?.ToString() ?? "";
                string mahang = focusedRow["MaHang"]?.ToString() ?? "";
                string userName = GlobleData.UserName ?? "";
                string madot = focusedRow["MaDot"]?.ToString() ?? "";
                string maLenhSX = focusedRow["MaLenhSanXuat"]?.ToString() ?? "";
                string magop = focusedRow["MaGop"]?.ToString() ?? "";

                int isSuaNPL = 0;
                if (tbl.Columns.Contains("IsSuaNPL"))
                {
                    var rowHasValue = tbl.AsEnumerable()
                        .FirstOrDefault(r =>
                            r["IsSuaNPL"] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(r["IsSuaNPL"].ToString()));

                    if (rowHasValue != null)
                        int.TryParse(rowHasValue["IsSuaNPL"].ToString(), out isSuaNPL);
                }
                string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=UpdateVT&para1={makh}&para2={mahang}&para3={magop}&para5={maLenhSX}&para6={userName}&para7={madot}&para10={isSuaNPL}";
                string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
            }
            else
            {
                layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem31.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
        }
        private void searchLookUpEditPO_Properties_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietCanDoi();
        }
        private void searchLookUpEditMau_Properties_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietCanDoi();
        }

        private void searchLookUpEditLine_Properties_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietCanDoi();
        }

        private void GetChiTietCanDoi()
        {
            try
            {
                string PO = (searchLookUpEditPO.EditValue as string) ?? "";
                string Mau = (searchLookUpEditMau.EditValue as string) ?? "";
                string Line = (searchLookUpEditLine.EditValue as string) ?? "";
                string url = $"{URL}ERPDonHangTong/Get?action=GetChiTietLenh&para={_maDH}&para2={PO}&para3={Mau}&para4={Line}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcChiTietCanDoi.DataSource = null;
                    return;
                }

                grcChiTietCanDoi.DataSource = tbl;
                CreateBandSize(tbl, bandedGridView1, gbSize, repotxtN0);
                AmoutRow(tbl);
            }
            catch (Exception ex)
            {

            }
        }
        private void GetPO()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetPO&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditPO.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditPO.Properties.DataSource = tbl;

            }
            catch (Exception ex)
            {

            }
        }
        private void GetMau()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetMau&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditMau.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditMau.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetLine()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetLineCT&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditLine.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditLine.Properties.DataSource = tbl;


            }
            catch (Exception ex)
            {

            }
        }
        private void GetChiTietDH()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetChiTiet&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    txtDH.EditValue = null;
                    txtLenhSX.EditValue = null;
                    txtMaHang.EditValue = null;
                    txtChuyenGoc.EditValue = null;
                    txtDotSX.EditValue = null;
                    return;
                }

                txtDH.EditValue = tbl.Rows[0]["GopDH"];
                txtLenhSX.EditValue = tbl.Rows[0]["MaLenh"];
                txtMaHang.EditValue = tbl.Rows[0]["MaHang"];
                txtChuyenGoc.EditValue = tbl.Rows[0]["Name"];
                txtDotSX.EditValue = tbl.Rows[0]["DotSX"];

                return;

            }
            catch (Exception ex)
            {

            }
        }
        private void AmoutRow(DataTable _data)
        {
            if (!_data.Columns.Contains("Amount"))
                _data.Columns.Add("Amount", typeof(int));

            foreach (DataRow row in _data.Rows)
            {
                int total = 0;
                foreach (DataColumn column in _data.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        total += Convert.ToInt32(row[column]);
                    }
                }
                row["Amount"] = total;
            }

            // 🔸 Nếu cột Amount đã tồn tại trong grid → chỉnh lại thuộc tính
            BandedGridColumn colAmount = bandedGridView1.Columns["Amount"] as BandedGridColumn;
            if (colAmount == null)
            {
                // Nếu chưa có, tạo mới
                colAmount = new BandedGridColumn
                {
                    FieldName = "Amount",
                    Caption = "Tổng",
                    Visible = true,
                    Width = 80,
                    OptionsColumn = { AllowEdit = false }
                };
                bandedGridView1.Columns.Add(colAmount);
            }
            else
            {
                // Nếu có, chỉnh caption/format nếu cần
                colAmount.Caption = "Tổng số lượng";
                colAmount.Width = 100;
            }

            // 🔸 Format số
            colAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colAmount.DisplayFormat.FormatString = "n0";

            // 🔸 Xóa summary cũ nếu có
            colAmount.Summary.Clear();

            // 🔸 Thêm summary mới
            colAmount.Summary.Add(DevExpress.Data.SummaryItemType.Sum, "Amount", "{0:n0}");
            bandedGridView1.OptionsView.ShowFooter = true;
        }
        public static void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA, DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repotxtN0, bool IsCheckSumSize = true)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];


                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = colName;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = repotxtN0;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                if (IsCheckSumSize)
                {
                    if (col.FieldName.Contains("@"))
                    {
                        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                        itemSize.FieldName = col.FieldName;
                        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        itemSize.DisplayFormat = "{0:n0}";
                        itemSize.ShowInGroupColumnFooter = col;
                        grvShared.GroupSummary.Add(itemSize);
                    }
                    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrName.Length > 1)
                    {
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    }
                }

                // Thêm cột vào grid
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });

                // Tạo và thêm GridBand vào grid
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
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }



        public static void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }
        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            int isLineGoc = 0;
            if (row.Table.Columns.Contains("IsLineGoc"))
                int.TryParse(row["IsLineGoc"].ToString(), out isLineGoc);

            if (e.Column.FieldName == "Name" && isLineGoc == 1)
            {

                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");   // màu nền đỏ nhạt
                e.Appearance.ForeColor = Color.Black;

            }
            var selectedCells = view.GetSelectedCells();
            bool isCellSelected = selectedCells.Any(c =>
                c.RowHandle == e.RowHandle &&
                c.Column.FieldName == e.Column.FieldName
            );
            if (isCellSelected)
            {
                return; // Để GridView tự tô màu selection
            }
            if (e.Column == null || !e.Column.FieldName.Contains("@")) return;
            if (isLineGoc == 1)
            {
                // Màu nền xám nhạt #EAEAEA
                Color bg = Color.FromArgb(234, 234, 234);
                e.Appearance.BackColor = bg;
                e.Appearance.Options.UseBackColor = true;

                // Tùy chọn: đổi màu chữ cho rõ
                //e.Appearance.ForeColor = Color.Gray;
                e.Appearance.Options.UseForeColor = true;
            }
            else
            {
                // Trả lại mặc định (nếu bạn cần)
                // e.Appearance.Reset(); // hoặc đặt trắng, tuỳ bạn
            }
        }
        private void grv_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as GridView; // hoặc BandedGridView nếu bạn dùng BandedGrid
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            // Lấy giá trị CheckDuyet và Status
            int checkDuyet = 0;
            int status = 0;
            int isCheckKichHoat = 0;
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(234, 234, 234);
                e.Appearance.ForeColor = Color.Black;
                return;
            }
            if (row.Table.Columns.Contains("CheckDuyet"))
                int.TryParse(row["CheckDuyet"]?.ToString(), out checkDuyet);
            if (row.Table.Columns.Contains("isCheckKichHoat"))
                int.TryParse(row["isCheckKichHoat"]?.ToString(), out isCheckKichHoat);
            if (row.Table.Columns.Contains("Status"))
            {
                if (row["Status"] is bool boolValue)
                    status = boolValue ? 1 : 0;
                else
                    int.TryParse(row["Status"]?.ToString(), out status);
            }

            // ✅ ÁP DỤNG MÀU CHO CẢ DÒNG DỰA TRÊN TRẠNG THÁI
            if (checkDuyet == 0)
            {
                // 🔸 Chưa duyệt (CheckDuyet = 0) → Màu trắng/mặc định
                e.Appearance.BackColor = Color.White;
                e.Appearance.ForeColor = Color.Black;
            }
            else if (isCheckKichHoat == 3)
            {
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#9B5DE0"); // tím
                e.Appearance.ForeColor = Color.Black;
            }
            else if (checkDuyet == 1 && status == 0)
            {
                // 🟡 Đã duyệt nhưng chưa kích hoạt → Màu vàng
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99"); // Vàng nhạt
                e.Appearance.ForeColor = Color.Black;
            }
            else if (checkDuyet == 1 && status == 1)
            {
                // 🟢 Đã duyệt và đã kích hoạt → Màu xanh
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184"); // Xanh lá nhạt
                e.Appearance.ForeColor = Color.Black;
            }

            e.Appearance.Options.UseBackColor = true;
            e.Appearance.Options.UseForeColor = true;
            if (e.Column.FieldName != "TrangThai")
                return;

            // Kiểm tra null và chuyển về chuỗi an toàn
            if (e.CellValue == null)
                return;
            if (e.CellValue.ToString() == "Đã duyệt lệnh")// Đã ss cấp phát
            {
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcad4");
                e.Appearance.ForeColor = Color.DarkGreen;
            }
            //else if (e.CellValue.ToString() == "Đã cấp phát") // Đã cp
            //{
            //    e.Appearance.BackColor = Color.LightYellow;
            //    e.Appearance.ForeColor = Color.OrangeRed;
            //}
            else if (e.CellValue.ToString() == "Đã duyệt BOM") // Đã có BOM
            {
                e.Appearance.BackColor = Color.LightSkyBlue;
                e.Appearance.ForeColor = Color.DarkBlue;
            }
            else if (e.CellValue.ToString() == "Đã hủy BOM")
            {
                e.Appearance.ForeColor = Color.Red;
            }
        }
        private void grvCanDoi_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            //var view = sender as GridView; // hoặc BandedGridView nếu bạn dùng BandedGrid
            //if (view == null) return;

            //DataRow row = null;
            //try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            //if (row == null) return;

            //// Lấy giá trị CheckDuyet và Status
            //int checkDuyet = row["Status"].ToString() == "Đã xác nhận" ? 1 : 0;
            //int status = 0;


            //// ✅ ÁP DỤNG MÀU CHO CẢ DÒNG DỰA TRÊN TRẠNG THÁI
            //if (checkDuyet == 0)
            //{
            //    // 🔸 Chưa duyệt (CheckDuyet = 0) → Màu trắng/mặc định
            //    e.Appearance.BackColor = Color.White;
            //    e.Appearance.ForeColor = Color.Black;
            //}
            //else if (checkDuyet == 1)
            //{
            //    // 🟡 Đã duyệt nhưng chưa kích hoạt → Màu vàng
            //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184"); // Vàng nhạt
            //    e.Appearance.ForeColor = Color.Black;
            //}
            //e.Appearance.Options.UseBackColor = true;
            //e.Appearance.Options.UseForeColor = true;
        }
        private void bandedGridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;
            if (view.FocusedColumn == null) return;

            // Chỉ quan tâm cột size động
            if (!view.FocusedColumn.FieldName.Contains("@")) return;

            DataRow row = null;
            try { row = view.GetDataRow(view.FocusedRowHandle); } catch { row = null; }
            if (row == null) return;

            int isLineGoc = 0;
            if (row.Table.Columns.Contains("IsLineGoc"))
                int.TryParse(row["IsLineGoc"].ToString(), out isLineGoc);

            if (isLineGoc == 1)
            {
                // Chặn mở editor → readonly trên từng dòng
                e.Cancel = true;
            }
        }
        private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Chỉ áp dụng cho cột size động
            if (e.Column == null || !e.Column.FieldName.Contains("@")) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.ListSourceRowIndex); } catch { row = null; }
            if (row == null) return;

            int isLineGoc = 0;
            if (row.Table.Columns.Contains("IsLineGoc"))
                int.TryParse(row["IsLineGoc"].ToString(), out isLineGoc);

            int value;
            // Kiểm tra xem text hiện tại có phải là số không
            if (int.TryParse(e.DisplayText, out value))
            {
                if (value == 0)
                {
                    e.DisplayText = "-";
                }
            }
        }
        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Chỉ tính lại khi thay đổi các cột size (có ký tự "@")
            if (e.Column.FieldName.Contains("@"))
            {
                DataRow row = view.GetDataRow(e.RowHandle);
                if (row == null) return;

                int total = 0;
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        int val = 0;
                        int.TryParse(row[column].ToString(), out val);
                        total += val;
                    }
                }

                row["Amount"] = total;

                // Cập nhật lại giao diện ngay
                view.RefreshRow(e.RowHandle);
            }
        }
        private void BandedGridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                var view = sender as BandedGridView;
                if (view == null) return;

                var selectedCells = view.GetSelectedCells();
                if (selectedCells == null || selectedCells.Length == 0) return;

                // ✅ LẤY DANH SÁCH CÁC DÒNG KHÁC NHAU ĐƯỢC CHỌN
                var distinctRows = selectedCells.Select(c => c.RowHandle).Distinct().ToList();

                // ✅ NẾU CHỌN QUA NHIỀU HƠN 1 DÒNG → HỦY SELECTION
                if (distinctRows.Count > 1)
                {
                    // Lấy dòng đầu tiên được chọn
                    int firstRow = distinctRows.First();

                    // Xóa tất cả selection
                    view.ClearSelection();

                    // Chỉ giữ lại các ô trong dòng đầu tiên
                    var cellsInFirstRow = selectedCells.Where(c => c.RowHandle == firstRow).ToList();

                    foreach (var cell in cellsInFirstRow)
                    {
                        view.SelectCell(cell.RowHandle, cell.Column);
                    }

                    // Thông báo (tùy chọn - có thể bỏ nếu thấy phiền)
                    // XtraMessageBox.Show("Chỉ được bôi đen trong 1 dòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void bandedGridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                var view = sender as BandedGridView;
                if (view == null || view.FocusedColumn == null) return;

                string fieldName = view.FocusedColumn.FieldName;
                if (!fieldName.Contains("@")) return; // chỉ áp dụng cho cột size động

                DataRow currentRow = view.GetDataRow(view.FocusedRowHandle);
                if (currentRow == null) return;

                // Nếu dòng hiện tại là chuyền gốc thì bỏ qua, không giới hạn
                if (currentRow["IsLineGoc"] != DBNull.Value && Convert.ToInt32(currentRow["IsLineGoc"]) == 1)
                    return;

                // Lấy giá trị người dùng nhập
                int newValue = 0;
                if (!int.TryParse(e.Value?.ToString(), out newValue))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập số hợp lệ.";
                    return;
                }


                if (Convert.ToInt32(e.Value) < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng phải lớn hơn 0";
                    return;
                }

                // Lấy giá trị cũ để tính delta
                int oldValue = 0;
                int.TryParse(currentRow[fieldName].ToString(), out oldValue);
                int delta = newValue - oldValue;  // số thay đổi so với trước

                // Xác định các khóa để tìm dòng gốc
                var poid = currentRow["POID"].ToString();
                var madh = currentRow["MaDH"].ToString();
                var mamau = currentRow["MaMau"].ToString();
                var dausizeid = currentRow["DauSizeID"].ToString();
                var codemau = currentRow["CodeMau"].ToString();

                // Lấy bảng nguồn
                DataTable tbl = view.GridControl.DataSource as DataTable;
                if (tbl == null) return;

                // Tìm dòng chuyền gốc
                var rowGoc = tbl.AsEnumerable().FirstOrDefault(r =>
                    r["POID"].ToString() == poid &&
                    r["MaDH"].ToString() == madh &&
                    r["MaMau"].ToString() == mamau &&
                    r["DauSizeID"].ToString() == dausizeid &&
                    r["CodeMau"].ToString() == codemau &&
                    Convert.ToInt32(r["IsLineGoc"]) == 1
                );

                if (rowGoc == null) return;

                int valueGoc = 0;
                int.TryParse(rowGoc[fieldName].ToString(), out valueGoc);

                // Kiểm tra không được vượt số gốc còn lại
                if (delta > valueGoc)
                {
                    e.Valid = false;
                    e.ErrorText = $"Không được nhập vượt quá số lượng còn lại ({valueGoc}).";
                    return;
                }

                // Nếu hợp lệ → cập nhật lại giá trị chuyền gốc
                rowGoc[fieldName] = valueGoc - delta;

                // Cập nhật lại Amount của cả dòng gốc và dòng hiện tại
                UpdateAmountForRow(currentRow);
                UpdateAmountForRow(rowGoc);
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateAmountForRow(DataRow row)
        {
            int total = 0;
            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.ColumnName.Contains("@"))
                {
                    int val = 0;
                    int.TryParse(row[col.ColumnName].ToString(), out val);
                    total += val;
                }
            }
            row["Amount"] = total;
        }

        private void BuildSaveTable()
        {
            var view = bandedGridView1;
            var tblSource = view.GridControl.DataSource as DataTable;
            if (tblSource == null) return;

            // Tạo DataTable kết quả

            List<CanDoiDonViSanXuatSaveEntity> listSave = new List<CanDoiDonViSanXuatSaveEntity>();
            // Duyệt từng dòng trong Grid
            foreach (DataRow row in tblSource.Rows)
            {
                string poid = row["POID"].ToString();
                string madh = row["MaDH"].ToString();
                string mamau = row["MaMau"].ToString();
                string dausizeid = row["DauSizeID"].ToString();
                string codemau = row["CodeMau"].ToString();
                string line = row["Line"].ToString();


                // Duyệt các cột size động
                foreach (DataColumn col in tblSource.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        string[] parts = col.ColumnName.Split('@');
                        if (parts.Length == 0) continue;
                        string size = parts[1];

                        int soLuong = 0;
                        int.TryParse(row[col.ColumnName].ToString(), out soLuong);

                        CanDoiDonViSanXuatSaveEntity itemSave = new CanDoiDonViSanXuatSaveEntity();

                        itemSave.POID = poid;
                        itemSave.MaDH = madh;
                        itemSave.MaMau = mamau;
                        itemSave.DauSizeID = dausizeid;
                        itemSave.Line = line;
                        itemSave.SizeID = size;
                        itemSave.SoLuong = soLuong;
                        itemSave.MaLenhSanXuat = _maLenhSX;
                        listSave.Add(itemSave);
                    }
                }
            }
            if (listSave == null || listSave.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để lưu.");
                return;
            }
            string url = string.Format("{0}", URL + $"ERPDonHangTong/Post?action=PostSoLuongChuyen&type=@TypeTable");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                GetChiTietCanDoi();
            }
        }
        private void GridControl_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopySizeCellsToClipboard();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteClipboardToSizeCells();
                e.Handled = true;
            }
        }
        private void CopySizeCellsToClipboard()
        {
            try
            {
                var view = bandedGridView1;

                // ✅ LẤY DANH SÁCH CÁC Ô ĐÃ CHỌN
                var selectedCells = view.GetSelectedCells();

                if (selectedCells == null || selectedCells.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng bôi đen các ô cần copy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ LỌC CHỈ LẤY CÁC Ô SIZE (có ký tự @)
                var sizeCells = selectedCells.Where(c => c.Column.FieldName.Contains("@")).ToList();

                if (sizeCells.Count == 0)
                {
                    XtraMessageBox.Show("Chỉ có thể copy các cột Size!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ NHÓM CÁC Ô THEO DÒNG
                var cellsByRow = sizeCells
                    .GroupBy(c => c.RowHandle)
                    .OrderBy(g => g.Key)
                    .ToList();

                StringBuilder clipboardText = new StringBuilder();

                // ✅ DUYỆT TỪNG DÒNG
                foreach (var rowGroup in cellsByRow)
                {
                    // Sắp xếp các ô trong dòng theo thứ tự cột
                    var cellsInRow = rowGroup.OrderBy(c => c.Column.VisibleIndex).ToList();

                    List<string> rowValues = new List<string>();

                    foreach (var cell in cellsInRow)
                    {
                        DataRow row = view.GetDataRow(cell.RowHandle);
                        if (row == null) continue;

                        object value = row[cell.Column.FieldName];

                        // Chuyển về chuỗi
                        string strValue = "";
                        if (value != null && value != DBNull.Value)
                        {
                            int intValue = 0;
                            if (int.TryParse(value.ToString(), out intValue))
                            {
                                strValue = intValue.ToString();
                            }
                        }
                        rowValues.Add(strValue);
                    }

                    // Nối các giá trị trong dòng bằng TAB
                    clipboardText.AppendLine(string.Join("\t", rowValues));
                }

                // ✅ ĐƯA VÀO CLIPBOARD
                if (clipboardText.Length > 0)
                {
                    Clipboard.SetText(clipboardText.ToString().TrimEnd());

                    // Thông báo (tùy chọn)
                    // XtraMessageBox.Show($"Đã copy {sizeCells.Count} ô!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void PasteClipboardToSizeCells()
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardText)) return;

                var view = bandedGridView1;
                if (view.FocusedRowHandle < 0 || view.FocusedColumn == null) return;

                DataTable tbl = grcChiTietCanDoi.DataSource as DataTable;

                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                if (row == null) return;

                // 🔸 Kiểm tra dòng hiện tại có phải chuyền gốc không
                if (row.Table.Columns.Contains("IsLineGoc") &&
                    int.TryParse(row["IsLineGoc"]?.ToString(), out int isLineGoc) &&
                    isLineGoc == 1)
                {
                    XtraMessageBox.Show("Không thể paste vào dòng chuyền gốc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 🔸 Tách dữ liệu từ clipboard
                string[] values = clipboardText
                    .Split(new[] { '\t', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length == 0) return;

                // 🔸 Lấy danh sách cột Size động từ DataTable (theo đúng thứ tự bạn muốn)
                var sizeColumns = tbl.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.Contains("@"))
                    .ToList();

                // 🔸 Giới hạn tránh tràn chỉ số
                int colCount = Math.Min(values.Length, sizeColumns.Count);

                List<string> errors = new List<string>();
                int successCount = 0;

                for (int i = 0; i < colCount; i++)
                {
                    string valueText = values[i].Trim();
                    if (string.IsNullOrWhiteSpace(valueText)) continue;

                    var column = sizeColumns[i];
                    string fieldName = column.ColumnName; // ✅ Dùng ColumnName vì FieldName = ColumnName

                    // 🔸 Kiểm tra giá trị hợp lệ
                    if (!int.TryParse(valueText, out int newVal))
                    {
                        errors.Add($"Cột '{column.ColumnName}': giá trị '{valueText}' không hợp lệ");
                        continue;
                    }

                    // 🔸 Kiểm tra hợp lệ qua sự kiện ValidatingEditor
                    var gridCol = view.Columns
                        .Cast<BandedGridColumn>()
                        .FirstOrDefault(c => c.FieldName == fieldName);

                    if (gridCol != null)
                    {
                        view.FocusedColumn = gridCol;
                        var validateArgs = new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs(valueText);
                        bandedGridView1_ValidatingEditor(view, validateArgs);

                        if (!validateArgs.Valid)
                        {
                            errors.Add($"Cột '{gridCol.Caption}': {validateArgs.ErrorText}");
                            continue;
                        }
                    }

                    // 🔸 Gán giá trị
                    row[fieldName] = newVal;
                    successCount++;
                }

                // 🔸 Cập nhật Amount và refresh view
                if (successCount > 0)
                {
                    UpdateAmountForRow(row);
                    view.RefreshRow(view.FocusedRowHandle);
                }

                // 🔸 Hiển thị kết quả
                if (errors.Count > 0)
                {
                    string message = $"Đã paste thành công {successCount}/{values.Length} giá trị.\n\n";
                    message += "Các lỗi:\n" + string.Join("\n", errors.Take(10));
                    if (errors.Count > 10)
                        message += $"\n... và {errors.Count - 10} lỗi khác";

                    XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi paste dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnChiaChuyenItem_Click(object sender, EventArgs e)
        {
            DataRow dtRow = grvLenhChia.GetFocusedDataRow();
            if (dtRow == null) return;
            string gopDH = dtRow["GopDH"].ToString();
            string maGop = dtRow["MaGop"].ToString();
            using (var frm = new frmCanDoiDHChiaChuyen(gopDH, _maLenhSX, maGop))
            {

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    GetChiTietCanDoi();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void btnDoiChuyenItem_Click(object sender, EventArgs e)
        {
            DataRow dtRow = grvLenhChia.GetFocusedDataRow();
            if (dtRow == null) return;
            string gopDH = dtRow["GopDH"].ToString();
            string maGop = dtRow["MaGop"].ToString();
            using (var frm = new frmCanDoiDHDoiChuyen(_maDH, _maLenhSX, gopDH, maGop))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    GetChiTietCanDoi();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void btnXoaChuyenItem_Click(object sender, EventArgs e)
        {
            DataRow dtRow = grvLenhChia.GetFocusedDataRow();
            if (dtRow == null) return;
            string gopDH = dtRow["GopDH"].ToString();
            string maGop = dtRow["MaGop"].ToString();
            //if (Convert.ToInt32(dtRow["CheckDuyet"]) == 1)
            //{
            //    XtraMessageBox.Show("Lệnh này đã được duyệt trước đó nên không xóa được chuyền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            using (var frm = new frmCanDoiDHXoaChuyen(_maDH, _maLenhSX, gopDH, maGop))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    GetChiTietCanDoi();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void btnSaveItem_Click(object sender, EventArgs e)
        {
            BuildSaveTable();
        }

        private void btnDuyetItem_Click(object sender, EventArgs e)
        {
            if (_isGiaCong)
            {
                return;
            }
            var dt = grcLenhChia.DataSource as DataTable;
            if (dt == null) return;

            // Tìm dòng có MaLenh = _lenhSX
            DataRow rowLenh = dt.AsEnumerable()
                                .FirstOrDefault(r => r["MaLenh"]?.ToString() == _lenhSX);

            if (rowLenh == null)
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                }
                XtraMessageBox.Show("Không tìm thấy lệnh sản xuất trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //// Kiểm tra đã duyệt chưa
            //if (rowLenh["CheckDuyet"] != DBNull.Value && Convert.ToInt32(rowLenh["CheckDuyet"]) == 1)
            //{
            //    XtraMessageBox.Show("Lệnh này đã được duyệt trước đó!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            if (!IsDuyetKichHoatlenh)
            {
                var confirm = XtraMessageBox.Show(
                    $"Bạn có chắc muốn xác nhận lệnh sản xuất: {_lenhSX} này không?",
                    "Xác nhận duyệt lệnh",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm != DialogResult.Yes)
                {
                    _isDuyetCancelled = true;
                    return;
                }
            }
            _isDuyetCancelled = false;

            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(
                    this,
                    typeof(Modules.ResourceForm.frmWait),
                    true,
                    true,
                    false
                );
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Vui lòng chờ...");

            bool ischanged = IsChangedSL();
            if (ischanged)
            {
                DongBoSoLuong();
                CheckChangedSoLuong();
                UpdateIsXNSD();
            }

            string userName = GlobleData.UserName ?? "";
            string makh = rowLenh["MaKH"].ToString() ?? "";
            string mahang = rowLenh["MaHang"].ToString() ?? "";
            string madot = string.Empty;
            string urlDot = $"{URL}ERPDonHangTong/Get?Action=GETMAXDOT&para={makh}&para2={mahang}";
            string jsonDot = Task.Run(async () => await _clientExtension.GetAsnyc(urlDot)).Result;

            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(jsonDot);

            if (tblDot != null && tblDot.Rows.Count != 0)
            {
                madot = tblDot.Rows[0]["MaDot"].ToString();

            }
            // Nếu chưa duyệt → gọi API để duyệt
            string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=GET_TSNEW&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
            string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;

            string urlXN = $"{URL}ERPDonHangTong/Get?action=XacNhanBOM&para={_lenhSX}&para2={makh}&para3={mahang}";
            string jsonXN = Task.Run(async () => await _clientExtension.GetAsnyc(urlXN)).Result;

            string url = $"{URL}ERPDongBoQLSX/DongBo?action=DongBo&para={_lenhSX}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            string urlIsKeoVe = $"{URL}ERPDonHangTong/GetChuyen?action=PostIsKeoVe&para={_maDH}&para5={_maLenhSX}";
            string jsonKeoVe = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlIsKeoVe); }).Result;
            string _Module = "ChiaChuyenVaDuyetSX";
            string _Action = "Xác nhận lệnh";
            string _Contents = _maDH + "@" + _maLenhSX;
            string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
            string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;

            if (json.ToUpper() != "2")
            {
                rowLenh["CheckDuyet"] = 1;
                string Title = "Lệnh sản xuất đã xác nhận số lượng";
                DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                if (rowLenhFocused == null) return;
                string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
                string SendTo = "PKH";
                if (!ischanged && !IsDuyetKichHoatlenh)
                {
                    SendNotify(Title, Detail, SendTo, "ALL", 1);
                }

                grcLenhChia.RefreshDataSource();
                //clsWaitForm.ShowSuccessForm(this, 2000);

            }
            else
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                }
                XtraMessageBox.Show("Duyệt thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            LoadCanDoi();
        }
        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (_lastSourceData != null && _lastSourceData.Rows.Count > 0)
            //{
            //    string oldFilter = grvLenhChia.ActiveFilterString;

            //    grcLenhChia.DataSource = _lastSourceData.Copy();

            //    grvLenhChia.ActiveFilterString = oldFilter;
            //    if (grvLenhChia.RowCount > 0)
            //    {
            //        grvLenhChia.FocusedRowHandle = 0;
            //    }
            //}
            //else
            //{
            //    LoadLenhSanXuat();
            //}
            ReloadData();
            //GetChiTietCanDoi();
            //LoadCanDoi();
        }


        private void grvLenhChia_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow drRow = grvLenhChia.GetFocusedDataRow();
            if (drRow == null) return;
            _maHang = drRow["MaHang"]?.ToString() ?? "";
            _maDH = drRow["MaDH"].ToString();
            _maLenhSX = drRow["MaLenhSanXuat"].ToString();
            _lenhSX = drRow["MaLenh"].ToString();
            CheckGiaCong();
            GetChiTietDH();
            GetChiTietCanDoi();
            GetPO();
            GetMau();
            GetLine();
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(
                    this,
                    typeof(Modules.ResourceForm.frmWait),
                    true,
                    true,
                    false
                );
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Vui lòng chờ...");

                LoadCanDoi();
                CheckIsUpdateBOM();
                CheckChangedSoLuong();
                GetIsDuyetLai();
                GetIsXNSD();
                CheckHasChangedBatch();
            }
            finally
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            //LoadCanDoi();
            //CheckIsUpdateBOM();
            //CheckChangedSoLuong();
            //GetIsDuyetLai();
        }
        private void grvLenhChia_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || view.FocusedColumn == null) return;

            // Chỉ xử lý khi người dùng click vào cột Status (checkbox)
            if (view.FocusedColumn.FieldName != "Status") return;

            DataRow row = view.GetDataRow(view.FocusedRowHandle);
            if (row == null) return;

            // Lấy các giá trị liên quan
            int checkDuyet = 0;
            if (row.Table.Columns.Contains("CheckDuyet"))
                int.TryParse(row["CheckDuyet"]?.ToString(), out checkDuyet);

            int currentStatus = 0;
            if (row.Table.Columns.Contains("Status"))
            {
                if (row["Status"] is bool boolValue)
                    currentStatus = boolValue ? 1 : 0;
                else
                    int.TryParse(row["Status"]?.ToString(), out currentStatus);
            }

            int isCheckKichHoat = 0;
            if (row.Table.Columns.Contains("isCheckKichHoat"))
                int.TryParse(row["isCheckKichHoat"]?.ToString(), out isCheckKichHoat);

            string maLenh = row.Table.Columns.Contains("MaLenh") ? row["MaLenh"]?.ToString() : "";

            //// ✅ 1. Nếu CheckDuyet = 0 và Status hiện tại = 0 → Không cho bật kích hoạt
            //if (checkDuyet == 0 && currentStatus == 0)
            //{
            //    XtraMessageBox.Show(
            //        "Vui lòng duyệt trước khi Kích hoạt!",
            //        "Cảnh báo",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Warning
            //    );
            //    e.Cancel = true;
            //    return;
            //}

            // ✅ 2. Nếu đang bật (Status = 1) mà isCheckKichHoat = 1 → Không cho tắt
            if (currentStatus == 1 && isCheckKichHoat == 1)
            {
                //TEST
                //XtraMessageBox.Show(
                //    $"Mã lệnh {maLenh} đã được kích hoạt, không được phép hủy kích hoạt!",
                //    "Cảnh báo",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Warning
                //);
                //e.Cancel = true;
                return;
            }
        }
        private void ReloadData()
        {
            try
            {
                string oldFilter = grvLenhChia.ActiveFilterString;
                string currentMaLenhSX = _maLenhSX;
                //IsDuyetKichHoatlenh = false;
                DataTable tblNew = null;

                if (_isFromSearch)
                {
                    string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para={_searchMaHang}&para2={_searchMaLenh}&para3={_searchChungLoai}&para4={_searchKhachHang}&para5={_searchDot}&para6={_searchPO}";
                    string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                    tblNew = JsonConvert.DeserializeObject<DataTable>(json);
                }
                else if (_isFromAll)
                {
                    string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para=all";
                    string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                    tblNew = JsonConvert.DeserializeObject<DataTable>(json);
                }
                else
                {
                    string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para={_maHang}";
                    string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                    tblNew = JsonConvert.DeserializeObject<DataTable>(json);
                }

                if (tblNew == null || tblNew.Rows.Count == 0)
                {
                    grcLenhChia.DataSource = null;
                    return;
                }

                if (!tblNew.Columns.Contains("Sort"))
                {
                    tblNew.Columns.Add("Sort", typeof(int));
                    foreach (DataRow r in tblNew.Rows) r["Sort"] = 0;
                }

                DataTable tblMaDHGop = GopVaTongLenhChiaSX(tblNew);

                foreach (DataRow r in tblMaDHGop.Rows)
                {
                    if (r["GopDH"].ToString() == _maDH || r["GopDH"].ToString().Contains(_maDH))
                        r["Sort"] = 1;
                }

                DataView dv = tblMaDHGop.DefaultView;
                dv.Sort = "Sort DESC";
                tblMaDHGop = dv.ToTable();
                if (!tblMaDHGop.Columns.Contains("IsChon"))
                {
                    tblMaDHGop.Columns.Add("IsChon", typeof(bool));
                    foreach (DataRow r in tblMaDHGop.Rows)
                        r["IsChon"] = false;
                }
                grcLenhChia.DataSource = tblMaDHGop;
                _lastSourceData = tblMaDHGop.Copy();

                if (tblMaDHGop.Rows.Count > 0)
                {
                    _maDH = tblMaDHGop.Rows[0]["MaDH"].ToString();
                    _maLenhSX = tblMaDHGop.Rows[0]["MaLenhSanXuat"].ToString();
                }

                grvLenhChia.ActiveFilterString = oldFilter;

                bool found = false;
                for (int i = 0; i < grvLenhChia.RowCount; i++)
                {
                    var row = grvLenhChia.GetDataRow(i);
                    if (row != null && row["MaLenhSanXuat"]?.ToString() == currentMaLenhSX)
                    {
                        grvLenhChia.FocusedRowHandle = i;
                        grvLenhChia.SelectRow(i);
                        grvLenhChia.MakeRowVisible(i);
                        found = true;
                        break;
                    }
                }
                if (!found && grvLenhChia.RowCount > 0)
                {
                    grvLenhChia.FocusedRowHandle = 0;
                    grvLenhChia.SelectRow(0);
                }

                grvLenhChia_FocusedRowChanged(grvLenhChia, new FocusedRowChangedEventArgs(-1, grvLenhChia.FocusedRowHandle));
                GetIsDuyetLai();
                CheckChangedSoLuong();
                CheckIsUpdateBOM();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi tải lại dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ResetIsChon(List<DataRow> rows)
        {
            foreach (var r in rows)
                r["IsChon"] = false;
            grcLenhChia.RefreshDataSource();
        }
        private void ResetIsChonByError(List<string> bomError, List<string> xacNhanError)
        {
            DataTable dt = grcLenhChia.DataSource as DataTable;
            if (dt == null) return;

            foreach (DataRow row in dt.Rows)
            {
                string maLenh = row["MaLenh"]?.ToString();
                if (string.IsNullOrEmpty(maLenh)) continue;

                if (bomError.Contains(maLenh) || xacNhanError.Contains(maLenh))
                {
                    if (row.Table.Columns.Contains("IsChon"))
                        row["IsChon"] = false;
                }
            }
            grcLenhChia.RefreshDataSource();
        }
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            if (GlobleData.UserName == "admin" || GlobleData.UserName == "QLDH_01")
            {
                var selectedRows = new List<DataRow>();
                for (int i = 0; i < grvLenhChia.RowCount; i++)
                {
                    DataRow row = grvLenhChia.GetDataRow(i);
                    if (row == null) continue;
                    if (row["IsChon"] != DBNull.Value && Convert.ToBoolean(row["IsChon"]))
                        selectedRows.Add(row);
                }
                if (selectedRows.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng tick chọn lệnh cần duyệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var lenhChuaBOM = new List<string>();
                var lenhChuaXacNhan = new List<string>();
                var chuaKichHoat = new List<DataRow>();
                List<string> lenhError = new List<string>();
                foreach (var row in selectedRows)
                {
                    string malenhsx = row["MaLenhSanXuat"].ToString();
                    string malenh = row["MaLenh"].ToString();
                    string urlHasData = $"{URL}ERPDonHangTong/Get?action=HasData&para={malenhsx}";
                    string jsonHasData = Task.Run(async () => await _clientExtension.GetAsnyc(urlHasData)).Result;
                    if (string.IsNullOrEmpty(jsonHasData) || JsonConvert.DeserializeObject<DataTable>(jsonHasData)?.Rows.Count == 0)
                        lenhChuaBOM.Add(malenh);
                    string urlNPL = $"{URL}ERPDonHangTong/Get?action=GetNPL&para={row["MaDH"]?.ToString()}&para5={malenhsx}";
                    string jsonNPL = Task.Run(async () => await _clientExtension.GetAsnyc(urlNPL)).Result;
                    if (!string.IsNullOrEmpty(jsonNPL))
                    {
                        DataTable tblNPL = JsonConvert.DeserializeObject<DataTable>(jsonNPL);
                        if (tblNPL?.AsEnumerable().Any(r => r["Status"]?.ToString() == "Chưa xác nhận") == true)
                            lenhChuaXacNhan.Add(malenh);
                    }
                    bool isActivated = false;
                    if (row["Status"] is bool b) isActivated = b;
                    else bool.TryParse(row["Status"]?.ToString(), out isActivated);

                    if (!isActivated) chuaKichHoat.Add(row);
                }

                if (lenhChuaBOM.Count > 0)
                    lenhError.AddRange(lenhChuaBOM.Select(m => m + " (Chưa có BOM)"));

                if (lenhChuaXacNhan.Count > 0)
                    lenhError.AddRange(lenhChuaXacNhan.Select(m => m + " (Chưa xác nhận cấp phát)"));

                if (lenhError.Count > 0)
                {
                    string msg = $"Có {lenhError.Count} lệnh không đủ điều kiện:\n" +
                                 string.Join(", ", lenhError);
                    var lenhKhongHopLe = new HashSet<string>(lenhChuaBOM.Concat(lenhChuaXacNhan));
                    bool conLenhHopLe = selectedRows.Any(r => !lenhKhongHopLe.Contains(r["MaLenh"]?.ToString()));

                    if (!conLenhHopLe)
                    {
                        XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    var confirm = XtraMessageBox.Show(msg + "\nBạn có muốn loại bỏ (bỏ tick) các lệnh không phù hợp và tiếp tục duyệt các lệnh còn lại không?",
                                                      "Thông báo",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        ResetIsChonByError(lenhChuaBOM, lenhChuaXacNhan);
                    }
                    else
                    {
                        return;
                    }
                }
                var validRows = selectedRows.Where(r => !lenhChuaBOM.Contains(r["MaLenh"]?.ToString()) && !lenhChuaXacNhan.Contains(r["MaLenh"]?.ToString())).ToList();

                if (validRows.Count == 0)
                    return;
                foreach (var row in validRows)
                {
                    if (row.Table.Columns.Contains("Status"))
                    {
                        bool isActivated = false;
                        if (row["Status"] is bool b)
                            isActivated = b;
                        else
                            bool.TryParse(row["Status"]?.ToString(), out isActivated);
                        if (!isActivated)
                        {
                            row["Status"] = true;
                        }
                    }
                }
                grcLenhChia.RefreshDataSource();

                if (XtraMessageBox.Show($"Bạn có chắc muốn duyệt {validRows.Count} lệnh: {string.Join(", ", validRows.Select(r => r["MaLenh"]))}",
                    "Xác nhận duyệt lệnh", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    ResetIsChon(validRows);
                    return;
                }
                grvLenhChia.FocusedRowChanged -= grvLenhChia_FocusedRowChanged;
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(this, typeof(Modules.ResourceForm.frmWait), true, true, false);
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Đang duyệt lệnh...");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Vui lòng chờ...");

                try
                {
                    foreach (var row in validRows)
                    {
                        _maDH = row["MaDH"].ToString();
                        _maLenhSX = row["MaLenhSanXuat"].ToString();
                        _lenhSX = row["MaLenh"].ToString();

                        int rowHandle = -1;
                        if (grvLenhChia.DataSource is DataTable dt)
                        {
                            int dataIndex = dt.Rows.IndexOf(row);
                            if (dataIndex >= 0)
                            {
                                rowHandle = grvLenhChia.GetRowHandle(dataIndex);
                            }
                        }
                        if (rowHandle < 0)
                        {
                            for (int i = 0; i < grvLenhChia.RowCount; i++)
                            {
                                if (grvLenhChia.GetDataRow(i) == row)
                                {
                                    rowHandle = i;
                                    break;
                                }
                            }
                        }
                        if (rowHandle >= 0)
                            grvLenhChia.FocusedRowHandle = rowHandle;
                        IsDuyetKichHoatlenh = true;
                        _isDuyetCancelled = false;

                        btnDuyetItem_Click(sender, e);
                        btnXacNhanCanDoi_Click(sender, e);
                        bool ischanged = IsChangedSL(row);
                        if (!ischanged && !IsDuyetKichHoatlenh)
                        {
                            string TitleSL = "Lệnh sản xuất đã xác nhận số lượng";
                            string DetailSL = $"Đơn Hàng: {row["MaDH"]?.ToString()}\nMã Hàng: {row["TenHang"]?.ToString()}\nLệnh: {row["MaLenh"]?.ToString()}\nSố lượng: {row["SoLuong"]?.ToString()}";
                            SendNotify(TitleSL, DetailSL, "PKH", "ALL", 1, 0);
                        }


                        string TitleXN = "Lệnh sản xuất đã xác nhận cấp phát";
                        string DetailXN = $"Đơn Hàng: {row["MaDH"]?.ToString()}\nMã Hàng: {row["TenHang"]?.ToString()}\nLệnh: {row["MaLenh"]?.ToString()}\nSố lượng: {row["SoLuong"]?.ToString()}";
                        SendNotify(TitleXN, DetailXN, "PKH", "ALL", 1, 0);
                    }
                    foreach (var row in validRows)
                    {
                        string malenh = row["MaLenh"].ToString();
                        string malenhsx = row["MaLenhSanXuat"].ToString();
                        string magop = row["MaGop"].ToString();

                        string urlDuyet = $"{URL}ERPDonHangTong/Get?action=CheckDuyetLai&para={malenhsx}&para2={magop}";
                        string jsonDuyet = Task.Run(async () => await _clientExtension.GetAsnyc(urlDuyet)).Result;

                        if (!string.IsNullOrEmpty(jsonDuyet))
                        {
                            DataTable tblDuyet = JsonConvert.DeserializeObject<DataTable>(jsonDuyet);
                            if (tblDuyet != null && tblDuyet.Rows.Count > 0)
                            {
                                int isDuyetLai = 0;
                                int.TryParse(tblDuyet.Rows[0]["IsDuyetLai"]?.ToString(), out isDuyetLai);
                                if (isDuyetLai == 1)
                                {
                                    string urlUpdateDL = $"{URL}ERPDonHangTong/Get?action=DuyetLan2&para={malenhsx}&para2={magop}&para3={malenh}";
                                    string jsonUpdateDL = Task.Run(async () => await _clientExtension.GetAsnyc(urlUpdateDL)).Result;
                                }
                            }
                        }
                    }
                    List<CanDoiDonViSanXuatSaveEntity> listKichHoat = new List<CanDoiDonViSanXuatSaveEntity>();
                    foreach (var row in validRows)
                    {
                        listKichHoat.Add(new CanDoiDonViSanXuatSaveEntity
                        {
                            MaLenh = row["MaLenh"].ToString()
                        });
                    }
                    string url = $"{URL}ERPDonHangTong/Post?action=PostKichHoat&type=@TypeTable";
                    var msResult = Task.Run(async () => await _clientExtension.PostAsync(url, listKichHoat)).Result;

                    if (msResult.ToUpper() == "TRUE")
                    {
                        string _Module = "ChiaChuyenVaDuyetSX";
                        string _Action = "Duyệt Lệnh Win";

                        foreach (var item in listKichHoat)
                        {
                            string urlGetDL = $"{URL}ERPDonHangTong/Get?action=GetDLByLenh&para={item.MaLenh}";
                            string jsonGetDL = Task.Run(async () => await _clientExtension.GetAsnyc(urlGetDL)).Result;

                            if (string.IsNullOrEmpty(jsonGetDL)) continue;

                            DataTable tblDL = JsonConvert.DeserializeObject<DataTable>(jsonGetDL);
                            if (tblDL == null || tblDL.Rows.Count == 0) continue;

                            string maDH_Log = tblDL.Rows[0]["MaDH"]?.ToString() ?? "";
                            string maLenhSX_Log = tblDL.Rows[0]["MaLenhSanXuat"]?.ToString() ?? "";
                            string _Contents = maDH_Log + "@" + maLenhSX_Log + "@" + item.MaLenh;

                            string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                            string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
                        }
                        foreach (var row in validRows)
                        {
                            string TitleBanHanh = "Lệnh sản xuất đã được ban hành";
                            string DetailBanHanh = $"Đơn Hàng: {row["MaDH"]?.ToString()}\nMã Hàng: {row["TenHang"]?.ToString()}\nLệnh: {row["MaLenh"]?.ToString()}\nSố lượng: {row["SoLuong"]?.ToString()}";
                            SendNotify(TitleBanHanh, DetailBanHanh, "ALL");
                        }

                        foreach (var r in validRows) r["IsChon"] = false;
                        ReloadData();
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                }
                finally
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    grvLenhChia.FocusedRowChanged += grvLenhChia_FocusedRowChanged;
                    IsDuyetKichHoatlenh = false;
                }

            }
            else
            {
                XtraMessageBox.Show("Bạn không có quyền thực hiện thao tác này!!!",
                   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        //private void simpleButton6_Click(object sender, EventArgs e)
        //{
        //    if (GlobleData.UserName == "admin" || GlobleData.UserName == "QLDH_01")
        //    {
        //        if (!HasDataCanDoiNPL())
        //        {
        //            XtraMessageBox.Show($"Không thể duyệt lệnh khi chưa có BOM", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            DataRow drFocused = grvLenhChia.GetFocusedDataRow();
        //            if (drFocused != null)
        //            {
        //                drFocused["Status"] = false;
        //                grcLenhChia.RefreshDataSource();
        //            }
        //            return;
        //        }
        //        var tblCanDoi = grcCanDoi.DataSource as DataTable;
        //        if (tblCanDoi != null && tblCanDoi.Rows.Count > 0)
        //        {
        //            bool chuaXacNhan = tblCanDoi.AsEnumerable()
        //                .Any(row => row["Status"] != DBNull.Value
        //                         && row["Status"].ToString() == "Chưa xác nhận");

        //            if (chuaXacNhan)
        //            {
        //                XtraMessageBox.Show(
        //                    "Không thể duyệt lệnh khi cấp phát chưa được xác nhận!",
        //                    "Thông báo",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Warning
        //                );
        //                DataRow drFocused = grvLenhChia.GetFocusedDataRow();
        //                if (drFocused != null)
        //                {
        //                    drFocused["Status"] = false;
        //                    grcLenhChia.RefreshDataSource();
        //                }
        //                return;
        //            }
        //        }
        //        var dt = grcLenhChia.DataSource as DataTable;
        //        if (dt == null) return;

        //        // Lấy tất cả dòng có Status = true 
        //        //var selectedRows = dt.AsEnumerable()
        //        //                     .Where(r => r["Status"] != DBNull.Value && Convert.ToBoolean(r["Status"]))
        //        //                     .ToList();
        //        var selectedRows = new List<DataRow>();
        //        for (int i = 0; i < grvLenhChia.RowCount; i++)
        //        {
        //            DataRow row = grvLenhChia.GetDataRow(i);
        //            if (row == null) continue;
        //            if (row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]))
        //                selectedRows.Add(row);
        //        }
        //        // Không có dòng nào được chọn
        //        if (selectedRows.Count == 0)
        //        {
        //            XtraMessageBox.Show("Vui lòng chọn lệnh để kích hoạt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        _isDuyetCancelled = false;
        //        IsDuyetKichHoatlenh = true;
        //        btnDuyetItem_Click(sender, e);

        //        if (_isDuyetCancelled)
        //        {
        //            _isDuyetCancelled = false;
        //            DataRow drFocused = grvLenhChia.GetFocusedDataRow();
        //            if (drFocused != null)
        //            {
        //                drFocused["Status"] = false;
        //                grcLenhChia.RefreshDataSource();
        //            }
        //            return;
        //        }
        //        btnXacNhanCanDoi_Click(sender, e);

        //        //// Kiểm tra xem tất cả dòng được chọn đã kích hoạt hay chưa
        //        //bool allActivated = selectedRows.All(r => r["isCheckKichHoat"] != DBNull.Value && Convert.ToInt32(r["isCheckKichHoat"]) == 1);

        //        //if (allActivated)
        //        //{
        //        //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        //        //    XtraMessageBox.Show("Tất cả các lệnh đã được kích hoạt trước đó.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        //    return;
        //        //}

        //        DataRow dr = grvLenhChia.GetFocusedDataRow();
        //        if (dr == null) return;
        //        string malenh = dr["MaLenh"].ToString();
        //        string malenhsx = dr["MaLenhSanXuat"].ToString();
        //        string magop = dr["MaGop"].ToString();
        //        string urlDuyet = $"{URL}ERPDonHangTong/Get?action=CheckDuyetLai&para={malenhsx}&para2={magop}";
        //        string jsonDuyet = Task.Run(async () => await _clientExtension.GetAsnyc(urlDuyet)).Result;

        //        if (string.IsNullOrEmpty(jsonDuyet)) return;

        //        DataTable tblDuyet = JsonConvert.DeserializeObject<DataTable>(jsonDuyet);
        //        if (tblDuyet == null || tblDuyet.Rows.Count == 0) return;

        //        int isDuyetLai = 0;
        //        int.TryParse(tblDuyet.Rows[0]["IsDuyetLai"]?.ToString(), out isDuyetLai);
        //        if (isDuyetLai == 1)
        //        {
        //            string urlUpdateDL = $"{URL}ERPDonHangTong/Get?action=DuyetLan2&para={malenhsx}&para2={magop}&para3={malenh}";
        //            string jsonUpdateDL = Task.Run(async () => await _clientExtension.GetAsnyc(urlUpdateDL)).Result;
        //        }

        //        List<CanDoiDonViSanXuatSaveEntity> listKichHoat = new List<CanDoiDonViSanXuatSaveEntity>();

        //        //Nếu còn dòng chưa kích hoạt → kích hoạt chúng
        //        foreach (var row in selectedRows)
        //        {
        //            CanDoiDonViSanXuatSaveEntity itemKichHoat = new CanDoiDonViSanXuatSaveEntity();
        //            itemKichHoat.MaLenh = row["MaLenh"].ToString();

        //            row["isCheckKichHoat"] = 1;
        //            listKichHoat.Add(itemKichHoat);
        //        }

        //        string url = string.Format("{0}", URL + $"ERPDonHangTong/Post?action=PostKichHoat&type=@TypeTable");
        //        var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listKichHoat); }).Result;
        //        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        //        if (msResult.ToUpper() == "TRUE")
        //        {
        //            string _Module = "ChiaChuyenVaDuyetSX";
        //            string _Action = "Duyệt Lệnh Win";

        //            foreach (var item in listKichHoat)
        //            {
        //                string urlGetDL = $"{URL}ERPDonHangTong/Get?action=GetDLByLenh&para={item.MaLenh}";
        //                string jsonGetDL = Task.Run(async () => await _clientExtension.GetAsnyc(urlGetDL)).Result;

        //                if (string.IsNullOrEmpty(jsonGetDL)) continue;

        //                DataTable tblDL = JsonConvert.DeserializeObject<DataTable>(jsonGetDL);
        //                if (tblDL == null || tblDL.Rows.Count == 0) continue;

        //                string maDH_Log = tblDL.Rows[0]["MaDH"]?.ToString() ?? "";
        //                string maLenhSX_Log = tblDL.Rows[0]["MaLenhSanXuat"]?.ToString() ?? "";

        //                string _Contents = maDH_Log + "@" + maLenhSX_Log + "@" + item.MaLenh;
        //                string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
        //                string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
        //            }

        //            //LoadLenhSanXuat();
        //            //LoadCanDoi();
        //            string Title = "Lệnh sản xuất đã được ban hành";
        //            DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
        //            if (rowLenhFocused == null) return;
        //            string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
        //            string SendTo = "ALL";
        //            SendNotify(Title, Detail, SendTo);
        //            ReloadData();
        //            IsDuyetKichHoatlenh = false;
        //            clsWaitForm.ShowSuccessForm(this, 2000);
        //        }
        //    }
        //    else
        //    {

        //        XtraMessageBox.Show("Bạn không có quyền thực hiện thao tác này!!!",
        //           "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //}



        private void btnXuatEX_Click(object sender, EventArgs e)
        {
            DataTable dtTable = grcChiTietCanDoi.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("ChiaChuyenSanXuat{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "ChiaChuyenSanXuat.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, dtTable);

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

        private void grvCanDoi_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = grvCanDoi.GetFocusedDataRow();
            if (dr == null)
                return;
            var editedColumns = new[] { "SoLuong", "DinhMuc", "DinhMucHaoHut", "CapPhat", "POMua", "GhiChu" };

            if (editedColumns.Contains(e.Column.FieldName))
            {
                dr["IsEdited"] = true;
            }
            if (e.Column.FieldName == "CapPhat")
            {
                dr["IsCapPhatEdited"] = true;
            }
            //DinhMucHaoHut
            else if (e.Column.FieldName == "DinhMucHaoHut")
            {
                decimal soLuong = 0, dinhMuc = 0, dinhMucHaoHut = 0;

                // TryParse an toàn cho từng cột
                decimal.TryParse(Convert.ToString(dr["SoLuong"]), out soLuong);
                decimal.TryParse(Convert.ToString(dr["DinhMuc"]), out dinhMuc);
                decimal.TryParse(Convert.ToString(dr["DinhMucHaoHut"]), out dinhMucHaoHut);

                // Tính CapPhat
                decimal capPhat = Math.Round(soLuong * dinhMuc + soLuong * dinhMuc * (dinhMucHaoHut / 100), 2);

                dr["CapPhat"] = capPhat;

                // Refresh hiển thị
                grvCanDoi.RefreshRow(e.RowHandle);
            }
            if (e.Column.FieldName == "DinhMuc")
            {
                decimal soLuong = 0, dinhMuc = 0, dinhMucHaoHut = 0;

                // TryParse an toàn cho từng cột
                decimal.TryParse(Convert.ToString(dr["SoLuong"]), out soLuong);
                decimal.TryParse(Convert.ToString(dr["DinhMuc"]), out dinhMuc);
                decimal.TryParse(Convert.ToString(dr["DinhMucHaoHut"]), out dinhMucHaoHut);

                // Tính CapPhat
                decimal capPhat = Math.Round(soLuong * dinhMuc + soLuong * dinhMuc * (dinhMucHaoHut / 100), 2);

                dr["CapPhat"] = capPhat;

                // Refresh hiển thị
                grvCanDoi.RefreshRow(e.RowHandle);
            }
            if (e.Column.FieldName == "SoLuong")
            {
                decimal soLuong = 0, dinhMuc = 0, dinhMucHaoHut = 0, dinhMucKH = 0;

                // TryParse an toàn cho từng cột
                decimal.TryParse(Convert.ToString(dr["SoLuong"]), out soLuong);
                decimal.TryParse(Convert.ToString(dr["DinhMuc"]), out dinhMuc);
                decimal.TryParse(Convert.ToString(dr["DinhMucHaoHut"]), out dinhMucHaoHut);
                decimal.TryParse(Convert.ToString(dr["DinhMucChung"]), out dinhMucKH);

                decimal capPhatKH = Math.Round(soLuong * dinhMucKH, 2);

                // Tính CapPhat
                decimal capPhat = Math.Round(soLuong * dinhMuc + soLuong * dinhMuc * (dinhMucHaoHut / 100), 2);
                dr["CapPhatKH"] = capPhatKH;
                dr["CapPhat"] = capPhat;

                // Refresh hiển thị
                grvCanDoi.RefreshRow(e.RowHandle);
            }
        }
        public void Export(string TemplateFileName, string ExportFileName, DataTable dataTable)
        {
            try
            {
                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                    FileInfo templateFile = new FileInfo(TemplateFileName);

                    var tbl = dataTable.AsEnumerable()
                           .GroupBy(x => new { Line = x["Line"]?.ToString(), Name = x["Name"]?.ToString() })
                           .Select(g => g.First())
                           .CopyToDataTable();
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                        {
                            ExcelWorksheet templateSheet = templatePackage.Workbook.Worksheets["Sheet1"];
                            //string sheetName = string.IsNullOrWhiteSpace(tbl.Rows[i]["Name"]?.ToString()) ? "sheet1" : tbl.Rows[i]["Name"].ToString();
                            // Tạo worksheet mới từ template
                            ExcelWorksheet ws = excelPackage.Workbook.Worksheets.Add($"{tbl.Rows[i]["Name"]}", templateSheet);

                            ExcelRange range = ws.Cells;

                            DataTable tblDT = dataTable.AsEnumerable().Where(x => x["Line"].ToString() == tbl.Rows[i]["Line"].ToString()).CopyToDataTable();

                            List<string> columnNamesWithSize = tblDT.Columns.Cast<DataColumn>()
                                 .Where(column => column.ColumnName.Contains("@"))
                                 .Select(column => column.ColumnName)
                                 .Distinct()
                                 .ToList();
                            int colum = 6;
                            int countRow = 8;
                            range = ws.Cells[6, 6, 6, 6 + columnNamesWithSize.Count - 1]; range.Merge = true; range.Value = "Size";
                            range = ws.Cells[6, 6 + columnNamesWithSize.Count, 7, 6 + columnNamesWithSize.Count]; range.Merge = true;
                            range.Value = "Tổng";

                            foreach (var item in columnNamesWithSize)
                            {
                                var size = item.Split('@')[0];
                                ws.Cells[7, colum].Value = size;

                                colum++;
                            }
                            foreach (DataRow item in tblDT.Rows)
                            {
                                colum = 6;
                                ws.Cells[countRow, 1].Value = item["PO"];
                                ws.Column(1).AutoFit();
                                ws.Cells[countRow, 2].Value = item["DauSize"];
                                ws.Cells[countRow, 3].Value = item["TenMau"];
                                ws.Cells[countRow, 4].Value = item["CodeMau"];
                                ws.Cells[countRow, 5].Value = item["Name"];
                                ws.Column(2).AutoFit();
                                ws.Column(3).AutoFit();
                                ws.Column(4).AutoFit();
                                foreach (var size in columnNamesWithSize)
                                {
                                    var valueSize = item[size];
                                    ws.Cells[countRow, colum].Value = Convert.ToInt32(valueSize) == 0 ? "" : valueSize;
                                    colum++;
                                }
                                ws.Cells[countRow, colum].Value = item["Amount"];
                                countRow++;
                            }
                            int sumAmount = tblDT.AsEnumerable().Sum(x => Convert.ToInt32(x["Amount"]));
                            // Tạo list để lưu tổng từng cột size
                            List<decimal> sizeColumnSums = new List<decimal>();

                            // Duyệt qua từng cột size (tên trong columnNamesWithSize)
                            foreach (var columnName in columnNamesWithSize)
                            {
                                decimal sum = 0;
                                foreach (DataRow row in tblDT.Rows)
                                {
                                    if (row[columnName] != DBNull.Value)
                                    {
                                        // cố gắng parse thành decimal (hoặc double tùy bạn)
                                        decimal value;
                                        if (decimal.TryParse(row[columnName].ToString(), out value))
                                        {
                                            sum += value;
                                        }
                                    }
                                }
                                sizeColumnSums.Add(sum);
                            }
                            colum = 6;
                            range = ws.Cells[countRow, 1, countRow, 5]; range.Merge = true; range.Value = "Tổng";

                            foreach (var item in sizeColumnSums)
                            {
                                ws.Cells[countRow, colum].Value = item;
                                colum++;
                            }
                            ws.Cells[countRow, colum].Value = sumAmount;
                            ws.Cells[6, 1, countRow, colum].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[6, 1, countRow, colum].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range = ws.Cells[1, 1, 2, colum]; range.Merge = true; range.Value = "CHIA CHUYỀN SẢN XUẤT";
                            range.Style.Font.Bold = true; range.Style.Font.Size = 18;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var dataRange = ws.Cells[6, 1, countRow, colum];
                            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            DataRow drRow = grvLenhChia.GetFocusedDataRow();
                            if (drRow == null) return;
                            ws.Cells["A4"].Value = $"Mã ĐH: {drRow["MaDH"].ToString()}";
                            ws.Cells["B4"].Value = $"LSX: {drRow["MaLenh"].ToString()}";
                            ws.Cells["D4"].Value = $"Mã hàng: {drRow["MaHang"].ToString()}";
                            ws.Cells["E4"].Value = $"Khách hàng: {drRow["TenKH"].ToString()}";
                            ws.Cells["H4"].Value = $"Chủng loại: {drRow["TenCL"].ToString()}";
                        }
                    }

                    // Save all sheets into one file
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                // Bạn nên log hoặc throw lỗi để dễ debug
                Console.WriteLine(ex.Message);
            }
        }
        //private void btnXacNhanPOMua_Click(object sender, EventArgs e)
        //{
        //    int[] selectedHandles = grvCanDoi.GetSelectedRows();
        //    string POMua = (txtPOMua.EditValue as string) ?? "";
        //    if (POMua == "") return;
        //    if (selectedHandles.Length == 0)
        //    {
        //        XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng trước khi lưu!",
        //            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    foreach (int handle in selectedHandles)
        //    {
        //        DataRow row = grvCanDoi.GetDataRow(handle);
        //        if (row != null)
        //        {
        //            row["POMua"] = POMua;
        //        }
        //    }

        //    grvCanDoi.RefreshData();
        //    txtPOMua.EditValue = "";
        //}

        private void grvCanDoi_ShownEditor(object sender, EventArgs e)
        {
            object statusObj = grvCanDoi.GetFocusedRowCellValue("Status");
            if (statusObj == null) return;

            string trangThai = statusObj.ToString();
            string colName = grvCanDoi.FocusedColumn.FieldName;
            if (trangThai == "Đã xác nhận" && (colName == "SoLuong" || colName == "DinhMuc" || colName == "CapPhat" || colName == "DinhMucHaoHut"))
            {
                if (grvCanDoi.ActiveEditor != null)
                {
                    grvCanDoi.ActiveEditor.ReadOnly = true;
                }
            }
        }

        private void grvCanDoi_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            //object statusObj = grvCanDoi.GetRowCellValue(e.RowHandle, "Status");
            //if (statusObj == null) return;

            //if (statusObj.ToString() == "Đã xác nhận" &&
            //    (e.Column.FieldName == "SoLuong" || e.Column.FieldName == "DinhMuc"))
            //{
            //    e.RepositoryItem.ReadOnly = true;
            //}
        }

        private void grvLenhChia_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            if (e.Column.FieldName != "Status") return;

            DataRow row = view.GetDataRow(e.RowHandle);
            if (row == null) return;

            int isCheckKichHoat = 0;
            if (row.Table.Columns.Contains("isCheckKichHoat"))
                int.TryParse(row["isCheckKichHoat"]?.ToString(), out isCheckKichHoat);

            bool newValue = false;
            if (e.Value is bool b) newValue = b;
            else if (e.Value != null) bool.TryParse(e.Value.ToString(), out newValue);

            if (isCheckKichHoat == 1 && newValue == false)
            {
                if (GlobleData.UserName == "admin" || GlobleData.UserName == "QLDH_01")
                {
                    var confirm = XtraMessageBox.Show(
                    $"Bạn có chắc muốn hủy kích hoạt lệnh {row["MaLenh"]}?\n" +
                    "Lệnh này đã được kích hoạt trước đó.",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                    );

                    if (confirm == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        HuyKichHoatLenh(row["MaLenh"].ToString());

                    }
                }
                else
                {
                    XtraMessageBox.Show("Bạn không có quyền thực hiện thao tác này!!!",
                       "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    string maLenhHienTai = row["MaLenh"]?.ToString();
                    this.BeginInvoke(new Action(() => {
                        row["Status"] = true;
                        view.RefreshRow(e.RowHandle);
                    }));
                    return;

                }
            }
        }
        private void HuyKichHoatLenh(string maLenh)
        {
            try
            {
                List<CanDoiDonViSanXuatSaveEntity> listHuyKichHoat = new List<CanDoiDonViSanXuatSaveEntity>
            {
                new CanDoiDonViSanXuatSaveEntity { MaLenh = maLenh }
            };
                string url = $"{URL}ERPDonHangTong/Post?action=PostHuyKichHoat&type=@TypeTable";
                var msResult = Task.Run(async () => await _clientExtension.PostAsync(url, listHuyKichHoat)).Result;

                if (msResult.ToUpper() == "TRUE")
                {
                    DataTable dt = grcLenhChia.DataSource as DataTable;
                    if (dt.Rows.Count == 0 || dt == null) return;
                    if (dt != null)
                    {
                        var row = dt.AsEnumerable().FirstOrDefault(r => r["MaLenh"]?.ToString() == maLenh);
                        if (row != null)
                        {
                            row["Status"] = 0;
                            row["isCheckKichHoat"] = 0;
                        }
                    }
                    string _Module = "ChiaChuyenVaDuyetSX";
                    string _Action = "Hủy duyệt Lệnh Win";
                    string _Contents = _maDH + "@" + _maLenhSX;
                    string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                    string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
                    ReloadData();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    string Title = "Lệnh sản xuất đã bị hủy ban hành";
                    DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                    if (rowLenhFocused == null) return;
                    string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
                    string SendTo = "PKH";
                    SendNotify(Title, Detail, SendTo);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void SelectAllIsChon(bool isChecked)
        {
            var dt = grcLenhChia.DataSource as DataTable;
            if (dt == null) return;
            grvLenhChia.FocusedRowChanged -= grvLenhChia_FocusedRowChanged;

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row.Table.Columns.Contains("IsChon"))
                    {
                        row["IsChon"] = isChecked;
                    }
                }
                grcLenhChia.RefreshDataSource();
                grvLenhChia.RefreshData();
                grvLenhChia.LayoutChanged();
            }
            finally
            {
                grvLenhChia.FocusedRowChanged += grvLenhChia_FocusedRowChanged;
            }
        }
        private void grvLenhChia_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;
            if (e.HitInfo.Column != null && e.HitInfo.Column.FieldName == "IsChon")
            {
                e.Menu.Items.Clear();

                var menuChonTatCa = new DevExpress.Utils.Menu.DXMenuItem("Chọn tất cả");
                menuChonTatCa.Click += (s, args) => SelectAllIsChon(true);

                var menuBoTatCa = new DevExpress.Utils.Menu.DXMenuItem("Bỏ chọn tất cả");
                menuBoTatCa.Click += (s, args) => SelectAllIsChon(false);

                e.Menu.Items.Add(menuChonTatCa);
                e.Menu.Items.Add(menuBoTatCa);
                return;
            }
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                DataRow row = view.GetFocusedDataRow();
                if (row == null) return;

                DevExpress.Utils.Menu.DXMenuItem menuItemKetThuc = new DevExpress.Utils.Menu.DXMenuItem("Kết thúc", KetThucClick);
                menuItemKetThuc.Tag = row["MaLenh"]?.ToString();
                menuItemKetThuc.ImageOptions.SvgImage = DevExpress.Utils.Svg.SvgImage.FromResources("NtbSoft.ERP.Win.Resources.close.svg", typeof(frmCanDoiDHChiaSXChiTiet).Assembly);

                e.Menu.Items.Add(menuItemKetThuc);
                DevExpress.Utils.Menu.DXMenuItem menuItemKichHoatLai = new DevExpress.Utils.Menu.DXMenuItem("Kích hoạt lại", KichHoatLaiLenh);
                menuItemKichHoatLai.Tag = row["MaLenh"]?.ToString();
                menuItemKichHoatLai.ImageOptions.SvgImage = DevExpress.Utils.Svg.SvgImage.FromResources("NtbSoft.ERP.Win.Resources.close.svg", typeof(frmCanDoiDHChiaSXChiTiet).Assembly);

                e.Menu.Items.Add(menuItemKichHoatLai);
            }
        }
        private void KetThucClick(object sender, EventArgs e)
        {
            try
            {
                var menuItem = sender as DevExpress.Utils.Menu.DXMenuItem;
                if (menuItem == null || menuItem.Tag == null) return;
                string maLenh = menuItem.Tag.ToString();
                var confirm = XtraMessageBox.Show(
                    $"Bạn có chắc muốn kết thúc lệnh {maLenh}?\n",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm == DialogResult.No) return;

                List<CanDoiDonViSanXuatSaveEntity> listKetThuc = new List<CanDoiDonViSanXuatSaveEntity>
                {
                    new CanDoiDonViSanXuatSaveEntity { MaLenh = maLenh }
                };

                string url = $"{URL}ERPDonHangTong/Post?action=PostStatusKetThuc&type=@TypeTable";
                var msResult = Task.Run(async () => await _clientExtension.PostAsync(url, listKetThuc)).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    ReloadData();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    string Title = "Lệnh sản xuất đã kết thúc";
                    DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                    if (rowLenhFocused == null) return;
                    string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
                    string SendTo = "PKH";
                    SendNotify(Title, Detail, SendTo);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void KichHoatLaiLenh(object sender, EventArgs e)
        {
            try
            {
                var menuItem = sender as DevExpress.Utils.Menu.DXMenuItem;
                if (menuItem == null || menuItem.Tag == null) return;
                string maLenh = menuItem.Tag.ToString();
                var confirm = XtraMessageBox.Show(
                    $"Bạn có chắc muốn kích hoạt lại lệnh {maLenh}?\n",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (confirm == DialogResult.No) return;

                List<CanDoiDonViSanXuatSaveEntity> listKetThuc = new List<CanDoiDonViSanXuatSaveEntity>
                {
                    new CanDoiDonViSanXuatSaveEntity { MaLenh = maLenh }
                };

                string url = $"{URL}ERPDonHangTong/Post?action=PostStatusKichHoatLai&type=@TypeTable";
                var msResult = Task.Run(async () => await _clientExtension.PostAsync(url, listKetThuc)).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    string _Module = "ChiaChuyenVaDuyetSX";
                    string _Action = "Kích hoạt lại lệnh";
                    string _Contents = _maDH + "@" + _maLenhSX;
                    string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                    string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
                    ReloadData();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    string Title = "Lệnh sản xuất đã được kích hoạt lại";
                    DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                    if (rowLenhFocused == null) return;
                    string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
                    string SendTo = "PKH";
                    SendNotify(Title, Detail, SendTo);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void btnUpdateBOM_Click(object sender, EventArgs e)
        {
            DataRow focusedRow = grvLenhChia.GetFocusedDataRow();
            if (focusedRow == null) return;

            string makh = focusedRow["MaKH"]?.ToString() ?? "";
            string mahang = focusedRow["MaHang"]?.ToString() ?? "";
            string userName = GlobleData.UserName;
            string madot = focusedRow["MaDot"]?.ToString() ?? "";
            string maLenhSX = focusedRow["MaLenhSanXuat"]?.ToString() ?? "";
            string magop = focusedRow["MaGop"]?.ToString() ?? "";
            bool hasDataInSDKH = false;
            bool hasVTMissingInBOM = false;
            //bool hasDataInDMNL = false;
            //string urlCheckDMNL = $"{URL}KhoiTaoBOMV1/Get?action=CheckBOMDMNL&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
            //string jsonCheckDMNL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheckDMNL); }).Result;

            //if (!string.IsNullOrEmpty(jsonCheckDMNL))
            //{
            //    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCheckDMNL);
            //    if (tbl != null && tbl.Rows.Count > 0)
            //    {
            //        int hasDataDM = 0;
            //        int.TryParse(tbl.Rows[0]["HasDataDMNL"]?.ToString(), out hasDataDM);
            //        if (hasDataDM == 1)
            //        {
            //            hasDataInDMNL = true;
            //        }
            //        else
            //        {
            //            hasDataInDMNL = false;
            //        }
            //    }
            //}
            string urlCheckTNC = $"{URL}KhoiTaoBOMV1/Get?action=CheckBOMTNC&para1={makh}&para2={mahang}&para3={magop}&para5={maLenhSX}&para6={userName}&para7={madot}";
            string jsonCheckTNC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheckTNC); }).Result;

            if (!string.IsNullOrEmpty(jsonCheckTNC))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCheckTNC);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int hasDataTNC = 0;
                    int.TryParse(tbl.Rows[0]["HasDataTNC"]?.ToString(), out hasDataTNC);
                    if (hasDataTNC == 1)
                    {
                        hasDataInSDKH = true;
                    }
                    else
                    {
                        hasDataInSDKH = false;
                    }
                }
            }
            DataTable tblCD = grcCanDoi.DataSource as DataTable;
            if (tblCD != null && tblCD.Rows.Count > 0)
            {
                hasVTMissingInBOM = tblCD.AsEnumerable()
                    .Any(r =>
                        r.IsNull("Dot") &&
                        r.IsNull("IsSuaNPL") &&
                        r.IsNull("STT")
                    );
            }
            var meg = "";
            var mesgDel = "Có vật tư đã được xóa trong BOM mà còn tồn tại trong Cấp phát NPL lệnh sản xuất.\n";
            var mesgTNC = "Và đã tác nghiệp cắt.\n";
            var mesgWarning = "Nếu cập nhật sẽ xóa đi những vật tư này!\n";
            var mesgAdd = "Có sự thay đổi BOM\n";
            if (hasVTMissingInBOM)
            {
                meg = meg + mesgDel + mesgWarning;
            }
            else if (hasVTMissingInBOM && hasDataInSDKH)
            {
                meg = meg + mesgDel + mesgTNC + mesgWarning;
            }
            else if (!hasVTMissingInBOM && !hasDataInSDKH)
            {
                meg = meg + mesgAdd;
            }
            var confirm = XtraMessageBox.Show(meg +
                    "Bạn có muốn cập nhật BOM cho lệnh này không?",
                    "Xác nhận cập nhật BOM",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

            if (confirm != DialogResult.Yes)
                return;
            int isSuaNPL = 0;

            if (grcCanDoi.DataSource is DataTable tblCanDoi && tblCanDoi.Rows.Count > 0 && tblCanDoi.Columns.Contains("IsSuaNPL"))
            {
                var rowHasValue = tblCanDoi.AsEnumerable()
                    .FirstOrDefault(r =>
                        r["IsSuaNPL"] != DBNull.Value &&
                        !string.IsNullOrWhiteSpace(r["IsSuaNPL"].ToString())
                    );

                if (rowHasValue != null)
                {
                    int.TryParse(rowHasValue["IsSuaNPL"].ToString(), out isSuaNPL);
                }
            }

            string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=UpdateVT&para1={makh}&para2={mahang}&para3={magop}&para5={maLenhSX}&para6={userName}&para7={madot}&para10={isSuaNPL}";
            string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
            int checkDuyet = 0;
            if (focusedRow.Table.Columns.Contains("CheckDuyet"))
                int.TryParse(focusedRow["CheckDuyet"]?.ToString(), out checkDuyet);

            if (checkDuyet == 1)
            {
                string url = $"{URL}ERPDonHangTong/Get?action=BatDuyetLai&para={maLenhSX}&para2={magop}&para3=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            }
            clsWaitForm.ShowSuccessForm(this, 2000);
            ReloadData();
            LoadCanDoi();
            GetChiTietCanDoi();
        }

        private void btnViewChangedBOM_Click(object sender, EventArgs e)
        {
            DataRow focusedRow = grvLenhChia.GetFocusedDataRow();
            string makh = focusedRow["MaKH"]?.ToString() ?? "";
            string mahang = focusedRow["MaHang"]?.ToString() ?? "";
            string madot = focusedRow["MaDot"]?.ToString() ?? "";
            string maGop = focusedRow["MaGop"]?.ToString() ?? "";
            using (var frm = new frmViewChangedBOMV1(_maDH, _maLenhSX, makh, mahang, madot, maGop))
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            }
        }
        private void btnTimLenh_Click(object sender, EventArgs e)
        {
            Point location = btnTimLenh.PointToScreen(new Point(0, btnTimLenh.Height + 2));
            using (var frmSearch = new frmSearchChiaChuyen(location.X, location.Y))
            {
                if (frmSearch.ShowDialog(this) != DialogResult.OK)
                    return;
                DataTable dtFromSearch = frmSearch.ResultTable;

                if (dtFromSearch == null || dtFromSearch.Rows.Count == 0)
                    return;

                _searchMaHang = frmSearch.maHang;
                _searchMaLenh = frmSearch.maLenh;
                _searchChungLoai = frmSearch.chungLoai;
                _searchKhachHang = frmSearch.khachHang;
                _searchDot = frmSearch.dot;
                _searchPO = frmSearch.po;
                _isFromSearch = true;

                DataTable dtGop = GopVaTongLenhChiaSX(dtFromSearch);

                if (dtGop == null || dtGop.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu sau khi gộp lệnh.", "Thông báo");
                    return;
                }
                if (!dtGop.Columns.Contains("IsChon"))
                {
                    dtGop.Columns.Add("IsChon", typeof(bool));
                    foreach (DataRow r in dtGop.Rows)
                        r["IsChon"] = false;
                }
                grcLenhChia.DataSource = dtGop;
                _isFromSearch = true;
                grvLenhChia.FocusedRowHandle = 0;
                grvLenhChia.SelectRow(0);
                grvLenhChia.MakeRowVisible(0);

                grvLenhChia_FocusedRowChanged(
                    grvLenhChia,
                    new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(-1, 0)
                );
                _lastSourceData = dtGop.Copy();
                //GetChiTietDH();
                //GetChiTietCanDoi();
                //GetPO();
                //GetMau();
                //GetLine();
                //LoadCanDoi();
            }
        }

        private void grcChiTietCanDoi_Click(object sender, EventArgs e)
        {

        }

        private void frmCanDoiDHChiaSXChiTiet_Load(object sender, EventArgs e)
        {
            InitBlinkTimer();
        }

        private async void btnGetAllLenhSX_Click(object sender, EventArgs e)
        {
            //int pageIndex = 1,pageSize = 50;
            //string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para=all&PageIndex ={pageIndex}&PageSize={pageSize}";
            IsDuyetKichHoatlenh = false;
            _isFromAll = true;
            _isFromSearch = false;
            string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para=all";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcLenhChia.DataSource = null;
                return;
            }
            DataTable tblMaDHGop = GopVaTongLenhChiaSX(tbl);
            if (!tblMaDHGop.Columns.Contains("IsChon"))
            {
                tblMaDHGop.Columns.Add("IsChon", typeof(bool));
                foreach (DataRow r in tblMaDHGop.Rows)
                    r["IsChon"] = false;
            }
            grcLenhChia.DataSource = tblMaDHGop;
            _maDH = tblMaDHGop.Rows[0]["MaDH"].ToString();
            _maLenhSX = tblMaDHGop.Rows[0]["MaLenhSanXuat"].ToString();
        }

        private void grvCanDoi_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || view.FocusedColumn == null) return;

            string fieldName = view.FocusedColumn.FieldName;

            if (fieldName != "SoLuong" && fieldName != "DinhMuc" && fieldName != "CapPhat") return;

            if (!_isGiaCong)
            {
                e.Cancel = true;
            }
        }

        //private void CalAllCapPhat()
        //{
        //    var tblSource = grcCanDoi.DataSource as DataTable;
        //    if (tblSource == null || tblSource.Rows.Count == 0) return;

        //    foreach (DataRow row in tblSource.Rows)
        //    {
        //        CalCapPhat(row);
        //    }
        //    grvCanDoi.RefreshData();
        //}
        private void btnSavePOMua_Click(object sender, EventArgs e)
        {
            //CalAllCapPhat();
            //bool ischanged = IsChangedSL();
            //if (ischanged)
            //{
            //    DongBoSoLuong();
            //    CheckChangedSoLuong();
            //}
            IsDuyetKichHoatlenh = false;
            var tblSource = grcCanDoi.DataSource as DataTable;
            if (tblSource != null && tblSource.Rows.Count > 0)
            {
                bool hasInvalid = tblSource.AsEnumerable().Any(row =>
                {
                    string npl = row["NPL"]?.ToString() ?? "";
                    if (npl != "Nguyên liệu") return false;

                    decimal dinhMuc = 0, capPhat = 0;
                    decimal.TryParse(row["DinhMuc"]?.ToString(), out dinhMuc);
                    decimal.TryParse(row["CapPhat"]?.ToString(), out capPhat);
                    return dinhMuc == 0 && capPhat == 0;
                });

                if (hasInvalid)
                {
                    var confirm = XtraMessageBox.Show(
                       $"Có dòng nguyên liệu có định mức và cấp phát bằng 0. Bạn có chắc chắn muốn xác nhận?",
                       "Xác nhận cấp phát",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question
                   );

                    if (confirm != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }

            if (!_isGiaCong)
                SaveCanDoi("PostPOMua");
            else
                SaveCanDoi("PostGC");
        }
        private void btnXacNhanCanDoi_Click(object sender, EventArgs e)
        {
            if (GlobleData.UserName == "admin" || GlobleData.UserName == "QLDH_01")
            {
                SaveCanDoi("XacNhanCanDoi");
            }
            else
            {

                XtraMessageBox.Show("Bạn không có quyền thực hiện thao tác này!!!",
                   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }
        private void DongBoSoLuong()
        {
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return;

            string malenh = dr["MaLenh"]?.ToString() ?? "";
            string malenhsx = dr["MaLenhSanXuat"]?.ToString() ?? "";
            string magop = dr["MaGop"]?.ToString() ?? "";
            string makh = dr["MaKH"]?.ToString() ?? "";
            string mahang = dr["MaHang"]?.ToString() ?? "";
            string url = $"{URL}ERPDonHangTong/PostUpSLCP?para={malenhsx}&para2={malenh}&para3={magop}&para4={makh}&para5={mahang}&para6=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //if (json.ToUpper() == "TRUE")
            //{
            //    //clsWaitForm.ShowSuccessForm(this, 2000);
            //    //LoadCanDoi();
            //}
        }
        private void UpdateIsXNSD()
        {
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return;
            string malenh = dr["MaLenh"].ToString();
            string malenhsx = dr["MaLenhSanXuat"].ToString();
            string magop = dr["MaGop"].ToString();
            string url = $"{URL}ERPDonHangTong/Get?action=UpdateIsXNSD&para={malenhsx}&para2={malenh}&para3={magop}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //if (json.ToUpper() == "TRUE")
            //{


            //}
            string Title = "Số lượng đơn hàng đã điều chỉnh";
            DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
            if (rowLenhFocused == null) return;
            DataTable tblChiTietLenh = grcChiTietCanDoi.DataSource as DataTable;
            string PO = "";
            if (tblChiTietLenh != null && tblChiTietLenh?.Rows?.Count > 0)
            {
                PO = string.Join(";", tblChiTietLenh.AsEnumerable().Select(x => x["PO"]?.ToString()));
            }

            string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nPO:{PO}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
            string SendTo = "QLSX";
            SendNotify(Title, Detail, SendTo, "ALL", -1, 1);
            SendNotify(Title, Detail, "PKH", "ALL", 1, 0);

        }
        private void SaveCanDoi(string action)
        {
            DataRow drLenh = grvLenhChia.GetFocusedDataRow();
            if (drLenh == null) return;
            string maGop = drLenh["MaGop"]?.ToString() ?? "";

            List<CanDoiDonViSanXuatDuyetSaveEntity> listSave = new List<CanDoiDonViSanXuatDuyetSaveEntity>();
            var tblSource = grcCanDoi.DataSource as DataTable;
            if (tblSource == null || tblSource.Rows.Count == 0) return;
            foreach (DataRow row in tblSource.Rows)
            {
                bool isEditedByUser = false;
                if (row.Table.Columns.Contains("IsEdited") && row["IsEdited"] != DBNull.Value)
                {
                    bool.TryParse(row["IsEdited"].ToString(), out isEditedByUser);
                }
                string poid = row["POMua"].ToString();
                int IdRow = Convert.ToInt32(row["ID"]);
                decimal dinhmuchaohut = 0;
                decimal.TryParse(Convert.ToString(row["DinhMucHaoHut"]), out dinhmuchaohut);
                decimal dinhmuc = 0;
                decimal.TryParse(Convert.ToString(row["DinhMuc"]), out dinhmuc);
                decimal soluong = 0;
                decimal.TryParse(row["SoLuong"]?.ToString(), out soluong);
                bool isCapPhatEdited = false;
                if (row.Table.Columns.Contains("IsCapPhatEdited") && row["IsCapPhatEdited"] != DBNull.Value)
                    bool.TryParse(row["IsCapPhatEdited"].ToString(), out isCapPhatEdited);

                decimal capphat = 0;
                decimal.TryParse(row["CapPhat"]?.ToString(), out capphat);
                CanDoiDonViSanXuatDuyetSaveEntity itemSave = new CanDoiDonViSanXuatDuyetSaveEntity();
                itemSave.POID = poid;
                itemSave.ID = IdRow;
                itemSave.SoLuong = dinhmuchaohut;
                itemSave.TrangThai = dinhmuc;
                itemSave.Line = row["GhiChu"].ToString();
                itemSave.DotSX = soluong;
                itemSave.STTLenh = isCapPhatEdited ? capphat : 0;
                itemSave.GhiChu = GlobleData.UserName;
                listSave.Add(itemSave);
            }

            if (listSave == null || listSave.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để lưu.");
                return;
            }
            string urltt = $"{URL}ERPDonHangTong/Get?action=UpdateStatusSD&para={_maLenhSX}&para2={maGop}";
            string jsontt = Task.Run(async () => await _clientExtension.GetAsnyc(urltt)).Result;

            string url = string.Format("{0}", URL + $"ERPDonHangTong/PostV1?action={action}&type=@TypeTable&para={_maLenhSX ?? ""}&para2={maGop}");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                string _Module = "ChiaChuyenVaDuyetSX";
                string _Action = "Xác nhận cấp phát NPL";
                string _Contents = _maDH + "@" + _maLenhSX;
                string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;

                //ReloadData();
                //clsWaitForm.ShowSuccessForm(this, 2000);
                //string Title = "Lệnh sản xuất đã xác nhận cấp phát";
                //DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                //if (rowLenhFocused == null) return;
                //string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
                //string SendTo = "PKH";
                //if (!IsDuyetKichHoatlenh)
                //{
                //    //SendNotify(Title, Detail, SendTo, "ALL", -1, 1);
                //    SendNotify(Title, Detail, "PKH", "ALL", 1, 0);
                //}
                if (!IsDuyetKichHoatlenh)
                {
                    ReloadData();
                    clsWaitForm.ShowSuccessForm(this, 2000);

                    string Title = "Lệnh sản xuất đã xác nhận cấp phát";
                    DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                    if (rowLenhFocused == null) return;
                    string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\n" +
                                    $"Mã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\n" +
                                    $"Lệnh: {rowLenhFocused["MaLenh"]?.ToString()}\n" +
                                    $"Số lượng: {rowLenhFocused["SoLuong"]?.ToString()}";
                    SendNotify(Title, Detail, "PKH", "ALL", 1, 0);
                }

            }
        }
        private void grvLenhChia_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

        }
        private void btnXacNhanCanDoi1_Click(object sender, EventArgs e)
        {
            if (GlobleData.UserName == "admin" || GlobleData.UserName == "QLDH_01")
            {
                SaveCanDoiV1("HuyXacNhanCD");
            }
            else
            {

                XtraMessageBox.Show("Bạn không có quyền thực hiện thao tác này!!!",
                   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void SaveCanDoiV1(string action)
        {
            List<CanDoiDonViSanXuatDuyetSaveEntity> listSave = new List<CanDoiDonViSanXuatDuyetSaveEntity>();
            var tblSource = grcCanDoi.DataSource as DataTable;
            if (tblSource == null) return;
            foreach (DataRow row in tblSource.Rows)
            {
                string poid = row["POMua"].ToString();
                int IdRow = Convert.ToInt32(row["ID"]);
                CanDoiDonViSanXuatDuyetSaveEntity itemSave = new CanDoiDonViSanXuatDuyetSaveEntity();
                itemSave.POID = poid;
                itemSave.ID = IdRow;
                listSave.Add(itemSave);
            }

            if (listSave == null || listSave.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để lưu.");
                return;
            }
            string url = string.Format("{0}", URL + $"ERPDonHangTong/PostV2?action={action}&type=@TypeTable");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            if (action == "HuyXacNhanCD")
            {
                LoadCanDoi();
            }
        }
        #region Send To Notify
        private void SendNotify(string Title, string Detail, string SendTo, string BoPhan = "ALL", int Status = -1, int IsQLSX = 0, string FrmName = "frmDonHangTong")
        {

            DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
            if (rowLenhFocused == null) return;

            try
            {
                string url = $"{URL}SendToNotification/PushNotificationFrm?" +
                $"UserIDTao={HttpUtility.UrlEncode(GlobleData.UserName)}&" +
                $"FrmName={HttpUtility.UrlEncode(FrmName)}&" +
                $"Title={HttpUtility.UrlEncode(Title)}&" +
                $"Detail={HttpUtility.UrlEncode(Detail)}&" +
                $"SendTo={HttpUtility.UrlEncode(SendTo)}&" +
                $"BoPhan={HttpUtility.UrlEncode(BoPhan)}&" +
                $"Status={Status}&IsQLSX={IsQLSX}&MaPhieu={rowLenhFocused["MaLenh"]}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;

            }
            catch (Exception e)
            {

            }

        }
        #endregion

        private void grvCanDoi_RowCellClick(object sender, RowCellClickEventArgs e)
        {

            DataRow rowFocusedCapPhat = grvCanDoi.GetFocusedDataRow() as DataRow;
            DataRow rowFocusedMaLenh = grvLenhChia.GetFocusedDataRow() as DataRow;
            DataTable tblLenhPO = grcChiTietCanDoi.DataSource as DataTable;
            if (tblLenhPO != null && tblLenhPO?.Rows?.Count == 0) return;
            if (rowFocusedCapPhat == null || rowFocusedMaLenh == null) return;

            #region My
            if (e.Column.FieldName == "POMua")
            {
                DataRow row = grvCanDoi.GetDataRow(e.RowHandle);
                if (row == null) return;

                DataRow dr = grvLenhChia.GetFocusedDataRow();
                if (dr == null) return;

                DataRow drLenh = grvLenhChia.GetFocusedDataRow();
                if (drLenh == null) return;

                string _PO = (searchLookUpEditPO.EditValue as string) ?? "";
                string _Mau = (searchLookUpEditMau.EditValue as string) ?? "";
                string _Line = (searchLookUpEditLine.EditValue as string) ?? "";

                _TenLenhSX = $"Đơn hàng: {drLenh["MaDH"] ?? ""} - Mã lệnh: {drLenh["Malenh"] ?? ""} - Mã hàng: {drLenh["MaHang"] ?? ""}  - KH: {drLenh["TenKH"] ?? ""} - Chủng loại: {drLenh["TenCL"] ?? ""} - Số lượng: {drLenh["SoLuong"] ?? ""}";

                _MaGop = dr["MaGop"].ToString();
                _MaDH_Send = _maDH;
                _MaLenhSX_Send = _maLenhSX;

                _MaVTID = row["MaVTID"]?.ToString() ?? "";
                _MauVTID = row["MauID"]?.ToString() ?? "";
                _KhoVaiID = row["KhoVaiID"]?.ToString() ?? "";
                _MaCLVT = row["MaNhomVT"]?.ToString() ?? "";

                object cellValue = e.CellValue;
                frmChiTietChonItemCode frm = new frmChiTietChonItemCode(_MaVTID, _MauVTID, _KhoVaiID, _MaCLVT, _MaDH_Send, _MaLenhSX_Send, _MaGop, _TenLenhSX, _PO, _Mau, _Line);
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
                LoadCanDoi();
            }
            #endregion

            if (string.IsNullOrEmpty(rowFocusedCapPhat["Code_TNC"]?.ToString())) return;
            if (grvCanDoi.FocusedColumn == colTNC)
            {

                frmTNC_QLSX_View frm = new frmTNC_QLSX_View(rowFocusedMaLenh, rowFocusedCapPhat, tblLenhPO);
                frm.ShowDialog();
            }
        }




        private void griview_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
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


        private static readonly Color BlinkRedColor = Color.FromArgb(255, 233, 238);

        private void repoSaveGC_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            this.ActiveControl = btnTimLenh;
            DataRow drLenh = grvLenhChia.GetFocusedDataRow();
            if (drLenh == null) return;
            string maGop = drLenh["MaGop"]?.ToString() ?? "";

            List<CanDoiDonViSanXuatDuyetSaveEntity> listSave = new List<CanDoiDonViSanXuatDuyetSaveEntity>();
            var tblSource = grcCanDoi.DataSource as DataTable;
            if (tblSource == null || tblSource.Rows.Count == 0) return;
            foreach (DataRow row in tblSource.Rows)
            {
                bool isEditedByUser = false;
                if (row.Table.Columns.Contains("IsEdited") && row["IsEdited"] != DBNull.Value)
                {
                    bool.TryParse(row["IsEdited"].ToString(), out isEditedByUser);
                }
                int IdRow = Convert.ToInt32(row["ID"]);
                CanDoiDonViSanXuatDuyetSaveEntity itemSave = new CanDoiDonViSanXuatDuyetSaveEntity();
                itemSave.ID = IdRow;
                itemSave.Line = row["GhiChu"].ToString();
                itemSave.GhiChu = GlobleData.UserName;
                listSave.Add(itemSave);
            }

            if (listSave == null || listSave.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để lưu.");
                return;
            }
            string url = string.Format("{0}", URL + $"ERPDonHangTong/PostV1?action=UpdateGhiChu&type=@TypeTable&para={_maLenhSX ?? ""}&para2={maGop}");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                string _Module = "ChiaChuyenVaDuyetSX";
                string _Action = "Update Ghi Chú";
                string _Contents = _maDH + "@" + _maLenhSX;
                string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;

                ReloadData();
                clsWaitForm.ShowSuccessForm(this, 2000);
                //string Title = "Lệnh sản xuất đã xác nhận cấp phát";
                //DataRow rowLenhFocused = grvLenhChia.GetFocusedDataRow();
                //if (rowLenhFocused == null) return;
                //string Detail = $"Đơn Hàng: {rowLenhFocused["MaDH"]?.ToString()}\nMã Hàng: {rowLenhFocused["TenHang"]?.ToString()}\nLệnh: {rowLenhFocused["MaLenh"]?.ToString()}\nSố lượng:{rowLenhFocused["SoLuong"]?.ToString()}";
                //string SendTo = "PKH";
                //if (!IsDuyetKichHoatlenh)
                //{
                //    //SendNotify(Title, Detail, SendTo, "ALL", -1, 1);
                //    SendNotify(Title, Detail, "PKH", "ALL", 1, 0);
                //}

            }
        }

        private void btnViewChangeDot_Click(object sender, EventArgs e)
        {
            DataRow focusedRow = grvLenhChia.GetFocusedDataRow();
            if (focusedRow == null) return;

            string makh = focusedRow["MaKH"]?.ToString() ?? "";
            string mahang = focusedRow["MaHang"]?.ToString() ?? "";
            string madot = focusedRow["MaDot"]?.ToString() ?? "";
            string maGop = focusedRow["MaGop"]?.ToString() ?? "";

            string preBatch = string.Empty;
            string urlPreBatch = $"{URL}ERPDonHangTong/Get?action=GetBatch&para={maGop}&para2={_maLenhSX}&para3={madot}";
            string jsonPreBatch = Task.Run(async () => await _clientExtension.GetAsnyc(urlPreBatch)).Result;

            if (!string.IsNullOrEmpty(jsonPreBatch) && jsonPreBatch != "[]")
            {
                DataTable tblPreBatch = JsonConvert.DeserializeObject<DataTable>(jsonPreBatch);
                if (tblPreBatch != null && tblPreBatch.Rows.Count > 0)
                    preBatch = tblPreBatch.Rows[0]["PreBatch"]?.ToString() ?? "";
            }

            using (var frm = new frmViewChangedBatch(_maDH, _maLenhSX, makh, mahang, madot, maGop, preBatch))
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            }
        }

        private static readonly Color BlinkYellowColor = Color.FromArgb(255, 248, 212);

        private System.Windows.Forms.Timer _blinkTimer;
        private bool _blinkState = false;
        private void frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _blinkTimer?.Stop();
            _blinkTimer?.Dispose();
        }

        private void InitBlinkTimer()
        {
            _blinkTimer = new System.Windows.Forms.Timer();
            _blinkTimer.Interval = 600;
            _blinkTimer.Tick += (s, e) =>
            {
                _blinkState = !_blinkState;
                try
                {
                    //if (!grvCanDoi.IsDisposing)
                    //    grvCanDoi.LayoutChanged(); // <-- dùng cái này thay InvalidateRows
                    if (!grvCanDoi.IsDisposing && !grvCanDoi.IsEditing)
                        grvCanDoi.LayoutChanged();
                }
                catch { }
            };
            _blinkTimer.Start();
        }
        private void grvCanDoi_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (view == null) return;

                DataRow row = null;
                try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
                if (row == null) return;
                if (e.RowHandle < 0) return; // bỏ qua header/footer rows

                if (e.RowHandle == grvCanDoi.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.Color.RoyalBlue;
                    e.Appearance.ForeColor = System.Drawing.Color.White;
                    return;
                }
                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font ?? gridView1.Appearance.Row.Font, System.Drawing.FontStyle.Bold);

                int.TryParse(row["IsXN_SD"]?.ToString(), out int IsXN_SD);

                if (IsXN_SD == 0 || IsXN_SD == 2)
                {
                    //string npl = row["NPL"]?.ToString() ?? "";
                    //if (npl == "Nguyên liệu")
                    //{
                    //    decimal dinhMuc = 0, capPhat = 0;
                    //    decimal.TryParse(row["DinhMuc"]?.ToString(), out dinhMuc);
                    //    decimal.TryParse(row["CapPhat"]?.ToString(), out capPhat);

                    //    if (dinhMuc == 0 && capPhat == 0)
                    //    {
                    //        e.Appearance.BackColor = Color.LightPink;
                    //        e.Appearance.ForeColor = Color.Black;
                    //        e.Appearance.Options.UseBackColor = true;
                    //        e.Appearance.Options.UseForeColor = true;
                    //        return;
                    //    }
                    //}
                }
                else
                {
                    switch (IsXN_SD)
                    {
                        case 1:
                            e.Appearance.BackColor = _blinkState ? BlinkRedColor : System.Drawing.Color.White;
                            e.Appearance.ForeColor = System.Drawing.Color.Black;
                            break;
                        case 3:
                            e.Appearance.BackColor = _blinkState ? BlinkYellowColor : System.Drawing.Color.White;
                            e.Appearance.ForeColor = System.Drawing.Color.Black;
                            break;
                    }
                }


            }
            catch (Exception ex)
            {

            }
        }
        private void GetIsXNSD()
        {
            DataRow dr = grvLenhChia.GetFocusedDataRow();
            if (dr == null) return;
            string malenh = dr["MaLenh"].ToString();
            string malenhsx = dr["MaLenhSanXuat"].ToString();
            string magop = dr["MaGop"].ToString();
            string url = $"{URL}ERPDonHangTong/Get?action=GetIsXNSD&para={malenhsx}&para2={magop}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (string.IsNullOrEmpty(json)) return;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;

            var tblCanDoi = grcCanDoi.DataSource as DataTable;
            bool hasCanDoiData = tblCanDoi != null && tblCanDoi.Rows.Count > 0;
            bool isCanDoiXacNhan = hasCanDoiData && tblCanDoi.AsEnumerable().All(row => row["Status"]?.ToString() == "Đã xác nhận");
            bool allXacNhanSD = tbl.AsEnumerable().Any(row => {
                int val = 0;
                int.TryParse(row["IsXN_SD"]?.ToString(), out val);
                return val == 4;
            });
            layoutControlItem33.Visibility = (hasCanDoiData && isCanDoiXacNhan && allXacNhanSD) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }
        private void CheckHasChangedBatch()
        {
            DataRow focusedRow = grvLenhChia.GetFocusedDataRow();
            var tblCanDoi = grcCanDoi.DataSource as DataTable;
            bool hasCanDoiData = tblCanDoi != null && tblCanDoi.Rows.Count > 0;
            if (!hasCanDoiData || focusedRow == null)
            {
                layoutControlItem24.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                return;
            }

            string maGop = focusedRow["MaGop"]?.ToString() ?? "";
            string madot = focusedRow["MaDot"]?.ToString() ?? "";

            string url = $"{URL}ERPDonHangTong/Get?action=GetBatch&para={maGop}&para2={_maLenhSX}&para3={madot}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            bool hasData = false;
            if (!string.IsNullOrEmpty(json) && json != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                hasData = tbl != null && tbl.Rows.Count > 0;
            }
            layoutControlItem24.Visibility = (hasData && hasCanDoiData) ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }
    }
}