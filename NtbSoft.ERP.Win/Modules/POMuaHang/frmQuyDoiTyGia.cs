using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmQuyDoiTyGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable gridControl1Table;
        private DataTable gridControl2Table;
        private DataTable tienteTable;
        private DataTable tygiaTable;
        private DataTable tygiatheoAPITable;

        private DateTime tygiaDate;
        private TimeSpan tygiaTime;

        public frmQuyDoiTyGia()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            loadComboBoxEdit1();

            getTienTe();
            calcTienTe();
            loadGridControl1();

            //await hexarateAPI();
            tygiaDate = DateTime.Today;
            tygiatheoNgay.EditValue = tygiaDate;
            tygiaTime = new TyGiaConfig().loadTyGiaTime();
            tygiatheoGio.EditValue = tygiaTime;
            repositoryItemDateEdit2.MaxValue = tygiaDate;

            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
        }



        private void loadComboBoxEdit1()
        {
            comboBoxEdit1.Properties.Items.Add("VND");
            //comboBoxEdit1.Properties.Items.Add("Nhà cung cấp");

            // Chọn item mặc định
            comboBoxEdit1.SelectedIndex = 0;
        }



        private void loadGridControl1()
        {

            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.ExpandAllGroups();
        }
        private void getTienTe()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "gettiente";
            string urlGetListDataTable = URL + "TyGiaMH/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            tienteTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!tienteTable.Columns.Contains("generatedID"))
                tienteTable.Columns.Add("generatedID");
            foreach (DataRow row in tienteTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void calcTienTe()
        {
            if (tienteTable == null || tienteTable.Columns.Count <= 0 || tienteTable.Rows.Count == 0) return;
            DataTable dt = tienteTable.Copy();           
            foreach (DataRow row in dt.Rows)
            {
                //row["TonKho"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
            }
            gridControl1Table = dt.Copy();
        }
        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisWord,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                e.Graphics.DrawString(e.Info.Caption, e.Appearance.Font, textBrush, e.Info.Bounds, sf);
            }

            e.Handled = true;
        }



        private void loadGridControl2()
        {

            gridControl2.DataSource = gridControl2Table;
            foreach (GridColumn col in gridView2.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView2.ExpandAllGroups();
        }
        private void getTyGia()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "gettygia";
            request.Parameter6 = tygiaDate;
            string urlGetListDataTable = URL + "TyGiaMH/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            tygiaTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!tygiaTable.Columns.Contains("generatedID"))
                tygiaTable.Columns.Add("generatedID");
            if (!tygiaTable.Columns.Contains("MaTienTe"))
                tygiaTable.Columns.Add("MaTienTe");
            if (!tygiaTable.Columns.Contains("BanTienMat"))
                tygiaTable.Columns.Add("BanTienMat", typeof(decimal));
            if (!tygiaTable.Columns.Contains("NgayTao"))
                tygiaTable.Columns.Add("NgayTao", typeof(DateTime));
            foreach (DataRow row in tygiaTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void calcTyGia()
        {
            if (tygiaTable == null || tygiaTable.Columns.Count <= 0 || tygiaTable.Rows.Count == 0) return;
            DataTable dt = tygiaTable.Copy();
            foreach (DataRow row in dt.Rows)
            {
                row["BanTienMat"] = XuLyVTUnits.SmartTryParse<decimal>(row["BanTienMat"].ToString());
            }
            gridControl2Table = dt.Copy();
        }
        private void gridView2_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisWord,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                e.Graphics.DrawString(e.Info.Caption, e.Appearance.Font, textBrush, e.Info.Bounds, sf);
            }

            e.Handled = true;
        }



        private void saveTyGia()
        {
            var request = XuLyVTRequestPost.createDefault("TyGia");
            request.Action = "createtygia";
            foreach(DataRow row in tygiatheoAPITable.Rows)
            {
                DataRow newRow = request.TypeTable.NewRow();
                newRow["MaTienTe"] = row["MaTienTe"];
                newRow["BanTienMat"] = row["BanTienMat"];
                newRow["NgayTao"] = row["NgayTao"];
                request.TypeTable.Rows.Add(newRow);
            }
            string urlGetListDataTable = URL + "TyGiaMH/Post";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
        }
        private async Task hexarateAPI()
        {
            gridView2.ShowLoadingPanel();
            try
            {
                tygiatheoAPITable = new DataTable();
                tygiatheoAPITable.Columns.Add("MaTienTe", typeof(string));
                tygiatheoAPITable.Columns.Add("BanTienMat", typeof(decimal));
                tygiatheoAPITable.Columns.Add("NgayTao", typeof(DateTime));
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                foreach (DataRow tienteRow in tienteTable.Rows)
                {
                    string target = tienteRow["MaTienTe"].ToString();
                    if (target == "VND") continue;
                    string url = $"https://hexarate.paikama.co/api/rates/{target}/VND/{date}";
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetStringAsync(url);
                        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response);
                        if (apiResponse.data != null)
                        {
                            DataRow newRow = tygiatheoAPITable.NewRow();
                            newRow["MaTienTe"] = tienteRow["MaTienTe"];
                            newRow["BanTienMat"] = XuLyVTUnits.SmartTryParse<decimal>(apiResponse.data.mid);
                            newRow["NgayTao"] = DateTime.Now;
                            tygiatheoAPITable.Rows.Add(newRow);
                        }
                    }
                }
                DataTable dtr = tygiatheoAPITable.Copy();
                saveTyGia();
            }
            catch (Exception ex)
            {
                gridView2.HideLoadingPanel();
            }
            finally
            {
                gridView2.HideLoadingPanel();
            }
        }
        public class ApiResponse 
        { 
            public int status_code { get; set; } 
            public CurrencyData data { get; set; } 
        }
        public class CurrencyData 
        {
            [Newtonsoft.Json.JsonProperty("base")] 
            public string BaseCurrency { get; set; } 
            public string target { get; set; } 
            public decimal mid { get; set; } 
            public int unit { get; set; } 
            public DateTime timestamp { get; set; } }


        public class WebClientWithTimeout : WebClient
        {
            public int Timeout { get; set; } = 5000; // 5 giây

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request != null)
                    request.Timeout = Timeout;
                return request;
            }
        }
        public void googleSheetAPI()
        {
            string url = "https://docs.google.com/spreadsheets/d/12ijCNu7liAAqdzicy65wRu1PopCAgxLu22LWZnHKMfs/export?format=csv";
            string csvString;
            try
            {
                using (var client = new WebClientWithTimeout { Timeout = 10000 })
                {
                    client.Encoding = Encoding.UTF8;
                    csvString = client.DownloadString(url);
                }
            }
            catch (WebException)
            {
                XtraMessageBox.Show(
                    "Không thể lấy tỷ giá (không có mạng hoặc Google Finance không phản hồi).",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    "Lỗi khi lấy tỷ giá:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            tygiatheoAPITable = new DataTable();
            tygiatheoAPITable.Columns.Add("MaTienTe", typeof(string));
            tygiatheoAPITable.Columns.Add("BanTienMat", typeof(decimal));
            tygiatheoAPITable.Columns.Add("NgayTao", typeof(DateTime));
            using (var reader = new StringReader(csvString))
            {
                string line;
                bool isHeader = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Bỏ header
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    var parts = line.Split(',');

                    if (parts.Length < 3)
                        continue;

                    var row = tygiatheoAPITable.NewRow();
                    row["MaTienTe"] = parts[0].Trim();
                    row["NgayTao"] = DateTime.Now;
                    row["BanTienMat"] = decimal.Parse(
                        parts[2].Trim(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture
                    );

                    tygiatheoAPITable.Rows.Add(row);
                }
            }
            DataTable dt = tygiatheoAPITable.Copy();
        }
        private void laytygiaBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (getTyGiaHomNay())
            {
                XtraMessageBox.Show("Đã có tỷ giá mới nhất", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            googleSheetAPI();
            saveTyGia();

            clsWaitForm.ShowSuccessForm(this, 3000);

            gridView2.ShowLoadingPanel();
            getTyGia();
            calcTyGia();
            loadGridControl2();
            gridView2.HideLoadingPanel();
        }
        private bool getTyGiaHomNay()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "gettygia";
            request.Parameter6 = DateTime.Today;
            string urlGetListDataTable = URL + "TyGiaMH/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void tygiatheoNgay_EditValueChanged(object sender, EventArgs e)
        {
            tygiaTable = null;
            gridControl2Table = null;
            tygiaDate = (DateTime)tygiatheoNgay.EditValue;
            getTyGia();
            calcTyGia();
            loadGridControl2();
        }

        private async void tygiatheoGio_EditValueChanged(object sender, EventArgs e)
        {
            if (tygiatheoGio.EditValue == null)
                return;

            DateTime? dt = tygiatheoGio.EditValue as DateTime?;
            if (!dt.HasValue)
                return;

            TimeSpan newTime = dt.Value.TimeOfDay;

            // chặn giờ không hợp lệ
            if (newTime == TimeSpan.Zero)
                return;

            if (newTime == tygiaTime)
                return;
            if (newTime == tygiaTime) return;
            DialogResult messResult = MessageBox.Show(
                    $"Bạn có chắn chắn thay đổi giờ lấy tỷ giá hàng ngày không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (messResult != DialogResult.Yes)
            {
                tygiatheoGio.EditValue = tygiaTime;
                return;
            }
            
            saveTyGiaTime(newTime);
            await TyGiaSetUp.updateTrigger(newTime);
            clsWaitForm.ShowSuccessForm(this, 3000);
        }
        private static void saveTyGiaTime(TimeSpan time)
        {
            var config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);

            config.AppSettings.Settings["TyGiaTime"].Value =
                time.ToString(@"hh\:mm");

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}