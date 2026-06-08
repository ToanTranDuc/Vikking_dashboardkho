using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmKiemMauDauChuyen_NhomKhacPhuc : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();

        public frmKiemMauDauChuyen_NhomKhacPhuc()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            gridView2.OptionsView.ShowGroupPanel = false;
            gridView2.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;

            NhomKhacPhuc.FieldName = "NhomKhacPhuc";
            NguoiTao.FieldName = "NguoiTao";
            NguoiSua.FieldName = "NguoiSua";

            // ✅ Wire up đúng tên BarButtonItem trên toolbar
            btnRefresh.ItemClick += (s, e) => LoadNhomKhacPhuc();
            btnImportExcel.ItemClick += btnImportExcel_ItemClick;

            // ✅ Thêm event cho 3 nút Thêm/Lưu/Xóa - thay tên cho đúng với tên trong designer
            btnThem.ItemClick += (s, e) => btnThem_Click(s, e);
            btnLuu.ItemClick += (s, e) => btnLuu_Click(s, e);
            btnXoa.ItemClick += (s, e) => btnXoa_Click(s, e);

            this.KeyPreview = true;
            this.KeyDown += frmKiemMauDauChuyen_NhomKhacPhuc_KeyDown;
        }

        private void frmKiemMauDauChuyen_NhomKhacPhuc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                SaveNhomKhacPhuc();

            if (e.KeyCode == Keys.F5)
                LoadNhomKhacPhuc();
        }

        private void gridView_CustomDrawRowIndicator(object sender,
            RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadNhomKhacPhuc();
        }

        // =====================
        // LOAD
        // =====================
        private void LoadNhomKhacPhuc()
        {
            string url = URL + "QTY_KiemDauChuyen/Get?action=GetNhomKhacPhuc";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (dt != null && dt.Rows.Count > 0)
                gridControl2.DataSource = dt;
            else
                gridControl2.DataSource = CreateTblNhomKhacPhuc();
        }

        private void SaveNhomKhacPhuc()
        {
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();

            var dtSource = gridControl2.DataSource as DataTable;
            if (dtSource == null || dtSource.Rows.Count == 0) return;

            var dtSave = CreateTblNhomKhacPhuc();
            int sort = 1;
            foreach (DataRow dr in dtSource.Rows)
            {
             
                string nhom = dr["NhomKhacPhuc"]?.ToString()?.Trim() ?? "";
                int id = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);

                if (id == 0 && string.IsNullOrWhiteSpace(nhom)) continue;

                var drSave = dtSave.NewRow();
                drSave["ID"] = id;
                drSave["MaNKP"] = dr["MaNKP"]?.ToString() ?? "";
                drSave["NhomKhacPhuc"] = nhom;
                drSave["NguoiTao"] = dr["NguoiTao"]?.ToString() ?? GlobleData.UserName;
                drSave["NguoiSua"] = GlobleData.UserName;
                drSave["Sort"] = sort++;
                dtSave.Rows.Add(drSave);
            }

            if (dtSave.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để lưu!", "Thông báo");
                return;
            }

            string url = URL + "QTY_KiemDauChuyen/Post?action=PostNhomKhacPhuc";
            var result = Task.Run(async () => await _clientExtension.PostAsync(url, dtSave)).Result;

            if (result.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNhomKhacPhuc();
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            var dtSource = gridControl2.DataSource as DataTable;
            if (dtSource == null)
            {
                dtSource = CreateTblNhomKhacPhuc();
                gridControl2.DataSource = dtSource;
            }

            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();

            var drFocus = gridView2.GetFocusedDataRow();
            var drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            drNew["MaNKP"] = "";
            drNew["NhomKhacPhuc"] = "";
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NguoiSua"] = GlobleData.UserName;
            drNew["Sort"] = dtSource.Rows.Count + 1;

            int index = 0;
            if (dtSource.Rows.Count > 0 && drFocus != null)
                index = dtSource.Rows.IndexOf(drFocus);

            dtSource.Rows.InsertAt(drNew, index + 1);
            gridView2.FocusedRowHandle = index + 1;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            SaveNhomKhacPhuc();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var drFocus = gridView2.GetFocusedDataRow();
            if (drFocus == null) return;

            DialogResult dialogResult = MessageBox.Show(
                "Bạn có chắc muốn xóa nhóm khắc phục này không!",
                "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes) return;

            if (Convert.ToInt32(drFocus["ID"]) == 0)
            {
                var dtSource = gridControl2.DataSource as DataTable;
                dtSource?.Rows.Remove(drFocus);
                return;
            }

            var dtSave = CreateTblNhomKhacPhuc();
            var drSave = dtSave.NewRow();
            drSave["ID"] = drFocus["ID"];
            dtSave.Rows.Add(drSave);

            string url = URL + "QTY_KiemDauChuyen/Post?action=DeleteNhomKhacPhuc";
            var result = Task.Run(async () => await _clientExtension.PostAsync(url, dtSave)).Result;

            if (result.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNhomKhacPhuc();
            }
        }
        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Excel File (*.xlsx)|*.xlsx";
            if (of.ShowDialog() != DialogResult.OK) return;

            DataTable dtExcel = ExcelQCReader.ReadExcel(of.FileName);
            if (dtExcel == null || dtExcel.Rows.Count == 0) return;

            var dtSource = CreateTblNhomKhacPhuc();
            int sort = 1;
            foreach (DataRow dr in dtExcel.Rows)
            {
                string nhom = dr[0]?.ToString().Trim();
                if (string.IsNullOrEmpty(nhom)) continue;

                var drNew = dtSource.NewRow();
                drNew["ID"] = 0;
                drNew["MaNKP"] = "";
                drNew["NhomKhacPhuc"] = nhom;
                drNew["NguoiTao"] = GlobleData.UserName;
                drNew["NguoiSua"] = GlobleData.UserName;
                drNew["Sort"] = sort++;
                dtSource.Rows.Add(drNew);
            }

            gridControl2.DataSource = dtSource;
        }

        private DataTable CreateTblNhomKhacPhuc()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaNKP", typeof(string));
            dt.Columns.Add("NhomKhacPhuc", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            return dt;
        }

        private void gridControl2_Click(object sender, EventArgs e) { }
    }
}