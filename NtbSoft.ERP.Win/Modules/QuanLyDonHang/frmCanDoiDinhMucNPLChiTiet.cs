using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDinhMucNPLChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _mahang = string.Empty, _tenhang = string.Empty, _malenh = string.Empty, _dot = string.Empty,
        _madvsx = string.Empty, _malenhsanxuat = string.Empty, _tenlenh = string.Empty, _poid = string.Empty, _manpl = string.Empty, _magop = string.Empty, _sttlenh = string.Empty,_madhgop=string.Empty, _ghichu = string.Empty;
        private int _valueSum = 0, _valueConLai = 0, _totalSum = 0, totalAmountcheck = 0, _SLTong = 0;
        List<CanDoiNPLEntity> lstCanDoiDinhMucNPL;
        List<EditCanDoiLenhSanXuatEntity> ListCanDoiSX;
        List<CanDoiNPLSaveEntity> Listcandoinpl;
        DataTable _dttemp;
        DataTable tbldvsx;
        DataTable tblCanDoiNPL;
        int _npl = 0;
        bool indicatorIcon = true;
        KeyDownControlHandler keyDownControlHandler;
        int _isKeoVe = 0;
        int _isDvsx = 0;
        //bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        string _nguoitao = string.Empty;
        private DataTable _tblCanDoiLenhSX_BeforeEdit;

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;
        public frmCanDoiDinhMucNPLChiTiet(string madh, string mahang, string tenhang, string malenh, string dot, string madvsx, string malenhsanxuat, string poid, int isKeoVe, string nguoitao, string magop, string tenlenh, string sttlenh,string madhgop="", string ghichu="")
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._madh = madh;
            this._mahang = mahang;
            this._tenhang = tenhang;
            this._malenh = malenh;
            this._dot = dot;
            this._madvsx = madvsx;
            this._malenhsanxuat = malenhsanxuat;
            this._poid = poid;
            this._tenlenh = tenlenh;
            this._sttlenh = sttlenh;
            this._ghichu = ghichu;
            tbldvsx = new DataTable();
            tblCanDoiNPL = new DataTable();
            ListCanDoiSX = new List<EditCanDoiLenhSanXuatEntity>();
            Listcandoinpl = new List<CanDoiNPLSaveEntity>();
            _isKeoVe = isKeoVe;
            this._nguoitao = nguoitao;
            this._magop = magop;
            this._madhgop = madhgop;

            groupLevelColors = new Dictionary<int, Color>
          {
            { -1, ColorTranslator.FromHtml("#2A5D9F") },
            { 0, ColorTranslator.FromHtml("#2A5D9F") },
            { 1, ColorTranslator.FromHtml("#A53E25") },
            { 2, ColorTranslator.FromHtml("#2E7D5B") }
          };
            groupLevelColorBackground = new Dictionary<int, Color>
            {
                { -1, ColorTranslator.FromHtml("#DDEBFB") },
                { 0, ColorTranslator.FromHtml("#DDEBFB") },
                { 1, ColorTranslator.FromHtml("#FFE2D3") },
                { 2, ColorTranslator.FromHtml("#D7F5E8") }
            };

        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            splitContainerControl1.SplitterPosition = ((int)(splitContainerControl1.Width * 3.35 / 4));
        }

        //private void CheckPerminsion()
        //{
        //    SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        //    string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
        //    List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
        //    SystemUserModuleEntity obj = (from m in List
        //                                  where m.FormShow == this.Name
        //                                  select m).FirstOrDefault();
        //    if (obj == null) return;
        //    _allowAdd = obj.AllowAdd;
        //    _allowEdit = obj.AllowEdit;
        //    _allowDelete = obj.AllowDelete;

        //    if (!_allowAdd)
        //    {
        //        //btChiaSX.Enabled = false;
        //    }
        //    if (_allowEdit)
        //    {
        //        btLuu.Enabled = true;
        //    }
        //    if (_allowDelete)
        //        btDelete.Enabled = true;
        //}

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, Them, true, ActionType.Add);
            AddActionControl(_lstActionControl, Luu, true, ActionType.Save);
            AddActionControl(_lstActionControl, LoadData, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        List<string> lstCheck = new List<string>() { "IsCheckCat", "IsCheckMay", "IsCheckHoanThanh" };

        protected override void OnLoad(EventArgs e)
        {
            LoadData();
            //CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            GetSearchLookUpDVSX();
            LoadDSCongDoan();

       
        }

        private void GetSearchLookUpDVSX()
        {
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            repositoryItemSearchLookUpDVSX.DataSource = dtDVSX;

        }

        private void CreateDefault()
        {
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            searchLookUpEditDVSX.Properties.DataSource = dtDVSX;
            searchLookUpEditDVSX.Properties.ValueMember = "MaDVSX";
            searchLookUpEditDVSX.Properties.DisplayMember = "TenDVSX";
            searchLookUpEditDVSX.Properties.Appearance.ForeColor = Color.Red;
            GridView dvView = searchLookUpEditDVSX.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaDVSX", Caption = "Mã DVSX", Name = "colMaDVSX", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVSX", Caption = "Đơn vị sản xuất", Name = "colTenDVSX", Visible = true });

            }

        }

        private void LoadData()
        {
            CreateDefault();
            _dttemp = new DataTable();
            txtDonHang.Text = _madhgop;
            txtMaHang.Text = _tenhang;
            txtLenhSX.Text = _malenh;
            txtDot.Text = _dot;
            searchLookUpEditDVSX.EditValue = _madvsx;
            
            _isDvsx = 2;
            //string encodedMadh = Uri.EscapeDataString(_madh);
            //string encodedMalenh = Uri.EscapeDataString(_malenh);
            //string encodedMalenhsanxuat = Uri.EscapeDataString(_malenhsanxuat);
            //string encodedPoid = Uri.EscapeDataString(_poid);
            //// Tạo URL với các tham số đã encode
            //string urldvsx = string.Format("{0}?madh={1}&&malenh={2}&&malenhsanxuat={3}&&poid={4}", URL + "CanDoiDonHangTong/GetChiTietCanDoiDVSX", _madh, _malenh, _malenhsanxuat, _poid);

            string urldvsx = string.Format("{0}?malenhsanxuat={1}", URL + "CanDoiDonHangTong/GetChiTietCanDoiDVSX", _malenhsanxuat);
            string jsondvsx = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvsx); }).Result;
            tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
            _tblCanDoiLenhSX_BeforeEdit = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
            // _SLTong = tbldvsx.AsEnumerable().Sum(x => Convert.ToInt32(x["SoLuong"]));
            if (tbldvsx.Rows.Count == 0 || tbldvsx == null) return;
            //if (tbldvsx.Rows.Count == 0 || tbldvsx == null) return;
            if (tbldvsx.Rows.Count > 1 || tbldvsx != null)
            {
                if (tbldvsx.Columns.Contains("GhiChu"))
                {
                    txtGhiChu.Text = tbldvsx.Rows[0]["GhiChu"]?.ToString() ?? "";
                }
            }    
                
            // _dttemp.Clear();
            //RemoveColumnsWithAtSign(_dttemp, gridView2);
            //TaoCot(_dttemp, gridView2);
            //_dttemp = getDataTemp(_dttemp, false);
            //RemoveDuplicateColumns(gridView2);
            // gridControl2.DataSource = _dttemp;

            CreateBandGridSize_Detail(tbldvsx, bandViewCanDoiLSX, gbSize);
            grcCanDoiLSX.DataSource = tbldvsx;
            grcCanDoiLSX.RefreshDataSource();
            string url = string.Format("{0}?madh={1}&&malenh={2}", URL + "CanDoiDonHangTong/GetChiTietCanDoiNPL", _magop, _malenhsanxuat);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblCanDoiNPL = JsonConvert.DeserializeObject<DataTable>(json);
            //if (tblCanDoiNPL.Rows.Count > 0)
            //{
            //    txtGhiChu.Text = tblCanDoiNPL.Rows[0]["GhiChuGop"]?.ToString().Trim() ?? "";
            //}
            //else
            //{
            //    txtGhiChu.Text = "";
            //}
            gridControl1.DataSource = tblCanDoiNPL;
            bandViewCanDoiLSX.ExpandAllGroups();
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
        private void RemoveDuplicateColumns(GridView gridView)
        {
            HashSet<string> columnNames = new HashSet<string>();

            // Duyệt qua danh sách cột từ cuối về đầu
            for (int i = gridView.Columns.Count - 1; i >= 0; i--)
            {
                GridColumn column = gridView.Columns[i];

                // Nếu tên cột đã tồn tại trong HashSet, loại bỏ cột đó khỏi GridView
                if (columnNames.Contains(column.FieldName))
                {
                    gridView.Columns.Remove(column);
                }
                else
                {
                    // Nếu không, thêm tên cột vào HashSet
                    columnNames.Add(column.FieldName);
                }
            }
        }
        private void TaoCot(DataTable _dt, GridView grdV)
        {
            if (_dt.Columns.Count == 0)
            {
                _dt.Columns.Add("POID", typeof(string));
                _dt.Columns.Add("PO", typeof(string));
                _dt.Columns.Add("MaQG", typeof(string));
                _dt.Columns.Add("TenQG", typeof(string));
                _dt.Columns.Add("NgayGH", typeof(string));
                _dt.Columns.Add("MaMau", typeof(string));
                _dt.Columns.Add("TenMau", typeof(string));
                _dt.Columns.Add("CodeMau", typeof(string));
                _dt.Columns.Add("DauSizeID", typeof(string));
                _dt.Columns.Add("DauSize", typeof(string));
                _dt.Columns.Add("TrangThai", typeof(int));
                _dt.Columns.Add("Dot", typeof(string));
                _dt.Columns.Add("MaDH", typeof(string));
                _dt.Columns.Add("Line", typeof(string));
                _dt.Columns.Add("MaLenh", typeof(string));
                _dt.Columns.Add("DepName", typeof(string));
             ;

            }
            int _valuIndedx = 12;
            for (int i = 0; i < tbldvsx.Rows.Count; i++)
            {
                string sizeID = tbldvsx.Rows[i]["SizeID"].ToString() + "@Size@" + tbldvsx.Rows[i]["Size"].ToString();
                bool columnExists = ColumnExists(_dt, sizeID);
                if (!columnExists)
                {
                    _dt.Columns.Add(sizeID, typeof(int));
                }

                int columnIndexToInsert = 12; // Vị trí cột cần chèn 
                _valuIndedx = _valuIndedx + 1;
                GridColumn newColumn = new GridColumn
                {
                    FieldName = sizeID,
                    Caption = tbldvsx.Rows[i]["Size"].ToString(),
                    Name = "Size@" + tbldvsx.Rows[i]["SizeID"].ToString(),
                    Visible = true,
                    Width = 50,
                    VisibleIndex = _valuIndedx,

                };
                grdV.Columns.Insert(columnIndexToInsert, newColumn);
                string[] arrName = newColumn.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                if (grdV.Name == "gridView2")
                {
                    if (arrName.Length > 1)
                    {
                        string tag1 = newColumn.FieldName + "|KH";
                        GridColumnSummaryItem item1 = newColumn.Summary.Add(DevExpress.Data.SummaryItemType.Custom, newColumn.FieldName, "{0:n0}");
                        item1.Tag = tag1;
                    }

                    if (colSizeType_1.Summary.Count == 0)
                    {
                        colSizeType_1.Summary.Add(DevExpress.Data.SummaryItemType.Custom, "Size", "SLTH");
                    }
                }

            }
            GridColumn newColumnS = new GridColumn();
            newColumnS.FieldName = "Amount"; // Tên trường dữ liệu
            newColumnS.Caption = "Tổng"; // Tiêu đề cột
            newColumnS.Visible = true; // Có hiển thị cột hay không
            newColumnS.Width = 40; // Độ rộng của cột
            newColumnS.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            newColumnS.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //newColumnS.SummaryItem
            newColumnS.Summary.Add(DevExpress.Data.SummaryItemType.Sum, newColumnS.FieldName, "{0:n0}");

            // Thêm cột vào GridView
            grdV.Columns.Add(newColumnS);
        }
        private static bool ColumnExists(DataTable dataTable, string columnName)
        {
            // Lặp qua tất cả các cột và kiểm tra tên của chúng
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnName == columnName)
                {
                    return true;
                }
            }
            return false;
        }
        private DataTable getDataTemp(DataTable DTemp, bool IsGrd)
        {
            for (int i = 0; i < tbldvsx.Rows.Count; i++)
            {
                string _Sizeheader = tbldvsx.Rows[i]["SizeID"].ToString() + "@Size@" + tbldvsx.Rows[i]["Size"].ToString();
                // Lọc dữ liệu từ DataTable
                string filterExpression = $"POID = '{tbldvsx.Rows[i]["POID"].ToString()}' AND MaMau = '{tbldvsx.Rows[i]["MaMau"].ToString()}' AND DauSizeID = '{tbldvsx.Rows[i]["DauSizeID"].ToString()}' AND MaLenh = '{tbldvsx.Rows[i]["MaLenh"].ToString()}' AND Line = '{tbldvsx.Rows[i]["Line"].ToString()}'";
                DataRow[] filteredRows = DTemp.Select(filterExpression);
                if (filteredRows.Length == 0)
                {

                    DataRow _dr = DTemp.NewRow();
                    _dr["POID"] = tbldvsx.Rows[i]["POID"].ToString();
                    _dr["PO"] = tbldvsx.Rows[i]["PO"].ToString();
                    _dr["MaQG"] = tbldvsx.Rows[i]["MaQG"].ToString();
                    _dr["TenQG"] = tbldvsx.Rows[i]["TenQG"].ToString();
                    _dr["NgayGH"] = tbldvsx.Rows[i]["NgayGH"].ToString();
                    _dr["MaMau"] = tbldvsx.Rows[i]["MaMau"].ToString();
                    _dr["TenMau"] = tbldvsx.Rows[i]["TenMau"].ToString();
                    _dr["CodeMau"] = tbldvsx.Rows[i]["CodeMau"].ToString();
                    _dr["DauSizeID"] = tbldvsx.Rows[i]["DauSizeID"].ToString();
                    _dr["DauSize"] = tbldvsx.Rows[i]["DauSize"].ToString();
                    _dr["TrangThai"] = tbldvsx.Rows[i]["TrangThai"].ToString();
                    _dr["MaDH"] = tbldvsx.Rows[i]["MaDH"].ToString();
                    _dr["MaLenh"] = tbldvsx.Rows[i]["MaLenh"].ToString();
                    _dr["DepName"] = tbldvsx.Rows[i]["DepName"].ToString();
                    _dr["Line"] = tbldvsx.Rows[i]["Line"].ToString();

                    // if (!IsGrd)
                    {
                        _dr[_Sizeheader] = tbldvsx.Rows[i]["SoLuong"].ToString() == "" ? 0 : Convert.ToInt32(tbldvsx.Rows[i]["SoLuong"].ToString());
                    }


                    DTemp.Rows.Add(_dr);
                }
                else
                {

                    DataRow foundRow = filteredRows[0];
                    if (Convert.ToInt32(tbldvsx.Rows[i]["TrangThai"]) == 1)
                    {
                        foundRow["TrangThai"] = 1;
                    }
                    //if (!IsGrd)
                    foundRow[_Sizeheader] = tbldvsx.Rows[i]["SoLuong"].ToString() == "" ? 0 : Convert.ToInt32(tbldvsx.Rows[i]["SoLuong"].ToString());
                }
            }
            return DTemp;
        }

        private void gridView2_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
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
                    _dttemp.AcceptChanges();
                    //DataTable _dtgrd = gridControl1.DataSource as DataTable;
                    DataTable _dtgrd = ((DataTable)grcCanDoiLSX.DataSource).Copy();
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

        private int CalculateCustomValue(DataTable _dt, string tagname)
        {
            int result = 0; // Ví dụ: Tính tổng của một cột.

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
                            result += Convert.ToInt32(valueSize);
                        }
                    }
                }
            }
            return result;
        }

        //private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    if (e.Value == null) return;
        //    GridView view = (GridView)sender;
        //    CanDoiDonViSanXuatSaveEntity _editCanDoiSX = new CanDoiDonViSanXuatSaveEntity();
        //    foreach (GridColumn col in view.Columns)
        //    {
        //        string[] arrMTName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (arrMTName.Length == 1) continue;
        //        if (e.Column.ToString() == arrMTName[2])
        //        {
        //            // phú : kiểm tra size có trong đơn hàng không
        //            string _madhFocus = gridView2.GetRowCellValue(gridView2.FocusedRowHandle, colMaDH_1).ToString();
        //            string _poid = gridView2.GetRowCellValue(gridView2.FocusedRowHandle, colPOID_1).ToString();
        //            string _mamau = gridView2.GetRowCellValue(gridView2.FocusedRowHandle, colColorID_1).ToString();
        //            string _dausizeid = gridView2.GetRowCellValue(gridView2.FocusedRowHandle, colSizeTypeID_1).ToString();
        //            string _filename = gridView2.FocusedColumn.FieldName.ToString();
        //            string urlKT = string.Format("{0}?madh={1}&&malenh={2}&&poid={3}&&mamau={4}&&dausizeid={5}", URL + "CanDoiDonHangTong/GetKTCanDoiNPL", _madhFocus, _malenh, _poid, _mamau, _dausizeid);
        //            string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
        //            DataTable tblKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
        //            if (!tblKT.Columns.Contains(_filename))
        //            {
                      
        //                MessageBox.Show("Size không có trong đơn hàng "+ _madhFocus +".", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                   
        //                return;
        //            }
        //            //
        //            _editCanDoiSX.MaLenhSanXuat = _malenhsanxuat;
        //            _editCanDoiSX.MaLenh = _malenh;
        //            _editCanDoiSX.DotSX = _dot;
        //            _editCanDoiSX.MaDH = view.GetFocusedRowCellValue(view.Columns["MaDH"]).ToString();
        //            _editCanDoiSX.MaDVSX = searchLookUpEditDVSX.EditValue.ToString() == "" ? _madvsx : searchLookUpEditDVSX.EditValue.ToString();
        //            _editCanDoiSX.POID = view.GetFocusedRowCellValue(view.Columns["POID"]).ToString();
        //            _editCanDoiSX.PO = view.GetFocusedRowCellValue(view.Columns["PO"]).ToString();
        //            _editCanDoiSX.MaQG = view.GetFocusedRowCellValue(view.Columns["MaQG"]).ToString();
        //            _editCanDoiSX.MaMau = view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString();
        //            _editCanDoiSX.DauSizeID = view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString();
        //            _editCanDoiSX.DauSize = view.GetFocusedRowCellValue(view.Columns["DauSize"]).ToString();
        //            _editCanDoiSX.SizeID = arrMTName[0];
        //            _editCanDoiSX.Size = arrMTName[2];
        //            if (ListCanDoiSX.Count > 0)
        //            {
        //                foreach (CanDoiDonViSanXuatSaveEntity editCanDoiSX in ListCanDoiSX)
        //                {
        //                    if (editCanDoiSX.POID.ToString() == view.GetFocusedRowCellValue(view.Columns["POID"]).ToString()
        //                        && editCanDoiSX.MaMau.ToString() == view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString() && editCanDoiSX.DauSizeID.ToString() == view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString()
        //                        && editCanDoiSX.SizeID.ToString() == arrMTName[0].ToString())
        //                    {
        //                        _editCanDoiSX.SoLuong = Convert.ToInt32(view.GetFocusedRowCellValue(view.Columns["SoLuong"]));
        //                    }
        //                    else
        //                    {
        //                        _editCanDoiSX.SoLuong = Convert.ToInt32(e.Value);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                _editCanDoiSX.SoLuong = Convert.ToInt32(e.Value);
        //            }
        //            CanDoiDonViSanXuatSaveEntity objCanDoi = (from l in ListCanDoiSX
        //                                                      where l.POID.ToString() == view.GetFocusedRowCellValue(view.Columns["POID"]).ToString() &&
        //                                                      l.DauSizeID.ToString() == view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString() &&
        //                                                      l.MaMau.ToString() == view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString() &&
        //                                                       l.MaDH.ToString() == view.GetFocusedRowCellValue(view.Columns["MaDH"]).ToString() &&
        //                                                      l.SizeID.ToString() == arrMTName[0].ToString()
        //                                                      select l).FirstOrDefault();
        //            if (objCanDoi == null)
        //            {
        //                ListCanDoiSX.Add(_editCanDoiSX);
        //            }
        //        }
        //    }
        //}

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void LayDuLieu()
        {
            totalAmountcheck = 0;
            if (tblCanDoiNPL == null)
                return;
            if (ListCanDoiSX.Count == 0)
                return;
            foreach (DataRow row in _dttemp.Rows)
            {
                foreach (DataColumn col in _dttemp.Columns)
                {
                    if (col.ColumnName != "MaDH" && col.ColumnName != "POID" && col.ColumnName != "PO" && col.ColumnName != "MaMau" && col.ColumnName != "DauSizeID" && col.ColumnName != "DauSize"
                        && col.ColumnName != "MaLenh" && col.ColumnName != "MaDVSX" && col.ColumnName != "DotSX" && col.ColumnName != "MaQG" && col.ColumnName != "TenMau" && col.ColumnName != "TenQG" && col.ColumnName != "NgayGH" && col.ColumnName != "Dot")
                    {
                        try
                        {
                            if (row[col.ColumnName] != null && !string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                            {
                                int value = Convert.ToInt32(row[col.ColumnName]);
                                totalAmountcheck += value;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            foreach (DataRow dr in tblCanDoiNPL.Rows)
            {
                if (Convert.ToInt32(dr["TrangThai"]) == 0)
                {
                    dr["CapPhat"] = Convert.ToDecimal((Convert.ToDouble(dr["DinhMuc"]) * totalAmountcheck));
                    dr["TrangThai"] = 0;
                    dr["GhiChu"] = dr["GhiChu"];
                }
            }
            _npl = 1;

            //int totalQuantity = 0;

            //foreach (DataRow row1 in tblCanDoiNPL.Rows)
            //{
            //    totalQuantity = 0;
            //    string mamau1 = row1["MaMauLenh"].ToString();
            //    string dausize1 = row1["DauSizeLenh"].ToString();
            //    string size = row1["SizeLenh"].ToString().Trim();

            //    foreach (DataRow row2 in _dttemp.Rows)
            //    {
            //        string mamau2 = row2["MaMau"].ToString();
            //        string dausize2 = row2["DauSizeID"].ToString();

            //        // Trường hợp 1: MaMau có, DauSizeID có và Size có
            //        if (mamau1 == mamau2 && dausize1 == dausize2)
            //        {
            //            if (size == "ALL")
            //            {
            //                // Trường hợp 2: MaMau có, DauSizeID có và Size = "ALL"
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        int quantity = Convert.ToInt32(row2[col]);
            //                        totalQuantity += quantity;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                // Trường hợp 1: MaMau có, DauSizeID có và Size có (size cụ thể)
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        string columnBaseName = col.ColumnName.Split('@')[0];
            //                        if (size.Split(';').Contains(columnBaseName))
            //                        {
            //                            int quantity = Convert.ToInt32(row2[col]);
            //                            totalQuantity += quantity;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else if (mamau1 == mamau2 && dausize1 == "ALL")
            //        {
            //            // Trường hợp 3: MaMau có, DauSizeID = "ALL" và Size có
            //            if (size == "ALL")
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        int quantity = Convert.ToInt32(row2[col]);
            //                        totalQuantity += quantity;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        string columnBaseName = col.ColumnName.Split('@')[0];
            //                        if (size.Split(';').Contains(columnBaseName))
            //                        {
            //                            int quantity = Convert.ToInt32(row2[col]);
            //                            totalQuantity += quantity;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else if (mamau1 == "ALL" && dausize2 == "ALL")
            //        {
            //            // Trường hợp 5: MaMau = "ALL", DauSizeID = "ALL" và Size = "ALL"
            //            if (size == "ALL")
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        int quantity = Convert.ToInt32(row2[col]);
            //                        totalQuantity += quantity;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        string columnBaseName = col.ColumnName.Split('@')[0];
            //                        if (size.Split(';').Contains(columnBaseName))
            //                        {
            //                            int quantity = Convert.ToInt32(row2[col]);
            //                            totalQuantity += quantity;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else if (mamau1 == "ALL" && dausize2 != "ALL")
            //        {
            //            // Trường hợp 7: MaMau = "ALL", DauSizeID có và Size có
            //            if (size == "ALL")
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        int quantity = Convert.ToInt32(row2[col]);
            //                        totalQuantity += quantity;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        string columnBaseName = col.ColumnName.Split('@')[0];
            //                        if (size.Split(';').Contains(columnBaseName))
            //                        {
            //                            int quantity = Convert.ToInt32(row2[col]);
            //                            totalQuantity += quantity;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else if (mamau1 == "ALL" && dausize2 == "ALL")
            //        {
            //            // Trường hợp 6: MaMau = "ALL", DauSizeID = "ALL" và Size có
            //            if (size == "ALL")
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        int quantity = Convert.ToInt32(row2[col]);
            //                        totalQuantity += quantity;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (DataColumn col in _dttemp.Columns)
            //                {
            //                    if (col.ColumnName.Contains("@"))
            //                    {
            //                        string columnBaseName = col.ColumnName.Split('@')[0];
            //                        if (size.Split(';').Contains(columnBaseName))
            //                        {
            //                            int quantity = Convert.ToInt32(row2[col]);
            //                            totalQuantity += quantity;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else if (mamau1 == "ALL" && dausize2 != "ALL" && size == "ALL")
            //        {
            //            // Trường hợp 8: MaMau = "ALL", DauSizeID có và Size = "ALL"
            //            foreach (DataColumn col in _dttemp.Columns)
            //            {
            //                if (col.ColumnName.Contains("@"))
            //                {
            //                    int quantity = Convert.ToInt32(row2[col]);
            //                    totalQuantity += quantity;
            //                }
            //            }
            //        }
            //    }

            //    row1["SoLuong"] = totalQuantity;
            //    row1["CapPhat"] = Math.Round(totalQuantity * Convert.ToDouble(row1["DinhMuc"]), 4);
            //}

            //_npl = 1;


        }

        private void btn_LayDuLieu_Click(object sender, EventArgs e)
        {
            LayDuLieu();
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

        private void gridView2_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //GridView view = (GridView)sender;

            //// Kiểm tra xem dòng này có được chọn không
            //if (view.IsRowSelected(e.RowHandle))
            //{
            //    // Thiết lập màu sắc cho dòng được chọn
            //    e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            //}
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //GridView view = (GridView)sender;

            //// Kiểm tra xem dòng này có được chọn không
            //if (view.IsRowSelected(e.RowHandle))
            //{
            //    // Thiết lập màu sắc cho dòng được chọn
            //    e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            //}
        }

     

        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflag = View.GetRowCellDisplayText(e.RowHandle, View.Columns["TrangThai"]);
                if (Strflag == "1")
                {
                    e.Appearance.BackColor = Color.LightCyan;
                    e.Appearance.BackColor2 = Color.SeaShell;
                    e.HighPriority = true;
                }
                //else
                //{
                //    if (Strflag == "2")
                //    {
                //        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFBD1"); ;
                //        e.Appearance.BackColor2 = Color.SeaShell;
                //        e.HighPriority = true;
                //    }
                //}
            }
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

        private void gridView2_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflag = View.GetRowCellDisplayText(e.RowHandle, View.Columns["TrangThai"]);
                if (Strflag == "1")
                {
                    e.Appearance.BackColor = Color.LightCyan;
                    e.Appearance.BackColor2 = Color.SeaShell;
                    e.HighPriority = true;
                }
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

            }
        }

        //private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        //{
        //    GridView view = (GridView)sender;
        //    if (view == null) return;
        //    //if (e.IsGetData)
        //    //{
        //    //    int sum = 0;
        //    //    bool isEmpty = true;
        //    //    DataRow row = (e.Row as DataRowView).Row;
        //    //    foreach (GridColumn col in view.Columns)
        //    //    {
        //    //        if (col.FieldName != "MaDH" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "TenMau" &&
        //    //            col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" &&
        //    //            col.FieldName != "DotSX" && col.FieldName != "MaQG" && col.FieldName != "TenQG" && col.FieldName != "TrangThai" && col.FieldName != "NgayGH" && col.FieldName != "Dot" &&
        //    //            row.Table.Columns.Contains(col.FieldName) &&
        //    //            col.UnboundType == UnboundColumnType.Bound)
        //    //        {
        //    //            try
        //    //            {
        //    //                if (row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
        //    //                {
        //    //                    int value = Convert.ToInt32(row[col.FieldName]);
        //    //                    sum += value;
        //    //                }
        //    //                isEmpty = false;
        //    //            }
        //    //            catch (Exception ex)
        //    //            {
        //    //                throw new Exception(ex.Message);
        //    //            }
        //    //        }
        //    //    }
        //    //    if (!isEmpty)
        //    //        e.Value = sum;
        //    //}


        //    try
        //    {

        //        if (e.IsGetData)
        //        {
        //            decimal tong = 0;
        //            if(view.Columns.Count > 0)
        //            {
        //                foreach (GridColumn col in view.Columns)
        //                {

        //                    if (col.FieldName != null && col.FieldName.Contains("@Size@"))
        //                    {
        //                        object cellValue = gridView2.GetRowCellValue(e.ListSourceRowIndex, col);
        //                        if (cellValue != null && decimal.TryParse(cellValue.ToString(), out decimal val))
        //                        {
        //                            tong += val;
        //                        }
        //                    }
        //                }
        //            }
                  


        //            e.Value = tong;
        //        }
        //        }
        //    catch (Exception ex)
        //    {

        //    }
        //}


        private void gridView1_KeyDown(object sender, KeyEventArgs e)
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

            }
        }
        private void Them()
        {

            DataRow row = bandViewCanDoiLSX.GetFocusedDataRow() as DataRow;
            string SawDep = row["SawDep"]?.ToString();
            frmCanDoiDonHangChiTiet frm = new frmCanDoiDonHangChiTiet(true, _madh, _mahang, _tenhang, _dot, _poid, _malenh, _dot, _madvsx, _malenhsanxuat, _magop, _tenlenh, _sttlenh, _isKeoVe,_madhgop, SawDep, txtGhiChu.Text.ToString());
            frm.ShowDialog();
            LoadData();
        }
        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            Them();
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            //{
            if (_isKeoVe == 1)
            {
                XtraMessageBox.Show("Lệnh này đã được thực hiện sản xuất.\nKhông thể xóa được!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa lệnh này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                DataRow row = bandViewCanDoiLSX.GetFocusedDataRow();
                if (row == null) return;
                List<CanDoiDonViSanXuatSaveEntity> _lstItemCanDoiDelete = new List<CanDoiDonViSanXuatSaveEntity>();
                CanDoiDonViSanXuatSaveEntity itemCanDoiDelete = new CanDoiDonViSanXuatSaveEntity();
                itemCanDoiDelete.MaDH = _madh;
                itemCanDoiDelete.MaLenhSanXuat = _malenhsanxuat;
                itemCanDoiDelete.MaDVSX = _madvsx;
                itemCanDoiDelete.DotSX = txtDot.Text;
                itemCanDoiDelete.POID = row["POID"].ToString();
                itemCanDoiDelete.DauSizeID = row["DauSizeID"].ToString();
                itemCanDoiDelete.MaMau = row["MaMau"].ToString();
                _lstItemCanDoiDelete.Add(itemCanDoiDelete);

                DataTable _tblDeleteItemCanDoi = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(_lstItemCanDoiDelete));
                string url = string.Format("{0}?", URL + "CanDoiDonHangTong/DeleteItemCanDoi");
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, _tblDeleteItemCanDoi); }).Result;
                LoadData();
                LayDuLieu();
            }
            //}    
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền Xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

        }

        private void searchLookUpEditDVSX_EditValueChanged(object sender, EventArgs e)
        {
            _isDvsx = 1;

        }
        private void gridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void simpleButtonXoa_Click(object sender, EventArgs e)
        {
            //if (_isKeoVe == 1)
            //{
            //    XtraMessageBox.Show("Nguyên phụ liệu này đã được thực hiện sản xuất.\nKhông thể xóa được!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //DialogResult messResult = MessageBox.Show("Bạn có muốn xóa nguyên phụ liệu này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (messResult == DialogResult.Yes)
            //{
            //    string url = string.Format("{0}?madh={1}&&malenh={2}&&malenhsanxuat={3}&&manpl={4}", URL + "CanDoiDonHangTong/DeleteDM", _magop, _malenh, _malenhsanxuat, _manpl);
            //    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            //    if (result.ToLower() == "true")
            //        LoadData();
            //    else XtraMessageBox.Show(result);
            //}
            //if (_isKeoVe == 1)
            //{
            //    XtraMessageBox.Show("Nguyên phụ liệu này đã được thực hiện sản xuất.\nKhông thể xóa được!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            var selectedRows = gridView1.GetSelectedRows();
            if (selectedRows.Length == 0)
            {
               
                return;
            }
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa các nguyên phụ liệu đã chọn không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                
               
                foreach (int rowHandle in selectedRows)
                {
                    string manpl = gridView1.GetRowCellValue(rowHandle, "MaNPL")?.ToString();
                    string url = string.Format("{0}?madh={1}&&malenh={2}&&malenhsanxuat={3}&&manpl={4}&&username={5}", URL + "CanDoiDonHangTong/DeleteDM", _magop, _malenh, _malenhsanxuat, manpl, GlobleData.UserName);

                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;

                    if (result.ToLower() != "true")
                    {
                        XtraMessageBox.Show($"Không thể xoá dòng có mã NPL: {manpl}\nLý do: {result}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LoadData();
                        return;
                    }
                }
                    LoadData(); 
            }
        }
      
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object _objMaNPL = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaNPL = view.GetRowCellValue(childHandle, colMaNPL);
            }
            else
            {
                _objMaNPL = view.GetFocusedRowCellValue(colMaNPL);
            }
            if (_objMaNPL != null)
                _manpl = _objMaNPL.ToString();
        }

        private void gridViewCongDoan_ShowingEditor(object sender, CancelEventArgs e)
        {
            bool isDongThung = (bool)gridViewCongDoan.GetFocusedRowCellValue(this.gridColumnIsDongThung);
            if (isDongThung)
            {
                e.Cancel = true;
                XtraMessageBox.Show("Lệnh sản xuất này đã được lập kế hoạch.\nKhông thể thay đổi DVSX.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            object _objMaNPL = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaNPL = view.GetRowCellValue(childHandle, colMaNPL);
            }
            else
            {
                _objMaNPL = view.GetFocusedRowCellValue(colMaNPL);
            }
            if (_objMaNPL != null)
                _manpl = _objMaNPL.ToString();
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            if (e.Value == null || e.Value == "") return;
            GridView view = (GridView)sender;
            DataRow drChange = view.GetFocusedDataRow();
            if (e.Column.FieldName == "CapPhat" || e.Column.FieldName == "DinhMuc")
            {
                drChange["TrangThai"] = 1;
                // drChange["NguoiSua"] = GlobleData.UserName;
                if (e.Column.FieldName == "DinhMuc")
                {
                    drChange["CapPhat"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) * Convert.ToInt32(_SLTong)), 4);
                }
                if (e.Column.FieldName == "CapPhat")
                {
                    if (Convert.ToInt32(_SLTong) == 0)
                    {
                        MessageBox.Show("Số lượng tổng bằng 0. Không thể tính lại định mức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        drChange["DinhMuc"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) / Convert.ToInt32(_SLTong)), 4);
                    }

                }
            }
            _npl = 1;
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "DinhMuc" || view.FocusedColumn.FieldName == "CapPhat")
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {

                    e.Valid = false;
                    e.ErrorText = "Giá trị không được để trống!";
                }
                if (Convert.ToInt32(e.Value) < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng phải lớn hơn 0";
                }
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "DinhMuc" || e.Column.FieldName == "CapPhat")
            {
                // Tạo một RepositoryItemTextEdit
                var repositoryItemTextEdit = new RepositoryItemTextEdit();
                repositoryItemTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                repositoryItemTextEdit.Mask.EditMask = "n4"; // Cho phép nhập số thập phân với 2 chữ số sau dấu phẩy

                e.RepositoryItem = repositoryItemTextEdit;
            }
        }

        private void barButtonItemLSX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel | *.xlsx";

                DataTable tbl = grcCanDoiLSX.DataSource as DataTable;
                string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCEL&Para1={_malenhsanxuat}&Para2={_madh}&Para3=A");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                Sfd.FileName = string.Format("LenhSX{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), dtTable.Rows[0]["MaHang"].ToString(), dtTable.Rows[0]["TenDVSX"].ToString());
                if (tbl.Rows.Count == 0 && tbl == null) return;
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

                        string fileName = "LSX_ROBE 4.xlsx";
                        string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                        string TemplateFileName = path;
                        string ExportFileName = Sfd.FileName;


                        ExportExcelLSX(TemplateFileName, ExportFileName, tbl, dtTable);
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
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void ExportExcelLSX(string TemplateFileName, string ExportFileName, DataTable tbl, DataTable dtTable)
        {
            try
            {

                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    string templateFilePath = TemplateFileName;

                    string resultFilePath = ExportFileName;
                    FileInfo templateFile = new FileInfo(templateFilePath);
                    ExcelPackage templatePackage = new ExcelPackage(templateFile);
                    ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    decimal sumSL_CapPhap = 0;

                    worksheet.Cells["c4"].Value = dtTable.Rows[0]["TenKH"].ToString();
                    worksheet.Cells["c5"].Value = dtTable.Rows[0]["TenHang"].ToString();
                    worksheet.Cells["c6"].Value = dtTable.Rows[0]["DotSX"].ToString();
                    worksheet.Cells["c7"].Value = dtTable.Rows[0]["TenDVSX"].ToString();
                    worksheet.Cells["c8"].Value = dtTable.Rows[0]["TenCL"].ToString();
                    worksheet.Cells["c9"].Value = dtTable.Rows[0]["BookingMaHang"].ToString();

                    var rangeGhiChu = worksheet.Cells[10, 1, 10, 2];
                    rangeGhiChu.Merge = true;
                    rangeGhiChu.Value = "Ghi Chú";
                    rangeGhiChu.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    rangeGhiChu.Style.Font.Bold = true;
                    rangeGhiChu.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rangeGhiChu.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rangeGhiChu.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rangeGhiChu.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells[10, 3].Value = txtGhiChu.Text.ToString();
                    worksheet.Cells[10, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Cells[10, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[10, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[10, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[10, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    List<string> columnNamesWithSize = tbl.Columns.Cast<DataColumn>()
                       .Where(column => column.ColumnName.Contains("@"))
                       .Select(column => column.ColumnName)
                       .Distinct()
                       .ToList();

                    int colums = 7;
                    range = worksheet.Cells[11, colums, 11, colums + columnNamesWithSize.Count - 1]; range.Value = "Size"; range.Merge = true; range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;
                    foreach (var item in columnNamesWithSize)
                    {
                        string[] _split = item.Split('@');
                        string _size = _split[2].ToString();
                        worksheet.Cells[12, colums].Value = _size;
                        worksheet.Cells[12, colums].Style.Font.Bold = true;
                        colums++;
                    }
                    range = worksheet.Cells[11, colums, 12, colums]; range.Value = "Grand Total"; range.Style.WrapText = true; range.Merge = true; range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;
                    int row = 13;
                    foreach (DataRow item in tbl.Rows)
                    {
                        worksheet.Cells[row, 1].Value = item["CodeMau"].ToString(); worksheet.Cells[row, 1].Style.WrapText = true;
                        worksheet.Cells[row, 2].Value = item["TenMau"].ToString(); worksheet.Cells[row, 2].Style.WrapText = true;
                        worksheet.Cells[row, 3].Value = item["PO"].ToString();
                        worksheet.Cells[row, 4].Value = item["DauSize"].ToString();
                        worksheet.Cells[row, 5].Value = item["TenQG"].ToString();
                        worksheet.Cells[row, 6].Value = Convert.ToDateTime(item["NgayGH"].ToString()).ToString("dd/MM/yyyy");
                        int sumSize = 0;
                        colums = 7;
                        foreach (var lstSize in columnNamesWithSize)
                        {
                            int sl = string.IsNullOrEmpty(item[lstSize].ToString()) ? 0 : Convert.ToInt32(item[lstSize]);
                            if (sl == 0)
                            {
                                worksheet.Cells[row, colums].Value = "";
                            }
                            else
                            {
                                worksheet.Cells[row, colums].Value = sl;
                            }
                            worksheet.Cells[row, colums].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
                            //sumSize += Convert.ToInt32(item[lstSize]);
                            sumSize += sl;
                            colums++;
                        }
                        worksheet.Cells[row, colums].Value = sumSize;
                        row++;
                    }
                    range = worksheet.Cells[row, 1]; range.Value = "Grand Total"; range.Style.Font.Color.SetColor(Color.Red);
                    for (int i = 7; i < colums + 1; i++)
                    {
                        worksheet.Cells[row, i].Formula = "=SUM(" + worksheet.Cells[13, i].Address + ":" + worksheet.Cells[row - 1, i].Address + ")";
                        worksheet.Cells[row, i].Style.Font.Bold = true;
                    }
                    range = worksheet.Cells[row, 1, row, colums];
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 150, 148));
                    var borderData = worksheet.Cells[11, 1, row, colums].Style.Border;
                    borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = ExcelBorderStyle.Thin;



                    FileInfo resultFile = new FileInfo(resultFilePath);
                    templatePackage.SaveAs(resultFile);
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }
        private void barButtonItemNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel | *.xlsx";
                string url1 = string.Format("{0}?madh={1}&&malenh={2}", URL + "CanDoiDonHangTong/GetCDNPL", _magop, _malenhsanxuat);
                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json1);
                string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCEL&Para1={_malenhsanxuat}&Para2={_madh}&Para3=A");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                Sfd.FileName = string.Format("NguyenPL_{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), dtTable.Rows[0]["MaHang"].ToString(), dtTable.Rows[0]["TenDVSX"].ToString());
                if (tbl.Rows.Count <= 0 || tbl == null ) return;
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

                        string fileName = "MauCapPhatNPL.xlsx";
                        string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                        string TemplateFileName = path;
                        string ExportFileName = Sfd.FileName;

                        ExportExcel(TemplateFileName, ExportFileName, tbl, dtTable);
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
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ExportExcel(string TemplateFileName, string ExportFileName, DataTable tbl, DataTable dtTable)
        {
            try
            {
                string date = DateTime.Now.ToString("dd MM yyyy");
                string[] dateParts = date.Split(' ');

                string day = dateParts[0]; // Ngày
                string month = dateParts[1]; // Tháng
                string year = dateParts[2]; // Năm

                //Lấy cột po tác ra mảng
                var poValues = dtTable.AsEnumerable().Select(row => row.Field<string>("po")).ToArray();

                // Chuyển mảng thành chuỗi bằng cách nối các giá trị với dấu phẩy ","
                string concatenatedString = string.Join(",", poValues);

                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    string templateFilePath = TemplateFileName;

                    string resultFilePath = ExportFileName;
                    FileInfo templateFile = new FileInfo(templateFilePath);
                    ExcelPackage templatePackage = new ExcelPackage(templateFile);
                    ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    int index = 1;
                    int row = 29;
                    decimal sumSL_CapPhap = 0;
                    for (int i = 29; i < 50; i++)
                    {
                        worksheet.Row(i).Height = 18;
                    }
                    foreach (DataRow item in tbl.Rows)
                    {
                        sumSL_CapPhap += Convert.ToDecimal(item["CapPhat"]);
                        worksheet.Cells[row, 1].Value = index;
                        worksheet.Cells[row, 2].Value = item["TenNhom"].ToString();
                        worksheet.Cells[row, 3].Value = item["MaVT"].ToString();
                        worksheet.Cells[row, 4].Value = item["TenVT"].ToString();
                        worksheet.Cells[row, 5].Value = item["MaMauVT"].ToString();
                        worksheet.Cells[row, 6].Value = item["MaMau"].ToString();
                        worksheet.Cells[row, 7].Value = item["KhoVai"].ToString();
                        worksheet.Cells[row, 8].Value = item["MaDV"].ToString();
                        worksheet.Cells[row, 9].Value = item["SoLuong"].ToString();
                        worksheet.Cells[row, 10].Value = Convert.ToDecimal(item["DinhMuc"]);
                        worksheet.Cells[row, 11].Value = Convert.ToDecimal(item["DinhMucHaoHut"]);
                        worksheet.Cells[row, 12].Value = Convert.ToDecimal(item["CapPhat"]);
                        //worksheet.Cells[row, 13].Value = Convert.ToDecimal(item["ThucNhan"]);
                        worksheet.Cells[row, 13].Value = item["GhiChuPKT"].ToString();
                        worksheet.Cells[row, 14].Value = item["GhiChu"].ToString();
                        //worksheet.Column(4).Width = 70; worksheet.Column(4).Style.WrapText = true;
                        //worksheet.Column(4).AutoFit(); worksheet.Column(6).AutoFit();
                        worksheet.Cells[27, 13].Value = "GHI CHÚ PKT";
                        worksheet.Cells[27, 14].Value = "GHI CHÚ PKH";
                        worksheet.Cells[28, 13].Value = "(12)";
                        worksheet.Cells[28, 14].Value = "(13)";

                        worksheet.Row(row).Height = -1;
                        index++;
                        row++;

                    }
                    ExcelRange borderRange = worksheet.Cells[27, 1, row - 1, 14]; // Từ dòng 29 đến dòng cuối cùng có dữ liệu
                    borderRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    borderRange.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    borderRange.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    borderRange.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    //worksheet.Cells["C4"].Value = dtTable.Rows[0]["TenKH"].ToString();
                    //worksheet.Cells["C5"].Value = dtTable.Rows[0]["TenHang"].ToString();
                    //worksheet.Cells["C6"].Value = dtTable.Rows[0]["DotSX"].ToString();
                    //worksheet.Cells["C7"].Value = dtTable.Rows[0]["TenDVSX"].ToString();
                    ////worksheet.Cells["C9"].Value = concatenatedString;
                    //worksheet.Cells["C8"].Value = dtTable.Rows[0]["TenCL"].ToString();
                    //worksheet.Cells["C9"].Value = dtTable.Rows[0]["BookingMaHang"].ToString();
                    //worksheet.Cells["I7"].Value = _SLTong + " PCS";
                    //worksheet.Cells["I8"].Value = sumSL_CapPhap + " PCS";

                    //range = worksheet.Cells[row, 3]; range.Value = "Tổng cộng"; range.Style.Font.Bold = true;

                    //range = worksheet.Cells[row, 10]; range.Value = sumSL_CapPhap; range.Style.Font.Bold = true;


                    //var borderData = worksheet.Cells[13, 1, row, 10].Style.Border;
                    //borderData.Bottom.Style =
                    //    borderData.Top.Style =
                    //    borderData.Left.Style =
                    //    borderData.Right.Style = ExcelBorderStyle.Thin;
                    //row += 2;
                    //worksheet.Row(row + 1).Height = 18;
                    //range = worksheet.Cells[row, 8, row, 9]; range.Value = "Ngày " + day + "Tháng " + month + "Năm " + year; range.Merge = true;
                    //range = worksheet.Cells[row + 1, 3]; range.Value = "Người lập/Prepared By"; range.Merge = true;
                    //range = worksheet.Cells[row + 1, 8, row + 1, 9]; range.Value = "PHÒNG KH - KD"; range.Style.Font.Size = 12; range.Merge = true; range.Style.Font.Bold = true; range.Style.Font.Italic = false;


                    FileInfo resultFile = new FileInfo(resultFilePath);
                    templatePackage.SaveAs(resultFile);
                }

            }
            catch (Exception EE)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi khi lưu file!");
                return;
            }
        }

        private void LoadDSCongDoan()
        {
            // _madh | _malenhsanxuat | _madvsx
            string url = string.Format("{0}?maDH={1}&&maLenhSX={2}", URL + "CongDoan/GetCongDoanAllowCondition", _magop, _malenhsanxuat);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable _tblCongDoan = JsonConvert.DeserializeObject<DataTable>(json);
            if (_tblCongDoan != null && _tblCongDoan.Rows.Count > 0)
            {
                if (_tblCongDoan.Columns.Contains("NguoiSua"))
                {
                    _tblCongDoan.Rows[0]["NguoiSua"] = GlobleData.UserName;
                    gridControlCongDoan.DataSource = _tblCongDoan;
                }
            }
            else
            {
                gridControlCongDoan.DataSource = InitDataCongDoan();
            }
        }

        // Nếu tblCongDoan trả về là NUll( chưa có dữ liệu trong bảng ChiTietCongDoan)
        // -> Tạo ra một DataTable với các giá trị MaDVSXCat | MaDVSXMay | MaDVSXHoanThanh là null
        private DataTable InitDataCongDoan()
        {
            DataTable tblInitCongDoan = CreateTableInitCongDoan();

            // _madh | _mahang | _malenhsanxuat  
            DataRow rowInitCongDoan = tblInitCongDoan.NewRow();

            rowInitCongDoan["MaDH"] = _madh;
            rowInitCongDoan["MaHang"] = _mahang;
            rowInitCongDoan["MaLenhSX"] = _malenhsanxuat;
            rowInitCongDoan["MaDVSXCat"] = null;
            rowInitCongDoan["MaDVSXMay"] = null;
            rowInitCongDoan["MaDVSXHoanThanh"] = null;
            rowInitCongDoan["NguoiTao"] = GlobleData.UserName;
            rowInitCongDoan["NguoiSua"] = GlobleData.UserName;
            rowInitCongDoan["MaDVSXDongThung"] = null;
            rowInitCongDoan["MaDVSXDongThung"] = null;
            rowInitCongDoan["IsDongThung"] = false;
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
            tbl.Columns.Add("IsDongThung", typeof(bool));
            return tbl;
        }

        private string LuuChiTietCongDoan()
        {
            DataTable tblSave = (gridViewCongDoan.DataSource as DataView).Table;
            if (tblSave.Rows.Count > 0 && tblSave != null)
            {
                foreach (DataRow dr in tblSave.Rows)
                {
                    if (_magop.ToString() != null)
                        dr["MaDH"] = _magop;
                }
            }
            tblSave = DeleteColumn(tblSave);
            string urlChitietCongDoan = string.Format("{0}?", URL + "CongDoan/PostChiTietCongDoan");
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlChitietCongDoan, tblSave); }).Result;
            return msResult;
        }
        private DataTable DeleteColumn(DataTable tbl)
        {
            List<string> lstNameColumn = new List<string> { "ID","MaDH","MaHang","MaLenhSX","MaDVSXCat","MaDVSXMay"
                ,"MaDVSXHoanThanh","NguoiTao","NguoiSua","MaDVSXDongThung"};
            for (int i = 0; i < tbl.Columns.Count;)
            {
                DataColumn column = tbl.Columns[i];
                if (!lstNameColumn.Contains(column.ColumnName))
                {
                    tbl.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
            return tbl;
        }
        //private void btnXuatLSXNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        SaveFileDialog Sfd = new SaveFileDialog();
        //        Sfd.Title = "File To Save";
        //        Sfd.Filter = "Excel | *.xlsx";

        //        DataTable tbl = gridControl2.DataSource as DataTable;
        //        string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCEL&Para1={_malenhsanxuat}&Para2={_madh}&Para3=A");
        //        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //        DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
        //        Sfd.FileName = string.Format("LenhSX{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), dtTable.Rows[0]["MaHang"].ToString(), dtTable.Rows[0]["TenDVSX"].ToString());
        //        if (tbl.Rows.Count == 0 && tbl == null) return;
        //        if (Sfd.ShowDialog() == DialogResult.OK)
        //        {
        //            try
        //            {
        //                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
        //                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
        //                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
        //                DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
        //                op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
        //                op.ShowGridLines = true;
        //                op.SheetName = string.Format("Bao Cao");

        //                string fileName = "LSX_ROBE 4.xlsx";
        //                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
        //                string TemplateFileName = path;
        //                string ExportFileName = Sfd.FileName;


        //                ExportExcelLSXNPL(TemplateFileName, ExportFileName, tbl, dtTable);
        //                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
        //                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
        //                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
        //                {
        //                    System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    try
        //    {
        //        SaveFileDialog Sfd = new SaveFileDialog();
        //        Sfd.Title = "File To Save";
        //        Sfd.Filter = "Excel | *.xlsx";
        //        string url1 = string.Format("{0}?madh={1}&&malenh={2}", URL + "CanDoiDonHangTong/GetCDNPL", _magop, _malenhsanxuat);
        //        string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
        //        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json1);
        //        string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCEL&Para1={_malenhsanxuat}&Para2={_madh}&Para3=A");
        //        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //        DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
        //        Sfd.FileName = string.Format("NguyenPL_{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), dtTable.Rows[0]["MaHang"].ToString(), dtTable.Rows[0]["TenDVSX"].ToString());
        //        if (tbl.Rows.Count <= 0 || tbl == null) return;
        //        if (Sfd.ShowDialog() == DialogResult.OK)
        //        {
        //            try
        //            {
        //                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
        //                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
        //                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
        //                DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
        //                op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
        //                op.ShowGridLines = true;
        //                op.SheetName = string.Format("Bao Cao");

        //                string fileName = "MauCapPhatNPL.xlsx";
        //                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
        //                string TemplateFileName = path;
        //                string ExportFileName = Sfd.FileName;

        //                ExportExcel(TemplateFileName, ExportFileName, tbl, dtTable);
        //                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
        //                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
        //                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
        //                {
        //                    System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void btnXuatLSXNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel | *.xlsx";

                // Lấy dữ liệu LSX
                DataTable tblLSX = grcCanDoiLSX.DataSource as DataTable;
                string urlLSX = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCEL&Para1={_malenhsanxuat}&Para2={_madh}&Para3=A");
                string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
                DataTable dtTableLSX = JsonConvert.DeserializeObject<DataTable>(jsonLSX);

                // Lấy dữ liệu NPL
                string urlNPL = string.Format("{0}?madh={1}&&malenh={2}", URL + "CanDoiDonHangTong/GetCDNPL", _magop, _malenhsanxuat);
                string jsonNPL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNPL); }).Result;
                DataTable tblNPL = JsonConvert.DeserializeObject<DataTable>(jsonNPL);

                // Tên file xuất
                Sfd.FileName = string.Format("LSX_NPL_{0}_{1}_{2}",
                    dtTableLSX.Rows[0]["MaHang"].ToString(),
                    dtTableLSX.Rows[0]["TenDVSX"].ToString(),
                    DateTime.Now.ToString("ddMMyyyy"));

                if ((tblLSX == null || tblLSX.Rows.Count == 0) && (tblNPL == null || tblNPL.Rows.Count <= 0))
                {
                    XtraMessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");

                        string fileName = "LSX_ROBE 4_V2.xlsx"; // Sử dụng template LSX làm chính
                        string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                        string TemplateFileName = path;
                        string ExportFileName = Sfd.FileName;

                        ExportExcelCombined(TemplateFileName, ExportFileName, tblLSX, tblNPL, dtTableLSX);

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
                        XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ExportExcelCombined(string TemplateFileName, string ExportFileName, DataTable tblLSX, DataTable tblNPL, DataTable dtTable)
        {
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    FileInfo templateFile = new FileInfo(TemplateFileName);
                    ExcelPackage templatePackage = new ExcelPackage(templateFile);
                    ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    int currentRow = 1;

                    // ==================== PHẦN LSX (Ở TRÊN) ====================
                    if (tblLSX != null && tblLSX.Rows.Count > 0)
                    {
                        // Header thông tin chung
                        worksheet.Cells[$"C{currentRow + 3}"].Value = dtTable.Rows[0]["TenKH"].ToString();
                        worksheet.Cells[$"C{currentRow + 4}"].Value = dtTable.Rows[0]["TenHang"].ToString();
                        worksheet.Cells[$"C{currentRow + 5}"].Value = dtTable.Rows[0]["DotSX"].ToString();
                        worksheet.Cells[$"C{currentRow + 6}"].Value = dtTable.Rows[0]["TenDVSX"].ToString();
                        worksheet.Cells[$"C{currentRow + 7}"].Value = dtTable.Rows[0]["TenCL"].ToString();
                        worksheet.Cells[$"C{currentRow + 8}"].Value = dtTable.Rows[0]["BookingMaHang"].ToString();
                        //worksheet.Cells[$"C{currentRow + 9}"].Value = txtGhiChu.Text.ToString();

                        var rangeGhiChu = worksheet.Cells[currentRow + 9, 1, currentRow + 9, 2];
                        rangeGhiChu.Merge = true;
                        rangeGhiChu.Value = "Ghi Chú";
                        rangeGhiChu.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rangeGhiChu.Style.Font.Bold = true;
                        rangeGhiChu.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        rangeGhiChu.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rangeGhiChu.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        rangeGhiChu.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[currentRow + 9, 3].Value = txtGhiChu.Text.ToString();
                        worksheet.Cells[currentRow + 9, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[currentRow + 9, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[currentRow + 9, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[currentRow + 9, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[currentRow + 9, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        // Tạo header cho phần LSX
                        currentRow = 11; // Bắt đầu từ dòng 11

                        List<string> columnNamesWithSize = tblLSX.Columns.Cast<DataColumn>()
                           .Where(column => column.ColumnName.Contains("@"))
                           .Select(column => column.ColumnName)
                           .Distinct()
                           .ToList();

                        int colums = 7;
                        range = worksheet.Cells[currentRow, colums, currentRow, colums + columnNamesWithSize.Count - 1];
                        range.Value = "Size"; range.Merge = true; range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;

                        foreach (var item in columnNamesWithSize)
                        {
                            string[] _split = item.Split('@');
                            string _size = _split[2].ToString();
                            worksheet.Cells[currentRow + 1, colums].Value = _size;
                            worksheet.Cells[currentRow + 1, colums].Style.Font.Bold = true;
                            colums++;
                        }

                        range = worksheet.Cells[currentRow, colums, currentRow + 1, colums];
                        range.Value = "Grand Total"; range.Style.WrapText = true; range.Merge = true;
                        range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;

                        currentRow += 2; // Dòng bắt đầu dữ liệu LSX
                        int startDataRowLSX = currentRow;

                        foreach (DataRow item in tblLSX.Rows)
                        {
                            worksheet.Cells[currentRow, 1].Value = item["CodeMau"].ToString(); worksheet.Cells[currentRow, 1].Style.WrapText = true;
                            worksheet.Cells[currentRow, 2].Value = item["TenMau"].ToString(); worksheet.Cells[currentRow, 2].Style.WrapText = true;
                            worksheet.Cells[currentRow, 3].Value = item["PO"].ToString();
                            worksheet.Cells[currentRow, 4].Value = item["DauSize"].ToString();
                            worksheet.Cells[currentRow, 5].Value = item["TenQG"].ToString();
                            worksheet.Cells[currentRow, 6].Value = Convert.ToDateTime(item["NgayGH"].ToString()).ToString("dd/MM/yyyy");
                            int sumSize = 0;
                            colums = 7;
                            foreach (var lstSize in columnNamesWithSize)
                            {
                                int sl = string.IsNullOrEmpty(item[lstSize].ToString()) ? 0 : Convert.ToInt32(item[lstSize]);
                                if (sl == 0)
                                {
                                    worksheet.Cells[currentRow, colums].Value = "";
                                }
                                else 
                                {
                                    worksheet.Cells[currentRow, colums].Value = sl;
                                }

                                worksheet.Cells[currentRow, colums].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
                                //sumSize += Convert.ToInt32(item[lstSize]);
                                sumSize += sl;
                                colums++;
                            }
                            worksheet.Cells[currentRow, colums].Value = sumSize;
                            currentRow++;
                        }

                        // Grand Total cho LSX
                        range = worksheet.Cells[currentRow, 1]; range.Value = "Grand Total"; range.Style.Font.Color.SetColor(Color.Red);
                        for (int i = 7; i < colums + 1; i++)
                        {
                            worksheet.Cells[currentRow, i].Formula = "=SUM(" + worksheet.Cells[startDataRowLSX, i].Address + ":" + worksheet.Cells[currentRow - 1, i].Address + ")";
                            worksheet.Cells[currentRow, i].Style.Font.Bold = true;
                        }

                        range = worksheet.Cells[currentRow, 1, currentRow, colums];
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 150, 148));

                        var borderDataLSX = worksheet.Cells[11, 1, currentRow, colums].Style.Border;
                        borderDataLSX.Bottom.Style = borderDataLSX.Top.Style = borderDataLSX.Left.Style = borderDataLSX.Right.Style = ExcelBorderStyle.Thin;

                        currentRow += 3; // Khoảng cách giữa LSX và NPL
                    }

                    // ==================== PHẦN NPL (Ở DƯỚI) ====================
                    if (tblNPL != null && tblNPL.Rows.Count > 0)
                    {
                        // Title cho phần NPL
                        range = worksheet.Cells[currentRow, 1, currentRow, 15];
                        range.Value = "DANH SÁCH NGUYÊN PHỤ LIỆU"; range.Merge = true;
                        range.Style.Font.Bold = true; range.Style.Font.Size = 14;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        currentRow += 2;
                        worksheet.Cells[currentRow, 1].Value = "STT";
                        worksheet.Cells[currentRow, 2].Value = "LOẠI VẬT TƯ";
                        worksheet.Cells[currentRow, 3].Value = "MÃ VẬT TƯ";
                        //worksheet.Cells[currentRow, 4].Value = "";
                        range = worksheet.Cells[currentRow, 4, currentRow, 5];
                        range.Value = "MÔ TẢ VẬT TƯ"; range.Merge = true; range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 6].Value = "MÃ MÀU VẬT TƯ";
                        worksheet.Cells[currentRow, 7].Value = "MÀU VẬT TƯ";
                        worksheet.Cells[currentRow, 8].Value = "SIZE/KHỔ";
                        worksheet.Cells[currentRow, 9].Value = "ĐVT";
                        worksheet.Cells[currentRow, 10].Value = "Số Lượng";
                        worksheet.Cells[currentRow, 11].Value = "DMSX";
                        worksheet.Cells[currentRow, 12].Value = "HAO HỤT";
                        worksheet.Cells[currentRow, 13].Value = "CẤP PHÁT";
                        //worksheet.Cells[currentRow, 14].Value = "THỰC NHẬN";
                        worksheet.Cells[currentRow, 14].Value = "GHI CHÚ PKT";
                        worksheet.Cells[currentRow, 15].Value = "GHI CHÚ PKH";

                        // Style header NPL 
                        range = worksheet.Cells[currentRow, 1, currentRow, 16];
                        range.Style.Font.Bold = true;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        currentRow++;
                        int startDataRowNPL = currentRow;
                        int index = 1;
                        decimal sumSL_CapPhap = 0;

                        // Dữ liệu NPL
                        foreach (DataRow item in tblNPL.Rows)
                        {
                            sumSL_CapPhap += Convert.ToDecimal(item["CapPhat"]);
                            worksheet.Cells[currentRow, 1].Value = index;
                        
                            worksheet.Cells[currentRow, 2].Value = item["TenNhom"].ToString();
                            worksheet.Cells[currentRow, 3].Value = item["MaVT"].ToString();
                            range = worksheet.Cells[currentRow, 4, currentRow, 5];
                            range.Value = item["TenVT"].ToString(); range.Merge = true; range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[currentRow, 6].Value = item["MaMauVT"].ToString();
                            worksheet.Cells[currentRow, 7].Value = item["MaMau"].ToString();
                            worksheet.Cells[currentRow, 8].Value = item["KhoVai"].ToString();
                            worksheet.Cells[currentRow, 9].Value = item["MaDV"].ToString();
                            worksheet.Cells[currentRow, 10].Value = item["SoLuong"].ToString();
                            //worksheet.Cells[currentRow, 11].Value = Convert.ToDecimal(item["DinhMuc"]);
                            //worksheet.Cells[currentRow, 12].Value = Convert.ToDecimal(item["DinhMucHaoHut"]);
                            //worksheet.Cells[currentRow, 13].Value = Convert.ToDecimal(item["CapPhat"]);
                            //worksheet.Cells[currentRow, 14].Value = Convert.ToDecimal(item["ThucNhan"]);

                            // worksheet.Cells[currentRow, 11].Value = Convert.ToDecimal(item["DinhMuc"]);
                            var valuecdm = Convert.ToDecimal(item["DinhMuc"]); // double

                            if (valuecdm % 1 == 0)
                            {
                                worksheet.Cells[currentRow,11].Value = Convert.ToInt64(valuecdm);
                            }
                            else
                            {
                                worksheet.Cells[currentRow, 11].Value = valuecdm;
                                worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "0.####";
                            }


                             //   worksheet.Cells[currentRow, 12].Value = Convert.ToDecimal(item["DinhMucHaoHut"]);

                            var valuechh = Convert.ToDecimal(item["DinhMucHaoHut"]); // double

                            if (valuechh % 1 == 0)
                            {
                                worksheet.Cells[currentRow, 12].Value = Convert.ToInt64(valuechh);
                            }
                            else
                            {
                                worksheet.Cells[currentRow, 12].Value = valuechh;
                                worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "0.####";
                            }


                            //worksheet.Cells[currentRow, 13].Value = Convert.ToDecimal(item["CapPhat"]);
                            // worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "0;-0;0.####";
                            var valuecp = Convert.ToDecimal(item["CapPhat"]); // double

                            if (valuecp % 1 == 0)
                            {
                                worksheet.Cells[currentRow, 13].Value = Convert.ToInt64(valuecp);
                            }
                            else
                            {
                                worksheet.Cells[currentRow, 13].Value = valuecp;
                                worksheet.Cells[currentRow, 13].Style.Numberformat.Format = "0.##";
                            }
                            //worksheet.Cells[currentRow, 14].Value = Convert.ToDecimal(item["ThucNhan"]);

                            //var valuectn = Convert.ToDecimal(item["ThucNhan"]); // double

                            //if (valuectn % 1 == 0)
                            //{
                            //    worksheet.Cells[currentRow, 14].Value = Convert.ToInt64(valuectn);
                            //}
                            //else
                            //{
                            //    worksheet.Cells[currentRow, 14].Value = valuectn;
                            //    worksheet.Cells[currentRow, 14].Style.Numberformat.Format = "0.##";
                            //}
                            worksheet.Cells[currentRow, 14].Value = item["GhiChuPKT"]?.ToString()?.Trim() ?? "";
                            worksheet.Cells[currentRow, 15].Value = item["GhiChu"]?.ToString()?.Trim() ?? "";//GhiChuPKH
                            //worksheet.Column(3).Width = 70; worksheet.Column(3).Style.WrapText = true;
                            worksheet.Column(4).AutoFit(); worksheet.Column(6).AutoFit();
                            worksheet.Row(currentRow).Height = 18;
                            index++;
                            currentRow++;
                        }
                        //worksheet.Cells.AutoFitColumns();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).Width = 20;
                        worksheet.Column(6).Width = 13;
                        worksheet.Column(7).Width = 13;
                        worksheet.Column(8).Width = 13;
                        worksheet.Column(9).Width = 13;
                        worksheet.Column(10).Width = 12;
                        worksheet.Column(11).Width = 12;
                        worksheet.Column(12).Width = 13;
                        worksheet.Column(13).Width = 13;
                        worksheet.Column(14).Width = 13;
                        worksheet.Column(15).Width = 13;
                        // worksheet.Column(15).Width = 20;
                        // worksheet.Column(16).Width = 20;
                        // Border cho phần NPL
                        ExcelRange borderRangeNPL = worksheet.Cells[startDataRowNPL - 1, 1, currentRow - 1, 15];
                        borderRangeNPL.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        borderRangeNPL.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        borderRangeNPL.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        borderRangeNPL.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        borderRangeNPL.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        borderRangeNPL.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        borderRangeNPL.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        borderRangeNPL.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                        // Tổng cộng cho NPL
                        currentRow++;
                        //range = worksheet.Cells[currentRow, 3]; range.Value = "Tổng cộng"; range.Style.Font.Bold = true;
                        //range = worksheet.Cells[currentRow, 12]; range.Value = sumSL_CapPhap; range.Style.Font.Bold = true;
                    }

                    // Lưu file
                    FileInfo resultFile = new FileInfo(ExportFileName);
                    templatePackage.SaveAs(resultFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xuất Excel: " + ex.Message);
            }
        }


        #region 07-11-2025 Sửa Cân Đối SX - Luân
        private void RemoveColumnSize(BandedGridView BandedGridView_Detail)
        {
            for (int i = 0; i < BandedGridView_Detail.Columns.Count;)
            {
                if (BandedGridView_Detail.Columns[i].FieldName.Contains("@Size@"))
                {
                    BandedGridView_Detail.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
        }

        private void ClearBand(GridBand GridBandSize, BandedGridView BandedGridView_Detail)
        {
            GridBandSize.Children.Clear();
            RemoveColumnSize(BandedGridView_Detail);
        }

        private bool CheckExistBand(string size, GridBand GridBandSize)
        {
            GridBand gbCheck = GridBandSize.Children.Where(x => x.Name == "gb" + "Size@" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        private void CreateBandGridSize_Detail(DataTable dt, BandedGridView BandedGridView_Detail, GridBand GridBandSize)
        {
            ClearBand(GridBandSize, BandedGridView_Detail);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                string[] parts = colName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                var _sizeID = parts[0];
                var _size = parts[1];
                if (!CheckExistBand(_sizeID, GridBandSize)) continue;

                var rItemSpinEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
                {
                    IsFloatValue = false,
                    MinValue = 0,
                    MaxValue = int.MaxValue,
                    Increment = 1,
                    AllowNullInput = DevExpress.Utils.DefaultBoolean.True,
                    DisplayFormat = { FormatType = DevExpress.Utils.FormatType.Numeric, FormatString = "n0" },
                    EditFormat = { FormatType = DevExpress.Utils.FormatType.Numeric, FormatString = "n0" }
                };




                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = $"{_sizeID}@Size@{_size}";
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.ReadOnly = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                col.ColumnEdit = rItemSpinEdit;

                //rItemSpinEdit.KeyPress += rItemTextEdit_KeyPress;

                GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                itemSize.FieldName = col.FieldName;
                itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                itemSize.DisplayFormat = "{0:n0}";
                itemSize.ShowInGroupColumnFooter = col;
                BandedGridView_Detail.GroupSummary.Add(itemSize);

                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
                BandedGridView_Detail.Columns.AddRange(new BandedGridColumn[] { col });
                GridBand gb = new GridBand();
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
                GridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }

        }
        private void BandedGridView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (!(sender is BandedGridView view) || e.Band == null)
                return;


            Rectangle rect = new Rectangle(e.Bounds.Location, e.Bounds.Size);


            if (rect.Width <= 2 || rect.Height <= 2)
                return;

            try
            {

                ControlPaint.DrawBorder3D(e.Graphics, rect);


                rect.Inflate(-1, -1);


                if (rect.Width > 0 && rect.Height > 0)
                {
                    Color backColor = ColorTranslator.FromHtml("#FFD480");

                    using (SolidBrush solidBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillRectangle(solidBrush, rect);
                    }
                }


                Font baseFont = e.Appearance.Font ?? Control.DefaultFont;

                if (!string.IsNullOrEmpty(e.Info.Caption) &&
                 e.Info.CaptionRect.Width > 0 && e.Info.CaptionRect.Height > 0)
                {
                    using (Font boldFont = new Font(baseFont, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };

                        e.Graphics.DrawString(e.Info.Caption, boldFont, textBrush, e.Info.CaptionRect, format);
                    }
                }


                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }

                e.Handled = true;
            }
            catch (ArgumentException ex)
            {

            }
        }

        private void GridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;
            GridColumn groupColumn = info.Column;
            int groupLevel = view.GetRowLevel(e.RowHandle);

            if (groupColumn == colMaLenh)
            {
                info.GroupText = $"Mã Lệnh : {info.GroupValueText}";
            }

            if (view.IsGroupRow(e.RowHandle))
            {

                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel + 1];
                }

                e.Appearance.ForeColor = textColor;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void BandedView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DauSize")
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "0";
                }

            }
            else if (e.Column.FieldName.Contains("@"))
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "-";
                }

            }
        }

        private void BandGridView_Detail_CustomDrawRowFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;


            int groupLevel = view.GetRowLevel(e.RowHandle);

            Color backColor = Color.FromArgb(255, 239, 204);


            if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color levelColor))
            {
                backColor = levelColor;
            }


            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, backColor, backColor, 90))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }


            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void BandGridView_Detail_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);


            if (groupLevelColors != null && groupLevelColors.TryGetValue(groupLevel, out Color groupColor))
            {
                e.Appearance.ForeColor = groupColor;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //e.Handled = true;
            }
            else if (groupLevelColorBackground != null && groupLevelColorBackground.TryGetValue(groupLevel, out Color fallbackColor))
            {
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, fallbackColor, fallbackColor, 90))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }


                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);

                e.Handled = true;
            }


        }

        private void bandViewCanDoiLSX_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData)
                {
                    decimal tong = 0;
                    foreach (BandedGridColumn col in bandViewCanDoiLSX.Columns)
                    {

                        if (col.FieldName != null && col.FieldName.Contains("@Size@"))
                        {
                            object cellValue = bandViewCanDoiLSX.GetRowCellValue(e.ListSourceRowIndex, col);
                            if (cellValue != null && decimal.TryParse(cellValue.ToString(), out decimal val))
                            {
                                tong += val;
                            }
                        }
                    }


                    e.Value = tong;
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void gridView2_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            // kiểm tra nhập liệu

            int num = 0;

            // in 
            if (bandViewCanDoiLSX.FocusedColumn.FieldName.Contains("@"))
            {
                DataTable tblCopyCD = grcCanDoiLSX.DataSource as DataTable;
                string MaLenh = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colMaLenh_cd).ToString();
                string Line = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colLine).ToString();
                string _madhFocus = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colMaDH_1).ToString();
                string _poid = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colPOID_1).ToString();
                string _mamau = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colColorID_1).ToString();
                string _dausizeid = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colSizeTypeID_1).ToString();
                string _filename = bandViewCanDoiLSX.FocusedColumn.FieldName.ToString();
                string urlKT = string.Format("{0}?madh={1}&&malenh={2}&&poid={3}&&mamau={4}&&dausizeid={5}", URL + "CanDoiDonHangTong/GetCheckEdiCanDoiSX", _madhFocus, _malenh, _poid, _mamau, _dausizeid);
                string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
                DataTable tblKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                if (!tblKT.Columns.Contains(_filename))
                {

                    e.ErrorText = $"Size không tồn tại trong đơn hàng " + _madhFocus + ".";
                    e.Value = 0;
                    return;
                }
                else
                {
                    if (tblKT == null || tblKT.Rows.Count == 0) return;
                  
                    string filterExpression =
                        "POID = '" + _poid + "' AND MaMau = '" + _mamau + "' AND DauSizeID = '" + _dausizeid + "'";

                    DataRow[] filteredRows = tblKT.Select(filterExpression);
                    // " AND MaLenh <> '" + MaLenh +
            
                   DataRow[] filteredRowsOther = tblCopyCD.Select(filterExpression + " AND Line <> '" + Line + "'");

                    DataRow foundRow = filteredRows[0];

                    int SL_KH = Convert.ToInt32(foundRow[_filename]);
                    int SLKhac = 0;

                    for (int i = 0; i < filteredRowsOther.Length; i++)
                    {
                        string val = filteredRowsOther[i][_filename] == null ? null : filteredRowsOther[i][_filename].ToString();
                        int sl;
                        if (!string.IsNullOrEmpty(val) && Int32.TryParse(val, out sl))
                        {
                            SLKhac += sl;
                        }
                    }

                    if (Convert.ToInt32(e.Value) + SLKhac > SL_KH)
                    {
                        string[] parts = _filename.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                        string po = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colPO_1).ToString();
                        string mau = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colTenMau_1).ToString();
                        string dausize = bandViewCanDoiLSX.GetRowCellValue(bandViewCanDoiLSX.FocusedRowHandle, colSizeType_1).ToString();

                        e.Valid = false;
                        e.ErrorText = $"Tổng số lượng Size {parts[1]} - PO: {po} - Inseam {dausize} - Màu: {mau} không vượt quá SLKH :" + SL_KH;
                    }

                    if (Convert.ToInt32(e.Value) < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Số lượng phải lớn hơn 0";
                    }
                }

            }


        }
        private void gridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            ColumnView view = sender as ColumnView;
            if (view == null) return;
            e.ExceptionMode = ExceptionMode.DisplayError;
            e.WindowCaption = "Cảnh Báo";
            view.HideEditor();


        }


        private void Luu()
        {
            try
            {
                this.ActiveControl = txtDonHang;
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                this.ActiveControl = this.button1;

                DataRow rowCongDoan = (gridViewCongDoan.DataSource as DataView).Table.AsEnumerable().FirstOrDefault();
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
                            gridViewCongDoan.SetFocusedRowCellValue(gridColumnIsCheckHoanThanh, null);
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

                //if (_npl == 0 && (tblCanDoiNPL != null || tblCanDoiNPL.Rows.Count == 0))
                //{
                //    MessageBox.Show("Vui lòng cân đối định mức nguyên phụ liệu trước khi lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
                string macAddress = "";
                string ipAddress = "";

                // Lấy MAC + IP từ card mạng đang hoạt động (không loopback, không ảo)
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                 ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                    .OrderByDescending(ni => ni.Speed)
                    .FirstOrDefault();

                if (networkInterface != null)
                {

                    PhysicalAddress pa = networkInterface.GetPhysicalAddress();
                    macAddress = string.Join("-", pa.GetAddressBytes().Select(b => b.ToString("X2")));


                    var ipProps = networkInterface.GetIPProperties();
                    var ipv4 = ipProps.UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

                    if (ipv4 != null)
                    {
                        ipAddress = ipv4.Address.ToString();
                    }
                }
              
                ListCanDoiSX = new List<EditCanDoiLenhSanXuatEntity>();
                DataTable tblCanDoi = grcCanDoiLSX.DataSource as DataTable;
                if (tblCanDoi?.Rows?.Count > 0)
                {

                    foreach (DataRow row in tblCanDoi.Rows)
                    {
                        foreach (DataColumn col in tblCanDoi.Columns)
                        {
                            if (col.ColumnName.Contains("@Size@"))
                            {
                                int Old_SL = 0;
                                string[] arraySize = col.ColumnName.Split(new string[] { "@Size@" }, StringSplitOptions.None);
                                if (Int32.TryParse(row[col.ColumnName]?.ToString(), out int SoLuong))
                                {
                                    if (SoLuong < 0) continue;
                                    var Query = _tblCanDoiLenhSX_BeforeEdit.AsEnumerable().
                                        FirstOrDefault(x => x["MaLenhSanXuat"]?.ToString() == row["MaLenhSanXuat"]?.ToString() && x["MaLenh"]?.ToString() == row["MaLenh"]?.ToString() && x["MaDH"]?.ToString() == row["MaDH"]?.ToString()
                                        && x["POID"]?.ToString() == row["POID"]?.ToString() && x["MaMau"]?.ToString() == row["MaMau"]?.ToString() && x["DauSizeID"]?.ToString() == row["DauSizeID"]?.ToString()
                                         && x["MaGop"]?.ToString() == row["MaGop"]?.ToString() && x["Line"]?.ToString() == row["Line"]?.ToString()
                                    );
                                    if (Query != null)
                                    {
                                        Int32.TryParse(Query[col.ColumnName]?.ToString(), out Old_SL);
                                    }


                                    EditCanDoiLenhSanXuatEntity objSaveCanDoiSX = new EditCanDoiLenhSanXuatEntity();
                                    objSaveCanDoiSX.MaLenhSanXuat = row["MaLenhSanXuat"]?.ToString();
                                    objSaveCanDoiSX.MaLenh = row["MaLenh"]?.ToString();
                                    objSaveCanDoiSX.TenLenh = row["TenLenh"]?.ToString();
                                    objSaveCanDoiSX.MaDH = row["MaDH"]?.ToString();
                                    objSaveCanDoiSX.POID = row["POID"]?.ToString();
                                    objSaveCanDoiSX.PO = row["PO"]?.ToString();
                                    objSaveCanDoiSX.MaMau = row["MaMau"]?.ToString();
                                    objSaveCanDoiSX.DauSizeID = row["DauSizeID"]?.ToString();
                                    objSaveCanDoiSX.DauSize = row["DauSize"]?.ToString();
                                    objSaveCanDoiSX.Size = arraySize[1];
                                    objSaveCanDoiSX.SizeID = arraySize[0];
                                    objSaveCanDoiSX.SoLuong = SoLuong;
                                    objSaveCanDoiSX.Old_SL = Old_SL;
                                    objSaveCanDoiSX.MaGop = row["MaGop"]?.ToString();
                                    objSaveCanDoiSX.Line = row["Line"]?.ToString();
                                    objSaveCanDoiSX.SawDep = row["SawDep"]?.ToString();
                                    objSaveCanDoiSX.UserName = GlobleData.UserName;
                                    objSaveCanDoiSX.CreaDate = DateTime.Now;
                                    objSaveCanDoiSX.Mac = macAddress;
                                    objSaveCanDoiSX.IPAdress = ipAddress;
                                    objSaveCanDoiSX.MachineName = Environment.MachineName;
                                    objSaveCanDoiSX.GhiChu = txtGhiChu.Text.ToString().Trim();
                                    ListCanDoiSX.Add(objSaveCanDoiSX);
                                }

                            }

                        }
                    }

                

                }

                //List<DinhMucSaveEntity> Listcandoinpl = new List<DinhMucSaveEntity>();
                //if (tblCanDoiNPL != null || tblCanDoiNPL.Rows.Count > 0)
                //{
                //    for (int i = 0; i <= tblCanDoiNPL.Rows.Count - 1; i++)
                //    {
                //        DinhMucSaveEntity addcandoinpl = new DinhMucSaveEntity();
                //        addcandoinpl.MaDH = _magop;
                //        addcandoinpl.MaLenhSanXuat = _malenhsanxuat;
                //        addcandoinpl.MaNPL = tblCanDoiNPL.Rows[i][2].ToString();
                //        addcandoinpl.MaVT = tblCanDoiNPL.Rows[i][3].ToString();
                //        addcandoinpl.TenVT = tblCanDoiNPL.Rows[i][4].ToString();
                //        addcandoinpl.MaMau = tblCanDoiNPL.Rows[i][5].ToString();
                //        addcandoinpl.KhoVai = tblCanDoiNPL.Rows[i][7].ToString();
                //        addcandoinpl.MaDV = tblCanDoiNPL.Rows[i][8].ToString();
                //        addcandoinpl.DinhMuc = tblCanDoiNPL.Rows[i][10].ToString() == "" ? 0 : Convert.ToDouble(tblCanDoiNPL.Rows[i][10]);
                //        addcandoinpl.SoLuong = tblCanDoiNPL.Rows[i][11].ToString() == "" ? 0 : Convert.ToInt32(tblCanDoiNPL.Rows[i][11]);
                //        addcandoinpl.CapPhat = tblCanDoiNPL.Rows[i][12].ToString() == "" ? 0 : Convert.ToDouble(tblCanDoiNPL.Rows[i][12]);
                //        addcandoinpl.CapThem = tblCanDoiNPL.Rows[i][13].ToString() == "" ? 0 : Convert.ToDouble(tblCanDoiNPL.Rows[i][13]);
                //        addcandoinpl.ThuHoi = tblCanDoiNPL.Rows[i][14].ToString() == "" ? 0 : Convert.ToDouble(tblCanDoiNPL.Rows[i][14]);
                //        addcandoinpl.TrangThai = Convert.ToInt32(tblCanDoiNPL.Rows[i][15]);
                //        if (Convert.ToInt32(tblCanDoiNPL.Rows[i][15]) == 1)
                //            addcandoinpl.NguoiSua = GlobleData.UserName;
                //        addcandoinpl.GhiChu = tblCanDoiNPL.Rows[i][16].ToString();
                //        addcandoinpl.MaMauLenh = tblCanDoiNPL.Rows[i][20].ToString();
                //        addcandoinpl.DauSizeLenh = tblCanDoiNPL.Rows[i][21].ToString();
                //        addcandoinpl.SizeLenh = tblCanDoiNPL.Rows[i][22].ToString();
                //        addcandoinpl.MaBom = tblCanDoiNPL.Rows[i][19].ToString();
                //        Listcandoinpl.Add(addcandoinpl);
                //    }
                //}
                if (_isDvsx == 1)
                {
                    if (_isKeoVe == 1)
                    {
                        XtraMessageBox.Show("Lệnh này đã được thực hiện sản xuất.\nKhông thể sửa đơn vị sản xuất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        searchLookUpEditDVSX.EditValue = _madvsx;
                        _isDvsx = 0;
                        return;
                    }
                    List<CanDoiDonViSanXuatSaveEntity> ListCanDoiDVSX = new List<CanDoiDonViSanXuatSaveEntity>();
                    CanDoiDonViSanXuatSaveEntity _editCanDoiSX = new CanDoiDonViSanXuatSaveEntity();
                    _editCanDoiSX.MaLenhSanXuat = _malenhsanxuat;
                    _editCanDoiSX.MaLenh = _malenh;
                    _editCanDoiSX.DotSX = _dot;
                    _editCanDoiSX.MaDH = _magop;
                    _editCanDoiSX.MaDVSX = searchLookUpEditDVSX.EditValue.ToString() == "" ? _madvsx : searchLookUpEditDVSX.EditValue.ToString();
                    ListCanDoiDVSX.Add(_editCanDoiSX);
                    string urlPDVSX = string.Format("{0}?", URL + "CanDoiDonHangTong/PostUDDVSX");
                    string msPDVSX = Task.Run(async () => { return await _clientExtension.PostAsync(urlPDVSX, ListCanDoiDVSX); }).Result;
                    if (msPDVSX.ToLower() != "true")
                        XtraMessageBox.Show(msPDVSX);
                }
                if (ListCanDoiSX.Count > 0)
                {
                    string urlPDVSX = string.Format("{0}?", URL + "CanDoiDonHangTong/PostUpdateDVSX");
                    string msPDVSX = Task.Run(async () => { return await _clientExtension.PostAsync(urlPDVSX, ListCanDoiSX); }).Result;
                    if (msPDVSX.ToLower() != "true")
                        XtraMessageBox.Show(msPDVSX);
                }
                //if (Listcandoinpl.Count > 0 && _npl == 1)
                //{
                //    string urlPNPL = string.Format("{0}?", URL + "CanDoiDonHangTong/PostUpdateNPL");
                //    string msPNPL = Task.Run(async () => { return await _clientExtension.PostAsync(urlPNPL, Listcandoinpl); }).Result;
                //    if (msPNPL.ToLower() != "true")
                //        XtraMessageBox.Show(msPNPL);

                //}
                string result = LuuChiTietCongDoan();

                if (result.ToLower().Equals("true"))
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                this.Close();
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
            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            Luu();
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
        }
        #endregion

    }
}
