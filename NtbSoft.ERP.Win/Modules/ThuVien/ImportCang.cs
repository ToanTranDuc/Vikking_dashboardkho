using DevExpress.XtraEditors;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class ImportCang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        public string SelectedTenCangText { get; private set; } = string.Empty;
        public string SelectedMaCangText { get; private set; } = string.Empty;

        public string InitialTenCangText { get; set; } = string.Empty;
        public string InitialMaCangText { get; set; } = string.Empty;

        private DataTable _tblCang;

        public ImportCang()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadDSCang(false);
            if (!string.IsNullOrWhiteSpace(InitialTenCangText))
            {
                textEdit1.EditValue = InitialTenCangText;
                PreselectExistingCang();

            }

        }

        private void LoadDSCang(bool isFromSave)
        {
            try
            {
                string url = $"{URL}ERPThuVienNK/Get?Action=GETCANG";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCang = JsonConvert.DeserializeObject<DataTable>(json);
                gridControl1.DataSource = _tblCang;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string text = textEdit1.EditValue?.ToString().Trim() ?? "";
            // Chia nhỏ danh sách tên cảng
            var tenCangs = text.Split(':').Select(t => t.Trim()).Where(t => t != "").ToList();

            var maCangs = new List<string>();
            var conflictedCangs = new List<string>();
            foreach (var ten in tenCangs)
            {
                // Tìm mã nếu cảng đã có trong _tblCang
                var found = _tblCang.AsEnumerable()
                    .FirstOrDefault(r => string.Equals(
                        r.Field<string>("TenCang")?.Trim(), ten, StringComparison.OrdinalIgnoreCase));

                if (found != null) 
                {
                    var assignedMaQG = found.Field<string>("ID_QG")?.Trim();

                    // Nếu cảng đã gán cho quốc gia khác
                    if (!string.IsNullOrEmpty(assignedMaQG) &&
                        !string.Equals(assignedMaQG, this.Tag?.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        conflictedCangs.Add($"{ten}");
                        continue;
                    }

                    maCangs.Add(found.Field<string>("MaCang"));
                }
                else 
                {
                    maCangs.Add(""); // Cảng mới => MaCang = 0 để API PostCang tự tạo
                }
                    
            }

            if (conflictedCangs.Count > 0)
            {
                string msg = "Các cảng sau đã được gán cho quốc gia khác:\n\n" +
                             string.Join("\n", conflictedCangs);
                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (conflictedCangs.Count == 0)
            {
                SelectedTenCangText = string.Join(":", tenCangs);
                SelectedMaCangText = string.Join(":", maCangs);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                return;
            }

        }

        private void textEdit1_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            UpdateSelectedCangText();
        }


        private void UpdateSelectedCangText()
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            var names = new List<string>();

            foreach (int handle in selectedRows)
            {
                string name = gridView1.GetRowCellValue(handle, "TenCang")?.ToString();
                if (!string.IsNullOrEmpty(name))
                    names.Add(name);
            }

            textEdit1.EditValue = string.Join(":", names);
        }

        private void PreselectExistingCang()
        {
            try
            {
                if (_tblCang == null || _tblCang.Rows.Count == 0)
                    return;

                // Lấy danh sách tên cảng đang có (tách theo dấu :)
                var existingTenCangs = InitialTenCangText.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(s => s.Trim().ToLower())
                                                         .ToList();

                // Duyệt toàn bộ grid để tick các dòng có tên cảng trùng
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    string tenCang = gridView1.GetRowCellValue(i, "TenCang")?.ToString()?.Trim().ToLower();
                    if (!string.IsNullOrEmpty(tenCang) && existingTenCangs.Contains(tenCang))
                    {
                        gridView1.SelectRow(i);
                    }
                }

                // Cập nhật lại textEdit1 (nếu cần)
                UpdateSelectedCangText();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi đánh dấu cảng đã có: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}