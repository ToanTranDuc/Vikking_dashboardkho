using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.POMuaHang;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmChiPhi : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        DataTable tbl = new DataTable();
        public DataTable _tbl = new DataTable();
        public List<DataRow> lstSelect = new List<DataRow>();
        public DataTable ListExistingCP = new DataTable();
        bool _ismuahang;
        public frmChiPhi()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            CheckPerminsion();
            _ismuahang = false;
        }

        public frmChiPhi(bool muahang = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
            CheckPerminsion();
            _ismuahang = muahang;
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateTableThuVien();
            LoadData();
            CreateSearchlookupcolNhom();
            if (_ismuahang == false) 
            {
                barButtonItem5.Visibility = BarItemVisibility.Never;
                gridView1.OptionsSelection.MultiSelect = false;
                gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            }
        }

        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                //barSubItem1.Enabled = false;
                barButtonItem3.Enabled = false;
                barButtonItem1.Enabled = false;
            }
            //if (!_allowEdit)
            //{
            //    barButtonItem4.Enabled = false;
            //}
            if (!_allowDelete)
            {
                barButtonItem4.Enabled = false;
                //barButtonItem10.Enabled = false;
            }

        }

        private void LoadData()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETCHPPS&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                gridControl1.DataSource = tbl;
            }
            else
            {
                CreateTableThuVien();
                gridControl1.DataSource = null;
            }

            if (ListExistingCP != null && ListExistingCP.Rows.Count > 0)
            {
                for (int i = tbl.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = tbl.Rows[i];

                    string rowMaVT = row["MaChiPhi"]?.ToString().Trim() ?? "";
                    string rowMauVT = row["MaNhom"]?.ToString().Trim() ?? "";
                    bool exists = ListExistingCP.AsEnumerable().Any(x =>
                    {
                        string exMaVT = x["MaChiPhi"]?.ToString().Trim() ?? "";
                        string exMauVT = x["MaNhom"]?.ToString().Trim() ?? "";

                        return exMaVT == rowMaVT
                            && exMauVT == rowMauVT;
                    });

                    if (exists)
                        tbl.Rows.RemoveAt(i);
                }
            }

        }

        private void CreateTableThuVien()
        {
            tbl = new DataTable("tbl");
            tbl.Columns.Add("MaChiPhi", typeof(int));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("ChiPhi", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridControl1.DataSource != null)
                {
                    tbl = gridControl1.DataSource as DataTable;
                }
                this.ActiveControl = simpleButton2;
                DataRow dr = tbl.NewRow();
                dr["MaChiPhi"] = 0;
                tbl.Rows.InsertAt(dr, 0);
                gridControl1.DataSource = tbl;
            }

            catch (Exception ex)
            {


            }
        }


        private void CreateSearchlookupcolNhom()
        {
            string url = $"{URL}NhaCC/GetCP?action=GETCHPPSN&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tblMau = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditmau = new RepositoryItemSearchLookUpEdit();
            rCountryEditmau.DataSource = _tblMau;
            rCountryEditmau.DisplayMember = "TenNhom";
            rCountryEditmau.ValueMember = "MaNhom";
            rCountryEditmau.ShowClearButton = false;
            rCountryEditmau.NullText = "[Chọn giá trị]";

            GridView dvViewmau = rCountryEditmau.View;
            if (dvViewmau.Columns.Count == 0)
            {
                dvViewmau.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewmau.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewmau.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewmau.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewmau.Columns.Add(new GridColumn { FieldName = "MaNhom", Caption = "Mã Nhóm", Name = "colMaHang", Visible = false });
                dvViewmau.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Nhóm", Name = "colTenHang", Visible = true });

            }
            gridColumn3.ColumnEdit = rCountryEditmau;

        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton2;
            if (tbl.Rows.Count > 0 && tbl != null)
            {

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["ChiPhi"] == DBNull.Value || string.IsNullOrWhiteSpace(row["ChiPhi"].ToString()))
                    {
                        int rowIndex = tbl.Rows.IndexOf(row) + 1;
                        XtraMessageBox.Show($"Có dòng bị thiếu thông tin Chi phí!. Vui lòng kiểm tra lại",
                                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string url = string.Format("{0}?", URL + "NhaCC/PostCP");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu Quy đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
                LoadData();
            }
        }



        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton2;
            GridView view = gridView1;
            int[] selectedRows = view.GetSelectedRows();

            // Tạo DataTable kết quả
            DataTable dtSelected = new DataTable();
            dtSelected.Columns.Add("MaChiPhi", typeof(int));
            dtSelected.Columns.Add("MaNhom", typeof(string));
            dtSelected.Columns.Add("ChiPhi", typeof(string));
            dtSelected.Columns.Add("GhiChu", typeof(string));
            // Lặp qua từng dòng được chọn
            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle < 0) continue; // Bỏ qua group row

                DataRow row = view.GetDataRow(rowHandle);

                if (row != null)
                {
                    DataRow newRow = dtSelected.NewRow();
                    newRow["MaChiPhi"] = row["MaChiPhi"];
                    newRow["MaNhom"] = row["MaNhom"];
                    newRow["ChiPhi"] = row["ChiPhi"];
                    newRow["GhiChu"] = row["GhiChu"];
                    dtSelected.Rows.Add(newRow);
                    lstSelect.Add(row);
                }
            }
            _tbl = dtSelected;

            this.DialogResult = DialogResult.OK;
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (e.Action != CollectionChangeAction.Add)
                return;

            GridView view = sender as GridView;
            if (view == null) return;

            DataRow selectedRow = view.GetDataRow(e.ControllerRow);
            if (selectedRow == null) return;

            string url = $"{URL}NhaCC/GetCP?action=GETCHPPS&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(json);
            
            if (tblcheck == null || tblcheck.Rows.Count == 0)
                return;

            string maChiPhi = selectedRow["MaChiPhi"].ToString();
            string maNhom = selectedRow["MaNhom"].ToString();

            bool exists = tblcheck.AsEnumerable().Any(r =>
                r["MaChiPhi"].ToString() == maChiPhi &&
                r["MaNhom"].ToString() == maNhom
            );

            if (!exists)
            {
                // ❌ Bỏ chọn ngay
                view.UnselectRow(e.ControllerRow);

                XtraMessageBox.Show(
                    $"Chi phí được chọn chưa được lưu!, vui lòng lưu trước khi chọn cho phiếu",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnthemnhomcp();
        }

        private void btnthemnhomcp() 
        {
            frmERPNhomCP frm = new frmERPNhomCP();
            frm.ShowDialog();
            frm.FormClosed += (s, e) =>
            {
                CreateSearchlookupcolNhom();
            };
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    int id = Convert.ToInt32(dr["MaChiPhi"].ToString());
                    if (id >= 1 && id <= 6)
                    {
                        XtraMessageBox.Show(
                            "Chi phí gốc không được phép xóa.",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                    //string macl = dr["MaLoaiNCC"].ToString();
                    string urlcheckpmh = $"{URL}NhaCC/GetCP?action=GETCPCKPMH&para1={id}&para2=&para3=&para4=&para5=";
                    string jsoncheckpmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpmh); }).Result;
                    DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheckpmh);

                    if (tblcheck != null && tblcheck.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Chi phí này đã có trong phiếu mua hàng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string urlcheckpbg = $"{URL}NhaCC/GetCP?action=GETCPCKPBG&para1={id}&para2=&para3=&para4=&para5=";
                    string jsoncheckpbg = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpbg); }).Result;
                    DataTable tblcheckpbg = JsonConvert.DeserializeObject<DataTable>(jsoncheckpbg);

                    if (tblcheckpbg != null && tblcheckpbg.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Chi phí này đã được gán cho phiếu báo giá. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string url = string.Format("{0}?Parameter={1}", URL + "NhaCC/DeletecP", id);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "ChiPhi" || e.Column.FieldName == "MaNhom") 
            {
                GridView view = sender as GridView;
                DataRow currentRow = view.GetDataRow(e.RowHandle);
                if (currentRow == null) return;

                string value = e.Value?.ToString().Trim();
                if (string.IsNullOrEmpty(value)) return;

                string manhom = currentRow["MaNhom"]?.ToString();

                bool isDuplicate = tbl.AsEnumerable()
                    .Where(r => r.RowState != DataRowState.Deleted)
                    .Any(r =>
                        !object.ReferenceEquals(r, currentRow) &&
                        r["ChiPhi"] != DBNull.Value &&
                        r["ChiPhi"].ToString().Trim()
                            .Equals(value, StringComparison.OrdinalIgnoreCase)
                        && r["MaNhom"]?.ToString() == manhom
                    );

                if (isDuplicate)
                {
                    XtraMessageBox.Show(
                        "Chi phí này đã tồn tại!",
                        "Trùng dữ liệu",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    // rollback giá trị
                    view.CellValueChanged -= gridView1_CellValueChanged;
                    view.SetRowCellValue(e.RowHandle, e.Column, DBNull.Value);
                    view.CellValueChanged += gridView1_CellValueChanged;
                }
            }

        }
    }
}