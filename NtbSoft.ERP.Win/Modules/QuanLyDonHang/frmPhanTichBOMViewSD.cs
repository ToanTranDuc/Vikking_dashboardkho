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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOMViewSD : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh, _mahang, _madot, _username = string.Empty;
        public bool HasData { get; private set; }
        private DataTable _dtRows;
        public frmPhanTichBOMViewSD(string makh, string mahang, string madot, DataTable dtRows, string username)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _makh = makh;
            _mahang = mahang;
            _madot = madot;
            _dtRows = dtRows;
            _username = username;
            LoadSoDo();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!HasData)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        private static DataTable CreateDataTable()
        {
            var dt = new DataTable();
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
            dt.Columns.Add("IsAllSize", typeof(bool));
            dt.Columns.Add("DinhMucKH", typeof(double));
            dt.Columns.Add("MaDVCL", typeof(string));
            dt.Columns.Add("MaDVDM", typeof(string));
            dt.Columns.Add("IsPS", typeof(int));
            return dt;
        }

        public static DataTable BuildRowsTable(string makh, string mahang, string madot, IEnumerable<DataRow> rows)
        {
            var dt = CreateDataTable();
            foreach (var row in rows)
            {
                string mauVTIDChung = row["MauVTIDChung"]?.ToString() ?? "";
                string[] mauVTIDs = mauVTIDChung
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .ToArray();
                if (mauVTIDs.Length == 0)
                    mauVTIDs = new[] { "" };

                foreach (string mauVTID in mauVTIDs)
                {
                    var nr = dt.NewRow();
                    nr["ID"] = 0;
                    nr["MaKH"] = makh;
                    nr["MaHang"] = mahang;
                    nr["MaDot"] = madot;
                    nr["MaNhom"] = row["MaNhom"]?.ToString() ?? "";
                    nr["MaVTID"] = row["MaVTID"]?.ToString() ?? "";
                    nr["MauVTID"] = mauVTID;
                    nr["KhoVaiID"] = row["KhoVaiID"]?.ToString() ?? "";
                    nr["MaCode"] = row["MaCode"]?.ToString() ?? "";
                    dt.Rows.Add(nr);
                }
            }
            return dt;
        }
        private static string ResolveMauVTID(DataRow row)
        {
            var fromChung = row["MauVTIDChung"]?.ToString().Split(',').FirstOrDefault(x => !string.IsNullOrEmpty(x));
            if (!string.IsNullOrEmpty(fromChung))
                return fromChung;
            foreach (DataColumn col in row.Table.Columns)
            {
                if (!col.ColumnName.Contains("@Mau@")) continue;
                var val = row[col]?.ToString();
                if (!string.IsNullOrEmpty(val))
                    return val;
            }
            return "";
        }
        private void LoadSoDo()
        {
            string url = $"{URL}KhoiTaoBOMV1/GetSD?action=GetDLSoDoKH";
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtRows); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                HasData = false;
                return;
            }
            HasData = true;
            gridControl1.DataSource = tbl;
        }

        private void btnXacNhan_ItemClick(object sender,DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult confirm = XtraMessageBox.Show(
                "Bạn có chắc chắn muốn xóa sơ đồ của các vật tư này không." +
                "\n Lưu ý: Tất cả sơ đồ của vật tư này sẽ mất và không thể khôi phục!" +
                "\n Người dùng sẽ chịu hoàn toàn trách nhiệm này!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            string urlDelete =$"{URL}KhoiTaoBOMV1/Post3?action=DeleteSD&para9={GlobleData.UserName}";
            string result = Task.Run(async () => await _clientExtension.PostAsync(urlDelete, _dtRows)).Result;
            if (result?.ToLower() != "true")
            {
                XtraMessageBox.Show("Xóa sơ đồ thất bại. Vui lòng thực hiện lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
