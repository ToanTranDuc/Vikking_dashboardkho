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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmERPNhomCP : DevExpress.XtraEditors.XtraForm
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
        public DataTable ListExistingNCP = new DataTable();
        public frmERPNhomCP()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }



        protected override void OnLoad(EventArgs e)
        {
            CreateTableThuVien();
            LoadData();
        }

        private void LoadData()
        {
            string url = $"{URL}LichSuPBG/GetNCP?action=GETNHOMCP&para1=&para2=&para3=&para4=&para5=";
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


        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridControl1.DataSource != null)
                {
                    tbl = gridControl1.DataSource as DataTable;
                }
                this.ActiveControl = simpleButton1;
                DataRow dr = tbl.NewRow();
                tbl.Rows.InsertAt(dr, 0);
                gridControl1.DataSource = tbl;
            }

            catch (Exception ex)
            {


            }
        }

        private void CreateTableThuVien()
        {
            tbl = new DataTable("tbl");
            tbl.Columns.Add("MaNhom", typeof(int));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView3.GetFocusedDataRow();
                    if (dr == null) return;
                    string id = dr["MaNhom"].ToString();
                    //string macl = dr["MaLoaiNCC"].ToString();
                    string urlcheckpmh = $"{URL}LichSuPBG/GetNCP?action=GetCheckNhomCP&para1={id}&para2=&para3=&para4=&para5=";
                    string jsoncheckpmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpmh); }).Result;
                    DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheckpmh);

                    if (tblcheck != null && tblcheck.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("nhóm chi phí này đã phát sinh chi phí phiếu. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    string url = string.Format("{0}?para1={1}", URL + "LichSuPBG/Delete", id);
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

        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "TenNhom")
            {
                GridView view = sender as GridView;
                DataRow currentRow = view.GetDataRow(e.RowHandle);
                if (currentRow == null) return;

                string value = e.Value?.ToString().Trim();
                if (string.IsNullOrEmpty(value)) return;

                string tennhom = currentRow["TenNhom"]?.ToString();

                bool isDuplicate = tbl.AsEnumerable()
                    .Where(r => r.RowState != DataRowState.Deleted)
                    .Any(r =>
                        !object.ReferenceEquals(r, currentRow) &&
                         r["TenNhom"]?.ToString() == tennhom
                    );

                if (isDuplicate)
                {
                    XtraMessageBox.Show(
                        "Nhóm chi phí này đã tồn tại!",
                        "Trùng dữ liệu",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    // rollback giá trị
                    view.CellValueChanged -= gridView3_CellValueChanged;
                    view.SetRowCellValue(e.RowHandle, e.Column, DBNull.Value);
                    view.CellValueChanged += gridView3_CellValueChanged;
                }
            }
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            if (tbl.Rows.Count > 0 && tbl != null)
            {

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["TenNhom"] == DBNull.Value || string.IsNullOrWhiteSpace(row["TenNhom"].ToString()))
                    {
                        int rowIndex = tbl.Rows.IndexOf(row) + 1;
                        XtraMessageBox.Show($"Có dòng bị thiếu thông tin nhóm chi phí!. Vui lòng kiểm tra lại",
                                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string url = string.Format("{0}?", URL + "LichSuPBG/Post");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
                LoadData();
            }
        }


        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
        }
    }
}