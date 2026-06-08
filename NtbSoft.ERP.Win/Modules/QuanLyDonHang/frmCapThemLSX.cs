using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCapThemLSX : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        int _rowhandle = 0, _rowhandles = 0;
        DataTable tblDM = new DataTable();
        string _magop = string.Empty, _nguoitao = string.Empty, _maLSX = string.Empty, _tenKH = string.Empty, _mahang = string.Empty, _madh = string.Empty, _malenh = string.Empty, _soluong = string.Empty, _tenhang = string.Empty, _dot = string.Empty, _makh = string.Empty, _madhgop = string.Empty, _madot = string.Empty;
        public frmCapThemLSX(string _magop, string _nguoitao, string _maLSX, string _tenKH, string _mahang, string _madh, string _malenh, string _soluong, string _tenhang, int _iskeove, string _dot, string _makh, string _madhgop, string madot)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._magop = _magop;
            this._nguoitao = _nguoitao;
            this._maLSX = _maLSX;
            this._tenKH = _tenKH;
            this._mahang = _mahang;
            this._madh = _madh;
            this._malenh = _malenh;
            this._soluong = _soluong;
            this._tenhang = _tenhang;
            this._dot = _dot;
            this._makh = _makh;
            this._madhgop = _madhgop;
            this._madot = madot;
        }

        protected override void OnLoad(EventArgs e)
        {
            txtLenhSX.Text = _malenh;
            txtDH.Text = _madhgop;
            txtKH.Text = _tenKH;
            txtMH.Text = _mahang;
            InItChungLoaiChiTiet();
            CreateSearchLookUpDot();
            LoaDM();
            loadCapPhat();
            this.ActiveControl = button1;
        }

        private void LoaDM()
        {
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETDMTONG&para1={_makh.ToString()}&para2={_mahang.ToString()}&para3={madot.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblDM = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void loadCapPhat()
        {
            try
            {
                string url = string.Format("{0}?magop={1}&&malenhsanxuat={2}&&madot={3}&&makh={4}&&mahang={5}", URL + "DonHangTong/GetCapPhatCanDoiCT", _magop.ToString(),
                   _maLSX.ToString(), searchLookUpEditDot.EditValue ?? "", _makh.ToString(), _mahang.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    gridControl3.DataSource = null;
                    return;
                }
                DataTable tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                //foreach (DataRow rowData in tblChiTiet.Rows)
                //{
                //    if (rowData["Status"].ToString() == "0")
                //    {
                //        string _mavtid = rowData["MaVTID"].ToString();
                //        string _mauID = rowData["MauID"].ToString();
                //        // string urls = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}&&manhom={7}&&KhoVaiID={8}&&macode={9}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                //        //_mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString(), "True", rowData["MaNhom"].ToString(), rowData["MaKhoVT"].ToString(),rowData["MaCode"].ToString());
                //        // string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
                //        // DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(jsons);
                //        string maVTID = rowData["MaVTID"].ToString();
                //        string mauVTID = rowData["MauID"].ToString();
                //        string khoVaiID = rowData["MaKhoVT"].ToString();
                //        string maNhom = rowData["MaNhom"].ToString();
                //        string maCode = rowData["MaCode"].ToString();

                //        // Lọc các dòng trùng khớp từ tblDM
                //        var matchingRows = tblDM.AsEnumerable()
                //            .Where(row =>
                //                row["MaVTID"].ToString() == maVTID &&
                //                row["MauVTID"].ToString() == mauVTID &&
                //                row["KhoVaiID"].ToString() == khoVaiID &&
                //                row["MaNhom"].ToString() == maNhom &&
                //                row["MaCode"].ToString() == maCode
                //            );

                //        // Copy về một DataTable mới
                //        DataTable dtDM = matchingRows.Any() ? matchingRows.CopyToDataTable() : tblDM.Clone();

                //        string urldvsx = string.Format("{0}?magop={1}&&malenhsanxuat={2}", URL + "DonHangTong/GetChiTietLenhSX", _magop, _maLSX);
                //        string jsondvsx = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvsx); }).Result;
                //        if (jsondvsx == "[]")
                //        {
                //            gridControl3.DataSource = tblChiTiet;
                //            return;
                //        }
                //        DataTable dtSL = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
                //        int rowCount = 0;

                //        var mausp = rowData["MaMau"].ToString()
                //                 .Split('|')
                //                 .Select(m => m.Trim())
                //                 .ToList();
                //        var distinctDauSizeIds = dtDM.AsEnumerable()
                //                                     .Select(row => row["DauSizeID"].ToString())
                //                                     .Distinct()
                //                                     .ToList();

                //        var duLieuDM = dtDM.AsEnumerable()
                //            .ToDictionary(
                //                row => $"{row["DauSizeID"]}_{row["MaMau"]}",
                //                row => row
                //            );

                //        var sizeColumns = dtSL.Columns.Cast<DataColumn>()
                //                              .Where(c => c.ColumnName.Contains("@"))
                //                              .Select(c => c.ColumnName)
                //                              .ToList();
                //        decimal totalQuantity = 0;

                //        if (rowData["TachMau"].ToString() == "False")
                //        {
                //            foreach (DataRow rowSL in dtSL.Rows)
                //            {
                //                string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                //                string maMauSL = rowSL["MaMau"].ToString();
                //                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                //                {
                //                    string key = $"{dauSizeIdSL}_{rowData["MaMau"].ToString()}";
                //                    if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                //                    {
                //                        foreach (string columnName in sizeColumns)
                //                        {
                //                            if (dtDM.Columns.Contains(columnName))
                //                            {
                //                                var valueDM = rowDM[columnName];
                //                                if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                //                                {
                //                                    var valueSL = rowSL[columnName];
                //                                    if (valueSL != DBNull.Value)
                //                                    {
                //                                        totalQuantity += Convert.ToDecimal(valueSL);
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            foreach (DataRow rowSL in dtSL.Rows)
                //            {
                //                string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                //                string maMauSL = rowSL["MaMau"].ToString();
                //                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                //                {
                //                    string key = $"{dauSizeIdSL}_{maMauSL.ToString()}";
                //                    if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                //                    {
                //                        foreach (string columnName in sizeColumns)
                //                        {
                //                            if (dtDM.Columns.Contains(columnName))
                //                            {
                //                                var valueDM = rowDM[columnName];
                //                                if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                //                                {
                //                                    var valueSL = rowSL[columnName];
                //                                    if (valueSL != DBNull.Value)
                //                                    {
                //                                        totalQuantity += Convert.ToDecimal(valueSL);
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //        decimal totalAverage = TinhCapPhatTong(_mavtid, _mauID, rowData["MaMau"].ToString(), dtSL, rowData["TachMau"].ToString() == "" ? "False" : rowData["TachMau"].ToString(), dtDM);
                //        Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);
                //        if (rowData["DinhMucHaoHut"].ToString() != "" && rowData["DinhMucHaoHut"].ToString() != "0")
                //        {
                //            decimal _dmhh = (Convert.ToDecimal(rowData["DinhMucHaoHut"]) / 100) + 1;
                //            rowData["CapPhatTK"] = Math.Round(finalAverage * _dmhh * totalQuantity, 2);
                //            rowData["CapPhat"] = 0;// Math.Round(finalAverage * _dmhh * totalQuantity, 2);
                //        }
                //        else
                //        {
                //            rowData["CapPhatTK"] = Math.Round(totalAverage, 2);
                //            rowData["CapPhat"] = 0;//Math.Round(totalAverage, 2);
                //        }
                //        rowData["DinhMucChung"] = finalAverage;
                //        rowData["DinhMuc"] = finalAverage;
                //        rowData["SoLuong"] = totalQuantity;
                //        rowData["NhuCau"] = 0;
                //        rowData["TachMau"] = rowData["TachMau"];
                //        rowData["MaVTGhep"] = rowData["MaVTGhep"];
                //        rowData["TachMauSPDH"] = 0;
                //        rowData["MaCode"] = rowData["MaCode"];
                //        //rowData["CapPhatTK"] = totalAverage;
                //    }
                //}
                gridControl3.DataSource = tblChiTiet;
            }
            catch (Exception ex) { }

        }
        private decimal TinhCapPhatTong(string _mavtid, string _mauID, string mausanpham, DataTable dtSL, string tachmau, DataTable dtDM)
        {
            DataTable tbl = new DataTable();
            var mausp = mausanpham.ToString()
                          .Split('|')
                          .Select(m => m.Trim())
                          .ToList();
            var filteredRows = dtSL.AsEnumerable()
                .Where(row => mausp.Contains(row["MaMau"].ToString()));
            if (tachmau.ToString() == "False")
            {
                tbl.Columns.Add("DauSizeID", typeof(string));
                tbl.Columns.Add("DauSize", typeof(string));
                foreach (DataColumn column in dtSL.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        tbl.Columns.Add(column.ColumnName, typeof(float));
                    }
                }
                var validDauSizeIDs = new HashSet<string>(
                                 dtDM.AsEnumerable()
                                     .Select(row => row["DauSizeID"].ToString())
                                     .Distinct()
                             );

                foreach (DataRow rowSL in dtSL.Rows)
                {
                    string dauSizeID = rowSL["DauSizeID"].ToString();


                    // Chỉ thêm nếu DauSizeID có trong dtDM
                    if (validDauSizeIDs.Contains(dauSizeID))
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                }
                DataTable dtSL_Filtered = filteredRows.Any()
                    ? filteredRows.CopyToDataTable()
                    : dtSL.Clone();

                var distinctDauSizeIds = dtDM.AsEnumerable().Select(row => row["DauSizeID"].ToString())
                                                                  .Distinct()
                                                                  .ToList();
                var columnsWithAt = dtSL_Filtered.Columns.Cast<DataColumn>()
                                                .Where(col => col.ColumnName.Contains("@"))
                                                .ToList();
                foreach (DataRow rowDM in dtDM.Rows)
                {
                    foreach (var column in columnsWithAt)
                    {

                        string dauSizeIDDM = rowDM["DauSizeID"].ToString();
                        decimal totalQuantityInSL = dtSL_Filtered.AsEnumerable()
                                                           .Where(row => mausp.Contains(row["MaMau"]) &&
                                                                        row["DauSizeID"].ToString() == dauSizeIDDM.ToString())
                                                           .Sum(row =>
                                                               decimal.TryParse(row[column].ToString(), out decimal value)
                                                               ? Math.Round(value, 4)
                                                               : 0);
                        if (totalQuantityInSL > 0)
                        {
                            var rowDMs = dtDM.AsEnumerable()
                                            .FirstOrDefault(row => row["DauSizeID"].ToString() == dauSizeIDDM.ToString() && rowDM.Table.Columns.Contains(column.ColumnName));

                            if (rowDM != null && rowDM.Table.Columns.Contains(column.ColumnName))
                            {
                                decimal quantityInDM = Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4);
                                decimal totalQuantity = totalQuantityInSL * quantityInDM;
                                DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}'");

                                foreach (DataRow rowTbl in rowsInTbl)
                                {
                                    foreach (DataColumn col in rowTbl.Table.Columns)
                                    {
                                        if (col.ToString() == column.ToString())
                                        {

                                            rowTbl[column.ColumnName] = totalQuantity;
                                        }
                                    }

                                }
                            }
                        }
                    }

                }
            }
            else
            {
                tbl.Columns.Add("DauSizeID", typeof(string));
                tbl.Columns.Add("DauSize", typeof(string));
                tbl.Columns.Add("MaMau", typeof(string));
                foreach (DataColumn column in dtSL.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        tbl.Columns.Add(column.ColumnName, typeof(float));
                    }
                }
                var validDauSizeIDs = new HashSet<string>(
                                 dtDM.AsEnumerable()
                                     .Select(row => row["DauSizeID"].ToString())
                                     .Distinct()
                             );

                foreach (DataRow rowSL in dtSL.Rows)
                {
                    string dauSizeID = rowSL["DauSizeID"].ToString();
                    string maMau = rowSL["MaMau"].ToString();


                    // Chỉ thêm nếu DauSizeID có trong dtDM
                    if (validDauSizeIDs.Contains(dauSizeID))
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID && r["MAMau"].ToString() == maMau);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            newRow["MaMau"] = rowSL["MaMau"];
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                }
                foreach (var mamau in mausp)
                {
                    var filteredRowsByMau = dtSL.AsEnumerable()
                        .Where(row => row["MaMau"].ToString() == mamau)
                        .ToList();

                    DataTable dtSL_Filtered_ByMau = filteredRowsByMau.Any()
                        ? filteredRowsByMau.CopyToDataTable()
                        : dtSL.Clone();

                    var columnsWithAt = dtSL_Filtered_ByMau.Columns.Cast<DataColumn>()
                        .Where(col => col.ColumnName.Contains("@"))
                        .ToList();

                    foreach (DataRow rowDM in dtDM.AsEnumerable().Where(r => r["MaMau"].ToString() == mamau))
                    {
                        string dauSizeIDDM = rowDM["DauSizeID"].ToString();

                        foreach (var column in columnsWithAt)
                        {
                            decimal totalQuantityInSL = dtSL_Filtered_ByMau.AsEnumerable()
                                .Where(row => row["DauSizeID"].ToString() == dauSizeIDDM)
                                .Sum(row =>
                                    decimal.TryParse(row[column].ToString(), out decimal value)
                                        ? Math.Round(value, 4)
                                        : 0);

                            if (totalQuantityInSL > 0)
                            {
                                decimal quantityInDM = rowDM.Table.Columns.Contains(column.ColumnName)
                                    ? Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4)
                                    : 0;

                                decimal totalQuantity = totalQuantityInSL * quantityInDM;

                                DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}' AND MaMau = '{mamau}'");

                                foreach (DataRow rowTbl in rowsInTbl)
                                {
                                    if (rowTbl.Table.Columns.Contains(column.ColumnName))
                                    {
                                        rowTbl[column.ColumnName] = totalQuantity;
                                    }
                                }
                            }
                        }
                    }
                }

            }

            decimal sum = 0;
            foreach (DataColumn dc in tbl.Columns)
            {
                if (dc.ColumnName.Contains("@"))
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (decimal.TryParse(row[dc].ToString(), out decimal value))
                        {
                            sum += value;
                        }
                    }
                }
            }
            return sum;
        }
        private void CreateSearchLookUpDot()
        {
            searchLookUpEditDot.Properties.DisplayMember = "Dot";
            searchLookUpEditDot.Properties.ValueMember = "MaDot";

            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "DonHangTong/GetCapPhatThongSoDot", _makh.ToString(), _mahang.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDot.Properties.DataSource = tblDot;
            if (!string.IsNullOrEmpty(_madot))
            {
                searchLookUpEditDot.EditValue = _madot;
            }
            //searchLookUpEditDot.EditValue = tblDot.Rows[0][0];
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            LuuBangCT();
        }
        private void LuuBangCT()
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGrid = gridControl31.DataSource as DataTable;
                if (tblGrid == null || tblGrid.Rows.Count == 0)
                {
                    return;
                }
                DataTable _dtSave = CreateTableSaveCT(tblGrid);

                if (_dtSave == null || _dtSave.Rows.Count == 0)
                {
                    return;
                }
                _rowhandle = gridView1.FocusedRowHandle;
                string url = string.Format("{0}", URL + "DonHangTong/PostCapThemLSXSua");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    loadCapPhat();
                    gridView1.FocusedRowHandle = _rowhandle;
                }

            }
            catch (Exception ex)
            {

            }
        }
        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);
        }

        private void gridView2_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_makh.ToString() == "" && _mahang.ToString() == "") return;
            string madot = searchLookUpEditDot.EditValue.ToString() == "" ? "" : searchLookUpEditDot.EditValue.ToString();
            frmPhanTichBOMViewV1 frm = new frmPhanTichBOMViewV1(_makh.ToString(), _mahang.ToString(), madot.ToString());
            frm.ShowDialog();
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {

        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {

        }

        private void gridControl31_Click(object sender, EventArgs e)
        {

        }


        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuBang();
        }
        private DataTable CreateTableSave(DataTable tblGrid)
        {

            DataTable tblSave = new DataTable("tblSave");
            #region tạo cột 
            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("MaLenhSanXuat", typeof(string));
            tblSave.Columns.Add("MaNPL", typeof(string));
            tblSave.Columns.Add("MaVT", typeof(string));
            tblSave.Columns.Add("TenVT", typeof(string));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("KhoVai", typeof(string));
            tblSave.Columns.Add("MaDV", typeof(string));
            tblSave.Columns.Add("DinhMuc", typeof(float));
            tblSave.Columns.Add("SoLuong", typeof(int));
            tblSave.Columns.Add("CapPhat", typeof(float));
            tblSave.Columns.Add("CapThem", typeof(float));
            tblSave.Columns.Add("ThuHoi", typeof(float));
            tblSave.Columns.Add("TrangThai", typeof(int));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NguoiTao", typeof(string));
            tblSave.Columns.Add("NguoiSua", typeof(string));
            tblSave.Columns.Add("NgayTao", typeof(string));
            tblSave.Columns.Add("NgaySua", typeof(string));
            tblSave.Columns.Add("MaMauLenh", typeof(string));
            tblSave.Columns.Add("DauSizeLenh", typeof(string));
            tblSave.Columns.Add("SizeLenh", typeof(string));
            tblSave.Columns.Add("MaBom", typeof(string));
            tblSave.Columns.Add("IsXetDuyet", typeof(bool));
            tblSave.Columns.Add("NguoiXet", typeof(string));
            tblSave.Columns.Add("NguoiHuy", typeof(string));
            tblSave.Columns.Add("NgayXet", typeof(string));
            tblSave.Columns.Add("NgayHuy", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("QtyES", typeof(float));
            tblSave.Columns.Add("CostES", typeof(float));
            tblSave.Columns.Add("NeedES", typeof(float));
            tblSave.Columns.Add("ToTalRec", typeof(float));
            tblSave.Columns.Add("ID_MaNPL", typeof(string));
            tblSave.Columns.Add("ThucNhan", typeof(float));
            tblSave.Columns.Add("TenTAVT", typeof(string));
            tblSave.Columns.Add("DinhMucHaoHut", typeof(float));
            tblSave.Columns.Add("MaNhomVT", typeof(string));
            tblSave.Columns.Add("MaVTTheoCayVai", typeof(string));
            tblSave.Columns.Add("TenLoaiVT", typeof(string));
            tblSave.Columns.Add("MaMauVT", typeof(string));
            tblSave.Columns.Add("MaKhoVT", typeof(string));
            tblSave.Columns.Add("MaDVVT", typeof(string));
            tblSave.Columns.Add("MaVTID", typeof(string));
            tblSave.Columns.Add("MaDot", typeof(string));
            tblSave.Columns.Add("ThucXuat", typeof(decimal));
            tblSave.Columns.Add("MauSP", typeof(string));
            tblSave.Columns.Add("DinhMucChung", typeof(decimal));
            tblSave.Columns.Add("CapPhatTK", typeof(decimal));
            tblSave.Columns.Add("Dot", typeof(string));
            tblSave.Columns.Add("NhuCau", typeof(decimal));
            tblSave.Columns.Add("MaVTGhep", typeof(string));
            tblSave.Columns.Add("TachMauSPDH", typeof(int));
            tblSave.Columns.Add("MaCode", typeof(string));
            #endregion
            foreach (DataRow row in tblGrid.Rows)
            {
                if((row["MaCLCT"] as string ?? "") == "")
                {
                    XtraMessageBox.Show("Vui lòng chọn đầy đủ chủng loại chi tiết trước khi lưu!",
                         "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                if (row["CapThem"].ToString() != "0")
                {
                    DataRow dr = tblSave.NewRow();
                    dr["ID"] = row["ID"];
                    dr["MaDH"] = _magop;
                    dr["MaLenhSanXuat"] = _maLSX;
                    dr["MaNPL"] = row["MaNPL"].ToString();
                    dr["MaVT"] = row["MaVT"];
                    dr["TenVT"] = row["ChiTiet"];
                    dr["MaMau"] = row["MauVT"];
                    dr["KhoVai"] = row["KhoVai"];
                    dr["MaDV"] = row["TenDVVT"];
                    dr["DinhMuc"] = row["DinhMuc"];
                    dr["SoLuong"] = row["SoLuong"];
                    dr["CapPhat"] = row["CapPhat"];
                    dr["CapThem"] = row["CapThem"];
                    dr["ThuHoi"] = 0;
                    dr["TrangThai"] = 0;
                    dr["GhiChu"] = row["GhiChu"];
                    dr["NguoiTao"] = GlobleData.UserName;
                    dr["NguoiSua"] = 0;
                    dr["NgayTao"] = "";
                    dr["NgaySua"] = "";
                    dr["MaMauLenh"] = 0;
                    dr["DauSizeLenh"] = 0;
                    dr["SizeLenh"] = row["MaCLCT"];
                    dr["MaBom"] = 0;
                    dr["IsXetDuyet"] = false;
                    dr["NguoiXet"] = 0;
                    dr["NguoiHuy"] = 0;
                    dr["NgayXet"] = "";
                    dr["NgayHuy"] = "";
                    dr["MaVTMau"] = 0;
                    dr["QtyES"] = 0;
                    dr["CostES"] = 0;
                    dr["NeedES"] = 0;
                    dr["ToTalRec"] = 0;
                    dr["ID_MaNPL"] = 0;
                    dr["ThucNhan"] = 0;
                    dr["TenTAVT"] = 0;
                    dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                    dr["MaNhomVT"] = row["MaNhom"];
                    dr["MaVTTheoCayVai"] = 0;
                    dr["TenLoaiVT"] = row["TenNhom"];
                    dr["MaMauVT"] = row["MauID"];
                    dr["MaKhoVT"] = row["MaKhoVT"];
                    dr["MaDVVT"] = row["MaDVVT"];
                    dr["MaVTID"] = row["MaVTID"];
                    dr["MaDot"] = searchLookUpEditDot.EditValue.ToString();
                    dr["ThucXuat"] = row["ThucXuat"];
                    dr["MauSP"] = row["TenMau"];
                    dr["DinhMucChung"] = row["DinhMucChung"];
                    dr["CapPhatTK"] = row["CapPhatTK"];
                    dr["Dot"] = (Convert.ToInt32(row["Dot"]) + 1).ToString();
                    dr["NhuCau"] = row["NhuCau"];
                    dr["MaVTGhep"] = row["MaVTGhep"];
                    dr["TachMauSPDH"] = row["TachMauSPDH"];
                    dr["MaCode"] = row["MaCode"];
                    tblSave.Rows.Add(dr);
                }
            }
            return tblSave;
        }
        private DataTable CreateTableSaveCT(DataTable tblGrid)
        {

            DataTable tblSave = new DataTable("tblSave");
            #region tạo cột 
            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("MaLenhSanXuat", typeof(string));
            tblSave.Columns.Add("MaNPL", typeof(string));
            tblSave.Columns.Add("MaVT", typeof(string));
            tblSave.Columns.Add("TenVT", typeof(string));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("KhoVai", typeof(string));
            tblSave.Columns.Add("MaDV", typeof(string));
            tblSave.Columns.Add("DinhMuc", typeof(float));
            tblSave.Columns.Add("SoLuong", typeof(int));
            tblSave.Columns.Add("CapPhat", typeof(float));
            tblSave.Columns.Add("CapThem", typeof(float));
            tblSave.Columns.Add("ThuHoi", typeof(float));
            tblSave.Columns.Add("TrangThai", typeof(int));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NguoiTao", typeof(string));
            tblSave.Columns.Add("NguoiSua", typeof(string));
            tblSave.Columns.Add("NgayTao", typeof(string));
            tblSave.Columns.Add("NgaySua", typeof(string));
            tblSave.Columns.Add("MaMauLenh", typeof(string));
            tblSave.Columns.Add("DauSizeLenh", typeof(string));
            tblSave.Columns.Add("SizeLenh", typeof(string));
            tblSave.Columns.Add("MaBom", typeof(string));
            tblSave.Columns.Add("IsXetDuyet", typeof(bool));
            tblSave.Columns.Add("NguoiXet", typeof(string));
            tblSave.Columns.Add("NguoiHuy", typeof(string));
            tblSave.Columns.Add("NgayXet", typeof(string));
            tblSave.Columns.Add("NgayHuy", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("QtyES", typeof(float));
            tblSave.Columns.Add("CostES", typeof(float));
            tblSave.Columns.Add("NeedES", typeof(float));
            tblSave.Columns.Add("ToTalRec", typeof(float));
            tblSave.Columns.Add("ID_MaNPL", typeof(string));
            tblSave.Columns.Add("ThucNhan", typeof(float));
            tblSave.Columns.Add("TenTAVT", typeof(string));
            tblSave.Columns.Add("DinhMucHaoHut", typeof(float));
            tblSave.Columns.Add("MaNhomVT", typeof(string));
            tblSave.Columns.Add("MaVTTheoCayVai", typeof(string));
            tblSave.Columns.Add("TenLoaiVT", typeof(string));
            tblSave.Columns.Add("MaMauVT", typeof(string));
            tblSave.Columns.Add("MaKhoVT", typeof(string));
            tblSave.Columns.Add("MaDVVT", typeof(string));
            tblSave.Columns.Add("MaVTID", typeof(string));
            tblSave.Columns.Add("MaDot", typeof(string));
            tblSave.Columns.Add("ThucXuat", typeof(decimal));
            tblSave.Columns.Add("MauSP", typeof(string));
            tblSave.Columns.Add("DinhMucChung", typeof(decimal));
            tblSave.Columns.Add("CapPhatTK", typeof(decimal));
            tblSave.Columns.Add("Dot", typeof(string));
            tblSave.Columns.Add("NhuCau", typeof(decimal));
            tblSave.Columns.Add("MaVTGhep", typeof(string));
            tblSave.Columns.Add("TachMauSPDH", typeof(int));
            tblSave.Columns.Add("MaCode", typeof(string));
            #endregion
            foreach (DataRow row in tblGrid.Rows)
            {
                if (row["Sua"].ToString() == "1")
                {
                    DataRow dr = tblSave.NewRow();
                    dr["ID"] = row["ID"];
                    dr["MaDH"] = _magop;
                    dr["MaLenhSanXuat"] = _maLSX;
                    dr["MaNPL"] = row["MaNPL"].ToString();
                    dr["MaVT"] = row["MaVT"];
                    dr["TenVT"] = row["ChiTiet"];
                    dr["MaMau"] = row["MauVT"];
                    dr["KhoVai"] = row["KhoVai"];
                    dr["MaDV"] = row["TenDVVT"];
                    dr["DinhMuc"] = row["DinhMuc"];
                    dr["SoLuong"] = row["SoLuong"];
                    dr["CapPhat"] = row["CapPhat"];
                    dr["CapThem"] = row["CapThem"];
                    dr["ThuHoi"] = 0;
                    dr["TrangThai"] = 0;
                    dr["GhiChu"] = row["GhiChu"];
                    dr["NguoiTao"] = GlobleData.UserName;
                    dr["NguoiSua"] = 0;
                    dr["NgayTao"] = "";
                    dr["NgaySua"] = "";
                    dr["MaMauLenh"] = 0;
                    dr["DauSizeLenh"] = 0;
                    dr["SizeLenh"] = 0;
                    dr["MaBom"] = 0;
                    dr["IsXetDuyet"] = false;
                    dr["NguoiXet"] = 0;
                    dr["NguoiHuy"] = 0;
                    dr["NgayXet"] = "";
                    dr["NgayHuy"] = "";
                    dr["MaVTMau"] = 0;
                    dr["QtyES"] = 0;
                    dr["CostES"] = 0;
                    dr["NeedES"] = 0;
                    dr["ToTalRec"] = 0;
                    dr["ID_MaNPL"] = 0;
                    dr["ThucNhan"] = 0;
                    dr["TenTAVT"] = 0;
                    dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                    dr["MaNhomVT"] = row["MaNhom"];
                    dr["MaVTTheoCayVai"] = 0;
                    dr["TenLoaiVT"] = row["TenNhom"];
                    dr["MaMauVT"] = row["MauID"];
                    dr["MaKhoVT"] = row["MaKhoVT"];
                    dr["MaDVVT"] = row["MaDVVT"];
                    dr["MaVTID"] = row["MaVTID"];
                    dr["MaDot"] = searchLookUpEditDot.EditValue.ToString();
                    dr["ThucXuat"] = row["ThucXuat"];
                    dr["MauSP"] = row["TenMau"];
                    dr["DinhMucChung"] = row["DinhMucChung"];
                    dr["CapPhatTK"] = row["CapPhatTK"];
                    dr["Dot"] = row["Dot"];
                    dr["NhuCau"] = row["NhuCau"];
                    dr["MaVTGhep"] = row["MaVTGhep"];
                    dr["TachMauSPDH"] = row["TachMauSPDH"];
                    dr["MaCode"] = row["MaCode"];

                    tblSave.Rows.Add(dr);
                }
            }
            return tblSave;
        }
        private void LuuBang()
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGrid = gridControl3.DataSource as DataTable;
                if (tblGrid == null || tblGrid.Rows.Count == 0)
                {
                    return;
                }
                DataTable _dtSave = CreateTableSave(tblGrid);

                if (_dtSave == null || _dtSave.Rows.Count == 0)
                {
                    return;
                }
                _rowhandle = gridView1.FocusedRowHandle;

                string url = string.Format("{0}", URL + "DonHangTong/PostCapThemLSX");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    loadCapPhat();
                    gridView1.FocusedRowHandle = _rowhandle;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow focusedRow = gridView1.GetDataRow(e.FocusedRowHandle);

                if (focusedRow != null)
                {
                    string url = string.Format("{0}?magop={1}&&malenhsanxuat={2}&&madot={3}&&manpl={4}", URL + "DonHangTong/GetCapThemLichSu", _magop.ToString(),
                       _maLSX.ToString(), searchLookUpEditDot.EditValue ?? "", focusedRow["MaNPL"].ToString());
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json == "[]")
                    {
                        gridControl31.DataSource = null;
                        return;
                    }
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    gridControl31.DataSource = tbl;

                }
            }
            catch (Exception ex)
            {

            }

        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow focusedRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);

                if (focusedRow != null)
                {
                    string url = string.Format("{0}?magop={1}&&malenhsanxuat={2}&&madot={3}&&manpl={4}", URL + "DonHangTong/GetCapThemLichSu", _magop.ToString(),
                       _maLSX.ToString(), searchLookUpEditDot.EditValue ?? "", focusedRow["MaNPL"].ToString());
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json == "[]")
                    {
                        gridControl31.DataSource = null;
                        return;
                    }
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    gridControl31.DataSource = tbl;

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void gridView11_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn17)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView11.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView11.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "IsNPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn17)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.ToString() == "CapThem" || view.FocusedColumn.FieldName.ToString() == "DinhMuc")
            {

                decimal outParse = -1;
                if (e != null && e.Value != null)
                {
                    string inputValue = e.Value.ToString();
                    bool isValidDecimal = decimal.TryParse(inputValue, out outParse);
                    if (string.IsNullOrEmpty(inputValue))
                    {
                        e.Value = 0;
                    }
                    else if (isValidDecimal)
                    {
                        if (outParse < 0)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập > 0!";
                            return;
                        }
                        int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(outParse)[3])[2];
                        //if(view.FocusedColumn.FieldName.ToString() == "CapThem")
                        //{
                        //    if (decimalPlaces > 2)
                        //    {
                        //        e.Valid = false;
                        //        e.ErrorText = "Vui lòng nhập tối đa 2 chữ số sau dấu thập phân!";
                        //        return;
                        //    }
                        //}  
                        //else
                        //{
                        if (decimalPlaces > 4)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập tối đa 2 chữ số sau dấu thập phân!";
                            return;
                        }
                        //}    

                    }
                    else
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!";
                        return;
                    }
                }
            }
        }
        private void gridView11_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.ToString() == "CapThem")
            {
                decimal outParse = -1;
                if (e != null && e.Value != null)
                {
                    string inputValue = e.Value.ToString();
                    bool isValidDecimal = decimal.TryParse(inputValue, out outParse);
                    if (string.IsNullOrEmpty(inputValue))
                    {
                        e.Value = 0;
                    }
                    else if (isValidDecimal)
                    {
                        if (outParse < 0)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập > 0!";
                            return;
                        }
                        int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(outParse)[3])[2];
                        if (decimalPlaces > 2)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập tối đa 2 chữ số sau dấu thập phân!";
                            return;
                        }
                    }
                    else
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!";
                        return;
                    }
                }
            }
        }

        private void gridView11_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "CapThem")
                {
                    if (e.Value.ToString() != "")
                    {
                        object originalValue = gridView11.GetRowCellValue(e.RowHandle, "CapThem_Old");
                        if (!object.Equals(originalValue, e.Value))
                        {
                            gridView11.SetRowCellValue(e.RowHandle, "Sua", 1);
                        }
                        else
                        {
                            gridView11.SetRowCellValue(e.RowHandle, "Sua", 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow focusedRow = gridView11.GetDataRow(gridView11.FocusedRowHandle);
                if (focusedRow == null) return;
                _rowhandle = gridView1.FocusedRowHandle;
                DialogResult result = MessageBox.Show("Bạn có muốn xóa đợt cấp thêm này không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string url = string.Format("{0}?madh={1}&&malenhsanxuat={2}&&manpl={3}&&madot={4}&&dot={5}", URL + "DonHangTong/DeleteCapThem", _magop.ToString(),
                   _maLSX.ToString(), focusedRow["MaNPL"].ToString(), searchLookUpEditDot.EditValue ?? "", focusedRow["Dot"].ToString());
                    string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    if (json.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        loadCapPhat();
                        gridView1.FocusedRowHandle = _rowhandle;
                    }

                }
            }
            catch (Exception ex)
            {

            }

        }
        #region Mạnh
        private DataTable _dtChungLoaiChiTiet; // Thêm biến toàn cục để lưu data
        private string firstSelectedTenNhom = null;
        private void InItChungLoaiChiTiet()
        {
            InIt();
            repoCL.ValueMember = "MaNhom";
            repoCL.DisplayMember = "TenNhom";
            repoCL.NullText = "[..Chủng loại CT..]";
            repoCL.Popup += RepoCL_Popup;

            GridView dvView = repoCL.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Tên nhóm chi tiết", Name = "colTenNhomCT", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaNhom", Caption = "Mã nhóm", Name = "colMaNhom", Visible = false }); // Thêm column này để filter
            }
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            gridView1.ShowingEditor += gridView1_ShowingEditor;


            LoadChungLoaiChiTiet();
        }

        private void LoadChungLoaiChiTiet()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GetChiTietNhom";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);

            // Gán DataSource cho repoCL - THIẾU PHẦN NÀY
            repoCL.DataSource = _dtChungLoaiChiTiet;


            searchLookUpEditCLCT.Properties.DataSource = _dtChungLoaiChiTiet;
        }

        private void RepoCL_Popup(object sender, EventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            if (editor == null) return;

            var drRow = gridView1.GetFocusedDataRow();
            if (drRow == null) return;

            string tenNhom = drRow["TenNhom"]?.ToString();
            if (string.IsNullOrEmpty(tenNhom)) return;

            var popupView = editor.Properties.View;
            popupView.ActiveFilterString = $"[TenNhom] = '{tenNhom}'";
        }
        bool showedMessage = false;
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            var view = sender as GridView;
            if (view == null) return;
            showedMessage = false;
            var selectedHandles = view.GetSelectedRows();
            if (firstSelectedTenNhom == null && selectedHandles.Length > 0)
            {
                DataRow firstRow = view.GetDataRow(selectedHandles[0]);
                if (firstRow != null)
                    firstSelectedTenNhom = firstRow["TenNhom"]?.ToString();
            }
            if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == Keys.None)
            {
                DataRow firstRow = view.GetDataRow(selectedHandles[0]);
                if (firstRow != null)
                    firstSelectedTenNhom = firstRow["TenNhom"]?.ToString();
                return;
            }
            if (selectedHandles.Length == 0)
            {
                firstSelectedTenNhom = null;
                return;
            }

            // 🔴 Kiểm tra tất cả các dòng đã chọn
            foreach (int handle in selectedHandles)
            {
                DataRow row = view.GetDataRow(handle);
                if (row == null) continue;

                string tenNhom = row["TenNhom"]?.ToString();
                if (tenNhom != firstSelectedTenNhom)
                {
                    // ❌ Bỏ chọn dòng không đúng nhóm
                    view.UnselectRow(handle);

                    if (!showedMessage)
                    {
                        showedMessage = true;
                        XtraMessageBox.Show(
                            "Tất cả các dòng được chọn phải có cùng Tên Nhóm với dòng đầu tiên!",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }

                    // ❗ break để không lặp cảnh báo trong cùng lần xử lý
                    break;
                }
            }
        }
      
      
        private void InIt()
        {
            searchLookUpEditCLCT.Properties.ValueMember = "MaNhom";
            searchLookUpEditCLCT.Properties.DisplayMember = "TenNhom";
            searchLookUpEditCLCT.Properties.NullValuePrompt = "Chọn chủng loại CT";

            searchLookUpEditCLCT.Popup += SearchLookUpEditCLCT_Popup;
         
        }
        private void SearchLookUpEditCLCT_Popup(object sender, EventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            if (editor == null) return;
            var selectedHandles = gridView1.GetSelectedRows();
            DataRow firstRow = gridView1.GetDataRow(selectedHandles[0]);

            if (firstRow == null) return;

            string tenNhom = firstRow["TenNhom"]?.ToString();
            if (string.IsNullOrEmpty(tenNhom)) return;

            var popupView = editor.Properties.View;
            popupView.ActiveFilterString = $"[TenNhom] = '{tenNhom}'";
        }
        private void searchLookUpEditCLCT_Properties_EditValueChanged(object sender, EventArgs e)
        {
            int[] selectedHandles = gridView1.GetSelectedRows();
            string CLCT = (searchLookUpEditCLCT.EditValue as string) ?? "";
            if (CLCT == "") return;
            if (selectedHandles.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng trước khi lưu!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (int handle in selectedHandles)
            {
                DataRow row = gridView1.GetDataRow(handle);
                if (row != null && row["CheckMaCLCT"].ToString() == "0")
                {
                    row["MaCLCT"] = CLCT;
                }
            }

            gridView1.RefreshData();
            searchLookUpEditCLCT.EditValue = null;
        }
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
          
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "MaCLCT") // cột cần readonly
            {
                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                if (row != null)
                {
                    int checkValue = Convert.ToInt32(row["CheckMaCLCT"]);
                    if (checkValue == 1)
                    {
                        // ❌ Không cho edit ô này
                        e.Cancel = true;
                    }
                }
            }
        }
        #endregion

    }
}
