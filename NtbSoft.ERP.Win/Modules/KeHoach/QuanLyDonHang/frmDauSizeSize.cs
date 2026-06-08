using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
    public partial class frmDauSizeSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh = string.Empty, _mahang = string.Empty, _mavtID = string.Empty, _mauID = string.Empty;
        private string selectedValuesDauSize = string.Empty, selectedValuessDauSize = string.Empty;
        private string selectedValuesSize = string.Empty, selectedValuessSize = string.Empty;
        DataTable _dt = new DataTable();
        public frmDauSizeSize(string makh, string mahang, string mavtid, string mauid)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dt = new DataTable();
            this._makh = makh;
            this._mahang = mahang;
            this._mavtID = mavtid;
            this._mauID = mauid;
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookup();
            LoadData();

        }

        private void CreateSearchLookup()
        {
            try
            {
                string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}", URL + "PhanTichBom/GetDauSize", _makh.ToString() == "" ? "" : _makh.ToString(),
                          _mahang.ToString() == "" ? "" : _mahang.ToString(), _mavtID.ToString() == "" ? "" : _mavtID.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditDauSize.Properties.DataSource = tbl;
                searchLookUpEditDauSize.Properties.ValueMember = "MaNhomSize";
                searchLookUpEditDauSize.Properties.DisplayMember = "NhomSize";
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadData()
        {
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}", URL + "PhanTichBom/GetChiTietSize", _makh.ToString(),
                     _mahang.ToString(), _mavtID.ToString(), _mauID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (_dt.Rows.Count > 0)
            {
                createTable(_dt);
                gridControl1.DataSource = _dt;

            }
            else
            {
                gridControl1.DataSource = null;
            }
        }
        private void SetCheckedDauSize()
        {
            for (int i = 0; i < searchLookUpEdit1View.RowCount; i++)
            {
                string checkMauValue = searchLookUpEdit1View.GetRowCellValue(i, "CheckDS").ToString();
                if (checkMauValue != null && checkMauValue.ToString() == "1")
                {
                    searchLookUpEdit1View.SelectRow(i);
                }
                else
                {
                    searchLookUpEdit1View.UnselectRow(i);
                }
            }

        }
        private void SetCheckedSize()
        {
            for (int i = 0; i < searchLookUpEdit2View.RowCount; i++)
            {
                string checkMauValue = searchLookUpEdit2View.GetRowCellValue(i, "CheckSize").ToString();
                if (checkMauValue != null && checkMauValue.ToString() == "1")
                {
                    searchLookUpEdit2View.SelectRow(i);
                }
                else
                {
                    searchLookUpEdit2View.UnselectRow(i);
                }
            }
        }
        private void searchLookUpEdit2View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            selectedValuesSize = string.Join(";", searchLookUpEdit2View.GetSelectedRows().Select(rowHandle => searchLookUpEdit2View.GetRowCellValue(rowHandle, searchLookUpEditSize.Properties.ValueMember)));
            searchLookUpEditSize.EditValue = selectedValuesSize;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            List<string> _lstDauSize = searchLookUpEditDauSize.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> _lstDauSizeID = selectedValuesDauSize.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> _lstSize = searchLookUpEditSize.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> _lstMaSize = selectedValuesSize.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (_dt.Rows.Count == 0)
            {
                if (!_dt.Columns.Contains("DauSizeID"))
                    _dt.Columns.Add("DauSizeID", typeof(string));

                if (!_dt.Columns.Contains("DauSize"))
                    _dt.Columns.Add("DauSize", typeof(string));
            }

            if (_lstMaSize.Count == 0 || _lstDauSizeID.Count == 0) return;
            if (_lstSize[0].ToString() == "Chưa chọn Size") return;
            for (int i = 0; i < _lstMaSize.Count; i++)
            {
                string columnName = string.Format("{0}@Size@{1}", _lstSize[i], _lstMaSize[i]);
                if (!_dt.Columns.Contains(columnName))
                {
                    _dt.Columns.Add(columnName, typeof(string));
                }
            }
            var columnsToDelete = new List<DataColumn>();
            foreach (DataColumn column in _dt.Columns)
            {
                string[] parts = column.ColumnName.Split('@');
                if (parts.Length != 3 || !_lstSize.Contains(parts[0]) || !_lstMaSize.Contains(parts[2]))
                {
                    columnsToDelete.Add(column);
                }
            }
            int columnCount = 0;

            foreach (DataColumn column in columnsToDelete)
            {
                if (columnCount >= 2)
                {
                    _dt.Columns.Remove(column);
                }
                columnCount++;
            }
            for (int j = 0; j < _lstDauSizeID.Count; j++)
            {
                bool exists = false;
                foreach (DataRow row in _dt.Rows)
                {
                    if (row["DauSizeID"].ToString() == _lstDauSizeID[j].ToString() &&
                        row["DauSize"].ToString() == _lstDauSize[j].ToString())
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    DataRow _dr = _dt.NewRow();
                    _dr["DauSizeID"] = _lstDauSizeID[j].ToString();
                    _dr["DauSize"] = _lstDauSize[j].ToString();
                    _dt.Rows.Add(_dr);
                }
            }
            for (int i = _dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = _dt.Rows[i];
                if (!_lstDauSizeID.Contains(row["DauSizeID"].ToString()))
                {
                    _dt.Rows.RemoveAt(i);
                }
            }
            createTable(_dt);
            gridControl1.DataSource = _dt;
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            GridView view = bandedGridView1;
            int focusedRowHandle = view.FocusedRowHandle;
            if (focusedRowHandle < 0)
                return;
            foreach (GridColumn column in view.Columns)
            {
                if (column.FieldName.Contains("@"))
                {
                    var value = view.GetRowCellValue(focusedRowHandle, column);
                    foreach (GridColumn nextColumn in view.Columns)
                    {
                        if (nextColumn.FieldName.Contains("@"))
                        {
                            view.SetRowCellValue(focusedRowHandle, nextColumn, value);
                        }
                    }
                    break;
                }
            }
        }

        private void createTable(DataTable tab)
        {
            bandedGridView1.OptionsView.AllowCellMerge = false;
            GridBand parentBand = bandedGridView1.Bands["gridBandSize"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView1.Columns.Count;)
                {
                    if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView1.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }
            int demColIndex = -1;

            foreach (DataColumn column in tab.Columns)
            {
                demColIndex++;
                if (demColIndex > 1 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = true;
                    col.Visible = true;
                    col.Width = 75;
                    // col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "{0:0.####}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 75;
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }
        private void bandedGridView1_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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

        private void bandedGridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.Contains("@"))
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
                        if (decimalPlaces > 4)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập tối đa 4 chữ số sau dấu thập phân!";
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

        private void searchLookUpEditSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessSize = string.Join(";", searchLookUpEdit2View.GetSelectedRows().Select(rowHandle => searchLookUpEdit2View.GetRowCellValue(rowHandle, searchLookUpEditSize.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessSize.ToString()))
            {
                e.DisplayText = "Chưa chọn Size";
            }
            else
            {
                e.DisplayText = selectedValuessSize.ToString();
            }

        }

        private void searchLookUpEditDauSize_Popup(object sender, EventArgs e)
        {
            SetCheckedDauSize();
        }

        private void searchLookUpEditSize_Popup(object sender, EventArgs e)
        {
            SetCheckedSize();
        }
        private void CreateSearchLookupSize()
        {
            string url = string.Format("{0}?makh={1}&&mahang={2}&&dausize={3}&&mavtID={4}&&mauID={5}", URL + "PhanTichBom/GetSize", _makh.ToString() == "" ? "" : _makh.ToString(),
            _mahang.ToString() == "" ? "" : _mahang.ToString(), searchLookUpEditDauSize.EditValue == null ? "" : searchLookUpEditDauSize.EditValue.ToString(),
            _mavtID.ToString() == "" ? "" : _mavtID.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditSize.Properties.DataSource = tbl;
            searchLookUpEditSize.Properties.ValueMember = "MaSize";
            searchLookUpEditSize.Properties.DisplayMember = "SizeSanXuat";
        }
        private void btnBangSize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBangSize frm = new frmBangSize();
            frm.ShowDialog();
            CreateSearchLookup();
            CreateSearchLookupSize();
        }

        private void searchLookUpEditDauSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            selectedValuessDauSize = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditDauSize.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessDauSize.ToString()))
            {
                e.DisplayText = "Chưa chọn InSeam";
            }
            else
            {
                e.DisplayText = selectedValuessDauSize.ToString();
            }
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            selectedValuesDauSize = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditDauSize.Properties.ValueMember)));
            searchLookUpEditDauSize.EditValue = selectedValuesDauSize;
            CreateSearchLookupSize();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dtTable = this.gridControl1.DataSource as DataTable;
            DataTable _dtSave = new DataTable();
            _dtSave.Columns.Add("MaKH", typeof(string));
            _dtSave.Columns.Add("MaHang", typeof(string));
            _dtSave.Columns.Add("MaVTID", typeof(string));
            _dtSave.Columns.Add("MauID", typeof(string));
            _dtSave.Columns.Add("DauSizeID", typeof(string));
            _dtSave.Columns.Add("SizeID", typeof(string));
            _dtSave.Columns.Add("DinhMuc", typeof(float));
            for (int i = 0; i <= dtTable.Rows.Count - 1; i++)
            {
                for (int j = 2; j <= dtTable.Columns.Count - 1; j++)
                {
                    DataRow _dr = _dtSave.NewRow();
                    string sizeSX = string.Empty;
                    string[] arrNewHeader = dtTable.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string sizeID = arrNewHeader[0];
                    sizeID = arrNewHeader[2];
                    string dinhmuc = dtTable.Rows[i][j].ToString();
                    _dr["MaKH"] = _makh.ToString();
                    _dr["MaHang"] = _mahang.ToString();
                    _dr["MaVTID"] = _mavtID.ToString();
                    _dr["MauID"] = _mauID.ToString();
                    _dr["DauSizeID"] = dtTable.Rows[i][0].ToString();
                    _dr["SizeID"] = sizeID;
                    if (System.Text.RegularExpressions.Regex.IsMatch(dinhmuc, "[0-9]"))
                    {
                        string strValue = System.Text.RegularExpressions.Regex.Replace(dinhmuc, "[^0-9a-zA-Z]+", "_");

                        _dr["DinhMuc"] = Math.Round(Convert.ToDecimal(dinhmuc), 4);
                    }
                    else
                    {
                        _dr["DinhMuc"] = 0;
                    }
                    _dtSave.Rows.Add(_dr);

                }
            }

            string url = string.Format("{0}", URL + "PhanTichBom/PostSizeSP");
            string save = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
            if (string.Compare(save, "True") != 0)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                clsWaitForm.ShowSuccessForm(this, 3000);
                this.Close();
            }
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                GridView view = bandedGridView1;
                object DauSizeID = null;
                if (view.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                    DauSizeID = view.GetRowCellValue(childHandle, bandedGridColumn1);
                }
                else
                {
                    DauSizeID = view.GetFocusedRowCellValue(bandedGridColumn1);
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa dòng này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}&&dausize={5}", URL + "PhanTichBom/Delete", _makh.ToString() == "" ? "" : _makh.ToString(),
                    _mahang.ToString() == "" ? "" : _mahang.ToString(), _mavtID.ToString() == "" ? "" : _mavtID.ToString(),
                    _mauID.ToString() == "" ? "" : _mauID.ToString(), DauSizeID == null ? "" : DauSizeID.ToString());
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa dòng đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
