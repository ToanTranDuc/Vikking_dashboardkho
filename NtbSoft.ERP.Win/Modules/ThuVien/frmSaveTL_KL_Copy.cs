using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmSaveTL_KL_Copy : DevExpress.XtraEditors.XtraForm
    {
        private readonly string _styleID, _maHang, _dausizeId, _sizeId, _size, _maND;

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        private HttpClientExtension _clientExtension;

        public DataTable CopiedDataTable { get; private set; }
        public DataTable CopiedQuiCachTable { get; private set; }
        public string GhiChuCopied { get; private set; }

        public frmSaveTL_KL_Copy(string styleID, string maHang, string dausize, string sizeid, string size, string maND)
        {
            InitializeComponent();

            _styleID = styleID;
            _maHang = maHang;
            _dausizeId = dausize;
            _sizeId = sizeid;
            _size = size;
            _maND = maND;

            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            LoadMaHang();
        }

        private void LoadMaHang()
        {
            try
            {
                string url = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetMaHang&Para1=A&Para2=&Para3=&Para4=");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null || dt.Rows.Count == 0) return;
                DataRow[] rowsToRemove = dt.Select($"StyleID = '{_styleID}'");
                foreach (DataRow row in rowsToRemove)
                    row.Delete();
                dt.AcceptChanges();

                searchLookUpEditMH.Properties.DataSource = dt;
                searchLookUpEditMH.Properties.DisplayMember = "MaHang";
                searchLookUpEditMH.Properties.ValueMember = "StyleID";
                searchLookUpEditMH.Properties.NullText = "[Chọn mã hàng để copy]";

                if (dt.Rows.Count > 0)
                    searchLookUpEditMH.EditValue = dt.Rows[0]["StyleID"];
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi load mã hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditMH.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng để copy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string styleID_Source = searchLookUpEditMH.EditValue.ToString();
            string maHang_Source = searchLookUpEditMH.Text;

            try
            {
                string urlTS = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetTLKLCopy&Para1={styleID_Source}");
                string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
                DataTable tblTS = JsonConvert.DeserializeObject<DataTable>(jsonTS);

                if (tblTS == null || tblTS.Rows.Count == 0) return;
                if (!tblTS.Columns.Contains("ChonQC"))
                    tblTS.Columns.Add("ChonQC", typeof(bool));
                if (!tblTS.Columns.Contains("TenQC"))
                    tblTS.Columns.Add("TenQC", typeof(string));
                if (!tblTS.Columns.Contains("Chon"))
                    tblTS.Columns.Add("Chon", typeof(bool));
                foreach (DataRow row in tblTS.Rows)
                {
                    row["Chon"] = true;
                }
                string urlQC = string.Format("{0}", URL + $"CaiDatThongSoKHDT/Get?Action=GetQuiCachCopy&Para1={styleID_Source}");
                string jsonQC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQC); }).Result;
                DataTable tblQC = JsonConvert.DeserializeObject<DataTable>(jsonQC);
                if (tblQC != null && tblQC.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[Copy] Tổng số qui cách: {tblQC.Rows.Count}");
                    foreach (DataRow rowTS in tblTS.Rows)
                    {
                        string sizeID = rowTS["SizeID"]?.ToString() ?? "";
                        string dauSizeID = rowTS["DauSizeID"]?.ToString() ?? "";
                        string maNoiDen = rowTS["MaNoiDen"]?.ToString() ?? "0";
                        var matchingQC = tblQC.AsEnumerable()
                            .Where(r => r["SizeID"]?.ToString() == sizeID
                                     && r["DauSizeID"]?.ToString() == dauSizeID
                                     && (r["MaNoiDen"]?.ToString() ?? "0") == maNoiDen);

                        if (matchingQC.Any())
                        {
                            string tenQC = string.Join(", ", matchingQC
                                .Select(r => r["TenQuiCach"]?.ToString() ?? "")
                                .Where(s => !string.IsNullOrWhiteSpace(s)));

                            rowTS["TenQC"] = tenQC;
                            rowTS["ChonQC"] = !string.IsNullOrWhiteSpace(tenQC);
                        }
                        else
                        {
                            rowTS["TenQC"] = "";
                            rowTS["ChonQC"] = false;
                        }
                    }
                }
                else
                {
                    foreach (DataRow rowTS in tblTS.Rows)
                    {
                        rowTS["TenQC"] = "";
                        rowTS["ChonQC"] = false;
                    }
                }
                //foreach (DataRow row in tblTS.Rows)
                //{
                //    row["StyleID"] = _styleID;
                //    row["MaHang"] = _maHang;
                //}

                //if (tblQC != null && tblQC.Rows.Count > 0)
                //{
                //    foreach (DataRow qcRow in tblQC.Rows)
                //    {
                //        if (tblQC.Columns.Contains("StyleID"))
                //            qcRow["StyleID"] = _styleID;

                //        if (tblQC.Columns.Contains("MaHang"))
                //            qcRow["MaHang"] = _maHang;
                //    }
                //}
                CopiedDataTable = tblTS;
                CopiedQuiCachTable = tblQC ?? new DataTable();
                GhiChuCopied = tblTS.Rows.Count > 0 ? (tblTS.Rows[0]["GhiChu"]?.ToString() ?? "") : "";

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}