using DevExpress.XtraGrid;
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
    public partial class frmPhanTichBOM_ChonMau : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string _makh = string.Empty, _mahang = string.Empty, _masizechung = string.Empty;
        //private Dictionary<string, string> selectRows = new Dictionary<string, string>();
        public string SelectedResult { get; set; }
        public string SelectedResultName { get; set; }
        bool isThemMauVT = false;
        DataRow _dr;
        DataTable tblMau = new DataTable();

        DataTable tblMauTV = new DataTable();
        public DataTable tblGrid = new DataTable();
        public string _mauvtidchung = string.Empty;
        public frmPhanTichBOM_ChonMau(DataRow dr)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            this._dr = dr;
            txtCL.Text = dr["TenNhom"].ToString();
            txtMaVT.Text = dr["MaVT"].ToString();
            txtChiTiet.Text = dr["ChiTiet"].ToString();
            txtKhoVai.Text = dr["KhoVai"].ToString();
            txtTenDVVT.Text = dr["TenDVVT"].ToString();
            txtDinhMuc.Text = dr["DinhMucChung"].ToString();
            txtHaohut.Text = dr["DinhMucHaoHut"].ToString();
            this._mauvtidchung = dr["MauVTIDChung"].ToString();
            this.ActiveControl = button1;
        }
        protected override void OnLoad(EventArgs e)
        {
            //List<string> listMaMau = _mamau.Split('|').Select(s => s.Trim()).ToList();
            //List<string> listTenMau = _tenmau.Split('|').Select(s => s.Trim()).ToList();
            //selectRows = listMaMau.Zip(listTenMau, (maMau, tenMau) => new { maMau, tenMau })
            //                                                 .ToDictionary(x => x.maMau, x => x.tenMau);
            loadMauVT();
            CreateTableGrid();
            loadMauVTTV();
        }
        private void CreateTableGrid()
        {
            tblGrid = new DataTable();
            tblGrid.Columns.Add("MauSP", typeof(string));
            tblGrid.Columns.Add("MauSPID", typeof(string));
            tblGrid.Columns.Add("MauVTID", typeof(string));

            // Duyệt qua từng cột trong DataRow
            foreach (DataColumn col in _dr.Table.Columns)
            {

                if (col.ColumnName.Contains("@Mau@"))
                {
                    DataRow newRow = tblGrid.NewRow();
                    newRow["MauSPID"] = col.ColumnName;  // Tên cột
                    newRow["MauSP"] = col.ColumnName.Split('@')[0].ToString();  // Tên cột
                    newRow["MauVTID"] = _dr[col].ToString();  // Giá trị
                    tblGrid.Rows.Add(newRow);

                }

            }
            if (tblGrid == null || tblGrid.Rows.Count == 0) return;
            gridControl1.DataSource = tblGrid;


        }
        private void loadMauVT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUVT&para1={_dr["MaNhom"].ToString()}&para2={_dr["MaVTID"].ToString()}&para3={_dr["KhoVaiID"].ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblMau == null || tblMau.Rows.Count == 0)
                {
                    repoMauVT.DataSource = null;
                    return;
                }
                repoMauVT.DisplayMember = "MaMauVT";
                repoMauVT.ValueMember = "MauVTID";
                repoMauVT.DataSource = tblMau;

                searchLookUpEditChonNhanh.Properties.DisplayMember = "MaMauVT";
                searchLookUpEditChonNhanh.Properties.ValueMember = "MauVTID";
                searchLookUpEditChonNhanh.Properties.DataSource = tblMau;


                //gridControl1.DataSource = tblMau;
            }

        }
        private void loadMauVTTV()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUVTTVSINGLE";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblMauTV = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblMauTV == null || tblMauTV.Rows.Count == 0)
                {
                    searchLookUpEditChon.Properties.DataSource = null;
                    return;
                }


                searchLookUpEditChon.Properties.DisplayMember = "MaMauVT";
                searchLookUpEditChon.Properties.ValueMember = "MauVTID";
                searchLookUpEditChon.Properties.DataSource = tblMauTV;

            }

        }

        Dictionary<string, HashSet<string>> ParseMaSizeChung(string source)
        {
            var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            var groups = source.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var group in groups)
            {
                var parts = group.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) continue;
                var key = parts[0].Trim();
                var sizes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!result.TryGetValue(key, out var set))
                {
                    set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    result[key] = set;
                }
                foreach (var s in sizes)
                    set.Add(s.Trim());
            }
            return result;
        }
        private void gVMau_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;

            //int[] selectedRowHandles = view.GetSelectedRows();
            //foreach (int rowHandle in selectedRowHandles)
            //{
            //    if (rowHandle >= 0) 
            //    {
            //        object maMauObj = view.GetRowCellValue(rowHandle, "MaMau");
            //        object tenMauObj = view.GetRowCellValue(rowHandle, "TenMau");

            //        if (maMauObj != null && tenMauObj != null)
            //        {
            //            string maMau = maMauObj.ToString().Trim();
            //            string tenMau = tenMauObj.ToString().Trim();

            //        }
            //    }
            //}
        }



        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SelectedResult = BuildResultStringLinq(gridView1);
            SelectedResultName = BuildResultStringLinqName(gridView1);
            this.ActiveControl = button1;
            this.DialogResult = DialogResult.OK;
        }

        private void btnThemMauVT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (isThemMauVT)
            {
                isThemMauVT = false;
                btnThemMauVT.Caption = "Thêm màu vật tư";
                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else
            {
                isThemMauVT = true;

                btnThemMauVT.Caption = "Ẩn thêm màu vật tư";
                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
        }


        string BuildResultStringLinq(GridView view)
        {
            var handles = view.GetSelectedRows()
                .Where(h => h >= 0)
                .Select(h => new
                {
                    MaNhomSize = Convert.ToString(view.GetRowCellValue(h, "MaNhomSize")),
                    MaSize = Convert.ToString(view.GetRowCellValue(h, "MaSize"))
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.MaNhomSize) && !string.IsNullOrWhiteSpace(x.MaSize))
                .GroupBy(x => x.MaNhomSize, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key + ":" + string.Join(",", g.Select(x => x.MaSize).Distinct(StringComparer.OrdinalIgnoreCase)))
                .ToArray();

            return string.Join("; ", handles);
        }


        string BuildResultStringLinqName(GridView view)
        {
            var handles = view.GetSelectedRows()
                .Where(h => h >= 0)
                .Select(h => new
                {
                    MaNhomSize = Convert.ToString(view.GetRowCellValue(h, "NhomSize")),
                    MaSize = Convert.ToString(view.GetRowCellValue(h, "TenSize"))
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.MaNhomSize) && !string.IsNullOrWhiteSpace(x.MaSize))
                .GroupBy(x => x.MaNhomSize, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Key + ":" + string.Join(",", g.Select(x => x.MaSize).Distinct(StringComparer.OrdinalIgnoreCase)))
                .ToArray();

            return string.Join("; ", handles);
        }



        private void searchLookUpEditChonNhanh_EditValueChanged(object sender, EventArgs e)
        {
            foreach (DataRow dr in tblGrid.Rows)
            {
                dr["MauVTID"] = searchLookUpEditChonNhanh.EditValue == null ? "" : searchLookUpEditChonNhanh.EditValue.ToString();
            }
            //int[] selectedRows = gridView1.GetSelectedRows();

            //// Kiểm tra nếu có dòng được chọn
            //if (selectedRows.Length > 0)
            //{
            //    foreach (int rowHandle in selectedRows)
            //    {
            //        // Kiểm tra rowHandle hợp lệ (>= 0 là data row)
            //        if (rowHandle >= 0)
            //        {
            //            // Lấy DataRow tương ứng
            //            DataRow dr = gridView1.GetDataRow(rowHandle);

            //            if (dr != null)
            //            {
            //                dr["MauVTID"] = searchLookUpEditChonNhanh.EditValue == null
            //                    ? ""
            //                    : searchLookUpEditChonNhanh.EditValue.ToString();
            //            }
            //        }
            //    }

            //    // Refresh GridView để hiển thị thay đổi
            //    //gridView1.RefreshData();
            //}

        }
        private HashSet<DataRow> selectedRowsMauVT = new HashSet<DataRow>();

        

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle = e.ControllerRow;
            if (view == null) return;

            if (e.Action == CollectionChangeAction.Add)
            {
                DataRow row = view.GetDataRow(e.ControllerRow);
                if (row != null && !selectedRowsMauVT.Contains(row))
                {
                    selectedRowsMauVT.Add(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DataRow row = view.GetDataRow(e.ControllerRow);
                if (row != null)
                {
                    selectedRowsMauVT.Remove(row);
                }
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                selectedRowsMauVT.Clear();


                int[] selectedRowHandles = view.GetSelectedRows();
                foreach (int rowHandle2 in selectedRowHandles)
                {
                    if (rowHandle2 >= 0)
                    {
                        DataRow row = view.GetDataRow(rowHandle2);
                        if (row != null)
                        {
                            selectedRowsMauVT.Add(row);
                        }
                    }
                }
            }
            string selectedValues = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle2 => searchLookUpEdit1View.GetRowCellValue(rowHandle2, searchLookUpEditChon.Properties.ValueMember)));
            searchLookUpEditChon.EditValue = selectedValues;
            //if (searchLookUpEditChon.EditValue is null) return;

        }

      

        private void searchLookUpEditChon_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            var displayValues = new List<string>();
            for (int i = 0; i < searchLookUpEdit1View.DataRowCount; i++)
            {
                object rowValueObj = searchLookUpEdit1View.GetRowCellValue(i, searchLookUpEditChon.Properties.ValueMember);
                string rowValue = rowValueObj?.ToString();
                if (rowValue != null && selectedRowsMauVT.Any(dr => dr["MauVTID"]?.ToString() == rowValue))
                {
                    string displayValue = searchLookUpEdit1View.GetRowCellValue(i, searchLookUpEditChon.Properties.DisplayMember)?.ToString();
                    if (displayValue != null)
                    {
                        displayValues.Add(displayValue);
                    }
                }
            }

            e.DisplayText = string.Join(";", displayValues);

        }
        private void searchLookUpEdit1View_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            var mauVTIDs = _dr["MauVTIDChung"].ToString()
                     .Split(',')
                     .Select(id => id.Trim())
                     .ToList();
            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            var mauVTID = Convert.ToString(
                view.GetListSourceRowCellValue(e.ListSourceRow, "MauVTID")
            );
            if (mauVTIDs.Contains(mauVTID, StringComparer.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private DataTable createTypeVattuSP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("DinhMucChung", typeof(double));
            dt.Columns.Add("DinhMucChiTiet", typeof(double));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaVTGhep", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private void searchLookUpEdit1View_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "MaMauVT")
            {
                string value1 = Convert.ToString(e.Value1);
                string value2 = Convert.ToString(e.Value2);

                bool isSingleColor1 = string.Equals(value1, "SINGLECOLOR", StringComparison.OrdinalIgnoreCase);
                bool isSingleColor2 = string.Equals(value2, "SINGLECOLOR", StringComparison.OrdinalIgnoreCase);

                if (isSingleColor1 && !isSingleColor2)
                {
                    e.Result = -1; // value1 lên trên
                }
                else if (!isSingleColor1 && isSingleColor2)
                {
                    e.Result = 1; // value2 lên trên
                }
                else
                {
                    e.Result = 0; // giữ nguyên thứ tự
                }

                e.Handled = true;
            }
        }

        private void searchLookUpEdit1View_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0) // Ensure it's a valid row
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view != null)
                {
                    string maMauVT = view.GetRowCellValue(e.RowHandle, "MaMauVT")?.ToString();
                    if (maMauVT == "SINGLECOLOR")
                    {
                        e.Appearance.BackColor = System.Drawing.Color.Red;
                        e.Appearance.ForeColor = System.Drawing.Color.White; // Optional: White text for contrast
                        e.HighPriority = true; // Ensure this style takes precedence
                    }
                }
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            searchLookUpEditChonNhanh.EditValue = null;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            SelectedResult = BuildResultStringLinq(gridView1);
            SelectedResultName = BuildResultStringLinqName(gridView1);
            this.ActiveControl = button1;
            this.DialogResult = DialogResult.OK;
        }

        private void btnSaveMauVT_Click(object sender, EventArgs e)
        {
            DataTable tblSave = createTypeVattuSP();
            if (selectedRowsMauVT.Count == 0) return;
            List<string> mauVtIds = _mauvtidchung.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (DataRow dr in selectedRowsMauVT)
            {
                DataRow _r = tblSave.NewRow();
                _r["ID"] = 0;
                _r["MaNhom"] = _dr["MaNhom"];
                _r["MaVTID"] = _dr["MaVTID"];
                _r["KhoVaiID"] = _dr["KhoVaiID"];
                _r["MaDot"] = _dr["MaDVVT"];
                _r["MauVTID"] = dr["MauVTID"];
                mauVtIds.Add(Convert.ToString(dr["MauVTID"]));
                tblSave.Rows.Add(_r);
            }
            string url = $"{URL}KhoiTaoBOMV1/Post3?Action=POSTMAUVTBOM&para1={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
            if (msResult == "True")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                _mauvtidchung = string.Join(",", mauVtIds);
                loadMauVT();
                ResetSearchLookUpEdit();

            }    
        }
        private void ResetSearchLookUpEdit()
        {
            GridView lookupView = searchLookUpEditChon.Properties.View as GridView;
            if (lookupView == null) return;
            lookupView.BeginUpdate();
            lookupView.ClearSelection();
            lookupView.EndUpdate();
            selectedRowsMauVT.Clear();
            searchLookUpEditChon.EditValue = null;
            searchLookUpEditChon.Properties.View.FocusedRowHandle = GridControl.InvalidRowHandle;
        }
    }
}

