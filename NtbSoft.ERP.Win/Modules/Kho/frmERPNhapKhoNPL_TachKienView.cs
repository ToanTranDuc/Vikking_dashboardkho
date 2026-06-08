using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.Kho;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmERPNhapKhoNPL_TachKienView : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataTable tblNhapKho;


        List<DevExpress.XtraGrid.Columns.GridColumn> lstColGroupDraw = new List<DevExpress.XtraGrid.Columns.GridColumn>();
        public frmERPNhapKhoNPL_TachKienView()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookUpSoLo();
            CreateSearchLookUpItems();
            CreateTableNhapKho();
            SetupGroupLevelColors();
            lstColGroupDraw.AddRange(new[] { colSoLo, colBarCodeGoc, colLoaiVatTu });

        }

        private Dictionary<int, Color> groupLevelColors;
        private Dictionary<int, Color> groupLevelColorBackground;
        private void SetupGroupLevelColors()
        {
            groupLevelColorBackground = new Dictionary<int, Color>
            {
                //{ 3, ColorTranslator.FromHtml("#DDEBFB") },
              
                { 1, ColorTranslator.FromHtml("#FFE2D3") },
                { 0, ColorTranslator.FromHtml("#D7F5E8") },
                { 2, ColorTranslator.FromHtml("#DDEBFB") },
            };

            groupLevelColors = new Dictionary<int, Color>
            {
                //{ 3, ColorTranslator.FromHtml("#2A5D9F") },
                { 0, ColorTranslator.FromHtml("#2A5D9F") },
                { 1, ColorTranslator.FromHtml("#A53E25") },
                { 2, ColorTranslator.FromHtml("#2E7D5B") }
            };


        }

        private void CreateSearchLookUpSoLo()
        {
            string url = $"{URL}ERPNhapKhoNPLTachKien/Get?Action=GETSOLO";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                repositoryItemSearchLookUpEdit2.DataSource = null;
                barEditItemSoLo.EditValue = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit2.DataSource = tbl;
            repositoryItemSearchLookUpEdit2.DisplayMember = "SoLo";
            repositoryItemSearchLookUpEdit2.ValueMember = "SoLoID";
            barEditItemSoLo.Refresh();
        }
        private void CreateTableNhapKho()
        {
            tblNhapKho = new DataTable("tblNhapKho");
            tblNhapKho.Columns.Add("ID", typeof(int));
            tblNhapKho.Columns.Add("SoLoID", typeof(string));
            tblNhapKho.Columns.Add("MaNPL", typeof(string));
            tblNhapKho.Columns.Add("MaVTID", typeof(string));
            tblNhapKho.Columns.Add("MaMauVT", typeof(string));
            tblNhapKho.Columns.Add("MauVT", typeof(string));
            tblNhapKho.Columns.Add("SoKien", typeof(string));
            tblNhapKho.Columns.Add("SoLot", typeof(string));
            tblNhapKho.Columns.Add("MaHaiQuan", typeof(string));
            tblNhapKho.Columns.Add("MaKeToan", typeof(string));
            tblNhapKho.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblNhapKho.Columns.Add("NW", typeof(decimal));
            tblNhapKho.Columns.Add("GW", typeof(decimal));
            tblNhapKho.Columns.Add("BarCode", typeof(string));
            tblNhapKho.Columns.Add("GhiChu", typeof(string));
            tblNhapKho.Columns.Add("IsNPL", typeof(bool));
            tblNhapKho.Columns.Add("KhoVaiID", typeof(string));
            tblNhapKho.Columns.Add("SoKienParent", typeof(string));
            tblNhapKho.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblNhapKho.Columns.Add("DonGia", typeof(decimal));
            tblNhapKho.Columns.Add("ThanhTien", typeof(decimal));
            tblNhapKho.Columns.Add("Pallet", typeof(string));
            tblNhapKho.Columns.Add("MaDVVT", typeof(string));
            tblNhapKho.Columns.Add("MauVTID", typeof(string));
            tblNhapKho.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKho.Columns.Add("IsNK", typeof(bool));
            tblNhapKho.Columns.Add("STT", typeof(string));
            tblNhapKho.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblNhapKho.Columns.Add("MaDVCD", typeof(string));
            tblNhapKho.Columns.Add("TenDVCD", typeof(string));
            tblNhapKho.Columns.Add("tileNW", typeof(decimal));
            tblNhapKho.Columns.Add("tileGW", typeof(decimal));
            tblNhapKho.Columns.Add("NgayNhapKho", typeof(DateTime));
            tblNhapKho.Columns.Add("BarCodeGoc", typeof(string));
            tblNhapKho.Columns.Add("TienTe", typeof(string));
            tblNhapKho.Columns.Add("MaVTGhep", typeof(string));
            tblNhapKho.Columns.Add("MaNhom", typeof(string));
            tblNhapKho.Columns.Add("QuyDoiID", typeof(string));
            tblNhapKho.Columns.Add("POMua", typeof(string));
            tblNhapKho.Columns.Add("Batch", typeof(string));
            tblNhapKho.Columns.Add("SLTong", typeof(decimal));
        }
        private void CreateSearchLookUpItems()
        {
            string soloid = barEditItemSoLo.EditValue == null ? "" : barEditItemSoLo.EditValue.ToString();
            string url = $"{URL}ERPNhapKhoNPLTachKien/Get?Action=GETCAYVAI&para={soloid}";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                repositoryItemSearchLookUpEdit1.DataSource = null;
                barEditItemItems.EditValue = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit1.DataSource = tbl;
            repositoryItemSearchLookUpEdit1.DisplayMember = "ChiTiet";
            repositoryItemSearchLookUpEdit1.ValueMember = "MaVTID";
            barEditItemItems.Refresh();
        }
        private void LoadData()
        {
            string soloid = GetFieldValue(barEditItemSoLo, "SoLoID");
            int IsNPL = GetFieldValue(barEditItemSoLo, "IsNPL") == "True" ? 1 : 0;
            string mavtid = barEditItemItems.EditValue == null ? "" : barEditItemItems.EditValue.ToString();
            string url = $"{URL}ERPNhapKhoNPLTachKien/Get?Action=GETCHITIET&para={soloid}&para2={mavtid}&para3={IsNPL}";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gC.DataSource = null;

                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && !tbl.Columns.Contains("IsTach"))
            {
                tbl.Columns.Add("IsTach", typeof(bool));
            }

            gV.ExpandAllGroups();
            gC.DataSource = CalcTableKienTach(tbl);
            //gridColumn44.ColumnEdit = ItemDeleteKien;
        }
        private DataTable SortDataTable(DataTable sourceTable)
        {
            var comparer = new STTComparerLib();

            // Sắp xếp theo cột SoKienHienThi với comparer custom
            var sortedRows = sourceTable.AsEnumerable()
                .OrderByDescending(row => row["SoKienHienThi"]?.ToString(), comparer)
                .CopyToDataTable();

            return sortedRows;
        }
        private DataTable CalcTableKienTach(DataTable tblKienTach)
        {
            DataTable tbl = SortDataTable(tblKienTach.Copy());

            foreach (DataRow row in tbl.Rows)
            {
                string BarCode = row["BarCode"]?.ToString();
                //object BarCodeGocDB = row["BarCodeGoc"];
                var query = GetChildrenBarCodes(tblKienTach, BarCode);

                string RootBarCode = FindRootBarCode(tblKienTach, BarCode);
                row["BarCodeGoc"] = RootBarCode;

                var ItemBarCodeParent = row;

                if (row == null) continue;

                decimal.TryParse(ItemBarCodeParent["SLThuHoi"]?.ToString(), out decimal ThuHoiParent);
                decimal.TryParse(ItemBarCodeParent["SLXuat"]?.ToString(), out decimal XuatDiParent);
                decimal.TryParse(ItemBarCodeParent["SoGhiDauCay"]?.ToString(), out decimal SLCTParent);

                decimal.TryParse(ItemBarCodeParent["SoLuongThucTe"]?.ToString(), out decimal SLTTParent);

                decimal.TryParse(ItemBarCodeParent["NW"]?.ToString(), out decimal NWParent);

                decimal.TryParse(ItemBarCodeParent["GW"]?.ToString(), out decimal GWParent);
                decimal.TryParse(ItemBarCodeParent["DonGia"]?.ToString(), out decimal DonGiaParent);

                if (query != null && query?.Count > 0)
                {
                    DataTable tblKienChild = query?.CopyToDataTable();
                   

                    var sums = tblKienChild.AsEnumerable().Aggregate(
                    new { TonKho = 0m, SLTT = 0m, SLCT = 0m, NW = 0m, GW = 0m },
                    (acc, rowSum) =>
                    {
                        decimal.TryParse(rowSum["TonKho"]?.ToString(), out decimal tk);
                        decimal.TryParse(rowSum["SoLuongThucTe"]?.ToString(), out decimal sltt);
                        decimal.TryParse(rowSum["SoGhiDauCay"]?.ToString(), out decimal slct);
                        decimal.TryParse(rowSum["NW"]?.ToString(), out decimal nw);
                        decimal.TryParse(rowSum["GW"]?.ToString(), out decimal gw);

                        return new
                        {
                            TonKho = acc.SLTT + tk,
                            SLTT = acc.SLTT + sltt,
                            SLCT = acc.SLCT + slct,
                            NW = acc.NW + nw,
                            GW = acc.GW + gw
                        };
                    }
                );



                    decimal SLTT = SLTTParent - sums.SLTT;
                    row["SLCTView"] = SLCTParent - sums.SLCT;
                    row["SLTTView"] = SLTT;
                    row["NWView"] = NWParent - sums.NW;
                    row["GWView"] = GWParent - sums.GW;
                    row["TonKho"] = SLTT - XuatDiParent + ThuHoiParent;
                    row["ThanhTienView"] = DonGiaParent * SLTT;


                }
                else
                {
                   
                    row["SLCTView"] = SLCTParent;
                    row["SLTTView"] = SLTTParent;
                    row["NWView"] = NWParent;
                    row["GWView"] = GWParent ;
                    row["TonKho"] = SLTTParent - XuatDiParent + ThuHoiParent;
                    row["ThanhTienView"] = DonGiaParent * SLTTParent;
                }
                
            }
            return tbl;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditItemSoLo.EditValue == null && barEditItemItems.EditValue == null) return;
            LoadData();
        }

        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            string level = GetGroupLevel(view.GetRowLevel(e.RowHandle));
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (lstColGroupDraw.Contains(info.Column))
            {
                if (info.Column == colBarCodeGoc)
                {
                    int childRowHandle = view.GetChildRowHandle(e.RowHandle, 0);
                    DataRow row = view.GetDataRow(childRowHandle);
                    if (row != null)
                    {
                        DataTable tbl = gC.DataSource as DataTable;
                        int CountChild = view.GetChildRowCount(e.RowHandle) - 1;
                        string barcodeGoc = row["SoKienParent"]?.ToString();

                        decimal SLTach = tbl.AsEnumerable()
                            .Where(x => x.RowState != DataRowState.Deleted) // bỏ row đã xóa
                            .GroupBy(x => x["BarCodeGoc"]?.ToString())
                            .Where(g => g.Key == barcodeGoc)
                            .Select(g => g.Where(x => x["Barcode"]?.ToString() != g.Key)
                                          .Sum(x =>
                                          {
                                              decimal value;
                                              return decimal.TryParse(x["TonKho"]?.ToString(), out value) ? value : 0;
                                          })
                            )
                            .FirstOrDefault();



                        info.GroupText = string.Format("{0} Kiện {1} - {2} {3} Kiện đã tách - SL tách {4}", level, row["KienGoc"]?.ToString(), level, CountChild, SLTach);
                    }

                }
                else
                {
                    info.GroupText = string.Format("{0}  {1} ", level, info.GroupValueText);
                }

            }


            if (gV.IsGroupRow(e.RowHandle))
            {
                int groupLevel = view.GetRowLevel(e.RowHandle);
                //groupLevel = groupLevel - 1;
                Color textColor = Color.Black;
                Font font = e.Appearance.Font;
                if (groupLevel >= 0 && groupLevel < groupLevelColors.Count)
                {
                    textColor = groupLevelColors[groupLevel];
                }

                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private string GetGroupLevel(int level)
        {
            switch (level)
            {
                case 0: return "📋"; // Fixed groups              
                case 1: return "🏷"; // Barcode package
                case 2: return "📦"; // Main package
                default: return "📦"; // Item level
            }
        }

        private void gV_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "TrangThai")
            {
                string value = gV.GetRowCellValue(e.RowHandle, e.Column)?.ToString();
                if (value == "Đã nhập kho")
                {
                    e.Appearance.ForeColor = Color.Green;
                }
                else if (value == "Chưa nhập kho")
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }

        private void gV_RowStyle(object sender, RowStyleEventArgs e)
        {
            // Thiết lập Focused Row riêng biệt
            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;
            if (e.RowHandle >= 0)
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                object BarCode = view.GetRowCellValue(e.RowHandle, "BarCode");
                object BarCodeGoc = view.GetRowCellValue(e.RowHandle, "BarCodeGoc");
                object SoKienHienThi = view.GetRowCellValue(e.RowHandle, "SoKienHienThi");
                bool isTach = BarCodeGoc != null && BarCode != null && BarCodeGoc.Equals(BarCode);

                if (!isTach)
                {
                    // Nền cho row đặc biệt
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#CCE5FF"); // Xanh nhạt
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("#17203D"); // Xám đậm
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }

            }


            gridView.Appearance.FocusedRow.BackColor = ColorTranslator.FromHtml("#FAF3E0"); // Xanh pastel đậm hơn
            gridView.Appearance.FocusedRow.ForeColor = ColorTranslator.FromHtml("#333333"); // Chữ xanh đậm
            gridView.Appearance.FocusedRow.Font = new Font(gridView.Appearance.FocusedRow.Font, FontStyle.Bold);
        }



        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) return;

            string[] specificColumnsToRemove = { "MauVT", "STT" };

            var columnsToRemove = tblNhapKho.Columns
                .Cast<DataColumn>()
                .Where(c => specificColumnsToRemove.Contains(c.ColumnName) || c.ColumnName.Contains("View"))
                .Select(c => c.ColumnName)
                .ToList();

            foreach (var col in columnsToRemove)
            {
                tblNhapKho.Columns.Remove(col);
            }

            string url = $"{URL}ERPNhapKhoNPLTachKien/Post?Action=POST";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKho); }).Result;
            if (msResult.ToLower() == "true")
            {
                tblNhapKho.Clear();
                LoadData();
                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }



        private void xoaKien(string id)
        {
            try
            {
                string url = $"{URL}ERPNhapKhoNPLTachKien/Delete?Action=DELETE&para={id}";
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                if (result.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }

        }

        private string GetFieldValue(BarEditItem barEditItem, string fieldName)
        {
            try
            {
                // Lấy EditValue trước
                object editValue = barEditItem.EditValue;
                if (editValue == null) return "";

                RepositoryItemSearchLookUpEdit searchLookUp = barEditItem?.Edit as RepositoryItemSearchLookUpEdit;
                if (searchLookUp?.DataSource == null) return "";

                // Lấy ValueMember để so sánh
                string valueMember = searchLookUp.ValueMember;
                if (string.IsNullOrEmpty(valueMember)) return "";

                // Tìm trong DataSource
                if (searchLookUp.DataSource is DataTable dataTable)
                {
                    // Tìm row có ValueMember = EditValue
                    DataRow[] foundRows = dataTable.Select($"{valueMember} = '{editValue}'");
                    if (foundRows.Length > 0)
                    {
                        return foundRows[0][fieldName]?.ToString() ?? "";
                    }
                }
                else if (searchLookUp.DataSource is DataView dataView)
                {
                    DataRow[] foundRows = dataView.Table.Select($"{valueMember} = '{editValue}'");
                    if (foundRows.Length > 0)
                    {
                        return foundRows[0][fieldName]?.ToString() ?? "";
                    }
                }
                else if (searchLookUp.DataSource is BindingSource bindingSource)
                {
                    if (bindingSource.DataSource is DataTable dt)
                    {
                        DataRow[] foundRows = dt.Select($"{valueMember} = '{editValue}'");
                        if (foundRows.Length > 0)
                        {
                            return foundRows[0][fieldName]?.ToString() ?? "";
                        }
                    }
                }
            }
            catch { }

            return "";
        }


        private void menuTachKien_Click(object sender, EventArgs e)
        {
            DataRow dr = gV.GetFocusedDataRow();

            if (dr == null) return;
            DataTable tblGC = gC.DataSource as DataTable;
            if (tblGC == null || tblGC.Rows.Count == 0) return;
            //if (dr["SoKienHienThi"].ToString().Contains('.'))
            //    return;
            double.TryParse(dr["TonKho"]?.ToString(), out double TonKho);
            if (TonKho > 0)
            {
                string barCodeGocValue = dr["BarCode"].ToString();
                DataRow[] resultRows = tblGC.Select($"SoKienParent = '{barCodeGocValue.Replace("'", "''")}'");
                int _sk = resultRows.AsEnumerable()
                .Select(r => r["Barcode"]?.ToString())
                .Where(s => !string.IsNullOrEmpty(s) && s.Contains('.'))
                .Select(s => s.Split('.').Last())
                .Select(x => int.TryParse(x, out var n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();


                frmERPNhapKhoNPL_TachKien frm = new frmERPNhapKhoNPL_TachKien(dr, _sk, tblGC);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    DataTable dt = frm.tblNhapKho;
                    if (dt == null || dt.Rows.Count == 0) return;

                    foreach (DataRow _r in dt.Rows)
                    {
                        //
                        var maVTID = _r["MaVTID"];
                        var mauVTID = _r["MauVTID"];
                        var khoVaiID = _r["KhoVaiID"];
                        var kienGoc = _r["BarCode"];
                        var soKienHienThi = _r["SoKienHienThi"];
                        DataRow firstRow = tblNhapKho.AsEnumerable()
                        .FirstOrDefault(r =>
                            object.Equals(r["MaVTID"], maVTID) &&
                            object.Equals(r["MauVTID"], mauVTID) &&
                            object.Equals(r["KhoVaiID"], khoVaiID) &&
                            object.Equals(r["BarCode"], kienGoc) &&
                            object.Equals(r["SoKienHienThi"], soKienHienThi)
                        );
                        if (firstRow == null)
                        {
                            tblNhapKho.ImportRow(_r);
                        }
                        //else
                        //{
                        //    firstRow["SoGhiDauCay"] = _r["SoGhiDauCay"];
                        //    firstRow["NW"] = _r["NW"];
                        //    firstRow["GW"] = _r["GW"];
                        //    firstRow["ThanhTien"] = _r["ThanhTien"];
                        //    firstRow["SoLuongThucTe"] = _r["SoLuongThucTe"];

                        //}


                        //if (dr["MaVTID"].ToString() == _r["MaVTID"].ToString() && dr["MauVTID"].ToString() == _r["MauVTID"].ToString() &&
                        //    dr["KhoVaiID"].ToString() == _r["KhoVaiID"].ToString() && dr["SoKienHienThi"].ToString() == _r["SoKienHienThi"].ToString())
                        //{
                        //    //dr["SoGhiDauCay"] = _r["SoGhiDauCay"];
                        //    //dr["NW"] = _r["NW"];
                        //    //dr["GW"] = _r["GW"];
                        //    //dr["ThanhTien"] = _r["ThanhTien"];
                        //    //dr["SoLuongThucTe"] = _r["SoLuongThucTe"];

                        //    dr["SLCTView"] = _r["SLCTView"];
                        //    dr["SLCTView"] = _r["SLCTView"];
                           
                        //    dr["NWView"] = _r["NWView"];
                        //    dr["GWView"] = _r["GWView"];

                        //    dr["ThanhTienView"] = _r["ThanhTienView"];

                        //    Decimal.TryParse(dr["SLXuat"]?.ToString(), out Decimal SLXuat);
                        //    Decimal.TryParse(dr["SLThuHoi"]?.ToString(), out Decimal SLThuHoi);
                        //    Decimal.TryParse(_r["SLTTView"]?.ToString(), out Decimal SLTTVIew);
                        //    dr["SLTTView"] = SLTTVIew;
                        //    dr["TonKho"] = SLTTVIew - SLXuat + SLThuHoi;
                        //    continue;
                        //}

                        if (!tblGC.AsEnumerable().Any(r =>
                            r["MaVTID"].ToString() == _r["MaVTID"].ToString() &&
                            r["MauVTID"].ToString() == _r["MauVTID"].ToString() &&
                            r["KhoVaiID"].ToString() == _r["KhoVaiID"].ToString() &&
                            r["SoKienHienThi"].ToString() == _r["SoKienHienThi"].ToString()))
                        {
                            DataRow _nr = tblGC.NewRow();
                            _nr["ID"] = _r["ID"];
                            _nr["SoLoID"] = _r["SoLoID"];
                            _nr["MaNPL"] = _r["MaNPL"];
                            _nr["MaVTID"] = _r["MaVTID"];
                            //

                            _nr["SoLo"] = dr["SoLo"];
                            _nr["MaVT"] = dr["MaVT"];
                            _nr["ChiTiet"] = dr["ChiTiet"];
                            _nr["KhoVai"] = dr["KhoVai"];
                            _nr["MaNhom"] = dr["MaNhom"];
                            _nr["TenNhom"] = dr["TenNhom"];
                            _nr["Sort"] = dr["Sort"];
                            _nr["BarCodeGoc"] = dr["BarCodeGoc"].ToString();
                            //
                            _nr["MaMauVT"] = _r["MaMauVT"];
                            _nr["MauVT"] = _r["MauVT"];
                            _nr["SoKien"] = _r["SoKien"];
                            _nr["SoLot"] = _r["SoLot"];
                            _nr["MaHaiQuan"] = _r["MaHaiQuan"];
                            _nr["MaKeToan"] = _r["MaKeToan"];
                            _nr["SoGhiDauCay"] = _r["SoGhiDauCay"];
                            _nr["NW"] = _r["NW"];
                            _nr["GW"] = _r["GW"];
                            _nr["NWView"] = _r["NWView"];
                            _nr["GWView"] = _r["GWView"];
                            _nr["Barcode"] = _r["Barcode"];
                            _nr["GhiChu"] = _r["GhiChu"];
                            _nr["IsNPL"] = _r["IsNPL"];
                            _nr["KhoVaiID"] = _r["KhoVaiID"];
                            _nr["SoKienParent"] = _r["SoKienParent"];
                            _nr["SoLuongThucTe"] = _r["SoLuongThucTe"];
                            _nr["DonGia"] = _r["DonGia"];
                            _nr["ThanhTien"] = _r["ThanhTien"];
                            _nr["ThanhTienView"] = _r["ThanhTienView"];
                            _nr["Pallet"] = _r["Pallet"];
                            _nr["MaDVVT"] = _r["MaDVVT"];
                            _nr["MauVTID"] = _r["MauVTID"];
                            _nr["SoKienHienThi"] = _r["SoKienHienThi"];
                            _nr["IsNK"] = _r["IsNK"];
                            _nr["NgayNKDuKien"] = _r["NgayNKDuKien"];
                            _nr["MaDVCD"] = _r["MaDVCD"];
                            _nr["TenDVCD"] = _r["TenDVCD"];
                            _nr["tileNW"] = _r["tileNW"];
                            _nr["tileGW"] = _r["tileGW"];
                            _nr["IsTach"] = true;
                            _nr["TonKho"] = _r["TonKho"];
                            _nr["SLXuat"] = 0;
                            _nr["SLThuHoi"] = 0;
                            _nr["SLCTView"] = _r["SLCTView"];
                            _nr["SLTTView"] = _r["SLTTView"];

                            _nr["TienTe"] = _r["TienTe"];
                            _nr["MaVTGhep"] = _r["MaVTGhep"];
                            _nr["MaNhom"] = _r["MaNhom"];
                            _nr["QuyDoiID"] = _r["QuyDoiID"];
                            _nr["POMua"] = _r["POMua"];
                            _nr["Batch"] = _r["Batch"];
                            _nr["SLTong"] = _r["SLTong"];
                            tblGC.Rows.InsertAt(_nr, 0);
                        }


                    }


                    gC.DataSource = CalcTableKienTach(tblGC);
                    gC.RefreshDataSource();



                }
            }
            //else
            //{
            //    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2016 Colorful");

            //    DevExpress.XtraEditors.XtraMessageBox.Show(
            //        $"Số kiện {dr["SoKien"]?.ToString()} hiện tại chưa tồn kho, không thể tách.",
            //        "Thông báo",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Warning);
            //}


        }

        private void gV_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                DataRow _rowFocus = gV.GetFocusedDataRow();
                if (_rowFocus == null || e.Menu == null) return;
                double.TryParse(_rowFocus["TonKho"]?.ToString(), out double TonKho);
                if (TonKho > 0 && !gV.IsGroupRow(gV.FocusedRowHandle))
                {
                    DXMenuItem menuTachKien = new DXMenuItem();
                    menuTachKien.Caption = "Tách Kiện";
                    menuTachKien.Appearance.Font = new Font("Arial", 9);
                    menuTachKien.Click += menuTachKien_Click;
                    e.Menu.Items.Add(menuTachKien);
                }
                //else
                //{
                //    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2016 Colorful");

                //    DevExpress.XtraEditors.XtraMessageBox.Show(
                //        $"Số kiện {_rowFocus["SoKien"]?.ToString()} hiện tại chưa tồn kho, không thể tách.",
                //        "Thông báo",
                //        MessageBoxButtons.OK,
                //        MessageBoxIcon.Warning);
                //}


            }
            catch (Exception ex)
            {


            }
        }


        private void gV_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "BarCodeGoc")
            {
                var row1 = e.RowObject1 as DataRowView;
                var row2 = e.RowObject2 as DataRowView;

                if (row1 != null && row2 != null)
                {
                    string phaseId1 = row1["SoKienHienThi"]?.ToString();
                    string phaseId2 = row2["SoKienHienThi"]?.ToString();

                    var comparer = new STTComparerLib();
                    e.Result = comparer.Compare(phaseId1, phaseId2);
                    e.Handled = true;
                }
            }
        }

        private bool HasChildItems(string BarCode)
        {
            DataTable tblKienGOC = gC.DataSource as DataTable;
            List<DataRow> lstChildKien = GetChildrenBarCodes(tblKienGOC, BarCode);

            return lstChildKien?.Count > 0;
        }

        /// <summary>
        /// Kiểm tra item có phải là con trực tiếp không (helper method)
        /// </summary>


        private string FindRootBarCode(DataTable dt, string inputBarCode)
        {
            var currentBarCode = inputBarCode;
            var visited = new HashSet<string>(); // Tránh vòng lặp vô hạn

            while (!string.IsNullOrEmpty(currentBarCode) && !visited.Contains(currentBarCode))
            {
                visited.Add(currentBarCode);

                // Tìm row có BarCode = currentBarCode
                var currentRow = dt.AsEnumerable()
                    .FirstOrDefault(row => row["BarCode"]?.ToString() == currentBarCode);

                if (currentRow == null)
                    break;

                var barCodeGoc = currentRow["SoKienParent"]?.ToString();

                // Nếu BarCodeGoc null hoặc giống với BarCode hiện tại thì đã đến gốc
                if (string.IsNullOrEmpty(barCodeGoc) || barCodeGoc == currentBarCode)
                    break;

                // Kiểm tra xem BarCodeGoc có tồn tại trong DataTable không
                var parentExists = dt.AsEnumerable()
                    .Any(row => row["BarCode"]?.ToString() == barCodeGoc);

                if (!parentExists)
                    break;

                currentBarCode = barCodeGoc;
            }

            return currentBarCode;
        }
        private List<DataRow> GetChildrenBarCodes(DataTable dt, string parentBarCode)
        {
            return dt.AsEnumerable()
                .Where(row => row["SoKienParent"]?.ToString() == parentBarCode && row["BarCode"]?.ToString() != parentBarCode)
                .ToList();
        }

        private void DeleteRowByBarcode(string barcode)
        {
            var tbl = gC.DataSource as DataTable;
            if (tbl == null) return;

            var rowToDelete = tbl.AsEnumerable()
                                  .FirstOrDefault(r => r.RowState != DataRowState.Deleted &&
                                                       r["BarCode"]?.ToString() == barcode);
            if (rowToDelete != null)
            {
                tbl.Rows.Remove(rowToDelete); // Xóa hẳn
            }
        }

        private void DeleteFromTableNhapKho(string barcode)
        {
            try
            {
                if (tblNhapKho is DataTable dataTable)
                {
                    // Tìm tất cả các row có BarCode tương ứng
                    var rowsToDelete = dataTable.AsEnumerable()
                        .Where(row => row.RowState != DataRowState.Deleted &&
                                      row["BarCode"]?.ToString() == barcode)
                        .ToList();

                    // Xóa hẳn bằng Rows.Remove
                    foreach (var row in rowsToDelete)
                    {
                        dataTable.Rows.Remove(row);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
            }
        }

        private void gV_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (gV.FocusedColumn == gridColumn44)
            {
                try
                {
                    DataRow dr = gV.GetFocusedDataRow();
                    if (dr == null) return;
                    string id = dr["ID"].ToString();
                    string BarCodeGoc = dr["SoKienParent"]?.ToString();
                    string BarCode = dr["BarCode"]?.ToString();
                    bool HasChild = HasChildItems(dr["BarCode"]?.ToString());
                    if (HasChild)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không thể xóa kiện gốc", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                    else if (BarCodeGoc == BarCode)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không thể xóa kiện", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    DialogResult messResult = MessageBox.Show($"Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.Yes)
                    {

                       
                        if (id != "0")
                        {
                            xoaKien(id);

                        }

                        DeleteRowByBarcode(BarCode);
                        DeleteFromTableNhapKho(BarCode);
                        gC.RefreshDataSource();
                        DataTable tbl = gC.DataSource as DataTable;
                        gC.DataSource = CalcTableKienTach(tbl);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
