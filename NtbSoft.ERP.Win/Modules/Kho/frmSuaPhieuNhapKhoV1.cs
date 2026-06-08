using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmSuaPhieuNhapKhoV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private int _stt = 0;
        private string selectedValuessItems = string.Empty;
        private string selectedValuesItems = string.Empty;
        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();
        DataTable _dtChiTiet;
        public frmSuaPhieuNhapKhoV1()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dtChiTiet = new DataTable();
        }
        protected override void OnLoad(EventArgs e)
        {
            dateEditNgayNhapKho.EditValue = DateTime.Now;
            LoadSTT();
            CreateSearchLookUpSoLo();
            CreateTableChiTiet();
        }
        private void CreateTableChiTiet()
        {
            _dtChiTiet = new DataTable("_dtChiTiet");
            _dtChiTiet.Columns.Add("SoLoID", typeof(string));
            _dtChiTiet.Columns.Add("MaNPL", typeof(string));
            _dtChiTiet.Columns.Add("MaVTID", typeof(string));
            _dtChiTiet.Columns.Add("MauVTID", typeof(string));
            _dtChiTiet.Columns.Add("MaMauVT", typeof(string));
            _dtChiTiet.Columns.Add("SoKien", typeof(string));
            _dtChiTiet.Columns.Add("SoLoT", typeof(string));
            _dtChiTiet.Columns.Add("SoLuongThucTe", typeof(decimal));
            _dtChiTiet.Columns.Add("SoGhiDauCay", typeof(decimal));
            _dtChiTiet.Columns.Add("NW", typeof(decimal));
            _dtChiTiet.Columns.Add("GW", typeof(decimal));
            _dtChiTiet.Columns.Add("GhiChu", typeof(string));

        }
        private void CreateSearchLookUpSoLo()
        {
            string url = string.Format("{0}?", URL + "PhieuNhapKho/GetSoLo");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditSoLo.Properties.DataSource = null;
                searchLookUpEditSoLo.EditValue = null;
                return;
            }


            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditSoLo.Properties.DataSource = tbl;
            searchLookUpEditSoLo.Properties.DisplayMember = "SoLo";
            searchLookUpEditSoLo.Properties.ValueMember = "SoLoID";
            searchLookUpEditSoLo.RefreshEditValue();
            searchLookUpEditSoLo.Refresh();
        }
        private void CreateSearchLookUpCayVai(string soloid)
        {
            string url = string.Format("{0}?soloid={1}", URL + "PhieuNhapKho/GetCayVai", soloid.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditVatTu.Properties.DataSource = null;
                searchLookUpEditVatTu.EditValue = null;
                return;
            }


            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditVatTu.Properties.DataSource = tbl;
            searchLookUpEditVatTu.Properties.DisplayMember = "ChiTiet";
            searchLookUpEditVatTu.Properties.ValueMember = "MaVTID";
            searchLookUpEditVatTu.RefreshEditValue();
            searchLookUpEditVatTu.Refresh();
        }
        private void LoadSTT()
        {
            string url = string.Format("{0}?", URL + "PhieuNhapKho/GetMaxSTT");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;
        }
        private void LoadDataThongTin(string soloid)
        {
            string url = string.Format("{0}?soloid={1}", URL + "PhieuNhapKho/GetThongTinSoLo", soloid.ToString() == "" ? "" : soloid.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            var Rows = tbl.AsEnumerable()
             .GroupBy(row => new
             {
                 NhaCungCap = row["NhaCungCap"],
                 SoChungTu = row["SoChungTu"],
                 NgayChungTu = row["NgayChungTu"],
                 SoBienBan = row["SoBienBan"],
                 NgayBienBan = row["NgayBienBan"],
                 SoHopDong = row["SoHopDong"]
             })
            .Select(group => group.First())
             .ToList();
            textBoxSoChungTu.Text = Rows[0]["SoChungTu"].ToString();
            textBoxSoHopDong.Text = Rows[0]["SoHopDong"].ToString();
            textBoxBienBanKiem.Text = Rows[0]["SoBienBan"].ToString();
            dateEditNgayChungTu.EditValue = Rows[0]["NgayChungTu"];
            dateEditNgayBBKiem.EditValue = Rows[0]["NgayBienBan"];
        }
        private void LoadData(string soloid, string manpl)
        {
            string url = string.Format("{0}?soloid={1}&&manpl={2}", URL + "PhieuNhapKho/GetDanhSach", soloid.ToString() == "" ? "" : soloid.ToString(),
                manpl.ToString() == "" ? "" : manpl.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gridControl2.DataSource = null;
                return;
            }
            DataTable tblDanhSach = JsonConvert.DeserializeObject<DataTable>(json);
            //var Rows = tblDanhSach.AsEnumerable()
            // .GroupBy(row => new
            // {
            //     NhaCungCap = row["NhaCungCap"],
            //     SoChungTu = row["SoChungTu"],
            //     NgayChungTu = row["NgayChungTu"],
            //     SoBienBan = row["SoBienBan"],
            //     NgayBienBan = row["NgayBienBan"],
            //     SoHopDong = row["SoHopDong"]
            // })
            //.Select(group => group.First())
            // .ToList();
            _dtChiTiet.Clear();
            gridControl2.DataSource = tblDanhSach;
        }

        private void searchLookUpEditSoLo_EditValueChanged(object sender, EventArgs e)
        {
            CreateSearchLookUpCayVai(searchLookUpEditSoLo.EditValue == null ? "" : searchLookUpEditSoLo.EditValue.ToString());
            LoadDataThongTin(searchLookUpEditSoLo.EditValue == null ? "" : searchLookUpEditSoLo.EditValue.ToString());
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CreateSearchLookUpSoLo();
            gridControl2.DataSource = null;
            gridControl21.DataSource = null;
        }

        private void gridView21_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SoLuong" && e.Value.ToString() != "")
            {
                var thucnhap = _dtChiTiet.AsEnumerable()
                       .Where(row => Convert.ToInt32(row["SoKien"]) <= Convert.ToInt32(e.Value))
                       .Sum(row => Convert.ToDecimal(row["SoGhiDauCay"]));
                var theoct = _dtChiTiet.AsEnumerable()
                    .Where(row => Convert.ToInt32(row["SoKien"]) <= Convert.ToInt32(e.Value))
                    .Sum(row => Convert.ToDecimal(row["SoLuongThucTe"]));
                decimal dongia = Convert.ToDecimal(gridView21.GetRowCellValue(e.RowHandle, "DonGia"));
                decimal thanhTien = theoct * dongia;
                gridView21.SetRowCellValue(e.RowHandle, "ThucNhap", thucnhap);
                gridView21.SetRowCellValue(e.RowHandle, "TheoCT", theoct);
                gridView21.SetRowCellValue(e.RowHandle, "ThanhTien", thanhTien);
            }
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = gridControl2.DataSource as DataTable;
            if (tbl == null) return;

            bool allFalse = _dtChiTiet.AsEnumerable().All(row => row.Field<bool>("IsCheck") == false);

            if (allFalse)
            {
                return;
            }
            //DataTable dtSave = new DataTable();
            //dtSave.Columns.Add("SoPhieu", typeof(string));
            //dtSave.Columns.Add("NgayNhapKho", typeof(string));
            //dtSave.Columns.Add("SoLoID", typeof(string));
            //dtSave.Columns.Add("MaNPL", typeof(string));
            //dtSave.Columns.Add("STT", typeof(int));
            //dtSave.Columns.Add("NguoiTao", typeof(string));

            DataTable dtSaveIsNK = new DataTable();
            dtSaveIsNK.Columns.Add("SoPhieu", typeof(string));
            dtSaveIsNK.Columns.Add("SoLoID", typeof(string));
            dtSaveIsNK.Columns.Add("MaNPL", typeof(string));
            dtSaveIsNK.Columns.Add("SoKien", typeof(string));
            dtSaveIsNK.Columns.Add("IsNK", typeof(bool));
            dtSaveIsNK.Columns.Add("NgayNhapKho", typeof(string));
            dtSaveIsNK.Columns.Add("NguoiTao", typeof(string));
            //foreach (DataRow dr in tbl.Rows)
            //{
            //    bool exists = dtSave.AsEnumerable().Any(row =>
            //      row.Field<string>("SoLoID") == dr["SoLoID"].ToString() &&
            //      row.Field<string>("MaNPL") == dr["MaNPL"].ToString());

            //    if (!exists)
            //    {
            //        DataRow _dr = dtSave.NewRow();
            //        _dr["SoPhieu"] = "PNK_" + _stt;
            //        _dr["NgayNhapKho"] = Convert.ToDateTime(dateEditNgayNhapKho.EditValue).ToString("yyyy-MM-dd");
            //        _dr["SoLoID"] = dr["SoLoID"];
            //        _dr["MaNPL"] = dr["MaNPL"];
            //        _dr["STT"] = _stt;
            //        _dr["NguoiTao"] = GlobleData.UserName;
            //        dtSave.Rows.Add(_dr);
            //    }

            //}
            foreach (DataRow dr in _dtChiTiet.Rows)
            {
                if ((bool)dr["IsCheck"] == true)
                {
                    DataRow _dr = dtSaveIsNK.NewRow();
                    _dr["SoPhieu"] = "PNK_" + _stt;
                    _dr["SoLoID"] = dr["SoLoID"];
                    _dr["MaNPL"] = dr["MaNPL"];
                    _dr["SoKien"] = dr["SoKien"];
                    _dr["IsNK"] = dr["IsCheck"];
                    _dr["NgayNhapKho"] = Convert.ToDateTime(dateEditNgayNhapKho.EditValue).ToString("yyyy-MM-dd");
                    _dr["NguoiTao"] = GlobleData.UserName;
                    dtSaveIsNK.Rows.Add(_dr);
                }

            }
            List<DataRow> rowsToDelete = new List<DataRow>();
            //foreach (DataRow dr in dtSave.Rows)
            //{
            //    bool exists = dtSaveIsNK.AsEnumerable().Any(row =>
            //        row.Field<string>("SoLoID") == dr["SoLoID"].ToString() &&
            //        row.Field<string>("MaNPL") == dr["MaNPL"].ToString());

            //    if (!exists) 
            //    {
            //        rowsToDelete.Add(dr);
            //    }
            //}
            //foreach (var row in rowsToDelete)
            //{
            //    row.Delete();
            //}

            //dtSave.AcceptChanges();

            //string url = string.Format("{0}?", URL + "PhieuNhapKho/Post");
            //string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            //if (result.ToLower() != "true")
            //    XtraMessageBox.Show(result);
            string urls = string.Format("{0}?", URL + "PhieuNhapKho/PostIsNK");
            string results = Task.Run(async () => { return await _clientExtension.PostAsync(urls, dtSaveIsNK); }).Result;
            if (results.ToLower() != "true")
                XtraMessageBox.Show(results);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadData(selectedValuesLo.ToString(), selectedValuesMaNPL.ToString());
                LoadDataThongTin(searchLookUpEditSoLo.EditValue == null ? "" : searchLookUpEditSoLo.EditValue.ToString());
                LoadSTT();

            }

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLichSuPhieuNhapKhoV1 frm = new frmLichSuPhieuNhapKhoV1();
            frm.ShowDialog();
            CreateSearchLookUpSoLo();
        }

        private void gridView21_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView21.FocusedColumn.FieldName == "SoLuong")
            {
                int outParse = -1;
                if (e != null && e.Value != null)
                {
                    bool flagParse = int.TryParse(e.Value.ToString(), out outParse);
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Value = 0;
                    }
                    else if (flagParse && outParse < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập > 0!.";
                        return;
                    }
                    else if (!flagParse)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!.";
                        return;
                    }
                    int soLuongNhap;
                    int soLuong;
                    if (int.TryParse(e.Value.ToString(), out soLuongNhap))
                    {
                        soLuong = Convert.ToInt32(gridView21.GetRowCellValue(gridView21.FocusedRowHandle, "SLCL"));
                        if (soLuongNhap > soLuong)
                        {
                            e.Valid = false;
                            e.ErrorText = "Số kiện vượt quá Số kiện Tổng. Vui lòng kiểm tra lại.!";
                        }
                    }
                }
            }
        }

        private void gridView2_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;


            if (column.FieldName == "SoLuong" || column.FieldName == "TheoCT" || column.FieldName == "ThucNhap" || column.FieldName == "DonGia" || column.FieldName == "ThanhTien")
            {
                var value = view.GetFocusedValue();

                if (value != null &&
                    ((value is int && (int)value == 0) ||
                     (value is decimal && (decimal)value == 0) ||
                     (value is double && (double)value == 0) ||
                     (value is float && (float)value == 0)))
                {

                    view.SetFocusedValue(null);
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPBienBanChiTiet frm = new frmERPBienBanChiTiet();
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //selectedValuesSoLo = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditSoLo.Properties.ValueMember)));
            //searchLookUpEditSoLo.EditValue = selectedValuesSoLo;
            //CreateSearchLookUpCayVai(selectedValuesSoLo);


        }

        private void searchLookUpEditSoLo_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //selectedValuessSoLo = string.Join("; ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditSoLo.Properties.DisplayMember)));
            //if (string.IsNullOrEmpty(selectedValuessSoLo.ToString()))
            //{
            //        e.DisplayText = "---Chưa chọn Lô---";
            //}
            //else
            //{
            //    e.DisplayText = selectedValuessSoLo.ToString();
            //}
        }
        string selectedValuesLo = "";
        string selectedValuesMaNPL = "";
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRows.Add(row);

                }
                else
                {
                    selectedRows.Remove(row);

                }
            }
            selectedValuesLo = string.Join(";",
                 gridView1.GetSelectedRows()
                     .Select(rowHandle => gridView1.GetRowCellValue(rowHandle, "SoLoID")).Distinct()
             );
            selectedValuesMaNPL = string.Join(";",
                gridView1.GetSelectedRows()
                    .Select(rowHandle => gridView1.GetRowCellValue(rowHandle, "MaNPL")).Distinct()
            );

            string selectedValuesChiTiet = string.Join(";",
               gridView1.GetSelectedRows()
                   .Select(rowHandle => gridView1.GetRowCellValue(rowHandle, "ChiTiet")).Distinct()

           );
            searchLookUpEditVatTu.EditValue = selectedValuesChiTiet;
            LoadData(selectedValuesLo.ToString(), selectedValuesMaNPL.ToString());
        }

        private void gridView1_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void searchLookUpEditVatTu_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessItems = string.Join("; ", gridView1.GetSelectedRows().Select(rowHandle => gridView1.GetRowCellValue(rowHandle, searchLookUpEditVatTu.Properties.DisplayMember)).Distinct());
            if (string.IsNullOrEmpty(selectedValuessItems.ToString()))
            {
                e.DisplayText = "---Chưa chọn cây vải---";
            }
            else
            {
                e.DisplayText = selectedValuessItems.ToString();
            }
        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }
            //if (currentText.Contains("."))
            //{
            //    int indexOfDot = currentText.IndexOf('.');
            //    string decimalPart = currentText.Substring(indexOfDot + 1);
            //    if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
            //    {
            //        e.Handled = true;
            //        return;
            //    }
            //}
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
            }
        }
        private void repositoryItemTextEditSoKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void gridView21_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            //if (gridView21.FocusedRowHandle >= 0)
            //{
            DataRow focusedDataRow = null;
            int levelGroup = gridView21.GetRowLevel(gridView21.FocusedRowHandle);
            if (levelGroup == 0)
            {
                int childHandle = levelGroup;
                focusedDataRow = gridView21.GetDataRow(childHandle);

            }
            else
            {
                if (gridView21.IsGroupRow(gridView21.FocusedRowHandle))
                {
                    int childRowHandle = gridView21.GetChildRowHandle(gridView21.FocusedRowHandle, 0);
                    focusedDataRow = gridView21.GetDataRow(childRowHandle);
                }
                else
                {
                    focusedDataRow = gridView21.GetDataRow(gridView21.FocusedRowHandle);
                }
            }

            if (focusedDataRow != null)
            {
                string url = string.Format("{0}?soloid={1}&&manpl={2}", URL + "PhieuNhapKho/GetSoKien", focusedDataRow["SoLoID"].ToString(), focusedDataRow["MaNPL"].ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    return;
                }
                if (_dtChiTiet == null || _dtChiTiet.Rows.Count == 0) CreateTableChiTiet();
                if (!_dtChiTiet.Columns.Contains("IsCheck"))
                {
                    _dtChiTiet.Columns.Add("IsCheck", typeof(bool));
                }

                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                foreach (DataRow dr in tbl.Rows)
                {
                    DataRow nr = _dtChiTiet.NewRow();
                    nr["SoLoID"] = dr["SoLoID"];
                    nr["MaNPL"] = dr["MaNPL"];
                    nr["MaVTID"] = dr["MaVTID"];
                    nr["MauVTID"] = dr["MauVTID"];
                    nr["MaMauVT"] = dr["MaMauVT"];
                    nr["SoKien"] = dr["SoKien"];
                    nr["SoLoT"] = dr["SoLoT"];
                    nr["SoLuongThucTe"] = dr["SoLuongThucTe"];
                    nr["SoGhiDauCay"] = dr["SoGhiDauCay"];
                    nr["NW"] = dr["NW"];
                    nr["GW"] = dr["GW"];
                    nr["GhiChu"] = dr["GhiChu"];
                    nr["IsCheck"] = dr["IsCheck"];
                    bool isDup = _dtChiTiet.AsEnumerable()
                    .Any(row => row["SoLoID"].ToString() == dr["SoLoID"].ToString() &&
                                row["MaNPL"].ToString() == dr["MaNPL"].ToString() &&
                                row["SoKien"].ToString() == dr["SoKien"].ToString());
                    if (!isDup)
                        _dtChiTiet.Rows.Add(nr);
                }




                DataTable tblChiTietClone = _dtChiTiet.Clone();
                var _drs = _dtChiTiet.AsEnumerable().Where(row =>
                                 row["MaNPL"].ToString() == focusedDataRow["MaNPL"].ToString()
                               );
                foreach (var a in _drs)
                {
                    tblChiTietClone.ImportRow(a);
                }

                gridControl21.DataSource = tblChiTietClone;
                gridView211.ClearSelection();
                for (int i = 0; i < gridView211.RowCount; i++)
                {
                    var rowView = gridView211.GetDataRow(i);
                    if (rowView != null && rowView["IsCheck"].ToString().ToLower() == "true")
                    {


                        gridView211.SelectRow(i);

                    }
                }
            }
            //}

        }

        private void gridView211_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView view = sender as GridView;
            //int rowHandle = e.ControllerRow;
            //if (rowHandle < 0) return;
            //DataRow dr = gridView211.GetFocusedDataRow();



            //    foreach (DataRow row in _dtChiTiet.Rows)
            //    {
            //        if (dr["MaNPL"].ToString() == row["MaNPL"].ToString() && dr["SoKien"].ToString() == row["SoKien"].ToString())
            //        {
            //            row["IsCheck"] = view.IsRowSelected(rowHandle);

            //        }
            //    }

            //gridControl21.RefreshDataSource();

            //gridView211.RefreshData();
            int[] selectedRowHandles = gridView211.GetSelectedRows();
            foreach (int rowHandle in selectedRowHandles)
            {
                //gridView211.ClearSelection();
                if (rowHandle >= 0)
                {
                    DataRow dr = gridView211.GetDataRow(rowHandle);
                    foreach (DataRow row in _dtChiTiet.Rows)
                    {
                        if (dr["MaNPL"].ToString() == row["MaNPL"].ToString() && dr["SoKien"].ToString() == row["SoKien"].ToString())
                        {
                            row["IsCheck"] = true;
                            break;

                        }

                    }


                }
            }
            DataRow roww = gridView21.GetFocusedDataRow();
            if (selectedRowHandles.Length == 0)
            {
                foreach (DataRow row in _dtChiTiet.Rows)
                {
                    if (roww["MaNPL"].ToString() == row["MaNPL"].ToString())
                    {
                        row["IsCheck"] = false;


                    }

                }
            }
            gridControl21.RefreshDataSource();

            gridView211.RefreshData();

        }

        private void gridView211_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            //GridView view = sender as GridView;

            //// Kiểm tra dòng chẵn hay lẻ và thay đổi màu nền
            //if (e.RowHandle % 2 == 0)
            //{
            //    e.Appearance.BackColor = Color.FromArgb(240, 255, 255);  // Màu nền cho dòng chẵn (Đen)
            //}
            ////else
            ////{
            ////    e.Appearance.BackColor = Color.FromArgb(245, 245, 245);  // Màu nền cho dòng lẻ (Trắng nhạt)
            ////}
        }


    }
}
